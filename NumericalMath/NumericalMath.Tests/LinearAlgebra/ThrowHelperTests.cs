using System;
using System.Collections.Generic;
using NumericalMath.Exceptions;
using NumericalMath.LinearAlgebra;
using NumericalMath.LinearAlgebra.Structures;
using NUnit.Framework.Constraints;

namespace NumericalMath.Tests.LinearAlgebra;

[TestFixture]
public class ThrowHelperTests
{
    [TestCase(new[]{1,2,3,4,5}, new[]{5, 4, 3, 2,1}, new[]{1,3,5})]
    public void ThrowIfDifferentLength_WhenDifferentLengthSpan_ShouldThrow(int[] baseline, int[] correctLength, int[] wrongLength)
    {
        Assert.That(() => ThrowHelper.ThrowIfDifferentLength<int>(baseline, correctLength), Throws.Nothing);
        Assert.That(() => ThrowHelper.ThrowIfDifferentLength<int>(baseline, wrongLength), Throws.Exception.TypeOf<DimensionMismatchException>());
    }

    [TestCase(new[] { 1, 2, 3, 4, 5 }, new[] { 5, 4, 3, 2, 1 }, new[] { 1, 3, 5 })]
    public void ThrowIfDifferentLength_WhenDifferentLengthVector_ShouldThrow(int[] baseline, int[] correctLength, int[] wrongLength)
    {
        var columnVectorBaseline = new ColumnVector<int>(baseline);
        var columnVectorCorrect = new ColumnVector<int>(correctLength);
        var columnVectorWrong = new ColumnVector<int>(wrongLength);
        
        Assert.That(() => ThrowHelper.ThrowIfDifferentLength(columnVectorBaseline, columnVectorCorrect), Throws.Nothing);
        Assert.That(() => ThrowHelper.ThrowIfDifferentLength(columnVectorBaseline, columnVectorWrong), Throws.Exception.TypeOf<DimensionMismatchException>());
    }

    private static IEnumerable<TestCaseData> WrongSizeMatrices()
    {
        yield return new TestCaseData((Matrix<int>)[[1, 2, 3], [2, 3, 4], [5, 6, 7]], (Matrix<int>)[[1, 2, 3], [6,3,2 ], [3, 2, 1]]).Returns(false);
        yield return new TestCaseData((Matrix<int>)[[1, 2], [2, 3], [5, 6]], (Matrix<int>)[[1, 2, 3], [6,3,2 ], [3, 2, 1]]).Returns(true);
        yield return new TestCaseData((Matrix<int>)[[1, 2, 3], [2, 3, 4]], (Matrix<int>)[[1, 2, 3], [6,3,2 ], [3, 2, 1]]).Returns(true);
        yield return new TestCaseData((Matrix<int>)[[]], (Matrix<int>)[[1, 2, 3], [6,3,2 ], [3, 2, 1]]).Returns(true);
        yield return new TestCaseData((Matrix<int>)[[]], (Matrix<int>)[[]]).Returns(false);
    }
    
    [TestCaseSource(nameof(WrongSizeMatrices))]
    public bool ThrowIfDifferentSize_WhenWrongMatrixSizes_ShouldThrow(Matrix<int> matrix1, Matrix<int> matrix2) 
        => MethodDoesThrow<DimensionMismatchException>(() => ThrowHelper.ThrowIfDifferentSize(matrix1, matrix2));

    private static bool MethodDoesThrow<T>(Action action) where T: Exception
    {
        try
        {
            action();
            return false;
        }
        catch (Exception e)
        {
            return e is T;
        }
    }

    private static IEnumerable<TestCaseData> IndexRangeMatrices()
    {
        Matrix<int> matrix = [[1, 2, 3, 4, 5], [3, 4, 5, 6, 7], [1, 2, 3, 4, 5]];
        yield return new TestCaseData(matrix, 0, 0, false, false).SetArgDisplayNames("Mat3x5 at (0,0)");
        yield return new TestCaseData(matrix, 2, 4, false, false).SetArgDisplayNames("Mat3x5 at (2,4)");
        yield return new TestCaseData(matrix, 2, 5, false, true).SetArgDisplayNames("Mat3x5 at (2,5)");
        yield return new TestCaseData(matrix, 3, 4, true, false).SetArgDisplayNames("Mat3x5 at (3,4)");
        yield return new TestCaseData(matrix,  2, -1, false, true).SetArgDisplayNames("Mat3x5 at (2,-1)");
        yield return new TestCaseData(matrix, -1, 2, true, false).SetArgDisplayNames("Mat3x5 at (-1,2)");

        matrix = [[]];
        yield return new TestCaseData(matrix, 0, 0, true, true).SetArgDisplayNames("Mat0x0 at (0,0)");
    }
    
    [TestCaseSource(nameof(IndexRangeMatrices))]
    public void IndexRangeMatrix_WhenIndexIsOutOfRange_ShouldThrow(Matrix<int> matrix, int i, int j, bool iInvalid, bool jInvalid)
    {
        Assert.That(() => ThrowHelper.ThrowIfOutOfRange(i, j, matrix.Storage), DoesThrow<IndexOutOfRangeException>(iInvalid || jInvalid));
        Assert.That(() => ThrowHelper.ThrowIfRowOutOfRange(i, matrix.Storage), DoesThrow<IndexOutOfRangeException>(iInvalid));
        Assert.That(() => ThrowHelper.ThrowIfColumnOutOfRange(j, matrix.Storage), DoesThrow<IndexOutOfRangeException>(jInvalid));
        return;
        
        IResolveConstraint DoesThrow<T>(bool throws) where T : Exception => throws ? Throws.Exception.TypeOf<T>() : Throws.Nothing;
    }

    private static IEnumerable<TestCaseData> IndexRangeVectorSet()
    {
        ColumnVector<int> columnVector = [1, 2, 3];
        yield return new TestCaseData(columnVector, 0).Returns(false).SetArgDisplayNames("ColumnVector3 at 0");
        yield return new TestCaseData(columnVector, -1).Returns(true).SetArgDisplayNames("ColumnVector3 at -1");
        yield return new TestCaseData(columnVector, 3).Returns(true).SetArgDisplayNames("ColumnVector3 at 3");
        
        RowVector<int> rowVector = [1, 2, 3];
        yield return new TestCaseData(rowVector, 0).Returns(false).SetArgDisplayNames("RowVector3 at 0");
        yield return new TestCaseData(rowVector, -1).Returns(true).SetArgDisplayNames("RowVector3 at -1");
        yield return new TestCaseData(rowVector, 3).Returns(true).SetArgDisplayNames("RowVector3 at 3");
    }

    [TestCaseSource(nameof(IndexRangeVectorSet))]
    public bool IndexRangeVector_WhenIndexIsOutOfRange_ShouldThrow(AbstractVector<int> vector, int i)
        => MethodDoesThrow<IndexOutOfRangeException>(() => ThrowHelper.ThrowIfOutOfRange(i, vector));


    public static IEnumerable<TestCaseData> EmptyMatrices()
    {
        yield return new TestCaseData((Matrix<int>) [[]]).Returns(true).SetArgDisplayNames("Empty Matrix");
        yield return new TestCaseData((Matrix<int>) [[1]]).Returns(false).SetArgDisplayNames("Non-Empty Matrix");
        
        yield return new TestCaseData((ColumnVector<int>) []).Returns(true).SetArgDisplayNames("Empty ColumnVector");
        yield return new TestCaseData((ColumnVector<int>) [1]).Returns(false).SetArgDisplayNames("Non-Empty ColumnVector");
        
        yield return new TestCaseData((RowVector<int>) []).Returns(true).SetArgDisplayNames("Empty RowVector");
        yield return new TestCaseData((RowVector<int>) [1]).Returns(false).SetArgDisplayNames("Non-Empty RowVector");
    }

    [TestCaseSource(nameof(EmptyMatrices))]
    public bool ThrowIfEmpty_WhenMatrixIsEmpty_ShouldThrow(IRectanglarMatrix<int> matrix)
        => MethodDoesThrow<Exception>(() => ThrowHelper.ThrowIfEmpty(matrix));
}