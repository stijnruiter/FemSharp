using FemSharp.Render;

namespace FemSharp;

internal interface IDrawableObject : IDisposable
{
    public void Draw(Renderer renderer);
}