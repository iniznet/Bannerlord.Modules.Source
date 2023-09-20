using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x02000062 RID: 98
	public class MPLobbySigilItemVM : ViewModel
	{
		// Token: 0x17000289 RID: 649
		// (get) Token: 0x0600083B RID: 2107 RVA: 0x0001F223 File Offset: 0x0001D423
		// (set) Token: 0x0600083C RID: 2108 RVA: 0x0001F22B File Offset: 0x0001D42B
		public int IconID { get; private set; }

		// Token: 0x0600083D RID: 2109 RVA: 0x0001F234 File Offset: 0x0001D434
		public MPLobbySigilItemVM()
		{
			this.RefreshWith(0);
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0001F243 File Offset: 0x0001D443
		public MPLobbySigilItemVM(int iconID, Action<MPLobbySigilItemVM> onSelection)
		{
			this.RefreshWith(iconID);
			this._onSelection = onSelection;
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0001F259 File Offset: 0x0001D459
		public void RefreshWith(int iconID)
		{
			this.IconPath = iconID.ToString();
			this.IconID = iconID;
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0001F26F File Offset: 0x0001D46F
		public void RefreshWith(string bannerCode)
		{
			this.RefreshWith(BannerCode.CreateFrom(bannerCode).CalculateBanner().BannerDataList[1].MeshId);
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0001F292 File Offset: 0x0001D492
		private void ExecuteSelectIcon()
		{
			Action<MPLobbySigilItemVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000842 RID: 2114 RVA: 0x0001F2A5 File Offset: 0x0001D4A5
		// (set) Token: 0x06000843 RID: 2115 RVA: 0x0001F2AD File Offset: 0x0001D4AD
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

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000844 RID: 2116 RVA: 0x0001F2CF File Offset: 0x0001D4CF
		// (set) Token: 0x06000845 RID: 2117 RVA: 0x0001F2D7 File Offset: 0x0001D4D7
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

		// Token: 0x04000428 RID: 1064
		private readonly Action<MPLobbySigilItemVM> _onSelection;

		// Token: 0x04000429 RID: 1065
		private string _iconPath;

		// Token: 0x0400042A RID: 1066
		private bool _isSelected;
	}
}
