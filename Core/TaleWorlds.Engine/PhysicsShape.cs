using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000006 RID: 6
	[EngineClass("rglPhysics_shape")]
	public sealed class PhysicsShape : Resource
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002301 File Offset: 0x00000501
		public static PhysicsShape GetFromResource(string bodyName, bool mayReturnNull = false)
		{
			return EngineApplicationInterface.IPhysicsShape.GetFromResource(bodyName, mayReturnNull);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000230F File Offset: 0x0000050F
		public static void AddPreloadQueueWithName(string bodyName, Vec3 scale)
		{
			EngineApplicationInterface.IPhysicsShape.AddPreloadQueueWithName(bodyName, ref scale);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000231E File Offset: 0x0000051E
		public static void ProcessPreloadQueue()
		{
			EngineApplicationInterface.IPhysicsShape.ProcessPreloadQueue();
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000232A File Offset: 0x0000052A
		public static void UnloadDynamicBodies()
		{
			EngineApplicationInterface.IPhysicsShape.UnloadDynamicBodies();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002336 File Offset: 0x00000536
		internal PhysicsShape(UIntPtr bodyPointer)
			: base(bodyPointer)
		{
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000233F File Offset: 0x0000053F
		public PhysicsShape CreateCopy()
		{
			return EngineApplicationInterface.IPhysicsShape.CreateBodyCopy(base.Pointer);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002351 File Offset: 0x00000551
		public int SphereCount()
		{
			return EngineApplicationInterface.IPhysicsShape.SphereCount(base.Pointer);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002363 File Offset: 0x00000563
		public void GetSphere(ref SphereData data, int index)
		{
			EngineApplicationInterface.IPhysicsShape.GetSphere(base.Pointer, ref data, index);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002378 File Offset: 0x00000578
		public void GetSphere(ref SphereData data, out PhysicsMaterial material, int index)
		{
			int num = -1;
			EngineApplicationInterface.IPhysicsShape.GetSphereWithMaterial(base.Pointer, ref data, ref num, index);
			material = new PhysicsMaterial(num);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000023A8 File Offset: 0x000005A8
		public PhysicsMaterial GetDominantMaterialForTriangleMesh(int meshIndex)
		{
			int dominantMaterialForTriangleMesh = EngineApplicationInterface.IPhysicsShape.GetDominantMaterialForTriangleMesh(this, meshIndex);
			return new PhysicsMaterial(dominantMaterialForTriangleMesh);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000023C8 File Offset: 0x000005C8
		public string GetName()
		{
			return EngineApplicationInterface.IPhysicsShape.GetName(this);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000023D5 File Offset: 0x000005D5
		public int TriangleMeshCount()
		{
			return EngineApplicationInterface.IPhysicsShape.TriangleMeshCount(base.Pointer);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000023E7 File Offset: 0x000005E7
		public int TriangleCountInTriangleMesh(int meshIndex)
		{
			return EngineApplicationInterface.IPhysicsShape.TriangleCountInTriangleMesh(base.Pointer, meshIndex);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000023FA File Offset: 0x000005FA
		public void GetTriangle(Vec3[] triangle, int meshIndex, int triangleIndex)
		{
			EngineApplicationInterface.IPhysicsShape.GetTriangle(base.Pointer, triangle, meshIndex, triangleIndex);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000240F File Offset: 0x0000060F
		public void Prepare()
		{
			EngineApplicationInterface.IPhysicsShape.Prepare(base.Pointer);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002421 File Offset: 0x00000621
		public int CapsuleCount()
		{
			return EngineApplicationInterface.IPhysicsShape.CapsuleCount(base.Pointer);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002433 File Offset: 0x00000633
		public void AddCapsule(CapsuleData data)
		{
			EngineApplicationInterface.IPhysicsShape.AddCapsule(base.Pointer, ref data);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002447 File Offset: 0x00000647
		public void InitDescription()
		{
			EngineApplicationInterface.IPhysicsShape.InitDescription(base.Pointer);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002459 File Offset: 0x00000659
		public void AddSphere(SphereData data)
		{
			EngineApplicationInterface.IPhysicsShape.AddSphere(base.Pointer, ref data.Origin, data.Radius);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002478 File Offset: 0x00000678
		public void SetCapsule(CapsuleData data, int index)
		{
			EngineApplicationInterface.IPhysicsShape.SetCapsule(base.Pointer, ref data, index);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000248D File Offset: 0x0000068D
		public void GetCapsule(ref CapsuleData data, int index)
		{
			EngineApplicationInterface.IPhysicsShape.GetCapsule(base.Pointer, ref data, index);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000024A4 File Offset: 0x000006A4
		public void GetCapsule(ref CapsuleData data, out PhysicsMaterial material, int index)
		{
			int num = -1;
			EngineApplicationInterface.IPhysicsShape.GetCapsuleWithMaterial(base.Pointer, ref data, ref num, index);
			material = new PhysicsMaterial(num);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000024D3 File Offset: 0x000006D3
		public Vec3 GetBoundingBoxCenter()
		{
			return EngineApplicationInterface.IPhysicsShape.GetBoundingBoxCenter(base.Pointer);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000024E5 File Offset: 0x000006E5
		public void Transform(ref MatrixFrame frame)
		{
			EngineApplicationInterface.IPhysicsShape.Transform(base.Pointer, ref frame);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000024F8 File Offset: 0x000006F8
		public void Clear()
		{
			EngineApplicationInterface.IPhysicsShape.clear(base.Pointer);
		}
	}
}
