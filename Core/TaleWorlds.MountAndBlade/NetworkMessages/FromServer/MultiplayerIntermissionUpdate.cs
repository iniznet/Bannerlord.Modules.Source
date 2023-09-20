using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionUpdate : GameNetworkMessage
	{
		public MultiplayerIntermissionState IntermissionState { get; private set; }

		public float IntermissionTimer { get; private set; }

		public MultiplayerIntermissionUpdate()
		{
		}

		public MultiplayerIntermissionUpdate(MultiplayerIntermissionState intermissionState, float intermissionTimer)
		{
			this.IntermissionState = intermissionState;
			this.IntermissionTimer = intermissionTimer;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.IntermissionStateCompressionInfo, ref flag);
			this.IntermissionState = (MultiplayerIntermissionState)num;
			this.IntermissionTimer = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.IntermissionTimerCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.IntermissionState, CompressionBasic.IntermissionStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.IntermissionTimer, CompressionBasic.IntermissionTimerCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return "Receiving runtime intermission state.";
		}
	}
}
