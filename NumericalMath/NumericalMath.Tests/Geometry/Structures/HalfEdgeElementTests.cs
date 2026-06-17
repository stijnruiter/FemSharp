using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.Geometry.Structures;

[TestFixture]
public class HalfEdgeElementTests
{
    /**
     * Create 3 connected triangles
     *
     *   0----- 2 
     *   | f0/ |  \     
     *   | / f1| f2\
     *   1-----3----4
     */
    private static HalfEdge[] CreateThreeConnectedTriangles()
    {
        return
        [
            new HalfEdge(0, 1, 2, 1, -1, 0),
            new HalfEdge(1, 2, 0, 2, 5, 0),
            new HalfEdge(2, 0, 1, 0, -1, 0),

            new HalfEdge(1, 3, 5, 4, -1, 1),
            new HalfEdge(3, 2, 3, 5, 8, 1),
            new HalfEdge(2, 1, 4, 3, 1, 1),

            new HalfEdge(3, 4, 8, 7, -1, 2),
            new HalfEdge(4, 2, 6, 8, -1, 2),
            new HalfEdge(2, 3, 7, 6, 4, 2)
        ];
    }

    [Test]
    public void Enumerating_WhenUsingStrongTyped_ShouldReturnTheTriangleEdges()
    {
        var edges = CreateThreeConnectedTriangles();
        Assume.That(edges, Has.Length.EqualTo(9));
        var expectedElement1 = (HalfEdge[])[edges[0], edges[1], edges[2]];
        var expectedElement2 = (HalfEdge[])[edges[3], edges[4], edges[5]];
        var expectedElement3 = (HalfEdge[])[edges[6], edges[7], edges[8]];
        
        Assert.That(new HalfEdgeElement(edges, 0).ToArray(), Is.EquivalentTo(expectedElement1));
        Assert.That(new HalfEdgeElement(edges, 1).ToArray(), Is.EquivalentTo(expectedElement1));
        Assert.That(new HalfEdgeElement(edges, 2).ToArray(), Is.EquivalentTo(expectedElement1));
        
        Assert.That(new HalfEdgeElement(edges, 3).ToArray(), Is.EquivalentTo(expectedElement2));
        Assert.That(new HalfEdgeElement(edges, 4).ToArray(), Is.EquivalentTo(expectedElement2));
        Assert.That(new HalfEdgeElement(edges, 5).ToArray(), Is.EquivalentTo(expectedElement2));
        
        Assert.That(new HalfEdgeElement(edges, 6).ToArray(), Is.EquivalentTo(expectedElement3));
        Assert.That(new HalfEdgeElement(edges, 7).ToArray(), Is.EquivalentTo(expectedElement3));
        Assert.That(new HalfEdgeElement(edges, 8).ToArray(), Is.EquivalentTo(expectedElement3));
    }

    [Test]
    public void Enumerating_WhenUsingWeakTyped_ShouldReturnTheTriangleEdges()
    {
        var edges = CreateThreeConnectedTriangles();
        Assume.That(edges, Has.Length.EqualTo(9));
        var expectedElement1 = (HalfEdge[])[edges[0], edges[1], edges[2]];
        var expectedElement2 = (HalfEdge[])[edges[3], edges[4], edges[5]];
        var expectedElement3 = (HalfEdge[])[edges[6], edges[7], edges[8]];
        
        Assert.That(AsWeakEnumerable(new HalfEdgeElement(edges, 0)), Is.EquivalentTo(expectedElement1));
        Assert.That(AsWeakEnumerable(new HalfEdgeElement(edges, 1)), Is.EquivalentTo(expectedElement1));
        Assert.That(AsWeakEnumerable(new HalfEdgeElement(edges, 2)), Is.EquivalentTo(expectedElement1));
        
        Assert.That(AsWeakEnumerable(new HalfEdgeElement(edges, 3)), Is.EquivalentTo(expectedElement2));
        Assert.That(AsWeakEnumerable(new HalfEdgeElement(edges, 4)), Is.EquivalentTo(expectedElement2));
        Assert.That(AsWeakEnumerable(new HalfEdgeElement(edges, 5)), Is.EquivalentTo(expectedElement2));
        
        Assert.That(AsWeakEnumerable(new HalfEdgeElement(edges, 6)), Is.EquivalentTo(expectedElement3));
        Assert.That(AsWeakEnumerable(new HalfEdgeElement(edges, 7)), Is.EquivalentTo(expectedElement3));
        Assert.That(AsWeakEnumerable(new HalfEdgeElement(edges, 8)), Is.EquivalentTo(expectedElement3));
    }

    [Test]
    public void EnumeratorReset_WhenCalledBeforeEnumeration_ShouldReturnIdenticalTriangleEdges()
    {
        var edges = CreateThreeConnectedTriangles();
        Assume.That(edges, Has.Length.EqualTo(9));
        var expectedElement2 = (HalfEdge[])[edges[3], edges[4], edges[5]];
        
        var halfEdgeElement = new HalfEdgeElement(edges, 3);
        using var enumerator = halfEdgeElement.GetEnumerator();
        
        List<HalfEdge> edgesFromEnum = [];
        while (enumerator.MoveNext())
        {
            edgesFromEnum.Add(enumerator.Current);
        }
        Assert.That(edgesFromEnum, Is.EquivalentTo(expectedElement2));
        
        enumerator.Reset();
        List<HalfEdge> edgesFromEnum2 = [];
        while (enumerator.MoveNext())
        {
            edgesFromEnum2.Add(enumerator.Current);
        }
        Assert.That(edgesFromEnum2, Is.EquivalentTo(expectedElement2));
    }

    [Test]
    public void Constructor_WhenIndexIsOutOfRange_ShouldThrow()
    {
        var edges = CreateThreeConnectedTriangles();
        Assert.That(() => new HalfEdgeElement(edges, -1), Throws.Exception.TypeOf<IndexOutOfRangeException>());
        Assert.That(() => new HalfEdgeElement(edges, edges.Length), Throws.Exception.TypeOf<IndexOutOfRangeException>());
    }
    
    private static List<object?> AsWeakEnumerable(IEnumerable source) => source.Cast<object?>().ToList();
}