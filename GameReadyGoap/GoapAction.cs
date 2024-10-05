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
    /// The requirements that must be met before the action can be performed.
    /// </summary>
    public List<GoapCondition> Requirements = [];
    /// <summary>
    /// Actions with lower costs will be prioritised.<br/>
    /// By default, always returns 1.
    /// </summary>
    public Func<GoapAgent, double> Cost = _ => 1;
    /// <summary>
    /// Invalid actions will be ignored.<br/>
    /// By default, always returns true.
    /// </summary>
    public Func<GoapAgent, bool> IsValid = _ => true;

    /// <summary>
    /// Gets the predicted states after the action is performed.
    /// </summary>
    public Dictionary<object, object?> PredictStates(IReadOnlyDictionary<object, object?> States) {
        Dictionary<object, object?> PredictedStates = States.ToDictionary();
        foreach (GoapEffect Effect in Effects) {
            PredictedStates[Effect.State] = Effect.PredictState(PredictedStates);
        }
        return PredictedStates;
    }
}