using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.BannerEditor
{
	public class BannerColorVM : ViewModel
	{
		public int ColorID { get; }

		public uint Color { get; }

		public BannerColorVM(int colorID, uint color, Action<BannerColorVM> onSelection)
		{
			this.Color = color;
			this.ColorAsStr = TaleWorlds.Library.Color.FromUint(this.Color).ToString();
			this.ColorID = colorID;
			this._onSelection = onSelection;
		}

		public void ExecuteSelectIcon()
		{
			this._onSelection(this);
		}

		public void SetOnSelectionAction(Action<BannerColorVM> onSelection)
		{
			this._onSelection = onSelection;
			this.IsSelected = false;
		}

		[DataSourceProperty]
		public string ColorAsStr
		{
			get
			{
				return this._colorAsStr;
			}
			set
			{
				if (value != this._colorAsStr)
				{
					this._colorAsStr = value;
					base.OnPropertyChangedWithValue<string>(value, "ColorAsStr");
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

		private Action<BannerColorVM> _onSelection;

		private string _colorAsStr;

		private bool _isSelected;
	}
}
