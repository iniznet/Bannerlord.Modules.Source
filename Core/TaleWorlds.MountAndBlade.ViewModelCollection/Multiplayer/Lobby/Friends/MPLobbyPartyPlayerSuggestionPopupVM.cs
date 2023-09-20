using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	public class MPLobbyPartyPlayerSuggestionPopupVM : ViewModel
	{
		public MPLobbyPartyPlayerSuggestionPopupVM()
		{
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=q2Y7aHSF}Invite Suggestion", null).ToString();
			this.DoYouWantToInviteText = new TextObject("{=VFqoa6vD}Do you want to invite this player to your party?", null).ToString();
		}

		public void OpenWith(MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData data)
		{
			this._suggestedPlayerId = data.PlayerId;
			this.SuggestedPlayer = new MPLobbyPlayerBaseVM(data.PlayerId, "", null, null);
			TextObject textObject = new TextObject("{=C7OHivNl}Your friend <a style=\"Strong\"><b>{PLAYER_NAME}</b></a> wants you to invite the player below to your party.", null);
			GameTexts.SetVariable("PLAYER_NAME", data.SuggestingPlayerName);
			this.PlayerSuggestedText = textObject.ToString();
			this.IsEnabled = true;
		}

		public void Close()
		{
			this.IsEnabled = false;
		}

		private void ExecuteAcceptSuggestion()
		{
			PlatformServices.Instance.CheckPrivilege(Privilege.Communication, true, delegate(bool result)
			{
				if (result)
				{
					PlatformServices.Instance.CheckPermissionWithUser(Permission.PlayMultiplayer, this._suggestedPlayerId, async delegate(bool permissionResult)
					{
						if (permissionResult)
						{
							if (PlatformServices.InvitationServices != null)
							{
								await NetworkMain.GameClient.InviteToPlatformSession(this._suggestedPlayerId);
							}
							else
							{
								bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(this._suggestedPlayerId);
								NetworkMain.GameClient.InviteToParty(this._suggestedPlayerId, flag);
							}
						}
						this.Close();
					});
					return;
				}
				this.Close();
			});
		}

		private void ExecuteDeclineSuggestion()
		{
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
		public string DoYouWantToInviteText
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
					base.OnPropertyChanged("DoYouWantToInviteText");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerSuggestedText
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
					base.OnPropertyChanged("PlayerSuggestedText");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyPlayerBaseVM SuggestedPlayer
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
					base.OnPropertyChanged("SuggestedPlayer");
				}
			}
		}

		private PlayerId _suggestedPlayerId;

		private bool _isEnabled;

		private string _titleText;

		private string _doYouWantToInviteText;

		private string _playerSuggestedText;

		private MPLobbyPlayerBaseVM _suggestedPlayer;

		public class PlayerPartySuggestionData
		{
			public PlayerId PlayerId { get; private set; }

			public string PlayerName { get; private set; }

			public PlayerId SuggestingPlayerId { get; private set; }

			public string SuggestingPlayerName { get; private set; }

			public PlayerPartySuggestionData(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
			{
				this.PlayerId = playerId;
				this.PlayerName = playerName;
				this.SuggestingPlayerId = suggestingPlayerId;
				this.SuggestingPlayerName = suggestingPlayerName;
			}
		}
	}
}
