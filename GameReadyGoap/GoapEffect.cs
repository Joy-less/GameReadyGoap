using System.Diagnostics.CodeAnalysis;

namespace GameReadyGoap;

/// <summary>
/// An effect a <see cref="GoapAction"/> will have on a <see cref="GoapAgent"/>'s states.
/// </summary>
public class GoapEffect() {
    /// <summary>
    /// The state to change.
    /// </summary>
    public required object State { get; set; }
    /// <summary>
    /// The operation to perform.
    /// </summary>
    public required GoapOperation Operation { get; set; }
    /// <summary>
    /// The operand for the operation.
    /// </summary>
    public required GoapValue Value { get; set; }

    /// <summary>
    /// Constructs a <see cref="GoapEffect"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapEffect(object State, GoapOperation Operation, GoapValue Value) : this() {
        this.State = State;
        this.Operation = Operation;
        this.Value = Value;
    }
    /// <summary>
    /// Gets the predicted state value after the effect is applied.
    /// </summary>
    public object? PredictState(IDictionary<object, object?> States) {
        return Operation.Operate(States[State], Value.Evaluate(States));
    }
}