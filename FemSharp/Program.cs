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
        var mesh = new Mesh2D(vertices, interior, boundary);
        using Window window = new Window(800, 600, "FEM Sharp");
        window.AddMesh(mesh);
        window.KeyDown += (OpenTK.Windowing.Common.KeyboardKeyEventArgs obj) =>
        {
            // Just to check if updating works
            switch(obj.Key)
            {
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left:
                    vertices[0].X -= 0.01f;
                    break;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right:
                    vertices[0].X += 0.01f;
                    break;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up:
                    vertices[0].Y += 0.01f;
                    break;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down:
                    vertices[0].Y -= 0.01f;
                    break;
            }
        };
        window.Run();
    }
}