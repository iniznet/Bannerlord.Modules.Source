using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaleWorlds.Diamond.ChatSystem.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000128 RID: 296
	public interface ILobbyClientSessionHandler
	{
		// Token: 0x060006CA RID: 1738
		void OnConnected();

		// Token: 0x060006CB RID: 1739
		void OnCantConnect();

		// Token: 0x060006CC RID: 1740
		void OnDisconnected(TextObject feedback);

		// Token: 0x060006CD RID: 1741
		void OnPlayerDataReceived(PlayerData playerData);

		// Token: 0x060006CE RID: 1742
		void OnPendingRejoin();

		// Token: 0x060006CF RID: 1743
		void OnBattleResultReceived();

		// Token: 0x060006D0 RID: 1744
		void OnBattleServerInformationReceived(BattleServerInformationForClient battleServerInformation);

		// Token: 0x060006D1 RID: 1745
		void OnBattleServerLost();

		// Token: 0x060006D2 RID: 1746
		void OnCancelJoiningBattle();

		// Token: 0x060006D3 RID: 1747
		void OnRejoinRequestRejected();

		// Token: 0x060006D4 RID: 1748
		void OnFindGameAnswer(bool successful, string[] selectedAndDisabledGameTypes, bool isRejoin);

		// Token: 0x060006D5 RID: 1749
		void OnEnterBattleWithPartyAnswer(string[] selectedGameTypes);

		// Token: 0x060006D6 RID: 1750
		void OnWhisperMessageReceived(string fromPlayer, string toPlayer, string message);

		// Token: 0x060006D7 RID: 1751
		void OnClanMessageReceived(string playerName, string message);

		// Token: 0x060006D8 RID: 1752
		void OnChannelMessageReceived(ChatChannelType channel, string playerName, string message);

		// Token: 0x060006D9 RID: 1753
		void OnPartyMessageReceived(string playerName, string message);

		// Token: 0x060006DA RID: 1754
		void OnSystemMessageReceived(string message);

		// Token: 0x060006DB RID: 1755
		void OnAdminMessageReceived(string message);

		// Token: 0x060006DC RID: 1756
		void OnGameClientStateChange(LobbyClient.State oldState);

		// Token: 0x060006DD RID: 1757
		void OnCustomGameServerListReceived(AvailableCustomGames customGameServerList);

		// Token: 0x060006DE RID: 1758
		void OnPartyInvitationReceived(string inviterPlayerName, PlayerId inviterPlayerId);

		// Token: 0x060006DF RID: 1759
		void OnPartyInvitationInvalidated();

		// Token: 0x060006E0 RID: 1760
		void OnPlayerInvitedToParty(PlayerId playerId);

		// Token: 0x060006E1 RID: 1761
		void OnPlayersAddedToParty([TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })] List<ValueTuple<PlayerId, string, bool>> addedPlayers, [TupleElementNames(new string[] { "PlayerId", "PlayerName" })] List<ValueTuple<PlayerId, string>> invitedPlayers);

		// Token: 0x060006E2 RID: 1762
		void OnPlayerRemovedFromParty(PlayerId playerId, PartyRemoveReason reason);

		// Token: 0x060006E3 RID: 1763
		void OnPlayerAssignedPartyLeader(PlayerId partyLeaderId);

		// Token: 0x060006E4 RID: 1764
		void OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName);

		// Token: 0x060006E5 RID: 1765
		void OnServerStatusReceived(ServerStatus serverStatus);

		// Token: 0x060006E6 RID: 1766
		void OnSigilChanged();

		// Token: 0x060006E7 RID: 1767
		void OnFriendListReceived(FriendInfo[] friends);

		// Token: 0x060006E8 RID: 1768
		void OnRecentPlayerStatusesReceived(FriendInfo[] friends);

		// Token: 0x060006E9 RID: 1769
		void OnNotificationsReceived(LobbyNotification[] notifications);

		// Token: 0x060006EA RID: 1770
		void OnChatMessageReceived(Guid roomId, string roomName, string playerName, string messageText, string textColor, MessageType type);

		// Token: 0x060006EB RID: 1771
		void OnClanInvitationReceived(string clanName, string clanTag, bool isCreation);

		// Token: 0x060006EC RID: 1772
		void OnClanInvitationAnswered(PlayerId playerId, ClanCreationAnswer answer);

		// Token: 0x060006ED RID: 1773
		void OnClanCreationSuccessful();

		// Token: 0x060006EE RID: 1774
		void OnClanCreationFailed();

		// Token: 0x060006EF RID: 1775
		void OnClanCreationStarted();

		// Token: 0x060006F0 RID: 1776
		void OnClanInfoChanged();

		// Token: 0x060006F1 RID: 1777
		void OnPremadeGameEligibilityStatusReceived(bool isEligible);

		// Token: 0x060006F2 RID: 1778
		void OnPremadeGameCreated();

		// Token: 0x060006F3 RID: 1779
		void OnPremadeGameListReceived();

		// Token: 0x060006F4 RID: 1780
		void OnPremadeGameCreationCancelled();

		// Token: 0x060006F5 RID: 1781
		void OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId, PlayerId[] challengerPlayerIDs, PlayerId challengerPartyLeaderID, PremadeGameType premadeGameType);

		// Token: 0x060006F6 RID: 1782
		void OnJoinPremadeGameRequestSuccessful();

		// Token: 0x060006F7 RID: 1783
		void OnQuitFromMatchmakerGame();

		// Token: 0x060006F8 RID: 1784
		void OnMatchmakerGameOver(int oldExperience, int newExperience, List<string> badgesEarned, int lootGained, RankBarInfo oldRankBarInfo, RankBarInfo newRankBarInfo);

		// Token: 0x060006F9 RID: 1785
		void OnRemovedFromMatchmakerGame(DisconnectType disconnectType);

		// Token: 0x060006FA RID: 1786
		void OnRejoinBattleRequestAnswered(bool isSuccessful);

		// Token: 0x060006FB RID: 1787
		void OnRegisterCustomGameServerResponse();

		// Token: 0x060006FC RID: 1788
		void OnCustomGameEnd();

		// Token: 0x060006FD RID: 1789
		PlayerJoinGameResponseDataFromHost[] OnClientWantsToConnectCustomGame(PlayerJoinGameData[] playerJoinData, string password);

		// Token: 0x060006FE RID: 1790
		void OnClientQuitFromCustomGame(PlayerId playerId);

		// Token: 0x060006FF RID: 1791
		void OnJoinCustomGameResponse(bool success, JoinGameData joinGameData, CustomGameJoinResponse failureReason);

		// Token: 0x06000700 RID: 1792
		void OnJoinCustomGameFailureResponse(CustomGameJoinResponse response);

		// Token: 0x06000701 RID: 1793
		void OnQuitFromCustomGame();

		// Token: 0x06000702 RID: 1794
		void OnRemovedFromCustomGame(DisconnectType disconnectType);

		// Token: 0x06000703 RID: 1795
		void OnAnnouncementReceived(Announcement announcement);

		// Token: 0x06000704 RID: 1796
		Task<bool> OnInviteToPlatformSession(PlayerId playerId);

		// Token: 0x06000705 RID: 1797
		void OnEnterCustomBattleWithPartyAnswer();
	}
}
