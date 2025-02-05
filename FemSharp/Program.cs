namespace FemSharp;

internal static class Program
{
    private static void Main(string[] args)
    {
        using Window window = new Window(800, 600, "FEM Sharp");
        window.Run();
    }
}