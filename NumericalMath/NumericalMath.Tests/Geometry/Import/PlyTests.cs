using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NumericalMath.Geometry;
using NumericalMath.Geometry.Import.Ply;

namespace NumericalMath.Tests.Geometry.Import;

[TestFixture]
public class PlyTests
{
    private static readonly string TestDataDir = 
        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Geometry", "Import", "TestData");
    
    
    [TestCaseSource(typeof(PlyTestData), nameof(PlyTestData.SampleTestData))]
    public void TestPly(PlyTestData testData)
    {
        var path = Path.Combine(TestDataDir, testData.Filename);
        var file = PlyFile.Load(path);
        
        Assert.That(file.Data["vertex"], Has.Count.EqualTo(testData.ExpectedVertices));
        Assert.That(file.Data["face"], Has.Count.EqualTo(testData.ExpectedFaces));
    }

    [TestCaseSource(typeof(PlyTestData), nameof(PlyTestData.SampleTestData))]
    public void TestMeshBuilder(PlyTestData testData)
    {
        var path = Path.Combine(TestDataDir, testData.Filename);
        var mesh = MeshBuilder.FromPly(path);
        Assert.That(mesh.Vertices, Has.Length.EqualTo(testData.ExpectedVertices));
        Assert.That(mesh.Faces, Has.Length.EqualTo(testData.ExpectedFaces));
    }
}

public record PlyTestData(string Filename, int ExpectedVertices, int ExpectedFaces)
{
    public static PlyTestData Apple => new("apple.ply", 867, 1704);
    public static PlyTestData Cube => new("cube.ply", 8, 6);

    public static IEnumerable<PlyTestData> SampleTestData()
    {
        yield return Cube;
        yield return Apple;
    }
    
    public override string ToString() => Filename;
}
