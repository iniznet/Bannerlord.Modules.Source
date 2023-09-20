using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x02000093 RID: 147
	public class MPLobbyClanChangeFactionPopupVM : ViewModel
	{
		// Token: 0x06000DBE RID: 3518 RVA: 0x0002F493 File Offset: 0x0002D693
		public MPLobbyClanChangeFactionPopupVM()
		{
			this.PrepareFactionsList();
			this.CanChangeFaction = false;
			this.RefreshValues();
		}

		// Token: 0x06000DBF RID: 3519 RVA: 0x0002F4AE File Offset: 0x0002D6AE
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=ghjSIyIL}Choose Culture", null).ToString();
			this.ApplyText = new TextObject("{=BAaS5Dkc}Apply", null).ToString();
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x0002F4E4 File Offset: 0x0002D6E4
		private void PrepareFactionsList()
		{
			this._selectedFaction = null;
			this.FactionsList = new MBBindingList<MPCultureItemVM>
			{
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("vlandia").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("sturgia").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("empire").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("battania").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("khuzait").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("aserai").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection))
			};
		}

		// Token: 0x06000DC1 RID: 3521 RVA: 0x0002F624 File Offset: 0x0002D824
		private void OnFactionSelection(MPCultureItemVM faction)
		{
			if (faction != this._selectedFaction)
			{
				if (this._selectedFaction != null)
				{
					this._selectedFaction.IsSelected = false;
				}
				this._selectedFaction = faction;
				if (this._selectedFaction != null)
				{
					this._selectedFaction.IsSelected = true;
					this.CanChangeFaction = true;
				}
			}
		}

		// Token: 0x06000DC2 RID: 3522 RVA: 0x0002F670 File Offset: 0x0002D870
		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
		}

		// Token: 0x06000DC3 RID: 3523 RVA: 0x0002F679 File Offset: 0x0002D879
		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x0002F684 File Offset: 0x0002D884
		public void ExecuteChangeFaction()
		{
			BasicCultureObject @object = Game.Current.ObjectManager.GetObject<BasicCultureObject>(this._selectedFaction.CultureCode);
			Banner banner = new Banner(NetworkMain.GameClient.ClanInfo.Sigil);
			banner.ChangeIconColors(@object.ForegroundColor1);
			banner.ChangePrimaryColor(@object.BackgroundColor1);
			NetworkMain.GameClient.ChangeClanSigil(banner.Serialize());
			NetworkMain.GameClient.ChangeClanFaction(this._selectedFaction.CultureCode);
			this.ExecuteClosePopup();
		}

		// Token: 0x06000DC5 RID: 3525 RVA: 0x0002F704 File Offset: 0x0002D904
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

		// Token: 0x06000DC6 RID: 3526 RVA: 0x0002F72D File Offset: 0x0002D92D
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000DC7 RID: 3527 RVA: 0x0002F73C File Offset: 0x0002D93C
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x0002F74B File Offset: 0x0002D94B
		// (set) Token: 0x06000DC9 RID: 3529 RVA: 0x0002F753 File Offset: 0x0002D953
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

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06000DCA RID: 3530 RVA: 0x0002F770 File Offset: 0x0002D970
		// (set) Token: 0x06000DCB RID: 3531 RVA: 0x0002F778 File Offset: 0x0002D978
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

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06000DCC RID: 3532 RVA: 0x0002F795 File Offset: 0x0002D995
		// (set) Token: 0x06000DCD RID: 3533 RVA: 0x0002F79D File Offset: 0x0002D99D
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

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06000DCE RID: 3534 RVA: 0x0002F7BA File Offset: 0x0002D9BA
		// (set) Token: 0x06000DCF RID: 3535 RVA: 0x0002F7C2 File Offset: 0x0002D9C2
		[DataSourceProperty]
		public bool CanChangeFaction
		{
			get
			{
				return this._canChangeFaction;
			}
			set
			{
				if (value != this._canChangeFaction)
				{
					this._canChangeFaction = value;
					base.OnPropertyChanged("CanChangeFaction");
				}
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06000DD0 RID: 3536 RVA: 0x0002F7DF File Offset: 0x0002D9DF
		// (set) Token: 0x06000DD1 RID: 3537 RVA: 0x0002F7E7 File Offset: 0x0002D9E7
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

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06000DD2 RID: 3538 RVA: 0x0002F809 File Offset: 0x0002DA09
		// (set) Token: 0x06000DD3 RID: 3539 RVA: 0x0002F811 File Offset: 0x0002DA11
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

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06000DD4 RID: 3540 RVA: 0x0002F833 File Offset: 0x0002DA33
		// (set) Token: 0x06000DD5 RID: 3541 RVA: 0x0002F83B File Offset: 0x0002DA3B
		[DataSourceProperty]
		public MBBindingList<MPCultureItemVM> FactionsList
		{
			get
			{
				return this._factionsList;
			}
			set
			{
				if (value != this._factionsList)
				{
					this._factionsList = value;
					base.OnPropertyChanged("FactionsList");
				}
			}
		}

		// Token: 0x04000696 RID: 1686
		private MPCultureItemVM _selectedFaction;

		// Token: 0x04000697 RID: 1687
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000698 RID: 1688
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000699 RID: 1689
		private bool _isSelected;

		// Token: 0x0400069A RID: 1690
		private bool _canChangeFaction;

		// Token: 0x0400069B RID: 1691
		private string _titleText;

		// Token: 0x0400069C RID: 1692
		private string _applyText;

		// Token: 0x0400069D RID: 1693
		private MBBindingList<MPCultureItemVM> _factionsList;
	}
}
