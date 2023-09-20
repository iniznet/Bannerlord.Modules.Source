using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001FF RID: 511
	public class MissionDeploymentPlan : IMissionDeploymentPlan
	{
		// Token: 0x06001C5B RID: 7259 RVA: 0x00064C4C File Offset: 0x00062E4C
		public MissionDeploymentPlan(Mission mission)
		{
			this._mission = mission;
			for (int i = 0; i < 2; i++)
			{
				BattleSideEnum battleSideEnum = (BattleSideEnum)i;
				this._battleSideDeploymentPlans[i] = new BattleSideDeploymentPlan(mission, battleSideEnum);
				this._playerSpawnFrames[i] = null;
			}
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x00064CB0 File Offset: 0x00062EB0
		public void CreateReinforcementPlans()
		{
			for (int i = 0; i < 2; i++)
			{
				this._battleSideDeploymentPlans[i].CreateReinforcementPlans();
			}
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x00064CD8 File Offset: 0x00062ED8
		public void ClearDeploymentPlanForSide(BattleSideEnum battleSide, DeploymentPlanType planType)
		{
			this._battleSideDeploymentPlans[(int)battleSide].ClearPlans(planType);
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x00064CE8 File Offset: 0x00062EE8
		public bool HasPlayerSpawnFrame(BattleSideEnum battleSide)
		{
			return this._playerSpawnFrames[(int)battleSide] != null;
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x00064CFC File Offset: 0x00062EFC
		public bool GetPlayerSpawnFrame(BattleSideEnum battleSide, out WorldPosition position, out Vec2 direction)
		{
			WorldFrame? worldFrame = this._playerSpawnFrames[(int)battleSide];
			if (worldFrame != null)
			{
				Scene scene = Mission.Current.Scene;
				UIntPtr zero = UIntPtr.Zero;
				WorldFrame worldFrame2 = worldFrame.Value;
				position = new WorldPosition(scene, zero, worldFrame2.Origin.GetGroundVec3(), false);
				worldFrame2 = worldFrame.Value;
				direction = worldFrame2.Rotation.f.AsVec2.Normalized();
				return true;
			}
			position = WorldPosition.Invalid;
			direction = Vec2.Invalid;
			return false;
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x00064D8E File Offset: 0x00062F8E
		public static bool HasSignificantMountedTroops(int footTroopCount, int mountedTroopCount)
		{
			return (float)mountedTroopCount / Math.Max((float)(mountedTroopCount + footTroopCount), 1f) >= 0.1f;
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x00064DAB File Offset: 0x00062FAB
		public void ClearAddedTroopsForBattleSide(BattleSideEnum battleSide, DeploymentPlanType planType)
		{
			this._battleSideDeploymentPlans[(int)battleSide].ClearAddedTroops(planType);
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x00064DBC File Offset: 0x00062FBC
		public void ClearAll()
		{
			for (int i = 0; i < 2; i++)
			{
				this._battleSideDeploymentPlans[i].ClearAddedTroops(DeploymentPlanType.Initial);
				this._battleSideDeploymentPlans[i].ClearPlans(DeploymentPlanType.Initial);
				this._battleSideDeploymentPlans[i].ClearAddedTroops(DeploymentPlanType.Reinforcement);
				this._battleSideDeploymentPlans[i].ClearPlans(DeploymentPlanType.Reinforcement);
			}
		}

		// Token: 0x06001C63 RID: 7267 RVA: 0x00064E0D File Offset: 0x0006300D
		public void AddTroopsForBattleSide(BattleSideEnum battleSide, DeploymentPlanType planType, FormationClass formationClass, int footTroopCount, int mountedTroopCount)
		{
			this._battleSideDeploymentPlans[(int)battleSide].AddTroops(formationClass, footTroopCount, mountedTroopCount, planType);
		}

		// Token: 0x06001C64 RID: 7268 RVA: 0x00064E24 File Offset: 0x00063024
		public void SetSpawnWithHorsesForSide(BattleSideEnum battleSide, bool spawnWithHorses)
		{
			this._battleSideDeploymentPlans[(int)battleSide].SetSpawnWithHorses(spawnWithHorses);
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x00064E44 File Offset: 0x00063044
		public void PlanBattleDeployment(BattleSideEnum battleSide, DeploymentPlanType planType, float spawnPathOffset = 0f)
		{
			if (!battleSide.IsValid())
			{
				Debug.FailedAssert("Cannot make deployment plan. Battle side is not valid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\MissionDeploymentPlan.cs", "PlanBattleDeployment", 125);
				return;
			}
			BattleSideDeploymentPlan battleSideDeploymentPlan = this._battleSideDeploymentPlans[(int)battleSide];
			if (this._battleSideDeploymentPlans[(int)battleSide].IsPlanMade(planType))
			{
				battleSideDeploymentPlan.ClearPlans(planType);
			}
			if (!this._mission.HasSpawnPath && this._formationSceneSpawnEntries == null)
			{
				this.ReadSpawnEntitiesFromScene(this._mission.IsFieldBattle);
			}
			battleSideDeploymentPlan.PlanBattleDeployment(this._formationSceneSpawnEntries, planType, spawnPathOffset);
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x00064EC8 File Offset: 0x000630C8
		public bool IsPositionInsideDeploymentBoundaries(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType)
		{
			BattleSideDeploymentPlan battleSideDeploymentPlan = this._battleSideDeploymentPlans[(int)battleSide];
			if (battleSideDeploymentPlan.HasDeploymentBoundaries(planType))
			{
				return battleSideDeploymentPlan.IsPositionInsideInitialDeploymentBoundaries(position);
			}
			Debug.FailedAssert("Cannot check if position is within deployment boundaries as requested battle side does not have deployment boundaries.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\MissionDeploymentPlan.cs", "IsPositionInsideDeploymentBoundaries", 154);
			return false;
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x00064F0C File Offset: 0x0006310C
		public Vec2 GetClosestDeploymentBoundaryPosition(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType)
		{
			BattleSideDeploymentPlan battleSideDeploymentPlan = this._battleSideDeploymentPlans[(int)battleSide];
			if (battleSideDeploymentPlan.HasDeploymentBoundaries(planType))
			{
				return battleSideDeploymentPlan.GetClosestInitialDeploymentBoundaryPosition(position);
			}
			Debug.FailedAssert("Cannot retrieve closest deployment boundary position as requested battle side does not have deployment boundaries.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\MissionDeploymentPlan.cs", "GetClosestDeploymentBoundaryPosition", 166);
			return position;
		}

		// Token: 0x06001C68 RID: 7272 RVA: 0x00064F52 File Offset: 0x00063152
		public int GetTroopCountForSide(BattleSideEnum side, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].GetTroopCount(planType);
		}

		// Token: 0x06001C69 RID: 7273 RVA: 0x00064F62 File Offset: 0x00063162
		public float GetSpawnPathOffsetForSide(BattleSideEnum side, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].GetSpawnPathOffset(planType);
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x00064F72 File Offset: 0x00063172
		public IFormationDeploymentPlan GetFormationPlan(BattleSideEnum side, FormationClass fClass, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].GetFormationPlan(fClass, planType);
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x00064F83 File Offset: 0x00063183
		public bool IsPlanMadeForBattleSide(BattleSideEnum side, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].IsPlanMade(planType);
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x00064F93 File Offset: 0x00063193
		public bool IsPlanMadeForBattleSide(BattleSideEnum side, out bool isFirstPlan, DeploymentPlanType planType)
		{
			isFirstPlan = false;
			if (this._battleSideDeploymentPlans[(int)side].IsPlanMade(planType))
			{
				isFirstPlan = this._battleSideDeploymentPlans[(int)side].IsFirstPlan(planType);
				return true;
			}
			return false;
		}

		// Token: 0x06001C6D RID: 7277 RVA: 0x00064FBB File Offset: 0x000631BB
		public bool IsInitialPlanSuitableForFormations(BattleSideEnum side, ValueTuple<int, int>[] troopDataPerFormationClass)
		{
			return this._battleSideDeploymentPlans[(int)side].IsInitialPlanSuitableForFormations(troopDataPerFormationClass);
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x00064FCB File Offset: 0x000631CB
		public bool HasDeploymentBoundaries(BattleSideEnum side, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].HasDeploymentBoundaries(planType);
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x00064FDB File Offset: 0x000631DB
		public MatrixFrame GetBattleSideDeploymentFrame(BattleSideEnum side, DeploymentPlanType planType)
		{
			if (this.IsPlanMadeForBattleSide(side, planType))
			{
				return this._battleSideDeploymentPlans[(int)side].GetDeploymentFrame(planType);
			}
			Debug.FailedAssert("Cannot retrieve formation deployment frame as deployment plan is not made for this battle side.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\MissionDeploymentPlan.cs", "GetBattleSideDeploymentFrame", 220);
			return MatrixFrame.Identity;
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x00065014 File Offset: 0x00063214
		public MBReadOnlyDictionary<string, List<Vec2>> GetDeploymentBoundaries(BattleSideEnum side, DeploymentPlanType planType)
		{
			if (this.HasDeploymentBoundaries(side, planType))
			{
				return this._battleSideDeploymentPlans[(int)side].GetDeploymentBoundaries(planType);
			}
			Debug.FailedAssert("Cannot retrieve battle side deployment boundaries as they are not available for this battle side.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\MissionDeploymentPlan.cs", "GetDeploymentBoundaries", 233);
			return null;
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x00065049 File Offset: 0x00063249
		public void UpdateReinforcementPlan(BattleSideEnum side)
		{
			this._battleSideDeploymentPlans[(int)side].UpdateReinforcementPlans();
		}

		// Token: 0x06001C72 RID: 7282 RVA: 0x00065058 File Offset: 0x00063258
		private void ReadSpawnEntitiesFromScene(bool isFieldBattle)
		{
			for (int i = 0; i < 2; i++)
			{
				this._playerSpawnFrames[i] = null;
			}
			this._formationSceneSpawnEntries = new FormationSceneSpawnEntry[2, 11];
			Scene scene = this._mission.Scene;
			if (isFieldBattle)
			{
				for (int j = 0; j < 2; j++)
				{
					string text = ((j == 1) ? "attacker_" : "defender_");
					for (int k = 0; k < 11; k++)
					{
						FormationClass formationClass = (FormationClass)k;
						GameEntity gameEntity = scene.FindEntityWithTag(text + formationClass.GetName().ToLower());
						if (gameEntity == null)
						{
							FormationClass formationClass2 = formationClass.FallbackClass();
							int num = (int)formationClass2;
							GameEntity spawnEntity = this._formationSceneSpawnEntries[j, num].SpawnEntity;
							gameEntity = ((spawnEntity != null) ? spawnEntity : scene.FindEntityWithTag(text + formationClass2.GetName().ToLower()));
							formationClass = ((gameEntity != null) ? formationClass2 : FormationClass.NumberOfAllFormations);
						}
						this._formationSceneSpawnEntries[j, k] = new FormationSceneSpawnEntry(formationClass, gameEntity, gameEntity);
					}
				}
				return;
			}
			GameEntity gameEntity2 = null;
			if (this._mission.IsSallyOutBattle)
			{
				gameEntity2 = scene.FindEntityWithTag("sally_out_ambush_battle_set");
			}
			if (gameEntity2 != null)
			{
				this.ReadSallyOutEntitiesFromScene(scene, gameEntity2);
			}
			else
			{
				this.ReadSiegeBattleEntitiesFromScene(scene, BattleSideEnum.Defender);
			}
			this.ReadSiegeBattleEntitiesFromScene(scene, BattleSideEnum.Attacker);
		}

		// Token: 0x06001C73 RID: 7283 RVA: 0x000651C0 File Offset: 0x000633C0
		private void ReadSallyOutEntitiesFromScene(Scene missionScene, GameEntity sallyOutSetEntity)
		{
			int num = 0;
			MatrixFrame globalFrame = sallyOutSetEntity.GetFirstChildEntityWithTag("sally_out_ambush_player").GetGlobalFrame();
			WorldPosition worldPosition = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalFrame.origin, false);
			this._playerSpawnFrames[num] = new WorldFrame?(new WorldFrame(globalFrame.rotation, worldPosition));
			GameEntity firstChildEntityWithTag = sallyOutSetEntity.GetFirstChildEntityWithTag("sally_out_ambush_infantry");
			GameEntity firstChildEntityWithTag2 = sallyOutSetEntity.GetFirstChildEntityWithTag("sally_out_ambush_archer");
			GameEntity firstChildEntityWithTag3 = sallyOutSetEntity.GetFirstChildEntityWithTag("sally_out_ambush_cavalry");
			for (int i = 0; i < 11; i++)
			{
				FormationClass formationClass = (FormationClass)i;
				FormationClass formationClass2 = formationClass.FallbackClass();
				GameEntity gameEntity = null;
				switch (formationClass2)
				{
				case FormationClass.Infantry:
					gameEntity = firstChildEntityWithTag;
					break;
				case FormationClass.Ranged:
					gameEntity = firstChildEntityWithTag2;
					break;
				case FormationClass.Cavalry:
				case FormationClass.HorseArcher:
					gameEntity = firstChildEntityWithTag3;
					break;
				}
				this._formationSceneSpawnEntries[num, i] = new FormationSceneSpawnEntry(formationClass, gameEntity, gameEntity);
			}
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x000652A4 File Offset: 0x000634A4
		private void ReadSiegeBattleEntitiesFromScene(Scene missionScene, BattleSideEnum battleSide)
		{
			int num = (int)battleSide;
			string text = battleSide.ToString().ToLower() + "_";
			for (int i = 0; i < 11; i++)
			{
				FormationClass formationClass = (FormationClass)i;
				string text2 = text + formationClass.GetName().ToLower();
				string text3 = text2 + "_reinforcement";
				GameEntity gameEntity = missionScene.FindEntityWithTag(text2);
				GameEntity gameEntity2;
				if (gameEntity == null)
				{
					FormationClass formationClass2 = formationClass.FallbackClass();
					int num2 = (int)formationClass2;
					FormationSceneSpawnEntry formationSceneSpawnEntry = this._formationSceneSpawnEntries[num, num2];
					if (formationSceneSpawnEntry.SpawnEntity != null)
					{
						gameEntity = formationSceneSpawnEntry.SpawnEntity;
						gameEntity2 = formationSceneSpawnEntry.ReinforcementSpawnEntity;
					}
					else
					{
						text2 = text + formationClass2.GetName().ToLower();
						text3 = text2 + "_reinforcement";
						gameEntity = missionScene.FindEntityWithTag(text2);
						gameEntity2 = missionScene.FindEntityWithTag(text3);
					}
					formationClass = ((gameEntity != null) ? formationClass2 : FormationClass.NumberOfAllFormations);
				}
				else
				{
					gameEntity2 = missionScene.FindEntityWithTag(text3);
				}
				if (gameEntity2 == null)
				{
					gameEntity2 = gameEntity;
				}
				this._formationSceneSpawnEntries[num, i] = new FormationSceneSpawnEntry(formationClass, gameEntity, gameEntity2);
			}
		}

		// Token: 0x06001C75 RID: 7285 RVA: 0x000653D7 File Offset: 0x000635D7
		bool IMissionDeploymentPlan.IsPositionInsideDeploymentBoundaries(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType)
		{
			return this.IsPositionInsideDeploymentBoundaries(battleSide, position, planType);
		}

		// Token: 0x06001C76 RID: 7286 RVA: 0x000653E2 File Offset: 0x000635E2
		Vec2 IMissionDeploymentPlan.GetClosestDeploymentBoundaryPosition(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType)
		{
			return this.GetClosestDeploymentBoundaryPosition(battleSide, position, planType);
		}

		// Token: 0x0400093A RID: 2362
		public const int NumFormationsWithUnset = 11;

		// Token: 0x0400093B RID: 2363
		private readonly Mission _mission;

		// Token: 0x0400093C RID: 2364
		private readonly BattleSideDeploymentPlan[] _battleSideDeploymentPlans = new BattleSideDeploymentPlan[2];

		// Token: 0x0400093D RID: 2365
		private readonly WorldFrame?[] _playerSpawnFrames = new WorldFrame?[2];

		// Token: 0x0400093E RID: 2366
		private FormationSceneSpawnEntry[,] _formationSceneSpawnEntries;
	}
}
