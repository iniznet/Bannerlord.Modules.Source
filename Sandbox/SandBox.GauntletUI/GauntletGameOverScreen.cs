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
	[GameStateScreen(typeof(GameOverState))]
	public class GauntletGameOverScreen : ScreenBase, IGameOverStateHandler, IGameStateListener
	{
		public GauntletGameOverScreen(GameOverState gameOverState)
		{
			this._gameOverState = gameOverState;
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this.CloseGameOverScreen();
			}
		}

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

		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
			base.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
			this._gameOverCategory.Unload();
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		private void CloseGameOverScreen()
		{
			if (false || Game.Current.IsDevelopmentMode || this._gameOverState.Reason == 2)
			{
				Game.Current.GameStateManager.PopState(0);
				return;
			}
			MBGameManager.EndGame();
		}

		private const string _panelOpenSound = "event:/ui/panels/panel_kingdom_open";

		private SpriteCategory _gameOverCategory;

		private GameOverVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private readonly GameOverState _gameOverState;
	}
}
