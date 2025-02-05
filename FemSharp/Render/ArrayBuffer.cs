using OpenTK.Graphics.OpenGL4;
using System.Runtime.CompilerServices;

namespace FemSharp.Render;

internal class ArrayBuffer<T> : IDisposable where T : struct
{
    public BufferUsageHint BufferUsageHint { get; private set; }
    
    public BufferTarget BufferTarget { get; private set; }

    public int Handle { get; private set; }

    public int Length { get; private set; }

    public int Size { get; private set; }

    public int Count { get; private set; }
    
    public ArrayBuffer(BufferTarget target, T[] data, BufferUsageHint bufferUsage = BufferUsageHint.StaticDraw)
    {
        BufferTarget = target;
        Handle = GL.GenBuffer();
        SetData(data, bufferUsage);
    }

    public void Bind()
    {
        GL.BindBuffer(BufferTarget, Handle);
    }

    public void SetData(T[] data)
    {
        Bind();
        Length = data.Length;
        Size = data.Length * Unsafe.SizeOf<T>();
        // TODO: Beter way to do this
        Count = Size / typeof(T).GetProperties().Count();
        GL.BufferData(BufferTarget, Size, data, BufferUsageHint);
    }

    public void SetData(T[] data, BufferUsageHint bufferUsage)
    {
        BufferUsageHint = bufferUsage;
        SetData(data);
    }

    public void Unbind()
    {
        GL.BindBuffer(BufferTarget, 0);
    }

    public void Dispose()
    {
        Unbind();
        GL.DeleteBuffer(Handle);
        Handle = 0;
        Length = 0;
        Size = 0;
        Count = 0;
    }
}