using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.ExtensionHelpers;

public static class EnumeratorExtensions
{
    public static IEnumerable<object?> ToWeakTypedEnumerable(this IEnumerator enumerator)
    {
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    public static IEnumerable<T> ToStrongTypedEnumerable<T>(this IEnumerator<T> enumerator)
    {
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    
    public static Vertex2[] GenerateRandomVertex2(this Random random, int n)
    {
        return Enumerable.Repeat(new Vertex2(random.NextSingle(), random.NextSingle()), n).ToArray();
    }
}