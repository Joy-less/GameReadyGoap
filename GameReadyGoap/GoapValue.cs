namespace GameReadyGoap;

public abstract class GoapValue {
    public abstract object? Evaluate(IReadOnlyDictionary<object, object?> States);

    public static implicit operator GoapValue(bool Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(byte Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(int Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(long Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(float Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(double Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(decimal Value) => new GoapConstantValue() { Value = Value };
    public static implicit operator GoapValue(string Value) => new GoapConstantValue() { Value = Value };
}
public class GoapConstantValue : GoapValue {
    public required object? Value;

    public override object? Evaluate(IReadOnlyDictionary<object, object?> States) {
        return Value;
    }
}
public class GoapStateValue : GoapValue {
    public required object State;

    public override object? Evaluate(IReadOnlyDictionary<object, object?> States) {
        return States.GetValueOrDefault(State);
    }
}
public class GoapStateOperationValue : GoapValue {
    public required object State;
    public required GoapOperation Operation;
    public required GoapValue Operand;

    public override object? Evaluate(IReadOnlyDictionary<object, object?> States) {
        return Operation.Operate(States.GetValueOrDefault(State), Operand.Evaluate(States));
    }
}