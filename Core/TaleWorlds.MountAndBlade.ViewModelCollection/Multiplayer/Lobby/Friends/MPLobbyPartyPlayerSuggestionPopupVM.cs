using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	// Token: 0x02000084 RID: 132
	public class MPLobbyPartyPlayerSuggestionPopupVM : ViewModel
	{
		// Token: 0x06000BD7 RID: 3031 RVA: 0x0002A2D7 File Offset: 0x000284D7
		public MPLobbyPartyPlayerSuggestionPopupVM()
		{
			this.RefreshValues();
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x0002A2E5 File Offset: 0x000284E5
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=q2Y7aHSF}Invite Suggestion", null).ToString();
			this.DoYouWantToInviteText = new TextObject("{=VFqoa6vD}Do you want to invite this player to your party?", null).ToString();
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x0002A31C File Offset: 0x0002851C
		public void OpenWith(MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData data)
		{
			this._suggestedPlayerId = data.PlayerId;
			this.SuggestedPlayer = new MPLobbyPlayerBaseVM(data.PlayerId, "", null, null);
			TextObject textObject = new TextObject("{=C7OHivNl}Your friend <a style=\"Strong\"><b>{PLAYER_NAME}</b></a> wants you to invite the player below to your party.", null);
			GameTexts.SetVariable("PLAYER_NAME", data.SuggestingPlayerName);
			this.PlayerSuggestedText = textObject.ToString();
			this.IsEnabled = true;
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x0002A37C File Offset: 0x0002857C
		public void Close()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x0002A385 File Offset: 0x00028585
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

		// Token: 0x06000BDC RID: 3036 RVA: 0x0002A39F File Offset: 0x0002859F
		private void ExecuteDeclineSuggestion()
		{
			this.Close();
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06000BDD RID: 3037 RVA: 0x0002A3A7 File Offset: 0x000285A7
		// (set) Token: 0x06000BDE RID: 3038 RVA: 0x0002A3AF File Offset: 0x000285AF
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

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06000BDF RID: 3039 RVA: 0x0002A3CC File Offset: 0x000285CC
		// (set) Token: 0x06000BE0 RID: 3040 RVA: 0x0002A3D4 File Offset: 0x000285D4
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

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06000BE1 RID: 3041 RVA: 0x0002A3F6 File Offset: 0x000285F6
		// (set) Token: 0x06000BE2 RID: 3042 RVA: 0x0002A3FE File Offset: 0x000285FE
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

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06000BE3 RID: 3043 RVA: 0x0002A420 File Offset: 0x00028620
		// (set) Token: 0x06000BE4 RID: 3044 RVA: 0x0002A428 File Offset: 0x00028628
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

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x0002A44A File Offset: 0x0002864A
		// (set) Token: 0x06000BE6 RID: 3046 RVA: 0x0002A452 File Offset: 0x00028652
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

		// Token: 0x040005A2 RID: 1442
		private PlayerId _suggestedPlayerId;

		// Token: 0x040005A3 RID: 1443
		private bool _isEnabled;

		// Token: 0x040005A4 RID: 1444
		private string _titleText;

		// Token: 0x040005A5 RID: 1445
		private string _doYouWantToInviteText;

		// Token: 0x040005A6 RID: 1446
		private string _playerSuggestedText;

		// Token: 0x040005A7 RID: 1447
		private MPLobbyPlayerBaseVM _suggestedPlayer;

		// Token: 0x020001C4 RID: 452
		public class PlayerPartySuggestionData
		{
			// Token: 0x170007D9 RID: 2009
			// (get) Token: 0x06001A35 RID: 6709 RVA: 0x00054767 File Offset: 0x00052967
			// (set) Token: 0x06001A36 RID: 6710 RVA: 0x0005476F File Offset: 0x0005296F
			public PlayerId PlayerId { get; private set; }

			// Token: 0x170007DA RID: 2010
			// (get) Token: 0x06001A37 RID: 6711 RVA: 0x00054778 File Offset: 0x00052978
			// (set) Token: 0x06001A38 RID: 6712 RVA: 0x00054780 File Offset: 0x00052980
			public string PlayerName { get; private set; }

			// Token: 0x170007DB RID: 2011
			// (get) Token: 0x06001A39 RID: 6713 RVA: 0x00054789 File Offset: 0x00052989
			// (set) Token: 0x06001A3A RID: 6714 RVA: 0x00054791 File Offset: 0x00052991
			public PlayerId SuggestingPlayerId { get; private set; }

			// Token: 0x170007DC RID: 2012
			// (get) Token: 0x06001A3B RID: 6715 RVA: 0x0005479A File Offset: 0x0005299A
			// (set) Token: 0x06001A3C RID: 6716 RVA: 0x000547A2 File Offset: 0x000529A2
			public string SuggestingPlayerName { get; private set; }

			// Token: 0x06001A3D RID: 6717 RVA: 0x000547AB File Offset: 0x000529AB
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
