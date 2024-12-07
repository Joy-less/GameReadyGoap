namespace GameReadyGoap;

/// <summary>
/// A set of extension methods used in <see cref="GameReadyGoap"/>.
/// </summary>
public static class GoapExtensions {
    /// <summary>
    /// Compares the first value with the second value.
    /// </summary>
    public static bool IsMet(this GoapComparison Comparison, dynamic? ValueA, dynamic? ValueB) {
        return Comparison switch {
            GoapComparison.EqualTo => ValueA == ValueB,
            GoapComparison.NotEqualTo => ValueA != ValueB,
            GoapComparison.LessThan => ValueA < ValueB,
            GoapComparison.GreaterThan => ValueA > ValueB,
            GoapComparison.LessThanOrEqualTo => ValueA <= ValueB,
            GoapComparison.GreaterThanOrEqualTo => ValueA >= ValueB,
            _ => throw new NotImplementedException()
        };
    }
    /// <summary>
    /// Compares the value with the target, or the value with the previous value.
    /// </summary>
    public static bool IsMetOrCloser(this GoapComparison Comparison, dynamic? Target, dynamic? Value, dynamic? PreviousValue) {
        return Comparison switch {
            GoapComparison.EqualTo => (Value == Target) || (Math.Abs(Value - Target) < Math.Abs(PreviousValue - Target)),
            GoapComparison.NotEqualTo => (Value != Target) || (Math.Abs(Value - Target) > Math.Abs(PreviousValue - Target)),
            GoapComparison.LessThan => (Value < Target) || (Value < PreviousValue),
            GoapComparison.GreaterThan => (Value > Target) || (Value > PreviousValue),
            GoapComparison.LessThanOrEqualTo => (Value <= Target) || (Value < PreviousValue),
            GoapComparison.GreaterThanOrEqualTo => (Value >= Target) || (Value > PreviousValue),
            _ => throw new NotImplementedException()
        };
    }
    /// <summary>
    /// Performs the operation on the first value with the second value.
    /// </summary>
    public static dynamic? Operate(this GoapOperation Operation, dynamic? ValueA, dynamic? ValueB) {
        return Operation switch {
            GoapOperation.SetTo => ValueB,
            GoapOperation.IncreaseBy => ValueA + ValueB,
            GoapOperation.DecreaseBy => ValueA - ValueB,
            GoapOperation.MultiplyBy => ValueA * ValueB,
            GoapOperation.DivideBy => ValueA / ValueB,
            GoapOperation.ModuloBy => ValueA % ValueB,
            GoapOperation.ExponentiateBy => Math.Pow(Convert.ToDouble(ValueA), Convert.ToDouble(ValueB)),
            _ => throw new NotImplementedException()
        };
    }
    /// <inheritdoc cref="CollectionExtensions.GetValueOrDefault{TKey, TValue}(IReadOnlyDictionary{TKey, TValue}, TKey, TValue)"/>
    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> Dictionary, TKey Key, TValue? Default = default) {
        return Dictionary.TryGetValue(Key, out TValue? Value) ? Value : Default;
    }
}