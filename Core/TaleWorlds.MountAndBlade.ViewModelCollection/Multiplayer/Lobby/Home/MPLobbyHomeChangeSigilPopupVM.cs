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
	public class MPLobbyHomeChangeSigilPopupVM : ViewModel
	{
		public MPLobbyCosmeticSigilItemVM SelectedSigil { get; private set; }

		public MPLobbyHomeChangeSigilPopupVM(Action<MPLobbyCosmeticSigilItemVM> onItemObtainRequested)
		{
			this._onItemObtainRequested = onItemObtainRequested;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=7R0i82Nw}Change Sigil", null).ToString();
			this.ChangeText = new TextObject("{=Ba50zU7Z}Change", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
		}

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

		private void SelectPlayerSigil(PlayerData playerData)
		{
			int playerBannerID = new Banner(playerData.Sigil).BannerDataList[1].MeshId;
			this.OnSigilSelected(this.SigilList.First((MPLobbyCosmeticSigilItemVM s) => s.IconID == playerBannerID));
		}

		public void Open()
		{
			this.IsInClan = NetworkMain.GameClient.IsInClan;
			this.IsEnabled = true;
		}

		public void ExecuteChangeSigil()
		{
			NetworkMain.GameClient.ChangeSigil(this.SelectedSigil.CosmeticID);
			NetworkMain.GameClient.PlayerData.IsUsingClanSigil = this.IsUsingClanSigil;
			this.IsEnabled = false;
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		private void OnSigilObtainRequested(MPLobbyCosmeticSigilItemVM sigilItem)
		{
			this._onItemObtainRequested(sigilItem);
		}

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

		public void OnLootUpdated(int finalLoot)
		{
			this.Loot = finalLoot;
		}

		private void OnIsEnabledChanged()
		{
			if (this.IsEnabled)
			{
				this.RefreshSigilList();
			}
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

		private readonly Action<MPLobbyCosmeticSigilItemVM> _onItemObtainRequested;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private bool _isEnabled;

		private bool _isLoading;

		private bool _isInClan;

		private bool _isUsingClanSigil;

		private string _titleText;

		private string _changeText;

		private string _cancelText;

		private int _loot;

		private MBBindingList<MPLobbyCosmeticSigilItemVM> _sigilList;

		private class SigilItemUnlockStatusComparer : IComparer<MPLobbyCosmeticSigilItemVM>
		{
			public int Compare(MPLobbyCosmeticSigilItemVM x, MPLobbyCosmeticSigilItemVM y)
			{
				return y.IsUnlocked.CompareTo(x.IsUnlocked);
			}
		}
	}
}
