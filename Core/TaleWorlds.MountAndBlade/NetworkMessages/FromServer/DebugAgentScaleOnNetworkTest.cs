using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.DebugFromServer)]
	internal sealed class DebugAgentScaleOnNetworkTest : GameNetworkMessage
	{
		internal int AgentToTestIndex { get; private set; }

		internal float ScaleToTest { get; private set; }

		public DebugAgentScaleOnNetworkTest()
		{
		}

		internal DebugAgentScaleOnNetworkTest(int agentToTestIndex, float scale)
		{
			this.AgentToTestIndex = agentToTestIndex;
			this.ScaleToTest = scale;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentToTestIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.ScaleToTest = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.DebugScaleValueCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentToTestIndex);
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
