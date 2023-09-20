using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001B8 RID: 440
	public abstract class MBMissile
	{
		// Token: 0x06001988 RID: 6536 RVA: 0x0005B932 File Offset: 0x00059B32
		protected MBMissile(Mission mission)
		{
			this._mission = mission;
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06001989 RID: 6537 RVA: 0x0005B941 File Offset: 0x00059B41
		// (set) Token: 0x0600198A RID: 6538 RVA: 0x0005B949 File Offset: 0x00059B49
		public int Index { get; set; }

		// Token: 0x0600198B RID: 6539 RVA: 0x0005B952 File Offset: 0x00059B52
		public Vec3 GetPosition()
		{
			return MBAPI.IMBMission.GetPositionOfMissile(this._mission.Pointer, this.Index);
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x0005B96F File Offset: 0x00059B6F
		public Vec3 GetVelocity()
		{
			return MBAPI.IMBMission.GetVelocityOfMissile(this._mission.Pointer, this.Index);
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x0005B98C File Offset: 0x00059B8C
		public bool GetHasRigidBody()
		{
			return MBAPI.IMBMission.GetMissileHasRigidBody(this._mission.Pointer, this.Index);
		}

		// Token: 0x040007C7 RID: 1991
		private readonly Mission _mission;
	}
}
