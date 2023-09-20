using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000219 RID: 537
	public class MapHotKeyCategory : GameKeyContext
	{
		// Token: 0x06001DDC RID: 7644 RVA: 0x0006BD6B File Offset: 0x00069F6B
		public MapHotKeyCategory()
			: base("MapHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x0006BD90 File Offset: 0x00069F90
		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.LeftMouseButton),
				new Key(InputKey.ControllerRDown)
			};
			base.RegisterHotKey(new HotKey("MapClick", "MapHotKeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			List<Key> list2 = new List<Key>
			{
				new Key(InputKey.LeftAlt),
				new Key(InputKey.ControllerLBumper)
			};
			base.RegisterHotKey(new HotKey("MapFollowModifier", "MapHotKeyCategory", list2, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			List<Key> list3 = new List<Key>
			{
				new Key(InputKey.ControllerRRight)
			};
			base.RegisterHotKey(new HotKey("MapChangeCursorMode", "MapHotKeyCategory", list3, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06001DDE RID: 7646 RVA: 0x0006BE48 File Offset: 0x0006A048
		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(49, "PartyMoveUp", "MapHotKeyCategory", InputKey.Up, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(50, "PartyMoveDown", "MapHotKeyCategory", InputKey.Down, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(51, "PartyMoveRight", "MapHotKeyCategory", InputKey.Right, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(52, "PartyMoveLeft", "MapHotKeyCategory", InputKey.Left, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(53, "QuickSave", "MapHotKeyCategory", InputKey.F5, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(54, "MapFastMove", "MapHotKeyCategory", InputKey.LeftShift, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(55, "MapZoomIn", "MapHotKeyCategory", InputKey.MouseScrollUp, InputKey.ControllerRTrigger, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(56, "MapZoomOut", "MapHotKeyCategory", InputKey.MouseScrollDown, InputKey.ControllerLTrigger, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(57, "MapRotateLeft", "MapHotKeyCategory", InputKey.Q, InputKey.Invalid, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(58, "MapRotateRight", "MapHotKeyCategory", InputKey.E, InputKey.Invalid, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(59, "MapTimeStop", "MapHotKeyCategory", InputKey.D1, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(60, "MapTimeNormal", "MapHotKeyCategory", InputKey.D2, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(61, "MapTimeFastForward", "MapHotKeyCategory", InputKey.D3, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(62, "MapTimeTogglePause", "MapHotKeyCategory", InputKey.Space, InputKey.ControllerRLeft, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(63, "MapCameraFollowMode", "MapHotKeyCategory", InputKey.Invalid, InputKey.ControllerLThumb, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(64, "MapToggleFastForward", "MapHotKeyCategory", InputKey.Invalid, InputKey.ControllerRBumper, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(65, "MapTrackSettlement", "MapHotKeyCategory", InputKey.Invalid, InputKey.ControllerRThumb, GameKeyMainCategories.CampaignMapCategory), true);
			base.RegisterGameKey(new GameKey(66, "MapGoToEncylopedia", "MapHotKeyCategory", InputKey.Invalid, InputKey.ControllerLOption, GameKeyMainCategories.CampaignMapCategory), true);
		}

		// Token: 0x06001DDF RID: 7647 RVA: 0x0006C0B4 File Offset: 0x0006A2B4
		private void RegisterGameAxisKeys()
		{
			GameKey gameKey = new GameKey(45, "MapMoveUp", "MapHotKeyCategory", InputKey.W, GameKeyMainCategories.CampaignMapCategory);
			GameKey gameKey2 = new GameKey(46, "MapMoveDown", "MapHotKeyCategory", InputKey.S, GameKeyMainCategories.CampaignMapCategory);
			GameKey gameKey3 = new GameKey(47, "MapMoveRight", "MapHotKeyCategory", InputKey.D, GameKeyMainCategories.CampaignMapCategory);
			GameKey gameKey4 = new GameKey(48, "MapMoveLeft", "MapHotKeyCategory", InputKey.A, GameKeyMainCategories.CampaignMapCategory);
			base.RegisterGameKey(gameKey, true);
			base.RegisterGameKey(gameKey2, true);
			base.RegisterGameKey(gameKey4, true);
			base.RegisterGameKey(gameKey3, true);
			base.RegisterGameAxisKey(new GameAxisKey("MovementAxisX", InputKey.ControllerLStick, gameKey3, gameKey4, GameAxisKey.AxisType.X), true);
			base.RegisterGameAxisKey(new GameAxisKey("MovementAxisY", InputKey.ControllerLStick, gameKey, gameKey2, GameAxisKey.AxisType.Y), true);
		}

		// Token: 0x04000AA4 RID: 2724
		public const string CategoryId = "MapHotKeyCategory";

		// Token: 0x04000AA5 RID: 2725
		public const int QuickSave = 53;

		// Token: 0x04000AA6 RID: 2726
		public const int PartyMoveUp = 49;

		// Token: 0x04000AA7 RID: 2727
		public const int PartyMoveLeft = 52;

		// Token: 0x04000AA8 RID: 2728
		public const int PartyMoveDown = 50;

		// Token: 0x04000AA9 RID: 2729
		public const int PartyMoveRight = 51;

		// Token: 0x04000AAA RID: 2730
		public const int MapMoveUp = 45;

		// Token: 0x04000AAB RID: 2731
		public const int MapMoveDown = 46;

		// Token: 0x04000AAC RID: 2732
		public const int MapMoveLeft = 48;

		// Token: 0x04000AAD RID: 2733
		public const int MapMoveRight = 47;

		// Token: 0x04000AAE RID: 2734
		public const string MovementAxisX = "MovementAxisX";

		// Token: 0x04000AAF RID: 2735
		public const string MovementAxisY = "MovementAxisY";

		// Token: 0x04000AB0 RID: 2736
		public const int MapFastMove = 54;

		// Token: 0x04000AB1 RID: 2737
		public const int MapZoomIn = 55;

		// Token: 0x04000AB2 RID: 2738
		public const int MapZoomOut = 56;

		// Token: 0x04000AB3 RID: 2739
		public const int MapRotateLeft = 57;

		// Token: 0x04000AB4 RID: 2740
		public const int MapRotateRight = 58;

		// Token: 0x04000AB5 RID: 2741
		public const int MapCameraFollowMode = 63;

		// Token: 0x04000AB6 RID: 2742
		public const int MapToggleFastForward = 64;

		// Token: 0x04000AB7 RID: 2743
		public const int MapTrackSettlement = 65;

		// Token: 0x04000AB8 RID: 2744
		public const int MapGoToEncylopedia = 66;

		// Token: 0x04000AB9 RID: 2745
		public const string MapClick = "MapClick";

		// Token: 0x04000ABA RID: 2746
		public const string MapFollowModifier = "MapFollowModifier";

		// Token: 0x04000ABB RID: 2747
		public const string MapChangeCursorMode = "MapChangeCursorMode";

		// Token: 0x04000ABC RID: 2748
		public const int MapTimeStop = 59;

		// Token: 0x04000ABD RID: 2749
		public const int MapTimeNormal = 60;

		// Token: 0x04000ABE RID: 2750
		public const int MapTimeFastForward = 61;

		// Token: 0x04000ABF RID: 2751
		public const int MapTimeTogglePause = 62;
	}
}
