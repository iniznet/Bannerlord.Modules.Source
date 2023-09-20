using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace TaleWorlds.TwoDimension.Standalone.Native.OpenGL
{
	// Token: 0x02000027 RID: 39
	[SuppressUnmanagedCodeSecurity]
	internal static class Opengl32
	{
		// Token: 0x0600011D RID: 285
		[DllImport("Opengl32.dll", EntryPoint = "glDrawArrays")]
		public static extern void DrawArrays(BeginMode mode, int first, int count);

		// Token: 0x0600011E RID: 286
		[DllImport("Opengl32.dll", EntryPoint = "glGetIntegerv")]
		public static extern void GetInteger(Target target, int[] parameters);

		// Token: 0x0600011F RID: 287
		[DllImport("Opengl32.dll", EntryPoint = "glScissor")]
		public static extern void Scissor(int x, int y, int width, int height);

		// Token: 0x06000120 RID: 288
		[DllImport("Opengl32.dll", EntryPoint = "glGetString")]
		private static extern IntPtr GetStringInner(uint name);

		// Token: 0x06000121 RID: 289 RVA: 0x00004E24 File Offset: 0x00003024
		public static string GetString(uint name)
		{
			new ASCIIEncoding();
			return Marshal.PtrToStringAnsi(Opengl32.GetStringInner(name));
		}

		// Token: 0x06000122 RID: 290
		[DllImport("Opengl32.dll", EntryPoint = "glPixelStorei")]
		public static extern void PixelStore(Target pname, int param);

		// Token: 0x06000123 RID: 291
		[DllImport("Opengl32.dll", EntryPoint = "glPixelZoom")]
		public static extern void PixelZoom(float xfactor, float yfactor);

		// Token: 0x06000124 RID: 292
		[DllImport("Opengl32.dll", EntryPoint = "glReadPixels")]
		public static extern void ReadPixels(int x, int y, int width, int height, PixelFormat format, DataType type, byte[] pixels);

		// Token: 0x06000125 RID: 293
		[DllImport("Opengl32.dll", EntryPoint = "glCopyPixels")]
		public static extern void CopyPixels(int x, int y, int width, int height, PixelFormat type);

		// Token: 0x06000126 RID: 294
		[DllImport("Opengl32.dll", EntryPoint = "glDeleteTextures")]
		public static extern void DeleteTextures(int n, int[] textures);

		// Token: 0x06000127 RID: 295
		[DllImport("Opengl32.dll", EntryPoint = "glBlendFunc")]
		public static extern void BlendFunc(BlendingSourceFactor sfactor, BlendingDestinationFactor dfactor);

		// Token: 0x06000128 RID: 296
		[DllImport("Opengl32.dll", EntryPoint = "glViewport")]
		public static extern void Viewport(int x, int y, int width, int height);

		// Token: 0x06000129 RID: 297
		[DllImport("Opengl32.dll", EntryPoint = "glMultMatrixf")]
		public static extern void MultMatrix(float[] matrix);

		// Token: 0x0600012A RID: 298
		[DllImport("Opengl32.dll", EntryPoint = "glLoadMatrixf")]
		public static extern void LoadMatrix(float[] matrix);

		// Token: 0x0600012B RID: 299
		[DllImport("Opengl32.dll", EntryPoint = "glLoadMatrixf")]
		public static extern void LoadMatrix(ref Matrix4x4 matrix);

		// Token: 0x0600012C RID: 300
		[DllImport("Opengl32.dll", EntryPoint = "glColorMaterial")]
		public static extern void ColorMaterial(uint face, uint mode);

		// Token: 0x0600012D RID: 301
		[DllImport("Opengl32.dll", EntryPoint = "glTexCoordPointer")]
		public static extern void TexCoordPointer(int size, DataType type, int stride, float[] vertexData);

		// Token: 0x0600012E RID: 302
		[DllImport("Opengl32.dll", EntryPoint = "glVertexPointer")]
		public static extern void VertexPointer(int size, DataType type, int stride, float[] vertexData);

		// Token: 0x0600012F RID: 303
		[DllImport("Opengl32.dll", EntryPoint = "glNormalPointer")]
		public static extern void NormalPointer(DataType type, int stride, float[] normalData);

		// Token: 0x06000130 RID: 304
		[DllImport("Opengl32.dll", EntryPoint = "glDrawElements")]
		public static extern void DrawElements(BeginMode mode, int count, DataType type, uint[] indices);

		// Token: 0x06000131 RID: 305
		[DllImport("Opengl32.dll", EntryPoint = "glDisableClientState")]
		public static extern void DisableClientState(uint array);

		// Token: 0x06000132 RID: 306
		[DllImport("Opengl32.dll", EntryPoint = "glEnableClientState")]
		public static extern void EnableClientState(uint array);

		// Token: 0x06000133 RID: 307
		[DllImport("Opengl32.dll", EntryPoint = "glTranslatef")]
		public static extern void Translate(float x, float y, float z);

		// Token: 0x06000134 RID: 308
		[DllImport("Opengl32.dll", EntryPoint = "glLightfv")]
		public static extern void Lightfv(uint light, uint pname, float[] parameters);

		// Token: 0x06000135 RID: 309
		[DllImport("Opengl32.dll", EntryPoint = "glHint")]
		public static extern void Hint(uint target, uint mode);

		// Token: 0x06000136 RID: 310
		[DllImport("Opengl32.dll", EntryPoint = "glMatrixMode")]
		public static extern void MatrixMode(MatrixMode mode);

		// Token: 0x06000137 RID: 311
		[DllImport("Opengl32.dll", EntryPoint = "glDepthFunc")]
		public static extern void DepthFunc(uint mode);

		// Token: 0x06000138 RID: 312
		[DllImport("Opengl32.dll", EntryPoint = "glShadeModel")]
		public static extern void ShadeModel(ShadingModel func);

		// Token: 0x06000139 RID: 313
		[DllImport("Opengl32.dll", EntryPoint = "glClearDepth")]
		public static extern void ClearDepth(double depth);

		// Token: 0x0600013A RID: 314
		[DllImport("Opengl32.dll", EntryPoint = "glPopMatrix")]
		public static extern void PopMatrix();

		// Token: 0x0600013B RID: 315
		[DllImport("Opengl32.dll", EntryPoint = "glPushMatrix")]
		public static extern void PushMatrix();

		// Token: 0x0600013C RID: 316
		[DllImport("Opengl32.dll", EntryPoint = "glRotated")]
		public static extern void Rotate(double angle, double x, double y, double z);

		// Token: 0x0600013D RID: 317
		[DllImport("Opengl32.dll", EntryPoint = "glRotatef")]
		public static extern void Rotate(float angle, float x, float y, float z);

		// Token: 0x0600013E RID: 318
		[DllImport("Opengl32.dll", EntryPoint = "glScaled")]
		public static extern void Scale(double x, double y, double z);

		// Token: 0x0600013F RID: 319
		[DllImport("Opengl32.dll", EntryPoint = "glScalef")]
		public static extern void Scale(float x, float y, float z);

		// Token: 0x06000140 RID: 320
		[DllImport("Opengl32.dll", EntryPoint = "glLoadIdentity")]
		public static extern void LoadIdentity();

		// Token: 0x06000141 RID: 321
		[DllImport("Opengl32.dll", EntryPoint = "glClear")]
		public static extern void Clear(AttribueMask mask);

		// Token: 0x06000142 RID: 322
		[DllImport("Opengl32.dll", EntryPoint = "glBegin")]
		public static extern void Begin(BeginMode mode);

		// Token: 0x06000143 RID: 323
		[DllImport("Opengl32.dll", EntryPoint = "glEnd")]
		public static extern void End();

		// Token: 0x06000144 RID: 324
		[DllImport("Opengl32.dll", EntryPoint = "glVertex2i")]
		public static extern void Vertex(int x, int y);

		// Token: 0x06000145 RID: 325
		[DllImport("Opengl32.dll", EntryPoint = "glVertex3f")]
		public static extern void Vertex(float x, float y, float z);

		// Token: 0x06000146 RID: 326
		[DllImport("Opengl32.dll", EntryPoint = "glColor3f")]
		public static extern void Color(float red, float green, float blue);

		// Token: 0x06000147 RID: 327
		[DllImport("Opengl32.dll", EntryPoint = "glColor4f")]
		public static extern void Color(float red, float green, float blue, float alpha);

		// Token: 0x06000148 RID: 328
		[DllImport("Opengl32.dll", EntryPoint = "glClearColor")]
		public static extern void ClearColor(float red, float green, float blue, float alpha);

		// Token: 0x06000149 RID: 329
		[DllImport("Opengl32.dll", EntryPoint = "glTexImage2D")]
		public static extern void TexImage2D(Target target, int level, uint internalformat, int width, int height, int border, PixelFormat format, DataType type, IntPtr pixels);

		// Token: 0x0600014A RID: 330
		[DllImport("Opengl32.dll", EntryPoint = "glTexImage2D")]
		public static extern void TexImage2D(Target target, int level, uint internalformat, int width, int height, int border, PixelFormat format, DataType type, byte[] pixels);

		// Token: 0x0600014B RID: 331
		[DllImport("Opengl32.dll", EntryPoint = "glGenTextures")]
		public static extern void GenTextures(int size, ref int textures);

		// Token: 0x0600014C RID: 332
		[DllImport("Opengl32.dll", EntryPoint = "glBindTexture")]
		public static extern void BindTexture(Target target, int texture);

		// Token: 0x0600014D RID: 333
		[DllImport("Opengl32.dll", EntryPoint = "glEnable")]
		public static extern void Enable(Target cap);

		// Token: 0x0600014E RID: 334
		[DllImport("Opengl32.dll", EntryPoint = "glDisable")]
		public static extern void Disable(Target cap);

		// Token: 0x0600014F RID: 335
		[DllImport("Opengl32.dll", EntryPoint = "glTexCoord2f")]
		public static extern void TexCoord(float s, float t);

		// Token: 0x06000150 RID: 336
		[DllImport("Opengl32.dll", EntryPoint = "glTexParameteri")]
		public static extern void TexParameteri(Target target, TextureParameterName pname, int param);

		// Token: 0x06000151 RID: 337
		[DllImport("Opengl32.dll", EntryPoint = "glGetError")]
		public static extern uint GetError();

		// Token: 0x06000152 RID: 338
		[DllImport("Opengl32.dll", EntryPoint = "glFlush")]
		public static extern void Flush();

		// Token: 0x06000153 RID: 339
		[DllImport("Opengl32.dll", EntryPoint = "glFinish")]
		public static extern void Finish();

		// Token: 0x06000154 RID: 340
		[DllImport("Opengl32.dll")]
		public static extern IntPtr wglCreateContext(IntPtr hdc);

		// Token: 0x06000155 RID: 341
		[DllImport("Opengl32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool wglMakeCurrent(IntPtr hdc, IntPtr hglrc);

		// Token: 0x06000156 RID: 342
		[DllImport("opengl32.dll")]
		public static extern IntPtr wglGetProcAddress(string name);

		// Token: 0x06000157 RID: 343
		[DllImport("opengl32.dll")]
		public static extern bool wglDeleteContext(IntPtr hglrc);
	}
}
