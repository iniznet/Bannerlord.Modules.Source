using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000AA RID: 170
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class LeaveChannelMessage : Message
	{
		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000268 RID: 616 RVA: 0x00003B22 File Offset: 0x00001D22
		// (set) Token: 0x06000269 RID: 617 RVA: 0x00003B2A File Offset: 0x00001D2A
		public ChatChannelType Channel { get; private set; }

		// Token: 0x0600026A RID: 618 RVA: 0x00003B33 File Offset: 0x00001D33
		public LeaveChannelMessage(ChatChannelType channel)
		{
			this.Channel = channel;
		}
	}
}
