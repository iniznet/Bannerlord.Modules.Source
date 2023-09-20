using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionMapItemAdded : GameNetworkMessage
	{
		public string MapId { get; private set; }

		public MultiplayerIntermissionMapItemAdded()
		{
		}

		public MultiplayerIntermissionMapItemAdded(string mapId)
		{
			this.MapId = mapId;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MapId = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.MapId);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return "Adding map for voting with id: " + this.MapId + ".";
		}
	}
}
