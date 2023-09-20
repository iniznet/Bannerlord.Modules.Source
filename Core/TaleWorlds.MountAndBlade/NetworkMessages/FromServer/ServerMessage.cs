using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ServerMessage : GameNetworkMessage
	{
		public string Message { get; private set; }

		public bool IsMessageTextId { get; private set; }

		public ServerMessage(string message, bool isMessageTextId = false)
		{
			this.Message = message;
			this.IsMessageTextId = isMessageTextId;
		}

		public ServerMessage()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.Message);
			GameNetworkMessage.WriteBoolToPacket(this.IsMessageTextId);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Message = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.IsMessageTextId = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Messaging;
		}

		protected override string OnGetLogFormat()
		{
			return "Message from server: " + this.Message;
		}
	}
}
