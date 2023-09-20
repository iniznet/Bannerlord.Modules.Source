using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200001C RID: 28
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class EnterBattleWithPartyAnswer : Message
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000061 RID: 97 RVA: 0x0000245A File Offset: 0x0000065A
		// (set) Token: 0x06000062 RID: 98 RVA: 0x00002462 File Offset: 0x00000662
		public bool Successful { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000063 RID: 99 RVA: 0x0000246B File Offset: 0x0000066B
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00002473 File Offset: 0x00000673
		public string[] SelectedAndEnabledGameTypes { get; private set; }

		// Token: 0x06000065 RID: 101 RVA: 0x0000247C File Offset: 0x0000067C
		public EnterBattleWithPartyAnswer(bool successful, string[] selectedAndEnabledGameTypes)
		{
			this.Successful = successful;
			this.SelectedAndEnabledGameTypes = selectedAndEnabledGameTypes;
		}
	}
}
