using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.AccessProvider.Test;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using TaleWorlds.MountAndBlade.Multiplayer;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public class LobbyState : GameState
	{
		private bool AutoConnect
		{
			get
			{
				return TestCommonBase.BaseInstance == null || !TestCommonBase.BaseInstance.IsTestEnabled;
			}
		}

		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public override bool IsMusicMenuState
		{
			get
			{
				return false;
			}
		}

		public bool IsLoggingIn { get; private set; }

		public ILobbyStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		public LobbyClient LobbyClient
		{
			get
			{
				return NetworkMain.GameClient;
			}
		}

		public NewsManager NewsManager { get; private set; }

		public bool? HasMultiplayerPrivilege { get; private set; }

		public bool? HasCrossplayPrivilege { get; private set; }

		public bool? HasUserGeneratedContentPrivilege { get; private set; }

		public event Action<GameServerEntry> ClientRefusedToJoinCustomServer;

		public void InitializeLogic(ILobbyStateHandler lobbyStateHandler)
		{
			this.Handler = lobbyStateHandler;
		}

		protected override async void OnInitialize()
		{
			base.OnInitialize();
			CommunityClient communityClient = NetworkMain.CommunityClient;
			CommunityClientOnlineLobbyGameHandler communityClientOnlineLobbyGameHandler = new CommunityClientOnlineLobbyGameHandler(this);
			communityClient.Handler = communityClientOnlineLobbyGameHandler;
			this.LobbyClient.SetLoadedModules(Utilities.GetModulesNames());
			PlatformServices.Instance.OnSignInStateUpdated += this.OnPlatformSignInStateUpdated;
			PlatformServices.Instance.OnNameUpdated += this.OnPlayerNameUpdated;
			foreach (IFriendListService friendListService in PlatformServices.Instance.GetFriendListServices())
			{
				Type type = friendListService.GetType();
				if (type == typeof(BannerlordFriendListService))
				{
					this._bannerlordFriendListService = (BannerlordFriendListService)friendListService;
				}
				else if (type == typeof(RecentPlayersFriendListService))
				{
					this._recentPlayersFriendListService = (RecentPlayersFriendListService)friendListService;
				}
				else if (type == typeof(ClanFriendListService))
				{
					this._clanFriendListService = (ClanFriendListService)friendListService;
				}
			}
			this.NewsManager = new NewsManager();
			this.NewsManager.SetNewsSourceURL(this.GetApplicableNewsSourceURL());
			RecentPlayersManager.Initialize();
			this._onCustomServerActionRequestedForServerEntry = new List<Func<GameServerEntry, List<CustomServerAction>>>();
			this._lobbyGameClientManager = new LobbyGameClientHandler();
			this._lobbyGameClientManager.LobbyState = this;
			this.NewsManager.UpdateNewsItems(false);
			if (this.HasMultiplayerPrivilege.GetValueOrDefault() && this.AutoConnect)
			{
				await this.TryLogin();
			}
			else
			{
				this.SetConnectionState(false);
				this.OnResume();
			}
			if (PlatformServices.SessionInvitationType != null)
			{
				this.OnSessionInvitationAccepted(PlatformServices.SessionInvitationType);
			}
			else if (PlatformServices.IsPlatformRequestedMultiplayer)
			{
				this.OnPlatformRequestedMultiplayer();
			}
			PlatformServices.OnSessionInvitationAccepted = (Action<SessionInvitationType>)Delegate.Combine(PlatformServices.OnSessionInvitationAccepted, new Action<SessionInvitationType>(this.OnSessionInvitationAccepted));
			PlatformServices.OnPlatformRequestedMultiplayer = (Action)Delegate.Combine(PlatformServices.OnPlatformRequestedMultiplayer, new Action(this.OnPlatformRequestedMultiplayer));
		}

		private void OnPlayerNameUpdated(string newName)
		{
			this.LobbyClient.OnPlayerNameUpdated(newName);
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerNameUpdated(newName);
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			PlatformServices.OnPlatformRequestedMultiplayer = (Action)Delegate.Remove(PlatformServices.OnPlatformRequestedMultiplayer, new Action(this.OnPlatformRequestedMultiplayer));
			PlatformServices.OnSessionInvitationAccepted = (Action<SessionInvitationType>)Delegate.Remove(PlatformServices.OnSessionInvitationAccepted, new Action<SessionInvitationType>(this.OnSessionInvitationAccepted));
			PlatformServices.Instance.OnSignInStateUpdated -= this.OnPlatformSignInStateUpdated;
			PlatformServices.Instance.OnNameUpdated -= this.OnPlayerNameUpdated;
			RecentPlayersManager.Serialize();
			this.NewsManager.OnFinalize();
			this.NewsManager = null;
			this._onCustomServerActionRequestedForServerEntry.Clear();
			this._onCustomServerActionRequestedForServerEntry = null;
			foreach (ValueTuple<PlayerId, Permission> valueTuple in this._registeredPermissionEvents.Keys)
			{
				if (PlatformServices.Instance.UnregisterPermissionChangeEvent(valueTuple.Item1, valueTuple.Item2, new PermissionChanged(this.MultiplayerPermissionWithPlayerChanged)))
				{
					bool flag;
					this._registeredPermissionEvents.TryRemove(new ValueTuple<PlayerId, Permission>(valueTuple.Item1, valueTuple.Item2), out flag);
				}
			}
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
		}

		private string GetApplicableNewsSourceURL()
		{
			bool flag = this.NewsManager.LocalizationID == "zh";
			bool isInPreviewMode = this.NewsManager.IsInPreviewMode;
			string text = (flag ? "zh" : "en");
			if (!isInPreviewMode)
			{
				return "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_" + text + ".json";
			}
			return "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_" + text + "_preview.json";
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		private void CheckValidityOfItems()
		{
			foreach (ItemObject itemObject in MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
			{
				if (itemObject.IsUsingTeamColor)
				{
					MetaMesh copy = MetaMesh.GetCopy(itemObject.MultiMeshName, false, false);
					for (int i = 0; i < copy.MeshCount; i++)
					{
						Material material = copy.GetMeshAtIndex(i).GetMaterial();
						if (material.Name != "vertex_color_lighting_skinned" && material.Name != "vertex_color_lighting" && material.GetTexture(1) == null)
						{
							MBDebug.ShowWarning("Item object(" + itemObject.Name + ") has 'Using Team Color' flag but does not have a mask texture in diffuse2 slot. ");
							break;
						}
					}
				}
			}
		}

		public async Task UpdateHasMultiplayerPrivilege()
		{
			TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
			PlatformServices.Instance.CheckPrivilege(0, true, delegate(bool result)
			{
				tsc.SetResult(result);
			});
			bool flag = await tsc.Task;
			this.HasMultiplayerPrivilege = new bool?(flag);
			Action<bool> onMultiplayerPrivilegeUpdated = this.OnMultiplayerPrivilegeUpdated;
			if (onMultiplayerPrivilegeUpdated != null)
			{
				onMultiplayerPrivilegeUpdated(this.HasMultiplayerPrivilege.Value);
			}
		}

		public async Task UpdateHasCrossplayPrivilege()
		{
			TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
			PlatformServices.Instance.CheckPrivilege(2, false, delegate(bool result)
			{
				tsc.SetResult(result);
			});
			bool flag = await tsc.Task;
			this.HasCrossplayPrivilege = new bool?(flag);
			Action<bool> onCrossplayPrivilegeUpdated = this.OnCrossplayPrivilegeUpdated;
			if (onCrossplayPrivilegeUpdated != null)
			{
				onCrossplayPrivilegeUpdated(this.HasCrossplayPrivilege.Value);
			}
		}

		public void OnClientRefusedToJoinCustomServer(GameServerEntry serverEntry)
		{
			Action<GameServerEntry> clientRefusedToJoinCustomServer = this.ClientRefusedToJoinCustomServer;
			if (clientRefusedToJoinCustomServer == null)
			{
				return;
			}
			clientRefusedToJoinCustomServer(serverEntry);
		}

		public async Task UpdateHasUserGeneratedContentPrivilege(bool showResolveUI)
		{
			TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
			PlatformServices.Instance.CheckPrivilege(5, showResolveUI, delegate(bool result)
			{
				tsc.SetResult(result);
			});
			bool flag = await tsc.Task;
			this.HasUserGeneratedContentPrivilege = new bool?(flag);
			Action<bool> onUserGeneratedContentPrivilegeUpdated = this.OnUserGeneratedContentPrivilegeUpdated;
			if (onUserGeneratedContentPrivilegeUpdated != null)
			{
				onUserGeneratedContentPrivilegeUpdated(this.HasUserGeneratedContentPrivilege.Value);
			}
		}

		public async Task TryLogin()
		{
			this.IsLoggingIn = true;
			LobbyClient gameClient = this.LobbyClient;
			if (gameClient.IsIdle)
			{
				TaskAwaiter<bool> taskAwaiter = gameClient.CanLogin().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					this.ShowFeedback(new TextObject("{=lVfmVHbz}Login Failed", null).ToString(), new TextObject("{=pgw7LMRo}Server over capacity.", null).ToString());
					this.IsLoggingIn = false;
					return;
				}
				await this.UpdateHasMultiplayerPrivilege();
				if (!this.HasMultiplayerPrivilege.Value)
				{
					this.ShowFeedback(new TextObject("{=lVfmVHbz}Login Failed", null).ToString(), new TextObject("{=cS0Hafjl}Player does not have access to multiplayer.", null).ToString());
					this.IsLoggingIn = false;
					return;
				}
				await this.UpdateHasCrossplayPrivilege();
				await this.UpdateHasUserGeneratedContentPrivilege(false);
				ILoginAccessProvider loginAccessProvider = await PlatformServices.Instance.CreateLobbyClientLoginProvider();
				string userName = loginAccessProvider.GetUserName();
				LobbyClient lobbyClient = gameClient;
				ILobbyClientSessionHandler lobbyGameClientManager = this._lobbyGameClientManager;
				ILoginAccessProvider loginAccessProvider2 = loginAccessProvider;
				string text = userName;
				bool? hasUserGeneratedContentPrivilege = this.HasUserGeneratedContentPrivilege;
				bool flag = true;
				LobbyClientConnectResult lobbyClientConnectResult = await lobbyClient.Connect(lobbyGameClientManager, loginAccessProvider2, text, (hasUserGeneratedContentPrivilege.GetValueOrDefault() == flag) & (hasUserGeneratedContentPrivilege != null), PlatformServices.Instance.GetInitParams());
				if (lobbyClientConnectResult.Connected)
				{
					if (PlatformServices.InvitationServices != null)
					{
						await PlatformServices.InvitationServices.OnLogin();
					}
					Game.Current.GetGameHandler<ChatBox>().OnLogin();
					this.OnResume();
				}
				else
				{
					this.ShowFeedback(new TextObject("{=lVfmVHbz}Login Failed", null).ToString(), lobbyClientConnectResult.Error.ToString());
					this.SetConnectionState(false);
					this.OnResume();
				}
			}
			this.IsLoggingIn = false;
		}

		public async Task TryLogin(string userName, string password)
		{
			this.IsLoggingIn = true;
			LobbyClientConnectResult lobbyClientConnectResult = await NetworkMain.GameClient.Connect(this._lobbyGameClientManager, new TestLoginAccessProvider(), userName, true, PlatformServices.Instance.GetInitParams());
			if (!lobbyClientConnectResult.Connected)
			{
				this.ShowFeedback(new TextObject("{=lVfmVHbz}Login Failed", null).ToString(), lobbyClientConnectResult.Error.ToString());
			}
			this.IsLoggingIn = false;
		}

		public void HostGame()
		{
			if (string.IsNullOrEmpty(MultiplayerOptionsExtensions.GetStrValue(0, 0)))
			{
				MultiplayerOptionsExtensions.SetValue(0, NetworkMain.GameClient.Name, 0);
			}
			string strValue = MultiplayerOptionsExtensions.GetStrValue(2, 0);
			string strValue2 = MultiplayerOptionsExtensions.GetStrValue(3, 0);
			string text = ((!string.IsNullOrEmpty(strValue)) ? Common.CalculateMD5Hash(strValue) : null);
			string text2 = ((!string.IsNullOrEmpty(strValue2)) ? Common.CalculateMD5Hash(strValue2) : null);
			MultiplayerOptionsExtensions.SetValue(2, text, 0);
			MultiplayerOptionsExtensions.SetValue(3, text2, 0);
			string strValue3 = MultiplayerOptionsExtensions.GetStrValue(11, 0);
			string gameModule = MultiplayerGameTypes.GetGameTypeInfo(strValue3).GameModule;
			string strValue4 = MultiplayerOptionsExtensions.GetStrValue(13, 0);
			string text3 = null;
			UniqueSceneId uniqueSceneId;
			if (Utilities.TryGetUniqueIdentifiersForScene(strValue4, ref uniqueSceneId))
			{
				text3 = uniqueSceneId.Serialize();
			}
			NetworkMain.GameClient.RegisterCustomGame(gameModule, strValue3, MultiplayerOptionsExtensions.GetStrValue(0, 0), MultiplayerOptionsExtensions.GetIntValue(16, 0), strValue4, text3, MultiplayerOptionsExtensions.GetStrValue(2, 0), MultiplayerOptionsExtensions.GetStrValue(3, 0), 9999);
			MultiplayerOptions.Instance.InitializeAllOptionsFromCurrent();
		}

		public void CreatePremadeGame()
		{
			string strValue = MultiplayerOptionsExtensions.GetStrValue(0, 0);
			string strValue2 = MultiplayerOptionsExtensions.GetStrValue(10, 0);
			string strValue3 = MultiplayerOptionsExtensions.GetStrValue(13, 0);
			string strValue4 = MultiplayerOptionsExtensions.GetStrValue(14, 0);
			string strValue5 = MultiplayerOptionsExtensions.GetStrValue(15, 0);
			string strValue6 = MultiplayerOptionsExtensions.GetStrValue(2, 0);
			PremadeGameType premadeGameType = (PremadeGameType)Enum.GetValues(typeof(PremadeGameType)).GetValue(MultiplayerOptionsExtensions.GetIntValue(12, 0));
			if (premadeGameType == 1)
			{
				bool flag = true;
				using (List<PartyPlayerInLobbyClient>.Enumerator enumerator = NetworkMain.GameClient.PlayersInParty.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PartyPlayerInLobbyClient partyPlayer = enumerator.Current;
						if (NetworkMain.GameClient.PlayersInClan.FirstOrDefault((ClanPlayer clanPlayer) => clanPlayer.PlayerId == partyPlayer.PlayerId) == null)
						{
							flag = false;
						}
					}
				}
				if (!flag)
				{
					this.ShowFeedback(new TextObject("{=oZrVNUOk}Error", null).ToString(), new TextObject("{=uNrXwGzr}Only practice matches are allowed with your current party. All members should be in the same clan for a clan match.", null).ToString());
					return;
				}
			}
			if (strValue != null && !Extensions.IsEmpty<char>(strValue) && premadeGameType != 2)
			{
				NetworkMain.GameClient.CreatePremadeGame(strValue, strValue2, strValue3, strValue4, strValue5, strValue6, premadeGameType);
				return;
			}
			if (premadeGameType == 2)
			{
				this.ShowFeedback(new TextObject("{=oZrVNUOk}Error", null).ToString(), new TextObject("{=PfnS8HUd}Premade game type is invalid!", null).ToString());
				return;
			}
			this.ShowFeedback(new TextObject("{=oZrVNUOk}Error", null).ToString(), new TextObject("{=EgTUzWUz}Name Can't Be Empty!", null).ToString());
		}

		public string ShowFeedback(string title, string message)
		{
			if (this.Handler != null)
			{
				return this.Handler.ShowFeedback(title, message);
			}
			return null;
		}

		public string ShowFeedback(InquiryData inquiryData)
		{
			if (this.Handler != null)
			{
				return this.Handler.ShowFeedback(inquiryData);
			}
			return null;
		}

		public void DismissFeedback(string messageId)
		{
			if (this.Handler != null)
			{
				this.Handler.DismissFeedback(messageId);
			}
		}

		public void OnPause()
		{
			if (this.Handler != null)
			{
				this.Handler.OnPause();
			}
		}

		public void OnResume()
		{
			if (this.Handler != null)
			{
				this.Handler.OnResume();
			}
		}

		public void OnRequestedToSearchBattle()
		{
			if (this.Handler != null)
			{
				this.Handler.OnRequestedToSearchBattle();
			}
		}

		public void OnUpdateFindingGame(MatchmakingWaitTimeStats matchmakingWaitTimeStats, string[] gameTypeInfo = null)
		{
			if (this.Handler != null)
			{
				this.Handler.OnUpdateFindingGame(matchmakingWaitTimeStats, gameTypeInfo);
			}
		}

		public void OnRequestedToCancelSearchBattle()
		{
			if (this.Handler != null)
			{
				this.Handler.OnRequestedToCancelSearchBattle();
			}
		}

		public void OnCancelFindingGame()
		{
			if (this.Handler != null)
			{
				this.Handler.OnSearchBattleCanceled();
			}
		}

		public void OnDisconnected(TextObject feedback)
		{
			if (this.Handler != null)
			{
				this.Handler.OnDisconnected();
			}
			if (feedback != null)
			{
				string text = new TextObject("{=MbXatV1Q}Disconnected", null).ToString();
				this.ShowFeedback(text, feedback.ToString());
			}
		}

		public void OnPlayerDataReceived(PlayerData playerData)
		{
			if (this.Handler != null)
			{
				this.Handler.OnPlayerDataReceived(playerData);
			}
		}

		public void OnPendingRejoin()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPendingRejoin();
		}

		public void OnEnterBattleWithParty(string[] selectedGameTypes)
		{
			if (this.Handler != null)
			{
				this.Handler.OnEnterBattleWithParty(selectedGameTypes);
			}
		}

		public async void OnPartyInvitationReceived(string inviterPlayerName, PlayerId playerId)
		{
			while (this.IsLoggingIn)
			{
				Debug.Print("Waiting for logging in to be done..", 0, 12, 17592186044416UL);
				await Task.Delay(100);
			}
			if (PermaMuteList.IsPlayerMuted(playerId))
			{
				this.LobbyClient.DeclinePartyInvitation();
			}
			else if (this.Handler != null)
			{
				PermissionResult <>9__1;
				PlatformServices.Instance.CheckPrivilege(3, true, delegate(bool privilegeResult)
				{
					if (!privilegeResult)
					{
						this.LobbyClient.DeclinePartyInvitation();
						return;
					}
					if (playerId.ProvidedType == NetworkMain.GameClient.PlayerID.ProvidedType)
					{
						IPlatformServices instance = PlatformServices.Instance;
						Permission permission = 1;
						PlayerId playerId2 = playerId;
						PermissionResult permissionResult2;
						if ((permissionResult2 = <>9__1) == null)
						{
							permissionResult2 = (<>9__1 = delegate(bool permissionResult)
							{
								if (!permissionResult)
								{
									this.LobbyClient.DeclinePartyInvitation();
									return;
								}
								ILobbyStateHandler handler2 = this.Handler;
								if (handler2 == null)
								{
									return;
								}
								handler2.OnPartyInvitationReceived(playerId);
							});
						}
						instance.CheckPermissionWithUser(permission, playerId2, permissionResult2);
						return;
					}
					ILobbyStateHandler handler = this.Handler;
					if (handler == null)
					{
						return;
					}
					handler.OnPartyInvitationReceived(playerId);
				});
			}
		}

		public async void OnPartyJoinRequestReceived(PlayerId joiningPlayerId, PlayerId viaPlayerId, string viaFriendName)
		{
			while (this.IsLoggingIn)
			{
				Debug.Print("Waiting for logging in to be done..", 0, 12, 17592186044416UL);
				await Task.Delay(100);
			}
			if (PermaMuteList.IsPlayerMuted(joiningPlayerId))
			{
				this.LobbyClient.DeclinePartyJoinRequest(joiningPlayerId, 1);
			}
			else if (this.Handler != null)
			{
				if (joiningPlayerId.ProvidedType != NetworkMain.GameClient.PlayerID.ProvidedType)
				{
					ILobbyStateHandler handler = this.Handler;
					if (handler != null)
					{
						handler.OnPartyJoinRequestReceived(joiningPlayerId, viaPlayerId, viaFriendName, !this.LobbyClient.IsInParty);
					}
				}
				else
				{
					PlatformServices.Instance.CheckPermissionWithUser(1, joiningPlayerId, delegate(bool permissionResult)
					{
						if (!permissionResult)
						{
							this.LobbyClient.DeclinePartyJoinRequest(joiningPlayerId, 1);
							return;
						}
						ILobbyStateHandler handler2 = this.Handler;
						if (handler2 == null)
						{
							return;
						}
						handler2.OnPartyJoinRequestReceived(joiningPlayerId, viaPlayerId, viaFriendName, !this.LobbyClient.IsInParty);
					});
				}
			}
		}

		public void OnAdminMessageReceived(string message)
		{
			if (this.Handler != null)
			{
				this.Handler.OnAdminMessageReceived(message);
			}
		}

		public void OnPartyInvitationInvalidated()
		{
			if (this.Handler != null)
			{
				this.Handler.OnPartyInvitationInvalidated();
			}
		}

		public void OnPlayerInvitedToParty(PlayerId playerId)
		{
			if (this.Handler != null)
			{
				this.Handler.OnPlayerInvitedToParty(playerId);
			}
		}

		public void OnPlayerRemovedFromParty(PlayerId playerId, PartyRemoveReason reason)
		{
			if (playerId.Equals(this.LobbyClient.PlayerID))
			{
				IPlatformInvitationServices invitationServices = PlatformServices.InvitationServices;
				if (invitationServices != null)
				{
					invitationServices.OnLeftParty();
				}
			}
			if (PlatformServices.Instance.UnregisterPermissionChangeEvent(playerId, 0, new PermissionChanged(this.MultiplayerPermissionWithPlayerChanged)))
			{
				bool flag;
				this._registeredPermissionEvents.TryRemove(new ValueTuple<PlayerId, Permission>(playerId, 0), out flag);
			}
			if (this.Handler != null)
			{
				this.Handler.OnPlayerRemovedFromParty(playerId, reason);
			}
		}

		public void OnPlayersAddedToParty([TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })] List<ValueTuple<PlayerId, string, bool>> addedPlayers, [TupleElementNames(new string[] { "PlayerId", "PlayerName" })] List<ValueTuple<PlayerId, string>> invitedPlayers)
		{
			using (List<ValueTuple<PlayerId, string, bool>>.Enumerator enumerator = addedPlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ValueTuple<PlayerId, string, bool> player = enumerator.Current;
					PlayerId item = player.Item1;
					if (item.ProvidedType != this.LobbyClient.PlayerID.ProvidedType)
					{
						ILobbyStateHandler handler = this.Handler;
						if (handler != null)
						{
							handler.OnPlayerAddedToParty(player.Item1, player.Item2, player.Item3);
						}
					}
					else
					{
						PlatformServices.Instance.CheckPermissionWithUser(0, player.Item1, delegate(bool hasPermission)
						{
							if (!hasPermission)
							{
								NetworkMain.GameClient.KickPlayerFromParty(NetworkMain.GameClient.PlayerID);
								return;
							}
							if (PlatformServices.Instance.RegisterPermissionChangeEvent(player.Item1, 0, new PermissionChanged(this.MultiplayerPermissionWithPlayerChanged)))
							{
								bool flag;
								this._registeredPermissionEvents.TryRemove(new ValueTuple<PlayerId, Permission>(player.Item1, 0), out flag);
							}
							ILobbyStateHandler handler3 = this.Handler;
							if (handler3 == null)
							{
								return;
							}
							handler3.OnPlayerAddedToParty(player.Item1, player.Item2, player.Item3);
						});
					}
				}
			}
			if (this.Handler != null)
			{
				foreach (ValueTuple<PlayerId, string> valueTuple in invitedPlayers)
				{
					PlayerId playerId = valueTuple.Item1;
					if (playerId.ProvidedType != this.LobbyClient.PlayerID.ProvidedType)
					{
						ILobbyStateHandler handler2 = this.Handler;
						if (handler2 != null)
						{
							handler2.OnPlayerInvitedToParty(playerId);
						}
					}
					else
					{
						PlatformServices.Instance.CheckPermissionWithUser(0, playerId, delegate(bool hasPermission)
						{
							if (hasPermission)
							{
								ILobbyStateHandler handler4 = this.Handler;
								if (handler4 == null)
								{
									return;
								}
								handler4.OnPlayerInvitedToParty(playerId);
							}
						});
					}
				}
			}
		}

		private void MultiplayerPermissionWithPlayerChanged(PlayerId targetPlayerId, Permission permission, bool hasPermission)
		{
			if (!hasPermission && NetworkMain.GameClient.PlayersInParty.FirstOrDefault((PartyPlayerInLobbyClient p) => p.PlayerId == targetPlayerId) != null)
			{
				NetworkMain.GameClient.KickPlayerFromParty(NetworkMain.GameClient.PlayerID);
			}
		}

		public void OnGameClientStateChange(LobbyClient.State state)
		{
			if (!this.LobbyClient.IsInGame)
			{
				PlatformServices.MultiplayerGameStateChanged(false);
			}
			ILobbyStateHandler handler = this.Handler;
			if (handler != null)
			{
				handler.OnGameClientStateChange(state);
			}
			if (state == 3)
			{
				MPPerkSelectionManager.Instance.InitializeForUser(this.LobbyClient.Name, this.LobbyClient.PlayerID);
			}
			else if (state == null)
			{
				MPPerkSelectionManager.FreeInstance();
			}
			else if (!this.LobbyClient.AtLobby)
			{
				MPPerkSelectionManager.Instance.ResetPendingChanges();
			}
			PlatformServices.LobbyClientStateChanged(state == 4, !this.LobbyClient.IsInParty || this.LobbyClient.IsPartyLeader);
		}

		public void SetConnectionState(bool isAuthenticated)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler != null)
			{
				handler.SetConnectionState(isAuthenticated);
			}
			PlatformServices.ConnectionStateChanged(isAuthenticated);
		}

		public void OnActivateHome()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateHome();
		}

		public void OnActivateCustomServer()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateCustomServer();
		}

		public void OnActivateMatchmaking()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateMatchmaking();
		}

		public void OnActivateProfile()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateProfile();
		}

		public void OnClanInvitationReceived(string clanName, string clanTag, bool isCreation)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanInvitationReceived(clanName, clanTag, isCreation);
		}

		public void OnClanInvitationAnswered(PlayerId playerId, ClanCreationAnswer answer)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanInvitationAnswered(playerId, answer);
		}

		public void OnClanCreationSuccessful()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanCreationSuccessful();
		}

		public void OnClanCreationFailed()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanCreationFailed();
		}

		public void OnClanCreationStarted()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanCreationStarted();
		}

		public void OnClanInfoChanged()
		{
			ClanFriendListService clanFriendListService = this._clanFriendListService;
			if (clanFriendListService != null)
			{
				clanFriendListService.OnClanInfoChanged(this.LobbyClient.PlayerInfosInClan);
			}
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanInfoChanged();
		}

		public void OnPremadeGameEligibilityStatusReceived(bool isEligible)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPremadeGameEligibilityStatusReceived(isEligible);
		}

		public void OnPremadeGameCreated()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPremadeGameCreated();
		}

		public void OnPremadeGameListReceived()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPremadeGameListReceived();
		}

		public void OnPremadeGameCreationCancelled()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPremadeGameCreationCancelled();
		}

		public void OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId, PlayerId[] challengerPlayerIDs, PlayerId challengerPartyLeaderID, PremadeGameType premadeGameType)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnJoinPremadeGameRequested(clanName, clanSigilCode, partyId, challengerPlayerIDs, challengerPartyLeaderID, premadeGameType);
		}

		public void OnJoinPremadeGameRequestSuccessful()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnJoinPremadeGameRequestSuccessful();
		}

		public void OnActivateArmory()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateArmory();
		}

		public void OnActivateOptions()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateOptions();
		}

		public void OnDeactivateOptions()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnDeactivateOptions();
		}

		public void OnCustomGameServerListReceived(AvailableCustomGames customGameServerList)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnCustomGameServerListReceived(customGameServerList);
		}

		public void OnMatchmakerGameOver(int oldExp, int newExp, List<string> badgesEarned, int lootGained, RankBarInfo oldRankBarInfo, RankBarInfo newRankBarInfo)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnMatchmakerGameOver(oldExp, newExp, badgesEarned, lootGained, oldRankBarInfo, newRankBarInfo);
		}

		public void OnBattleServerLost()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnBattleServerLost();
		}

		public void OnRemovedFromMatchmakerGame(DisconnectType disconnectType)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnRemovedFromMatchmakerGame(disconnectType);
		}

		public void OnRemovedFromCustomGame(DisconnectType disconnectType)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnRemovedFromCustomGame(disconnectType);
		}

		public void OnPlayerAssignedPartyLeader(PlayerId partyLeaderId)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerAssignedPartyLeader(partyLeaderId);
		}

		public void OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerSuggestedToParty(playerId, playerName, suggestingPlayerId, suggestingPlayerName);
		}

		public void OnJoinCustomGameFailureResponse(CustomGameJoinResponse response)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnJoinCustomGameFailureResponse(response);
		}

		public void OnServerStatusReceived(ServerStatus serverStatus)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnServerStatusReceived(serverStatus);
		}

		public void OnFriendListReceived(FriendInfo[] friends)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler != null)
			{
				handler.OnFriendListUpdated();
			}
			BannerlordFriendListService bannerlordFriendListService = this._bannerlordFriendListService;
			if (bannerlordFriendListService == null)
			{
				return;
			}
			bannerlordFriendListService.OnFriendListReceived(friends);
		}

		public void OnRecentPlayerStatusesReceived(FriendInfo[] friends)
		{
			RecentPlayersFriendListService recentPlayersFriendListService = this._recentPlayersFriendListService;
			if (recentPlayersFriendListService == null)
			{
				return;
			}
			recentPlayersFriendListService.OnFriendListReceived(friends);
		}

		public void OnBattleServerInformationReceived(BattleServerInformationForClient battleServerInformation)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnBattleServerInformationReceived(battleServerInformation);
		}

		public void OnRejoinBattleRequestAnswered(bool isSuccessful)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnRejoinBattleRequestAnswered(isSuccessful);
		}

		internal void OnSigilChanged()
		{
			if (this.Handler != null)
			{
				this.Handler.OnSigilChanged();
			}
		}

		public void OnNotificationsReceived(LobbyNotification[] notifications)
		{
			if (this.Handler != null)
			{
				this.Handler.OnNotificationsReceived(notifications);
			}
		}

		private void OnPlatformSignInStateUpdated(bool isSignedIn, TextObject message)
		{
			if (!isSignedIn && this.LobbyClient.Connected)
			{
				this.LobbyClient.Logout(message ?? new TextObject("{=oPOa77dI}Logged out of platform", null));
			}
		}

		[Conditional("DEBUG")]
		private void PrintCompressionInfoKey()
		{
			try
			{
				List<Type> list = new List<Type>();
				Assembly[] array = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
					where assembly.GetName().Name.StartsWith("TaleWorlds.")
					select assembly).ToArray<Assembly>();
				Assembly[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					Type type = Extensions.GetTypesSafe(array2[i], null).FirstOrDefault((Type ty) => ty.Name.Contains("CompressionInfo"));
					if (type != null)
					{
						list.AddRange(type.GetNestedTypes());
						break;
					}
				}
				List<FieldInfo> list2 = new List<FieldInfo>();
				array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					foreach (Type type2 in Extensions.GetTypesSafe(array2[i], null))
					{
						foreach (FieldInfo fieldInfo in type2.GetFields())
						{
							if (list.Contains(fieldInfo.FieldType))
							{
								list2.Add(fieldInfo);
							}
						}
					}
				}
				int num = 0;
				foreach (FieldInfo fieldInfo2 in list2)
				{
					object value = fieldInfo2.GetValue(null);
					MethodInfo method = fieldInfo2.FieldType.GetMethod("GetHashKey", BindingFlags.Instance | BindingFlags.NonPublic);
					num += (int)method.Invoke(value, new object[0]);
				}
				Debug.Print("CompressionInfoKey: " + num, 0, 7, 17179869184UL);
			}
			catch
			{
				Debug.Print("CompressionInfoKey checking failed.", 0, 7, 17179869184UL);
			}
		}

		public async Task<bool> OnInviteToPlatformSession(PlayerId playerId)
		{
			bool flag;
			if (!this.LobbyClient.Connected)
			{
				flag = false;
			}
			else
			{
				bool flag2 = false;
				if ((!this.LobbyClient.IsInParty || this.LobbyClient.IsPartyLeader) && this.LobbyClient.PlayersInParty.Count < Parameters.MaxPlayerCountInParty && PlatformServices.InvitationServices != null)
				{
					flag2 = await PlatformServices.InvitationServices.OnInviteToPlatformSession(playerId);
				}
				if (!flag2)
				{
					InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=ljHPjjmX}Could not invite player to the game", null).ToString()));
				}
				flag = flag2;
			}
			return flag;
		}

		public async void OnPlatformRequestedMultiplayer()
		{
			PlatformServices.OnPlatformMultiplayerRequestHandled();
			await this.UpdateHasMultiplayerPrivilege();
			if (this.HasMultiplayerPrivilege != null && this.HasMultiplayerPrivilege.Value)
			{
				if (this.LobbyClient.IsIdle)
				{
					await this.TryLogin();
					int waitTime = 0;
					while (this.LobbyClient.CurrentState != null && this.LobbyClient.CurrentState != 4 && waitTime < 3000)
					{
						await Task.Delay(100);
						waitTime += 100;
					}
				}
			}
		}

		public async void OnSessionInvitationAccepted(SessionInvitationType targetGameType)
		{
			if (targetGameType == 1)
			{
				PlatformServices.OnSessionInvitationHandled();
				await this.UpdateHasMultiplayerPrivilege();
				if (this.HasMultiplayerPrivilege != null && this.HasMultiplayerPrivilege.Value)
				{
					await Task.Delay(2000);
					if (this.LobbyClient.IsIdle)
					{
						await this.TryLogin();
						int waitTime = 0;
						while (this.LobbyClient.CurrentState != null && this.LobbyClient.CurrentState != 4 && waitTime < 3000)
						{
							await Task.Delay(100);
							waitTime += 100;
						}
					}
					if (this.LobbyClient.CurrentState == 4)
					{
						if (PlatformServices.InvitationServices != null)
						{
							Tuple<bool, ulong> tuple = await PlatformServices.InvitationServices.JoinSession();
							if (tuple.Item1)
							{
								TaskAwaiter<bool> taskAwaiter = this.LobbyClient.SendPSPlayerJoinedToPlayerSessionMessage(tuple.Item2).GetAwaiter();
								if (!taskAwaiter.IsCompleted)
								{
									await taskAwaiter;
									TaskAwaiter<bool> taskAwaiter2;
									taskAwaiter = taskAwaiter2;
									taskAwaiter2 = default(TaskAwaiter<bool>);
								}
								if (!taskAwaiter.GetResult())
								{
									await PlatformServices.InvitationServices.LeaveSession(true);
								}
							}
						}
					}
				}
			}
		}

		public List<CustomServerAction> GetCustomActionsForServer(GameServerEntry gameServerEntry)
		{
			List<CustomServerAction> list = new List<CustomServerAction>();
			for (int i = 0; i < this._onCustomServerActionRequestedForServerEntry.Count; i++)
			{
				List<CustomServerAction> list2 = this._onCustomServerActionRequestedForServerEntry[i](gameServerEntry);
				if (list2 != null && list2.Count > 0)
				{
					list.AddRange(list2);
				}
			}
			return list;
		}

		public void RegisterForCustomServerAction(Func<GameServerEntry, List<CustomServerAction>> action)
		{
			this._onCustomServerActionRequestedForServerEntry.Add(action);
		}

		public void UnregisterForCustomServerAction(Func<GameServerEntry, List<CustomServerAction>> action)
		{
			this._onCustomServerActionRequestedForServerEntry.Remove(action);
		}

		private const string _newsSourceURLBase = "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_";

		private BannerlordFriendListService _bannerlordFriendListService;

		private RecentPlayersFriendListService _recentPlayersFriendListService;

		private ClanFriendListService _clanFriendListService;

		private readonly object _sessionInvitationDataLock = new object();

		[TupleElementNames(new string[] { "PlayerId", "Permission" })]
		private ConcurrentDictionary<ValueTuple<PlayerId, Permission>, bool> _registeredPermissionEvents = new ConcurrentDictionary<ValueTuple<PlayerId, Permission>, bool>();

		private ILobbyStateHandler _handler;

		private LobbyGameClientHandler _lobbyGameClientManager;

		private List<Func<GameServerEntry, List<CustomServerAction>>> _onCustomServerActionRequestedForServerEntry;

		public Action<bool> OnMultiplayerPrivilegeUpdated;

		public Action<bool> OnCrossplayPrivilegeUpdated;

		public Action<bool> OnUserGeneratedContentPrivilegeUpdated;
	}
}
