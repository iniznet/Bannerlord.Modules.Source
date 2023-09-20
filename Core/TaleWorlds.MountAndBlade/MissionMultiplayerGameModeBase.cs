using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MissionMultiplayerGameModeBase : MissionNetwork
	{
		public abstract bool IsGameModeHidingAllAgentVisuals { get; }

		public abstract bool IsGameModeUsingOpposingTeams { get; }

		public SpawnComponent SpawnComponent { get; private set; }

		private protected bool CanGameModeSystemsTickThisFrame { protected get; private set; }

		public abstract MultiplayerGameType GetMissionType();

		public virtual bool CheckIfOvertime()
		{
			return false;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.MultiplayerTeamSelectComponent = base.Mission.GetMissionBehavior<MultiplayerTeamSelectComponent>();
			this.MissionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this.GameModeBaseClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this.NotificationsComponent = base.Mission.GetMissionBehavior<MultiplayerGameNotificationsComponent>();
			this.RoundController = base.Mission.GetMissionBehavior<MultiplayerRoundController>();
			this.WarmupComponent = base.Mission.GetMissionBehavior<MultiplayerWarmupComponent>();
			this.TimerComponent = base.Mission.GetMissionBehavior<MultiplayerTimerComponent>();
			this.SpawnComponent = Mission.Current.GetMissionBehavior<SpawnComponent>();
			this._agentVisualSpawnComponent = base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
			this._lastPerkTickTime = Mission.Current.CurrentTime;
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (Mission.Current.CurrentTime - this._lastPerkTickTime >= 1f)
			{
				this._lastPerkTickTime = Mission.Current.CurrentTime;
				MPPerkObject.TickAllPeerPerks((int)(this._lastPerkTickTime / 1f));
			}
		}

		public virtual bool CheckForWarmupEnd()
		{
			return false;
		}

		public virtual bool CheckForRoundEnd()
		{
			return false;
		}

		public virtual bool CheckForMatchEnd()
		{
			return false;
		}

		public virtual bool UseCultureSelection()
		{
			return false;
		}

		public virtual bool UseRoundController()
		{
			return false;
		}

		public virtual Team GetWinnerTeam()
		{
			return null;
		}

		public virtual void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
		{
		}

		public override void OnClearScene()
		{
			base.OnClearScene();
			if (this.RoundController == null)
			{
				this.ClearPeerCounts();
			}
			this._lastPerkTickTime = Mission.Current.CurrentTime;
		}

		public void ClearPeerCounts()
		{
			List<MissionPeer> list = VirtualPlayer.Peers<MissionPeer>();
			for (int i = 0; i < list.Count; i++)
			{
				MissionPeer missionPeer = list[i];
				missionPeer.AssistCount = 0;
				missionPeer.DeathCount = 0;
				missionPeer.KillCount = 0;
				missionPeer.Score = 0;
				missionPeer.ResetRequestedKickPollCount();
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new KillDeathCountChange(missionPeer.GetNetworkPeer(), null, missionPeer.KillCount, missionPeer.AssistCount, missionPeer.DeathCount, missionPeer.Score));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
		}

		public bool ShouldSpawnVisualsForServer(NetworkCommunicator spawningNetworkPeer)
		{
			if (GameNetwork.IsDedicatedServer)
			{
				return false;
			}
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			if (missionPeer != null)
			{
				MissionPeer component = spawningNetworkPeer.GetComponent<MissionPeer>();
				return (!this.IsGameModeHidingAllAgentVisuals && component.Team == missionPeer.Team) || spawningNetworkPeer.IsServerPeer;
			}
			return false;
		}

		public void HandleAgentVisualSpawning(NetworkCommunicator spawningNetworkPeer, AgentBuildData spawningAgentBuildData, int troopCountInFormation = 0, bool useCosmetics = true)
		{
			MissionPeer component = spawningNetworkPeer.GetComponent<MissionPeer>();
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new SyncPerksForCurrentlySelectedTroop(spawningNetworkPeer, component.Perks[component.SelectedTroopIndex]));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, spawningNetworkPeer);
			component.HasSpawnedAgentVisuals = true;
			component.EquipmentUpdatingExpired = false;
			if (useCosmetics)
			{
				this.AddCosmeticItemsToEquipment(spawningAgentBuildData.AgentOverridenSpawnEquipment, this.GetUsedCosmeticsFromPeer(component, spawningAgentBuildData.AgentCharacter));
			}
			if (!this.IsGameModeHidingAllAgentVisuals)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new CreateAgentVisuals(spawningNetworkPeer, spawningAgentBuildData, component.SelectedTroopIndex, troopCountInFormation));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, spawningNetworkPeer);
				return;
			}
			if (!spawningNetworkPeer.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(spawningNetworkPeer);
				GameNetwork.WriteMessage(new CreateAgentVisuals(spawningNetworkPeer, spawningAgentBuildData, component.SelectedTroopIndex, troopCountInFormation));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		public virtual bool AllowCustomPlayerBanners()
		{
			return true;
		}

		public int GetScoreForKill(Agent killedAgent)
		{
			return 20;
		}

		public virtual float GetTroopNumberMultiplierForMissingPlayer(MissionPeer spawningPeer)
		{
			return 1f;
		}

		public int GetCurrentGoldForPeer(MissionPeer peer)
		{
			return peer.Representative.Gold;
		}

		public void ChangeCurrentGoldForPeer(MissionPeer peer, int newAmount)
		{
			if (newAmount >= 0)
			{
				newAmount = MBMath.ClampInt(newAmount, 0, 2000);
			}
			if (peer.Peer.Communicator.IsConnectionActive)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SyncGoldsForSkirmish(peer.Peer, newAmount));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
			if (this.GameModeBaseClient != null)
			{
				this.GameModeBaseClient.OnGoldAmountChangedForRepresentative(peer.Representative, newAmount);
			}
		}

		protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			if (this.GameModeBaseClient.IsGameModeUsingGold)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					if (networkCommunicator != networkPeer)
					{
						MissionRepresentativeBase component = networkCommunicator.GetComponent<MissionRepresentativeBase>();
						if (component != null)
						{
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new SyncGoldsForSkirmish(component.Peer, component.Gold));
							GameNetwork.EndModuleEventAsServer();
						}
					}
				}
			}
		}

		public virtual bool CheckIfPlayerCanDespawn(MissionPeer missionPeer)
		{
			return false;
		}

		public override void OnPreMissionTick(float dt)
		{
			this.CanGameModeSystemsTickThisFrame = false;
			this._gameModeSystemTickTimer += dt;
			if (this._gameModeSystemTickTimer >= 0.25f)
			{
				this._gameModeSystemTickTimer -= 0.25f;
				this.CanGameModeSystemsTickThisFrame = true;
			}
		}

		public Dictionary<string, string> GetUsedCosmeticsFromPeer(MissionPeer missionPeer, BasicCharacterObject selectedTroopCharacter)
		{
			if (missionPeer.Peer.UsedCosmetics != null)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>();
				int num = -1;
				for (int i = 0; i < objectTypeList.Count; i++)
				{
					if (objectTypeList[i].HeroCharacter == selectedTroopCharacter || objectTypeList[i].TroopCharacter == selectedTroopCharacter)
					{
						num = i;
						break;
					}
				}
				List<int> list;
				missionPeer.Peer.UsedCosmetics.TryGetValue(num, out list);
				if (list != null)
				{
					foreach (int num2 in list)
					{
						ClothingCosmeticElement clothingCosmeticElement;
						if ((clothingCosmeticElement = CosmeticsManager.CosmeticElementsList[num2] as ClothingCosmeticElement) != null)
						{
							foreach (string text in clothingCosmeticElement.ReplaceItemsId)
							{
								dictionary.Add(text, CosmeticsManager.CosmeticElementsList[num2].Id);
							}
							foreach (Tuple<string, string> tuple in clothingCosmeticElement.ReplaceItemless)
							{
								if (tuple.Item1 == objectTypeList[num].StringId)
								{
									dictionary.Add(tuple.Item2, CosmeticsManager.CosmeticElementsList[num2].Id);
									break;
								}
							}
						}
					}
				}
				return dictionary;
			}
			return null;
		}

		public void AddCosmeticItemsToEquipment(Equipment equipment, Dictionary<string, string> choosenCosmetics)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ArmorItemEndSlot; equipmentIndex++)
			{
				if (equipment[equipmentIndex].Item == null)
				{
					string text = equipmentIndex.ToString();
					switch (equipmentIndex)
					{
					case EquipmentIndex.NumAllWeaponSlots:
						text = "Head";
						break;
					case EquipmentIndex.Body:
						text = "Body";
						break;
					case EquipmentIndex.Leg:
						text = "Leg";
						break;
					case EquipmentIndex.Gloves:
						text = "Gloves";
						break;
					case EquipmentIndex.Cape:
						text = "Cape";
						break;
					}
					string text2 = null;
					if (choosenCosmetics != null)
					{
						choosenCosmetics.TryGetValue(text, out text2);
					}
					if (text2 != null)
					{
						ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(text2);
						EquipmentElement equipmentElement = equipment[equipmentIndex];
						equipmentElement.CosmeticItem = @object;
						equipment[equipmentIndex] = equipmentElement;
					}
				}
				else
				{
					string stringId = equipment[equipmentIndex].Item.StringId;
					string text3 = null;
					if (choosenCosmetics != null)
					{
						choosenCosmetics.TryGetValue(stringId, out text3);
					}
					if (text3 != null)
					{
						ItemObject object2 = MBObjectManager.Instance.GetObject<ItemObject>(text3);
						EquipmentElement equipmentElement2 = equipment[equipmentIndex];
						equipmentElement2.CosmeticItem = object2;
						equipment[equipmentIndex] = equipmentElement2;
					}
				}
			}
		}

		public const int GoldCap = 2000;

		public const float PerkTickPeriod = 1f;

		public const float GameModeSystemTickPeriod = 0.25f;

		private float _lastPerkTickTime;

		private MultiplayerMissionAgentVisualSpawnComponent _agentVisualSpawnComponent;

		public MultiplayerTeamSelectComponent MultiplayerTeamSelectComponent;

		protected MissionLobbyComponent MissionLobbyComponent;

		protected MultiplayerGameNotificationsComponent NotificationsComponent;

		public MultiplayerRoundController RoundController;

		public MultiplayerWarmupComponent WarmupComponent;

		public MultiplayerTimerComponent TimerComponent;

		protected MissionMultiplayerGameModeBaseClient GameModeBaseClient;

		private float _gameModeSystemTickTimer;
	}
}
