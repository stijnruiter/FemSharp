using System;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.Geometry.Structures;
using NumericalMath.Tests.ExtensionHelpers;

namespace NumericalMath.Tests.Geometry.Structures;

[TestFixture]
public class PlanarStraightLineGraphTests
{
    [Test]
    public void Construct_WhenNoArguments_ShouldCreateEmptyGraph()
    {
        var graph = new PlanarStraightLineGraph();
        Assert.That(graph.Vertices, Has.Count.EqualTo(0));
        Assert.That(graph.Segments, Has.Count.EqualTo(0));
    }

    private static IEnumerable<TestCaseData> OpenPolygonSets()
    {
        Vertex2[] rectangle = [new(0, 0), new(1, 0), new(1, 1), new(0, 1)];
        LineElement[] lineSegments = [new(0, 1), new(1, 2), new(2, 3)];
        yield return new TestCaseData(rectangle).Returns(lineSegments).SetArgDisplayNames("4 vertices");
        
        var polygon = new Random(12345).GenerateRandomVertex2(10);
        lineSegments = [
            new(0, 1), new(1, 2), new(2, 3), 
            new(3, 4), new(4, 5), new(5, 6), 
            new(6, 7), new(7, 8), new(8, 9)
        ];
        yield return new TestCaseData(polygon).Returns(lineSegments).SetArgDisplayNames("10 vertices");
    }

    private static IEnumerable<TestCaseData> ClosedPolygonSets()
    {
        Vertex2[] rectangle = [new(0, 0), new(1, 0), new(1, 1), new(0, 1)];
        LineElement[] lineSegments = [new(0, 1), new(1, 2), new(2, 3), new(3, 0)];
        yield return new TestCaseData(rectangle).Returns(lineSegments).SetArgDisplayNames("4 vertices");
        
        var polygon = new Random(12345).GenerateRandomVertex2(10);
        lineSegments = [
            new(0, 1), new(1, 2), new(2, 3), 
            new(3, 4), new(4, 5), new(5, 6), 
            new(6, 7), new(7, 8), new(8, 9),
            new (9, 0)
        ];
        yield return new TestCaseData(polygon).Returns(lineSegments).SetArgDisplayNames("10 vertices");
    }

    [Test]
    public void AddLineSegments_WhenNoValidSegments_ShouldThrowArgumentException()
    {
        var graph = new PlanarStraightLineGraph();
        Assert.That(() => graph.AddLineSegments(), Throws.ArgumentException);
        Assert.That(() => graph.AddLineSegments(new Vertex2(1, 2)), Throws.ArgumentException);
    }
    
    [Test]
    public void AddClosedLineSegments_WhenNoValidSegments_ShouldThrowArgumentException()
    {
        var graph = new PlanarStraightLineGraph();
        Assert.That(() => graph.AddClosedLineSegments(), Throws.ArgumentException);
        Assert.That(() => graph.AddClosedLineSegments(new Vertex2(1, 2)), Throws.ArgumentException);
    }
    
    [TestCaseSource(nameof(OpenPolygonSets))]
    public List<LineElement> AddLineSegments_WhenGraphIsEmpty_ShouldCreateSinglePolygonGraph(Vertex2[] polygon)
    {
        var graph = new PlanarStraightLineGraph();
        graph.AddLineSegments(polygon);
        Assert.That(graph.Vertices, Is.EqualTo(polygon));
        Assert.That(graph.Segments, Has.Count.EqualTo(polygon.Length - 1));
        return graph.Segments.ToList();
    }
    
    [TestCaseSource(nameof(ClosedPolygonSets))]
    public List<LineElement> AddClosedLineSegments_WhenGraphIsEmpty_ShouldCreateSinglePolygonGraph(Vertex2[] polygon)
    {
        var graph = new PlanarStraightLineGraph();
        graph.AddClosedLineSegments(polygon);
        Assert.That(graph.Vertices, Is.EqualTo(polygon));
        Assert.That(graph.Segments, Has.Count.EqualTo(polygon.Length));
        return graph.Segments.ToList();
    }

    [Test]
    public void AddLineSegments_WhenUsedThreeTimes_ShouldCreateThreeDisjunctSetOfPolygons()
    {
        var random = new Random(12345);
        Vertex2[] vertices1 = random.GenerateRandomVertex2(10);
        Vertex2[] vertices2 = random.GenerateRandomVertex2(5);
        Vertex2[] vertices3 = random.GenerateRandomVertex2(5);
        LineElement[] expectedSegments = [
            // Polygon1
            new( 0,  1), new( 1, 2),  new( 2,  3), new( 3,  4), new( 4,  5), 
            new( 5,  6), new( 6,  7), new( 7, 8),  new( 8,  9),
            
            // Polygon2
            new(10, 11), new(11, 12), new(12, 13), new(13, 14),
            
            // Polygon3
            new(15, 16), new(16, 17), new(17, 18), new(18, 19)
        ]; 
        var graph = new PlanarStraightLineGraph();
        Assume.That(graph.Vertices, Has.Count.EqualTo(0));
        Assume.That(graph.Segments, Has.Count.EqualTo(0));
        
        graph.AddLineSegments(vertices1);
        graph.AddLineSegments(vertices2);
        graph.AddLineSegments(vertices3);
        Assert.That(graph.Vertices, Is.EqualTo((Vertex2[]) [..vertices1, ..vertices2, ..vertices3]));
        Assert.That(graph.Segments, Is.EquivalentTo(expectedSegments));
    }
    
    [Test]
    public void AddClosedLineSegments_WhenUsedThreeTimes_ShouldCreateThreeDisjunctSetOfPolygons()
    {
        var random = new Random(12345);
        Vertex2[] vertices1 = random.GenerateRandomVertex2(10);
        Vertex2[] vertices2 = random.GenerateRandomVertex2(5);
        Vertex2[] vertices3 = random.GenerateRandomVertex2(5);
        LineElement[] expectedSegments = [
            // Polygon1
            new( 0,  1), new( 1, 2),  new( 2,  3), new( 3,  4), new( 4,  5), 
            new( 5,  6), new( 6,  7), new( 7, 8),  new( 8,  9), new(9, 0),
            
            // Polygon2
            new(10, 11), new(11, 12), new(12, 13), new(13, 14), new(14, 10),
            
            // Polygon3
            new(15, 16), new(16, 17), new(17, 18), new(18, 19), new(19, 15)
        ]; 
        var graph = new PlanarStraightLineGraph();
        Assume.That(graph.Vertices, Has.Count.EqualTo(0));
        Assume.That(graph.Segments, Has.Count.EqualTo(0));
        
        graph.AddClosedLineSegments(vertices1);
        graph.AddClosedLineSegments(vertices2);
        graph.AddClosedLineSegments(vertices3);
        Assert.That(graph.Vertices, Is.EqualTo((Vertex2[]) [..vertices1, ..vertices2, ..vertices3]));
        Assert.That(graph.Segments, Is.EquivalentTo(expectedSegments));
    }
    
    [Test]
    public void AddLineSegment_WhenUsedThreeTimes_ShouldCreateThreeDisjunctSetOfPolygons()
    {
        var random = new Random(12345);
        Vertex2[] vertices1 = random.GenerateRandomVertex2(2);
        Vertex2[] vertices2 = random.GenerateRandomVertex2(2);
        Vertex2[] vertices3 = random.GenerateRandomVertex2(2);
        LineElement[] expectedSegments = [
            // Segment1      Segment 2      Segment 3
            new( 0,  1), new(2, 3), new(4, 5)
        ]; 
        var graph = new PlanarStraightLineGraph();
        Assume.That(graph.Vertices, Has.Count.EqualTo(0));
        Assume.That(graph.Segments, Has.Count.EqualTo(0));
        
        graph.AddLineSegment(vertices1[0], vertices1[1]);
        graph.AddLineSegment(vertices2[0], vertices2[1]);
        graph.AddLineSegment(vertices3[0], vertices3[1]);
        Assert.That(graph.Vertices, Is.EqualTo((Vertex2[]) [..vertices1, ..vertices2, ..vertices3]));
        Assert.That(graph.Segments, Is.EquivalentTo(expectedSegments));
    }

    [TestCase(0.25f)]
    [TestCase(0.5f)]
    [TestCase(1f)]
    public void SplitLineSegment_WhenSecondLineIsSelected_ShouldRemoveItAndAddTwoConnectedSegments(float alpha)
    {
        var vertices = new Vertex2[] { new(0, 0), new(1, 0), new(1, 1), new(0, 1) };
        var edgesBefore = new LineElement[] { new(0, 1), new(1, 2), new(2, 3), new(3, 0) };
        var edgesAfter = new LineElement[] { new(0, 1), new(1, 4), new(4, 2), new(2, 3), new(3, 0) };
        var addedVertex = new Vertex2(1, alpha);
        
        var graph = new PlanarStraightLineGraph();
        graph.AddClosedLineSegments(vertices);
        Assume.That(graph.Vertices, Is.EquivalentTo(vertices));
        Assume.That(graph.Segments, Is.EquivalentTo(edgesBefore));
        
        graph.SplitLineSegment(1, alpha);
        Assert.That(graph.Segments, Is.EquivalentTo(edgesAfter));
        Assert.That(graph.Vertices, Is.EquivalentTo((Vertex2[]) [..vertices, addedVertex]));
    }
}