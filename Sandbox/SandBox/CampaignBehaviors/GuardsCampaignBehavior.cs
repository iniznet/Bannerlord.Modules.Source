using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Conversation;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SandBox.CampaignBehaviors
{
	public class GuardsCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		private float GetProsperityMultiplier(SettlementComponent settlement)
		{
			return (settlement.GetProsperityLevel() + 1f) / 3f;
		}

		private void AddGarrisonAndPrisonCharacters(Settlement settlement)
		{
			this.InitializeGarrisonCharacters(settlement);
			Location locationWithId = settlement.LocationComplex.GetLocationWithId("center");
			CultureObject cultureObject = ((Campaign.Current.GameMode == 1) ? settlement.MapFaction.Culture : settlement.Culture);
			locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreatePrisonGuard), cultureObject, 0, 1);
		}

		private void InitializeGarrisonCharacters(Settlement settlement)
		{
			this._garrisonTroops.Clear();
			if (Campaign.Current.GameMode == 1)
			{
				MobileParty garrisonParty = settlement.Town.GarrisonParty;
				if (garrisonParty != null)
				{
					foreach (TroopRosterElement troopRosterElement in garrisonParty.MemberRoster.GetTroopRoster())
					{
						if (troopRosterElement.Character.Occupation == 7)
						{
							this._garrisonTroops.Add(new ValueTuple<CharacterObject, int>(troopRosterElement.Character, troopRosterElement.Number - troopRosterElement.WoundedNumber));
						}
					}
				}
			}
		}

		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			if (settlement.IsFortification)
			{
				this.AddGarrisonAndPrisonCharacters(settlement);
				if ((settlement.IsTown || settlement.IsCastle) && CampaignMission.Current != null)
				{
					Location location = CampaignMission.Current.Location;
					this.AddGuardsFromGarrison(settlement, unusedUsablePointCount, location);
				}
			}
		}

		private void AddGuardsFromGarrison(Settlement settlement, Dictionary<string, int> unusedUsablePointCount, Location location)
		{
			int num;
			unusedUsablePointCount.TryGetValue("sp_guard", out num);
			int num2;
			unusedUsablePointCount.TryGetValue("sp_guard_with_spear", out num2);
			int num3;
			unusedUsablePointCount.TryGetValue("sp_guard_patrol", out num3);
			int num4;
			unusedUsablePointCount.TryGetValue("sp_guard_unarmed", out num4);
			int num5;
			unusedUsablePointCount.TryGetValue("sp_guard_castle", out num5);
			float prosperityMultiplier = this.GetProsperityMultiplier(settlement.SettlementComponent);
			float num6 = (settlement.IsCastle ? 1.6f : 0.4f);
			num = (int)((float)num * prosperityMultiplier);
			num2 = (int)((float)num2 * prosperityMultiplier);
			num3 = (int)((float)num3 * prosperityMultiplier);
			num4 = (int)((float)num4 * prosperityMultiplier * num6);
			if (num5 > 0)
			{
				location.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateCastleGuard), settlement.Culture, 0, num5);
			}
			if (num > 0)
			{
				location.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateStandGuard), settlement.Culture, 0, num);
			}
			if (num2 > 0)
			{
				location.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateStandGuardWithSpear), settlement.Culture, 0, num2);
			}
			if (num3 > 0)
			{
				location.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreatePatrollingGuard), settlement.Culture, 0, num3);
			}
			if (num4 > 0 && location != settlement.LocationComplex.GetLocationWithId("lordshall"))
			{
				location.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateUnarmedGuard), settlement.Culture, 0, num4);
			}
			if (location.StringId == "prison")
			{
				int num7;
				if (unusedUsablePointCount.TryGetValue("area_marker_1", out num7) && num7 > 0)
				{
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateStandGuard), settlement.Culture, 0, 1);
				}
				if (unusedUsablePointCount.TryGetValue("area_marker_2", out num7) && num7 > 0)
				{
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateStandGuard), settlement.Culture, 0, 1);
				}
				if (unusedUsablePointCount.TryGetValue("area_marker_3", out num7) && num7 > 0)
				{
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateStandGuard), settlement.Culture, 0, 1);
				}
			}
		}

		private static ItemObject GetSuitableSpear(CultureObject culture)
		{
			string text = ((culture.StringId == "battania") ? "northern_spear_2_t3" : "western_spear_3_t3");
			return MBObjectManager.Instance.GetObject<ItemObject>(text);
		}

		private AgentData TakeGuardAgentDataFromGarrisonTroopList(CultureObject culture, bool overrideWeaponWithSpear = false, bool unarmed = false)
		{
			CharacterObject characterObject;
			if (this._garrisonTroops.Count > 0)
			{
				List<ValueTuple<ValueTuple<CharacterObject, int>, float>> list = new List<ValueTuple<ValueTuple<CharacterObject, int>, float>>();
				foreach (ValueTuple<CharacterObject, int> valueTuple in this._garrisonTroops)
				{
					list.Add(new ValueTuple<ValueTuple<CharacterObject, int>, float>(new ValueTuple<CharacterObject, int>(valueTuple.Item1, valueTuple.Item2), (float)valueTuple.Item1.Level));
				}
				int num;
				ValueTuple<CharacterObject, int> valueTuple2 = MBRandom.ChooseWeighted<ValueTuple<CharacterObject, int>>(list, ref num);
				characterObject = valueTuple2.Item1;
				if (valueTuple2.Item2 <= 1)
				{
					this._garrisonTroops.RemoveAt(num);
				}
				else
				{
					this._garrisonTroops[num] = new ValueTuple<CharacterObject, int>(valueTuple2.Item1, valueTuple2.Item2 - 1);
				}
			}
			else
			{
				characterObject = culture.Guard;
			}
			return GuardsCampaignBehavior.PrepareGuardAgentDataFromGarrison(characterObject, overrideWeaponWithSpear, unarmed);
		}

		public static AgentData PrepareGuardAgentDataFromGarrison(CharacterObject guardRosterElement, bool overrideWeaponWithSpear = false, bool unarmed = false)
		{
			Banner banner = ((Campaign.Current.GameMode == 1) ? PlayerEncounter.LocationEncounter.Settlement.OwnerClan.Banner : null);
			Equipment randomEquipmentElements = Equipment.GetRandomEquipmentElements(guardRosterElement, false, false, -1);
			Dictionary<ItemObject.ItemTypeEnum, int> dictionary = new Dictionary<ItemObject.ItemTypeEnum, int>
			{
				{ 4, 0 },
				{ 9, 0 },
				{ 6, 0 },
				{ 8, 0 },
				{ 5, 0 },
				{ 10, 0 },
				{ 7, 0 }
			};
			int num = 0;
			for (int i = 0; i <= 4; i++)
			{
				if (randomEquipmentElements[i].Item != null)
				{
					if (dictionary.ContainsKey(randomEquipmentElements[i].Item.ItemType))
					{
						Dictionary<ItemObject.ItemTypeEnum, int> dictionary2 = dictionary;
						ItemObject.ItemTypeEnum itemTypeEnum = randomEquipmentElements[i].Item.ItemType;
						int num2 = dictionary2[itemTypeEnum];
						dictionary2[itemTypeEnum] = num2 + 1;
					}
					else
					{
						num++;
					}
				}
			}
			if (overrideWeaponWithSpear && dictionary[4] > 0)
			{
				Dictionary<ItemObject.ItemTypeEnum, int> dictionary3 = dictionary;
				int num2 = dictionary3[4];
				dictionary3[4] = num2 - 1;
			}
			if (num > 0)
			{
				num--;
			}
			else if (dictionary[4] > 0)
			{
				Dictionary<ItemObject.ItemTypeEnum, int> dictionary4 = dictionary;
				int num2 = dictionary4[4];
				dictionary4[4] = num2 - 1;
			}
			else if (dictionary[8] > 0)
			{
				Dictionary<ItemObject.ItemTypeEnum, int> dictionary5 = dictionary;
				int num2 = dictionary5[5];
				dictionary5[5] = num2 - 1;
				Dictionary<ItemObject.ItemTypeEnum, int> dictionary6 = dictionary;
				num2 = dictionary6[8];
				dictionary6[8] = num2 - 1;
			}
			else if (dictionary[9] > 0)
			{
				Dictionary<ItemObject.ItemTypeEnum, int> dictionary7 = dictionary;
				int num2 = dictionary7[9];
				dictionary7[9] = num2 - 1;
				Dictionary<ItemObject.ItemTypeEnum, int> dictionary8 = dictionary;
				num2 = dictionary8[6];
				dictionary8[6] = num2 - 1;
			}
			for (int j = 4; j >= 0; j--)
			{
				if (randomEquipmentElements[j].Item != null)
				{
					bool flag = false;
					int num3;
					if (dictionary.TryGetValue(randomEquipmentElements[j].Item.ItemType, out num3))
					{
						if (num3 > 0)
						{
							flag = true;
							Dictionary<ItemObject.ItemTypeEnum, int> dictionary9 = dictionary;
							ItemObject.ItemTypeEnum itemTypeEnum = randomEquipmentElements[j].Item.ItemType;
							int num2 = dictionary9[itemTypeEnum];
							dictionary9[itemTypeEnum] = num2 - 1;
						}
					}
					else if (num > 0)
					{
						flag = true;
						num--;
					}
					if (flag)
					{
						randomEquipmentElements.AddEquipmentToSlotWithoutAgent(j, default(EquipmentElement));
					}
				}
			}
			if (overrideWeaponWithSpear)
			{
				if (!GuardsCampaignBehavior.IfEquipmentHasSpearSwapSlots(randomEquipmentElements))
				{
					ItemObject suitableSpear = GuardsCampaignBehavior.GetSuitableSpear(guardRosterElement.Culture);
					randomEquipmentElements.AddEquipmentToSlotWithoutAgent(3, new EquipmentElement(suitableSpear, null, null, false));
					GuardsCampaignBehavior.IfEquipmentHasSpearSwapSlots(randomEquipmentElements);
				}
			}
			else if (unarmed)
			{
				randomEquipmentElements.AddEquipmentToSlotWithoutAgent(0, default(EquipmentElement));
				randomEquipmentElements.AddEquipmentToSlotWithoutAgent(1, default(EquipmentElement));
				randomEquipmentElements.AddEquipmentToSlotWithoutAgent(2, default(EquipmentElement));
				randomEquipmentElements.AddEquipmentToSlotWithoutAgent(3, default(EquipmentElement));
				randomEquipmentElements.AddEquipmentToSlotWithoutAgent(4, default(EquipmentElement));
				randomEquipmentElements.AddEquipmentToSlotWithoutAgent(5, default(EquipmentElement));
				randomEquipmentElements.AddEquipmentToSlotWithoutAgent(8, default(EquipmentElement));
			}
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(guardRosterElement.Race, "_settlement");
			return new AgentData(new SimpleAgentOrigin(guardRosterElement, -1, banner, default(UniqueTroopDescriptor))).Equipment(randomEquipmentElements).Monster(monsterWithSuffix).NoHorses(true);
		}

		private static bool IfEquipmentHasSpearSwapSlots(Equipment equipment)
		{
			for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
			{
				ItemObject item = equipment[equipmentIndex].Item;
				if (item != null && item.WeaponComponent.PrimaryWeapon.IsPolearm)
				{
					Equipment.SwapWeapons(equipment, equipmentIndex, 0);
					return true;
				}
			}
			return false;
		}

		private void RemoveShields(Equipment equipment)
		{
			for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
			{
				ItemObject item = equipment[equipmentIndex].Item;
				if (item != null && item.WeaponComponent.PrimaryWeapon.IsShield)
				{
					equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, default(EquipmentElement));
				}
			}
		}

		private LocationCharacter CreateCastleGuard(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			AgentData agentData = this.TakeGuardAgentDataFromGarrisonTroopList(culture, true, false);
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddStandGuardBehaviors), "sp_guard_castle", true, 0, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_guard"), false, false, null, false, false, true);
		}

		private LocationCharacter CreateStandGuard(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			AgentData agentData = this.TakeGuardAgentDataFromGarrisonTroopList(culture, false, false);
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddStandGuardBehaviors), "sp_guard", true, 0, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_guard"), false, false, null, false, false, true);
		}

		private LocationCharacter CreateStandGuardWithSpear(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			AgentData agentData = this.TakeGuardAgentDataFromGarrisonTroopList(culture, true, false);
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddStandGuardBehaviors), "sp_guard_with_spear", true, 0, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_guard"), false, false, null, false, false, true);
		}

		private LocationCharacter CreateUnarmedGuard(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			AgentData agentData = this.TakeGuardAgentDataFromGarrisonTroopList(culture, false, true);
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "sp_guard_unarmed", true, 0, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_unarmed_guard"), false, false, null, false, false, true);
		}

		private LocationCharacter CreatePatrollingGuard(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			AgentData agentData = this.TakeGuardAgentDataFromGarrisonTroopList(culture, false, false);
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddPatrollingGuardBehaviors), "sp_guard_patrol", true, 0, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_guard"), false, false, null, false, false, true);
		}

		private LocationCharacter CreatePrisonGuard(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject prisonGuard = culture.PrisonGuard;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(prisonGuard.Race, "_settlement");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(prisonGuard, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix);
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddStandGuardBehaviors), "sp_prison_guard", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_guard"), false, true, null, false, false, true);
		}

		protected void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("disguise_start_conversation_alt", "start", "close_window", "{=uTycGRdI}You need to move along. I'm on duty right now and I can't spare any coin. May Heaven provide.", new ConversationSentence.OnConditionDelegate(this.conversation_disguised_start_on_condition_alt), null, 100, null);
			campaignGameStarter.AddDialogLine("disguise_start_conversation", "start", "close_window", "{=P98iCLjl}Get out of my face, you vile beggar.[if:convo_angry]", new ConversationSentence.OnConditionDelegate(this.conversation_disguised_start_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("prison_guard_start_criminal", "start", "prison_guard_talk_criminal", "{=0UUCTaEj}We hear a lot of complaints about you lately. You better start behaving or you'll get yourself a good flogging.[if:convo_mocking_revenge]", new ConversationSentence.OnConditionDelegate(this.conversation_prison_guard_criminal_start_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("prison_guard_ask_criminal", "prison_guard_talk_criminal", "prison_guard_talk", "{=XqTa0iQZ}What do you want, you degenerate?[if:convo_stern]", null, null, 100, null);
			campaignGameStarter.AddDialogLine("prison_guard_start", "start", "prison_guard_talk", "{=6SppoTum}Yes? What do you want?", new ConversationSentence.OnConditionDelegate(this.conversation_prison_guard_start_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("prison_guard_ask_prisoners", "prison_guard_talk", "prison_guard_ask_prisoners", "{=av0bRae8}Who is imprisoned here?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("prison_guard_ask_prisoner_talk", "prison_guard_talk", "close_window", "{=QxIXbHai}I want to speak with a prisoner (Cheat).", new ConversationSentence.OnConditionDelegate(this.conversation_prison_guard_visit_prison_cheat_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_prison_guard_visit_prison_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("prison_guard_ask_prisoner_talk_2", "prison_guard_talk", "prison_guard_visit_prison", "{=EGI6ztlH}I want to speak with a prisoner.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("prison_guard_talk_end", "prison_guard_talk", "close_window", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("prison_guard_talk_about_prisoners", "prison_guard_ask_prisoners", "prison_guard_talk", "{=2eydhtcz}Currently, {PRISONER_NAMES} {?IS_PLURAL}are{?}is{\\?} imprisoned here.", new ConversationSentence.OnConditionDelegate(this.conversation_prison_guard_talk_about_prisoners_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("prison_guard_visit_prison_ask_for_permission", "prison_guard_visit_prison", "prison_guard_visit_prison_ask_for_permission_answer", "{=XN0XZAkI}I can't let you in. My {?SETTLEMENT_OWNER.GENDER}Lady{?}Lord{\\?} {SETTLEMENT_OWNER.NAME} would be furious.", new ConversationSentence.OnConditionDelegate(this.conversation_prison_guard_reject_visit_prison_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("prison_guard_visit_prison", "prison_guard_visit_prison", "close_window", "{=XWpEpaQ4}Of course, {?PLAYER.GENDER}madam{?}sir{\\?}. Go in.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_prison_guard_visit_prison_on_consequence), 100, null);
			campaignGameStarter.AddPlayerLine("prison_guard_visit_prison_ask_answer", "prison_guard_visit_prison_ask_for_permission_answer", "prison_guard_visit_prison_ask_for_permission_guard_answer", "{=k3b5KqSc}Come on now. I thought you were the boss here.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("prison_guard_visit_prison_ask_answer_3", "prison_guard_visit_prison_ask_for_permission_guard_answer", "prison_guard_visit_prison_ask_for_permission_answer_options", "{=JaAltoKP}Um... What are you saying?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("prison_guard_visit_permission_try_bribe", "prison_guard_visit_prison_ask_for_permission_answer_options", "prison_guard_bribe_answer_satisfied", "{=dY3Vazug}I found a purse with {AMOUNT}{GOLD_ICON} a few paces away. I reckon it belongs to you.", new ConversationSentence.OnConditionDelegate(this.prison_guard_visit_permission_try_bribe_on_condition), null, 100, new ConversationSentence.OnClickableConditionDelegate(this.can_player_bribe_to_prison_guard_clickable), null);
			campaignGameStarter.AddPlayerLine("prison_guard_visit_prison_ask_answer_3_2", "prison_guard_visit_prison_ask_for_permission_answer_options", "close_window", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("prison_guard_visit_prison_nobody_inside", "prison_guard_visit_prison", "prison_guard_talk", "{=rVHbbrCQ}We're not holding anyone in here right now. There's no reason for you to go in.[ib:closed]", new ConversationSentence.OnConditionDelegate(this.conversation_prison_guard_visit_prison_nobody_inside_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("prison_guard_visit_empty_prison", "prison_guard_visit_prison_nobody_1", "close_window", "{=b3KFoJJ8}All right then. I'll have a look at the prison.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_prison_guard_visit_prison_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("prison_guard_visit_empty_prison_2", "prison_guard_visit_prison_nobody_2", "close_window", "{=b3KFoJJ8}All right then. I'll have a look at the prison.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("prison_guard_not_visit_empty_prison", "prison_guard_visit_prison_nobody_1", "close_window", "{=L5vAhxhO}I have more important business to do.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("prison_guard_not_visit_empty_prison_2", "prison_guard_visit_prison_nobody_2", "close_window", "{=L5vAhxhO}I have more important business to do.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("prison_guard_visit_permission_leave", "prison_guard_visit_prison_2", "close_window", "{=qPRl07mD}All right then. I'll try that.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("prison_guard_visit_permission_bribe", "prison_guard_bribe_answer_satisfied", "close_window", "{=fCrVeHP3}Ah! I was looking for this all day. How good of you to bring it back {?PLAYER.GENDER}madam{?}sir{\\?}. Well, now that I know what an honest {?PLAYER.GENDER}lady{?}man{\\?} you are, there can be no harm in letting you inside for a look. Go in.... Just so you know, though -- I'll be hanging onto the keys, in case you were thinking about undoing anyone's chains.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_prison_guard_visit_permission_bribe_on_consequence), 100, null);
			campaignGameStarter.AddPlayerLine("prison_guard_visit_permission_try_break", "prison_guard_visit_prison_4", "prison_guard_visit_break", "{=htfLEQlf}Give me the keys to the cells -- now!", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("prison_guard_visit_break", "prison_guard_visit_break", "close_window", "{=Kto7RWKE}Help! Help! Prison break!", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_prison_guard_visit_break_on_consequence), 100, null);
			campaignGameStarter.AddDialogLine("castle_guard_start_criminal", "start", "castle_guard_talk_criminal", "{=0UUCTaEj}We hear a lot of complaints about you lately. You better start behaving or you'll get yourself a good flogging.[if:convo_mocking_revenge]", new ConversationSentence.OnConditionDelegate(this.conversation_castle_guard_criminal_start_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("castle_guard_ask_criminal", "castle_guard_talk_criminal", "castle_guard_talk", "{=XqTa0iQZ}What do you want, you degenerate?[if:convo_stern]", null, null, 100, null);
			campaignGameStarter.AddDialogLine("castle_guard_start", "start", "castle_guard_talk", "{=6SppoTum}Yes? What do you want?", new ConversationSentence.OnConditionDelegate(this.conversation_castle_guard_start_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("guard_start", "start", "close_window", "{=!}{GUARD_COMMENT}", new ConversationSentence.OnConditionDelegate(this.conversation_guard_start_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("player_ask_for_permission_to_enter_lords_hall", "castle_guard_talk", "player_ask_permission_to_lords_hall", "{=b2h3r1kL}I want to visit the lord's hall.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("player_ask_for_permission_to_enter_lords_hall_2", "castle_guard_talk", "close_window", "{=never_mind}Never mind.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("castle_guard_no_permission_nobody_inside", "player_ask_permission_to_lords_hall", "permisson_for_lords_hall", "{=RJtCakaG}There is nobody inside to receive you right now.", new ConversationSentence.OnConditionDelegate(this.conversation_castle_guard_nobody_inside_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("castle_guard_player_can_enter", "player_ask_permission_to_lords_hall", "close_window", "{=bbroVUrD}Of course, my {?PLAYER.GENDER}lady{?}lord{\\?}.", new ConversationSentence.OnConditionDelegate(this.conversation_castle_guard_player_can_enter_lordshall_condition), delegate
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += this.OpenLordsHallMission;
			}, 100, null);
			campaignGameStarter.AddDialogLine("castle_guard_no_permission", "player_ask_permission_to_lords_hall", "permisson_for_lords_hall", "{=rcoESVVz}Sorry, but we don't know you. We can't just let anyone in. (Not enough renown)", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("player_bribe_to_enter_lords_hall", "permisson_for_lords_hall", "player_bribe_to_castle_guard", "{=7wkHMnNM}Maybe {AMOUNT}{GOLD_ICON} will help you to remember me.", new ConversationSentence.OnConditionDelegate(this.conversation_player_bribe_to_enter_lords_hall_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_bribe_to_enter_lords_hall_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("player_not_bribe_to_enter_lords_hall", "permisson_for_lords_hall", "close_window", "{=xatWDriV}Never mind then.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("castle_guard_let_player_in", "player_bribe_to_castle_guard", "close_window", "{=g5ofoKa8}Yeah... Now I remember you.", null, delegate
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += this.OpenLordsHallMission;
			}, 100, null);
		}

		private bool conversation_prison_guard_criminal_start_on_condition()
		{
			return !Campaign.Current.IsMainHeroDisguised && CharacterObject.OneToOneConversationCharacter.Occupation == 23 && Settlement.CurrentSettlement.MapFaction != Hero.MainHero.MapFaction && (Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingModerate(Settlement.CurrentSettlement.MapFaction) || Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(Settlement.CurrentSettlement.MapFaction) || Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingMild(Settlement.CurrentSettlement.MapFaction));
		}

		private bool conversation_prison_guard_start_on_condition()
		{
			return !Campaign.Current.IsMainHeroDisguised && CharacterObject.OneToOneConversationCharacter.Occupation == 23 && (Settlement.CurrentSettlement.MapFaction == Hero.MainHero.MapFaction || (Settlement.CurrentSettlement.MapFaction != Hero.MainHero.MapFaction && !Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingModerate(Settlement.CurrentSettlement.MapFaction) && !Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(Settlement.CurrentSettlement.MapFaction) && !Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingMild(Settlement.CurrentSettlement.MapFaction)));
		}

		private bool conversation_prison_guard_talk_about_prisoners_on_condition()
		{
			List<CharacterObject> prisonerHeroes = Settlement.CurrentSettlement.SettlementComponent.GetPrisonerHeroes();
			if (prisonerHeroes.Count == 0)
			{
				MBTextManager.SetTextVariable("PRISONER_NAMES", GameTexts.FindText("str_nobody", null), false);
				MBTextManager.SetTextVariable("IS_PLURAL", "0", false);
			}
			else
			{
				for (int i = 0; i < prisonerHeroes.Count; i++)
				{
					if (i == 0)
					{
						MBTextManager.SetTextVariable("LEFT", prisonerHeroes[i].Name, false);
					}
					else
					{
						MBTextManager.SetTextVariable("RIGHT", prisonerHeroes[i].Name, false);
						MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString(), false);
					}
				}
				MBTextManager.SetTextVariable("IS_PLURAL", (prisonerHeroes.Count > 1) ? 1 : 0);
				MBTextManager.SetTextVariable("PRISONER_NAMES", GameTexts.FindText("str_LEFT_ONLY", null).ToString(), false);
			}
			return true;
		}

		private bool conversation_prison_guard_visit_prison_cheat_on_condition()
		{
			return Game.Current.IsDevelopmentMode;
		}

		private bool can_player_bribe_to_prison_guard_clickable(out TextObject explanation)
		{
			explanation = TextObject.Empty;
			int bribeToEnterDungeon = Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterDungeon(Settlement.CurrentSettlement);
			if (Hero.MainHero.Gold < bribeToEnterDungeon)
			{
				explanation = new TextObject("{=TP7rZTKs}You don't have {DENAR_AMOUNT}{GOLD_ICON} denars.", null);
				explanation.SetTextVariable("DENAR_AMOUNT", bribeToEnterDungeon);
				explanation.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
				return false;
			}
			explanation = new TextObject("{=hCavIm4G}You will pay {AMOUNT}{GOLD_ICON} denars.", null);
			explanation.SetTextVariable("AMOUNT", bribeToEnterDungeon);
			explanation.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			return true;
		}

		private bool conversation_prison_guard_reject_visit_prison_on_condition()
		{
			bool flag = Settlement.CurrentSettlement.BribePaid >= Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterDungeon(Settlement.CurrentSettlement);
			StringHelpers.SetCharacterProperties("SETTLEMENT_OWNER", Settlement.CurrentSettlement.OwnerClan.Leader.CharacterObject, null, false);
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterDungeon(Settlement.CurrentSettlement, ref accessDetails);
			return !flag && accessDetails.AccessLevel != 2;
		}

		private void conversation_prison_guard_visit_prison_on_consequence()
		{
			if (Settlement.CurrentSettlement.IsFortification)
			{
				Campaign.Current.GameMenuManager.NextLocation = LocationComplex.Current.GetLocationWithId("prison");
				Campaign.Current.GameMenuManager.PreviousLocation = LocationComplex.Current.GetLocationWithId("center");
			}
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				Mission.Current.EndMission();
			};
		}

		private bool conversation_guard_start_on_condition()
		{
			if (Campaign.Current.ConversationManager.OneToOneConversationAgent == null || this.CheckIfConversationAgentIsEscortingTheMainAgent())
			{
				return false;
			}
			if (CharacterObject.OneToOneConversationCharacter.Occupation == 7 && PlayerEncounter.Current != null && PlayerEncounter.InsideSettlement)
			{
				TextObject textObject = new TextObject("{=6JL4GyKC}Can't talk right now. Got to keep my eye on things around here.", null);
				if (Settlement.CurrentSettlement.OwnerClan == Hero.MainHero.Clan || Settlement.CurrentSettlement.MapFaction.Leader == Hero.MainHero)
				{
					textObject = new TextObject("{=xizHRti3}Nothing to report, your lordship.", null);
					if (Hero.MainHero.IsFemale)
					{
						textObject = new TextObject("{=sIfL5Vnx}Nothing to report, your ladyship.", null);
					}
				}
				else if (Settlement.CurrentSettlement.IsTown && Settlement.CurrentSettlement.Town.Security <= 20f)
				{
					textObject = new TextObject("{=3sfjBnaJ}It's quiet. Too quiet. Things never stay quiet around here for long.", null);
				}
				else if (Settlement.CurrentSettlement.IsTown && Settlement.CurrentSettlement.Town.Security <= 40f)
				{
					textObject = new TextObject("{=jjkOBPkY}Can't let down your guard around here. Too many bastards up to no good.", null);
				}
				else if (Settlement.CurrentSettlement.IsTown && Settlement.CurrentSettlement.Town.Security >= 70f)
				{
					textObject = new TextObject("{=AHg5k9q2}Welcome to {SETTLEMENT_NAME}. I think you'll find these are good, law-abiding folk, for the most part.", null);
					textObject.SetTextVariable("SETTLEMENT_NAME", Settlement.CurrentSettlement.Name);
				}
				MBTextManager.SetTextVariable("GUARD_COMMENT", textObject, false);
				return true;
			}
			return false;
		}

		private bool CheckIfConversationAgentIsEscortingTheMainAgent()
		{
			return Agent.Main != null && Agent.Main.IsActive() && Settlement.CurrentSettlement != null && ConversationMission.OneToOneConversationAgent != null && EscortAgentBehavior.CheckIfAgentIsEscortedBy(ConversationMission.OneToOneConversationAgent, Agent.Main);
		}

		private bool conversation_prison_guard_visit_prison_nobody_inside_condition()
		{
			return Settlement.CurrentSettlement.SettlementComponent.GetPrisonerHeroes().Count == 0;
		}

		private bool prison_guard_visit_permission_try_bribe_on_condition()
		{
			int bribeToEnterDungeon = Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterDungeon(Settlement.CurrentSettlement);
			MBTextManager.SetTextVariable("AMOUNT", bribeToEnterDungeon);
			MBTextManager.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">", false);
			return Hero.MainHero.Gold >= bribeToEnterDungeon && !Campaign.Current.IsMainHeroDisguised;
		}

		private void conversation_prison_guard_visit_permission_bribe_on_consequence()
		{
			int bribeToEnterDungeon = Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterDungeon(Settlement.CurrentSettlement);
			BribeGuardsAction.Apply(Settlement.CurrentSettlement, bribeToEnterDungeon);
			this.conversation_prison_guard_visit_prison_on_consequence();
		}

		private void conversation_prison_guard_visit_break_on_consequence()
		{
		}

		private bool IsCastleGuard()
		{
			Agent oneToOneConversationAgent = ConversationMission.OneToOneConversationAgent;
			AgentNavigator agentNavigator = ((oneToOneConversationAgent != null) ? oneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator : null);
			bool flag = false;
			if (agentNavigator != null)
			{
				flag = agentNavigator.TargetUsableMachine != null && oneToOneConversationAgent.IsUsingGameObject && agentNavigator.TargetUsableMachine.GameEntity.HasTag("sp_guard_castle");
				if (!flag && (agentNavigator.SpecialTargetTag == "sp_guard_castle" || agentNavigator.SpecialTargetTag == "sp_guard"))
				{
					Location lordsHallLocation = LocationComplex.Current.GetLocationWithId("lordshall");
					MissionAgentHandler missionBehavior = Mission.Current.GetMissionBehavior<MissionAgentHandler>();
					if (missionBehavior != null)
					{
						UsableMachine usableMachine = missionBehavior.TownPassageProps.Find((UsableMachine x) => ((Passage)x).ToLocation == lordsHallLocation);
						if (usableMachine != null && usableMachine.GameEntity.GlobalPosition.DistanceSquared(oneToOneConversationAgent.Position) < 100f)
						{
							flag = true;
						}
					}
				}
			}
			return flag;
		}

		private bool conversation_castle_guard_start_on_condition()
		{
			return !Campaign.Current.IsMainHeroDisguised && this.IsCastleGuard() && (Settlement.CurrentSettlement.MapFaction == Hero.MainHero.MapFaction || (Settlement.CurrentSettlement.MapFaction != Hero.MainHero.MapFaction && !Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingModerate(Settlement.CurrentSettlement.MapFaction) && !Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(Settlement.CurrentSettlement.MapFaction) && !Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingMild(Settlement.CurrentSettlement.MapFaction)));
		}

		private bool conversation_castle_guard_criminal_start_on_condition()
		{
			return !Campaign.Current.IsMainHeroDisguised && this.IsCastleGuard() && Settlement.CurrentSettlement.MapFaction != Hero.MainHero.MapFaction && (Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingModerate(Settlement.CurrentSettlement.MapFaction) || Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(Settlement.CurrentSettlement.MapFaction) || Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingMild(Settlement.CurrentSettlement.MapFaction));
		}

		private bool conversation_castle_guard_nobody_inside_condition()
		{
			return !LocationComplex.Current.GetLocationWithId("lordshall").GetCharacterList().Any((LocationCharacter c) => c.Character.IsHero && c.Character.HeroObject.IsLord) && Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterLordsHall(Settlement.CurrentSettlement) > 0 && Settlement.CurrentSettlement.OwnerClan != Clan.PlayerClan;
		}

		private bool conversation_castle_guard_player_can_enter_lordshall_condition()
		{
			bool flag;
			TextObject textObject;
			return Campaign.Current.Models.SettlementAccessModel.CanMainHeroAccessLocation(Settlement.CurrentSettlement, "lordshall", ref flag, ref textObject);
		}

		private bool conversation_player_bribe_to_enter_lords_hall_on_condition()
		{
			int bribeToEnterLordsHall = Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterLordsHall(Settlement.CurrentSettlement);
			MBTextManager.SetTextVariable("AMOUNT", bribeToEnterLordsHall);
			MBTextManager.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">", false);
			return bribeToEnterLordsHall > 0 && Hero.MainHero.Gold >= bribeToEnterLordsHall && !Campaign.Current.IsMainHeroDisguised && !this.conversation_castle_guard_nobody_inside_condition();
		}

		private void conversation_player_bribe_to_enter_lords_hall_on_consequence()
		{
			int bribeToEnterLordsHall = Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterLordsHall(Settlement.CurrentSettlement);
			BribeGuardsAction.Apply(Settlement.CurrentSettlement, bribeToEnterLordsHall);
		}

		private void OpenLordsHallMission()
		{
			Campaign.Current.GameMenuManager.NextLocation = LocationComplex.Current.GetLocationWithId("lordshall");
			Campaign.Current.GameMenuManager.PreviousLocation = LocationComplex.Current.GetLocationWithId("center");
			Mission.Current.EndMission();
		}

		private bool conversation_disguised_start_on_condition()
		{
			return Campaign.Current.IsMainHeroDisguised && (this.IsCastleGuard() || CharacterObject.OneToOneConversationCharacter.Occupation == 23 || CharacterObject.OneToOneConversationCharacter.Occupation == 24 || CharacterObject.OneToOneConversationCharacter.Occupation == 5 || CharacterObject.OneToOneConversationCharacter.Occupation == 7);
		}

		private bool conversation_disguised_start_on_condition_alt()
		{
			return Campaign.Current.IsMainHeroDisguised && MBRandom.RandomInt(2) == 0 && (this.IsCastleGuard() || CharacterObject.OneToOneConversationCharacter.Occupation == 23 || CharacterObject.OneToOneConversationCharacter.Occupation == 24 || CharacterObject.OneToOneConversationCharacter.Occupation == 5 || CharacterObject.OneToOneConversationCharacter.Occupation == 7);
		}

		public const float UnarmedTownGuardSpawnRate = 0.4f;

		private readonly List<ValueTuple<CharacterObject, int>> _garrisonTroops = new List<ValueTuple<CharacterObject, int>>();
	}
}
