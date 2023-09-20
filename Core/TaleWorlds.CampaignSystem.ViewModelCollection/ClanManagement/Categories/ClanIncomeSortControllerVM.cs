using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Supporters;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories
{
	public class ClanIncomeSortControllerVM : ViewModel
	{
		public ClanIncomeSortControllerVM(MBBindingList<ClanFinanceWorkshopItemVM> workshopList, MBBindingList<ClanSupporterGroupVM> supporterList, MBBindingList<ClanFinanceAlleyItemVM> alleyList)
		{
			this._workshopList = workshopList;
			this._supporterList = supporterList;
			this._alleyList = alleyList;
			this._workshopNameComparer = new ClanIncomeSortControllerVM.WorkshopItemNameComparer();
			this._supporterNameComparer = new ClanIncomeSortControllerVM.SupporterItemNameComparer();
			this._alleyNameComparer = new ClanIncomeSortControllerVM.AlleyItemNameComparer();
			this._workshopLocationComparer = new ClanIncomeSortControllerVM.WorkshopItemLocationComparer();
			this._alleyLocationComparer = new ClanIncomeSortControllerVM.AlleyItemLocationComparer();
			this._workshopIncomeComparer = new ClanIncomeSortControllerVM.WorkshopItemIncomeComparer();
			this._supporterIncomeComparer = new ClanIncomeSortControllerVM.SupporterItemIncomeComparer();
			this._alleyIncomeComparer = new ClanIncomeSortControllerVM.AlleyItemIncomeComparer();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.LocationText = GameTexts.FindText("str_tooltip_label_location", null).ToString();
			this.IncomeText = GameTexts.FindText("str_income", null).ToString();
		}

		public void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				int nameState2 = this.NameState;
				this.NameState = nameState2 + 1;
			}
			this._workshopNameComparer.SetSortMode(this.NameState == 1);
			this._supporterNameComparer.SetSortMode(this.NameState == 1);
			this._alleyNameComparer.SetSortMode(this.NameState == 1);
			this._workshopList.Sort(this._workshopNameComparer);
			this._supporterList.Sort(this._supporterNameComparer);
			this._alleyList.Sort(this._alleyNameComparer);
			this.IsNameSelected = true;
		}

		public void ExecuteSortByLocation()
		{
			int locationState = this.LocationState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.LocationState = (locationState + 1) % 3;
			if (this.LocationState == 0)
			{
				int locationState2 = this.LocationState;
				this.LocationState = locationState2 + 1;
			}
			this._workshopLocationComparer.SetSortMode(this.LocationState == 1);
			this._alleyLocationComparer.SetSortMode(this.LocationState == 1);
			this._workshopList.Sort(this._workshopLocationComparer);
			this._alleyList.Sort(this._alleyLocationComparer);
			this.IsLocationSelected = true;
		}

		public void ExecuteSortByIncome()
		{
			int incomeState = this.IncomeState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.IncomeState = (incomeState + 1) % 3;
			if (this.IncomeState == 0)
			{
				int incomeState2 = this.IncomeState;
				this.IncomeState = incomeState2 + 1;
			}
			this._workshopIncomeComparer.SetSortMode(this.IncomeState == 1);
			this._supporterIncomeComparer.SetSortMode(this.IncomeState == 1);
			this._alleyIncomeComparer.SetSortMode(this.IncomeState == 1);
			this._workshopList.Sort(this._workshopIncomeComparer);
			this._supporterList.Sort(this._supporterIncomeComparer);
			this._alleyList.Sort(this._alleyIncomeComparer);
			this.IsIncomeSelected = true;
		}

		private void SetAllStates(CampaignUIHelper.SortState state)
		{
			this.NameState = (int)state;
			this.LocationState = (int)state;
			this.IncomeState = (int)state;
			this.IsNameSelected = false;
			this.IsLocationSelected = false;
			this.IsIncomeSelected = false;
		}

		public void ResetAllStates()
		{
			this.SetAllStates(CampaignUIHelper.SortState.Default);
		}

		[DataSourceProperty]
		public int NameState
		{
			get
			{
				return this._nameState;
			}
			set
			{
				if (value != this._nameState)
				{
					this._nameState = value;
					base.OnPropertyChangedWithValue(value, "NameState");
				}
			}
		}

		[DataSourceProperty]
		public int LocationState
		{
			get
			{
				return this._locationState;
			}
			set
			{
				if (value != this._locationState)
				{
					this._locationState = value;
					base.OnPropertyChangedWithValue(value, "LocationState");
				}
			}
		}

		[DataSourceProperty]
		public int IncomeState
		{
			get
			{
				return this._incomeState;
			}
			set
			{
				if (value != this._incomeState)
				{
					this._incomeState = value;
					base.OnPropertyChangedWithValue(value, "IncomeState");
				}
			}
		}

		[DataSourceProperty]
		public bool IsNameSelected
		{
			get
			{
				return this._isNameSelected;
			}
			set
			{
				if (value != this._isNameSelected)
				{
					this._isNameSelected = value;
					base.OnPropertyChangedWithValue(value, "IsNameSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLocationSelected
		{
			get
			{
				return this._isLocationSelected;
			}
			set
			{
				if (value != this._isLocationSelected)
				{
					this._isLocationSelected = value;
					base.OnPropertyChangedWithValue(value, "IsLocationSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsIncomeSelected
		{
			get
			{
				return this._isIncomeSelected;
			}
			set
			{
				if (value != this._isIncomeSelected)
				{
					this._isIncomeSelected = value;
					base.OnPropertyChangedWithValue(value, "IsIncomeSelected");
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
		public string LocationText
		{
			get
			{
				return this._locationText;
			}
			set
			{
				if (value != this._locationText)
				{
					this._locationText = value;
					base.OnPropertyChangedWithValue<string>(value, "LocationText");
				}
			}
		}

		[DataSourceProperty]
		public string IncomeText
		{
			get
			{
				return this._incomeText;
			}
			set
			{
				if (value != this._incomeText)
				{
					this._incomeText = value;
					base.OnPropertyChangedWithValue<string>(value, "IncomeText");
				}
			}
		}

		private readonly MBBindingList<ClanFinanceWorkshopItemVM> _workshopList;

		private readonly MBBindingList<ClanSupporterGroupVM> _supporterList;

		private readonly MBBindingList<ClanFinanceAlleyItemVM> _alleyList;

		private readonly ClanIncomeSortControllerVM.WorkshopItemNameComparer _workshopNameComparer;

		private readonly ClanIncomeSortControllerVM.SupporterItemNameComparer _supporterNameComparer;

		private readonly ClanIncomeSortControllerVM.AlleyItemNameComparer _alleyNameComparer;

		private readonly ClanIncomeSortControllerVM.WorkshopItemLocationComparer _workshopLocationComparer;

		private readonly ClanIncomeSortControllerVM.AlleyItemLocationComparer _alleyLocationComparer;

		private readonly ClanIncomeSortControllerVM.WorkshopItemIncomeComparer _workshopIncomeComparer;

		private readonly ClanIncomeSortControllerVM.SupporterItemIncomeComparer _supporterIncomeComparer;

		private readonly ClanIncomeSortControllerVM.AlleyItemIncomeComparer _alleyIncomeComparer;

		private int _nameState;

		private int _locationState;

		private int _incomeState;

		private bool _isNameSelected;

		private bool _isLocationSelected;

		private bool _isIncomeSelected;

		private string _nameText;

		private string _locationText;

		private string _incomeText;

		public abstract class WorkshopItemComparerBase : IComparer<ClanFinanceWorkshopItemVM>
		{
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			public abstract int Compare(ClanFinanceWorkshopItemVM x, ClanFinanceWorkshopItemVM y);

			protected bool _isAcending;
		}

		public abstract class SupporterItemComparerBase : IComparer<ClanSupporterGroupVM>
		{
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			public abstract int Compare(ClanSupporterGroupVM x, ClanSupporterGroupVM y);

			protected bool _isAcending;
		}

		public abstract class AlleyItemComparerBase : IComparer<ClanFinanceAlleyItemVM>
		{
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			public abstract int Compare(ClanFinanceAlleyItemVM x, ClanFinanceAlleyItemVM y);

			protected bool _isAcending;
		}

		public class WorkshopItemNameComparer : ClanIncomeSortControllerVM.WorkshopItemComparerBase
		{
			public override int Compare(ClanFinanceWorkshopItemVM x, ClanFinanceWorkshopItemVM y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		public class SupporterItemNameComparer : ClanIncomeSortControllerVM.SupporterItemComparerBase
		{
			public override int Compare(ClanSupporterGroupVM x, ClanSupporterGroupVM y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		public class AlleyItemNameComparer : ClanIncomeSortControllerVM.AlleyItemComparerBase
		{
			public override int Compare(ClanFinanceAlleyItemVM x, ClanFinanceAlleyItemVM y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		public class WorkshopItemLocationComparer : ClanIncomeSortControllerVM.WorkshopItemComparerBase
		{
			public override int Compare(ClanFinanceWorkshopItemVM x, ClanFinanceWorkshopItemVM y)
			{
				if (this._isAcending)
				{
					return y.Workshop.Settlement.GetTrackDistanceToMainAgent().CompareTo(x.Workshop.Settlement.GetTrackDistanceToMainAgent()) * -1;
				}
				return y.Workshop.Settlement.GetTrackDistanceToMainAgent().CompareTo(x.Workshop.Settlement.GetTrackDistanceToMainAgent());
			}
		}

		public class AlleyItemLocationComparer : ClanIncomeSortControllerVM.AlleyItemComparerBase
		{
			public override int Compare(ClanFinanceAlleyItemVM x, ClanFinanceAlleyItemVM y)
			{
				if (this._isAcending)
				{
					return y.Alley.Settlement.GetTrackDistanceToMainAgent().CompareTo(x.Alley.Settlement.GetTrackDistanceToMainAgent()) * -1;
				}
				return y.Alley.Settlement.GetTrackDistanceToMainAgent().CompareTo(x.Alley.Settlement.GetTrackDistanceToMainAgent());
			}
		}

		public class WorkshopItemIncomeComparer : ClanIncomeSortControllerVM.WorkshopItemComparerBase
		{
			public override int Compare(ClanFinanceWorkshopItemVM x, ClanFinanceWorkshopItemVM y)
			{
				if (this._isAcending)
				{
					return y.Workshop.ProfitMade.CompareTo(x.Workshop.ProfitMade) * -1;
				}
				return y.Workshop.ProfitMade.CompareTo(x.Workshop.ProfitMade);
			}
		}

		public class SupporterItemIncomeComparer : ClanIncomeSortControllerVM.SupporterItemComparerBase
		{
			public override int Compare(ClanSupporterGroupVM x, ClanSupporterGroupVM y)
			{
				if (this._isAcending)
				{
					return y.TotalInfluenceBonus.CompareTo(x.TotalInfluenceBonus) * -1;
				}
				return y.TotalInfluenceBonus.CompareTo(x.TotalInfluenceBonus);
			}
		}

		public class AlleyItemIncomeComparer : ClanIncomeSortControllerVM.AlleyItemComparerBase
		{
			public override int Compare(ClanFinanceAlleyItemVM x, ClanFinanceAlleyItemVM y)
			{
				if (this._isAcending)
				{
					return y.Income.CompareTo(x.Income) * -1;
				}
				return y.Income.CompareTo(x.Income);
			}
		}
	}
}
