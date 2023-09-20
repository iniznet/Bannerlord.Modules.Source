using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattleObjects
{
	// Token: 0x02000029 RID: 41
	public class CustomBattleBannerEffects
	{
		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001CB RID: 459 RVA: 0x0000A602 File Offset: 0x00008802
		private static CustomBattleBannerEffects Instance
		{
			get
			{
				return CustomGame.Current.CustomBattleBannerEffects;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001CC RID: 460 RVA: 0x0000A60E File Offset: 0x0000880E
		public static BannerEffect IncreasedMeleeDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedMeleeDamage;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001CD RID: 461 RVA: 0x0000A61A File Offset: 0x0000881A
		public static BannerEffect IncreasedDamageAgainstMountedTroops
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedDamageAgainstMountedTroops;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0000A626 File Offset: 0x00008826
		public static BannerEffect IncreasedRangedDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedRangedDamage;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0000A632 File Offset: 0x00008832
		public static BannerEffect IncreasedChargeDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedChargeDamage;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000A63E File Offset: 0x0000883E
		public static BannerEffect DecreasedRangedWeaponAccuracy
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedRangedAccuracyPenalty;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x0000A64A File Offset: 0x0000884A
		public static BannerEffect DecreasedMoraleShock
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedMoraleShock;
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x0000A656 File Offset: 0x00008856
		public static BannerEffect DecreasedMeleeAttackDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedMeleeAttackDamage;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x0000A662 File Offset: 0x00008862
		public static BannerEffect DecreasedRangedAttackDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedRangedAttackDamage;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000A66E File Offset: 0x0000886E
		public static BannerEffect DecreasedShieldDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedShieldDamage;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x0000A67A File Offset: 0x0000887A
		public static BannerEffect IncreasedTroopMovementSpeed
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedTroopMovementSpeed;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000A686 File Offset: 0x00008886
		public static BannerEffect IncreasedMountMovementSpeed
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedMountMovementSpeed;
			}
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000A692 File Offset: 0x00008892
		public CustomBattleBannerEffects()
		{
			this.RegisterAll();
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000A6A0 File Offset: 0x000088A0
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

		// Token: 0x060001D9 RID: 473 RVA: 0x0000A76E File Offset: 0x0000896E
		private BannerEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<BannerEffect>(new BannerEffect(stringId));
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000A788 File Offset: 0x00008988
		private void InitializeAll()
		{
			this._increasedMeleeDamage.Initialize("{=unaWKloT}Increased Melee Damage", "{=8ZNOgT8Z}{BONUS_AMOUNT}% melee damage to troops in your formation.", 0.05f, 0.1f, 0.15f, 1);
			this._increasedDamageAgainstMountedTroops.Initialize("{=2bHoiaoe}Increased Damage Against Mounted Troops", "{=9RZLSV3E}{BONUS_AMOUNT}% damage by melee troops in your formation against cavalry.", 0.1f, 0.2f, 0.3f, 1);
			this._increasedRangedDamage.Initialize("{=Ch5NpCd0}Increased Ranged Damage", "{=labbKop6}{BONUS_AMOUNT}% ranged damage to troops in your formation.", 0.04f, 0.06f, 0.08f, 1);
			this._decreasedRangedAccuracyPenalty.Initialize("{=MkBPRCuF}Decreased Ranged Accuracy Penalty", "{=m86oViPX}{BONUS_AMOUNT}% accuracy penalty for ranged troops in your formation.", -0.04f, -0.06f, -0.08f, 1);
			this._increasedChargeDamage.Initialize("{=O2oBC9sH}Increased Charge Damage", "{=Z2xgnrDa}{BONUS_AMOUNT}% charge damage to mounted troops in your formation.", 0.1f, 0.2f, 0.3f, 1);
			this._decreasedMoraleShock.Initialize("{=nOMT0Cw6}Decreased Morale Shock", "{=Lso8j7Iv}{BONUS_AMOUNT}% morale penalty from casualties to troops in your formation.", -0.1f, -0.2f, -0.3f, 1);
			this._decreasedMeleeAttackDamage.Initialize("{=a3Vc59WV}Decreased Taken Melee Attack Damage", "{=ORFrCYSn}{BONUS_AMOUNT}% damage by melee attacks to troops in your formation.", -0.05f, -0.1f, -0.15f, 1);
			this._decreasedRangedAttackDamage.Initialize("{=p0JFbL7G}Decreased Taken Ranged Attack Damage", "{=q7NmR3AP}{BONUS_AMOUNT}% morale penalty from casualties to troops in your formation.", -0.05f, -0.1f, -0.15f, 1);
			this._decreasedShieldDamage.Initialize("{=T79exjaP}Decreased Taken Shield Damage", "{=klGEDUmw}{BONUS_AMOUNT}% damage to shields of troops in your formation.", -0.15f, -0.25f, -0.3f, 1);
			this._increasedTroopMovementSpeed.Initialize("{=PbJAOKKZ}Increased Troop Movement Speed", "{=nqWulUTP}{BONUS_AMOUNT}% movement speed to infantry in your formation.", 0.15f, 0.25f, 0.3f, 1);
			this._increasedMountMovementSpeed.Initialize("{=nMfxbc0Y}Increased Mount Movement Speed", "{=g0l7W5xQ}{BONUS_AMOUNT}% movement speed to mounts in your formation.", 0.05f, 0.08f, 0.1f, 1);
		}

		// Token: 0x04000121 RID: 289
		private BannerEffect _increasedMeleeDamage;

		// Token: 0x04000122 RID: 290
		private BannerEffect _increasedDamageAgainstMountedTroops;

		// Token: 0x04000123 RID: 291
		private BannerEffect _increasedRangedDamage;

		// Token: 0x04000124 RID: 292
		private BannerEffect _increasedChargeDamage;

		// Token: 0x04000125 RID: 293
		private BannerEffect _decreasedRangedAccuracyPenalty;

		// Token: 0x04000126 RID: 294
		private BannerEffect _decreasedMoraleShock;

		// Token: 0x04000127 RID: 295
		private BannerEffect _decreasedMeleeAttackDamage;

		// Token: 0x04000128 RID: 296
		private BannerEffect _decreasedRangedAttackDamage;

		// Token: 0x04000129 RID: 297
		private BannerEffect _decreasedShieldDamage;

		// Token: 0x0400012A RID: 298
		private BannerEffect _increasedTroopMovementSpeed;

		// Token: 0x0400012B RID: 299
		private BannerEffect _increasedMountMovementSpeed;
	}
}
