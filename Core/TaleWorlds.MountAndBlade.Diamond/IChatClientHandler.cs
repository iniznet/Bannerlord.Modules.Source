using System;
using TaleWorlds.Diamond.ChatSystem.Library;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000116 RID: 278
	public interface IChatClientHandler
	{
		// Token: 0x0600054C RID: 1356
		void OnChatMessageReceived(Guid roomId, string roomName, string playerName, string textMessage, string textColor, MessageType type);
	}
}
