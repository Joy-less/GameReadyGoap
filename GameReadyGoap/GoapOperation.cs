namespace GameReadyGoap;

/// <summary>
/// Ways to operate two values.
/// </summary>
public enum GoapOperation {
    /// <summary>
    /// Returns the second value.
    /// </summary>
    SetTo,
    /// <summary>
    /// Returns the first value plus the second value.
    /// </summary>
    IncreaseBy,
    /// <summary>
    /// Returns the first value minus the second value.
    /// </summary>
    DecreaseBy,
    /// <summary>
    /// Returns the first value times the second value.
    /// </summary>
    MultiplyBy,
    /// <summary>
    /// Returns the first value over the second value.
    /// </summary>
    DivideBy,
    /// <summary>
    /// Returns the first value modulo the second value.
    /// </summary>
    ModuloBy,
    /// <summary>
    /// Returns the first value to the power of the second value.
    /// </summary>
    ExponentiateBy,
}