using FemSharp.Render;
using LinearAlgebra.Structures;

namespace FemSharp.Simulation;


/// <summary>
/// A very simple, not optimized, implementation of the FiniteElementAnalysis for Helmholtz equation, with source F
/// -\nabla^2\phi + k\phi=f(x,y), with dirichlet boundary conditions
/// </summary>
internal class LaplaceFem
{
    private readonly Mesh2D _mesh;
    private readonly Matrix<float> _matrixA;
    private readonly ColumnVector<float> _columnVectorF;

    private ColumnVector<float>? _solution;

    public LaplaceFem(Mesh2D mesh)
    {
        _mesh = mesh;
        (_matrixA, _columnVectorF) = AssembleElementMatrix();
    }

    public ColumnVector<float> Solve()
    {
        if (_solution is null)
        {
            _solution = _matrixA.Solve(_columnVectorF, 0f);
        }

        return _solution;
    }

    private (Matrix<float>, ColumnVector<float>) AssembleElementMatrix()
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

            elementMatrix[(int)element.I, (int)element.I] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi0;
            elementMatrix[(int)element.J, (int)element.I] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi0;
            elementMatrix[(int)element.K, (int)element.I] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi0;
            elementMatrix[(int)element.I, (int)element.J] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi1;
            elementMatrix[(int)element.J, (int)element.J] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi1;
            elementMatrix[(int)element.K, (int)element.J] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi1;
            elementMatrix[(int)element.I, (int)element.K] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi2;
            elementMatrix[(int)element.J, (int)element.K] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi2;
            elementMatrix[(int)element.K, (int)element.K] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi2;
        }
        return (elementMatrix, sourceVector);
    }

    public void NaturalBoundaryConditions(Func<FemVertex, FemVertex, float> constNaturalBoundaryFunc)
    {
        foreach (var boundaryElement in _mesh.BoundaryElements)
        {
            var vertex1 = _mesh.Vertices[boundaryElement.I];
            var vertex2 = _mesh.Vertices[boundaryElement.J];
            var length = vertex1.Distance(vertex2);

            var constNaturalBoundary = 0.5f * length * constNaturalBoundaryFunc(vertex1, vertex2);

            _columnVectorF[(int)boundaryElement.I] += constNaturalBoundary;
            _columnVectorF[(int)boundaryElement.J] += constNaturalBoundary;
        }
    }

    public void EssentialBoundaryCondition(Func<FemVertex, (bool, float)> essentialBoundaryFunc)
    {
        foreach(var boundaryIndex in _mesh.BoundaryElements.SelectMany(line => new[] {line.I, line.J}).Distinct())
        {
            (var shouldApply, var value) = essentialBoundaryFunc(_mesh.Vertices[boundaryIndex]);
            if (!shouldApply)
                continue;

            for(var j = 0; j < _matrixA.ColumnCount; j++)
            {
                _matrixA[(int)boundaryIndex, j] = 0.0f;
            }

            _matrixA[(int)boundaryIndex, (int)boundaryIndex] = 1.0f;
            _columnVectorF[(int)boundaryIndex] = value;
        }
    }



    private static Matrix<float> Jacobian(FemVertex vertex1, FemVertex vertex2, FemVertex vertex3) =>
            [[vertex2.X - vertex1.X, vertex3.X - vertex1.X],
             [vertex2.Y - vertex1.Y, vertex3.Y - vertex1.Y]];
}
