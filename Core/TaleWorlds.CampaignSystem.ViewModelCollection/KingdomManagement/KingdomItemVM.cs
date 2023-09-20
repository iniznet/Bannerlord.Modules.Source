using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement
{
	public abstract class KingdomItemVM : ViewModel
	{
		protected virtual void OnSelect()
		{
			this.IsSelected = true;
		}

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

		private bool _isSelected;

		private bool _isNew;
	}
}
