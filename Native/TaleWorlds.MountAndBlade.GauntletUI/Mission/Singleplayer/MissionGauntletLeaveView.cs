using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	[OverrideView(typeof(MissionLeaveView))]
	public class MissionGauntletLeaveView : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MissionLeaveVM(new Func<float>(base.Mission.GetMissionEndTimerValue), new Func<float>(base.Mission.GetMissionEndTimeInSeconds));
			this._gauntletLayer = new GauntletLayer(47, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("LeaveUI", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this._dataSource.Tick(dt);
		}

		private void OnEscapeMenuToggled(bool isOpened)
		{
			ScreenManager.SetSuspendLayer(this._gauntletLayer, !isOpened);
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

		private GauntletLayer _gauntletLayer;

		private MissionLeaveVM _dataSource;
	}
}
