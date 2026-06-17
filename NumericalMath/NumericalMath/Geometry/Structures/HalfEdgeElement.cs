using System;
using System.Collections;
using System.Collections.Generic;

namespace NumericalMath.Geometry.Structures;

public class HalfEdgeElement : IEnumerable<HalfEdge>
{
    private readonly IReadOnlyList<HalfEdge> _edges;
    
    private readonly int _firstFirstEdgeIndex;
    
    public HalfEdgeElement(IReadOnlyList<HalfEdge> halfEdges, int firstEdgeIndex)
    {
        if (firstEdgeIndex < 0 || firstEdgeIndex >= halfEdges.Count)
            throw new IndexOutOfRangeException($"Index {firstEdgeIndex} not in range, Count: {halfEdges.Count}.");
        _edges = halfEdges;
        _firstFirstEdgeIndex = firstEdgeIndex;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<HalfEdge> GetEnumerator() => new HalfEdgeEnumerator(_edges, _firstFirstEdgeIndex);

    private class HalfEdgeEnumerator(IReadOnlyList<HalfEdge> halfEdges, int index) : IEnumerator<HalfEdge>
    {
        public bool MoveNext()
        {
            if (_position == -1)
            {
                _position = _index;
                return _position < _halfEdges.Count;
            }

            _position = _halfEdges[_position].NextEdge;
            return _position != _index;
        }

        public void Reset()
        {
            _position = -1;
        }

        public HalfEdge Current => _halfEdges[_position];

        object? IEnumerator.Current => Current;
        private int _position = -1;
        private readonly IReadOnlyList<HalfEdge> _halfEdges = halfEdges;
        private readonly int _index = index;

        public void Dispose()
        {
        }
    }
}
