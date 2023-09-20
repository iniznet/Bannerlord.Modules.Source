using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.BannerBuilder
{
	public class BannerBuilderItemVM : ViewModel
	{
		public BannerIconData IconData { get; private set; }

		public string BackgroundTextureID { get; private set; }

		public BannerBuilderItemVM(int key, BannerIconData iconData, Action<BannerBuilderItemVM> onItemSelection)
		{
			this.MeshID = key;
			this.IconData = iconData;
			this._onItemSelection = onItemSelection;
		}

		public BannerBuilderItemVM(int key, string backgroundTextureID, Action<BannerBuilderItemVM> onItemSelection)
		{
			this.MeshID = key;
			this.BackgroundTextureID = backgroundTextureID;
			this._onItemSelection = onItemSelection;
		}

		public void ExecuteSelection()
		{
			Action<BannerBuilderItemVM> onItemSelection = this._onItemSelection;
			if (onItemSelection == null)
			{
				return;
			}
			onItemSelection(this);
		}

		[DataSourceProperty]
		public int MeshID
		{
			get
			{
				return this._meshID;
			}
			set
			{
				if (value != this._meshID)
				{
					this._meshID = value;
					base.OnPropertyChangedWithValue(value, "MeshID");
					this.MeshIDAsString = this._meshID.ToString();
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

		[DataSourceProperty]
		public string MeshIDAsString
		{
			get
			{
				return this._meshIDAsString;
			}
			set
			{
				if (value != this._meshIDAsString)
				{
					this._meshIDAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "MeshIDAsString");
				}
			}
		}

		private readonly Action<BannerBuilderItemVM> _onItemSelection;

		public int _meshID;

		public string _meshIDAsString;

		public bool _isSelected;
	}
}
