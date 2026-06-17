using System.Runtime.CompilerServices;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.Geometry.Structures;

[TestFixture]
public class SimplexElementsTests
{
    [Test]
    public void LineElementConstructor_WhenUsingDefaultArguments_ShouldHaveEmptyElement()
    {
        var element = new LineElement();
        Assert.That(element.I, Is.EqualTo(0));
        Assert.That(element.J, Is.EqualTo(0));
    }

    [Test]
    public void LineElementConstructor_WhenUsingValuedArguments_ShouldHaveValuedProperties()
    {
        var element = new LineElement(5, 6);
        Assert.That(element.I, Is.EqualTo(5));
        Assert.That(element.J, Is.EqualTo(6));
    }

    [Test]
    public void LineElement_WhenCreated_ShouldHaveAlignedByteSize()
    {
        Assert.That(Unsafe.SizeOf<LineElement>(), Is.EqualTo(8));
    }
    
    [Test]
    public void LineElementToString_WhenCalled_ShouldHaveTheCorrectOrderIndices()
    {
        var element = new LineElement(5, 6);
        Assert.That(element.ToString(), Does.Contain("5, 6"));
    }

    [Test]
    public void TriangleElementConstructor_WhenUsingDefaultArguments_ShouldHaveEmptyElement()
    {
        var element = new TriangleElement();
        Assert.That(element.I, Is.EqualTo(0));
        Assert.That(element.J, Is.EqualTo(0));
        Assert.That(element.K, Is.EqualTo(0));
    }

    [Test]
    public void TriangleElementConstructor_WhenUsingValuedArguments_ShouldHaveValuedProperties()
    {
        var element = new TriangleElement(5, 6, 7);
        Assert.That(element.I, Is.EqualTo(5));
        Assert.That(element.J, Is.EqualTo(6));
        Assert.That(element.K, Is.EqualTo(7));
    }
    
    [Test]
    public void TriangleElement_WhenCreated_ShouldHaveAlignedByteSize()
    {
        Assert.That(Unsafe.SizeOf<TriangleElement>(), Is.EqualTo(12));
    }

    [Test]
    public void TriangleElementToString_WhenCalled_ShouldHaveTheCorrectOrderIndices()
    {
        var element = new TriangleElement(5, 6, 7);
        Assert.That(element.ToString(), Does.Contain("5, 6, 7"));
    }

    [Test]
    public void TetrahedronElementConstructor_WhenUsingDefaultArguments_ShouldHaveEmptyElement()
    {
        var element = new TetrahedronElement();
        Assert.That(element.I, Is.EqualTo(0));
        Assert.That(element.J, Is.EqualTo(0));
        Assert.That(element.K, Is.EqualTo(0));
        Assert.That(element.L, Is.EqualTo(0));   
    }
    
    [Test]
    public void TetrahedronElementConstructor_WhenUsingValuedArguments_ShouldHaveValuedProperties()
    {
        var element = new TetrahedronElement(5, 6, 7, 8);
        Assert.That(element.I, Is.EqualTo(5));
        Assert.That(element.J, Is.EqualTo(6));
        Assert.That(element.K, Is.EqualTo(7));
        Assert.That(element.L, Is.EqualTo(8));
    }
    
    [Test]
    public void TetrahedronElement_WhenCreated_ShouldHaveAlignedByteSize()
    {
        Assert.That(Unsafe.SizeOf<TetrahedronElement>(), Is.EqualTo(16));
    }

    [Test]
    public void TetrahedronElementToString_WhenCalled_ShouldHaveTheCorrectOrderIndices()
    {
        var element = new TetrahedronElement(5, 6, 7, 8);
        Assert.That(element.ToString(), Does.Contain("5, 6, 7, 8"));
    }
}