using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200020D RID: 525
	public class ChatLogHotKeyCategory : GameKeyContext
	{
		// Token: 0x06001DAE RID: 7598 RVA: 0x0006AB82 File Offset: 0x00068D82
		public ChatLogHotKeyCategory()
			: base("ChatLogHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x0006ABA4 File Offset: 0x00068DA4
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

		// Token: 0x06001DB0 RID: 7600 RVA: 0x0006AC5C File Offset: 0x00068E5C
		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(6, "InitiateAllChat", "ChatLogHotKeyCategory", InputKey.T, GameKeyMainCategories.ChatCategory), true);
			base.RegisterGameKey(new GameKey(7, "InitiateTeamChat", "ChatLogHotKeyCategory", InputKey.Y, GameKeyMainCategories.ChatCategory), true);
			base.RegisterGameKey(new GameKey(8, "FinalizeChat", "ChatLogHotKeyCategory", InputKey.Enter, InputKey.ControllerLOption, GameKeyMainCategories.ChatCategory), true);
		}

		// Token: 0x06001DB1 RID: 7601 RVA: 0x0006ACC8 File Offset: 0x00068EC8
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x040009BD RID: 2493
		public const string CategoryId = "ChatLogHotKeyCategory";

		// Token: 0x040009BE RID: 2494
		public const int InitiateAllChat = 6;

		// Token: 0x040009BF RID: 2495
		public const int InitiateTeamChat = 7;

		// Token: 0x040009C0 RID: 2496
		public const int FinalizeChat = 8;

		// Token: 0x040009C1 RID: 2497
		public const string CycleChatTypes = "CycleChatTypes";

		// Token: 0x040009C2 RID: 2498
		public const string FinalizeChatAlternative = "FinalizeChatAlternative";

		// Token: 0x040009C3 RID: 2499
		public const string SendMessage = "SendMessage";
	}
}
