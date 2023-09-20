using System;
using System.Collections.Generic;
using StoryMode.Missions;
using StoryMode.View.Missions;
using StoryMode.ViewModelCollection.Missions;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace StoryMode.GauntletUI.Missions
{
	// Token: 0x02000040 RID: 64
	[OverrideView(typeof(MissionTrainingFieldObjectiveView))]
	public class MissionGauntletTrainingFieldObjectiveView : MissionView
	{
		// Token: 0x060001C1 RID: 449 RVA: 0x00006EA8 File Offset: 0x000050A8
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			TrainingFieldMissionController missionBehavior = base.Mission.GetMissionBehavior<TrainingFieldMissionController>();
			this._dataSource = new TrainingFieldObjectivesVM();
			this._dataSource.UpdateCurrentObjectiveText(missionBehavior.InitialCurrentObjective);
			this._layer = new GauntletLayer(2, "GauntletLayer", false);
			this._layer.LoadMovie("TrainingFieldObjectives", this._dataSource);
			base.MissionScreen.AddLayer(this._layer);
			missionBehavior.TimerTick = new Action<string>(this._dataSource.UpdateTimerText);
			missionBehavior.CurrentObjectiveTick = new Action<TextObject>(this._dataSource.UpdateCurrentObjectiveText);
			missionBehavior.AllObjectivesTick = new Action<List<TrainingFieldMissionController.TutorialObjective>>(this._dataSource.UpdateObjectivesWith);
			missionBehavior.UIStartTimer = new Action(this.BeginTimer);
			missionBehavior.UIEndTimer = new Func<float>(this.EndTimer);
			missionBehavior.CurrentMouseObjectiveTick = new Action<TrainingFieldMissionController.MouseObjectives>(this._dataSource.UpdateCurrentMouseObjective);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00006FA0 File Offset: 0x000051A0
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._isTimerActive)
			{
				this._dataSource.UpdateTimerText((base.Mission.CurrentTime - this._beginningTime).ToString("0.0"));
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00006FE6 File Offset: 0x000051E6
		private void BeginTimer()
		{
			this._isTimerActive = true;
			this._beginningTime = base.Mission.CurrentTime;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00007000 File Offset: 0x00005200
		private float EndTimer()
		{
			this._isTimerActive = false;
			this._dataSource.UpdateTimerText("");
			return base.Mission.CurrentTime - this._beginningTime;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000702B File Offset: 0x0000522B
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._layer);
			this._dataSource = null;
			this._layer = null;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00007052 File Offset: 0x00005252
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._layer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000706F File Offset: 0x0000526F
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._layer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x0400006D RID: 109
		private TrainingFieldObjectivesVM _dataSource;

		// Token: 0x0400006E RID: 110
		private GauntletLayer _layer;

		// Token: 0x0400006F RID: 111
		private float _beginningTime;

		// Token: 0x04000070 RID: 112
		private bool _isTimerActive;
	}
}
