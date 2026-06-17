using BenchmarkDotNet.Attributes;
using System;
using System.Numerics;

namespace Benchmarks;

public class MathFuncBenchmark<T> where T: struct, INumber<T>
{
    [Params(10, 100, 1000)]
    public int Size { get; set; }

    private T[] left, right, result;

    [GlobalSetup]
    public void Setup()
    {
        left = new T[Size];
        right = new T[Size];
        result = new T[Size];

        Array.Fill(left, T.One + T.One + T.One);
        Array.Fill(right, T.One + T.One + T.One);
    }

    [Benchmark(Baseline = true)]
    public void SummationSpan()
    {
        MathImpl.ElementwiseAdditionSpan(left, right, result.AsSpan());
    }

    [Benchmark]
    public void SubtractionSpan()
    {
        MathImpl.ElementwiseSubtractionSpan(left, right, result.AsSpan());
    }

    [Benchmark]
    public void ProductSpan()
    {
        MathImpl.ElementwiseProductSpan(left, right, result.AsSpan());
    }

    [Benchmark]
    public void DivisionSpan()
    {
        MathImpl.ElementwiseDivisionSpan(left, right, result.AsSpan());
    }


    [Benchmark]
    public void SummationUnsafe()
    {
        MathImpl.ElementwiseAdditionUnsafe(left, right, result.AsSpan());
    }

    [Benchmark]
    public void SubtractionUnsafe()
    {
        MathImpl.ElementwiseSubtractionUnsafe(left, right, result.AsSpan());
    }

    [Benchmark]
    public void ProductUnsafe()
    {
        MathImpl.ElementwiseProductUnsafe(left, right, result.AsSpan());
    }

    [Benchmark]
    public void DivisionUnsafe()
    {
        MathImpl.ElementwiseDivisionUnsafe(left, right, result.AsSpan());
    }

    [Benchmark]
    public void AdditionVectorized()
    {
        MathImpl.ElementwiseAdditionVectorized(left, right, result.AsSpan());
    }

    [Benchmark]
    public void SubtractionVectorized()
    {
        MathImpl.ElementwiseSubtractionVectorized(left, right, result.AsSpan());
    }

    [Benchmark]
    public void ProductVectorized()
    {
        MathImpl.ElementwiseProductVectorized(left, right, result.AsSpan());
    }

    [Benchmark]
    public void DivisionVectorized()
    {
        MathImpl.ElementwiseDivisionVectorized(left, right, result.AsSpan());
    }
}
