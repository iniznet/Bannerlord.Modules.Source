using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000374 RID: 884
	public static class DebugSiegeBehavior
	{
		// Token: 0x0600304F RID: 12367 RVA: 0x000C6AE8 File Offset: 0x000C4CE8
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

		// Token: 0x04001413 RID: 5139
		public static bool ToggleTargetDebug;

		// Token: 0x04001414 RID: 5140
		public static DebugSiegeBehavior.DebugStateAttacker DebugAttackState;

		// Token: 0x04001415 RID: 5141
		public static DebugSiegeBehavior.DebugStateDefender DebugDefendState;

		// Token: 0x02000685 RID: 1669
		public enum DebugStateAttacker
		{
			// Token: 0x0400213D RID: 8509
			None,
			// Token: 0x0400213E RID: 8510
			DebugAttackersToBallistae,
			// Token: 0x0400213F RID: 8511
			DebugAttackersToMangonels,
			// Token: 0x04002140 RID: 8512
			DebugAttackersToBattlements
		}

		// Token: 0x02000686 RID: 1670
		public enum DebugStateDefender
		{
			// Token: 0x04002142 RID: 8514
			None,
			// Token: 0x04002143 RID: 8515
			DebugDefendersToBallistae,
			// Token: 0x04002144 RID: 8516
			DebugDefendersToMangonels,
			// Token: 0x04002145 RID: 8517
			DebugDefendersToRam,
			// Token: 0x04002146 RID: 8518
			DebugDefendersToTower
		}
	}
}
