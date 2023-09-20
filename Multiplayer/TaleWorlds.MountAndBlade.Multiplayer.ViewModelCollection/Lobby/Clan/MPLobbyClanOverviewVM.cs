using System;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Clan
{
	public class MPLobbyClanOverviewVM : ViewModel
	{
		public MPLobbyClanOverviewVM(Action openInviteClanMemberPopup)
		{
			this._openInviteClanMemberPopup = openInviteClanMemberPopup;
			this.AnnouncementsList = new MBBindingList<MPLobbyClanAnnouncementVM>();
			this.ChangeSigilPopup = new MPLobbyClanChangeSigilPopupVM();
			this.ChangeFactionPopup = new MPLobbyClanChangeFactionPopupVM();
			this.SendAnnouncementPopup = new MPLobbyClanSendPostPopupVM(MPLobbyClanSendPostPopupVM.PostPopupMode.Announcement);
			this.SetClanInformationPopup = new MPLobbyClanSendPostPopupVM(MPLobbyClanSendPostPopupVM.PostPopupMode.Information);
			this.AreActionButtonsEnabled = true;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ChangeSigilText = new TextObject("{=7R0i82Nw}Change Sigil", null).ToString();
			this.ChangeFactionText = new TextObject("{=aGGq9lJT}Change Culture", null).ToString();
			this.LeaveText = new TextObject("{=3sRdGQou}Leave", null).ToString();
			this.DisbandText = new TextObject("{=xXSFaGW8}Disband", null).ToString();
			this.InformationText = new TextObject("{=SyklU5aP}Information", null).ToString();
			this.AnnouncementsText = new TextObject("{=JY2pBVHQ}Announcements", null).ToString();
			this.NoAnnouncementsText = new TextObject("{=0af2iQvw}Clan doesn't have any announcements", null).ToString();
			this.NoDescriptionText = new TextObject("{=NwiYsUwm}Clan doesn't have a description", null).ToString();
			this.TitleText = new TextObject("{=r223yChR}Overview", null).ToString();
			this.CantLeaveHint = new HintViewModel(new TextObject("{=76HlhP7r}You have to give leadership to another member to leave", null), null);
			this.InviteMembersHint = new HintViewModel(new TextObject("{=tSMckUw3}Invite Members", null), null);
		}

		public async Task RefreshClanInformation(ClanHomeInfo info)
		{
			if (info == null || info.ClanInfo == null)
			{
				this.CloseAllPopups();
			}
			else
			{
				ClanInfo clanInfo = info.ClanInfo;
				GameTexts.SetVariable("STR", clanInfo.Tag);
				string clanTagInBrackets = new TextObject("{=uTXYEAOg}[{STR}]", null).ToString();
				string text = await PlatformServices.FilterString(clanInfo.Name, new TextObject("{=wNUcqcJP}Clan Name", null).ToString());
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", clanTagInBrackets);
				this.NameText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
				GameTexts.SetVariable("LEFT", new TextObject("{=lBn2pSBL}Members", null).ToString());
				GameTexts.SetVariable("RIGHT", clanInfo.Players.Length);
				this.MembersText = GameTexts.FindText("str_LEFT_colon_RIGHT", null).ToString();
				this.SigilImage = new ImageIdentifierVM(BannerCode.CreateFrom(clanInfo.Sigil), true);
				this.FactionCultureID = clanInfo.Faction;
				BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(this.FactionCultureID);
				this.CultureColor1 = Color.FromUint((@object != null) ? @object.Color : 0U);
				this.CultureColor2 = Color.FromUint((@object != null) ? @object.Color2 : 0U);
				this.IsLeader = NetworkMain.GameClient.IsClanLeader;
				this.IsPrivilegedMember = this.IsLeader || NetworkMain.GameClient.IsClanOfficer;
				this.ClanDescriptionText = clanInfo.InformationText;
				this.DoesHaveDescription = true;
				if (string.IsNullOrEmpty(clanInfo.InformationText))
				{
					this.DoesHaveDescription = false;
				}
				this.AnnouncementsList.Clear();
				ClanAnnouncement[] announcements = clanInfo.Announcements;
				foreach (ClanAnnouncement clanAnnouncement in announcements)
				{
					this.AnnouncementsList.Add(new MPLobbyClanAnnouncementVM(clanAnnouncement.AuthorId, clanAnnouncement.Announcement, clanAnnouncement.CreationTime, clanAnnouncement.Id, this.IsPrivilegedMember));
				}
				this.DoesHaveAnnouncements = true;
				if (Extensions.IsEmpty<ClanAnnouncement>(announcements))
				{
					this.DoesHaveAnnouncements = false;
				}
				clanInfo = null;
				clanTagInBrackets = null;
			}
		}

		private void ExecuteDisbandClan()
		{
			string text = new TextObject("{=oFWcihyW}Disband Clan", null).ToString();
			string text2 = new TextObject("{=vW1VgmaP}Are you sure want to disband your clan?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.DisbandClan), null, "", 0f, null, null, null), false, false);
		}

		private void DisbandClan()
		{
			NetworkMain.GameClient.DestroyClan();
		}

		private void ExecuteLeaveClan()
		{
			string text = new TextObject("{=4ZE6i9nW}Leave Clan", null).ToString();
			string text2 = new TextObject("{=67hsZZor}Are you sure want to leave your clan?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.LeaveClan), null, "", 0f, null, null, null), false, false);
		}

		private void LeaveClan()
		{
			NetworkMain.GameClient.KickFromClan(NetworkMain.GameClient.PlayerID);
		}

		private void ExecuteOpenChangeSigilPopup()
		{
			this.ChangeSigilPopup.ExecuteOpenPopup();
		}

		private void ExecuteCloseChangeSigilPopup()
		{
			this.ChangeSigilPopup.ExecuteClosePopup();
		}

		private void ExecuteOpenChangeFactionPopup()
		{
			this.ChangeFactionPopup.ExecuteOpenPopup();
		}

		private void ExecuteCloseChangeFactionPopup()
		{
			this.ChangeFactionPopup.ExecuteClosePopup();
		}

		private void ExecuteOpenSendAnnouncementPopup()
		{
			this.SendAnnouncementPopup.ExecuteOpenPopup();
		}

		private void ExecuteCloseSendAnnouncementPopup()
		{
			this.SendAnnouncementPopup.ExecuteClosePopup();
		}

		private void ExecuteOpenSetClanInformationPopup()
		{
			this.SetClanInformationPopup.ExecuteOpenPopup();
		}

		private void ExecuteCloseSetClanInformationPopup()
		{
			this.SetClanInformationPopup.ExecuteClosePopup();
		}

		private void ExecuteOpenInviteClanMemberPopup()
		{
			Action openInviteClanMemberPopup = this._openInviteClanMemberPopup;
			if (openInviteClanMemberPopup == null)
			{
				return;
			}
			openInviteClanMemberPopup();
		}

		private void CloseAllPopups()
		{
			this.ChangeSigilPopup.ExecuteClosePopup();
			this.ChangeFactionPopup.ExecuteClosePopup();
			this.SendAnnouncementPopup.ExecuteClosePopup();
			this.SetClanInformationPopup.ExecuteClosePopup();
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
		public bool IsLeader
		{
			get
			{
				return this._isLeader;
			}
			set
			{
				if (value != this._isLeader)
				{
					this._isLeader = value;
					base.OnPropertyChanged("IsLeader");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPrivilegedMember
		{
			get
			{
				return this._isPrivilegedMember;
			}
			set
			{
				if (value != this._isPrivilegedMember)
				{
					this._isPrivilegedMember = value;
					base.OnPropertyChanged("IsPrivilegedMember");
				}
			}
		}

		[DataSourceProperty]
		public bool AreActionButtonsEnabled
		{
			get
			{
				return this._areActionButtonsEnabled;
			}
			set
			{
				if (value != this._areActionButtonsEnabled)
				{
					this._areActionButtonsEnabled = value;
					base.OnPropertyChanged("AreActionButtonsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool DoesHaveDescription
		{
			get
			{
				return this._doesHaveDescription;
			}
			set
			{
				if (value != this._doesHaveDescription)
				{
					this._doesHaveDescription = value;
					base.OnPropertyChanged("DoesHaveDescription");
				}
			}
		}

		[DataSourceProperty]
		public bool DoesHaveAnnouncements
		{
			get
			{
				return this._doesHaveAnnouncements;
			}
			set
			{
				if (value != this._doesHaveAnnouncements)
				{
					this._doesHaveAnnouncements = value;
					base.OnPropertyChanged("DoesHaveAnnouncements");
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
		public string MembersText
		{
			get
			{
				return this._membersText;
			}
			set
			{
				if (value != this._membersText)
				{
					this._membersText = value;
					base.OnPropertyChanged("MembersText");
				}
			}
		}

		[DataSourceProperty]
		public string ChangeSigilText
		{
			get
			{
				return this._changeSigilText;
			}
			set
			{
				if (value != this._changeSigilText)
				{
					this._changeSigilText = value;
					base.OnPropertyChanged("ChangeSigilText");
				}
			}
		}

		[DataSourceProperty]
		public string ChangeFactionText
		{
			get
			{
				return this._changeFactionText;
			}
			set
			{
				if (value != this._changeFactionText)
				{
					this._changeFactionText = value;
					base.OnPropertyChanged("ChangeFactionText");
				}
			}
		}

		[DataSourceProperty]
		public string LeaveText
		{
			get
			{
				return this._leaveText;
			}
			set
			{
				if (value != this._leaveText)
				{
					this._leaveText = value;
					base.OnPropertyChanged("LeaveText");
				}
			}
		}

		[DataSourceProperty]
		public string DisbandText
		{
			get
			{
				return this._disbandText;
			}
			set
			{
				if (value != this._disbandText)
				{
					this._disbandText = value;
					base.OnPropertyChanged("DisbandText");
				}
			}
		}

		[DataSourceProperty]
		public string FactionCultureID
		{
			get
			{
				return this._factionCultureID;
			}
			set
			{
				if (value != this._factionCultureID)
				{
					this._factionCultureID = value;
					base.OnPropertyChanged("FactionCultureID");
				}
			}
		}

		[DataSourceProperty]
		public Color CultureColor1
		{
			get
			{
				return this._cultureColor1;
			}
			set
			{
				if (value != this._cultureColor1)
				{
					this._cultureColor1 = value;
					base.OnPropertyChangedWithValue(value, "CultureColor1");
				}
			}
		}

		[DataSourceProperty]
		public Color CultureColor2
		{
			get
			{
				return this._cultureColor2;
			}
			set
			{
				if (value != this._cultureColor2)
				{
					this._cultureColor2 = value;
					base.OnPropertyChangedWithValue(value, "CultureColor2");
				}
			}
		}

		[DataSourceProperty]
		public string InformationText
		{
			get
			{
				return this._informationText;
			}
			set
			{
				if (value != this._informationText)
				{
					this._informationText = value;
					base.OnPropertyChanged("InformationText");
				}
			}
		}

		[DataSourceProperty]
		public string AnnouncementsText
		{
			get
			{
				return this._announcementsText;
			}
			set
			{
				if (value != this._announcementsText)
				{
					this._announcementsText = value;
					base.OnPropertyChanged("AnnouncementsText");
				}
			}
		}

		[DataSourceProperty]
		public string ClanDescriptionText
		{
			get
			{
				return this._clanDescriptionText;
			}
			set
			{
				if (value != this._clanDescriptionText)
				{
					this._clanDescriptionText = value;
					base.OnPropertyChanged("ClanDescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public string NoDescriptionText
		{
			get
			{
				return this._noDescriptionText;
			}
			set
			{
				if (value != this._noDescriptionText)
				{
					this._noDescriptionText = value;
					base.OnPropertyChanged("NoDescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public string NoAnnouncementsText
		{
			get
			{
				return this._noAnnouncementsText;
			}
			set
			{
				if (value != this._noAnnouncementsText)
				{
					this._noAnnouncementsText = value;
					base.OnPropertyChanged("NoAnnouncementsText");
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
		public ImageIdentifierVM SigilImage
		{
			get
			{
				return this._sigilImage;
			}
			set
			{
				if (value != this._sigilImage)
				{
					this._sigilImage = value;
					base.OnPropertyChanged("SigilImage");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyClanChangeSigilPopupVM ChangeSigilPopup
		{
			get
			{
				return this._changeSigilPopup;
			}
			set
			{
				if (value != this._changeSigilPopup)
				{
					this._changeSigilPopup = value;
					base.OnPropertyChanged("ChangeSigilPopup");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyClanChangeFactionPopupVM ChangeFactionPopup
		{
			get
			{
				return this._changeFactionPopup;
			}
			set
			{
				if (value != this._changeFactionPopup)
				{
					this._changeFactionPopup = value;
					base.OnPropertyChanged("ChangeFactionPopup");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPLobbyClanAnnouncementVM> AnnouncementsList
		{
			get
			{
				return this._announcementsList;
			}
			set
			{
				if (value != this._announcementsList)
				{
					this._announcementsList = value;
					base.OnPropertyChanged("AnnouncementsList");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyClanSendPostPopupVM SendAnnouncementPopup
		{
			get
			{
				return this._sendAnnouncementPopup;
			}
			set
			{
				if (value != this._sendAnnouncementPopup)
				{
					this._sendAnnouncementPopup = value;
					base.OnPropertyChanged("SendAnnouncementPopup");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyClanSendPostPopupVM SetClanInformationPopup
		{
			get
			{
				return this._setClanInformationPopup;
			}
			set
			{
				if (value != this._setClanInformationPopup)
				{
					this._setClanInformationPopup = value;
					base.OnPropertyChanged("SetClanInformationPopup");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel CantLeaveHint
		{
			get
			{
				return this._cantLeaveHint;
			}
			set
			{
				if (value != this._cantLeaveHint)
				{
					this._cantLeaveHint = value;
					base.OnPropertyChanged("CantLeaveHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel InviteMembersHint
		{
			get
			{
				return this._inviteMembersHint;
			}
			set
			{
				if (value != this._inviteMembersHint)
				{
					this._inviteMembersHint = value;
					base.OnPropertyChanged("InviteMembersHint");
				}
			}
		}

		private readonly Action _openInviteClanMemberPopup;

		private bool _isSelected;

		private bool _isLeader;

		private bool _isPrivilegedMember;

		private bool _areActionButtonsEnabled;

		private bool _doesHaveDescription;

		private bool _doesHaveAnnouncements;

		private string _nameText;

		private string _membersText;

		private string _changeSigilText;

		private string _changeFactionText;

		private string _leaveText;

		private string _disbandText;

		private string _factionCultureID;

		private string _informationText;

		private string _announcementsText;

		private string _clanDescriptionText;

		private string _noDescriptionText;

		private string _noAnnouncementsText;

		private string _titleText;

		private Color _cultureColor1;

		private Color _cultureColor2;

		private ImageIdentifierVM _sigilImage;

		private MPLobbyClanChangeSigilPopupVM _changeSigilPopup;

		private MPLobbyClanChangeFactionPopupVM _changeFactionPopup;

		private MBBindingList<MPLobbyClanAnnouncementVM> _announcementsList;

		private MPLobbyClanSendPostPopupVM _sendAnnouncementPopup;

		private MPLobbyClanSendPostPopupVM _setClanInformationPopup;

		private HintViewModel _cantLeaveHint;

		private HintViewModel _inviteMembersHint;
	}
}
