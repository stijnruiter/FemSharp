using System;
using System.Linq;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Geometry;

/**
 * J. Ruppert, A Delaunay Refinement Algorithm for Quality 2-Dimensional Mesh Generation,
 * Journal of Algorithms, Volume 18, Issue 3, 1995, https://doi.org/10.1006/jagm.1995.1021.
 */
public class RefinedDelaunay : Delaunay
{
    public PlanarStraightLineGraph Graph { get; }
    
    private RefinedDelaunay(PlanarStraightLineGraph graph, Triangle boundingElement, int nVertexCapacity) : base(boundingElement, nVertexCapacity)
    {
        Graph = graph;
    }
    
    public static RefinedDelaunay CreateTriangulation(PlanarStraightLineGraph graph)
    {
        if(graph.Vertices.Count <= 3)
            throw new ArgumentException("Graph is empty.");
        
        var delaunay = new RefinedDelaunay(graph, Triangle.ContainingTriangle(graph.Vertices.ToArray(), 1e5f), graph.Vertices.Count);
        foreach (var vertex in graph.Vertices)
        {
            delaunay.InsertPoint(vertex);
        }
        
        return delaunay;
    }

    public void Refine(float alphaDegrees)
    {
        var alphaRadians = float.DegreesToRadians(alphaDegrees);
        while (true)
        {
            for (int i = 0; i < Graph.Segments.Count; i++)
            {
                var segment = Graph.Segments[i];
                var v1 = Graph.Vertices[segment.I];
                var v2 = Graph.Vertices[segment.J];

                var mid = 0.5f * (v1 + v2);
                var radius = Distance(v1, mid);
                if (_vertices.Any(p => Distance(p, mid) < radius - 1e-6f))
                {
                    InsertPoint(mid);
                    Graph.SplitLineSegment(i);
                    i--;
                }
            }

            var smallestTriangle = GetSmallestAngleTriangle();
            var circumCenter = smallestTriangle.Circumcenter();
            var angle = smallestTriangle.GetSmallestAngle();
            Console.WriteLine($"Smallest angle: {angle}");
            if (angle >= alphaRadians)
                return;

            var circumcenterEncroached = false;
            for (var i = 0; i < Graph.Segments.Count; i++)
            {
                var segment = Graph.Segments[i];
                var v1 = Graph.Vertices[segment.I];
                var v2 = Graph.Vertices[segment.J];

                var mid = 0.5f * (v1 + v2);
                var radius = Distance(v1, mid);
                if (Distance(circumCenter, mid) < radius - 1e-3f)
                {
                    InsertPoint(mid);
                    Graph.SplitLineSegment(i);
                    i--;
                    circumcenterEncroached = true;
                }
            }

            if (!circumcenterEncroached)
            {
                InsertPoint(circumCenter);
            }
        }
    }
    
    private static float Distance(Vertex2 v1, Vertex2 v2) => MathF.Sqrt(MathF.Pow(v1.X - v2.X, 2) + MathF.Pow(v1.Y - v2.Y, 2));
}