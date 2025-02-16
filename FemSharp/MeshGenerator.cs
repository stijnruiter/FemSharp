using FemSharp.Render;
using LinearAlgebra.Structures;
using System.Diagnostics;

namespace FemSharp;

internal class MeshGenerator
{
    public static Mesh2D NaiveRectangle(Rect rect, uint nx, uint ny)
    {
        List<ValuedVertex> vertices = [];
        List<TriangularElement> interiorElements = [];
        List<LineElement> boundaryElements = [];

        float dx = rect.Width / (nx);
        float dy = rect.Height / (ny);

        for (uint j = 0; j <= ny; j++)
        {
            for (uint i = 0; i <= nx; i++)
            {
                var x = rect.Left + dx * i;
                var y = rect.Bottom + dy * j;
                vertices.Add(new ValuedVertex(x, y, 0f, 0f));
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

    public static Mesh2D NaiveCircle(float cx, float cy, float radius, float maxh)
    {
        List<Vertex> vertices = [];
        List<TriangularElement> interiorElements = [];
        List<LineElement> boundaryElements = [];

        uint pointsOnCircle = (uint)MathF.Ceiling(MathF.PI / MathF.Asin(0.5f * maxh / radius));
        pointsOnCircle = pointsOnCircle < 3 ? 3 : pointsOnCircle;
        var angle = 2f * MathF.PI / pointsOnCircle;

        vertices.Add(new Vertex(cx, cy, 0f));
        for (uint i = 0; i < pointsOnCircle; i++)
        {
              vertices.Add(new Vertex(cx + radius * MathF.Cos(i * angle), cy + radius * MathF.Sin(i * angle), 0f));
        }

        for (uint i = 1; i < pointsOnCircle; i++)
        {
            interiorElements.Add(new TriangularElement(0, i, i + 1));
            boundaryElements.Add(new LineElement(i, i + 1));
        }

        interiorElements.Add(new TriangularElement(0, 1, pointsOnCircle));
        boundaryElements.Add(new LineElement(1, pointsOnCircle));


        return new Mesh2D(vertices.Select(v => new ValuedVertex(v)).ToArray(), interiorElements.ToArray(), boundaryElements.ToArray());
    }

    public static Mesh2D DelaunayTriangulation(ReadOnlySpan<Vertex> vertices)
    {
        if (vertices.Length < 3)
            throw new ArgumentException("Unable to create triangulation. At least 3 vertices are required.");

        // Create a convex hull (rectangle) of the vertices, with a 0.1 margin
        var bounds = GetContainingRect(vertices, 0.1f);
        var convexHullVertices = new List<Vertex>()
        {
            new(bounds.Left, bounds.Bottom, 0),
            new(bounds.Right, bounds.Bottom, 0),
            new(bounds.Right, bounds.Top, 0),
            new(bounds.Left, bounds.Top, 0),
        };
        var interiorElements = new List<TriangularElement> { 
            new(0, 1, 2), new(0, 2, 3) 
        };

        var boundaryElements = new List<LineElement> {
            new(0, 1),new(1, 2),
            new(2, 3),new(0, 3)
        };

        // Insert points using Delaunay
        for (int i = 0; i < vertices.Length; i++)
        {
            InsertPoint(vertices[i], convexHullVertices, interiorElements, boundaryElements);
        }

        // Remove the 4 convex hull points
        boundaryElements.Clear();
        for (var i = interiorElements.Count - 1;  i >= 0; i--)
        {
            var element = interiorElements[i];
            if (element.I < 4)
            {
                if (element.J >=4 && element.K >=4)
                {
                    boundaryElements.Add(new LineElement(element.J - 4, element.K - 4));
                }
                interiorElements.RemoveAt(i);
            }
            else if (element.J < 4)
            {
                if (element.K >= 4)
                {
                    boundaryElements.Add(new LineElement(element.K - 4, element.I - 4));
                }
                interiorElements.RemoveAt(i);
            }
            else if (element.K < 4)
            {
                boundaryElements.Add(new LineElement(element.I - 4, element.J - 4));
                interiorElements.RemoveAt(i);
            }
            else
            {
                interiorElements[i] = new TriangularElement(element.I - 4, element.J - 4, element.K - 4);
            }
        }

        return new Mesh2D(convexHullVertices.Skip(4).Select(p => new ValuedVertex(p, 0)).ToArray(), 
            interiorElements.ToArray(), boundaryElements.ToArray());
    }

    private static Rect GetContainingRect(ReadOnlySpan<Vertex> vertices, float dilate)
    {
        Rect rect = new Rect(float.MaxValue, float.MinValue, float.MaxValue, float.MinValue);
        foreach(var vertex in vertices)
        {
            rect.Left   = float.Min(rect.Left,   vertex.X - dilate);
            rect.Right  = float.Max(rect.Right,  vertex.X + dilate);
            rect.Bottom = float.Min(rect.Bottom, vertex.Y - dilate);
            rect.Top    = float.Max(rect.Top,    vertex.Y + dilate);
        }
        return rect;
    }

    private static void InsertPoint(Vertex p, List<Vertex> vertices, List<TriangularElement> elements, List<LineElement> boundaryElements)
    {
        var element = FindElement(p, vertices, elements);
        var indexP = (uint)vertices.Count;

        // Replace ABC by ABP, ACP, BCP
        vertices.Add(p);
        elements.Remove(element);
        elements.AddRange(ReplaceElement(indexP, element));
        
        FlipTest(indexP, element.I, element.J, vertices, elements, boundaryElements);
        FlipTest(indexP, element.J, element.K, vertices, elements, boundaryElements);
        FlipTest(indexP, element.K, element.I, vertices, elements, boundaryElements);
    }

    private static void FlipTest(uint indexP, uint i, uint j, List<Vertex> vertices, List<TriangularElement> elements, List<LineElement> boundaryElements)
    {
        if (boundaryElements.Contains(new LineElement(i, j)) || boundaryElements.Contains(new LineElement(j, i))) 
            return;

        // TODO: store neighbours
        var elementsWithEdge = FindElementsWithEdge(i, j, elements).ToArray();
        Debug.Assert(elementsWithEdge.Length == 2);
        (var element_pij, var other_ij) = Contains(elementsWithEdge[0], indexP) ?
            (elementsWithEdge[0], elementsWithEdge[1]) : (elementsWithEdge[1], elementsWithEdge[0]);

        Debug.Assert(!Contains(other_ij, indexP));
        Debug.Assert(Contains(element_pij, indexP));

        var otherP = MirroredVertex(other_ij, i, j);

        var vi = vertices[(int)i];
        var vj = vertices[(int)j];
        var vp = vertices[(int)indexP];
        var vOther = vertices[(int)otherP];
        if (!InCircle(vj, vp, vi, vOther))
            return;

        // Flip edges
        var newE1 = new TriangularElement(indexP, i, otherP);
        var newE2 = new TriangularElement(indexP, otherP, j);
        elements.Remove(element_pij);
        elements.Remove(other_ij);
        elements.Add(newE1);
        elements.Add(newE2);

        // Test new edges 
        FlipTest(indexP, i, otherP, vertices, elements, boundaryElements);
        FlipTest(indexP, otherP, j, vertices, elements, boundaryElements);
    }

    private static uint MirroredVertex(TriangularElement element, uint i, uint j)
    {
        if (element.I != i && element.I != j)
            return element.I;
        if (element.J != i && element.J != j)
            return element.J;
        return element.K;
    }

    private static bool Contains(TriangularElement element, uint i)
    {
        return element.I == i || element.J == i || element.K == i;
    }

    private static IEnumerable<TriangularElement> FindElementsWithEdge(uint i, uint j, List<TriangularElement> elements)
    {
        foreach(var element in elements)
        {
            if (Contains(element, i) && Contains(element, j))
                yield return element;
        }
    }

    private static IEnumerable<TriangularElement> ReplaceElement(uint indexP, TriangularElement element)
    {
        yield return new TriangularElement(indexP, element.I, element.J);
        yield return new TriangularElement(indexP, element.J, element.K);
        yield return new TriangularElement(indexP, element.K, element.I);
    }

    private static TriangularElement FindElement(Vertex point, List<Vertex> vertices, List<TriangularElement> elements)
    {
        foreach (var element in elements)
        {
            var v1 = vertices[(int)element.I];
            var v2 = vertices[(int)element.J];
            var v3 = vertices[(int)element.K];
            if (!PointInTriangle(point, v1, v2, v3))
                continue;

            return element;
        }
        throw new Exception($"Point {point} not in elements.");
    }

    private static bool InCircle(Vertex a, Vertex b, Vertex c, Vertex d)
    {
        return InCircleDet(a, b, c, d) > 0;
    }

    private static float InCircleDet(Vertex a, Vertex b,  Vertex c, Vertex d)
    {
        return new Matrix<float>(new float[,]{
            { a.X, a.Y, a.X * a.X + a.Y * a.Y, 1 },
            { b.X, b.Y, b.X * b.X + b.Y * b.Y, 1},
            { c.X, c.Y, c.X * c.X + c.Y * c.Y, 1},
            { d.X, d.Y, d.X * d.X + d.Y * d.Y, 1} }).Determinant();
    }

    private static bool PointInTriangle(Vertex p, Vertex v1, Vertex v2, Vertex v3)
    {
        var d1 = HalfPlaneSide(p, v1, v2);
        var d2 = HalfPlaneSide(p, v2, v3);
        var d3 = HalfPlaneSide(p, v3, v1);

        var has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        var has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);

        float HalfPlaneSide(Vertex p1, Vertex p2, Vertex p3)
        {
            return (p1.X - p3.X) * (p2.Y- p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
    }
}
