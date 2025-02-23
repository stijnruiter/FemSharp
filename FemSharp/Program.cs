using OpenTK.Windowing.GraphicsLibraryFramework;
using FemSharp.Scenes;
using NumericalMath.Geometry.Structures;

namespace FemSharp;

internal static class Program
{
    private static void Main(string[] args)
    {
        using Window window = new Window(800, 600, "FEM Sharp");

        Rectangle bounds = new Rectangle(-0.75f, 0.75f, -0.75f, 0.75f);
        window.Scenes.Add(new DelaunayScene(Keys.R, bounds, 25));
        window.Scenes.Add(new CircularScene(Keys.Q, 0, 0, 1, 0.1f));
        window.Scenes.Add(new HelmholtzScene(Keys.W, bounds, 10, 10, 5));
        window.Scenes.Add(new LaplaceScene(Keys.E, bounds, 10, 10));
        window.ActivateScene(window.Scenes.First());
        
        window.Run();
    }
}