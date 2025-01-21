namespace GameReadyGoap.Tests;

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

    [Fact]
    public void Test() {
        Assert.NotNull(Agent.FindPlan());
        Assert.NotNull(Agent.FindPlan(Agent.Goals[0]));
        Assert.NotNull(Agent.FindPlan()?.Actions.FirstOrDefault());
    }
}
public enum FarmerState {
    Energy,
    CropHealth,
}