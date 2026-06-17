using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NumericalMath.Geometry.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct LineElement(int i, int j)
{
    public int I = i;
    public int J = j;

    public override string ToString()
    {
        return $"({I}, {J})";
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct TriangleElement(int i, int j, int k)
{
    public int I = i;
    public int J = j;
    public int K = k;

    public override string ToString()
    {
        return $"({I}, {J}, {K})";
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
[DebuggerDisplay("({I}, {J}, {K}, {L})")]
public struct TetrahedronElement(int i, int j, int k, int l)
{
    public int I = i;
    public int J = j;
    public int K = k;
    public int L = l;

    public override string ToString()
    {
        return $"({I}, {J}, {K}, {L})";
    }
}