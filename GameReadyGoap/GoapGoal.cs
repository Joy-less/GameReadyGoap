namespace GameReadyGoap;

public class GoapGoal(object? Name = null) {
    public object? Name = Name;
    public GoapCondition[] Objectives = [];
    public Func<GoapAgent, double> Priority = _ => 1;
    public Func<GoapAgent, bool> IsValid = _ => true;

    public bool IsReached(IReadOnlyDictionary<object, object?> States) {
        return Objectives.All(Objective => Objective.IsMet(States));
    }
    public bool IsReachedWithBestEffort(IReadOnlyDictionary<object, object?> States, IReadOnlyDictionary<object, object?> PreviousStates) {
        return Objectives.All(Objective => Objective.IsMetOrCloser(States, PreviousStates));
    }
}