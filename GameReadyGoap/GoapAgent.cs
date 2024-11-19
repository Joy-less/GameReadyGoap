using System.Collections.Concurrent;

namespace GameReadyGoap;

/// <summary>
/// An entity that can plan actions to change its states to reach its goals.
/// </summary>
public class GoapAgent(object? Name = null) {
    /// <summary>
    /// An optional identifier.
    /// </summary>
    public object? Name = Name;
    /// <summary>
    /// The values describing the agent's current state.
    /// </summary>
    public required ConcurrentDictionary<object, object?> States;
    /// <summary>
    /// The goals the agent is trying to achieve.
    /// </summary>
    public required List<GoapGoal> Goals;
    /// <summary>
    /// The actions the agent can perform to change its states.
    /// </summary>
    public required List<GoapAction> Actions;

    /// <summary>
    /// Gets the current value of the given state, casting it to the given type.
    /// </summary>
    public T GetState<T>(object State) {
        return (T)States.GetValueOrDefault(State)!;
    }
    /// <summary>
    /// Gets the current value of the given state, cast to a <see langword="dynamic"/>.
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
    /// Returns true if the goal is valid for this agent with the given states.
    /// </summary>
    public bool IsGoalValid(GoapGoal Goal, IReadOnlyDictionary<object, object?> States) {
        if (Goal.IsValidOverride(this) is bool Override) {
            return Override;
        }
        if (Goal.IsReached(States)) {
            return false;
        }
        return true;
    }
    /// <summary>
    /// Returns true if the goal is valid for this agent.
    /// </summary>
    public bool IsGoalValid(GoapGoal Goal) {
        return IsGoalValid(Goal, States);
    }
    /// <summary>
    /// Returns true if the action is valid for this agent with the given states.
    /// </summary>
    public bool IsActionValid(GoapAction Action, IReadOnlyDictionary<object, object?> States) {
        if (Action.IsValidOverride(this) is bool Override) {
            return Override;
        }
        if (!Action.Requirements.All(Requirement => Requirement.IsMet(States))) {
            return false;
        }
        return true;
    }
    /// <summary>
    /// Returns true if the action is valid for this agent.
    /// </summary>
    public bool IsActionValid(GoapAction Action) {
        return IsActionValid(Action, States);
    }
    /// <summary>
    /// Returns the valid goals for this agent with the given states.
    /// </summary>
    public IEnumerable<GoapGoal> GetValidGoals(IReadOnlyDictionary<object, object?> States) {
        return Goals.Where(Goal => IsGoalValid(Goal, States));
    }
    /// <summary>
    /// Returns the valid goals for this agent.
    /// </summary>
    public IEnumerable<GoapGoal> GetValidGoals() {
        return GetValidGoals(States);
    }
    /// <summary>
    /// Returns the valid actions for this agent with the given states.
    /// </summary>
    public IEnumerable<GoapAction> GetValidActions(IReadOnlyDictionary<object, object?> States) {
        return Actions.Where(Action => IsActionValid(Action, States));
    }
    /// <summary>
    /// Returns the valid actions for this agent.
    /// </summary>
    public IEnumerable<GoapAction> GetValidActions() {
        return GetValidActions(States);
    }
}