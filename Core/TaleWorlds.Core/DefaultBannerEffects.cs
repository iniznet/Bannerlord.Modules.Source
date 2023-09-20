using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200004A RID: 74
	public class DefaultBannerEffects
	{
		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x00013FA4 File Offset: 0x000121A4
		private static DefaultBannerEffects Instance
		{
			get
			{
				return Game.Current.DefaultBannerEffects;
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x00013FB0 File Offset: 0x000121B0
		public static BannerEffect IncreasedMeleeDamage
		{
			get
			{
				return DefaultBannerEffects.Instance._increasedMeleeDamage;
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000572 RID: 1394 RVA: 0x00013FBC File Offset: 0x000121BC
		public static BannerEffect IncreasedDamageAgainstMountedTroops
		{
			get
			{
				return DefaultBannerEffects.Instance._increasedDamageAgainstMountedTroops;
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x00013FC8 File Offset: 0x000121C8
		public static BannerEffect IncreasedRangedDamage
		{
			get
			{
				return DefaultBannerEffects.Instance._increasedRangedDamage;
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x00013FD4 File Offset: 0x000121D4
		public static BannerEffect IncreasedChargeDamage
		{
			get
			{
				return DefaultBannerEffects.Instance._increasedChargeDamage;
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000575 RID: 1397 RVA: 0x00013FE0 File Offset: 0x000121E0
		public static BannerEffect DecreasedRangedAccuracyPenalty
		{
			get
			{
				return DefaultBannerEffects.Instance._decreasedRangedAccuracyPenalty;
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x00013FEC File Offset: 0x000121EC
		public static BannerEffect DecreasedMoraleShock
		{
			get
			{
				return DefaultBannerEffects.Instance._decreasedMoraleShock;
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000577 RID: 1399 RVA: 0x00013FF8 File Offset: 0x000121F8
		public static BannerEffect DecreasedMeleeAttackDamage
		{
			get
			{
				return DefaultBannerEffects.Instance._decreasedMeleeAttackDamage;
			}
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000578 RID: 1400 RVA: 0x00014004 File Offset: 0x00012204
		public static BannerEffect DecreasedRangedAttackDamage
		{
			get
			{
				return DefaultBannerEffects.Instance._decreasedRangedAttackDamage;
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000579 RID: 1401 RVA: 0x00014010 File Offset: 0x00012210
		public static BannerEffect DecreasedShieldDamage
		{
			get
			{
				return DefaultBannerEffects.Instance._decreasedShieldDamage;
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x0001401C File Offset: 0x0001221C
		public static BannerEffect IncreasedTroopMovementSpeed
		{
			get
			{
				return DefaultBannerEffects.Instance._increasedTroopMovementSpeed;
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x0600057B RID: 1403 RVA: 0x00014028 File Offset: 0x00012228
		public static BannerEffect IncreasedMountMovementSpeed
		{
			get
			{
				return DefaultBannerEffects.Instance._increasedMountMovementSpeed;
			}
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x00014034 File Offset: 0x00012234
		public DefaultBannerEffects()
		{
			this.RegisterAll();
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x00014044 File Offset: 0x00012244
		private void RegisterAll()
		{
			this._increasedMeleeDamage = this.Create("IncreasedMeleeDamage");
			this._increasedDamageAgainstMountedTroops = this.Create("IncreasedDamageAgainstMountedTroops");
			this._increasedRangedDamage = this.Create("IncreasedRangedDamage");
			this._increasedChargeDamage = this.Create("IncreasedChargeDamage");
			this._decreasedRangedAccuracyPenalty = this.Create("DecreasedRangedAccuracyPenalty");
			this._decreasedMoraleShock = this.Create("DecreasedMoraleShock");
			this._decreasedMeleeAttackDamage = this.Create("DecreasedMeleeAttackDamage");
			this._decreasedRangedAttackDamage = this.Create("DecreasedRangedAttackDamage");
			this._decreasedShieldDamage = this.Create("DecreasedShieldDamage");
			this._increasedTroopMovementSpeed = this.Create("IncreasedTroopMovementSpeed");
			this._increasedMountMovementSpeed = this.Create("IncreasedMountMovementSpeed");
			this.InitializeAll();
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00014112 File Offset: 0x00012312
		private BannerEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<BannerEffect>(new BannerEffect(stringId));
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0001412C File Offset: 0x0001232C
		private void InitializeAll()
		{
			this._increasedMeleeDamage.Initialize("{=unaWKloT}Increased Melee Damage", "{=8ZNOgT8Z}{BONUS_AMOUNT}% melee damage to troops in your formation.", 0.05f, 0.1f, 0.15f, BannerEffect.EffectIncrementType.AddFactor);
			this._increasedDamageAgainstMountedTroops.Initialize("{=2bHoiaoe}Increased Damage Against Mounted Troops", "{=9RZLSV3E}{BONUS_AMOUNT}% damage by melee troops in your formation against cavalry.", 0.1f, 0.2f, 0.3f, BannerEffect.EffectIncrementType.AddFactor);
			this._increasedRangedDamage.Initialize("{=Ch5NpCd0}Increased Ranged Damage", "{=labbKop6}{BONUS_AMOUNT}% ranged damage to troops in your formation.", 0.04f, 0.06f, 0.08f, BannerEffect.EffectIncrementType.AddFactor);
			this._increasedChargeDamage.Initialize("{=O2oBC9sH}Increased Charge Damage", "{=Z2xgnrDa}{BONUS_AMOUNT}% charge damage to mounted troops in your formation.", 0.1f, 0.2f, 0.3f, BannerEffect.EffectIncrementType.AddFactor);
			this._decreasedRangedAccuracyPenalty.Initialize("{=MkBPRCuF}Decreased Ranged Accuracy Penalty", "{=Gu0Wxxul}{BONUS_AMOUNT}% accuracy penalty for ranged troops in your formation.", -0.04f, -0.06f, -0.08f, BannerEffect.EffectIncrementType.AddFactor);
			this._decreasedMoraleShock.Initialize("{=nOMT0Cw6}Decreased Morale Shock", "{=Lso8j7Iv}{BONUS_AMOUNT}% morale penalty from casualties to troops in your formation.", -0.1f, -0.2f, -0.3f, BannerEffect.EffectIncrementType.AddFactor);
			this._decreasedMeleeAttackDamage.Initialize("{=a3Vc59WV}Decreased Taken Melee Attack Damage", "{=ORFrCYSn}{BONUS_AMOUNT}% damage by melee attacks to troops in your formation.", -0.05f, -0.1f, -0.15f, BannerEffect.EffectIncrementType.AddFactor);
			this._decreasedRangedAttackDamage.Initialize("{=p0JFbL7G}Decreased Taken Ranged Attack Damage", "{=q7NmR3AP}{BONUS_AMOUNT}% morale penalty from casualties to troops in your formation.", -0.05f, -0.1f, -0.15f, BannerEffect.EffectIncrementType.AddFactor);
			this._decreasedShieldDamage.Initialize("{=T79exjaP}Decreased Taken Shield Damage", "{=klGEDUmw}{BONUS_AMOUNT}% damage to shields of troops in your formation.", -0.15f, -0.25f, -0.3f, BannerEffect.EffectIncrementType.AddFactor);
			this._increasedTroopMovementSpeed.Initialize("{=PbJAOKKZ}Increased Troop Movement Speed", "{=nqWulUTP}{BONUS_AMOUNT}% movement speed to infantry in your formation.", 0.15f, 0.25f, 0.3f, BannerEffect.EffectIncrementType.AddFactor);
			this._increasedMountMovementSpeed.Initialize("{=nMfxbc0Y}Increased Mount Movement Speed", "{=g0l7W5xQ}{BONUS_AMOUNT}% movement speed to mounts in your formation.", 0.05f, 0.08f, 0.1f, BannerEffect.EffectIncrementType.AddFactor);
		}

		// Token: 0x040002A2 RID: 674
		private BannerEffect _increasedMeleeDamage;

		// Token: 0x040002A3 RID: 675
		private BannerEffect _increasedDamageAgainstMountedTroops;

		// Token: 0x040002A4 RID: 676
		private BannerEffect _increasedRangedDamage;

		// Token: 0x040002A5 RID: 677
		private BannerEffect _increasedChargeDamage;

		// Token: 0x040002A6 RID: 678
		private BannerEffect _decreasedRangedAccuracyPenalty;

		// Token: 0x040002A7 RID: 679
		private BannerEffect _decreasedMoraleShock;

		// Token: 0x040002A8 RID: 680
		private BannerEffect _decreasedMeleeAttackDamage;

		// Token: 0x040002A9 RID: 681
		private BannerEffect _decreasedRangedAttackDamage;

		// Token: 0x040002AA RID: 682
		private BannerEffect _decreasedShieldDamage;

		// Token: 0x040002AB RID: 683
		private BannerEffect _increasedTroopMovementSpeed;

		// Token: 0x040002AC RID: 684
		private BannerEffect _increasedMountMovementSpeed;
	}
}
