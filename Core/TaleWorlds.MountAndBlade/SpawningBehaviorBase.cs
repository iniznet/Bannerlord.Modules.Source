using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	public abstract class SpawningBehaviorBase
	{
		public MultiplayerMissionAgentVisualSpawnComponent AgentVisualSpawnComponent { get; private set; }

		protected Mission Mission
		{
			get
			{
				return this.SpawnComponent.Mission;
			}
		}

		protected event Action<MissionPeer> OnAllAgentsFromPeerSpawnedFromVisuals;

		protected event Action<MissionPeer> OnPeerSpawnedFromVisuals;

		public event SpawningBehaviorBase.OnSpawningEndedEventDelegate OnSpawningEnded;

		public virtual void Initialize(SpawnComponent spawnComponent)
		{
			this.SpawnComponent = spawnComponent;
			this.AgentVisualSpawnComponent = this.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
			this.GameMode = this.Mission.GetMissionBehavior<MissionMultiplayerGameModeBase>();
			this.MissionLobbyComponent = this.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this.MissionLobbyEquipmentNetworkComponent = this.Mission.GetMissionBehavior<MissionLobbyEquipmentNetworkComponent>();
			this.MissionLobbyEquipmentNetworkComponent.OnEquipmentRefreshed += this.OnPeerEquipmentUpdated;
			this._spawnCheckTimer = new Timer(Mission.Current.CurrentTime, 0.2f, true);
		}

		public virtual void Clear()
		{
			this.MissionLobbyEquipmentNetworkComponent.OnEquipmentRefreshed -= this.OnPeerEquipmentUpdated;
		}

		public virtual void OnTick(float dt)
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (networkCommunicator.IsSynchronized)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && component.ControlledAgent == null && component.HasSpawnedAgentVisuals && !this.CanUpdateSpawnEquipment(component))
					{
						MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(component, false);
						MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(component);
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SyncPerksForCurrentlySelectedTroop(networkCommunicator, component.Perks[component.SelectedTroopIndex]));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, networkCommunicator);
						int num = 0;
						bool flag = false;
						if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0 && (this.GameMode.WarmupComponent == null || !this.GameMode.WarmupComponent.IsInWarmup))
						{
							num = MPPerkObject.GetTroopCount(mpheroClassForPeer, onSpawnPerkHandler);
							using (List<MPPerkObject>.Enumerator enumerator2 = component.SelectedPerks.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									if (enumerator2.Current.HasBannerBearer)
									{
										flag = true;
										break;
									}
								}
							}
						}
						if (num > 0)
						{
							num = (int)((float)num * this.GameMode.GetTroopNumberMultiplierForMissingPlayer(component));
						}
						num += (flag ? 2 : 1);
						IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> enumerable = ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetAlternativeEquipments(false) : null);
						int i = 0;
						while (i < num)
						{
							bool flag2 = i == 0;
							BasicCharacterObject basicCharacterObject = (flag2 ? mpheroClassForPeer.HeroCharacter : ((flag && i == 1) ? mpheroClassForPeer.BannerBearerCharacter : mpheroClassForPeer.TroopCharacter));
							uint num2 = ((!this.GameMode.IsGameModeUsingOpposingTeams || component.Team == this.Mission.AttackerTeam) ? component.Culture.Color : component.Culture.ClothAlternativeColor);
							uint num3 = ((!this.GameMode.IsGameModeUsingOpposingTeams || component.Team == this.Mission.AttackerTeam) ? component.Culture.Color2 : component.Culture.ClothAlternativeColor2);
							uint num4 = ((!this.GameMode.IsGameModeUsingOpposingTeams || component.Team == this.Mission.AttackerTeam) ? component.Culture.BackgroundColor1 : component.Culture.BackgroundColor2);
							uint num5 = ((!this.GameMode.IsGameModeUsingOpposingTeams || component.Team == this.Mission.AttackerTeam) ? component.Culture.ForegroundColor1 : component.Culture.ForegroundColor2);
							Banner banner = new Banner(component.Peer.BannerCode, num4, num5);
							AgentBuildData agentBuildData = new AgentBuildData(basicCharacterObject).VisualsIndex(i).Team(component.Team).TroopOrigin(new BasicBattleAgentOrigin(basicCharacterObject))
								.Formation(component.ControlledFormation)
								.IsFemale(flag2 ? component.Peer.IsFemale : basicCharacterObject.IsFemale)
								.ClothingColor1(num2)
								.ClothingColor2(num3)
								.Banner(banner);
							if (flag2)
							{
								agentBuildData.MissionPeer(component);
							}
							else
							{
								agentBuildData.OwningMissionPeer(component);
							}
							Equipment equipment = (flag2 ? basicCharacterObject.Equipment.Clone(false) : Equipment.GetRandomEquipmentElements(basicCharacterObject, false, false, MBRandom.RandomInt()));
							IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> enumerable2 = (flag2 ? ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetAlternativeEquipments(true) : null) : enumerable);
							if (enumerable2 != null)
							{
								foreach (ValueTuple<EquipmentIndex, EquipmentElement> valueTuple in enumerable2)
								{
									equipment[valueTuple.Item1] = valueTuple.Item2;
								}
							}
							agentBuildData.Equipment(equipment);
							if (flag2)
							{
								this.AgentVisualSpawnComponent.AddCosmeticItemsToEquipment(equipment, this.AgentVisualSpawnComponent.GetUsedCosmeticsFromPeer(component, basicCharacterObject));
							}
							if (flag2)
							{
								agentBuildData.BodyProperties(this.GetBodyProperties(component, component.Culture));
								agentBuildData.Age((int)agentBuildData.AgentBodyProperties.Age);
							}
							else
							{
								agentBuildData.EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(basicCharacterObject, agentBuildData.AgentVisualsIndex));
								agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentRace, agentBuildData.AgentIsFemale, basicCharacterObject.GetBodyPropertiesMin(false), basicCharacterObject.GetBodyPropertiesMax(), (int)agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, basicCharacterObject.HairTags, basicCharacterObject.BeardTags, basicCharacterObject.TattooTags));
							}
							if (component.ControlledFormation != null && component.ControlledFormation.Banner == null)
							{
								component.ControlledFormation.Banner = banner;
							}
							MatrixFrame spawnFrame = this.SpawnComponent.GetSpawnFrame(component.Team, equipment[EquipmentIndex.ArmorItemEndSlot].Item != null, component.SpawnCountThisRound == 0);
							if (spawnFrame.IsIdentity)
							{
								goto IL_533;
							}
							Vec2 vec;
							if (!(spawnFrame.origin != agentBuildData.AgentInitialPosition))
							{
								vec = spawnFrame.rotation.f.AsVec2.Normalized();
								Vec2? agentInitialDirection = agentBuildData.AgentInitialDirection;
								if (!(vec != agentInitialDirection))
								{
									goto IL_533;
								}
							}
							agentBuildData.InitialPosition(spawnFrame.origin);
							AgentBuildData agentBuildData2 = agentBuildData;
							vec = spawnFrame.rotation.f.AsVec2;
							vec = vec.Normalized();
							agentBuildData2.InitialDirection(vec);
							IL_54C:
							if (component.ControlledAgent != null && !flag2)
							{
								MatrixFrame frame = component.ControlledAgent.Frame;
								frame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
								MatrixFrame matrixFrame = frame;
								matrixFrame.origin -= matrixFrame.rotation.f.NormalizedCopy() * 3.5f;
								Mat3 rotation = matrixFrame.rotation;
								rotation.MakeUnit();
								bool flag3 = !basicCharacterObject.Equipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty;
								int num6 = MathF.Min(num, 10);
								MatrixFrame matrixFrame2 = Formation.GetFormationFramesForBeforeFormationCreation((float)num6 * Formation.GetDefaultUnitDiameter(flag3) + (float)(num6 - 1) * Formation.GetDefaultMinimumInterval(flag3), num, flag3, new WorldPosition(Mission.Current.Scene, matrixFrame.origin), rotation)[i - 1].ToGroundMatrixFrame();
								agentBuildData.InitialPosition(matrixFrame2.origin);
								AgentBuildData agentBuildData3 = agentBuildData;
								vec = matrixFrame2.rotation.f.AsVec2;
								vec = vec.Normalized();
								agentBuildData3.InitialDirection(vec);
							}
							Agent agent = this.Mission.SpawnAgent(agentBuildData, true);
							agent.AddComponent(new MPPerksAgentComponent(agent));
							Agent mountAgent = agent.MountAgent;
							if (mountAgent != null)
							{
								mountAgent.UpdateAgentProperties();
							}
							float num7 = ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetHitpoints(flag2) : 0f);
							agent.HealthLimit += num7;
							agent.Health = agent.HealthLimit;
							if (!flag2)
							{
								agent.SetWatchState(Agent.WatchState.Alarmed);
							}
							agent.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp);
							if (flag2)
							{
								Action<MissionPeer> onPeerSpawnedFromVisuals = this.OnPeerSpawnedFromVisuals;
								if (onPeerSpawnedFromVisuals != null)
								{
									onPeerSpawnedFromVisuals(component);
								}
							}
							i++;
							continue;
							IL_533:
							Debug.FailedAssert("Spawn frame could not be found.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\SpawnBehaviors\\SpawningBehaviors\\SpawningBehaviorBase.cs", "OnTick", 194);
							goto IL_54C;
						}
						MissionPeer missionPeer = component;
						int spawnCountThisRound = missionPeer.SpawnCountThisRound;
						missionPeer.SpawnCountThisRound = spawnCountThisRound + 1;
						Action<MissionPeer> onAllAgentsFromPeerSpawnedFromVisuals = this.OnAllAgentsFromPeerSpawnedFromVisuals;
						if (onAllAgentsFromPeerSpawnedFromVisuals != null)
						{
							onAllAgentsFromPeerSpawnedFromVisuals(component);
						}
						this.AgentVisualSpawnComponent.RemoveAgentVisuals(component, true);
						MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(component);
						if (perkHandler != null)
						{
							perkHandler.OnEvent(MPPerkCondition.PerkEventFlags.SpawnEnd);
						}
					}
				}
			}
			if (!this.IsSpawningEnabled && this.IsRoundInProgress())
			{
				if (this.SpawningDelayTimer >= this.SpawningEndDelay && !this._hasCalledSpawningEnded)
				{
					Mission.Current.AllowAiTicking = true;
					if (this.OnSpawningEnded != null)
					{
						this.OnSpawningEnded();
					}
					this._hasCalledSpawningEnded = true;
				}
				this.SpawningDelayTimer += dt;
			}
		}

		public bool AreAgentsSpawning()
		{
			return this.IsSpawningEnabled;
		}

		protected void ResetSpawnCounts()
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (component != null)
				{
					component.SpawnCountThisRound = 0;
				}
			}
		}

		protected void ResetSpawnTimers()
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (component != null)
				{
					component.SpawnTimer.Reset(Mission.Current.CurrentTime, 0f);
				}
			}
		}

		public virtual void RequestStartSpawnSession()
		{
			this.IsSpawningEnabled = true;
			this.SpawningDelayTimer = 0f;
			this._hasCalledSpawningEnded = false;
			this.ResetSpawnCounts();
		}

		public void RequestStopSpawnSession()
		{
			this.IsSpawningEnabled = false;
			this.SpawningDelayTimer = 0f;
			this._hasCalledSpawningEnded = false;
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (component != null)
				{
					this.AgentVisualSpawnComponent.RemoveAgentVisuals(component, true);
				}
			}
			foreach (ICommunicator communicator in MBNetwork.DisconnectedNetworkPeers)
			{
				NetworkCommunicator networkCommunicator2 = communicator as NetworkCommunicator;
				MissionPeer missionPeer = ((networkCommunicator2 != null) ? networkCommunicator2.GetComponent<MissionPeer>() : null);
				if (missionPeer != null)
				{
					this.AgentVisualSpawnComponent.RemoveAgentVisuals(missionPeer, false);
				}
			}
		}

		public void SetRemainingAgentsInvulnerable()
		{
			foreach (Agent agent in this.Mission.Agents)
			{
				agent.SetMortalityState(Agent.MortalityState.Invulnerable);
			}
		}

		protected abstract void SpawnAgents();

		protected BodyProperties GetBodyProperties(MissionPeer missionPeer, BasicCultureObject cultureLimit)
		{
			NetworkCommunicator networkPeer = missionPeer.GetNetworkPeer();
			if (networkPeer != null)
			{
				return networkPeer.PlayerConnectionInfo.GetParameter<PlayerData>("PlayerData").BodyProperties;
			}
			Debug.FailedAssert("networkCommunicator != null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\SpawnBehaviors\\SpawningBehaviors\\SpawningBehaviorBase.cs", "GetBodyProperties", 366);
			Team team = missionPeer.Team;
			BasicCharacterObject troopCharacter = MultiplayerClassDivisions.GetMPHeroClasses(cultureLimit).ToMBList<MultiplayerClassDivisions.MPHeroClass>().GetRandomElement<MultiplayerClassDivisions.MPHeroClass>()
				.TroopCharacter;
			MatrixFrame spawnFrame = this.SpawnComponent.GetSpawnFrame(team, troopCharacter.HasMount(), true);
			AgentBuildData agentBuildData = new AgentBuildData(troopCharacter).Team(team).InitialPosition(spawnFrame.origin);
			Vec2 vec = spawnFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(vec).TroopOrigin(new BasicBattleAgentOrigin(troopCharacter)).EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(troopCharacter, 0))
				.ClothingColor1((team.Side == BattleSideEnum.Attacker) ? cultureLimit.Color : cultureLimit.ClothAlternativeColor)
				.ClothingColor2((team.Side == BattleSideEnum.Attacker) ? cultureLimit.Color2 : cultureLimit.ClothAlternativeColor2)
				.IsFemale(troopCharacter.IsFemale);
			agentBuildData2.Equipment(Equipment.GetRandomEquipmentElements(troopCharacter, !(Game.Current.GameType is MultiplayerGame), false, agentBuildData2.AgentEquipmentSeed));
			agentBuildData2.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData2.AgentRace, agentBuildData2.AgentIsFemale, troopCharacter.GetBodyPropertiesMin(false), troopCharacter.GetBodyPropertiesMax(), (int)agentBuildData2.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData2.AgentEquipmentSeed, troopCharacter.HairTags, troopCharacter.BeardTags, troopCharacter.TattooTags));
			return agentBuildData2.AgentBodyProperties;
		}

		protected void SpawnBot(Team agentTeam, BasicCultureObject cultureLimit)
		{
			BasicCharacterObject troopCharacter = MultiplayerClassDivisions.GetMPHeroClasses(cultureLimit).ToMBList<MultiplayerClassDivisions.MPHeroClass>().GetRandomElement<MultiplayerClassDivisions.MPHeroClass>()
				.TroopCharacter;
			MatrixFrame spawnFrame = this.SpawnComponent.GetSpawnFrame(agentTeam, troopCharacter.HasMount(), true);
			AgentBuildData agentBuildData = new AgentBuildData(troopCharacter).Team(agentTeam).InitialPosition(spawnFrame.origin);
			Vec2 vec = spawnFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(vec).TroopOrigin(new BasicBattleAgentOrigin(troopCharacter)).EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(troopCharacter, 0))
				.ClothingColor1((agentTeam.Side == BattleSideEnum.Attacker) ? cultureLimit.Color : cultureLimit.ClothAlternativeColor)
				.ClothingColor2((agentTeam.Side == BattleSideEnum.Attacker) ? cultureLimit.Color2 : cultureLimit.ClothAlternativeColor2)
				.IsFemale(troopCharacter.IsFemale);
			agentBuildData2.Equipment(Equipment.GetRandomEquipmentElements(troopCharacter, !(Game.Current.GameType is MultiplayerGame), false, agentBuildData2.AgentEquipmentSeed));
			agentBuildData2.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData2.AgentRace, agentBuildData2.AgentIsFemale, troopCharacter.GetBodyPropertiesMin(false), troopCharacter.GetBodyPropertiesMax(), (int)agentBuildData2.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData2.AgentEquipmentSeed, troopCharacter.HairTags, troopCharacter.BeardTags, troopCharacter.TattooTags));
			this.Mission.SpawnAgent(agentBuildData2, false).AIStateFlags |= Agent.AIStateFlag.Alarmed;
		}

		private void OnPeerEquipmentUpdated(MissionPeer peer)
		{
			if (this.IsSpawningEnabled && this.CanUpdateSpawnEquipment(peer))
			{
				peer.HasSpawnedAgentVisuals = false;
				Debug.Print("HasSpawnedAgentVisuals = false for peer: " + peer.Name + " because he just updated his equipment", 0, Debug.DebugColor.White, 17592186044416UL);
				if (peer.ControlledFormation != null)
				{
					peer.ControlledFormation.HasBeenPositioned = false;
					peer.ControlledFormation.SetSpawnIndex(0);
				}
			}
		}

		public virtual bool CanUpdateSpawnEquipment(MissionPeer missionPeer)
		{
			return !missionPeer.EquipmentUpdatingExpired && !this._equipmentUpdatingExpired;
		}

		public void ToggleUpdatingSpawnEquipment(bool canUpdate)
		{
			this._equipmentUpdatingExpired = !canUpdate;
		}

		public abstract bool AllowEarlyAgentVisualsDespawning(MissionPeer missionPeer);

		public virtual int GetMaximumReSpawnPeriodForPeer(MissionPeer peer)
		{
			return 3;
		}

		protected abstract bool IsRoundInProgress();

		public virtual void OnClearScene()
		{
		}

		protected MissionMultiplayerGameModeBase GameMode;

		protected SpawnComponent SpawnComponent;

		private bool _equipmentUpdatingExpired;

		protected bool IsSpawningEnabled;

		protected Timer _spawnCheckTimer;

		protected float SpawningEndDelay = 1f;

		protected float SpawningDelayTimer;

		private bool _hasCalledSpawningEnded;

		protected MissionLobbyComponent MissionLobbyComponent;

		protected MissionLobbyEquipmentNetworkComponent MissionLobbyEquipmentNetworkComponent;

		public static readonly ActionIndexCache PoseActionInfantry = ActionIndexCache.Create("act_walk_idle_unarmed");

		public static readonly ActionIndexCache PoseActionCavalry = ActionIndexCache.Create("act_horse_stand_1");

		public delegate void OnSpawningEndedEventDelegate();
	}
}
