using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200007F RID: 127
	public static class Screen
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060009CA RID: 2506 RVA: 0x0000A857 File Offset: 0x00008A57
		// (set) Token: 0x060009CB RID: 2507 RVA: 0x0000A85E File Offset: 0x00008A5E
		public static float RealScreenResolutionWidth { get; private set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060009CC RID: 2508 RVA: 0x0000A866 File Offset: 0x00008A66
		// (set) Token: 0x060009CD RID: 2509 RVA: 0x0000A86D File Offset: 0x00008A6D
		public static float RealScreenResolutionHeight { get; private set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060009CE RID: 2510 RVA: 0x0000A875 File Offset: 0x00008A75
		public static Vec2 RealScreenResolution
		{
			get
			{
				return new Vec2(Screen.RealScreenResolutionWidth, Screen.RealScreenResolutionHeight);
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060009CF RID: 2511 RVA: 0x0000A886 File Offset: 0x00008A86
		// (set) Token: 0x060009D0 RID: 2512 RVA: 0x0000A88D File Offset: 0x00008A8D
		public static float AspectRatio { get; private set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060009D1 RID: 2513 RVA: 0x0000A895 File Offset: 0x00008A95
		// (set) Token: 0x060009D2 RID: 2514 RVA: 0x0000A89C File Offset: 0x00008A9C
		public static Vec2 DesktopResolution { get; private set; }

		// Token: 0x060009D3 RID: 2515 RVA: 0x0000A8A4 File Offset: 0x00008AA4
		internal static void Update()
		{
			Screen.RealScreenResolutionWidth = EngineApplicationInterface.IScreen.GetRealScreenResolutionWidth();
			Screen.RealScreenResolutionHeight = EngineApplicationInterface.IScreen.GetRealScreenResolutionHeight();
			Screen.AspectRatio = EngineApplicationInterface.IScreen.GetAspectRatio();
			Screen.DesktopResolution = new Vec2(EngineApplicationInterface.IScreen.GetDesktopWidth(), EngineApplicationInterface.IScreen.GetDesktopHeight());
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x0000A8FC File Offset: 0x00008AFC
		public static bool GetMouseVisible()
		{
			return EngineApplicationInterface.IScreen.GetMouseVisible();
		}
	}
}
