using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BarkAgent : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public int IndexOfBark { get; private set; }

		public BarkAgent(int agent, int indexOfBark)
		{
			this.AgentIndex = agent;
			this.IndexOfBark = indexOfBark;
		}

		public BarkAgent()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.IndexOfBark = GameNetworkMessage.ReadIntFromPacket(CompressionMission.BarkIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteIntToPacket(this.IndexOfBark, CompressionMission.BarkIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "FromServer.BarkAgent agent-index: ", this.AgentIndex, ", IndexOfBark", this.IndexOfBark });
		}
	}
}
