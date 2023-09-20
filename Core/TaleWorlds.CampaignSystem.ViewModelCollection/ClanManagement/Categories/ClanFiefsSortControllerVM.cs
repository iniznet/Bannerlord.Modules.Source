using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories
{
	public class ClanFiefsSortControllerVM : ViewModel
	{
		public ClanFiefsSortControllerVM(List<MBBindingList<ClanSettlementItemVM>> listsToControl)
		{
			this._listsToControl = listsToControl;
			this._nameComparer = new ClanFiefsSortControllerVM.ItemNameComparer();
			this._governorComparer = new ClanFiefsSortControllerVM.ItemGovernorComparer();
			this._profitComparer = new ClanFiefsSortControllerVM.ItemProfitComparer();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.GovernorText = GameTexts.FindText("str_notable_governor", null).ToString();
			this.ProfitText = GameTexts.FindText("str_profit", null).ToString();
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
			this._nameComparer.SetSortMode(this.NameState == 1);
			foreach (MBBindingList<ClanSettlementItemVM> mbbindingList in this._listsToControl)
			{
				mbbindingList.Sort(this._nameComparer);
			}
			this.IsNameSelected = true;
		}

		public void ExecuteSortByGovernor()
		{
			int governorState = this.GovernorState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.GovernorState = (governorState + 1) % 3;
			if (this.GovernorState == 0)
			{
				int governorState2 = this.GovernorState;
				this.GovernorState = governorState2 + 1;
			}
			this._governorComparer.SetSortMode(this.GovernorState == 1);
			foreach (MBBindingList<ClanSettlementItemVM> mbbindingList in this._listsToControl)
			{
				mbbindingList.Sort(this._governorComparer);
			}
			this.IsGovernorSelected = true;
		}

		public void ExecuteSortByProfit()
		{
			int profitState = this.ProfitState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.ProfitState = (profitState + 1) % 3;
			if (this.ProfitState == 0)
			{
				int profitState2 = this.ProfitState;
				this.ProfitState = profitState2 + 1;
			}
			this._profitComparer.SetSortMode(this.ProfitState == 1);
			foreach (MBBindingList<ClanSettlementItemVM> mbbindingList in this._listsToControl)
			{
				mbbindingList.Sort(this._profitComparer);
			}
			this.IsProfitSelected = true;
		}

		private void SetAllStates(CampaignUIHelper.SortState state)
		{
			this.NameState = (int)state;
			this.GovernorState = (int)state;
			this.ProfitState = (int)state;
			this.IsNameSelected = false;
			this.IsGovernorSelected = false;
			this.IsProfitSelected = false;
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
		public int GovernorState
		{
			get
			{
				return this._governorState;
			}
			set
			{
				if (value != this._governorState)
				{
					this._governorState = value;
					base.OnPropertyChangedWithValue(value, "GovernorState");
				}
			}
		}

		[DataSourceProperty]
		public int ProfitState
		{
			get
			{
				return this._profitState;
			}
			set
			{
				if (value != this._profitState)
				{
					this._profitState = value;
					base.OnPropertyChangedWithValue(value, "ProfitState");
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
		public bool IsGovernorSelected
		{
			get
			{
				return this._isGovernorSelected;
			}
			set
			{
				if (value != this._isGovernorSelected)
				{
					this._isGovernorSelected = value;
					base.OnPropertyChangedWithValue(value, "IsGovernorSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsProfitSelected
		{
			get
			{
				return this._isProfitSelected;
			}
			set
			{
				if (value != this._isProfitSelected)
				{
					this._isProfitSelected = value;
					base.OnPropertyChangedWithValue(value, "IsProfitSelected");
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
		public string ProfitText
		{
			get
			{
				return this._profitText;
			}
			set
			{
				if (value != this._profitText)
				{
					this._profitText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProfitText");
				}
			}
		}

		private readonly List<MBBindingList<ClanSettlementItemVM>> _listsToControl;

		private readonly ClanFiefsSortControllerVM.ItemNameComparer _nameComparer;

		private readonly ClanFiefsSortControllerVM.ItemGovernorComparer _governorComparer;

		private readonly ClanFiefsSortControllerVM.ItemProfitComparer _profitComparer;

		private int _nameState;

		private int _governorState;

		private int _profitState;

		private bool _isNameSelected;

		private bool _isGovernorSelected;

		private bool _isProfitSelected;

		private string _nameText;

		private string _governorText;

		private string _profitText;

		public abstract class ItemComparerBase : IComparer<ClanSettlementItemVM>
		{
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			public abstract int Compare(ClanSettlementItemVM x, ClanSettlementItemVM y);

			protected bool _isAcending;
		}

		public class ItemNameComparer : ClanFiefsSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanSettlementItemVM x, ClanSettlementItemVM y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		public class ItemGovernorComparer : ClanFiefsSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanSettlementItemVM x, ClanSettlementItemVM y)
			{
				if (this._isAcending)
				{
					if (y.HasGovernor && x.HasGovernor)
					{
						return y.Governor.NameText.CompareTo(x.Governor.NameText) * -1;
					}
					if (y.HasGovernor)
					{
						return 1;
					}
					if (x.HasGovernor)
					{
						return -1;
					}
					return 0;
				}
				else
				{
					if (y.HasGovernor && x.HasGovernor)
					{
						return y.Governor.NameText.CompareTo(x.Governor.NameText);
					}
					if (y.HasGovernor)
					{
						return 1;
					}
					if (x.HasGovernor)
					{
						return -1;
					}
					return 0;
				}
			}
		}

		public class ItemProfitComparer : ClanFiefsSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanSettlementItemVM x, ClanSettlementItemVM y)
			{
				if (this._isAcending)
				{
					return y.TotalProfit.Value.CompareTo(x.TotalProfit.Value) * -1;
				}
				return y.TotalProfit.Value.CompareTo(x.TotalProfit.Value);
			}
		}
	}
}
