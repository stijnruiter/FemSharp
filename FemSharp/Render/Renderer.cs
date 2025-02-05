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

    public void DrawElements(Color4 color, ArrayBuffer<TriangularElement> elements)
    {
        SolidColorShader.Use();
        SolidColorShader.SetColor("drawColor", color);
        elements.Bind();
        GL.DrawElements(PrimitiveType.Triangles, elements.Length * 3, DrawElementsType.UnsignedInt, 0);
    }

    public void DrawLines(Color4 color, ArrayBuffer<LineElement> elements)
    {
        SolidColorShader.Use();
        SolidColorShader.SetColor("drawColor", color);
        elements.Bind();
        GL.DrawElements(PrimitiveType.Lines, elements.Length * 2, DrawElementsType.UnsignedInt, 0);
    }

    public void DrawLinesClosed(Color4 color, ArrayBuffer<LineElement> elements)
    {
        SolidColorShader.Use();
        SolidColorShader.SetColor("drawColor", color);
        elements.Bind();
        GL.DrawElements(PrimitiveType.LineLoop, elements.Length * 2, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        SolidColorShader.Dispose();
    }
}