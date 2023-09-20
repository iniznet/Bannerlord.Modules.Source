using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.FlagMarker;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI.Mission
{
	[OverrideView(typeof(MissionMultiplayerMarkerUIHandler))]
	public class MissionGauntletMultiplayerMarkerUIHandler : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MultiplayerMissionMarkerVM(base.MissionScreen.CombatCamera);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MPMissionMarkers", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.Input.IsGameKeyDown(5))
			{
				this._dataSource.IsEnabled = true;
			}
			else
			{
				this._dataSource.IsEnabled = false;
			}
			this._dataSource.Tick(dt);
		}

		private GauntletLayer _gauntletLayer;

		private MultiplayerMissionMarkerVM _dataSource;
	}
}
