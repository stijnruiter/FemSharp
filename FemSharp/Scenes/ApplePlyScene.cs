using System.Reflection;
using FemSharp.Render;
using NumericalMath.Geometry;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FemSharp.Scenes;

internal class ApplePlyScene : IScene
{
    public Keys ActivateKey { get; }
    private Mesh2D Mesh { get; }
    private DrawableMesh2D DrawableMesh { get; }
    private Axis Axis { get; }

    public ApplePlyScene(Keys activateKey)
    {
        var file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Resources", "apple.ply");
        var mesh = MeshBuilder.FromPly(file);
        ActivateKey = activateKey;
        Mesh = new Mesh2D(mesh.Vertices.Select(v => new ValuedVertex(v)).ToArray(), mesh.Faces, []);
        DrawableMesh = new DrawableMesh2D(Mesh);
        Axis = new Axis();
    }

    public void Dispose()
    {
        DrawableMesh.Dispose();
        Axis.Dispose();
    }

    public void Update()
    {
    }

    public void Activate(Window window)
    {
        window.ResetCameraPosition(new Vector3(0.25f, 0.25f, 0.25f), Vector3.UnitZ);
        window.Renderer.EnableDepthTest();
    }

    public void Draw(Renderer renderer)
    {
        DrawableMesh.Draw(renderer);
        Axis.Draw(renderer);
    }
}
