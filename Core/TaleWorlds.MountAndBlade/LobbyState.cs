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
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200022D RID: 557
	public class LobbyState : GameState
	{
		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x06001E5D RID: 7773 RVA: 0x0006D105 File Offset: 0x0006B305
		private bool AutoConnect
		{
			get
			{
				return TestCommonBase.BaseInstance == null || !TestCommonBase.BaseInstance.IsTestEnabled;
			}
		}

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x06001E5E RID: 7774 RVA: 0x0006D11D File Offset: 0x0006B31D
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x06001E5F RID: 7775 RVA: 0x0006D120 File Offset: 0x0006B320
		public override bool IsMusicMenuState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x06001E60 RID: 7776 RVA: 0x0006D123 File Offset: 0x0006B323
		// (set) Token: 0x06001E61 RID: 7777 RVA: 0x0006D12B File Offset: 0x0006B32B
		public bool IsLoggingIn { get; private set; }

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x06001E62 RID: 7778 RVA: 0x0006D134 File Offset: 0x0006B334
		// (set) Token: 0x06001E63 RID: 7779 RVA: 0x0006D13C File Offset: 0x0006B33C
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

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x06001E64 RID: 7780 RVA: 0x0006D145 File Offset: 0x0006B345
		public LobbyClient LobbyClient
		{
			get
			{
				return NetworkMain.GameClient;
			}
		}

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x06001E65 RID: 7781 RVA: 0x0006D14C File Offset: 0x0006B34C
		// (set) Token: 0x06001E66 RID: 7782 RVA: 0x0006D154 File Offset: 0x0006B354
		public NewsManager NewsManager { get; private set; }

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x06001E67 RID: 7783 RVA: 0x0006D15D File Offset: 0x0006B35D
		// (set) Token: 0x06001E68 RID: 7784 RVA: 0x0006D165 File Offset: 0x0006B365
		public bool? HasMultiplayerPrivilege { get; private set; }

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x06001E69 RID: 7785 RVA: 0x0006D16E File Offset: 0x0006B36E
		// (set) Token: 0x06001E6A RID: 7786 RVA: 0x0006D176 File Offset: 0x0006B376
		public bool? HasCrossplayPrivilege { get; private set; }

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06001E6B RID: 7787 RVA: 0x0006D17F File Offset: 0x0006B37F
		// (set) Token: 0x06001E6C RID: 7788 RVA: 0x0006D187 File Offset: 0x0006B387
		public bool? HasUserGeneratedContentPrivilege { get; private set; }

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06001E6D RID: 7789 RVA: 0x0006D190 File Offset: 0x0006B390
		// (remove) Token: 0x06001E6E RID: 7790 RVA: 0x0006D1C8 File Offset: 0x0006B3C8
		public event Action<GameServerEntry> ClientRefusedToJoinCustomServer;

		// Token: 0x06001E6F RID: 7791 RVA: 0x0006D1FD File Offset: 0x0006B3FD
		public void InitializeLogic(ILobbyStateHandler lobbyStateHandler)
		{
			this.Handler = lobbyStateHandler;
		}

		// Token: 0x06001E70 RID: 7792 RVA: 0x0006D208 File Offset: 0x0006B408
		protected override async void OnInitialize()
		{
			base.OnInitialize();
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
			if (PlatformServices.SessionInvitationType != SessionInvitationType.None)
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

		// Token: 0x06001E71 RID: 7793 RVA: 0x0006D241 File Offset: 0x0006B441
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

		// Token: 0x06001E72 RID: 7794 RVA: 0x0006D260 File Offset: 0x0006B460
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

		// Token: 0x06001E73 RID: 7795 RVA: 0x0006D38C File Offset: 0x0006B58C
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
		}

		// Token: 0x06001E74 RID: 7796 RVA: 0x0006D398 File Offset: 0x0006B598
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

		// Token: 0x06001E75 RID: 7797 RVA: 0x0006D3FC File Offset: 0x0006B5FC
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
						if (material.Name != "vertex_color_lighting_skinned" && material.Name != "vertex_color_lighting" && material.GetTexture(Material.MBTextureType.DiffuseMap2) == null)
						{
							MBDebug.ShowWarning("Item object(" + itemObject.Name + ") has 'Using Team Color' flag but does not have a mask texture in diffuse2 slot. ");
							break;
						}
					}
				}
			}
		}

		// Token: 0x06001E76 RID: 7798 RVA: 0x0006D4DC File Offset: 0x0006B6DC
		public async Task UpdateHasMultiplayerPrivilege()
		{
			TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
			PlatformServices.Instance.CheckPrivilege(Privilege.Multiplayer, true, delegate(bool result)
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

		// Token: 0x06001E77 RID: 7799 RVA: 0x0006D524 File Offset: 0x0006B724
		public async Task UpdateHasCrossplayPrivilege()
		{
			TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
			PlatformServices.Instance.CheckPrivilege(Privilege.Crossplay, false, delegate(bool result)
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

		// Token: 0x06001E78 RID: 7800 RVA: 0x0006D569 File Offset: 0x0006B769
		public void OnClientRefusedToJoinCustomServer(GameServerEntry serverEntry)
		{
			Action<GameServerEntry> clientRefusedToJoinCustomServer = this.ClientRefusedToJoinCustomServer;
			if (clientRefusedToJoinCustomServer == null)
			{
				return;
			}
			clientRefusedToJoinCustomServer(serverEntry);
		}

		// Token: 0x06001E79 RID: 7801 RVA: 0x0006D57C File Offset: 0x0006B77C
		public async Task UpdateHasUserGeneratedContentPrivilege(bool showResolveUI)
		{
			TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
			PlatformServices.Instance.CheckPrivilege(Privilege.UserGeneratedContent, showResolveUI, delegate(bool result)
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

		// Token: 0x06001E7A RID: 7802 RVA: 0x0006D5CC File Offset: 0x0006B7CC
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

		// Token: 0x06001E7B RID: 7803 RVA: 0x0006D614 File Offset: 0x0006B814
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

		// Token: 0x06001E7C RID: 7804 RVA: 0x0006D664 File Offset: 0x0006B864
		public void HostGame()
		{
			if (string.IsNullOrEmpty(MultiplayerOptions.OptionType.ServerName.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)))
			{
				MultiplayerOptions.OptionType.ServerName.SetValue(NetworkMain.GameClient.Name, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			}
			string strValue = MultiplayerOptions.OptionType.GamePassword.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string strValue2 = MultiplayerOptions.OptionType.AdminPassword.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string text = ((!string.IsNullOrEmpty(strValue)) ? Common.CalculateMD5Hash(strValue) : null);
			string text2 = ((!string.IsNullOrEmpty(strValue2)) ? Common.CalculateMD5Hash(strValue2) : null);
			MultiplayerOptions.OptionType.GamePassword.SetValue(text, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			MultiplayerOptions.OptionType.AdminPassword.SetValue(text2, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string strValue3 = MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string gameModule = MultiplayerGameTypes.GetGameTypeInfo(strValue3).GameModule;
			string strValue4 = MultiplayerOptions.OptionType.Map.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string text3 = null;
			UniqueSceneId uniqueSceneId;
			if (Utilities.TryGetUniqueIdentifiersForScene(strValue4, out uniqueSceneId))
			{
				text3 = uniqueSceneId.Serialize();
			}
			NetworkMain.GameClient.RegisterCustomGame(gameModule, strValue3, MultiplayerOptions.OptionType.ServerName.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions), MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions), strValue4, text3, MultiplayerOptions.OptionType.GamePassword.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions), MultiplayerOptions.OptionType.AdminPassword.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions), 9999);
			MultiplayerOptions.Instance.InitializeAllOptionsFromCurrent();
		}

		// Token: 0x06001E7D RID: 7805 RVA: 0x0006D74C File Offset: 0x0006B94C
		public void CreatePremadeGame()
		{
			string strValue = MultiplayerOptions.OptionType.ServerName.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string strValue2 = MultiplayerOptions.OptionType.PremadeMatchGameMode.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string strValue3 = MultiplayerOptions.OptionType.Map.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string strValue4 = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string strValue5 = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string strValue6 = MultiplayerOptions.OptionType.GamePassword.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			PremadeGameType premadeGameType = (PremadeGameType)Enum.GetValues(typeof(PremadeGameType)).GetValue(MultiplayerOptions.OptionType.PremadeGameType.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			if (premadeGameType == PremadeGameType.Clan)
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
			if (strValue != null && !strValue.IsEmpty<char>() && premadeGameType != PremadeGameType.Invalid)
			{
				NetworkMain.GameClient.CreatePremadeGame(strValue, strValue2, strValue3, strValue4, strValue5, strValue6, premadeGameType);
				return;
			}
			if (premadeGameType == PremadeGameType.Invalid)
			{
				this.ShowFeedback(new TextObject("{=oZrVNUOk}Error", null).ToString(), new TextObject("{=PfnS8HUd}Premade game type is invalid!", null).ToString());
				return;
			}
			this.ShowFeedback(new TextObject("{=oZrVNUOk}Error", null).ToString(), new TextObject("{=EgTUzWUz}Name Can't Be Empty!", null).ToString());
		}

		// Token: 0x06001E7E RID: 7806 RVA: 0x0006D8D4 File Offset: 0x0006BAD4
		public string ShowFeedback(string title, string message)
		{
			if (this.Handler != null)
			{
				return this.Handler.ShowFeedback(title, message);
			}
			return null;
		}

		// Token: 0x06001E7F RID: 7807 RVA: 0x0006D8ED File Offset: 0x0006BAED
		public string ShowFeedback(InquiryData inquiryData)
		{
			if (this.Handler != null)
			{
				return this.Handler.ShowFeedback(inquiryData);
			}
			return null;
		}

		// Token: 0x06001E80 RID: 7808 RVA: 0x0006D905 File Offset: 0x0006BB05
		public void DismissFeedback(string messageId)
		{
			if (this.Handler != null)
			{
				this.Handler.DismissFeedback(messageId);
			}
		}

		// Token: 0x06001E81 RID: 7809 RVA: 0x0006D91B File Offset: 0x0006BB1B
		public void OnPause()
		{
			if (this.Handler != null)
			{
				this.Handler.OnPause();
			}
		}

		// Token: 0x06001E82 RID: 7810 RVA: 0x0006D930 File Offset: 0x0006BB30
		public void OnResume()
		{
			if (this.Handler != null)
			{
				this.Handler.OnResume();
			}
		}

		// Token: 0x06001E83 RID: 7811 RVA: 0x0006D945 File Offset: 0x0006BB45
		public void OnRequestedToSearchBattle()
		{
			if (this.Handler != null)
			{
				this.Handler.OnRequestedToSearchBattle();
			}
		}

		// Token: 0x06001E84 RID: 7812 RVA: 0x0006D95A File Offset: 0x0006BB5A
		public void OnUpdateFindingGame(MatchmakingWaitTimeStats matchmakingWaitTimeStats, string[] gameTypeInfo = null)
		{
			if (this.Handler != null)
			{
				this.Handler.OnUpdateFindingGame(matchmakingWaitTimeStats, gameTypeInfo);
			}
		}

		// Token: 0x06001E85 RID: 7813 RVA: 0x0006D971 File Offset: 0x0006BB71
		public void OnRequestedToCancelSearchBattle()
		{
			if (this.Handler != null)
			{
				this.Handler.OnRequestedToCancelSearchBattle();
			}
		}

		// Token: 0x06001E86 RID: 7814 RVA: 0x0006D986 File Offset: 0x0006BB86
		public void OnCancelFindingGame()
		{
			if (this.Handler != null)
			{
				this.Handler.OnSearchBattleCanceled();
			}
		}

		// Token: 0x06001E87 RID: 7815 RVA: 0x0006D99C File Offset: 0x0006BB9C
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

		// Token: 0x06001E88 RID: 7816 RVA: 0x0006D9DE File Offset: 0x0006BBDE
		public void OnPlayerDataReceived(PlayerData playerData)
		{
			if (this.Handler != null)
			{
				this.Handler.OnPlayerDataReceived(playerData);
			}
		}

		// Token: 0x06001E89 RID: 7817 RVA: 0x0006D9F4 File Offset: 0x0006BBF4
		public void OnPendingRejoin()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPendingRejoin();
		}

		// Token: 0x06001E8A RID: 7818 RVA: 0x0006DA06 File Offset: 0x0006BC06
		public void OnEnterBattleWithParty(string[] selectedGameTypes)
		{
			if (this.Handler != null)
			{
				this.Handler.OnEnterBattleWithParty(selectedGameTypes);
			}
		}

		// Token: 0x06001E8B RID: 7819 RVA: 0x0006DA1C File Offset: 0x0006BC1C
		public async void OnPartyInvitationReceived(string inviterPlayerName, PlayerId playerId)
		{
			while (this.IsLoggingIn)
			{
				Debug.Print("Waiting for logging in to be done..", 0, Debug.DebugColor.White, 17592186044416UL);
				await Task.Delay(100);
			}
			if (PermaMuteList.IsPlayerMuted(playerId))
			{
				this.LobbyClient.DeclinePartyInvitation();
			}
			else if (this.Handler != null)
			{
				PermissionResult <>9__1;
				PlatformServices.Instance.CheckPrivilege(Privilege.Communication, true, delegate(bool privilegeResult)
				{
					if (!privilegeResult)
					{
						this.LobbyClient.DeclinePartyInvitation();
						return;
					}
					if (playerId.ProvidedType == NetworkMain.GameClient.PlayerID.ProvidedType)
					{
						IPlatformServices instance = PlatformServices.Instance;
						Permission permission = Permission.CommunicateUsingText;
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

		// Token: 0x06001E8C RID: 7820 RVA: 0x0006DA5D File Offset: 0x0006BC5D
		public void OnAdminMessageReceived(string message)
		{
			if (this.Handler != null)
			{
				this.Handler.OnAdminMessageReceived(message);
			}
		}

		// Token: 0x06001E8D RID: 7821 RVA: 0x0006DA73 File Offset: 0x0006BC73
		public void OnPartyInvitationInvalidated()
		{
			if (this.Handler != null)
			{
				this.Handler.OnPartyInvitationInvalidated();
			}
		}

		// Token: 0x06001E8E RID: 7822 RVA: 0x0006DA88 File Offset: 0x0006BC88
		public void OnPlayerInvitedToParty(PlayerId playerId)
		{
			if (this.Handler != null)
			{
				this.Handler.OnPlayerInvitedToParty(playerId);
			}
		}

		// Token: 0x06001E8F RID: 7823 RVA: 0x0006DAA0 File Offset: 0x0006BCA0
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
			if (PlatformServices.Instance.UnregisterPermissionChangeEvent(playerId, Permission.PlayMultiplayer, new PermissionChanged(this.MultiplayerPermissionWithPlayerChanged)))
			{
				bool flag;
				this._registeredPermissionEvents.TryRemove(new ValueTuple<PlayerId, Permission>(playerId, Permission.PlayMultiplayer), out flag);
			}
			if (this.Handler != null)
			{
				this.Handler.OnPlayerRemovedFromParty(playerId, reason);
			}
		}

		// Token: 0x06001E90 RID: 7824 RVA: 0x0006DB24 File Offset: 0x0006BD24
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
						PlatformServices.Instance.CheckPermissionWithUser(Permission.PlayMultiplayer, player.Item1, delegate(bool hasPermission)
						{
							if (!hasPermission)
							{
								NetworkMain.GameClient.KickPlayerFromParty(NetworkMain.GameClient.PlayerID);
								return;
							}
							if (PlatformServices.Instance.RegisterPermissionChangeEvent(player.Item1, Permission.PlayMultiplayer, new PermissionChanged(this.MultiplayerPermissionWithPlayerChanged)))
							{
								bool flag;
								this._registeredPermissionEvents.TryRemove(new ValueTuple<PlayerId, Permission>(player.Item1, Permission.PlayMultiplayer), out flag);
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
						PlatformServices.Instance.CheckPermissionWithUser(Permission.PlayMultiplayer, playerId, delegate(bool hasPermission)
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

		// Token: 0x06001E91 RID: 7825 RVA: 0x0006DCC0 File Offset: 0x0006BEC0
		private void MultiplayerPermissionWithPlayerChanged(PlayerId targetPlayerId, Permission permission, bool hasPermission)
		{
			if (!hasPermission && NetworkMain.GameClient.PlayersInParty.FirstOrDefault((PartyPlayerInLobbyClient p) => p.PlayerId == targetPlayerId) != null)
			{
				NetworkMain.GameClient.KickPlayerFromParty(NetworkMain.GameClient.PlayerID);
			}
		}

		// Token: 0x06001E92 RID: 7826 RVA: 0x0006DD10 File Offset: 0x0006BF10
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
			if (state == LobbyClient.State.SessionRequested)
			{
				MPPerkSelectionManager.Instance.InitializeForUser(this.LobbyClient.Name, this.LobbyClient.PlayerID);
			}
			else if (state == LobbyClient.State.Idle)
			{
				MPPerkSelectionManager.FreeInstance();
			}
			else if (!this.LobbyClient.AtLobby)
			{
				MPPerkSelectionManager.Instance.ResetPendingChanges();
			}
			PlatformServices.LobbyClientStateChanged(state == LobbyClient.State.AtLobby, !this.LobbyClient.IsInParty || this.LobbyClient.IsPartyLeader);
		}

		// Token: 0x06001E93 RID: 7827 RVA: 0x0006DDAD File Offset: 0x0006BFAD
		public void SetConnectionState(bool isAuthenticated)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler != null)
			{
				handler.SetConnectionState(isAuthenticated);
			}
			PlatformServices.ConnectionStateChanged(isAuthenticated);
		}

		// Token: 0x06001E94 RID: 7828 RVA: 0x0006DDC7 File Offset: 0x0006BFC7
		public void OnActivateHome()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateHome();
		}

		// Token: 0x06001E95 RID: 7829 RVA: 0x0006DDD9 File Offset: 0x0006BFD9
		public void OnActivateCustomServer()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateCustomServer();
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x0006DDEB File Offset: 0x0006BFEB
		public void OnActivateMatchmaking()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateMatchmaking();
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x0006DDFD File Offset: 0x0006BFFD
		public void OnActivateProfile()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateProfile();
		}

		// Token: 0x06001E98 RID: 7832 RVA: 0x0006DE0F File Offset: 0x0006C00F
		public void OnClanInvitationReceived(string clanName, string clanTag, bool isCreation)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanInvitationReceived(clanName, clanTag, isCreation);
		}

		// Token: 0x06001E99 RID: 7833 RVA: 0x0006DE24 File Offset: 0x0006C024
		public void OnClanInvitationAnswered(PlayerId playerId, ClanCreationAnswer answer)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanInvitationAnswered(playerId, answer);
		}

		// Token: 0x06001E9A RID: 7834 RVA: 0x0006DE38 File Offset: 0x0006C038
		public void OnClanCreationSuccessful()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanCreationSuccessful();
		}

		// Token: 0x06001E9B RID: 7835 RVA: 0x0006DE4A File Offset: 0x0006C04A
		public void OnClanCreationFailed()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanCreationFailed();
		}

		// Token: 0x06001E9C RID: 7836 RVA: 0x0006DE5C File Offset: 0x0006C05C
		public void OnClanCreationStarted()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnClanCreationStarted();
		}

		// Token: 0x06001E9D RID: 7837 RVA: 0x0006DE6E File Offset: 0x0006C06E
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

		// Token: 0x06001E9E RID: 7838 RVA: 0x0006DE9C File Offset: 0x0006C09C
		public void OnPremadeGameEligibilityStatusReceived(bool isEligible)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPremadeGameEligibilityStatusReceived(isEligible);
		}

		// Token: 0x06001E9F RID: 7839 RVA: 0x0006DEAF File Offset: 0x0006C0AF
		public void OnPremadeGameCreated()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPremadeGameCreated();
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x0006DEC1 File Offset: 0x0006C0C1
		public void OnPremadeGameListReceived()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPremadeGameListReceived();
		}

		// Token: 0x06001EA1 RID: 7841 RVA: 0x0006DED3 File Offset: 0x0006C0D3
		public void OnPremadeGameCreationCancelled()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPremadeGameCreationCancelled();
		}

		// Token: 0x06001EA2 RID: 7842 RVA: 0x0006DEE5 File Offset: 0x0006C0E5
		public void OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId, PlayerId[] challengerPlayerIDs, PlayerId challengerPartyLeaderID, PremadeGameType premadeGameType)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnJoinPremadeGameRequested(clanName, clanSigilCode, partyId, challengerPlayerIDs, challengerPartyLeaderID, premadeGameType);
		}

		// Token: 0x06001EA3 RID: 7843 RVA: 0x0006DF00 File Offset: 0x0006C100
		public void OnJoinPremadeGameRequestSuccessful()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnJoinPremadeGameRequestSuccessful();
		}

		// Token: 0x06001EA4 RID: 7844 RVA: 0x0006DF12 File Offset: 0x0006C112
		public void OnActivateArmory()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateArmory();
		}

		// Token: 0x06001EA5 RID: 7845 RVA: 0x0006DF24 File Offset: 0x0006C124
		public void OnActivateOptions()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnActivateOptions();
		}

		// Token: 0x06001EA6 RID: 7846 RVA: 0x0006DF36 File Offset: 0x0006C136
		public void OnDeactivateOptions()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnDeactivateOptions();
		}

		// Token: 0x06001EA7 RID: 7847 RVA: 0x0006DF48 File Offset: 0x0006C148
		public void OnCustomGameServerListReceived(AvailableCustomGames customGameServerList)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnCustomGameServerListReceived(customGameServerList);
		}

		// Token: 0x06001EA8 RID: 7848 RVA: 0x0006DF5B File Offset: 0x0006C15B
		public void OnMatchmakerGameOver(int oldExp, int newExp, List<string> badgesEarned, int lootGained, RankBarInfo oldRankBarInfo, RankBarInfo newRankBarInfo)
		{
			if (this.Handler != null)
			{
				this.Handler.OnMatchmakerGameOver(oldExp, newExp, badgesEarned, lootGained, oldRankBarInfo, newRankBarInfo);
			}
		}

		// Token: 0x06001EA9 RID: 7849 RVA: 0x0006DF79 File Offset: 0x0006C179
		public void OnBattleServerLost()
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnBattleServerLost();
		}

		// Token: 0x06001EAA RID: 7850 RVA: 0x0006DF8B File Offset: 0x0006C18B
		public void OnRemovedFromMatchmakerGame(DisconnectType disconnectType)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnRemovedFromMatchmakerGame(disconnectType);
		}

		// Token: 0x06001EAB RID: 7851 RVA: 0x0006DF9E File Offset: 0x0006C19E
		public void OnRemovedFromCustomGame(DisconnectType disconnectType)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnRemovedFromCustomGame(disconnectType);
		}

		// Token: 0x06001EAC RID: 7852 RVA: 0x0006DFB1 File Offset: 0x0006C1B1
		public void OnPlayerAssignedPartyLeader(PlayerId partyLeaderId)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerAssignedPartyLeader(partyLeaderId);
		}

		// Token: 0x06001EAD RID: 7853 RVA: 0x0006DFC4 File Offset: 0x0006C1C4
		public void OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerSuggestedToParty(playerId, playerName, suggestingPlayerId, suggestingPlayerName);
		}

		// Token: 0x06001EAE RID: 7854 RVA: 0x0006DFDB File Offset: 0x0006C1DB
		public void OnJoinCustomGameFailureResponse(CustomGameJoinResponse response)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnJoinCustomGameFailureResponse(response);
		}

		// Token: 0x06001EAF RID: 7855 RVA: 0x0006DFEE File Offset: 0x0006C1EE
		public void OnServerStatusReceived(ServerStatus serverStatus)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnServerStatusReceived(serverStatus);
		}

		// Token: 0x06001EB0 RID: 7856 RVA: 0x0006E001 File Offset: 0x0006C201
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

		// Token: 0x06001EB1 RID: 7857 RVA: 0x0006E025 File Offset: 0x0006C225
		public void OnRecentPlayerStatusesReceived(FriendInfo[] friends)
		{
			RecentPlayersFriendListService recentPlayersFriendListService = this._recentPlayersFriendListService;
			if (recentPlayersFriendListService == null)
			{
				return;
			}
			recentPlayersFriendListService.OnFriendListReceived(friends);
		}

		// Token: 0x06001EB2 RID: 7858 RVA: 0x0006E038 File Offset: 0x0006C238
		public void OnBattleServerInformationReceived(BattleServerInformationForClient battleServerInformation)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnBattleServerInformationReceived(battleServerInformation);
		}

		// Token: 0x06001EB3 RID: 7859 RVA: 0x0006E04B File Offset: 0x0006C24B
		public void OnRejoinBattleRequestAnswered(bool isSuccessful)
		{
			ILobbyStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnRejoinBattleRequestAnswered(isSuccessful);
		}

		// Token: 0x06001EB4 RID: 7860 RVA: 0x0006E05E File Offset: 0x0006C25E
		internal void OnSigilChanged()
		{
			if (this.Handler != null)
			{
				this.Handler.OnSigilChanged();
			}
		}

		// Token: 0x06001EB5 RID: 7861 RVA: 0x0006E073 File Offset: 0x0006C273
		public void OnNotificationsReceived(LobbyNotification[] notifications)
		{
			if (this.Handler != null)
			{
				this.Handler.OnNotificationsReceived(notifications);
			}
		}

		// Token: 0x06001EB6 RID: 7862 RVA: 0x0006E089 File Offset: 0x0006C289
		private void OnPlatformSignInStateUpdated(bool isSignedIn, TextObject message)
		{
			if (!isSignedIn && this.LobbyClient.Connected)
			{
				this.LobbyClient.Logout(message ?? new TextObject("{=oPOa77dI}Logged out of platform", null));
			}
		}

		// Token: 0x06001EB7 RID: 7863 RVA: 0x0006E0B8 File Offset: 0x0006C2B8
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
					Type type = array2[i].GetTypes().FirstOrDefault((Type ty) => ty.Name.Contains("CompressionInfo"));
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
					Type[] types = array2[i].GetTypes();
					for (int j = 0; j < types.Length; j++)
					{
						foreach (FieldInfo fieldInfo in types[j].GetFields())
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
				Debug.Print("CompressionInfoKey: " + num, 0, Debug.DebugColor.Cyan, 17179869184UL);
			}
			catch
			{
				Debug.Print("CompressionInfoKey checking failed.", 0, Debug.DebugColor.Cyan, 17179869184UL);
			}
		}

		// Token: 0x06001EB8 RID: 7864 RVA: 0x0006E2A4 File Offset: 0x0006C4A4
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

		// Token: 0x06001EB9 RID: 7865 RVA: 0x0006E2F4 File Offset: 0x0006C4F4
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
					while (this.LobbyClient.CurrentState != LobbyClient.State.Idle && this.LobbyClient.CurrentState != LobbyClient.State.AtLobby && waitTime < 3000)
					{
						await Task.Delay(100);
						waitTime += 100;
					}
				}
			}
		}

		// Token: 0x06001EBA RID: 7866 RVA: 0x0006E330 File Offset: 0x0006C530
		public async void OnSessionInvitationAccepted(SessionInvitationType targetGameType)
		{
			if (targetGameType == SessionInvitationType.Multiplayer)
			{
				PlatformServices.OnSessionInvitationHandled();
				await this.UpdateHasMultiplayerPrivilege();
				if (this.HasMultiplayerPrivilege != null && this.HasMultiplayerPrivilege.Value)
				{
					if (this.LobbyClient.IsIdle)
					{
						await this.TryLogin();
						int waitTime = 0;
						while (this.LobbyClient.CurrentState != LobbyClient.State.Idle && this.LobbyClient.CurrentState != LobbyClient.State.AtLobby && waitTime < 3000)
						{
							await Task.Delay(100);
							waitTime += 100;
						}
					}
					if (this.LobbyClient.CurrentState == LobbyClient.State.AtLobby)
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

		// Token: 0x06001EBB RID: 7867 RVA: 0x0006E374 File Offset: 0x0006C574
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

		// Token: 0x06001EBC RID: 7868 RVA: 0x0006E3C4 File Offset: 0x0006C5C4
		public void RegisterForCustomServerAction(Func<GameServerEntry, List<CustomServerAction>> action)
		{
			this._onCustomServerActionRequestedForServerEntry.Add(action);
		}

		// Token: 0x06001EBD RID: 7869 RVA: 0x0006E3D2 File Offset: 0x0006C5D2
		public void UnregisterForCustomServerAction(Func<GameServerEntry, List<CustomServerAction>> action)
		{
			this._onCustomServerActionRequestedForServerEntry.Remove(action);
		}

		// Token: 0x04000B39 RID: 2873
		private const string _newsSourceURLBase = "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_";

		// Token: 0x04000B3A RID: 2874
		private BannerlordFriendListService _bannerlordFriendListService;

		// Token: 0x04000B3B RID: 2875
		private RecentPlayersFriendListService _recentPlayersFriendListService;

		// Token: 0x04000B3C RID: 2876
		private ClanFriendListService _clanFriendListService;

		// Token: 0x04000B3E RID: 2878
		private readonly object _sessionInvitationDataLock = new object();

		// Token: 0x04000B3F RID: 2879
		[TupleElementNames(new string[] { "PlayerId", "Permission" })]
		private ConcurrentDictionary<ValueTuple<PlayerId, Permission>, bool> _registeredPermissionEvents = new ConcurrentDictionary<ValueTuple<PlayerId, Permission>, bool>();

		// Token: 0x04000B40 RID: 2880
		private ILobbyStateHandler _handler;

		// Token: 0x04000B41 RID: 2881
		private LobbyGameClientHandler _lobbyGameClientManager;

		// Token: 0x04000B42 RID: 2882
		private List<Func<GameServerEntry, List<CustomServerAction>>> _onCustomServerActionRequestedForServerEntry;

		// Token: 0x04000B44 RID: 2884
		public Action<bool> OnMultiplayerPrivilegeUpdated;

		// Token: 0x04000B45 RID: 2885
		public Action<bool> OnCrossplayPrivilegeUpdated;

		// Token: 0x04000B46 RID: 2886
		public Action<bool> OnUserGeneratedContentPrivilegeUpdated;
	}
}
