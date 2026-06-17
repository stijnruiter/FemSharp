using System;

namespace NumericalMath.Geometry.Structures;

public struct Triangle(Vertex2 v1, Vertex2 v2, Vertex2 v3)
{
    public Vertex2 V1 = v1;
    public Vertex2 V2 = v2;
    public Vertex2 V3 = v3;

    public static bool Contains(Vertex2 p, Vertex2 v1, Vertex2 v2, Vertex2 v3)
    {
        var triangle = new Triangle(v1, v2, v3);
        return triangle.Contains(p);
    }

    public readonly bool Contains(Vertex2 vertex)
    {
        var d1 = HalfPlaneSide(vertex, V1, V2);
        var d2 = HalfPlaneSide(vertex, V2, V3);
        var d3 = HalfPlaneSide(vertex, V3, V1);

        var has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        var has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    public (float A1, float A2, float A3) GetAngles()
    {
        var a1 = (V2 - V1).Normalized();
        var a2 = (V3 - V1).Normalized();
        var angle1 = MathF.Acos(Vertex2.DotProduct(a1, a2));
        var b1 = (V1 - V2).Normalized();
        var b2 = (V3 - V2).Normalized();
        var angle2 = MathF.Acos(Vertex2.DotProduct(b1, b2));
        var angle3 = MathF.PI - angle1 - angle2;
        
        return (angle1, angle2, angle3);
    }

    public float GetSmallestAngle()
    {
        var angles = GetAngles();
        return MathF.Min(MathF.Min(angles.A1, angles.A2), angles.A3);
    }

    private static float HalfPlaneSide(Vertex2 p1, Vertex2 p2, Vertex2 p3)
    {
        return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
    }

    public static Triangle ContainingTriangle(ReadOnlySpan<Vertex2> vertices, float dilate = 0f)
    {
        var boundingBox = Rectangle.BoundingBox(vertices, dilate);
        var v1 = new Vertex2(boundingBox.Left, boundingBox.Bottom);
        var v2 = new Vertex2(boundingBox.Width + boundingBox.Height, boundingBox.Bottom);
        var v3 = new Vertex2(boundingBox.Left, boundingBox.Width + boundingBox.Height);
        return new Triangle(v1, v2, v3);
    }

    public Vertex2 Circumcenter()
    {
        var lengthSquared1 = (V1.X * V1.X + V1.Y * V1.Y);
        var lengthSquared2 = (V2.X * V2.X + V2.Y * V2.Y);
        var lengthSquared3 = (V3.X * V3.X + V3.Y * V3.Y);

        return new Vertex2(
            (lengthSquared1 * (V2.Y - V3.Y) + lengthSquared2 * (V3.Y - V1.Y) + lengthSquared3 * (V1.Y - V2.Y))
                / (2 * (V1.X * (V2.Y - V3.Y) + V2.X * (V3.Y - V1.Y) + V3.X * (V1.Y - V2.Y))),
            (lengthSquared1 * (V2.X - V3.X) + lengthSquared2 * (V3.X - V1.X) + lengthSquared3 * (V1.X - V2.X))
                / (2 * (V1.Y * (V2.X - V3.X) + V2.Y * (V3.X - V1.X) + V3.Y * (V1.X - V2.X))));
    }

    public override string ToString() => $"[{V1}, {V2}, {V3}]";
}
