using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200001F RID: 31
	[ApplicationInterfaceBase]
	internal interface IPhysicsShape
	{
		// Token: 0x0600019B RID: 411
		[EngineMethod("get_from_resource", false)]
		PhysicsShape GetFromResource(string bodyName, bool mayReturnNull);

		// Token: 0x0600019C RID: 412
		[EngineMethod("add_preload_queue_with_name", false)]
		void AddPreloadQueueWithName(string bodyName, ref Vec3 scale);

		// Token: 0x0600019D RID: 413
		[EngineMethod("process_preload_queue", false)]
		void ProcessPreloadQueue();

		// Token: 0x0600019E RID: 414
		[EngineMethod("unload_dynamic_bodies", false)]
		void UnloadDynamicBodies();

		// Token: 0x0600019F RID: 415
		[EngineMethod("create_body_copy", false)]
		PhysicsShape CreateBodyCopy(UIntPtr bodyPointer);

		// Token: 0x060001A0 RID: 416
		[EngineMethod("get_name", false)]
		string GetName(PhysicsShape shape);

		// Token: 0x060001A1 RID: 417
		[EngineMethod("triangle_mesh_count", false)]
		int TriangleMeshCount(UIntPtr pointer);

		// Token: 0x060001A2 RID: 418
		[EngineMethod("triangle_count_in_triangle_mesh", false)]
		int TriangleCountInTriangleMesh(UIntPtr pointer, int meshIndex);

		// Token: 0x060001A3 RID: 419
		[EngineMethod("get_dominant_material_index_for_mesh_at_index", false)]
		int GetDominantMaterialForTriangleMesh(PhysicsShape shape, int meshIndex);

		// Token: 0x060001A4 RID: 420
		[EngineMethod("get_triangle", false)]
		void GetTriangle(UIntPtr pointer, Vec3[] data, int meshIndex, int triangleIndex);

		// Token: 0x060001A5 RID: 421
		[EngineMethod("sphere_count", false)]
		int SphereCount(UIntPtr pointer);

		// Token: 0x060001A6 RID: 422
		[EngineMethod("get_sphere", false)]
		void GetSphere(UIntPtr shapePointer, ref SphereData data, int sphereIndex);

		// Token: 0x060001A7 RID: 423
		[EngineMethod("get_sphere_with_material", false)]
		void GetSphereWithMaterial(UIntPtr shapePointer, ref SphereData data, ref int materialIndex, int sphereIndex);

		// Token: 0x060001A8 RID: 424
		[EngineMethod("prepare", false)]
		void Prepare(UIntPtr shapePointer);

		// Token: 0x060001A9 RID: 425
		[EngineMethod("capsule_count", false)]
		int CapsuleCount(UIntPtr shapePointer);

		// Token: 0x060001AA RID: 426
		[EngineMethod("add_capsule", false)]
		void AddCapsule(UIntPtr shapePointer, ref CapsuleData data);

		// Token: 0x060001AB RID: 427
		[EngineMethod("init_description", false)]
		void InitDescription(UIntPtr shapePointer);

		// Token: 0x060001AC RID: 428
		[EngineMethod("add_sphere", false)]
		void AddSphere(UIntPtr shapePointer, ref Vec3 origin, float radius);

		// Token: 0x060001AD RID: 429
		[EngineMethod("set_capsule", false)]
		void SetCapsule(UIntPtr shapePointer, ref CapsuleData data, int index);

		// Token: 0x060001AE RID: 430
		[EngineMethod("get_capsule", false)]
		void GetCapsule(UIntPtr shapePointer, ref CapsuleData data, int index);

		// Token: 0x060001AF RID: 431
		[EngineMethod("get_capsule_with_material", false)]
		void GetCapsuleWithMaterial(UIntPtr shapePointer, ref CapsuleData data, ref int materialIndex, int index);

		// Token: 0x060001B0 RID: 432
		[EngineMethod("clear", false)]
		void clear(UIntPtr shapePointer);

		// Token: 0x060001B1 RID: 433
		[EngineMethod("transform", false)]
		void Transform(UIntPtr shapePointer, ref MatrixFrame frame);

		// Token: 0x060001B2 RID: 434
		[EngineMethod("get_bounding_box_center", false)]
		Vec3 GetBoundingBoxCenter(UIntPtr shapePointer);
	}
}
