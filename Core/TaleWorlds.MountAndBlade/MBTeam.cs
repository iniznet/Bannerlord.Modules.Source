using System;

namespace TaleWorlds.MountAndBlade
{
	public struct MBTeam
	{
		internal MBTeam(Mission mission, int index)
		{
			this._mission = mission;
			this.Index = index;
		}

		public static MBTeam InvalidTeam
		{
			get
			{
				return new MBTeam(null, -1);
			}
		}

		public override int GetHashCode()
		{
			return this.Index;
		}

		public override bool Equals(object obj)
		{
			return ((MBTeam)obj).Index == this.Index;
		}

		public static bool operator ==(MBTeam team1, MBTeam team2)
		{
			return team1.Index == team2.Index;
		}

		public static bool operator !=(MBTeam team1, MBTeam team2)
		{
			return team1.Index != team2.Index;
		}

		public bool IsValid
		{
			get
			{
				return this.Index >= 0;
			}
		}

		public bool IsEnemyOf(MBTeam otherTeam)
		{
			return MBAPI.IMBTeam.IsEnemy(this._mission.Pointer, this.Index, otherTeam.Index);
		}

		public void SetIsEnemyOf(MBTeam otherTeam, bool isEnemyOf)
		{
			MBAPI.IMBTeam.SetIsEnemy(this._mission.Pointer, this.Index, otherTeam.Index, isEnemyOf);
		}

		public override string ToString()
		{
			return "Mission Team: " + this.Index;
		}

		public readonly int Index;

		private readonly Mission _mission;
	}
}
