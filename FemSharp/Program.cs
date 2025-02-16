using OpenTK.Windowing.GraphicsLibraryFramework;
using FemSharp.Scenes;

namespace FemSharp;

internal static class Program
{
    private static void Main(string[] args)
    {
        using Window window = new Window(800, 600, "FEM Sharp");

        Rect Bounds = new Rect(-0.75f, 0.75f, -0.75f, 0.75f);
        window.Scenes.Add(new DelaunayScene(Keys.A, Bounds, 25));
        window.Scenes.Add(new CircularScene(Keys.Q, 0, 0, 1, 0.1f));
        window.Scenes.Add(new HelmholtzScene(Keys.W, Bounds, 10, 10, 5));
        window.Scenes.Add(new LaplaceScene(Keys.E, Bounds, 10, 10));
        window.ActivateScene(window.Scenes.First());
        
        window.Run();
    }
}