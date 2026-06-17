using System.Linq;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Geometry;

public static class MeshBuilder
{
    public static Mesh3D FromPly(string filename)
    {
        const string vertexDataName = "vertex";
        const string faceDataName = "face";
        var plyFile = Import.Ply.PlyFile.Load(filename);
        var vertexInfo =  plyFile.Header.Elements.Single(p => p.Name == vertexDataName);

        var indexOfX = vertexInfo.Properties.FindIndex(p => p.Name == "x");
        var indexOfY = vertexInfo.Properties.FindIndex(p => p.Name == "y");
        var indexOfZ = vertexInfo.Properties.FindIndex(p => p.Name == "z");

        var vertices = plyFile.Data[vertexDataName].Select(v => 
            new Vertex3((float)v[indexOfX], (float)v[indexOfY], (float)v[indexOfZ])).ToArray();
        var faces = plyFile.Data[faceDataName].Select(f =>
        {
            var list = (int[])f[0];
            return new TriangleElement(list[0], list[1], list[2]);
        }).ToArray();
        return new Mesh3D(vertices, faces);
    }
}