namespace NumericalMath.Geometry.Structures;

public class Mesh3D(Vertex3[] vertices, TriangleElement[] faces)
{
    public Vertex3[] Vertices { get; } = vertices;
    public TriangleElement[] Faces { get; } = faces;
}
