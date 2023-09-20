using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A7 RID: 423
	[ScriptingInterfaceBase]
	internal interface IMBBannerlordTableauManager
	{
		// Token: 0x06001730 RID: 5936
		[EngineMethod("request_character_tableau_render", false)]
		void RequestCharacterTableauRender(int characterCodeId, string path, UIntPtr poseEntity, UIntPtr cameraObject, int tableauType);

		// Token: 0x06001731 RID: 5937
		[EngineMethod("initialize_character_tableau_render_system", false)]
		void InitializeCharacterTableauRenderSystem();

		// Token: 0x06001732 RID: 5938
		[EngineMethod("get_number_of_pending_tableau_requests", false)]
		int GetNumberOfPendingTableauRequests();
	}
}
