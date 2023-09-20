using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerMissionAgentVisualSpawnComponent : MissionNetwork
	{
		public event Action OnMyAgentVisualSpawned;

		public event Action OnMyAgentSpawnedFromVisual;

		public event Action OnMyAgentVisualRemoved;

		public MultiplayerMissionAgentVisualSpawnComponent()
		{
			this._mpTroops = MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>();
		}

		public Dictionary<string, string> GetUsedCosmeticsFromPeer(MissionPeer missionPeer, BasicCharacterObject selectedTroopCharacter)
		{
			if (missionPeer.Peer.UsedCosmetics != null)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				int troopIndexFromCharacter = this.GetTroopIndexFromCharacter(selectedTroopCharacter);
				List<int> list;
				missionPeer.Peer.UsedCosmetics.TryGetValue(troopIndexFromCharacter, out list);
				if (list != null)
				{
					foreach (int num in list)
					{
						CosmeticsManager.ClothingCosmeticElement clothingCosmeticElement;
						if ((clothingCosmeticElement = CosmeticsManager.GetCosmeticElementList[num] as CosmeticsManager.ClothingCosmeticElement) != null)
						{
							foreach (string text in clothingCosmeticElement.ReplaceItemsId)
							{
								dictionary.Add(text, CosmeticsManager.GetCosmeticElementList[num].Id);
							}
							foreach (Tuple<string, string> tuple in clothingCosmeticElement.ReplaceItemless)
							{
								if (tuple.Item1 == this._mpTroops[troopIndexFromCharacter].StringId)
								{
									dictionary.Add(tuple.Item2, CosmeticsManager.GetCosmeticElementList[num].Id);
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

		private int GetTroopIndexFromCharacter(BasicCharacterObject character)
		{
			for (int i = 0; i < this._mpTroops.Count; i++)
			{
				if (this._mpTroops[i].HeroCharacter == character || this._mpTroops[i].TroopCharacter == character)
				{
					return i;
				}
			}
			return -1;
		}

		public void SpawnAgentVisualsForPeer(MissionPeer missionPeer, AgentBuildData buildData, int selectedEquipmentSetIndex = -1, bool isBot = false, int totalTroopCount = 0)
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			if (myPeer != null)
			{
				myPeer.GetComponent<MissionPeer>();
			}
			if (buildData.AgentVisualsIndex == 0)
			{
				missionPeer.ClearAllVisuals(false);
			}
			missionPeer.ClearVisuals(buildData.AgentVisualsIndex);
			Equipment equipment = new Equipment(buildData.AgentOverridenSpawnEquipment);
			ItemObject item = equipment[10].Item;
			MatrixFrame spawnPointFrameForPlayer = this._spawnFrameSelectionHelper.GetSpawnPointFrameForPlayer(missionPeer.Peer, missionPeer.Team.Side, buildData.AgentVisualsIndex, totalTroopCount, item != null);
			ActionIndexCache actionIndexCache = ((item == null) ? SpawningBehaviorBase.PoseActionInfantry : SpawningBehaviorBase.PoseActionCavalry);
			MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(buildData.AgentCharacter);
			MBReadOnlyList<MPPerkObject> selectedPerks = missionPeer.SelectedPerks;
			float num = 0.1f + MBRandom.RandomFloat * 0.8f;
			IAgentVisual agentVisual = null;
			if (item != null)
			{
				Monster monster = item.HorseComponent.Monster;
				AgentVisualsData agentVisualsData = new AgentVisualsData().Equipment(equipment).Scale(item.ScaleFactor).Frame(MatrixFrame.Identity)
					.ActionSet(MBGlobals.GetActionSet(monster.ActionSetCode))
					.Scene(Mission.Current.Scene)
					.Monster(monster)
					.PrepareImmediately(false)
					.MountCreationKey(MountCreationKey.GetRandomMountKeyString(item, MBRandom.RandomInt()));
				agentVisual = Mission.Current.AgentVisualCreator.Create(agentVisualsData, "Agent " + buildData.AgentCharacter.StringId + " mount", true, false);
				MatrixFrame matrixFrame = spawnPointFrameForPlayer;
				matrixFrame.rotation.ApplyScaleLocal(agentVisualsData.ScaleData);
				ActionIndexCache actionIndexCache2 = ActionIndexCache.act_none;
				foreach (MPPerkObject mpperkObject in selectedPerks)
				{
					if (!isBot && mpperkObject.HeroMountIdleAnimOverride != null)
					{
						actionIndexCache2 = ActionIndexCache.Create(mpperkObject.HeroMountIdleAnimOverride);
						break;
					}
					if (isBot && mpperkObject.TroopMountIdleAnimOverride != null)
					{
						actionIndexCache2 = ActionIndexCache.Create(mpperkObject.TroopMountIdleAnimOverride);
						break;
					}
				}
				if (actionIndexCache2 == ActionIndexCache.act_none)
				{
					if (item.StringId == "mp_aserai_camel")
					{
						Debug.Print("Client is spawning a camel for without mountCustomAction from the perk.", 0, Debug.DebugColor.White, 17179869184UL);
						if (!isBot)
						{
							actionIndexCache2 = ActionIndexCache.Create("act_hero_mount_idle_camel");
						}
						else
						{
							actionIndexCache2 = ActionIndexCache.Create("act_camel_idle_1");
						}
					}
					else
					{
						if (!isBot && !string.IsNullOrEmpty(mpheroClassForCharacter.HeroMountIdleAnim))
						{
							actionIndexCache2 = ActionIndexCache.Create(mpheroClassForCharacter.HeroMountIdleAnim);
						}
						if (isBot && !string.IsNullOrEmpty(mpheroClassForCharacter.TroopMountIdleAnim))
						{
							actionIndexCache2 = ActionIndexCache.Create(mpheroClassForCharacter.TroopMountIdleAnim);
						}
					}
				}
				if (actionIndexCache2 != ActionIndexCache.act_none)
				{
					agentVisual.SetAction(actionIndexCache2, 0f, true);
					agentVisual.GetVisuals().GetSkeleton().SetAnimationParameterAtChannel(0, num);
					agentVisual.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.1f, matrixFrame, true);
				}
				agentVisual.GetVisuals().GetEntity().SetFrame(ref matrixFrame);
			}
			ActionIndexCache actionIndexCache3 = actionIndexCache;
			if (agentVisual != null)
			{
				actionIndexCache3 = agentVisual.GetVisuals().GetSkeleton().GetActionAtChannel(0);
			}
			else
			{
				foreach (MPPerkObject mpperkObject2 in selectedPerks)
				{
					if (!isBot && mpperkObject2.HeroIdleAnimOverride != null)
					{
						actionIndexCache3 = ActionIndexCache.Create(mpperkObject2.HeroIdleAnimOverride);
						break;
					}
					if (isBot && mpperkObject2.TroopIdleAnimOverride != null)
					{
						actionIndexCache3 = ActionIndexCache.Create(mpperkObject2.TroopIdleAnimOverride);
						break;
					}
				}
				if (actionIndexCache3 == actionIndexCache)
				{
					if (!isBot && !string.IsNullOrEmpty(mpheroClassForCharacter.HeroIdleAnim))
					{
						actionIndexCache3 = ActionIndexCache.Create(mpheroClassForCharacter.HeroIdleAnim);
					}
					if (isBot && !string.IsNullOrEmpty(mpheroClassForCharacter.TroopIdleAnim))
					{
						actionIndexCache3 = ActionIndexCache.Create(mpheroClassForCharacter.TroopIdleAnim);
					}
				}
			}
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(buildData.AgentCharacter.Race);
			IAgentVisual agentVisual2 = Mission.Current.AgentVisualCreator.Create(new AgentVisualsData().Equipment(equipment).BodyProperties(buildData.AgentBodyProperties).Frame(spawnPointFrameForPlayer)
				.ActionSet(MBActionSet.GetActionSet(baseMonsterFromRace.ActionSetCode))
				.Scene(Mission.Current.Scene)
				.Monster(baseMonsterFromRace)
				.PrepareImmediately(false)
				.UseMorphAnims(true)
				.SkeletonType(buildData.AgentIsFemale ? SkeletonType.Female : SkeletonType.Male)
				.ClothColor1(buildData.AgentClothingColor1)
				.ClothColor2(buildData.AgentClothingColor2)
				.AddColorRandomness(buildData.AgentVisualsIndex != 0)
				.ActionCode(actionIndexCache3), "Mission::SpawnAgentVisuals", true, false);
			agentVisual2.SetAction(actionIndexCache3, 0f, true);
			agentVisual2.GetVisuals().GetSkeleton().SetAnimationParameterAtChannel(0, num);
			agentVisual2.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.1f, spawnPointFrameForPlayer, true);
			agentVisual2.GetVisuals().SetFrame(ref spawnPointFrameForPlayer);
			agentVisual2.SetCharacterObjectID(buildData.AgentCharacter.StringId);
			EquipmentIndex equipmentIndex;
			EquipmentIndex equipmentIndex2;
			bool flag;
			equipment.GetInitialWeaponIndicesToEquip(out equipmentIndex, out equipmentIndex2, out flag);
			if (flag)
			{
				equipmentIndex2 = EquipmentIndex.None;
			}
			agentVisual2.GetVisuals().SetWieldedWeaponIndices((int)equipmentIndex, (int)equipmentIndex2);
			PeerVisualsHolder peerVisualsHolder = new PeerVisualsHolder(missionPeer, buildData.AgentVisualsIndex, agentVisual2, agentVisual);
			missionPeer.OnVisualsSpawned(peerVisualsHolder, peerVisualsHolder.VisualsIndex);
			if (buildData.AgentVisualsIndex == 0)
			{
				missionPeer.HasSpawnedAgentVisuals = true;
				missionPeer.EquipmentUpdatingExpired = false;
			}
			if (missionPeer.IsMine && buildData.AgentVisualsIndex == 0)
			{
				Action onMyAgentVisualSpawned = this.OnMyAgentVisualSpawned;
				if (onMyAgentVisualSpawned == null)
				{
					return;
				}
				onMyAgentVisualSpawned();
			}
		}

		public void RemoveAgentVisuals(MissionPeer missionPeer, bool sync = false)
		{
			missionPeer.ClearAllVisuals(false);
			if (!GameNetwork.IsDedicatedServer && !missionPeer.Peer.IsMine)
			{
				this._spawnFrameSelectionHelper.FreeSpawnPointFromPlayer(missionPeer.Peer);
			}
			if (sync && GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new RemoveAgentVisualsForPeer(missionPeer.GetNetworkPeer()));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
			missionPeer.HasSpawnedAgentVisuals = false;
			if (this.OnMyAgentVisualRemoved != null && missionPeer.IsMine)
			{
				this.OnMyAgentVisualRemoved();
			}
			Debug.Print("Removed visuals for " + missionPeer.Name + ".", 0, Debug.DebugColor.White, 17179869184UL);
		}

		public void RemoveAgentVisualsWithVisualIndex(MissionPeer missionPeer, int visualsIndex, bool sync = false)
		{
			missionPeer.ClearVisuals(visualsIndex);
			if (!GameNetwork.IsDedicatedServer && visualsIndex == 0 && !missionPeer.Peer.IsMine)
			{
				this._spawnFrameSelectionHelper.FreeSpawnPointFromPlayer(missionPeer.Peer);
			}
			if (sync && GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new RemoveAgentVisualsFromIndexForPeer(missionPeer.GetNetworkPeer(), visualsIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, missionPeer.GetNetworkPeer());
			}
			if (this.OnMyAgentVisualRemoved != null && missionPeer.IsMine && visualsIndex == 0)
			{
				this.OnMyAgentVisualRemoved();
			}
			Debug.Print("Removed visuals.", 0, Debug.DebugColor.BrightWhite, 64UL);
		}

		public void OnMyAgentSpawned()
		{
			Action onMyAgentSpawnedFromVisual = this.OnMyAgentSpawnedFromVisual;
			if (onMyAgentSpawnedFromVisual == null)
			{
				return;
			}
			onMyAgentSpawnedFromVisual();
		}

		public override void OnPreMissionTick(float dt)
		{
			if (!GameNetwork.IsDedicatedServer && this._spawnFrameSelectionHelper == null && Mission.Current != null && GameNetwork.MyPeer != null)
			{
				this._spawnFrameSelectionHelper = new MultiplayerMissionAgentVisualSpawnComponent.VisualSpawnFrameSelectionHelper();
			}
		}

		private MultiplayerMissionAgentVisualSpawnComponent.VisualSpawnFrameSelectionHelper _spawnFrameSelectionHelper;

		private MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> _mpTroops;

		private class VisualSpawnFrameSelectionHelper
		{
			public VisualSpawnFrameSelectionHelper()
			{
				this._visualSpawnPoints = new GameEntity[6];
				this._visualAttackerSpawnPoints = new GameEntity[6];
				this._visualDefenderSpawnPoints = new GameEntity[6];
				this._visualSpawnPointUsers = new VirtualPlayer[6];
				for (int i = 0; i < 6; i++)
				{
					List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTag("sp_visual_" + i).ToList<GameEntity>();
					if (list.Count > 0)
					{
						this._visualSpawnPoints[i] = list[0];
					}
					list = Mission.Current.Scene.FindEntitiesWithTag("sp_visual_attacker_" + i).ToList<GameEntity>();
					if (list.Count > 0)
					{
						this._visualAttackerSpawnPoints[i] = list[0];
					}
					list = Mission.Current.Scene.FindEntitiesWithTag("sp_visual_defender_" + i).ToList<GameEntity>();
					if (list.Count > 0)
					{
						this._visualDefenderSpawnPoints[i] = list[0];
					}
				}
				this._visualSpawnPointUsers[0] = GameNetwork.MyPeer.VirtualPlayer;
			}

			public MatrixFrame GetSpawnPointFrameForPlayer(VirtualPlayer player, BattleSideEnum side, int agentVisualIndex, int totalTroopCount, bool isMounted = false)
			{
				if (agentVisualIndex == 0)
				{
					int num = -1;
					int num2 = -1;
					for (int i = 0; i < this._visualSpawnPointUsers.Length; i++)
					{
						if (this._visualSpawnPointUsers[i] == player)
						{
							num = i;
							break;
						}
						if (num2 < 0 && this._visualSpawnPointUsers[i] == null)
						{
							num2 = i;
						}
					}
					int num3 = ((num >= 0) ? num : num2);
					if (num3 >= 0)
					{
						this._visualSpawnPointUsers[num3] = player;
						GameEntity gameEntity = null;
						if (side == BattleSideEnum.Attacker)
						{
							gameEntity = this._visualAttackerSpawnPoints[num3];
						}
						else if (side == BattleSideEnum.Defender)
						{
							gameEntity = this._visualDefenderSpawnPoints[num3];
						}
						MatrixFrame matrixFrame = ((gameEntity != null) ? gameEntity.GetGlobalFrame() : this._visualSpawnPoints[num3].GetGlobalFrame());
						matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
						return matrixFrame;
					}
					Debug.FailedAssert("Couldn't find a valid spawn point for player.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MultiplayerMissionAgentVisualSpawnComponent.cs", "GetSpawnPointFrameForPlayer", 137);
					return MatrixFrame.Identity;
				}
				else
				{
					Vec3 origin = this._visualSpawnPoints[3].GetGlobalFrame().origin;
					Vec3 origin2 = this._visualSpawnPoints[1].GetGlobalFrame().origin;
					Vec3 origin3 = this._visualSpawnPoints[5].GetGlobalFrame().origin;
					Mat3 rotation = this._visualSpawnPoints[0].GetGlobalFrame().rotation;
					rotation.MakeUnit();
					List<WorldFrame> formationFramesForBeforeFormationCreation = Formation.GetFormationFramesForBeforeFormationCreation(origin2.Distance(origin3), totalTroopCount, isMounted, new WorldPosition(Mission.Current.Scene, origin), rotation);
					if (formationFramesForBeforeFormationCreation.Count < agentVisualIndex)
					{
						return new MatrixFrame(rotation, origin);
					}
					return formationFramesForBeforeFormationCreation[agentVisualIndex - 1].ToGroundMatrixFrame();
				}
			}

			public void FreeSpawnPointFromPlayer(VirtualPlayer player)
			{
				for (int i = 0; i < this._visualSpawnPointUsers.Length; i++)
				{
					if (this._visualSpawnPointUsers[i] == player)
					{
						this._visualSpawnPointUsers[i] = null;
						return;
					}
				}
			}

			private const string SpawnPointTagPrefix = "sp_visual_";

			private const string AttackerSpawnPointTagPrefix = "sp_visual_attacker_";

			private const string DefenderSpawnPointTagPrefix = "sp_visual_defender_";

			private const int NumberOfSpawnPoints = 6;

			private const int PlayerSpawnPointIndex = 0;

			private GameEntity[] _visualSpawnPoints;

			private GameEntity[] _visualAttackerSpawnPoints;

			private GameEntity[] _visualDefenderSpawnPoints;

			private VirtualPlayer[] _visualSpawnPointUsers;
		}
	}
}
