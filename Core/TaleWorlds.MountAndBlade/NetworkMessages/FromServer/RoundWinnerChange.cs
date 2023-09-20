using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RoundWinnerChange : GameNetworkMessage
	{
		public BattleSideEnum RoundWinner { get; private set; }

		public RoundWinnerChange(BattleSideEnum roundWinner)
		{
			this.RoundWinner = roundWinner;
		}

		public RoundWinnerChange()
		{
			this.RoundWinner = BattleSideEnum.None;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RoundWinner = (BattleSideEnum)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.RoundWinner, CompressionMission.TeamSideCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Change round winner to: " + this.RoundWinner.ToString();
		}
	}
}
