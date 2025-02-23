using NumericalMath.Geometry.Structures;

namespace FemSharp.Simulation;

/// <summary>
/// Helmholtz equation with source
/// -\nabla^2 u + u = f
/// \nabla u = 0 on boundary
/// f(x,y) =  cos(pi * (x - Bounds.Left) / Bounds.Width)) * cos(pi * (y - Bounds.Bottom) / Bounds.Height))
/// </summary>
internal class HelmholtzEquationWithSourceFEM : Abstract2DFemProblem
{
    public HelmholtzEquationWithSourceFEM(Rectangle bounds, Mesh2D mesh, float k) : base(mesh)
    {
        _bounds = bounds;
        _k = k;

        Add_Matrix_NablaA_NablaV(-1f);
        Add_Matrix_U_V(_k);

        Add_Vector_U_F(SourceFunction);
    }

    public override bool HasAnalyticSolution => true;

    protected override float AnalyticSolutionFunction(Vertex3 position)
    {
        var radX = MathF.PI * (position.X + _bounds.Left) / _bounds.Width;
        var radY = MathF.PI * (position.Y + _bounds.Bottom) / _bounds.Height;

        return MathF.Cos(radX) * MathF.Cos(radY);
    }

    private float SourceFunction(Vertex3 vertex)
    {
        return (_k + MathF.Pow(MathF.PI / _bounds.Width, 2) + MathF.Pow(MathF.PI / _bounds.Height, 2)) * AnalyticSolutionFunction(vertex);
    }

    private readonly Rectangle _bounds;
    private readonly float _k;
}
