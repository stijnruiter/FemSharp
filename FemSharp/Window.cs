using FemSharp.Render;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FemSharp;

internal class Window : GameWindow
{
    // TODO: Z-index
    public readonly List<IDrawableObject> DrawableObjects = [];

    public Window(int width, int height, string title)
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        UpdateFrequency = 45;
        ClientSize = (width, height);
        Title = title;
        _renderer = new Renderer();
        _renderer.ClearColor(new Color4(0.2f, 0.3f, 0.3f, 1.0f));
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        var model = Matrix4.CreateRotationX(-0.5f * MathF.PI);
        Vector3 position = (new Vector4(2.5f * MathF.Cos(_angle), 2.5f * MathF.Sin(_angle), 2.5f, 1.0f) * model).Xyz;
        var view = Matrix4.LookAt(position, Vector3.Zero, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)Size.X / Size.Y, 0.1f, 100.0f);
        _renderer.SetCamera(model, view, projection);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        if (KeyboardState.IsKeyDown(Keys.Left))
        {
            _angle -= _rotationSpeed * (float)e.Time;
            UpdateCamera();
        }
        else if (KeyboardState.IsKeyDown(Keys.Right))
        {
            _angle += _rotationSpeed * (float)e.Time;
            UpdateCamera();
        }

        foreach (var drawable in DrawableObjects)
        {
            drawable.Update();
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
    private float _angle = -0.8f; // 0.25f * MathF.PI;
    private const float _rotationSpeed = 1.0f;
}

