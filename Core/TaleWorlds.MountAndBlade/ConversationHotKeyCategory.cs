using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200020F RID: 527
	public class ConversationHotKeyCategory : GameKeyContext
	{
		// Token: 0x06001DB6 RID: 7606 RVA: 0x0006B24F File Offset: 0x0006944F
		public ConversationHotKeyCategory()
			: base("ConversationHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DB7 RID: 7607 RVA: 0x0006B274 File Offset: 0x00069474
		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.Space),
				new Key(InputKey.Enter),
				new Key(InputKey.NumpadEnter),
				new Key(InputKey.ControllerRDown)
			};
			base.RegisterHotKey(new HotKey("ContinueKey", "ConversationHotKeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("ContinueClick", "ConversationHotKeyCategory", InputKey.LeftMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x0006B2F7 File Offset: 0x000694F7
		private void RegisterGameKeys()
		{
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x0006B2F9 File Offset: 0x000694F9
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x040009EC RID: 2540
		public const string CategoryId = "ConversationHotKeyCategory";

		// Token: 0x040009ED RID: 2541
		public const string ContinueKey = "ContinueKey";

		// Token: 0x040009EE RID: 2542
		public const string ContinueClick = "ContinueClick";
	}
}
