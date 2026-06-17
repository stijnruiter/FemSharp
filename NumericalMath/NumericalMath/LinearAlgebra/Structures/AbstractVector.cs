using NumericalMath.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumericalMath.LinearAlgebra.Structures;

public abstract class AbstractVector<T> : IRectanglarMatrix<T>, IEquatable<AbstractVector<T>> where T : struct, INumber<T>
{
    protected Memory<T> values;
    internal Span<T> Span => values.Span;
    public int Length { get; }
    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }

    public int Stride { get; protected set; }

    public abstract T this[int rowIndex, int columnIndex] { get; set; }

    protected AbstractVector(int count)
    {
        values = new T[count];
        Stride = 1;
        Length = count;
    }

    protected AbstractVector(Memory<T> memory, int stride)
    {
        Stride = stride;
        values = memory;
        Length = (memory.Length + stride - 1) / stride;
    }

    public T this[int index]
    {
        get
        {
            ThrowHelper.ThrowIfOutOfRange(index, this);

            return values.Span[index * Stride];
        }

        set
        {
            ThrowHelper.ThrowIfOutOfRange(index, this);

            values.Span[index * Stride] = value;
        }
    }

    public bool Equals(AbstractVector<T>? other)
    {
        if (other is null)
            return false;

        if (RowCount != other.RowCount || ColumnCount != other.ColumnCount)
            return false;

        for (int i = 0; i < RowCount * ColumnCount; i++)
        {
            if (this[i] != other[i])
                return false;
        }
        return true;
    }

    /// <summary>
    /// Get largest absolute column value, e.g. max(abs(matrix[startIndex.. , columnIndex]))
    /// </summary>
    /// <param name="matrix">The searchmatrix</param>
    /// <returns>The max abs value and its index</returns>
    public (T value, int index) AbsMax()
    {
        ThrowHelper.ThrowIfEmpty(this);

        // Start with first element
        ReadOnlySpan<T> values = Span;
        int maxIndex = 0;
        T maxValue = T.Abs(values[0]);
        T nextValue;

        // Loop over the rest of the rows
        for (int i = 1; i < Length; i++)
        {
            if ((nextValue = T.Abs(values[i * Stride])) > maxValue)
            {
                maxValue = nextValue;
                maxIndex = i;
            }
        }
        return (maxValue, maxIndex);
    }

    public override string ToString() => $"Vec{ColumnCount}x{RowCount} [{string.Join(", ", Enumerable.Range(0, Length).Select(i => this[i]))}]";

    public T[] ToArray()
    {
        T[] values = new T[Length];
        for (int i = 0; i < Length; i++)
        {
            values[i] = this[i];
        }
        return values;
    }

    public IEnumerator<T> GetEnumerator() => new VectorEnumerator<T>(this);
}
