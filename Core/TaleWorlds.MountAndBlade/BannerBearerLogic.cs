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
	// Token: 0x0200025C RID: 604
	public class BannerBearerLogic : MissionLogic
	{
		// Token: 0x1400002D RID: 45
		// (add) Token: 0x06002089 RID: 8329 RVA: 0x00074300 File Offset: 0x00072500
		// (remove) Token: 0x0600208A RID: 8330 RVA: 0x00074338 File Offset: 0x00072538
		public event Action<Formation> OnBannerBearersUpdated;

		// Token: 0x0600208B RID: 8331 RVA: 0x0007436D File Offset: 0x0007256D
		public BannerBearerLogic()
		{
			this._bannerSearcherUpdateTimer = new BasicMissionTimer();
		}

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x0600208C RID: 8332 RVA: 0x000743A1 File Offset: 0x000725A1
		// (set) Token: 0x0600208D RID: 8333 RVA: 0x000743A9 File Offset: 0x000725A9
		public MissionAgentSpawnLogic AgentSpawnLogic { get; private set; }

		// Token: 0x0600208E RID: 8334 RVA: 0x000743B4 File Offset: 0x000725B4
		public bool IsFormationBanner(Formation formation, SpawnedItemEntity spawnedItem)
		{
			if (!BannerBearerLogic.IsBannerItem(spawnedItem.WeaponCopy.Item))
			{
				return false;
			}
			BannerBearerLogic.FormationBannerController formationControllerFromBannerEntity = this.GetFormationControllerFromBannerEntity(spawnedItem.GameEntity);
			return formationControllerFromBannerEntity != null && formationControllerFromBannerEntity.Formation == formation;
		}

		// Token: 0x0600208F RID: 8335 RVA: 0x000743F4 File Offset: 0x000725F4
		public bool HasBannerOnGround(Formation formation)
		{
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			return formationControllerFromFormation != null && formationControllerFromFormation.HasBannerOnGround();
		}

		// Token: 0x06002090 RID: 8336 RVA: 0x00074414 File Offset: 0x00072614
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

		// Token: 0x06002091 RID: 8337 RVA: 0x00074444 File Offset: 0x00072644
		public List<Agent> GetFormationBannerBearers(Formation formation)
		{
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			if (formationControllerFromFormation != null)
			{
				return formationControllerFromFormation.BannerBearers;
			}
			return new List<Agent>();
		}

		// Token: 0x06002092 RID: 8338 RVA: 0x00074468 File Offset: 0x00072668
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

		// Token: 0x06002093 RID: 8339 RVA: 0x0007448C File Offset: 0x0007268C
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

		// Token: 0x06002094 RID: 8340 RVA: 0x000744BC File Offset: 0x000726BC
		public int GetMissingBannerCount(Formation formation)
		{
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			if (formationControllerFromFormation == null)
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

		// Token: 0x06002095 RID: 8341 RVA: 0x000744F8 File Offset: 0x000726F8
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

		// Token: 0x06002096 RID: 8342 RVA: 0x00074538 File Offset: 0x00072738
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

		// Token: 0x06002097 RID: 8343 RVA: 0x00074591 File Offset: 0x00072791
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MissionGameModels.Current.BattleBannerBearersModel.InitializeModel(this);
			this.AgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
		}

		// Token: 0x06002098 RID: 8344 RVA: 0x000745BA File Offset: 0x000727BA
		protected override void OnEndMission()
		{
			base.OnEndMission();
			MissionGameModels.Current.BattleBannerBearersModel.FinalizeModel();
			this.AgentSpawnLogic = null;
			this._isMissionEnded = true;
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x000745E0 File Offset: 0x000727E0
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

		// Token: 0x0600209A RID: 8346 RVA: 0x000746B8 File Offset: 0x000728B8
		public override void OnItemPickup(Agent agent, SpawnedItemEntity spawnedItem)
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

		// Token: 0x0600209B RID: 8347 RVA: 0x00074704 File Offset: 0x00072904
		public override void OnItemDrop(Agent agent, SpawnedItemEntity spawnedItem)
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

		// Token: 0x0600209C RID: 8348 RVA: 0x0007474C File Offset: 0x0007294C
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent.Banner != null && agentState == AgentState.Routed)
			{
				GameEntity weaponEntityFromEquipmentSlot = affectedAgent.GetWeaponEntityFromEquipmentSlot(EquipmentIndex.ExtraWeaponSlot);
				BannerBearerLogic.FormationBannerController formationControllerFromBannerEntity = this.GetFormationControllerFromBannerEntity(weaponEntityFromEquipmentSlot);
				if (formationControllerFromBannerEntity != null)
				{
					this.RemoveBannerEntity(formationControllerFromBannerEntity, weaponEntityFromEquipmentSlot);
					formationControllerFromBannerEntity.UpdateAgentStats(false);
				}
			}
		}

		// Token: 0x0600209D RID: 8349 RVA: 0x00074787 File Offset: 0x00072987
		public override void OnAgentPanicked(Agent affectedAgent)
		{
			if (affectedAgent.Banner != null)
			{
				BannerBearerLogic.ForceDropAgentBanner(affectedAgent);
			}
		}

		// Token: 0x0600209E RID: 8350 RVA: 0x00074798 File Offset: 0x00072998
		public Agent RespawnAsBannerBearer(Agent agent, bool isAlarmed, bool wieldInitialWeapons, bool forceDismounted, string specialActionSetSuffix = null, bool useTroopClassForSpawn = false)
		{
			Formation formation = agent.Formation;
			BannerBearerLogic.FormationBannerController formationControllerFromFormation = this.GetFormationControllerFromFormation(formation);
			ItemObject bannerItem = formationControllerFromFormation.BannerItem;
			Agent agent2 = base.Mission.RespawnTroop(agent, isAlarmed, wieldInitialWeapons, forceDismounted, specialActionSetSuffix, bannerItem, useTroopClassForSpawn);
			agent2.UpdateCachedAndFormationValues(false, false);
			GameEntity weaponEntityFromEquipmentSlot = agent2.GetWeaponEntityFromEquipmentSlot(EquipmentIndex.ExtraWeaponSlot);
			this.AddBannerEntity(formationControllerFromFormation, weaponEntityFromEquipmentSlot);
			formationControllerFromFormation.OnBannerEntityPickedUp(weaponEntityFromEquipmentSlot, agent2);
			return agent2;
		}

		// Token: 0x0600209F RID: 8351 RVA: 0x000747F8 File Offset: 0x000729F8
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

		// Token: 0x060020A0 RID: 8352 RVA: 0x00074866 File Offset: 0x00072A66
		public static bool IsBannerItem(ItemObject item)
		{
			return item != null && item.IsBannerItem && item.BannerComponent != null;
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x0007487E File Offset: 0x00072A7E
		private void AddBannerEntity(BannerBearerLogic.FormationBannerController formationBannerController, GameEntity bannerEntity)
		{
			this._bannerToFormationMap.Add(bannerEntity.Pointer, formationBannerController);
			formationBannerController.AddBannerEntity(bannerEntity);
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x00074899 File Offset: 0x00072A99
		private void RemoveBannerEntity(BannerBearerLogic.FormationBannerController formationBannerController, GameEntity bannerEntity)
		{
			this._bannerToFormationMap.Remove(bannerEntity.Pointer);
			formationBannerController.RemoveBannerEntity(bannerEntity);
		}

		// Token: 0x060020A3 RID: 8355 RVA: 0x000748B4 File Offset: 0x00072AB4
		private BannerBearerLogic.FormationBannerController GetFormationControllerFromFormation(Formation formation)
		{
			BannerBearerLogic.FormationBannerController formationBannerController;
			if (!this._formationBannerData.TryGetValue(formation, out formationBannerController))
			{
				return null;
			}
			return formationBannerController;
		}

		// Token: 0x060020A4 RID: 8356 RVA: 0x000748D4 File Offset: 0x00072AD4
		private BannerBearerLogic.FormationBannerController GetFormationControllerFromBannerEntity(GameEntity bannerEntity)
		{
			BannerBearerLogic.FormationBannerController formationBannerController;
			if (this._bannerToFormationMap.TryGetValue(bannerEntity.Pointer, out formationBannerController))
			{
				return formationBannerController;
			}
			return null;
		}

		// Token: 0x060020A5 RID: 8357 RVA: 0x000748F9 File Offset: 0x00072AF9
		private static void ForceDropAgentBanner(Agent agent)
		{
			if (agent != null)
			{
				ItemObject banner = agent.Banner;
			}
			agent.DropItem(EquipmentIndex.ExtraWeaponSlot, WeaponClass.Undefined);
		}

		// Token: 0x04000BF5 RID: 3061
		public const float DefaultBannerBearerAgentDefensiveness = 1f;

		// Token: 0x04000BF6 RID: 3062
		public const float BannerSearcherUpdatePeriod = 3f;

		// Token: 0x04000BF9 RID: 3065
		private readonly Dictionary<UIntPtr, BannerBearerLogic.FormationBannerController> _bannerToFormationMap = new Dictionary<UIntPtr, BannerBearerLogic.FormationBannerController>();

		// Token: 0x04000BFA RID: 3066
		private readonly Dictionary<Formation, BannerBearerLogic.FormationBannerController> _formationBannerData = new Dictionary<Formation, BannerBearerLogic.FormationBannerController>();

		// Token: 0x04000BFB RID: 3067
		private readonly BasicMissionTimer _bannerSearcherUpdateTimer;

		// Token: 0x04000BFC RID: 3068
		private readonly List<BannerBearerLogic.FormationBannerController> _playerFormationsRequiringUpdate = new List<BannerBearerLogic.FormationBannerController>();

		// Token: 0x04000BFD RID: 3069
		private bool _isMissionEnded;

		// Token: 0x02000569 RID: 1385
		private class FormationBannerController
		{
			// Token: 0x1700097E RID: 2430
			// (get) Token: 0x06003A66 RID: 14950 RVA: 0x000EBB13 File Offset: 0x000E9D13
			// (set) Token: 0x06003A67 RID: 14951 RVA: 0x000EBB1B File Offset: 0x000E9D1B
			public Formation Formation { get; private set; }

			// Token: 0x1700097F RID: 2431
			// (get) Token: 0x06003A68 RID: 14952 RVA: 0x000EBB24 File Offset: 0x000E9D24
			// (set) Token: 0x06003A69 RID: 14953 RVA: 0x000EBB2C File Offset: 0x000E9D2C
			public ItemObject BannerItem { get; private set; }

			// Token: 0x17000980 RID: 2432
			// (get) Token: 0x06003A6A RID: 14954 RVA: 0x000EBB35 File Offset: 0x000E9D35
			public bool HasBanner
			{
				get
				{
					return this.BannerItem != null;
				}
			}

			// Token: 0x17000981 RID: 2433
			// (get) Token: 0x06003A6B RID: 14955 RVA: 0x000EBB40 File Offset: 0x000E9D40
			public List<Agent> BannerBearers
			{
				get
				{
					return (from instance in this._bannerInstances.Values
						where instance.IsOnAgent
						select instance.BannerBearer).ToList<Agent>();
				}
			}

			// Token: 0x17000982 RID: 2434
			// (get) Token: 0x06003A6C RID: 14956 RVA: 0x000EBBA8 File Offset: 0x000E9DA8
			public List<GameEntity> BannersOnGround
			{
				get
				{
					return (from instance in this._bannerInstances.Values
						where instance.IsOnGround
						select instance.Entity).ToList<GameEntity>();
				}
			}

			// Token: 0x17000983 RID: 2435
			// (get) Token: 0x06003A6D RID: 14957 RVA: 0x000EBC0D File Offset: 0x000E9E0D
			public int NumberOfBannerBearers
			{
				get
				{
					return this._bannerInstances.Values.Count((BannerBearerLogic.FormationBannerController.BannerInstance instance) => instance.IsOnAgent);
				}
			}

			// Token: 0x17000984 RID: 2436
			// (get) Token: 0x06003A6E RID: 14958 RVA: 0x000EBC3E File Offset: 0x000E9E3E
			public int NumberOfBanners
			{
				get
				{
					return this._bannerInstances.Count;
				}
			}

			// Token: 0x17000985 RID: 2437
			// (get) Token: 0x06003A6F RID: 14959 RVA: 0x000EBC4B File Offset: 0x000E9E4B
			public static float BannerSearchDistance
			{
				get
				{
					return 9f;
				}
			}

			// Token: 0x06003A70 RID: 14960 RVA: 0x000EBC54 File Offset: 0x000E9E54
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

			// Token: 0x06003A71 RID: 14961 RVA: 0x000EBD0F File Offset: 0x000E9F0F
			public void SetBannerItem(ItemObject bannerItem)
			{
				if (bannerItem != null)
				{
					BannerBearerLogic.IsBannerItem(bannerItem);
				}
				this.BannerItem = bannerItem;
			}

			// Token: 0x06003A72 RID: 14962 RVA: 0x000EBD25 File Offset: 0x000E9F25
			public bool HasBannerEntity(GameEntity bannerEntity)
			{
				return bannerEntity != null && this._bannerInstances.Keys.Contains(bannerEntity.Pointer);
			}

			// Token: 0x06003A73 RID: 14963 RVA: 0x000EBD48 File Offset: 0x000E9F48
			public bool HasBannerOnGround()
			{
				if (this.HasBanner)
				{
					return this._bannerInstances.Any((KeyValuePair<UIntPtr, BannerBearerLogic.FormationBannerController.BannerInstance> instance) => instance.Value.IsOnGround);
				}
				return false;
			}

			// Token: 0x06003A74 RID: 14964 RVA: 0x000EBD7E File Offset: 0x000E9F7E
			public bool HasActiveBannerBearers()
			{
				return this.GetNumberOfActiveBannerBearers() > 0;
			}

			// Token: 0x06003A75 RID: 14965 RVA: 0x000EBD89 File Offset: 0x000E9F89
			public bool IsBannerSearchingAgent(Agent agent)
			{
				return this._bannerSearchers.Keys.Contains(agent);
			}

			// Token: 0x06003A76 RID: 14966 RVA: 0x000EBD9C File Offset: 0x000E9F9C
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

			// Token: 0x06003A77 RID: 14967 RVA: 0x000EBDEE File Offset: 0x000E9FEE
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

			// Token: 0x06003A78 RID: 14968 RVA: 0x000EBE30 File Offset: 0x000EA030
			public unsafe void RepositionFormation()
			{
				this.Formation.SetMovementOrder(*this.Formation.GetReadonlyMovementOrderReference());
				this.Formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					agent.UpdateCachedAndFormationValues(true, false);
				}, null);
			}

			// Token: 0x06003A79 RID: 14969 RVA: 0x000EBE84 File Offset: 0x000EA084
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

			// Token: 0x06003A7A RID: 14970 RVA: 0x000EC0E8 File Offset: 0x000EA2E8
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
						int num3 = MissionAgentSpawnLogic.MaxNumberOfAgentsForMission - this._mission.AllAgents.Count;
						Agent item = valueTuple.Item1;
						int num4 = (item.HasMount ? 2 : 1);
						if (num3 >= num4)
						{
							IAgentOriginBase origin = item.Origin;
							Agent agent4;
							if (valueTuple.Item2)
							{
								agent4 = this._bannerLogic.RespawnAsBannerBearer(item, true, true, false, null, this._mission.IsSallyOutBattle);
							}
							else
							{
								agent4 = this._mission.RespawnTroop(item, true, true, false, null, null, this._mission.IsSallyOutBattle);
								agent4.UpdateCachedAndFormationValues(false, false);
							}
							agent4.SetIsAIPaused(true);
						}
						else
						{
							Debug.FailedAssert("Banner bearer logic cannot respawn agent within formation " + (int)this.Formation.FormationIndex + " as mission does not have enough quota.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\FormationBannerController.cs", "UpdateBannerBearersForDeployment", 389);
							if (num4 == 1)
							{
								break;
							}
						}
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

			// Token: 0x06003A7B RID: 14971 RVA: 0x000EC3B8 File Offset: 0x000EA5B8
			public void AddBannerEntity(GameEntity entity)
			{
				if (!this._bannerInstances.ContainsKey(entity.Pointer))
				{
					this._bannerInstances.Add(entity.Pointer, new BannerBearerLogic.FormationBannerController.BannerInstance(null, entity, BannerBearerLogic.FormationBannerController.BannerState.Initialized));
				}
			}

			// Token: 0x06003A7C RID: 14972 RVA: 0x000EC3E6 File Offset: 0x000EA5E6
			public void RemoveBannerEntity(GameEntity entity)
			{
				this._bannerInstances.Remove(entity.Pointer);
				this.UpdateBannerSearchers();
				this.CheckRequiresAgentStatUpdate();
			}

			// Token: 0x06003A7D RID: 14973 RVA: 0x000EC406 File Offset: 0x000EA606
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

			// Token: 0x06003A7E RID: 14974 RVA: 0x000EC446 File Offset: 0x000EA646
			public void OnBannerEntityDropped(GameEntity entity)
			{
				this._bannerInstances[entity.Pointer] = new BannerBearerLogic.FormationBannerController.BannerInstance(null, entity, BannerBearerLogic.FormationBannerController.BannerState.OnGround);
				this.UpdateBannerSearchers();
				this.CheckRequiresAgentStatUpdate();
			}

			// Token: 0x06003A7F RID: 14975 RVA: 0x000EC46D File Offset: 0x000EA66D
			public void OnBeforeFormationMovementOrderApplied(Formation formation, MovementOrder.MovementOrderEnum orderType)
			{
				if (formation == this.Formation)
				{
					this.UpdateBannerBearerArrangementPositions();
				}
			}

			// Token: 0x06003A80 RID: 14976 RVA: 0x000EC47E File Offset: 0x000EA67E
			public void OnAfterArrangementOrderApplied(Formation formation, ArrangementOrder.ArrangementOrderEnum orderEnum)
			{
				if (formation == this.Formation)
				{
					this.UpdateBannerBearerArrangementPositions();
				}
			}

			// Token: 0x06003A81 RID: 14977 RVA: 0x000EC490 File Offset: 0x000EA690
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

			// Token: 0x06003A82 RID: 14978 RVA: 0x000EC578 File Offset: 0x000EA778
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

			// Token: 0x06003A83 RID: 14979 RVA: 0x000EC62C File Offset: 0x000EA82C
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

			// Token: 0x06003A84 RID: 14980 RVA: 0x000EC7BC File Offset: 0x000EA9BC
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

			// Token: 0x06003A85 RID: 14981 RVA: 0x000EC850 File Offset: 0x000EAA50
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

			// Token: 0x06003A86 RID: 14982 RVA: 0x000EC8E8 File Offset: 0x000EAAE8
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

			// Token: 0x06003A87 RID: 14983 RVA: 0x000EC92A File Offset: 0x000EAB2A
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

			// Token: 0x06003A88 RID: 14984 RVA: 0x000EC94F File Offset: 0x000EAB4F
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

			// Token: 0x04001D01 RID: 7425
			private int _lastActiveBannerBearerCount;

			// Token: 0x04001D02 RID: 7426
			private bool _requiresAgentStatUpdate;

			// Token: 0x04001D03 RID: 7427
			private BannerBearerLogic _bannerLogic;

			// Token: 0x04001D04 RID: 7428
			private Mission _mission;

			// Token: 0x04001D05 RID: 7429
			[TupleElementNames(new string[] { "bannerEntity", "lastDistance" })]
			private Dictionary<Agent, ValueTuple<GameEntity, float>> _bannerSearchers;

			// Token: 0x04001D06 RID: 7430
			private readonly Dictionary<UIntPtr, BannerBearerLogic.FormationBannerController.BannerInstance> _bannerInstances;

			// Token: 0x04001D07 RID: 7431
			private MBList<Agent> _nearbyAllyAgentsListCache = new MBList<Agent>();

			// Token: 0x020006F5 RID: 1781
			public enum BannerState
			{
				// Token: 0x04002324 RID: 8996
				Initialized,
				// Token: 0x04002325 RID: 8997
				OnAgent,
				// Token: 0x04002326 RID: 8998
				OnGround
			}

			// Token: 0x020006F6 RID: 1782
			public struct BannerInstance
			{
				// Token: 0x17000A20 RID: 2592
				// (get) Token: 0x06004055 RID: 16469 RVA: 0x000F9DE6 File Offset: 0x000F7FE6
				public bool IsOnGround
				{
					get
					{
						return this.State == BannerBearerLogic.FormationBannerController.BannerState.OnGround;
					}
				}

				// Token: 0x17000A21 RID: 2593
				// (get) Token: 0x06004056 RID: 16470 RVA: 0x000F9DF1 File Offset: 0x000F7FF1
				public bool IsOnAgent
				{
					get
					{
						return this.State == BannerBearerLogic.FormationBannerController.BannerState.OnAgent;
					}
				}

				// Token: 0x06004057 RID: 16471 RVA: 0x000F9DFC File Offset: 0x000F7FFC
				public BannerInstance(Agent bannerBearer, GameEntity entity, BannerBearerLogic.FormationBannerController.BannerState state)
				{
					this.BannerBearer = bannerBearer;
					this.Entity = entity;
					this.State = state;
				}

				// Token: 0x04002327 RID: 8999
				public readonly Agent BannerBearer;

				// Token: 0x04002328 RID: 9000
				public readonly GameEntity Entity;

				// Token: 0x04002329 RID: 9001
				private readonly BannerBearerLogic.FormationBannerController.BannerState State;
			}
		}
	}
}
