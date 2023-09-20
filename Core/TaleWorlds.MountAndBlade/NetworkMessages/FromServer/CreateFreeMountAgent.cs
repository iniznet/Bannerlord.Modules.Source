using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateFreeMountAgent : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public EquipmentElement HorseItem { get; private set; }

		public EquipmentElement HorseHarnessItem { get; private set; }

		public Vec3 Position { get; private set; }

		public Vec2 Direction { get; private set; }

		public CreateFreeMountAgent(Agent agent, Vec3 position, Vec2 direction)
		{
			this.AgentIndex = agent.Index;
			this.HorseItem = agent.SpawnEquipment.GetEquipmentFromSlot(EquipmentIndex.ArmorItemEndSlot);
			this.HorseHarnessItem = agent.SpawnEquipment.GetEquipmentFromSlot(EquipmentIndex.HorseHarness);
			this.Position = position;
			this.Direction = direction.Normalized();
		}

		public CreateFreeMountAgent()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.HorseItem = ModuleNetworkData.ReadItemReferenceFromPacket(Game.Current.ObjectManager, ref flag);
			this.HorseHarnessItem = ModuleNetworkData.ReadItemReferenceFromPacket(Game.Current.ObjectManager, ref flag);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.Direction = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			ModuleNetworkData.WriteItemReferenceToPacket(this.HorseItem);
			ModuleNetworkData.WriteItemReferenceToPacket(this.HorseHarnessItem);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec2ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		protected override string OnGetLogFormat()
		{
			return "Create a mount-agent with index: " + this.AgentIndex;
		}
	}
}
