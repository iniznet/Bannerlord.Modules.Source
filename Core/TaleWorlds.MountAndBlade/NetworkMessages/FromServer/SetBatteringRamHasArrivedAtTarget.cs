using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000091 RID: 145
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetBatteringRamHasArrivedAtTarget : GameNetworkMessage
	{
		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060005DD RID: 1501 RVA: 0x0000AEA5 File Offset: 0x000090A5
		// (set) Token: 0x060005DE RID: 1502 RVA: 0x0000AEAD File Offset: 0x000090AD
		public BatteringRam BatteringRam { get; private set; }

		// Token: 0x060005DF RID: 1503 RVA: 0x0000AEB6 File Offset: 0x000090B6
		public SetBatteringRamHasArrivedAtTarget(BatteringRam batteringRam)
		{
			this.BatteringRam = batteringRam;
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x0000AEC5 File Offset: 0x000090C5
		public SetBatteringRamHasArrivedAtTarget()
		{
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x0000AED0 File Offset: 0x000090D0
		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.BatteringRam = missionObject as BatteringRam;
			return flag;
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x0000AEF4 File Offset: 0x000090F4
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.BatteringRam);
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0000AF01 File Offset: 0x00009101
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x0000AF0C File Offset: 0x0000910C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Battering Ram with ID: ",
				this.BatteringRam.Id,
				" and name: ",
				this.BatteringRam.GameEntity.Name,
				" has arrived at its target."
			});
		}
	}
}
