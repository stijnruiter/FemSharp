using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace FemSharp.Render
{
    internal class Axis : IDrawableObject
    {
        public Axis()
        {
            _vertexArray = new VertexArray();
            _dataBuffer = new ArrayBuffer<ColoredVertex>(BufferTarget.ArrayBuffer, _vertices);
            _elementBuffer = new ArrayBuffer<LineElement>(BufferTarget.ElementArrayBuffer, _lines);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<ColoredVertex>(), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, Unsafe.SizeOf<ColoredVertex>(), sizeof(float) * 3);
            GL.EnableVertexAttribArray(1);
        }

        public void Dispose()
        {
            _vertexArray.Dispose();
            _dataBuffer.Dispose();
        }

        public void Draw(Renderer renderer)
        {
            _vertexArray.Bind();
            renderer.UseVertexColor();
            renderer.SetLineWidth(4);
            renderer.DrawLines(_elementBuffer);
        }

        public void Update()
        {
        }

        private readonly ColoredVertex[] _vertices = [
            new(new(-5,  0,  0), Color4.Red), new(new(5, 0,  0), Color4.Red),
            new(new( 0, -5,  0), Color4.Green), new(new(0, 5, 0), Color4.Green),
            new(new( 0,  0, -5), Color4.Blue), new(new(0, 0, 5), Color4.Blue)
        ];
        private readonly LineElement[] _lines = [
            new(0, 1),
            new(2, 3),
            new(4, 5)
            ];

        private readonly VertexArray _vertexArray;
        private readonly ArrayBuffer<ColoredVertex> _dataBuffer;
        private readonly ArrayBuffer<LineElement> _elementBuffer;
    }
}
