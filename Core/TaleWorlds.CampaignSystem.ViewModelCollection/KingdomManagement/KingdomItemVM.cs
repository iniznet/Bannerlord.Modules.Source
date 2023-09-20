using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement
{
	// Token: 0x02000054 RID: 84
	public abstract class KingdomItemVM : ViewModel
	{
		// Token: 0x060006A4 RID: 1700 RVA: 0x0001DF7B File Offset: 0x0001C17B
		protected virtual void OnSelect()
		{
			this.IsSelected = true;
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x0001DF84 File Offset: 0x0001C184
		// (set) Token: 0x060006A6 RID: 1702 RVA: 0x0001DF8C File Offset: 0x0001C18C
		[DataSourceProperty]
		public bool IsNew
		{
			get
			{
				return this._isNew;
			}
			set
			{
				if (value != this._isNew)
				{
					this._isNew = value;
					base.OnPropertyChangedWithValue(value, "IsNew");
				}
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x0001DFAA File Offset: 0x0001C1AA
		// (set) Token: 0x060006A8 RID: 1704 RVA: 0x0001DFB2 File Offset: 0x0001C1B2
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

		// Token: 0x040002F0 RID: 752
		private bool _isSelected;

		// Token: 0x040002F1 RID: 753
		private bool _isNew;
	}
}
