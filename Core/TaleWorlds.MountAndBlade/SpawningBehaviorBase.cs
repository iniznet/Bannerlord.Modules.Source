using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002CC RID: 716
	public abstract class SpawningBehaviorBase
	{
		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x0600272C RID: 10028 RVA: 0x0009597A File Offset: 0x00093B7A
		// (set) Token: 0x0600272D RID: 10029 RVA: 0x00095982 File Offset: 0x00093B82
		public MultiplayerMissionAgentVisualSpawnComponent AgentVisualSpawnComponent { get; private set; }

		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x0600272E RID: 10030 RVA: 0x0009598B File Offset: 0x00093B8B
		protected Mission Mission
		{
			get
			{
				return this.SpawnComponent.Mission;
			}
		}

		// Token: 0x14000070 RID: 112
		// (add) Token: 0x0600272F RID: 10031 RVA: 0x00095998 File Offset: 0x00093B98
		// (remove) Token: 0x06002730 RID: 10032 RVA: 0x000959D0 File Offset: 0x00093BD0
		protected event Action<MissionPeer> OnAllAgentsFromPeerSpawnedFromVisuals;

		// Token: 0x14000071 RID: 113
		// (add) Token: 0x06002731 RID: 10033 RVA: 0x00095A08 File Offset: 0x00093C08
		// (remove) Token: 0x06002732 RID: 10034 RVA: 0x00095A40 File Offset: 0x00093C40
		protected event Action<MissionPeer> OnPeerSpawnedFromVisuals;

		// Token: 0x14000072 RID: 114
		// (add) Token: 0x06002733 RID: 10035 RVA: 0x00095A78 File Offset: 0x00093C78
		// (remove) Token: 0x06002734 RID: 10036 RVA: 0x00095AB0 File Offset: 0x00093CB0
		public event SpawningBehaviorBase.OnSpawningEndedEventDelegate OnSpawningEnded;

		// Token: 0x06002735 RID: 10037 RVA: 0x00095AE8 File Offset: 0x00093CE8
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

		// Token: 0x06002736 RID: 10038 RVA: 0x00095B72 File Offset: 0x00093D72
		public virtual void Clear()
		{
			this.MissionLobbyEquipmentNetworkComponent.OnEquipmentRefreshed -= this.OnPeerEquipmentUpdated;
		}

		// Token: 0x06002737 RID: 10039 RVA: 0x00095B8C File Offset: 0x00093D8C
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

		// Token: 0x06002738 RID: 10040 RVA: 0x000963A4 File Offset: 0x000945A4
		public bool AreAgentsSpawning()
		{
			return this.IsSpawningEnabled;
		}

		// Token: 0x06002739 RID: 10041 RVA: 0x000963AC File Offset: 0x000945AC
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

		// Token: 0x0600273A RID: 10042 RVA: 0x00096400 File Offset: 0x00094600
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

		// Token: 0x0600273B RID: 10043 RVA: 0x00096468 File Offset: 0x00094668
		public virtual void RequestStartSpawnSession()
		{
			this.IsSpawningEnabled = true;
			this.SpawningDelayTimer = 0f;
			this._hasCalledSpawningEnded = false;
			this.ResetSpawnCounts();
		}

		// Token: 0x0600273C RID: 10044 RVA: 0x0009648C File Offset: 0x0009468C
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

		// Token: 0x0600273D RID: 10045 RVA: 0x0009655C File Offset: 0x0009475C
		public void SetRemainingAgentsInvulnerable()
		{
			foreach (Agent agent in this.Mission.Agents)
			{
				agent.SetMortalityState(Agent.MortalityState.Invulnerable);
			}
		}

		// Token: 0x0600273E RID: 10046
		protected abstract void SpawnAgents();

		// Token: 0x0600273F RID: 10047 RVA: 0x000965B4 File Offset: 0x000947B4
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

		// Token: 0x06002740 RID: 10048 RVA: 0x00096748 File Offset: 0x00094948
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

		// Token: 0x06002741 RID: 10049 RVA: 0x000968A4 File Offset: 0x00094AA4
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

		// Token: 0x06002742 RID: 10050 RVA: 0x0009690F File Offset: 0x00094B0F
		public virtual bool CanUpdateSpawnEquipment(MissionPeer missionPeer)
		{
			return !missionPeer.EquipmentUpdatingExpired && !this._equipmentUpdatingExpired;
		}

		// Token: 0x06002743 RID: 10051 RVA: 0x00096924 File Offset: 0x00094B24
		public void ToggleUpdatingSpawnEquipment(bool canUpdate)
		{
			this._equipmentUpdatingExpired = !canUpdate;
		}

		// Token: 0x06002744 RID: 10052
		public abstract bool AllowEarlyAgentVisualsDespawning(MissionPeer missionPeer);

		// Token: 0x06002745 RID: 10053 RVA: 0x00096930 File Offset: 0x00094B30
		public virtual int GetMaximumReSpawnPeriodForPeer(MissionPeer peer)
		{
			return 3;
		}

		// Token: 0x06002746 RID: 10054
		protected abstract bool IsRoundInProgress();

		// Token: 0x06002747 RID: 10055 RVA: 0x00096933 File Offset: 0x00094B33
		public virtual void OnClearScene()
		{
		}

		// Token: 0x04000E88 RID: 3720
		protected MissionMultiplayerGameModeBase GameMode;

		// Token: 0x04000E89 RID: 3721
		protected SpawnComponent SpawnComponent;

		// Token: 0x04000E8A RID: 3722
		private bool _equipmentUpdatingExpired;

		// Token: 0x04000E8B RID: 3723
		protected bool IsSpawningEnabled;

		// Token: 0x04000E8C RID: 3724
		protected Timer _spawnCheckTimer;

		// Token: 0x04000E8D RID: 3725
		protected float SpawningEndDelay = 1f;

		// Token: 0x04000E8E RID: 3726
		protected float SpawningDelayTimer;

		// Token: 0x04000E8F RID: 3727
		private bool _hasCalledSpawningEnded;

		// Token: 0x04000E90 RID: 3728
		protected MissionLobbyComponent MissionLobbyComponent;

		// Token: 0x04000E91 RID: 3729
		protected MissionLobbyEquipmentNetworkComponent MissionLobbyEquipmentNetworkComponent;

		// Token: 0x04000E92 RID: 3730
		public static readonly ActionIndexCache PoseActionInfantry = ActionIndexCache.Create("act_walk_idle_unarmed");

		// Token: 0x04000E93 RID: 3731
		public static readonly ActionIndexCache PoseActionCavalry = ActionIndexCache.Create("act_horse_stand_1");

		// Token: 0x020005E9 RID: 1513
		// (Invoke) Token: 0x06003CCB RID: 15563
		public delegate void OnSpawningEndedEventDelegate();
	}
}
