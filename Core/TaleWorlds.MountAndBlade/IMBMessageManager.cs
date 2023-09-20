using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A0 RID: 416
	[ScriptingInterfaceBase]
	internal interface IMBMessageManager
	{
		// Token: 0x060016F4 RID: 5876
		[EngineMethod("display_message", false)]
		void DisplayMessage(string message);

		// Token: 0x060016F5 RID: 5877
		[EngineMethod("display_message_with_color", false)]
		void DisplayMessageWithColor(string message, uint color);

		// Token: 0x060016F6 RID: 5878
		[EngineMethod("set_message_manager", false)]
		void SetMessageManager(MessageManagerBase messageManager);
	}
}
