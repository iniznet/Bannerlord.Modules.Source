using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement
{
	public abstract class KingdomCategoryVM : ViewModel
	{
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

		private int _notificationCount;

		private string _categoryNameText;

		private string _noItemSelectedText;

		private bool _show;

		private bool _isAcceptableItemSelected;
	}
}
