using System;
using System.Diagnostics;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200017A RID: 378
	public class Threat
	{
		// Token: 0x0600136C RID: 4972 RVA: 0x0004C50C File Offset: 0x0004A70C
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x0600136D RID: 4973 RVA: 0x0004C514 File Offset: 0x0004A714
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

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x0600136E RID: 4974 RVA: 0x0004C584 File Offset: 0x0004A784
		public Vec3 Position
		{
			get
			{
				if (this.WeaponEntity != null)
				{
					return (this.WeaponEntity.GetTargetEntity().GlobalBoxMax + this.WeaponEntity.GetTargetEntity().GlobalBoxMin) * 0.5f + this.WeaponEntity.GetTargetingOffset();
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

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x0600136F RID: 4975 RVA: 0x0004C630 File Offset: 0x0004A830
		public Vec3 BoundingBoxMin
		{
			get
			{
				if (this.WeaponEntity != null)
				{
					return this.WeaponEntity.GetTargetEntity().GlobalBoxMin + this.WeaponEntity.GetTargetingOffset();
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

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06001370 RID: 4976 RVA: 0x0004C6AC File Offset: 0x0004A8AC
		public Vec3 BoundingBoxMax
		{
			get
			{
				if (this.WeaponEntity != null)
				{
					return this.WeaponEntity.GetTargetEntity().GlobalBoxMax + this.WeaponEntity.GetTargetingOffset();
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

		// Token: 0x06001371 RID: 4977 RVA: 0x0004C728 File Offset: 0x0004A928
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

		// Token: 0x06001372 RID: 4978 RVA: 0x0004C764 File Offset: 0x0004A964
		public override bool Equals(object obj)
		{
			Threat threat;
			return (threat = obj as Threat) != null && this.WeaponEntity == threat.WeaponEntity && this.Formation == threat.Formation;
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x0004C79B File Offset: 0x0004A99B
		[Conditional("DEBUG")]
		public void DisplayDebugInfo()
		{
		}

		// Token: 0x04000593 RID: 1427
		public ITargetable WeaponEntity;

		// Token: 0x04000594 RID: 1428
		public Formation Formation;

		// Token: 0x04000595 RID: 1429
		public Agent Agent;

		// Token: 0x04000596 RID: 1430
		public float ThreatValue;
	}
}
