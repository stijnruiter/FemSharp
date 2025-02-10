using FemSharp.Render;
using LinearAlgebra.Structures;

namespace FemSharp.Simulation;


/// <summary>
/// A very simple, not optimized, implementation of the FiniteElementAnalysis for Helmholtz equation, with source F
/// -\nabla^2\phi + k\phi=f(x,y), with dirichlet boundary conditions
/// </summary>
internal class HelmholtzEquationWithSourceFEM
{
    private readonly Mesh2D _mesh;
    private readonly Matrix<float> _matrixA;
    private readonly ColumnVector<float> _columnVectorF;

    private ColumnVector<float>? _solution;

    public HelmholtzEquationWithSourceFEM(Mesh2D mesh, float k, Func<FemVertex, float> sourceF)
    {
        _mesh = mesh;
        (_matrixA, _columnVectorF) = AssembleElementMatrix(k, sourceF);
    }

    public ColumnVector<float> Solve()
    {
        if (_solution is null)
        {
            _solution = _matrixA.Solve(_columnVectorF, 0f);
        }

        return _solution;
    }

    private (Matrix<float>, ColumnVector<float>) AssembleElementMatrix(float k, Func<FemVertex, float> sourceF)
    {
        var elementMatrix = Matrix<float>.Zero(_mesh.Vertices.Length, _mesh.Vertices.Length);
        var sourceVector = ColumnVector<float>.Zero(_mesh.Vertices.Length);

        foreach(var element in _mesh.InteriorElements)
        {
            var vertex0 = _mesh.Vertices[element.I];
            var vertex1 = _mesh.Vertices[element.J];
            var vertex2 = _mesh.Vertices[element.K];

            var jacobian = Jacobian(vertex0, vertex1, vertex2);
            var detJ = jacobian.Determinant();
            var invJT = jacobian.Inverse().Transpose();

            var nablaPhi0 = invJT * new ColumnVector<float>([-1, -1]);
            var nablaPhi1 = invJT * new ColumnVector<float>([1, 0]);
            var nablaPhi2 = invJT * new ColumnVector<float>([0, 1]);

            elementMatrix[(int)element.I, (int)element.I] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi0 + k * detJ / 12;
            elementMatrix[(int)element.J, (int)element.I] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi0 + k * detJ / 24;
            elementMatrix[(int)element.K, (int)element.I] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi0 + k * detJ / 24;
            elementMatrix[(int)element.I, (int)element.J] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi1 + k * detJ / 24;
            elementMatrix[(int)element.J, (int)element.J] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi1 + k * detJ / 12;
            elementMatrix[(int)element.K, (int)element.J] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi1 + k * detJ / 24;
            elementMatrix[(int)element.I, (int)element.K] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi2 + k * detJ / 24;
            elementMatrix[(int)element.J, (int)element.K] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi2 + k * detJ / 24;
            elementMatrix[(int)element.K, (int)element.K] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi2 + k * detJ / 12;

            var f0 = sourceF(vertex0) / 24;
            var f1 = sourceF(vertex1) / 24;
            var f2 = sourceF(vertex2) / 24;

            sourceVector[(int)element.I] += detJ * (2 * f0 + f1 + f2);
            sourceVector[(int)element.J] += detJ * (f0 + 2 * f1 + f2);
            sourceVector[(int)element.K] += detJ * (f0 + f1 + 2 * f2);

        }
        return (elementMatrix, sourceVector);
    }

    private static Matrix<float> Jacobian(FemVertex vertex1, FemVertex vertex2, FemVertex vertex3) =>
            [[vertex2.X - vertex1.X, vertex3.X - vertex1.X],
             [vertex2.Y - vertex1.Y, vertex3.Y - vertex1.Y]];
}
