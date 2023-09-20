using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000102 RID: 258
	[Serializable]
	public class ClanPlayer
	{
		// Token: 0x170001CE RID: 462
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x000069EA File Offset: 0x00004BEA
		// (set) Token: 0x060004A9 RID: 1193 RVA: 0x000069F2 File Offset: 0x00004BF2
		public PlayerId PlayerId { get; private set; }

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x060004AA RID: 1194 RVA: 0x000069FB File Offset: 0x00004BFB
		// (set) Token: 0x060004AB RID: 1195 RVA: 0x00006A03 File Offset: 0x00004C03
		public Guid ClanId { get; private set; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x060004AC RID: 1196 RVA: 0x00006A0C File Offset: 0x00004C0C
		// (set) Token: 0x060004AD RID: 1197 RVA: 0x00006A14 File Offset: 0x00004C14
		public ClanPlayerRole Role { get; private set; }

		// Token: 0x060004AE RID: 1198 RVA: 0x00006A1D File Offset: 0x00004C1D
		public ClanPlayer(PlayerId playerId, Guid clanId, ClanPlayerRole role)
		{
			this.PlayerId = playerId;
			this.ClanId = clanId;
			this.Role = role;
		}
	}
}
