using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class FinishedLoading : GameNetworkMessage
	{
		public int BattleIndex { get; private set; }

		public FinishedLoading()
		{
		}

		public FinishedLoading(int battleIndex)
		{
			this.BattleIndex = battleIndex;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.BattleIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AutomatedBattleIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.BattleIndex, CompressionMission.AutomatedBattleIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		protected override string OnGetLogFormat()
		{
			return "Finished Loading";
		}
	}
}
