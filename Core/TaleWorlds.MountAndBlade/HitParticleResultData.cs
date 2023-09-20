using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Hit_particle_result_data", false)]
	public struct HitParticleResultData
	{
		public void Reset()
		{
			this.StartHitParticleIndex = -1;
			this.ContinueHitParticleIndex = -1;
			this.EndHitParticleIndex = -1;
		}

		public int StartHitParticleIndex;

		public int ContinueHitParticleIndex;

		public int EndHitParticleIndex;
	}
}
