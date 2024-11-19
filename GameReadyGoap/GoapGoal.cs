using System.Diagnostics.CodeAnalysis;

namespace GameReadyGoap;

/// <summary>
/// A set of state conditions a <see cref="GoapAgent"/> is trying to reach.
/// </summary>
public class GoapGoal(object? Name = null) {
    /// <summary>
    /// An optional identifier.
    /// </summary>
    public object? Name = Name;
    /// <summary>
    /// The conditions that must be met for the goal to be reached.
    /// </summary>
    public required List<GoapCondition> Objectives = [];
    /// <summary>
    /// Goals with higher priorities will be prioritised.<br/>
    /// By default, always returns 1.
    /// </summary>
    public Func<GoapAgent, double> Priority = _ => 1;
    /// <summary>
    /// Invalid goals will be ignored.
    /// </summary>
    /// <remarks>By default, always returns null.</remarks>
    public Func<GoapAgent, bool?> IsValidOverride = _ => null;

    /// <summary>
    /// Constructs a <see cref="GoapGoal"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapGoal(object? Name, List<GoapCondition> Objectives) : this() {
        this.Name = Name;
        this.Objectives = Objectives;
    }
    /// <summary>
    /// Returns true if the goal is reached with the given states.
    /// </summary>
    public bool IsReached(IReadOnlyDictionary<object, object?> States) {
        return Objectives.All(Objective => Objective.IsMet(States));
    }
    /// <summary>
    /// Returns true if the goal is reached with the given states, or closer to being reached than with the previous states.
    /// </summary>
    public bool IsReachedWithBestEffort(IReadOnlyDictionary<object, object?> States, IReadOnlyDictionary<object, object?> PreviousStates) {
        return Objectives.All(Objective => Objective.IsMetOrCloser(States, PreviousStates));
    }
}