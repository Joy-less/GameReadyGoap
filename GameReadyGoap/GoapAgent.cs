using System.Collections.Concurrent;

namespace GameReadyGoap;

public class GoapAgent(string? Name = null) {
    public string? Name = Name;
    public required ConcurrentDictionary<object, object?> States = [];
    public required List<GoapGoal> Goals = [];
    public required List<GoapAction> Actions = [];

    public T GetState<T>(object State) {
        return (T)States.GetValueOrDefault(State)!;
    }
    public dynamic? GetState(object State) {
        return States.GetValueOrDefault(State);
    }
    public void SetState(object State, object Value) {
        States[State] = Value;
    }
    public IEnumerable<GoapGoal> ChooseGoals() {
        return GetValidGoals().OrderByDescending(Goal => Goal.Priority(this));
    }
    public GoapPlan? FindPlan(GoapPlanSettings? Settings = null) {
        // Try find plan for highest priority goals first
        foreach (GoapGoal Goal in ChooseGoals()) {
            GoapPlan? Plan = GoapPlan.Find(this, Goal, Settings);
            if (Plan is not null) {
                return Plan;
            }
        }
        // No plan found for any goal
        return null;
    }
    public IEnumerable<GoapGoal> GetValidGoals() {
        foreach (GoapGoal Goal in Goals) {
            if (!Goal.IsValid(this)) {
                continue;
            }
            yield return Goal;
        }
    }
    public IEnumerable<GoapAction> GetValidActions() {
        foreach (GoapAction Action in Actions) {
            if (!Action.IsValid(this)) {
                continue;
            }
            if (!Action.Requirements.All(Requirement => Requirement.IsMet(States))) {
                continue;
            }
            yield return Action;
        }
    }
    public IEnumerable<GoapAction> GetValidActions(IReadOnlyDictionary<object, object?> States) {
        foreach (GoapAction Action in Actions) {
            if (!Action.IsValid(this)) {
                continue;
            }
            if (!Action.Requirements.All(Requirement => Requirement.IsMet(States))) {
                continue;
            }
            yield return Action;
        }
    }
}