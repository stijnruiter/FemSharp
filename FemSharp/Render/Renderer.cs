using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FemSharp.Render;

internal class Renderer : IDisposable
{
    public Shader SolidColorShader { get; }
    public Shader ScalarColorShader { get; }
    public Shader VertexColorShader { get; }

    public Renderer()
    {
        SolidColorShader = Shader.Create("uniform_color.vertex.glsl", "uniform_color.fragment.glsl");
        ScalarColorShader = Shader.Create("scalar_color.vertex.glsl", "scalar_color.fragment.glsl");
        VertexColorShader = Shader.Create("vertex_color.vertex.glsl", "vertex_color.fragment.glsl");
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

    public void EnableLinearAlphaBlend()
    {
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    public void DisableAlphaBlend()
    {
        GL.Disable(EnableCap.Blend);
    }

    public void UseSolidColor(Color4 color)
    {
        SolidColorShader.Use();
        SolidColorShader.SetColor("drawColor", color);
    }

    public void UseScalarColor()
    {
        ScalarColorShader.Use();
    }

    public void UseVertexColor()
    {
        VertexColorShader.Use();
    }

    public void DrawElements(ArrayBuffer<TriangularElement> elements)
    {
        elements.Bind();
        GL.DrawElements(PrimitiveType.Triangles, elements.Length * 3, DrawElementsType.UnsignedInt, 0);
    }

    public void DrawLines(ArrayBuffer<LineElement> elements)
    {
        elements.Bind();
        GL.DrawElements(PrimitiveType.Lines, elements.Length * 2, DrawElementsType.UnsignedInt, 0);
    }

    public void DrawLinesClosed(Color4 color, ArrayBuffer<LineElement> elements)
    {
        elements.Bind();
        GL.DrawElements(PrimitiveType.LineLoop, elements.Length * 2, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        SolidColorShader.Dispose();
    }
}