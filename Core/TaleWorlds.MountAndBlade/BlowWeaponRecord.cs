using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Blow_weapon_record")]
	public struct BlowWeaponRecord
	{
		public void FillAsMeleeBlow(ItemObject item, WeaponComponentData weaponComponentData, int affectorWeaponSlot, sbyte weaponAttachBoneIndex)
		{
			this._isMissile = false;
			if (weaponComponentData != null)
			{
				this.ItemFlags = item.ItemFlags;
				this.WeaponFlags = weaponComponentData.WeaponFlags;
				this.WeaponClass = weaponComponentData.WeaponClass;
				this.BoneNoToAttach = weaponAttachBoneIndex;
				this.AffectorWeaponSlotOrMissileIndex = affectorWeaponSlot;
				this.Weight = item.Weight;
				this._isMaterialMetal = weaponComponentData.PhysicsMaterial.Contains("metal");
				return;
			}
			this._isMaterialMetal = false;
			this.AffectorWeaponSlotOrMissileIndex = -1;
		}

		public void FillAsMissileBlow(ItemObject item, WeaponComponentData weaponComponentData, int missileIndex, sbyte weaponAttachBoneIndex, Vec3 startingPosition, Vec3 currentPosition, Vec3 velocity)
		{
			this._isMissile = true;
			this.StartingPosition = startingPosition;
			this.CurrentPosition = currentPosition;
			this.Velocity = velocity;
			this.ItemFlags = item.ItemFlags;
			this.WeaponFlags = weaponComponentData.WeaponFlags;
			this.WeaponClass = weaponComponentData.WeaponClass;
			this.BoneNoToAttach = weaponAttachBoneIndex;
			this.AffectorWeaponSlotOrMissileIndex = missileIndex;
			this.Weight = item.Weight;
			this._isMaterialMetal = weaponComponentData.PhysicsMaterial.Contains("metal");
		}

		public bool HasWeapon()
		{
			return this.AffectorWeaponSlotOrMissileIndex >= 0;
		}

		public bool IsMissile
		{
			get
			{
				return this._isMissile;
			}
		}

		public bool IsShield
		{
			get
			{
				return !this.WeaponFlags.HasAnyFlag(WeaponFlags.WeaponMask) && this.WeaponFlags.HasAllFlags(WeaponFlags.HasHitPoints | WeaponFlags.CanBlockRanged);
			}
		}

		public bool IsRanged
		{
			get
			{
				return this.WeaponFlags.HasAnyFlag(WeaponFlags.RangedWeapon);
			}
		}

		public bool IsAmmo
		{
			get
			{
				return !this.WeaponFlags.HasAnyFlag(WeaponFlags.WeaponMask) && this.WeaponFlags.HasAnyFlag(WeaponFlags.Consumable);
			}
		}

		public int GetHitSound(bool isOwnerHumanoid, bool isCriticalBlow, bool isLowBlow, bool isNonTipThrust, AgentAttackType attackType, DamageTypes damageType)
		{
			int num;
			if (this.HasWeapon())
			{
				if (this.IsRanged || this.IsAmmo)
				{
					switch (this.WeaponClass)
					{
					case WeaponClass.Stone:
						if (isCriticalBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatThrowingStoneHigh;
						}
						else if (isLowBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatThrowingStoneLow;
						}
						else
						{
							num = CombatSoundContainer.SoundCodeMissionCombatThrowingStoneMed;
						}
						break;
					case WeaponClass.Boulder:
						if (isCriticalBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatBoulderHigh;
						}
						else if (isLowBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatBoulderLow;
						}
						else
						{
							num = CombatSoundContainer.SoundCodeMissionCombatBoulderMed;
						}
						break;
					case WeaponClass.ThrowingAxe:
						if (isCriticalBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatThrowingAxeHigh;
						}
						else if (isLowBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatThrowingAxeLow;
						}
						else
						{
							num = CombatSoundContainer.SoundCodeMissionCombatThrowingAxeMed;
						}
						break;
					case WeaponClass.ThrowingKnife:
						if (isCriticalBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerHigh;
						}
						else if (isLowBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerLow;
						}
						else
						{
							num = CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerMed;
						}
						break;
					case WeaponClass.Javelin:
						if (isCriticalBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatMissileHigh;
						}
						else if (isLowBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatMissileLow;
						}
						else
						{
							num = CombatSoundContainer.SoundCodeMissionCombatMissileMed;
						}
						break;
					default:
						if (isCriticalBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatMissileHigh;
						}
						else if (isLowBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatMissileLow;
						}
						else
						{
							num = CombatSoundContainer.SoundCodeMissionCombatMissileMed;
						}
						break;
					}
				}
				else if (this.IsShield)
				{
					if (this._isMaterialMetal)
					{
						num = CombatSoundContainer.SoundCodeMissionCombatMetalShieldBash;
					}
					else
					{
						num = CombatSoundContainer.SoundCodeMissionCombatWoodShieldBash;
					}
				}
				else if (attackType == AgentAttackType.Bash)
				{
					num = CombatSoundContainer.SoundCodeMissionCombatBluntLow;
				}
				else
				{
					if (isNonTipThrust)
					{
						damageType = DamageTypes.Blunt;
					}
					switch (damageType)
					{
					case DamageTypes.Cut:
						if (isCriticalBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatCutHigh;
						}
						else if (isLowBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatCutLow;
						}
						else
						{
							num = CombatSoundContainer.SoundCodeMissionCombatCutMed;
						}
						break;
					case DamageTypes.Pierce:
						if (isCriticalBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatPierceHigh;
						}
						else if (isLowBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatPierceLow;
						}
						else
						{
							num = CombatSoundContainer.SoundCodeMissionCombatPierceMed;
						}
						break;
					case DamageTypes.Blunt:
						if (isCriticalBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatBluntHigh;
						}
						else if (isLowBlow)
						{
							num = CombatSoundContainer.SoundCodeMissionCombatBluntLow;
						}
						else
						{
							num = CombatSoundContainer.SoundCodeMissionCombatBluntMed;
						}
						break;
					default:
						num = CombatSoundContainer.SoundCodeMissionCombatBluntMed;
						Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\BlowWeaponRecord.cs", "GetHitSound", 246);
						break;
					}
				}
			}
			else if (!isOwnerHumanoid)
			{
				num = CombatSoundContainer.SoundCodeMissionCombatChargeDamage;
			}
			else if (attackType == AgentAttackType.Kick)
			{
				num = CombatSoundContainer.SoundCodeMissionCombatKick;
			}
			else if (isCriticalBlow)
			{
				num = CombatSoundContainer.SoundCodeMissionCombatPunchHigh;
			}
			else if (isLowBlow)
			{
				num = CombatSoundContainer.SoundCodeMissionCombatPunchLow;
			}
			else
			{
				num = CombatSoundContainer.SoundCodeMissionCombatPunchMed;
			}
			return num;
		}

		public Vec3 StartingPosition;

		public Vec3 CurrentPosition;

		public Vec3 Velocity;

		public ItemFlags ItemFlags;

		public WeaponFlags WeaponFlags;

		public WeaponClass WeaponClass;

		public sbyte BoneNoToAttach;

		public int AffectorWeaponSlotOrMissileIndex;

		public float Weight;

		[MarshalAs(UnmanagedType.I1)]
		private bool _isMissile;

		[MarshalAs(UnmanagedType.I1)]
		private bool _isMaterialMetal;
	}
}
