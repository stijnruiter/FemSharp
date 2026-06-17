using System;
using NumericalMath.Exceptions;
using NumericalMath.LinearAlgebra.Structures;

namespace NumericalMath.Tests.LinearAlgebra.Structures;

[TestFixture]
public class ColumnVectorTests
{
    [TestCase(new[]{1,2,3,4}, new[]{1,2,3,5},
        TestName = "Equals_WhenDifferentEntries_ShouldReturnFalse")]
    [TestCase(new[]{1,2,3,4}, new[]{1,2,3},
        TestName = "Equals_WhenDifferentSize_ShouldReturnFalse")]
    public void NotEqualColumns(int[] array1, int[] array2)
    {
        var columnVector1 = new ColumnVector<int>(array1);
        var columnVector2 = new ColumnVector<int>(array2);
        Assert.That(columnVector1, Is.Not.EqualTo(columnVector2));
    }
    
    [Test]
    public void Constructor_WhenConstantScalar_ShouldReturnConstantScalar()
    {
        var columnVector = new ColumnVector<int>(5, 0);
        Assert.That(columnVector, Is.EqualTo(ColumnVector<int>.Zero(5)));

        columnVector = new ColumnVector<int>(5, 3);
        Assert.That(columnVector.ToArray(), Is.EqualTo(new[] { 3, 3, 3, 3, 3 }));
    }
    
    [Test]
    public void Random_WhenTwoVectorsAreCreated_ShouldNotBeEqual()
    {
        var randomColumn1 = ColumnVector<int>.Random(5);
        var randomColumn2 = ColumnVector<int>.Random(5);
        Assert.That(randomColumn1, Is.Not.EqualTo(randomColumn2));
    }
    
    [Test]
    public void ExplicitCast_WhenIntArray_ShouldReturnVectorWithEntries()
    {
        int[] values = [1, 2, 3, 4, 5];
        var column = (ColumnVector<int>)values;
        Assert.That(column.ToArray(), Is.EqualTo(values));
    }
    
    [Test]
    public void Copy_WhenCreated_ShouldCreateDistinctDeepCopy()
    {
        var columnVector = new ColumnVector<int>([1, 2, 3, 4]);
        var columnVector2 = columnVector.Copy();
        
        Assert.That(columnVector, Is.EqualTo(columnVector2));
        columnVector[0] = 5;
        Assert.That(columnVector, Is.Not.EqualTo(columnVector2));
    }
    
    [Test]
    public void Constructor_WhenNoValuesSet_ShouldOnlyContainZero()
    {
        var columnVector = new ColumnVector<int>(4);
        Assert.That(columnVector.Length, Is.EqualTo(4));
        Assert.That(columnVector.RowCount, Is.EqualTo(4));
        Assert.That(columnVector.ColumnCount, Is.EqualTo(1));

        Assert.That(columnVector[0], Is.EqualTo(0));
        Assert.That(columnVector[1], Is.EqualTo(0));
        Assert.That(columnVector[2], Is.EqualTo(0));
        Assert.That(columnVector[3], Is.EqualTo(0));

        Assert.That(columnVector.Equals(null), Is.False);
    }
    
    [Test]
    public void Indexing_WhenVectorIndex_ShouldReturnVectorEntries()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3, 4]);
        Assert.That(columnVector.Length, Is.EqualTo(4));
        Assert.That(columnVector.RowCount, Is.EqualTo(4));
        Assert.That(columnVector.ColumnCount, Is.EqualTo(1));

        Assert.Throws<IndexOutOfRangeException>(() => { _ = columnVector[-1]; });
        Assert.That(columnVector[0], Is.EqualTo(1));
        Assert.That(columnVector[1], Is.EqualTo(2));
        Assert.That(columnVector[2], Is.EqualTo(3));
        Assert.That(columnVector[3], Is.EqualTo(4));
        Assert.Throws<IndexOutOfRangeException>(() => { _ = columnVector[4]; });

        columnVector[0] = 5;
        columnVector[1] = 6;
        columnVector[2] = 7;
        columnVector[3] = 8;
        
        Assert.That(columnVector[0], Is.EqualTo(5));
        Assert.That(columnVector[1], Is.EqualTo(6));
        Assert.That(columnVector[2], Is.EqualTo(7));
        Assert.That(columnVector[3], Is.EqualTo(8));

    }
    [Test]
    public void Indexing_WhenMatrixIndexing_ShouldReturnVectorEntries()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3, 4]);

        Assert.Throws<IndexOutOfRangeException>(() => { columnVector[0, 1] = 1; });
        Assert.Throws<IndexOutOfRangeException>(() => { _ = columnVector[0, 1]; });
        Assert.Throws<IndexOutOfRangeException>(() => { columnVector[4, 0] = 1; });
        Assert.Throws<IndexOutOfRangeException>(() => { _ = columnVector[4, 0]; });

        Assert.That(columnVector[0, 0], Is.EqualTo(1));
        Assert.That(columnVector[1, 0], Is.EqualTo(2));
        Assert.That(columnVector[2, 0], Is.EqualTo(3));
        Assert.That(columnVector[3, 0], Is.EqualTo(4));

        columnVector[0, 0] = 5;
        columnVector[1, 0] = 6;
        columnVector[2, 0] = 7;
        columnVector[3, 0] = 8;

        Assert.That(columnVector[0], Is.EqualTo(5));
        Assert.That(columnVector[1], Is.EqualTo(6));
        Assert.That(columnVector[2], Is.EqualTo(7));
        Assert.That(columnVector[3], Is.EqualTo(8));
    }

    [Test]
    public void Transpose_WhenTransposed_ShouldReturnRowVector()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3]);
        RowVector<int> rowVector = new RowVector<int>([1, 2, 3]);

        Assert.That(columnVector.Transpose(), Is.EqualTo(rowVector));
        Assert.That(columnVector.Transpose().Transpose(), Is.EqualTo(columnVector));
    }
    
    [Test]
    public void Addition_WhenDimensionsMatch_ShouldReturnVectorSum()
    {
        var columnVector1 = new ColumnVector<int>([1, 2, 3]);
        var columnVector2 = new ColumnVector<int>([4, 5, 6]);
        var result = new ColumnVector<int>([5, 7, 9]);
        Assert.That(columnVector1 + columnVector2, Is.EqualTo(result));
        
        columnVector1 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];
        result = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40];
        Assert.That(columnVector1 + columnVector1, Is.EqualTo(result));
    }

    [Test]
    public void Subtraction_WhenDimensionsMatch_ShouldReturnVectorSubtraction()
    {
        var columnVector1 = new ColumnVector<int>([1, 2, 3]);
        var columnVector2 = new ColumnVector<int>([4, 5, 6]);
        var result = new ColumnVector<int>([-3, -3, -3]);
        Assert.That(columnVector1 - columnVector2, Is.EqualTo(result));
        
        columnVector1 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];
        result = ColumnVector<int>.Zero(20);
        Assert.That(columnVector1 - columnVector1, Is.EqualTo(result));
        
    }
    
    [Test]
    public void Addition_WhenDimensionsMisMatch_ShouldThrow()
    {
        Assert.Throws<DimensionMismatchException>(() => { _ = new ColumnVector<int>([1, 2, 3]) + new ColumnVector<int>([1, 2]); });
    }
        
    [Test]
    public void Subtraction_WhenDimensionsMisMatch_ShouldThrow()
    {
        Assert.Throws<DimensionMismatchException>(() => { _ = new ColumnVector<int>([1, 2, 3]) - new ColumnVector<int>([1, 2]); });
    }
    
    [Test]
    public void Multiply_WhenVectorScalar_ShouldReturnScaledVector()
    {
        var columnVector = new ColumnVector<int>([1, 2, 3]);

        Assert.That(columnVector * 2, Is.EqualTo(new ColumnVector<int>([2, 4, 6])));
        Assert.That(2 * columnVector, Is.EqualTo(new ColumnVector<int>([2, 4, 6])));
        Assert.That(2 * columnVector * 3, Is.EqualTo(new ColumnVector<int>([6, 12, 18])));
        
        columnVector = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];
        ColumnVector<int> result = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40];
        
        Assert.That(columnVector * 2, Is.EqualTo(result));
        Assert.That(2 * columnVector, Is.EqualTo(result));
    }
    
    [Test]
    public void Operations_WhenStridesAreDifferent_ShouldBehaveIdenticalToSameStride()
    {
        var columnVector1 = new ColumnVector<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3);
        var columnVector2 = new ColumnVector<int>(new[] { 1, 2, 3, 4, 5, 6 }, 2);
        
        Assert.That(columnVector1, Has.Length.EqualTo(3));
        Assert.That(columnVector2, Has.Length.EqualTo(3));

        Assert.That((columnVector1 + columnVector2).ToArray(), Is.EqualTo(new[] { 2, 7, 12 }));
        Assert.That((columnVector1 - columnVector2).ToArray(), Is.EqualTo(new[] { 0, 1, 2 }));
        Assert.That((5 * columnVector1).ToArray(), Is.EqualTo(new[] { 5, 20, 35 }));
        Assert.That((4 * columnVector2).ToArray(), Is.EqualTo(new[] { 4, 12, 20 }));
    }
}