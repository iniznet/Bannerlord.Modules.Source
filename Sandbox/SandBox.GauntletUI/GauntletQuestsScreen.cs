using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.Quests;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	[GameStateScreen(typeof(QuestsState))]
	public class GauntletQuestsScreen : ScreenBase, IGameStateListener
	{
		public GauntletQuestsScreen(QuestsState questsState)
		{
			this._questsState = questsState;
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
			if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit") || this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm") || this._gauntletLayer.Input.IsGameKeyDownAndReleased(42))
			{
				this._dataSource.ExecuteClose();
			}
		}

		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._questCategory = spriteData.SpriteCategories["ui_quest"];
			this._questCategory.Load(resourceContext, uiresourceDepot);
			this._dataSource = new QuestsVM(new Action(this.CloseQuestsScreen));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", true);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._gauntletLayer.LoadMovie("QuestsScreen", this._dataSource);
			this._gauntletLayer.IsFocusLayer = true;
			base.AddLayer(this._gauntletLayer);
			ScreenManager.TrySetFocus(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(11));
			if (this._questsState.InitialSelectedIssue != null)
			{
				this._dataSource.SetSelectedIssue(this._questsState.InitialSelectedIssue);
			}
			else if (this._questsState.InitialSelectedQuest != null)
			{
				this._dataSource.SetSelectedQuest(this._questsState.InitialSelectedQuest);
			}
			else if (this._questsState.InitialSelectedLog != null)
			{
				this._dataSource.SetSelectedLog(this._questsState.InitialSelectedLog);
			}
			SoundEvent.PlaySound2D("event:/ui/panels/panel_quest_open");
			this._gauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(2, null);
		}

		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
			this._questCategory.Unload();
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			base.RemoveLayer(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
			QuestsVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		private void CloseQuestsScreen()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		private const string _panelOpenSound = "event:/ui/panels/panel_quest_open";

		private QuestsVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private SpriteCategory _questCategory;

		private readonly QuestsState _questsState;
	}
}
