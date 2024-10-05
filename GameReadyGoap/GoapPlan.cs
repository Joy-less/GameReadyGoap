using Priority_Queue;

namespace GameReadyGoap;

/// <summary>
/// A sequence of actions a <see cref="GoapAgent"/> could perform to reach a <see cref="GoapGoal"/>.
/// </summary>
public class GoapPlan {
    /// <summary>
    /// The agent the plan was created for.
    /// </summary>
    public required GoapAgent Agent;
    /// <summary>
    /// The goal the plan was created for.
    /// </summary>
    public required GoapGoal Goal;
    /// <summary>
    /// The actions that should be performed to reach the goal.
    /// </summary>
    public required List<GoapAction> Actions;
    /// <summary>
    /// The agent's predicted states after the plan is performed.
    /// </summary>
    public required Dictionary<object, object?> PredictedStates;
    /// <summary>
    /// If true, the plan won't reach the goal but will get the agent closer to it.
    /// </summary>
    public required bool IsBestEffort;

    /// <summary>
    /// Finds a plan that reaches the goal using the A* algorithm.
    /// </summary>
    public static GoapPlan? Find(GoapAgent Agent, GoapGoal Goal, GoapPlanSettings? Settings = null) {
        Settings ??= new GoapPlanSettings();

        // Create queue that puts cheapest steps first
        SimplePriorityQueue<GoapStep, double> OpenQueue = [];

        // Create first step from initial states
        GoapStep FirstStep = new() {
            Previous = null,
            Action = null, 
            PredictedStates = Agent.States.ToDictionary(),
            Cost = 0,
        };
        OpenQueue.Enqueue(FirstStep, 0);

        // Track cost to reach each state
        Dictionary<Dictionary<object, object?>, double> StateCosts = new(new DictionaryComparer<object, object?>());
        // Track best step
        GoapStep BestStep = FirstStep;

        // Repeatedly find next steps
        for (int Iteration = 0; Iteration < Settings.Value.MaxIterations; Iteration++) {
            // Get most promising step
            if (!OpenQueue.TryDequeue(out BestStep)) {
                break;
            }

            // Plan found
            if (Goal.IsReached(BestStep.PredictedStates)) {
                return new GoapPlan() {
                    Agent = Agent,
                    Goal = Goal,
                    Actions = BestStep.GetActions(),
                    PredictedStates = BestStep.PredictedStates,
                    IsBestEffort = false,
                };
            }

            // Check possible continuations
            foreach (GoapAction Action in Agent.GetValidActions(BestStep.PredictedStates)) {
                // Create step to continue most promising step
                GoapStep NextStep = new() {
                    Previous = BestStep,
                    Action = Action,
                    PredictedStates = Action.PredictStates(BestStep.PredictedStates),
                    Cost = Action.Cost(Agent) + (BestStep.Previous?.Cost ?? 0),
                };

                // Get heuristic distance to goal
                double HeuristicDistance = NextStep.EstimateDistance(Goal);
                if (HeuristicDistance > Settings.Value.MaxDistanceEstimate) {
                    continue;
                }
                double TotalCost = NextStep.Cost + HeuristicDistance;

                // Skip if there's a cheaper path to this state already
                if (StateCosts.TryGetValue(NextStep.PredictedStates, out double CurrentCost) && CurrentCost <= TotalCost) {
                    continue;
                }
                // Otherwise set as cheapest path
                else {
                    StateCosts[NextStep.PredictedStates] = TotalCost;
                }

                // Submit step in order of priority
                OpenQueue.EnqueueWithoutDuplicates(NextStep, TotalCost);
            }
        }

        // Plan found that makes some progress toward the goal
        if (Goal.IsReachedWithBestEffort(BestStep.PredictedStates, FirstStep.PredictedStates)) {
            return new GoapPlan() {
                Agent = Agent,
                Goal = Goal,
                Actions = BestStep.GetActions(),
                PredictedStates = BestStep.PredictedStates,
                IsBestEffort = true,
            };
        }

        // No plan found
        return null;
    }
}
/// <summary>
/// Settings to fine-tune the finding of a <see cref="GoapPlan"/>.
/// </summary>
public struct GoapPlanSettings() {
    /// <summary>
    /// How many times to try to find a plan that reaches the goal before giving up.<br/>
    /// If too low, plans that take more actions will be missed.<br/>
    /// If too high, time will be wasted when there is no possible plan.<br/>
    /// Default: 1000
    /// </summary>
    public int MaxIterations = 1000;
    /// <summary>
    /// How much to consider plans that won't help the agent in the short term.<br/>
    /// If too low, plans that could be beneficial in the long term will be missed (e.g. buying a sword to deal more damage to the player).<br/>
    /// If too high, time will be wasted considering plans that are unlikely to reach the goal.<br/>
    /// Default: 10
    /// </summary>
    public int MaxDistanceEstimate = 10;
}