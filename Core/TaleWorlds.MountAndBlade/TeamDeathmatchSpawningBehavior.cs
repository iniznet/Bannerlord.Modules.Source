using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class TeamDeathmatchSpawningBehavior : SpawningBehaviorBase
	{
		public TeamDeathmatchSpawningBehavior()
		{
			this.IsSpawningEnabled = true;
		}

		public override void Initialize(SpawnComponent spawnComponent)
		{
			base.Initialize(spawnComponent);
			base.OnAllAgentsFromPeerSpawnedFromVisuals += this.OnAllAgentsFromPeerSpawnedFromVisuals;
		}

		public override void Clear()
		{
			base.Clear();
			base.OnAllAgentsFromPeerSpawnedFromVisuals -= this.OnAllAgentsFromPeerSpawnedFromVisuals;
		}

		public override void OnTick(float dt)
		{
			if (this.IsSpawningEnabled && this._spawnCheckTimer.Check(base.Mission.CurrentTime))
			{
				this.SpawnAgents();
			}
			base.OnTick(dt);
		}

		protected override void SpawnAgents()
		{
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			BasicCultureObject object2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (networkCommunicator.IsSynchronized)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && component.ControlledAgent == null && !component.HasSpawnedAgentVisuals && component.Team != null && component.Team != base.Mission.SpectatorTeam && component.TeamInitialPerkInfoReady && component.SpawnTimer.Check(base.Mission.CurrentTime))
					{
						BasicCultureObject basicCultureObject = ((component.Team.Side == BattleSideEnum.Attacker) ? @object : object2);
						MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(component, false);
						if (mpheroClassForPeer == null || mpheroClassForPeer.TroopCasualCost > this.GameMode.GetCurrentGoldForPeer(component))
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
								.BodyProperties(base.GetBodyProperties(component, (component.Team == base.Mission.AttackerTeam) ? @object : object2))
								.VisualsIndex(0)
								.ClothingColor1((component.Team == base.Mission.AttackerTeam) ? basicCultureObject.Color : basicCultureObject.ClothAlternativeColor)
								.ClothingColor2((component.Team == base.Mission.AttackerTeam) ? basicCultureObject.Color2 : basicCultureObject.ClothAlternativeColor2);
							if (this.GameMode.ShouldSpawnVisualsForServer(networkCommunicator))
							{
								base.AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(component, agentBuildData, component.SelectedTroopIndex, false, 0);
							}
							this.GameMode.HandleAgentVisualSpawning(networkCommunicator, agentBuildData, 0, true);
						}
					}
				}
			}
			if (base.Mission.AttackerTeam != null)
			{
				int num = 0;
				foreach (Agent agent in base.Mission.AttackerTeam.ActiveAgents)
				{
					if (agent.Character != null && agent.MissionPeer == null)
					{
						num++;
					}
				}
				if (num < MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions))
				{
					base.SpawnBot(base.Mission.AttackerTeam, @object);
				}
			}
			if (base.Mission.DefenderTeam != null)
			{
				int num2 = 0;
				foreach (Agent agent2 in base.Mission.DefenderTeam.ActiveAgents)
				{
					if (agent2.Character != null && agent2.MissionPeer == null)
					{
						num2++;
					}
				}
				if (num2 < MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions))
				{
					base.SpawnBot(base.Mission.DefenderTeam, object2);
				}
			}
		}

		public override bool AllowEarlyAgentVisualsDespawning(MissionPeer lobbyPeer)
		{
			return true;
		}

		public override int GetMaximumReSpawnPeriodForPeer(MissionPeer peer)
		{
			if (this.GameMode.WarmupComponent != null && this.GameMode.WarmupComponent.IsInWarmup)
			{
				return 3;
			}
			if (peer.Team != null)
			{
				if (peer.Team.Side == BattleSideEnum.Attacker)
				{
					return MultiplayerOptions.OptionType.RespawnPeriodTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
				if (peer.Team.Side == BattleSideEnum.Defender)
				{
					return MultiplayerOptions.OptionType.RespawnPeriodTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
			}
			return -1;
		}

		protected override bool IsRoundInProgress()
		{
			return Mission.Current.CurrentState == Mission.State.Continuing;
		}

		private new void OnAllAgentsFromPeerSpawnedFromVisuals(MissionPeer peer)
		{
			bool flag = peer.Team == base.Mission.AttackerTeam;
			Team defenderTeam = base.Mission.DefenderTeam;
			MultiplayerClassDivisions.MPHeroClass mpheroClass = MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(flag ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions))).ElementAt(peer.SelectedTroopIndex);
			this.GameMode.ChangeCurrentGoldForPeer(peer, this.GameMode.GetCurrentGoldForPeer(peer) - mpheroClass.TroopCasualCost);
		}
	}
}
