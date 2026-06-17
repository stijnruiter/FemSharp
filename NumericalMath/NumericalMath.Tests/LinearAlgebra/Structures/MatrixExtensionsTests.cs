using System;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.LinearAlgebra.Structures;
using NumericalMath.Tests.ExtensionHelpers;

namespace NumericalMath.Tests.LinearAlgebra.Structures;

public class MatrixExtensionsTests
{
    
    
    [Test]
    public void RandomNumbers_WhenTypeIsSupported_ShouldReturnMemoryOfRandomNumbers()
    {
        var repeatableRandom = new Random(12345);
        var intValues1 = repeatableRandom.RandomNumbers<int>(12);
        var intValues2 = repeatableRandom.RandomNumbers<int>(12);
        Assert.That(intValues1.ToArray().Zip(intValues2.ToArray()).Any(value => value.First == value.Second), Is.False);
        
        var singleValues1 = repeatableRandom.RandomNumbers<float>(12);
        var singleValues2 = repeatableRandom.RandomNumbers<float>(12);
        Assert.That(singleValues1.ToArray().Zip(singleValues2.ToArray()).Any(ApproxEqualsSingle), Is.False);
        
        var doubleValues1 = repeatableRandom.RandomNumbers<double>(12);
        var doubleValues2 = repeatableRandom.RandomNumbers<double>(12);        
        Assert.That(doubleValues1.ToArray().Zip(doubleValues2.ToArray()).Any(ApproxEqualsDouble), Is.False);
        
        return;
        bool ApproxEqualsSingle((float a, float b) val) => Math.Abs(val.a - val.b) < 1e-6f;
        bool ApproxEqualsDouble((double a, double b) val) => Math.Abs(val.a - val.b) < 1e-6d;
    }
    
    [Test]
    public void RandomNumbers_WhenTypeIsNotSupported_ShouldReturnArrayOfRandomNumbers()
    {
        Assert.That(() => TestContext.CurrentContext.Random.RandomNumbers<byte>(12), Throws.Exception.TypeOf<NotSupportedException>());
    }
    
    [TestCaseSource(typeof(PluFactorizationTests), nameof(PluFactorizationTests.LinearSystemSets))]
    public void SolveMatrixVector_WhenUsingSingles_ShouldReturnTheSystemSolution(Matrix<float> matrix, ColumnVector<float> rhs, ColumnVector<float> result)
    {
        var x = matrix.Solve(rhs);
        Assert.That(x, Is.EqualTo(result).Using<AbstractVector<float>>((a, b) => a.ApproxEquals(b, 5e-5f)));
    }

    [TestCaseSource(typeof(PluFactorizationTests), nameof(PluFactorizationTests.LinearSystemSets))]
    public void SolveMatrixVector_WhenUsingDoubles_ShouldReturnTheSystemSolution(Matrix<float> matrix,
        ColumnVector<float> rhs, ColumnVector<float> result)
    {
        var results = ToDoubles(result);
        var x2 = matrix.ToDoubles().Solve(ToDoubles(rhs));        
        Assert.That(x2, Is.EqualTo(results).Using<AbstractVector<double>>((a, b) => a.ApproxEquals(b, 5e-5f)));
    }

    private static ColumnVector<double> ToDoubles(ColumnVector<float> values)
    {
        var result = new ColumnVector<double>(values.RowCount);
        for (int i = 0; i < values.RowCount; i++)
        {
            result[i] = values[i];
        }
        return result;
    }

    [TestCaseSource(typeof(PluFactorizationTests), nameof(PluFactorizationTests.InverseMatricesSets))]
    public void InverseMatrixExtension(Matrix<float> matrix, Matrix<float> invA)
    {
        Assert.That(matrix.Inverse(), Is.EqualTo(invA).Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-6f)));
        Assert.That(matrix.ToDoubles().Inverse().ToSingles(), Is.EqualTo(invA).Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-6f)));
    }

    
    public static IEnumerable<TestCaseData> MatrixDeterminantSets
    {
        get
        {
            var matrix = new Matrix<float>(new float[,] { { 6, -7 }, { 0, 3 } });
            float determinant = 18f;
            yield return new TestCaseData(matrix, determinant, determinant).SetArgDisplayNames("Matrix2x2");

            matrix = new Matrix<float>(new float[,] {{ 1, 2, 3}, { 3, 2, 1}, { 2, 1, 3}});
            determinant = -12;
            yield return new TestCaseData(matrix, determinant, determinant).SetArgDisplayNames("Matrix3x3");


            matrix = new Matrix<float>(new[,]{ { 1.0f,   5.0f,  3.2f,  4.3f, 8.2f,  6.0f },
                { 1.0f,   4.3f,  5.6f,  2.4f, 5.1f, -5.0f },
                { 0.0f,   0.0f,  3.1f,  4.0f, 5.0f,  6.0f },
                { 0.0f,  18.0f,  0.1f,  2.0f, 5.0f,  8.0f },
                { 1.0f, -50.0f, -3.21f, 3.0f, 1.0f,  0.0f },
                { 0.0f, - 2.0f,  3.1f,  4.0f, 0.0f,  0.0f }});
            determinant = -12805.829f;
            double determinant2 = -12805.830597d;
            yield return new TestCaseData(matrix, determinant, determinant2).SetArgDisplayNames("Matrix6x6");
        }
    }

    [TestCaseSource(nameof(MatrixDeterminantSets))]
    public void Determinant_WhenMatrixIsValid_ShouldComputeDeterminant(Matrix<float> matrix, float determinant, double determinant2)
    {
        Assert.That(matrix.Determinant(), Is.EqualTo(determinant).Within(1e-5f));
        Assert.That(matrix.ToDoubles().Determinant(), Is.EqualTo(determinant2).Within(1e-6d));
    }

}