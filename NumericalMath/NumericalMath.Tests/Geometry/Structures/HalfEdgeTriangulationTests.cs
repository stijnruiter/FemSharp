using System.Linq;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.Geometry.Structures;

[TestFixture]
public class HalfEdgeTriangulationTests
{
    [Test]
    public void Constructor_WhenCapacityDefined_ShouldBeEmptyWithCorrectCapacity()
    {
        var halfEdges = new HalfEdgeTriangulation(5);
        Assert.That(halfEdges.ElementCount, Is.EqualTo(0));
        Assert.That(halfEdges.EdgeCount, Is.EqualTo(0));
        
        Assert.That(halfEdges.ElementCapacity, Is.EqualTo(5));
        Assert.That(halfEdges.EdgeCapacity, Is.EqualTo(15));
    }
    
    [Test]
    public void AddTriangle_WhenAddingTwoTriangles_ShouldReturnSixEdges()
    {
        HalfEdgeTriangulation halfEdges = new HalfEdgeTriangulation(2);
        halfEdges.AddTriangle(0, 1, 2);
        halfEdges.AddTriangle(1, 3, 2);
        Assert.That(halfEdges.ElementCount, Is.EqualTo(2));
        Assert.That(halfEdges.EdgeCount, Is.EqualTo(6));
    
        var expectedEdges = new[]
        {
            new HalfEdge(0, 1, 2, 1, -1, 0),
            new HalfEdge(1, 2, 0, 2, 5, 0),
            new HalfEdge(2, 0, 1, 0, -1, 0),
            
            new HalfEdge(1, 3, 5, 4, -1, 1),
            new HalfEdge(3, 2, 3, 5, -1, 1),
            new HalfEdge(2, 1, 4, 3, 1, 1),
        };
        Assert.That(halfEdges.Edges.ToArray(), Is.EquivalentTo(expectedEdges));
    }

    private static HalfEdgeTriangulation CreateHexagonTriangulation()
    {
        // Create "hexagon" shaped triangulation
        var triangulation = new HalfEdgeTriangulation(6);
        triangulation.AddTriangle(0, 1, 2);
        triangulation.AddTriangle(3, 4, 2);
        triangulation.AddTriangle(2, 1, 3);
        triangulation.AddTriangle(6, 2, 5);
        triangulation.AddTriangle(2, 6, 0);
        triangulation.AddTriangle(5, 2, 4);
        
        Assume.That(triangulation.ElementCount, Is.EqualTo(6));
        Assume.That(triangulation.EdgeCount, Is.EqualTo(18));
        return triangulation;
    }

    [Test]
    public void GetTriangleElements_WhenCalledFromHexagonTriangulation_ShouldReturnSixTriangles()
    {
        var triangulation = CreateHexagonTriangulation();
        var edges = triangulation.Edges.ToArray();
        Assert.That(edges.Length, Is.EqualTo(18));

        TriangleElement[] expectedTriangles =
        [
            new(0, 1, 2),
            new(3, 4, 2),
            new(2, 1, 3),
            new(6, 2, 5),
            new(2, 6, 0),
            new(5, 2, 4)
        ];
        Assert.That(triangulation.GetTriangleElements(), Is.EqualTo(expectedTriangles));
    }

    [Test]
    public void Edges_WhenCalledFromHexagonTriangulation_ShouldReturnSixBoundaryElements()
    {
        var edges = CreateHexagonTriangulation().Edges.ToArray();
        Assert.That(edges.Length, Is.EqualTo(18));

        LineElement[] expectedBoundaries =
        [
            new(0, 1), new(1, 3), new(3, 4), new(4, 5), new(5, 6), new(6, 0)
        ];
        
        Assert.That(edges.Where(IsBoundary).Select(ToLine), Is.EquivalentTo(expectedBoundaries));
    }
    
    [Test]
    public void GetTriangleElements_WhenCalledFromHexagonTriangulation_ShouldReturnSixNeighbourPairs()
    {
        var edges = CreateHexagonTriangulation().Edges.ToArray();
        Assume.That(edges.Length, Is.EqualTo(18));

        LineElement[] neighbourPairs =
        [
            new(0,2), new(2,0),
            new(1,2), new(2,1),
            new(3,2), new(2,3),
            new(4,2), new(2,4),
            new(5,2), new(2,5),
            new(6,2), new(2,6),
        ];
        Assert.That(edges.Where(IsNoBoundary).Select(ToLine), Is.EquivalentTo(neighbourPairs));
        for (var i = 0; i < edges.Length; i++)
        {
            if (IsBoundary(edges[i]))
                continue;
            
            // Check that twin edges point to eachother
            Assert.That(edges[edges[i].TwinEdge].TwinEdge, Is.EqualTo(i));
        }
    }
    
    private static LineElement ToLine(HalfEdge halfEdge) => new(halfEdge.V1, halfEdge.V2);
    private static bool IsBoundary(HalfEdge halfEdge) => halfEdge.TwinEdge == -1;
    private static bool IsNoBoundary(HalfEdge halfEdge) => !IsBoundary(halfEdge);
}