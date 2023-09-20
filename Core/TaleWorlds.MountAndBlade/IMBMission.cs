using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000195 RID: 405
	[ScriptingInterfaceBase]
	internal interface IMBMission
	{
		// Token: 0x06001627 RID: 5671
		[EngineMethod("clear_resources", false)]
		void ClearResources(UIntPtr missionPointer);

		// Token: 0x06001628 RID: 5672
		[EngineMethod("create_mission", false)]
		UIntPtr CreateMission(Mission mission);

		// Token: 0x06001629 RID: 5673
		[EngineMethod("tick_agents_and_teams_async", false)]
		void tickAgentsAndTeamsAsync(UIntPtr missionPointer, float dt);

		// Token: 0x0600162A RID: 5674
		[EngineMethod("clear_agent_actions", false)]
		void ClearAgentActions(UIntPtr missionPointer);

		// Token: 0x0600162B RID: 5675
		[EngineMethod("clear_missiles", false)]
		void ClearMissiles(UIntPtr missionPointer);

		// Token: 0x0600162C RID: 5676
		[EngineMethod("clear_corpses", false)]
		void ClearCorpses(UIntPtr missionPointer, bool isMissionReset);

		// Token: 0x0600162D RID: 5677
		[EngineMethod("get_pause_ai_tick", false)]
		bool GetPauseAITick(UIntPtr missionPointer);

		// Token: 0x0600162E RID: 5678
		[EngineMethod("set_pause_ai_tick", false)]
		void SetPauseAITick(UIntPtr missionPointer, bool I);

		// Token: 0x0600162F RID: 5679
		[EngineMethod("get_clear_scene_timer_elapsed_time", false)]
		float GetClearSceneTimerElapsedTime(UIntPtr missionPointer);

		// Token: 0x06001630 RID: 5680
		[EngineMethod("reset_first_third_person_view", false)]
		void ResetFirstThirdPersonView(UIntPtr missionPointer);

		// Token: 0x06001631 RID: 5681
		[EngineMethod("set_camera_is_first_person", false)]
		void SetCameraIsFirstPerson(bool value);

		// Token: 0x06001632 RID: 5682
		[EngineMethod("set_camera_frame", false)]
		void SetCameraFrame(UIntPtr missionPointer, ref MatrixFrame cameraFrame, float zoomFactor, ref Vec3 attenuationPosition);

		// Token: 0x06001633 RID: 5683
		[EngineMethod("get_camera_frame", false)]
		MatrixFrame GetCameraFrame(UIntPtr missionPointer);

		// Token: 0x06001634 RID: 5684
		[EngineMethod("get_is_loading_finished", false)]
		bool GetIsLoadingFinished(UIntPtr missionPointer);

		// Token: 0x06001635 RID: 5685
		[EngineMethod("clear_scene", false)]
		void ClearScene(UIntPtr missionPointer);

		// Token: 0x06001636 RID: 5686
		[EngineMethod("initialize_mission", false)]
		void InitializeMission(UIntPtr missionPointer, ref MissionInitializerRecord rec);

		// Token: 0x06001637 RID: 5687
		[EngineMethod("finalize_mission", false)]
		void FinalizeMission(UIntPtr missionPointer);

		// Token: 0x06001638 RID: 5688
		[EngineMethod("get_time", false)]
		float GetTime(UIntPtr missionPointer);

		// Token: 0x06001639 RID: 5689
		[EngineMethod("get_average_fps", false)]
		float GetAverageFps(UIntPtr missionPointer);

		// Token: 0x0600163A RID: 5690
		[EngineMethod("get_combat_type", false)]
		int GetCombatType(UIntPtr missionPointer);

		// Token: 0x0600163B RID: 5691
		[EngineMethod("set_combat_type", false)]
		void SetCombatType(UIntPtr missionPointer, int combatType);

		// Token: 0x0600163C RID: 5692
		[EngineMethod("ray_cast_for_closest_agent", false)]
		Agent RayCastForClosestAgent(UIntPtr missionPointer, Vec3 SourcePoint, Vec3 RayFinishPoint, int ExcludeAgentIndex, ref float CollisionDistance, float RayThickness);

		// Token: 0x0600163D RID: 5693
		[EngineMethod("ray_cast_for_closest_agents_limbs", false)]
		bool RayCastForClosestAgentsLimbs(UIntPtr missionPointer, Vec3 SourcePoint, Vec3 RayFinishPoint, int ExcludeAgentIndex, ref float CollisionDistance, ref int AgentIndex, ref sbyte BoneIndex);

		// Token: 0x0600163E RID: 5694
		[EngineMethod("ray_cast_for_given_agents_limbs", false)]
		bool RayCastForGivenAgentsLimbs(UIntPtr missionPointer, Vec3 SourcePoint, Vec3 RayFinishPoint, int GivenAgentIndex, ref float CollisionDistance, ref sbyte BoneIndex);

		// Token: 0x0600163F RID: 5695
		[EngineMethod("get_number_of_teams", false)]
		int GetNumberOfTeams(UIntPtr missionPointer);

		// Token: 0x06001640 RID: 5696
		[EngineMethod("reset_teams", false)]
		void ResetTeams(UIntPtr missionPointer);

		// Token: 0x06001641 RID: 5697
		[EngineMethod("add_team", false)]
		int AddTeam(UIntPtr missionPointer);

		// Token: 0x06001642 RID: 5698
		[EngineMethod("restart_record", false)]
		void RestartRecord(UIntPtr missionPointer);

		// Token: 0x06001643 RID: 5699
		[EngineMethod("is_position_inside_boundaries", false)]
		bool IsPositionInsideBoundaries(UIntPtr missionPointer, Vec2 position);

		// Token: 0x06001644 RID: 5700
		[EngineMethod("get_alternate_position_for_navmeshless_or_out_of_bounds_position", false)]
		WorldPosition GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(UIntPtr ptr, ref Vec2 directionTowards, ref WorldPosition originalPosition, ref float positionPenalty);

		// Token: 0x06001645 RID: 5701
		[EngineMethod("add_missile", false)]
		int AddMissile(UIntPtr missionPointer, bool isPrediction, int shooterAgentIndex, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, int weaponStatsDataLength, float damageBonus, ref Vec3 position, ref Vec3 direction, ref Mat3 orientation, float baseSpeed, float speed, bool addRigidBody, UIntPtr entityPointer, int forcedMissileIndex, bool isPrimaryWeaponShot, out UIntPtr missileEntity);

		// Token: 0x06001646 RID: 5702
		[EngineMethod("add_missile_single_usage", false)]
		int AddMissileSingleUsage(UIntPtr missionPointer, bool isPrediction, int shooterAgentIndex, in WeaponData weaponData, in WeaponStatsData weaponStatsData, float damageBonus, ref Vec3 position, ref Vec3 direction, ref Mat3 orientation, float baseSpeed, float speed, bool addRigidBody, UIntPtr entityPointer, int forcedMissileIndex, bool isPrimaryWeaponShot, out UIntPtr missileEntity);

		// Token: 0x06001647 RID: 5703
		[EngineMethod("get_missile_collision_point", false)]
		Vec3 GetMissileCollisionPoint(UIntPtr missionPointer, Vec3 missileStartingPosition, Vec3 missileDirection, float missileStartingSpeed, in WeaponData weaponData);

		// Token: 0x06001648 RID: 5704
		[EngineMethod("remove_missile", false)]
		void RemoveMissile(UIntPtr missionPointer, int missileIndex);

		// Token: 0x06001649 RID: 5705
		[EngineMethod("get_missile_vertical_aim_correction", false)]
		float GetMissileVerticalAimCorrection(Vec3 vecToTarget, float missileStartingSpeed, ref WeaponStatsData weaponStatsData, float airFrictionConstant);

		// Token: 0x0600164A RID: 5706
		[EngineMethod("get_missile_range", false)]
		float GetMissileRange(float missileStartingSpeed, float heightDifference);

		// Token: 0x0600164B RID: 5707
		[EngineMethod("prepare_missile_weapon_for_drop", false)]
		void PrepareMissileWeaponForDrop(UIntPtr missionPointer, int missileIndex);

		// Token: 0x0600164C RID: 5708
		[EngineMethod("add_particle_system_burst_by_name", false)]
		void AddParticleSystemBurstByName(UIntPtr missionPointer, string particleSystem, ref MatrixFrame frame, bool synchThroughNetwork);

		// Token: 0x0600164D RID: 5709
		[EngineMethod("tick", false)]
		void Tick(UIntPtr missionPointer, float dt);

		// Token: 0x0600164E RID: 5710
		[EngineMethod("idle_tick", false)]
		void IdleTick(UIntPtr missionPointer, float dt);

		// Token: 0x0600164F RID: 5711
		[EngineMethod("make_sound", false)]
		void MakeSound(UIntPtr pointer, int nativeSoundCode, Vec3 position, bool soundCanBePredicted, bool isReliable, int relatedAgent1, int relatedAgent2);

		// Token: 0x06001650 RID: 5712
		[EngineMethod("make_sound_with_parameter", false)]
		void MakeSoundWithParameter(UIntPtr pointer, int nativeSoundCode, Vec3 position, bool soundCanBePredicted, bool isReliable, int relatedAgent1, int relatedAgent2, SoundEventParameter parameter);

		// Token: 0x06001651 RID: 5713
		[EngineMethod("make_sound_only_on_related_peer", false)]
		void MakeSoundOnlyOnRelatedPeer(UIntPtr pointer, int nativeSoundCode, Vec3 position, int relatedAgent);

		// Token: 0x06001652 RID: 5714
		[EngineMethod("add_sound_alarm_factor_to_agents", false)]
		void AddSoundAlarmFactorToAgents(UIntPtr pointer, int ownerId, Vec3 position, float alarmFactor);

		// Token: 0x06001653 RID: 5715
		[EngineMethod("get_enemy_alarm_state_indicator", false)]
		int GetEnemyAlarmStateIndicator(UIntPtr missionPointer);

		// Token: 0x06001654 RID: 5716
		[EngineMethod("get_player_alarm_indicator", false)]
		float GetPlayerAlarmIndicator(UIntPtr missionPointer);

		// Token: 0x06001655 RID: 5717
		[EngineMethod("create_agent", false)]
		Mission.AgentCreationResult CreateAgent(UIntPtr missionPointer, ulong monsterFlag, int forcedAgentIndex, bool isFemale, ref AgentSpawnData spawnData, ref CapsuleData bodyCapsule, ref CapsuleData crouchedBodyCapsule, ref AnimationSystemData animationSystemData, int instanceNo);

		// Token: 0x06001656 RID: 5718
		[EngineMethod("get_position_of_missile", false)]
		Vec3 GetPositionOfMissile(UIntPtr missionPointer, int index);

		// Token: 0x06001657 RID: 5719
		[EngineMethod("get_velocity_of_missile", false)]
		Vec3 GetVelocityOfMissile(UIntPtr missionPointer, int index);

		// Token: 0x06001658 RID: 5720
		[EngineMethod("get_missile_has_rigid_body", false)]
		bool GetMissileHasRigidBody(UIntPtr missionPointer, int index);

		// Token: 0x06001659 RID: 5721
		[EngineMethod("add_boundary", false)]
		bool AddBoundary(UIntPtr missionPointer, string name, Vec2[] boundaryPoints, int boundaryPointCount, bool isAllowanceInside);

		// Token: 0x0600165A RID: 5722
		[EngineMethod("remove_boundary", false)]
		bool RemoveBoundary(UIntPtr missionPointer, string name);

		// Token: 0x0600165B RID: 5723
		[EngineMethod("get_boundary_points", false)]
		void GetBoundaryPoints(UIntPtr missionPointer, string name, int boundaryPointOffset, Vec2[] boundaryPoints, int boundaryPointsSize, ref int retrievedPointCount);

		// Token: 0x0600165C RID: 5724
		[EngineMethod("get_boundary_count", false)]
		int GetBoundaryCount(UIntPtr missionPointer);

		// Token: 0x0600165D RID: 5725
		[EngineMethod("get_boundary_radius", false)]
		float GetBoundaryRadius(UIntPtr missionPointer, string name);

		// Token: 0x0600165E RID: 5726
		[EngineMethod("get_boundary_name", false)]
		string GetBoundaryName(UIntPtr missionPointer, int boundaryIndex);

		// Token: 0x0600165F RID: 5727
		[EngineMethod("get_closest_boundary_position", false)]
		Vec2 GetClosestBoundaryPosition(UIntPtr missionPointer, Vec2 position);

		// Token: 0x06001660 RID: 5728
		[EngineMethod("get_navigation_points", false)]
		bool GetNavigationPoints(UIntPtr missionPointer, ref NavigationData navigationData);

		// Token: 0x06001661 RID: 5729
		[EngineMethod("set_navigation_face_cost_with_id_around_position", false)]
		void SetNavigationFaceCostWithIdAroundPosition(UIntPtr missionPointer, int navigationFaceId, Vec3 position, float cost);

		// Token: 0x06001662 RID: 5730
		[EngineMethod("pause_mission_scene_sounds", false)]
		void PauseMissionSceneSounds(UIntPtr missionPointer);

		// Token: 0x06001663 RID: 5731
		[EngineMethod("resume_mission_scene_sounds", false)]
		void ResumeMissionSceneSounds(UIntPtr missionPointer);

		// Token: 0x06001664 RID: 5732
		[EngineMethod("process_record_until_time", false)]
		void ProcessRecordUntilTime(UIntPtr missionPointer, float time);

		// Token: 0x06001665 RID: 5733
		[EngineMethod("end_of_record", false)]
		bool EndOfRecord(UIntPtr missionPointer);

		// Token: 0x06001666 RID: 5734
		[EngineMethod("record_current_state", false)]
		void RecordCurrentState(UIntPtr missionPointer);

		// Token: 0x06001667 RID: 5735
		[EngineMethod("start_recording", false)]
		void StartRecording();

		// Token: 0x06001668 RID: 5736
		[EngineMethod("backup_record_to_file", false)]
		void BackupRecordToFile(UIntPtr missionPointer, string fileName, string gameType, string sceneLevels);

		// Token: 0x06001669 RID: 5737
		[EngineMethod("restore_record_from_file", false)]
		void RestoreRecordFromFile(UIntPtr missionPointer, string fileName);

		// Token: 0x0600166A RID: 5738
		[EngineMethod("clear_record_buffers", false)]
		void ClearRecordBuffers(UIntPtr missionPointer);

		// Token: 0x0600166B RID: 5739
		[EngineMethod("get_scene_name_for_replay", false)]
		string GetSceneNameForReplay(PlatformFilePath replayName);

		// Token: 0x0600166C RID: 5740
		[EngineMethod("get_game_type_for_replay", false)]
		string GetGameTypeForReplay(PlatformFilePath replayName);

		// Token: 0x0600166D RID: 5741
		[EngineMethod("get_scene_levels_for_replay", false)]
		string GetSceneLevelsForReplay(PlatformFilePath replayName);

		// Token: 0x0600166E RID: 5742
		[EngineMethod("get_atmosphere_name_for_replay", false)]
		string GetAtmosphereNameForReplay(PlatformFilePath replayName);

		// Token: 0x0600166F RID: 5743
		[EngineMethod("get_atmosphere_season_for_replay", false)]
		int GetAtmosphereSeasonForReplay(PlatformFilePath replayName);

		// Token: 0x06001670 RID: 5744
		[EngineMethod("get_closest_enemy", false)]
		Agent GetClosestEnemy(UIntPtr missionPointer, int teamIndex, Vec3 position, float radius);

		// Token: 0x06001671 RID: 5745
		[EngineMethod("get_closest_ally", false)]
		Agent GetClosestAlly(UIntPtr missionPointer, int teamIndex, Vec3 position, float radius);

		// Token: 0x06001672 RID: 5746
		[EngineMethod("is_agent_in_proximity_map", false)]
		bool IsAgentInProximityMap(UIntPtr missionPointer, int agentIndex);

		// Token: 0x06001673 RID: 5747
		[EngineMethod("has_any_agents_of_team_around", false)]
		bool HasAnyAgentsOfTeamAround(UIntPtr missionPointer, Vec3 origin, float radius, int teamNo);

		// Token: 0x06001674 RID: 5748
		[EngineMethod("get_agent_count_around_position", false)]
		void GetAgentCountAroundPosition(UIntPtr missionPointer, int teamIndex, Vec2 position, float radius, ref int allyCount, ref int enemyCount);

		// Token: 0x06001675 RID: 5749
		[EngineMethod("find_agent_with_index", false)]
		Agent FindAgentWithIndex(UIntPtr missionPointer, int index);

		// Token: 0x06001676 RID: 5750
		[EngineMethod("set_random_decide_time_of_agents", false)]
		void SetRandomDecideTimeOfAgents(UIntPtr missionPointer, int agentCount, int[] agentIndices, float minAIReactionTime, float maxAIReactionTime);

		// Token: 0x06001677 RID: 5751
		[EngineMethod("get_average_morale_of_agents", false)]
		float GetAverageMoraleOfAgents(UIntPtr missionPointer, int agentCount, int[] agentIndices);

		// Token: 0x06001678 RID: 5752
		[EngineMethod("get_best_slope_towards_direction", false)]
		WorldPosition GetBestSlopeTowardsDirection(UIntPtr missionPointer, ref WorldPosition centerPosition, float halfsize, ref WorldPosition referencePosition);

		// Token: 0x06001679 RID: 5753
		[EngineMethod("get_best_slope_angle_height_pos_for_defending", false)]
		WorldPosition GetBestSlopeAngleHeightPosForDefending(UIntPtr missionPointer, WorldPosition enemyPosition, WorldPosition defendingPosition, int sampleSize, float distanceRatioAllowedFromDefendedPos, float distanceSqrdAllowedFromBoundary, float cosinusOfBestSlope, float cosinusOfMaxAcceptedSlope, float minSlopeScore, float maxSlopeScore, float excessiveSlopePenalty, float nearConeCenterRatio, float nearConeCenterBonus, float heightDifferenceCeiling, float maxDisplacementPenalty);

		// Token: 0x0600167A RID: 5754
		[EngineMethod("get_nearby_agents_aux", false)]
		void GetNearbyAgentsAux(UIntPtr missionPointer, Vec2 center, float radius, int teamIndex, int friendOrEnemyOrAll, int agentsArrayOffset, ref EngineStackArray.StackArray40Int agentIds, ref int retrievedAgentCount);

		// Token: 0x0600167B RID: 5755
		[EngineMethod("get_weighted_point_of_enemies", false)]
		Vec2 GetWeightedPointOfEnemies(UIntPtr missionPointer, int agentIndex, Vec2 basePoint);

		// Token: 0x0600167C RID: 5756
		[EngineMethod("is_formation_unit_position_available", false)]
		bool IsFormationUnitPositionAvailable(UIntPtr missionPointer, ref WorldPosition orderPosition, ref WorldPosition unitPosition, ref WorldPosition nearestAvailableUnitPosition, float manhattanDistance);

		// Token: 0x0600167D RID: 5757
		[EngineMethod("get_straight_path_to_target", false)]
		WorldPosition GetStraightPathToTarget(UIntPtr scenePointer, Vec2 targetPosition, WorldPosition startingPosition, float samplingDistance, bool stopAtObstacle);

		// Token: 0x0600167E RID: 5758
		[EngineMethod("set_bow_missile_speed_modifier", false)]
		void SetBowMissileSpeedModifier(UIntPtr missionPointer, float modifier);

		// Token: 0x0600167F RID: 5759
		[EngineMethod("set_crossbow_missile_speed_modifier", false)]
		void SetCrossbowMissileSpeedModifier(UIntPtr missionPointer, float modifier);

		// Token: 0x06001680 RID: 5760
		[EngineMethod("set_throwing_missile_speed_modifier", false)]
		void SetThrowingMissileSpeedModifier(UIntPtr missionPointer, float modifier);

		// Token: 0x06001681 RID: 5761
		[EngineMethod("set_missile_range_modifier", false)]
		void SetMissileRangeModifier(UIntPtr missionPointer, float modifier);

		// Token: 0x06001682 RID: 5762
		[EngineMethod("set_last_movement_key_pressed", false)]
		void SetLastMovementKeyPressed(UIntPtr missionPointer, Agent.MovementControlFlag lastMovementKeyPressed);

		// Token: 0x06001683 RID: 5763
		[EngineMethod("fastforward_mission", false)]
		void FastForwardMission(UIntPtr missionPointer, float startTime, float endTime);

		// Token: 0x06001684 RID: 5764
		[EngineMethod("get_debug_agent", false)]
		int GetDebugAgent(UIntPtr missionPointer);

		// Token: 0x06001685 RID: 5765
		[EngineMethod("set_debug_agent", false)]
		void SetDebugAgent(UIntPtr missionPointer, int index);

		// Token: 0x06001686 RID: 5766
		[EngineMethod("add_ai_debug_text", false)]
		void AddAiDebugText(UIntPtr missionPointer, string text);

		// Token: 0x06001687 RID: 5767
		[EngineMethod("agent_proximity_map_begin_search", false)]
		AgentProximityMap.ProximityMapSearchStructInternal ProximityMapBeginSearch(UIntPtr missionPointer, Vec2 searchPos, float searchRadius);

		// Token: 0x06001688 RID: 5768
		[EngineMethod("agent_proximity_map_find_next", false)]
		void ProximityMapFindNext(UIntPtr missionPointer, ref AgentProximityMap.ProximityMapSearchStructInternal searchStruct);

		// Token: 0x06001689 RID: 5769
		[EngineMethod("agent_proximity_map_get_max_search_radius", false)]
		float ProximityMapMaxSearchRadius(UIntPtr missionPointer);

		// Token: 0x0600168A RID: 5770
		[EngineMethod("get_biggest_agent_collision_padding", false)]
		float GetBiggestAgentCollisionPadding(UIntPtr missionPointer);

		// Token: 0x0600168B RID: 5771
		[EngineMethod("set_mission_corpse_fade_out_time_in_seconds", false)]
		void SetMissionCorpseFadeOutTimeInSeconds(UIntPtr missionPointer, float corpseFadeOutTimeInSeconds);

		// Token: 0x0600168C RID: 5772
		[EngineMethod("set_report_stuck_agents_mode", false)]
		void SetReportStuckAgentsMode(UIntPtr missionPointer, bool value);

		// Token: 0x0600168D RID: 5773
		[EngineMethod("batch_formation_unit_positions", false)]
		void BatchFormationUnitPositions(UIntPtr missionPointer, Vec2i[] orderedPositionIndices, Vec2[] orderedLocalPositions, int[] availabilityTable, WorldPosition[] globalPositionTable, WorldPosition orderPosition, Vec2 direction, int fileCount, int rankCount);

		// Token: 0x0600168E RID: 5774
		[EngineMethod("toggle_disable_fall_avoid", false)]
		bool ToggleDisableFallAvoid();

		// Token: 0x0600168F RID: 5775
		[EngineMethod("get_water_level_at_position", false)]
		float GetWaterLevelAtPosition(UIntPtr missionPointer, Vec2 position);
	}
}
