using System;
using System.Diagnostics;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001CE RID: 462
	public static class MessageManager
	{
		// Token: 0x06001A2F RID: 6703 RVA: 0x0005C65A File Offset: 0x0005A85A
		public static void DisplayMessage(string message)
		{
			MBAPI.IMBMessageManager.DisplayMessage(message);
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x0005C667 File Offset: 0x0005A867
		public static void DisplayMessage(string message, uint color)
		{
			MBAPI.IMBMessageManager.DisplayMessageWithColor(message, color);
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x0005C678 File Offset: 0x0005A878
		[Conditional("DEBUG")]
		public static void DisplayDebugMessage(string message)
		{
			if (message.Length > 4 && message.Substring(0, 4).Equals("[DEBUG]"))
			{
				message = message.Substring(4);
			}
			MBAPI.IMBMessageManager.DisplayMessageWithColor("[DEBUG]: " + message, 4294936712U);
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x0005C6C8 File Offset: 0x0005A8C8
		public static void DisplayMultilineMessage(string message, uint color)
		{
			if (message.Contains("\n"))
			{
				string[] array = message.Split(new char[] { '\n' });
				for (int i = 0; i < array.Length; i++)
				{
					MBAPI.IMBMessageManager.DisplayMessageWithColor(array[i], color);
				}
				return;
			}
			MBAPI.IMBMessageManager.DisplayMessageWithColor(message, color);
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x0005C71D File Offset: 0x0005A91D
		public static void EraseMessageLines()
		{
			MBAPI.IMBWindowManager.EraseMessageLines();
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x0005C729 File Offset: 0x0005A929
		public static void SetMessageManager(MessageManagerBase messageManager)
		{
			MBAPI.IMBMessageManager.SetMessageManager(messageManager);
		}
	}
}
