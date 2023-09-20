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
	// Token: 0x02000012 RID: 18
	[OverrideView(typeof(MissionArenaPracticeFightView))]
	public class MissionGauntletArenaPracticeFightView : MissionView
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x00006F4C File Offset: 0x0000514C
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			ArenaPracticeFightMissionController missionBehavior = base.Mission.GetMissionBehavior<ArenaPracticeFightMissionController>();
			this._dataSource = new MissionArenaPracticeFightVM(missionBehavior);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._movie = this._gauntletLayer.LoadMovie("ArenaPracticeFight", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00006FBB File Offset: 0x000051BB
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.Tick();
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00006FCF File Offset: 0x000051CF
		public override void OnMissionScreenFinalize()
		{
			this._dataSource.OnFinalize();
			this._gauntletLayer.ReleaseMovie(this._movie);
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			base.OnMissionScreenFinalize();
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00007004 File Offset: 0x00005204
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00007021 File Offset: 0x00005221
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x04000058 RID: 88
		private MissionArenaPracticeFightVM _dataSource;

		// Token: 0x04000059 RID: 89
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400005A RID: 90
		private IGauntletMovie _movie;
	}
}
