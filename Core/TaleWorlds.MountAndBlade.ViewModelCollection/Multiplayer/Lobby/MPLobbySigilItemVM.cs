using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	public class MPLobbySigilItemVM : ViewModel
	{
		public int IconID { get; private set; }

		public MPLobbySigilItemVM()
		{
			this.RefreshWith(0);
		}

		public MPLobbySigilItemVM(int iconID, Action<MPLobbySigilItemVM> onSelection)
		{
			this.RefreshWith(iconID);
			this._onSelection = onSelection;
		}

		public void RefreshWith(int iconID)
		{
			this.IconPath = iconID.ToString();
			this.IconID = iconID;
		}

		public void RefreshWith(string bannerCode)
		{
			this.RefreshWith(BannerCode.CreateFrom(bannerCode).CalculateBanner().BannerDataList[1].MeshId);
		}

		private void ExecuteSelectIcon()
		{
			Action<MPLobbySigilItemVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
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
					base.OnPropertyChanged("IconPath");
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
					base.OnPropertyChanged("IsSelected");
				}
			}
		}

		private readonly Action<MPLobbySigilItemVM> _onSelection;

		private string _iconPath;

		private bool _isSelected;
	}
}
