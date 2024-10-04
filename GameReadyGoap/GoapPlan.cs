using Priority_Queue;

namespace GameReadyGoap;

public class GoapPlan {
    public required GoapAgent Agent;
    public required GoapGoal Goal;
    public required List<GoapAction> Actions;
    public required Dictionary<Enum, object?> PredictedStates;
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
        Dictionary<Dictionary<Enum, object?>, double> StateCosts = new(new DictionaryComparer<Enum, object?>());
        // Track best step
        GoapStep BestStep = FirstStep;

        // Repeatedly find new steps
        for (int Iteration = 0; Iteration < Settings.Value.MaxIterations; Iteration++) {
            // Get most promising step
            if (!OpenQueue.TryDequeue(out BestStep)) {
                break;
            }

            // Found plan
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
                GoapStep NextStep = new() {
                    Previous = BestStep,
                    Action = Action,
                    PredictedStates = Action.PredictStates(BestStep.PredictedStates),
                    Cost = Action.Cost(Agent) + (BestStep.Previous?.Cost ?? 0),
                };

                // Get heuristic distance
                double Distance = NextStep.GetDistance(Goal);
                if (Distance > Settings.Value.MaxDistance) {
                    continue;
                }
                double TotalCost = NextStep.Cost + Distance;

                // Ignore if there's a cheaper path to this state already
                if (!StateCosts.TryAdd(NextStep.PredictedStates, TotalCost) && StateCosts[NextStep.PredictedStates] <= TotalCost) {
                    continue;
                }

                // Enqueue step in order of priority
                OpenQueue.EnqueueWithoutDuplicates(NextStep, TotalCost);
            }
        }

        // Return a plan that makes some progress towards the goal if possible
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
public struct GoapPlanSettings() {
    public int MaxIterations = 1000;
    public int MaxDistance = 10;
}