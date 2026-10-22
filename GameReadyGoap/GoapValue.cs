using System.Diagnostics.CodeAnalysis;

namespace GameReadyGoap;

/// <summary>
/// An abstract class for values.
/// </summary>
public abstract class GoapValue {
    /// <summary>
    /// Returns the final value to be used.
    /// </summary>
    public abstract double Evaluate(IDictionary<object, double> States);

    /// <summary>
    /// Constructs a <see cref="GoapConstantValue"/> from a <see cref="double"/>.
    /// </summary>
    public static implicit operator GoapValue(double Value) => new GoapConstantValue(Value);
}
/// <summary>
/// The value of a constant.
/// </summary>
public class GoapConstantValue() : GoapValue {
    /// <summary>
    /// The constant value.
    /// </summary>
    public required double Value { get; set; }

    /// <summary>
    /// Constructs a <see cref="GoapConstantValue"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapConstantValue(double Value) : this() {
        this.Value = Value;
    }
    /// <summary>
    /// Returns the constant value.
    /// </summary>
    public override double Evaluate(IDictionary<object, double> States) {
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
    public override double Evaluate(IDictionary<object, double> States) {
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
    public override double Evaluate(IDictionary<object, double> States) {
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
    public required Func<double> GetValue { get; set; }

    /// <summary>
    /// Constructs a <see cref="GoapDynamicValue"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapDynamicValue(Func<double> GetValue) : this() {
        this.GetValue = GetValue;
    }
    /// <summary>
    /// Returns the state value after the operation.
    /// </summary>
    public override double Evaluate(IDictionary<object, double> States) {
        return GetValue();
    }
}