using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x02000101 RID: 257
	public class ClanFinanceIncomeItemBaseVM : ViewModel
	{
		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x060017B4 RID: 6068 RVA: 0x000572EF File Offset: 0x000554EF
		// (set) Token: 0x060017B5 RID: 6069 RVA: 0x000572F7 File Offset: 0x000554F7
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

		// Token: 0x060017B6 RID: 6070 RVA: 0x00057310 File Offset: 0x00055510
		protected ClanFinanceIncomeItemBaseVM(Action<ClanFinanceIncomeItemBaseVM> onSelection, Action onRefresh)
		{
			this._onSelection = onSelection;
			this._onRefresh = onRefresh;
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x00057331 File Offset: 0x00055531
		protected virtual void PopulateStatsList()
		{
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x00057333 File Offset: 0x00055533
		protected virtual void PopulateActionList()
		{
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x00057335 File Offset: 0x00055535
		public void OnIncomeSelection()
		{
			this._onSelection(this);
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x00057344 File Offset: 0x00055544
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

		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x060017BB RID: 6075 RVA: 0x000573A1 File Offset: 0x000555A1
		// (set) Token: 0x060017BC RID: 6076 RVA: 0x000573A9 File Offset: 0x000555A9
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

		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x060017BD RID: 6077 RVA: 0x000573C7 File Offset: 0x000555C7
		// (set) Token: 0x060017BE RID: 6078 RVA: 0x000573CF File Offset: 0x000555CF
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

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x060017BF RID: 6079 RVA: 0x000573F2 File Offset: 0x000555F2
		// (set) Token: 0x060017C0 RID: 6080 RVA: 0x000573FA File Offset: 0x000555FA
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

		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x060017C1 RID: 6081 RVA: 0x0005741D File Offset: 0x0005561D
		// (set) Token: 0x060017C2 RID: 6082 RVA: 0x00057425 File Offset: 0x00055625
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

		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x060017C3 RID: 6083 RVA: 0x00057443 File Offset: 0x00055643
		// (set) Token: 0x060017C4 RID: 6084 RVA: 0x0005744B File Offset: 0x0005564B
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

		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x060017C5 RID: 6085 RVA: 0x0005746E File Offset: 0x0005566E
		// (set) Token: 0x060017C6 RID: 6086 RVA: 0x00057476 File Offset: 0x00055676
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

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x060017C7 RID: 6087 RVA: 0x00057499 File Offset: 0x00055699
		// (set) Token: 0x060017C8 RID: 6088 RVA: 0x000574A1 File Offset: 0x000556A1
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

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x060017C9 RID: 6089 RVA: 0x000574BF File Offset: 0x000556BF
		// (set) Token: 0x060017CA RID: 6090 RVA: 0x000574C7 File Offset: 0x000556C7
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

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x060017CB RID: 6091 RVA: 0x000574E5 File Offset: 0x000556E5
		// (set) Token: 0x060017CC RID: 6092 RVA: 0x000574ED File Offset: 0x000556ED
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

		// Token: 0x04000B40 RID: 2880
		protected Action _onRefresh;

		// Token: 0x04000B41 RID: 2881
		protected Action<ClanFinanceIncomeItemBaseVM> _onSelection;

		// Token: 0x04000B42 RID: 2882
		protected IncomeTypes _incomeTypeAsEnum;

		// Token: 0x04000B43 RID: 2883
		private int _incomeType;

		// Token: 0x04000B44 RID: 2884
		private string _name;

		// Token: 0x04000B45 RID: 2885
		private string _location;

		// Token: 0x04000B46 RID: 2886
		private string _incomeValueText;

		// Token: 0x04000B47 RID: 2887
		private string _imageName;

		// Token: 0x04000B48 RID: 2888
		private int _income;

		// Token: 0x04000B49 RID: 2889
		private bool _isSelected;

		// Token: 0x04000B4A RID: 2890
		private ImageIdentifierVM _visual;

		// Token: 0x04000B4B RID: 2891
		private MBBindingList<SelectableItemPropertyVM> _itemProperties = new MBBindingList<SelectableItemPropertyVM>();
	}
}
