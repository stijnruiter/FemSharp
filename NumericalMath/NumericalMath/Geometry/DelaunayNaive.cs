using NumericalMath.Geometry.Structures;
using NumericalMath.LinearAlgebra.Structures;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;

namespace NumericalMath.Geometry;

public interface IDelaunay
{
    public Triangle GetSmallestAngleTriangle();
    public IReadOnlyList<Vertex2> Vertices { get; }
    static abstract float InCircleDeterminant(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d);
    static abstract IDelaunay CreateTriangulation(ReadOnlySpan<Vertex2> vertices);
    (List<Vertex2> Vertices, List<TriangleElement> Interior, List<LineElement> Boundary) ToMesh();

    public int FindElement(Vertex2 point);
    public void InsertPoint(Vertex2 point);
}

internal class DelaunayNaive : IDelaunay
{
    private readonly List<Vertex2> _vertices;
    private readonly List<TriangleElement> _interiorElements;
    private readonly List<LineElement> _boundaryElements;
    
    private DelaunayNaive(Triangle boundingTriangle, int vertexCapacity)
    {
        _vertices = new List<Vertex2>(vertexCapacity + 3)
        {
            boundingTriangle.V1,
            boundingTriangle.V2,
            boundingTriangle.V3
        };
        _interiorElements = new List<TriangleElement>(2 * _vertices.Capacity + 1)
        {
            new(0, 1, 2)
        };
        _boundaryElements = [];
    }

    public static IDelaunay CreateTriangulation(ReadOnlySpan<Vertex2> vertices)
    {
        // Create an "infinite" containing triangle
        var containingTriangle = Triangle.ContainingTriangle(vertices.ToArray(), 1e5f);
        var delaunay = new DelaunayNaive(containingTriangle, vertices.Length);
        foreach(var vertex in vertices)
        {
            delaunay.InsertPoint(vertex);
        }
        return delaunay;
    }

    public (List<Vertex2> Vertices, List<TriangleElement> Interior, List<LineElement> Boundary) ToMesh()
    {
        Cleanup();
        return (_vertices[3..], _interiorElements, _boundaryElements);
    }

    public void InsertPoint(Vertex2 point)
    {
        var indexP = _vertices.Count;
        _vertices.Add(point);
        var index = FindElement(point);
        var element = _interiorElements[index];

        // Replace ABC by ABP, ACP, BCP
        _interiorElements.RemoveAt(index);
        _interiorElements.Add(new(indexP, element.I, element.J));
        _interiorElements.Add(new(indexP, element.J, element.K));
        _interiorElements.Add(new(indexP, element.K, element.I));

        FlipTest(indexP, element.I, element.J);
        FlipTest(indexP, element.J, element.K);
        FlipTest(indexP, element.K, element.I);
    }

    private void FlipTest(int indexP, int i, int j)
    {
        if (IsBoundaryEdge(i, j))
            return;

        var elementsWithEdge = FindElementsWithEdge(i, j).ToArray();
        Debug.Assert(elementsWithEdge.Length == 2);
        
        (var element_pij, var other_ij) = Contains(elementsWithEdge[0], indexP) ? (elementsWithEdge[0], elementsWithEdge[1]) : (elementsWithEdge[1], elementsWithEdge[0]);

        Debug.Assert(!Contains(other_ij, indexP));
        Debug.Assert(Contains(element_pij, indexP));

        var otherP = GetThirdIndex(other_ij, i, j);
        if (!InCircle(j, indexP, i, otherP))
            return;

        // Flip edges
        _interiorElements.Remove(element_pij);
        _interiorElements.Remove(other_ij);
        _interiorElements.Add(new TriangleElement(indexP, i, otherP));
        _interiorElements.Add(new TriangleElement(indexP, otherP, j));

        // Test new edges 
        FlipTest(indexP, i, otherP);
        FlipTest(indexP, otherP, j);
    }

    private IEnumerable<TriangleElement> FindElementsWithEdge(int i, int j)
    {
        foreach (var element in _interiorElements)
        {
            if (Contains(element, i) && Contains(element, j))
                yield return element;
        }
    }

    public int FindElement(Vertex2 point)
    {
        for (var i = 0; i < _interiorElements.Count; i++)
        {
            var element = _interiorElements[i];
            if (Triangle.Contains(point, _vertices[element.I], _vertices[element.J], _vertices[element.K]))
                return i;
        }
        throw new Exception($"Point {point} not in elements.");
    }

    private bool InCircle(int ai, int bi, int ci, int di)
    {
        return InCircleDeterminant(_vertices[ai], _vertices[bi], _vertices[ci], _vertices[di]) > 0;
    }

    public Triangle GetSmallestAngleTriangle()
    {
        Triangle? smallestAngleTriangle = null;
        float smallestAngle = float.MaxValue;
        foreach (var element in _interiorElements)
        {
            if (element.I < 3 || element.J < 3 || element.K < 3)
                continue;

            var triangle = GetTriangle(element);
            var angle = triangle.GetSmallestAngle();

            if (angle < smallestAngle)
            {
                smallestAngle = angle;
                smallestAngleTriangle = triangle;
            }
        }

        return smallestAngleTriangle!.Value;

        Triangle GetTriangle(TriangleElement element)
        {
            var v1 = _vertices[element.I];
            var v2 = _vertices[element.J];
            var v3 = _vertices[element.K];
            return new Triangle(v1, v2, v3);
        }
    }
    

    public IReadOnlyList<Vertex2> Vertices => _vertices;

    public static float InCircleDeterminant(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d)
    {
        return new Matrix<float>(new float[,]{
            { a.X, a.Y, a.X * a.X + a.Y * a.Y, 1 },
            { b.X, b.Y, b.X * b.X + b.Y * b.Y, 1},
            { c.X, c.Y, c.X * c.X + c.Y * c.Y, 1},
            { d.X, d.Y, d.X * d.X + d.Y * d.Y, 1} }).Determinant();
    }

    private static int GetThirdIndex(TriangleElement element, int i, int j)
    {
        if (element.I != i && element.I != j)
            return element.I;
        if (element.J != i && element.J != j)
            return element.J;
        return element.K;
    }

    private static bool Contains(TriangleElement element, int i)
    {
        return element.I == i || element.J == i || element.K == i;
    }

    private static bool IsBoundaryEdge(int i, int j)
    {
        // It is an edge if both indices are 0, 1, 2
        return i < 3 && j < 3;
    }
    
    private void Cleanup()
    {
        _boundaryElements.Clear();
        Delaunay.Cleanup(_interiorElements, _boundaryElements);
    }
}