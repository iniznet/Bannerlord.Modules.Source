using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	public class DefaultSkillEffects
	{
		private static DefaultSkillEffects Instance
		{
			get
			{
				return Campaign.Current.DefaultSkillEffects;
			}
		}

		public static SkillEffect OneHandedSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectOneHandedSpeed;
			}
		}

		public static SkillEffect OneHandedDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectOneHandedDamage;
			}
		}

		public static SkillEffect TwoHandedSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTwoHandedSpeed;
			}
		}

		public static SkillEffect TwoHandedDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTwoHandedDamage;
			}
		}

		public static SkillEffect PolearmSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectPolearmSpeed;
			}
		}

		public static SkillEffect PolearmDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectPolearmDamage;
			}
		}

		public static SkillEffect BowLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectBowLevel;
			}
		}

		public static SkillEffect BowDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectBowDamage;
			}
		}

		public static SkillEffect BowAccuracy
		{
			get
			{
				return DefaultSkillEffects.Instance._effectBowAccuracy;
			}
		}

		public static SkillEffect ThrowingSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectThrowingSpeed;
			}
		}

		public static SkillEffect ThrowingDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectThrowingDamage;
			}
		}

		public static SkillEffect ThrowingAccuracy
		{
			get
			{
				return DefaultSkillEffects.Instance._effectThrowingAccuracy;
			}
		}

		public static SkillEffect CrossbowReloadSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectCrossbowReloadSpeed;
			}
		}

		public static SkillEffect CrossbowAccuracy
		{
			get
			{
				return DefaultSkillEffects.Instance._effectCrossbowAccuracy;
			}
		}

		public static SkillEffect HorseLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHorseLevel;
			}
		}

		public static SkillEffect HorseSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHorseSpeed;
			}
		}

		public static SkillEffect HorseManeuver
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHorseManeuver;
			}
		}

		public static SkillEffect MountedWeaponDamagePenalty
		{
			get
			{
				return DefaultSkillEffects.Instance._effectMountedWeaponDamagePenalty;
			}
		}

		public static SkillEffect MountedWeaponSpeedPenalty
		{
			get
			{
				return DefaultSkillEffects.Instance._effectMountedWeaponSpeedPenalty;
			}
		}

		public static SkillEffect MountedWeaponAccuracyPenalty
		{
			get
			{
				return DefaultSkillEffects.Instance._effectMountedWeaponAccuracyPenalty;
			}
		}

		public static SkillEffect DismountResistance
		{
			get
			{
				return DefaultSkillEffects.Instance._effectDismountResistance;
			}
		}

		public static SkillEffect AthleticsSpeedFactor
		{
			get
			{
				return DefaultSkillEffects.Instance._effectAthleticsSpeedFactor;
			}
		}

		public static SkillEffect AthleticsWeightFactor
		{
			get
			{
				return DefaultSkillEffects.Instance._effectAthleticsWeightFactor;
			}
		}

		public static SkillEffect KnockBackResistance
		{
			get
			{
				return DefaultSkillEffects.Instance._effectKnockBackResistance;
			}
		}

		public static SkillEffect KnockDownResistance
		{
			get
			{
				return DefaultSkillEffects.Instance._effectKnockDownResistance;
			}
		}

		public static SkillEffect SmithingLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectSmithingLevel;
			}
		}

		public static SkillEffect TacticsAdvantage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTacticsAdvantage;
			}
		}

		public static SkillEffect TacticsTroopSacrificeReduction
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTacticsTroopSacrificeReduction;
			}
		}

		public static SkillEffect TrackingRadius
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTrackingRadius;
			}
		}

		public static SkillEffect TrackingLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTrackingLevel;
			}
		}

		public static SkillEffect TrackingSpottingDistance
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTrackingSpottingDistance;
			}
		}

		public static SkillEffect TrackingTrackInformation
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTrackingTrackInformation;
			}
		}

		public static SkillEffect RogueryLootBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectRogueryLootBonus;
			}
		}

		public static SkillEffect CharmRelationBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectCharmRelationBonus;
			}
		}

		public static SkillEffect TradePenaltyReduction
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTradePenaltyReduction;
			}
		}

		public static SkillEffect SurgeonSurvivalBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectSurgeonSurvivalBonus;
			}
		}

		public static SkillEffect SiegeEngineProductionBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectSiegeEngineProductionBonus;
			}
		}

		public static SkillEffect TownProjectBuildingBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTownProjectBuildingBonus;
			}
		}

		public static SkillEffect HealingRateBonusForHeroes
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHealingRateBonusForHeroes;
			}
		}

		public static SkillEffect HealingRateBonusForRegulars
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHealingRateBonusForRegulars;
			}
		}

		public static SkillEffect GovernorHealingRateBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectGovernorHealingRateBonus;
			}
		}

		public static SkillEffect LeadershipMoraleBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectLeadershipMoraleBonus;
			}
		}

		public static SkillEffect LeadershipGarrisonSizeBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectLeadershipGarrisonSizeBonus;
			}
		}

		public static SkillEffect StewardPartySizeBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectStewardPartySizeBonus;
			}
		}

		public static SkillEffect EngineerLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectEngineerLevel;
			}
		}

		public DefaultSkillEffects()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			this._effectOneHandedSpeed = this.Create("OneHandedSpeed");
			this._effectOneHandedDamage = this.Create("OneHandedDamage");
			this._effectTwoHandedSpeed = this.Create("TwoHandedSpeed");
			this._effectTwoHandedDamage = this.Create("TwoHandedDamage");
			this._effectPolearmSpeed = this.Create("PolearmSpeed");
			this._effectPolearmDamage = this.Create("PolearmDamage");
			this._effectBowLevel = this.Create("BowLevel");
			this._effectBowDamage = this.Create("BowDamage");
			this._effectBowAccuracy = this.Create("BowAccuracy");
			this._effectThrowingSpeed = this.Create("ThrowingSpeed");
			this._effectThrowingDamage = this.Create("ThrowingDamage");
			this._effectThrowingAccuracy = this.Create("ThrowingAccuracy");
			this._effectCrossbowReloadSpeed = this.Create("CrossbowReloadSpeed");
			this._effectCrossbowAccuracy = this.Create("CrossbowAccuracy");
			this._effectHorseLevel = this.Create("HorseLevel");
			this._effectHorseSpeed = this.Create("HorseSpeed");
			this._effectHorseManeuver = this.Create("HorseManeuver");
			this._effectMountedWeaponDamagePenalty = this.Create("MountedWeaponDamagePenalty");
			this._effectMountedWeaponSpeedPenalty = this.Create("MountedWeaponSpeedPenalty");
			this._effectMountedWeaponAccuracyPenalty = this.Create("MountedWeaponAccuracyPenalty");
			this._effectDismountResistance = this.Create("DismountResistance");
			this._effectAthleticsSpeedFactor = this.Create("AthleticsSpeedFactor");
			this._effectAthleticsWeightFactor = this.Create("AthleticsWeightFactor");
			this._effectKnockBackResistance = this.Create("KnockBackResistance");
			this._effectKnockDownResistance = this.Create("KnockDownResistance");
			this._effectSmithingLevel = this.Create("SmithingLevel");
			this._effectTacticsAdvantage = this.Create("TacticsAdvantage");
			this._effectTacticsTroopSacrificeReduction = this.Create("TacticsTroopSacrificeReduction");
			this._effectTrackingRadius = this.Create("TrackingRadius");
			this._effectTrackingLevel = this.Create("TrackingLevel");
			this._effectTrackingSpottingDistance = this.Create("TrackingSpottingDistance");
			this._effectTrackingTrackInformation = this.Create("TrackingTrackInformation");
			this._effectRogueryLootBonus = this.Create("RogueryLootBonus");
			this._effectCharmRelationBonus = this.Create("CharmRelationBonus");
			this._effectTradePenaltyReduction = this.Create("TradePenaltyReduction");
			this._effectLeadershipMoraleBonus = this.Create("LeadershipMoraleBonus");
			this._effectLeadershipGarrisonSizeBonus = this.Create("LeadershipGarrisonSizeBonus");
			this._effectSurgeonSurvivalBonus = this.Create("SurgeonSurvivalBonus");
			this._effectHealingRateBonusForHeroes = this.Create("HealingRateBonusForHeroes");
			this._effectHealingRateBonusForRegulars = this.Create("HealingRateBonusForRegulars");
			this._effectGovernorHealingRateBonus = this.Create("GovernorHealingRateBonus");
			this._effectSiegeEngineProductionBonus = this.Create("SiegeEngineProductionBonus");
			this._effectTownProjectBuildingBonus = this.Create("TownProjectBuildingBonus");
			this._effectStewardPartySizeBonus = this.Create("StewardPartySizeBonus");
			this._effectEngineerLevel = this.Create("EngineerLevel");
			this.InitializeAll();
		}

		private SkillEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<SkillEffect>(new SkillEffect(stringId));
		}

		private void InitializeAll()
		{
			this._effectOneHandedSpeed.Initialize(new TextObject("{=hjxRvb9l}One handed weapon speed: +{a0}%", null), new SkillObject[] { DefaultSkills.OneHanded }, SkillEffect.PerkRole.Personal, 0.07f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectOneHandedDamage.Initialize(new TextObject("{=baUFKAbd}One handed weapon damage: +{a0}%", null), new SkillObject[] { DefaultSkills.OneHanded }, SkillEffect.PerkRole.Personal, 0.15f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectTwoHandedSpeed.Initialize(new TextObject("{=Np94rYMz}Two handed weapon speed: +{a0}%", null), new SkillObject[] { DefaultSkills.TwoHanded }, SkillEffect.PerkRole.Personal, 0.06f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectTwoHandedDamage.Initialize(new TextObject("{=QkbbLb4v}Two handed weapon damage: +{a0}%", null), new SkillObject[] { DefaultSkills.TwoHanded }, SkillEffect.PerkRole.Personal, 0.16f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectPolearmSpeed.Initialize(new TextObject("{=2ATI9qVM}Polearm weapon speed: +{a0}%", null), new SkillObject[] { DefaultSkills.Polearm }, SkillEffect.PerkRole.Personal, 0.06f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectPolearmDamage.Initialize(new TextObject("{=17cIGVQE}Polearm weapon damage: +{a0}%", null), new SkillObject[] { DefaultSkills.Polearm }, SkillEffect.PerkRole.Personal, 0.07f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectBowLevel.Initialize(new TextObject("{=XN7BX0qP}Max usable bow difficulty: {a0}", null), new SkillObject[] { DefaultSkills.Bow }, SkillEffect.PerkRole.Personal, 1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectBowDamage.Initialize(new TextObject("{=RUZHJMQO}Bow Damage: +{a0}%", null), new SkillObject[] { DefaultSkills.Bow }, SkillEffect.PerkRole.Personal, 0.11f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectBowAccuracy.Initialize(new TextObject("{=sQCS90Wq}Bow Accuracy: +{a0}%", null), new SkillObject[] { DefaultSkills.Bow }, SkillEffect.PerkRole.Personal, 0.09f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectThrowingSpeed.Initialize(new TextObject("{=Z0CoeojG}Thrown weapon speed: +{a0}%", null), new SkillObject[] { DefaultSkills.Throwing }, SkillEffect.PerkRole.Personal, 0.07f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectThrowingDamage.Initialize(new TextObject("{=TQMGppEk}Thrown weapon damage: +{a0}%", null), new SkillObject[] { DefaultSkills.Throwing }, SkillEffect.PerkRole.Personal, 0.06f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectThrowingAccuracy.Initialize(new TextObject("{=SfKrjKuO}Thrown weapon accuracy: +{a0}%", null), new SkillObject[] { DefaultSkills.Throwing }, SkillEffect.PerkRole.Personal, 0.06f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectCrossbowReloadSpeed.Initialize(new TextObject("{=W0Zu4iDz}Crossbow reload speed: +{a0}%", null), new SkillObject[] { DefaultSkills.Crossbow }, SkillEffect.PerkRole.Personal, 0.07f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectCrossbowAccuracy.Initialize(new TextObject("{=JwWnpD40}Crossbow accuracy: +{a0}%", null), new SkillObject[] { DefaultSkills.Crossbow }, SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectHorseLevel.Initialize(new TextObject("{=8uBbbwY9}Max mount difficulty: {a0}", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, 1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectHorseSpeed.Initialize(new TextObject("{=Y07OcP1T}Horse speed: +{a0}", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectHorseManeuver.Initialize(new TextObject("{=AahNTeXY}Horse maneuver: +{a0}", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, 0.04f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectMountedWeaponDamagePenalty.Initialize(new TextObject("{=0dbwEczK}Mounted weapon damage penalty: {a0}%", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 20f, 0f);
			this._effectMountedWeaponSpeedPenalty.Initialize(new TextObject("{=oE5etyy0}Mounted weapon speed & reload penalty: {a0}%", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 20f, 0f);
			this._effectMountedWeaponAccuracyPenalty.Initialize(new TextObject("{=*}Mounted weapon accuracy penalty: {a0}%", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, -0.4f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 40f, 0f);
			this._effectDismountResistance.Initialize(new TextObject("{=kbHJVxAo}Dismount resistance: {a0}% of max. hitpoints", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 40f, 0f);
			this._effectAthleticsSpeedFactor.Initialize(new TextObject("{=rgb6vdon}Running speed increased by {a0}%", null), new SkillObject[] { DefaultSkills.Athletics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectAthleticsWeightFactor.Initialize(new TextObject("{=WaUuhxwv}Weight penalty reduced by: {a0}%", null), new SkillObject[] { DefaultSkills.Athletics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectKnockBackResistance.Initialize(new TextObject("{=TyjDHQUv}Knock back resistance: {a0}% of max. hitpoints", null), new SkillObject[] { DefaultSkills.Athletics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 15f, 0f);
			this._effectKnockDownResistance.Initialize(new TextObject("{=tlNZIH3l}Knock down resistance: {a0}% of max. hitpoints", null), new SkillObject[] { DefaultSkills.Athletics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 40f, 0f);
			this._effectSmithingLevel.Initialize(new TextObject("{=ImN8Cfk6}Max difficulty of weapon that can be smithed without penalty: {a0}", null), new SkillObject[] { DefaultSkills.Crafting }, SkillEffect.PerkRole.Personal, 1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectTacticsAdvantage.Initialize(new TextObject("{=XO3SOlZx}Simulation advantage: +{a0}%", null), new SkillObject[] { DefaultSkills.Tactics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectTacticsTroopSacrificeReduction.Initialize(new TextObject("{=VHdyQYKI}Decrease the sacrificed troop number when trying to get away +{a0}%", null), new SkillObject[] { DefaultSkills.Tactics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectTrackingRadius.Initialize(new TextObject("{=kqJipMqc}Track detection radius +{a0}%", null), new SkillObject[] { DefaultSkills.Scouting }, SkillEffect.PerkRole.Scout, 0.1f, SkillEffect.PerkRole.None, 0.05f, SkillEffect.EffectIncrementType.Add, 0f, 0f);
			this._effectTrackingLevel.Initialize(new TextObject("{=aGecGUub}Max track difficulty that can be detected: {a0}", null), new SkillObject[] { DefaultSkills.Scouting }, SkillEffect.PerkRole.Scout, 1f, SkillEffect.PerkRole.None, 1f, SkillEffect.EffectIncrementType.Add, 0f, 0f);
			this._effectTrackingSpottingDistance.Initialize(new TextObject("{=lbrOAvKj}Spotting distance +{a0}%", null), new SkillObject[] { DefaultSkills.Scouting }, SkillEffect.PerkRole.Scout, 0.06f, SkillEffect.PerkRole.None, 0.03f, SkillEffect.EffectIncrementType.Add, 0f, 0f);
			this._effectTrackingTrackInformation.Initialize(new TextObject("{=uNls3bOP}Track information level: {a0}", null), new SkillObject[] { DefaultSkills.Scouting }, SkillEffect.PerkRole.Scout, 0.04f, SkillEffect.PerkRole.None, 0.03f, SkillEffect.EffectIncrementType.Add, 0f, 0f);
			this._effectRogueryLootBonus.Initialize(new TextObject("{=bN3bLDb2}Battle Loot +{a0}%", null), new SkillObject[] { DefaultSkills.Roguery }, SkillEffect.PerkRole.PartyLeader, 0.25f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectCharmRelationBonus.Initialize(new TextObject("{=c5dsio8Q}Relation increase with NPCs +{a0}%", null), new SkillObject[] { DefaultSkills.Charm }, SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectTradePenaltyReduction.Initialize(new TextObject("{=uq7JwT1Z}Trade penalty Reduction +{a0}%", null), new SkillObject[] { DefaultSkills.Trade }, SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectLeadershipMoraleBonus.Initialize(new TextObject("{=n3bFiuVu}Increase morale of the parties under your command +{a0}", null), new SkillObject[] { DefaultSkills.Leadership }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 0f, 0f);
			this._effectLeadershipGarrisonSizeBonus.Initialize(new TextObject("{=cSt26auo}Increase garrison size by +{a0}", null), new SkillObject[] { DefaultSkills.Leadership }, SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 0f, 0f);
			this._effectSurgeonSurvivalBonus.Initialize(new TextObject("{=w4BzNJYl}Casualty survival chance +{a0}%", null), new SkillObject[] { DefaultSkills.Medicine }, SkillEffect.PerkRole.Surgeon, 0.01f, SkillEffect.PerkRole.None, 0.01f, SkillEffect.EffectIncrementType.Add, 0f, 0f);
			this._effectHealingRateBonusForHeroes.Initialize(new TextObject("{=fUvs4g40}Healing rate increase for heroes +{a0}%", null), new SkillObject[] { DefaultSkills.Medicine }, SkillEffect.PerkRole.Surgeon, 0.5f, SkillEffect.PerkRole.None, 0.05f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectHealingRateBonusForRegulars.Initialize(new TextObject("{=A310vHqJ}Healing rate increase for troops +{a0}%", null), new SkillObject[] { DefaultSkills.Medicine }, SkillEffect.PerkRole.Surgeon, 1f, SkillEffect.PerkRole.None, 0.05f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectGovernorHealingRateBonus.Initialize(new TextObject("{=6mQGst9s}Healing rate increase +{a0}%", null), new SkillObject[] { DefaultSkills.Medicine }, SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectSiegeEngineProductionBonus.Initialize(new TextObject("{=spbYlf0y}Faster siege engine production +{a0}%", null), new SkillObject[] { DefaultSkills.Engineering }, SkillEffect.PerkRole.Engineer, 0.1f, SkillEffect.PerkRole.None, 0.05f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectTownProjectBuildingBonus.Initialize(new TextObject("{=2paRqO8u}Faster building production +{a0}%", null), new SkillObject[] { DefaultSkills.Engineering }, SkillEffect.PerkRole.Governor, 0.25f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectStewardPartySizeBonus.Initialize(new TextObject("{=jNDUXetG}Increase party size by +{a0}", null), new SkillObject[] { DefaultSkills.Steward }, SkillEffect.PerkRole.Quartermaster, 0.25f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 0f, 0f);
			this._effectEngineerLevel.Initialize(new TextObject("{=aQduWCrg}Max difficulty of siege engine that can be built: {a0}", null), new SkillObject[] { DefaultSkills.Engineering }, SkillEffect.PerkRole.Engineer, 1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 0f, 0f);
		}

		private SkillEffect _effectOneHandedSpeed;

		private SkillEffect _effectOneHandedDamage;

		private SkillEffect _effectTwoHandedSpeed;

		private SkillEffect _effectTwoHandedDamage;

		private SkillEffect _effectPolearmSpeed;

		private SkillEffect _effectPolearmDamage;

		private SkillEffect _effectBowLevel;

		private SkillEffect _effectBowDamage;

		private SkillEffect _effectBowAccuracy;

		private SkillEffect _effectThrowingSpeed;

		private SkillEffect _effectThrowingDamage;

		private SkillEffect _effectThrowingAccuracy;

		private SkillEffect _effectCrossbowReloadSpeed;

		private SkillEffect _effectCrossbowAccuracy;

		private SkillEffect _effectHorseLevel;

		private SkillEffect _effectHorseSpeed;

		private SkillEffect _effectHorseManeuver;

		private SkillEffect _effectMountedWeaponDamagePenalty;

		private SkillEffect _effectMountedWeaponSpeedPenalty;

		private SkillEffect _effectMountedWeaponAccuracyPenalty;

		private SkillEffect _effectDismountResistance;

		private SkillEffect _effectAthleticsSpeedFactor;

		private SkillEffect _effectAthleticsWeightFactor;

		private SkillEffect _effectKnockBackResistance;

		private SkillEffect _effectKnockDownResistance;

		private SkillEffect _effectSmithingLevel;

		private SkillEffect _effectTacticsAdvantage;

		private SkillEffect _effectTacticsTroopSacrificeReduction;

		private SkillEffect _effectTrackingLevel;

		private SkillEffect _effectTrackingRadius;

		private SkillEffect _effectTrackingSpottingDistance;

		private SkillEffect _effectTrackingTrackInformation;

		private SkillEffect _effectRogueryLootBonus;

		private SkillEffect _effectCharmRelationBonus;

		private SkillEffect _effectTradePenaltyReduction;

		private SkillEffect _effectSurgeonSurvivalBonus;

		private SkillEffect _effectSiegeEngineProductionBonus;

		private SkillEffect _effectTownProjectBuildingBonus;

		private SkillEffect _effectHealingRateBonusForHeroes;

		private SkillEffect _effectHealingRateBonusForRegulars;

		private SkillEffect _effectGovernorHealingRateBonus;

		private SkillEffect _effectLeadershipMoraleBonus;

		private SkillEffect _effectLeadershipGarrisonSizeBonus;

		private SkillEffect _effectStewardPartySizeBonus;

		private SkillEffect _effectEngineerLevel;
	}
}
