using BenchmarkDotNet.Running;

namespace Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        //var summary = BenchmarkRunner.Run<MathFuncBenchmark<float>>();
        //var summary1 = BenchmarkRunner.Run<MatrixOptimization<float>>();
        //var summary2 = BenchmarkRunner.Run<MatrixOptimization<double>>();
        BenchmarkRunner.Run<DelaunaySubroutines>();
    }
}