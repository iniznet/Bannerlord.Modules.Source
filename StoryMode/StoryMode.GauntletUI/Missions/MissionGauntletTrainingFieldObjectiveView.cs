using System;
using System.Collections.Generic;
using StoryMode.Missions;
using StoryMode.View.Missions;
using StoryMode.ViewModelCollection.Missions;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace StoryMode.GauntletUI.Missions
{
	[OverrideView(typeof(MissionTrainingFieldObjectiveView))]
	public class MissionGauntletTrainingFieldObjectiveView : MissionView
	{
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
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._isTimerActive)
			{
				this._dataSource.UpdateTimerText((base.Mission.CurrentTime - this._beginningTime).ToString("0.0"));
			}
		}

		private void BeginTimer()
		{
			this._isTimerActive = true;
			this._beginningTime = base.Mission.CurrentTime;
		}

		private float EndTimer()
		{
			this._isTimerActive = false;
			this._dataSource.UpdateTimerText("");
			return base.Mission.CurrentTime - this._beginningTime;
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._layer);
			this._dataSource = null;
			this._layer = null;
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._layer._gauntletUIContext.ContextAlpha = 0f;
		}

		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._layer._gauntletUIContext.ContextAlpha = 1f;
		}

		private void OnGamepadActiveStateChanged()
		{
			TrainingFieldObjectivesVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.UpdateIsGamepadActive();
		}

		private TrainingFieldObjectivesVM _dataSource;

		private GauntletLayer _layer;

		private float _beginningTime;

		private bool _isTimerActive;
	}
}
