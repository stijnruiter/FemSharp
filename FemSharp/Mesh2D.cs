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

    public static Mesh2D NaiveRectangle(Rect rect, uint nx, uint ny)
    {
        List<Vertex> vertices = new List<Vertex>();
        List<TriangularElement> interiorElements = new List<TriangularElement>();
        List<LineElement> boundaryElements = new List<LineElement>();

        float dx = rect.Width / (nx);
        float dy = rect.Height / (ny);

        for (uint j = 0; j <= ny; j++)
        {
            for (uint i = 0; i <= nx; i++)
            {
                var x = rect.Left + dx * i;
                var y = rect.Bottom + dy * j;
                vertices.Add(new Vertex(x, y, 0f));
            }
        }

        for (uint j = 0; j < ny; j++)
        {
            for (uint i = 0; i < nx; i++)
            {
                // 3--2
                // |  |
                // 0--1

                var corner0 = i + (nx + 1) * j;
                var corner1 = i + 1 + (nx + 1) * j;
                var corner2 = i + 1 + (nx + 1) * (j + 1);
                var corner3 = i + (nx + 1) * (j + 1);

                interiorElements.Add(new TriangularElement(corner0, corner1, corner2));
                interiorElements.Add(new TriangularElement(corner0, corner2, corner3));
            }
        }

        for (uint i = 0; i < nx; i++)
        {
            boundaryElements.Add(new LineElement(i, i + 1));
            boundaryElements.Add(new LineElement(i + (nx + 1) * ny, i + 1 + (nx + 1) * ny));
        }

        for (uint j = 0; j < ny; j++)
        {
            //boundaryElements.Add(new LineElement(i, i + 1));
            boundaryElements.Add(new LineElement((nx + 1) * j, (nx + 1) * (j + 1)));
            boundaryElements.Add(new LineElement((nx + 1) * j + nx, (nx + 1) * (j + 1) + nx));
        }

        return new Mesh2D(vertices.ToArray(), interiorElements.ToArray(), boundaryElements.ToArray());
    }
}