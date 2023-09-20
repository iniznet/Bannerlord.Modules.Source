using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200021D RID: 541
	public class PartyHotKeyCategory : GameKeyContext
	{
		// Token: 0x06001DEA RID: 7658 RVA: 0x0006C662 File Offset: 0x0006A862
		public PartyHotKeyCategory()
			: base("PartyHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DEB RID: 7659 RVA: 0x0006C684 File Offset: 0x0006A884
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

		// Token: 0x06001DEC RID: 7660 RVA: 0x0006C845 File Offset: 0x0006AA45
		private void RegisterGameKeys()
		{
		}

		// Token: 0x06001DED RID: 7661 RVA: 0x0006C847 File Offset: 0x0006AA47
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x04000AF3 RID: 2803
		public const string CategoryId = "PartyHotKeyCategory";

		// Token: 0x04000AF4 RID: 2804
		public const string TakeAllTroops = "TakeAllTroops";

		// Token: 0x04000AF5 RID: 2805
		public const string GiveAllTroops = "GiveAllTroops";

		// Token: 0x04000AF6 RID: 2806
		public const string TakeAllPrisoners = "TakeAllPrisoners";

		// Token: 0x04000AF7 RID: 2807
		public const string GiveAllPrisoners = "GiveAllPrisoners";

		// Token: 0x04000AF8 RID: 2808
		public const string PopupItemPrimaryAction = "PopupItemPrimaryAction";

		// Token: 0x04000AF9 RID: 2809
		public const string PopupItemSecondaryAction = "PopupItemSecondaryAction";

		// Token: 0x04000AFA RID: 2810
		public const string OpenUpgradePopup = "OpenUpgradePopup";

		// Token: 0x04000AFB RID: 2811
		public const string OpenRecruitPopup = "OpenRecruitPopup";
	}
}
