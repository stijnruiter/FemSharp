using FemSharp.Render;

namespace FemSharp;

internal interface IDrawableObject : IDisposable
{
    public void Update();
    public void Draw(Renderer renderer);
}