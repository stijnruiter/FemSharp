using System;
using System.Numerics;
using System.Runtime.InteropServices;
using NumericalMath.LinearAlgebra.Structures;

namespace NumericalMath.LinearAlgebra;

internal static class InternalArithmetics
{
    private static bool OperationCanBeVectorized<T>(AbstractVector<T> lhs, AbstractVector<T> rhs) where T : struct, INumber<T>
    {
        return lhs.Stride == 1 && rhs.Stride == 1;
    }

    private static bool OperationCanBeVectorized<T>(AbstractVector<T> lhs, AbstractVector<T> rhs, AbstractVector<T> result) where T : struct, INumber<T>
    {
        return lhs.Stride == 1 && rhs.Stride == 1 && result.Stride == 1;
    }

    private static bool OperationCanBeVectorized<T>(Matrix<T> lhs, Matrix<T> rhs) where T : struct, INumber<T>
    {
        return lhs.Storage.GetType() == rhs.Storage.GetType();
    }

    private static bool OperationCanBeVectorized<T>(Matrix<T> lhs, Matrix<T> rhs, Matrix<T> result) where T : struct, INumber<T>
    {
        return lhs.Storage.GetType() == rhs.Storage.GetType() && lhs.Storage.GetType() == result.Storage.GetType();
    }

    internal static T DotProduct<T>(AbstractVector<T> lhs, AbstractVector<T> rhs) where T : struct, INumber<T>
    {
        ThrowHelper.ThrowIfDifferentLength(lhs, rhs);

        T result = T.Zero;
        if (OperationCanBeVectorized(lhs, rhs))
        {
            return VectorizedDotProduct<T>(lhs.Span, rhs.Span);
        }
        else
        {
            for (int i = 0; i < lhs.Length; i++)
            {
                result += lhs[i] * rhs[i];
            }
            return result;
        }
    }

    internal static void ScalarProduct<T>(T scalar, AbstractVector<T> vector, AbstractVector<T> result) where T : struct, INumber<T>
    {
        ThrowHelper.ThrowIfDifferentLength(vector, result);

        if (OperationCanBeVectorized(vector, result))
        {
            VectorizedScalarProduct(scalar, vector.Span, result.Span);
        }
        else
        {
            for (int i = 0; i < vector.Length; i++)
            {
                result[i] = vector[i] * scalar;
            }
        }
    }

    internal static void ScalarProduct<T>(T scalar, Matrix<T> matrix, Matrix<T> result) where T : struct, INumber<T>
    {
        ThrowHelper.ThrowIfDifferentSize(matrix, result);

        if (OperationCanBeVectorized(matrix, result))
        {
            VectorizedScalarProduct(scalar, matrix.Span, result.Span);
        }
        else
        {
            for (int i = 0; i < matrix.RowCount; i++)
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    result[i, j] = matrix[i, j] * scalar;
                }
        }
    }

    internal static void Product<T>(Matrix<T> lhs, Matrix<T> rhs, Matrix<T> result) where T : struct, INumber<T>
    {
        for (int j = 0; j < rhs.ColumnCount; j++)
        {
            ColumnVector<T> column = rhs.Column(j);
            for (int i = 0; i < lhs.RowCount; i++)
            {
                result[i, j] = DotProduct(lhs.Row(i), column);
            }
        }
    }

    internal static void Product<T>(Matrix<T> lhs, ColumnVector<T> rhs, ColumnVector<T> result) where T : struct, INumber<T>
    {
        for (int i = 0; i < lhs.RowCount; i++)
        {
            result[i] = DotProduct(lhs.Row(i), rhs);
        }
    }

    internal static void Product<T>(RowVector<T> lhs, Matrix<T> rhs, RowVector<T> result) where T : struct, INumber<T>
    {
        for (int i = 0; i < rhs.ColumnCount; i++)
        {
            result[i] = DotProduct(lhs, rhs.Column(i));
        }
    }

    internal static void Addition<T>(AbstractVector<T> lhs, AbstractVector<T> rhs, AbstractVector<T> result) where T : struct, INumber<T>
    {
        ThrowHelper.ThrowIfDifferentLength(lhs, rhs);
        ThrowHelper.ThrowIfDifferentLength(lhs, result);

        if (OperationCanBeVectorized(lhs, rhs, result))
        {
            VectorizedAddition(lhs.Span, rhs.Span, result.Span);
        }
        else
        {
            for (int i = 0; i < lhs.Length; i++)
            {
                result[i] = lhs[i] + rhs[i];
            }
        }
    }

    internal static void Addition<T>(Matrix<T> lhs, Matrix<T> rhs, Matrix<T> result) where T : struct, INumber<T>
    {
        ThrowHelper.ThrowIfDifferentSize(lhs, rhs);
        ThrowHelper.ThrowIfDifferentSize(lhs, result);

        if (OperationCanBeVectorized(lhs, rhs, result))
        {
            VectorizedAddition(lhs.Span, rhs.Span, result.Span);
        }
        else
        {
            for (int i = 0; i < lhs.RowCount; i++)
            {
                for (int j = 0; j < lhs.ColumnCount; j++)
                {
                    result[i, j] = lhs[i, j] + rhs[i, j];
                }
            }
        }
    }

    internal static void Subtraction<T>(AbstractVector<T> lhs, AbstractVector<T> rhs, AbstractVector<T> result) where T : struct, INumber<T>
    {
        ThrowHelper.ThrowIfDifferentLength(lhs, rhs);
        ThrowHelper.ThrowIfDifferentLength(lhs, result);

        if (OperationCanBeVectorized(lhs, rhs, result))
        {
            VectorizedSubtraction(lhs.Span, rhs.Span, result.Span);
        }
        else
        {
            for (int i = 0; i < lhs.Length; i++)
            {
                result[i] = lhs[i] - rhs[i];
            }
        }
    }

    internal static void Subtraction<T>(Matrix<T> lhs, Matrix<T> rhs, Matrix<T> result) where T : struct, INumber<T>
    {
        ThrowHelper.ThrowIfDifferentSize(lhs, rhs);
        ThrowHelper.ThrowIfDifferentSize(lhs, result);

        if (OperationCanBeVectorized(lhs, rhs, result))
        {
            VectorizedSubtraction(lhs.Span, rhs.Span, result.Span);
        }
        else
        {
            for (int i = 0; i < lhs.RowCount; i++)
            {
                for (int j = 0; j < lhs.ColumnCount; j++)
                {
                    result[i, j] = lhs[i, j] - rhs[i, j];
                }
            }
        }
    }

    private static void VectorizedAddition<T>(ReadOnlySpan<T> lhs, ReadOnlySpan<T> rhs, Span<T> result) where T : struct, INumberBase<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(lhs);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(rhs);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] + rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < lhs.Length; i++)
        {
            result[i] = lhs[i] + rhs[i];
        }
    }

    private static void VectorizedSubtraction<T>(ReadOnlySpan<T> lhs, ReadOnlySpan<T> rhs, Span<T> result) where T : struct, INumberBase<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(lhs);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(rhs);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] - rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < lhs.Length; i++)
        {
            result[i] = lhs[i] - rhs[i];
        }
    }

    private static T VectorizedDotProduct<T>(ReadOnlySpan<T> lhs, ReadOnlySpan<T> rhs) where T : struct, INumberBase<T>
    {
        T result = T.AdditiveIdentity;

        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(lhs);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(rhs);

        for (int i = 0; i < leftVec.Length; i++)
        {
            result += Vector.Dot(leftVec[i], rightVec[i]);
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < lhs.Length; i++)
        {
            result += lhs[i] * rhs[i];
        }
        return result;
    }

    private static void VectorizedScalarProduct<T>(T scalar, ReadOnlySpan<T> vector, Span<T> result) where T : struct, INumberBase<T>
    {
        Vector<T> scalarVec = new Vector<T>(scalar);
        ReadOnlySpan<Vector<T>> vectors = MemoryMarshal.Cast<T, Vector<T>>(vector);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < vectors.Length; i++)
        {
            resultVec[i] = vectors[i] * scalarVec;
        }
        for (int i = vectors.Length * Vector<T>.Count; i < vector.Length; i++)
        {
            result[i] = vector[i] * scalar;
        }
    }

    internal static Matrix<T> OuterProduct<T>(ColumnVector<T> lhs, RowVector<T> rhs) where T : struct, INumber<T>
    {
        ThrowHelper.ThrowIfDifferentLength(lhs, rhs);

        Matrix<T> result = new Matrix<T>(lhs.RowCount, rhs.ColumnCount);
        for (var i = 0; i < lhs.Length; i++)
        {
            for (var j = 0; j < rhs.Length; j++)
            {
                result[i, j] = lhs[i] * rhs[j];
            }
        }
        return result;
    }

    internal static IRectanglarMatrix<T> TensorProduct<T>(IRectanglarMatrix<T> left, IRectanglarMatrix<T> right) where T : struct, INumber<T>
    {
        int rowCount = left.RowCount * right.RowCount;
        int columnCount = left.ColumnCount * right.ColumnCount;
        IRectanglarMatrix<T> result =
            rowCount == 1 ? new RowVector<T>(columnCount) :
            columnCount == 1 ? new ColumnVector<T>(rowCount) :
            new Matrix<T>(rowCount, columnCount);

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                result[i, j] = left[i / right.RowCount, j / right.ColumnCount] * right[i % right.RowCount, j % right.ColumnCount];
            }
        }
        return result;
    }
}
