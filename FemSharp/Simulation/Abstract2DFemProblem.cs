﻿using FemSharp.Render;
using NumericalMath.Geometry.Structures;
using NumericalMath.LinearAlgebra.Structures;
using System.Diagnostics;

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

    public ColumnVector<float>? Solution => _solution;

    public ColumnVector<float> AnalyticSolution
    {
        get
        {
            if (!HasAnalyticSolution)
                throw new Exception("This problem does not have an analytic solution.");

            if (_analyticSolution == null)
            {
                _analyticSolution = new ColumnVector<float>(Mesh.Vertices.Select(v => AnalyticSolutionFunction(v.Position)).ToArray());
            }

            return _analyticSolution;
        }
    }

    public abstract bool HasAnalyticSolution { get; }

    public ColumnVector<float> Solve()
    {
        _solution = MatrixA.Solve(VectorB);
        Mesh.SetFemValues(_solution);
        LogDifferences();
        return _solution;
    }

    private void LogDifferences()
    {
        string output = (GetType().Name + " finished") + Environment.NewLine;
        if (Solution is not null && HasAnalyticSolution && AnalyticSolution is not null)
        {
            output += ($"|u-u_h|_1 = {(Solution - AnalyticSolution).Norm1()}") + Environment.NewLine;
            output += ($"|u-u_h|_2 = {(Solution - AnalyticSolution).Norm2()}") + Environment.NewLine;
            output += ($"|u-u_h|_Inf = {(Solution - AnalyticSolution).NormInf()}") + Environment.NewLine;
        }
        Debug.WriteLine(output);
    }

    protected abstract float AnalyticSolutionFunction(Vertex3 position);

    protected void Add_Matrix_NablaA_NablaV(float scalar)
    {
        foreach (var element in Mesh.InteriorElements)
        {
            var vertex0 = Mesh.Vertices[element.I].Position;
            var vertex1 = Mesh.Vertices[element.J].Position;
            var vertex2 = Mesh.Vertices[element.K].Position;

            var jacobian = Jacobian(vertex0, vertex1, vertex2);
            var detJ = jacobian.Determinant();
            var invJT = jacobian.Inverse().Transpose();

            var nablaPhi0 = invJT * new ColumnVector<float>([-1, -1]);
            var nablaPhi1 = invJT * new ColumnVector<float>([1, 0]);
            var nablaPhi2 = invJT * new ColumnVector<float>([0, 1]);

            MatrixA[element.I, element.I] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi0;
            MatrixA[element.J, element.I] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi0;
            MatrixA[element.K, element.I] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi0;
            MatrixA[element.I, element.J] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi1;
            MatrixA[element.J, element.J] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi1;
            MatrixA[element.K, element.J] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi1;
            MatrixA[element.I, element.K] += 0.5f * detJ * nablaPhi0.Transpose() * nablaPhi2;
            MatrixA[element.J, element.K] += 0.5f * detJ * nablaPhi1.Transpose() * nablaPhi2;
            MatrixA[element.K, element.K] += 0.5f * detJ * nablaPhi2.Transpose() * nablaPhi2;
        }
    }

    protected void Add_Matrix_U_V(float scalar)
    {
        foreach (var element in Mesh.InteriorElements)
        {
            var vertex0 = Mesh.Vertices[element.I].Position;
            var vertex1 = Mesh.Vertices[element.J].Position;
            var vertex2 = Mesh.Vertices[element.K].Position;

            var jacobian = Jacobian(vertex0, vertex1, vertex2);
            var detJ = jacobian.Determinant();

            MatrixA[element.I, element.I] += scalar * detJ / 12;
            MatrixA[element.J, element.I] += scalar * detJ / 24;
            MatrixA[element.K, element.I] += scalar * detJ / 24;
            MatrixA[element.I, element.J] += scalar * detJ / 24;
            MatrixA[element.J, element.J] += scalar * detJ / 12;
            MatrixA[element.K, element.J] += scalar * detJ / 24;
            MatrixA[element.I, element.K] += scalar * detJ / 24;
            MatrixA[element.J, element.K] += scalar * detJ / 24;
            MatrixA[element.K, element.K] += scalar * detJ / 12;
        }
    }

    protected void Add_Vector_U_F(Func<Vertex3, float> sourceF)
    {
        foreach (var element in Mesh.InteriorElements)
        {
            var vertex0 = Mesh.Vertices[element.I].Position;
            var vertex1 = Mesh.Vertices[element.J].Position;
            var vertex2 = Mesh.Vertices[element.K].Position;

            var jacobian = Jacobian(vertex0, vertex1, vertex2);
            var detJ = jacobian.Determinant();

            var f0 = sourceF(vertex0) / 24;
            var f1 = sourceF(vertex1) / 24;
            var f2 = sourceF(vertex2) / 24;

            VectorB[element.I] += detJ * (2 * f0 + f1 + f2);
            VectorB[element.J] += detJ * (f0 + 2 * f1 + f2);
            VectorB[element.K] += detJ * (f0 + f1 + 2 * f2);
        }
    }

    protected void ApplyNaturalBoundaryConditions(Func<Vertex3, Vertex3, float> constNaturalBoundaryFunc)
    {
        foreach (var boundaryElement in Mesh.BoundaryElements)
        {
            var vertex1 = Mesh.Vertices[(int)boundaryElement.I].Position;
            var vertex2 = Mesh.Vertices[(int)boundaryElement.J].Position;
            var length = vertex1.Distance(vertex2);

            var constNaturalBoundary = 0.5f * length * constNaturalBoundaryFunc(vertex1, vertex2);

            VectorB[boundaryElement.I] += constNaturalBoundary;
            VectorB[boundaryElement.J] += constNaturalBoundary;
        }
    }

    protected void ApplyEssentialBoundaryCondition(Func<Vertex3, float?> essentialBoundaryFunc)
    {
        foreach (var boundaryIndex in Mesh.BoundaryElements.SelectMany(line => new[] { line.I, line.J }).Distinct())
        {
            if (essentialBoundaryFunc(Mesh.Vertices[boundaryIndex].Position) is not { } value)
                continue;

            for (var j = 0; j < MatrixA.ColumnCount; j++)
            {
                MatrixA[boundaryIndex, j] = 0.0f;
            }

            MatrixA[boundaryIndex, boundaryIndex] = 1.0f;
            VectorB[boundaryIndex] = value;
        }
    }

    private static Matrix<float> Jacobian(Vertex3 vertex1, Vertex3 vertex2, Vertex3 vertex3)
    {
        return [[vertex2.X - vertex1.X, vertex3.X - vertex1.X],
             [vertex2.Y - vertex1.Y, vertex3.Y - vertex1.Y]];
    }

    private ColumnVector<float>? _analyticSolution;

    private ColumnVector<float>? _solution;
}
