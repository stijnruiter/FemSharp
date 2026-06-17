using NumericalMath.LinearAlgebra.Structures.MatrixStorage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace NumericalMath.LinearAlgebra.Structures;

public static class StructureBuilder
{
    public static RowVector<T> CreateRowVector<T>(ReadOnlySpan<T> values) where T : struct, INumber<T>
    {
        return new RowVector<T>(values.ToArray());
    }
    public static ColumnVector<T> CreateColumnVector<T>(ReadOnlySpan<T> values) where T : struct, INumber<T>
    {
        return new ColumnVector<T>(values.ToArray());
    }
    public static Matrix<T> CreateMatrix<T>(ReadOnlySpan<RowVector<T>> rows) where T : struct, INumber<T>
    {
        if (rows.Length == 0)
            return Matrix<T>.Empty;

        return new Matrix<T>(new RowMajorMatrixStorage<T>(rows));
    }
}

public class VectorEnumerator<T> : IEnumerator<T> where T : struct, INumber<T>
{
    private int position;
    private readonly AbstractVector<T> vector;
    public VectorEnumerator(AbstractVector<T> vec)
    {
        vector = vec;
        position = -1;
    }

    public T Current => vector[position];

    object IEnumerator.Current => vector[position];

    public void Dispose() { }

    public bool MoveNext()
    {
        position += 1;
        if (position < vector.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Reset()
    {
        position = -1;
    }
}

public class MatrixEnumerator<T>(IMatrixStorage<T> matrix) : IEnumerator<RowVector<T>>
    where T : struct, INumber<T>
{
    private int _position = -1;

    public RowVector<T> Current => matrix.GetRow(_position);

    object IEnumerator.Current => matrix.GetRow(_position);

    public void Dispose() { }

    public bool MoveNext()
    {
        _position += 1;
        if (_position < matrix.RowCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Reset()
    {
        _position = -1;
    }
}