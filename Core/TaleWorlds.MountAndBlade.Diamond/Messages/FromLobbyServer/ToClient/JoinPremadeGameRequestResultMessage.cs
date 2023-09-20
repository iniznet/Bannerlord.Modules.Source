using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200003C RID: 60
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinPremadeGameRequestResultMessage : Message
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00002A45 File Offset: 0x00000C45
		// (set) Token: 0x060000E8 RID: 232 RVA: 0x00002A4D File Offset: 0x00000C4D
		public bool Successful { get; private set; }

		// Token: 0x060000E9 RID: 233 RVA: 0x00002A56 File Offset: 0x00000C56
		public JoinPremadeGameRequestResultMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
