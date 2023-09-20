using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A7 RID: 167
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeTowerGateState : GameNetworkMessage
	{
		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x0000C79C File Offset: 0x0000A99C
		// (set) Token: 0x060006C9 RID: 1737 RVA: 0x0000C7A4 File Offset: 0x0000A9A4
		public SiegeTower SiegeTower { get; private set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x0000C7AD File Offset: 0x0000A9AD
		// (set) Token: 0x060006CB RID: 1739 RVA: 0x0000C7B5 File Offset: 0x0000A9B5
		public SiegeTower.GateState State { get; private set; }

		// Token: 0x060006CC RID: 1740 RVA: 0x0000C7BE File Offset: 0x0000A9BE
		public SetSiegeTowerGateState(SiegeTower siegeTower, SiegeTower.GateState state)
		{
			this.SiegeTower = siegeTower;
			this.State = state;
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0000C7D4 File Offset: 0x0000A9D4
		public SetSiegeTowerGateState()
		{
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x0000C7DC File Offset: 0x0000A9DC
		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.SiegeTower = missionObject as SiegeTower;
			this.State = (SiegeTower.GateState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeTowerGateStateCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x0000C812 File Offset: 0x0000AA12
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeTower);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.SiegeTowerGateStateCompressionInfo);
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x0000C82F File Offset: 0x0000AA2F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x0000C838 File Offset: 0x0000AA38
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set SiegeTower State to: ",
				this.State,
				" on SiegeTower with ID: ",
				this.SiegeTower.Id,
				" and name: ",
				this.SiegeTower.GameEntity.Name
			});
		}
	}
}
