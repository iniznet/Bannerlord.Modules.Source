using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Clan
{
	public class MPLobbyClanChangeSigilPopupVM : ViewModel
	{
		public MPLobbyClanChangeSigilPopupVM()
		{
			this.PrepareSigilIconsList();
			this.CanChangeSigil = false;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=q7VcSSbp}Choose Sigil", null).ToString();
			this.ApplyText = new TextObject("{=BAaS5Dkc}Apply", null).ToString();
		}

		private void PrepareSigilIconsList()
		{
			this.IconsList = new MBBindingList<MPLobbySigilItemVM>();
			this._selectedSigilIcon = null;
			foreach (BannerIconGroup bannerIconGroup in BannerManager.Instance.BannerIconGroups)
			{
				if (!bannerIconGroup.IsPattern)
				{
					foreach (KeyValuePair<int, BannerIconData> keyValuePair in bannerIconGroup.AvailableIcons)
					{
						MPLobbySigilItemVM mplobbySigilItemVM = new MPLobbySigilItemVM(keyValuePair.Key, new Action<MPLobbySigilItemVM>(this.OnSigilIconSelection));
						this.IconsList.Add(mplobbySigilItemVM);
					}
				}
			}
		}

		private void OnSigilIconSelection(MPLobbySigilItemVM sigilIcon)
		{
			if (sigilIcon != this._selectedSigilIcon)
			{
				if (this._selectedSigilIcon != null)
				{
					this._selectedSigilIcon.IsSelected = false;
				}
				this._selectedSigilIcon = sigilIcon;
				if (this._selectedSigilIcon != null)
				{
					this._selectedSigilIcon.IsSelected = true;
					this.CanChangeSigil = true;
				}
			}
		}

		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
		}

		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
		}

		public void ExecuteChangeSigil()
		{
			BasicCultureObject @object = Game.Current.ObjectManager.GetObject<BasicCultureObject>(NetworkMain.GameClient.ClanInfo.Faction);
			Banner banner = new Banner(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
			banner.BannerDataList[1].MeshId = this._selectedSigilIcon.IconID;
			NetworkMain.GameClient.ChangeClanSigil(banner.Serialize());
			this.ExecuteClosePopup();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChanged("CancelInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChanged("DoneInputKey");
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

		[DataSourceProperty]
		public bool CanChangeSigil
		{
			get
			{
				return this._canChangeSigil;
			}
			set
			{
				if (value != this._canChangeSigil)
				{
					this._canChangeSigil = value;
					base.OnPropertyChanged("CanChangeSigil");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChanged("TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string ApplyText
		{
			get
			{
				return this._applyText;
			}
			set
			{
				if (value != this._applyText)
				{
					this._applyText = value;
					base.OnPropertyChanged("ApplyText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPLobbySigilItemVM> IconsList
		{
			get
			{
				return this._iconsList;
			}
			set
			{
				if (value != this._iconsList)
				{
					this._iconsList = value;
					base.OnPropertyChanged("IconsList");
				}
			}
		}

		private MPLobbySigilItemVM _selectedSigilIcon;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private bool _isSelected;

		private bool _canChangeSigil;

		private string _titleText;

		private string _applyText;

		private MBBindingList<MPLobbySigilItemVM> _iconsList;
	}
}
