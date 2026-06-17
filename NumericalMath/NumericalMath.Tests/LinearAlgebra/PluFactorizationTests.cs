using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using System;
using NumericalMath.LinearAlgebra.Structures;
using NumericalMath.LinearAlgebra;
using NumericalMath.Exceptions;
using NumericalMath.Tests.ExtensionHelpers;

namespace NumericalMath.Tests.LinearAlgebra;

[TestFixture]
public class PluFactorizationTests
{

    public static IEnumerable<TestCaseData> FactorizationMatrices
    {
        get
        {
            var matrixElements = new float[,] {{ 6, -7},
                                             { 0,  3 }};
            var matrix = new Matrix<float>(matrixElements);
            int[] pivots = [0, 1];
            var permutations = 0;
            var lower = Matrix<float>.Identity(2);
            var upper = matrix.Copy();
            yield return new TestCaseData(matrix, pivots, permutations, lower, upper);

            matrixElements = new float[,] {{ 1, 2, 3},
                                    { 3, 2, 1},
                                    { 2, 1, 3}};
            matrix = new Matrix<float>(matrixElements);
            upper = new Matrix<float>(new[,]
            {
                { 3,    2, 1 },
                { 0, 4f/3, 8f/3 },
                { 0,    0, 3 }
            });
            lower = new Matrix<float>(new[,]
            {
                { 1,     0,    0 },
                { 1f/3,  1,    0 },
                { 2f/3, -1f/4, 1 }
            });
            pivots = [1, 0, 2];
            permutations = 1;
            yield return new TestCaseData(matrix, pivots, permutations, lower, upper);


            matrixElements = new[,] {{ 1.0f,   5.0f,  3.2f,  4.3f, 8.2f,  6.0f },
                                    { 1.0f,   4.3f,  5.6f,  2.4f, 5.1f, -5.0f },
                                    { 0.0f,   0.0f,  3.1f,  4.0f, 5.0f,  6.0f },
                                    { 0.0f,  18.0f,  0.1f,  2.0f, 5.0f,  8.0f },
                                    { 1.0f, -50.0f, -3.21f, 3.0f, 1.0f,  0.0f },
                                    { 0.0f, - 2.0f,  3.1f,  4.0f, 0.0f,  0.0f }};
             // [0, 1, 2, 3, 4, 5]
             // [0, 3, 2, 1, 4, 5]
             // [0, 3, 4, 1, 2, 5]
             // [0, 3, 4, 5, 2, 1]
             // [0, 3, 4, 5, 1, 2]
             
            matrix = new Matrix<float>(matrixElements);

            upper = new Matrix<float>(new[,]
            {
                { 1f,   5f,  3.2f    ,  4.3f    ,  8.2f    ,   6f },
                { 0,  -55f, -6.41f   , -1.3f    , -7.2f    ,  -6f },
                { 0,     0,  3.33309f,  4.04727f,  0.26182f,   0.21818f},
                { 0,     0,  0       , -4.89677f, -3.20329f, -11.08608f},
                { 0,     0,  0       ,  0       ,  4.60226f,   5.26332f},
                { 0,     0,  0       ,  0       ,  0       ,  -3.09968f}
            });
            lower = new Matrix<float>(new[,]
            {
                { 1, 0,        0,         0,        0,        0 },
                { 1, 1,        0,         0,        0,        0 },
                { 0, 0.03636f, 1,         0,        0,        0 },
                { 1, 0.01273f, 0.74453f,  1,        0,        0 },
                { 0, 0,        0.93007f, -0.04815f, 1,        0 },
                { 0,-0.32727f,-0.59939f, -0.81695f, 0.03990f, 1 }
            });

            pivots = [0, 4, 5, 1, 2, 3];
            permutations = 4;
            yield return new TestCaseData(matrix, pivots, permutations, lower, upper);
        }
    }

    [TestCaseSource(nameof(FactorizationMatrices))]
    public void PluFactorization_WhenMatrixIsInvertible_ShouldComputeCorrectLuDecomposition(Matrix<float> matrix, int[] expectedPivots, int expectedPermutations, Matrix<float> expectedL, Matrix<float> expectedU)
    {
        var (lu, pivots, permutations) = PluFactorizationOperations.PluFactorization(matrix, 1e-5f);

        Assert.That(pivots.SequenceEqual(expectedPivots), "Pivots not identical.\r\nExpected: [{0}]\r\nBut was: [{1}]", string.Join(",", expectedPivots), string.Join(",", pivots));
        Assert.That(permutations, Is.EqualTo(expectedPermutations));

        var lower = PluFactorizationOperations.ExtractLower(lu);
        var upper = PluFactorizationOperations.ExtractUpper(lu);

        Assert.That(lower, Is.EqualTo(expectedL).Using<Matrix<float>>((a, b) => a.ApproxEquals(b)));
        Assert.That(upper, Is.EqualTo(expectedU).Using<Matrix<float>>((a, b) => a.ApproxEquals(b)));


        // PA = LU
        Matrix<float> pa = new Matrix<float>(matrix.RowCount, matrix.ColumnCount);
        for (var i = 0; i < matrix.RowCount; i++)
        {
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                pa[i, j] = matrix[pivots[i], j];
            }
        }
        Assert.That(lower * upper, Is.EqualTo(pa).Using<Matrix<float>>((a, b) => a.ApproxEquals(b)));
    }

    public static IEnumerable<TestCaseData> LinearSystemSets
    {
        get
        {
            var matrixValues = new float[,] {{ 6, -7},
                                             { 0,  3 }};
            var matrix = new Matrix<float>(matrixValues);
            var b = new ColumnVector<float>([3, 4]);
            var result = new ColumnVector<float>([37f / 18f, 4f / 3f]);
            yield return new TestCaseData(matrix, b, result);
            
            matrixValues = new float[,] {{ 1, 2, 3},
                                    { 3, 2, 1},
                                    { 2, 1, 3}};
            matrix = new Matrix<float>(matrixValues);
            b = new ColumnVector<float>([1, 2, 2]);
            result = new ColumnVector<float>([0.75f, -0.25f, 0.25f]);
            yield return new TestCaseData(matrix, b, result);

            matrixValues = new[,] {{ 1.0f,   5.0f,  3.2f,  4.3f, 8.2f,  6.0f },
                                    { 1.0f,   4.3f,  5.6f,  2.4f, 5.1f, -5.0f },
                                    { 0.0f,   0.0f,  3.1f,  4.0f, 5.0f,  6.0f },
                                    { 0.0f,  18.0f,  0.1f,  2.0f, 5.0f,  8.0f },
                                    { 1.0f, -50.0f, -3.21f, 3.0f, 1.0f,  0.0f },
                                    { 0.0f, - 2.0f,  3.1f,  4.0f, 0.0f,  0.0f }};

            float[] bValues = [1.0f, 2.0f, 3.3f, 4.4f, 5.5f, 6.6f];
            float[] results =
            [
                -10.2777f,
                  0.214471f,
                 -3.5016f,
                  4.47097f,
                  1.8482f,
                 -2.16166f
            ];
            matrix = new Matrix<float>(matrixValues);
            b = new ColumnVector<float>(bValues);
            result = new ColumnVector<float>(results);
            yield return new TestCaseData(matrix, b, result);
        }
    }

    [TestCaseSource(nameof(LinearSystemSets))]
    public void SolveUsingPlu_WhenMatrixIsInvertible_ShouldComputeCorrectSolution(Matrix<float> matrix, ColumnVector<float> b, ColumnVector<float> result)
    {
        ColumnVector<float> x2 = PluFactorizationOperations.SolveUsingPlu(matrix, b, 1e-5f);
        Assert.That(x2, Is.EqualTo(result).Using<AbstractVector<float>>((x, y) => x.ApproxEquals(y, 5e-5f)));
    }

    [Test]
    public void Invert_WhenMatrixIsNotInvertible_ShouldThrowException()
    {
        Matrix<float> mat =
        [
            [1, 2, 3],
            [4, 5, 6],
            [7, 8, 9]
        ];
        Assert.That(() => mat.Inverse(), Throws.Exception.TypeOf<DegenerateMatrixException>());
    }

    public static IEnumerable<TestCaseData> InverseMatricesSets
    {
        get
        {
            yield return new TestCaseData(
                new Matrix<float>(new[,]
                {
                    { 1f, 2f },
                    { 3f, 4f }
                }),
                new Matrix<float>(new[,]
                {
                    {-2.0f,  1.0f },
                    { 1.5f, -0.5f }
                }));

            yield return new TestCaseData(
                new Matrix<float>(new[,]
                {
                    { 1.0f,   5.0f,  3.2f,  4.3f, 8.2f,  6.0f },
                    { 1.0f,   4.3f,  5.6f,  2.4f, 5.1f, -5.0f },
                    { 0.0f,   0.0f,  3.1f,  4.0f, 5.0f,  6.0f },
                    { 0.0f,  18.0f,  0.1f,  2.0f, 5.0f,  8.0f },
                    { 1.0f, -50.0f, -3.21f, 3.0f, 1.0f,  0.0f },
                    { 0.0f, - 2.0f,  3.1f,  4.0f, 0.0f,  0.0f }
                }),
                new Matrix<float>(new[,]
                {
                    {  2.48269f,  -1.08299f,   -1.39254f,   -1.49448f,   -0.399701f,   0.420458f },
                    { -0.0065064f, 0.0108966f, -0.0450246f,  0.0454587f, -0.00439019f, 0.0260444f },
                    {  0.426931f, -0.234924f,   0.179534f,  -0.601676f,  -0.192006f,  -0.0526869f },
                    { -0.334124f,  0.187515f,  -0.161651f,   0.489028f,   0.14661f,    0.303855f },
                    { -0.435192f,  0.311171f,   0.202564f,   0.368953f,   0.124022f,  -0.198927f },
                    {  0.364829f, -0.262941f,   0.0128716f, -0.322614f,  -0.101888f,  -0.00957533f }
                }));
        }
    }

    [TestCaseSource(nameof(InverseMatricesSets))]
    public void SolveUsingPlu_WhenMatrixIsInvertible_ShouldComputeCorrectInverseMatrix(Matrix<float> matrix, Matrix<float> expectedInverseMatrix)
    {
        Assert.That(PluFactorizationOperations.SolveUsingPlu(matrix, Matrix<float>.Identity(matrix.RowCount), Constants.DefaultFloatTolerance), Is.EqualTo(expectedInverseMatrix)
            .Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-5f)));
    }

    [TestCaseSource(nameof(LinearSystemSets))]
    public void SolveUsingPlu_WhenRhsIsVector_ShouldComputeCorrectLinearSystemSolution(Matrix<float> matrix, ColumnVector<float> b, ColumnVector<float> result)
    {
        Assert.That(PluFactorizationOperations.SolveUsingPlu(matrix, matrix, Constants.DefaultFloatTolerance), Is.EqualTo(Matrix<float>.Identity(matrix.RowCount))
            .Using<Matrix<float>>((x,y) => x.ApproxEquals(y)));
    }

    [TestCase(5)]
    [TestCase(10)]
    [TestCase(50)]
    [TestCase(100)]
    public void SolveUsingPlu_WhenMatrixVectorSizeIsDifferent_ShouldComputeLinearSystem(int n)
    {
        Randomizer randomizer = new(1234567890);
        Matrix<float> matrix = GenerateMatrix(n, n, randomizer.NextFloat);
        ColumnVector<float> b = GenerateColumnVector(n, randomizer.NextFloat);

        ColumnVector<float> x = PluFactorizationOperations.SolveUsingPlu(matrix, b, Constants.DefaultFloatTolerance);

        Assert.That(x.RowCount, Is.EqualTo(n));
        Assert.That(matrix * x, Is.EqualTo(b).Using<AbstractVector<float>>((left, right) => left.ApproxEquals(right, 5e-5f)));
    }

    [TestCase(5, 5)]
    [TestCase(10, 5)]
    [TestCase(50, 40)]
    [TestCase(100, 70)]
    public void SolveUsingPlu_WhenMatrixMatrixSizeIsDifferent_ShouldComputeLinearSystem(int n, int m)
    {
        Randomizer randomizer = new(1234567890);

        Matrix<float> matrixA = GenerateMatrix(n, n, randomizer.NextFloat);
        Matrix<float> matrixB = GenerateMatrix(n, m, randomizer.NextFloat);

        Matrix<float> matrixResult = PluFactorizationOperations.SolveUsingPlu(matrixA, matrixB, Constants.DefaultFloatTolerance);

        Assert.That((matrixResult.RowCount, matrixResult.ColumnCount), Is.EqualTo((n, m)));
        Assert.That(matrixA * matrixResult, Is.EqualTo(matrixB).Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 1e-4f)));
    }

    private static IEnumerable<TestCaseData> ForwardSubstitutionVectorSets()
    {
        yield return new TestCaseData((Matrix<int>)[[1, 0], [2, 1]], (ColumnVector<int>)[1, 5])
            .Returns((ColumnVector<int>)[1, 3]);
        
        yield return new TestCaseData((Matrix<int>)[[1, 0, 0, 0], [2, 1, 0, 0], [3, 4, 1, 0], [1, 2, 3, 1]], (ColumnVector<int>)[1, 5, 3, 4])
            .Returns((ColumnVector<int>)[1, 3, -12, 33]);
    }
    
    private static IEnumerable<TestCaseData> ForwardSubstitutionMatrixSets()
    {
        yield return new TestCaseData((Matrix<int>)[[1, 0], [2, 1]], (Matrix<int>)[[1, 2],[5,10]])
            .Returns((Matrix<int>)[[1,2], [3,6]]);
        
        yield return new TestCaseData((Matrix<int>)[[1, 0, 0, 0], [2, 1, 0, 0], [3, 4, 1, 0], [1, 2, 3, 1]], 
                (Matrix<int>)[[1,2,4,8], [5,10,20,40], [3,6,12,24], [4,8,16,32]])
            .Returns((Matrix<int>)[[1,2,4,8], [3,6,12,24], [-12,-24,-48,-96], [33,66,132,264]]);
    }
    
    private static IEnumerable<TestCaseData> BackwardSubstitutionVectorSets()
    {
        yield return new TestCaseData((Matrix<int>)[[3, 4], [0, 2]], (ColumnVector<int>)[3, 6])
            .Returns((ColumnVector<int>)[-3, 3]);
        
        yield return new TestCaseData((Matrix<int>)[[3, 4, 5, 6], [0, 2, 4, 2], [0, 0, 6, 5], [0, 0, 0, 2]], (ColumnVector<int>)[3, 6, 3, 6])
            .Returns((ColumnVector<int>)[-7,4, -2, 3]);
    }
    
    private static IEnumerable<TestCaseData> BackwardSubstitutionMatrixSets()
    {
        yield return new TestCaseData((Matrix<int>)[[3, 4], [0, 2]], (Matrix<int>)[[3, 6], [6, 12]])
            .Returns((Matrix<int>)[[-3, -6], [3,6]]);
        
        yield return new TestCaseData((Matrix<int>)[[3, 4, 5, 6], [0, 2, 4, 2], [0, 0, 6, 5], [0, 0, 0, 2]], 
                (Matrix<int>)[[3, 6, 12], [6, 12, 24], [3, 6, 12], [6, 12, 24]])
            .Returns((Matrix<int>)[[-7, -14, -28],[4, 8, 16], [-2, -4, -8], [3, 6, 12]]);
    }

    [TestCaseSource(nameof(ForwardSubstitutionVectorSets))]
    public ColumnVector<int> ForwardSubstitution_WhenRhsIsVector_ShouldReturnCorrectVector(Matrix<int> lowerTriangle, ColumnVector<int> rhs) 
        => PluFactorizationOperations.ForwardSubstitution(lowerTriangle, rhs);


    [TestCaseSource(nameof(BackwardSubstitutionVectorSets))]
    public ColumnVector<int> BackwardSubstitution_WhenRhsIsVector_ShouldReturnCorrectVector(Matrix<int> upper, ColumnVector<int> rhs) 
        => PluFactorizationOperations.BackwardSubstitution(upper, rhs);

    [TestCaseSource(nameof(ForwardSubstitutionMatrixSets))]
    public Matrix<int> ForwardSubstitution_WhenRhsIsMatrix_ShouldReturnCorrectMatrix(Matrix<int> lowerTriangle, Matrix<int> rhs) 
        => PluFactorizationOperations.ForwardSubstitution(lowerTriangle, rhs);

    [TestCaseSource(nameof(BackwardSubstitutionMatrixSets))]
    public Matrix<int> BackwardSubstitution_WhenRhsIsMatrix_ShouldReturnCorrectMatrix(Matrix<int> upper, Matrix<int> rhs) 
        => PluFactorizationOperations.BackwardSubstitution(upper, rhs);

    private static Matrix<T> GenerateMatrix<T>(int n, int m, Func<T> randomGenerator) where T : struct, System.Numerics.INumber<T>
    {
        Matrix<T> matrix = new(n, m);
        for (int i = 0; i < n; i++)
        {
            for (var j = 0; j < m; j++)
            {
                matrix[i, j] = randomGenerator();
            }
        }
        return matrix;
    }

    private static ColumnVector<T> GenerateColumnVector<T>(int n, Func<T> randomGenerator) where T : struct, System.Numerics.INumber<T>
    {
        ColumnVector<T> vector = new(n);
        for (int i = 0; i < n; i++)
        {
            vector[i] = randomGenerator();
        }
        return vector;
    }
}
