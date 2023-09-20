using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200018D RID: 397
	[ScriptingInterfaceBase]
	internal interface IMBTestRun
	{
		// Token: 0x060014C6 RID: 5318
		[EngineMethod("auto_continue", false)]
		int AutoContinue(int type);

		// Token: 0x060014C7 RID: 5319
		[EngineMethod("get_fps", false)]
		int GetFPS();

		// Token: 0x060014C8 RID: 5320
		[EngineMethod("enter_edit_mode", false)]
		bool EnterEditMode();

		// Token: 0x060014C9 RID: 5321
		[EngineMethod("open_scene", false)]
		bool OpenScene(string sceneName);

		// Token: 0x060014CA RID: 5322
		[EngineMethod("close_scene", false)]
		bool CloseScene();

		// Token: 0x060014CB RID: 5323
		[EngineMethod("leave_edit_mode", false)]
		bool LeaveEditMode();

		// Token: 0x060014CC RID: 5324
		[EngineMethod("new_scene", false)]
		bool NewScene();

		// Token: 0x060014CD RID: 5325
		[EngineMethod("start_mission", false)]
		void StartMission();
	}
}
