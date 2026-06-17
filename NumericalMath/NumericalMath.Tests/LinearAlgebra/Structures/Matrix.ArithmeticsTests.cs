using NumericalMath.Exceptions;
using NumericalMath.LinearAlgebra.Structures;
using NumericalMath.LinearAlgebra.Structures.MatrixStorage;

namespace NumericalMath.Tests.LinearAlgebra.Structures;

[TestFixture]
public class MatrixArithmeticsTests
{
    [Test]
    public void AdditionMatrix_WhenDimensionsMismatch_ShouldThrow()
    {
        var matrix1 = new Matrix<int>(new[,] { { 7, 3, 2 }, { 9, 8, 6 } });
        var matrix2 = new Matrix<int>(new[,] { { 7, 9 }, { 3, 8 }, { 2, 6 } });

        Assert.That(() => { _ = matrix1 + matrix2; },
            Throws.Exception.TypeOf<DimensionMismatchException>());
    }
    
    [Test]
    public void SubtractionMatrix_WhenDimensionsMismatch_ShouldThrow()
    {
        var matrix1 = new Matrix<int>(new[,] { { 7, 3, 2 }, { 9, 8, 6 } });
        var matrix2 = new Matrix<int>(new[,] { { 7, 9 }, { 3, 8 }, { 2, 6 } });
        
        Assert.That(() => { _ = matrix1 + matrix2;}, 
            Throws.Exception.TypeOf<DimensionMismatchException>());
    }
    
    [Test]
    public void AdditionMatrix_WhenDimensionsMatch_ShouldReturnAdditionResult()
    {
        var matrix1 = new Matrix<int>(new[,] { { 7, 3, 2 }, { 9, 8, 6 } });
        var matrix2 =  new Matrix<int>(new[,] { { 1, 2, 3}, { 4, 5, 6 } });
        var result = new Matrix<int>(new[,] { {  8,  5,  5 }, { 13, 13, 12 } });

        Assert.That(matrix1 + matrix2, Is.EqualTo(result));
    }
    
    [Test]
    public void SubtractMatrix_WhenDimensionsMatch_ShouldReturnAdditionResult()
    {
        var matrix1 = new Matrix<int>(new[,] { { 7, 3, 2 }, { 9, 8, 6 } });
        var matrix2 =  new Matrix<int>(new[,] { { 1, 2, 3}, { 4, 5, 6 } });
        var result = new Matrix<int>(new[,] { {  6,  1, -1 }, { 5, 3, 0 } });

        Assert.That(matrix1 - matrix2, Is.EqualTo(result));
    }
    
    [Test]
    public void MultiplyMatrixColumn_WhenIdentityMatrix_ShouldReturnIdenticalColumnVector()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3, 4, 5]);
        Assert.That(Matrix<int>.Identity(5) * columnVector, Is.EqualTo(columnVector));
    }

    [Test]
    public void MultiplyMatrixColumn_WhenDimensionMismatch_ShouldThrow()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3, 4, 5]);
        Assert.Throws<DimensionMismatchException>(() => { _ = new Matrix<int>(5, 3) * columnVector; });
        Assert.Throws<DimensionMismatchException>(() => { _ = Matrix<int>.Identity(4) * columnVector; });
    }

    [Test]
    public void MultiplyMatrixColumn_WhenDimensionsMatch_ShouldReturnColumnVector()
    {
        var columnVector = new ColumnVector<int>([1, 2, 3, 4, 5]);
        var nonSquare = new Matrix<int>(new[,]
        {
            { 1, 2, 3, 4, 5 },
            { 6, 7, 8, 9, 10 },
            { 11, 12, 13, 14, 15 }
        });
        Assert.That(nonSquare * columnVector, Is.EqualTo(new ColumnVector<int>([55, 130, 205])));
    }

    [Test]
    public void MultiplyRowMatrix_WhenIdentityMatrix_ShouldReturnIdenticalRow()
    {
        var rowVector = new RowVector<int>([1, 2, 3, 4, 5]);
        Assert.That(rowVector * Matrix<int>.Identity(5), Is.EqualTo(rowVector));
    }

    [Test]
    public void MultiplyRowMatrix_WhenDimensionMismatch_ShouldThrow()
    {
        var rowVector = new RowVector<int>([1, 2, 3, 4, 5]);
        Assert.Throws<DimensionMismatchException>(() => { _ = rowVector * new Matrix<int>(3, 5); });
        Assert.Throws<DimensionMismatchException>(() => { _ = rowVector * Matrix<int>.Identity(4); });
    }

    [Test]
    public void MultiplyRowMatrix_WhenDimensionsMatch_ShouldReturnRowVector()
    {
        var rowVector = new RowVector<int>([1, 2, 3, 4, 5]);
        var nonSquare = new Matrix<int>(new[,]
        {
            { 1,  2,  3 },
            { 4,  5,  6 },
            { 7,  8,  9 },
            {10, 11, 12 },
            {13, 14, 15 }
        });
        var expected = new RowVector<int>([135, 150, 165]);
        Assert.That(rowVector * nonSquare, Is.EqualTo(expected));
    }

    [Test]
    public void MultiplyMatrixMatrix_WhenIdentityMatrix_ShouldReturnIdenticalMatrix()
    {
        var identity = Matrix<int>.Identity(3);
        Assert.That(identity * identity, Is.EqualTo(identity));

        var nonSquare = new Matrix<int>(new[,]
        {
            { 1, 2, 3, 4 },
            { 3, 2, 1, 5 },
            { 4, 5, 1, 5 },
        });

        Assert.That(nonSquare * Matrix<int>.Identity(4), Is.EqualTo(nonSquare));
        Assert.That(Matrix<int>.Identity(3) * nonSquare, Is.EqualTo(nonSquare));
    }

    [Test]
    public void MultiplyMatrixMatrix_WhenDimensionsMismatch_ShouldThrow()
    {
        var nonSquare = new Matrix<int>(new[,]
        {
            { 1, 2, 3, 4 },
            { 3, 2, 1, 5 },
            { 4, 5, 1, 5 },
        });
        var otherNonSquare = new Matrix<int>(new[,]
        {
            {1, 2, 3, 4, 3},
            {3, 2, 1, 5, 2},
            {4, 5, 1, 5, 1},
            {4, 5, 1, 5, 1},
        });
        Assert.Throws<DimensionMismatchException>(() => { _ = nonSquare * Matrix<int>.Identity(3); });
        Assert.Throws<DimensionMismatchException>(() => { _ = Matrix<int>.Identity(4) * nonSquare; });
        Assert.Throws<DimensionMismatchException>(() => { _ = otherNonSquare * nonSquare; });
    }
    
    [Test]
    public void MultiplyMatrixMatrix_WhenDimensionsMatch_ShouldReturnMatrix()
    {
        var nonSquare = new Matrix<int>(new[,]
        {
            { 1, 2, 3, 4 },
            { 3, 2, 1, 5 },
            { 4, 5, 1, 5 },
        });
        var otherNonSquare = new Matrix<int>(new[,]
        {
            {1, 2, 3, 4, 3},
            {3, 2, 1, 5, 2},
            {4, 5, 1, 5, 1},
            {4, 5, 1, 5, 1},
        });

        var productResult = new Matrix<int>(new[,]
        {
            {35, 41, 12, 49, 14},
            {33, 40, 17, 52, 19},
            {43, 48, 23, 71, 28}
        });

        Assert.That(nonSquare * otherNonSquare, Is.EqualTo(productResult));
    }
    
    [Test]
    public void MatrixOperations_WhenDifferentStoragesAreUsed_ShouldComputedCorrectValues()
    {
        var columnStorage = new ColumnMajorMatrixStorage<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        var rowStorage  = new RowMajorMatrixStorage<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        var columnMatrix = new Matrix<int>(columnStorage);
        var rowMatrix = new Matrix<int>(rowStorage);

        
        Matrix<int> resultSum =
        [
            [2, 4, 6],
            [8, 10, 12],
            [14, 16, 18]
        ];
        
        Matrix<int> resultSub = Matrix<int>.Zero(3, 3);

        Matrix<int> scalarProduct =
        [
            [5, 10, 15],
            [20, 25, 30],
            [35, 40, 45]
        ];

        Assert.That(columnMatrix + rowMatrix, Is.EqualTo(resultSum));
        Assert.That(columnMatrix - rowMatrix, Is.EqualTo(resultSub));
        Assert.That(5 * columnMatrix, Is.EqualTo(scalarProduct));
        Assert.That(5 * rowMatrix, Is.EqualTo(scalarProduct));
        Assert.That(columnMatrix * 5, Is.EqualTo(scalarProduct));
        Assert.That(rowMatrix * 5, Is.EqualTo(scalarProduct));
    }
}