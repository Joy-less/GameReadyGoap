using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using GameReadyGoap.Tests;

namespace GameReadyGoap.Benchmarks;

public static class Program {
    public static void Main() {
        BenchmarkRunner.Run<FarmerBenchmark>();
    }
}

public class FarmerBenchmark {
    [Benchmark]
    public void Farmer1000() {
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
                        new GoapEffect() {
                            State = FarmerState.Energy,
                            Operation = GoapOperation.DecreaseBy,
                            Value = 50,
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
        for (int Counter = 0; Counter < 1000; Counter++) {
            Agent.FindPlan();
        }
    }
}