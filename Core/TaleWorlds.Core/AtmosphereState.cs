using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public class AtmosphereState
	{
		public AtmosphereState()
		{
		}

		public AtmosphereState(Vec3 position, float tempAv, float tempVar, float humAv, float humVar, string colorGradeTexture)
		{
			this.Position = position;
			this.TemperatureAverage = tempAv;
			this.TemperatureVariance = tempVar;
			this.HumidityAverage = humAv;
			this.HumidityVariance = humVar;
			this.ColorGradeTexture = colorGradeTexture;
		}

		public Vec3 Position = Vec3.Zero;

		public float TemperatureAverage;

		public float TemperatureVariance;

		public float HumidityAverage;

		public float HumidityVariance;

		public float distanceForMaxWeight = 1f;

		public float distanceForMinWeight = 1f;

		public string ColorGradeTexture = "";
	}
}
