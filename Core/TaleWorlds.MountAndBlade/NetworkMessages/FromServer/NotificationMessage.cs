using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class NotificationMessage : GameNetworkMessage
	{
		public int Message { get; private set; }

		public int ParameterOne { get; private set; }

		public int ParameterTwo { get; private set; }

		private bool HasParameterOne
		{
			get
			{
				return this.ParameterOne != -1;
			}
		}

		private bool HasParameterTwo
		{
			get
			{
				return this.ParameterOne != -1;
			}
		}

		public NotificationMessage(int message, int param1, int param2)
		{
			this.Message = message;
			this.ParameterOne = param1;
			this.ParameterTwo = param2;
		}

		public NotificationMessage()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.Message, CompressionMission.MultiplayerNotificationCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.HasParameterOne);
			if (this.HasParameterOne)
			{
				GameNetworkMessage.WriteIntToPacket(this.ParameterOne, CompressionMission.MultiplayerNotificationParameterCompressionInfo);
				GameNetworkMessage.WriteBoolToPacket(this.HasParameterTwo);
				if (this.HasParameterTwo)
				{
					GameNetworkMessage.WriteIntToPacket(this.ParameterTwo, CompressionMission.MultiplayerNotificationParameterCompressionInfo);
				}
			}
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.ParameterOne = (this.ParameterTwo = -1);
			this.Message = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MultiplayerNotificationCompressionInfo, ref flag);
			if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
			{
				this.ParameterOne = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MultiplayerNotificationParameterCompressionInfo, ref flag);
				if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
				{
					this.ParameterTwo = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MultiplayerNotificationParameterCompressionInfo, ref flag);
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
			return string.Concat(new object[]
			{
				"Receiving message: ",
				this.Message,
				this.HasParameterOne ? (" With first parameter: " + this.ParameterOne) : "",
				this.HasParameterTwo ? (" and second parameter: " + this.ParameterTwo) : ""
			});
		}
	}
}
