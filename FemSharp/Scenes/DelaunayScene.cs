using FemSharp.Render;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace FemSharp.Scenes
{
    internal class DelaunayScene : IScene
    {
        public Keys ActivateKey { get; }
        public Mesh2D Mesh { get; }
        public DrawableMesh2D _drawableMesh2D { get; }
        public Axis _axis { get; }

        public DelaunayScene(Keys key, Rect bounds, int n)
        {
            ActivateKey = key;
            var vertices = RendomVertices(bounds, n);
            Mesh = MeshGenerator.DelaunayTriangulation(vertices);
            Debug.Assert(Mesh.Vertices.Length == n);
            _drawableMesh2D = new DrawableMesh2D(Mesh);
            _axis = new Axis();
        }

        private Vertex[] RendomVertices(Rect bounds, int n)
        {
            var random = new Random();
            var vertices = new Vertex[n];
            for (int i = 0; i < n; i++)
            {
                vertices[i] = new Vertex(bounds.Left + random.NextSingle() * bounds.Width, bounds.Bottom + random.NextSingle() * bounds.Height, 0f);
            }
            return vertices;
        }

        public void Activate(Window window)
        {
            window.Renderer.SetLineWidth(3);
            window.Renderer.DisableDepthTest();
            window.ResetCameraPosition(new Vector3(0, 0, 2.5f), Vector3.UnitY);
        }

        public void Dispose()
        {
            _drawableMesh2D.Dispose();
            _axis.Dispose();
        }

        public void Draw(Renderer renderer)
        {
            _drawableMesh2D.Draw(renderer);
            _axis.Draw(renderer);
        }

        public void Update()
        {
        }
    }
}
