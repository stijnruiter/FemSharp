using System;
using System.Collections.Generic;

namespace NumericalMath.Geometry.Structures;

public struct Rectangle(float left, float right, float bottom, float top)
{
    public float Left = left;
    public float Right = right;
    public float Top = top;
    public float Bottom = bottom;

    public readonly float Width => Right - Left;
    public readonly float Height => Top - Bottom;

    /// <summary>
    /// Return the four corners of the rectangle in counter-clockwise order
    /// </summary>
    public IEnumerable<Vertex2> ToVertices()
    {
        yield return new Vertex2(Left, Bottom);
        yield return new Vertex2(Right, Bottom);
        yield return new Vertex2(Right, Top);
        yield return new Vertex2(Left, Top);
    }

    public static Rectangle BoundingBox(ReadOnlySpan<Vertex2> vertices, float dilate = 0f)
    {
        Rectangle rect = new(float.MaxValue, float.MinValue, float.MaxValue, float.MinValue);
        foreach (var vertex in vertices)
        {
            rect.Left = float.Min(rect.Left, vertex.X - dilate);
            rect.Right = float.Max(rect.Right, vertex.X + dilate);
            rect.Bottom = float.Min(rect.Bottom, vertex.Y - dilate);
            rect.Top = float.Max(rect.Top, vertex.Y + dilate);
        }
        return rect;
    }
}
