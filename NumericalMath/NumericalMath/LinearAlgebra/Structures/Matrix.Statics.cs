using System;
using System.Numerics;

namespace NumericalMath.LinearAlgebra.Structures;

public partial class Matrix<T> : IRectanglarMatrix<T>, IEquatable<Matrix<T>> where T : struct, INumber<T>
{

    public static T Determinant(T m11, T m12,
                                T m21, T m22)
    {
        return m11 * m22 - m21 * m12;
    }

    public static T Determinant(T m11, T m12, T m13,
                                T m21, T m22, T m23,
                                T m31, T m32, T m33)
    {
        return m11 * Determinant(m22, m23, m32, m33)
             - m12 * Determinant(m21, m23, m31, m33)
             + m13 * Determinant(m21, m22, m31, m32);
    }
}
