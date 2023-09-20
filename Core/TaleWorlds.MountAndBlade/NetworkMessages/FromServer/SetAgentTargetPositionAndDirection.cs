using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentTargetPositionAndDirection : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public Vec2 Position { get; private set; }

		public Vec3 Direction { get; private set; }

		public SetAgentTargetPositionAndDirection(Agent agent, ref Vec2 position, ref Vec3 direction)
		{
			this.Agent = agent;
			this.Position = position;
			this.Direction = direction;
		}

		public SetAgentTargetPositionAndDirection()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Position = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.Direction = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteVec2ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set TargetPositionAndDirection: ",
				this.Position,
				" ",
				this.Direction,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
