using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BarkAgent : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public int IndexOfBark { get; private set; }

		public BarkAgent(Agent agent, int indexOfBark)
		{
			this.Agent = agent;
			this.IndexOfBark = indexOfBark;
		}

		public BarkAgent()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IndexOfBark = GameNetworkMessage.ReadIntFromPacket(CompressionMission.BarkIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket(this.IndexOfBark, CompressionMission.BarkIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"FromServer.BarkAgent: ",
				this.Agent.Index,
				", ",
				this.IndexOfBark
			});
		}
	}
}
