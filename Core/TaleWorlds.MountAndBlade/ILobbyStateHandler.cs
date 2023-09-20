using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000230 RID: 560
	public interface ILobbyStateHandler
	{
		// Token: 0x06001ECB RID: 7883
		void SetConnectionState(bool isAuthenticated);

		// Token: 0x06001ECC RID: 7884
		string ShowFeedback(string title, string feedbackText);

		// Token: 0x06001ECD RID: 7885
		string ShowFeedback(InquiryData inquiryData);

		// Token: 0x06001ECE RID: 7886
		void DismissFeedback(string id);

		// Token: 0x06001ECF RID: 7887
		void OnPause();

		// Token: 0x06001ED0 RID: 7888
		void OnResume();

		// Token: 0x06001ED1 RID: 7889
		void OnDisconnected();

		// Token: 0x06001ED2 RID: 7890
		void OnRequestedToSearchBattle();

		// Token: 0x06001ED3 RID: 7891
		void OnUpdateFindingGame(MatchmakingWaitTimeStats matchmakingWaitTimeStats, string[] gameTypeInfo);

		// Token: 0x06001ED4 RID: 7892
		void OnRequestedToCancelSearchBattle();

		// Token: 0x06001ED5 RID: 7893
		void OnSearchBattleCanceled();

		// Token: 0x06001ED6 RID: 7894
		void OnPlayerDataReceived(PlayerData playerData);

		// Token: 0x06001ED7 RID: 7895
		void OnPendingRejoin();

		// Token: 0x06001ED8 RID: 7896
		void OnEnterBattleWithParty(string[] selectedGameTypes);

		// Token: 0x06001ED9 RID: 7897
		void OnPartyInvitationReceived(PlayerId playerId);

		// Token: 0x06001EDA RID: 7898
		void OnPartyInvitationInvalidated();

		// Token: 0x06001EDB RID: 7899
		void OnPlayerInvitedToParty(PlayerId playerId);

		// Token: 0x06001EDC RID: 7900
		void OnPlayerAddedToParty(PlayerId playerId, string playerName, bool isPartyLeader);

		// Token: 0x06001EDD RID: 7901
		void OnPlayerRemovedFromParty(PlayerId playerId, PartyRemoveReason reason);

		// Token: 0x06001EDE RID: 7902
		void OnPlayerNameUpdated(string newName);

		// Token: 0x06001EDF RID: 7903
		void OnGameClientStateChange(LobbyClient.State state);

		// Token: 0x06001EE0 RID: 7904
		void OnAdminMessageReceived(string message);

		// Token: 0x06001EE1 RID: 7905
		void OnActivateHome();

		// Token: 0x06001EE2 RID: 7906
		void OnActivateCustomServer();

		// Token: 0x06001EE3 RID: 7907
		void OnActivateMatchmaking();

		// Token: 0x06001EE4 RID: 7908
		void OnActivateArmory();

		// Token: 0x06001EE5 RID: 7909
		void OnActivateOptions();

		// Token: 0x06001EE6 RID: 7910
		void OnDeactivateOptions();

		// Token: 0x06001EE7 RID: 7911
		void OnCustomGameServerListReceived(AvailableCustomGames customGameServerList);

		// Token: 0x06001EE8 RID: 7912
		void OnMatchmakerGameOver(int oldExperience, int newExperience, List<string> badgesEarned, int lootGained, RankBarInfo oldRankBarInfo, RankBarInfo newRankBarInfo);

		// Token: 0x06001EE9 RID: 7913
		void OnBattleServerLost();

		// Token: 0x06001EEA RID: 7914
		void OnRemovedFromMatchmakerGame(DisconnectType disconnectType);

		// Token: 0x06001EEB RID: 7915
		void OnRemovedFromCustomGame(DisconnectType disconnectType);

		// Token: 0x06001EEC RID: 7916
		void OnPlayerAssignedPartyLeader(PlayerId partyLeaderId);

		// Token: 0x06001EED RID: 7917
		void OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName);

		// Token: 0x06001EEE RID: 7918
		void OnJoinCustomGameFailureResponse(CustomGameJoinResponse response);

		// Token: 0x06001EEF RID: 7919
		void OnRejoinBattleRequestAnswered(bool isSuccessful);

		// Token: 0x06001EF0 RID: 7920
		void OnServerStatusReceived(ServerStatus serverStatus);

		// Token: 0x06001EF1 RID: 7921
		void OnBattleServerInformationReceived(BattleServerInformationForClient battleServerInformation);

		// Token: 0x06001EF2 RID: 7922
		void OnActivateProfile();

		// Token: 0x06001EF3 RID: 7923
		void OnClanInvitationReceived(string clanName, string clanTag, bool isCreation);

		// Token: 0x06001EF4 RID: 7924
		void OnClanInvitationAnswered(PlayerId playerId, ClanCreationAnswer answer);

		// Token: 0x06001EF5 RID: 7925
		void OnClanCreationSuccessful();

		// Token: 0x06001EF6 RID: 7926
		void OnClanCreationFailed();

		// Token: 0x06001EF7 RID: 7927
		void OnClanCreationStarted();

		// Token: 0x06001EF8 RID: 7928
		void OnClanInfoChanged();

		// Token: 0x06001EF9 RID: 7929
		void OnPremadeGameEligibilityStatusReceived(bool isEligible);

		// Token: 0x06001EFA RID: 7930
		void OnPremadeGameCreated();

		// Token: 0x06001EFB RID: 7931
		void OnPremadeGameListReceived();

		// Token: 0x06001EFC RID: 7932
		void OnPremadeGameCreationCancelled();

		// Token: 0x06001EFD RID: 7933
		void OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId, PlayerId[] challengerPlayerIDs, PlayerId challengerPartyLeaderID, PremadeGameType premadeGameType);

		// Token: 0x06001EFE RID: 7934
		void OnJoinPremadeGameRequestSuccessful();

		// Token: 0x06001EFF RID: 7935
		void OnSigilChanged();

		// Token: 0x06001F00 RID: 7936
		void OnNotificationsReceived(LobbyNotification[] notifications);

		// Token: 0x06001F01 RID: 7937
		void OnFriendListUpdated();
	}
}
