using System;
using TaleWorlds.TwoDimension.Standalone.Native;
using TaleWorlds.TwoDimension.Standalone.Native.OpenGL;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x0200000C RID: 12
	public class VertexArrayObject
	{
		// Token: 0x060000AF RID: 175 RVA: 0x00004366 File Offset: 0x00002566
		private VertexArrayObject(uint vertexArrayObject)
		{
			this._vertexArrayObject = vertexArrayObject;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00004375 File Offset: 0x00002575
		public void LoadVertexData(float[] vertices)
		{
			this.LoadDataToBuffer(this._vertexBuffer, vertices);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00004384 File Offset: 0x00002584
		public void LoadUVData(float[] uvs)
		{
			this.LoadDataToBuffer(this._uvBuffer, uvs);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00004393 File Offset: 0x00002593
		public void LoadIndexData(uint[] indices)
		{
			this.LoadDataToIndexBuffer(this._indexBuffer, indices);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000043A4 File Offset: 0x000025A4
		private void LoadDataToBuffer(uint buffer, float[] data)
		{
			this.Bind();
			using (AutoPinner autoPinner = new AutoPinner(data))
			{
				IntPtr intPtr = autoPinner;
				Opengl32ARB.BindBuffer(BufferBindingTarget.ArrayBuffer, buffer);
				Opengl32ARB.BufferSubData(BufferBindingTarget.ArrayBuffer, 0, data.Length * 4, intPtr);
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00004408 File Offset: 0x00002608
		private void LoadDataToIndexBuffer(uint buffer, uint[] data)
		{
			using (AutoPinner autoPinner = new AutoPinner(data))
			{
				IntPtr intPtr = autoPinner;
				Opengl32ARB.BindBuffer(BufferBindingTarget.ElementArrayBuffer, buffer);
				Opengl32ARB.BufferSubData(BufferBindingTarget.ElementArrayBuffer, 0, data.Length * 4, intPtr);
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00004468 File Offset: 0x00002668
		public void Bind()
		{
			Opengl32ARB.BindVertexArray(this._vertexArrayObject);
			Opengl32ARB.BindBuffer(BufferBindingTarget.ElementArrayBuffer, this._indexBuffer);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000448F File Offset: 0x0000268F
		public static void UnBind()
		{
			Opengl32ARB.BindVertexArray(0U);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000449C File Offset: 0x0000269C
		private static uint CreateArrayBuffer()
		{
			uint[] array = new uint[1];
			Opengl32ARB.GenBuffers(1, array);
			uint num = array[0];
			Opengl32ARB.BindBuffer(BufferBindingTarget.ArrayBuffer, num);
			Opengl32ARB.BufferData(BufferBindingTarget.ArrayBuffer, 524288, IntPtr.Zero, 35048);
			return num;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000044F0 File Offset: 0x000026F0
		private static uint CreateElementArrayBuffer()
		{
			uint[] array = new uint[1];
			Opengl32ARB.GenBuffers(1, array);
			uint num = array[0];
			Opengl32ARB.BindBuffer(BufferBindingTarget.ElementArrayBuffer, num);
			Opengl32ARB.BufferData(BufferBindingTarget.ElementArrayBuffer, 524288, IntPtr.Zero, 35048);
			return num;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00004544 File Offset: 0x00002744
		public static VertexArrayObject Create()
		{
			VertexArrayObject vertexArrayObject = new VertexArrayObject(VertexArrayObject.CreateVertexArray());
			uint num = VertexArrayObject.CreateArrayBuffer();
			VertexArrayObject.BindBuffer(0U, num);
			uint num2 = VertexArrayObject.CreateElementArrayBuffer();
			VertexArrayObject.BindIndexBuffer(num2);
			vertexArrayObject._vertexBuffer = num;
			vertexArrayObject._indexBuffer = num2;
			return vertexArrayObject;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00004584 File Offset: 0x00002784
		public static VertexArrayObject CreateWithUVBuffer()
		{
			VertexArrayObject vertexArrayObject = new VertexArrayObject(VertexArrayObject.CreateVertexArray());
			uint num = VertexArrayObject.CreateArrayBuffer();
			uint num2 = VertexArrayObject.CreateArrayBuffer();
			VertexArrayObject.BindBuffer(0U, num);
			VertexArrayObject.BindBuffer(1U, num2);
			uint num3 = VertexArrayObject.CreateElementArrayBuffer();
			VertexArrayObject.BindIndexBuffer(num3);
			vertexArrayObject._vertexBuffer = num;
			vertexArrayObject._uvBuffer = num2;
			vertexArrayObject._indexBuffer = num3;
			return vertexArrayObject;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000045D6 File Offset: 0x000027D6
		private static void BindBuffer(uint index, uint buffer)
		{
			Opengl32ARB.EnableVertexAttribArray(index);
			Opengl32ARB.BindBuffer(BufferBindingTarget.ArrayBuffer, buffer);
			Opengl32ARB.VertexAttribPointer(index, 2, DataType.Float, 0, 0, IntPtr.Zero);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000460B File Offset: 0x0000280B
		private static void BindIndexBuffer(uint buffer)
		{
			Opengl32ARB.BindBuffer(BufferBindingTarget.ElementArrayBuffer, buffer);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00004620 File Offset: 0x00002820
		private static uint CreateVertexArray()
		{
			uint[] array = new uint[1];
			Opengl32ARB.GenVertexArrays(1, array);
			uint num = array[0];
			Opengl32ARB.BindVertexArray(num);
			return num;
		}

		// Token: 0x04000040 RID: 64
		private uint _vertexArrayObject;

		// Token: 0x04000041 RID: 65
		private uint _vertexBuffer;

		// Token: 0x04000042 RID: 66
		private uint _uvBuffer;

		// Token: 0x04000043 RID: 67
		private uint _indexBuffer;
	}
}
