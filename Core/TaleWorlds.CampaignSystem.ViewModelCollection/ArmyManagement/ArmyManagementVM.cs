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
	public class ArmyManagementVM : ViewModel
	{
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

		private void OnFocus(ArmyManagementItemVM focusedItem)
		{
			this.FocusedItem = focusedItem;
		}

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

		public void ExecuteCancel()
		{
			ChangeClanInfluenceAction.Apply(Clan.PlayerClan, this._initialInfluence - Clan.PlayerClan.Influence);
			this._onClose();
		}

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

		public void ExecuteBoostCohesionManual()
		{
			this.OnBoostCohesion();
			Game.Current.EventManager.TriggerEvent<ArmyCohesionBoostedByPlayerEvent>(new ArmyCohesionBoostedByPlayerEvent());
		}

		private void DisbandArmy()
		{
			foreach (ArmyManagementItemVM armyManagementItemVM in this.PartiesInCart.ToList<ArmyManagementItemVM>())
			{
				this.OnRemove(armyManagementItemVM);
			}
			this.ExecuteDone();
		}

		private void OnCloseBoost()
		{
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(TutorialContexts.ArmyManagement));
		}

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

		public void SetResetInputKey(HotKey hotKey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetRemoveInputKey(HotKey hotKey)
		{
			this.RemoveInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
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

		private readonly Action _onClose;

		private readonly ArmyManagementItemVM _mainPartyItem;

		private readonly ArmyManagementVM.ManagementItemComparer _itemComparer;

		private readonly float _initialInfluence;

		private string _latestTutorialElementID;

		private string _playerDoesntHaveEnoughInfluenceStr;

		private const int _cohesionBoostAmount = 10;

		private int _influenceSpentForCohesionBoosting;

		private int _boostedCohesion;

		private string _titleText;

		private string _boostTitleText;

		private string _cancelText;

		private string _doneText;

		private bool _canCreateArmy;

		private bool _canBoostCohesion;

		private List<MobileParty> _currentParties;

		private ArmyManagementItemVM _focusedItem;

		private MBBindingList<ArmyManagementItemVM> _partyList;

		private MBBindingList<ArmyManagementItemVM> _partiesInCart;

		private MBBindingList<ArmyManagementItemVM> _partiesToRemove;

		private ArmyManagementSortControllerVM _sortControllerVM;

		private int _totalStrength;

		private int _totalCost;

		private int _cohesion;

		private int _cohesionBoostCost;

		private string _cohesionText;

		private int _newCohesion;

		private string _totalStrengthText;

		private string _totalCostText;

		private string _totalCostNumbersText;

		private string _totalInfluence;

		private string _totalLords;

		private string _costText;

		private string _strengthText;

		private string _lordsText;

		private string _distanceText;

		private string _clanText;

		private string _ownerText;

		private string _nameText;

		private string _disbandArmyText;

		private string _cohesionBoostAmountText;

		private bool _playerHasArmy;

		private bool _canDisbandArmy;

		private bool _canAffordInfluenceCost;

		private string _moraleText;

		private string _foodText;

		private BasicTooltipViewModel _cohesionHint;

		private HintViewModel _moraleHint;

		private HintViewModel _foodHint;

		private HintViewModel _boostCohesionHint;

		private HintViewModel _disbandArmyHint;

		private HintViewModel _doneHint;

		public ElementNotificationVM _tutorialNotification;

		private InputKeyItemVM _resetInputKey;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _removeInputKey;

		public class ManagementItemComparer : IComparer<ArmyManagementItemVM>
		{
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
