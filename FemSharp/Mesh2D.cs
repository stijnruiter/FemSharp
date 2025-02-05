using FemSharp.Render;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FemSharp;


[StructLayout(LayoutKind.Sequential)]
internal record struct Vertex(float X, float Y, float Z);

[StructLayout(LayoutKind.Sequential)]
internal record struct TriangularElement(uint I, uint J, uint K);

[StructLayout(LayoutKind.Sequential)]
internal record struct LineElement(uint I, uint J);

internal class Mesh2D : IDrawableObject
{
    public Vertex[] Vertices { get; }
    public TriangularElement[] InteriorElements { get; }
    public LineElement[] BoundaryElements { get; }

    public Mesh2D(Vertex[] vertices, TriangularElement[] interior, LineElement[] boundary)
    {
        Vertices = vertices;
        InteriorElements = interior;
        BoundaryElements = boundary;

        _vertexArray = new VertexArray();
        _dataBuffer = new ArrayBuffer<Vertex>(BufferTarget.ArrayBuffer, Vertices);
        _interiorElementBuffer = new ArrayBuffer<TriangularElement>(BufferTarget.ElementArrayBuffer, InteriorElements);
        _boundaryElementBuffer = new ArrayBuffer<LineElement>(BufferTarget.ElementArrayBuffer, BoundaryElements);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
        GL.EnableVertexAttribArray(0);
    }

    public void Dispose()
    {
        _vertexArray.Dispose();
        _dataBuffer.Dispose();
        _interiorElementBuffer.Dispose();
        _boundaryElementBuffer.Dispose();
    }

    public void Draw(Renderer renderer)
    {
        renderer.DrawElements(Color4.Red, _interiorElementBuffer);
        renderer.DrawLinesClosed(Color4.White, _interiorElementBuffer);
        renderer.DrawLines(Color4.Green, _boundaryElementBuffer);
    }

    public readonly VertexArray _vertexArray;
    public readonly ArrayBuffer<Vertex> _dataBuffer;
    public readonly ArrayBuffer<TriangularElement> _interiorElementBuffer;
    public readonly ArrayBuffer<LineElement> _boundaryElementBuffer;
}