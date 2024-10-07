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
    /// Executes each action if valid and successful.
    /// </summary>
    /// <returns>true if finished.</returns>
    public bool Execute(GoapAgent Agent, Func<GoapAction, bool> ExecuteAction) {
        foreach (GoapAction Action in Actions) {
            if (!Agent.IsActionValid(Action)) {
                return false;
            }
            if (!ExecuteAction(Action)) {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Executes each action if valid and successful.
    /// </summary>
    /// <returns>true if finished.</returns>
    public bool Execute(GoapAgent Agent, Action<GoapAction> ExecuteAction, bool CancelOnAgentGoalChange = true) {
        return Execute(Agent, Action => {
            if (CancelOnAgentGoalChange && Goal != Agent.ChooseGoals().FirstOrDefault()) {
                return false;
            }
            ExecuteAction(Action);
            return true;
        });
    }
    /// <summary>
    /// Executes each action if valid and successful.
    /// </summary>
    /// <returns>true if finished.</returns>
    public async Task<bool> ExecuteAsync(GoapAgent Agent, Func<GoapAction, Task<bool>> ExecuteActionAsync) {
        foreach (GoapAction Action in Actions) {
            if (!Agent.IsActionValid(Action)) {
                return false;
            }
            if (!await ExecuteActionAsync(Action)) {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Executes each action if valid and successful.
    /// </summary>
    /// <returns>true if finished.</returns>
    public async Task<bool> ExecuteAsync(GoapAgent Agent, Func<GoapAction, Task> ExecuteActionAsync, bool CancelOnAgentGoalChange = true) {
        return await ExecuteAsync(Agent, async Action => {
            if (CancelOnAgentGoalChange && Goal != Agent.ChooseGoals().FirstOrDefault()) {
                return false;
            }
            await ExecuteActionAsync(Action);
            return true;
        });
    }

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
            ActionCount = 0,
        };
        OpenQueue.Enqueue(FirstStep, 0);

        // Track cost to reach each state
        Dictionary<Dictionary<object, object?>, double> StateCosts = new(new DictionaryComparer<object, object?>());
        // Track step that gets the closest
        GoapStep BestStep = FirstStep;

        // Repeatedly find next steps
        for (int Iteration = 0; Iteration < Settings.Value.MaxIterations; Iteration++) {
            // Get most promising step
            if (!OpenQueue.TryDequeue(out GoapStep? CurrentStep)) {
                break;
            }

            // Plan found
            if (Goal.IsReached(CurrentStep.PredictedStates)) {
                return new GoapPlan() {
                    Agent = Agent,
                    Goal = Goal,
                    Actions = CurrentStep.GetActions(),
                    PredictedStates = CurrentStep.PredictedStates,
                    IsBestEffort = false,
                };
            }

            // Set step as best if it gets the closest to the goal
            if (Goal.IsReachedWithBestEffort(CurrentStep.PredictedStates, BestStep.PredictedStates)) {
                BestStep = CurrentStep;
            }

            // Ensure action count is under the maximum
            if (CurrentStep.ActionCount < Settings.Value.MaxActions) {
                // Check possible continuations
                foreach (GoapAction Action in Agent.GetValidActions(CurrentStep.PredictedStates)) {
                    // Create step to continue most promising step
                    GoapStep NextStep = new() {
                        Previous = CurrentStep,
                        Action = Action,
                        PredictedStates = Action.PredictStates(CurrentStep.PredictedStates),
                        Cost = Action.Cost(Agent) + (CurrentStep.Previous?.Cost ?? 0),
                        ActionCount = CurrentStep.ActionCount + 1,
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
    /// <summary>
    /// The maximum number of actions in a valid plan.<br/>
    /// Default: ∞
    /// </summary>
    public int MaxActions = int.MaxValue;
}