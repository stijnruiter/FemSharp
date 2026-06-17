using System;
using System.Numerics;

namespace NumericalMath.LinearAlgebra.Structures;

public static class MatrixExtensions
{
    private static Matrix<Tout> Convert<Tin, Tout>(this Matrix<Tin> matrix, Func<Tin, Tout> converter) where Tin : struct, INumber<Tin> where Tout : struct, INumber<Tout>
    {
        Matrix<Tout> result = new Matrix<Tout>(matrix.RowCount, matrix.ColumnCount);
        for (int i = 0; i < matrix.RowCount; i++)
        {
            for (int j = 0; j < matrix.ColumnCount; j++)
            {
                result[i, j] = converter(matrix[i, j]);
            }
        }
        return result;
    }

    public static Matrix<double> ToDoubles(this Matrix<float> matrix) => matrix.Convert(System.Convert.ToDouble);
    public static Matrix<float> ToSingles(this Matrix<double> matrix) => matrix.Convert(System.Convert.ToSingle);

    public static ColumnVector<float> Solve(this Matrix<float> A, ColumnVector<float> b, float degenerateTolerance = Constants.DefaultFloatTolerance)
        => PluFactorizationOperations.SolveUsingPlu(A, b, degenerateTolerance);
    public static ColumnVector<double> Solve(this Matrix<double> A, ColumnVector<double> b, double degenerateTolerance = Constants.DefaultDoubleTolerance)
        => PluFactorizationOperations.SolveUsingPlu(A, b, degenerateTolerance);

    public static float Determinant(this Matrix<float> A, float degenerateTolerance = Constants.DefaultFloatTolerance)
        => PluFactorizationOperations.PluDeterminant(A, degenerateTolerance);
    public static double Determinant(this Matrix<double> A, double degenerateTolerance = Constants.DefaultDoubleTolerance)
        => PluFactorizationOperations.PluDeterminant(A, degenerateTolerance);

    public static Matrix<float> Inverse(this Matrix<float> A, float degenerateTolerance = Constants.DefaultFloatTolerance)
        => PluFactorizationOperations.SolveUsingPlu(A, Matrix<float>.Identity(A.RowCount), degenerateTolerance);

    public static Matrix<double> Inverse(this Matrix<double> A, double degenerateTolerance = Constants.DefaultDoubleTolerance)
        => PluFactorizationOperations.SolveUsingPlu(A, Matrix<double>.Identity(A.RowCount), degenerateTolerance);

    internal static Memory<T> RandomNumbers<T>(this Random random, int size)
    {
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.Int32:
                int[] ints = new int[size];
                for (int i = 0; i < size; i++)
                {
                    ints[i] = random.Next();
                }
                return ints as T[];

            case TypeCode.Single:
                float[] singles = new float[size];
                for (int i = 0; i < size; i++)
                {
                    singles[i] = random.Next();
                }
                return singles as T[];

            case TypeCode.Double:
                double[] doubles = new double[size];
                for (int i = 0; i < size; i++)
                {
                    doubles[i] = random.Next();
                }
                return doubles as T[];
            default:
                throw new NotSupportedException();
        }
    }
}
