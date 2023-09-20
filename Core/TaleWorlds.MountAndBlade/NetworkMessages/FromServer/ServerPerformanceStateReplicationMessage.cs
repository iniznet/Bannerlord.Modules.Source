using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ServerPerformanceStateReplicationMessage : GameNetworkMessage
	{
		internal ServerPerformanceState ServerPerformanceProblemState { get; private set; }

		public ServerPerformanceStateReplicationMessage()
		{
		}

		internal ServerPerformanceStateReplicationMessage(ServerPerformanceState serverPerformanceProblemState)
		{
			this.ServerPerformanceProblemState = serverPerformanceProblemState;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.ServerPerformanceProblemState = (ServerPerformanceState)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ServerPerformanceStateCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.ServerPerformanceProblemState, CompressionBasic.ServerPerformanceStateCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "ServerPerformanceStateReplicationMessage";
		}
	}
}
