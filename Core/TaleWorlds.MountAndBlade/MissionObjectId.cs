using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000249 RID: 585
	public struct MissionObjectId
	{
		// Token: 0x06001FDE RID: 8158 RVA: 0x00071036 File Offset: 0x0006F236
		public MissionObjectId(int id, bool createdAtRuntime = false)
		{
			this.Id = id;
			this.CreatedAtRuntime = createdAtRuntime;
		}

		// Token: 0x06001FDF RID: 8159 RVA: 0x00071046 File Offset: 0x0006F246
		public static bool operator ==(MissionObjectId a, MissionObjectId b)
		{
			return a.Id == b.Id && a.CreatedAtRuntime == b.CreatedAtRuntime;
		}

		// Token: 0x06001FE0 RID: 8160 RVA: 0x00071066 File Offset: 0x0006F266
		public static bool operator !=(MissionObjectId a, MissionObjectId b)
		{
			return a.Id != b.Id || a.CreatedAtRuntime != b.CreatedAtRuntime;
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x0007108C File Offset: 0x0006F28C
		public override bool Equals(object obj)
		{
			if (!(obj is MissionObjectId))
			{
				return false;
			}
			MissionObjectId missionObjectId = (MissionObjectId)obj;
			return missionObjectId.Id == this.Id && missionObjectId.CreatedAtRuntime == this.CreatedAtRuntime;
		}

		// Token: 0x06001FE2 RID: 8162 RVA: 0x000710C8 File Offset: 0x0006F2C8
		public override int GetHashCode()
		{
			int num = this.Id;
			if (this.CreatedAtRuntime)
			{
				num |= 1073741824;
			}
			return num.GetHashCode();
		}

		// Token: 0x06001FE3 RID: 8163 RVA: 0x000710F4 File Offset: 0x0006F2F4
		public override string ToString()
		{
			return this.Id + " - " + this.CreatedAtRuntime.ToString();
		}

		// Token: 0x04000BD1 RID: 3025
		public readonly int Id;

		// Token: 0x04000BD2 RID: 3026
		public readonly bool CreatedAtRuntime;
	}
}
