using System.Runtime.InteropServices;

namespace FemSharp.Render;

[StructLayout(LayoutKind.Sequential)]
internal record struct FemVertex(float X, float Y, float Z, float Value);

[StructLayout(LayoutKind.Sequential)]
internal record struct TriangularElement(uint I, uint J, uint K);

[StructLayout(LayoutKind.Sequential)]
internal record struct LineElement(uint I, uint J);

internal static class ElementExtensions
{
    public static float Distance(this FemVertex vertex1, FemVertex vertex2)
    {
        return MathF.Sqrt(MathF.Pow(vertex1.X - vertex2.X, 2) + MathF.Pow(vertex1.Y - vertex2.Y, 2) + MathF.Pow(vertex1.Z - vertex2.Z, 2));
    }
}