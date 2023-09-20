using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000A7 RID: 167
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class JoinChannelMessage : Message
	{
		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600025F RID: 607 RVA: 0x00003AC2 File Offset: 0x00001CC2
		// (set) Token: 0x06000260 RID: 608 RVA: 0x00003ACA File Offset: 0x00001CCA
		public ChatChannelType Channel { get; private set; }

		// Token: 0x06000261 RID: 609 RVA: 0x00003AD3 File Offset: 0x00001CD3
		public JoinChannelMessage(ChatChannelType channel)
		{
			this.Channel = channel;
		}
	}
}
