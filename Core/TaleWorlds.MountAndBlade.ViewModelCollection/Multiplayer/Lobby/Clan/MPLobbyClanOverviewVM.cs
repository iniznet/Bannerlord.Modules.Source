using System;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x0200009E RID: 158
	public class MPLobbyClanOverviewVM : ViewModel
	{
		// Token: 0x06000EED RID: 3821 RVA: 0x00032240 File Offset: 0x00030440
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

		// Token: 0x06000EEE RID: 3822 RVA: 0x000322A0 File Offset: 0x000304A0
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

		// Token: 0x06000EEF RID: 3823 RVA: 0x000323A8 File Offset: 0x000305A8
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
				if (announcements.IsEmpty<ClanAnnouncement>())
				{
					this.DoesHaveAnnouncements = false;
				}
				clanInfo = null;
				clanTagInBrackets = null;
			}
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x000323F8 File Offset: 0x000305F8
		private void ExecuteDisbandClan()
		{
			string text = new TextObject("{=oFWcihyW}Disband Clan", null).ToString();
			string text2 = new TextObject("{=vW1VgmaP}Are you sure want to disband your clan?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.DisbandClan), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x0003246F File Offset: 0x0003066F
		private void DisbandClan()
		{
			NetworkMain.GameClient.DestroyClan();
		}

		// Token: 0x06000EF2 RID: 3826 RVA: 0x0003247C File Offset: 0x0003067C
		private void ExecuteLeaveClan()
		{
			string text = new TextObject("{=4ZE6i9nW}Leave Clan", null).ToString();
			string text2 = new TextObject("{=67hsZZor}Are you sure want to leave your clan?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.LeaveClan), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000EF3 RID: 3827 RVA: 0x000324F3 File Offset: 0x000306F3
		private void LeaveClan()
		{
			NetworkMain.GameClient.KickFromClan(NetworkMain.GameClient.PlayerID);
		}

		// Token: 0x06000EF4 RID: 3828 RVA: 0x00032509 File Offset: 0x00030709
		private void ExecuteOpenChangeSigilPopup()
		{
			this.ChangeSigilPopup.ExecuteOpenPopup();
		}

		// Token: 0x06000EF5 RID: 3829 RVA: 0x00032516 File Offset: 0x00030716
		private void ExecuteCloseChangeSigilPopup()
		{
			this.ChangeSigilPopup.ExecuteClosePopup();
		}

		// Token: 0x06000EF6 RID: 3830 RVA: 0x00032523 File Offset: 0x00030723
		private void ExecuteOpenChangeFactionPopup()
		{
			this.ChangeFactionPopup.ExecuteOpenPopup();
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x00032530 File Offset: 0x00030730
		private void ExecuteCloseChangeFactionPopup()
		{
			this.ChangeFactionPopup.ExecuteClosePopup();
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x0003253D File Offset: 0x0003073D
		private void ExecuteOpenSendAnnouncementPopup()
		{
			this.SendAnnouncementPopup.ExecuteOpenPopup();
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x0003254A File Offset: 0x0003074A
		private void ExecuteCloseSendAnnouncementPopup()
		{
			this.SendAnnouncementPopup.ExecuteClosePopup();
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x00032557 File Offset: 0x00030757
		private void ExecuteOpenSetClanInformationPopup()
		{
			this.SetClanInformationPopup.ExecuteOpenPopup();
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x00032564 File Offset: 0x00030764
		private void ExecuteCloseSetClanInformationPopup()
		{
			this.SetClanInformationPopup.ExecuteClosePopup();
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x00032571 File Offset: 0x00030771
		private void ExecuteOpenInviteClanMemberPopup()
		{
			Action openInviteClanMemberPopup = this._openInviteClanMemberPopup;
			if (openInviteClanMemberPopup == null)
			{
				return;
			}
			openInviteClanMemberPopup();
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x00032583 File Offset: 0x00030783
		private void CloseAllPopups()
		{
			this.ChangeSigilPopup.ExecuteClosePopup();
			this.ChangeFactionPopup.ExecuteClosePopup();
			this.SendAnnouncementPopup.ExecuteClosePopup();
			this.SetClanInformationPopup.ExecuteClosePopup();
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06000EFE RID: 3838 RVA: 0x000325B1 File Offset: 0x000307B1
		// (set) Token: 0x06000EFF RID: 3839 RVA: 0x000325B9 File Offset: 0x000307B9
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

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06000F00 RID: 3840 RVA: 0x000325D6 File Offset: 0x000307D6
		// (set) Token: 0x06000F01 RID: 3841 RVA: 0x000325DE File Offset: 0x000307DE
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

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06000F02 RID: 3842 RVA: 0x000325FB File Offset: 0x000307FB
		// (set) Token: 0x06000F03 RID: 3843 RVA: 0x00032603 File Offset: 0x00030803
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

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06000F04 RID: 3844 RVA: 0x00032620 File Offset: 0x00030820
		// (set) Token: 0x06000F05 RID: 3845 RVA: 0x00032628 File Offset: 0x00030828
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

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06000F06 RID: 3846 RVA: 0x00032645 File Offset: 0x00030845
		// (set) Token: 0x06000F07 RID: 3847 RVA: 0x0003264D File Offset: 0x0003084D
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

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06000F08 RID: 3848 RVA: 0x0003266A File Offset: 0x0003086A
		// (set) Token: 0x06000F09 RID: 3849 RVA: 0x00032672 File Offset: 0x00030872
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

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06000F0A RID: 3850 RVA: 0x0003268F File Offset: 0x0003088F
		// (set) Token: 0x06000F0B RID: 3851 RVA: 0x00032697 File Offset: 0x00030897
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

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06000F0C RID: 3852 RVA: 0x000326B9 File Offset: 0x000308B9
		// (set) Token: 0x06000F0D RID: 3853 RVA: 0x000326C1 File Offset: 0x000308C1
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

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06000F0E RID: 3854 RVA: 0x000326E3 File Offset: 0x000308E3
		// (set) Token: 0x06000F0F RID: 3855 RVA: 0x000326EB File Offset: 0x000308EB
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

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06000F10 RID: 3856 RVA: 0x0003270D File Offset: 0x0003090D
		// (set) Token: 0x06000F11 RID: 3857 RVA: 0x00032715 File Offset: 0x00030915
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

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06000F12 RID: 3858 RVA: 0x00032737 File Offset: 0x00030937
		// (set) Token: 0x06000F13 RID: 3859 RVA: 0x0003273F File Offset: 0x0003093F
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

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06000F14 RID: 3860 RVA: 0x00032761 File Offset: 0x00030961
		// (set) Token: 0x06000F15 RID: 3861 RVA: 0x00032769 File Offset: 0x00030969
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

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x0003278B File Offset: 0x0003098B
		// (set) Token: 0x06000F17 RID: 3863 RVA: 0x00032793 File Offset: 0x00030993
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

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06000F18 RID: 3864 RVA: 0x000327B5 File Offset: 0x000309B5
		// (set) Token: 0x06000F19 RID: 3865 RVA: 0x000327BD File Offset: 0x000309BD
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

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06000F1A RID: 3866 RVA: 0x000327DF File Offset: 0x000309DF
		// (set) Token: 0x06000F1B RID: 3867 RVA: 0x000327E7 File Offset: 0x000309E7
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

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06000F1C RID: 3868 RVA: 0x00032809 File Offset: 0x00030A09
		// (set) Token: 0x06000F1D RID: 3869 RVA: 0x00032811 File Offset: 0x00030A11
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

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06000F1E RID: 3870 RVA: 0x00032833 File Offset: 0x00030A33
		// (set) Token: 0x06000F1F RID: 3871 RVA: 0x0003283B File Offset: 0x00030A3B
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

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06000F20 RID: 3872 RVA: 0x0003285D File Offset: 0x00030A5D
		// (set) Token: 0x06000F21 RID: 3873 RVA: 0x00032865 File Offset: 0x00030A65
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

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06000F22 RID: 3874 RVA: 0x00032887 File Offset: 0x00030A87
		// (set) Token: 0x06000F23 RID: 3875 RVA: 0x0003288F File Offset: 0x00030A8F
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

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06000F24 RID: 3876 RVA: 0x000328B1 File Offset: 0x00030AB1
		// (set) Token: 0x06000F25 RID: 3877 RVA: 0x000328B9 File Offset: 0x00030AB9
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

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06000F26 RID: 3878 RVA: 0x000328D6 File Offset: 0x00030AD6
		// (set) Token: 0x06000F27 RID: 3879 RVA: 0x000328DE File Offset: 0x00030ADE
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

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06000F28 RID: 3880 RVA: 0x000328FB File Offset: 0x00030AFB
		// (set) Token: 0x06000F29 RID: 3881 RVA: 0x00032903 File Offset: 0x00030B03
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

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06000F2A RID: 3882 RVA: 0x00032920 File Offset: 0x00030B20
		// (set) Token: 0x06000F2B RID: 3883 RVA: 0x00032928 File Offset: 0x00030B28
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

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06000F2C RID: 3884 RVA: 0x00032945 File Offset: 0x00030B45
		// (set) Token: 0x06000F2D RID: 3885 RVA: 0x0003294D File Offset: 0x00030B4D
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

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06000F2E RID: 3886 RVA: 0x0003296A File Offset: 0x00030B6A
		// (set) Token: 0x06000F2F RID: 3887 RVA: 0x00032972 File Offset: 0x00030B72
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

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06000F30 RID: 3888 RVA: 0x0003298F File Offset: 0x00030B8F
		// (set) Token: 0x06000F31 RID: 3889 RVA: 0x00032997 File Offset: 0x00030B97
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

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06000F32 RID: 3890 RVA: 0x000329B4 File Offset: 0x00030BB4
		// (set) Token: 0x06000F33 RID: 3891 RVA: 0x000329BC File Offset: 0x00030BBC
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

		// Token: 0x04000715 RID: 1813
		private readonly Action _openInviteClanMemberPopup;

		// Token: 0x04000716 RID: 1814
		private bool _isSelected;

		// Token: 0x04000717 RID: 1815
		private bool _isLeader;

		// Token: 0x04000718 RID: 1816
		private bool _isPrivilegedMember;

		// Token: 0x04000719 RID: 1817
		private bool _areActionButtonsEnabled;

		// Token: 0x0400071A RID: 1818
		private bool _doesHaveDescription;

		// Token: 0x0400071B RID: 1819
		private bool _doesHaveAnnouncements;

		// Token: 0x0400071C RID: 1820
		private string _nameText;

		// Token: 0x0400071D RID: 1821
		private string _membersText;

		// Token: 0x0400071E RID: 1822
		private string _changeSigilText;

		// Token: 0x0400071F RID: 1823
		private string _changeFactionText;

		// Token: 0x04000720 RID: 1824
		private string _leaveText;

		// Token: 0x04000721 RID: 1825
		private string _disbandText;

		// Token: 0x04000722 RID: 1826
		private string _factionCultureID;

		// Token: 0x04000723 RID: 1827
		private string _informationText;

		// Token: 0x04000724 RID: 1828
		private string _announcementsText;

		// Token: 0x04000725 RID: 1829
		private string _clanDescriptionText;

		// Token: 0x04000726 RID: 1830
		private string _noDescriptionText;

		// Token: 0x04000727 RID: 1831
		private string _noAnnouncementsText;

		// Token: 0x04000728 RID: 1832
		private string _titleText;

		// Token: 0x04000729 RID: 1833
		private ImageIdentifierVM _sigilImage;

		// Token: 0x0400072A RID: 1834
		private MPLobbyClanChangeSigilPopupVM _changeSigilPopup;

		// Token: 0x0400072B RID: 1835
		private MPLobbyClanChangeFactionPopupVM _changeFactionPopup;

		// Token: 0x0400072C RID: 1836
		private MBBindingList<MPLobbyClanAnnouncementVM> _announcementsList;

		// Token: 0x0400072D RID: 1837
		private MPLobbyClanSendPostPopupVM _sendAnnouncementPopup;

		// Token: 0x0400072E RID: 1838
		private MPLobbyClanSendPostPopupVM _setClanInformationPopup;

		// Token: 0x0400072F RID: 1839
		private HintViewModel _cantLeaveHint;

		// Token: 0x04000730 RID: 1840
		private HintViewModel _inviteMembersHint;
	}
}
