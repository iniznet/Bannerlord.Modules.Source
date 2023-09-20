using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000038 RID: 56
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ServerMessage : GameNetworkMessage
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000421E File Offset: 0x0000241E
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x00004226 File Offset: 0x00002426
		public string Message { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001CA RID: 458 RVA: 0x0000422F File Offset: 0x0000242F
		// (set) Token: 0x060001CB RID: 459 RVA: 0x00004237 File Offset: 0x00002437
		public bool IsMessageTextId { get; private set; }

		// Token: 0x060001CC RID: 460 RVA: 0x00004240 File Offset: 0x00002440
		public ServerMessage(string message, bool isMessageTextId = false)
		{
			this.Message = message;
			this.IsMessageTextId = isMessageTextId;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00004256 File Offset: 0x00002456
		public ServerMessage()
		{
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000425E File Offset: 0x0000245E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.Message);
			GameNetworkMessage.WriteBoolToPacket(this.IsMessageTextId);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00004278 File Offset: 0x00002478
		protected override bool OnRead()
		{
			bool flag = true;
			this.Message = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.IsMessageTextId = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x000042A2 File Offset: 0x000024A2
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Messaging;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x000042A6 File Offset: 0x000024A6
		protected override string OnGetLogFormat()
		{
			return "Message from server: " + this.Message;
		}
	}
}
