using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000126 RID: 294
	[Serializable]
	public class JoinGameData
	{
		// Token: 0x17000269 RID: 617
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x0000B30F File Offset: 0x0000950F
		public GameServerProperties GameServerProperties { get; }

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x060006C7 RID: 1735 RVA: 0x0000B317 File Offset: 0x00009517
		public int PeerIndex { get; }

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x0000B31F File Offset: 0x0000951F
		public int SessionKey { get; }

		// Token: 0x060006C9 RID: 1737 RVA: 0x0000B327 File Offset: 0x00009527
		public JoinGameData(GameServerProperties gameServerProperties, int peerIndex, int sessionKey)
		{
			this.GameServerProperties = gameServerProperties;
			this.PeerIndex = peerIndex;
			this.SessionKey = sessionKey;
		}
	}
}
