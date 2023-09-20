using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Messages.FromLobbyServer.ToClient;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Clan
{
	public class MPLobbyClanCreationPopupVM : ViewModel
	{
		public MPLobbyClanCreationPopupVM()
		{
			this.PartyMembersList = new MBBindingList<MPLobbyClanMemberItemVM>();
			this.PrepareFactionsList();
			this.PrepareSigilIconsList();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CreateClanText = new TextObject("{=ECb8IPbA}Create Clan", null).ToString();
			this.NameText = new TextObject("{=PDdh1sBj}Name", null).ToString();
			this.TagText = new TextObject("{=OUvFT99g}Tag", null).ToString();
			this.FactionText = new TextObject("{=PUjDWe5j}Culture", null).ToString();
			this.SigilText = new TextObject("{=P5Z9owOy}Sigil", null).ToString();
			this.CreateText = new TextObject("{=65oGXBYQ}Create", null).ToString();
			this.CancelText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
			this.WaitingForConfirmationText = new TextObject("{=08KLQa3P}Waiting For Party Members", null).ToString();
			this.ResetAll();
		}

		private void ResetAll()
		{
			this.ResetErrorTexts();
			this.ResetUserInputs();
		}

		private void ResetErrorTexts()
		{
			this.NameErrorText = "";
			this.TagErrorText = "";
			this.FactionErrorText = "";
			this.SigilIconErrorText = "";
		}

		private void ResetUserInputs()
		{
			this.NameInputText = "";
			this.TagInputText = "";
			this.OnFactionSelection(null);
			this.OnSigilIconSelection(null);
		}

		public void ExecuteOpenPopup()
		{
			this.RefreshValues();
			this.IsEnabled = true;
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		private void PrepareFactionsList()
		{
			this._selectedFaction = null;
			MBBindingList<MPCultureItemVM> mbbindingList = new MBBindingList<MPCultureItemVM>();
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("vlandia").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("sturgia").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("empire").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("battania").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("khuzait").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("aserai").StringId, new Action<MPCultureItemVM>(this.OnFactionSelection)));
			this.FactionsList = mbbindingList;
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

		private void PreparePartyMembersList()
		{
			this.PartyMembersList.Clear();
			foreach (PartyPlayerInLobbyClient partyPlayerInLobbyClient in NetworkMain.GameClient.PlayersInParty)
			{
				if (partyPlayerInLobbyClient.PlayerId != NetworkMain.GameClient.PlayerID)
				{
					MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = new MPLobbyClanMemberItemVM(partyPlayerInLobbyClient.PlayerId);
					mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=c0ZdKSkn}Waiting", null).ToString();
					this.PartyMembersList.Add(mplobbyClanMemberItemVM);
				}
			}
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
					this.FactionErrorText = "";
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
					this.SigilIconErrorText = "";
				}
			}
		}

		private void UpdateNameErrorText(StringValidationError error)
		{
			this.NameErrorText = "";
			if (error == null)
			{
				this.NameErrorText = new TextObject("{=bExIl1A2}Name Length Is Invalid", null).ToString();
				return;
			}
			if (error == 2)
			{
				this.NameErrorText = new TextObject("{=Agtv9l7S}This Name Already Exists", null).ToString();
				return;
			}
			if (error == 1)
			{
				this.NameErrorText = new TextObject("{=lO1hok44}Name Has Invalid Characters In It", null).ToString();
				return;
			}
			if (error == 3)
			{
				this.NameErrorText = new TextObject("{=cl2DnRYR}Name Should Not Contain Offensive Words", null).ToString();
				return;
			}
			if (error == 4)
			{
				this.NameErrorText = new TextObject("{=UEgS8RcB}Name Has Invalid Content", null).ToString();
			}
		}

		private void UpdateTagErrorText(StringValidationError error)
		{
			this.TagErrorText = "";
			if (error == null)
			{
				this.TagErrorText = new TextObject("{=MjnlWhih}Tag Length Is Invalid", null).ToString();
				return;
			}
			if (error == 2)
			{
				this.TagErrorText = new TextObject("{=ulzyykHO}This Tag Already Exists", null).ToString();
				return;
			}
			if (error == 1)
			{
				this.TagErrorText = new TextObject("{=FjmxNxZJ}Tag Has Invalid Characters In It", null).ToString();
				return;
			}
			if (error == 3)
			{
				this.TagErrorText = new TextObject("{=jyJXcOLe}Tag Should Not Contain Offensive Words", null).ToString();
				return;
			}
			if (error == 4)
			{
				this.TagErrorText = new TextObject("{=hCNnqVgK}Tag Has Invalid Content", null).ToString();
			}
		}

		public void UpdateFactionErrorText()
		{
			this.FactionErrorText = "";
			this.FactionErrorText = new TextObject("{=p83IO9ls}You must select a culture", null).ToString();
		}

		public void UpdateSigilIconErrorText()
		{
			this.SigilIconErrorText = "";
			this.SigilIconErrorText = new TextObject("{=uOrwqeQl}You must select a sigil icon", null).ToString();
		}

		public void UpdateConfirmation(PlayerId playerId, ClanCreationAnswer answer)
		{
			foreach (MPLobbyClanMemberItemVM mplobbyClanMemberItemVM in this.PartyMembersList)
			{
				if (mplobbyClanMemberItemVM.ProvidedID == playerId)
				{
					if (answer == 1)
					{
						mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=JTMegIk4}Accepted", null).ToString();
					}
					else if (answer == 2)
					{
						mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=FgaORzy5}Declined", null).ToString();
					}
				}
			}
		}

		private BasicCultureObject GetSelectedCulture()
		{
			return Game.Current.ObjectManager.GetObject<BasicCultureObject>(this._selectedFaction.CultureCode);
		}

		private Banner GetCreatedClanSigil()
		{
			BasicCultureObject selectedCulture = this.GetSelectedCulture();
			Banner banner = new Banner(selectedCulture.BannerKey, selectedCulture.BackgroundColor1, selectedCulture.ForegroundColor1);
			banner.BannerDataList[1].MeshId = this._selectedSigilIcon.IconID;
			return banner;
		}

		private async void ExecuteTryCreateClan()
		{
			bool areAllInputsValid = true;
			this.ResetErrorTexts();
			CheckClanParameterValidResult checkClanParameterValidResult = await NetworkMain.GameClient.ClanNameExists(this.NameInputText);
			if (!checkClanParameterValidResult.IsValid)
			{
				areAllInputsValid = false;
				this.UpdateNameErrorText(checkClanParameterValidResult.Error);
			}
			TaskAwaiter<bool> taskAwaiter = PlatformServices.Instance.VerifyString(this.NameInputText).GetAwaiter();
			TaskAwaiter<bool> taskAwaiter2;
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				areAllInputsValid = false;
				this.UpdateNameErrorText(4);
			}
			CheckClanParameterValidResult checkClanParameterValidResult2 = await NetworkMain.GameClient.ClanTagExists(this.TagInputText);
			if (!checkClanParameterValidResult2.IsValid)
			{
				areAllInputsValid = false;
				this.UpdateTagErrorText(checkClanParameterValidResult2.Error);
			}
			taskAwaiter = PlatformServices.Instance.VerifyString(this.TagInputText).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				areAllInputsValid = false;
				this.UpdateTagErrorText(4);
			}
			if (this._selectedFaction == null)
			{
				areAllInputsValid = false;
				this.UpdateFactionErrorText();
			}
			if (this._selectedSigilIcon == null)
			{
				areAllInputsValid = false;
				this.UpdateSigilIconErrorText();
			}
			if (areAllInputsValid)
			{
				this.HasCreationStarted = true;
				NetworkMain.GameClient.SendCreateClanMessage(this.NameInputText, this.TagInputText, this.GetSelectedCulture().StringId, this.GetCreatedClanSigil().Serialize());
			}
		}

		public void ExecuteSwitchToWaiting()
		{
			this.PreparePartyMembersList();
			this.IsWaiting = true;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey == null)
			{
				return;
			}
			cancelInputKey.OnFinalize();
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
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
					base.OnPropertyChanged("IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool HasCreationStarted
		{
			get
			{
				return this._hasCreationStarted;
			}
			set
			{
				if (value != this._hasCreationStarted)
				{
					this._hasCreationStarted = value;
					base.OnPropertyChanged("HasCreationStarted");
				}
			}
		}

		[DataSourceProperty]
		public bool IsWaiting
		{
			get
			{
				return this._isWaiting;
			}
			set
			{
				if (value != this._isWaiting)
				{
					this._isWaiting = value;
					base.OnPropertyChanged("IsWaiting");
				}
			}
		}

		[DataSourceProperty]
		public string CreateClanText
		{
			get
			{
				return this._createClanText;
			}
			set
			{
				if (value != this._createClanText)
				{
					this._createClanText = value;
					base.OnPropertyChanged("CreateClanText");
				}
			}
		}

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChanged("NameText");
				}
			}
		}

		[DataSourceProperty]
		public string NameErrorText
		{
			get
			{
				return this._nameErrorText;
			}
			set
			{
				if (value != this._nameErrorText)
				{
					this._nameErrorText = value;
					base.OnPropertyChanged("NameErrorText");
				}
			}
		}

		[DataSourceProperty]
		public string TagText
		{
			get
			{
				return this._tagText;
			}
			set
			{
				if (value != this._tagText)
				{
					this._tagText = value;
					base.OnPropertyChanged("TagText");
				}
			}
		}

		[DataSourceProperty]
		public string TagErrorText
		{
			get
			{
				return this._tagErrorText;
			}
			set
			{
				if (value != this._tagErrorText)
				{
					this._tagErrorText = value;
					base.OnPropertyChanged("TagErrorText");
				}
			}
		}

		[DataSourceProperty]
		public string FactionText
		{
			get
			{
				return this._factionText;
			}
			set
			{
				if (value != this._factionText)
				{
					this._factionText = value;
					base.OnPropertyChanged("FactionText");
				}
			}
		}

		[DataSourceProperty]
		public string FactionErrorText
		{
			get
			{
				return this._factionErrorText;
			}
			set
			{
				if (value != this._factionErrorText)
				{
					this._factionErrorText = value;
					base.OnPropertyChanged("FactionErrorText");
				}
			}
		}

		[DataSourceProperty]
		public string SigilText
		{
			get
			{
				return this._sigilText;
			}
			set
			{
				if (value != this._sigilText)
				{
					this._sigilText = value;
					base.OnPropertyChanged("SigilText");
				}
			}
		}

		[DataSourceProperty]
		public string SigilIconErrorText
		{
			get
			{
				return this._sigilIconErrorText;
			}
			set
			{
				if (value != this._sigilIconErrorText)
				{
					this._sigilIconErrorText = value;
					base.OnPropertyChanged("SigilIconErrorText");
				}
			}
		}

		[DataSourceProperty]
		public string CreateText
		{
			get
			{
				return this._createText;
			}
			set
			{
				if (value != this._createText)
				{
					this._createText = value;
					base.OnPropertyChanged("CreateText");
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
					base.OnPropertyChanged("CancelText");
				}
			}
		}

		[DataSourceProperty]
		public string NameInputText
		{
			get
			{
				return this._nameInputText;
			}
			set
			{
				if (value != this._nameInputText)
				{
					this._nameInputText = value;
					base.OnPropertyChanged("NameInputText");
					this.NameErrorText = "";
				}
			}
		}

		[DataSourceProperty]
		public string TagInputText
		{
			get
			{
				return this._tagInputText;
			}
			set
			{
				if (value != this._tagInputText)
				{
					this._tagInputText = value;
					base.OnPropertyChanged("TagInputText");
					this.TagErrorText = "";
				}
			}
		}

		[DataSourceProperty]
		public string WaitingForConfirmationText
		{
			get
			{
				return this._waitingForConfirmationText;
			}
			set
			{
				if (value != this._waitingForConfirmationText)
				{
					this._waitingForConfirmationText = value;
					base.OnPropertyChanged("WaitingForConfirmationText");
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

		[DataSourceProperty]
		public MBBindingList<MPLobbyClanMemberItemVM> PartyMembersList
		{
			get
			{
				return this._partyMembersList;
			}
			set
			{
				if (value != this._partyMembersList)
				{
					this._partyMembersList = value;
					base.OnPropertyChanged("PartyMembersList");
				}
			}
		}

		private MPCultureItemVM _selectedFaction;

		private MPLobbySigilItemVM _selectedSigilIcon;

		private InputKeyItemVM _cancelInputKey;

		private bool _isEnabled;

		private bool _hasCreationStarted;

		private bool _isWaiting;

		private string _createClanText;

		private string _nameText;

		private string _nameErrorText;

		private string _tagText;

		private string _tagErrorText;

		private string _factionText;

		private string _factionErrorText;

		private string _sigilText;

		private string _sigilIconErrorText;

		private string _createText;

		private string _cancelText;

		private string _nameInputText;

		private string _tagInputText;

		private string _waitingForConfirmationText;

		private MBBindingList<MPCultureItemVM> _factionsList;

		private MBBindingList<MPLobbySigilItemVM> _iconsList;

		private MBBindingList<MPLobbyClanMemberItemVM> _partyMembersList;
	}
}
