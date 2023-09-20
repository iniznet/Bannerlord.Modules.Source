using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001C2 RID: 450
	public struct MBParticleSystem
	{
		// Token: 0x060019E1 RID: 6625 RVA: 0x0005C1A9 File Offset: 0x0005A3A9
		internal MBParticleSystem(int i)
		{
			this.index = i;
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x0005C1B2 File Offset: 0x0005A3B2
		public bool Equals(MBParticleSystem a)
		{
			return this.index == a.index;
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x0005C1C2 File Offset: 0x0005A3C2
		public override int GetHashCode()
		{
			return this.index;
		}

		// Token: 0x04000816 RID: 2070
		private int index;
	}
}
