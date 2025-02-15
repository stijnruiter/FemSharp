using FemSharp.Render;
using FemSharp.Simulation;
using LinearAlgebra.Structures;
using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using FemSharp.Scenes;

namespace FemSharp;

internal static class Program
{
    private static void Main(string[] args)
    {
        using Window window = new Window(800, 600, "FEM Sharp");

        Rect Bounds = new Rect(-0.75f, 0.75f, -0.75f, 0.75f);
        window.Scenes.Add(new HelmholtzScene(Keys.D1, Bounds, 10, 10, 5));
        window.Scenes.Add(new LaplaceScene(Keys.D2, Bounds, 10, 10));
        window.Scenes.Add(new CircularScene(Keys.D3, 0, 0, 1, 5));
        window.ActivateScene(window.Scenes.First());
        
        window.Run();
    }
}