using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NumericalMath.Geometry.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
[DebuggerDisplay("({X}, {Y})")]
public struct Vertex2(float x, float y)
{
    public float X = x;
    public float Y = y;

    public override string ToString() => $"({X}, {Y})";
    public static Vertex2 operator +(Vertex2 lhs, Vertex2 rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y);
    public static Vertex2 operator -(Vertex2 lhs, Vertex2 rhs) => new(lhs.X - rhs.X, lhs.Y - rhs.Y);
    public static Vertex2 operator *(Vertex2 lhs, float rhs) => new(lhs.X * rhs, lhs.Y * rhs);
    public static Vertex2 operator *(float lhs, Vertex2 rhs) => rhs * lhs;

    public Vertex2 Normalized()
    {
        var length = MathF.Sqrt(X * X + Y * Y);
        return new Vertex2(X / length, Y / length);
    }

    public static float DotProduct(Vertex2 a, Vertex2 b) => a.X * b.X + a.Y * b.Y;
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
[DebuggerDisplay("({X}, {Y}, {Z})")]
public struct Vertex3(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;

    public override string ToString() => $"({X}, {Y}, {Z})";
}
