using FemSharp.Render;

namespace FemSharp;

internal static class Program
{
    private static void Main(string[] args)
    {
        var bounds = new Rect(-0.75f, 0.75f, -0.75f, 0.75f);
        var mesh = Mesh2D.NaiveRectangle(bounds, 8, 5);
        using Window window = new Window(800, 600, "FEM Sharp");
        window.AddMesh(mesh);
        window.Run();
    }

}