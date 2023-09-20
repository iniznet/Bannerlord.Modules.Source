using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class PartyHotKeyCategory : GameKeyContext
	{
		public PartyHotKeyCategory()
			: base("PartyHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.Q),
				new Key(InputKey.ControllerLTrigger)
			};
			List<Key> list2 = new List<Key>
			{
				new Key(InputKey.E),
				new Key(InputKey.ControllerRTrigger)
			};
			List<Key> list3 = new List<Key>
			{
				new Key(InputKey.A),
				new Key(InputKey.ControllerLBumper)
			};
			List<Key> list4 = new List<Key>
			{
				new Key(InputKey.D),
				new Key(InputKey.ControllerRBumper)
			};
			List<Key> list5 = new List<Key>
			{
				new Key(InputKey.ControllerLBumper)
			};
			List<Key> list6 = new List<Key>
			{
				new Key(InputKey.ControllerRBumper)
			};
			List<Key> list7 = new List<Key>
			{
				new Key(InputKey.ControllerLThumb)
			};
			List<Key> list8 = new List<Key>
			{
				new Key(InputKey.ControllerRThumb)
			};
			base.RegisterHotKey(new HotKey("TakeAllTroops", "PartyHotKeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("GiveAllTroops", "PartyHotKeyCategory", list2, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("TakeAllPrisoners", "PartyHotKeyCategory", list3, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("GiveAllPrisoners", "PartyHotKeyCategory", list4, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("OpenUpgradePopup", "PartyHotKeyCategory", list7, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("OpenRecruitPopup", "PartyHotKeyCategory", list8, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("PopupItemPrimaryAction", "PartyHotKeyCategory", list5, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("PopupItemSecondaryAction", "PartyHotKeyCategory", list6, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		private void RegisterGameKeys()
		{
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "PartyHotKeyCategory";

		public const string TakeAllTroops = "TakeAllTroops";

		public const string GiveAllTroops = "GiveAllTroops";

		public const string TakeAllPrisoners = "TakeAllPrisoners";

		public const string GiveAllPrisoners = "GiveAllPrisoners";

		public const string PopupItemPrimaryAction = "PopupItemPrimaryAction";

		public const string PopupItemSecondaryAction = "PopupItemSecondaryAction";

		public const string OpenUpgradePopup = "OpenUpgradePopup";

		public const string OpenRecruitPopup = "OpenRecruitPopup";
	}
}
