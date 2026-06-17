using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NumericalMath.LinearAlgebra.Structures;

[CollectionBuilder(typeof(StructureBuilder), nameof(StructureBuilder.CreateRowVector))]
public class RowVector<T> : AbstractVector<T> where T : struct, INumber<T>
{
    public RowVector(T[] values) : this(new Memory<T>(values), 1)
    {

    }

    public RowVector(int count) : base(count)
    {
    }

    public RowVector(Memory<T> values, int stride = 1) : base(values, stride)
    {
    }

    public RowVector(int count, T scalar) : base(count)
    {
        values.Span.Fill(scalar);
    }

    public override int RowCount => 1;

    public override int ColumnCount => Length;

    public override T this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex != 0)
                throw new IndexOutOfRangeException();

            return this[columnIndex];
        }

        set
        {
            if (rowIndex != 0)
                throw new IndexOutOfRangeException();

            this[columnIndex] = value;
        }
    }

    public ColumnVector<T> Transpose() => new ColumnVector<T>(values);

    public static explicit operator RowVector<T>(T[] vector) => new RowVector<T>(vector);

    public static RowVector<T> operator +(RowVector<T> lhs, RowVector<T> rhs)
    {
        ThrowHelper.ThrowIfDifferentLength(lhs, rhs);

        RowVector<T> result = new RowVector<T>(lhs.Length);
        InternalArithmetics.Addition(lhs, rhs, result);
        return result;
    }

    public static RowVector<T> operator -(RowVector<T> lhs, RowVector<T> rhs)
    {
        ThrowHelper.ThrowIfDifferentLength(lhs, rhs);

        RowVector<T> result = new RowVector<T>(lhs.Length);
        InternalArithmetics.Subtraction(lhs, rhs, result);
        return result;
    }

    public static RowVector<T> operator *(T lhs, RowVector<T> rhs)
    {
        RowVector<T> result = new RowVector<T>(rhs.Length);
        InternalArithmetics.ScalarProduct(lhs, rhs, result);
        return result;
    }

    public static RowVector<T> operator *(RowVector<T> lhs, T rhs)
    {
        RowVector<T> result = new RowVector<T>(lhs.Length);
        InternalArithmetics.ScalarProduct(rhs, lhs, result);
        return result;
    }

    public static T operator *(RowVector<T> lhs, ColumnVector<T> rhs)
        => InternalArithmetics.DotProduct(lhs, rhs);

    public static RowVector<T> Zero(int size) => new(size);
    
    public RowVector<T> Copy()
    {
        Memory<T> copy = new T[values.Length];
        values.CopyTo(copy);
        return new RowVector<T>(copy);
    }

    public static RowVector<T> Random(int size)
    {
        var random = new Random();
        var vector = new RowVector<T>(random.RandomNumbers<T>(size));
        return vector;
    }
}