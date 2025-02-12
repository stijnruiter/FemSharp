using System.Runtime.InteropServices;

namespace FemSharp.Render;

[StructLayout(LayoutKind.Sequential)]
internal struct Vertex(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ValuedVertex(Vertex position, float value = 0.0f)
{
    public Vertex Position = position;
    public float Value = value;

    public ValuedVertex(float x, float y, float z, float value = 0.0f) : this(new Vertex(x, y, z), value)
    {
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct TriangularElement(uint i, uint j, uint k)
{
    public uint I = i;
    public uint J = j;
    public uint K = k;
}

[StructLayout(LayoutKind.Sequential)]
internal struct LineElement(uint i, uint j)
{
    public uint I = i;
    public uint J = j;
}

internal static class ElementExtensions
{
    public static float Distance(this Vertex vertex1, Vertex vertex2)
    {
        return MathF.Sqrt(MathF.Pow(vertex1.X - vertex2.X, 2) + MathF.Pow(vertex1.Y - vertex2.Y, 2) + MathF.Pow(vertex1.Z - vertex2.Z, 2));
    }

    public static float Distance(this ValuedVertex vertex1, ValuedVertex vertex2)
    {
        return Distance(vertex1.Position, vertex2.Position);
    }
}