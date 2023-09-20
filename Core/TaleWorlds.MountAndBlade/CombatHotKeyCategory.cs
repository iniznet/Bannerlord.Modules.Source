using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200020E RID: 526
	public class CombatHotKeyCategory : GameKeyContext
	{
		// Token: 0x06001DB2 RID: 7602 RVA: 0x0006ACCA File Offset: 0x00068ECA
		public CombatHotKeyCategory()
			: base("CombatHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DB3 RID: 7603 RVA: 0x0006ACEC File Offset: 0x00068EEC
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

		// Token: 0x06001DB4 RID: 7604 RVA: 0x0006AEEC File Offset: 0x000690EC
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

		// Token: 0x06001DB5 RID: 7605 RVA: 0x0006B24D File Offset: 0x0006944D
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x040009C4 RID: 2500
		public const string CategoryId = "CombatHotKeyCategory";

		// Token: 0x040009C5 RID: 2501
		public const int MissionScreenHotkeyCameraZoomIn = 28;

		// Token: 0x040009C6 RID: 2502
		public const int MissionScreenHotkeyCameraZoomOut = 29;

		// Token: 0x040009C7 RID: 2503
		public const int Action = 13;

		// Token: 0x040009C8 RID: 2504
		public const int Jump = 14;

		// Token: 0x040009C9 RID: 2505
		public const int Crouch = 15;

		// Token: 0x040009CA RID: 2506
		public const int Attack = 9;

		// Token: 0x040009CB RID: 2507
		public const int Defend = 10;

		// Token: 0x040009CC RID: 2508
		public const int Kick = 16;

		// Token: 0x040009CD RID: 2509
		public const int ToggleWeaponMode = 17;

		// Token: 0x040009CE RID: 2510
		public const int ToggleWalkMode = 30;

		// Token: 0x040009CF RID: 2511
		public const int EquipWeapon1 = 18;

		// Token: 0x040009D0 RID: 2512
		public const int EquipWeapon2 = 19;

		// Token: 0x040009D1 RID: 2513
		public const int EquipWeapon3 = 20;

		// Token: 0x040009D2 RID: 2514
		public const int EquipWeapon4 = 21;

		// Token: 0x040009D3 RID: 2515
		public const int EquipPrimaryWeapon = 11;

		// Token: 0x040009D4 RID: 2516
		public const int EquipSecondaryWeapon = 12;

		// Token: 0x040009D5 RID: 2517
		public const int DropWeapon = 22;

		// Token: 0x040009D6 RID: 2518
		public const int SheathWeapon = 23;

		// Token: 0x040009D7 RID: 2519
		public const int Zoom = 24;

		// Token: 0x040009D8 RID: 2520
		public const int ViewCharacter = 25;

		// Token: 0x040009D9 RID: 2521
		public const int LockTarget = 26;

		// Token: 0x040009DA RID: 2522
		public const int CameraToggle = 27;

		// Token: 0x040009DB RID: 2523
		public const int Cheer = 31;

		// Token: 0x040009DC RID: 2524
		public const int PushToTalk = 33;

		// Token: 0x040009DD RID: 2525
		public const int EquipmentSwitch = 34;

		// Token: 0x040009DE RID: 2526
		public const string DeploymentCameraIsActive = "DeploymentCameraIsActive";

		// Token: 0x040009DF RID: 2527
		public const string ToggleZoom = "ToggleZoom";

		// Token: 0x040009E0 RID: 2528
		public const string ControllerEquipDropRRight = "ControllerEquipDropWeapon1";

		// Token: 0x040009E1 RID: 2529
		public const string ControllerEquipDropRUp = "ControllerEquipDropWeapon2";

		// Token: 0x040009E2 RID: 2530
		public const string ControllerEquipDropRLeft = "ControllerEquipDropWeapon3";

		// Token: 0x040009E3 RID: 2531
		public const string ControllerEquipDropRDown = "ControllerEquipDropWeapon4";

		// Token: 0x040009E4 RID: 2532
		public const string CheerBarkSelectFirstCategory = "CheerBarkSelectFirstCategory";

		// Token: 0x040009E5 RID: 2533
		public const string CheerBarkSelectSecondCategory = "CheerBarkSelectSecondCategory";

		// Token: 0x040009E6 RID: 2534
		public const string CheerBarkCloseMenu = "CheerBarkCloseMenu";

		// Token: 0x040009E7 RID: 2535
		public const string CheerBarkItem1 = "CheerBarkItem1";

		// Token: 0x040009E8 RID: 2536
		public const string CheerBarkItem2 = "CheerBarkItem2";

		// Token: 0x040009E9 RID: 2537
		public const string CheerBarkItem3 = "CheerBarkItem3";

		// Token: 0x040009EA RID: 2538
		public const string CheerBarkItem4 = "CheerBarkItem4";

		// Token: 0x040009EB RID: 2539
		public const string ForfeitSpawn = "ForfeitSpawn";
	}
}
