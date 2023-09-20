using System;
using System.Threading.Tasks;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000117 RID: 279
	public interface ICustomBattleServerSessionHandler
	{
		// Token: 0x0600054D RID: 1357
		void OnConnected();

		// Token: 0x0600054E RID: 1358
		void OnCantConnect();

		// Token: 0x0600054F RID: 1359
		void OnDisconnected();

		// Token: 0x06000550 RID: 1360
		void OnStateChanged(CustomBattleServer.State state);

		// Token: 0x06000551 RID: 1361
		void OnSuccessfulGameRegister();

		// Token: 0x06000552 RID: 1362
		Task<PlayerJoinGameResponseDataFromHost[]> OnClientWantsToConnectCustomGame(PlayerJoinGameData[] playerJoinData, string password);

		// Token: 0x06000553 RID: 1363
		void OnClientQuitFromCustomGame(PlayerId playerId);

		// Token: 0x06000554 RID: 1364
		void OnGameFinished();

		// Token: 0x06000555 RID: 1365
		void OnChatFilterListsReceived(string[] profanityList, string[] allowList);

		// Token: 0x06000556 RID: 1366
		void OnPlayerKickRequested(PlayerId playerID, bool isBanning);
	}
}
