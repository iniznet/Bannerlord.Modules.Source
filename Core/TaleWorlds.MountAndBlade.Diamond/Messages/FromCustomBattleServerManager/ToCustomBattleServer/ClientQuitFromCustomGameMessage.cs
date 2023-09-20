using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	// Token: 0x02000062 RID: 98
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[Serializable]
	public class ClientQuitFromCustomGameMessage : Message
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000191 RID: 401 RVA: 0x00003210 File Offset: 0x00001410
		// (set) Token: 0x06000192 RID: 402 RVA: 0x00003218 File Offset: 0x00001418
		public PlayerId PlayerId { get; private set; }

		// Token: 0x06000193 RID: 403 RVA: 0x00003221 File Offset: 0x00001421
		public ClientQuitFromCustomGameMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
