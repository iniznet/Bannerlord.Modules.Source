using System;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox.Tournaments.AgentControllers
{
	public class TownHorseRaceAgentController : AgentController
	{
		public override void OnInitialize()
		{
			this._controller = base.Mission.GetMissionBehavior<TownHorseRaceMissionController>();
			this._checkPointIndex = 0;
			this._tourCount = 0;
		}

		public void DisableMovement()
		{
			if (base.Owner.IsAIControlled)
			{
				WorldPosition worldPosition = base.Owner.GetWorldPosition();
				base.Owner.SetScriptedPositionAndDirection(ref worldPosition, base.Owner.Frame.rotation.f.AsVec2.RotationInRadians, false, 0);
			}
		}

		public void Start()
		{
			if (this._checkPointIndex < this._controller.CheckPoints.Count)
			{
				TownHorseRaceMissionController.CheckPoint checkPoint = this._controller.CheckPoints[this._checkPointIndex];
				checkPoint.AddToCheckList(base.Owner);
				if (base.Owner.IsAIControlled)
				{
					WorldPosition worldPosition;
					worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, checkPoint.GetBestTargetPosition(), false);
					base.Owner.SetScriptedPosition(ref worldPosition, false, 8);
				}
			}
		}

		public void OnEnterCheckPoint(VolumeBox checkPoint)
		{
			this._controller.CheckPoints[this._checkPointIndex].RemoveFromCheckList(base.Owner);
			this._checkPointIndex++;
			if (this._checkPointIndex < this._controller.CheckPoints.Count)
			{
				if (base.Owner.IsAIControlled)
				{
					WorldPosition worldPosition;
					worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, this._controller.CheckPoints[this._checkPointIndex].GetBestTargetPosition(), false);
					base.Owner.SetScriptedPosition(ref worldPosition, false, 8);
				}
				this._controller.CheckPoints[this._checkPointIndex].AddToCheckList(base.Owner);
				return;
			}
			this._tourCount++;
			if (this._tourCount < 2)
			{
				this._checkPointIndex = 0;
				if (base.Owner.IsAIControlled)
				{
					WorldPosition worldPosition2;
					worldPosition2..ctor(Mission.Current.Scene, UIntPtr.Zero, this._controller.CheckPoints[this._checkPointIndex].GetBestTargetPosition(), false);
					base.Owner.SetScriptedPosition(ref worldPosition2, false, 8);
				}
				this._controller.CheckPoints[this._checkPointIndex].AddToCheckList(base.Owner);
			}
		}

		private TownHorseRaceMissionController _controller;

		private int _checkPointIndex;

		private int _tourCount;
	}
}
