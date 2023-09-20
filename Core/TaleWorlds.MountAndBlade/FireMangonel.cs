using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200034F RID: 847
	public class FireMangonel : Mangonel
	{
		// Token: 0x06002D8F RID: 11663 RVA: 0x000B2D64 File Offset: 0x000B0F64
		public override SiegeEngineType GetSiegeEngineType()
		{
			if (this._defaultSide != BattleSideEnum.Attacker)
			{
				return DefaultSiegeEngineTypes.FireCatapult;
			}
			return DefaultSiegeEngineTypes.FireOnager;
		}

		// Token: 0x06002D90 RID: 11664 RVA: 0x000B2D7C File Offset: 0x000B0F7C
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
				baseValue *= 12f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsStructure))
			{
				baseValue *= 1.5f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsSmall))
			{
				baseValue *= 8f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsMoving))
			{
				baseValue *= 12f;
			}
			if (flags.HasAnyFlag(TargetFlags.DebugThreat))
			{
				baseValue *= 10f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsSiegeTower))
			{
				baseValue *= 12f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsFlammable))
			{
				baseValue *= 100f;
			}
			return baseValue;
		}
	}
}
