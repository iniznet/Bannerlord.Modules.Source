using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Recruitment
{
	// Token: 0x0200009A RID: 154
	public class RecruitmentVM : ViewModel
	{
		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06000EEE RID: 3822 RVA: 0x0003AA62 File Offset: 0x00038C62
		// (set) Token: 0x06000EEF RID: 3823 RVA: 0x0003AA6A File Offset: 0x00038C6A
		public bool IsQuitting { get; private set; }

		// Token: 0x06000EF0 RID: 3824 RVA: 0x0003AA74 File Offset: 0x00038C74
		public RecruitmentVM()
		{
			this.VolunteerList = new MBBindingList<RecruitVolunteerVM>();
			this.TroopsInCart = new MBBindingList<RecruitVolunteerTroopVM>();
			this.RefreshValues();
			if (Settlement.CurrentSettlement != null)
			{
				this.RefreshScreen();
			}
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			RecruitVolunteerTroopVM.OnFocused = (Action<RecruitVolunteerTroopVM>)Delegate.Combine(RecruitVolunteerTroopVM.OnFocused, new Action<RecruitVolunteerTroopVM>(this.OnVolunteerTroopFocusChanged));
			RecruitVolunteerOwnerVM.OnFocused = (Action<RecruitVolunteerOwnerVM>)Delegate.Combine(RecruitVolunteerOwnerVM.OnFocused, new Action<RecruitVolunteerOwnerVM>(this.OnVolunteerOwnerFocusChanged));
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x0003AB44 File Offset: 0x00038D44
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PartyWageHint = new HintViewModel(GameTexts.FindText("str_weekly_wage", null), null);
			this.TotalWealthHint = new HintViewModel(GameTexts.FindText("str_wealth", null), null);
			this.TotalCostHint = new HintViewModel(GameTexts.FindText("str_total_cost", null), null);
			this.PartyCapacityHint = new HintViewModel();
			this.PartySpeedHint = new BasicTooltipViewModel();
			this.RemainingFoodHint = new HintViewModel();
			this.DoneHint = new HintViewModel();
			this.ResetHint = new HintViewModel(GameTexts.FindText("str_reset", null), null);
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.TitleText = GameTexts.FindText("str_recruitment", null).ToString();
			this._recruitAllTextObject = GameTexts.FindText("str_recruit_all", null);
			this.ResetAllText = GameTexts.FindText("str_reset_all", null).ToString();
			this.CancelText = GameTexts.FindText("str_party_cancel", null).ToString();
			this._playerDoesntHaveEnoughMoneyStr = GameTexts.FindText("str_warning_you_dont_have_enough_money", null).ToString();
			this._playerIsOverPartyLimitStr = GameTexts.FindText("str_party_size_limit_exceeded", null).ToString();
			this.VolunteerList.ApplyActionOnAllItems(delegate(RecruitVolunteerVM x)
			{
				x.RefreshValues();
			});
			this.TroopsInCart.ApplyActionOnAllItems(delegate(RecruitVolunteerTroopVM x)
			{
				x.RefreshValues();
			});
			this.SetRecruitAllHint();
			this.UpdateRecruitAllProperties();
			if (Settlement.CurrentSettlement != null)
			{
				this.RefreshScreen();
			}
		}

		// Token: 0x06000EF2 RID: 3826 RVA: 0x0003ACE4 File Offset: 0x00038EE4
		public void RefreshScreen()
		{
			this.VolunteerList.Clear();
			this.TroopsInCart.Clear();
			int num = 0;
			this.InitialPartySize = PartyBase.MainParty.NumberOfAllMembers;
			this.RefreshPartyProperties();
			foreach (Hero hero in Settlement.CurrentSettlement.Notables)
			{
				if (hero.CanHaveRecruits)
				{
					MBTextManager.SetTextVariable("INDIVIDUAL_NAME", hero.Name, false);
					List<CharacterObject> volunteerTroopsOfHeroForRecruitment = HeroHelper.GetVolunteerTroopsOfHeroForRecruitment(hero);
					RecruitVolunteerVM recruitVolunteerVM = new RecruitVolunteerVM(hero, volunteerTroopsOfHeroForRecruitment, new Action<RecruitVolunteerVM, RecruitVolunteerTroopVM>(this.OnRecruit), new Action<RecruitVolunteerVM, RecruitVolunteerTroopVM>(this.OnRemoveFromCart));
					this.VolunteerList.Add(recruitVolunteerVM);
					num++;
				}
			}
			this.TotalWealth = Hero.MainHero.Gold;
			this.UpdateRecruitAllProperties();
		}

		// Token: 0x06000EF3 RID: 3827 RVA: 0x0003ADCC File Offset: 0x00038FCC
		private void OnRecruit(RecruitVolunteerVM recruitNotable, RecruitVolunteerTroopVM recruitTroop)
		{
			if (!recruitTroop.CanBeRecruited)
			{
				return;
			}
			recruitNotable.OnRecruitMoveToCart(recruitTroop);
			recruitTroop.CanBeRecruited = false;
			this.TroopsInCart.Add(recruitTroop);
			recruitTroop.IsInCart = true;
			CampaignEventDispatcher.Instance.OnPlayerStartRecruitment(recruitTroop.Character);
			this.RefreshPartyProperties();
		}

		// Token: 0x06000EF4 RID: 3828 RVA: 0x0003AE1C File Offset: 0x0003901C
		private void RefreshPartyProperties()
		{
			int num = this.TroopsInCart.Sum((RecruitVolunteerTroopVM t) => t.Wage);
			this.PartyWage = MobileParty.MainParty.TotalWage;
			if (num > 0)
			{
				GameTexts.SetVariable("NUM1", this.PartyWage.ToString());
				GameTexts.SetVariable("NUM2", "+" + num.ToString());
				this.PartyWageText = GameTexts.FindText("str_value_change", null).ToString();
			}
			else
			{
				this.PartyWageText = this.PartyWage.ToString();
			}
			double num2 = 0.0;
			if (this.TroopsInCart.Count > 0)
			{
				int num3 = 0;
				int num4 = 0;
				using (IEnumerator<RecruitVolunteerTroopVM> enumerator = this.TroopsInCart.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Character.IsMounted)
						{
							num4++;
						}
						else
						{
							num3++;
						}
					}
				}
				num2 = (double)(MathF.Round(Campaign.Current.Models.PartySpeedCalculatingModel.CalculateFinalSpeed(MobileParty.MainParty, Campaign.Current.Models.PartySpeedCalculatingModel.CalculateBaseSpeed(MobileParty.MainParty, false, 0, 0)).ResultNumber, 1) - MathF.Round(Campaign.Current.Models.PartySpeedCalculatingModel.CalculateFinalSpeed(MobileParty.MainParty, Campaign.Current.Models.PartySpeedCalculatingModel.CalculateBaseSpeed(MobileParty.MainParty, false, num3, num4)).ResultNumber, 1));
			}
			this.PartySpeedText = MobileParty.MainParty.Speed.ToString("0.0");
			this.PartySpeedHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartySpeedTooltip());
			if (num2 != 0.0)
			{
				GameTexts.SetVariable("NUM1", this.PartySpeedText);
				if (num2 < 0.0)
				{
					GameTexts.SetVariable("NUM2", num2.ToString("0.0"));
				}
				else
				{
					GameTexts.SetVariable("NUM2", "+" + num2.ToString("0.0"));
				}
				this.PartySpeedText = GameTexts.FindText("str_value_change", null).ToString();
			}
			int partySizeLimit = PartyBase.MainParty.PartySizeLimit;
			this.CurrentPartySize = PartyBase.MainParty.NumberOfAllMembers + this.TroopsInCart.Count;
			this.PartyCapacity = partySizeLimit;
			this.IsPartyCapacityWarningEnabled = this.CurrentPartySize > this.PartyCapacity;
			GameTexts.SetVariable("LEFT", this.CurrentPartySize.ToString());
			GameTexts.SetVariable("RIGHT", partySizeLimit.ToString());
			this.PartyCapacityText = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
			this.PartyCapacityHint.HintText = new TextObject("{=!}" + PartyBase.MainParty.PartySizeLimitExplainer.ToString(), null);
			float food = MobileParty.MainParty.Food;
			this.RemainingFoodText = MathF.Round(food, 1).ToString();
			float foodChange = MobileParty.MainParty.FoodChange;
			int totalFoodAtInventory = MobileParty.MainParty.TotalFoodAtInventory;
			int numDaysForFoodToLast = MobileParty.MainParty.GetNumDaysForFoodToLast();
			MBTextManager.SetTextVariable("DAY_NUM", numDaysForFoodToLast);
			this.RemainingFoodHint.HintText = GameTexts.FindText("str_food_consumption_tooltip", null);
			this.RemainingFoodHint.HintText.SetTextVariable("DAILY_FOOD_CONSUMPTION", foodChange);
			this.RemainingFoodHint.HintText.SetTextVariable("REMAINING_DAYS", GameTexts.FindText("str_party_food_left", null));
			this.RemainingFoodHint.HintText.SetTextVariable("TOTAL_FOOD_AMOUNT", ((double)totalFoodAtInventory + 0.01 * (double)PartyBase.MainParty.RemainingFoodPercentage).ToString("0.00"));
			this.RemainingFoodHint.HintText.SetTextVariable("TOTAL_FOOD", totalFoodAtInventory);
			int num5 = this.TroopsInCart.Sum((RecruitVolunteerTroopVM t) => t.Cost);
			this.TotalCostText = num5.ToString();
			bool flag = num5 <= Hero.MainHero.Gold;
			this.IsDoneEnabled = flag;
			this.DoneHint.HintText = new TextObject("{=!}" + this.GetDoneHint(flag), null);
			this.UpdateRecruitAllProperties();
		}

		// Token: 0x06000EF5 RID: 3829 RVA: 0x0003B2B4 File Offset: 0x000394B4
		public void ExecuteDone()
		{
			if (this.CurrentPartySize <= this.PartyCapacity)
			{
				this.OnDone();
				return;
			}
			GameTexts.SetVariable("newline", "\n");
			string text = GameTexts.FindText("str_party_over_limit_troops", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=uJro3Bua}Over Limit", null).ToString(), text, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
			{
				this.OnDone();
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000EF6 RID: 3830 RVA: 0x0003B350 File Offset: 0x00039550
		private void OnDone()
		{
			this.RefreshPartyProperties();
			int num = this.TroopsInCart.Sum((RecruitVolunteerTroopVM t) => t.Cost);
			if (num > Hero.MainHero.Gold)
			{
				Debug.FailedAssert("Execution shouldn't come here. The checks should happen before", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\GameMenu\\Recruitment\\RecruitmentVM.cs", "OnDone", 235);
				return;
			}
			foreach (RecruitVolunteerTroopVM recruitVolunteerTroopVM in this.TroopsInCart)
			{
				recruitVolunteerTroopVM.Owner.OwnerHero.VolunteerTypes[recruitVolunteerTroopVM.Index] = null;
				MobileParty.MainParty.MemberRoster.AddToCounts(recruitVolunteerTroopVM.Character, 1, false, 0, 0, true, -1);
				CampaignEventDispatcher.Instance.OnUnitRecruited(recruitVolunteerTroopVM.Character, 1);
			}
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, num, true);
			if (num > 0)
			{
				MBTextManager.SetTextVariable("GOLD_AMOUNT", MathF.Abs(num));
				InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_gold_removed_with_icon", null).ToString(), "event:/ui/notification/coins_negative"));
			}
			this.Deactivate();
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x0003B47C File Offset: 0x0003967C
		public void ExecuteForceQuit()
		{
			if (!this.IsQuitting)
			{
				this.IsQuitting = true;
				if (this.TroopsInCart.Count > 0)
				{
					InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_quit", null).ToString(), GameTexts.FindText("str_quit_question", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
					{
						this.ExecuteReset();
						this.ExecuteDone();
						this.IsQuitting = false;
					}, delegate
					{
						this.IsQuitting = false;
					}, "", 0f, null, null, null), true, false);
					return;
				}
				this.Deactivate();
			}
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x0003B524 File Offset: 0x00039724
		public void ExecuteReset()
		{
			for (int i = this.TroopsInCart.Count - 1; i >= 0; i--)
			{
				this.TroopsInCart[i].ExecuteRemoveFromCart();
			}
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x0003B55C File Offset: 0x0003975C
		public void ExecuteRecruitAll()
		{
			foreach (RecruitVolunteerVM recruitVolunteerVM in this.VolunteerList.ToList<RecruitVolunteerVM>())
			{
				foreach (RecruitVolunteerTroopVM recruitVolunteerTroopVM in recruitVolunteerVM.Troops.ToList<RecruitVolunteerTroopVM>())
				{
					recruitVolunteerTroopVM.ExecuteRecruit();
				}
			}
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x0003B5F0 File Offset: 0x000397F0
		public void Deactivate()
		{
			this.ExecuteReset();
			this.Enabled = false;
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x0003B600 File Offset: 0x00039800
		public override void OnFinalize()
		{
			base.OnFinalize();
			RecruitVolunteerTroopVM.OnFocused = (Action<RecruitVolunteerTroopVM>)Delegate.Remove(RecruitVolunteerTroopVM.OnFocused, new Action<RecruitVolunteerTroopVM>(this.OnVolunteerTroopFocusChanged));
			RecruitVolunteerOwnerVM.OnFocused = (Action<RecruitVolunteerOwnerVM>)Delegate.Remove(RecruitVolunteerOwnerVM.OnFocused, new Action<RecruitVolunteerOwnerVM>(this.OnVolunteerOwnerFocusChanged));
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.CancelInputKey.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.ResetInputKey.OnFinalize();
			this.RecruitAllInputKey.OnFinalize();
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x0003B69C File Offset: 0x0003989C
		private void OnRemoveFromCart(RecruitVolunteerVM recruitNotable, RecruitVolunteerTroopVM recruitTroop)
		{
			if (this.TroopsInCart.Any((RecruitVolunteerTroopVM r) => r == recruitTroop))
			{
				recruitNotable.OnRecruitRemovedFromCart(recruitTroop);
				recruitTroop.CanBeRecruited = true;
				recruitTroop.IsInCart = false;
				recruitTroop.IsHiglightEnabled = false;
				this.TroopsInCart.Remove(recruitTroop);
				this.RefreshPartyProperties();
			}
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x0003B717 File Offset: 0x00039917
		private static bool IsBitSet(int num, int bit)
		{
			return 1 == ((num >> bit) & 1);
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x0003B724 File Offset: 0x00039924
		private string GetDoneHint(bool doesPlayerHasEnoughMoney)
		{
			if (!doesPlayerHasEnoughMoney)
			{
				return this._playerDoesntHaveEnoughMoneyStr;
			}
			return null;
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x0003B731 File Offset: 0x00039931
		private void SetRecruitAllHint()
		{
			this.RecruitAllHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetRecruitAllKey());
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_recruit_all", null));
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		// Token: 0x06000F00 RID: 3840 RVA: 0x0003B74C File Offset: 0x0003994C
		private void UpdateRecruitAllProperties()
		{
			int numberOfAvailableRecruits = this.GetNumberOfAvailableRecruits();
			GameTexts.SetVariable("STR", numberOfAvailableRecruits);
			GameTexts.SetVariable("STR1", this._recruitAllTextObject);
			GameTexts.SetVariable("STR2", GameTexts.FindText("str_STR_in_parentheses", null));
			this.RecruitAllText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.CanRecruitAll = numberOfAvailableRecruits > 0;
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x0003B7B0 File Offset: 0x000399B0
		private int GetNumberOfAvailableRecruits()
		{
			int num = 0;
			foreach (RecruitVolunteerVM recruitVolunteerVM in this.VolunteerList)
			{
				foreach (RecruitVolunteerTroopVM recruitVolunteerTroopVM in recruitVolunteerVM.Troops)
				{
					if (!recruitVolunteerTroopVM.IsInCart && recruitVolunteerTroopVM.CanBeRecruited)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000F02 RID: 3842 RVA: 0x0003B840 File Offset: 0x00039A40
		private void OnVolunteerTroopFocusChanged(RecruitVolunteerTroopVM volunteer)
		{
			this.FocusedVolunteerTroop = volunteer;
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0003B849 File Offset: 0x00039A49
		private void OnVolunteerOwnerFocusChanged(RecruitVolunteerOwnerVM owner)
		{
			this.FocusedVolunteerOwner = owner;
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0003B854 File Offset: 0x00039A54
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null && this._isAvailableTroopsHighlightApplied)
				{
					this.SetAvailableTroopsHighlightState(false);
					this._isAvailableTroopsHighlightApplied = false;
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._latestTutorialElementID != null && !this._isAvailableTroopsHighlightApplied && this._latestTutorialElementID == "AvailableTroops")
				{
					this.SetAvailableTroopsHighlightState(true);
					this._isAvailableTroopsHighlightApplied = true;
				}
			}
		}

		// Token: 0x06000F05 RID: 3845 RVA: 0x0003B8D0 File Offset: 0x00039AD0
		private void SetAvailableTroopsHighlightState(bool state)
		{
			foreach (RecruitVolunteerVM recruitVolunteerVM in this.VolunteerList)
			{
				foreach (RecruitVolunteerTroopVM recruitVolunteerTroopVM in recruitVolunteerVM.Troops)
				{
					if (recruitVolunteerTroopVM.Wage < Hero.MainHero.Gold && recruitVolunteerTroopVM.PlayerHasEnoughRelation && !recruitVolunteerTroopVM.IsTroopEmpty)
					{
						recruitVolunteerTroopVM.IsHiglightEnabled = state;
					}
				}
			}
		}

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06000F06 RID: 3846 RVA: 0x0003B974 File Offset: 0x00039B74
		// (set) Token: 0x06000F07 RID: 3847 RVA: 0x0003B97C File Offset: 0x00039B7C
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

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06000F08 RID: 3848 RVA: 0x0003B99A File Offset: 0x00039B9A
		// (set) Token: 0x06000F09 RID: 3849 RVA: 0x0003B9A2 File Offset: 0x00039BA2
		[DataSourceProperty]
		public RecruitVolunteerTroopVM FocusedVolunteerTroop
		{
			get
			{
				return this._focusedVolunteerTroop;
			}
			set
			{
				if (value != this._focusedVolunteerTroop)
				{
					this._focusedVolunteerTroop = value;
					base.OnPropertyChangedWithValue<RecruitVolunteerTroopVM>(value, "FocusedVolunteerTroop");
				}
			}
		}

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06000F0A RID: 3850 RVA: 0x0003B9C0 File Offset: 0x00039BC0
		// (set) Token: 0x06000F0B RID: 3851 RVA: 0x0003B9C8 File Offset: 0x00039BC8
		[DataSourceProperty]
		public RecruitVolunteerOwnerVM FocusedVolunteerOwner
		{
			get
			{
				return this._focusedVolunteerOwner;
			}
			set
			{
				if (value != this._focusedVolunteerOwner)
				{
					this._focusedVolunteerOwner = value;
					base.OnPropertyChangedWithValue<RecruitVolunteerOwnerVM>(value, "FocusedVolunteerOwner");
				}
			}
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06000F0C RID: 3852 RVA: 0x0003B9E6 File Offset: 0x00039BE6
		// (set) Token: 0x06000F0D RID: 3853 RVA: 0x0003B9EE File Offset: 0x00039BEE
		[DataSourceProperty]
		public HintViewModel PartyWageHint
		{
			get
			{
				return this._partyWageHint;
			}
			set
			{
				if (value != this._partyWageHint)
				{
					this._partyWageHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PartyWageHint");
				}
			}
		}

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06000F0E RID: 3854 RVA: 0x0003BA0C File Offset: 0x00039C0C
		// (set) Token: 0x06000F0F RID: 3855 RVA: 0x0003BA14 File Offset: 0x00039C14
		[DataSourceProperty]
		public HintViewModel PartyCapacityHint
		{
			get
			{
				return this._partyCapacityHint;
			}
			set
			{
				if (value != this._partyCapacityHint)
				{
					this._partyCapacityHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PartyCapacityHint");
				}
			}
		}

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06000F10 RID: 3856 RVA: 0x0003BA32 File Offset: 0x00039C32
		// (set) Token: 0x06000F11 RID: 3857 RVA: 0x0003BA3A File Offset: 0x00039C3A
		[DataSourceProperty]
		public BasicTooltipViewModel PartySpeedHint
		{
			get
			{
				return this._partySpeedHint;
			}
			set
			{
				if (value != this._partySpeedHint)
				{
					this._partySpeedHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "PartySpeedHint");
				}
			}
		}

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06000F12 RID: 3858 RVA: 0x0003BA58 File Offset: 0x00039C58
		// (set) Token: 0x06000F13 RID: 3859 RVA: 0x0003BA60 File Offset: 0x00039C60
		[DataSourceProperty]
		public HintViewModel RemainingFoodHint
		{
			get
			{
				return this._remainingFoodHint;
			}
			set
			{
				if (value != this._remainingFoodHint)
				{
					this._remainingFoodHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RemainingFoodHint");
				}
			}
		}

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06000F14 RID: 3860 RVA: 0x0003BA7E File Offset: 0x00039C7E
		// (set) Token: 0x06000F15 RID: 3861 RVA: 0x0003BA86 File Offset: 0x00039C86
		[DataSourceProperty]
		public HintViewModel TotalWealthHint
		{
			get
			{
				return this._totalWealthHint;
			}
			set
			{
				if (value != this._totalWealthHint)
				{
					this._totalWealthHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "TotalWealthHint");
				}
			}
		}

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x0003BAA4 File Offset: 0x00039CA4
		// (set) Token: 0x06000F17 RID: 3863 RVA: 0x0003BAAC File Offset: 0x00039CAC
		[DataSourceProperty]
		public HintViewModel TotalCostHint
		{
			get
			{
				return this._totalCostHint;
			}
			set
			{
				if (value != this._totalCostHint)
				{
					this._totalCostHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "TotalCostHint");
				}
			}
		}

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06000F18 RID: 3864 RVA: 0x0003BACA File Offset: 0x00039CCA
		// (set) Token: 0x06000F19 RID: 3865 RVA: 0x0003BAD2 File Offset: 0x00039CD2
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

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06000F1A RID: 3866 RVA: 0x0003BAF0 File Offset: 0x00039CF0
		// (set) Token: 0x06000F1B RID: 3867 RVA: 0x0003BAF8 File Offset: 0x00039CF8
		[DataSourceProperty]
		public BasicTooltipViewModel RecruitAllHint
		{
			get
			{
				return this._recruitAllHint;
			}
			set
			{
				if (value != this._recruitAllHint)
				{
					this._recruitAllHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "RecruitAllHint");
				}
			}
		}

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06000F1C RID: 3868 RVA: 0x0003BB16 File Offset: 0x00039D16
		// (set) Token: 0x06000F1D RID: 3869 RVA: 0x0003BB1E File Offset: 0x00039D1E
		[DataSourceProperty]
		public int PartyWage
		{
			get
			{
				return this._partyWage;
			}
			set
			{
				if (value != this._partyWage)
				{
					this._partyWage = value;
					base.OnPropertyChangedWithValue(value, "PartyWage");
				}
			}
		}

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06000F1E RID: 3870 RVA: 0x0003BB3C File Offset: 0x00039D3C
		// (set) Token: 0x06000F1F RID: 3871 RVA: 0x0003BB44 File Offset: 0x00039D44
		[DataSourceProperty]
		public string PartyCapacityText
		{
			get
			{
				return this._partyCapacityText;
			}
			set
			{
				if (value != this._partyCapacityText)
				{
					this._partyCapacityText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartyCapacityText");
				}
			}
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06000F20 RID: 3872 RVA: 0x0003BB67 File Offset: 0x00039D67
		// (set) Token: 0x06000F21 RID: 3873 RVA: 0x0003BB6F File Offset: 0x00039D6F
		[DataSourceProperty]
		public string PartyWageText
		{
			get
			{
				return this._partyWageText;
			}
			set
			{
				if (value != this._partyWageText)
				{
					this._partyWageText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartyWageText");
				}
			}
		}

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06000F22 RID: 3874 RVA: 0x0003BB92 File Offset: 0x00039D92
		// (set) Token: 0x06000F23 RID: 3875 RVA: 0x0003BB9A File Offset: 0x00039D9A
		[DataSourceProperty]
		public string RecruitAllText
		{
			get
			{
				return this._recruitAllText;
			}
			set
			{
				if (value != this._recruitAllText)
				{
					this._recruitAllText = value;
					base.OnPropertyChangedWithValue<string>(value, "RecruitAllText");
				}
			}
		}

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06000F24 RID: 3876 RVA: 0x0003BBBD File Offset: 0x00039DBD
		// (set) Token: 0x06000F25 RID: 3877 RVA: 0x0003BBC5 File Offset: 0x00039DC5
		[DataSourceProperty]
		public string PartySpeedText
		{
			get
			{
				return this._partySpeedText;
			}
			set
			{
				if (value != this._partySpeedText)
				{
					this._partySpeedText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartySpeedText");
				}
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06000F26 RID: 3878 RVA: 0x0003BBE8 File Offset: 0x00039DE8
		// (set) Token: 0x06000F27 RID: 3879 RVA: 0x0003BBF0 File Offset: 0x00039DF0
		[DataSourceProperty]
		public string ResetAllText
		{
			get
			{
				return this._resetAllText;
			}
			set
			{
				if (value != this._resetAllText)
				{
					this._resetAllText = value;
					base.OnPropertyChangedWithValue<string>(value, "ResetAllText");
				}
			}
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06000F28 RID: 3880 RVA: 0x0003BC13 File Offset: 0x00039E13
		// (set) Token: 0x06000F29 RID: 3881 RVA: 0x0003BC1B File Offset: 0x00039E1B
		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06000F2A RID: 3882 RVA: 0x0003BC3E File Offset: 0x00039E3E
		// (set) Token: 0x06000F2B RID: 3883 RVA: 0x0003BC46 File Offset: 0x00039E46
		[DataSourceProperty]
		public string RemainingFoodText
		{
			get
			{
				return this._remainingFoodText;
			}
			set
			{
				if (value != this._remainingFoodText)
				{
					this._remainingFoodText = value;
					base.OnPropertyChangedWithValue<string>(value, "RemainingFoodText");
				}
			}
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06000F2C RID: 3884 RVA: 0x0003BC69 File Offset: 0x00039E69
		// (set) Token: 0x06000F2D RID: 3885 RVA: 0x0003BC71 File Offset: 0x00039E71
		[DataSourceProperty]
		public string TotalCostText
		{
			get
			{
				return this._totalCostText;
			}
			set
			{
				if (value != this._totalCostText)
				{
					this._totalCostText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalCostText");
				}
			}
		}

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06000F2E RID: 3886 RVA: 0x0003BC94 File Offset: 0x00039E94
		// (set) Token: 0x06000F2F RID: 3887 RVA: 0x0003BC9C File Offset: 0x00039E9C
		[DataSourceProperty]
		public bool Enabled
		{
			get
			{
				return this._enabled;
			}
			set
			{
				if (value != this._enabled)
				{
					this._enabled = value;
					base.OnPropertyChangedWithValue(value, "Enabled");
				}
			}
		}

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06000F30 RID: 3888 RVA: 0x0003BCBC File Offset: 0x00039EBC
		// (set) Token: 0x06000F31 RID: 3889 RVA: 0x0003BCC4 File Offset: 0x00039EC4
		[DataSourceProperty]
		public bool IsDoneEnabled
		{
			get
			{
				return this._isDoneEnabled;
			}
			set
			{
				if (value != this._isDoneEnabled)
				{
					this._isDoneEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsDoneEnabled");
				}
			}
		}

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06000F32 RID: 3890 RVA: 0x0003BCE2 File Offset: 0x00039EE2
		// (set) Token: 0x06000F33 RID: 3891 RVA: 0x0003BCEA File Offset: 0x00039EEA
		[DataSourceProperty]
		public bool IsPartyCapacityWarningEnabled
		{
			get
			{
				return this._isPartyCapacityWarningEnabled;
			}
			set
			{
				if (value != this._isPartyCapacityWarningEnabled)
				{
					this._isPartyCapacityWarningEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsPartyCapacityWarningEnabled");
				}
			}
		}

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06000F34 RID: 3892 RVA: 0x0003BD08 File Offset: 0x00039F08
		// (set) Token: 0x06000F35 RID: 3893 RVA: 0x0003BD10 File Offset: 0x00039F10
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06000F36 RID: 3894 RVA: 0x0003BD33 File Offset: 0x00039F33
		// (set) Token: 0x06000F37 RID: 3895 RVA: 0x0003BD3B File Offset: 0x00039F3B
		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06000F38 RID: 3896 RVA: 0x0003BD5E File Offset: 0x00039F5E
		// (set) Token: 0x06000F39 RID: 3897 RVA: 0x0003BD66 File Offset: 0x00039F66
		[DataSourceProperty]
		public bool CanRecruitAll
		{
			get
			{
				return this._canRecruitAll;
			}
			set
			{
				if (value != this._canRecruitAll)
				{
					this._canRecruitAll = value;
					base.OnPropertyChangedWithValue(value, "CanRecruitAll");
				}
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06000F3A RID: 3898 RVA: 0x0003BD84 File Offset: 0x00039F84
		// (set) Token: 0x06000F3B RID: 3899 RVA: 0x0003BD8C File Offset: 0x00039F8C
		[DataSourceProperty]
		public int TotalWealth
		{
			get
			{
				return this._totalWealth;
			}
			set
			{
				if (value != this._totalWealth)
				{
					this._totalWealth = value;
					base.OnPropertyChangedWithValue(value, "TotalWealth");
				}
			}
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06000F3C RID: 3900 RVA: 0x0003BDAA File Offset: 0x00039FAA
		// (set) Token: 0x06000F3D RID: 3901 RVA: 0x0003BDB2 File Offset: 0x00039FB2
		[DataSourceProperty]
		public int PartyCapacity
		{
			get
			{
				return this._partyCapacity;
			}
			set
			{
				if (value != this._partyCapacity)
				{
					this._partyCapacity = value;
					base.OnPropertyChangedWithValue(value, "PartyCapacity");
				}
			}
		}

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06000F3E RID: 3902 RVA: 0x0003BDD0 File Offset: 0x00039FD0
		// (set) Token: 0x06000F3F RID: 3903 RVA: 0x0003BDD8 File Offset: 0x00039FD8
		[DataSourceProperty]
		public int InitialPartySize
		{
			get
			{
				return this._initialPartySize;
			}
			set
			{
				if (value != this._initialPartySize)
				{
					this._initialPartySize = value;
					base.OnPropertyChangedWithValue(value, "InitialPartySize");
				}
			}
		}

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06000F40 RID: 3904 RVA: 0x0003BDF6 File Offset: 0x00039FF6
		// (set) Token: 0x06000F41 RID: 3905 RVA: 0x0003BDFE File Offset: 0x00039FFE
		[DataSourceProperty]
		public int CurrentPartySize
		{
			get
			{
				return this._currentPartySize;
			}
			set
			{
				if (value != this._currentPartySize)
				{
					this._currentPartySize = value;
					base.OnPropertyChangedWithValue(value, "CurrentPartySize");
				}
			}
		}

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06000F42 RID: 3906 RVA: 0x0003BE1C File Offset: 0x0003A01C
		// (set) Token: 0x06000F43 RID: 3907 RVA: 0x0003BE24 File Offset: 0x0003A024
		[DataSourceProperty]
		public MBBindingList<RecruitVolunteerVM> VolunteerList
		{
			get
			{
				return this._volunteerList;
			}
			set
			{
				if (value != this._volunteerList)
				{
					this._volunteerList = value;
					base.OnPropertyChangedWithValue<MBBindingList<RecruitVolunteerVM>>(value, "VolunteerList");
				}
			}
		}

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06000F44 RID: 3908 RVA: 0x0003BE42 File Offset: 0x0003A042
		// (set) Token: 0x06000F45 RID: 3909 RVA: 0x0003BE4A File Offset: 0x0003A04A
		[DataSourceProperty]
		public MBBindingList<RecruitVolunteerTroopVM> TroopsInCart
		{
			get
			{
				return this._troopsInCart;
			}
			set
			{
				if (value != this._troopsInCart)
				{
					this._troopsInCart = value;
					base.OnPropertyChangedWithValue<MBBindingList<RecruitVolunteerTroopVM>>(value, "TroopsInCart");
				}
			}
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x0003BE68 File Offset: 0x0003A068
		public void SetGetKeyTextFromKeyIDFunc(Func<string, TextObject> getKeyTextFromKeyId)
		{
			this._getKeyTextFromKeyId = getKeyTextFromKeyId;
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x0003BE71 File Offset: 0x0003A071
		private string GetRecruitAllKey()
		{
			if (this.RecruitAllInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return string.Empty;
			}
			return this._getKeyTextFromKeyId(this.RecruitAllInputKey.KeyID).ToString();
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x0003BEA4 File Offset: 0x0003A0A4
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x0003BEB3 File Offset: 0x0003A0B3
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x0003BEC2 File Offset: 0x0003A0C2
		public void SetRecruitAllInputKey(HotKey hotKey)
		{
			this.RecruitAllInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetRecruitAllHint();
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x0003BED7 File Offset: 0x0003A0D7
		public void SetResetInputKey(HotKey hotKey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06000F4C RID: 3916 RVA: 0x0003BEE6 File Offset: 0x0003A0E6
		// (set) Token: 0x06000F4D RID: 3917 RVA: 0x0003BEEE File Offset: 0x0003A0EE
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

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06000F4E RID: 3918 RVA: 0x0003BF0C File Offset: 0x0003A10C
		// (set) Token: 0x06000F4F RID: 3919 RVA: 0x0003BF14 File Offset: 0x0003A114
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

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06000F50 RID: 3920 RVA: 0x0003BF32 File Offset: 0x0003A132
		// (set) Token: 0x06000F51 RID: 3921 RVA: 0x0003BF3A File Offset: 0x0003A13A
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

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06000F52 RID: 3922 RVA: 0x0003BF58 File Offset: 0x0003A158
		// (set) Token: 0x06000F53 RID: 3923 RVA: 0x0003BF60 File Offset: 0x0003A160
		[DataSourceProperty]
		public InputKeyItemVM RecruitAllInputKey
		{
			get
			{
				return this._recruitAllInputKey;
			}
			set
			{
				if (value != this._recruitAllInputKey)
				{
					this._recruitAllInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RecruitAllInputKey");
				}
			}
		}

		// Token: 0x040006EF RID: 1775
		private TextObject _recruitAllTextObject;

		// Token: 0x040006F0 RID: 1776
		private string _playerDoesntHaveEnoughMoneyStr;

		// Token: 0x040006F1 RID: 1777
		private string _playerIsOverPartyLimitStr;

		// Token: 0x040006F2 RID: 1778
		private Func<string, TextObject> _getKeyTextFromKeyId;

		// Token: 0x040006F3 RID: 1779
		private bool _isAvailableTroopsHighlightApplied;

		// Token: 0x040006F4 RID: 1780
		private string _latestTutorialElementID;

		// Token: 0x040006F5 RID: 1781
		private bool _enabled;

		// Token: 0x040006F6 RID: 1782
		private bool _isDoneEnabled;

		// Token: 0x040006F7 RID: 1783
		private bool _isPartyCapacityWarningEnabled;

		// Token: 0x040006F8 RID: 1784
		private bool _canRecruitAll;

		// Token: 0x040006F9 RID: 1785
		private string _titleText;

		// Token: 0x040006FA RID: 1786
		private string _doneText;

		// Token: 0x040006FB RID: 1787
		private string _recruitAllText;

		// Token: 0x040006FC RID: 1788
		private string _resetAllText;

		// Token: 0x040006FD RID: 1789
		private string _cancelText;

		// Token: 0x040006FE RID: 1790
		private int _totalWealth;

		// Token: 0x040006FF RID: 1791
		private int _partyCapacity;

		// Token: 0x04000700 RID: 1792
		private int _initialPartySize;

		// Token: 0x04000701 RID: 1793
		private int _currentPartySize;

		// Token: 0x04000702 RID: 1794
		private MBBindingList<RecruitVolunteerVM> _volunteerList;

		// Token: 0x04000703 RID: 1795
		private MBBindingList<RecruitVolunteerTroopVM> _troopsInCart;

		// Token: 0x04000704 RID: 1796
		private int _partyWage;

		// Token: 0x04000705 RID: 1797
		private string _partyCapacityText = "";

		// Token: 0x04000706 RID: 1798
		private string _partyWageText = "";

		// Token: 0x04000707 RID: 1799
		private string _partySpeedText = "";

		// Token: 0x04000708 RID: 1800
		private string _remainingFoodText = "";

		// Token: 0x04000709 RID: 1801
		private string _totalCostText = "";

		// Token: 0x0400070A RID: 1802
		private RecruitVolunteerTroopVM _focusedVolunteerTroop;

		// Token: 0x0400070B RID: 1803
		private RecruitVolunteerOwnerVM _focusedVolunteerOwner;

		// Token: 0x0400070C RID: 1804
		private HintViewModel _partyWageHint;

		// Token: 0x0400070D RID: 1805
		private HintViewModel _partyCapacityHint;

		// Token: 0x0400070E RID: 1806
		private BasicTooltipViewModel _partySpeedHint;

		// Token: 0x0400070F RID: 1807
		private HintViewModel _remainingFoodHint;

		// Token: 0x04000710 RID: 1808
		private HintViewModel _totalWealthHint;

		// Token: 0x04000711 RID: 1809
		private HintViewModel _totalCostHint;

		// Token: 0x04000712 RID: 1810
		private HintViewModel _resetHint;

		// Token: 0x04000713 RID: 1811
		private HintViewModel _doneHint;

		// Token: 0x04000714 RID: 1812
		private BasicTooltipViewModel _recruitAllHint;

		// Token: 0x04000715 RID: 1813
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000716 RID: 1814
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000717 RID: 1815
		private InputKeyItemVM _resetInputKey;

		// Token: 0x04000718 RID: 1816
		private InputKeyItemVM _recruitAllInputKey;
	}
}
