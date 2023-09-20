using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200011A RID: 282
	public class DefaultMapWeatherModel : MapWeatherModel
	{
		// Token: 0x0600160B RID: 5643 RVA: 0x00068ED5 File Offset: 0x000670D5
		public void InitializeSnowAmountData(byte[] snowAmountData)
		{
			this._snowAmountData = snowAmountData;
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x00068EDE File Offset: 0x000670DE
		public override AtmosphereState GetInterpolatedAtmosphereState(CampaignTime timeOfYear, Vec3 pos)
		{
			if (this._atmosphereGrid == null)
			{
				this._atmosphereGrid = new AtmosphereGrid();
				this._atmosphereGrid.Initialize();
			}
			return this._atmosphereGrid.GetInterpolatedStateInfo(pos);
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x00068F0C File Offset: 0x0006710C
		public override AtmosphereInfo GetAtmosphereModel(CampaignTime timeOfYear, Vec3 pos)
		{
			float hourOfDayNormalized = this.GetHourOfDayNormalized();
			float seasonTimeFactor = this.GetSeasonTimeFactor();
			DefaultMapWeatherModel.SunPosition sunPosition = this.GetSunPosition(hourOfDayNormalized, seasonTimeFactor);
			float environmentMultiplier = this.GetEnvironmentMultiplier(sunPosition, seasonTimeFactor);
			float num = this.GetModifiedEnvironmentMultiplier(environmentMultiplier);
			num = MathF.Max(MathF.Pow(num, 1.5f), 0.001f);
			Vec3 sunColor = this.GetSunColor(environmentMultiplier);
			int num2 = -1;
			string text = "field_battle";
			if (Settlement.CurrentSettlement != null && !Settlement.CurrentSettlement.IsHideout)
			{
				text = Settlement.CurrentSettlement.Culture.StringId;
				if (text != "empire" && text != "aserai" && text != "sturgia" && text != "vlandia" && text != "khuzait" && text != "battania")
				{
					text = "field_battle";
				}
				if (text == "aserai")
				{
					num2 = 1;
				}
			}
			AtmosphereState interpolatedAtmosphereState = this.GetInterpolatedAtmosphereState(timeOfYear, pos);
			float temperature = this.GetTemperature(ref interpolatedAtmosphereState, seasonTimeFactor);
			float humidity = this.GetHumidity(ref interpolatedAtmosphereState, seasonTimeFactor);
			int num3 = 0;
			if (humidity > 20f)
			{
				num3 = 1;
			}
			if (humidity > 40f)
			{
				num3 = 2;
			}
			if (humidity > 60f)
			{
				num3 = 3;
			}
			int num4 = timeOfYear.GetSeasonOfYear;
			float num5 = 0f;
			if (num2 != -1)
			{
				num4 = num2;
			}
			else
			{
				float normalizedSnowValueInPos = this.GetNormalizedSnowValueInPos(pos);
				if ((double)normalizedSnowValueInPos > 0.55)
				{
					num4 = 3;
					num5 = MBMath.SmoothStep(0.6f, 1f, normalizedSnowValueInPos);
				}
				else if (num4 == 3)
				{
					num4 = 1;
				}
			}
			AtmosphereInfo atmosphereInfo = new AtmosphereInfo();
			atmosphereInfo.SunInfo.Altitude = sunPosition.Altitude;
			atmosphereInfo.SunInfo.Angle = sunPosition.Angle;
			atmosphereInfo.SunInfo.Color = sunColor;
			atmosphereInfo.SunInfo.Brightness = this.GetSunBrightness(environmentMultiplier, false);
			atmosphereInfo.SunInfo.Size = this.GetSunSize(environmentMultiplier);
			atmosphereInfo.SunInfo.RayStrength = this.GetSunRayStrength(environmentMultiplier);
			atmosphereInfo.SunInfo.MaxBrightness = this.GetSunBrightness(1f, true);
			atmosphereInfo.RainInfo.Density = num5;
			atmosphereInfo.SnowInfo.Density = num5;
			atmosphereInfo.AmbientInfo.EnvironmentMultiplier = MathF.Max(num * 0.5f, 0.001f);
			atmosphereInfo.AmbientInfo.AmbientColor = this.GetAmbientFogColor(num);
			atmosphereInfo.AmbientInfo.MieScatterStrength = this.GetMieScatterStrength(environmentMultiplier);
			atmosphereInfo.AmbientInfo.RayleighConstant = this.GetRayleighConstant(environmentMultiplier);
			atmosphereInfo.SkyInfo.Brightness = this.GetSkyBrightness(hourOfDayNormalized, environmentMultiplier);
			atmosphereInfo.FogInfo.Density = this.GetFogDensity(environmentMultiplier, pos);
			atmosphereInfo.FogInfo.Color = this.GetFogColor(num);
			atmosphereInfo.FogInfo.Falloff = 1.48f;
			atmosphereInfo.TimeInfo.TimeOfDay = this.GetHourOfDay();
			atmosphereInfo.TimeInfo.WinterTimeFactor = this.GetWinterTimeFactor(timeOfYear);
			atmosphereInfo.TimeInfo.DrynessFactor = this.GetDrynessFactor(timeOfYear);
			atmosphereInfo.TimeInfo.NightTimeFactor = this.GetNightTimeFactor();
			atmosphereInfo.TimeInfo.Season = num4;
			atmosphereInfo.AreaInfo.Temperature = temperature;
			atmosphereInfo.AreaInfo.Humidity = humidity;
			atmosphereInfo.AreaInfo.AreaType = num3;
			atmosphereInfo.PostProInfo.MinExposure = MBMath.Lerp(-3f, -2f, this.GetExposureCoefBetweenDayNight(), 1E-05f);
			atmosphereInfo.PostProInfo.MaxExposure = MBMath.Lerp(2f, 0f, num, 1E-05f);
			atmosphereInfo.PostProInfo.BrightpassThreshold = MBMath.Lerp(0.7f, 0.9f, num, 1E-05f);
			atmosphereInfo.PostProInfo.MiddleGray = 0.1f;
			atmosphereInfo.AtmosphereTypeName = text;
			return atmosphereInfo;
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x000692CD File Offset: 0x000674CD
		public override void LogAtmosphere(AtmosphereInfo atmoshere)
		{
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x000692D0 File Offset: 0x000674D0
		private DefaultMapWeatherModel.SunPosition GetSunPosition(float hourNorm, float seasonFactor)
		{
			float num2;
			float num3;
			if (hourNorm >= 0.083333336f && hourNorm < 0.9166667f)
			{
				this._sunIsMoon = false;
				float num = (hourNorm - 0.083333336f) / 0.8333334f;
				num2 = MBMath.Lerp(0f, 180f, num, 1E-05f);
				num3 = 50f * seasonFactor;
			}
			else
			{
				this._sunIsMoon = true;
				if (hourNorm >= 0.9166667f)
				{
					hourNorm -= 1f;
				}
				float num4 = (hourNorm - -0.08333331f) / 0.16666666f;
				num4 = ((num4 < 0f) ? 0f : ((num4 > 1f) ? 1f : num4));
				num2 = MBMath.Lerp(180f, 0f, num4, 1E-05f);
				num3 = 50f * seasonFactor;
			}
			return new DefaultMapWeatherModel.SunPosition(num3, num2);
		}

		// Token: 0x06001610 RID: 5648 RVA: 0x0006939C File Offset: 0x0006759C
		private Vec3 GetSunColor(float environmentMultiplier)
		{
			Vec3 vec;
			if (!this._sunIsMoon)
			{
				vec = new Vec3(1f, 1f - (1f - MathF.Pow(environmentMultiplier, 0.3f)) / 2f, 0.9f - (1f - MathF.Pow(environmentMultiplier, 0.3f)) / 2.5f, -1f);
			}
			else
			{
				vec = new Vec3(0.85f - MathF.Pow(environmentMultiplier, 0.4f), 0.8f - MathF.Pow(environmentMultiplier, 0.5f), 0.8f - MathF.Pow(environmentMultiplier, 0.8f), -1f);
				vec = Vec3.Vec3Max(vec, new Vec3(0.05f, 0.05f, 0.1f, -1f));
			}
			return vec;
		}

		// Token: 0x06001611 RID: 5649 RVA: 0x00069460 File Offset: 0x00067660
		private float GetSunBrightness(float environmentMultiplier, bool forceDay = false)
		{
			float num;
			if (!this._sunIsMoon || forceDay)
			{
				num = MathF.Sin(MathF.Pow((environmentMultiplier - 0.001f) / 0.999f, 1.2f) * 1.5707964f) * 85f;
				num = MathF.Min(MathF.Max(num, 0.2f), 35f);
			}
			else
			{
				num = 0.2f;
			}
			return num;
		}

		// Token: 0x06001612 RID: 5650 RVA: 0x000694C2 File Offset: 0x000676C2
		private float GetSunSize(float envMultiplier)
		{
			return 0.1f + (1f - envMultiplier) / 8f;
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x000694D8 File Offset: 0x000676D8
		private float GetSunRayStrength(float envMultiplier)
		{
			return MathF.Min(MathF.Max(MathF.Sin(MathF.Pow((envMultiplier - 0.001f) / 0.999f, 0.4f) * 3.1415927f / 2f) - 0.15f, 0.01f), 0.5f);
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x00069527 File Offset: 0x00067727
		private float GetRainDensity(float humidity, float temperature)
		{
			if (temperature <= 0f)
			{
				return 0f;
			}
			if (humidity >= 40f)
			{
				return MBMath.InverseLerp(40f, 100f, humidity);
			}
			return 0f;
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x00069555 File Offset: 0x00067755
		private float GetSnowDensity(float humidity, float temperature)
		{
			if (temperature > 0f)
			{
				return 0f;
			}
			if (humidity >= 40f)
			{
				return MBMath.InverseLerp(40f, 100f, humidity);
			}
			return 0f;
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x00069584 File Offset: 0x00067784
		private float GetEnvironmentMultiplier(DefaultMapWeatherModel.SunPosition sunPos, float seasonFactor)
		{
			float num;
			if (this._sunIsMoon)
			{
				num = sunPos.Altitude / 180f * 2f;
			}
			else
			{
				num = sunPos.Altitude / 180f * 2f;
			}
			num = ((num > 1f) ? (2f - num) : num);
			num = MathF.Pow(num, 0.5f);
			float num2 = 1f - 0.011111111f * sunPos.Angle;
			float num3 = MBMath.ClampFloat(num * num2, 0f, 1f);
			return MBMath.ClampFloat(MathF.Min(MathF.Sin(num3 * num3) * 2f, 1f), 0f, 1f) * 0.999f + 0.001f;
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x0006963C File Offset: 0x0006783C
		private float GetModifiedEnvironmentMultiplier(float envMultiplier)
		{
			float num;
			if (!this._sunIsMoon)
			{
				num = (envMultiplier - 0.001f) / 0.999f;
				num = num * 0.999f + 0.001f;
			}
			else
			{
				num = (envMultiplier - 0.001f) / 0.999f;
				num = num * 0f + 0.001f;
			}
			return num;
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x0006968C File Offset: 0x0006788C
		private float GetSkyBrightness(float hourNorm, float envMultiplier)
		{
			float num = (envMultiplier - 0.001f) / 0.999f;
			float num2;
			if (!this._sunIsMoon)
			{
				num2 = MathF.Sin(MathF.Pow(num, 1.3f) * 1.5707964f) * 80f;
				num2 -= 1f;
				num2 = MathF.Min(MathF.Max(num2, 0.055f), 25f);
			}
			else
			{
				num2 = 0.055f;
			}
			return num2;
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x000696FC File Offset: 0x000678FC
		private float GetFogDensity(float environmentMultiplier, Vec3 pos)
		{
			float num = (this._sunIsMoon ? 0.5f : 0.4f);
			float num2 = 1f - environmentMultiplier;
			float num3 = 1f - MBMath.ClampFloat((pos.z - 30f) / 200f, 0f, 0.9f);
			return MathF.Min((0f + num * num2) * num3, 10f);
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x00069764 File Offset: 0x00067964
		private Vec3 GetFogColor(float environmentMultiplier)
		{
			Vec3 vec;
			if (!this._sunIsMoon)
			{
				vec = new Vec3(1f - (1f - environmentMultiplier) / 7f, 0.75f - environmentMultiplier / 4f, 0.55f - environmentMultiplier / 5f, -1f);
			}
			else
			{
				vec = new Vec3(1f - environmentMultiplier * 10f, 0.75f + environmentMultiplier * 1.5f, 0.65f + environmentMultiplier * 2f, -1f);
				vec = Vec3.Vec3Max(vec, new Vec3(0.55f, 0.59f, 0.6f, -1f));
			}
			return vec;
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x00069808 File Offset: 0x00067A08
		private Vec3 GetAmbientFogColor(float moddedEnvMul)
		{
			return Vec3.Vec3Min(new Vec3(0.15f, 0.3f, 0.5f, -1f) + new Vec3(moddedEnvMul / 3f, moddedEnvMul / 2f, moddedEnvMul / 1.5f, -1f), new Vec3(1f, 1f, 1f, -1f));
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x00069870 File Offset: 0x00067A70
		private float GetMieScatterStrength(float envMultiplier)
		{
			return (1f + (1f - envMultiplier)) * 10f;
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x00069888 File Offset: 0x00067A88
		private float GetRayleighConstant(float envMultiplier)
		{
			float num = (envMultiplier - 0.001f) / 0.999f;
			return MathF.Min(MathF.Max(1f - MathF.Sin(MathF.Pow(num, 0.45f) * 3.1415927f / 2f) + (0.14f + num * 2f), 0.65f), 0.99f);
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x000698E8 File Offset: 0x00067AE8
		private float GetHourOfDay()
		{
			return (float)(CampaignTime.Now.ToHours % 24.0);
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x0006990D File Offset: 0x00067B0D
		private float GetHourOfDayNormalized()
		{
			return this.GetHourOfDay() / 24f;
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x0006991C File Offset: 0x00067B1C
		private float GetNightTimeFactor()
		{
			float num = this.GetHourOfDay() - 2f;
			for (num %= 24f; num < 0f; num += 24f)
			{
			}
			num = MathF.Max(num - 20f, 0f);
			return MathF.Min(num / 0.1f, 1f);
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x00069978 File Offset: 0x00067B78
		private float GetExposureCoefBetweenDayNight()
		{
			float hourOfDay = this.GetHourOfDay();
			float num = 0f;
			if (hourOfDay > 2f && hourOfDay < 4f)
			{
				num = 1f - (hourOfDay - 2f) / 2f;
			}
			if (hourOfDay < 22f && hourOfDay > 20f)
			{
				num = (hourOfDay - 20f) / 2f;
			}
			if (hourOfDay > 22f || hourOfDay < 2f)
			{
				num = 1f;
			}
			return num;
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x000699EC File Offset: 0x00067BEC
		private float GetNormalizedSnowValueInPos(Vec3 pos)
		{
			Vec2 terrainSize = Campaign.Current.MapSceneWrapper.GetTerrainSize();
			int num = MathF.Floor(pos.x / terrainSize.X * 1024f);
			int num2 = MathF.Floor(pos.y / terrainSize.Y * 1024f);
			num = MBMath.ClampIndex(num, 0, 1024);
			num2 = MBMath.ClampIndex(num2, 0, 1024);
			float num3 = (float)this._snowAmountData[num2 * 1024 + num] / 255f;
			float num4 = MBMath.Lerp(0.55f, -0.1f, Campaign.Current.Models.MapWeatherModel.GetSeasonTimeFactor(), 1E-05f);
			float num5 = MBMath.SmoothStep(num4 - 0.65f, num4 + 0.65f, num3);
			return MBMath.Lerp(0f, num5, num5, 1E-05f);
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x00069AC4 File Offset: 0x00067CC4
		public override bool GetIsSnowTerrainInPos(Vec3 pos)
		{
			return this.GetNormalizedSnowValueInPos(pos) > 0.55f;
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x00069AD4 File Offset: 0x00067CD4
		private float GetWinterTimeFactor(CampaignTime timeOfYear)
		{
			float num = 0f;
			if (timeOfYear.GetSeasonOfYear == 3)
			{
				float num2 = MathF.Abs((float)Math.IEEERemainder(CampaignTime.Now.ToSeasons, 1.0));
				num = MBMath.SplitLerp(0f, 0.35f, 0f, 0.5f, num2, 1E-05f);
			}
			return num;
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x00069B34 File Offset: 0x00067D34
		private float GetDrynessFactor(CampaignTime timeOfYear)
		{
			float num = 0f;
			float num2 = MathF.Abs((float)Math.IEEERemainder(CampaignTime.Now.ToSeasons, 1.0));
			switch (timeOfYear.GetSeasonOfYear)
			{
			case 1:
			{
				float num3 = MBMath.ClampFloat(num2 * 2f, 0f, 1f);
				num = MBMath.Lerp(0f, 1f, num3, 1E-05f);
				break;
			}
			case 2:
				num = 1f;
				break;
			case 3:
				num = MBMath.Lerp(1f, 0f, num2, 1E-05f);
				break;
			}
			return num;
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x00069BD8 File Offset: 0x00067DD8
		public override float GetSeasonTimeFactor()
		{
			float num = (float)CampaignTime.Now.ToSeasons % 4f;
			float num2 = 0f;
			if (num > 1.5f && (double)num <= 3.5)
			{
				float num3 = (num - 1.5f) / 2f;
				num2 = MBMath.Lerp(0f, 1f, num3, 1E-05f);
			}
			else if (num <= 1.5f)
			{
				float num3 = num / 1.5f;
				num2 = MBMath.Lerp(0.75f, 0f, num3, 1E-05f);
			}
			else if (num > 3.5f)
			{
				float num3 = (num - 3.5f) * 2f;
				num2 = MBMath.Lerp(1f, 0.75f, num3, 1E-05f);
			}
			return num2;
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x00069C98 File Offset: 0x00067E98
		private float GetTemperature(ref AtmosphereState gridInfo, float seasonFactor)
		{
			if (gridInfo == null)
			{
				return 0f;
			}
			float temperatureAverage = gridInfo.TemperatureAverage;
			float num = (seasonFactor - 0.5f) * -2f;
			float num2 = gridInfo.TemperatureVariance * num;
			return temperatureAverage + num2;
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x00069CD0 File Offset: 0x00067ED0
		private float GetHumidity(ref AtmosphereState gridInfo, float seasonFactor)
		{
			if (gridInfo == null)
			{
				return 0f;
			}
			float humidityAverage = gridInfo.HumidityAverage;
			float num = (seasonFactor - 0.5f) * 2f;
			float num2 = gridInfo.HumidityVariance * num;
			return MBMath.ClampFloat(humidityAverage + num2, 0f, 100f);
		}

		// Token: 0x040007B0 RID: 1968
		private byte[] _snowAmountData = new byte[1048576];

		// Token: 0x040007B1 RID: 1969
		private const float _sunRiseNorm = 0.083333336f;

		// Token: 0x040007B2 RID: 1970
		private const float _sunSetNorm = 0.9166667f;

		// Token: 0x040007B3 RID: 1971
		private const float _dayTime = 20f;

		// Token: 0x040007B4 RID: 1972
		private const float _nightTime = 4f;

		// Token: 0x040007B5 RID: 1973
		private const float _dayTimeNorm = 0.8333333f;

		// Token: 0x040007B6 RID: 1974
		private const float _nightTimeNorm = 0.16666667f;

		// Token: 0x040007B7 RID: 1975
		private const float _nightTimeBeforeMidnightNorm = 0.08333331f;

		// Token: 0x040007B8 RID: 1976
		private const float _minSunAngle = 0f;

		// Token: 0x040007B9 RID: 1977
		private const float _maxSunAngle = 50f;

		// Token: 0x040007BA RID: 1978
		private const float _minEnvMultiplier = 0.001f;

		// Token: 0x040007BB RID: 1979
		private const float _dayEnvMulFactor = 1f;

		// Token: 0x040007BC RID: 1980
		private const float _nightEnvMulFactor = 0.001f;

		// Token: 0x040007BD RID: 1981
		private bool _sunIsMoon;

		// Token: 0x040007BE RID: 1982
		private const float _maxSnowCoverage = 0.35f;

		// Token: 0x040007BF RID: 1983
		private AtmosphereGrid _atmosphereGrid;

		// Token: 0x0200050C RID: 1292
		private struct SunPosition
		{
			// Token: 0x17000D94 RID: 3476
			// (get) Token: 0x06004235 RID: 16949 RVA: 0x00134D8F File Offset: 0x00132F8F
			// (set) Token: 0x06004236 RID: 16950 RVA: 0x00134D97 File Offset: 0x00132F97
			public float Angle { get; private set; }

			// Token: 0x17000D95 RID: 3477
			// (get) Token: 0x06004237 RID: 16951 RVA: 0x00134DA0 File Offset: 0x00132FA0
			// (set) Token: 0x06004238 RID: 16952 RVA: 0x00134DA8 File Offset: 0x00132FA8
			public float Altitude { get; private set; }

			// Token: 0x06004239 RID: 16953 RVA: 0x00134DB1 File Offset: 0x00132FB1
			public SunPosition(float angle, float altitude)
			{
				this = default(DefaultMapWeatherModel.SunPosition);
				this.Angle = angle;
				this.Altitude = altitude;
			}
		}
	}
}
