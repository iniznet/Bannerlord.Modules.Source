using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade
{
	public class DuelSpawningBehavior : SpawningBehaviorBase
	{
		public DuelSpawningBehavior()
		{
			this.IsSpawningEnabled = true;
		}

		public override void Initialize(SpawnComponent spawnComponent)
		{
			base.Initialize(spawnComponent);
			base.OnPeerSpawnedFromVisuals += this.OnPeerSpawned;
		}

		public override void Clear()
		{
			base.Clear();
			base.OnPeerSpawnedFromVisuals -= this.OnPeerSpawned;
		}

		public override void OnTick(float dt)
		{
			if (this.IsSpawningEnabled && this._spawnCheckTimer.Check(Mission.Current.CurrentTime))
			{
				this.SpawnAgents();
			}
			base.OnTick(dt);
		}

		protected override void SpawnAgents()
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (networkCommunicator.IsSynchronized)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component.Representative is DuelMissionRepresentative && networkCommunicator.IsSynchronized && component.ControlledAgent == null && !component.HasSpawnedAgentVisuals && component.Team != null && component.Team != base.Mission.SpectatorTeam && component.TeamInitialPerkInfoReady && component.Culture != null && component.SpawnTimer.Check(Mission.Current.CurrentTime))
					{
						MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(component, false);
						if (mpheroClassForPeer == null)
						{
							if (component.SelectedTroopIndex != 0)
							{
								component.SelectedTroopIndex = 0;
								GameNetwork.BeginBroadcastModuleEvent();
								GameNetwork.WriteMessage(new UpdateSelectedTroopIndex(networkCommunicator, 0));
								GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, networkCommunicator);
							}
						}
						else
						{
							BasicCharacterObject heroCharacter = mpheroClassForPeer.HeroCharacter;
							Equipment equipment = heroCharacter.Equipment.Clone(false);
							MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(component);
							IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> enumerable = ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetAlternativeEquipments(true) : null);
							if (enumerable != null)
							{
								foreach (ValueTuple<EquipmentIndex, EquipmentElement> valueTuple in enumerable)
								{
									equipment[valueTuple.Item1] = valueTuple.Item2;
								}
							}
							AgentBuildData agentBuildData = new AgentBuildData(heroCharacter).MissionPeer(component).Equipment(equipment).Team(component.Team)
								.TroopOrigin(new BasicBattleAgentOrigin(heroCharacter))
								.IsFemale(component.Peer.IsFemale)
								.BodyProperties(base.GetBodyProperties(component, component.Culture))
								.VisualsIndex(0)
								.ClothingColor1(component.Culture.Color)
								.ClothingColor2(component.Culture.Color2);
							if (this.GameMode.ShouldSpawnVisualsForServer(networkCommunicator))
							{
								base.AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(component, agentBuildData, component.SelectedTroopIndex, false, 0);
							}
							this.GameMode.HandleAgentVisualSpawning(networkCommunicator, agentBuildData, 0, true);
						}
					}
				}
			}
		}

		public override bool AllowEarlyAgentVisualsDespawning(MissionPeer missionPeer)
		{
			return true;
		}

		protected override bool IsRoundInProgress()
		{
			return Mission.Current.CurrentState == Mission.State.Continuing;
		}

		private void OnPeerSpawned(MissionPeer peer)
		{
			MissionRepresentativeBase representative = peer.Representative;
		}
	}
}
