using System;
using TaleWorlds.TwoDimension.Standalone.Native;
using TaleWorlds.TwoDimension.Standalone.Native.OpenGL;

namespace TaleWorlds.TwoDimension.Standalone
{
	public class VertexArrayObject
	{
		private VertexArrayObject(uint vertexArrayObject)
		{
			this._vertexArrayObject = vertexArrayObject;
		}

		public void LoadVertexData(float[] vertices)
		{
			this.LoadDataToBuffer(this._vertexBuffer, vertices);
		}

		public void LoadUVData(float[] uvs)
		{
			this.LoadDataToBuffer(this._uvBuffer, uvs);
		}

		public void LoadIndexData(uint[] indices)
		{
			this.LoadDataToIndexBuffer(this._indexBuffer, indices);
		}

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

		private void LoadDataToIndexBuffer(uint buffer, uint[] data)
		{
			using (AutoPinner autoPinner = new AutoPinner(data))
			{
				IntPtr intPtr = autoPinner;
				Opengl32ARB.BindBuffer(BufferBindingTarget.ElementArrayBuffer, buffer);
				Opengl32ARB.BufferSubData(BufferBindingTarget.ElementArrayBuffer, 0, data.Length * 4, intPtr);
			}
		}

		public void Bind()
		{
			Opengl32ARB.BindVertexArray(this._vertexArrayObject);
			Opengl32ARB.BindBuffer(BufferBindingTarget.ElementArrayBuffer, this._indexBuffer);
		}

		public static void UnBind()
		{
			Opengl32ARB.BindVertexArray(0U);
		}

		private static uint CreateArrayBuffer()
		{
			uint[] array = new uint[1];
			Opengl32ARB.GenBuffers(1, array);
			uint num = array[0];
			Opengl32ARB.BindBuffer(BufferBindingTarget.ArrayBuffer, num);
			Opengl32ARB.BufferData(BufferBindingTarget.ArrayBuffer, 524288, IntPtr.Zero, 35048);
			return num;
		}

		private static uint CreateElementArrayBuffer()
		{
			uint[] array = new uint[1];
			Opengl32ARB.GenBuffers(1, array);
			uint num = array[0];
			Opengl32ARB.BindBuffer(BufferBindingTarget.ElementArrayBuffer, num);
			Opengl32ARB.BufferData(BufferBindingTarget.ElementArrayBuffer, 524288, IntPtr.Zero, 35048);
			return num;
		}

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

		private static void BindBuffer(uint index, uint buffer)
		{
			Opengl32ARB.EnableVertexAttribArray(index);
			Opengl32ARB.BindBuffer(BufferBindingTarget.ArrayBuffer, buffer);
			Opengl32ARB.VertexAttribPointer(index, 2, DataType.Float, 0, 0, IntPtr.Zero);
		}

		private static void BindIndexBuffer(uint buffer)
		{
			Opengl32ARB.BindBuffer(BufferBindingTarget.ElementArrayBuffer, buffer);
		}

		private static uint CreateVertexArray()
		{
			uint[] array = new uint[1];
			Opengl32ARB.GenVertexArrays(1, array);
			uint num = array[0];
			Opengl32ARB.BindVertexArray(num);
			return num;
		}

		private uint _vertexArrayObject;

		private uint _vertexBuffer;

		private uint _uvBuffer;

		private uint _indexBuffer;
	}
}
