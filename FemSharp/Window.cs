using FemSharp.Render;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FemSharp;

[StructLayout(LayoutKind.Sequential)]
public record struct Vertex(float X, float Y, float Z)
{
    public static int Count => 3;
    public static VertexAttribPointerType PointerType => VertexAttribPointerType.Float;
}

[StructLayout(LayoutKind.Sequential)]
public record struct TriangularElement(uint I, uint J, uint K)
{
    public static int Count => 3;
}

internal class Window : GameWindow
{
    public Window(int width, int height, string title)
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        ClientSize = (width, height);
        Title = title;

        _shader = Shader.Create("shader.vertex.glsl", "shader.fragment.glsl");
        _shader.Use();

        _vertexArray = new VertexArray();
        _dataBuffer = new ArrayBuffer<Vertex>(BufferTarget.ArrayBuffer, _vertices);
        _indexBuffer = new ArrayBuffer<TriangularElement>(BufferTarget.ElementArrayBuffer, _indices);
        GL.VertexAttribPointer(0, Vertex.Count, Vertex.PointerType, false, Unsafe.SizeOf<Vertex>(), 0);
        GL.EnableVertexAttribArray(0);

        GL.ClearColor(Color4.Black);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.LineWidth(3);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader.Use();
        _vertexArray.Bind();
        _shader.SetColor("drawColor", Color4.Red);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length * TriangularElement.Count, DrawElementsType.UnsignedInt, 0);
        _shader.SetColor("drawColor", Color4.White);
        GL.DrawElements(PrimitiveType.LineLoop, _indices.Length * TriangularElement.Count, DrawElementsType.UnsignedInt, 0);


        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }

    public override void Dispose()
    {
        base.Dispose();
        _shader.Dispose();
        _vertexArray.Dispose();
        _indexBuffer.Dispose();
        _dataBuffer.Dispose();
    }

    private readonly Shader _shader;
    private readonly VertexArray _vertexArray;
    private readonly ArrayBuffer<Vertex> _dataBuffer;
    private readonly ArrayBuffer<TriangularElement> _indexBuffer;

    private readonly TriangularElement[] _indices = [new(0, 1, 3), new(1, 2, 3)];
    private readonly Vertex[] _vertices = [new(0.5f, 0.5f, 0.0f), new(0.5f, -0.5f, 0.0f), new(-0.5f, -0.5f, 0.0f), new(-0.5f, 0.5f, 0.0f)];
}

