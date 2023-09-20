using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000064 RID: 100
	public class MeshBuilder
	{
		// Token: 0x060007F6 RID: 2038 RVA: 0x000079A8 File Offset: 0x00005BA8
		public MeshBuilder()
		{
			this.vertices = new List<Vec3>();
			this.faceCorners = new List<MeshBuilder.FaceCorner>();
			this.faces = new List<MeshBuilder.Face>();
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x000079D4 File Offset: 0x00005BD4
		public int AddFaceCorner(Vec3 position, Vec3 normal, Vec2 uvCoord, uint color)
		{
			this.vertices.Add(new Vec3(position, -1f));
			MeshBuilder.FaceCorner faceCorner;
			faceCorner.vertexIndex = this.vertices.Count - 1;
			faceCorner.color = color;
			faceCorner.uvCoord = uvCoord;
			faceCorner.normal = normal;
			this.faceCorners.Add(faceCorner);
			return this.faceCorners.Count - 1;
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x00007A40 File Offset: 0x00005C40
		public int AddFace(int patchNode0, int patchNode1, int patchNode2)
		{
			MeshBuilder.Face face;
			face.fc0 = patchNode0;
			face.fc1 = patchNode1;
			face.fc2 = patchNode2;
			this.faces.Add(face);
			return this.faces.Count - 1;
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x00007A7E File Offset: 0x00005C7E
		public void Clear()
		{
			this.vertices.Clear();
			this.faceCorners.Clear();
			this.faces.Clear();
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x00007AA4 File Offset: 0x00005CA4
		public new Mesh Finalize()
		{
			Vec3[] array = this.vertices.ToArray();
			MeshBuilder.FaceCorner[] array2 = this.faceCorners.ToArray();
			MeshBuilder.Face[] array3 = this.faces.ToArray();
			Mesh mesh = EngineApplicationInterface.IMeshBuilder.FinalizeMeshBuilder(this.vertices.Count, array, this.faceCorners.Count, array2, this.faces.Count, array3);
			this.vertices.Clear();
			this.faceCorners.Clear();
			this.faces.Clear();
			return mesh;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00007B24 File Offset: 0x00005D24
		public static Mesh CreateUnitMesh()
		{
			Mesh mesh = Mesh.CreateMeshWithMaterial(Material.GetDefaultMaterial());
			Vec3 vec = new Vec3(0f, -1f, 0f, -1f);
			Vec3 vec2 = new Vec3(1f, -1f, 0f, -1f);
			Vec3 vec3 = new Vec3(1f, 0f, 0f, -1f);
			Vec3 vec4 = new Vec3(0f, 0f, 0f, -1f);
			Vec3 vec5 = new Vec3(0f, 0f, 1f, -1f);
			Vec2 vec6 = new Vec2(0f, 0f);
			Vec2 vec7 = new Vec2(1f, 0f);
			Vec2 vec8 = new Vec2(1f, 1f);
			Vec2 vec9 = new Vec2(0f, 1f);
			UIntPtr uintPtr = mesh.LockEditDataWrite();
			int num = mesh.AddFaceCorner(vec, vec5, vec6, uint.MaxValue, uintPtr);
			int num2 = mesh.AddFaceCorner(vec2, vec5, vec7, uint.MaxValue, uintPtr);
			int num3 = mesh.AddFaceCorner(vec3, vec5, vec8, uint.MaxValue, uintPtr);
			int num4 = mesh.AddFaceCorner(vec4, vec5, vec9, uint.MaxValue, uintPtr);
			mesh.AddFace(num, num2, num3, uintPtr);
			mesh.AddFace(num3, num4, num, uintPtr);
			mesh.UpdateBoundingBox();
			mesh.UnlockEditDataWrite(uintPtr);
			return mesh;
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x00007C7A File Offset: 0x00005E7A
		public static Mesh CreateTilingWindowMesh(string baseMeshName, Vec2 meshSizeMin, Vec2 meshSizeMax, Vec2 borderThickness, Vec2 bgBorderThickness)
		{
			return EngineApplicationInterface.IMeshBuilder.CreateTilingWindowMesh(baseMeshName, ref meshSizeMin, ref meshSizeMax, ref borderThickness, ref bgBorderThickness);
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x00007C8F File Offset: 0x00005E8F
		public static Mesh CreateTilingButtonMesh(string baseMeshName, Vec2 meshSizeMin, Vec2 meshSizeMax, Vec2 borderThickness)
		{
			return EngineApplicationInterface.IMeshBuilder.CreateTilingButtonMesh(baseMeshName, ref meshSizeMin, ref meshSizeMax, ref borderThickness);
		}

		// Token: 0x0400012E RID: 302
		private List<Vec3> vertices;

		// Token: 0x0400012F RID: 303
		private List<MeshBuilder.FaceCorner> faceCorners;

		// Token: 0x04000130 RID: 304
		private List<MeshBuilder.Face> faces;

		// Token: 0x020000BC RID: 188
		[EngineStruct("rglMeshBuilder_face_corner")]
		public struct FaceCorner
		{
			// Token: 0x040003EE RID: 1006
			public int vertexIndex;

			// Token: 0x040003EF RID: 1007
			public Vec2 uvCoord;

			// Token: 0x040003F0 RID: 1008
			public Vec3 normal;

			// Token: 0x040003F1 RID: 1009
			public uint color;
		}

		// Token: 0x020000BD RID: 189
		[EngineStruct("rglMeshBuilder_face")]
		public struct Face
		{
			// Token: 0x040003F2 RID: 1010
			public int fc0;

			// Token: 0x040003F3 RID: 1011
			public int fc1;

			// Token: 0x040003F4 RID: 1012
			public int fc2;
		}
	}
}
