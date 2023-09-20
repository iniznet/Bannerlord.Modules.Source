using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x0200001B RID: 27
	public sealed class DrawObject2D
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00006462 File Offset: 0x00004662
		// (set) Token: 0x060000FB RID: 251 RVA: 0x0000646A File Offset: 0x0000466A
		public MeshTopology Topology { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00006473 File Offset: 0x00004673
		// (set) Token: 0x060000FD RID: 253 RVA: 0x0000647B File Offset: 0x0000467B
		public float[] Vertices { get; private set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00006484 File Offset: 0x00004684
		// (set) Token: 0x060000FF RID: 255 RVA: 0x0000648C File Offset: 0x0000468C
		public float[] TextureCoordinates { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00006495 File Offset: 0x00004695
		// (set) Token: 0x06000101 RID: 257 RVA: 0x0000649D File Offset: 0x0000469D
		public uint[] Indices { get; private set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000102 RID: 258 RVA: 0x000064A6 File Offset: 0x000046A6
		// (set) Token: 0x06000103 RID: 259 RVA: 0x000064AE File Offset: 0x000046AE
		public int VertexCount { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000104 RID: 260 RVA: 0x000064B7 File Offset: 0x000046B7
		// (set) Token: 0x06000105 RID: 261 RVA: 0x000064BF File Offset: 0x000046BF
		public ulong HashCode1 { get; private set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000106 RID: 262 RVA: 0x000064C8 File Offset: 0x000046C8
		// (set) Token: 0x06000107 RID: 263 RVA: 0x000064D0 File Offset: 0x000046D0
		public ulong HashCode2 { get; private set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000108 RID: 264 RVA: 0x000064D9 File Offset: 0x000046D9
		// (set) Token: 0x06000109 RID: 265 RVA: 0x000064E1 File Offset: 0x000046E1
		public Rectangle BoundingRectangle { get; private set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600010A RID: 266 RVA: 0x000064EA File Offset: 0x000046EA
		// (set) Token: 0x0600010B RID: 267 RVA: 0x000064F2 File Offset: 0x000046F2
		public DrawObjectType DrawObjectType { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600010C RID: 268 RVA: 0x000064FB File Offset: 0x000046FB
		// (set) Token: 0x0600010D RID: 269 RVA: 0x00006503 File Offset: 0x00004703
		public float Width { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600010E RID: 270 RVA: 0x0000650C File Offset: 0x0000470C
		// (set) Token: 0x0600010F RID: 271 RVA: 0x00006514 File Offset: 0x00004714
		public float Height { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000110 RID: 272 RVA: 0x0000651D File Offset: 0x0000471D
		// (set) Token: 0x06000111 RID: 273 RVA: 0x00006525 File Offset: 0x00004725
		public float MinU { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000112 RID: 274 RVA: 0x0000652E File Offset: 0x0000472E
		// (set) Token: 0x06000113 RID: 275 RVA: 0x00006536 File Offset: 0x00004736
		public float MinV { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000114 RID: 276 RVA: 0x0000653F File Offset: 0x0000473F
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00006547 File Offset: 0x00004747
		public float MaxU { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00006550 File Offset: 0x00004750
		// (set) Token: 0x06000117 RID: 279 RVA: 0x00006558 File Offset: 0x00004758
		public float MaxV { get; set; }

		// Token: 0x06000118 RID: 280 RVA: 0x00006564 File Offset: 0x00004764
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

		// Token: 0x06000119 RID: 281 RVA: 0x000065F0 File Offset: 0x000047F0
		public DrawObject2D(MeshTopology topology, float[] vertices, float[] uvs, uint[] indices, int vertexCount)
		{
			this.Topology = topology;
			this.Vertices = vertices;
			this.TextureCoordinates = uvs;
			this.Indices = indices;
			this.VertexCount = vertexCount;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000661D File Offset: 0x0000481D
		public DrawObject2D(MeshTopology topology, int vertexCount)
		{
			this.Topology = topology;
			this.Vertices = new float[vertexCount * 2];
			this.TextureCoordinates = new float[vertexCount * 2];
			this.Indices = new uint[vertexCount];
			this.VertexCount = vertexCount;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000665B File Offset: 0x0000485B
		public void SetVertexAt(int index, Vector2 vertex)
		{
			this.Vertices[2 * index] = vertex.X;
			this.Vertices[2 * index + 1] = vertex.Y;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00006680 File Offset: 0x00004880
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

		// Token: 0x0600011D RID: 285 RVA: 0x00006770 File Offset: 0x00004970
		public static DrawObject2D CreateLineTopologyMeshWithPolygonCoordinates(List<Vector2> vertices)
		{
			int num = 2 * vertices.Count;
			float[] array = new float[num * 2];
			float[] array2 = new float[num * 2];
			uint[] array3 = new uint[num];
			DrawObject2D.FillLineTopologyMeshWithPolygonCoordinates(array, array3, vertices);
			return new DrawObject2D(MeshTopology.Lines, array, array2, array3, num);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000067B4 File Offset: 0x000049B4
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

		// Token: 0x0600011F RID: 287 RVA: 0x00006834 File Offset: 0x00004A34
		public static DrawObject2D CreateLineTopologyMeshWithQuadVertices(float[] quadVertices, uint[] indices, int vertexCount)
		{
			float[] array = new float[vertexCount * 2 * 2];
			float[] array2 = new float[vertexCount * 2 * 2];
			DrawObject2D.QuadVerticesToLineVertices(quadVertices, vertexCount, array);
			return new DrawObject2D(MeshTopology.Lines, array, array2, indices, vertexCount);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000686C File Offset: 0x00004A6C
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

		// Token: 0x06000121 RID: 289 RVA: 0x000068C4 File Offset: 0x00004AC4
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

		// Token: 0x06000122 RID: 290 RVA: 0x0000692C File Offset: 0x00004B2C
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

		// Token: 0x06000123 RID: 291 RVA: 0x00006A2C File Offset: 0x00004C2C
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

		// Token: 0x06000124 RID: 292 RVA: 0x00006AD8 File Offset: 0x00004CD8
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

		// Token: 0x06000125 RID: 293 RVA: 0x00006B3C File Offset: 0x00004D3C
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

		// Token: 0x06000126 RID: 294 RVA: 0x00006E4C File Offset: 0x0000504C
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

		// Token: 0x04000098 RID: 152
		private static byte[] floatTemporaryHolder = new byte[4];

		// Token: 0x04000099 RID: 153
		private static uint[] uintTemporaryHolder = new uint[1];

		// Token: 0x040000A2 RID: 162
		private static List<Vector2> _referenceCirclePoints = new List<Vector2>(64);

		// Token: 0x040000A3 RID: 163
		private static List<Vector2> _circlePolygonPoints = new List<Vector2>(64);
	}
}
