using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200001D RID: 29
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class EnterCustomBattleWithPartyAnswer : Message
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00002492 File Offset: 0x00000692
		// (set) Token: 0x06000067 RID: 103 RVA: 0x0000249A File Offset: 0x0000069A
		public bool Successful { get; private set; }

		// Token: 0x06000068 RID: 104 RVA: 0x000024A3 File Offset: 0x000006A3
		public EnterCustomBattleWithPartyAnswer(bool successful)
		{
			this.Successful = successful;
		}
	}
}
