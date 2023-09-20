using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200001A RID: 26
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CustomGameServerListResponse : FunctionResult
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00002432 File Offset: 0x00000632
		// (set) Token: 0x0600005E RID: 94 RVA: 0x0000243A File Offset: 0x0000063A
		public AvailableCustomGames AvailableCustomGames { get; private set; }

		// Token: 0x0600005F RID: 95 RVA: 0x00002443 File Offset: 0x00000643
		public CustomGameServerListResponse(AvailableCustomGames availableCustomGames)
		{
			this.AvailableCustomGames = availableCustomGames;
		}
	}
}
