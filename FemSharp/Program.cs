namespace FemSharp;

internal static class Program
{
    private static void Main(string[] args)
    {
        var vertices = new Vertex[]
        {
            new(-0.5f, -0.5f, 0.0f),
            new( 0.5f, -0.5f, 0.0f),
            new( 0.5f,  0.5f, 0.0f),
            new(-0.5f,  0.5f, 0.0f),
        };

        var interior = new TriangularElement[]
        {
            new(0, 1, 2),
            new(0, 2, 3)
        };

        var boundary = new LineElement[]
        {
            new(0, 1),
            new(1, 2),
            new(2, 3),
            new(0, 3),
        };

        using Window window = new Window(800, 600, "FEM Sharp");
        window.DrawableObjects.Add(new Mesh2D(vertices, interior, boundary));
        window.Run();
    }
}