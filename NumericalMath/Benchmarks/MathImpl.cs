using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Benchmarks;

internal static class MathImpl
{
    internal static void ElementwiseAdditionSpan<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        for (int i = 0; i < left.Length; i++)
        {
            result[i] = left[i] + right[i];
        }
    }

    internal static void ElementwiseSubtractionSpan<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        for (int i = 0; i < left.Length; i++)
        {
            result[i] = left[i] - right[i];
        }
    }

    internal static void ElementwiseProductSpan<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        for (int i = 0; i < left.Length; i++)
        {
            result[i] = left[i] * right[i];
        }
    }

    internal static void ElementwiseDivisionSpan<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        for (int i = 0; i < left.Length; i++)
        {
            result[i] = left[i] / right[i];
        }
    }

    internal static unsafe void ElementwiseAdditionUnsafe<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        fixed (T* pa = &left[0])
        fixed (T* pb = &right[0])
        fixed (T* pr = &result[0])
        {
            for (var i = 0; i < left.Length; i++)
            {
                pr[i] = pa[i] + pb[i];
            }
        }
    }
    
    internal static unsafe void ElementwiseSubtractionUnsafe<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        fixed (T* pa = &left[0])
        fixed (T* pb = &right[0])
        fixed (T* pr = &result[0])
        {
            for (var i = 0; i < left.Length; i++)
            {
                pr[i] = pa[i] - pb[i];
            }
        }
    }
    
    internal static unsafe void ElementwiseProductUnsafe<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        fixed (T* pa = &left[0])
        fixed (T* pb = &right[0])
        fixed (T* pr = &result[0])
        {
            for (var i = 0; i < left.Length; i++)
            {
                pr[i] = pa[i] * pb[i];
            }
        }
    }
    
    internal static unsafe void ElementwiseDivisionUnsafe<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        fixed (T* pa = &left[0])
        fixed (T* pb = &right[0])
        fixed (T* pr = &result[0])
        {
            for (var i = 0; i < left.Length; i++)
            {
                pr[i] = pa[i] / pb[i];
            }
        }
    }

    internal static void ElementwiseAdditionVectorized<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(left);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(right);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] + rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < left.Length; i++)
        {
            result[i] = left[i] + right[i];
        }
    }

    internal static void ElementwiseSubtractionVectorized<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(left);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(right);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] - rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < left.Length; i++)
        {
            result[i] = left[i] - right[i];
        }
    }

    internal static void ElementwiseProductVectorized<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(left);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(right);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] * rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < left.Length; i++)
        {
            result[i] = left[i] * right[i];
        }
    }

    internal static void ElementwiseDivisionVectorized<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Span<T> result) where T : struct, INumber<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(left);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(right);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] / rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < left.Length; i++)
        {
            result[i] = left[i] / right[i];
        }
    }


    internal static T SumSpan<T>(ReadOnlySpan<T> values) where T : struct, INumber<T>
    {
        T result = T.AdditiveIdentity;
        for (int i = 0; i < values.Length; i++)
        {
            result += values[i];
        }
        return result;
    }

    internal static T SumVectorized<T>(ReadOnlySpan<T> values) where T : struct, INumber<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(values);
        Vector<T> resultVec = Vector<T>.Zero;
        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec += leftVec[i];
        }
        T result = Vector.Dot(resultVec, Vector<T>.One);
        for (int i = leftVec.Length * Vector<T>.Count; i < values.Length; i++)
        {
            result += values[i];
        }
        return result;
    }


    internal static T DotProductSpan<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right) where T : struct, INumber<T>
    {
        T result = T.AdditiveIdentity;
        for (int i = 0; i < left.Length; i++)
        {
            result += left[i] * right[i];
        }
        return result;
    }

    internal static T DotProductVectorized1<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right) where T : struct, INumber<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(left);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(right);
        Vector<T> resultVec = Vector<T>.Zero;
        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec += leftVec[i] * rightVec[i];
        }
        T result = Vector.Dot(resultVec, Vector<T>.One);
        for (int i = leftVec.Length * Vector<T>.Count; i < left.Length; i++)
        {
            result += left[i] * right[i];
        }
        return result;
    }

    internal static T DotProductVectorized2<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right) where T : struct, INumber<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(left);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(right);

        T result = T.AdditiveIdentity;
        for (int i = 0; i < leftVec.Length; i++)
        {
            result += Vector.Dot(leftVec[i], rightVec[i]);
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < left.Length; i++)
        {
            result += left[i] * right[i];
        }
        return result;
    }

}
