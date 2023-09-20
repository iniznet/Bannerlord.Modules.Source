using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public struct MissionWeapon
	{
		public ItemObject Item { get; private set; }

		public ItemModifier ItemModifier { get; private set; }

		public int WeaponsCount
		{
			get
			{
				return this._weapons.Count;
			}
		}

		public WeaponComponentData CurrentUsageItem
		{
			get
			{
				if (this._weapons == null || this._weapons.Count == 0)
				{
					return null;
				}
				return this._weapons[this.CurrentUsageIndex];
			}
		}

		public short ReloadPhase { get; set; }

		public short ReloadPhaseCount
		{
			get
			{
				short num = 1;
				if (this.CurrentUsageItem != null)
				{
					num = this.CurrentUsageItem.ReloadPhaseCount;
				}
				return num;
			}
		}

		public bool IsReloading
		{
			get
			{
				return this.ReloadPhase < this.ReloadPhaseCount;
			}
		}

		public Banner Banner { get; private set; }

		public float GlossMultiplier { get; private set; }

		public short RawDataForNetwork
		{
			get
			{
				return this._dataValue;
			}
		}

		public short HitPoints
		{
			get
			{
				return this._dataValue;
			}
			set
			{
				this._dataValue = value;
			}
		}

		public short Amount
		{
			get
			{
				return this._dataValue;
			}
			set
			{
				this._dataValue = value;
			}
		}

		public short Ammo
		{
			get
			{
				MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
				if (ammoWeapon == null)
				{
					return 0;
				}
				return ammoWeapon.Value._dataValue;
			}
		}

		public MissionWeapon AmmoWeapon
		{
			get
			{
				MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
				if (ammoWeapon == null)
				{
					return MissionWeapon.Invalid;
				}
				return ammoWeapon.Value;
			}
		}

		public short MaxAmmo
		{
			get
			{
				return this._modifiedMaxDataValue;
			}
		}

		public short ModifiedMaxAmount
		{
			get
			{
				return this._modifiedMaxDataValue;
			}
		}

		public short ModifiedMaxHitPoints
		{
			get
			{
				return this._modifiedMaxDataValue;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.CurrentUsageItem == null;
			}
		}

		public MissionWeapon(ItemObject item, ItemModifier itemModifier, Banner banner)
		{
			this.Item = item;
			this.ItemModifier = itemModifier;
			this.Banner = banner;
			this.CurrentUsageIndex = 0;
			this._weapons = new List<WeaponComponentData>(1);
			this._modifiedMaxDataValue = 0;
			this._hasAnyConsumableUsage = false;
			if (item != null && item.Weapons != null)
			{
				foreach (WeaponComponentData weaponComponentData in item.Weapons)
				{
					this._weapons.Add(weaponComponentData);
					bool isConsumable = weaponComponentData.IsConsumable;
					if (isConsumable || weaponComponentData.IsRangedWeapon || weaponComponentData.WeaponFlags.HasAnyFlag(WeaponFlags.HasHitPoints))
					{
						this._modifiedMaxDataValue = weaponComponentData.MaxDataValue;
						if (itemModifier != null)
						{
							if (weaponComponentData.WeaponFlags.HasAnyFlag(WeaponFlags.HasHitPoints))
							{
								this._modifiedMaxDataValue = weaponComponentData.GetModifiedMaximumHitPoints(itemModifier);
							}
							else if (isConsumable)
							{
								this._modifiedMaxDataValue = weaponComponentData.GetModifiedStackCount(itemModifier);
							}
						}
					}
					if (isConsumable)
					{
						this._hasAnyConsumableUsage = true;
					}
				}
			}
			this._dataValue = this._modifiedMaxDataValue;
			this.ReloadPhase = 0;
			this._ammoWeapon = null;
			this._attachedWeapons = null;
			this._attachedWeaponFrames = null;
			this.GlossMultiplier = 1f;
		}

		public MissionWeapon(ItemObject primaryItem, ItemModifier itemModifier, Banner banner, short dataValue)
		{
			this = new MissionWeapon(primaryItem, itemModifier, banner);
			this._dataValue = dataValue;
		}

		public MissionWeapon(ItemObject primaryItem, ItemModifier itemModifier, Banner banner, short dataValue, short reloadPhase, MissionWeapon? ammoWeapon)
		{
			this = new MissionWeapon(primaryItem, itemModifier, banner, dataValue);
			this.ReloadPhase = reloadPhase;
			this._ammoWeapon = ((ammoWeapon != null) ? new MissionWeapon.MissionSubWeapon(ammoWeapon.Value) : null);
		}

		public TextObject GetModifiedItemName()
		{
			if (this.ItemModifier == null)
			{
				return this.Item.Name;
			}
			TextObject name = this.ItemModifier.Name;
			name.SetTextVariable("ITEMNAME", this.Item.Name);
			return name;
		}

		public bool IsEqualTo(MissionWeapon other)
		{
			return this.Item == other.Item;
		}

		public bool IsSameType(MissionWeapon other)
		{
			return this.Item.PrimaryWeapon.WeaponClass == other.Item.PrimaryWeapon.WeaponClass;
		}

		public float GetWeight()
		{
			float num = (this.Item.PrimaryWeapon.IsConsumable ? (this.GetBaseWeight() * (float)this._dataValue) : this.GetBaseWeight());
			MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
			return num + ((ammoWeapon != null) ? ammoWeapon.Value.GetWeight() : 0f);
		}

		private float GetBaseWeight()
		{
			return this.Item.Weight;
		}

		public WeaponComponentData GetWeaponComponentDataForUsage(int usageIndex)
		{
			return this._weapons[usageIndex];
		}

		public int GetGetModifiedArmorForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedArmor(this.ItemModifier);
		}

		public int GetModifiedThrustDamageForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedThrustDamage(this.ItemModifier);
		}

		public int GetModifiedSwingDamageForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedSwingDamage(this.ItemModifier);
		}

		public int GetModifiedMissileDamageForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedMissileDamage(this.ItemModifier);
		}

		public int GetModifiedThrustSpeedForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedThrustSpeed(this.ItemModifier);
		}

		public int GetModifiedSwingSpeedForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedSwingSpeed(this.ItemModifier);
		}

		public int GetModifiedMissileSpeedForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedMissileSpeed(this.ItemModifier);
		}

		public int GetModifiedMissileSpeedForUsage(int usageIndex)
		{
			return this._weapons[usageIndex].GetModifiedMissileSpeed(this.ItemModifier);
		}

		public int GetModifiedHandlingForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedHandling(this.ItemModifier);
		}

		public WeaponData GetWeaponData(bool needBatchedVersionForMeshes)
		{
			if (!this.IsEmpty && this.Item.WeaponComponent != null)
			{
				WeaponComponent weaponComponent = this.Item.WeaponComponent;
				WeaponData weaponData = new WeaponData
				{
					WeaponKind = (int)this.Item.Id.InternalValue,
					ItemHolsterIndices = this.Item.GetItemHolsterIndices(),
					ReloadPhase = this.ReloadPhase,
					Difficulty = this.Item.Difficulty,
					BaseWeight = this.GetBaseWeight(),
					HasFlagAnimation = false,
					WeaponFrame = weaponComponent.PrimaryWeapon.Frame,
					ScaleFactor = this.Item.ScaleFactor,
					Inertia = weaponComponent.PrimaryWeapon.Inertia,
					CenterOfMass = weaponComponent.PrimaryWeapon.CenterOfMass,
					CenterOfMass3D = weaponComponent.PrimaryWeapon.CenterOfMass3D,
					HolsterPositionShift = this.Item.HolsterPositionShift,
					TrailParticleName = weaponComponent.PrimaryWeapon.TrailParticleName,
					AmmoOffset = weaponComponent.PrimaryWeapon.AmmoOffset
				};
				string physicsMaterial = weaponComponent.PrimaryWeapon.PhysicsMaterial;
				weaponData.PhysicsMaterialIndex = (string.IsNullOrEmpty(physicsMaterial) ? PhysicsMaterial.InvalidPhysicsMaterial.Index : PhysicsMaterial.GetFromName(physicsMaterial).Index);
				weaponData.FlyingSoundCode = SoundManager.GetEventGlobalIndex(weaponComponent.PrimaryWeapon.FlyingSoundCode);
				weaponData.PassbySoundCode = SoundManager.GetEventGlobalIndex(weaponComponent.PrimaryWeapon.PassbySoundCode);
				weaponData.StickingFrame = weaponComponent.PrimaryWeapon.StickingFrame;
				weaponData.CollisionShape = ((!needBatchedVersionForMeshes || string.IsNullOrEmpty(this.Item.CollisionBodyName)) ? null : PhysicsShape.GetFromResource(this.Item.CollisionBodyName, false));
				weaponData.Shape = ((!needBatchedVersionForMeshes || string.IsNullOrEmpty(this.Item.BodyName)) ? null : PhysicsShape.GetFromResource(this.Item.BodyName, false));
				weaponData.DataValue = this._dataValue;
				weaponData.CurrentUsageIndex = this.CurrentUsageIndex;
				int rangedUsageIndex = this.GetRangedUsageIndex();
				WeaponComponentData weaponComponentData;
				if (this.GetConsumableIfAny(out weaponComponentData))
				{
					weaponData.AirFrictionConstant = ItemObject.GetAirFrictionConstant(weaponComponentData.WeaponClass, weaponComponentData.WeaponFlags);
				}
				else if (rangedUsageIndex >= 0)
				{
					weaponData.AirFrictionConstant = ItemObject.GetAirFrictionConstant(this.GetWeaponComponentDataForUsage(rangedUsageIndex).WeaponClass, this.GetWeaponComponentDataForUsage(rangedUsageIndex).WeaponFlags);
				}
				weaponData.GlossMultiplier = this.GlossMultiplier;
				weaponData.HasLowerHolsterPriority = this.Item.HasLowerHolsterPriority;
				MissionWeapon.OnGetWeaponDataDelegate onGetWeaponDataHandler = MissionWeapon.OnGetWeaponDataHandler;
				if (onGetWeaponDataHandler != null)
				{
					onGetWeaponDataHandler(ref weaponData, this, false, this.Banner, needBatchedVersionForMeshes);
				}
				return weaponData;
			}
			return WeaponData.InvalidWeaponData;
		}

		public WeaponStatsData[] GetWeaponStatsData()
		{
			WeaponStatsData[] array = new WeaponStatsData[this._weapons.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.GetWeaponStatsDataForUsage(i);
			}
			return array;
		}

		public WeaponStatsData GetWeaponStatsDataForUsage(int usageIndex)
		{
			WeaponStatsData weaponStatsData = default(WeaponStatsData);
			WeaponComponentData weaponComponentData = this._weapons[usageIndex];
			weaponStatsData.WeaponClass = (int)weaponComponentData.WeaponClass;
			weaponStatsData.AmmoClass = (int)weaponComponentData.AmmoClass;
			weaponStatsData.Properties = (uint)this.Item.ItemFlags;
			weaponStatsData.WeaponFlags = (ulong)weaponComponentData.WeaponFlags;
			weaponStatsData.ItemUsageIndex = (string.IsNullOrEmpty(weaponComponentData.ItemUsage) ? (-1) : weaponComponentData.GetItemUsageIndex());
			weaponStatsData.ThrustSpeed = weaponComponentData.GetModifiedThrustSpeed(this.ItemModifier);
			weaponStatsData.SwingSpeed = weaponComponentData.GetModifiedSwingSpeed(this.ItemModifier);
			weaponStatsData.MissileSpeed = weaponComponentData.GetModifiedMissileSpeed(this.ItemModifier);
			weaponStatsData.ShieldArmor = weaponComponentData.GetModifiedArmor(this.ItemModifier);
			weaponStatsData.Accuracy = weaponComponentData.Accuracy;
			weaponStatsData.WeaponLength = weaponComponentData.WeaponLength;
			weaponStatsData.WeaponBalance = weaponComponentData.WeaponBalance;
			weaponStatsData.ThrustDamage = weaponComponentData.GetModifiedThrustDamage(this.ItemModifier);
			weaponStatsData.ThrustDamageType = (int)weaponComponentData.ThrustDamageType;
			weaponStatsData.SwingDamage = weaponComponentData.GetModifiedSwingDamage(this.ItemModifier);
			weaponStatsData.SwingDamageType = (int)weaponComponentData.SwingDamageType;
			weaponStatsData.DefendSpeed = weaponComponentData.GetModifiedHandling(this.ItemModifier);
			weaponStatsData.SweetSpot = weaponComponentData.SweetSpotReach;
			weaponStatsData.MaxDataValue = this._modifiedMaxDataValue;
			weaponStatsData.WeaponFrame = weaponComponentData.Frame;
			weaponStatsData.RotationSpeed = weaponComponentData.RotationSpeed;
			weaponStatsData.ReloadPhaseCount = weaponComponentData.ReloadPhaseCount;
			return weaponStatsData;
		}

		public WeaponData GetAmmoWeaponData(bool needBatchedVersion)
		{
			return this.AmmoWeapon.GetWeaponData(needBatchedVersion);
		}

		public WeaponStatsData[] GetAmmoWeaponStatsData()
		{
			return this.AmmoWeapon.GetWeaponStatsData();
		}

		public int GetAttachedWeaponsCount()
		{
			List<MissionWeapon.MissionSubWeapon> attachedWeapons = this._attachedWeapons;
			if (attachedWeapons == null)
			{
				return 0;
			}
			return attachedWeapons.Count;
		}

		public MissionWeapon GetAttachedWeapon(int attachmentIndex)
		{
			return this._attachedWeapons[attachmentIndex].Value;
		}

		public MatrixFrame GetAttachedWeaponFrame(int attachmentIndex)
		{
			return this._attachedWeaponFrames[attachmentIndex];
		}

		public bool IsShield()
		{
			return this._weapons.Count == 1 && this._weapons[0].IsShield;
		}

		public bool IsBanner()
		{
			return this._weapons.Count == 1 && this._weapons[0].WeaponClass == WeaponClass.Banner;
		}

		public bool IsAnyAmmo()
		{
			using (List<WeaponComponentData>.Enumerator enumerator = this._weapons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsAmmo)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasAnyUsageWithWeaponClass(WeaponClass weaponClass)
		{
			using (List<WeaponComponentData>.Enumerator enumerator = this._weapons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.WeaponClass == weaponClass)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasAnyUsageWithAmmoClass(WeaponClass ammoClass)
		{
			using (List<WeaponComponentData>.Enumerator enumerator = this._weapons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.AmmoClass == ammoClass)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasAllUsagesWithAnyWeaponFlag(WeaponFlags flags)
		{
			using (List<WeaponComponentData>.Enumerator enumerator = this._weapons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.WeaponFlags.HasAnyFlag(flags))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool HasAnyUsageWithoutWeaponFlag(WeaponFlags flags)
		{
			using (List<WeaponComponentData>.Enumerator enumerator = this._weapons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.WeaponFlags.HasAnyFlag(flags))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void GatherInformationFromWeapon(out bool weaponHasMelee, out bool weaponHasShield, out bool weaponHasPolearm, out bool weaponHasNonConsumableRanged, out bool weaponHasThrown, out WeaponClass rangedAmmoClass)
		{
			weaponHasMelee = false;
			weaponHasShield = false;
			weaponHasPolearm = false;
			weaponHasNonConsumableRanged = false;
			weaponHasThrown = false;
			rangedAmmoClass = WeaponClass.Undefined;
			foreach (WeaponComponentData weaponComponentData in this._weapons)
			{
				weaponHasMelee = weaponHasMelee || weaponComponentData.IsMeleeWeapon;
				weaponHasShield = weaponHasShield || weaponComponentData.IsShield;
				weaponHasPolearm = weaponComponentData.IsPolearm;
				if (weaponComponentData.IsRangedWeapon)
				{
					weaponHasThrown = weaponComponentData.IsConsumable;
					weaponHasNonConsumableRanged = !weaponHasThrown;
					rangedAmmoClass = weaponComponentData.AmmoClass;
				}
			}
		}

		public bool GetConsumableIfAny(out WeaponComponentData consumableWeapon)
		{
			consumableWeapon = null;
			if (this._hasAnyConsumableUsage)
			{
				foreach (WeaponComponentData weaponComponentData in this._weapons)
				{
					if (weaponComponentData.IsConsumable)
					{
						consumableWeapon = weaponComponentData;
						break;
					}
				}
				return true;
			}
			return false;
		}

		public bool IsAnyConsumable()
		{
			return this._hasAnyConsumableUsage;
		}

		public int GetRangedUsageIndex()
		{
			for (int i = 0; i < this._weapons.Count; i++)
			{
				if (this._weapons[i].IsRangedWeapon)
				{
					return i;
				}
			}
			return -1;
		}

		public MissionWeapon Consume(short count)
		{
			this.Amount -= count;
			return new MissionWeapon(this.Item, this.ItemModifier, this.Banner, count, 0, null);
		}

		public void ConsumeAmmo(short count)
		{
			if (count > 0)
			{
				MissionWeapon value = this._ammoWeapon.Value;
				value.Amount = count;
				this._ammoWeapon = new MissionWeapon.MissionSubWeapon(value);
				return;
			}
			this._ammoWeapon = null;
		}

		public void SetAmmo(MissionWeapon ammoWeapon)
		{
			this._ammoWeapon = new MissionWeapon.MissionSubWeapon(ammoWeapon);
		}

		public void ReloadAmmo(MissionWeapon ammoWeapon, short reloadPhase)
		{
			if (this._ammoWeapon != null && this._ammoWeapon.Value.Amount >= 0)
			{
				ammoWeapon.Amount += this._ammoWeapon.Value.Amount;
			}
			this._ammoWeapon = new MissionWeapon.MissionSubWeapon(ammoWeapon);
			this.ReloadPhase = reloadPhase;
		}

		public void AttachWeapon(MissionWeapon attachedWeapon, ref MatrixFrame attachFrame)
		{
			if (this._attachedWeapons == null)
			{
				this._attachedWeapons = new List<MissionWeapon.MissionSubWeapon>();
				this._attachedWeaponFrames = new List<MatrixFrame>();
			}
			this._attachedWeapons.Add(new MissionWeapon.MissionSubWeapon(attachedWeapon));
			this._attachedWeaponFrames.Add(attachFrame);
		}

		public void RemoveAttachedWeapon(int attachmentIndex)
		{
			this._attachedWeapons.RemoveAt(attachmentIndex);
			this._attachedWeaponFrames.RemoveAt(attachmentIndex);
		}

		public bool HasEnoughSpaceForAmount(int amount)
		{
			return (int)(this.ModifiedMaxAmount - this.Amount) >= amount;
		}

		public void SetRandomGlossMultiplier(int seed)
		{
			Random random = new Random(seed);
			float num = 1f + (random.NextFloat() * 2f - 1f) * 0.3f;
			this.GlossMultiplier = num;
		}

		public void AddExtraModifiedMaxValue(short extraValue)
		{
			this._modifiedMaxDataValue += extraValue;
		}

		public const short ReloadPhaseCountMax = 10;

		public static MissionWeapon.OnGetWeaponDataDelegate OnGetWeaponDataHandler;

		public static readonly MissionWeapon Invalid = new MissionWeapon(null, null, null);

		private readonly List<WeaponComponentData> _weapons;

		public int CurrentUsageIndex;

		private bool _hasAnyConsumableUsage;

		private short _dataValue;

		private short _modifiedMaxDataValue;

		private MissionWeapon.MissionSubWeapon _ammoWeapon;

		private List<MissionWeapon.MissionSubWeapon> _attachedWeapons;

		private List<MatrixFrame> _attachedWeaponFrames;

		public struct ImpactSoundModifier
		{
			public const string ModifierName = "impactModifier";

			public const float None = 0f;

			public const float ActiveBlock = 0.1f;

			public const float ChamberBlocked = 0.2f;

			public const float CrushThrough = 0.3f;
		}

		private class MissionSubWeapon
		{
			public MissionWeapon Value { get; private set; }

			public MissionSubWeapon(MissionWeapon subWeapon)
			{
				this.Value = subWeapon;
			}
		}

		public delegate void OnGetWeaponDataDelegate(ref WeaponData weaponData, MissionWeapon weapon, bool isFemale, Banner banner, bool needBatchedVersion);
	}
}
