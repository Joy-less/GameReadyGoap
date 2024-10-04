namespace GameReadyGoap.Tests;

[TestClass]
public class FarmerTest {
    [TestMethod]
    public void Test() {
        GoapAgent Agent = new() {
            States = new() {
                [FarmerState.IsFarming] = false,
                [FarmerState.Energy] = 100,
                [FarmerState.CropHealth] = 0,
            },
            Goals = [
                new GoapGoal("IncreaseCropHealth") {
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
                    ],
                    Requirements = [
                        new GoapCondition() {
                            State = FarmerState.Energy,
                            Comparison = GoapComparison.GreaterThanOrEqualTo,
                            Value = 10,
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
        Assert.IsNotNull(Agent.FindPlan());
        Assert.IsNotNull(GoapPlan.Find(Agent, Agent.Goals[0]));
    }
}

public enum FarmerState {
    IsFarming,
    Energy,
    CropHealth,
}