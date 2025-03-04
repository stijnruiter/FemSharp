﻿using NumericalMath.Geometry.Structures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace FemSharp.Render;

internal class DrawableLines : IDrawableObject
{
    public DrawableLines(PlanarStraightLineGraph graph)
    {
        _vertexArray = new VertexArray();
        _dataBuffer = new ArrayBuffer<Vertex3>(BufferTarget.ArrayBuffer, graph.Vertices.Select(v => new Vertex3(v.X, v.Y, 0f)).ToArray());
        _interiorElementEdgesBuffer = new ArrayBuffer<LineElement>(BufferTarget.ElementArrayBuffer, graph.Segments.ToArray());
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex3>(), 0);
        GL.EnableVertexAttribArray(0);
    }

    public void Dispose()
    {
        _vertexArray.Dispose();
        _dataBuffer.Dispose();
        _interiorElementEdgesBuffer.Dispose();
    }
    
    private readonly VertexArray _vertexArray;
    private readonly ArrayBuffer<Vertex3> _dataBuffer;
    private readonly ArrayBuffer<LineElement> _interiorElementEdgesBuffer;
    public void Update() { }

    public void Draw(Renderer renderer)
    {
        _vertexArray.Bind();

        renderer.UseSolidColor(Color4.Black);
        renderer.DrawLines(_interiorElementEdgesBuffer);
    }
}

internal class DrawableMesh2D : IDrawableObject
{
    public DrawableMesh2D(Mesh2D mesh)
    {
        _mesh = mesh;
        _vertexArray = new VertexArray();
        _dataBuffer = new ArrayBuffer<ValuedVertex>(BufferTarget.ArrayBuffer, mesh.Vertices);
        _interiorElementBuffer = new ArrayBuffer<TriangleElement>(BufferTarget.ElementArrayBuffer, mesh.InteriorElements);
        _interiorElementEdgesBuffer = new ArrayBuffer<LineElement>(BufferTarget.ElementArrayBuffer, mesh.InteriorEdges);
        _boundaryElementBuffer = new ArrayBuffer<LineElement>(BufferTarget.ElementArrayBuffer, mesh.BoundaryElements);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<ValuedVertex>(), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, Unsafe.SizeOf<ValuedVertex>(), sizeof(float) * 3);
        GL.EnableVertexAttribArray(1);
    }

    public void Update() { }

    public void UpdateMeshData()
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
        _vertexArray.Bind();

        renderer.EnableLinearAlphaBlend();
        
        renderer.UseScalarColor();
        renderer.DrawElements(_interiorElementBuffer);
        
        renderer.UseSolidColor(ColorElementEdges);
        renderer.DrawLines(_interiorElementEdgesBuffer);
        
        renderer.UseSolidColor(ColorBoundaryEdges);
        renderer.DrawLines(_boundaryElementBuffer);
    }

    public Color4 ColorElements { get; set; } = Color4.Transparent;
    public Color4 ColorElementEdges { get; set; } = new Color4(255, 255, 255, 100);
    public Color4 ColorBoundaryEdges { get; set; } = new Color4(255, 100, 255, 100);

    private readonly Mesh2D _mesh;

    private readonly VertexArray _vertexArray;
    private readonly ArrayBuffer<ValuedVertex> _dataBuffer;
    private readonly ArrayBuffer<TriangleElement> _interiorElementBuffer;
    private readonly ArrayBuffer<LineElement> _interiorElementEdgesBuffer;
    private readonly ArrayBuffer<LineElement> _boundaryElementBuffer;
}

