using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200007C RID: 124
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChannelMessage : Message
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060001DB RID: 475 RVA: 0x0000352C File Offset: 0x0000172C
		// (set) Token: 0x060001DC RID: 476 RVA: 0x00003534 File Offset: 0x00001734
		public ChatChannelType Channel { get; private set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060001DD RID: 477 RVA: 0x0000353D File Offset: 0x0000173D
		// (set) Token: 0x060001DE RID: 478 RVA: 0x00003545 File Offset: 0x00001745
		public string Message { get; private set; }

		// Token: 0x060001DF RID: 479 RVA: 0x0000354E File Offset: 0x0000174E
		public ChannelMessage(ChatChannelType channel, string message)
		{
			this.Channel = channel;
			this.Message = message;
		}
	}
}
