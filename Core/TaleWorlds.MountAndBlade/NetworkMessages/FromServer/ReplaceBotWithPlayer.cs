using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ReplaceBotWithPlayer : GameNetworkMessage
	{
		public int BotAgentIndex { get; private set; }

		public NetworkCommunicator Peer { get; private set; }

		public int Health { get; private set; }

		public int MountHealth { get; private set; }

		public ReplaceBotWithPlayer(NetworkCommunicator peer, int botAgentIndex, float botAgentHealth, float botAgentMountHealth = -1f)
		{
			this.Peer = peer;
			this.BotAgentIndex = botAgentIndex;
			this.Health = MathF.Ceiling(botAgentHealth);
			this.MountHealth = MathF.Ceiling(botAgentMountHealth);
		}

		public ReplaceBotWithPlayer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.BotAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.Health = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref flag);
			this.MountHealth = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteAgentIndexToPacket(this.BotAgentIndex);
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
