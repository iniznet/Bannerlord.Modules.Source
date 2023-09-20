using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	public class MPLobbyClanChangeFactionPopupVM : ViewModel
	{
		public MPLobbyClanChangeFactionPopupVM()
		{
			this.PrepareFactionsList();
			this.CanChangeFaction = false;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=ghjSIyIL}Choose Culture", null).ToString();
			this.ApplyText = new TextObject("{=BAaS5Dkc}Apply", null).ToString();
		}

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

		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
		}

		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
		}

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

		private MPCultureItemVM _selectedFaction;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private bool _isSelected;

		private bool _canChangeFaction;

		private string _titleText;

		private string _applyText;

		private MBBindingList<MPCultureItemVM> _factionsList;
	}
}
