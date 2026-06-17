# Numerical Mathematics from scratch

[![UnitTests](https://github.com/stijnruiter/NumericalMath/actions/workflows/dotnet.yml/badge.svg)](https://github.com/stijnruiter/NumericalMath/actions)

This repository is my implementation of numerical analysis procedures purely written in C# using .NET Core 8. I intend to only use .NET Core as a dependency, and write all computational mathematics procedures from scratch. I will use benchmark tools for optimization purposes.

The main goal of this project is to learn more about the .NET functionalities for computational mathematics as well as scientific computing algorithms. 

## Implemented

### Linear Algebra
- Dense Column/Row vector data structure
	- Use INumber\<T\> interface
- Dense Matrix data structure (both row-major and column major storage)
- Some simple matrix vector arithmetics (nothing optimized)
	- Operators for scalar/vector/matrix arithmetics
- _LU_ factorization (with partial pivoting)
	- Use SIMD instructions to speed up larger systems. [Results](Benchmarks/MatrixOptimization.cs)
- Solving the system **Ax=b** and **AX=B** 
- Determinant and inverse of matrix

### Geometry
- Incremental Delaunay triangulation

## Roadmap

### General

- Rational numbers
- Derivatives
- Integrals

### Linear Algebra

- Cholesky factorization
- Sparse matrices

### Geometry
- Mesh generation
- B-splines

### Numerical Methods
- Finite Difference Method
- Finite Element Method
- Material Point Method
