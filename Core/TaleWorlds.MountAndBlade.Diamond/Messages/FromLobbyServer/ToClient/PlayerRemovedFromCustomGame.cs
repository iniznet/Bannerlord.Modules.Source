using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000047 RID: 71
	[Serializable]
	public class PlayerRemovedFromCustomGame : Message
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000104 RID: 260 RVA: 0x00002B7B File Offset: 0x00000D7B
		// (set) Token: 0x06000105 RID: 261 RVA: 0x00002B83 File Offset: 0x00000D83
		public DisconnectType DisconnectType { get; private set; }

		// Token: 0x06000106 RID: 262 RVA: 0x00002B8C File Offset: 0x00000D8C
		public PlayerRemovedFromCustomGame(DisconnectType disconnectType)
		{
			this.DisconnectType = disconnectType;
		}
	}
}
