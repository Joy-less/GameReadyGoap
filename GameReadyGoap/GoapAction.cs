namespace GameReadyGoap;

public class GoapAction(string? Name = null) {
    public string? Name = Name;
    public required List<GoapEffect> Effects;
    public List<GoapCondition> Requirements = [];
    public Func<GoapAgent, double> Cost = _ => 1;
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