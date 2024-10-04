using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using GameReadyGoap.Tests;

namespace GameReadyGoap.Benchmarks;

public class Program {
    public static void Main() {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();

        //new FarmerBenchmark().Farmer1000();
    }
}

[SimpleJob(RuntimeMoniker.Net80)]
public class FarmerBenchmark {
    [Benchmark]
    public void Farmer1000() {
        for (int Counter = 0; Counter < 1000; Counter++) {
            FarmerTest.Agent.FindPlan();
        }
    }
}