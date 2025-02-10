using FemSharp.Render;
using FemSharp.Simulation;
using LinearAlgebra.Structures;
using System.Diagnostics;

namespace FemSharp;

internal static class Program
{
    private static Rect Bounds = new Rect(-0.75f, 0.75f, -0.75f, 0.75f);

    private const float k = -5f;

    private static float AnalyticSolution(FemVertex vertex)
    {
        var radX = MathF.PI * (vertex.X + Bounds.Left) / Bounds.Width;
        var radY = MathF.PI * (vertex.Y + Bounds.Bottom) / Bounds.Height;

        return MathF.Cos(radX) * MathF.Cos(radY);
    }

    private static float SourceFunction(FemVertex vertex)
    {
        return (k + MathF.Pow(MathF.PI / Bounds.Width, 2) + MathF.Pow(MathF.PI / Bounds.Height, 2)) * AnalyticSolution(vertex);
    }

    private static void Main(string[] args)
    {
        var mesh = MeshGenerator.NaiveRectangle(Bounds, 30, 30);
        var femSimulation = new HelmholtzEquationWithSourceFEM(mesh, k, SourceFunction);
        var solution = femSimulation.Solve();
        mesh.SetFemValues(solution);

        var analyticSolution = new ColumnVector<float>(mesh.Vertices.Select(AnalyticSolution).ToArray());

        Debug.WriteLine($"|u-u_h|_1 = {(solution - analyticSolution).Norm1()}");
        Debug.WriteLine($"|u-u_h|_2 = {(solution - analyticSolution).Norm2()}");
        Debug.WriteLine($"|u-u_h|_Inf = {(solution - analyticSolution).NormInf()}");

        using Window window = new Window(800, 600, "FEM Sharp");
        window.AddMesh(mesh);
        window.Run();
    }
}