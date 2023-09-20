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
	// Token: 0x0200000D RID: 13
	[GameStateScreen(typeof(QuestsState))]
	public class GauntletQuestsScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x0600009B RID: 155 RVA: 0x000067F7 File Offset: 0x000049F7
		public GauntletQuestsScreen(QuestsState questsState)
		{
			this._questsState = questsState;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006808 File Offset: 0x00004A08
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
			if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit") || this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm") || this._gauntletLayer.Input.IsGameKeyDownAndReleased(42))
			{
				this._dataSource.ExecuteClose();
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00006870 File Offset: 0x00004A70
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

		// Token: 0x0600009E RID: 158 RVA: 0x00006A28 File Offset: 0x00004C28
		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
			this._questCategory.Unload();
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			base.RemoveLayer(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006A7E File Offset: 0x00004C7E
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00006A80 File Offset: 0x00004C80
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

		// Token: 0x060000A1 RID: 161 RVA: 0x00006AA1 File Offset: 0x00004CA1
		private void CloseQuestsScreen()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x0400004A RID: 74
		private const string _panelOpenSound = "event:/ui/panels/panel_quest_open";

		// Token: 0x0400004B RID: 75
		private QuestsVM _dataSource;

		// Token: 0x0400004C RID: 76
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400004D RID: 77
		private SpriteCategory _questCategory;

		// Token: 0x0400004E RID: 78
		private readonly QuestsState _questsState;
	}
}
