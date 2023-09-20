using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000048 RID: 72
	[Serializable]
	public class PlayerRemovedFromMatchmakerGame : Message
	{
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00002B9B File Offset: 0x00000D9B
		// (set) Token: 0x06000108 RID: 264 RVA: 0x00002BA3 File Offset: 0x00000DA3
		public DisconnectType DisconnectType { get; private set; }

		// Token: 0x06000109 RID: 265 RVA: 0x00002BAC File Offset: 0x00000DAC
		public PlayerRemovedFromMatchmakerGame(DisconnectType disconnectType)
		{
			this.DisconnectType = disconnectType;
		}
	}
}
