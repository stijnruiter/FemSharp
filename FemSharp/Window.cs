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

[StructLayout(LayoutKind.Sequential)]
public record struct LineElement(uint I, uint J)
{
    public static int Count => 2;
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
        _interiorElementBuffer = new ArrayBuffer<TriangularElement>(BufferTarget.ElementArrayBuffer, _interiorElement);
        _boundaryElementBuffer = new ArrayBuffer<LineElement>(BufferTarget.ElementArrayBuffer, _boundaryElement);
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
        _interiorElementBuffer.Bind();
        _shader.SetColor("drawColor", Color4.Red);
        GL.DrawElements(PrimitiveType.Triangles, _interiorElement.Length * TriangularElement.Count, DrawElementsType.UnsignedInt, 0);
        _shader.SetColor("drawColor", Color4.White);
        GL.DrawElements(PrimitiveType.LineLoop, _interiorElement.Length * TriangularElement.Count, DrawElementsType.UnsignedInt, 0);

        _boundaryElementBuffer.Bind();
        _shader.SetColor("drawColor", Color4.Green);
        GL.DrawElements(PrimitiveType.Lines, _boundaryElement.Length * LineElement.Count, DrawElementsType.UnsignedInt, 0);

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
        _interiorElementBuffer.Dispose();
        _boundaryElementBuffer.Dispose();
        _dataBuffer.Dispose();
    }

    private readonly Shader _shader;
    private readonly VertexArray _vertexArray;
    private readonly ArrayBuffer<Vertex> _dataBuffer;
    private readonly ArrayBuffer<TriangularElement> _interiorElementBuffer;
    private readonly ArrayBuffer<LineElement> _boundaryElementBuffer;

    private readonly TriangularElement[] _interiorElement = [new(0, 1, 3), new(1, 2, 3)];
    private readonly LineElement[] _boundaryElement = [new(0, 1), new(1, 2), new(2, 3), new(3, 0)];
    private readonly Vertex[] _vertices = [new(0.5f, 0.5f, 0.0f), new(0.5f, -0.5f, 0.0f), new(-0.5f, -0.5f, 0.0f), new(-0.5f, 0.5f, 0.0f)];
}

