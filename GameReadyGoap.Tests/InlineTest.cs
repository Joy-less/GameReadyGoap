namespace GameReadyGoap.Tests;

public class InlineTest {
    public static readonly GoapAgent Agent = new() {
        States = new() {
            [InlineState.Money] = 0.0,
        },
        Goals = [
            new GoapGoal("BeRich", [
                new GoapCondition(InlineState.Money, GoapComparison.EqualTo, 5_000) {
                    BestEffort = true,
                },
            ]),
        ],
        Actions = [
            new GoapAction("Capitalize", [
                new GoapEffect(InlineState.Money, GoapOperation.IncreaseBy, 105),
            ]),
        ],
    };

    [Fact]
    public void Test() {
        Assert.NotNull(Agent.FindPlan());
        Assert.NotNull(Agent.FindPlan(Agent.Goals[0]));
        Assert.NotNull(Agent.FindPlan()?.Actions.FirstOrDefault());

        Assert.True(Agent.FindPlan()?.IsBestEffort);
    }
}
public enum InlineState {
    Money,
}