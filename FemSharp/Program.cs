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
        var femProblemLaplace = LaplaceEquationWithEssentialAndNaturalBoundaryCondition(mesh);
        var femProblemHelmholtz = HelmholtzEquationWithSource(mesh);

        LogDifferences("Laplace (Key: D1)", femProblemLaplace);
        LogDifferences("Helmholtz (Key: D2)", femProblemHelmholtz);

        mesh.SetFemValues(femProblemLaplace.Approximation);

        using Window window = new Window(800, 600, "FEM Sharp");
        window.KeyDown += args =>
        {
            switch(args.Key)
            {
                case Keys.D1:
                case Keys.KeyPad1:
                    mesh.SetFemValues(femProblemLaplace.Approximation);
                    break;
                case Keys.D2:
                case Keys.KeyPad2:
                    mesh.SetFemValues(femProblemHelmholtz.Approximation);
                    break;
            }
        };
        window.AddMesh(mesh);
        window.Run();
    }

    /// <summary>
    /// Helmholtz equation with source
    /// -\nabla^2 u + u = f
    /// \nabla u = 0 on boundary
    /// f(x,y) =  cos(pi * (x - Bounds.Left) / Bounds.Width)) * cos(pi * (y - Bounds.Bottom) / Bounds.Height))
    /// </summary>
    private static (ColumnVector<float> Approximation, ColumnVector<float> Analytic) HelmholtzEquationWithSource(Mesh2D mesh)
    {
        float k = -5f;
        var femSimulation = new HelmholtzEquationWithSourceFEM(mesh, k, SourceFunction);
        var solution = femSimulation.Solve();
        var analyticSolution = new ColumnVector<float>(mesh.Vertices.Select(AnalyticSolutionHelmholtz).ToArray());
        return (solution, analyticSolution);

        float SourceFunction(FemVertex vertex)
        {
            return (k + MathF.Pow(MathF.PI / Bounds.Width, 2) + MathF.Pow(MathF.PI / Bounds.Height, 2)) * AnalyticSolutionHelmholtz(vertex);
        }

        float AnalyticSolutionHelmholtz(FemVertex vertex)
        {
            var radX = MathF.PI * (vertex.X + Bounds.Left) / Bounds.Width;
            var radY = MathF.PI * (vertex.Y + Bounds.Bottom) / Bounds.Height;

            return MathF.Cos(radX) * MathF.Cos(radY);
        }
    }

    /// <summary>
    /// Very unnatural problem, but just to combine Dirichlet and Neumann Boundary conditions
    /// \nabla^2 u = 0
    /// u(Bounds.Left, y) = u(Bounds.Right, y) = y
    /// du/dy(x, Bounds.Bottom) = du/dy(x, Bounds.Top) = 1
    /// </summary>
    private static (ColumnVector<float> Approximation, ColumnVector<float> Analytic) LaplaceEquationWithEssentialAndNaturalBoundaryCondition(Mesh2D mesh)
    {
        var femSimulation = new LaplaceFem(mesh);
        femSimulation.NaturalBoundaryConditions(NaturalBoundaryCondition);
        femSimulation.EssentialBoundaryCondition(EssentialBoundaryCondition);
        var solution = femSimulation.Solve();

        var analyticSolution = new ColumnVector<float>(mesh.Vertices.Select(AnalyticSolutionLaplace).ToArray());

        return (solution, analyticSolution);

        float AnalyticSolutionLaplace(FemVertex vertex)
        {
            return vertex.Y;
        }

        float NaturalBoundaryCondition(FemVertex vertex1, FemVertex vertex2)
        {
            // dU/dn = \nabla Y * n = (0, 1) * n
            if (vertex1.Y == vertex2.Y && vertex1.Y == Bounds.Bottom) // n = (0, -1)
                return -1f;

            if (vertex1.Y == vertex2.Y && vertex1.Y == Bounds.Top) // n = (0, 1)
                return 1f;

            return 0f; // n = (1, 0) | (-1, 0)
        }

        (bool, float) EssentialBoundaryCondition(FemVertex vertex1)
        {
            return (vertex1.X == Bounds.Left || vertex1.X == Bounds.Right, vertex1.Y);
        }
    }

    private static void LogDifferences(string name, (ColumnVector<float> Approximation, ColumnVector<float> Analytic) femProblem)
    {
        Debug.WriteLine(name);
        Debug.WriteLine($"|u-u_h|_1 = {(femProblem.Approximation - femProblem.Analytic).Norm1()}");
        Debug.WriteLine($"|u-u_h|_2 = {(femProblem.Approximation - femProblem.Analytic).Norm2()}");
        Debug.WriteLine($"|u-u_h|_Inf = {(femProblem.Approximation - femProblem.Analytic).NormInf()}");
    }
}