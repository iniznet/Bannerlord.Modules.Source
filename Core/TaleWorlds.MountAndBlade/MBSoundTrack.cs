using System;

namespace TaleWorlds.MountAndBlade
{
	public struct MBSoundTrack
	{
		internal MBSoundTrack(int i)
		{
			this.index = i;
		}

		public bool Equals(MBSoundTrack a)
		{
			return this.index == a.index;
		}

		public override int GetHashCode()
		{
			return this.index;
		}

		private int index;
	}
}
