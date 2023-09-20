using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200007A RID: 122
	public interface IBattleObserver
	{
		// Token: 0x06000789 RID: 1929
		void TroopNumberChanged(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject character, int number = 0, int numberKilled = 0, int numberWounded = 0, int numberRouted = 0, int killCount = 0, int numberReadyToUpgrade = 0);

		// Token: 0x0600078A RID: 1930
		void TroopSideChanged(BattleSideEnum prevSide, BattleSideEnum newSide, IBattleCombatant battleCombatant, BasicCharacterObject character);

		// Token: 0x0600078B RID: 1931
		void HeroSkillIncreased(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject heroCharacter, SkillObject skill);

		// Token: 0x0600078C RID: 1932
		void BattleResultsReady();
	}
}
