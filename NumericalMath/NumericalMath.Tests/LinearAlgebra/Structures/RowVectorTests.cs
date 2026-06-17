using System;
using NumericalMath.Exceptions;
using NumericalMath.LinearAlgebra.Structures;

namespace NumericalMath.Tests.LinearAlgebra.Structures;

[TestFixture]
internal class RowVectorTests
{
    
    [TestCase(new[]{1,2,3,4}, new[]{1,2,3,5},
        TestName = "Equals_WhenDifferentEntries_ShouldReturnFalse")]
    [TestCase(new[]{1,2,3,4}, new[]{1,2,3},
        TestName = "Equals_WhenDifferentSize_ShouldReturnFalse")]
    public void NotEqualRows(int[] array1, int[] array2)
    {
        var rowVector1 = new RowVector<int>(array1);
        var rowVector2 = new RowVector<int>(array2);
        Assert.That(rowVector1, Is.Not.EqualTo(rowVector2));
    }
    
    [Test]
    public void Constructor_WhenNoValuesSet_ShouldOnlyContainZero()
    {
        RowVector<int> rowVector = new RowVector<int>(4);
        Assert.That(rowVector.Length, Is.EqualTo(4));
        Assert.That(rowVector.RowCount, Is.EqualTo(1));
        Assert.That(rowVector.ColumnCount, Is.EqualTo(4));

        Assert.Throws<IndexOutOfRangeException>(() => { _ = rowVector[-1]; });
        Assert.That(rowVector[0], Is.EqualTo(0));
        Assert.That(rowVector[1], Is.EqualTo(0));
        Assert.That(rowVector[2], Is.EqualTo(0));
        Assert.That(rowVector[3], Is.EqualTo(0));
        Assert.Throws<IndexOutOfRangeException>(() => { _ = rowVector[4]; });

        Assert.That(rowVector.Equals(null), Is.False);
    }
    
    [Test]
    public void Constructor_WhenConstantScalar_ShouldReturnConstantScalar()
    {
        var rowVector = new RowVector<int>(5, 0);
        Assert.That(rowVector, Is.EqualTo(RowVector<int>.Zero(5)));

        rowVector = new RowVector<int>(5, 4);
        Assert.That(rowVector.ToArray(), Is.EqualTo(new[] { 4, 4, 4, 4, 4 }));
    }
    
    [Test]
    public void Random_WhenTwoVectorsAreCreated_ShouldNotBeEqual()
    {
        var randomRow1 = RowVector<double>.Random(5);
        var randomRow2 = RowVector<double>.Random(5);
        Assert.That(randomRow1, Is.Not.EqualTo(randomRow2));
    }
    
    [Test]
    public void ExplicitCast_WhenIntArray_ShouldReturnVectorWithEntries()
    {
        int[] values = [1, 2, 3, 4, 5];
        var row = (RowVector<int>)values;
        Assert.That(row.ToArray(), Is.EqualTo(values));
    }
    
    [Test]
    public void Copy_WhenCreated_ShouldCreateDistinctDeepCopy()
    {
        var rowVector = new RowVector<int>([1, 2, 3, 4]);
        var rowVector2 = rowVector.Copy();
        
        Assert.That(rowVector, Is.EqualTo(rowVector2));
        rowVector[0] = 5;
        Assert.That(rowVector, Is.Not.EqualTo(rowVector2));
    }

    [Test]
    public void Indexing_WhenVectorIndex_ShouldReturnVectorEntries()
    {
        RowVector<int> rowVector = new RowVector<int>([1, 2, 3, 4]);
        Assert.That(rowVector.Length, Is.EqualTo(4));
        Assert.That(rowVector.RowCount, Is.EqualTo(1));
        Assert.That(rowVector.ColumnCount, Is.EqualTo(4));

        Assert.Throws<IndexOutOfRangeException>(() => { _ = rowVector[-1]; });
        Assert.That(rowVector[0], Is.EqualTo(1));
        Assert.That(rowVector[1], Is.EqualTo(2));
        Assert.That(rowVector[2], Is.EqualTo(3));
        Assert.That(rowVector[3], Is.EqualTo(4));
        Assert.Throws<IndexOutOfRangeException>(() => { _ = rowVector[4]; });
        
        rowVector[0] = 5;
        rowVector[1] = 6;
        rowVector[2] = 7;
        rowVector[3] = 8;
        
        Assert.That(rowVector[0], Is.EqualTo(5));
        Assert.That(rowVector[1], Is.EqualTo(6));
        Assert.That(rowVector[2], Is.EqualTo(7));
        Assert.That(rowVector[3], Is.EqualTo(8));
    }

    [Test]
    public void Indexing_WhenMatrixIndexing_ShouldReturnVectorEntries()
    {
        RowVector<int> rowVector = new RowVector<int>([1, 2, 3, 4]);

        Assert.Throws<IndexOutOfRangeException>(() => { rowVector[1, 0] = 1; });
        Assert.Throws<IndexOutOfRangeException>(() => { _ = rowVector[1, 0]; });
        Assert.Throws<IndexOutOfRangeException>(() => { rowVector[0, 4] = 1; });
        Assert.Throws<IndexOutOfRangeException>(() => { _ = rowVector[0, 4]; });

        Assert.That(rowVector[0, 0], Is.EqualTo(1));
        Assert.That(rowVector[0, 1], Is.EqualTo(2));
        Assert.That(rowVector[0, 2], Is.EqualTo(3));
        Assert.That(rowVector[0, 3], Is.EqualTo(4));

        rowVector[0, 0] = 5;
        rowVector[0, 1] = 6;
        rowVector[0, 2] = 7;
        rowVector[0, 3] = 8;

        Assert.That(rowVector[0], Is.EqualTo(5));
        Assert.That(rowVector[1], Is.EqualTo(6));
        Assert.That(rowVector[2], Is.EqualTo(7));
        Assert.That(rowVector[3], Is.EqualTo(8));
    }

    [Test]
    public void Transpose_WhenTransposed_ShouldReturnColumnVector()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3]);
        RowVector<int> rowVector = new RowVector<int>([1, 2, 3]);

        Assert.That(rowVector.Transpose(), Is.EqualTo(columnVector));
        Assert.That(rowVector.Transpose().Transpose(), Is.EqualTo(rowVector));
    }

    [Test]
    public void Addition_WhenDimensionsMatch_ShouldReturnVectorSum()
    {
        var rowVector1 = new RowVector<int>([1, 2, 3]);
        var rowVector2 = new RowVector<int>([4, 5, 6]);
        var result = new RowVector<int>([5, 7, 9]);

        Assert.That(rowVector1 + rowVector2, Is.EqualTo(result));
        
        rowVector1 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];
        result = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40];
        
        Assert.That(rowVector1 + rowVector1, Is.EqualTo(result));
        
    }

    [Test]
    public void Subtraction_WhenDimensionsMatch_ShouldReturnVectorSubtraction()
    {
        var rowVector1 = new RowVector<int>([1, 2, 3]);
        var rowVector2 = new RowVector<int>([4, 5, 6]);
        var result = new RowVector<int>([-3, -3, -3]);
        
        Assert.That(rowVector1 - rowVector2, Is.EqualTo(result));
        
        rowVector1 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];
        result = RowVector<int>.Zero(20);
        Assert.That(rowVector1 - rowVector1, Is.EqualTo(result));
    }
    
    [Test]
    public void Addition_WhenDimensionsMisMatch_ShouldThrow()
    {
        Assert.Throws<DimensionMismatchException>(() => { _ = new RowVector<int>([1, 2, 3]) + new RowVector<int>([1, 2]); });
    }
        
    [Test]
    public void Subtraction_WhenDimensionsMisMatch_ShouldThrow()
    {
        Assert.Throws<DimensionMismatchException>(() => { _ = new RowVector<int>([1, 2, 3]) - new RowVector<int>([1, 2]); });
    }
    
    [Test]
    public void Multiply_WhenVectorScalar_ShouldReturnScaledVector()
    {
        var rowVector = new RowVector<int>([4, 5, 6]);

        Assert.That(rowVector * 2, Is.EqualTo(new RowVector<int>([8, 10, 12])));
        Assert.That(2 * rowVector, Is.EqualTo(new RowVector<int>([8, 10, 12])));
        Assert.That(2 * rowVector * 3, Is.EqualTo(new RowVector<int>([24, 30, 36])));
        
        rowVector = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];
        RowVector<int> result = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40];
        
        Assert.That(rowVector * 2, Is.EqualTo(result));
        Assert.That(2 * rowVector, Is.EqualTo(result));
    }
    
    [Test]
    public void Operations_WhenStridesAreDifferent_ShouldBehaveIdenticalToSameStride()
    {
        var rowVector1 = new RowVector<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3);
        var rowVector2 = new RowVector<int>(new[] { 1, 2, 3, 4, 5, 6 }, 2);
        
        Assert.That(rowVector1, Has.Length.EqualTo(3));
        Assert.That(rowVector2, Has.Length.EqualTo(3));

        Assert.That((rowVector1 + rowVector2).ToArray(), Is.EqualTo(new[] { 2, 7, 12 }));
        Assert.That((rowVector1 - rowVector2).ToArray(), Is.EqualTo(new[] { 0, 1, 2 }));
        Assert.That((5 * rowVector1).ToArray(), Is.EqualTo(new[] { 5, 20, 35 }));
        Assert.That((4 * rowVector2).ToArray(), Is.EqualTo(new[] { 4, 12, 20 }));
    }
    
}