namespace GameReadyGoap.Tests;

[TestClass]
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

    [TestMethod]
    public void Test() {
        Assert.IsNotNull(Agent.FindPlan());
        Assert.IsNotNull(Agent.FindPlan(Agent.Goals[0]));
        Assert.IsNotNull(Agent.FindPlan()?.Actions.FirstOrDefault());
    }
}
public enum InlineState {
    Money,
}