﻿using System;
using TaleWorlds.Library;
using TaleWorlds.Network;

namespace TaleWorlds.Diamond.Socket
{
	[MessageId(1)]
	public class SocketMessage : MessageContract
	{
		public Message Message { get; private set; }

		public SocketMessage()
		{
		}

		public SocketMessage(Message message)
		{
			this.Message = message;
		}

		public override void SerializeToNetworkMessage(INetworkMessageWriter networkMessage)
		{
			byte[] array = Common.SerializeObject(this.Message);
			networkMessage.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				networkMessage.Write(array[i]);
			}
		}

		public override void DeserializeFromNetworkMessage(INetworkMessageReader networkMessage)
		{
			byte[] array = new byte[networkMessage.ReadInt32()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = networkMessage.ReadByte();
			}
			this.Message = (Message)Common.DeserializeObject(array);
		}
	}
}
