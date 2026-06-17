using System.Collections.Generic;

namespace NumericalMath.Geometry.Import.Ply;

public sealed class PlyElement(string name, int count)
{
    public string Name { get; } = name;
    public int Count { get; } = count;
    public List<PlyProperty> Properties { get; } = new();
}
