using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A8 RID: 168
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeTowerHasArrivedAtTarget : GameNetworkMessage
	{
		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x0000C89C File Offset: 0x0000AA9C
		// (set) Token: 0x060006D3 RID: 1747 RVA: 0x0000C8A4 File Offset: 0x0000AAA4
		public SiegeTower SiegeTower { get; private set; }

		// Token: 0x060006D4 RID: 1748 RVA: 0x0000C8AD File Offset: 0x0000AAAD
		public SetSiegeTowerHasArrivedAtTarget(SiegeTower siegeTower)
		{
			this.SiegeTower = siegeTower;
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x0000C8BC File Offset: 0x0000AABC
		public SetSiegeTowerHasArrivedAtTarget()
		{
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x0000C8C4 File Offset: 0x0000AAC4
		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.SiegeTower = missionObject as SiegeTower;
			return flag;
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0000C8E8 File Offset: 0x0000AAE8
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeTower);
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x0000C8F5 File Offset: 0x0000AAF5
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeapons;
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x0000C900 File Offset: 0x0000AB00
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"SiegeTower with ID: ",
				this.SiegeTower.Id,
				" and name: ",
				this.SiegeTower.GameEntity.Name,
				" has arrived at its target."
			});
		}
	}
}
