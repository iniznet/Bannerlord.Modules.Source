using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000009 RID: 9
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ChannelMessageReceivedMessage : Message
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002151 File Offset: 0x00000351
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002159 File Offset: 0x00000359
		public ChatChannelType Channel { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002162 File Offset: 0x00000362
		// (set) Token: 0x0600001D RID: 29 RVA: 0x0000216A File Offset: 0x0000036A
		public string PlayerName { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002173 File Offset: 0x00000373
		// (set) Token: 0x0600001F RID: 31 RVA: 0x0000217B File Offset: 0x0000037B
		public string Message { get; private set; }

		// Token: 0x06000020 RID: 32 RVA: 0x00002184 File Offset: 0x00000384
		public ChannelMessageReceivedMessage(ChatChannelType channel, string playerName, string message)
		{
			this.Channel = channel;
			this.PlayerName = playerName;
			this.Message = message;
		}
	}
}
