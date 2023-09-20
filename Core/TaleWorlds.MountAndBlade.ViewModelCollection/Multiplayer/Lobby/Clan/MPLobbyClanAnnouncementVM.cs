using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	public class MPLobbyClanAnnouncementVM : ViewModel
	{
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

		private void ExecuteDeleteAnnouncement()
		{
			string text = new TextObject("{=P1MybNr7}Delete Announcement", null).ToString();
			string text2 = new TextObject("{=CW2JkWzC}Are you sure want to delete this announcement?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.DeleteAnnouncement), null, "", 0f, null, null, null), false, false);
		}

		private void DeleteAnnouncement()
		{
			NetworkMain.GameClient.RemoveClanAnnouncement(this._id);
		}

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

		private DateTime _announcedDate;

		private PlayerId _senderId;

		private int _id;

		private bool _canBeDeleted;

		private string _messageText;

		private string _details;

		private MPLobbyPlayerBaseVM _senderPlayer;
	}
}
