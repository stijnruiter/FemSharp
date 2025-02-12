using FemSharp.Render;

namespace FemSharp;

internal class MeshGenerator
{
    public static Mesh2D NaiveRectangle(Rect rect, uint nx, uint ny)
    {
        List<ValuedVertex> vertices = [];
        List<TriangularElement> interiorElements = [];
        List<LineElement> boundaryElements = [];

        float dx = rect.Width / (nx);
        float dy = rect.Height / (ny);

        for (uint j = 0; j <= ny; j++)
        {
            for (uint i = 0; i <= nx; i++)
            {
                var x = rect.Left + dx * i;
                var y = rect.Bottom + dy * j;
                vertices.Add(new ValuedVertex(x, y, 0f, 0f));
            }
        }

        for (uint j = 0; j < ny; j++)
        {
            for (uint i = 0; i < nx; i++)
            {
                // 3--2
                // |  |
                // 0--1

                var corner0 = i + (nx + 1) * j;
                var corner1 = i + 1 + (nx + 1) * j;
                var corner2 = i + 1 + (nx + 1) * (j + 1);
                var corner3 = i + (nx + 1) * (j + 1);

                interiorElements.Add(new TriangularElement(corner0, corner1, corner2));
                interiorElements.Add(new TriangularElement(corner0, corner2, corner3));
            }
        }

        for (uint i = 0; i < nx; i++)
        {
            boundaryElements.Add(new LineElement(i, i + 1));
            boundaryElements.Add(new LineElement(i + (nx + 1) * ny, i + 1 + (nx + 1) * ny));
        }

        for (uint j = 0; j < ny; j++)
        {
            //boundaryElements.Add(new LineElement(i, i + 1));
            boundaryElements.Add(new LineElement((nx + 1) * j, (nx + 1) * (j + 1)));
            boundaryElements.Add(new LineElement((nx + 1) * j + nx, (nx + 1) * (j + 1) + nx));
        }

        return new Mesh2D(vertices.ToArray(), interiorElements.ToArray(), boundaryElements.ToArray());
    }
}
