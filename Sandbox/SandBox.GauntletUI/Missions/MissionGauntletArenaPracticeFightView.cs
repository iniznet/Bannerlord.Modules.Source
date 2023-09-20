using System;
using SandBox.Missions.MissionLogics.Arena;
using SandBox.View.Missions;
using SandBox.ViewModelCollection.Missions;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.GauntletUI.Missions
{
	[OverrideView(typeof(MissionArenaPracticeFightView))]
	public class MissionGauntletArenaPracticeFightView : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			ArenaPracticeFightMissionController missionBehavior = base.Mission.GetMissionBehavior<ArenaPracticeFightMissionController>();
			this._dataSource = new MissionArenaPracticeFightVM(missionBehavior);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._movie = this._gauntletLayer.LoadMovie("ArenaPracticeFight", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.Tick();
		}

		public override void OnMissionScreenFinalize()
		{
			this._dataSource.OnFinalize();
			this._gauntletLayer.ReleaseMovie(this._movie);
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			base.OnMissionScreenFinalize();
		}

		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		private MissionArenaPracticeFightVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _movie;
	}
}
