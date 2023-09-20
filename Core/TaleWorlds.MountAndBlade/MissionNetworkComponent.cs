using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class MissionNetworkComponent : MissionNetwork
	{
		public event Action OnMyClientSynchronized;

		public event Action<NetworkCommunicator> OnClientSynchronizedEvent;

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClientOrReplay)
			{
				registerer.Register<CreateFreeMountAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateFreeMountAgent>(this.HandleServerEventCreateFreeMountAgentEvent));
				registerer.Register<CreateAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateAgent>(this.HandleServerEventCreateAgent));
				registerer.Register<CreateAgentVisuals>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateAgentVisuals>(this.HandleServerEventCreateAgentVisuals));
				registerer.Register<RemoveAgentVisualsForPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveAgentVisualsForPeer>(this.HandleServerEventRemoveAgentVisualsForPeer));
				registerer.Register<RemoveAgentVisualsFromIndexForPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveAgentVisualsFromIndexForPeer>(this.HandleServerEventRemoveAgentVisualsFromIndexForPeer));
				registerer.Register<ReplaceBotWithPlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<ReplaceBotWithPlayer>(this.HandleServerEventReplaceBotWithPlayer));
				registerer.Register<SetWieldedItemIndex>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetWieldedItemIndex>(this.HandleServerEventSetWieldedItemIndex));
				registerer.Register<SetWeaponNetworkData>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetWeaponNetworkData>(this.HandleServerEventSetWeaponNetworkData));
				registerer.Register<SetWeaponAmmoData>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetWeaponAmmoData>(this.HandleServerEventSetWeaponAmmoData));
				registerer.Register<SetWeaponReloadPhase>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetWeaponReloadPhase>(this.HandleServerEventSetWeaponReloadPhase));
				registerer.Register<WeaponUsageIndexChangeMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<WeaponUsageIndexChangeMessage>(this.HandleServerEventWeaponUsageIndexChangeMessage));
				registerer.Register<StartSwitchingWeaponUsageIndex>(new GameNetworkMessage.ServerMessageHandlerDelegate<StartSwitchingWeaponUsageIndex>(this.HandleServerEventStartSwitchingWeaponUsageIndex));
				registerer.Register<InitializeFormation>(new GameNetworkMessage.ServerMessageHandlerDelegate<InitializeFormation>(this.HandleServerEventInitializeFormation));
				registerer.Register<SetSpawnedFormationCount>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSpawnedFormationCount>(this.HandleServerEventSetSpawnedFormationCount));
				registerer.Register<AddTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<AddTeam>(this.HandleServerEventAddTeam));
				registerer.Register<TeamSetIsEnemyOf>(new GameNetworkMessage.ServerMessageHandlerDelegate<TeamSetIsEnemyOf>(this.HandleServerEventTeamSetIsEnemyOf));
				registerer.Register<AssignFormationToPlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<AssignFormationToPlayer>(this.HandleServerEventAssignFormationToPlayer));
				registerer.Register<ExistingObjectsBegin>(new GameNetworkMessage.ServerMessageHandlerDelegate<ExistingObjectsBegin>(this.HandleServerEventExistingObjectsBegin));
				registerer.Register<ExistingObjectsEnd>(new GameNetworkMessage.ServerMessageHandlerDelegate<ExistingObjectsEnd>(this.HandleServerEventExistingObjectsEnd));
				registerer.Register<ClearMission>(new GameNetworkMessage.ServerMessageHandlerDelegate<ClearMission>(this.HandleServerEventClearMission));
				registerer.Register<CreateMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateMissionObject>(this.HandleServerEventCreateMissionObject));
				registerer.Register<RemoveMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveMissionObject>(this.HandleServerEventRemoveMissionObject));
				registerer.Register<StopPhysicsAndSetFrameOfMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<StopPhysicsAndSetFrameOfMissionObject>(this.HandleServerEventStopPhysicsAndSetFrameOfMissionObject));
				registerer.Register<BurstMissionObjectParticles>(new GameNetworkMessage.ServerMessageHandlerDelegate<BurstMissionObjectParticles>(this.HandleServerEventBurstMissionObjectParticles));
				registerer.Register<SetMissionObjectVisibility>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectVisibility>(this.HandleServerEventSetMissionObjectVisibility));
				registerer.Register<SetMissionObjectDisabled>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectDisabled>(this.HandleServerEventSetMissionObjectDisabled));
				registerer.Register<SetMissionObjectColors>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectColors>(this.HandleServerEventSetMissionObjectColors));
				registerer.Register<SetMissionObjectFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectFrame>(this.HandleServerEventSetMissionObjectFrame));
				registerer.Register<SetMissionObjectGlobalFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectGlobalFrame>(this.HandleServerEventSetMissionObjectGlobalFrame));
				registerer.Register<SetMissionObjectFrameOverTime>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectFrameOverTime>(this.HandleServerEventSetMissionObjectFrameOverTime));
				registerer.Register<SetMissionObjectGlobalFrameOverTime>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectGlobalFrameOverTime>(this.HandleServerEventSetMissionObjectGlobalFrameOverTime));
				registerer.Register<SetMissionObjectAnimationAtChannel>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectAnimationAtChannel>(this.HandleServerEventSetMissionObjectAnimationAtChannel));
				registerer.Register<SetMissionObjectAnimationChannelParameter>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectAnimationChannelParameter>(this.HandleServerEventSetMissionObjectAnimationChannelParameter));
				registerer.Register<SetMissionObjectAnimationPaused>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectAnimationPaused>(this.HandleServerEventSetMissionObjectAnimationPaused));
				registerer.Register<SetMissionObjectVertexAnimation>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectVertexAnimation>(this.HandleServerEventSetMissionObjectVertexAnimation));
				registerer.Register<SetMissionObjectVertexAnimationProgress>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectVertexAnimationProgress>(this.HandleServerEventSetMissionObjectVertexAnimationProgress));
				registerer.Register<SetMissionObjectImpulse>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectImpulse>(this.HandleServerEventSetMissionObjectImpulse));
				registerer.Register<AddMissionObjectBodyFlags>(new GameNetworkMessage.ServerMessageHandlerDelegate<AddMissionObjectBodyFlags>(this.HandleServerEventAddMissionObjectBodyFlags));
				registerer.Register<RemoveMissionObjectBodyFlags>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveMissionObjectBodyFlags>(this.HandleServerEventRemoveMissionObjectBodyFlags));
				registerer.Register<SetMachineTargetRotation>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMachineTargetRotation>(this.HandleServerEventSetMachineTargetRotation));
				registerer.Register<SetUsableMissionObjectIsDeactivated>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetUsableMissionObjectIsDeactivated>(this.HandleServerEventSetUsableGameObjectIsDeactivated));
				registerer.Register<SetUsableMissionObjectIsDisabledForPlayers>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetUsableMissionObjectIsDisabledForPlayers>(this.HandleServerEventSetUsableGameObjectIsDisabledForPlayers));
				registerer.Register<SetRangedSiegeWeaponState>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetRangedSiegeWeaponState>(this.HandleServerEventSetRangedSiegeWeaponState));
				registerer.Register<SetRangedSiegeWeaponAmmo>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetRangedSiegeWeaponAmmo>(this.HandleServerEventSetRangedSiegeWeaponAmmo));
				registerer.Register<RangedSiegeWeaponChangeProjectile>(new GameNetworkMessage.ServerMessageHandlerDelegate<RangedSiegeWeaponChangeProjectile>(this.HandleServerEventRangedSiegeWeaponChangeProjectile));
				registerer.Register<SetStonePileAmmo>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetStonePileAmmo>(this.HandleServerEventSetStonePileAmmo));
				registerer.Register<SetSiegeMachineMovementDistance>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSiegeMachineMovementDistance>(this.HandleServerEventSetSiegeMachineMovementDistance));
				registerer.Register<SetSiegeLadderState>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSiegeLadderState>(this.HandleServerEventSetSiegeLadderState));
				registerer.Register<SetAgentTargetPositionAndDirection>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentTargetPositionAndDirection>(this.HandleServerEventSetAgentTargetPositionAndDirection));
				registerer.Register<SetAgentTargetPosition>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentTargetPosition>(this.HandleServerEventSetAgentTargetPosition));
				registerer.Register<ClearAgentTargetFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<ClearAgentTargetFrame>(this.HandleServerEventClearAgentTargetFrame));
				registerer.Register<AgentTeleportToFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<AgentTeleportToFrame>(this.HandleServerEventAgentTeleportToFrame));
				registerer.Register<SetSiegeTowerGateState>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSiegeTowerGateState>(this.HandleServerEventSetSiegeTowerGateState));
				registerer.Register<SetSiegeTowerHasArrivedAtTarget>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSiegeTowerHasArrivedAtTarget>(this.HandleServerEventSetSiegeTowerHasArrivedAtTarget));
				registerer.Register<SetBatteringRamHasArrivedAtTarget>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetBatteringRamHasArrivedAtTarget>(this.HandleServerEventSetBatteringRamHasArrivedAtTarget));
				registerer.Register<SetPeerTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetPeerTeam>(this.HandleServerEventSetPeerTeam));
				registerer.Register<SynchronizeMissionTimeTracker>(new GameNetworkMessage.ServerMessageHandlerDelegate<SynchronizeMissionTimeTracker>(this.HandleServerEventSyncMissionTimer));
				registerer.Register<SetAgentPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentPeer>(this.HandleServerEventSetAgentPeer));
				registerer.Register<SetAgentIsPlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentIsPlayer>(this.HandleServerEventSetAgentIsPlayer));
				registerer.Register<SetAgentHealth>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentHealth>(this.HandleServerEventSetAgentHealth));
				registerer.Register<AgentSetTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<AgentSetTeam>(this.HandleServerEventAgentSetTeam));
				registerer.Register<SetAgentActionSet>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentActionSet>(this.HandleServerEventSetAgentActionSet));
				registerer.Register<MakeAgentDead>(new GameNetworkMessage.ServerMessageHandlerDelegate<MakeAgentDead>(this.HandleServerEventMakeAgentDead));
				registerer.Register<AgentSetFormation>(new GameNetworkMessage.ServerMessageHandlerDelegate<AgentSetFormation>(this.HandleServerEventAgentSetFormation));
				registerer.Register<AddPrefabComponentToAgentBone>(new GameNetworkMessage.ServerMessageHandlerDelegate<AddPrefabComponentToAgentBone>(this.HandleServerEventAddPrefabComponentToAgentBone));
				registerer.Register<SetAgentPrefabComponentVisibility>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentPrefabComponentVisibility>(this.HandleServerEventSetAgentPrefabComponentVisibility));
				registerer.Register<UseObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<UseObject>(this.HandleServerEventUseObject));
				registerer.Register<StopUsingObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<StopUsingObject>(this.HandleServerEventStopUsingObject));
				registerer.Register<SyncObjectHitpoints>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncObjectHitpoints>(this.HandleServerEventHitSynchronizeObjectHitpoints));
				registerer.Register<SyncObjectDestructionLevel>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncObjectDestructionLevel>(this.HandleServerEventHitSynchronizeObjectDestructionLevel));
				registerer.Register<BurstAllHeavyHitParticles>(new GameNetworkMessage.ServerMessageHandlerDelegate<BurstAllHeavyHitParticles>(this.HandleServerEventHitBurstAllHeavyHitParticles));
				registerer.Register<SynchronizeMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<SynchronizeMissionObject>(this.HandleServerEventSynchronizeMissionObject));
				registerer.Register<SpawnWeaponWithNewEntity>(new GameNetworkMessage.ServerMessageHandlerDelegate<SpawnWeaponWithNewEntity>(this.HandleServerEventSpawnWeaponWithNewEntity));
				registerer.Register<AttachWeaponToSpawnedWeapon>(new GameNetworkMessage.ServerMessageHandlerDelegate<AttachWeaponToSpawnedWeapon>(this.HandleServerEventAttachWeaponToSpawnedWeapon));
				registerer.Register<AttachWeaponToAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<AttachWeaponToAgent>(this.HandleServerEventAttachWeaponToAgent));
				registerer.Register<SpawnWeaponAsDropFromAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<SpawnWeaponAsDropFromAgent>(this.HandleServerEventSpawnWeaponAsDropFromAgent));
				registerer.Register<SpawnAttachedWeaponOnSpawnedWeapon>(new GameNetworkMessage.ServerMessageHandlerDelegate<SpawnAttachedWeaponOnSpawnedWeapon>(this.HandleServerEventSpawnAttachedWeaponOnSpawnedWeapon));
				registerer.Register<SpawnAttachedWeaponOnCorpse>(new GameNetworkMessage.ServerMessageHandlerDelegate<SpawnAttachedWeaponOnCorpse>(this.HandleServerEventSpawnAttachedWeaponOnCorpse));
				registerer.Register<HandleMissileCollisionReaction>(new GameNetworkMessage.ServerMessageHandlerDelegate<HandleMissileCollisionReaction>(this.HandleServerEventHandleMissileCollisionReaction));
				registerer.Register<RemoveEquippedWeapon>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveEquippedWeapon>(this.HandleServerEventRemoveEquippedWeapon));
				registerer.Register<BarkAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<BarkAgent>(this.HandleServerEventBarkAgent));
				registerer.Register<EquipWeaponWithNewEntity>(new GameNetworkMessage.ServerMessageHandlerDelegate<EquipWeaponWithNewEntity>(this.HandleServerEventEquipWeaponWithNewEntity));
				registerer.Register<AttachWeaponToWeaponInAgentEquipmentSlot>(new GameNetworkMessage.ServerMessageHandlerDelegate<AttachWeaponToWeaponInAgentEquipmentSlot>(this.HandleServerEventAttachWeaponToWeaponInAgentEquipmentSlot));
				registerer.Register<EquipWeaponFromSpawnedItemEntity>(new GameNetworkMessage.ServerMessageHandlerDelegate<EquipWeaponFromSpawnedItemEntity>(this.HandleServerEventEquipWeaponFromSpawnedItemEntity));
				registerer.Register<CreateMissile>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateMissile>(this.HandleServerEventCreateMissile));
				registerer.Register<CombatLogNetworkMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<CombatLogNetworkMessage>(this.HandleServerEventAgentHit));
				registerer.Register<ConsumeWeaponAmount>(new GameNetworkMessage.ServerMessageHandlerDelegate<ConsumeWeaponAmount>(this.HandleServerEventConsumeWeaponAmount));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.Register<SetFollowedAgent>(new GameNetworkMessage.ClientMessageHandlerDelegate<SetFollowedAgent>(this.HandleClientEventSetFollowedAgent));
				registerer.Register<SetMachineRotation>(new GameNetworkMessage.ClientMessageHandlerDelegate<SetMachineRotation>(this.HandleClientEventSetMachineRotation));
				registerer.Register<RequestUseObject>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestUseObject>(this.HandleClientEventRequestUseObject));
				registerer.Register<RequestStopUsingObject>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestStopUsingObject>(this.HandleClientEventRequestStopUsingObject));
				registerer.Register<ApplyOrder>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrder>(this.HandleClientEventApplyOrder));
				registerer.Register<ApplySiegeWeaponOrder>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplySiegeWeaponOrder>(this.HandleClientEventApplySiegeWeaponOrder));
				registerer.Register<ApplyOrderWithPosition>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithPosition>(this.HandleClientEventApplyOrderWithPosition));
				registerer.Register<ApplyOrderWithFormation>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithFormation>(this.HandleClientEventApplyOrderWithFormation));
				registerer.Register<ApplyOrderWithFormationAndPercentage>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithFormationAndPercentage>(this.HandleClientEventApplyOrderWithFormationAndPercentage));
				registerer.Register<ApplyOrderWithFormationAndNumber>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithFormationAndNumber>(this.HandleClientEventApplyOrderWithFormationAndNumber));
				registerer.Register<ApplyOrderWithTwoPositions>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithTwoPositions>(this.HandleClientEventApplyOrderWithTwoPositions));
				registerer.Register<ApplyOrderWithMissionObject>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithMissionObject>(this.HandleClientEventApplyOrderWithGameEntity));
				registerer.Register<ApplyOrderWithAgent>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithAgent>(this.HandleClientEventApplyOrderWithAgent));
				registerer.Register<SelectAllFormations>(new GameNetworkMessage.ClientMessageHandlerDelegate<SelectAllFormations>(this.HandleClientEventSelectAllFormations));
				registerer.Register<SelectAllSiegeWeapons>(new GameNetworkMessage.ClientMessageHandlerDelegate<SelectAllSiegeWeapons>(this.HandleClientEventSelectAllSiegeWeapons));
				registerer.Register<ClearSelectedFormations>(new GameNetworkMessage.ClientMessageHandlerDelegate<ClearSelectedFormations>(this.HandleClientEventClearSelectedFormations));
				registerer.Register<SelectFormation>(new GameNetworkMessage.ClientMessageHandlerDelegate<SelectFormation>(this.HandleClientEventSelectFormation));
				registerer.Register<SelectSiegeWeapon>(new GameNetworkMessage.ClientMessageHandlerDelegate<SelectSiegeWeapon>(this.HandleClientEventSelectSiegeWeapon));
				registerer.Register<UnselectFormation>(new GameNetworkMessage.ClientMessageHandlerDelegate<UnselectFormation>(this.HandleClientEventUnselectFormation));
				registerer.Register<UnselectSiegeWeapon>(new GameNetworkMessage.ClientMessageHandlerDelegate<UnselectSiegeWeapon>(this.HandleClientEventUnselectSiegeWeapon));
				registerer.Register<DropWeapon>(new GameNetworkMessage.ClientMessageHandlerDelegate<DropWeapon>(this.HandleClientEventDropWeapon));
				registerer.Register<CancelCheering>(new GameNetworkMessage.ClientMessageHandlerDelegate<CancelCheering>(this.HandleClientEventCancelCheering));
				registerer.Register<CheerSelected>(new GameNetworkMessage.ClientMessageHandlerDelegate<CheerSelected>(this.HandleClientEventCheerSelected));
				registerer.Register<BarkSelected>(new GameNetworkMessage.ClientMessageHandlerDelegate<BarkSelected>(this.HandleClientEventBarkSelected));
				registerer.Register<AgentVisualsBreakInvulnerability>(new GameNetworkMessage.ClientMessageHandlerDelegate<AgentVisualsBreakInvulnerability>(this.HandleClientEventBreakAgentVisualsInvulnerability));
				registerer.Register<RequestToSpawnAsBot>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestToSpawnAsBot>(this.HandleClientEventRequestToSpawnAsBot));
			}
		}

		private Team GetTeamOfPeer(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component.ControlledAgent == null)
			{
				MBDebug.Print("peer.ControlledAgent == null", 0, Debug.DebugColor.White, 17592186044416UL);
				return null;
			}
			Team team = component.ControlledAgent.Team;
			if (team == null)
			{
				MBDebug.Print("peersTeam == null", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			return team;
		}

		private OrderController GetOrderControllerOfPeer(NetworkCommunicator networkPeer)
		{
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			if (teamOfPeer != null)
			{
				return teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent);
			}
			MBDebug.Print("peersTeam == null", 0, Debug.DebugColor.White, 17592186044416UL);
			return null;
		}

		private void HandleServerEventSyncMissionTimer(SynchronizeMissionTimeTracker message)
		{
			base.Mission.MissionTimeTracker.UpdateSync(message.CurrentTime);
		}

		private void HandleServerEventSetPeerTeam(SetPeerTeam message)
		{
			message.Peer.GetComponent<MissionPeer>().Team = message.Team;
			if (message.Peer.IsMine)
			{
				base.Mission.PlayerTeam = message.Team;
			}
		}

		private void HandleServerEventCreateFreeMountAgentEvent(CreateFreeMountAgent message)
		{
			Mission mission = base.Mission;
			EquipmentElement horseItem = message.HorseItem;
			EquipmentElement horseHarnessItem = message.HorseHarnessItem;
			Vec3 position = message.Position;
			Vec2 vec = message.Direction;
			vec = vec.Normalized();
			mission.SpawnMonster(horseItem, horseHarnessItem, position, vec, message.AgentIndex);
		}

		private void HandleServerEventCreateAgent(CreateAgent message)
		{
			BasicCharacterObject character = message.Character;
			NetworkCommunicator peer = message.Peer;
			MissionPeer missionPeer = ((peer != null) ? peer.GetComponent<MissionPeer>() : null);
			AgentBuildData agentBuildData = new AgentBuildData(character).MissionPeer(message.IsPlayerAgent ? missionPeer : null).Monster(message.Monster).TroopOrigin(new BasicBattleAgentOrigin(character))
				.Equipment(message.SpawnEquipment)
				.EquipmentSeed(message.BodyPropertiesSeed);
			Vec3 position = message.Position;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(position);
			Vec2 vec = message.Direction;
			vec = vec.Normalized();
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(vec).MissionEquipment(message.SpawnMissionEquipment).Team(message.Team)
				.Index(message.AgentIndex)
				.MountIndex(message.MountAgentIndex)
				.IsFemale(message.IsFemale)
				.ClothingColor1(message.ClothingColor1)
				.ClothingColor2(message.ClothingColor2);
			Formation formation = null;
			if (message.Team != null && message.FormationIndex >= 0 && !GameNetwork.IsReplay)
			{
				formation = message.Team.GetFormation((FormationClass)message.FormationIndex);
				agentBuildData3.Formation(formation);
			}
			if (message.IsPlayerAgent)
			{
				agentBuildData3.BodyProperties(missionPeer.Peer.BodyProperties);
				agentBuildData3.Age((int)agentBuildData3.AgentBodyProperties.Age);
			}
			else
			{
				agentBuildData3.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData3.AgentRace, agentBuildData3.AgentIsFemale, character.GetBodyPropertiesMin(false), character.GetBodyPropertiesMax(), (int)agentBuildData3.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData3.AgentEquipmentSeed, character.HairTags, character.BeardTags, character.TattooTags));
			}
			Banner banner = null;
			if (formation != null)
			{
				if (!string.IsNullOrEmpty(formation.BannerCode))
				{
					if (formation.Banner == null)
					{
						banner = new Banner(formation.BannerCode, message.Team.Color, message.Team.Color2);
						formation.Banner = banner;
					}
					else
					{
						banner = formation.Banner;
					}
				}
			}
			else if (missionPeer != null)
			{
				banner = new Banner(missionPeer.Peer.BannerCode, message.Team.Color, message.Team.Color2);
			}
			agentBuildData3.Banner(banner);
			Agent mountAgent = base.Mission.SpawnAgent(agentBuildData3, false).MountAgent;
		}

		private void HandleServerEventCreateAgentVisuals(CreateAgentVisuals message)
		{
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			BattleSideEnum side = component.Team.Side;
			BasicCharacterObject character = message.Character;
			AgentBuildData agentBuildData = new AgentBuildData(character).VisualsIndex(message.VisualsIndex).Equipment(message.Equipment).EquipmentSeed(message.BodyPropertiesSeed)
				.IsFemale(message.IsFemale)
				.ClothingColor1(character.Culture.Color)
				.ClothingColor2(character.Culture.Color2);
			if (message.VisualsIndex == 0)
			{
				agentBuildData.BodyProperties(component.Peer.BodyProperties);
			}
			else
			{
				agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentRace, agentBuildData.AgentIsFemale, character.GetBodyPropertiesMin(false), character.GetBodyPropertiesMax(), (int)agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, message.BodyPropertiesSeed, character.HairTags, character.BeardTags, character.TattooTags));
			}
			base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().SpawnAgentVisualsForPeer(component, agentBuildData, message.SelectedEquipmentSetIndex, false, message.TroopCountInFormation);
		}

		private void HandleServerEventRemoveAgentVisualsForPeer(RemoveAgentVisualsForPeer message)
		{
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, false);
		}

		private void HandleServerEventRemoveAgentVisualsFromIndexForPeer(RemoveAgentVisualsFromIndexForPeer message)
		{
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisualsWithVisualIndex(component, message.VisualsIndex, false);
		}

		private void HandleServerEventReplaceBotWithPlayer(ReplaceBotWithPlayer message)
		{
			Agent botAgent = message.BotAgent;
			if (botAgent.Formation != null)
			{
				botAgent.Formation.PlayerOwner = botAgent;
			}
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			botAgent.MissionPeer = message.Peer.GetComponent<MissionPeer>();
			botAgent.Formation = component.ControlledFormation;
			botAgent.Health = (float)message.Health;
			if (botAgent.MountAgent != null)
			{
				botAgent.MountAgent.Health = (float)message.MountHealth;
			}
			if (botAgent.Formation != null)
			{
				botAgent.Team.AssignPlayerAsSergeantOfFormation(component, component.ControlledFormation.FormationIndex);
			}
		}

		private void HandleServerEventSetWieldedItemIndex(SetWieldedItemIndex message)
		{
			if (message.Agent != null)
			{
				message.Agent.SetWieldedItemIndexAsClient(message.IsLeftHand ? Agent.HandIndex.OffHand : Agent.HandIndex.MainHand, message.WieldedItemIndex, message.IsWieldedInstantly, message.IsWieldedOnSpawn, message.MainHandCurrentUsageIndex);
				message.Agent.UpdateAgentStats();
			}
		}

		private void HandleServerEventSetWeaponNetworkData(SetWeaponNetworkData message)
		{
			ItemObject item = message.Agent.Equipment[message.WeaponEquipmentIndex].Item;
			WeaponComponentData weaponComponentData = ((item != null) ? item.PrimaryWeapon : null);
			if (weaponComponentData != null)
			{
				if (weaponComponentData.WeaponFlags.HasAnyFlag(WeaponFlags.HasHitPoints))
				{
					message.Agent.ChangeWeaponHitPoints(message.WeaponEquipmentIndex, message.DataValue);
					return;
				}
				if (weaponComponentData.IsConsumable)
				{
					message.Agent.SetWeaponAmountInSlot(message.WeaponEquipmentIndex, message.DataValue, true);
				}
			}
		}

		private void HandleServerEventSetWeaponAmmoData(SetWeaponAmmoData message)
		{
			if (message.Agent.Equipment[message.WeaponEquipmentIndex].CurrentUsageItem.IsRangedWeapon)
			{
				message.Agent.SetWeaponAmmoAsClient(message.WeaponEquipmentIndex, message.AmmoEquipmentIndex, message.Ammo);
				return;
			}
			Debug.FailedAssert("Invalid item type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MissionNetworkComponent.cs", "HandleServerEventSetWeaponAmmoData", 412);
		}

		private void HandleServerEventSetWeaponReloadPhase(SetWeaponReloadPhase message)
		{
			message.Agent.SetWeaponReloadPhaseAsClient(message.EquipmentIndex, message.ReloadPhase);
		}

		private void HandleServerEventWeaponUsageIndexChangeMessage(WeaponUsageIndexChangeMessage message)
		{
			message.Agent.SetUsageIndexOfWeaponInSlotAsClient(message.SlotIndex, message.UsageIndex);
		}

		private void HandleServerEventStartSwitchingWeaponUsageIndex(StartSwitchingWeaponUsageIndex message)
		{
			message.Agent.StartSwitchingWeaponUsageIndexAsClient(message.EquipmentIndex, message.UsageIndex, message.CurrentMovementFlagUsageDirection);
		}

		private void HandleServerEventInitializeFormation(InitializeFormation message)
		{
			message.Team.GetFormation((FormationClass)message.FormationIndex).BannerCode = message.BannerCode;
		}

		private void HandleServerEventSetSpawnedFormationCount(SetSpawnedFormationCount message)
		{
			base.Mission.NumOfFormationsSpawnedTeamOne = message.NumOfFormationsTeamOne;
			base.Mission.NumOfFormationsSpawnedTeamTwo = message.NumOfFormationsTeamTwo;
		}

		private void HandleServerEventAddTeam(AddTeam message)
		{
			Banner banner = (string.IsNullOrEmpty(message.BannerCode) ? null : new Banner(message.BannerCode, message.Color, message.Color2));
			base.Mission.Teams.Add(message.Side, message.Color, message.Color2, banner, message.IsPlayerGeneral, message.IsPlayerSergeant, true);
		}

		private void HandleServerEventTeamSetIsEnemyOf(TeamSetIsEnemyOf message)
		{
			message.Team1.SetIsEnemyOf(message.Team2, message.IsEnemyOf);
		}

		private void HandleServerEventAssignFormationToPlayer(AssignFormationToPlayer message)
		{
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			component.Team.AssignPlayerAsSergeantOfFormation(component, message.FormationClass);
		}

		private void HandleServerEventExistingObjectsBegin(ExistingObjectsBegin message)
		{
		}

		private void HandleServerEventExistingObjectsEnd(ExistingObjectsEnd message)
		{
		}

		private void HandleServerEventClearMission(ClearMission message)
		{
			base.Mission.ResetMission();
		}

		private void HandleServerEventCreateMissionObject(CreateMissionObject message)
		{
			GameEntity gameEntity = GameEntity.Instantiate(base.Mission.Scene, message.Prefab, message.Frame);
			MissionObject firstScriptOfType = gameEntity.GetFirstScriptOfType<MissionObject>();
			if (firstScriptOfType != null)
			{
				firstScriptOfType.Id = message.ObjectId;
				int num = 0;
				using (IEnumerator<GameEntity> enumerator = gameEntity.GetChildren().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionObject firstScriptOfType2;
						if ((firstScriptOfType2 = enumerator.Current.GetFirstScriptOfType<MissionObject>()) != null)
						{
							firstScriptOfType2.Id = message.ChildObjectIds[num++];
						}
					}
				}
			}
		}

		private void HandleServerEventRemoveMissionObject(RemoveMissionObject message)
		{
			MissionObject missionObject = base.Mission.MissionObjects.FirstOrDefault((MissionObject mo) => mo.Id == message.ObjectId);
			if (missionObject == null)
			{
				return;
			}
			missionObject.GameEntity.Remove(82);
		}

		private void HandleServerEventStopPhysicsAndSetFrameOfMissionObject(StopPhysicsAndSetFrameOfMissionObject message)
		{
			SpawnedItemEntity spawnedItemEntity = (SpawnedItemEntity)base.Mission.MissionObjects.FirstOrDefault((MissionObject mo) => mo.Id == message.ObjectId);
			if (spawnedItemEntity == null)
			{
				return;
			}
			MatrixFrame frame = message.Frame;
			MissionObject parent = message.Parent;
			spawnedItemEntity.StopPhysicsAndSetFrameForClient(frame, (parent != null) ? parent.GameEntity : null);
		}

		private void HandleServerEventBurstMissionObjectParticles(BurstMissionObjectParticles message)
		{
			(message.MissionObject as SynchedMissionObject).BurstParticlesSynched(message.DoChildren);
		}

		private void HandleServerEventSetMissionObjectVisibility(SetMissionObjectVisibility message)
		{
			message.MissionObject.GameEntity.SetVisibilityExcludeParents(message.Visible);
		}

		private void HandleServerEventSetMissionObjectDisabled(SetMissionObjectDisabled message)
		{
			message.MissionObject.SetDisabledAndMakeInvisible(false);
		}

		private void HandleServerEventSetMissionObjectColors(SetMissionObjectColors message)
		{
			SynchedMissionObject synchedMissionObject = message.MissionObject as SynchedMissionObject;
			if (synchedMissionObject != null)
			{
				synchedMissionObject.SetTeamColors(message.Color, message.Color2);
			}
		}

		private void HandleServerEventSetMissionObjectFrame(SetMissionObjectFrame message)
		{
			SynchedMissionObject synchedMissionObject = message.MissionObject as SynchedMissionObject;
			MatrixFrame frame = message.Frame;
			synchedMissionObject.SetFrameSynched(ref frame, true);
		}

		private void HandleServerEventSetMissionObjectGlobalFrame(SetMissionObjectGlobalFrame message)
		{
			SynchedMissionObject synchedMissionObject = message.MissionObject as SynchedMissionObject;
			MatrixFrame frame = message.Frame;
			synchedMissionObject.SetGlobalFrameSynched(ref frame, true);
		}

		private void HandleServerEventSetMissionObjectFrameOverTime(SetMissionObjectFrameOverTime message)
		{
			SynchedMissionObject synchedMissionObject = message.MissionObject as SynchedMissionObject;
			MatrixFrame frame = message.Frame;
			synchedMissionObject.SetFrameSynchedOverTime(ref frame, message.Duration, true);
		}

		private void HandleServerEventSetMissionObjectGlobalFrameOverTime(SetMissionObjectGlobalFrameOverTime message)
		{
			SynchedMissionObject synchedMissionObject = message.MissionObject as SynchedMissionObject;
			MatrixFrame frame = message.Frame;
			synchedMissionObject.SetGlobalFrameSynchedOverTime(ref frame, message.Duration, true);
		}

		private void HandleServerEventSetMissionObjectAnimationAtChannel(SetMissionObjectAnimationAtChannel message)
		{
			message.MissionObject.GameEntity.Skeleton.SetAnimationAtChannel(message.AnimationIndex, message.ChannelNo, message.AnimationSpeed, -1f, 0f);
		}

		private void HandleServerEventSetRangedSiegeWeaponAmmo(SetRangedSiegeWeaponAmmo message)
		{
			message.RangedSiegeWeapon.SetAmmo(message.AmmoCount);
		}

		private void HandleServerEventRangedSiegeWeaponChangeProjectile(RangedSiegeWeaponChangeProjectile message)
		{
			message.RangedSiegeWeapon.ChangeProjectileEntityClient(message.Index);
		}

		private void HandleServerEventSetStonePileAmmo(SetStonePileAmmo message)
		{
			message.StonePile.SetAmmo(message.AmmoCount);
		}

		private void HandleServerEventSetRangedSiegeWeaponState(SetRangedSiegeWeaponState message)
		{
			message.RangedSiegeWeapon.State = message.State;
		}

		private void HandleServerEventSetSiegeLadderState(SetSiegeLadderState message)
		{
			message.SiegeLadder.State = message.State;
		}

		private void HandleServerEventSetSiegeTowerGateState(SetSiegeTowerGateState message)
		{
			message.SiegeTower.State = message.State;
		}

		private void HandleServerEventSetSiegeTowerHasArrivedAtTarget(SetSiegeTowerHasArrivedAtTarget message)
		{
			message.SiegeTower.HasArrivedAtTarget = true;
		}

		private void HandleServerEventSetBatteringRamHasArrivedAtTarget(SetBatteringRamHasArrivedAtTarget message)
		{
			message.BatteringRam.HasArrivedAtTarget = true;
		}

		private void HandleServerEventSetSiegeMachineMovementDistance(SetSiegeMachineMovementDistance message)
		{
			if (message.UsableMachine != null)
			{
				if (message.UsableMachine is SiegeTower)
				{
					((SiegeTower)message.UsableMachine).MovementComponent.SetDistanceTraveledAsClient(message.Distance);
					return;
				}
				((BatteringRam)message.UsableMachine).MovementComponent.SetDistanceTraveledAsClient(message.Distance);
			}
		}

		private void HandleServerEventSetMissionObjectAnimationChannelParameter(SetMissionObjectAnimationChannelParameter message)
		{
			if (message.MissionObject != null)
			{
				message.MissionObject.GameEntity.Skeleton.SetAnimationParameterAtChannel(message.ChannelNo, message.Parameter);
			}
		}

		private void HandleServerEventSetMissionObjectVertexAnimation(SetMissionObjectVertexAnimation message)
		{
			if (message.MissionObject != null)
			{
				(message.MissionObject as VertexAnimator).SetAnimationSynched(message.BeginKey, message.EndKey, message.Speed);
			}
		}

		private void HandleServerEventSetMissionObjectVertexAnimationProgress(SetMissionObjectVertexAnimationProgress message)
		{
			if (message.MissionObject != null)
			{
				(message.MissionObject as VertexAnimator).SetProgressSynched(message.Progress);
			}
		}

		private void HandleServerEventSetMissionObjectAnimationPaused(SetMissionObjectAnimationPaused message)
		{
			if (message.MissionObject != null)
			{
				if (message.IsPaused)
				{
					message.MissionObject.GameEntity.PauseSkeletonAnimation();
					return;
				}
				message.MissionObject.GameEntity.ResumeSkeletonAnimation();
			}
		}

		private void HandleServerEventAddMissionObjectBodyFlags(AddMissionObjectBodyFlags message)
		{
			if (message.MissionObject != null)
			{
				message.MissionObject.GameEntity.AddBodyFlags(message.BodyFlags, message.ApplyToChildren);
			}
		}

		private void HandleServerEventRemoveMissionObjectBodyFlags(RemoveMissionObjectBodyFlags message)
		{
			if (message.MissionObject != null)
			{
				message.MissionObject.GameEntity.RemoveBodyFlags(message.BodyFlags, message.ApplyToChildren);
			}
		}

		private void HandleServerEventSetMachineTargetRotation(SetMachineTargetRotation message)
		{
			if (message.UsableMachine != null && message.UsableMachine.PilotAgent != null)
			{
				((RangedSiegeWeapon)message.UsableMachine).AimAtRotation(message.HorizontalRotation, message.VerticalRotation);
			}
		}

		private void HandleServerEventSetUsableGameObjectIsDeactivated(SetUsableMissionObjectIsDeactivated message)
		{
			if (message.UsableGameObject != null)
			{
				message.UsableGameObject.IsDeactivated = message.IsDeactivated;
			}
		}

		private void HandleServerEventSetUsableGameObjectIsDisabledForPlayers(SetUsableMissionObjectIsDisabledForPlayers message)
		{
			if (message.UsableGameObject != null)
			{
				message.UsableGameObject.IsDisabledForPlayers = message.IsDisabledForPlayers;
			}
		}

		private void HandleServerEventSetMissionObjectImpulse(SetMissionObjectImpulse message)
		{
			if (message.MissionObject != null)
			{
				Vec3 position = message.Position;
				message.MissionObject.GameEntity.ApplyLocalImpulseToDynamicBody(position, message.Impulse);
			}
		}

		private void HandleServerEventSetAgentTargetPositionAndDirection(SetAgentTargetPositionAndDirection message)
		{
			Vec2 position = message.Position;
			Vec3 direction = message.Direction;
			message.Agent.SetTargetPositionAndDirectionSynched(ref position, ref direction);
		}

		private void HandleServerEventSetAgentTargetPosition(SetAgentTargetPosition message)
		{
			Vec2 position = message.Position;
			message.Agent.SetTargetPositionSynched(ref position);
		}

		private void HandleServerEventClearAgentTargetFrame(ClearAgentTargetFrame message)
		{
			message.Agent.ClearTargetFrame();
		}

		private void HandleServerEventAgentTeleportToFrame(AgentTeleportToFrame message)
		{
			message.Agent.TeleportToPosition(message.Position);
			Vec2 vec = message.Direction.Normalized();
			message.Agent.SetMovementDirection(vec);
			message.Agent.LookDirection = vec.ToVec3(0f);
		}

		private void HandleServerEventSetAgentPeer(SetAgentPeer message)
		{
			if (message.Agent != null)
			{
				NetworkCommunicator peer = message.Peer;
				MissionPeer missionPeer = ((peer != null) ? peer.GetComponent<MissionPeer>() : null);
				message.Agent.MissionPeer = missionPeer;
			}
		}

		private void HandleServerEventSetAgentIsPlayer(SetAgentIsPlayer message)
		{
			if (message.Agent.Controller == Agent.ControllerType.Player != message.IsPlayer)
			{
				if (!message.Agent.IsMine)
				{
					message.Agent.Controller = Agent.ControllerType.None;
					return;
				}
				message.Agent.Controller = Agent.ControllerType.Player;
			}
		}

		private void HandleServerEventSetAgentHealth(SetAgentHealth message)
		{
			message.Agent.Health = (float)message.Health;
		}

		private void HandleServerEventAgentSetTeam(AgentSetTeam message)
		{
			message.Agent.SetTeam(base.Mission.Teams.Find(message.Team), false);
		}

		private void HandleServerEventSetAgentActionSet(SetAgentActionSet message)
		{
			AnimationSystemData animationSystemData = message.Agent.Monster.FillAnimationSystemData(message.ActionSet, message.StepSize, false);
			animationSystemData.NumPaces = message.NumPaces;
			animationSystemData.MonsterUsageSetIndex = message.MonsterUsageSetIndex;
			animationSystemData.WalkingSpeedLimit = message.WalkingSpeedLimit;
			animationSystemData.CrouchWalkingSpeedLimit = message.CrouchWalkingSpeedLimit;
			message.Agent.SetActionSet(ref animationSystemData);
		}

		private void HandleServerEventMakeAgentDead(MakeAgentDead message)
		{
			message.Agent.MakeDead(message.IsKilled, message.ActionCodeIndex);
		}

		private void HandleServerEventAddPrefabComponentToAgentBone(AddPrefabComponentToAgentBone message)
		{
			message.Agent.AddSynchedPrefabComponentToBone(message.PrefabName, message.BoneIndex);
		}

		private void HandleServerEventSetAgentPrefabComponentVisibility(SetAgentPrefabComponentVisibility message)
		{
			message.Agent.SetSynchedPrefabComponentVisibility(message.ComponentIndex, message.Visibility);
		}

		private void HandleServerEventAgentSetFormation(AgentSetFormation message)
		{
			Agent agent = message.Agent;
			Team team = agent.Team;
			Formation formation = null;
			if (team != null)
			{
				formation = ((message.FormationIndex >= 0) ? team.GetFormation((FormationClass)message.FormationIndex) : null);
			}
			agent.Formation = formation;
		}

		private void HandleServerEventUseObject(UseObject message)
		{
			UsableMissionObject usableGameObject = message.UsableGameObject;
			if (usableGameObject == null)
			{
				return;
			}
			usableGameObject.SetUserForClient(message.Agent);
		}

		private void HandleServerEventStopUsingObject(StopUsingObject message)
		{
			Agent agent = message.Agent;
			if (agent == null)
			{
				return;
			}
			agent.StopUsingGameObject(message.IsSuccessful, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
		}

		private void HandleServerEventHitSynchronizeObjectHitpoints(SyncObjectHitpoints message)
		{
			if (message.MissionObject != null)
			{
				message.MissionObject.GameEntity.GetFirstScriptOfType<DestructableComponent>().HitPoint = message.Hitpoints;
			}
		}

		private void HandleServerEventHitSynchronizeObjectDestructionLevel(SyncObjectDestructionLevel message)
		{
			MissionObject missionObject = message.MissionObject;
			if (missionObject == null)
			{
				return;
			}
			missionObject.GameEntity.GetFirstScriptOfType<DestructableComponent>().SetDestructionLevel(message.DestructionLevel, message.ForcedIndex, message.BlowMagnitude, message.BlowPosition, message.BlowDirection, false);
		}

		private void HandleServerEventHitBurstAllHeavyHitParticles(BurstAllHeavyHitParticles message)
		{
			MissionObject missionObject = message.MissionObject;
			if (missionObject == null)
			{
				return;
			}
			missionObject.GameEntity.GetFirstScriptOfType<DestructableComponent>().BurstHeavyHitParticles();
		}

		private void HandleServerEventSynchronizeMissionObject(SynchronizeMissionObject message)
		{
		}

		private void HandleServerEventSpawnWeaponWithNewEntity(SpawnWeaponWithNewEntity message)
		{
			GameEntity gameEntity = base.Mission.SpawnWeaponWithNewEntityAux(message.Weapon, message.WeaponSpawnFlags, message.Frame, message.ForcedIndex, message.ParentMissionObject, message.HasLifeTime);
			if (!message.IsVisible)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
		}

		private void HandleServerEventAttachWeaponToSpawnedWeapon(AttachWeaponToSpawnedWeapon message)
		{
			base.Mission.AttachWeaponWithNewEntityToSpawnedWeapon(message.Weapon, message.MissionObject as SpawnedItemEntity, message.AttachLocalFrame);
		}

		private void HandleServerEventAttachWeaponToAgent(AttachWeaponToAgent message)
		{
			MatrixFrame attachLocalFrame = message.AttachLocalFrame;
			message.Agent.AttachWeaponToBone(message.Weapon, null, message.BoneIndex, ref attachLocalFrame);
		}

		private void HandleServerEventHandleMissileCollisionReaction(HandleMissileCollisionReaction message)
		{
			base.Mission.HandleMissileCollisionReaction(message.MissileIndex, message.CollisionReaction, message.AttachLocalFrame, message.IsAttachedFrameLocal, message.AttackerAgent, message.AttachedAgent, message.AttachedToShield, message.AttachedBoneIndex, message.AttachedMissionObject, message.BounceBackVelocity, message.BounceBackAngularVelocity, message.ForcedSpawnIndex);
		}

		private void HandleServerEventSpawnWeaponAsDropFromAgent(SpawnWeaponAsDropFromAgent message)
		{
			Debug.Print("HandleServerEventSpawnWeaponAsDropFromAgent started.", 0, Debug.DebugColor.White, 17592186044416UL);
			Vec3 velocity = message.Velocity;
			Vec3 angularVelocity = message.AngularVelocity;
			base.Mission.SpawnWeaponAsDropFromAgentAux(message.Agent, message.EquipmentIndex, ref velocity, ref angularVelocity, message.WeaponSpawnFlags, message.ForcedIndex);
			Debug.Print("HandleServerEventSpawnWeaponAsDropFromAgent ended.", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		private void HandleServerEventSpawnAttachedWeaponOnSpawnedWeapon(SpawnAttachedWeaponOnSpawnedWeapon message)
		{
			base.Mission.SpawnAttachedWeaponOnSpawnedWeapon(message.SpawnedWeapon, message.AttachmentIndex, message.ForcedIndex);
		}

		private void HandleServerEventSpawnAttachedWeaponOnCorpse(SpawnAttachedWeaponOnCorpse message)
		{
			base.Mission.SpawnAttachedWeaponOnCorpse(message.Agent, message.AttachedIndex, message.ForcedIndex);
		}

		private void HandleServerEventRemoveEquippedWeapon(RemoveEquippedWeapon message)
		{
			message.Agent.RemoveEquippedWeapon(message.SlotIndex);
		}

		private void HandleServerEventBarkAgent(BarkAgent message)
		{
			message.Agent.HandleBark(message.IndexOfBark);
			if (!this._chatBox.IsPlayerMuted(message.Agent.MissionPeer.Peer.Id))
			{
				GameTexts.SetVariable("LEFT", message.Agent.Name);
				GameTexts.SetVariable("RIGHT", SkinVoiceManager.VoiceType.MpBarks[message.IndexOfBark].GetName());
				InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString(), Color.White, "Bark"));
			}
		}

		private void HandleServerEventEquipWeaponWithNewEntity(EquipWeaponWithNewEntity message)
		{
			if (message.Agent != null)
			{
				MissionWeapon weapon = message.Weapon;
				message.Agent.EquipWeaponWithNewEntity(message.SlotIndex, ref weapon);
			}
		}

		private void HandleServerEventAttachWeaponToWeaponInAgentEquipmentSlot(AttachWeaponToWeaponInAgentEquipmentSlot message)
		{
			MatrixFrame attachLocalFrame = message.AttachLocalFrame;
			message.Agent.AttachWeaponToWeapon(message.SlotIndex, message.Weapon, null, ref attachLocalFrame);
		}

		private void HandleServerEventEquipWeaponFromSpawnedItemEntity(EquipWeaponFromSpawnedItemEntity message)
		{
			message.Agent.EquipWeaponFromSpawnedItemEntity(message.SlotIndex, message.SpawnedItemEntity, message.RemoveWeapon);
		}

		private void HandleServerEventCreateMissile(CreateMissile message)
		{
			if (message.WeaponIndex != EquipmentIndex.None)
			{
				Vec3 vec = message.Direction * message.Speed;
				base.Mission.OnAgentShootMissile(message.Agent, message.WeaponIndex, message.Position, vec, message.Orientation, message.HasRigidBody, message.IsPrimaryWeaponShot, message.MissileIndex);
				return;
			}
			base.Mission.AddCustomMissile(message.Agent, message.Weapon, message.Position, message.Direction, message.Orientation, message.Speed, message.Speed, message.HasRigidBody, message.MissionObjectToIgnore, message.MissileIndex);
		}

		private void HandleServerEventAgentHit(CombatLogNetworkMessage networkMessage)
		{
			CombatLogManager.GenerateCombatLog(networkMessage.GetData());
		}

		private void HandleServerEventConsumeWeaponAmount(ConsumeWeaponAmount message)
		{
			(message.SpawnedItemEntity as SpawnedItemEntity).ConsumeWeaponAmount(message.ConsumedAmount);
		}

		private bool HandleClientEventSetFollowedAgent(NetworkCommunicator networkPeer, SetFollowedAgent message)
		{
			networkPeer.GetComponent<MissionPeer>().FollowedAgent = message.Agent;
			return true;
		}

		private bool HandleClientEventSetMachineRotation(NetworkCommunicator networkPeer, SetMachineRotation message)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component.IsControlledAgentActive && message.UsableMachine is RangedSiegeWeapon)
			{
				RangedSiegeWeapon rangedSiegeWeapon = message.UsableMachine as RangedSiegeWeapon;
				if (component.ControlledAgent == rangedSiegeWeapon.PilotAgent && rangedSiegeWeapon.PilotAgent != null)
				{
					rangedSiegeWeapon.AimAtRotation(message.HorizontalRotation, message.VerticalRotation);
				}
			}
			return true;
		}

		private bool HandleClientEventRequestUseObject(NetworkCommunicator networkPeer, RequestUseObject message)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (message.UsableGameObject != null && component.ControlledAgent != null && component.ControlledAgent.IsActive())
			{
				Vec3 position = component.ControlledAgent.Position;
				Vec3 globalPosition = message.UsableGameObject.InteractionEntity.GlobalPosition;
				Vec3 vec;
				Vec3 vec2;
				message.UsableGameObject.InteractionEntity.GetPhysicsMinMax(true, out vec, out vec2, false);
				float num = globalPosition.Distance(vec);
				float num2 = globalPosition.Distance(vec2);
				float num3 = MathF.Max(num, num2);
				float num4 = globalPosition.Distance(new Vec3(position.x, position.y, position.z + component.ControlledAgent.GetEyeGlobalHeight(), -1f));
				num4 -= num3;
				num4 = MathF.Max(num4, 0f);
				if (component.ControlledAgent.CurrentlyUsedGameObject != message.UsableGameObject && component.ControlledAgent.CanReachAndUseObject(message.UsableGameObject, num4 * num4 * 0.9f * 0.9f) && component.ControlledAgent.ObjectHasVacantPosition(message.UsableGameObject))
				{
					component.ControlledAgent.UseGameObject(message.UsableGameObject, message.UsedObjectPreferenceIndex);
				}
			}
			return true;
		}

		private bool HandleClientEventRequestStopUsingObject(NetworkCommunicator networkPeer, RequestStopUsingObject message)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			Agent controlledAgent = component.ControlledAgent;
			if (((controlledAgent != null) ? controlledAgent.CurrentlyUsedGameObject : null) != null)
			{
				component.ControlledAgent.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
			return true;
		}

		private bool HandleClientEventApplyOrder(NetworkCommunicator networkPeer, ApplyOrder message)
		{
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SetOrder(message.OrderType);
			}
			return true;
		}

		private bool HandleClientEventApplySiegeWeaponOrder(NetworkCommunicator networkPeer, ApplySiegeWeaponOrder message)
		{
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SiegeWeaponController.SetOrder(message.OrderType);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithPosition(NetworkCommunicator networkPeer, ApplyOrderWithPosition message)
		{
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				WorldPosition worldPosition = new WorldPosition(base.Mission.Scene, UIntPtr.Zero, message.Position, false);
				orderControllerOfPeer.SetOrderWithPosition(message.OrderType, worldPosition);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithFormation(NetworkCommunicator networkPeer, ApplyOrderWithFormation message)
		{
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			OrderController orderController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent) : null);
			Formation formation = ((teamOfPeer != null) ? teamOfPeer.FormationsIncludingEmpty.SingleOrDefault((Formation f) => f.CountOfUnits > 0 && f.Index == message.FormationIndex) : null);
			if (teamOfPeer != null && orderController != null && formation != null)
			{
				orderController.SetOrderWithFormation(message.OrderType, formation);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithFormationAndPercentage(NetworkCommunicator networkPeer, ApplyOrderWithFormationAndPercentage message)
		{
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			OrderController orderController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent) : null);
			Formation formation = ((teamOfPeer != null) ? teamOfPeer.FormationsIncludingEmpty.SingleOrDefault((Formation f) => f.CountOfUnits > 0 && f.Index == message.FormationIndex) : null);
			float num = (float)message.Percentage * 0.01f;
			if (teamOfPeer != null && orderController != null && formation != null)
			{
				orderController.SetOrderWithFormationAndPercentage(message.OrderType, formation, num);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithFormationAndNumber(NetworkCommunicator networkPeer, ApplyOrderWithFormationAndNumber message)
		{
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			OrderController orderController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent) : null);
			Formation formation = ((teamOfPeer != null) ? teamOfPeer.FormationsIncludingEmpty.SingleOrDefault((Formation f) => f.CountOfUnits > 0 && f.Index == message.FormationIndex) : null);
			int number = message.Number;
			if (teamOfPeer != null && orderController != null && formation != null)
			{
				orderController.SetOrderWithFormationAndNumber(message.OrderType, formation, number);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithTwoPositions(NetworkCommunicator networkPeer, ApplyOrderWithTwoPositions message)
		{
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				WorldPosition worldPosition = new WorldPosition(base.Mission.Scene, UIntPtr.Zero, message.Position1, false);
				WorldPosition worldPosition2 = new WorldPosition(base.Mission.Scene, UIntPtr.Zero, message.Position2, false);
				orderControllerOfPeer.SetOrderWithTwoPositions(message.OrderType, worldPosition, worldPosition2);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithGameEntity(NetworkCommunicator networkPeer, ApplyOrderWithMissionObject message)
		{
			IOrderable orderable = message.MissionObject as IOrderable;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SetOrderWithOrderableObject(orderable);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithAgent(NetworkCommunicator networkPeer, ApplyOrderWithAgent message)
		{
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SetOrderWithAgent(message.OrderType, message.Agent);
			}
			return true;
		}

		private bool HandleClientEventSelectAllFormations(NetworkCommunicator networkPeer, SelectAllFormations message)
		{
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SelectAllFormations(false);
			}
			return true;
		}

		private bool HandleClientEventSelectAllSiegeWeapons(NetworkCommunicator networkPeer, SelectAllSiegeWeapons message)
		{
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SiegeWeaponController.SelectAll();
			}
			return true;
		}

		private bool HandleClientEventClearSelectedFormations(NetworkCommunicator networkPeer, ClearSelectedFormations message)
		{
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.ClearSelectedFormations();
			}
			return true;
		}

		private bool HandleClientEventSelectFormation(NetworkCommunicator networkPeer, SelectFormation message)
		{
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			OrderController orderController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent) : null);
			Formation formation = ((teamOfPeer != null) ? teamOfPeer.FormationsIncludingEmpty.SingleOrDefault((Formation f) => f.Index == message.FormationIndex && f.CountOfUnits > 0) : null);
			if (teamOfPeer != null && orderController != null && formation != null)
			{
				orderController.SelectFormation(formation);
			}
			return true;
		}

		private bool HandleClientEventSelectSiegeWeapon(NetworkCommunicator networkPeer, SelectSiegeWeapon message)
		{
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			SiegeWeaponController siegeWeaponController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent).SiegeWeaponController : null);
			SiegeWeapon siegeWeapon = message.SiegeWeapon;
			if (teamOfPeer != null && siegeWeaponController != null && siegeWeapon != null)
			{
				siegeWeaponController.Select(siegeWeapon);
			}
			return true;
		}

		private bool HandleClientEventUnselectFormation(NetworkCommunicator networkPeer, UnselectFormation message)
		{
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			OrderController orderController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent) : null);
			Formation formation = ((teamOfPeer != null) ? teamOfPeer.FormationsIncludingEmpty.SingleOrDefault((Formation f) => f.CountOfUnits > 0 && f.Index == message.FormationIndex) : null);
			if (teamOfPeer != null && orderController != null && formation != null)
			{
				orderController.DeselectFormation(formation);
			}
			return true;
		}

		private bool HandleClientEventUnselectSiegeWeapon(NetworkCommunicator networkPeer, UnselectSiegeWeapon message)
		{
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			SiegeWeaponController siegeWeaponController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent).SiegeWeaponController : null);
			SiegeWeapon siegeWeapon = message.SiegeWeapon;
			if (teamOfPeer != null && siegeWeaponController != null && siegeWeapon != null)
			{
				siegeWeaponController.Deselect(siegeWeapon);
			}
			return true;
		}

		private bool HandleClientEventDropWeapon(NetworkCommunicator networkPeer, DropWeapon message)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (((component != null) ? component.ControlledAgent : null) != null && component.ControlledAgent.IsActive())
			{
				component.ControlledAgent.HandleDropWeapon(message.IsDefendPressed, message.ForcedSlotIndexToDropWeaponFrom);
			}
			return true;
		}

		private bool HandleClientEventCancelCheering(NetworkCommunicator networkPeer, CancelCheering message)
		{
			bool flag = false;
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.CancelCheering();
				flag = true;
			}
			return flag;
		}

		private bool HandleClientEventCheerSelected(NetworkCommunicator networkPeer, CheerSelected message)
		{
			bool flag = false;
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.HandleCheer(message.IndexOfCheer);
				flag = true;
			}
			return flag;
		}

		private bool HandleClientEventBarkSelected(NetworkCommunicator networkPeer, BarkSelected message)
		{
			bool flag = false;
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.HandleBark(message.IndexOfBark);
				flag = true;
			}
			return flag;
		}

		private bool HandleClientEventBreakAgentVisualsInvulnerability(NetworkCommunicator networkPeer, AgentVisualsBreakInvulnerability message)
		{
			if (base.Mission == null || base.Mission.GetMissionBehavior<SpawnComponent>() == null || networkPeer.GetComponent<MissionPeer>() == null)
			{
				return false;
			}
			base.Mission.GetMissionBehavior<SpawnComponent>().SetEarlyAgentVisualsDespawning(networkPeer.GetComponent<MissionPeer>(), true);
			return true;
		}

		private bool HandleClientEventRequestToSpawnAsBot(NetworkCommunicator networkPeer, RequestToSpawnAsBot message)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component == null)
			{
				return false;
			}
			if (component.HasSpawnTimerExpired)
			{
				component.WantsToSpawnAsBot = true;
			}
			return true;
		}

		private void SendExistingObjectsToPeer(NetworkCommunicator networkPeer)
		{
			MBDebug.Print(string.Concat(new object[] { "Sending all existing objects to peer: ", networkPeer.UserName, " with index: ", networkPeer.Index }), 0, Debug.DebugColor.White, 17179869184UL);
			GameNetwork.BeginModuleEventAsServer(networkPeer);
			GameNetwork.WriteMessage(new ExistingObjectsBegin());
			GameNetwork.EndModuleEventAsServer();
			GameNetwork.BeginModuleEventAsServer(networkPeer);
			GameNetwork.WriteMessage(new SynchronizeMissionTimeTracker((float)MissionTime.Now.ToSeconds));
			GameNetwork.EndModuleEventAsServer();
			this.SendTeamsToPeer(networkPeer);
			this.SendTeamRelationsToPeer(networkPeer);
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeersIncludingDisconnectedPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (component != null)
				{
					if (component.Team != null)
					{
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						GameNetwork.WriteMessage(new SetPeerTeam(networkCommunicator, component.Team));
						GameNetwork.EndModuleEventAsServer();
					}
					if (component.Culture != null)
					{
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						GameNetwork.WriteMessage(new ChangeCulture(component, component.Culture));
						GameNetwork.EndModuleEventAsServer();
					}
				}
			}
			this.SendFormationInformation(networkPeer);
			this.SendAgentsToPeer(networkPeer);
			this.SendSpawnedMissionObjectsToPeer(networkPeer);
			this.SynchronizeMissionObjectsToPeer(networkPeer);
			this.SendMissilesToPeer(networkPeer);
			this.SendTroopSelectionInformation(networkPeer);
			networkPeer.SendExistingObjects(base.Mission);
			GameNetwork.BeginModuleEventAsServer(networkPeer);
			GameNetwork.WriteMessage(new ExistingObjectsEnd());
			GameNetwork.EndModuleEventAsServer();
		}

		private void SendTroopSelectionInformation(NetworkCommunicator networkPeer)
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeersIncludingDisconnectedPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (component != null && component.SelectedTroopIndex != 0)
				{
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					GameNetwork.WriteMessage(new UpdateSelectedTroopIndex(networkCommunicator, component.SelectedTroopIndex));
					GameNetwork.EndModuleEventAsServer();
				}
			}
		}

		private void SendTeamsToPeer(NetworkCommunicator networkPeer)
		{
			foreach (Team team in base.Mission.Teams)
			{
				MBDebug.Print(string.Concat(new object[] { "Syncing a team to peer: ", networkPeer.UserName, " with index: ", networkPeer.Index }), 0, Debug.DebugColor.White, 17179869184UL);
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new AddTeam(team));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		private void SendTeamRelationsToPeer(NetworkCommunicator networkPeer)
		{
			int count = base.Mission.Teams.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = i; j < count; j++)
				{
					Team team = base.Mission.Teams[i];
					Team team2 = base.Mission.Teams[j];
					if (team.IsEnemyOf(team2))
					{
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						GameNetwork.WriteMessage(new TeamSetIsEnemyOf(team, team2, true));
						GameNetwork.EndModuleEventAsServer();
					}
				}
			}
		}

		private void SendFormationInformation(NetworkCommunicator networkPeer)
		{
			MBDebug.Print("formations sending begin-", 0, Debug.DebugColor.White, 17179869184UL);
			foreach (Team team in base.Mission.Teams)
			{
				if (team.IsValid && team.Side != BattleSideEnum.None)
				{
					foreach (Formation formation in team.FormationsIncludingEmpty)
					{
						if (!string.IsNullOrEmpty(formation.BannerCode))
						{
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new InitializeFormation(formation, team, formation.BannerCode));
							GameNetwork.EndModuleEventAsServer();
						}
					}
				}
			}
			if (!networkPeer.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new SetSpawnedFormationCount(base.Mission.NumOfFormationsSpawnedTeamOne, base.Mission.NumOfFormationsSpawnedTeamTwo));
				GameNetwork.EndModuleEventAsServer();
			}
			MBDebug.Print("formations sending end-", 0, Debug.DebugColor.White, 17179869184UL);
		}

		private void SendAgentVisualsToPeer(NetworkCommunicator networkPeer, Team peerTeam)
		{
			MBDebug.Print("agentvisuals sending begin-", 0, Debug.DebugColor.White, 17179869184UL);
			foreach (MissionPeer missionPeer in from p in GameNetwork.NetworkPeers
				select p.GetComponent<MissionPeer>() into pr
				where pr != null
				select pr)
			{
				if (missionPeer.Team == peerTeam)
				{
					int amountOfAgentVisualsForPeer = missionPeer.GetAmountOfAgentVisualsForPeer();
					for (int i = 0; i < amountOfAgentVisualsForPeer; i++)
					{
						PeerVisualsHolder visuals = missionPeer.GetVisuals(i);
						IAgentVisual agentVisuals = visuals.AgentVisuals;
						MatrixFrame frame = agentVisuals.GetFrame();
						AgentBuildData agentBuildData = new AgentBuildData(MBObjectManager.Instance.GetObject<BasicCharacterObject>(agentVisuals.GetCharacterObjectID())).MissionPeer(missionPeer).Equipment(agentVisuals.GetEquipment()).VisualsIndex(visuals.VisualsIndex)
							.Team(missionPeer.Team)
							.InitialPosition(frame.origin);
						Vec2 vec = frame.rotation.f.AsVec2;
						vec = vec.Normalized();
						AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(vec).IsFemale(agentVisuals.GetIsFemale()).BodyProperties(agentVisuals.GetBodyProperties());
						networkPeer.GetComponent<MissionPeer>();
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						GameNetwork.WriteMessage(new CreateAgentVisuals(missionPeer.GetNetworkPeer(), agentBuildData2, missionPeer.SelectedTroopIndex, 0));
						GameNetwork.EndModuleEventAsServer();
					}
				}
			}
			MBDebug.Print("agentvisuals sending end-", 0, Debug.DebugColor.White, 17179869184UL);
		}

		private void SendAgentsToPeer(NetworkCommunicator networkPeer)
		{
			MBDebug.Print("agents sending begin-", 0, Debug.DebugColor.White, 17179869184UL);
			using (List<Agent>.Enumerator enumerator = base.Mission.AllAgents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Agent agent = enumerator.Current;
					bool isMount = agent.IsMount;
					AgentState state = agent.State;
					if (state == AgentState.Active || ((state == AgentState.Killed || state == AgentState.Unconscious) && (agent.GetAttachedWeaponsCount() > 0 || (!isMount && (agent.GetWieldedItemIndex(Agent.HandIndex.MainHand) >= EquipmentIndex.WeaponItemBeginSlot || agent.GetWieldedItemIndex(Agent.HandIndex.OffHand) >= EquipmentIndex.WeaponItemBeginSlot)) || base.Mission.IsAgentInProximityMap(agent))) || (state != AgentState.Active && base.Mission.Missiles.Any((Mission.Missile m) => m.ShooterAgent == agent)))
					{
						if (isMount && agent.RiderAgent == null)
						{
							MBDebug.Print("mount sending " + agent.Index, 0, Debug.DebugColor.White, 17179869184UL);
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new CreateFreeMountAgent(agent, agent.Position, agent.GetMovementDirection()));
							GameNetwork.EndModuleEventAsServer();
							agent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
							int attachedWeaponsCount = agent.GetAttachedWeaponsCount();
							for (int i = 0; i < attachedWeaponsCount; i++)
							{
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new AttachWeaponToAgent(agent.GetAttachedWeapon(i), agent, agent.GetAttachedWeaponBoneIndex(i), agent.GetAttachedWeaponFrame(i)));
								GameNetwork.EndModuleEventAsServer();
							}
							if (!agent.IsActive())
							{
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new MakeAgentDead(agent, state == AgentState.Killed, agent.GetCurrentActionValue(0)));
								GameNetwork.EndModuleEventAsServer();
							}
						}
						else if (!isMount)
						{
							MBDebug.Print("human sending " + agent.Index, 0, Debug.DebugColor.White, 17179869184UL);
							Agent agent3 = agent.MountAgent;
							if (agent3 != null && agent3.RiderAgent == null)
							{
								agent3 = null;
							}
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							Agent agent2 = agent;
							bool flag = agent.MissionPeer != null && agent.OwningAgentMissionPeer == null;
							Vec3 position = agent.Position;
							Vec2 movementDirection = agent.GetMovementDirection();
							MissionPeer missionPeer = agent.MissionPeer;
							NetworkCommunicator networkCommunicator;
							if ((networkCommunicator = ((missionPeer != null) ? missionPeer.GetNetworkPeer() : null)) == null)
							{
								MissionPeer owningAgentMissionPeer = agent.OwningAgentMissionPeer;
								networkCommunicator = ((owningAgentMissionPeer != null) ? owningAgentMissionPeer.GetNetworkPeer() : null);
							}
							GameNetwork.WriteMessage(new CreateAgent(agent2, flag, position, movementDirection, networkCommunicator));
							GameNetwork.EndModuleEventAsServer();
							agent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
							if (agent3 != null)
							{
								agent3.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
							}
							for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
							{
								for (int j = 0; j < agent.Equipment[equipmentIndex].GetAttachedWeaponsCount(); j++)
								{
									GameNetwork.BeginModuleEventAsServer(networkPeer);
									GameNetwork.WriteMessage(new AttachWeaponToWeaponInAgentEquipmentSlot(agent.Equipment[equipmentIndex].GetAttachedWeapon(j), agent, equipmentIndex, agent.Equipment[equipmentIndex].GetAttachedWeaponFrame(j)));
									GameNetwork.EndModuleEventAsServer();
								}
							}
							int num = agent.GetAttachedWeaponsCount();
							for (int k = 0; k < num; k++)
							{
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new AttachWeaponToAgent(agent.GetAttachedWeapon(k), agent, agent.GetAttachedWeaponBoneIndex(k), agent.GetAttachedWeaponFrame(k)));
								GameNetwork.EndModuleEventAsServer();
							}
							if (agent3 != null)
							{
								num = agent3.GetAttachedWeaponsCount();
								for (int l = 0; l < num; l++)
								{
									GameNetwork.BeginModuleEventAsServer(networkPeer);
									GameNetwork.WriteMessage(new AttachWeaponToAgent(agent3.GetAttachedWeapon(l), agent3, agent3.GetAttachedWeaponBoneIndex(l), agent3.GetAttachedWeaponFrame(l)));
									GameNetwork.EndModuleEventAsServer();
								}
							}
							EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
							int num2 = ((wieldedItemIndex != EquipmentIndex.None) ? agent.Equipment[wieldedItemIndex].CurrentUsageIndex : 0);
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new SetWieldedItemIndex(agent, false, true, true, wieldedItemIndex, num2));
							GameNetwork.EndModuleEventAsServer();
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new SetWieldedItemIndex(agent, true, true, true, agent.GetWieldedItemIndex(Agent.HandIndex.OffHand), num2));
							GameNetwork.EndModuleEventAsServer();
							MBActionSet actionSet = agent.ActionSet;
							if (actionSet.IsValid)
							{
								AnimationSystemData animationSystemData = agent.Monster.FillAnimationSystemData(actionSet, agent.Character.GetStepSize(), false);
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new SetAgentActionSet(agent, animationSystemData));
								GameNetwork.EndModuleEventAsServer();
								if (!agent.IsActive())
								{
									GameNetwork.BeginModuleEventAsServer(networkPeer);
									GameNetwork.WriteMessage(new MakeAgentDead(agent, state == AgentState.Killed, agent.GetCurrentActionValue(0)));
									GameNetwork.EndModuleEventAsServer();
								}
							}
							else if (!agent.IsActive())
							{
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new MakeAgentDead(agent, state == AgentState.Killed, ActionIndexValueCache.act_none));
								GameNetwork.EndModuleEventAsServer();
							}
						}
						else
						{
							MBDebug.Print("agent not sending " + agent.Index, 0, Debug.DebugColor.White, 17179869184UL);
						}
					}
				}
			}
			MBDebug.Print("agents sending end-", 0, Debug.DebugColor.White, 17179869184UL);
		}

		private void SendSpawnedMissionObjectsToPeer(NetworkCommunicator networkPeer)
		{
			using (List<MissionObject>.Enumerator enumerator = base.Mission.MissionObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MissionObject missionObject = enumerator.Current;
					SpawnedItemEntity spawnedItemEntity;
					if ((spawnedItemEntity = missionObject as SpawnedItemEntity) != null)
					{
						GameEntity gameEntity = spawnedItemEntity.GameEntity;
						if (gameEntity.Parent == null || !gameEntity.Parent.HasScriptOfType<SpawnedItemEntity>())
						{
							MissionObject missionObject2 = null;
							if (spawnedItemEntity.GameEntity.Parent != null)
							{
								missionObject2 = gameEntity.Parent.GetFirstScriptOfType<MissionObject>();
							}
							MatrixFrame matrixFrame = gameEntity.GetGlobalFrame();
							if (missionObject2 != null)
							{
								matrixFrame = missionObject2.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref matrixFrame);
							}
							matrixFrame.origin.z = MathF.Max(matrixFrame.origin.z, CompressionBasic.PositionCompressionInfo.GetMinimumValue() + 1f);
							Mission.WeaponSpawnFlags weaponSpawnFlags = spawnedItemEntity.SpawnFlags;
							if (weaponSpawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithPhysics) && !gameEntity.GetPhysicsState())
							{
								weaponSpawnFlags = (weaponSpawnFlags & ~Mission.WeaponSpawnFlags.WithPhysics) | Mission.WeaponSpawnFlags.WithStaticPhysics;
							}
							bool flag = true;
							bool flag2 = gameEntity.Parent == null || missionObject2 != null;
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new SpawnWeaponWithNewEntity(spawnedItemEntity.WeaponCopy, weaponSpawnFlags, spawnedItemEntity.Id.Id, matrixFrame, missionObject2, flag2, flag));
							GameNetwork.EndModuleEventAsServer();
							for (int i = 0; i < spawnedItemEntity.WeaponCopy.GetAttachedWeaponsCount(); i++)
							{
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new AttachWeaponToSpawnedWeapon(spawnedItemEntity.WeaponCopy.GetAttachedWeapon(i), spawnedItemEntity, spawnedItemEntity.WeaponCopy.GetAttachedWeaponFrame(i)));
								GameNetwork.EndModuleEventAsServer();
								if (spawnedItemEntity.WeaponCopy.GetAttachedWeapon(i).Item.ItemFlags.HasAnyFlag(ItemFlags.CanBePickedUpFromCorpse))
								{
									if (gameEntity.GetChild(i) == null)
									{
										Debug.Print(string.Concat(new object[]
										{
											"spawnedItemGameEntity child is null. item: ",
											spawnedItemEntity.WeaponCopy.Item.StringId,
											" attached item: ",
											spawnedItemEntity.WeaponCopy.GetAttachedWeapon(i).Item.StringId,
											" attachment index: ",
											i
										}), 0, Debug.DebugColor.White, 17592186044416UL);
									}
									else if (gameEntity.GetChild(i).GetFirstScriptOfType<SpawnedItemEntity>() == null)
									{
										Debug.Print(string.Concat(new object[]
										{
											"spawnedItemGameEntity child SpawnedItemEntity script is null. item: ",
											spawnedItemEntity.WeaponCopy.Item.StringId,
											" attached item: ",
											spawnedItemEntity.WeaponCopy.GetAttachedWeapon(i).Item.StringId,
											" attachment index: ",
											i
										}), 0, Debug.DebugColor.White, 17592186044416UL);
									}
									GameNetwork.BeginModuleEventAsServer(networkPeer);
									GameNetwork.WriteMessage(new SpawnAttachedWeaponOnSpawnedWeapon(spawnedItemEntity, i, gameEntity.GetChild(i).GetFirstScriptOfType<SpawnedItemEntity>().Id.Id));
									GameNetwork.EndModuleEventAsServer();
								}
							}
						}
					}
					else if (missionObject.CreatedAtRuntime)
					{
						Mission.DynamicallyCreatedEntity dynamicallyCreatedEntity = base.Mission.AddedEntitiesInfo.SingleOrDefault((Mission.DynamicallyCreatedEntity x) => x.ObjectId == missionObject.Id);
						if (dynamicallyCreatedEntity != null)
						{
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new CreateMissionObject(dynamicallyCreatedEntity.ObjectId, dynamicallyCreatedEntity.Prefab, dynamicallyCreatedEntity.Frame, dynamicallyCreatedEntity.ChildObjectIds));
							GameNetwork.EndModuleEventAsServer();
						}
					}
				}
			}
		}

		private void SynchronizeMissionObjectsToPeer(NetworkCommunicator networkPeer)
		{
			using (List<MissionObject>.Enumerator enumerator = base.Mission.MissionObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SynchedMissionObject synchedMissionObject;
					if ((synchedMissionObject = enumerator.Current as SynchedMissionObject) != null)
					{
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						GameNetwork.WriteMessage(new SynchronizeMissionObject(synchedMissionObject));
						GameNetwork.EndModuleEventAsServer();
					}
				}
			}
		}

		private void SendMissilesToPeer(NetworkCommunicator networkPeer)
		{
			foreach (Mission.Missile missile in base.Mission.Missiles)
			{
				Vec3 velocity = missile.GetVelocity();
				float num = velocity.Normalize();
				Mat3 identity = Mat3.Identity;
				identity.f = velocity;
				identity.Orthonormalize();
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new CreateMissile(missile.Index, missile.ShooterAgent, EquipmentIndex.None, missile.Weapon, missile.GetPosition(), velocity, num, identity, missile.GetHasRigidBody(), missile.MissionObjectToIgnore, false));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		public override void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component != null && component.HasSpawnedAgentVisuals)
			{
				base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, false);
			}
		}

		protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			if (!networkPeer.IsServerPeer)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					if (networkCommunicator.IsSynchronized || networkCommunicator.JustReconnecting)
					{
						networkCommunicator.VirtualPlayer.SynchronizeComponentsTo(networkPeer.VirtualPlayer);
					}
				}
				foreach (ICommunicator communicator in MBNetwork.DisconnectedNetworkPeers)
				{
					communicator.VirtualPlayer.SynchronizeComponentsTo(networkPeer.VirtualPlayer);
				}
			}
			MissionPeer missionPeer = networkPeer.AddComponent<MissionPeer>();
			if (networkPeer.JustReconnecting && missionPeer.Team != null)
			{
				MBAPI.IMBPeer.SetTeam(networkPeer.Index, missionPeer.Team.MBTeam.Index);
			}
			missionPeer.JoinTime = DateTime.Now;
		}

		protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			if (!networkPeer.IsServerPeer)
			{
				this.SendExistingObjectsToPeer(networkPeer);
			}
		}

		protected override void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component != null)
			{
				Mission mission = base.Mission;
				if (mission == null)
				{
					return;
				}
				mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, true);
			}
		}

		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
		}

		protected override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component != null)
			{
				if (component.ControlledAgent != null)
				{
					Agent controlledAgent = component.ControlledAgent;
					Blow blow = new Blow(controlledAgent.Index);
					blow.WeaponRecord = default(BlowWeaponRecord);
					blow.DamageType = DamageTypes.Invalid;
					blow.BaseMagnitude = 10000f;
					blow.WeaponRecord.WeaponClass = WeaponClass.Undefined;
					blow.Position = controlledAgent.Position;
					blow.DamagedPercentage = 1f;
					controlledAgent.Die(blow, Agent.KillInfo.Invalid);
				}
				if (base.Mission.AllAgents != null)
				{
					foreach (Agent agent in base.Mission.AllAgents)
					{
						if (agent.MissionPeer == component)
						{
							agent.MissionPeer = null;
						}
						if (agent.OwningAgentMissionPeer == component)
						{
							agent.OwningAgentMissionPeer = null;
						}
					}
				}
				if (component.ControlledFormation != null)
				{
					component.ControlledFormation.PlayerOwner = null;
				}
			}
		}

		public override void OnAddTeam(Team team)
		{
			base.OnAddTeam(team);
			if (GameNetwork.IsServerOrRecorder)
			{
				MBDebug.Print("----------OnAddTeam-", 0, Debug.DebugColor.White, 17592186044416UL);
				MBDebug.Print("Adding a team and sending it to all clients", 0, Debug.DebugColor.White, 17179869184UL);
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new AddTeam(team));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				return;
			}
			if (team.Side != BattleSideEnum.Attacker && team.Side != BattleSideEnum.Defender && base.Mission.SpectatorTeam == null)
			{
				base.Mission.SpectatorTeam = team;
			}
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._chatBox = Game.Current.GetGameHandler<ChatBox>();
		}

		public override void OnClearScene()
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				MBDebug.Print("I am clearing the scene, and sending this message to all clients", 0, Debug.DebugColor.White, 17179869184UL);
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new ClearMission());
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		public override void OnMissionTick(float dt)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				this._accumulatedTimeSinceLastTimerSync += dt;
				if (this._accumulatedTimeSinceLastTimerSync > 2f)
				{
					this._accumulatedTimeSinceLastTimerSync -= 2f;
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SynchronizeMissionTimeTracker((float)MissionTime.Now.ToSeconds));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
			}
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionRepresentativeBase component = networkCommunicator.GetComponent<MissionRepresentativeBase>();
				if (component != null)
				{
					component.Tick(dt);
				}
				if (GameNetwork.IsServer)
				{
					MissionPeer component2 = networkCommunicator.GetComponent<MissionPeer>();
					if (component2 != null)
					{
						component2.TickInactivityStatus();
					}
				}
			}
		}

		protected override void OnEndMission()
		{
			if (GameNetwork.IsServer)
			{
				foreach (MissionPeer missionPeer in VirtualPlayer.Peers<MissionPeer>())
				{
					missionPeer.ControlledAgent = null;
				}
				foreach (Agent agent in base.Mission.AllAgents)
				{
					agent.MissionPeer = null;
				}
			}
			base.OnEndMission();
		}

		public void OnPeerSelectedTeam(MissionPeer missionPeer)
		{
			this.SendAgentVisualsToPeer(missionPeer.GetNetworkPeer(), missionPeer.Team);
		}

		public void OnClientSynchronized(NetworkCommunicator networkPeer)
		{
			Action<NetworkCommunicator> onClientSynchronizedEvent = this.OnClientSynchronizedEvent;
			if (onClientSynchronizedEvent != null)
			{
				onClientSynchronizedEvent(networkPeer);
			}
			if (networkPeer.IsMine)
			{
				Action onMyClientSynchronized = this.OnMyClientSynchronized;
				if (onMyClientSynchronized == null)
				{
					return;
				}
				onMyClientSynchronized();
			}
		}

		private float _accumulatedTimeSinceLastTimerSync;

		private const float TimerSyncPeriod = 2f;

		private ChatBox _chatBox;
	}
}
