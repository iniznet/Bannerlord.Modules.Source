using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200001B RID: 27
	[ApplicationInterfaceBase]
	internal interface IMetaMesh
	{
		// Token: 0x06000136 RID: 310
		[EngineMethod("set_material", false)]
		void SetMaterial(UIntPtr multiMeshPointer, UIntPtr materialPointer);

		// Token: 0x06000137 RID: 311
		[EngineMethod("set_lod_bias", false)]
		void SetLodBias(UIntPtr multiMeshPointer, int lod_bias);

		// Token: 0x06000138 RID: 312
		[EngineMethod("create_meta_mesh", false)]
		MetaMesh CreateMetaMesh(string name);

		// Token: 0x06000139 RID: 313
		[EngineMethod("check_meta_mesh_existence", false)]
		void CheckMetaMeshExistence(string multiMeshPrefixName, int lod_count_check);

		// Token: 0x0600013A RID: 314
		[EngineMethod("create_copy_from_name", false)]
		MetaMesh CreateCopyFromName(string multiMeshPrefixName, bool showErrors, bool mayReturnNull);

		// Token: 0x0600013B RID: 315
		[EngineMethod("get_lod_mask_for_mesh_at_index", false)]
		int GetLodMaskForMeshAtIndex(UIntPtr multiMeshPointer, int meshIndex);

		// Token: 0x0600013C RID: 316
		[EngineMethod("get_total_gpu_size", false)]
		int GetTotalGpuSize(UIntPtr multiMeshPointer);

		// Token: 0x0600013D RID: 317
		[EngineMethod("remove_meshes_with_tag", false)]
		int RemoveMeshesWithTag(UIntPtr multiMeshPointer, string tag);

		// Token: 0x0600013E RID: 318
		[EngineMethod("remove_meshes_without_tag", false)]
		int RemoveMeshesWithoutTag(UIntPtr multiMeshPointer, string tag);

		// Token: 0x0600013F RID: 319
		[EngineMethod("get_mesh_count_with_tag", false)]
		int GetMeshCountWithTag(UIntPtr multiMeshPointer, string tag);

		// Token: 0x06000140 RID: 320
		[EngineMethod("has_vertex_buffer_or_edit_data_or_package_item", false)]
		bool HasVertexBufferOrEditDataOrPackageItem(UIntPtr multiMeshPointer);

		// Token: 0x06000141 RID: 321
		[EngineMethod("has_any_generated_lods", false)]
		bool HasAnyGeneratedLods(UIntPtr multiMeshPointer);

		// Token: 0x06000142 RID: 322
		[EngineMethod("has_any_lods", false)]
		bool HasAnyLods(UIntPtr multiMeshPointer);

		// Token: 0x06000143 RID: 323
		[EngineMethod("copy_to", false)]
		void CopyTo(UIntPtr metaMesh, UIntPtr targetMesh, bool copyMeshes);

		// Token: 0x06000144 RID: 324
		[EngineMethod("clear_meshes_for_other_lods", false)]
		void ClearMeshesForOtherLods(UIntPtr multiMeshPointer, int lodToKeep);

		// Token: 0x06000145 RID: 325
		[EngineMethod("clear_meshes_for_lod", false)]
		void ClearMeshesForLod(UIntPtr multiMeshPointer, int lodToClear);

		// Token: 0x06000146 RID: 326
		[EngineMethod("clear_meshes_for_lower_lods", false)]
		void ClearMeshesForLowerLods(UIntPtr multiMeshPointer, int lod);

		// Token: 0x06000147 RID: 327
		[EngineMethod("clear_meshes", false)]
		void ClearMeshes(UIntPtr multiMeshPointer);

		// Token: 0x06000148 RID: 328
		[EngineMethod("set_num_lods", false)]
		void SetNumLods(UIntPtr multiMeshPointer, int num_lod);

		// Token: 0x06000149 RID: 329
		[EngineMethod("add_mesh", false)]
		void AddMesh(UIntPtr multiMeshPointer, UIntPtr meshPointer, uint lodLevel);

		// Token: 0x0600014A RID: 330
		[EngineMethod("add_meta_mesh", false)]
		void AddMetaMesh(UIntPtr metaMeshPtr, UIntPtr otherMetaMeshPointer);

		// Token: 0x0600014B RID: 331
		[EngineMethod("set_cull_mode", false)]
		void SetCullMode(UIntPtr metaMeshPtr, MBMeshCullingMode cullMode);

		// Token: 0x0600014C RID: 332
		[EngineMethod("merge_with_meta_mesh", false)]
		void MergeMultiMeshes(UIntPtr multiMeshPointer, UIntPtr multiMeshToMergePointer);

		// Token: 0x0600014D RID: 333
		[EngineMethod("assign_cloth_body_from", false)]
		void AssignClothBodyFrom(UIntPtr multiMeshPointer, UIntPtr multiMeshToMergePointer);

		// Token: 0x0600014E RID: 334
		[EngineMethod("batch_with_meta_mesh", false)]
		void BatchMultiMeshes(UIntPtr multiMeshPointer, UIntPtr multiMeshToMergePointer);

		// Token: 0x0600014F RID: 335
		[EngineMethod("has_cloth_simulation_data", false)]
		bool HasClothData(UIntPtr multiMeshPointer);

		// Token: 0x06000150 RID: 336
		[EngineMethod("batch_with_meta_mesh_multiple", false)]
		void BatchMultiMeshesMultiple(UIntPtr multiMeshPointer, UIntPtr[] multiMeshToMergePointers, int metaMeshCount);

		// Token: 0x06000151 RID: 337
		[EngineMethod("clear_edit_data", false)]
		void ClearEditData(UIntPtr multiMeshPointer);

		// Token: 0x06000152 RID: 338
		[EngineMethod("get_mesh_count", false)]
		int GetMeshCount(UIntPtr multiMeshPointer);

		// Token: 0x06000153 RID: 339
		[EngineMethod("get_mesh_at_index", false)]
		Mesh GetMeshAtIndex(UIntPtr multiMeshPointer, int meshIndex);

		// Token: 0x06000154 RID: 340
		[EngineMethod("get_morphed_copy", false)]
		MetaMesh GetMorphedCopy(string multiMeshName, float morphTarget, bool showErrors);

		// Token: 0x06000155 RID: 341
		[EngineMethod("create_copy", false)]
		MetaMesh CreateCopy(UIntPtr ptr);

		// Token: 0x06000156 RID: 342
		[EngineMethod("release", false)]
		void Release(UIntPtr multiMeshPointer);

		// Token: 0x06000157 RID: 343
		[EngineMethod("set_gloss_multiplier", false)]
		void SetGlossMultiplier(UIntPtr multiMeshPointer, float value);

		// Token: 0x06000158 RID: 344
		[EngineMethod("get_factor_1", false)]
		uint GetFactor1(UIntPtr multiMeshPointer);

		// Token: 0x06000159 RID: 345
		[EngineMethod("get_factor_2", false)]
		uint GetFactor2(UIntPtr multiMeshPointer);

		// Token: 0x0600015A RID: 346
		[EngineMethod("set_factor_1_linear", false)]
		void SetFactor1Linear(UIntPtr multiMeshPointer, uint linearFactorColor1);

		// Token: 0x0600015B RID: 347
		[EngineMethod("set_factor_2_linear", false)]
		void SetFactor2Linear(UIntPtr multiMeshPointer, uint linearFactorColor2);

		// Token: 0x0600015C RID: 348
		[EngineMethod("set_factor_1", false)]
		void SetFactor1(UIntPtr multiMeshPointer, uint factorColor1);

		// Token: 0x0600015D RID: 349
		[EngineMethod("set_factor_2", false)]
		void SetFactor2(UIntPtr multiMeshPointer, uint factorColor2);

		// Token: 0x0600015E RID: 350
		[EngineMethod("set_vector_argument", false)]
		void SetVectorArgument(UIntPtr multiMeshPointer, float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3);

		// Token: 0x0600015F RID: 351
		[EngineMethod("set_vector_argument_2", false)]
		void SetVectorArgument2(UIntPtr multiMeshPointer, float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3);

		// Token: 0x06000160 RID: 352
		[EngineMethod("get_vector_argument_2", false)]
		Vec3 GetVectorArgument2(UIntPtr multiMeshPointer);

		// Token: 0x06000161 RID: 353
		[EngineMethod("get_frame", false)]
		void GetFrame(UIntPtr multiMeshPointer, ref MatrixFrame outFrame);

		// Token: 0x06000162 RID: 354
		[EngineMethod("set_frame", false)]
		void SetFrame(UIntPtr multiMeshPointer, ref MatrixFrame meshFrame);

		// Token: 0x06000163 RID: 355
		[EngineMethod("get_vector_user_data", false)]
		Vec3 GetVectorUserData(UIntPtr multiMeshPointer);

		// Token: 0x06000164 RID: 356
		[EngineMethod("set_vector_user_data", false)]
		void SetVectorUserData(UIntPtr multiMeshPointer, ref Vec3 vectorArg);

		// Token: 0x06000165 RID: 357
		[EngineMethod("set_billboarding", false)]
		void SetBillboarding(UIntPtr multiMeshPointer, BillboardType billboard);

		// Token: 0x06000166 RID: 358
		[EngineMethod("use_head_bone_facegen_scaling", false)]
		void UseHeadBoneFaceGenScaling(UIntPtr multiMeshPointer, UIntPtr skeleton, sbyte headLookDirectionBoneIndex, ref MatrixFrame frame);

		// Token: 0x06000167 RID: 359
		[EngineMethod("draw_text_with_default_font", false)]
		void DrawTextWithDefaultFont(UIntPtr multiMeshPointer, string text, Vec2 textPositionMin, Vec2 textPositionMax, Vec2 size, uint color, TextFlags flags);

		// Token: 0x06000168 RID: 360
		[EngineMethod("get_bounding_box", false)]
		void GetBoundingBox(UIntPtr multiMeshPointer, ref BoundingBox outBoundingBox);

		// Token: 0x06000169 RID: 361
		[EngineMethod("get_visibility_mask", false)]
		VisibilityMaskFlags GetVisibilityMask(UIntPtr multiMeshPointer);

		// Token: 0x0600016A RID: 362
		[EngineMethod("set_visibility_mask", false)]
		void SetVisibilityMask(UIntPtr multiMeshPointer, VisibilityMaskFlags visibilityMask);

		// Token: 0x0600016B RID: 363
		[EngineMethod("get_name", false)]
		string GetName(UIntPtr multiMeshPointer);

		// Token: 0x0600016C RID: 364
		[EngineMethod("get_multi_mesh_count", false)]
		int GetMultiMeshCount();

		// Token: 0x0600016D RID: 365
		[EngineMethod("get_all_multi_meshes", false)]
		int GetAllMultiMeshes(UIntPtr[] gameEntitiesTemp);

		// Token: 0x0600016E RID: 366
		[EngineMethod("get_multi_mesh", false)]
		MetaMesh GetMultiMesh(string name);

		// Token: 0x0600016F RID: 367
		[EngineMethod("preload_for_rendering", false)]
		void PreloadForRendering(UIntPtr multiMeshPointer);

		// Token: 0x06000170 RID: 368
		[EngineMethod("check_resources", false)]
		int CheckResources(UIntPtr meshPointer);

		// Token: 0x06000171 RID: 369
		[EngineMethod("preload_shaders", false)]
		void PreloadShaders(UIntPtr multiMeshPointer, bool useTableau, bool useTeamColor);

		// Token: 0x06000172 RID: 370
		[EngineMethod("recompute_bounding_box", false)]
		void RecomputeBoundingBox(UIntPtr multiMeshPointer, bool recomputeMeshes);

		// Token: 0x06000173 RID: 371
		[EngineMethod("add_edit_data_user", false)]
		void AddEditDataUser(UIntPtr meshPointer);

		// Token: 0x06000174 RID: 372
		[EngineMethod("release_edit_data_user", false)]
		void ReleaseEditDataUser(UIntPtr meshPointer);

		// Token: 0x06000175 RID: 373
		[EngineMethod("set_edit_data_policy", false)]
		void SetEditDataPolicy(UIntPtr meshPointer, EditDataPolicy policy);

		// Token: 0x06000176 RID: 374
		[EngineMethod("set_contour_state", false)]
		void SetContourState(UIntPtr meshPointer, bool alwaysVisible);

		// Token: 0x06000177 RID: 375
		[EngineMethod("set_contour_color", false)]
		void SetContourColor(UIntPtr meshPointer, uint color);

		// Token: 0x06000178 RID: 376
		[EngineMethod("set_material_to_sub_meshes_with_tag", false)]
		void SetMaterialToSubMeshesWithTag(UIntPtr meshPointer, UIntPtr materialPointer, string tag);

		// Token: 0x06000179 RID: 377
		[EngineMethod("set_factor_color_to_sub_meshes_with_tag", false)]
		void SetFactorColorToSubMeshesWithTag(UIntPtr meshPointer, uint color, string tag);
	}
}
