using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x02000096 RID: 150
	public class CommonTownsfolkCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060006ED RID: 1773 RVA: 0x000353B9 File Offset: 0x000335B9
		private float GetSpawnRate(Settlement settlement)
		{
			return this.TimeOfDayPercentage() * this.GetProsperityMultiplier(settlement.SettlementComponent);
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x000353CE File Offset: 0x000335CE
		private float GetConfigValue()
		{
			return BannerlordConfig.CivilianAgentCount;
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x000353D5 File Offset: 0x000335D5
		private float GetProsperityMultiplier(SettlementComponent settlement)
		{
			return (settlement.GetProsperityLevel() + 1f) / 3f;
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x000353EC File Offset: 0x000335EC
		private float TimeOfDayPercentage()
		{
			return 1f - MathF.Abs(CampaignTime.Now.CurrentHourInDay - 15f) / 15f;
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0003541D File Offset: 0x0003361D
		public override void RegisterEvents()
		{
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x00035436 File Offset: 0x00033636
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x00035438 File Offset: 0x00033638
		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			if (!settlement.IsCastle)
			{
				Location locationWithId = settlement.LocationComplex.GetLocationWithId("center");
				Location locationWithId2 = settlement.LocationComplex.GetLocationWithId("tavern");
				if (CampaignMission.Current.Location == locationWithId)
				{
					this.AddPeopleToTownCenter(settlement, unusedUsablePointCount, CampaignTime.Now.IsDayTime);
				}
				if (CampaignMission.Current.Location == locationWithId2)
				{
					this.AddPeopleToTownTavern(settlement, unusedUsablePointCount);
				}
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x000354B4 File Offset: 0x000336B4
		private void AddPeopleToTownTavern(Settlement settlement, Dictionary<string, int> unusedUsablePointCount)
		{
			Location locationWithId = settlement.LocationComplex.GetLocationWithId("tavern");
			int num;
			unusedUsablePointCount.TryGetValue("npc_common", out num);
			if (num > 0)
			{
				int num2 = (int)((float)num * 0.3f);
				if (num2 > 0)
				{
					locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateTownsManForTavern), settlement.Culture, 0, num2);
				}
				int num3 = (int)((float)num * 0.1f);
				if (num3 > 0)
				{
					locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateTownsWomanForTavern), settlement.Culture, 0, num3);
				}
			}
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x00035534 File Offset: 0x00033734
		private void AddPeopleToTownCenter(Settlement settlement, Dictionary<string, int> unusedUsablePointCount, bool isDayTime)
		{
			Location locationWithId = settlement.LocationComplex.GetLocationWithId("center");
			CultureObject culture = settlement.Culture;
			int num;
			unusedUsablePointCount.TryGetValue("npc_common", out num);
			int num2;
			unusedUsablePointCount.TryGetValue("npc_common_limited", out num2);
			float num3 = (float)(num + num2) * 0.65000004f;
			if (num3 != 0f)
			{
				float num4 = MBMath.ClampFloat(this.GetConfigValue() / num3, 0f, 1f);
				float num5 = this.GetSpawnRate(settlement) * num4;
				if (num > 0)
				{
					int num6 = (int)((float)num * 0.2f * num5);
					if (num6 > 0)
					{
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateTownsMan), culture, 0, num6);
					}
					int num7 = (int)((float)num * 0.15f * num5);
					if (num7 > 0)
					{
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateTownsWoman), culture, 0, num7);
					}
				}
				if (isDayTime)
				{
					if (num2 > 0)
					{
						int num8 = (int)((float)num2 * 0.15f * num5);
						if (num8 > 0)
						{
							locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateTownsManCarryingStuff), culture, 0, num8);
						}
						int num9 = (int)((float)num2 * 0.1f * num5);
						if (num9 > 0)
						{
							locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateTownsWomanCarryingStuff), culture, 0, num9);
						}
						int num10 = (int)((float)num2 * 0.05f * num5);
						if (num10 > 0)
						{
							locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateMaleChild), culture, 0, num10);
							locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateFemaleChild), culture, 0, num10);
							locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateMaleTeenager), culture, 0, num10);
							locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateFemaleTeenager), culture, 0, num10);
						}
					}
					int num11 = 0;
					if (unusedUsablePointCount.TryGetValue("spawnpoint_cleaner", out num11))
					{
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateBroomsWoman), culture, 0, num11);
					}
					if (unusedUsablePointCount.TryGetValue("npc_dancer", out num11))
					{
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateDancer), culture, 0, num11);
					}
					if (settlement.IsTown && unusedUsablePointCount.TryGetValue("npc_beggar", out num11))
					{
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateFemaleBeggar), culture, 0, (num11 == 1) ? 0 : (num11 / 2));
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CommonTownsfolkCampaignBehavior.CreateMaleBeggar), culture, 0, (num11 == 1) ? 1 : (num11 / 2));
					}
				}
			}
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x00035778 File Offset: 0x00033978
		public static string GetActionSetSuffixAndMonsterForItem(string itemId, int race, bool isFemale, out Monster monster)
		{
			monster = FaceGen.GetMonsterWithSuffix(race, "_settlement");
			uint num = <PrivateImplementationDetails>.ComputeStringHash(itemId);
			if (num <= 2354022098U)
			{
				if (num <= 524654717U)
				{
					if (num != 330511441U)
					{
						if (num != 423989003U)
						{
							if (num != 524654717U)
							{
								goto IL_20B;
							}
							if (!(itemId == "_to_carry_bed_convolute_g"))
							{
								goto IL_20B;
							}
							return "_villager_carry_on_shoulder";
						}
						else
						{
							if (!(itemId == "_to_carry_bed_convolute_a"))
							{
								goto IL_20B;
							}
							return "_villager_carry_front";
						}
					}
					else
					{
						if (!(itemId == "_to_carry_bd_basket_a"))
						{
							goto IL_20B;
						}
						return "_villager_with_backpack";
					}
				}
				else if (num != 1406916035U)
				{
					if (num != 1726492488U)
					{
						if (num != 2354022098U)
						{
							goto IL_20B;
						}
						if (!(itemId == "_to_carry_kitchen_pot_c"))
						{
							goto IL_20B;
						}
						return "_villager_carry_right_hand";
					}
					else if (!(itemId == "_to_carry_foods_watermelon_a"))
					{
						goto IL_20B;
					}
				}
				else
				{
					if (!(itemId == "_to_carry_arm_kitchen_pot_c"))
					{
						goto IL_20B;
					}
					return "_villager_carry_right_arm";
				}
			}
			else if (num <= 3512086304U)
			{
				if (num != 2481184366U)
				{
					if (num != 3004030871U)
					{
						if (num != 3512086304U)
						{
							goto IL_20B;
						}
						if (!(itemId == "_to_carry_bd_fabric_c"))
						{
							goto IL_20B;
						}
					}
					else
					{
						if (!(itemId == "_to_carry_foods_basket_apple"))
						{
							goto IL_20B;
						}
						return "_villager_carry_over_head_v2";
					}
				}
				else
				{
					if (!(itemId == "_to_carry_kitchen_pitcher_a"))
					{
						goto IL_20B;
					}
					return "_villager_carry_over_head";
				}
			}
			else if (num <= 3737849652U)
			{
				if (num != 3710634116U)
				{
					if (num != 3737849652U)
					{
						goto IL_20B;
					}
					if (!(itemId == "_to_carry_merchandise_hides_b"))
					{
						goto IL_20B;
					}
					return "_villager_with_backpack";
				}
				else
				{
					if (!(itemId == "practice_spear_t1"))
					{
						goto IL_20B;
					}
					return "_villager_with_staff";
				}
			}
			else if (num != 4035495654U)
			{
				if (num != 4038602446U)
				{
					goto IL_20B;
				}
				if (!(itemId == "simple_sparth_axe_t2"))
				{
					goto IL_20B;
				}
				return "_villager_carry_axe";
			}
			else
			{
				if (!(itemId == "_to_carry_foods_pumpkin_a"))
				{
					goto IL_20B;
				}
				return "_villager_carry_front_v2";
			}
			return "_villager_carry_right_side";
			IL_20B:
			return "_villager_carry_right_hand";
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x00035998 File Offset: 0x00033B98
		public static Tuple<string, Monster> GetRandomTownsManActionSetAndMonster(int race)
		{
			int num = MBRandom.RandomInt(3);
			Monster monster;
			if (num == 0)
			{
				monster = FaceGen.GetMonsterWithSuffix(race, "_settlement");
				return new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monster, false, "_villager"), monster);
			}
			if (num != 1)
			{
				monster = FaceGen.GetMonsterWithSuffix(race, "_settlement");
				return new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monster, false, "_villager_3"), monster);
			}
			monster = FaceGen.GetMonsterWithSuffix(race, "_settlement_slow");
			return new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monster, false, "_villager_2"), monster);
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x00035A14 File Offset: 0x00033C14
		public static Tuple<string, Monster> GetRandomTownsWomanActionSetAndMonster(int race)
		{
			Monster monster;
			if (MBRandom.RandomInt(4) == 0)
			{
				monster = FaceGen.GetMonsterWithSuffix(race, "_settlement_fast");
				return new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monster, true, "_villager"), monster);
			}
			monster = FaceGen.GetMonsterWithSuffix(race, "_settlement_slow");
			return new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monster, true, "_villager_2"), monster);
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00035A68 File Offset: 0x00033C68
		private static LocationCharacter CreateTownsMan(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townsman = culture.Townsman;
			Tuple<string, Monster> randomTownsManActionSetAndMonster = CommonTownsfolkCampaignBehavior.GetRandomTownsManActionSetAndMonster(townsman.Race);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townsman, ref num, ref num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(townsman, -1, null, default(UniqueTroopDescriptor))).Monster(randomTownsManActionSetAndMonster.Item2).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common", false, relation, randomTownsManActionSetAndMonster.Item1, true, false, null, false, false, true);
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00035B04 File Offset: 0x00033D04
		private static LocationCharacter CreateTownsManForTavern(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townsman = culture.Townsman;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townsman.Race, "_settlement_slow");
			string text;
			if (culture.StringId.ToLower() == "aserai" || culture.StringId.ToLower() == "khuzait")
			{
				text = ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, townsman.IsFemale, "_villager_in_aserai_tavern");
			}
			else
			{
				text = ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, townsman.IsFemale, "_villager_in_tavern");
			}
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townsman, ref num, ref num2, "TavernVisitor");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(townsman, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common", true, relation, text, true, false, null, false, false, true);
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x00035BF0 File Offset: 0x00033DF0
		private static LocationCharacter CreateTownsWomanForTavern(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townswoman = culture.Townswoman;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townswoman.Race, "_settlement_slow");
			string text;
			if (culture.StringId.ToLower() == "aserai" || culture.StringId.ToLower() == "khuzait")
			{
				text = ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, townswoman.IsFemale, "_warrior_in_aserai_tavern");
			}
			else
			{
				text = ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, townswoman.IsFemale, "_warrior_in_tavern");
			}
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townswoman, ref num, ref num2, "TavernVisitor");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(townswoman, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common", true, relation, text, true, false, null, false, false, true);
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x00035CDC File Offset: 0x00033EDC
		private static LocationCharacter CreateTownsManCarryingStuff(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townsman = culture.Townsman;
			string randomStuff = SettlementHelper.GetRandomStuff(false);
			Monster monster;
			string actionSetSuffixAndMonsterForItem = CommonTownsfolkCampaignBehavior.GetActionSetSuffixAndMonsterForItem(randomStuff, townsman.Race, false, out monster);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townsman, ref num, ref num2, "TownsfolkCarryingStuff");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townsman, -1, null, default(UniqueTroopDescriptor))).Monster(monster).Age(MBRandom.RandomInt(num, num2));
			ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(randomStuff);
			LocationCharacter locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common_limited", false, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, townsman.IsFemale, actionSetSuffixAndMonsterForItem), true, false, @object, false, false, true);
			if (@object == null)
			{
				locationCharacter.PrefabNamesForBones.Add(agentData.AgentMonster.MainHandItemBoneIndex, randomStuff);
			}
			return locationCharacter;
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x00035DC4 File Offset: 0x00033FC4
		private static LocationCharacter CreateTownsWoman(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townswoman = culture.Townswoman;
			Tuple<string, Monster> randomTownsWomanActionSetAndMonster = CommonTownsfolkCampaignBehavior.GetRandomTownsWomanActionSetAndMonster(townswoman.Race);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townswoman, ref num, ref num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(townswoman, -1, null, default(UniqueTroopDescriptor))).Monster(randomTownsWomanActionSetAndMonster.Item2).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common", false, relation, randomTownsWomanActionSetAndMonster.Item1, true, false, null, false, false, true);
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x00035E60 File Offset: 0x00034060
		private static LocationCharacter CreateMaleChild(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townsmanChild = culture.TownsmanChild;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townsmanChild.Race, "_child");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townsmanChild, ref num, ref num2, "Child");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townsmanChild, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common_limited", false, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, townsmanChild.IsFemale, "_child"), true, false, null, false, false, true);
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x00035F10 File Offset: 0x00034110
		private static LocationCharacter CreateFemaleChild(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townswomanChild = culture.TownswomanChild;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townswomanChild.Race, "_child");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townswomanChild, ref num, ref num2, "Child");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townswomanChild, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common_limited", false, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, townswomanChild.IsFemale, "_child"), true, false, null, false, false, true);
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x00035FC0 File Offset: 0x000341C0
		private static LocationCharacter CreateMaleTeenager(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townsmanTeenager = culture.TownsmanTeenager;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townsmanTeenager.Race, "_child");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townsmanTeenager, ref num, ref num2, "Teenager");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townsmanTeenager, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common_limited", false, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, townsmanTeenager.IsFemale, "_villager"), true, false, null, false, false, true);
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x00036070 File Offset: 0x00034270
		private static LocationCharacter CreateFemaleTeenager(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townswomanTeenager = culture.TownswomanTeenager;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townswomanTeenager.Race, "_child");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townswomanTeenager, ref num, ref num2, "Teenager");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townswomanTeenager, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common_limited", false, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, townswomanTeenager.IsFemale, "_villager"), true, false, null, false, false, true);
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x00036120 File Offset: 0x00034320
		private static LocationCharacter CreateTownsWomanCarryingStuff(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townswoman = culture.Townswoman;
			string randomStuff = SettlementHelper.GetRandomStuff(true);
			Monster monster;
			string actionSetSuffixAndMonsterForItem = CommonTownsfolkCampaignBehavior.GetActionSetSuffixAndMonsterForItem(randomStuff, townswoman.Race, false, out monster);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townswoman, ref num, ref num2, "TownsfolkCarryingStuff");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townswoman, -1, null, default(UniqueTroopDescriptor))).Monster(monster).Age(MBRandom.RandomInt(num, num2));
			ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(randomStuff);
			LocationCharacter locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common_limited", false, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, townswoman.IsFemale, actionSetSuffixAndMonsterForItem), true, false, @object, false, false, true);
			if (@object == null)
			{
				locationCharacter.PrefabNamesForBones.Add(agentData.AgentMonster.MainHandItemBoneIndex, randomStuff);
			}
			return locationCharacter;
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x00036208 File Offset: 0x00034408
		public static LocationCharacter CreateBroomsWoman(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townswoman = culture.Townswoman;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townswoman.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townswoman, ref num, ref num2, "BroomsWoman");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(townswoman, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "spawnpoint_cleaner", false, relation, null, true, false, null, false, false, true);
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x000362A0 File Offset: 0x000344A0
		private static LocationCharacter CreateDancer(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject femaleDancer = culture.FemaleDancer;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(femaleDancer.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(femaleDancer, ref num, ref num2, "Dancer");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(femaleDancer, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_dancer", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_dancer"), true, false, null, false, false, true);
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x00036350 File Offset: 0x00034550
		public static LocationCharacter CreateMaleBeggar(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject beggar = culture.Beggar;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(beggar.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(beggar, ref num, ref num2, "Beggar");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(beggar, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_beggar", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_beggar"), true, false, null, false, false, true);
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00036400 File Offset: 0x00034600
		public static LocationCharacter CreateFemaleBeggar(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject femaleBeggar = culture.FemaleBeggar;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(femaleBeggar.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(femaleBeggar, ref num, ref num2, "Beggar");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(femaleBeggar, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_beggar", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_beggar"), true, false, null, false, false, true);
		}

		// Token: 0x040002F7 RID: 759
		public const float TownsmanSpawnPercentageMale = 0.2f;

		// Token: 0x040002F8 RID: 760
		public const float TownsmanSpawnPercentageFemale = 0.15f;

		// Token: 0x040002F9 RID: 761
		public const float TownsmanSpawnPercentageLimitedMale = 0.15f;

		// Token: 0x040002FA RID: 762
		public const float TownsmanSpawnPercentageLimitedFemale = 0.1f;

		// Token: 0x040002FB RID: 763
		public const float TownOtherPeopleSpawnPercentage = 0.05f;

		// Token: 0x040002FC RID: 764
		public const float TownsmanSpawnPercentageTavernMale = 0.3f;

		// Token: 0x040002FD RID: 765
		public const float TownsmanSpawnPercentageTavernFemale = 0.1f;

		// Token: 0x040002FE RID: 766
		public const float BeggarSpawnPercentage = 0.33f;
	}
}
