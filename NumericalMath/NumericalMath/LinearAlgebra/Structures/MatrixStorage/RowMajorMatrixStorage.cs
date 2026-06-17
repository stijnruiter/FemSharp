using NumericalMath.LinearAlgebra;
using NumericalMath.LinearAlgebra.Structures;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NumericalMath.LinearAlgebra.Structures.MatrixStorage;

public class RowMajorMatrixStorage<T> : IMatrixStorage<T> where T : struct, INumber<T>
{
    private Memory<T> _values;

    public int RowCount { get; }

    public int ColumnCount { get; }

    public Span<T> Span => _values.Span;

    public RowMajorMatrixStorage(int rowCount, int columnCount)
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

    public RowMajorMatrixStorage(int rowCount, int columnCount, Memory<T> values)
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

    public RowMajorMatrixStorage(int rowCount, int columnCount, T scalar) : this(rowCount, columnCount)
    {
        _values.Span.Fill(scalar);
    }

    public RowMajorMatrixStorage(T[,] values)
    {
        RowCount = values.GetLength(0);
        ColumnCount = values.GetLength(1);
        // An array T[,] is stored row-major
        T[] destination = new T[RowCount * ColumnCount];
        Buffer.BlockCopy(values, 0, destination, 0, values.Length * Unsafe.SizeOf<T>());
        _values = destination;
    }

    public RowMajorMatrixStorage(ReadOnlySpan<RowVector<T>> rows)
    {
        if (rows.Length == 0 || rows[0].Length == 0)
        {
            RowCount = 0;
            ColumnCount = 0;
            _values = Array.Empty<T>();
            return;
        }

        RowCount = rows.Length;
        ColumnCount = rows[0].Length;
        T[] destination = new T[RowCount * ColumnCount];
        for (int i = 0; i < RowCount; i++)
        {
            ThrowHelper.ThrowIfDifferentLength(rows[0], rows[i]);
            for (int j = 0; j < ColumnCount; j++)
            {
                destination[i * ColumnCount + j] = rows[i][j];
            }
        }
        _values = destination;
    }

    public ColumnVector<T> GetColumn(int j)
    {
        ThrowHelper.ThrowIfColumnOutOfRange(j, this);

        return new ColumnVector<T>(_values.Slice(j), stride: ColumnCount);
    }

    public ColumnVector<T> GetColumnSlice(int j, int start)
    {
        ThrowHelper.ThrowIfColumnOutOfRange(j, this);

        if (start == RowCount)
            return new ColumnVector<T>(0);

        return new ColumnVector<T>(_values.Slice(start * ColumnCount + j), stride: ColumnCount);
    }

    public ColumnVector<T> GetColumnSlice(int j, int start, int length)
    {
        ThrowHelper.ThrowIfColumnOutOfRange(j, this);

        if (start == RowCount || length == 0)
            return new ColumnVector<T>(0);

        return new ColumnVector<T>(_values.Slice(start * ColumnCount + j, (length-1) * ColumnCount + 1), stride: ColumnCount);
    }

    public T GetElement(int i, int j)
    {
        ThrowHelper.ThrowIfOutOfRange(i, j, this);

        return _values.Span[i * ColumnCount + j];
    }

    public RowVector<T> GetRow(int i)
    {
        ThrowHelper.ThrowIfRowOutOfRange(i, this);

        return new RowVector<T>(_values.Slice(i * ColumnCount, ColumnCount), stride: 1);
    }

    public RowVector<T> GetRowSlice(int i, int start)
    {
        ThrowHelper.ThrowIfRowOutOfRange(i, this);

        return new RowVector<T>(_values.Slice(i * ColumnCount, ColumnCount).Slice(start), stride: 1);
    }

    public RowVector<T> GetRowSlice(int i, int start, int length)
    {
        ThrowHelper.ThrowIfRowOutOfRange(i, this);

        if (start == ColumnCount || length == 0)
            return new RowVector<T>(0);
        
        return new RowVector<T>(_values.Slice(i * ColumnCount, ColumnCount).Slice(start, length), stride: 1);
    }

    public void SetElement(int i, int j, T value)
    {
        ThrowHelper.ThrowIfOutOfRange(i, j, this);

        _values.Span[i * ColumnCount + j] = value;
    }

    public void SwapRows(int row1, int row2)
    {
        ThrowHelper.ThrowIfRowOutOfRange(row1, this);
        ThrowHelper.ThrowIfRowOutOfRange(row2, this);

        Memory<T> rowView1 = _values.Slice(row1 * ColumnCount, ColumnCount);
        Memory<T> rowView2 = _values.Slice(row2 * ColumnCount, ColumnCount);
        Memory<T> tempCopy = new T[rowView1.Length];
        rowView1.CopyTo(tempCopy);
        rowView2.CopyTo(rowView1);
        tempCopy.CopyTo(rowView2);
    }

    public IMatrixStorage<T> Copy()
    {
        T[] copyValues = new T[_values.Length];
        _values.CopyTo(copyValues);
        return new RowMajorMatrixStorage<T>(RowCount, ColumnCount, copyValues);
    }
}
