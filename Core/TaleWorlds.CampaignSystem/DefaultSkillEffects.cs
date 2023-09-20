using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000078 RID: 120
	public class DefaultSkillEffects
	{
		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06000EF1 RID: 3825 RVA: 0x00045BE1 File Offset: 0x00043DE1
		private static DefaultSkillEffects Instance
		{
			get
			{
				return Campaign.Current.DefaultSkillEffects;
			}
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x00045BED File Offset: 0x00043DED
		public static SkillEffect OneHandedSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectOneHandedSpeed;
			}
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06000EF3 RID: 3827 RVA: 0x00045BF9 File Offset: 0x00043DF9
		public static SkillEffect OneHandedDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectOneHandedDamage;
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x00045C05 File Offset: 0x00043E05
		public static SkillEffect TwoHandedSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTwoHandedSpeed;
			}
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x00045C11 File Offset: 0x00043E11
		public static SkillEffect TwoHandedDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTwoHandedDamage;
			}
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x00045C1D File Offset: 0x00043E1D
		public static SkillEffect PolearmSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectPolearmSpeed;
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06000EF7 RID: 3831 RVA: 0x00045C29 File Offset: 0x00043E29
		public static SkillEffect PolearmDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectPolearmDamage;
			}
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x00045C35 File Offset: 0x00043E35
		public static SkillEffect BowLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectBowLevel;
			}
		}

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06000EF9 RID: 3833 RVA: 0x00045C41 File Offset: 0x00043E41
		public static SkillEffect BowDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectBowDamage;
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06000EFA RID: 3834 RVA: 0x00045C4D File Offset: 0x00043E4D
		public static SkillEffect BowAccuracy
		{
			get
			{
				return DefaultSkillEffects.Instance._effectBowAccuracy;
			}
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06000EFB RID: 3835 RVA: 0x00045C59 File Offset: 0x00043E59
		public static SkillEffect ThrowingSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectThrowingSpeed;
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06000EFC RID: 3836 RVA: 0x00045C65 File Offset: 0x00043E65
		public static SkillEffect ThrowingDamage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectThrowingDamage;
			}
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06000EFD RID: 3837 RVA: 0x00045C71 File Offset: 0x00043E71
		public static SkillEffect ThrowingAccuracy
		{
			get
			{
				return DefaultSkillEffects.Instance._effectThrowingAccuracy;
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06000EFE RID: 3838 RVA: 0x00045C7D File Offset: 0x00043E7D
		public static SkillEffect CrossbowReloadSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectCrossbowReloadSpeed;
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06000EFF RID: 3839 RVA: 0x00045C89 File Offset: 0x00043E89
		public static SkillEffect CrossbowAccuracy
		{
			get
			{
				return DefaultSkillEffects.Instance._effectCrossbowAccuracy;
			}
		}

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06000F00 RID: 3840 RVA: 0x00045C95 File Offset: 0x00043E95
		public static SkillEffect HorseLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHorseLevel;
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06000F01 RID: 3841 RVA: 0x00045CA1 File Offset: 0x00043EA1
		public static SkillEffect HorseSpeed
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHorseSpeed;
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06000F02 RID: 3842 RVA: 0x00045CAD File Offset: 0x00043EAD
		public static SkillEffect HorseManeuver
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHorseManeuver;
			}
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06000F03 RID: 3843 RVA: 0x00045CB9 File Offset: 0x00043EB9
		public static SkillEffect HorseWeaponDamagePenalty
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHorseWeaponDamagePenalty;
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06000F04 RID: 3844 RVA: 0x00045CC5 File Offset: 0x00043EC5
		public static SkillEffect HorseWeaponSpeedPenalty
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHorseWeaponSpeedPenalty;
			}
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06000F05 RID: 3845 RVA: 0x00045CD1 File Offset: 0x00043ED1
		public static SkillEffect DismountResistance
		{
			get
			{
				return DefaultSkillEffects.Instance._effectDismountResistance;
			}
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06000F06 RID: 3846 RVA: 0x00045CDD File Offset: 0x00043EDD
		public static SkillEffect AthleticsSpeedFactor
		{
			get
			{
				return DefaultSkillEffects.Instance._effectAthleticsSpeedFactor;
			}
		}

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06000F07 RID: 3847 RVA: 0x00045CE9 File Offset: 0x00043EE9
		public static SkillEffect AthleticsWeightFactor
		{
			get
			{
				return DefaultSkillEffects.Instance._effectAthleticsWeightFactor;
			}
		}

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06000F08 RID: 3848 RVA: 0x00045CF5 File Offset: 0x00043EF5
		public static SkillEffect KnockBackResistance
		{
			get
			{
				return DefaultSkillEffects.Instance._effectKnockBackResistance;
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06000F09 RID: 3849 RVA: 0x00045D01 File Offset: 0x00043F01
		public static SkillEffect KnockDownResistance
		{
			get
			{
				return DefaultSkillEffects.Instance._effectKnockDownResistance;
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06000F0A RID: 3850 RVA: 0x00045D0D File Offset: 0x00043F0D
		public static SkillEffect SmithingLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectSmithingLevel;
			}
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06000F0B RID: 3851 RVA: 0x00045D19 File Offset: 0x00043F19
		public static SkillEffect TacticsAdvantage
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTacticsAdvantage;
			}
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06000F0C RID: 3852 RVA: 0x00045D25 File Offset: 0x00043F25
		public static SkillEffect TacticsTroopSacrificeReduction
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTacticsTroopSacrificeReduction;
			}
		}

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06000F0D RID: 3853 RVA: 0x00045D31 File Offset: 0x00043F31
		public static SkillEffect TrackingRadius
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTrackingRadius;
			}
		}

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06000F0E RID: 3854 RVA: 0x00045D3D File Offset: 0x00043F3D
		public static SkillEffect TrackingLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTrackingLevel;
			}
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06000F0F RID: 3855 RVA: 0x00045D49 File Offset: 0x00043F49
		public static SkillEffect TrackingSpottingDistance
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTrackingSpottingDistance;
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06000F10 RID: 3856 RVA: 0x00045D55 File Offset: 0x00043F55
		public static SkillEffect TrackingTrackInformation
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTrackingTrackInformation;
			}
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06000F11 RID: 3857 RVA: 0x00045D61 File Offset: 0x00043F61
		public static SkillEffect RogueryLootBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectRogueryLootBonus;
			}
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06000F12 RID: 3858 RVA: 0x00045D6D File Offset: 0x00043F6D
		public static SkillEffect CharmRelationBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectCharmRelationBonus;
			}
		}

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06000F13 RID: 3859 RVA: 0x00045D79 File Offset: 0x00043F79
		public static SkillEffect TradePenaltyReduction
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTradePenaltyReduction;
			}
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06000F14 RID: 3860 RVA: 0x00045D85 File Offset: 0x00043F85
		public static SkillEffect SurgeonSurvivalBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectSurgeonSurvivalBonus;
			}
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06000F15 RID: 3861 RVA: 0x00045D91 File Offset: 0x00043F91
		public static SkillEffect SiegeEngineProductionBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectSiegeEngineProductionBonus;
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x00045D9D File Offset: 0x00043F9D
		public static SkillEffect TownProjectBuildingBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectTownProjectBuildingBonus;
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06000F17 RID: 3863 RVA: 0x00045DA9 File Offset: 0x00043FA9
		public static SkillEffect HealingRateBonusForHeroes
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHealingRateBonusForHeroes;
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06000F18 RID: 3864 RVA: 0x00045DB5 File Offset: 0x00043FB5
		public static SkillEffect HealingRateBonusForRegulars
		{
			get
			{
				return DefaultSkillEffects.Instance._effectHealingRateBonusForRegulars;
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06000F19 RID: 3865 RVA: 0x00045DC1 File Offset: 0x00043FC1
		public static SkillEffect GovernorHealingRateBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectGovernorHealingRateBonus;
			}
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06000F1A RID: 3866 RVA: 0x00045DCD File Offset: 0x00043FCD
		public static SkillEffect LeadershipMoraleBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectLeadershipMoraleBonus;
			}
		}

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06000F1B RID: 3867 RVA: 0x00045DD9 File Offset: 0x00043FD9
		public static SkillEffect LeadershipGarrisonSizeBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectLeadershipGarrisonSizeBonus;
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06000F1C RID: 3868 RVA: 0x00045DE5 File Offset: 0x00043FE5
		public static SkillEffect StewardPartySizeBonus
		{
			get
			{
				return DefaultSkillEffects.Instance._effectStewardPartySizeBonus;
			}
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06000F1D RID: 3869 RVA: 0x00045DF1 File Offset: 0x00043FF1
		public static SkillEffect EngineerLevel
		{
			get
			{
				return DefaultSkillEffects.Instance._effectEngineerLevel;
			}
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x00045DFD File Offset: 0x00043FFD
		public DefaultSkillEffects()
		{
			this.RegisterAll();
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x00045E0C File Offset: 0x0004400C
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
			this._effectHorseWeaponDamagePenalty = this.Create("HorseWeaponDamagePenalty");
			this._effectHorseWeaponSpeedPenalty = this.Create("HorseWeaponSpeedPenalty");
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

		// Token: 0x06000F20 RID: 3872 RVA: 0x0004610B File Offset: 0x0004430B
		private SkillEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<SkillEffect>(new SkillEffect(stringId));
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x00046124 File Offset: 0x00044324
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
			this._effectHorseWeaponDamagePenalty.Initialize(new TextObject("{=0dbwEczK}Mounted weapon damage penalty: {a0}%", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 20f, 0f);
			this._effectHorseWeaponSpeedPenalty.Initialize(new TextObject("{=oE5etyy0}Mounted weapon speed & reload penalty: {a0}%", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 20f, 0f);
			this._effectDismountResistance.Initialize(new TextObject("{=kbHJVxAo}Dismount resistance: {a0}% of max. hitpoints", null), new SkillObject[] { DefaultSkills.Riding }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 40f, 0f);
			this._effectAthleticsSpeedFactor.Initialize(new TextObject("{=rgb6vdon}Running speed increased by {a0}%", null), new SkillObject[] { DefaultSkills.Athletics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectAthleticsWeightFactor.Initialize(new TextObject("{=WaUuhxwv}Weight penalty reduced by: {a0}%", null), new SkillObject[] { DefaultSkills.Athletics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectKnockBackResistance.Initialize(new TextObject("{=TyjDHQUv}Knock back resistance: {a0}% of max. hitpoints", null), new SkillObject[] { DefaultSkills.Athletics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 15f, 0f);
			this._effectKnockDownResistance.Initialize(new TextObject("{=tlNZIH3l}Knock down resistance: {a0}% of max. hitpoints", null), new SkillObject[] { DefaultSkills.Athletics }, SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, 40f, 0f);
			this._effectSmithingLevel.Initialize(new TextObject("{=ImN8Cfk6}Max difficulty of weapon that can be smithed without penalty: {a0}", null), new SkillObject[] { DefaultSkills.Crafting }, SkillEffect.PerkRole.Personal, 1f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
			this._effectTacticsAdvantage.Initialize(new TextObject("{=XO3SOlZx}Simulation advantage: +{a0}%", null), new SkillObject[] { DefaultSkills.Tactics }, SkillEffect.PerkRole.Personal, 0.001f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, 0f, 0f);
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

		// Token: 0x040004F8 RID: 1272
		private SkillEffect _effectOneHandedSpeed;

		// Token: 0x040004F9 RID: 1273
		private SkillEffect _effectOneHandedDamage;

		// Token: 0x040004FA RID: 1274
		private SkillEffect _effectTwoHandedSpeed;

		// Token: 0x040004FB RID: 1275
		private SkillEffect _effectTwoHandedDamage;

		// Token: 0x040004FC RID: 1276
		private SkillEffect _effectPolearmSpeed;

		// Token: 0x040004FD RID: 1277
		private SkillEffect _effectPolearmDamage;

		// Token: 0x040004FE RID: 1278
		private SkillEffect _effectBowLevel;

		// Token: 0x040004FF RID: 1279
		private SkillEffect _effectBowDamage;

		// Token: 0x04000500 RID: 1280
		private SkillEffect _effectBowAccuracy;

		// Token: 0x04000501 RID: 1281
		private SkillEffect _effectThrowingSpeed;

		// Token: 0x04000502 RID: 1282
		private SkillEffect _effectThrowingDamage;

		// Token: 0x04000503 RID: 1283
		private SkillEffect _effectThrowingAccuracy;

		// Token: 0x04000504 RID: 1284
		private SkillEffect _effectCrossbowReloadSpeed;

		// Token: 0x04000505 RID: 1285
		private SkillEffect _effectCrossbowAccuracy;

		// Token: 0x04000506 RID: 1286
		private SkillEffect _effectHorseLevel;

		// Token: 0x04000507 RID: 1287
		private SkillEffect _effectHorseSpeed;

		// Token: 0x04000508 RID: 1288
		private SkillEffect _effectHorseManeuver;

		// Token: 0x04000509 RID: 1289
		private SkillEffect _effectHorseWeaponDamagePenalty;

		// Token: 0x0400050A RID: 1290
		private SkillEffect _effectHorseWeaponSpeedPenalty;

		// Token: 0x0400050B RID: 1291
		private SkillEffect _effectDismountResistance;

		// Token: 0x0400050C RID: 1292
		private SkillEffect _effectAthleticsSpeedFactor;

		// Token: 0x0400050D RID: 1293
		private SkillEffect _effectAthleticsWeightFactor;

		// Token: 0x0400050E RID: 1294
		private SkillEffect _effectKnockBackResistance;

		// Token: 0x0400050F RID: 1295
		private SkillEffect _effectKnockDownResistance;

		// Token: 0x04000510 RID: 1296
		private SkillEffect _effectSmithingLevel;

		// Token: 0x04000511 RID: 1297
		private SkillEffect _effectTacticsAdvantage;

		// Token: 0x04000512 RID: 1298
		private SkillEffect _effectTacticsTroopSacrificeReduction;

		// Token: 0x04000513 RID: 1299
		private SkillEffect _effectTrackingLevel;

		// Token: 0x04000514 RID: 1300
		private SkillEffect _effectTrackingRadius;

		// Token: 0x04000515 RID: 1301
		private SkillEffect _effectTrackingSpottingDistance;

		// Token: 0x04000516 RID: 1302
		private SkillEffect _effectTrackingTrackInformation;

		// Token: 0x04000517 RID: 1303
		private SkillEffect _effectRogueryLootBonus;

		// Token: 0x04000518 RID: 1304
		private SkillEffect _effectCharmRelationBonus;

		// Token: 0x04000519 RID: 1305
		private SkillEffect _effectTradePenaltyReduction;

		// Token: 0x0400051A RID: 1306
		private SkillEffect _effectSurgeonSurvivalBonus;

		// Token: 0x0400051B RID: 1307
		private SkillEffect _effectSiegeEngineProductionBonus;

		// Token: 0x0400051C RID: 1308
		private SkillEffect _effectTownProjectBuildingBonus;

		// Token: 0x0400051D RID: 1309
		private SkillEffect _effectHealingRateBonusForHeroes;

		// Token: 0x0400051E RID: 1310
		private SkillEffect _effectHealingRateBonusForRegulars;

		// Token: 0x0400051F RID: 1311
		private SkillEffect _effectGovernorHealingRateBonus;

		// Token: 0x04000520 RID: 1312
		private SkillEffect _effectLeadershipMoraleBonus;

		// Token: 0x04000521 RID: 1313
		private SkillEffect _effectLeadershipGarrisonSizeBonus;

		// Token: 0x04000522 RID: 1314
		private SkillEffect _effectStewardPartySizeBonus;

		// Token: 0x04000523 RID: 1315
		private SkillEffect _effectEngineerLevel;
	}
}
