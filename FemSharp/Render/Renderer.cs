using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FemSharp.Render;

internal class Renderer : IDisposable
{
    public Shader SolidColorShader { get; }

    public Renderer()
    {
        SolidColorShader = Shader.Create("shader.vertex.glsl", "shader.fragment.glsl");
    }

    public void ClearColor(Color4 color)
    {
        GL.ClearColor(color);
    }

    public void Clear()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void SetLineWidth(int width)
    {
        GL.LineWidth(width);
    }

    public void DrawElements<T>(Color4 color, ArrayBuffer<T> elements) where T : struct
    {
        SolidColorShader.Use();
        SolidColorShader.SetColor("drawColor", color);
        elements.Bind();
        GL.DrawElements(PrimitiveType.Triangles, elements.Count, DrawElementsType.UnsignedInt, 0);
    }

    public void DrawLines<T>(Color4 color, ArrayBuffer<T> elements) where T : struct
    {
        SolidColorShader.Use();
        SolidColorShader.SetColor("drawColor", color);
        elements.Bind();
        GL.DrawElements(PrimitiveType.Lines, elements.Count, DrawElementsType.UnsignedInt, 0);
    }

    public void DrawLinesClosed<T>(Color4 color, ArrayBuffer<T> elements) where T : struct
    {
        SolidColorShader.Use();
        SolidColorShader.SetColor("drawColor", color);
        elements.Bind();
        GL.DrawElements(PrimitiveType.LineLoop, elements.Count, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        SolidColorShader.Dispose();
    }
}