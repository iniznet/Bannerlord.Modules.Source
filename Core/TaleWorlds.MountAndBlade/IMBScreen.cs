using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200019B RID: 411
	[ScriptingInterfaceBase]
	internal interface IMBScreen
	{
		// Token: 0x060016DB RID: 5851
		[EngineMethod("on_exit_button_click", false)]
		void OnExitButtonClick();

		// Token: 0x060016DC RID: 5852
		[EngineMethod("on_edit_mode_enter_press", false)]
		void OnEditModeEnterPress();

		// Token: 0x060016DD RID: 5853
		[EngineMethod("on_edit_mode_enter_release", false)]
		void OnEditModeEnterRelease();
	}
}
