using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public class BannerBearerLogic : MissionLogic
	{
		public event Action<Formation> OnBannerBearersUpdated;

		public BannerBearerLogic()
		{
			this._bannerSearcherUpdateTimer = new BasicMissionTimer();
		}

		public MissionAgentSpawnLogic AgentSpawnLogic { get; private set; }

		public bool IsFormationBanner(Formation formation, SpawnedItemEntity spawnedItem)
		{
			if (!BannerBearerLogic.IsBannerItem(spawnedItem.WeaponCopy.Item))
			{
				return false;
			}
			BannerBearerLogic.FormationBannerController formationControllerFromBannerEntity = this.GetFormationControllerFromBannerEntity(spawnedItem.GameEntity);
			return formationControllerFromBannerEntity != null && formationControllerFromBannerEntity.Formation == formation;
		}

		public bool HasBannerOnGround(Formation formation)
		{
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			return formationControllerFromFormation != null && formationControllerFromFormation.HasBannerOnGround();
		}

		public BannerComponent GetActiveBanner(Formation formation)
		{
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			if (formationControllerFromFormation == null)
			{
				return null;
			}
			if (!formationControllerFromFormation.HasActiveBannerBearers())
			{
				return null;
			}
			return formationControllerFromFormation.BannerItem.BannerComponent;
		}

		public List<Agent> GetFormationBannerBearers(Formation formation)
		{
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			if (formationControllerFromFormation != null)
			{
				return formationControllerFromFormation.BannerBearers;
			}
			return new List<Agent>();
		}

		public ItemObject GetFormationBanner(Formation formation)
		{
			ItemObject itemObject = null;
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			if (formationControllerFromFormation != null)
			{
				itemObject = formationControllerFromFormation.BannerItem;
			}
			return itemObject;
		}

		public bool IsBannerSearchingAgent(Agent agent)
		{
			if (agent.Formation != null)
			{
				BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(agent.Formation);
				if (formationControllerFromFormation != null)
				{
					return formationControllerFromFormation.IsBannerSearchingAgent(agent);
				}
			}
			return false;
		}

		public int GetMissingBannerCount(Formation formation)
		{
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			if (formationControllerFromFormation == null || formationControllerFromFormation.BannerItem == null)
			{
				return 0;
			}
			int num = MissionGameModels.Current.BattleBannerBearersModel.GetDesiredNumberOfBannerBearersForFormation(formation) - formationControllerFromFormation.NumberOfBanners;
			if (num <= 0)
			{
				return 0;
			}
			return num;
		}

		public Formation GetFormationFromBanner(SpawnedItemEntity spawnedItem)
		{
			GameEntity gameEntity = spawnedItem.GameEntity;
			gameEntity = ((gameEntity == null) ? spawnedItem.GameEntityWithWorldPosition.GameEntity : gameEntity);
			BannerBearerLogic.FormationBannerController formationControllerFromBannerEntity = this.GetFormationControllerFromBannerEntity(gameEntity);
			if (formationControllerFromBannerEntity == null)
			{
				return null;
			}
			return formationControllerFromBannerEntity.Formation;
		}

		public void SetFormationBanner(Formation formation, ItemObject newBanner)
		{
			if (newBanner != null)
			{
				BannerBearerLogic.IsBannerItem(newBanner);
			}
			BannerBearerLogic.FormationBannerController formationBannerController = this.GetFormationControllerFromFormation(formation);
			if (formationBannerController != null)
			{
				if (formationBannerController.BannerItem != newBanner)
				{
					formationBannerController.SetBannerItem(newBanner);
				}
			}
			else
			{
				formationBannerController = new BannerBearerLogic.FormationBannerController(formation, newBanner, this, base.Mission);
				this._formationBannerData.Add(formation, formationBannerController);
			}
			formationBannerController.UpdateBannerBearersForDeployment();
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MissionGameModels.Current.BattleBannerBearersModel.InitializeModel(this);
			this.AgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			base.Mission.OnItemPickUp += this.OnItemPickup;
			base.Mission.OnItemDrop += this.OnItemDrop;
			this._initialSpawnEquipments.Clear();
		}

		protected override void OnEndMission()
		{
			base.OnEndMission();
			MissionGameModels.Current.BattleBannerBearersModel.FinalizeModel();
			base.Mission.OnItemPickUp -= this.OnItemPickup;
			base.Mission.OnItemDrop -= this.OnItemDrop;
			this.AgentSpawnLogic = null;
			this._isMissionEnded = true;
		}

		public override void OnDeploymentFinished()
		{
			this._initialSpawnEquipments.Clear();
			this._isMissionEnded = false;
		}

		public override void OnMissionTick(float dt)
		{
			if (this._bannerSearcherUpdateTimer.ElapsedTime >= 3f)
			{
				foreach (BannerBearerLogic.FormationBannerController formationBannerController in this._formationBannerData.Values)
				{
					formationBannerController.UpdateBannerSearchers();
				}
				this._bannerSearcherUpdateTimer.Reset();
			}
			if (base.Mission.Mode == MissionMode.Deployment && !this._playerFormationsRequiringUpdate.IsEmpty<BannerBearerLogic.FormationBannerController>())
			{
				foreach (BannerBearerLogic.FormationBannerController formationBannerController2 in this._playerFormationsRequiringUpdate)
				{
					formationBannerController2.UpdateBannerBearersForDeployment();
				}
				this._playerFormationsRequiringUpdate.Clear();
			}
		}

		public void OnItemPickup(Agent agent, SpawnedItemEntity spawnedItem)
		{
			if (!BannerBearerLogic.IsBannerItem(spawnedItem.WeaponCopy.Item))
			{
				return;
			}
			GameEntity gameEntity = spawnedItem.GameEntityWithWorldPosition.GameEntity;
			BannerBearerLogic.FormationBannerController formationControllerFromBannerEntity = this.GetFormationControllerFromBannerEntity(gameEntity);
			if (formationControllerFromBannerEntity != null)
			{
				formationControllerFromBannerEntity.OnBannerEntityPickedUp(gameEntity, agent);
				formationControllerFromBannerEntity.UpdateAgentStats(false);
			}
		}

		public void OnItemDrop(Agent agent, SpawnedItemEntity spawnedItem)
		{
			if (!BannerBearerLogic.IsBannerItem(spawnedItem.WeaponCopy.Item))
			{
				return;
			}
			BannerBearerLogic.FormationBannerController formationControllerFromBannerEntity = this.GetFormationControllerFromBannerEntity(spawnedItem.GameEntity);
			if (formationControllerFromBannerEntity != null)
			{
				formationControllerFromBannerEntity.OnBannerEntityDropped(spawnedItem.GameEntity);
				formationControllerFromBannerEntity.UpdateAgentStats(false);
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent.Banner != null && agentState == AgentState.Routed)
			{
				this.RemoveBannerOfAgent(affectedAgent);
			}
		}

		public override void OnAgentPanicked(Agent affectedAgent)
		{
			if (affectedAgent.Banner != null)
			{
				BannerBearerLogic.ForceDropAgentBanner(affectedAgent);
			}
		}

		public void UpdateAgent(Agent agent, bool willBecomeBannerBearer)
		{
			if (willBecomeBannerBearer)
			{
				Formation formation = agent.Formation;
				BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
				ItemObject bannerItem = formationControllerFromFormation.BannerItem;
				Equipment equipment = this.CreateBannerEquipmentForAgent(agent, bannerItem);
				agent.UpdateSpawnEquipmentAndRefreshVisuals(equipment);
				GameEntity weaponEntityFromEquipmentSlot = agent.GetWeaponEntityFromEquipmentSlot(EquipmentIndex.ExtraWeaponSlot);
				this.AddBannerEntity(formationControllerFromFormation, weaponEntityFromEquipmentSlot);
				formationControllerFromFormation.OnBannerEntityPickedUp(weaponEntityFromEquipmentSlot, agent);
			}
			else if (agent.Banner != null)
			{
				this.RemoveBannerOfAgent(agent);
				agent.UpdateSpawnEquipmentAndRefreshVisuals(this._initialSpawnEquipments[agent]);
			}
			agent.UpdateCachedAndFormationValues(false, false);
			agent.SetIsAIPaused(true);
		}

		public Agent SpawnBannerBearer(IAgentOriginBase troopOrigin, bool isPlayerSide, Formation formation, bool spawnWithHorse, bool isReinforcement, int formationTroopCount, int formationTroopIndex, bool isAlarmed, bool wieldInitialWeapons, bool forceDismounted, Vec3? initialPosition, Vec2? initialDirection, string specialActionSetSuffix = null, bool useTroopClassForSpawn = false)
		{
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			ItemObject bannerItem = formationControllerFromFormation.BannerItem;
			Agent agent = base.Mission.SpawnTroop(troopOrigin, isPlayerSide, true, spawnWithHorse, isReinforcement, formationTroopCount, formationTroopIndex, isAlarmed, wieldInitialWeapons, forceDismounted, initialPosition, initialDirection, specialActionSetSuffix, bannerItem, formationControllerFromFormation.Formation.FormationIndex, useTroopClassForSpawn);
			agent.UpdateCachedAndFormationValues(false, false);
			GameEntity weaponEntityFromEquipmentSlot = agent.GetWeaponEntityFromEquipmentSlot(EquipmentIndex.ExtraWeaponSlot);
			this.AddBannerEntity(formationControllerFromFormation, weaponEntityFromEquipmentSlot);
			formationControllerFromFormation.OnBannerEntityPickedUp(weaponEntityFromEquipmentSlot, agent);
			return agent;
		}

		public static bool IsBannerItem(ItemObject item)
		{
			return item != null && item.IsBannerItem && item.BannerComponent != null;
		}

		private void AddBannerEntity(BannerBearerLogic.FormationBannerController formationBannerController, GameEntity bannerEntity)
		{
			this._bannerToFormationMap.Add(bannerEntity.Pointer, formationBannerController);
			formationBannerController.AddBannerEntity(bannerEntity);
		}

		private void RemoveBannerEntity(BannerBearerLogic.FormationBannerController formationBannerController, GameEntity bannerEntity)
		{
			this._bannerToFormationMap.Remove(bannerEntity.Pointer);
			formationBannerController.RemoveBannerEntity(bannerEntity);
		}

		private BannerBearerLogic.FormationBannerController GetFormationControllerFromFormation(Formation formation)
		{
			BannerBearerLogic.FormationBannerController formationBannerController;
			if (!this._formationBannerData.TryGetValue(formation, out formationBannerController))
			{
				return null;
			}
			return formationBannerController;
		}

		private BannerBearerLogic.FormationBannerController GetFormationControllerFromBannerEntity(GameEntity bannerEntity)
		{
			BannerBearerLogic.FormationBannerController formationBannerController;
			if (this._bannerToFormationMap.TryGetValue(bannerEntity.Pointer, out formationBannerController))
			{
				return formationBannerController;
			}
			return null;
		}

		private Equipment CreateBannerEquipmentForAgent(Agent agent, ItemObject bannerItem)
		{
			Equipment spawnEquipment = agent.SpawnEquipment;
			if (!this._initialSpawnEquipments.ContainsKey(agent))
			{
				this._initialSpawnEquipments[agent] = spawnEquipment;
			}
			Equipment equipment = new Equipment(spawnEquipment);
			ItemObject bannerBearerReplacementWeapon = MissionGameModels.Current.BattleBannerBearersModel.GetBannerBearerReplacementWeapon(agent.Character);
			equipment[EquipmentIndex.WeaponItemBeginSlot] = new EquipmentElement(bannerBearerReplacementWeapon, null, null, false);
			for (int i = 1; i < 4; i++)
			{
				equipment[i] = default(EquipmentElement);
			}
			equipment[EquipmentIndex.ExtraWeaponSlot] = new EquipmentElement(bannerItem, null, null, false);
			return equipment;
		}

		private void RemoveBannerOfAgent(Agent agent)
		{
			GameEntity weaponEntityFromEquipmentSlot = agent.GetWeaponEntityFromEquipmentSlot(EquipmentIndex.ExtraWeaponSlot);
			BannerBearerLogic.FormationBannerController formationControllerFromBannerEntity = this.GetFormationControllerFromBannerEntity(weaponEntityFromEquipmentSlot);
			if (formationControllerFromBannerEntity != null)
			{
				this.RemoveBannerEntity(formationControllerFromBannerEntity, weaponEntityFromEquipmentSlot);
				formationControllerFromBannerEntity.UpdateAgentStats(false);
			}
		}

		private static void ForceDropAgentBanner(Agent agent)
		{
			if (agent != null)
			{
				ItemObject banner = agent.Banner;
			}
			agent.DropItem(EquipmentIndex.ExtraWeaponSlot, WeaponClass.Undefined);
		}

		public const float DefaultBannerBearerAgentDefensiveness = 1f;

		public const float BannerSearcherUpdatePeriod = 3f;

		private readonly Dictionary<UIntPtr, BannerBearerLogic.FormationBannerController> _bannerToFormationMap = new Dictionary<UIntPtr, BannerBearerLogic.FormationBannerController>();

		private readonly Dictionary<Formation, BannerBearerLogic.FormationBannerController> _formationBannerData = new Dictionary<Formation, BannerBearerLogic.FormationBannerController>();

		private readonly Dictionary<Agent, Equipment> _initialSpawnEquipments = new Dictionary<Agent, Equipment>();

		private readonly BasicMissionTimer _bannerSearcherUpdateTimer;

		private readonly List<BannerBearerLogic.FormationBannerController> _playerFormationsRequiringUpdate = new List<BannerBearerLogic.FormationBannerController>();

		private bool _isMissionEnded;

		private class FormationBannerController
		{
			public Formation Formation { get; private set; }

			public ItemObject BannerItem { get; private set; }

			public bool HasBanner
			{
				get
				{
					return this.BannerItem != null;
				}
			}

			public List<Agent> BannerBearers
			{
				get
				{
					return (from instance in this._bannerInstances.Values
						where instance.IsOnAgent
						select instance.BannerBearer).ToList<Agent>();
				}
			}

			public List<GameEntity> BannersOnGround
			{
				get
				{
					return (from instance in this._bannerInstances.Values
						where instance.IsOnGround
						select instance.Entity).ToList<GameEntity>();
				}
			}

			public int NumberOfBannerBearers
			{
				get
				{
					return this._bannerInstances.Values.Count((BannerBearerLogic.FormationBannerController.BannerInstance instance) => instance.IsOnAgent);
				}
			}

			public int NumberOfBanners
			{
				get
				{
					return this._bannerInstances.Count;
				}
			}

			public static float BannerSearchDistance
			{
				get
				{
					return 9f;
				}
			}

			public FormationBannerController(Formation formation, ItemObject bannerItem, BannerBearerLogic bannerLogic, Mission mission)
			{
				this.Formation = formation;
				this.Formation.OnUnitAdded += this.OnAgentAdded;
				this.Formation.OnUnitRemoved += this.OnAgentRemoved;
				this.Formation.OnBeforeMovementOrderApplied += this.OnBeforeFormationMovementOrderApplied;
				this.Formation.OnAfterArrangementOrderApplied += this.OnAfterArrangementOrderApplied;
				this._bannerInstances = new Dictionary<UIntPtr, BannerBearerLogic.FormationBannerController.BannerInstance>();
				this._bannerSearchers = new Dictionary<Agent, ValueTuple<GameEntity, float>>();
				this._requiresAgentStatUpdate = false;
				this._lastActiveBannerBearerCount = 0;
				this._bannerLogic = bannerLogic;
				this._mission = mission;
				this.SetBannerItem(bannerItem);
			}

			public void SetBannerItem(ItemObject bannerItem)
			{
				if (bannerItem != null)
				{
					BannerBearerLogic.IsBannerItem(bannerItem);
				}
				this.BannerItem = bannerItem;
			}

			public bool HasBannerEntity(GameEntity bannerEntity)
			{
				return bannerEntity != null && this._bannerInstances.Keys.Contains(bannerEntity.Pointer);
			}

			public bool HasBannerOnGround()
			{
				if (this.HasBanner)
				{
					return this._bannerInstances.Any((KeyValuePair<UIntPtr, BannerBearerLogic.FormationBannerController.BannerInstance> instance) => instance.Value.IsOnGround);
				}
				return false;
			}

			public bool HasActiveBannerBearers()
			{
				return this.GetNumberOfActiveBannerBearers() > 0;
			}

			public bool IsBannerSearchingAgent(Agent agent)
			{
				return this._bannerSearchers.Keys.Contains(agent);
			}

			public int GetNumberOfActiveBannerBearers()
			{
				int num = 0;
				if (this.HasBanner)
				{
					BattleBannerBearersModel bannerBearersModel = MissionGameModels.Current.BattleBannerBearersModel;
					num = this._bannerInstances.Values.Count((BannerBearerLogic.FormationBannerController.BannerInstance instance) => instance.IsOnAgent && bannerBearersModel.CanBannerBearerProvideEffectToFormation(instance.BannerBearer, this.Formation));
				}
				return num;
			}

			public void UpdateAgentStats(bool forceUpdate = false)
			{
				if (forceUpdate || this._requiresAgentStatUpdate)
				{
					this.Formation.ApplyActionOnEachUnit(delegate(Agent agent)
					{
						agent.UpdateAgentProperties();
						Agent mountAgent = agent.MountAgent;
						if (mountAgent != null)
						{
							mountAgent.UpdateAgentProperties();
						}
					}, null);
					this._requiresAgentStatUpdate = false;
				}
			}

			public unsafe void RepositionFormation()
			{
				this.Formation.SetMovementOrder(*this.Formation.GetReadonlyMovementOrderReference());
				this.Formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					agent.UpdateCachedAndFormationValues(true, false);
				}, null);
			}

			public void UpdateBannerSearchers()
			{
				List<GameEntity> bannersOnGround = this.BannersOnGround;
				if (!this._bannerSearchers.IsEmpty<KeyValuePair<Agent, ValueTuple<GameEntity, float>>>())
				{
					List<Agent> list = new List<Agent>();
					using (Dictionary<Agent, ValueTuple<GameEntity, float>>.Enumerator enumerator = this._bannerSearchers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<Agent, ValueTuple<GameEntity, float>> searcherTuple = enumerator.Current;
							Agent key = searcherTuple.Key;
							if (key.IsActive())
							{
								if (!bannersOnGround.Any((GameEntity bannerEntity) => bannerEntity.Pointer == searcherTuple.Value.Item1.Pointer))
								{
									list.Add(key);
								}
							}
							else
							{
								list.Add(key);
							}
						}
					}
					foreach (Agent agent in list)
					{
						this.RemoveBannerSearcher(agent);
					}
				}
				using (List<GameEntity>.Enumerator enumerator3 = bannersOnGround.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						GameEntity banner = enumerator3.Current;
						bool flag = false;
						if (this._bannerSearchers.IsEmpty<KeyValuePair<Agent, ValueTuple<GameEntity, float>>>())
						{
							flag = true;
						}
						else
						{
							KeyValuePair<Agent, ValueTuple<GameEntity, float>> keyValuePair = this._bannerSearchers.FirstOrDefault(([TupleElementNames(new string[] { "bannerEntity", "lastDistance" })] KeyValuePair<Agent, ValueTuple<GameEntity, float>> tuple) => tuple.Value.Item1.Pointer == banner.Pointer);
							if (keyValuePair.Key == null)
							{
								flag = true;
							}
							else
							{
								Agent key2 = keyValuePair.Key;
								if (key2.IsActive())
								{
									GameEntity item = keyValuePair.Value.Item1;
									float item2 = keyValuePair.Value.Item2;
									float num = key2.Position.AsVec2.Distance(item.GlobalPosition.AsVec2);
									if (num <= item2 && num < BannerBearerLogic.FormationBannerController.BannerSearchDistance)
									{
										this._bannerSearchers[key2] = new ValueTuple<GameEntity, float>(item, num);
									}
									else
									{
										this.RemoveBannerSearcher(key2);
										flag = true;
									}
								}
								else
								{
									this.RemoveBannerSearcher(key2);
									flag = true;
								}
							}
						}
						if (flag)
						{
							float num2;
							Agent agent2 = this.FindBestSearcherForBanner(banner, out num2);
							if (agent2 != null)
							{
								this.AddBannerSearcher(agent2, banner, num2);
							}
						}
					}
				}
			}

			public void UpdateBannerBearersForDeployment()
			{
				List<Agent> bannerBearers = this.BannerBearers;
				List<ValueTuple<Agent, bool>> list = new List<ValueTuple<Agent, bool>>();
				List<Agent> list2 = new List<Agent>();
				int num = 0;
				BattleBannerBearersModel battleBannerBearersModel = MissionGameModels.Current.BattleBannerBearersModel;
				if (battleBannerBearersModel.CanFormationDeployBannerBearers(this.Formation))
				{
					num = battleBannerBearersModel.GetDesiredNumberOfBannerBearersForFormation(this.Formation);
					using (List<Agent>.Enumerator enumerator = bannerBearers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Agent agent = enumerator.Current;
							if (num > 0 && agent.Formation == this.Formation)
							{
								num--;
							}
							else
							{
								list2.Add(agent);
							}
						}
						goto IL_92;
					}
				}
				list2.AddRange(bannerBearers);
				IL_92:
				foreach (Agent agent2 in list2)
				{
					bool flag = false;
					if (num > 0)
					{
						flag = true;
						num--;
					}
					list.Add(new ValueTuple<Agent, bool>(agent2, flag));
				}
				if (num > 0)
				{
					List<Agent> list3 = this.FindBannerBearableAgents(num);
					int num2 = 0;
					while (num2 < list3.Count && num > 0)
					{
						Agent agent3 = list3[num2];
						list2.Add(agent3);
						list.Add(new ValueTuple<Agent, bool>(agent3, true));
						num--;
						num2++;
					}
				}
				if (!list.IsEmpty<ValueTuple<Agent, bool>>())
				{
					BattleSideEnum side = this.Formation.Team.Side;
					this._bannerLogic.AgentSpawnLogic.GetSpawnHorses(side);
					BattleSideEnum side2 = this._mission.PlayerTeam.Side;
					foreach (ValueTuple<Agent, bool> valueTuple in list)
					{
						this._bannerLogic.UpdateAgent(valueTuple.Item1, valueTuple.Item2);
					}
				}
				this.UpdateAgentStats(false);
				this.RepositionFormation();
				Action<Formation> onBannerBearersUpdated = this._bannerLogic.OnBannerBearersUpdated;
				if (onBannerBearersUpdated == null)
				{
					return;
				}
				onBannerBearersUpdated(this.Formation);
			}

			public void AddBannerEntity(GameEntity entity)
			{
				if (!this._bannerInstances.ContainsKey(entity.Pointer))
				{
					this._bannerInstances.Add(entity.Pointer, new BannerBearerLogic.FormationBannerController.BannerInstance(null, entity, BannerBearerLogic.FormationBannerController.BannerState.Initialized));
				}
			}

			public void RemoveBannerEntity(GameEntity entity)
			{
				this._bannerInstances.Remove(entity.Pointer);
				this.UpdateBannerSearchers();
				this.CheckRequiresAgentStatUpdate();
			}

			public void OnBannerEntityPickedUp(GameEntity entity, Agent agent)
			{
				this._bannerInstances[entity.Pointer] = new BannerBearerLogic.FormationBannerController.BannerInstance(agent, entity, BannerBearerLogic.FormationBannerController.BannerState.OnAgent);
				if (agent.IsAIControlled)
				{
					agent.ResetEnemyCaches();
					agent.Defensiveness = 1f;
				}
				this.UpdateBannerSearchers();
				this.CheckRequiresAgentStatUpdate();
			}

			public void OnBannerEntityDropped(GameEntity entity)
			{
				this._bannerInstances[entity.Pointer] = new BannerBearerLogic.FormationBannerController.BannerInstance(null, entity, BannerBearerLogic.FormationBannerController.BannerState.OnGround);
				this.UpdateBannerSearchers();
				this.CheckRequiresAgentStatUpdate();
			}

			public void OnBeforeFormationMovementOrderApplied(Formation formation, MovementOrder.MovementOrderEnum orderType)
			{
				if (formation == this.Formation)
				{
					this.UpdateBannerBearerArrangementPositions();
				}
			}

			public void OnAfterArrangementOrderApplied(Formation formation, ArrangementOrder.ArrangementOrderEnum orderEnum)
			{
				if (formation == this.Formation)
				{
					this.UpdateBannerBearerArrangementPositions();
				}
			}

			private Agent FindBestSearcherForBanner(GameEntity banner, out float distance)
			{
				distance = float.MaxValue;
				Agent agent = null;
				Vec2 asVec = banner.GlobalPosition.AsVec2;
				this._mission.GetNearbyAllyAgents(asVec, BannerBearerLogic.FormationBannerController.BannerSearchDistance, this.Formation.Team, this._nearbyAllyAgentsListCache);
				BattleBannerBearersModel battleBannerBearersModel = MissionGameModels.Current.BattleBannerBearersModel;
				foreach (Agent agent2 in this._nearbyAllyAgentsListCache)
				{
					if (agent2.Formation == this.Formation && battleBannerBearersModel.CanAgentPickUpAnyBanner(agent2))
					{
						float num = agent2.Position.AsVec2.Distance(asVec);
						if (num < distance && !this._bannerSearchers.ContainsKey(agent2))
						{
							agent = agent2;
							distance = num;
						}
					}
				}
				return agent;
			}

			private List<Agent> FindBannerBearableAgents(int count)
			{
				List<Agent> list = new List<Agent>();
				if (count > 0)
				{
					BattleBannerBearersModel bannerBearerModel = MissionGameModels.Current.BattleBannerBearersModel;
					using (List<IFormationUnit>.Enumerator enumerator = this.Formation.UnitsWithoutLooseDetachedOnes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Agent agent2;
							if ((agent2 = enumerator.Current as Agent) != null && agent2.Banner == null && bannerBearerModel.CanAgentBecomeBannerBearer(agent2))
							{
								list.Add(agent2);
							}
						}
					}
					list = list.OrderByDescending((Agent agent) => bannerBearerModel.GetAgentBannerBearingPriority(agent)).ToList<Agent>();
				}
				return list;
			}

			private void UpdateBannerBearerArrangementPositions()
			{
				List<Agent> list = (from instance in this._bannerInstances.Values
					where instance.IsOnAgent && instance.BannerBearer.Formation == this.Formation
					select instance.BannerBearer).ToList<Agent>();
				List<FormationArrangementModel.ArrangementPosition> bannerBearerPositions = MissionGameModels.Current.FormationArrangementsModel.GetBannerBearerPositions(this.Formation, list.Count);
				if (bannerBearerPositions == null || bannerBearerPositions.IsEmpty<FormationArrangementModel.ArrangementPosition>())
				{
					return;
				}
				int i = 0;
				foreach (Agent agent in list)
				{
					if (agent != null && agent.IsAIControlled && agent.Formation == this.Formation)
					{
						int num;
						int num2;
						agent.GetFormationFileAndRankInfo(out num, out num2);
						while (i < bannerBearerPositions.Count)
						{
							FormationArrangementModel.ArrangementPosition arrangementPosition = bannerBearerPositions[i];
							int fileIndex = arrangementPosition.FileIndex;
							int rankIndex = arrangementPosition.RankIndex;
							bool flag = num == fileIndex && num2 == rankIndex;
							if (!flag)
							{
								IFormationUnit unit = this.Formation.Arrangement.GetUnit(fileIndex, rankIndex);
								Agent agent2;
								if (unit != null && (agent2 = unit as Agent) != null)
								{
									if (agent2 == agent)
									{
										flag = true;
									}
									else if (agent2 != this.Formation.Captain)
									{
										this.Formation.SwitchUnitLocations(agent, agent2);
										flag = true;
									}
								}
							}
							if (flag)
							{
								i++;
								break;
							}
							i++;
						}
					}
				}
			}

			private void OnAgentAdded(Formation formation, Agent agent)
			{
				if (this.Formation == formation)
				{
					if (!this._bannerLogic._isMissionEnded && this._mission.Mode == MissionMode.Deployment && formation.Team.IsPlayerTeam && MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle())
					{
						int minimumFormationTroopCountToBearBanners = MissionGameModels.Current.BattleBannerBearersModel.GetMinimumFormationTroopCountToBearBanners();
						if (formation.CountOfUnits == minimumFormationTroopCountToBearBanners && !this._bannerLogic._playerFormationsRequiringUpdate.Contains(this))
						{
							this._bannerLogic._playerFormationsRequiringUpdate.Add(this);
							return;
						}
					}
					else
					{
						this.UpdateBannerSearchers();
					}
				}
			}

			private void OnAgentRemoved(Formation formation, Agent agent)
			{
				if (this.Formation == formation)
				{
					if (!this._bannerLogic._isMissionEnded && this._mission.Mode == MissionMode.Deployment && formation.Team.IsPlayerTeam && MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle())
					{
						int minimumFormationTroopCountToBearBanners = MissionGameModels.Current.BattleBannerBearersModel.GetMinimumFormationTroopCountToBearBanners();
						if (formation.CountOfUnits == minimumFormationTroopCountToBearBanners - 1 && !this._bannerLogic._playerFormationsRequiringUpdate.Contains(this))
						{
							this._bannerLogic._playerFormationsRequiringUpdate.Add(this);
							return;
						}
					}
					else
					{
						this.UpdateBannerSearchers();
					}
				}
			}

			private void CheckRequiresAgentStatUpdate()
			{
				if (!this._requiresAgentStatUpdate)
				{
					int numberOfActiveBannerBearers = this.GetNumberOfActiveBannerBearers();
					if ((numberOfActiveBannerBearers > 0 && this._lastActiveBannerBearerCount == 0) || (numberOfActiveBannerBearers == 0 && this._lastActiveBannerBearerCount > 0))
					{
						this._requiresAgentStatUpdate = true;
						this._lastActiveBannerBearerCount = numberOfActiveBannerBearers;
					}
				}
			}

			private void AddBannerSearcher(Agent searcher, GameEntity banner, float distance)
			{
				this._bannerSearchers.Add(searcher, new ValueTuple<GameEntity, float>(banner, distance));
				HumanAIComponent humanAIComponent = searcher.HumanAIComponent;
				if (humanAIComponent == null)
				{
					return;
				}
				humanAIComponent.DisablePickUpForAgentIfNeeded();
			}

			private void RemoveBannerSearcher(Agent searcher)
			{
				this._bannerSearchers.Remove(searcher);
				if (searcher.IsActive())
				{
					HumanAIComponent humanAIComponent = searcher.HumanAIComponent;
					if (humanAIComponent == null)
					{
						return;
					}
					humanAIComponent.DisablePickUpForAgentIfNeeded();
				}
			}

			private int _lastActiveBannerBearerCount;

			private bool _requiresAgentStatUpdate;

			private BannerBearerLogic _bannerLogic;

			private Mission _mission;

			[TupleElementNames(new string[] { "bannerEntity", "lastDistance" })]
			private Dictionary<Agent, ValueTuple<GameEntity, float>> _bannerSearchers;

			private readonly Dictionary<UIntPtr, BannerBearerLogic.FormationBannerController.BannerInstance> _bannerInstances;

			private MBList<Agent> _nearbyAllyAgentsListCache = new MBList<Agent>();

			public enum BannerState
			{
				Initialized,
				OnAgent,
				OnGround
			}

			public struct BannerInstance
			{
				public bool IsOnGround
				{
					get
					{
						return this.State == BannerBearerLogic.FormationBannerController.BannerState.OnGround;
					}
				}

				public bool IsOnAgent
				{
					get
					{
						return this.State == BannerBearerLogic.FormationBannerController.BannerState.OnAgent;
					}
				}

				public BannerInstance(Agent bannerBearer, GameEntity entity, BannerBearerLogic.FormationBannerController.BannerState state)
				{
					this.BannerBearer = bannerBearer;
					this.Entity = entity;
					this.State = state;
				}

				public readonly Agent BannerBearer;

				public readonly GameEntity Entity;

				private readonly BannerBearerLogic.FormationBannerController.BannerState State;
			}
		}
	}
}
