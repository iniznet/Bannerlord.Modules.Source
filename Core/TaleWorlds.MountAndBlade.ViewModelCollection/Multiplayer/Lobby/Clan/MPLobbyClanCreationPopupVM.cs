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

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x02000096 RID: 150
	public class MPLobbyClanCreationPopupVM : ViewModel
	{
		// Token: 0x06000E12 RID: 3602 RVA: 0x000301A1 File Offset: 0x0002E3A1
		public MPLobbyClanCreationPopupVM()
		{
			this.PartyMembersList = new MBBindingList<MPLobbyClanMemberItemVM>();
			this.PrepareFactionsList();
			this.PrepareSigilIconsList();
			this.RefreshValues();
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x000301C8 File Offset: 0x0002E3C8
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

		// Token: 0x06000E14 RID: 3604 RVA: 0x00030291 File Offset: 0x0002E491
		private void ResetAll()
		{
			this.ResetErrorTexts();
			this.ResetUserInputs();
		}

		// Token: 0x06000E15 RID: 3605 RVA: 0x0003029F File Offset: 0x0002E49F
		private void ResetErrorTexts()
		{
			this.NameErrorText = "";
			this.TagErrorText = "";
			this.FactionErrorText = "";
			this.SigilIconErrorText = "";
		}

		// Token: 0x06000E16 RID: 3606 RVA: 0x000302CD File Offset: 0x0002E4CD
		private void ResetUserInputs()
		{
			this.NameInputText = "";
			this.TagInputText = "";
			this.OnFactionSelection(null);
			this.OnSigilIconSelection(null);
		}

		// Token: 0x06000E17 RID: 3607 RVA: 0x000302F3 File Offset: 0x0002E4F3
		public void ExecuteOpenPopup()
		{
			this.RefreshValues();
			this.IsEnabled = true;
		}

		// Token: 0x06000E18 RID: 3608 RVA: 0x00030302 File Offset: 0x0002E502
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x0003030C File Offset: 0x0002E50C
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

		// Token: 0x06000E1A RID: 3610 RVA: 0x0003044C File Offset: 0x0002E64C
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

		// Token: 0x06000E1B RID: 3611 RVA: 0x00030518 File Offset: 0x0002E718
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

		// Token: 0x06000E1C RID: 3612 RVA: 0x000305B8 File Offset: 0x0002E7B8
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

		// Token: 0x06000E1D RID: 3613 RVA: 0x00030608 File Offset: 0x0002E808
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

		// Token: 0x06000E1E RID: 3614 RVA: 0x00030658 File Offset: 0x0002E858
		private void UpdateNameErrorText(StringValidationError error)
		{
			this.NameErrorText = "";
			if (error == StringValidationError.InvalidLength)
			{
				this.NameErrorText = new TextObject("{=bExIl1A2}Name Length Is Invalid", null).ToString();
				return;
			}
			if (error == StringValidationError.AlreadyExists)
			{
				this.NameErrorText = new TextObject("{=Agtv9l7S}This Name Already Exists", null).ToString();
				return;
			}
			if (error == StringValidationError.HasNonLettersCharacters)
			{
				this.NameErrorText = new TextObject("{=lO1hok44}Name Has Invalid Characters In It", null).ToString();
				return;
			}
			if (error == StringValidationError.ContainsProfanity)
			{
				this.NameErrorText = new TextObject("{=cl2DnRYR}Name Should Not Contain Offensive Words", null).ToString();
				return;
			}
			if (error == StringValidationError.Unspecified)
			{
				this.NameErrorText = new TextObject("{=UEgS8RcB}Name Has Invalid Content", null).ToString();
			}
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x000306F8 File Offset: 0x0002E8F8
		private void UpdateTagErrorText(StringValidationError error)
		{
			this.TagErrorText = "";
			if (error == StringValidationError.InvalidLength)
			{
				this.TagErrorText = new TextObject("{=MjnlWhih}Tag Length Is Invalid", null).ToString();
				return;
			}
			if (error == StringValidationError.AlreadyExists)
			{
				this.TagErrorText = new TextObject("{=ulzyykHO}This Tag Already Exists", null).ToString();
				return;
			}
			if (error == StringValidationError.HasNonLettersCharacters)
			{
				this.TagErrorText = new TextObject("{=FjmxNxZJ}Tag Has Invalid Characters In It", null).ToString();
				return;
			}
			if (error == StringValidationError.ContainsProfanity)
			{
				this.TagErrorText = new TextObject("{=jyJXcOLe}Tag Should Not Contain Offensive Words", null).ToString();
				return;
			}
			if (error == StringValidationError.Unspecified)
			{
				this.TagErrorText = new TextObject("{=hCNnqVgK}Tag Has Invalid Content", null).ToString();
			}
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x00030795 File Offset: 0x0002E995
		public void UpdateFactionErrorText()
		{
			this.FactionErrorText = "";
			this.FactionErrorText = new TextObject("{=p83IO9ls}You must select a culture", null).ToString();
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x000307B8 File Offset: 0x0002E9B8
		public void UpdateSigilIconErrorText()
		{
			this.SigilIconErrorText = "";
			this.SigilIconErrorText = new TextObject("{=uOrwqeQl}You must select a sigil icon", null).ToString();
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x000307DC File Offset: 0x0002E9DC
		public void UpdateConfirmation(PlayerId playerId, ClanCreationAnswer answer)
		{
			foreach (MPLobbyClanMemberItemVM mplobbyClanMemberItemVM in this.PartyMembersList)
			{
				if (mplobbyClanMemberItemVM.ProvidedID == playerId)
				{
					if (answer == ClanCreationAnswer.Accepted)
					{
						mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=JTMegIk4}Accepted", null).ToString();
					}
					else if (answer == ClanCreationAnswer.Declined)
					{
						mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=FgaORzy5}Declined", null).ToString();
					}
				}
			}
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x00030868 File Offset: 0x0002EA68
		private BasicCultureObject GetSelectedCulture()
		{
			return Game.Current.ObjectManager.GetObject<BasicCultureObject>(this._selectedFaction.CultureCode);
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x00030884 File Offset: 0x0002EA84
		private Banner GetCreatedClanSigil()
		{
			BasicCultureObject selectedCulture = this.GetSelectedCulture();
			Banner banner = new Banner(selectedCulture.BannerKey, selectedCulture.BackgroundColor1, selectedCulture.ForegroundColor1);
			banner.BannerDataList[1].MeshId = this._selectedSigilIcon.IconID;
			return banner;
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x000308CC File Offset: 0x0002EACC
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
				this.UpdateNameErrorText(StringValidationError.Unspecified);
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
				this.UpdateTagErrorText(StringValidationError.Unspecified);
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

		// Token: 0x06000E26 RID: 3622 RVA: 0x00030905 File Offset: 0x0002EB05
		public void ExecuteSwitchToWaiting()
		{
			this.PreparePartyMembersList();
			this.IsWaiting = true;
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x00030914 File Offset: 0x0002EB14
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

		// Token: 0x06000E28 RID: 3624 RVA: 0x0003092C File Offset: 0x0002EB2C
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06000E29 RID: 3625 RVA: 0x0003093B File Offset: 0x0002EB3B
		// (set) Token: 0x06000E2A RID: 3626 RVA: 0x00030943 File Offset: 0x0002EB43
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

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06000E2B RID: 3627 RVA: 0x00030960 File Offset: 0x0002EB60
		// (set) Token: 0x06000E2C RID: 3628 RVA: 0x00030968 File Offset: 0x0002EB68
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

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06000E2D RID: 3629 RVA: 0x00030985 File Offset: 0x0002EB85
		// (set) Token: 0x06000E2E RID: 3630 RVA: 0x0003098D File Offset: 0x0002EB8D
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

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06000E2F RID: 3631 RVA: 0x000309AA File Offset: 0x0002EBAA
		// (set) Token: 0x06000E30 RID: 3632 RVA: 0x000309B2 File Offset: 0x0002EBB2
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

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06000E31 RID: 3633 RVA: 0x000309CF File Offset: 0x0002EBCF
		// (set) Token: 0x06000E32 RID: 3634 RVA: 0x000309D7 File Offset: 0x0002EBD7
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

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06000E33 RID: 3635 RVA: 0x000309F9 File Offset: 0x0002EBF9
		// (set) Token: 0x06000E34 RID: 3636 RVA: 0x00030A01 File Offset: 0x0002EC01
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

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06000E35 RID: 3637 RVA: 0x00030A23 File Offset: 0x0002EC23
		// (set) Token: 0x06000E36 RID: 3638 RVA: 0x00030A2B File Offset: 0x0002EC2B
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

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06000E37 RID: 3639 RVA: 0x00030A4D File Offset: 0x0002EC4D
		// (set) Token: 0x06000E38 RID: 3640 RVA: 0x00030A55 File Offset: 0x0002EC55
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

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06000E39 RID: 3641 RVA: 0x00030A77 File Offset: 0x0002EC77
		// (set) Token: 0x06000E3A RID: 3642 RVA: 0x00030A7F File Offset: 0x0002EC7F
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

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06000E3B RID: 3643 RVA: 0x00030AA1 File Offset: 0x0002ECA1
		// (set) Token: 0x06000E3C RID: 3644 RVA: 0x00030AA9 File Offset: 0x0002ECA9
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

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06000E3D RID: 3645 RVA: 0x00030ACB File Offset: 0x0002ECCB
		// (set) Token: 0x06000E3E RID: 3646 RVA: 0x00030AD3 File Offset: 0x0002ECD3
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

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06000E3F RID: 3647 RVA: 0x00030AF5 File Offset: 0x0002ECF5
		// (set) Token: 0x06000E40 RID: 3648 RVA: 0x00030AFD File Offset: 0x0002ECFD
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

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06000E41 RID: 3649 RVA: 0x00030B1F File Offset: 0x0002ED1F
		// (set) Token: 0x06000E42 RID: 3650 RVA: 0x00030B27 File Offset: 0x0002ED27
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

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06000E43 RID: 3651 RVA: 0x00030B49 File Offset: 0x0002ED49
		// (set) Token: 0x06000E44 RID: 3652 RVA: 0x00030B51 File Offset: 0x0002ED51
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

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06000E45 RID: 3653 RVA: 0x00030B73 File Offset: 0x0002ED73
		// (set) Token: 0x06000E46 RID: 3654 RVA: 0x00030B7B File Offset: 0x0002ED7B
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

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06000E47 RID: 3655 RVA: 0x00030B9D File Offset: 0x0002ED9D
		// (set) Token: 0x06000E48 RID: 3656 RVA: 0x00030BA5 File Offset: 0x0002EDA5
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

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06000E49 RID: 3657 RVA: 0x00030BD2 File Offset: 0x0002EDD2
		// (set) Token: 0x06000E4A RID: 3658 RVA: 0x00030BDA File Offset: 0x0002EDDA
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

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06000E4B RID: 3659 RVA: 0x00030C07 File Offset: 0x0002EE07
		// (set) Token: 0x06000E4C RID: 3660 RVA: 0x00030C0F File Offset: 0x0002EE0F
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

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06000E4D RID: 3661 RVA: 0x00030C31 File Offset: 0x0002EE31
		// (set) Token: 0x06000E4E RID: 3662 RVA: 0x00030C39 File Offset: 0x0002EE39
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

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06000E4F RID: 3663 RVA: 0x00030C56 File Offset: 0x0002EE56
		// (set) Token: 0x06000E50 RID: 3664 RVA: 0x00030C5E File Offset: 0x0002EE5E
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

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06000E51 RID: 3665 RVA: 0x00030C7B File Offset: 0x0002EE7B
		// (set) Token: 0x06000E52 RID: 3666 RVA: 0x00030C83 File Offset: 0x0002EE83
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

		// Token: 0x040006B5 RID: 1717
		private MPCultureItemVM _selectedFaction;

		// Token: 0x040006B6 RID: 1718
		private MPLobbySigilItemVM _selectedSigilIcon;

		// Token: 0x040006B7 RID: 1719
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040006B8 RID: 1720
		private bool _isEnabled;

		// Token: 0x040006B9 RID: 1721
		private bool _hasCreationStarted;

		// Token: 0x040006BA RID: 1722
		private bool _isWaiting;

		// Token: 0x040006BB RID: 1723
		private string _createClanText;

		// Token: 0x040006BC RID: 1724
		private string _nameText;

		// Token: 0x040006BD RID: 1725
		private string _nameErrorText;

		// Token: 0x040006BE RID: 1726
		private string _tagText;

		// Token: 0x040006BF RID: 1727
		private string _tagErrorText;

		// Token: 0x040006C0 RID: 1728
		private string _factionText;

		// Token: 0x040006C1 RID: 1729
		private string _factionErrorText;

		// Token: 0x040006C2 RID: 1730
		private string _sigilText;

		// Token: 0x040006C3 RID: 1731
		private string _sigilIconErrorText;

		// Token: 0x040006C4 RID: 1732
		private string _createText;

		// Token: 0x040006C5 RID: 1733
		private string _cancelText;

		// Token: 0x040006C6 RID: 1734
		private string _nameInputText;

		// Token: 0x040006C7 RID: 1735
		private string _tagInputText;

		// Token: 0x040006C8 RID: 1736
		private string _waitingForConfirmationText;

		// Token: 0x040006C9 RID: 1737
		private MBBindingList<MPCultureItemVM> _factionsList;

		// Token: 0x040006CA RID: 1738
		private MBBindingList<MPLobbySigilItemVM> _iconsList;

		// Token: 0x040006CB RID: 1739
		private MBBindingList<MPLobbyClanMemberItemVM> _partyMembersList;
	}
}
