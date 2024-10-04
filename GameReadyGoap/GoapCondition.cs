namespace GameReadyGoap;

public class GoapCondition {
    public required Enum State;
    public required GoapComparison Comparison;
    public required GoapValue Value;
    /// <summary>
    /// If true, plans that get the agent closer to the condition will be considered, even if they won't reach it.<br/>
    /// The values must be numbers.<br/>
    /// Default: true
    /// </summary>
    public bool BestEffort = false;
    /// <summary>
    /// Should return a number that's lower when the value is closer to the target. If null, the planner uses 0 when met and 1 when not met.
    /// </summary>
    public DistanceFunction? Distance = null;

    public bool IsMet(IReadOnlyDictionary<Enum, object?> States) {
        return Comparison.Compare(States.GetValueOrDefault(State), Value.Evaluate(States));
    }
    public bool IsMetOrCloser(IReadOnlyDictionary<Enum, object?> States, IReadOnlyDictionary<Enum, object?> PreviousStates) {
        if (BestEffort) {
            return Comparison.IsMetOrCloser(Value.Evaluate(States), States.GetValueOrDefault(State), PreviousStates.GetValueOrDefault(State));
        }
        else {
            return IsMet(States);
        }
    }
}
public delegate double DistanceFunction(dynamic? Value, dynamic? TargetValue);