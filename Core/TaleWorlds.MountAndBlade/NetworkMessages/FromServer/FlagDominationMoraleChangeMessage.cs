using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class FlagDominationMoraleChangeMessage : GameNetworkMessage
	{
		public float Morale { get; private set; }

		public FlagDominationMoraleChangeMessage()
		{
		}

		public FlagDominationMoraleChangeMessage(float morale)
		{
			this.Morale = morale;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteFloatToPacket(this.Morale, CompressionMission.FlagDominationMoraleCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Morale = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagDominationMoraleCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return "Morale synched: " + this.Morale;
		}
	}
}
