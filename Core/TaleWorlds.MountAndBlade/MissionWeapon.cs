using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002D0 RID: 720
	public struct MissionWeapon
	{
		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x0600275D RID: 10077 RVA: 0x000973C7 File Offset: 0x000955C7
		// (set) Token: 0x0600275E RID: 10078 RVA: 0x000973CF File Offset: 0x000955CF
		public ItemObject Item { get; private set; }

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x0600275F RID: 10079 RVA: 0x000973D8 File Offset: 0x000955D8
		// (set) Token: 0x06002760 RID: 10080 RVA: 0x000973E0 File Offset: 0x000955E0
		public ItemModifier ItemModifier { get; private set; }

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x06002761 RID: 10081 RVA: 0x000973E9 File Offset: 0x000955E9
		public int WeaponsCount
		{
			get
			{
				return this._weapons.Count;
			}
		}

		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x06002762 RID: 10082 RVA: 0x000973F6 File Offset: 0x000955F6
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

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06002763 RID: 10083 RVA: 0x00097420 File Offset: 0x00095620
		// (set) Token: 0x06002764 RID: 10084 RVA: 0x00097428 File Offset: 0x00095628
		public short ReloadPhase { get; set; }

		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x06002765 RID: 10085 RVA: 0x00097434 File Offset: 0x00095634
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

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06002766 RID: 10086 RVA: 0x00097458 File Offset: 0x00095658
		public bool IsReloading
		{
			get
			{
				return this.ReloadPhase < this.ReloadPhaseCount;
			}
		}

		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x06002767 RID: 10087 RVA: 0x00097468 File Offset: 0x00095668
		// (set) Token: 0x06002768 RID: 10088 RVA: 0x00097470 File Offset: 0x00095670
		public Banner Banner { get; private set; }

		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x06002769 RID: 10089 RVA: 0x00097479 File Offset: 0x00095679
		// (set) Token: 0x0600276A RID: 10090 RVA: 0x00097481 File Offset: 0x00095681
		public float GlossMultiplier { get; private set; }

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x0600276B RID: 10091 RVA: 0x0009748A File Offset: 0x0009568A
		public short RawDataForNetwork
		{
			get
			{
				return this._dataValue;
			}
		}

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x0600276C RID: 10092 RVA: 0x00097492 File Offset: 0x00095692
		// (set) Token: 0x0600276D RID: 10093 RVA: 0x0009749A File Offset: 0x0009569A
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

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x0600276E RID: 10094 RVA: 0x000974A3 File Offset: 0x000956A3
		// (set) Token: 0x0600276F RID: 10095 RVA: 0x000974AB File Offset: 0x000956AB
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

		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x06002770 RID: 10096 RVA: 0x000974B4 File Offset: 0x000956B4
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

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x06002771 RID: 10097 RVA: 0x000974CC File Offset: 0x000956CC
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

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x06002772 RID: 10098 RVA: 0x000974E3 File Offset: 0x000956E3
		public short MaxAmmo
		{
			get
			{
				return this._modifiedMaxDataValue;
			}
		}

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06002773 RID: 10099 RVA: 0x000974EB File Offset: 0x000956EB
		public short ModifiedMaxAmount
		{
			get
			{
				return this._modifiedMaxDataValue;
			}
		}

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06002774 RID: 10100 RVA: 0x000974F3 File Offset: 0x000956F3
		public short ModifiedMaxHitPoints
		{
			get
			{
				return this._modifiedMaxDataValue;
			}
		}

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06002775 RID: 10101 RVA: 0x000974FB File Offset: 0x000956FB
		public bool IsEmpty
		{
			get
			{
				return this.CurrentUsageItem == null;
			}
		}

		// Token: 0x06002776 RID: 10102 RVA: 0x00097508 File Offset: 0x00095708
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

		// Token: 0x06002777 RID: 10103 RVA: 0x00097650 File Offset: 0x00095850
		public MissionWeapon(ItemObject primaryItem, ItemModifier itemModifier, Banner banner, short dataValue)
		{
			this = new MissionWeapon(primaryItem, itemModifier, banner);
			this._dataValue = dataValue;
		}

		// Token: 0x06002778 RID: 10104 RVA: 0x00097663 File Offset: 0x00095863
		public MissionWeapon(ItemObject primaryItem, ItemModifier itemModifier, Banner banner, short dataValue, short reloadPhase, MissionWeapon? ammoWeapon)
		{
			this = new MissionWeapon(primaryItem, itemModifier, banner, dataValue);
			this.ReloadPhase = reloadPhase;
			this._ammoWeapon = ((ammoWeapon != null) ? new MissionWeapon.MissionSubWeapon(ammoWeapon.Value) : null);
		}

		// Token: 0x06002779 RID: 10105 RVA: 0x00097696 File Offset: 0x00095896
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

		// Token: 0x0600277A RID: 10106 RVA: 0x000976CE File Offset: 0x000958CE
		public bool IsEqualTo(MissionWeapon other)
		{
			return this.Item == other.Item;
		}

		// Token: 0x0600277B RID: 10107 RVA: 0x000976DF File Offset: 0x000958DF
		public bool IsSameType(MissionWeapon other)
		{
			return this.Item.PrimaryWeapon.WeaponClass == other.Item.PrimaryWeapon.WeaponClass;
		}

		// Token: 0x0600277C RID: 10108 RVA: 0x00097704 File Offset: 0x00095904
		public float GetWeight()
		{
			float num = (this.Item.PrimaryWeapon.IsConsumable ? (this.GetBaseWeight() * (float)this._dataValue) : this.GetBaseWeight());
			MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
			return num + ((ammoWeapon != null) ? ammoWeapon.Value.GetWeight() : 0f);
		}

		// Token: 0x0600277D RID: 10109 RVA: 0x00097758 File Offset: 0x00095958
		private float GetBaseWeight()
		{
			return this.Item.Weight;
		}

		// Token: 0x0600277E RID: 10110 RVA: 0x00097765 File Offset: 0x00095965
		public WeaponComponentData GetWeaponComponentDataForUsage(int usageIndex)
		{
			return this._weapons[usageIndex];
		}

		// Token: 0x0600277F RID: 10111 RVA: 0x00097773 File Offset: 0x00095973
		public int GetGetModifiedArmorForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedArmor(this.ItemModifier);
		}

		// Token: 0x06002780 RID: 10112 RVA: 0x00097791 File Offset: 0x00095991
		public int GetModifiedThrustDamageForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedThrustDamage(this.ItemModifier);
		}

		// Token: 0x06002781 RID: 10113 RVA: 0x000977AF File Offset: 0x000959AF
		public int GetModifiedSwingDamageForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedSwingDamage(this.ItemModifier);
		}

		// Token: 0x06002782 RID: 10114 RVA: 0x000977CD File Offset: 0x000959CD
		public int GetModifiedMissileDamageForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedMissileDamage(this.ItemModifier);
		}

		// Token: 0x06002783 RID: 10115 RVA: 0x000977EB File Offset: 0x000959EB
		public int GetModifiedThrustSpeedForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedThrustSpeed(this.ItemModifier);
		}

		// Token: 0x06002784 RID: 10116 RVA: 0x00097809 File Offset: 0x00095A09
		public int GetModifiedSwingSpeedForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedSwingSpeed(this.ItemModifier);
		}

		// Token: 0x06002785 RID: 10117 RVA: 0x00097827 File Offset: 0x00095A27
		public int GetModifiedMissileSpeedForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedMissileSpeed(this.ItemModifier);
		}

		// Token: 0x06002786 RID: 10118 RVA: 0x00097845 File Offset: 0x00095A45
		public int GetModifiedMissileSpeedForUsage(int usageIndex)
		{
			return this._weapons[usageIndex].GetModifiedMissileSpeed(this.ItemModifier);
		}

		// Token: 0x06002787 RID: 10119 RVA: 0x0009785E File Offset: 0x00095A5E
		public int GetModifiedHandlingForCurrentUsage()
		{
			return this._weapons[this.CurrentUsageIndex].GetModifiedHandling(this.ItemModifier);
		}

		// Token: 0x06002788 RID: 10120 RVA: 0x0009787C File Offset: 0x00095A7C
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

		// Token: 0x06002789 RID: 10121 RVA: 0x00097B34 File Offset: 0x00095D34
		public WeaponStatsData[] GetWeaponStatsData()
		{
			WeaponStatsData[] array = new WeaponStatsData[this._weapons.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.GetWeaponStatsDataForUsage(i);
			}
			return array;
		}

		// Token: 0x0600278A RID: 10122 RVA: 0x00097B70 File Offset: 0x00095D70
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

		// Token: 0x0600278B RID: 10123 RVA: 0x00097CF0 File Offset: 0x00095EF0
		public WeaponData GetAmmoWeaponData(bool needBatchedVersion)
		{
			return this.AmmoWeapon.GetWeaponData(needBatchedVersion);
		}

		// Token: 0x0600278C RID: 10124 RVA: 0x00097D0C File Offset: 0x00095F0C
		public WeaponStatsData[] GetAmmoWeaponStatsData()
		{
			return this.AmmoWeapon.GetWeaponStatsData();
		}

		// Token: 0x0600278D RID: 10125 RVA: 0x00097D27 File Offset: 0x00095F27
		public int GetAttachedWeaponsCount()
		{
			List<MissionWeapon.MissionSubWeapon> attachedWeapons = this._attachedWeapons;
			if (attachedWeapons == null)
			{
				return 0;
			}
			return attachedWeapons.Count;
		}

		// Token: 0x0600278E RID: 10126 RVA: 0x00097D3A File Offset: 0x00095F3A
		public MissionWeapon GetAttachedWeapon(int attachmentIndex)
		{
			return this._attachedWeapons[attachmentIndex].Value;
		}

		// Token: 0x0600278F RID: 10127 RVA: 0x00097D4D File Offset: 0x00095F4D
		public MatrixFrame GetAttachedWeaponFrame(int attachmentIndex)
		{
			return this._attachedWeaponFrames[attachmentIndex];
		}

		// Token: 0x06002790 RID: 10128 RVA: 0x00097D5B File Offset: 0x00095F5B
		public bool IsShield()
		{
			return this._weapons.Count == 1 && this._weapons[0].IsShield;
		}

		// Token: 0x06002791 RID: 10129 RVA: 0x00097D7E File Offset: 0x00095F7E
		public bool IsBanner()
		{
			return this._weapons.Count == 1 && this._weapons[0].WeaponClass == WeaponClass.Banner;
		}

		// Token: 0x06002792 RID: 10130 RVA: 0x00097DA8 File Offset: 0x00095FA8
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

		// Token: 0x06002793 RID: 10131 RVA: 0x00097E04 File Offset: 0x00096004
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

		// Token: 0x06002794 RID: 10132 RVA: 0x00097E60 File Offset: 0x00096060
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

		// Token: 0x06002795 RID: 10133 RVA: 0x00097EBC File Offset: 0x000960BC
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

		// Token: 0x06002796 RID: 10134 RVA: 0x00097F1C File Offset: 0x0009611C
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

		// Token: 0x06002797 RID: 10135 RVA: 0x00097F7C File Offset: 0x0009617C
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

		// Token: 0x06002798 RID: 10136 RVA: 0x00098028 File Offset: 0x00096228
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

		// Token: 0x06002799 RID: 10137 RVA: 0x00098090 File Offset: 0x00096290
		public bool IsAnyConsumable()
		{
			return this._hasAnyConsumableUsage;
		}

		// Token: 0x0600279A RID: 10138 RVA: 0x00098098 File Offset: 0x00096298
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

		// Token: 0x0600279B RID: 10139 RVA: 0x000980D4 File Offset: 0x000962D4
		public MissionWeapon Consume(short count)
		{
			this.Amount -= count;
			return new MissionWeapon(this.Item, this.ItemModifier, this.Banner, count, 0, null);
		}

		// Token: 0x0600279C RID: 10140 RVA: 0x00098114 File Offset: 0x00096314
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

		// Token: 0x0600279D RID: 10141 RVA: 0x0009814D File Offset: 0x0009634D
		public void SetAmmo(MissionWeapon ammoWeapon)
		{
			this._ammoWeapon = new MissionWeapon.MissionSubWeapon(ammoWeapon);
		}

		// Token: 0x0600279E RID: 10142 RVA: 0x0009815C File Offset: 0x0009635C
		public void ReloadAmmo(MissionWeapon ammoWeapon, short reloadPhase)
		{
			if (this._ammoWeapon != null && this._ammoWeapon.Value.Amount >= 0)
			{
				ammoWeapon.Amount += this._ammoWeapon.Value.Amount;
			}
			this._ammoWeapon = new MissionWeapon.MissionSubWeapon(ammoWeapon);
			this.ReloadPhase = reloadPhase;
		}

		// Token: 0x0600279F RID: 10143 RVA: 0x000981BC File Offset: 0x000963BC
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

		// Token: 0x060027A0 RID: 10144 RVA: 0x00098209 File Offset: 0x00096409
		public void RemoveAttachedWeapon(int attachmentIndex)
		{
			this._attachedWeapons.RemoveAt(attachmentIndex);
			this._attachedWeaponFrames.RemoveAt(attachmentIndex);
		}

		// Token: 0x060027A1 RID: 10145 RVA: 0x00098223 File Offset: 0x00096423
		public bool HasEnoughSpaceForAmount(int amount)
		{
			return (int)(this.ModifiedMaxAmount - this.Amount) >= amount;
		}

		// Token: 0x060027A2 RID: 10146 RVA: 0x00098238 File Offset: 0x00096438
		public void SetRandomGlossMultiplier(int seed)
		{
			Random random = new Random(seed);
			float num = 1f + (random.NextFloat() * 2f - 1f) * 0.3f;
			this.GlossMultiplier = num;
		}

		// Token: 0x060027A3 RID: 10147 RVA: 0x00098272 File Offset: 0x00096472
		public void AddExtraModifiedMaxValue(short extraValue)
		{
			this._modifiedMaxDataValue += extraValue;
		}

		// Token: 0x04000E98 RID: 3736
		public const short ReloadPhaseCountMax = 10;

		// Token: 0x04000E99 RID: 3737
		public static MissionWeapon.OnGetWeaponDataDelegate OnGetWeaponDataHandler;

		// Token: 0x04000E9A RID: 3738
		public static readonly MissionWeapon Invalid = new MissionWeapon(null, null, null);

		// Token: 0x04000E9D RID: 3741
		private readonly List<WeaponComponentData> _weapons;

		// Token: 0x04000E9E RID: 3742
		public int CurrentUsageIndex;

		// Token: 0x04000E9F RID: 3743
		private bool _hasAnyConsumableUsage;

		// Token: 0x04000EA0 RID: 3744
		private short _dataValue;

		// Token: 0x04000EA1 RID: 3745
		private short _modifiedMaxDataValue;

		// Token: 0x04000EA5 RID: 3749
		private MissionWeapon.MissionSubWeapon _ammoWeapon;

		// Token: 0x04000EA6 RID: 3750
		private List<MissionWeapon.MissionSubWeapon> _attachedWeapons;

		// Token: 0x04000EA7 RID: 3751
		private List<MatrixFrame> _attachedWeaponFrames;

		// Token: 0x020005EA RID: 1514
		public struct ImpactSoundModifier
		{
			// Token: 0x04001EF8 RID: 7928
			public const string ModifierName = "impactModifier";

			// Token: 0x04001EF9 RID: 7929
			public const float None = 0f;

			// Token: 0x04001EFA RID: 7930
			public const float ActiveBlock = 0.1f;

			// Token: 0x04001EFB RID: 7931
			public const float ChamberBlocked = 0.2f;

			// Token: 0x04001EFC RID: 7932
			public const float CrushThrough = 0.3f;
		}

		// Token: 0x020005EB RID: 1515
		private class MissionSubWeapon
		{
			// Token: 0x170009B7 RID: 2487
			// (get) Token: 0x06003CCE RID: 15566 RVA: 0x000F194C File Offset: 0x000EFB4C
			// (set) Token: 0x06003CCF RID: 15567 RVA: 0x000F1954 File Offset: 0x000EFB54
			public MissionWeapon Value { get; private set; }

			// Token: 0x06003CD0 RID: 15568 RVA: 0x000F195D File Offset: 0x000EFB5D
			public MissionSubWeapon(MissionWeapon subWeapon)
			{
				this.Value = subWeapon;
			}
		}

		// Token: 0x020005EC RID: 1516
		// (Invoke) Token: 0x06003CD2 RID: 15570
		public delegate void OnGetWeaponDataDelegate(ref WeaponData weaponData, MissionWeapon weapon, bool isFemale, Banner banner, bool needBatchedVersion);
	}
}
