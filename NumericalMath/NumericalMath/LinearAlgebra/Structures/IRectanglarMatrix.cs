using System.Numerics;

namespace NumericalMath.LinearAlgebra.Structures;

public interface IRectanglarMatrix<T> where T : struct, INumber<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public T this[int rowIndex, int columnIndex] { get; set; }
}
