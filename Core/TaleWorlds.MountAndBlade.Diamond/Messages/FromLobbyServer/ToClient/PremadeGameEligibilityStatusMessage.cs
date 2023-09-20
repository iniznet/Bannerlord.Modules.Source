using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200004C RID: 76
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PremadeGameEligibilityStatusMessage : Message
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600011C RID: 284 RVA: 0x00002CB2 File Offset: 0x00000EB2
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00002CBA File Offset: 0x00000EBA
		public PremadeGameType[] EligibleGameTypes { get; private set; }

		// Token: 0x0600011E RID: 286 RVA: 0x00002CC3 File Offset: 0x00000EC3
		public PremadeGameEligibilityStatusMessage(PremadeGameType[] eligibleGameTypes)
		{
			this.EligibleGameTypes = eligibleGameTypes;
		}
	}
}
