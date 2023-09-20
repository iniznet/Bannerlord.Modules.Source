using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.View.Map
{
	public class MapWeatherVisual
	{
		public Vec2 Position
		{
			get
			{
				return this._weatherNode.Position;
			}
		}

		public Vec2 PrefabSpawnOffset
		{
			get
			{
				Vec2 terrainSize = Campaign.Current.MapSceneWrapper.GetTerrainSize();
				float num = terrainSize.X / (float)Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;
				float num2 = terrainSize.Y / (float)Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;
				return new Vec2(num * 0.5f, num2 * 0.5f);
			}
		}

		public int MaskPixelIndex
		{
			get
			{
				if (this._maskPixelIndex == -1)
				{
					Vec2 terrainSize = Campaign.Current.MapSceneWrapper.GetTerrainSize();
					float num = terrainSize.X / (float)Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;
					float num2 = terrainSize.Y / (float)Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;
					int num3 = (int)(this.Position.x / num);
					int num4 = (int)(this.Position.y / num2);
					this._maskPixelIndex = num4 * Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension + num3;
				}
				return this._maskPixelIndex;
			}
		}

		public override string ToString()
		{
			return this.Position.ToString();
		}

		public MapWeatherVisual(WeatherNode weatherNode)
		{
			this._weatherNode = weatherNode;
			this._previousWeatherEvent = 0;
		}

		public void Tick()
		{
			if (this._weatherNode.IsVisuallyDirty)
			{
				bool flag = this._previousWeatherEvent == 2;
				bool flag2 = this._previousWeatherEvent == 4;
				MapWeatherModel.WeatherEvent weatherEventInPosition = Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(this.Position);
				bool flag3 = weatherEventInPosition == 2;
				bool flag4 = weatherEventInPosition == 1;
				bool flag5 = weatherEventInPosition == 4;
				byte b = (flag4 ? 125 : (flag3 ? 200 : 0));
				byte b2 = (byte)Math.Max((int)b, flag5 ? 200 : 0);
				MapWeatherVisualManager.Current.SetRainData(this.MaskPixelIndex, b);
				MapWeatherVisualManager.Current.SetCloudData(this.MaskPixelIndex, b2);
				if (this.Prefab == null)
				{
					if (flag3)
					{
						this.AttachNewRainPrefabToVisual();
					}
					else if (flag5)
					{
						this.AttachNewBlizzardPrefabToVisual();
					}
					else if (MBRandom.RandomFloat < 0.1f)
					{
						MapWeatherVisualManager.Current.SetCloudData(this.MaskPixelIndex, 200);
					}
				}
				else
				{
					if (flag && !flag3 && flag5)
					{
						MapWeatherVisualManager.Current.ReleaseRainPrefab(this.Prefab);
						this.AttachNewBlizzardPrefabToVisual();
					}
					else if (flag2 && !flag5 && flag3)
					{
						MapWeatherVisualManager.Current.ReleaseBlizzardPrefab(this.Prefab);
						this.AttachNewRainPrefabToVisual();
					}
					if (!flag3 && !flag5)
					{
						if (flag)
						{
							MapWeatherVisualManager.Current.ReleaseRainPrefab(this.Prefab);
						}
						else if (flag2)
						{
							MapWeatherVisualManager.Current.ReleaseBlizzardPrefab(this.Prefab);
						}
						this.Prefab = null;
					}
				}
				this._previousWeatherEvent = weatherEventInPosition;
				this._weatherNode.OnVisualUpdated();
			}
		}

		private void AttachNewRainPrefabToVisual()
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = new Vec3(this.Position + this.PrefabSpawnOffset, 26f, -1f);
			GameEntity rainPrefabFromPool = MapWeatherVisualManager.Current.GetRainPrefabFromPool();
			rainPrefabFromPool.SetVisibilityExcludeParents(true);
			rainPrefabFromPool.SetGlobalFrame(ref identity);
			this.Prefab = rainPrefabFromPool;
		}

		private void AttachNewBlizzardPrefabToVisual()
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = new Vec3(this.Position + this.PrefabSpawnOffset, 26f, -1f);
			GameEntity blizzardPrefabFromPool = MapWeatherVisualManager.Current.GetBlizzardPrefabFromPool();
			blizzardPrefabFromPool.SetVisibilityExcludeParents(true);
			blizzardPrefabFromPool.SetGlobalFrame(ref identity);
			this.Prefab = blizzardPrefabFromPool;
		}

		private readonly WeatherNode _weatherNode;

		public GameEntity Prefab;

		private MapWeatherModel.WeatherEvent _previousWeatherEvent;

		private int _maskPixelIndex = -1;
	}
}
