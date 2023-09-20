using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000138 RID: 312
	[Serializable]
	public class PlayerBattleServerInformation
	{
		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000770 RID: 1904 RVA: 0x0000C1FA File Offset: 0x0000A3FA
		// (set) Token: 0x06000771 RID: 1905 RVA: 0x0000C202 File Offset: 0x0000A402
		public int PeerIndex { get; private set; }

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000772 RID: 1906 RVA: 0x0000C20B File Offset: 0x0000A40B
		// (set) Token: 0x06000773 RID: 1907 RVA: 0x0000C213 File Offset: 0x0000A413
		public int SessionKey { get; private set; }

		// Token: 0x06000774 RID: 1908 RVA: 0x0000C21C File Offset: 0x0000A41C
		public PlayerBattleServerInformation(int peerIndex, int sessionKey)
		{
			this.PeerIndex = peerIndex;
			this.SessionKey = sessionKey;
		}
	}
}
