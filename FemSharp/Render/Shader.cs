using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace FemSharp.Render;

internal class ShaderSource : IDisposable
{
    public int Handle { get; private set; }
    
    public ShaderType Type { get; private set; }

    private ShaderSource()
    {
    }

    public static ShaderSource Compile(ShaderType type, string source)
    {
        var handle = GL.CreateShader(type);
        GL.ShaderSource(handle, source);
        GL.CompileShader(handle);
        GL.GetShader(handle, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string compileInfo = GL.GetShaderInfoLog(handle);
            GL.DeleteShader(handle);
            throw new Exception(compileInfo);
        }
        return new ShaderSource { Handle = handle, Type = type };
    }

    public void Attach(int shaderProgram)
    {
        GL.AttachShader(shaderProgram, Handle);
    }

    public void Detach(int shaderProgram)
    {
        GL.DetachShader(shaderProgram, Handle);
    }

    public void Dispose()
    {
        GL.DeleteShader(Handle);
    }
}

[DebuggerDisplay("{_vertexFileName}, {_fragmentFileName}")]
internal class Shader : IDisposable
{
    public int Handle { get; private set; }

    private Shader(string vertexFileName, string fragmentFileName)
    {
        _vertexFileName = vertexFileName;
        _fragmentFileName = fragmentFileName;
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public static Shader Create(string vertexShader, string fragmentShader)
    {
        var vertexSource = EmbeddedLoader.Load(vertexShader);
        var fragmentSource = EmbeddedLoader.Load(fragmentShader);

        using var vert = ShaderSource.Compile(ShaderType.VertexShader, vertexSource);
        using var frag = ShaderSource.Compile(ShaderType.FragmentShader, fragmentSource);

        var shaderHandle = GL.CreateProgram();
        vert.Attach(shaderHandle);
        frag.Attach(shaderHandle);
        GL.LinkProgram(shaderHandle);

        GL.GetProgram(shaderHandle, GetProgramParameterName.LinkStatus, out var success);
        if (success == 0)
        {
            string linkInfo = GL.GetProgramInfoLog(shaderHandle);
            GL.DeleteProgram(shaderHandle);
            throw new Exception(linkInfo);
        }
        vert.Detach(shaderHandle);
        frag.Detach(shaderHandle);
        return new Shader(vertexShader, fragmentShader) { Handle = shaderHandle };
    }

    public int GetUniformLocation(string uniformName)
    {
        return GL.GetUniformLocation(Handle, uniformName);
    }

    public bool SetUniform1<T>(string uniformName, T value) where T : struct
    {
        var location = GetUniformLocation(uniformName);
        if (location < 0)
            return false;

        if (value is double doubleValue)
        {
            GL.Uniform1(location, doubleValue);
            return true;
        }
        if (value is float floatValue)
        {
            GL.Uniform1(location, floatValue);
            return true;
        }
        if (value is int intValue)
        {
            GL.Uniform1(location, intValue);
            return true;
        }
        if (value is uint uintValue)
        {
            GL.Uniform1(location, uintValue);
            return true;
        }
        throw new NotImplementedException();
    }


    public bool SetColor(string uniformName, Color4 value)
    {
        var location = GetUniformLocation(uniformName);
        if (location < 0)
        {
            Debug.Fail($"ShaderLocation {uniformName} not found.");
            return false;
        }

        Use();
        GL.Uniform4(location, value);
        return true;
    }

    public void Dispose()
    {
        GL.DeleteShader(Handle);
        Handle = 0;
    }

    internal bool SetMatrix4(string uniformName, Matrix4 view)
    {
        var location = GetUniformLocation(uniformName);
        if (location < 0)
        {
            Debug.Fail($"ShaderLocation {uniformName} not found.");
            return false;
        }

        Use();
        GL.UniformMatrix4(location, true, ref view);
        return true;
    }

    private readonly string _vertexFileName;
    private readonly string _fragmentFileName;
}