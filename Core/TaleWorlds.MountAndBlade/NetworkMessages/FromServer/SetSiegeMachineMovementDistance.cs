using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A6 RID: 166
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeMachineMovementDistance : GameNetworkMessage
	{
		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x0000C69C File Offset: 0x0000A89C
		// (set) Token: 0x060006BF RID: 1727 RVA: 0x0000C6A4 File Offset: 0x0000A8A4
		public UsableMachine UsableMachine { get; private set; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x0000C6AD File Offset: 0x0000A8AD
		// (set) Token: 0x060006C1 RID: 1729 RVA: 0x0000C6B5 File Offset: 0x0000A8B5
		public float Distance { get; private set; }

		// Token: 0x060006C2 RID: 1730 RVA: 0x0000C6BE File Offset: 0x0000A8BE
		public SetSiegeMachineMovementDistance(UsableMachine usableMachine, float distance)
		{
			this.UsableMachine = usableMachine;
			this.Distance = distance;
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x0000C6D4 File Offset: 0x0000A8D4
		public SetSiegeMachineMovementDistance()
		{
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0000C6DC File Offset: 0x0000A8DC
		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.UsableMachine = missionObject as UsableMachine;
			this.Distance = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x0000C712 File Offset: 0x0000A912
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableMachine);
			GameNetworkMessage.WriteFloatToPacket(this.Distance, CompressionBasic.PositionCompressionInfo);
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x0000C72F File Offset: 0x0000A92F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0000C738 File Offset: 0x0000A938
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Movement Distance: ",
				this.Distance,
				" of SiegeMachine with ID: ",
				this.UsableMachine.Id,
				" and name: ",
				this.UsableMachine.GameEntity.Name
			});
		}
	}
}
