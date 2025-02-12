using FemSharp.Render;
using LinearAlgebra.Structures;

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
    public ValuedVertex[] Vertices { get; }
    public TriangularElement[] InteriorElements { get; }
    public LineElement[] BoundaryElements { get; }
    public LineElement[] InteriorEdges { get; }

    public ColumnVector<float> VertexValues { get; set; }

    public Mesh2D(ValuedVertex[] vertices, TriangularElement[] interior, LineElement[] boundary)
    {
        Vertices = vertices;
        InteriorElements = interior;
        BoundaryElements = boundary;
        InteriorEdges = EdgesFromElements(interior);
        VertexValues = ColumnVector<float>.Zero(vertices.Length);
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

    public void SetFemValues(AbstractVector<float> values)
    {
        if (values.Length != Vertices.Length)
            throw new ArgumentException($"Element lengths do not match. {values.Length}!={Vertices.Length}");

        // Normalize values between 0 and 1
        var min = float.MaxValue;
        var max = float.MinValue;
        foreach(var value in values)
        {
            min = min > value ? value : min;
            max = max < value ? value : max;
        }

        for (var i = 0; i < Vertices.Length; i++)
        {
            Vertices[i].Position.Z = values[i];
            Vertices[i].Value = (values[i] - min) / (max - min);
        }
    }
}