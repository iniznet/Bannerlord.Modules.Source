using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200001E RID: 30
	[ApplicationInterfaceBase]
	internal interface ICompositeComponent
	{
		// Token: 0x06000186 RID: 390
		[EngineMethod("create_composite_component", false)]
		CompositeComponent CreateCompositeComponent();

		// Token: 0x06000187 RID: 391
		[EngineMethod("set_material", false)]
		void SetMaterial(UIntPtr compositeComponentPointer, UIntPtr materialPointer);

		// Token: 0x06000188 RID: 392
		[EngineMethod("create_copy", false)]
		CompositeComponent CreateCopy(UIntPtr pointer);

		// Token: 0x06000189 RID: 393
		[EngineMethod("add_component", false)]
		void AddComponent(UIntPtr pointer, UIntPtr componentPointer);

		// Token: 0x0600018A RID: 394
		[EngineMethod("add_prefab_entity", false)]
		void AddPrefabEntity(UIntPtr pointer, UIntPtr scenePointer, string prefabName);

		// Token: 0x0600018B RID: 395
		[EngineMethod("release", false)]
		void Release(UIntPtr compositeComponentPointer);

		// Token: 0x0600018C RID: 396
		[EngineMethod("get_factor_1", false)]
		uint GetFactor1(UIntPtr compositeComponentPointer);

		// Token: 0x0600018D RID: 397
		[EngineMethod("get_factor_2", false)]
		uint GetFactor2(UIntPtr compositeComponentPointer);

		// Token: 0x0600018E RID: 398
		[EngineMethod("set_factor_1", false)]
		void SetFactor1(UIntPtr compositeComponentPointer, uint factorColor1);

		// Token: 0x0600018F RID: 399
		[EngineMethod("set_factor_2", false)]
		void SetFactor2(UIntPtr compositeComponentPointer, uint factorColor2);

		// Token: 0x06000190 RID: 400
		[EngineMethod("set_vector_argument", false)]
		void SetVectorArgument(UIntPtr compositeComponentPointer, float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3);

		// Token: 0x06000191 RID: 401
		[EngineMethod("get_frame", false)]
		void GetFrame(UIntPtr compositeComponentPointer, ref MatrixFrame outFrame);

		// Token: 0x06000192 RID: 402
		[EngineMethod("set_frame", false)]
		void SetFrame(UIntPtr compositeComponentPointer, ref MatrixFrame meshFrame);

		// Token: 0x06000193 RID: 403
		[EngineMethod("get_vector_user_data", false)]
		Vec3 GetVectorUserData(UIntPtr compositeComponentPointer);

		// Token: 0x06000194 RID: 404
		[EngineMethod("set_vector_user_data", false)]
		void SetVectorUserData(UIntPtr compositeComponentPointer, ref Vec3 vectorArg);

		// Token: 0x06000195 RID: 405
		[EngineMethod("get_bounding_box", false)]
		void GetBoundingBox(UIntPtr compositeComponentPointer, ref BoundingBox outBoundingBox);

		// Token: 0x06000196 RID: 406
		[EngineMethod("set_visibility_mask", false)]
		void SetVisibilityMask(UIntPtr compositeComponentPointer, VisibilityMaskFlags visibilityMask);

		// Token: 0x06000197 RID: 407
		[EngineMethod("get_first_meta_mesh", false)]
		MetaMesh GetFirstMetaMesh(UIntPtr compositeComponentPointer);

		// Token: 0x06000198 RID: 408
		[EngineMethod("add_multi_mesh", false)]
		void AddMultiMesh(UIntPtr compositeComponentPointer, string multiMeshName);

		// Token: 0x06000199 RID: 409
		[EngineMethod("is_visible", false)]
		bool IsVisible(UIntPtr compositeComponentPointer);

		// Token: 0x0600019A RID: 410
		[EngineMethod("set_visible", false)]
		void SetVisible(UIntPtr compositeComponentPointer, bool visible);
	}
}
