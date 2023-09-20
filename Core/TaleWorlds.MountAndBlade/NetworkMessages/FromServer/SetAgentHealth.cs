using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentHealth : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public int Health { get; private set; }

		public SetAgentHealth(Agent agent, int newHealth)
		{
			this.Agent = agent;
			this.Health = newHealth;
		}

		public SetAgentHealth()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Health = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket(this.Health, CompressionMission.AgentHealthCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set agent health to: ",
				this.Health,
				", for ",
				(!this.Agent.IsMount) ? " YOU" : " YOUR MOUNT"
			});
		}
	}
}
