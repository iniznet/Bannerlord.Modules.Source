using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelRequest : GameNetworkMessage
	{
		public int RequesterAgentIndex { get; private set; }

		public int RequestedAgentIndex { get; private set; }

		public TroopType SelectedAreaTroopType { get; private set; }

		public DuelRequest(int requesterAgentIndex, int requestedAgentIndex, TroopType selectedAreaTroopType)
		{
			this.RequesterAgentIndex = requesterAgentIndex;
			this.RequestedAgentIndex = requestedAgentIndex;
			this.SelectedAreaTroopType = selectedAreaTroopType;
		}

		public DuelRequest()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RequesterAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.RequestedAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.SelectedAreaTroopType = (TroopType)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.TroopTypeCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.RequesterAgentIndex);
			GameNetworkMessage.WriteAgentIndexToPacket(this.RequestedAgentIndex);
			GameNetworkMessage.WriteIntToPacket((int)this.SelectedAreaTroopType, CompressionBasic.TroopTypeCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return "Request duel from agent with index: " + this.RequestedAgentIndex;
		}
	}
}
