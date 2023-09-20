using System;
using System.Diagnostics;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public static class MessageManager
	{
		public static void DisplayMessage(string message)
		{
			MBAPI.IMBMessageManager.DisplayMessage(message);
		}

		public static void DisplayMessage(string message, uint color)
		{
			MBAPI.IMBMessageManager.DisplayMessageWithColor(message, color);
		}

		[Conditional("DEBUG")]
		public static void DisplayDebugMessage(string message)
		{
			if (message.Length > 4 && message.Substring(0, 4).Equals("[DEBUG]"))
			{
				message = message.Substring(4);
			}
			MBAPI.IMBMessageManager.DisplayMessageWithColor("[DEBUG]: " + message, 4294936712U);
		}

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

		public static void EraseMessageLines()
		{
			MBAPI.IMBWindowManager.EraseMessageLines();
		}

		public static void SetMessageManager(MessageManagerBase messageManager)
		{
			MBAPI.IMBMessageManager.SetMessageManager(messageManager);
		}
	}
}
