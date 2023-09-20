using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	public class EncyclopediaPageVM : ViewModel
	{
		public object Obj
		{
			get
			{
				return this._args.Obj;
			}
		}

		public virtual string GetName()
		{
			return "";
		}

		public virtual string GetNavigationBarURL()
		{
			return "";
		}

		public virtual void Refresh()
		{
		}

		public EncyclopediaPageVM(EncyclopediaPageArgs args)
		{
			this._args = args;
			this.BookmarkHint = new HintViewModel();
		}

		public virtual void OnTick()
		{
		}

		public virtual void ExecuteSwitchBookmarkedState()
		{
			this.IsBookmarked = !this.IsBookmarked;
			this.UpdateBookmarkHintText();
		}

		protected void UpdateBookmarkHintText()
		{
			if (this.IsBookmarked)
			{
				this.BookmarkHint.HintText = new TextObject("{=BV5exuPf}Remove From Bookmarks", null);
				return;
			}
			this.BookmarkHint.HintText = new TextObject("{=d8jrv3nA}Add To Bookmarks", null);
		}

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

		private EncyclopediaPageArgs _args;

		private bool _isLoadingOver;

		private bool _isBookmarked;

		private HintViewModel _bookmarkHint;
	}
}
