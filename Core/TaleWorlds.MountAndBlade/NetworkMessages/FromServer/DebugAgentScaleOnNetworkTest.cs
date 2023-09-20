using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.DebugFromServer)]
	internal sealed class DebugAgentScaleOnNetworkTest : GameNetworkMessage
	{
		internal Agent AgentToTest { get; private set; }

		internal float ScaleToTest { get; private set; }

		public DebugAgentScaleOnNetworkTest()
		{
		}

		internal DebugAgentScaleOnNetworkTest(Agent toTest, float scale)
		{
			this.AgentToTest = toTest;
			this.ScaleToTest = scale;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentToTest = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.ScaleToTest = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.DebugScaleValueCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.AgentToTest);
			GameNetworkMessage.WriteFloatToPacket(this.ScaleToTest, CompressionMission.DebugScaleValueCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "DebugAgentScaleOnNetworkTest";
		}
	}
}
