using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002C9 RID: 713
	public class DuelSpawningBehavior : SpawningBehaviorBase
	{
		// Token: 0x0600270A RID: 9994 RVA: 0x00093DFD File Offset: 0x00091FFD
		public DuelSpawningBehavior()
		{
			this.IsSpawningEnabled = true;
		}

		// Token: 0x0600270B RID: 9995 RVA: 0x00093E0C File Offset: 0x0009200C
		public override void Initialize(SpawnComponent spawnComponent)
		{
			base.Initialize(spawnComponent);
			base.OnPeerSpawnedFromVisuals += this.OnPeerSpawned;
		}

		// Token: 0x0600270C RID: 9996 RVA: 0x00093E27 File Offset: 0x00092027
		public override void Clear()
		{
			base.Clear();
			base.OnPeerSpawnedFromVisuals -= this.OnPeerSpawned;
		}

		// Token: 0x0600270D RID: 9997 RVA: 0x00093E41 File Offset: 0x00092041
		public override void OnTick(float dt)
		{
			if (this.IsSpawningEnabled && this._spawnCheckTimer.Check(Mission.Current.CurrentTime))
			{
				this.SpawnAgents();
			}
			base.OnTick(dt);
		}

		// Token: 0x0600270E RID: 9998 RVA: 0x00093E70 File Offset: 0x00092070
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

		// Token: 0x0600270F RID: 9999 RVA: 0x000940C8 File Offset: 0x000922C8
		public override bool AllowEarlyAgentVisualsDespawning(MissionPeer missionPeer)
		{
			return true;
		}

		// Token: 0x06002710 RID: 10000 RVA: 0x000940CB File Offset: 0x000922CB
		protected override bool IsRoundInProgress()
		{
			return Mission.Current.CurrentState == Mission.State.Continuing;
		}

		// Token: 0x06002711 RID: 10001 RVA: 0x000940DA File Offset: 0x000922DA
		private void OnPeerSpawned(MissionPeer peer)
		{
			MissionRepresentativeBase representative = peer.Representative;
		}
	}
}
