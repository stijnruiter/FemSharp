using FemSharp.Render;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FemSharp;

internal class Window : GameWindow
{
    public readonly List<IScene> Scenes = [];
    private IScene? _currentScene = null;

    public Window(int width, int height, string title)
        : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        UpdateFrequency = 45;
        ClientSize = (width, height);
        Title = title;
        Renderer = new Renderer();
        Renderer.ClearColor(new Color4(0.2f, 0.3f, 0.3f, 1.0f));
        ResetCameraPosition(Vector3.One * 2.5f, Vector3.UnitY);
    }

    private void UpdateCamera()
    {
        // TODO: Move this to its own class
        var model = Matrix4.CreateRotationZ(-_angle);
        var view = Matrix4.LookAt(_initialCameraPosition, Vector3.Zero, _up);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)Size.X / Size.Y, 0.1f, 100.0f);
        Renderer.SetCamera(model, view, projection);
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

        foreach(var scene in Scenes)
        {
            if (KeyboardState.IsKeyDown(scene.ActivateKey))
            {
                ActivateScene(scene);
                return;
            }
        }
    }

    public void ActivateScene(IScene scene)
    {
        scene?.Activate(this);
        _currentScene = scene;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        Renderer.Clear();
        _currentScene?.Draw(Renderer);
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
        Renderer.Dispose();
    }

    public void ResetCameraPosition(Vector3 position, Vector3 up)
    {
        _up = up;
        _initialCameraPosition = position;
        _angle = 0f;
        UpdateCamera();
    }

    public readonly Renderer Renderer;

    private Vector3 _up;
    private Vector3 _initialCameraPosition;
    private float _angle = 0f;
    private const float _rotationSpeed = 1.0f;
}

