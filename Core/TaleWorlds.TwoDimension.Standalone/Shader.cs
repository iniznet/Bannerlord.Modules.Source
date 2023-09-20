using System;
using System.Numerics;
using System.Text;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.OpenGL;

namespace TaleWorlds.TwoDimension.Standalone
{
	public class Shader
	{
		private Shader(GraphicsContext graphicsContext, int program)
		{
			this._graphicsContext = graphicsContext;
			this._program = program;
		}

		public static Shader CreateShader(GraphicsContext graphicsContext, string vertexShaderCode, string fragmentShaderCode)
		{
			int num = Shader.CompileShaders(vertexShaderCode, fragmentShaderCode);
			return new Shader(graphicsContext, num);
		}

		public static int CompileShaders(string vertexShaderCode, string fragmentShaderCode)
		{
			int num = Opengl32ARB.CreateShaderObject(ShaderType.VertexShader);
			Opengl32ARB.ShaderSource(num, vertexShaderCode);
			Opengl32ARB.CompileShader(num);
			int num2 = -1;
			Opengl32ARB.GetShaderiv(num, 35713, out num2);
			if (num2 != 1)
			{
				int num3 = -1;
				Opengl32ARB.GetShaderiv(num, 35716, out num3);
				int num4 = -1;
				byte[] array = new byte[4096];
				Opengl32ARB.GetShaderInfoLog(num, 4096, out num4, array);
				Encoding.ASCII.GetString(array);
			}
			int num5 = Opengl32ARB.CreateShaderObject(ShaderType.FragmentShader);
			Opengl32ARB.ShaderSource(num5, fragmentShaderCode);
			Opengl32ARB.CompileShader(num5);
			Opengl32ARB.GetShaderiv(num5, 35713, out num2);
			if (num2 != 1)
			{
				int num6 = -1;
				Opengl32ARB.GetShaderiv(num5, 35716, out num6);
				int num7 = -1;
				byte[] array2 = new byte[4096];
				Opengl32ARB.GetShaderInfoLog(num5, 4096, out num7, array2);
				Encoding.ASCII.GetString(array2);
			}
			int num8 = Opengl32ARB.CreateProgramObject();
			Opengl32ARB.AttachShader(num8, num);
			Opengl32ARB.AttachShader(num8, num5);
			Opengl32ARB.LinkProgram(num8);
			Opengl32ARB.GetProgramiv(num8, 35714, out num2);
			if (num2 != 1)
			{
				int num9 = -1;
				Opengl32ARB.GetProgramiv(num8, 35716, out num9);
				int num10 = -1;
				byte[] array3 = new byte[4096];
				Opengl32ARB.GetProgramInfoLog(num8, 4096, out num10, array3);
				Encoding.ASCII.GetString(array3);
			}
			Opengl32ARB.DetachShader(num8, num);
			Opengl32ARB.DetachShader(num8, num5);
			Opengl32ARB.DeleteShader(num);
			Opengl32ARB.DeleteShader(num5);
			return num8;
		}

		public void SetTexture(string name, OpenGLTexture texture)
		{
			if (this._currentTextureUnit == 0)
			{
				Opengl32ARB.ActiveTexture(TextureUnit.Texture0);
			}
			else if (this._currentTextureUnit == 1)
			{
				Opengl32ARB.ActiveTexture(TextureUnit.Texture1);
			}
			Opengl32.BindTexture(Target.Texture2D, (texture != null) ? texture.Id : (-1));
			int uniformLocation = Opengl32ARB.GetUniformLocation(this._program, name);
			Opengl32ARB.Uniform1i(uniformLocation, this._currentTextureUnit);
			this._currentTextureUnit++;
		}

		public void SetColor(string name, Color color)
		{
			int uniformLocation = Opengl32ARB.GetUniformLocation(this._program, name);
			Opengl32ARB.Uniform4f(uniformLocation, color.Red, color.Green, color.Blue, color.Alpha);
		}

		public void Use()
		{
			Opengl32ARB.UseProgram(this._program);
		}

		public void StopUsing()
		{
			this._currentTextureUnit = 0;
			Opengl32ARB.UseProgram(0);
		}

		public void SetMatrix(string name, Matrix4x4 matrix)
		{
			Opengl32ARB.UniformMatrix4fv(Opengl32ARB.GetUniformLocation(this._program, name), 1, false, matrix);
		}

		public void SetBoolean(string name, bool value)
		{
			int uniformLocation = Opengl32ARB.GetUniformLocation(this._program, name);
			Opengl32ARB.Uniform1i(uniformLocation, value ? 1 : 0);
		}

		public void SetFloat(string name, float value)
		{
			int uniformLocation = Opengl32ARB.GetUniformLocation(this._program, name);
			Opengl32ARB.Uniform1f(uniformLocation, value);
		}

		public void SetVector2(string name, Vector2 value)
		{
			int uniformLocation = Opengl32ARB.GetUniformLocation(this._program, name);
			Opengl32ARB.Uniform2f(uniformLocation, value.X, value.Y);
		}

		private GraphicsContext _graphicsContext;

		private int _program;

		private int _currentTextureUnit;
	}
}
