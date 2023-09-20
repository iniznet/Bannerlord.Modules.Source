using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.View.Map
{
	public class MapWeatherVisualManager : CampaignEntityVisualComponent
	{
		public static MapWeatherVisualManager Current
		{
			get
			{
				return Campaign.Current.GetEntityComponent<MapWeatherVisualManager>();
			}
		}

		private int DimensionSquared
		{
			get
			{
				return Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;
			}
		}

		public MapWeatherVisualManager()
		{
			this._unusedRainPrefabEntityPool = new List<GameEntity>();
			this._unusedBlizzardPrefabEntityPool = new List<GameEntity>();
			for (int i = 0; i < this.DimensionSquared * 2; i++)
			{
				this._rainData[i] = 0;
				this._rainDataTemporal[i] = 0;
			}
			this._allWeatherNodeVisuals = new MapWeatherVisual[this.DimensionSquared];
			this._mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
			WeatherNode[] allWeatherNodes = Campaign.Current.GetCampaignBehavior<MapWeatherCampaignBehavior>().AllWeatherNodes;
			for (int j = 0; j < allWeatherNodes.Length; j++)
			{
				this._allWeatherNodeVisuals[j] = new MapWeatherVisual(allWeatherNodes[j]);
			}
		}

		public override void OnVisualTick(MapScreen screen, float realDt, float dt)
		{
			for (int i = 0; i < this._allWeatherNodeVisuals.Length; i++)
			{
				this._allWeatherNodeVisuals[i].Tick();
			}
			TWParallel.For(0, this.DimensionSquared, delegate(int startInclusive, int endExclusive)
			{
				for (int j = startInclusive; j < endExclusive; j++)
				{
					int num = j * 2;
					this._rainDataTemporal[num] = (byte)MBMath.Lerp((float)this._rainDataTemporal[num], (float)this._rainData[num], 1f - (float)Math.Exp((double)(-1.8f * (realDt + dt))), 1E-05f);
					this._rainDataTemporal[num + 1] = (byte)MBMath.Lerp((float)this._rainDataTemporal[num + 1], (float)this._rainData[num + 1], 1f - (float)Math.Exp((double)(-1.8f * (realDt + dt))), 1E-05f);
				}
			}, 16);
			this._mapScene.SetLandscapeRainMaskData(this._rainDataTemporal);
			this.WeatherAudioTick();
		}

		public void SetRainData(int dataIndex, byte value)
		{
			this._rainData[dataIndex * 2] = value;
		}

		public void SetCloudData(int dataIndex, byte value)
		{
			this._rainData[dataIndex * 2 + 1] = value;
		}

		private void WeatherAudioTick()
		{
			SoundManager.SetGlobalParameter("Rainfall", 0.5f);
			float num = 1f;
			float num2 = 0f;
			IMapScene mapSceneWrapper = Campaign.Current.MapSceneWrapper;
			MatrixFrame matrixFrame = this._mapScene.LastFinalRenderCameraFrame;
			mapSceneWrapper.GetHeightAtPoint(matrixFrame.origin.AsVec2, ref num2);
			matrixFrame = this._mapScene.LastFinalRenderCameraFrame;
			float num3 = matrixFrame.origin.Z - num2;
			if (26f > num3)
			{
				num = num3 / 26f;
			}
			matrixFrame = this._mapScene.LastFinalRenderCameraFrame;
			Vec3 origin = matrixFrame.Elevate(-25f * num).origin;
			MapWeatherModel.WeatherEvent weatherEventInPosition = Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(origin.AsVec2);
			if (weatherEventInPosition != null)
			{
				if (weatherEventInPosition == 2)
				{
					if (this._mapScene.LastFinalRenderCameraPosition.z < 65f)
					{
						this._cameraRainEffect.SetVisibilityExcludeParents(true);
						matrixFrame = this._mapScene.LastFinalRenderCameraFrame;
						MatrixFrame matrixFrame2 = matrixFrame.Elevate(-5f);
						this._cameraRainEffect.SetFrame(ref matrixFrame2);
					}
					else
					{
						this._cameraRainEffect.SetVisibilityExcludeParents(false);
					}
					this.DestroyBlizzardSound();
					this.StartRainSoundIfNeeded();
					MBMapScene.ApplyRainColorGrade = true;
					return;
				}
				if (weatherEventInPosition == 4)
				{
					this.DestroyRainSound();
					this.StartBlizzardSoundIfNeeded();
					this._cameraRainEffect.SetVisibilityExcludeParents(false);
					MBMapScene.ApplyRainColorGrade = false;
					return;
				}
			}
			else
			{
				this.DestroyBlizzardSound();
				this.DestroyRainSound();
				this._cameraRainEffect.SetVisibilityExcludeParents(false);
				MBMapScene.ApplyRainColorGrade = false;
			}
		}

		private void DestroyRainSound()
		{
			if (this._currentRainSound != null)
			{
				this._currentRainSound.Stop();
				this._currentRainSound = null;
			}
		}

		private void DestroyBlizzardSound()
		{
			if (this._currentBlizzardSound != null)
			{
				this._currentBlizzardSound.Stop();
				this._currentBlizzardSound = null;
			}
		}

		private void StartRainSoundIfNeeded()
		{
			if (this._currentRainSound == null)
			{
				this._currentRainSound = SoundManager.CreateEvent("event:/map/ambient/bed/rain", this._mapScene);
				this._currentRainSound.Play();
			}
		}

		private void StartBlizzardSoundIfNeeded()
		{
			if (this._currentBlizzardSound == null)
			{
				this._currentBlizzardSound = SoundManager.CreateEvent("event:/map/ambient/bed/snow", this._mapScene);
				this._currentBlizzardSound.Play();
			}
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this.InitializeObjectPoolWithDefaultCount();
			this._cameraRainEffect = GameEntity.Instantiate(this._mapScene, "map_camera_rain_prefab", MatrixFrame.Identity);
		}

		public GameEntity GetRainPrefabFromPool()
		{
			if (Extensions.IsEmpty<GameEntity>(this._unusedRainPrefabEntityPool))
			{
				this._unusedRainPrefabEntityPool.AddRange(this.CreateNewWeatherPrefabPoolElements("campaign_rain_prefab", 5));
			}
			GameEntity gameEntity = this._unusedRainPrefabEntityPool[0];
			this._unusedRainPrefabEntityPool.Remove(gameEntity);
			return gameEntity;
		}

		public GameEntity GetBlizzardPrefabFromPool()
		{
			if (Extensions.IsEmpty<GameEntity>(this._unusedBlizzardPrefabEntityPool))
			{
				this._unusedBlizzardPrefabEntityPool.AddRange(this.CreateNewWeatherPrefabPoolElements("campaign_snow_prefab", 5));
			}
			GameEntity gameEntity = this._unusedBlizzardPrefabEntityPool[0];
			this._unusedBlizzardPrefabEntityPool.Remove(gameEntity);
			return gameEntity;
		}

		public void ReleaseRainPrefab(GameEntity prefab)
		{
			this._unusedRainPrefabEntityPool.Add(prefab);
			prefab.SetVisibilityExcludeParents(false);
		}

		public void ReleaseBlizzardPrefab(GameEntity prefab)
		{
			this._unusedBlizzardPrefabEntityPool.Add(prefab);
			prefab.SetVisibilityExcludeParents(false);
		}

		private void InitializeObjectPoolWithDefaultCount()
		{
			this._unusedRainPrefabEntityPool.AddRange(this.CreateNewWeatherPrefabPoolElements("campaign_rain_prefab", 5));
			this._unusedBlizzardPrefabEntityPool.AddRange(this.CreateNewWeatherPrefabPoolElements("campaign_snow_prefab", 5));
		}

		private List<GameEntity> CreateNewWeatherPrefabPoolElements(string prefabName, int delta)
		{
			List<GameEntity> list = new List<GameEntity>();
			for (int i = 0; i < delta; i++)
			{
				GameEntity gameEntity = GameEntity.Instantiate(this._mapScene, prefabName, MatrixFrame.Identity);
				gameEntity.SetVisibilityExcludeParents(false);
				list.Add(gameEntity);
			}
			return list;
		}

		public const int DefaultCloudHeight = 26;

		private MapWeatherVisual[] _allWeatherNodeVisuals;

		private const string RainPrefabName = "campaign_rain_prefab";

		private const string BlizzardPrefabName = "campaign_snow_prefab";

		private const string RainSoundPath = "event:/map/ambient/bed/rain";

		private const string SnowSoundPath = "event:/map/ambient/bed/snow";

		private const string WeatherEventParameterName = "Rainfall";

		private const string CameraRainPrefabName = "map_camera_rain_prefab";

		private const int DefaultRainObjectPoolCount = 5;

		private const int DefaultBlizzardObjectPoolCount = 5;

		private const int WeatherCheckOriginZDelta = 25;

		private readonly List<GameEntity> _unusedRainPrefabEntityPool;

		private readonly List<GameEntity> _unusedBlizzardPrefabEntityPool;

		private readonly Scene _mapScene;

		private readonly byte[] _rainData = new byte[Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * 2];

		private readonly byte[] _rainDataTemporal = new byte[Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * 2];

		private SoundEvent _currentRainSound;

		private SoundEvent _currentBlizzardSound;

		private GameEntity _cameraRainEffect;
	}
}
