using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MissionDeploymentPlan : IMissionDeploymentPlan
	{
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

		public void CreateReinforcementPlans()
		{
			for (int i = 0; i < 2; i++)
			{
				this._battleSideDeploymentPlans[i].CreateReinforcementPlans();
			}
		}

		public void ClearDeploymentPlanForSide(BattleSideEnum battleSide, DeploymentPlanType planType)
		{
			this._battleSideDeploymentPlans[(int)battleSide].ClearPlans(planType);
		}

		public bool HasPlayerSpawnFrame(BattleSideEnum battleSide)
		{
			return this._playerSpawnFrames[(int)battleSide] != null;
		}

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

		public static bool HasSignificantMountedTroops(int footTroopCount, int mountedTroopCount)
		{
			return (float)mountedTroopCount / Math.Max((float)(mountedTroopCount + footTroopCount), 1f) >= 0.1f;
		}

		public void ClearAddedTroopsForBattleSide(BattleSideEnum battleSide, DeploymentPlanType planType)
		{
			this._battleSideDeploymentPlans[(int)battleSide].ClearAddedTroops(planType);
		}

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

		public void AddTroopsForBattleSide(BattleSideEnum battleSide, DeploymentPlanType planType, FormationClass formationClass, int footTroopCount, int mountedTroopCount)
		{
			this._battleSideDeploymentPlans[(int)battleSide].AddTroops(formationClass, footTroopCount, mountedTroopCount, planType);
		}

		public void SetSpawnWithHorsesForSide(BattleSideEnum battleSide, bool spawnWithHorses)
		{
			this._battleSideDeploymentPlans[(int)battleSide].SetSpawnWithHorses(spawnWithHorses);
		}

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

		public int GetTroopCountForSide(BattleSideEnum side, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].GetTroopCount(planType);
		}

		public float GetSpawnPathOffsetForSide(BattleSideEnum side, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].GetSpawnPathOffset(planType);
		}

		public IFormationDeploymentPlan GetFormationPlan(BattleSideEnum side, FormationClass fClass, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].GetFormationPlan(fClass, planType);
		}

		public bool IsPlanMadeForBattleSide(BattleSideEnum side, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].IsPlanMade(planType);
		}

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

		public bool IsInitialPlanSuitableForFormations(BattleSideEnum side, ValueTuple<int, int>[] troopDataPerFormationClass)
		{
			return this._battleSideDeploymentPlans[(int)side].IsInitialPlanSuitableForFormations(troopDataPerFormationClass);
		}

		public bool HasDeploymentBoundaries(BattleSideEnum side, DeploymentPlanType planType)
		{
			return this._battleSideDeploymentPlans[(int)side].HasDeploymentBoundaries(planType);
		}

		public MatrixFrame GetBattleSideDeploymentFrame(BattleSideEnum side, DeploymentPlanType planType)
		{
			if (this.IsPlanMadeForBattleSide(side, planType))
			{
				return this._battleSideDeploymentPlans[(int)side].GetDeploymentFrame(planType);
			}
			Debug.FailedAssert("Cannot retrieve formation deployment frame as deployment plan is not made for this battle side.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\MissionDeploymentPlan.cs", "GetBattleSideDeploymentFrame", 220);
			return MatrixFrame.Identity;
		}

		public MBReadOnlyDictionary<string, List<Vec2>> GetDeploymentBoundaries(BattleSideEnum side, DeploymentPlanType planType)
		{
			if (this.HasDeploymentBoundaries(side, planType))
			{
				return this._battleSideDeploymentPlans[(int)side].GetDeploymentBoundaries(planType);
			}
			Debug.FailedAssert("Cannot retrieve battle side deployment boundaries as they are not available for this battle side.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\MissionDeploymentPlan.cs", "GetDeploymentBoundaries", 233);
			return null;
		}

		public void UpdateReinforcementPlan(BattleSideEnum side)
		{
			this._battleSideDeploymentPlans[(int)side].UpdateReinforcementPlans();
		}

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

		bool IMissionDeploymentPlan.IsPositionInsideDeploymentBoundaries(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType)
		{
			return this.IsPositionInsideDeploymentBoundaries(battleSide, position, planType);
		}

		Vec2 IMissionDeploymentPlan.GetClosestDeploymentBoundaryPosition(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType)
		{
			return this.GetClosestDeploymentBoundaryPosition(battleSide, position, planType);
		}

		public const int NumFormationsWithUnset = 11;

		private readonly Mission _mission;

		private readonly BattleSideDeploymentPlan[] _battleSideDeploymentPlans = new BattleSideDeploymentPlan[2];

		private readonly WorldFrame?[] _playerSpawnFrames = new WorldFrame?[2];

		private FormationSceneSpawnEntry[,] _formationSceneSpawnEntries;
	}
}
