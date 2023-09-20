﻿using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class PartyHealingModel : GameModel
	{
		public abstract float GetSurgeryChance(PartyBase party, CharacterObject character);

		public abstract float GetSurvivalChance(PartyBase party, CharacterObject agentCharacter, DamageTypes damageType, bool canDamageKillEvenIfBlunt, PartyBase enemyParty = null);

		public abstract int GetSkillXpFromHealingTroop(PartyBase party);

		public abstract ExplainedNumber GetDailyHealingForRegulars(MobileParty party, bool includeDescriptions = false);

		public abstract ExplainedNumber GetDailyHealingHpForHeroes(MobileParty party, bool includeDescriptions = false);

		public abstract int GetHeroesEffectedHealingAmount(Hero hero, float healingRate);

		public abstract float GetSiegeBombardmentHitSurgeryChance(PartyBase party);

		public abstract int GetBattleEndHealingAmount(MobileParty party, CharacterObject character);
	}
}
