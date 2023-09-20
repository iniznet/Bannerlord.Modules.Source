using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public class ChatLogHotKeyCategory : GameKeyContext
	{
		public ChatLogHotKeyCategory()
			: base("ChatLogHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.Tab),
				new Key(InputKey.ControllerRUp)
			};
			List<Key> list2 = new List<Key>
			{
				new Key(InputKey.NumpadEnter),
				new Key(InputKey.ControllerLOption)
			};
			List<Key> list3 = new List<Key>
			{
				new Key(InputKey.ControllerRLeft)
			};
			base.RegisterHotKey(new HotKey("CycleChatTypes", "ChatLogHotKeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("FinalizeChatAlternative", "ChatLogHotKeyCategory", list2, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("SendMessage", "ChatLogHotKeyCategory", list3, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(6, "InitiateAllChat", "ChatLogHotKeyCategory", InputKey.T, GameKeyMainCategories.ChatCategory), true);
			base.RegisterGameKey(new GameKey(7, "InitiateTeamChat", "ChatLogHotKeyCategory", InputKey.Y, GameKeyMainCategories.ChatCategory), true);
			base.RegisterGameKey(new GameKey(8, "FinalizeChat", "ChatLogHotKeyCategory", InputKey.Enter, InputKey.ControllerLOption, GameKeyMainCategories.ChatCategory), true);
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "ChatLogHotKeyCategory";

		public const int InitiateAllChat = 6;

		public const int InitiateTeamChat = 7;

		public const int FinalizeChat = 8;

		public const string CycleChatTypes = "CycleChatTypes";

		public const string FinalizeChatAlternative = "FinalizeChatAlternative";

		public const string SendMessage = "SendMessage";
	}
}
