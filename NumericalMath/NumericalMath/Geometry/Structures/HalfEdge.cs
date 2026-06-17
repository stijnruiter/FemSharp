using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NumericalMath.Geometry.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct HalfEdge
{
    public int V1 = -1;
    public int V2 = -1;
    public int PrevEdge = -1;
    public int NextEdge = -1;
    public int TwinEdge = -1;
    public int ElementIndex = -1;

    public HalfEdge()
    {
    }

    public HalfEdge(int v1, int v2, int prevEdge, int nextEdge, int twinEdge, int elementIndex)
    {
        V1 = v1;
        V2 = v2;
        PrevEdge = prevEdge;
        NextEdge = nextEdge;
        TwinEdge = twinEdge;
        ElementIndex = elementIndex;
    }

    public override string ToString() =>
        $"Face {ElementIndex}: prev(e): {PrevEdge}, next(e): {NextEdge}, twin(e): {TwinEdge}. ({V1}, {V2})";
}
