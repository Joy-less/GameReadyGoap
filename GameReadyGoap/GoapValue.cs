namespace GameReadyGoap;

/// <summary>
/// An abstract class for values.
/// </summary>
public abstract class GoapValue {
    /// <summary>
    /// Returns the final value to be used.
    /// </summary>
    public abstract object? Evaluate(IReadOnlyDictionary<object, object?> States);

#pragma warning disable CS1591
    public static implicit operator GoapValue(bool Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(byte Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(int Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(long Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(float Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(double Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(decimal Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(string Value) => new GoapConstantValue() { Value = Value };
#pragma warning restore CS1591
}
/// <summary>
/// The value of a constant.
/// </summary>
public class GoapConstantValue : GoapValue {
    /// <summary>
    /// The constant value.
    /// </summary>
    public required object? Value;

    /// <summary>
    /// Returns the constant value.
    /// </summary>
    public override object? Evaluate(IReadOnlyDictionary<object, object?> States) {
        return Value;
    }
}
/// <summary>
/// The value of a state.
/// </summary>
public class GoapStateValue : GoapValue {
    /// <summary>
    /// The state to query.
    /// </summary>
    public required object State;

    /// <summary>
    /// Returns the state value.
    /// </summary>
    public override object? Evaluate(IReadOnlyDictionary<object, object?> States) {
        return States.GetValueOrDefault(State);
    }
}
/// <summary>
/// The value of a state after an operation.
/// </summary>
public class GoapStateOperationValue : GoapValue {
    /// <summary>
    /// The state to query.
    /// </summary>
    public required object State;
    /// <summary>
    /// The operation to perform.
    /// </summary>
    public required GoapOperation Operation;
    /// <summary>
    /// The operand for the operation.
    /// </summary>
    public required GoapValue Operand;

    /// <summary>
    /// Returns the state value after the operation.
    /// </summary>
    public override object? Evaluate(IReadOnlyDictionary<object, object?> States) {
        return Operation.Operate(States.GetValueOrDefault(State), Operand.Evaluate(States));
    }
}