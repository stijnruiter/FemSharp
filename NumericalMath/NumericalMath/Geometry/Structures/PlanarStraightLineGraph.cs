using System;
using System.Collections.Generic;
using System.Linq;

namespace NumericalMath.Geometry.Structures;

public class PlanarStraightLineGraph()
{
    private readonly List<Vertex2> _vertices = [];
    private readonly List<LineElement> _lineSegments = [];

    public IReadOnlyList<Vertex2> Vertices => _vertices;
    public IReadOnlyList<LineElement> Segments => _lineSegments;

    public void AddLineSegment(Vertex2 start, Vertex2 end)
    {
        _vertices.Add(start);
        _vertices.Add(end);
        _lineSegments.Add(new LineElement(_vertices.Count - 2, _vertices.Count - 1));
    }

    public void AddLineSegments(params Vertex2[] polygon)
    {
        if (polygon.Length <= 1)
            throw new ArgumentException("The polygon must have at least two vertices.");

        var startIndex = _vertices.Count;
        _vertices.AddRange(polygon);
        _lineSegments.AddRange(Enumerable.Range(startIndex, polygon.Length - 1)
            .Select(start => new LineElement(start, start + 1)));
    }
    
    public void AddClosedLineSegments(params Vertex2[] polygon)
    {
        AddLineSegments(polygon);
        // Connect the last vertex to the first vertex to close the polygon loop
        _lineSegments.Add(new LineElement(_vertices.Count - 1, _vertices.Count - polygon.Length));
    }
    
    public void RemoveLineSegment(int lineSegmentIndex) 
        => _lineSegments.RemoveAt(lineSegmentIndex);

    public void SplitLineSegment(int lineSegmentIndex, float alpha = 0.5f)
    {
        var lineSegment = _lineSegments[lineSegmentIndex];
        var startVertex = _vertices[lineSegment.I];
        var endVertex = _vertices[lineSegment.J];
        
        var splitVertexIndex = _vertices.Count;
        var splitVertex = startVertex + alpha * (endVertex - startVertex);
        _vertices.Add(splitVertex);
        RemoveLineSegment(lineSegmentIndex);
        _lineSegments.Add(new LineElement(lineSegment.I, splitVertexIndex));
        _lineSegments.Add(new LineElement(splitVertexIndex, lineSegment.J));
    }
}