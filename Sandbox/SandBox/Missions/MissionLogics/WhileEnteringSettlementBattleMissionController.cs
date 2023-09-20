using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000059 RID: 89
	public class WhileEnteringSettlementBattleMissionController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		// Token: 0x060003D9 RID: 985 RVA: 0x0001BD42 File Offset: 0x00019F42
		public WhileEnteringSettlementBattleMissionController(IMissionTroopSupplier[] suppliers, int numberOfMaxTroopForPlayer, int numberOfMaxTroopForEnemy)
		{
			this._troopSuppliers = suppliers;
			this._numberOfMaxTroopForPlayer = numberOfMaxTroopForPlayer;
			this._numberOfMaxTroopForEnemy = numberOfMaxTroopForEnemy;
		}

		// Token: 0x060003DA RID: 986 RVA: 0x0001BD5F File Offset: 0x00019F5F
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._battleAgentLogic = Mission.Current.GetMissionBehavior<BattleAgentLogic>();
		}

		// Token: 0x060003DB RID: 987 RVA: 0x0001BD78 File Offset: 0x00019F78
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

		// Token: 0x060003DC RID: 988 RVA: 0x0001BDFC File Offset: 0x00019FFC
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

		// Token: 0x060003DD RID: 989 RVA: 0x0001C044 File Offset: 0x0001A244
		public void StartSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x060003DE RID: 990 RVA: 0x0001C046 File Offset: 0x0001A246
		public void StopSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x060003DF RID: 991 RVA: 0x0001C048 File Offset: 0x0001A248
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return false;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x0001C04B File Offset: 0x0001A24B
		public float GetReinforcementInterval()
		{
			return 0f;
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0001C052 File Offset: 0x0001A252
		public bool IsSideDepleted(BattleSideEnum side)
		{
			if (side == base.Mission.PlayerTeam.Side)
			{
				return this._troopSuppliers[side].NumRemovedTroops == this._playerSideSpawnedTroopCount;
			}
			return this._troopSuppliers[side].NumRemovedTroops == this._otherSideSpawnedTroopCount;
		}

		// Token: 0x040001CB RID: 459
		private const int GuardSpawnPointAndPlayerSpawnPointPositionDelta = 20;

		// Token: 0x040001CC RID: 460
		private BattleAgentLogic _battleAgentLogic;

		// Token: 0x040001CD RID: 461
		private bool _isMissionInitialized;

		// Token: 0x040001CE RID: 462
		private bool _troopsInitialized;

		// Token: 0x040001CF RID: 463
		private int _numberOfMaxTroopForPlayer;

		// Token: 0x040001D0 RID: 464
		private int _numberOfMaxTroopForEnemy;

		// Token: 0x040001D1 RID: 465
		private int _playerSideSpawnedTroopCount;

		// Token: 0x040001D2 RID: 466
		private int _otherSideSpawnedTroopCount;

		// Token: 0x040001D3 RID: 467
		private readonly IMissionTroopSupplier[] _troopSuppliers;
	}
}
