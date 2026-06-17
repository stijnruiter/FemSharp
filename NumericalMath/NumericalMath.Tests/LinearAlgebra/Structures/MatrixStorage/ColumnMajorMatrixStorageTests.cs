using System;
using NumericalMath.LinearAlgebra.Structures;
using NumericalMath.LinearAlgebra.Structures.MatrixStorage;

namespace NumericalMath.Tests.LinearAlgebra.Structures.MatrixStorage;

[TestFixture]
public class ColumnMajorMatrixStorageTests
{
    
    [Test]
    public void Constructor_WhenParametersInvalid_ThrowsArgumentException()
    {
        Assert.That(() => new ColumnMajorMatrixStorage<int>(-1, 5), Throws.ArgumentException);
        Assert.That(() => new ColumnMajorMatrixStorage<int>(5, -1), Throws.ArgumentException);
        Assert.That(() => new ColumnMajorMatrixStorage<int>(-5, 1, null), Throws.ArgumentException);
        Assert.That(() => new ColumnMajorMatrixStorage<int>(5, -1, null), Throws.ArgumentException);
    }

    [Test]
    public void Constructor_WhenEmptyArray_ReturnsEmptyColumnStorage()
    {
        var storage = new ColumnMajorMatrixStorage<int>(new int[,]{});
        Assert.That(storage.ColumnCount, Is.EqualTo(0));
        Assert.That(storage.RowCount, Is.EqualTo(0));
    }
    
    [Test]
    public void Constructor_WhenNonEmptyValues_ReturnsFilledColumnStorage()
    {
        var values = new int[12];
        values.AsSpan().Fill(3);
        var fill = new ColumnMajorMatrixStorage<int>(3, 4, values);
        var constant = new ColumnMajorMatrixStorage<int>(3, 4, 3);
        
        Assert.That(fill.Span.ToArray(), Is.EqualTo(values));
        Assert.That(constant.Span.ToArray(), Is.EqualTo(values));
    }
    
    [Test]
    public void Copy_WhenStorageIsCreated_ShouldHaveDistinctStorages()
    {
        var column = new ColumnMajorMatrixStorage<int>(3, 4, TestContext.CurrentContext.Random.RandomNumbers<int>(12));
        var columCopy = column.Copy();
        Assert.That(column.Span.ToArray(), Is.EqualTo(columCopy.Span.ToArray()));
        column.SetElement(1,2, column.GetElement(1,2)+ 1);
        Assert.That(column.Span.ToArray(), Is.Not.EqualTo(columCopy.Span.ToArray()));
    }
}