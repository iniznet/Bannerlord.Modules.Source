using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class UnloadMission : GameNetworkMessage
	{
		public bool UnloadingForBattleIndexMismatch { get; private set; }

		public UnloadMission()
		{
		}

		public UnloadMission(bool unloadingForBattleIndexMismatch)
		{
			this.UnloadingForBattleIndexMismatch = unloadingForBattleIndexMismatch;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UnloadingForBattleIndexMismatch = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.UnloadingForBattleIndexMismatch);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Unload Mission";
		}
	}
}
