using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FemSharp.Render;

internal class DrawableMesh2D : IDrawableObject
{
    public DrawableMesh2D(Mesh2D mesh)
    {
        _mesh = mesh;
        _vertexArray = new VertexArray();
        _dataBuffer = new ArrayBuffer<Vertex>(BufferTarget.ArrayBuffer, mesh.Vertices);
        _interiorElementBuffer = new ArrayBuffer<TriangularElement>(BufferTarget.ElementArrayBuffer, mesh.InteriorElements);
        _interiorElementEdgesBuffer = new ArrayBuffer<LineElement>(BufferTarget.ElementArrayBuffer, mesh.InteriorEdges);
        _boundaryElementBuffer = new ArrayBuffer<LineElement>(BufferTarget.ElementArrayBuffer, mesh.BoundaryElements);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
        GL.EnableVertexAttribArray(0);
    }

    public void Update()
    {
        _dataBuffer.SetData(_mesh.Vertices);
        _interiorElementEdgesBuffer.SetData(_mesh.InteriorEdges);
        _interiorElementBuffer.SetData(_mesh.InteriorElements);
        _boundaryElementBuffer.SetData(_mesh.BoundaryElements);
    }

    public void Dispose()
    {
        _vertexArray.Dispose();
        _dataBuffer.Dispose();
        _interiorElementBuffer.Dispose();
        _interiorElementEdgesBuffer.Dispose();
        _boundaryElementBuffer.Dispose();
    }

    public void Draw(Renderer renderer)
    {
        renderer.SetLineWidth(5);
        renderer.DrawElements(ColorElements, _interiorElementBuffer);
        renderer.DrawLines(ColorElementEdges, _interiorElementEdgesBuffer);
        renderer.DrawLines(ColorBoundaryEdges, _boundaryElementBuffer);
    }

    public Color4 ColorElements { get; set; } = Color4.BurlyWood;
    public Color4 ColorElementEdges { get; set; } = Color4.Coral;
    public Color4 ColorBoundaryEdges { get; set; } = Color4.Black;

    private readonly Mesh2D _mesh;

    private readonly VertexArray _vertexArray;
    private readonly ArrayBuffer<Vertex> _dataBuffer;
    private readonly ArrayBuffer<TriangularElement> _interiorElementBuffer;
    private readonly ArrayBuffer<LineElement> _interiorElementEdgesBuffer;
    private readonly ArrayBuffer<LineElement> _boundaryElementBuffer;
}

