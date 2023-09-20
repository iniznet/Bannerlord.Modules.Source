using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x02000025 RID: 37
	[DefaultView]
	public class MissionGauntletCameraFadeView : MissionView
	{
		// Token: 0x060001AF RID: 431 RVA: 0x000091BC File Offset: 0x000073BC
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new BindingListFloatItem(0f);
			this._layer = new GauntletLayer(100000, "GauntletLayer", false);
			this._layer.LoadMovie("CameraFade", this._dataSource);
			base.MissionScreen.AddLayer(this._layer);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000921D File Offset: 0x0000741D
		public override void AfterStart()
		{
			base.AfterStart();
			this._controller = base.Mission.GetMissionBehavior<MissionCameraFadeView>();
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00009236 File Offset: 0x00007436
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._dataSource != null && this._controller != null)
			{
				this._dataSource.Item = this._controller.FadeAlpha;
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00009265 File Offset: 0x00007465
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._layer);
			this._controller = null;
			this._dataSource = null;
			this._layer = null;
		}

		// Token: 0x040000C9 RID: 201
		private GauntletLayer _layer;

		// Token: 0x040000CA RID: 202
		private BindingListFloatItem _dataSource;

		// Token: 0x040000CB RID: 203
		private MissionCameraFadeView _controller;
	}
}
