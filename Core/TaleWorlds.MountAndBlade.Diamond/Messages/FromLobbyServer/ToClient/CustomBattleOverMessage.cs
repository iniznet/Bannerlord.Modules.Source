using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000019 RID: 25
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CustomBattleOverMessage : Message
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000056 RID: 86 RVA: 0x000023E2 File Offset: 0x000005E2
		// (set) Token: 0x06000057 RID: 87 RVA: 0x000023EA File Offset: 0x000005EA
		public int OldExperience { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000058 RID: 88 RVA: 0x000023F3 File Offset: 0x000005F3
		// (set) Token: 0x06000059 RID: 89 RVA: 0x000023FB File Offset: 0x000005FB
		public int NewExperience { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002404 File Offset: 0x00000604
		// (set) Token: 0x0600005B RID: 91 RVA: 0x0000240C File Offset: 0x0000060C
		public int GoldGain { get; set; }

		// Token: 0x0600005C RID: 92 RVA: 0x00002415 File Offset: 0x00000615
		public CustomBattleOverMessage(int oldExperience, int newExperience, int goldGain)
		{
			this.OldExperience = oldExperience;
			this.NewExperience = newExperience;
			this.GoldGain = goldGain;
		}
	}
}
