using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Home
{
	// Token: 0x0200007C RID: 124
	public class MPLobbyHomeChangeSigilPopupVM : ViewModel
	{
		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06000AEC RID: 2796 RVA: 0x00026F16 File Offset: 0x00025116
		// (set) Token: 0x06000AED RID: 2797 RVA: 0x00026F1E File Offset: 0x0002511E
		public MPLobbyCosmeticSigilItemVM SelectedSigil { get; private set; }

		// Token: 0x06000AEE RID: 2798 RVA: 0x00026F27 File Offset: 0x00025127
		public MPLobbyHomeChangeSigilPopupVM(Action<MPLobbyCosmeticSigilItemVM> onItemObtainRequested)
		{
			this._onItemObtainRequested = onItemObtainRequested;
			this.RefreshValues();
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x00026F3C File Offset: 0x0002513C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=7R0i82Nw}Change Sigil", null).ToString();
			this.ChangeText = new TextObject("{=Ba50zU7Z}Change", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
		}

		// Token: 0x06000AF0 RID: 2800 RVA: 0x00026F94 File Offset: 0x00025194
		private void RefreshSigilList()
		{
			this.SigilList = new MBBindingList<MPLobbyCosmeticSigilItemVM>();
			this.SelectedSigil = null;
			MBReadOnlyList<CosmeticsManager.CosmeticElement> getCosmeticElementList = CosmeticsManager.GetCosmeticElementList;
			IReadOnlyList<string> ownedCosmetics = NetworkMain.GameClient.OwnedCosmetics;
			for (int i = 0; i < getCosmeticElementList.Count; i++)
			{
				if (getCosmeticElementList[i].Type == CosmeticsManager.CosmeticType.Sigil)
				{
					CosmeticsManager.SigilCosmeticElement sigilCosmeticElement = getCosmeticElementList[i] as CosmeticsManager.SigilCosmeticElement;
					MPLobbyCosmeticSigilItemVM mplobbyCosmeticSigilItemVM = new MPLobbyCosmeticSigilItemVM(new Banner(sigilCosmeticElement.BannerCode).BannerDataList[1].MeshId, new Action<MPLobbyCosmeticSigilItemVM>(this.OnSigilSelected), new Action<MPLobbyCosmeticSigilItemVM>(this.OnSigilObtainRequested), (int)sigilCosmeticElement.Rarity, sigilCosmeticElement.Cost, sigilCosmeticElement.Id);
					mplobbyCosmeticSigilItemVM.IsUnlocked = ownedCosmetics.Contains(sigilCosmeticElement.Id) || sigilCosmeticElement.IsFree;
					this.SigilList.Add(mplobbyCosmeticSigilItemVM);
				}
			}
			this.IsUsingClanSigil = NetworkMain.GameClient.PlayerData.IsUsingClanSigil;
			this.SelectPlayerSigil(NetworkMain.GameClient.PlayerData);
			this.Loot = NetworkMain.GameClient.PlayerData.Gold;
			this.SigilList.Sort(new MPLobbyHomeChangeSigilPopupVM.SigilItemUnlockStatusComparer());
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x000270BC File Offset: 0x000252BC
		private void SelectPlayerSigil(PlayerData playerData)
		{
			int playerBannerID = new Banner(playerData.Sigil).BannerDataList[1].MeshId;
			this.OnSigilSelected(this.SigilList.First((MPLobbyCosmeticSigilItemVM s) => s.IconID == playerBannerID));
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x0002710D File Offset: 0x0002530D
		public void Open()
		{
			this.IsInClan = NetworkMain.GameClient.IsInClan;
			this.IsEnabled = true;
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x00027126 File Offset: 0x00025326
		public void ExecuteChangeSigil()
		{
			NetworkMain.GameClient.ChangeSigil(this.SelectedSigil.CosmeticID);
			NetworkMain.GameClient.PlayerData.IsUsingClanSigil = this.IsUsingClanSigil;
			this.IsEnabled = false;
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x00027159 File Offset: 0x00025359
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00027162 File Offset: 0x00025362
		private void OnSigilObtainRequested(MPLobbyCosmeticSigilItemVM sigilItem)
		{
			this._onItemObtainRequested(sigilItem);
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00027170 File Offset: 0x00025370
		private void OnSigilSelected(MPLobbyCosmeticSigilItemVM sigilItem)
		{
			if (sigilItem != this.SelectedSigil)
			{
				if (this.SelectedSigil != null)
				{
					this.SelectedSigil.IsUsed = false;
				}
				this.SelectedSigil = sigilItem;
				if (this.SelectedSigil != null)
				{
					this.SelectedSigil.IsUsed = true;
				}
			}
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x000271AA File Offset: 0x000253AA
		public void OnLootUpdated(int finalLoot)
		{
			this.Loot = finalLoot;
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x000271B3 File Offset: 0x000253B3
		private void OnIsEnabledChanged()
		{
			if (this.IsEnabled)
			{
				this.RefreshSigilList();
			}
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x000271C3 File Offset: 0x000253C3
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

		// Token: 0x06000AFA RID: 2810 RVA: 0x000271EC File Offset: 0x000253EC
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x000271FB File Offset: 0x000253FB
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06000AFC RID: 2812 RVA: 0x0002720A File Offset: 0x0002540A
		// (set) Token: 0x06000AFD RID: 2813 RVA: 0x00027212 File Offset: 0x00025412
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

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06000AFE RID: 2814 RVA: 0x0002722F File Offset: 0x0002542F
		// (set) Token: 0x06000AFF RID: 2815 RVA: 0x00027237 File Offset: 0x00025437
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

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06000B00 RID: 2816 RVA: 0x00027254 File Offset: 0x00025454
		// (set) Token: 0x06000B01 RID: 2817 RVA: 0x0002725C File Offset: 0x0002545C
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
					this.OnIsEnabledChanged();
				}
			}
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06000B02 RID: 2818 RVA: 0x00027280 File Offset: 0x00025480
		// (set) Token: 0x06000B03 RID: 2819 RVA: 0x00027288 File Offset: 0x00025488
		[DataSourceProperty]
		public bool IsLoading
		{
			get
			{
				return this._isLoading;
			}
			set
			{
				if (value != this._isLoading)
				{
					this._isLoading = value;
					base.OnPropertyChangedWithValue(value, "IsLoading");
				}
			}
		}

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06000B04 RID: 2820 RVA: 0x000272A6 File Offset: 0x000254A6
		// (set) Token: 0x06000B05 RID: 2821 RVA: 0x000272AE File Offset: 0x000254AE
		[DataSourceProperty]
		public bool IsInClan
		{
			get
			{
				return this._isInClan;
			}
			set
			{
				if (value != this._isInClan)
				{
					this._isInClan = value;
					base.OnPropertyChangedWithValue(value, "IsInClan");
				}
			}
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06000B06 RID: 2822 RVA: 0x000272CC File Offset: 0x000254CC
		// (set) Token: 0x06000B07 RID: 2823 RVA: 0x000272D4 File Offset: 0x000254D4
		[DataSourceProperty]
		public bool IsUsingClanSigil
		{
			get
			{
				return this._isUsingClanSigil;
			}
			set
			{
				if (value != this._isUsingClanSigil)
				{
					this._isUsingClanSigil = value;
					base.OnPropertyChangedWithValue(value, "IsUsingClanSigil");
				}
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06000B08 RID: 2824 RVA: 0x000272F2 File Offset: 0x000254F2
		// (set) Token: 0x06000B09 RID: 2825 RVA: 0x000272FA File Offset: 0x000254FA
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
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06000B0A RID: 2826 RVA: 0x0002731D File Offset: 0x0002551D
		// (set) Token: 0x06000B0B RID: 2827 RVA: 0x00027325 File Offset: 0x00025525
		[DataSourceProperty]
		public string ChangeText
		{
			get
			{
				return this._changeText;
			}
			set
			{
				if (value != this._changeText)
				{
					this._changeText = value;
					base.OnPropertyChangedWithValue<string>(value, "ChangeText");
				}
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06000B0C RID: 2828 RVA: 0x00027348 File Offset: 0x00025548
		// (set) Token: 0x06000B0D RID: 2829 RVA: 0x00027350 File Offset: 0x00025550
		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06000B0E RID: 2830 RVA: 0x00027373 File Offset: 0x00025573
		// (set) Token: 0x06000B0F RID: 2831 RVA: 0x0002737B File Offset: 0x0002557B
		[DataSourceProperty]
		public int Loot
		{
			get
			{
				return this._loot;
			}
			set
			{
				if (value != this._loot)
				{
					this._loot = value;
					base.OnPropertyChangedWithValue(value, "Loot");
				}
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06000B10 RID: 2832 RVA: 0x00027399 File Offset: 0x00025599
		// (set) Token: 0x06000B11 RID: 2833 RVA: 0x000273A1 File Offset: 0x000255A1
		[DataSourceProperty]
		public MBBindingList<MPLobbyCosmeticSigilItemVM> SigilList
		{
			get
			{
				return this._sigilList;
			}
			set
			{
				if (value != this._sigilList)
				{
					this._sigilList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyCosmeticSigilItemVM>>(value, "SigilList");
				}
			}
		}

		// Token: 0x04000548 RID: 1352
		private readonly Action<MPLobbyCosmeticSigilItemVM> _onItemObtainRequested;

		// Token: 0x0400054A RID: 1354
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x0400054B RID: 1355
		private InputKeyItemVM _doneInputKey;

		// Token: 0x0400054C RID: 1356
		private bool _isEnabled;

		// Token: 0x0400054D RID: 1357
		private bool _isLoading;

		// Token: 0x0400054E RID: 1358
		private bool _isInClan;

		// Token: 0x0400054F RID: 1359
		private bool _isUsingClanSigil;

		// Token: 0x04000550 RID: 1360
		private string _titleText;

		// Token: 0x04000551 RID: 1361
		private string _changeText;

		// Token: 0x04000552 RID: 1362
		private string _cancelText;

		// Token: 0x04000553 RID: 1363
		private int _loot;

		// Token: 0x04000554 RID: 1364
		private MBBindingList<MPLobbyCosmeticSigilItemVM> _sigilList;

		// Token: 0x020001A3 RID: 419
		private class SigilItemUnlockStatusComparer : IComparer<MPLobbyCosmeticSigilItemVM>
		{
			// Token: 0x060019D0 RID: 6608 RVA: 0x00053584 File Offset: 0x00051784
			public int Compare(MPLobbyCosmeticSigilItemVM x, MPLobbyCosmeticSigilItemVM y)
			{
				return y.IsUnlocked.CompareTo(x.IsUnlocked);
			}
		}
	}
}
