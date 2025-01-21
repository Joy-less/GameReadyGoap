namespace GameReadyGoap.Tests;

public class ShopkeeperTest {
    public static readonly GoapAgent Agent = new() {
        States = new() {
            [ShopkeeperState.DisplayedStock] = 0,
            [ShopkeeperState.StoredStock] = 5,
        },
        Goals = [
            new GoapGoal("DisplayMostStock") {
                Objectives = [
                    new GoapCondition() {
                        State = ShopkeeperState.DisplayedStock,
                        Comparison = GoapComparison.GreaterThan,
                        Value = new GoapStateValue() {
                            State = ShopkeeperState.DisplayedStock,
                        },
                        BestEffort = true,
                    },
                ],
            },
        ],
        Actions = [
            new GoapAction("Restock") {
                Effects = [
                    new GoapEffect() {
                        State = ShopkeeperState.DisplayedStock,
                        Operation = GoapOperation.IncreaseBy,
                        Value = new GoapStateValue() {
                            State = ShopkeeperState.StoredStock,
                        },
                    },
                    new GoapEffect() {
                        State = ShopkeeperState.StoredStock,
                        Operation = GoapOperation.SetTo,
                        Value = 0,
                    },
                ],
            },
        ],
    };

    [Fact]
    public void Test() {
        Assert.NotNull(Agent.FindPlan());
        Assert.NotNull(Agent.FindPlan(Agent.Goals[0]));
        Assert.NotNull(Agent.FindPlan()?.Actions.FirstOrDefault());
    }
}
public enum ShopkeeperState {
    DisplayedStock,
    StoredStock,
}