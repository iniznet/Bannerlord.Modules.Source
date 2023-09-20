using System;
using SandBox.ViewModelCollection.GameOver;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	// Token: 0x02000009 RID: 9
	[GameStateScreen(typeof(GameOverState))]
	public class GauntletGameOverScreen : ScreenBase, IGameOverStateHandler, IGameStateListener
	{
		// Token: 0x06000058 RID: 88 RVA: 0x00004C22 File Offset: 0x00002E22
		public GauntletGameOverScreen(GameOverState gameOverState)
		{
			this._gameOverState = gameOverState;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004C31 File Offset: 0x00002E31
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this.CloseGameOverScreen();
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004C58 File Offset: 0x00002E58
		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._gameOverCategory = spriteData.SpriteCategories["ui_gameover"];
			this._gameOverCategory.Load(resourceContext, uiresourceDepot);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", true);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			base.AddLayer(this._gauntletLayer);
			this._dataSource = new GameOverVM(this._gameOverState.Reason, new Action(this.CloseGameOverScreen));
			this._dataSource.SetCloseInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._gauntletLayer.LoadMovie("GameOverScreen", this._dataSource);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(15));
			SoundEvent.PlaySound2D("event:/ui/panels/panel_kingdom_open");
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004D7F File Offset: 0x00002F7F
		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
			base.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004DBF File Offset: 0x00002FBF
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004DC1 File Offset: 0x00002FC1
		void IGameStateListener.OnFinalize()
		{
			this._gameOverCategory.Unload();
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004DE7 File Offset: 0x00002FE7
		private void CloseGameOverScreen()
		{
			if (false || Game.Current.IsDevelopmentMode || this._gameOverState.Reason == 2)
			{
				Game.Current.GameStateManager.PopState(0);
				return;
			}
			MBGameManager.EndGame();
		}

		// Token: 0x04000030 RID: 48
		private const string _panelOpenSound = "event:/ui/panels/panel_kingdom_open";

		// Token: 0x04000031 RID: 49
		private SpriteCategory _gameOverCategory;

		// Token: 0x04000032 RID: 50
		private GameOverVM _dataSource;

		// Token: 0x04000033 RID: 51
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000034 RID: 52
		private readonly GameOverState _gameOverState;
	}
}
