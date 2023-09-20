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
	public class TownManagementVM : ViewModel
	{
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

		private void OnChangeInBuildingQueue()
		{
			this.OnProjectSelectionDone();
			this.RefreshTownManagementStats();
		}

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

		private void UpdateGovernorSelectionProperties()
		{
			this.HasGovernor = this.CurrentGovernor.Hero != null;
			TextObject textObject;
			this.IsGovernorSelectionEnabled = this.GetCanChangeGovernor(out textObject);
			this.GovernorSelectionDisabledHint = new HintViewModel(textObject, null);
		}

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

		private void OnReserveUpdated()
		{
			this.RefreshCurrentDevelopment();
			this.RefreshTownManagementStats();
		}

		public void ExecuteDone()
		{
			this.OnProjectSelectionDone();
			this.Show = false;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
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

		private ITeleportationCampaignBehavior _teleportationBehavior;

		private InputKeyItemVM _doneInputKey;

		private bool _isThereCurrentProject;

		private bool _isSelectingGovernor;

		private SettlementProjectSelectionVM _projectSelection;

		private SettlementGovernorSelectionVM _governorSelection;

		private TownManagementReserveControlVM _reserveControl;

		private MBBindingList<TownManagementDescriptionItemVM> _middleLeftTextList;

		private MBBindingList<TownManagementDescriptionItemVM> _middleRightTextList;

		private MBBindingList<TownManagementShopItemVM> _shops;

		private MBBindingList<TownManagementVillageItemVM> _villages;

		private HintViewModel _governorSelectionDisabledHint;

		private bool _show;

		private bool _isTown;

		private bool _hasGovernor;

		private bool _isGovernorSelectionEnabled;

		private string _titleText;

		private bool _isCurrentProjectDaily;

		private int _currentProjectProgress;

		private string _currentProjectText;

		private HeroVM _currentGovernor;

		private string _manageText;

		private string _doneText;

		private string _wallsText;

		private string _completionText;

		private string _villagesText;

		private string _shopsInSettlementText;

		private BasicTooltipViewModel _consumptionTooltip;

		private string _governorText;
	}
}
