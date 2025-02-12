using FemSharp.Render;

namespace FemSharp.Simulation;

/// <summary>
/// Very unnatural problem, but just to combine Dirichlet and Neumann Boundary conditions
/// \nabla^2 u = 0
/// u(Bounds.Left, y) = u(Bounds.Right, y) = y
/// du/dy(x, Bounds.Bottom) = du/dy(x, Bounds.Top) = 1
/// </summary>
internal class LaplaceFem : Abstract2DFemProblem
{
    public LaplaceFem(Rect bounds, Mesh2D mesh) : base(mesh)
    {
        _bounds = bounds;
        Add_Matrix_NablaA_NablaV(1.0f);
    
        ApplyNaturalBoundaryConditions(NaturalBoundaryCondition);
        ApplyEssentialBoundaryCondition(EssentialBoundaryCondition);
    }

    public override bool HasAnalyticSolution => true;

    private float NaturalBoundaryCondition(Vertex vertex1, Vertex vertex2)
    {
        // dU/dn = \nabla Y * n = (0, 1) * n
        if (vertex1.Y == vertex2.Y && vertex1.Y == _bounds.Bottom) // n = (0, -1)
            return -1f;

        if (vertex1.Y == vertex2.Y && vertex1.Y == _bounds.Top) // n = (0, 1)
            return 1f;

        return 0f; // n = (1, 0) | (-1, 0)
    }

    private float? EssentialBoundaryCondition(Vertex vertex1)
    {
        return (vertex1.X == _bounds.Left || vertex1.X == _bounds.Right) ? vertex1.Y : null;
    }

    protected override float AnalyticSolutionFunction(Vertex position)
    {
        return position.Y;
    }

    private readonly Rect _bounds;
}
