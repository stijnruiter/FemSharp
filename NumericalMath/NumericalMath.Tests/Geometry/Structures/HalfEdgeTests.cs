using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.Geometry.Structures;

[TestFixture]
public class HalfEdgeTests
{
    [Test]
    public void Constructor_WhenUsingDefaultArguments_ShouldBeNegative()
    {
        var halfEdge = new HalfEdge();
        var halfEdgeDefault = new HalfEdge(-1, -1, -1, -1, -1, -1);
        Assert.That(halfEdge, Is.EqualTo(halfEdgeDefault));
    }
    
    [Test]
    public void Constructor_WhenUsingValuedArguments_ShouldHaveIdenticalValuedProperties()
    {
        var halfEdge = new HalfEdge(4, 5, 6, 7, 8, 9);
        
        Assert.That(halfEdge.V1, Is.EqualTo(4));
        Assert.That(halfEdge.V2, Is.EqualTo(5));
        Assert.That(halfEdge.PrevEdge, Is.EqualTo(6));
        Assert.That(halfEdge.NextEdge, Is.EqualTo(7));
        Assert.That(halfEdge.TwinEdge, Is.EqualTo(8));
        Assert.That(halfEdge.ElementIndex, Is.EqualTo(9));
        
        Assert.That(halfEdge.ToString(), Does.Contain("(4, 5)"));
    }
}