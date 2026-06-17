using System.Collections.Generic;
using System.Linq;
using NumericalMath.LinearAlgebra.Structures;
using NumericalMath.Tests.ExtensionHelpers;

namespace NumericalMath.Tests.LinearAlgebra.Structures;

[TestFixture]
public class VectorTests
{
    [Test]
    public void Product_WhenRowColumnVector_ShouldReturnInnerProductScalar()
    {
        var columnVector = new ColumnVector<int>([4, 5, 6]);
        var rowVector1 = new RowVector<int>([1, 2, 3]);
        var rowVector2 = new RowVector<int>([4, 5, -6]);

        Assert.That(rowVector1 * columnVector, Is.EqualTo(32));
        Assert.That(rowVector2 * columnVector, Is.EqualTo(5));
    }

    [Test]
    public void Product_WhenColumnRowVector_ShouldReturnOuterProductMatrix()
    {
        var columnVector = new ColumnVector<int>([4, 5, 6]);
        var rowVector1 = new RowVector<int>([1, 2, 3]);
        var rowVector2 = new RowVector<int>([4, 5, -6]);
        var outer1 = columnVector * rowVector1;
        var outer2 = columnVector * rowVector2;
        Assert.That(outer1, Is.EqualTo(new Matrix<int>(new[,] { { 4, 8, 12 }, { 5, 10, 15 }, { 6, 12, 18 } })));
        Assert.That(outer2, Is.EqualTo(new Matrix<int>(new[,] { { 16, 20, -24 }, { 20, 25, -30 }, { 24, 30, -36 } })));
    }

    private static IEnumerable<TestCaseData> AbstractVectors()
    {
        var elements = new[] { 1, 2, 3, 4, 5 };
        yield return new TestCaseData(new RowVector<int>(elements)).Returns(elements).SetArgDisplayNames("RowVector");
        yield return new TestCaseData(new ColumnVector<int>(elements)).Returns(elements).SetArgDisplayNames("ColumnVector");
    }
    
    [TestCaseSource(nameof(AbstractVectors))]
    public IEnumerable<int> GetEnumerator_WhenEnumerateWeakAndStrongTyped_ShouldReturnVectorIndices(AbstractVector<int> vector)
    {
        using var enumerator = vector.GetEnumerator();
        List<int> values = [];
        while (enumerator.MoveNext())
        {
            values.Add(enumerator.Current);
        }

        object?[] basicValues = enumerator.ToWeakTypedEnumerable().ToArray();
        Assert.That(basicValues, Is.EqualTo(values));
        return values;
    }
}