using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class PollResponse : GameNetworkMessage
	{
		public bool Accepted { get; private set; }

		public PollResponse(bool accepted)
		{
			this.Accepted = accepted;
		}

		public PollResponse()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Accepted = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.Accepted);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return "Receiving poll response: " + (this.Accepted ? "Accepted." : "Not accepted.");
		}
	}
}
