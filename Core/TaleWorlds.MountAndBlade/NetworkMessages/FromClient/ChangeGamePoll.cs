using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ChangeGamePoll : GameNetworkMessage
	{
		public string GameType { get; private set; }

		public string Map { get; private set; }

		public ChangeGamePoll(string gameType, string map)
		{
			this.GameType = gameType;
			this.Map = map;
		}

		public ChangeGamePoll()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.GameType = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.Map = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.GameType);
			GameNetworkMessage.WriteStringToPacket(this.Map);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return "Poll Requested: Change Map to: " + this.Map + " and GameType to: " + this.GameType;
		}
	}
}
