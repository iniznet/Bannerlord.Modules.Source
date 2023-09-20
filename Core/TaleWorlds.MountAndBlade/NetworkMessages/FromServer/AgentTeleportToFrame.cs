using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AgentTeleportToFrame : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public Vec3 Position { get; private set; }

		public Vec2 Direction { get; private set; }

		public AgentTeleportToFrame(Agent agent, Vec3 position, Vec2 direction)
		{
			this.Agent = agent;
			this.Position = position;
			this.Direction = direction.Normalized();
		}

		public AgentTeleportToFrame()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.Direction = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec2ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Teleporting agent with name: ",
				this.Agent.Name,
				", and index: ",
				this.Agent.Index,
				" to frame with position: ",
				this.Position,
				" and direction: ",
				this.Direction
			});
		}
	}
}
