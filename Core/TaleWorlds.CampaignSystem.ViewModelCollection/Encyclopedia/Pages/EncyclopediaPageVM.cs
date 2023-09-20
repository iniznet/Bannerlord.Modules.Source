using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000B7 RID: 183
	public class EncyclopediaPageVM : ViewModel
	{
		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x06001204 RID: 4612 RVA: 0x00046C77 File Offset: 0x00044E77
		public object Obj
		{
			get
			{
				return this._args.Obj;
			}
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x00046C84 File Offset: 0x00044E84
		public virtual string GetName()
		{
			return "";
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x00046C8B File Offset: 0x00044E8B
		public virtual string GetNavigationBarURL()
		{
			return "";
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x00046C92 File Offset: 0x00044E92
		public virtual void Refresh()
		{
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x00046C94 File Offset: 0x00044E94
		public EncyclopediaPageVM(EncyclopediaPageArgs args)
		{
			this._args = args;
			this.BookmarkHint = new HintViewModel();
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x00046CAE File Offset: 0x00044EAE
		public virtual void OnTick()
		{
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x00046CB0 File Offset: 0x00044EB0
		public virtual void ExecuteSwitchBookmarkedState()
		{
			this.IsBookmarked = !this.IsBookmarked;
			this.UpdateBookmarkHintText();
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x00046CC7 File Offset: 0x00044EC7
		protected void UpdateBookmarkHintText()
		{
			if (this.IsBookmarked)
			{
				this.BookmarkHint.HintText = new TextObject("{=BV5exuPf}Remove From Bookmarks", null);
				return;
			}
			this.BookmarkHint.HintText = new TextObject("{=d8jrv3nA}Add To Bookmarks", null);
		}

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x0600120C RID: 4620 RVA: 0x00046CFE File Offset: 0x00044EFE
		// (set) Token: 0x0600120D RID: 4621 RVA: 0x00046D06 File Offset: 0x00044F06
		[DataSourceProperty]
		public bool IsLoadingOver
		{
			get
			{
				return this._isLoadingOver;
			}
			set
			{
				if (value != this._isLoadingOver)
				{
					this._isLoadingOver = value;
					base.OnPropertyChangedWithValue(value, "IsLoadingOver");
				}
			}
		}

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x0600120E RID: 4622 RVA: 0x00046D24 File Offset: 0x00044F24
		// (set) Token: 0x0600120F RID: 4623 RVA: 0x00046D2C File Offset: 0x00044F2C
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
					base.OnPropertyChanged("IsBookmarked");
				}
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x06001210 RID: 4624 RVA: 0x00046D49 File Offset: 0x00044F49
		// (set) Token: 0x06001211 RID: 4625 RVA: 0x00046D51 File Offset: 0x00044F51
		[DataSourceProperty]
		public HintViewModel BookmarkHint
		{
			get
			{
				return this._bookmarkHint;
			}
			set
			{
				if (value != this._bookmarkHint)
				{
					this._bookmarkHint = value;
					base.OnPropertyChanged("BookmarkHint");
				}
			}
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06001212 RID: 4626 RVA: 0x00046D6E File Offset: 0x00044F6E
		// (set) Token: 0x06001213 RID: 4627 RVA: 0x00046D71 File Offset: 0x00044F71
		[DataSourceProperty]
		public virtual MBBindingList<EncyclopediaListItemVM> Items
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06001214 RID: 4628 RVA: 0x00046D73 File Offset: 0x00044F73
		// (set) Token: 0x06001215 RID: 4629 RVA: 0x00046D76 File Offset: 0x00044F76
		[DataSourceProperty]
		public virtual MBBindingList<EncyclopediaFilterGroupVM> FilterGroups
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06001216 RID: 4630 RVA: 0x00046D78 File Offset: 0x00044F78
		// (set) Token: 0x06001217 RID: 4631 RVA: 0x00046D7B File Offset: 0x00044F7B
		[DataSourceProperty]
		public virtual EncyclopediaListSortControllerVM SortController
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x04000869 RID: 2153
		private EncyclopediaPageArgs _args;

		// Token: 0x0400086A RID: 2154
		private bool _isLoadingOver;

		// Token: 0x0400086B RID: 2155
		private bool _isBookmarked;

		// Token: 0x0400086C RID: 2156
		private HintViewModel _bookmarkHint;
	}
}
