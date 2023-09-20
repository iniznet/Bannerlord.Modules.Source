using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SiegeMoraleChangeMessage : GameNetworkMessage
	{
		public int AttackerMorale { get; private set; }

		public int DefenderMorale { get; private set; }

		public int[] CapturePointRemainingMoraleGains { get; private set; }

		public SiegeMoraleChangeMessage()
		{
		}

		public SiegeMoraleChangeMessage(int attackerMorale, int defenderMorale, int[] capturePointRemainingMoraleGains)
		{
			this.AttackerMorale = attackerMorale;
			this.DefenderMorale = defenderMorale;
			this.CapturePointRemainingMoraleGains = capturePointRemainingMoraleGains;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.AttackerMorale, CompressionMission.SiegeMoraleCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.DefenderMorale, CompressionMission.SiegeMoraleCompressionInfo);
			int[] capturePointRemainingMoraleGains = this.CapturePointRemainingMoraleGains;
			for (int i = 0; i < capturePointRemainingMoraleGains.Length; i++)
			{
				GameNetworkMessage.WriteIntToPacket(capturePointRemainingMoraleGains[i], CompressionMission.SiegeMoralePerFlagCompressionInfo);
			}
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AttackerMorale = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeMoraleCompressionInfo, ref flag);
			this.DefenderMorale = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeMoraleCompressionInfo, ref flag);
			this.CapturePointRemainingMoraleGains = new int[7];
			for (int i = 0; i < this.CapturePointRemainingMoraleGains.Length; i++)
			{
				this.CapturePointRemainingMoraleGains[i] = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeMoralePerFlagCompressionInfo, ref flag);
			}
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Morale synched. A: ", this.AttackerMorale, " D: ", this.DefenderMorale });
		}
	}
}
