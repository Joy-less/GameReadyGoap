using System.Diagnostics.CodeAnalysis;

namespace GameReadyGoap;

/// <summary>
/// An action a <see cref="GoapAgent"/> can perform to change its states.
/// </summary>
public class GoapAction(object? Name = null) {
    /// <summary>
    /// An optional identifier.
    /// </summary>
    public object? Name = Name;
    /// <summary>
    /// The effects that the action is predicted to have on the agent's states.
    /// </summary>
    public required List<GoapEffect> Effects;
    /// <summary>
    /// The requirements that must be met before the action becomes valid.
    /// </summary>
    public List<GoapCondition> Requirements = [];
    /// <summary>
    /// Actions with lower costs will be prioritised.<br/>
    /// By default, always returns 1.
    /// </summary>
    public Func<GoapAgent, double> Cost = _ => 1;
    /// <summary>
    /// Invalid actions will be ignored.
    /// </summary>
    /// <remarks>By default, always returns null.</remarks>
    public Func<GoapAgent, bool?> IsValidOverride = _ => null;

    /// <summary>
    /// Constructs a <see cref="GoapAction"/> in-line.
    /// </summary>
    [SetsRequiredMembers]
    public GoapAction(object? Name, List<GoapEffect> Effects) : this() {
        this.Name = Name;
        this.Effects = Effects;
    }
    /// <summary>
    /// Gets the predicted states after the action is performed.
    /// </summary>
    public Dictionary<object, object?> PredictStates(IReadOnlyDictionary<object, object?> States) {
        Dictionary<object, object?> PredictedStates = new(States);
        foreach (GoapEffect Effect in Effects) {
            PredictedStates[Effect.State] = Effect.PredictState(PredictedStates);
        }
        return PredictedStates;
    }
}