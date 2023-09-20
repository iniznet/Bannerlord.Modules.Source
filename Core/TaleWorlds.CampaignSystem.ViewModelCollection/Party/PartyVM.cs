using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	// Token: 0x02000028 RID: 40
	public class PartyVM : ViewModel, IPartyScreenLogicHandler, PartyScreenPrisonHandler, IPartyScreenTroopHandler
	{
		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000310 RID: 784 RVA: 0x00013670 File Offset: 0x00011870
		// (set) Token: 0x06000311 RID: 785 RVA: 0x00013678 File Offset: 0x00011878
		public PartyScreenLogic PartyScreenLogic { get; private set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000312 RID: 786 RVA: 0x00013681 File Offset: 0x00011881
		public bool IsAnyPopUpOpen
		{
			get
			{
				PartyUpgradeTroopVM upgradePopUp = this.UpgradePopUp;
				if (upgradePopUp == null || !upgradePopUp.IsOpen)
				{
					PartyRecruitTroopVM recruitPopUp = this.RecruitPopUp;
					return recruitPopUp != null && recruitPopUp.IsOpen;
				}
				return true;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000313 RID: 787 RVA: 0x000136AA File Offset: 0x000118AA
		// (set) Token: 0x06000314 RID: 788 RVA: 0x000136B2 File Offset: 0x000118B2
		[DataSourceProperty]
		public PartyCharacterVM CurrentCharacter
		{
			get
			{
				return this._currentCharacter;
			}
			set
			{
				if (value != null && value != this._currentCharacter)
				{
					this._currentCharacter = value;
					this.RefreshCurrentCharacterInformation();
					base.OnPropertyChangedWithValue<PartyCharacterVM>(value, "CurrentCharacter");
					this.ExecuteRemoveZeroCounts();
				}
				this.ExecuteRemoveZeroCounts();
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000315 RID: 789 RVA: 0x000136E8 File Offset: 0x000118E8
		private List<Tuple<string, TextObject>> FormationNames
		{
			get
			{
				if (this._formationNames == null)
				{
					int num = 8;
					this._formationNames = new List<Tuple<string, TextObject>>(num + 1);
					for (int i = 0; i < num; i++)
					{
						string text = "<img src=\"PartyScreen\\FormationIcons\\" + (i + 1) + "\"/>";
						TextObject textObject = GameTexts.FindText("str_troop_group_name", i.ToString());
						this._formationNames.Add(new Tuple<string, TextObject>(text, textObject));
					}
				}
				return this._formationNames;
			}
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0001375C File Offset: 0x0001195C
		public PartyVM(PartyScreenLogic partyScreenLogic)
		{
			this.PartyScreenLogic = partyScreenLogic;
			this._currentMode = PartyScreenManager.Instance.CurrentMode;
			this._viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			this.OtherPartyTroops = new MBBindingList<PartyCharacterVM>();
			this.OtherPartyPrisoners = new MBBindingList<PartyCharacterVM>();
			this.MainPartyTroops = new MBBindingList<PartyCharacterVM>();
			this.MainPartyPrisoners = new MBBindingList<PartyCharacterVM>();
			this.UpgradePopUp = new PartyUpgradeTroopVM(this);
			this.RecruitPopUp = new PartyRecruitTroopVM(this);
			this.SelectedCharacter = new HeroViewModel(CharacterViewModel.StanceTypes.None);
			this.DoneHint = new HintViewModel();
			this.DenarHint = new HintViewModel();
			this.MoraleHint = new HintViewModel();
			this.SpeedHint = new BasicTooltipViewModel();
			this.TotalWageHint = new HintViewModel();
			this.FormationHint = new HintViewModel();
			PartyCharacterVM.ProcessCharacterLock = new Action<PartyCharacterVM, bool>(this.ProcessCharacterLock);
			PartyCharacterVM.OnFocus = new Action<PartyCharacterVM>(this.OnFocusCharacter);
			PartyCharacterVM.OnShift = null;
			PartyCharacterVM.OnTransfer = new Action<PartyCharacterVM, int, int, PartyScreenLogic.PartyRosterSide>(this.OnTransferTroop);
			PartyCharacterVM.SetSelected = new Action<PartyCharacterVM>(this.SetSelectedCharacter);
			this.OtherPartyComposition = new PartyCompositionVM();
			this.MainPartyComposition = new PartyCompositionVM();
			this.CanChooseRoles = this._currentMode == PartyScreenMode.Normal;
			if (this.PartyScreenLogic != null)
			{
				this.PartyScreenLogic.PartyGoldChange += this.OnPartyGoldChanged;
				this.PartyScreenLogic.PartyHorseChange += this.OnPartyHorseChanged;
				this.PartyScreenLogic.PartyInfluenceChange += this.OnPartyInfluenceChanged;
				this.PartyScreenLogic.PartyMoraleChange += this.OnPartyMoraleChanged;
				this.PartyScreenLogic.UpdateDelegate = new PartyScreenLogic.PresentationUpdate(this.Update);
				this.PartyScreenLogic.AfterReset += this.AfterReset;
				this.ShowQuestProgress = this.PartyScreenLogic.ShowProgressBar;
				if (this.ShowQuestProgress)
				{
					this.QuestProgressRequiredCount = this.PartyScreenLogic.GetCurrentQuestRequiredCount();
					this.IsDoneDisabled = !this.PartyScreenLogic.IsDoneActive();
					this.DoneHint.HintText = new TextObject("{=!}" + this.PartyScreenLogic.DoneReasonString, null);
					this.IsCancelDisabled = !this.PartyScreenLogic.IsCancelActive();
				}
				this.InitializeStaticInformation();
				this.InitializeTroopLists();
				this.RefreshPartyInformation();
			}
			this.UpdateTroopManagerPopUpCounts();
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			Campaign campaign = Campaign.Current;
			if (campaign != null)
			{
				PlayerUpdateTracker playerUpdateTracker = campaign.PlayerUpdateTracker;
				if (playerUpdateTracker != null)
				{
					playerUpdateTracker.ClearPartyNotification();
				}
			}
			this.OtherPartySortController = new PartySortControllerVM(PartyScreenLogic.PartyRosterSide.Left, new Action<PartyScreenLogic.PartyRosterSide, PartyScreenLogic.TroopSortType, bool>(this.OnSortTroops));
			this.MainPartySortController = new PartySortControllerVM(PartyScreenLogic.PartyRosterSide.Right, new Action<PartyScreenLogic.PartyRosterSide, PartyScreenLogic.TroopSortType, bool>(this.OnSortTroops));
			this.MainPartySortController.SortWith((PartyScreenLogic.TroopSortType)this._viewDataTracker.GetPartySortType(), this._viewDataTracker.GetIsPartySortAscending());
			this.RefreshValues();
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00013A78 File Offset: 0x00011C78
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ResetHint = new HintViewModel(GameTexts.FindText("str_reset", null), null);
			this.LevelHint = new HintViewModel(GameTexts.FindText("str_level_tag", null), null);
			this.TitleLbl = GameTexts.FindText("str_party", null).ToString();
			this.OtherPartyAccompanyingLbl = GameTexts.FindText("str_party_list_tag_attached_groups", null).ToString();
			this.MoraleHint.HintText = GameTexts.FindText("str_party_morale", null);
			this.TotalWageHint.HintText = GameTexts.FindText("str_weekly_wage", null);
			this.TalkLbl = GameTexts.FindText("str_talk_button", null).ToString();
			this.InfoLbl = GameTexts.FindText("str_info", null).ToString();
			this.CancelLbl = GameTexts.FindText("str_cancel", null).ToString();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			this.FormationHint.HintText = GameTexts.FindText("str_party_formation", null);
			this.TroopsLabel = GameTexts.FindText("str_troops_group", null).ToString();
			this.PrisonersLabel = GameTexts.FindText("str_party_category_prisoners_tooltip", null).ToString();
			this.TransferAllMainTroopsHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("TEXT", new TextObject("{=Pvx4TU66}Transfer All Main Troops", null));
				GameTexts.SetVariable("HOTKEY", this.GetTransferAllMainTroopsKeyText());
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
			this.TransferAllMainPrisonersHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("TEXT", new TextObject("{=PpbopMjT}Transfer All Main Prisoners", null));
				GameTexts.SetVariable("HOTKEY", this.GetTransferAllMainPrisonersKeyText());
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
			this.TransferAllOtherTroopsHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("TEXT", new TextObject("{=sVsaWZjg}Transfer All Other Troops", null));
				GameTexts.SetVariable("HOTKEY", this.GetTransferAllOtherTroopsKeyText());
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
			this.TransferAllOtherPrisonersHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("TEXT", new TextObject("{=Srr4rOSq}Transfer All Other Prisoners", null));
				GameTexts.SetVariable("HOTKEY", this.GetTransferAllOtherPrisonersKeyText());
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
			this.WageHint = new HintViewModel(GameTexts.FindText("str_wage", null), null);
			this.UpgradePopUp.RefreshValues();
			this.RecruitPopUp.RefreshValues();
			MBBindingList<PartyCharacterVM> otherPartyTroops = this.OtherPartyTroops;
			if (otherPartyTroops != null)
			{
				otherPartyTroops.ApplyActionOnAllItems(delegate(PartyCharacterVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<PartyCharacterVM> otherPartyPrisoners = this.OtherPartyPrisoners;
			if (otherPartyPrisoners != null)
			{
				otherPartyPrisoners.ApplyActionOnAllItems(delegate(PartyCharacterVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<PartyCharacterVM> mainPartyTroops = this.MainPartyTroops;
			if (mainPartyTroops != null)
			{
				mainPartyTroops.ApplyActionOnAllItems(delegate(PartyCharacterVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<PartyCharacterVM> mainPartyPrisoners = this.MainPartyPrisoners;
			if (mainPartyPrisoners != null)
			{
				mainPartyPrisoners.ApplyActionOnAllItems(delegate(PartyCharacterVM x)
				{
					x.RefreshValues();
				});
			}
			this.UpdateLabelHints();
			this.OnPartyGoldChanged();
			if (this.PartyScreenLogic != null)
			{
				this.InitializeStaticInformation();
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00013D0E File Offset: 0x00011F0E
		public void SetFiveStackShortcutKeyText(string text)
		{
			PartyCharacterVM.FiveStackShortcutKeyText = text;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00013D16 File Offset: 0x00011F16
		public void SetEntireStackShortcutKeyText(string text)
		{
			PartyCharacterVM.EntireStackShortcutKeyText = text;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00013D20 File Offset: 0x00011F20
		private void OnPartyGoldChanged()
		{
			MBTextManager.SetTextVariable("PAY_OR_GET", (this.PartyScreenLogic.CurrentData.PartyGoldChangeAmount > 0) ? 1 : 0);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(this.PartyScreenLogic.CurrentData.PartyGoldChangeAmount));
			this.GoldChangeText = ((this.PartyScreenLogic.CurrentData.PartyGoldChangeAmount == 0) ? "" : GameTexts.FindText("str_inventory_trade_label", null).ToString());
		}

		// Token: 0x0600031B RID: 795 RVA: 0x00013D9C File Offset: 0x00011F9C
		private void OnPartyMoraleChanged()
		{
			MBTextManager.SetTextVariable("PAY_OR_GET", (this.PartyScreenLogic.CurrentData.PartyMoraleChangeAmount > 0) ? 1 : 0);
			MBTextManager.SetTextVariable("MORALE_ICON", "{=!}<img src=\"General\\Icons\\Morale@2x\" extend=\"8\">", false);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(this.PartyScreenLogic.CurrentData.PartyMoraleChangeAmount));
			this.MoraleChangeText = ((this.PartyScreenLogic.CurrentData.PartyMoraleChangeAmount == 0) ? "" : GameTexts.FindText("str_party_morale_label", null).ToString());
		}

		// Token: 0x0600031C RID: 796 RVA: 0x00013E28 File Offset: 0x00012028
		private void OnPartyInfluenceChanged()
		{
			int num = this.PartyScreenLogic.CurrentData.PartyInfluenceChangeAmount.Item1 + this.PartyScreenLogic.CurrentData.PartyInfluenceChangeAmount.Item2 + this.PartyScreenLogic.CurrentData.PartyInfluenceChangeAmount.Item3;
			MBTextManager.SetTextVariable("PAY_OR_GET", (num > 0) ? 1 : 0);
			MBTextManager.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">", false);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(num));
			this.InfluenceChangeText = ((num == 0) ? "" : GameTexts.FindText("str_party_influence_label", null).ToString());
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00013ECC File Offset: 0x000120CC
		private void OnPartyHorseChanged()
		{
			MBTextManager.SetTextVariable("IS_PLURAL", (this.PartyScreenLogic.CurrentData.PartyHorseChangeAmount > 1) ? 1 : 0);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(this.PartyScreenLogic.CurrentData.PartyHorseChangeAmount));
			this.HorseChangeText = ((this.PartyScreenLogic.CurrentData.PartyHorseChangeAmount == 0) ? "" : GameTexts.FindText("str_party_horse_label", null).ToString());
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00013F48 File Offset: 0x00012148
		private void InitializeTroopLists()
		{
			this.ArePrisonersRelevantOnCurrentMode = this._currentMode != PartyScreenMode.TroopsManage && this._currentMode != PartyScreenMode.QuestTroopManage;
			this.AreMembersRelevantOnCurrentMode = this._currentMode != PartyScreenMode.PrisonerManage && this._currentMode != PartyScreenMode.Ransom;
			this._lockedTroopIDs = this._viewDataTracker.GetPartyTroopLocks().ToList<string>();
			this._lockedPrisonerIDs = this._viewDataTracker.GetPartyPrisonerLocks().ToList<string>();
			this.InitializePartyList(this.MainPartyPrisoners, this.PartyScreenLogic.PrisonerRosters[1], PartyScreenLogic.TroopType.Prisoner, 1);
			this.InitializePartyList(this.OtherPartyPrisoners, this.PartyScreenLogic.PrisonerRosters[0], PartyScreenLogic.TroopType.Prisoner, 0);
			this.InitializePartyList(this.MainPartyTroops, this.PartyScreenLogic.MemberRosters[1], PartyScreenLogic.TroopType.Member, 1);
			this.InitializePartyList(this.OtherPartyTroops, this.PartyScreenLogic.MemberRosters[0], PartyScreenLogic.TroopType.Member, 0);
			this.CurrentCharacter = ((this.MainPartyTroops.Count > 0) ? this.MainPartyTroops[0] : this.OtherPartyTroops[0]);
			this.RefreshTopInformation();
			this.OtherPartyComposition.RefreshCounts(this.OtherPartyTroops);
			this.MainPartyComposition.RefreshCounts(this.MainPartyTroops);
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00014080 File Offset: 0x00012280
		private void RefreshTopInformation()
		{
			this.MainPartyTotalWeeklyCostLbl = MobileParty.MainParty.TotalWage.ToString();
			this.MainPartyTotalGoldLbl = Hero.MainHero.Gold.ToString();
			this.MainPartyTotalMoraleLbl = ((int)MobileParty.MainParty.Morale).ToString("##.0");
			this.MainPartyTotalSpeedLbl = CampaignUIHelper.FloatToString(MobileParty.MainParty.Speed);
			this.UpdateLabelHints();
		}

		// Token: 0x06000320 RID: 800 RVA: 0x000140F8 File Offset: 0x000122F8
		private void UpdateLabelHints()
		{
			this.SpeedHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartySpeedTooltip());
			if (this.PartyScreenLogic.RightOwnerParty != null)
			{
				this.MainPartyTroopSizeLimitHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyTroopSizeLimitTooltip(this.PartyScreenLogic.RightOwnerParty));
				this.MainPartyPrisonerSizeLimitHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyPrisonerSizeLimitTooltip(this.PartyScreenLogic.RightOwnerParty));
			}
			if (this.PartyScreenLogic.LeftOwnerParty != null)
			{
				this.OtherPartyTroopSizeLimitHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyTroopSizeLimitTooltip(this.PartyScreenLogic.LeftOwnerParty));
				this.OtherPartyPrisonerSizeLimitHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyPrisonerSizeLimitTooltip(this.PartyScreenLogic.LeftOwnerParty));
			}
			this.UsedHorsesHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetUsedHorsesTooltip(this.PartyScreenLogic.CurrentData.UsedUpgradeHorsesHistory));
			this.DenarHint.HintText = GameTexts.FindText("str_gold", null);
		}

		// Token: 0x06000321 RID: 801 RVA: 0x000141D4 File Offset: 0x000123D4
		private void InitializeStaticInformation()
		{
			if (this.PartyScreenLogic.RightOwnerParty != null)
			{
				this.MainPartyNameLbl = this.PartyScreenLogic.RightOwnerParty.Name.ToString();
			}
			else
			{
				this.MainPartyNameLbl = ((!TextObject.IsNullOrEmpty(this.PartyScreenLogic.RightPartyName)) ? this.PartyScreenLogic.RightPartyName.ToString() : string.Empty);
			}
			MBTextManager.SetTextVariable("PARTY_NAME", MobileParty.MainParty.Name, false);
			if (this.PartyScreenLogic.LeftOwnerParty != null)
			{
				this.OtherPartyNameLbl = this.PartyScreenLogic.LeftOwnerParty.Name.ToString();
			}
			else
			{
				this.OtherPartyNameLbl = ((!TextObject.IsNullOrEmpty(this.PartyScreenLogic.LeftPartyName)) ? this.PartyScreenLogic.LeftPartyName.ToString() : GameTexts.FindText("str_dismiss", null).ToString());
			}
			if (this.PartyScreenLogic.Header == null || string.IsNullOrEmpty(this.PartyScreenLogic.Header.ToString()))
			{
				this.HeaderLbl = GameTexts.FindText("str_party", null).ToString();
				return;
			}
			this.HeaderLbl = this.PartyScreenLogic.Header.ToString();
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00014304 File Offset: 0x00012504
		public void SetSelectedCharacter(PartyCharacterVM troop)
		{
			this.CurrentCharacter = troop;
			this.CurrentCharacter.UpdateRecruitable();
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00014318 File Offset: 0x00012518
		private void ProcessCharacterLock(PartyCharacterVM troop, bool isLocked)
		{
			List<string> list = (troop.IsPrisoner ? this._lockedPrisonerIDs : this._lockedTroopIDs);
			if (isLocked && !list.Contains(troop.StringId))
			{
				list.Add(troop.StringId);
				return;
			}
			if (!isLocked && list.Contains(troop.StringId))
			{
				list.Remove(troop.StringId);
			}
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00014378 File Offset: 0x00012578
		private PartyCompositionVM GetCompositionForList(MBBindingList<PartyCharacterVM> list)
		{
			if (list == this.MainPartyTroops)
			{
				return this.MainPartyComposition;
			}
			if (list == this.OtherPartyTroops)
			{
				return this.OtherPartyComposition;
			}
			return null;
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0001439B File Offset: 0x0001259B
		private void SaveSortState()
		{
			this._viewDataTracker.SetPartySortType((int)this.PartyScreenLogic.ActiveMainPartySortType);
			this._viewDataTracker.SetIsPartySortAscending(this.PartyScreenLogic.IsMainPartySortAscending);
		}

		// Token: 0x06000326 RID: 806 RVA: 0x000143C9 File Offset: 0x000125C9
		private void SaveCharacterLockStates()
		{
			this._viewDataTracker.SetPartyTroopLocks(this._lockedTroopIDs);
			this._viewDataTracker.SetPartyPrisonerLocks(this._lockedPrisonerIDs);
		}

		// Token: 0x06000327 RID: 807 RVA: 0x000143ED File Offset: 0x000125ED
		private bool IsTroopLocked(TroopRosterElement troop, bool isPrisoner)
		{
			if (!isPrisoner)
			{
				return this._lockedTroopIDs.Contains(troop.Character.StringId);
			}
			return this._lockedPrisonerIDs.Contains(troop.Character.StringId);
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0001441F File Offset: 0x0001261F
		private void UpdateCurrentCharacterFormationClass(SelectorVM<SelectorItemVM> s)
		{
			Campaign.Current.SetPlayerFormationPreference(this.CurrentCharacter.Character, (FormationClass)s.SelectedIndex);
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0001443C File Offset: 0x0001263C
		private void InitializePartyList(MBBindingList<PartyCharacterVM> partyList, TroopRoster currentTroopRoster, PartyScreenLogic.TroopType type, int side)
		{
			partyList.Clear();
			for (int i = 0; i < currentTroopRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = currentTroopRoster.GetElementCopyAtIndex(i);
				PartyCharacterVM partyCharacterVM = new PartyCharacterVM(this.PartyScreenLogic, this, currentTroopRoster, i, type, (PartyScreenLogic.PartyRosterSide)side, this.PartyScreenLogic.IsTroopTransferable(type, elementCopyAtIndex.Character, side));
				partyList.Add(partyCharacterVM);
				partyCharacterVM.ThrowOnPropertyChanged();
				partyCharacterVM.IsLocked = partyCharacterVM.Side == PartyScreenLogic.PartyRosterSide.Right && this.IsTroopLocked(partyCharacterVM.Troop, partyCharacterVM.IsPrisoner);
			}
		}

		// Token: 0x0600032A RID: 810 RVA: 0x000144C0 File Offset: 0x000126C0
		public void ExecuteTransferWithParameters(PartyCharacterVM party, int index, string targetTag)
		{
			PartyScreenLogic.PartyRosterSide side = party.Side;
			PartyScreenLogic.PartyRosterSide partyRosterSide = (targetTag.StartsWith("MainParty") ? PartyScreenLogic.PartyRosterSide.Right : PartyScreenLogic.PartyRosterSide.Left);
			if (targetTag == "MainParty")
			{
				index = -1;
			}
			else if (targetTag.EndsWith("Prisoners") != party.IsPrisoner)
			{
				index = -1;
			}
			if (side != partyRosterSide && party.IsTroopTransferrable)
			{
				this.OnTransferTroop(party, index, party.Number, party.Side);
				this.ExecuteRemoveZeroCounts();
				return;
			}
			if (side == partyRosterSide)
			{
				this.OnShiftTroop(party, index);
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00014544 File Offset: 0x00012744
		private void OnTransferTroop(PartyCharacterVM troop, int newIndex, int transferAmount, PartyScreenLogic.PartyRosterSide fromSide)
		{
			if (troop.Side == PartyScreenLogic.PartyRosterSide.None || fromSide == PartyScreenLogic.PartyRosterSide.None)
			{
				return;
			}
			PartyScreenLogic.PartyRosterSide side = troop.Side;
			this.SetSelectedCharacter(troop);
			PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
			if (newIndex == -1)
			{
				newIndex = this.PartyScreenLogic.GetIndexToInsertTroop(PartyScreenLogic.PartyRosterSide.Right - troop.Side, troop.Type, troop.Troop);
			}
			else if (fromSide == PartyScreenLogic.PartyRosterSide.Left)
			{
				this.MainPartySortController.SelectSortType(PartyScreenLogic.TroopSortType.Custom);
			}
			else if (fromSide == PartyScreenLogic.PartyRosterSide.Right)
			{
				this.OtherPartySortController.SelectSortType(PartyScreenLogic.TroopSortType.Custom);
			}
			if (transferAmount > 0)
			{
				int numberOfHealthyTroopNumberForSide = this.GetNumberOfHealthyTroopNumberForSide(troop.Troop.Character, fromSide, troop.IsPrisoner);
				int numberOfWoundedTroopNumberForSide = this.GetNumberOfWoundedTroopNumberForSide(troop.Troop.Character, fromSide, troop.IsPrisoner);
				if ((this.PartyScreenLogic.TransferHealthiesGetWoundedsFirst && fromSide == PartyScreenLogic.PartyRosterSide.Right) || (!this.PartyScreenLogic.TransferHealthiesGetWoundedsFirst && fromSide == PartyScreenLogic.PartyRosterSide.Left))
				{
					int num = ((transferAmount <= numberOfHealthyTroopNumberForSide) ? 0 : (transferAmount - numberOfHealthyTroopNumberForSide));
					num = (int)MathF.Clamp((float)num, 0f, (float)numberOfWoundedTroopNumberForSide);
					partyCommand.FillForTransferTroop(fromSide, troop.Type, troop.Character, transferAmount, num, newIndex);
				}
				else
				{
					partyCommand.FillForTransferTroop(fromSide, troop.Type, troop.Character, transferAmount, (numberOfWoundedTroopNumberForSide >= transferAmount) ? transferAmount : numberOfWoundedTroopNumberForSide, newIndex);
				}
				this.PartyScreenLogic.AddCommand(partyCommand);
			}
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00014682 File Offset: 0x00012882
		private void OnFocusCharacter(PartyCharacterVM character)
		{
			this.CurrentFocusedCharacter = character;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0001468B File Offset: 0x0001288B
		private int GetNumberOfWoundedTroopNumberForSide(CharacterObject character, PartyScreenLogic.PartyRosterSide fromSide, bool isPrisoner)
		{
			return this.FindCharacterVM(character, fromSide, isPrisoner).WoundedCount;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0001469C File Offset: 0x0001289C
		private int GetNumberOfHealthyTroopNumberForSide(CharacterObject character, PartyScreenLogic.PartyRosterSide fromSide, bool isPrisoner)
		{
			PartyCharacterVM partyCharacterVM = this.FindCharacterVM(character, fromSide, isPrisoner);
			return partyCharacterVM.Troop.Number - partyCharacterVM.Troop.WoundedNumber;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x000146D0 File Offset: 0x000128D0
		private void OnSortTroops(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopSortType sortType, bool isAscending)
		{
			PartyScreenLogic.TroopSortType activeSortTypeForSide = this.PartyScreenLogic.GetActiveSortTypeForSide(side);
			bool isAscendingSortForSide = this.PartyScreenLogic.GetIsAscendingSortForSide(side);
			if (activeSortTypeForSide != sortType || isAscendingSortForSide != isAscending)
			{
				PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
				partyCommand.FillForSortTroops(side, sortType, isAscending);
				this.PartyScreenLogic.AddCommand(partyCommand);
			}
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00014718 File Offset: 0x00012918
		private PartyCharacterVM FindCharacterVM(CharacterObject character, PartyScreenLogic.PartyRosterSide side, bool isPrisoner)
		{
			MBBindingList<PartyCharacterVM> mbbindingList = null;
			if (side == PartyScreenLogic.PartyRosterSide.Left)
			{
				mbbindingList = (isPrisoner ? this.OtherPartyPrisoners : this.OtherPartyTroops);
			}
			else if (side == PartyScreenLogic.PartyRosterSide.Right)
			{
				mbbindingList = (isPrisoner ? this.MainPartyPrisoners : this.MainPartyTroops);
			}
			if (mbbindingList == null)
			{
				return null;
			}
			return mbbindingList.First((PartyCharacterVM x) => x.Troop.Character == character);
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00014778 File Offset: 0x00012978
		private void OnShiftTroop(PartyCharacterVM troop, int newIndex)
		{
			if (troop.Side == PartyScreenLogic.PartyRosterSide.None)
			{
				return;
			}
			this.SetSelectedCharacter(troop);
			PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
			partyCommand.FillForShiftTroop(troop.Side, troop.Type, troop.Character, newIndex);
			this.PartyScreenLogic.AddCommand(partyCommand);
		}

		// Token: 0x06000332 RID: 818 RVA: 0x000147C4 File Offset: 0x000129C4
		private void Update(PartyScreenLogic.PartyCommand command)
		{
			switch (command.Code)
			{
			case PartyScreenLogic.PartyCommandCode.TransferTroop:
			case PartyScreenLogic.PartyCommandCode.TransferPartyLeaderTroop:
			case PartyScreenLogic.PartyCommandCode.TransferTroopToLeaderSlot:
				this.TransferTroop(command);
				break;
			case PartyScreenLogic.PartyCommandCode.UpgradeTroop:
				this.UpgradeTroop(command);
				this.RefreshTroopsUpgradeable();
				this.UpgradePopUp.OnTroopUpgraded();
				break;
			case PartyScreenLogic.PartyCommandCode.ShiftTroop:
				this.ShiftTroop(command);
				break;
			case PartyScreenLogic.PartyCommandCode.RecruitTroop:
			{
				PartyCharacterVM currentCharacter = this.CurrentCharacter;
				this.RecruitTroop(command);
				this.RecruitPopUp.OnTroopRecruited(currentCharacter);
				break;
			}
			case PartyScreenLogic.PartyCommandCode.ExecuteTroop:
				this.ExecuteTroop(command);
				break;
			case PartyScreenLogic.PartyCommandCode.TransferAllTroops:
				this.TransferAllTroops(command);
				break;
			case PartyScreenLogic.PartyCommandCode.SortTroops:
				this.SortTroops(command);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this.RefreshTopInformation();
			this.UpdateTroopManagerPopUpCounts();
			this.RefreshPrisonersRecruitable();
			this.IsDoneDisabled = !this.PartyScreenLogic.IsDoneActive();
			this.DoneHint.HintText = new TextObject("{=!}" + this.PartyScreenLogic.DoneReasonString, null);
			this.IsCancelDisabled = !this.PartyScreenLogic.IsCancelActive();
		}

		// Token: 0x06000333 RID: 819 RVA: 0x000148D0 File Offset: 0x00012AD0
		private MBBindingList<PartyCharacterVM> GetPartyCharacterVMList(PartyScreenLogic.PartyRosterSide rosterSide, PartyScreenLogic.TroopType type)
		{
			MBBindingList<PartyCharacterVM> mbbindingList = null;
			if (type == PartyScreenLogic.TroopType.Member)
			{
				if (rosterSide == PartyScreenLogic.PartyRosterSide.Left)
				{
					mbbindingList = this.OtherPartyTroops;
				}
				else if (rosterSide == PartyScreenLogic.PartyRosterSide.Right)
				{
					mbbindingList = this.MainPartyTroops;
				}
			}
			else if (type == PartyScreenLogic.TroopType.Prisoner)
			{
				if (rosterSide == PartyScreenLogic.PartyRosterSide.Left)
				{
					mbbindingList = this.OtherPartyPrisoners;
				}
				else if (rosterSide == PartyScreenLogic.PartyRosterSide.Right)
				{
					mbbindingList = this.MainPartyPrisoners;
				}
			}
			return mbbindingList;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00014918 File Offset: 0x00012B18
		private void AfterReset(PartyScreenLogic partyScreenLogic, bool fromCancel)
		{
			if (!fromCancel)
			{
				this.InitializeTroopLists();
				this.RefreshPartyInformation();
				this.OnPartyGoldChanged();
				this.OnPartyMoraleChanged();
				this.OnPartyHorseChanged();
				this.OnPartyInfluenceChanged();
				this.UpdateTroopManagerPopUpCounts();
				this.MainPartyComposition.RefreshCounts(this.MainPartyTroops);
				this.OtherPartyComposition.RefreshCounts(this.OtherPartyTroops);
				this.IsDoneDisabled = !partyScreenLogic.IsDoneActive();
				this.DoneHint.HintText = new TextObject("{=!}" + this.PartyScreenLogic.DoneReasonString, null);
				this.IsCancelDisabled = !partyScreenLogic.IsCancelActive();
			}
		}

		// Token: 0x06000335 RID: 821 RVA: 0x000149BC File Offset: 0x00012BBC
		private void TransferTroop(PartyScreenLogic.PartyCommand command)
		{
			PartyScreenLogic.PartyRosterSide partyRosterSide = PartyScreenLogic.PartyRosterSide.None;
			PartyScreenLogic.PartyRosterSide rosterSide = command.RosterSide;
			if (rosterSide != PartyScreenLogic.PartyRosterSide.Left)
			{
				if (rosterSide == PartyScreenLogic.PartyRosterSide.Right)
				{
					partyRosterSide = PartyScreenLogic.PartyRosterSide.Left;
				}
			}
			else
			{
				partyRosterSide = PartyScreenLogic.PartyRosterSide.Right;
			}
			MBBindingList<PartyCharacterVM> partyCharacterVMList = this.GetPartyCharacterVMList(command.RosterSide, command.Type);
			MBBindingList<PartyCharacterVM> partyCharacterVMList2 = this.GetPartyCharacterVMList(partyRosterSide, command.Type);
			if (command.Type == PartyScreenLogic.TroopType.Member)
			{
				this._currentCharacter.Troop = this.PartyScreenLogic.MemberRosters[(int)this.CurrentCharacter.Side].GetElementCopyAtIndex(this.PartyScreenLogic.MemberRosters[(int)this.CurrentCharacter.Side].FindIndexOfTroop(this.CurrentCharacter.Character));
			}
			else if (command.Type == PartyScreenLogic.TroopType.Prisoner)
			{
				this._currentCharacter.Troop = this.PartyScreenLogic.PrisonerRosters[(int)this.CurrentCharacter.Side].GetElementCopyAtIndex(this.PartyScreenLogic.PrisonerRosters[(int)this.CurrentCharacter.Side].FindIndexOfTroop(this.CurrentCharacter.Character));
			}
			this._currentCharacter.UpdateTradeData();
			this._currentCharacter.ThrowOnPropertyChanged();
			TroopRoster troopRoster = null;
			TroopRoster troopRoster2 = null;
			int num = 0;
			int num2 = 0;
			PartyScreenLogic.TroopType type = command.Type;
			if (type != PartyScreenLogic.TroopType.Member)
			{
				if (type == PartyScreenLogic.TroopType.Prisoner)
				{
					troopRoster = this.PartyScreenLogic.PrisonerRosters[(int)partyRosterSide];
					num = this.PartyScreenLogic.PrisonerRosters[(int)partyRosterSide].FindIndexOfTroop(this.CurrentCharacter.Character);
					troopRoster2 = this.PartyScreenLogic.PrisonerRosters[(int)command.RosterSide];
					num2 = this.PartyScreenLogic.PrisonerRosters[(int)command.RosterSide].FindIndexOfTroop(this.CurrentCharacter.Character);
				}
			}
			else
			{
				troopRoster = this.PartyScreenLogic.MemberRosters[(int)partyRosterSide];
				num = this.PartyScreenLogic.MemberRosters[(int)partyRosterSide].FindIndexOfTroop(this.CurrentCharacter.Character);
				troopRoster2 = this.PartyScreenLogic.MemberRosters[(int)command.RosterSide];
				num2 = this.PartyScreenLogic.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(this.CurrentCharacter.Character);
			}
			PartyCharacterVM partyCharacterVM = partyCharacterVMList.FirstOrDefault((PartyCharacterVM q) => q.Character == this.CurrentCharacter.Character);
			if (troopRoster2.FindIndexOfTroop(this.CurrentCharacter.Character) != -1 && partyCharacterVM != null)
			{
				partyCharacterVM.Troop = troopRoster2.GetElementCopyAtIndex(num2);
				partyCharacterVM.ThrowOnPropertyChanged();
				partyCharacterVM.UpdateTradeData();
			}
			if (partyCharacterVMList2.Contains(this.CurrentCharacter))
			{
				PartyCharacterVM partyCharacterVM2 = partyCharacterVMList2.First((PartyCharacterVM q) => q.Character == this.CurrentCharacter.Character);
				partyCharacterVM2.Troop = troopRoster.GetElementCopyAtIndex(num);
				partyCharacterVM2.ThrowOnPropertyChanged();
				if (!partyCharacterVMList.Contains(this.CurrentCharacter))
				{
					this.SetSelectedCharacter(partyCharacterVM2);
				}
				partyCharacterVM2.UpdateTradeData();
			}
			else
			{
				PartyCharacterVM partyCharacterVM3 = new PartyCharacterVM(this.PartyScreenLogic, this, troopRoster, num, command.Type, partyRosterSide, this.PartyScreenLogic.IsTroopTransferable(command.Type, troopRoster.GetCharacterAtIndex(num), (int)partyRosterSide));
				if (command.Index != -1)
				{
					partyCharacterVMList2.Insert(command.Index, partyCharacterVM3);
				}
				else
				{
					partyCharacterVMList2.Add(partyCharacterVM3);
				}
				if (!partyCharacterVMList.Contains(this.CurrentCharacter))
				{
					this.SetSelectedCharacter(partyCharacterVM3);
				}
				partyCharacterVM3.IsLocked = partyCharacterVM3.Side == PartyScreenLogic.PartyRosterSide.Right && this.IsTroopLocked(partyCharacterVM3.Troop, partyCharacterVM3.IsPrisoner);
			}
			PartyCompositionVM compositionForList = this.GetCompositionForList(partyCharacterVMList);
			if (compositionForList != null)
			{
				compositionForList.OnTroopRemoved(command.Character.DefaultFormationClass, command.TotalNumber);
			}
			PartyCompositionVM compositionForList2 = this.GetCompositionForList(partyCharacterVMList2);
			if (compositionForList2 != null)
			{
				compositionForList2.OnTroopAdded(command.Character.DefaultFormationClass, command.TotalNumber);
			}
			this.CurrentCharacter.UpdateTradeData();
			this.CurrentCharacter.OnTransferred();
			this.CurrentCharacter.ThrowOnPropertyChanged();
			this.RefreshTopInformation();
			this.RefreshPartyInformation();
			Game.Current.EventManager.TriggerEvent<PlayerMoveTroopEvent>(new PlayerMoveTroopEvent(command.Character, command.RosterSide, (command.RosterSide + 1) % (PartyScreenLogic.PartyRosterSide)2, command.TotalNumber, command.Type == PartyScreenLogic.TroopType.Prisoner));
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00014DA0 File Offset: 0x00012FA0
		private void ShiftTroop(PartyScreenLogic.PartyCommand command)
		{
			MBBindingList<PartyCharacterVM> partyCharacterVMList = this.GetPartyCharacterVMList(command.RosterSide, command.Type);
			if (command.Index < 0)
			{
				return;
			}
			PartyCharacterVM currentCharacter = this.CurrentCharacter;
			int num = partyCharacterVMList.IndexOf(this.CurrentCharacter);
			int num2 = -1;
			partyCharacterVMList.Remove(this.CurrentCharacter);
			if (partyCharacterVMList.Count < command.Index)
			{
				partyCharacterVMList.Add(currentCharacter);
			}
			else
			{
				num2 = ((num < command.Index) ? (command.Index - 1) : command.Index);
				partyCharacterVMList.Insert(num2, currentCharacter);
			}
			this.SetSelectedCharacter(currentCharacter);
			if (num != num2)
			{
				bool isAscendingSortForSide = this.PartyScreenLogic.GetIsAscendingSortForSide(command.RosterSide);
				this.OnSortTroops(command.RosterSide, PartyScreenLogic.TroopSortType.Custom, isAscendingSortForSide);
			}
			this.CurrentCharacter.ThrowOnPropertyChanged();
			this.RefreshTopInformation();
			this.RefreshPartyInformation();
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00014E6A File Offset: 0x0001306A
		public void OnUpgradePopUpClosed(bool isCancelled)
		{
			if (!isCancelled)
			{
				this.UpdateTroopManagerPopUpCounts();
			}
			Game.Current.EventManager.TriggerEvent<PlayerToggledUpgradePopupEvent>(new PlayerToggledUpgradePopupEvent(false));
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00014E8A File Offset: 0x0001308A
		public void OnRecruitPopUpClosed(bool isCancelled)
		{
			if (!isCancelled)
			{
				this.UpdateTroopManagerPopUpCounts();
			}
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00014E98 File Offset: 0x00013098
		private void UpdateTroopManagerPopUpCounts()
		{
			if (this.UpgradePopUp.IsOpen || this.RecruitPopUp.IsOpen)
			{
				return;
			}
			this.RecruitableTroopCount = 0;
			this.UpgradableTroopCount = 0;
			this.MainPartyPrisoners.ApplyActionOnAllItems(delegate(PartyCharacterVM x)
			{
				this.RecruitableTroopCount += x.NumOfRecruitablePrisoners;
			});
			this.MainPartyTroops.ApplyActionOnAllItems(delegate(PartyCharacterVM x)
			{
				this.UpgradableTroopCount += x.NumOfUpgradeableTroops;
			});
			this.IsRecruitPopUpDisabled = !this.ArePrisonersRelevantOnCurrentMode || this.RecruitableTroopCount == 0 || this.PartyScreenLogic.IsTroopUpgradesDisabled;
			this.IsUpgradePopUpDisabled = !this.AreMembersRelevantOnCurrentMode || this.UpgradableTroopCount == 0 || this.PartyScreenLogic.IsTroopUpgradesDisabled;
			this.RecruitPopUp.UpdateOpenButtonHint(this.IsRecruitPopUpDisabled, !this.ArePrisonersRelevantOnCurrentMode, this.PartyScreenLogic.IsTroopUpgradesDisabled);
			this.UpgradePopUp.UpdateOpenButtonHint(this.IsUpgradePopUpDisabled, !this.AreMembersRelevantOnCurrentMode, this.PartyScreenLogic.IsTroopUpgradesDisabled);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00014F90 File Offset: 0x00013190
		private void UpgradeTroop(PartyScreenLogic.PartyCommand command)
		{
			int num = this.PartyScreenLogic.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(command.Character.UpgradeTargets[command.UpgradeTarget]);
			PartyCharacterVM newCharacter = new PartyCharacterVM(this.PartyScreenLogic, this, this.PartyScreenLogic.MemberRosters[(int)command.RosterSide], num, command.Type, command.RosterSide, this.PartyScreenLogic.IsTroopTransferable(command.Type, this.PartyScreenLogic.MemberRosters[(int)command.RosterSide].GetCharacterAtIndex(num), (int)command.RosterSide));
			newCharacter.IsLocked = this.IsTroopLocked(newCharacter.Troop, false);
			MBBindingList<PartyCharacterVM> partyCharacterVMList = this.GetPartyCharacterVMList(command.RosterSide, command.Type);
			if (partyCharacterVMList.Contains(newCharacter))
			{
				PartyCharacterVM partyCharacterVM = partyCharacterVMList.First((PartyCharacterVM character) => character.Equals(newCharacter));
				partyCharacterVM.Troop = newCharacter.Troop;
				partyCharacterVM.ThrowOnPropertyChanged();
			}
			else
			{
				if (command.Index != -1)
				{
					partyCharacterVMList.Insert(command.Index, newCharacter);
				}
				else
				{
					partyCharacterVMList.Add(newCharacter);
				}
				newCharacter.ThrowOnPropertyChanged();
			}
			int num2 = -1;
			if (command.Type == PartyScreenLogic.TroopType.Member)
			{
				num2 = this.PartyScreenLogic.MemberRosters[(int)this.CurrentCharacter.Side].FindIndexOfTroop(this.CurrentCharacter.Character);
				if (num2 > 0)
				{
					this._currentCharacter.Troop = this.PartyScreenLogic.MemberRosters[(int)this.CurrentCharacter.Side].GetElementCopyAtIndex(num2);
				}
			}
			else if (command.Type == PartyScreenLogic.TroopType.Prisoner)
			{
				num2 = this.PartyScreenLogic.MemberRosters[(int)this.CurrentCharacter.Side].FindIndexOfTroop(this.CurrentCharacter.Character);
				if (num2 > 0)
				{
					this._currentCharacter.Troop = this.PartyScreenLogic.PrisonerRosters[(int)this.CurrentCharacter.Side].GetElementCopyAtIndex(num2);
				}
			}
			if (num2 < 0)
			{
				this.UpgradePopUp.OnRanOutTroop(this.CurrentCharacter);
				partyCharacterVMList.Remove(this.CurrentCharacter);
				this.CurrentCharacter = newCharacter;
				MBInformationManager.HideInformations();
			}
			else
			{
				this.CurrentCharacter.InitializeUpgrades();
				this.CurrentCharacter.ThrowOnPropertyChanged();
			}
			PartyCompositionVM compositionForList = this.GetCompositionForList(partyCharacterVMList);
			if (compositionForList != null)
			{
				compositionForList.OnTroopRemoved(command.Character.DefaultFormationClass, command.TotalNumber);
			}
			PartyCompositionVM compositionForList2 = this.GetCompositionForList(partyCharacterVMList);
			if (compositionForList2 != null)
			{
				compositionForList2.OnTroopAdded(newCharacter.Character.DefaultFormationClass, command.TotalNumber);
			}
			PartyCharacterVM currentCharacter = this.CurrentCharacter;
			if (currentCharacter != null)
			{
				currentCharacter.UpdateTradeData();
			}
			Game.Current.EventManager.TriggerEvent<PlayerRequestUpgradeTroopEvent>(new PlayerRequestUpgradeTroopEvent(command.Character, command.Character.UpgradeTargets[command.UpgradeTarget], command.TotalNumber));
			this.RefreshTopInformation();
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0001526C File Offset: 0x0001346C
		private void RecruitTroop(PartyScreenLogic.PartyCommand command)
		{
			int num = this.PartyScreenLogic.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(command.Character);
			PartyCharacterVM newCharacter = new PartyCharacterVM(this.PartyScreenLogic, this, this.PartyScreenLogic.MemberRosters[(int)command.RosterSide], num, PartyScreenLogic.TroopType.Member, command.RosterSide, this.PartyScreenLogic.IsTroopTransferable(command.Type, this.PartyScreenLogic.MemberRosters[(int)command.RosterSide].GetCharacterAtIndex(num), (int)command.RosterSide));
			newCharacter.IsLocked = this.IsTroopLocked(newCharacter.Troop, false);
			MBBindingList<PartyCharacterVM> partyCharacterVMList = this.GetPartyCharacterVMList(command.RosterSide, PartyScreenLogic.TroopType.Member);
			MBBindingList<PartyCharacterVM> partyCharacterVMList2 = this.GetPartyCharacterVMList(command.RosterSide, PartyScreenLogic.TroopType.Prisoner);
			if (partyCharacterVMList.Contains(newCharacter))
			{
				PartyCharacterVM partyCharacterVM = partyCharacterVMList.First((PartyCharacterVM character) => character.Equals(newCharacter));
				partyCharacterVM.Troop = newCharacter.Troop;
				partyCharacterVM.ThrowOnPropertyChanged();
			}
			else
			{
				if (command.Index != -1)
				{
					partyCharacterVMList.Insert(command.Index, newCharacter);
				}
				else
				{
					partyCharacterVMList.Add(newCharacter);
				}
				newCharacter.ThrowOnPropertyChanged();
			}
			int num2 = -1;
			if (command.Type == PartyScreenLogic.TroopType.Prisoner)
			{
				num2 = this.PartyScreenLogic.PrisonerRosters[(int)this.CurrentCharacter.Side].FindIndexOfTroop(this.CurrentCharacter.Character);
				if (num2 >= 0)
				{
					this._currentCharacter.Troop = this.PartyScreenLogic.PrisonerRosters[(int)this.CurrentCharacter.Side].GetElementCopyAtIndex(num2);
				}
			}
			else
			{
				Debug.FailedAssert("Players can only recruit prisoners", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Party\\PartyVM.cs", "RecruitTroop", 1010);
			}
			if (num2 < 0)
			{
				partyCharacterVMList2.Remove(this.CurrentCharacter);
				this.CurrentCharacter = newCharacter;
				MBInformationManager.HideInformations();
			}
			else
			{
				this.CurrentCharacter.InitializeUpgrades();
				this.CurrentCharacter.ThrowOnPropertyChanged();
			}
			PartyCompositionVM compositionForList = this.GetCompositionForList(partyCharacterVMList);
			if (compositionForList != null)
			{
				compositionForList.OnTroopAdded(command.Character.DefaultFormationClass, command.TotalNumber);
			}
			PartyCharacterVM currentCharacter = this.CurrentCharacter;
			if (currentCharacter != null)
			{
				currentCharacter.UpdateTradeData();
			}
			this.RefreshTopInformation();
			this.RefreshPartyInformation();
		}

		// Token: 0x0600033C RID: 828 RVA: 0x00015498 File Offset: 0x00013698
		private void ExecuteTroop(PartyScreenLogic.PartyCommand command)
		{
			this.PartyScreenLogic.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(command.Character);
			MBBindingList<PartyCharacterVM> partyCharacterVMList = this.GetPartyCharacterVMList(command.RosterSide, PartyScreenLogic.TroopType.Member);
			MBBindingList<PartyCharacterVM> partyCharacterVMList2 = this.GetPartyCharacterVMList(command.RosterSide, PartyScreenLogic.TroopType.Prisoner);
			int num = -1;
			if (command.Type == PartyScreenLogic.TroopType.Prisoner)
			{
				num = this.PartyScreenLogic.PrisonerRosters[(int)this.CurrentCharacter.Side].FindIndexOfTroop(this.CurrentCharacter.Character);
				if (num >= 0)
				{
					this._currentCharacter.Troop = this.PartyScreenLogic.PrisonerRosters[(int)this.CurrentCharacter.Side].GetElementCopyAtIndex(num);
				}
			}
			else
			{
				Debug.FailedAssert("Players can only execute prisoners", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Party\\PartyVM.cs", "ExecuteTroop", 1050);
			}
			if (num < 0)
			{
				partyCharacterVMList2.Remove(this.CurrentCharacter);
				this.CurrentCharacter = partyCharacterVMList2.FirstOrDefault<PartyCharacterVM>() ?? partyCharacterVMList.FirstOrDefault<PartyCharacterVM>();
				MBInformationManager.HideInformations();
			}
			else
			{
				Debug.FailedAssert("The prisoner should have been removed from the prisoner roster after execution", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Party\\PartyVM.cs", "ExecuteTroop", 1061);
			}
			this.RefreshTopInformation();
			this.RefreshPartyInformation();
		}

		// Token: 0x0600033D RID: 829 RVA: 0x000155AC File Offset: 0x000137AC
		private void TransferAllTroops(PartyScreenLogic.PartyCommand command)
		{
			TroopRoster troopRoster = null;
			TroopRoster troopRoster2 = null;
			MBBindingList<PartyCharacterVM> mbbindingList = null;
			MBBindingList<PartyCharacterVM> mbbindingList2 = null;
			if (command.Type == PartyScreenLogic.TroopType.Member)
			{
				troopRoster = this.PartyScreenLogic.GetRoster(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Member);
				troopRoster2 = this.PartyScreenLogic.GetRoster(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Member);
				mbbindingList = this.OtherPartyTroops;
				mbbindingList2 = this.MainPartyTroops;
			}
			if (command.Type == PartyScreenLogic.TroopType.Prisoner)
			{
				troopRoster = this.PartyScreenLogic.GetRoster(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Prisoner);
				troopRoster2 = this.PartyScreenLogic.GetRoster(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Prisoner);
				mbbindingList = this.OtherPartyPrisoners;
				mbbindingList2 = this.MainPartyPrisoners;
			}
			mbbindingList.Clear();
			mbbindingList2.Clear();
			for (int i = 0; i < troopRoster.Count; i++)
			{
				CharacterObject characterAtIndex = troopRoster.GetCharacterAtIndex(i);
				mbbindingList.Add(new PartyCharacterVM(this.PartyScreenLogic, this, troopRoster, i, command.Type, PartyScreenLogic.PartyRosterSide.Left, this.PartyScreenLogic.IsTroopTransferable(command.Type, characterAtIndex, i)));
			}
			for (int j = 0; j < troopRoster2.Count; j++)
			{
				CharacterObject characterAtIndex2 = troopRoster2.GetCharacterAtIndex(j);
				mbbindingList2.Add(new PartyCharacterVM(this.PartyScreenLogic, this, troopRoster2, j, command.Type, PartyScreenLogic.PartyRosterSide.Right, this.PartyScreenLogic.IsTroopTransferable(command.Type, characterAtIndex2, j)));
			}
			this.OtherPartyComposition.RefreshCounts(this.OtherPartyTroops);
			this.MainPartyComposition.RefreshCounts(this.MainPartyTroops);
			this.RefreshTopInformation();
			this.RefreshPartyInformation();
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00015704 File Offset: 0x00013904
		private void SortTroops(PartyScreenLogic.PartyCommand command)
		{
			if (command.SortType != PartyScreenLogic.TroopSortType.Custom)
			{
				PartyScreenLogic.TroopSortType activeSortTypeForSide = this.PartyScreenLogic.GetActiveSortTypeForSide(command.RosterSide);
				PartyVM.TroopVMComparer troopVMComparer = new PartyVM.TroopVMComparer(this.PartyScreenLogic.GetComparer(activeSortTypeForSide));
				if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Left)
				{
					this.OtherPartyTroops.Sort(troopVMComparer);
					this.OtherPartyPrisoners.Sort(troopVMComparer);
				}
				else if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Right)
				{
					this.MainPartyTroops.Sort(troopVMComparer);
					this.MainPartyPrisoners.Sort(troopVMComparer);
				}
			}
			if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Left)
			{
				this.OtherPartySortController.IsAscending = command.IsSortAscending;
				this.OtherPartySortController.SelectSortType(command.SortType);
				return;
			}
			if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Right)
			{
				this.MainPartySortController.IsAscending = command.IsSortAscending;
				this.MainPartySortController.SelectSortType(command.SortType);
			}
		}

		// Token: 0x0600033F RID: 831 RVA: 0x000157D6 File Offset: 0x000139D6
		public void ExecuteTransferAllMainTroops()
		{
			this.TransferAllCharacters(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Member);
			this.ExecuteRemoveZeroCounts();
		}

		// Token: 0x06000340 RID: 832 RVA: 0x000157E6 File Offset: 0x000139E6
		public void ExecuteTransferAllOtherTroops()
		{
			this.TransferAllCharacters(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Member);
			this.ExecuteRemoveZeroCounts();
		}

		// Token: 0x06000341 RID: 833 RVA: 0x000157F6 File Offset: 0x000139F6
		public void ExecuteTransferAllMainPrisoners()
		{
			this.TransferAllCharacters(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Prisoner);
			this.ExecuteRemoveZeroCounts();
		}

		// Token: 0x06000342 RID: 834 RVA: 0x00015806 File Offset: 0x00013A06
		public void ExecuteTransferAllOtherPrisoners()
		{
			this.TransferAllCharacters(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Prisoner);
			this.ExecuteRemoveZeroCounts();
		}

		// Token: 0x06000343 RID: 835 RVA: 0x00015816 File Offset: 0x00013A16
		public void ExecuteOpenUpgradePopUp()
		{
			this.UpgradePopUp.OpenPopUp();
			Game.Current.EventManager.TriggerEvent<PlayerToggledUpgradePopupEvent>(new PlayerToggledUpgradePopupEvent(true));
		}

		// Token: 0x06000344 RID: 836 RVA: 0x00015838 File Offset: 0x00013A38
		public void ExecuteOpenRecruitPopUp()
		{
			this.RecruitPopUp.OpenPopUp();
		}

		// Token: 0x06000345 RID: 837 RVA: 0x00015848 File Offset: 0x00013A48
		public void ExecuteUpgrade(PartyCharacterVM troop, int upgradeTargetType, int maxUpgradeCount)
		{
			this.CurrentCharacter = troop;
			if (this.CurrentCharacter.Side == PartyScreenLogic.PartyRosterSide.Right && this.CurrentCharacter.Type == PartyScreenLogic.TroopType.Member)
			{
				int num = 1;
				if (this.IsEntireStackModifierActive)
				{
					num = maxUpgradeCount;
				}
				else if (this.IsFiveStackModifierActive)
				{
					num = MathF.Min(maxUpgradeCount, 5);
				}
				PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
				int indexToInsertTroop = this.PartyScreenLogic.GetIndexToInsertTroop(this.CurrentCharacter.Side, this.CurrentCharacter.Type, this.CurrentCharacter.Troop);
				partyCommand.FillForUpgradeTroop(this.CurrentCharacter.Side, this.CurrentCharacter.Type, this.CurrentCharacter.Character, num, upgradeTargetType, indexToInsertTroop);
				this.PartyScreenLogic.AddCommand(partyCommand);
			}
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00015908 File Offset: 0x00013B08
		public void ExecuteRecruit(PartyCharacterVM character, bool recruitAll = false)
		{
			this.CurrentCharacter = character;
			if (this.PartyScreenLogic.IsPrisonerRecruitable(this.CurrentCharacter.Type, this.CurrentCharacter.Character, this.CurrentCharacter.Side))
			{
				int num = 1;
				if (this.IsEntireStackModifierActive || recruitAll)
				{
					num = this.CurrentCharacter.NumOfRecruitablePrisoners;
				}
				else if (this.IsFiveStackModifierActive)
				{
					num = MathF.Min(this.CurrentCharacter.NumOfRecruitablePrisoners, 5);
				}
				int indexToInsertTroop = this.PartyScreenLogic.GetIndexToInsertTroop(character.Side, character.Type, character.Troop);
				PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
				partyCommand.FillForRecruitTroop(this.CurrentCharacter.Side, this.CurrentCharacter.Type, this.CurrentCharacter.Character, num, indexToInsertTroop);
				this.PartyScreenLogic.AddCommand(partyCommand);
				this.CurrentCharacter.UpdateRecruitable();
			}
		}

		// Token: 0x06000347 RID: 839 RVA: 0x000159E8 File Offset: 0x00013BE8
		public void ExecuteExecution()
		{
			if (this.PartyScreenLogic.IsExecutable(this.CurrentCharacter.Type, this.CurrentCharacter.Character, this.CurrentCharacter.Side))
			{
				PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
				partyCommand.FillForExecuteTroop(this.CurrentCharacter.Side, this.CurrentCharacter.Type, this.CurrentCharacter.Character);
				this.PartyScreenLogic.AddCommand(partyCommand);
			}
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00015A5C File Offset: 0x00013C5C
		public void ExecuteRemoveZeroCounts()
		{
			this.PartyScreenLogic.RemoveZeroCounts();
			List<PartyCharacterVM> list = this.OtherPartyTroops.ToList<PartyCharacterVM>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].Number == 0 && this.OtherPartyTroops.Count > i)
				{
					this.OtherPartyTroops.RemoveAt(i);
				}
			}
			List<PartyCharacterVM> list2 = this.OtherPartyPrisoners.ToList<PartyCharacterVM>();
			for (int j = list2.Count - 1; j >= 0; j--)
			{
				if (list2[j].Number == 0 && this.OtherPartyPrisoners.Count > j)
				{
					this.OtherPartyPrisoners.RemoveAt(j);
				}
			}
			List<PartyCharacterVM> list3 = this.MainPartyTroops.ToList<PartyCharacterVM>();
			for (int k = list3.Count - 1; k >= 0; k--)
			{
				if (list3[k].Number == 0 && this.MainPartyTroops.Count > k)
				{
					this.MainPartyTroops.RemoveAt(k);
				}
			}
			List<PartyCharacterVM> list4 = this.MainPartyPrisoners.ToList<PartyCharacterVM>();
			for (int l = list4.Count - 1; l >= 0; l--)
			{
				if (list4[l].Number == 0 && this.MainPartyPrisoners.Count > l)
				{
					this.MainPartyPrisoners.RemoveAt(l);
				}
			}
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00015BAC File Offset: 0x00013DAC
		private void TransferAllCharacters(PartyScreenLogic.PartyRosterSide rosterSide, PartyScreenLogic.TroopType type)
		{
			PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
			partyCommand.FillForTransferAllTroops(rosterSide, type);
			this.PartyScreenLogic.AddCommand(partyCommand);
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00015BD4 File Offset: 0x00013DD4
		private void RefreshCurrentCharacterInformation()
		{
			bool flag = this.CurrentCharacter.Character == CharacterObject.PlayerCharacter;
			this.CurrentCharacterWageLbl = "";
			if (this.CurrentCharacter.Type == PartyScreenLogic.TroopType.Member && !flag)
			{
				this.CurrentCharacterWageLbl = this.CurrentCharacter.Character.TroopWage.ToString();
			}
			this.CurrentCharacterLevelLbl = "-";
			if (this.CurrentCharacter.Type == PartyScreenLogic.TroopType.Member || this.CurrentCharacter.Type == PartyScreenLogic.TroopType.Prisoner)
			{
				this.CurrentCharacterLevelLbl = this.CurrentCharacter.Character.Level.ToString();
			}
			this.CurrentCharacter.InitializeUpgrades();
			if (this.CurrentCharacter.Character != null)
			{
				if (this.CurrentCharacter.Character.IsHero)
				{
					this.SelectedCharacter.FillFrom(this.CurrentCharacter.Character.HeroObject, -1, false, false);
				}
				else
				{
					string text = "";
					if (!this.CurrentCharacter.IsPrisoner)
					{
						if (this.CurrentCharacter.Side == PartyScreenLogic.PartyRosterSide.Left)
						{
							text = ((this.PartyScreenLogic.LeftOwnerParty != null && this.PartyScreenLogic.LeftOwnerParty.Banner != null) ? BannerCode.CreateFrom(this.PartyScreenLogic.LeftOwnerParty.Banner).Code : "");
						}
						else
						{
							text = ((this.PartyScreenLogic.RightOwnerParty != null && this.PartyScreenLogic.RightOwnerParty.Banner != null) ? BannerCode.CreateFrom(this.PartyScreenLogic.RightOwnerParty.Banner).Code : "");
						}
					}
					this.SelectedCharacter.FillFrom(this.CurrentCharacter.Character, this.CurrentCharacter.Character.StringId.GetDeterministicHashCode());
					this.SelectedCharacter.BannerCodeText = text;
				}
			}
			this.SelectedCharacter.SetEquipment(this.CurrentCharacter.Character.Equipment);
			if (!this.CurrentCharacter.IsPrisoner)
			{
				if (this.CurrentCharacter.Side == PartyScreenLogic.PartyRosterSide.Right && this.PartyScreenLogic.RightOwnerParty != null && this.PartyScreenLogic.RightOwnerParty.MapFaction != null)
				{
					CharacterViewModel selectedCharacter = this.SelectedCharacter;
					PartyBase rightOwnerParty = this.PartyScreenLogic.RightOwnerParty;
					uint? num;
					if (rightOwnerParty == null)
					{
						num = null;
					}
					else
					{
						IFaction mapFaction = rightOwnerParty.MapFaction;
						num = ((mapFaction != null) ? new uint?(mapFaction.Color) : null);
					}
					selectedCharacter.ArmorColor1 = num ?? 0U;
					CharacterViewModel selectedCharacter2 = this.SelectedCharacter;
					PartyBase rightOwnerParty2 = this.PartyScreenLogic.RightOwnerParty;
					uint? num2;
					if (rightOwnerParty2 == null)
					{
						num2 = null;
					}
					else
					{
						IFaction mapFaction2 = rightOwnerParty2.MapFaction;
						num2 = ((mapFaction2 != null) ? new uint?(mapFaction2.Color2) : null);
					}
					selectedCharacter2.ArmorColor2 = num2 ?? 0U;
				}
				else if (this.CurrentCharacter.Side == PartyScreenLogic.PartyRosterSide.Left && this.PartyScreenLogic.LeftOwnerParty != null && this.PartyScreenLogic.LeftOwnerParty.MapFaction != null)
				{
					CharacterViewModel selectedCharacter3 = this.SelectedCharacter;
					PartyBase leftOwnerParty = this.PartyScreenLogic.LeftOwnerParty;
					uint? num3;
					if (leftOwnerParty == null)
					{
						num3 = null;
					}
					else
					{
						IFaction mapFaction3 = leftOwnerParty.MapFaction;
						num3 = ((mapFaction3 != null) ? new uint?(mapFaction3.Color) : null);
					}
					selectedCharacter3.ArmorColor1 = num3 ?? 0U;
					CharacterViewModel selectedCharacter4 = this.SelectedCharacter;
					PartyBase leftOwnerParty2 = this.PartyScreenLogic.LeftOwnerParty;
					uint? num4;
					if (leftOwnerParty2 == null)
					{
						num4 = null;
					}
					else
					{
						IFaction mapFaction4 = leftOwnerParty2.MapFaction;
						num4 = ((mapFaction4 != null) ? new uint?(mapFaction4.Color2) : null);
					}
					selectedCharacter4.ArmorColor2 = num4 ?? 0U;
				}
			}
			this.IsCurrentCharacterFormationEnabled = !this.CurrentCharacter.IsMainHero && !this.CurrentCharacter.IsPrisoner && this.CurrentCharacter.Side > PartyScreenLogic.PartyRosterSide.Left;
			this.IsCurrentCharacterWageEnabled = !this.CurrentCharacter.IsMainHero && !this.CurrentCharacter.IsPrisoner;
			this.CurrentCharacterTier = CampaignUIHelper.GetCharacterTierData(this.CurrentCharacter.Character, true);
		}

		// Token: 0x0600034B RID: 843 RVA: 0x0001600C File Offset: 0x0001420C
		private void RefreshPartyInformation()
		{
			this.OtherPartyTroopsLbl = PartyVM.PopulatePartyListLabel(this.OtherPartyTroops, this.PartyScreenLogic.LeftPartyMembersSizeLimit);
			this.OtherPartyPrisonersLbl = PartyVM.PopulatePartyListLabel(this.OtherPartyPrisoners, this.PartyScreenLogic.LeftPartyPrisonersSizeLimit);
			this.MainPartyTroopsLbl = PartyVM.PopulatePartyListLabel(this.MainPartyTroops, this.PartyScreenLogic.RightPartyMembersSizeLimit);
			this.MainPartyPrisonersLbl = PartyVM.PopulatePartyListLabel(this.MainPartyPrisoners, this.PartyScreenLogic.RightPartyPrisonersSizeLimit);
			if (this.ShowQuestProgress)
			{
				this.QuestProgressCurrentCount = this.PartyScreenLogic.GetCurrentQuestCurrentCount();
			}
			this.IsMainTroopsLimitWarningEnabled = this.PartyScreenLogic.RightPartyMembersSizeLimit < this.PartyScreenLogic.MemberRosters[1].TotalManCount && this.AreMembersRelevantOnCurrentMode;
			this.IsOtherTroopsLimitWarningEnabled = (this._currentMode == PartyScreenMode.TroopsManage || this._currentMode == PartyScreenMode.QuestTroopManage) && this.PartyScreenLogic.LeftPartyMembersSizeLimit < this.PartyScreenLogic.MemberRosters[0].TotalManCount && this.ArePrisonersRelevantOnCurrentMode;
			this.IsMainPrisonersLimitWarningEnabled = this.PartyScreenLogic.RightPartyPrisonersSizeLimit < this.PartyScreenLogic.PrisonerRosters[1].TotalManCount && this.ArePrisonersRelevantOnCurrentMode;
			PartyVM.UpdateAnyTransferableTroops(this.MainPartyTroops, delegate(bool result)
			{
				this.IsMainTroopsHaveTransferableTroops = result;
			}, this.DismissAllTroopsInputKey);
			PartyVM.UpdateAnyTransferableTroops(this.MainPartyPrisoners, delegate(bool result)
			{
				this.IsMainPrisonersHaveTransferableTroops = result;
			}, this.DismissAllPrisonersInputKey);
			PartyVM.UpdateAnyTransferableTroops(this.OtherPartyTroops, delegate(bool result)
			{
				this.IsOtherTroopsHaveTransferableTroops = result;
			}, this.TakeAllTroopsInputKey);
			PartyVM.UpdateAnyTransferableTroops(this.OtherPartyPrisoners, delegate(bool result)
			{
				this.IsOtherPrisonersHaveTransferableTroops = result;
			}, this.TakeAllPrisonersInputKey);
		}

		// Token: 0x0600034C RID: 844 RVA: 0x000161B4 File Offset: 0x000143B4
		private void RefreshPrisonersRecruitable()
		{
			foreach (PartyCharacterVM partyCharacterVM in this.MainPartyPrisoners)
			{
				partyCharacterVM.UpdateRecruitable();
			}
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00016200 File Offset: 0x00014400
		private void RefreshTroopsUpgradeable()
		{
			foreach (PartyCharacterVM partyCharacterVM in this.MainPartyTroops)
			{
				partyCharacterVM.InitializeUpgrades();
			}
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0001624C File Offset: 0x0001444C
		private static void UpdateAnyTransferableTroops(MBBindingList<PartyCharacterVM> partyList, Action<bool> setTransferableBoolean, InputKeyItemVM keyItem)
		{
			bool flag = false;
			for (int i = 0; i < partyList.Count; i++)
			{
				PartyCharacterVM partyCharacterVM = partyList[i];
				if (partyCharacterVM.Troop.Number > 0 && partyCharacterVM.IsTroopTransferrable)
				{
					flag = true;
					break;
				}
			}
			setTransferableBoolean(flag);
			bool? flag2 = null;
			if (!flag)
			{
				flag2 = new bool?(false);
			}
			if (keyItem != null)
			{
				keyItem.SetForcedVisibility(flag2);
			}
		}

		// Token: 0x0600034F RID: 847 RVA: 0x000162B8 File Offset: 0x000144B8
		private static string PopulatePartyListLabel(MBBindingList<PartyCharacterVM> partyList, int limit = 0)
		{
			int num = partyList.Sum((PartyCharacterVM item) => MathF.Max(0, item.Number - item.WoundedCount));
			int num2 = partyList.Sum(delegate(PartyCharacterVM item)
			{
				if (item.Number < item.WoundedCount)
				{
					return 0;
				}
				return item.WoundedCount;
			});
			MBTextManager.SetTextVariable("COUNT", num);
			MBTextManager.SetTextVariable("WEAK_COUNT", num2);
			if (limit != 0)
			{
				MBTextManager.SetTextVariable("MAX_COUNT", limit);
				if (num2 > 0)
				{
					MBTextManager.SetTextVariable("PARTY_LIST_TAG", "", false);
					MBTextManager.SetTextVariable("WEAK_COUNT", num2);
					MBTextManager.SetTextVariable("TOTAL_COUNT", num + num2);
					return GameTexts.FindText("str_party_list_label_with_weak", null).ToString();
				}
				MBTextManager.SetTextVariable("PARTY_LIST_TAG", "", false);
				return GameTexts.FindText("str_party_list_label", null).ToString();
			}
			else
			{
				if (num2 > 0)
				{
					return GameTexts.FindText("str_party_list_label_with_weak_without_max", null).ToString();
				}
				return num.ToString();
			}
		}

		// Token: 0x06000350 RID: 848 RVA: 0x000163B0 File Offset: 0x000145B0
		public void ExecuteTalk()
		{
			if (this.CurrentCharacter.Side == PartyScreenLogic.PartyRosterSide.Right && this.CurrentCharacter.Character != CharacterObject.PlayerCharacter)
			{
				if (Settlement.CurrentSettlement == null)
				{
					CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false), new ConversationCharacterData(this.CurrentCharacter.Character, PartyBase.MainParty, false, false, false, this.CurrentCharacter.IsPrisoner, false, false), "", "");
					return;
				}
				PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationOfCharacter(LocationComplex.Current.GetFirstLocationCharacterOfCharacter(this.CurrentCharacter.Character)), null, this.CurrentCharacter.Character, null);
			}
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0001646C File Offset: 0x0001466C
		public void ExecuteDone()
		{
			if (this.PartyScreenLogic.IsDoneActive())
			{
				this.ExecuteRemoveZeroCounts();
				if (this.PartyScreenLogic.IsThereAnyChanges() && (this.IsMainPrisonersLimitWarningEnabled || this.IsMainTroopsLimitWarningEnabled || this.IsOtherTroopsLimitWarningEnabled))
				{
					GameTexts.SetVariable("newline", "\n");
					string text = string.Empty;
					if (this.IsMainTroopsLimitWarningEnabled)
					{
						text = GameTexts.FindText("str_party_over_limit_troops", null).ToString();
					}
					else if (this.IsMainPrisonersLimitWarningEnabled)
					{
						text = GameTexts.FindText("str_party_over_limit_prisoners", null).ToString();
					}
					else if (this.IsOtherTroopsLimitWarningEnabled)
					{
						text = GameTexts.FindText("str_other_party_over_limit_troops", null).ToString();
					}
					InformationManager.ShowInquiry(new InquiryData(new TextObject("{=uJro3Bua}Over Limit", null).ToString(), text, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.CloseScreenInternal), null, "", 0f, null, null, null), false, false);
					return;
				}
				this.CloseScreenInternal();
			}
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0001657E File Offset: 0x0001477E
		private void CloseScreenInternal()
		{
			this.SaveSortState();
			this.SaveCharacterLockStates();
			PartyScreenManager.CloseScreen(false, false);
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00016593 File Offset: 0x00014793
		public void ExecuteReset()
		{
			this.PartyScreenLogic.Reset(false);
			this.CurrentFocusedCharacter = null;
			this.CurrentFocusedUpgrade = null;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x000165AF File Offset: 0x000147AF
		public void ExecuteCancel()
		{
			if (this.PartyScreenLogic.IsCancelActive())
			{
				this.PartyScreenLogic.Reset(true);
				PartyScreenManager.CloseScreen(false, true);
			}
		}

		// Token: 0x06000355 RID: 853 RVA: 0x000165D4 File Offset: 0x000147D4
		[Conditional("DEBUG")]
		private void EnsureLogicRostersAreInSyncWithVMLists()
		{
			List<TroopRoster> list = new List<TroopRoster>
			{
				this.PartyScreenLogic.GetRoster(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Member),
				this.PartyScreenLogic.GetRoster(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Prisoner),
				this.PartyScreenLogic.GetRoster(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Member),
				this.PartyScreenLogic.GetRoster(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Prisoner)
			};
			List<MBBindingList<PartyCharacterVM>> list2 = new List<MBBindingList<PartyCharacterVM>>
			{
				this.GetPartyCharacterVMList(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Member),
				this.GetPartyCharacterVMList(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Prisoner),
				this.GetPartyCharacterVMList(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Member),
				this.GetPartyCharacterVMList(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Prisoner)
			};
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Count != list2[i].Count)
				{
					Debug.FailedAssert("Logic and VM list counts do not match", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Party\\PartyVM.cs", "EnsureLogicRostersAreInSyncWithVMLists", 1578);
				}
				else
				{
					for (int j = 0; j < list[i].Count; j++)
					{
						if (list[i].GetCharacterAtIndex(j).StringId != list2[i][j].Character.StringId)
						{
							Debug.FailedAssert("Logic and VM rosters do not match", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Party\\PartyVM.cs", "EnsureLogicRostersAreInSyncWithVMLists", 1586);
							return;
						}
					}
				}
			}
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0001671E File Offset: 0x0001491E
		void PartyScreenPrisonHandler.ExecuteTakeAllPrisonersScript()
		{
			this.ExecuteTransferAllOtherPrisoners();
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00016726 File Offset: 0x00014926
		void PartyScreenPrisonHandler.ExecuteDoneScript()
		{
			this.ExecuteDone();
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0001672E File Offset: 0x0001492E
		void PartyScreenPrisonHandler.ExecuteResetScript()
		{
			this.ExecuteReset();
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00016736 File Offset: 0x00014936
		void PartyScreenPrisonHandler.ExecuteSellAllPrisoners()
		{
			this.ExecuteTransferAllMainPrisoners();
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0001673E File Offset: 0x0001493E
		void IPartyScreenTroopHandler.PartyTroopTransfer()
		{
			this.ExecuteTransferAllMainTroops();
		}

		// Token: 0x0600035B RID: 859 RVA: 0x00016746 File Offset: 0x00014946
		void IPartyScreenTroopHandler.ExecuteDoneScript()
		{
			this.ExecuteDone();
		}

		// Token: 0x0600035C RID: 860 RVA: 0x00016750 File Offset: 0x00014950
		public override void OnFinalize()
		{
			base.OnFinalize();
			this._selectedCharacter.OnFinalize();
			this._selectedCharacter = null;
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.CancelInputKey.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.ResetInputKey.OnFinalize();
			this.TakeAllTroopsInputKey.OnFinalize();
			this.DismissAllTroopsInputKey.OnFinalize();
			this.TakeAllPrisonersInputKey.OnFinalize();
			this.DismissAllPrisonersInputKey.OnFinalize();
			InputKeyItemVM openUpgradePanelInputKey = this.OpenUpgradePanelInputKey;
			if (openUpgradePanelInputKey != null)
			{
				openUpgradePanelInputKey.OnFinalize();
			}
			InputKeyItemVM openRecruitPanelInputKey = this.OpenRecruitPanelInputKey;
			if (openRecruitPanelInputKey != null)
			{
				openRecruitPanelInputKey.OnFinalize();
			}
			PartyCharacterVM.ProcessCharacterLock = null;
			PartyCharacterVM.SetSelected = null;
			PartyCharacterVM.OnShift = null;
			PartyCharacterVM.OnFocus = null;
			PartyCharacterVM.OnTransfer = null;
			this.UpgradePopUp.OnFinalize();
			this.RecruitPopUp.OnFinalize();
		}

		// Token: 0x0600035D RID: 861 RVA: 0x00016833 File Offset: 0x00014A33
		public void RequestUserInput(string text, Action accept, Action cancel)
		{
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x0600035E RID: 862 RVA: 0x00016835 File Offset: 0x00014A35
		// (set) Token: 0x0600035F RID: 863 RVA: 0x0001683D File Offset: 0x00014A3D
		[DataSourceProperty]
		public PartySortControllerVM OtherPartySortController
		{
			get
			{
				return this._otherPartySortController;
			}
			set
			{
				if (value != this._otherPartySortController)
				{
					this._otherPartySortController = value;
					base.OnPropertyChangedWithValue<PartySortControllerVM>(value, "OtherPartySortController");
				}
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000360 RID: 864 RVA: 0x0001685B File Offset: 0x00014A5B
		// (set) Token: 0x06000361 RID: 865 RVA: 0x00016863 File Offset: 0x00014A63
		[DataSourceProperty]
		public PartySortControllerVM MainPartySortController
		{
			get
			{
				return this._mainPartySortController;
			}
			set
			{
				if (value != this._mainPartySortController)
				{
					this._mainPartySortController = value;
					base.OnPropertyChangedWithValue<PartySortControllerVM>(value, "MainPartySortController");
				}
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000362 RID: 866 RVA: 0x00016881 File Offset: 0x00014A81
		// (set) Token: 0x06000363 RID: 867 RVA: 0x00016889 File Offset: 0x00014A89
		[DataSourceProperty]
		public PartyCompositionVM OtherPartyComposition
		{
			get
			{
				return this._otherPartyComposition;
			}
			set
			{
				if (value != this._otherPartyComposition)
				{
					this._otherPartyComposition = value;
					base.OnPropertyChangedWithValue<PartyCompositionVM>(value, "OtherPartyComposition");
				}
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000364 RID: 868 RVA: 0x000168A7 File Offset: 0x00014AA7
		// (set) Token: 0x06000365 RID: 869 RVA: 0x000168AF File Offset: 0x00014AAF
		[DataSourceProperty]
		public PartyCompositionVM MainPartyComposition
		{
			get
			{
				return this._mainPartyComposition;
			}
			set
			{
				if (value != this._mainPartyComposition)
				{
					this._mainPartyComposition = value;
					base.OnPropertyChangedWithValue<PartyCompositionVM>(value, "MainPartyComposition");
				}
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000366 RID: 870 RVA: 0x000168CD File Offset: 0x00014ACD
		// (set) Token: 0x06000367 RID: 871 RVA: 0x000168D5 File Offset: 0x00014AD5
		[DataSourceProperty]
		public PartyCharacterVM CurrentFocusedCharacter
		{
			get
			{
				return this._currentFocusedCharacter;
			}
			set
			{
				if (value != this._currentFocusedCharacter)
				{
					this._currentFocusedCharacter = value;
					base.OnPropertyChangedWithValue<PartyCharacterVM>(value, "CurrentFocusedCharacter");
				}
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000368 RID: 872 RVA: 0x000168F3 File Offset: 0x00014AF3
		// (set) Token: 0x06000369 RID: 873 RVA: 0x000168FB File Offset: 0x00014AFB
		[DataSourceProperty]
		public UpgradeTargetVM CurrentFocusedUpgrade
		{
			get
			{
				return this._currentFocusedUpgrade;
			}
			set
			{
				if (value != this._currentFocusedUpgrade)
				{
					this._currentFocusedUpgrade = value;
					base.OnPropertyChangedWithValue<UpgradeTargetVM>(value, "CurrentFocusedUpgrade");
				}
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x0600036A RID: 874 RVA: 0x00016919 File Offset: 0x00014B19
		// (set) Token: 0x0600036B RID: 875 RVA: 0x00016921 File Offset: 0x00014B21
		[DataSourceProperty]
		public string HeaderLbl
		{
			get
			{
				return this._headerLbl;
			}
			set
			{
				if (value != this._headerLbl)
				{
					this._headerLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "HeaderLbl");
				}
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600036C RID: 876 RVA: 0x00016944 File Offset: 0x00014B44
		// (set) Token: 0x0600036D RID: 877 RVA: 0x0001694C File Offset: 0x00014B4C
		[DataSourceProperty]
		public string OtherPartyNameLbl
		{
			get
			{
				return this._otherPartyNameLbl;
			}
			set
			{
				if (value != this._otherPartyNameLbl)
				{
					this._otherPartyNameLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "OtherPartyNameLbl");
				}
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600036E RID: 878 RVA: 0x0001696F File Offset: 0x00014B6F
		// (set) Token: 0x0600036F RID: 879 RVA: 0x00016977 File Offset: 0x00014B77
		[DataSourceProperty]
		public MBBindingList<PartyCharacterVM> OtherPartyTroops
		{
			get
			{
				return this._otherPartyTroops;
			}
			set
			{
				if (value != this._otherPartyTroops)
				{
					this._otherPartyTroops = value;
					base.OnPropertyChangedWithValue<MBBindingList<PartyCharacterVM>>(value, "OtherPartyTroops");
				}
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000370 RID: 880 RVA: 0x00016995 File Offset: 0x00014B95
		// (set) Token: 0x06000371 RID: 881 RVA: 0x0001699D File Offset: 0x00014B9D
		[DataSourceProperty]
		public MBBindingList<PartyCharacterVM> OtherPartyPrisoners
		{
			get
			{
				return this._otherPartyPrisoners;
			}
			set
			{
				if (value != this._otherPartyPrisoners)
				{
					this._otherPartyPrisoners = value;
					base.OnPropertyChangedWithValue<MBBindingList<PartyCharacterVM>>(value, "OtherPartyPrisoners");
				}
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000372 RID: 882 RVA: 0x000169BB File Offset: 0x00014BBB
		// (set) Token: 0x06000373 RID: 883 RVA: 0x000169C3 File Offset: 0x00014BC3
		[DataSourceProperty]
		public MBBindingList<PartyCharacterVM> MainPartyTroops
		{
			get
			{
				return this._mainPartyTroops;
			}
			set
			{
				if (value != this._mainPartyTroops)
				{
					this._mainPartyTroops = value;
					base.OnPropertyChangedWithValue<MBBindingList<PartyCharacterVM>>(value, "MainPartyTroops");
				}
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000374 RID: 884 RVA: 0x000169E1 File Offset: 0x00014BE1
		// (set) Token: 0x06000375 RID: 885 RVA: 0x000169E9 File Offset: 0x00014BE9
		[DataSourceProperty]
		public MBBindingList<PartyCharacterVM> MainPartyPrisoners
		{
			get
			{
				return this._mainPartyPrisoners;
			}
			set
			{
				if (value != this._mainPartyPrisoners)
				{
					this._mainPartyPrisoners = value;
					base.OnPropertyChangedWithValue<MBBindingList<PartyCharacterVM>>(value, "MainPartyPrisoners");
				}
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000376 RID: 886 RVA: 0x00016A07 File Offset: 0x00014C07
		// (set) Token: 0x06000377 RID: 887 RVA: 0x00016A0F File Offset: 0x00014C0F
		[DataSourceProperty]
		public PartyUpgradeTroopVM UpgradePopUp
		{
			get
			{
				return this._upgradePopUp;
			}
			set
			{
				if (value != this._upgradePopUp)
				{
					this._upgradePopUp = value;
					base.OnPropertyChangedWithValue<PartyUpgradeTroopVM>(value, "UpgradePopUp");
				}
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000378 RID: 888 RVA: 0x00016A2D File Offset: 0x00014C2D
		// (set) Token: 0x06000379 RID: 889 RVA: 0x00016A35 File Offset: 0x00014C35
		[DataSourceProperty]
		public PartyRecruitTroopVM RecruitPopUp
		{
			get
			{
				return this._recruitPopUp;
			}
			set
			{
				if (value != this._recruitPopUp)
				{
					this._recruitPopUp = value;
					base.OnPropertyChangedWithValue<PartyRecruitTroopVM>(value, "RecruitPopUp");
				}
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600037A RID: 890 RVA: 0x00016A53 File Offset: 0x00014C53
		// (set) Token: 0x0600037B RID: 891 RVA: 0x00016A5B File Offset: 0x00014C5B
		[DataSourceProperty]
		public HeroViewModel SelectedCharacter
		{
			get
			{
				return this._selectedCharacter;
			}
			set
			{
				if (value != this._selectedCharacter)
				{
					this._selectedCharacter = value;
					base.OnPropertyChangedWithValue<HeroViewModel>(value, "SelectedCharacter");
				}
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x0600037C RID: 892 RVA: 0x00016A79 File Offset: 0x00014C79
		// (set) Token: 0x0600037D RID: 893 RVA: 0x00016A81 File Offset: 0x00014C81
		[DataSourceProperty]
		public string CurrentCharacterLevelLbl
		{
			get
			{
				return this._currentCharacterLevelLbl;
			}
			set
			{
				if (value != this._currentCharacterLevelLbl)
				{
					this._currentCharacterLevelLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCharacterLevelLbl");
				}
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600037E RID: 894 RVA: 0x00016AA4 File Offset: 0x00014CA4
		// (set) Token: 0x0600037F RID: 895 RVA: 0x00016AAC File Offset: 0x00014CAC
		[DataSourceProperty]
		public string CurrentCharacterWageLbl
		{
			get
			{
				return this._currentCharacterWageLbl;
			}
			set
			{
				if (value != this._currentCharacterWageLbl)
				{
					this._currentCharacterWageLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCharacterWageLbl");
				}
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000380 RID: 896 RVA: 0x00016ACF File Offset: 0x00014CCF
		// (set) Token: 0x06000381 RID: 897 RVA: 0x00016AD7 File Offset: 0x00014CD7
		[DataSourceProperty]
		public BasicTooltipViewModel TransferAllOtherTroopsHint
		{
			get
			{
				return this._transferAllOtherTroopsHint;
			}
			set
			{
				if (value != this._transferAllOtherTroopsHint)
				{
					this._transferAllOtherTroopsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TransferAllOtherTroopsHint");
				}
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000382 RID: 898 RVA: 0x00016AF5 File Offset: 0x00014CF5
		// (set) Token: 0x06000383 RID: 899 RVA: 0x00016AFD File Offset: 0x00014CFD
		[DataSourceProperty]
		public BasicTooltipViewModel TransferAllOtherPrisonersHint
		{
			get
			{
				return this._transferAllOtherPrisonersHint;
			}
			set
			{
				if (value != this._transferAllOtherPrisonersHint)
				{
					this._transferAllOtherPrisonersHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TransferAllOtherPrisonersHint");
				}
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000384 RID: 900 RVA: 0x00016B1B File Offset: 0x00014D1B
		// (set) Token: 0x06000385 RID: 901 RVA: 0x00016B23 File Offset: 0x00014D23
		[DataSourceProperty]
		public BasicTooltipViewModel TransferAllMainTroopsHint
		{
			get
			{
				return this._transferAllMainTroopsHint;
			}
			set
			{
				if (value != this._transferAllMainTroopsHint)
				{
					this._transferAllMainTroopsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TransferAllMainTroopsHint");
				}
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000386 RID: 902 RVA: 0x00016B41 File Offset: 0x00014D41
		// (set) Token: 0x06000387 RID: 903 RVA: 0x00016B49 File Offset: 0x00014D49
		[DataSourceProperty]
		public BasicTooltipViewModel TransferAllMainPrisonersHint
		{
			get
			{
				return this._transferAllMainPrisonersHint;
			}
			set
			{
				if (value != this._transferAllMainPrisonersHint)
				{
					this._transferAllMainPrisonersHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TransferAllMainPrisonersHint");
				}
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000388 RID: 904 RVA: 0x00016B67 File Offset: 0x00014D67
		// (set) Token: 0x06000389 RID: 905 RVA: 0x00016B6F File Offset: 0x00014D6F
		[DataSourceProperty]
		public StringItemWithHintVM CurrentCharacterTier
		{
			get
			{
				return this._currentCharacterTier;
			}
			set
			{
				if (value != this._currentCharacterTier)
				{
					this._currentCharacterTier = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "CurrentCharacterTier");
				}
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x0600038A RID: 906 RVA: 0x00016B8D File Offset: 0x00014D8D
		// (set) Token: 0x0600038B RID: 907 RVA: 0x00016B95 File Offset: 0x00014D95
		[DataSourceProperty]
		public HintViewModel ResetHint
		{
			get
			{
				return this._resetHint;
			}
			set
			{
				if (value != this._resetHint)
				{
					this._resetHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResetHint");
				}
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x0600038C RID: 908 RVA: 0x00016BB3 File Offset: 0x00014DB3
		// (set) Token: 0x0600038D RID: 909 RVA: 0x00016BBB File Offset: 0x00014DBB
		[DataSourceProperty]
		public HintViewModel DoneHint
		{
			get
			{
				return this._doneHint;
			}
			set
			{
				if (value != this._doneHint)
				{
					this._doneHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DoneHint");
				}
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x0600038E RID: 910 RVA: 0x00016BD9 File Offset: 0x00014DD9
		// (set) Token: 0x0600038F RID: 911 RVA: 0x00016BE1 File Offset: 0x00014DE1
		[DataSourceProperty]
		public string OtherPartyAccompanyingLbl
		{
			get
			{
				return this._otherPartyAccompanyingLbl;
			}
			set
			{
				if (value != this._otherPartyAccompanyingLbl)
				{
					this._otherPartyAccompanyingLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "OtherPartyAccompanyingLbl");
				}
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000390 RID: 912 RVA: 0x00016C04 File Offset: 0x00014E04
		// (set) Token: 0x06000391 RID: 913 RVA: 0x00016C0C File Offset: 0x00014E0C
		[DataSourceProperty]
		public HintViewModel MoraleHint
		{
			get
			{
				return this._moraleHint;
			}
			set
			{
				if (value != this._moraleHint)
				{
					this._moraleHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "MoraleHint");
				}
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000392 RID: 914 RVA: 0x00016C2A File Offset: 0x00014E2A
		// (set) Token: 0x06000393 RID: 915 RVA: 0x00016C32 File Offset: 0x00014E32
		[DataSourceProperty]
		public HintViewModel TotalWageHint
		{
			get
			{
				return this._totalWageHint;
			}
			set
			{
				if (value != this._totalWageHint)
				{
					this._totalWageHint = value;
					base.OnPropertyChanged("Upgrade2Hint");
				}
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000394 RID: 916 RVA: 0x00016C4F File Offset: 0x00014E4F
		// (set) Token: 0x06000395 RID: 917 RVA: 0x00016C57 File Offset: 0x00014E57
		[DataSourceProperty]
		public BasicTooltipViewModel SpeedHint
		{
			get
			{
				return this._speedHint;
			}
			set
			{
				if (value != this._speedHint)
				{
					this._speedHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SpeedHint");
				}
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000396 RID: 918 RVA: 0x00016C75 File Offset: 0x00014E75
		// (set) Token: 0x06000397 RID: 919 RVA: 0x00016C7D File Offset: 0x00014E7D
		[DataSourceProperty]
		public BasicTooltipViewModel MainPartyTroopSizeLimitHint
		{
			get
			{
				return this._mainPartyTroopSizeLimitHint;
			}
			set
			{
				if (value != this._mainPartyTroopSizeLimitHint)
				{
					this._mainPartyTroopSizeLimitHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "MainPartyTroopSizeLimitHint");
				}
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000398 RID: 920 RVA: 0x00016C9B File Offset: 0x00014E9B
		// (set) Token: 0x06000399 RID: 921 RVA: 0x00016CA3 File Offset: 0x00014EA3
		[DataSourceProperty]
		public BasicTooltipViewModel MainPartyPrisonerSizeLimitHint
		{
			get
			{
				return this._mainPartyPrisonerSizeLimitHint;
			}
			set
			{
				if (value != this._mainPartyPrisonerSizeLimitHint)
				{
					this._mainPartyPrisonerSizeLimitHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "MainPartyPrisonerSizeLimitHint");
				}
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x0600039A RID: 922 RVA: 0x00016CC1 File Offset: 0x00014EC1
		// (set) Token: 0x0600039B RID: 923 RVA: 0x00016CC9 File Offset: 0x00014EC9
		[DataSourceProperty]
		public BasicTooltipViewModel OtherPartyTroopSizeLimitHint
		{
			get
			{
				return this._otherPartyTroopSizeLimitHint;
			}
			set
			{
				if (value != this._otherPartyTroopSizeLimitHint)
				{
					this._otherPartyTroopSizeLimitHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "OtherPartyTroopSizeLimitHint");
				}
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600039C RID: 924 RVA: 0x00016CE7 File Offset: 0x00014EE7
		// (set) Token: 0x0600039D RID: 925 RVA: 0x00016CEF File Offset: 0x00014EEF
		[DataSourceProperty]
		public BasicTooltipViewModel OtherPartyPrisonerSizeLimitHint
		{
			get
			{
				return this._otherPartyPrisonerSizeLimitHint;
			}
			set
			{
				if (value != this._otherPartyPrisonerSizeLimitHint)
				{
					this._otherPartyPrisonerSizeLimitHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "OtherPartyPrisonerSizeLimitHint");
				}
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x0600039E RID: 926 RVA: 0x00016D0D File Offset: 0x00014F0D
		// (set) Token: 0x0600039F RID: 927 RVA: 0x00016D15 File Offset: 0x00014F15
		[DataSourceProperty]
		public BasicTooltipViewModel UsedHorsesHint
		{
			get
			{
				return this._usedHorsesHint;
			}
			set
			{
				if (value != this._usedHorsesHint)
				{
					this._usedHorsesHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "UsedHorsesHint");
				}
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x00016D33 File Offset: 0x00014F33
		// (set) Token: 0x060003A1 RID: 929 RVA: 0x00016D3B File Offset: 0x00014F3B
		[DataSourceProperty]
		public HintViewModel DenarHint
		{
			get
			{
				return this._denarHint;
			}
			set
			{
				if (value != this._denarHint)
				{
					this._denarHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DenarHint");
				}
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060003A2 RID: 930 RVA: 0x00016D59 File Offset: 0x00014F59
		// (set) Token: 0x060003A3 RID: 931 RVA: 0x00016D61 File Offset: 0x00014F61
		[DataSourceProperty]
		public HintViewModel LevelHint
		{
			get
			{
				return this._levelHint;
			}
			set
			{
				if (value != this._levelHint)
				{
					this._levelHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LevelHint");
				}
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060003A4 RID: 932 RVA: 0x00016D7F File Offset: 0x00014F7F
		// (set) Token: 0x060003A5 RID: 933 RVA: 0x00016D87 File Offset: 0x00014F87
		[DataSourceProperty]
		public HintViewModel WageHint
		{
			get
			{
				return this._wageHint;
			}
			set
			{
				if (value != this._wageHint)
				{
					this._wageHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "WageHint");
				}
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x00016DA5 File Offset: 0x00014FA5
		// (set) Token: 0x060003A7 RID: 935 RVA: 0x00016DAD File Offset: 0x00014FAD
		[DataSourceProperty]
		public string TitleLbl
		{
			get
			{
				return this._titleLbl;
			}
			set
			{
				if (value != this._titleLbl)
				{
					this._titleLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleLbl");
				}
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00016DD0 File Offset: 0x00014FD0
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x00016DD8 File Offset: 0x00014FD8
		[DataSourceProperty]
		public string MainPartyNameLbl
		{
			get
			{
				return this._mainPartyNameLbl;
			}
			set
			{
				if (value != this._mainPartyNameLbl)
				{
					this._mainPartyNameLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "MainPartyNameLbl");
				}
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00016DFB File Offset: 0x00014FFB
		// (set) Token: 0x060003AB RID: 939 RVA: 0x00016E03 File Offset: 0x00015003
		[DataSourceProperty]
		public HintViewModel FormationHint
		{
			get
			{
				return this._formationHint;
			}
			set
			{
				if (value != this._formationHint)
				{
					this._formationHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FormationHint");
				}
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060003AC RID: 940 RVA: 0x00016E21 File Offset: 0x00015021
		// (set) Token: 0x060003AD RID: 941 RVA: 0x00016E29 File Offset: 0x00015029
		[DataSourceProperty]
		public string TalkLbl
		{
			get
			{
				return this._talkLbl;
			}
			set
			{
				if (value != this._talkLbl)
				{
					this._talkLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "TalkLbl");
				}
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060003AE RID: 942 RVA: 0x00016E4C File Offset: 0x0001504C
		// (set) Token: 0x060003AF RID: 943 RVA: 0x00016E54 File Offset: 0x00015054
		[DataSourceProperty]
		public string InfoLbl
		{
			get
			{
				return this._infoLbl;
			}
			set
			{
				if (value != this._infoLbl)
				{
					this._infoLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "InfoLbl");
				}
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060003B0 RID: 944 RVA: 0x00016E77 File Offset: 0x00015077
		// (set) Token: 0x060003B1 RID: 945 RVA: 0x00016E7F File Offset: 0x0001507F
		[DataSourceProperty]
		public string CancelLbl
		{
			get
			{
				return this._cancelLbl;
			}
			set
			{
				if (value != this._cancelLbl)
				{
					this._cancelLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelLbl");
				}
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x00016EA2 File Offset: 0x000150A2
		// (set) Token: 0x060003B3 RID: 947 RVA: 0x00016EAA File Offset: 0x000150AA
		[DataSourceProperty]
		public string DoneLbl
		{
			get
			{
				return this._doneLbl;
			}
			set
			{
				if (value != this._doneLbl)
				{
					this._doneLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneLbl");
				}
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x00016ECD File Offset: 0x000150CD
		// (set) Token: 0x060003B5 RID: 949 RVA: 0x00016ED5 File Offset: 0x000150D5
		[DataSourceProperty]
		public string TroopsLabel
		{
			get
			{
				return this._troopsLbl;
			}
			set
			{
				if (value != this._troopsLbl)
				{
					this._troopsLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "TroopsLabel");
				}
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x00016EF8 File Offset: 0x000150F8
		// (set) Token: 0x060003B7 RID: 951 RVA: 0x00016F00 File Offset: 0x00015100
		[DataSourceProperty]
		public string PrisonersLabel
		{
			get
			{
				return this._prisonersLabel;
			}
			set
			{
				if (value != this._prisonersLabel)
				{
					this._prisonersLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "PrisonersLabel");
				}
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x00016F23 File Offset: 0x00015123
		// (set) Token: 0x060003B9 RID: 953 RVA: 0x00016F2B File Offset: 0x0001512B
		[DataSourceProperty]
		public string MainPartyTotalGoldLbl
		{
			get
			{
				return this._mainPartyTotalGoldLbl;
			}
			set
			{
				if (value != this._mainPartyTotalGoldLbl)
				{
					this._mainPartyTotalGoldLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "MainPartyTotalGoldLbl");
				}
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060003BA RID: 954 RVA: 0x00016F4E File Offset: 0x0001514E
		// (set) Token: 0x060003BB RID: 955 RVA: 0x00016F56 File Offset: 0x00015156
		[DataSourceProperty]
		public string MainPartyTotalMoraleLbl
		{
			get
			{
				return this._mainPartyTotalMoraleLbl;
			}
			set
			{
				if (value != this._mainPartyTotalMoraleLbl)
				{
					this._mainPartyTotalMoraleLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "MainPartyTotalMoraleLbl");
				}
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060003BC RID: 956 RVA: 0x00016F79 File Offset: 0x00015179
		// (set) Token: 0x060003BD RID: 957 RVA: 0x00016F81 File Offset: 0x00015181
		[DataSourceProperty]
		public string MainPartyTotalSpeedLbl
		{
			get
			{
				return this._mainPartyTotalSpeedLbl;
			}
			set
			{
				if (value != this._mainPartyTotalSpeedLbl)
				{
					this._mainPartyTotalSpeedLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "MainPartyTotalSpeedLbl");
				}
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060003BE RID: 958 RVA: 0x00016FA4 File Offset: 0x000151A4
		// (set) Token: 0x060003BF RID: 959 RVA: 0x00016FAC File Offset: 0x000151AC
		[DataSourceProperty]
		public string MainPartyTotalWeeklyCostLbl
		{
			get
			{
				return this._mainPartyTotalWeeklyCostLbl;
			}
			set
			{
				if (value != this._mainPartyTotalWeeklyCostLbl)
				{
					this._mainPartyTotalWeeklyCostLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "MainPartyTotalWeeklyCostLbl");
				}
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x00016FCF File Offset: 0x000151CF
		// (set) Token: 0x060003C1 RID: 961 RVA: 0x00016FD7 File Offset: 0x000151D7
		[DataSourceProperty]
		public bool IsCurrentCharacterFormationEnabled
		{
			get
			{
				return this._isCurrentCharacterFormationEnabled;
			}
			set
			{
				if (value != this._isCurrentCharacterFormationEnabled)
				{
					this._isCurrentCharacterFormationEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCurrentCharacterFormationEnabled");
				}
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x00016FF5 File Offset: 0x000151F5
		// (set) Token: 0x060003C3 RID: 963 RVA: 0x00016FFD File Offset: 0x000151FD
		[DataSourceProperty]
		public bool IsCurrentCharacterWageEnabled
		{
			get
			{
				return this._isCurrentCharacterWageEnabled;
			}
			set
			{
				if (value != this._isCurrentCharacterWageEnabled)
				{
					this._isCurrentCharacterWageEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCurrentCharacterWageEnabled");
				}
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x0001701B File Offset: 0x0001521B
		// (set) Token: 0x060003C5 RID: 965 RVA: 0x00017023 File Offset: 0x00015223
		[DataSourceProperty]
		public bool CanChooseRoles
		{
			get
			{
				return this._canChooseRoles;
			}
			set
			{
				if (value != this._canChooseRoles)
				{
					this._canChooseRoles = value;
					base.OnPropertyChangedWithValue(value, "CanChooseRoles");
				}
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x00017041 File Offset: 0x00015241
		// (set) Token: 0x060003C7 RID: 967 RVA: 0x00017049 File Offset: 0x00015249
		[DataSourceProperty]
		public string OtherPartyTroopsLbl
		{
			get
			{
				return this._otherPartyTroopsLbl;
			}
			set
			{
				if (value != this._otherPartyTroopsLbl)
				{
					this._otherPartyTroopsLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "OtherPartyTroopsLbl");
				}
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x0001706C File Offset: 0x0001526C
		// (set) Token: 0x060003C9 RID: 969 RVA: 0x00017074 File Offset: 0x00015274
		[DataSourceProperty]
		public string OtherPartyPrisonersLbl
		{
			get
			{
				return this._otherPartyPrisonersLbl;
			}
			set
			{
				if (value != this._otherPartyPrisonersLbl)
				{
					this._otherPartyPrisonersLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "OtherPartyPrisonersLbl");
				}
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060003CA RID: 970 RVA: 0x00017097 File Offset: 0x00015297
		// (set) Token: 0x060003CB RID: 971 RVA: 0x0001709F File Offset: 0x0001529F
		[DataSourceProperty]
		public string MainPartyTroopsLbl
		{
			get
			{
				return this._mainPartyTroopsLbl;
			}
			set
			{
				if (value != this._mainPartyTroopsLbl)
				{
					this._mainPartyTroopsLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "MainPartyTroopsLbl");
				}
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060003CC RID: 972 RVA: 0x000170C2 File Offset: 0x000152C2
		// (set) Token: 0x060003CD RID: 973 RVA: 0x000170CA File Offset: 0x000152CA
		[DataSourceProperty]
		public string MainPartyPrisonersLbl
		{
			get
			{
				return this._mainPartyPrisonersLbl;
			}
			set
			{
				if (value != this._mainPartyPrisonersLbl)
				{
					this._mainPartyPrisonersLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "MainPartyPrisonersLbl");
				}
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060003CE RID: 974 RVA: 0x000170ED File Offset: 0x000152ED
		// (set) Token: 0x060003CF RID: 975 RVA: 0x000170F5 File Offset: 0x000152F5
		[DataSourceProperty]
		public bool ShowQuestProgress
		{
			get
			{
				return this._showQuestProgress;
			}
			set
			{
				if (value != this._showQuestProgress)
				{
					this._showQuestProgress = value;
					base.OnPropertyChangedWithValue(value, "ShowQuestProgress");
				}
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060003D0 RID: 976 RVA: 0x00017113 File Offset: 0x00015313
		// (set) Token: 0x060003D1 RID: 977 RVA: 0x0001711B File Offset: 0x0001531B
		[DataSourceProperty]
		public int QuestProgressRequiredCount
		{
			get
			{
				return this._questProgressRequiredCount;
			}
			set
			{
				if (value != this._questProgressRequiredCount)
				{
					this._questProgressRequiredCount = value;
					base.OnPropertyChangedWithValue(value, "QuestProgressRequiredCount");
				}
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060003D2 RID: 978 RVA: 0x00017139 File Offset: 0x00015339
		// (set) Token: 0x060003D3 RID: 979 RVA: 0x00017141 File Offset: 0x00015341
		[DataSourceProperty]
		public int QuestProgressCurrentCount
		{
			get
			{
				return this._questProgressCurrentCount;
			}
			set
			{
				if (value != this._questProgressCurrentCount)
				{
					this._questProgressCurrentCount = value;
					base.OnPropertyChangedWithValue(value, "QuestProgressCurrentCount");
				}
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060003D4 RID: 980 RVA: 0x0001715F File Offset: 0x0001535F
		// (set) Token: 0x060003D5 RID: 981 RVA: 0x00017167 File Offset: 0x00015367
		[DataSourceProperty]
		public int UpgradableTroopCount
		{
			get
			{
				return this._upgradableTroopCount;
			}
			set
			{
				if (value != this._upgradableTroopCount)
				{
					this._upgradableTroopCount = value;
					base.OnPropertyChangedWithValue(value, "UpgradableTroopCount");
				}
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060003D6 RID: 982 RVA: 0x00017185 File Offset: 0x00015385
		// (set) Token: 0x060003D7 RID: 983 RVA: 0x0001718D File Offset: 0x0001538D
		[DataSourceProperty]
		public int RecruitableTroopCount
		{
			get
			{
				return this._recruitableTroopCount;
			}
			set
			{
				if (value != this._recruitableTroopCount)
				{
					this._recruitableTroopCount = value;
					base.OnPropertyChangedWithValue(value, "RecruitableTroopCount");
				}
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x000171AB File Offset: 0x000153AB
		// (set) Token: 0x060003D9 RID: 985 RVA: 0x000171B3 File Offset: 0x000153B3
		[DataSourceProperty]
		public bool IsDoneDisabled
		{
			get
			{
				return this._isDoneDisabled;
			}
			set
			{
				if (value != this._isDoneDisabled)
				{
					this._isDoneDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDoneDisabled");
				}
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060003DA RID: 986 RVA: 0x000171D1 File Offset: 0x000153D1
		// (set) Token: 0x060003DB RID: 987 RVA: 0x000171D9 File Offset: 0x000153D9
		[DataSourceProperty]
		public bool IsUpgradePopUpDisabled
		{
			get
			{
				return this._isUpgradePopUpDisabled;
			}
			set
			{
				if (value != this._isUpgradePopUpDisabled)
				{
					this._isUpgradePopUpDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsUpgradePopUpDisabled");
				}
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060003DC RID: 988 RVA: 0x000171F7 File Offset: 0x000153F7
		// (set) Token: 0x060003DD RID: 989 RVA: 0x000171FF File Offset: 0x000153FF
		[DataSourceProperty]
		public bool IsRecruitPopUpDisabled
		{
			get
			{
				return this._isRecruitPopUpDisabled;
			}
			set
			{
				if (value != this._isRecruitPopUpDisabled)
				{
					this._isRecruitPopUpDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsRecruitPopUpDisabled");
				}
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060003DE RID: 990 RVA: 0x0001721D File Offset: 0x0001541D
		// (set) Token: 0x060003DF RID: 991 RVA: 0x00017225 File Offset: 0x00015425
		[DataSourceProperty]
		public bool IsMainPrisonersLimitWarningEnabled
		{
			get
			{
				return this._isMainPrisonersLimitWarningEnabled;
			}
			set
			{
				if (value != this._isMainPrisonersLimitWarningEnabled)
				{
					this._isMainPrisonersLimitWarningEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMainPrisonersLimitWarningEnabled");
				}
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060003E0 RID: 992 RVA: 0x00017243 File Offset: 0x00015443
		// (set) Token: 0x060003E1 RID: 993 RVA: 0x0001724B File Offset: 0x0001544B
		[DataSourceProperty]
		public bool IsMainTroopsLimitWarningEnabled
		{
			get
			{
				return this._isMainTroopsLimitWarningEnabled;
			}
			set
			{
				if (value != this._isMainTroopsLimitWarningEnabled)
				{
					this._isMainTroopsLimitWarningEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMainTroopsLimitWarningEnabled");
				}
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060003E2 RID: 994 RVA: 0x00017269 File Offset: 0x00015469
		// (set) Token: 0x060003E3 RID: 995 RVA: 0x00017271 File Offset: 0x00015471
		[DataSourceProperty]
		public bool IsOtherPrisonersLimitWarningEnabled
		{
			get
			{
				return this._isOtherPrisonersLimitWarningEnabled;
			}
			set
			{
				if (value != this._isOtherPrisonersLimitWarningEnabled)
				{
					this._isOtherPrisonersLimitWarningEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsOtherPrisonersLimitWarningEnabled");
				}
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060003E4 RID: 996 RVA: 0x0001728F File Offset: 0x0001548F
		// (set) Token: 0x060003E5 RID: 997 RVA: 0x00017297 File Offset: 0x00015497
		[DataSourceProperty]
		public bool IsUpgradePopupButtonHighlightEnabled
		{
			get
			{
				return this._isUpgradePopupButtonHighlightEnabled;
			}
			set
			{
				if (value != this._isUpgradePopupButtonHighlightEnabled)
				{
					this._isUpgradePopupButtonHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsUpgradePopupButtonHighlightEnabled");
				}
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x000172B5 File Offset: 0x000154B5
		// (set) Token: 0x060003E7 RID: 999 RVA: 0x000172BD File Offset: 0x000154BD
		[DataSourceProperty]
		public bool IsOtherTroopsLimitWarningEnabled
		{
			get
			{
				return this._isOtherTroopsLimitWarningEnabled;
			}
			set
			{
				if (value != this._isOtherTroopsLimitWarningEnabled)
				{
					this._isOtherTroopsLimitWarningEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsOtherTroopsLimitWarningEnabled");
				}
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060003E8 RID: 1000 RVA: 0x000172DB File Offset: 0x000154DB
		// (set) Token: 0x060003E9 RID: 1001 RVA: 0x000172E3 File Offset: 0x000154E3
		[DataSourceProperty]
		public bool IsMainTroopsHaveTransferableTroops
		{
			get
			{
				return this._isMainTroopsHaveTransferableTroops;
			}
			set
			{
				if (value != this._isMainTroopsHaveTransferableTroops)
				{
					this._isMainTroopsHaveTransferableTroops = value;
					base.OnPropertyChangedWithValue(value, "IsMainTroopsHaveTransferableTroops");
				}
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060003EA RID: 1002 RVA: 0x00017301 File Offset: 0x00015501
		// (set) Token: 0x060003EB RID: 1003 RVA: 0x00017309 File Offset: 0x00015509
		[DataSourceProperty]
		public bool IsMainPrisonersHaveTransferableTroops
		{
			get
			{
				return this._isMainPrisonersHaveTransferableTroops;
			}
			set
			{
				if (value != this._isMainPrisonersHaveTransferableTroops)
				{
					this._isMainPrisonersHaveTransferableTroops = value;
					base.OnPropertyChangedWithValue(value, "IsMainPrisonersHaveTransferableTroops");
				}
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x00017327 File Offset: 0x00015527
		// (set) Token: 0x060003ED RID: 1005 RVA: 0x0001732F File Offset: 0x0001552F
		[DataSourceProperty]
		public bool IsOtherTroopsHaveTransferableTroops
		{
			get
			{
				return this._isOtherTroopsHaveTransferableTroops;
			}
			set
			{
				if (value != this._isOtherTroopsHaveTransferableTroops)
				{
					this._isOtherTroopsHaveTransferableTroops = value;
					base.OnPropertyChangedWithValue(value, "IsOtherTroopsHaveTransferableTroops");
				}
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x0001734D File Offset: 0x0001554D
		// (set) Token: 0x060003EF RID: 1007 RVA: 0x00017355 File Offset: 0x00015555
		[DataSourceProperty]
		public bool IsOtherPrisonersHaveTransferableTroops
		{
			get
			{
				return this._isOtherPrisonersHaveTransferableTroops;
			}
			set
			{
				if (value != this._isOtherPrisonersHaveTransferableTroops)
				{
					this._isOtherPrisonersHaveTransferableTroops = value;
					base.OnPropertyChangedWithValue(value, "IsOtherPrisonersHaveTransferableTroops");
				}
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00017373 File Offset: 0x00015573
		// (set) Token: 0x060003F1 RID: 1009 RVA: 0x0001737B File Offset: 0x0001557B
		[DataSourceProperty]
		public bool IsCancelDisabled
		{
			get
			{
				return this._isCancelDisabled;
			}
			set
			{
				if (value != this._isCancelDisabled)
				{
					this._isCancelDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsCancelDisabled");
				}
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00017399 File Offset: 0x00015599
		// (set) Token: 0x060003F3 RID: 1011 RVA: 0x000173A1 File Offset: 0x000155A1
		[DataSourceProperty]
		public bool AreMembersRelevantOnCurrentMode
		{
			get
			{
				return this._anyMembersOnEitherSide;
			}
			set
			{
				if (value != this._anyMembersOnEitherSide)
				{
					this._anyMembersOnEitherSide = value;
					base.OnPropertyChangedWithValue(value, "AreMembersRelevantOnCurrentMode");
				}
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x000173BF File Offset: 0x000155BF
		// (set) Token: 0x060003F5 RID: 1013 RVA: 0x000173C7 File Offset: 0x000155C7
		[DataSourceProperty]
		public bool ArePrisonersRelevantOnCurrentMode
		{
			get
			{
				return this._anyPrisonersOnEitherSide;
			}
			set
			{
				if (value != this._anyPrisonersOnEitherSide)
				{
					this._anyPrisonersOnEitherSide = value;
					base.OnPropertyChangedWithValue(value, "ArePrisonersRelevantOnCurrentMode");
				}
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x000173E5 File Offset: 0x000155E5
		// (set) Token: 0x060003F7 RID: 1015 RVA: 0x000173ED File Offset: 0x000155ED
		[DataSourceProperty]
		public string GoldChangeText
		{
			get
			{
				return this._goldChangeText;
			}
			set
			{
				if (value != this._goldChangeText)
				{
					this._goldChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GoldChangeText");
				}
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00017410 File Offset: 0x00015610
		// (set) Token: 0x060003F9 RID: 1017 RVA: 0x00017418 File Offset: 0x00015618
		[DataSourceProperty]
		public string MoraleChangeText
		{
			get
			{
				return this._moraleChangeText;
			}
			set
			{
				if (value != this._moraleChangeText)
				{
					this._moraleChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "MoraleChangeText");
				}
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x0001743B File Offset: 0x0001563B
		// (set) Token: 0x060003FB RID: 1019 RVA: 0x00017443 File Offset: 0x00015643
		[DataSourceProperty]
		public string HorseChangeText
		{
			get
			{
				return this._horseChangeText;
			}
			set
			{
				if (value != this._horseChangeText)
				{
					this._horseChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "HorseChangeText");
				}
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060003FC RID: 1020 RVA: 0x00017466 File Offset: 0x00015666
		// (set) Token: 0x060003FD RID: 1021 RVA: 0x0001746E File Offset: 0x0001566E
		[DataSourceProperty]
		public string InfluenceChangeText
		{
			get
			{
				return this._influenceChangeText;
			}
			set
			{
				if (value != this._influenceChangeText)
				{
					this._influenceChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "InfluenceChangeText");
				}
			}
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00017491 File Offset: 0x00015691
		private TextObject GetTransferAllOtherTroopsKeyText()
		{
			if (this.TakeAllTroopsInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.TakeAllTroopsInputKey.KeyID);
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x000174BF File Offset: 0x000156BF
		private TextObject GetTransferAllMainTroopsKeyText()
		{
			if (this.DismissAllTroopsInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.DismissAllTroopsInputKey.KeyID);
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x000174ED File Offset: 0x000156ED
		private TextObject GetTransferAllOtherPrisonersKeyText()
		{
			if (this.TakeAllPrisonersInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.TakeAllPrisonersInputKey.KeyID);
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0001751B File Offset: 0x0001571B
		private TextObject GetTransferAllMainPrisonersKeyText()
		{
			if (this.DismissAllPrisonersInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.DismissAllPrisonersInputKey.KeyID);
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00017549 File Offset: 0x00015749
		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00017558 File Offset: 0x00015758
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00017567 File Offset: 0x00015767
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.UpgradePopUp.SetDoneInputKey(hotKey);
			this.RecruitPopUp.SetDoneInputKey(hotKey);
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00017590 File Offset: 0x00015790
		public void SetTakeAllTroopsInputKey(HotKey hotKey)
		{
			this.TakeAllTroopsInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.TransferAllOtherTroopsHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("TEXT", new TextObject("{=sVsaWZjg}Transfer All Other Troops", null));
				GameTexts.SetVariable("HOTKEY", this.GetTransferAllOtherTroopsKeyText());
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
			PartyVM.UpdateAnyTransferableTroops(this.OtherPartyTroops, delegate(bool result)
			{
				this.IsOtherTroopsHaveTransferableTroops = result;
			}, this.TakeAllTroopsInputKey);
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x000175E0 File Offset: 0x000157E0
		public void SetDismissAllTroopsInputKey(HotKey hotKey)
		{
			this.DismissAllTroopsInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.TransferAllMainTroopsHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("TEXT", new TextObject("{=Pvx4TU66}Transfer All Main Troops", null));
				GameTexts.SetVariable("HOTKEY", this.GetTransferAllMainTroopsKeyText());
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
			PartyVM.UpdateAnyTransferableTroops(this.MainPartyTroops, delegate(bool result)
			{
				this.IsMainTroopsHaveTransferableTroops = result;
			}, this.DismissAllTroopsInputKey);
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00017630 File Offset: 0x00015830
		public void SetTakeAllPrisonersInputKey(HotKey hotKey)
		{
			this.TakeAllPrisonersInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.TransferAllOtherPrisonersHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("TEXT", new TextObject("{=Srr4rOSq}Transfer All Other Prisoners", null));
				GameTexts.SetVariable("HOTKEY", this.GetTransferAllOtherPrisonersKeyText());
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
			PartyVM.UpdateAnyTransferableTroops(this.OtherPartyPrisoners, delegate(bool result)
			{
				this.IsOtherPrisonersHaveTransferableTroops = result;
			}, this.TakeAllPrisonersInputKey);
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00017680 File Offset: 0x00015880
		public void SetDismissAllPrisonersInputKey(HotKey hotKey)
		{
			this.DismissAllPrisonersInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.TransferAllMainPrisonersHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("TEXT", new TextObject("{=PpbopMjT}Transfer All Main Prisoners", null));
				GameTexts.SetVariable("HOTKEY", this.GetTransferAllMainPrisonersKeyText());
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
			PartyVM.UpdateAnyTransferableTroops(this.MainPartyPrisoners, delegate(bool result)
			{
				this.IsMainPrisonersHaveTransferableTroops = result;
			}, this.DismissAllPrisonersInputKey);
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x000176CE File Offset: 0x000158CE
		public void SetOpenUpgradePanelInputKey(HotKey hotKey)
		{
			this.OpenUpgradePanelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x000176DD File Offset: 0x000158DD
		public void SetOpenRecruitPanelInputKey(HotKey hotKey)
		{
			this.OpenRecruitPanelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x000176EC File Offset: 0x000158EC
		public void SetGetKeyTextFromKeyIDFunc(Func<string, TextObject> getKeyTextFromKeyId)
		{
			this._getKeyTextFromKeyId = getKeyTextFromKeyId;
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x000176F5 File Offset: 0x000158F5
		// (set) Token: 0x0600040D RID: 1037 RVA: 0x000176FD File Offset: 0x000158FD
		[DataSourceProperty]
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x0001771B File Offset: 0x0001591B
		// (set) Token: 0x0600040F RID: 1039 RVA: 0x00017723 File Offset: 0x00015923
		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x00017741 File Offset: 0x00015941
		// (set) Token: 0x06000411 RID: 1041 RVA: 0x00017749 File Offset: 0x00015949
		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x00017767 File Offset: 0x00015967
		// (set) Token: 0x06000413 RID: 1043 RVA: 0x0001776F File Offset: 0x0001596F
		[DataSourceProperty]
		public InputKeyItemVM TakeAllTroopsInputKey
		{
			get
			{
				return this._takeAllTroopsInputKey;
			}
			set
			{
				if (value != this._takeAllTroopsInputKey)
				{
					this._takeAllTroopsInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "TakeAllTroopsInputKey");
				}
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000414 RID: 1044 RVA: 0x0001778D File Offset: 0x0001598D
		// (set) Token: 0x06000415 RID: 1045 RVA: 0x00017795 File Offset: 0x00015995
		[DataSourceProperty]
		public InputKeyItemVM DismissAllTroopsInputKey
		{
			get
			{
				return this._dismissAllTroopsInputKey;
			}
			set
			{
				if (value != this._dismissAllTroopsInputKey)
				{
					this._dismissAllTroopsInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DismissAllTroopsInputKey");
				}
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x000177B3 File Offset: 0x000159B3
		// (set) Token: 0x06000417 RID: 1047 RVA: 0x000177BB File Offset: 0x000159BB
		[DataSourceProperty]
		public InputKeyItemVM TakeAllPrisonersInputKey
		{
			get
			{
				return this._takeAllPrisonersInputKey;
			}
			set
			{
				if (value != this._takeAllPrisonersInputKey)
				{
					this._takeAllPrisonersInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "TakeAllPrisonersInputKey");
				}
			}
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x000177DC File Offset: 0x000159DC
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null)
				{
					if (this._isUpgradePopupButtonHighlightApplied)
					{
						this.IsUpgradePopupButtonHighlightEnabled = false;
						this._isUpgradePopupButtonHighlightApplied = false;
					}
					if (this._isUpgradeButtonHighlightApplied)
					{
						this.SetUpgradeButtonsHighlightState(false);
						this._isUpgradeButtonHighlightApplied = false;
					}
					if (this._isRecruitButtonHighlightApplied)
					{
						this.SetRecruitButtonsHighlightState(false);
						this._isRecruitButtonHighlightApplied = false;
					}
					if (this._isTransferButtonHighlightApplied)
					{
						this.SetTransferButtonHighlightState(false, null);
						this._isTransferButtonHighlightApplied = false;
					}
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._latestTutorialElementID != null)
				{
					if (!this._isUpgradePopupButtonHighlightApplied && this._latestTutorialElementID == this._upgradePopupButtonID)
					{
						this.IsUpgradePopupButtonHighlightEnabled = true;
						this._isUpgradePopupButtonHighlightApplied = true;
					}
					if (this._latestTutorialElementID == this._upgradeButtonID)
					{
						this.SetUpgradeButtonsHighlightState(true);
						this._isUpgradeButtonHighlightApplied = true;
					}
					if (!this._isRecruitButtonHighlightApplied && this._latestTutorialElementID == this._recruitButtonID)
					{
						this.SetRecruitButtonsHighlightState(true);
						this._isRecruitButtonHighlightApplied = true;
					}
					if (!this._isTransferButtonHighlightApplied && this._latestTutorialElementID == this._transferButtonOnlyOtherPrisonersID)
					{
						this.SetTransferButtonHighlightState(true, (PartyCharacterVM x) => x.Side == PartyScreenLogic.PartyRosterSide.Left && x.IsPrisoner && x.IsTroopTransferrable);
						this._isTransferButtonHighlightApplied = true;
					}
				}
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x00017932 File Offset: 0x00015B32
		// (set) Token: 0x0600041A RID: 1050 RVA: 0x0001793A File Offset: 0x00015B3A
		[DataSourceProperty]
		public InputKeyItemVM DismissAllPrisonersInputKey
		{
			get
			{
				return this._dismissAllPrisonersInputKey;
			}
			set
			{
				if (value != this._dismissAllPrisonersInputKey)
				{
					this._dismissAllPrisonersInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DismissAllPrisonersInputKey");
				}
			}
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x00017958 File Offset: 0x00015B58
		private void SetUpgradeButtonsHighlightState(bool state)
		{
			MBBindingList<PartyCharacterVM> mainPartyTroops = this.MainPartyTroops;
			if (mainPartyTroops == null)
			{
				return;
			}
			mainPartyTroops.ApplyActionOnAllItems(delegate(PartyCharacterVM x)
			{
				x.SetIsUpgradeButtonHighlighted(state);
			});
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x0001798E File Offset: 0x00015B8E
		// (set) Token: 0x0600041D RID: 1053 RVA: 0x00017996 File Offset: 0x00015B96
		[DataSourceProperty]
		public InputKeyItemVM OpenUpgradePanelInputKey
		{
			get
			{
				return this._openUpgradePanelInputKey;
			}
			set
			{
				if (value != this._openUpgradePanelInputKey)
				{
					this._openUpgradePanelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "OpenUpgradePanelInputKey");
				}
			}
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x000179B4 File Offset: 0x00015BB4
		private void SetRecruitButtonsHighlightState(bool state)
		{
			foreach (PartyCharacterVM partyCharacterVM in this.MainPartyTroops)
			{
				partyCharacterVM.IsRecruitButtonsHiglighted = state;
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x00017A00 File Offset: 0x00015C00
		// (set) Token: 0x06000420 RID: 1056 RVA: 0x00017A08 File Offset: 0x00015C08
		[DataSourceProperty]
		public InputKeyItemVM OpenRecruitPanelInputKey
		{
			get
			{
				return this._openRecruitPanelInputKey;
			}
			set
			{
				if (value != this._openRecruitPanelInputKey)
				{
					this._openRecruitPanelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "OpenRecruitPanelInputKey");
				}
			}
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00017A28 File Offset: 0x00015C28
		private void SetTransferButtonHighlightState(bool state, Func<PartyCharacterVM, bool> predicate)
		{
			foreach (PartyCharacterVM partyCharacterVM in this.MainPartyTroops)
			{
				if (predicate == null || predicate(partyCharacterVM))
				{
					partyCharacterVM.IsTransferButtonHiglighted = state;
				}
			}
			foreach (PartyCharacterVM partyCharacterVM2 in this.MainPartyPrisoners)
			{
				if (predicate == null || predicate(partyCharacterVM2))
				{
					partyCharacterVM2.IsTransferButtonHiglighted = state;
				}
			}
			foreach (PartyCharacterVM partyCharacterVM3 in this.OtherPartyTroops)
			{
				if (predicate == null || predicate(partyCharacterVM3))
				{
					partyCharacterVM3.IsTransferButtonHiglighted = state;
				}
			}
			foreach (PartyCharacterVM partyCharacterVM4 in this.OtherPartyPrisoners)
			{
				if (predicate == null || predicate(partyCharacterVM4))
				{
					partyCharacterVM4.IsTransferButtonHiglighted = state;
				}
			}
		}

		// Token: 0x04000164 RID: 356
		private readonly PartyScreenMode _currentMode;

		// Token: 0x04000165 RID: 357
		private readonly IViewDataTracker _viewDataTracker;

		// Token: 0x04000166 RID: 358
		public bool IsFiveStackModifierActive;

		// Token: 0x04000167 RID: 359
		public bool IsEntireStackModifierActive;

		// Token: 0x04000168 RID: 360
		private PartyCharacterVM _currentCharacter;

		// Token: 0x04000169 RID: 361
		private List<string> _lockedTroopIDs;

		// Token: 0x0400016A RID: 362
		private List<string> _lockedPrisonerIDs;

		// Token: 0x0400016B RID: 363
		private Func<string, TextObject> _getKeyTextFromKeyId;

		// Token: 0x0400016C RID: 364
		private List<Tuple<string, TextObject>> _formationNames;

		// Token: 0x0400016D RID: 365
		private PartySortControllerVM _otherPartySortController;

		// Token: 0x0400016E RID: 366
		private PartySortControllerVM _mainPartySortController;

		// Token: 0x0400016F RID: 367
		private PartyCompositionVM _otherPartyComposition;

		// Token: 0x04000170 RID: 368
		private PartyCompositionVM _mainPartyComposition;

		// Token: 0x04000171 RID: 369
		private PartyCharacterVM _currentFocusedCharacter;

		// Token: 0x04000172 RID: 370
		private UpgradeTargetVM _currentFocusedUpgrade;

		// Token: 0x04000173 RID: 371
		private HeroViewModel _selectedCharacter;

		// Token: 0x04000174 RID: 372
		private MBBindingList<PartyCharacterVM> _otherPartyTroops;

		// Token: 0x04000175 RID: 373
		private MBBindingList<PartyCharacterVM> _otherPartyPrisoners;

		// Token: 0x04000176 RID: 374
		private MBBindingList<PartyCharacterVM> _mainPartyTroops;

		// Token: 0x04000177 RID: 375
		private MBBindingList<PartyCharacterVM> _mainPartyPrisoners;

		// Token: 0x04000178 RID: 376
		private PartyUpgradeTroopVM _upgradePopUp;

		// Token: 0x04000179 RID: 377
		private PartyRecruitTroopVM _recruitPopUp;

		// Token: 0x0400017A RID: 378
		private string _titleLbl;

		// Token: 0x0400017B RID: 379
		private string _mainPartyNameLbl;

		// Token: 0x0400017C RID: 380
		private string _otherPartyNameLbl;

		// Token: 0x0400017D RID: 381
		private string _headerLbl;

		// Token: 0x0400017E RID: 382
		private string _otherPartyAccompanyingLbl;

		// Token: 0x0400017F RID: 383
		private string _talkLbl;

		// Token: 0x04000180 RID: 384
		private string _infoLbl;

		// Token: 0x04000181 RID: 385
		private string _cancelLbl;

		// Token: 0x04000182 RID: 386
		private string _doneLbl;

		// Token: 0x04000183 RID: 387
		private string _troopsLbl;

		// Token: 0x04000184 RID: 388
		private string _prisonersLabel;

		// Token: 0x04000185 RID: 389
		private string _mainPartyTotalGoldLbl;

		// Token: 0x04000186 RID: 390
		private string _mainPartyTotalMoraleLbl;

		// Token: 0x04000187 RID: 391
		private string _mainPartyTotalSpeedLbl;

		// Token: 0x04000188 RID: 392
		private string _mainPartyTotalWeeklyCostLbl;

		// Token: 0x04000189 RID: 393
		private string _currentCharacterWageLbl;

		// Token: 0x0400018A RID: 394
		private string _currentCharacterLevelLbl;

		// Token: 0x0400018B RID: 395
		private BasicTooltipViewModel _transferAllMainTroopsHint;

		// Token: 0x0400018C RID: 396
		private BasicTooltipViewModel _transferAllMainPrisonersHint;

		// Token: 0x0400018D RID: 397
		private BasicTooltipViewModel _transferAllOtherTroopsHint;

		// Token: 0x0400018E RID: 398
		private BasicTooltipViewModel _transferAllOtherPrisonersHint;

		// Token: 0x0400018F RID: 399
		private HintViewModel _moraleHint;

		// Token: 0x04000190 RID: 400
		private HintViewModel _doneHint;

		// Token: 0x04000191 RID: 401
		private BasicTooltipViewModel _speedHint;

		// Token: 0x04000192 RID: 402
		private BasicTooltipViewModel _mainPartyTroopSizeLimitHint;

		// Token: 0x04000193 RID: 403
		private BasicTooltipViewModel _mainPartyPrisonerSizeLimitHint;

		// Token: 0x04000194 RID: 404
		private BasicTooltipViewModel _otherPartyTroopSizeLimitHint;

		// Token: 0x04000195 RID: 405
		private BasicTooltipViewModel _otherPartyPrisonerSizeLimitHint;

		// Token: 0x04000196 RID: 406
		private BasicTooltipViewModel _usedHorsesHint;

		// Token: 0x04000197 RID: 407
		private HintViewModel _denarHint;

		// Token: 0x04000198 RID: 408
		private HintViewModel _totalWageHint;

		// Token: 0x04000199 RID: 409
		private HintViewModel _levelHint;

		// Token: 0x0400019A RID: 410
		private HintViewModel _wageHint;

		// Token: 0x0400019B RID: 411
		private HintViewModel _formationHint;

		// Token: 0x0400019C RID: 412
		private HintViewModel _resetHint;

		// Token: 0x0400019D RID: 413
		private StringItemWithHintVM _currentCharacterTier;

		// Token: 0x0400019E RID: 414
		private bool _isCurrentCharacterFormationEnabled;

		// Token: 0x0400019F RID: 415
		private bool _isCurrentCharacterWageEnabled;

		// Token: 0x040001A0 RID: 416
		private bool _anyPrisonersOnEitherSide;

		// Token: 0x040001A1 RID: 417
		private bool _anyMembersOnEitherSide;

		// Token: 0x040001A2 RID: 418
		private bool _canChooseRoles;

		// Token: 0x040001A3 RID: 419
		private string _otherPartyTroopsLbl;

		// Token: 0x040001A4 RID: 420
		private string _otherPartyPrisonersLbl;

		// Token: 0x040001A5 RID: 421
		private string _mainPartyTroopsLbl;

		// Token: 0x040001A6 RID: 422
		private string _mainPartyPrisonersLbl;

		// Token: 0x040001A7 RID: 423
		private string _goldChangeText;

		// Token: 0x040001A8 RID: 424
		private string _moraleChangeText;

		// Token: 0x040001A9 RID: 425
		private string _horseChangeText;

		// Token: 0x040001AA RID: 426
		private string _influenceChangeText;

		// Token: 0x040001AB RID: 427
		private bool _isMainTroopsLimitWarningEnabled;

		// Token: 0x040001AC RID: 428
		private bool _isMainPrisonersLimitWarningEnabled;

		// Token: 0x040001AD RID: 429
		private bool _isOtherTroopsLimitWarningEnabled;

		// Token: 0x040001AE RID: 430
		private bool _isOtherPrisonersLimitWarningEnabled;

		// Token: 0x040001AF RID: 431
		private bool _isMainTroopsHaveTransferableTroops;

		// Token: 0x040001B0 RID: 432
		private bool _isMainPrisonersHaveTransferableTroops;

		// Token: 0x040001B1 RID: 433
		private bool _isOtherTroopsHaveTransferableTroops;

		// Token: 0x040001B2 RID: 434
		private bool _isOtherPrisonersHaveTransferableTroops;

		// Token: 0x040001B3 RID: 435
		private bool _showQuestProgress;

		// Token: 0x040001B4 RID: 436
		private bool _isUpgradePopupButtonHighlightEnabled;

		// Token: 0x040001B5 RID: 437
		private int _questProgressRequiredCount;

		// Token: 0x040001B6 RID: 438
		private int _questProgressCurrentCount;

		// Token: 0x040001B7 RID: 439
		private int _upgradableTroopCount;

		// Token: 0x040001B8 RID: 440
		private int _recruitableTroopCount;

		// Token: 0x040001B9 RID: 441
		private bool _isDoneDisabled;

		// Token: 0x040001BA RID: 442
		private bool _isCancelDisabled;

		// Token: 0x040001BB RID: 443
		private bool _isUpgradePopUpDisabled;

		// Token: 0x040001BC RID: 444
		private bool _isRecruitPopUpDisabled;

		// Token: 0x040001BD RID: 445
		private InputKeyItemVM _resetInputKey;

		// Token: 0x040001BE RID: 446
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040001BF RID: 447
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040001C0 RID: 448
		private InputKeyItemVM _takeAllTroopsInputKey;

		// Token: 0x040001C1 RID: 449
		private InputKeyItemVM _dismissAllTroopsInputKey;

		// Token: 0x040001C2 RID: 450
		private InputKeyItemVM _takeAllPrisonersInputKey;

		// Token: 0x040001C3 RID: 451
		private InputKeyItemVM _dismissAllPrisonersInputKey;

		// Token: 0x040001C4 RID: 452
		private InputKeyItemVM _openUpgradePanelInputKey;

		// Token: 0x040001C5 RID: 453
		private InputKeyItemVM _openRecruitPanelInputKey;

		// Token: 0x040001C6 RID: 454
		private readonly string _upgradePopupButtonID = "UpgradePopupButton";

		// Token: 0x040001C7 RID: 455
		private readonly string _upgradeButtonID = "UpgradeButton";

		// Token: 0x040001C8 RID: 456
		private readonly string _recruitButtonID = "RecruitButton";

		// Token: 0x040001C9 RID: 457
		private readonly string _transferButtonOnlyOtherPrisonersID = "TransferButtonOnlyOtherPrisoners";

		// Token: 0x040001CA RID: 458
		private bool _isUpgradePopupButtonHighlightApplied;

		// Token: 0x040001CB RID: 459
		private bool _isUpgradeButtonHighlightApplied;

		// Token: 0x040001CC RID: 460
		private bool _isRecruitButtonHighlightApplied;

		// Token: 0x040001CD RID: 461
		private bool _isTransferButtonHighlightApplied;

		// Token: 0x040001CE RID: 462
		private string _latestTutorialElementID;

		// Token: 0x02000162 RID: 354
		private class TroopVMComparer : IComparer<PartyCharacterVM>
		{
			// Token: 0x06001EDD RID: 7901 RVA: 0x0006DD25 File Offset: 0x0006BF25
			public TroopVMComparer(PartyScreenLogic.TroopComparer originalTroopComparer)
			{
				this._originalTroopComparer = originalTroopComparer;
			}

			// Token: 0x06001EDE RID: 7902 RVA: 0x0006DD34 File Offset: 0x0006BF34
			public int Compare(PartyCharacterVM x, PartyCharacterVM y)
			{
				return this._originalTroopComparer.Compare(x.Troop, y.Troop);
			}

			// Token: 0x04000EC8 RID: 3784
			private readonly PartyScreenLogic.TroopComparer _originalTroopComparer;
		}
	}
}
