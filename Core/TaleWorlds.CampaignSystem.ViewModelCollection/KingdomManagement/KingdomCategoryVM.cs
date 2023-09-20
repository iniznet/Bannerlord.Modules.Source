using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement
{
	// Token: 0x02000052 RID: 82
	public abstract class KingdomCategoryVM : ViewModel
	{
		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000675 RID: 1653 RVA: 0x0001D9EC File Offset: 0x0001BBEC
		// (set) Token: 0x06000676 RID: 1654 RVA: 0x0001D9F4 File Offset: 0x0001BBF4
		[DataSourceProperty]
		public string CategoryNameText
		{
			get
			{
				return this._categoryNameText;
			}
			set
			{
				if (value != this._categoryNameText)
				{
					this._categoryNameText = value;
					base.OnPropertyChanged("NameText");
				}
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000677 RID: 1655 RVA: 0x0001DA16 File Offset: 0x0001BC16
		// (set) Token: 0x06000678 RID: 1656 RVA: 0x0001DA1E File Offset: 0x0001BC1E
		[DataSourceProperty]
		public string NoItemSelectedText
		{
			get
			{
				return this._noItemSelectedText;
			}
			set
			{
				if (value != this._noItemSelectedText)
				{
					this._noItemSelectedText = value;
					base.OnPropertyChangedWithValue<string>(value, "NoItemSelectedText");
				}
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000679 RID: 1657 RVA: 0x0001DA41 File Offset: 0x0001BC41
		// (set) Token: 0x0600067A RID: 1658 RVA: 0x0001DA49 File Offset: 0x0001BC49
		[DataSourceProperty]
		public bool IsAcceptableItemSelected
		{
			get
			{
				return this._isAcceptableItemSelected;
			}
			set
			{
				if (value != this._isAcceptableItemSelected)
				{
					this._isAcceptableItemSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAcceptableItemSelected");
				}
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x0001DA67 File Offset: 0x0001BC67
		// (set) Token: 0x0600067C RID: 1660 RVA: 0x0001DA6F File Offset: 0x0001BC6F
		[DataSourceProperty]
		public int NotificationCount
		{
			get
			{
				return this._notificationCount;
			}
			set
			{
				if (value != this._notificationCount)
				{
					this._notificationCount = value;
					base.OnPropertyChanged("NotificationCount");
				}
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x0600067D RID: 1661 RVA: 0x0001DA8C File Offset: 0x0001BC8C
		// (set) Token: 0x0600067E RID: 1662 RVA: 0x0001DA94 File Offset: 0x0001BC94
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
					base.OnPropertyChanged("Show");
				}
			}
		}

		// Token: 0x040002DB RID: 731
		private int _notificationCount;

		// Token: 0x040002DC RID: 732
		private string _categoryNameText;

		// Token: 0x040002DD RID: 733
		private string _noItemSelectedText;

		// Token: 0x040002DE RID: 734
		private bool _show;

		// Token: 0x040002DF RID: 735
		private bool _isAcceptableItemSelected;
	}
}
