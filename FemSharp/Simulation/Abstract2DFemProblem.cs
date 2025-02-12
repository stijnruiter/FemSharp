using FemSharp.Render;
using LinearAlgebra.Structures;

namespace FemSharp.Simulation;

internal abstract class Abstract2DFemProblem
{
    public Abstract2DFemProblem(Mesh2D mesh)
    {
        Mesh = mesh;
        MatrixA = Matrix<float>.Zero(mesh.Vertices.Length);
        VectorB = ColumnVector<float>.Zero(mesh.Vertices.Length);
    }

    public Mesh2D Mesh { get; }

    public Matrix<float> MatrixA { get; }

    public ColumnVector<float> VectorB { get; }

    public ColumnVector<float> Solution
    {
        get
        {
            if (_solution == null)
            {
                _solution = Solve();
            }
            return _solution;
        }
    }

    public ColumnVector<float> AnalyticSolution
    {
        get
        {
            if (!HasAnalyticSolution)
                throw new Exception("This problem does not have an analytic solution.");

            if (_analyticSolution == null)
            {
                _analyticSolution = new ColumnVector<float>(Mesh.Vertices.Select(AnalyticSolutionFunction).ToArray());
            }

            return _analyticSolution;
        }
    }

    public abstract bool HasAnalyticSolution { get; }

    public ColumnVector<float> Solve()
    {
        _solution = MatrixA.Solve(VectorB);
        return _solution;
    }

    protected abstract float AnalyticSolutionFunction(FemVertex position);

    protected void Add_Matrix_NablaA_NablaV(float scalar)
    {
        foreach (var element in Mesh.InteriorElements)
        {
            var vertex0 = Mesh.Vertices[element.I];
            var vertex1 = Mesh.Vertices[element.J];
            var vertex2 = Mesh.Vertices[element.K];

            var jacobian = Jacobian(vertex0, vertex1, vertex2);
            var detJ = jacobian.Determinant();
            var invJT = jacobian.Inverse().Transpose();

            var nablaPhi0 = invJT * new ColumnVector<float>([-1, -1]);
            var nablaPhi1 = invJT * new ColumnVector<float>([1, 0]);
            var nablaPhi2 = invJT * new ColumnVector<float>([0, 1]);

            MatrixA[(int)element.I, (int)element.I] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi0;
            MatrixA[(int)element.J, (int)element.I] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi0;
            MatrixA[(int)element.K, (int)element.I] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi0;
            MatrixA[(int)element.I, (int)element.J] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi1;
            MatrixA[(int)element.J, (int)element.J] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi1;
            MatrixA[(int)element.K, (int)element.J] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi1;
            MatrixA[(int)element.I, (int)element.K] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi2;
            MatrixA[(int)element.J, (int)element.K] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi2;
            MatrixA[(int)element.K, (int)element.K] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi2;
        }
    }

    protected void Add_Matrix_U_V(float scalar)
    {
        foreach (var element in Mesh.InteriorElements)
        {
            var vertex0 = Mesh.Vertices[element.I];
            var vertex1 = Mesh.Vertices[element.J];
            var vertex2 = Mesh.Vertices[element.K];

            var jacobian = Jacobian(vertex0, vertex1, vertex2);
            var detJ = jacobian.Determinant();

            MatrixA[(int)element.I, (int)element.I] += scalar * detJ / 12;
            MatrixA[(int)element.J, (int)element.I] += scalar * detJ / 24;
            MatrixA[(int)element.K, (int)element.I] += scalar * detJ / 24;
            MatrixA[(int)element.I, (int)element.J] += scalar * detJ / 24;
            MatrixA[(int)element.J, (int)element.J] += scalar * detJ / 12;
            MatrixA[(int)element.K, (int)element.J] += scalar * detJ / 24;
            MatrixA[(int)element.I, (int)element.K] += scalar * detJ / 24;
            MatrixA[(int)element.J, (int)element.K] += scalar * detJ / 24;
            MatrixA[(int)element.K, (int)element.K] += scalar * detJ / 12;
        }
    }

    protected void Add_Vector_U_F(Func<FemVertex, float> sourceF)
    {
        foreach (var element in Mesh.InteriorElements)
        {
            var vertex0 = Mesh.Vertices[element.I];
            var vertex1 = Mesh.Vertices[element.J];
            var vertex2 = Mesh.Vertices[element.K];

            var jacobian = Jacobian(vertex0, vertex1, vertex2);
            var detJ = jacobian.Determinant();

            var f0 = sourceF(vertex0) / 24;
            var f1 = sourceF(vertex1) / 24;
            var f2 = sourceF(vertex2) / 24;

            VectorB[(int)element.I] += detJ * (2 * f0 + f1 + f2);
            VectorB[(int)element.J] += detJ * (f0 + 2 * f1 + f2);
            VectorB[(int)element.K] += detJ * (f0 + f1 + 2 * f2);
        }
    }

    protected void ApplyNaturalBoundaryConditions(Func<FemVertex, FemVertex, float> constNaturalBoundaryFunc)
    {
        foreach (var boundaryElement in Mesh.BoundaryElements)
        {
            var vertex1 = Mesh.Vertices[boundaryElement.I];
            var vertex2 = Mesh.Vertices[boundaryElement.J];
            var length = vertex1.Distance(vertex2);

            var constNaturalBoundary = 0.5f * length * constNaturalBoundaryFunc(vertex1, vertex2);

            VectorB[(int)boundaryElement.I] += constNaturalBoundary;
            VectorB[(int)boundaryElement.J] += constNaturalBoundary;
        }
    }

    protected void ApplyEssentialBoundaryCondition(Func<FemVertex, float?> essentialBoundaryFunc)
    {
        foreach (var boundaryIndex in Mesh.BoundaryElements.SelectMany(line => new[] { line.I, line.J }).Distinct())
        {
            if (essentialBoundaryFunc(Mesh.Vertices[boundaryIndex]) is not { } value)
                continue;

            for (var j = 0; j < MatrixA.ColumnCount; j++)
            {
                MatrixA[(int)boundaryIndex, j] = 0.0f;
            }

            MatrixA[(int)boundaryIndex, (int)boundaryIndex] = 1.0f;
            VectorB[(int)boundaryIndex] = value;
        }
    }

    private static Matrix<float> Jacobian(FemVertex vertex1, FemVertex vertex2, FemVertex vertex3)
    {
        return [[vertex2.X - vertex1.X, vertex3.X - vertex1.X],
             [vertex2.Y - vertex1.Y, vertex3.Y - vertex1.Y]];
    }

    private ColumnVector<float>? _analyticSolution;

    private ColumnVector<float>? _solution;
}
