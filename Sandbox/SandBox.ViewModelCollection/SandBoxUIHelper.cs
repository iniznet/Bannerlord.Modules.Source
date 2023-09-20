using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection
{
	// Token: 0x02000004 RID: 4
	public static class SandBoxUIHelper
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		private static void TooltipAddExplanation(List<TooltipProperty> properties, ref ExplainedNumber explainedNumber)
		{
			List<ValueTuple<string, float>> lines = explainedNumber.GetLines();
			if (lines.Count > 0)
			{
				for (int i = 0; i < lines.Count; i++)
				{
					ValueTuple<string, float> valueTuple = lines[i];
					string item = valueTuple.Item1;
					string changeValueString = SandBoxUIHelper.GetChangeValueString(valueTuple.Item2);
					properties.Add(new TooltipProperty(item, changeValueString, 0, false, 0));
				}
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020AF File Offset: 0x000002AF
		public static List<TooltipProperty> GetExplainedNumberTooltip(ref ExplainedNumber explanation)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			SandBoxUIHelper.TooltipAddExplanation(list, ref explanation);
			return list;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020BD File Offset: 0x000002BD
		public static List<TooltipProperty> GetBattleLootAwardTooltip(float lootPercentage)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			GameTexts.SetVariable("AMOUNT", lootPercentage);
			list.Add(new TooltipProperty(string.Empty, SandBoxUIHelper._lootStr.ToString(), 0, false, 0));
			return list;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020EC File Offset: 0x000002EC
		public static string GetSkillEffectText(SkillEffect effect, int skillLevel)
		{
			MBTextManager.SetTextVariable("a0", effect.GetPrimaryValue(skillLevel).ToString("0.0"), false);
			MBTextManager.SetTextVariable("a1", effect.GetSecondaryValue(skillLevel).ToString("0.0"), false);
			string text = effect.Description.ToString();
			if (effect.PrimaryRole == null || effect.PrimaryBonus == 0f)
			{
				return text;
			}
			TextObject textObject = GameTexts.FindText("role", effect.PrimaryRole.ToString());
			if (effect.SecondaryRole != null && effect.SecondaryBonus != 0f)
			{
				TextObject textObject2 = GameTexts.FindText("role", effect.SecondaryRole.ToString());
				return string.Format("({0} / {1}) {2} ", textObject.ToString(), textObject2.ToString(), text);
			}
			return string.Format("({0}) {1} ", textObject.ToString(), text);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021E0 File Offset: 0x000003E0
		public static string GetRecruitNotificationText(int recruitmentAmount)
		{
			object obj = GameTexts.FindText("str_settlement_recruit_notification", null);
			MBTextManager.SetTextVariable("RECRUIT_AMOUNT", recruitmentAmount);
			MBTextManager.SetTextVariable("ISPLURAL", recruitmentAmount > 1);
			return obj.ToString();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002210 File Offset: 0x00000410
		public static string GetItemSoldNotificationText(ItemRosterElement item, int itemAmount, bool fromHeroToSettlement)
		{
			string text = item.EquipmentElement.Item.ItemCategory.GetName().ToString();
			object obj = GameTexts.FindText("str_settlement_item_sold_notification", null);
			MBTextManager.SetTextVariable("IS_POSITIVE", !fromHeroToSettlement);
			MBTextManager.SetTextVariable("ITEM_AMOUNT", itemAmount);
			MBTextManager.SetTextVariable("ITEM_TYPE", text, false);
			return obj.ToString();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002276 File Offset: 0x00000476
		public static string GetTroopGivenToSettlementNotificationText(int givenAmount)
		{
			object obj = GameTexts.FindText("str_settlement_given_troop_notification", null);
			MBTextManager.SetTextVariable("TROOP_AMOUNT", givenAmount);
			MBTextManager.SetTextVariable("ISPLURAL", givenAmount > 1);
			return obj.ToString();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000022A8 File Offset: 0x000004A8
		internal static string GetItemsTradedNotificationText(List<ValueTuple<EquipmentElement, int>> items, bool isSelling)
		{
			TextObject textObject;
			if (isSelling)
			{
				textObject = SandBoxUIHelper._soldStr;
			}
			else
			{
				textObject = SandBoxUIHelper._purchasedStr;
			}
			List<IGrouping<ItemCategory, ValueTuple<EquipmentElement, int>>> list = (from i in items
				group i by i.Item1.Item.ItemCategory into i
				orderby i.Sum((ValueTuple<EquipmentElement, int> e) => e.Item2 * e.Item1.Item.Value)
				select i).ToList<IGrouping<ItemCategory, ValueTuple<EquipmentElement, int>>>();
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "GetItemsTradedNotificationText");
			int num = MathF.Min(3, list.Count);
			for (int j = 0; j < num; j++)
			{
				IGrouping<ItemCategory, ValueTuple<EquipmentElement, int>> grouping = list[j];
				int num2 = MathF.Abs(grouping.Sum((ValueTuple<EquipmentElement, int> x) => x.Item2));
				grouping.Key.GetName().ToString();
				SandBoxUIHelper._itemTransactionStr.SetTextVariable("ITEM_NAME", grouping.Key.GetName());
				SandBoxUIHelper._itemTransactionStr.SetTextVariable("ITEM_NUMBER", num2);
				mbstringBuilder.Append<string>(SandBoxUIHelper._itemTransactionStr.ToString());
			}
			textObject.SetTextVariable("ITEMS", mbstringBuilder.ToStringAndRelease());
			return textObject.ToString();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000023F0 File Offset: 0x000005F0
		public static List<TooltipProperty> GetSiegeEngineInProgressTooltip(SiegeEvent.SiegeEngineConstructionProgress engineInProgress)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (((engineInProgress != null) ? engineInProgress.SiegeEngine : null) != null)
			{
				int num = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(PlayerSiege.PlayerSide).SiegeEngines.DeployedSiegeEngines.Where((SiegeEvent.SiegeEngineConstructionProgress e) => !e.IsConstructed).ToList<SiegeEvent.SiegeEngineConstructionProgress>().IndexOf(engineInProgress);
				list = SandBoxUIHelper.GetSiegeEngineTooltip(engineInProgress.SiegeEngine);
				if (engineInProgress.IsConstructed)
				{
					string text = ((int)(engineInProgress.Hitpoints / engineInProgress.MaxHitPoints * 100f)).ToString();
					GameTexts.SetVariable("NUMBER", text);
					GameTexts.SetVariable("STR1", GameTexts.FindText("str_NUMBER_percent", null));
					GameTexts.SetVariable("LEFT", ((int)engineInProgress.Hitpoints).ToString());
					GameTexts.SetVariable("RIGHT", ((int)engineInProgress.MaxHitPoints).ToString());
					GameTexts.SetVariable("STR2", GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", null));
					list.Add(new TooltipProperty(GameTexts.FindText("str_hitpoints", null).ToString(), GameTexts.FindText("str_STR1_space_STR2", null).ToString(), 0, false, 0));
				}
				else
				{
					string text2 = MathF.Round(engineInProgress.Progress / 1f * 100f).ToString();
					GameTexts.SetVariable("NUMBER", text2);
					list.Add(new TooltipProperty(GameTexts.FindText("str_inprogress", null).ToString(), GameTexts.FindText("str_NUMBER_percent", null).ToString(), 0, false, 0));
					if (num == 0)
					{
						list.Add(new TooltipProperty(GameTexts.FindText("str_currently_building", null).ToString(), " ", 0, false, 0));
					}
					else if (num > 0)
					{
						list.Add(new TooltipProperty(GameTexts.FindText("str_in_queue", null).ToString(), num.ToString(), 0, false, 0));
					}
				}
			}
			return list;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000025D8 File Offset: 0x000007D8
		public static List<TooltipProperty> GetSiegeEngineTooltip(SiegeEngineType engine)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (engine != null)
			{
				list.Add(new TooltipProperty("", engine.Name.ToString(), 0, false, 4096));
				list.Add(new TooltipProperty("", engine.Description.ToString(), 0, false, 1));
				list.Add(new TooltipProperty(new TextObject("{=Ahy035gM}Build Cost", null).ToString(), engine.ManDayCost.ToString("F1"), 0, false, 0));
				float siegeEngineHitPoints = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineHitPoints(PlayerSiege.PlayerSiegeEvent, engine, PlayerSiege.PlayerSide);
				list.Add(new TooltipProperty(new TextObject("{=oBbiVeKE}Hit Points", null).ToString(), siegeEngineHitPoints.ToString(), 0, false, 0));
				if (engine.Difficulty > 0)
				{
					list.Add(new TooltipProperty(new TextObject("{=raD9MK3O}Difficulty", null).ToString(), engine.Difficulty.ToString(), 0, false, 0));
				}
				if (engine.ToolCost > 0)
				{
					list.Add(new TooltipProperty(new TextObject("{=lPMYSSAa}Tools Required", null).ToString(), engine.ToolCost.ToString(), 0, false, 0));
				}
				if (engine.IsRanged)
				{
					list.Add(new TooltipProperty(GameTexts.FindText("str_daily_rate_of_fire", null).ToString(), engine.CampaignRateOfFirePerDay.ToString("F1"), 0, false, 0));
					list.Add(new TooltipProperty(GameTexts.FindText("str_projectile_damage", null).ToString(), engine.Damage.ToString("F1"), 0, false, 0));
					list.Add(new TooltipProperty(" ", " ", 0, false, 0));
				}
			}
			return list;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002794 File Offset: 0x00000994
		public static List<TooltipProperty> GetWallSectionTooltip(Settlement settlement, int wallIndex)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (settlement.IsFortification)
			{
				list.Add(new TooltipProperty("", GameTexts.FindText("str_wall", null).ToString(), 0, false, 4096));
				list.Add(new TooltipProperty(" ", " ", 0, false, 0));
				float maxHitPointsOfOneWallSection = settlement.MaxHitPointsOfOneWallSection;
				float num = settlement.SettlementWallSectionHitPointsRatioList[wallIndex] * maxHitPointsOfOneWallSection;
				if (num > 0f)
				{
					string text = ((int)(num / maxHitPointsOfOneWallSection * 100f)).ToString();
					GameTexts.SetVariable("NUMBER", text);
					GameTexts.SetVariable("STR1", GameTexts.FindText("str_NUMBER_percent", null));
					GameTexts.SetVariable("LEFT", ((int)num).ToString());
					GameTexts.SetVariable("RIGHT", ((int)maxHitPointsOfOneWallSection).ToString());
					GameTexts.SetVariable("STR2", GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", null));
					list.Add(new TooltipProperty(GameTexts.FindText("str_hitpoints", null).ToString(), GameTexts.FindText("str_STR1_space_STR2", null).ToString(), 0, false, 0));
				}
				else
				{
					list.Add(new TooltipProperty(GameTexts.FindText("str_wall_breached", null).ToString(), " ", 0, false, 0));
				}
			}
			return list;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000028D9 File Offset: 0x00000AD9
		public static string GetPrisonersSoldNotificationText(int soldPrisonerAmount)
		{
			object obj = GameTexts.FindText("str_settlement_prisoner_sold_notification", null);
			MBTextManager.SetTextVariable("PRISONERS_AMOUNT", soldPrisonerAmount);
			MBTextManager.SetTextVariable("ISPLURAL", soldPrisonerAmount > 1);
			return obj.ToString();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000290C File Offset: 0x00000B0C
		public static List<ValueTuple<SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject>> GetQuestStateOfHero(Hero queriedHero)
		{
			List<ValueTuple<SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject>> list = new List<ValueTuple<SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject>>();
			if (Campaign.Current != null)
			{
				IssueBase relatedIssue;
				Campaign.Current.IssueManager.Issues.TryGetValue(queriedHero, ref relatedIssue);
				if (relatedIssue == null)
				{
					relatedIssue = queriedHero.Issue;
				}
				List<QuestBase> questsRelatedToHero = SandBoxUIHelper.GetQuestsRelatedToHero(queriedHero);
				if (questsRelatedToHero.Count > 0)
				{
					for (int i = 0; i < questsRelatedToHero.Count; i++)
					{
						if (questsRelatedToHero[i].QuestGiver == queriedHero)
						{
							list.Add(new ValueTuple<SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject>(questsRelatedToHero[i].IsSpecialQuest ? SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest : SandBoxUIHelper.IssueQuestFlags.ActiveIssue, questsRelatedToHero[i].Title, (questsRelatedToHero[i].JournalEntries.Count > 0) ? questsRelatedToHero[i].JournalEntries[0].LogText : TextObject.Empty));
						}
						else
						{
							list.Add(new ValueTuple<SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject>(questsRelatedToHero[i].IsSpecialQuest ? SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest : SandBoxUIHelper.IssueQuestFlags.TrackedIssue, questsRelatedToHero[i].Title, (questsRelatedToHero[i].JournalEntries.Count > 0) ? questsRelatedToHero[i].JournalEntries[0].LogText : TextObject.Empty));
						}
					}
				}
				bool flag;
				if (questsRelatedToHero != null)
				{
					IssueBase relatedIssue2 = relatedIssue;
					if (((relatedIssue2 != null) ? relatedIssue2.IssueQuest : null) != null)
					{
						flag = questsRelatedToHero.Any((QuestBase q) => q == relatedIssue.IssueQuest);
						goto IL_171;
					}
				}
				flag = false;
				IL_171:
				bool flag2 = flag;
				if (relatedIssue != null && !flag2)
				{
					ValueTuple<SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject> valueTuple = new ValueTuple<SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject>(SandBoxUIHelper.GetIssueType(relatedIssue), relatedIssue.Title, relatedIssue.Description);
					list.Add(valueTuple);
				}
			}
			return list;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002AC8 File Offset: 0x00000CC8
		public static List<QuestBase> GetQuestsRelatedToHero(Hero hero)
		{
			List<QuestBase> list = new List<QuestBase>();
			List<QuestBase> list2;
			Campaign.Current.QuestManager.TrackedObjects.TryGetValue(hero, ref list2);
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i].IsTrackEnabled)
					{
						list.Add(list2[i]);
					}
				}
			}
			IssueBase issue = hero.Issue;
			if (((issue != null) ? issue.IssueQuest : null) != null && hero.Issue.IssueQuest.IsTrackEnabled && !hero.Issue.IssueQuest.IsTracked(hero))
			{
				list.Add(hero.Issue.IssueQuest);
			}
			return list;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002B70 File Offset: 0x00000D70
		public static List<QuestBase> GetQuestsRelatedToParty(MobileParty party)
		{
			List<QuestBase> list = new List<QuestBase>();
			List<QuestBase> list2;
			Campaign.Current.QuestManager.TrackedObjects.TryGetValue(party, ref list2);
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i].IsTrackEnabled)
					{
						list.Add(list2[i]);
					}
				}
			}
			if (party.MemberRoster.TotalHeroes > 0)
			{
				if (party.LeaderHero != null && party.MemberRoster.TotalHeroes == 1)
				{
					List<QuestBase> questsRelatedToHero = SandBoxUIHelper.GetQuestsRelatedToHero(party.LeaderHero);
					if (questsRelatedToHero != null && questsRelatedToHero.Count > 0)
					{
						list.AddRange(questsRelatedToHero.Except(list));
					}
				}
				else
				{
					for (int j = 0; j < party.MemberRoster.Count; j++)
					{
						CharacterObject characterAtIndex = party.MemberRoster.GetCharacterAtIndex(j);
						Hero hero = ((characterAtIndex != null) ? characterAtIndex.HeroObject : null);
						if (hero != null)
						{
							List<QuestBase> questsRelatedToHero2 = SandBoxUIHelper.GetQuestsRelatedToHero(hero);
							if (questsRelatedToHero2 != null && questsRelatedToHero2.Count > 0)
							{
								list.AddRange(questsRelatedToHero2.Except(list));
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002C78 File Offset: 0x00000E78
		public static List<ValueTuple<bool, QuestBase>> GetQuestsRelatedToSettlement(Settlement settlement)
		{
			List<ValueTuple<bool, QuestBase>> list = new List<ValueTuple<bool, QuestBase>>();
			foreach (KeyValuePair<ITrackableCampaignObject, List<QuestBase>> keyValuePair in Campaign.Current.QuestManager.TrackedObjects)
			{
				Hero hero = keyValuePair.Key as Hero;
				MobileParty mobileParty = keyValuePair.Key as MobileParty;
				if ((hero != null && hero.CurrentSettlement == settlement) || (mobileParty != null && mobileParty.CurrentSettlement == settlement))
				{
					for (int i = 0; i < keyValuePair.Value.Count; i++)
					{
						bool flag = keyValuePair.Value[i].QuestGiver != null && (keyValuePair.Value[i].QuestGiver == hero || keyValuePair.Value[i].QuestGiver == ((mobileParty != null) ? mobileParty.LeaderHero : null));
						if (!list.Contains(new ValueTuple<bool, QuestBase>(flag, keyValuePair.Value[i])) && keyValuePair.Value[i].IsTrackEnabled)
						{
							list.Add(new ValueTuple<bool, QuestBase>(flag, keyValuePair.Value[i]));
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002DE4 File Offset: 0x00000FE4
		public static bool IsQuestRelatedToSettlement(QuestBase quest, Settlement settlement)
		{
			Hero questGiver = quest.QuestGiver;
			if (((questGiver != null) ? questGiver.CurrentSettlement : null) == settlement || quest.IsTracked(settlement))
			{
				return true;
			}
			foreach (KeyValuePair<ITrackableCampaignObject, List<QuestBase>> keyValuePair in Campaign.Current.QuestManager.TrackedObjects)
			{
				Hero hero = keyValuePair.Key as Hero;
				MobileParty mobileParty = keyValuePair.Key as MobileParty;
				if ((hero != null && hero.CurrentSettlement == settlement) || (mobileParty != null && mobileParty.CurrentSettlement == settlement))
				{
					for (int i = 0; i < keyValuePair.Value.Count; i++)
					{
						if (keyValuePair.Value[i].IsTrackEnabled && keyValuePair.Value[i] == quest)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002EDC File Offset: 0x000010DC
		public static SandBoxUIHelper.IssueQuestFlags GetIssueType(IssueBase issue)
		{
			if (issue.IsSolvingWithAlternative || issue.IsSolvingWithLordSolution || issue.IsSolvingWithQuest)
			{
				return SandBoxUIHelper.IssueQuestFlags.ActiveIssue;
			}
			return SandBoxUIHelper.IssueQuestFlags.AvailableIssue;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002EFC File Offset: 0x000010FC
		public static int GetPartyHealthyCount(MobileParty party)
		{
			int num = party.Party.NumberOfHealthyMembers;
			if (party.Army != null && party.Army.LeaderParty == party)
			{
				foreach (MobileParty mobileParty in party.Army.LeaderParty.AttachedParties)
				{
					num += mobileParty.Party.NumberOfHealthyMembers;
				}
			}
			return num;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002F84 File Offset: 0x00001184
		public static string GetPartyWoundedText(int woundedAmount)
		{
			TextObject textObject = new TextObject("{=O9nwLrYp}+{WOUNDEDAMOUNT}w", null);
			textObject.SetTextVariable("WOUNDEDAMOUNT", woundedAmount);
			return textObject.ToString();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002FA3 File Offset: 0x000011A3
		public static string GetPartyPrisonerText(int prisonerAmount)
		{
			TextObject textObject = new TextObject("{=CiIWjF3f}+{PRISONERAMOUNT}p", null);
			textObject.SetTextVariable("PRISONERAMOUNT", prisonerAmount);
			return textObject.ToString();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002FC4 File Offset: 0x000011C4
		public static int GetAllWoundedMembersAmount(MobileParty party)
		{
			int num = party.Party.NumberOfWoundedTotalMembers;
			if (party.Army != null && party.Army.LeaderParty == party)
			{
				num += party.Army.LeaderParty.AttachedParties.Sum((MobileParty p) => p.Party.NumberOfWoundedTotalMembers);
			}
			return num;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000302C File Offset: 0x0000122C
		public static int GetAllPrisonerMembersAmount(MobileParty party)
		{
			int num = party.Party.NumberOfPrisoners;
			if (party.Army != null && party.Army.LeaderParty == party)
			{
				num += party.Army.LeaderParty.AttachedParties.Sum((MobileParty p) => p.Party.NumberOfPrisoners);
			}
			return num;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003094 File Offset: 0x00001294
		public static CharacterCode GetCharacterCode(CharacterObject character, bool useCivilian = false)
		{
			TextObject textObject;
			if (character.IsHero && SandBoxUIHelper.IsHeroInformationHidden(character.HeroObject, out textObject))
			{
				return CharacterCode.CreateEmpty();
			}
			if (character.IsHero && character.HeroObject.IsLord)
			{
				string[] array = CharacterCode.CreateFrom(character).Code.Split(new string[] { "@---@" }, StringSplitOptions.RemoveEmptyEntries);
				Equipment equipment = ((useCivilian && character.FirstCivilianEquipment != null) ? character.FirstCivilianEquipment.Clone(false) : character.Equipment.Clone(false));
				equipment[5] = new EquipmentElement(null, null, null, false);
				for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
				{
					ItemObject item = equipment[equipmentIndex].Item;
					bool flag;
					if (item == null)
					{
						flag = false;
					}
					else
					{
						WeaponComponent weaponComponent = item.WeaponComponent;
						bool? flag2;
						if (weaponComponent == null)
						{
							flag2 = null;
						}
						else
						{
							WeaponComponentData primaryWeapon = weaponComponent.PrimaryWeapon;
							flag2 = ((primaryWeapon != null) ? new bool?(primaryWeapon.IsShield) : null);
						}
						bool? flag3 = flag2;
						bool flag4 = true;
						flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					if (flag)
					{
						equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, default(EquipmentElement));
					}
				}
				array[0] = equipment.CalculateEquipmentCode();
				return CharacterCode.CreateFrom(string.Join("@---@", array));
			}
			return CharacterCode.CreateFrom(character);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000031D4 File Offset: 0x000013D4
		public static bool IsHeroInformationHidden(Hero hero, out TextObject disableReason)
		{
			bool flag = !Campaign.Current.Models.InformationRestrictionModel.DoesPlayerKnowDetailsOf(hero);
			disableReason = (flag ? new TextObject("{=akHsjtPh}You haven't met this hero yet.", null) : TextObject.Empty);
			return flag;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003212 File Offset: 0x00001412
		public static SandBoxUIHelper.MapEventVisualTypes GetMapEventVisualTypeFromMapEvent(MapEvent mapEvent)
		{
			if (mapEvent.MapEventSettlement == null)
			{
				return SandBoxUIHelper.MapEventVisualTypes.Battle;
			}
			if (mapEvent.IsSiegeAssault || mapEvent.IsSiegeOutside)
			{
				return SandBoxUIHelper.MapEventVisualTypes.Siege;
			}
			if (mapEvent.IsSallyOut)
			{
				return SandBoxUIHelper.MapEventVisualTypes.SallyOut;
			}
			return SandBoxUIHelper.MapEventVisualTypes.Raid;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000323C File Offset: 0x0000143C
		private static string GetChangeValueString(float value)
		{
			string text = value.ToString("0.##");
			if (value > 0.001f)
			{
				MBTextManager.SetTextVariable("NUMBER", text, false);
				return GameTexts.FindText("str_plus_with_number", null).ToString();
			}
			return text;
		}

		// Token: 0x04000001 RID: 1
		public static SandBoxUIHelper.IssueQuestFlags[] IssueQuestFlagsValues = (SandBoxUIHelper.IssueQuestFlags[])Enum.GetValues(typeof(SandBoxUIHelper.IssueQuestFlags));

		// Token: 0x04000002 RID: 2
		private static readonly TextObject _soldStr = new TextObject("{=YgyHVu8S}Sold{ITEMS}", null);

		// Token: 0x04000003 RID: 3
		private static readonly TextObject _purchasedStr = new TextObject("{=qIeDZoSx}Purchased{ITEMS}", null);

		// Token: 0x04000004 RID: 4
		private static readonly TextObject _itemTransactionStr = new TextObject("{=CqAhj27p} {ITEM_NAME} x{ITEM_NUMBER}", null);

		// Token: 0x04000005 RID: 5
		private static readonly TextObject _lootStr = new TextObject("{=nvemmBZz}You earned {AMOUNT}% of the loot and prisoners", null);

		// Token: 0x0200003F RID: 63
		[Flags]
		public enum IssueQuestFlags
		{
			// Token: 0x0400024B RID: 587
			None = 0,
			// Token: 0x0400024C RID: 588
			AvailableIssue = 1,
			// Token: 0x0400024D RID: 589
			ActiveIssue = 2,
			// Token: 0x0400024E RID: 590
			ActiveStoryQuest = 4,
			// Token: 0x0400024F RID: 591
			TrackedIssue = 8,
			// Token: 0x04000250 RID: 592
			TrackedStoryQuest = 16
		}

		// Token: 0x02000040 RID: 64
		public enum MapEventVisualTypes
		{
			// Token: 0x04000252 RID: 594
			None,
			// Token: 0x04000253 RID: 595
			Raid,
			// Token: 0x04000254 RID: 596
			Siege,
			// Token: 0x04000255 RID: 597
			Battle,
			// Token: 0x04000256 RID: 598
			Rebellion,
			// Token: 0x04000257 RID: 599
			SallyOut
		}
	}
}
