using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BotData : GameNetworkMessage
	{
		public BattleSideEnum Side { get; private set; }

		public int KillCount { get; private set; }

		public int AssistCount { get; private set; }

		public int DeathCount { get; private set; }

		public int AliveBotCount { get; private set; }

		public BotData(BattleSideEnum side, int kill, int assist, int death, int alive)
		{
			this.Side = side;
			this.KillCount = kill;
			this.AssistCount = assist;
			this.DeathCount = death;
			this.AliveBotCount = alive;
		}

		public BotData()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Side = (BattleSideEnum)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			this.KillCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.AssistCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.DeathCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.AliveBotCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.Side, CompressionMission.TeamSideCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.KillCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.AssistCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.DeathCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.AliveBotCount, CompressionMission.AgentCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "BOTS for side: ", this.Side, ", Kill: ", this.KillCount, " Death: ", this.DeathCount, " Assist: ", this.AssistCount, ", Alive: ", this.AliveBotCount });
		}
	}
}
