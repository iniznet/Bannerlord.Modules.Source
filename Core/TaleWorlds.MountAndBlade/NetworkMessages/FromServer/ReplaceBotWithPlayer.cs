using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ReplaceBotWithPlayer : GameNetworkMessage
	{
		public Agent BotAgent { get; private set; }

		public NetworkCommunicator Peer { get; private set; }

		public int Health { get; private set; }

		public int MountHealth { get; private set; }

		public ReplaceBotWithPlayer(NetworkCommunicator peer, Agent botAgent)
		{
			this.Peer = peer;
			this.BotAgent = botAgent;
			this.Health = MathF.Ceiling(botAgent.Health);
			Agent mountAgent = this.BotAgent.MountAgent;
			this.MountHealth = MathF.Ceiling((mountAgent != null) ? mountAgent.Health : (-1f));
		}

		public ReplaceBotWithPlayer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.BotAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Health = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref flag);
			this.MountHealth = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.BotAgent);
			GameNetworkMessage.WriteIntToPacket(this.Health, CompressionMission.AgentHealthCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.MountHealth, CompressionMission.AgentHealthCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		protected override string OnGetLogFormat()
		{
			return "Replace a bot with a player";
		}
	}
}
