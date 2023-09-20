using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionCultureItemAdded : GameNetworkMessage
	{
		public string CultureId { get; private set; }

		public MultiplayerIntermissionCultureItemAdded()
		{
		}

		public MultiplayerIntermissionCultureItemAdded(string cultureId)
		{
			this.CultureId = cultureId;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.CultureId = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.CultureId);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return "Adding culture for voting with id: " + this.CultureId + ".";
		}
	}
}
