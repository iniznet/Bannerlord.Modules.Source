using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class CombatXpModel : GameModel
	{
		public abstract SkillObject GetSkillForWeapon(WeaponComponentData weapon, bool isSiegeEngineHit);

		public abstract void GetXpFromHit(CharacterObject attackerTroop, CharacterObject captain, CharacterObject attackedTroop, PartyBase attackerParty, int damage, bool isFatal, CombatXpModel.MissionTypeEnum missionType, out int xpAmount);

		public abstract float GetXpMultiplierFromShotDifficulty(float shotDifficulty);

		public abstract float CaptainRadius { get; }

		public enum MissionTypeEnum
		{
			Battle,
			PracticeFight,
			Tournament,
			SimulationBattle,
			NoXp
		}
	}
}
