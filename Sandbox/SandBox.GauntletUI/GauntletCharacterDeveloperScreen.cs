using System;
using SandBox.View;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	// Token: 0x02000005 RID: 5
	[GameStateScreen(typeof(CharacterDeveloperState))]
	public class GauntletCharacterDeveloperScreen : ScreenBase, IGameStateListener, IChangeableScreen, ICharacterDeveloperStateHandler
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000021F0 File Offset: 0x000003F0
		public GauntletCharacterDeveloperScreen(CharacterDeveloperState clanState)
		{
			this._characterDeveloperState = clanState;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002200 File Offset: 0x00000400
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit") || this._gauntletLayer.Input.IsGameKeyPressed(37))
			{
				if (this._dataSource.CurrentCharacter.IsInspectingAnAttribute)
				{
					this._dataSource.CurrentCharacter.ExecuteStopInspectingCurrentAttribute();
					return;
				}
				if (this._dataSource.CurrentCharacter.PerkSelection.IsActive)
				{
					this._dataSource.CurrentCharacter.PerkSelection.ExecuteDeactivate();
					return;
				}
				this.CloseCharacterDeveloperScreen();
				return;
			}
			else
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
				{
					this.ExecuteConfirm();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyReleased("Reset"))
				{
					this.ExecuteReset();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToPreviousTab"))
				{
					this.ExecuteSwitchToPreviousTab();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToNextTab"))
				{
					this.ExecuteSwitchToNextTab();
				}
				return;
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002314 File Offset: 0x00000514
		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._characterdeveloper = spriteData.SpriteCategories["ui_characterdeveloper"];
			this._characterdeveloper.Load(resourceContext, uiresourceDepot);
			this._dataSource = new CharacterDeveloperVM(new Action(this.CloseCharacterDeveloperScreen));
			this._dataSource.SetGetKeyTextFromKeyIDFunc(new Func<string, TextObject>(Game.Current.GameTextManager.GetHotKeyGameTextFromKeyID));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._dataSource.SetPreviousCharacterInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this._dataSource.SetNextCharacterInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			if (this._characterDeveloperState.InitialSelectedHero != null)
			{
				this._dataSource.SelectHero(this._characterDeveloperState.InitialSelectedHero);
			}
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", true);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._gauntletLayer.LoadMovie("CharacterDeveloper", this._dataSource);
			base.AddLayer(this._gauntletLayer);
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(3));
			SoundEvent.PlaySound2D("event:/ui/panels/panel_character_open");
			this._gauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(2, null);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000251A File Offset: 0x0000071A
		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
			base.RemoveLayer(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000255A File Offset: 0x0000075A
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000255C File Offset: 0x0000075C
		void IGameStateListener.OnFinalize()
		{
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
			this._characterdeveloper.Unload();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002582 File Offset: 0x00000782
		private void CloseCharacterDeveloperScreen()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002594 File Offset: 0x00000794
		private void ExecuteConfirm()
		{
			this._dataSource.ExecuteDone();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000025A1 File Offset: 0x000007A1
		private void ExecuteReset()
		{
			this._dataSource.ExecuteReset();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000025AE File Offset: 0x000007AE
		private void ExecuteSwitchToPreviousTab()
		{
			this._dataSource.CharacterList.ExecuteSelectPreviousItem();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000025C0 File Offset: 0x000007C0
		private void ExecuteSwitchToNextTab()
		{
			this._dataSource.CharacterList.ExecuteSelectNextItem();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000025D2 File Offset: 0x000007D2
		bool IChangeableScreen.AnyUnsavedChanges()
		{
			return this._dataSource.IsThereAnyChanges();
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000025DF File Offset: 0x000007DF
		bool IChangeableScreen.CanChangesBeApplied()
		{
			return true;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000025E2 File Offset: 0x000007E2
		void IChangeableScreen.ApplyChanges()
		{
			this._dataSource.ApplyAllChanges();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000025EF File Offset: 0x000007EF
		void IChangeableScreen.ResetChanges()
		{
			this._dataSource.ExecuteReset();
		}

		// Token: 0x04000002 RID: 2
		private const string _panelOpenSound = "event:/ui/panels/panel_character_open";

		// Token: 0x04000003 RID: 3
		private CharacterDeveloperVM _dataSource;

		// Token: 0x04000004 RID: 4
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000005 RID: 5
		private SpriteCategory _characterdeveloper;

		// Token: 0x04000006 RID: 6
		private readonly CharacterDeveloperState _characterDeveloperState;
	}
}
