using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	[DefaultView]
	public class MissionGauntletCameraFadeView : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new BindingListFloatItem(0f);
			this._layer = new GauntletLayer(100000, "GauntletLayer", false);
			this._layer.LoadMovie("CameraFade", this._dataSource);
			base.MissionScreen.AddLayer(this._layer);
		}

		public override void AfterStart()
		{
			base.AfterStart();
			this._controller = base.Mission.GetMissionBehavior<MissionCameraFadeView>();
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._dataSource != null && this._controller != null)
			{
				this._dataSource.Item = this._controller.FadeAlpha;
			}
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._layer);
			this._controller = null;
			this._dataSource = null;
			this._layer = null;
		}

		private GauntletLayer _layer;

		private BindingListFloatItem _dataSource;

		private MissionCameraFadeView _controller;
	}
}
