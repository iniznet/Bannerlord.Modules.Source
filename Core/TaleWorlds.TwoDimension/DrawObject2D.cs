using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public sealed class DrawObject2D
	{
		public MeshTopology Topology { get; private set; }

		public float[] Vertices { get; private set; }

		public float[] TextureCoordinates { get; private set; }

		public uint[] Indices { get; private set; }

		public int VertexCount { get; set; }

		public ulong HashCode1 { get; private set; }

		public ulong HashCode2 { get; private set; }

		public Rectangle BoundingRectangle { get; private set; }

		public DrawObjectType DrawObjectType { get; set; }

		public float Width { get; set; }

		public float Height { get; set; }

		public float MinU { get; set; }

		public float MinV { get; set; }

		public float MaxU { get; set; }

		public float MaxV { get; set; }

		static DrawObject2D()
		{
			for (int i = 0; i < 16; i++)
			{
				Vector2 vector = default(Vector2);
				float num = (float)i;
				num *= 22.5f;
				vector.X = Mathf.Cos(num * 0.017453292f);
				vector.Y = Mathf.Sin(num * 0.017453292f);
				DrawObject2D._referenceCirclePoints.Add(vector);
			}
		}

		public DrawObject2D(MeshTopology topology, float[] vertices, float[] uvs, uint[] indices, int vertexCount)
		{
			this.Topology = topology;
			this.Vertices = vertices;
			this.TextureCoordinates = uvs;
			this.Indices = indices;
			this.VertexCount = vertexCount;
		}

		public DrawObject2D(MeshTopology topology, int vertexCount)
		{
			this.Topology = topology;
			this.Vertices = new float[vertexCount * 2];
			this.TextureCoordinates = new float[vertexCount * 2];
			this.Indices = new uint[vertexCount];
			this.VertexCount = vertexCount;
		}

		public void SetVertexAt(int index, Vector2 vertex)
		{
			this.Vertices[2 * index] = vertex.X;
			this.Vertices[2 * index + 1] = vertex.Y;
		}

		public static DrawObject2D CreateTriangleTopologyMeshWithPolygonCoordinates(List<Vector2> vertices)
		{
			int num = 3 * (vertices.Count - 2);
			float[] array = new float[num * 2];
			float[] array2 = new float[num * 2];
			uint[] array3 = new uint[num];
			for (int i = 0; i < num / 3; i++)
			{
				array[6 * i] = vertices[0].X;
				array[6 * i + 1] = vertices[0].Y;
				array[6 * i + 2] = vertices[i + 1].X;
				array[6 * i + 3] = vertices[i + 1].Y;
				array[6 * i + 4] = vertices[i + 2].X;
				array[6 * i + 5] = vertices[i + 2].Y;
			}
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				array3[(int)num2] = num2;
				num2 += 1U;
			}
			return new DrawObject2D(MeshTopology.Triangles, array, array2, array3, num);
		}

		public static DrawObject2D CreateLineTopologyMeshWithPolygonCoordinates(List<Vector2> vertices)
		{
			int num = 2 * vertices.Count;
			float[] array = new float[num * 2];
			float[] array2 = new float[num * 2];
			uint[] array3 = new uint[num];
			DrawObject2D.FillLineTopologyMeshWithPolygonCoordinates(array, array3, vertices);
			return new DrawObject2D(MeshTopology.Lines, array, array2, array3, num);
		}

		private static void FillLineTopologyMeshWithPolygonCoordinates(float[] lineTopologyVertices, uint[] indices, List<Vector2> vertices)
		{
			for (int i = 0; i < vertices.Count; i++)
			{
				int num = i;
				int num2 = ((i + 1 == vertices.Count) ? 0 : (i + 1));
				lineTopologyVertices[i * 4] = vertices[num].X;
				lineTopologyVertices[i * 4 + 1] = vertices[num].Y;
				lineTopologyVertices[i * 4 + 2] = vertices[num2].X;
				lineTopologyVertices[i * 4 + 3] = vertices[num2].Y;
				indices[i] = (uint)i;
			}
		}

		public static DrawObject2D CreateLineTopologyMeshWithQuadVertices(float[] quadVertices, uint[] indices, int vertexCount)
		{
			float[] array = new float[vertexCount * 2 * 2];
			float[] array2 = new float[vertexCount * 2 * 2];
			DrawObject2D.QuadVerticesToLineVertices(quadVertices, vertexCount, array);
			return new DrawObject2D(MeshTopology.Lines, array, array2, indices, vertexCount);
		}

		public static void QuadVerticesToLineVertices(float[] quadVertices, int vertexCount, float[] lineVertices)
		{
			for (int i = 0; i < vertexCount; i++)
			{
				int num = 2 * i;
				int num2 = ((i + 1 == vertexCount) ? 0 : (2 * (i + 1)));
				lineVertices[i * 4] = quadVertices[num];
				lineVertices[i * 4 + 1] = quadVertices[num + 1];
				lineVertices[i * 4 + 2] = quadVertices[num2];
				lineVertices[i * 4 + 3] = quadVertices[num2 + 1];
			}
		}

		public static DrawObject2D CreateTriangleTopologyMeshWithCircleRadius(float radius)
		{
			DrawObject2D._circlePolygonPoints.Clear();
			for (int i = 0; i < DrawObject2D._referenceCirclePoints.Count; i++)
			{
				Vector2 vector = DrawObject2D._referenceCirclePoints[i];
				vector.X *= radius;
				vector.Y *= radius;
				DrawObject2D._circlePolygonPoints.Add(vector);
			}
			return DrawObject2D.CreateTriangleTopologyMeshWithPolygonCoordinates(DrawObject2D._circlePolygonPoints);
		}

		public static DrawObject2D CreateLineTopologyMeshWithCircleRadius(float radius)
		{
			DrawObject2D._circlePolygonPoints.Clear();
			for (int i = 0; i < DrawObject2D._referenceCirclePoints.Count; i++)
			{
				Vector2 vector = DrawObject2D._referenceCirclePoints[i];
				vector.X *= radius;
				vector.Y *= radius;
				DrawObject2D._circlePolygonPoints.Add(vector);
			}
			int num = 2 * DrawObject2D._circlePolygonPoints.Count + 2;
			float[] array = new float[num * 2];
			float[] array2 = new float[num * 2];
			uint[] array3 = new uint[num];
			DrawObject2D.FillLineTopologyMeshWithPolygonCoordinates(array, array3, DrawObject2D._circlePolygonPoints);
			Vector2 vector2 = new Vector2(1f, 0f);
			vector2.X *= radius;
			vector2.Y *= radius;
			array[array.Length - 4] = 0f;
			array[array.Length - 3] = 0f;
			array[array.Length - 2] = vector2.X;
			array[array.Length - 1] = vector2.Y;
			return new DrawObject2D(MeshTopology.Lines, array, array2, array3, num);
		}

		public void RecalculateProperties()
		{
			ulong num;
			ulong num2;
			this.ConvertToHashInPlace(out num, out num2);
			this.HashCode1 = num;
			this.HashCode2 = num2;
			float num3 = float.MaxValue;
			float num4 = float.MaxValue;
			float num5 = float.MinValue;
			float num6 = float.MinValue;
			for (int i = 0; i < this.VertexCount; i++)
			{
				float num7 = this.Vertices[2 * i];
				float num8 = this.Vertices[2 * i + 1];
				if (num7 < num3)
				{
					num3 = num7;
				}
				if (num8 < num4)
				{
					num4 = num8;
				}
				if (num7 > num5)
				{
					num5 = num7;
				}
				if (num8 > num6)
				{
					num6 = num8;
				}
			}
			this.BoundingRectangle = new Rectangle(num3, num4, num5 - num3, num6 - num4);
		}

		public byte[] AsByteArray()
		{
			return Common.SerializeObject(new MeshData
			{
				Topology = this.Topology,
				Vertices = this.Vertices,
				TextureCoordinates = this.TextureCoordinates,
				Indices = this.Indices,
				VertexCount = this.VertexCount
			});
		}

		public void ConvertToHashInPlace(out ulong hash1, out ulong hash2)
		{
			ulong num = 5381UL;
			ulong num2 = 5381UL;
			int num3 = this.Vertices.Length / 2;
			int num4 = this.TextureCoordinates.Length / 2;
			int num5 = this.Indices.Length / 2;
			for (int i = 0; i < num3; i++)
			{
				Buffer.BlockCopy(this.Vertices, i * 4, DrawObject2D.floatTemporaryHolder, 0, 4);
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[0];
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[1];
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[2];
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[3];
			}
			for (int j = num3; j < this.Vertices.Length; j++)
			{
				Buffer.BlockCopy(this.Vertices, j * 4, DrawObject2D.floatTemporaryHolder, 0, 4);
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[0];
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[1];
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[2];
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[3];
			}
			for (int k = 0; k < num4; k++)
			{
				Buffer.BlockCopy(this.TextureCoordinates, k * 4, DrawObject2D.floatTemporaryHolder, 0, 4);
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[0];
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[1];
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[2];
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[3];
			}
			for (int l = num4; l < this.TextureCoordinates.Length; l++)
			{
				Buffer.BlockCopy(this.TextureCoordinates, l * 4, DrawObject2D.floatTemporaryHolder, 0, 4);
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[0];
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[1];
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[2];
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[3];
			}
			for (int m = 0; m < num5; m++)
			{
				Buffer.BlockCopy(this.Indices, m * 4, DrawObject2D.floatTemporaryHolder, 0, 4);
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[0];
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[1];
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[2];
				num = (num << 5) + num + (ulong)DrawObject2D.floatTemporaryHolder[3];
			}
			for (int n = num5; n < this.Indices.Length; n++)
			{
				Buffer.BlockCopy(this.Indices, n * 4, DrawObject2D.floatTemporaryHolder, 0, 4);
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[0];
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[1];
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[2];
				num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[3];
			}
			num = (num << 5) + num + (ulong)((byte)this.Topology);
			DrawObject2D.uintTemporaryHolder[0] = (uint)this.VertexCount;
			Buffer.BlockCopy(DrawObject2D.uintTemporaryHolder, 0, DrawObject2D.floatTemporaryHolder, 0, 4);
			num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[0];
			num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[1];
			num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[2];
			num2 = (num2 << 5) + num2 + (ulong)DrawObject2D.floatTemporaryHolder[3];
			hash1 = num;
			hash2 = num2;
		}

		public static DrawObject2D CreateQuad(Vector2 size)
		{
			DrawObject2D drawObject2D = DrawObject2D.CreateTriangleTopologyMeshWithPolygonCoordinates(new List<Vector2>
			{
				new Vector2(0f, 0f),
				new Vector2(0f, size.Y),
				new Vector2(size.X, size.Y),
				new Vector2(size.X, 0f)
			});
			drawObject2D.DrawObjectType = DrawObjectType.Quad;
			drawObject2D.TextureCoordinates[0] = 0f;
			drawObject2D.TextureCoordinates[1] = 0f;
			drawObject2D.TextureCoordinates[2] = 0f;
			drawObject2D.TextureCoordinates[3] = 1f;
			drawObject2D.TextureCoordinates[4] = 1f;
			drawObject2D.TextureCoordinates[5] = 1f;
			drawObject2D.TextureCoordinates[6] = 0f;
			drawObject2D.TextureCoordinates[7] = 0f;
			drawObject2D.TextureCoordinates[8] = 1f;
			drawObject2D.TextureCoordinates[9] = 1f;
			drawObject2D.TextureCoordinates[10] = 1f;
			drawObject2D.TextureCoordinates[11] = 0f;
			drawObject2D.Width = size.X;
			drawObject2D.Height = size.Y;
			drawObject2D.MinU = 0f;
			drawObject2D.MaxU = 1f;
			drawObject2D.MinV = 0f;
			drawObject2D.MaxV = 1f;
			return drawObject2D;
		}

		private static byte[] floatTemporaryHolder = new byte[4];

		private static uint[] uintTemporaryHolder = new uint[1];

		private static List<Vector2> _referenceCirclePoints = new List<Vector2>(64);

		private static List<Vector2> _circlePolygonPoints = new List<Vector2>(64);
	}
}
