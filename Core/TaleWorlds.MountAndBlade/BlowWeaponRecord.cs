using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001D2 RID: 466
	[EngineStruct("Blow_weapon_record")]
	public struct BlowWeaponRecord
	{
		// Token: 0x06001A3D RID: 6717 RVA: 0x0005C7B8 File Offset: 0x0005A9B8
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

		// Token: 0x06001A3E RID: 6718 RVA: 0x0005C834 File Offset: 0x0005AA34
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

		// Token: 0x06001A3F RID: 6719 RVA: 0x0005C8B5 File Offset: 0x0005AAB5
		public bool HasWeapon()
		{
			return this.AffectorWeaponSlotOrMissileIndex >= 0;
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06001A40 RID: 6720 RVA: 0x0005C8C3 File Offset: 0x0005AAC3
		public bool IsMissile
		{
			get
			{
				return this._isMissile;
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06001A41 RID: 6721 RVA: 0x0005C8CB File Offset: 0x0005AACB
		public bool IsShield
		{
			get
			{
				return !this.WeaponFlags.HasAnyFlag(WeaponFlags.WeaponMask) && this.WeaponFlags.HasAllFlags(WeaponFlags.HasHitPoints | WeaponFlags.CanBlockRanged);
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x06001A42 RID: 6722 RVA: 0x0005C8EF File Offset: 0x0005AAEF
		public bool IsRanged
		{
			get
			{
				return this.WeaponFlags.HasAnyFlag(WeaponFlags.RangedWeapon);
			}
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06001A43 RID: 6723 RVA: 0x0005C8FE File Offset: 0x0005AAFE
		public bool IsAmmo
		{
			get
			{
				return !this.WeaponFlags.HasAnyFlag(WeaponFlags.WeaponMask) && this.WeaponFlags.HasAnyFlag(WeaponFlags.Consumable);
			}
		}

		// Token: 0x06001A44 RID: 6724 RVA: 0x0005C924 File Offset: 0x0005AB24
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

		// Token: 0x04000851 RID: 2129
		public Vec3 StartingPosition;

		// Token: 0x04000852 RID: 2130
		public Vec3 CurrentPosition;

		// Token: 0x04000853 RID: 2131
		public Vec3 Velocity;

		// Token: 0x04000854 RID: 2132
		public ItemFlags ItemFlags;

		// Token: 0x04000855 RID: 2133
		public WeaponFlags WeaponFlags;

		// Token: 0x04000856 RID: 2134
		public WeaponClass WeaponClass;

		// Token: 0x04000857 RID: 2135
		public sbyte BoneNoToAttach;

		// Token: 0x04000858 RID: 2136
		public int AffectorWeaponSlotOrMissileIndex;

		// Token: 0x04000859 RID: 2137
		public float Weight;

		// Token: 0x0400085A RID: 2138
		[MarshalAs(UnmanagedType.I1)]
		private bool _isMissile;

		// Token: 0x0400085B RID: 2139
		[MarshalAs(UnmanagedType.I1)]
		private bool _isMaterialMetal;
	}
}
