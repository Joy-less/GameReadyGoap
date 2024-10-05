using System.Collections.Concurrent;

namespace GameReadyGoap;

/// <summary>
/// A being that can plan actions to change its states to reach its goals.
/// </summary>
public class GoapAgent(object? Name = null) {
    /// <summary>
    /// An optional identifier.
    /// </summary>
    public object? Name = Name;
    /// <summary>
    /// The values describing the agent's current state.
    /// </summary>
    public required ConcurrentDictionary<object, object?> States = [];
    /// <summary>
    /// The goals the agent is trying to achieve.
    /// </summary>
    public required List<GoapGoal> Goals = [];
    /// <summary>
    /// The actions the agent can perform to change its states.
    /// </summary>
    public required List<GoapAction> Actions = [];

    /// <summary>
    /// Gets the current value of the given state, casting it to the given type.
    /// </summary>
    public T GetState<T>(object State) {
        return (T)States.GetValueOrDefault(State)!;
    }
    /// <summary>
    /// Gets the current value of the given state, cast as a <see langword="dynamic"/>.
    /// </summary>
    public dynamic? GetState(object State) {
        return States.GetValueOrDefault(State);
    }
    /// <summary>
    /// Sets the current value of the given state.
    /// </summary>
    public void SetState(object State, object? Value) {
        States[State] = Value;
    }
    /// <summary>
    /// Gets the agent's valid goals in order of priority.
    /// </summary>
    public IEnumerable<GoapGoal> ChooseGoals() {
        return GetValidGoals().OrderByDescending(Goal => Goal.Priority(this));
    }
    /// <summary>
    /// Attempts to find a plan to reach one of the agent's goals.
    /// </summary>
    public GoapPlan? FindPlan(GoapPlanSettings? Settings = null) {
        // Try find plan for highest priority goals first
        foreach (GoapGoal Goal in ChooseGoals()) {
            GoapPlan? Plan = FindPlan(Goal, Settings);
            if (Plan is not null) {
                return Plan;
            }
        }
        // No plan found for any goal
        return null;
    }
    /// <summary>
    /// Attempts to find a plan to reach the given goal.
    /// </summary>
    public GoapPlan? FindPlan(GoapGoal Goal, GoapPlanSettings? Settings = null) {
        return GoapPlan.Find(this, Goal, Settings);
    }
    /// <summary>
    /// Gets the agent's goals that should be considered.
    /// </summary>
    public IEnumerable<GoapGoal> GetValidGoals() {
        foreach (GoapGoal Goal in Goals) {
            if (!Goal.IsValid(this)) {
                continue;
            }
            if (Goal.IsReached(States)) {
                continue;
            }
            yield return Goal;
        }
    }
    /// <summary>
    /// Gets the agent's actions that should be considered with the given states.
    /// </summary>
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
    /// <summary>
    /// Gets the agent's actions that should be considered.
    /// </summary>
    public IEnumerable<GoapAction> GetValidActions() {
        return GetValidActions(States);
    }
}