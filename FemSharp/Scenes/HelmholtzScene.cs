using FemSharp.Render;
using FemSharp.Simulation;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FemSharp.Scenes;

internal class HelmholtzScene : IScene
{
    public Keys ActivateKey { get; }
    public Mesh2D Mesh { get; }
    public DrawableMesh2D _drawableMesh { get; }
    private Axis _axis;
    public Abstract2DFemProblem Simulation { get; }

    public HelmholtzScene(Keys key, Rect bounds, uint nx, uint ny, float k)
    {
        ActivateKey = key;
        Mesh = MeshGenerator.NaiveRectangle(bounds, nx, ny);
        _drawableMesh = new DrawableMesh2D(Mesh);
        _axis = new Axis();
        Simulation = new HelmholtzEquationWithSourceFEM(bounds, Mesh, k);
        Task.Run(Simulation.Solve);
    }

    public void Update()
    {
    }

    public void Draw(Renderer renderer)
    {
        _drawableMesh.Draw(renderer);
        _axis.Draw(renderer);
    }

    public void Dispose()
    {
        _drawableMesh.Dispose();
        _axis.Dispose();
    }

    public void Activate(Window window)
    {
        window.ResetCameraPosition(new Vector3(2.5f, -2.5f, 2.5f), Vector3.UnitZ);
        window.Renderer.EnableDepthTest();
        _drawableMesh.UpdateMeshData();
    }
}
