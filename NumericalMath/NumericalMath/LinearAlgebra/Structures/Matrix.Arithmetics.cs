using NumericalMath.Exceptions;
using System;
using System.Numerics;

namespace NumericalMath.LinearAlgebra.Structures;

public partial class Matrix<T> : IRectanglarMatrix<T>, IEquatable<Matrix<T>> where T : struct, INumber<T>
{
    public static Matrix<T> operator +(Matrix<T> lhs, Matrix<T> rhs)
    {
        ThrowHelper.ThrowIfDifferentSize(lhs, rhs);

        Matrix<T> result = new Matrix<T>(lhs.RowCount, lhs.ColumnCount);
        InternalArithmetics.Addition(lhs, rhs, result);
        return result;
    }

    public static Matrix<T> operator -(Matrix<T> lhs, Matrix<T> rhs)
    {
        ThrowHelper.ThrowIfDifferentSize(lhs, rhs);

        Matrix<T> result = new Matrix<T>(lhs.RowCount, lhs.ColumnCount);
        InternalArithmetics.Subtraction(lhs, rhs, result);
        return result;
    }

    public static Matrix<T> operator *(Matrix<T> lhs, Matrix<T> rhs)
    {
        if (lhs.ColumnCount != rhs.RowCount)
            throw new DimensionMismatchException("Unable to compute product, lhs matrix columns do not match rhs matrix rows.", lhs.ColumnCount, rhs.RowCount);

        Matrix<T> result = new Matrix<T>(lhs.RowCount, rhs.ColumnCount);
        InternalArithmetics.Product(lhs, rhs, result);
        return result;
    }

    public static ColumnVector<T> operator *(Matrix<T> lhs, ColumnVector<T> rhs)
    {
        if (lhs.ColumnCount != rhs.RowCount)
            throw new DimensionMismatchException("RowVector and Matrix dimensions do not match", lhs.ColumnCount, rhs.RowCount);

        ColumnVector<T> result = new ColumnVector<T>(lhs.RowCount);
        InternalArithmetics.Product(lhs, rhs, result);
        return result;
    }

    public static RowVector<T> operator *(RowVector<T> lhs, Matrix<T> rhs)
    {
        if (lhs.ColumnCount != rhs.RowCount)
            throw new DimensionMismatchException("RowVector and Matrix dimensions do not match", lhs.ColumnCount, rhs.RowCount);

        RowVector<T> result = new RowVector<T>(rhs.ColumnCount);
        InternalArithmetics.Product(lhs, rhs, result);
        return result;
    }

    public static Matrix<T> operator *(T lhs, Matrix<T> rhs)
    {
        Matrix<T> result = new Matrix<T>(rhs.RowCount, rhs.ColumnCount);
        InternalArithmetics.ScalarProduct(lhs, rhs, result);
        return result;
    }

    public static Matrix<T> operator *(Matrix<T> lhs, T rhs)
    {
        Matrix<T> result = new Matrix<T>(lhs.RowCount, lhs.ColumnCount);
        InternalArithmetics.ScalarProduct(rhs, lhs, result);
        return result;
    }

}
