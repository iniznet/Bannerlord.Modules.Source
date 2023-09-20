using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000022 RID: 34
	[ApplicationInterfaceBase]
	internal interface IMesh
	{
		// Token: 0x060001B7 RID: 439
		[EngineMethod("create_mesh", false)]
		Mesh CreateMesh(bool editable);

		// Token: 0x060001B8 RID: 440
		[EngineMethod("get_base_mesh", false)]
		Mesh GetBaseMesh(UIntPtr ptr);

		// Token: 0x060001B9 RID: 441
		[EngineMethod("create_mesh_with_material", false)]
		Mesh CreateMeshWithMaterial(UIntPtr ptr);

		// Token: 0x060001BA RID: 442
		[EngineMethod("create_mesh_copy", false)]
		Mesh CreateMeshCopy(UIntPtr meshPointer);

		// Token: 0x060001BB RID: 443
		[EngineMethod("set_color_and_stroke", false)]
		void SetColorAndStroke(UIntPtr meshPointer, bool drawStroke);

		// Token: 0x060001BC RID: 444
		[EngineMethod("set_mesh_render_order", false)]
		void SetMeshRenderOrder(UIntPtr meshPointer, int renderorder);

		// Token: 0x060001BD RID: 445
		[EngineMethod("has_tag", false)]
		bool HasTag(UIntPtr meshPointer, string tag);

		// Token: 0x060001BE RID: 446
		[EngineMethod("get_mesh_from_resource", false)]
		Mesh GetMeshFromResource(string materialName);

		// Token: 0x060001BF RID: 447
		[EngineMethod("get_random_mesh_with_vdecl", false)]
		Mesh GetRandomMeshWithVdecl(int vdecl);

		// Token: 0x060001C0 RID: 448
		[EngineMethod("set_material_by_name", false)]
		void SetMaterialByName(UIntPtr meshPointer, string materialName);

		// Token: 0x060001C1 RID: 449
		[EngineMethod("set_material", false)]
		void SetMaterial(UIntPtr meshPointer, UIntPtr materialpointer);

		// Token: 0x060001C2 RID: 450
		[EngineMethod("set_vector_argument", false)]
		void SetVectorArgument(UIntPtr meshPointer, float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3);

		// Token: 0x060001C3 RID: 451
		[EngineMethod("set_vector_argument_2", false)]
		void SetVectorArgument2(UIntPtr meshPointer, float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3);

		// Token: 0x060001C4 RID: 452
		[EngineMethod("get_material", false)]
		Material GetMaterial(UIntPtr meshPointer);

		// Token: 0x060001C5 RID: 453
		[EngineMethod("get_second_material", false)]
		Material GetSecondMaterial(UIntPtr meshPointer);

		// Token: 0x060001C6 RID: 454
		[EngineMethod("release_resources", false)]
		void ReleaseResources(UIntPtr meshPointer);

		// Token: 0x060001C7 RID: 455
		[EngineMethod("add_face_corner", false)]
		int AddFaceCorner(UIntPtr meshPointer, Vec3 vertexPosition, Vec3 vertexNormal, Vec2 vertexUVCoordinates, uint vertexColor, UIntPtr lockHandle);

		// Token: 0x060001C8 RID: 456
		[EngineMethod("add_face", false)]
		int AddFace(UIntPtr meshPointer, int faceCorner0, int faceCorner1, int faceCorner2, UIntPtr lockHandle);

		// Token: 0x060001C9 RID: 457
		[EngineMethod("clear_mesh", false)]
		void ClearMesh(UIntPtr meshPointer);

		// Token: 0x060001CA RID: 458
		[EngineMethod("set_name", false)]
		void SetName(UIntPtr meshPointer, string name);

		// Token: 0x060001CB RID: 459
		[EngineMethod("get_name", false)]
		string GetName(UIntPtr meshPointer);

		// Token: 0x060001CC RID: 460
		[EngineMethod("set_morph_time", false)]
		void SetMorphTime(UIntPtr meshPointer, float newTime);

		// Token: 0x060001CD RID: 461
		[EngineMethod("set_culling_mode", false)]
		void SetCullingMode(UIntPtr meshPointer, uint newCullingMode);

		// Token: 0x060001CE RID: 462
		[EngineMethod("set_color", false)]
		void SetColor(UIntPtr meshPointer, uint newColor);

		// Token: 0x060001CF RID: 463
		[EngineMethod("get_color", false)]
		uint GetColor(UIntPtr meshPointer);

		// Token: 0x060001D0 RID: 464
		[EngineMethod("set_color_2", false)]
		void SetColor2(UIntPtr meshPointer, uint newColor2);

		// Token: 0x060001D1 RID: 465
		[EngineMethod("get_color_2", false)]
		uint GetColor2(UIntPtr meshPointer);

		// Token: 0x060001D2 RID: 466
		[EngineMethod("set_color_alpha", false)]
		void SetColorAlpha(UIntPtr meshPointer, uint newColorAlpha);

		// Token: 0x060001D3 RID: 467
		[EngineMethod("get_face_count", false)]
		uint GetFaceCount(UIntPtr meshPointer);

		// Token: 0x060001D4 RID: 468
		[EngineMethod("get_face_corner_count", false)]
		uint GetFaceCornerCount(UIntPtr meshPointer);

		// Token: 0x060001D5 RID: 469
		[EngineMethod("compute_normals", false)]
		void ComputeNormals(UIntPtr meshPointer);

		// Token: 0x060001D6 RID: 470
		[EngineMethod("compute_tangents", false)]
		void ComputeTangents(UIntPtr meshPointer);

		// Token: 0x060001D7 RID: 471
		[EngineMethod("add_mesh_to_mesh", false)]
		void AddMeshToMesh(UIntPtr meshPointer, UIntPtr newMeshPointer, ref MatrixFrame meshFrame);

		// Token: 0x060001D8 RID: 472
		[EngineMethod("set_local_frame", false)]
		void SetLocalFrame(UIntPtr meshPointer, ref MatrixFrame meshFrame);

		// Token: 0x060001D9 RID: 473
		[EngineMethod("get_local_frame", false)]
		void GetLocalFrame(UIntPtr meshPointer, ref MatrixFrame outFrame);

		// Token: 0x060001DA RID: 474
		[EngineMethod("update_bounding_box", false)]
		void UpdateBoundingBox(UIntPtr meshPointer);

		// Token: 0x060001DB RID: 475
		[EngineMethod("set_as_not_effected_by_season", false)]
		void SetAsNotEffectedBySeason(UIntPtr meshPointer);

		// Token: 0x060001DC RID: 476
		[EngineMethod("get_bounding_box_width", false)]
		float GetBoundingBoxWidth(UIntPtr meshPointer);

		// Token: 0x060001DD RID: 477
		[EngineMethod("get_bounding_box_height", false)]
		float GetBoundingBoxHeight(UIntPtr meshPointer);

		// Token: 0x060001DE RID: 478
		[EngineMethod("get_bounding_box_min", false)]
		Vec3 GetBoundingBoxMin(UIntPtr meshPointer);

		// Token: 0x060001DF RID: 479
		[EngineMethod("get_bounding_box_max", false)]
		Vec3 GetBoundingBoxMax(UIntPtr meshPointer);

		// Token: 0x060001E0 RID: 480
		[EngineMethod("add_triangle", false)]
		void AddTriangle(UIntPtr meshPointer, Vec3 p1, Vec3 p2, Vec3 p3, Vec2 uv1, Vec2 uv2, Vec2 uv3, uint color, UIntPtr lockHandle);

		// Token: 0x060001E1 RID: 481
		[EngineMethod("add_triangle_with_vertex_colors", false)]
		void AddTriangleWithVertexColors(UIntPtr meshPointer, Vec3 p1, Vec3 p2, Vec3 p3, Vec2 uv1, Vec2 uv2, Vec2 uv3, uint c1, uint c2, uint c3, UIntPtr lockHandle);

		// Token: 0x060001E2 RID: 482
		[EngineMethod("hint_indices_dynamic", false)]
		void HintIndicesDynamic(UIntPtr meshPointer);

		// Token: 0x060001E3 RID: 483
		[EngineMethod("hint_vertices_dynamic", false)]
		void HintVerticesDynamic(UIntPtr meshPointer);

		// Token: 0x060001E4 RID: 484
		[EngineMethod("recompute_bounding_box", false)]
		void RecomputeBoundingBox(UIntPtr meshPointer);

		// Token: 0x060001E5 RID: 485
		[EngineMethod("get_billboard", false)]
		BillboardType GetBillboard(UIntPtr meshPointer);

		// Token: 0x060001E6 RID: 486
		[EngineMethod("set_billboard", false)]
		void SetBillboard(UIntPtr meshPointer, BillboardType value);

		// Token: 0x060001E7 RID: 487
		[EngineMethod("get_visibility_mask", false)]
		VisibilityMaskFlags GetVisibilityMask(UIntPtr meshPointer);

		// Token: 0x060001E8 RID: 488
		[EngineMethod("set_visibility_mask", false)]
		void SetVisibilityMask(UIntPtr meshPointer, VisibilityMaskFlags value);

		// Token: 0x060001E9 RID: 489
		[EngineMethod("get_edit_data_face_corner_count", false)]
		int GetEditDataFaceCornerCount(UIntPtr meshPointer);

		// Token: 0x060001EA RID: 490
		[EngineMethod("set_edit_data_face_corner_vertex_color", false)]
		void SetEditDataFaceCornerVertexColor(UIntPtr meshPointer, int index, uint color);

		// Token: 0x060001EB RID: 491
		[EngineMethod("get_edit_data_face_corner_vertex_color", false)]
		uint GetEditDataFaceCornerVertexColor(UIntPtr meshPointer, int index);

		// Token: 0x060001EC RID: 492
		[EngineMethod("preload_for_rendering", false)]
		void PreloadForRendering(UIntPtr meshPointer);

		// Token: 0x060001ED RID: 493
		[EngineMethod("set_contour_color", false)]
		void SetContourColor(UIntPtr meshPointer, Vec3 color, bool alwaysVisible, bool maskMesh);

		// Token: 0x060001EE RID: 494
		[EngineMethod("disable_contour", false)]
		void DisableContour(UIntPtr meshPointer);

		// Token: 0x060001EF RID: 495
		[EngineMethod("set_external_bounding_box", false)]
		void SetExternalBoundingBox(UIntPtr meshPointer, ref BoundingBox bbox);

		// Token: 0x060001F0 RID: 496
		[EngineMethod("add_edit_data_user", false)]
		void AddEditDataUser(UIntPtr meshPointer);

		// Token: 0x060001F1 RID: 497
		[EngineMethod("release_edit_data_user", false)]
		void ReleaseEditDataUser(UIntPtr meshPointer);

		// Token: 0x060001F2 RID: 498
		[EngineMethod("set_edit_data_policy", false)]
		void SetEditDataPolicy(UIntPtr meshPointer, EditDataPolicy policy);

		// Token: 0x060001F3 RID: 499
		[EngineMethod("lock_edit_data_write", false)]
		UIntPtr LockEditDataWrite(UIntPtr meshPointer);

		// Token: 0x060001F4 RID: 500
		[EngineMethod("unlock_edit_data_write", false)]
		void UnlockEditDataWrite(UIntPtr meshPointer, UIntPtr handle);
	}
}
