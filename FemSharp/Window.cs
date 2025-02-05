using FemSharp.Render;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FemSharp;

internal class Window : GameWindow
{
    public readonly List<IDrawableObject> DrawableObjects = [];

    public Window(int width, int height, string title)
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        ClientSize = (width, height);
        Title = title;
        _renderer = new Renderer();
        _renderer.ClearColor(new OpenTK.Mathematics.Color4(0.2f, 0.3f, 0.3f, 1.0f));
        _renderer.SetLineWidth(3);
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

        _renderer.Clear();
        foreach (var drawable in DrawableObjects)
        {
            drawable.Draw(_renderer);
        }
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
        _renderer.Dispose();
    }

    private readonly Renderer _renderer;

}

