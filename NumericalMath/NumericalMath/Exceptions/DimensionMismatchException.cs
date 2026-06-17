using System;

namespace NumericalMath.Exceptions;

public class DimensionMismatchException : Exception
{
    public DimensionMismatchException(string? message) : base(message) { }
    public DimensionMismatchException(string? message, int dim1, int dim2) : base($"{message}\n{dim1} != {dim2}") { }
}
