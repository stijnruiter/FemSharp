using FemSharp.Render;

namespace FemSharp;

internal struct Rect(float left, float right, float bottom, float top)
{
    public float Left = left;
    public float Right = right;
    public float Top = top;
    public float Bottom = bottom;

    public readonly float Width => Right - Left;
    public readonly float Height => Top - Bottom;
}

internal class Mesh2D
{
    public Vertex[] Vertices { get; }
    public TriangularElement[] InteriorElements { get; }
    public LineElement[] BoundaryElements { get; }
    public LineElement[] InteriorEdges { get; }

    public Mesh2D(Vertex[] vertices, TriangularElement[] interior, LineElement[] boundary)
    {
        Vertices = vertices;
        InteriorElements = interior;
        BoundaryElements = boundary;
        InteriorEdges = EdgesFromElements(interior);
    }

    private static LineElement[] EdgesFromElements(TriangularElement[] elements)
    {
        var edges = new List<LineElement>();
        foreach (var element in elements)
        {
            edges.Add(UnidirectionalEdge(element.I, element.J));
            edges.Add(UnidirectionalEdge(element.J, element.K));
            edges.Add(UnidirectionalEdge(element.K, element.I));
        }
        LineElement UnidirectionalEdge(uint x, uint y) => new LineElement(x < y ? x : y, x < y ? y : x);
        return edges.Distinct().ToArray();
    }
}