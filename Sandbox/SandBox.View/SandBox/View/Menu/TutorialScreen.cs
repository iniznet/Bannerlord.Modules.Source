using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.Menu
{
	// Token: 0x02000037 RID: 55
	[GameStateScreen(typeof(TutorialState))]
	public class TutorialScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060001BF RID: 447 RVA: 0x00011F17 File Offset: 0x00010117
		public MenuViewContext MenuViewContext { get; }

		// Token: 0x060001C0 RID: 448 RVA: 0x00011F1F File Offset: 0x0001011F
		public TutorialScreen(TutorialState tutorialState)
		{
			this.MenuViewContext = new MenuViewContext(this, tutorialState.MenuContext);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00011F39 File Offset: 0x00010139
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this.MenuViewContext.OnFrameTick(dt);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00011F4E File Offset: 0x0001014E
		protected override void OnActivate()
		{
			base.OnActivate();
			this.MenuViewContext.OnActivate();
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00011F66 File Offset: 0x00010166
		protected override void OnDeactivate()
		{
			this.MenuViewContext.OnDeactivate();
			base.OnDeactivate();
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00011F79 File Offset: 0x00010179
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this.MenuViewContext.OnInitialize();
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00011F8C File Offset: 0x0001018C
		protected override void OnFinalize()
		{
			this.MenuViewContext.OnFinalize();
			base.OnFinalize();
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00011F9F File Offset: 0x0001019F
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00011FA1 File Offset: 0x000101A1
		void IGameStateListener.OnDeactivate()
		{
			this.MenuViewContext.OnGameStateDeactivate();
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00011FAE File Offset: 0x000101AE
		void IGameStateListener.OnInitialize()
		{
			this.MenuViewContext.OnGameStateInitialize();
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00011FBB File Offset: 0x000101BB
		void IGameStateListener.OnFinalize()
		{
			this.MenuViewContext.OnGameStateFinalize();
		}
	}
}
