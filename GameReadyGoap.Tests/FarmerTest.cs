namespace GameReadyGoap.Tests;

[TestClass]
public class FarmerTest {
    public static readonly GoapAgent Agent = new() {
        States = new() {
            [FarmerState.Energy] = 100,
            [FarmerState.CropHealth] = 0,
        },
        Goals = [
            new GoapGoal("TendToCrops") {
                Objectives = [
                    new GoapCondition() {
                        State = FarmerState.CropHealth,
                        Comparison = GoapComparison.GreaterThanOrEqualTo,
                        Value = 100,
                        BestEffort = true,
                    },
                ],
            },
        ],
        Actions = [
            new GoapAction("Farm") {
                Effects = [
                    new GoapEffect() {
                        State = FarmerState.CropHealth,
                        Operation = GoapOperation.IncreaseBy,
                        Value = 20,
                    },
                    new GoapEffect() {
                        State = FarmerState.Energy,
                        Operation = GoapOperation.DecreaseBy,
                        Value = 30,
                    },
                ],
                Requirements = [
                    new GoapCondition() {
                        State = FarmerState.Energy,
                        Comparison = GoapComparison.GreaterThanOrEqualTo,
                        Value = 30,
                    },
                ],
            },
            new GoapAction("Sleep") {
                Effects = [
                    new GoapEffect() {
                        State = FarmerState.Energy,
                        Operation = GoapOperation.IncreaseBy,
                        Value = 5,
                    },
                ],
            },
        ],
    };

    [TestMethod]
    public void Test() {
        Assert.IsNotNull(Agent.FindPlan());
        Assert.IsNotNull(GoapPlan.Find(Agent, Agent.Goals[0]));
    }
}
public enum FarmerState {
    Energy,
    CropHealth,
}