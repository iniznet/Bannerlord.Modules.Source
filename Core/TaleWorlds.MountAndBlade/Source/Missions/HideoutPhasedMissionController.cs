using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	public class HideoutPhasedMissionController : MissionLogic
	{
		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return MissionBehaviorType.Logic;
			}
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (!this._isNewlyPopulatedFormationGivenOrder)
			{
				foreach (Team team in base.Mission.Teams)
				{
					if (team.Side == BattleSideEnum.Defender)
					{
						foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
						{
							if (formation.CountOfUnits > 0)
							{
								formation.SetMovementOrder(MovementOrder.MovementOrderMove(formation.QuerySystem.MedianPosition));
								this._isNewlyPopulatedFormationGivenOrder = true;
							}
						}
					}
				}
			}
		}

		protected override void OnEndMission()
		{
			base.Mission.AreOrderGesturesEnabled_AdditionalCondition -= this.AreOrderGesturesEnabled_AdditionalCondition;
		}

		public override void OnBehaviorInitialize()
		{
			this.ReadySpawnPointLogic();
			base.Mission.AreOrderGesturesEnabled_AdditionalCondition += this.AreOrderGesturesEnabled_AdditionalCondition;
		}

		public override void AfterStart()
		{
			base.AfterStart();
			MissionAgentSpawnLogic missionBehavior = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			if (missionBehavior != null && this.IsPhasingInitialized)
			{
				missionBehavior.AddPhaseChangeAction(BattleSideEnum.Defender, new MissionAgentSpawnLogic.OnPhaseChangedDelegate(this.OnPhaseChanged));
			}
		}

		private bool IsPhasingInitialized
		{
			get
			{
				return true;
			}
		}

		private void ReadySpawnPointLogic()
		{
			List<GameEntity> list = Mission.Current.GetActiveEntitiesWithScriptComponentOfType<HideoutSpawnPointGroup>().ToList<GameEntity>();
			if (list.Count == 0)
			{
				return;
			}
			HideoutSpawnPointGroup[] array = new HideoutSpawnPointGroup[list.Count];
			foreach (GameEntity gameEntity in list)
			{
				HideoutSpawnPointGroup firstScriptOfType = gameEntity.GetFirstScriptOfType<HideoutSpawnPointGroup>();
				array[firstScriptOfType.PhaseNumber - 1] = firstScriptOfType;
			}
			List<HideoutSpawnPointGroup> list2 = array.ToList<HideoutSpawnPointGroup>();
			list2.RemoveAt(0);
			for (int i = 0; i < 3; i++)
			{
				list2.RemoveAt(MBRandom.RandomInt(list2.Count));
			}
			this._spawnPointFrames = new Stack<MatrixFrame[]>();
			for (int j = 0; j < array.Length; j++)
			{
				if (!list2.Contains(array[j]))
				{
					this._spawnPointFrames.Push(array[j].GetSpawnPointFrames());
					Debug.Print("Spawn " + array[j].PhaseNumber + " is active.", 0, Debug.DebugColor.Green, 64UL);
				}
				array[j].RemoveWithAllChildren();
			}
			this.CreateSpawnPoints();
		}

		private void CreateSpawnPoints()
		{
			MatrixFrame[] array = this._spawnPointFrames.Pop();
			this._spawnPoints = new GameEntity[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].IsIdentity)
				{
					this._spawnPoints[i] = GameEntity.CreateEmpty(base.Mission.Scene, true);
					this._spawnPoints[i].SetGlobalFrame(array[i]);
					this._spawnPoints[i].AddTag("defender_" + ((FormationClass)i).GetName().ToLower());
				}
			}
		}

		private void OnPhaseChanged()
		{
			if (this._spawnPointFrames.Count == 0)
			{
				Debug.FailedAssert("No position left.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\HideoutPhasedMissionController.cs", "OnPhaseChanged", 142);
				return;
			}
			for (int i = 0; i < this._spawnPoints.Length; i++)
			{
				if (!(this._spawnPoints[i] == null))
				{
					this._spawnPoints[i].Remove(78);
				}
			}
			this.CreateSpawnPoints();
			this._isNewlyPopulatedFormationGivenOrder = false;
		}

		private bool AreOrderGesturesEnabled_AdditionalCondition()
		{
			return false;
		}

		public const int PhaseCount = 4;

		private GameEntity[] _spawnPoints;

		private Stack<MatrixFrame[]> _spawnPointFrames;

		private bool _isNewlyPopulatedFormationGivenOrder = true;
	}
}
