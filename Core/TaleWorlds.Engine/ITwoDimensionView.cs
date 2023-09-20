using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000033 RID: 51
	[ApplicationInterfaceBase]
	internal interface ITwoDimensionView
	{
		// Token: 0x06000465 RID: 1125
		[EngineMethod("create_twodimension_view", false)]
		TwoDimensionView CreateTwoDimensionView();

		// Token: 0x06000466 RID: 1126
		[EngineMethod("begin_frame", false)]
		void BeginFrame(UIntPtr pointer);

		// Token: 0x06000467 RID: 1127
		[EngineMethod("end_frame", false)]
		void EndFrame(UIntPtr pointer);

		// Token: 0x06000468 RID: 1128
		[EngineMethod("clear", false)]
		void Clear(UIntPtr pointer);

		// Token: 0x06000469 RID: 1129
		[EngineMethod("add_new_mesh", false)]
		void AddNewMesh(UIntPtr pointer, float[] vertices, float[] uvs, uint[] indices, int vertexCount, int indexCount, UIntPtr material, ref TwoDimensionMeshDrawData meshDrawData);

		// Token: 0x0600046A RID: 1130
		[EngineMethod("add_new_quad_mesh", false)]
		void AddNewQuadMesh(UIntPtr pointer, UIntPtr material, ref TwoDimensionMeshDrawData meshDrawData);

		// Token: 0x0600046B RID: 1131
		[EngineMethod("add_cached_text_mesh", false)]
		bool AddCachedTextMesh(UIntPtr pointer, UIntPtr material, ref TwoDimensionTextMeshDrawData meshDrawData);

		// Token: 0x0600046C RID: 1132
		[EngineMethod("add_new_text_mesh", false)]
		void AddNewTextMesh(UIntPtr pointer, float[] vertices, float[] uvs, uint[] indices, int vertexCount, int indexCount, UIntPtr material, ref TwoDimensionTextMeshDrawData meshDrawData);
	}
}
