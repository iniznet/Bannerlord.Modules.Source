using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000215 RID: 533
	public class GenericCampaignPanelsGameKeyCategory : GameKeyContext
	{
		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x06001DC6 RID: 7622 RVA: 0x0006B658 File Offset: 0x00069858
		// (set) Token: 0x06001DC7 RID: 7623 RVA: 0x0006B65F File Offset: 0x0006985F
		public static GenericCampaignPanelsGameKeyCategory Current { get; private set; }

		// Token: 0x06001DC8 RID: 7624 RVA: 0x0006B667 File Offset: 0x00069867
		public GenericCampaignPanelsGameKeyCategory(string categoryId = "GenericCampaignPanelsGameKeyCategory")
			: base(categoryId, 108, GameKeyContext.GameKeyContextType.Default)
		{
			GenericCampaignPanelsGameKeyCategory.Current = this;
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DC9 RID: 7625 RVA: 0x0006B68C File Offset: 0x0006988C
		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.LeftShift),
				new Key(InputKey.RightShift)
			};
			base.RegisterHotKey(new HotKey("FiveStackModifier", "GenericCampaignPanelsGameKeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			List<Key> list2 = new List<Key>
			{
				new Key(InputKey.LeftControl),
				new Key(InputKey.RightControl)
			};
			base.RegisterHotKey(new HotKey("EntireStackModifier", "GenericCampaignPanelsGameKeyCategory", list2, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06001DCA RID: 7626 RVA: 0x0006B710 File Offset: 0x00069910
		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(36, "BannerWindow", "GenericCampaignPanelsGameKeyCategory", InputKey.B, GameKeyMainCategories.MenuShortcutCategory), true);
			base.RegisterGameKey(new GameKey(37, "CharacterWindow", "GenericCampaignPanelsGameKeyCategory", InputKey.C, GameKeyMainCategories.MenuShortcutCategory), true);
			base.RegisterGameKey(new GameKey(38, "InventoryWindow", "GenericCampaignPanelsGameKeyCategory", InputKey.I, GameKeyMainCategories.MenuShortcutCategory), true);
			base.RegisterGameKey(new GameKey(39, "EncyclopediaWindow", "GenericCampaignPanelsGameKeyCategory", InputKey.N, InputKey.ControllerLOption, GameKeyMainCategories.MenuShortcutCategory), true);
			base.RegisterGameKey(new GameKey(40, "KingdomWindow", "GenericCampaignPanelsGameKeyCategory", InputKey.K, GameKeyMainCategories.MenuShortcutCategory), true);
			base.RegisterGameKey(new GameKey(41, "ClanWindow", "GenericCampaignPanelsGameKeyCategory", InputKey.L, GameKeyMainCategories.MenuShortcutCategory), true);
			base.RegisterGameKey(new GameKey(42, "QuestsWindow", "GenericCampaignPanelsGameKeyCategory", InputKey.J, GameKeyMainCategories.MenuShortcutCategory), true);
			base.RegisterGameKey(new GameKey(43, "PartyWindow", "GenericCampaignPanelsGameKeyCategory", InputKey.P, GameKeyMainCategories.MenuShortcutCategory), true);
			base.RegisterGameKey(new GameKey(44, "FacegenWindow", "GenericCampaignPanelsGameKeyCategory", InputKey.V, GameKeyMainCategories.MenuShortcutCategory), true);
		}

		// Token: 0x06001DCB RID: 7627 RVA: 0x0006B839 File Offset: 0x00069A39
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x04000A7A RID: 2682
		public const string CategoryId = "GenericCampaignPanelsGameKeyCategory";

		// Token: 0x04000A7B RID: 2683
		public const string FiveStackModifier = "FiveStackModifier";

		// Token: 0x04000A7C RID: 2684
		public const string EntireStackModifier = "EntireStackModifier";

		// Token: 0x04000A7D RID: 2685
		public const int BannerWindow = 36;

		// Token: 0x04000A7E RID: 2686
		public const int CharacterWindow = 37;

		// Token: 0x04000A7F RID: 2687
		public const int InventoryWindow = 38;

		// Token: 0x04000A80 RID: 2688
		public const int EncyclopediaWindow = 39;

		// Token: 0x04000A81 RID: 2689
		public const int PartyWindow = 43;

		// Token: 0x04000A82 RID: 2690
		public const int KingdomWindow = 40;

		// Token: 0x04000A83 RID: 2691
		public const int ClanWindow = 41;

		// Token: 0x04000A84 RID: 2692
		public const int QuestsWindow = 42;

		// Token: 0x04000A85 RID: 2693
		public const int FacegenWindow = 44;
	}
}
