using System;
using System.Linq;
using NumericalMath.Geometry;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.Geometry;

[TestFixture]
public class RefinedDelaunayTests
{
    [Test]
    public void CreateTriangulation_WhenGraphHasLessThen3Vertices_ShouldThrowException()
    {
        var graph = new PlanarStraightLineGraph();
        Assert.That(() => { _ = RefinedDelaunay.CreateTriangulation(graph); }, Throws.ArgumentException);
        graph.AddLineSegment(new Vertex2(0, 0), new Vertex2(1, 1));
        Assert.That(() => { _ = RefinedDelaunay.CreateTriangulation(graph); }, Throws.ArgumentException);
    }

    [Test]
    public void CreateTriangulation_WhenGraph1Polygon_CreatesDelaunayOfVertices()
    {
        var zPolygon = new Vertex2[] { new(0, 0), new(1, 0), new(0, 1), new(1, 2) };
        var graph = new PlanarStraightLineGraph();
        graph.AddLineSegments(zPolygon);
        var delaunay = RefinedDelaunay.CreateTriangulation(graph);
        var (meshVertices, meshInterior, _) = delaunay.ToMesh();
        Assert.That(meshVertices, Is.EquivalentTo(zPolygon));
        Assert.That(meshInterior, Is.EquivalentTo(new TriangleElement[]
        {
            new(0, 1, 2),
            new(1, 3, 2),
        }));
    }

    [Test]
    public void Refine_WhenAngleIsLarge_ShouldResultSimpleDelaunay()
    {
        var zPolygon = new Vertex2[] { new(0, 0), new(1, 0), new(0, 1), new(1, 2) };
        var graph = new PlanarStraightLineGraph();
        graph.AddLineSegments(zPolygon);
        var delaunay = RefinedDelaunay.CreateTriangulation(graph);
        delaunay.Refine(25);
        var (meshVertices, meshInterior, _) = delaunay.ToMesh();
        Assert.That(meshVertices, Is.EquivalentTo(zPolygon));
        Assert.That(meshInterior, Is.EquivalentTo(new TriangleElement[]
        {
            new(0, 1, 2),
            new(1, 3, 2),
        }));
    }

    [TestCase(6, 60, 60)]
    [TestCase(12, 30, 30)]
    [TestCase(20, 18, 31.717f)]
    [TestCase(30, 12, 25.111f)]
    public void Refine_WhenAngleIsSmall_ShouldResultRefinedDelaunay(int n, float angeBeforeRefinement, float angleAfterRefinement)
    {
        var angle = 2f * MathF.PI / n;
        var vertices = Enumerable.Range(0, n)
            .Select(i => new Vertex2(MathF.Cos(i * angle), MathF.Sin(i * angle))).ToArray();
        var graph = new PlanarStraightLineGraph();
        graph.AddClosedLineSegments(vertices);
        var delaunay = RefinedDelaunay.CreateTriangulation(graph);
        delaunay.InsertPoint(new Vertex2(0, 0));
        Assert.That(float.RadiansToDegrees(delaunay.GetSmallestAngleTriangle().GetSmallestAngle()), Is.EqualTo(angeBeforeRefinement).Within(1e-3f));
        delaunay.Refine(25);
        Assert.That(float.RadiansToDegrees(delaunay.GetSmallestAngleTriangle().GetSmallestAngle()), Is.EqualTo(angleAfterRefinement).Within(1e-3f));
    }

    [Test]
    public void Refine_WhenPolygonIsComplex_ShouldResultRefinedDelaunay()
    {
        Vertex2[] vertices =
        [
            new(-0.822222222f,  0.862222222f),
            new(0.257777778f,  0.76f),
            new(0.057777778f,  0.053333333f),
            new(0.333333333f, -0.235555556f),
            new(0.502222222f,  0.626666667f),
            new(0.764444444f, -0.075555556f),
            new(0.466666667f, -0.791111111f),
            new(0.155555556f, -0.871111111f),
            new(-0.177777778f, -0.866666667f),
            new(-0.497777778f, -0.684444444f),
            new(0.04f, -0.457777778f),
            new(0.022222222f, -0.395555556f),
            new(-0.773333333f, -0.035555556f),
        ];
        var graph = new PlanarStraightLineGraph();
        graph.AddClosedLineSegments(vertices);
        var delaunay = RefinedDelaunay.CreateTriangulation(graph);
        var (verticesBefore, interiorElementsBefore, _) = delaunay.ToMesh();
        
        Assert.That(verticesBefore, Is.EquivalentTo(graph.Vertices));
        Assert.That(interiorElementsBefore, Is.EquivalentTo(new TriangleElement[]
        {
            new(0, 2, 1), 
            new(10, 6, 3), 
            new(3, 6, 5), 
            new(5, 2, 3), 
            new(2, 4, 1), 
            new(12, 2, 0), 
            new(3, 11, 10), 
            new(2, 5, 4), 
            new(6, 10, 7), 
            new(11, 3, 2),
            new (9, 8, 10), 
            new (7, 10, 8), 
            new (12, 11, 2), 
            new (9, 10, 11), 
            new (11, 12, 9)
        }).Using<TriangleElement>(DelaunayTests<Delaunay>.AreCyclicalIdentical));
        

        delaunay.Refine(7);
        var (verticesAfter, interiorElementsAfter, _) = delaunay.ToMesh();
        Assert.That(verticesAfter, Has.Count.EqualTo(19));
        Assert.That(interiorElementsAfter, Is.EquivalentTo(new TriangleElement[]
        {
            new (12, 18, 0), 
            new (10, 6, 3), 
            new (15, 3, 6), 
            new (13, 3, 5), 
            new (4, 18, 13), 
            new (18, 12, 2), 
            new (3, 11, 10), 
            new (3, 13, 2), 
            new (6, 10, 7), 
            new (11, 3, 2),
            new (16, 8, 10), 
            new (7, 10, 8), 
            new (9, 17, 12), 
            new (8, 16, 9), 
            new (17, 9, 16), 
            new (18, 4, 1), 
            new (4, 13, 14), 
            new (13, 5, 14), 
            new (3, 15, 5), 
            new (10, 11, 16),
            new (17, 16, 11), 
            new (11, 2, 17), 
            new (2, 12, 17), 
            new (13, 18, 2), 
            new (1, 0, 18)
        }).Using<TriangleElement>(DelaunayTests<Delaunay>.AreCyclicalIdentical));
    }
}