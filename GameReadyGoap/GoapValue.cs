using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace GameReadyGoap;

/// <summary>
/// An abstract class for values.
/// </summary>
public abstract class GoapValue {
    /// <summary>
    /// Returns the final value to be used.
    /// </summary>
    public abstract object? Evaluate(IDictionary<object, object?> States);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static implicit operator GoapValue(bool Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(sbyte Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(byte Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(short Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(ushort Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(int Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(uint Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(long Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(ulong Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(Int128 Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(UInt128 Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(BigInteger Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(Half Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(float Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(double Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(decimal Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(string Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(char Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(DateTime Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(DateTimeOffset Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(TimeSpan Value) => new GoapConstantValue(Value);
    public static implicit operator GoapValue(Guid Value) => new GoapConstantValue(Value);
#pragma warning restore CS1591
}
/// <summary>
/// The value of a constant.
/// </summary>
public class GoapConstantValue() : GoapValue {
    /// <summary>
    /// The constant value.
    /// </summary>
    public required object? Value { get; set; }

    /// <summary>
    /// Constructs a <see cref="GoapConstantValue"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapConstantValue(object? Value) : this() {
        this.Value = Value;
    }
    /// <summary>
    /// Returns the constant value.
    /// </summary>
    public override object? Evaluate(IDictionary<object, object?> States) {
        return Value;
    }
}
/// <summary>
/// The value of a state.
/// </summary>
public class GoapStateValue() : GoapValue {
    /// <summary>
    /// The state to query.
    /// </summary>
    public required object State { get; set; }

    /// <summary>
    /// Constructs a <see cref="GoapStateValue"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapStateValue(object State) : this() {
        this.State = State;
    }
    /// <summary>
    /// Returns the state value.
    /// </summary>
    public override object? Evaluate(IDictionary<object, object?> States) {
        return States.GetValueOrDefault(State);
    }
}
/// <summary>
/// The value of a state after an operation.
/// </summary>
public class GoapStateOperationValue() : GoapValue {
    /// <summary>
    /// The state to query.
    /// </summary>
    public required object State { get; set; }
    /// <summary>
    /// The operation to perform.
    /// </summary>
    public required GoapOperation Operation { get; set; }
    /// <summary>
    /// The operand for the operation.
    /// </summary>
    public required GoapValue Operand { get; set; }

    /// <summary>
    /// Constructs a <see cref="GoapStateOperationValue"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapStateOperationValue(object State, GoapOperation Operation, GoapValue Operand) : this() {
        this.State = State;
        this.Operation = Operation;
        this.Operand = Operand;
    }
    /// <summary>
    /// Returns the state value after the operation.
    /// </summary>
    public override object? Evaluate(IDictionary<object, object?> States) {
        return Operation.Operate(States.GetValueOrDefault(State), Operand.Evaluate(States));
    }
}
/// <summary>
/// The value returned from a function.
/// </summary>
public class GoapDynamicValue() : GoapValue {
    /// <summary>
    /// The function that returns the value.
    /// </summary>
    public required Func<object?> GetValue { get; set; }

    /// <summary>
    /// Constructs a <see cref="GoapDynamicValue"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapDynamicValue(Func<object?> GetValue) : this() {
        this.GetValue = GetValue;
    }
    /// <summary>
    /// Returns the state value after the operation.
    /// </summary>
    public override object? Evaluate(IDictionary<object, object?> States) {
        return GetValue();
    }
}