using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.CampaignBehaviors
{
	public class ConversationAnimationToolCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void Tick(float dt)
		{
			if (ConversationAnimationToolCampaignBehavior._isToolEnabled)
			{
				ConversationAnimationToolCampaignBehavior.StartImGUIWindow("Conversation Animation Test Tool");
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Character Type:", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("0 for noble, 1 for notable, 2 for companion, 3 for troop", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUIIntegerField("Enter character type: ", ref ConversationAnimationToolCampaignBehavior._characterType, false, false);
				ConversationAnimationToolCampaignBehavior.Separator();
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Character State:", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("0 for active, 1 for prisoner", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUIIntegerField("Enter character state: ", ref ConversationAnimationToolCampaignBehavior._characterState, false, false);
				ConversationAnimationToolCampaignBehavior.Separator();
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Character Gender:", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("0 for male, 1 for female", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUIIntegerField("Enter character gender: ", ref ConversationAnimationToolCampaignBehavior._characterGender, false, false);
				ConversationAnimationToolCampaignBehavior.Separator();
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Character Age:", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Enter a custom age or leave -1 for not changing the age value", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUIIntegerField("Enter character age: ", ref ConversationAnimationToolCampaignBehavior._characterAge, false, false);
				ConversationAnimationToolCampaignBehavior.Separator();
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Character Wounded State:", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Change to 1 to change character state to wounded", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUIIntegerField("Enter character wounded state: ", ref ConversationAnimationToolCampaignBehavior._characterWoundedState, false, false);
				ConversationAnimationToolCampaignBehavior.Separator();
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Character Equipment Type:", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Change to 1 to change to equipment to civilian, default equipment is battle", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUIIntegerField("Enter equipment type: ", ref ConversationAnimationToolCampaignBehavior._equipmentType, false, false);
				ConversationAnimationToolCampaignBehavior.Separator();
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Character Relation With Main Hero:", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Leave -1 for no change, 0 for enemy, 1 for neutral, 2 for friend", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUIIntegerField("Enter relation type: ", ref ConversationAnimationToolCampaignBehavior._relationType, false, false);
				ConversationAnimationToolCampaignBehavior.Separator();
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Character Persona Type:", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUITextArea("Leave -1 for no change, 0 for curt, 1 for earnest, 2 for ironic, 3 for softspoken", false, false);
				ConversationAnimationToolCampaignBehavior.ImGUIIntegerField("Enter persona type: ", ref ConversationAnimationToolCampaignBehavior._personaType, false, false);
				ConversationAnimationToolCampaignBehavior.Separator();
				if (ConversationAnimationToolCampaignBehavior.ImGUIButton(" Start Conversation ", true))
				{
					ConversationAnimationToolCampaignBehavior.StartConversation();
				}
				ConversationAnimationToolCampaignBehavior.EndImGUIWindow();
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("enable_conversation_animation_test_tool", "campaign")]
		public static string EnableConversationAnimationTool(List<string> strings)
		{
			if (strings.Count != 1)
			{
				return "Usage: enable_conversation_animation_test_tool [1/0]";
			}
			if (!(Game.Current.GameStateManager.ActiveState is MapState))
			{
				return "Active state must be map state to use the tool.";
			}
			int num;
			if (!int.TryParse(strings[0], out num))
			{
				return "Enter a number.";
			}
			if (ConversationAnimationToolCampaignBehavior._isToolEnabled && num == 1)
			{
				return "'Conversation Animation Test Tool' is already enabled!";
			}
			ConversationAnimationToolCampaignBehavior._isToolEnabled = num == 1;
			if (!ConversationAnimationToolCampaignBehavior._isToolEnabled)
			{
				ConversationAnimationToolCampaignBehavior.CloseConversationAnimationTool();
			}
			return "'Conversation Animation Test Tool' is " + ((num == 1) ? "enabled." : "disabled.\nCampaign time must be passed to keep the tool open.");
		}

		private static void CloseConversationAnimationTool()
		{
			ConversationAnimationToolCampaignBehavior._isToolEnabled = false;
			ConversationAnimationToolCampaignBehavior._characterType = -1;
			ConversationAnimationToolCampaignBehavior._characterState = -1;
			ConversationAnimationToolCampaignBehavior._characterGender = -1;
			ConversationAnimationToolCampaignBehavior._characterAge = -1;
			ConversationAnimationToolCampaignBehavior._characterWoundedState = -1;
			ConversationAnimationToolCampaignBehavior._equipmentType = -1;
			ConversationAnimationToolCampaignBehavior._relationType = -1;
			ConversationAnimationToolCampaignBehavior._personaType = -1;
		}

		private static void StartConversation()
		{
			bool flag = true;
			bool flag2 = true;
			Occupation occupation = 0;
			switch (ConversationAnimationToolCampaignBehavior._characterType)
			{
			case 0:
				occupation = 3;
				break;
			case 1:
				occupation = 18;
				break;
			case 2:
				occupation = 16;
				break;
			case 3:
				occupation = 7;
				flag2 = false;
				break;
			default:
				flag = false;
				break;
			}
			if (!flag)
			{
				return;
			}
			bool flag3 = false;
			bool flag4 = false;
			if (ConversationAnimationToolCampaignBehavior._characterState == 0)
			{
				flag3 = true;
			}
			else if (ConversationAnimationToolCampaignBehavior._characterState == 1)
			{
				flag4 = true;
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				return;
			}
			bool flag5 = false;
			if (ConversationAnimationToolCampaignBehavior._characterGender == 1)
			{
				flag5 = true;
			}
			else if (ConversationAnimationToolCampaignBehavior._characterGender == 0)
			{
				flag5 = false;
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				return;
			}
			bool flag6 = false;
			if (ConversationAnimationToolCampaignBehavior._characterAge == -1)
			{
				flag6 = false;
			}
			else if (ConversationAnimationToolCampaignBehavior._characterAge > 0 && ConversationAnimationToolCampaignBehavior._characterAge <= 128)
			{
				flag6 = true;
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				return;
			}
			bool flag7 = ConversationAnimationToolCampaignBehavior._characterWoundedState == 1;
			bool flag8 = ConversationAnimationToolCampaignBehavior._equipmentType == 1;
			if (ConversationAnimationToolCampaignBehavior._relationType != 0 && ConversationAnimationToolCampaignBehavior._relationType != 1 && ConversationAnimationToolCampaignBehavior._relationType != 2)
			{
				return;
			}
			CharacterObject characterObject = null;
			if (flag2)
			{
				Hero hero = null;
				foreach (Hero hero2 in Hero.AllAliveHeroes)
				{
					if (hero2 != Hero.MainHero && hero2.Occupation == occupation && hero2.IsFemale == flag5 && (hero2.PartyBelongedTo == null || hero2.PartyBelongedTo.MapEvent == null))
					{
						hero = hero2;
						break;
					}
				}
				if (hero == null)
				{
					hero = HeroCreator.CreateHeroAtOccupation(occupation, null);
				}
				if (flag6)
				{
					hero.SetBirthDay(HeroHelper.GetRandomBirthDayForAge((float)ConversationAnimationToolCampaignBehavior._characterAge));
				}
				if (flag4)
				{
					TakePrisonerAction.Apply(PartyBase.MainParty, hero);
				}
				if (flag7)
				{
					hero.MakeWounded(null, 0);
				}
				if (flag3)
				{
					hero.ChangeState(1);
				}
				hero.UpdatePlayerGender(flag5);
				characterObject = hero.CharacterObject;
			}
			else
			{
				foreach (CharacterObject characterObject2 in CharacterObject.All)
				{
					if (characterObject2.Occupation == occupation && characterObject2.IsFemale == flag5)
					{
						characterObject = characterObject2;
						break;
					}
				}
				if (characterObject == null)
				{
					characterObject = Campaign.Current.ObjectManager.GetObject<CultureObject>("empire").BasicTroop;
				}
			}
			if (characterObject == null)
			{
				return;
			}
			if (characterObject.IsHero && ConversationAnimationToolCampaignBehavior._relationType != -1)
			{
				Hero heroObject = characterObject.HeroObject;
				float relationWithPlayer = heroObject.GetRelationWithPlayer();
				float num = 0f;
				if (ConversationAnimationToolCampaignBehavior._relationType == 0 && !heroObject.IsEnemy(Hero.MainHero))
				{
					num = -relationWithPlayer - 15f;
				}
				else if (ConversationAnimationToolCampaignBehavior._relationType == 1 && !heroObject.IsNeutral(Hero.MainHero))
				{
					num = -relationWithPlayer;
				}
				else if (ConversationAnimationToolCampaignBehavior._relationType == 2 && !heroObject.IsFriend(Hero.MainHero))
				{
					num = -relationWithPlayer + 15f;
				}
				ChangeRelationAction.ApplyPlayerRelation(heroObject, (int)num, true, true);
			}
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, false, false, false, false, false, false), new ConversationCharacterData(characterObject, null, false, false, false, flag8, flag8, false));
			ConversationAnimationToolCampaignBehavior.CloseConversationAnimationTool();
		}

		private static void StartImGUIWindow(string str)
		{
			Imgui.BeginMainThreadScope();
			Imgui.Begin(str);
		}

		private static void ImGUITextArea(string text, bool separatorNeeded, bool onSameLine)
		{
			Imgui.Text(text);
			ConversationAnimationToolCampaignBehavior.ImGUISeparatorSameLineHandler(separatorNeeded, onSameLine);
		}

		private static bool ImGUIButton(string buttonText, bool smallButton)
		{
			if (smallButton)
			{
				return Imgui.SmallButton(buttonText);
			}
			return Imgui.Button(buttonText);
		}

		private static void ImGUIIntegerField(string fieldText, ref int value, bool separatorNeeded, bool onSameLine)
		{
			Imgui.InputInt(fieldText, ref value);
			ConversationAnimationToolCampaignBehavior.ImGUISeparatorSameLineHandler(separatorNeeded, onSameLine);
		}

		private static void ImGUICheckBox(string text, ref bool is_checked, bool separatorNeeded, bool onSameLine)
		{
			Imgui.Checkbox(text, ref is_checked);
			ConversationAnimationToolCampaignBehavior.ImGUISeparatorSameLineHandler(separatorNeeded, onSameLine);
		}

		private static void ImGUISeparatorSameLineHandler(bool separatorNeeded, bool onSameLine)
		{
			if (separatorNeeded)
			{
				ConversationAnimationToolCampaignBehavior.Separator();
			}
			if (onSameLine)
			{
				ConversationAnimationToolCampaignBehavior.OnSameLine();
			}
		}

		private static void OnSameLine()
		{
			Imgui.SameLine(0f, 0f);
		}

		private static void Separator()
		{
			Imgui.Separator();
		}

		private static void EndImGUIWindow()
		{
			Imgui.End();
			Imgui.EndMainThreadScope();
		}

		private static bool _isToolEnabled = false;

		private static int _characterType = -1;

		private static int _characterState = -1;

		private static int _characterGender = -1;

		private static int _characterAge = -1;

		private static int _characterWoundedState = -1;

		private static int _equipmentType = -1;

		private static int _relationType = -1;

		private static int _personaType = -1;
	}
}
