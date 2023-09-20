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
	// Token: 0x020001AB RID: 427
	public sealed class Mission : DotNetObject, IMission
	{
		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x0600174E RID: 5966 RVA: 0x0004FCCF File Offset: 0x0004DECF
		// (set) Token: 0x0600174F RID: 5967 RVA: 0x0004FCD7 File Offset: 0x0004DED7
		internal UIntPtr Pointer { get; private set; }

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06001750 RID: 5968 RVA: 0x0004FCE0 File Offset: 0x0004DEE0
		public bool IsFinalized
		{
			get
			{
				return this.Pointer == UIntPtr.Zero;
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06001751 RID: 5969 RVA: 0x0004FCF2 File Offset: 0x0004DEF2
		// (set) Token: 0x06001752 RID: 5970 RVA: 0x0004FCFF File Offset: 0x0004DEFF
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

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06001753 RID: 5971 RVA: 0x0004FD10 File Offset: 0x0004DF10
		// (set) Token: 0x06001754 RID: 5972 RVA: 0x0004FD18 File Offset: 0x0004DF18
		private MissionInitializerRecord InitializerRecord { get; set; }

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06001755 RID: 5973 RVA: 0x0004FD21 File Offset: 0x0004DF21
		public string SceneName
		{
			get
			{
				return this.InitializerRecord.SceneName;
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06001756 RID: 5974 RVA: 0x0004FD2E File Offset: 0x0004DF2E
		public string SceneLevels
		{
			get
			{
				return this.InitializerRecord.SceneLevels;
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06001757 RID: 5975 RVA: 0x0004FD3B File Offset: 0x0004DF3B
		public float DamageToPlayerMultiplier
		{
			get
			{
				return this.InitializerRecord.DamageToPlayerMultiplier;
			}
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06001758 RID: 5976 RVA: 0x0004FD48 File Offset: 0x0004DF48
		public float DamageToFriendsMultiplier
		{
			get
			{
				return this.InitializerRecord.DamageToFriendsMultiplier;
			}
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06001759 RID: 5977 RVA: 0x0004FD55 File Offset: 0x0004DF55
		public float DamageFromPlayerToFriendsMultiplier
		{
			get
			{
				return this.InitializerRecord.DamageFromPlayerToFriendsMultiplier;
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x0600175A RID: 5978 RVA: 0x0004FD62 File Offset: 0x0004DF62
		public bool HasValidTerrainType
		{
			get
			{
				return this.InitializerRecord.TerrainType >= 0;
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x0600175B RID: 5979 RVA: 0x0004FD75 File Offset: 0x0004DF75
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

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x0600175C RID: 5980 RVA: 0x0004FD8C File Offset: 0x0004DF8C
		// (set) Token: 0x0600175D RID: 5981 RVA: 0x0004FD94 File Offset: 0x0004DF94
		public Scene Scene { get; private set; }

		// Token: 0x0600175E RID: 5982 RVA: 0x0004FDA0 File Offset: 0x0004DFA0
		public IEnumerable<GameEntity> GetActiveEntitiesWithScriptComponentOfType<T>()
		{
			return from amo in this._activeMissionObjects
				where amo is T
				select amo.GameEntity;
		}

		// Token: 0x0600175F RID: 5983 RVA: 0x0004FDFB File Offset: 0x0004DFFB
		public void AddActiveMissionObject(MissionObject missionObject)
		{
			this._missionObjects.Add(missionObject);
			this._activeMissionObjects.Add(missionObject);
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x0004FE15 File Offset: 0x0004E015
		public void ActivateMissionObject(MissionObject missionObject)
		{
			this._activeMissionObjects.Add(missionObject);
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x0004FE23 File Offset: 0x0004E023
		public void DeactivateMissionObject(MissionObject missionObject)
		{
			this._activeMissionObjects.Remove(missionObject);
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06001762 RID: 5986 RVA: 0x0004FE32 File Offset: 0x0004E032
		public MBReadOnlyList<MissionObject> ActiveMissionObjects
		{
			get
			{
				return this._activeMissionObjects;
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06001763 RID: 5987 RVA: 0x0004FE3A File Offset: 0x0004E03A
		public MBReadOnlyList<MissionObject> MissionObjects
		{
			get
			{
				return this._missionObjects;
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06001764 RID: 5988 RVA: 0x0004FE42 File Offset: 0x0004E042
		public MBReadOnlyList<Mission.DynamicallyCreatedEntity> AddedEntitiesInfo
		{
			get
			{
				return this._addedEntitiesInfo;
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06001765 RID: 5989 RVA: 0x0004FE4A File Offset: 0x0004E04A
		// (set) Token: 0x06001766 RID: 5990 RVA: 0x0004FE52 File Offset: 0x0004E052
		public Mission.MBBoundaryCollection Boundaries { get; private set; }

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06001767 RID: 5991 RVA: 0x0004FE5C File Offset: 0x0004E05C
		// (set) Token: 0x06001768 RID: 5992 RVA: 0x0004FEA6 File Offset: 0x0004E0A6
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
				return this._isMainAgentObjectInteractionEnabled;
			}
			set
			{
				this._isMainAgentObjectInteractionEnabled = value;
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06001769 RID: 5993 RVA: 0x0004FEB0 File Offset: 0x0004E0B0
		// (set) Token: 0x0600176A RID: 5994 RVA: 0x0004FEFA File Offset: 0x0004E0FA
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

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x0600176B RID: 5995 RVA: 0x0004FF03 File Offset: 0x0004E103
		// (set) Token: 0x0600176C RID: 5996 RVA: 0x0004FF0B File Offset: 0x0004E10B
		public bool IsTeleportingAgents { get; set; }

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x0600176D RID: 5997 RVA: 0x0004FF14 File Offset: 0x0004E114
		// (set) Token: 0x0600176E RID: 5998 RVA: 0x0004FF1C File Offset: 0x0004E11C
		public bool ForceTickOccasionally { get; set; }

		// Token: 0x0600176F RID: 5999 RVA: 0x0004FF25 File Offset: 0x0004E125
		private void FinalizeMission()
		{
			TeamAISiegeComponent.OnMissionFinalize();
			MBAPI.IMBMission.FinalizeMission(this.Pointer);
			this.Pointer = UIntPtr.Zero;
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06001770 RID: 6000 RVA: 0x0004FF47 File Offset: 0x0004E147
		// (set) Token: 0x06001771 RID: 6001 RVA: 0x0004FF59 File Offset: 0x0004E159
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

		// Token: 0x06001772 RID: 6002 RVA: 0x0004FF6C File Offset: 0x0004E16C
		public void SetMissionCombatType(Mission.MissionCombatType missionCombatType)
		{
			MBAPI.IMBMission.SetCombatType(this.Pointer, (int)missionCombatType);
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06001773 RID: 6003 RVA: 0x0004FF7F File Offset: 0x0004E17F
		public MissionMode Mode
		{
			get
			{
				return this._missionMode;
			}
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x0004FF88 File Offset: 0x0004E188
		public void ConversationCharacterChanged()
		{
			foreach (IMissionListener missionListener in this._listeners)
			{
				missionListener.OnConversationCharacterChanged();
			}
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x0004FFD8 File Offset: 0x0004E1D8
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

		// Token: 0x06001776 RID: 6006 RVA: 0x00050074 File Offset: 0x0004E274
		private Mission.AgentCreationResult CreateAgentInternal(AgentFlag agentFlags, int forcedAgentIndex, bool isFemale, ref AgentSpawnData spawnData, ref AgentCapsuleData capsuleData, ref AnimationSystemData animationSystemData, int instanceNo)
		{
			return MBAPI.IMBMission.CreateAgent(this.Pointer, (ulong)agentFlags, forcedAgentIndex, isFemale, ref spawnData, ref capsuleData.BodyCap, ref capsuleData.CrouchedBodyCap, ref animationSystemData, instanceNo);
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06001777 RID: 6007 RVA: 0x000500A9 File Offset: 0x0004E2A9
		public float CurrentTime
		{
			get
			{
				return this._cachedMissionTime;
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06001778 RID: 6008 RVA: 0x000500B1 File Offset: 0x0004E2B1
		// (set) Token: 0x06001779 RID: 6009 RVA: 0x000500C3 File Offset: 0x0004E2C3
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

		// Token: 0x0600177A RID: 6010 RVA: 0x000500D6 File Offset: 0x0004E2D6
		[UsedImplicitly]
		[MBCallback]
		internal void UpdateMissionTimeCache(float curTime)
		{
			this._cachedMissionTime = curTime;
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x000500DF File Offset: 0x0004E2DF
		public float GetAverageFps()
		{
			return MBAPI.IMBMission.GetAverageFps(this.Pointer);
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x000500F1 File Offset: 0x0004E2F1
		public static bool ToggleDisableFallAvoid()
		{
			return MBAPI.IMBMission.ToggleDisableFallAvoid();
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x000500FD File Offset: 0x0004E2FD
		public bool IsPositionInsideBoundaries(Vec2 position)
		{
			return MBAPI.IMBMission.IsPositionInsideBoundaries(this.Pointer, position);
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x00050110 File Offset: 0x0004E310
		private bool IsFormationUnitPositionAvailableAux(ref WorldPosition formationPosition, ref WorldPosition unitPosition, ref WorldPosition nearestAvailableUnitPosition, float manhattanDistance)
		{
			return MBAPI.IMBMission.IsFormationUnitPositionAvailable(this.Pointer, ref formationPosition, ref unitPosition, ref nearestAvailableUnitPosition, manhattanDistance);
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x00050127 File Offset: 0x0004E327
		public Agent RayCastForClosestAgent(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, int excludedAgentIndex = -1, float rayThickness = 0.01f)
		{
			collisionDistance = float.NaN;
			return MBAPI.IMBMission.RayCastForClosestAgent(this.Pointer, sourcePoint, targetPoint, excludedAgentIndex, ref collisionDistance, rayThickness);
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x00050147 File Offset: 0x0004E347
		public bool RayCastForClosestAgentsLimbs(Vec3 sourcePoint, Vec3 targetPoint, int excludeAgentIndex, out float collisionDistance, ref int agentIndex, ref sbyte boneIndex)
		{
			collisionDistance = float.NaN;
			return MBAPI.IMBMission.RayCastForClosestAgentsLimbs(this.Pointer, sourcePoint, targetPoint, excludeAgentIndex, ref collisionDistance, ref agentIndex, ref boneIndex);
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x0005016A File Offset: 0x0004E36A
		public bool RayCastForClosestAgentsLimbs(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, ref int agentIndex, ref sbyte boneIndex)
		{
			collisionDistance = float.NaN;
			return MBAPI.IMBMission.RayCastForClosestAgentsLimbs(this.Pointer, sourcePoint, targetPoint, -1, ref collisionDistance, ref agentIndex, ref boneIndex);
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x0005018B File Offset: 0x0004E38B
		public bool RayCastForGivenAgentsLimbs(Vec3 sourcePoint, Vec3 rayFinishPoint, int givenAgentIndex, ref float collisionDistance, ref sbyte boneIndex)
		{
			return MBAPI.IMBMission.RayCastForGivenAgentsLimbs(this.Pointer, sourcePoint, rayFinishPoint, givenAgentIndex, ref collisionDistance, ref boneIndex);
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x000501A4 File Offset: 0x0004E3A4
		internal AgentProximityMap.ProximityMapSearchStructInternal ProximityMapBeginSearch(Vec2 searchPos, float searchRadius)
		{
			return MBAPI.IMBMission.ProximityMapBeginSearch(this.Pointer, searchPos, searchRadius);
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x000501B8 File Offset: 0x0004E3B8
		internal float ProximityMapMaxSearchRadius()
		{
			return MBAPI.IMBMission.ProximityMapMaxSearchRadius(this.Pointer);
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x000501CA File Offset: 0x0004E3CA
		public float GetBiggestAgentCollisionPadding()
		{
			return MBAPI.IMBMission.GetBiggestAgentCollisionPadding(this.Pointer);
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x000501DC File Offset: 0x0004E3DC
		public void SetMissionCorpseFadeOutTimeInSeconds(float corpseFadeOutTimeInSeconds)
		{
			MBAPI.IMBMission.SetMissionCorpseFadeOutTimeInSeconds(this.Pointer, corpseFadeOutTimeInSeconds);
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x000501EF File Offset: 0x0004E3EF
		public void SetReportStuckAgentsMode(bool value)
		{
			MBAPI.IMBMission.SetReportStuckAgentsMode(this.Pointer, value);
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x00050204 File Offset: 0x0004E404
		internal void BatchFormationUnitPositions(MBArrayList<Vec2i> orderedPositionIndices, MBArrayList<Vec2> orderedLocalPositions, MBList2D<int> availabilityTable, MBList2D<WorldPosition> globalPositionTable, WorldPosition orderPosition, Vec2 direction, int fileCount, int rankCount)
		{
			MBAPI.IMBMission.BatchFormationUnitPositions(this.Pointer, orderedPositionIndices.RawArray, orderedLocalPositions.RawArray, availabilityTable.RawArray, globalPositionTable.RawArray, orderPosition, direction, fileCount, rankCount);
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x00050242 File Offset: 0x0004E442
		internal void ProximityMapFindNext(ref AgentProximityMap.ProximityMapSearchStructInternal searchStruct)
		{
			MBAPI.IMBMission.ProximityMapFindNext(this.Pointer, ref searchStruct);
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x00050258 File Offset: 0x0004E458
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

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x0600178B RID: 6027 RVA: 0x00050450 File Offset: 0x0004E650
		// (remove) Token: 0x0600178C RID: 6028 RVA: 0x00050488 File Offset: 0x0004E688
		public event PropertyChangedEventHandler OnMissionReset;

		// Token: 0x0600178D RID: 6029 RVA: 0x000504C0 File Offset: 0x0004E6C0
		public void Initialize()
		{
			Mission.Current = this;
			this.CurrentState = Mission.State.Initializing;
			MissionInitializerRecord initializerRecord = this.InitializerRecord;
			MBAPI.IMBMission.InitializeMission(this.Pointer, ref initializerRecord);
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x000504F3 File Offset: 0x0004E6F3
		[UsedImplicitly]
		[MBCallback]
		internal void OnSceneCreated(Scene scene)
		{
			this.Scene = scene;
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x000504FC File Offset: 0x0004E6FC
		[UsedImplicitly]
		[MBCallback]
		internal void TickAgentsAndTeams(float dt)
		{
			this.TickAgentsAndTeamsImp(dt);
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x00050505 File Offset: 0x0004E705
		public void TickAgentsAndTeamsAsync(float dt)
		{
			MBAPI.IMBMission.tickAgentsAndTeamsAsync(this.Pointer, dt);
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x00050518 File Offset: 0x0004E718
		internal void Tick(float dt)
		{
			MBAPI.IMBMission.Tick(this.Pointer, dt);
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x0005052B File Offset: 0x0004E72B
		internal void IdleTick(float dt)
		{
			MBAPI.IMBMission.IdleTick(this.Pointer, dt);
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x0005053E File Offset: 0x0004E73E
		public void MakeSound(int soundIndex, Vec3 position, bool soundCanBePredicted, bool isReliable, int relatedAgent1, int relatedAgent2)
		{
			MBAPI.IMBMission.MakeSound(this.Pointer, soundIndex, position, soundCanBePredicted, isReliable, relatedAgent1, relatedAgent2);
		}

		// Token: 0x06001794 RID: 6036 RVA: 0x0005055C File Offset: 0x0004E75C
		public void MakeSound(int soundIndex, Vec3 position, bool soundCanBePredicted, bool isReliable, int relatedAgent1, int relatedAgent2, ref SoundEventParameter parameter)
		{
			MBAPI.IMBMission.MakeSoundWithParameter(this.Pointer, soundIndex, position, soundCanBePredicted, isReliable, relatedAgent1, relatedAgent2, parameter);
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x00050589 File Offset: 0x0004E789
		public void MakeSoundOnlyOnRelatedPeer(int soundIndex, Vec3 position, int relatedAgent)
		{
			MBAPI.IMBMission.MakeSoundOnlyOnRelatedPeer(this.Pointer, soundIndex, position, relatedAgent);
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x0005059E File Offset: 0x0004E79E
		public void AddSoundAlarmFactorToAgents(int ownerId, Vec3 position, float alarmFactor)
		{
			MBAPI.IMBMission.AddSoundAlarmFactorToAgents(this.Pointer, ownerId, position, alarmFactor);
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x000505B3 File Offset: 0x0004E7B3
		public void AddDynamicallySpawnedMissionObjectInfo(Mission.DynamicallyCreatedEntity entityInfo)
		{
			this._addedEntitiesInfo.Add(entityInfo);
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x000505C4 File Offset: 0x0004E7C4
		private void RemoveDynamicallySpawnedMissionObjectInfo(MissionObjectId id)
		{
			Mission.DynamicallyCreatedEntity dynamicallyCreatedEntity = this._addedEntitiesInfo.FirstOrDefault((Mission.DynamicallyCreatedEntity x) => x.ObjectId == id);
			if (dynamicallyCreatedEntity != null)
			{
				this._addedEntitiesInfo.Remove(dynamicallyCreatedEntity);
			}
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x00050608 File Offset: 0x0004E808
		private int AddMissileAux(int forcedMissileIndex, bool isPrediction, Agent shooterAgent, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, float damageBonus, ref Vec3 position, ref Vec3 direction, ref Mat3 orientation, float baseSpeed, float speed, bool addRigidBody, GameEntity gameEntityToIgnore, bool isPrimaryWeaponShot, out GameEntity missileEntity)
		{
			UIntPtr uintPtr;
			int num = MBAPI.IMBMission.AddMissile(this.Pointer, isPrediction, shooterAgent.Index, weaponData, weaponStatsData, weaponStatsData.Length, damageBonus, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, (gameEntityToIgnore != null) ? gameEntityToIgnore.Pointer : UIntPtr.Zero, forcedMissileIndex, isPrimaryWeaponShot, out uintPtr);
			missileEntity = (isPrediction ? null : new GameEntity(uintPtr));
			return num;
		}

		// Token: 0x0600179A RID: 6042 RVA: 0x00050668 File Offset: 0x0004E868
		private int AddMissileSingleUsageAux(int forcedMissileIndex, bool isPrediction, Agent shooterAgent, in WeaponData weaponData, in WeaponStatsData weaponStatsData, float damageBonus, ref Vec3 position, ref Vec3 direction, ref Mat3 orientation, float baseSpeed, float speed, bool addRigidBody, GameEntity gameEntityToIgnore, bool isPrimaryWeaponShot, out GameEntity missileEntity)
		{
			UIntPtr uintPtr;
			int num = MBAPI.IMBMission.AddMissileSingleUsage(this.Pointer, isPrediction, shooterAgent.Index, weaponData, weaponStatsData, damageBonus, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, (gameEntityToIgnore != null) ? gameEntityToIgnore.Pointer : UIntPtr.Zero, forcedMissileIndex, isPrimaryWeaponShot, out uintPtr);
			missileEntity = (isPrediction ? null : new GameEntity(uintPtr));
			return num;
		}

		// Token: 0x0600179B RID: 6043 RVA: 0x000506C4 File Offset: 0x0004E8C4
		public Vec3 GetMissileCollisionPoint(Vec3 missileStartingPosition, Vec3 missileDirection, float missileSpeed, in WeaponData weaponData)
		{
			return MBAPI.IMBMission.GetMissileCollisionPoint(this.Pointer, missileStartingPosition, missileDirection, missileSpeed, weaponData);
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x000506DB File Offset: 0x0004E8DB
		public void RemoveMissileAsClient(int missileIndex)
		{
			MBAPI.IMBMission.RemoveMissile(this.Pointer, missileIndex);
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x000506EE File Offset: 0x0004E8EE
		public static float GetMissileVerticalAimCorrection(Vec3 vecToTarget, float missileStartingSpeed, ref WeaponStatsData weaponStatsData, float airFrictionConstant)
		{
			return MBAPI.IMBMission.GetMissileVerticalAimCorrection(vecToTarget, missileStartingSpeed, ref weaponStatsData, airFrictionConstant);
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x000506FE File Offset: 0x0004E8FE
		public static float GetMissileRange(float missileStartingSpeed, float heightDifference)
		{
			return MBAPI.IMBMission.GetMissileRange(missileStartingSpeed, heightDifference);
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x0005070C File Offset: 0x0004E90C
		public void PrepareMissileWeaponForDrop(int missileIndex)
		{
			MBAPI.IMBMission.PrepareMissileWeaponForDrop(this.Pointer, missileIndex);
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x0005071F File Offset: 0x0004E91F
		public void AddParticleSystemBurstByName(string particleSystem, MatrixFrame frame, bool synchThroughNetwork)
		{
			MBAPI.IMBMission.AddParticleSystemBurstByName(this.Pointer, particleSystem, ref frame, synchThroughNetwork);
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x060017A1 RID: 6049 RVA: 0x00050735 File Offset: 0x0004E935
		public int EnemyAlarmStateIndicator
		{
			get
			{
				return MBAPI.IMBMission.GetEnemyAlarmStateIndicator(this.Pointer);
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x060017A2 RID: 6050 RVA: 0x00050747 File Offset: 0x0004E947
		public float PlayerAlarmIndicator
		{
			get
			{
				return MBAPI.IMBMission.GetPlayerAlarmIndicator(this.Pointer);
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x060017A3 RID: 6051 RVA: 0x00050759 File Offset: 0x0004E959
		public bool IsLoadingFinished
		{
			get
			{
				return MBAPI.IMBMission.GetIsLoadingFinished(this.Pointer);
			}
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x0005076B File Offset: 0x0004E96B
		public Vec2 GetClosestBoundaryPosition(Vec2 position)
		{
			return MBAPI.IMBMission.GetClosestBoundaryPosition(this.Pointer, position);
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x00050780 File Offset: 0x0004E980
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

		// Token: 0x060017A6 RID: 6054 RVA: 0x00050824 File Offset: 0x0004EA24
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

		// Token: 0x060017A7 RID: 6055 RVA: 0x000508A8 File Offset: 0x0004EAA8
		public int GetFreeRuntimeMissionObjectId()
		{
			float totalMissionTime = MBCommon.GetTotalMissionTime();
			int num = -1;
			if (this._emptyRuntimeMissionObjectIds.Count > 0)
			{
				if (totalMissionTime - this._emptyRuntimeMissionObjectIds.Peek().Item2 > 30f || this._lastRuntimeMissionObjectIdCount >= 4094)
				{
					num = this._emptyRuntimeMissionObjectIds.Pop().Item1;
				}
				else
				{
					num = this._lastRuntimeMissionObjectIdCount;
					this._lastRuntimeMissionObjectIdCount++;
				}
			}
			else if (this._lastRuntimeMissionObjectIdCount < 4094)
			{
				num = this._lastRuntimeMissionObjectIdCount;
				this._lastRuntimeMissionObjectIdCount++;
			}
			return num;
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x0005093E File Offset: 0x0004EB3E
		private void ReturnRuntimeMissionObjectId(int id)
		{
			this._emptyRuntimeMissionObjectIds.Push(new ValueTuple<int, float>(id, MBCommon.GetTotalMissionTime()));
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x00050956 File Offset: 0x0004EB56
		public int GetFreeSceneMissionObjectId()
		{
			int lastSceneMissionObjectIdCount = this._lastSceneMissionObjectIdCount;
			this._lastSceneMissionObjectIdCount++;
			return lastSceneMissionObjectIdCount;
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x0005096C File Offset: 0x0004EB6C
		public void SetCameraFrame(ref MatrixFrame cameraFrame, float zoomFactor)
		{
			this.SetCameraFrame(ref cameraFrame, zoomFactor, ref cameraFrame.origin);
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x0005097C File Offset: 0x0004EB7C
		public void SetCameraFrame(ref MatrixFrame cameraFrame, float zoomFactor, ref Vec3 attenuationPosition)
		{
			cameraFrame.Fill();
			MBAPI.IMBMission.SetCameraFrame(this.Pointer, ref cameraFrame, zoomFactor, ref attenuationPosition);
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x00050997 File Offset: 0x0004EB97
		public MatrixFrame GetCameraFrame()
		{
			return MBAPI.IMBMission.GetCameraFrame(this.Pointer);
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x060017AD RID: 6061 RVA: 0x000509A9 File Offset: 0x0004EBA9
		// (set) Token: 0x060017AE RID: 6062 RVA: 0x000509B0 File Offset: 0x0004EBB0
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

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x060017AF RID: 6063 RVA: 0x000509D1 File Offset: 0x0004EBD1
		// (set) Token: 0x060017B0 RID: 6064 RVA: 0x000509D8 File Offset: 0x0004EBD8
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

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x060017B1 RID: 6065 RVA: 0x000509E8 File Offset: 0x0004EBE8
		public float ClearSceneTimerElapsedTime
		{
			get
			{
				return MBAPI.IMBMission.GetClearSceneTimerElapsedTime(this.Pointer);
			}
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x000509FA File Offset: 0x0004EBFA
		public void ResetFirstThirdPersonView()
		{
			MBAPI.IMBMission.ResetFirstThirdPersonView(this.Pointer);
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x00050A0C File Offset: 0x0004EC0C
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

		// Token: 0x060017B4 RID: 6068 RVA: 0x00050AFB File Offset: 0x0004ECFB
		public void AddTimeSpeedRequest(Mission.TimeSpeedRequest request)
		{
			this._timeSpeedRequests.Add(request);
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x00050B0C File Offset: 0x0004ED0C
		[Conditional("_RGL_KEEP_ASSERTS")]
		private void AssertTimeSpeedRequestDoesntExist(Mission.TimeSpeedRequest request)
		{
			for (int i = 0; i < this._timeSpeedRequests.Count; i++)
			{
				int requestID = this._timeSpeedRequests[i].RequestID;
				int requestID2 = request.RequestID;
			}
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x00050B4C File Offset: 0x0004ED4C
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

		// Token: 0x060017B7 RID: 6071 RVA: 0x00050B98 File Offset: 0x0004ED98
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

		// Token: 0x060017B8 RID: 6072 RVA: 0x00050BF2 File Offset: 0x0004EDF2
		public void ClearAgentActions()
		{
			MBAPI.IMBMission.ClearAgentActions(this.Pointer);
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x00050C04 File Offset: 0x0004EE04
		public void ClearMissiles()
		{
			MBAPI.IMBMission.ClearMissiles(this.Pointer);
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x00050C16 File Offset: 0x0004EE16
		public void ClearCorpses(bool isMissionReset)
		{
			MBAPI.IMBMission.ClearCorpses(this.Pointer, isMissionReset);
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x00050C29 File Offset: 0x0004EE29
		private Agent FindAgentWithIndexAux(int index)
		{
			if (index >= 0)
			{
				return MBAPI.IMBMission.FindAgentWithIndex(this.Pointer, index);
			}
			return null;
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x00050C42 File Offset: 0x0004EE42
		private Agent GetClosestEnemyAgent(MBTeam team, Vec3 position, float radius)
		{
			return MBAPI.IMBMission.GetClosestEnemy(this.Pointer, team.Index, position, radius);
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x00050C5C File Offset: 0x0004EE5C
		private Agent GetClosestAllyAgent(MBTeam team, Vec3 position, float radius)
		{
			return MBAPI.IMBMission.GetClosestAlly(this.Pointer, team.Index, position, radius);
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x00050C76 File Offset: 0x0004EE76
		public bool IsAgentInProximityMap(Agent agent)
		{
			return MBAPI.IMBMission.IsAgentInProximityMap(this.Pointer, agent.Index);
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x00050C90 File Offset: 0x0004EE90
		private int GetNearbyEnemyAgentCount(MBTeam team, Vec2 position, float radius)
		{
			int num = 0;
			int num2 = 0;
			MBAPI.IMBMission.GetAgentCountAroundPosition(this.Pointer, team.Index, position, radius, ref num, ref num2);
			return num2;
		}

		// Token: 0x060017C0 RID: 6080 RVA: 0x00050CC0 File Offset: 0x0004EEC0
		public void OnMissionStateActivate()
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnMissionStateActivated();
			}
		}

		// Token: 0x060017C1 RID: 6081 RVA: 0x00050D10 File Offset: 0x0004EF10
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

		// Token: 0x060017C2 RID: 6082 RVA: 0x00050D68 File Offset: 0x0004EF68
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

		// Token: 0x060017C3 RID: 6083 RVA: 0x00050E1C File Offset: 0x0004F01C
		public void ClearUnreferencedResources(bool forceClearGPUResources)
		{
			Common.MemoryCleanupGC(false);
			if (forceClearGPUResources)
			{
				MBAPI.IMBMission.ClearResources(this.Pointer);
			}
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x00050E38 File Offset: 0x0004F038
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
						if (enumerator.Current.OnHit(attackerAgent, inflictedDamage, impactPosition, impactDirection, weapon, null, out flag3) && !flag2)
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
			if (flag && !attackerAgent.IsMount && !attackerAgent.IsAIControlled)
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

		// Token: 0x060017C5 RID: 6085 RVA: 0x00050F28 File Offset: 0x0004F128
		public float GetMainAgentMaxCameraZoom()
		{
			if (this.MainAgent != null)
			{
				return MissionGameModels.Current.AgentStatCalculateModel.GetMaxCameraZoom(this.MainAgent);
			}
			return 1f;
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x00050F4D File Offset: 0x0004F14D
		public WorldPosition GetBestSlopeTowardsDirection(ref WorldPosition centerPosition, float halfSize, ref WorldPosition referencePosition)
		{
			return MBAPI.IMBMission.GetBestSlopeTowardsDirection(this.Pointer, ref centerPosition, halfSize, ref referencePosition);
		}

		// Token: 0x060017C7 RID: 6087 RVA: 0x00050F64 File Offset: 0x0004F164
		public WorldPosition GetBestSlopeAngleHeightPosForDefending(WorldPosition enemyPosition, WorldPosition defendingPosition, int sampleSize, float distanceRatioAllowedFromDefendedPos, float distanceSqrdAllowedFromBoundary, float cosinusOfBestSlope, float cosinusOfMaxAcceptedSlope, float minSlopeScore, float maxSlopeScore, float excessiveSlopePenalty, float nearConeCenterRatio, float nearConeCenterBonus, float heightDifferenceCeiling, float maxDisplacementPenalty)
		{
			return MBAPI.IMBMission.GetBestSlopeAngleHeightPosForDefending(this.Pointer, enemyPosition, defendingPosition, sampleSize, distanceRatioAllowedFromDefendedPos, distanceSqrdAllowedFromBoundary, cosinusOfBestSlope, cosinusOfMaxAcceptedSlope, minSlopeScore, maxSlopeScore, excessiveSlopePenalty, nearConeCenterRatio, nearConeCenterBonus, heightDifferenceCeiling, maxDisplacementPenalty);
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x00050F9C File Offset: 0x0004F19C
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

		// Token: 0x060017C9 RID: 6089 RVA: 0x0005101C File Offset: 0x0004F21C
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

		// Token: 0x060017CA RID: 6090 RVA: 0x000510C0 File Offset: 0x0004F2C0
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

		// Token: 0x060017CB RID: 6091 RVA: 0x00051140 File Offset: 0x0004F340
		public void SetRandomDecideTimeOfAgentsWithIndices(int[] agentIndices, float? minAIReactionTime = null, float? maxAIReactionTime = null)
		{
			if (minAIReactionTime == null || maxAIReactionTime == null)
			{
				maxAIReactionTime = new float?((float)(-1));
				minAIReactionTime = maxAIReactionTime;
			}
			MBAPI.IMBMission.SetRandomDecideTimeOfAgents(this.Pointer, agentIndices.Length, agentIndices, minAIReactionTime.Value, maxAIReactionTime.Value);
		}

		// Token: 0x060017CC RID: 6092 RVA: 0x0005118D File Offset: 0x0004F38D
		public void SetBowMissileSpeedModifier(float modifier)
		{
			MBAPI.IMBMission.SetBowMissileSpeedModifier(this.Pointer, modifier);
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x000511A0 File Offset: 0x0004F3A0
		public void SetCrossbowMissileSpeedModifier(float modifier)
		{
			MBAPI.IMBMission.SetCrossbowMissileSpeedModifier(this.Pointer, modifier);
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x000511B3 File Offset: 0x0004F3B3
		public void SetThrowingMissileSpeedModifier(float modifier)
		{
			MBAPI.IMBMission.SetThrowingMissileSpeedModifier(this.Pointer, modifier);
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x000511C6 File Offset: 0x0004F3C6
		public void SetMissileRangeModifier(float modifier)
		{
			MBAPI.IMBMission.SetMissileRangeModifier(this.Pointer, modifier);
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x000511D9 File Offset: 0x0004F3D9
		public void SetLastMovementKeyPressed(Agent.MovementControlFlag lastMovementKeyPressed)
		{
			MBAPI.IMBMission.SetLastMovementKeyPressed(this.Pointer, lastMovementKeyPressed);
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x000511EC File Offset: 0x0004F3EC
		public Vec2 GetWeightedPointOfEnemies(Agent agent, Vec2 basePoint)
		{
			return MBAPI.IMBMission.GetWeightedPointOfEnemies(this.Pointer, agent.Index, basePoint);
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x00051205 File Offset: 0x0004F405
		public bool GetPathBetweenPositions(ref NavigationData navData)
		{
			return MBAPI.IMBMission.GetNavigationPoints(this.Pointer, ref navData);
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x00051218 File Offset: 0x0004F418
		public void SetNavigationFaceCostWithIdAroundPosition(int navigationFaceId, Vec3 position, float cost)
		{
			MBAPI.IMBMission.SetNavigationFaceCostWithIdAroundPosition(this.Pointer, navigationFaceId, position, cost);
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x0005122D File Offset: 0x0004F42D
		public WorldPosition GetStraightPathToTarget(Vec2 targetPosition, WorldPosition startingPosition, float samplingDistance = 1f, bool stopAtObstacle = true)
		{
			return MBAPI.IMBMission.GetStraightPathToTarget(this.Pointer, targetPosition, startingPosition, samplingDistance, stopAtObstacle);
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x00051244 File Offset: 0x0004F444
		public void FastForwardMission(float startTime, float endTime)
		{
			MBAPI.IMBMission.FastForwardMission(this.Pointer, startTime, endTime);
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x00051258 File Offset: 0x0004F458
		public int GetDebugAgent()
		{
			return MBAPI.IMBMission.GetDebugAgent(this.Pointer);
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x0005126A File Offset: 0x0004F46A
		public void AddAiDebugText(string str)
		{
			MBAPI.IMBMission.AddAiDebugText(this.Pointer, str);
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x0005127D File Offset: 0x0004F47D
		public void SetDebugAgent(int index)
		{
			MBAPI.IMBMission.SetDebugAgent(this.Pointer, index);
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x00051290 File Offset: 0x0004F490
		public static float GetFirstPersonFov()
		{
			return BannerlordConfig.FirstPersonFov;
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x00051297 File Offset: 0x0004F497
		public float GetWaterLevelAtPosition(Vec2 position)
		{
			return MBAPI.IMBMission.GetWaterLevelAtPosition(this.Pointer, position);
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x000512AA File Offset: 0x0004F4AA
		public float GetWaterLevelAtPositionMT(Vec2 position)
		{
			return MBAPI.IMBMission.GetWaterLevelAtPosition(this.Pointer, position);
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x060017DC RID: 6108 RVA: 0x000512C0 File Offset: 0x0004F4C0
		// (remove) Token: 0x060017DD RID: 6109 RVA: 0x000512F8 File Offset: 0x0004F4F8
		public event Func<WorldPosition, Team, bool> IsFormationUnitPositionAvailable_AdditionalCondition;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x060017DE RID: 6110 RVA: 0x00051330 File Offset: 0x0004F530
		// (remove) Token: 0x060017DF RID: 6111 RVA: 0x00051368 File Offset: 0x0004F568
		public event Func<Agent, bool> CanAgentRout_AdditionalCondition;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x060017E0 RID: 6112 RVA: 0x000513A0 File Offset: 0x0004F5A0
		// (remove) Token: 0x060017E1 RID: 6113 RVA: 0x000513D8 File Offset: 0x0004F5D8
		public event Func<bool> AreOrderGesturesEnabled_AdditionalCondition;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x060017E2 RID: 6114 RVA: 0x00051410 File Offset: 0x0004F610
		// (remove) Token: 0x060017E3 RID: 6115 RVA: 0x00051448 File Offset: 0x0004F648
		public event Func<Agent, WorldPosition?> GetOverriddenFleePositionForAgent;

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x060017E4 RID: 6116 RVA: 0x00051480 File Offset: 0x0004F680
		// (remove) Token: 0x060017E5 RID: 6117 RVA: 0x000514B8 File Offset: 0x0004F6B8
		public event Func<bool> IsAgentInteractionAllowed_AdditionalCondition;

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x060017E6 RID: 6118 RVA: 0x000514ED File Offset: 0x0004F6ED
		// (set) Token: 0x060017E7 RID: 6119 RVA: 0x000514F5 File Offset: 0x0004F6F5
		public bool MissionEnded { get; private set; }

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x060017E8 RID: 6120 RVA: 0x00051500 File Offset: 0x0004F700
		// (remove) Token: 0x060017E9 RID: 6121 RVA: 0x00051538 File Offset: 0x0004F738
		public event PropertyChangedEventHandler OnMainAgentChanged;

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x060017EA RID: 6122 RVA: 0x0005156D File Offset: 0x0004F76D
		public MBReadOnlyList<Agent> MountsWithoutRiders
		{
			get
			{
				return this._mountsWithoutRiders;
			}
		}

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060017EB RID: 6123 RVA: 0x00051578 File Offset: 0x0004F778
		// (remove) Token: 0x060017EC RID: 6124 RVA: 0x000515B0 File Offset: 0x0004F7B0
		public event Func<bool> IsBattleInRetreatEvent;

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x060017ED RID: 6125 RVA: 0x000515E8 File Offset: 0x0004F7E8
		// (remove) Token: 0x060017EE RID: 6126 RVA: 0x00051620 File Offset: 0x0004F820
		public event Mission.OnBeforeAgentRemovedDelegate OnBeforeAgentRemoved;

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x060017EF RID: 6127 RVA: 0x00051655 File Offset: 0x0004F855
		// (set) Token: 0x060017F0 RID: 6128 RVA: 0x0005165D File Offset: 0x0004F85D
		public BattleSideEnum RetreatSide { get; private set; } = BattleSideEnum.None;

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x060017F1 RID: 6129 RVA: 0x00051666 File Offset: 0x0004F866
		// (set) Token: 0x060017F2 RID: 6130 RVA: 0x0005166E File Offset: 0x0004F86E
		public bool IsFastForward { get; private set; }

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x060017F3 RID: 6131 RVA: 0x00051677 File Offset: 0x0004F877
		// (set) Token: 0x060017F4 RID: 6132 RVA: 0x0005167F File Offset: 0x0004F87F
		public bool FixedDeltaTimeMode { get; set; }

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x060017F5 RID: 6133 RVA: 0x00051688 File Offset: 0x0004F888
		// (set) Token: 0x060017F6 RID: 6134 RVA: 0x00051690 File Offset: 0x0004F890
		public float FixedDeltaTime { get; set; }

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x060017F7 RID: 6135 RVA: 0x00051699 File Offset: 0x0004F899
		// (set) Token: 0x060017F8 RID: 6136 RVA: 0x000516A1 File Offset: 0x0004F8A1
		public Mission.State CurrentState { get; private set; }

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x060017F9 RID: 6137 RVA: 0x000516AA File Offset: 0x0004F8AA
		// (set) Token: 0x060017FA RID: 6138 RVA: 0x000516B2 File Offset: 0x0004F8B2
		public Mission.TeamCollection Teams { get; private set; }

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x060017FB RID: 6139 RVA: 0x000516BB File Offset: 0x0004F8BB
		public Team AttackerTeam
		{
			get
			{
				return this.Teams.Attacker;
			}
		}

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x060017FC RID: 6140 RVA: 0x000516C8 File Offset: 0x0004F8C8
		public Team DefenderTeam
		{
			get
			{
				return this.Teams.Defender;
			}
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x060017FD RID: 6141 RVA: 0x000516D5 File Offset: 0x0004F8D5
		public Team AttackerAllyTeam
		{
			get
			{
				return this.Teams.AttackerAlly;
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x060017FE RID: 6142 RVA: 0x000516E2 File Offset: 0x0004F8E2
		public Team DefenderAllyTeam
		{
			get
			{
				return this.Teams.DefenderAlly;
			}
		}

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x060017FF RID: 6143 RVA: 0x000516EF File Offset: 0x0004F8EF
		// (set) Token: 0x06001800 RID: 6144 RVA: 0x000516FC File Offset: 0x0004F8FC
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

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06001801 RID: 6145 RVA: 0x0005170A File Offset: 0x0004F90A
		public Team PlayerEnemyTeam
		{
			get
			{
				return this.Teams.PlayerEnemy;
			}
		}

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06001802 RID: 6146 RVA: 0x00051717 File Offset: 0x0004F917
		public Team PlayerAllyTeam
		{
			get
			{
				return this.Teams.PlayerAlly;
			}
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06001803 RID: 6147 RVA: 0x00051724 File Offset: 0x0004F924
		// (set) Token: 0x06001804 RID: 6148 RVA: 0x0005172C File Offset: 0x0004F92C
		public Team SpectatorTeam { get; set; }

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06001805 RID: 6149 RVA: 0x00051735 File Offset: 0x0004F935
		IMissionTeam IMission.PlayerTeam
		{
			get
			{
				return this.PlayerTeam;
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06001806 RID: 6150 RVA: 0x0005173D File Offset: 0x0004F93D
		public bool IsMissionEnding
		{
			get
			{
				return this.CurrentState != Mission.State.Over && this.MissionEnded;
			}
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06001807 RID: 6151 RVA: 0x00051750 File Offset: 0x0004F950
		public List<MissionLogic> MissionLogics { get; }

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06001808 RID: 6152 RVA: 0x00051758 File Offset: 0x0004F958
		public List<MissionBehavior> MissionBehaviors { get; }

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06001809 RID: 6153 RVA: 0x00051760 File Offset: 0x0004F960
		public IEnumerable<Mission.Missile> Missiles
		{
			get
			{
				return this._missiles.Values;
			}
		}

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x0600180A RID: 6154 RVA: 0x0005176D File Offset: 0x0004F96D
		// (set) Token: 0x0600180B RID: 6155 RVA: 0x00051775 File Offset: 0x0004F975
		public IInputContext InputManager { get; set; }

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x0600180C RID: 6156 RVA: 0x0005177E File Offset: 0x0004F97E
		// (set) Token: 0x0600180D RID: 6157 RVA: 0x00051786 File Offset: 0x0004F986
		public bool NeedsMemoryCleanup { get; internal set; }

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x0600180E RID: 6158 RVA: 0x0005178F File Offset: 0x0004F98F
		// (set) Token: 0x0600180F RID: 6159 RVA: 0x00051797 File Offset: 0x0004F997
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
				if (!MBNetwork.IsClient)
				{
					this.MainAgentServer = this._mainAgent;
				}
			}
		}

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06001810 RID: 6160 RVA: 0x000517CA File Offset: 0x0004F9CA
		public IMissionDeploymentPlan DeploymentPlan
		{
			get
			{
				return this._deploymentPlan;
			}
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x000517D4 File Offset: 0x0004F9D4
		public float GetRemovedAgentRatioForSide(BattleSideEnum side)
		{
			float num = 0f;
			if (side == BattleSideEnum.NumSides)
			{
				Debug.FailedAssert("Cannot get removed agent count for side. Invalid battle side passed!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "GetRemovedAgentRatioForSide", 476);
			}
			float num2 = (float)this._initialAgentCountPerSide[(int)side];
			if (num2 > 0f && this._agentCount > 0)
			{
				num = MathF.Min((float)this._removedAgentCountPerSide[(int)side] / num2, 1f);
			}
			return num;
		}

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06001812 RID: 6162 RVA: 0x00051836 File Offset: 0x0004FA36
		// (set) Token: 0x06001813 RID: 6163 RVA: 0x0005183E File Offset: 0x0004FA3E
		public Agent MainAgentServer { get; set; }

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06001814 RID: 6164 RVA: 0x00051847 File Offset: 0x0004FA47
		public bool HasSpawnPath
		{
			get
			{
				return this._battleSpawnPathSelector.IsInitialized;
			}
		}

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06001815 RID: 6165 RVA: 0x00051854 File Offset: 0x0004FA54
		public bool IsFieldBattle
		{
			get
			{
				return this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.FieldBattle;
			}
		}

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06001816 RID: 6166 RVA: 0x0005185F File Offset: 0x0004FA5F
		public bool IsSiegeBattle
		{
			get
			{
				return this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.Siege;
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06001817 RID: 6167 RVA: 0x0005186A File Offset: 0x0004FA6A
		public bool IsSallyOutBattle
		{
			get
			{
				return this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.SallyOut;
			}
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06001818 RID: 6168 RVA: 0x00051875 File Offset: 0x0004FA75
		public MBReadOnlyList<Agent> AllAgents
		{
			get
			{
				return this._allAgents;
			}
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06001819 RID: 6169 RVA: 0x0005187D File Offset: 0x0004FA7D
		public MBReadOnlyList<Agent> Agents
		{
			get
			{
				return this._activeAgents;
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x0600181A RID: 6170 RVA: 0x00051885 File Offset: 0x0004FA85
		public bool IsInventoryAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsInventoryAccessibleAtMission || this._isScreenAccessAllowed) && this.IsInventoryAccessible;
			}
		}

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x0600181B RID: 6171 RVA: 0x000518A8 File Offset: 0x0004FAA8
		// (set) Token: 0x0600181C RID: 6172 RVA: 0x000518B0 File Offset: 0x0004FAB0
		public bool IsInventoryAccessible { private get; set; }

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x0600181D RID: 6173 RVA: 0x000518B9 File Offset: 0x0004FAB9
		// (set) Token: 0x0600181E RID: 6174 RVA: 0x000518C1 File Offset: 0x0004FAC1
		public MissionResult MissionResult { get; private set; }

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x0600181F RID: 6175 RVA: 0x000518CA File Offset: 0x0004FACA
		// (set) Token: 0x06001820 RID: 6176 RVA: 0x000518D2 File Offset: 0x0004FAD2
		public bool IsQuestScreenAccessible { private get; set; }

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06001821 RID: 6177 RVA: 0x000518DB File Offset: 0x0004FADB
		private bool _isScreenAccessAllowed
		{
			get
			{
				return this.Mode != MissionMode.Stealth && this.Mode != MissionMode.Battle && this.Mode != MissionMode.Deployment && this.Mode != MissionMode.Duel && this.Mode != MissionMode.CutScene;
			}
		}

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06001822 RID: 6178 RVA: 0x00051910 File Offset: 0x0004FB10
		public bool IsQuestScreenAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsQuestScreenAccessibleAtMission || this._isScreenAccessAllowed) && this.IsQuestScreenAccessible;
			}
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06001823 RID: 6179 RVA: 0x00051933 File Offset: 0x0004FB33
		// (set) Token: 0x06001824 RID: 6180 RVA: 0x0005193B File Offset: 0x0004FB3B
		public bool IsCharacterWindowAccessible { private get; set; }

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06001825 RID: 6181 RVA: 0x00051944 File Offset: 0x0004FB44
		public bool IsCharacterWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsCharacterWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsCharacterWindowAccessible;
			}
		}

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06001826 RID: 6182 RVA: 0x00051967 File Offset: 0x0004FB67
		// (set) Token: 0x06001827 RID: 6183 RVA: 0x0005196F File Offset: 0x0004FB6F
		public bool IsPartyWindowAccessible { private get; set; }

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06001828 RID: 6184 RVA: 0x00051978 File Offset: 0x0004FB78
		public bool IsPartyWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsPartyWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsPartyWindowAccessible;
			}
		}

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06001829 RID: 6185 RVA: 0x0005199B File Offset: 0x0004FB9B
		// (set) Token: 0x0600182A RID: 6186 RVA: 0x000519A3 File Offset: 0x0004FBA3
		public bool IsKingdomWindowAccessible { private get; set; }

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x0600182B RID: 6187 RVA: 0x000519AC File Offset: 0x0004FBAC
		public bool IsKingdomWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsKingdomWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsKingdomWindowAccessible;
			}
		}

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x0600182C RID: 6188 RVA: 0x000519CF File Offset: 0x0004FBCF
		// (set) Token: 0x0600182D RID: 6189 RVA: 0x000519D7 File Offset: 0x0004FBD7
		public bool IsClanWindowAccessible { private get; set; }

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x0600182E RID: 6190 RVA: 0x000519E0 File Offset: 0x0004FBE0
		public bool IsClanWindowAccessAllowed
		{
			get
			{
				return Game.Current.GameType.IsClanWindowAccessibleAtMission && this._isScreenAccessAllowed && this.IsClanWindowAccessible;
			}
		}

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x0600182F RID: 6191 RVA: 0x00051A03 File Offset: 0x0004FC03
		// (set) Token: 0x06001830 RID: 6192 RVA: 0x00051A0B File Offset: 0x0004FC0B
		public bool IsEncyclopediaWindowAccessible { private get; set; }

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06001831 RID: 6193 RVA: 0x00051A14 File Offset: 0x0004FC14
		public bool IsEncyclopediaWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsEncyclopediaWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsEncyclopediaWindowAccessible;
			}
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06001832 RID: 6194 RVA: 0x00051A37 File Offset: 0x0004FC37
		// (set) Token: 0x06001833 RID: 6195 RVA: 0x00051A3F File Offset: 0x0004FC3F
		public bool IsBannerWindowAccessible { private get; set; }

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06001834 RID: 6196 RVA: 0x00051A48 File Offset: 0x0004FC48
		public bool IsBannerWindowAccessAllowed
		{
			get
			{
				return (Game.Current.GameType.IsBannerWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsBannerWindowAccessible;
			}
		}

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06001835 RID: 6197 RVA: 0x00051A6B File Offset: 0x0004FC6B
		// (set) Token: 0x06001836 RID: 6198 RVA: 0x00051A73 File Offset: 0x0004FC73
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

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06001837 RID: 6199 RVA: 0x00051A7C File Offset: 0x0004FC7C
		// (set) Token: 0x06001838 RID: 6200 RVA: 0x00051A84 File Offset: 0x0004FC84
		public Mission.MissionTeamAITypeEnum MissionTeamAIType { get; set; }

		// Token: 0x06001839 RID: 6201 RVA: 0x00051A90 File Offset: 0x0004FC90
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

		// Token: 0x0600183A RID: 6202 RVA: 0x00051B14 File Offset: 0x0004FD14
		public void MakeDefaultDeploymentPlans()
		{
			for (int i = 0; i < 2; i++)
			{
				BattleSideEnum battleSideEnum = (BattleSideEnum)i;
				this.MakeDeploymentPlanForSide(battleSideEnum, DeploymentPlanType.Initial, 0f);
				this.MakeDeploymentPlanForSide(battleSideEnum, DeploymentPlanType.Reinforcement, 0f);
			}
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x00051B49 File Offset: 0x0004FD49
		public ref readonly List<SiegeWeapon> GetAttackerWeaponsForFriendlyFirePreventing()
		{
			return ref this._attackerWeaponsForFriendlyFirePreventing;
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x00051B51 File Offset: 0x0004FD51
		public void ClearDeploymentPlanForSide(BattleSideEnum battleSide, DeploymentPlanType planType)
		{
			this._deploymentPlan.ClearDeploymentPlanForSide(battleSide, planType);
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x00051B60 File Offset: 0x0004FD60
		public void ClearAddedTroopsInDeploymentPlan(BattleSideEnum battleSide, DeploymentPlanType planType)
		{
			this._deploymentPlan.ClearAddedTroopsForBattleSide(battleSide, planType);
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x00051B6F File Offset: 0x0004FD6F
		public void SetDeploymentPlanSpawnWithHorses(BattleSideEnum side, bool spawnWithHorses)
		{
			this._deploymentPlan.SetSpawnWithHorsesForSide(side, spawnWithHorses);
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x00051B7E File Offset: 0x0004FD7E
		public void UpdateReinforcementPlan(BattleSideEnum side)
		{
			this._deploymentPlan.UpdateReinforcementPlan(side);
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x00051B8C File Offset: 0x0004FD8C
		public void AddTroopsToDeploymentPlan(BattleSideEnum side, DeploymentPlanType planType, FormationClass fClass, int footTroopCount, int mountedTroopCount)
		{
			this._deploymentPlan.AddTroopsForBattleSide(side, planType, fClass, footTroopCount, mountedTroopCount);
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x00051BA0 File Offset: 0x0004FDA0
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

		// Token: 0x06001842 RID: 6210 RVA: 0x00051D34 File Offset: 0x0004FF34
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
				team.ResetTactic();
				team.Tick(0f);
				for (int j = 0; j < list.Count; j++)
				{
					list[j].ApplyActionOnEachUnit(delegate(Agent agent)
					{
						agent.UpdateCachedAndFormationValues(true, false);
					}, null);
				}
				this.IsTeleportingAgents = isTeleportingAgents;
				this.ForceTickOccasionally = forceTickOccasionally;
				this.AllowAiTicking = allowAiTicking;
				for (int k = 0; k < list.Count; k++)
				{
					Formation formation2 = list[k];
					bool flag = array[k];
					formation2.SetControlledByAI(true, flag);
				}
			}
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x00051E80 File Offset: 0x00050080
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

		// Token: 0x06001844 RID: 6212 RVA: 0x00052008 File Offset: 0x00050208
		public WorldPosition GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(Vec2 directionTowards, WorldPosition originalPosition, ref float positionPenalty)
		{
			return MBAPI.IMBMission.GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(this.Pointer, ref directionTowards, ref originalPosition, ref positionPenalty);
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x0005201F File Offset: 0x0005021F
		public int GetNextDynamicNavMeshIdStart()
		{
			int nextDynamicNavMeshIdStart = this._NextDynamicNavMeshIdStart;
			this._NextDynamicNavMeshIdStart += 10;
			return nextDynamicNavMeshIdStart;
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x00052038 File Offset: 0x00050238
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

		// Token: 0x06001847 RID: 6215 RVA: 0x000520DC File Offset: 0x000502DC
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

		// Token: 0x06001848 RID: 6216 RVA: 0x0005214C File Offset: 0x0005034C
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

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06001849 RID: 6217 RVA: 0x000527A2 File Offset: 0x000509A2
		// (set) Token: 0x0600184A RID: 6218 RVA: 0x000527AA File Offset: 0x000509AA
		public MissionTimeTracker MissionTimeTracker { get; private set; }

		// Token: 0x0600184B RID: 6219 RVA: 0x000527B4 File Offset: 0x000509B4
		public MBReadOnlyList<FleePosition> GetFleePositionsForSide(BattleSideEnum side)
		{
			if (side == BattleSideEnum.NumSides)
			{
				Debug.FailedAssert("Flee position with invalid battle side field found!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "GetFleePositionsForSide", 1438);
				return null;
			}
			int num = (int)((side == BattleSideEnum.None) ? BattleSideEnum.Defender : (side + 1));
			return this._fleePositions[num];
		}

		// Token: 0x0600184C RID: 6220 RVA: 0x000527F3 File Offset: 0x000509F3
		public void AddToWeaponListForFriendlyFirePreventing(SiegeWeapon weapon)
		{
			this._attackerWeaponsForFriendlyFirePreventing.Add(weapon);
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x00052804 File Offset: 0x00050A04
		public Mission(MissionInitializerRecord rec, MissionState missionState)
		{
			this.Pointer = MBAPI.IMBMission.CreateMission(this);
			this._spawnedItemEntitiesCreatedAtRuntime = new List<SpawnedItemEntity>();
			this._missionObjects = new MBList<MissionObject>();
			this._activeMissionObjects = new MBList<MissionObject>();
			this._mountsWithoutRiders = new MBList<Agent>();
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

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x0600184E RID: 6222 RVA: 0x00052A28 File Offset: 0x00050C28
		private Lazy<MissionRecorder> _recorder
		{
			get
			{
				return new Lazy<MissionRecorder>(() => new MissionRecorder(this));
			}
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x0600184F RID: 6223 RVA: 0x00052A3B File Offset: 0x00050C3B
		public MissionRecorder Recorder
		{
			get
			{
				return this._recorder.Value;
			}
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x00052A48 File Offset: 0x00050C48
		public void AddFleePosition(FleePosition fleePosition)
		{
			BattleSideEnum side = fleePosition.GetSide();
			if (side == BattleSideEnum.NumSides)
			{
				Debug.FailedAssert("Flee position with invalid battle side field found!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "AddFleePosition", 1519);
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

		// Token: 0x06001851 RID: 6225 RVA: 0x00052AB4 File Offset: 0x00050CB4
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
			if (MBNetwork.DisconnectedNetworkPeers != null)
			{
				Console.WriteLine("> DisconnectedNetworkPeers.Clear()");
				MBNetwork.DisconnectedNetworkPeers.Clear();
			}
			this._missionState = null;
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x00052B74 File Offset: 0x00050D74
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

		// Token: 0x06001853 RID: 6227 RVA: 0x00052BE0 File Offset: 0x00050DE0
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

		// Token: 0x06001854 RID: 6228 RVA: 0x00052C4C File Offset: 0x00050E4C
		public bool HasMissionBehavior<T>() where T : MissionBehavior
		{
			return this.GetMissionBehavior<T>() != null;
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x00052C5C File Offset: 0x00050E5C
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

		// Token: 0x06001856 RID: 6230 RVA: 0x00052CB0 File Offset: 0x00050EB0
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
				GameNetwork.WriteMessage(new SpawnAttachedWeaponOnCorpse(agent, attachedWeaponIndex, firstScriptOfType.Id.Id));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.SpawnWeaponAux(attachedWeaponEntity, attachedWeapon, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, Vec3.Zero, Vec3.Zero, false);
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x00052D4F File Offset: 0x00050F4F
		public void AddMountWithoutRider(Agent mount)
		{
			this._mountsWithoutRiders.Add(mount);
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x00052D5D File Offset: 0x00050F5D
		public void RemoveMountWithoutRider(Agent mount)
		{
			this._mountsWithoutRiders.Remove(mount);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x00052D6C File Offset: 0x00050F6C
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

		// Token: 0x0600185A RID: 6234 RVA: 0x00052DE4 File Offset: 0x00050FE4
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
			if (!GameNetwork.IsClientOrReplay && agentState != AgentState.Routed && affectedAgent.GetAgentFlags().HasAnyFlag(AgentFlag.CanWieldWeapon) && affectedAgent.WieldedOffhandWeapon.CurrentUsageItem != null && affectedAgent.WieldedOffhandWeapon.CurrentUsageItem.WeaponClass == WeaponClass.Banner)
			{
				affectedAgent.DropItem(EquipmentIndex.ExtraWeaponSlot, WeaponClass.Undefined);
			}
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x00052FA8 File Offset: 0x000511A8
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

		// Token: 0x0600185C RID: 6236 RVA: 0x0005301C File Offset: 0x0005121C
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

		// Token: 0x0600185D RID: 6237 RVA: 0x000530E8 File Offset: 0x000512E8
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

		// Token: 0x0600185E RID: 6238 RVA: 0x00053154 File Offset: 0x00051354
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
				GameNetwork.WriteMessage(new SpawnWeaponAsDropFromAgent(agent, equipmentIndex, velocity, angularVelocity, spawnFlags, firstScriptOfType.Id.Id));
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
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnItemDrop(agent, firstScriptOfType);
			}
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x00053304 File Offset: 0x00051504
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
				GameNetwork.WriteMessage(new SpawnAttachedWeaponOnSpawnedWeapon(spawnedWeapon, attachmentIndex, firstScriptOfType.Id.Id));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x00053399 File Offset: 0x00051599
		public GameEntity SpawnWeaponWithNewEntity(ref MissionWeapon weapon, Mission.WeaponSpawnFlags spawnFlags, MatrixFrame frame)
		{
			return this.SpawnWeaponWithNewEntityAux(weapon, spawnFlags, frame, -1, null, false);
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x000533AC File Offset: 0x000515AC
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
				GameNetwork.WriteMessage(new SpawnWeaponWithNewEntity(weapon, spawnFlags, firstScriptOfType.Id.Id, frame, attachedMissionObject, true, hasLifeTime));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				for (int i = 0; i < weapon.GetAttachedWeaponsCount(); i++)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new AttachWeaponToSpawnedWeapon(weapon.GetAttachedWeapon(i), firstScriptOfType, weapon.GetAttachedWeaponFrame(i)));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			Vec3 zero = Vec3.Zero;
			this.SpawnWeaponAux(gameEntity, weapon, spawnFlags, zero, zero, hasLifeTime);
			return gameEntity;
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x000534C0 File Offset: 0x000516C0
		public void AttachWeaponWithNewEntityToSpawnedWeapon(MissionWeapon weapon, SpawnedItemEntity spawnedItem, MatrixFrame attachLocalFrame)
		{
			GameEntity gameEntity = GameEntityExtensions.Instantiate(this.Scene, weapon, false, true);
			spawnedItem.GameEntity.AddChild(gameEntity, false);
			gameEntity.SetFrame(ref attachLocalFrame);
			spawnedItem.AttachWeaponToWeapon(weapon, ref attachLocalFrame);
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x000534FC File Offset: 0x000516FC
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

		// Token: 0x06001864 RID: 6244 RVA: 0x00053624 File Offset: 0x00051824
		public void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
		{
			foreach (IMissionListener missionListener in this._listeners)
			{
				missionListener.OnEquipItemsFromSpawnEquipmentBegin(agent, creationType);
			}
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x00053678 File Offset: 0x00051878
		public void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
		{
			foreach (IMissionListener missionListener in this._listeners)
			{
				missionListener.OnEquipItemsFromSpawnEquipment(agent, creationType);
			}
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x000536CC File Offset: 0x000518CC
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

		// Token: 0x06001867 RID: 6247 RVA: 0x00053774 File Offset: 0x00051974
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
			string text2 = strings[0];
			if (strings.IsEmpty<string>() || text2 == "help")
			{
				return "makes an entire team or a team's formation flee battle.\n" + text;
			}
			if (strings.Count >= 3)
			{
				return "invalid number of parameters.\n" + text;
			}
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

		// Token: 0x06001868 RID: 6248 RVA: 0x00053A4C File Offset: 0x00051C4C
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
				Debug.FailedAssert("Item has no body! Applying a default body, but this should not happen! Check this!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "RecalculateBody", 2289);
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
							Debug.FailedAssert("Item has 0 body parts. Applying a default body, but this should not happen! Check this!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "RecalculateBody", 2427);
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
							Debug.FailedAssert("Shields should not have recalculate body flag.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "RecalculateBody", 2501);
							break;
						}
					}
				}
			}
			weaponData.CenterOfMassShift = weaponData.Shape.GetWeaponCenterOfMass();
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x000540E4 File Offset: 0x000522E4
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

		// Token: 0x0600186A RID: 6250 RVA: 0x00054128 File Offset: 0x00052328
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

		// Token: 0x0600186B RID: 6251 RVA: 0x0005422C File Offset: 0x0005242C
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

		// Token: 0x0600186C RID: 6252 RVA: 0x000542D4 File Offset: 0x000524D4
		private void waitTickCompletion()
		{
			while (!this.tickCompleted)
			{
				Thread.Sleep(1);
			}
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x000542E8 File Offset: 0x000524E8
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

		// Token: 0x0600186E RID: 6254 RVA: 0x000543C4 File Offset: 0x000525C4
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

		// Token: 0x0600186F RID: 6255 RVA: 0x00054414 File Offset: 0x00052614
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

		// Token: 0x06001870 RID: 6256 RVA: 0x000545D7 File Offset: 0x000527D7
		public void RemoveSpawnedItemsAndMissiles()
		{
			this.ClearMissiles();
			this._missiles.Clear();
			this.RemoveSpawnedMissionObjects();
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x000545F0 File Offset: 0x000527F0
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

		// Token: 0x06001872 RID: 6258 RVA: 0x000547BC File Offset: 0x000529BC
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

		// Token: 0x06001873 RID: 6259 RVA: 0x00054864 File Offset: 0x00052A64
		public float GetMissionEndTimeInSeconds()
		{
			return 0.6f;
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x0005486B File Offset: 0x00052A6B
		public float GetMissionEndTimerValue()
		{
			if (this._leaveMissionTimer == null)
			{
				return -1f;
			}
			return this._leaveMissionTimer.ElapsedTime;
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x00054888 File Offset: 0x00052A88
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

		// Token: 0x06001876 RID: 6262 RVA: 0x000548BC File Offset: 0x00052ABC
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

		// Token: 0x06001877 RID: 6263 RVA: 0x00054924 File Offset: 0x00052B24
		public Path GetInitialSpawnPath()
		{
			return this._battleSpawnPathSelector.InitialPath;
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x00054934 File Offset: 0x00052B34
		public SpawnPathData GetInitialSpawnPathDataOfSide(BattleSideEnum battleSide)
		{
			SpawnPathData spawnPathData;
			this._battleSpawnPathSelector.GetInitialPathDataOfSide(battleSide, out spawnPathData);
			return spawnPathData;
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x00054951 File Offset: 0x00052B51
		public MBReadOnlyList<SpawnPathData> GetReinforcementPathsDataOfSide(BattleSideEnum battleSide)
		{
			return this._battleSpawnPathSelector.GetReinforcementPathsDataOfSide(battleSide);
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x00054960 File Offset: 0x00052B60
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
					FormationClass formationClass = agentCharacter.GetFormationClass();
					this.GetFormationSpawnFrame(side, formationClass, agentIsReinforcement, out worldPosition, out direction);
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

		// Token: 0x0600187B RID: 6267 RVA: 0x00054B4C File Offset: 0x00052D4C
		public void GetFormationSpawnFrame(BattleSideEnum side, FormationClass formationClass, bool isReinforcement, out WorldPosition spawnPosition, out Vec2 spawnDirection)
		{
			DeploymentPlanType deploymentPlanType = (isReinforcement ? DeploymentPlanType.Reinforcement : DeploymentPlanType.Initial);
			IFormationDeploymentPlan formationPlan = this._deploymentPlan.GetFormationPlan(side, formationClass, deploymentPlanType);
			spawnPosition = formationPlan.CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3);
			spawnDirection = formationPlan.GetDirection();
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x00054B8C File Offset: 0x00052D8C
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

		// Token: 0x0600187D RID: 6269 RVA: 0x00054BF8 File Offset: 0x00052DF8
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

		// Token: 0x0600187E RID: 6270 RVA: 0x00054D18 File Offset: 0x00052F18
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

		// Token: 0x0600187F RID: 6271 RVA: 0x00054DB4 File Offset: 0x00052FB4
		public void SetBattleAgentCount(int agentCount)
		{
			if (this._agentCount == 0 || this._agentCount > agentCount)
			{
				this._agentCount = agentCount;
			}
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x00054DD0 File Offset: 0x00052FD0
		public Vec2 GetFormationSpawnPosition(BattleSideEnum side, FormationClass formationClass, bool isReinforcement)
		{
			DeploymentPlanType deploymentPlanType = (isReinforcement ? DeploymentPlanType.Reinforcement : DeploymentPlanType.Initial);
			return this._deploymentPlan.GetFormationPlan(side, formationClass, deploymentPlanType).CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.None).AsVec2;
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x00054E04 File Offset: 0x00053004
		public FormationClass GetFormationSpawnClass(BattleSideEnum side, FormationClass formationClass, bool isReinforcement)
		{
			DeploymentPlanType deploymentPlanType = (isReinforcement ? DeploymentPlanType.Reinforcement : DeploymentPlanType.Initial);
			return this._deploymentPlan.GetFormationPlan(side, formationClass, deploymentPlanType).SpawnClass;
		}

		// Token: 0x06001882 RID: 6274 RVA: 0x00054E2C File Offset: 0x0005302C
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
				this.SpawnFormation(agentFormation);
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
				ItemObject itemObject = equipment[EquipmentIndex.ExtraWeaponSlot].Item;
				if (itemObject == null)
				{
					itemObject = agentBuildData.AgentBannerItem;
				}
				if (itemObject != null)
				{
					equipment[EquipmentIndex.ExtraWeaponSlot] = default(EquipmentElement);
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
			ItemObject item = equipment[EquipmentIndex.ArmorItemEndSlot].Item;
			if (item != null && item.HasHorseComponent && item.HorseComponent.IsRideable)
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
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new CreateAgent(agent, flag, valueOrDefault, valueOrDefault2, networkCommunicator2));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			MultiplayerMissionAgentVisualSpawnComponent missionBehavior = this.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
			if (missionBehavior != null && agentBuildData.AgentMissionPeer != null && agentBuildData.AgentMissionPeer.IsMine && agentBuildData.AgentVisualsIndex == 0)
			{
				try
				{
					missionBehavior.OnMyAgentSpawned();
				}
				catch (Exception ex)
				{
					Debug.Print("OnMyAgentSpawnedFromVisual exception", 0, Debug.DebugColor.White, 17592186044416UL);
					Debug.Print(ex.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
				}
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
				else
				{
					agent.SetRidingOrder(1);
				}
			}
			return agent;
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x000557AC File Offset: 0x000539AC
		public void SetInitialAgentCountForSide(BattleSideEnum side, int agentCount)
		{
			if (side >= BattleSideEnum.Defender && side < BattleSideEnum.NumSides)
			{
				this._initialAgentCountPerSide[(int)side] = agentCount;
				return;
			}
			Debug.FailedAssert("Cannot set initial agent count.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "SetInitialAgentCountForSide", 3759);
		}

		// Token: 0x06001884 RID: 6276 RVA: 0x000557E8 File Offset: 0x000539E8
		public void SpawnFormation(Formation formation)
		{
			IFormationDeploymentPlan formationPlan = this._deploymentPlan.GetFormationPlan(formation.Team.Side, formation.FormationIndex, DeploymentPlanType.Initial);
			if (formationPlan.PlannedTroopCount > 0 && formationPlan.HasDimensions)
			{
				formation.FormOrder = FormOrder.FormOrderCustom(formationPlan.PlannedWidth);
			}
			formation.SetPositioning(new WorldPosition?(formationPlan.CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.None)), new Vec2?(formationPlan.GetDirection()), null);
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x0005585B File Offset: 0x00053A5B
		public Agent SpawnMonster(ItemRosterElement rosterElement, ItemRosterElement harnessRosterElement, in Vec3 initialPosition, in Vec2 initialDirection, int forcedAgentIndex = -1)
		{
			return this.SpawnMonster(rosterElement.EquipmentElement, harnessRosterElement.EquipmentElement, initialPosition, initialDirection, forcedAgentIndex);
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x00055878 File Offset: 0x00053A78
		public Agent RespawnTroop(Agent agent, bool isAlarmed, bool wieldInitialWeapons, bool forceDismounted, string specialActionSetSuffix = null, ItemObject bannerItem = null, bool useTroopClassForSpawn = false)
		{
			IAgentOriginBase origin = agent.Origin;
			bool flag = this.MainAgent != null && agent.IsFriendOf(this.MainAgent);
			bool flag2 = agent.Formation != null;
			bool hasMount = agent.HasMount;
			FormationClass formationClass = (flag2 ? agent.Formation.FormationIndex : FormationClass.NumberOfAllFormations);
			Vec3 position = agent.Position;
			Vec2 movementDirection = agent.GetMovementDirection();
			if (flag2)
			{
				agent.Formation.Team.DetachmentManager.OnAgentRemoved(agent);
				agent.Formation = null;
			}
			agent.FadeOut(true, true);
			return this.SpawnTroop(origin, flag, flag2, hasMount, false, 0, 0, isAlarmed, wieldInitialWeapons, forceDismounted, new Vec3?(position), new Vec2?(movementDirection), specialActionSetSuffix, bannerItem, formationClass, useTroopClassForSpawn);
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x00055928 File Offset: 0x00053B28
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

		// Token: 0x06001888 RID: 6280 RVA: 0x000559A8 File Offset: 0x00053BA8
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
					formation = agentTeam.GetFormation(troop.GetFormationClass());
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
			if (bannerItem != null && bannerItem.IsBannerItem && bannerItem.BannerComponent != null)
			{
				agentBuildData.BannerItem(bannerItem);
				ItemObject bannerBearerReplacementWeapon = MissionGameModels.Current.BattleBannerBearersModel.GetBannerBearerReplacementWeapon(troop);
				agentBuildData.BannerReplacementWeaponItem(bannerBearerReplacementWeapon);
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
				agent.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp);
			}
			if (!string.IsNullOrEmpty(specialActionSetSuffix))
			{
				AnimationSystemData animationSystemData = agentBuildData.AgentMonster.FillAnimationSystemData(MBGlobals.GetActionSetWithSuffix(agentBuildData.AgentMonster, agentBuildData.AgentIsFemale, specialActionSetSuffix), agent.Character.GetStepSize(), false);
				agent.SetActionSet(ref animationSystemData);
			}
			return agent;
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x00055BA4 File Offset: 0x00053DA4
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
						GameNetwork.WriteMessage(new ReplaceBotWithPlayer(networkPeer, botAgent));
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

		// Token: 0x0600188A RID: 6282 RVA: 0x00055C98 File Offset: 0x00053E98
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

		// Token: 0x0600188B RID: 6283 RVA: 0x00055D10 File Offset: 0x00053F10
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

		// Token: 0x0600188C RID: 6284 RVA: 0x00055D88 File Offset: 0x00053F88
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

		// Token: 0x0600188D RID: 6285 RVA: 0x00055DC4 File Offset: 0x00053FC4
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

		// Token: 0x0600188E RID: 6286 RVA: 0x00055F40 File Offset: 0x00054140
		private void StopSoundEvents()
		{
			if (this._ambientSoundEvent != null)
			{
				this._ambientSoundEvent.Stop();
			}
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x00055F58 File Offset: 0x00054158
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

		// Token: 0x06001890 RID: 6288 RVA: 0x00055FB0 File Offset: 0x000541B0
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

		// Token: 0x06001891 RID: 6289 RVA: 0x00056020 File Offset: 0x00054220
		public void RemoveMissionBehavior(MissionBehavior missionBehavior)
		{
			missionBehavior.OnRemoveBehavior();
			MissionBehaviorType behaviorType = missionBehavior.BehaviorType;
			if (behaviorType != MissionBehaviorType.Logic)
			{
				if (behaviorType != MissionBehaviorType.Other)
				{
					Debug.FailedAssert("Invalid behavior type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "RemoveMissionBehavior", 4181);
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

		// Token: 0x06001892 RID: 6290 RVA: 0x00056094 File Offset: 0x00054294
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
				Debug.FailedAssert("Player is neither attacker nor defender.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "JoinEnemyTeam", 4225);
			}
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x00056164 File Offset: 0x00054364
		public void OnEndMissionResult()
		{
			MissionLogic[] array = this.MissionLogics.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnBattleEnded();
			}
			this.RetreatMission();
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x0005619C File Offset: 0x0005439C
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

		// Token: 0x06001895 RID: 6293 RVA: 0x000561F0 File Offset: 0x000543F0
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

		// Token: 0x06001896 RID: 6294 RVA: 0x00056244 File Offset: 0x00054444
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

		// Token: 0x06001897 RID: 6295 RVA: 0x000562A8 File Offset: 0x000544A8
		private bool CheckMissionEnded()
		{
			foreach (MissionLogic missionLogic in this.MissionLogics)
			{
				MissionResult missionResult = null;
				if (missionLogic.MissionEnded(ref missionResult))
				{
					this.MissionResult = missionResult;
					this.MissionEnded = true;
					this.MissionResultReady(missionResult);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x0005631C File Offset: 0x0005451C
		private void MissionResultReady(MissionResult missionResult)
		{
			foreach (MissionLogic missionLogic in this.MissionLogics)
			{
				missionLogic.OnMissionResultReady(missionResult);
			}
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x00056370 File Offset: 0x00054570
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

		// Token: 0x0600189A RID: 6298 RVA: 0x0005646C File Offset: 0x0005466C
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

		// Token: 0x0600189B RID: 6299 RVA: 0x00056514 File Offset: 0x00054714
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

		// Token: 0x0600189C RID: 6300 RVA: 0x00056608 File Offset: 0x00054808
		public WorldPosition FindBestDefendingPosition(WorldPosition enemyPosition, WorldPosition defendedPosition)
		{
			return this.GetBestSlopeAngleHeightPosForDefending(enemyPosition, defendedPosition, 10, 0.5f, 4f, 0.5f, 0.70710677f, 0.1f, 1f, 0.7f, 0.5f, 1.2f, 20f, 0.6f);
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x00056656 File Offset: 0x00054856
		public WorldPosition FindPositionWithBiggestSlopeTowardsDirectionInSquare(ref WorldPosition center, float halfSize, ref WorldPosition referencePosition)
		{
			return this.GetBestSlopeTowardsDirection(ref center, halfSize, ref referencePosition);
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x00056664 File Offset: 0x00054864
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
				GameNetwork.WriteMessage(new CreateMissile(num, shooterAgent, EquipmentIndex.None, missileWeapon, position, direction, speed, orientation, addRigidBody, missionObjectToIgnore, false));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x00056764 File Offset: 0x00054964
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
					GameNetwork.WriteMessage(new CreateMissile(num4, shooterAgent, weaponIndex, MissionWeapon.Invalid, position, vec, num2, orientation, hasRigidBody, null, isPrimaryWeaponShot));
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

		// Token: 0x060018A0 RID: 6304 RVA: 0x00056994 File Offset: 0x00054B94
		[UsedImplicitly]
		[MBCallback]
		internal AgentState GetAgentState(Agent affectorAgent, Agent agent, DamageTypes damageType)
		{
			float num;
			float agentStateProbability = MissionGameModels.Current.AgentDecideKilledOrUnconsciousModel.GetAgentStateProbability(affectorAgent, agent, damageType, out num);
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
			if (flag && affectorAgent.Team != null && agent.Team != null && affectorAgent.Team == agent.Team)
			{
				flag = false;
			}
			for (int i = 0; i < this.MissionBehaviors.Count; i++)
			{
				this.MissionBehaviors[i].OnGetAgentState(agent, flag);
			}
			return agentState;
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x00056A90 File Offset: 0x00054C90
		public void OnAgentMount(Agent agent)
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnAgentMount(agent);
			}
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x00056AE4 File Offset: 0x00054CE4
		public void OnAgentDismount(Agent agent)
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnAgentDismount(agent);
			}
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x00056B38 File Offset: 0x00054D38
		public void OnObjectUsed(Agent userAgent, UsableMissionObject usableGameObject)
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnObjectUsed(userAgent, usableGameObject);
			}
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x00056B8C File Offset: 0x00054D8C
		public void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usableGameObject)
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnObjectStoppedBeingUsed(userAgent, usableGameObject);
			}
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x00056BE0 File Offset: 0x00054DE0
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

		// Token: 0x060018A6 RID: 6310 RVA: 0x00056C45 File Offset: 0x00054E45
		public Agent GetClosestEnemyAgent(Team team, Vec3 position, float radius)
		{
			return this.GetClosestEnemyAgent(team.MBTeam, position, radius);
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x00056C55 File Offset: 0x00054E55
		public Agent GetClosestAllyAgent(Team team, Vec3 position, float radius)
		{
			return this.GetClosestAllyAgent(team.MBTeam, position, radius);
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x00056C65 File Offset: 0x00054E65
		public int GetNearbyEnemyAgentCount(Team team, Vec2 position, float radius)
		{
			return this.GetNearbyEnemyAgentCount(team.MBTeam, position, radius);
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x00056C78 File Offset: 0x00054E78
		public bool HasAnyAgentsOfSideInRange(Vec3 origin, float radius, BattleSideEnum side)
		{
			Team team = ((side == BattleSideEnum.Attacker) ? this.AttackerTeam : this.DefenderTeam);
			return MBAPI.IMBMission.HasAnyAgentsOfTeamAround(this.Pointer, origin, radius, team.MBTeam.Index);
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x00056CB8 File Offset: 0x00054EB8
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

		// Token: 0x060018AB RID: 6315 RVA: 0x00056D58 File Offset: 0x00054F58
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
			if (missionObject is SpawnedItemEntity)
			{
				Debug.Print(string.Concat(new object[]
				{
					"SpawnedItemEntity with id: ",
					missionObject.Id.Id,
					" is removed. Remove reason: ",
					removeReason
				}), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			this._activeMissionObjects.Remove(missionObject);
			return this._missionObjects.Remove(missionObject);
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x00056E20 File Offset: 0x00055020
		public bool AgentLookingAtAgent(Agent agent1, Agent agent2)
		{
			Vec3 vec = agent2.Position - agent1.Position;
			float num = vec.Normalize();
			float num2 = Vec3.DotProduct(vec, agent1.LookDirection);
			return num2 < 1f && num2 > 0.86f && num < 4f;
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x00056E73 File Offset: 0x00055073
		public Agent FindAgentWithIndex(int agentId)
		{
			return this.FindAgentWithIndexAux(agentId);
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x00056E7C File Offset: 0x0005507C
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

		// Token: 0x060018AF RID: 6319 RVA: 0x00056F14 File Offset: 0x00055114
		public static Team GetAgentTeam(IAgentOriginBase troopOrigin, bool isPlayerSide)
		{
			if (Mission.Current == null)
			{
				Debug.FailedAssert("Mission current is null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "GetAgentTeam", 4913);
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

		// Token: 0x060018B0 RID: 6320 RVA: 0x00056F90 File Offset: 0x00055190
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

		// Token: 0x060018B1 RID: 6321 RVA: 0x00056FDC File Offset: 0x000551DC
		public void OnRenderingStarted()
		{
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnRenderingStarted();
			}
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x0005702C File Offset: 0x0005522C
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

		// Token: 0x060018B3 RID: 6323 RVA: 0x00057074 File Offset: 0x00055274
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

		// Token: 0x060018B4 RID: 6324 RVA: 0x00057138 File Offset: 0x00055338
		public void ShowInMissionLoadingScreen(int durationInSecond, Action onLoadingEndedAction)
		{
			this._inMissionLoadingScreenTimer = new Timer(this.CurrentTime, (float)durationInSecond, true);
			this._onLoadingEndedAction = onLoadingEndedAction;
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x0005715A File Offset: 0x0005535A
		public bool CanAgentRout(Agent agent)
		{
			return (agent.IsRunningAway || (agent.CommonAIComponent != null && agent.CommonAIComponent.IsRetreating)) && agent.RiderAgent == null && (this.CanAgentRout_AdditionalCondition == null || this.CanAgentRout_AdditionalCondition(agent));
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x00057199 File Offset: 0x00055399
		internal bool CanGiveDamageToAgentShield(Agent attacker, WeaponComponentData attackerWeapon, Agent defender)
		{
			return MissionGameModels.Current.AgentApplyDamageModel.CanWeaponIgnoreFriendlyFireChecks(attackerWeapon) || !this.CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(attacker, defender);
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x000571BC File Offset: 0x000553BC
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
					if (!collisionData.IsAlternativeAttack && attacker.IsDoingPassiveAttack && !MBNetwork.IsSessionActive && ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.ReportDamage) > 0f)
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

		// Token: 0x060018B8 RID: 6328 RVA: 0x00057544 File Offset: 0x00055744
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

		// Token: 0x060018B9 RID: 6329 RVA: 0x00057598 File Offset: 0x00055798
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

		// Token: 0x060018BA RID: 6330 RVA: 0x00057628 File Offset: 0x00055828
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

		// Token: 0x060018BB RID: 6331 RVA: 0x000578C0 File Offset: 0x00055AC0
		private void RegisterBlow(Agent attacker, Agent victim, GameEntity realHitEntity, Blow b, ref AttackCollisionData collisionData, in MissionWeapon attackerWeapon, ref CombatLogData combatLogData)
		{
			b.VictimBodyPart = collisionData.VictimHitBodyPart;
			if (!collisionData.AttackBlockedWithShield)
			{
				Blow blow;
				AttackCollisionData attackCollisionData;
				attacker.CreateBlowFromBlowAsReflection(b, collisionData, out blow, out attackCollisionData);
				if (collisionData.IsColliderAgent)
				{
					if (b.SelfInflictedDamage > 0 && attacker != null && attacker.IsFriendOf(victim))
					{
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
					MissionWeapon missionWeapon = (b.IsMissile ? this._missiles[b.WeaponRecord.AffectorWeaponSlotOrMissileIndex].Weapon : (b.WeaponRecord.HasWeapon() ? attacker.Equipment[b.WeaponRecord.AffectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid));
					this.OnEntityHit(realHitEntity, attacker, b.InflictedDamage, (DamageTypes)collisionData.DamageType, b.Position, b.SwingDirection, missionWeapon);
					if (b.SelfInflictedDamage > 0)
					{
						attacker.RegisterBlow(blow, attackCollisionData);
					}
				}
			}
			foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
			{
				missionBehavior.OnRegisterBlow(attacker, victim, realHitEntity, b, ref collisionData, attackerWeapon);
			}
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x00057A84 File Offset: 0x00055C84
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

		// Token: 0x060018BD RID: 6333 RVA: 0x00057B98 File Offset: 0x00055D98
		private Blow CreateMissileBlow(Agent attackerAgent, in AttackCollisionData collisionData, in MissionWeapon attackerWeapon, Vec3 missilePosition, Vec3 missileStartingPosition)
		{
			Blow blow = new Blow(attackerAgent.Index);
			MissionWeapon missionWeapon = attackerWeapon;
			blow.BlowFlag = (missionWeapon.CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.CanKnockDown) ? BlowFlags.KnockDown : BlowFlags.None);
			AttackCollisionData attackCollisionData = collisionData;
			blow.Direction = attackCollisionData.MissileVelocity.NormalizedCopy();
			blow.SwingDirection = blow.Direction;
			attackCollisionData = collisionData;
			blow.Position = attackCollisionData.CollisionGlobalPosition;
			attackCollisionData = collisionData;
			blow.BoneIndex = attackCollisionData.CollisionBoneIndex;
			attackCollisionData = collisionData;
			blow.StrikeType = (StrikeType)attackCollisionData.StrikeType;
			attackCollisionData = collisionData;
			blow.DamageType = (DamageTypes)attackCollisionData.DamageType;
			attackCollisionData = collisionData;
			blow.VictimBodyPart = attackCollisionData.VictimHitBodyPart;
			missionWeapon = attackerWeapon;
			ItemObject item = missionWeapon.Item;
			missionWeapon = attackerWeapon;
			WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
			attackCollisionData = collisionData;
			int affectorWeaponSlotOrMissileIndex = attackCollisionData.AffectorWeaponSlotOrMissileIndex;
			Monster monster = attackerAgent.Monster;
			missionWeapon = attackerWeapon;
			sbyte boneToAttachForItemFlags = monster.GetBoneToAttachForItemFlags(missionWeapon.Item.ItemFlags);
			attackCollisionData = collisionData;
			blow.WeaponRecord.FillAsMissileBlow(item, currentUsageItem, affectorWeaponSlotOrMissileIndex, boneToAttachForItemFlags, missileStartingPosition, missilePosition, attackCollisionData.MissileVelocity);
			blow.BaseMagnitude = collisionData.BaseMagnitude;
			blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
			blow.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
			blow.InflictedDamage = collisionData.InflictedDamage;
			blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
			blow.DamageCalculated = true;
			return blow;
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x00057D24 File Offset: 0x00055F24
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

		// Token: 0x060018BF RID: 6335 RVA: 0x00057DAC File Offset: 0x00055FAC
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
			blow.Position = attackCollisionData.CollisionGlobalPosition;
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

		// Token: 0x060018C0 RID: 6336 RVA: 0x0005816C File Offset: 0x0005636C
		internal float OnAgentHit(Agent affectedAgent, Agent affectorAgent, in Blow b, in AttackCollisionData collisionData, bool isBlocked, float damagedHp)
		{
			float num = -1f;
			bool flag = false;
			int affectorWeaponSlotOrMissileIndex = b.WeaponRecord.AffectorWeaponSlotOrMissileIndex;
			Blow blow = b;
			bool isMissile = blow.IsMissile;
			int inflictedDamage = b.InflictedDamage;
			blow = b;
			float num2 = (blow.IsMissile ? (b.Position - b.WeaponRecord.StartingPosition).Length : 0f);
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

		// Token: 0x060018C1 RID: 6337 RVA: 0x000582F8 File Offset: 0x000564F8
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
			AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(this, blowInput.Position.AsVec2, num, true);
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
					float num5 = globalFrame.TransformToParent(skeleton.GetBoneEntitialFrame(b2).origin).DistanceSquared(blowInput.Position);
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

		// Token: 0x060018C2 RID: 6338 RVA: 0x000585D4 File Offset: 0x000567D4
		[UsedImplicitly]
		[MBCallback]
		internal void OnMissileRemoved(int missileIndex)
		{
			this._missiles.Remove(missileIndex);
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x000585E4 File Offset: 0x000567E4
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
				else if (victim.IsHuman && !attacker.IsEnemyOf(victim))
				{
					flag5 = true;
				}
				else if (flag && attacker != null && attacker.Controller == Agent.ControllerType.AI && victim.RiderAgent != null && attacker.IsFriendOf(victim.RiderAgent))
				{
					flag5 = true;
				}
				if (flag5)
				{
					if (flag && attacker == Agent.Main && attacker.IsFriendOf(victim))
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
			MatrixFrame matrixFrame;
			bool flag11;
			if (!collisionData.MissileHasPhysics && missileCollisionReaction == Mission.MissileCollisionReaction.Stick)
			{
				matrixFrame = this.CalculateAttachedLocalFrame(attachGlobalFrame, collisionData, missile.Weapon.CurrentUsageItem, victim, hitEntity, movementVelocity, missileAngularVelocity, affectedShieldGlobalFrame, true);
				flag11 = true;
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

		// Token: 0x060018C4 RID: 6340 RVA: 0x00058D54 File Offset: 0x00056F54
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
				GameNetwork.WriteMessage(new HandleMissileCollisionReaction(missileIndex, collisionReaction, attachLocalFrame, isAttachedFrameLocal, attackerAgent, attachedAgent, attachedToShield, attachedBoneIndex, attachedMissionObject, bounceBackVelocity, bounceBackAngularVelocity, missionObjectId.Id));
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

		// Token: 0x060018C5 RID: 6341 RVA: 0x00058ED4 File Offset: 0x000570D4
		[UsedImplicitly]
		[MBCallback]
		internal void MissileCalculatePassbySoundParametersCallbackMT(int missileIndex, ref SoundEventParameter soundEventParameter)
		{
			this._missiles[missileIndex].CalculatePassbySoundParametersMT(ref soundEventParameter);
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x00058EE8 File Offset: 0x000570E8
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

		// Token: 0x060018C7 RID: 6343 RVA: 0x0005903C File Offset: 0x0005723C
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

		// Token: 0x060018C8 RID: 6344 RVA: 0x000590F4 File Offset: 0x000572F4
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
				blow.WeaponRecord.CurrentPosition = blow.Position;
				blow.WeaponRecord.StartingPosition = blow.Position;
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
				blow.Position = agent.Position;
				AttackCollisionData attackCollisionDataForDebugPurpose = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Abdomen, b, Agent.UsageDirection.AttackLeft, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.Position, Vec3.Zero, Vec3.Zero, agent.Velocity, Vec3.Up);
				agent.RegisterBlow(blow, attackCollisionDataForDebugPurpose);
			}
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x00059380 File Offset: 0x00057580
		public void KillAgentCheat(Agent agent)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				Agent agent2 = this.MainAgent ?? agent;
				Blow blow = new Blow(agent2.Index);
				blow.DamageType = DamageTypes.Blunt;
				blow.BoneIndex = agent.Monster.HeadLookDirectionBoneIndex;
				blow.Position = agent.Position;
				blow.Position.z = blow.Position.z + agent.GetEyeGlobalHeight();
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
				AttackCollisionData attackCollisionDataForDebugPurpose = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Head, mainHandItemBoneIndex, Agent.UsageDirection.AttackLeft, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.Position, Vec3.Zero, Vec3.Zero, agent.Velocity, Vec3.Up);
				agent.RegisterBlow(blow, attackCollisionDataForDebugPurpose);
			}
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x00059604 File Offset: 0x00057804
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

		// Token: 0x060018CB RID: 6347 RVA: 0x0005978C File Offset: 0x0005798C
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

		// Token: 0x060018CC RID: 6348 RVA: 0x00059805 File Offset: 0x00057A05
		public float GetDamageMultiplierOfCombatDifficulty(Agent victimAgent, Agent attackerAgent = null)
		{
			if (MissionGameModels.Current.MissionDifficultyModel != null)
			{
				return MissionGameModels.Current.MissionDifficultyModel.GetDamageMultiplierOfCombatDifficulty(victimAgent, attackerAgent);
			}
			return 1f;
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x0005982C File Offset: 0x00057A2C
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

		// Token: 0x060018CE RID: 6350 RVA: 0x000598E4 File Offset: 0x00057AE4
		private MatrixFrame CalculateAttachedLocalFrame(in MatrixFrame attachedGlobalFrame, AttackCollisionData collisionData, WeaponComponentData missileWeapon, Agent affectedAgent, GameEntity hitEntity, Vec3 missileMovementVelocity, Vec3 missileRotationSpeed, MatrixFrame shieldGlobalFrame, bool shouldMissilePenetrate)
		{
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
			}
			else if (affectedAgent != null)
			{
				if (flag)
				{
					MBAgentVisuals agentVisuals = affectedAgent.AgentVisuals;
					matrixFrame = agentVisuals.GetGlobalFrame().TransformToParent(agentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex(collisionData.CollisionBoneIndex)).GetUnitRotFrame(affectedAgent.AgentScale)
						.TransformToLocalNonOrthogonal(ref matrixFrame);
				}
			}
			else if (hitEntity != null)
			{
				if (collisionData.CollisionBoneIndex >= 0)
				{
					matrixFrame = hitEntity.Skeleton.GetBoneEntitialFrameWithIndex(collisionData.CollisionBoneIndex).TransformToLocalNonOrthogonal(ref matrixFrame);
				}
				else
				{
					matrixFrame = hitEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref matrixFrame);
				}
			}
			else
			{
				matrixFrame.origin.z = Math.Max(matrixFrame.origin.z, -100f);
			}
			return matrixFrame;
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x00059C08 File Offset: 0x00057E08
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

		// Token: 0x060018D0 RID: 6352 RVA: 0x00059CEC File Offset: 0x00057EEC
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

		// Token: 0x060018D1 RID: 6353 RVA: 0x00059FE0 File Offset: 0x000581E0
		private void PrintAttackCollisionResults(Agent attackerAgent, Agent victimAgent, GameEntity hitEntity, ref AttackCollisionData attackCollisionData, ref CombatLogData combatLog)
		{
			if (attackCollisionData.IsColliderAgent && !attackCollisionData.AttackBlockedWithShield && (attackerAgent.CanLogCombatFor || victimAgent.CanLogCombatFor) && victimAgent.State == AgentState.Active)
			{
				this.AddCombatLogSafe(attackerAgent, victimAgent, hitEntity, combatLog);
			}
		}

		// Token: 0x060018D2 RID: 6354 RVA: 0x0005A030 File Offset: 0x00058230
		private void AddCombatLogSafe(Agent attackerAgent, Agent victimAgent, GameEntity hitEntity, CombatLogData combatLog)
		{
			combatLog.SetVictimAgent(victimAgent);
			if (GameNetwork.IsServerOrRecorder)
			{
				CombatLogNetworkMessage combatLogNetworkMessage = new CombatLogNetworkMessage(attackerAgent, victimAgent, hitEntity, combatLog);
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

		// Token: 0x060018D3 RID: 6355 RVA: 0x0005A118 File Offset: 0x00058318
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

		// Token: 0x060018D4 RID: 6356 RVA: 0x0005A1D4 File Offset: 0x000583D4
		public int GetNearbyAllyAgentsCount(Vec2 center, float radius, Team team)
		{
			return this.GetNearbyAgentsCountAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Friend);
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x0005A1E5 File Offset: 0x000583E5
		public MBList<Agent> GetNearbyAllyAgents(Vec2 center, float radius, Team team, MBList<Agent> agents)
		{
			agents.Clear();
			this.GetNearbyAgentsAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Friend, agents);
			return agents;
		}

		// Token: 0x060018D6 RID: 6358 RVA: 0x0005A201 File Offset: 0x00058401
		public MBList<Agent> GetNearbyEnemyAgents(Vec2 center, float radius, Team team, MBList<Agent> agents)
		{
			agents.Clear();
			this.GetNearbyAgentsAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Enemy, agents);
			return agents;
		}

		// Token: 0x060018D7 RID: 6359 RVA: 0x0005A21D File Offset: 0x0005841D
		public MBList<Agent> GetNearbyAgents(Vec2 center, float radius, MBList<Agent> agents)
		{
			agents.Clear();
			this.GetNearbyAgentsAux(center, radius, MBTeam.InvalidTeam, Mission.GetNearbyAgentsAuxType.All, agents);
			return agents;
		}

		// Token: 0x060018D8 RID: 6360 RVA: 0x0005A235 File Offset: 0x00058435
		public bool IsFormationUnitPositionAvailable(ref WorldPosition formationPosition, ref WorldPosition unitPosition, ref WorldPosition nearestAvailableUnitPosition, float manhattanDistance, Team team)
		{
			return formationPosition.IsValid && unitPosition.IsValid && (this.IsFormationUnitPositionAvailable_AdditionalCondition == null || this.IsFormationUnitPositionAvailable_AdditionalCondition(unitPosition, team)) && this.IsFormationUnitPositionAvailableAux(ref formationPosition, ref unitPosition, ref nearestAvailableUnitPosition, manhattanDistance);
		}

		// Token: 0x060018D9 RID: 6361 RVA: 0x0005A274 File Offset: 0x00058474
		public bool IsOrderPositionAvailable(in WorldPosition orderPosition, Team team)
		{
			WorldPosition worldPosition = orderPosition;
			if (!worldPosition.IsValid)
			{
				return false;
			}
			if (this.IsFormationUnitPositionAvailable_AdditionalCondition != null && !this.IsFormationUnitPositionAvailable_AdditionalCondition(orderPosition, team))
			{
				return false;
			}
			worldPosition = orderPosition;
			return this.IsPositionInsideBoundaries(worldPosition.AsVec2);
		}

		// Token: 0x060018DA RID: 6362 RVA: 0x0005A2C8 File Offset: 0x000584C8
		public bool IsFormationUnitPositionAvailable(ref WorldPosition unitPosition, Team team)
		{
			WorldPosition worldPosition = unitPosition;
			float num = 1f;
			WorldPosition invalid = WorldPosition.Invalid;
			return this.IsFormationUnitPositionAvailable(ref worldPosition, ref unitPosition, ref invalid, num, team);
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x0005A2F5 File Offset: 0x000584F5
		public bool HasSceneMapPatch()
		{
			return this.InitializerRecord.SceneHasMapPatch;
		}

		// Token: 0x060018DC RID: 6364 RVA: 0x0005A304 File Offset: 0x00058504
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

		// Token: 0x060018DD RID: 6365 RVA: 0x0005A3E8 File Offset: 0x000585E8
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

		// Token: 0x060018DE RID: 6366 RVA: 0x0005A434 File Offset: 0x00058634
		private void TickDebugAgents()
		{
		}

		// Token: 0x060018DF RID: 6367 RVA: 0x0005A438 File Offset: 0x00058638
		public void AddTimerToDynamicEntity(GameEntity gameEntity, float timeToKill = 10f)
		{
			Mission.DynamicEntityInfo dynamicEntityInfo = new Mission.DynamicEntityInfo
			{
				Entity = gameEntity,
				TimerToDisable = new Timer(this.CurrentTime, timeToKill, true)
			};
			this._dynamicEntities.Add(dynamicEntityInfo);
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x0005A471 File Offset: 0x00058671
		public void AddListener(IMissionListener listener)
		{
			this._listeners.Add(listener);
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x0005A47F File Offset: 0x0005867F
		public void RemoveListener(IMissionListener listener)
		{
			this._listeners.Remove(listener);
		}

		// Token: 0x060018E2 RID: 6370 RVA: 0x0005A490 File Offset: 0x00058690
		public void OnAgentFleeing(Agent agent)
		{
			for (int i = this.MissionBehaviors.Count - 1; i >= 0; i--)
			{
				this.MissionBehaviors[i].OnAgentFleeing(agent);
			}
			agent.OnFleeing();
		}

		// Token: 0x060018E3 RID: 6371 RVA: 0x0005A4D0 File Offset: 0x000586D0
		public void OnAgentPanicked(Agent agent)
		{
			for (int i = this.MissionBehaviors.Count - 1; i >= 0; i--)
			{
				this.MissionBehaviors[i].OnAgentPanicked(agent);
			}
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x0005A508 File Offset: 0x00058708
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

		// Token: 0x060018E5 RID: 6373 RVA: 0x0005A590 File Offset: 0x00058790
		public void SetFastForwardingFromUI(bool fastForwarding)
		{
			this.IsFastForward = fastForwarding;
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x0005A599 File Offset: 0x00058799
		public bool CheckIfBattleInRetreat()
		{
			Func<bool> isBattleInRetreatEvent = this.IsBattleInRetreatEvent;
			return isBattleInRetreatEvent != null && isBattleInRetreatEvent();
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x0005A5AC File Offset: 0x000587AC
		public void AddSpawnedItemEntityCreatedAtRuntime(SpawnedItemEntity spawnedItemEntity)
		{
			this._spawnedItemEntitiesCreatedAtRuntime.Add(spawnedItemEntity);
		}

		// Token: 0x060018E8 RID: 6376 RVA: 0x0005A5BC File Offset: 0x000587BC
		[UsedImplicitly]
		[MBCallback]
		internal static void DebugLogNativeMissionNetworkEvent(int eventEnum, string eventName, int bitCount)
		{
			int num = eventEnum + CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo.GetMaximumValue() + 1;
			DebugNetworkEventStatistics.StartEvent(eventName, num);
			DebugNetworkEventStatistics.AddDataToStatistic(bitCount);
			DebugNetworkEventStatistics.EndEvent();
		}

		// Token: 0x060018E9 RID: 6377 RVA: 0x0005A5EA File Offset: 0x000587EA
		[UsedImplicitly]
		[MBCallback]
		internal void PauseMission()
		{
			this._missionState.Paused = true;
		}

		// Token: 0x060018EA RID: 6378 RVA: 0x0005A5F8 File Offset: 0x000587F8
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

		// Token: 0x060018EB RID: 6379 RVA: 0x0005A6D0 File Offset: 0x000588D0
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

		// Token: 0x060018EC RID: 6380 RVA: 0x0005A7A8 File Offset: 0x000589A8
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

		// Token: 0x060018ED RID: 6381 RVA: 0x0005A85C File Offset: 0x00058A5C
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

		// Token: 0x060018EE RID: 6382 RVA: 0x0005A918 File Offset: 0x00058B18
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

		// Token: 0x060018EF RID: 6383 RVA: 0x0005A9BC File Offset: 0x00058BBC
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

		// Token: 0x0400074F RID: 1871
		public const int MaxRuntimeMissionObjects = 4094;

		// Token: 0x04000753 RID: 1875
		private int _lastSceneMissionObjectIdCount;

		// Token: 0x04000754 RID: 1876
		private int _lastRuntimeMissionObjectIdCount;

		// Token: 0x04000755 RID: 1877
		private bool _isMainAgentObjectInteractionEnabled = true;

		// Token: 0x04000756 RID: 1878
		private List<Mission.TimeSpeedRequest> _timeSpeedRequests = new List<Mission.TimeSpeedRequest>();

		// Token: 0x04000757 RID: 1879
		private bool _isMainAgentItemInteractionEnabled = true;

		// Token: 0x04000758 RID: 1880
		private readonly MBList<MissionObject> _activeMissionObjects;

		// Token: 0x04000759 RID: 1881
		private readonly MBList<MissionObject> _missionObjects;

		// Token: 0x0400075A RID: 1882
		private readonly List<SpawnedItemEntity> _spawnedItemEntitiesCreatedAtRuntime;

		// Token: 0x0400075B RID: 1883
		private readonly MBList<Mission.DynamicallyCreatedEntity> _addedEntitiesInfo;

		// Token: 0x0400075C RID: 1884
		private readonly Stack<ValueTuple<int, float>> _emptyRuntimeMissionObjectIds;

		// Token: 0x0400075D RID: 1885
		private static bool _isCameraFirstPerson = false;

		// Token: 0x04000761 RID: 1889
		private MissionMode _missionMode;

		// Token: 0x04000762 RID: 1890
		private float _cachedMissionTime;

		// Token: 0x04000764 RID: 1892
		private static readonly object GetNearbyAgentsAuxLock = new object();

		// Token: 0x04000765 RID: 1893
		private const float NavigationMeshHeightLimit = 1.5f;

		// Token: 0x04000766 RID: 1894
		private const float SpeedBonusFactorForSwing = 0.7f;

		// Token: 0x04000767 RID: 1895
		private const float SpeedBonusFactorForThrust = 0.5f;

		// Token: 0x04000768 RID: 1896
		private const float _exitTimeInSeconds = 0.6f;

		// Token: 0x0400076E RID: 1902
		private Dictionary<int, Mission.Missile> _missiles;

		// Token: 0x04000770 RID: 1904
		private readonly List<Mission.DynamicEntityInfo> _dynamicEntities = new List<Mission.DynamicEntityInfo>();

		// Token: 0x04000771 RID: 1905
		public bool DisableDying;

		// Token: 0x04000772 RID: 1906
		public const int MaxDamage = 2000;

		// Token: 0x04000773 RID: 1907
		public bool ForceNoFriendlyFire;

		// Token: 0x04000774 RID: 1908
		private const int MaxNavMeshPerDynamicObject = 10;

		// Token: 0x04000775 RID: 1909
		private int _NextDynamicNavMeshIdStart = 1000010;

		// Token: 0x04000776 RID: 1910
		private readonly object _lockHelper = new object();

		// Token: 0x04000777 RID: 1911
		public const int MaxNavMeshId = 1000000;

		// Token: 0x04000778 RID: 1912
		public bool IsFriendlyMission = true;

		// Token: 0x04000779 RID: 1913
		public BasicCultureObject MusicCulture;

		// Token: 0x0400077B RID: 1915
		private MBList<Agent> _activeAgents;

		// Token: 0x0400077C RID: 1916
		private List<IMissionListener> _listeners = new List<IMissionListener>();

		// Token: 0x0400077D RID: 1917
		private MissionState _missionState;

		// Token: 0x0400077E RID: 1918
		private MissionDeploymentPlan _deploymentPlan;

		// Token: 0x0400077F RID: 1919
		private List<MissionBehavior> _otherMissionBehaviors;

		// Token: 0x04000780 RID: 1920
		private BasicMissionTimer _leaveMissionTimer;

		// Token: 0x04000781 RID: 1921
		private readonly MBList<Agent> _mountsWithoutRiders;

		// Token: 0x04000782 RID: 1922
		public bool IsOrderMenuOpen;

		// Token: 0x04000783 RID: 1923
		public bool IsTransferMenuOpen;

		// Token: 0x04000784 RID: 1924
		public bool IsInPhotoMode;

		// Token: 0x04000785 RID: 1925
		private Agent _mainAgent;

		// Token: 0x04000786 RID: 1926
		private Action _onLoadingEndedAction;

		// Token: 0x04000787 RID: 1927
		private Timer _inMissionLoadingScreenTimer;

		// Token: 0x04000788 RID: 1928
		public bool AllowAiTicking = true;

		// Token: 0x04000789 RID: 1929
		private int _agentCreationIndex;

		// Token: 0x0400078A RID: 1930
		private readonly MBList<FleePosition>[] _fleePositions = new MBList<FleePosition>[3];

		// Token: 0x0400078B RID: 1931
		private bool _doesMissionRequireCivilianEquipment;

		// Token: 0x0400078C RID: 1932
		public IAgentVisualCreator AgentVisualCreator;

		// Token: 0x0400078D RID: 1933
		private readonly int[] _initialAgentCountPerSide = new int[2];

		// Token: 0x0400078E RID: 1934
		private readonly int[] _removedAgentCountPerSide = new int[2];

		// Token: 0x04000790 RID: 1936
		private ConcurrentQueue<CombatLogData> _combatLogsCreated = new ConcurrentQueue<CombatLogData>();

		// Token: 0x04000791 RID: 1937
		private MBList<Agent> _allAgents;

		// Token: 0x04000793 RID: 1939
		private List<SiegeWeapon> _attackerWeaponsForFriendlyFirePreventing = new List<SiegeWeapon>();

		// Token: 0x0400079D RID: 1949
		private float _missionEndTime;

		// Token: 0x0400079E RID: 1950
		public float MissionCloseTimeAfterFinish = 30f;

		// Token: 0x0400079F RID: 1951
		private static Mission _current = null;

		// Token: 0x040007A2 RID: 1954
		public float NextCheckTimeEndMission = 10f;

		// Token: 0x040007A4 RID: 1956
		public int NumOfFormationsSpawnedTeamOne;

		// Token: 0x040007A5 RID: 1957
		private SoundEvent _ambientSoundEvent;

		// Token: 0x040007A6 RID: 1958
		private readonly BattleSpawnPathSelector _battleSpawnPathSelector;

		// Token: 0x040007A7 RID: 1959
		private int _agentCount;

		// Token: 0x040007A8 RID: 1960
		public int NumOfFormationsSpawnedTeamTwo;

		// Token: 0x040007B4 RID: 1972
		private bool tickCompleted = true;

		// Token: 0x020004FE RID: 1278
		public class MBBoundaryCollection : IDictionary<string, ICollection<Vec2>>, ICollection<KeyValuePair<string, ICollection<Vec2>>>, IEnumerable<KeyValuePair<string, ICollection<Vec2>>>, IEnumerable, INotifyCollectionChanged
		{
			// Token: 0x06003903 RID: 14595 RVA: 0x000E6FAB File Offset: 0x000E51AB
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

			// Token: 0x06003904 RID: 14596 RVA: 0x000E6FBA File Offset: 0x000E51BA
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

			// Token: 0x1700095B RID: 2395
			// (get) Token: 0x06003905 RID: 14597 RVA: 0x000E6FC9 File Offset: 0x000E51C9
			public int Count
			{
				get
				{
					return MBAPI.IMBMission.GetBoundaryCount(this._mission.Pointer);
				}
			}

			// Token: 0x06003906 RID: 14598 RVA: 0x000E6FE0 File Offset: 0x000E51E0
			public float GetBoundaryRadius(string name)
			{
				return MBAPI.IMBMission.GetBoundaryRadius(this._mission.Pointer, name);
			}

			// Token: 0x1700095C RID: 2396
			// (get) Token: 0x06003907 RID: 14599 RVA: 0x000E6FF8 File Offset: 0x000E51F8
			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06003908 RID: 14600 RVA: 0x000E6FFC File Offset: 0x000E51FC
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

			// Token: 0x06003909 RID: 14601 RVA: 0x000E7134 File Offset: 0x000E5334
			internal MBBoundaryCollection(Mission mission)
			{
				this._mission = mission;
			}

			// Token: 0x0600390A RID: 14602 RVA: 0x000E7143 File Offset: 0x000E5343
			public void Add(KeyValuePair<string, ICollection<Vec2>> item)
			{
				this.Add(item.Key, item.Value);
			}

			// Token: 0x0600390B RID: 14603 RVA: 0x000E715C File Offset: 0x000E535C
			public void Clear()
			{
				foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
				{
					this.Remove(keyValuePair.Key);
				}
			}

			// Token: 0x0600390C RID: 14604 RVA: 0x000E71AC File Offset: 0x000E53AC
			public bool Contains(KeyValuePair<string, ICollection<Vec2>> item)
			{
				return this.ContainsKey(item.Key);
			}

			// Token: 0x0600390D RID: 14605 RVA: 0x000E71BC File Offset: 0x000E53BC
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

			// Token: 0x0600390E RID: 14606 RVA: 0x000E7238 File Offset: 0x000E5438
			public bool Remove(KeyValuePair<string, ICollection<Vec2>> item)
			{
				return this.Remove(item.Key);
			}

			// Token: 0x1700095D RID: 2397
			// (get) Token: 0x0600390F RID: 14607 RVA: 0x000E7248 File Offset: 0x000E5448
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

			// Token: 0x1700095E RID: 2398
			// (get) Token: 0x06003910 RID: 14608 RVA: 0x000E72A0 File Offset: 0x000E54A0
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

			// Token: 0x1700095F RID: 2399
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

			// Token: 0x06003913 RID: 14611 RVA: 0x000E7342 File Offset: 0x000E5542
			public void Add(string name, ICollection<Vec2> points)
			{
				this.Add(name, points, true);
			}

			// Token: 0x06003914 RID: 14612 RVA: 0x000E7350 File Offset: 0x000E5550
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

			// Token: 0x06003915 RID: 14613 RVA: 0x000E745C File Offset: 0x000E565C
			public bool ContainsKey(string name)
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				return this.GetBoundaryPoints(name).Count > 0;
			}

			// Token: 0x06003916 RID: 14614 RVA: 0x000E747C File Offset: 0x000E567C
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

			// Token: 0x06003917 RID: 14615 RVA: 0x000E754C File Offset: 0x000E574C
			public bool TryGetValue(string name, out ICollection<Vec2> points)
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				points = this.GetBoundaryPoints(name);
				return points.Count > 0;
			}

			// Token: 0x06003918 RID: 14616 RVA: 0x000E7570 File Offset: 0x000E5770
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

			// Token: 0x1400009E RID: 158
			// (add) Token: 0x06003919 RID: 14617 RVA: 0x000E75D0 File Offset: 0x000E57D0
			// (remove) Token: 0x0600391A RID: 14618 RVA: 0x000E7608 File Offset: 0x000E5808
			public event NotifyCollectionChangedEventHandler CollectionChanged;

			// Token: 0x04001B5D RID: 7005
			private readonly Mission _mission;
		}

		// Token: 0x020004FF RID: 1279
		public class DynamicallyCreatedEntity
		{
			// Token: 0x0600391B RID: 14619 RVA: 0x000E763D File Offset: 0x000E583D
			public DynamicallyCreatedEntity(string prefab, MissionObjectId objectId, MatrixFrame frame, ref List<MissionObjectId> childObjectIds)
			{
				this.Prefab = prefab;
				this.ObjectId = objectId;
				this.Frame = frame;
				this.ChildObjectIds = childObjectIds;
			}

			// Token: 0x04001B5F RID: 7007
			public string Prefab;

			// Token: 0x04001B60 RID: 7008
			public MissionObjectId ObjectId;

			// Token: 0x04001B61 RID: 7009
			public MatrixFrame Frame;

			// Token: 0x04001B62 RID: 7010
			public List<MissionObjectId> ChildObjectIds;
		}

		// Token: 0x02000500 RID: 1280
		[Flags]
		public enum WeaponSpawnFlags
		{
			// Token: 0x04001B64 RID: 7012
			None = 0,
			// Token: 0x04001B65 RID: 7013
			WithHolster = 1,
			// Token: 0x04001B66 RID: 7014
			WithoutHolster = 2,
			// Token: 0x04001B67 RID: 7015
			AsMissile = 4,
			// Token: 0x04001B68 RID: 7016
			WithPhysics = 8,
			// Token: 0x04001B69 RID: 7017
			WithStaticPhysics = 16,
			// Token: 0x04001B6A RID: 7018
			UseAnimationSpeed = 32,
			// Token: 0x04001B6B RID: 7019
			CannotBePickedUp = 64
		}

		// Token: 0x02000501 RID: 1281
		[EngineStruct("Mission_combat_type")]
		public enum MissionCombatType
		{
			// Token: 0x04001B6D RID: 7021
			Combat,
			// Token: 0x04001B6E RID: 7022
			ArenaCombat,
			// Token: 0x04001B6F RID: 7023
			NoCombat
		}

		// Token: 0x02000502 RID: 1282
		public enum BattleSizeType
		{
			// Token: 0x04001B71 RID: 7025
			Battle,
			// Token: 0x04001B72 RID: 7026
			Siege,
			// Token: 0x04001B73 RID: 7027
			SallyOut
		}

		// Token: 0x02000503 RID: 1283
		[EngineStruct("Agent_creation_result")]
		internal struct AgentCreationResult
		{
			// Token: 0x04001B74 RID: 7028
			internal int Index;

			// Token: 0x04001B75 RID: 7029
			internal UIntPtr AgentPtr;

			// Token: 0x04001B76 RID: 7030
			internal UIntPtr PositionPtr;

			// Token: 0x04001B77 RID: 7031
			internal UIntPtr IndexPtr;

			// Token: 0x04001B78 RID: 7032
			internal UIntPtr FlagsPtr;

			// Token: 0x04001B79 RID: 7033
			internal UIntPtr StatePtr;
		}

		// Token: 0x02000504 RID: 1284
		public struct TimeSpeedRequest
		{
			// Token: 0x17000960 RID: 2400
			// (get) Token: 0x0600391C RID: 14620 RVA: 0x000E7663 File Offset: 0x000E5863
			// (set) Token: 0x0600391D RID: 14621 RVA: 0x000E766B File Offset: 0x000E586B
			public float RequestedTimeSpeed { get; private set; }

			// Token: 0x17000961 RID: 2401
			// (get) Token: 0x0600391E RID: 14622 RVA: 0x000E7674 File Offset: 0x000E5874
			// (set) Token: 0x0600391F RID: 14623 RVA: 0x000E767C File Offset: 0x000E587C
			public int RequestID { get; private set; }

			// Token: 0x06003920 RID: 14624 RVA: 0x000E7685 File Offset: 0x000E5885
			public TimeSpeedRequest(float requestedTime, int requestID)
			{
				this.RequestedTimeSpeed = requestedTime;
				this.RequestID = requestID;
			}
		}

		// Token: 0x02000505 RID: 1285
		private enum GetNearbyAgentsAuxType
		{
			// Token: 0x04001B7D RID: 7037
			Friend = 1,
			// Token: 0x04001B7E RID: 7038
			Enemy,
			// Token: 0x04001B7F RID: 7039
			All
		}

		// Token: 0x02000506 RID: 1286
		public class Missile : MBMissile
		{
			// Token: 0x06003921 RID: 14625 RVA: 0x000E7695 File Offset: 0x000E5895
			public Missile(Mission mission, GameEntity entity)
				: base(mission)
			{
				this.Entity = entity;
			}

			// Token: 0x17000962 RID: 2402
			// (get) Token: 0x06003922 RID: 14626 RVA: 0x000E76A5 File Offset: 0x000E58A5
			// (set) Token: 0x06003923 RID: 14627 RVA: 0x000E76AD File Offset: 0x000E58AD
			public GameEntity Entity { get; private set; }

			// Token: 0x17000963 RID: 2403
			// (get) Token: 0x06003924 RID: 14628 RVA: 0x000E76B6 File Offset: 0x000E58B6
			// (set) Token: 0x06003925 RID: 14629 RVA: 0x000E76BE File Offset: 0x000E58BE
			public MissionWeapon Weapon { get; set; }

			// Token: 0x17000964 RID: 2404
			// (get) Token: 0x06003926 RID: 14630 RVA: 0x000E76C7 File Offset: 0x000E58C7
			// (set) Token: 0x06003927 RID: 14631 RVA: 0x000E76CF File Offset: 0x000E58CF
			public Agent ShooterAgent { get; set; }

			// Token: 0x17000965 RID: 2405
			// (get) Token: 0x06003928 RID: 14632 RVA: 0x000E76D8 File Offset: 0x000E58D8
			// (set) Token: 0x06003929 RID: 14633 RVA: 0x000E76E0 File Offset: 0x000E58E0
			public MissionObject MissionObjectToIgnore { get; set; }

			// Token: 0x0600392A RID: 14634 RVA: 0x000E76EC File Offset: 0x000E58EC
			public void CalculatePassbySoundParametersMT(ref SoundEventParameter soundEventParameter)
			{
				if (this.Weapon.CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.CanPenetrateShield))
				{
					soundEventParameter.Update("impactModifier", 0.3f);
				}
			}

			// Token: 0x0600392B RID: 14635 RVA: 0x000E772C File Offset: 0x000E592C
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
					Debug.FailedAssert("Unknown missile type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", "CalculateBounceBackVelocity", 151);
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

		// Token: 0x02000507 RID: 1287
		public struct SpectatorData
		{
			// Token: 0x17000966 RID: 2406
			// (get) Token: 0x0600392C RID: 14636 RVA: 0x000E7B42 File Offset: 0x000E5D42
			// (set) Token: 0x0600392D RID: 14637 RVA: 0x000E7B4A File Offset: 0x000E5D4A
			public Agent AgentToFollow { get; private set; }

			// Token: 0x17000967 RID: 2407
			// (get) Token: 0x0600392E RID: 14638 RVA: 0x000E7B53 File Offset: 0x000E5D53
			// (set) Token: 0x0600392F RID: 14639 RVA: 0x000E7B5B File Offset: 0x000E5D5B
			public IAgentVisual AgentVisualToFollow { get; private set; }

			// Token: 0x17000968 RID: 2408
			// (get) Token: 0x06003930 RID: 14640 RVA: 0x000E7B64 File Offset: 0x000E5D64
			// (set) Token: 0x06003931 RID: 14641 RVA: 0x000E7B6C File Offset: 0x000E5D6C
			public SpectatorCameraTypes CameraType { get; private set; }

			// Token: 0x06003932 RID: 14642 RVA: 0x000E7B75 File Offset: 0x000E5D75
			public SpectatorData(Agent agentToFollow, IAgentVisual agentVisualToFollow, SpectatorCameraTypes cameraType)
			{
				this.AgentToFollow = agentToFollow;
				this.CameraType = cameraType;
				this.AgentVisualToFollow = agentVisualToFollow;
			}
		}

		// Token: 0x02000508 RID: 1288
		public enum State
		{
			// Token: 0x04001B88 RID: 7048
			NewlyCreated,
			// Token: 0x04001B89 RID: 7049
			Initializing,
			// Token: 0x04001B8A RID: 7050
			Continuing,
			// Token: 0x04001B8B RID: 7051
			EndingNextFrame,
			// Token: 0x04001B8C RID: 7052
			Over
		}

		// Token: 0x02000509 RID: 1289
		private class DynamicEntityInfo
		{
			// Token: 0x04001B8D RID: 7053
			public GameEntity Entity;

			// Token: 0x04001B8E RID: 7054
			public Timer TimerToDisable;
		}

		// Token: 0x0200050A RID: 1290
		public enum BattleSizeQualifier
		{
			// Token: 0x04001B90 RID: 7056
			Small,
			// Token: 0x04001B91 RID: 7057
			Medium
		}

		// Token: 0x0200050B RID: 1291
		public enum MissionTeamAITypeEnum
		{
			// Token: 0x04001B93 RID: 7059
			NoTeamAI,
			// Token: 0x04001B94 RID: 7060
			FieldBattle,
			// Token: 0x04001B95 RID: 7061
			Siege,
			// Token: 0x04001B96 RID: 7062
			SallyOut
		}

		// Token: 0x0200050C RID: 1292
		public enum MissileCollisionReaction
		{
			// Token: 0x04001B98 RID: 7064
			Invalid = -1,
			// Token: 0x04001B99 RID: 7065
			Stick,
			// Token: 0x04001B9A RID: 7066
			PassThrough,
			// Token: 0x04001B9B RID: 7067
			BounceBack,
			// Token: 0x04001B9C RID: 7068
			BecomeInvisible,
			// Token: 0x04001B9D RID: 7069
			Count
		}

		// Token: 0x0200050D RID: 1293
		// (Invoke) Token: 0x06003935 RID: 14645
		public delegate void OnBeforeAgentRemovedDelegate(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow);

		// Token: 0x0200050E RID: 1294
		public sealed class TeamCollection : List<Team>
		{
			// Token: 0x1400009F RID: 159
			// (add) Token: 0x06003938 RID: 14648 RVA: 0x000E7B94 File Offset: 0x000E5D94
			// (remove) Token: 0x06003939 RID: 14649 RVA: 0x000E7BCC File Offset: 0x000E5DCC
			public event Action<Team, Team> OnPlayerTeamChanged;

			// Token: 0x17000969 RID: 2409
			// (get) Token: 0x0600393A RID: 14650 RVA: 0x000E7C01 File Offset: 0x000E5E01
			// (set) Token: 0x0600393B RID: 14651 RVA: 0x000E7C09 File Offset: 0x000E5E09
			public Team Attacker { get; private set; }

			// Token: 0x1700096A RID: 2410
			// (get) Token: 0x0600393C RID: 14652 RVA: 0x000E7C12 File Offset: 0x000E5E12
			// (set) Token: 0x0600393D RID: 14653 RVA: 0x000E7C1A File Offset: 0x000E5E1A
			public Team Defender { get; private set; }

			// Token: 0x1700096B RID: 2411
			// (get) Token: 0x0600393E RID: 14654 RVA: 0x000E7C23 File Offset: 0x000E5E23
			// (set) Token: 0x0600393F RID: 14655 RVA: 0x000E7C2B File Offset: 0x000E5E2B
			public Team AttackerAlly { get; private set; }

			// Token: 0x1700096C RID: 2412
			// (get) Token: 0x06003940 RID: 14656 RVA: 0x000E7C34 File Offset: 0x000E5E34
			// (set) Token: 0x06003941 RID: 14657 RVA: 0x000E7C3C File Offset: 0x000E5E3C
			public Team DefenderAlly { get; private set; }

			// Token: 0x1700096D RID: 2413
			// (get) Token: 0x06003942 RID: 14658 RVA: 0x000E7C45 File Offset: 0x000E5E45
			// (set) Token: 0x06003943 RID: 14659 RVA: 0x000E7C4D File Offset: 0x000E5E4D
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

			// Token: 0x1700096E RID: 2414
			// (get) Token: 0x06003944 RID: 14660 RVA: 0x000E7C6B File Offset: 0x000E5E6B
			// (set) Token: 0x06003945 RID: 14661 RVA: 0x000E7C73 File Offset: 0x000E5E73
			public Team PlayerEnemy { get; private set; }

			// Token: 0x1700096F RID: 2415
			// (get) Token: 0x06003946 RID: 14662 RVA: 0x000E7C7C File Offset: 0x000E5E7C
			// (set) Token: 0x06003947 RID: 14663 RVA: 0x000E7C84 File Offset: 0x000E5E84
			public Team PlayerAlly { get; private set; }

			// Token: 0x06003948 RID: 14664 RVA: 0x000E7C8D File Offset: 0x000E5E8D
			public TeamCollection(Mission mission)
				: base(new List<Team>())
			{
				this._mission = mission;
			}

			// Token: 0x06003949 RID: 14665 RVA: 0x000E7CA1 File Offset: 0x000E5EA1
			private MBTeam AddNative()
			{
				return new MBTeam(this._mission, MBAPI.IMBMission.AddTeam(this._mission.Pointer));
			}

			// Token: 0x0600394A RID: 14666 RVA: 0x000E7CC3 File Offset: 0x000E5EC3
			public new void Add(Team t)
			{
				MBDebug.ShowWarning("Pre-created Team can not be added to TeamCollection!");
			}

			// Token: 0x0600394B RID: 14667 RVA: 0x000E7CD0 File Offset: 0x000E5ED0
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

			// Token: 0x0600394C RID: 14668 RVA: 0x000E7E18 File Offset: 0x000E6018
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

			// Token: 0x0600394D RID: 14669 RVA: 0x000E7E5C File Offset: 0x000E605C
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

			// Token: 0x0600394E RID: 14670 RVA: 0x000E7E98 File Offset: 0x000E6098
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

			// Token: 0x0600394F RID: 14671 RVA: 0x000E7F04 File Offset: 0x000E6104
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

			// Token: 0x06003950 RID: 14672 RVA: 0x000E7F48 File Offset: 0x000E6148
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

			// Token: 0x06003951 RID: 14673 RVA: 0x000E7F90 File Offset: 0x000E6190
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

			// Token: 0x17000970 RID: 2416
			// (get) Token: 0x06003952 RID: 14674 RVA: 0x000E808F File Offset: 0x000E628F
			private int TeamCountNative
			{
				get
				{
					return MBAPI.IMBMission.GetNumberOfTeams(this._mission.Pointer);
				}
			}

			// Token: 0x04001B9F RID: 7071
			private Mission _mission;

			// Token: 0x04001BA4 RID: 7076
			private Team _playerTeam;
		}
	}
}
