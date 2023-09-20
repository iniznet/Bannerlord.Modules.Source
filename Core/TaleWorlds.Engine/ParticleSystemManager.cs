using System;

namespace TaleWorlds.Engine
{
	// Token: 0x0200006E RID: 110
	public sealed class ParticleSystemManager
	{
		// Token: 0x0600088C RID: 2188 RVA: 0x00008859 File Offset: 0x00006A59
		public static int GetRuntimeIdByName(string particleSystemName)
		{
			return EngineApplicationInterface.IParticleSystem.GetRuntimeIdByName(particleSystemName);
		}
	}
}
