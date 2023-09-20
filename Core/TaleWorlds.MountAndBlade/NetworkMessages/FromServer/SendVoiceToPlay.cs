using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SendVoiceToPlay : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public byte[] Buffer { get; private set; }

		public int BufferLength { get; private set; }

		public SendVoiceToPlay()
		{
		}

		public SendVoiceToPlay(NetworkCommunicator peer, byte[] buffer, int bufferLength)
		{
			this.Peer = peer;
			this.Buffer = buffer;
			this.BufferLength = bufferLength;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteByteArrayToPacket(this.Buffer, 0, this.BufferLength);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Buffer = new byte[1440];
			this.BufferLength = GameNetworkMessage.ReadByteArrayFromPacket(this.Buffer, 0, 1440, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		protected override string OnGetLogFormat()
		{
			return string.Empty;
		}
	}
}
