using System.Diagnostics.CodeAnalysis;

namespace GameReadyGoap;

/// <summary>
/// An action a <see cref="GoapAgent"/> can perform to change its states.
/// </summary>
public class GoapAction(object? Name = null) {
    /// <summary>
    /// An optional identifier.
    /// </summary>
    public object? Name { get; set; } = Name;
    /// <summary>
    /// The effects that the action is predicted to have on the agent's states.
    /// </summary>
    public required List<GoapEffect> Effects { get; set; }
    /// <summary>
    /// The requirements that must be met before the action becomes valid.
    /// </summary>
    public List<GoapCondition> Requirements { get; set; } = [];
    /// <summary>
    /// Actions with lower costs will be prioritised.
    /// </summary>
    /// <remarks>By default, always returns 1.</remarks>
    public Func<GoapAgent, double> Cost { get; set; } = _ => 1;
    /// <summary>
    /// Invalid actions will be ignored.
    /// </summary>
    /// <remarks>By default, always returns null.</remarks>
    public Func<GoapAgent, bool?> IsValidOverride { get; set; } = _ => null;
    /// <summary>
    /// The function that asynchronously executes the action and returns true if successfully completed.
    /// </summary>
    /// <remarks>By default, always returns true.</remarks>
    public Func<Task<bool>> ExecuteAsync { get; set; } = () => Task.FromResult(true);

    /// <summary>
    /// Constructs a <see cref="GoapAction"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapAction(object? Name, List<GoapEffect> Effects) : this() {
        this.Name = Name;
        this.Effects = Effects;
    }
    /// <summary>
    /// Changes the given states by the effects of the action.
    /// </summary>
    public void UpdateStates(IDictionary<object, object?> States) {
        foreach (GoapEffect Effect in Effects) {
            States[Effect.State] = Effect.PredictState(States);
        }
    }
    /// <summary>
    /// Gets the predicted states after the action is performed.
    /// </summary>
    public Dictionary<object, object?> PredictStates(IDictionary<object, object?> States) {
        Dictionary<object, object?> PredictedStates = new(States);
        UpdateStates(PredictedStates);
        return PredictedStates;
    }
}