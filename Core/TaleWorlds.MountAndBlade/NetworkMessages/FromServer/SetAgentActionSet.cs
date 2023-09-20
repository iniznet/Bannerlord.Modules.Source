using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200008A RID: 138
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentActionSet : GameNetworkMessage
	{
		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x0000A5E3 File Offset: 0x000087E3
		// (set) Token: 0x0600058A RID: 1418 RVA: 0x0000A5EB File Offset: 0x000087EB
		public Agent Agent { get; private set; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x0600058B RID: 1419 RVA: 0x0000A5F4 File Offset: 0x000087F4
		// (set) Token: 0x0600058C RID: 1420 RVA: 0x0000A5FC File Offset: 0x000087FC
		public MBActionSet ActionSet { get; private set; }

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x0000A605 File Offset: 0x00008805
		// (set) Token: 0x0600058E RID: 1422 RVA: 0x0000A60D File Offset: 0x0000880D
		public int NumPaces { get; private set; }

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600058F RID: 1423 RVA: 0x0000A616 File Offset: 0x00008816
		// (set) Token: 0x06000590 RID: 1424 RVA: 0x0000A61E File Offset: 0x0000881E
		public int MonsterUsageSetIndex { get; private set; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000591 RID: 1425 RVA: 0x0000A627 File Offset: 0x00008827
		// (set) Token: 0x06000592 RID: 1426 RVA: 0x0000A62F File Offset: 0x0000882F
		public float WalkingSpeedLimit { get; private set; }

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x0000A638 File Offset: 0x00008838
		// (set) Token: 0x06000594 RID: 1428 RVA: 0x0000A640 File Offset: 0x00008840
		public float CrouchWalkingSpeedLimit { get; private set; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000595 RID: 1429 RVA: 0x0000A649 File Offset: 0x00008849
		// (set) Token: 0x06000596 RID: 1430 RVA: 0x0000A651 File Offset: 0x00008851
		public float StepSize { get; private set; }

		// Token: 0x06000597 RID: 1431 RVA: 0x0000A65C File Offset: 0x0000885C
		public SetAgentActionSet(Agent agent, AnimationSystemData animationSystemData)
		{
			this.Agent = agent;
			this.ActionSet = animationSystemData.ActionSet;
			this.NumPaces = animationSystemData.NumPaces;
			this.MonsterUsageSetIndex = animationSystemData.MonsterUsageSetIndex;
			this.WalkingSpeedLimit = animationSystemData.WalkingSpeedLimit;
			this.CrouchWalkingSpeedLimit = animationSystemData.CrouchWalkingSpeedLimit;
			this.StepSize = animationSystemData.StepSize;
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x0000A6BE File Offset: 0x000088BE
		public SetAgentActionSet()
		{
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x0000A6C8 File Offset: 0x000088C8
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.ActionSet = GameNetworkMessage.ReadActionSetReferenceFromPacket(CompressionMission.ActionSetCompressionInfo, ref flag);
			this.NumPaces = GameNetworkMessage.ReadIntFromPacket(CompressionMission.NumberOfPacesCompressionInfo, ref flag);
			this.MonsterUsageSetIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MonsterUsageSetCompressionInfo, ref flag);
			this.WalkingSpeedLimit = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.WalkingSpeedLimitCompressionInfo, ref flag);
			this.CrouchWalkingSpeedLimit = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.WalkingSpeedLimitCompressionInfo, ref flag);
			this.StepSize = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.StepSizeCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x0000A754 File Offset: 0x00008954
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteActionSetReferenceToPacket(this.ActionSet, CompressionMission.ActionSetCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.NumPaces, CompressionMission.NumberOfPacesCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.MonsterUsageSetIndex, CompressionMission.MonsterUsageSetCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.WalkingSpeedLimit, CompressionMission.WalkingSpeedLimitCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.CrouchWalkingSpeedLimit, CompressionMission.WalkingSpeedLimitCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.StepSize, CompressionMission.StepSizeCompressionInfo);
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x0000A7CC File Offset: 0x000089CC
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentAnimations;
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x0000A7D4 File Offset: 0x000089D4
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set ActionSet: ",
				this.ActionSet,
				" on agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
