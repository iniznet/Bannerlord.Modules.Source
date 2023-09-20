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
				registerer.RegisterBaseHandler<CreateFreeMountAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventCreateFreeMountAgentEvent));
				registerer.RegisterBaseHandler<CreateAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventCreateAgent));
				registerer.RegisterBaseHandler<SynchronizeAgentSpawnEquipment>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSynchronizeAgentEquipment));
				registerer.RegisterBaseHandler<CreateAgentVisuals>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventCreateAgentVisuals));
				registerer.RegisterBaseHandler<RemoveAgentVisualsForPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventRemoveAgentVisualsForPeer));
				registerer.RegisterBaseHandler<RemoveAgentVisualsFromIndexForPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventRemoveAgentVisualsFromIndexForPeer));
				registerer.RegisterBaseHandler<ReplaceBotWithPlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventReplaceBotWithPlayer));
				registerer.RegisterBaseHandler<SetWieldedItemIndex>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetWieldedItemIndex));
				registerer.RegisterBaseHandler<SetWeaponNetworkData>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetWeaponNetworkData));
				registerer.RegisterBaseHandler<SetWeaponAmmoData>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetWeaponAmmoData));
				registerer.RegisterBaseHandler<SetWeaponReloadPhase>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetWeaponReloadPhase));
				registerer.RegisterBaseHandler<WeaponUsageIndexChangeMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventWeaponUsageIndexChangeMessage));
				registerer.RegisterBaseHandler<StartSwitchingWeaponUsageIndex>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventStartSwitchingWeaponUsageIndex));
				registerer.RegisterBaseHandler<InitializeFormation>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventInitializeFormation));
				registerer.RegisterBaseHandler<SetSpawnedFormationCount>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetSpawnedFormationCount));
				registerer.RegisterBaseHandler<AddTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAddTeam));
				registerer.RegisterBaseHandler<TeamSetIsEnemyOf>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventTeamSetIsEnemyOf));
				registerer.RegisterBaseHandler<AssignFormationToPlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAssignFormationToPlayer));
				registerer.RegisterBaseHandler<ExistingObjectsBegin>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventExistingObjectsBegin));
				registerer.RegisterBaseHandler<ExistingObjectsEnd>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventExistingObjectsEnd));
				registerer.RegisterBaseHandler<ClearMission>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventClearMission));
				registerer.RegisterBaseHandler<CreateMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventCreateMissionObject));
				registerer.RegisterBaseHandler<RemoveMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventRemoveMissionObject));
				registerer.RegisterBaseHandler<StopPhysicsAndSetFrameOfMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventStopPhysicsAndSetFrameOfMissionObject));
				registerer.RegisterBaseHandler<BurstMissionObjectParticles>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventBurstMissionObjectParticles));
				registerer.RegisterBaseHandler<SetMissionObjectVisibility>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectVisibility));
				registerer.RegisterBaseHandler<SetMissionObjectDisabled>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectDisabled));
				registerer.RegisterBaseHandler<SetMissionObjectColors>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectColors));
				registerer.RegisterBaseHandler<SetMissionObjectFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectFrame));
				registerer.RegisterBaseHandler<SetMissionObjectGlobalFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectGlobalFrame));
				registerer.RegisterBaseHandler<SetMissionObjectFrameOverTime>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectFrameOverTime));
				registerer.RegisterBaseHandler<SetMissionObjectGlobalFrameOverTime>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectGlobalFrameOverTime));
				registerer.RegisterBaseHandler<SetMissionObjectAnimationAtChannel>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectAnimationAtChannel));
				registerer.RegisterBaseHandler<SetMissionObjectAnimationChannelParameter>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectAnimationChannelParameter));
				registerer.RegisterBaseHandler<SetMissionObjectAnimationPaused>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectAnimationPaused));
				registerer.RegisterBaseHandler<SetMissionObjectVertexAnimation>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectVertexAnimation));
				registerer.RegisterBaseHandler<SetMissionObjectVertexAnimationProgress>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectVertexAnimationProgress));
				registerer.RegisterBaseHandler<SetMissionObjectImpulse>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMissionObjectImpulse));
				registerer.RegisterBaseHandler<AddMissionObjectBodyFlags>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAddMissionObjectBodyFlags));
				registerer.RegisterBaseHandler<RemoveMissionObjectBodyFlags>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventRemoveMissionObjectBodyFlags));
				registerer.RegisterBaseHandler<SetMachineTargetRotation>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetMachineTargetRotation));
				registerer.RegisterBaseHandler<SetUsableMissionObjectIsDeactivated>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetUsableGameObjectIsDeactivated));
				registerer.RegisterBaseHandler<SetUsableMissionObjectIsDisabledForPlayers>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetUsableGameObjectIsDisabledForPlayers));
				registerer.RegisterBaseHandler<SetRangedSiegeWeaponState>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetRangedSiegeWeaponState));
				registerer.RegisterBaseHandler<SetRangedSiegeWeaponAmmo>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetRangedSiegeWeaponAmmo));
				registerer.RegisterBaseHandler<RangedSiegeWeaponChangeProjectile>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventRangedSiegeWeaponChangeProjectile));
				registerer.RegisterBaseHandler<SetStonePileAmmo>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetStonePileAmmo));
				registerer.RegisterBaseHandler<SetSiegeMachineMovementDistance>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetSiegeMachineMovementDistance));
				registerer.RegisterBaseHandler<SetSiegeLadderState>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetSiegeLadderState));
				registerer.RegisterBaseHandler<SetAgentTargetPositionAndDirection>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetAgentTargetPositionAndDirection));
				registerer.RegisterBaseHandler<SetAgentTargetPosition>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetAgentTargetPosition));
				registerer.RegisterBaseHandler<ClearAgentTargetFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventClearAgentTargetFrame));
				registerer.RegisterBaseHandler<AgentTeleportToFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAgentTeleportToFrame));
				registerer.RegisterBaseHandler<SetSiegeTowerGateState>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetSiegeTowerGateState));
				registerer.RegisterBaseHandler<SetSiegeTowerHasArrivedAtTarget>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetSiegeTowerHasArrivedAtTarget));
				registerer.RegisterBaseHandler<SetBatteringRamHasArrivedAtTarget>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetBatteringRamHasArrivedAtTarget));
				registerer.RegisterBaseHandler<SetPeerTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetPeerTeam));
				registerer.RegisterBaseHandler<SynchronizeMissionTimeTracker>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSyncMissionTimer));
				registerer.RegisterBaseHandler<SetAgentPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetAgentPeer));
				registerer.RegisterBaseHandler<SetAgentIsPlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetAgentIsPlayer));
				registerer.RegisterBaseHandler<SetAgentHealth>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetAgentHealth));
				registerer.RegisterBaseHandler<AgentSetTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAgentSetTeam));
				registerer.RegisterBaseHandler<SetAgentActionSet>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetAgentActionSet));
				registerer.RegisterBaseHandler<MakeAgentDead>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventMakeAgentDead));
				registerer.RegisterBaseHandler<AgentSetFormation>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAgentSetFormation));
				registerer.RegisterBaseHandler<AddPrefabComponentToAgentBone>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAddPrefabComponentToAgentBone));
				registerer.RegisterBaseHandler<SetAgentPrefabComponentVisibility>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSetAgentPrefabComponentVisibility));
				registerer.RegisterBaseHandler<UseObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventUseObject));
				registerer.RegisterBaseHandler<StopUsingObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventStopUsingObject));
				registerer.RegisterBaseHandler<SyncObjectHitpoints>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventHitSynchronizeObjectHitpoints));
				registerer.RegisterBaseHandler<SyncObjectDestructionLevel>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventHitSynchronizeObjectDestructionLevel));
				registerer.RegisterBaseHandler<BurstAllHeavyHitParticles>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventHitBurstAllHeavyHitParticles));
				registerer.RegisterBaseHandler<SynchronizeMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSynchronizeMissionObject));
				registerer.RegisterBaseHandler<SpawnWeaponWithNewEntity>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSpawnWeaponWithNewEntity));
				registerer.RegisterBaseHandler<AttachWeaponToSpawnedWeapon>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAttachWeaponToSpawnedWeapon));
				registerer.RegisterBaseHandler<AttachWeaponToAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAttachWeaponToAgent));
				registerer.RegisterBaseHandler<SpawnWeaponAsDropFromAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSpawnWeaponAsDropFromAgent));
				registerer.RegisterBaseHandler<SpawnAttachedWeaponOnSpawnedWeapon>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSpawnAttachedWeaponOnSpawnedWeapon));
				registerer.RegisterBaseHandler<SpawnAttachedWeaponOnCorpse>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSpawnAttachedWeaponOnCorpse));
				registerer.RegisterBaseHandler<HandleMissileCollisionReaction>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventHandleMissileCollisionReaction));
				registerer.RegisterBaseHandler<RemoveEquippedWeapon>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventRemoveEquippedWeapon));
				registerer.RegisterBaseHandler<BarkAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventBarkAgent));
				registerer.RegisterBaseHandler<EquipWeaponWithNewEntity>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventEquipWeaponWithNewEntity));
				registerer.RegisterBaseHandler<AttachWeaponToWeaponInAgentEquipmentSlot>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAttachWeaponToWeaponInAgentEquipmentSlot));
				registerer.RegisterBaseHandler<EquipWeaponFromSpawnedItemEntity>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventEquipWeaponFromSpawnedItemEntity));
				registerer.RegisterBaseHandler<CreateMissile>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventCreateMissile));
				registerer.RegisterBaseHandler<CombatLogNetworkMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAgentHit));
				registerer.RegisterBaseHandler<ConsumeWeaponAmount>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventConsumeWeaponAmount));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.RegisterBaseHandler<SetFollowedAgent>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventSetFollowedAgent));
				registerer.RegisterBaseHandler<SetMachineRotation>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventSetMachineRotation));
				registerer.RegisterBaseHandler<RequestUseObject>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventRequestUseObject));
				registerer.RegisterBaseHandler<RequestStopUsingObject>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventRequestStopUsingObject));
				registerer.RegisterBaseHandler<ApplyOrder>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventApplyOrder));
				registerer.RegisterBaseHandler<ApplySiegeWeaponOrder>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventApplySiegeWeaponOrder));
				registerer.RegisterBaseHandler<ApplyOrderWithPosition>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventApplyOrderWithPosition));
				registerer.RegisterBaseHandler<ApplyOrderWithFormation>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventApplyOrderWithFormation));
				registerer.RegisterBaseHandler<ApplyOrderWithFormationAndPercentage>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventApplyOrderWithFormationAndPercentage));
				registerer.RegisterBaseHandler<ApplyOrderWithFormationAndNumber>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventApplyOrderWithFormationAndNumber));
				registerer.RegisterBaseHandler<ApplyOrderWithTwoPositions>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventApplyOrderWithTwoPositions));
				registerer.RegisterBaseHandler<ApplyOrderWithMissionObject>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventApplyOrderWithGameEntity));
				registerer.RegisterBaseHandler<ApplyOrderWithAgent>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventApplyOrderWithAgent));
				registerer.RegisterBaseHandler<SelectAllFormations>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventSelectAllFormations));
				registerer.RegisterBaseHandler<SelectAllSiegeWeapons>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventSelectAllSiegeWeapons));
				registerer.RegisterBaseHandler<ClearSelectedFormations>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventClearSelectedFormations));
				registerer.RegisterBaseHandler<SelectFormation>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventSelectFormation));
				registerer.RegisterBaseHandler<SelectSiegeWeapon>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventSelectSiegeWeapon));
				registerer.RegisterBaseHandler<UnselectFormation>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventUnselectFormation));
				registerer.RegisterBaseHandler<UnselectSiegeWeapon>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventUnselectSiegeWeapon));
				registerer.RegisterBaseHandler<DropWeapon>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventDropWeapon));
				registerer.RegisterBaseHandler<TauntSelected>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventCheerSelected));
				registerer.RegisterBaseHandler<BarkSelected>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventBarkSelected));
				registerer.RegisterBaseHandler<AgentVisualsBreakInvulnerability>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventBreakAgentVisualsInvulnerability));
				registerer.RegisterBaseHandler<RequestToSpawnAsBot>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventRequestToSpawnAsBot));
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

		private void HandleServerEventSyncMissionTimer(GameNetworkMessage baseMessage)
		{
			SynchronizeMissionTimeTracker synchronizeMissionTimeTracker = (SynchronizeMissionTimeTracker)baseMessage;
			base.Mission.MissionTimeTracker.UpdateSync(synchronizeMissionTimeTracker.CurrentTime);
		}

		private void HandleServerEventSetPeerTeam(GameNetworkMessage baseMessage)
		{
			SetPeerTeam setPeerTeam = (SetPeerTeam)baseMessage;
			MissionPeer component = setPeerTeam.Peer.GetComponent<MissionPeer>();
			component.Team = Mission.MissionNetworkHelper.GetTeamFromTeamIndex(setPeerTeam.TeamIndex);
			if (setPeerTeam.Peer.IsMine)
			{
				base.Mission.PlayerTeam = component.Team;
			}
		}

		private void HandleServerEventCreateFreeMountAgentEvent(GameNetworkMessage baseMessage)
		{
			CreateFreeMountAgent createFreeMountAgent = (CreateFreeMountAgent)baseMessage;
			Mission mission = base.Mission;
			EquipmentElement horseItem = createFreeMountAgent.HorseItem;
			EquipmentElement horseHarnessItem = createFreeMountAgent.HorseHarnessItem;
			Vec3 position = createFreeMountAgent.Position;
			Vec2 vec = createFreeMountAgent.Direction;
			vec = vec.Normalized();
			mission.SpawnMonster(horseItem, horseHarnessItem, position, vec, createFreeMountAgent.AgentIndex);
		}

		private void HandleServerEventCreateAgent(GameNetworkMessage baseMessage)
		{
			CreateAgent createAgent = (CreateAgent)baseMessage;
			BasicCharacterObject character = createAgent.Character;
			NetworkCommunicator peer = createAgent.Peer;
			MissionPeer missionPeer = ((peer != null) ? peer.GetComponent<MissionPeer>() : null);
			Team teamFromTeamIndex = Mission.MissionNetworkHelper.GetTeamFromTeamIndex(createAgent.TeamIndex);
			AgentBuildData agentBuildData = new AgentBuildData(character).MissionPeer(createAgent.IsPlayerAgent ? missionPeer : null).Monster(createAgent.Monster).TroopOrigin(new BasicBattleAgentOrigin(character))
				.Equipment(createAgent.SpawnEquipment)
				.EquipmentSeed(createAgent.BodyPropertiesSeed);
			Vec3 position = createAgent.Position;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(position);
			Vec2 vec = createAgent.Direction;
			vec = vec.Normalized();
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(vec).MissionEquipment(createAgent.MissionEquipment).Team(teamFromTeamIndex)
				.Index(createAgent.AgentIndex)
				.MountIndex(createAgent.MountAgentIndex)
				.IsFemale(createAgent.IsFemale)
				.ClothingColor1(createAgent.ClothingColor1)
				.ClothingColor2(createAgent.ClothingColor2);
			Formation formation = null;
			if (teamFromTeamIndex != null && createAgent.FormationIndex >= 0 && !GameNetwork.IsReplay)
			{
				formation = teamFromTeamIndex.GetFormation((FormationClass)createAgent.FormationIndex);
				agentBuildData3.Formation(formation);
			}
			if (createAgent.IsPlayerAgent)
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
						banner = new Banner(formation.BannerCode, teamFromTeamIndex.Color, teamFromTeamIndex.Color2);
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
				banner = new Banner(missionPeer.Peer.BannerCode, teamFromTeamIndex.Color, teamFromTeamIndex.Color2);
			}
			agentBuildData3.Banner(banner);
			Agent mountAgent = base.Mission.SpawnAgent(agentBuildData3, false).MountAgent;
		}

		private void HandleServerEventSynchronizeAgentEquipment(GameNetworkMessage baseMessage)
		{
			SynchronizeAgentSpawnEquipment synchronizeAgentSpawnEquipment = (SynchronizeAgentSpawnEquipment)baseMessage;
			Mission.MissionNetworkHelper.GetAgentFromIndex(synchronizeAgentSpawnEquipment.AgentIndex, false).UpdateSpawnEquipmentAndRefreshVisuals(synchronizeAgentSpawnEquipment.SpawnEquipment);
		}

		private void HandleServerEventCreateAgentVisuals(GameNetworkMessage baseMessage)
		{
			CreateAgentVisuals createAgentVisuals = (CreateAgentVisuals)baseMessage;
			MissionPeer component = createAgentVisuals.Peer.GetComponent<MissionPeer>();
			BattleSideEnum side = component.Team.Side;
			BasicCharacterObject character = createAgentVisuals.Character;
			BasicCultureObject culture = character.Culture;
			AgentBuildData agentBuildData = new AgentBuildData(character).VisualsIndex(createAgentVisuals.VisualsIndex).Equipment(createAgentVisuals.Equipment).EquipmentSeed(createAgentVisuals.BodyPropertiesSeed)
				.IsFemale(createAgentVisuals.IsFemale)
				.ClothingColor1((side == BattleSideEnum.Attacker) ? culture.Color : culture.ClothAlternativeColor)
				.ClothingColor2((side == BattleSideEnum.Attacker) ? culture.Color2 : culture.ClothAlternativeColor2);
			if (createAgentVisuals.VisualsIndex == 0)
			{
				agentBuildData.BodyProperties(component.Peer.BodyProperties);
			}
			else
			{
				agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentRace, agentBuildData.AgentIsFemale, character.GetBodyPropertiesMin(false), character.GetBodyPropertiesMax(), (int)agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, createAgentVisuals.BodyPropertiesSeed, character.HairTags, character.BeardTags, character.TattooTags));
			}
			base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().SpawnAgentVisualsForPeer(component, agentBuildData, createAgentVisuals.SelectedEquipmentSetIndex, false, createAgentVisuals.TroopCountInFormation);
			if (agentBuildData.AgentVisualsIndex == 0)
			{
				component.HasSpawnedAgentVisuals = true;
				component.EquipmentUpdatingExpired = false;
			}
		}

		private void HandleServerEventRemoveAgentVisualsForPeer(GameNetworkMessage baseMessage)
		{
			MissionPeer component = ((RemoveAgentVisualsForPeer)baseMessage).Peer.GetComponent<MissionPeer>();
			base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, false);
			component.HasSpawnedAgentVisuals = false;
		}

		private void HandleServerEventRemoveAgentVisualsFromIndexForPeer(GameNetworkMessage baseMessage)
		{
			((RemoveAgentVisualsFromIndexForPeer)baseMessage).Peer.GetComponent<MissionPeer>();
		}

		private void HandleServerEventReplaceBotWithPlayer(GameNetworkMessage baseMessage)
		{
			ReplaceBotWithPlayer replaceBotWithPlayer = (ReplaceBotWithPlayer)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(replaceBotWithPlayer.BotAgentIndex, false);
			if (agentFromIndex.Formation != null)
			{
				agentFromIndex.Formation.PlayerOwner = agentFromIndex;
			}
			MissionPeer component = replaceBotWithPlayer.Peer.GetComponent<MissionPeer>();
			agentFromIndex.MissionPeer = replaceBotWithPlayer.Peer.GetComponent<MissionPeer>();
			agentFromIndex.Formation = component.ControlledFormation;
			agentFromIndex.Health = (float)replaceBotWithPlayer.Health;
			if (agentFromIndex.MountAgent != null)
			{
				agentFromIndex.MountAgent.Health = (float)replaceBotWithPlayer.MountHealth;
			}
			if (agentFromIndex.Formation != null)
			{
				agentFromIndex.Team.AssignPlayerAsSergeantOfFormation(component, component.ControlledFormation.FormationIndex);
			}
		}

		private void HandleServerEventSetWieldedItemIndex(GameNetworkMessage baseMessage)
		{
			SetWieldedItemIndex setWieldedItemIndex = (SetWieldedItemIndex)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(setWieldedItemIndex.AgentIndex, false);
			if (agentFromIndex != null)
			{
				agentFromIndex.SetWieldedItemIndexAsClient(setWieldedItemIndex.IsLeftHand ? Agent.HandIndex.OffHand : Agent.HandIndex.MainHand, setWieldedItemIndex.WieldedItemIndex, setWieldedItemIndex.IsWieldedInstantly, setWieldedItemIndex.IsWieldedOnSpawn, setWieldedItemIndex.MainHandCurrentUsageIndex);
				agentFromIndex.UpdateAgentStats();
			}
		}

		private void HandleServerEventSetWeaponNetworkData(GameNetworkMessage baseMessage)
		{
			SetWeaponNetworkData setWeaponNetworkData = (SetWeaponNetworkData)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(setWeaponNetworkData.AgentIndex, false);
			ItemObject item = agentFromIndex.Equipment[setWeaponNetworkData.WeaponEquipmentIndex].Item;
			WeaponComponentData weaponComponentData = ((item != null) ? item.PrimaryWeapon : null);
			if (weaponComponentData != null)
			{
				if (weaponComponentData.WeaponFlags.HasAnyFlag(WeaponFlags.HasHitPoints))
				{
					agentFromIndex.ChangeWeaponHitPoints(setWeaponNetworkData.WeaponEquipmentIndex, setWeaponNetworkData.DataValue);
					return;
				}
				if (weaponComponentData.IsConsumable)
				{
					agentFromIndex.SetWeaponAmountInSlot(setWeaponNetworkData.WeaponEquipmentIndex, setWeaponNetworkData.DataValue, true);
				}
			}
		}

		private void HandleServerEventSetWeaponAmmoData(GameNetworkMessage baseMessage)
		{
			SetWeaponAmmoData setWeaponAmmoData = (SetWeaponAmmoData)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(setWeaponAmmoData.AgentIndex, false);
			if (agentFromIndex.Equipment[setWeaponAmmoData.WeaponEquipmentIndex].CurrentUsageItem.IsRangedWeapon)
			{
				agentFromIndex.SetWeaponAmmoAsClient(setWeaponAmmoData.WeaponEquipmentIndex, setWeaponAmmoData.AmmoEquipmentIndex, setWeaponAmmoData.Ammo);
				return;
			}
			Debug.FailedAssert("Invalid item type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MissionNetworkComponent.cs", "HandleServerEventSetWeaponAmmoData", 463);
		}

		private void HandleServerEventSetWeaponReloadPhase(GameNetworkMessage baseMessage)
		{
			SetWeaponReloadPhase setWeaponReloadPhase = (SetWeaponReloadPhase)baseMessage;
			Mission.MissionNetworkHelper.GetAgentFromIndex(setWeaponReloadPhase.AgentIndex, false).SetWeaponReloadPhaseAsClient(setWeaponReloadPhase.EquipmentIndex, setWeaponReloadPhase.ReloadPhase);
		}

		private void HandleServerEventWeaponUsageIndexChangeMessage(GameNetworkMessage baseMessage)
		{
			WeaponUsageIndexChangeMessage weaponUsageIndexChangeMessage = (WeaponUsageIndexChangeMessage)baseMessage;
			Mission.MissionNetworkHelper.GetAgentFromIndex(weaponUsageIndexChangeMessage.AgentIndex, false).SetUsageIndexOfWeaponInSlotAsClient(weaponUsageIndexChangeMessage.SlotIndex, weaponUsageIndexChangeMessage.UsageIndex);
		}

		private void HandleServerEventStartSwitchingWeaponUsageIndex(GameNetworkMessage baseMessage)
		{
			StartSwitchingWeaponUsageIndex startSwitchingWeaponUsageIndex = (StartSwitchingWeaponUsageIndex)baseMessage;
			Mission.MissionNetworkHelper.GetAgentFromIndex(startSwitchingWeaponUsageIndex.AgentIndex, false).StartSwitchingWeaponUsageIndexAsClient(startSwitchingWeaponUsageIndex.EquipmentIndex, startSwitchingWeaponUsageIndex.UsageIndex, startSwitchingWeaponUsageIndex.CurrentMovementFlagUsageDirection);
		}

		private void HandleServerEventInitializeFormation(GameNetworkMessage baseMessage)
		{
			InitializeFormation initializeFormation = (InitializeFormation)baseMessage;
			Mission.MissionNetworkHelper.GetTeamFromTeamIndex(initializeFormation.TeamIndex).GetFormation((FormationClass)initializeFormation.FormationIndex).BannerCode = initializeFormation.BannerCode;
		}

		private void HandleServerEventSetSpawnedFormationCount(GameNetworkMessage baseMessage)
		{
			SetSpawnedFormationCount setSpawnedFormationCount = (SetSpawnedFormationCount)baseMessage;
			base.Mission.NumOfFormationsSpawnedTeamOne = setSpawnedFormationCount.NumOfFormationsTeamOne;
			base.Mission.NumOfFormationsSpawnedTeamTwo = setSpawnedFormationCount.NumOfFormationsTeamTwo;
		}

		private void HandleServerEventAddTeam(GameNetworkMessage baseMessage)
		{
			AddTeam addTeam = (AddTeam)baseMessage;
			Banner banner = (string.IsNullOrEmpty(addTeam.BannerCode) ? null : new Banner(addTeam.BannerCode, addTeam.Color, addTeam.Color2));
			base.Mission.Teams.Add(addTeam.Side, addTeam.Color, addTeam.Color2, banner, addTeam.IsPlayerGeneral, addTeam.IsPlayerSergeant, true);
		}

		private void HandleServerEventTeamSetIsEnemyOf(GameNetworkMessage baseMessage)
		{
			TeamSetIsEnemyOf teamSetIsEnemyOf = (TeamSetIsEnemyOf)baseMessage;
			Team teamFromTeamIndex = Mission.MissionNetworkHelper.GetTeamFromTeamIndex(teamSetIsEnemyOf.Team1Index);
			Team teamFromTeamIndex2 = Mission.MissionNetworkHelper.GetTeamFromTeamIndex(teamSetIsEnemyOf.Team2Index);
			teamFromTeamIndex.SetIsEnemyOf(teamFromTeamIndex2, teamSetIsEnemyOf.IsEnemyOf);
		}

		private void HandleServerEventAssignFormationToPlayer(GameNetworkMessage baseMessage)
		{
			AssignFormationToPlayer assignFormationToPlayer = (AssignFormationToPlayer)baseMessage;
			MissionPeer component = assignFormationToPlayer.Peer.GetComponent<MissionPeer>();
			component.Team.AssignPlayerAsSergeantOfFormation(component, assignFormationToPlayer.FormationClass);
		}

		private void HandleServerEventExistingObjectsBegin(GameNetworkMessage baseMessage)
		{
		}

		private void HandleServerEventExistingObjectsEnd(GameNetworkMessage baseMessage)
		{
		}

		private void HandleServerEventClearMission(GameNetworkMessage baseMessage)
		{
			base.Mission.ResetMission();
		}

		private void HandleServerEventCreateMissionObject(GameNetworkMessage baseMessage)
		{
			CreateMissionObject createMissionObject = (CreateMissionObject)baseMessage;
			GameEntity gameEntity = GameEntity.Instantiate(base.Mission.Scene, createMissionObject.Prefab, createMissionObject.Frame);
			MissionObject firstScriptOfType = gameEntity.GetFirstScriptOfType<MissionObject>();
			if (firstScriptOfType != null)
			{
				firstScriptOfType.Id = createMissionObject.ObjectId;
				int num = 0;
				using (IEnumerator<GameEntity> enumerator = gameEntity.GetChildren().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionObject firstScriptOfType2;
						if ((firstScriptOfType2 = enumerator.Current.GetFirstScriptOfType<MissionObject>()) != null)
						{
							firstScriptOfType2.Id = createMissionObject.ChildObjectIds[num++];
						}
					}
				}
			}
		}

		private void HandleServerEventRemoveMissionObject(GameNetworkMessage baseMessage)
		{
			RemoveMissionObject message = (RemoveMissionObject)baseMessage;
			MissionObject missionObject = base.Mission.MissionObjects.FirstOrDefault((MissionObject mo) => mo.Id == message.ObjectId);
			if (missionObject == null)
			{
				return;
			}
			missionObject.GameEntity.Remove(82);
		}

		private void HandleServerEventStopPhysicsAndSetFrameOfMissionObject(GameNetworkMessage baseMessage)
		{
			StopPhysicsAndSetFrameOfMissionObject message = (StopPhysicsAndSetFrameOfMissionObject)baseMessage;
			SpawnedItemEntity spawnedItemEntity = (SpawnedItemEntity)base.Mission.MissionObjects.FirstOrDefault((MissionObject mo) => mo.Id == message.ObjectId);
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(message.ParentId);
			if (spawnedItemEntity == null)
			{
				return;
			}
			spawnedItemEntity.StopPhysicsAndSetFrameForClient(message.Frame, (missionObjectFromMissionObjectId != null) ? missionObjectFromMissionObjectId.GameEntity : null);
		}

		private void HandleServerEventBurstMissionObjectParticles(GameNetworkMessage baseMessage)
		{
			BurstMissionObjectParticles burstMissionObjectParticles = (BurstMissionObjectParticles)baseMessage;
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(burstMissionObjectParticles.MissionObjectId) as SynchedMissionObject).BurstParticlesSynched(burstMissionObjectParticles.DoChildren);
		}

		private void HandleServerEventSetMissionObjectVisibility(GameNetworkMessage baseMessage)
		{
			SetMissionObjectVisibility setMissionObjectVisibility = (SetMissionObjectVisibility)baseMessage;
			Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectVisibility.MissionObjectId).GameEntity.SetVisibilityExcludeParents(setMissionObjectVisibility.Visible);
		}

		private void HandleServerEventSetMissionObjectDisabled(GameNetworkMessage baseMessage)
		{
			Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(((SetMissionObjectDisabled)baseMessage).MissionObjectId).SetDisabledAndMakeInvisible(false);
		}

		private void HandleServerEventSetMissionObjectColors(GameNetworkMessage baseMessage)
		{
			SetMissionObjectColors setMissionObjectColors = (SetMissionObjectColors)baseMessage;
			SynchedMissionObject synchedMissionObject = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectColors.MissionObjectId) as SynchedMissionObject;
			if (synchedMissionObject != null)
			{
				synchedMissionObject.SetTeamColors(setMissionObjectColors.Color, setMissionObjectColors.Color2);
			}
		}

		private void HandleServerEventSetMissionObjectFrame(GameNetworkMessage baseMessage)
		{
			SetMissionObjectFrame setMissionObjectFrame = (SetMissionObjectFrame)baseMessage;
			SynchedMissionObject synchedMissionObject = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectFrame.MissionObjectId) as SynchedMissionObject;
			MatrixFrame frame = setMissionObjectFrame.Frame;
			synchedMissionObject.SetFrameSynched(ref frame, true);
		}

		private void HandleServerEventSetMissionObjectGlobalFrame(GameNetworkMessage baseMessage)
		{
			SetMissionObjectGlobalFrame setMissionObjectGlobalFrame = (SetMissionObjectGlobalFrame)baseMessage;
			SynchedMissionObject synchedMissionObject = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectGlobalFrame.MissionObjectId) as SynchedMissionObject;
			MatrixFrame frame = setMissionObjectGlobalFrame.Frame;
			synchedMissionObject.SetGlobalFrameSynched(ref frame, true);
		}

		private void HandleServerEventSetMissionObjectFrameOverTime(GameNetworkMessage baseMessage)
		{
			SetMissionObjectFrameOverTime setMissionObjectFrameOverTime = (SetMissionObjectFrameOverTime)baseMessage;
			SynchedMissionObject synchedMissionObject = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectFrameOverTime.MissionObjectId) as SynchedMissionObject;
			MatrixFrame frame = setMissionObjectFrameOverTime.Frame;
			synchedMissionObject.SetFrameSynchedOverTime(ref frame, setMissionObjectFrameOverTime.Duration, true);
		}

		private void HandleServerEventSetMissionObjectGlobalFrameOverTime(GameNetworkMessage baseMessage)
		{
			SetMissionObjectGlobalFrameOverTime setMissionObjectGlobalFrameOverTime = (SetMissionObjectGlobalFrameOverTime)baseMessage;
			SynchedMissionObject synchedMissionObject = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectGlobalFrameOverTime.MissionObjectId) as SynchedMissionObject;
			MatrixFrame frame = setMissionObjectGlobalFrameOverTime.Frame;
			synchedMissionObject.SetGlobalFrameSynchedOverTime(ref frame, setMissionObjectGlobalFrameOverTime.Duration, true);
		}

		private void HandleServerEventSetMissionObjectAnimationAtChannel(GameNetworkMessage baseMessage)
		{
			SetMissionObjectAnimationAtChannel setMissionObjectAnimationAtChannel = (SetMissionObjectAnimationAtChannel)baseMessage;
			Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectAnimationAtChannel.MissionObjectId).GameEntity.Skeleton.SetAnimationAtChannel(setMissionObjectAnimationAtChannel.AnimationIndex, setMissionObjectAnimationAtChannel.ChannelNo, setMissionObjectAnimationAtChannel.AnimationSpeed, -1f, 0f);
		}

		private void HandleServerEventSetRangedSiegeWeaponAmmo(GameNetworkMessage baseMessage)
		{
			SetRangedSiegeWeaponAmmo setRangedSiegeWeaponAmmo = (SetRangedSiegeWeaponAmmo)baseMessage;
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setRangedSiegeWeaponAmmo.RangedSiegeWeaponId) as RangedSiegeWeapon).SetAmmo(setRangedSiegeWeaponAmmo.AmmoCount);
		}

		private void HandleServerEventRangedSiegeWeaponChangeProjectile(GameNetworkMessage baseMessage)
		{
			RangedSiegeWeaponChangeProjectile rangedSiegeWeaponChangeProjectile = (RangedSiegeWeaponChangeProjectile)baseMessage;
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(rangedSiegeWeaponChangeProjectile.RangedSiegeWeaponId) as RangedSiegeWeapon).ChangeProjectileEntityClient(rangedSiegeWeaponChangeProjectile.Index);
		}

		private void HandleServerEventSetStonePileAmmo(GameNetworkMessage baseMessage)
		{
			SetStonePileAmmo setStonePileAmmo = (SetStonePileAmmo)baseMessage;
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setStonePileAmmo.StonePileId) as StonePile).SetAmmo(setStonePileAmmo.AmmoCount);
		}

		private void HandleServerEventSetRangedSiegeWeaponState(GameNetworkMessage baseMessage)
		{
			SetRangedSiegeWeaponState setRangedSiegeWeaponState = (SetRangedSiegeWeaponState)baseMessage;
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setRangedSiegeWeaponState.RangedSiegeWeaponId) as RangedSiegeWeapon).State = setRangedSiegeWeaponState.State;
		}

		private void HandleServerEventSetSiegeLadderState(GameNetworkMessage baseMessage)
		{
			SetSiegeLadderState setSiegeLadderState = (SetSiegeLadderState)baseMessage;
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setSiegeLadderState.SiegeLadderId) as SiegeLadder).State = setSiegeLadderState.State;
		}

		private void HandleServerEventSetSiegeTowerGateState(GameNetworkMessage baseMessage)
		{
			SetSiegeTowerGateState setSiegeTowerGateState = (SetSiegeTowerGateState)baseMessage;
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setSiegeTowerGateState.SiegeTowerId) as SiegeTower).State = setSiegeTowerGateState.State;
		}

		private void HandleServerEventSetSiegeTowerHasArrivedAtTarget(GameNetworkMessage baseMessage)
		{
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(((SetSiegeTowerHasArrivedAtTarget)baseMessage).SiegeTowerId) as SiegeTower).HasArrivedAtTarget = true;
		}

		private void HandleServerEventSetBatteringRamHasArrivedAtTarget(GameNetworkMessage baseMessage)
		{
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(((SetBatteringRamHasArrivedAtTarget)baseMessage).BatteringRamId) as BatteringRam).HasArrivedAtTarget = true;
		}

		private void HandleServerEventSetSiegeMachineMovementDistance(GameNetworkMessage baseMessage)
		{
			SetSiegeMachineMovementDistance setSiegeMachineMovementDistance = (SetSiegeMachineMovementDistance)baseMessage;
			UsableMachine usableMachine = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setSiegeMachineMovementDistance.UsableMachineId) as UsableMachine;
			if (usableMachine != null)
			{
				if (usableMachine is SiegeTower)
				{
					((SiegeTower)usableMachine).MovementComponent.SetDistanceTraveledAsClient(setSiegeMachineMovementDistance.Distance);
					return;
				}
				((BatteringRam)usableMachine).MovementComponent.SetDistanceTraveledAsClient(setSiegeMachineMovementDistance.Distance);
			}
		}

		private void HandleServerEventSetMissionObjectAnimationChannelParameter(GameNetworkMessage baseMessage)
		{
			SetMissionObjectAnimationChannelParameter setMissionObjectAnimationChannelParameter = (SetMissionObjectAnimationChannelParameter)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectAnimationChannelParameter.MissionObjectId);
			if (missionObjectFromMissionObjectId != null)
			{
				missionObjectFromMissionObjectId.GameEntity.Skeleton.SetAnimationParameterAtChannel(setMissionObjectAnimationChannelParameter.ChannelNo, setMissionObjectAnimationChannelParameter.Parameter);
			}
		}

		private void HandleServerEventSetMissionObjectVertexAnimation(GameNetworkMessage baseMessage)
		{
			SetMissionObjectVertexAnimation setMissionObjectVertexAnimation = (SetMissionObjectVertexAnimation)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectVertexAnimation.MissionObjectId);
			if (missionObjectFromMissionObjectId != null)
			{
				(missionObjectFromMissionObjectId as VertexAnimator).SetAnimationSynched(setMissionObjectVertexAnimation.BeginKey, setMissionObjectVertexAnimation.EndKey, setMissionObjectVertexAnimation.Speed);
			}
		}

		private void HandleServerEventSetMissionObjectVertexAnimationProgress(GameNetworkMessage baseMessage)
		{
			SetMissionObjectVertexAnimationProgress setMissionObjectVertexAnimationProgress = (SetMissionObjectVertexAnimationProgress)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectVertexAnimationProgress.MissionObjectId);
			if (missionObjectFromMissionObjectId != null)
			{
				(missionObjectFromMissionObjectId as VertexAnimator).SetProgressSynched(setMissionObjectVertexAnimationProgress.Progress);
			}
		}

		private void HandleServerEventSetMissionObjectAnimationPaused(GameNetworkMessage baseMessage)
		{
			SetMissionObjectAnimationPaused setMissionObjectAnimationPaused = (SetMissionObjectAnimationPaused)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectAnimationPaused.MissionObjectId);
			if (missionObjectFromMissionObjectId != null)
			{
				if (setMissionObjectAnimationPaused.IsPaused)
				{
					missionObjectFromMissionObjectId.GameEntity.PauseSkeletonAnimation();
					return;
				}
				missionObjectFromMissionObjectId.GameEntity.ResumeSkeletonAnimation();
			}
		}

		private void HandleServerEventAddMissionObjectBodyFlags(GameNetworkMessage baseMessage)
		{
			AddMissionObjectBodyFlags addMissionObjectBodyFlags = (AddMissionObjectBodyFlags)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(addMissionObjectBodyFlags.MissionObjectId);
			if (missionObjectFromMissionObjectId != null)
			{
				missionObjectFromMissionObjectId.GameEntity.AddBodyFlags(addMissionObjectBodyFlags.BodyFlags, addMissionObjectBodyFlags.ApplyToChildren);
			}
		}

		private void HandleServerEventRemoveMissionObjectBodyFlags(GameNetworkMessage baseMessage)
		{
			RemoveMissionObjectBodyFlags removeMissionObjectBodyFlags = (RemoveMissionObjectBodyFlags)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(removeMissionObjectBodyFlags.MissionObjectId);
			if (missionObjectFromMissionObjectId != null)
			{
				missionObjectFromMissionObjectId.GameEntity.RemoveBodyFlags(removeMissionObjectBodyFlags.BodyFlags, removeMissionObjectBodyFlags.ApplyToChildren);
			}
		}

		private void HandleServerEventSetMachineTargetRotation(GameNetworkMessage baseMessage)
		{
			SetMachineTargetRotation setMachineTargetRotation = (SetMachineTargetRotation)baseMessage;
			UsableMachine usableMachine = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMachineTargetRotation.UsableMachineId) as UsableMachine;
			if (usableMachine != null && usableMachine.PilotAgent != null)
			{
				((RangedSiegeWeapon)usableMachine).AimAtRotation(setMachineTargetRotation.HorizontalRotation, setMachineTargetRotation.VerticalRotation);
			}
		}

		private void HandleServerEventSetUsableGameObjectIsDeactivated(GameNetworkMessage baseMessage)
		{
			SetUsableMissionObjectIsDeactivated setUsableMissionObjectIsDeactivated = (SetUsableMissionObjectIsDeactivated)baseMessage;
			UsableMissionObject usableMissionObject = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setUsableMissionObjectIsDeactivated.UsableGameObjectId) as UsableMissionObject;
			if (usableMissionObject != null)
			{
				usableMissionObject.IsDeactivated = setUsableMissionObjectIsDeactivated.IsDeactivated;
			}
		}

		private void HandleServerEventSetUsableGameObjectIsDisabledForPlayers(GameNetworkMessage baseMessage)
		{
			SetUsableMissionObjectIsDisabledForPlayers setUsableMissionObjectIsDisabledForPlayers = (SetUsableMissionObjectIsDisabledForPlayers)baseMessage;
			UsableMissionObject usableMissionObject = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setUsableMissionObjectIsDisabledForPlayers.UsableGameObjectId) as UsableMissionObject;
			if (usableMissionObject != null)
			{
				usableMissionObject.IsDisabledForPlayers = setUsableMissionObjectIsDisabledForPlayers.IsDisabledForPlayers;
			}
		}

		private void HandleServerEventSetMissionObjectImpulse(GameNetworkMessage baseMessage)
		{
			SetMissionObjectImpulse setMissionObjectImpulse = (SetMissionObjectImpulse)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMissionObjectImpulse.MissionObjectId);
			if (missionObjectFromMissionObjectId != null)
			{
				Vec3 position = setMissionObjectImpulse.Position;
				missionObjectFromMissionObjectId.GameEntity.ApplyLocalImpulseToDynamicBody(position, setMissionObjectImpulse.Impulse);
			}
		}

		private void HandleServerEventSetAgentTargetPositionAndDirection(GameNetworkMessage baseMessage)
		{
			SetAgentTargetPositionAndDirection setAgentTargetPositionAndDirection = (SetAgentTargetPositionAndDirection)baseMessage;
			Vec2 position = setAgentTargetPositionAndDirection.Position;
			Vec3 direction = setAgentTargetPositionAndDirection.Direction;
			Mission.MissionNetworkHelper.GetAgentFromIndex(setAgentTargetPositionAndDirection.AgentIndex, false).SetTargetPositionAndDirectionSynched(ref position, ref direction);
		}

		private void HandleServerEventSetAgentTargetPosition(GameNetworkMessage baseMessage)
		{
			SetAgentTargetPosition setAgentTargetPosition = (SetAgentTargetPosition)baseMessage;
			Vec2 position = setAgentTargetPosition.Position;
			Mission.MissionNetworkHelper.GetAgentFromIndex(setAgentTargetPosition.AgentIndex, false).SetTargetPositionSynched(ref position);
		}

		private void HandleServerEventClearAgentTargetFrame(GameNetworkMessage baseMessage)
		{
			Mission.MissionNetworkHelper.GetAgentFromIndex(((ClearAgentTargetFrame)baseMessage).AgentIndex, false).ClearTargetFrame();
		}

		private void HandleServerEventAgentTeleportToFrame(GameNetworkMessage baseMessage)
		{
			AgentTeleportToFrame agentTeleportToFrame = (AgentTeleportToFrame)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(agentTeleportToFrame.AgentIndex, false);
			agentFromIndex.TeleportToPosition(agentTeleportToFrame.Position);
			Vec2 vec = agentTeleportToFrame.Direction.Normalized();
			agentFromIndex.SetMovementDirection(vec);
			agentFromIndex.LookDirection = vec.ToVec3(0f);
		}

		private void HandleServerEventSetAgentPeer(GameNetworkMessage baseMessage)
		{
			SetAgentPeer setAgentPeer = (SetAgentPeer)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(setAgentPeer.AgentIndex, true);
			if (agentFromIndex != null)
			{
				NetworkCommunicator peer = setAgentPeer.Peer;
				MissionPeer missionPeer = ((peer != null) ? peer.GetComponent<MissionPeer>() : null);
				agentFromIndex.MissionPeer = missionPeer;
			}
		}

		private void HandleServerEventSetAgentIsPlayer(GameNetworkMessage baseMessage)
		{
			SetAgentIsPlayer setAgentIsPlayer = (SetAgentIsPlayer)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(setAgentIsPlayer.AgentIndex, false);
			if (agentFromIndex.Controller == Agent.ControllerType.Player != setAgentIsPlayer.IsPlayer)
			{
				if (!agentFromIndex.IsMine)
				{
					agentFromIndex.Controller = Agent.ControllerType.None;
					return;
				}
				agentFromIndex.Controller = Agent.ControllerType.Player;
			}
		}

		private void HandleServerEventSetAgentHealth(GameNetworkMessage baseMessage)
		{
			SetAgentHealth setAgentHealth = (SetAgentHealth)baseMessage;
			Mission.MissionNetworkHelper.GetAgentFromIndex(setAgentHealth.AgentIndex, false).Health = (float)setAgentHealth.Health;
		}

		private void HandleServerEventAgentSetTeam(GameNetworkMessage baseMessage)
		{
			AgentSetTeam agentSetTeam = (AgentSetTeam)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(agentSetTeam.AgentIndex, false);
			MBTeam mbteamFromTeamIndex = Mission.MissionNetworkHelper.GetMBTeamFromTeamIndex(agentSetTeam.TeamIndex);
			agentFromIndex.SetTeam(base.Mission.Teams.Find(mbteamFromTeamIndex), false);
		}

		private void HandleServerEventSetAgentActionSet(GameNetworkMessage baseMessage)
		{
			SetAgentActionSet setAgentActionSet = (SetAgentActionSet)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(setAgentActionSet.AgentIndex, false);
			AnimationSystemData animationSystemData = agentFromIndex.Monster.FillAnimationSystemData(setAgentActionSet.ActionSet, setAgentActionSet.StepSize, false);
			animationSystemData.NumPaces = setAgentActionSet.NumPaces;
			animationSystemData.MonsterUsageSetIndex = setAgentActionSet.MonsterUsageSetIndex;
			animationSystemData.WalkingSpeedLimit = setAgentActionSet.WalkingSpeedLimit;
			animationSystemData.CrouchWalkingSpeedLimit = setAgentActionSet.CrouchWalkingSpeedLimit;
			agentFromIndex.SetActionSet(ref animationSystemData);
		}

		private void HandleServerEventMakeAgentDead(GameNetworkMessage baseMessage)
		{
			MakeAgentDead makeAgentDead = (MakeAgentDead)baseMessage;
			Mission.MissionNetworkHelper.GetAgentFromIndex(makeAgentDead.AgentIndex, false).MakeDead(makeAgentDead.IsKilled, makeAgentDead.ActionCodeIndex);
		}

		private void HandleServerEventAddPrefabComponentToAgentBone(GameNetworkMessage baseMessage)
		{
			AddPrefabComponentToAgentBone addPrefabComponentToAgentBone = (AddPrefabComponentToAgentBone)baseMessage;
			Mission.MissionNetworkHelper.GetAgentFromIndex(addPrefabComponentToAgentBone.AgentIndex, false).AddSynchedPrefabComponentToBone(addPrefabComponentToAgentBone.PrefabName, addPrefabComponentToAgentBone.BoneIndex);
		}

		private void HandleServerEventSetAgentPrefabComponentVisibility(GameNetworkMessage baseMessage)
		{
			SetAgentPrefabComponentVisibility setAgentPrefabComponentVisibility = (SetAgentPrefabComponentVisibility)baseMessage;
			Mission.MissionNetworkHelper.GetAgentFromIndex(setAgentPrefabComponentVisibility.AgentIndex, false).SetSynchedPrefabComponentVisibility(setAgentPrefabComponentVisibility.ComponentIndex, setAgentPrefabComponentVisibility.Visibility);
		}

		private void HandleServerEventAgentSetFormation(GameNetworkMessage baseMessage)
		{
			AgentSetFormation agentSetFormation = (AgentSetFormation)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(agentSetFormation.AgentIndex, false);
			Team team = agentFromIndex.Team;
			Formation formation = null;
			if (team != null)
			{
				formation = ((agentSetFormation.FormationIndex >= 0) ? team.GetFormation((FormationClass)agentSetFormation.FormationIndex) : null);
			}
			agentFromIndex.Formation = formation;
		}

		private void HandleServerEventUseObject(GameNetworkMessage baseMessage)
		{
			UseObject useObject = (UseObject)baseMessage;
			UsableMissionObject usableMissionObject = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(useObject.UsableGameObjectId) as UsableMissionObject;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(useObject.AgentIndex, false);
			if (usableMissionObject != null)
			{
				usableMissionObject.SetUserForClient(agentFromIndex);
			}
		}

		private void HandleServerEventStopUsingObject(GameNetworkMessage baseMessage)
		{
			StopUsingObject stopUsingObject = (StopUsingObject)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(stopUsingObject.AgentIndex, false);
			if (agentFromIndex == null)
			{
				return;
			}
			agentFromIndex.StopUsingGameObject(stopUsingObject.IsSuccessful, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
		}

		private void HandleServerEventHitSynchronizeObjectHitpoints(GameNetworkMessage baseMessage)
		{
			SyncObjectHitpoints syncObjectHitpoints = (SyncObjectHitpoints)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(syncObjectHitpoints.MissionObjectId);
			if (missionObjectFromMissionObjectId != null)
			{
				missionObjectFromMissionObjectId.GameEntity.GetFirstScriptOfType<DestructableComponent>().HitPoint = syncObjectHitpoints.Hitpoints;
			}
		}

		private void HandleServerEventHitSynchronizeObjectDestructionLevel(GameNetworkMessage baseMessage)
		{
			SyncObjectDestructionLevel syncObjectDestructionLevel = (SyncObjectDestructionLevel)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(syncObjectDestructionLevel.MissionObjectId);
			if (missionObjectFromMissionObjectId == null)
			{
				return;
			}
			missionObjectFromMissionObjectId.GameEntity.GetFirstScriptOfType<DestructableComponent>().SetDestructionLevel(syncObjectDestructionLevel.DestructionLevel, syncObjectDestructionLevel.ForcedIndex, syncObjectDestructionLevel.BlowMagnitude, syncObjectDestructionLevel.BlowPosition, syncObjectDestructionLevel.BlowDirection, false);
		}

		private void HandleServerEventHitBurstAllHeavyHitParticles(GameNetworkMessage baseMessage)
		{
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(((BurstAllHeavyHitParticles)baseMessage).MissionObjectId);
			if (missionObjectFromMissionObjectId == null)
			{
				return;
			}
			missionObjectFromMissionObjectId.GameEntity.GetFirstScriptOfType<DestructableComponent>().BurstHeavyHitParticles();
		}

		private void HandleServerEventSynchronizeMissionObject(GameNetworkMessage baseMessage)
		{
			SynchronizeMissionObject synchronizeMissionObject = (SynchronizeMissionObject)baseMessage;
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(synchronizeMissionObject.MissionObjectId) as SynchedMissionObject).OnAfterReadFromNetwork(synchronizeMissionObject.RecordPair);
		}

		private void HandleServerEventSpawnWeaponWithNewEntity(GameNetworkMessage baseMessage)
		{
			SpawnWeaponWithNewEntity spawnWeaponWithNewEntity = (SpawnWeaponWithNewEntity)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(spawnWeaponWithNewEntity.ParentMissionObjectId);
			GameEntity gameEntity = base.Mission.SpawnWeaponWithNewEntityAux(spawnWeaponWithNewEntity.Weapon, spawnWeaponWithNewEntity.WeaponSpawnFlags, spawnWeaponWithNewEntity.Frame, spawnWeaponWithNewEntity.ForcedIndex, missionObjectFromMissionObjectId, spawnWeaponWithNewEntity.HasLifeTime);
			if (!spawnWeaponWithNewEntity.IsVisible)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
		}

		private void HandleServerEventAttachWeaponToSpawnedWeapon(GameNetworkMessage baseMessage)
		{
			AttachWeaponToSpawnedWeapon attachWeaponToSpawnedWeapon = (AttachWeaponToSpawnedWeapon)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(attachWeaponToSpawnedWeapon.MissionObjectId);
			base.Mission.AttachWeaponWithNewEntityToSpawnedWeapon(attachWeaponToSpawnedWeapon.Weapon, missionObjectFromMissionObjectId as SpawnedItemEntity, attachWeaponToSpawnedWeapon.AttachLocalFrame);
		}

		private void HandleServerEventAttachWeaponToAgent(GameNetworkMessage baseMessage)
		{
			AttachWeaponToAgent attachWeaponToAgent = (AttachWeaponToAgent)baseMessage;
			MatrixFrame attachLocalFrame = attachWeaponToAgent.AttachLocalFrame;
			Mission.MissionNetworkHelper.GetAgentFromIndex(attachWeaponToAgent.AgentIndex, false).AttachWeaponToBone(attachWeaponToAgent.Weapon, null, attachWeaponToAgent.BoneIndex, ref attachLocalFrame);
		}

		private void HandleServerEventHandleMissileCollisionReaction(GameNetworkMessage baseMessage)
		{
			HandleMissileCollisionReaction handleMissileCollisionReaction = (HandleMissileCollisionReaction)baseMessage;
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(handleMissileCollisionReaction.AttachedMissionObjectId);
			base.Mission.HandleMissileCollisionReaction(handleMissileCollisionReaction.MissileIndex, handleMissileCollisionReaction.CollisionReaction, handleMissileCollisionReaction.AttachLocalFrame, handleMissileCollisionReaction.IsAttachedFrameLocal, Mission.MissionNetworkHelper.GetAgentFromIndex(handleMissileCollisionReaction.AttackerAgentIndex, true), Mission.MissionNetworkHelper.GetAgentFromIndex(handleMissileCollisionReaction.AttachedAgentIndex, true), handleMissileCollisionReaction.AttachedToShield, handleMissileCollisionReaction.AttachedBoneIndex, missionObjectFromMissionObjectId, handleMissileCollisionReaction.BounceBackVelocity, handleMissileCollisionReaction.BounceBackAngularVelocity, handleMissileCollisionReaction.ForcedSpawnIndex);
		}

		private void HandleServerEventSpawnWeaponAsDropFromAgent(GameNetworkMessage baseMessage)
		{
			SpawnWeaponAsDropFromAgent spawnWeaponAsDropFromAgent = (SpawnWeaponAsDropFromAgent)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(spawnWeaponAsDropFromAgent.AgentIndex, false);
			Vec3 velocity = spawnWeaponAsDropFromAgent.Velocity;
			Vec3 angularVelocity = spawnWeaponAsDropFromAgent.AngularVelocity;
			base.Mission.SpawnWeaponAsDropFromAgentAux(agentFromIndex, spawnWeaponAsDropFromAgent.EquipmentIndex, ref velocity, ref angularVelocity, spawnWeaponAsDropFromAgent.WeaponSpawnFlags, spawnWeaponAsDropFromAgent.ForcedIndex);
		}

		private void HandleServerEventSpawnAttachedWeaponOnSpawnedWeapon(GameNetworkMessage baseMessage)
		{
			SpawnAttachedWeaponOnSpawnedWeapon spawnAttachedWeaponOnSpawnedWeapon = (SpawnAttachedWeaponOnSpawnedWeapon)baseMessage;
			SpawnedItemEntity spawnedItemEntity = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(spawnAttachedWeaponOnSpawnedWeapon.SpawnedWeaponId) as SpawnedItemEntity;
			base.Mission.SpawnAttachedWeaponOnSpawnedWeapon(spawnedItemEntity, spawnAttachedWeaponOnSpawnedWeapon.AttachmentIndex, spawnAttachedWeaponOnSpawnedWeapon.ForcedIndex);
		}

		private void HandleServerEventSpawnAttachedWeaponOnCorpse(GameNetworkMessage baseMessage)
		{
			SpawnAttachedWeaponOnCorpse spawnAttachedWeaponOnCorpse = (SpawnAttachedWeaponOnCorpse)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(spawnAttachedWeaponOnCorpse.AgentIndex, false);
			base.Mission.SpawnAttachedWeaponOnCorpse(agentFromIndex, spawnAttachedWeaponOnCorpse.AttachedIndex, spawnAttachedWeaponOnCorpse.ForcedIndex);
		}

		private void HandleServerEventRemoveEquippedWeapon(GameNetworkMessage baseMessage)
		{
			RemoveEquippedWeapon removeEquippedWeapon = (RemoveEquippedWeapon)baseMessage;
			Mission.MissionNetworkHelper.GetAgentFromIndex(removeEquippedWeapon.AgentIndex, false).RemoveEquippedWeapon(removeEquippedWeapon.SlotIndex);
		}

		private void HandleServerEventBarkAgent(GameNetworkMessage baseMessage)
		{
			BarkAgent barkAgent = (BarkAgent)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(barkAgent.AgentIndex, false);
			agentFromIndex.HandleBark(barkAgent.IndexOfBark);
			if (!this._chatBox.IsPlayerMuted(agentFromIndex.MissionPeer.Peer.Id))
			{
				GameTexts.SetVariable("LEFT", agentFromIndex.Name);
				GameTexts.SetVariable("RIGHT", SkinVoiceManager.VoiceType.MpBarks[barkAgent.IndexOfBark].GetName());
				InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString(), Color.White, "Bark"));
			}
		}

		private void HandleServerEventEquipWeaponWithNewEntity(GameNetworkMessage baseMessage)
		{
			EquipWeaponWithNewEntity equipWeaponWithNewEntity = (EquipWeaponWithNewEntity)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(equipWeaponWithNewEntity.AgentIndex, false);
			if (agentFromIndex != null)
			{
				MissionWeapon weapon = equipWeaponWithNewEntity.Weapon;
				agentFromIndex.EquipWeaponWithNewEntity(equipWeaponWithNewEntity.SlotIndex, ref weapon);
			}
		}

		private void HandleServerEventAttachWeaponToWeaponInAgentEquipmentSlot(GameNetworkMessage baseMessage)
		{
			AttachWeaponToWeaponInAgentEquipmentSlot attachWeaponToWeaponInAgentEquipmentSlot = (AttachWeaponToWeaponInAgentEquipmentSlot)baseMessage;
			MatrixFrame attachLocalFrame = attachWeaponToWeaponInAgentEquipmentSlot.AttachLocalFrame;
			Mission.MissionNetworkHelper.GetAgentFromIndex(attachWeaponToWeaponInAgentEquipmentSlot.AgentIndex, false).AttachWeaponToWeapon(attachWeaponToWeaponInAgentEquipmentSlot.SlotIndex, attachWeaponToWeaponInAgentEquipmentSlot.Weapon, null, ref attachLocalFrame);
		}

		private void HandleServerEventEquipWeaponFromSpawnedItemEntity(GameNetworkMessage baseMessage)
		{
			EquipWeaponFromSpawnedItemEntity equipWeaponFromSpawnedItemEntity = (EquipWeaponFromSpawnedItemEntity)baseMessage;
			SpawnedItemEntity spawnedItemEntity = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(equipWeaponFromSpawnedItemEntity.SpawnedItemEntityId) as SpawnedItemEntity;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(equipWeaponFromSpawnedItemEntity.AgentIndex, true);
			if (agentFromIndex == null)
			{
				return;
			}
			agentFromIndex.EquipWeaponFromSpawnedItemEntity(equipWeaponFromSpawnedItemEntity.SlotIndex, spawnedItemEntity, equipWeaponFromSpawnedItemEntity.RemoveWeapon);
		}

		private void HandleServerEventCreateMissile(GameNetworkMessage baseMessage)
		{
			CreateMissile createMissile = (CreateMissile)baseMessage;
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(createMissile.AgentIndex, false);
			if (createMissile.WeaponIndex != EquipmentIndex.None)
			{
				Vec3 vec = createMissile.Direction * createMissile.Speed;
				base.Mission.OnAgentShootMissile(agentFromIndex, createMissile.WeaponIndex, createMissile.Position, vec, createMissile.Orientation, createMissile.HasRigidBody, createMissile.IsPrimaryWeaponShot, createMissile.MissileIndex);
				return;
			}
			MissionObject missionObjectFromMissionObjectId = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(createMissile.MissionObjectToIgnoreId);
			base.Mission.AddCustomMissile(agentFromIndex, createMissile.Weapon, createMissile.Position, createMissile.Direction, createMissile.Orientation, createMissile.Speed, createMissile.Speed, createMissile.HasRigidBody, missionObjectFromMissionObjectId, createMissile.MissileIndex);
		}

		private void HandleServerEventAgentHit(GameNetworkMessage baseMessage)
		{
			CombatLogManager.GenerateCombatLog(Mission.MissionNetworkHelper.GetCombatLogDataForCombatLogNetworkMessage((CombatLogNetworkMessage)baseMessage));
		}

		private void HandleServerEventConsumeWeaponAmount(GameNetworkMessage baseMessage)
		{
			ConsumeWeaponAmount consumeWeaponAmount = (ConsumeWeaponAmount)baseMessage;
			(Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(consumeWeaponAmount.SpawnedItemEntityId) as SpawnedItemEntity).ConsumeWeaponAmount(consumeWeaponAmount.ConsumedAmount);
		}

		private bool HandleClientEventSetFollowedAgent(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			SetFollowedAgent setFollowedAgent = (SetFollowedAgent)baseMessage;
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(setFollowedAgent.AgentIndex, true);
			component.FollowedAgent = agentFromIndex;
			return true;
		}

		private bool HandleClientEventSetMachineRotation(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			SetMachineRotation setMachineRotation = (SetMachineRotation)baseMessage;
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			UsableMachine usableMachine = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(setMachineRotation.UsableMachineId) as UsableMachine;
			if (component.IsControlledAgentActive && usableMachine is RangedSiegeWeapon)
			{
				RangedSiegeWeapon rangedSiegeWeapon = usableMachine as RangedSiegeWeapon;
				if (component.ControlledAgent == rangedSiegeWeapon.PilotAgent && rangedSiegeWeapon.PilotAgent != null)
				{
					rangedSiegeWeapon.AimAtRotation(setMachineRotation.HorizontalRotation, setMachineRotation.VerticalRotation);
				}
			}
			return true;
		}

		private bool HandleClientEventRequestUseObject(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			RequestUseObject requestUseObject = (RequestUseObject)baseMessage;
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			UsableMissionObject usableMissionObject = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(requestUseObject.UsableMissionObjectId) as UsableMissionObject;
			if (usableMissionObject != null && component.ControlledAgent != null && component.ControlledAgent.IsActive())
			{
				Vec3 position = component.ControlledAgent.Position;
				Vec3 globalPosition = usableMissionObject.InteractionEntity.GlobalPosition;
				float num;
				if (usableMissionObject is StandingPoint)
				{
					num = usableMissionObject.GetUserFrameForAgent(component.ControlledAgent).Origin.AsVec2.DistanceSquared(component.ControlledAgent.Position.AsVec2);
				}
				else
				{
					Vec3 vec;
					Vec3 vec2;
					usableMissionObject.InteractionEntity.GetPhysicsMinMax(true, out vec, out vec2, false);
					float num2 = globalPosition.Distance(vec);
					float num3 = globalPosition.Distance(vec2);
					float num4 = MathF.Max(num2, num3);
					num = globalPosition.Distance(new Vec3(position.x, position.y, position.z + component.ControlledAgent.GetEyeGlobalHeight(), -1f));
					num -= num4;
					num = MathF.Max(num, 0f);
				}
				if (component.ControlledAgent.CurrentlyUsedGameObject != usableMissionObject && component.ControlledAgent.CanReachAndUseObject(usableMissionObject, num * num * 0.9f * 0.9f) && component.ControlledAgent.ObjectHasVacantPosition(usableMissionObject))
				{
					component.ControlledAgent.UseGameObject(usableMissionObject, requestUseObject.UsedObjectPreferenceIndex);
				}
			}
			return true;
		}

		private bool HandleClientEventRequestStopUsingObject(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			RequestStopUsingObject requestStopUsingObject = (RequestStopUsingObject)baseMessage;
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			Agent controlledAgent = component.ControlledAgent;
			if (((controlledAgent != null) ? controlledAgent.CurrentlyUsedGameObject : null) != null)
			{
				component.ControlledAgent.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
			return true;
		}

		private bool HandleClientEventApplyOrder(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			ApplyOrder applyOrder = (ApplyOrder)baseMessage;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SetOrder(applyOrder.OrderType);
			}
			return true;
		}

		private bool HandleClientEventApplySiegeWeaponOrder(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			ApplySiegeWeaponOrder applySiegeWeaponOrder = (ApplySiegeWeaponOrder)baseMessage;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SiegeWeaponController.SetOrder(applySiegeWeaponOrder.OrderType);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithPosition(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			ApplyOrderWithPosition applyOrderWithPosition = (ApplyOrderWithPosition)baseMessage;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				WorldPosition worldPosition = new WorldPosition(base.Mission.Scene, UIntPtr.Zero, applyOrderWithPosition.Position, false);
				orderControllerOfPeer.SetOrderWithPosition(applyOrderWithPosition.OrderType, worldPosition);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithFormation(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			ApplyOrderWithFormation message = (ApplyOrderWithFormation)baseMessage;
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			OrderController orderController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent) : null);
			Formation formation = ((teamOfPeer != null) ? teamOfPeer.FormationsIncludingEmpty.SingleOrDefault((Formation f) => f.CountOfUnits > 0 && f.Index == message.FormationIndex) : null);
			if (teamOfPeer != null && orderController != null && formation != null)
			{
				orderController.SetOrderWithFormation(message.OrderType, formation);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithFormationAndPercentage(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			ApplyOrderWithFormationAndPercentage message = (ApplyOrderWithFormationAndPercentage)baseMessage;
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

		private bool HandleClientEventApplyOrderWithFormationAndNumber(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			ApplyOrderWithFormationAndNumber message = (ApplyOrderWithFormationAndNumber)baseMessage;
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

		private bool HandleClientEventApplyOrderWithTwoPositions(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			ApplyOrderWithTwoPositions applyOrderWithTwoPositions = (ApplyOrderWithTwoPositions)baseMessage;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				WorldPosition worldPosition = new WorldPosition(base.Mission.Scene, UIntPtr.Zero, applyOrderWithTwoPositions.Position1, false);
				WorldPosition worldPosition2 = new WorldPosition(base.Mission.Scene, UIntPtr.Zero, applyOrderWithTwoPositions.Position2, false);
				orderControllerOfPeer.SetOrderWithTwoPositions(applyOrderWithTwoPositions.OrderType, worldPosition, worldPosition2);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithGameEntity(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			IOrderable orderable = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(((ApplyOrderWithMissionObject)baseMessage).MissionObjectId) as IOrderable;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SetOrderWithOrderableObject(orderable);
			}
			return true;
		}

		private bool HandleClientEventApplyOrderWithAgent(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			ApplyOrderWithAgent applyOrderWithAgent = (ApplyOrderWithAgent)baseMessage;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(applyOrderWithAgent.AgentIndex, false);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SetOrderWithAgent(applyOrderWithAgent.OrderType, agentFromIndex);
			}
			return true;
		}

		private bool HandleClientEventSelectAllFormations(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			SelectAllFormations selectAllFormations = (SelectAllFormations)baseMessage;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SelectAllFormations(false);
			}
			return true;
		}

		private bool HandleClientEventSelectAllSiegeWeapons(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			SelectAllSiegeWeapons selectAllSiegeWeapons = (SelectAllSiegeWeapons)baseMessage;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.SiegeWeaponController.SelectAll();
			}
			return true;
		}

		private bool HandleClientEventClearSelectedFormations(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			ClearSelectedFormations clearSelectedFormations = (ClearSelectedFormations)baseMessage;
			OrderController orderControllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
			if (orderControllerOfPeer != null)
			{
				orderControllerOfPeer.ClearSelectedFormations();
			}
			return true;
		}

		private bool HandleClientEventSelectFormation(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			SelectFormation message = (SelectFormation)baseMessage;
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			OrderController orderController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent) : null);
			Formation formation = ((teamOfPeer != null) ? teamOfPeer.FormationsIncludingEmpty.SingleOrDefault((Formation f) => f.Index == message.FormationIndex && f.CountOfUnits > 0) : null);
			if (teamOfPeer != null && orderController != null && formation != null)
			{
				orderController.SelectFormation(formation);
			}
			return true;
		}

		private bool HandleClientEventSelectSiegeWeapon(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			SelectSiegeWeapon selectSiegeWeapon = (SelectSiegeWeapon)baseMessage;
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			SiegeWeaponController siegeWeaponController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent).SiegeWeaponController : null);
			SiegeWeapon siegeWeapon = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(selectSiegeWeapon.SiegeWeaponId) as SiegeWeapon;
			if (teamOfPeer != null && siegeWeaponController != null && siegeWeapon != null)
			{
				siegeWeaponController.Select(siegeWeapon);
			}
			return true;
		}

		private bool HandleClientEventUnselectFormation(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			UnselectFormation message = (UnselectFormation)baseMessage;
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			OrderController orderController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent) : null);
			Formation formation = ((teamOfPeer != null) ? teamOfPeer.FormationsIncludingEmpty.SingleOrDefault((Formation f) => f.CountOfUnits > 0 && f.Index == message.FormationIndex) : null);
			if (teamOfPeer != null && orderController != null && formation != null)
			{
				orderController.DeselectFormation(formation);
			}
			return true;
		}

		private bool HandleClientEventUnselectSiegeWeapon(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			UnselectSiegeWeapon unselectSiegeWeapon = (UnselectSiegeWeapon)baseMessage;
			Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
			SiegeWeaponController siegeWeaponController = ((teamOfPeer != null) ? teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent).SiegeWeaponController : null);
			SiegeWeapon siegeWeapon = Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(unselectSiegeWeapon.SiegeWeaponId) as SiegeWeapon;
			if (teamOfPeer != null && siegeWeaponController != null && siegeWeapon != null)
			{
				siegeWeaponController.Deselect(siegeWeapon);
			}
			return true;
		}

		private bool HandleClientEventDropWeapon(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			DropWeapon dropWeapon = (DropWeapon)baseMessage;
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (((component != null) ? component.ControlledAgent : null) != null && component.ControlledAgent.IsActive())
			{
				component.ControlledAgent.HandleDropWeapon(dropWeapon.IsDefendPressed, dropWeapon.ForcedSlotIndexToDropWeaponFrom);
			}
			return true;
		}

		private bool HandleClientEventCheerSelected(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			TauntSelected tauntSelected = (TauntSelected)baseMessage;
			bool flag = false;
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.HandleTaunt(tauntSelected.IndexOfTaunt, false);
				flag = true;
			}
			return flag;
		}

		private bool HandleClientEventBarkSelected(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			BarkSelected barkSelected = (BarkSelected)baseMessage;
			bool flag = false;
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.HandleBark(barkSelected.IndexOfBark);
				flag = true;
			}
			return flag;
		}

		private bool HandleClientEventBreakAgentVisualsInvulnerability(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			AgentVisualsBreakInvulnerability agentVisualsBreakInvulnerability = (AgentVisualsBreakInvulnerability)baseMessage;
			if (base.Mission == null || base.Mission.GetMissionBehavior<SpawnComponent>() == null || networkPeer.GetComponent<MissionPeer>() == null)
			{
				return false;
			}
			base.Mission.GetMissionBehavior<SpawnComponent>().SetEarlyAgentVisualsDespawning(networkPeer.GetComponent<MissionPeer>(), true);
			return true;
		}

		private bool HandleClientEventRequestToSpawnAsBot(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			RequestToSpawnAsBot requestToSpawnAsBot = (RequestToSpawnAsBot)baseMessage;
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
						GameNetwork.WriteMessage(new SetPeerTeam(networkCommunicator, component.Team.TeamIndex));
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
				GameNetwork.WriteMessage(new AddTeam(team.TeamIndex, team.Side, team.Color, team.Color2, (team.Banner != null) ? BannerCode.CreateFrom(team.Banner).Code : string.Empty, team.IsPlayerGeneral, team.IsPlayerSergeant));
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
						GameNetwork.WriteMessage(new TeamSetIsEnemyOf(team.TeamIndex, team2.TeamIndex, true));
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
							GameNetwork.WriteMessage(new InitializeFormation(formation, team.TeamIndex, formation.BannerCode));
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
								GameNetwork.WriteMessage(new AttachWeaponToAgent(agent.GetAttachedWeapon(i), agent.Index, agent.GetAttachedWeaponBoneIndex(i), agent.GetAttachedWeaponFrame(i)));
								GameNetwork.EndModuleEventAsServer();
							}
							if (!agent.IsActive())
							{
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new MakeAgentDead(agent.Index, state == AgentState.Killed, agent.GetCurrentActionValue(0)));
								GameNetwork.EndModuleEventAsServer();
							}
						}
						else if (!isMount)
						{
							MBDebug.Print("human sending " + agent.Index, 0, Debug.DebugColor.White, 17179869184UL);
							Agent agent2 = agent.MountAgent;
							if (agent2 != null && agent2.RiderAgent == null)
							{
								agent2 = null;
							}
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							int index = agent.Index;
							BasicCharacterObject character = agent.Character;
							Monster monster = agent.Monster;
							Equipment spawnEquipment = agent.SpawnEquipment;
							MissionEquipment equipment = agent.Equipment;
							BodyProperties bodyPropertiesValue = agent.BodyPropertiesValue;
							int bodyPropertiesSeed = agent.BodyPropertiesSeed;
							bool isFemale = agent.IsFemale;
							Team team = agent.Team;
							int num = ((team != null) ? team.TeamIndex : (-1));
							Formation formation = agent.Formation;
							int num2 = ((formation != null) ? formation.Index : (-1));
							uint clothingColor = agent.ClothingColor1;
							uint clothingColor2 = agent.ClothingColor2;
							int num3 = ((agent2 != null) ? agent2.Index : (-1));
							Agent mountAgent = agent.MountAgent;
							Equipment equipment2 = ((mountAgent != null) ? mountAgent.SpawnEquipment : null);
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
							GameNetwork.WriteMessage(new CreateAgent(index, character, monster, spawnEquipment, equipment, bodyPropertiesValue, bodyPropertiesSeed, isFemale, num, num2, clothingColor, clothingColor2, num3, equipment2, flag, position, movementDirection, networkCommunicator));
							GameNetwork.EndModuleEventAsServer();
							agent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
							if (agent2 != null)
							{
								agent2.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
							}
							for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
							{
								for (int j = 0; j < agent.Equipment[equipmentIndex].GetAttachedWeaponsCount(); j++)
								{
									GameNetwork.BeginModuleEventAsServer(networkPeer);
									GameNetwork.WriteMessage(new AttachWeaponToWeaponInAgentEquipmentSlot(agent.Equipment[equipmentIndex].GetAttachedWeapon(j), agent.Index, equipmentIndex, agent.Equipment[equipmentIndex].GetAttachedWeaponFrame(j)));
									GameNetwork.EndModuleEventAsServer();
								}
							}
							int num4 = agent.GetAttachedWeaponsCount();
							for (int k = 0; k < num4; k++)
							{
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new AttachWeaponToAgent(agent.GetAttachedWeapon(k), agent.Index, agent.GetAttachedWeaponBoneIndex(k), agent.GetAttachedWeaponFrame(k)));
								GameNetwork.EndModuleEventAsServer();
							}
							if (agent2 != null)
							{
								num4 = agent2.GetAttachedWeaponsCount();
								for (int l = 0; l < num4; l++)
								{
									GameNetwork.BeginModuleEventAsServer(networkPeer);
									GameNetwork.WriteMessage(new AttachWeaponToAgent(agent2.GetAttachedWeapon(l), agent2.Index, agent2.GetAttachedWeaponBoneIndex(l), agent2.GetAttachedWeaponFrame(l)));
									GameNetwork.EndModuleEventAsServer();
								}
							}
							EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
							int num5 = ((wieldedItemIndex != EquipmentIndex.None) ? agent.Equipment[wieldedItemIndex].CurrentUsageIndex : 0);
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new SetWieldedItemIndex(agent.Index, false, true, true, wieldedItemIndex, num5));
							GameNetwork.EndModuleEventAsServer();
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new SetWieldedItemIndex(agent.Index, true, true, true, agent.GetWieldedItemIndex(Agent.HandIndex.OffHand), num5));
							GameNetwork.EndModuleEventAsServer();
							MBActionSet actionSet = agent.ActionSet;
							if (actionSet.IsValid)
							{
								AnimationSystemData animationSystemData = agent.Monster.FillAnimationSystemData(actionSet, agent.Character.GetStepSize(), false);
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new SetAgentActionSet(agent.Index, animationSystemData));
								GameNetwork.EndModuleEventAsServer();
								if (!agent.IsActive())
								{
									GameNetwork.BeginModuleEventAsServer(networkPeer);
									GameNetwork.WriteMessage(new MakeAgentDead(agent.Index, state == AgentState.Killed, agent.GetCurrentActionValue(0)));
									GameNetwork.EndModuleEventAsServer();
								}
							}
							else
							{
								Debug.FailedAssert("Checking to see if we enter this condition.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MissionNetworkComponent.cs", "SendAgentsToPeer", 1975);
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new MakeAgentDead(agent.Index, state == AgentState.Killed, ActionIndexValueCache.act_none));
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
							GameNetwork.WriteMessage(new SpawnWeaponWithNewEntity(spawnedItemEntity.WeaponCopy, weaponSpawnFlags, spawnedItemEntity.Id.Id, matrixFrame, (missionObject2 != null) ? missionObject2.Id : MissionObjectId.Invalid, flag2, flag));
							GameNetwork.EndModuleEventAsServer();
							for (int i = 0; i < spawnedItemEntity.WeaponCopy.GetAttachedWeaponsCount(); i++)
							{
								GameNetwork.BeginModuleEventAsServer(networkPeer);
								GameNetwork.WriteMessage(new AttachWeaponToSpawnedWeapon(spawnedItemEntity.WeaponCopy.GetAttachedWeapon(i), spawnedItemEntity.Id, spawnedItemEntity.WeaponCopy.GetAttachedWeaponFrame(i)));
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
									GameNetwork.WriteMessage(new SpawnAttachedWeaponOnSpawnedWeapon(spawnedItemEntity.Id, i, gameEntity.GetChild(i).GetFirstScriptOfType<SpawnedItemEntity>().Id.Id));
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
				int index = missile.Index;
				int index2 = missile.ShooterAgent.Index;
				EquipmentIndex equipmentIndex = EquipmentIndex.None;
				MissionWeapon weapon = missile.Weapon;
				Vec3 position = missile.GetPosition();
				Vec3 vec = velocity;
				float num2 = num;
				Mat3 mat = identity;
				bool hasRigidBody = missile.GetHasRigidBody();
				MissionObject missionObjectToIgnore = missile.MissionObjectToIgnore;
				GameNetwork.WriteMessage(new CreateMissile(index, index2, equipmentIndex, weapon, position, vec, num2, mat, hasRigidBody, (missionObjectToIgnore != null) ? missionObjectToIgnore.Id : MissionObjectId.Invalid, false));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		public override void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component != null && component.HasSpawnedAgentVisuals)
			{
				base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, false);
				component.HasSpawnedAgentVisuals = false;
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
				foreach (NetworkCommunicator networkCommunicator2 in GameNetwork.DisconnectedNetworkPeers)
				{
					networkCommunicator2.VirtualPlayer.SynchronizeComponentsTo(networkPeer.VirtualPlayer);
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
				if (mission != null)
				{
					mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, true);
				}
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new RemoveAgentVisualsForPeer(component.GetNetworkPeer()));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
				component.HasSpawnedAgentVisuals = false;
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
					blow.GlobalPosition = controlledAgent.Position;
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
				GameNetwork.WriteMessage(new AddTeam(team.TeamIndex, team.Side, team.Color, team.Color2, (team.Banner != null) ? BannerCode.CreateFrom(team.Banner).Code : string.Empty, team.IsPlayerGeneral, team.IsPlayerSergeant));
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
				if (GameNetwork.IsServer && !networkCommunicator.IsServerPeer && !MultiplayerOptions.OptionType.DisableInactivityKick.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions))
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
