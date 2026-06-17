using System;
using NumericalMath.LinearAlgebra.Structures;
using NumericalMath.LinearAlgebra.Structures.MatrixStorage;

namespace NumericalMath.Tests.LinearAlgebra.Structures.MatrixStorage;

[TestFixture]
public class RowMatrixStorageTests
{

    [Test]
    public void Constructor_WhenParametersInvalid_ThrowsArgumentException()
    {
        Assert.That(() => new RowMajorMatrixStorage<int>(-1, 5), Throws.ArgumentException);
        Assert.That(() => new RowMajorMatrixStorage<int>(5, -1), Throws.ArgumentException);
        Assert.That(() => new RowMajorMatrixStorage<int>(-5, 1, null), Throws.ArgumentException);
        Assert.That(() => new RowMajorMatrixStorage<int>(5, -1, null), Throws.ArgumentException);
    }

    [Test]
    public void Constructor_WhenEmptyArray_ReturnsEmptyRowStorage()
    {
        var storage = new RowMajorMatrixStorage<int>(Array.Empty<RowVector<int>>());
        Assert.That(storage.ColumnCount, Is.EqualTo(0));
        Assert.That(storage.RowCount, Is.EqualTo(0));
    }
    
    
    [Test]
    public void Constructor_WhenNonEmptyValues_ReturnsFilledRowStorage()
    {
        var values = new int[12];
        values.AsSpan().Fill(3);
        var fill = new RowMajorMatrixStorage<int>(3, 4, values);
        var constant = new RowMajorMatrixStorage<int>(3, 4, 3);
        
        Assert.That(fill.Span.ToArray(), Is.EqualTo(values));
        Assert.That(constant.Span.ToArray(), Is.EqualTo(values));
    }

    [Test]
    public void Copy_WhenStorageIsCreated_ShouldHaveDistinctStorages()
    {
        var rowStorage = new RowMajorMatrixStorage<int>(3, 4, TestContext.CurrentContext.Random.RandomNumbers<int>(12));
        var rowCopy = rowStorage.Copy();
        Assert.That(rowStorage.Span.ToArray(), Is.EqualTo(rowCopy.Span.ToArray()));
        rowStorage.SetElement(1,2, rowStorage.GetElement(1,2)+ 1);
        Assert.That(rowStorage.Span.ToArray(), Is.Not.EqualTo(rowCopy.Span.ToArray()));
    }
}