using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A5 RID: 165
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeLadderState : GameNetworkMessage
	{
		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060006B4 RID: 1716 RVA: 0x0000C599 File Offset: 0x0000A799
		// (set) Token: 0x060006B5 RID: 1717 RVA: 0x0000C5A1 File Offset: 0x0000A7A1
		public SiegeLadder SiegeLadder { get; private set; }

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x0000C5AA File Offset: 0x0000A7AA
		// (set) Token: 0x060006B7 RID: 1719 RVA: 0x0000C5B2 File Offset: 0x0000A7B2
		public SiegeLadder.LadderState State { get; private set; }

		// Token: 0x060006B8 RID: 1720 RVA: 0x0000C5BB File Offset: 0x0000A7BB
		public SetSiegeLadderState(SiegeLadder siegeLadder, SiegeLadder.LadderState state)
		{
			this.SiegeLadder = siegeLadder;
			this.State = state;
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0000C5D1 File Offset: 0x0000A7D1
		public SetSiegeLadderState()
		{
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0000C5DC File Offset: 0x0000A7DC
		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.SiegeLadder = missionObject as SiegeLadder;
			this.State = (SiegeLadder.LadderState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeLadderStateCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x0000C612 File Offset: 0x0000A812
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeLadder);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.SiegeLadderStateCompressionInfo);
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x0000C62F File Offset: 0x0000A82F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x0000C638 File Offset: 0x0000A838
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set SiegeLadder State to: ",
				this.State,
				" on SiegeLadderState with ID: ",
				this.SiegeLadder.Id,
				" and name: ",
				this.SiegeLadder.GameEntity.Name
			});
		}
	}
}
