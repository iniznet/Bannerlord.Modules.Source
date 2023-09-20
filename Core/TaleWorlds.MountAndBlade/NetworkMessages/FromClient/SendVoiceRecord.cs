using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SendVoiceRecord : GameNetworkMessage
	{
		public byte[] Buffer { get; private set; }

		public int BufferLength { get; private set; }

		public List<VirtualPlayer> ReceiverList { get; private set; }

		public bool HasReceiverList { get; private set; }

		public SendVoiceRecord()
		{
		}

		public SendVoiceRecord(byte[] buffer, int bufferLength)
		{
			this.Buffer = buffer;
			this.BufferLength = bufferLength;
		}

		public SendVoiceRecord(byte[] buffer, int bufferLength, List<VirtualPlayer> receiverList)
		{
			this.Buffer = buffer;
			this.BufferLength = bufferLength;
			this.ReceiverList = receiverList;
			this.HasReceiverList = true;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteByteArrayToPacket(this.Buffer, 0, this.BufferLength);
			int num = 0;
			if (this.ReceiverList != null)
			{
				num = this.ReceiverList.Count;
			}
			GameNetworkMessage.WriteBoolToPacket(this.HasReceiverList);
			GameNetworkMessage.WriteIntToPacket(num, CompressionBasic.PlayerCompressionInfo);
			for (int i = 0; i < num; i++)
			{
				GameNetworkMessage.WriteVirtualPlayerReferenceToPacket(this.ReceiverList[i]);
			}
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Buffer = new byte[1440];
			this.BufferLength = GameNetworkMessage.ReadByteArrayFromPacket(this.Buffer, 0, 1440, ref flag);
			this.HasReceiverList = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			if (this.HasReceiverList)
			{
				this.ReceiverList = new List<VirtualPlayer>();
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						VirtualPlayer virtualPlayer = GameNetworkMessage.ReadVirtualPlayerReferenceToPacket(ref flag, false);
						this.ReceiverList.Add(virtualPlayer);
					}
				}
			}
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
