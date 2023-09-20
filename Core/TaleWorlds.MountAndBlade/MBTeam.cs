using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001CA RID: 458
	public struct MBTeam
	{
		// Token: 0x06001A12 RID: 6674 RVA: 0x0005C435 File Offset: 0x0005A635
		internal MBTeam(Mission mission, int index)
		{
			this._mission = mission;
			this.Index = index;
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06001A13 RID: 6675 RVA: 0x0005C445 File Offset: 0x0005A645
		public static MBTeam InvalidTeam
		{
			get
			{
				return new MBTeam(null, -1);
			}
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x0005C44E File Offset: 0x0005A64E
		public override int GetHashCode()
		{
			return this.Index;
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x0005C456 File Offset: 0x0005A656
		public override bool Equals(object obj)
		{
			return ((MBTeam)obj).Index == this.Index;
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x0005C46B File Offset: 0x0005A66B
		public static bool operator ==(MBTeam team1, MBTeam team2)
		{
			return team1.Index == team2.Index;
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x0005C47B File Offset: 0x0005A67B
		public static bool operator !=(MBTeam team1, MBTeam team2)
		{
			return team1.Index != team2.Index;
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06001A18 RID: 6680 RVA: 0x0005C48E File Offset: 0x0005A68E
		public bool IsValid
		{
			get
			{
				return this.Index >= 0;
			}
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x0005C49C File Offset: 0x0005A69C
		public bool IsEnemyOf(MBTeam otherTeam)
		{
			return MBAPI.IMBTeam.IsEnemy(this._mission.Pointer, this.Index, otherTeam.Index);
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x0005C4BF File Offset: 0x0005A6BF
		public void SetIsEnemyOf(MBTeam otherTeam, bool isEnemyOf)
		{
			MBAPI.IMBTeam.SetIsEnemy(this._mission.Pointer, this.Index, otherTeam.Index, isEnemyOf);
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x0005C4E3 File Offset: 0x0005A6E3
		public override string ToString()
		{
			return "Mission Team: " + this.Index;
		}

		// Token: 0x0400082E RID: 2094
		public readonly int Index;

		// Token: 0x0400082F RID: 2095
		private readonly Mission _mission;
	}
}
