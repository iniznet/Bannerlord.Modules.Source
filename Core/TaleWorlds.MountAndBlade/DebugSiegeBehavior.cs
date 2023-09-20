using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public static class DebugSiegeBehavior
	{
		public static void SiegeDebug(UsableMachine usableMachine)
		{
			if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyAimAtRam"))
			{
				DebugSiegeBehavior.DebugDefendState = DebugSiegeBehavior.DebugStateDefender.DebugDefendersToRam;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyAimAtSt"))
			{
				DebugSiegeBehavior.DebugDefendState = DebugSiegeBehavior.DebugStateDefender.DebugDefendersToTower;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyAimAtBallistas2"))
			{
				DebugSiegeBehavior.DebugDefendState = DebugSiegeBehavior.DebugStateDefender.DebugDefendersToBallistae;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyAimAtMangonels2"))
			{
				DebugSiegeBehavior.DebugDefendState = DebugSiegeBehavior.DebugStateDefender.DebugDefendersToMangonels;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyAimAtNone2"))
			{
				DebugSiegeBehavior.DebugDefendState = DebugSiegeBehavior.DebugStateDefender.None;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyAimAtBallistas"))
			{
				DebugSiegeBehavior.DebugAttackState = DebugSiegeBehavior.DebugStateAttacker.DebugAttackersToBallistae;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyAimAtMangonels"))
			{
				DebugSiegeBehavior.DebugAttackState = DebugSiegeBehavior.DebugStateAttacker.DebugAttackersToMangonels;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyAimAtBattlements"))
			{
				DebugSiegeBehavior.DebugAttackState = DebugSiegeBehavior.DebugStateAttacker.DebugAttackersToBattlements;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyAimAtNone"))
			{
				DebugSiegeBehavior.DebugAttackState = DebugSiegeBehavior.DebugStateAttacker.None;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyTargetDebugActive"))
			{
				DebugSiegeBehavior.ToggleTargetDebug = true;
			}
			else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviorHotkeyTargetDebugDisactive"))
			{
				DebugSiegeBehavior.ToggleTargetDebug = false;
			}
			bool toggleTargetDebug = DebugSiegeBehavior.ToggleTargetDebug;
		}

		public static bool ToggleTargetDebug;

		public static DebugSiegeBehavior.DebugStateAttacker DebugAttackState;

		public static DebugSiegeBehavior.DebugStateDefender DebugDefendState;

		public enum DebugStateAttacker
		{
			None,
			DebugAttackersToBallistae,
			DebugAttackersToMangonels,
			DebugAttackersToBattlements
		}

		public enum DebugStateDefender
		{
			None,
			DebugDefendersToBallistae,
			DebugDefendersToMangonels,
			DebugDefendersToRam,
			DebugDefendersToTower
		}
	}
}
