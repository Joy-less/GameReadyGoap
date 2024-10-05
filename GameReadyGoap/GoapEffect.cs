namespace GameReadyGoap;

public class GoapEffect {
    public required object State;
    public required GoapOperation Operation;
    public required GoapValue Value;

    /// <summary>
    /// Gets the predicted state value after the effect is applied.
    /// </summary>
    public object? PredictState(IReadOnlyDictionary<object, object?> States) {
        return Operation.Operate(States[State], Value.Evaluate(States));
    }
}