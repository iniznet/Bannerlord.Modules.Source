using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200017F RID: 383
	[EngineStruct("Hit_particle_result_data")]
	public struct HitParticleResultData
	{
		// Token: 0x060013A9 RID: 5033 RVA: 0x0004D180 File Offset: 0x0004B380
		public void Reset()
		{
			this.StartHitParticleIndex = -1;
			this.ContinueHitParticleIndex = -1;
			this.EndHitParticleIndex = -1;
		}

		// Token: 0x04000616 RID: 1558
		public int StartHitParticleIndex;

		// Token: 0x04000617 RID: 1559
		public int ContinueHitParticleIndex;

		// Token: 0x04000618 RID: 1560
		public int EndHitParticleIndex;
	}
}
