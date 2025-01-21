namespace GameReadyGoap.Tests;

public class InlineTest {
    public static readonly GoapAgent Agent = new() {
        States = new() {
            [InlineState.Money] = 0m,
        },
        Goals = [
            new GoapGoal("BeRich", [
                new GoapCondition(InlineState.Money, GoapComparison.EqualTo, decimal.MaxValue) {
                    BestEffort = true,
                },
            ]),
        ],
        Actions = [
            new GoapAction("Capitalize", [
                new GoapEffect(InlineState.Money, GoapOperation.IncreaseBy, 100),
            ]),
        ],
    };

    [Fact]
    public void Test() {
        Assert.NotNull(Agent.FindPlan());
        Assert.NotNull(Agent.FindPlan(Agent.Goals[0]));
        Assert.NotNull(Agent.FindPlan()?.Actions.FirstOrDefault());
    }
}
public enum InlineState {
    Money,
}