namespace GameReadyGoap;

internal sealed class GoapStep {
    public required GoapStep? Previous;
    public required GoapAction? Action;
    public required Dictionary<object, object?> PredictedStates;
    public required double Cost;
    public required int ActionCount;

    /// <summary>
    /// Gets the full list of actions resulting in <see cref="PredictedStates"/>.
    /// </summary>
    public List<GoapAction> GetActions() {
        List<GoapAction> Actions = new(ActionCount);
        GoapStep CurrentStep = this;
        while (CurrentStep.Previous is not null) {
            if (CurrentStep.Action is not null) {
                Actions.Add(CurrentStep.Action);
            }
            CurrentStep = CurrentStep.Previous;
        }
        Actions.Reverse();
        return Actions;
    }
    /// <summary>
    /// Gets the heuristic distance between the step and the goal.
    /// </summary>
    public double EstimateDistance(GoapGoal Goal) {
        // Get distance of resultant states to desired states
        double Distance = 0;
        foreach (GoapCondition Objective in Goal.Objectives) {
            if (Objective.EstimateDistance is null) {
                Distance += Objective.IsMet(PredictedStates) ? 0 : 2;
            }
            else {
                Distance += Math.Abs(Objective.EstimateDistance(PredictedStates.GetValueOrDefault(Objective.State), Objective.Value.Evaluate(PredictedStates)));
            }
        }
        return Distance;
    }
}