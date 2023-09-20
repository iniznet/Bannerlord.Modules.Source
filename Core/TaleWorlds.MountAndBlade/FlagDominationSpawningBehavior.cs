using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class FlagDominationSpawningBehavior : SpawningBehaviorBase
	{
		public FlagDominationSpawningBehavior()
		{
			this._enforcedSpawnTimers = new List<KeyValuePair<MissionPeer, Timer>>();
		}

		public override void Initialize(SpawnComponent spawnComponent)
		{
			base.Initialize(spawnComponent);
			this._flagDominationMissionController = base.Mission.GetMissionBehavior<MissionMultiplayerFlagDomination>();
			this._roundController = base.Mission.GetMissionBehavior<MultiplayerRoundController>();
			this._roundController.OnRoundStarted += this.RequestStartSpawnSession;
			this._roundController.OnRoundEnding += base.RequestStopSpawnSession;
			this._roundController.OnRoundEnding += base.SetRemainingAgentsInvulnerable;
			if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) == 0)
			{
				this._roundController.EnableEquipmentUpdate();
			}
			base.OnAllAgentsFromPeerSpawnedFromVisuals += this.OnAllAgentsFromPeerSpawnedFromVisuals;
			base.OnPeerSpawnedFromVisuals += this.OnPeerSpawnedFromVisuals;
		}

		public override void Clear()
		{
			base.Clear();
			this._roundController.OnRoundStarted -= this.RequestStartSpawnSession;
			this._roundController.OnRoundEnding -= base.SetRemainingAgentsInvulnerable;
			this._roundController.OnRoundEnding -= base.RequestStopSpawnSession;
			base.OnAllAgentsFromPeerSpawnedFromVisuals -= this.OnAllAgentsFromPeerSpawnedFromVisuals;
			base.OnPeerSpawnedFromVisuals -= this.OnPeerSpawnedFromVisuals;
		}

		public override void OnTick(float dt)
		{
			if (this._spawningTimerTicking)
			{
				this._spawningTimer += dt;
			}
			if (this.IsSpawningEnabled)
			{
				if (!this._roundInitialSpawnOver && this.IsRoundInProgress())
				{
					foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
					{
						MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
						if (((component != null) ? component.Team : null) != null && component.Team.Side != BattleSideEnum.None)
						{
							this.SpawnComponent.SetEarlyAgentVisualsDespawning(component, true);
						}
					}
					this._roundInitialSpawnOver = true;
					base.Mission.AllowAiTicking = true;
				}
				this.SpawnAgents();
				if (this._roundInitialSpawnOver && this._flagDominationMissionController.GameModeUsesSingleSpawning && this._spawningTimer > (float)MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions))
				{
					this.IsSpawningEnabled = false;
					this._spawningTimer = 0f;
					this._spawningTimerTicking = false;
				}
			}
			base.OnTick(dt);
		}

		public override void RequestStartSpawnSession()
		{
			if (!this.IsSpawningEnabled)
			{
				Mission.Current.SetBattleAgentCount(-1);
				this.IsSpawningEnabled = true;
				this._haveBotsBeenSpawned = false;
				this._spawningTimerTicking = true;
				base.ResetSpawnCounts();
				base.ResetSpawnTimers();
			}
		}

		protected override void SpawnAgents()
		{
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			BasicCultureObject object2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			if (!this._haveBotsBeenSpawned && (MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0 || MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0))
			{
				Mission.Current.AllowAiTicking = false;
				List<string> list = new List<string> { "11.8.1.4345.4345.770.774.1.0.0.133.7.5.512.512.784.769.1.0.0", "11.8.1.4345.4345.770.774.1.0.0.156.7.5.512.512.784.769.1.0.0", "11.8.1.4345.4345.770.774.1.0.0.155.7.5.512.512.784.769.1.0.0", "11.8.1.4345.4345.770.774.1.0.0.158.7.5.512.512.784.769.1.0.0", "11.8.1.4345.4345.770.774.1.0.0.118.7.5.512.512.784.769.1.0.0", "11.8.1.4345.4345.770.774.1.0.0.149.7.5.512.512.784.769.1.0.0" };
				foreach (Team team in base.Mission.Teams)
				{
					if (base.Mission.AttackerTeam == team || base.Mission.DefenderTeam == team)
					{
						BasicCultureObject teamCulture = ((team == base.Mission.AttackerTeam) ? @object : object2);
						int num = ((base.Mission.AttackerTeam == team) ? MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
						int num2 = 0;
						Func<MultiplayerClassDivisions.MPHeroClass, bool> <>9__1;
						for (int i = 0; i < num; i++)
						{
							Formation formation = null;
							if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
							{
								while (formation == null || formation.PlayerOwner != null)
								{
									FormationClass formationClass = (FormationClass)num2;
									formation = team.GetFormation(formationClass);
									num2++;
								}
							}
							if (formation != null)
							{
								formation.BannerCode = list[num2 - 1];
							}
							MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> mpheroClasses = MultiplayerClassDivisions.GetMPHeroClasses();
							Func<MultiplayerClassDivisions.MPHeroClass, bool> func;
							if ((func = <>9__1) == null)
							{
								func = (<>9__1 = (MultiplayerClassDivisions.MPHeroClass x) => x.Culture == teamCulture);
							}
							MultiplayerClassDivisions.MPHeroClass randomElementWithPredicate = mpheroClasses.GetRandomElementWithPredicate(func);
							BasicCharacterObject heroCharacter = randomElementWithPredicate.HeroCharacter;
							AgentBuildData agentBuildData = new AgentBuildData(heroCharacter).Equipment(randomElementWithPredicate.HeroCharacter.Equipment).TroopOrigin(new BasicBattleAgentOrigin(heroCharacter)).EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(heroCharacter, 0))
								.Team(team)
								.VisualsIndex(0)
								.Formation(formation)
								.IsFemale(heroCharacter.IsFemale)
								.ClothingColor1((team.Side == BattleSideEnum.Attacker) ? teamCulture.Color : teamCulture.ClothAlternativeColor)
								.ClothingColor2((team.Side == BattleSideEnum.Attacker) ? teamCulture.Color2 : teamCulture.ClothAlternativeColor2);
							if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) == 0)
							{
								MatrixFrame spawnFrame = this.SpawnComponent.GetSpawnFrame(team, randomElementWithPredicate.HeroCharacter.Equipment[EquipmentIndex.ArmorItemEndSlot].Item != null, true);
								agentBuildData.InitialPosition(spawnFrame.origin);
								AgentBuildData agentBuildData2 = agentBuildData;
								Vec2 vec = spawnFrame.rotation.f.AsVec2;
								vec = vec.Normalized();
								agentBuildData2.InitialDirection(vec);
							}
							agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentRace, agentBuildData.AgentIsFemale, heroCharacter.GetBodyPropertiesMin(false), heroCharacter.GetBodyPropertiesMax(), (int)agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, heroCharacter.HairTags, heroCharacter.BeardTags, heroCharacter.TattooTags));
							Agent agent = base.Mission.SpawnAgent(agentBuildData, false);
							agent.SetWatchState(Agent.WatchState.Alarmed);
							agent.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp);
							if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
							{
								int num3 = MathF.Ceiling((float)MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) * randomElementWithPredicate.TroopMultiplier);
								for (int j = 0; j < num3; j++)
								{
									this.SpawnBotInBotFormation(j + 1, team, teamCulture, randomElementWithPredicate.TroopCharacter, formation);
								}
								this.BotFormationSpawned(team);
								formation.SetControlledByAI(true, false);
							}
						}
						if (num > 0)
						{
							if (team.FormationsIncludingEmpty.AnyQ((Formation f) => f.CountOfUnits > 0))
							{
								TeamAIGeneral teamAIGeneral = new TeamAIGeneral(Mission.Current, team, 10f, 1f);
								teamAIGeneral.AddTacticOption(new TacticSergeantMPBotTactic(team));
								team.AddTeamAI(teamAIGeneral, false);
							}
						}
					}
				}
				this.AllBotFormationsSpawned();
				this._haveBotsBeenSpawned = true;
			}
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (networkCommunicator.IsSynchronized && component.Team != null && component.Team.Side != BattleSideEnum.None && (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) != 0 || !this.CheckIfEnforcedSpawnTimerExpiredForPeer(component)))
				{
					Team team2 = component.Team;
					bool flag = team2 == base.Mission.AttackerTeam;
					Team defenderTeam = base.Mission.DefenderTeam;
					BasicCultureObject basicCultureObject = (flag ? @object : object2);
					MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(component, false);
					int num4 = ((this._flagDominationMissionController.GetMissionType() == MissionLobbyComponent.MultiplayerGameType.Battle) ? mpheroClassForPeer.TroopBattleCost : mpheroClassForPeer.TroopCost);
					if (component.ControlledAgent == null && !component.HasSpawnedAgentVisuals && component.Team != null && component.Team != base.Mission.SpectatorTeam && component.TeamInitialPerkInfoReady && component.SpawnTimer.Check(base.Mission.CurrentTime))
					{
						int currentGoldForPeer = this._flagDominationMissionController.GetCurrentGoldForPeer(component);
						if (mpheroClassForPeer == null || (this._flagDominationMissionController.UseGold() && num4 > currentGoldForPeer))
						{
							if (currentGoldForPeer >= MultiplayerClassDivisions.GetMinimumTroopCost(basicCultureObject) && component.SelectedTroopIndex != 0)
							{
								component.SelectedTroopIndex = 0;
								GameNetwork.BeginBroadcastModuleEvent();
								GameNetwork.WriteMessage(new UpdateSelectedTroopIndex(networkCommunicator, 0));
								GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, networkCommunicator);
							}
						}
						else
						{
							if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) == 0)
							{
								this.CreateEnforcedSpawnTimerForPeer(component, 15);
							}
							Formation formation2 = component.ControlledFormation;
							if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0 && formation2 == null)
							{
								FormationClass formationIndex = component.Team.FormationsIncludingEmpty.First((Formation x) => x.PlayerOwner == null && !x.ContainsAgentVisuals && x.CountOfUnits == 0).FormationIndex;
								formation2 = team2.GetFormation(formationIndex);
								formation2.ContainsAgentVisuals = true;
								if (string.IsNullOrEmpty(formation2.BannerCode))
								{
									formation2.BannerCode = component.Peer.BannerCode;
								}
							}
							BasicCharacterObject heroCharacter2 = mpheroClassForPeer.HeroCharacter;
							AgentBuildData agentBuildData3 = new AgentBuildData(heroCharacter2).MissionPeer(component).Team(component.Team).VisualsIndex(0)
								.Formation(formation2)
								.MakeUnitStandOutOfFormationDistance(7f)
								.IsFemale(component.Peer.IsFemale)
								.BodyProperties(base.GetBodyProperties(component, (component.Team == base.Mission.AttackerTeam) ? @object : object2))
								.ClothingColor1((team2 == base.Mission.AttackerTeam) ? basicCultureObject.Color : basicCultureObject.ClothAlternativeColor)
								.ClothingColor2((team2 == base.Mission.AttackerTeam) ? basicCultureObject.Color2 : basicCultureObject.ClothAlternativeColor2);
							MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(component);
							Equipment equipment = heroCharacter2.Equipment.Clone(false);
							IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> enumerable = ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetAlternativeEquipments(true) : null);
							if (enumerable != null)
							{
								foreach (ValueTuple<EquipmentIndex, EquipmentElement> valueTuple in enumerable)
								{
									equipment[valueTuple.Item1] = valueTuple.Item2;
								}
							}
							int amountOfAgentVisualsForPeer = component.GetAmountOfAgentVisualsForPeer();
							bool flag2 = amountOfAgentVisualsForPeer > 0;
							agentBuildData3.Equipment(equipment);
							if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) == 0)
							{
								if (!flag2)
								{
									MatrixFrame spawnFrame2 = this.SpawnComponent.GetSpawnFrame(component.Team, equipment[EquipmentIndex.ArmorItemEndSlot].Item != null, true);
									agentBuildData3.InitialPosition(spawnFrame2.origin);
									AgentBuildData agentBuildData4 = agentBuildData3;
									Vec2 vec = spawnFrame2.rotation.f.AsVec2;
									vec = vec.Normalized();
									agentBuildData4.InitialDirection(vec);
								}
								else
								{
									MatrixFrame frame = component.GetAgentVisualForPeer(0).GetFrame();
									agentBuildData3.InitialPosition(frame.origin);
									AgentBuildData agentBuildData5 = agentBuildData3;
									Vec2 vec = frame.rotation.f.AsVec2;
									vec = vec.Normalized();
									agentBuildData5.InitialDirection(vec);
								}
							}
							if (this.GameMode.ShouldSpawnVisualsForServer(networkCommunicator))
							{
								base.AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(component, agentBuildData3, component.SelectedTroopIndex, false, 0);
							}
							this.GameMode.HandleAgentVisualSpawning(networkCommunicator, agentBuildData3, 0, true);
							component.ControlledFormation = formation2;
							if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
							{
								int troopCount = MPPerkObject.GetTroopCount(mpheroClassForPeer, onSpawnPerkHandler);
								IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> enumerable2 = ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetAlternativeEquipments(false) : null);
								for (int k = 0; k < troopCount; k++)
								{
									if (k + 1 >= amountOfAgentVisualsForPeer)
									{
										flag2 = false;
									}
									this.SpawnBotVisualsInPlayerFormation(component, k + 1, team2, basicCultureObject, mpheroClassForPeer.TroopCharacter.StringId, formation2, flag2, troopCount, enumerable2);
								}
							}
						}
					}
				}
			}
		}

		private new void OnPeerSpawnedFromVisuals(MissionPeer peer)
		{
			if (peer.ControlledFormation != null)
			{
				peer.ControlledAgent.Team.AssignPlayerAsSergeantOfFormation(peer, peer.ControlledFormation.FormationIndex);
			}
		}

		private new void OnAllAgentsFromPeerSpawnedFromVisuals(MissionPeer peer)
		{
			if (peer.ControlledFormation != null)
			{
				peer.ControlledFormation.OnFormationDispersed();
				peer.ControlledFormation.SetMovementOrder(MovementOrder.MovementOrderFollow(peer.ControlledAgent));
				NetworkCommunicator networkPeer = peer.GetNetworkPeer();
				if (peer.BotsUnderControlAlive != 0 || peer.BotsUnderControlTotal != 0)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new BotsControlledChange(networkPeer, peer.BotsUnderControlAlive, peer.BotsUnderControlTotal));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					base.Mission.GetMissionBehavior<MissionMultiplayerGameModeFlagDominationClient>().OnBotsControlledChanged(peer, peer.BotsUnderControlAlive, peer.BotsUnderControlTotal);
				}
				if (peer.Team == base.Mission.AttackerTeam)
				{
					base.Mission.NumOfFormationsSpawnedTeamOne++;
				}
				else
				{
					base.Mission.NumOfFormationsSpawnedTeamTwo++;
				}
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetSpawnedFormationCount(base.Mission.NumOfFormationsSpawnedTeamOne, base.Mission.NumOfFormationsSpawnedTeamTwo));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
			if (this._flagDominationMissionController.UseGold())
			{
				bool flag = peer.Team == base.Mission.AttackerTeam;
				Team defenderTeam = base.Mission.DefenderTeam;
				MultiplayerClassDivisions.MPHeroClass mpheroClass = MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(flag ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions))).ElementAt(peer.SelectedTroopIndex);
				int num = ((this._flagDominationMissionController.GetMissionType() == MissionLobbyComponent.MultiplayerGameType.Battle) ? mpheroClass.TroopBattleCost : mpheroClass.TroopCost);
				this._flagDominationMissionController.ChangeCurrentGoldForPeer(peer, this._flagDominationMissionController.GetCurrentGoldForPeer(peer) - num);
			}
		}

		private void BotFormationSpawned(Team team)
		{
			if (team == base.Mission.AttackerTeam)
			{
				base.Mission.NumOfFormationsSpawnedTeamOne++;
				return;
			}
			if (team == base.Mission.DefenderTeam)
			{
				base.Mission.NumOfFormationsSpawnedTeamTwo++;
			}
		}

		private void AllBotFormationsSpawned()
		{
			if (base.Mission.NumOfFormationsSpawnedTeamOne != 0 || base.Mission.NumOfFormationsSpawnedTeamTwo != 0)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetSpawnedFormationCount(base.Mission.NumOfFormationsSpawnedTeamOne, base.Mission.NumOfFormationsSpawnedTeamTwo));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
		}

		public override bool AllowEarlyAgentVisualsDespawning(MissionPeer lobbyPeer)
		{
			if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) != 0)
			{
				return false;
			}
			if (!this._roundController.IsRoundInProgress)
			{
				return false;
			}
			if (!lobbyPeer.HasSpawnTimerExpired && lobbyPeer.SpawnTimer.Check(Mission.Current.CurrentTime))
			{
				lobbyPeer.HasSpawnTimerExpired = true;
			}
			return lobbyPeer.HasSpawnTimerExpired;
		}

		protected override bool IsRoundInProgress()
		{
			return this._roundController.IsRoundInProgress;
		}

		private void CreateEnforcedSpawnTimerForPeer(MissionPeer peer, int durationInSeconds)
		{
			if (this._enforcedSpawnTimers.Any((KeyValuePair<MissionPeer, Timer> pair) => pair.Key == peer))
			{
				return;
			}
			this._enforcedSpawnTimers.Add(new KeyValuePair<MissionPeer, Timer>(peer, new Timer(base.Mission.CurrentTime, (float)durationInSeconds, true)));
			Debug.Print(string.Concat(new object[] { "EST for ", peer.Name, " set to ", durationInSeconds, " seconds." }), 0, Debug.DebugColor.Yellow, 64UL);
		}

		private bool CheckIfEnforcedSpawnTimerExpiredForPeer(MissionPeer peer)
		{
			KeyValuePair<MissionPeer, Timer> keyValuePair = this._enforcedSpawnTimers.FirstOrDefault((KeyValuePair<MissionPeer, Timer> pr) => pr.Key == peer);
			if (keyValuePair.Key == null)
			{
				return false;
			}
			if (peer.ControlledAgent != null)
			{
				this._enforcedSpawnTimers.RemoveAll((KeyValuePair<MissionPeer, Timer> p) => p.Key == peer);
				Debug.Print("EST for " + peer.Name + " is no longer valid (spawned already).", 0, Debug.DebugColor.Yellow, 64UL);
				return false;
			}
			Timer value = keyValuePair.Value;
			if (peer.HasSpawnedAgentVisuals && value.Check(Mission.Current.CurrentTime))
			{
				this.SpawnComponent.SetEarlyAgentVisualsDespawning(peer, true);
				this._enforcedSpawnTimers.RemoveAll((KeyValuePair<MissionPeer, Timer> p) => p.Key == peer);
				Debug.Print("EST for " + peer.Name + " has expired.", 0, Debug.DebugColor.Yellow, 64UL);
				return true;
			}
			return false;
		}

		public override void OnClearScene()
		{
			base.OnClearScene();
			this._enforcedSpawnTimers.Clear();
			this._roundInitialSpawnOver = false;
		}

		protected void SpawnBotInBotFormation(int visualsIndex, Team agentTeam, BasicCultureObject cultureLimit, BasicCharacterObject character, Formation formation)
		{
			AgentBuildData agentBuildData = new AgentBuildData(character).Team(agentTeam).TroopOrigin(new BasicBattleAgentOrigin(character)).VisualsIndex(visualsIndex)
				.EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(character, visualsIndex))
				.Formation(formation)
				.IsFemale(character.IsFemale)
				.ClothingColor1((agentTeam.Side == BattleSideEnum.Attacker) ? cultureLimit.Color : cultureLimit.ClothAlternativeColor)
				.ClothingColor2((agentTeam.Side == BattleSideEnum.Attacker) ? cultureLimit.Color2 : cultureLimit.ClothAlternativeColor2);
			agentBuildData.Equipment(Equipment.GetRandomEquipmentElements(character, !(Game.Current.GameType is MultiplayerGame), false, agentBuildData.AgentEquipmentSeed));
			agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentRace, agentBuildData.AgentIsFemale, character.GetBodyPropertiesMin(false), character.GetBodyPropertiesMax(), (int)agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, character.HairTags, character.BeardTags, character.TattooTags));
			base.Mission.SpawnAgent(agentBuildData, false).AIStateFlags |= Agent.AIStateFlag.Alarmed;
		}

		protected void SpawnBotVisualsInPlayerFormation(MissionPeer missionPeer, int visualsIndex, Team agentTeam, BasicCultureObject cultureLimit, string troopName, Formation formation, bool updateExistingAgentVisuals, int totalCount, IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments)
		{
			BasicCharacterObject @object = MBObjectManager.Instance.GetObject<BasicCharacterObject>(troopName);
			AgentBuildData agentBuildData = new AgentBuildData(@object).Team(agentTeam).OwningMissionPeer(missionPeer).VisualsIndex(visualsIndex)
				.TroopOrigin(new BasicBattleAgentOrigin(@object))
				.EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(@object, visualsIndex))
				.Formation(formation)
				.IsFemale(@object.IsFemale)
				.ClothingColor1((agentTeam.Side == BattleSideEnum.Attacker) ? cultureLimit.Color : cultureLimit.ClothAlternativeColor)
				.ClothingColor2((agentTeam.Side == BattleSideEnum.Attacker) ? cultureLimit.Color2 : cultureLimit.ClothAlternativeColor2);
			Equipment randomEquipmentElements = Equipment.GetRandomEquipmentElements(@object, !(Game.Current.GameType is MultiplayerGame), false, MBRandom.RandomInt());
			if (alternativeEquipments != null)
			{
				foreach (ValueTuple<EquipmentIndex, EquipmentElement> valueTuple in alternativeEquipments)
				{
					randomEquipmentElements[valueTuple.Item1] = valueTuple.Item2;
				}
			}
			agentBuildData.Equipment(randomEquipmentElements);
			agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentRace, agentBuildData.AgentIsFemale, @object.GetBodyPropertiesMin(false), @object.GetBodyPropertiesMax(), (int)agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, @object.HairTags, @object.BeardTags, @object.TattooTags));
			NetworkCommunicator networkPeer = missionPeer.GetNetworkPeer();
			if (this.GameMode.ShouldSpawnVisualsForServer(networkPeer))
			{
				base.AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(missionPeer, agentBuildData, -1, true, totalCount);
			}
			this.GameMode.HandleAgentVisualSpawning(networkPeer, agentBuildData, totalCount, false);
		}

		private const int EnforcedSpawnTimeInSeconds = 15;

		private float _spawningTimer;

		private bool _spawningTimerTicking;

		private bool _haveBotsBeenSpawned;

		private bool _roundInitialSpawnOver;

		private MissionMultiplayerFlagDomination _flagDominationMissionController;

		private MultiplayerRoundController _roundController;

		private List<KeyValuePair<MissionPeer, Timer>> _enforcedSpawnTimers;
	}
}
