using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000014 RID: 20
	[ApplicationInterfaceBase]
	internal interface IManagedMeshEditOperations
	{
		// Token: 0x0600007B RID: 123
		[EngineMethod("create", false)]
		ManagedMeshEditOperations Create(UIntPtr meshPointer);

		// Token: 0x0600007C RID: 124
		[EngineMethod("weld", false)]
		void Weld(UIntPtr Pointer);

		// Token: 0x0600007D RID: 125
		[EngineMethod("add_vertex", false)]
		int AddVertex(UIntPtr Pointer, ref Vec3 vertexPos);

		// Token: 0x0600007E RID: 126
		[EngineMethod("add_face_corner1", false)]
		int AddFaceCorner1(UIntPtr Pointer, int vertexIndex, ref Vec2 uv0, ref Vec3 color, ref Vec3 normal);

		// Token: 0x0600007F RID: 127
		[EngineMethod("add_face_corner2", false)]
		int AddFaceCorner2(UIntPtr Pointer, int vertexIndex, ref Vec2 uv0, ref Vec2 uv1, ref Vec3 color, ref Vec3 normal);

		// Token: 0x06000080 RID: 128
		[EngineMethod("add_face", false)]
		int AddFace(UIntPtr Pointer, int patchNode0, int patchNode1, int patchNode2);

		// Token: 0x06000081 RID: 129
		[EngineMethod("add_triangle1", false)]
		void AddTriangle1(UIntPtr Pointer, ref Vec3 p1, ref Vec3 p2, ref Vec3 p3, ref Vec2 uv1, ref Vec2 uv2, ref Vec2 uv3, ref Vec3 color);

		// Token: 0x06000082 RID: 130
		[EngineMethod("add_triangle2", false)]
		void AddTriangle2(UIntPtr Pointer, ref Vec3 p1, ref Vec3 p2, ref Vec3 p3, ref Vec3 n1, ref Vec3 n2, ref Vec3 n3, ref Vec2 uv1, ref Vec2 uv2, ref Vec2 uv3, ref Vec3 c1, ref Vec3 c2, ref Vec3 c3);

		// Token: 0x06000083 RID: 131
		[EngineMethod("add_rectangle", false)]
		void AddRectangle3(UIntPtr Pointer, ref Vec3 o, ref Vec2 size, ref Vec2 uv_origin, ref Vec2 uvSize, ref Vec3 color);

		// Token: 0x06000084 RID: 132
		[EngineMethod("add_rectangle_with_inverse_uv", false)]
		void AddRectangleWithInverseUV(UIntPtr Pointer, ref Vec3 o, ref Vec2 size, ref Vec2 uv_origin, ref Vec2 uvSize, ref Vec3 color);

		// Token: 0x06000085 RID: 133
		[EngineMethod("add_rect", false)]
		void AddRect(UIntPtr Pointer, ref Vec3 originBegin, ref Vec3 originEnd, ref Vec2 uvBegin, ref Vec2 uvEnd, ref Vec3 color);

		// Token: 0x06000086 RID: 134
		[EngineMethod("add_rect_z_up", false)]
		void AddRectWithZUp(UIntPtr Pointer, ref Vec3 originBegin, ref Vec3 originEnd, ref Vec2 uvBegin, ref Vec2 uvEnd, ref Vec3 color);

		// Token: 0x06000087 RID: 135
		[EngineMethod("invert_faces_winding_order", false)]
		void InvertFacesWindingOrder(UIntPtr Pointer);

		// Token: 0x06000088 RID: 136
		[EngineMethod("scale_vertices1", false)]
		void ScaleVertices1(UIntPtr Pointer, float newScale);

		// Token: 0x06000089 RID: 137
		[EngineMethod("move_vertices_along_normal", false)]
		void MoveVerticesAlongNormal(UIntPtr Pointer, float moveAmount);

		// Token: 0x0600008A RID: 138
		[EngineMethod("scale_vertices2", false)]
		void ScaleVertices2(UIntPtr Pointer, ref Vec3 newScale, bool keepUvX = false, float maxUvSize = 1f);

		// Token: 0x0600008B RID: 139
		[EngineMethod("translate_vertices", false)]
		void TranslateVertices(UIntPtr Pointer, ref Vec3 newOrigin);

		// Token: 0x0600008C RID: 140
		[EngineMethod("add_mesh_aux", false)]
		void AddMeshAux(UIntPtr Pointer, UIntPtr meshPointer, ref MatrixFrame frame, sbyte boneIndex, ref Vec3 color, bool transformNormal, bool heightGradient, bool addSkinData, bool useDoublePrecision = true);

		// Token: 0x0600008D RID: 141
		[EngineMethod("compute_tangents", false)]
		int ComputeTangents(UIntPtr Pointer, bool checkFixedNormals);

		// Token: 0x0600008E RID: 142
		[EngineMethod("generate_grid", false)]
		void GenerateGrid(UIntPtr Pointer, ref Vec2i numEdges, ref Vec2 edgeScale);

		// Token: 0x0600008F RID: 143
		[EngineMethod("rescale_mesh_2d", false)]
		void RescaleMesh2d(UIntPtr Pointer, ref Vec2 scaleSizeMin, ref Vec2 scaleSizeMax);

		// Token: 0x06000090 RID: 144
		[EngineMethod("rescale_mesh_2d_repeat_x", false)]
		void RescaleMesh2dRepeatX(UIntPtr Pointer, ref Vec2 scaleSizeMin, ref Vec2 scaleSizeMax, float frameThickness = 0f, int frameSide = 0);

		// Token: 0x06000091 RID: 145
		[EngineMethod("rescale_mesh_2d_repeat_y", false)]
		void RescaleMesh2dRepeatY(UIntPtr Pointer, ref Vec2 scaleSizeMin, ref Vec2 scaleSizeMax, float frameThickness = 0f, int frameSide = 0);

		// Token: 0x06000092 RID: 146
		[EngineMethod("rescale_mesh_2d_repeat_x_with_tiling", false)]
		void RescaleMesh2dRepeatXWithTiling(UIntPtr Pointer, ref Vec2 scaleSizeMin, ref Vec2 scaleSizeMax, float frameThickness = 0f, int frameSide = 0, float xyRatio = 0f);

		// Token: 0x06000093 RID: 147
		[EngineMethod("rescale_mesh_2d_repeat_y_with_tiling", false)]
		void RescaleMesh2dRepeatYWithTiling(UIntPtr Pointer, ref Vec2 scaleSizeMin, ref Vec2 scaleSizeMax, float frameThickness = 0f, int frameSide = 0, float xyRatio = 0f);

		// Token: 0x06000094 RID: 148
		[EngineMethod("rescale_mesh_2d_without_changing_uv", false)]
		void RescaleMesh2dWithoutChangingUV(UIntPtr Pointer, ref Vec2 scaleSizeMin, ref Vec2 scaleSizeMax, float remaining);

		// Token: 0x06000095 RID: 149
		[EngineMethod("add_line", false)]
		void AddLine(UIntPtr Pointer, ref Vec3 start, ref Vec3 end, ref Vec3 color, float lineWidth = 0.004f);

		// Token: 0x06000096 RID: 150
		[EngineMethod("compute_corner_normals", false)]
		void ComputeCornerNormals(UIntPtr Pointer, bool checkFixedNormals = false, bool smoothCornerNormals = true);

		// Token: 0x06000097 RID: 151
		[EngineMethod("compute_corner_normals_with_smoothing_data", false)]
		void ComputeCornerNormalsWithSmoothingData(UIntPtr Pointer);

		// Token: 0x06000098 RID: 152
		[EngineMethod("add_mesh", false)]
		void AddMesh(UIntPtr Pointer, UIntPtr meshPointer, ref MatrixFrame frame);

		// Token: 0x06000099 RID: 153
		[EngineMethod("add_mesh_with_skin_data", false)]
		void AddMeshWithSkinData(UIntPtr Pointer, UIntPtr meshPointer, ref MatrixFrame frame, sbyte boneIndex);

		// Token: 0x0600009A RID: 154
		[EngineMethod("add_mesh_with_color", false)]
		void AddMeshWithColor(UIntPtr Pointer, UIntPtr meshPointer, ref MatrixFrame frame, ref Vec3 vertexColor, bool useDoublePrecision = true);

		// Token: 0x0600009B RID: 155
		[EngineMethod("add_mesh_to_bone", false)]
		void AddMeshToBone(UIntPtr Pointer, UIntPtr meshPointer, ref MatrixFrame frame, sbyte boneIndex);

		// Token: 0x0600009C RID: 156
		[EngineMethod("add_mesh_with_fixed_normals", false)]
		void AddMeshWithFixedNormals(UIntPtr Pointer, UIntPtr meshPointer, ref MatrixFrame frame);

		// Token: 0x0600009D RID: 157
		[EngineMethod("add_mesh_with_fixed_normals_with_height_gradient_color", false)]
		void AddMeshWithFixedNormalsWithHeightGradientColor(UIntPtr Pointer, UIntPtr meshPointer, ref MatrixFrame frame);

		// Token: 0x0600009E RID: 158
		[EngineMethod("add_skinned_mesh_with_color", false)]
		void AddSkinnedMeshWithColor(UIntPtr Pointer, UIntPtr meshPointer, ref MatrixFrame frame, ref Vec3 vertexColor, bool useDoublePrecision = true);

		// Token: 0x0600009F RID: 159
		[EngineMethod("set_corner_vertex_color", false)]
		void SetCornerVertexColor(UIntPtr Pointer, int cornerNo, ref Vec3 vertexColor);

		// Token: 0x060000A0 RID: 160
		[EngineMethod("set_corner_vertex_uv", false)]
		void SetCornerUV(UIntPtr Pointer, int cornerNo, ref Vec2 newUV, int uvNumber = 0);

		// Token: 0x060000A1 RID: 161
		[EngineMethod("reserve_vertices", false)]
		void ReserveVertices(UIntPtr Pointer, int count);

		// Token: 0x060000A2 RID: 162
		[EngineMethod("reserve_face_corners", false)]
		void ReserveFaceCorners(UIntPtr Pointer, int count);

		// Token: 0x060000A3 RID: 163
		[EngineMethod("reserve_faces", false)]
		void ReserveFaces(UIntPtr Pointer, int count);

		// Token: 0x060000A4 RID: 164
		[EngineMethod("remove_duplicated_corners", false)]
		int RemoveDuplicatedCorners(UIntPtr Pointer);

		// Token: 0x060000A5 RID: 165
		[EngineMethod("transform_vertices_to_parent", false)]
		void TransformVerticesToParent(UIntPtr Pointer, ref MatrixFrame frame);

		// Token: 0x060000A6 RID: 166
		[EngineMethod("transform_vertices_to_local", false)]
		void TransformVerticesToLocal(UIntPtr Pointer, ref MatrixFrame frame);

		// Token: 0x060000A7 RID: 167
		[EngineMethod("set_vertex_color", false)]
		void SetVertexColor(UIntPtr Pointer, ref Vec3 color);

		// Token: 0x060000A8 RID: 168
		[EngineMethod("get_vertex_color", false)]
		Vec3 GetVertexColor(UIntPtr Pointer, int faceCornerIndex);

		// Token: 0x060000A9 RID: 169
		[EngineMethod("set_vertex_color_alpha", false)]
		void SetVertexColorAlpha(UIntPtr Pointer, float newAlpha);

		// Token: 0x060000AA RID: 170
		[EngineMethod("get_vertex_color_alpha", false)]
		float GetVertexColorAlpha(UIntPtr Pointer);

		// Token: 0x060000AB RID: 171
		[EngineMethod("ensure_transformed_vertices", false)]
		void EnsureTransformedVertices(UIntPtr Pointer);

		// Token: 0x060000AC RID: 172
		[EngineMethod("apply_cpu_skinning", false)]
		void ApplyCPUSkinning(UIntPtr Pointer, UIntPtr skeletonPointer);

		// Token: 0x060000AD RID: 173
		[EngineMethod("update_overlapped_vertex_normals", false)]
		void UpdateOverlappedVertexNormals(UIntPtr Pointer, UIntPtr meshPointer, ref MatrixFrame attachFrame, float mergeRadiusSQ = 0.0025f);

		// Token: 0x060000AE RID: 174
		[EngineMethod("clear_all", false)]
		void ClearAll(UIntPtr Pointer);

		// Token: 0x060000AF RID: 175
		[EngineMethod("set_tangents_of_face_corner", false)]
		void SetTangentsOfFaceCorner(UIntPtr Pointer, int faceCornerIndex, ref Vec3 tangent, ref Vec3 binormal);

		// Token: 0x060000B0 RID: 176
		[EngineMethod("set_position_of_vertex", false)]
		void SetPositionOfVertex(UIntPtr Pointer, int vertexIndex, ref Vec3 position);

		// Token: 0x060000B1 RID: 177
		[EngineMethod("get_position_of_vertex", false)]
		Vec3 GetPositionOfVertex(UIntPtr Pointer, int vertexIndex);

		// Token: 0x060000B2 RID: 178
		[EngineMethod("remove_face", false)]
		void RemoveFace(UIntPtr Pointer, int faceIndex);

		// Token: 0x060000B3 RID: 179
		[EngineMethod("finalize_editing", false)]
		void FinalizeEditing(UIntPtr Pointer);
	}
}
