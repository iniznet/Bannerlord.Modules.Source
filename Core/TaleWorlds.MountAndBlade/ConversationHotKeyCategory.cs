using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public class ConversationHotKeyCategory : GameKeyContext
	{
		public ConversationHotKeyCategory()
			: base("ConversationHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

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

		private void RegisterGameKeys()
		{
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "ConversationHotKeyCategory";

		public const string ContinueKey = "ContinueKey";

		public const string ContinueClick = "ContinueClick";
	}
}
