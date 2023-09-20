using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MBMissile
	{
		protected MBMissile(Mission mission)
		{
			this._mission = mission;
		}

		public int Index { get; set; }

		public Vec3 GetPosition()
		{
			return MBAPI.IMBMission.GetPositionOfMissile(this._mission.Pointer, this.Index);
		}

		public Vec3 GetVelocity()
		{
			return MBAPI.IMBMission.GetVelocityOfMissile(this._mission.Pointer, this.Index);
		}

		public bool GetHasRigidBody()
		{
			return MBAPI.IMBMission.GetMissileHasRigidBody(this._mission.Pointer, this.Index);
		}

		private readonly Mission _mission;
	}
}
