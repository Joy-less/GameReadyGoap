namespace GameReadyGoap;

/// <summary>
/// An effect a <see cref="GoapAction"/> will have on a <see cref="GoapAgent"/>'s states.
/// </summary>
public class GoapEffect {
    /// <summary>
    /// The state to change.
    /// </summary>
    public required object State;
    /// <summary>
    /// The operation to perform.
    /// </summary>
    public required GoapOperation Operation;
    /// <summary>
    /// The operand for the operation.
    /// </summary>
    public required GoapValue Value;

    /// <summary>
    /// Gets the predicted state value after the effect is applied.
    /// </summary>
    public object? PredictState(IReadOnlyDictionary<object, object?> States) {
        return Operation.Operate(States[State], Value.Evaluate(States));
    }
}