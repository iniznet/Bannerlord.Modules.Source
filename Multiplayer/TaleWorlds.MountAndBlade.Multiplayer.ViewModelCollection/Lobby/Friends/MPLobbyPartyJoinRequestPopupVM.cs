using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Friends
{
	public class MPLobbyPartyJoinRequestPopupVM : ViewModel
	{
		public MPLobbyPartyJoinRequestPopupVM()
		{
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=re37GzKI}Party Join Request", null).ToString();
			this.AcceptJoinRequestText = new TextObject("{=Ogr2N5bx}Accept request to join to your party?", null).ToString();
		}

		public void OpenWith(PlayerId joiningPlayer, PlayerId viaPlayerId, string viaPlayerName)
		{
			this._viaPlayerId = viaPlayerId;
			this.JoiningPlayer = new MPLobbyPlayerBaseVM(joiningPlayer, "", null, null);
			if (viaPlayerId == NetworkMain.GameClient.PlayerID)
			{
				TextObject textObject = new TextObject("{=BcEN71ts}Player wants to join your party.", null);
				this.JoiningPlayerText = textObject.ToString();
			}
			else
			{
				TextObject textObject = new TextObject("{=q3uBjUyB}Player wants to join your party through your party member <a style=\"Strong\"><b>{PLAYER_NAME}</b></a>.", null);
				GameTexts.SetVariable("PLAYER_NAME", viaPlayerName);
				this.JoiningPlayerText = textObject.ToString();
			}
			this.IsEnabled = true;
		}

		public void OpenWithNewParty(PlayerId joiningPlayer)
		{
			this.JoiningPlayer = new MPLobbyPlayerBaseVM(joiningPlayer, "", null, null);
			this.JoiningPlayerText = "";
			this.IsEnabled = true;
		}

		public void Close()
		{
			this.IsEnabled = false;
		}

		private void ExecuteAcceptJoinRequest()
		{
			PlatformServices.Instance.CheckPrivilege(3, true, delegate(bool result)
			{
				if (result)
				{
					PlatformServices.Instance.CheckPermissionWithUser(0, this.JoiningPlayer.ProvidedID, delegate(bool permissionResult)
					{
						if (permissionResult)
						{
							NetworkMain.GameClient.AcceptPartyJoinRequest(this.JoiningPlayer.ProvidedID);
						}
						else
						{
							NetworkMain.GameClient.DeclinePartyJoinRequest(this.JoiningPlayer.ProvidedID, 1);
						}
						this.Close();
					});
					return;
				}
				NetworkMain.GameClient.DeclinePartyJoinRequest(this.JoiningPlayer.ProvidedID, 1);
				this.Close();
			});
		}

		private void ExecuteDeclineJoinRequest()
		{
			NetworkMain.GameClient.DeclinePartyJoinRequest(this.JoiningPlayer.ProvidedID, 2);
			this.Close();
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
		public string AcceptJoinRequestText
		{
			get
			{
				return this._doYouWantToInviteText;
			}
			set
			{
				if (value != this._doYouWantToInviteText)
				{
					this._doYouWantToInviteText = value;
					base.OnPropertyChanged("AcceptJoinRequestText");
				}
			}
		}

		[DataSourceProperty]
		public string JoiningPlayerText
		{
			get
			{
				return this._playerSuggestedText;
			}
			set
			{
				if (value != this._playerSuggestedText)
				{
					this._playerSuggestedText = value;
					base.OnPropertyChanged("JoiningPlayerText");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyPlayerBaseVM JoiningPlayer
		{
			get
			{
				return this._suggestedPlayer;
			}
			set
			{
				if (value != this._suggestedPlayer)
				{
					this._suggestedPlayer = value;
					base.OnPropertyChanged("JoiningPlayer");
				}
			}
		}

		private PlayerId _viaPlayerId;

		private bool _isEnabled;

		private string _titleText;

		private string _doYouWantToInviteText;

		private string _playerSuggestedText;

		private MPLobbyPlayerBaseVM _suggestedPlayer;
	}
}
