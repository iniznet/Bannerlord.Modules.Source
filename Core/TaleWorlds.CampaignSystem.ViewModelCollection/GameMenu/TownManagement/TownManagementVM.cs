using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x02000096 RID: 150
	public class TownManagementVM : ViewModel
	{
		// Token: 0x06000E64 RID: 3684 RVA: 0x000392F8 File Offset: 0x000374F8
		public TownManagementVM()
		{
			this._teleportationBehavior = Campaign.Current.GetCampaignBehavior<ITeleportationCampaignBehavior>();
			this.ProjectSelection = new SettlementProjectSelectionVM(Settlement.CurrentSettlement, new Action(this.OnChangeInBuildingQueue));
			this.GovernorSelection = new SettlementGovernorSelectionVM(Settlement.CurrentSettlement, new Action<Hero>(this.OnGovernorSelectionDone));
			this.ReserveControl = new TownManagementReserveControlVM(Settlement.CurrentSettlement, new Action(this.OnReserveUpdated));
			this.MiddleFirstTextList = new MBBindingList<TownManagementDescriptionItemVM>();
			this.MiddleSecondTextList = new MBBindingList<TownManagementDescriptionItemVM>();
			this.Shops = new MBBindingList<TownManagementShopItemVM>();
			this.Villages = new MBBindingList<TownManagementVillageItemVM>();
			this.Show = false;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			this.IsTown = currentSettlement != null && currentSettlement.IsTown;
			Settlement currentSettlement2 = Settlement.CurrentSettlement;
			if (currentSettlement2 == null || !currentSettlement2.IsFortification)
			{
				return;
			}
			this.IsThereCurrentProject = Settlement.CurrentSettlement.Town.CurrentBuilding != null;
			this.CurrentGovernor = new HeroVM(Settlement.CurrentSettlement.Town.Governor ?? CampaignUIHelper.GetTeleportingGovernor(Settlement.CurrentSettlement, this._teleportationBehavior), true);
			this.UpdateGovernorSelectionProperties();
			this.RefreshCurrentDevelopment();
			this.RefreshTownManagementStats();
			if (Settlement.CurrentSettlement.Town != null)
			{
				foreach (Workshop workshop in Settlement.CurrentSettlement.Town.Workshops)
				{
					WorkshopType workshopType = workshop.WorkshopType;
					if (workshopType != null && !workshopType.IsHidden)
					{
						this.Shops.Add(new TownManagementShopItemVM(workshop));
					}
				}
			}
			foreach (Village village in Settlement.CurrentSettlement.BoundVillages)
			{
				this.Villages.Add(new TownManagementVillageItemVM(village));
			}
			this.ConsumptionTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetSettlementConsumptionTooltip(Settlement.CurrentSettlement));
			this.RefreshValues();
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x00039508 File Offset: 0x00037708
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CurrentProjectText = new TextObject("{=qBq70qDq}Current Project", null).ToString();
			this.CompletionText = new TextObject("{=Rkh2k1OA}Completion:", null).ToString();
			this.ManageText = new TextObject("{=XseYJYka}Manage", null).ToString();
			this.DoneText = new TextObject("{=WiNRdfsm}Done", null).ToString();
			this.WallsText = new TextObject("{=LsZEdD2z}Walls", null).ToString();
			this.VillagesText = GameTexts.FindText("str_bound_village", null).ToString();
			this.ShopsInSettlementText = GameTexts.FindText("str_shops_in_settlement", null).ToString();
			this.GovernorText = GameTexts.FindText("str_sort_by_governor_label", null).ToString();
			this.MiddleFirstTextList.ApplyActionOnAllItems(delegate(TownManagementDescriptionItemVM x)
			{
				x.RefreshValues();
			});
			this.MiddleSecondTextList.ApplyActionOnAllItems(delegate(TownManagementDescriptionItemVM x)
			{
				x.RefreshValues();
			});
			this.ProjectSelection.RefreshValues();
			this.GovernorSelection.RefreshValues();
			this.ReserveControl.RefreshValues();
			this.Shops.ApplyActionOnAllItems(delegate(TownManagementShopItemVM x)
			{
				x.RefreshValues();
			});
			this.Villages.ApplyActionOnAllItems(delegate(TownManagementVillageItemVM x)
			{
				x.RefreshValues();
			});
			this.CurrentGovernor.RefreshValues();
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x000396A0 File Offset: 0x000378A0
		private void RefreshTownManagementStats()
		{
			this.MiddleFirstTextList.Clear();
			this.MiddleSecondTextList.Clear();
			ExplainedNumber taxExplanation = Campaign.Current.Models.SettlementTaxModel.CalculateTownTax(Settlement.CurrentSettlement.Town, true);
			int taxValue = (int)taxExplanation.ResultNumber;
			BasicTooltipViewModel basicTooltipViewModel = new BasicTooltipViewModel(() => CampaignUIHelper.GetTooltipForAccumulatingPropertyWithResult(GameTexts.FindText("str_town_management_population_tax", null).ToString(), (float)taxValue, ref taxExplanation));
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_town_management_population_tax", null));
			this.MiddleFirstTextList.Add(new TownManagementDescriptionItemVM(GameTexts.FindText("str_LEFT_colon", null), taxValue, 0, TownManagementDescriptionItemVM.DescriptionType.Gold, basicTooltipViewModel));
			BasicTooltipViewModel basicTooltipViewModel2 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownProsperityTooltip(Settlement.CurrentSettlement.Town));
			this.MiddleFirstTextList.Add(new TownManagementDescriptionItemVM(GameTexts.FindText("str_town_management_prosperity", null), (int)Settlement.CurrentSettlement.Prosperity, (int)Campaign.Current.Models.SettlementProsperityModel.CalculateProsperityChange(Settlement.CurrentSettlement.Town, false).ResultNumber, TownManagementDescriptionItemVM.DescriptionType.Prosperity, basicTooltipViewModel2));
			BasicTooltipViewModel basicTooltipViewModel3 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownDailyProductionTooltip(Settlement.CurrentSettlement.Town));
			this.MiddleFirstTextList.Add(new TownManagementDescriptionItemVM(GameTexts.FindText("str_town_management_daily_production", null), (int)Campaign.Current.Models.BuildingConstructionModel.CalculateDailyConstructionPower(Settlement.CurrentSettlement.Town, false).ResultNumber, 0, TownManagementDescriptionItemVM.DescriptionType.Production, basicTooltipViewModel3));
			BasicTooltipViewModel basicTooltipViewModel4 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownSecurityTooltip(Settlement.CurrentSettlement.Town));
			this.MiddleFirstTextList.Add(new TownManagementDescriptionItemVM(GameTexts.FindText("str_town_management_security", null), (int)Settlement.CurrentSettlement.Town.Security, (int)Campaign.Current.Models.SettlementSecurityModel.CalculateSecurityChange(Settlement.CurrentSettlement.Town, false).ResultNumber, TownManagementDescriptionItemVM.DescriptionType.Security, basicTooltipViewModel4));
			BasicTooltipViewModel basicTooltipViewModel5 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownLoyaltyTooltip(Settlement.CurrentSettlement.Town));
			this.MiddleSecondTextList.Add(new TownManagementDescriptionItemVM(GameTexts.FindText("str_town_management_loyalty", null), (int)Settlement.CurrentSettlement.Town.Loyalty, (int)Campaign.Current.Models.SettlementLoyaltyModel.CalculateLoyaltyChange(Settlement.CurrentSettlement.Town, false).ResultNumber, TownManagementDescriptionItemVM.DescriptionType.Loyalty, basicTooltipViewModel5));
			BasicTooltipViewModel basicTooltipViewModel6 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownFoodTooltip(Settlement.CurrentSettlement.Town));
			this.MiddleSecondTextList.Add(new TownManagementDescriptionItemVM(GameTexts.FindText("str_town_management_food", null), (int)Settlement.CurrentSettlement.Town.FoodStocks, (int)Campaign.Current.Models.SettlementFoodModel.CalculateTownFoodStocksChange(Settlement.CurrentSettlement.Town, true, false).ResultNumber, TownManagementDescriptionItemVM.DescriptionType.Food, basicTooltipViewModel6));
			BasicTooltipViewModel basicTooltipViewModel7 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownMilitiaTooltip(Settlement.CurrentSettlement.Town));
			this.MiddleSecondTextList.Add(new TownManagementDescriptionItemVM(GameTexts.FindText("str_town_management_militia", null), (int)Settlement.CurrentSettlement.Militia, (int)Campaign.Current.Models.SettlementMilitiaModel.CalculateMilitiaChange(Settlement.CurrentSettlement, false).ResultNumber, TownManagementDescriptionItemVM.DescriptionType.Militia, basicTooltipViewModel7));
			BasicTooltipViewModel basicTooltipViewModel8 = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownGarrisonTooltip(Settlement.CurrentSettlement.Town));
			Collection<TownManagementDescriptionItemVM> middleSecondTextList = this.MiddleSecondTextList;
			TextObject textObject = GameTexts.FindText("str_town_management_garrison", null);
			MobileParty garrisonParty = Settlement.CurrentSettlement.Town.GarrisonParty;
			middleSecondTextList.Add(new TownManagementDescriptionItemVM(textObject, (garrisonParty != null) ? garrisonParty.Party.NumberOfAllMembers : 0, (int)Campaign.Current.Models.SettlementGarrisonModel.CalculateGarrisonChange(Settlement.CurrentSettlement, false).ResultNumber, TownManagementDescriptionItemVM.DescriptionType.Garrison, basicTooltipViewModel8));
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x00039AA0 File Offset: 0x00037CA0
		private void OnChangeInBuildingQueue()
		{
			this.OnProjectSelectionDone();
			this.RefreshTownManagementStats();
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x00039AB0 File Offset: 0x00037CB0
		private void RefreshCurrentDevelopment()
		{
			if (Settlement.CurrentSettlement.Town.CurrentBuilding != null)
			{
				this.IsCurrentProjectDaily = Settlement.CurrentSettlement.Town.CurrentBuilding.BuildingType.IsDefaultProject;
				if (!this.IsCurrentProjectDaily)
				{
					this.CurrentProjectProgress = (int)(BuildingHelper.GetProgressOfBuilding(this.ProjectSelection.CurrentSelectedProject.Building, Settlement.CurrentSettlement.Town) * 100f);
					this.ProjectSelection.CurrentSelectedProject.RefreshProductionText();
				}
			}
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x00039B34 File Offset: 0x00037D34
		private void OnProjectSelectionDone()
		{
			List<Building> localDevelopmentList = this.ProjectSelection.LocalDevelopmentList;
			Building building = this.ProjectSelection.CurrentDailyDefault.Building;
			if (localDevelopmentList != null)
			{
				BuildingHelper.ChangeCurrentBuildingQueue(localDevelopmentList, Settlement.CurrentSettlement.Town);
			}
			if (building != Settlement.CurrentSettlement.Town.CurrentDefaultBuilding)
			{
				BuildingHelper.ChangeDefaultBuilding(building, Settlement.CurrentSettlement.Town);
			}
			this.RefreshCurrentDevelopment();
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x00039B9C File Offset: 0x00037D9C
		private void OnGovernorSelectionDone(Hero selectedGovernor)
		{
			if (selectedGovernor != this.CurrentGovernor.Hero)
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				if (((currentSettlement != null) ? currentSettlement.Town : null) != null)
				{
					this.CurrentGovernor = new HeroVM(selectedGovernor, true);
					ChangeGovernorAction.Apply(Settlement.CurrentSettlement.Town, this.CurrentGovernor.Hero);
					if (selectedGovernor != null)
					{
						MobileParty partyBelongedTo = selectedGovernor.PartyBelongedTo;
						if (partyBelongedTo != null)
						{
							partyBelongedTo.RemoveHeroPerkRole(selectedGovernor);
						}
					}
				}
			}
			this.UpdateGovernorSelectionProperties();
			this.RefreshTownManagementStats();
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x00039C14 File Offset: 0x00037E14
		private void UpdateGovernorSelectionProperties()
		{
			this.HasGovernor = this.CurrentGovernor.Hero != null;
			TextObject textObject;
			this.IsGovernorSelectionEnabled = this.GetCanChangeGovernor(out textObject);
			this.GovernorSelectionDisabledHint = new HintViewModel(textObject, null);
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x00039C50 File Offset: 0x00037E50
		private bool GetCanChangeGovernor(out TextObject disabledReason)
		{
			HeroVM currentGovernor = this.CurrentGovernor;
			bool flag;
			if (currentGovernor == null)
			{
				flag = false;
			}
			else
			{
				Hero hero = currentGovernor.Hero;
				bool? flag2 = ((hero != null) ? new bool?(hero.IsTraveling) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			if (flag)
			{
				disabledReason = new TextObject("{=qbqimqMb}{GOVERNOR.NAME} is on the way to be the new governor of {SETTLEMENT_NAME}", null);
				if (this.CurrentGovernor.Hero.CharacterObject != null)
				{
					StringHelpers.SetCharacterProperties("GOVERNOR", this.CurrentGovernor.Hero.CharacterObject, disabledReason, false);
				}
				TextObject textObject = disabledReason;
				string text = "SETTLEMENT_NAME";
				TextObject name = Settlement.CurrentSettlement.Name;
				textObject.SetTextVariable(text, ((name != null) ? name.ToString() : null) ?? string.Empty);
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x00039D12 File Offset: 0x00037F12
		private void OnReserveUpdated()
		{
			this.RefreshCurrentDevelopment();
			this.RefreshTownManagementStats();
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x00039D20 File Offset: 0x00037F20
		public void ExecuteDone()
		{
			this.OnProjectSelectionDone();
			this.Show = false;
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x00039D2F File Offset: 0x00037F2F
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x00039D42 File Offset: 0x00037F42
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06000E71 RID: 3697 RVA: 0x00039D51 File Offset: 0x00037F51
		// (set) Token: 0x06000E72 RID: 3698 RVA: 0x00039D59 File Offset: 0x00037F59
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

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06000E73 RID: 3699 RVA: 0x00039D77 File Offset: 0x00037F77
		// (set) Token: 0x06000E74 RID: 3700 RVA: 0x00039D7F File Offset: 0x00037F7F
		[DataSourceProperty]
		public string CompletionText
		{
			get
			{
				return this._completionText;
			}
			set
			{
				if (value != this._completionText)
				{
					this._completionText = value;
					base.OnPropertyChangedWithValue<string>(value, "CompletionText");
				}
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06000E75 RID: 3701 RVA: 0x00039DA2 File Offset: 0x00037FA2
		// (set) Token: 0x06000E76 RID: 3702 RVA: 0x00039DAA File Offset: 0x00037FAA
		[DataSourceProperty]
		public string GovernorText
		{
			get
			{
				return this._governorText;
			}
			set
			{
				if (value != this._governorText)
				{
					this._governorText = value;
					base.OnPropertyChangedWithValue<string>(value, "GovernorText");
				}
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06000E77 RID: 3703 RVA: 0x00039DCD File Offset: 0x00037FCD
		// (set) Token: 0x06000E78 RID: 3704 RVA: 0x00039DD5 File Offset: 0x00037FD5
		[DataSourceProperty]
		public string ManageText
		{
			get
			{
				return this._manageText;
			}
			set
			{
				if (value != this._manageText)
				{
					this._manageText = value;
					base.OnPropertyChangedWithValue<string>(value, "ManageText");
				}
			}
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06000E79 RID: 3705 RVA: 0x00039DF8 File Offset: 0x00037FF8
		// (set) Token: 0x06000E7A RID: 3706 RVA: 0x00039E00 File Offset: 0x00038000
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

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06000E7B RID: 3707 RVA: 0x00039E23 File Offset: 0x00038023
		// (set) Token: 0x06000E7C RID: 3708 RVA: 0x00039E2B File Offset: 0x0003802B
		[DataSourceProperty]
		public string WallsText
		{
			get
			{
				return this._wallsText;
			}
			set
			{
				if (value != this._wallsText)
				{
					this._wallsText = value;
					base.OnPropertyChangedWithValue<string>(value, "WallsText");
				}
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06000E7D RID: 3709 RVA: 0x00039E4E File Offset: 0x0003804E
		// (set) Token: 0x06000E7E RID: 3710 RVA: 0x00039E56 File Offset: 0x00038056
		[DataSourceProperty]
		public string CurrentProjectText
		{
			get
			{
				return this._currentProjectText;
			}
			set
			{
				if (value != this._currentProjectText)
				{
					this._currentProjectText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentProjectText");
				}
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06000E7F RID: 3711 RVA: 0x00039E79 File Offset: 0x00038079
		// (set) Token: 0x06000E80 RID: 3712 RVA: 0x00039E81 File Offset: 0x00038081
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

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06000E81 RID: 3713 RVA: 0x00039EA4 File Offset: 0x000380A4
		// (set) Token: 0x06000E82 RID: 3714 RVA: 0x00039EAC File Offset: 0x000380AC
		[DataSourceProperty]
		public bool HasGovernor
		{
			get
			{
				return this._hasGovernor;
			}
			set
			{
				if (value != this._hasGovernor)
				{
					this._hasGovernor = value;
					base.OnPropertyChangedWithValue(value, "HasGovernor");
				}
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06000E83 RID: 3715 RVA: 0x00039ECA File Offset: 0x000380CA
		// (set) Token: 0x06000E84 RID: 3716 RVA: 0x00039ED2 File Offset: 0x000380D2
		[DataSourceProperty]
		public bool IsGovernorSelectionEnabled
		{
			get
			{
				return this._isGovernorSelectionEnabled;
			}
			set
			{
				if (value != this._isGovernorSelectionEnabled)
				{
					this._isGovernorSelectionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsGovernorSelectionEnabled");
				}
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06000E85 RID: 3717 RVA: 0x00039EF0 File Offset: 0x000380F0
		// (set) Token: 0x06000E86 RID: 3718 RVA: 0x00039EF8 File Offset: 0x000380F8
		[DataSourceProperty]
		public bool IsTown
		{
			get
			{
				return this._isTown;
			}
			set
			{
				if (value != this._isTown)
				{
					this._isTown = value;
					base.OnPropertyChangedWithValue(value, "IsTown");
				}
			}
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06000E87 RID: 3719 RVA: 0x00039F16 File Offset: 0x00038116
		// (set) Token: 0x06000E88 RID: 3720 RVA: 0x00039F1E File Offset: 0x0003811E
		[DataSourceProperty]
		public bool Show
		{
			get
			{
				return this._show;
			}
			set
			{
				if (value != this._show)
				{
					this._show = value;
					base.OnPropertyChangedWithValue(value, "Show");
				}
			}
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06000E89 RID: 3721 RVA: 0x00039F3C File Offset: 0x0003813C
		// (set) Token: 0x06000E8A RID: 3722 RVA: 0x00039F44 File Offset: 0x00038144
		[DataSourceProperty]
		public bool IsThereCurrentProject
		{
			get
			{
				return this._isThereCurrentProject;
			}
			set
			{
				if (value != this._isThereCurrentProject)
				{
					this._isThereCurrentProject = value;
					base.OnPropertyChangedWithValue(value, "IsThereCurrentProject");
				}
			}
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06000E8B RID: 3723 RVA: 0x00039F62 File Offset: 0x00038162
		// (set) Token: 0x06000E8C RID: 3724 RVA: 0x00039F6A File Offset: 0x0003816A
		[DataSourceProperty]
		public bool IsSelectingGovernor
		{
			get
			{
				return this._isSelectingGovernor;
			}
			set
			{
				if (value != this._isSelectingGovernor)
				{
					this._isSelectingGovernor = value;
					base.OnPropertyChangedWithValue(value, "IsSelectingGovernor");
				}
			}
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06000E8D RID: 3725 RVA: 0x00039F88 File Offset: 0x00038188
		// (set) Token: 0x06000E8E RID: 3726 RVA: 0x00039F90 File Offset: 0x00038190
		[DataSourceProperty]
		public MBBindingList<TownManagementDescriptionItemVM> MiddleFirstTextList
		{
			get
			{
				return this._middleLeftTextList;
			}
			set
			{
				if (value != this._middleLeftTextList)
				{
					this._middleLeftTextList = value;
					base.OnPropertyChanged("MiddleLeftTextList");
				}
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06000E8F RID: 3727 RVA: 0x00039FAD File Offset: 0x000381AD
		// (set) Token: 0x06000E90 RID: 3728 RVA: 0x00039FB5 File Offset: 0x000381B5
		[DataSourceProperty]
		public MBBindingList<TownManagementDescriptionItemVM> MiddleSecondTextList
		{
			get
			{
				return this._middleRightTextList;
			}
			set
			{
				if (value != this._middleRightTextList)
				{
					this._middleRightTextList = value;
					base.OnPropertyChanged("MiddleRightTextList");
				}
			}
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06000E91 RID: 3729 RVA: 0x00039FD2 File Offset: 0x000381D2
		// (set) Token: 0x06000E92 RID: 3730 RVA: 0x00039FDA File Offset: 0x000381DA
		[DataSourceProperty]
		public MBBindingList<TownManagementShopItemVM> Shops
		{
			get
			{
				return this._shops;
			}
			set
			{
				if (value != this._shops)
				{
					this._shops = value;
					base.OnPropertyChangedWithValue<MBBindingList<TownManagementShopItemVM>>(value, "Shops");
				}
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06000E93 RID: 3731 RVA: 0x00039FF8 File Offset: 0x000381F8
		// (set) Token: 0x06000E94 RID: 3732 RVA: 0x0003A000 File Offset: 0x00038200
		[DataSourceProperty]
		public MBBindingList<TownManagementVillageItemVM> Villages
		{
			get
			{
				return this._villages;
			}
			set
			{
				if (value != this._villages)
				{
					this._villages = value;
					base.OnPropertyChangedWithValue<MBBindingList<TownManagementVillageItemVM>>(value, "Villages");
				}
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06000E95 RID: 3733 RVA: 0x0003A01E File Offset: 0x0003821E
		// (set) Token: 0x06000E96 RID: 3734 RVA: 0x0003A026 File Offset: 0x00038226
		[DataSourceProperty]
		public HintViewModel GovernorSelectionDisabledHint
		{
			get
			{
				return this._governorSelectionDisabledHint;
			}
			set
			{
				if (value != this._governorSelectionDisabledHint)
				{
					this._governorSelectionDisabledHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "GovernorSelectionDisabledHint");
				}
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06000E97 RID: 3735 RVA: 0x0003A044 File Offset: 0x00038244
		// (set) Token: 0x06000E98 RID: 3736 RVA: 0x0003A04C File Offset: 0x0003824C
		[DataSourceProperty]
		public string VillagesText
		{
			get
			{
				return this._villagesText;
			}
			set
			{
				if (value != this._villagesText)
				{
					this._villagesText = value;
					base.OnPropertyChangedWithValue<string>(value, "VillagesText");
				}
			}
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06000E99 RID: 3737 RVA: 0x0003A06F File Offset: 0x0003826F
		// (set) Token: 0x06000E9A RID: 3738 RVA: 0x0003A077 File Offset: 0x00038277
		[DataSourceProperty]
		public string ShopsInSettlementText
		{
			get
			{
				return this._shopsInSettlementText;
			}
			set
			{
				if (value != this._shopsInSettlementText)
				{
					this._shopsInSettlementText = value;
					base.OnPropertyChangedWithValue<string>(value, "ShopsInSettlementText");
				}
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06000E9B RID: 3739 RVA: 0x0003A09A File Offset: 0x0003829A
		// (set) Token: 0x06000E9C RID: 3740 RVA: 0x0003A0A2 File Offset: 0x000382A2
		[DataSourceProperty]
		public bool IsCurrentProjectDaily
		{
			get
			{
				return this._isCurrentProjectDaily;
			}
			set
			{
				if (value != this._isCurrentProjectDaily)
				{
					this._isCurrentProjectDaily = value;
					base.OnPropertyChangedWithValue(value, "IsCurrentProjectDaily");
				}
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06000E9D RID: 3741 RVA: 0x0003A0C0 File Offset: 0x000382C0
		// (set) Token: 0x06000E9E RID: 3742 RVA: 0x0003A0C8 File Offset: 0x000382C8
		[DataSourceProperty]
		public int CurrentProjectProgress
		{
			get
			{
				return this._currentProjectProgress;
			}
			set
			{
				if (value != this._currentProjectProgress)
				{
					this._currentProjectProgress = value;
					base.OnPropertyChangedWithValue(value, "CurrentProjectProgress");
				}
			}
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06000E9F RID: 3743 RVA: 0x0003A0E6 File Offset: 0x000382E6
		// (set) Token: 0x06000EA0 RID: 3744 RVA: 0x0003A0EE File Offset: 0x000382EE
		[DataSourceProperty]
		public SettlementProjectSelectionVM ProjectSelection
		{
			get
			{
				return this._projectSelection;
			}
			set
			{
				if (value != this._projectSelection)
				{
					this._projectSelection = value;
					base.OnPropertyChangedWithValue<SettlementProjectSelectionVM>(value, "ProjectSelection");
				}
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06000EA1 RID: 3745 RVA: 0x0003A10C File Offset: 0x0003830C
		// (set) Token: 0x06000EA2 RID: 3746 RVA: 0x0003A114 File Offset: 0x00038314
		[DataSourceProperty]
		public SettlementGovernorSelectionVM GovernorSelection
		{
			get
			{
				return this._governorSelection;
			}
			set
			{
				if (value != this._governorSelection)
				{
					this._governorSelection = value;
					base.OnPropertyChangedWithValue<SettlementGovernorSelectionVM>(value, "GovernorSelection");
				}
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06000EA3 RID: 3747 RVA: 0x0003A132 File Offset: 0x00038332
		// (set) Token: 0x06000EA4 RID: 3748 RVA: 0x0003A13A File Offset: 0x0003833A
		[DataSourceProperty]
		public TownManagementReserveControlVM ReserveControl
		{
			get
			{
				return this._reserveControl;
			}
			set
			{
				if (value != this._reserveControl)
				{
					this._reserveControl = value;
					base.OnPropertyChangedWithValue<TownManagementReserveControlVM>(value, "ReserveControl");
				}
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06000EA5 RID: 3749 RVA: 0x0003A158 File Offset: 0x00038358
		// (set) Token: 0x06000EA6 RID: 3750 RVA: 0x0003A160 File Offset: 0x00038360
		[DataSourceProperty]
		public HeroVM CurrentGovernor
		{
			get
			{
				return this._currentGovernor;
			}
			set
			{
				if (value != this._currentGovernor)
				{
					this._currentGovernor = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "CurrentGovernor");
				}
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06000EA7 RID: 3751 RVA: 0x0003A17E File Offset: 0x0003837E
		// (set) Token: 0x06000EA8 RID: 3752 RVA: 0x0003A186 File Offset: 0x00038386
		[DataSourceProperty]
		public BasicTooltipViewModel ConsumptionTooltip
		{
			get
			{
				return this._consumptionTooltip;
			}
			set
			{
				if (value != this._consumptionTooltip)
				{
					this._consumptionTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ConsumptionTooltip");
				}
			}
		}

		// Token: 0x040006AF RID: 1711
		private ITeleportationCampaignBehavior _teleportationBehavior;

		// Token: 0x040006B0 RID: 1712
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040006B1 RID: 1713
		private bool _isThereCurrentProject;

		// Token: 0x040006B2 RID: 1714
		private bool _isSelectingGovernor;

		// Token: 0x040006B3 RID: 1715
		private SettlementProjectSelectionVM _projectSelection;

		// Token: 0x040006B4 RID: 1716
		private SettlementGovernorSelectionVM _governorSelection;

		// Token: 0x040006B5 RID: 1717
		private TownManagementReserveControlVM _reserveControl;

		// Token: 0x040006B6 RID: 1718
		private MBBindingList<TownManagementDescriptionItemVM> _middleLeftTextList;

		// Token: 0x040006B7 RID: 1719
		private MBBindingList<TownManagementDescriptionItemVM> _middleRightTextList;

		// Token: 0x040006B8 RID: 1720
		private MBBindingList<TownManagementShopItemVM> _shops;

		// Token: 0x040006B9 RID: 1721
		private MBBindingList<TownManagementVillageItemVM> _villages;

		// Token: 0x040006BA RID: 1722
		private HintViewModel _governorSelectionDisabledHint;

		// Token: 0x040006BB RID: 1723
		private bool _show;

		// Token: 0x040006BC RID: 1724
		private bool _isTown;

		// Token: 0x040006BD RID: 1725
		private bool _hasGovernor;

		// Token: 0x040006BE RID: 1726
		private bool _isGovernorSelectionEnabled;

		// Token: 0x040006BF RID: 1727
		private string _titleText;

		// Token: 0x040006C0 RID: 1728
		private bool _isCurrentProjectDaily;

		// Token: 0x040006C1 RID: 1729
		private int _currentProjectProgress;

		// Token: 0x040006C2 RID: 1730
		private string _currentProjectText;

		// Token: 0x040006C3 RID: 1731
		private HeroVM _currentGovernor;

		// Token: 0x040006C4 RID: 1732
		private string _manageText;

		// Token: 0x040006C5 RID: 1733
		private string _doneText;

		// Token: 0x040006C6 RID: 1734
		private string _wallsText;

		// Token: 0x040006C7 RID: 1735
		private string _completionText;

		// Token: 0x040006C8 RID: 1736
		private string _villagesText;

		// Token: 0x040006C9 RID: 1737
		private string _shopsInSettlementText;

		// Token: 0x040006CA RID: 1738
		private BasicTooltipViewModel _consumptionTooltip;

		// Token: 0x040006CB RID: 1739
		private string _governorText;
	}
}
