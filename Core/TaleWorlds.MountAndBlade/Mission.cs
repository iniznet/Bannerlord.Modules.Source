using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.ComponentInterfaces;
using TaleWorlds.MountAndBlade.Network;

namespace TaleWorlds.MountAndBlade
{
	public sealed class Mission : DotNetObject, IMission
	{
		internal UIntPtr Pointer { get; private set; }

		public bool IsFinalized
		{
			get
			{
				return this.Pointer == UIntPtr.Zero;
			}
		}

		public static Mission Current
		{
			get
			{
				Mission current = Mission._current;
				return Mission._current;
			}
			private set
			{
				if (value == null)
				{
					Mission current = Mission._current;
				}
				Mission._current = value;
			}
		}

		private MissionInitializerRecord InitializerRecord { get; set; }

		public string SceneName
		{
			get
			{
				return this.InitializerRecord.SceneName;
			}
		}

		public string SceneLevels
		{
			get
			{
				return this.InitializerRecord.SceneLevels;
			}
		}

		public float DamageToPlayerMultiplier
		{
			get
			{
				return this.InitializerRecord.DamageToPlayerMultiplier;
			}
		}

		public float DamageToFriendsMultiplier
		{
			get
			{
				return this.InitializerRecord.DamageToFriendsMultiplier;
			}
		}

		public float DamageFromPlayerToFriendsMultiplier
		{
			get
			{
				return this.InitializerRecord.DamageFromPlayerToFriendsMultiplier;
			}
		}

		public bool HasValidTerrainType
		{
			get
			{
				return this.InitializerRecord.TerrainType >= 0;
			}
		}

		public TerrainType TerrainType
		{
			get
			{
				if (!this.HasValidTerrainType)
				{
					return TerrainType.Water;
				}
				return (TerrainType)this.InitializerRecord.TerrainType;
			}
		}

		public Scene Scene { get; private set; }

		public IEnumerable<GameEntity> GetActiveEntitiesWithScriptComponentOfType<T>()
		{
			return from amo in this._activeMissionObjects
				where amo is T
				select amo.GameEntity;
		}

		public void AddActiveMissionObject(MissionObject missionObject)
		{
			this._missionObjects.Add(missionObject);
			this._activeMissionObjects.Add(missionObject);
		}

		public void ActivateMissionObject(MissionObject missionObject)
		{
			this._activeMissionObjects.Add(missionObject);
		}

		public void DeactivateMissionObject(MissionObject missionObject)
		{
			this._activeMissionObjects.Remove(missionObject);
		}

		public MBReadOnlyList<MissionObject> ActiveMissionObjects
		{
			get
			{
				return this._activeMissionObjects;
			}
		}

		public MBReadOnlyList<MissionObject> MissionObjects
		{
			get
			{
				return this._missionObjects;
			}
		}

		public MBReadOnlyList<Mission.DynamicallyCreatedEntity> AddedEntitiesInfo
		{
			get
			{
				return this._addedEntitiesInfo;
			}
		}

		public Mission.MBBoundaryCollection Boundaries { get; private set; }

		public bool IsMainAgentObjectInteractionEnabled
		{
			get
			{
				switch (this._missionMode)
				{
				case MissionMode.Conversation:
				case MissionMode.Barter:
				case MissionMode.Deployment:
				case MissionMode.Replay:
				case MissionMode.CutScene:
					return false;
				}
				return !this.MissionEnded && this._isMainAgentObjectInteractionEnabled;
			}
			set
			{
				this._isMainAgentObjectInteractionEnabled = value;
			}
		}

		public bool IsMainAgentItemInteractionEnabled
		{
			get
			{
				switch (this._missionMode)
				{
				case MissionMode.Conversation:
				case MissionMode.Barter:
				case MissionMode.Deployment:
				case MissionMode.Replay:
				case MissionMode.CutScene:
					return false;
				}
				return this._isMainAgentItemInteractionEnabled;
			}
			set
			{
				this._isMainAgentItemInteractionEnabled = value;
			}
		}

		public bool IsTeleportingAgents { get; set; }

		public bool ForceTickOccasionally { get; set; }

		private void FinalizeMission()
		{
			TeamAISiegeComponent.OnMissionFinalize();
			MBAPI.IMBMission.FinalizeMission(this.Pointer);
			this.Pointer = UIntPtr.Zero;
		}

		public Mission.MissionCombatType CombatType
		{
			get
			{
				return (Mission.MissionCombatType)MBAPI.IMBMission.GetCombatType(this.Pointer);
			}
			set
			{
				MBAPI.IMBMission.SetCombatType(this.Pointer, (int)value);
			}
		}

		public void SetMissionCombatType(Mission.MissionCombatType missionCombatType)
		{
			MBAPI.IMBMission.SetCombatType(this.Pointer, (int)missionCombatType);
		}

		public MissionMode Mode
		{
			get
			{
				return this._missionMode;
			}
		}

		public void ConversationCharacterChanged()
		{
			foreach (IMissionListener missionListener in this._listeners)
			{
				missionListener.OnConversationCharacterChanged();
			}
		}

		public void SetMissionMode(MissionMode newMode, bool atStart)
		{
			if (this._missionMode != newMode)
			{
				MissionMode missionMode = this._missionMode;
				this._missionMode = newMode;
				if (this.CurrentState != Mission.State.Over)
				{
					for (int i = 0; i < this.MissionBehaviors.Count; i++)
					{
						this.MissionBehaviors[i].OnMissionModeChange(missionMode, atStart);
					}
					foreach (IMissionListener missionListener in this._listeners)
					{
						missionListener.OnMissionModeChange(missionMode, atStart);
					}
				}
			}
		}

		private Mission.AgentCreationResult CreateAgentInternal(AgentFlag agentFlags, int forcedAgentIndex, bool isFemale, ref AgentSpawnData spawnData, ref AgentCapsuleData capsuleData, ref AnimationSystemData animationSystemData, int instanceNo)
		{
			return MBAPI.IMBMission.CreateAgent(this.Pointer, (ulong)agentFlags, forcedAgentIndex, isFemale, ref spawnData, ref capsuleData.BodyCap, ref capsuleData.CrouchedBodyCap, ref animationSystemData, instanceNo);
		}

		public float CurrentTime
		{
			get
			{
				return this._cachedMissionTime;
			}
		}

		public bool PauseAITick
		{
			get
			{
				return MBAPI.IMBMission.GetPauseAITick(this.Pointer);
			}
			set
			{
				MBAPI.IMBMission.SetPauseAITick(this.Pointer, value);
			}
		}

		[UsedImplicitly]
		[MBCallback]
		internal void UpdateMissionTimeCache(float curTime)
		{
			this._cachedMissionTime = curTime;
		}

		public float GetAverageFps()
		{
			return MBAPI.IMBMission.GetAverageFps(this.Pointer);
		}

		public static bool ToggleDisableFallAvoid()
		{
			return MBAPI.IMBMission.ToggleDisableFallAvoid();
		}

		public bool IsPositionInsideBoundaries(Vec2 position)
		{
			return MBAPI.IMBMission.IsPositionInsideBoundaries(this.Pointer, position);
		}

		public bool IsPositionInsideAnyBlockerNavMeshFace2D(Vec2 position)
		{
			return MBAPI.IMBMission.IsPositionInsideAnyBlockerNavMeshFace2D(this.Pointer, position);
		}

		private bool IsFormationUnitPositionAvailableAux(ref WorldPosition formationPosition, ref WorldPosition unitPosition, ref WorldPosition nearestAvailableUnitPosition, float manhattanDistance)
		{
			return MBAPI.IMBMission.IsFormationUnitPositionAvailable(this.Pointer, ref formationPosition, ref unitPosition, ref nearestAvailableUnitPosition, manhattanDistance);
		}

		public Agent RayCastForClosestAgent(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, int excludedAgentIndex = -1, float rayThickness = 0.01f)
		{
			collisionDistance = float.NaN;
			return MBAPI.IMBMission.RayCastForClosestAgent(this.Pointer, sourcePoint, targetPoint, excludedAgentIndex, ref collisionDistance, rayThickness);
		}

		public bool RayCastForClosestAgentsLimbs(Vec3 sourcePoint, Vec3 targetPoint, int excludeAgentIndex, out float collisionDistance, ref int agentIndex, ref sbyte boneIndex)
		{
			collisionDistance = float.NaN;
			return MBAPI.IMBMission.RayCastForClosestAgentsLimbs(this.Pointer, sourcePoint, targetPoint, excludeAgentIndex, ref collisionDistance, ref agentIndex, ref boneIndex);
		}

		public bool RayCastForClosestAgentsLimbs(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, ref int agentIndex, ref sbyte boneIndex)
		{
			collisionDistance = float.NaN;
			return MBAPI.IMBMission.RayCastForClosestAgentsLimbs(this.Pointer, sourcePoint, targetPoint, -1, ref collisionDistance, ref agentIndex, ref boneIndex);
		}

		public bool RayCastForGivenAgentsLimbs(Vec3 sourcePoint, Vec3 rayFinishPoint, int givenAgentIndex, ref float collisionDistance, ref sbyte boneIndex)
		{
			return MBAPI.IMBMission.RayCastForGivenAgentsLimbs(this.Pointer, sourcePoint, rayFinishPoint, givenAgentIndex, ref collisionDistance, ref boneIndex);
		}

		internal AgentProximityMap.ProximityMapSearchStructInternal ProximityMapBeginSearch(Vec2 searchPos, float searchRadius)
		{
			return MBAPI.IMBMission.ProximityMapBeginSearch(this.Pointer, searchPos, searchRadius);
		}

		internal float ProximityMapMaxSearchRadius()
		{
			return MBAPI.IMBMission.ProximityMapMaxSearchRadius(this.Pointer);
		}

		public float GetBiggestAgentCollisionPadding()
		{
			return MBAPI.IMBMission.GetBiggestAgentCollisionPadding(this.Pointer);
		}

		public void SetMissionCorpseFadeOutTimeInSeconds(float corpseFadeOutTimeInSeconds)
		{
			MBAPI.IMBMission.SetMissionCorpseFadeOutTimeInSeconds(this.Pointer, corpseFadeOutTimeInSeconds);
		}

		public void SetReportStuckAgentsMode(bool value)
		{
			MBAPI.IMBMission.SetReportStuckAgentsMode(this.Pointer, value);
		}

		internal void BatchFormationUnitPositions(MBArrayList<Vec2i> orderedPositionIndices, MBArrayList<Vec2> orderedLocalPositions, MBList2D<int> availabilityTable, MBList2D<WorldPosition> globalPositionTable, WorldPosition orderPosition, Vec2 direction, int fileCount, int rankCount)
		{
			MBAPI.IMBMission.BatchFormationUnitPositions(this.Pointer, orderedPositionIndices.RawArray, orderedLocalPositions.RawArray, availabilityTable.RawArray, globalPositionTable.RawArray, orderPosition, direction, fileCount, rankCount);
		}

		internal void ProximityMapFindNext(ref AgentProximityMap.ProximityMapSearchStructInternal searchStruct)
		{
			MBAPI.IMBMission.ProximityMapFindNext(this.Pointer, ref searchStruct);
		}

		[UsedImplicitly]
		[MBCallback]
		public void ResetMission()
		{
			IMissionListener[] array = this._listeners.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnResetMission();
			}
			foreach (Agent agent in this._activeAgents)
			{
				agent.OnRemove();
			}
			foreach (Agent agent2 in this._allAgents)
			{
				agent2.OnDelete();
			}
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnClearScene();
			}
			this.NumOfFormationsSpawnedTeamOne = 0;
			this.NumOfFormationsSpawnedTeamTwo = 0;
			foreach (Team team in this.Teams)
			{
				team.Reset();
			}
			MBAPI.IMBMission.ClearScene(this.Pointer);
			this._activeAgents.Clear();
			this._allAgents.Clear();
			this._mountsWithoutRiders.Clear();
			this.MainAgent = null;
			this.ClearMissiles();
			this._missiles.Clear();
			this._agentCount = 0;
			for (int j = 0; j < 2; j++)
			{
				this._initialAgentCountPerSide[j] = 0;
				this._removedAgentCountPerSide[j] = 0;
			}
			this.ResetMissionObjects();
			this.RemoveSpawnedMissionObjects();
			this._activeMissionObjects.Clear();
			this._activeMissionObjects.AddRange(this.MissionObjects);
			this.Scene.ClearDecals();
			PropertyChangedEventHandler onMissionReset = this.OnMissionReset;
			if (onMissionReset == null)
			{
				return;
			}
			onMissionReset(this, null);
		}

		public event PropertyChangedEventHandler OnMissionReset;

		public void Initialize()
		{
			Mission.Current = this;
			this.CurrentState = Mission.State.Initializing;
			MissionInitializerRecord initializerRecord = this.InitializerRecord;
			MBAPI.IMBMission.InitializeMission(this.Pointer, ref initializerRecord);
		}

		[UsedImplicitly]
		[MBCallback]
		internal void OnSceneCreated(Scene scene)
		{
			this.Scene = scene;
		}

		[UsedImplicitly]
		[MBCallback]
		internal void TickAgentsAndTeams(float dt)
		{
			this.TickAgentsAndTeamsImp(dt);
		}

		public void TickAgentsAndTeamsAsync(float dt)
		{
			MBAPI.IMBMission.tickAgentsAndTeamsAsync(this.Pointer, dt);
		}

		internal void Tick(float dt)
		{
			MBAPI.IMBMission.Tick(this.Pointer, dt);
		}

		internal void IdleTick(float dt)
		{
			MBAPI.IMBMission.IdleTick(this.Pointer, dt);
		}

		public void MakeSound(int soundIndex, Vec3 position, bool soundCanBePredicted, bool isReliable, int relatedAgent1, int relatedAgent2)
		{
			MBAPI.IMBMission.MakeSound(this.Pointer, soundIndex, position, soundCanBePredicted, isReliable, relatedAgent1, relatedAgent2);
		}

		public void MakeSound(int soundIndex, Vec3 position, bool soundCanBePredicted, bool isReliable, int relatedAgent1, int relatedAgent2, ref SoundEventParameter parameter)
		{
			MBAPI.IMBMission.MakeSoundWithParameter(this.Pointer, soundIndex, position, soundCanBePredicted, isReliable, relatedAgent1, relatedAgent2, parameter);
		}

		public void MakeSoundOnlyOnRelatedPeer(int soundIndex, Vec3 position, int relatedAgent)
		{
			MBAPI.IMBMission.MakeSoundOnlyOnRelatedPeer(this.Pointer, soundIndex, position, relatedAgent);
		}

		public void AddSoundAlarmFactorToAgents(int ownerId, Vec3 position, float alarmFactor)
		{
			MBAPI.IMBMission.AddSoundAlarmFactorToAgents(this.Pointer, ownerId, position, alarmFactor);
		}

		public void AddDynamicallySpawnedMissionObjectInfo(Mission.DynamicallyCreatedEntity entityInfo)
		{
			this._addedEntitiesInfo.Add(entityInfo);
		}

		private void RemoveDynamicallySpawnedMissionObjectInfo(MissionObjectId id)
		{
			Mission.DynamicallyCreatedEntity dynamicallyCreatedEntity = this._addedEntitiesInfo.FirstOrDefault((Mission.DynamicallyCreatedEntity x) => x.ObjectId == id);
			if (dynamicallyCreatedEntity != null)
			{
				this._addedEntitiesInfo.Remove(dynamicallyCreatedEntity);
			}
		}

		private int AddMissileAux(int forcedMissileIndex, bool isPrediction, Agent shooterAgent, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, float damageBonus, ref Vec3 position, ref Vec3 direction, ref Mat3 orientation, float baseSpeed, float speed, bool addRigidBody, GameEntity gameEntityToIgnore, bool isPrimaryWeaponShot, out GameEntity missileEntity)
		{
			UIntPtr uintPtr;
			int num = MBAPI.IMBMission.AddMissile(this.Pointer, isPrediction, shooterAgent.Index, weaponData, weaponStatsData, weaponStatsData.Length, damageBonus, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, (gameEntityToIgnore != null) ? gameEntityToIgnore.Pointer : UIntPtr.Zero, forcedMissileIndex, isPrimaryWeaponShot, out uintPtr);
			missileEntity = (isPrediction ? null : new GameEntity(uintPtr));
			return num;
		}

		private int AddMissileSingleUsageAux(int forcedMissileIndex, bool isPrediction, Agent shooterAgent, in WeaponData weaponData, in WeaponStatsData weaponStatsData, float damageBonus, ref Vec3 position, ref Vec3 direction, ref Mat3 orientation, float baseSpeed, float speed, bool addRigidBody, GameEntity gameEntityToIgnore, bool isPrimaryWeaponShot, out GameEntity missileEntity)
		{
			UIntPtr uintPtr;
			int num = MBAPI.IMBMission.AddMissileSingleUsage(this.Pointer, isPrediction, shooterAgent.Index, weaponData, weaponStatsData, damageBonus, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, (gameEntityToIgnore != null) ? gameEntityToIgnore.Pointer : UIntPtr.Zero, forcedMissileIndex, isPrimaryWeaponShot, out uintPtr);
			missileEntity = (isPrediction ? null : new GameEntity(uintPtr));
			return num;
		}

		public Vec3 GetMissileCollisionPoint(Vec3 missileStartingPosition, Vec3 missileDirection, float missileSpeed, in WeaponData weaponData)
		{
			return MBAPI.IMBMission.GetMissileCollisionPoint(this.Pointer, missileStartingPosition, missileDirection, missileSpeed, weaponData);
		}

		public void RemoveMissileAsClient(int missileIndex)
		{
			MBAPI.IMBMission.RemoveMissile(this.Pointer, missileIndex);
		}

		public static float GetMissileVerticalAimCorrection(Vec3 vecToTarget, float missileStartingSpeed, ref WeaponStatsData weaponStatsData, float airFrictionConstant)
		{
			return MBAPI.IMBMission.GetMissileVerticalAimCorrection(vecToTarget, missileStartingSpeed, ref weaponStatsData, airFrictionConstant);
		}

		public static float GetMissileRange(float missileStartingSpeed, float heightDifference)
		{
			return MBAPI.IMBMission.GetMissileRange(missileStartingSpeed, heightDifference);
		}

		public void PrepareMissileWeaponForDrop(int missileIndex)
		{
			MBAPI.IMBMission.PrepareMissileWeaponForDrop(this.Pointer, missileIndex);
		}

		public void AddParticleSystemBurstByName(string particleSystem, MatrixFrame frame, bool synchThroughNetwork)
		{
			MBAPI.IMBMission.AddParticleSystemBurstByName(this.Pointer, particleSystem, ref frame, synchThroughNetwork);
		}

		public int EnemyAlarmStateIndicator
		{
			get
			{
				return MBAPI.IMBMission.GetEnemyAlarmStateIndicator(this.Pointer);
			}
		}

		public float PlayerAlarmIndicator
		{
			get
			{
				return MBAPI.IMBMission.GetPlayerAlarmIndicator(this.Pointer);
			}
		}

		public bool IsLoadingFinished
		{
			get
			{
				return MBAPI.IMBMission.GetIsLoadingFinished(this.Pointer);
			}
		}

		public Vec2 GetClosestBoundaryPosition(Vec2 position)
		{
			return MBAPI.IMBMission.GetClosestBoundaryPosition(this.Pointer, position);
		}

		private void ResetMissionObjects()
		{
			for (int i = this._dynamicEntities.Count - 1; i >= 0; i--)
			{
				Mission.DynamicEntityInfo dynamicEntityInfo = this._dynamicEntities[i];
				dynamicEntityInfo.Entity.RemoveEnginePhysics();
				dynamicEntityInfo.Entity.Remove(74);
				this._dynamicEntities.RemoveAt(i);
			}
			foreach (MissionObject missionObject in this.MissionObjects)
			{
				if (missionObject.CreatedAtRuntime)
				{
					break;
				}
				missionObject.OnMissionReset();
			}
		}

		private void RemoveSpawnedMissionObjects()
		{
			MissionObject[] array = this._missionObjects.ToArray();
			for (int i = array.Length - 1; i >= 0; i--)
			{
				MissionObject missionObject = array[i];
				if (!missionObject.CreatedAtRuntime)
				{
					break;
				}
				if (missionObject.GameEntity != null)
				{
					missionObject.GameEntity.RemoveAllChildren();
					missionObject.GameEntity.Remove(75);
				}
			}
			this._spawnedItemEntitiesCreatedAtRuntime.Clear();
			this._lastRuntimeMissionObjectIdCount = 0;
			this._emptyRuntimeMissionObjectIds.Clear();
			this._addedEntitiesInfo.Clear();
		}

		public int GetFreeRuntimeMissionObjectId()
		{
			float totalMissionTime = MBCommon.GetTotalMissionTime();
			int num = -1;
			if (this._emptyRuntimeMissionObjectIds.Count > 0)
			{
				if (totalMissionTime - this._emptyRuntimeMissionObjectIds.Peek().Item2 > 30f || this._lastRuntimeMissionObjectIdCount >= 4095)
				{
					num = this._emptyRuntimeMissionObjectIds.Pop().Item1;
				}
				else
				{
					num = this._lastRuntimeMissionObjectIdCount;
					this._lastRuntimeMissionObjectIdCount++;
				}
			}
			else if (this._lastRuntimeMissionObjectIdCount < 4095)
			{
				num = this._lastRuntimeMissionObjectIdCount;
				this._lastRuntimeMissionObjectIdCount++;
			}
			return num;
		}

		private void ReturnRuntimeMissionObjectId(int id)
		{
			this._emptyRuntimeMissionObjectIds.Push(new ValueTuple<int, float>(id, MBCommon.GetTotalMissionTime()));
		}

		public int GetFreeSceneMissionObjectId()
		{
			int lastSceneMissionObjectIdCount = this._lastSceneMissionObjectIdCount;
			this._lastSceneMissionObjectIdCount++;
			return lastSceneMissionObjectIdCount;
		}

		public void SetCameraFrame(ref MatrixFrame cameraFrame, float zoomFactor)
		{
			this.SetCameraFrame(ref cameraFrame, zoomFactor, ref cameraFrame.origin);
		}

		public void SetCameraFrame(ref MatrixFrame cameraFrame, float zoomFactor, ref Vec3 attenuationPosition)
		{
			cameraFrame.Fill();
			MBAPI.IMBMission.SetCameraFrame(this.Pointer, ref cameraFrame, zoomFactor, ref attenuationPosition);
		}

		public MatrixFrame GetCameraFrame()
		{
			return MBAPI.IMBMission.GetCameraFrame(this.Pointer);
		}

		public bool CameraIsFirstPerson
		{
			get
			{
				return Mission._isCameraFirstPerson;
			}
			set
			{
				if (Mission._isCameraFirstPerson != value)
				{
					Mission._isCameraFirstPerson = value;
					MBAPI.IMBMission.SetCameraIsFirstPerson(value);
					this.ResetFirstThirdPersonView();
				}
			}
		}

		public static float CameraAddedDistance
		{
			get
			{
				return BannerlordConfig.CombatCameraDistance;
			}
			set
			{
				if (value != BannerlordConfig.CombatCameraDistance)
				{
					BannerlordConfig.CombatCameraDistance = value;
				}
			}
		}

		public float ClearSceneTimerElapsedTime
		{
			get
			{
				return MBAPI.IMBMission.GetClearSceneTimerElapsedTime(this.Pointer);
			}
		}

		public void ResetFirstThirdPersonView()
		{
			MBAPI.IMBMission.ResetFirstThirdPersonView(this.Pointer);
		}

		internal void UpdateSceneTimeSpeed()
		{
			if (this.Scene != null)
			{
				float num = 1f;
				int num2 = -1;
				for (int i = 0; i < this._timeSpeedRequests.Count; i++)
				{
					if (this._timeSpeedRequests[i].RequestedTimeSpeed < num)
					{
						num = this._timeSpeedRequests[i].RequestedTimeSpeed;
						num2 = this._timeSpeedRequests[i].RequestID;
					}
				}
				if (!this.Scene.TimeSpeed.ApproximatelyEqualsTo(num, 1E-05f))
				{
					if (num2 != -1)
					{
						Debug.Print(string.Format("Updated mission time speed with request ID:{0}, time speed{1}", num2, num), 0, Debug.DebugColor.White, 17592186044416UL);
					}
					else
					{
						Debug.Print(string.Format("Reverted time speed back to default({0})", num), 0, Debug.DebugColor.White, 17592186044416UL);
					}
					this.Scene.TimeSpeed = num;
				}
			}
		}

		public void AddTimeSpeedRequest(Mission.TimeSpeedRequest request)
		{
			this._timeSpeedRequests.Add(request);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		private void AssertTimeSpeedRequestDoesntExist(Mission.TimeSpeedRequest request)
		{
			for (int i = 0; i < this._timeSpeedRequests.Count; i++)
			{
				int requestID = this._timeSpeedRequests[i].RequestID;
				int requestID2 = request.RequestID;
			}
		}

		public void RemoveTimeSpeedRequest(int timeSpeedRequestID)
		{
			int num = -1;
			for (int i = 0; i < this._timeSpeedRequests.Count; i++)
			{
				if (this._timeSpeedRequests[i].RequestID == timeSpeedRequestID)
				{
					num = i;
				}
			}
			this._timeSpeedRequests.RemoveAt(num);
		}

		public bool GetRequestedTimeSpeed(int timeSpeedRequestID, out float requestedTime)
		{
			for (int i = 0; i < this._timeSpeedRequests.Count; i++)
			{
				if (this._timeSpeedRequests[i].RequestID == timeSpeedRequestID)
				{
					requestedTime = this._timeSpeedRequests[i].RequestedTimeSpeed;
					return true;
				}
			}
			requestedTime = 0f;
			return false;
		}

		public void ClearAgentActions()
		{
			MBAPI.IMBMission.ClearAgentActions(this.Pointer);
		}

		public void ClearMissiles()
		{
			MBAPI.IMBMission.ClearMissiles(this.Pointer);
		}

		public void ClearCorpses(bool isMissionReset)
		{
			MBAPI.IMBMission.ClearCorpses(this.Pointer, isMissionReset);
		}

		private Agent FindAgentWithIndexAux(int index)
		{
			if (index >= 0)
			{
				return MBAPI.IMBMission.FindAgentWithIndex(this.Pointer, index);
			}
			return null;
		}

		private Agent GetClosestEnemyAgent(MBTeam team, Vec3 position, float radius)
		{
			return MBAPI.IMBMission.GetClosestEnemy(this.Pointer, team.Index, position, radius);
		}

		private Agent GetClosestAllyAgent(MBTeam team, Vec3 position, float radius)
		{
			return MBAPI.IMBMission.GetClosestAlly(this.Pointer, team.Index, position, radius);
		}

		private int GetNearbyEnemyAgentCount(MBTeam team, Vec2 position, float radius)
		{
			int num = 0;
			int num2 = 0;
			MBAPI.IMBMission.GetAgentCountAroundPosition(this.Pointer, team.Index, position, radius, ref num, ref num2);
			return num2;
		}

		public bool IsAgentInProximityMap(Agent agent)
		{
			return MBAPI.IMBMission.IsAgentInProximityMap(this.Pointer, agent.Index);
		}

		public void OnMissionStateActivate()
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnMissionStateActivated();
			}
		}

		public void OnMissionStateDeactivate()
		{
			if (this.MissionBehaviors != null)
			{
				foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
				{
					missionBehavior.OnMissionStateDeactivated();
				}
			}
		}

		public void OnMissionStateFinalize(bool forceClearGPUResources)
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnMissionStateFinalized();
			}
			if (GameNetwork.IsSessionActive && this.GetMissionBehavior<MissionNetworkComponent>() != null)
			{
				this.RemoveMissionBehavior(this.GetMissionBehavior<MissionNetworkComponent>());
			}
			for (int i = this.MissionBehaviors.Count - 1; i >= 0; i--)
			{
				this.RemoveMissionBehavior(this.MissionBehaviors[i]);
			}
			this.MissionLogics.Clear();
			this.Scene = null;
			Mission.Current = null;
			this.ClearUnreferencedResources(forceClearGPUResources);
		}

		public void ClearUnreferencedResources(bool forceClearGPUResources)
		{
			Common.MemoryCleanupGC(false);
			if (forceClearGPUResources)
			{
				MBAPI.IMBMission.ClearResources(this.Pointer);
			}
		}

		internal void OnEntityHit(GameEntity entity, Agent attackerAgent, int inflictedDamage, DamageTypes damageType, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon)
		{
			bool flag = false;
			while (entity != null)
			{
				bool flag2 = false;
				using (IEnumerator<MissionObject> enumerator = entity.GetScriptComponents<MissionObject>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						bool flag3;
						if (enumerator.Current.OnHit(attackerAgent, inflictedDamage, impactPosition, impactDirection, weapon, null, out flag3))
						{
							flag2 = true;
						}
						flag = flag || flag3;
					}
				}
				if (flag2)
				{
					break;
				}
				entity = entity.Parent;
			}
			if (flag && attackerAgent != null && !attackerAgent.IsMount && !attackerAgent.IsAIControlled)
			{
				bool flag4 = false;
				bool isHuman = attackerAgent.IsHuman;
				bool isMine = attackerAgent.IsMine;
				bool flag5 = attackerAgent.RiderAgent != null;
				Agent riderAgent = attackerAgent.RiderAgent;
				this.AddCombatLogSafe(attackerAgent, null, entity, new CombatLogData(flag4, isHuman, isMine, flag5, riderAgent != null && riderAgent.IsMine, attackerAgent.IsMount, false, false, false, false, false, false, true, false, false, false, 0f)
				{
					DamageType = damageType,
					InflictedDamage = inflictedDamage
				});
			}
		}

		public float GetMainAgentMaxCameraZoom()
		{
			if (this.MainAgent != null)
			{
				return MissionGameModels.Current.AgentStatCalculateModel.GetMaxCameraZoom(this.MainAgent);
			}
			return 1f;
		}

		public WorldPosition GetBestSlopeTowardsDirection(ref WorldPosition centerPosition, float halfSize, ref WorldPosition referencePosition)
		{
			return MBAPI.IMBMission.GetBestSlopeTowardsDirection(this.Pointer, ref centerPosition, halfSize, ref referencePosition);
		}

		public WorldPosition GetBestSlopeAngleHeightPosForDefending(WorldPosition enemyPosition, WorldPosition defendingPosition, int sampleSize, float distanceRatioAllowedFromDefendedPos, float distanceSqrdAllowedFromBoundary, float cosinusOfBestSlope, float cosinusOfMaxAcceptedSlope, float minSlopeScore, float maxSlopeScore, float excessiveSlopePenalty, float nearConeCenterRatio, float nearConeCenterBonus, float heightDifferenceCeiling, float maxDisplacementPenalty)
		{
			return MBAPI.IMBMission.GetBestSlopeAngleHeightPosForDefending(this.Pointer, enemyPosition, defendingPosition, sampleSize, distanceRatioAllowedFromDefendedPos, distanceSqrdAllowedFromBoundary, cosinusOfBestSlope, cosinusOfMaxAcceptedSlope, minSlopeScore, maxSlopeScore, excessiveSlopePenalty, nearConeCenterRatio, nearConeCenterBonus, heightDifferenceCeiling, maxDisplacementPenalty);
		}

		public Vec2 GetAveragePositionOfAgents(List<Agent> agents)
		{
			int num = 0;
			Vec2 vec = Vec2.Zero;
			foreach (Agent agent in agents)
			{
				num++;
				vec += agent.Position.AsVec2;
			}
			if (num == 0)
			{
				return Vec2.Invalid;
			}
			return vec * (1f / (float)num);
		}

		private void GetNearbyAgentsAux(Vec2 center, float radius, MBTeam team, Mission.GetNearbyAgentsAuxType type, MBList<Agent> resultList)
		{
			EngineStackArray.StackArray40Int stackArray40Int = default(EngineStackArray.StackArray40Int);
			object getNearbyAgentsAuxLock = Mission.GetNearbyAgentsAuxLock;
			lock (getNearbyAgentsAuxLock)
			{
				int num = 0;
				for (;;)
				{
					int num2 = -1;
					MBAPI.IMBMission.GetNearbyAgentsAux(this.Pointer, center, radius, team.Index, (int)type, num, ref stackArray40Int, ref num2);
					for (int i = 0; i < num2; i++)
					{
						Agent agent = DotNetObject.GetManagedObjectWithId(stackArray40Int[i]) as Agent;
						resultList.Add(agent);
					}
					if (num2 < 40)
					{
						break;
					}
					num += 40;
				}
			}
		}

		private int GetNearbyAgentsCountAux(Vec2 center, float radius, MBTeam team, Mission.GetNearbyAgentsAuxType type)
		{
			int num = 0;
			EngineStackArray.StackArray40Int stackArray40Int = default(EngineStackArray.StackArray40Int);
			object getNearbyAgentsAuxLock = Mission.GetNearbyAgentsAuxLock;
			lock (getNearbyAgentsAuxLock)
			{
				int num2 = 0;
				for (;;)
				{
					int num3 = -1;
					MBAPI.IMBMission.GetNearbyAgentsAux(this.Pointer, center, radius, team.Index, (int)type, num2, ref stackArray40Int, ref num3);
					num += num3;
					if (num3 < 40)
					{
						break;
					}
					num2 += 40;
				}
			}
			return num;
		}

		public void SetRandomDecideTimeOfAgentsWithIndices(int[] agentIndices, float? minAIReactionTime = null, float? maxAIReactionTime = null)
		{
			if (minAIReactionTime == null || maxAIReactionTime == null)
			{
				maxAIReactionTime = new float?((float)(-1));
				minAIReactionTime = maxAIReactionTime;
			}
			MBAPI.IMBMission.SetRandomDecideTimeOfAgents(this.Pointer, agentIndices.Length, agentIndices, minAIReactionTime.Value, maxAIReactionTime.Value);
		}

		public void SetBowMissileSpeedModifier(float modifier)
		{
			MBAPI.IMBMission.SetBowMissileSpeedModifier(this.Pointer, modifier);
		}

		public void SetCrossbowMissileSpeedModifier(float modifier)
		{
			MBAPI.IMBMission.SetCrossbowMissileSpeedModifier(this.Pointer, modifier);
		}

		public void SetThrowingMissileSpeedModifier(float modifier)
		{
			MBAPI.IMBMission.SetThrowingMissileSpeedModifier(this.Pointer, modifier);
		}

		public void SetMissileRangeModifier(float modifier)
		{
			MBAPI.IMBMission.SetMissileRangeModifier(this.Pointer, modifier);
		}

		public void SetLastMovementKeyPressed(Agent.MovementControlFlag lastMovementKeyPressed)
		{
			MBAPI.IMBMission.SetLastMovementKeyPressed(this.Pointer, lastMovementKeyPressed);
		}

		public Vec2 GetWeightedPointOfEnemies(Agent agent, Vec2 basePoint)
		{
			return MBAPI.IMBMission.GetWeightedPointOfEnemies(this.Pointer, agent.Index, basePoint);
		}

		public bool GetPathBetweenPositions(ref NavigationData navData)
		{
			return MBAPI.IMBMission.GetNavigationPoints(this.Pointer, ref navData);
		}

		public void SetNavigationFaceCostWithIdAroundPosition(int navigationFaceId, Vec3 position, float cost)
		{
			MBAPI.IMBMission.SetNavigationFaceCostWithIdAroundPosition(this.Pointer, navigationFaceId, position, cost);
		}

		public WorldPosition GetStraightPathToTarget(Vec2 targetPosition, WorldPosition startingPosition, float samplingDistance = 1f, bool stopAtObstacle = true)
		{
			return MBAPI.IMBMission.GetStraightPathToTarget(this.Pointer, targetPosition, startingPosition, samplingDistance, stopAtObstacle);
		}

		public void FastForwardMission(float startTime, float endTime)
		{
			MBAPI.IMBMission.FastForwardMission(this.Pointer, startTime, endTime);
		}

		public int GetDebugAgent()
		{
			return MBAPI.IMBMission.GetDebugAgent(this.Pointer);
		}

		public void AddAiDebugText(string str)
		{
			MBAPI.IMBMission.AddAiDebugText(this.Pointer, str);
		}

		public void SetDebugAgent(int index)
		{
			MBAPI.IMBMission.SetDebugAgent(this.Pointer, index);
		}

		public static float GetFirstPersonFov()
		{
			return BannerlordConfig.FirstPersonFov;
		}

		public float GetWaterLevelAtPosition(Vec2 position)
		{
			return MBAPI.IMBMission.GetWaterLevelAtPosition(this.Pointer, position);
		}

		public float GetWaterLevelAtPositionMT(Vec2 position)
		{
			return MBAPI.IMBMission.GetWaterLevelAtPosition(this.Pointer, position);
		}

		public event Func<WorldPosition, Team, bool> IsFormationUnitPositionAvailable_AdditionalCondition;

		public event Func<Agent, bool> CanAgentRout_AdditionalCondition;

		public event Func<BattleSideEnum, BasicCharacterObject, FormationClass> GetAgentTroopClass_Override;

		public event Func<bool> IsAgentInteractionAllowed_AdditionalCondition;

		public event Func<Agent, WorldPosition?> GetOverriddenFleePositionForAgent;

		public event Action<Agent, SpawnedItemEntity> OnItemPickUp;

		public event Action<Agent, SpawnedItemEntity> OnItemDrop;

		public event PropertyChangedEventHandler OnMainAgentChanged;

		public event Func<bool> AreOrderGesturesEnabled_AdditionalCondition;

		public bool MissionIsEnding { get; private set; }

		public bool MissionEnded
		{
			get
			{
				return this._missionEnded;
			}
			private set
			{
				if (!this._missionEnded && value)
				{
					this.MissionIsEnding = true;
					foreach (MissionObject missionObject in this.MissionObjects)
					{
						missionObject.OnMissionEnded();
					}
					this.MissionIsEnding = false;
				}
				this._missionEnded = value;
			}
		}

		public MBReadOnlyList<KeyValuePair<Agent, MissionTime>> MountsWithoutRiders
		{
			get
			{
				return this._mountsWithoutRiders;
			}
		}

		public event Func<bool> IsBattleInRetreatEvent;

		public event Mission.OnBeforeAgentRemovedDelegate OnBeforeAgentRemoved;

		public BattleSideEnum RetreatSide { get; private set; } = BattleSideEnum.None;

		public bool IsFastForward { get; private set; }

		public bool FixedDeltaTimeMode { get; set; }

		public float FixedDeltaTime { get; set; }

		public Mission.State CurrentState { get; private set; }

		public Mission.TeamCollection Teams { get; private set; }

		public Team AttackerTeam
		{
			get
			{
				return this.Teams.Attacker;
			}
		}

		public Team DefenderTeam
		{
			get
			{
				return this.Teams.Defender;
			}
		}

		public Team AttackerAllyTeam
		{
			get
			{
				return this.Teams.AttackerAlly;
			}
		}

		public Team DefenderAllyTeam
		{
			get
			{
				return this.Teams.DefenderAlly;
			}
		}

		public Team PlayerTeam
		{
			get
			{
				return this.Teams.Player;
			}
			set
			{
				this.Teams.Player = value;
			}
		}

		public Team PlayerEnemyTeam
		{
			get
			{
				return this.Teams.PlayerEnemy;
			}
		}

		public Team PlayerAllyTeam
		{
			get
			{
				return this.Teams.PlayerAlly;
			}
		}

		public Team SpectatorTeam { get; set; }

		IMissionTeam IMission.PlayerTeam
		{
			get
			{
				return this.PlayerTeam;
			}
		}

		public bool IsMissionEnding
		{
			get
			{
				return this.CurrentState != Mission.State.Over && this.MissionEnded;
			}
		}

		public List<MissionLogic> MissionLogics { get; }

		public List<MissionBehavior> MissionBehaviors { get; }

		public IEnumerable<Mission.Missile> Missiles
		{
			get
			{
				return this._missiles.Values;
			}
		}

		public IInputContext InputManager { get; set; }

		public bool NeedsMemoryCleanup { get; internal set; }

		public Agent MainAgent
		{
			get
			{
				return this._mainAgent;
			}
			set
			{
				this._mainAgent = value;
				if (this.OnMainAgentChanged != null)
				{
					this.OnMainAgentChanged(this, null);
				}
				if (!GameNetwork.IsClient)
				{
					this.MainAgentServer = this._mainAgent;
				}
			}
		}

		public IMissionDeploymentPlan DeploymentPlan
		{
			get
			{
				return this._deploymentPlan;
			}
		}

		public float GetRemovedAgentRatioForSide(BattleSideEnum side)
		{
			float num = 0f;
			if (side == BattleSideEnum.NumSides)
			{
				Debug.FailedAssert("Cannot get removed agent count for side. Invalid battle side passed!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "GetRemovedAgentRatioForSide", 634);
			}
			float num2 = (float)this._initialAgentCountPerSide[(int)side];
			if (num2 > 0f && this._agentCount > 0)
			{
				num = MathF.Min((float)this._removedAgentCountPerSide[(int)side] / num2, 1f);
			}
			return num;
		}

		public Agent MainAgentServer { get; set; }

		public bool HasSpawnPath
		{
			get
			{
				return this._battleSpawnPathSelector.IsInitialized;
			}
		}

		public bool IsFieldBattle
		{
			get
			{
				return this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.FieldBattle;
			}
		}

		public bool IsSiegeBattle
		{
			get
			{
				return this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.Siege;
			}
		}

		public bool IsSallyOutBattle
		{
			get
			{
				return this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.SallyOut;
			}
		}

		public MBReadOnlyList<Agent> AllAgents
		{
			get
			{
				return this._allAgents;
			}
		}

		public MBReadOnlyList<Agent> Agents
		{
			get
			{
				return this._activeAgents;
			}
		}

		public bool IsInventoryAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsInventoryAccessibleAtMission || this._isScreenAccessAllowed) && this.IsInventoryAccessible;
			}
		}

		public bool IsInventoryAccessible { private get; set; }

		public MissionResult MissionResult { get; private set; }

		public bool IsQuestScreenAccessible { private get; set; }

		private bool _isScreenAccessAllowed
		{
			get
			{
				return this.Mode != MissionMode.Stealth && this.Mode != MissionMode.Battle && this.Mode != MissionMode.Deployment && this.Mode != MissionMode.Duel && this.Mode != MissionMode.CutScene;
			}
		}

		public bool IsQuestScreenAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsQuestScreenAccessibleAtMission || this._isScreenAccessAllowed) && this.IsQuestScreenAccessible;
			}
		}

		public bool IsCharacterWindowAccessible { private get; set; }

		public bool IsCharacterWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsCharacterWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsCharacterWindowAccessible;
			}
		}

		public bool IsPartyWindowAccessible { private get; set; }

		public bool IsPartyWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsPartyWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsPartyWindowAccessible;
			}
		}

		public bool IsKingdomWindowAccessible { private get; set; }

		public bool IsKingdomWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsKingdomWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsKingdomWindowAccessible;
			}
		}

		public bool IsClanWindowAccessible { private get; set; }

		public bool IsClanWindowAccessAllowed
		{
			get
			{
				return Game.Current.GameType.IsClanWindowAccessibleAtMission && this._isScreenAccessAllowed && this.IsClanWindowAccessible;
			}
		}

		public bool IsEncyclopediaWindowAccessible { private get; set; }

		public bool IsEncyclopediaWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsEncyclopediaWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsEncyclopediaWindowAccessible;
			}
		}

		public bool IsBannerWindowAccessible { private get; set; }

		public bool IsBannerWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsBannerWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsBannerWindowAccessible;
			}
		}

		public bool DoesMissionRequireCivilianEquipment
		{
			get
			{
				return this._doesMissionRequireCivilianEquipment;
			}
			set
			{
				this._doesMissionRequireCivilianEquipment = value;
			}
		}

		public Mission.MissionTeamAITypeEnum MissionTeamAIType { get; set; }

		public void MakeDeploymentPlanForSide(BattleSideEnum battleSide, DeploymentPlanType planType, float spawnPathOffset = 0f)
		{
			if (!this._deploymentPlan.IsPlanMadeForBattleSide(battleSide, planType))
			{
				this._deploymentPlan.PlanBattleDeployment(battleSide, planType, spawnPathOffset);
				bool flag;
				if (this._deploymentPlan.IsPlanMadeForBattleSide(battleSide, out flag, planType) && planType == DeploymentPlanType.Initial)
				{
					foreach (IMissionListener missionListener in this._listeners)
					{
						missionListener.OnInitialDeploymentPlanMade(battleSide, flag);
					}
				}
			}
		}

		public void MakeDefaultDeploymentPlans()
		{
			for (int i = 0; i < 2; i++)
			{
				BattleSideEnum battleSideEnum = (BattleSideEnum)i;
				this.MakeDeploymentPlanForSide(battleSideEnum, DeploymentPlanType.Initial, 0f);
				this.MakeDeploymentPlanForSide(battleSideEnum, DeploymentPlanType.Reinforcement, 0f);
			}
		}

		public ref readonly List<SiegeWeapon> GetAttackerWeaponsForFriendlyFirePreventing()
		{
			return ref this._attackerWeaponsForFriendlyFirePreventing;
		}

		public void ClearDeploymentPlanForSide(BattleSideEnum battleSide, DeploymentPlanType planType)
		{
			this._deploymentPlan.ClearDeploymentPlanForSide(battleSide, planType);
		}

		public void ClearAddedTroopsInDeploymentPlan(BattleSideEnum battleSide, DeploymentPlanType planType)
		{
			this._deploymentPlan.ClearAddedTroopsForBattleSide(battleSide, planType);
		}

		public void SetDeploymentPlanSpawnWithHorses(BattleSideEnum side, bool spawnWithHorses)
		{
			this._deploymentPlan.SetSpawnWithHorsesForSide(side, spawnWithHorses);
		}

		public void UpdateReinforcementPlan(BattleSideEnum side)
		{
			this._deploymentPlan.UpdateReinforcementPlan(side);
		}

		public void AddTroopsToDeploymentPlan(BattleSideEnum side, DeploymentPlanType planType, FormationClass fClass, int footTroopCount, int mountedTroopCount)
		{
			this._deploymentPlan.AddTroopsForBattleSide(side, planType, fClass, footTroopCount, mountedTroopCount);
		}

		public bool TryRemakeInitialDeploymentPlanForBattleSide(BattleSideEnum battleSide)
		{
			if (this._deploymentPlan.IsPlanMadeForBattleSide(battleSide, DeploymentPlanType.Initial))
			{
				float spawnPathOffsetForSide = this._deploymentPlan.GetSpawnPathOffsetForSide(battleSide, DeploymentPlanType.Initial);
				ValueTuple<int, int>[] array = new ValueTuple<int, int>[11];
				foreach (Agent agent2 in this._allAgents.Where((Agent agent) => agent.IsHuman && agent.Team != null && agent.Team.Side == battleSide && agent.Formation != null))
				{
					int formationIndex = (int)agent2.Formation.FormationIndex;
					ValueTuple<int, int> valueTuple = array[formationIndex];
					array[formationIndex] = (agent2.HasMount ? new ValueTuple<int, int>(valueTuple.Item1, valueTuple.Item2 + 1) : new ValueTuple<int, int>(valueTuple.Item1 + 1, valueTuple.Item2));
				}
				if (!this._deploymentPlan.IsInitialPlanSuitableForFormations(battleSide, array))
				{
					this._deploymentPlan.ClearAddedTroopsForBattleSide(battleSide, DeploymentPlanType.Initial);
					this._deploymentPlan.ClearDeploymentPlanForSide(battleSide, DeploymentPlanType.Initial);
					for (int i = 0; i < 11; i++)
					{
						ValueTuple<int, int> valueTuple2 = array[i];
						int item = valueTuple2.Item1;
						int item2 = valueTuple2.Item2;
						if (item + item2 > 0)
						{
							this._deploymentPlan.AddTroopsForBattleSide(battleSide, DeploymentPlanType.Initial, (FormationClass)i, item, item2);
						}
					}
					this.MakeDeploymentPlanForSide(battleSide, DeploymentPlanType.Initial, spawnPathOffsetForSide);
					return this._deploymentPlan.IsPlanMadeForBattleSide(battleSide, DeploymentPlanType.Initial);
				}
			}
			return false;
		}

		public void AutoDeployTeamUsingTeamAI(Team team, bool enforceNotSplittableByAI = false)
		{
			if (team.Side == BattleSideEnum.Attacker)
			{
				this.AutoDeployTeamUsingDeploymentPlan(team);
			}
			List<Formation> list = team.FormationsIncludingEmpty.ToList<Formation>();
			if (list.Count > 0)
			{
				bool[] array = new bool[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					Formation formation = list[i];
					array[i] = formation.EnforceNotSplittableByAI;
					formation.SetControlledByAI(true, formation.EnforceNotSplittableByAI || enforceNotSplittableByAI);
				}
				bool allowAiTicking = this.AllowAiTicking;
				this.AllowAiTicking = true;
				bool forceTickOccasionally = this.ForceTickOccasionally;
				this.ForceTickOccasionally = true;
				bool isTeleportingAgents = this.IsTeleportingAgents;
				if (team.Side == BattleSideEnum.Defender)
				{
					this.IsTeleportingAgents = true;
				}
				else
				{
					this.IsTeleportingAgents = false;
				}
				if (!team.DetachmentManager.Detachments.IsEmpty<ValueTuple<IDetachment, DetachmentData>>())
				{
					team.PlayerOrderController.SelectAllFormations(false);
					team.PlayerOrderController.SetOrder(OrderType.AIControlOff);
					int num = 0;
					int num2 = 0;
					foreach (ValueTuple<IDetachment, DetachmentData> valueTuple in team.DetachmentManager.Detachments)
					{
						num += valueTuple.Item1.GetNumberOfUsableSlots();
					}
					foreach (Formation formation2 in team.FormationsIncludingEmpty)
					{
						num2 += formation2.CountOfDetachableNonplayerUnits;
					}
					for (int j = 0; j < MathF.Min(num, num2); j++)
					{
						team.DetachmentManager.TickDetachments();
					}
					team.PlayerOrderController.SetOrder(OrderType.AIControlOn);
				}
				team.ResetTactic();
				team.Tick(0f);
				for (int k = 0; k < list.Count; k++)
				{
					list[k].ApplyActionOnEachUnit(delegate(Agent agent)
					{
						agent.UpdateCachedAndFormationValues(true, false);
					}, null);
				}
				this.IsTeleportingAgents = isTeleportingAgents;
				this.ForceTickOccasionally = forceTickOccasionally;
				this.AllowAiTicking = allowAiTicking;
				for (int l = 0; l < list.Count; l++)
				{
					Formation formation3 = list[l];
					bool flag = array[l];
					formation3.SetControlledByAI(true, flag);
				}
			}
		}

		public void AutoDeployTeamUsingDeploymentPlan(Team team)
		{
			if (this._deploymentPlan.IsPlanMadeForBattleSide(team.Side, DeploymentPlanType.Initial))
			{
				List<Formation> list = team.FormationsIncludingEmpty.ToList<Formation>();
				if (list.Count > 0)
				{
					bool[] array = new bool[list.Count];
					for (int i = 0; i < list.Count; i++)
					{
						Formation formation = list[i];
						array[i] = formation.IsAIControlled;
						formation.SetControlledByAI(true, false);
					}
					bool isTeleportingAgents = this.IsTeleportingAgents;
					this.IsTeleportingAgents = true;
					for (int j = 0; j < list.Count; j++)
					{
						Formation formation2 = list[j];
						IFormationDeploymentPlan formationPlan = this._deploymentPlan.GetFormationPlan(team.Side, (FormationClass)j, DeploymentPlanType.Initial);
						if (formationPlan.HasDimensions)
						{
							formation2.FormOrder = FormOrder.FormOrderCustom(formationPlan.PlannedWidth);
						}
						WorldPosition worldPosition;
						Vec2 vec;
						this.GetFormationSpawnFrame(formation2.Team.Side, formation2.FormationIndex, false, out worldPosition, out vec);
						formation2.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition));
						formation2.FacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
						formation2.SetPositioning(new WorldPosition?(worldPosition), new Vec2?(vec), null);
						formation2.ApplyActionOnEachUnit(delegate(Agent agent)
						{
							agent.UpdateCachedAndFormationValues(true, false);
						}, null);
					}
					this.IsTeleportingAgents = isTeleportingAgents;
					for (int k = 0; k < list.Count; k++)
					{
						list[k].SetControlledByAI(array[k], false);
					}
				}
			}
		}

		public WorldPosition GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(Vec2 directionTowards, WorldPosition originalPosition, ref float positionPenalty)
		{
			return MBAPI.IMBMission.GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(this.Pointer, ref directionTowards, ref originalPosition, ref positionPenalty);
		}

		public int GetNextDynamicNavMeshIdStart()
		{
			int nextDynamicNavMeshIdStart = this._nextDynamicNavMeshIdStart;
			this._nextDynamicNavMeshIdStart += 10;
			return nextDynamicNavMeshIdStart;
		}

		public FormationClass GetAgentTroopClass(BattleSideEnum battleSide, BasicCharacterObject agentCharacter)
		{
			if (this.GetAgentTroopClass_Override != null)
			{
				return this.GetAgentTroopClass_Override(battleSide, agentCharacter);
			}
			FormationClass formationClass = agentCharacter.GetFormationClass();
			if (this.IsSiegeBattle || (this.IsSallyOutBattle && battleSide == BattleSideEnum.Attacker))
			{
				formationClass = formationClass.DismountedClass();
			}
			return formationClass;
		}

		[UsedImplicitly]
		[MBCallback]
		public WorldPosition GetClosestFleePositionForAgent(Agent agent)
		{
			if (this.GetOverriddenFleePositionForAgent != null)
			{
				WorldPosition? worldPosition = this.GetOverriddenFleePositionForAgent(agent);
				if (worldPosition != null)
				{
					return worldPosition.Value;
				}
			}
			WorldPosition worldPosition2 = agent.GetWorldPosition();
			float maximumForwardUnlimitedSpeed = agent.MaximumForwardUnlimitedSpeed;
			Team team = agent.Team;
			BattleSideEnum battleSideEnum = BattleSideEnum.None;
			bool flag = agent.MountAgent != null;
			if (team != null)
			{
				team.UpdateCachedEnemyDataForFleeing();
				battleSideEnum = team.Side;
			}
			MBReadOnlyList<FleePosition> mbreadOnlyList = ((this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.SallyOut && agent.IsMount) ? this.GetFleePositionsForSide(BattleSideEnum.Attacker) : this.GetFleePositionsForSide(battleSideEnum));
			return this.GetClosestFleePosition(mbreadOnlyList, worldPosition2, maximumForwardUnlimitedSpeed, flag, (team != null) ? team.CachedEnemyDataForFleeing : null);
		}

		public WorldPosition GetClosestFleePositionForFormation(Formation formation)
		{
			WorldPosition medianPosition = formation.QuerySystem.MedianPosition;
			float movementSpeedMaximum = formation.QuerySystem.MovementSpeedMaximum;
			bool flag = formation.QuerySystem.IsCavalryFormation || formation.QuerySystem.IsRangedCavalryFormation;
			Team team = formation.Team;
			team.UpdateCachedEnemyDataForFleeing();
			MBReadOnlyList<FleePosition> fleePositionsForSide = this.GetFleePositionsForSide(team.Side);
			return this.GetClosestFleePosition(fleePositionsForSide, medianPosition, movementSpeedMaximum, flag, team.CachedEnemyDataForFleeing);
		}

		private WorldPosition GetClosestFleePosition(MBReadOnlyList<FleePosition> availableFleePositions, WorldPosition runnerPosition, float runnerSpeed, bool runnerHasMount, MBReadOnlyList<ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>> chaserData)
		{
			int num = ((chaserData != null) ? chaserData.Count : 0);
			if (availableFleePositions.Count > 0)
			{
				float[] array = new float[availableFleePositions.Count];
				WorldPosition[] array2 = new WorldPosition[availableFleePositions.Count];
				for (int i = 0; i < availableFleePositions.Count; i++)
				{
					array[i] = 1f;
					array2[i] = new WorldPosition(this.Scene, UIntPtr.Zero, availableFleePositions[i].GetClosestPointToEscape(runnerPosition.AsVec2), false);
					array2[i].SetVec2(array2[i].AsVec2 - runnerPosition.AsVec2);
				}
				for (int j = 0; j < num; j++)
				{
					float item = chaserData[j].Item1;
					if (item > 0f)
					{
						ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool> valueTuple = chaserData[j];
						Vec2 asVec = valueTuple.Item2.AsVec2;
						int item2 = chaserData[j].Item3;
						Vec2 vec;
						if (item2 > 1)
						{
							Vec2 item3 = chaserData[j].Item4;
							Vec2 item4 = chaserData[j].Item5;
							vec = MBMath.GetClosestPointInLineSegmentToPoint(runnerPosition.AsVec2, item3, item4) - runnerPosition.AsVec2;
						}
						else
						{
							vec = asVec - runnerPosition.AsVec2;
						}
						for (int k = 0; k < availableFleePositions.Count; k++)
						{
							float num2 = vec.DotProduct(array2[k].AsVec2.Normalized());
							if (num2 > 0f)
							{
								float num3 = MathF.Max(MathF.Abs(vec.DotProduct(array2[k].AsVec2.LeftVec().Normalized())) / item, 1f);
								float num4 = MathF.Max(num2 / runnerSpeed, 1f);
								if (num4 > num3)
								{
									float num5 = num4 / num3;
									num5 /= num2;
									array[k] += num5 * (float)item2;
								}
							}
						}
					}
				}
				for (int l = 0; l < availableFleePositions.Count; l++)
				{
					WorldPosition worldPosition = new WorldPosition(this.Scene, UIntPtr.Zero, availableFleePositions[l].GetClosestPointToEscape(runnerPosition.AsVec2), false);
					float num6;
					if (this.Scene.GetPathDistanceBetweenPositions(ref runnerPosition, ref worldPosition, 0f, out num6))
					{
						array[l] *= num6;
					}
					else
					{
						array[l] = float.MaxValue;
					}
				}
				int num7 = -1;
				float num8 = float.MaxValue;
				for (int m = 0; m < availableFleePositions.Count; m++)
				{
					if (num8 > array[m])
					{
						num7 = m;
						num8 = array[m];
					}
				}
				if (num7 >= 0)
				{
					Vec3 closestPointToEscape = availableFleePositions[num7].GetClosestPointToEscape(runnerPosition.AsVec2);
					return new WorldPosition(this.Scene, UIntPtr.Zero, closestPointToEscape, false);
				}
			}
			float[] array3 = new float[4];
			for (int n = 0; n < num; n++)
			{
				ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool> valueTuple = chaserData[n];
				Vec2 asVec2 = valueTuple.Item2.AsVec2;
				int item5 = chaserData[n].Item3;
				Vec2 vec2;
				if (item5 > 1)
				{
					Vec2 item6 = chaserData[n].Item4;
					Vec2 item7 = chaserData[n].Item5;
					vec2 = MBMath.GetClosestPointInLineSegmentToPoint(runnerPosition.AsVec2, item6, item7) - runnerPosition.AsVec2;
				}
				else
				{
					vec2 = asVec2 - runnerPosition.AsVec2;
				}
				float num9 = vec2.Length;
				if (chaserData[n].Item6)
				{
					num9 *= 0.5f;
				}
				if (runnerHasMount)
				{
					num9 *= 2f;
				}
				float num10 = MBMath.ClampFloat(1f - (num9 - 40f) / 40f, 0.01f, 1f);
				Vec2 vec3 = vec2.Normalized();
				float num11 = 1.2f;
				float num12 = num10 * (float)item5 * num11;
				float num13 = num12 * MathF.Abs(vec3.x);
				float num14 = num12 * MathF.Abs(vec3.y);
				array3[(vec3.y < 0f) ? 0 : 1] -= num14;
				array3[(vec3.x < 0f) ? 2 : 3] -= num13;
				array3[(vec3.y < 0f) ? 1 : 0] += num14;
				array3[(vec3.x < 0f) ? 3 : 2] += num13;
			}
			float num15 = 0.04f;
			Vec3 vec4;
			Vec3 vec5;
			this.Scene.GetBoundingBox(out vec4, out vec5);
			Vec2 closestBoundaryPosition = this.GetClosestBoundaryPosition(new Vec2(runnerPosition.X, vec4.y));
			Vec2 closestBoundaryPosition2 = this.GetClosestBoundaryPosition(new Vec2(runnerPosition.X, vec5.y));
			Vec2 closestBoundaryPosition3 = this.GetClosestBoundaryPosition(new Vec2(vec4.x, runnerPosition.Y));
			Vec2 closestBoundaryPosition4 = this.GetClosestBoundaryPosition(new Vec2(vec5.x, runnerPosition.Y));
			float num16 = closestBoundaryPosition2.y - closestBoundaryPosition.y;
			float num17 = closestBoundaryPosition4.x - closestBoundaryPosition3.x;
			array3[0] += (num16 - (runnerPosition.Y - closestBoundaryPosition.y)) * num15;
			array3[1] += (num16 - (closestBoundaryPosition2.y - runnerPosition.Y)) * num15;
			array3[2] += (num17 - (runnerPosition.X - closestBoundaryPosition3.x)) * num15;
			array3[3] += (num17 - (closestBoundaryPosition4.x - runnerPosition.X)) * num15;
			Vec2 vec6;
			if (array3[0] >= array3[1] && array3[0] >= array3[2] && array3[0] >= array3[3])
			{
				vec6 = new Vec2(closestBoundaryPosition.x, closestBoundaryPosition.y);
			}
			else if (array3[1] >= array3[2] && array3[1] >= array3[3])
			{
				vec6 = new Vec2(closestBoundaryPosition2.x, closestBoundaryPosition2.y);
			}
			else if (array3[2] >= array3[3])
			{
				vec6 = new Vec2(closestBoundaryPosition3.x, closestBoundaryPosition3.y);
			}
			else
			{
				vec6 = new Vec2(closestBoundaryPosition4.x, closestBoundaryPosition4.y);
			}
			return new WorldPosition(this.Scene, UIntPtr.Zero, new Vec3(vec6, runnerPosition.GetNavMeshZ(), -1f), false);
		}

		public MissionTimeTracker MissionTimeTracker { get; private set; }

		public MBReadOnlyList<FleePosition> GetFleePositionsForSide(BattleSideEnum side)
		{
			if (side == BattleSideEnum.NumSides)
			{
				Debug.FailedAssert("Flee position with invalid battle side field found!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "GetFleePositionsForSide", 1665);
				return null;
			}
			int num = (int)((side == BattleSideEnum.None) ? BattleSideEnum.Defender : (side + 1));
			return this._fleePositions[num];
		}

		public void AddToWeaponListForFriendlyFirePreventing(SiegeWeapon weapon)
		{
			this._attackerWeaponsForFriendlyFirePreventing.Add(weapon);
		}

		public Mission(MissionInitializerRecord rec, MissionState missionState)
		{
			this.Pointer = MBAPI.IMBMission.CreateMission(this);
			this._spawnedItemEntitiesCreatedAtRuntime = new List<SpawnedItemEntity>();
			this._missionObjects = new MBList<MissionObject>();
			this._activeMissionObjects = new MBList<MissionObject>();
			this._mountsWithoutRiders = new MBList<KeyValuePair<Agent, MissionTime>>();
			this._addedEntitiesInfo = new MBList<Mission.DynamicallyCreatedEntity>();
			this._emptyRuntimeMissionObjectIds = new Stack<ValueTuple<int, float>>();
			this.Boundaries = new Mission.MBBoundaryCollection(this);
			this.InitializerRecord = rec;
			this.CurrentState = Mission.State.NewlyCreated;
			this.IsInventoryAccessible = false;
			this.IsQuestScreenAccessible = true;
			this.IsCharacterWindowAccessible = true;
			this.IsPartyWindowAccessible = true;
			this.IsKingdomWindowAccessible = true;
			this.IsClanWindowAccessible = true;
			this.IsBannerWindowAccessible = false;
			this.IsEncyclopediaWindowAccessible = true;
			this._missiles = new Dictionary<int, Mission.Missile>();
			this._activeAgents = new MBList<Agent>(256);
			this._allAgents = new MBList<Agent>(256);
			for (int i = 0; i < 3; i++)
			{
				this._fleePositions[i] = new MBList<FleePosition>(32);
			}
			for (int j = 0; j < 2; j++)
			{
				this._initialAgentCountPerSide[j] = 0;
				this._removedAgentCountPerSide[j] = 0;
			}
			this.MissionBehaviors = new List<MissionBehavior>();
			this.MissionLogics = new List<MissionLogic>();
			this._otherMissionBehaviors = new List<MissionBehavior>();
			this._missionState = missionState;
			this._battleSpawnPathSelector = new BattleSpawnPathSelector(this);
			this.Teams = new Mission.TeamCollection(this);
			this._deploymentPlan = new MissionDeploymentPlan(this);
			this.MissionTimeTracker = new MissionTimeTracker();
		}

		private Lazy<MissionRecorder> _recorder
		{
			get
			{
				return new Lazy<MissionRecorder>(() => new MissionRecorder(this));
			}
		}

		public MissionRecorder Recorder
		{
			get
			{
				return this._recorder.Value;
			}
		}

		public void AddFleePosition(FleePosition fleePosition)
		{
			BattleSideEnum side = fleePosition.GetSide();
			if (side == BattleSideEnum.NumSides)
			{
				Debug.FailedAssert("Flee position with invalid battle side field found!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "AddFleePosition", 1746);
				return;
			}
			if (side == BattleSideEnum.None)
			{
				for (int i = 0; i < this._fleePositions.Length; i++)
				{
					this._fleePositions[i].Add(fleePosition);
				}
				return;
			}
			int num = (int)(side + 1);
			this._fleePositions[num].Add(fleePosition);
		}

		private void FreeResources()
		{
			this.MainAgent = null;
			this.Teams.ClearResources();
			this.SpectatorTeam = null;
			this._activeAgents = null;
			this._allAgents = null;
			if (GameNetwork.NetworkPeersValid)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null)
					{
						component.ClearAllVisuals(true);
						networkCommunicator.RemoveComponent(component);
					}
					MissionRepresentativeBase component2 = networkCommunicator.GetComponent<MissionRepresentativeBase>();
					if (component2 != null)
					{
						networkCommunicator.RemoveComponent(component2);
					}
				}
			}
			if (GameNetwork.DisconnectedNetworkPeers != null)
			{
				Debug.Print("DisconnectedNetworkPeers.Clear()", 0, Debug.DebugColor.White, 17179869184UL);
				GameNetwork.DisconnectedNetworkPeers.Clear();
			}
			this._missionState = null;
		}

		public void RetreatMission()
		{
			foreach (MissionLogic missionLogic in this.MissionLogics)
			{
				missionLogic.OnRetreatMission();
			}
			if (MBEditor.EditModeEnabled && MBEditor.IsEditModeOn)
			{
				MBEditor.LeaveEditMissionMode();
				return;
			}
			this.EndMission();
		}

		public void SurrenderMission()
		{
			foreach (MissionLogic missionLogic in this.MissionLogics)
			{
				missionLogic.OnSurrenderMission();
			}
			if (MBEditor.EditModeEnabled && MBEditor.IsEditModeOn)
			{
				MBEditor.LeaveEditMissionMode();
				return;
			}
			this.EndMission();
		}

		public bool HasMissionBehavior<T>() where T : MissionBehavior
		{
			return this.GetMissionBehavior<T>() != null;
		}

		[UsedImplicitly]
		[MBCallback]
		internal void OnAgentAddedAsCorpse(Agent affectedAgent)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				for (int i = 0; i < affectedAgent.GetAttachedWeaponsCount(); i++)
				{
					if (affectedAgent.GetAttachedWeapon(i).Item.ItemFlags.HasAnyFlag(ItemFlags.CanBePickedUpFromCorpse))
					{
						this.SpawnAttachedWeaponOnCorpse(affectedAgent, i, -1);
					}
				}
				affectedAgent.ClearAttachedWeapons();
			}
		}

		public void SpawnAttachedWeaponOnCorpse(Agent agent, int attachedWeaponIndex, int forcedSpawnIndex)
		{
			agent.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
			MissionWeapon attachedWeapon = agent.GetAttachedWeapon(attachedWeaponIndex);
			GameEntity attachedWeaponEntity = agent.AgentVisuals.GetAttachedWeaponEntity(attachedWeaponIndex);
			attachedWeaponEntity.CreateAndAddScriptComponent(typeof(SpawnedItemEntity).Name);
			SpawnedItemEntity firstScriptOfType = attachedWeaponEntity.GetFirstScriptOfType<SpawnedItemEntity>();
			if (forcedSpawnIndex >= 0)
			{
				firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
			}
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SpawnAttachedWeaponOnCorpse(agent.Index, attachedWeaponIndex, firstScriptOfType.Id.Id));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.SpawnWeaponAux(attachedWeaponEntity, attachedWeapon, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, Vec3.Zero, Vec3.Zero, false);
		}

		public void AddMountWithoutRider(Agent mount)
		{
			this._mountsWithoutRiders.Add(new KeyValuePair<Agent, MissionTime>(mount, MissionTime.Now));
		}

		public void RemoveMountWithoutRider(Agent mount)
		{
			for (int i = 0; i < this._mountsWithoutRiders.Count; i++)
			{
				if (this._mountsWithoutRiders[i].Key == mount)
				{
					this._mountsWithoutRiders.RemoveAt(i);
					return;
				}
			}
		}

		[UsedImplicitly]
		[MBCallback]
		internal void OnAgentDeleted(Agent affectedAgent)
		{
			if (affectedAgent != null)
			{
				affectedAgent.State = AgentState.Deleted;
				foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
				{
					missionBehavior.OnAgentDeleted(affectedAgent);
				}
				this._allAgents.Remove(affectedAgent);
				affectedAgent.OnDelete();
				affectedAgent.SetTeam(null, false);
			}
		}

		[UsedImplicitly]
		[MBCallback]
		internal void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			Mission.OnBeforeAgentRemovedDelegate onBeforeAgentRemoved = this.OnBeforeAgentRemoved;
			if (onBeforeAgentRemoved != null)
			{
				onBeforeAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			}
			affectedAgent.State = agentState;
			if (affectorAgent != null && affectorAgent.Team != affectedAgent.Team)
			{
				affectorAgent.KillCount++;
			}
			Team team = affectedAgent.Team;
			if (team != null)
			{
				team.DeactivateAgent(affectedAgent);
			}
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnEarlyAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			}
			foreach (MissionBehavior missionBehavior2 in this.MissionBehaviors)
			{
				missionBehavior2.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			}
			bool flag = this.MainAgent == affectedAgent;
			if (flag)
			{
				affectedAgent.OnMainAgentWieldedItemChange = null;
				this.MainAgent = null;
			}
			affectedAgent.OnAgentWieldedItemChange = null;
			affectedAgent.OnAgentMountedStateChanged = null;
			if (affectedAgent.Team != null && affectedAgent.Team.Side != BattleSideEnum.None)
			{
				this._removedAgentCountPerSide[(int)affectedAgent.Team.Side]++;
			}
			this._activeAgents.Remove(affectedAgent);
			affectedAgent.OnRemove();
			if (affectedAgent.IsMount && affectedAgent.RiderAgent == null)
			{
				this.RemoveMountWithoutRider(affectedAgent);
			}
			if (flag)
			{
				affectedAgent.Team.DelegateCommandToAI();
			}
			if (!GameNetwork.IsClientOrReplay && agentState != AgentState.Routed && affectedAgent.GetAgentFlags().HasAnyFlag(AgentFlag.CanWieldWeapon))
			{
				EquipmentIndex wieldedItemIndex = affectedAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				if (wieldedItemIndex == EquipmentIndex.ExtraWeaponSlot)
				{
					WeaponComponentData currentUsageItem = affectedAgent.Equipment[wieldedItemIndex].CurrentUsageItem;
					if (currentUsageItem != null && currentUsageItem.WeaponClass == WeaponClass.Banner)
					{
						affectedAgent.DropItem(EquipmentIndex.ExtraWeaponSlot, WeaponClass.Undefined);
					}
				}
			}
		}

		public void OnObjectDisabled(DestructableComponent destructionComponent)
		{
			UsableMachine firstScriptOfType = destructionComponent.GameEntity.GetFirstScriptOfType<UsableMachine>();
			if (firstScriptOfType != null)
			{
				firstScriptOfType.Disable();
			}
			if (destructionComponent != null)
			{
				destructionComponent.SetAbilityOfFaces(false);
			}
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnObjectDisabled(destructionComponent);
			}
		}

		public MissionObjectId SpawnWeaponAsDropFromMissile(int missileIndex, MissionObject attachedMissionObject, in MatrixFrame attachLocalFrame, Mission.WeaponSpawnFlags spawnFlags, in Vec3 velocity, in Vec3 angularVelocity, int forcedSpawnIndex)
		{
			this.PrepareMissileWeaponForDrop(missileIndex);
			Mission.Missile missile = this._missiles[missileIndex];
			if (attachedMissionObject != null)
			{
				attachedMissionObject.AddStuckMissile(missile.Entity);
			}
			if (attachedMissionObject != null)
			{
				GameEntity entity = missile.Entity;
				MatrixFrame matrixFrame = attachedMissionObject.GameEntity.GetGlobalFrame();
				matrixFrame = matrixFrame.TransformToParent(attachLocalFrame);
				entity.SetGlobalFrame(matrixFrame);
			}
			else
			{
				missile.Entity.SetGlobalFrame(attachLocalFrame);
			}
			missile.Entity.CreateAndAddScriptComponent(typeof(SpawnedItemEntity).Name);
			SpawnedItemEntity firstScriptOfType = missile.Entity.GetFirstScriptOfType<SpawnedItemEntity>();
			if (forcedSpawnIndex >= 0)
			{
				firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
			}
			this.SpawnWeaponAux(missile.Entity, missile.Weapon, spawnFlags, velocity, angularVelocity, true);
			return firstScriptOfType.Id;
		}

		[UsedImplicitly]
		[MBCallback]
		internal void SpawnWeaponAsDropFromAgent(Agent agent, EquipmentIndex equipmentIndex, ref Vec3 velocity, ref Vec3 angularVelocity, Mission.WeaponSpawnFlags spawnFlags)
		{
			Vec3 velocity2 = agent.Velocity;
			if ((velocity - velocity2).LengthSquared > 100f)
			{
				Vec3 vec = (velocity - velocity2).NormalizedCopy() * 10f;
				velocity = velocity2 + vec;
			}
			this.SpawnWeaponAsDropFromAgentAux(agent, equipmentIndex, ref velocity, ref angularVelocity, spawnFlags, -1);
		}

		public void SpawnWeaponAsDropFromAgentAux(Agent agent, EquipmentIndex equipmentIndex, ref Vec3 velocity, ref Vec3 angularVelocity, Mission.WeaponSpawnFlags spawnFlags, int forcedSpawnIndex)
		{
			agent.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
			agent.PrepareWeaponForDropInEquipmentSlot(equipmentIndex, (spawnFlags & Mission.WeaponSpawnFlags.WithHolster) > Mission.WeaponSpawnFlags.None);
			GameEntity weaponEntityFromEquipmentSlot = agent.GetWeaponEntityFromEquipmentSlot(equipmentIndex);
			weaponEntityFromEquipmentSlot.CreateAndAddScriptComponent(typeof(SpawnedItemEntity).Name);
			SpawnedItemEntity firstScriptOfType = weaponEntityFromEquipmentSlot.GetFirstScriptOfType<SpawnedItemEntity>();
			if (forcedSpawnIndex >= 0)
			{
				firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
			}
			float maximumValue = CompressionMission.SpawnedItemVelocityCompressionInfo.GetMaximumValue();
			float maximumValue2 = CompressionMission.SpawnedItemAngularVelocityCompressionInfo.GetMaximumValue();
			if (velocity.LengthSquared > maximumValue * maximumValue)
			{
				velocity = velocity.NormalizedCopy() * maximumValue;
			}
			if (angularVelocity.LengthSquared > maximumValue2 * maximumValue2)
			{
				angularVelocity = angularVelocity.NormalizedCopy() * maximumValue2;
			}
			MissionWeapon missionWeapon = agent.Equipment[equipmentIndex];
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SpawnWeaponAsDropFromAgent(agent.Index, equipmentIndex, velocity, angularVelocity, spawnFlags, firstScriptOfType.Id.Id));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.SpawnWeaponAux(weaponEntityFromEquipmentSlot, missionWeapon, spawnFlags, velocity, angularVelocity, true);
			if (!GameNetwork.IsClientOrReplay)
			{
				for (int i = 0; i < missionWeapon.GetAttachedWeaponsCount(); i++)
				{
					if (missionWeapon.GetAttachedWeapon(i).Item.ItemFlags.HasAnyFlag(ItemFlags.CanBePickedUpFromCorpse))
					{
						this.SpawnAttachedWeaponOnSpawnedWeapon(firstScriptOfType, i, -1);
					}
				}
			}
			agent.OnWeaponDrop(equipmentIndex);
			Action<Agent, SpawnedItemEntity> onItemDrop = this.OnItemDrop;
			if (onItemDrop == null)
			{
				return;
			}
			onItemDrop(agent, firstScriptOfType);
		}

		public void SpawnAttachedWeaponOnSpawnedWeapon(SpawnedItemEntity spawnedWeapon, int attachmentIndex, int forcedSpawnIndex)
		{
			GameEntity child = spawnedWeapon.GameEntity.GetChild(attachmentIndex);
			child.CreateAndAddScriptComponent(typeof(SpawnedItemEntity).Name);
			SpawnedItemEntity firstScriptOfType = child.GetFirstScriptOfType<SpawnedItemEntity>();
			if (forcedSpawnIndex >= 0)
			{
				firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
			}
			this.SpawnWeaponAux(child, spawnedWeapon.WeaponCopy.GetAttachedWeapon(attachmentIndex), Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, Vec3.Zero, Vec3.Zero, false);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SpawnAttachedWeaponOnSpawnedWeapon(spawnedWeapon.Id, attachmentIndex, firstScriptOfType.Id.Id));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		public GameEntity SpawnWeaponWithNewEntity(ref MissionWeapon weapon, Mission.WeaponSpawnFlags spawnFlags, MatrixFrame frame)
		{
			return this.SpawnWeaponWithNewEntityAux(weapon, spawnFlags, frame, -1, null, false);
		}

		public GameEntity SpawnWeaponWithNewEntityAux(MissionWeapon weapon, Mission.WeaponSpawnFlags spawnFlags, MatrixFrame frame, int forcedSpawnIndex, MissionObject attachedMissionObject, bool hasLifeTime)
		{
			GameEntity gameEntity = GameEntityExtensions.Instantiate(this.Scene, weapon, spawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithHolster), true);
			gameEntity.CreateAndAddScriptComponent(typeof(SpawnedItemEntity).Name);
			SpawnedItemEntity firstScriptOfType = gameEntity.GetFirstScriptOfType<SpawnedItemEntity>();
			if (forcedSpawnIndex >= 0)
			{
				firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
			}
			if (attachedMissionObject != null)
			{
				attachedMissionObject.GameEntity.AddChild(gameEntity, false);
			}
			if (attachedMissionObject != null)
			{
				GameEntity gameEntity2 = gameEntity;
				MatrixFrame matrixFrame = attachedMissionObject.GameEntity.GetGlobalFrame();
				matrixFrame = matrixFrame.TransformToParent(frame);
				gameEntity2.SetGlobalFrame(matrixFrame);
			}
			else
			{
				gameEntity.SetGlobalFrame(frame);
			}
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SpawnWeaponWithNewEntity(weapon, spawnFlags, firstScriptOfType.Id.Id, frame, attachedMissionObject.Id, true, hasLifeTime));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				for (int i = 0; i < weapon.GetAttachedWeaponsCount(); i++)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new AttachWeaponToSpawnedWeapon(weapon.GetAttachedWeapon(i), firstScriptOfType.Id, weapon.GetAttachedWeaponFrame(i)));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			Vec3 zero = Vec3.Zero;
			this.SpawnWeaponAux(gameEntity, weapon, spawnFlags, zero, zero, hasLifeTime);
			return gameEntity;
		}

		public void AttachWeaponWithNewEntityToSpawnedWeapon(MissionWeapon weapon, SpawnedItemEntity spawnedItem, MatrixFrame attachLocalFrame)
		{
			GameEntity gameEntity = GameEntityExtensions.Instantiate(this.Scene, weapon, false, true);
			spawnedItem.GameEntity.AddChild(gameEntity, false);
			gameEntity.SetFrame(ref attachLocalFrame);
			spawnedItem.AttachWeaponToWeapon(weapon, ref attachLocalFrame);
		}

		private void SpawnWeaponAux(GameEntity weaponEntity, MissionWeapon weapon, Mission.WeaponSpawnFlags spawnFlags, Vec3 velocity, Vec3 angularVelocity, bool hasLifeTime)
		{
			SpawnedItemEntity firstScriptOfType = weaponEntity.GetFirstScriptOfType<SpawnedItemEntity>();
			bool flag = weapon.IsBanner();
			MissionWeapon missionWeapon = weapon;
			bool flag2 = !flag && hasLifeTime;
			Mission.WeaponSpawnFlags weaponSpawnFlags = spawnFlags;
			Vec3 vec = (flag ? velocity : Vec3.Zero);
			firstScriptOfType.Initialize(missionWeapon, flag2, weaponSpawnFlags, vec);
			if (spawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithPhysics | Mission.WeaponSpawnFlags.WithStaticPhysics))
			{
				BodyFlags bodyFlags = BodyFlags.OnlyCollideWithRaycast | BodyFlags.DroppedItem;
				if (weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.CannotBePickedUp) || spawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.CannotBePickedUp))
				{
					bodyFlags |= BodyFlags.DoNotCollideWithRaycast;
				}
				weaponEntity.BodyFlag |= bodyFlags;
				WeaponData weaponData = weapon.GetWeaponData(true);
				this.RecalculateBody(ref weaponData, weapon.Item.ItemComponent, weapon.Item.WeaponDesign, ref spawnFlags);
				if (flag)
				{
					weaponEntity.AddPhysics(weaponData.BaseWeight, weaponData.CenterOfMassShift, weaponData.Shape, velocity, angularVelocity, PhysicsMaterial.GetFromIndex(weaponData.PhysicsMaterialIndex), true, 0);
					weaponData.DeinitializeManagedPointers();
				}
				else if (spawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithPhysics | Mission.WeaponSpawnFlags.WithStaticPhysics))
				{
					weaponEntity.AddPhysics(weaponData.BaseWeight, weaponData.CenterOfMassShift, weaponData.Shape, velocity, angularVelocity, PhysicsMaterial.GetFromIndex(weaponData.PhysicsMaterialIndex), spawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithStaticPhysics), 0);
				}
				weaponData.DeinitializeManagedPointers();
			}
		}

		public void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
		{
			foreach (IMissionListener missionListener in this._listeners)
			{
				missionListener.OnEquipItemsFromSpawnEquipmentBegin(agent, creationType);
			}
		}

		public void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
		{
			foreach (IMissionListener missionListener in this._listeners)
			{
				missionListener.OnEquipItemsFromSpawnEquipment(agent, creationType);
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("flee_enemies", "mission")]
		public static string MakeEnemiesFleeCheat(List<string> strings)
		{
			if (GameNetwork.IsClientOrReplay)
			{
				return "does not work in multiplayer";
			}
			if (Mission.Current != null && Mission.Current.Agents != null)
			{
				foreach (Agent agent2 in Mission.Current.Agents.Where((Agent agent) => agent.IsHuman && agent.IsEnemyOf(Agent.Main)))
				{
					CommonAIComponent commonAIComponent = agent2.CommonAIComponent;
					if (commonAIComponent != null)
					{
						commonAIComponent.Panic();
					}
				}
				return "enemies are fleeing";
			}
			return "mission is not available";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("flee_team", "mission")]
		public static string MakeTeamFleeCheat(List<string> strings)
		{
			if (GameNetwork.IsClientOrReplay)
			{
				return "does not work in multiplayer";
			}
			if (Mission.Current == null || Mission.Current.Agents == null)
			{
				return "mission is not available";
			}
			string text = "Usage 1: flee_team [ Attacker | AttackerAlly | Defender | DefenderAlly ]\nUsage 2: flee_team [ Attacker | AttackerAlly | Defender | DefenderAlly ] [FormationNo]";
			if (strings.IsEmpty<string>() || strings[0] == "help")
			{
				return "makes an entire team or a team's formation flee battle.\n" + text;
			}
			if (strings.Count >= 3)
			{
				return "invalid number of parameters.\n" + text;
			}
			string text2 = strings[0];
			Team targetTeam = null;
			string text3 = text2.ToLower();
			if (!(text3 == "attacker"))
			{
				if (!(text3 == "attackerally"))
				{
					if (!(text3 == "defender"))
					{
						if (text3 == "defenderally")
						{
							targetTeam = Mission.Current.DefenderAllyTeam;
						}
					}
					else
					{
						targetTeam = Mission.Current.DefenderTeam;
					}
				}
				else
				{
					targetTeam = Mission.Current.AttackerAllyTeam;
				}
			}
			else
			{
				targetTeam = Mission.Current.AttackerTeam;
			}
			if (targetTeam == null)
			{
				return "given team is not valid";
			}
			Formation targetFormation = null;
			if (strings.Count == 2)
			{
				int num = 8;
				int num2 = int.Parse(strings[1]);
				if (num2 < 0 || num2 >= num)
				{
					return "invalid formation index. formation index should be between [0, " + (num - 1) + "]";
				}
				FormationClass formationClass = (FormationClass)num2;
				targetFormation = targetTeam.GetFormation(formationClass);
			}
			if (targetFormation == null)
			{
				IEnumerable<Agent> agents = Mission.Current.Agents;
				Func<Agent, bool> <>9__0;
				Func<Agent, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (Agent agent) => agent.IsHuman && agent.Team == targetTeam);
				}
				foreach (Agent agent3 in agents.Where(func))
				{
					CommonAIComponent commonAIComponent = agent3.CommonAIComponent;
					if (commonAIComponent != null)
					{
						commonAIComponent.Panic();
					}
				}
				return "agents in team: " + text2 + " are fleeing";
			}
			IEnumerable<Agent> agents2 = Mission.Current.Agents;
			Func<Agent, bool> <>9__1;
			Func<Agent, bool> func2;
			if ((func2 = <>9__1) == null)
			{
				func2 = (<>9__1 = (Agent agent) => agent.IsHuman && agent.Formation == targetFormation);
			}
			foreach (Agent agent2 in agents2.Where(func2))
			{
				CommonAIComponent commonAIComponent2 = agent2.CommonAIComponent;
				if (commonAIComponent2 != null)
				{
					commonAIComponent2.Panic();
				}
			}
			return string.Concat(new object[]
			{
				"agents in team: ",
				text2,
				" and formation: ",
				(int)targetFormation.FormationIndex,
				" (",
				targetFormation.FormationIndex.ToString(),
				") are fleeing"
			});
		}

		public void RecalculateBody(ref WeaponData weaponData, ItemComponent itemComponent, WeaponDesign craftedWeaponData, ref Mission.WeaponSpawnFlags spawnFlags)
		{
			WeaponComponent weaponComponent = (WeaponComponent)itemComponent;
			ItemObject item = weaponComponent.Item;
			if (spawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithHolster))
			{
				weaponData.Shape = (string.IsNullOrEmpty(item.HolsterBodyName) ? null : PhysicsShape.GetFromResource(item.HolsterBodyName, false));
			}
			else
			{
				weaponData.Shape = (string.IsNullOrEmpty(item.BodyName) ? null : PhysicsShape.GetFromResource(item.BodyName, false));
			}
			PhysicsShape physicsShape = weaponData.Shape;
			if (physicsShape == null)
			{
				Debug.FailedAssert("Item has no body! Applying a default body, but this should not happen! Check this!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "RecalculateBody", 2535);
				physicsShape = PhysicsShape.GetFromResource("bo_axe_short", false);
			}
			if (!weaponComponent.Item.ItemFlags.HasAnyFlag(ItemFlags.DoNotScaleBodyAccordingToWeaponLength))
			{
				if (spawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithHolster) || !item.RecalculateBody)
				{
					weaponData.Shape = physicsShape;
				}
				else
				{
					PhysicsShape physicsShape2 = physicsShape.CreateCopy();
					weaponData.Shape = physicsShape2;
					float num = (float)weaponComponent.PrimaryWeapon.WeaponLength * 0.01f;
					if (craftedWeaponData != null)
					{
						physicsShape2.Clear();
						physicsShape2.InitDescription();
						float num2 = 0f;
						float num3 = 0f;
						float num4 = 0f;
						for (int i = 0; i < craftedWeaponData.UsedPieces.Length; i++)
						{
							WeaponDesignElement weaponDesignElement = craftedWeaponData.UsedPieces[i];
							if (weaponDesignElement.IsValid)
							{
								float scaledPieceOffset = weaponDesignElement.ScaledPieceOffset;
								float num5 = craftedWeaponData.PiecePivotDistances[i];
								float num6 = num5 + scaledPieceOffset - weaponDesignElement.ScaledDistanceToPreviousPiece;
								float num7 = num5 - scaledPieceOffset + weaponDesignElement.ScaledDistanceToNextPiece;
								num2 = MathF.Min(num6, num2);
								if (num7 > num3)
								{
									num3 = num7;
									num4 = (num7 + num6) * 0.5f;
								}
							}
						}
						WeaponDesignElement weaponDesignElement2 = craftedWeaponData.UsedPieces[2];
						if (weaponDesignElement2.IsValid)
						{
							float scaledPieceOffset2 = weaponDesignElement2.ScaledPieceOffset;
							num2 -= scaledPieceOffset2;
						}
						physicsShape2.AddCapsule(new CapsuleData(0.035f, new Vec3(0f, 0f, craftedWeaponData.CraftedWeaponLength, -1f), new Vec3(0f, 0f, num2, -1f)));
						bool flag = false;
						if (craftedWeaponData.UsedPieces[1].IsValid)
						{
							float num8 = craftedWeaponData.PiecePivotDistances[1];
							physicsShape2.AddCapsule(new CapsuleData(0.05f, new Vec3(-0.1f, 0f, num8, -1f), new Vec3(0.1f, 0f, num8, -1f)));
							flag = true;
						}
						if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.OneHandedAxe || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedAxe || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.ThrowingAxe)
						{
							WeaponDesignElement weaponDesignElement3 = craftedWeaponData.UsedPieces[0];
							float num9 = craftedWeaponData.PiecePivotDistances[0];
							float num10 = num9 + weaponDesignElement3.CraftingPiece.Length * 0.8f;
							float num11 = num9 - weaponDesignElement3.CraftingPiece.Length * 0.8f;
							float num12 = num9 + weaponDesignElement3.CraftingPiece.Length;
							float num13 = num9 - weaponDesignElement3.CraftingPiece.Length;
							float bladeWidth = weaponDesignElement3.CraftingPiece.BladeData.BladeWidth;
							physicsShape2.AddCapsule(new CapsuleData(0.05f, new Vec3(0f, 0f, num10, -1f), new Vec3(-bladeWidth, 0f, num12, -1f)));
							physicsShape2.AddCapsule(new CapsuleData(0.05f, new Vec3(0f, 0f, num11, -1f), new Vec3(-bladeWidth, 0f, num13, -1f)));
							physicsShape2.AddCapsule(new CapsuleData(0.05f, new Vec3(-bladeWidth, 0f, num12, -1f), new Vec3(-bladeWidth, 0f, num13, -1f)));
							flag = true;
						}
						if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedPolearm || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.Javelin)
						{
							float num14 = craftedWeaponData.PiecePivotDistances[0];
							physicsShape2.AddCapsule(new CapsuleData(0.025f, new Vec3(-0.05f, 0f, num14, -1f), new Vec3(0.05f, 0f, num14, -1f)));
							flag = true;
						}
						if (!flag)
						{
							physicsShape2.AddCapsule(new CapsuleData(0.025f, new Vec3(-0.05f, 0f, num4, -1f), new Vec3(0.05f, 0f, num4, -1f)));
						}
					}
					else
					{
						weaponData.Shape.Prepare();
						int num15 = physicsShape.CapsuleCount();
						if (num15 == 0)
						{
							Debug.FailedAssert("Item has 0 body parts. Applying a default body, but this should not happen! Check this!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "RecalculateBody", 2673);
							return;
						}
						switch (weaponComponent.PrimaryWeapon.WeaponClass)
						{
						case WeaponClass.Dagger:
						case WeaponClass.OneHandedSword:
						case WeaponClass.TwoHandedSword:
						case WeaponClass.ThrowingKnife:
						{
							CapsuleData capsuleData = default(CapsuleData);
							physicsShape2.GetCapsule(ref capsuleData, 0);
							float radius = capsuleData.Radius;
							Vec3 p = capsuleData.P1;
							Vec3 p2 = capsuleData.P2;
							physicsShape2.SetCapsule(new CapsuleData(radius, new Vec3(p.x, p.y, p.z * num, -1f), p2), 0);
							break;
						}
						case WeaponClass.OneHandedAxe:
						case WeaponClass.TwoHandedAxe:
						case WeaponClass.Mace:
						case WeaponClass.TwoHandedMace:
						case WeaponClass.OneHandedPolearm:
						case WeaponClass.TwoHandedPolearm:
						case WeaponClass.LowGripPolearm:
						case WeaponClass.Arrow:
						case WeaponClass.Bolt:
						case WeaponClass.Crossbow:
						case WeaponClass.ThrowingAxe:
						case WeaponClass.Javelin:
						case WeaponClass.Banner:
						{
							CapsuleData capsuleData2 = default(CapsuleData);
							physicsShape2.GetCapsule(ref capsuleData2, 0);
							float radius2 = capsuleData2.Radius;
							Vec3 p3 = capsuleData2.P1;
							Vec3 p4 = capsuleData2.P2;
							physicsShape2.SetCapsule(new CapsuleData(radius2, new Vec3(p3.x, p3.y, p3.z * num, -1f), p4), 0);
							for (int j = 1; j < num15; j++)
							{
								CapsuleData capsuleData3 = default(CapsuleData);
								physicsShape2.GetCapsule(ref capsuleData3, j);
								float radius3 = capsuleData3.Radius;
								Vec3 p5 = capsuleData3.P1;
								Vec3 p6 = capsuleData3.P2;
								physicsShape2.SetCapsule(new CapsuleData(radius3, new Vec3(p5.x, p5.y, p5.z * num, -1f), new Vec3(p6.x, p6.y, p6.z * num, -1f)), j);
							}
							break;
						}
						case WeaponClass.SmallShield:
						case WeaponClass.LargeShield:
							Debug.FailedAssert("Shields should not have recalculate body flag.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "RecalculateBody", 2747);
							break;
						}
					}
				}
			}
			weaponData.CenterOfMassShift = weaponData.Shape.GetWeaponCenterOfMass();
		}

		[UsedImplicitly]
		[MBCallback]
		internal void OnPreTick(float dt)
		{
			this.waitTickCompletion();
			for (int i = this.MissionBehaviors.Count - 1; i >= 0; i--)
			{
				this.MissionBehaviors[i].OnPreMissionTick(dt);
			}
			this.TickDebugAgents();
		}

		[UsedImplicitly]
		[MBCallback]
		internal void ApplySkeletonScaleToAllEquippedItems(string itemName)
		{
			int count = this.Agents.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < 12; j++)
				{
					EquipmentElement equipmentElement = this.Agents[i].SpawnEquipment[j];
					if (!equipmentElement.IsEmpty && equipmentElement.Item.StringId == itemName)
					{
						HorseComponent horseComponent = equipmentElement.Item.HorseComponent;
						if (((horseComponent != null) ? horseComponent.SkeletonScale : null) != null)
						{
							this.Agents[i].AgentVisuals.ApplySkeletonScale(equipmentElement.Item.HorseComponent.SkeletonScale.MountSitBoneScale, equipmentElement.Item.HorseComponent.SkeletonScale.MountRadiusAdder, equipmentElement.Item.HorseComponent.SkeletonScale.BoneIndices, equipmentElement.Item.HorseComponent.SkeletonScale.Scales);
							break;
						}
					}
				}
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_facial_anim_to_agent", "mission")]
		public static string SetFacialAnimToAgent(List<string> strings)
		{
			Mission mission = Mission.Current;
			if (mission == null)
			{
				return "Mission could not be found";
			}
			if (strings.Count != 2)
			{
				return "Enter agent index and animation name please";
			}
			int num;
			if (int.TryParse(strings[0], out num) && num >= 0)
			{
				foreach (Agent agent in mission.Agents)
				{
					if (agent.Index == num)
					{
						agent.SetAgentFacialAnimation(Agent.FacialAnimChannel.High, strings[1], true);
						return "Done";
					}
				}
			}
			return "Please enter a valid agent index";
		}

		private void waitTickCompletion()
		{
			while (!this.tickCompleted)
			{
				Thread.Sleep(1);
			}
		}

		public void TickAgentsAndTeamsImp(float dt)
		{
			foreach (Agent agent in this.AllAgents)
			{
				agent.Tick(dt);
			}
			foreach (Team team in this.Teams)
			{
				team.Tick(dt);
			}
			this.tickCompleted = true;
			foreach (MBSubModuleBase mbsubModuleBase in Module.GetInstance().SubModules)
			{
				mbsubModuleBase.AfterAsyncTickTick(dt);
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("formation_speed_adjustment_enabled", "ai")]
		public static string EnableSpeedAdjustmentCommand(List<string> strings)
		{
			if (!GameNetwork.IsSessionActive)
			{
				HumanAIComponent.FormationSpeedAdjustmentEnabled = !HumanAIComponent.FormationSpeedAdjustmentEnabled;
				string text = "Speed Adjustment ";
				if (HumanAIComponent.FormationSpeedAdjustmentEnabled)
				{
					text += "enabled";
				}
				else
				{
					text += "disabled";
				}
				return text;
			}
			return "Does not work on multiplayer.";
		}

		public void OnTick(float dt, float realDt, bool updateCamera, bool doAsyncAITick)
		{
			this.ApplyGeneratedCombatLogs();
			if (this.InputManager == null)
			{
				this.InputManager = new EmptyInputContext();
			}
			this.MissionTimeTracker.Tick(dt);
			this.CheckMissionEnd(this.CurrentTime);
			if (this.IsFastForward && this.MissionEnded)
			{
				this.IsFastForward = false;
			}
			if (this.CurrentState == Mission.State.Continuing)
			{
				if (this._inMissionLoadingScreenTimer != null && this._inMissionLoadingScreenTimer.Check(this.CurrentTime))
				{
					this._inMissionLoadingScreenTimer = null;
					Action onLoadingEndedAction = this._onLoadingEndedAction;
					if (onLoadingEndedAction != null)
					{
						onLoadingEndedAction();
					}
					LoadingWindow.DisableGlobalLoadingWindow();
				}
				for (int i = this.MissionBehaviors.Count - 1; i >= 0; i--)
				{
					this.MissionBehaviors[i].OnPreDisplayMissionTick(dt);
				}
				if (!GameNetwork.IsDedicatedServer && updateCamera)
				{
					this._missionState.Handler.UpdateCamera(this, realDt);
				}
				this.tickCompleted = false;
				for (int j = this.MissionBehaviors.Count - 1; j >= 0; j--)
				{
					this.MissionBehaviors[j].OnMissionTick(dt);
				}
				for (int k = this._dynamicEntities.Count - 1; k >= 0; k--)
				{
					Mission.DynamicEntityInfo dynamicEntityInfo = this._dynamicEntities[k];
					if (dynamicEntityInfo.TimerToDisable.Check(this.CurrentTime))
					{
						dynamicEntityInfo.Entity.RemoveEnginePhysics();
						dynamicEntityInfo.Entity.Remove(79);
						this._dynamicEntities.RemoveAt(k);
					}
				}
				this.HandleSpawnedItems();
				DebugNetworkEventStatistics.EndTick(dt);
				if (this.CurrentState == Mission.State.Continuing && this.IsFriendlyMission && !this.IsInPhotoMode)
				{
					if (this.InputManager.IsGameKeyDown(4))
					{
						this.OnEndMissionRequest();
					}
					else
					{
						this._leaveMissionTimer = null;
					}
				}
				if (doAsyncAITick)
				{
					this.TickAgentsAndTeamsAsync(dt);
					return;
				}
				this.TickAgentsAndTeamsImp(dt);
			}
		}

		public void RemoveSpawnedItemsAndMissiles()
		{
			this.ClearMissiles();
			this._missiles.Clear();
			this.RemoveSpawnedMissionObjects();
		}

		public void AfterStart()
		{
			this._activeAgents.Clear();
			this._allAgents.Clear();
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnBeforeMissionBehaviorInitialize(this);
			}
			for (int i = 0; i < this.MissionBehaviors.Count; i++)
			{
				this.MissionBehaviors[i].OnBehaviorInitialize();
			}
			foreach (MBSubModuleBase mbsubModuleBase2 in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase2.OnMissionBehaviorInitialize(this);
			}
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.EarlyStart();
			}
			this._battleSpawnPathSelector.Initialize();
			this._deploymentPlan.CreateReinforcementPlans();
			foreach (MissionBehavior missionBehavior2 in this.MissionBehaviors)
			{
				missionBehavior2.AfterStart();
			}
			foreach (MissionObject missionObject in this.MissionObjects)
			{
				missionObject.AfterMissionStart();
			}
			if (MissionGameModels.Current.ApplyWeatherEffectsModel != null)
			{
				MissionGameModels.Current.ApplyWeatherEffectsModel.ApplyWeatherEffects();
			}
			this.CurrentState = Mission.State.Continuing;
		}

		public void OnEndMissionRequest()
		{
			foreach (MissionLogic missionLogic in this.MissionLogics)
			{
				bool flag;
				InquiryData inquiryData = missionLogic.OnEndMissionRequest(out flag);
				if (!flag)
				{
					this._leaveMissionTimer = null;
					return;
				}
				if (inquiryData != null)
				{
					this._leaveMissionTimer = null;
					InformationManager.ShowInquiry(inquiryData, true, false);
					return;
				}
			}
			if (this._leaveMissionTimer != null)
			{
				if (this._leaveMissionTimer.ElapsedTime > 0.6f)
				{
					this._leaveMissionTimer = null;
					this.EndMission();
					return;
				}
			}
			else
			{
				this._leaveMissionTimer = new BasicMissionTimer();
			}
		}

		public float GetMissionEndTimeInSeconds()
		{
			return 0.6f;
		}

		public float GetMissionEndTimerValue()
		{
			if (this._leaveMissionTimer == null)
			{
				return -1f;
			}
			return this._leaveMissionTimer.ElapsedTime;
		}

		private void ApplyGeneratedCombatLogs()
		{
			if (!this._combatLogsCreated.IsEmpty)
			{
				CombatLogData combatLogData;
				while (this._combatLogsCreated.TryDequeue(out combatLogData))
				{
					CombatLogManager.GenerateCombatLog(combatLogData);
				}
			}
		}

		public int GetMemberCountOfSide(BattleSideEnum side)
		{
			int num = 0;
			foreach (Team team in this.Teams)
			{
				if (team.Side == side)
				{
					num += team.ActiveAgents.Count;
				}
			}
			return num;
		}

		public Path GetInitialSpawnPath()
		{
			return this._battleSpawnPathSelector.InitialPath;
		}

		public SpawnPathData GetInitialSpawnPathDataOfSide(BattleSideEnum battleSide)
		{
			SpawnPathData spawnPathData;
			this._battleSpawnPathSelector.GetInitialPathDataOfSide(battleSide, out spawnPathData);
			return spawnPathData;
		}

		public MBReadOnlyList<SpawnPathData> GetReinforcementPathsDataOfSide(BattleSideEnum battleSide)
		{
			return this._battleSpawnPathSelector.GetReinforcementPathsDataOfSide(battleSide);
		}

		public void GetTroopSpawnFrameWithIndex(AgentBuildData buildData, int troopSpawnIndex, int troopSpawnCount, out Vec3 troopSpawnPosition, out Vec2 troopSpawnDirection)
		{
			Formation agentFormation = buildData.AgentFormation;
			BasicCharacterObject agentCharacter = buildData.AgentCharacter;
			troopSpawnPosition = Vec3.Invalid;
			WorldPosition worldPosition;
			Vec2 direction;
			if (buildData.AgentSpawnsIntoOwnFormation)
			{
				worldPosition = agentFormation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3);
				direction = agentFormation.Direction;
			}
			else
			{
				IAgentOriginBase agentOrigin = buildData.AgentOrigin;
				bool agentIsReinforcement = buildData.AgentIsReinforcement;
				BattleSideEnum side = buildData.AgentTeam.Side;
				if (buildData.AgentSpawnsUsingOwnTroopClass)
				{
					FormationClass agentTroopClass = this.GetAgentTroopClass(side, agentCharacter);
					this.GetFormationSpawnFrame(side, agentTroopClass, agentIsReinforcement, out worldPosition, out direction);
				}
				else if (agentCharacter.IsHero && agentOrigin != null && agentOrigin.BattleCombatant != null && agentCharacter == agentOrigin.BattleCombatant.General && this.GetFormationSpawnClass(side, FormationClass.NumberOfRegularFormations, agentIsReinforcement) == FormationClass.NumberOfRegularFormations)
				{
					this.GetFormationSpawnFrame(side, FormationClass.NumberOfRegularFormations, agentIsReinforcement, out worldPosition, out direction);
				}
				else
				{
					this.GetFormationSpawnFrame(side, agentFormation.FormationIndex, agentIsReinforcement, out worldPosition, out direction);
				}
			}
			bool flag = !buildData.AgentNoHorses && agentFormation.HasAnyMountedUnit;
			WorldPosition? worldPosition2;
			Vec2? vec;
			agentFormation.GetUnitSpawnFrameWithIndex(troopSpawnIndex, worldPosition, direction, agentFormation.Width, troopSpawnCount, agentFormation.UnitSpacing, flag, out worldPosition2, out vec);
			if (worldPosition2 != null && buildData.MakeUnitStandOutDistance != 0f)
			{
				worldPosition2.Value.SetVec2(worldPosition2.Value.AsVec2 + vec.Value * buildData.MakeUnitStandOutDistance);
			}
			if (worldPosition2 != null)
			{
				if (worldPosition2.Value.GetNavMesh() == UIntPtr.Zero)
				{
					troopSpawnPosition = this.Scene.GetLastPointOnNavigationMeshFromWorldPositionToDestination(ref worldPosition, worldPosition2.Value.AsVec2);
				}
				else
				{
					troopSpawnPosition = worldPosition2.Value.GetGroundVec3();
				}
			}
			if (!troopSpawnPosition.IsValid)
			{
				troopSpawnPosition = worldPosition.GetGroundVec3();
			}
			troopSpawnDirection = ((vec != null) ? vec.Value : direction);
		}

		public void GetFormationSpawnFrame(BattleSideEnum side, FormationClass formationClass, bool isReinforcement, out WorldPosition spawnPosition, out Vec2 spawnDirection)
		{
			DeploymentPlanType deploymentPlanType = (isReinforcement ? DeploymentPlanType.Reinforcement : DeploymentPlanType.Initial);
			IFormationDeploymentPlan formationPlan = this._deploymentPlan.GetFormationPlan(side, formationClass, deploymentPlanType);
			spawnPosition = formationPlan.CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3);
			spawnDirection = formationPlan.GetDirection();
		}

		public WorldFrame GetBattleSideInitialSpawnPathFrame(BattleSideEnum battleSide, float pathOffset = 0f)
		{
			SpawnPathData initialSpawnPathDataOfSide = this.GetInitialSpawnPathDataOfSide(battleSide);
			if (initialSpawnPathDataOfSide.IsValid)
			{
				Vec2 vec;
				Vec2 vec2;
				initialSpawnPathDataOfSide.GetOrientedSpawnPathPosition(out vec, out vec2, pathOffset);
				Mat3 identity = Mat3.Identity;
				identity.RotateAboutUp(vec2.RotationInRadians);
				WorldPosition worldPosition = new WorldPosition(this.Scene, UIntPtr.Zero, vec.ToVec3(0f), false);
				return new WorldFrame(identity, worldPosition);
			}
			return WorldFrame.Invalid;
		}

		private void BuildAgent(Agent agent, AgentBuildData agentBuildData)
		{
			if (agent == null)
			{
				throw new MBNullParameterException("agent");
			}
			agent.Build(agentBuildData, this._agentCreationIndex);
			this._agentCreationIndex++;
			if (!agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty)
			{
				EquipmentElement equipmentElement = agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot];
				if (equipmentElement.Item.HorseComponent.BodyLength != 0)
				{
					agent.SetInitialAgentScale(0.01f * (float)equipmentElement.Item.HorseComponent.BodyLength);
				}
			}
			agent.EquipItemsFromSpawnEquipment(true);
			agent.InitializeAgentRecord();
			agent.AgentVisuals.BatchLastLodMeshes();
			agent.PreloadForRendering();
			ActionIndexValueCache currentActionValue = agent.GetCurrentActionValue(0);
			if (currentActionValue != ActionIndexValueCache.act_none)
			{
				agent.SetActionChannel(0, currentActionValue, false, 0UL, 0f, 1f, -0.2f, 0.4f, MBRandom.RandomFloat * 0.8f, false, -0.2f, 0, true);
			}
			agent.InitializeComponents();
			if (agent.Controller == Agent.ControllerType.Player)
			{
				this.ResetFirstThirdPersonView();
			}
			this._activeAgents.Add(agent);
			this._allAgents.Add(agent);
		}

		private Agent CreateAgent(Monster monster, bool isFemale, int instanceNo, Agent.CreationType creationType, float stepSize, int forcedAgentIndex, int weight, BasicCharacterObject characterObject)
		{
			AnimationSystemData animationSystemData = monster.FillAnimationSystemData(stepSize, false, isFemale);
			AgentCapsuleData agentCapsuleData = monster.FillCapsuleData();
			AgentSpawnData agentSpawnData = monster.FillSpawnData(null);
			Mission.AgentCreationResult agentCreationResult = this.CreateAgentInternal(monster.Flags, forcedAgentIndex, isFemale, ref agentSpawnData, ref agentCapsuleData, ref animationSystemData, instanceNo);
			Agent agent = new Agent(this, agentCreationResult, creationType, monster);
			agent.Character = characterObject;
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnAgentCreated(agent);
			}
			return agent;
		}

		public void SetBattleAgentCount(int agentCount)
		{
			if (this._agentCount == 0 || this._agentCount > agentCount)
			{
				this._agentCount = agentCount;
			}
		}

		public Vec2 GetFormationSpawnPosition(BattleSideEnum side, FormationClass formationClass, bool isReinforcement)
		{
			DeploymentPlanType deploymentPlanType = (isReinforcement ? DeploymentPlanType.Reinforcement : DeploymentPlanType.Initial);
			return this._deploymentPlan.GetFormationPlan(side, formationClass, deploymentPlanType).CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.None).AsVec2;
		}

		public FormationClass GetFormationSpawnClass(BattleSideEnum side, FormationClass formationClass, bool isReinforcement)
		{
			DeploymentPlanType deploymentPlanType = (isReinforcement ? DeploymentPlanType.Reinforcement : DeploymentPlanType.Initial);
			return this._deploymentPlan.GetFormationPlan(side, formationClass, deploymentPlanType).SpawnClass;
		}

		public Agent SpawnAgent(AgentBuildData agentBuildData, bool spawnFromAgentVisuals = false)
		{
			BasicCharacterObject agentCharacter = agentBuildData.AgentCharacter;
			if (agentCharacter == null)
			{
				throw new MBNullParameterException("npcCharacterObject");
			}
			int num = -1;
			if (agentBuildData.AgentIndexOverriden)
			{
				num = agentBuildData.AgentIndex;
			}
			Agent agent = this.CreateAgent(agentBuildData.AgentMonster, agentBuildData.GenderOverriden ? agentBuildData.AgentIsFemale : agentCharacter.IsFemale, 0, Agent.CreationType.FromCharacterObj, agentCharacter.GetStepSize(), num, agentBuildData.AgentMonster.Weight, agentCharacter);
			agent.FormationPositionPreference = agentCharacter.FormationPositionPreference;
			float num2 = (agentBuildData.AgeOverriden ? ((float)agentBuildData.AgentAge) : agentCharacter.Age);
			if (num2 == 0f)
			{
				agentBuildData.Age(29);
			}
			else if (MBBodyProperties.GetMaturityType(num2) < BodyMeshMaturityType.Teenager && (this.Mode == MissionMode.Battle || this.Mode == MissionMode.Duel || this.Mode == MissionMode.Tournament || this.Mode == MissionMode.Stealth))
			{
				agentBuildData.Age(27);
			}
			if (agentBuildData.BodyPropertiesOverriden)
			{
				agent.UpdateBodyProperties(agentBuildData.AgentBodyProperties);
				if (!agentBuildData.AgeOverriden)
				{
					agent.Age = agentCharacter.Age;
				}
			}
			agent.BodyPropertiesSeed = agentBuildData.AgentEquipmentSeed;
			if (agentBuildData.AgeOverriden)
			{
				agent.Age = (float)agentBuildData.AgentAge;
			}
			if (agentBuildData.GenderOverriden)
			{
				agent.IsFemale = agentBuildData.AgentIsFemale;
			}
			agent.SetTeam(agentBuildData.AgentTeam, false);
			agent.SetClothingColor1(agentBuildData.AgentClothingColor1);
			agent.SetClothingColor2(agentBuildData.AgentClothingColor2);
			agent.SetRandomizeColors(agentBuildData.RandomizeColors);
			agent.Origin = agentBuildData.AgentOrigin;
			Formation agentFormation = agentBuildData.AgentFormation;
			if (agentFormation != null && !agentFormation.HasBeenPositioned)
			{
				this.SetFormationPositioningFromDeploymentPlan(agentFormation);
			}
			if (agentBuildData.AgentInitialPosition == null)
			{
				BattleSideEnum side = agentBuildData.AgentTeam.Side;
				Vec3 vec = Vec3.Invalid;
				Vec2 vec2 = Vec2.Invalid;
				if (agentCharacter == Game.Current.PlayerTroop && this._deploymentPlan.HasPlayerSpawnFrame(side))
				{
					WorldPosition worldPosition;
					Vec2 vec3;
					this._deploymentPlan.GetPlayerSpawnFrame(side, out worldPosition, out vec3);
					vec = worldPosition.GetGroundVec3();
					vec2 = vec3;
				}
				else if (agentFormation != null)
				{
					int num3;
					int num4;
					if (agentBuildData.AgentSpawnsIntoOwnFormation)
					{
						num3 = agentFormation.CountOfUnits;
						num4 = num3 + 1;
					}
					else if (agentBuildData.AgentFormationTroopSpawnIndex >= 0 && agentBuildData.AgentFormationTroopSpawnCount > 0)
					{
						num3 = agentBuildData.AgentFormationTroopSpawnIndex;
						num4 = agentBuildData.AgentFormationTroopSpawnCount;
					}
					else
					{
						num3 = agentFormation.GetNextSpawnIndex();
						num4 = num3 + 1;
					}
					if (num3 >= num4)
					{
						num4 = num3 + 1;
					}
					this.GetTroopSpawnFrameWithIndex(agentBuildData, num3, num4, out vec, out vec2);
				}
				else
				{
					WorldPosition worldPosition2;
					this.GetFormationSpawnFrame(side, FormationClass.NumberOfAllFormations, agentBuildData.AgentIsReinforcement, out worldPosition2, out vec2);
					vec = worldPosition2.GetGroundVec3();
				}
				agentBuildData.InitialPosition(vec).InitialDirection(vec2);
			}
			Agent agent2 = agent;
			Vec3 vec4 = agentBuildData.AgentInitialPosition.GetValueOrDefault();
			Vec2 vec5 = agentBuildData.AgentInitialDirection.GetValueOrDefault();
			agent2.SetInitialFrame(vec4, vec5, agentBuildData.AgentCanSpawnOutsideOfMissionBoundary);
			if (agentCharacter.AllEquipments == null)
			{
				Debug.Print("characterObject.AllEquipments is null for \"" + agentCharacter.StringId + "\".", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			if (agentCharacter.AllEquipments != null)
			{
				if (agentCharacter.AllEquipments.Any((Equipment eq) => eq == null))
				{
					Debug.Print("Character with id \"" + agentCharacter.StringId + "\" has a null equipment in its AllEquipments.", 0, Debug.DebugColor.White, 17592186044416UL);
				}
			}
			if (agentCharacter.AllEquipments.Where((Equipment eq) => eq.IsCivilian) == null)
			{
				agentBuildData.CivilianEquipment(false);
			}
			if (agentCharacter.IsHero)
			{
				agentBuildData.FixedEquipment(true);
			}
			Equipment equipment;
			if (agentBuildData.AgentOverridenSpawnEquipment != null)
			{
				equipment = agentBuildData.AgentOverridenSpawnEquipment.Clone(false);
			}
			else if (!agentBuildData.AgentFixedEquipment)
			{
				equipment = Equipment.GetRandomEquipmentElements(agent.Character, !Game.Current.GameType.IsCoreOnlyGameMode, agentBuildData.AgentCivilianEquipment, agentBuildData.AgentEquipmentSeed);
			}
			else
			{
				equipment = agentCharacter.GetFirstEquipment(agentBuildData.AgentCivilianEquipment).Clone(false);
			}
			Agent agent3 = null;
			if (agentBuildData.AgentNoHorses)
			{
				equipment[EquipmentIndex.ArmorItemEndSlot] = default(EquipmentElement);
				equipment[EquipmentIndex.HorseHarness] = default(EquipmentElement);
			}
			if (agentBuildData.AgentNoWeapons)
			{
				equipment[EquipmentIndex.WeaponItemBeginSlot] = default(EquipmentElement);
				equipment[EquipmentIndex.Weapon1] = default(EquipmentElement);
				equipment[EquipmentIndex.Weapon2] = default(EquipmentElement);
				equipment[EquipmentIndex.Weapon3] = default(EquipmentElement);
				equipment[EquipmentIndex.ExtraWeaponSlot] = default(EquipmentElement);
			}
			if (agentCharacter.IsHero)
			{
				ItemObject itemObject = null;
				ItemObject item = equipment[EquipmentIndex.ExtraWeaponSlot].Item;
				if (item != null && item.IsBannerItem && item.BannerComponent != null)
				{
					itemObject = item;
					equipment[EquipmentIndex.ExtraWeaponSlot] = default(EquipmentElement);
				}
				else if (agentBuildData.AgentBannerItem != null)
				{
					itemObject = agentBuildData.AgentBannerItem;
				}
				if (itemObject != null)
				{
					agent.SetFormationBanner(itemObject);
				}
			}
			else if (agentBuildData.AgentBannerItem != null)
			{
				equipment[EquipmentIndex.Weapon1] = default(EquipmentElement);
				equipment[EquipmentIndex.Weapon2] = default(EquipmentElement);
				equipment[EquipmentIndex.Weapon3] = default(EquipmentElement);
				if (agentBuildData.AgentBannerReplacementWeaponItem != null)
				{
					equipment[EquipmentIndex.WeaponItemBeginSlot] = new EquipmentElement(agentBuildData.AgentBannerReplacementWeaponItem, null, null, false);
				}
				else
				{
					equipment[EquipmentIndex.WeaponItemBeginSlot] = default(EquipmentElement);
				}
				equipment[EquipmentIndex.ExtraWeaponSlot] = new EquipmentElement(agentBuildData.AgentBannerItem, null, null, false);
				if (agentBuildData.AgentOverridenSpawnMissionEquipment != null)
				{
					agentBuildData.AgentOverridenSpawnMissionEquipment[EquipmentIndex.ExtraWeaponSlot] = new MissionWeapon(agentBuildData.AgentBannerItem, null, agentBuildData.AgentBanner);
				}
			}
			if (agentBuildData.AgentNoArmor)
			{
				equipment[EquipmentIndex.Gloves] = default(EquipmentElement);
				equipment[EquipmentIndex.Body] = default(EquipmentElement);
				equipment[EquipmentIndex.Cape] = default(EquipmentElement);
				equipment[EquipmentIndex.NumAllWeaponSlots] = default(EquipmentElement);
				equipment[EquipmentIndex.Leg] = default(EquipmentElement);
			}
			for (int i = 0; i < 5; i++)
			{
				if (!equipment[(EquipmentIndex)i].IsEmpty && equipment[(EquipmentIndex)i].Item.ItemFlags.HasAnyFlag(ItemFlags.CannotBePickedUp))
				{
					equipment[(EquipmentIndex)i] = default(EquipmentElement);
				}
			}
			agent.InitializeSpawnEquipment(equipment);
			agent.InitializeMissionEquipment(agentBuildData.AgentOverridenSpawnMissionEquipment, agentBuildData.AgentBanner);
			if (agent.RandomizeColors)
			{
				agent.Equipment.SetGlossMultipliersOfWeaponsRandomly(agentBuildData.AgentEquipmentSeed);
			}
			ItemObject item2 = equipment[EquipmentIndex.ArmorItemEndSlot].Item;
			if (item2 != null && item2.HasHorseComponent && item2.HorseComponent.IsRideable)
			{
				int num5 = -1;
				if (agentBuildData.AgentMountIndexOverriden)
				{
					num5 = agentBuildData.AgentMountIndex;
				}
				EquipmentElement equipmentElement = equipment[EquipmentIndex.ArmorItemEndSlot];
				EquipmentElement equipmentElement2 = equipment[EquipmentIndex.HorseHarness];
				vec4 = agentBuildData.AgentInitialPosition.GetValueOrDefault();
				vec5 = agentBuildData.AgentInitialDirection.GetValueOrDefault();
				agent3 = this.CreateHorseAgentFromRosterElements(equipmentElement, equipmentElement2, vec4, vec5, num5, agentBuildData.AgentMountKey);
				Equipment equipment2 = new Equipment();
				equipment2[EquipmentIndex.ArmorItemEndSlot] = equipment[EquipmentIndex.ArmorItemEndSlot];
				equipment2[EquipmentIndex.HorseHarness] = equipment[EquipmentIndex.HorseHarness];
				Equipment equipment3 = equipment2;
				agent3.InitializeSpawnEquipment(equipment3);
				agent.SetMountAgentBeforeBuild(agent3);
			}
			if (spawnFromAgentVisuals || !GameNetwork.IsClientOrReplay)
			{
				agent.Equipment.CheckLoadedAmmos();
			}
			if (!agentBuildData.BodyPropertiesOverriden)
			{
				agent.UpdateBodyProperties(agentCharacter.GetBodyProperties(equipment, agentBuildData.AgentEquipmentSeed));
			}
			if (GameNetwork.IsServerOrRecorder && agent.RiderAgent == null)
			{
				Vec3 valueOrDefault = agentBuildData.AgentInitialPosition.GetValueOrDefault();
				Vec2 valueOrDefault2 = agentBuildData.AgentInitialDirection.GetValueOrDefault();
				if (agent.IsMount)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new CreateFreeMountAgent(agent, valueOrDefault, valueOrDefault2));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				else
				{
					bool flag = agentBuildData.AgentMissionPeer != null;
					NetworkCommunicator networkCommunicator;
					if (!flag)
					{
						MissionPeer owningAgentMissionPeer = agentBuildData.OwningAgentMissionPeer;
						networkCommunicator = ((owningAgentMissionPeer != null) ? owningAgentMissionPeer.GetNetworkPeer() : null);
					}
					else
					{
						networkCommunicator = agentBuildData.AgentMissionPeer.GetNetworkPeer();
					}
					NetworkCommunicator networkCommunicator2 = networkCommunicator;
					bool flag2 = agent.MountAgent != null && agent.MountAgent.RiderAgent == agent;
					GameNetwork.BeginBroadcastModuleEvent();
					int index = agent.Index;
					BasicCharacterObject character = agent.Character;
					Monster monster = agent.Monster;
					Equipment spawnEquipment = agent.SpawnEquipment;
					MissionEquipment equipment4 = agent.Equipment;
					BodyProperties bodyPropertiesValue = agent.BodyPropertiesValue;
					int bodyPropertiesSeed = agent.BodyPropertiesSeed;
					bool isFemale = agent.IsFemale;
					Team team = agent.Team;
					int num6 = ((team != null) ? team.TeamIndex : (-1));
					Formation formation = agent.Formation;
					int num7 = ((formation != null) ? formation.Index : (-1));
					uint clothingColor = agent.ClothingColor1;
					uint clothingColor2 = agent.ClothingColor2;
					int num8 = (flag2 ? agent.MountAgent.Index : (-1));
					Agent mountAgent = agent.MountAgent;
					GameNetwork.WriteMessage(new CreateAgent(index, character, monster, spawnEquipment, equipment4, bodyPropertiesValue, bodyPropertiesSeed, isFemale, num6, num7, clothingColor, clothingColor2, num8, (mountAgent != null) ? mountAgent.SpawnEquipment : null, flag, valueOrDefault, valueOrDefault2, networkCommunicator2));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			MultiplayerMissionAgentVisualSpawnComponent missionBehavior = this.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
			if (missionBehavior != null && agentBuildData.AgentMissionPeer != null && agentBuildData.AgentMissionPeer.IsMine && agentBuildData.AgentVisualsIndex == 0)
			{
				missionBehavior.OnMyAgentSpawned();
			}
			if (agent3 != null)
			{
				this.BuildAgent(agent3, agentBuildData);
				foreach (MissionBehavior missionBehavior2 in this.MissionBehaviors)
				{
					missionBehavior2.OnAgentBuild(agent3, null);
				}
			}
			this.BuildAgent(agent, agentBuildData);
			if (agentBuildData.AgentMissionPeer != null)
			{
				agent.MissionPeer = agentBuildData.AgentMissionPeer;
			}
			if (agentBuildData.OwningAgentMissionPeer != null)
			{
				agent.OwningAgentMissionPeer = agentBuildData.OwningAgentMissionPeer;
			}
			foreach (MissionBehavior missionBehavior3 in this.MissionBehaviors)
			{
				Agent agent4 = agent;
				Banner banner;
				if ((banner = agentBuildData.AgentBanner) == null)
				{
					Team agentTeam = agentBuildData.AgentTeam;
					banner = ((agentTeam != null) ? agentTeam.Banner : null);
				}
				missionBehavior3.OnAgentBuild(agent4, banner);
			}
			agent.AgentVisuals.CheckResources(true);
			if (agent.IsAIControlled)
			{
				if (agent3 == null)
				{
					AgentFlag agentFlag = agent.GetAgentFlags() & ~AgentFlag.CanRide;
					agent.SetAgentFlags(agentFlag);
				}
				else if (agent.Formation == null)
				{
					agent.SetRidingOrder(RidingOrder.RidingOrderEnum.Mount);
				}
			}
			return agent;
		}

		public void SetInitialAgentCountForSide(BattleSideEnum side, int agentCount)
		{
			if (side >= BattleSideEnum.Defender && side < BattleSideEnum.NumSides)
			{
				this._initialAgentCountPerSide[(int)side] = agentCount;
				return;
			}
			Debug.FailedAssert("Cannot set initial agent count.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "SetInitialAgentCountForSide", 4062);
		}

		public void SetFormationPositioningFromDeploymentPlan(Formation formation)
		{
			IFormationDeploymentPlan formationPlan = this._deploymentPlan.GetFormationPlan(formation.Team.Side, formation.FormationIndex, DeploymentPlanType.Initial);
			if (formationPlan.PlannedTroopCount > 0 && formationPlan.HasDimensions)
			{
				formation.FormOrder = FormOrder.FormOrderCustom(formationPlan.PlannedWidth);
			}
			formation.SetPositioning(new WorldPosition?(formationPlan.CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.None)), new Vec2?(formationPlan.GetDirection()), null);
		}

		public Agent SpawnMonster(ItemRosterElement rosterElement, ItemRosterElement harnessRosterElement, in Vec3 initialPosition, in Vec2 initialDirection, int forcedAgentIndex = -1)
		{
			return this.SpawnMonster(rosterElement.EquipmentElement, harnessRosterElement.EquipmentElement, initialPosition, initialDirection, forcedAgentIndex);
		}

		public Agent SpawnMonster(EquipmentElement equipmentElement, EquipmentElement harnessRosterElement, in Vec3 initialPosition, in Vec2 initialDirection, int forcedAgentIndex = -1)
		{
			Agent agent = this.CreateHorseAgentFromRosterElements(equipmentElement, harnessRosterElement, initialPosition, initialDirection, forcedAgentIndex, MountCreationKey.GetRandomMountKeyString(equipmentElement.Item, MBRandom.RandomInt()));
			Equipment equipment = new Equipment();
			equipment[EquipmentIndex.ArmorItemEndSlot] = equipmentElement;
			equipment[EquipmentIndex.HorseHarness] = harnessRosterElement;
			Equipment equipment2 = equipment;
			agent.InitializeSpawnEquipment(equipment2);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new CreateFreeMountAgent(agent, initialPosition, initialDirection));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.BuildAgent(agent, null);
			return agent;
		}

		public Agent SpawnTroop(IAgentOriginBase troopOrigin, bool isPlayerSide, bool hasFormation, bool spawnWithHorse, bool isReinforcement, int formationTroopCount, int formationTroopIndex, bool isAlarmed, bool wieldInitialWeapons, bool forceDismounted, Vec3? initialPosition, Vec2? initialDirection, string specialActionSetSuffix = null, ItemObject bannerItem = null, FormationClass formationIndex = FormationClass.NumberOfAllFormations, bool useTroopClassForSpawn = false)
		{
			BasicCharacterObject troop = troopOrigin.Troop;
			Team agentTeam = Mission.GetAgentTeam(troopOrigin, isPlayerSide);
			if (troop.IsPlayerCharacter && !forceDismounted)
			{
				spawnWithHorse = true;
			}
			AgentBuildData agentBuildData = new AgentBuildData(troop).Team(agentTeam).Banner(troopOrigin.Banner).ClothingColor1(agentTeam.Color)
				.ClothingColor2(agentTeam.Color2)
				.TroopOrigin(troopOrigin)
				.NoHorses(!spawnWithHorse)
				.CivilianEquipment(this.DoesMissionRequireCivilianEquipment)
				.SpawnsUsingOwnTroopClass(useTroopClassForSpawn);
			if (hasFormation)
			{
				Formation formation;
				if (formationIndex == FormationClass.NumberOfAllFormations)
				{
					formation = agentTeam.GetFormation(this.GetAgentTroopClass(agentTeam.Side, troop));
				}
				else
				{
					formation = agentTeam.GetFormation(formationIndex);
				}
				agentBuildData.Formation(formation);
				agentBuildData.FormationTroopSpawnCount(formationTroopCount).FormationTroopSpawnIndex(formationTroopIndex);
			}
			if (!troop.IsPlayerCharacter)
			{
				agentBuildData.IsReinforcement(isReinforcement);
			}
			if (bannerItem != null)
			{
				if (bannerItem.IsBannerItem && bannerItem.BannerComponent != null)
				{
					agentBuildData.BannerItem(bannerItem);
					ItemObject bannerBearerReplacementWeapon = MissionGameModels.Current.BattleBannerBearersModel.GetBannerBearerReplacementWeapon(troop);
					agentBuildData.BannerReplacementWeaponItem(bannerBearerReplacementWeapon);
				}
				else
				{
					Debug.FailedAssert("Passed banner item with name: " + bannerItem.Name + " is not a proper banner item", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "SpawnTroop", 4169);
					Debug.Print("Invalid banner item: " + bannerItem.Name + " is passed to a troop to be spawned", 0, Debug.DebugColor.Yellow, 17592186044416UL);
				}
			}
			if (initialPosition != null)
			{
				AgentBuildData agentBuildData2 = agentBuildData;
				Vec3 value = initialPosition.Value;
				agentBuildData2.InitialPosition(value);
				AgentBuildData agentBuildData3 = agentBuildData;
				Vec2 value2 = initialDirection.Value;
				agentBuildData3.InitialDirection(value2);
			}
			if (spawnWithHorse)
			{
				agentBuildData.MountKey(MountCreationKey.GetRandomMountKeyString(troop.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, troop.GetMountKeySeed()));
			}
			if (isPlayerSide && troop == Game.Current.PlayerTroop)
			{
				agentBuildData.Controller(Agent.ControllerType.Player);
			}
			Agent agent = this.SpawnAgent(agentBuildData, false);
			if (agent.Character.IsHero)
			{
				agent.SetAgentFlags(agent.GetAgentFlags() | AgentFlag.IsUnique);
			}
			if (agent.IsAIControlled && isAlarmed)
			{
				agent.SetWatchState(Agent.WatchState.Alarmed);
			}
			if (wieldInitialWeapons)
			{
				agent.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp, Equipment.InitialWeaponEquipPreference.Any);
			}
			if (!string.IsNullOrEmpty(specialActionSetSuffix))
			{
				AnimationSystemData animationSystemData = agentBuildData.AgentMonster.FillAnimationSystemData(MBGlobals.GetActionSetWithSuffix(agentBuildData.AgentMonster, agentBuildData.AgentIsFemale, specialActionSetSuffix), agent.Character.GetStepSize(), false);
				agent.SetActionSet(ref animationSystemData);
			}
			return agent;
		}

		public Agent ReplaceBotWithPlayer(Agent botAgent, MissionPeer missionPeer)
		{
			if (!GameNetwork.IsClientOrReplay && botAgent != null)
			{
				if (GameNetwork.IsServer)
				{
					NetworkCommunicator networkPeer = missionPeer.GetNetworkPeer();
					if (!networkPeer.IsServerPeer)
					{
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						NetworkCommunicator networkCommunicator = networkPeer;
						int index = botAgent.Index;
						float health = botAgent.Health;
						Agent mountAgent = botAgent.MountAgent;
						GameNetwork.WriteMessage(new ReplaceBotWithPlayer(networkCommunicator, index, health, (mountAgent != null) ? mountAgent.Health : (-1f)));
						GameNetwork.EndModuleEventAsServer();
					}
				}
				if (botAgent.Formation != null)
				{
					botAgent.Formation.PlayerOwner = botAgent;
				}
				botAgent.OwningAgentMissionPeer = null;
				botAgent.MissionPeer = missionPeer;
				botAgent.Formation = missionPeer.ControlledFormation;
				AgentFlag agentFlags = botAgent.GetAgentFlags();
				if (!agentFlags.HasAnyFlag(AgentFlag.CanRide))
				{
					botAgent.SetAgentFlags(agentFlags | AgentFlag.CanRide);
				}
				int botsUnderControlAlive = missionPeer.BotsUnderControlAlive;
				missionPeer.BotsUnderControlAlive = botsUnderControlAlive - 1;
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new BotsControlledChange(missionPeer.GetNetworkPeer(), missionPeer.BotsUnderControlAlive, missionPeer.BotsUnderControlTotal));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				if (botAgent.Formation != null)
				{
					missionPeer.Team.AssignPlayerAsSergeantOfFormation(missionPeer, missionPeer.ControlledFormation.FormationIndex);
				}
				return botAgent;
			}
			return null;
		}

		private Agent CreateHorseAgentFromRosterElements(EquipmentElement mount, EquipmentElement mountHarness, in Vec3 initialPosition, in Vec2 initialDirection, int forcedAgentMountIndex, string horseCreationKey)
		{
			HorseComponent horseComponent = mount.Item.HorseComponent;
			Agent agent = this.CreateAgent(horseComponent.Monster, false, 0, Agent.CreationType.FromHorseObj, 1f, forcedAgentMountIndex, (int)mount.Weight, null);
			agent.SetInitialFrame(initialPosition, initialDirection, false);
			agent.BaseHealthLimit = (float)mount.GetModifiedMountHitPoints();
			agent.HealthLimit = agent.BaseHealthLimit;
			agent.Health = agent.HealthLimit;
			agent.SetMountInitialValues(mount.GetModifiedItemName(), horseCreationKey);
			return agent;
		}

		public void OnAgentInteraction(Agent requesterAgent, Agent targetAgent)
		{
			if (requesterAgent == Agent.Main && targetAgent.IsMount)
			{
				Agent.Main.Mount(targetAgent);
				return;
			}
			if (targetAgent.IsHuman)
			{
				foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
				{
					missionBehavior.OnAgentInteraction(requesterAgent, targetAgent);
				}
			}
		}

		[UsedImplicitly]
		[MBCallback]
		public void EndMission()
		{
			Debug.Print("I called EndMission", 0, Debug.DebugColor.White, 17179869184UL);
			this._missionEndTime = -1f;
			this.NextCheckTimeEndMission = -1f;
			this.MissionEnded = true;
			this.CurrentState = Mission.State.EndingNextFrame;
		}

		private void EndMissionInternal()
		{
			MBDebug.Print("I called EndMissionInternal", 0, Debug.DebugColor.White, 17179869184UL);
			this._deploymentPlan.ClearAll();
			IMissionListener[] array = this._listeners.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnEndMission();
			}
			this.StopSoundEvents();
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnEndMissionInternal();
			}
			foreach (Agent agent in this.Agents)
			{
				agent.OnRemove();
			}
			foreach (Agent agent2 in this.AllAgents)
			{
				agent2.OnDelete();
				agent2.Clear();
			}
			this.Teams.Clear();
			foreach (MissionObject missionObject in this.MissionObjects)
			{
				missionObject.OnEndMission();
			}
			this.CurrentState = Mission.State.Over;
			this.FreeResources();
			this.FinalizeMission();
		}

		private void StopSoundEvents()
		{
			if (this._ambientSoundEvent != null)
			{
				this._ambientSoundEvent.Stop();
			}
		}

		public void AddMissionBehavior(MissionBehavior missionBehavior)
		{
			this.MissionBehaviors.Add(missionBehavior);
			missionBehavior.Mission = this;
			MissionBehaviorType behaviorType = missionBehavior.BehaviorType;
			if (behaviorType != MissionBehaviorType.Logic)
			{
				if (behaviorType == MissionBehaviorType.Other)
				{
					this._otherMissionBehaviors.Add(missionBehavior);
				}
			}
			else
			{
				this.MissionLogics.Add(missionBehavior as MissionLogic);
			}
			missionBehavior.OnCreated();
		}

		public T GetMissionBehavior<T>() where T : class, IMissionBehavior
		{
			using (List<MissionBehavior>.Enumerator enumerator = this.MissionBehaviors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T t;
					if ((t = enumerator.Current as T) != null)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		public void RemoveMissionBehavior(MissionBehavior missionBehavior)
		{
			missionBehavior.OnRemoveBehavior();
			MissionBehaviorType behaviorType = missionBehavior.BehaviorType;
			if (behaviorType != MissionBehaviorType.Logic)
			{
				if (behaviorType != MissionBehaviorType.Other)
				{
					Debug.FailedAssert("Invalid behavior type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "RemoveMissionBehavior", 4463);
				}
				else
				{
					this._otherMissionBehaviors.Remove(missionBehavior);
				}
			}
			else
			{
				this.MissionLogics.Remove(missionBehavior as MissionLogic);
			}
			this.MissionBehaviors.Remove(missionBehavior);
			missionBehavior.Mission = null;
		}

		public void JoinEnemyTeam()
		{
			if (this.PlayerTeam == this.DefenderTeam)
			{
				Agent leader = this.AttackerTeam.Leader;
				if (leader != null)
				{
					if (this.MainAgent != null && this.MainAgent.IsActive())
					{
						this.MainAgent.Controller = Agent.ControllerType.AI;
					}
					leader.Controller = Agent.ControllerType.Player;
					this.PlayerTeam = this.AttackerTeam;
					return;
				}
			}
			else if (this.PlayerTeam == this.AttackerTeam)
			{
				Agent leader2 = this.DefenderTeam.Leader;
				if (leader2 != null)
				{
					if (this.MainAgent != null && this.MainAgent.IsActive())
					{
						this.MainAgent.Controller = Agent.ControllerType.AI;
					}
					leader2.Controller = Agent.ControllerType.Player;
					this.PlayerTeam = this.DefenderTeam;
					return;
				}
			}
			else
			{
				Debug.FailedAssert("Player is neither attacker nor defender.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "JoinEnemyTeam", 4507);
			}
		}

		public void OnEndMissionResult()
		{
			MissionLogic[] array = this.MissionLogics.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnBattleEnded();
			}
			this.RetreatMission();
		}

		public bool IsAgentInteractionAllowed()
		{
			if (this.IsAgentInteractionAllowed_AdditionalCondition != null)
			{
				Delegate[] invocationList = this.IsAgentInteractionAllowed_AdditionalCondition.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					object obj;
					if ((obj = invocationList[i].DynamicInvoke(Array.Empty<object>())) is bool && !(bool)obj)
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool IsOrderGesturesEnabled()
		{
			if (this.AreOrderGesturesEnabled_AdditionalCondition != null)
			{
				Delegate[] invocationList = this.AreOrderGesturesEnabled_AdditionalCondition.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					object obj;
					if ((obj = invocationList[i].DynamicInvoke(Array.Empty<object>())) is bool && !(bool)obj)
					{
						return false;
					}
				}
			}
			return true;
		}

		public List<EquipmentElement> GetExtraEquipmentElementsForCharacter(BasicCharacterObject character, bool getAllEquipments = false)
		{
			List<EquipmentElement> list = new List<EquipmentElement>();
			foreach (MissionLogic missionLogic in this.MissionLogics)
			{
				List<EquipmentElement> extraEquipmentElementsForCharacter = missionLogic.GetExtraEquipmentElementsForCharacter(character, getAllEquipments);
				if (extraEquipmentElementsForCharacter != null)
				{
					list.AddRange(extraEquipmentElementsForCharacter);
				}
			}
			return list;
		}

		private bool CheckMissionEnded()
		{
			foreach (MissionLogic missionLogic in this.MissionLogics)
			{
				MissionResult missionResult = null;
				if (missionLogic.MissionEnded(ref missionResult))
				{
					Debug.Print("CheckMissionEnded::ended", 0, Debug.DebugColor.White, 17592186044416UL);
					this.MissionResult = missionResult;
					this.MissionEnded = true;
					this.MissionResultReady(missionResult);
					return true;
				}
			}
			return false;
		}

		private void MissionResultReady(MissionResult missionResult)
		{
			foreach (MissionLogic missionLogic in this.MissionLogics)
			{
				missionLogic.OnMissionResultReady(missionResult);
			}
		}

		private void CheckMissionEnd(float currentTime)
		{
			if (!GameNetwork.IsClient && currentTime > this.NextCheckTimeEndMission)
			{
				if (this.CurrentState == Mission.State.Continuing)
				{
					if (this.MissionEnded)
					{
						return;
					}
					this.NextCheckTimeEndMission += 0.1f;
					this.CheckMissionEnded();
					if (!this.MissionEnded)
					{
						return;
					}
					this._missionEndTime = currentTime + this.MissionCloseTimeAfterFinish;
					this.NextCheckTimeEndMission += 5f;
					using (List<MissionLogic>.Enumerator enumerator = this.MissionLogics.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MissionLogic missionLogic = enumerator.Current;
							missionLogic.ShowBattleResults();
						}
						return;
					}
				}
				if (currentTime > this._missionEndTime)
				{
					this.EndMissionInternal();
					return;
				}
				this.NextCheckTimeEndMission += 5f;
				return;
			}
			else if (this.CurrentState != Mission.State.Continuing && currentTime > this.NextCheckTimeEndMission)
			{
				this.EndMissionInternal();
			}
		}

		public bool IsPlayerCloseToAnEnemy(float distance = 5f)
		{
			if (this.MainAgent == null)
			{
				return false;
			}
			Vec3 position = this.MainAgent.Position;
			float num = distance * distance;
			AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(this, position.AsVec2, distance, false);
			while (proximityMapSearchStruct.LastFoundAgent != null)
			{
				Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
				if (lastFoundAgent != this.MainAgent && lastFoundAgent.GetAgentFlags().HasAnyFlag(AgentFlag.CanAttack) && lastFoundAgent.Position.DistanceSquared(position) <= num && (!lastFoundAgent.IsAIControlled || lastFoundAgent.CurrentWatchState == Agent.WatchState.Alarmed) && lastFoundAgent.IsEnemyOf(this.MainAgent) && !lastFoundAgent.IsRetreating())
				{
					return true;
				}
				AgentProximityMap.FindNext(this, ref proximityMapSearchStruct);
			}
			return false;
		}

		public Vec3 GetRandomPositionAroundPoint(Vec3 center, float minDistance, float maxDistance, bool nearFirst = false)
		{
			Vec3 vec = new Vec3(-1f, 0f, 0f, -1f);
			vec.RotateAboutZ(6.2831855f * MBRandom.RandomFloat);
			float num = maxDistance - minDistance;
			if (nearFirst)
			{
				for (int i = 4; i > 0; i--)
				{
					int num2 = 0;
					while ((float)num2 <= 10f)
					{
						vec.RotateAboutZ(1.2566371f);
						Vec3 vec2 = center + vec * (minDistance + num / (float)i);
						if (this.Scene.GetNavigationMeshForPosition(ref vec2))
						{
							return vec2;
						}
						num2++;
					}
				}
			}
			else
			{
				for (int j = 1; j < 5; j++)
				{
					int num3 = 0;
					while ((float)num3 <= 10f)
					{
						vec.RotateAboutZ(1.2566371f);
						Vec3 vec3 = center + vec * (minDistance + num / (float)j);
						if (this.Scene.GetNavigationMeshForPosition(ref vec3))
						{
							return vec3;
						}
						num3++;
					}
				}
			}
			return center;
		}

		public WorldPosition FindBestDefendingPosition(WorldPosition enemyPosition, WorldPosition defendedPosition)
		{
			return this.GetBestSlopeAngleHeightPosForDefending(enemyPosition, defendedPosition, 10, 0.5f, 4f, 0.5f, 0.70710677f, 0.1f, 1f, 0.7f, 0.5f, 1.2f, 20f, 0.6f);
		}

		public WorldPosition FindPositionWithBiggestSlopeTowardsDirectionInSquare(ref WorldPosition center, float halfSize, ref WorldPosition referencePosition)
		{
			return this.GetBestSlopeTowardsDirection(ref center, halfSize, ref referencePosition);
		}

		public void AddCustomMissile(Agent shooterAgent, MissionWeapon missileWeapon, Vec3 position, Vec3 direction, Mat3 orientation, float baseSpeed, float speed, bool addRigidBody, MissionObject missionObjectToIgnore, int forcedMissileIndex = -1)
		{
			WeaponData weaponData = missileWeapon.GetWeaponData(true);
			GameEntity gameEntity;
			int num;
			if (missileWeapon.WeaponsCount == 1)
			{
				WeaponStatsData weaponStatsDataForUsage = missileWeapon.GetWeaponStatsDataForUsage(0);
				num = this.AddMissileSingleUsageAux(forcedMissileIndex, false, shooterAgent, weaponData, weaponStatsDataForUsage, 0f, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, (missionObjectToIgnore != null) ? missionObjectToIgnore.GameEntity : null, false, out gameEntity);
			}
			else
			{
				WeaponStatsData[] weaponStatsData = missileWeapon.GetWeaponStatsData();
				num = this.AddMissileAux(forcedMissileIndex, false, shooterAgent, weaponData, weaponStatsData, 0f, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, (missionObjectToIgnore != null) ? missionObjectToIgnore.GameEntity : null, false, out gameEntity);
			}
			weaponData.DeinitializeManagedPointers();
			Mission.Missile missile = new Mission.Missile(this, gameEntity)
			{
				ShooterAgent = shooterAgent,
				Weapon = missileWeapon,
				MissionObjectToIgnore = missionObjectToIgnore,
				Index = num
			};
			this._missiles.Add(num, missile);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new CreateMissile(num, shooterAgent.Index, EquipmentIndex.None, missileWeapon, position, direction, speed, orientation, addRigidBody, missionObjectToIgnore.Id, false));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		[UsedImplicitly]
		[MBCallback]
		internal void OnAgentShootMissile(Agent shooterAgent, EquipmentIndex weaponIndex, Vec3 position, Vec3 velocity, Mat3 orientation, bool hasRigidBody, bool isPrimaryWeaponShot, int forcedMissileIndex)
		{
			float num = 0f;
			MissionWeapon missionWeapon;
			if (shooterAgent.Equipment[weaponIndex].CurrentUsageItem.IsRangedWeapon && shooterAgent.Equipment[weaponIndex].CurrentUsageItem.IsConsumable)
			{
				missionWeapon = shooterAgent.Equipment[weaponIndex];
			}
			else
			{
				missionWeapon = shooterAgent.Equipment[weaponIndex].AmmoWeapon;
				if (shooterAgent.Equipment[weaponIndex].CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.HasString))
				{
					num = (float)shooterAgent.Equipment[weaponIndex].GetModifiedThrustDamageForCurrentUsage();
				}
			}
			missionWeapon.Amount = 1;
			WeaponData weaponData = missionWeapon.GetWeaponData(true);
			Vec3 vec = velocity;
			float num2 = vec.Normalize();
			bool flag = GameNetwork.IsClient && forcedMissileIndex == -1;
			float num3 = (float)shooterAgent.Equipment[shooterAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)].GetModifiedMissileSpeedForCurrentUsage();
			GameEntity gameEntity;
			int num4;
			if (missionWeapon.WeaponsCount == 1)
			{
				WeaponStatsData weaponStatsDataForUsage = missionWeapon.GetWeaponStatsDataForUsage(0);
				num4 = this.AddMissileSingleUsageAux(forcedMissileIndex, flag, shooterAgent, weaponData, weaponStatsDataForUsage, num, ref position, ref vec, ref orientation, num3, num2, hasRigidBody, null, isPrimaryWeaponShot, out gameEntity);
			}
			else
			{
				WeaponStatsData[] weaponStatsData = missionWeapon.GetWeaponStatsData();
				num4 = this.AddMissileAux(forcedMissileIndex, flag, shooterAgent, weaponData, weaponStatsData, num, ref position, ref vec, ref orientation, num3, num2, hasRigidBody, null, isPrimaryWeaponShot, out gameEntity);
			}
			weaponData.DeinitializeManagedPointers();
			if (!flag)
			{
				Mission.Missile missile = new Mission.Missile(this, gameEntity)
				{
					ShooterAgent = shooterAgent,
					Weapon = missionWeapon,
					Index = num4
				};
				gameEntity.ManualInvalidate();
				this._missiles.Add(num4, missile);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new CreateMissile(num4, shooterAgent.Index, weaponIndex, MissionWeapon.Invalid, position, vec, num2, orientation, hasRigidBody, MissionObjectId.Invalid, isPrimaryWeaponShot));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnAgentShootMissile(shooterAgent, weaponIndex, position, velocity, orientation, hasRigidBody, forcedMissileIndex);
			}
			if (shooterAgent != null)
			{
				shooterAgent.UpdateLastRangedAttackTimeDueToAnAttack(MBCommon.GetTotalMissionTime());
			}
		}

		[UsedImplicitly]
		[MBCallback]
		internal AgentState GetAgentState(Agent affectorAgent, Agent agent, DamageTypes damageType, WeaponFlags weaponFlags)
		{
			float num;
			float agentStateProbability = MissionGameModels.Current.AgentDecideKilledOrUnconsciousModel.GetAgentStateProbability(affectorAgent, agent, damageType, weaponFlags, out num);
			AgentState agentState = AgentState.None;
			bool flag = false;
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				if (missionBehavior is IAgentStateDecider)
				{
					agentState = (missionBehavior as IAgentStateDecider).GetAgentState(agent, agentStateProbability, out flag);
					break;
				}
			}
			if (agentState == AgentState.None)
			{
				float randomFloat = MBRandom.RandomFloat;
				if (randomFloat < agentStateProbability)
				{
					agentState = AgentState.Killed;
					flag = true;
				}
				else
				{
					agentState = AgentState.Unconscious;
					if (randomFloat > 1f - num)
					{
						flag = true;
					}
				}
			}
			if (flag && affectorAgent != null && affectorAgent.Team != null && agent.Team != null && affectorAgent.Team == agent.Team)
			{
				flag = false;
			}
			for (int i = 0; i < this.MissionBehaviors.Count; i++)
			{
				this.MissionBehaviors[i].OnGetAgentState(agent, flag);
			}
			return agentState;
		}

		public void OnAgentMount(Agent agent)
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnAgentMount(agent);
			}
		}

		public void OnAgentDismount(Agent agent)
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnAgentDismount(agent);
			}
		}

		public void OnObjectUsed(Agent userAgent, UsableMissionObject usableGameObject)
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnObjectUsed(userAgent, usableGameObject);
			}
		}

		public void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usableGameObject)
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnObjectStoppedBeingUsed(userAgent, usableGameObject);
			}
		}

		public void InitializeStartingBehaviors(MissionLogic[] logicBehaviors, MissionBehavior[] otherBehaviors, MissionNetwork[] networkBehaviors)
		{
			foreach (MissionLogic missionLogic in logicBehaviors)
			{
				this.AddMissionBehavior(missionLogic);
			}
			foreach (MissionNetwork missionNetwork in networkBehaviors)
			{
				this.AddMissionBehavior(missionNetwork);
			}
			foreach (MissionBehavior missionBehavior in otherBehaviors)
			{
				this.AddMissionBehavior(missionBehavior);
			}
		}

		public Agent GetClosestEnemyAgent(Team team, Vec3 position, float radius)
		{
			return this.GetClosestEnemyAgent(team.MBTeam, position, radius);
		}

		public Agent GetClosestAllyAgent(Team team, Vec3 position, float radius)
		{
			return this.GetClosestAllyAgent(team.MBTeam, position, radius);
		}

		public int GetNearbyEnemyAgentCount(Team team, Vec2 position, float radius)
		{
			return this.GetNearbyEnemyAgentCount(team.MBTeam, position, radius);
		}

		public bool HasAnyAgentsOfSideInRange(Vec3 origin, float radius, BattleSideEnum side)
		{
			Team team = ((side == BattleSideEnum.Attacker) ? this.AttackerTeam : this.DefenderTeam);
			return MBAPI.IMBMission.HasAnyAgentsOfTeamAround(this.Pointer, origin, radius, team.MBTeam.Index);
		}

		private void HandleSpawnedItems()
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				int num = 0;
				for (int i = this._spawnedItemEntitiesCreatedAtRuntime.Count - 1; i >= 0; i--)
				{
					SpawnedItemEntity spawnedItemEntity = this._spawnedItemEntitiesCreatedAtRuntime[i];
					if (!spawnedItemEntity.IsRemoved)
					{
						if (!spawnedItemEntity.IsDeactivated && !spawnedItemEntity.HasUser && spawnedItemEntity.HasLifeTime && !spawnedItemEntity.HasAIMovingTo && (num > 500 || spawnedItemEntity.IsReadyToBeDeleted()))
						{
							spawnedItemEntity.GameEntity.Remove(80);
						}
						else
						{
							num++;
						}
					}
					if (spawnedItemEntity.IsRemoved)
					{
						this._spawnedItemEntitiesCreatedAtRuntime.RemoveAt(i);
					}
				}
			}
		}

		public bool OnMissionObjectRemoved(MissionObject missionObject, int removeReason)
		{
			if (!GameNetwork.IsClientOrReplay && missionObject.CreatedAtRuntime)
			{
				this.ReturnRuntimeMissionObjectId(missionObject.Id.Id);
				if (GameNetwork.IsServerOrRecorder)
				{
					this.RemoveDynamicallySpawnedMissionObjectInfo(missionObject.Id);
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new RemoveMissionObject(missionObject.Id));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			this._activeMissionObjects.Remove(missionObject);
			return this._missionObjects.Remove(missionObject);
		}

		public bool AgentLookingAtAgent(Agent agent1, Agent agent2)
		{
			Vec3 vec = agent2.Position - agent1.Position;
			float num = vec.Normalize();
			float num2 = Vec3.DotProduct(vec, agent1.LookDirection);
			return num2 < 1f && num2 > 0.86f && num < 4f;
		}

		public Agent FindAgentWithIndex(int agentId)
		{
			return this.FindAgentWithIndexAux(agentId);
		}

		public static Agent.UnderAttackType GetUnderAttackTypeOfAgents(IEnumerable<Agent> agents, float timeLimit = 3f)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			timeLimit += MBCommon.GetTotalMissionTime();
			foreach (Agent agent in agents)
			{
				num = MathF.Max(num, agent.LastMeleeHitTime);
				num2 = MathF.Max(num2, agent.LastRangedHitTime);
				if (num2 >= 0f && num2 < timeLimit)
				{
					return Agent.UnderAttackType.UnderRangedAttack;
				}
				if (num >= 0f && num < timeLimit)
				{
					return Agent.UnderAttackType.UnderMeleeAttack;
				}
			}
			return Agent.UnderAttackType.NotUnderAttack;
		}

		public static Team GetAgentTeam(IAgentOriginBase troopOrigin, bool isPlayerSide)
		{
			if (Mission.Current == null)
			{
				Debug.FailedAssert("Mission current is null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "GetAgentTeam", 5198);
				return null;
			}
			Team team;
			if (troopOrigin.IsUnderPlayersCommand)
			{
				team = Mission.Current.PlayerTeam;
			}
			else if (isPlayerSide)
			{
				if (Mission.Current.PlayerAllyTeam != null)
				{
					team = Mission.Current.PlayerAllyTeam;
				}
				else
				{
					team = Mission.Current.PlayerTeam;
				}
			}
			else
			{
				team = Mission.Current.PlayerEnemyTeam;
			}
			return team;
		}

		public static float GetBattleSizeOffset(int battleSize, Path path)
		{
			if (path != null && path.NumberOfPoints > 1)
			{
				float totalLength = path.GetTotalLength();
				float num = 800f / totalLength;
				float battleSizeFactor = Mission.GetBattleSizeFactor(battleSize, num);
				return -(0.44f * battleSizeFactor);
			}
			return 0f;
		}

		public void OnRenderingStarted()
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnRenderingStarted();
			}
		}

		public static float GetBattleSizeFactor(int battleSize, float normalizationFactor)
		{
			float num = -1f;
			if (battleSize > 0)
			{
				num = 0.04f + 0.08f * MathF.Pow((float)battleSize, 0.4f);
				num *= normalizationFactor;
			}
			return MBMath.ClampFloat(num, 0.15f, 1f);
		}

		public unsafe Agent.MovementBehaviorType GetMovementTypeOfAgents(IEnumerable<Agent> agents)
		{
			float totalMissionTime = MBCommon.GetTotalMissionTime();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (Agent agent in agents)
			{
				num++;
				if (agent.IsAIControlled)
				{
					if (!agent.IsRetreating())
					{
						if (agent.Formation == null)
						{
							goto IL_60;
						}
						MovementOrder movementOrder = *agent.Formation.GetReadonlyMovementOrderReference();
						if (movementOrder.OrderType != OrderType.Retreat)
						{
							goto IL_60;
						}
					}
					num2++;
				}
				IL_60:
				if (totalMissionTime - agent.LastMeleeAttackTime < 3f)
				{
					num3++;
				}
			}
			if ((float)num2 * 1f / (float)num > 0.3f)
			{
				return Agent.MovementBehaviorType.Flee;
			}
			if (num3 > 0)
			{
				return Agent.MovementBehaviorType.Engaged;
			}
			return Agent.MovementBehaviorType.Idle;
		}

		public void ShowInMissionLoadingScreen(int durationInSecond, Action onLoadingEndedAction)
		{
			this._inMissionLoadingScreenTimer = new Timer(this.CurrentTime, (float)durationInSecond, true);
			this._onLoadingEndedAction = onLoadingEndedAction;
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		public bool CanAgentRout(Agent agent)
		{
			return (agent.IsRunningAway || (agent.CommonAIComponent != null && agent.CommonAIComponent.IsRetreating)) && agent.RiderAgent == null && (this.CanAgentRout_AdditionalCondition == null || this.CanAgentRout_AdditionalCondition(agent));
		}

		internal bool CanGiveDamageToAgentShield(Agent attacker, WeaponComponentData attackerWeapon, Agent defender)
		{
			return MissionGameModels.Current.AgentApplyDamageModel.CanWeaponIgnoreFriendlyFireChecks(attackerWeapon) || !this.CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(attacker, defender);
		}

		[UsedImplicitly]
		[MBCallback]
		internal void MeleeHitCallback(ref AttackCollisionData collisionData, Agent attacker, Agent victim, GameEntity realHitEntity, ref float inOutMomentumRemaining, ref MeleeCollisionReaction colReaction, CrushThroughState crushThroughState, Vec3 blowDir, Vec3 swingDir, ref HitParticleResultData hitParticleResultData, bool crushedThroughWithoutAgentCollision)
		{
			hitParticleResultData.Reset();
			bool flag = collisionData.CollisionResult == CombatCollisionResult.Parried || collisionData.CollisionResult == CombatCollisionResult.Blocked || collisionData.CollisionResult == CombatCollisionResult.ChamberBlocked;
			if (collisionData.IsAlternativeAttack && !flag && victim != null && victim.IsHuman && collisionData.CollisionBoneIndex != -1 && (collisionData.VictimHitBodyPart == BoneBodyPartType.ArmLeft || collisionData.VictimHitBodyPart == BoneBodyPartType.ArmRight) && victim.IsHuman)
			{
				colReaction = MeleeCollisionReaction.ContinueChecking;
			}
			if (colReaction != MeleeCollisionReaction.ContinueChecking)
			{
				bool flag2 = this.CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(attacker, victim);
				bool flag3 = victim != null && victim.CurrentMortalityState == Agent.MortalityState.Invulnerable;
				bool flag4;
				if (flag2)
				{
					collisionData.AttackerStunPeriod = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerFriendlyFire);
					flag4 = true;
				}
				else
				{
					flag4 = flag3 || (flag && !collisionData.AttackBlockedWithShield);
				}
				int affectorWeaponSlotOrMissileIndex = collisionData.AffectorWeaponSlotOrMissileIndex;
				MissionWeapon missionWeapon = ((affectorWeaponSlotOrMissileIndex >= 0) ? attacker.Equipment[affectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid);
				if (crushThroughState == CrushThroughState.CrushedThisFrame && !collisionData.IsAlternativeAttack)
				{
					this.UpdateMomentumRemaining(ref inOutMomentumRemaining, default(Blow), collisionData, attacker, victim, missionWeapon, true);
				}
				WeaponComponentData weaponComponentData = null;
				CombatLogData combatLogData = default(CombatLogData);
				if (!flag4)
				{
					this.GetAttackCollisionResults(attacker, victim, realHitEntity, inOutMomentumRemaining, missionWeapon, crushThroughState > CrushThroughState.None, flag4, crushedThroughWithoutAgentCollision, ref collisionData, out weaponComponentData, out combatLogData);
					if (!collisionData.IsAlternativeAttack && attacker.IsDoingPassiveAttack && !GameNetwork.IsSessionActive && ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.ReportDamage) > 0f)
					{
						if (attacker.HasMount)
						{
							if (attacker.IsMainAgent)
							{
								InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_delivered_couched_lance_damage", null).ToString(), Color.ConvertStringToColor("#AE4AD9FF")));
							}
							else if (victim != null && victim.IsMainAgent)
							{
								InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_received_couched_lance_damage", null).ToString(), Color.ConvertStringToColor("#D65252FF")));
							}
						}
						else if (attacker.IsMainAgent)
						{
							InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_delivered_braced_polearm_damage", null).ToString(), Color.ConvertStringToColor("#AE4AD9FF")));
						}
						else if (victim != null && victim.IsMainAgent)
						{
							InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_received_braced_polearm_damage", null).ToString(), Color.ConvertStringToColor("#D65252FF")));
						}
					}
					if (collisionData.CollidedWithShieldOnBack && weaponComponentData != null && victim != null && victim.IsMainAgent)
					{
						InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back", null).ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
					}
				}
				else
				{
					collisionData.InflictedDamage = 0;
					collisionData.BaseMagnitude = 0f;
					collisionData.AbsorbedByArmor = 0;
					collisionData.SelfInflictedDamage = 0;
				}
				if (!crushedThroughWithoutAgentCollision)
				{
					Blow blow = this.CreateMeleeBlow(attacker, victim, collisionData, missionWeapon, crushThroughState, blowDir, swingDir, flag4);
					if (!flag && ((victim != null && victim.IsActive()) || realHitEntity != null))
					{
						this.RegisterBlow(attacker, victim, realHitEntity, blow, ref collisionData, missionWeapon, ref combatLogData);
					}
					this.UpdateMomentumRemaining(ref inOutMomentumRemaining, blow, collisionData, attacker, victim, missionWeapon, false);
					bool flag5 = victim != null && victim.Health <= 0f;
					bool flag6 = (blow.BlowFlag & BlowFlags.ShrugOff) > BlowFlags.None;
					this.DecideAgentHitParticles(attacker, victim, blow, collisionData, ref hitParticleResultData);
					this.DecideWeaponCollisionReaction(blow, collisionData, attacker, victim, missionWeapon, flag5, flag6, out colReaction);
				}
				else
				{
					colReaction = MeleeCollisionReaction.ContinueChecking;
				}
				foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
				{
					missionBehavior.OnMeleeHit(attacker, victim, flag4, collisionData);
				}
			}
		}

		private bool HitWithAnotherBone(in AttackCollisionData collisionData, Agent attacker, in MissionWeapon attackerWeapon)
		{
			MissionWeapon missionWeapon = attackerWeapon;
			int num;
			if (missionWeapon.IsEmpty || attacker == null || !attacker.IsHuman)
			{
				num = -1;
			}
			else
			{
				Monster monster = attacker.Monster;
				missionWeapon = attackerWeapon;
				num = (int)monster.GetBoneToAttachForItemFlags(missionWeapon.Item.ItemFlags);
			}
			int num2 = num;
			return MissionCombatMechanicsHelper.IsCollisionBoneDifferentThanWeaponAttachBone(collisionData, num2);
		}

		private void DecideAgentHitParticles(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData, ref HitParticleResultData hprd)
		{
			if (victim != null && (blow.InflictedDamage > 0 || victim.Health <= 0f))
			{
				BlowWeaponRecord weaponRecord = blow.WeaponRecord;
				bool flag;
				if (weaponRecord.HasWeapon() && !blow.WeaponRecord.WeaponFlags.HasAnyFlag(WeaponFlags.NoBlood))
				{
					AttackCollisionData attackCollisionData = collisionData;
					flag = attackCollisionData.IsAlternativeAttack;
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					MissionGameModels.Current.DamageParticleModel.GetMeleeAttackSweatParticles(attacker, victim, blow, collisionData, out hprd);
					return;
				}
				MissionGameModels.Current.DamageParticleModel.GetMeleeAttackBloodParticles(attacker, victim, blow, collisionData, out hprd);
			}
		}

		private void DecideWeaponCollisionReaction(Blow registeredBlow, in AttackCollisionData collisionData, Agent attacker, Agent defender, in MissionWeapon attackerWeapon, bool isFatalHit, bool isShruggedOff, out MeleeCollisionReaction colReaction)
		{
			AttackCollisionData attackCollisionData = collisionData;
			if (attackCollisionData.IsColliderAgent)
			{
				attackCollisionData = collisionData;
				if (attackCollisionData.StrikeType == 1)
				{
					attackCollisionData = collisionData;
					if (attackCollisionData.CollisionHitResultFlags.HasAnyFlag(CombatHitResultFlags.HitWithStartOfTheAnimation))
					{
						colReaction = MeleeCollisionReaction.Staggered;
						return;
					}
				}
			}
			attackCollisionData = collisionData;
			if (!attackCollisionData.IsColliderAgent)
			{
				attackCollisionData = collisionData;
				if (attackCollisionData.PhysicsMaterialIndex != -1)
				{
					attackCollisionData = collisionData;
					if (PhysicsMaterial.GetFromIndex(attackCollisionData.PhysicsMaterialIndex).GetFlags().HasAnyFlag(PhysicsMaterialFlags.AttacksCanPassThrough))
					{
						colReaction = MeleeCollisionReaction.SlicedThrough;
						return;
					}
				}
			}
			attackCollisionData = collisionData;
			if (!attackCollisionData.IsColliderAgent || registeredBlow.InflictedDamage <= 0)
			{
				colReaction = MeleeCollisionReaction.Bounced;
				return;
			}
			attackCollisionData = collisionData;
			if (attackCollisionData.StrikeType == 1 && attacker.IsDoingPassiveAttack)
			{
				colReaction = MissionGameModels.Current.AgentApplyDamageModel.DecidePassiveAttackCollisionReaction(attacker, defender, isFatalHit);
				return;
			}
			if (this.HitWithAnotherBone(collisionData, attacker, attackerWeapon))
			{
				colReaction = MeleeCollisionReaction.Bounced;
				return;
			}
			MissionWeapon missionWeapon = attackerWeapon;
			WeaponClass weaponClass;
			if (missionWeapon.IsEmpty)
			{
				weaponClass = WeaponClass.Undefined;
			}
			else
			{
				missionWeapon = attackerWeapon;
				weaponClass = missionWeapon.CurrentUsageItem.WeaponClass;
			}
			WeaponClass weaponClass2 = weaponClass;
			missionWeapon = attackerWeapon;
			if (missionWeapon.IsEmpty || isFatalHit || !isShruggedOff)
			{
				missionWeapon = attackerWeapon;
				if (missionWeapon.IsEmpty && defender != null && defender.IsHuman)
				{
					attackCollisionData = collisionData;
					if (!attackCollisionData.IsAlternativeAttack)
					{
						attackCollisionData = collisionData;
						if (attackCollisionData.VictimHitBodyPart == BoneBodyPartType.Chest)
						{
							goto IL_1B2;
						}
						attackCollisionData = collisionData;
						if (attackCollisionData.VictimHitBodyPart == BoneBodyPartType.ShoulderLeft)
						{
							goto IL_1B2;
						}
						attackCollisionData = collisionData;
						if (attackCollisionData.VictimHitBodyPart == BoneBodyPartType.ShoulderRight)
						{
							goto IL_1B2;
						}
						attackCollisionData = collisionData;
						if (attackCollisionData.VictimHitBodyPart == BoneBodyPartType.Abdomen)
						{
							goto IL_1B2;
						}
						attackCollisionData = collisionData;
						if (attackCollisionData.VictimHitBodyPart == BoneBodyPartType.Legs)
						{
							goto IL_1B2;
						}
					}
				}
				if ((weaponClass2 != WeaponClass.OneHandedAxe && weaponClass2 != WeaponClass.TwoHandedAxe) || isFatalHit || (float)collisionData.InflictedDamage >= defender.HealthLimit * 0.5f)
				{
					missionWeapon = attackerWeapon;
					if (missionWeapon.IsEmpty)
					{
						attackCollisionData = collisionData;
						if (!attackCollisionData.IsAlternativeAttack)
						{
							attackCollisionData = collisionData;
							if (attackCollisionData.AttackDirection == Agent.UsageDirection.AttackUp)
							{
								goto IL_257;
							}
						}
					}
					attackCollisionData = collisionData;
					if (attackCollisionData.ThrustTipHit)
					{
						attackCollisionData = collisionData;
						if (attackCollisionData.DamageType == 1)
						{
							missionWeapon = attackerWeapon;
							if (!missionWeapon.IsEmpty)
							{
								attackCollisionData = collisionData;
								if (defender.CanThrustAttackStickToBone(attackCollisionData.VictimHitBodyPart))
								{
									goto IL_257;
								}
							}
						}
					}
					colReaction = MeleeCollisionReaction.SlicedThrough;
					goto IL_261;
				}
				IL_257:
				colReaction = MeleeCollisionReaction.Stuck;
				goto IL_261;
			}
			IL_1B2:
			colReaction = MeleeCollisionReaction.Bounced;
			IL_261:
			attackCollisionData = collisionData;
			if (!attackCollisionData.AttackBlockedWithShield)
			{
				attackCollisionData = collisionData;
				if (!attackCollisionData.CollidedWithShieldOnBack)
				{
					return;
				}
			}
			if (colReaction == MeleeCollisionReaction.SlicedThrough)
			{
				colReaction = MeleeCollisionReaction.Bounced;
			}
		}

		private void RegisterBlow(Agent attacker, Agent victim, GameEntity realHitEntity, Blow b, ref AttackCollisionData collisionData, in MissionWeapon attackerWeapon, ref CombatLogData combatLogData)
		{
			b.VictimBodyPart = collisionData.VictimHitBodyPart;
			if (!collisionData.AttackBlockedWithShield)
			{
				if (collisionData.IsColliderAgent)
				{
					if (b.SelfInflictedDamage > 0 && attacker != null && attacker.IsFriendOf(victim))
					{
						Blow blow;
						AttackCollisionData attackCollisionData;
						attacker.CreateBlowFromBlowAsReflection(b, collisionData, out blow, out attackCollisionData);
						if (victim.IsMount && attacker.MountAgent != null)
						{
							attacker.MountAgent.RegisterBlow(blow, attackCollisionData);
						}
						else
						{
							attacker.RegisterBlow(blow, attackCollisionData);
						}
					}
					if (b.InflictedDamage > 0)
					{
						combatLogData.IsFatalDamage = victim != null && victim.Health - (float)b.InflictedDamage < 1f;
						combatLogData.InflictedDamage = b.InflictedDamage - combatLogData.ModifiedDamage;
						this.PrintAttackCollisionResults(attacker, victim, realHitEntity, ref collisionData, ref combatLogData);
					}
					victim.RegisterBlow(b, collisionData);
				}
				else if (collisionData.EntityExists)
				{
					MissionWeapon missionWeapon = (b.IsMissile ? this._missiles[b.WeaponRecord.AffectorWeaponSlotOrMissileIndex].Weapon : ((attacker != null && b.WeaponRecord.HasWeapon()) ? attacker.Equipment[b.WeaponRecord.AffectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid));
					this.OnEntityHit(realHitEntity, attacker, b.InflictedDamage, (DamageTypes)collisionData.DamageType, b.GlobalPosition, b.SwingDirection, missionWeapon);
					if (attacker != null && b.SelfInflictedDamage > 0)
					{
						Blow blow2;
						AttackCollisionData attackCollisionData2;
						attacker.CreateBlowFromBlowAsReflection(b, collisionData, out blow2, out attackCollisionData2);
						attacker.RegisterBlow(blow2, attackCollisionData2);
					}
				}
			}
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnRegisterBlow(attacker, victim, realHitEntity, b, ref collisionData, attackerWeapon);
			}
		}

		private void UpdateMomentumRemaining(ref float momentumRemaining, Blow b, in AttackCollisionData collisionData, Agent attacker, Agent victim, in MissionWeapon attackerWeapon, bool isCrushThrough)
		{
			float num = momentumRemaining;
			momentumRemaining = 0f;
			if (isCrushThrough)
			{
				momentumRemaining = num * 0.3f;
				return;
			}
			if (b.InflictedDamage > 0)
			{
				AttackCollisionData attackCollisionData = collisionData;
				if (!attackCollisionData.AttackBlockedWithShield)
				{
					attackCollisionData = collisionData;
					if (!attackCollisionData.CollidedWithShieldOnBack)
					{
						attackCollisionData = collisionData;
						if (attackCollisionData.IsColliderAgent)
						{
							attackCollisionData = collisionData;
							if (!attackCollisionData.IsHorseCharge)
							{
								if (attacker != null && attacker.IsDoingPassiveAttack)
								{
									momentumRemaining = num * 0.5f;
									return;
								}
								if (!this.HitWithAnotherBone(collisionData, attacker, attackerWeapon))
								{
									MissionWeapon missionWeapon = attackerWeapon;
									if (!missionWeapon.IsEmpty && b.StrikeType != StrikeType.Thrust)
									{
										missionWeapon = attackerWeapon;
										if (!missionWeapon.IsEmpty)
										{
											missionWeapon = attackerWeapon;
											if (missionWeapon.CurrentUsageItem.CanHitMultipleTargets)
											{
												momentumRemaining = num * (1f - b.AbsorbedByArmor / (float)b.InflictedDamage);
												momentumRemaining *= 0.5f;
												if (momentumRemaining < 0.25f)
												{
													momentumRemaining = 0f;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private Blow CreateMissileBlow(Agent attackerAgent, in AttackCollisionData collisionData, in MissionWeapon attackerWeapon, Vec3 missilePosition, Vec3 missileStartingPosition)
		{
			Blow blow = new Blow((attackerAgent != null) ? attackerAgent.Index : (-1));
			MissionWeapon missionWeapon = attackerWeapon;
			blow.BlowFlag = (missionWeapon.CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.CanKnockDown) ? BlowFlags.KnockDown : BlowFlags.None);
			AttackCollisionData attackCollisionData = collisionData;
			blow.Direction = attackCollisionData.MissileVelocity.NormalizedCopy();
			blow.SwingDirection = blow.Direction;
			attackCollisionData = collisionData;
			blow.GlobalPosition = attackCollisionData.CollisionGlobalPosition;
			attackCollisionData = collisionData;
			blow.BoneIndex = attackCollisionData.CollisionBoneIndex;
			attackCollisionData = collisionData;
			blow.StrikeType = (StrikeType)attackCollisionData.StrikeType;
			attackCollisionData = collisionData;
			blow.DamageType = (DamageTypes)attackCollisionData.DamageType;
			attackCollisionData = collisionData;
			blow.VictimBodyPart = attackCollisionData.VictimHitBodyPart;
			sbyte b;
			if (attackerAgent == null)
			{
				b = -1;
			}
			else
			{
				Monster monster = attackerAgent.Monster;
				missionWeapon = attackerWeapon;
				b = monster.GetBoneToAttachForItemFlags(missionWeapon.Item.ItemFlags);
			}
			sbyte b2 = b;
			missionWeapon = attackerWeapon;
			ItemObject item = missionWeapon.Item;
			missionWeapon = attackerWeapon;
			WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
			attackCollisionData = collisionData;
			int affectorWeaponSlotOrMissileIndex = attackCollisionData.AffectorWeaponSlotOrMissileIndex;
			sbyte b3 = b2;
			attackCollisionData = collisionData;
			blow.WeaponRecord.FillAsMissileBlow(item, currentUsageItem, affectorWeaponSlotOrMissileIndex, b3, missileStartingPosition, missilePosition, attackCollisionData.MissileVelocity);
			blow.BaseMagnitude = collisionData.BaseMagnitude;
			blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
			blow.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
			blow.InflictedDamage = collisionData.InflictedDamage;
			blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
			blow.DamageCalculated = true;
			return blow;
		}

		[UsedImplicitly]
		[MBCallback]
		internal float OnAgentHitBlocked(Agent affectedAgent, Agent affectorAgent, ref AttackCollisionData collisionData, Vec3 blowDirection, Vec3 swingDirection, bool isMissile)
		{
			Blow blow;
			if (isMissile)
			{
				Mission.Missile missile = this._missiles[collisionData.AffectorWeaponSlotOrMissileIndex];
				MissionWeapon weapon = missile.Weapon;
				blow = this.CreateMissileBlow(affectorAgent, collisionData, weapon, missile.GetPosition(), collisionData.MissileStartingPosition);
			}
			else
			{
				int affectorWeaponSlotOrMissileIndex = collisionData.AffectorWeaponSlotOrMissileIndex;
				MissionWeapon missionWeapon = ((affectorWeaponSlotOrMissileIndex >= 0) ? affectorAgent.Equipment[affectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid);
				blow = this.CreateMeleeBlow(affectorAgent, affectedAgent, collisionData, missionWeapon, CrushThroughState.None, blowDirection, swingDirection, true);
			}
			return this.OnAgentHit(affectedAgent, affectorAgent, blow, collisionData, true, 0f);
		}

		private Blow CreateMeleeBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, in MissionWeapon attackerWeapon, CrushThroughState crushThroughState, Vec3 blowDirection, Vec3 swingDirection, bool cancelDamage)
		{
			Blow blow = new Blow(attackerAgent.Index);
			AttackCollisionData attackCollisionData = collisionData;
			blow.VictimBodyPart = attackCollisionData.VictimHitBodyPart;
			bool flag = this.HitWithAnotherBone(collisionData, attackerAgent, attackerWeapon);
			attackCollisionData = collisionData;
			MissionWeapon missionWeapon;
			if (attackCollisionData.IsAlternativeAttack)
			{
				missionWeapon = attackerWeapon;
				blow.AttackType = (missionWeapon.IsEmpty ? AgentAttackType.Kick : AgentAttackType.Bash);
			}
			else
			{
				blow.AttackType = AgentAttackType.Standard;
			}
			missionWeapon = attackerWeapon;
			sbyte b;
			if (!missionWeapon.IsEmpty)
			{
				Monster monster = attackerAgent.Monster;
				missionWeapon = attackerWeapon;
				b = monster.GetBoneToAttachForItemFlags(missionWeapon.Item.ItemFlags);
			}
			else
			{
				b = -1;
			}
			sbyte b2 = b;
			missionWeapon = attackerWeapon;
			ItemObject item = missionWeapon.Item;
			missionWeapon = attackerWeapon;
			WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
			attackCollisionData = collisionData;
			blow.WeaponRecord.FillAsMeleeBlow(item, currentUsageItem, attackCollisionData.AffectorWeaponSlotOrMissileIndex, b2);
			attackCollisionData = collisionData;
			blow.StrikeType = (StrikeType)attackCollisionData.StrikeType;
			missionWeapon = attackerWeapon;
			DamageTypes damageTypes;
			if (!missionWeapon.IsEmpty && !flag)
			{
				attackCollisionData = collisionData;
				if (!attackCollisionData.IsAlternativeAttack)
				{
					attackCollisionData = collisionData;
					damageTypes = (DamageTypes)attackCollisionData.DamageType;
					goto IL_122;
				}
			}
			damageTypes = DamageTypes.Blunt;
			IL_122:
			blow.DamageType = damageTypes;
			attackCollisionData = collisionData;
			blow.NoIgnore = attackCollisionData.IsAlternativeAttack;
			attackCollisionData = collisionData;
			blow.AttackerStunPeriod = attackCollisionData.AttackerStunPeriod;
			attackCollisionData = collisionData;
			blow.DefenderStunPeriod = attackCollisionData.DefenderStunPeriod;
			blow.BlowFlag = BlowFlags.None;
			attackCollisionData = collisionData;
			blow.GlobalPosition = attackCollisionData.CollisionGlobalPosition;
			attackCollisionData = collisionData;
			blow.BoneIndex = attackCollisionData.CollisionBoneIndex;
			blow.Direction = blowDirection;
			blow.SwingDirection = swingDirection;
			if (cancelDamage)
			{
				blow.BaseMagnitude = 0f;
				blow.MovementSpeedDamageModifier = 0f;
				blow.InflictedDamage = 0;
				blow.SelfInflictedDamage = 0;
				blow.AbsorbedByArmor = 0f;
			}
			else
			{
				blow.BaseMagnitude = collisionData.BaseMagnitude;
				blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
				blow.InflictedDamage = collisionData.InflictedDamage;
				blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
				blow.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
			}
			blow.DamageCalculated = true;
			if (crushThroughState != CrushThroughState.None)
			{
				blow.BlowFlag |= BlowFlags.CrushThrough;
			}
			if (blow.StrikeType == StrikeType.Thrust)
			{
				attackCollisionData = collisionData;
				if (!attackCollisionData.ThrustTipHit)
				{
					blow.BlowFlag |= BlowFlags.NonTipThrust;
				}
			}
			attackCollisionData = collisionData;
			if (attackCollisionData.IsColliderAgent)
			{
				if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentShrugOffBlow(victimAgent, collisionData, blow))
				{
					blow.BlowFlag |= BlowFlags.ShrugOff;
				}
				if (victimAgent.IsHuman)
				{
					Agent mountAgent = victimAgent.MountAgent;
					if (mountAgent != null)
					{
						if (mountAgent.RiderAgent == victimAgent)
						{
							AgentApplyDamageModel agentApplyDamageModel = MissionGameModels.Current.AgentApplyDamageModel;
							missionWeapon = attackerWeapon;
							if (agentApplyDamageModel.DecideAgentDismountedByBlow(attackerAgent, victimAgent, collisionData, missionWeapon.CurrentUsageItem, blow))
							{
								blow.BlowFlag |= BlowFlags.CanDismount;
							}
						}
					}
					else
					{
						AgentApplyDamageModel agentApplyDamageModel2 = MissionGameModels.Current.AgentApplyDamageModel;
						missionWeapon = attackerWeapon;
						if (agentApplyDamageModel2.DecideAgentKnockedBackByBlow(attackerAgent, victimAgent, collisionData, missionWeapon.CurrentUsageItem, blow))
						{
							blow.BlowFlag |= BlowFlags.KnockBack;
						}
						AgentApplyDamageModel agentApplyDamageModel3 = MissionGameModels.Current.AgentApplyDamageModel;
						missionWeapon = attackerWeapon;
						if (agentApplyDamageModel3.DecideAgentKnockedDownByBlow(attackerAgent, victimAgent, collisionData, missionWeapon.CurrentUsageItem, blow))
						{
							blow.BlowFlag |= BlowFlags.KnockDown;
						}
					}
				}
				else if (victimAgent.IsMount)
				{
					AgentApplyDamageModel agentApplyDamageModel4 = MissionGameModels.Current.AgentApplyDamageModel;
					missionWeapon = attackerWeapon;
					if (agentApplyDamageModel4.DecideMountRearedByBlow(attackerAgent, victimAgent, collisionData, missionWeapon.CurrentUsageItem, blow))
					{
						blow.BlowFlag |= BlowFlags.MakesRear;
					}
				}
			}
			return blow;
		}

		internal float OnAgentHit(Agent affectedAgent, Agent affectorAgent, in Blow b, in AttackCollisionData collisionData, bool isBlocked, float damagedHp)
		{
			float num = -1f;
			bool flag = false;
			int affectorWeaponSlotOrMissileIndex = b.WeaponRecord.AffectorWeaponSlotOrMissileIndex;
			Blow blow = b;
			bool isMissile = blow.IsMissile;
			int inflictedDamage = b.InflictedDamage;
			blow = b;
			float num2 = (blow.IsMissile ? (b.GlobalPosition - b.WeaponRecord.StartingPosition).Length : 0f);
			MissionWeapon missionWeapon;
			if (isMissile)
			{
				missionWeapon = this._missiles[affectorWeaponSlotOrMissileIndex].Weapon;
				flag = this._missiles[affectorWeaponSlotOrMissileIndex].MissionObjectToIgnore != null;
			}
			else
			{
				missionWeapon = ((affectorAgent != null && affectorWeaponSlotOrMissileIndex >= 0) ? affectorAgent.Equipment[affectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid);
			}
			if (affectorAgent != null && isMissile)
			{
				num = this.GetShootDifficulty(affectedAgent, affectorAgent, b.VictimBodyPart == BoneBodyPartType.Head);
			}
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnAgentHit(affectedAgent, affectorAgent, missionWeapon, b, collisionData);
				missionBehavior.OnScoreHit(affectedAgent, affectorAgent, missionWeapon.CurrentUsageItem, isBlocked, flag, b, collisionData, damagedHp, num2, num);
			}
			foreach (AgentComponent agentComponent in affectedAgent.Components)
			{
				agentComponent.OnHit(affectorAgent, inflictedDamage, missionWeapon);
			}
			affectedAgent.CheckToDropFlaggedItem();
			return (float)inflictedDamage;
		}

		[UsedImplicitly]
		[MBCallback]
		internal void MissileAreaDamageCallback(ref AttackCollisionData collisionDataInput, ref Blow blowInput, Agent alreadyDamagedAgent, Agent shooterAgent, bool isBigExplosion)
		{
			float num = (isBigExplosion ? 2.8f : 1.2f);
			float num2 = (isBigExplosion ? 1.6f : 1f);
			float num3 = 1f;
			if (collisionDataInput.MissileVelocity.LengthSquared < 484f)
			{
				num2 *= 0.8f;
				num3 = 0.5f;
			}
			AttackCollisionData attackCollisionData = collisionDataInput;
			blowInput.VictimBodyPart = collisionDataInput.VictimHitBodyPart;
			List<Agent> list = new List<Agent>();
			AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(this, blowInput.GlobalPosition.AsVec2, num, true);
			while (proximityMapSearchStruct.LastFoundAgent != null)
			{
				Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
				if (lastFoundAgent.CurrentMortalityState != Agent.MortalityState.Invulnerable && lastFoundAgent != shooterAgent && lastFoundAgent != alreadyDamagedAgent)
				{
					list.Add(lastFoundAgent);
				}
				AgentProximityMap.FindNext(this, ref proximityMapSearchStruct);
			}
			foreach (Agent agent in list)
			{
				Blow blow = blowInput;
				blow.DamageCalculated = false;
				attackCollisionData = collisionDataInput;
				float num4 = float.MaxValue;
				sbyte b = -1;
				Skeleton skeleton = agent.AgentVisuals.GetSkeleton();
				sbyte boneCount = skeleton.GetBoneCount();
				MatrixFrame globalFrame = agent.AgentVisuals.GetGlobalFrame();
				for (sbyte b2 = 0; b2 < boneCount; b2 += 1)
				{
					float num5 = globalFrame.TransformToParent(skeleton.GetBoneEntitialFrame(b2).origin).DistanceSquared(blowInput.GlobalPosition);
					if (num5 < num4)
					{
						b = b2;
						num4 = num5;
					}
				}
				if (num4 <= num * num)
				{
					float num6 = MathF.Sqrt(num4);
					float num7 = 1f;
					if (num6 > num2)
					{
						float num8 = MBMath.Lerp(1f, 3f, (num6 - num2) / (num - num2), 1E-05f);
						num7 = 1f / (num8 * num8);
					}
					num7 *= num3;
					attackCollisionData.SetCollisionBoneIndexForAreaDamage(b);
					MissionWeapon weapon = this._missiles[attackCollisionData.AffectorWeaponSlotOrMissileIndex].Weapon;
					WeaponComponentData weaponComponentData;
					CombatLogData combatLogData;
					this.GetAttackCollisionResults(shooterAgent, agent, null, 1f, weapon, false, false, false, ref attackCollisionData, out weaponComponentData, out combatLogData);
					blow.BaseMagnitude = attackCollisionData.BaseMagnitude;
					blow.MovementSpeedDamageModifier = attackCollisionData.MovementSpeedDamageModifier;
					blow.InflictedDamage = attackCollisionData.InflictedDamage;
					blow.SelfInflictedDamage = attackCollisionData.SelfInflictedDamage;
					blow.AbsorbedByArmor = (float)attackCollisionData.AbsorbedByArmor;
					blow.DamageCalculated = true;
					blow.InflictedDamage = MathF.Round((float)blow.InflictedDamage * num7);
					blow.SelfInflictedDamage = MathF.Round((float)blow.SelfInflictedDamage * num7);
					combatLogData.ModifiedDamage = MathF.Round((float)combatLogData.ModifiedDamage * num7);
					this.RegisterBlow(shooterAgent, agent, null, blow, ref attackCollisionData, weapon, ref combatLogData);
				}
			}
		}

		[UsedImplicitly]
		[MBCallback]
		internal void OnMissileRemoved(int missileIndex)
		{
			this._missiles.Remove(missileIndex);
		}

		[UsedImplicitly]
		[MBCallback]
		internal bool MissileHitCallback(out int extraHitParticleIndex, ref AttackCollisionData collisionData, Vec3 missileStartingPosition, Vec3 missilePosition, Vec3 missileAngularVelocity, Vec3 movementVelocity, MatrixFrame attachGlobalFrame, MatrixFrame affectedShieldGlobalFrame, int numDamagedAgents, Agent attacker, Agent victim, GameEntity hitEntity)
		{
			Mission.Missile missile = this._missiles[collisionData.AffectorWeaponSlotOrMissileIndex];
			MissionWeapon weapon = missile.Weapon;
			WeaponFlags weaponFlags = weapon.CurrentUsageItem.WeaponFlags;
			float num = 1f;
			WeaponComponentData weaponComponentData = null;
			MissionGameModels.Current.AgentApplyDamageModel.DecideMissileWeaponFlags(attacker, missile.Weapon, ref weaponFlags);
			extraHitParticleIndex = -1;
			Mission.MissileCollisionReaction missileCollisionReaction = Mission.MissileCollisionReaction.Invalid;
			bool flag = !GameNetwork.IsSessionActive;
			bool missileHasPhysics = collisionData.MissileHasPhysics;
			PhysicsMaterial fromIndex = PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex);
			object obj = (fromIndex.IsValid ? fromIndex.GetFlags() : PhysicsMaterialFlags.None);
			bool flag2 = (weaponFlags & WeaponFlags.AmmoSticksWhenShot) > (WeaponFlags)0UL;
			object obj2 = obj;
			bool flag3 = (obj2 & 1) == 0;
			bool flag4 = (obj2 & 8) != 0;
			MissionObject missionObject = null;
			if (victim == null && hitEntity != null)
			{
				GameEntity gameEntity = hitEntity;
				do
				{
					missionObject = gameEntity.GetFirstScriptOfType<MissionObject>();
					gameEntity = gameEntity.Parent;
				}
				while (missionObject == null && gameEntity != null);
				hitEntity = ((missionObject != null) ? missionObject.GameEntity : null);
			}
			Mission.MissileCollisionReaction missileCollisionReaction2;
			if (flag4)
			{
				missileCollisionReaction2 = Mission.MissileCollisionReaction.PassThrough;
			}
			else if (weaponFlags.HasAnyFlag(WeaponFlags.Burning))
			{
				missileCollisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
			}
			else if (!flag3 || !flag2)
			{
				missileCollisionReaction2 = Mission.MissileCollisionReaction.BounceBack;
			}
			else
			{
				missileCollisionReaction2 = Mission.MissileCollisionReaction.Stick;
			}
			bool flag5 = false;
			bool flag6 = victim != null && victim.CurrentMortalityState == Agent.MortalityState.Invulnerable;
			if (collisionData.MissileGoneUnderWater || collisionData.MissileGoneOutOfBorder || flag6)
			{
				missileCollisionReaction = Mission.MissileCollisionReaction.BecomeInvisible;
			}
			else if (victim == null)
			{
				if (hitEntity != null)
				{
					CombatLogData combatLogData;
					this.GetAttackCollisionResults(attacker, victim, hitEntity, num, weapon, false, false, false, ref collisionData, out weaponComponentData, out combatLogData);
					Blow blow = this.CreateMissileBlow(attacker, collisionData, weapon, missilePosition, missileStartingPosition);
					this.RegisterBlow(attacker, null, hitEntity, blow, ref collisionData, weapon, ref combatLogData);
				}
				missileCollisionReaction = missileCollisionReaction2;
			}
			else if (collisionData.AttackBlockedWithShield)
			{
				CombatLogData combatLogData;
				this.GetAttackCollisionResults(attacker, victim, hitEntity, num, weapon, false, false, false, ref collisionData, out weaponComponentData, out combatLogData);
				if (!collisionData.IsShieldBroken)
				{
					this.MakeSound(ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeStone, collisionData.CollisionGlobalPosition, false, false, -1, -1);
				}
				bool flag7 = false;
				if (weaponFlags.HasAnyFlag(WeaponFlags.CanPenetrateShield))
				{
					if (!collisionData.IsShieldBroken)
					{
						EquipmentIndex wieldedItemIndex = victim.GetWieldedItemIndex(Agent.HandIndex.OffHand);
						if ((float)collisionData.InflictedDamage > ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationOffset) + ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationFactor) * (float)victim.Equipment[wieldedItemIndex].GetGetModifiedArmorForCurrentUsage())
						{
							flag7 = true;
						}
					}
					else
					{
						flag7 = true;
					}
				}
				if (flag7)
				{
					victim.MakeVoice(SkinVoiceManager.VoiceType.Pain, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
					num *= 0.4f + MBRandom.RandomFloat * 0.2f;
					missileCollisionReaction = Mission.MissileCollisionReaction.PassThrough;
				}
				else
				{
					missileCollisionReaction = (collisionData.IsShieldBroken ? Mission.MissileCollisionReaction.BecomeInvisible : missileCollisionReaction2);
				}
			}
			else if (collisionData.MissileBlockedWithWeapon)
			{
				CombatLogData combatLogData;
				this.GetAttackCollisionResults(attacker, victim, hitEntity, num, weapon, false, false, false, ref collisionData, out weaponComponentData, out combatLogData);
				missileCollisionReaction = Mission.MissileCollisionReaction.BounceBack;
			}
			else
			{
				if (attacker != null && attacker.IsFriendOf(victim))
				{
					if (this.ForceNoFriendlyFire)
					{
						flag5 = true;
					}
					else if (!missileHasPhysics)
					{
						if (flag)
						{
							if (attacker.Controller == Agent.ControllerType.AI)
							{
								flag5 = true;
							}
						}
						else if ((MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0) || this.Mode == MissionMode.Duel)
						{
							flag5 = true;
						}
					}
				}
				else if (victim.IsHuman && attacker != null && !attacker.IsEnemyOf(victim))
				{
					flag5 = true;
				}
				else if (flag && attacker != null && attacker.Controller == Agent.ControllerType.AI && victim.RiderAgent != null && attacker.IsFriendOf(victim.RiderAgent))
				{
					flag5 = true;
				}
				if (flag5)
				{
					if (flag && attacker != null && attacker == Agent.Main && attacker.IsFriendOf(victim))
					{
						InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_you_hit_a_friendly_troop", null).ToString(), Color.ConvertStringToColor("#D65252FF")));
					}
					missileCollisionReaction = Mission.MissileCollisionReaction.BecomeInvisible;
				}
				else
				{
					bool flag8 = (weaponFlags & WeaponFlags.MultiplePenetration) > (WeaponFlags)0UL;
					CombatLogData combatLogData;
					this.GetAttackCollisionResults(attacker, victim, null, num, weapon, false, false, false, ref collisionData, out weaponComponentData, out combatLogData);
					Blow blow2 = this.CreateMissileBlow(attacker, collisionData, weapon, missilePosition, missileStartingPosition);
					if (collisionData.IsColliderAgent && flag8 && numDamagedAgents > 0)
					{
						blow2.InflictedDamage /= numDamagedAgents;
						blow2.SelfInflictedDamage /= numDamagedAgents;
						combatLogData.InflictedDamage = blow2.InflictedDamage - combatLogData.ModifiedDamage;
					}
					if (collisionData.IsColliderAgent)
					{
						if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentShrugOffBlow(victim, collisionData, blow2))
						{
							blow2.BlowFlag |= BlowFlags.ShrugOff;
						}
						else if (victim.IsHuman)
						{
							Agent mountAgent = victim.MountAgent;
							if (mountAgent != null)
							{
								if (mountAgent.RiderAgent == victim && MissionGameModels.Current.AgentApplyDamageModel.DecideAgentDismountedByBlow(attacker, victim, collisionData, weapon.CurrentUsageItem, blow2))
								{
									blow2.BlowFlag |= BlowFlags.CanDismount;
								}
							}
							else
							{
								if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentKnockedBackByBlow(attacker, victim, collisionData, weapon.CurrentUsageItem, blow2))
								{
									blow2.BlowFlag |= BlowFlags.KnockBack;
								}
								if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentKnockedDownByBlow(attacker, victim, collisionData, weapon.CurrentUsageItem, blow2))
								{
									blow2.BlowFlag |= BlowFlags.KnockDown;
								}
							}
						}
					}
					if (victim.State == AgentState.Active)
					{
						this.RegisterBlow(attacker, victim, null, blow2, ref collisionData, weapon, ref combatLogData);
					}
					extraHitParticleIndex = MissionGameModels.Current.DamageParticleModel.GetMissileAttackParticle(attacker, victim, blow2, collisionData);
					if (flag8 && numDamagedAgents < 3)
					{
						missileCollisionReaction = Mission.MissileCollisionReaction.PassThrough;
					}
					else
					{
						missileCollisionReaction = missileCollisionReaction2;
						if (missileCollisionReaction2 == Mission.MissileCollisionReaction.Stick && !collisionData.CollidedWithShieldOnBack)
						{
							bool flag9 = this.CombatType == Mission.MissionCombatType.Combat;
							if (flag9)
							{
								bool flag10 = victim.IsHuman && collisionData.VictimHitBodyPart == BoneBodyPartType.Head;
								flag9 = victim.State != AgentState.Active || !flag10;
							}
							if (flag9)
							{
								float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MissileMinimumDamageToStick);
								float num2 = 2f * managedParameter;
								if ((float)blow2.InflictedDamage < managedParameter && blow2.AbsorbedByArmor > num2 && !GameNetwork.IsClientOrReplay)
								{
									missileCollisionReaction = Mission.MissileCollisionReaction.BounceBack;
								}
							}
							else
							{
								missileCollisionReaction = Mission.MissileCollisionReaction.BecomeInvisible;
							}
						}
					}
				}
			}
			if (collisionData.CollidedWithShieldOnBack && weaponComponentData != null && victim != null && victim.IsMainAgent)
			{
				InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back", null).ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
			}
			bool flag11;
			MatrixFrame matrixFrame;
			if (!collisionData.MissileHasPhysics && missileCollisionReaction == Mission.MissileCollisionReaction.Stick)
			{
				matrixFrame = this.CalculateAttachedLocalFrame(attachGlobalFrame, collisionData, missile.Weapon.CurrentUsageItem, victim, hitEntity, movementVelocity, missileAngularVelocity, affectedShieldGlobalFrame, true, out flag11);
			}
			else
			{
				matrixFrame = attachGlobalFrame;
				matrixFrame.origin.z = Math.Max(matrixFrame.origin.z, -100f);
				missionObject = null;
				flag11 = false;
			}
			Vec3 zero = Vec3.Zero;
			Vec3 zero2 = Vec3.Zero;
			if (missileCollisionReaction == Mission.MissileCollisionReaction.BounceBack)
			{
				WeaponFlags weaponFlags2 = weaponFlags & WeaponFlags.AmmoBreakOnBounceBackMask;
				if ((weaponFlags2 == WeaponFlags.AmmoCanBreakOnBounceBack && collisionData.MissileVelocity.Length > ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BreakableProjectileMinimumBreakSpeed)) || weaponFlags2 == WeaponFlags.AmmoBreaksOnBounceBack)
				{
					missileCollisionReaction = Mission.MissileCollisionReaction.BecomeInvisible;
					extraHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_broken_arrow");
				}
				else
				{
					missile.CalculateBounceBackVelocity(missileAngularVelocity, collisionData, out zero, out zero2);
				}
			}
			this.HandleMissileCollisionReaction(collisionData.AffectorWeaponSlotOrMissileIndex, missileCollisionReaction, matrixFrame, flag11, attacker, victim, collisionData.AttackBlockedWithShield, collisionData.CollisionBoneIndex, missionObject, zero, zero2, -1);
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnMissileHit(attacker, victim, flag5, collisionData);
			}
			return missileCollisionReaction != Mission.MissileCollisionReaction.PassThrough;
		}

		public void HandleMissileCollisionReaction(int missileIndex, Mission.MissileCollisionReaction collisionReaction, MatrixFrame attachLocalFrame, bool isAttachedFrameLocal, Agent attackerAgent, Agent attachedAgent, bool attachedToShield, sbyte attachedBoneIndex, MissionObject attachedMissionObject, Vec3 bounceBackVelocity, Vec3 bounceBackAngularVelocity, int forcedSpawnIndex)
		{
			Mission.Missile missile = this._missiles[missileIndex];
			MissionObjectId missionObjectId = new MissionObjectId(-1, true);
			switch (collisionReaction)
			{
			case Mission.MissileCollisionReaction.Stick:
				missile.Entity.SetVisibilityExcludeParents(true);
				if (attachedAgent != null)
				{
					this.PrepareMissileWeaponForDrop(missileIndex);
					if (attachedToShield)
					{
						EquipmentIndex wieldedItemIndex = attachedAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
						attachedAgent.AttachWeaponToWeapon(wieldedItemIndex, missile.Weapon, missile.Entity, ref attachLocalFrame);
					}
					else
					{
						attachedAgent.AttachWeaponToBone(missile.Weapon, missile.Entity, attachedBoneIndex, ref attachLocalFrame);
					}
				}
				else
				{
					Vec3 zero = Vec3.Zero;
					missionObjectId = this.SpawnWeaponAsDropFromMissile(missileIndex, attachedMissionObject, attachLocalFrame, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, zero, zero, forcedSpawnIndex);
				}
				break;
			case Mission.MissileCollisionReaction.BounceBack:
				missile.Entity.SetVisibilityExcludeParents(true);
				missionObjectId = this.SpawnWeaponAsDropFromMissile(missileIndex, null, attachLocalFrame, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithPhysics, bounceBackVelocity, bounceBackAngularVelocity, forcedSpawnIndex);
				break;
			case Mission.MissileCollisionReaction.BecomeInvisible:
				missile.Entity.Remove(81);
				break;
			}
			bool flag = collisionReaction != Mission.MissileCollisionReaction.PassThrough;
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new HandleMissileCollisionReaction(missileIndex, collisionReaction, attachLocalFrame, isAttachedFrameLocal, attackerAgent.Index, (attachedAgent != null) ? attachedAgent.Index : (-1), attachedToShield, attachedBoneIndex, (attachedMissionObject != null) ? attachedMissionObject.Id : MissionObjectId.Invalid, bounceBackVelocity, bounceBackAngularVelocity, missionObjectId.Id));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			else if (GameNetwork.IsClientOrReplay && flag)
			{
				this.RemoveMissileAsClient(missileIndex);
			}
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnMissileCollisionReaction(collisionReaction, attackerAgent, attachedAgent, attachedBoneIndex);
			}
		}

		[UsedImplicitly]
		[MBCallback]
		internal void MissileCalculatePassbySoundParametersCallbackMT(int missileIndex, ref SoundEventParameter soundEventParameter)
		{
			this._missiles[missileIndex].CalculatePassbySoundParametersMT(ref soundEventParameter);
		}

		[UsedImplicitly]
		[MBCallback]
		internal void ChargeDamageCallback(ref AttackCollisionData collisionData, Blow blow, Agent attacker, Agent victim)
		{
			if (victim.CurrentMortalityState != Agent.MortalityState.Invulnerable && (attacker.RiderAgent == null || attacker.IsEnemyOf(victim)))
			{
				WeaponComponentData weaponComponentData;
				CombatLogData combatLogData;
				this.GetAttackCollisionResults(attacker, victim, null, 1f, MissionWeapon.Invalid, false, false, false, ref collisionData, out weaponComponentData, out combatLogData);
				if (collisionData.CollidedWithShieldOnBack && weaponComponentData != null && victim != null && victim.IsMainAgent)
				{
					InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back", null).ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
				}
				if ((float)collisionData.InflictedDamage > 0f)
				{
					blow.BaseMagnitude = collisionData.BaseMagnitude;
					blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
					blow.InflictedDamage = collisionData.InflictedDamage;
					blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
					blow.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
					blow.DamageCalculated = true;
					if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentKnockedBackByBlow(attacker, victim, collisionData, null, blow))
					{
						blow.BlowFlag |= BlowFlags.KnockBack;
					}
					else
					{
						blow.BlowFlag &= ~BlowFlags.KnockBack;
					}
					if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentKnockedDownByBlow(attacker, victim, collisionData, null, blow))
					{
						blow.BlowFlag |= BlowFlags.KnockDown;
					}
					GameEntity gameEntity = null;
					Blow blow2 = blow;
					MissionWeapon missionWeapon = default(MissionWeapon);
					this.RegisterBlow(attacker, victim, gameEntity, blow2, ref collisionData, missionWeapon, ref combatLogData);
				}
			}
		}

		[UsedImplicitly]
		[MBCallback]
		internal void FallDamageCallback(ref AttackCollisionData collisionData, Blow b, Agent attacker, Agent victim)
		{
			if (victim.CurrentMortalityState != Agent.MortalityState.Invulnerable)
			{
				WeaponComponentData weaponComponentData;
				CombatLogData combatLogData;
				this.GetAttackCollisionResults(attacker, victim, null, 1f, MissionWeapon.Invalid, false, false, false, ref collisionData, out weaponComponentData, out combatLogData);
				b.BaseMagnitude = collisionData.BaseMagnitude;
				b.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
				b.InflictedDamage = collisionData.InflictedDamage;
				b.SelfInflictedDamage = collisionData.SelfInflictedDamage;
				b.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
				b.DamageCalculated = true;
				if (b.InflictedDamage > 0)
				{
					Agent riderAgent = victim.RiderAgent;
					GameEntity gameEntity = null;
					Blow blow = b;
					MissionWeapon missionWeapon = default(MissionWeapon);
					this.RegisterBlow(attacker, victim, gameEntity, blow, ref collisionData, missionWeapon, ref combatLogData);
					if (riderAgent != null)
					{
						this.FallDamageCallback(ref collisionData, b, riderAgent, riderAgent);
					}
				}
			}
		}

		public void KillAgentsOnEntity(GameEntity entity, Agent destroyerAgent, bool burnAgents)
		{
			if (entity == null)
			{
				return;
			}
			int num;
			sbyte b;
			if (destroyerAgent != null)
			{
				num = destroyerAgent.Index;
				b = destroyerAgent.Monster.MainHandItemBoneIndex;
			}
			else
			{
				num = -1;
				b = -1;
			}
			Vec3 vec;
			Vec3 vec2;
			entity.GetPhysicsMinMax(true, out vec, out vec2, false);
			Vec2 vec3 = (vec2.AsVec2 + vec.AsVec2) * 0.5f;
			float num2 = (vec2.AsVec2 - vec.AsVec2).Length * 0.5f;
			Blow blow = new Blow(num);
			blow.DamageCalculated = true;
			blow.BaseMagnitude = 2000f;
			blow.InflictedDamage = 2000;
			blow.Direction = new Vec3(0f, 0f, -1f, -1f);
			blow.DamageType = DamageTypes.Blunt;
			blow.BoneIndex = 0;
			blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, 0);
			if (burnAgents)
			{
				blow.WeaponRecord.WeaponFlags = blow.WeaponRecord.WeaponFlags | (WeaponFlags.AffectsArea | WeaponFlags.Burning);
				blow.WeaponRecord.CurrentPosition = blow.GlobalPosition;
				blow.WeaponRecord.StartingPosition = blow.GlobalPosition;
			}
			Vec2 asVec = entity.GetGlobalFrame().TransformToParent(vec3.ToVec3(0f)).AsVec2;
			List<Agent> list = new List<Agent>();
			AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(this, asVec, num2, false);
			while (proximityMapSearchStruct.LastFoundAgent != null)
			{
				Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
				GameEntity gameEntity = lastFoundAgent.GetSteppedEntity();
				while (gameEntity != null && !(gameEntity == entity))
				{
					gameEntity = gameEntity.Parent;
				}
				if (gameEntity != null)
				{
					list.Add(lastFoundAgent);
				}
				AgentProximityMap.FindNext(this, ref proximityMapSearchStruct);
			}
			foreach (Agent agent in list)
			{
				blow.GlobalPosition = agent.Position;
				AttackCollisionData attackCollisionDataForDebugPurpose = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Abdomen, b, Agent.UsageDirection.AttackLeft, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, agent.Velocity, Vec3.Up);
				agent.RegisterBlow(blow, attackCollisionDataForDebugPurpose);
			}
		}

		public void KillAgentCheat(Agent agent)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				Agent agent2 = this.MainAgent ?? agent;
				Blow blow = new Blow(agent2.Index);
				blow.DamageType = DamageTypes.Blunt;
				blow.BoneIndex = agent.Monster.HeadLookDirectionBoneIndex;
				blow.GlobalPosition = agent.Position;
				blow.GlobalPosition.z = blow.GlobalPosition.z + agent.GetEyeGlobalHeight();
				blow.BaseMagnitude = 2000f;
				blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, -1);
				blow.InflictedDamage = 2000;
				blow.SwingDirection = agent.LookDirection;
				if (this.InputManager.IsGameKeyDown(2))
				{
					MatrixFrame matrixFrame = agent.Frame;
					blow.SwingDirection = matrixFrame.rotation.TransformToParent(new Vec3(-1f, 0f, 0f, -1f));
					blow.SwingDirection.Normalize();
				}
				else if (this.InputManager.IsGameKeyDown(3))
				{
					MatrixFrame matrixFrame = agent.Frame;
					blow.SwingDirection = matrixFrame.rotation.TransformToParent(new Vec3(1f, 0f, 0f, -1f));
					blow.SwingDirection.Normalize();
				}
				else if (this.InputManager.IsGameKeyDown(1))
				{
					MatrixFrame matrixFrame = agent.Frame;
					blow.SwingDirection = matrixFrame.rotation.TransformToParent(new Vec3(0f, -1f, 0f, -1f));
					blow.SwingDirection.Normalize();
				}
				else if (this.InputManager.IsGameKeyDown(0))
				{
					MatrixFrame matrixFrame = agent.Frame;
					blow.SwingDirection = matrixFrame.rotation.TransformToParent(new Vec3(0f, 1f, 0f, -1f));
					blow.SwingDirection.Normalize();
				}
				blow.Direction = blow.SwingDirection;
				blow.DamageCalculated = true;
				sbyte mainHandItemBoneIndex = agent2.Monster.MainHandItemBoneIndex;
				AttackCollisionData attackCollisionDataForDebugPurpose = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Head, mainHandItemBoneIndex, Agent.UsageDirection.AttackLeft, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, agent.Velocity, Vec3.Up);
				agent.RegisterBlow(blow, attackCollisionDataForDebugPurpose);
			}
		}

		public bool KillCheats(bool killAll, bool killEnemy, bool killHorse, bool killYourself)
		{
			bool flag = false;
			if (!GameNetwork.IsClientOrReplay)
			{
				if (killYourself)
				{
					if (this.MainAgent != null)
					{
						if (killHorse)
						{
							if (this.MainAgent.MountAgent != null)
							{
								Agent mountAgent = this.MainAgent.MountAgent;
								this.KillAgentCheat(mountAgent);
								flag = true;
							}
						}
						else
						{
							Agent mainAgent = this.MainAgent;
							this.KillAgentCheat(mainAgent);
							flag = true;
						}
					}
				}
				else
				{
					bool flag2 = false;
					int num = this.Agents.Count - 1;
					while (num >= 0 && !flag2)
					{
						Agent agent = this.Agents[num];
						if (agent != this.MainAgent && agent.GetAgentFlags().HasAnyFlag(AgentFlag.CanAttack) && this.PlayerTeam != null)
						{
							if (killEnemy)
							{
								if (agent.Team.IsValid && this.PlayerTeam.IsEnemyOf(agent.Team))
								{
									if (killHorse && agent.HasMount)
									{
										if (agent.MountAgent != null)
										{
											this.KillAgentCheat(agent.MountAgent);
											if (!killAll)
											{
												flag2 = true;
											}
											flag = true;
										}
									}
									else
									{
										this.KillAgentCheat(agent);
										if (!killAll)
										{
											flag2 = true;
										}
										flag = true;
									}
								}
							}
							else if (agent.Team.IsValid && this.PlayerTeam.IsFriendOf(agent.Team))
							{
								if (killHorse)
								{
									if (agent.MountAgent != null)
									{
										this.KillAgentCheat(agent.MountAgent);
										if (!killAll)
										{
											flag2 = true;
										}
										flag = true;
									}
								}
								else
								{
									this.KillAgentCheat(agent);
									if (!killAll)
									{
										flag2 = true;
									}
									flag = true;
								}
							}
						}
						num--;
					}
				}
			}
			return flag;
		}

		private bool CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(Agent attacker, Agent victim)
		{
			if (victim == null || attacker == null)
			{
				return false;
			}
			bool flag = !GameNetwork.IsSessionActive || this.ForceNoFriendlyFire || (MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0) || this.Mode == MissionMode.Duel || attacker.Controller == Agent.ControllerType.AI;
			bool flag2 = attacker.IsFriendOf(victim);
			return (flag && flag2) || (victim.IsHuman && !flag2 && !attacker.IsEnemyOf(victim));
		}

		public float GetDamageMultiplierOfCombatDifficulty(Agent victimAgent, Agent attackerAgent = null)
		{
			if (MissionGameModels.Current.MissionDifficultyModel != null)
			{
				return MissionGameModels.Current.MissionDifficultyModel.GetDamageMultiplierOfCombatDifficulty(victimAgent, attackerAgent);
			}
			return 1f;
		}

		public float GetShootDifficulty(Agent affectedAgent, Agent affectorAgent, bool isHeadShot)
		{
			Vec2 vec = affectedAgent.MovementVelocity - affectorAgent.MovementVelocity;
			Vec3 vec2 = new Vec3(vec.x, vec.y, 0f, -1f);
			Vec3 vec3 = affectedAgent.Position - affectorAgent.Position;
			float num = vec3.Normalize();
			float num2 = vec2.Normalize();
			float length = Vec3.CrossProduct(vec2, vec3).Length;
			float num3 = MBMath.ClampFloat(0.3f * ((4f + num) / 4f) * ((4f + length * num2) / 4f), 1f, 12f);
			if (isHeadShot)
			{
				num3 *= 1.2f;
			}
			return num3;
		}

		private MatrixFrame CalculateAttachedLocalFrame(in MatrixFrame attachedGlobalFrame, AttackCollisionData collisionData, WeaponComponentData missileWeapon, Agent affectedAgent, GameEntity hitEntity, Vec3 missileMovementVelocity, Vec3 missileRotationSpeed, MatrixFrame shieldGlobalFrame, bool shouldMissilePenetrate, out bool isAttachedFrameLocal)
		{
			isAttachedFrameLocal = false;
			MatrixFrame matrixFrame = attachedGlobalFrame;
			bool isNonZero = missileWeapon.RotationSpeed.IsNonZero;
			bool flag = affectedAgent != null && !collisionData.AttackBlockedWithShield && missileWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.AmmoSticksWhenShot);
			float managedParameter = ManagedParameters.Instance.GetManagedParameter(flag ? (isNonZero ? ManagedParametersEnum.RotatingProjectileMinPenetration : ManagedParametersEnum.ProjectileMinPenetration) : ManagedParametersEnum.ObjectMinPenetration);
			float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(flag ? (isNonZero ? ManagedParametersEnum.RotatingProjectileMaxPenetration : ManagedParametersEnum.ProjectileMaxPenetration) : ManagedParametersEnum.ObjectMaxPenetration);
			Vec3 vec = missileMovementVelocity;
			float num = vec.Normalize();
			float num2 = MBMath.ClampFloat(flag ? ((float)collisionData.InflictedDamage / affectedAgent.HealthLimit) : (num / ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ProjectileMaxPenetrationSpeed)), 0f, 1f);
			if (shouldMissilePenetrate)
			{
				float num3 = managedParameter + (managedParameter2 - managedParameter) * num2;
				matrixFrame.origin += vec * num3;
			}
			if (missileRotationSpeed.IsNonZero)
			{
				float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(flag ? ManagedParametersEnum.AgentProjectileNormalWeight : ManagedParametersEnum.ProjectileNormalWeight);
				Vec3 vec2 = missileWeapon.GetMissileStartingFrame().TransformToParent(missileRotationSpeed);
				Vec3 vec3 = -collisionData.CollisionGlobalNormal;
				float num4 = vec2.x * vec2.x;
				float num5 = vec2.y * vec2.y;
				float num6 = vec2.z * vec2.z;
				int num7 = ((num4 > num5 && num4 > num6) ? 0 : ((num5 > num6) ? 1 : 2));
				vec3 -= vec3.ProjectOnUnitVector(matrixFrame.rotation[num7]);
				Vec3 vec4 = Vec3.CrossProduct(vec, vec3.NormalizedCopy());
				float num8 = vec4.Normalize();
				matrixFrame.rotation.RotateAboutAnArbitraryVector(vec4, num8 * managedParameter3);
			}
			if (!collisionData.AttackBlockedWithShield && affectedAgent != null)
			{
				float num9 = Vec3.DotProduct(collisionData.CollisionGlobalNormal, vec) + 1f;
				if (num9 > 0.5f)
				{
					matrixFrame.origin -= num9 * 0.1f * collisionData.CollisionGlobalNormal;
				}
			}
			matrixFrame = matrixFrame.TransformToParent(missileWeapon.GetMissileStartingFrame().TransformToParent(missileWeapon.StickingFrame));
			matrixFrame = matrixFrame.TransformToParent(missileWeapon.GetMissileStartingFrame());
			if (collisionData.AttackBlockedWithShield)
			{
				matrixFrame = shieldGlobalFrame.TransformToLocal(matrixFrame);
				isAttachedFrameLocal = true;
			}
			else if (affectedAgent != null)
			{
				if (flag)
				{
					MBAgentVisuals agentVisuals = affectedAgent.AgentVisuals;
					matrixFrame = agentVisuals.GetGlobalFrame().TransformToParent(agentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex(collisionData.CollisionBoneIndex)).GetUnitRotFrame(affectedAgent.AgentScale)
						.TransformToLocalNonOrthogonal(ref matrixFrame);
					isAttachedFrameLocal = true;
				}
			}
			else if (hitEntity != null)
			{
				if (collisionData.CollisionBoneIndex >= 0)
				{
					matrixFrame = hitEntity.Skeleton.GetBoneEntitialFrameWithIndex(collisionData.CollisionBoneIndex).TransformToLocalNonOrthogonal(ref matrixFrame);
					isAttachedFrameLocal = true;
				}
				else
				{
					matrixFrame = hitEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref matrixFrame);
					isAttachedFrameLocal = true;
				}
			}
			else
			{
				matrixFrame.origin.z = Math.Max(matrixFrame.origin.z, -100f);
			}
			return matrixFrame;
		}

		[UsedImplicitly]
		[MBCallback]
		internal void GetDefendCollisionResults(Agent attackerAgent, Agent defenderAgent, CombatCollisionResult collisionResult, int attackerWeaponSlotIndex, bool isAlternativeAttack, StrikeType strikeType, Agent.UsageDirection attackDirection, float collisionDistanceOnWeapon, float attackProgress, bool attackIsParried, bool isPassiveUsageHit, bool isHeavyAttack, ref float defenderStunPeriod, ref float attackerStunPeriod, ref bool crushedThrough)
		{
			bool flag = false;
			MissionCombatMechanicsHelper.GetDefendCollisionResults(attackerAgent, defenderAgent, collisionResult, attackerWeaponSlotIndex, isAlternativeAttack, strikeType, attackDirection, collisionDistanceOnWeapon, attackProgress, attackIsParried, isPassiveUsageHit, isHeavyAttack, ref defenderStunPeriod, ref attackerStunPeriod, ref crushedThrough, ref flag);
			if ((crushedThrough || flag) && (attackerAgent.CanLogCombatFor || defenderAgent.CanLogCombatFor))
			{
				CombatLogData combatLogData = new CombatLogData(false, attackerAgent.IsHuman, attackerAgent.IsMine, attackerAgent.RiderAgent != null, attackerAgent.RiderAgent != null && attackerAgent.RiderAgent.IsMine, attackerAgent.IsMount, defenderAgent.IsHuman, defenderAgent.IsMine, defenderAgent.Health <= 0f, defenderAgent.HasMount, defenderAgent.RiderAgent != null && defenderAgent.RiderAgent.IsMine, defenderAgent.IsMount, false, defenderAgent.RiderAgent == attackerAgent, crushedThrough, flag, 0f);
				this.AddCombatLogSafe(attackerAgent, defenderAgent, null, combatLogData);
			}
		}

		private CombatLogData GetAttackCollisionResults(Agent attackerAgent, Agent victimAgent, GameEntity hitObject, float momentumRemaining, in MissionWeapon attackerWeapon, bool crushedThrough, bool cancelDamage, bool crushedThroughWithoutAgentCollision, ref AttackCollisionData attackCollisionData, out WeaponComponentData shieldOnBack, out CombatLogData combatLog)
		{
			AttackInformation attackInformation = new AttackInformation(attackerAgent, victimAgent, hitObject, attackCollisionData, attackerWeapon);
			shieldOnBack = attackInformation.ShieldOnBack;
			MPPerkObject.MPCombatPerkHandler combatPerkHandler = MPPerkObject.GetCombatPerkHandler(attackerAgent, victimAgent);
			int num;
			MissionCombatMechanicsHelper.GetAttackCollisionResults(attackInformation, crushedThrough, momentumRemaining, attackerWeapon, cancelDamage, ref attackCollisionData, out combatLog, out num);
			float num2 = (float)attackCollisionData.InflictedDamage;
			float num3 = num2;
			if (combatPerkHandler != null && !attackCollisionData.IsFallDamage && !attackCollisionData.IsHorseCharge && num > 0)
			{
				float num4 = (float)num / 100f;
				float num5 = num4 * combatPerkHandler.GetSpeedBonusEffectiveness() + num4;
				attackCollisionData.BaseMagnitude *= (num5 + 1f) / (num4 + 1f);
			}
			if (num2 > 0f)
			{
				if (attackCollisionData.AttackBlockedWithShield && combatPerkHandler != null)
				{
					float num6 = 1f + combatPerkHandler.GetShieldDamage(attackCollisionData.CorrectSideShieldBlock) + combatPerkHandler.GetShieldDamageTaken(attackCollisionData.CorrectSideShieldBlock);
					num3 = MathF.Max(0f, num3 * num6);
				}
				num3 = MissionGameModels.Current.AgentApplyDamageModel.CalculateDamage(attackInformation, attackCollisionData, attackerWeapon, num3);
				if (combatPerkHandler != null)
				{
					float num7 = 0f;
					float num8 = 1f;
					MPPerkObject.MPCombatPerkHandler mpcombatPerkHandler = combatPerkHandler;
					MissionWeapon missionWeapon = attackerWeapon;
					float num9 = num8 + mpcombatPerkHandler.GetDamage(missionWeapon.CurrentUsageItem, combatLog.DamageType, attackCollisionData.IsAlternativeAttack);
					MPPerkObject.MPCombatPerkHandler mpcombatPerkHandler2 = combatPerkHandler;
					missionWeapon = attackerWeapon;
					float num10 = MathF.Max(num7, num9 + mpcombatPerkHandler2.GetDamageTaken(missionWeapon.CurrentUsageItem, combatLog.DamageType));
					if (attackInformation.IsHeadShot)
					{
						missionWeapon = attackerWeapon;
						if (missionWeapon.CurrentUsageItem != null)
						{
							missionWeapon = attackerWeapon;
							if (!missionWeapon.CurrentUsageItem.IsConsumable)
							{
								missionWeapon = attackerWeapon;
								if (!missionWeapon.CurrentUsageItem.IsRangedWeapon)
								{
									goto IL_19D;
								}
							}
							num10 += combatPerkHandler.GetRangedHeadShotDamage();
						}
					}
					IL_19D:
					num3 *= num10;
				}
				combatLog.ModifiedDamage = MathF.Round(num3 - num2);
				attackCollisionData.InflictedDamage = MathF.Round(num3);
			}
			else
			{
				combatLog.ModifiedDamage = 0;
				attackCollisionData.InflictedDamage = 0;
			}
			if (!attackCollisionData.IsFallDamage && attackInformation.IsFriendlyFire)
			{
				if (!attackInformation.IsAttackerAIControlled && GameNetwork.IsSessionActive)
				{
					int num11 = (attackCollisionData.IsMissile ? MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
					attackCollisionData.SelfInflictedDamage = MathF.Round((float)attackCollisionData.InflictedDamage * ((float)num11 * 0.01f));
					int num12 = (attackCollisionData.IsMissile ? MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
					attackCollisionData.InflictedDamage = MathF.Round((float)attackCollisionData.InflictedDamage * ((float)num12 * 0.01f));
					combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
				}
				combatLog.IsFriendlyFire = true;
			}
			if (attackCollisionData.AttackBlockedWithShield && attackCollisionData.InflictedDamage > 0 && (int)attackInformation.VictimShield.HitPoints - attackCollisionData.InflictedDamage <= 0)
			{
				attackCollisionData.IsShieldBroken = true;
			}
			if (!crushedThroughWithoutAgentCollision)
			{
				combatLog.BodyPartHit = attackCollisionData.VictimHitBodyPart;
				combatLog.IsVictimEntity = hitObject != null;
			}
			return combatLog;
		}

		private void PrintAttackCollisionResults(Agent attackerAgent, Agent victimAgent, GameEntity hitEntity, ref AttackCollisionData attackCollisionData, ref CombatLogData combatLog)
		{
			if (attackCollisionData.IsColliderAgent && !attackCollisionData.AttackBlockedWithShield && attackerAgent != null && (attackerAgent.CanLogCombatFor || victimAgent.CanLogCombatFor) && victimAgent.State == AgentState.Active)
			{
				this.AddCombatLogSafe(attackerAgent, victimAgent, hitEntity, combatLog);
			}
		}

		private void AddCombatLogSafe(Agent attackerAgent, Agent victimAgent, GameEntity hitEntity, CombatLogData combatLog)
		{
			combatLog.SetVictimAgent(victimAgent);
			if (GameNetwork.IsServerOrRecorder)
			{
				CombatLogNetworkMessage combatLogNetworkMessage = new CombatLogNetworkMessage(attackerAgent.Index, (victimAgent != null) ? victimAgent.Index : (-1), hitEntity, combatLog);
				object obj = ((attackerAgent == null) ? null : (attackerAgent.IsHuman ? attackerAgent : attackerAgent.RiderAgent));
				object obj2;
				if (obj == null)
				{
					obj2 = null;
				}
				else
				{
					MissionPeer missionPeer = obj.MissionPeer;
					obj2 = ((missionPeer != null) ? missionPeer.Peer.Communicator : null);
				}
				NetworkCommunicator networkCommunicator = obj2 as NetworkCommunicator;
				object obj3 = ((victimAgent == null) ? null : (victimAgent.IsHuman ? victimAgent : victimAgent.RiderAgent));
				object obj4;
				if (obj3 == null)
				{
					obj4 = null;
				}
				else
				{
					MissionPeer missionPeer2 = obj3.MissionPeer;
					obj4 = ((missionPeer2 != null) ? missionPeer2.Peer.Communicator : null);
				}
				NetworkCommunicator networkCommunicator2 = obj4 as NetworkCommunicator;
				if (networkCommunicator != null && !networkCommunicator.IsServerPeer)
				{
					GameNetwork.BeginModuleEventAsServer(networkCommunicator);
					GameNetwork.WriteMessage(combatLogNetworkMessage);
					GameNetwork.EndModuleEventAsServer();
				}
				if (networkCommunicator2 != null && !networkCommunicator2.IsServerPeer && networkCommunicator2 != networkCommunicator)
				{
					GameNetwork.BeginModuleEventAsServer(networkCommunicator2);
					GameNetwork.WriteMessage(combatLogNetworkMessage);
					GameNetwork.EndModuleEventAsServer();
				}
			}
			this._combatLogsCreated.Enqueue(combatLog);
		}

		public MissionObject CreateMissionObjectFromPrefab(string prefab, MatrixFrame frame)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				GameEntity gameEntity = GameEntity.Instantiate(this.Scene, prefab, frame);
				MissionObject firstScriptOfType = gameEntity.GetFirstScriptOfType<MissionObject>();
				List<MissionObjectId> list = new List<MissionObjectId>();
				using (IEnumerator<GameEntity> enumerator = gameEntity.GetChildren().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionObject firstScriptOfType2;
						if ((firstScriptOfType2 = enumerator.Current.GetFirstScriptOfType<MissionObject>()) != null)
						{
							list.Add(firstScriptOfType2.Id);
						}
					}
				}
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new CreateMissionObject(firstScriptOfType.Id, prefab, frame, list));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					this.AddDynamicallySpawnedMissionObjectInfo(new Mission.DynamicallyCreatedEntity(prefab, firstScriptOfType.Id, frame, ref list));
				}
				return firstScriptOfType;
			}
			return null;
		}

		public int GetNearbyAllyAgentsCount(Vec2 center, float radius, Team team)
		{
			return this.GetNearbyAgentsCountAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Friend);
		}

		public MBList<Agent> GetNearbyAllyAgents(Vec2 center, float radius, Team team, MBList<Agent> agents)
		{
			agents.Clear();
			this.GetNearbyAgentsAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Friend, agents);
			return agents;
		}

		public MBList<Agent> GetNearbyEnemyAgents(Vec2 center, float radius, Team team, MBList<Agent> agents)
		{
			agents.Clear();
			this.GetNearbyAgentsAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Enemy, agents);
			return agents;
		}

		public MBList<Agent> GetNearbyAgents(Vec2 center, float radius, MBList<Agent> agents)
		{
			agents.Clear();
			this.GetNearbyAgentsAux(center, radius, MBTeam.InvalidTeam, Mission.GetNearbyAgentsAuxType.All, agents);
			return agents;
		}

		public bool IsFormationUnitPositionAvailable(ref WorldPosition formationPosition, ref WorldPosition unitPosition, ref WorldPosition nearestAvailableUnitPosition, float manhattanDistance, Team team)
		{
			return formationPosition.IsValid && !(formationPosition.GetNavMesh() == UIntPtr.Zero) && unitPosition.IsValid && !(unitPosition.GetNavMesh() == UIntPtr.Zero) && (this.IsFormationUnitPositionAvailable_AdditionalCondition == null || this.IsFormationUnitPositionAvailable_AdditionalCondition(unitPosition, team)) && this.IsFormationUnitPositionAvailableAux(ref formationPosition, ref unitPosition, ref nearestAvailableUnitPosition, manhattanDistance);
		}

		public bool IsOrderPositionAvailable(in WorldPosition orderPosition, Team team)
		{
			WorldPosition worldPosition = orderPosition;
			if (worldPosition.IsValid)
			{
				worldPosition = orderPosition;
				if (!(worldPosition.GetNavMesh() == UIntPtr.Zero))
				{
					if (this.IsFormationUnitPositionAvailable_AdditionalCondition != null && !this.IsFormationUnitPositionAvailable_AdditionalCondition(orderPosition, team))
					{
						return false;
					}
					worldPosition = orderPosition;
					return this.IsPositionInsideBoundaries(worldPosition.AsVec2);
				}
			}
			return false;
		}

		public bool IsFormationUnitPositionAvailable(ref WorldPosition unitPosition, Team team)
		{
			WorldPosition worldPosition = unitPosition;
			float num = 1f;
			WorldPosition invalid = WorldPosition.Invalid;
			return this.IsFormationUnitPositionAvailable(ref worldPosition, ref unitPosition, ref invalid, num, team);
		}

		public bool HasSceneMapPatch()
		{
			return this.InitializerRecord.SceneHasMapPatch;
		}

		public bool GetPatchSceneEncounterPosition(out Vec3 position)
		{
			if (this.InitializerRecord.SceneHasMapPatch)
			{
				Vec2 patchCoordinates = this.InitializerRecord.PatchCoordinates;
				float northRotation = this.Scene.GetNorthRotation();
				Vec2 vec;
				Vec2 vec2;
				this.Boundaries.GetOrientedBoundariesBox(out vec, out vec2, northRotation);
				Vec2 side = Vec2.Side;
				side.RotateCCW(northRotation);
				Vec2 vec3 = side.LeftVec();
				Vec2 vec4 = vec2 - vec;
				Vec2 vec5 = vec.x * side + vec.y * vec3 + vec4.x * patchCoordinates.x * side + vec4.y * patchCoordinates.y * vec3;
				position = vec5.ToVec3(this.Scene.GetTerrainHeight(vec5, true));
				return true;
			}
			position = Vec3.Invalid;
			return false;
		}

		public bool GetPatchSceneEncounterDirection(out Vec2 direction)
		{
			if (this.InitializerRecord.SceneHasMapPatch)
			{
				float northRotation = this.Scene.GetNorthRotation();
				direction = this.InitializerRecord.PatchEncounterDir;
				direction.RotateCCW(northRotation);
				return true;
			}
			direction = Vec2.Invalid;
			return false;
		}

		private void TickDebugAgents()
		{
		}

		public void AddTimerToDynamicEntity(GameEntity gameEntity, float timeToKill = 10f)
		{
			Mission.DynamicEntityInfo dynamicEntityInfo = new Mission.DynamicEntityInfo
			{
				Entity = gameEntity,
				TimerToDisable = new Timer(this.CurrentTime, timeToKill, true)
			};
			this._dynamicEntities.Add(dynamicEntityInfo);
		}

		public void AddListener(IMissionListener listener)
		{
			this._listeners.Add(listener);
		}

		public void RemoveListener(IMissionListener listener)
		{
			this._listeners.Remove(listener);
		}

		public void OnAgentFleeing(Agent agent)
		{
			for (int i = this.MissionBehaviors.Count - 1; i >= 0; i--)
			{
				this.MissionBehaviors[i].OnAgentFleeing(agent);
			}
			agent.OnFleeing();
		}

		public void OnAgentPanicked(Agent agent)
		{
			for (int i = this.MissionBehaviors.Count - 1; i >= 0; i--)
			{
				this.MissionBehaviors[i].OnAgentPanicked(agent);
			}
		}

		public void OnDeploymentFinished()
		{
			foreach (Team team in this.Teams)
			{
				if (team.TeamAI != null)
				{
					team.TeamAI.OnDeploymentFinished();
				}
			}
			for (int i = this.MissionBehaviors.Count - 1; i >= 0; i--)
			{
				this.MissionBehaviors[i].OnDeploymentFinished();
			}
		}

		public void SetFastForwardingFromUI(bool fastForwarding)
		{
			this.IsFastForward = fastForwarding;
		}

		public bool CheckIfBattleInRetreat()
		{
			Func<bool> isBattleInRetreatEvent = this.IsBattleInRetreatEvent;
			return isBattleInRetreatEvent != null && isBattleInRetreatEvent();
		}

		public void AddSpawnedItemEntityCreatedAtRuntime(SpawnedItemEntity spawnedItemEntity)
		{
			this._spawnedItemEntitiesCreatedAtRuntime.Add(spawnedItemEntity);
		}

		public void TriggerOnItemPickUpEvent(Agent agent, SpawnedItemEntity spawnedItemEntity)
		{
			Action<Agent, SpawnedItemEntity> onItemPickUp = this.OnItemPickUp;
			if (onItemPickUp == null)
			{
				return;
			}
			onItemPickUp(agent, spawnedItemEntity);
		}

		[UsedImplicitly]
		[MBCallback]
		internal static void DebugLogNativeMissionNetworkEvent(int eventEnum, string eventName, int bitCount)
		{
			int num = eventEnum + CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo.GetMaximumValue() + 1;
			DebugNetworkEventStatistics.StartEvent(eventName, num);
			DebugNetworkEventStatistics.AddDataToStatistic(bitCount);
			DebugNetworkEventStatistics.EndEvent();
		}

		[UsedImplicitly]
		[MBCallback]
		internal void PauseMission()
		{
			this._missionState.Paused = true;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("toggleDisableDying", "mission")]
		public static string ToggleDisableDying(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			int num = 0;
			if (strings.Count > 0 && !int.TryParse(strings[0], out num))
			{
				return "Please write the arguments in the correct format. Correct format is: 'toggleDisableDying [index]' or just 'toggleDisableDying' for making all agents invincible.";
			}
			if (Mission.Current == null)
			{
				return "No active mission found";
			}
			if (strings.Count == 0 || num == -1)
			{
				Mission.Current.DisableDying = !Mission.Current.DisableDying;
				if (Mission.Current.DisableDying)
				{
					return "Dying disabled for all";
				}
				return "Dying not disabled for all";
			}
			else
			{
				Agent agent = Mission.Current.FindAgentWithIndex(num);
				if (agent != null)
				{
					agent.ToggleInvulnerable();
					return "Disable Dying for agent " + num.ToString() + ": " + (agent.CurrentMortalityState == Agent.MortalityState.Invulnerable).ToString();
				}
				return "Invalid agent index " + num.ToString();
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("toggleDisableDyingTeam", "mission")]
		public static string ToggleDisableDyingTeam(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			int num = 0;
			if (strings.Count > 0 && !int.TryParse(strings[0], out num))
			{
				return "Please write the arguments in the correct format. Correct format is: 'toggleDisableDyingTeam [team_no]' for making all active agents of a team invincible.";
			}
			int num2 = 0;
			foreach (Agent agent in Mission.Current.AllAgents)
			{
				if (agent.Team != null && agent.Team.MBTeam.Index == num)
				{
					agent.ToggleInvulnerable();
					num2++;
				}
			}
			return string.Concat(new object[]
			{
				"Toggled invulnerability for active agents of team ",
				num.ToString(),
				", agent count: ",
				num2
			});
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("killAgent", "mission")]
		public static string KillAgent(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (Mission.Current == null)
			{
				return "Current mission does not exist.";
			}
			int num;
			if (strings.Count == 0 || !int.TryParse(strings[0], out num))
			{
				return "Please write the arguments in the correct format. Correct format is: 'killAgent [index]'";
			}
			Agent agent = Mission.Current.FindAgentWithIndex(num);
			if (agent == null)
			{
				return "Agent " + num.ToString() + " not found.";
			}
			if (agent.State == AgentState.Active)
			{
				Mission.Current.KillAgentCheat(agent);
				return "Agent " + num.ToString() + " died.";
			}
			return "Agent " + num.ToString() + " already dead.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_battering_ram_speed", "mission")]
		public static string IncreaseBatteringRamSpeeds(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			float num;
			if (strings.Count == 0 || !float.TryParse(strings[0], out num))
			{
				return "Please enter a speed value";
			}
			foreach (MissionObject missionObject in Mission.Current.ActiveMissionObjects)
			{
				if (missionObject.GameEntity.HasScriptOfType<BatteringRam>())
				{
					missionObject.GameEntity.GetFirstScriptOfType<BatteringRam>().MovementComponent.MaxSpeed = num;
					missionObject.GameEntity.GetFirstScriptOfType<BatteringRam>().MovementComponent.MinSpeed = num;
				}
			}
			return "Battering ram max speed increased.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_siege_tower_speed", "mission")]
		public static string IncreaseSiegeTowerSpeed(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			float num;
			if (strings.Count == 0 || !float.TryParse(strings[0], out num))
			{
				return "Please enter a speed value";
			}
			foreach (MissionObject missionObject in Mission.Current.ActiveMissionObjects)
			{
				if (missionObject.GameEntity.HasScriptOfType<SiegeTower>())
				{
					missionObject.GameEntity.GetFirstScriptOfType<SiegeTower>().MovementComponent.MaxSpeed = num;
				}
			}
			return "Siege tower max speed increased.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("reload_managed_core_params", "game")]
		public static string LoadParamsDebug(List<string> strings)
		{
			if (!GameNetwork.IsSessionActive)
			{
				ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_core_parameters"));
				return "Managed core parameters reloaded.";
			}
			return "Does not work on multiplayer.";
		}

		public const int MaxRuntimeMissionObjects = 4095;

		private int _lastSceneMissionObjectIdCount;

		private int _lastRuntimeMissionObjectIdCount;

		private bool _isMainAgentObjectInteractionEnabled = true;

		private List<Mission.TimeSpeedRequest> _timeSpeedRequests = new List<Mission.TimeSpeedRequest>();

		private bool _isMainAgentItemInteractionEnabled = true;

		private readonly MBList<MissionObject> _activeMissionObjects;

		private readonly MBList<MissionObject> _missionObjects;

		private readonly List<SpawnedItemEntity> _spawnedItemEntitiesCreatedAtRuntime;

		private readonly MBList<Mission.DynamicallyCreatedEntity> _addedEntitiesInfo;

		private readonly Stack<ValueTuple<int, float>> _emptyRuntimeMissionObjectIds;

		private static bool _isCameraFirstPerson = false;

		private MissionMode _missionMode;

		private float _cachedMissionTime;

		private static readonly object GetNearbyAgentsAuxLock = new object();

		private const float NavigationMeshHeightLimit = 1.5f;

		private const float SpeedBonusFactorForSwing = 0.7f;

		private const float SpeedBonusFactorForThrust = 0.5f;

		private const float _exitTimeInSeconds = 0.6f;

		public const int MaxNavMeshId = 1000000;

		private const int MaxNavMeshPerDynamicObject = 10;

		private bool _missionEnded;

		private Dictionary<int, Mission.Missile> _missiles;

		private readonly List<Mission.DynamicEntityInfo> _dynamicEntities = new List<Mission.DynamicEntityInfo>();

		public bool DisableDying;

		public bool ForceNoFriendlyFire;

		private int _nextDynamicNavMeshIdStart = 1000010;

		public bool IsFriendlyMission = true;

		public const int MaxDamage = 2000;

		public BasicCultureObject MusicCulture;

		private List<IMissionListener> _listeners = new List<IMissionListener>();

		private MissionState _missionState;

		private MissionDeploymentPlan _deploymentPlan;

		private List<MissionBehavior> _otherMissionBehaviors;

		private readonly object _lockHelper = new object();

		private MBList<Agent> _activeAgents;

		private BasicMissionTimer _leaveMissionTimer;

		private readonly MBList<KeyValuePair<Agent, MissionTime>> _mountsWithoutRiders;

		public bool IsOrderMenuOpen;

		public bool IsTransferMenuOpen;

		public bool IsInPhotoMode;

		private Agent _mainAgent;

		private Action _onLoadingEndedAction;

		private Timer _inMissionLoadingScreenTimer;

		public bool AllowAiTicking = true;

		private int _agentCreationIndex;

		private readonly MBList<FleePosition>[] _fleePositions = new MBList<FleePosition>[3];

		private bool _doesMissionRequireCivilianEquipment;

		public IAgentVisualCreator AgentVisualCreator;

		private readonly int[] _initialAgentCountPerSide = new int[2];

		private readonly int[] _removedAgentCountPerSide = new int[2];

		private ConcurrentQueue<CombatLogData> _combatLogsCreated = new ConcurrentQueue<CombatLogData>();

		private MBList<Agent> _allAgents;

		private List<SiegeWeapon> _attackerWeaponsForFriendlyFirePreventing = new List<SiegeWeapon>();

		private float _missionEndTime;

		public float MissionCloseTimeAfterFinish = 30f;

		private static Mission _current = null;

		public float NextCheckTimeEndMission = 10f;

		public int NumOfFormationsSpawnedTeamOne;

		private SoundEvent _ambientSoundEvent;

		private readonly BattleSpawnPathSelector _battleSpawnPathSelector;

		private int _agentCount;

		public int NumOfFormationsSpawnedTeamTwo;

		private bool tickCompleted = true;

		public class MBBoundaryCollection : IDictionary<string, ICollection<Vec2>>, ICollection<KeyValuePair<string, ICollection<Vec2>>>, IEnumerable<KeyValuePair<string, ICollection<Vec2>>>, IEnumerable, INotifyCollectionChanged
		{
			IEnumerator IEnumerable.GetEnumerator()
			{
				int count = this.Count;
				int num;
				for (int i = 0; i < count; i = num + 1)
				{
					string boundaryName = MBAPI.IMBMission.GetBoundaryName(this._mission.Pointer, i);
					List<Vec2> boundaryPoints = this.GetBoundaryPoints(boundaryName);
					yield return new KeyValuePair<string, ICollection<Vec2>>(boundaryName, boundaryPoints);
					num = i;
				}
				yield break;
			}

			public IEnumerator<KeyValuePair<string, ICollection<Vec2>>> GetEnumerator()
			{
				int count = this.Count;
				int num;
				for (int i = 0; i < count; i = num + 1)
				{
					string boundaryName = MBAPI.IMBMission.GetBoundaryName(this._mission.Pointer, i);
					List<Vec2> boundaryPoints = this.GetBoundaryPoints(boundaryName);
					yield return new KeyValuePair<string, ICollection<Vec2>>(boundaryName, boundaryPoints);
					num = i;
				}
				yield break;
			}

			public int Count
			{
				get
				{
					return MBAPI.IMBMission.GetBoundaryCount(this._mission.Pointer);
				}
			}

			public float GetBoundaryRadius(string name)
			{
				return MBAPI.IMBMission.GetBoundaryRadius(this._mission.Pointer, name);
			}

			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public void GetOrientedBoundariesBox(out Vec2 boxMinimum, out Vec2 boxMaximum, float rotationInRadians = 0f)
			{
				Vec2 side = Vec2.Side;
				side.RotateCCW(rotationInRadians);
				Vec2 vec = side.LeftVec();
				boxMinimum = new Vec2(float.MaxValue, float.MaxValue);
				boxMaximum = new Vec2(float.MinValue, float.MinValue);
				foreach (ICollection<Vec2> collection in this.Values)
				{
					foreach (Vec2 vec2 in collection)
					{
						float num = Vec2.DotProduct(vec2, side);
						float num2 = Vec2.DotProduct(vec2, vec);
						boxMinimum.x = ((num < boxMinimum.x) ? num : boxMinimum.x);
						boxMinimum.y = ((num2 < boxMinimum.y) ? num2 : boxMinimum.y);
						boxMaximum.x = ((num > boxMaximum.x) ? num : boxMaximum.x);
						boxMaximum.y = ((num2 > boxMaximum.y) ? num2 : boxMaximum.y);
					}
				}
			}

			internal MBBoundaryCollection(Mission mission)
			{
				this._mission = mission;
			}

			public void Add(KeyValuePair<string, ICollection<Vec2>> item)
			{
				this.Add(item.Key, item.Value);
			}

			public void Clear()
			{
				foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
				{
					this.Remove(keyValuePair.Key);
				}
			}

			public bool Contains(KeyValuePair<string, ICollection<Vec2>> item)
			{
				return this.ContainsKey(item.Key);
			}

			public void CopyTo(KeyValuePair<string, ICollection<Vec2>>[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}
				foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
				{
					array[arrayIndex] = keyValuePair;
					arrayIndex++;
					if (arrayIndex >= array.Length)
					{
						throw new ArgumentException("Not enough size in array.");
					}
				}
			}

			public bool Remove(KeyValuePair<string, ICollection<Vec2>> item)
			{
				return this.Remove(item.Key);
			}

			public ICollection<string> Keys
			{
				get
				{
					List<string> list = new List<string>();
					foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
					{
						list.Add(keyValuePair.Key);
					}
					return list;
				}
			}

			public ICollection<ICollection<Vec2>> Values
			{
				get
				{
					List<ICollection<Vec2>> list = new List<ICollection<Vec2>>();
					foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
					{
						list.Add(keyValuePair.Value);
					}
					return list;
				}
			}

			public ICollection<Vec2> this[string name]
			{
				get
				{
					if (name == null)
					{
						throw new ArgumentNullException("name");
					}
					List<Vec2> boundaryPoints = this.GetBoundaryPoints(name);
					if (boundaryPoints.Count == 0)
					{
						throw new KeyNotFoundException();
					}
					return boundaryPoints;
				}
				set
				{
					if (name == null)
					{
						throw new ArgumentNullException("name");
					}
					this.Add(name, value);
				}
			}

			public void Add(string name, ICollection<Vec2> points)
			{
				this.Add(name, points, true);
			}

			public void Add(string name, ICollection<Vec2> points, bool isAllowanceInside)
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				if (points == null)
				{
					throw new ArgumentNullException("points");
				}
				if (points.Count < 3)
				{
					throw new ArgumentException("At least three points are required.");
				}
				bool flag = MBAPI.IMBMission.AddBoundary(this._mission.Pointer, name, points.ToArray<Vec2>(), points.Count, isAllowanceInside);
				if (!flag)
				{
					throw new ArgumentException("An element with the same name already exists.");
				}
				if (flag)
				{
					NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
					if (collectionChanged != null)
					{
						collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, name));
					}
				}
				foreach (Team team in Mission.Current.Teams)
				{
					foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
					{
						formation.ResetMovementOrderPositionCache();
					}
				}
			}

			public bool ContainsKey(string name)
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				return this.GetBoundaryPoints(name).Count > 0;
			}

			public bool Remove(string name)
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				bool flag = MBAPI.IMBMission.RemoveBoundary(this._mission.Pointer, name);
				if (flag)
				{
					NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
					if (collectionChanged != null)
					{
						collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, name));
					}
				}
				foreach (Team team in Mission.Current.Teams)
				{
					foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
					{
						formation.ResetMovementOrderPositionCache();
					}
				}
				return flag;
			}

			public bool TryGetValue(string name, out ICollection<Vec2> points)
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				points = this.GetBoundaryPoints(name);
				return points.Count > 0;
			}

			private List<Vec2> GetBoundaryPoints(string name)
			{
				List<Vec2> list = new List<Vec2>();
				Vec2[] array = new Vec2[10];
				for (int i = 0; i < 1000; i += 10)
				{
					int num = -1;
					MBAPI.IMBMission.GetBoundaryPoints(this._mission.Pointer, name, i, array, 10, ref num);
					list.AddRange(array.Take(num));
					if (num < 10)
					{
						break;
					}
				}
				return list;
			}

			public event NotifyCollectionChangedEventHandler CollectionChanged;

			private readonly Mission _mission;
		}

		public class DynamicallyCreatedEntity
		{
			public DynamicallyCreatedEntity(string prefab, MissionObjectId objectId, MatrixFrame frame, ref List<MissionObjectId> childObjectIds)
			{
				this.Prefab = prefab;
				this.ObjectId = objectId;
				this.Frame = frame;
				this.ChildObjectIds = childObjectIds;
			}

			public string Prefab;

			public MissionObjectId ObjectId;

			public MatrixFrame Frame;

			public List<MissionObjectId> ChildObjectIds;
		}

		[Flags]
		[EngineStruct("Weapon_spawn_flag", false)]
		public enum WeaponSpawnFlags : uint
		{
			None = 0U,
			WithHolster = 1U,
			WithoutHolster = 2U,
			AsMissile = 4U,
			WithPhysics = 8U,
			WithStaticPhysics = 16U,
			UseAnimationSpeed = 32U,
			CannotBePickedUp = 64U
		}

		[EngineStruct("Mission_combat_type", false)]
		public enum MissionCombatType
		{
			Combat,
			ArenaCombat,
			NoCombat
		}

		public enum BattleSizeType
		{
			Battle,
			Siege,
			SallyOut
		}

		[EngineStruct("Agent_creation_result", false)]
		internal struct AgentCreationResult
		{
			internal int Index;

			internal UIntPtr AgentPtr;

			internal UIntPtr PositionPtr;

			internal UIntPtr IndexPtr;

			internal UIntPtr FlagsPtr;

			internal UIntPtr StatePtr;
		}

		public struct TimeSpeedRequest
		{
			public float RequestedTimeSpeed { get; private set; }

			public int RequestID { get; private set; }

			public TimeSpeedRequest(float requestedTime, int requestID)
			{
				this.RequestedTimeSpeed = requestedTime;
				this.RequestID = requestID;
			}
		}

		private enum GetNearbyAgentsAuxType
		{
			Friend = 1,
			Enemy,
			All
		}

		public static class MissionNetworkHelper
		{
			public static Agent GetAgentFromIndex(int agentIndex, bool canBeNull = false)
			{
				Agent agent = Mission.Current.FindAgentWithIndex(agentIndex);
				if (!canBeNull && agent == null && agentIndex >= 0)
				{
					Debug.Print("Agent with index: " + agentIndex + " could not be found while reading reference from packet.", 0, Debug.DebugColor.White, 17592186044416UL);
					throw new MBNotFoundException("Agent with index: " + agentIndex + " could not be found while reading reference from packet.");
				}
				return agent;
			}

			public static MBTeam GetMBTeamFromTeamIndex(int teamIndex)
			{
				if (Mission.Current == null)
				{
					throw new Exception("Mission.Current is null!");
				}
				if (teamIndex < 0)
				{
					return MBTeam.InvalidTeam;
				}
				return new MBTeam(Mission.Current, teamIndex);
			}

			public static Team GetTeamFromTeamIndex(int teamIndex)
			{
				if (Mission.Current == null)
				{
					throw new Exception("Mission.Current is null!");
				}
				if (teamIndex < 0)
				{
					return Team.Invalid;
				}
				MBTeam mbteamFromTeamIndex = Mission.MissionNetworkHelper.GetMBTeamFromTeamIndex(teamIndex);
				return Mission.Current.Teams.Find(mbteamFromTeamIndex);
			}

			public static MissionObject GetMissionObjectFromMissionObjectId(MissionObjectId missionObjectId)
			{
				if (Mission.Current == null)
				{
					throw new Exception("Mission.Current is null!");
				}
				if (missionObjectId.Id < 0)
				{
					return null;
				}
				MissionObject missionObject = Mission.Current.MissionObjects.FirstOrDefault((MissionObject mo) => mo.Id == missionObjectId);
				if (missionObject == null)
				{
					MBDebug.Print(string.Concat(new object[]
					{
						"MissionObject with ID: ",
						missionObjectId.Id,
						" runtime: ",
						missionObjectId.CreatedAtRuntime.ToString(),
						" could not be found."
					}), 0, Debug.DebugColor.White, 17592186044416UL);
				}
				return missionObject;
			}

			public static CombatLogData GetCombatLogDataForCombatLogNetworkMessage(CombatLogNetworkMessage message)
			{
				if (Mission.Current == null)
				{
					throw new Exception("Mission.Current is null!");
				}
				Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(message.AttackerAgentIndex, false);
				Agent agentFromIndex2 = Mission.MissionNetworkHelper.GetAgentFromIndex(message.VictimAgentIndex, true);
				bool flag = agentFromIndex != null;
				bool flag2 = flag && agentFromIndex.IsHuman;
				bool flag3 = flag && agentFromIndex.IsMine;
				bool flag4 = flag && agentFromIndex.RiderAgent != null;
				bool flag5 = flag4 && agentFromIndex.RiderAgent.IsMine;
				bool flag6 = flag && agentFromIndex.IsMount;
				bool flag7 = agentFromIndex2 != null && agentFromIndex2.Health <= 0f;
				bool flag8 = agentFromIndex != null && ((agentFromIndex2 != null) ? agentFromIndex2.RiderAgent : null) == agentFromIndex;
				bool flag9 = agentFromIndex == agentFromIndex2;
				bool flag10 = flag2;
				bool flag11 = flag3;
				bool flag12 = flag4;
				bool flag13 = flag5;
				bool flag14 = flag6;
				bool flag15 = agentFromIndex2 != null && agentFromIndex2.IsHuman;
				bool flag16 = agentFromIndex2 != null && agentFromIndex2.IsMine;
				bool flag17 = flag7;
				bool flag18 = ((agentFromIndex2 != null) ? agentFromIndex2.RiderAgent : null) != null;
				bool? flag19;
				if (agentFromIndex2 == null)
				{
					flag19 = null;
				}
				else
				{
					Agent riderAgent = agentFromIndex2.RiderAgent;
					flag19 = ((riderAgent != null) ? new bool?(riderAgent.IsMine) : null);
				}
				CombatLogData combatLogData = new CombatLogData(flag9, flag10, flag11, flag12, flag13, flag14, flag15, flag16, flag17, flag18, flag19 ?? false, agentFromIndex2 != null && agentFromIndex2.IsMount, message.IsVictimEntity, flag8, message.CrushedThrough, message.Chamber, message.Distance);
				combatLogData.DamageType = message.DamageType;
				combatLogData.IsRangedAttack = message.IsRangedAttack;
				combatLogData.IsFriendlyFire = message.IsFriendlyFire;
				combatLogData.IsFatalDamage = message.IsFatalDamage;
				combatLogData.BodyPartHit = message.BodyPartHit;
				combatLogData.HitSpeed = message.HitSpeed;
				combatLogData.InflictedDamage = message.InflictedDamage;
				combatLogData.AbsorbedDamage = message.AbsorbedDamage;
				combatLogData.ModifiedDamage = message.ModifiedDamage;
				string text;
				if (agentFromIndex2 == null)
				{
					text = null;
				}
				else
				{
					MissionPeer missionPeer = agentFromIndex2.MissionPeer;
					text = ((missionPeer != null) ? missionPeer.DisplayedName : null);
				}
				string text2;
				if ((text2 = text) == null)
				{
					text2 = ((agentFromIndex2 != null) ? agentFromIndex2.Name : null) ?? "";
				}
				combatLogData.VictimAgentName = text2;
				return combatLogData;
			}
		}

		public class Missile : MBMissile
		{
			public Missile(Mission mission, GameEntity entity)
				: base(mission)
			{
				this.Entity = entity;
			}

			public GameEntity Entity { get; private set; }

			public MissionWeapon Weapon { get; set; }

			public Agent ShooterAgent { get; set; }

			public MissionObject MissionObjectToIgnore { get; set; }

			public void CalculatePassbySoundParametersMT(ref SoundEventParameter soundEventParameter)
			{
				if (this.Weapon.CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.CanPenetrateShield))
				{
					soundEventParameter.Update("impactModifier", 0.3f);
				}
			}

			public void CalculateBounceBackVelocity(Vec3 rotationSpeed, AttackCollisionData collisionData, out Vec3 velocity, out Vec3 angularVelocity)
			{
				Vec3 missileVelocity = collisionData.MissileVelocity;
				float num = (float)this.Weapon.CurrentUsageItem.WeaponLength * 0.01f * this.Weapon.Item.ScaleFactor;
				PhysicsMaterial fromIndex = PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex);
				float num2;
				float num3;
				if (fromIndex.IsValid)
				{
					num2 = fromIndex.GetDynamicFriction();
					num3 = fromIndex.GetRestitution();
				}
				else
				{
					num2 = 0.3f;
					num3 = 0.4f;
				}
				PhysicsMaterial fromName = PhysicsMaterial.GetFromName(this.Weapon.Item.PrimaryWeapon.PhysicsMaterial);
				float num4;
				float num5;
				if (fromName.IsValid)
				{
					num4 = fromName.GetDynamicFriction();
					num5 = fromName.GetRestitution();
				}
				else
				{
					num4 = 0.3f;
					num5 = 0.4f;
				}
				float num6 = (num2 + num4) * 0.5f;
				float num7 = (num3 + num5) * 0.5f;
				Vec3 vec = missileVelocity.Reflect(collisionData.CollisionGlobalNormal);
				float num8 = Vec3.DotProduct(vec, collisionData.CollisionGlobalNormal);
				Vec3 vec2 = collisionData.CollisionGlobalNormal.RotateAboutAnArbitraryVector(Vec3.CrossProduct(vec, collisionData.CollisionGlobalNormal).NormalizedCopy(), 1.5707964f);
				float num9 = Vec3.DotProduct(vec, vec2);
				velocity = collisionData.CollisionGlobalNormal * (num7 * num8) + vec2 * (num9 * num6);
				velocity += collisionData.CollisionGlobalNormal;
				angularVelocity = -Vec3.CrossProduct(collisionData.CollisionGlobalNormal, velocity);
				float lengthSquared = angularVelocity.LengthSquared;
				float weight = this.Weapon.GetWeight();
				WeaponClass weaponClass = this.Weapon.CurrentUsageItem.WeaponClass;
				float num10;
				if (weaponClass == WeaponClass.Arrow || weaponClass == WeaponClass.Bolt)
				{
					num10 = 0.25f * weight * 0.055f * 0.055f + 0.08333333f * weight * num * num;
				}
				else if (weaponClass == WeaponClass.ThrowingKnife)
				{
					num10 = 0.25f * weight * 0.2f * 0.2f + 0.08333333f * weight * num * num;
					num10 += 0.5f * weight * 0.2f * 0.2f;
					rotationSpeed * num3;
					MatrixFrame matrixFrame = this.Entity.GetGlobalFrame();
					angularVelocity = matrixFrame.rotation.TransformToParent(rotationSpeed * num3);
				}
				else if (weaponClass == WeaponClass.ThrowingAxe)
				{
					num10 = 0.25f * weight * 0.2f * 0.2f + 0.08333333f * weight * num * num;
					num10 += 0.5f * weight * 0.2f * 0.2f;
					rotationSpeed * num3;
					MatrixFrame matrixFrame = this.Entity.GetGlobalFrame();
					angularVelocity = matrixFrame.rotation.TransformToParent(rotationSpeed * num3);
				}
				else if (weaponClass == WeaponClass.Javelin)
				{
					num10 = 0.25f * weight * 0.155f * 0.155f + 0.08333333f * weight * num * num;
				}
				else if (weaponClass == WeaponClass.Stone)
				{
					num10 = 0.4f * weight * 0.1f * 0.1f;
				}
				else if (weaponClass == WeaponClass.Boulder)
				{
					num10 = 0.4f * weight * 0.4f * 0.4f;
				}
				else
				{
					Debug.FailedAssert("Unknown missile type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "CalculateBounceBackVelocity", 266);
					num10 = 0f;
				}
				float num11 = 0.5f * num10 * lengthSquared;
				float length = missileVelocity.Length;
				float num12 = MathF.Sqrt((0.5f * weight * length * length - num11) * 2f / weight);
				velocity *= num12 / length;
				float maximumValue = CompressionMission.SpawnedItemVelocityCompressionInfo.GetMaximumValue();
				float maximumValue2 = CompressionMission.SpawnedItemAngularVelocityCompressionInfo.GetMaximumValue();
				if (velocity.LengthSquared > maximumValue * maximumValue)
				{
					velocity = velocity.NormalizedCopy() * maximumValue;
				}
				if (angularVelocity.LengthSquared > maximumValue2 * maximumValue2)
				{
					angularVelocity = angularVelocity.NormalizedCopy() * maximumValue2;
				}
			}
		}

		public struct SpectatorData
		{
			public Agent AgentToFollow { get; private set; }

			public IAgentVisual AgentVisualToFollow { get; private set; }

			public SpectatorCameraTypes CameraType { get; private set; }

			public SpectatorData(Agent agentToFollow, IAgentVisual agentVisualToFollow, SpectatorCameraTypes cameraType)
			{
				this.AgentToFollow = agentToFollow;
				this.CameraType = cameraType;
				this.AgentVisualToFollow = agentVisualToFollow;
			}
		}

		public enum State
		{
			NewlyCreated,
			Initializing,
			Continuing,
			EndingNextFrame,
			Over
		}

		private class DynamicEntityInfo
		{
			public GameEntity Entity;

			public Timer TimerToDisable;
		}

		public enum BattleSizeQualifier
		{
			Small,
			Medium
		}

		public enum MissionTeamAITypeEnum
		{
			NoTeamAI,
			FieldBattle,
			Siege,
			SallyOut
		}

		public enum MissileCollisionReaction
		{
			Invalid = -1,
			Stick,
			PassThrough,
			BounceBack,
			BecomeInvisible,
			Count
		}

		public delegate void OnBeforeAgentRemovedDelegate(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow);

		public sealed class TeamCollection : List<Team>
		{
			public event Action<Team, Team> OnPlayerTeamChanged;

			public Team Attacker { get; private set; }

			public Team Defender { get; private set; }

			public Team AttackerAlly { get; private set; }

			public Team DefenderAlly { get; private set; }

			public Team Player
			{
				get
				{
					return this._playerTeam;
				}
				set
				{
					if (this._playerTeam != value)
					{
						this.SetPlayerTeamAux((value == null) ? (-1) : base.IndexOf(value));
					}
				}
			}

			public Team PlayerEnemy { get; private set; }

			public Team PlayerAlly { get; private set; }

			public TeamCollection(Mission mission)
				: base(new List<Team>())
			{
				this._mission = mission;
			}

			private MBTeam AddNative()
			{
				return new MBTeam(this._mission, MBAPI.IMBMission.AddTeam(this._mission.Pointer));
			}

			public new void Add(Team t)
			{
				MBDebug.ShowWarning("Pre-created Team can not be added to TeamCollection!");
			}

			public Team Add(BattleSideEnum side, uint color = 4294967295U, uint color2 = 4294967295U, Banner banner = null, bool isPlayerGeneral = true, bool isPlayerSergeant = false, bool isSettingRelations = true)
			{
				MBDebug.Print("----------Mission-AddTeam-" + side, 0, Debug.DebugColor.White, 17592186044416UL);
				Team team = new Team(this.AddNative(), side, this._mission, color, color2, banner);
				if (!GameNetwork.IsClientOrReplay)
				{
					team.SetPlayerRole(isPlayerGeneral, isPlayerSergeant);
				}
				base.Add(team);
				foreach (MissionBehavior missionBehavior in this._mission.MissionBehaviors)
				{
					missionBehavior.OnAddTeam(team);
				}
				if (isSettingRelations)
				{
					this.SetRelations(team);
				}
				if (side == BattleSideEnum.Attacker)
				{
					if (this.Attacker == null)
					{
						this.Attacker = team;
					}
					else if (this.AttackerAlly == null)
					{
						this.AttackerAlly = team;
					}
				}
				else if (side == BattleSideEnum.Defender)
				{
					if (this.Defender == null)
					{
						this.Defender = team;
					}
					else if (this.DefenderAlly == null)
					{
						this.DefenderAlly = team;
					}
				}
				this.AdjustPlayerTeams();
				foreach (MissionBehavior missionBehavior2 in this._mission.MissionBehaviors)
				{
					missionBehavior2.AfterAddTeam(team);
				}
				return team;
			}

			public Team Find(MBTeam mbTeam)
			{
				if (mbTeam.IsValid)
				{
					for (int i = 0; i < base.Count; i++)
					{
						Team team = base[i];
						if (team.MBTeam == mbTeam)
						{
							return team;
						}
					}
				}
				return Team.Invalid;
			}

			public void ClearResources()
			{
				this.Attacker = null;
				this.AttackerAlly = null;
				this.Defender = null;
				this.DefenderAlly = null;
				this._playerTeam = null;
				this.PlayerEnemy = null;
				this.PlayerAlly = null;
				Team.Invalid = null;
			}

			public new void Clear()
			{
				foreach (Team team in this)
				{
					team.Clear();
				}
				base.Clear();
				this.ClearResources();
				MBAPI.IMBMission.ResetTeams(this._mission.Pointer);
			}

			private void SetRelations(Team team)
			{
				BattleSideEnum side = team.Side;
				for (int i = 0; i < base.Count; i++)
				{
					Team team2 = base[i];
					if (side.IsOpponentOf(team2.Side))
					{
						team.SetIsEnemyOf(team2, true);
					}
				}
			}

			private void SetPlayerTeamAux(int index)
			{
				Team playerTeam = this._playerTeam;
				this._playerTeam = ((index == -1) ? null : base[index]);
				this.AdjustPlayerTeams();
				Action<Team, Team> onPlayerTeamChanged = this.OnPlayerTeamChanged;
				if (onPlayerTeamChanged == null)
				{
					return;
				}
				onPlayerTeamChanged(playerTeam, this._playerTeam);
			}

			private void AdjustPlayerTeams()
			{
				if (this.Player == null)
				{
					this.PlayerEnemy = null;
					this.PlayerAlly = null;
					return;
				}
				if (this.Player != this.Attacker)
				{
					if (this.Player == this.Defender)
					{
						if (this.Attacker != null && this.Player.IsEnemyOf(this.Attacker))
						{
							this.PlayerEnemy = this.Attacker;
						}
						else
						{
							this.PlayerEnemy = null;
						}
						if (this.DefenderAlly != null && this.Player.IsFriendOf(this.DefenderAlly))
						{
							this.PlayerAlly = this.DefenderAlly;
							return;
						}
						this.PlayerAlly = null;
					}
					return;
				}
				if (this.Defender != null && this.Player.IsEnemyOf(this.Defender))
				{
					this.PlayerEnemy = this.Defender;
				}
				else
				{
					this.PlayerEnemy = null;
				}
				if (this.AttackerAlly != null && this.Player.IsFriendOf(this.AttackerAlly))
				{
					this.PlayerAlly = this.AttackerAlly;
					return;
				}
				this.PlayerAlly = null;
			}

			private int TeamCountNative
			{
				get
				{
					return MBAPI.IMBMission.GetNumberOfTeams(this._mission.Pointer);
				}
			}

			private Mission _mission;

			private Team _playerTeam;
		}
	}
}
