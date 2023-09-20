using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentTargetPosition : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public Vec2 Position { get; private set; }

		public SetAgentTargetPosition(Agent agent, ref Vec2 position)
		{
			this.Agent = agent;
			this.Position = position;
		}

		public SetAgentTargetPosition()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Position = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteVec2ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Target Position: ",
				this.Position,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
