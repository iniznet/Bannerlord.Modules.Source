using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000120 RID: 288
	public class PartyPlayerInLobbyClient
	{
		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000679 RID: 1657 RVA: 0x0000AF66 File Offset: 0x00009166
		// (set) Token: 0x0600067A RID: 1658 RVA: 0x0000AF6E File Offset: 0x0000916E
		public PlayerId PlayerId { get; private set; }

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x0000AF77 File Offset: 0x00009177
		// (set) Token: 0x0600067C RID: 1660 RVA: 0x0000AF7F File Offset: 0x0000917F
		public string Name { get; private set; }

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x0600067D RID: 1661 RVA: 0x0000AF88 File Offset: 0x00009188
		// (set) Token: 0x0600067E RID: 1662 RVA: 0x0000AF90 File Offset: 0x00009190
		public bool WaitingInvitation { get; private set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x0600067F RID: 1663 RVA: 0x0000AF99 File Offset: 0x00009199
		// (set) Token: 0x06000680 RID: 1664 RVA: 0x0000AFA1 File Offset: 0x000091A1
		public bool IsPartyLeader { get; private set; }

		// Token: 0x06000681 RID: 1665 RVA: 0x0000AFAA File Offset: 0x000091AA
		public PartyPlayerInLobbyClient(PlayerId playerId, string name, bool isPartyLeader = false)
		{
			this.PlayerId = playerId;
			this.Name = name;
			this.IsPartyLeader = isPartyLeader;
			this.WaitingInvitation = true;
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x0000AFCE File Offset: 0x000091CE
		public void SetAtParty()
		{
			this.WaitingInvitation = false;
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x0000AFD7 File Offset: 0x000091D7
		public void SetLeader()
		{
			this.IsPartyLeader = true;
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x0000AFE0 File Offset: 0x000091E0
		public void SetMember()
		{
			this.IsPartyLeader = false;
		}
	}
}
