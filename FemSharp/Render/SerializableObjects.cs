using System.Runtime.InteropServices;

namespace FemSharp.Render;

[StructLayout(LayoutKind.Sequential)]
internal record struct FemVertex(float X, float Y, float Z, float Value);

[StructLayout(LayoutKind.Sequential)]
internal record struct TriangularElement(uint I, uint J, uint K);

[StructLayout(LayoutKind.Sequential)]
internal record struct LineElement(uint I, uint J);