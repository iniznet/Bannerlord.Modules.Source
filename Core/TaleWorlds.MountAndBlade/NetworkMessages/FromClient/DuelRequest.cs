using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class DuelRequest : GameNetworkMessage
	{
		public int RequestedAgentIndex { get; private set; }

		public DuelRequest(int requestedAgentIndex)
		{
			this.RequestedAgentIndex = requestedAgentIndex;
		}

		public DuelRequest()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RequestedAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.RequestedAgentIndex);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return "Duel requested from agent with index: " + this.RequestedAgentIndex;
		}
	}
}
