using FemSharp.Render;
using NumericalMath.Geometry.Structures;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using NumericalMath.Geometry;

namespace FemSharp.Scenes
{
    internal class DelaunayScene : IScene
    {
        public Keys ActivateKey { get; }
        public Mesh2D Mesh { get; }
        public PlanarStraightLineGraph PSLG { get; }
        public DrawableLines _drawableLines { get; }
        public DrawableMesh2D _drawableMesh2D { get; }
        public Axis _axis { get; }

        public DelaunayScene(Keys key, Rectangle bounds, int n)
        {
            ActivateKey = key;
            Vertex2[] vertices =
            [
                new(-0.822222222f,  0.862222222f),
                new(0.257777778f,  0.76f),
                new(0.057777778f,  0.053333333f),
                new(0.333333333f, -0.235555556f),
                new(0.502222222f,  0.626666667f),
                new(0.764444444f, -0.075555556f),
                new(0.466666667f, -0.791111111f),
                new(0.155555556f, -0.871111111f),
                new(-0.177777778f, -0.866666667f),
                new(-0.497777778f, -0.684444444f),
                new(0.04f, -0.457777778f),
                new(0.022222222f, -0.395555556f),
                new(-0.773333333f, -0.035555556f),
            ];
            PSLG = new PlanarStraightLineGraph();
            
            PSLG.AddClosedLineSegments(vertices);
            PSLG.AddLineSegments(new Vertex2(-0.6f, 0.5f), new Vertex2(-0.5f, 0.5f), new Vertex2(-0.3f, 0.3f));
            var delaunay = RefinedDelaunay.CreateTriangulation(PSLG);
            delaunay.Refine(25);
            var triangulation = delaunay.ToMesh();
            
            Mesh = new Mesh2D(triangulation.Vertices.Select(v => new ValuedVertex(v)).ToArray(), triangulation.Interior.ToArray(), triangulation.Boundary.ToArray());
            _drawableMesh2D = new DrawableMesh2D(Mesh);
            _drawableLines = new DrawableLines(PSLG);
            _axis = new Axis();
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
            _drawableLines.Draw(renderer);
        }

        public void Update()
        {
        }
    }
}
