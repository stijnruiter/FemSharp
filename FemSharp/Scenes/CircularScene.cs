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

    public CircularScene(Keys activateKey, float cx, float cy, float radius, float maxh)
    {
        ActivateKey = activateKey;
        var delaunay = MeshGenerator.DelaunayCircle(cx, cy, radius, maxh);
        delaunay.Refine(25);
        var triangulation = delaunay.ToMesh();
        Mesh = new Mesh2D(triangulation.Vertices.Select(v => new ValuedVertex(v)).ToArray(), triangulation.Interior.ToArray(), triangulation.Boundary.ToArray());
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
