using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class LoadMission : GameNetworkMessage
	{
		public string GameType { get; private set; }

		public string Map { get; private set; }

		public int BattleIndex { get; private set; }

		public LoadMission(string gameType, string map, int battleIndex)
		{
			this.GameType = gameType;
			this.Map = map;
			this.BattleIndex = battleIndex;
		}

		public LoadMission()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.GameType = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.Map = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.BattleIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AutomatedBattleIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.GameType);
			GameNetworkMessage.WriteStringToPacket(this.Map);
			GameNetworkMessage.WriteIntToPacket(this.BattleIndex, CompressionMission.AutomatedBattleIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Load Mission";
		}
	}
}
