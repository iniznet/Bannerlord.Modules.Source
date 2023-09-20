using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using SandBox.Missions.AgentBehaviors;
using SandBox.Objects.AnimationPoints;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.MountAndBlade.Source.Objects;
using TaleWorlds.ObjectSystem;

namespace SandBox.Missions.MissionLogics
{
	public class MissionAgentHandler : MissionLogic
	{
		public bool HasPassages()
		{
			List<UsableMachine> list;
			return this._usablePoints.TryGetValue("npc_passage", out list) && list.Count > 0;
		}

		public List<UsableMachine> TownPassageProps
		{
			get
			{
				List<UsableMachine> list;
				this._usablePoints.TryGetValue("npc_passage", out list);
				return list;
			}
		}

		public MissionAgentHandler(Location location, string playerSpecialSpawnTag = null)
		{
			this._currentLocation = location;
			this._previousLocation = ((Campaign.Current.GameMode == 1) ? Campaign.Current.GameMenuManager.PreviousLocation : null);
			if (this._previousLocation != null && !this._currentLocation.LocationsOfPassages.Contains(this._previousLocation))
			{
				Debug.FailedAssert(string.Concat(new object[]
				{
					"No passage from ",
					this._previousLocation.DoorName,
					" to ",
					this._currentLocation.DoorName
				}), "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\MissionAgentHandler.cs", ".ctor", 75);
				this._previousLocation = null;
			}
			this._usablePoints = new Dictionary<string, List<UsableMachine>>();
			this._pairedUsablePoints = new Dictionary<string, List<UsableMachine>>();
			this._disabledPassages = new List<UsableMachine>();
			this._checkPossibleQuestTimer = new BasicMissionTimer();
			this._playerSpecialSpawnTag = playerSpecialSpawnTag;
		}

		public override void OnCreated()
		{
			if (this._currentLocation != null)
			{
				CampaignMission.Current.Location = this._currentLocation;
			}
		}

		public override void EarlyStart()
		{
			this._passageUsageTime = base.Mission.CurrentTime + 30f;
			this.GetAllProps();
			this.InitializePairedUsableObjects();
			base.Mission.SetReportStuckAgentsMode(true);
		}

		public override void OnRenderingStarted()
		{
		}

		public override void OnMissionTick(float dt)
		{
			float currentTime = base.Mission.CurrentTime;
			if (currentTime > this._passageUsageTime)
			{
				this._passageUsageTime = currentTime + 30f;
				if (PlayerEncounter.LocationEncounter != null && LocationComplex.Current != null)
				{
					LocationComplex.Current.AgentPassageUsageTick();
				}
			}
		}

		public override void OnRemoveBehavior()
		{
			foreach (Location location in LocationComplex.Current.GetListOfLocations())
			{
				if (location.StringId == "center" || location.StringId == "village_center" || location.StringId == "lordshall" || location.StringId == "prison" || location.StringId == "tavern" || location.StringId == "alley")
				{
					location.RemoveAllCharacters((LocationCharacter x) => !x.Character.IsHero);
				}
			}
			base.OnRemoveBehavior();
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectedAgent.IsHuman && (agentState == 4 || agentState == 3))
			{
				LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(affectedAgent.Origin);
				if (locationCharacter != null)
				{
					CampaignMission.Current.Location.RemoveLocationCharacter(locationCharacter);
					if (PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter) != null && affectedAgent.State == 4)
					{
						PlayerEncounter.LocationEncounter.RemoveAccompanyingCharacter(locationCharacter);
					}
				}
			}
			foreach (Agent agent in base.Mission.Agents)
			{
				CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
				if (component != null)
				{
					component.OnAgentRemoved(affectedAgent);
				}
			}
		}

		private void InitializePairedUsableObjects()
		{
			Dictionary<string, List<UsableMachine>> dictionary = new Dictionary<string, List<UsableMachine>>();
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair in this._usablePoints)
			{
				foreach (UsableMachine usableMachine in keyValuePair.Value)
				{
					using (List<StandingPoint>.Enumerator enumerator3 = usableMachine.StandingPoints.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							AnimationPoint animationPoint;
							if ((animationPoint = enumerator3.Current as AnimationPoint) != null && animationPoint.PairEntity != null)
							{
								if (this._pairedUsablePoints.ContainsKey(keyValuePair.Key))
								{
									if (!this._pairedUsablePoints[keyValuePair.Key].Contains(usableMachine))
									{
										this._pairedUsablePoints[keyValuePair.Key].Add(usableMachine);
									}
								}
								else
								{
									this._pairedUsablePoints.Add(keyValuePair.Key, new List<UsableMachine> { usableMachine });
								}
								if (dictionary.ContainsKey(keyValuePair.Key))
								{
									dictionary[keyValuePair.Key].Add(usableMachine);
								}
								else
								{
									dictionary.Add(keyValuePair.Key, new List<UsableMachine> { usableMachine });
								}
							}
						}
					}
				}
			}
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair2 in dictionary)
			{
				foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair3 in this._usablePoints)
				{
					foreach (UsableMachine usableMachine2 in dictionary[keyValuePair2.Key])
					{
						keyValuePair3.Value.Remove(usableMachine2);
					}
				}
			}
		}

		private void GetAllProps()
		{
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("navigation_mesh_deactivator");
			if (gameEntity != null)
			{
				NavigationMeshDeactivator firstScriptOfType = gameEntity.GetFirstScriptOfType<NavigationMeshDeactivator>();
				MissionAgentHandler._disabledFaceId = firstScriptOfType.DisableFaceWithId;
				MissionAgentHandler._disabledFaceIdForAnimals = firstScriptOfType.DisableFaceWithIdForAnimals;
			}
			this._usablePoints.Clear();
			foreach (UsableMachine usableMachine in MBExtensions.FindAllWithType<UsableMachine>(base.Mission.MissionObjects))
			{
				foreach (string text in usableMachine.GameEntity.Tags)
				{
					if (!this._usablePoints.ContainsKey(text))
					{
						this._usablePoints.Add(text, new List<UsableMachine>());
					}
					this._usablePoints[text].Add(usableMachine);
				}
			}
			if (Settlement.CurrentSettlement.IsTown || Settlement.CurrentSettlement.IsVillage)
			{
				foreach (AreaMarker areaMarker in MBExtensions.FindAllWithType<AreaMarker>(base.Mission.ActiveMissionObjects).ToList<AreaMarker>())
				{
					string tag = areaMarker.Tag;
					List<UsableMachine> usableMachinesInRange = areaMarker.GetUsableMachinesInRange(areaMarker.Tag.Contains("workshop") ? "unaffected_by_area" : null);
					if (!this._usablePoints.ContainsKey(tag))
					{
						this._usablePoints.Add(tag, new List<UsableMachine>());
					}
					foreach (UsableMachine usableMachine2 in usableMachinesInRange)
					{
						foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair in this._usablePoints)
						{
							if (keyValuePair.Value.Contains(usableMachine2))
							{
								keyValuePair.Value.Remove(usableMachine2);
							}
						}
						if (usableMachine2.GameEntity.HasTag("hold_tag_always"))
						{
							string text2 = usableMachine2.GameEntity.Tags[0] + "_" + areaMarker.Tag;
							usableMachine2.GameEntity.AddTag(text2);
							if (!this._usablePoints.ContainsKey(text2))
							{
								this._usablePoints.Add(text2, new List<UsableMachine>());
								this._usablePoints[text2].Add(usableMachine2);
							}
							else
							{
								this._usablePoints[text2].Add(usableMachine2);
							}
						}
						else
						{
							foreach (UsableMachine usableMachine3 in usableMachinesInRange)
							{
								if (!usableMachine3.GameEntity.HasTag(tag))
								{
									usableMachine3.GameEntity.AddTag(tag);
								}
							}
						}
					}
					if (this._usablePoints.ContainsKey(tag))
					{
						usableMachinesInRange.RemoveAll((UsableMachine x) => this._usablePoints[tag].Contains(x));
						if (usableMachinesInRange.Count > 0)
						{
							this._usablePoints[tag].AddRange(usableMachinesInRange);
						}
					}
					foreach (UsableMachine usableMachine4 in areaMarker.GetUsableMachinesWithTagInRange("unaffected_by_area"))
					{
						string text3 = usableMachine4.GameEntity.Tags[0];
						foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair2 in this._usablePoints)
						{
							if (keyValuePair2.Value.Contains(usableMachine4))
							{
								keyValuePair2.Value.Remove(usableMachine4);
							}
						}
						if (this._usablePoints.ContainsKey(text3))
						{
							this._usablePoints[text3].Add(usableMachine4);
						}
						else
						{
							this._usablePoints.Add(text3, new List<UsableMachine>());
							this._usablePoints[text3].Add(usableMachine4);
						}
					}
				}
			}
			this.DisableUnavailableWaypoints();
			this.RemoveDeactivatedUsablePlacesFromList();
		}

		[Conditional("DEBUG")]
		public void DetectMissingEntities()
		{
			if (CampaignMission.Current.Location != null && !Utilities.CommandLineArgumentExists("CampaignGameplayTest"))
			{
				IEnumerable<LocationCharacter> characterList = CampaignMission.Current.Location.GetCharacterList();
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (LocationCharacter locationCharacter in characterList)
				{
					if (locationCharacter.SpecialTargetTag != null && !locationCharacter.IsHidden)
					{
						if (dictionary.ContainsKey(locationCharacter.SpecialTargetTag))
						{
							Dictionary<string, int> dictionary2 = dictionary;
							string specialTargetTag = locationCharacter.SpecialTargetTag;
							int num = dictionary2[specialTargetTag];
							dictionary2[specialTargetTag] = num + 1;
						}
						else
						{
							dictionary.Add(locationCharacter.SpecialTargetTag, 1);
						}
					}
				}
				foreach (KeyValuePair<string, int> keyValuePair in dictionary)
				{
					string key = keyValuePair.Key;
					int value = keyValuePair.Value;
					int num2 = 0;
					if (this._usablePoints.ContainsKey(key))
					{
						num2 += this._usablePoints[key].Count;
						foreach (UsableMachine usableMachine in this._usablePoints[key])
						{
							num2 += MissionAgentHandler.GetPointCountOfUsableMachine(usableMachine, false);
						}
					}
					if (this._pairedUsablePoints.ContainsKey(key))
					{
						num2 += this._pairedUsablePoints[key].Count;
						foreach (UsableMachine usableMachine2 in this._pairedUsablePoints[key])
						{
							num2 += MissionAgentHandler.GetPointCountOfUsableMachine(usableMachine2, false);
						}
					}
					if (num2 < value)
					{
						string.Concat(new object[]
						{
							"Trying to spawn ",
							value,
							" npc with \"",
							key,
							"\" but there are ",
							num2,
							" suitable spawn points in scene ",
							base.Mission.SceneName
						});
						if (TestCommonBase.BaseInstance != null)
						{
							bool isTestEnabled = TestCommonBase.BaseInstance.IsTestEnabled;
						}
					}
				}
			}
		}

		public void RemoveDeactivatedUsablePlacesFromList()
		{
			Dictionary<string, List<UsableMachine>> dictionary = new Dictionary<string, List<UsableMachine>>();
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair in this._usablePoints)
			{
				foreach (UsableMachine usableMachine in keyValuePair.Value)
				{
					if (usableMachine.IsDeactivated)
					{
						if (dictionary.ContainsKey(keyValuePair.Key))
						{
							dictionary[keyValuePair.Key].Add(usableMachine);
						}
						else
						{
							dictionary.Add(keyValuePair.Key, new List<UsableMachine>());
							dictionary[keyValuePair.Key].Add(usableMachine);
						}
					}
				}
			}
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair2 in dictionary)
			{
				foreach (UsableMachine usableMachine2 in keyValuePair2.Value)
				{
					this._usablePoints[keyValuePair2.Key].Remove(usableMachine2);
				}
			}
		}

		private Dictionary<string, int> FindUnusedUsablePointCount()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair in this._usablePoints)
			{
				int num = 0;
				foreach (UsableMachine usableMachine in keyValuePair.Value)
				{
					num += MissionAgentHandler.GetPointCountOfUsableMachine(usableMachine, true);
				}
				if (num > 0)
				{
					dictionary.Add(keyValuePair.Key, num);
				}
			}
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair2 in this._pairedUsablePoints)
			{
				int num2 = 0;
				foreach (UsableMachine usableMachine2 in keyValuePair2.Value)
				{
					num2 += MissionAgentHandler.GetPointCountOfUsableMachine(usableMachine2, true);
				}
				if (num2 > 0)
				{
					if (!dictionary.ContainsKey(keyValuePair2.Key))
					{
						dictionary.Add(keyValuePair2.Key, num2);
					}
					else
					{
						Dictionary<string, int> dictionary2 = dictionary;
						string key = keyValuePair2.Key;
						dictionary2[key] += num2;
					}
				}
			}
			return dictionary;
		}

		private CharacterObject GetPlayerCharacter()
		{
			CharacterObject characterObject = CharacterObject.PlayerCharacter;
			if (characterObject == null)
			{
				characterObject = Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero_for_perf");
			}
			return characterObject;
		}

		public void SpawnPlayer(bool civilianEquipment = false, bool noHorses = false, bool noWeapon = false, bool wieldInitialWeapons = false, bool isStealth = false, string spawnTag = "")
		{
			if (Campaign.Current.GameMode != 1)
			{
				civilianEquipment = false;
			}
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("spawnpoint_player");
			if (gameEntity != null)
			{
				matrixFrame = gameEntity.GetGlobalFrame();
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			}
			bool flag = Campaign.Current.GameMode == 1 && PlayerEncounter.IsActive && (Settlement.CurrentSettlement.IsTown || Settlement.CurrentSettlement.IsCastle) && !Campaign.Current.IsNight && CampaignMission.Current.Location.StringId == "center" && !PlayerEncounter.LocationEncounter.IsInsideOfASettlement;
			bool flag2 = false;
			if (this._playerSpecialSpawnTag != null)
			{
				GameEntity gameEntity2 = null;
				UsableMachine usableMachine = this.GetAllUsablePointsWithTag(this._playerSpecialSpawnTag).FirstOrDefault<UsableMachine>();
				if (usableMachine != null)
				{
					StandingPoint standingPoint = usableMachine.StandingPoints.FirstOrDefault<StandingPoint>();
					gameEntity2 = ((standingPoint != null) ? standingPoint.GameEntity : null);
				}
				if (gameEntity2 == null)
				{
					gameEntity2 = base.Mission.Scene.FindEntityWithTag(this._playerSpecialSpawnTag);
				}
				if (gameEntity2 != null)
				{
					matrixFrame = gameEntity2.GetGlobalFrame();
					matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				}
			}
			else if (CampaignMission.Current.Location.StringId == "arena")
			{
				GameEntity gameEntity3 = base.Mission.Scene.FindEntityWithTag("sp_player_near_arena_master");
				if (gameEntity3 != null)
				{
					matrixFrame = gameEntity3.GetGlobalFrame();
					matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				}
			}
			else if (this._previousLocation != null)
			{
				matrixFrame = this.GetSpawnFrameOfPassage(this._previousLocation);
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				noHorses = true;
				flag2 = true;
			}
			else if (flag)
			{
				GameEntity gameEntity4 = base.Mission.Scene.FindEntityWithTag(isStealth ? "sp_player_stealth" : "spawnpoint_player_outside");
				if (gameEntity4 != null)
				{
					matrixFrame = gameEntity4.GetGlobalFrame();
					matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				}
			}
			else
			{
				GameEntity gameEntity5 = base.Mission.Scene.FindEntityWithTag("spawnpoint_player");
				if (gameEntity5 != null)
				{
					matrixFrame = gameEntity5.GetGlobalFrame();
					matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				}
			}
			if (PlayerEncounter.LocationEncounter is TownEncounter)
			{
				PlayerEncounter.LocationEncounter.IsInsideOfASettlement = true;
			}
			CharacterObject playerCharacter = this.GetPlayerCharacter();
			AgentBuildData agentBuildData = new AgentBuildData(playerCharacter).Team(base.Mission.PlayerTeam).InitialPosition(ref matrixFrame.origin);
			Vec2 vec = matrixFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).CivilianEquipment(civilianEquipment).NoHorses(noHorses)
				.NoWeapons(noWeapon)
				.ClothingColor1(base.Mission.PlayerTeam.Color)
				.ClothingColor2(base.Mission.PlayerTeam.Color2)
				.TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, this.GetPlayerCharacter(), -1, default(UniqueTroopDescriptor), false))
				.MountKey(MountCreationKey.GetRandomMountKeyString(playerCharacter.Equipment[10].Item, playerCharacter.GetMountKeySeed()))
				.Controller(2);
			Hero heroObject = playerCharacter.HeroObject;
			if (((heroObject != null) ? heroObject.ClanBanner : null) != null)
			{
				agentBuildData2.Banner(playerCharacter.HeroObject.ClanBanner);
			}
			if (Campaign.Current.GameMode != 1)
			{
				agentBuildData2.TroopOrigin(new SimpleAgentOrigin(CharacterObject.PlayerCharacter, -1, null, default(UniqueTroopDescriptor)));
			}
			if (isStealth)
			{
				agentBuildData2.Equipment(this.GetStealthEquipmentForPlayer());
			}
			else if (Campaign.Current.IsMainHeroDisguised)
			{
				Equipment defaultEquipment = MBObjectManager.Instance.GetObject<MBEquipmentRoster>("npc_disguised_hero_equipment_template").DefaultEquipment;
				Equipment firstCivilianEquipment = CharacterObject.PlayerCharacter.FirstCivilianEquipment;
				for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
				{
					ItemObject item = firstCivilianEquipment[equipmentIndex].Item;
					defaultEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, new EquipmentElement(item, null, null, false));
				}
				agentBuildData2.Equipment(defaultEquipment);
			}
			Agent agent = base.Mission.SpawnAgent(agentBuildData2, false);
			if (wieldInitialWeapons)
			{
				agent.WieldInitialWeapons(2);
			}
			if (flag2)
			{
				base.Mission.MakeSound(MiscSoundContainer.SoundCodeMovementFoleyDoorClose, matrixFrame.origin, true, false, -1, -1);
			}
			this.SpawnCharactersAccompanyingPlayer(noHorses);
			for (int i = 0; i < 3; i++)
			{
				Agent.Main.AgentVisuals.GetSkeleton().TickAnimations(0.1f, Agent.Main.AgentVisuals.GetGlobalFrame(), true);
			}
		}

		private Equipment GetStealthEquipmentForPlayer()
		{
			Equipment equipment = new Equipment();
			equipment.AddEquipmentToSlotWithoutAgent(6, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("ragged_robes"), null, null, false));
			equipment.AddEquipmentToSlotWithoutAgent(5, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("pilgrim_hood"), null, null, false));
			equipment.AddEquipmentToSlotWithoutAgent(7, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("ragged_boots"), null, null, false));
			equipment.AddEquipmentToSlotWithoutAgent(8, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("ragged_armwraps"), null, null, false));
			for (int i = 0; i < 5; i++)
			{
				EquipmentElement equipmentFromSlot = CharacterObject.PlayerCharacter.Equipment.GetEquipmentFromSlot(i);
				if (equipmentFromSlot.Item != null)
				{
					equipment.AddEquipmentToSlotWithoutAgent(i, new EquipmentElement(equipmentFromSlot.Item, null, null, false));
				}
				else if (i >= 0 && i <= 3)
				{
					equipment.AddEquipmentToSlotWithoutAgent(i, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("throwing_stone"), null, null, false));
				}
			}
			return equipment;
		}

		private MatrixFrame GetSpawnFrameOfPassage(Location location)
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			UsableMachine usableMachine = this.TownPassageProps.FirstOrDefault((UsableMachine x) => ((Passage)x).ToLocation == location) ?? this._disabledPassages.FirstOrDefault((UsableMachine x) => ((Passage)x).ToLocation == location);
			if (usableMachine != null)
			{
				MatrixFrame globalFrame = usableMachine.PilotStandingPoint.GameEntity.GetGlobalFrame();
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				globalFrame.origin.z = base.Mission.Scene.GetGroundHeightAtPosition(globalFrame.origin, 6402441);
				globalFrame.rotation.RotateAboutUp(3.1415927f);
				matrixFrame = globalFrame;
			}
			return matrixFrame;
		}

		public void DisableUnavailableWaypoints()
		{
			bool isNight = Campaign.Current.IsNight;
			string text = "";
			int num = 0;
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair in this._usablePoints)
			{
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int i = 0;
				while (i < keyValuePair.Value.Count)
				{
					UsableMachine usableMachine = keyValuePair.Value[i];
					if (!Mission.Current.IsPositionInsideBoundaries(usableMachine.GameEntity.GlobalPosition.AsVec2))
					{
						foreach (StandingPoint standingPoint in usableMachine.StandingPoints)
						{
							standingPoint.IsDeactivated = true;
							num++;
						}
					}
					if (usableMachine is Chair)
					{
						using (List<StandingPoint>.Enumerator enumerator2 = usableMachine.StandingPoints.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								StandingPoint standingPoint2 = enumerator2.Current;
								Vec3 origin = standingPoint2.GameEntity.GetGlobalFrame().origin;
								PathFaceRecord nullFaceRecord = PathFaceRecord.NullFaceRecord;
								base.Mission.Scene.GetNavMeshFaceIndex(ref nullFaceRecord, origin, true);
								if (!nullFaceRecord.IsValid() || (MissionAgentHandler._disabledFaceId != -1 && nullFaceRecord.FaceGroupIndex == MissionAgentHandler._disabledFaceId))
								{
									standingPoint2.IsDeactivated = true;
									num2++;
								}
							}
							goto IL_2B2;
						}
						goto IL_146;
					}
					goto IL_146;
					IL_2B2:
					i++;
					continue;
					IL_146:
					if (usableMachine is Passage)
					{
						Passage passage = usableMachine as Passage;
						if (passage.ToLocation == null || !passage.ToLocation.CanPlayerSee())
						{
							foreach (StandingPoint standingPoint3 in passage.StandingPoints)
							{
								standingPoint3.IsDeactivated = true;
							}
							passage.Disable();
							this._disabledPassages.Add(usableMachine);
							Location toLocation = passage.ToLocation;
							keyValuePair.Value.RemoveAt(i);
							i--;
							num3++;
							goto IL_2B2;
						}
						goto IL_2B2;
					}
					else
					{
						if (usableMachine is UsablePlace)
						{
							foreach (StandingPoint standingPoint4 in usableMachine.StandingPoints)
							{
								Vec3 origin2 = standingPoint4.GameEntity.GetGlobalFrame().origin;
								PathFaceRecord nullFaceRecord2 = PathFaceRecord.NullFaceRecord;
								base.Mission.Scene.GetNavMeshFaceIndex(ref nullFaceRecord2, origin2, true);
								if (!nullFaceRecord2.IsValid() || (MissionAgentHandler._disabledFaceId != -1 && nullFaceRecord2.FaceGroupIndex == MissionAgentHandler._disabledFaceId) || (isNight && usableMachine.GameEntity.HasTag("disable_at_night")) || (!isNight && usableMachine.GameEntity.HasTag("enable_at_night")))
								{
									standingPoint4.IsDeactivated = true;
									num4++;
								}
							}
							goto IL_2B2;
						}
						goto IL_2B2;
					}
				}
				if (num4 + num2 + num3 > 0)
				{
					text = text + "_____________________________________________\n\"" + keyValuePair.Key + "\" :\n";
					if (num4 > 0)
					{
						text = string.Concat(new object[] { text, "Disabled standing point : ", num4, "\n" });
					}
					if (num2 > 0)
					{
						text = string.Concat(new object[] { text, "Disabled chair use point : ", num2, "\n" });
					}
					if (num3 > 0)
					{
						text = string.Concat(new object[] { text, "Disabled passage info : ", num3, "\n" });
					}
				}
			}
		}

		public void SpawnLocationCharacters(string overridenTagValue = null)
		{
			Dictionary<string, int> dictionary = this.FindUnusedUsablePointCount();
			IEnumerable<LocationCharacter> characterList = CampaignMission.Current.Location.GetCharacterList();
			if (PlayerEncounter.LocationEncounter.Settlement.IsTown && CampaignMission.Current.Location == LocationComplex.Current.GetLocationWithId("center"))
			{
				foreach (LocationCharacter locationCharacter in LocationComplex.Current.GetLocationWithId("alley").GetCharacterList())
				{
					characterList.Append(locationCharacter);
				}
			}
			CampaignEventDispatcher.Instance.LocationCharactersAreReadyToSpawn(dictionary);
			foreach (LocationCharacter locationCharacter2 in characterList)
			{
				if (!this.IsAlreadySpawned(locationCharacter2.AgentOrigin) && !locationCharacter2.IsHidden)
				{
					if (!string.IsNullOrEmpty(overridenTagValue))
					{
						locationCharacter2.SpecialTargetTag = overridenTagValue;
					}
					MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(this.SpawnLocationCharacter(locationCharacter2, false), MissionAgentHandler._disabledFaceId);
				}
			}
			foreach (Agent agent in base.Mission.Agents)
			{
				this.SimulateAgent(agent);
			}
			CampaignEventDispatcher.Instance.LocationCharactersSimulated();
		}

		private bool IsAlreadySpawned(IAgentOriginBase agentOrigin)
		{
			return Mission.Current != null && Mission.Current.Agents.Any((Agent x) => x.Origin == agentOrigin);
		}

		public Agent SpawnLocationCharacter(LocationCharacter locationCharacter, bool simulateAgentAfterSpawn = false)
		{
			Agent agent = this.SpawnWanderingAgent(locationCharacter);
			if (simulateAgentAfterSpawn)
			{
				this.SimulateAgent(agent);
			}
			if (locationCharacter.IsVisualTracked)
			{
				VisualTrackerMissionBehavior missionBehavior = Mission.Current.GetMissionBehavior<VisualTrackerMissionBehavior>();
				if (missionBehavior != null)
				{
					missionBehavior.RegisterLocalOnlyObject(agent);
				}
			}
			return agent;
		}

		public void SimulateAgent(Agent agent)
		{
			if (agent.IsHuman)
			{
				AgentNavigator agentNavigator = agent.GetComponent<CampaignAgentComponent>().AgentNavigator;
				int num = MBRandom.RandomInt(35, 50);
				agent.PreloadForRendering();
				for (int i = 0; i < num; i++)
				{
					if (agentNavigator != null)
					{
						agentNavigator.Tick(0.1f, true);
					}
					if (agent.IsUsingGameObject)
					{
						agent.CurrentlyUsedGameObject.SimulateTick(0.1f);
					}
				}
			}
		}

		public void SpawnThugs()
		{
			IEnumerable<LocationCharacter> characterList = CampaignMission.Current.Location.GetCharacterList();
			List<MatrixFrame> list = (from x in base.Mission.Scene.FindEntitiesWithTag("spawnpoint_thug")
				select x.GetGlobalFrame()).ToList<MatrixFrame>();
			int num = 0;
			foreach (LocationCharacter locationCharacter in characterList)
			{
				if (locationCharacter.CharacterRelation == 2)
				{
					MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(this.SpawnWanderingAgent(locationCharacter, list[num % list.Count], true), MissionAgentHandler._disabledFaceId);
					num++;
				}
			}
		}

		private void GetFrameForFollowingAgent(Agent followedAgent, out MatrixFrame frame)
		{
			frame = followedAgent.Frame;
			frame.origin += -(frame.rotation.f * 1.5f);
		}

		public void SpawnCharactersAccompanyingPlayer(bool noHorse)
		{
			int num = 0;
			bool flag = PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer.Any((AccompanyingCharacter c) => c.IsFollowingPlayerAtMissionStart);
			foreach (AccompanyingCharacter accompanyingCharacter in PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer)
			{
				bool flag2 = accompanyingCharacter.LocationCharacter.Character.IsHero && accompanyingCharacter.LocationCharacter.Character.HeroObject.IsWounded;
				if ((this._currentLocation.GetCharacterList().Contains(accompanyingCharacter.LocationCharacter) || !flag2) && accompanyingCharacter.CanEnterLocation(this._currentLocation))
				{
					this._currentLocation.AddCharacter(accompanyingCharacter.LocationCharacter);
					if (accompanyingCharacter.IsFollowingPlayerAtMissionStart || (!flag && num == 0))
					{
						WorldFrame worldFrame = base.Mission.MainAgent.GetWorldFrame();
						worldFrame.Origin.SetVec2(base.Mission.GetRandomPositionAroundPoint(worldFrame.Origin.GetNavMeshVec3(), 0.5f, 2f, false).AsVec2);
						Agent agent = this.SpawnWanderingAgent(accompanyingCharacter.LocationCharacter, worldFrame.ToGroundMatrixFrame(), noHorse);
						int num2 = 0;
						for (;;)
						{
							Agent agent2 = agent;
							WorldPosition worldPosition = base.Mission.MainAgent.GetWorldPosition();
							if (agent2.CanMoveDirectlyToPosition(ref worldPosition) || num2 >= 50)
							{
								break;
							}
							worldFrame.Origin.SetVec2(base.Mission.GetRandomPositionAroundPoint(worldFrame.Origin.GetNavMeshVec3(), 0.5f, 4f, false).AsVec2);
							agent.TeleportToPosition(worldFrame.ToGroundMatrixFrame().origin);
							num2++;
						}
						agent.SetTeam(base.Mission.PlayerTeam, true);
						num++;
					}
					else
					{
						this.SpawnWanderingAgent(accompanyingCharacter.LocationCharacter).SetTeam(base.Mission.PlayerTeam, true);
					}
					foreach (Agent agent3 in base.Mission.Agents)
					{
						LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(agent3.Origin);
						AccompanyingCharacter accompanyingCharacter2 = PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter);
						if (agent3.GetComponent<CampaignAgentComponent>().AgentNavigator != null && accompanyingCharacter2 != null)
						{
							DailyBehaviorGroup behaviorGroup = agent3.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
							if (accompanyingCharacter.IsFollowingPlayerAtMissionStart)
							{
								(behaviorGroup.GetBehavior<FollowAgentBehavior>() ?? behaviorGroup.AddBehavior<FollowAgentBehavior>()).SetTargetAgent(Agent.Main);
								behaviorGroup.SetScriptedBehavior<FollowAgentBehavior>();
							}
							else
							{
								behaviorGroup.Behaviors.Clear();
							}
						}
					}
				}
			}
		}

		public void FadeoutExitingLocationCharacter(LocationCharacter locationCharacter)
		{
			foreach (Agent agent in Mission.Current.Agents)
			{
				if ((CharacterObject)agent.Character == locationCharacter.Character)
				{
					agent.FadeOut(false, true);
					break;
				}
			}
		}

		public void SpawnEnteringLocationCharacter(LocationCharacter locationCharacter, Location fromLocation)
		{
			if (fromLocation != null)
			{
				foreach (UsableMachine usableMachine in this.TownPassageProps)
				{
					Passage passage = usableMachine as Passage;
					if (passage.ToLocation == fromLocation)
					{
						MatrixFrame globalFrame = passage.PilotStandingPoint.GameEntity.GetGlobalFrame();
						globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
						globalFrame.origin.z = base.Mission.Scene.GetGroundHeightAtPosition(globalFrame.origin, 6402441);
						Vec3 f = globalFrame.rotation.f;
						f.Normalize();
						globalFrame.origin -= 0.3f * f;
						globalFrame.rotation.RotateAboutUp(3.1415927f);
						Agent agent = this.SpawnWanderingAgent(locationCharacter, globalFrame, true);
						MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
						base.Mission.MakeSound(MiscSoundContainer.SoundCodeMovementFoleyDoorClose, globalFrame.origin, true, false, -1, -1);
						agent.FadeIn();
						break;
					}
				}
				return;
			}
			this.SpawnLocationCharacter(locationCharacter, true);
		}

		public Agent SpawnHiddenLocationCharacter(LocationCharacter locationCharacter, MatrixFrame spawnFrame)
		{
			Agent agent = this.SpawnWanderingAgent(locationCharacter, spawnFrame, true);
			agent.FadeIn();
			return agent;
		}

		public void SpawnGuardsForSneakCaught()
		{
			List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("spawnpoint_npc_sneak").ToList<GameEntity>();
			IEnumerable<LocationCharacter> characterList = CampaignMission.Current.Location.GetCharacterList();
			int num = 0;
			foreach (LocationCharacter locationCharacter in characterList)
			{
				if (locationCharacter.Character.Occupation == 2)
				{
					MatrixFrame frame = list[num].GetFrame();
					Mission mission = base.Mission;
					AgentBuildData agentBuildData = new AgentBuildData(locationCharacter.Character).TroopOrigin(new SimpleAgentOrigin(locationCharacter.Character, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerEnemyTeam).InitialPosition(ref frame.origin);
					Vec2 vec = frame.rotation.f.AsVec2;
					vec = vec.Normalized();
					mission.SpawnAgent(agentBuildData.InitialDirection(ref vec), false);
					num++;
					if (num >= list.Count)
					{
						break;
					}
				}
			}
		}

		public void SpawnGuards()
		{
			List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("sp_guard").ToList<GameEntity>();
			int num = 0;
			foreach (LocationCharacter locationCharacter in CampaignMission.Current.Location.GetCharacterList())
			{
				if (locationCharacter.Character.Occupation == 2 || locationCharacter.Character.Occupation == 24)
				{
					if (num >= list.Count)
					{
						break;
					}
					MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(this.SpawnWanderingAgent(locationCharacter, list[num].GetGlobalFrame(), true), MissionAgentHandler._disabledFaceId);
					num++;
				}
			}
		}

		private static void SimulateAnimalAnimations(Agent agent)
		{
			int num = 10 + MBRandom.RandomInt(90);
			for (int i = 0; i < num; i++)
			{
				agent.TickActionChannels(0.1f);
				Vec3 vec = agent.ComputeAnimationDisplacement(0.1f);
				if (vec.LengthSquared > 0f)
				{
					agent.TeleportToPosition(agent.Position + vec);
				}
				agent.AgentVisuals.GetSkeleton().TickAnimations(0.1f, agent.AgentVisuals.GetGlobalFrame(), true);
			}
		}

		public static void SpawnSheeps()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("sp_sheep"))
			{
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("sheep"), 0, null);
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Mission mission = Mission.Current;
				ItemRosterElement itemRosterElement2 = itemRosterElement;
				ItemRosterElement itemRosterElement3 = default(ItemRosterElement);
				Vec2 asVec = globalFrame.rotation.f.AsVec2;
				Agent agent = mission.SpawnMonster(itemRosterElement2, itemRosterElement3, ref globalFrame.origin, ref asVec, -1);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceIdForAnimals);
				AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(gameEntity, agent);
				MissionAgentHandler.SimulateAnimalAnimations(agent);
			}
		}

		public static void SpawnCows()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("sp_cow"))
			{
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("cow"), 0, null);
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Mission mission = Mission.Current;
				ItemRosterElement itemRosterElement2 = itemRosterElement;
				ItemRosterElement itemRosterElement3 = default(ItemRosterElement);
				Vec2 asVec = globalFrame.rotation.f.AsVec2;
				Agent agent = mission.SpawnMonster(itemRosterElement2, itemRosterElement3, ref globalFrame.origin, ref asVec, -1);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceIdForAnimals);
				AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(gameEntity, agent);
				MissionAgentHandler.SimulateAnimalAnimations(agent);
			}
		}

		public static void SpawnGeese()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("sp_goose"))
			{
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("goose"), 0, null);
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Mission mission = Mission.Current;
				ItemRosterElement itemRosterElement2 = itemRosterElement;
				ItemRosterElement itemRosterElement3 = default(ItemRosterElement);
				Vec2 asVec = globalFrame.rotation.f.AsVec2;
				Agent agent = mission.SpawnMonster(itemRosterElement2, itemRosterElement3, ref globalFrame.origin, ref asVec, -1);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceIdForAnimals);
				AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(gameEntity, agent);
				MissionAgentHandler.SimulateAnimalAnimations(agent);
			}
		}

		public static void SpawnChicken()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("sp_chicken"))
			{
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("chicken"), 0, null);
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Mission mission = Mission.Current;
				ItemRosterElement itemRosterElement2 = itemRosterElement;
				ItemRosterElement itemRosterElement3 = default(ItemRosterElement);
				Vec2 asVec = globalFrame.rotation.f.AsVec2;
				Agent agent = mission.SpawnMonster(itemRosterElement2, itemRosterElement3, ref globalFrame.origin, ref asVec, -1);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceIdForAnimals);
				AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(gameEntity, agent);
				MissionAgentHandler.SimulateAnimalAnimations(agent);
			}
		}

		public static void SpawnHogs()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("sp_hog"))
			{
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("hog"), 0, null);
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Mission mission = Mission.Current;
				ItemRosterElement itemRosterElement2 = itemRosterElement;
				ItemRosterElement itemRosterElement3 = default(ItemRosterElement);
				Vec2 asVec = globalFrame.rotation.f.AsVec2;
				Agent agent = mission.SpawnMonster(itemRosterElement2, itemRosterElement3, ref globalFrame.origin, ref asVec, -1);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceIdForAnimals);
				AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(gameEntity, agent);
				MissionAgentHandler.SimulateAnimalAnimations(agent);
			}
		}

		public static void SpawnCats()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("sp_cat"))
			{
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("cat"), 0, null);
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Mission mission = Mission.Current;
				ItemRosterElement itemRosterElement2 = itemRosterElement;
				ItemRosterElement itemRosterElement3 = default(ItemRosterElement);
				Vec2 asVec = globalFrame.rotation.f.AsVec2;
				Agent agent = mission.SpawnMonster(itemRosterElement2, itemRosterElement3, ref globalFrame.origin, ref asVec, -1);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceIdForAnimals);
				AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(gameEntity, agent);
				MissionAgentHandler.SimulateAnimalAnimations(agent);
			}
		}

		public static void SpawnDogs()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("sp_dog").ToList<GameEntity>())
			{
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("dog"), 0, null);
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Mission mission = Mission.Current;
				ItemRosterElement itemRosterElement2 = itemRosterElement;
				ItemRosterElement itemRosterElement3 = default(ItemRosterElement);
				Vec2 asVec = globalFrame.rotation.f.AsVec2;
				Agent agent = mission.SpawnMonster(itemRosterElement2, itemRosterElement3, ref globalFrame.origin, ref asVec, -1);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
				MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceIdForAnimals);
				AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(gameEntity, agent);
				MissionAgentHandler.SimulateAnimalAnimations(agent);
			}
		}

		public static List<Agent> SpawnHorses()
		{
			List<Agent> list = new List<Agent>();
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("sp_horse"))
			{
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				string text = gameEntity.Tags[1];
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(text);
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(@object, 1, null);
				ItemRosterElement itemRosterElement2 = default(ItemRosterElement);
				if (@object.HasHorseComponent)
				{
					globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
					Mission mission = Mission.Current;
					ItemRosterElement itemRosterElement3 = itemRosterElement;
					ItemRosterElement itemRosterElement4 = itemRosterElement2;
					Vec2 asVec = globalFrame.rotation.f.AsVec2;
					Agent agent = mission.SpawnMonster(itemRosterElement3, itemRosterElement4, ref globalFrame.origin, ref asVec, -1);
					AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(gameEntity, agent);
					MissionAgentHandler.SimulateAnimalAnimations(agent);
					list.Add(agent);
				}
			}
			return list;
		}

		public IEnumerable<string> GetAllSpawnTags()
		{
			return this._usablePoints.Keys.ToList<string>().Concat(this._pairedUsablePoints.Keys.ToList<string>());
		}

		public List<UsableMachine> GetAllUsablePointsWithTag(string tag)
		{
			List<UsableMachine> list = new List<UsableMachine>();
			List<UsableMachine> list2 = new List<UsableMachine>();
			if (this._usablePoints.TryGetValue(tag, out list2))
			{
				list.AddRange(list2);
			}
			List<UsableMachine> list3 = new List<UsableMachine>();
			if (this._pairedUsablePoints.TryGetValue(tag, out list3))
			{
				list.AddRange(list3);
			}
			return list;
		}

		public Agent SpawnWanderingAgent(LocationCharacter locationCharacter)
		{
			bool flag = false;
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			if (locationCharacter.SpecialTargetTag != null)
			{
				List<UsableMachine> allUsablePointsWithTag = this.GetAllUsablePointsWithTag(locationCharacter.SpecialTargetTag);
				if (allUsablePointsWithTag.Count > 0)
				{
					foreach (UsableMachine usableMachine in allUsablePointsWithTag)
					{
						MatrixFrame matrixFrame2;
						if (this.GetSpawnFrameFromUsableMachine(usableMachine, out matrixFrame2))
						{
							matrixFrame = matrixFrame2;
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				List<UsableMachine> allUsablePointsWithTag2 = this.GetAllUsablePointsWithTag("npc_common_limited");
				if (allUsablePointsWithTag2.Count > 0)
				{
					foreach (UsableMachine usableMachine2 in allUsablePointsWithTag2)
					{
						MatrixFrame matrixFrame3;
						if (this.GetSpawnFrameFromUsableMachine(usableMachine2, out matrixFrame3))
						{
							matrixFrame = matrixFrame3;
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				List<UsableMachine> allUsablePointsWithTag3 = this.GetAllUsablePointsWithTag("npc_common");
				if (allUsablePointsWithTag3.Count > 0)
				{
					foreach (UsableMachine usableMachine3 in allUsablePointsWithTag3)
					{
						MatrixFrame matrixFrame4;
						if (this.GetSpawnFrameFromUsableMachine(usableMachine3, out matrixFrame4))
						{
							matrixFrame = matrixFrame4;
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag && this._usablePoints.Count > 0)
			{
				foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair in this._usablePoints)
				{
					if (keyValuePair.Value.Count > 0)
					{
						foreach (UsableMachine usableMachine4 in keyValuePair.Value)
						{
							MatrixFrame matrixFrame5;
							if (this.GetSpawnFrameFromUsableMachine(usableMachine4, out matrixFrame5))
							{
								matrixFrame = matrixFrame5;
								flag = true;
								break;
							}
						}
					}
				}
			}
			if (!flag && this._pairedUsablePoints.Count > 0)
			{
				foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair2 in this._pairedUsablePoints)
				{
					if (keyValuePair2.Value.Count > 0)
					{
						foreach (UsableMachine usableMachine5 in keyValuePair2.Value)
						{
							MatrixFrame matrixFrame6;
							if (this.GetSpawnFrameFromUsableMachine(usableMachine5, out matrixFrame6))
							{
								matrixFrame = matrixFrame6;
								flag = true;
								break;
							}
						}
					}
				}
			}
			matrixFrame.rotation.f.z = 0f;
			matrixFrame.rotation.f.Normalize();
			matrixFrame.rotation.u = Vec3.Up;
			matrixFrame.rotation.s = Vec3.CrossProduct(matrixFrame.rotation.f, matrixFrame.rotation.u);
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			Agent agent = this.SpawnWanderingAgent(locationCharacter, matrixFrame, true);
			MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
			return agent;
		}

		private bool GetSpawnFrameFromUsableMachine(UsableMachine usableMachine, out MatrixFrame frame)
		{
			frame = MatrixFrame.Identity;
			StandingPoint randomElementWithPredicate = Extensions.GetRandomElementWithPredicate<StandingPoint>(usableMachine.StandingPoints, (StandingPoint x) => !x.IsDeactivated && !x.IsDisabled);
			if (randomElementWithPredicate != null)
			{
				frame = randomElementWithPredicate.GameEntity.GetGlobalFrame();
				return true;
			}
			return false;
		}

		private Agent SpawnWanderingAgent(LocationCharacter locationCharacter, MatrixFrame spawnPointFrame, bool noHorses = true)
		{
			Team team = Team.Invalid;
			switch (locationCharacter.CharacterRelation)
			{
			case 0:
				team = Team.Invalid;
				break;
			case 1:
				team = base.Mission.PlayerAllyTeam;
				break;
			case 2:
				team = base.Mission.PlayerEnemyTeam;
				break;
			}
			spawnPointFrame.origin.z = base.Mission.Scene.GetGroundHeightAtPosition(spawnPointFrame.origin, 6402441);
			ValueTuple<uint, uint> agentSettlementColors = MissionAgentHandler.GetAgentSettlementColors(locationCharacter);
			AgentBuildData agentBuildData = locationCharacter.GetAgentBuildData().Team(team).InitialPosition(ref spawnPointFrame.origin);
			Vec2 vec = spawnPointFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).ClothingColor1(agentSettlementColors.Item1).ClothingColor2(agentSettlementColors.Item2)
				.CivilianEquipment(locationCharacter.UseCivilianEquipment)
				.NoHorses(noHorses);
			CharacterObject character = locationCharacter.Character;
			Banner banner;
			if (character == null)
			{
				banner = null;
			}
			else
			{
				Hero heroObject = character.HeroObject;
				if (heroObject == null)
				{
					banner = null;
				}
				else
				{
					Clan clan = heroObject.Clan;
					banner = ((clan != null) ? clan.Banner : null);
				}
			}
			AgentBuildData agentBuildData3 = agentBuildData2.Banner(banner);
			Agent agent = base.Mission.SpawnAgent(agentBuildData3, false);
			MissionAgentHandler.SetAgentExcludeFaceGroupIdAux(agent, MissionAgentHandler._disabledFaceId);
			AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(agentBuildData3.AgentMonster, MBGlobals.GetActionSet(locationCharacter.ActionSetCode), locationCharacter.Character.GetStepSize(), false);
			agent.SetActionSet(ref animationSystemData);
			agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator(locationCharacter);
			locationCharacter.AddBehaviors.Invoke(agent);
			return agent;
		}

		private static void SetAgentExcludeFaceGroupIdAux(Agent agent, int _disabledFaceId)
		{
			if (_disabledFaceId != -1)
			{
				agent.SetAgentExcludeStateForFaceGroupId(_disabledFaceId, true);
			}
		}

		public static uint GetRandomTournamentTeamColor(int teamIndex)
		{
			return MissionAgentHandler._tournamentTeamColors[teamIndex % MissionAgentHandler._tournamentTeamColors.Length];
		}

		[return: TupleElementNames(new string[] { "color1", "color2" })]
		public static ValueTuple<uint, uint> GetAgentSettlementColors(LocationCharacter locationCharacter)
		{
			CharacterObject character = locationCharacter.Character;
			if (character.IsHero)
			{
				if (character.HeroObject.Clan == CharacterObject.PlayerCharacter.HeroObject.Clan)
				{
					return new ValueTuple<uint, uint>(Clan.PlayerClan.Color, Clan.PlayerClan.Color2);
				}
				if (!character.HeroObject.IsNotable)
				{
					return new ValueTuple<uint, uint>(locationCharacter.AgentData.AgentClothingColor1, locationCharacter.AgentData.AgentClothingColor2);
				}
				return CharacterHelper.GetDeterministicColorsForCharacter(character);
			}
			else
			{
				if (character.IsSoldier)
				{
					return new ValueTuple<uint, uint>(Settlement.CurrentSettlement.MapFaction.Color, Settlement.CurrentSettlement.MapFaction.Color2);
				}
				return new ValueTuple<uint, uint>(MissionAgentHandler._villagerClothColors[MBRandom.RandomInt(MissionAgentHandler._villagerClothColors.Length)], MissionAgentHandler._villagerClothColors[MBRandom.RandomInt(MissionAgentHandler._villagerClothColors.Length)]);
			}
		}

		public UsableMachine FindUnusedPointWithTagForAgent(Agent agent, string tag)
		{
			return this.FindUnusedPointForAgent(agent, this._pairedUsablePoints, tag) ?? this.FindUnusedPointForAgent(agent, this._usablePoints, tag);
		}

		private UsableMachine FindUnusedPointForAgent(Agent agent, Dictionary<string, List<UsableMachine>> usableMachinesList, string primaryTag)
		{
			List<UsableMachine> list;
			if (usableMachinesList.TryGetValue(primaryTag, out list) && list.Count > 0)
			{
				int num = MBRandom.RandomInt(0, list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					UsableMachine usableMachine = list[(num + i) % list.Count];
					if (!usableMachine.IsDisabled && !usableMachine.IsDestroyed && usableMachine.IsStandingPointAvailableForAgent(agent))
					{
						return usableMachine;
					}
				}
			}
			return null;
		}

		public List<UsableMachine> FindAllUnusedPoints(Agent agent, string primaryTag)
		{
			List<UsableMachine> list = new List<UsableMachine>();
			List<UsableMachine> list2 = new List<UsableMachine>();
			List<UsableMachine> list3;
			this._usablePoints.TryGetValue(primaryTag, out list3);
			List<UsableMachine> list4;
			this._pairedUsablePoints.TryGetValue(primaryTag, out list4);
			list4 = ((list4 != null) ? list4.Distinct<UsableMachine>().ToList<UsableMachine>() : null);
			if (list3 != null && list3.Count > 0)
			{
				list.AddRange(list3);
			}
			if (list4 != null && list4.Count > 0)
			{
				list.AddRange(list4);
			}
			if (list.Count > 0)
			{
				Predicate<StandingPoint> <>9__0;
				foreach (UsableMachine usableMachine in list)
				{
					List<StandingPoint> standingPoints = usableMachine.StandingPoints;
					Predicate<StandingPoint> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = (StandingPoint sp) => (sp.IsInstantUse || (!sp.HasUser && !sp.HasAIMovingTo)) && !sp.IsDisabledForAgent(agent));
					}
					if (standingPoints.Exists(predicate))
					{
						list2.Add(usableMachine);
					}
				}
			}
			return list2;
		}

		public void RemovePropReference(List<GameEntity> props)
		{
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair in this._usablePoints)
			{
				foreach (GameEntity gameEntity in props)
				{
					if (gameEntity.HasTag(keyValuePair.Key))
					{
						UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
						keyValuePair.Value.Remove(firstScriptOfType);
					}
				}
			}
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair2 in this._pairedUsablePoints)
			{
				foreach (GameEntity gameEntity2 in props)
				{
					if (gameEntity2.HasTag(keyValuePair2.Key))
					{
						UsableMachine firstScriptOfType2 = gameEntity2.GetFirstScriptOfType<UsableMachine>();
						keyValuePair2.Value.Remove(firstScriptOfType2);
					}
				}
			}
		}

		public void AddPropReference(List<GameEntity> props)
		{
			foreach (KeyValuePair<string, List<UsableMachine>> keyValuePair in this._usablePoints)
			{
				foreach (GameEntity gameEntity in props)
				{
					UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
					if (firstScriptOfType != null && gameEntity.HasTag(keyValuePair.Key))
					{
						keyValuePair.Value.Add(firstScriptOfType);
					}
				}
			}
		}

		public void TeleportTargetAgentNearReferenceAgent(Agent referenceAgent, Agent teleportAgent, bool teleportFollowers, bool teleportOpposite)
		{
			Vec3 vec = referenceAgent.Position + referenceAgent.LookDirection.NormalizedCopy() * 4f;
			Vec3 vec2;
			if (teleportOpposite)
			{
				vec2 = vec;
				vec2.z = base.Mission.Scene.GetGroundHeightAtPosition(vec2, 6402441);
			}
			else
			{
				vec2 = Mission.Current.GetRandomPositionAroundPoint(referenceAgent.Position, 2f, 4f, true);
				vec2.z = base.Mission.Scene.GetGroundHeightAtPosition(vec2, 6402441);
			}
			WorldFrame worldFrame;
			worldFrame..ctor(referenceAgent.Frame.rotation, new WorldPosition(base.Mission.Scene, referenceAgent.Frame.origin));
			Vec3 vec3;
			vec3..ctor(worldFrame.Origin.AsVec2 - vec2.AsVec2, 0f, -1f);
			teleportAgent.LookDirection = vec3.NormalizedCopy();
			teleportAgent.TeleportToPosition(vec2);
			if (teleportFollowers && teleportAgent.Controller == 2)
			{
				foreach (Agent agent in base.Mission.Agents)
				{
					LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(agent.Origin);
					AccompanyingCharacter accompanyingCharacter = PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter);
					if (agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null && accompanyingCharacter != null && accompanyingCharacter.IsFollowingPlayerAtMissionStart)
					{
						MatrixFrame matrixFrame;
						this.GetFrameForFollowingAgent(teleportAgent, out matrixFrame);
						agent.TeleportToPosition(matrixFrame.origin);
					}
				}
			}
		}

		public static int GetPointCountOfUsableMachine(UsableMachine usableMachine, bool checkForUnusedOnes)
		{
			int num = 0;
			List<AnimationPoint> list = new List<AnimationPoint>();
			foreach (StandingPoint standingPoint in usableMachine.StandingPoints)
			{
				AnimationPoint animationPoint = standingPoint as AnimationPoint;
				if (animationPoint != null && animationPoint.IsActive && !standingPoint.IsDeactivated && !standingPoint.IsDisabled && !standingPoint.IsInstantUse && (!checkForUnusedOnes || (!standingPoint.HasUser && !standingPoint.HasAIMovingTo)))
				{
					List<AnimationPoint> alternatives = animationPoint.GetAlternatives();
					if (alternatives.Count == 0)
					{
						num++;
					}
					else if (!list.Contains(animationPoint))
					{
						if (checkForUnusedOnes)
						{
							if (alternatives.Any((AnimationPoint x) => x.HasUser && x.HasAIMovingTo))
							{
								continue;
							}
						}
						list.AddRange(alternatives);
						num++;
					}
				}
			}
			return num;
		}

		private const float PassageUsageDeltaTime = 30f;

		private static readonly uint[] _tournamentTeamColors = new uint[]
		{
			4294110933U, 4290269521U, 4291535494U, 4286151096U, 4290286497U, 4291600739U, 4291868275U, 4287285710U, 4283204487U, 4287282028U,
			4290300789U
		};

		private static readonly uint[] _villagerClothColors = new uint[]
		{
			4292860590U, 4291351206U, 4289117081U, 4288460959U, 4287541416U, 4288922566U, 4292654718U, 4289243320U, 4290286483U, 4290288531U,
			4290156159U, 4291136871U, 4289233774U, 4291205980U, 4291735684U, 4292722283U, 4293119406U, 4293911751U, 4294110933U, 4291535494U,
			4289955192U, 4289631650U, 4292133587U, 4288785593U, 4286288275U, 4286222496U, 4287601851U, 4286622134U, 4285898909U, 4285638289U,
			4289830302U, 4287593853U, 4289957781U, 4287071646U, 4284445583U
		};

		private static int _disabledFaceId = -1;

		private static int _disabledFaceIdForAnimals = 1;

		private readonly Dictionary<string, List<UsableMachine>> _usablePoints;

		private readonly Dictionary<string, List<UsableMachine>> _pairedUsablePoints;

		private List<UsableMachine> _disabledPassages;

		private readonly Location _previousLocation;

		private readonly Location _currentLocation;

		private readonly string _playerSpecialSpawnTag;

		private BasicMissionTimer _checkPossibleQuestTimer;

		private float _passageUsageTime;
	}
}
