using System;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.LinearAlgebra.Structures;
using NumericalMath.LinearAlgebra.Structures.MatrixStorage;
using NumericalMath.Tests.ExtensionHelpers;

namespace NumericalMath.Tests.LinearAlgebra.Structures.MatrixStorage;

[TestFixture(typeof(ColumnMajorMatrixStorage<int>))]
[TestFixture(typeof(RowMajorMatrixStorage<int>))]
public class MatrixStorageTests<T> where T : IMatrixStorage<int>
{
    [Test]
    public void MatrixEnumerator_WhenStrongTypedIteration_ShouldReturnRowEnumerable()
    {
        using var enumerator = new MatrixEnumerator<int>(MatrixStorage3X3);
        var enum1 = new List<RowVector<int>>();
        
        while (enumerator.MoveNext())
        {
            enum1.Add(enumerator.Current);
        }

        var enum2 = enumerator.ToWeakTypedEnumerable().ToArray();
        
        Assert.That(enum1, Is.EqualTo(_expectedRows3X3));
        Assert.That(enum2, Is.EqualTo(_expectedRows3X3));
    }
    
    [Test]
    public void SwapRows_WhenStorageIsValid_ShouldSwapRowsInPlace()
    {
        Assume.That(MatrixStorage3X3.GetRow(0).ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
        Assume.That(MatrixStorage3X3.GetRow(1).ToArray(), Is.EqualTo(new[] { 4, 5, 6 }));
        Assume.That(MatrixStorage3X3.GetRow(2).ToArray(), Is.EqualTo(new[] { 7, 8, 9 }));
        
        MatrixStorage3X3.SwapRows(0, 1);
        
        Assert.That(MatrixStorage3X3.GetRow(0).ToArray(), Is.EqualTo(new[] { 4, 5, 6 }));
        Assert.That(MatrixStorage3X3.GetRow(1).ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(MatrixStorage3X3.GetRow(2).ToArray(), Is.EqualTo(new[] { 7, 8, 9 }));
    }

    [Test]
    public void StorageColumnSlicesStartIndex()
    {
        Assert.That(MatrixStorage3X3.GetColumnSlice(0, 0).ToArray(), Is.EqualTo(new[] { 1, 4, 7 }));
        Assert.That(MatrixStorage3X3.GetColumnSlice(1, 0).ToArray(), Is.EqualTo(new[] { 2, 5, 8 }));
        Assert.That(MatrixStorage3X3.GetColumnSlice(2, 0).ToArray(), Is.EqualTo(new[] { 3, 6, 9 }));

        Assert.That(MatrixStorage3X3.GetColumnSlice(0, 1).ToArray(), Is.EqualTo(new[] { 4, 7 }));
        Assert.That(MatrixStorage3X3.GetColumnSlice(1, 1).ToArray(), Is.EqualTo(new[] { 5, 8 }));
        Assert.That(MatrixStorage3X3.GetColumnSlice(2, 1).ToArray(), Is.EqualTo(new[] { 6, 9 }));

        Assert.That(MatrixStorage3X3.GetColumnSlice(0, 2).ToArray(), Is.EqualTo(new[] { 7 }));
        Assert.That(MatrixStorage3X3.GetColumnSlice(1, 2).ToArray(), Is.EqualTo(new[] { 8 }));
        Assert.That(MatrixStorage3X3.GetColumnSlice(2, 2).ToArray(), Is.EqualTo(new[] { 9 }));

        Assert.That(MatrixStorage3X3.GetColumnSlice(0, 3).ToArray(), Is.EqualTo(new int[] { }));
        Assert.That(MatrixStorage3X3.GetColumnSlice(1, 3).ToArray(), Is.EqualTo(new int[] { }));
        Assert.That(MatrixStorage3X3.GetColumnSlice(2, 3).ToArray(), Is.EqualTo(new int[] { }));
    }
    
    [Test]
    public void StorageColumnSlicesStartWithLength()
    {
        Assert.That(MatrixStorage3X3.GetColumnSlice(0, 0, 3).ToArray(), Is.EqualTo(new[] {1, 4, 7}));
        Assert.That(MatrixStorage3X3.GetColumnSlice(1, 0, 2).ToArray(), Is.EqualTo(new[] {2, 5}));
        Assert.That(MatrixStorage3X3.GetColumnSlice(2, 0, 1).ToArray(), Is.EqualTo(new[] {3}));
        
        Assert.That(() => MatrixStorage3X3.GetColumnSlice(0, 1, 3), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(MatrixStorage3X3.GetColumnSlice(1, 1, 2).ToArray(), Is.EqualTo(new[] {5, 8}));
        Assert.That(MatrixStorage3X3.GetColumnSlice(2, 1, 1).ToArray(), Is.EqualTo(new[] {6}));
        
        Assert.That(() => MatrixStorage3X3.GetColumnSlice(0, 2, 2), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(MatrixStorage3X3.GetColumnSlice(1, 2, 1).ToArray(), Is.EqualTo(new[] {8}));
        Assert.That(MatrixStorage3X3.GetColumnSlice(2, 2, 0).ToArray(), Is.EqualTo(new int[] {}));
        
        Assert.That(MatrixStorage3X3.GetColumnSlice(0, 3, 1).ToArray(),  Is.EqualTo(new int[] {}));
        Assert.That(MatrixStorage3X3.GetColumnSlice(1, 3, 0).ToArray(), Is.EqualTo(new int[] {}));
    }

    [Test]
    public void StorageRowSlicesStartIndex()
    {
        Assert.That(MatrixStorage3X3.GetRowSlice(0, 0).ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(MatrixStorage3X3.GetRowSlice(1, 0).ToArray(), Is.EqualTo(new[] { 4, 5, 6 }));
        Assert.That(MatrixStorage3X3.GetRowSlice(2, 0).ToArray(), Is.EqualTo(new[] { 7, 8, 9 }));

        Assert.That(MatrixStorage3X3.GetRowSlice(0, 1).ToArray(), Is.EqualTo(new[] { 2, 3 }));
        Assert.That(MatrixStorage3X3.GetRowSlice(1, 1).ToArray(), Is.EqualTo(new[] { 5, 6 }));
        Assert.That(MatrixStorage3X3.GetRowSlice(2, 1).ToArray(), Is.EqualTo(new[] { 8, 9 }));

        Assert.That(MatrixStorage3X3.GetRowSlice(0, 2).ToArray(), Is.EqualTo(new[] { 3 }));
        Assert.That(MatrixStorage3X3.GetRowSlice(1, 2).ToArray(), Is.EqualTo(new[] { 6 }));
        Assert.That(MatrixStorage3X3.GetRowSlice(2, 2).ToArray(), Is.EqualTo(new[] { 9 }));

        Assert.That(MatrixStorage3X3.GetRowSlice(0, 3).ToArray(), Is.EqualTo(new int[] { }));
        Assert.That(MatrixStorage3X3.GetRowSlice(1, 3).ToArray(), Is.EqualTo(new int[] { }));
        Assert.That(MatrixStorage3X3.GetRowSlice(2, 3).ToArray(), Is.EqualTo(new int[] { }));
    }
    
    [Test]
    public void StorageRowSlicesStartWithLength()
    {
        Assert.That(MatrixStorage3X3.GetRowSlice(0, 0, 3).ToArray(), Is.EqualTo(new[] {1, 2, 3}));
        Assert.That(MatrixStorage3X3.GetRowSlice(1, 0, 2).ToArray(), Is.EqualTo(new[] {4, 5}));
        Assert.That(MatrixStorage3X3.GetRowSlice(2, 0, 1).ToArray(), Is.EqualTo(new[] {7}));
        
        Assert.That(() => MatrixStorage3X3.GetRowSlice(0, 1, 3), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(MatrixStorage3X3.GetRowSlice(1, 1, 2).ToArray(), Is.EqualTo(new[] {5, 6}));
        Assert.That(MatrixStorage3X3.GetRowSlice(2, 1, 1).ToArray(), Is.EqualTo(new[] {8}));
        
        Assert.That(() => MatrixStorage3X3.GetRowSlice(0, 2, 2), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(MatrixStorage3X3.GetRowSlice(1, 2, 1).ToArray(), Is.EqualTo(new[] {6}));
        Assert.That(MatrixStorage3X3.GetRowSlice(2, 2, 0).ToArray(), Is.EqualTo(new int[] {}));
        
        Assert.That(MatrixStorage3X3.GetRowSlice(0, 3, 1).ToArray(),  Is.EqualTo(new int[] {}));
        Assert.That(MatrixStorage3X3.GetRowSlice(1, 3, 0).ToArray(), Is.EqualTo(new int[] {}));
    }
    
    [OneTimeSetUp]
    public void CreateStorage()
    {
        if (typeof(T) == typeof(ColumnMajorMatrixStorage<int>))
        {
            MatrixStorage3X3 = new ColumnMajorMatrixStorage<int>(_matrixElements3X3);
            return;
        } 
        else if (typeof(T) == typeof(RowMajorMatrixStorage<int>))
        {
            MatrixStorage3X3 = new RowMajorMatrixStorage<int>(_matrixElements3X3);
            return;
        }
        throw new InvalidCastException($"Cannot convert {typeof(T).Name} to MatrixStorage");
    }

    private readonly int[,] _matrixElements3X3 = new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
    private readonly RowVector<int>[] _expectedRows3X3 = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
    private IMatrixStorage<int> MatrixStorage3X3 { get; set; } = null!;
    
}