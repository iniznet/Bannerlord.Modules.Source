using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public abstract class SpawningBehaviorBase
	{
		private protected MultiplayerMissionAgentVisualSpawnComponent AgentVisualSpawnComponent { protected get; private set; }

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
			this._agentsToBeSpawnedCache = new List<AgentBuildData>();
			this._nextTimeToCleanUpMounts = MissionTime.Now;
			this._botsCountForSides = new int[2];
		}

		public virtual void Clear()
		{
			this.MissionLobbyEquipmentNetworkComponent.OnEquipmentRefreshed -= this.OnPeerEquipmentUpdated;
			this._agentsToBeSpawnedCache = null;
		}

		public virtual void OnTick(float dt)
		{
			int count = Mission.Current.AllAgents.Count;
			int num = 0;
			this._agentsToBeSpawnedCache.Clear();
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
						int num2 = 0;
						bool flag = false;
						int intValue = MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						if (intValue > 0 && (this.GameMode.WarmupComponent == null || !this.GameMode.WarmupComponent.IsInWarmup))
						{
							num2 = MPPerkObject.GetTroopCount(mpheroClassForPeer, intValue, onSpawnPerkHandler);
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
						if (num2 > 0)
						{
							num2 = (int)((float)num2 * this.GameMode.GetTroopNumberMultiplierForMissingPlayer(component));
						}
						num2 += (flag ? 2 : 1);
						IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> enumerable = ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetAlternativeEquipments(false) : null);
						int i = 0;
						while (i < num2)
						{
							bool flag2 = i == 0;
							BasicCharacterObject basicCharacterObject = (flag2 ? mpheroClassForPeer.HeroCharacter : ((flag && i == 1) ? mpheroClassForPeer.BannerBearerCharacter : mpheroClassForPeer.TroopCharacter));
							uint num3 = ((!this.GameMode.IsGameModeUsingOpposingTeams || component.Team == this.Mission.AttackerTeam) ? component.Culture.Color : component.Culture.ClothAlternativeColor);
							uint num4 = ((!this.GameMode.IsGameModeUsingOpposingTeams || component.Team == this.Mission.AttackerTeam) ? component.Culture.Color2 : component.Culture.ClothAlternativeColor2);
							uint num5 = ((!this.GameMode.IsGameModeUsingOpposingTeams || component.Team == this.Mission.AttackerTeam) ? component.Culture.BackgroundColor1 : component.Culture.BackgroundColor2);
							uint num6 = ((!this.GameMode.IsGameModeUsingOpposingTeams || component.Team == this.Mission.AttackerTeam) ? component.Culture.ForegroundColor1 : component.Culture.ForegroundColor2);
							Banner banner = new Banner(component.Peer.BannerCode, num5, num6);
							AgentBuildData agentBuildData = new AgentBuildData(basicCharacterObject).VisualsIndex(i).Team(component.Team).TroopOrigin(new BasicBattleAgentOrigin(basicCharacterObject))
								.Formation(component.ControlledFormation)
								.IsFemale(flag2 ? component.Peer.IsFemale : basicCharacterObject.IsFemale)
								.ClothingColor1(num3)
								.ClothingColor2(num4)
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
								this.GameMode.AddCosmeticItemsToEquipment(equipment, this.GameMode.GetUsedCosmeticsFromPeer(component, basicCharacterObject));
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
								goto IL_587;
							}
							Vec2 vec;
							if (!(spawnFrame.origin != agentBuildData.AgentInitialPosition))
							{
								vec = spawnFrame.rotation.f.AsVec2.Normalized();
								Vec2? agentInitialDirection = agentBuildData.AgentInitialDirection;
								if (!(vec != agentInitialDirection))
								{
									goto IL_587;
								}
							}
							agentBuildData.InitialPosition(spawnFrame.origin);
							AgentBuildData agentBuildData2 = agentBuildData;
							vec = spawnFrame.rotation.f.AsVec2;
							vec = vec.Normalized();
							agentBuildData2.InitialDirection(vec);
							IL_5A0:
							if (component.ControlledAgent != null && !flag2)
							{
								MatrixFrame frame = component.ControlledAgent.Frame;
								frame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
								MatrixFrame matrixFrame = frame;
								matrixFrame.origin -= matrixFrame.rotation.f.NormalizedCopy() * 3.5f;
								Mat3 rotation = matrixFrame.rotation;
								rotation.MakeUnit();
								bool flag3 = !basicCharacterObject.Equipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty;
								int num7 = MathF.Min(num2, 10);
								MatrixFrame matrixFrame2 = Formation.GetFormationFramesForBeforeFormationCreation((float)num7 * Formation.GetDefaultUnitDiameter(flag3) + (float)(num7 - 1) * Formation.GetDefaultMinimumInterval(flag3), num2, flag3, new WorldPosition(Mission.Current.Scene, matrixFrame.origin), rotation)[i - 1].ToGroundMatrixFrame();
								agentBuildData.InitialPosition(matrixFrame2.origin);
								AgentBuildData agentBuildData3 = agentBuildData;
								vec = matrixFrame2.rotation.f.AsVec2;
								vec = vec.Normalized();
								agentBuildData3.InitialDirection(vec);
							}
							this._agentsToBeSpawnedCache.Add(agentBuildData);
							num++;
							if (!agentBuildData.AgentOverridenSpawnEquipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty)
							{
								num++;
							}
							i++;
							continue;
							IL_587:
							Debug.FailedAssert("Spawn frame could not be found.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\SpawnBehaviors\\SpawningBehaviors\\SpawningBehaviorBase.cs", "OnTick", 216);
							goto IL_5A0;
						}
					}
				}
			}
			int num8 = num + count;
			if (num8 > SpawningBehaviorBase.AgentCountThreshold && this._nextTimeToCleanUpMounts.IsPast)
			{
				this._nextTimeToCleanUpMounts = MissionTime.SecondsFromNow(5f);
				for (int j = Mission.Current.MountsWithoutRiders.Count - 1; j >= 0; j--)
				{
					KeyValuePair<Agent, MissionTime> keyValuePair = Mission.Current.MountsWithoutRiders[j];
					Agent key = keyValuePair.Key;
					if (keyValuePair.Value.ElapsedSeconds > 30f)
					{
						key.FadeOut(false, false);
					}
				}
			}
			int num9 = SpawningBehaviorBase.MaxAgentCount - num8;
			if (num9 >= 0)
			{
				for (int k = this._agentsToBeSpawnedCache.Count - 1; k >= 0; k--)
				{
					AgentBuildData agentBuildData4 = this._agentsToBeSpawnedCache[k];
					bool flag4 = agentBuildData4.AgentMissionPeer != null;
					MissionPeer missionPeer = (flag4 ? agentBuildData4.AgentMissionPeer : agentBuildData4.OwningAgentMissionPeer);
					MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler2 = MPPerkObject.GetOnSpawnPerkHandler(missionPeer);
					Agent agent = this.Mission.SpawnAgent(agentBuildData4, true);
					agent.AddComponent(new MPPerksAgentComponent(agent));
					Agent mountAgent = agent.MountAgent;
					if (mountAgent != null)
					{
						mountAgent.UpdateAgentProperties();
					}
					agent.HealthLimit += ((onSpawnPerkHandler2 != null) ? onSpawnPerkHandler2.GetHitpoints(flag4) : 0f);
					agent.Health = agent.HealthLimit;
					if (!flag4)
					{
						agent.SetWatchState(Agent.WatchState.Alarmed);
					}
					agent.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp, Equipment.InitialWeaponEquipPreference.Any);
					if (flag4)
					{
						MissionPeer missionPeer2 = missionPeer;
						int spawnCountThisRound = missionPeer2.SpawnCountThisRound;
						missionPeer2.SpawnCountThisRound = spawnCountThisRound + 1;
						Action<MissionPeer> onPeerSpawnedFromVisuals = this.OnPeerSpawnedFromVisuals;
						if (onPeerSpawnedFromVisuals != null)
						{
							onPeerSpawnedFromVisuals(missionPeer);
						}
						Action<MissionPeer> onAllAgentsFromPeerSpawnedFromVisuals = this.OnAllAgentsFromPeerSpawnedFromVisuals;
						if (onAllAgentsFromPeerSpawnedFromVisuals != null)
						{
							onAllAgentsFromPeerSpawnedFromVisuals(missionPeer);
						}
						this.AgentVisualSpawnComponent.RemoveAgentVisuals(missionPeer, true);
						if (GameNetwork.IsServerOrRecorder)
						{
							GameNetwork.BeginBroadcastModuleEvent();
							GameNetwork.WriteMessage(new RemoveAgentVisualsForPeer(missionPeer.GetNetworkPeer()));
							GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
						}
						missionPeer.HasSpawnedAgentVisuals = false;
						MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(missionPeer);
						if (perkHandler != null)
						{
							perkHandler.OnEvent(MPPerkCondition.PerkEventFlags.SpawnEnd);
						}
					}
				}
				int intValue2 = MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				int intValue3 = MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				if (this.GameMode.IsGameModeUsingOpposingTeams && (intValue2 > 0 || intValue3 > 0))
				{
					ValueTuple<Team, BasicCultureObject, int>[] array = new ValueTuple<Team, BasicCultureObject, int>[]
					{
						new ValueTuple<Team, BasicCultureObject, int>(this.Mission.DefenderTeam, MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)), intValue3 - this._botsCountForSides[0]),
						new ValueTuple<Team, BasicCultureObject, int>(this.Mission.AttackerTeam, MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)), intValue2 - this._botsCountForSides[1])
					};
					if (num9 >= 4)
					{
						for (int l = 0; l < Math.Min(num9 / 2, array[0].Item3 + array[1].Item3); l++)
						{
							this.SpawnBot(array[l % 2].Item1, array[l % 2].Item2);
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
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new RemoveAgentVisualsForPeer(component.GetNetworkPeer()));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
					component.HasSpawnedAgentVisuals = false;
				}
			}
			foreach (NetworkCommunicator networkCommunicator2 in GameNetwork.DisconnectedNetworkPeers)
			{
				MissionPeer missionPeer = ((networkCommunicator2 != null) ? networkCommunicator2.GetComponent<MissionPeer>() : null);
				if (missionPeer != null)
				{
					this.AgentVisualSpawnComponent.RemoveAgentVisuals(missionPeer, false);
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new RemoveAgentVisualsForPeer(missionPeer.GetNetworkPeer()));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
					missionPeer.HasSpawnedAgentVisuals = false;
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
			Debug.FailedAssert("networkCommunicator != null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\SpawnBehaviors\\SpawningBehaviors\\SpawningBehaviorBase.cs", "GetBodyProperties", 510);
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
			agentBuildData2.Equipment(Equipment.GetRandomEquipmentElements(troopCharacter, !GameNetwork.IsMultiplayer, false, agentBuildData2.AgentEquipmentSeed));
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
			agentBuildData2.Equipment(Equipment.GetRandomEquipmentElements(troopCharacter, !GameNetwork.IsMultiplayer, false, agentBuildData2.AgentEquipmentSeed));
			agentBuildData2.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData2.AgentRace, agentBuildData2.AgentIsFemale, troopCharacter.GetBodyPropertiesMin(false), troopCharacter.GetBodyPropertiesMax(), (int)agentBuildData2.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData2.AgentEquipmentSeed, troopCharacter.HairTags, troopCharacter.BeardTags, troopCharacter.TattooTags));
			Agent agent = this.Mission.SpawnAgent(agentBuildData2, false);
			agent.AIStateFlags |= Agent.AIStateFlag.Alarmed;
			this._botsCountForSides[(int)agent.Team.Side]++;
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
			for (int i = 0; i < this._botsCountForSides.Length; i++)
			{
				this._botsCountForSides[i] = 0;
			}
		}

		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent.IsHuman && affectedAgent.MissionPeer == null && affectedAgent.OwningAgentMissionPeer == null)
			{
				this._botsCountForSides[(int)affectedAgent.Team.Side]--;
			}
		}

		private static int MaxAgentCount = MBAPI.IMBAgent.GetMaximumNumberOfAgents();

		private static int AgentCountThreshold = (int)((float)SpawningBehaviorBase.MaxAgentCount * 0.9f);

		private const float SecondsToWaitForEachMountBeforeSelectingToFadeOut = 30f;

		private const float SecondsToWaitBeforeNextMountCleanup = 5f;

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

		private List<AgentBuildData> _agentsToBeSpawnedCache;

		private MissionTime _nextTimeToCleanUpMounts;

		private int[] _botsCountForSides;

		public delegate void OnSpawningEndedEventDelegate();
	}
}
