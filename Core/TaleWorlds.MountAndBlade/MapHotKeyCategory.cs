﻿using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public class MapHotKeyCategory : GameKeyContext
	{
		public MapHotKeyCategory()
			: base("MapHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

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

		public const string CategoryId = "MapHotKeyCategory";

		public const int QuickSave = 53;

		public const int PartyMoveUp = 49;

		public const int PartyMoveLeft = 52;

		public const int PartyMoveDown = 50;

		public const int PartyMoveRight = 51;

		public const int MapMoveUp = 45;

		public const int MapMoveDown = 46;

		public const int MapMoveLeft = 48;

		public const int MapMoveRight = 47;

		public const string MovementAxisX = "MovementAxisX";

		public const string MovementAxisY = "MovementAxisY";

		public const int MapFastMove = 54;

		public const int MapZoomIn = 55;

		public const int MapZoomOut = 56;

		public const int MapRotateLeft = 57;

		public const int MapRotateRight = 58;

		public const int MapCameraFollowMode = 63;

		public const int MapToggleFastForward = 64;

		public const int MapTrackSettlement = 65;

		public const int MapGoToEncylopedia = 66;

		public const string MapClick = "MapClick";

		public const string MapFollowModifier = "MapFollowModifier";

		public const string MapChangeCursorMode = "MapChangeCursorMode";

		public const int MapTimeStop = 59;

		public const int MapTimeNormal = 60;

		public const int MapTimeFastForward = 61;

		public const int MapTimeTogglePause = 62;
	}
}
