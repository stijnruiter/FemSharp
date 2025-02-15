using FemSharp.Render;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FemSharp;

internal interface IDrawableObject : IDisposable
{
    public void Update();
    public void Draw(Renderer renderer);
}

internal interface IScene : IDrawableObject
{
    public Keys ActivateKey { get; }
    // TODO: Avoid using window as argument
    public void Activate(Window window);
}