using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class WhileEnteringSettlementBattleMissionController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		public WhileEnteringSettlementBattleMissionController(IMissionTroopSupplier[] suppliers, int numberOfMaxTroopForPlayer, int numberOfMaxTroopForEnemy)
		{
			this._troopSuppliers = suppliers;
			this._numberOfMaxTroopForPlayer = numberOfMaxTroopForPlayer;
			this._numberOfMaxTroopForEnemy = numberOfMaxTroopForEnemy;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._battleAgentLogic = Mission.Current.GetMissionBehavior<BattleAgentLogic>();
		}

		public override void OnMissionTick(float dt)
		{
			if (!this._isMissionInitialized)
			{
				this.SpawnAgents();
				this._isMissionInitialized = true;
				return;
			}
			if (!this._troopsInitialized)
			{
				this._troopsInitialized = true;
				foreach (Agent agent in base.Mission.Agents)
				{
					this._battleAgentLogic.OnAgentBuild(agent, null);
				}
			}
		}

		private void SpawnAgents()
		{
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_outside_near_town_main_gate");
			IMissionTroopSupplier[] troopSuppliers = this._troopSuppliers;
			for (int i = 0; i < troopSuppliers.Length; i++)
			{
				foreach (IAgentOriginBase agentOriginBase in troopSuppliers[i].SupplyTroops(this._numberOfMaxTroopForPlayer + this._numberOfMaxTroopForEnemy).ToList<IAgentOriginBase>())
				{
					bool flag = agentOriginBase.IsUnderPlayersCommand || agentOriginBase.Troop.IsPlayerCharacter;
					if ((!flag || this._numberOfMaxTroopForPlayer >= this._playerSideSpawnedTroopCount) && (flag || this._numberOfMaxTroopForEnemy >= this._otherSideSpawnedTroopCount))
					{
						WorldFrame worldFrame;
						worldFrame..ctor(gameEntity.GetGlobalFrame().rotation, new WorldPosition(base.Mission.Scene, gameEntity.GetGlobalFrame().origin));
						if (!flag)
						{
							worldFrame.Origin.SetVec2(worldFrame.Origin.AsVec2 + worldFrame.Rotation.f.AsVec2 * 20f);
							worldFrame.Rotation.f = (gameEntity.GetGlobalFrame().origin.AsVec2 - worldFrame.Origin.AsVec2).ToVec3(0f);
							worldFrame.Origin.SetVec2(base.Mission.GetRandomPositionAroundPoint(worldFrame.Origin.GetNavMeshVec3(), 0f, 2.5f, false).AsVec2);
						}
						worldFrame.Rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
						base.Mission.SpawnTroop(agentOriginBase, flag, false, false, false, 0, 0, true, false, false, new Vec3?(worldFrame.Origin.GetGroundVec3()), new Vec2?(worldFrame.Rotation.f.AsVec2), null, null, 10, false).Defensiveness = 1f;
						if (flag)
						{
							this._playerSideSpawnedTroopCount++;
						}
						else
						{
							this._otherSideSpawnedTroopCount++;
						}
					}
				}
			}
		}

		public void StartSpawner(BattleSideEnum side)
		{
		}

		public void StopSpawner(BattleSideEnum side)
		{
		}

		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return false;
		}

		public float GetReinforcementInterval()
		{
			return 0f;
		}

		public bool IsSideDepleted(BattleSideEnum side)
		{
			if (side == base.Mission.PlayerTeam.Side)
			{
				return this._troopSuppliers[side].NumRemovedTroops == this._playerSideSpawnedTroopCount;
			}
			return this._troopSuppliers[side].NumRemovedTroops == this._otherSideSpawnedTroopCount;
		}

		private const int GuardSpawnPointAndPlayerSpawnPointPositionDelta = 20;

		private BattleAgentLogic _battleAgentLogic;

		private bool _isMissionInitialized;

		private bool _troopsInitialized;

		private int _numberOfMaxTroopForPlayer;

		private int _numberOfMaxTroopForEnemy;

		private int _playerSideSpawnedTroopCount;

		private int _otherSideSpawnedTroopCount;

		private readonly IMissionTroopSupplier[] _troopSuppliers;
	}
}
