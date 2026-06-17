using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NumericalMath.LinearAlgebra.Structures;

[CollectionBuilder(typeof(StructureBuilder), nameof(StructureBuilder.CreateColumnVector))]
public class ColumnVector<T> : AbstractVector<T> where T : struct, INumber<T>
{
    public ColumnVector(T[] values) : this(new Memory<T>(values)) { }

    public ColumnVector(int count) : base(count) { }

    public ColumnVector(Memory<T> values, int stride = 1) : base(values, stride) { }

    public ColumnVector(int count, T scalar) : base(count)
    {
        values.Span.Fill(scalar);
    }

    public override int RowCount => Length;

    public override int ColumnCount => 1;

    public override T this[int rowIndex, int columnIndex]
    {
        get
        {
            if (columnIndex != 0)
                throw new IndexOutOfRangeException();

            return this[rowIndex];
        }

        set
        {
            if (columnIndex != 0)
                throw new IndexOutOfRangeException();

            this[rowIndex] = value;
        }
    }

    public RowVector<T> Transpose() => new RowVector<T>(values);

    public static explicit operator ColumnVector<T>(T[] vector) => new ColumnVector<T>(vector);

    public static ColumnVector<T> operator +(ColumnVector<T> lhs, ColumnVector<T> rhs)
    {
        ThrowHelper.ThrowIfDifferentLength(lhs, rhs);

        ColumnVector<T> result = new ColumnVector<T>(lhs.Length);
        InternalArithmetics.Addition(lhs, rhs, result);
        return result;
    }

    public static ColumnVector<T> operator -(ColumnVector<T> lhs, ColumnVector<T> rhs)
    {
        ThrowHelper.ThrowIfDifferentLength(lhs, rhs);

        ColumnVector<T> result = new ColumnVector<T>(lhs.Length);
        InternalArithmetics.Subtraction(lhs, rhs, result);
        return result;
    }

    public static ColumnVector<T> operator *(T lhs, ColumnVector<T> rhs)
    {
        ColumnVector<T> result = new ColumnVector<T>(rhs.Length);
        InternalArithmetics.ScalarProduct(lhs, rhs, result);
        return result;
    }

    public static ColumnVector<T> operator *(ColumnVector<T> lhs, T rhs)
    {
        ColumnVector<T> result = new ColumnVector<T>(lhs.Length);
        InternalArithmetics.ScalarProduct(rhs, lhs, result);
        return result;
    }

    public static Matrix<T> operator *(ColumnVector<T> lhs, RowVector<T> rhs)
        => InternalArithmetics.OuterProduct(lhs, rhs);

    public static ColumnVector<T> Zero(int size) => new ColumnVector<T>(size);

    public ColumnVector<T> Copy()
    {
        Memory<T> copy = new T[values.Length];
        values.CopyTo(copy);
        return new ColumnVector<T>(copy);
    }

    public static ColumnVector<T> Random(int size)
    {
        Random random = new Random();
        ColumnVector<T> vector = new ColumnVector<T>(random.RandomNumbers<T>(size));
        return vector;
    }

    public ColumnVector<T> Slice(int start)
    {
        return new ColumnVector<T>(values.Slice(start * Stride), stride: Stride);
    }

    public ColumnVector<T> Slice(int start, int length)
    {
        return new ColumnVector<T>(values.Slice(start * Stride, length * Stride), stride: Stride);
    }

}