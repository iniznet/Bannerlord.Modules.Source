using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000015 RID: 21
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClientWantsToConnectCustomGameMessage : Message
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000048 RID: 72 RVA: 0x0000234A File Offset: 0x0000054A
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002352 File Offset: 0x00000552
		public PlayerJoinGameData[] PlayerJoinGameData { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600004A RID: 74 RVA: 0x0000235B File Offset: 0x0000055B
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002363 File Offset: 0x00000563
		public string Password { get; private set; }

		// Token: 0x0600004C RID: 76 RVA: 0x0000236C File Offset: 0x0000056C
		public ClientWantsToConnectCustomGameMessage(PlayerJoinGameData[] playerJoinGameData, string password)
		{
			this.PlayerJoinGameData = playerJoinGameData;
			this.Password = password;
		}
	}
}
