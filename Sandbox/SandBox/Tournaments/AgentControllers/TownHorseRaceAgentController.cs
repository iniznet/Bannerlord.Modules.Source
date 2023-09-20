using System;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox.Tournaments.AgentControllers
{
	// Token: 0x02000021 RID: 33
	public class TownHorseRaceAgentController : AgentController
	{
		// Token: 0x0600018D RID: 397 RVA: 0x0000B881 File Offset: 0x00009A81
		public override void OnInitialize()
		{
			this._controller = base.Mission.GetMissionBehavior<TownHorseRaceMissionController>();
			this._checkPointIndex = 0;
			this._tourCount = 0;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000B8A4 File Offset: 0x00009AA4
		public void DisableMovement()
		{
			if (base.Owner.IsAIControlled)
			{
				WorldPosition worldPosition = base.Owner.GetWorldPosition();
				base.Owner.SetScriptedPositionAndDirection(ref worldPosition, base.Owner.Frame.rotation.f.AsVec2.RotationInRadians, false, 0);
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000B900 File Offset: 0x00009B00
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

		// Token: 0x06000190 RID: 400 RVA: 0x0000B984 File Offset: 0x00009B84
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

		// Token: 0x040000A6 RID: 166
		private TownHorseRaceMissionController _controller;

		// Token: 0x040000A7 RID: 167
		private int _checkPointIndex;

		// Token: 0x040000A8 RID: 168
		private int _tourCount;
	}
}
