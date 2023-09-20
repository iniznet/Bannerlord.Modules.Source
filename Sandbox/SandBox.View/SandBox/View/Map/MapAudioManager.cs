using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;

namespace SandBox.View.Map
{
	internal class MapAudioManager : CampaignEntityVisualComponent
	{
		public MapAudioManager()
		{
			this._mapScene = Campaign.Current.MapSceneWrapper as MapScene;
		}

		public override void OnVisualTick(MapScreen screen, float realDt, float dt)
		{
			if (CampaignTime.Now.GetSeasonOfYear != this._lastCachedSeason)
			{
				SoundManager.SetGlobalParameter("Season", CampaignTime.Now.GetSeasonOfYear);
				this._lastCachedSeason = CampaignTime.Now.GetSeasonOfYear;
			}
			if (Math.Abs(this._lastCameraZ - this._mapScene.Scene.LastFinalRenderCameraPosition.Z) > 0.1f)
			{
				SoundManager.SetGlobalParameter("CampaignCameraHeight", this._mapScene.Scene.LastFinalRenderCameraPosition.Z);
				this._lastCameraZ = this._mapScene.Scene.LastFinalRenderCameraPosition.Z;
			}
			if ((int)CampaignTime.Now.CurrentHourInDay == this._lastHourUpdate)
			{
				SoundManager.SetGlobalParameter("Daytime", CampaignTime.Now.CurrentHourInDay);
				this._lastHourUpdate = (int)CampaignTime.Now.CurrentHourInDay;
			}
		}

		private const string SeasonParameterId = "Season";

		private const string CameraHeightParameterId = "CampaignCameraHeight";

		private const string TimeOfDayParameterId = "Daytime";

		private const string WeatherEventIntensityParameterId = "Rainfall";

		private CampaignTime.Seasons _lastCachedSeason;

		private float _lastCameraZ;

		private int _lastHourUpdate;

		private MapScene _mapScene;
	}
}
