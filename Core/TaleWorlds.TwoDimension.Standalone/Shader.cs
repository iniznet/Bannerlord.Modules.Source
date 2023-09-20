using System;
using System.Numerics;
using System.Text;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.OpenGL;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x02000009 RID: 9
	public class Shader
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00003D37 File Offset: 0x00001F37
		private Shader(GraphicsContext graphicsContext, int program)
		{
			this._graphicsContext = graphicsContext;
			this._program = program;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003D50 File Offset: 0x00001F50
		public static Shader CreateShader(GraphicsContext graphicsContext, string vertexShaderCode, string fragmentShaderCode)
		{
			int num = Shader.CompileShaders(vertexShaderCode, fragmentShaderCode);
			return new Shader(graphicsContext, num);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003D6C File Offset: 0x00001F6C
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

		// Token: 0x06000071 RID: 113 RVA: 0x00003F34 File Offset: 0x00002134
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

		// Token: 0x06000072 RID: 114 RVA: 0x00003FB4 File Offset: 0x000021B4
		public void SetColor(string name, Color color)
		{
			int uniformLocation = Opengl32ARB.GetUniformLocation(this._program, name);
			Opengl32ARB.Uniform4f(uniformLocation, color.Red, color.Green, color.Blue, color.Alpha);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003FF1 File Offset: 0x000021F1
		public void Use()
		{
			Opengl32ARB.UseProgram(this._program);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004003 File Offset: 0x00002203
		public void StopUsing()
		{
			this._currentTextureUnit = 0;
			Opengl32ARB.UseProgram(0);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004017 File Offset: 0x00002217
		public void SetMatrix(string name, Matrix4x4 matrix)
		{
			Opengl32ARB.UniformMatrix4fv(Opengl32ARB.GetUniformLocation(this._program, name), 1, false, matrix);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004030 File Offset: 0x00002230
		public void SetBoolean(string name, bool value)
		{
			int uniformLocation = Opengl32ARB.GetUniformLocation(this._program, name);
			Opengl32ARB.Uniform1i(uniformLocation, value ? 1 : 0);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000405C File Offset: 0x0000225C
		public void SetFloat(string name, float value)
		{
			int uniformLocation = Opengl32ARB.GetUniformLocation(this._program, name);
			Opengl32ARB.Uniform1f(uniformLocation, value);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00004084 File Offset: 0x00002284
		public void SetVector2(string name, Vector2 value)
		{
			int uniformLocation = Opengl32ARB.GetUniformLocation(this._program, name);
			Opengl32ARB.Uniform2f(uniformLocation, value.X, value.Y);
		}

		// Token: 0x0400003A RID: 58
		private GraphicsContext _graphicsContext;

		// Token: 0x0400003B RID: 59
		private int _program;

		// Token: 0x0400003C RID: 60
		private int _currentTextureUnit;
	}
}
