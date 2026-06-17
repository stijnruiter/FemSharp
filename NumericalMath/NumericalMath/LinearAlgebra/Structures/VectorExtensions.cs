using System;
using System.Numerics;

namespace NumericalMath.LinearAlgebra.Structures;

public static class VectorExtensions
{

    public static T Norm2<T>(this AbstractVector<T> vector) where T : struct, INumber<T>, IRootFunctions<T>
    {
        T result = T.AdditiveIdentity;

        for (int i = 0; i < vector.Length; i++)
        {
            result += vector[i] * vector[i];
        }
        return T.Sqrt(result);
    }

    public static T Norm1<T>(this AbstractVector<T> vector) where T : struct, INumber<T>
    {
        T result = T.AdditiveIdentity;

        for (int i = 0; i < vector.Length; i++)
        {
            result += T.Abs(vector[i]);
        }
        return result;
    }

    public static T? NormInf<T>(this AbstractVector<T> vector) where T : struct, INumber<T>, IComparisonOperators<T, T, bool>
    {
        if (vector.Length == 0)
            return null;

        T max = T.Abs(vector[0]);
        T newValue;
        for (int i = 1; i < vector.Length; i++)
        {
            if ((newValue = T.Abs(vector[i])) > max)
                max = newValue;
        }
        return max;
    }
}
