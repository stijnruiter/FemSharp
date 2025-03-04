using FemSharp.Render;
using NumericalMath.Geometry;
using NumericalMath.Geometry.Structures;

namespace FemSharp;

internal static class MeshGenerator
{
    public static Mesh2D NaiveRectangle(Rectangle rect, int nx, int ny)
    {
        List<ValuedVertex> vertices = [];
        List<TriangleElement> interiorElements = [];
        List<LineElement> boundaryElements = [];

        float dx = rect.Width / (nx);
        float dy = rect.Height / (ny);

        for (int j = 0; j <= ny; j++)
        {
            for (int i = 0; i <= nx; i++)
            {
                var x = rect.Left + dx * i;
                var y = rect.Bottom + dy * j;
                vertices.Add(new ValuedVertex(x, y, 0f, 0f));
            }
        }

        for (int j = 0; j < ny; j++)
        {
            for (int i = 0; i < nx; i++)
            {
                // 3--2
                // |  |
                // 0--1

                var corner0 = i + (nx + 1) * j;
                var corner1 = i + 1 + (nx + 1) * j;
                var corner2 = i + 1 + (nx + 1) * (j + 1);
                var corner3 = i + (nx + 1) * (j + 1);

                interiorElements.Add(new TriangleElement(corner0, corner1, corner2));
                interiorElements.Add(new TriangleElement(corner0, corner2, corner3));
            }
        }

        for (int i = 0; i < nx; i++)
        {
            boundaryElements.Add(new LineElement(i, i + 1));
            boundaryElements.Add(new LineElement(i + (nx + 1) * ny, i + 1 + (nx + 1) * ny));
        }

        for (int j = 0; j < ny; j++)
        {
            //boundaryElements.Add(new LineElement(i, i + 1));
            boundaryElements.Add(new LineElement((nx + 1) * j, (nx + 1) * (j + 1)));
            boundaryElements.Add(new LineElement((nx + 1) * j + nx, (nx + 1) * (j + 1) + nx));
        }

        return new Mesh2D(vertices.ToArray(), interiorElements.ToArray(), boundaryElements.ToArray());
    }

    public static Mesh2D NaiveCircle(float cx, float cy, float radius, float maxh)
    {
        List<Vertex2> vertices = [];
        List<TriangleElement> interiorElements = [];
        List<LineElement> boundaryElements = [];

        int pointsOnCircle = (int)MathF.Ceiling(MathF.PI / MathF.Asin(0.5f * maxh / radius));
        pointsOnCircle = pointsOnCircle < 3 ? 3 : pointsOnCircle;
        var angle = 2f * MathF.PI / pointsOnCircle;

        vertices.Add(new Vertex2(cx, cy));
        for (int i = 0; i < pointsOnCircle; i++)
        {
              vertices.Add(new Vertex2(cx + radius * MathF.Cos(i * angle), cy + radius * MathF.Sin(i * angle)));
        }

        for (int i = 1; i < pointsOnCircle; i++)
        {
            interiorElements.Add(new TriangleElement(0, i, i + 1));
            boundaryElements.Add(new LineElement(i, i + 1));
        }

        interiorElements.Add(new TriangleElement(0, 1, pointsOnCircle));
        boundaryElements.Add(new LineElement(1, pointsOnCircle));

        return new Mesh2D(vertices.Select(v => new ValuedVertex(v)).ToArray(), interiorElements.ToArray(), boundaryElements.ToArray());
    }
    
    public static RefinedDelaunay DelaunayCircle(float cx, float cy, float radius, float maxh)
    {
        var graph = new PlanarStraightLineGraph();
        var pointsOnCircle = (int)MathF.Ceiling(MathF.PI / MathF.Asin(0.5f * maxh / radius));
        pointsOnCircle = pointsOnCircle < 3 ? 3 : pointsOnCircle;
        var angle = 2f * MathF.PI / pointsOnCircle;
        graph.AddClosedLineSegments(Enumerable.Range(0, pointsOnCircle)
            .Select(i => new Vertex2(cx + radius * MathF.Cos(i * angle), cy + radius * MathF.Sin(i * angle))).ToArray());
        var delaunay =  RefinedDelaunay.CreateTriangulation(graph);
        delaunay.InsertPoint(new Vertex2(cx, cy));
        return delaunay;
    }
    

    public static Mesh2D DelaunayTriangulation(Vertex2[] vertices)
    {
        var triangulation = Delaunay.CreateTriangulation(vertices).ToMesh();
        return new Mesh2D(vertices.Select(v => new ValuedVertex(v)).ToArray(), triangulation.Interior.ToArray(), triangulation.Boundary.ToArray());
    }

}
