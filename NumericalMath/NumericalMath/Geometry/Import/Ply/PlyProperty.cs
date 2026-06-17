using System;
using System.Globalization;

namespace NumericalMath.Geometry.Import.Ply;

public sealed class PlyProperty
{
    public string Name { get; }
    public Type DataType { get; }
    public bool IsList { get; }
    public Type? CountType { get; }

    public PlyProperty(string name, Type dataType)
    {
        Name = name;
        DataType = dataType;
        IsList = false;
    }

    public PlyProperty(string name, Type countType, Type dataType)
    {
        Name = name;
        CountType = countType;
        DataType = dataType;
        IsList = true;
    }
    
    public static Type MapPlyType(string plyType) => plyType switch
    {
        "char" or "int8"      => typeof(sbyte),
        "uchar" or "uint8"    => typeof(byte),
        "short" or "int16"    => typeof(short),
        "ushort" or "uint16"  => typeof(ushort),
        "int" or "int32"      => typeof(int),
        "uint" or "uint32"    => typeof(uint),
        "float" or "float32"  => typeof(float),
        "double" or "float64" => typeof(double),
        _ => throw new NotSupportedException($"Unsupported PLY type: {plyType}")
    };

    public object Parse(string token)
    {
        if (DataType == typeof(float)) return float.Parse(token, CultureInfo.InvariantCulture);
        if (DataType == typeof(double)) return double.Parse(token, CultureInfo.InvariantCulture);
        if (DataType == typeof(decimal)) return decimal.Parse(token, CultureInfo.InvariantCulture);
        if (DataType == typeof(int)) return int.Parse(token, CultureInfo.InvariantCulture);
        if (DataType == typeof(uint)) return uint.Parse(token, CultureInfo.InvariantCulture);
        if (DataType == typeof(short)) return short.Parse(token, CultureInfo.InvariantCulture);
        if (DataType == typeof(ushort)) return ushort.Parse(token, CultureInfo.InvariantCulture);
        if (DataType == typeof(byte)) return byte.Parse(token, CultureInfo.InvariantCulture);
        if (DataType == typeof(sbyte)) return sbyte.Parse(token, CultureInfo.InvariantCulture);
        throw new NotSupportedException($"Unsupported PLY type: {DataType}");
    
    }

}
