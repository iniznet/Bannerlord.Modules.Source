﻿using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Weapon_data", "TaleWorlds.MountAndBlade.AutoGenerated.WeaponDataAsNative", false)]
	public struct WeaponData
	{
		public bool IsValid()
		{
			return this.WeaponKind >= 0;
		}

		public ItemObject GetItemObject()
		{
			return ((this.WeaponKind >= 0) ? MBObjectManager.Instance.GetObject(new MBGUID((uint)this.WeaponKind)) : null) as ItemObject;
		}

		public void DeinitializeManagedPointers()
		{
			if (this.WeaponMesh != null)
			{
				this.WeaponMesh.ManualInvalidate();
			}
			if (this.HolsterMesh != null)
			{
				this.HolsterMesh.ManualInvalidate();
			}
			if (this.HolsterMeshWithWeapon != null)
			{
				this.HolsterMeshWithWeapon.ManualInvalidate();
			}
			if (this.FlyingMesh != null)
			{
				this.FlyingMesh.ManualInvalidate();
			}
		}

		public MetaMesh WeaponMesh;

		public MetaMesh HolsterMesh;

		public MetaMesh HolsterMeshWithWeapon;

		public MetaMesh FlyingMesh;

		[CustomEngineStructMemberData("prefab_name")]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Prefab;

		[CustomEngineStructMemberData("body")]
		public PhysicsShape Shape;

		[CustomEngineStructMemberData("collision_body")]
		public PhysicsShape CollisionShape;

		public Material TableauMaterial;

		public MatrixFrame WeaponFrame;

		public int PhysicsMaterialIndex;

		public int WeaponKind;

		public StackArray.StackArray4Int ItemHolsterIndices;

		public int Difficulty;

		public float BaseWeight;

		public float Inertia;

		public short ReloadPhase;

		public bool HasFlagAnimation;

		public Vec3 AmmoOffset;

		public MatrixFrame StickingFrame;

		public float ScaleFactor;

		public float CenterOfMass;

		[CustomEngineStructMemberData("center_of_mass_3d")]
		public Vec3 CenterOfMass3D;

		public Vec3 HolsterPositionShift;

		public int FlyingSoundCode;

		public int PassbySoundCode;

		[CustomEngineStructMemberData("flying_missile_trail_particle_name")]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string TrailParticleName;

		public Vec3 CenterOfMassShift;

		public short DataValue;

		public int CurrentUsageIndex;

		public float AirFrictionConstant;

		public bool HasLowerHolsterPriority;

		public float GlossMultiplier;

		public static WeaponData InvalidWeaponData = new WeaponData
		{
			WeaponKind = -1
		};
	}
}
