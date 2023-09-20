using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000056 RID: 86
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class UpdatePlayerDataMessage : Message
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600013E RID: 318 RVA: 0x00002E27 File Offset: 0x00001027
		// (set) Token: 0x0600013F RID: 319 RVA: 0x00002E2F File Offset: 0x0000102F
		public PlayerData PlayerData { get; private set; }

		// Token: 0x06000140 RID: 320 RVA: 0x00002E38 File Offset: 0x00001038
		public UpdatePlayerDataMessage(PlayerData playerData)
		{
			this.PlayerData = playerData;
		}
	}
}
