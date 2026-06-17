using System.Collections.Generic;
using System.Linq;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.Geometry.Structures;

[TestFixture]
public class RectangleTests
{
    [Test]
    public void Constructor_WhenUsingDefaultArguments_ShouldBeDefault()
    {
        var rect = new Rectangle();
        Assert.Multiple(() =>
        {
            Assert.That(rect.Left, Is.EqualTo(0f));
            Assert.That(rect.Right, Is.EqualTo(0f));
            Assert.That(rect.Bottom, Is.EqualTo(0f));
            Assert.That(rect.Top, Is.EqualTo(0f));
            Assert.That(rect.Width, Is.EqualTo(0f));
            Assert.That(rect.Height, Is.EqualTo(0f));
        });
    }

    [TestCase(5, 10, -10, 5, 5, 15, 
        TestName = "Constructor_WhenAssigningPositiveSize_ShouldHavePositiveComputedSize")]
    [TestCase(5, -10, 5, -5, -15, -10, 
        TestName = "Constructor_WhenAssigningNegativeSize_ShouldHaveNegativeComputedSize")]
    public void Constructor_WhenUsingValuedArguments_ShouldHaveValuedProperties
        (float left, float right, float bottom, float top, float width, float height)
    {
        var rect = new Rectangle(left, right, bottom, top);
        Assert.Multiple(() =>
        {
            Assert.That(rect.Left, Is.EqualTo(left));
            Assert.That(rect.Right, Is.EqualTo(right));
            Assert.That(rect.Bottom, Is.EqualTo(bottom));
            Assert.That(rect.Top, Is.EqualTo(top));
            Assert.That(rect.Width, Is.EqualTo(width));
            Assert.That(rect.Height, Is.EqualTo(height));
        });
    }

    [Test]
    public void ToVertices_WhenUsed_ShouldReturnFourCornerCounterClockwiseOrder()
    {
        var rect = new Rectangle(0, 1, 2, 3);
        var vertices = rect.ToVertices().ToArray();
        Assert.That(vertices, Has.Length.EqualTo(4));
        Assert.That(vertices[0], Is.EqualTo(new Vertex2(0, 2)));
        Assert.That(vertices[1], Is.EqualTo(new Vertex2(1, 2)));
        Assert.That(vertices[2], Is.EqualTo(new Vertex2(1, 3)));
        Assert.That(vertices[3], Is.EqualTo(new Vertex2(0, 3)));
    }

    [TestCase(5, 0f)]
    [TestCase(25, 0f)]
    [TestCase(5, 5f)]
    [TestCase(25, 5f)]
    public void BoundingBox_WhenDilating_ShouldReturnTheDilatedRectangle(int nPoints, float dilate)
    {
        var x = GetFloats(nPoints).ToArray();
        var y = GetFloats(nPoints).ToArray();
        var expectedRectangle = new Rectangle(x.Min() - dilate, x.Max() + dilate, y.Min() - dilate, y.Max() + dilate);

        var vertices = x.Zip(y).Select(pair => new Vertex2(pair.First, pair.Second)).ToArray();
        var boundingBox = Rectangle.BoundingBox(vertices, dilate);
        
        Assert.That(boundingBox.Left, Is.EqualTo(expectedRectangle.Left).Within(1e-5));
        Assert.That(boundingBox.Right, Is.EqualTo(expectedRectangle.Right).Within(1e-5));
        Assert.That(boundingBox.Bottom, Is.EqualTo(expectedRectangle.Bottom).Within(1e-5));
        Assert.That(boundingBox.Top, Is.EqualTo(expectedRectangle.Top).Within(1e-5));
    }
    
    private static IEnumerable<float> GetFloats(int nPoints)
    {
        for (var i = 0; i < nPoints; i++)
        {
            yield return TestContext.CurrentContext.Random.NextFloat(-5f, 5f);
        }
    }
}