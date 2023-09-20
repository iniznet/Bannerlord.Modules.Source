using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000031 RID: 49
	[ApplicationInterfaceBase]
	internal interface IThumbnailCreatorView
	{
		// Token: 0x0600043E RID: 1086
		[EngineMethod("create_thumbnail_creator_view", false)]
		ThumbnailCreatorView CreateThumbnailCreatorView();

		// Token: 0x0600043F RID: 1087
		[EngineMethod("register_scene", false)]
		void RegisterScene(UIntPtr pointer, UIntPtr scene_ptr, bool use_postfx);

		// Token: 0x06000440 RID: 1088
		[EngineMethod("clear_requests", false)]
		void ClearRequests(UIntPtr pointer);

		// Token: 0x06000441 RID: 1089
		[EngineMethod("cancel_request", false)]
		void CancelRequest(UIntPtr pointer, string render_id);

		// Token: 0x06000442 RID: 1090
		[EngineMethod("register_entity", false)]
		void RegisterEntity(UIntPtr pointer, UIntPtr scene_ptr, UIntPtr cam_ptr, UIntPtr texture_ptr, UIntPtr entity_ptr, string render_id, int allocationGroupIndex);

		// Token: 0x06000443 RID: 1091
		[EngineMethod("register_entity_without_texture", false)]
		void RegisterEntityWithoutTexture(UIntPtr pointer, UIntPtr scene_ptr, UIntPtr cam_ptr, UIntPtr entity_ptr, int width, int height, string render_id, string debug_name, int allocationGroupIndex);

		// Token: 0x06000444 RID: 1092
		[EngineMethod("get_number_of_pending_requests", false)]
		int GetNumberOfPendingRequests(UIntPtr pointer);

		// Token: 0x06000445 RID: 1093
		[EngineMethod("is_memory_cleared", false)]
		bool IsMemoryCleared(UIntPtr pointer);
	}
}
