using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public class ClanFinanceIncomeItemBaseVM : ViewModel
	{
		public IncomeTypes IncomeTypeAsEnum
		{
			get
			{
				return this._incomeTypeAsEnum;
			}
			protected set
			{
				if (value != this._incomeTypeAsEnum)
				{
					this._incomeTypeAsEnum = value;
					this.IncomeType = (int)value;
				}
			}
		}

		protected ClanFinanceIncomeItemBaseVM(Action<ClanFinanceIncomeItemBaseVM> onSelection, Action onRefresh)
		{
			this._onSelection = onSelection;
			this._onRefresh = onRefresh;
		}

		protected virtual void PopulateStatsList()
		{
		}

		protected virtual void PopulateActionList()
		{
		}

		public void OnIncomeSelection()
		{
			this._onSelection(this);
		}

		protected string DetermineIncomeText(int incomeAmount)
		{
			if (incomeAmount == 0)
			{
				return GameTexts.FindText("str_clan_finance_value_zero", null).ToString();
			}
			GameTexts.SetVariable("IS_POSITIVE", (this.Income > 0) ? 1 : 0);
			GameTexts.SetVariable("NUMBER", MathF.Abs(this.Income));
			return GameTexts.FindText("str_clan_finance_value", null).ToString();
		}

		[DataSourceProperty]
		public MBBindingList<SelectableItemPropertyVM> ItemProperties
		{
			get
			{
				return this._itemProperties;
			}
			set
			{
				if (value != this._itemProperties)
				{
					this._itemProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<SelectableItemPropertyVM>>(value, "ItemProperties");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string Location
		{
			get
			{
				return this._location;
			}
			set
			{
				if (value != this._location)
				{
					this._location = value;
					base.OnPropertyChangedWithValue<string>(value, "Location");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public string IncomeValueText
		{
			get
			{
				return this._incomeValueText;
			}
			set
			{
				if (value != this._incomeValueText)
				{
					this._incomeValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "IncomeValueText");
				}
			}
		}

		[DataSourceProperty]
		public string ImageName
		{
			get
			{
				return this._imageName;
			}
			set
			{
				if (value != this._imageName)
				{
					this._imageName = value;
					base.OnPropertyChangedWithValue<string>(value, "ImageName");
				}
			}
		}

		[DataSourceProperty]
		public int Income
		{
			get
			{
				return this._income;
			}
			set
			{
				if (value != this._income)
				{
					this._income = value;
					base.OnPropertyChangedWithValue(value, "Income");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		[DataSourceProperty]
		public int IncomeType
		{
			get
			{
				return this._incomeType;
			}
			set
			{
				if (value != this._incomeType)
				{
					this._incomeType = value;
					base.OnPropertyChangedWithValue(value, "IncomeType");
				}
			}
		}

		protected Action _onRefresh;

		protected Action<ClanFinanceIncomeItemBaseVM> _onSelection;

		protected IncomeTypes _incomeTypeAsEnum;

		private int _incomeType;

		private string _name;

		private string _location;

		private string _incomeValueText;

		private string _imageName;

		private int _income;

		private bool _isSelected;

		private ImageIdentifierVM _visual;

		private MBBindingList<SelectableItemPropertyVM> _itemProperties = new MBBindingList<SelectableItemPropertyVM>();
	}
}
