using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class PlayerMessageAll : GameNetworkMessage
	{
		public string Message { get; private set; }

		public List<VirtualPlayer> ReceiverList { get; private set; }

		public bool HasReceiverList { get; private set; }

		public PlayerMessageAll(string message, List<VirtualPlayer> receiverList)
		{
			this.Message = message;
			this.ReceiverList = receiverList;
			this.HasReceiverList = true;
		}

		public PlayerMessageAll(string message)
		{
			this.Message = message;
		}

		public PlayerMessageAll()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.Message);
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
			this.Message = GameNetworkMessage.ReadStringFromPacket(ref flag);
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
			return MultiplayerMessageFilter.Messaging;
		}

		protected override string OnGetLogFormat()
		{
			return "Receiving Player message to all: " + this.Message;
		}
	}
}
