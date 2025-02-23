using FemSharp.Render;
using FemSharp.Simulation;
using NumericalMath.Geometry.Structures;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FemSharp.Scenes;

internal class LaplaceScene : IScene
{
    public Keys ActivateKey { get; }
    public Mesh2D Mesh { get; }
    public DrawableMesh2D Drawable { get; }
    private Axis _axis;
    public Abstract2DFemProblem Simulation { get; }

    public LaplaceScene(Keys key, Rectangle bounds, int nx, int ny)
    {
        ActivateKey = key;
        Mesh = MeshGenerator.NaiveRectangle(bounds, nx, ny);
        Drawable = new DrawableMesh2D(Mesh);
        Simulation = new LaplaceFem(bounds, Mesh);
        Task.Run(Simulation.Solve);
        _axis = new Axis();
    }

    public void Update()
    {
    }

    public void Draw(Renderer renderer)
    {
        Drawable.Draw(renderer);
        _axis.Draw(renderer);
    }

    public void Dispose()
    {
        Drawable.Dispose();
        _axis.Dispose();
    }

    public void Activate(Window window)
    {
        window.ResetCameraPosition(new Vector3(2.5f, -2.5f, 2.5f), Vector3.UnitZ);
        window.Renderer.EnableDepthTest();
        Drawable.UpdateMeshData();
    }
}
