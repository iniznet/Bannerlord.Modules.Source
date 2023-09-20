using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.Engine.InputSystem
{
	public class CheatsHotKeyCategory : GameKeyContext
	{
		public CheatsHotKeyCategory()
			: base("Cheats", 0, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterCheatHotkey("Pause", InputKey.F11, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyIncreaseCameraSpeed", InputKey.Up, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyDecreaseCameraSpeed", InputKey.Down, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("ResetCameraSpeed", InputKey.MiddleMouseButton, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyIncreaseSlowMotionFactor", InputKey.NumpadPlus, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyDecreaseSlowMotionFactor", InputKey.NumpadMinus, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("EnterSlowMotion", InputKey.F9, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeySwitchAgentToAi", InputKey.F5, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyControlFollowedAgent", InputKey.Numpad5, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyHealYourSelf", InputKey.H, HotKey.Modifiers.Control, HotKey.Modifiers.Shift);
			this.RegisterCheatHotkey("MissionScreenHotkeyHealYourHorse", InputKey.H, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillEnemyAgent", InputKey.F4, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillAllEnemyAgents", InputKey.F4, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillEnemyHorse", InputKey.F4, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillAllEnemyHorses", InputKey.F4, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillFriendlyAgent", InputKey.F2, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillAllFriendlyAgents", InputKey.F2, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillFriendlyHorse", InputKey.F2, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillAllFriendlyHorses", InputKey.F2, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillYourSelf", InputKey.F3, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyKillYourHorse", InputKey.F3, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyGhostCam", InputKey.K, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterCheatHotkey("MissionScreenHotkeyTeleportMainAgent", InputKey.X, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
		}

		private void RegisterCheatHotkey(string id, InputKey hotkeyKey, HotKey.Modifiers modifiers, HotKey.Modifiers negativeModifiers = HotKey.Modifiers.None)
		{
			base.RegisterHotKey(new HotKey(id, "Cheats", hotkeyKey, modifiers, negativeModifiers), true);
		}

		public const string CategoryId = "Cheats";

		public const string MissionScreenHotkeyIncreaseCameraSpeed = "MissionScreenHotkeyIncreaseCameraSpeed";

		public const string MissionScreenHotkeyDecreaseCameraSpeed = "MissionScreenHotkeyDecreaseCameraSpeed";

		public const string ResetCameraSpeed = "ResetCameraSpeed";

		public const string MissionScreenHotkeyIncreaseSlowMotionFactor = "MissionScreenHotkeyIncreaseSlowMotionFactor";

		public const string MissionScreenHotkeyDecreaseSlowMotionFactor = "MissionScreenHotkeyDecreaseSlowMotionFactor";

		public const string EnterSlowMotion = "EnterSlowMotion";

		public const string Pause = "Pause";

		public const string MissionScreenHotkeyHealYourSelf = "MissionScreenHotkeyHealYourSelf";

		public const string MissionScreenHotkeyHealYourHorse = "MissionScreenHotkeyHealYourHorse";

		public const string MissionScreenHotkeyKillEnemyAgent = "MissionScreenHotkeyKillEnemyAgent";

		public const string MissionScreenHotkeyKillAllEnemyAgents = "MissionScreenHotkeyKillAllEnemyAgents";

		public const string MissionScreenHotkeyKillEnemyHorse = "MissionScreenHotkeyKillEnemyHorse";

		public const string MissionScreenHotkeyKillAllEnemyHorses = "MissionScreenHotkeyKillAllEnemyHorses";

		public const string MissionScreenHotkeyKillFriendlyAgent = "MissionScreenHotkeyKillFriendlyAgent";

		public const string MissionScreenHotkeyKillAllFriendlyAgents = "MissionScreenHotkeyKillAllFriendlyAgents";

		public const string MissionScreenHotkeyKillFriendlyHorse = "MissionScreenHotkeyKillFriendlyHorse";

		public const string MissionScreenHotkeyKillAllFriendlyHorses = "MissionScreenHotkeyKillAllFriendlyHorses";

		public const string MissionScreenHotkeyKillYourSelf = "MissionScreenHotkeyKillYourSelf";

		public const string MissionScreenHotkeyKillYourHorse = "MissionScreenHotkeyKillYourHorse";

		public const string MissionScreenHotkeyGhostCam = "MissionScreenHotkeyGhostCam";

		public const string MissionScreenHotkeySwitchAgentToAi = "MissionScreenHotkeySwitchAgentToAi";

		public const string MissionScreenHotkeyControlFollowedAgent = "MissionScreenHotkeyControlFollowedAgent";

		public const string MissionScreenHotkeyTeleportMainAgent = "MissionScreenHotkeyTeleportMainAgent";
	}
}
