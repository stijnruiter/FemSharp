using BenchmarkDotNet.Attributes;
using NumericalMath.LinearAlgebra;
using NumericalMath.LinearAlgebra.Structures;
using System.Numerics;

namespace Benchmarks;

/**
 * Results 18/11/2024
 * BenchmarkDotNet v0.14.0, Windows 11
 * AMD Ryzen 5 5500H with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
 * .NET SDK 8.0.303
 *  [Host]     : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
 *  DefaultJob : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
 * 
| Method             | Size | Mean                | Error             | StdDev            | Median              | Ratio    | RatioSD |
|------------------- |----- |--------------------:|------------------:|------------------:|--------------------:|---------:|--------:|
| Addition           | 10   |            49.90 ns |          0.707 ns |          0.662 ns |            49.87 ns |     1.00 |    0.02 |
| MathNetAddition    | 10   |            84.65 ns |          1.615 ns |          1.511 ns |            84.83 ns |     1.70 |    0.04 |
| SolveMatrix        | 10   |         7,101.38 ns |        141.777 ns |        132.618 ns |         7,040.48 ns |   142.34 |    3.15 |
| MathNetSolveMatrix | 10   |         1,507.03 ns |         19.703 ns |         17.467 ns |         1,505.91 ns |    30.21 |    0.51 |
| SolveVector        | 10   |         1,814.56 ns |          7.257 ns |          6.433 ns |         1,814.31 ns |    36.37 |    0.48 |
| MathNetSolveVector | 10   |           739.83 ns |          4.336 ns |          3.844 ns |           739.38 ns |    14.83 |    0.20 |
|                    |      |                     |                   |                   |                     |          |         |
| Addition           | 100  |         2,064.80 ns |         32.297 ns |         30.210 ns |         2,058.55 ns |     1.00 |    0.02 |
| MathNetAddition    | 100  |         4,283.07 ns |         24.397 ns |         21.627 ns |         4,281.50 ns |     2.07 |    0.03 |
| SolveMatrix        | 100  |       858,174.10 ns |      4,570.073 ns |      4,274.849 ns |       858,723.14 ns |   415.70 |    6.19 |
| MathNetSolveMatrix | 100  |     1,011,176.86 ns |      3,627.130 ns |      3,215.357 ns |     1,011,184.28 ns |   489.82 |    7.07 |
| SolveVector        | 100  |       165,420.39 ns |      1,189.420 ns |      1,112.585 ns |       165,333.74 ns |    80.13 |    1.24 |
| MathNetSolveVector | 100  |       235,358.95 ns |        877.162 ns |        820.498 ns |       234,944.36 ns |   114.01 |    1.65 |
|                    |      |                     |                   |                   |                     |          |         |
| Addition           | 500  |       158,611.84 ns |      1,917.702 ns |      1,793.820 ns |       158,488.17 ns |     1.00 |    0.02 |
| MathNetAddition    | 500  |       151,613.42 ns |      1,187.020 ns |      1,110.340 ns |       151,867.92 ns |     0.96 |    0.01 |
| SolveMatrix        | 500  |    42,887,446.53 ns |    217,423.527 ns |    169,749.989 ns |    42,908,283.33 ns |   270.42 |    3.14 |
| MathNetSolveMatrix | 500  |   156,459,301.67 ns |    557,019.511 ns |    990,101.693 ns |   156,153,566.67 ns |   986.55 |   12.44 |
| SolveVector        | 500  |     7,876,395.57 ns |    149,404.129 ns |    159,860.745 ns |     7,860,850.00 ns |    49.66 |    1.12 |
| MathNetSolveVector | 500  |    30,316,015.62 ns |     96,851.401 ns |     90,594.860 ns |    30,303,637.50 ns |   191.16 |    2.17 |
|                    |      |                     |                   |                   |                     |          |         |
| Addition           | 1000 |       872,315.43 ns |     25,486.357 ns |     75,147.104 ns |       886,235.11 ns |     1.01 |    0.12 |
| MathNetAddition    | 1000 |       929,398.83 ns |     18,444.329 ns |     52,920.243 ns |       950,252.73 ns |     1.07 |    0.11 |
| SolveMatrix        | 1000 |   280,622,045.24 ns |  5,520,913.560 ns |  6,572,255.446 ns |   276,135,400.00 ns |   324.06 |   28.69 |
| MathNetSolveMatrix | 1000 | 1,505,439,373.33 ns | 15,838,850.265 ns | 14,815,670.372 ns | 1,502,964,800.00 ns | 1,738.46 |  149.58 |
| SolveVector        | 1000 |    49,954,762.24 ns |    505,958.049 ns |    422,497.886 ns |    50,001,636.36 ns |    57.69 |    4.96 |
| MathNetSolveVector | 1000 |   249,083,962.22 ns |  1,566,266.507 ns |  1,465,086.663 ns |   248,750,566.67 ns |   287.64 |   24.65 |
*
*/

public class MatrixOptimization<T> where T : struct, INumber<T>
{
    [Params(10, 100, 500, 1000)]
    public int Size { get; set; }

    private Matrix<T> left;
    private Matrix<T> right;
    private ColumnVector<T> rightVec;

    private MathNet.Numerics.LinearAlgebra.Matrix<float> mLeft;
    private MathNet.Numerics.LinearAlgebra.Matrix<float> mRight;
    private MathNet.Numerics.LinearAlgebra.Vector<float> mRightVec;


    [GlobalSetup]
    public void Setup()
    {
        left = Matrix<T>.Random(Size, Size);
        right = Matrix<T>.Random(Size, Size);
        rightVec = ColumnVector<T>.Random(Size);

        mLeft = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.Random(Size, Size, 12345);
        mRight = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.Random(Size, Size, 45678);
        mRightVec = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Random(Size, 234567);
    }

    [Benchmark(Baseline = true)]
    public Matrix<T> Addition() => left + right;

    [Benchmark]
    public MathNet.Numerics.LinearAlgebra.Matrix<float> MathNetAddition() => mLeft + mRight;

    [Benchmark]
    public Matrix<T> SolveMatrix() => PluFactorizationOperations.SolveUsingPlu(left, right, T.Zero);

    [Benchmark]
    public MathNet.Numerics.LinearAlgebra.Matrix<float> MathNetSolveMatrix() => mLeft.Solve(mRight);

    [Benchmark]
    public ColumnVector<T> SolveVector() => PluFactorizationOperations.SolveUsingPlu(left, rightVec, T.Zero);

    [Benchmark]
    public MathNet.Numerics.LinearAlgebra.Vector<float> MathNetSolveVector() => mLeft.Solve(mRightVec);
}
