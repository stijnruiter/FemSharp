using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NumericalMath.Geometry.Structures;
using NumericalMath.Tests.ExtensionHelpers;

namespace NumericalMath.Tests.Geometry.Structures;

[TestFixture]
public class Vertex2Tests
{
    [Test]
    public void Constructor_WhenUsingDefaultArguments_ShouldHaveOriginVertex2()
    {
        var vertex2 = new Vertex2();
        Assert.That(vertex2.X, Is.EqualTo(0).Within(1e-8f));
        Assert.That(vertex2.Y, Is.EqualTo(0).Within(1e-8f));
    }

    [Test]
    public void Constructor_WhenUsingValuedArguments_ShouldHaveValuedProperties()
    {
        var vertex2 = new Vertex2(1, 2);
        Assert.That(vertex2.X, Is.EqualTo(1).Within(1e-8f));
        Assert.That(vertex2.Y, Is.EqualTo(2).Within(1e-8f));
    }

    [Test]
    public void Vertex_WhenSizeOf_ShouldHaveAlignedByteSize()
    {
        Assert.That(Unsafe.SizeOf<Vertex2>(), Is.EqualTo(8));
    }

    [Test]
    public void ToString_WhenCalled_ShouldHaveTheCorrectOrderIndices()
    {
        var vertex2 = new Vertex2(1, 2);
        Assert.That(vertex2.ToString(), Does.Contain($"{1f}, {2f}"));
    }

    [Test]
    public void Sum_WhenTwoVertices_ShouldReturnLinearAlgebraVectorSum()
    {
        var vertex1 = new Vertex2(1, 2);
        var vertex2 = new Vertex2(3, 5);
        var zeroVertex = new Vertex2(0, 0);
        Assert.That(vertex1 + vertex2, Is.EqualTo(new Vertex2(4, 7)));
        Assert.That(vertex1 + zeroVertex, Is.EqualTo(vertex1));
    }

    [Test]
    public void Subtract_WhenTwoVertices_ShouldReturnLinearAlgebraVectorSubtract()
    {
        var vertex1 = new Vertex2(1, 2);
        var vertex2 = new Vertex2(3, 5);
        var zeroVertex = new Vertex2(0, 0);
        Assert.That(vertex1 - vertex2, Is.EqualTo(new Vertex2(-2, -3)));
        Assert.That(vertex1 - zeroVertex, Is.EqualTo(vertex1));
    }

    [Test]
    public void ScalarProduct_WhenComputed_ShouldReturnLinearAlgebraScalarVectorProduct()
    {
        Assert.That(new Vertex2(1, 2) * 2, Is.EqualTo(new Vertex2(2, 4)));
        Assert.That(2 * new Vertex2(1, 2), Is.EqualTo(new Vertex2(2, 4)));

        Assert.That(new Vertex2(1, 2) * -2, Is.EqualTo(new Vertex2(-2, -4)));
        Assert.That(-2 * new Vertex2(1, 2), Is.EqualTo(new Vertex2(-2, -4)));

        Assert.That(new Vertex2(0, 0) * -10, Is.EqualTo(new Vertex2(0, 0)));
        Assert.That(-2 * new Vertex2(0, 0), Is.EqualTo(new Vertex2(0, 0)));
    }

    private static IEnumerable<TriangleTestSet> TriangleSets()
    {
        yield return new TriangleTestSet("Isosceles right triangle")
        {
            Triangle = new Triangle(new Vertex2(5,5), new Vertex2(6,5), new Vertex2(5,6)),
            Area = 0.5f,
            Angles = [0.25f * MathF.PI, 0.5f * MathF.PI, 0.25f * MathF.PI],
            Circumcenter = new Vertex2(5.5f, 5.5f)
        };

        yield return new TriangleTestSet
        {
            Triangle = new Triangle(new Vertex2(2, -3), new Vertex2(4, -1), new Vertex2(3, 2)),
            Area = 4,
            Angles = [MathF.Acos(7 / MathF.Sqrt(65)), MathF.Acos(3 / MathF.Sqrt(13)), MathF.Acos(-1 / MathF.Sqrt(5))],
            Circumcenter = new Vertex2(1.25f, -0.25f)
        };
        
        yield return new TriangleTestSet
        {
            Triangle = new Triangle(new Vertex2(3, 0), new Vertex2(0, 3), new Vertex2(6, 1)),
            Area = 6,
            Angles = [MathF.Acos(0.8f), MathF.Acos(-1 / MathF.Sqrt(5)), MathF.Acos(2 / MathF.Sqrt(5))],
            Circumcenter = new Vertex2(3.5f, 3.5f)
        }; 
    }

    public class TriangleTestSet(string? displayName = null)
    {
        public Triangle Triangle { get; init; }
        public float Area { get; init; }
        public float[] Angles { get; init; } = [];
        public Vertex2 Circumcenter { get; init; }

        public override string ToString() => displayName ?? Triangle.ToString();
    }
    

    [TestCaseSource(nameof(TriangleSets))]
    public void GetAngles_WhenTriangle_ShouldReturnThreeAnglesSumToPi(TriangleTestSet testSet)
    {
        Assume.That(testSet.Angles.Sum(), Is.EqualTo(MathF.PI).Within( Constants.DefaultFloatTolerance));

        var angles = testSet.Triangle.GetAngles().ToFloats();
        Assert.That(angles.Sum(), Is.EqualTo(MathF.PI).Within( Constants.DefaultFloatTolerance));
        Assert.That(testSet.Triangle.GetAngles().ToFloats(), Is.EquivalentTo(testSet.Angles).Using<float>((f1, f2) => MathF.Abs(f1 - f2) <  Constants.DefaultFloatTolerance));
    }

    [TestCaseSource(nameof(TriangleSets))]
    public void GetSmallestAngle_WhenTriangle_ShouldComputeSmallestTriangle(TriangleTestSet testSet)
    {
        Assume.That(testSet.Angles.Sum(), Is.EqualTo(MathF.PI).Within( Constants.DefaultFloatTolerance));
        Assert.That(testSet.Triangle.GetSmallestAngle(), Is.EqualTo(testSet.Angles.Min()).Within(Constants.DefaultFloatTolerance));
    }
    
    [TestCaseSource(nameof(TriangleSets))]
    public void Circumcenter_WhenTriangle_ShouldComputeCircumcenter(TriangleTestSet testSet)
    {
        Assert.That(testSet.Triangle.Circumcenter(), Is.EqualTo(testSet.Circumcenter));
    }
    
}

[TestFixture]
public class Vertex3Tests
{
    [Test]
    public void Constructor_WhenUsingDefaultArguments_ShouldHaveOriginVertex2()
    {
        var vertex3 = new Vertex3();
        Assert.That(vertex3.X, Is.EqualTo(0));
        Assert.That(vertex3.Y, Is.EqualTo(0));
        Assert.That(vertex3.Z, Is.EqualTo(0));
    }

    [Test]
    public void Constructor_WhenUsingValuedArguments_ShouldHaveValuedProperties()
    {
        var vertex3 = new Vertex3(1, 2, 3);
        Assert.That(vertex3.X, Is.EqualTo(1));
        Assert.That(vertex3.Y, Is.EqualTo(2));
        Assert.That(vertex3.Z, Is.EqualTo(3));
    }

    [Test]
    public void Vertex_WhenSizeOf_ShouldHaveAlignedByteSize()
    {
        Assert.That(Unsafe.SizeOf<Vertex3>(), Is.EqualTo(12));
    }
    
    [Test]
    public void ToString_WhenCalled_ShouldHaveTheCorrectOrderIndices()
    {
        var vertex3 = new Vertex3(1, 2, 3);
        Assert.That(vertex3.ToString(), Does.Contain($"{1f}, {2f}, {3f}"));
    }
}