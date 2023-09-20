using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000115 RID: 277
	public interface IBattleServerSessionHandler
	{
		// Token: 0x06000544 RID: 1348
		void OnConnected();

		// Token: 0x06000545 RID: 1349
		void OnCantConnect();

		// Token: 0x06000546 RID: 1350
		void OnDisconnected();

		// Token: 0x06000547 RID: 1351
		void OnNewPlayer(BattlePeer peer);

		// Token: 0x06000548 RID: 1352
		void OnStartGame(string sceneName, string gameType, string faction1, string faction2, int minRequiredPlayerCountToStartBattle, int battleSize, string[] profanityList, string[] allowList);

		// Token: 0x06000549 RID: 1353
		void OnPlayerFledBattle(BattlePeer peer, out BattleResult battleResult);

		// Token: 0x0600054A RID: 1354
		void OnEndMission();

		// Token: 0x0600054B RID: 1355
		void OnStopServer();
	}
}
