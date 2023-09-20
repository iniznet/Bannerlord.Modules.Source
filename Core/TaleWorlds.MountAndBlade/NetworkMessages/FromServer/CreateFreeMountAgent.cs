using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000075 RID: 117
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateFreeMountAgent : GameNetworkMessage
	{
		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000484 RID: 1156 RVA: 0x00008BCD File Offset: 0x00006DCD
		// (set) Token: 0x06000485 RID: 1157 RVA: 0x00008BD5 File Offset: 0x00006DD5
		public int AgentIndex { get; private set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000486 RID: 1158 RVA: 0x00008BDE File Offset: 0x00006DDE
		// (set) Token: 0x06000487 RID: 1159 RVA: 0x00008BE6 File Offset: 0x00006DE6
		public EquipmentElement HorseItem { get; private set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000488 RID: 1160 RVA: 0x00008BEF File Offset: 0x00006DEF
		// (set) Token: 0x06000489 RID: 1161 RVA: 0x00008BF7 File Offset: 0x00006DF7
		public EquipmentElement HorseHarnessItem { get; private set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x00008C00 File Offset: 0x00006E00
		// (set) Token: 0x0600048B RID: 1163 RVA: 0x00008C08 File Offset: 0x00006E08
		public Vec3 Position { get; private set; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600048C RID: 1164 RVA: 0x00008C11 File Offset: 0x00006E11
		// (set) Token: 0x0600048D RID: 1165 RVA: 0x00008C19 File Offset: 0x00006E19
		public Vec2 Direction { get; private set; }

		// Token: 0x0600048E RID: 1166 RVA: 0x00008C24 File Offset: 0x00006E24
		public CreateFreeMountAgent(Agent agent, Vec3 position, Vec2 direction)
		{
			this.AgentIndex = agent.Index;
			this.HorseItem = agent.SpawnEquipment.GetEquipmentFromSlot(EquipmentIndex.ArmorItemEndSlot);
			this.HorseHarnessItem = agent.SpawnEquipment.GetEquipmentFromSlot(EquipmentIndex.HorseHarness);
			this.Position = position;
			this.Direction = direction.Normalized();
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00008C7D File Offset: 0x00006E7D
		public CreateFreeMountAgent()
		{
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00008C88 File Offset: 0x00006E88
		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(CompressionMission.AgentCompressionInfo, ref flag);
			this.HorseItem = ModuleNetworkData.ReadItemReferenceFromPacket(Game.Current.ObjectManager, ref flag);
			this.HorseHarnessItem = ModuleNetworkData.ReadItemReferenceFromPacket(Game.Current.ObjectManager, ref flag);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.Direction = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x00008CFC File Offset: 0x00006EFC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.AgentIndex, CompressionMission.AgentCompressionInfo);
			ModuleNetworkData.WriteItemReferenceToPacket(this.HorseItem);
			ModuleNetworkData.WriteItemReferenceToPacket(this.HorseHarnessItem);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec2ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x00008D4F File Offset: 0x00006F4F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x00008D57 File Offset: 0x00006F57
		protected override string OnGetLogFormat()
		{
			return "Create a mount-agent with index: " + this.AgentIndex;
		}
	}
}
