using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000192 RID: 402
	public abstract class MapWeatherModel : GameModel
	{
		// Token: 0x060019FF RID: 6655
		public abstract AtmosphereState GetInterpolatedAtmosphereState(CampaignTime timeOfYear, Vec3 pos);

		// Token: 0x06001A00 RID: 6656
		public abstract AtmosphereInfo GetAtmosphereModel(CampaignTime timeOfYear, Vec3 pos);

		// Token: 0x06001A01 RID: 6657
		public abstract float GetSeasonTimeFactor();

		// Token: 0x06001A02 RID: 6658
		public abstract void LogAtmosphere(AtmosphereInfo atmosphere);

		// Token: 0x06001A03 RID: 6659
		public abstract bool GetIsSnowTerrainInPos(Vec3 pos);
	}
}
