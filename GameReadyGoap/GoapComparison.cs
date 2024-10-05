namespace GameReadyGoap;

/// <summary>
/// Ways to compare two values.
/// </summary>
public enum GoapComparison {
    /// <summary>
    /// Returns true if the first value is equal to the second value.
    /// </summary>
    EqualTo,
    /// <summary>
    /// Returns true if the first value is not equal to the second value.
    /// </summary>
    NotEqualTo,
    /// <summary>
    /// Returns true if the first value is less than the second value.
    /// </summary>
    LessThan,
    /// <summary>
    /// Returns true if the first value is greater than the second value.
    /// </summary>
    GreaterThan,
    /// <summary>
    /// Returns true if the first value is less than or equal to the second value.
    /// </summary>
    LessThanOrEqualTo,
    /// <summary>
    /// Returns true if the first value is greater than or equal to the second value.
    /// </summary>
    GreaterThanOrEqualTo,
}