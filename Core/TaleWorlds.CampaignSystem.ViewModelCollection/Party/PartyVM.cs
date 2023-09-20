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
	public class PartyVM : ViewModel, IPartyScreenLogicHandler, PartyScreenPrisonHandler, IPartyScreenTroopHandler
	{
		public PartyScreenLogic PartyScreenLogic { get; private set; }

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

		public void SetFiveStackShortcutKeyText(string text)
		{
			PartyCharacterVM.FiveStackShortcutKeyText = text;
		}

		public void SetEntireStackShortcutKeyText(string text)
		{
			PartyCharacterVM.EntireStackShortcutKeyText = text;
		}

		private void OnPartyGoldChanged()
		{
			MBTextManager.SetTextVariable("PAY_OR_GET", (this.PartyScreenLogic.CurrentData.PartyGoldChangeAmount > 0) ? 1 : 0);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(this.PartyScreenLogic.CurrentData.PartyGoldChangeAmount));
			this.GoldChangeText = ((this.PartyScreenLogic.CurrentData.PartyGoldChangeAmount == 0) ? "" : GameTexts.FindText("str_inventory_trade_label", null).ToString());
		}

		private void OnPartyMoraleChanged()
		{
			MBTextManager.SetTextVariable("PAY_OR_GET", (this.PartyScreenLogic.CurrentData.PartyMoraleChangeAmount > 0) ? 1 : 0);
			MBTextManager.SetTextVariable("MORALE_ICON", "{=!}<img src=\"General\\Icons\\Morale@2x\" extend=\"8\">", false);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(this.PartyScreenLogic.CurrentData.PartyMoraleChangeAmount));
			this.MoraleChangeText = ((this.PartyScreenLogic.CurrentData.PartyMoraleChangeAmount == 0) ? "" : GameTexts.FindText("str_party_morale_label", null).ToString());
		}

		private void OnPartyInfluenceChanged()
		{
			int num = this.PartyScreenLogic.CurrentData.PartyInfluenceChangeAmount.Item1 + this.PartyScreenLogic.CurrentData.PartyInfluenceChangeAmount.Item2 + this.PartyScreenLogic.CurrentData.PartyInfluenceChangeAmount.Item3;
			MBTextManager.SetTextVariable("PAY_OR_GET", (num > 0) ? 1 : 0);
			MBTextManager.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">", false);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(num));
			this.InfluenceChangeText = ((num == 0) ? "" : GameTexts.FindText("str_party_influence_label", null).ToString());
		}

		private void OnPartyHorseChanged()
		{
			MBTextManager.SetTextVariable("IS_PLURAL", (this.PartyScreenLogic.CurrentData.PartyHorseChangeAmount > 1) ? 1 : 0);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(this.PartyScreenLogic.CurrentData.PartyHorseChangeAmount));
			this.HorseChangeText = ((this.PartyScreenLogic.CurrentData.PartyHorseChangeAmount == 0) ? "" : GameTexts.FindText("str_party_horse_label", null).ToString());
		}

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

		private void RefreshTopInformation()
		{
			this.MainPartyTotalWeeklyCostLbl = MobileParty.MainParty.TotalWage.ToString();
			this.MainPartyTotalGoldLbl = Hero.MainHero.Gold.ToString();
			this.MainPartyTotalMoraleLbl = ((int)MobileParty.MainParty.Morale).ToString("##.0");
			this.MainPartyTotalSpeedLbl = CampaignUIHelper.FloatToString(MobileParty.MainParty.Speed);
			this.UpdateLabelHints();
		}

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

		public void SetSelectedCharacter(PartyCharacterVM troop)
		{
			this.CurrentCharacter = troop;
			this.CurrentCharacter.UpdateRecruitable();
		}

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

		private void SaveSortState()
		{
			this._viewDataTracker.SetPartySortType((int)this.PartyScreenLogic.ActiveMainPartySortType);
			this._viewDataTracker.SetIsPartySortAscending(this.PartyScreenLogic.IsMainPartySortAscending);
		}

		private void SaveCharacterLockStates()
		{
			this._viewDataTracker.SetPartyTroopLocks(this._lockedTroopIDs);
			this._viewDataTracker.SetPartyPrisonerLocks(this._lockedPrisonerIDs);
		}

		private bool IsTroopLocked(TroopRosterElement troop, bool isPrisoner)
		{
			if (!isPrisoner)
			{
				return this._lockedTroopIDs.Contains(troop.Character.StringId);
			}
			return this._lockedPrisonerIDs.Contains(troop.Character.StringId);
		}

		private void UpdateCurrentCharacterFormationClass(SelectorVM<SelectorItemVM> s)
		{
			Campaign.Current.SetPlayerFormationPreference(this.CurrentCharacter.Character, (FormationClass)s.SelectedIndex);
		}

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

		private void OnFocusCharacter(PartyCharacterVM character)
		{
			this.CurrentFocusedCharacter = character;
		}

		private int GetNumberOfWoundedTroopNumberForSide(CharacterObject character, PartyScreenLogic.PartyRosterSide fromSide, bool isPrisoner)
		{
			return this.FindCharacterVM(character, fromSide, isPrisoner).WoundedCount;
		}

		private int GetNumberOfHealthyTroopNumberForSide(CharacterObject character, PartyScreenLogic.PartyRosterSide fromSide, bool isPrisoner)
		{
			PartyCharacterVM partyCharacterVM = this.FindCharacterVM(character, fromSide, isPrisoner);
			return partyCharacterVM.Troop.Number - partyCharacterVM.Troop.WoundedNumber;
		}

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

		public void OnUpgradePopUpClosed(bool isCancelled)
		{
			if (!isCancelled)
			{
				this.UpdateTroopManagerPopUpCounts();
			}
			Game.Current.EventManager.TriggerEvent<PlayerToggledUpgradePopupEvent>(new PlayerToggledUpgradePopupEvent(false));
		}

		public void OnRecruitPopUpClosed(bool isCancelled)
		{
			if (!isCancelled)
			{
				this.UpdateTroopManagerPopUpCounts();
			}
		}

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

		public void ExecuteTransferAllMainTroops()
		{
			this.TransferAllCharacters(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Member);
			this.ExecuteRemoveZeroCounts();
		}

		public void ExecuteTransferAllOtherTroops()
		{
			this.TransferAllCharacters(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Member);
			this.ExecuteRemoveZeroCounts();
		}

		public void ExecuteTransferAllMainPrisoners()
		{
			this.TransferAllCharacters(PartyScreenLogic.PartyRosterSide.Right, PartyScreenLogic.TroopType.Prisoner);
			this.ExecuteRemoveZeroCounts();
		}

		public void ExecuteTransferAllOtherPrisoners()
		{
			this.TransferAllCharacters(PartyScreenLogic.PartyRosterSide.Left, PartyScreenLogic.TroopType.Prisoner);
			this.ExecuteRemoveZeroCounts();
		}

		public void ExecuteOpenUpgradePopUp()
		{
			this.UpgradePopUp.OpenPopUp();
			Game.Current.EventManager.TriggerEvent<PlayerToggledUpgradePopupEvent>(new PlayerToggledUpgradePopupEvent(true));
		}

		public void ExecuteOpenRecruitPopUp()
		{
			this.RecruitPopUp.OpenPopUp();
		}

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

		public void ExecuteExecution()
		{
			if (this.PartyScreenLogic.IsExecutable(this.CurrentCharacter.Type, this.CurrentCharacter.Character, this.CurrentCharacter.Side))
			{
				PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
				partyCommand.FillForExecuteTroop(this.CurrentCharacter.Side, this.CurrentCharacter.Type, this.CurrentCharacter.Character);
				this.PartyScreenLogic.AddCommand(partyCommand);
			}
		}

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

		private void TransferAllCharacters(PartyScreenLogic.PartyRosterSide rosterSide, PartyScreenLogic.TroopType type)
		{
			PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
			partyCommand.FillForTransferAllTroops(rosterSide, type);
			this.PartyScreenLogic.AddCommand(partyCommand);
		}

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

		private void RefreshPrisonersRecruitable()
		{
			foreach (PartyCharacterVM partyCharacterVM in this.MainPartyPrisoners)
			{
				partyCharacterVM.UpdateRecruitable();
			}
		}

		private void RefreshTroopsUpgradeable()
		{
			foreach (PartyCharacterVM partyCharacterVM in this.MainPartyTroops)
			{
				partyCharacterVM.InitializeUpgrades();
			}
		}

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

		private void CloseScreenInternal()
		{
			this.SaveSortState();
			this.SaveCharacterLockStates();
			PartyScreenManager.CloseScreen(false, false);
		}

		public void ExecuteReset()
		{
			this.PartyScreenLogic.Reset(false);
			this.CurrentFocusedCharacter = null;
			this.CurrentFocusedUpgrade = null;
		}

		public void ExecuteCancel()
		{
			if (this.PartyScreenLogic.IsCancelActive())
			{
				this.PartyScreenLogic.Reset(true);
				PartyScreenManager.CloseScreen(false, true);
			}
		}

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

		void PartyScreenPrisonHandler.ExecuteTakeAllPrisonersScript()
		{
			this.ExecuteTransferAllOtherPrisoners();
		}

		void PartyScreenPrisonHandler.ExecuteDoneScript()
		{
			this.ExecuteDone();
		}

		void PartyScreenPrisonHandler.ExecuteResetScript()
		{
			this.ExecuteReset();
		}

		void PartyScreenPrisonHandler.ExecuteSellAllPrisoners()
		{
			this.ExecuteTransferAllMainPrisoners();
		}

		void IPartyScreenTroopHandler.PartyTroopTransfer()
		{
			this.ExecuteTransferAllMainTroops();
		}

		void IPartyScreenTroopHandler.ExecuteDoneScript()
		{
			this.ExecuteDone();
		}

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

		public void RequestUserInput(string text, Action accept, Action cancel)
		{
		}

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

		private TextObject GetTransferAllOtherTroopsKeyText()
		{
			if (this.TakeAllTroopsInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.TakeAllTroopsInputKey.KeyID);
		}

		private TextObject GetTransferAllMainTroopsKeyText()
		{
			if (this.DismissAllTroopsInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.DismissAllTroopsInputKey.KeyID);
		}

		private TextObject GetTransferAllOtherPrisonersKeyText()
		{
			if (this.TakeAllPrisonersInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.TakeAllPrisonersInputKey.KeyID);
		}

		private TextObject GetTransferAllMainPrisonersKeyText()
		{
			if (this.DismissAllPrisonersInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.DismissAllPrisonersInputKey.KeyID);
		}

		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.UpgradePopUp.SetDoneInputKey(hotKey);
			this.RecruitPopUp.SetDoneInputKey(hotKey);
		}

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

		public void SetOpenUpgradePanelInputKey(HotKey hotKey)
		{
			this.OpenUpgradePanelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetOpenRecruitPanelInputKey(HotKey hotKey)
		{
			this.OpenRecruitPanelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetGetKeyTextFromKeyIDFunc(Func<string, TextObject> getKeyTextFromKeyId)
		{
			this._getKeyTextFromKeyId = getKeyTextFromKeyId;
		}

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

		private void SetRecruitButtonsHighlightState(bool state)
		{
			foreach (PartyCharacterVM partyCharacterVM in this.MainPartyTroops)
			{
				partyCharacterVM.IsRecruitButtonsHiglighted = state;
			}
		}

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

		private readonly PartyScreenMode _currentMode;

		private readonly IViewDataTracker _viewDataTracker;

		public bool IsFiveStackModifierActive;

		public bool IsEntireStackModifierActive;

		private PartyCharacterVM _currentCharacter;

		private List<string> _lockedTroopIDs;

		private List<string> _lockedPrisonerIDs;

		private Func<string, TextObject> _getKeyTextFromKeyId;

		private List<Tuple<string, TextObject>> _formationNames;

		private PartySortControllerVM _otherPartySortController;

		private PartySortControllerVM _mainPartySortController;

		private PartyCompositionVM _otherPartyComposition;

		private PartyCompositionVM _mainPartyComposition;

		private PartyCharacterVM _currentFocusedCharacter;

		private UpgradeTargetVM _currentFocusedUpgrade;

		private HeroViewModel _selectedCharacter;

		private MBBindingList<PartyCharacterVM> _otherPartyTroops;

		private MBBindingList<PartyCharacterVM> _otherPartyPrisoners;

		private MBBindingList<PartyCharacterVM> _mainPartyTroops;

		private MBBindingList<PartyCharacterVM> _mainPartyPrisoners;

		private PartyUpgradeTroopVM _upgradePopUp;

		private PartyRecruitTroopVM _recruitPopUp;

		private string _titleLbl;

		private string _mainPartyNameLbl;

		private string _otherPartyNameLbl;

		private string _headerLbl;

		private string _otherPartyAccompanyingLbl;

		private string _talkLbl;

		private string _infoLbl;

		private string _cancelLbl;

		private string _doneLbl;

		private string _troopsLbl;

		private string _prisonersLabel;

		private string _mainPartyTotalGoldLbl;

		private string _mainPartyTotalMoraleLbl;

		private string _mainPartyTotalSpeedLbl;

		private string _mainPartyTotalWeeklyCostLbl;

		private string _currentCharacterWageLbl;

		private string _currentCharacterLevelLbl;

		private BasicTooltipViewModel _transferAllMainTroopsHint;

		private BasicTooltipViewModel _transferAllMainPrisonersHint;

		private BasicTooltipViewModel _transferAllOtherTroopsHint;

		private BasicTooltipViewModel _transferAllOtherPrisonersHint;

		private HintViewModel _moraleHint;

		private HintViewModel _doneHint;

		private BasicTooltipViewModel _speedHint;

		private BasicTooltipViewModel _mainPartyTroopSizeLimitHint;

		private BasicTooltipViewModel _mainPartyPrisonerSizeLimitHint;

		private BasicTooltipViewModel _otherPartyTroopSizeLimitHint;

		private BasicTooltipViewModel _otherPartyPrisonerSizeLimitHint;

		private BasicTooltipViewModel _usedHorsesHint;

		private HintViewModel _denarHint;

		private HintViewModel _totalWageHint;

		private HintViewModel _levelHint;

		private HintViewModel _wageHint;

		private HintViewModel _formationHint;

		private HintViewModel _resetHint;

		private StringItemWithHintVM _currentCharacterTier;

		private bool _isCurrentCharacterFormationEnabled;

		private bool _isCurrentCharacterWageEnabled;

		private bool _anyPrisonersOnEitherSide;

		private bool _anyMembersOnEitherSide;

		private bool _canChooseRoles;

		private string _otherPartyTroopsLbl;

		private string _otherPartyPrisonersLbl;

		private string _mainPartyTroopsLbl;

		private string _mainPartyPrisonersLbl;

		private string _goldChangeText;

		private string _moraleChangeText;

		private string _horseChangeText;

		private string _influenceChangeText;

		private bool _isMainTroopsLimitWarningEnabled;

		private bool _isMainPrisonersLimitWarningEnabled;

		private bool _isOtherTroopsLimitWarningEnabled;

		private bool _isOtherPrisonersLimitWarningEnabled;

		private bool _isMainTroopsHaveTransferableTroops;

		private bool _isMainPrisonersHaveTransferableTroops;

		private bool _isOtherTroopsHaveTransferableTroops;

		private bool _isOtherPrisonersHaveTransferableTroops;

		private bool _showQuestProgress;

		private bool _isUpgradePopupButtonHighlightEnabled;

		private int _questProgressRequiredCount;

		private int _questProgressCurrentCount;

		private int _upgradableTroopCount;

		private int _recruitableTroopCount;

		private bool _isDoneDisabled;

		private bool _isCancelDisabled;

		private bool _isUpgradePopUpDisabled;

		private bool _isRecruitPopUpDisabled;

		private InputKeyItemVM _resetInputKey;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _takeAllTroopsInputKey;

		private InputKeyItemVM _dismissAllTroopsInputKey;

		private InputKeyItemVM _takeAllPrisonersInputKey;

		private InputKeyItemVM _dismissAllPrisonersInputKey;

		private InputKeyItemVM _openUpgradePanelInputKey;

		private InputKeyItemVM _openRecruitPanelInputKey;

		private readonly string _upgradePopupButtonID = "UpgradePopupButton";

		private readonly string _upgradeButtonID = "UpgradeButton";

		private readonly string _recruitButtonID = "RecruitButton";

		private readonly string _transferButtonOnlyOtherPrisonersID = "TransferButtonOnlyOtherPrisoners";

		private bool _isUpgradePopupButtonHighlightApplied;

		private bool _isUpgradeButtonHighlightApplied;

		private bool _isRecruitButtonHighlightApplied;

		private bool _isTransferButtonHighlightApplied;

		private string _latestTutorialElementID;

		private class TroopVMComparer : IComparer<PartyCharacterVM>
		{
			public TroopVMComparer(PartyScreenLogic.TroopComparer originalTroopComparer)
			{
				this._originalTroopComparer = originalTroopComparer;
			}

			public int Compare(PartyCharacterVM x, PartyCharacterVM y)
			{
				return this._originalTroopComparer.Compare(x.Troop, y.Troop);
			}

			private readonly PartyScreenLogic.TroopComparer _originalTroopComparer;
		}
	}
}
