namespace GameReadyGoap.Tests;

[TestClass]
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

    [TestMethod]
    public void Test() {
        Assert.IsNotNull(Agent.FindPlan());
        Assert.IsNotNull(Agent.FindPlan(Agent.Goals[0]));
        Assert.IsNotNull(Agent.FindPlan()?.Actions.FirstOrDefault());
    }
}
public enum ShopkeeperState {
    DisplayedStock,
    StoredStock,
}