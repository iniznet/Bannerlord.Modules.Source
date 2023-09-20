using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelRequest : GameNetworkMessage
	{
		public Agent RequesterAgent { get; private set; }

		public Agent RequestedAgent { get; private set; }

		public TroopType SelectedAreaTroopType { get; private set; }

		public DuelRequest(Agent requesterAgent, Agent requestedAgent, TroopType selectedAreaTroopType)
		{
			this.RequesterAgent = requesterAgent;
			this.RequestedAgent = requestedAgent;
			this.SelectedAreaTroopType = selectedAreaTroopType;
		}

		public DuelRequest()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RequesterAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.RequestedAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.SelectedAreaTroopType = (TroopType)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.TroopTypeCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.RequesterAgent);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.RequestedAgent);
			GameNetworkMessage.WriteIntToPacket((int)this.SelectedAreaTroopType, CompressionBasic.TroopTypeCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Request duel from agent with name: ",
				this.RequestedAgent.Name,
				" and index: ",
				this.RequestedAgent.Index
			});
		}
	}
}
