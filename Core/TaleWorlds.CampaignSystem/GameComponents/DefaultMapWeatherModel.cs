using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultMapWeatherModel : MapWeatherModel
	{
		public override CampaignTime WeatherUpdatePeriod
		{
			get
			{
				return CampaignTime.Hours(4f);
			}
		}

		public override CampaignTime WeatherUpdateFrequency
		{
			get
			{
				return new CampaignTime(this.WeatherUpdatePeriod.NumTicks / (long)(this.DefaultWeatherNodeDimension * this.DefaultWeatherNodeDimension));
			}
		}

		public override int DefaultWeatherNodeDimension
		{
			get
			{
				return 32;
			}
		}

		private CampaignTime PreviousRainDataCheckForWetness
		{
			get
			{
				return CampaignTime.Hours(24f);
			}
		}

		private uint GetSeed(CampaignTime campaignTime, Vec2 position)
		{
			campaignTime += new CampaignTime((long)Campaign.Current.UniqueGameId.GetHashCode());
			int num;
			int num2;
			this.GetNodePositionForWeather(position, out num, out num2);
			uint num3 = (uint)(campaignTime.ToHours / this.WeatherUpdatePeriod.ToHours);
			if (campaignTime.ToSeconds % this.WeatherUpdatePeriod.ToSeconds < this.WeatherUpdateFrequency.ToSeconds * (double)(num * this.DefaultWeatherNodeDimension + num2))
			{
				num3 -= 1U;
			}
			return num3;
		}

		public override AtmosphereState GetInterpolatedAtmosphereState(CampaignTime timeOfYear, Vec3 pos)
		{
			if (this._atmosphereGrid == null)
			{
				this._atmosphereGrid = new AtmosphereGrid();
				this._atmosphereGrid.Initialize();
			}
			return this._atmosphereGrid.GetInterpolatedStateInfo(pos);
		}

		private Vec2 GetNodePositionForWeather(Vec2 pos, out int xIndex, out int yIndex)
		{
			if (Campaign.Current.MapSceneWrapper != null)
			{
				Vec2 terrainSize = Campaign.Current.MapSceneWrapper.GetTerrainSize();
				float num = terrainSize.X / (float)this.DefaultWeatherNodeDimension;
				float num2 = terrainSize.Y / (float)this.DefaultWeatherNodeDimension;
				xIndex = (int)(pos.x / num);
				yIndex = (int)(pos.y / num2);
				float num3 = (float)xIndex * num;
				float num4 = (float)yIndex * num2;
				return new Vec2(num3, num4);
			}
			xIndex = 0;
			yIndex = 0;
			return Vec2.Zero;
		}

		public override AtmosphereInfo GetAtmosphereModel(Vec3 position)
		{
			float hourOfDayNormalized = this.GetHourOfDayNormalized();
			float num;
			float num2;
			this.GetSeasonTimeFactorOfCampaignTime(CampaignTime.Now, out num, out num2, true);
			DefaultMapWeatherModel.SunPosition sunPosition = this.GetSunPosition(hourOfDayNormalized, num);
			float environmentMultiplier = this.GetEnvironmentMultiplier(sunPosition);
			float num3 = this.GetModifiedEnvironmentMultiplier(environmentMultiplier);
			num3 = MathF.Max(MathF.Pow(num3, 1.5f), 0.001f);
			Vec3 sunColor = this.GetSunColor(environmentMultiplier);
			AtmosphereState interpolatedAtmosphereState = this.GetInterpolatedAtmosphereState(CampaignTime.Now, position);
			float temperature = this.GetTemperature(ref interpolatedAtmosphereState, num);
			float humidity = this.GetHumidity(ref interpolatedAtmosphereState, num);
			ValueTuple<CampaignTime.Seasons, bool, float, float> seasonRainAndSnowDataForOpeningMission = this.GetSeasonRainAndSnowDataForOpeningMission(position.AsVec2);
			CampaignTime.Seasons item = seasonRainAndSnowDataForOpeningMission.Item1;
			bool item2 = seasonRainAndSnowDataForOpeningMission.Item2;
			float item3 = seasonRainAndSnowDataForOpeningMission.Item3;
			float item4 = seasonRainAndSnowDataForOpeningMission.Item4;
			string selectedAtmosphereId = this.GetSelectedAtmosphereId(item, item2, item4, item3);
			AtmosphereInfo atmosphereInfo = default(AtmosphereInfo);
			atmosphereInfo.SunInfo.Altitude = sunPosition.Altitude;
			atmosphereInfo.SunInfo.Angle = sunPosition.Angle;
			atmosphereInfo.SunInfo.Color = sunColor;
			atmosphereInfo.SunInfo.Brightness = this.GetSunBrightness(environmentMultiplier, false);
			atmosphereInfo.SunInfo.Size = this.GetSunSize(environmentMultiplier);
			atmosphereInfo.SunInfo.RayStrength = this.GetSunRayStrength(environmentMultiplier);
			atmosphereInfo.SunInfo.MaxBrightness = this.GetSunBrightness(1f, true);
			atmosphereInfo.RainInfo.Density = item3;
			atmosphereInfo.SnowInfo.Density = item4;
			atmosphereInfo.AmbientInfo.EnvironmentMultiplier = MathF.Max(num3 * 0.5f, 0.001f);
			atmosphereInfo.AmbientInfo.AmbientColor = this.GetAmbientFogColor(num3);
			atmosphereInfo.AmbientInfo.MieScatterStrength = this.GetMieScatterStrength(environmentMultiplier);
			atmosphereInfo.AmbientInfo.RayleighConstant = this.GetRayleighConstant(environmentMultiplier);
			atmosphereInfo.SkyInfo.Brightness = this.GetSkyBrightness(hourOfDayNormalized, environmentMultiplier);
			atmosphereInfo.FogInfo.Density = this.GetFogDensity(environmentMultiplier, position);
			atmosphereInfo.FogInfo.Color = this.GetFogColor(num3);
			atmosphereInfo.FogInfo.Falloff = 1.48f;
			atmosphereInfo.TimeInfo.TimeOfDay = this.GetHourOfDay();
			atmosphereInfo.TimeInfo.WinterTimeFactor = this.GetWinterTimeFactor(CampaignTime.Now);
			atmosphereInfo.TimeInfo.DrynessFactor = this.GetDrynessFactor(CampaignTime.Now);
			atmosphereInfo.TimeInfo.NightTimeFactor = this.GetNightTimeFactor();
			atmosphereInfo.TimeInfo.Season = (int)item;
			atmosphereInfo.AreaInfo.Temperature = temperature;
			atmosphereInfo.AreaInfo.Humidity = humidity;
			atmosphereInfo.PostProInfo.MinExposure = MBMath.Lerp(-3f, -2f, this.GetExposureCoefficientBetweenDayNight(), 1E-05f);
			atmosphereInfo.PostProInfo.MaxExposure = MBMath.Lerp(2f, 0f, num3, 1E-05f);
			atmosphereInfo.PostProInfo.BrightpassThreshold = MBMath.Lerp(0.7f, 0.9f, num3, 1E-05f);
			atmosphereInfo.PostProInfo.MiddleGray = 0.1f;
			atmosphereInfo.InterpolatedAtmosphereName = selectedAtmosphereId;
			return atmosphereInfo;
		}

		public void InitializeSnowAndRainAmountData(byte[] snowAndRainAmountData)
		{
			this._snowAndRainAmountData = snowAndRainAmountData;
		}

		public override MapWeatherModel.WeatherEvent UpdateWeatherForPosition(Vec2 position, CampaignTime ct)
		{
			ValueTuple<float, float> snowAndRainDataFromTexture = this.GetSnowAndRainDataFromTexture(position, ct);
			float item = snowAndRainDataFromTexture.Item1;
			float item2 = snowAndRainDataFromTexture.Item2;
			if (item > 0.55f)
			{
				return this.SetIsBlizzardOrSnowFromFunction(item, ct, position);
			}
			return this.SetIsRainingOrWetFromFunction(item2, ct, position);
		}

		private MapWeatherModel.WeatherEvent SetIsBlizzardOrSnowFromFunction(float snowValue, CampaignTime campaignTime, in Vec2 position)
		{
			int num;
			int num2;
			Vec2 nodePositionForWeather = this.GetNodePositionForWeather(position, out num, out num2);
			if (snowValue >= 0.65000004f)
			{
				float num3 = (snowValue - 0.55f) / 0.45f;
				uint seed = this.GetSeed(campaignTime, position);
				bool currentWeatherInAdjustedPosition = this.GetCurrentWeatherInAdjustedPosition(seed, num3, 0.1f, nodePositionForWeather);
				this._weatherDataCache[num2 * 32 + num] = (currentWeatherInAdjustedPosition ? MapWeatherModel.WeatherEvent.Blizzard : MapWeatherModel.WeatherEvent.Snowy);
			}
			else
			{
				this._weatherDataCache[num2 * 32 + num] = ((snowValue > 0.55f) ? MapWeatherModel.WeatherEvent.Snowy : MapWeatherModel.WeatherEvent.Clear);
			}
			return this._weatherDataCache[num2 * 32 + num];
		}

		private MapWeatherModel.WeatherEvent SetIsRainingOrWetFromFunction(float rainValue, CampaignTime campaignTime, in Vec2 position)
		{
			int num;
			int num2;
			Vec2 nodePositionForWeather = this.GetNodePositionForWeather(position, out num, out num2);
			if (rainValue >= 0.6f)
			{
				float num3 = (rainValue - 0.6f) / 0.39999998f;
				uint seed = this.GetSeed(campaignTime, position);
				this._weatherDataCache[num2 * 32 + num] = MapWeatherModel.WeatherEvent.Clear;
				if (this.GetCurrentWeatherInAdjustedPosition(seed, num3, 0.45f, nodePositionForWeather))
				{
					this._weatherDataCache[num2 * 32 + num] = MapWeatherModel.WeatherEvent.HeavyRain;
				}
				else
				{
					CampaignTime campaignTime2 = new CampaignTime(campaignTime.NumTicks - this.WeatherUpdatePeriod.NumTicks);
					uint num4 = this.GetSeed(campaignTime2, position);
					float num5 = (this.GetSnowAndRainDataFromTexture(position, campaignTime2).Item2 - 0.6f) / 0.39999998f;
					while (campaignTime.NumTicks - campaignTime2.NumTicks < this.PreviousRainDataCheckForWetness.NumTicks)
					{
						if (this.GetCurrentWeatherInAdjustedPosition(num4, num5, 0.45f, nodePositionForWeather))
						{
							this._weatherDataCache[num2 * 32 + num] = MapWeatherModel.WeatherEvent.LightRain;
							break;
						}
						campaignTime2 = new CampaignTime(campaignTime2.NumTicks - this.WeatherUpdatePeriod.NumTicks);
						num4 = this.GetSeed(campaignTime2, position);
						num5 = (this.GetSnowAndRainDataFromTexture(position, campaignTime2).Item2 - 0.6f) / 0.39999998f;
					}
				}
			}
			else
			{
				this._weatherDataCache[num2 * 32 + num] = MapWeatherModel.WeatherEvent.Clear;
			}
			return this._weatherDataCache[num2 * 32 + num];
		}

		private bool GetCurrentWeatherInAdjustedPosition(uint seed, float frequency, float chanceModifier, in Vec2 adjustedPosition)
		{
			float num = frequency * chanceModifier;
			float mapDiagonal = Campaign.MapDiagonal;
			Vec2 vec = adjustedPosition;
			float num2 = mapDiagonal * vec.X;
			vec = adjustedPosition;
			return num > MBRandom.RandomFloatWithSeed(seed, (uint)(num2 + vec.Y));
		}

		private string GetSelectedAtmosphereId(CampaignTime.Seasons selectedSeason, bool isRaining, float snowValue, float rainValue)
		{
			string text = "semicloudy_field_battle";
			if (Settlement.CurrentSettlement != null && (Settlement.CurrentSettlement.IsFortification || Settlement.CurrentSettlement.IsVillage))
			{
				text = "semicloudy_" + Settlement.CurrentSettlement.Culture.StringId;
			}
			if (selectedSeason == CampaignTime.Seasons.Winter)
			{
				if (snowValue >= 0.85f)
				{
					text = "dense_snowy";
				}
				else
				{
					text = "semi_snowy";
				}
			}
			else
			{
				if (rainValue > 0.6f)
				{
					text = "wet";
				}
				if (isRaining)
				{
					if (rainValue >= 0.85f)
					{
						text = "dense_rainy";
					}
					else
					{
						text = "semi_rainy";
					}
				}
			}
			return text;
		}

		private ValueTuple<CampaignTime.Seasons, bool, float, float> GetSeasonRainAndSnowDataForOpeningMission(Vec2 position)
		{
			CampaignTime.Seasons seasons = CampaignTime.Now.GetSeasonOfYear;
			MapWeatherModel.WeatherEvent weatherEventInPosition = this.GetWeatherEventInPosition(position);
			float num = 0f;
			float num2 = 0.85f;
			bool flag = false;
			switch (weatherEventInPosition)
			{
			case MapWeatherModel.WeatherEvent.Clear:
				if (seasons == CampaignTime.Seasons.Winter)
				{
					seasons = ((CampaignTime.Now.GetDayOfSeason > 10) ? CampaignTime.Seasons.Spring : CampaignTime.Seasons.Autumn);
				}
				break;
			case MapWeatherModel.WeatherEvent.LightRain:
				if (seasons == CampaignTime.Seasons.Winter)
				{
					seasons = ((CampaignTime.Now.GetDayOfSeason > 10) ? CampaignTime.Seasons.Spring : CampaignTime.Seasons.Autumn);
				}
				num = 0.7f;
				break;
			case MapWeatherModel.WeatherEvent.HeavyRain:
				if (seasons == CampaignTime.Seasons.Winter)
				{
					seasons = ((CampaignTime.Now.GetDayOfSeason > 10) ? CampaignTime.Seasons.Spring : CampaignTime.Seasons.Autumn);
				}
				flag = true;
				num = 0.85f + MBRandom.RandomFloatRanged(0f, 0.14999998f);
				break;
			case MapWeatherModel.WeatherEvent.Snowy:
				seasons = CampaignTime.Seasons.Winter;
				num = 0.55f;
				num2 = 0.55f + MBRandom.RandomFloatRanged(0f, 0.3f);
				break;
			case MapWeatherModel.WeatherEvent.Blizzard:
				seasons = CampaignTime.Seasons.Winter;
				num = 0.85f;
				num2 = 0.85f;
				break;
			}
			return new ValueTuple<CampaignTime.Seasons, bool, float, float>(seasons, flag, num, num2);
		}

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

		private float GetSunSize(float envMultiplier)
		{
			return 0.1f + (1f - envMultiplier) / 8f;
		}

		private float GetSunRayStrength(float envMultiplier)
		{
			return MathF.Min(MathF.Max(MathF.Sin(MathF.Pow((envMultiplier - 0.001f) / 0.999f, 0.4f) * 3.1415927f / 2f) - 0.15f, 0.01f), 0.5f);
		}

		private float GetEnvironmentMultiplier(DefaultMapWeatherModel.SunPosition sunPos)
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

		private float GetFogDensity(float environmentMultiplier, Vec3 pos)
		{
			float num = (this._sunIsMoon ? 0.5f : 0.4f);
			float num2 = 1f - environmentMultiplier;
			float num3 = 1f - MBMath.ClampFloat((pos.z - 30f) / 200f, 0f, 0.9f);
			return MathF.Min((0f + num * num2) * num3, 10f);
		}

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

		private Vec3 GetAmbientFogColor(float moddedEnvMul)
		{
			return Vec3.Vec3Min(new Vec3(0.15f, 0.3f, 0.5f, -1f) + new Vec3(moddedEnvMul / 3f, moddedEnvMul / 2f, moddedEnvMul / 1.5f, -1f), new Vec3(1f, 1f, 1f, -1f));
		}

		private float GetMieScatterStrength(float envMultiplier)
		{
			return (1f + (1f - envMultiplier)) * 10f;
		}

		private float GetRayleighConstant(float envMultiplier)
		{
			float num = (envMultiplier - 0.001f) / 0.999f;
			return MathF.Min(MathF.Max(1f - MathF.Sin(MathF.Pow(num, 0.45f) * 3.1415927f / 2f) + (0.14f + num * 2f), 0.65f), 0.99f);
		}

		private float GetHourOfDay()
		{
			return (float)(CampaignTime.Now.ToHours % 24.0);
		}

		private float GetHourOfDayNormalized()
		{
			return this.GetHourOfDay() / 24f;
		}

		private float GetNightTimeFactor()
		{
			float num = this.GetHourOfDay() - 2f;
			for (num %= 24f; num < 0f; num += 24f)
			{
			}
			num = MathF.Max(num - 20f, 0f);
			return MathF.Min(num / 0.1f, 1f);
		}

		private float GetExposureCoefficientBetweenDayNight()
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

		private int GetTextureDataIndexForPosition(Vec2 position)
		{
			Vec2 terrainSize = Campaign.Current.MapSceneWrapper.GetTerrainSize();
			int num = MathF.Floor(position.x / terrainSize.X * 1024f);
			int num2 = MathF.Floor(position.y / terrainSize.Y * 1024f);
			num = MBMath.ClampIndex(num, 0, 1024);
			return MBMath.ClampIndex(num2, 0, 1024) * 1024 + num;
		}

		public ValueTuple<float, float> GetSnowAndRainDataFromTexture(Vec2 position, CampaignTime ct)
		{
			int num;
			int num2;
			Vec2 nodePositionForWeather = this.GetNodePositionForWeather(position, out num, out num2);
			int textureDataIndexForPosition = this.GetTextureDataIndexForPosition(position);
			int textureDataIndexForPosition2 = this.GetTextureDataIndexForPosition(nodePositionForWeather);
			byte b = this._snowAndRainAmountData[textureDataIndexForPosition * 2];
			float num3 = (float)this._snowAndRainAmountData[textureDataIndexForPosition2 * 2 + 1];
			float num4 = (float)b / 255f;
			float num5 = num3 / 255f;
			float num6;
			float num7;
			Campaign.Current.Models.MapWeatherModel.GetSeasonTimeFactorOfCampaignTime(ct, out num6, out num7, true);
			float num8 = MBMath.Lerp(0.55f, -0.1f, num6, 1E-05f);
			float num9 = MBMath.Lerp(0.7f, 0.3f, num7, 1E-05f);
			float num10 = MBMath.SmoothStep(num8 - 0.65f, num8 + 0.65f, num4);
			float num11 = MBMath.SmoothStep(num9 - 0.45f, num9 + 0.45f, num5);
			return new ValueTuple<float, float>(MBMath.Lerp(0f, num10, num10, 1E-05f), num11);
		}

		public override MapWeatherModel.WeatherEvent GetWeatherEventInPosition(Vec2 pos)
		{
			int num;
			int num2;
			this.GetNodePositionForWeather(pos, out num, out num2);
			return this._weatherDataCache[num2 * 32 + num];
		}

		public override MapWeatherModel.WeatherEventEffectOnTerrain GetWeatherEffectOnTerrainForPosition(Vec2 pos)
		{
			switch (this.GetWeatherEventInPosition(pos))
			{
			case MapWeatherModel.WeatherEvent.Clear:
				return MapWeatherModel.WeatherEventEffectOnTerrain.Default;
			case MapWeatherModel.WeatherEvent.LightRain:
				return MapWeatherModel.WeatherEventEffectOnTerrain.Wet;
			case MapWeatherModel.WeatherEvent.HeavyRain:
				return MapWeatherModel.WeatherEventEffectOnTerrain.Wet;
			case MapWeatherModel.WeatherEvent.Snowy:
				return MapWeatherModel.WeatherEventEffectOnTerrain.Wet;
			case MapWeatherModel.WeatherEvent.Blizzard:
				return MapWeatherModel.WeatherEventEffectOnTerrain.Wet;
			default:
				return MapWeatherModel.WeatherEventEffectOnTerrain.Default;
			}
		}

		private float GetWinterTimeFactor(CampaignTime timeOfYear)
		{
			float num = 0f;
			if (timeOfYear.GetSeasonOfYear == CampaignTime.Seasons.Winter)
			{
				float num2 = MathF.Abs((float)Math.IEEERemainder(CampaignTime.Now.ToSeasons, 1.0));
				num = MBMath.SplitLerp(0f, 0.75f, 0f, 0.5f, num2, 1E-05f);
			}
			return num;
		}

		private float GetDrynessFactor(CampaignTime timeOfYear)
		{
			float num = 0f;
			float num2 = MathF.Abs((float)Math.IEEERemainder(CampaignTime.Now.ToSeasons, 1.0));
			switch (timeOfYear.GetSeasonOfYear)
			{
			case CampaignTime.Seasons.Summer:
			{
				float num3 = MBMath.ClampFloat(num2 * 2f, 0f, 1f);
				num = MBMath.Lerp(0f, 1f, num3, 1E-05f);
				break;
			}
			case CampaignTime.Seasons.Autumn:
				num = 1f;
				break;
			case CampaignTime.Seasons.Winter:
				num = MBMath.Lerp(1f, 0f, num2, 1E-05f);
				break;
			}
			return num;
		}

		public override void GetSeasonTimeFactorOfCampaignTime(CampaignTime ct, out float timeFactorForSnow, out float timeFactorForRain, bool snapCampaignTimeToWeatherPeriod = true)
		{
			if (snapCampaignTimeToWeatherPeriod)
			{
				ct = CampaignTime.Hours((float)((int)(ct.ToHours / this.WeatherUpdatePeriod.ToHours / 2.0) * (int)this.WeatherUpdatePeriod.ToHours * 2));
			}
			float num = (float)ct.ToSeasons % 4f;
			timeFactorForSnow = this.CalculateTimeFactorForSnow(num);
			timeFactorForRain = this.CalculateTimeFactorForRain(num);
		}

		private float CalculateTimeFactorForSnow(float yearProgress)
		{
			float num = 0f;
			if (yearProgress > 1.5f && (double)yearProgress <= 3.5)
			{
				num = MBMath.Map(yearProgress, 1.5f, 3.5f, 0f, 1f);
			}
			else if (yearProgress <= 1.5f)
			{
				num = MBMath.Map(yearProgress, 0f, 1.5f, 0.75f, 0f);
			}
			else if (yearProgress > 3.5f)
			{
				num = MBMath.Map(yearProgress, 3.5f, 4f, 1f, 0.75f);
			}
			return num;
		}

		private float CalculateTimeFactorForRain(float yearProgress)
		{
			float num = 0f;
			if (yearProgress > 1f && (double)yearProgress <= 2.5)
			{
				num = MBMath.Map(yearProgress, 1f, 2.5f, 0f, 1f);
			}
			else if (yearProgress <= 1f)
			{
				num = MBMath.Map(yearProgress, 0f, 1f, 1f, 0f);
			}
			else if (yearProgress > 2.5f)
			{
				num = 1f;
			}
			return num;
		}

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

		private const float SunRiseNorm = 0.083333336f;

		private const float SunSetNorm = 0.9166667f;

		private const float DayTime = 20f;

		private const float MinSunAngle = 0f;

		private const float MaxSunAngle = 50f;

		private const float MinEnvironmentMultiplier = 0.001f;

		private const float DayEnvironmentMultiplier = 1f;

		private const float NightEnvironmentMultiplier = 0.001f;

		private const float SnowStartThreshold = 0.55f;

		private const float DenseSnowStartThreshold = 0.85f;

		private const float NoSnowDelta = 0.1f;

		private const float WetThreshold = 0.6f;

		private const float WetThresholdForTexture = 0.3f;

		private const float LightRainStartThreshold = 0.7f;

		private const float DenseRainStartThreshold = 0.85f;

		private const float SnowFrequencyModifier = 0.1f;

		private const float RainFrequencyModifier = 0.45f;

		private const float MaxSnowCoverage = 0.75f;

		private const int SnowAndRainDataTextureDimension = 1024;

		private const int WeatherNodeDimension = 32;

		private MapWeatherModel.WeatherEvent[] _weatherDataCache = new MapWeatherModel.WeatherEvent[1024];

		private AtmosphereGrid _atmosphereGrid;

		private byte[] _snowAndRainAmountData = new byte[2097152];

		private bool _sunIsMoon;

		private struct SunPosition
		{
			public float Angle { get; private set; }

			public float Altitude { get; private set; }

			public SunPosition(float angle, float altitude)
			{
				this = default(DefaultMapWeatherModel.SunPosition);
				this.Angle = angle;
				this.Altitude = altitude;
			}
		}
	}
}
