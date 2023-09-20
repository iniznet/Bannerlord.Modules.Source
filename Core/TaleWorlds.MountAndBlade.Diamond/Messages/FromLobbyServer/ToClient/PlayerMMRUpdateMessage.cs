using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000045 RID: 69
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PlayerMMRUpdateMessage : Message
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00002B3B File Offset: 0x00000D3B
		// (set) Token: 0x060000FF RID: 255 RVA: 0x00002B43 File Offset: 0x00000D43
		public RankBarInfo OldInfo { get; private set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00002B4C File Offset: 0x00000D4C
		// (set) Token: 0x06000101 RID: 257 RVA: 0x00002B54 File Offset: 0x00000D54
		public RankBarInfo NewInfo { get; private set; }

		// Token: 0x06000102 RID: 258 RVA: 0x00002B5D File Offset: 0x00000D5D
		public PlayerMMRUpdateMessage(RankBarInfo oldInfo, RankBarInfo newInfo)
		{
			this.OldInfo = oldInfo;
			this.NewInfo = newInfo;
		}
	}
}
