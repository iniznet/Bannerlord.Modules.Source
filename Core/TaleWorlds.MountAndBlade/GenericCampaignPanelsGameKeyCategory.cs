using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class GenericCampaignPanelsGameKeyCategory : GameKeyContext
	{
		public static GenericCampaignPanelsGameKeyCategory Current { get; private set; }

		public GenericCampaignPanelsGameKeyCategory(string categoryId = "GenericCampaignPanelsGameKeyCategory")
			: base(categoryId, 108, GameKeyContext.GameKeyContextType.Default)
		{
			GenericCampaignPanelsGameKeyCategory.Current = this;
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

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

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "GenericCampaignPanelsGameKeyCategory";

		public const string FiveStackModifier = "FiveStackModifier";

		public const string EntireStackModifier = "EntireStackModifier";

		public const int BannerWindow = 36;

		public const int CharacterWindow = 37;

		public const int InventoryWindow = 38;

		public const int EncyclopediaWindow = 39;

		public const int PartyWindow = 43;

		public const int KingdomWindow = 40;

		public const int ClanWindow = 41;

		public const int QuestsWindow = 42;

		public const int FacegenWindow = 44;
	}
}
