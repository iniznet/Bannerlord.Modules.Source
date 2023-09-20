using System;

namespace TaleWorlds.MountAndBlade
{
	public struct MBMusicTrack
	{
		public MBMusicTrack(MBMusicTrack obj)
		{
			this.index = obj.index;
		}

		internal MBMusicTrack(int i)
		{
			this.index = i;
		}

		private bool IsValid
		{
			get
			{
				return this.index >= 0;
			}
		}

		public bool Equals(MBMusicTrack obj)
		{
			return this.index == obj.index;
		}

		public override int GetHashCode()
		{
			return this.index;
		}

		private int index;
	}
}
