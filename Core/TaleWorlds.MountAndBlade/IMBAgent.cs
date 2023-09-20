using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000190 RID: 400
	[ScriptingInterfaceBase]
	internal interface IMBAgent
	{
		// Token: 0x06001519 RID: 5401
		[EngineMethod("get_movement_flags", false)]
		uint GetMovementFlags(UIntPtr agentPointer);

		// Token: 0x0600151A RID: 5402
		[EngineMethod("set_movement_flags", false)]
		void SetMovementFlags(UIntPtr agentPointer, Agent.MovementControlFlag value);

		// Token: 0x0600151B RID: 5403
		[EngineMethod("get_movement_input_vector", false)]
		Vec2 GetMovementInputVector(UIntPtr agentPointer);

		// Token: 0x0600151C RID: 5404
		[EngineMethod("set_movement_input_vector", false)]
		void SetMovementInputVector(UIntPtr agentPointer, Vec2 value);

		// Token: 0x0600151D RID: 5405
		[EngineMethod("get_collision_capsule", false)]
		void GetCollisionCapsule(UIntPtr agentPointer, ref CapsuleData value);

		// Token: 0x0600151E RID: 5406
		[EngineMethod("set_attack_state", false)]
		void SetAttackState(UIntPtr agentPointer, int attackState);

		// Token: 0x0600151F RID: 5407
		[EngineMethod("get_agent_visuals", false)]
		MBAgentVisuals GetAgentVisuals(UIntPtr agentPointer);

		// Token: 0x06001520 RID: 5408
		[EngineMethod("get_event_control_flags", false)]
		uint GetEventControlFlags(UIntPtr agentPointer);

		// Token: 0x06001521 RID: 5409
		[EngineMethod("set_event_control_flags", false)]
		void SetEventControlFlags(UIntPtr agentPointer, Agent.EventControlFlag eventflag);

		// Token: 0x06001522 RID: 5410
		[EngineMethod("set_average_ping_in_milliseconds", false)]
		void SetAveragePingInMilliseconds(UIntPtr agentPointer, double averagePingInMilliseconds);

		// Token: 0x06001523 RID: 5411
		[EngineMethod("set_look_agent", false)]
		void SetLookAgent(UIntPtr agentPointer, UIntPtr lookAtAgentPointer);

		// Token: 0x06001524 RID: 5412
		[EngineMethod("get_look_agent", false)]
		Agent GetLookAgent(UIntPtr agentPointer);

		// Token: 0x06001525 RID: 5413
		[EngineMethod("get_target_agent", false)]
		Agent GetTargetAgent(UIntPtr agentPointer);

		// Token: 0x06001526 RID: 5414
		[EngineMethod("set_interaction_agent", false)]
		void SetInteractionAgent(UIntPtr agentPointer, UIntPtr interactionAgentPointer);

		// Token: 0x06001527 RID: 5415
		[EngineMethod("set_look_to_point_of_interest", false)]
		void SetLookToPointOfInterest(UIntPtr agentPointer, Vec3 point);

		// Token: 0x06001528 RID: 5416
		[EngineMethod("disable_look_to_point_of_interest", false)]
		void DisableLookToPointOfInterest(UIntPtr agentPointer);

		// Token: 0x06001529 RID: 5417
		[EngineMethod("is_enemy", false)]
		bool IsEnemy(UIntPtr agentPointer1, UIntPtr agentPointer2);

		// Token: 0x0600152A RID: 5418
		[EngineMethod("is_friend", false)]
		bool IsFriend(UIntPtr agentPointer1, UIntPtr agentPointer2);

		// Token: 0x0600152B RID: 5419
		[EngineMethod("get_agent_flags", false)]
		uint GetAgentFlags(UIntPtr agentPointer);

		// Token: 0x0600152C RID: 5420
		[EngineMethod("set_agent_flags", false)]
		void SetAgentFlags(UIntPtr agentPointer, uint agentFlags);

		// Token: 0x0600152D RID: 5421
		[EngineMethod("set_selected_mount_index", false)]
		void SetSelectedMountIndex(UIntPtr agentPointer, int mount_index);

		// Token: 0x0600152E RID: 5422
		[EngineMethod("get_selected_mount_index", false)]
		int GetSelectedMountIndex(UIntPtr agentPointer);

		// Token: 0x0600152F RID: 5423
		[EngineMethod("get_riding_order", false)]
		int GetRidingOrder(UIntPtr agentPointer);

		// Token: 0x06001530 RID: 5424
		[EngineMethod("get_stepped_entity_id", false)]
		UIntPtr GetSteppedEntityId(UIntPtr agentPointer);

		// Token: 0x06001531 RID: 5425
		[EngineMethod("set_network_peer", false)]
		void SetNetworkPeer(UIntPtr agentPointer, int networkPeerIndex);

		// Token: 0x06001532 RID: 5426
		[EngineMethod("die", false)]
		void Die(UIntPtr agentPointer, ref Blow b, sbyte overrideKillInfo);

		// Token: 0x06001533 RID: 5427
		[EngineMethod("make_dead", false)]
		void MakeDead(UIntPtr agentPointer, bool isKilled, int actionIndex);

		// Token: 0x06001534 RID: 5428
		[EngineMethod("set_formation_frame_disabled", false)]
		void SetFormationFrameDisabled(UIntPtr agentPointer);

		// Token: 0x06001535 RID: 5429
		[EngineMethod("set_formation_frame_enabled", false)]
		bool SetFormationFrameEnabled(UIntPtr agentPointer, WorldPosition position, Vec2 direction, float formationDirectionEnforcingFactor);

		// Token: 0x06001536 RID: 5430
		[EngineMethod("set_should_catch_up_with_formation", false)]
		void SetShouldCatchUpWithFormation(UIntPtr agentPointer, bool value);

		// Token: 0x06001537 RID: 5431
		[EngineMethod("set_formation_integrity_data", false)]
		void SetFormationIntegrityData(UIntPtr agentPointer, Vec2 position, Vec2 currentFormationDirection, Vec2 averageVelocityOfCloseAgents, float averageMaxUnlimitedSpeedOfCloseAgents, float deviationOfPositions);

		// Token: 0x06001538 RID: 5432
		[EngineMethod("set_formation_info", false)]
		void SetFormationInfo(UIntPtr agentPointer, int fileIndex, int rankIndex, int fileCount, int rankCount, Vec2 wallDir, int unitSpacing);

		// Token: 0x06001539 RID: 5433
		[EngineMethod("set_retreat_mode", false)]
		void SetRetreatMode(UIntPtr agentPointer, WorldPosition retreatPos, bool retreat);

		// Token: 0x0600153A RID: 5434
		[EngineMethod("is_retreating", false)]
		bool IsRetreating(UIntPtr agentPointer);

		// Token: 0x0600153B RID: 5435
		[EngineMethod("is_fading_out", false)]
		bool IsFadingOut(UIntPtr agentPointer);

		// Token: 0x0600153C RID: 5436
		[EngineMethod("start_fading_out", false)]
		void StartFadingOut(UIntPtr agentPointer);

		// Token: 0x0600153D RID: 5437
		[EngineMethod("set_render_check_enabled", false)]
		void SetRenderCheckEnabled(UIntPtr agentPointer, bool value);

		// Token: 0x0600153E RID: 5438
		[EngineMethod("get_render_check_enabled", false)]
		bool GetRenderCheckEnabled(UIntPtr agentPointer);

		// Token: 0x0600153F RID: 5439
		[EngineMethod("get_retreat_pos", false)]
		WorldPosition GetRetreatPos(UIntPtr agentPointer);

		// Token: 0x06001540 RID: 5440
		[EngineMethod("get_team", false)]
		int GetTeam(UIntPtr agentPointer);

		// Token: 0x06001541 RID: 5441
		[EngineMethod("set_team", false)]
		void SetTeam(UIntPtr agentPointer, int teamIndex);

		// Token: 0x06001542 RID: 5442
		[EngineMethod("set_courage", false)]
		void SetCourage(UIntPtr agentPointer, float courage);

		// Token: 0x06001543 RID: 5443
		[EngineMethod("update_driven_properties", false)]
		void UpdateDrivenProperties(UIntPtr agentPointer, float[] values);

		// Token: 0x06001544 RID: 5444
		[EngineMethod("get_look_direction", false)]
		Vec3 GetLookDirection(UIntPtr agentPointer);

		// Token: 0x06001545 RID: 5445
		[EngineMethod("set_look_direction", false)]
		void SetLookDirection(UIntPtr agentPointer, Vec3 lookDirection);

		// Token: 0x06001546 RID: 5446
		[EngineMethod("get_look_down_limit", false)]
		float GetLookDownLimit(UIntPtr agentPointer);

		// Token: 0x06001547 RID: 5447
		[EngineMethod("get_position", false)]
		Vec3 GetPosition(UIntPtr agentPointer);

		// Token: 0x06001548 RID: 5448
		[EngineMethod("set_position", false)]
		void SetPosition(UIntPtr agentPointer, ref Vec3 position);

		// Token: 0x06001549 RID: 5449
		[EngineMethod("get_rotation_frame", false)]
		void GetRotationFrame(UIntPtr agentPointer, ref MatrixFrame outFrame);

		// Token: 0x0600154A RID: 5450
		[EngineMethod("get_eye_global_height", false)]
		float GetEyeGlobalHeight(UIntPtr agentPointer);

		// Token: 0x0600154B RID: 5451
		[EngineMethod("get_movement_velocity", false)]
		Vec2 GetMovementVelocity(UIntPtr agentPointer);

		// Token: 0x0600154C RID: 5452
		[EngineMethod("get_average_velocity", false)]
		Vec3 GetAverageVelocity(UIntPtr agentPointer);

		// Token: 0x0600154D RID: 5453
		[EngineMethod("get_is_left_stance", false)]
		bool GetIsLeftStance(UIntPtr agentPointer);

		// Token: 0x0600154E RID: 5454
		[EngineMethod("invalidate_target_agent", false)]
		void InvalidateTargetAgent(UIntPtr agentPointer);

		// Token: 0x0600154F RID: 5455
		[EngineMethod("invalidate_ai_weapon_selections", false)]
		void InvalidateAIWeaponSelections(UIntPtr agentPointer);

		// Token: 0x06001550 RID: 5456
		[EngineMethod("reset_enemy_caches", false)]
		void ResetEnemyCaches(UIntPtr agentPointer);

		// Token: 0x06001551 RID: 5457
		[EngineMethod("get_ai_state_flags", false)]
		Agent.AIStateFlag GetAIStateFlags(UIntPtr agentPointer);

		// Token: 0x06001552 RID: 5458
		[EngineMethod("set_ai_state_flags", false)]
		void SetAIStateFlags(UIntPtr agentPointer, Agent.AIStateFlag aiStateFlags);

		// Token: 0x06001553 RID: 5459
		[EngineMethod("get_state_flags", false)]
		AgentState GetStateFlags(UIntPtr agentPointer);

		// Token: 0x06001554 RID: 5460
		[EngineMethod("set_state_flags", false)]
		void SetStateFlags(UIntPtr agentPointer, AgentState StateFlags);

		// Token: 0x06001555 RID: 5461
		[EngineMethod("get_mount_agent", false)]
		Agent GetMountAgent(UIntPtr agentPointer);

		// Token: 0x06001556 RID: 5462
		[EngineMethod("set_mount_agent", false)]
		void SetMountAgent(UIntPtr agentPointer, int mountAgentIndex);

		// Token: 0x06001557 RID: 5463
		[EngineMethod("set_always_attack_in_melee", false)]
		void SetAlwaysAttackInMelee(UIntPtr agentPointer, bool attack);

		// Token: 0x06001558 RID: 5464
		[EngineMethod("get_rider_agent", false)]
		Agent GetRiderAgent(UIntPtr agentPointer);

		// Token: 0x06001559 RID: 5465
		[EngineMethod("set_controller", false)]
		void SetController(UIntPtr agentPointer, Agent.ControllerType controller);

		// Token: 0x0600155A RID: 5466
		[EngineMethod("get_controller", false)]
		Agent.ControllerType GetController(UIntPtr agentPointer);

		// Token: 0x0600155B RID: 5467
		[EngineMethod("set_initial_frame", false)]
		void SetInitialFrame(UIntPtr agentPointer, in Vec3 initialPosition, in Vec2 initialDirection, bool canSpawnOutsideOfMissionBoundary);

		// Token: 0x0600155C RID: 5468
		[EngineMethod("weapon_equipped", false)]
		void WeaponEquipped(UIntPtr agentPointer, int equipmentSlot, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, int weaponStatsDataLength, in WeaponData ammoWeaponData, WeaponStatsData[] ammoWeaponStatsData, int ammoWeaponStatsDataLength, UIntPtr weaponEntity, bool removeOldWeaponFromScene, bool isWieldedOnSpawn);

		// Token: 0x0600155D RID: 5469
		[EngineMethod("drop_item", false)]
		void DropItem(UIntPtr agentPointer, int itemIndex, int pickedUpItemType);

		// Token: 0x0600155E RID: 5470
		[EngineMethod("set_weapon_amount_in_slot", false)]
		void SetWeaponAmountInSlot(UIntPtr agentPointer, int equipmentSlot, short amount, bool enforcePrimaryItem);

		// Token: 0x0600155F RID: 5471
		[EngineMethod("clear_equipment", false)]
		void ClearEquipment(UIntPtr agentPointer);

		// Token: 0x06001560 RID: 5472
		[EngineMethod("get_wielded_item_index", false)]
		EquipmentIndex GetWieldedItemIndex(UIntPtr agentPointer, int handIndex);

		// Token: 0x06001561 RID: 5473
		[EngineMethod("set_wielded_item_index_as_client", false)]
		void SetWieldedItemIndexAsClient(UIntPtr agentPointer, int handIndex, int wieldedItemIndex, bool isWieldedInstantly, bool isWieldedOnSpawn, int mainHandCurrentUsageIndex);

		// Token: 0x06001562 RID: 5474
		[EngineMethod("set_usage_index_of_weapon_in_slot_as_client", false)]
		void SetUsageIndexOfWeaponInSlotAsClient(UIntPtr agentPointer, int slotIndex, int usageIndex);

		// Token: 0x06001563 RID: 5475
		[EngineMethod("set_weapon_hit_points_in_slot", false)]
		void SetWeaponHitPointsInSlot(UIntPtr agentPointer, int wieldedItemIndex, short hitPoints);

		// Token: 0x06001564 RID: 5476
		[EngineMethod("set_weapon_ammo_as_client", false)]
		void SetWeaponAmmoAsClient(UIntPtr agentPointer, int equipmentIndex, int ammoEquipmentIndex, short ammo);

		// Token: 0x06001565 RID: 5477
		[EngineMethod("set_weapon_reload_phase_as_client", false)]
		void SetWeaponReloadPhaseAsClient(UIntPtr agentPointer, int wieldedItemIndex, short reloadPhase);

		// Token: 0x06001566 RID: 5478
		[EngineMethod("set_reload_ammo_in_slot", false)]
		void SetReloadAmmoInSlot(UIntPtr agentPointer, int slotIndex, int ammoSlotIndex, short reloadedAmmo);

		// Token: 0x06001567 RID: 5479
		[EngineMethod("start_switching_weapon_usage_index_as_client", false)]
		void StartSwitchingWeaponUsageIndexAsClient(UIntPtr agentPointer, int wieldedItemIndex, int usageIndex, Agent.UsageDirection currentMovementFlagUsageDirection);

		// Token: 0x06001568 RID: 5480
		[EngineMethod("try_to_wield_weapon_in_slot", false)]
		void TryToWieldWeaponInSlot(UIntPtr agentPointer, int equipmentSlot, int type, bool isWieldedOnSpawn);

		// Token: 0x06001569 RID: 5481
		[EngineMethod("get_weapon_entity_from_equipment_slot", false)]
		UIntPtr GetWeaponEntityFromEquipmentSlot(UIntPtr agentPointer, int equipmentSlot);

		// Token: 0x0600156A RID: 5482
		[EngineMethod("prepare_weapon_for_drop_in_equipment_slot", false)]
		void PrepareWeaponForDropInEquipmentSlot(UIntPtr agentPointer, int equipmentSlot, bool dropWithHolster);

		// Token: 0x0600156B RID: 5483
		[EngineMethod("try_to_sheath_weapon_in_hand", false)]
		void TryToSheathWeaponInHand(UIntPtr agentPointer, int handIndex, int type);

		// Token: 0x0600156C RID: 5484
		[EngineMethod("update_weapons", false)]
		void UpdateWeapons(UIntPtr agentPointer);

		// Token: 0x0600156D RID: 5485
		[EngineMethod("attach_weapon_to_bone", false)]
		void AttachWeaponToBone(UIntPtr agentPointer, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, int weaponStatsDataLength, UIntPtr weaponEntity, sbyte boneIndex, ref MatrixFrame attachLocalFrame);

		// Token: 0x0600156E RID: 5486
		[EngineMethod("delete_attached_weapon_from_bone", false)]
		void DeleteAttachedWeaponFromBone(UIntPtr agentPointer, int attachedWeaponIndex);

		// Token: 0x0600156F RID: 5487
		[EngineMethod("attach_weapon_to_weapon_in_slot", false)]
		void AttachWeaponToWeaponInSlot(UIntPtr agentPointer, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, int weaponStatsDataLength, UIntPtr weaponEntity, int slotIndex, ref MatrixFrame attachLocalFrame);

		// Token: 0x06001570 RID: 5488
		[EngineMethod("build", false)]
		void Build(UIntPtr agentPointer, Vec3 eyeOffsetWrtHead);

		// Token: 0x06001571 RID: 5489
		[EngineMethod("lock_agent_replication_table_with_current_reliable_sequence_no", false)]
		void LockAgentReplicationTableDataWithCurrentReliableSequenceNo(UIntPtr agentPointer, int peerIndex);

		// Token: 0x06001572 RID: 5490
		[EngineMethod("set_agent_exclude_state_for_face_group_id", false)]
		void SetAgentExcludeStateForFaceGroupId(UIntPtr agentPointer, int faceGroupId, bool isExcluded);

		// Token: 0x06001573 RID: 5491
		[EngineMethod("set_agent_scale", false)]
		void SetAgentScale(UIntPtr agentPointer, float scale);

		// Token: 0x06001574 RID: 5492
		[EngineMethod("initialize_agent_record", false)]
		void InitializeAgentRecord(UIntPtr agentPointer);

		// Token: 0x06001575 RID: 5493
		[EngineMethod("get_current_velocity", false)]
		Vec2 GetCurrentVelocity(UIntPtr agentPointer);

		// Token: 0x06001576 RID: 5494
		[EngineMethod("get_turn_speed", false)]
		float GetTurnSpeed(UIntPtr agentPointer);

		// Token: 0x06001577 RID: 5495
		[EngineMethod("get_movement_direction_as_angle", false)]
		float GetMovementDirectionAsAngle(UIntPtr agentPointer);

		// Token: 0x06001578 RID: 5496
		[EngineMethod("get_movement_direction", false)]
		Vec2 GetMovementDirection(UIntPtr agentPointer);

		// Token: 0x06001579 RID: 5497
		[EngineMethod("set_movement_direction", false)]
		void SetMovementDirection(UIntPtr agentPointer, in Vec2 direction);

		// Token: 0x0600157A RID: 5498
		[EngineMethod("get_current_speed_limit", false)]
		float GetCurrentSpeedLimit(UIntPtr agentPointer);

		// Token: 0x0600157B RID: 5499
		[EngineMethod("set_minimum_speed", false)]
		void SetMinimumSpeed(UIntPtr agentPointer, float speed);

		// Token: 0x0600157C RID: 5500
		[EngineMethod("set_maximum_speed_limit", false)]
		void SetMaximumSpeedLimit(UIntPtr agentPointer, float maximumSpeedLimit, bool isMultiplier);

		// Token: 0x0600157D RID: 5501
		[EngineMethod("get_maximum_speed_limit", false)]
		float GetMaximumSpeedLimit(UIntPtr agentPointer);

		// Token: 0x0600157E RID: 5502
		[EngineMethod("get_maximum_forward_unlimited_speed", false)]
		float GetMaximumForwardUnlimitedSpeed(UIntPtr agentPointer);

		// Token: 0x0600157F RID: 5503
		[EngineMethod("fade_out", false)]
		void FadeOut(UIntPtr agentPointer, bool hideInstantly);

		// Token: 0x06001580 RID: 5504
		[EngineMethod("fade_in", false)]
		void FadeIn(UIntPtr agentPointer);

		// Token: 0x06001581 RID: 5505
		[EngineMethod("get_scripted_flags", false)]
		int GetScriptedFlags(UIntPtr agentPointer);

		// Token: 0x06001582 RID: 5506
		[EngineMethod("set_scripted_flags", false)]
		void SetScriptedFlags(UIntPtr agentPointer, int flags);

		// Token: 0x06001583 RID: 5507
		[EngineMethod("get_debug_values", false)]
		void GetDebugValues(UIntPtr agentPointer, float[] values, ref int valueCount);

		// Token: 0x06001584 RID: 5508
		[EngineMethod("get_scripted_combat_flags", false)]
		int GetScriptedCombatFlags(UIntPtr agentPointer);

		// Token: 0x06001585 RID: 5509
		[EngineMethod("set_scripted_combat_flags", false)]
		void SetScriptedCombatFlags(UIntPtr agentPointer, int flags);

		// Token: 0x06001586 RID: 5510
		[EngineMethod("set_scripted_position_and_direction", false)]
		bool SetScriptedPositionAndDirection(UIntPtr agentPointer, ref WorldPosition targetPosition, float targetDirection, bool addHumanLikeDelay, int additionalFlags);

		// Token: 0x06001587 RID: 5511
		[EngineMethod("set_scripted_position", false)]
		bool SetScriptedPosition(UIntPtr agentPointer, ref WorldPosition targetPosition, bool addHumanLikeDelay, int additionalFlags);

		// Token: 0x06001588 RID: 5512
		[EngineMethod("set_scripted_target_entity", false)]
		void SetScriptedTargetEntity(UIntPtr agentPointer, UIntPtr entityId, ref WorldPosition specialPosition, int additionalFlags, bool ignoreIfAlreadyAttacking);

		// Token: 0x06001589 RID: 5513
		[EngineMethod("disable_scripted_movement", false)]
		void DisableScriptedMovement(UIntPtr agentPointer);

		// Token: 0x0600158A RID: 5514
		[EngineMethod("disable_scripted_combat_movement", false)]
		void DisableScriptedCombatMovement(UIntPtr agentPointer);

		// Token: 0x0600158B RID: 5515
		[EngineMethod("force_ai_behavior_selection", false)]
		void ForceAiBehaviorSelection(UIntPtr agentPointer);

		// Token: 0x0600158C RID: 5516
		[EngineMethod("has_path_through_navigation_face_id_from_direction", false)]
		bool HasPathThroughNavigationFaceIdFromDirection(UIntPtr agentPointer, int navigationFaceId, ref Vec2 direction);

		// Token: 0x0600158D RID: 5517
		[EngineMethod("has_path_through_navigation_faces_id_from_direction", false)]
		bool HasPathThroughNavigationFacesIDFromDirection(UIntPtr agentPointer, int navigationFaceID_1, int navigationFaceID_2, int navigationFaceID_3, ref Vec2 direction);

		// Token: 0x0600158E RID: 5518
		[EngineMethod("can_move_directly_to_position", false)]
		bool CanMoveDirectlyToPosition(UIntPtr agentPointer, in WorldPosition position);

		// Token: 0x0600158F RID: 5519
		[EngineMethod("check_path_to_ai_target_agent_passes_through_navigation_face_id_from_direction", false)]
		bool CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(UIntPtr agentPointer, int navigationFaceId, ref Vec3 direction, float overridenCostForFaceId);

		// Token: 0x06001590 RID: 5520
		[EngineMethod("get_path_distance_to_point", false)]
		float GetPathDistanceToPoint(UIntPtr agentPointer, ref Vec3 direction);

		// Token: 0x06001591 RID: 5521
		[EngineMethod("get_current_navigation_face_id", false)]
		int GetCurrentNavigationFaceId(UIntPtr agentPointer);

		// Token: 0x06001592 RID: 5522
		[EngineMethod("get_world_position", false)]
		WorldPosition GetWorldPosition(UIntPtr agentPointer);

		// Token: 0x06001593 RID: 5523
		[EngineMethod("set_agent_facial_animation", false)]
		void SetAgentFacialAnimation(UIntPtr agentPointer, int channel, string animationName, bool loop);

		// Token: 0x06001594 RID: 5524
		[EngineMethod("get_agent_facial_animation", false)]
		string GetAgentFacialAnimation(UIntPtr agentPointer);

		// Token: 0x06001595 RID: 5525
		[EngineMethod("get_agent_voice_definiton", false)]
		string GetAgentVoiceDefinition(UIntPtr agentPointer);

		// Token: 0x06001596 RID: 5526
		[EngineMethod("get_current_animation_flags", false)]
		ulong GetCurrentAnimationFlags(UIntPtr agentPointer, int channelNo);

		// Token: 0x06001597 RID: 5527
		[EngineMethod("get_current_action", false)]
		int GetCurrentAction(UIntPtr agentPointer, int channelNo);

		// Token: 0x06001598 RID: 5528
		[EngineMethod("get_current_action_type", false)]
		int GetCurrentActionType(UIntPtr agentPointer, int channelNo);

		// Token: 0x06001599 RID: 5529
		[EngineMethod("get_current_action_stage", false)]
		int GetCurrentActionStage(UIntPtr agentPointer, int channelNo);

		// Token: 0x0600159A RID: 5530
		[EngineMethod("get_current_action_direction", false)]
		int GetCurrentActionDirection(UIntPtr agentPointer, int channelNo);

		// Token: 0x0600159B RID: 5531
		[EngineMethod("compute_animation_displacement", false)]
		Vec3 ComputeAnimationDisplacement(UIntPtr agentPointer, float dt);

		// Token: 0x0600159C RID: 5532
		[EngineMethod("get_current_action_priority", false)]
		int GetCurrentActionPriority(UIntPtr agentPointer, int channelNo);

		// Token: 0x0600159D RID: 5533
		[EngineMethod("get_current_action_progress", false)]
		float GetCurrentActionProgress(UIntPtr agentPointer, int channelNo);

		// Token: 0x0600159E RID: 5534
		[EngineMethod("set_current_action_progress", false)]
		void SetCurrentActionProgress(UIntPtr agentPointer, int channelNo, float progress);

		// Token: 0x0600159F RID: 5535
		[EngineMethod("set_action_channel", false)]
		bool SetActionChannel(UIntPtr agentPointer, int channelNo, int actionNo, ulong additionalFlags, bool ignorePriority, float blendWithNextActionFactor, float actionSpeed, float blendInPeriod, float blendOutPeriodToNoAnim, float startProgress, bool useLinearSmoothing, float blendOutPeriod, bool forceFaceMorphRestart);

		// Token: 0x060015A0 RID: 5536
		[EngineMethod("set_current_action_speed", false)]
		void SetCurrentActionSpeed(UIntPtr agentPointer, int channelNo, float actionSpeed);

		// Token: 0x060015A1 RID: 5537
		[EngineMethod("tick_action_channels", false)]
		void TickActionChannels(UIntPtr agentPointer, float dt);

		// Token: 0x060015A2 RID: 5538
		[EngineMethod("get_action_channel_weight", false)]
		float GetActionChannelWeight(UIntPtr agentPointer, int channelNo);

		// Token: 0x060015A3 RID: 5539
		[EngineMethod("get_action_channel_current_action_weight", false)]
		float GetActionChannelCurrentActionWeight(UIntPtr agentPointer, int channelNo);

		// Token: 0x060015A4 RID: 5540
		[EngineMethod("set_action_set", false)]
		void SetActionSet(UIntPtr agentPointer, ref AnimationSystemData animationSystemData);

		// Token: 0x060015A5 RID: 5541
		[EngineMethod("get_action_set_no", false)]
		int GetActionSetNo(UIntPtr agentPointer);

		// Token: 0x060015A6 RID: 5542
		[EngineMethod("get_movement_locked_state", false)]
		AgentMovementLockedState GetMovementLockedState(UIntPtr agentPointer);

		// Token: 0x060015A7 RID: 5543
		[EngineMethod("get_aiming_timer", false)]
		float GetAimingTimer(UIntPtr agentPointer);

		// Token: 0x060015A8 RID: 5544
		[EngineMethod("get_target_position", false)]
		Vec2 GetTargetPosition(UIntPtr agentPointer);

		// Token: 0x060015A9 RID: 5545
		[EngineMethod("set_target_position", false)]
		void SetTargetPosition(UIntPtr agentPointer, ref Vec2 targetPosition);

		// Token: 0x060015AA RID: 5546
		[EngineMethod("get_target_direction", false)]
		Vec3 GetTargetDirection(UIntPtr agentPointer);

		// Token: 0x060015AB RID: 5547
		[EngineMethod("set_target_position_and_direction", false)]
		void SetTargetPositionAndDirection(UIntPtr agentPointer, ref Vec2 targetPosition, ref Vec3 targetDirection);

		// Token: 0x060015AC RID: 5548
		[EngineMethod("clear_target_frame", false)]
		void ClearTargetFrame(UIntPtr agentPointer);

		// Token: 0x060015AD RID: 5549
		[EngineMethod("get_is_look_direction_locked", false)]
		bool GetIsLookDirectionLocked(UIntPtr agentPointer);

		// Token: 0x060015AE RID: 5550
		[EngineMethod("set_is_look_direction_locked", false)]
		void SetIsLookDirectionLocked(UIntPtr agentPointer, bool isLocked);

		// Token: 0x060015AF RID: 5551
		[EngineMethod("set_mono_object", false)]
		void SetMonoObject(UIntPtr agentPointer, Agent monoObject);

		// Token: 0x060015B0 RID: 5552
		[EngineMethod("get_eye_global_position", false)]
		Vec3 GetEyeGlobalPosition(UIntPtr agentPointer);

		// Token: 0x060015B1 RID: 5553
		[EngineMethod("get_chest_global_position", false)]
		Vec3 GetChestGlobalPosition(UIntPtr agentPointer);

		// Token: 0x060015B2 RID: 5554
		[EngineMethod("add_mesh_to_bone", false)]
		void AddMeshToBone(UIntPtr agentPointer, UIntPtr meshPointer, sbyte boneIndex);

		// Token: 0x060015B3 RID: 5555
		[EngineMethod("remove_mesh_from_bone", false)]
		void RemoveMeshFromBone(UIntPtr agentPointer, UIntPtr meshPointer, sbyte boneIndex);

		// Token: 0x060015B4 RID: 5556
		[EngineMethod("add_prefab_to_agent_bone", false)]
		CompositeComponent AddPrefabToAgentBone(UIntPtr agentPointer, string prefabName, sbyte boneIndex);

		// Token: 0x060015B5 RID: 5557
		[EngineMethod("wield_next_weapon", false)]
		void WieldNextWeapon(UIntPtr agentPointer, int handIndex, int wieldActionType);

		// Token: 0x060015B6 RID: 5558
		[EngineMethod("preload_for_rendering", false)]
		void PreloadForRendering(UIntPtr agentPointer);

		// Token: 0x060015B7 RID: 5559
		[EngineMethod("get_agent_scale", false)]
		float GetAgentScale(UIntPtr agentPointer);

		// Token: 0x060015B8 RID: 5560
		[EngineMethod("get_crouch_mode", false)]
		bool GetCrouchMode(UIntPtr agentPointer);

		// Token: 0x060015B9 RID: 5561
		[EngineMethod("get_walk_mode", false)]
		bool GetWalkMode(UIntPtr agentPointer);

		// Token: 0x060015BA RID: 5562
		[EngineMethod("get_visual_position", false)]
		Vec3 GetVisualPosition(UIntPtr agentPointer);

		// Token: 0x060015BB RID: 5563
		[EngineMethod("is_look_rotation_in_slow_motion", false)]
		bool IsLookRotationInSlowMotion(UIntPtr agentPointer);

		// Token: 0x060015BC RID: 5564
		[EngineMethod("get_look_direction_as_angle", false)]
		float GetLookDirectionAsAngle(UIntPtr agentPointer);

		// Token: 0x060015BD RID: 5565
		[EngineMethod("set_look_direction_as_angle", false)]
		void SetLookDirectionAsAngle(UIntPtr agentPointer, float value);

		// Token: 0x060015BE RID: 5566
		[EngineMethod("attack_direction_to_movement_flag", false)]
		Agent.MovementControlFlag AttackDirectionToMovementFlag(UIntPtr agentPointer, Agent.UsageDirection direction);

		// Token: 0x060015BF RID: 5567
		[EngineMethod("defend_direction_to_movement_flag", false)]
		Agent.MovementControlFlag DefendDirectionToMovementFlag(UIntPtr agentPointer, Agent.UsageDirection direction);

		// Token: 0x060015C0 RID: 5568
		[EngineMethod("get_head_camera_mode", false)]
		bool GetHeadCameraMode(UIntPtr agentPointer);

		// Token: 0x060015C1 RID: 5569
		[EngineMethod("set_head_camera_mode", false)]
		void SetHeadCameraMode(UIntPtr agentPointer, bool value);

		// Token: 0x060015C2 RID: 5570
		[EngineMethod("kick_clear", false)]
		bool KickClear(UIntPtr agentPointer);

		// Token: 0x060015C3 RID: 5571
		[EngineMethod("reset_guard", false)]
		void ResetGuard(UIntPtr agentPointer);

		// Token: 0x060015C4 RID: 5572
		[EngineMethod("get_current_guard_mode", false)]
		Agent.GuardMode GetCurrentGuardMode(UIntPtr agentPointer);

		// Token: 0x060015C5 RID: 5573
		[EngineMethod("get_defend_movement_flag", false)]
		Agent.MovementControlFlag GetDefendMovementFlag(UIntPtr agentPointer);

		// Token: 0x060015C6 RID: 5574
		[EngineMethod("get_attack_direction", false)]
		Agent.UsageDirection GetAttackDirection(UIntPtr agentPointer, bool doAiCheck);

		// Token: 0x060015C7 RID: 5575
		[EngineMethod("player_attack_direction", false)]
		Agent.UsageDirection PlayerAttackDirection(UIntPtr agentPointer);

		// Token: 0x060015C8 RID: 5576
		[EngineMethod("get_wielded_weapon_info", false)]
		bool GetWieldedWeaponInfo(UIntPtr agentPointer, int handIndex, ref bool isMeleeWeapon, ref bool isRangedWeapon);

		// Token: 0x060015C9 RID: 5577
		[EngineMethod("get_immediate_enemy", false)]
		Agent GetImmediateEnemy(UIntPtr agentPointer);

		// Token: 0x060015CA RID: 5578
		[EngineMethod("try_get_immediate_agent_movement_data", false)]
		bool TryGetImmediateEnemyAgentMovementData(UIntPtr agentPointer, out float maximumForwardUnlimitedSpeed, out Vec3 position);

		// Token: 0x060015CB RID: 5579
		[EngineMethod("get_is_doing_passive_attack", false)]
		bool GetIsDoingPassiveAttack(UIntPtr agentPointer);

		// Token: 0x060015CC RID: 5580
		[EngineMethod("get_is_passive_usage_conditions_are_met", false)]
		bool GetIsPassiveUsageConditionsAreMet(UIntPtr agentPointer);

		// Token: 0x060015CD RID: 5581
		[EngineMethod("get_current_aiming_turbulance", false)]
		float GetCurrentAimingTurbulance(UIntPtr agentPointer);

		// Token: 0x060015CE RID: 5582
		[EngineMethod("get_current_aiming_error", false)]
		float GetCurrentAimingError(UIntPtr agentPointer);

		// Token: 0x060015CF RID: 5583
		[EngineMethod("get_body_rotation_constraint", false)]
		Vec3 GetBodyRotationConstraint(UIntPtr agentPointer, int channelIndex);

		// Token: 0x060015D0 RID: 5584
		[EngineMethod("get_action_direction", false)]
		Agent.UsageDirection GetActionDirection(int actionIndex);

		// Token: 0x060015D1 RID: 5585
		[EngineMethod("get_attack_direction_usage", false)]
		Agent.UsageDirection GetAttackDirectionUsage(UIntPtr agentPointer);

		// Token: 0x060015D2 RID: 5586
		[EngineMethod("handle_blow_aux", false)]
		void HandleBlowAux(UIntPtr agentPointer, ref Blow blow);

		// Token: 0x060015D3 RID: 5587
		[EngineMethod("make_voice", false)]
		void MakeVoice(UIntPtr agentPointer, int voiceType, int predictionType);

		// Token: 0x060015D4 RID: 5588
		[EngineMethod("set_hand_inverse_kinematics_frame", false)]
		bool SetHandInverseKinematicsFrame(UIntPtr agentPointer, ref MatrixFrame leftGlobalFrame, ref MatrixFrame rightGlobalFrame);

		// Token: 0x060015D5 RID: 5589
		[EngineMethod("set_hand_inverse_kinematics_frame_for_mission_object_usage", false)]
		bool SetHandInverseKinematicsFrameForMissionObjectUsage(UIntPtr agentPointer, in MatrixFrame localIKFrame, in MatrixFrame boundEntityGlobalFrame, float animationHeightDifference);

		// Token: 0x060015D6 RID: 5590
		[EngineMethod("clear_hand_inverse_kinematics", false)]
		void ClearHandInverseKinematics(UIntPtr agentPointer);

		// Token: 0x060015D7 RID: 5591
		[EngineMethod("debug_more", false)]
		void DebugMore(UIntPtr agentPointer);

		// Token: 0x060015D8 RID: 5592
		[EngineMethod("is_on_land", false)]
		bool IsOnLand(UIntPtr agentPointer);

		// Token: 0x060015D9 RID: 5593
		[EngineMethod("is_sliding", false)]
		bool IsSliding(UIntPtr agentPointer);

		// Token: 0x060015DA RID: 5594
		[EngineMethod("is_running_away", false)]
		bool IsRunningAway(UIntPtr agentPointer);

		// Token: 0x060015DB RID: 5595
		[EngineMethod("get_cur_weapon_offset", false)]
		Vec3 GetCurWeaponOffset(UIntPtr agentPointer);

		// Token: 0x060015DC RID: 5596
		[EngineMethod("get_walking_speed_limit_of_mountable", false)]
		float GetWalkSpeedLimitOfMountable(UIntPtr agentPointer);

		// Token: 0x060015DD RID: 5597
		[EngineMethod("create_blood_burst_at_limb", false)]
		void CreateBloodBurstAtLimb(UIntPtr agentPointer, sbyte realBoneIndex, float scale);

		// Token: 0x060015DE RID: 5598
		[EngineMethod("get_native_action_index", false)]
		int GetNativeActionIndex(string actionName);

		// Token: 0x060015DF RID: 5599
		[EngineMethod("set_guarded_agent_index", false)]
		void SetGuardedAgentIndex(UIntPtr agentPointer, int guardedAgentIndex);

		// Token: 0x060015E0 RID: 5600
		[EngineMethod("set_columnwise_follow_agent", false)]
		void SetColumnwiseFollowAgent(UIntPtr agentPointer, int followAgentIndex, ref Vec2 followPosition);

		// Token: 0x060015E1 RID: 5601
		[EngineMethod("get_monster_usage_index", false)]
		int GetMonsterUsageIndex(string monsterUsage);

		// Token: 0x060015E2 RID: 5602
		[EngineMethod("get_missile_range_with_height_difference", false)]
		float GetMissileRangeWithHeightDifference(UIntPtr agentPointer, float targetZ);

		// Token: 0x060015E3 RID: 5603
		[EngineMethod("set_formation_no", false)]
		void SetFormationNo(UIntPtr agentPointer, int formationNo);

		// Token: 0x060015E4 RID: 5604
		[EngineMethod("enforce_shield_usage", false)]
		void EnforceShieldUsage(UIntPtr agentPointer, Agent.UsageDirection direction);

		// Token: 0x060015E5 RID: 5605
		[EngineMethod("set_firing_order", false)]
		void SetFiringOrder(UIntPtr agentPointer, int order);

		// Token: 0x060015E6 RID: 5606
		[EngineMethod("set_riding_order", false)]
		void SetRidingOrder(UIntPtr agentPointer, int order);

		// Token: 0x060015E7 RID: 5607
		[EngineMethod("set_direction_change_tendency", false)]
		void SetDirectionChangeTendency(UIntPtr agentPointer, float tendency);

		// Token: 0x060015E8 RID: 5608
		[EngineMethod("set_ai_behavior_params", false)]
		void SetAIBehaviorParams(UIntPtr agentPointer, int behavior, float y1, float x2, float y2, float x3, float y3);

		// Token: 0x060015E9 RID: 5609
		[EngineMethod("set_all_ai_behavior_params", false)]
		void SetAllAIBehaviorParams(UIntPtr agentPointer, HumanAIComponent.BehaviorValues[] behaviorParams);

		// Token: 0x060015EA RID: 5610
		[EngineMethod("set_body_armor_material_type", false)]
		void SetBodyArmorMaterialType(UIntPtr agentPointer, ArmorComponent.ArmorMaterialTypes bodyArmorMaterialType);

		// Token: 0x060015EB RID: 5611
		[EngineMethod("get_maximum_number_of_agents", false)]
		int GetMaximumNumberOfAgents();

		// Token: 0x060015EC RID: 5612
		[EngineMethod("get_running_simulation_data_until_maximum_speed_reached", false)]
		void GetRunningSimulationDataUntilMaximumSpeedReached(UIntPtr agentPointer, ref float combatAccelerationTime, ref float maxSpeed, float[] speedValues);

		// Token: 0x060015ED RID: 5613
		[EngineMethod("get_last_target_visibility_state", false)]
		int GetLastTargetVisibilityState(UIntPtr agentPointer);

		// Token: 0x060015EE RID: 5614
		[EngineMethod("get_missile_range", false)]
		float GetMissileRange(UIntPtr agentPointer);

		// Token: 0x060015EF RID: 5615
		[EngineMethod("set_sound_occlusion", false)]
		void SetSoundOcclusion(UIntPtr agentPointer, float value);
	}
}
