using System;

namespace TaleWorlds.MountAndBlade
{
	public struct MBParticleSystem
	{
		internal MBParticleSystem(int i)
		{
			this.index = i;
		}

		public bool Equals(MBParticleSystem a)
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
