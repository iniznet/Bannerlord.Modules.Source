using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000021 RID: 33
	[ApplicationInterfaceBase]
	internal interface IMeshBuilder
	{
		// Token: 0x060001B4 RID: 436
		[EngineMethod("create_tiling_window_mesh", false)]
		Mesh CreateTilingWindowMesh(string baseMeshName, ref Vec2 meshSizeMin, ref Vec2 meshSizeMax, ref Vec2 borderThickness, ref Vec2 backgroundBorderThickness);

		// Token: 0x060001B5 RID: 437
		[EngineMethod("create_tiling_button_mesh", false)]
		Mesh CreateTilingButtonMesh(string baseMeshName, ref Vec2 meshSizeMin, ref Vec2 meshSizeMax, ref Vec2 borderThickness);

		// Token: 0x060001B6 RID: 438
		[EngineMethod("finalize_mesh_builder", false)]
		Mesh FinalizeMeshBuilder(int num_vertices, Vec3[] vertices, int num_face_corners, MeshBuilder.FaceCorner[] faceCorners, int num_faces, MeshBuilder.Face[] faces);
	}
}
