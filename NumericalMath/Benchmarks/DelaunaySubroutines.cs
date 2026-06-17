using BenchmarkDotNet.Attributes;
using NumericalMath.Geometry;
using NumericalMath.Geometry.Structures;
using System;
using System.Collections.Generic;

namespace Benchmarks;

/**
  *BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
  *AMD Ryzen 5 5500H with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
  *.NET SDK 9.0.102
  *  [Host]     : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX2
  *  DefaultJob : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX2
  *
  *
  *| Method            | Size | Mean       | Error     | StdDev     | Ratio | RatioSD | Gen0     | Gen1   | Allocated  | Alloc Ratio |
  *|------------------ |----- |-----------:|----------:|-----------:|------:|--------:|---------:|-------:|-----------:|------------:|
  *| DelaunayDefault   | 10   |  49.643 us | 0.9842 us |  1.0939 us |  1.00 |    0.03 |  25.2075 |      - |  103.07 KB |        1.00 |
  *| DelaunayOptimized | 10   |   4.938 us | 0.0948 us |  0.1297 us |  0.10 |    0.00 |   2.8915 |      - |   11.81 KB |        0.11 |
  *|                   |      |            |           |            |       |         |          |        |            |             |
  *| DelaunayDefault   | 25   | 163.086 us | 1.5648 us |  1.4637 us |  1.00 |    0.01 |  90.3320 |      - |  369.82 KB |        1.00 |
  *| DelaunayOptimized | 25   |  12.624 us | 0.1937 us |  0.1812 us |  0.08 |    0.00 |   6.6528 |      - |   27.23 KB |        0.07 |
  *|                   |      |            |           |            |       |         |          |        |            |             |
  *| DelaunayDefault   | 50   | 459.270 us | 9.0009 us | 15.2843 us |  1.00 |    0.05 | 268.5547 |      - | 1098.42 KB |        1.00 |
  *| DelaunayOptimized | 50   |  27.410 us | 0.4501 us |  0.6597 us |  0.06 |    0.00 |  13.0310 | 0.0305 |   53.32 KB |        0.05 |
  *
  **/

[MemoryDiagnoser]
public class DelaunaySubroutines
{
    [Params(10, 25, 50)]
    public int Size { get; set; }

    private Vertex2[] _vertices;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(123);
        _vertices = new Vertex2[Size];
        for (int i = 0; i < Size; i++)
        {
            _vertices[i] = new Vertex2(rnd.NextSingle(), rnd.NextSingle());
        }
    }

    [Benchmark(Baseline = true)]
    public (List<Vertex2> Vertices, List<TriangleElement> Interior, List<LineElement> Boundary) DelaunayDefault()
    {
        return DelaunayNaive.CreateTriangulation(_vertices).ToMesh();
    }

    [Benchmark]
    public (List<Vertex2> Vertices, List<TriangleElement> Interior, List<LineElement> Boundary) DelaunayOptimized()
    {
        return Delaunay.CreateTriangulation(_vertices).ToMesh();
    }
}
