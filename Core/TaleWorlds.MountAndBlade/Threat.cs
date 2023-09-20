using System;
using System.Diagnostics;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class Threat
	{
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public string Name
		{
			get
			{
				if (this.WeaponEntity != null)
				{
					return this.WeaponEntity.Entity().Name;
				}
				if (this.Agent != null)
				{
					return this.Agent.Name.ToString();
				}
				if (this.Formation != null)
				{
					return this.Formation.ToString();
				}
				Debug.FailedAssert("Invalid threat", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Threat.cs", "Name", 38);
				return "Invalid";
			}
		}

		public Vec3 Position
		{
			get
			{
				if (this.WeaponEntity != null)
				{
					return (this.WeaponEntity.GetTargetEntity().PhysicsGlobalBoxMax + this.WeaponEntity.GetTargetEntity().PhysicsGlobalBoxMin) * 0.5f + this.WeaponEntity.GetTargetingOffset();
				}
				if (this.Agent != null)
				{
					return this.Agent.CollisionCapsuleCenter;
				}
				if (this.Formation != null)
				{
					return this.Formation.GetMedianAgent(false, false, this.Formation.GetAveragePositionOfUnits(false, false)).Position;
				}
				Debug.FailedAssert("Invalid threat", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Threat.cs", "Position", 62);
				return Vec3.Invalid;
			}
		}

		public Vec3 BoundingBoxMin
		{
			get
			{
				if (this.WeaponEntity != null)
				{
					return this.WeaponEntity.GetTargetEntity().PhysicsGlobalBoxMin + this.WeaponEntity.GetTargetingOffset();
				}
				if (this.Agent != null)
				{
					return this.Agent.CollisionCapsule.GetBoxMin();
				}
				if (this.Formation != null)
				{
					Debug.FailedAssert("Nobody should be requesting a bounding box for a formation", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Threat.cs", "BoundingBoxMin", 82);
					return Vec3.Invalid;
				}
				return Vec3.Invalid;
			}
		}

		public Vec3 BoundingBoxMax
		{
			get
			{
				if (this.WeaponEntity != null)
				{
					return this.WeaponEntity.GetTargetEntity().PhysicsGlobalBoxMax + this.WeaponEntity.GetTargetingOffset();
				}
				if (this.Agent != null)
				{
					return this.Agent.CollisionCapsule.GetBoxMax();
				}
				if (this.Formation != null)
				{
					Debug.FailedAssert("Nobody should be requesting a bounding box for a formation", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Threat.cs", "BoundingBoxMax", 106);
					return Vec3.Invalid;
				}
				return Vec3.Invalid;
			}
		}

		public Vec3 GetVelocity()
		{
			if (this.WeaponEntity != null)
			{
				Vec3 zero = Vec3.Zero;
				IMoveableSiegeWeapon moveableSiegeWeapon = this.WeaponEntity as IMoveableSiegeWeapon;
				if (moveableSiegeWeapon != null)
				{
					return moveableSiegeWeapon.MovementComponent.Velocity;
				}
			}
			return Vec3.Zero;
		}

		public override bool Equals(object obj)
		{
			Threat threat;
			return (threat = obj as Threat) != null && this.WeaponEntity == threat.WeaponEntity && this.Formation == threat.Formation;
		}

		[Conditional("DEBUG")]
		public void DisplayDebugInfo()
		{
		}

		public ITargetable WeaponEntity;

		public Formation Formation;

		public Agent Agent;

		public float ThreatValue;
	}
}
