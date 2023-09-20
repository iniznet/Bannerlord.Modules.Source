using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement
{
	// Token: 0x02000134 RID: 308
	public class ArmyManagementVM : ViewModel
	{
		// Token: 0x06001DB5 RID: 7605 RVA: 0x00069D44 File Offset: 0x00067F44
		public ArmyManagementVM(Action onClose)
		{
			this._onClose = onClose;
			this._itemComparer = new ArmyManagementVM.ManagementItemComparer();
			this.PartyList = new MBBindingList<ArmyManagementItemVM>();
			this.PartiesInCart = new MBBindingList<ArmyManagementItemVM>();
			this._partiesToRemove = new MBBindingList<ArmyManagementItemVM>();
			this._currentParties = new List<MobileParty>();
			this.CohesionHint = new BasicTooltipViewModel();
			this.FoodHint = new HintViewModel();
			this.MoraleHint = new HintViewModel();
			this.BoostCohesionHint = new HintViewModel();
			this.DisbandArmyHint = new HintViewModel();
			this.DoneHint = new HintViewModel();
			this.TutorialNotification = new ElementNotificationVM();
			this.CanAffordInfluenceCost = true;
			this.PlayerHasArmy = MobileParty.MainParty.Army != null;
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				if (mobileParty.LeaderHero != null && mobileParty.MapFaction == Hero.MainHero.MapFaction && mobileParty.LeaderHero != Hero.MainHero && !mobileParty.IsCaravan)
				{
					this.PartyList.Add(new ArmyManagementItemVM(new Action<ArmyManagementItemVM>(this.OnAddToCart), new Action<ArmyManagementItemVM>(this.OnRemove), new Action<ArmyManagementItemVM>(this.OnFocus), mobileParty));
				}
			}
			this._mainPartyItem = new ArmyManagementItemVM(null, null, null, Hero.MainHero.PartyBelongedTo)
			{
				IsAlreadyWithPlayer = true,
				IsMainHero = true,
				IsInCart = true
			};
			this.PartiesInCart.Add(this._mainPartyItem);
			foreach (ArmyManagementItemVM armyManagementItemVM in this.PartyList)
			{
				if (MobileParty.MainParty.Army != null && armyManagementItemVM.Party.Army == MobileParty.MainParty.Army && armyManagementItemVM.Party != MobileParty.MainParty)
				{
					armyManagementItemVM.Cost = 0;
					armyManagementItemVM.IsAlreadyWithPlayer = true;
					armyManagementItemVM.IsInCart = true;
					this.PartiesInCart.Add(armyManagementItemVM);
				}
			}
			this.CalculateCohesion();
			this.CanBoostCohesion = this.PlayerHasArmy && this.NewCohesion < 100;
			if (MobileParty.MainParty.Army != null)
			{
				this.CohesionBoostCost = Campaign.Current.Models.ArmyManagementCalculationModel.GetCohesionBoostInfluenceCost(MobileParty.MainParty.Army, 10);
			}
			this._initialInfluence = Hero.MainHero.Clan.Influence;
			this.OnRefresh();
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(TutorialContexts.ArmyManagement));
			this.SortControllerVM = new ArmyManagementSortControllerVM(ref this._partyList);
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.RefreshValues();
		}

		// Token: 0x06001DB6 RID: 7606 RVA: 0x0006A018 File Offset: 0x00068218
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = GameTexts.FindText("str_army_management", null).ToString();
			this.BoostTitleText = GameTexts.FindText("str_boost_cohesion", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.DistanceText = GameTexts.FindText("str_distance", null).ToString();
			this.CostText = GameTexts.FindText("str_cost", null).ToString();
			this.StrengthText = GameTexts.FindText("str_men_numbersign", null).ToString();
			this.LordsText = GameTexts.FindText("str_leader", null).ToString();
			this.ClanText = GameTexts.FindText("str_clans", null).ToString();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.OwnerText = GameTexts.FindText("str_party", null).ToString();
			this.DisbandArmyText = GameTexts.FindText("str_disband_army", null).ToString();
			this._playerDoesntHaveEnoughInfluenceStr = GameTexts.FindText("str_warning_you_dont_have_enough_influence", null).ToString();
			GameTexts.SetVariable("TOTAL_INFLUENCE", MathF.Round(Hero.MainHero.Clan.Influence));
			this.TotalInfluence = GameTexts.FindText("str_total_influence", null).ToString();
			GameTexts.SetVariable("NUMBER", 10);
			this.CohesionBoostAmountText = GameTexts.FindText("str_plus_with_number", null).ToString();
			this.PartyList.ApplyActionOnAllItems(delegate(ArmyManagementItemVM x)
			{
				x.RefreshValues();
			});
			this.PartiesInCart.ApplyActionOnAllItems(delegate(ArmyManagementItemVM x)
			{
				x.RefreshValues();
			});
			this.TutorialNotification.RefreshValues();
		}

		// Token: 0x06001DB7 RID: 7607 RVA: 0x0006A200 File Offset: 0x00068400
		private void CalculateCohesion()
		{
			if (MobileParty.MainParty.Army != null)
			{
				this.Cohesion = (int)MobileParty.MainParty.Army.Cohesion;
				this.NewCohesion = MathF.Min(this.Cohesion + this._boostedCohesion, 100);
				ArmyManagementCalculationModel armyManagementCalculationModel = Campaign.Current.Models.ArmyManagementCalculationModel;
				this._currentParties.Clear();
				foreach (ArmyManagementItemVM armyManagementItemVM in this.PartiesInCart)
				{
					if (!armyManagementItemVM.Party.IsMainParty)
					{
						this._currentParties.Add(armyManagementItemVM.Party);
						if (!armyManagementItemVM.IsAlreadyWithPlayer)
						{
							this.NewCohesion = armyManagementCalculationModel.CalculateNewCohesion(MobileParty.MainParty.Army, armyManagementItemVM.Party.Party, this.NewCohesion, 1);
						}
					}
				}
			}
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x0006A2F0 File Offset: 0x000684F0
		private void OnFocus(ArmyManagementItemVM focusedItem)
		{
			this.FocusedItem = focusedItem;
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x0006A2FC File Offset: 0x000684FC
		private void OnAddToCart(ArmyManagementItemVM armyItem)
		{
			if (!this.PartiesInCart.Contains(armyItem))
			{
				this.PartiesInCart.Add(armyItem);
				armyItem.IsInCart = true;
				Game.Current.EventManager.TriggerEvent<PartyAddedToArmyByPlayerEvent>(new PartyAddedToArmyByPlayerEvent(armyItem.Party));
				if (this._partiesToRemove.Contains(armyItem))
				{
					this._partiesToRemove.Remove(armyItem);
				}
				if (armyItem.IsAlreadyWithPlayer)
				{
					armyItem.CanJoinBackWithoutCost = false;
				}
				this.TotalCost += armyItem.Cost;
			}
			this.CalculateCohesion();
			this.OnRefresh();
		}

		// Token: 0x06001DBA RID: 7610 RVA: 0x0006A390 File Offset: 0x00068590
		private void OnRemove(ArmyManagementItemVM armyItem)
		{
			if (this.PartiesInCart.Contains(armyItem))
			{
				this.PartiesInCart.Remove(armyItem);
				armyItem.IsInCart = false;
				this._partiesToRemove.Add(armyItem);
				if (armyItem.IsAlreadyWithPlayer)
				{
					armyItem.CanJoinBackWithoutCost = true;
				}
				this.TotalCost -= armyItem.Cost;
			}
			this.CalculateCohesion();
			this.OnRefresh();
		}

		// Token: 0x06001DBB RID: 7611 RVA: 0x0006A3FC File Offset: 0x000685FC
		private void ApplyCohesionChange()
		{
			if (MobileParty.MainParty.Army != null)
			{
				int num = this.NewCohesion - this.Cohesion;
				bool flag;
				if (this._influenceSpentForCohesionBoosting <= 0)
				{
					flag = MobileParty.MainParty.Army.Parties.All((MobileParty p) => p.ActualClan == MobileParty.MainParty.ActualClan);
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					MobileParty.MainParty.Army.BoostCohesionWithInfluence((float)num, this._influenceSpentForCohesionBoosting);
				}
			}
		}

		// Token: 0x06001DBC RID: 7612 RVA: 0x0006A47C File Offset: 0x0006867C
		private void OnBoostCohesion()
		{
			Army army = MobileParty.MainParty.Army;
			if (army != null && army.Cohesion < (float)100)
			{
				if (Hero.MainHero.Clan.Influence >= (float)(this.CohesionBoostCost + this.TotalCost))
				{
					this.NewCohesion += 10;
					this.TotalCost += this.CohesionBoostCost;
					this._boostedCohesion += 10;
					this._influenceSpentForCohesionBoosting += this.CohesionBoostCost;
					this.OnRefresh();
					return;
				}
				MBInformationManager.AddQuickInformation(new TextObject("{=Xmw93W6a}Not Enough Influence", null), 0, null, "");
			}
		}

		// Token: 0x06001DBD RID: 7613 RVA: 0x0006A52C File Offset: 0x0006872C
		private void OnRefresh()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			float num4 = 0f;
			foreach (ArmyManagementItemVM armyManagementItemVM in this.PartiesInCart)
			{
				num2++;
				num += armyManagementItemVM.Party.MemberRoster.TotalManCount;
				if (armyManagementItemVM.IsAlreadyWithPlayer)
				{
					num4 += armyManagementItemVM.Party.Food;
					num3 += (int)armyManagementItemVM.Party.Morale;
				}
			}
			num += Campaign.Current.Models.ArmyManagementCalculationModel.GetPartyStrength(PartyBase.MainParty);
			this.TotalStrength = num;
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_total_cost", null).ToString());
			this.TotalCostText = GameTexts.FindText("str_LEFT_colon", null).ToString();
			GameTexts.SetVariable("LEFT", this.TotalCost.ToString());
			GameTexts.SetVariable("RIGHT", ((int)Hero.MainHero.Clan.Influence).ToString());
			this.TotalCostNumbersText = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
			GameTexts.SetVariable("NUM", num2);
			this.TotalLords = GameTexts.FindText("str_NUM_lords", null).ToString();
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_strength", null).ToString());
			this.TotalStrengthText = GameTexts.FindText("str_LEFT_colon", null).ToString();
			this.CanCreateArmy = (float)this.TotalCost <= Hero.MainHero.Clan.Influence && num2 > 1;
			bool flag;
			if (MobileParty.MainParty.Army != null)
			{
				if (this._partiesToRemove.Count > 0)
				{
					flag = this.PartiesInCart.Count((ArmyManagementItemVM p) => p.IsAlreadyWithPlayer) >= 1;
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = false;
			}
			this.PlayerHasArmy = flag;
			this.CanBoostCohesion = this.PlayerHasArmy && 100 - this.NewCohesion >= 10;
			if (this.CanBoostCohesion)
			{
				TextObject textObject = new TextObject("{=s5b77f0H}Add +{BOOSTAMOUNT} cohesion to your army", null);
				textObject.SetTextVariable("BOOSTAMOUNT", 10);
				this.BoostCohesionHint.HintText = new TextObject("{=!}" + textObject.ToString(), null);
			}
			else if (100 - this.NewCohesion >= 10)
			{
				TextObject textObject2 = new TextObject("{=rsHPaaYZ}Cohesion needs to be lower than {MINAMOUNT} to boost", null);
				textObject2.SetTextVariable("MINAMOUNT", 90);
				this.BoostCohesionHint.HintText = new TextObject("{=!}" + textObject2.ToString(), null);
			}
			else
			{
				this.BoostCohesionHint.HintText = new TextObject("{=Ioiqzz4E}You need to be in an army to boost cohesion", null);
			}
			if (MobileParty.MainParty.Army != null)
			{
				this.CohesionText = GameTexts.FindText("str_cohesion", null).ToString();
				num3 += (int)MobileParty.MainParty.Morale;
				num4 += MobileParty.MainParty.Food;
			}
			this.MoraleText = num3.ToString();
			this.FoodText = MathF.Round(num4, 1).ToString();
			this.UpdateTooltips();
			this.PartiesInCart.Sort(this._itemComparer);
			TextObject textObject3;
			this.CanDisbandArmy = this.GetCanDisbandArmyWithReason(out textObject3);
			this.DisbandArmyHint.HintText = textObject3;
		}

		// Token: 0x06001DBE RID: 7614 RVA: 0x0006A894 File Offset: 0x00068A94
		private bool GetCanDisbandArmyWithReason(out TextObject disabledReason)
		{
			if (MobileParty.MainParty.Army == null)
			{
				disabledReason = new TextObject("{=iSZTOeYH}No army to disband.", null);
				return false;
			}
			if (MobileParty.MainParty.MapEvent != null)
			{
				disabledReason = new TextObject("{=uipNpzVw}Cannot disband the army right now.", null);
				return false;
			}
			if (PlayerSiege.PlayerSiegeEvent != null)
			{
				disabledReason = GameTexts.FindText("str_action_disabled_reason_siege", null);
				return false;
			}
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		// Token: 0x06001DBF RID: 7615 RVA: 0x0006A904 File Offset: 0x00068B04
		private void UpdateTooltips()
		{
			if (this.PlayerHasArmy)
			{
				this.CohesionHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetArmyCohesionTooltip(PartyBase.MainParty.MobileParty.Army));
				PartyBase.MainParty.MobileParty.Army.RecalculateArmyMorale();
				MathF.Round(PartyBase.MainParty.MobileParty.Army.Morale, 1).ToString("0.0");
				MBTextManager.SetTextVariable("BASE_EFFECT", MathF.Round(MobileParty.MainParty.Morale, 1).ToString("0.0"), false);
				MBTextManager.SetTextVariable("STR1", "", false);
				MBTextManager.SetTextVariable("STR2", "", false);
				MBTextManager.SetTextVariable("ARMY_MORALE", MobileParty.MainParty.Army.Morale);
				foreach (MobileParty mobileParty in MobileParty.MainParty.Army.Parties)
				{
					MBTextManager.SetTextVariable("STR1", GameTexts.FindText("str_STR1_STR2", null).ToString(), false);
					MBTextManager.SetTextVariable("PARTY_NAME", mobileParty.Name, false);
					MBTextManager.SetTextVariable("PARTY_MORALE", (int)mobileParty.Morale);
					MBTextManager.SetTextVariable("STR2", GameTexts.FindText("str_new_morale_item_line", null), false);
				}
				MBTextManager.SetTextVariable("ARMY_MORALE_ITEMS", GameTexts.FindText("str_STR1_STR2", null).ToString(), false);
				this.MoraleHint.HintText = GameTexts.FindText("str_army_morale_tooltip", null);
			}
			else
			{
				GameTexts.SetVariable("reg1", (int)MobileParty.MainParty.Morale);
				this.MoraleHint.HintText = GameTexts.FindText("str_morale_reg1", null);
			}
			this.DoneHint.HintText = new TextObject("{=!}" + (this.CanAffordInfluenceCost ? null : this._playerDoesntHaveEnoughInfluenceStr), null);
			MBTextManager.SetTextVariable("newline", "\n", false);
			MBTextManager.SetTextVariable("DAILY_FOOD_CONSUMPTION", MobileParty.MainParty.FoodChange);
			this.FoodHint.HintText = GameTexts.FindText("str_food_consumption_tooltip", null);
		}

		// Token: 0x06001DC0 RID: 7616 RVA: 0x0006AB44 File Offset: 0x00068D44
		public void ExecuteDone()
		{
			if (this.CanAffordInfluenceCost)
			{
				if (this.NewCohesion > this.Cohesion)
				{
					this.ApplyCohesionChange();
				}
				if (this.PartiesInCart.Count > 1 && MobileParty.MainParty.MapFaction.IsKingdomFaction)
				{
					if (MobileParty.MainParty.Army == null)
					{
						((Kingdom)MobileParty.MainParty.MapFaction).CreateArmy(Hero.MainHero, Hero.MainHero.HomeSettlement, Army.ArmyTypes.Patrolling);
					}
					foreach (ArmyManagementItemVM armyManagementItemVM in this.PartiesInCart)
					{
						if (armyManagementItemVM.Party != MobileParty.MainParty)
						{
							armyManagementItemVM.Party.Army = MobileParty.MainParty.Army;
							SetPartyAiAction.GetActionForEscortingParty(armyManagementItemVM.Party, MobileParty.MainParty);
						}
					}
					ChangeClanInfluenceAction.Apply(Clan.PlayerClan, (float)(-(float)(this.TotalCost - this._influenceSpentForCohesionBoosting)));
				}
				if (this._partiesToRemove.Count > 0)
				{
					bool flag = false;
					foreach (ArmyManagementItemVM armyManagementItemVM2 in this._partiesToRemove)
					{
						if (armyManagementItemVM2.Party == MobileParty.MainParty)
						{
							armyManagementItemVM2.Party.Army = null;
							flag = true;
						}
					}
					if (!flag)
					{
						foreach (ArmyManagementItemVM armyManagementItemVM3 in this._partiesToRemove)
						{
							Army army = MobileParty.MainParty.Army;
							if (army != null && army.Parties.Contains(armyManagementItemVM3.Party))
							{
								armyManagementItemVM3.Party.Army = null;
							}
						}
					}
					this._partiesToRemove.Clear();
				}
				this._onClose();
				CampaignEventDispatcher.Instance.OnArmyOverlaySetDirty();
			}
		}

		// Token: 0x06001DC1 RID: 7617 RVA: 0x0006AD38 File Offset: 0x00068F38
		public void ExecuteCancel()
		{
			ChangeClanInfluenceAction.Apply(Clan.PlayerClan, this._initialInfluence - Clan.PlayerClan.Influence);
			this._onClose();
		}

		// Token: 0x06001DC2 RID: 7618 RVA: 0x0006AD60 File Offset: 0x00068F60
		public void ExecuteReset()
		{
			foreach (ArmyManagementItemVM armyManagementItemVM in this.PartiesInCart.ToList<ArmyManagementItemVM>())
			{
				this.OnRemove(armyManagementItemVM);
				armyManagementItemVM.UpdateEligibility();
			}
			this.PartiesInCart.Add(this._mainPartyItem);
			foreach (ArmyManagementItemVM armyManagementItemVM2 in this.PartyList)
			{
				if (armyManagementItemVM2.IsAlreadyWithPlayer)
				{
					this.PartiesInCart.Add(armyManagementItemVM2);
					armyManagementItemVM2.IsInCart = true;
					armyManagementItemVM2.CanJoinBackWithoutCost = false;
				}
			}
			this.NewCohesion = this.Cohesion;
			ChangeClanInfluenceAction.Apply(Clan.PlayerClan, this._initialInfluence - Clan.PlayerClan.Influence);
			this.TotalCost = 0;
			this._boostedCohesion = 0;
			this._influenceSpentForCohesionBoosting = 0;
			this._partiesToRemove.Clear();
			this.OnRefresh();
		}

		// Token: 0x06001DC3 RID: 7619 RVA: 0x0006AE74 File Offset: 0x00069074
		public void ExecuteDisbandArmy()
		{
			if (this.CanDisbandArmy)
			{
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=ViYdZUbQ}Disband Army", null).ToString(), new TextObject("{=kqeA8rjL}Are you sure you want to disband your army?", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					this.DisbandArmy();
				}, null, "", 0f, null, null, null), false, false);
			}
		}

		// Token: 0x06001DC4 RID: 7620 RVA: 0x0006AEF1 File Offset: 0x000690F1
		public void ExecuteBoostCohesionManual()
		{
			this.OnBoostCohesion();
			Game.Current.EventManager.TriggerEvent<ArmyCohesionBoostedByPlayerEvent>(new ArmyCohesionBoostedByPlayerEvent());
		}

		// Token: 0x06001DC5 RID: 7621 RVA: 0x0006AF10 File Offset: 0x00069110
		private void DisbandArmy()
		{
			foreach (ArmyManagementItemVM armyManagementItemVM in this.PartiesInCart.ToList<ArmyManagementItemVM>())
			{
				this.OnRemove(armyManagementItemVM);
			}
			this.ExecuteDone();
		}

		// Token: 0x06001DC6 RID: 7622 RVA: 0x0006AF70 File Offset: 0x00069170
		private void OnCloseBoost()
		{
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(TutorialContexts.ArmyManagement));
		}

		// Token: 0x06001DC7 RID: 7623 RVA: 0x0006AF88 File Offset: 0x00069188
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = string.Empty;
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = this._latestTutorialElementID;
				}
			}
		}

		// Token: 0x06001DC8 RID: 7624 RVA: 0x0006AFE8 File Offset: 0x000691E8
		public override void OnFinalize()
		{
			base.OnFinalize();
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM resetInputKey = this.ResetInputKey;
			if (resetInputKey != null)
			{
				resetInputKey.OnFinalize();
			}
			InputKeyItemVM removeInputKey = this.RemoveInputKey;
			if (removeInputKey == null)
			{
				return;
			}
			removeInputKey.OnFinalize();
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x06001DC9 RID: 7625 RVA: 0x0006B059 File Offset: 0x00069259
		// (set) Token: 0x06001DCA RID: 7626 RVA: 0x0006B061 File Offset: 0x00069261
		[DataSourceProperty]
		public ElementNotificationVM TutorialNotification
		{
			get
			{
				return this._tutorialNotification;
			}
			set
			{
				if (value != this._tutorialNotification)
				{
					this._tutorialNotification = value;
					base.OnPropertyChangedWithValue<ElementNotificationVM>(value, "TutorialNotification");
				}
			}
		}

		// Token: 0x17000A33 RID: 2611
		// (get) Token: 0x06001DCB RID: 7627 RVA: 0x0006B07F File Offset: 0x0006927F
		// (set) Token: 0x06001DCC RID: 7628 RVA: 0x0006B087 File Offset: 0x00069287
		[DataSourceProperty]
		public ArmyManagementSortControllerVM SortControllerVM
		{
			get
			{
				return this._sortControllerVM;
			}
			set
			{
				if (value != this._sortControllerVM)
				{
					this._sortControllerVM = value;
					base.OnPropertyChangedWithValue<ArmyManagementSortControllerVM>(value, "SortControllerVM");
				}
			}
		}

		// Token: 0x17000A34 RID: 2612
		// (get) Token: 0x06001DCD RID: 7629 RVA: 0x0006B0A5 File Offset: 0x000692A5
		// (set) Token: 0x06001DCE RID: 7630 RVA: 0x0006B0AD File Offset: 0x000692AD
		[DataSourceProperty]
		public string BoostTitleText
		{
			get
			{
				return this._boostTitleText;
			}
			set
			{
				if (value != this._boostTitleText)
				{
					this._boostTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "BoostTitleText");
				}
			}
		}

		// Token: 0x17000A35 RID: 2613
		// (get) Token: 0x06001DCF RID: 7631 RVA: 0x0006B0D0 File Offset: 0x000692D0
		// (set) Token: 0x06001DD0 RID: 7632 RVA: 0x0006B0D8 File Offset: 0x000692D8
		[DataSourceProperty]
		public string DisbandArmyText
		{
			get
			{
				return this._disbandArmyText;
			}
			set
			{
				if (value != this._disbandArmyText)
				{
					this._disbandArmyText = value;
					base.OnPropertyChangedWithValue<string>(value, "DisbandArmyText");
				}
			}
		}

		// Token: 0x17000A36 RID: 2614
		// (get) Token: 0x06001DD1 RID: 7633 RVA: 0x0006B0FB File Offset: 0x000692FB
		// (set) Token: 0x06001DD2 RID: 7634 RVA: 0x0006B103 File Offset: 0x00069303
		[DataSourceProperty]
		public string CohesionBoostAmountText
		{
			get
			{
				return this._cohesionBoostAmountText;
			}
			set
			{
				if (value != this._cohesionBoostAmountText)
				{
					this._cohesionBoostAmountText = value;
					base.OnPropertyChangedWithValue<string>(value, "CohesionBoostAmountText");
				}
			}
		}

		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x06001DD3 RID: 7635 RVA: 0x0006B126 File Offset: 0x00069326
		// (set) Token: 0x06001DD4 RID: 7636 RVA: 0x0006B12E File Offset: 0x0006932E
		[DataSourceProperty]
		public string DistanceText
		{
			get
			{
				return this._distanceText;
			}
			set
			{
				if (value != this._distanceText)
				{
					this._distanceText = value;
					base.OnPropertyChangedWithValue<string>(value, "DistanceText");
				}
			}
		}

		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x06001DD5 RID: 7637 RVA: 0x0006B151 File Offset: 0x00069351
		// (set) Token: 0x06001DD6 RID: 7638 RVA: 0x0006B159 File Offset: 0x00069359
		[DataSourceProperty]
		public string CostText
		{
			get
			{
				return this._costText;
			}
			set
			{
				if (value != this._costText)
				{
					this._costText = value;
					base.OnPropertyChangedWithValue<string>(value, "CostText");
				}
			}
		}

		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x06001DD7 RID: 7639 RVA: 0x0006B17C File Offset: 0x0006937C
		// (set) Token: 0x06001DD8 RID: 7640 RVA: 0x0006B184 File Offset: 0x00069384
		[DataSourceProperty]
		public string OwnerText
		{
			get
			{
				return this._ownerText;
			}
			set
			{
				if (value != this._ownerText)
				{
					this._ownerText = value;
					base.OnPropertyChangedWithValue<string>(value, "OwnerText");
				}
			}
		}

		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x06001DD9 RID: 7641 RVA: 0x0006B1A7 File Offset: 0x000693A7
		// (set) Token: 0x06001DDA RID: 7642 RVA: 0x0006B1AF File Offset: 0x000693AF
		[DataSourceProperty]
		public string StrengthText
		{
			get
			{
				return this._strengthText;
			}
			set
			{
				if (value != this._strengthText)
				{
					this._strengthText = value;
					base.OnPropertyChangedWithValue<string>(value, "StrengthText");
				}
			}
		}

		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x06001DDB RID: 7643 RVA: 0x0006B1D2 File Offset: 0x000693D2
		// (set) Token: 0x06001DDC RID: 7644 RVA: 0x0006B1DA File Offset: 0x000693DA
		[DataSourceProperty]
		public string LordsText
		{
			get
			{
				return this._lordsText;
			}
			set
			{
				if (value != this._lordsText)
				{
					this._lordsText = value;
					base.OnPropertyChangedWithValue<string>(value, "LordsText");
				}
			}
		}

		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x06001DDD RID: 7645 RVA: 0x0006B1FD File Offset: 0x000693FD
		// (set) Token: 0x06001DDE RID: 7646 RVA: 0x0006B205 File Offset: 0x00069405
		[DataSourceProperty]
		public string TotalInfluence
		{
			get
			{
				return this._totalInfluence;
			}
			set
			{
				if (value != this._totalInfluence)
				{
					this._totalInfluence = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalInfluence");
				}
			}
		}

		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x06001DDF RID: 7647 RVA: 0x0006B228 File Offset: 0x00069428
		// (set) Token: 0x06001DE0 RID: 7648 RVA: 0x0006B230 File Offset: 0x00069430
		[DataSourceProperty]
		public int TotalStrength
		{
			get
			{
				return this._totalStrength;
			}
			set
			{
				if (value != this._totalStrength)
				{
					this._totalStrength = value;
					base.OnPropertyChangedWithValue(value, "TotalStrength");
				}
			}
		}

		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x06001DE1 RID: 7649 RVA: 0x0006B24E File Offset: 0x0006944E
		// (set) Token: 0x06001DE2 RID: 7650 RVA: 0x0006B258 File Offset: 0x00069458
		[DataSourceProperty]
		public int TotalCost
		{
			get
			{
				return this._totalCost;
			}
			set
			{
				if (value != this._totalCost)
				{
					this._totalCost = value;
					this.CanAffordInfluenceCost = this.TotalCost <= 0 || (float)this.TotalCost <= Hero.MainHero.Clan.Influence;
					base.OnPropertyChangedWithValue(value, "TotalCost");
				}
			}
		}

		// Token: 0x17000A3F RID: 2623
		// (get) Token: 0x06001DE3 RID: 7651 RVA: 0x0006B2AE File Offset: 0x000694AE
		// (set) Token: 0x06001DE4 RID: 7652 RVA: 0x0006B2B6 File Offset: 0x000694B6
		[DataSourceProperty]
		public string TotalLords
		{
			get
			{
				return this._totalLords;
			}
			set
			{
				if (value != this._totalLords)
				{
					this._totalLords = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalLords");
				}
			}
		}

		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06001DE5 RID: 7653 RVA: 0x0006B2D9 File Offset: 0x000694D9
		// (set) Token: 0x06001DE6 RID: 7654 RVA: 0x0006B2E1 File Offset: 0x000694E1
		[DataSourceProperty]
		public bool CanCreateArmy
		{
			get
			{
				return this._canCreateArmy;
			}
			set
			{
				if (value != this._canCreateArmy)
				{
					this._canCreateArmy = value;
					base.OnPropertyChangedWithValue(value, "CanCreateArmy");
				}
			}
		}

		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06001DE7 RID: 7655 RVA: 0x0006B2FF File Offset: 0x000694FF
		// (set) Token: 0x06001DE8 RID: 7656 RVA: 0x0006B307 File Offset: 0x00069507
		[DataSourceProperty]
		public bool CanBoostCohesion
		{
			get
			{
				return this._canBoostCohesion;
			}
			set
			{
				if (value != this._canBoostCohesion)
				{
					this._canBoostCohesion = value;
					base.OnPropertyChangedWithValue(value, "CanBoostCohesion");
				}
			}
		}

		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06001DE9 RID: 7657 RVA: 0x0006B325 File Offset: 0x00069525
		// (set) Token: 0x06001DEA RID: 7658 RVA: 0x0006B32D File Offset: 0x0006952D
		[DataSourceProperty]
		public bool CanDisbandArmy
		{
			get
			{
				return this._canDisbandArmy;
			}
			set
			{
				if (value != this._canDisbandArmy)
				{
					this._canDisbandArmy = value;
					base.OnPropertyChangedWithValue(value, "CanDisbandArmy");
				}
			}
		}

		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x06001DEB RID: 7659 RVA: 0x0006B34B File Offset: 0x0006954B
		// (set) Token: 0x06001DEC RID: 7660 RVA: 0x0006B353 File Offset: 0x00069553
		[DataSourceProperty]
		public bool CanAffordInfluenceCost
		{
			get
			{
				return this._canAffordInfluenceCost;
			}
			set
			{
				if (value != this._canAffordInfluenceCost)
				{
					this._canAffordInfluenceCost = value;
					base.OnPropertyChangedWithValue(value, "CanAffordInfluenceCost");
				}
			}
		}

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x06001DED RID: 7661 RVA: 0x0006B371 File Offset: 0x00069571
		// (set) Token: 0x06001DEE RID: 7662 RVA: 0x0006B379 File Offset: 0x00069579
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

		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x06001DEF RID: 7663 RVA: 0x0006B39C File Offset: 0x0006959C
		// (set) Token: 0x06001DF0 RID: 7664 RVA: 0x0006B3A4 File Offset: 0x000695A4
		[DataSourceProperty]
		public string ClanText
		{
			get
			{
				return this._clanText;
			}
			set
			{
				if (value != this._clanText)
				{
					this._clanText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClanText");
				}
			}
		}

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x06001DF1 RID: 7665 RVA: 0x0006B3C7 File Offset: 0x000695C7
		// (set) Token: 0x06001DF2 RID: 7666 RVA: 0x0006B3CF File Offset: 0x000695CF
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x06001DF3 RID: 7667 RVA: 0x0006B3F2 File Offset: 0x000695F2
		// (set) Token: 0x06001DF4 RID: 7668 RVA: 0x0006B3FA File Offset: 0x000695FA
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

		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x06001DF5 RID: 7669 RVA: 0x0006B41D File Offset: 0x0006961D
		// (set) Token: 0x06001DF6 RID: 7670 RVA: 0x0006B425 File Offset: 0x00069625
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

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06001DF7 RID: 7671 RVA: 0x0006B448 File Offset: 0x00069648
		// (set) Token: 0x06001DF8 RID: 7672 RVA: 0x0006B450 File Offset: 0x00069650
		[DataSourceProperty]
		public ArmyManagementItemVM FocusedItem
		{
			get
			{
				return this._focusedItem;
			}
			set
			{
				if (value != this._focusedItem)
				{
					this._focusedItem = value;
					base.OnPropertyChangedWithValue<ArmyManagementItemVM>(value, "FocusedItem");
				}
			}
		}

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06001DF9 RID: 7673 RVA: 0x0006B46E File Offset: 0x0006966E
		// (set) Token: 0x06001DFA RID: 7674 RVA: 0x0006B476 File Offset: 0x00069676
		[DataSourceProperty]
		public MBBindingList<ArmyManagementItemVM> PartyList
		{
			get
			{
				return this._partyList;
			}
			set
			{
				if (value != this._partyList)
				{
					this._partyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<ArmyManagementItemVM>>(value, "PartyList");
				}
			}
		}

		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x06001DFB RID: 7675 RVA: 0x0006B494 File Offset: 0x00069694
		// (set) Token: 0x06001DFC RID: 7676 RVA: 0x0006B49C File Offset: 0x0006969C
		[DataSourceProperty]
		public MBBindingList<ArmyManagementItemVM> PartiesInCart
		{
			get
			{
				return this._partiesInCart;
			}
			set
			{
				if (value != this._partiesInCart)
				{
					this._partiesInCart = value;
					base.OnPropertyChangedWithValue<MBBindingList<ArmyManagementItemVM>>(value, "PartiesInCart");
				}
			}
		}

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06001DFD RID: 7677 RVA: 0x0006B4BA File Offset: 0x000696BA
		// (set) Token: 0x06001DFE RID: 7678 RVA: 0x0006B4C2 File Offset: 0x000696C2
		[DataSourceProperty]
		public string TotalStrengthText
		{
			get
			{
				return this._totalStrengthText;
			}
			set
			{
				if (value != this._totalStrengthText)
				{
					this._totalStrengthText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalStrengthText");
				}
			}
		}

		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06001DFF RID: 7679 RVA: 0x0006B4E5 File Offset: 0x000696E5
		// (set) Token: 0x06001E00 RID: 7680 RVA: 0x0006B4ED File Offset: 0x000696ED
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

		// Token: 0x17000A4E RID: 2638
		// (get) Token: 0x06001E01 RID: 7681 RVA: 0x0006B510 File Offset: 0x00069710
		// (set) Token: 0x06001E02 RID: 7682 RVA: 0x0006B518 File Offset: 0x00069718
		[DataSourceProperty]
		public string TotalCostNumbersText
		{
			get
			{
				return this._totalCostNumbersText;
			}
			set
			{
				if (value != this._totalCostNumbersText)
				{
					this._totalCostNumbersText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalCostNumbersText");
				}
			}
		}

		// Token: 0x17000A4F RID: 2639
		// (get) Token: 0x06001E03 RID: 7683 RVA: 0x0006B53B File Offset: 0x0006973B
		// (set) Token: 0x06001E04 RID: 7684 RVA: 0x0006B543 File Offset: 0x00069743
		[DataSourceProperty]
		public string CohesionText
		{
			get
			{
				return this._cohesionText;
			}
			set
			{
				if (value != this._cohesionText)
				{
					this._cohesionText = value;
					base.OnPropertyChangedWithValue<string>(value, "CohesionText");
				}
			}
		}

		// Token: 0x17000A50 RID: 2640
		// (get) Token: 0x06001E05 RID: 7685 RVA: 0x0006B566 File Offset: 0x00069766
		// (set) Token: 0x06001E06 RID: 7686 RVA: 0x0006B56E File Offset: 0x0006976E
		[DataSourceProperty]
		public int Cohesion
		{
			get
			{
				return this._cohesion;
			}
			set
			{
				if (value != this._cohesion)
				{
					this._cohesion = value;
					base.OnPropertyChangedWithValue(value, "Cohesion");
				}
			}
		}

		// Token: 0x17000A51 RID: 2641
		// (get) Token: 0x06001E07 RID: 7687 RVA: 0x0006B58C File Offset: 0x0006978C
		// (set) Token: 0x06001E08 RID: 7688 RVA: 0x0006B594 File Offset: 0x00069794
		[DataSourceProperty]
		public int CohesionBoostCost
		{
			get
			{
				return this._cohesionBoostCost;
			}
			set
			{
				if (value != this._cohesionBoostCost)
				{
					this._cohesionBoostCost = value;
					base.OnPropertyChangedWithValue(value, "CohesionBoostCost");
				}
			}
		}

		// Token: 0x17000A52 RID: 2642
		// (get) Token: 0x06001E09 RID: 7689 RVA: 0x0006B5B2 File Offset: 0x000697B2
		// (set) Token: 0x06001E0A RID: 7690 RVA: 0x0006B5BA File Offset: 0x000697BA
		[DataSourceProperty]
		public bool PlayerHasArmy
		{
			get
			{
				return this._playerHasArmy;
			}
			set
			{
				if (value != this._playerHasArmy)
				{
					this._playerHasArmy = value;
					base.OnPropertyChangedWithValue(value, "PlayerHasArmy");
				}
			}
		}

		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x06001E0B RID: 7691 RVA: 0x0006B5D8 File Offset: 0x000697D8
		// (set) Token: 0x06001E0C RID: 7692 RVA: 0x0006B5E0 File Offset: 0x000697E0
		[DataSourceProperty]
		public string MoraleText
		{
			get
			{
				return this._moraleText;
			}
			set
			{
				if (value != this._moraleText)
				{
					this._moraleText = value;
					base.OnPropertyChangedWithValue<string>(value, "MoraleText");
				}
			}
		}

		// Token: 0x17000A54 RID: 2644
		// (get) Token: 0x06001E0D RID: 7693 RVA: 0x0006B603 File Offset: 0x00069803
		// (set) Token: 0x06001E0E RID: 7694 RVA: 0x0006B60B File Offset: 0x0006980B
		[DataSourceProperty]
		public string FoodText
		{
			get
			{
				return this._foodText;
			}
			set
			{
				if (value != this._foodText)
				{
					this._foodText = value;
					base.OnPropertyChangedWithValue<string>(value, "FoodText");
				}
			}
		}

		// Token: 0x17000A55 RID: 2645
		// (get) Token: 0x06001E0F RID: 7695 RVA: 0x0006B62E File Offset: 0x0006982E
		// (set) Token: 0x06001E10 RID: 7696 RVA: 0x0006B636 File Offset: 0x00069836
		[DataSourceProperty]
		public int NewCohesion
		{
			get
			{
				return this._newCohesion;
			}
			set
			{
				if (value != this._newCohesion)
				{
					this._newCohesion = value;
					base.OnPropertyChangedWithValue(value, "NewCohesion");
				}
			}
		}

		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x06001E11 RID: 7697 RVA: 0x0006B654 File Offset: 0x00069854
		// (set) Token: 0x06001E12 RID: 7698 RVA: 0x0006B65C File Offset: 0x0006985C
		[DataSourceProperty]
		public BasicTooltipViewModel CohesionHint
		{
			get
			{
				return this._cohesionHint;
			}
			set
			{
				if (value != this._cohesionHint)
				{
					this._cohesionHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "CohesionHint");
				}
			}
		}

		// Token: 0x17000A57 RID: 2647
		// (get) Token: 0x06001E13 RID: 7699 RVA: 0x0006B67A File Offset: 0x0006987A
		// (set) Token: 0x06001E14 RID: 7700 RVA: 0x0006B682 File Offset: 0x00069882
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

		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x06001E15 RID: 7701 RVA: 0x0006B6A0 File Offset: 0x000698A0
		// (set) Token: 0x06001E16 RID: 7702 RVA: 0x0006B6A8 File Offset: 0x000698A8
		[DataSourceProperty]
		public HintViewModel BoostCohesionHint
		{
			get
			{
				return this._boostCohesionHint;
			}
			set
			{
				if (value != this._boostCohesionHint)
				{
					this._boostCohesionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "BoostCohesionHint");
				}
			}
		}

		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x06001E17 RID: 7703 RVA: 0x0006B6C6 File Offset: 0x000698C6
		// (set) Token: 0x06001E18 RID: 7704 RVA: 0x0006B6CE File Offset: 0x000698CE
		[DataSourceProperty]
		public HintViewModel DisbandArmyHint
		{
			get
			{
				return this._disbandArmyHint;
			}
			set
			{
				if (value != this._disbandArmyHint)
				{
					this._disbandArmyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisbandArmyHint");
				}
			}
		}

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x06001E19 RID: 7705 RVA: 0x0006B6EC File Offset: 0x000698EC
		// (set) Token: 0x06001E1A RID: 7706 RVA: 0x0006B6F4 File Offset: 0x000698F4
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

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x06001E1B RID: 7707 RVA: 0x0006B712 File Offset: 0x00069912
		// (set) Token: 0x06001E1C RID: 7708 RVA: 0x0006B71A File Offset: 0x0006991A
		[DataSourceProperty]
		public HintViewModel FoodHint
		{
			get
			{
				return this._foodHint;
			}
			set
			{
				if (value != this._foodHint)
				{
					this._foodHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FoodHint");
				}
			}
		}

		// Token: 0x06001E1D RID: 7709 RVA: 0x0006B738 File Offset: 0x00069938
		public void SetResetInputKey(HotKey hotKey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001E1E RID: 7710 RVA: 0x0006B747 File Offset: 0x00069947
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001E1F RID: 7711 RVA: 0x0006B756 File Offset: 0x00069956
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001E20 RID: 7712 RVA: 0x0006B765 File Offset: 0x00069965
		public void SetRemoveInputKey(HotKey hotKey)
		{
			this.RemoveInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000A5C RID: 2652
		// (get) Token: 0x06001E21 RID: 7713 RVA: 0x0006B774 File Offset: 0x00069974
		// (set) Token: 0x06001E22 RID: 7714 RVA: 0x0006B77C File Offset: 0x0006997C
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

		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x06001E23 RID: 7715 RVA: 0x0006B79A File Offset: 0x0006999A
		// (set) Token: 0x06001E24 RID: 7716 RVA: 0x0006B7A2 File Offset: 0x000699A2
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

		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x06001E25 RID: 7717 RVA: 0x0006B7C0 File Offset: 0x000699C0
		// (set) Token: 0x06001E26 RID: 7718 RVA: 0x0006B7C8 File Offset: 0x000699C8
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

		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x06001E27 RID: 7719 RVA: 0x0006B7E6 File Offset: 0x000699E6
		// (set) Token: 0x06001E28 RID: 7720 RVA: 0x0006B7F0 File Offset: 0x000699F0
		[DataSourceProperty]
		public InputKeyItemVM RemoveInputKey
		{
			get
			{
				return this._removeInputKey;
			}
			set
			{
				if (value != this._removeInputKey)
				{
					this._removeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RemoveInputKey");
					foreach (ArmyManagementItemVM armyManagementItemVM in this.PartyList)
					{
						armyManagementItemVM.RemoveInputKey = value;
					}
				}
			}
		}

		// Token: 0x04000E02 RID: 3586
		private readonly Action _onClose;

		// Token: 0x04000E03 RID: 3587
		private readonly ArmyManagementItemVM _mainPartyItem;

		// Token: 0x04000E04 RID: 3588
		private readonly ArmyManagementVM.ManagementItemComparer _itemComparer;

		// Token: 0x04000E05 RID: 3589
		private readonly float _initialInfluence;

		// Token: 0x04000E06 RID: 3590
		private string _latestTutorialElementID;

		// Token: 0x04000E07 RID: 3591
		private string _playerDoesntHaveEnoughInfluenceStr;

		// Token: 0x04000E08 RID: 3592
		private const int _cohesionBoostAmount = 10;

		// Token: 0x04000E09 RID: 3593
		private int _influenceSpentForCohesionBoosting;

		// Token: 0x04000E0A RID: 3594
		private int _boostedCohesion;

		// Token: 0x04000E0B RID: 3595
		private string _titleText;

		// Token: 0x04000E0C RID: 3596
		private string _boostTitleText;

		// Token: 0x04000E0D RID: 3597
		private string _cancelText;

		// Token: 0x04000E0E RID: 3598
		private string _doneText;

		// Token: 0x04000E0F RID: 3599
		private bool _canCreateArmy;

		// Token: 0x04000E10 RID: 3600
		private bool _canBoostCohesion;

		// Token: 0x04000E11 RID: 3601
		private List<MobileParty> _currentParties;

		// Token: 0x04000E12 RID: 3602
		private ArmyManagementItemVM _focusedItem;

		// Token: 0x04000E13 RID: 3603
		private MBBindingList<ArmyManagementItemVM> _partyList;

		// Token: 0x04000E14 RID: 3604
		private MBBindingList<ArmyManagementItemVM> _partiesInCart;

		// Token: 0x04000E15 RID: 3605
		private MBBindingList<ArmyManagementItemVM> _partiesToRemove;

		// Token: 0x04000E16 RID: 3606
		private ArmyManagementSortControllerVM _sortControllerVM;

		// Token: 0x04000E17 RID: 3607
		private int _totalStrength;

		// Token: 0x04000E18 RID: 3608
		private int _totalCost;

		// Token: 0x04000E19 RID: 3609
		private int _cohesion;

		// Token: 0x04000E1A RID: 3610
		private int _cohesionBoostCost;

		// Token: 0x04000E1B RID: 3611
		private string _cohesionText;

		// Token: 0x04000E1C RID: 3612
		private int _newCohesion;

		// Token: 0x04000E1D RID: 3613
		private string _totalStrengthText;

		// Token: 0x04000E1E RID: 3614
		private string _totalCostText;

		// Token: 0x04000E1F RID: 3615
		private string _totalCostNumbersText;

		// Token: 0x04000E20 RID: 3616
		private string _totalInfluence;

		// Token: 0x04000E21 RID: 3617
		private string _totalLords;

		// Token: 0x04000E22 RID: 3618
		private string _costText;

		// Token: 0x04000E23 RID: 3619
		private string _strengthText;

		// Token: 0x04000E24 RID: 3620
		private string _lordsText;

		// Token: 0x04000E25 RID: 3621
		private string _distanceText;

		// Token: 0x04000E26 RID: 3622
		private string _clanText;

		// Token: 0x04000E27 RID: 3623
		private string _ownerText;

		// Token: 0x04000E28 RID: 3624
		private string _nameText;

		// Token: 0x04000E29 RID: 3625
		private string _disbandArmyText;

		// Token: 0x04000E2A RID: 3626
		private string _cohesionBoostAmountText;

		// Token: 0x04000E2B RID: 3627
		private bool _playerHasArmy;

		// Token: 0x04000E2C RID: 3628
		private bool _canDisbandArmy;

		// Token: 0x04000E2D RID: 3629
		private bool _canAffordInfluenceCost;

		// Token: 0x04000E2E RID: 3630
		private string _moraleText;

		// Token: 0x04000E2F RID: 3631
		private string _foodText;

		// Token: 0x04000E30 RID: 3632
		private BasicTooltipViewModel _cohesionHint;

		// Token: 0x04000E31 RID: 3633
		private HintViewModel _moraleHint;

		// Token: 0x04000E32 RID: 3634
		private HintViewModel _foodHint;

		// Token: 0x04000E33 RID: 3635
		private HintViewModel _boostCohesionHint;

		// Token: 0x04000E34 RID: 3636
		private HintViewModel _disbandArmyHint;

		// Token: 0x04000E35 RID: 3637
		private HintViewModel _doneHint;

		// Token: 0x04000E36 RID: 3638
		public ElementNotificationVM _tutorialNotification;

		// Token: 0x04000E37 RID: 3639
		private InputKeyItemVM _resetInputKey;

		// Token: 0x04000E38 RID: 3640
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000E39 RID: 3641
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000E3A RID: 3642
		private InputKeyItemVM _removeInputKey;

		// Token: 0x02000286 RID: 646
		public class ManagementItemComparer : IComparer<ArmyManagementItemVM>
		{
			// Token: 0x06002293 RID: 8851 RVA: 0x00072FDC File Offset: 0x000711DC
			public int Compare(ArmyManagementItemVM x, ArmyManagementItemVM y)
			{
				if (x.IsMainHero)
				{
					return -1;
				}
				return y.IsAlreadyWithPlayer.CompareTo(x.IsAlreadyWithPlayer);
			}
		}
	}
}
