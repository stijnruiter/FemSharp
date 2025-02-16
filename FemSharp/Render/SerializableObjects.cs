using NumericalMath.Geometry.Structures;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FemSharp.Render;


[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("P: {Position}, V: {Value}")]
internal struct ValuedVertex(Vertex3 position, float value = 0.0f)
{
    public Vertex3 Position = position;
    public float Value = value;

    public ValuedVertex(Vertex2 vertex, float z = 0f, float value = 0.0f) : this(vertex.X, vertex.Y, z, value)
    {
    }

    public ValuedVertex(float x, float y, float z, float value = 0.0f) : this(new Vertex3(x, y, z), value)
    {
    }
}

[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("P: {Position}, Color: {Value}")]
internal struct ColoredVertex(Vertex3 position, Color4 color)
{
    public Vertex3 Position = position;
    public Color4 Value = color;

    public ColoredVertex(float x, float y, float z, Color4 color) : this(new Vertex3(x, y, z), color)
    {
    }
}

internal static class ElementExtensions
{
    public static float Distance(this Vertex3 vertex1, Vertex3 vertex2)
    {
        return MathF.Sqrt(MathF.Pow(vertex1.X - vertex2.X, 2) + MathF.Pow(vertex1.Y - vertex2.Y, 2) + MathF.Pow(vertex1.Z - vertex2.Z, 2));
    }

    public static float Distance(this ValuedVertex vertex1, ValuedVertex vertex2)
    {
        return Distance(vertex1.Position, vertex2.Position);
    }
}