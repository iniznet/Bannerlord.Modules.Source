using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200019F RID: 415
	[ScriptingInterfaceBase]
	internal interface IMBWorld
	{
		// Token: 0x060016E9 RID: 5865
		[EngineMethod("get_global_time", false)]
		float GetGlobalTime(MBCommon.TimeType timeType);

		// Token: 0x060016EA RID: 5866
		[EngineMethod("get_last_messages", false)]
		string GetLastMessages();

		// Token: 0x060016EB RID: 5867
		[EngineMethod("get_game_type", false)]
		int GetGameType();

		// Token: 0x060016EC RID: 5868
		[EngineMethod("set_game_type", false)]
		void SetGameType(int gameType);

		// Token: 0x060016ED RID: 5869
		[EngineMethod("pause_game", false)]
		void PauseGame();

		// Token: 0x060016EE RID: 5870
		[EngineMethod("unpause_game", false)]
		void UnpauseGame();

		// Token: 0x060016EF RID: 5871
		[EngineMethod("set_mesh_used", false)]
		void SetMeshUsed(string meshName);

		// Token: 0x060016F0 RID: 5872
		[EngineMethod("set_material_used", false)]
		void SetMaterialUsed(string materialName);

		// Token: 0x060016F1 RID: 5873
		[EngineMethod("set_body_used", false)]
		void SetBodyUsed(string bodyName);

		// Token: 0x060016F2 RID: 5874
		[EngineMethod("fix_skeletons", false)]
		void FixSkeletons();

		// Token: 0x060016F3 RID: 5875
		[EngineMethod("check_resource_modifications", false)]
		void CheckResourceModifications();
	}
}
