using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public class CombatHotKeyCategory : GameKeyContext
	{
		public CombatHotKeyCategory()
			: base("CombatHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("DeploymentCameraIsActive", "CombatHotKeyCategory", InputKey.MiddleMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("ToggleZoom", "CombatHotKeyCategory", InputKey.ControllerRThumb, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("ControllerEquipDropWeapon1", "CombatHotKeyCategory", InputKey.ControllerRRight, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("ControllerEquipDropWeapon2", "CombatHotKeyCategory", InputKey.ControllerRUp, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("ControllerEquipDropWeapon3", "CombatHotKeyCategory", InputKey.ControllerRLeft, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("ControllerEquipDropWeapon4", "CombatHotKeyCategory", InputKey.ControllerRDown, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("CheerBarkSelectFirstCategory", "CombatHotKeyCategory", new List<Key>
			{
				new Key(InputKey.LeftMouseButton),
				new Key(InputKey.ControllerRLeft)
			}, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("CheerBarkSelectSecondCategory", "CombatHotKeyCategory", new List<Key>
			{
				new Key(InputKey.RightMouseButton),
				new Key(InputKey.ControllerRRight)
			}, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("CheerBarkCloseMenu", "CombatHotKeyCategory", InputKey.ControllerRThumb, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("CheerBarkItem1", "CombatHotKeyCategory", InputKey.ControllerRUp, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("CheerBarkItem2", "CombatHotKeyCategory", InputKey.ControllerRRight, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("CheerBarkItem3", "CombatHotKeyCategory", InputKey.ControllerRDown, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("CheerBarkItem4", "CombatHotKeyCategory", InputKey.ControllerRLeft, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("ForfeitSpawn", "CombatHotKeyCategory", new List<Key>
			{
				new Key(InputKey.X),
				new Key(InputKey.ControllerRLeft)
			}, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(9, "Attack", "CombatHotKeyCategory", InputKey.LeftMouseButton, InputKey.ControllerRTrigger, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(10, "Defend", "CombatHotKeyCategory", InputKey.RightMouseButton, InputKey.ControllerLTrigger, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(11, "EquipPrimaryWeapon", "CombatHotKeyCategory", InputKey.MouseScrollUp, InputKey.Invalid, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(12, "EquipSecondaryWeapon", "CombatHotKeyCategory", InputKey.MouseScrollDown, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(13, "Action", "CombatHotKeyCategory", InputKey.F, InputKey.ControllerRUp, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(14, "Jump", "CombatHotKeyCategory", InputKey.Space, InputKey.ControllerRDown, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(15, "Crouch", "CombatHotKeyCategory", InputKey.Z, InputKey.ControllerLDown, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(16, "Kick", "CombatHotKeyCategory", InputKey.E, InputKey.ControllerRLeft, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(17, "ToggleWeaponMode", "CombatHotKeyCategory", InputKey.X, InputKey.Invalid, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(18, "EquipWeapon1", "CombatHotKeyCategory", InputKey.Numpad1, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(19, "EquipWeapon2", "CombatHotKeyCategory", InputKey.Numpad2, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(20, "EquipWeapon3", "CombatHotKeyCategory", InputKey.Numpad3, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(21, "EquipWeapon4", "CombatHotKeyCategory", InputKey.Numpad4, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(22, "DropWeapon", "CombatHotKeyCategory", InputKey.G, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(23, "SheathWeapon", "CombatHotKeyCategory", InputKey.BackSlash, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(24, "Zoom", "CombatHotKeyCategory", InputKey.LeftShift, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(25, "ViewCharacter", "CombatHotKeyCategory", InputKey.Tilde, InputKey.ControllerLLeft, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(26, "LockTarget", "CombatHotKeyCategory", InputKey.MiddleMouseButton, InputKey.ControllerRThumb, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(27, "CameraToggle", "CombatHotKeyCategory", InputKey.R, InputKey.ControllerLThumb, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(28, "MissionScreenHotkeyCameraZoomIn", "CombatHotKeyCategory", InputKey.NumpadPlus, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(29, "MissionScreenHotkeyCameraZoomOut", "CombatHotKeyCategory", InputKey.NumpadMinus, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(30, "ToggleWalkMode", "CombatHotKeyCategory", InputKey.CapsLock, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(31, "Cheer", "CombatHotKeyCategory", InputKey.O, InputKey.ControllerLUp, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(33, "PushToTalk", "CombatHotKeyCategory", InputKey.V, InputKey.ControllerLRight, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(34, "EquipmentSwitch", "CombatHotKeyCategory", InputKey.U, InputKey.ControllerRBumper, GameKeyMainCategories.ActionCategory), true);
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "CombatHotKeyCategory";

		public const int MissionScreenHotkeyCameraZoomIn = 28;

		public const int MissionScreenHotkeyCameraZoomOut = 29;

		public const int Action = 13;

		public const int Jump = 14;

		public const int Crouch = 15;

		public const int Attack = 9;

		public const int Defend = 10;

		public const int Kick = 16;

		public const int ToggleWeaponMode = 17;

		public const int ToggleWalkMode = 30;

		public const int EquipWeapon1 = 18;

		public const int EquipWeapon2 = 19;

		public const int EquipWeapon3 = 20;

		public const int EquipWeapon4 = 21;

		public const int EquipPrimaryWeapon = 11;

		public const int EquipSecondaryWeapon = 12;

		public const int DropWeapon = 22;

		public const int SheathWeapon = 23;

		public const int Zoom = 24;

		public const int ViewCharacter = 25;

		public const int LockTarget = 26;

		public const int CameraToggle = 27;

		public const int Cheer = 31;

		public const int PushToTalk = 33;

		public const int EquipmentSwitch = 34;

		public const string DeploymentCameraIsActive = "DeploymentCameraIsActive";

		public const string ToggleZoom = "ToggleZoom";

		public const string ControllerEquipDropRRight = "ControllerEquipDropWeapon1";

		public const string ControllerEquipDropRUp = "ControllerEquipDropWeapon2";

		public const string ControllerEquipDropRLeft = "ControllerEquipDropWeapon3";

		public const string ControllerEquipDropRDown = "ControllerEquipDropWeapon4";

		public const string CheerBarkSelectFirstCategory = "CheerBarkSelectFirstCategory";

		public const string CheerBarkSelectSecondCategory = "CheerBarkSelectSecondCategory";

		public const string CheerBarkCloseMenu = "CheerBarkCloseMenu";

		public const string CheerBarkItem1 = "CheerBarkItem1";

		public const string CheerBarkItem2 = "CheerBarkItem2";

		public const string CheerBarkItem3 = "CheerBarkItem3";

		public const string CheerBarkItem4 = "CheerBarkItem4";

		public const string ForfeitSpawn = "ForfeitSpawn";
	}
}
