using System.Numerics;
using NumericalMath.LinearAlgebra.Structures;

namespace NumericalMath.Tests.ExtensionHelpers;

public static class CompareMethods
{
    private static bool ApproxEquals<T>(this IRectanglarMatrix<T> left, IRectanglarMatrix<T> right, T tolerance) where T : struct, INumber<T>
    {
        if (left.RowCount != right.RowCount)
            return false;
        if (left.ColumnCount != right.ColumnCount)
            return false;

        for (int i = 0; i < left.RowCount; i++)
            for (int j = 0; j < left.ColumnCount; j++)
                if (T.Abs(left[i, j] - right[i, j]) > tolerance)
                    return false;
        return true;
    }

    public static bool ApproxEquals(this IRectanglarMatrix<float> left, IRectanglarMatrix<float> right, float tolerance = Constants.DefaultFloatTolerance) => left.ApproxEquals<float>(right, tolerance);
    public static bool ApproxEquals(this IRectanglarMatrix<double> left, IRectanglarMatrix<double> right, double tolerance = Constants.DefaultDoubleTolerance) => left.ApproxEquals<double>(right, tolerance);
    public static float[] ToFloats(this (float, float, float) tuple) => [tuple.Item1, tuple.Item2, tuple.Item3];
    
    
}
