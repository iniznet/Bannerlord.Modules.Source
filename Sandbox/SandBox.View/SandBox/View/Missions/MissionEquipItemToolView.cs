using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	public class MissionEquipItemToolView : MissionView
	{
		public override void AfterStart()
		{
			this._itemsXmls = new List<XmlDocument>();
			string text = ModuleHelper.GetModuleFullPath("SandBoxCore") + "ModuleData/items/";
			foreach (FileInfo fileInfo in new DirectoryInfo(text).GetFiles("*.xml"))
			{
				this._itemsXmls.Add(this.LoadXmlFile(text + fileInfo.Name));
			}
			this._cam = Camera.CreateCamera();
			this.GetItems("Item");
			this.GetItems("CraftedItem");
			foreach (Kingdom kingdom in Game.Current.ObjectManager.GetObjectTypeList<Kingdom>().ToList<Kingdom>())
			{
				if (kingdom.IsKingdomFaction || kingdom.Name.ToString() == "Looters")
				{
					this._allFactions.Add(kingdom.Culture);
				}
			}
			foreach (Clan clan in Game.Current.ObjectManager.GetObjectTypeList<Clan>().ToList<Clan>())
			{
				if (clan.Name.ToString() == "Looters")
				{
					this._allFactions.Add(clan.Culture);
				}
			}
			this._groups.Add(0);
			this._groups.Add(1);
			this._groups.Add(2);
			this._groups.Add(3);
			this._groups.Add(4);
			this._groups.Add(5);
			this._groups.Add(6);
			this._groups.Add(7);
			this._allCharacters = Game.Current.ObjectManager.GetObjectTypeList<CharacterObject>().ToList<CharacterObject>();
			this.SpawnAgent("guard");
			this.SpawnItems();
		}

		private void GetItems(string str)
		{
			List<ItemObject> list = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>().ToList<ItemObject>();
			foreach (XmlDocument xmlDocument in this._itemsXmls)
			{
				foreach (object obj in xmlDocument.DocumentElement.SelectNodes(str))
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.Attributes != null && xmlNode.Attributes["id"] != null)
					{
						string innerText = xmlNode.Attributes["id"].InnerText;
						foreach (ItemObject itemObject in list)
						{
							if (itemObject.StringId == innerText)
							{
								this._allItemObjects.Add(itemObject);
							}
						}
					}
				}
			}
		}

		public override void OnMissionTick(float dt)
		{
			this.OnEquipToolDebugTick(dt);
			if (this._init)
			{
				this._init = false;
				this.UpdateCamera();
			}
			if (this._activeIndex == -1)
			{
				if (!base.DebugInput.IsKeyDown(29) && !base.DebugInput.IsKeyDown(157) && !base.DebugInput.IsKeyDown(42) && !base.DebugInput.IsKeyDown(54) && !base.DebugInput.IsKeyDown(56) && !base.DebugInput.IsKeyDown(184))
				{
					if (base.DebugInput.IsKeyPressed(2))
					{
						this._activeIndex = 0;
					}
					else if (base.DebugInput.IsKeyPressed(3))
					{
						this._activeIndex = 1;
					}
					else if (base.DebugInput.IsKeyPressed(4))
					{
						this._activeIndex = 2;
					}
					else if (base.DebugInput.IsKeyPressed(5))
					{
						this._activeIndex = 3;
					}
					else if (base.DebugInput.IsKeyPressed(8))
					{
						this._activeWeaponSlot = 0;
					}
					else if (base.DebugInput.IsKeyPressed(9))
					{
						this._activeWeaponSlot = 1;
					}
					else if (base.DebugInput.IsKeyPressed(10))
					{
						this._activeWeaponSlot = 2;
					}
					else if (base.DebugInput.IsKeyPressed(11))
					{
						this._activeWeaponSlot = 3;
					}
					else if (base.DebugInput.IsKeyPressed(80))
					{
						this._factionIndex++;
						if (this._factionIndex >= this._allFactions.Count)
						{
							this._factionIndex = 0;
						}
					}
					else if (base.DebugInput.IsKeyPressed(79))
					{
						this._factionIndex--;
						if (this._factionIndex < 0)
						{
							this._factionIndex = this._allFactions.Count - 1;
						}
					}
					else if (base.DebugInput.IsKeyPressed(76))
					{
						this._groupIndex++;
						if (this._groupIndex >= this._groups.Count)
						{
							this._groupIndex = 0;
						}
					}
					else if (base.DebugInput.IsKeyPressed(75))
					{
						this._groupIndex--;
						if (this._groupIndex < 0)
						{
							this._groupIndex = this._groups.Count - 1;
						}
					}
					else if (base.DebugInput.IsKeyPressed(72))
					{
						this._setIndex++;
						if (this._setIndex >= this._mainAgent.Character.AllEquipments.Count + 1)
						{
							this._setIndex = 0;
						}
					}
					else if (base.DebugInput.IsKeyPressed(71))
					{
						this._setIndex--;
						if (this._setIndex < 0)
						{
							this._setIndex = this._mainAgent.Character.AllEquipments.Count - 1;
						}
					}
					else if (base.DebugInput.IsKeyPressed(229) && this._index > 0)
					{
						foreach (MissionEquipItemToolView.ItemData itemData in this._currentItems)
						{
							MatrixFrame frame = itemData.Entity.GetFrame();
							frame.origin += Vec3.Up * this._diff;
							itemData.Entity.SetFrame(ref frame);
						}
						this._index--;
					}
					else if (base.DebugInput.IsKeyPressed(230) && this._index < this._currentItems.Count - 1)
					{
						foreach (MissionEquipItemToolView.ItemData itemData2 in this._currentItems)
						{
							MatrixFrame frame2 = itemData2.Entity.GetFrame();
							frame2.origin -= Vec3.Up * this._diff;
							itemData2.Entity.SetFrame(ref frame2);
						}
						this._index++;
					}
					else if (base.DebugInput.IsKeyPressed(59))
					{
						if (!this._genderSet)
						{
							this._mainAgent.Character.IsFemale = false;
							this._genderSet = true;
							this.SpawnAgent(this._attributes[0]);
							this.SpawnItems();
						}
					}
					else if (base.DebugInput.IsKeyPressed(60) && !this._genderSet)
					{
						this._mainAgent.Character.IsFemale = true;
						this._genderSet = true;
						this.SpawnAgent(this._attributes[0]);
						this.SpawnItems();
					}
					if (base.DebugInput.IsKeyPressed(33))
					{
						if (this._currentItems.Count <= 0)
						{
							goto IL_12CF;
						}
						using (List<ItemObject>.Enumerator enumerator2 = this._allItemObjects.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								ItemObject itemObject = enumerator2.Current;
								if (itemObject.StringId == this._currentItems[this._index].Id)
								{
									int num;
									if (this._activeFilter == 5 || this._activeFilter == 9 || this._activeFilter == 6 || this._activeFilter == 7 || this._activeFilter == 8 || this._activeFilter == 10 || this._activeFilter == 11)
									{
										num = this._activeFilter;
									}
									else
									{
										num = this._activeWeaponSlot;
									}
									EquipmentIndex equipmentIndex = num;
									if ((equipmentIndex == null || equipmentIndex == 1 || equipmentIndex == 2 || equipmentIndex == 3 || equipmentIndex == 4) && !this._mainAgent.Equipment[equipmentIndex].IsEmpty)
									{
										this._mainAgent.DropItem(equipmentIndex, 0);
										base.Mission.RemoveSpawnedItemsAndMissiles();
									}
									if (itemObject.Type == 1)
									{
										this._horse = itemObject;
									}
									if (itemObject.Type == 23)
									{
										this._harness = itemObject;
									}
									if (equipmentIndex == null || equipmentIndex == 1 || equipmentIndex == 2 || equipmentIndex == 3 || equipmentIndex == 4)
									{
										ItemObject itemObject2 = itemObject;
										ItemModifier itemModifier = null;
										IAgentOriginBase origin = this._mainAgent.Origin;
										MissionWeapon missionWeapon;
										missionWeapon..ctor(itemObject2, itemModifier, (origin != null) ? origin.Banner : null);
										this._mainAgent.EquipWeaponWithNewEntity(equipmentIndex, ref missionWeapon);
									}
									Equipment equipment = this._mainAgent.SpawnEquipment.Clone(false);
									equipment[equipmentIndex] = new EquipmentElement(itemObject, null, null, false);
									BasicCharacterObject character = this._mainAgent.Character;
									this.SpawnHorse(this._horse, this._harness);
									Mat3 rotation = this._mainAgent.Frame.rotation;
									this._mainAgent.FadeOut(true, false);
									Mission mission = base.Mission;
									AgentBuildData agentBuildData = new AgentBuildData(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))).Equipment(equipment).NoHorses(true).Team(base.Mission.DefenderTeam);
									Vec3 vec = new Vec3(500f, 200f, 1f, -1f);
									AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref vec);
									Vec2 vec2 = rotation.f.AsVec2;
									this._mainAgent = mission.SpawnAgent(agentBuildData2.InitialDirection(ref vec2), false);
									foreach (Agent agent in base.Mission.Agents)
									{
										if (agent != this._mainAgent)
										{
											this._mountAgent = agent;
										}
									}
									this.UpdateCamera();
									this.UpdateActiveItems();
									break;
								}
							}
							goto IL_12CF;
						}
					}
					if (base.DebugInput.IsKeyPressed(14))
					{
						int num2 = this._activeFilter;
						if (this._activeFilter < 5 || this._activeFilter > 11)
						{
							num2 = this._activeWeaponSlot;
						}
						EquipmentIndex equipmentIndex2 = num2;
						if ((equipmentIndex2 == null || equipmentIndex2 == 1 || equipmentIndex2 == 2 || equipmentIndex2 == 3 || equipmentIndex2 == 4) && !this._mainAgent.Equipment[equipmentIndex2].IsEmpty)
						{
							this._mainAgent.DropItem(equipmentIndex2, 0);
							base.Mission.RemoveSpawnedItemsAndMissiles();
						}
						else if (equipmentIndex2 == 10)
						{
							this._mountAgent.FadeOut(true, false);
							this._horse = null;
							this.SpawnHorse(this._horse, this._harness);
						}
						else if (equipmentIndex2 == 11)
						{
							this._mountAgent.FadeOut(true, false);
							this._harness = null;
							this.SpawnHorse(this._horse, this._harness);
						}
						else
						{
							Equipment spawnEquipment = this._mainAgent.SpawnEquipment;
							spawnEquipment[equipmentIndex2] = new EquipmentElement(null, null, null, false);
							BasicCharacterObject character2 = this._mainAgent.Character;
							Mat3 rotation2 = this._mainAgent.Frame.rotation;
							this._mainAgent.FadeOut(true, false);
							Mission mission2 = base.Mission;
							AgentBuildData agentBuildData3 = new AgentBuildData(new SimpleAgentOrigin(character2, -1, null, default(UniqueTroopDescriptor))).Equipment(spawnEquipment).NoHorses(true).Team(base.Mission.DefenderTeam);
							Vec3 vec = new Vec3(500f, 200f, 1f, -1f);
							AgentBuildData agentBuildData4 = agentBuildData3.InitialPosition(ref vec);
							Vec2 vec2 = rotation2.f.AsVec2;
							this._mainAgent = mission2.SpawnAgent(agentBuildData4.InitialDirection(ref vec2), false);
						}
						this.UpdateActiveItems();
					}
					else if (base.DebugInput.IsKeyPressed(16))
					{
						this._activeFilter = 5;
						this.Clear(this._filters);
						this._filters[this._activeFilter] = true;
						this.SortFilter(12);
					}
					else if (base.DebugInput.IsKeyPressed(17))
					{
						this._activeFilter = 9;
						this.Clear(this._filters);
						this._filters[this._activeFilter] = true;
						this.SortFilter(22);
					}
					else if (base.DebugInput.IsKeyPressed(18))
					{
						this._activeFilter = 6;
						this.Clear(this._filters);
						this._filters[this._activeFilter] = true;
						this.SortFilter(13);
					}
					else if (base.DebugInput.IsKeyPressed(19))
					{
						this._activeFilter = 8;
						this.Clear(this._filters);
						this._filters[this._activeFilter] = true;
						this.SortFilter(15);
					}
					else if (base.DebugInput.IsKeyPressed(20))
					{
						this._activeFilter = 7;
						this.Clear(this._filters);
						this._filters[this._activeFilter] = true;
						this.SortFilter(14);
					}
					else if (base.DebugInput.IsKeyPressed(45))
					{
						this._activeFilter = 12;
						this.Clear(this._filters);
						this._filters[this._activeFilter] = true;
						this.SortFilter(7);
					}
					else if (base.DebugInput.IsKeyPressed(48))
					{
						this._activeFilter = 13;
						this.Clear(this._filters);
						this._filters[this._activeFilter] = true;
						this.SortFilter(8);
					}
					else if (base.DebugInput.IsKeyPressed(46))
					{
						this._activeFilter = 15;
						this.Clear(this._filters);
						this._filters[this._activeFilter] = true;
						this.SortFilter(9);
					}
					else if (base.DebugInput.IsKeyPressed(34))
					{
						this._activeFilter = 10;
						this.Clear(this._filters);
						this._filters[this._activeFilter] = true;
						this.SortFilter(1);
					}
				}
				else
				{
					if (base.DebugInput.IsKeyDown(29) || base.DebugInput.IsKeyDown(157))
					{
						if (base.DebugInput.IsKeyPressed(31))
						{
							this.SaveToXml();
						}
						if (base.DebugInput.IsKeyPressed(24))
						{
							this.CheckForLoad();
						}
						if (base.DebugInput.IsKeyPressed(18))
						{
							this.EditNode();
						}
						if (base.DebugInput.IsKeyPressed(19))
						{
							this.SpawnAgent(this._attributes[0]);
						}
						if (base.DebugInput.IsKeyPressed(79))
						{
							this._itemCulture--;
							if (this._itemCulture < -1)
							{
								this._itemCulture = this._allFactions.Count - 1;
								goto IL_12CF;
							}
							goto IL_12CF;
						}
						else if (base.DebugInput.IsKeyPressed(80))
						{
							this._itemCulture++;
							if (this._itemCulture >= this._allFactions.Count)
							{
								this._itemCulture = -1;
								goto IL_12CF;
							}
							goto IL_12CF;
						}
						else
						{
							if (!base.DebugInput.IsKeyPressed(229) && !base.DebugInput.IsKeyPressed(230))
							{
								goto IL_12CF;
							}
							float num3 = 30f;
							bool flag = base.DebugInput.IsKeyDown(56);
							if (base.DebugInput.IsKeyPressed(230))
							{
								num3 *= -1f;
							}
							using (List<MissionEquipItemToolView.ItemData>.Enumerator enumerator = this._currentItems.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									MissionEquipItemToolView.ItemData itemData3 = enumerator.Current;
									MatrixFrame matrixFrame;
									matrixFrame..ctor(itemData3.Entity.GetFrame().rotation, itemData3.Entity.GetFrame().origin);
									if (!flag)
									{
										matrixFrame.rotation.RotateAboutUp(0.017453292f * num3);
									}
									itemData3.Entity.SetFrame(ref matrixFrame);
								}
								goto IL_12CF;
							}
						}
					}
					if (base.DebugInput.IsKeyDown(42) || base.DebugInput.IsKeyDown(54))
					{
						if (base.DebugInput.IsKeyPressed(16))
						{
							this._activeFilter = 1;
							this.Clear(this._filters);
							this._filters[this._activeFilter] = true;
							this.SortFilter(2);
						}
						else if (base.DebugInput.IsKeyPressed(17))
						{
							this._activeFilter = 2;
							this.Clear(this._filters);
							this._filters[this._activeFilter] = true;
							this.SortFilter(3);
						}
						else if (base.DebugInput.IsKeyPressed(18))
						{
							this._activeFilter = 3;
							this.Clear(this._filters);
							this._filters[this._activeFilter] = true;
							this.SortFilter(4);
						}
						else if (base.DebugInput.IsKeyPressed(19))
						{
							this._activeFilter = 4;
							this.Clear(this._filters);
							this._filters[this._activeFilter] = true;
							this.SortFilter(10);
						}
						else if (base.DebugInput.IsKeyPressed(48))
						{
							this._activeFilter = 14;
							this.Clear(this._filters);
							this._filters[this._activeFilter] = true;
							this.SortFilter(5);
						}
						else if (base.DebugInput.IsKeyPressed(46))
						{
							this._activeFilter = 16;
							this.Clear(this._filters);
							this._filters[this._activeFilter] = true;
							this.SortFilter(6);
						}
						else if (base.DebugInput.IsKeyPressed(34))
						{
							this._activeFilter = 11;
							this.Clear(this._filters);
							this._filters[this._activeFilter] = true;
							this.SortFilter(23);
						}
					}
				}
			}
			else
			{
				InputKey inputKey = 11;
				if (base.DebugInput.IsKeyDown(21) && !this.yGuard)
				{
					this.str += "y";
					this.yGuard = true;
				}
				if (base.DebugInput.IsKeyDown(44) && !this.zGuard)
				{
					this.str += "z";
					this.zGuard = true;
				}
				if (base.DebugInput.IsKeyDown(45) && !this.xGuard)
				{
					this.str += "x";
					this.xGuard = true;
				}
				if (base.DebugInput.IsKeyPressed(28))
				{
					this._attributes[this._activeIndex] = ((this._activeIndex != 0) ? this.str : this.str.ToLower());
					this._activeIndex = -1;
					this.str = "";
					this.underscoreGuard = false;
					this.yGuard = false;
					this.xGuard = false;
					this.zGuard = false;
				}
				if (base.DebugInput.IsKeyPressed(1))
				{
					this._activeIndex = -1;
					this.str = "";
					this.underscoreGuard = false;
					this.yGuard = false;
					this.xGuard = false;
					this.zGuard = false;
				}
				if (base.DebugInput.IsKeyPressed(14) && this.str.Length > 0)
				{
					this.str = this.str.TrimEnd(new char[] { this.str[this.str.Length - 1] });
					this.underscoreGuard = false;
					this.yGuard = false;
					this.xGuard = false;
					this.zGuard = false;
				}
				if (base.DebugInput.IsKeyPressed(57))
				{
					this.str += " ";
					this.underscoreGuard = false;
					this.yGuard = false;
					this.xGuard = false;
					this.zGuard = false;
				}
				if (base.DebugInput.IsKeyPressed(58))
				{
					this._capsLock = !this._capsLock;
					this.underscoreGuard = false;
					this.yGuard = false;
					this.xGuard = false;
					this.zGuard = false;
				}
				if (base.DebugInput.IsKeyDown(54) && base.DebugInput.IsKeyDown(12) && !this.underscoreGuard)
				{
					this.str += "_";
					this.underscoreGuard = true;
				}
				if (base.DebugInput.IsKeyDown(29) && base.DebugInput.IsKeyPressed(47))
				{
					string clipboardText = Input.GetClipboardText();
					this.str += clipboardText;
					this.underscoreGuard = false;
					this.yGuard = false;
					this.xGuard = false;
					this.zGuard = false;
					return;
				}
				for (int i = 0; i < 40; i++)
				{
					if (base.DebugInput.IsKeyPressed(inputKey + i))
					{
						string text = (this._capsLock ? (inputKey + i).ToString().ToLower() : (inputKey + i).ToString());
						text = ((text.ToLower() == "d" + i.ToString()) ? text.ToLower().Replace("d", "") : text);
						this.str += text;
						this.underscoreGuard = false;
						this.yGuard = false;
						this.xGuard = false;
						this.zGuard = false;
					}
				}
			}
			IL_12CF:
			if (base.DebugInput.IsKeyDown(56) && !base.DebugInput.IsKeyDown(29) && (base.DebugInput.IsKeyPressed(229) || base.DebugInput.IsKeyPressed(230)))
			{
				float num4 = 60f;
				if (base.DebugInput.IsKeyPressed(230))
				{
					num4 *= -1f;
				}
				MatrixFrame frame3 = this._mainAgent.Frame;
				frame3.rotation.RotateAboutUp(0.017453292f * num4);
				this._mainAgent.SetTargetPositionAndDirection(frame3.origin.AsVec2, frame3.rotation.f);
			}
		}

		private void OnEquipToolDebugTick(float dt)
		{
			if (this._genderSet)
			{
				float num = 10f + 70f + 15f + 120f;
				for (int i = 0; i < this._currentItems.Count; i++)
				{
					int index = this._index;
					ItemObject.ItemTypeEnum itemType = this._currentItems[i].itemType;
					if (itemType != 13 && itemType != 12 && itemType != 14 && itemType != 15 && itemType != 21)
					{
						bool flag = itemType == 22;
					}
				}
			}
		}

		private void SpawnItems()
		{
			float num = -((float)this._allItemObjects.Count * this._diff / 2f);
			this._allItems.Clear();
			foreach (ItemObject itemObject in this._allItemObjects)
			{
				MatrixFrame matrixFrame;
				matrixFrame..ctor(this._mainAgent.Frame.rotation, this._mainAgent.Position + new Vec3(-1f, 1f, 0f, -1f) + Vec3.Up * num);
				GameEntity gameEntity = GameEntity.CreateEmpty(Mission.Current.Scene, true);
				MetaMesh copy = MetaMesh.GetCopy(itemObject.MultiMeshName, true, false);
				gameEntity.AddMultiMesh(copy, true);
				gameEntity.SetFrame(ref matrixFrame);
				gameEntity.SetVisibilityExcludeParents(false);
				MissionEquipItemToolView.ItemData itemData = new MissionEquipItemToolView.ItemData();
				itemData.Entity = gameEntity;
				itemData.Name = itemObject.Name.ToString();
				itemData.Id = itemObject.StringId;
				itemData.Culture = itemObject.Culture;
				if (Extensions.HasAnyFlag<ItemFlags>(itemObject.ItemFlags, 1024))
				{
					itemData.Gender = MissionEquipItemToolView.GenderEnum.Male;
				}
				else if (Extensions.HasAnyFlag<ItemFlags>(itemObject.ItemFlags, 2048))
				{
					itemData.Gender = MissionEquipItemToolView.GenderEnum.Female;
				}
				else
				{
					itemData.Gender = MissionEquipItemToolView.GenderEnum.Unisex;
				}
				itemData.itemType = itemObject.ItemType;
				this._allItems.Add(itemData);
				num += this._diff;
			}
		}

		private void SortFilter(ItemObject.ItemTypeEnum type)
		{
			this._currentItems.Clear();
			this._currentArmorValues.Clear();
			using (List<MissionEquipItemToolView.ItemData>.Enumerator enumerator = this._allItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MissionEquipItemToolView.ItemData item = enumerator.Current;
					if (item.itemType == type && (this._itemCulture == -1 || item.Culture == this._allFactions[this._itemCulture]))
					{
						bool flag = false;
						int j;
						for (j = 0; j < this._currentItems.Count; j++)
						{
							string text = this._currentItems[j].Name.ToLower();
							int num = 0;
							while (num < item.Name.Length && num < text.Length)
							{
								if (item.Name.ToLower()[num] < text[num])
								{
									flag = true;
									break;
								}
								if (item.Name.ToLower()[num] > text[num])
								{
									break;
								}
								num++;
							}
							if (flag)
							{
								break;
							}
						}
						if (item.Gender == MissionEquipItemToolView.GenderEnum.Unisex || (this._mainAgent.Character.IsFemale && item.Gender == MissionEquipItemToolView.GenderEnum.Female) || (!this._mainAgent.Character.IsFemale && item.Gender == MissionEquipItemToolView.GenderEnum.Male))
						{
							this._currentItems.Insert(j, item);
							ItemComponent itemComponent = this._allItemObjects.Where((ItemObject i) => i.StringId == item.Id).FirstOrDefault<ItemObject>().ItemComponent;
							ArmorComponent armorComponent = ((itemComponent != null && itemComponent is ArmorComponent) ? ((ArmorComponent)itemComponent) : null);
							int num2 = 0;
							int num3 = 0;
							int num4 = 0;
							int num5 = 0;
							if (armorComponent != null)
							{
								num2 = armorComponent.HeadArmor;
								num3 = armorComponent.BodyArmor;
								num4 = armorComponent.LegArmor;
								num5 = armorComponent.ArmArmor;
							}
							Tuple<int, int, int, int> tuple = new Tuple<int, int, int, int>(num2, num3, num4, num5);
							this._currentArmorValues.Insert(j, tuple);
						}
					}
					else
					{
						item.Entity.SetVisibilityExcludeParents(false);
					}
				}
			}
			this.PositionCurrentItems();
		}

		private void SpawnHorse(ItemObject horse, ItemObject harness)
		{
			ItemRosterElement itemRosterElement = default(ItemRosterElement);
			ItemRosterElement itemRosterElement2 = default(ItemRosterElement);
			if (horse != null)
			{
				itemRosterElement..ctor(horse, 1, null);
				if (harness != null)
				{
					itemRosterElement2..ctor(harness, 1, null);
				}
				else
				{
					itemRosterElement2 = ItemRosterElement.Invalid;
				}
			}
			else
			{
				if (harness == null)
				{
					return;
				}
				itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("mule"), 1, null);
				itemRosterElement2..ctor(harness, 1, null);
			}
			if (this._mountAgent != null)
			{
				this._mountAgent.FadeOut(true, false);
			}
			this._horse = itemRosterElement.EquipmentElement.Item;
			this._harness = itemRosterElement2.EquipmentElement.Item;
			Mission mission = base.Mission;
			ItemRosterElement itemRosterElement3 = itemRosterElement;
			ItemRosterElement itemRosterElement4 = itemRosterElement2;
			Vec3 vec = new Vec3(500f, this._mainAgent.Position.y - 3f, 1f, -1f);
			this._mountAgent = mission.SpawnMonster(itemRosterElement3, itemRosterElement4, ref vec, ref Vec2.Forward, -1);
			this._mountAgent.Controller = 0;
		}

		private void SpawnAgent(string id)
		{
			Agent mountAgent = this._mountAgent;
			if (mountAgent != null)
			{
				mountAgent.FadeOut(true, false);
			}
			Agent mainAgent = this._mainAgent;
			if (mainAgent != null)
			{
				mainAgent.FadeOut(true, false);
			}
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(id);
			List<Equipment> list = @object.AllEquipments.ToList<Equipment>();
			Mission mission = base.Mission;
			AgentBuildData agentBuildData = new AgentBuildData(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Equipment(list[this._setIndex]).NoHorses(true).Team(base.Mission.DefenderTeam);
			Vec3 vec = new Vec3(500f, 200f, 1f, -1f);
			this._mainAgent = mission.SpawnAgent(agentBuildData.InitialPosition(ref vec).InitialDirection(ref Vec2.Forward), false);
			if (list[this._setIndex].Horse.Item != null)
			{
				this.SpawnHorse(list[this._setIndex].Horse.Item, list[this._setIndex].GetEquipmentFromSlot(11).Item);
			}
			this._groupIndex = @object.DefaultFormationGroup;
			this._attributes[0] = @object.StringId;
			this._attributes[1] = @object.Name.ToString();
			this._attributes[2] = @object.Level.ToString();
			this._attributes[3] = @object.Occupation.ToString();
			for (int i = 0; i < this._attributes.Length; i++)
			{
				this._spawnAttributes[i] = this._attributes[i];
			}
			this._groupIndex = @object.DefaultFormationGroup;
			for (int j = 0; j < this._allFactions.Count; j++)
			{
				if (this._allFactions[j].StringId == @object.Culture.StringId)
				{
					this._factionIndex = j;
					this._itemCulture = -1;
					break;
				}
			}
			this._spawnSetIndex = this._setIndex;
			this.UpdateActiveItems();
			this.UpdateCamera();
		}

		private void PositionCurrentItems()
		{
			float num = 0f;
			float num2 = 0f;
			if (this._activeFilter == 6)
			{
				this._diff = 1.5f;
			}
			else if (this._activeFilter == 10 || this._activeFilter == 11)
			{
				this._diff = 4f;
				num = -1f;
			}
			else if (this._activeFilter == 1 || this._activeFilter == 2 || this._activeFilter == 14 || this._activeFilter == 16)
			{
				this._diff = 1.5f;
			}
			else if (this._activeFilter == 13 || this._activeFilter == 15)
			{
				this._diff = 2.5f;
				num2 = 1f;
			}
			else
			{
				this._diff = 0.75f;
			}
			float num3 = -((float)(this._currentItems.Count / 2) * this._diff);
			this._textStart = this._mainAgent.Position + new Vec3(-1f, 1f, 0f, -1f) + Vec3.Up * num3;
			foreach (MissionEquipItemToolView.ItemData itemData in this._currentItems)
			{
				MatrixFrame matrixFrame;
				matrixFrame..ctor(Mat3.Identity, this._mainAgent.Position + new Vec3(-1f, 1f + num, num2, -1f) + Vec3.Up * num3);
				itemData.Entity.SetVisibilityExcludeParents(true);
				itemData.Entity.SetFrame(ref matrixFrame);
				num3 += this._diff;
				if (itemData.Entity.GetMetaMesh(0) != null)
				{
					BoundingBox boundingBox = itemData.Entity.GetMetaMesh(0).GetBoundingBox();
					if (!this._bounds.Contains(boundingBox))
					{
						this._bounds.Add(boundingBox);
					}
				}
			}
			this._index = this._currentItems.Count / 2;
			this._pivotDiff = 0f;
			foreach (BoundingBox boundingBox2 in this._bounds)
			{
				this._pivotDiff += boundingBox2.center.z;
			}
			this._pivotDiff /= (float)this._bounds.Count;
			this._bounds.Clear();
		}

		private void EditNode()
		{
			this._charactersXml = this.LoadXmlFile(ModuleHelper.GetModuleFullPath("SandBoxCore") + "ModuleData/spnpccharacters.xml");
			this._charactersXml.DocumentElement.SelectNodes("NPCCharacter");
			foreach (object obj in this._charactersXml.DocumentElement.SelectNodes("NPCCharacter"))
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Attributes["id"] != null && this._spawnAttributes[0] == xmlNode.Attributes["id"].InnerText)
				{
					xmlNode.Attributes["id"].InnerText = this._attributes[0];
					xmlNode.Attributes["name"].InnerText = this._attributes[1];
					if (xmlNode.Attributes["level"] != null)
					{
						xmlNode.Attributes["level"].InnerText = this._attributes[2];
					}
					xmlNode.Attributes["occupation"].InnerText = this._attributes[3];
					xmlNode.Attributes["culture"].InnerText = "Culture." + this._allFactions[this._factionIndex].StringId;
					xmlNode.Attributes["default_group"].InnerText = FormationClassExtensions.GetName(this._groups[this._groupIndex]);
					this.SlotCheck("Head", 0, xmlNode, null);
					this.SlotCheck("Cape", 1, xmlNode, null);
					this.SlotCheck("Body", 2, xmlNode, null);
					this.SlotCheck("Gloves", 3, xmlNode, null);
					this.SlotCheck("Leg", 4, xmlNode, null);
					this.SlotCheck("Item0", 5, xmlNode, null);
					this.SlotCheck("Item1", 6, xmlNode, null);
					this.SlotCheck("Item2", 7, xmlNode, null);
					this.SlotCheck("Item3", 8, xmlNode, null);
					this.SlotCheck("Horse", -1, xmlNode, this._horse);
					this.SlotCheck("HorseHarness", -1, xmlNode, this._harness);
					this._charactersXml.Save(ModuleHelper.GetModuleFullPath("SandBoxCore") + "ModuleData/spnpccharacters.xml");
					for (int i = 0; i < this._attributes.Length; i++)
					{
						this._spawnAttributes[i] = this._attributes[i];
					}
				}
			}
		}

		private void CheckForLoad()
		{
			if (this._spawnAttributes[0] != this._attributes[0] && Game.Current.ObjectManager.GetObject<CharacterObject>(this._attributes[0]) != null)
			{
				this.SpawnAgent(this._attributes[0]);
				return;
			}
			if (this._spawnAttributes[1] != this._attributes[1])
			{
				foreach (CharacterObject characterObject in this._allCharacters)
				{
					if (characterObject.Name.ToString() == this._attributes[1])
					{
						this.SpawnAgent(characterObject.StringId);
						return;
					}
				}
			}
			if (this._setIndex != this._spawnSetIndex)
			{
				this.SpawnAgent(this._mainAgent.Character.StringId);
			}
		}

		private void UpdateActiveItems()
		{
			this._activeItems.Clear();
			this._activeItems.Add(this._mainAgent.SpawnEquipment[5].Item);
			this._activeItems.Add(this._mainAgent.SpawnEquipment[9].Item);
			this._activeItems.Add(this._mainAgent.SpawnEquipment[6].Item);
			this._activeItems.Add(this._mainAgent.SpawnEquipment[8].Item);
			this._activeItems.Add(this._mainAgent.SpawnEquipment[7].Item);
			MissionWeapon missionWeapon = this._mainAgent.Equipment[0];
			if (missionWeapon.WeaponsCount > 0)
			{
				this._activeItems.Add(missionWeapon.Item);
			}
			MissionWeapon missionWeapon2 = this._mainAgent.Equipment[1];
			if (missionWeapon2.WeaponsCount > 0)
			{
				this._activeItems.Add(missionWeapon2.Item);
			}
			MissionWeapon missionWeapon3 = this._mainAgent.Equipment[2];
			if (missionWeapon3.WeaponsCount > 0)
			{
				this._activeItems.Add(missionWeapon3.Item);
			}
			MissionWeapon missionWeapon4 = this._mainAgent.Equipment[3];
			if (missionWeapon4.WeaponsCount > 0)
			{
				this._activeItems.Add(missionWeapon4.Item);
			}
			this._activeItems.Add(this._mainAgent.SpawnEquipment[10].Item);
			this._activeItems.Add(this._mainAgent.SpawnEquipment[11].Item);
		}

		private void SlotCheck(string slotName, int index, XmlNode parentNode, ItemObject obj = null)
		{
			XmlNodeList xmlNodeList = parentNode.SelectNodes("equipmentSet")[this._setIndex].SelectNodes("equipment");
			bool flag = false;
			foreach (object obj2 in xmlNodeList)
			{
				XmlNode xmlNode = (XmlNode)obj2;
				if (xmlNode.Attributes != null && xmlNode.Attributes["slot"].InnerText == slotName)
				{
					if ((index != -1 && this._activeItems[index] == null) || (index == -1 && obj == null))
					{
						xmlNode.ParentNode.RemoveChild(xmlNode);
						return;
					}
					xmlNode.Attributes["id"].Value = "Item." + ((obj == null) ? this._activeItems[index].StringId : obj.StringId);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if ((index != -1 && this._activeItems[index] == null) || (index == -1 && obj == null))
				{
					return;
				}
				XmlElement xmlElement = this._charactersXml.CreateElement("equipment");
				XmlAttribute xmlAttribute = this._charactersXml.CreateAttribute("slot");
				xmlAttribute.Value = slotName;
				XmlAttribute xmlAttribute2 = this._charactersXml.CreateAttribute("id");
				xmlAttribute2.Value = "Item." + ((obj == null) ? this._activeItems[index].StringId : obj.StringId);
				xmlElement.Attributes.Append(xmlAttribute);
				xmlElement.Attributes.Append(xmlAttribute2);
				parentNode.SelectNodes("equipmentSet")[this._setIndex].AppendChild(xmlElement);
			}
		}

		private void UpdateCamera()
		{
			Vec3 vec = ((this._mainAgent.MountAgent == null) ? new Vec3(1.3f, 2f, 1f, -1f) : new Vec3(2f, 3f, 2f, -1f));
			MatrixFrame matrixFrame = default(MatrixFrame);
			matrixFrame.rotation.u = -(this._mainAgent.Position - this._cam.Position).NormalizedCopy();
			matrixFrame.rotation.f = Vec3.Up;
			matrixFrame.rotation.s = Vec3.CrossProduct(matrixFrame.rotation.f, matrixFrame.rotation.u);
			matrixFrame.rotation.s.Normalize();
			matrixFrame.rotation.Orthonormalize();
			float aspectRatio = Screen.AspectRatio;
			this._cam.SetFovVertical(1.134464f, aspectRatio, 1E-08f, 1000f);
			matrixFrame.origin = this._mainAgent.Position + vec;
			this._cam.Frame = matrixFrame;
			base.MissionScreen.CustomCamera = this._cam;
		}

		private void SaveToXml()
		{
			this._charactersXml = this.LoadXmlFile(ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/spnpccharacters.xml");
			XmlNodeList xmlNodeList = this._charactersXml.DocumentElement.SelectNodes("/NPCCharacters");
			bool flag = false;
			string text = "\n  <equipmentSet>\n";
			foreach (object obj in this._charactersXml.DocumentElement.SelectNodes("NPCCharacter"))
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Attributes["id"] != null && this._attributes[0] == xmlNode.Attributes["id"].InnerText)
				{
					flag = true;
					if (this._setIndex > this._spawnSetIndex)
					{
						for (int i = 0; i < 9; i++)
						{
							if (this._activeItems[i] != null)
							{
								string text2 = "";
								if (i == 0)
								{
									text2 = "Head";
								}
								else if (i == 1)
								{
									text2 = "Cape";
								}
								else if (i == 2)
								{
									text2 = "Body";
								}
								else if (i == 3)
								{
									text2 = "Gloves";
								}
								else if (i == 4)
								{
									text2 = "Leg";
								}
								else if (i == 5)
								{
									text2 = "Item0";
								}
								else if (i == 6)
								{
									text2 = "Item1";
								}
								else if (i == 7)
								{
									text2 = "Item2";
								}
								else if (i == 8)
								{
									text2 = "Item3";
								}
								text = string.Concat(new string[]
								{
									text,
									"    <equipment slot=\"",
									text2,
									"\" id=\"Item.",
									this._activeItems[i].StringId,
									"\" />\n"
								});
							}
						}
						if (this._horse != null)
						{
							text = text + "    <equipment slot=\"Horse\" id=\"Item." + this._horse.StringId + "\" />\n";
						}
						if (this._harness != null)
						{
							text = text + "    <equipment slot=\"HorseHarness\" id=\"Item." + this._harness.StringId + "\" />\n";
						}
						text += "  </equipmentSet>\n";
						XmlNode xmlNode2 = xmlNode;
						xmlNode2.InnerXml += text;
						this._charactersXml.Save(ModuleHelper.GetModuleFullPath("SandBoxCore") + "ModuleData/spnpccharacters.xml");
						Utilities.ConstructMainThreadJob(new MissionEquipItemToolView.MainThreadDelegate(Mission.Current.EndMission), Array.Empty<object>());
						MBGameManager.EndGame();
						for (int j = 0; j < 10; j++)
						{
						}
						break;
					}
					return;
				}
				else if (xmlNode.Attributes["id"] == null || this._attributes[1] == xmlNode.Attributes["name"].InnerText)
				{
				}
			}
			if (!flag)
			{
				XmlElement xmlElement = this._charactersXml.CreateElement("NPCCharacter");
				XmlAttribute xmlAttribute = this._charactersXml.CreateAttribute("id");
				xmlAttribute.Value = this._attributes[0];
				xmlElement.Attributes.Append(xmlAttribute);
				XmlAttribute xmlAttribute2 = this._charactersXml.CreateAttribute("default_group");
				xmlAttribute2.Value = FormationClassExtensions.GetName(this._groups[this._groupIndex]);
				xmlElement.Attributes.Append(xmlAttribute2);
				XmlAttribute xmlAttribute3 = this._charactersXml.CreateAttribute("level");
				xmlAttribute3.Value = this._attributes[2];
				xmlElement.Attributes.Append(xmlAttribute3);
				XmlAttribute xmlAttribute4 = this._charactersXml.CreateAttribute("name");
				xmlAttribute4.Value = this._attributes[1];
				xmlElement.Attributes.Append(xmlAttribute4);
				XmlAttribute xmlAttribute5 = this._charactersXml.CreateAttribute("occupation");
				xmlAttribute5.Value = this._attributes[3];
				xmlElement.Attributes.Append(xmlAttribute5);
				XmlAttribute xmlAttribute6 = this._charactersXml.CreateAttribute("culture");
				xmlAttribute6.Value = "Culture." + this._allFactions[this._factionIndex].StringId;
				xmlElement.Attributes.Append(xmlAttribute6);
				XmlElement xmlElement2 = this._charactersXml.CreateElement("face");
				XmlElement xmlElement3 = this._charactersXml.CreateElement("face_key_template");
				XmlAttribute xmlAttribute7 = this._charactersXml.CreateAttribute("value");
				xmlAttribute7.Value = "NPCCharacter.villager_vlandia";
				xmlElement3.Attributes.Append(xmlAttribute7);
				xmlElement2.AppendChild(xmlElement3);
				xmlElement.AppendChild(xmlElement2);
				XmlElement xmlElement4 = this._charactersXml.CreateElement("equipmentSet");
				for (int k = 0; k < 9; k++)
				{
					if (this._activeItems[k] != null)
					{
						XmlElement xmlElement5 = this._charactersXml.CreateElement("equipment");
						XmlAttribute xmlAttribute8 = this._charactersXml.CreateAttribute("slot");
						string text3 = "";
						if (k == 0)
						{
							text3 = "Head";
						}
						else if (k == 1)
						{
							text3 = "Cape";
						}
						else if (k == 2)
						{
							text3 = "Body";
						}
						else if (k == 3)
						{
							text3 = "Gloves";
						}
						else if (k == 4)
						{
							text3 = "Leg";
						}
						else if (k == 5)
						{
							text3 = "Item0";
						}
						else if (k == 6)
						{
							text3 = "Item1";
						}
						else if (k == 7)
						{
							text3 = "Item2";
						}
						else if (k == 8)
						{
							text3 = "Item3";
						}
						xmlAttribute8.Value = text3;
						xmlElement5.Attributes.Append(xmlAttribute8);
						XmlAttribute xmlAttribute9 = this._charactersXml.CreateAttribute("id");
						xmlAttribute9.Value = "Item." + this._activeItems[k].StringId;
						xmlElement5.Attributes.Append(xmlAttribute9);
						xmlElement4.AppendChild(xmlElement5);
					}
				}
				xmlElement.AppendChild(xmlElement4);
				if (this._horse != null)
				{
					XmlElement xmlElement6 = this._charactersXml.CreateElement("equipment");
					XmlAttribute xmlAttribute10 = this._charactersXml.CreateAttribute("slot");
					xmlAttribute10.Value = "Horse";
					xmlElement6.Attributes.Append(xmlAttribute10);
					XmlAttribute xmlAttribute11 = this._charactersXml.CreateAttribute("id");
					xmlAttribute11.Value = "Item." + this._horse.StringId;
					xmlElement6.Attributes.Append(xmlAttribute11);
					xmlElement.AppendChild(xmlElement6);
				}
				if (this._harness != null)
				{
					XmlElement xmlElement7 = this._charactersXml.CreateElement("equipment");
					XmlAttribute xmlAttribute12 = this._charactersXml.CreateAttribute("slot");
					xmlAttribute12.Value = "HorseHarness";
					xmlElement7.Attributes.Append(xmlAttribute12);
					XmlAttribute xmlAttribute13 = this._charactersXml.CreateAttribute("id");
					xmlAttribute13.Value = "Item." + this._harness.StringId;
					xmlElement7.Attributes.Append(xmlAttribute13);
					xmlElement.AppendChild(xmlElement7);
				}
				xmlNodeList[xmlNodeList.Count - 1].AppendChild(xmlElement);
				this._charactersXml.Save(ModuleHelper.GetModuleFullPath("SandBoxCore") + "ModuleData/spnpccharacters.xml");
				Utilities.ConstructMainThreadJob(new MissionEquipItemToolView.MainThreadDelegate(Mission.Current.EndMission), Array.Empty<object>());
				MBGameManager.EndGame();
				for (int l = 0; l < 10; l++)
				{
				}
			}
		}

		private void Clear(bool[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = false;
			}
		}

		private XmlDocument LoadXmlFile(string path)
		{
			Debug.Print("opening " + path, 0, 12, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadToEnd();
			xmlDocument.LoadXml(text);
			streamReader.Close();
			return xmlDocument;
		}

		private string str = "";

		private int _itemCulture;

		private bool[] _filters = new bool[17];

		private bool _genderSet;

		private Agent _mainAgent;

		private List<ItemObject> _allItemObjects = new List<ItemObject>();

		private List<MissionEquipItemToolView.ItemData> _allItems = new List<MissionEquipItemToolView.ItemData>();

		private List<MissionEquipItemToolView.ItemData> _currentItems = new List<MissionEquipItemToolView.ItemData>();

		private List<Tuple<int, int, int, int>> _currentArmorValues = new List<Tuple<int, int, int, int>>();

		private List<CultureObject> _allFactions = new List<CultureObject>();

		private List<CharacterObject> _allCharacters = new List<CharacterObject>();

		private List<FormationClass> _groups = new List<FormationClass>();

		private int _activeIndex = -1;

		private int _factionIndex;

		private int _groupIndex;

		private XmlDocument _charactersXml;

		private List<XmlDocument> _itemsXmls;

		private string[] _attributes = new string[] { "id", "name", "level", "occupation", "culture", "group" };

		private string[] _spawnAttributes = new string[] { "id", "name", "level", "occupation", "culture", "group" };

		private bool underscoreGuard;

		private bool yGuard;

		private bool zGuard;

		private bool xGuard;

		private bool _capsLock;

		private List<ItemObject> _activeItems = new List<ItemObject>();

		private int _setIndex;

		private int _spawnSetIndex;

		private Camera _cam;

		private bool _init = true;

		private int _index;

		private float _diff = 0.75f;

		private int _activeFilter;

		private int _activeWeaponSlot;

		private Vec3 _textStart;

		private List<BoundingBox> _bounds = new List<BoundingBox>();

		private float _pivotDiff;

		private Agent _mountAgent;

		private ItemObject _horse;

		private ItemObject _harness;

		private enum Filter
		{
			Head = 5,
			Cape = 9,
			Body = 6,
			Hand = 8,
			Leg = 7,
			Shield = 12,
			Bow,
			Crossbow = 15,
			Horse = 10,
			Onehanded = 1,
			Twohanded,
			Polearm,
			Thrown,
			Arrow = 14,
			Bolt = 16,
			Harness = 11
		}

		private delegate void MainThreadDelegate();

		private class ItemData
		{
			public GameEntity Entity;

			public string Name;

			public string Id;

			public BasicCultureObject Culture;

			public ItemObject.ItemTypeEnum itemType;

			public MissionEquipItemToolView.GenderEnum Gender;
		}

		public enum GenderEnum
		{
			Male = 1,
			Unisex,
			Female
		}
	}
}
