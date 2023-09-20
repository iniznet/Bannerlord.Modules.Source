using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000309 RID: 777
	public class MultiplayerData : MBMultiplayerData
	{
		// Token: 0x06002A15 RID: 10773 RVA: 0x000A2DD0 File Offset: 0x000A0FD0
		public MultiplayerData()
		{
			new List<NetworkCommunicator>();
		}

		// Token: 0x06002A16 RID: 10774 RVA: 0x000A2DE6 File Offset: 0x000A0FE6
		public bool IsMultiplayerTeamAvailable(int peerNo, int teamNo)
		{
			return false;
		}

		// Token: 0x04001010 RID: 4112
		public readonly int AutoTeamBalanceLimit = 50;
	}
}
