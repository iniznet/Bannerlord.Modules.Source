using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x0200002C RID: 44
	[GameStateScreen(typeof(GameLoadingState))]
	public class GameLoadingScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x060001B5 RID: 437 RVA: 0x0000F002 File Offset: 0x0000D202
		public GameLoadingScreen(GameLoadingState gameLoadingState)
		{
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000F00A File Offset: 0x0000D20A
		protected override void OnActivate()
		{
			base.OnActivate();
			LoadingWindow.EnableGlobalLoadingWindow();
			Utilities.SetScreenTextRenderingState(false);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000F01D File Offset: 0x0000D21D
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			Utilities.SetScreenTextRenderingState(true);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000F02C File Offset: 0x0000D22C
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000F02E File Offset: 0x0000D22E
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000F030 File Offset: 0x0000D230
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000F032 File Offset: 0x0000D232
		void IGameStateListener.OnFinalize()
		{
		}
	}
}
