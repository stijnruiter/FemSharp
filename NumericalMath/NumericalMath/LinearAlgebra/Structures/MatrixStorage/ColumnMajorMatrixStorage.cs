using NumericalMath.LinearAlgebra;
using NumericalMath.LinearAlgebra.Structures;
using System;
using System.Numerics;

namespace NumericalMath.LinearAlgebra.Structures.MatrixStorage;

internal class ColumnMajorMatrixStorage<T> : IMatrixStorage<T> where T : struct, INumber<T>
{
    private Memory<T> _values;

    public Span<T> Span => _values.Span;

    public int RowCount { get; }

    public int ColumnCount { get; }

    public ColumnMajorMatrixStorage(int rowCount, int columnCount)
    {
        if (rowCount < 0)
        {
            throw new ArgumentException("RowCount cannot be negative.");
        }
        if (columnCount < 0)
        {
            throw new ArgumentException("ColumnCount cannot be negative.");
        }
        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = new T[rowCount * columnCount];
    }

    public ColumnMajorMatrixStorage(int rowCount, int columnCount, Memory<T> values)
    {
        if (rowCount < 0)
        {
            throw new ArgumentException("RowCount cannot be negative.");
        }
        if (columnCount < 0)
        {
            throw new ArgumentException("ColumnCount cannot be negative.");
        }
        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = values;
    }

    public ColumnMajorMatrixStorage(int rowCount, int columnCount, T scalar) : this(rowCount, columnCount)
    {
        _values.Span.Fill(scalar);
    }

    public ColumnMajorMatrixStorage(T[,] values) : this(values.GetLength(0), values.GetLength(1))
    {
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                SetElement(i, j, values[i, j]);
            }
        }
    }

    public IMatrixStorage<T> Copy()
    {
        Memory<T> values = new T[_values.Length];
        _values.CopyTo(values);
        return new ColumnMajorMatrixStorage<T>(RowCount, ColumnCount, values);
    }

    public ColumnVector<T> GetColumn(int j)
    {
        ThrowHelper.ThrowIfColumnOutOfRange(j, this);

        return new ColumnVector<T>(_values.Slice(j * RowCount, RowCount), stride: 1);
    }

    public ColumnVector<T> GetColumnSlice(int j, int start)
    {
        ThrowHelper.ThrowIfColumnOutOfRange(j, this);

        return new ColumnVector<T>(_values.Slice(j * RowCount, RowCount).Slice(start), stride: 1);
    }

    public ColumnVector<T> GetColumnSlice(int j, int start, int length)
    {
        ThrowHelper.ThrowIfOutOfRange(0, j, this);
        
        if (start == RowCount)
            return new ColumnVector<T>(0);

        return new ColumnVector<T>(_values.Slice(j * RowCount, RowCount).Slice(start, length), stride: 1);
    }

    public T GetElement(int i, int j)
    {
        ThrowHelper.ThrowIfOutOfRange(i, j, this);

        return _values.Span[i + j * RowCount];
    }

    public RowVector<T> GetRow(int i)
    {
        ThrowHelper.ThrowIfRowOutOfRange(i, this);

        return new RowVector<T>(_values.Slice(i), stride: RowCount);
    }

    public RowVector<T> GetRowSlice(int i, int start)
    {
        ThrowHelper.ThrowIfRowOutOfRange(i, this);

        if (start == ColumnCount)
            return new RowVector<T>(0);

        return new RowVector<T>(_values.Slice(start * RowCount + i), stride: RowCount);
    }

    public RowVector<T> GetRowSlice(int i, int start, int length)
    {
        ThrowHelper.ThrowIfRowOutOfRange(i, this);

        if (start == ColumnCount || length == 0)
            return new RowVector<T>(0);

        return new RowVector<T>(_values.Slice(start * RowCount + i, (length - 1) * ColumnCount + 1), stride: ColumnCount);
    }

    public void SetElement(int i, int j, T value)
    {
        ThrowHelper.ThrowIfOutOfRange(i, j, this);

        _values.Span[i + j * RowCount] = value;
    }

    public void SwapRows(int row1, int row2)
    {
        ThrowHelper.ThrowIfRowOutOfRange(row1, this);
        ThrowHelper.ThrowIfRowOutOfRange(row2, this);

        Span<T> view = _values.Span;
        for (int j = 0; j < ColumnCount; j++)
        {
            (view[row1 + j * RowCount], view[row2 + j * RowCount]) = (view[row2 + j * RowCount], view[row1 + j * RowCount]);
        }
    }
}
