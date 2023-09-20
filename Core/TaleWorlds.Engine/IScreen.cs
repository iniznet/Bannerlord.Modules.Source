using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200003A RID: 58
	[ApplicationInterfaceBase]
	internal interface IScreen
	{
		// Token: 0x0600051D RID: 1309
		[EngineMethod("get_real_screen_resolution_width", false)]
		float GetRealScreenResolutionWidth();

		// Token: 0x0600051E RID: 1310
		[EngineMethod("get_real_screen_resolution_height", false)]
		float GetRealScreenResolutionHeight();

		// Token: 0x0600051F RID: 1311
		[EngineMethod("get_desktop_width", false)]
		float GetDesktopWidth();

		// Token: 0x06000520 RID: 1312
		[EngineMethod("get_desktop_height", false)]
		float GetDesktopHeight();

		// Token: 0x06000521 RID: 1313
		[EngineMethod("get_aspect_ratio", false)]
		float GetAspectRatio();

		// Token: 0x06000522 RID: 1314
		[EngineMethod("get_mouse_visible", false)]
		bool GetMouseVisible();

		// Token: 0x06000523 RID: 1315
		[EngineMethod("set_mouse_visible", false)]
		void SetMouseVisible(bool value);

		// Token: 0x06000524 RID: 1316
		[EngineMethod("get_usable_area_percentages", false)]
		Vec2 GetUsableAreaPercentages();

		// Token: 0x06000525 RID: 1317
		[EngineMethod("is_enter_button_cross", false)]
		bool IsEnterButtonCross();
	}
}
