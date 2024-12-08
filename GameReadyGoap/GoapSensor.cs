using System.Diagnostics.CodeAnalysis;

namespace GameReadyGoap;

/// <summary>
/// A dynamic updater of a state.
/// Before finding a plan, the agent will call the function to set the state.
/// </summary>
public class GoapSensor() {
    /// <summary>
    /// The state to change.
    /// </summary>
    public required object State { get; set; }
    /// <summary>
    /// The function that returns the value.
    /// </summary>
    public required Func<GoapValue> GetValue { get; set; }

    /// <summary>
    /// Constructs a <see cref="GoapSensor"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapSensor(object State, Func<GoapValue> GetValue) : this() {
        this.State = State;
        this.GetValue = GetValue;
    }
}