using System;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.LinearAlgebra.Structures;
using NumericalMath.Tests.ExtensionHelpers;

namespace NumericalMath.Tests.LinearAlgebra.Structures;

[TestFixture]
public class MatrixTests
{
    [Test]
    public void Empty_WhenCompared_ShouldOnlyBeEmptyNotNull()
    {
        var empty = Matrix<int>.Empty;
        Assert.That(empty.Equals(null), Is.False);
        Assert.That(empty, Is.EqualTo(new Matrix<int>(0, 0)) );
        empty = [];
        Assert.That(empty, Is.EqualTo(new Matrix<int>(0, 0)) );
    }
    
    private static IEnumerable<TestCaseData> DimensionalMismatchMatrices()
    {
        Matrix<int> mat1 = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
        yield return new TestCaseData(mat1, (Matrix<int>)[[1, 2, 3], [4, 5, 6]]).SetArgDisplayNames("3x3 vs 2x3");
        yield return new TestCaseData(mat1, (Matrix<int>)[[1, 2], [4, 5], [7, 8]]).SetArgDisplayNames("3x3 vs 3x2");
        yield return new TestCaseData(mat1, (Matrix<int>)[[2, 2, 3], [4, 5, 6], [7, 8, 9]]).SetArgDisplayNames("3x3 vs 3x3, different entry");
    }
    
    [TestCaseSource(nameof(DimensionalMismatchMatrices))]
    public void Equals_WhenComparingDifferentMatrices_ShouldReturnFalse(Matrix<int> mat1, Matrix<int> mat2)
    {
        Assert.That(mat2, Is.Not.EqualTo(mat1));
    }
    
    [Test]
    public void Zero_WhenCalled_ShouldReturnOnlyZeroMatrixElements()
    {
        var empty = Matrix<int>.Zero(3);
        Assert.That(empty, Is.EqualTo(new Matrix<int>(new[,]
        {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
        })));
    }
    
    [Test]
    public void Constructor_WhenCreatedFromMultidimensionalArray_ShouldCreateMatrixAsRows()
    {
        int[,] values = { { 1, 2, 3 }, { 4, 5, 6 } };
        var matrix = new Matrix<int>(values);

        Assert.That(matrix.RowCount, Is.EqualTo(2));
        Assert.That(matrix.ColumnCount, Is.EqualTo(3));

        Assert.That(matrix[0, 0], Is.EqualTo(1));
        Assert.That(matrix[0, 1], Is.EqualTo(2));
        Assert.That(matrix[0, 2], Is.EqualTo(3));

        Assert.That(matrix[1, 0], Is.EqualTo(4));
        Assert.That(matrix[1, 1], Is.EqualTo(5));
        Assert.That(matrix[1, 2], Is.EqualTo(6));
    }
    
    [Test]
    public void Indexing_WhenOutOfRange_ShouldThrow()
    {
        var matrix = new Matrix<int>(5, 3);

        Assert.That(matrix.RowCount, Is.EqualTo(5));
        Assert.That(matrix.ColumnCount, Is.EqualTo(3));

        Assert.That(matrix[0, 0], Is.EqualTo(0));

        Assert.That(matrix[4, 2], Is.EqualTo(0));
        Assert.That(() => { _ = matrix[2, 4]; }, Throws.Exception.TypeOf<IndexOutOfRangeException>());
    }
    
    [Test]
    public void Column_WhenIndexInRange_ShouldReturnTheMatrixColumn()
    {
        var matrix = new Matrix<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 } });

        Assert.That(matrix.Column(0).ToArray(), Is.EqualTo(new[] { 1, 4 }));
        Assert.That(matrix.Column(1).ToArray(), Is.EqualTo(new[] { 2, 5 }));
        Assert.That(matrix.Column(2).ToArray(), Is.EqualTo(new[] { 3, 6 }));
    }

    [Test]
    public void Column_WhenIndexOutOfRange_ShouldThrow()
    {
        var matrix = new Matrix<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 } });
        Assert.That(() => { _ = matrix.Column(-1); }, Throws.Exception.TypeOf<IndexOutOfRangeException>());
        Assert.That(() => { _ = matrix.Column(3); }, Throws.Exception.TypeOf<IndexOutOfRangeException>());
    }

    [Test]
    public void Row_WhenIndexInRange_ShouldReturnTheMatrixColumn()
    {
        var matrix = new Matrix<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 } });
        Assert.That(matrix.Row(0).ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(matrix.Row(1).ToArray(), Is.EqualTo(new[] { 4, 5, 6 }));
    }

    [Test]
    public void Row_WhenIndexOutOfRange_ShouldThrow()
    {
        var matrix = new Matrix<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 } });
        Assert.That(() => { _ = matrix.Row(-1); }, Throws.Exception.TypeOf<IndexOutOfRangeException>());
        Assert.That(() => { _ = matrix.Row(2); }, Throws.Exception.TypeOf<IndexOutOfRangeException>());
    }

    [Test]
    public void Transpose_WhenIdentityMatrix_ShouldReturnItself()
    {
        var identity = Matrix<int>.Identity(5);
        Assert.That(identity.Transpose(), Is.EqualTo(identity));
    }

    [Test]
    public void Transpose_WhenNonSquareMatrix_ShouldReturnTransposedMatrix()
    {
        var nonSquare = new Matrix<int>(new[,]
        {
            {  1,  2,  3,  4,  5 },
            {  6,  7,  8,  9, 10 },
            { 11, 12, 13, 14, 15 }
        });
        var nonSquareTransposed = new Matrix<int>(new[,]
        {
            { 1,  6, 11 },
            { 2,  7, 12 },
            { 3,  8, 13 },
            { 4,  9, 14 },
            { 5, 10, 15 }
        });

        Assert.That(nonSquare.Transpose(), Is.EqualTo(nonSquareTransposed));
        Assert.That(nonSquare.Transpose().Transpose(), Is.EqualTo(nonSquare));
    }
    
    
    [TestCase(new[]{1,2,3,4,5,6,7,8,9}, 3, 3, ExpectedResult = new[]{1,5,9}, 
        TestName = "Diagonal_WhenSquareMatrix_ShouldReturnDiagonal")]
    [TestCase(new[]{1,2,3,4,5,6}, 3, 2, ExpectedResult = new[]{1,4}, 
        TestName = "Diagonal_WhenNonSquareMatrix_ShouldReturnDiagonal")]
    [TestCase(new[]{1,2,3,4,5,6}, 2, 3, ExpectedResult = new[]{1,5}, 
        TestName = "Diagonal_WhenNonSquareTransposedMatrix_ShouldReturnDiagonal")]
    public int[] Diagonal(int[] values, int rowCount, int columnCount)
    {
        var matrix = new Matrix<int>(rowCount, columnCount, values);
        return matrix.Diagonal().ToArray();
    }

    [Test]
    public void Copy_WhenMatrixIsCreated_ShouldHaveDistinctStorages()
    {
        Matrix<int> mat1 = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
        Matrix<int> mat2 = mat1.Copy();
        Assert.That(mat1, Is.EqualTo(mat2));
        
        // Change 1 element, mat2 should remain unaltered
        mat1[1, 1] = 3;
        Assert.That(mat1, Is.Not.EqualTo(mat2));
    }
    
    private static IEnumerable<TestCaseData> MatrixTraceSets
    {
        get
        {
            var matrix = new Matrix<float>(new float[,]{{ 6, -7},
                { 0,  3}});
            var trace = 9f;
            yield return new TestCaseData(matrix, trace).SetArgDisplayNames("Matrix2x2");

            matrix = new Matrix<float>(new float[,] {{ 1, 2, 3},
                { 3, 2, 1},
                { 2, 1, 3}});
            trace = 6;
            yield return new TestCaseData(matrix, trace).SetArgDisplayNames("Matrix3x3");

            matrix = new Matrix<float>(new[,]{ { 1.0f,   5.0f,  3.2f,  4.3f, 8.2f,  6.0f },
                { 1.0f,   4.3f,  5.6f,  2.4f, 5.1f, -5.0f },
                { 0.0f,   0.0f,  3.1f,  4.0f, 5.0f,  6.0f },
                { 0.0f,  18.0f,  0.1f,  2.0f, 5.0f,  8.0f },
                { 1.0f, -50.0f, -3.21f, 3.0f, 1.0f,  0.0f },
                { 0.0f, - 2.0f,  3.1f,  4.0f, 0.0f,  0.0f }});
            trace = 11.4f;
            yield return new TestCaseData(matrix, trace).SetArgDisplayNames("Matrix6x6");
        }
    }

    [TestCaseSource(nameof(MatrixTraceSets))]
    public void Trace_WhenMatrixIsSquare_ShouldReturnTrace(Matrix<float> matrix, float trace)
    {
        Assert.That(matrix.Trace(), Is.EqualTo(trace).Within(1e-5f));
        Assert.That(matrix.ToDoubles().Trace(), Is.EqualTo(trace).Within(1e-6d));
    }

    [Test]
    public void Random_WhenCalledTwice_ShouldReturnDifferentMatrices()
    {
        var mat1 = Matrix<float>.Random(3, 4);
        var mat2 = Matrix<float>.Random(3, 4);
        Assert.That(mat1, Is.Not.EqualTo(mat2));
    }

    [Test]
    public void GetEnumerator_WhenEnumerated_ShouldReturnMatrixRows()
    {
        var expectedRows  = new RowVector<int>[]{[1, 2, 3], [4, 5, 6], [7, 8, 9]};
        Matrix<int> mat1 = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
        using var enumerator = mat1.GetEnumerator();
        var rows = enumerator.ToStrongTypedEnumerable().ToArray();
        Assert.That(rows, Is.EquivalentTo(expectedRows));
    }



}