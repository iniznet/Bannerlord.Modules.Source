using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.BannerEditor
{
	public class BannerIconVM : ViewModel
	{
		public int IconID { get; }

		public BannerIconVM(int iconID, Action<BannerIconVM> onSelection)
		{
			this.IconPath = iconID.ToString();
			this.IconID = iconID;
			this._onSelection = onSelection;
		}

		public void ExecuteSelectIcon()
		{
			this._onSelection(this);
		}

		[DataSourceProperty]
		public string IconPath
		{
			get
			{
				return this._iconPath;
			}
			set
			{
				if (value != this._iconPath)
				{
					this._iconPath = value;
					base.OnPropertyChangedWithValue<string>(value, "IconPath");
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

		private readonly Action<BannerIconVM> _onSelection;

		private string _iconPath;

		private bool _isSelected;
	}
}
