using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200034E RID: 846
	public class FireBallista : Ballista
	{
		// Token: 0x06002D8C RID: 11660 RVA: 0x000B2CD8 File Offset: 0x000B0ED8
		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.FireBallista;
		}

		// Token: 0x06002D8D RID: 11661 RVA: 0x000B2CE0 File Offset: 0x000B0EE0
		public override float ProcessTargetValue(float baseValue, TargetFlags flags)
		{
			if (flags.HasAnyFlag(TargetFlags.NotAThreat))
			{
				return -1000f;
			}
			if (flags.HasAnyFlag(TargetFlags.None))
			{
				baseValue *= 1.5f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsSiegeEngine))
			{
				baseValue *= 2.5f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsStructure))
			{
				baseValue *= 0.1f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsFlammable))
			{
				baseValue *= 2f;
			}
			if (flags.HasAnyFlag(TargetFlags.DebugThreat))
			{
				baseValue *= 1000f;
			}
			return baseValue;
		}
	}
}
