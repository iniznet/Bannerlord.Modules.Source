using System;
using TaleWorlds.MountAndBlade.Diamond;

// Token: 0x02000004 RID: 4
public interface IChatHandler
{
	void ReceiveChatMessage(ChatChannelType channel, string sender, string message);
}
