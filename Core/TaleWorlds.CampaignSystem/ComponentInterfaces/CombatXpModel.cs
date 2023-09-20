using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200017F RID: 383
	public abstract class CombatXpModel : GameModel
	{
		// Token: 0x06001952 RID: 6482
		public abstract SkillObject GetSkillForWeapon(WeaponComponentData weapon, bool isSiegeEngineHit);

		// Token: 0x06001953 RID: 6483
		public abstract void GetXpFromHit(CharacterObject attackerTroop, CharacterObject captain, CharacterObject attackedTroop, PartyBase attackerParty, int damage, bool isFatal, CombatXpModel.MissionTypeEnum missionType, out int xpAmount);

		// Token: 0x06001954 RID: 6484
		public abstract float GetXpMultiplierFromShotDifficulty(float shotDifficulty);

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06001955 RID: 6485
		public abstract float CaptainRadius { get; }

		// Token: 0x0200055A RID: 1370
		public enum MissionTypeEnum
		{
			// Token: 0x04001691 RID: 5777
			Battle,
			// Token: 0x04001692 RID: 5778
			PracticeFight,
			// Token: 0x04001693 RID: 5779
			Tournament,
			// Token: 0x04001694 RID: 5780
			SimulationBattle,
			// Token: 0x04001695 RID: 5781
			NoXp
		}
	}
}
