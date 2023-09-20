using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x02000092 RID: 146
	public class MPLobbyClanAnnouncementVM : ViewModel
	{
		// Token: 0x06000DB2 RID: 3506 RVA: 0x0002F284 File Offset: 0x0002D484
		public MPLobbyClanAnnouncementVM(PlayerId senderId, string message, DateTime date, int id, bool canBeDeleted)
		{
			this._id = id;
			this._senderId = senderId;
			this._announcedDate = date;
			this.SenderPlayer = new MPLobbyPlayerBaseVM(senderId, "", null, null);
			this.MessageText = message;
			this.CanBeDeleted = canBeDeleted;
			this.RefreshValues();
		}

		// Token: 0x06000DB3 RID: 3507 RVA: 0x0002F2D8 File Offset: 0x0002D4D8
		public override void RefreshValues()
		{
			base.RefreshValues();
			string text = new TextObject("{=oMiNaY1E}Posted By", null).ToString();
			GameTexts.SetVariable("STR1", text);
			GameTexts.SetVariable("STR2", this.SenderPlayer.Name);
			string text2 = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			string dateFormattedByLanguage = LocalizedTextManager.GetDateFormattedByLanguage(BannerlordConfig.Language, this._announcedDate);
			GameTexts.SetVariable("STR1", text2);
			GameTexts.SetVariable("STR2", dateFormattedByLanguage);
			this.Details = new TextObject("{=QvDxB57o}{STR1} | {STR2}", null).ToString();
		}

		// Token: 0x06000DB4 RID: 3508 RVA: 0x0002F36C File Offset: 0x0002D56C
		private void ExecuteDeleteAnnouncement()
		{
			string text = new TextObject("{=P1MybNr7}Delete Announcement", null).ToString();
			string text2 = new TextObject("{=CW2JkWzC}Are you sure want to delete this announcement?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.DeleteAnnouncement), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000DB5 RID: 3509 RVA: 0x0002F3E3 File Offset: 0x0002D5E3
		private void DeleteAnnouncement()
		{
			NetworkMain.GameClient.RemoveClanAnnouncement(this._id);
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06000DB6 RID: 3510 RVA: 0x0002F3F5 File Offset: 0x0002D5F5
		// (set) Token: 0x06000DB7 RID: 3511 RVA: 0x0002F3FD File Offset: 0x0002D5FD
		[DataSourceProperty]
		public bool CanBeDeleted
		{
			get
			{
				return this._canBeDeleted;
			}
			set
			{
				if (value != this._canBeDeleted)
				{
					this._canBeDeleted = value;
					base.OnPropertyChanged("CanBeDeleted");
				}
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06000DB8 RID: 3512 RVA: 0x0002F41A File Offset: 0x0002D61A
		// (set) Token: 0x06000DB9 RID: 3513 RVA: 0x0002F422 File Offset: 0x0002D622
		[DataSourceProperty]
		public string MessageText
		{
			get
			{
				return this._messageText;
			}
			set
			{
				if (value != this._messageText)
				{
					this._messageText = value;
					base.OnPropertyChanged("MessageText");
				}
			}
		}

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06000DBA RID: 3514 RVA: 0x0002F444 File Offset: 0x0002D644
		// (set) Token: 0x06000DBB RID: 3515 RVA: 0x0002F44C File Offset: 0x0002D64C
		[DataSourceProperty]
		public string Details
		{
			get
			{
				return this._details;
			}
			set
			{
				if (value != this._details)
				{
					this._details = value;
					base.OnPropertyChanged("Details");
				}
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06000DBC RID: 3516 RVA: 0x0002F46E File Offset: 0x0002D66E
		// (set) Token: 0x06000DBD RID: 3517 RVA: 0x0002F476 File Offset: 0x0002D676
		[DataSourceProperty]
		public MPLobbyPlayerBaseVM SenderPlayer
		{
			get
			{
				return this._senderPlayer;
			}
			set
			{
				if (value != this._senderPlayer)
				{
					this._senderPlayer = value;
					base.OnPropertyChanged("SenderPlayer");
				}
			}
		}

		// Token: 0x0400068F RID: 1679
		private DateTime _announcedDate;

		// Token: 0x04000690 RID: 1680
		private PlayerId _senderId;

		// Token: 0x04000691 RID: 1681
		private int _id;

		// Token: 0x04000692 RID: 1682
		private bool _canBeDeleted;

		// Token: 0x04000693 RID: 1683
		private string _messageText;

		// Token: 0x04000694 RID: 1684
		private string _details;

		// Token: 0x04000695 RID: 1685
		private MPLobbyPlayerBaseVM _senderPlayer;
	}
}
