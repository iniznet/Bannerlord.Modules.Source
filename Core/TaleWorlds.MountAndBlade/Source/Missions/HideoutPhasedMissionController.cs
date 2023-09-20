using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	// Token: 0x020003F4 RID: 1012
	public class HideoutPhasedMissionController : MissionLogic
	{
		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x060034CE RID: 13518 RVA: 0x000DBD13 File Offset: 0x000D9F13
		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return MissionBehaviorType.Logic;
			}
		}

		// Token: 0x060034CF RID: 13519 RVA: 0x000DBD18 File Offset: 0x000D9F18
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

		// Token: 0x060034D0 RID: 13520 RVA: 0x000DBDE4 File Offset: 0x000D9FE4
		protected override void OnEndMission()
		{
			base.Mission.AreOrderGesturesEnabled_AdditionalCondition -= this.AreOrderGesturesEnabled_AdditionalCondition;
		}

		// Token: 0x060034D1 RID: 13521 RVA: 0x000DBDFD File Offset: 0x000D9FFD
		public override void OnBehaviorInitialize()
		{
			this.ReadySpawnPointLogic();
			base.Mission.AreOrderGesturesEnabled_AdditionalCondition += this.AreOrderGesturesEnabled_AdditionalCondition;
		}

		// Token: 0x060034D2 RID: 13522 RVA: 0x000DBE1C File Offset: 0x000DA01C
		public override void AfterStart()
		{
			base.AfterStart();
			MissionAgentSpawnLogic missionBehavior = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			if (missionBehavior != null && this.IsPhasingInitialized)
			{
				missionBehavior.AddPhaseChangeAction(BattleSideEnum.Defender, new MissionAgentSpawnLogic.OnPhaseChangedDelegate(this.OnPhaseChanged));
			}
		}

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x060034D3 RID: 13523 RVA: 0x000DBE59 File Offset: 0x000DA059
		private bool IsPhasingInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060034D4 RID: 13524 RVA: 0x000DBE5C File Offset: 0x000DA05C
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

		// Token: 0x060034D5 RID: 13525 RVA: 0x000DBF80 File Offset: 0x000DA180
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

		// Token: 0x060034D6 RID: 13526 RVA: 0x000DC014 File Offset: 0x000DA214
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

		// Token: 0x060034D7 RID: 13527 RVA: 0x000DC087 File Offset: 0x000DA287
		private bool AreOrderGesturesEnabled_AdditionalCondition()
		{
			return false;
		}

		// Token: 0x040016A2 RID: 5794
		public const int PhaseCount = 4;

		// Token: 0x040016A3 RID: 5795
		private GameEntity[] _spawnPoints;

		// Token: 0x040016A4 RID: 5796
		private Stack<MatrixFrame[]> _spawnPointFrames;

		// Token: 0x040016A5 RID: 5797
		private bool _isNewlyPopulatedFormationGivenOrder = true;
	}
}
