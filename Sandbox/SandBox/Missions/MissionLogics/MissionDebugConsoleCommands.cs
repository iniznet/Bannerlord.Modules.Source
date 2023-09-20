using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace SandBox.Missions.MissionLogics
{
	public static class MissionDebugConsoleCommands
	{
		private static void LoadItems()
		{
			if (MissionDebugConsoleCommands._aiDebugTextItems == null)
			{
				string text = BasePath.Name + "/Modules/SandBoxCore/ModuleData/spitems";
				MBStringBuilder mbstringBuilder = default(MBStringBuilder);
				mbstringBuilder.Initialize(2048, "LoadItems");
				foreach (FileInfo fileInfo in new DirectoryInfo(text).GetFiles("*.xml"))
				{
					StreamReader streamReader = new StreamReader(text + "/" + fileInfo.Name);
					XmlDocument xmlDocument = new XmlDocument();
					string text2 = streamReader.ReadToEnd();
					xmlDocument.LoadXml(text2);
					streamReader.Close();
					for (int j = 0; j < xmlDocument.ChildNodes.Count; j++)
					{
						XmlNode xmlNode = xmlDocument.ChildNodes[j].FirstChild;
						for (;;)
						{
							if (xmlNode != null && xmlNode.Name == "Item")
							{
								XmlAttribute xmlAttribute = xmlNode.Attributes["id"];
								bool flag = false;
								for (int k = 0; k < xmlNode.ChildNodes.Count; k++)
								{
									XmlNode firstChild = xmlNode.ChildNodes[k].FirstChild;
									if (firstChild != null && firstChild.Name.ToLower() == "flag" && firstChild.Attributes["name"].Value.ToLower() == "type")
									{
										string value = firstChild.Attributes["value"].Value;
										mbstringBuilder.AppendLine<string>(xmlAttribute.Value + " (" + value + ")");
										flag = true;
									}
								}
								if (!flag)
								{
									mbstringBuilder.AppendLine<string>(xmlAttribute.Value);
								}
							}
							if (xmlNode == null || xmlNode.NextSibling == null)
							{
								break;
							}
							xmlNode = xmlNode.NextSibling;
						}
					}
				}
				MissionDebugConsoleCommands._aiDebugTextItems = mbstringBuilder.ToStringAndRelease();
			}
		}

		private static void LoadChars()
		{
			if (MissionDebugConsoleCommands._aiDebugTextChars == null)
			{
				MBStringBuilder mbstringBuilder = default(MBStringBuilder);
				mbstringBuilder.Initialize(2048, "LoadChars");
				XmlDocument xmlDocument = new XmlDocument();
				StreamReader streamReader = new StreamReader(ModuleHelper.GetModuleFullPath("SandBoxCore") + "ModuleData/spnpccharacters.xml");
				string text = streamReader.ReadToEnd();
				xmlDocument.LoadXml(text);
				streamReader.Close();
				for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
				{
					XmlNode xmlNode = xmlDocument.ChildNodes[i].FirstChild;
					for (;;)
					{
						if (xmlNode != null && xmlNode.Name == "NPCCharacter")
						{
							string value = xmlNode.Attributes["id"].Value;
							string value2 = xmlNode.Attributes["default_group"].Value;
							mbstringBuilder.AppendLine<string>(value + " (" + value2 + ")");
						}
						if (xmlNode == null || xmlNode.NextSibling == null)
						{
							break;
						}
						xmlNode = xmlNode.NextSibling;
					}
				}
				MissionDebugConsoleCommands._aiDebugTextChars = mbstringBuilder.ToStringAndRelease();
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("delete_agent", "agent")]
		public static string RemoveAgent(List<string> strings)
		{
			if (strings.Count != 1)
			{
				return "delete_agent AGENT_ID";
			}
			int num = int.Parse(strings[0]);
			Agent agent = Mission.Current.FindAgentWithIndex(num);
			if (agent == null || !agent.IsActive())
			{
				return "No active agent with ID " + num;
			}
			agent.FadeOut(false, false);
			return "Agent with ID " + num + " is removed.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("fade_out", "agent")]
		public static string FadeOutAgent(List<string> strings)
		{
			int value;
			if (strings.Count != 1 || (!(strings[0].ToLower() == "all") && !int.TryParse(strings[0], out value)))
			{
				return "fade_out all : To fade out all ai controlled agents\nfade_out AGENT_INDEX : To fade out agent with that index";
			}
			if (strings[0].ToLower() == "all")
			{
				List<Agent> list = new List<Agent>();
				list.AddRange(Mission.Current.Agents);
				foreach (Agent agent in list)
				{
					if (agent.IsAIControlled)
					{
						agent.FadeOut(false, false);
					}
				}
				return "success";
			}
			if (int.TryParse(strings[0], out value))
			{
				Agent agent2 = Mission.Current.Agents.FirstOrDefault((Agent x) => x.Index == value);
				if (agent2 != null && agent2.IsActive())
				{
					agent2.FadeOut(false, true);
				}
			}
			return "success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("fade_in", "agent")]
		public static string FadeInAgent(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			int value;
			if (strings.Count != 1 || (!(strings[0].ToLower() == "all") && !int.TryParse(strings[0], out value)))
			{
				return "fade_in all : To fade in all ai controlled agents\nfade_in AGENT_INDEX : To fade in agent with that index";
			}
			if (strings[0].ToLower() == "all")
			{
				foreach (Agent agent in Mission.Current.Agents)
				{
					if (agent.IsAIControlled)
					{
						agent.FadeIn();
					}
				}
				return "success";
			}
			if (int.TryParse(strings[0], out value))
			{
				Agent agent2 = Mission.Current.Agents.FirstOrDefault((Agent x) => x.Index == value);
				if (agent2 != null && agent2.IsActive())
				{
					agent2.FadeIn();
				}
			}
			return "success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("main_agent_play_action_at_channel", "agent")]
		public static string PlayMainHeroAnimation(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (strings.Count != 2)
			{
				return "Example: main_agent_play_action_at_channel 0 act_jump";
			}
			int num;
			if (int.TryParse(strings[0], out num))
			{
				Agent.Main.SetActionChannel(num, ActionIndexValueCache.Create(strings[1]), true, (ulong)int.MinValue, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
			return "DONE!";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("main_mount_play_action_at_channel", "agent")]
		public static string PlayMainMountAnimation(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (strings.Count != 2)
			{
				return "Example: main_agent_play_action_at_channel 0 act_jump";
			}
			if (Agent.Main.MountAgent == null)
			{
				return "Main agent does not have a mount!";
			}
			int num;
			if (int.TryParse(strings[0], out num))
			{
				Agent.Main.MountAgent.SetActionChannel(num, ActionIndexValueCache.Create(strings[1]), true, (ulong)int.MinValue, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
			return "DONE!";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("change_action_set", "agent")]
		public static string ChangePlayerActionSet(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (strings.Count != 1)
			{
				return "change_action_set ACTION_SET_NAME \nACTION_SET_NAME can be villager, female_villager, warrior";
			}
			string text = strings[0];
			string text2 = "";
			if (!(text == "villager"))
			{
				if (!(text == "female_villager"))
				{
					if (!(text == "warrior"))
					{
						if (text == "female_warrior")
						{
							text2 = ActionSetCode.GenerateActionSetNameWithSuffix(Agent.Main.Monster, true, "_warrior");
						}
					}
					else
					{
						text2 = ActionSetCode.GenerateActionSetNameWithSuffix(Agent.Main.Monster, false, "_warrior");
					}
				}
				else
				{
					text2 = ActionSetCode.GenerateActionSetNameWithSuffix(Agent.Main.Monster, true, "_villager");
				}
			}
			else
			{
				text2 = ActionSetCode.GenerateActionSetNameWithSuffix(Agent.Main.Monster, false, "_villager");
			}
			if (Extensions.IsEmpty<char>(text2))
			{
				return "change_action_set ACTION_SET_NAME \nACTION_SET_NAME can be villager, female_villager, warrior";
			}
			AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(Agent.Main.Monster, MBGlobals.GetActionSet(text2), CharacterObject.PlayerCharacter.GetStepSize(), false);
			Agent.Main.SetActionSet(ref animationSystemData);
			return text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_health", "ai")]
		public static string AISetHealth(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (Mission.Current == null)
			{
				return "Mission is null";
			}
			if (strings.Count > 0 && strings[0] == "?")
			{
				return "Sets given agent health to given value. (e.g: ai.set_health index:0 health:150)";
			}
			int index = 0;
			int num = 100;
			foreach (string text in strings)
			{
				string text2 = text.Trim();
				if (text2.Length > 0)
				{
					string[] array = text2.Split(new char[] { ':' });
					if (array.Length == 2)
					{
						if (array[0] == "index")
						{
							int.TryParse(array[1], out index);
						}
						else if (array[1] == "health")
						{
							int.TryParse(array[1], out num);
						}
					}
				}
			}
			Agent agent = Mission.Current.Agents.FirstOrDefault((Agent a) => a.Index == index);
			if (agent != null)
			{
				agent.Health = (float)num;
				return string.Concat(new object[] { "Agent ", index, " health set to ", num });
			}
			return "Cannot find agent";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("show_items", "ai")]
		public static string AIShowItems(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (strings.Count > 0 && strings[0] == "?")
			{
				return "Shows spitems.xml content\r\nfilter: filters items (e.g: filter:sword)";
			}
			MissionDebugConsoleCommands.LoadItems();
			string text = MissionDebugConsoleCommands.AISpawnParameters.GetParameter(strings, "filter", "");
			if (text == "")
			{
				return MissionDebugConsoleCommands._aiDebugTextItems;
			}
			text = text.ToLower();
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(2048, "AIShowItems");
			foreach (string text2 in MissionDebugConsoleCommands._aiDebugTextItems.Split(new char[] { '\n' }))
			{
				if (text2.ToLower().Contains(text))
				{
					mbstringBuilder.AppendLine<string>(text2.Trim());
				}
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("show_chars", "ai")]
		public static string AIShowChars(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (strings.Count > 0 && strings[0] == "?")
			{
				return "Shows spnpccharacters.xml content\r\nfilter: filters items (e.g: filter:crazy)";
			}
			MissionDebugConsoleCommands.LoadChars();
			string text = MissionDebugConsoleCommands.AISpawnParameters.GetParameter(strings, "filter", "");
			if (text == "")
			{
				return MissionDebugConsoleCommands._aiDebugTextChars;
			}
			text = text.ToLower();
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(2048, "AIShowChars");
			foreach (string text2 in MissionDebugConsoleCommands._aiDebugTextChars.Split(new char[] { '\n' }))
			{
				if (text2.ToLower().Contains(text))
				{
					mbstringBuilder.AppendLine<string>(text2.Trim());
				}
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("driven_property", "ai")]
		public static string AIDrivenProperty(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (strings.Count == 1 && strings[0].Trim() == "")
			{
				MBStringBuilder mbstringBuilder = default(MBStringBuilder);
				mbstringBuilder.Initialize(16, "AIDrivenProperty");
				foreach (PropertyInfo propertyInfo in typeof(AgentDrivenProperties).GetProperties())
				{
					mbstringBuilder.AppendLine<string>(propertyInfo.Name);
				}
				return mbstringBuilder.ToStringAndRelease();
			}
			if (strings.Count > 2)
			{
				int num;
				int.TryParse(strings[0].Trim(), out num);
				string text = strings[1].Trim();
				float num2;
				float.TryParse(strings[2].Trim(), out num2);
				PropertyInfo[] array = typeof(AgentDrivenProperties).GetProperties();
				int i = 0;
				while (i < array.Length)
				{
					if (array[i].Name == text)
					{
						bool flag = false;
						DrivenProperty drivenProperty;
						if (Enum.TryParse<DrivenProperty>(text, out drivenProperty))
						{
							foreach (Agent agent in Mission.Current.Agents)
							{
								if (num == -1 || num == agent.Index)
								{
									agent.SetAgentDrivenPropertyValueFromConsole(drivenProperty, num2);
									flag = true;
									if (num > -1)
									{
										break;
									}
								}
							}
						}
						if (!flag)
						{
							return "Cannot find Agent with Index " + num;
						}
						return "Done!";
					}
					else
					{
						i++;
					}
				}
				return "Cannot find property " + text;
			}
			return "Usage: ai.driven_property agent_index property_name property_value";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("spawn", "ai")]
		public static string AISpawn(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (Mission.Current == null)
			{
				return "Mission is null";
			}
			if (strings.Count > 0 && strings[0] == "?")
			{
				return "AI Spawn Parameters\ntype: npc name [e.g: type:crazy_man]\nenemy: true if entered, false if keyword is absent\nnotAlarmed: true if entered, false if keyword is absent (agent is in alarmed state)\nposx: x spawn position\nposy: y spawn position\nposz: z spawn position\nweapon0: agent weapon\nweapon1: agent weapon\nweapon2: agent weapon\nweapon3: agent weapon\nlevel: agent level (between 1-30)\nhealth: agent health\ncount: agent spawn count";
			}
			if (Agent.Main == null && Mission.Current.DefenderTeam == null)
			{
				Mission.Current.Teams.Add(0, 4294901760U, 4294901760U, null, true, false, true);
				Mission.Current.Teams.Add(1, 4284776512U, 4284776512U, null, true, false, true);
			}
			MissionDebugConsoleCommands.AISpawnParameters aispawnParameters = new MissionDebugConsoleCommands.AISpawnParameters(strings);
			for (int i = 0; i < aispawnParameters.SpawnCount; i++)
			{
				if (aispawnParameters.animalType != "")
				{
					MatrixFrame lastFinalRenderCameraFrame = Mission.Current.Scene.LastFinalRenderCameraFrame;
					string text = aispawnParameters.animalType.ToLower();
					ItemRosterElement itemRosterElement;
					if (!(text == "sheep"))
					{
						if (!(text == "cow"))
						{
							if (!(text == "hog"))
							{
								if (!(text == "goose"))
								{
									return "Unknown monster type";
								}
								itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("goose"), 0, null);
							}
							else
							{
								itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("hog"), 0, null);
							}
						}
						else
						{
							itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("cow"), 0, null);
						}
					}
					else
					{
						itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("sheep"), 0, null);
					}
					Mission mission = Mission.Current;
					ItemRosterElement itemRosterElement2 = itemRosterElement;
					ItemRosterElement itemRosterElement3 = default(ItemRosterElement);
					Vec2 vec = lastFinalRenderCameraFrame.rotation.f.AsVec2;
					vec = vec.Normalized();
					mission.SpawnMonster(itemRosterElement2, itemRosterElement3, ref lastFinalRenderCameraFrame.origin, ref vec, -1);
				}
				else
				{
					Vec3 vec2 = aispawnParameters.position(i);
					MatrixFrame matrixFrame;
					matrixFrame..ctor(Mat3.Identity, vec2);
					CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(aispawnParameters.agentType);
					if (@object == null)
					{
						return "character not found";
					}
					if (aispawnParameters.HasWeapon(0))
					{
						@object.Equipment[0] = aispawnParameters.GetWeapon(0);
					}
					if (aispawnParameters.HasWeapon(1))
					{
						@object.Equipment[1] = aispawnParameters.GetWeapon(1);
					}
					if (aispawnParameters.HasWeapon(2))
					{
						@object.Equipment[2] = aispawnParameters.GetWeapon(2);
					}
					if (aispawnParameters.HasWeapon(3))
					{
						@object.Equipment[3] = aispawnParameters.GetWeapon(3);
					}
					Formation formation = aispawnParameters.team.GetFormation(@object.DefaultFormationClass);
					Mission mission2 = Mission.Current;
					AgentBuildData agentBuildData = new AgentBuildData(@object).Team(aispawnParameters.team).Formation(formation).InitialPosition(ref matrixFrame.origin);
					Vec2 vec = matrixFrame.rotation.f.AsVec2;
					vec = vec.Normalized();
					Agent agent = mission2.SpawnAgent(agentBuildData.InitialDirection(ref vec).TroopOrigin(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))), false);
					agent.Health = (float)aispawnParameters.Health;
					agent.SetWatchState(aispawnParameters.isAlarmed ? 2 : 0);
					agent.TeleportToPosition(vec2);
					formation.Team.MasterOrderController.SetOrder(4);
					formation.SetPositioning(new WorldPosition?(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, vec2, false)), null, null);
				}
			}
			return aispawnParameters.errorMessage;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("AddDebugTeleporter", "mission")]
		public static string AddDebugTeleporter(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (Mission.Current == null)
			{
				return "No mission";
			}
			if (Mission.Current.GetMissionBehavior<DebugAgentTeleporterMissionController>() == null)
			{
				Mission.Current.AddMissionBehavior(new DebugAgentTeleporterMissionController());
				return "Done";
			}
			return "Already added";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("AddObjectDestroyer", "mission")]
		public static string AddObjectDestroyer(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (Mission.Current == null)
			{
				return "No mission";
			}
			if (Mission.Current.GetMissionBehavior<DebugObjectDestroyerMissionController>() == null)
			{
				Mission.Current.AddMissionBehavior(new DebugObjectDestroyerMissionController());
				return "Done. Use middle mouse button to hit. While hitting use 'alt' key to hit as 'boulder'.";
			}
			return "Already added";
		}

		private static string _aiDebugTextItems;

		private static string _aiDebugTextChars;

		private class AISpawnParameters
		{
			internal Vec3 position(int index)
			{
				Vec3 zero = Vec3.Zero;
				if (!float.TryParse(this.posx, out zero.x))
				{
					zero.x = Mission.Current.Scene.LastFinalRenderCameraPosition.x + (float)index * MBRandom.RandomFloat;
				}
				if (!float.TryParse(this.posy, out zero.y))
				{
					zero.y = Mission.Current.Scene.LastFinalRenderCameraPosition.y + (float)index * MBRandom.RandomFloat;
				}
				if (!float.TryParse(this.posz, out zero.z))
				{
					zero.z = Mission.Current.Scene.LastFinalRenderCameraPosition.y + (float)index * MBRandom.RandomFloat;
				}
				return zero;
			}

			internal int Health
			{
				get
				{
					int num;
					if (int.TryParse(this.health, out num))
					{
						return num;
					}
					return 250;
				}
			}

			internal int Level
			{
				get
				{
					int num;
					if (int.TryParse(this.level, out num))
					{
						return num;
					}
					return 30;
				}
			}

			internal int SpawnCount
			{
				get
				{
					int num;
					if (int.TryParse(this.count, out num))
					{
						return num;
					}
					return 1;
				}
			}

			internal bool HasWeapon(int index)
			{
				switch (index)
				{
				case 0:
					return this.weapon0.Length > 0;
				case 1:
					return this.weapon1.Length > 0;
				case 2:
					return this.weapon2.Length > 0;
				case 3:
					return this.weapon3.Length > 0;
				default:
					return false;
				}
			}

			internal EquipmentElement GetWeapon(int index)
			{
				string text = "";
				switch (index)
				{
				case 0:
					text = this.weapon0;
					break;
				case 1:
					text = this.weapon1;
					break;
				case 2:
					text = this.weapon2;
					break;
				case 3:
					text = this.weapon3;
					break;
				}
				if (text.Length > 0)
				{
					return new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>(text), null, null, false);
				}
				return default(EquipmentElement);
			}

			internal AISpawnParameters(List<string> strings)
			{
				this.animalType = "";
				this.agentType = "crazy_man";
				this.weapon0 = "";
				this.weapon1 = "";
				this.weapon2 = "";
				this.weapon3 = "";
				this.posx = "";
				this.posy = "";
				this.posz = "";
				this.level = "";
				this.health = "";
				this.count = "";
				this.isAlarmed = true;
				foreach (string text in strings)
				{
					string text2 = text.Trim();
					if (text2.Length > 0)
					{
						string text3;
						string text4;
						if (text2.IndexOf(':') >= 0)
						{
							string[] array = text2.Split(new char[] { ':' });
							text3 = array[0];
							text4 = array[1];
						}
						else
						{
							text3 = text2;
							text4 = "";
						}
						this.ParseParameter(text3, text4);
					}
				}
				if (this.team == null)
				{
					if (Agent.Main == null)
					{
						this.team = (this.isEnemy ? Mission.Current.DefenderTeam : Mission.Current.AttackerTeam);
						return;
					}
					this.team = ((this.isEnemy != (Agent.Main.Team == Mission.Current.DefenderTeam)) ? Mission.Current.DefenderTeam : Mission.Current.AttackerTeam);
				}
			}

			private void ParseParameter(string key, string value)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
				if (num <= 2105282640U)
				{
					if (num <= 1361572173U)
					{
						if (num != 690885942U)
						{
							if (num != 967958004U)
							{
								if (num == 1361572173U)
								{
									if (key == "type")
									{
										this.agentType = value;
										goto IL_2A8;
									}
								}
							}
							else if (key == "count")
							{
								this.count = value;
								goto IL_2A8;
							}
						}
						else if (key == "notAlarmed")
						{
							this.isAlarmed = false;
							goto IL_2A8;
						}
					}
					else if (num <= 1805184399U)
					{
						if (num != 1776971179U)
						{
							if (num == 1805184399U)
							{
								if (key == "health")
								{
									this.health = value;
									goto IL_2A8;
								}
							}
						}
						else if (key == "enemy")
						{
							this.isEnemy = true;
							goto IL_2A8;
						}
					}
					else if (num != 2095572432U)
					{
						if (num == 2105282640U)
						{
							if (key == "weapon1")
							{
								this.weapon1 = value;
								goto IL_2A8;
							}
						}
					}
					else if (key == "posy")
					{
						this.posy = value;
						goto IL_2A8;
					}
				}
				else if (num <= 2138837878U)
				{
					if (num != 2112350051U)
					{
						if (num != 2122060259U)
						{
							if (num == 2138837878U)
							{
								if (key == "weapon3")
								{
									this.weapon3 = value;
									goto IL_2A8;
								}
							}
						}
						else if (key == "weapon0")
						{
							this.weapon0 = value;
							goto IL_2A8;
						}
					}
					else if (key == "posx")
					{
						this.posx = value;
						goto IL_2A8;
					}
				}
				else if (num <= 2155615497U)
				{
					if (num != 2145905289U)
					{
						if (num == 2155615497U)
						{
							if (key == "weapon2")
							{
								this.weapon2 = value;
								goto IL_2A8;
							}
						}
					}
					else if (key == "posz")
					{
						this.posz = value;
						goto IL_2A8;
					}
				}
				else if (num != 2610554845U)
				{
					if (num == 3258507357U)
					{
						if (key == "animal")
						{
							this.animalType = value;
							goto IL_2A8;
						}
					}
				}
				else if (key == "level")
				{
					this.level = value;
					goto IL_2A8;
				}
				this.errorMessage = this.errorMessage + "Unknown Parameter: " + key + "\r\n";
				IL_2A8:
				if (Agent.Main == null)
				{
					this.team = (this.isEnemy ? Mission.Current.DefenderTeam : Mission.Current.AttackerTeam);
					return;
				}
				this.team = ((this.isEnemy ^ (Agent.Main.Team == Mission.Current.DefenderTeam)) ? Mission.Current.DefenderTeam : Mission.Current.AttackerTeam);
			}

			internal static string GetParameter(List<string> strings, string key, string defaultValue)
			{
				key += ":";
				foreach (string text in strings)
				{
					string text2 = text.Trim();
					if (text2.StartsWith(key))
					{
						return text2.Remove(0, key.Length);
					}
				}
				return defaultValue;
			}

			internal static bool GetParameter(List<string> strings, string key, bool defaultValue)
			{
				using (List<string>.Enumerator enumerator = strings.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Trim() == key)
						{
							return true;
						}
					}
				}
				return defaultValue;
			}

			internal string animalType;

			internal string agentType;

			internal Team team;

			internal bool isEnemy;

			internal bool isAlarmed;

			private string weapon0;

			private string weapon1;

			private string weapon2;

			private string weapon3;

			private string posx;

			private string posy;

			private string posz;

			private string level;

			private string health;

			internal string errorMessage;

			private string count;
		}
	}
}
