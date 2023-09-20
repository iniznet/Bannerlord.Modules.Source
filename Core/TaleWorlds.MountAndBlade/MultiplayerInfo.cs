using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200030A RID: 778
	public class MultiplayerInfo
	{
		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x06002A17 RID: 10775 RVA: 0x000A2DE9 File Offset: 0x000A0FE9
		public MultiplayerData MultiplayerDataValues
		{
			get
			{
				return this.multiplayerDataValues;
			}
		}

		// Token: 0x06002A18 RID: 10776 RVA: 0x000A2DF1 File Offset: 0x000A0FF1
		public MultiplayerInfo()
		{
			this.multiplayerDataValues = new MultiplayerData();
		}

		// Token: 0x06002A19 RID: 10777 RVA: 0x000A2E04 File Offset: 0x000A1004
		public bool IsMultiplayerTeamAvailable(int peerNo, int teamNo)
		{
			return true;
		}

		// Token: 0x04001011 RID: 4113
		protected MultiplayerData multiplayerDataValues;
	}
}
