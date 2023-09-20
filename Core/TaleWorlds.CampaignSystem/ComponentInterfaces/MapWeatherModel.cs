using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class MapWeatherModel : GameModel
	{
		public abstract AtmosphereState GetInterpolatedAtmosphereState(CampaignTime timeOfYear, Vec3 pos);

		public abstract AtmosphereInfo GetAtmosphereModel(CampaignTime timeOfYear, Vec3 pos);

		public abstract float GetSeasonTimeFactor();

		public abstract void LogAtmosphere(AtmosphereInfo atmosphere);

		public abstract bool GetIsSnowTerrainInPos(Vec3 pos);
	}
}
