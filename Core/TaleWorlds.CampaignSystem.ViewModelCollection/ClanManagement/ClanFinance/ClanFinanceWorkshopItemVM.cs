using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance
{
	public class ClanFinanceWorkshopItemVM : ClanFinanceIncomeItemBaseVM
	{
		public Workshop Workshop { get; private set; }

		public ClanFinanceWorkshopItemVM(Workshop workshop, Action<ClanFinanceWorkshopItemVM> onSelection, Action onRefresh, Action<ClanCardSelectionInfo> openCardSelectionPopup)
			: base(null, onRefresh)
		{
			this._workshopWarehouseBehavior = Campaign.Current.GetCampaignBehavior<IWorkshopWarehouseCampaignBehavior>();
			this.Workshop = workshop;
			this._openCardSelectionPopup = openCardSelectionPopup;
			this._workshopModel = Campaign.Current.Models.WorkshopModel;
			base.IncomeTypeAsEnum = IncomeTypes.Workshop;
			this._onSelection = new Action<ClanFinanceIncomeItemBaseVM>(this.tempOnSelection);
			this._onSelectionT = onSelection;
			SettlementComponent settlementComponent = this.Workshop.Settlement.SettlementComponent;
			base.ImageName = ((settlementComponent != null) ? settlementComponent.WaitMeshName : "");
			this.ManageWorkshopHint = new HintViewModel(new TextObject("{=LxWVtDF0}Manage Workshop", null), null);
			this.UseWarehouseAsInputHint = new HintViewModel(new TextObject("{=a4oqWgUi}If there are no raw materials in the warehouse, the workshop will buy raw materials from the market until the warehouse is restocked", null), null);
			this.StoreOutputPercentageHint = new HintViewModel(new TextObject("{=NVUi4bB9}When the warehouse is full, the workshop will sell the products to the town market", null), null);
			this.InputWarehouseCountsTooltip = new BasicTooltipViewModel();
			this.OutputWarehouseCountsTooltip = new BasicTooltipViewModel();
			this.ReceiveInputFromWarehouse = this._workshopWarehouseBehavior.IsGettingInputsFromWarehouse(workshop);
			this.WarehousePercentageSelector = new SelectorVM<WorkshopPercentageSelectorItemVM>(0, new Action<SelectorVM<WorkshopPercentageSelectorItemVM>>(this.OnStoreOutputInWarehousePercentageUpdated));
			this.RefreshStoragePercentages();
			float currentPercentage = this._workshopWarehouseBehavior.GetStockProductionInWarehouseRatio(workshop);
			WorkshopPercentageSelectorItemVM workshopPercentageSelectorItemVM = this.WarehousePercentageSelector.ItemList.FirstOrDefault((WorkshopPercentageSelectorItemVM x) => x.Percentage.ApproximatelyEqualsTo(currentPercentage, 0.1f));
			this.WarehousePercentageSelector.SelectedIndex = ((workshopPercentageSelectorItemVM != null) ? this.WarehousePercentageSelector.ItemList.IndexOf(workshopPercentageSelectorItemVM) : 0);
			this.RefreshValues();
		}

		private void tempOnSelection(ClanFinanceIncomeItemBaseVM temp)
		{
			this._onSelectionT(this);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			base.Name = this.Workshop.WorkshopType.Name.ToString();
			this.WorkshopTypeId = this.Workshop.WorkshopType.StringId;
			base.Location = this.Workshop.Settlement.Name.ToString();
			base.Income = (int)((float)this.Workshop.ProfitMade * (1f / Campaign.Current.Models.ClanFinanceModel.RevenueSmoothenFraction()));
			base.IncomeValueText = base.DetermineIncomeText(base.Income);
			this.InputsText = GameTexts.FindText("str_clan_workshop_inputs", null).ToString();
			this.OutputsText = GameTexts.FindText("str_clan_workshop_outputs", null).ToString();
			this.StoreOutputPercentageText = new TextObject("{=y6qCNFQj}Store Outputs in the Warehouse", null).ToString();
			this.UseWarehouseAsInputText = new TextObject("{=88WPmTKH}Get Input from the Warehouse", null).ToString();
			this.WarehouseCapacityText = new TextObject("{=X6eG4Q5V}Warehouse Capacity", null).ToString();
			float warehouseItemRosterWeight = this._workshopWarehouseBehavior.GetWarehouseItemRosterWeight(this.Workshop.Settlement);
			int warehouseCapacity = Campaign.Current.Models.WorkshopModel.WarehouseCapacity;
			this.WarehouseCapacityValue = GameTexts.FindText("str_LEFT_over_RIGHT", null).SetTextVariable("LEFT", warehouseItemRosterWeight).SetTextVariable("RIGHT", warehouseCapacity)
				.ToString();
			this.WarehouseInputAmount = this._workshopWarehouseBehavior.GetInputCount(this.Workshop);
			this.WarehouseOutputAmount = this._workshopWarehouseBehavior.GetOutputCount(this.Workshop);
			this._inputDetails = this._workshopWarehouseBehavior.GetInputDailyChange(this.Workshop);
			this._outputDetails = this._workshopWarehouseBehavior.GetOutputDailyChange(this.Workshop);
			this.InputWarehouseCountsTooltip.SetToolipCallback(() => this.GetWarehouseInputOutputTooltip(true));
			this.OutputWarehouseCountsTooltip.SetToolipCallback(() => this.GetWarehouseInputOutputTooltip(false));
			base.ItemProperties.Clear();
			this.PopulateStatsList();
		}

		private List<TooltipProperty> GetWarehouseInputOutputTooltip(bool isInput)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			ExplainedNumber explainedNumber = (isInput ? this._inputDetails : this._outputDetails);
			if (!explainedNumber.ResultNumber.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				list.Add(new TooltipProperty(new TextObject("{=Y9egTJg0}Daily Change", null).ToString(), "", 1, false, TooltipProperty.TooltipPropertyFlags.Title));
				list.Add(new TooltipProperty("", "", 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
				foreach (ValueTuple<string, float> valueTuple in explainedNumber.GetLines())
				{
					string text = GameTexts.FindText("str_clan_workshop_material_daily_Change", null).SetTextVariable("CHANGE", MathF.Abs(valueTuple.Item2).ToString("F1")).SetTextVariable("IS_POSITIVE", (valueTuple.Item2 > 0f) ? 1 : 0)
						.ToString();
					list.Add(new TooltipProperty(valueTuple.Item1, text, 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			return list;
		}

		private void RefreshStoragePercentages()
		{
			this.WarehousePercentageSelector.ItemList.Clear();
			TextObject textObject = GameTexts.FindText("str_NUMBER_percent", null);
			textObject.SetTextVariable("NUMBER", 0);
			this.WarehousePercentageSelector.AddItem(new WorkshopPercentageSelectorItemVM(textObject.ToString(), 0f));
			textObject.SetTextVariable("NUMBER", 25);
			this.WarehousePercentageSelector.AddItem(new WorkshopPercentageSelectorItemVM(textObject.ToString(), 0.25f));
			textObject.SetTextVariable("NUMBER", 50);
			this.WarehousePercentageSelector.AddItem(new WorkshopPercentageSelectorItemVM(textObject.ToString(), 0.5f));
			textObject.SetTextVariable("NUMBER", 75);
			this.WarehousePercentageSelector.AddItem(new WorkshopPercentageSelectorItemVM(textObject.ToString(), 0.75f));
			textObject.SetTextVariable("NUMBER", 100);
			this.WarehousePercentageSelector.AddItem(new WorkshopPercentageSelectorItemVM(textObject.ToString(), 1f));
		}

		public void ExecuteToggleWarehouseUsage()
		{
			this.ReceiveInputFromWarehouse = !this.ReceiveInputFromWarehouse;
			this._workshopWarehouseBehavior.SetIsGettingInputsFromWarehouse(this.Workshop, this.ReceiveInputFromWarehouse);
			base.ItemProperties.Clear();
			this.PopulateStatsList();
		}

		protected override void PopulateStatsList()
		{
			ValueTuple<TextObject, bool, BasicTooltipViewModel> workshopStatus = this.GetWorkshopStatus(this.Workshop);
			if (!TextObject.IsNullOrEmpty(workshopStatus.Item1))
			{
				base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=DXczLzml}Status", null).ToString(), workshopStatus.Item1.ToString(), workshopStatus.Item2, workshopStatus.Item3));
			}
			SelectableItemPropertyVM currentCapitalProperty = this.GetCurrentCapitalProperty();
			base.ItemProperties.Add(currentCapitalProperty);
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=CaRbMaZY}Daily Wage", null).ToString(), this.Workshop.Expense.ToString(), false, null));
			TextObject textObject;
			TextObject textObject2;
			ClanFinanceWorkshopItemVM.GetWorkshopTypeProductionTexts(this.Workshop.WorkshopType, out textObject, out textObject2);
			this.InputProducts = textObject.ToString();
			this.OutputProducts = textObject2.ToString();
		}

		private SelectableItemPropertyVM GetCurrentCapitalProperty()
		{
			string text3 = new TextObject("{=Ra17aK4e}Current Capital", null).ToString();
			string text2 = this.Workshop.Capital.ToString();
			bool flag = false;
			BasicTooltipViewModel basicTooltipViewModel;
			if (this.Workshop.Capital < this._workshopModel.CapitalLowLimit)
			{
				flag = true;
				basicTooltipViewModel = new BasicTooltipViewModel(() => new TextObject("{=Qu5clctb}The workshop is losing money. The expenses are being paid from your treasury because the workshop's capital is below {LOWER_THRESHOLD} denars", null).SetTextVariable("LOWER_THRESHOLD", this._workshopModel.CapitalLowLimit).ToString());
			}
			else
			{
				TextObject text = new TextObject("{=dEMUqz2Y}This workshop will send 20% of its profits above {INITIAL_CAPITAL} capital to your treasury", null);
				text.SetTextVariable("INITIAL_CAPITAL", Campaign.Current.Models.WorkshopModel.InitialCapital);
				basicTooltipViewModel = new BasicTooltipViewModel(() => text.ToString());
			}
			return new SelectableItemPropertyVM(text3, text2, flag, basicTooltipViewModel);
		}

		[return: TupleElementNames(new string[] { "Status", "IsWarning", "Hint" })]
		private ValueTuple<TextObject, bool, BasicTooltipViewModel> GetWorkshopStatus(Workshop workshop)
		{
			TextObject textObject = TextObject.Empty;
			bool flag = false;
			BasicTooltipViewModel basicTooltipViewModel = null;
			if (workshop.LastRunCampaignTime.ElapsedDaysUntilNow >= 1f)
			{
				textObject = this._haltedText;
				flag = true;
				TextObject tooltipText = TextObject.Empty;
				if (!this._workshopWarehouseBehavior.IsRawMaterialsSufficientInTownMarket(workshop))
				{
					tooltipText = this._noRawMaterialsText;
				}
				else if (this.WarehousePercentageSelector.SelectedItem.Percentage < 1f)
				{
					tooltipText = this._noProfitText;
				}
				int num = (int)workshop.LastRunCampaignTime.ElapsedDaysUntilNow;
				tooltipText.SetTextVariable("DAY", num);
				tooltipText.SetTextVariable("PLURAL_DAYS", (num == 1) ? "0" : "1");
				basicTooltipViewModel = new BasicTooltipViewModel(() => tooltipText.ToString());
			}
			else
			{
				textObject = this._runningText;
			}
			return new ValueTuple<TextObject, bool, BasicTooltipViewModel>(textObject, flag, basicTooltipViewModel);
		}

		private static void GetWorkshopTypeProductionTexts(WorkshopType workshopType, out TextObject inputsText, out TextObject outputsText)
		{
			CampaignUIHelper.ProductInputOutputEqualityComparer productInputOutputEqualityComparer = new CampaignUIHelper.ProductInputOutputEqualityComparer();
			IEnumerable<TextObject> enumerable = from x in workshopType.Productions.SelectMany((WorkshopType.Production p) => p.Inputs).Distinct(productInputOutputEqualityComparer)
				select x.Item1.GetName();
			IEnumerable<TextObject> enumerable2 = from x in workshopType.Productions.SelectMany((WorkshopType.Production p) => p.Outputs).Distinct(productInputOutputEqualityComparer)
				select x.Item1.GetName();
			inputsText = CampaignUIHelper.GetCommaSeparatedText(null, enumerable);
			outputsText = CampaignUIHelper.GetCommaSeparatedText(null, enumerable2);
		}

		public void ExecuteBeginWorkshopHint()
		{
			if (this.Workshop.WorkshopType != null)
			{
				InformationManager.ShowTooltip(typeof(Workshop), new object[] { this.Workshop });
			}
		}

		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		public void OnStoreOutputInWarehousePercentageUpdated(SelectorVM<WorkshopPercentageSelectorItemVM> selector)
		{
			if (selector.SelectedIndex != -1)
			{
				this._workshopWarehouseBehavior.SetStockProductionInWarehouseRatio(this.Workshop, selector.SelectedItem.Percentage);
				this._inputDetails = this._workshopWarehouseBehavior.GetInputDailyChange(this.Workshop);
				this._outputDetails = this._workshopWarehouseBehavior.GetOutputDailyChange(this.Workshop);
			}
		}

		public void ExecuteManageWorkshop()
		{
			TextObject textObject = new TextObject("{=LxWVtDF0}Manage Workshop", null);
			ClanCardSelectionInfo clanCardSelectionInfo = new ClanCardSelectionInfo(textObject, this.GetManageWorkshopItems(), new Action<List<object>, Action>(this.OnManageWorkshopDone), false);
			Action<ClanCardSelectionInfo> openCardSelectionPopup = this._openCardSelectionPopup;
			if (openCardSelectionPopup == null)
			{
				return;
			}
			openCardSelectionPopup(clanCardSelectionInfo);
		}

		private IEnumerable<ClanCardSelectionItemInfo> GetManageWorkshopItems()
		{
			int costForNotable = this._workshopModel.GetCostForNotable(this.Workshop);
			TextObject textObject = new TextObject("{=ysireFjT}Sell This Workshop for {GOLD_AMOUNT}{GOLD_ICON}", null);
			textObject.SetTextVariable("GOLD_AMOUNT", costForNotable);
			textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			yield return new ClanCardSelectionItemInfo(textObject, false, null, ClanCardSelectionItemPropertyInfo.CreateActionGoldChangeText(costForNotable));
			int costOfChangingType = this._workshopModel.GetConvertProductionCost(this.Workshop.WorkshopType);
			TextObject cannotChangeTypeReason = new TextObject("{=av51ur2M}You need at least {REQUIRED_AMOUNT} denars to change the production type of this workshop.", null);
			cannotChangeTypeReason.SetTextVariable("REQUIRED_AMOUNT", costOfChangingType);
			foreach (WorkshopType workshopType in WorkshopType.All)
			{
				if (this.Workshop.WorkshopType != workshopType && !workshopType.IsHidden)
				{
					TextObject name = workshopType.Name;
					bool flag = costOfChangingType <= Hero.MainHero.Gold;
					yield return new ClanCardSelectionItemInfo(workshopType, name, null, CardSelectionItemSpriteType.Workshop, workshopType.StringId, null, this.GetWorkshopItemProperties(workshopType), !flag, cannotChangeTypeReason, ClanCardSelectionItemPropertyInfo.CreateActionGoldChangeText(-costOfChangingType));
				}
			}
			List<WorkshopType>.Enumerator enumerator = default(List<WorkshopType>.Enumerator);
			yield break;
			yield break;
		}

		private IEnumerable<ClanCardSelectionItemPropertyInfo> GetWorkshopItemProperties(WorkshopType workshopType)
		{
			Workshop workshop = this.Workshop;
			int? num;
			if (workshop == null)
			{
				num = null;
			}
			else
			{
				Settlement settlement = workshop.Settlement;
				if (settlement == null)
				{
					num = null;
				}
				else
				{
					Town town = settlement.Town;
					if (town == null)
					{
						num = null;
					}
					else
					{
						Workshop[] workshops = town.Workshops;
						num = ((workshops != null) ? new int?(workshops.Count((Workshop x) => ((x != null) ? x.WorkshopType : null) == workshopType)) : null);
					}
				}
			}
			int num2 = num ?? 0;
			TextObject textObject = ((num2 == 0) ? new TextObject("{=gu5xmV0E}No other {WORKSHOP_NAME} in this town.", null) : new TextObject("{=lhIpaGt9}There {?(COUNT > 1)}are{?}is{\\?} {COUNT} more {?(COUNT > 1)}{PLURAL(WORKSHOP_NAME)}{?}{WORKSHOP_NAME}{\\?} in this town.", null));
			textObject.SetTextVariable("WORKSHOP_NAME", workshopType.Name);
			textObject.SetTextVariable("COUNT", num2);
			TextObject inputsText;
			TextObject outputsText;
			ClanFinanceWorkshopItemVM.GetWorkshopTypeProductionTexts(workshopType, out inputsText, out outputsText);
			yield return new ClanCardSelectionItemPropertyInfo(textObject);
			yield return new ClanCardSelectionItemPropertyInfo(ClanCardSelectionItemPropertyInfo.CreateLabeledValueText(new TextObject("{=XCz81XYm}Inputs", null), inputsText));
			yield return new ClanCardSelectionItemPropertyInfo(ClanCardSelectionItemPropertyInfo.CreateLabeledValueText(new TextObject("{=ErnykQEH}Outputs", null), outputsText));
			yield break;
		}

		private void OnManageWorkshopDone(List<object> selectedItems, Action closePopup)
		{
			if (closePopup != null)
			{
				closePopup();
			}
			if (selectedItems.Count == 1)
			{
				WorkshopType workshopType = (WorkshopType)selectedItems[0];
				if (workshopType == null)
				{
					if (this.Workshop.Settlement.Town.Workshops.Count((Workshop x) => x.Owner == Hero.MainHero) == 1)
					{
						bool flag = Hero.MainHero.CurrentSettlement == this.Workshop.Settlement;
						InformationManager.ShowInquiry(new InquiryData(new TextObject("{=HiJTlBgF}Sell Workshop", null).ToString(), flag ? new TextObject("{=s06mScpJ}If you have goods in the warehouse, they will be transferred to your party. Are you sure?", null).ToString() : new TextObject("{=yuxBDKgM}If you have goods in the warehouse, they will be lost! Are you sure?", null).ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), new Action(this.ExecuteSellWorkshop), null, "", 0f, null, null, null), false, false);
					}
					else
					{
						this.ExecuteSellWorkshop();
					}
				}
				else
				{
					ChangeProductionTypeOfWorkshopAction.Apply(this.Workshop, workshopType, false);
				}
				Action onRefresh = this._onRefresh;
				if (onRefresh == null)
				{
					return;
				}
				onRefresh();
			}
		}

		private void ExecuteSellWorkshop()
		{
			Hero notableOwnerForWorkshop = Campaign.Current.Models.WorkshopModel.GetNotableOwnerForWorkshop(this.Workshop.Settlement);
			ChangeOwnerOfWorkshopAction.ApplyByPlayerSelling(this.Workshop, notableOwnerForWorkshop, this.Workshop.WorkshopType);
			Action onRefresh = this._onRefresh;
			if (onRefresh == null)
			{
				return;
			}
			onRefresh();
		}

		[DataSourceProperty]
		public HintViewModel UseWarehouseAsInputHint
		{
			get
			{
				return this._useWarehouseAsInputHint;
			}
			set
			{
				if (value != this._useWarehouseAsInputHint)
				{
					this._useWarehouseAsInputHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "UseWarehouseAsInputHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel StoreOutputPercentageHint
		{
			get
			{
				return this._storeOutputPercentageHint;
			}
			set
			{
				if (value != this._storeOutputPercentageHint)
				{
					this._storeOutputPercentageHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "StoreOutputPercentageHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ManageWorkshopHint
		{
			get
			{
				return this._manageWorkshopHint;
			}
			set
			{
				if (value != this._manageWorkshopHint)
				{
					this._manageWorkshopHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ManageWorkshopHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel InputWarehouseCountsTooltip
		{
			get
			{
				return this._inputWarehouseCountsTooltip;
			}
			set
			{
				if (value != this._inputWarehouseCountsTooltip)
				{
					this._inputWarehouseCountsTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "InputWarehouseCountsTooltip");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel OutputWarehouseCountsTooltip
		{
			get
			{
				return this._outputWarehouseCountsTooltip;
			}
			set
			{
				if (value != this._outputWarehouseCountsTooltip)
				{
					this._outputWarehouseCountsTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "OutputWarehouseCountsTooltip");
				}
			}
		}

		public string WorkshopTypeId
		{
			get
			{
				return this._workshopTypeId;
			}
			set
			{
				if (value != this._workshopTypeId)
				{
					this._workshopTypeId = value;
					base.OnPropertyChangedWithValue<string>(value, "WorkshopTypeId");
				}
			}
		}

		public string InputsText
		{
			get
			{
				return this._inputsText;
			}
			set
			{
				if (value != this._inputsText)
				{
					this._inputsText = value;
					base.OnPropertyChangedWithValue<string>(value, "InputsText");
				}
			}
		}

		public string OutputsText
		{
			get
			{
				return this._outputsText;
			}
			set
			{
				if (value != this._outputsText)
				{
					this._outputsText = value;
					base.OnPropertyChangedWithValue<string>(value, "OutputsText");
				}
			}
		}

		public string InputProducts
		{
			get
			{
				return this._inputProducts;
			}
			set
			{
				if (value != this._inputProducts)
				{
					this._inputProducts = value;
					base.OnPropertyChangedWithValue<string>(value, "InputProducts");
				}
			}
		}

		public string OutputProducts
		{
			get
			{
				return this._outputProducts;
			}
			set
			{
				if (value != this._outputProducts)
				{
					this._outputProducts = value;
					base.OnPropertyChangedWithValue<string>(value, "OutputProducts");
				}
			}
		}

		public string UseWarehouseAsInputText
		{
			get
			{
				return this._useWarehouseAsInputText;
			}
			set
			{
				if (value != this._useWarehouseAsInputText)
				{
					this._useWarehouseAsInputText = value;
					base.OnPropertyChangedWithValue<string>(value, "UseWarehouseAsInputText");
				}
			}
		}

		public string StoreOutputPercentageText
		{
			get
			{
				return this._storeOutputPercentageText;
			}
			set
			{
				if (value != this._storeOutputPercentageText)
				{
					this._storeOutputPercentageText = value;
					base.OnPropertyChangedWithValue<string>(value, "StoreOutputPercentageText");
				}
			}
		}

		public string WarehouseCapacityText
		{
			get
			{
				return this._warehouseCapacityText;
			}
			set
			{
				if (value != this._warehouseCapacityText)
				{
					this._warehouseCapacityText = value;
					base.OnPropertyChangedWithValue<string>(value, "WarehouseCapacityText");
				}
			}
		}

		public string WarehouseCapacityValue
		{
			get
			{
				return this._warehouseCapacityValue;
			}
			set
			{
				if (value != this._warehouseCapacityValue)
				{
					this._warehouseCapacityValue = value;
					base.OnPropertyChangedWithValue<string>(value, "WarehouseCapacityValue");
				}
			}
		}

		public bool ReceiveInputFromWarehouse
		{
			get
			{
				return this._receiveInputFromWarehouse;
			}
			set
			{
				if (value != this._receiveInputFromWarehouse)
				{
					this._receiveInputFromWarehouse = value;
					base.OnPropertyChangedWithValue(value, "ReceiveInputFromWarehouse");
				}
			}
		}

		public int WarehouseInputAmount
		{
			get
			{
				return this._warehouseInputAmount;
			}
			set
			{
				if (value != this._warehouseInputAmount)
				{
					this._warehouseInputAmount = value;
					base.OnPropertyChangedWithValue(value, "WarehouseInputAmount");
				}
			}
		}

		public int WarehouseOutputAmount
		{
			get
			{
				return this._warehouseOutputAmount;
			}
			set
			{
				if (value != this._warehouseOutputAmount)
				{
					this._warehouseOutputAmount = value;
					base.OnPropertyChangedWithValue(value, "WarehouseOutputAmount");
				}
			}
		}

		public SelectorVM<WorkshopPercentageSelectorItemVM> WarehousePercentageSelector
		{
			get
			{
				return this._warehousePercentageSelector;
			}
			set
			{
				if (value != this._warehousePercentageSelector)
				{
					this._warehousePercentageSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<WorkshopPercentageSelectorItemVM>>(value, "WarehousePercentageSelector");
				}
			}
		}

		private readonly TextObject _runningText = new TextObject("{=iuKvbKJ7}Running", null);

		private readonly TextObject _haltedText = new TextObject("{=zgnEagTJ}Halted", null);

		private readonly TextObject _noRawMaterialsText = new TextObject("{=JRKC4ed4}This workshop has not been producing for {DAY} {?PLURAL_DAYS}days{?}day{\\?} due to lack of raw materials in the town market.", null);

		private readonly TextObject _noProfitText = new TextObject("{=no0chrAH}This workshop has not been running for {DAY} {?PLURAL_DAYS}days{?}day{\\?} because the production has not been profitable", null);

		private readonly IWorkshopWarehouseCampaignBehavior _workshopWarehouseBehavior;

		private readonly WorkshopModel _workshopModel;

		private readonly Action<ClanCardSelectionInfo> _openCardSelectionPopup;

		private readonly Action<ClanFinanceWorkshopItemVM> _onSelectionT;

		private ExplainedNumber _inputDetails;

		private ExplainedNumber _outputDetails;

		private HintViewModel _useWarehouseAsInputHint;

		private HintViewModel _storeOutputPercentageHint;

		private HintViewModel _manageWorkshopHint;

		private BasicTooltipViewModel _inputWarehouseCountsTooltip;

		private BasicTooltipViewModel _outputWarehouseCountsTooltip;

		private string _workshopTypeId;

		private string _inputsText;

		private string _outputsText;

		private string _inputProducts;

		private string _outputProducts;

		private string _useWarehouseAsInputText;

		private string _storeOutputPercentageText;

		private string _warehouseCapacityText;

		private string _warehouseCapacityValue;

		private bool _receiveInputFromWarehouse;

		private int _warehouseInputAmount;

		private int _warehouseOutputAmount;

		private SelectorVM<WorkshopPercentageSelectorItemVM> _warehousePercentageSelector;
	}
}
