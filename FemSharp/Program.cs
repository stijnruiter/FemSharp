using FemSharp.Render;
using FemSharp.Simulation;
using LinearAlgebra.Structures;
using System.Diagnostics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FemSharp;

internal static class Program
{
    private static Rect Bounds = new Rect(-0.75f, 0.75f, -0.75f, 0.75f);

    private static void Main(string[] args)
    {
        var mesh = MeshGenerator.NaiveRectangle(Bounds, 30, 30);
        var femProblemLaplace = new LaplaceFem(Bounds, mesh);
        var femProblemHelmholtz = new HelmholtzEquationWithSourceFEM(Bounds, mesh, -5f);

        LogDifferences("Laplace (Key: D1)", femProblemLaplace);
        LogDifferences("Helmholtz (Key: D2)", femProblemHelmholtz);

        mesh.SetFemValues(femProblemLaplace.Solution);

        using Window window = new Window(800, 600, "FEM Sharp");
        window.KeyDown += args =>
        {
            switch(args.Key)
            {
                case Keys.D1:
                case Keys.KeyPad1:
                    mesh.SetFemValues(femProblemLaplace.Solution);
                    break;
                case Keys.D2:
                case Keys.KeyPad2:
                    mesh.SetFemValues(femProblemHelmholtz.Solution);
                    break;
            }
        };
        window.AddMesh(mesh);
        window.Run();
    }

    private static void LogDifferences(string name, Abstract2DFemProblem problem)
    {
        Debug.WriteLine(name);
        Debug.WriteLine($"|u-u_h|_1 = {(problem.Solution - problem.AnalyticSolution).Norm1()}");
        Debug.WriteLine($"|u-u_h|_2 = {(problem.Solution - problem.AnalyticSolution).Norm2()}");
        Debug.WriteLine($"|u-u_h|_Inf = {(problem.Solution - problem.AnalyticSolution).NormInf()}");
    }
}