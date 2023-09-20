using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class InitializeCustomGameMessage : GameNetworkMessage
	{
		public bool InMission { get; private set; }

		public string GameType { get; private set; }

		public string Map { get; private set; }

		public int BattleIndex { get; private set; }

		public InitializeCustomGameMessage(bool inMission, string gameType, string map, int battleIndex)
		{
			this.InMission = inMission;
			this.GameType = gameType;
			this.Map = map;
			this.BattleIndex = battleIndex;
		}

		public InitializeCustomGameMessage()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.InMission = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.GameType = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.Map = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.BattleIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AutomatedBattleIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.InMission);
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
			return "Initialize Custom Game";
		}
	}
}
