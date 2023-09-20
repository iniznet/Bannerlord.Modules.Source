using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x0200000B RID: 11
	public class AtmosphereState
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00002CB0 File Offset: 0x00000EB0
		public AtmosphereState()
		{
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00002CE4 File Offset: 0x00000EE4
		public AtmosphereState(Vec3 position, float tempAv, float tempVar, float humAv, float humVar, string colorGradeTexture)
		{
			this.Position = position;
			this.TemperatureAverage = tempAv;
			this.TemperatureVariance = tempVar;
			this.HumidityAverage = humAv;
			this.HumidityVariance = humVar;
			this.ColorGradeTexture = colorGradeTexture;
		}

		// Token: 0x040000D0 RID: 208
		public Vec3 Position = Vec3.Zero;

		// Token: 0x040000D1 RID: 209
		public float TemperatureAverage;

		// Token: 0x040000D2 RID: 210
		public float TemperatureVariance;

		// Token: 0x040000D3 RID: 211
		public float HumidityAverage;

		// Token: 0x040000D4 RID: 212
		public float HumidityVariance;

		// Token: 0x040000D5 RID: 213
		public float distanceForMaxWeight = 1f;

		// Token: 0x040000D6 RID: 214
		public float distanceForMinWeight = 1f;

		// Token: 0x040000D7 RID: 215
		public string ColorGradeTexture = "";
	}
}
