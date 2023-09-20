using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public class MeshBuilder
	{
		public MeshBuilder()
		{
			this.vertices = new List<Vec3>();
			this.faceCorners = new List<MeshBuilder.FaceCorner>();
			this.faces = new List<MeshBuilder.Face>();
		}

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

		public int AddFace(int patchNode0, int patchNode1, int patchNode2)
		{
			MeshBuilder.Face face;
			face.fc0 = patchNode0;
			face.fc1 = patchNode1;
			face.fc2 = patchNode2;
			this.faces.Add(face);
			return this.faces.Count - 1;
		}

		public void Clear()
		{
			this.vertices.Clear();
			this.faceCorners.Clear();
			this.faces.Clear();
		}

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

		public static Mesh CreateTilingWindowMesh(string baseMeshName, Vec2 meshSizeMin, Vec2 meshSizeMax, Vec2 borderThickness, Vec2 bgBorderThickness)
		{
			return EngineApplicationInterface.IMeshBuilder.CreateTilingWindowMesh(baseMeshName, ref meshSizeMin, ref meshSizeMax, ref borderThickness, ref bgBorderThickness);
		}

		public static Mesh CreateTilingButtonMesh(string baseMeshName, Vec2 meshSizeMin, Vec2 meshSizeMax, Vec2 borderThickness)
		{
			return EngineApplicationInterface.IMeshBuilder.CreateTilingButtonMesh(baseMeshName, ref meshSizeMin, ref meshSizeMax, ref borderThickness);
		}

		private List<Vec3> vertices;

		private List<MeshBuilder.FaceCorner> faceCorners;

		private List<MeshBuilder.Face> faces;

		[EngineStruct("rglMeshBuilder_face_corner", false)]
		public struct FaceCorner
		{
			public int vertexIndex;

			public Vec2 uvCoord;

			public Vec3 normal;

			public uint color;
		}

		[EngineStruct("rglMeshBuilder_face", false)]
		public struct Face
		{
			public int fc0;

			public int fc1;

			public int fc2;
		}
	}
}
