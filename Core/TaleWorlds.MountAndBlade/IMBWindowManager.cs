using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A1 RID: 417
	[ScriptingInterfaceBase]
	internal interface IMBWindowManager
	{
		// Token: 0x060016F7 RID: 5879
		[EngineMethod("erase_message_lines", false)]
		void EraseMessageLines();

		// Token: 0x060016F8 RID: 5880
		[EngineMethod("world_to_screen", false)]
		float WorldToScreen(UIntPtr cameraPointer, Vec3 worldSpacePosition, ref float screenX, ref float screenY, ref float w);

		// Token: 0x060016F9 RID: 5881
		[EngineMethod("world_to_screen_with_fixed_z", false)]
		float WorldToScreenWithFixedZ(UIntPtr cameraPointer, Vec3 cameraPosition, Vec3 worldSpacePosition, ref float screenX, ref float screenY, ref float w);

		// Token: 0x060016FA RID: 5882
		[EngineMethod("dont_change_cursor_pos", false)]
		void DontChangeCursorPos();

		// Token: 0x060016FB RID: 5883
		[EngineMethod("pre_display", false)]
		void PreDisplay();

		// Token: 0x060016FC RID: 5884
		[EngineMethod("screen_to_world", false)]
		void ScreenToWorld(UIntPtr pointer, float screenX, float screenY, float z, ref Vec3 worldSpacePosition);
	}
}
