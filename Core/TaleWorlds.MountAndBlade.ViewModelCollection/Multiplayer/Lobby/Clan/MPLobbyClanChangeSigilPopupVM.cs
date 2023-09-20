using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x02000094 RID: 148
	public class MPLobbyClanChangeSigilPopupVM : ViewModel
	{
		// Token: 0x06000DD6 RID: 3542 RVA: 0x0002F858 File Offset: 0x0002DA58
		public MPLobbyClanChangeSigilPopupVM()
		{
			this.PrepareSigilIconsList();
			this.CanChangeSigil = false;
			this.RefreshValues();
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x0002F873 File Offset: 0x0002DA73
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=q7VcSSbp}Choose Sigil", null).ToString();
			this.ApplyText = new TextObject("{=BAaS5Dkc}Apply", null).ToString();
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x0002F8A8 File Offset: 0x0002DAA8
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

		// Token: 0x06000DD9 RID: 3545 RVA: 0x0002F974 File Offset: 0x0002DB74
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

		// Token: 0x06000DDA RID: 3546 RVA: 0x0002F9C0 File Offset: 0x0002DBC0
		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x0002F9C9 File Offset: 0x0002DBC9
		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x0002F9D4 File Offset: 0x0002DBD4
		public void ExecuteChangeSigil()
		{
			BasicCultureObject @object = Game.Current.ObjectManager.GetObject<BasicCultureObject>(NetworkMain.GameClient.ClanInfo.Faction);
			Banner banner = new Banner(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
			banner.BannerDataList[1].MeshId = this._selectedSigilIcon.IconID;
			NetworkMain.GameClient.ChangeClanSigil(banner.Serialize());
			this.ExecuteClosePopup();
		}

		// Token: 0x06000DDD RID: 3549 RVA: 0x0002FA4A File Offset: 0x0002DC4A
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

		// Token: 0x06000DDE RID: 3550 RVA: 0x0002FA73 File Offset: 0x0002DC73
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000DDF RID: 3551 RVA: 0x0002FA82 File Offset: 0x0002DC82
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06000DE0 RID: 3552 RVA: 0x0002FA91 File Offset: 0x0002DC91
		// (set) Token: 0x06000DE1 RID: 3553 RVA: 0x0002FA99 File Offset: 0x0002DC99
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

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06000DE2 RID: 3554 RVA: 0x0002FAB6 File Offset: 0x0002DCB6
		// (set) Token: 0x06000DE3 RID: 3555 RVA: 0x0002FABE File Offset: 0x0002DCBE
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

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x0002FADB File Offset: 0x0002DCDB
		// (set) Token: 0x06000DE5 RID: 3557 RVA: 0x0002FAE3 File Offset: 0x0002DCE3
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

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06000DE6 RID: 3558 RVA: 0x0002FB00 File Offset: 0x0002DD00
		// (set) Token: 0x06000DE7 RID: 3559 RVA: 0x0002FB08 File Offset: 0x0002DD08
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

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06000DE8 RID: 3560 RVA: 0x0002FB25 File Offset: 0x0002DD25
		// (set) Token: 0x06000DE9 RID: 3561 RVA: 0x0002FB2D File Offset: 0x0002DD2D
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

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06000DEA RID: 3562 RVA: 0x0002FB4F File Offset: 0x0002DD4F
		// (set) Token: 0x06000DEB RID: 3563 RVA: 0x0002FB57 File Offset: 0x0002DD57
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

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06000DEC RID: 3564 RVA: 0x0002FB79 File Offset: 0x0002DD79
		// (set) Token: 0x06000DED RID: 3565 RVA: 0x0002FB81 File Offset: 0x0002DD81
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

		// Token: 0x0400069E RID: 1694
		private MPLobbySigilItemVM _selectedSigilIcon;

		// Token: 0x0400069F RID: 1695
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040006A0 RID: 1696
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040006A1 RID: 1697
		private bool _isSelected;

		// Token: 0x040006A2 RID: 1698
		private bool _canChangeSigil;

		// Token: 0x040006A3 RID: 1699
		private string _titleText;

		// Token: 0x040006A4 RID: 1700
		private string _applyText;

		// Token: 0x040006A5 RID: 1701
		private MBBindingList<MPLobbySigilItemVM> _iconsList;
	}
}
