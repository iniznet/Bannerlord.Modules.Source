using System;

namespace TaleWorlds.MountAndBlade
{
	public struct MissionObjectId
	{
		public MissionObjectId(int id, bool createdAtRuntime = false)
		{
			this.Id = id;
			this.CreatedAtRuntime = createdAtRuntime;
		}

		public static bool operator ==(MissionObjectId a, MissionObjectId b)
		{
			return a.Id == b.Id && a.CreatedAtRuntime == b.CreatedAtRuntime;
		}

		public static bool operator !=(MissionObjectId a, MissionObjectId b)
		{
			return a.Id != b.Id || a.CreatedAtRuntime != b.CreatedAtRuntime;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is MissionObjectId))
			{
				return false;
			}
			MissionObjectId missionObjectId = (MissionObjectId)obj;
			return missionObjectId.Id == this.Id && missionObjectId.CreatedAtRuntime == this.CreatedAtRuntime;
		}

		public override int GetHashCode()
		{
			int num = this.Id;
			if (this.CreatedAtRuntime)
			{
				num |= 1073741824;
			}
			return num.GetHashCode();
		}

		public override string ToString()
		{
			return this.Id + " - " + this.CreatedAtRuntime.ToString();
		}

		public readonly int Id;

		public readonly bool CreatedAtRuntime;

		public static readonly MissionObjectId Invalid = new MissionObjectId(-1, false);
	}
}
