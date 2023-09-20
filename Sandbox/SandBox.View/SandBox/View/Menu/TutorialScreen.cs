using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.Menu
{
	[GameStateScreen(typeof(TutorialState))]
	public class TutorialScreen : ScreenBase, IGameStateListener
	{
		public MenuViewContext MenuViewContext { get; }

		public TutorialScreen(TutorialState tutorialState)
		{
			this.MenuViewContext = new MenuViewContext(this, tutorialState.MenuContext);
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this.MenuViewContext.OnFrameTick(dt);
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this.MenuViewContext.OnActivate();
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		protected override void OnDeactivate()
		{
			this.MenuViewContext.OnDeactivate();
			base.OnDeactivate();
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this.MenuViewContext.OnInitialize();
		}

		protected override void OnFinalize()
		{
			this.MenuViewContext.OnFinalize();
			base.OnFinalize();
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
			this.MenuViewContext.OnGameStateDeactivate();
		}

		void IGameStateListener.OnInitialize()
		{
			this.MenuViewContext.OnGameStateInitialize();
		}

		void IGameStateListener.OnFinalize()
		{
			this.MenuViewContext.OnGameStateFinalize();
		}
	}
}
