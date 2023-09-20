using System;
using SandBox.View;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	// Token: 0x0200000C RID: 12
	[GameStateScreen(typeof(PartyState))]
	public class GauntletPartyScreen : ScreenBase, IGameStateListener, IChangeableScreen
	{
		// Token: 0x0600008C RID: 140 RVA: 0x00005DB2 File Offset: 0x00003FB2
		public GauntletPartyScreen(PartyState partyState)
		{
			this._partyState = partyState;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005DC4 File Offset: 0x00003FC4
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
			this._dataSource.IsFiveStackModifierActive = this._gauntletLayer.Input.IsHotKeyDown("FiveStackModifier");
			this._dataSource.IsEntireStackModifierActive = this._gauntletLayer.Input.IsHotKeyDown("EntireStackModifier");
			if (!this._partyState.IsActive || this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit") || (!this._gauntletLayer.Input.IsControlDown() && this._gauntletLayer.Input.IsGameKeyDownAndReleased(43)))
			{
				this.HandleCancelInput();
				return;
			}
			if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
			{
				this.HandleDoneInput();
				return;
			}
			if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Reset"))
			{
				this.HandleResetInput();
				return;
			}
			if (!this._dataSource.IsAnyPopUpOpen)
			{
				if (this._gauntletLayer.Input.IsHotKeyPressed("TakeAllTroops"))
				{
					this._dataSource.ExecuteTransferAllOtherTroops();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyPressed("GiveAllTroops"))
				{
					this._dataSource.ExecuteTransferAllMainTroops();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyPressed("TakeAllPrisoners"))
				{
					if (this._dataSource.CurrentFocusedCharacter == null || !Input.IsGamepadActive)
					{
						this._dataSource.ExecuteTransferAllOtherPrisoners();
						return;
					}
					if (this._dataSource.CurrentFocusedCharacter.IsTroopTransferrable && this._dataSource.CurrentFocusedCharacter.Side == null)
					{
						this._dataSource.CurrentFocusedCharacter.ExecuteTransferSingle();
						return;
					}
				}
				else if (this._gauntletLayer.Input.IsHotKeyPressed("GiveAllPrisoners"))
				{
					if (this._dataSource.CurrentFocusedCharacter == null || !Input.IsGamepadActive)
					{
						this._dataSource.ExecuteTransferAllMainPrisoners();
						return;
					}
					if (this._dataSource.CurrentFocusedCharacter.IsTroopTransferrable && this._dataSource.CurrentFocusedCharacter.Side == 1)
					{
						this._dataSource.CurrentFocusedCharacter.ExecuteTransferSingle();
						return;
					}
				}
				else if (this._gauntletLayer.Input.IsHotKeyPressed("OpenUpgradePopup"))
				{
					if (!this._dataSource.IsUpgradePopUpDisabled)
					{
						this._dataSource.ExecuteOpenUpgradePopUp();
						return;
					}
				}
				else if (this._gauntletLayer.Input.IsHotKeyPressed("OpenRecruitPopup"))
				{
					if (!this._dataSource.IsRecruitPopUpDisabled)
					{
						this._dataSource.ExecuteOpenRecruitPopUp();
						return;
					}
				}
				else if (this._gauntletLayer.Input.IsGameKeyReleased(39) && this._dataSource.CurrentFocusedCharacter != null && Input.IsGamepadActive)
				{
					this._dataSource.CurrentFocusedCharacter.ExecuteOpenTroopEncyclopedia();
					return;
				}
			}
			else if (this._gauntletLayer.Input.IsHotKeyPressed("PopupItemPrimaryAction"))
			{
				PartyUpgradeTroopVM upgradePopUp = this._dataSource.UpgradePopUp;
				if (upgradePopUp != null && upgradePopUp.IsOpen)
				{
					this._dataSource.UpgradePopUp.ExecuteItemPrimaryAction();
					return;
				}
				PartyRecruitTroopVM recruitPopUp = this._dataSource.RecruitPopUp;
				if (recruitPopUp != null && recruitPopUp.IsOpen)
				{
					this._dataSource.RecruitPopUp.ExecuteItemPrimaryAction();
					return;
				}
			}
			else if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("PopupItemSecondaryAction"))
			{
				PartyUpgradeTroopVM upgradePopUp2 = this._dataSource.UpgradePopUp;
				if (upgradePopUp2 != null && upgradePopUp2.IsOpen)
				{
					this._dataSource.UpgradePopUp.ExecuteItemSecondaryAction();
					return;
				}
				PartyRecruitTroopVM recruitPopUp2 = this._dataSource.RecruitPopUp;
				if (recruitPopUp2 != null && recruitPopUp2.IsOpen)
				{
					this._dataSource.RecruitPopUp.ExecuteItemSecondaryAction();
					return;
				}
			}
			else if (Input.IsGamepadActive && this._gauntletLayer.Input.IsGameKeyReleased(39))
			{
				PartyRecruitTroopVM recruitPopUp3 = this._dataSource.RecruitPopUp;
				if (recruitPopUp3 != null && recruitPopUp3.IsOpen && this._dataSource.RecruitPopUp.FocusedTroop != null)
				{
					this._dataSource.RecruitPopUp.FocusedTroop.PartyCharacter.ExecuteOpenTroopEncyclopedia();
					return;
				}
				PartyUpgradeTroopVM upgradePopUp3 = this._dataSource.UpgradePopUp;
				if (upgradePopUp3 != null && upgradePopUp3.IsOpen)
				{
					if (this._dataSource.UpgradePopUp.FocusedTroop != null)
					{
						this._dataSource.UpgradePopUp.FocusedTroop.ExecuteOpenTroopEncyclopedia();
						return;
					}
					if (this._dataSource.CurrentFocusedUpgrade != null)
					{
						this._dataSource.CurrentFocusedUpgrade.ExecuteUpgradeEncyclopediaLink();
					}
				}
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00006230 File Offset: 0x00004430
		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._partyscreenCategory = spriteData.SpriteCategories["ui_partyscreen"];
			this._partyscreenCategory.Load(resourceContext, uiresourceDepot);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", true);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("PartyHotKeyCategory"));
			this._dataSource = new PartyVM(this._partyState.PartyScreenLogic);
			this._dataSource.SetGetKeyTextFromKeyIDFunc(new Func<string, TextObject>(Game.Current.GameTextManager.GetHotKeyGameTextFromKeyID));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetTakeAllTroopsInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("TakeAllTroops"));
			this._dataSource.SetDismissAllTroopsInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("GiveAllTroops"));
			this._dataSource.SetTakeAllPrisonersInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("TakeAllPrisoners"));
			this._dataSource.SetDismissAllPrisonersInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("GiveAllPrisoners"));
			this._dataSource.SetOpenUpgradePanelInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("OpenUpgradePopup"));
			this._dataSource.SetOpenRecruitPanelInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("OpenRecruitPopup"));
			this._dataSource.UpgradePopUp.SetPrimaryActionInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("PopupItemPrimaryAction"));
			this._dataSource.UpgradePopUp.SetSecondaryActionInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("PopupItemSecondaryAction"));
			this._dataSource.RecruitPopUp.SetPrimaryActionInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("PopupItemPrimaryAction"));
			this._dataSource.RecruitPopUp.SetSecondaryActionInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("PopupItemSecondaryAction"));
			string fiveStackShortcutkeyText = this.GetFiveStackShortcutkeyText();
			this._dataSource.SetFiveStackShortcutKeyText(fiveStackShortcutkeyText);
			string entireStackShortcutkeyText = this.GetEntireStackShortcutkeyText();
			this._dataSource.SetEntireStackShortcutKeyText(entireStackShortcutkeyText);
			this._partyState.Handler = this._dataSource;
			this._gauntletLayer.LoadMovie("PartyScreen", this._dataSource);
			base.AddLayer(this._gauntletLayer);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(1));
			SoundEvent.PlaySound2D("event:/ui/panels/panel_party_open");
			this._gauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(2, null);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00006574 File Offset: 0x00004774
		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
			PartyBase.MainParty.Visuals.SetMapIconAsDirty();
			this._gauntletLayer.IsFocusLayer = false;
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			base.RemoveLayer(this._gauntletLayer);
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
			if (Campaign.Current.ConversationManager.IsConversationInProgress && !Campaign.Current.ConversationManager.IsConversationFlowActive)
			{
				Campaign.Current.ConversationManager.OnConversationActivate();
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000660F File Offset: 0x0000480F
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00006611 File Offset: 0x00004811
		void IGameStateListener.OnFinalize()
		{
			this._dataSource.OnFinalize();
			this._partyscreenCategory.Unload();
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00006637 File Offset: 0x00004837
		private void HandleResetInput()
		{
			if (!this._dataSource.IsAnyPopUpOpen)
			{
				this._dataSource.ExecuteReset();
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00006654 File Offset: 0x00004854
		private void HandleCancelInput()
		{
			PartyUpgradeTroopVM upgradePopUp = this._dataSource.UpgradePopUp;
			if (upgradePopUp != null && upgradePopUp.IsOpen)
			{
				this._dataSource.UpgradePopUp.ExecuteCancel();
				return;
			}
			PartyRecruitTroopVM recruitPopUp = this._dataSource.RecruitPopUp;
			if (recruitPopUp != null && recruitPopUp.IsOpen)
			{
				this._dataSource.RecruitPopUp.ExecuteCancel();
				return;
			}
			this._partyState.PartyScreenLogic.Reset(true);
			PartyScreenManager.CloseScreen(false, true);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000066D0 File Offset: 0x000048D0
		private void HandleDoneInput()
		{
			PartyUpgradeTroopVM upgradePopUp = this._dataSource.UpgradePopUp;
			if (upgradePopUp != null && upgradePopUp.IsOpen)
			{
				this._dataSource.UpgradePopUp.ExecuteDone();
				return;
			}
			PartyRecruitTroopVM recruitPopUp = this._dataSource.RecruitPopUp;
			if (recruitPopUp != null && recruitPopUp.IsOpen)
			{
				this._dataSource.RecruitPopUp.ExecuteDone();
				return;
			}
			this._dataSource.ExecuteDone();
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000673C File Offset: 0x0000493C
		private string GetFiveStackShortcutkeyText()
		{
			if (!Input.IsControllerConnected || Input.IsMouseActive)
			{
				return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anyshift").ToString();
			}
			return string.Empty;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00006776 File Offset: 0x00004976
		private string GetEntireStackShortcutkeyText()
		{
			if (!Input.IsControllerConnected || Input.IsMouseActive)
			{
				return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anycontrol").ToString();
			}
			return null;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000067AC File Offset: 0x000049AC
		bool IChangeableScreen.AnyUnsavedChanges()
		{
			return this._partyState.PartyScreenLogic.IsThereAnyChanges();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000067BE File Offset: 0x000049BE
		bool IChangeableScreen.CanChangesBeApplied()
		{
			return this._partyState.PartyScreenLogic.IsDoneActive();
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000067D0 File Offset: 0x000049D0
		void IChangeableScreen.ApplyChanges()
		{
			this._partyState.PartyScreenLogic.DoneLogic(true);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000067E4 File Offset: 0x000049E4
		void IChangeableScreen.ResetChanges()
		{
			this._partyState.PartyScreenLogic.Reset(true);
		}

		// Token: 0x04000045 RID: 69
		private const string _panelOpenSound = "event:/ui/panels/panel_party_open";

		// Token: 0x04000046 RID: 70
		private PartyVM _dataSource;

		// Token: 0x04000047 RID: 71
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000048 RID: 72
		private SpriteCategory _partyscreenCategory;

		// Token: 0x04000049 RID: 73
		private readonly PartyState _partyState;
	}
}
