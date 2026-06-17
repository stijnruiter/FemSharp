using System;
using System.Collections.Generic;
using System.IO;

namespace NumericalMath.Geometry.Import.Ply;

public class PlyFile
{
    public PlyHeader Header { get; }
    public Dictionary<string, List<object[]>> Data { get; }

    private PlyFile(PlyHeader header, Dictionary<string, List<object[]>> data)
    {
        Header = header;
        Data = data;
    }

    public static PlyFile Load(string path)
    {
        using var reader = new StreamReader(path);

        var header = ParseHeader(reader);

        if (header.Format != PlyFormat.Ascii)
            throw new NotSupportedException("Binary PLY not yet supported");

        var data = ParseAsciiData(reader, header);
        return new PlyFile(header, data);
    }
    
    private static PlyHeader ParseHeader(StreamReader reader)
    {
        var header = new PlyHeader();
        PlyElement? current = null;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tokens[0] == "end_header")
                break;

            switch (tokens[0])
            {
                case "format":
                    header.Format = tokens[1] switch
                    {
                        "ascii" => PlyFormat.Ascii,
                        "binary_little_endian" => PlyFormat.BinaryLittleEndian,
                        "binary_big_endian" => PlyFormat.BinaryBigEndian,
                        _ => throw new NotSupportedException()
                    };
                    break;

                case "comment":
                    header.Comments.Add(line.Substring(8));
                    break;

                case "element":
                    current = new PlyElement(tokens[1], int.Parse(tokens[2]));
                    header.Elements.Add(current);
                    break;

                case "property":
                    if (current == null)
                        throw new InvalidDataException("Property without element");

                    if (tokens[1] == "list")
                    {
                        current.Properties.Add(
                            new PlyProperty(
                                tokens[4],
                                PlyProperty.MapPlyType(tokens[2]),
                                PlyProperty.MapPlyType(tokens[3])
                            )
                        );
                    }
                    else
                    {
                        current.Properties.Add(
                            new PlyProperty(tokens[2], PlyProperty.MapPlyType(tokens[1]))
                        );
                    }
                    break;
            }
        }

        return header;
    }
    
    private static Dictionary<string, List<object[]>> ParseAsciiData(StreamReader reader, PlyHeader header)
    {
        var result = new Dictionary<string, List<object[]>>();

        foreach (var element in header.Elements)
        {
            var rows = new List<object[]>();

            for (var i = 0; i < element.Count; i++)
            {
                var tokens = reader.ReadLine()!.Split();
                var values = new List<object>();

                var index = 0;
                foreach (var prop in element.Properties)
                {
                    if (prop.IsList)
                    {
                        var count = Convert.ToInt32(tokens[index++]);
                        var list = Array.CreateInstance(prop.DataType, count);
                        for (var j = 0; j < count; j++)
                        {
                            list.SetValue(prop.Parse(tokens[index++]), j);
                        }
                        values.Add(list);
                    }
                    else
                    {
                        values.Add(prop.Parse(tokens[index++]));
                    }
                }

                rows.Add(values.ToArray());
            }

            result[element.Name] = rows;
        }

        return result;
    }

    
}