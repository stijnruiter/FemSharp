using FemSharp.Render;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FemSharp.Scenes;

internal class CircularScene : IScene
{
    public Keys ActivateKey { get; }
    public Mesh2D Mesh { get; }
    private DrawableMesh2D _drawableMesh { get; }
    private Axis _axis { get; }

    public CircularScene(Keys activateKey, float cx, float cy, float radius, uint nTriangles)
    {
        ActivateKey = activateKey;
        Mesh = MeshGenerator.NaiveCircle(cx, cy, radius, nTriangles);
        _drawableMesh = new DrawableMesh2D(Mesh);
        _axis = new Axis();
    }

    public void Dispose()
    {
        _drawableMesh.Dispose();
        _axis.Dispose();
    }

    public void Update()
    {
    }

    public void Activate(Window window)
    {
        window.Renderer.DisableDepthTest();
        window.ResetCameraPosition(new Vector3(0, 0, 2.5f), Vector3.UnitY);
    }

    public void Draw(Renderer renderer)
    {
        _drawableMesh.Draw(renderer);
        _axis.Draw(renderer);
    }
}
