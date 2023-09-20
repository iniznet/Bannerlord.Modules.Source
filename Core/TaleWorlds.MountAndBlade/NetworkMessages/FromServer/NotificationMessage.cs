using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000C6 RID: 198
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class NotificationMessage : GameNetworkMessage
	{
		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000822 RID: 2082 RVA: 0x0000E8EB File Offset: 0x0000CAEB
		// (set) Token: 0x06000823 RID: 2083 RVA: 0x0000E8F3 File Offset: 0x0000CAF3
		public int Message { get; private set; }

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000824 RID: 2084 RVA: 0x0000E8FC File Offset: 0x0000CAFC
		// (set) Token: 0x06000825 RID: 2085 RVA: 0x0000E904 File Offset: 0x0000CB04
		public int ParameterOne { get; private set; }

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000826 RID: 2086 RVA: 0x0000E90D File Offset: 0x0000CB0D
		// (set) Token: 0x06000827 RID: 2087 RVA: 0x0000E915 File Offset: 0x0000CB15
		public int ParameterTwo { get; private set; }

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000828 RID: 2088 RVA: 0x0000E91E File Offset: 0x0000CB1E
		private bool HasParameterOne
		{
			get
			{
				return this.ParameterOne != -1;
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000829 RID: 2089 RVA: 0x0000E92C File Offset: 0x0000CB2C
		private bool HasParameterTwo
		{
			get
			{
				return this.ParameterOne != -1;
			}
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0000E93A File Offset: 0x0000CB3A
		public NotificationMessage(int message, int param1, int param2)
		{
			this.Message = message;
			this.ParameterOne = param1;
			this.ParameterTwo = param2;
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0000E957 File Offset: 0x0000CB57
		public NotificationMessage()
		{
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0000E960 File Offset: 0x0000CB60
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

		// Token: 0x0600082D RID: 2093 RVA: 0x0000E9C4 File Offset: 0x0000CBC4
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

		// Token: 0x0600082E RID: 2094 RVA: 0x0000EA2C File Offset: 0x0000CC2C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Messaging;
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x0000EA30 File Offset: 0x0000CC30
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
