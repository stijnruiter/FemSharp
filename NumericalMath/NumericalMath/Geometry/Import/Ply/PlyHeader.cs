using System;
using System.Collections.Generic;
using System.IO;

namespace NumericalMath.Geometry.Import.Ply;

public sealed class PlyHeader
{
    public PlyFormat Format { get; set; }
    public List<PlyElement> Elements { get; } = new();
    public List<string> Comments { get; } = new();
    

}
