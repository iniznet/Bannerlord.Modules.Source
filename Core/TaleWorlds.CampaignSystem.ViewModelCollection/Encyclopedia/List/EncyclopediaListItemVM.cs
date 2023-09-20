using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	// Token: 0x020000BD RID: 189
	public class EncyclopediaListItemVM : ViewModel
	{
		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x060012A6 RID: 4774 RVA: 0x0004863A File Offset: 0x0004683A
		// (set) Token: 0x060012A7 RID: 4775 RVA: 0x00048642 File Offset: 0x00046842
		public object Object { get; private set; }

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x060012A8 RID: 4776 RVA: 0x0004864B File Offset: 0x0004684B
		public EncyclopediaListItem ListItem { get; }

		// Token: 0x060012A9 RID: 4777 RVA: 0x00048654 File Offset: 0x00046854
		public EncyclopediaListItemVM(EncyclopediaListItem listItem)
		{
			this.Object = listItem.Object;
			this.Id = listItem.Id;
			this._type = listItem.TypeName;
			this.ListItem = listItem;
			this.PlayerCanSeeValues = listItem.PlayerCanSeeValues;
			this._onShowTooltip = listItem.OnShowTooltip;
			this.RefreshValues();
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x000486B0 File Offset: 0x000468B0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.ListItem.Name;
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x000486C9 File Offset: 0x000468C9
		public void Execute()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this._type, this.Id);
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x000486E6 File Offset: 0x000468E6
		public void SetComparedValue(EncyclopediaListItemComparerBase comparer)
		{
			this.ComparedValue = comparer.GetComparedValueText(this.ListItem);
		}

		// Token: 0x060012AD RID: 4781 RVA: 0x000486FA File Offset: 0x000468FA
		public void ExecuteBeginTooltip()
		{
			Action onShowTooltip = this._onShowTooltip;
			if (onShowTooltip == null)
			{
				return;
			}
			onShowTooltip();
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x0004870C File Offset: 0x0004690C
		public void ExecuteEndTooltip()
		{
			if (this._onShowTooltip != null)
			{
				MBInformationManager.HideInformations();
			}
		}

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x060012AF RID: 4783 RVA: 0x0004871B File Offset: 0x0004691B
		// (set) Token: 0x060012B0 RID: 4784 RVA: 0x00048723 File Offset: 0x00046923
		[DataSourceProperty]
		public bool IsFiltered
		{
			get
			{
				return this._isFiltered;
			}
			set
			{
				if (value != this._isFiltered)
				{
					this._isFiltered = value;
					base.OnPropertyChangedWithValue(value, "IsFiltered");
				}
			}
		}

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x060012B1 RID: 4785 RVA: 0x00048741 File Offset: 0x00046941
		// (set) Token: 0x060012B2 RID: 4786 RVA: 0x00048749 File Offset: 0x00046949
		[DataSourceProperty]
		public bool PlayerCanSeeValues
		{
			get
			{
				return this._playerCanSeeValues;
			}
			set
			{
				if (value != this._playerCanSeeValues)
				{
					this._playerCanSeeValues = value;
					base.OnPropertyChangedWithValue(value, "PlayerCanSeeValues");
				}
			}
		}

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x060012B3 RID: 4787 RVA: 0x00048767 File Offset: 0x00046967
		// (set) Token: 0x060012B4 RID: 4788 RVA: 0x0004876F File Offset: 0x0004696F
		[DataSourceProperty]
		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				if (value != this._id)
				{
					this._id = value;
					base.OnPropertyChangedWithValue<string>(value, "Id");
				}
			}
		}

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x060012B5 RID: 4789 RVA: 0x00048792 File Offset: 0x00046992
		// (set) Token: 0x060012B6 RID: 4790 RVA: 0x0004879A File Offset: 0x0004699A
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

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x060012B7 RID: 4791 RVA: 0x000487BD File Offset: 0x000469BD
		// (set) Token: 0x060012B8 RID: 4792 RVA: 0x000487C5 File Offset: 0x000469C5
		[DataSourceProperty]
		public string ComparedValue
		{
			get
			{
				return this._comparedValue;
			}
			set
			{
				if (value != this._comparedValue)
				{
					this._comparedValue = value;
					base.OnPropertyChangedWithValue<string>(value, "ComparedValue");
				}
			}
		}

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x060012B9 RID: 4793 RVA: 0x000487E8 File Offset: 0x000469E8
		// (set) Token: 0x060012BA RID: 4794 RVA: 0x000487F0 File Offset: 0x000469F0
		[DataSourceProperty]
		public bool IsBookmarked
		{
			get
			{
				return this._isBookmarked;
			}
			set
			{
				if (value != this._isBookmarked)
				{
					this._isBookmarked = value;
					base.OnPropertyChangedWithValue(value, "IsBookmarked");
				}
			}
		}

		// Token: 0x040008AB RID: 2219
		private readonly string _type;

		// Token: 0x040008AC RID: 2220
		private readonly Action _onShowTooltip;

		// Token: 0x040008AD RID: 2221
		private string _id;

		// Token: 0x040008AE RID: 2222
		private string _name;

		// Token: 0x040008AF RID: 2223
		private string _comparedValue;

		// Token: 0x040008B0 RID: 2224
		private bool _isFiltered;

		// Token: 0x040008B1 RID: 2225
		private bool _isBookmarked;

		// Token: 0x040008B2 RID: 2226
		private bool _playerCanSeeValues;
	}
}
