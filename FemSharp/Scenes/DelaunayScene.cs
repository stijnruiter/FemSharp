using FemSharp.Render;
using NumericalMath.Geometry.Structures;
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
            Vertex2[] vertices = RendomVertices(bounds, n);
            Mesh = MeshGenerator.DelaunayTriangulation(vertices);
            Debug.Assert(Mesh.Vertices.Length == n);
            _drawableMesh2D = new DrawableMesh2D(Mesh);
            _axis = new Axis();
        }

        private Vertex2[] RendomVertices(Rect bounds, int n)
        {
            var random = new Random();
            var vertices = new Vertex2[n];
            for (int i = 0; i < n; i++)
            {
                vertices[i] = new Vertex2(bounds.Left + random.NextSingle() * bounds.Width, bounds.Bottom + random.NextSingle() * bounds.Height);
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
