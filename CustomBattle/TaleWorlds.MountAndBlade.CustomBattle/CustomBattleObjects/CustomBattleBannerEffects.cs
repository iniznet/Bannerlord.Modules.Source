using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattleObjects
{
	public class CustomBattleBannerEffects
	{
		private static CustomBattleBannerEffects Instance
		{
			get
			{
				return CustomGame.Current.CustomBattleBannerEffects;
			}
		}

		public static BannerEffect IncreasedMeleeDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedMeleeDamage;
			}
		}

		public static BannerEffect IncreasedMeleeDamageAgainstMountedTroops
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedMeleeDamageAgainstMountedTroops;
			}
		}

		public static BannerEffect IncreasedRangedDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedRangedDamage;
			}
		}

		public static BannerEffect IncreasedChargeDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedChargeDamage;
			}
		}

		public static BannerEffect DecreasedRangedWeaponAccuracy
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedRangedAccuracyPenalty;
			}
		}

		public static BannerEffect DecreasedMoraleShock
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedMoraleShock;
			}
		}

		public static BannerEffect DecreasedMeleeAttackDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedMeleeAttackDamage;
			}
		}

		public static BannerEffect DecreasedRangedAttackDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedRangedAttackDamage;
			}
		}

		public static BannerEffect DecreasedShieldDamage
		{
			get
			{
				return CustomBattleBannerEffects.Instance._decreasedShieldDamage;
			}
		}

		public static BannerEffect IncreasedTroopMovementSpeed
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedTroopMovementSpeed;
			}
		}

		public static BannerEffect IncreasedMountMovementSpeed
		{
			get
			{
				return CustomBattleBannerEffects.Instance._increasedMountMovementSpeed;
			}
		}

		public CustomBattleBannerEffects()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			this._increasedMeleeDamage = this.Create("IncreasedMeleeDamage");
			this._increasedMeleeDamageAgainstMountedTroops = this.Create("IncreasedMeleeDamageAgainstMountedTroops");
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

		private BannerEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<BannerEffect>(new BannerEffect(stringId));
		}

		private void InitializeAll()
		{
			this._increasedMeleeDamage.Initialize("{=unaWKloT}Increased Melee Damage", "{=8ZNOgT8Z}{BONUS_AMOUNT}% melee damage to troops in your formation.", 0.05f, 0.1f, 0.15f, 1);
			this._increasedMeleeDamageAgainstMountedTroops.Initialize("{=2bHoiaoe}Increased Damage Against Mounted Troops", "{=9RZLSV3E}{BONUS_AMOUNT}% damage by melee troops in your formation against cavalry.", 0.1f, 0.2f, 0.3f, 1);
			this._increasedRangedDamage.Initialize("{=Ch5NpCd0}Increased Ranged Damage", "{=labbKop6}{BONUS_AMOUNT}% ranged damage to troops in your formation.", 0.04f, 0.06f, 0.08f, 1);
			this._decreasedRangedAccuracyPenalty.Initialize("{=MkBPRCuF}Decreased Ranged Accuracy Penalty", "{=m86oViPX}{BONUS_AMOUNT}% accuracy penalty for ranged troops in your formation.", -0.04f, -0.06f, -0.08f, 1);
			this._increasedChargeDamage.Initialize("{=O2oBC9sH}Increased Charge Damage", "{=Z2xgnrDa}{BONUS_AMOUNT}% charge damage to mounted troops in your formation.", 0.1f, 0.2f, 0.3f, 1);
			this._decreasedMoraleShock.Initialize("{=nOMT0Cw6}Decreased Morale Shock", "{=W0agPHes}{BONUS_AMOUNT}% morale penalty from casualties to troops in your formation.", -0.1f, -0.2f, -0.3f, 1);
			this._decreasedMeleeAttackDamage.Initialize("{=a3Vc59WV}Decreased Taken Melee Attack Damage", "{=ORFrCYSn}{BONUS_AMOUNT}% damage by melee attacks to troops in your formation.", -0.05f, -0.1f, -0.15f, 1);
			this._decreasedRangedAttackDamage.Initialize("{=p0JFbL7G}Decreased Taken Ranged Attack Damage", "{=W0agPHes}{BONUS_AMOUNT}% morale penalty from casualties to troops in your formation.", -0.05f, -0.1f, -0.15f, 1);
			this._decreasedShieldDamage.Initialize("{=T79exjaP}Decreased Taken Shield Damage", "{=klGEDUmw}{BONUS_AMOUNT}% damage to shields of troops in your formation.", -0.15f, -0.25f, -0.3f, 1);
			this._increasedTroopMovementSpeed.Initialize("{=PbJAOKKZ}Increased Troop Movement Speed", "{=nqWulUTP}{BONUS_AMOUNT}% movement speed to infantry in your formation.", 0.15f, 0.25f, 0.3f, 1);
			this._increasedMountMovementSpeed.Initialize("{=nMfxbc0Y}Increased Mount Movement Speed", "{=g0l7W5xQ}{BONUS_AMOUNT}% movement speed to mounts in your formation.", 0.05f, 0.08f, 0.1f, 1);
		}

		private BannerEffect _increasedMeleeDamage;

		private BannerEffect _increasedMeleeDamageAgainstMountedTroops;

		private BannerEffect _increasedRangedDamage;

		private BannerEffect _increasedChargeDamage;

		private BannerEffect _decreasedRangedAccuracyPenalty;

		private BannerEffect _decreasedMoraleShock;

		private BannerEffect _decreasedMeleeAttackDamage;

		private BannerEffect _decreasedRangedAttackDamage;

		private BannerEffect _decreasedShieldDamage;

		private BannerEffect _increasedTroopMovementSpeed;

		private BannerEffect _increasedMountMovementSpeed;
	}
}
