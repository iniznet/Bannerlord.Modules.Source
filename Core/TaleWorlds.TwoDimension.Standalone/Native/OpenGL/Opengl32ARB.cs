using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace TaleWorlds.TwoDimension.Standalone.Native.OpenGL
{
	internal static class Opengl32ARB
	{
		public static void LoadExtensions()
		{
			if (!Opengl32ARB._extensionsLoaded)
			{
				Opengl32ARB._extensionsLoaded = true;
				Opengl32ARB.ActiveTexture = Opengl32ARB.LoadFunction<Opengl32ARB.ActiveTextureDelegate>("glActiveTexture");
				Opengl32ARB.wglCreateContextAttribs = Opengl32ARB.LoadFunction<Opengl32ARB.wglCreateContextAttribsDelegate>("wglCreateContextAttribsARB");
				Opengl32ARB.CreateProgramObject = Opengl32ARB.LoadFunction<Opengl32ARB.CreateProgramObjectDelegate>("glCreateProgramObjectARB");
				Opengl32ARB.CreateShaderObject = Opengl32ARB.LoadFunction<Opengl32ARB.CreateShaderObjectDelegate>("glCreateShaderObjectARB");
				Opengl32ARB.CompileShader = Opengl32ARB.LoadFunction<Opengl32ARB.CompileShaderDelegate>("glCompileShaderARB");
				Opengl32ARB.ShaderSourceInternal = Opengl32ARB.LoadFunction<Opengl32ARB.ShaderSourceDelegate>("glShaderSourceARB");
				Opengl32ARB.AttachShader = Opengl32ARB.LoadFunction<Opengl32ARB.AttachShaderDelegate>("glAttachShader");
				Opengl32ARB.LinkProgram = Opengl32ARB.LoadFunction<Opengl32ARB.LinkProgramDelegate>("glLinkProgram");
				Opengl32ARB.DeleteProgram = Opengl32ARB.LoadFunction<Opengl32ARB.DeleteProgramDelegate>("glDeleteProgram");
				Opengl32ARB.UseProgram = Opengl32ARB.LoadFunction<Opengl32ARB.UseProgramDelegate>("glUseProgram");
				Opengl32ARB.UniformMatrix4fvInternal = Opengl32ARB.LoadFunction<Opengl32ARB.UniformMatrix4fvDelegate>("glUniformMatrix4fv");
				Opengl32ARB.Uniform4f = Opengl32ARB.LoadFunction<Opengl32ARB.Uniform4fDelegate>("glUniform4f");
				Opengl32ARB.Uniform1i = Opengl32ARB.LoadFunction<Opengl32ARB.Uniform1iDelegate>("glUniform1i");
				Opengl32ARB.Uniform1f = Opengl32ARB.LoadFunction<Opengl32ARB.Uniform1fDelegate>("glUniform1f");
				Opengl32ARB.Uniform2f = Opengl32ARB.LoadFunction<Opengl32ARB.Uniform2fDelegate>("glUniform2f");
				Opengl32ARB.GetShaderiv = Opengl32ARB.LoadFunction<Opengl32ARB.GetShaderivDelegate>("glGetShaderiv");
				Opengl32ARB.GetShaderInfoLog = Opengl32ARB.LoadFunction<Opengl32ARB.GetShaderInfoLogDelegate>("glGetShaderInfoLog");
				Opengl32ARB.GetProgramInfoLog = Opengl32ARB.LoadFunction<Opengl32ARB.GetProgramInfoLogDelegate>("glGetProgramInfoLog");
				Opengl32ARB.GetProgramiv = Opengl32ARB.LoadFunction<Opengl32ARB.GetProgramivDelegate>("glGetProgramiv");
				Opengl32ARB.GetUniformLocationInternal = Opengl32ARB.LoadFunction<Opengl32ARB.GetUniformLocationDelegate>("glGetUniformLocation");
				Opengl32ARB.DetachShader = Opengl32ARB.LoadFunction<Opengl32ARB.DetachShaderDelegate>("glDetachShader");
				Opengl32ARB.DeleteShader = Opengl32ARB.LoadFunction<Opengl32ARB.DeleteShaderDelegate>("glDeleteShader");
				Opengl32ARB.GenBuffers = Opengl32ARB.LoadFunction<Opengl32ARB.GenBuffersDelegate>("glGenBuffers");
				Opengl32ARB.BindBuffer = Opengl32ARB.LoadFunction<Opengl32ARB.BindBufferDelegate>("glBindBuffer");
				Opengl32ARB.BufferData = Opengl32ARB.LoadFunction<Opengl32ARB.BufferDataDelegate>("glBufferData");
				Opengl32ARB.BufferSubData = Opengl32ARB.LoadFunction<Opengl32ARB.BufferSubDataDelegate>("glBufferSubData");
				Opengl32ARB.EnableVertexAttribArray = Opengl32ARB.LoadFunction<Opengl32ARB.EnableVertexAttribArrayDelegate>("glEnableVertexAttribArray");
				Opengl32ARB.DisableVertexAttribArray = Opengl32ARB.LoadFunction<Opengl32ARB.DisableVertexAttribArrayDelegate>("glDisableVertexAttribArray");
				Opengl32ARB.VertexAttribPointer = Opengl32ARB.LoadFunction<Opengl32ARB.VertexAttribPointerDelegate>("glVertexAttribPointer");
				Opengl32ARB.GenVertexArrays = Opengl32ARB.LoadFunction<Opengl32ARB.GenVertexArraysDelegate>("glGenVertexArrays");
				Opengl32ARB.BindVertexArray = Opengl32ARB.LoadFunction<Opengl32ARB.BindVertexArrayDelegate>("glBindVertexArray");
				Opengl32ARB.BlendFuncSeparate = Opengl32ARB.LoadFunction<Opengl32ARB.BlendFuncSeparateDelegate>("glBlendFuncSeparate");
			}
		}

		private static T LoadFunction<T>(string name) where T : class
		{
			return Marshal.GetDelegateForFunctionPointer(Opengl32.wglGetProcAddress(name), typeof(T)) as T;
		}

		public static void ShaderSource(int shader, string shaderSource)
		{
			string[] array = shaderSource.Split(Environment.NewLine.ToCharArray());
			byte[][] array2 = new byte[array.Length][];
			int[] array3 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				byte[] array4 = new byte[Encoding.UTF8.GetByteCount(text) + 2];
				Encoding.UTF8.GetBytes(text, 0, text.Length, array4, 0);
				array4[array4.Length - 2] = 10;
				array4[array4.Length - 1] = 0;
				array2[i] = array4;
				array3[i] = array4.Length - 1;
			}
			AutoPinner[] array5 = new AutoPinner[array.Length];
			IntPtr[] array6 = new IntPtr[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				AutoPinner autoPinner = new AutoPinner(array2[j]);
				IntPtr intPtr = autoPinner;
				array6[j] = intPtr;
				array5[j] = autoPinner;
			}
			Opengl32ARB.ShaderSourceInternal(shader, array.Length, array6, array3);
			AutoPinner[] array7 = array5;
			for (int k = 0; k < array7.Length; k++)
			{
				array7[k].Dispose();
			}
		}

		public static int GetUniformLocation(int program, string parameter)
		{
			byte[] array = new byte[Encoding.ASCII.GetByteCount(parameter) + 1];
			Encoding.UTF8.GetBytes(parameter, 0, parameter.Length, array, 0);
			return Opengl32ARB.GetUniformLocationInternal(program, array);
		}

		public static void UniformMatrix4fv(int location, int count, bool isTranspose, Matrix4x4 matrix)
		{
			float[] array = new float[]
			{
				matrix.M11, matrix.M12, matrix.M13, matrix.M14, matrix.M21, matrix.M22, matrix.M23, matrix.M24, matrix.M31, matrix.M32,
				matrix.M33, matrix.M34, matrix.M41, matrix.M42, matrix.M43, matrix.M44
			};
			Opengl32ARB.UniformMatrix4fvInternal(location, count, isTranspose ? 1 : 0, array);
		}

		private static bool _extensionsLoaded;

		public static Opengl32ARB.BlendFuncSeparateDelegate BlendFuncSeparate;

		public static Opengl32ARB.ActiveTextureDelegate ActiveTexture;

		public static Opengl32ARB.BindVertexArrayDelegate BindVertexArray;

		public static Opengl32ARB.GenVertexArraysDelegate GenVertexArrays;

		public static Opengl32ARB.VertexAttribPointerDelegate VertexAttribPointer;

		public static Opengl32ARB.EnableVertexAttribArrayDelegate EnableVertexAttribArray;

		public static Opengl32ARB.DisableVertexAttribArrayDelegate DisableVertexAttribArray;

		public static Opengl32ARB.GenBuffersDelegate GenBuffers;

		public static Opengl32ARB.BindBufferDelegate BindBuffer;

		public static Opengl32ARB.BufferDataDelegate BufferData;

		public static Opengl32ARB.BufferSubDataDelegate BufferSubData;

		public static Opengl32ARB.DetachShaderDelegate DetachShader;

		public static Opengl32ARB.DeleteShaderDelegate DeleteShader;

		private static Opengl32ARB.GetUniformLocationDelegate GetUniformLocationInternal;

		public static Opengl32ARB.GetProgramInfoLogDelegate GetProgramInfoLog;

		public static Opengl32ARB.GetShaderInfoLogDelegate GetShaderInfoLog;

		public static Opengl32ARB.GetProgramivDelegate GetProgramiv;

		public static Opengl32ARB.GetShaderivDelegate GetShaderiv;

		private static Opengl32ARB.UniformMatrix4fvDelegate UniformMatrix4fvInternal;

		public static Opengl32ARB.Uniform4fDelegate Uniform4f;

		public static Opengl32ARB.Uniform1iDelegate Uniform1i;

		public static Opengl32ARB.Uniform1fDelegate Uniform1f;

		public static Opengl32ARB.Uniform2fDelegate Uniform2f;

		public static Opengl32ARB.UseProgramDelegate UseProgram;

		public static Opengl32ARB.DeleteProgramDelegate DeleteProgram;

		public static Opengl32ARB.LinkProgramDelegate LinkProgram;

		public static Opengl32ARB.AttachShaderDelegate AttachShader;

		private static Opengl32ARB.ShaderSourceDelegate ShaderSourceInternal;

		public static Opengl32ARB.CompileShaderDelegate CompileShader;

		public static Opengl32ARB.CreateProgramObjectDelegate CreateProgramObject;

		public static Opengl32ARB.CreateShaderObjectDelegate CreateShaderObject;

		public static Opengl32ARB.wglCreateContextAttribsDelegate wglCreateContextAttribs;

		public const int GL_COMPILE_STATUS = 35713;

		public const int GL_LINK_STATUS = 35714;

		public const int GL_INFO_LOG_LENGTH = 35716;

		public const int StaticDraw = 35044;

		public const int DynamicDraw = 35048;

		public delegate void BlendFuncSeparateDelegate(BlendingSourceFactor srcRGB, BlendingDestinationFactor dstRGB, BlendingSourceFactor srcAlpha, BlendingDestinationFactor dstAlpha);

		public delegate void ActiveTextureDelegate(TextureUnit textureUnit);

		public delegate void BindVertexArrayDelegate(uint buffer);

		public delegate void GenVertexArraysDelegate(int size, uint[] buffers);

		public delegate void VertexAttribPointerDelegate(uint index, int size, DataType type, byte normalized, int stride, IntPtr pointer);

		public delegate void EnableVertexAttribArrayDelegate(uint index);

		public delegate void DisableVertexAttribArrayDelegate(int index);

		public delegate void GenBuffersDelegate(int size, uint[] buffers);

		public delegate void BindBufferDelegate(BufferBindingTarget target, uint buffer);

		public delegate void BufferDataDelegate(BufferBindingTarget target, int size, IntPtr data, int usage);

		public delegate void BufferSubDataDelegate(BufferBindingTarget target, int offset, int size, IntPtr data);

		public delegate void DetachShaderDelegate(int program, int shader);

		public delegate int DeleteShaderDelegate(int shader);

		private delegate int GetUniformLocationDelegate(int program, byte[] parameter);

		public delegate void GetProgramInfoLogDelegate(int shader, int maxLength, out int length, byte[] output);

		public delegate void GetShaderInfoLogDelegate(int shader, int maxLength, out int length, byte[] output);

		public delegate void GetProgramivDelegate(int program, int paremeterName, out int returnValue);

		public delegate void GetShaderivDelegate(int shader, int paremeterName, out int returnValue);

		private delegate void UniformMatrix4fvDelegate(int location, int count, byte isTranspose, float[] matrix);

		public delegate void Uniform4fDelegate(int location, float f1, float f2, float f3, float f4);

		public delegate void Uniform1iDelegate(int location, int i);

		public delegate void Uniform1fDelegate(int location, float f);

		public delegate void Uniform2fDelegate(int location, float f1, float f2);

		public delegate void UseProgramDelegate(int program);

		public delegate void DeleteProgramDelegate(int program);

		public delegate void LinkProgramDelegate(int program);

		public delegate void AttachShaderDelegate(int program, int shader);

		private delegate void ShaderSourceDelegate(int shader, int count, IntPtr[] shaderSource, int[] length);

		public delegate int CompileShaderDelegate(int shader);

		public delegate int CreateProgramObjectDelegate();

		public delegate int CreateShaderObjectDelegate(ShaderType shaderType);

		public delegate IntPtr wglCreateContextAttribsDelegate(IntPtr hDC, IntPtr hShareContext, int[] attribList);
	}
}
