namespace GameReadyGoap;

/// <summary>
/// A sequence of actions a <see cref="GoapAgent"/> could perform to reach a <see cref="GoapGoal"/>.
/// </summary>
public class GoapPlan {
    /// <summary>
    /// The agent the plan was created for.
    /// </summary>
    public required GoapAgent Agent { get; set; }
    /// <summary>
    /// The goal the plan was created for.
    /// </summary>
    public required GoapGoal Goal { get; set; }
    /// <summary>
    /// The actions that should be performed to reach the goal.
    /// </summary>
    public required List<GoapAction> Actions { get; set; }
    /// <summary>
    /// The agent's predicted states after the plan is performed.
    /// </summary>
    public required Dictionary<object, object?> PredictedStates { get; set; }
    /// <summary>
    /// If true, the plan won't reach the goal but will get the agent closer to it.
    /// </summary>
    public required bool IsBestEffort { get; set; }

    /// <summary>
    /// Finds a plan that reaches the goal using the A* algorithm.
    /// </summary>
    public static GoapPlan? Find(GoapAgent Agent, GoapGoal Goal, GoapPlanSettings? Settings = null) {
        Settings ??= new GoapPlanSettings();

        // Update agent states from sensors
        Agent.SenseStates();

        // Create queue that puts cheapest steps first
        PriorityQueue<GoapStep, double> OpenQueue = new();

        // Create first step from initial states
        GoapStep FirstStep = new() {
            Previous = null,
            Action = null, 
            PredictedStates = new(Agent.States),
            TotalCost = 0,
            TotalSteps = 0,
        };
        OpenQueue.Enqueue(FirstStep, 0);

        // Track cost (including heuristics) to reach each state
        Dictionary<Dictionary<object, object?>, double> StateCosts = new(new DictionaryComparer<object, object?>());
        // Track step that gets the closest
        GoapStep BestStep = FirstStep;

        // Repeatedly find next steps
        for (int Iteration = 0; Iteration < Settings.Value.MaxIterations; Iteration++) {
            // Get most promising step
            if (!OpenQueue.TryDequeue(out GoapStep? CurrentStep, out _)) {
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

            // Ensure total actions (plus next action) is under maximum
            if (CurrentStep.TotalSteps < Settings.Value.MaxActions) {
                // Check possible continuations
                foreach (GoapAction Action in Agent.GetValidActions(CurrentStep.PredictedStates)) {
                    // Create step to continue most promising step
                    GoapStep NextStep = new() {
                        Previous = CurrentStep,
                        Action = Action,
                        PredictedStates = Action.PredictStates(CurrentStep.PredictedStates),
                        TotalCost = Action.Cost(Agent) + (CurrentStep.Previous?.TotalCost ?? 0),
                        TotalSteps = CurrentStep.TotalSteps + 1,
                    };

                    // Ensure total cost is under maximum
                    if (NextStep.TotalCost > Settings.Value.MaxCost) {
                        continue;
                    }

                    // Get heuristic distance to goal
                    double HeuristicDistance = NextStep.EstimateDistance(Goal);
                    if (HeuristicDistance > Settings.Value.MaxDistanceEstimate) {
                        continue;
                    }
                    double TotalCost = NextStep.TotalCost + HeuristicDistance;

                    // Skip if there's a cheaper path to this state already
                    if (StateCosts.TryGetValue(NextStep.PredictedStates, out double CurrentCost) && CurrentCost <= TotalCost) {
                        continue;
                    }
                    // Otherwise set as cheapest path
                    else {
                        StateCosts[NextStep.PredictedStates] = TotalCost;
                    }

                    // Submit step in order of priority
                    OpenQueue.Enqueue(NextStep, TotalCost);
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

    /// <summary>
    /// Tries to execute each action in the plan by calling <see cref="GoapAction.ExecuteAsync"/> and updating the agent's states.
    /// </summary>
    /// <param name="CancelOnGoalChange">
    /// If <see langword="true"/>, cancels the plan if the agent's prioritised goal changes.
    /// </param>
    /// <returns>Whether the plan was fully executed successfully.</returns>
    public async Task<bool> ExecuteAsync(bool CancelOnGoalChange = true) {
        foreach (GoapAction Action in Actions) {
            // Cancel if prioritised goal has changed
            if (CancelOnGoalChange && Goal != Agent.ChooseGoals().FirstOrDefault()) {
                return false;
            }
            // Cancel if action is invalid
            if (!Agent.IsActionValid(Action)) {
                return false;
            }
            // Execute action and cancel if failed
            if (!await Action.ExecuteAsync()) {
                return false;
            }
            // Apply the action's effects
            Action.UpdateStates(Agent.States);
            // Update states from sensors
            Agent.SenseStates();
        }
        return true;
    }
    /// <inheritdoc cref="ExecuteAsync(bool)"/>
    public bool Execute(bool CancelOnGoalChange = true) {
        return ExecuteAsync(CancelOnGoalChange).GetAwaiter().GetResult();
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
    public int MaxIterations { get; set; } = 1000;
    /// <summary>
    /// How much to consider plans that won't help the agent in the short term.<br/>
    /// If too low, plans that could be beneficial in the long term will be missed (e.g. buying a sword to deal more damage to the player).<br/>
    /// If too high, time will be wasted considering plans that are unlikely to reach the goal.<br/>
    /// Default: 10
    /// </summary>
    public int MaxDistanceEstimate { get; set; } = 10;
    /// <summary>
    /// The maximum number of actions in a valid plan.<br/>
    /// Default: ∞
    /// </summary>
    public int MaxActions { get; set; } = int.MaxValue;
    /// <summary>
    /// The maximum cost of a valid plan.<br/>
    /// Default: ∞
    /// </summary>
    public double MaxCost { get; set; } = double.MaxValue;
}