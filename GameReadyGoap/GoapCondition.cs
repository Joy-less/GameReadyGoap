using System.Diagnostics.CodeAnalysis;

namespace GameReadyGoap;

/// <summary>
/// A condition for a state's value.
/// </summary>
public class GoapCondition() {
    /// <summary>
    /// The state to compare.
    /// </summary>
    public required object State;
    /// <summary>
    /// How to compare the state value and value.
    /// </summary>
    public required GoapComparison Comparison;
    /// <summary>
    /// The value to compare the state with.
    /// </summary>
    public required GoapValue Value;
    /// <summary>
    /// If true, plans that get the agent closer to the condition will be considered, even if they won't reach it. The values must be numbers.<br/>
    /// Default: false
    /// </summary>
    public bool BestEffort = false;
    /// <summary>
    /// A function that returns a lower number when the value is closer to the target.<br/>
    /// By default, uses 0 if met and 2 if not met.
    /// </summary>
    public DistanceFunction? EstimateDistance = null;

    /// <summary>
    /// Constructs a <see cref="GoapCondition"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapCondition(object State, GoapComparison Comparison, GoapValue Value) : this() {
        this.State = State;
        this.Comparison = Comparison;
        this.Value = Value;
    }
    /// <summary>
    /// Returns true if the condition is met with the given states.
    /// </summary>
    public bool IsMet(IReadOnlyDictionary<object, object?> States) {
        return Comparison.IsMet(States.GetValueOrDefault(State), Value.Evaluate(States));
    }
    /// <summary>
    /// Returns true if the condition is met with the given states, or closer to being met than with the previous states.
    /// </summary>
    public bool IsMetOrCloser(IReadOnlyDictionary<object, object?> States, IReadOnlyDictionary<object, object?> PreviousStates) {
        if (!BestEffort) {
            return IsMet(States);
        }
        return Comparison.IsMetOrCloser(Value.Evaluate(States), States.GetValueOrDefault(State), PreviousStates.GetValueOrDefault(State));
    }
}

/// <summary>
/// A function that returns the distance between the value and the target.
/// </summary>
public delegate double DistanceFunction(dynamic? Value, dynamic? TargetValue);