﻿using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public class WeaponComponentData
	{
		internal static void AutoGeneratedStaticCollectObjectsWeaponComponentData(object o, List<object> collectedObjects)
		{
			((WeaponComponentData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		public WeaponComponentData.WeaponTiers WeaponTier { get; private set; }

		public string WeaponDescriptionId { get; private set; }

		public int BodyArmor { get; private set; }

		public string PhysicsMaterial { get; private set; }

		public string FlyingSoundCode { get; private set; }

		public string PassbySoundCode { get; private set; }

		public string ItemUsage { get; private set; }

		public int ThrustSpeed { get; private set; }

		public int SwingSpeed { get; private set; }

		public int MissileSpeed { get; private set; }

		public int WeaponLength { get; private set; }

		public float WeaponBalance { get; private set; }

		public int ThrustDamage { get; private set; }

		public DamageTypes ThrustDamageType { get; private set; }

		public int SwingDamage { get; private set; }

		public DamageTypes SwingDamageType { get; private set; }

		public int Accuracy { get; private set; }

		public WeaponClass WeaponClass { get; private set; }

		public WeaponClass AmmoClass { get; private set; }

		public int MissileDamage
		{
			get
			{
				return this.ThrustDamage;
			}
		}

		public float Inertia { get; private set; }

		public float CenterOfMass { get; private set; }

		public Vec3 CenterOfMass3D { get; private set; }

		public float SwingDamageFactor { get; private set; }

		public float ThrustDamageFactor { get; private set; }

		public int Handling { get; private set; }

		public float SweetSpotReach { get; private set; }

		public string TrailParticleName { get; private set; }

		public MatrixFrame StickingFrame { get; private set; }

		public Vec3 AmmoOffset { get; private set; }

		public short MaxDataValue { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public Vec3 RotationSpeed { get; private set; }

		public short ReloadPhaseCount { get; private set; }

		public void Init(string weaponUsageName, string physicsMaterial, string itemUsage, DamageTypes thrustDamageType, DamageTypes swingDamageType, int bodyArmor, int weaponLength, float weaponBalance, float inertia, float centerOfMass, int handling, float swingDamageFactor, float thrustDamageFactor, short maxDataValue, string passBySoundCode, int accuracy, int missileSpeed, MatrixFrame stickingFrame, WeaponClass ammoClass, float sweetSpot, int swingSpeed, int swingDamage, int thrustSpeed, int thrustDamage, Vec3 rotationSpeed, WeaponComponentData.WeaponTiers tier, short reloadPhaseCount)
		{
			this.WeaponDescriptionId = weaponUsageName;
			this.PhysicsMaterial = physicsMaterial;
			this.ItemUsage = itemUsage;
			this.ThrustDamageType = thrustDamageType;
			this.SwingDamageType = swingDamageType;
			this.BodyArmor = bodyArmor;
			this.WeaponLength = weaponLength;
			this.WeaponBalance = weaponBalance;
			this.Inertia = inertia;
			this.CenterOfMass = centerOfMass;
			this.Handling = handling;
			this.SwingDamageFactor = swingDamageFactor;
			this.ThrustDamageFactor = thrustDamageFactor;
			this.MaxDataValue = maxDataValue;
			this.PassbySoundCode = passBySoundCode;
			this.Accuracy = accuracy;
			this.MissileSpeed = missileSpeed;
			this.StickingFrame = stickingFrame;
			this.AmmoClass = ammoClass;
			this.SweetSpotReach = sweetSpot;
			this.SwingSpeed = swingSpeed;
			this.SwingDamage = swingDamage;
			this.ThrustSpeed = thrustSpeed;
			this.ThrustDamage = thrustDamage;
			this.Frame = MatrixFrame.Identity;
			this.CenterOfMass3D = new Vec3(0f, 0f, centerOfMass, -1f);
			this.RotationSpeed = rotationSpeed;
			this.WeaponTier = tier;
			this.ReloadPhaseCount = reloadPhaseCount;
		}

		public bool IsMeleeWeapon
		{
			get
			{
				return this.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon);
			}
		}

		public bool IsRangedWeapon
		{
			get
			{
				return this.WeaponFlags.HasAllFlags(WeaponFlags.RangedWeapon);
			}
		}

		public bool IsPolearm
		{
			get
			{
				return this.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip);
			}
		}

		public bool IsConsumable
		{
			get
			{
				return this.WeaponFlags.HasAllFlags(WeaponFlags.Consumable);
			}
		}

		public bool IsAmmo
		{
			get
			{
				return !this.WeaponFlags.HasAnyFlag(WeaponFlags.WeaponMask) && this.IsConsumable;
			}
		}

		public bool IsShield
		{
			get
			{
				return !this.WeaponFlags.HasAnyFlag(WeaponFlags.WeaponMask) && this.WeaponFlags.HasAllFlags(WeaponFlags.HasHitPoints | WeaponFlags.CanBlockRanged);
			}
		}

		public bool IsTwoHanded
		{
			get
			{
				return this.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand);
			}
		}

		public bool IsOneHanded
		{
			get
			{
				return this.WeaponFlags.HasFlag(WeaponFlags.MeleeWeapon) && !this.IsTwoHanded;
			}
		}

		public bool IsBow
		{
			get
			{
				return this.WeaponFlags.HasAllFlags((WeaponFlags)527360UL);
			}
		}

		public bool IsCrossBow
		{
			get
			{
				return this.WeaponFlags.HasFlag(WeaponFlags.HasString) && !this.IsBow;
			}
		}

		public void SetFrame(MatrixFrame frame)
		{
			this.Frame = frame;
		}

		public void SetAmmoOffset(Vec3 ammoOffset)
		{
			this.AmmoOffset = ammoOffset;
		}

		public SkillObject RelevantSkill
		{
			get
			{
				return WeaponComponentData.GetRelevantSkillFromWeaponClass(this.WeaponClass);
			}
		}

		public bool CanHitMultipleTargets
		{
			get
			{
				return this.WeaponClass == WeaponClass.TwoHandedAxe || this.WeaponClass == WeaponClass.TwoHandedMace;
			}
		}

		public static SkillObject GetRelevantSkillFromWeaponClass(WeaponClass weaponClass)
		{
			SkillObject skillObject = null;
			switch (weaponClass)
			{
			case WeaponClass.Dagger:
			case WeaponClass.OneHandedSword:
			case WeaponClass.OneHandedAxe:
			case WeaponClass.Mace:
				skillObject = DefaultSkills.OneHanded;
				break;
			case WeaponClass.TwoHandedSword:
			case WeaponClass.TwoHandedAxe:
			case WeaponClass.TwoHandedMace:
				skillObject = DefaultSkills.TwoHanded;
				break;
			case WeaponClass.OneHandedPolearm:
			case WeaponClass.TwoHandedPolearm:
			case WeaponClass.LowGripPolearm:
				skillObject = DefaultSkills.Polearm;
				break;
			case WeaponClass.Arrow:
			case WeaponClass.Bow:
				skillObject = DefaultSkills.Bow;
				break;
			case WeaponClass.Bolt:
			case WeaponClass.Crossbow:
				skillObject = DefaultSkills.Crossbow;
				break;
			case WeaponClass.Stone:
			case WeaponClass.Boulder:
			case WeaponClass.ThrowingAxe:
			case WeaponClass.ThrowingKnife:
			case WeaponClass.Javelin:
				skillObject = DefaultSkills.Throwing;
				break;
			case WeaponClass.SmallShield:
			case WeaponClass.LargeShield:
				skillObject = DefaultSkills.OneHanded;
				break;
			}
			return skillObject;
		}

		public static ItemObject.ItemTypeEnum GetItemTypeFromWeaponClass(WeaponClass weaponClass)
		{
			switch (weaponClass)
			{
			case WeaponClass.Undefined:
			case WeaponClass.NumClasses:
				return ItemObject.ItemTypeEnum.Invalid;
			case WeaponClass.Dagger:
			case WeaponClass.OneHandedSword:
			case WeaponClass.OneHandedAxe:
			case WeaponClass.Mace:
				return ItemObject.ItemTypeEnum.OneHandedWeapon;
			case WeaponClass.TwoHandedSword:
			case WeaponClass.TwoHandedAxe:
			case WeaponClass.Pick:
			case WeaponClass.TwoHandedMace:
				return ItemObject.ItemTypeEnum.TwoHandedWeapon;
			case WeaponClass.OneHandedPolearm:
			case WeaponClass.TwoHandedPolearm:
			case WeaponClass.LowGripPolearm:
				return ItemObject.ItemTypeEnum.Polearm;
			case WeaponClass.Arrow:
				return ItemObject.ItemTypeEnum.Arrows;
			case WeaponClass.Bolt:
				return ItemObject.ItemTypeEnum.Bolts;
			case WeaponClass.Cartridge:
				return ItemObject.ItemTypeEnum.Bullets;
			case WeaponClass.Bow:
				return ItemObject.ItemTypeEnum.Bow;
			case WeaponClass.Crossbow:
				return ItemObject.ItemTypeEnum.Crossbow;
			case WeaponClass.Stone:
			case WeaponClass.Boulder:
			case WeaponClass.ThrowingAxe:
			case WeaponClass.ThrowingKnife:
			case WeaponClass.Javelin:
				return ItemObject.ItemTypeEnum.Thrown;
			case WeaponClass.Pistol:
				return ItemObject.ItemTypeEnum.Pistol;
			case WeaponClass.Musket:
				return ItemObject.ItemTypeEnum.Musket;
			case WeaponClass.SmallShield:
			case WeaponClass.LargeShield:
				return ItemObject.ItemTypeEnum.Shield;
			case WeaponClass.Banner:
				return ItemObject.ItemTypeEnum.Banner;
			default:
				return ItemObject.ItemTypeEnum.Invalid;
			}
		}

		public WeaponComponentData(ItemObject item, WeaponClass weaponClass = WeaponClass.Undefined, WeaponFlags weaponFlags = (WeaponFlags)0UL)
		{
			this.BodyArmor = 0;
			this.PhysicsMaterial = "";
			this.FlyingSoundCode = "";
			this.PassbySoundCode = "";
			this.ItemUsage = null;
			this.SwingSpeed = 0;
			this.ThrustSpeed = 0;
			this.MissileSpeed = 0;
			this.WeaponLength = 0;
			this.ThrustDamage = 0;
			this.SwingDamage = 0;
			this.AmmoOffset = Vec3.Zero;
			this.Accuracy = 0;
			this.StickingFrame = MatrixFrame.Identity;
			this.TrailParticleName = "";
			this.WeaponClass = weaponClass;
			this.WeaponFlags = weaponFlags;
			this.Frame = MatrixFrame.Identity;
			this.RotationSpeed = Vec3.Zero;
			this.ReloadPhaseCount = 0;
		}

		public void Deserialize(ItemObject item, XmlNode node)
		{
			this.BodyArmor = ((node.Attributes["body_armor"] != null) ? int.Parse(node.Attributes["body_armor"].Value) : 0);
			XmlAttribute xmlAttribute = node.Attributes["physics_material"];
			this.PhysicsMaterial = ((xmlAttribute != null) ? xmlAttribute.Value : null);
			XmlAttribute xmlAttribute2 = node.Attributes["flying_sound_code"];
			this.FlyingSoundCode = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
			XmlAttribute xmlAttribute3 = node.Attributes["passby_sound_code"];
			this.PassbySoundCode = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
			XmlAttribute xmlAttribute4 = node.Attributes["item_usage"];
			this.ItemUsage = ((xmlAttribute4 != null) ? xmlAttribute4.Value : null);
			this.WeaponBalance = ((node.Attributes["weapon_balance"] != null) ? ((float)int.Parse(node.Attributes["weapon_balance"].Value) * 0.01f) : 0f);
			this.SwingSpeed = ((node.Attributes["speed_rating"] != null) ? int.Parse(node.Attributes["speed_rating"].Value) : 0);
			this.ThrustSpeed = ((node.Attributes["thrust_speed"] != null) ? int.Parse(node.Attributes["thrust_speed"].Value) : 0);
			this.MissileSpeed = ((node.Attributes["missile_speed"] != null) ? int.Parse(node.Attributes["missile_speed"].Value) : 0);
			this.WeaponLength = ((node.Attributes["weapon_length"] != null) ? int.Parse(node.Attributes["weapon_length"].Value) : 0);
			this.ThrustDamage = ((node.Attributes["thrust_damage"] != null) ? int.Parse(node.Attributes["thrust_damage"].Value) : 0);
			this.SwingDamage = ((node.Attributes["swing_damage"] != null) ? int.Parse(node.Attributes["swing_damage"].Value) : 0);
			this.Accuracy = ((node.Attributes["accuracy"] != null) ? int.Parse(node.Attributes["accuracy"].Value) : 100);
			this.ThrustDamageType = ((node.Attributes["thrust_damage_type"] != null) ? ((DamageTypes)Enum.Parse(typeof(DamageTypes), node.Attributes["thrust_damage_type"].Value)) : DamageTypes.Blunt);
			this.SwingDamageType = ((node.Attributes["swing_damage_type"] != null) ? ((DamageTypes)Enum.Parse(typeof(DamageTypes), node.Attributes["swing_damage_type"].Value)) : DamageTypes.Blunt);
			this.WeaponClass = ((node.Attributes["weapon_class"] != null) ? ((WeaponClass)Enum.Parse(typeof(WeaponClass), node.Attributes["weapon_class"].Value)) : WeaponClass.Undefined);
			this.AmmoClass = ((node.Attributes["ammo_class"] != null) ? ((WeaponClass)Enum.Parse(typeof(WeaponClass), node.Attributes["ammo_class"].Value)) : WeaponClass.Undefined);
			this.ReloadPhaseCount = ((node.Attributes["reload_phase_count"] != null) ? short.Parse(node.Attributes["reload_phase_count"].Value) : 1);
			this.CenterOfMass = (float)this.WeaponLength * 0.5f * 0.01f;
			this.CenterOfMass3D = ((node.Attributes["center_of_mass"] != null) ? Vec3.Parse(node.Attributes["center_of_mass"].Value) : Vec3.Zero);
			if (this.WeaponClass != WeaponClass.Bow && this.WeaponClass != WeaponClass.Crossbow && this.WeaponClass != WeaponClass.SmallShield && this.WeaponClass != WeaponClass.LargeShield && this.WeaponClass != WeaponClass.Arrow && this.WeaponClass != WeaponClass.Bolt && this.WeaponClass != WeaponClass.ThrowingKnife && this.WeaponClass != WeaponClass.ThrowingAxe && this.WeaponClass != WeaponClass.Javelin && this.WeaponClass != WeaponClass.Stone)
			{
				WeaponClass weaponClass = this.WeaponClass;
			}
			XmlAttribute xmlAttribute5 = node.Attributes["ammo_limit"];
			XmlAttribute xmlAttribute6 = node.Attributes["stack_amount"];
			XmlAttribute xmlAttribute7 = node.Attributes["hit_points"];
			if (xmlAttribute5 != null)
			{
				this.MaxDataValue = short.Parse(xmlAttribute5.Value);
			}
			else if (xmlAttribute6 != null)
			{
				this.MaxDataValue = short.Parse(xmlAttribute6.Value);
			}
			else if (xmlAttribute7 != null)
			{
				this.MaxDataValue = short.Parse(xmlAttribute7.Value);
			}
			else
			{
				this.MaxDataValue = 0;
			}
			Vec3 vec = default(Vec3);
			Mat3 identity = Mat3.Identity;
			XmlNode xmlNode = node.Attributes["sticking_position"];
			if (xmlNode != null)
			{
				string[] array = xmlNode.Value.Split(new char[] { ',' });
				if (array.Length == 3)
				{
					float.TryParse(array[0], out vec.x);
					float.TryParse(array[1], out vec.y);
					float.TryParse(array[2], out vec.z);
				}
			}
			XmlNode xmlNode2 = node.Attributes["sticking_rotation"];
			if (xmlNode2 != null)
			{
				string[] array2 = xmlNode2.Value.Split(new char[] { ',' });
				if (array2.Length == 3)
				{
					float num;
					float.TryParse(array2[0], out num);
					float num2;
					float.TryParse(array2[1], out num2);
					float num3;
					float.TryParse(array2[2], out num3);
					identity.RotateAboutSide(num.ToRadians());
					identity.RotateAboutUp(num2.ToRadians());
					identity.RotateAboutForward(num3.ToRadians());
				}
			}
			vec = identity.TransformToParent(vec);
			this.StickingFrame = new MatrixFrame(identity, vec);
			Vec3 vec2 = default(Vec3);
			Mat3 identity2 = Mat3.Identity;
			XmlNode xmlNode3 = node.Attributes["position"];
			if (xmlNode3 != null)
			{
				string[] array3 = xmlNode3.Value.Split(new char[] { ',' });
				if (array3.Length == 3)
				{
					float.TryParse(array3[0], out vec2.x);
					float.TryParse(array3[1], out vec2.y);
					float.TryParse(array3[2], out vec2.z);
				}
			}
			XmlNode xmlNode4 = node.Attributes["rotation"];
			if (xmlNode4 != null)
			{
				string[] array4 = xmlNode4.Value.Split(new char[] { ',' });
				if (array4.Length == 3)
				{
					float num4;
					float.TryParse(array4[0], out num4);
					float num5;
					float.TryParse(array4[1], out num5);
					float num6;
					float.TryParse(array4[2], out num6);
					identity2.RotateAboutUp(num6.ToRadians());
					identity2.RotateAboutSide(num4.ToRadians());
					identity2.RotateAboutForward(num5.ToRadians());
				}
			}
			this.Frame = new MatrixFrame(identity2, vec2);
			this.RotationSpeed = ((node.Attributes["rotation_speed"] != null) ? Vec3.Parse(node.Attributes["rotation_speed"].Value) : Vec3.Zero);
			XmlAttribute xmlAttribute8 = node.Attributes["trail_particle_name"];
			this.TrailParticleName = ((xmlAttribute8 != null) ? xmlAttribute8.Value : null);
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode5 = (XmlNode)obj;
				if (xmlNode5.Name == "WeaponFlags")
				{
					foreach (object obj2 in Enum.GetValues(typeof(WeaponFlags)))
					{
						WeaponFlags weaponFlags = (WeaponFlags)obj2;
						if (xmlNode5.Attributes[weaponFlags.ToString()] != null)
						{
							this.WeaponFlags |= weaponFlags;
						}
					}
				}
			}
			this.Inertia = item.Weight * 0.05f;
			this.Handling = this.ThrustSpeed;
			this.SweetSpotReach = 0.93f;
			this.SetDamageFactors(item.Weight);
		}

		private void SetDamageFactors(float weight)
		{
			DamageTypes swingDamageType = this.SwingDamageType;
			if (this.WeaponClass == WeaponClass.Bow || this.WeaponClass == WeaponClass.Crossbow || this.WeaponClass == WeaponClass.ThrowingAxe || this.WeaponClass == WeaponClass.ThrowingKnife || this.WeaponClass == WeaponClass.Javelin || this.WeaponClass == WeaponClass.Arrow || this.WeaponClass == WeaponClass.Bolt)
			{
				this.SwingDamageFactor = 1f;
				this.ThrustDamageFactor = 1f;
				return;
			}
			float num = MathF.Sqrt(MathF.Sqrt(weight / ((float)this.WeaponLength * 0.01f)));
			float num2 = 0f;
			switch (swingDamageType)
			{
			case DamageTypes.Cut:
				num *= 0.8f;
				num2 = 0.5f;
				break;
			case DamageTypes.Pierce:
				num *= 0.7f;
				num2 = 0.4f;
				break;
			case DamageTypes.Blunt:
				num *= 1f;
				num2 = 1f;
				break;
			}
			num *= 0.8f;
			num2 *= 0.8f;
			this.SwingDamageFactor = num2;
			this.ThrustDamageFactor = num2;
		}

		public float GetRealWeaponLength()
		{
			return (float)this.WeaponLength * 0.01f + Vec3.DotProduct(this.Frame.rotation.u, this.Frame.origin);
		}

		public MatrixFrame GetMissileStartingFrame()
		{
			MatrixFrame identity;
			if (this.WeaponClass == WeaponClass.Arrow || this.WeaponClass == WeaponClass.Bolt)
			{
				Mat3 mat = new Mat3(1f, 0f, 0f, 0f, 0f, -1f, 0f, 1f, 0f);
				identity.rotation = mat;
				identity.origin = Vec3.Zero;
			}
			else
			{
				identity = MatrixFrame.Identity;
				if (this.WeaponClass == WeaponClass.ThrowingAxe)
				{
					identity.rotation.RotateAboutUp(-1.5707964f);
				}
				else if (this.WeaponClass == WeaponClass.ThrowingKnife)
				{
					identity.rotation.RotateAboutUp(-1.5707964f);
				}
				else
				{
					Mat3 mat2 = new Mat3(1f, 0f, 0f, 0f, 0f, -1f, 0f, 1f, 0f);
					identity.rotation = mat2;
				}
			}
			return identity;
		}

		public WeaponFlags WeaponFlags;

		public enum WeaponTiers
		{
			Tier1,
			Tier2,
			Tier3,
			Tier4,
			Special
		}
	}
}
