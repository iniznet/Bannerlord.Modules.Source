using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
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
			this.Workshop = workshop;
			this._openCardSelectionPopup = openCardSelectionPopup;
			this._workshopModel = Campaign.Current.Models.WorkshopModel;
			base.IncomeTypeAsEnum = IncomeTypes.Workshop;
			this._onSelection = new Action<ClanFinanceIncomeItemBaseVM>(this.tempOnSelection);
			this._onSelectionT = onSelection;
			SettlementComponent settlementComponent = this.Workshop.Settlement.SettlementComponent;
			base.ImageName = ((settlementComponent != null) ? settlementComponent.WaitMeshName : "");
			this.ManageWorkshopHint = new HintViewModel(new TextObject("{=LxWVtDF0}Manage Workshop", null), null);
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
			base.ItemProperties.Clear();
			this.PopulateStatsList();
		}

		protected override void PopulateStatsList()
		{
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=DzviXC3W}Level", null).ToString(), this.Workshop.Level.ToString(), null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=DXczLzml}Status", null).ToString(), this.Workshop.IsRunning ? new TextObject("{=nMcvafHY}Active", null).ToString() : new TextObject("{=bnrRzeiF}Not Active", null).ToString(), null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=ScDj827c}Initial Capital", null).ToString(), this.Workshop.InitialCapital.ToString(), null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=Ra17aK4e}Current Capital", null).ToString(), this.Workshop.Capital.ToString(), null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=CaRbMaZY}Daily Wage", null).ToString(), this.Workshop.Expense.ToString(), null));
			TextObject textObject;
			TextObject textObject2;
			ClanFinanceWorkshopItemVM.GetWorkshopTypeProductionTexts(this.Workshop.WorkshopType, out textObject, out textObject2);
			this.InputProducts = textObject.ToString();
			this.OutputProducts = textObject2.ToString();
			if (this.Workshop.NotRunnedDays > 0)
			{
				TextObject textObject3 = new TextObject("{=c6BtCIrX}{DAYS} days ago", null);
				textObject3.SetTextVariable("DAYS", this.Workshop.NotRunnedDays);
				base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=9huJZblF}Last Run", null).ToString(), textObject3.ToString(), null));
			}
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
			int sellingCost = this._workshopModel.GetSellingCost(this.Workshop);
			TextObject textObject;
			bool flag = Campaign.Current.Models.WorkshopModel.CanPlayerSellWorkshop(this.Workshop, out textObject);
			TextObject textObject2 = new TextObject("{=ysireFjT}Sell This Workshop for {GOLD_AMOUNT}{GOLD_ICON}", null);
			textObject2.SetTextVariable("GOLD_AMOUNT", sellingCost);
			textObject2.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			yield return new ClanCardSelectionItemInfo(textObject2, !flag, textObject, ClanCardSelectionItemPropertyInfo.CreateActionGoldChangeText(sellingCost));
			int costOfChangingType = this._workshopModel.GetConvertProductionCost(this.Workshop.WorkshopType);
			TextObject cannotChangeTypeReason = new TextObject("{=av51ur2M}You need at least {REQUIRED_AMOUNT} denars to change the production type of this workshop.", null);
			cannotChangeTypeReason.SetTextVariable("REQUIRED_AMOUNT", costOfChangingType);
			foreach (WorkshopType workshopType in WorkshopType.All)
			{
				if (this.Workshop.WorkshopType != workshopType && !workshopType.IsHidden)
				{
					TextObject name = workshopType.Name;
					bool flag2 = costOfChangingType <= Hero.MainHero.Gold;
					yield return new ClanCardSelectionItemInfo(workshopType, name, null, CardSelectionItemSpriteType.Workshop, workshopType.StringId, null, this.GetWorkshopItemProperties(workshopType), !flag2, cannotChangeTypeReason, ClanCardSelectionItemPropertyInfo.CreateActionGoldChangeText(-costOfChangingType));
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
					int sellingCost = Campaign.Current.Models.WorkshopModel.GetSellingCost(this.Workshop);
					Hero hero = Campaign.Current.Models.WorkshopModel.SelectNextOwnerForWorkshop(this.Workshop.Settlement.Town, this.Workshop, this.Workshop.Owner, sellingCost);
					ChangeOwnerOfWorkshopAction.ApplyByTrade(this.Workshop, hero, this.Workshop.WorkshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), true, sellingCost, null);
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

		private readonly WorkshopModel _workshopModel;

		private Action<ClanFinanceWorkshopItemVM> _onSelectionT;

		private readonly Action<ClanCardSelectionInfo> _openCardSelectionPopup;

		private HintViewModel _manageWorkshopHint;

		private string _workshopTypeId;

		private string _inputsText;

		private string _outputsText;

		private string _inputProducts;

		private string _outputProducts;
	}
}
