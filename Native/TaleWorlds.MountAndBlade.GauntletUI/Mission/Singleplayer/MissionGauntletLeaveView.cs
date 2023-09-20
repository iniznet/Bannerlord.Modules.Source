using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	// Token: 0x02000031 RID: 49
	[OverrideView(typeof(MissionLeaveView))]
	public class MissionGauntletLeaveView : MissionView
	{
		// Token: 0x06000251 RID: 593 RVA: 0x0000CD48 File Offset: 0x0000AF48
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MissionLeaveVM(new Func<float>(base.Mission.GetMissionEndTimerValue), new Func<float>(base.Mission.GetMissionEndTimeInSeconds));
			this._gauntletLayer = new GauntletLayer(47, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("LeaveUI", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000CDC3 File Offset: 0x0000AFC3
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000CDE4 File Offset: 0x0000AFE4
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this._dataSource.Tick(dt);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000CDF9 File Offset: 0x0000AFF9
		private void OnEscapeMenuToggled(bool isOpened)
		{
			ScreenManager.SetSuspendLayer(this._gauntletLayer, !isOpened);
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000CE0A File Offset: 0x0000B00A
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000CE27 File Offset: 0x0000B027
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x04000126 RID: 294
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000127 RID: 295
		private MissionLeaveVM _dataSource;
	}
}
