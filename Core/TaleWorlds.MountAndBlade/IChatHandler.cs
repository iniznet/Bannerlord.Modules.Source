using System;
using TaleWorlds.MountAndBlade.Diamond;

// Token: 0x02000004 RID: 4
public interface IChatHandler
{
	// Token: 0x06000003 RID: 3
	void ReceiveChatMessage(ChatChannelType channel, string sender, string message);
}
