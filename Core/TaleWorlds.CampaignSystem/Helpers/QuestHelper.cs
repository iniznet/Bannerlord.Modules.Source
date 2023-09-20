using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Helpers
{
	// Token: 0x0200000F RID: 15
	public static class QuestHelper
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00007124 File Offset: 0x00005324
		public static void AddMapArrowFromPointToTarget(TextObject name, Vec2 sourcePosition, Vec2 targetPosition, float life, float error)
		{
			Vec2 vec = targetPosition - sourcePosition;
			vec.Normalize();
			vec.x += error * (MBRandom.RandomFloat - 0.5f);
			vec.y += error * (MBRandom.RandomFloat - 0.5f);
			vec.Normalize();
			Vec2 vec2 = sourcePosition + vec * 4f;
			IMapTracksCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IMapTracksCampaignBehavior>();
			if (campaignBehavior == null)
			{
				return;
			}
			campaignBehavior.AddMapArrow(name, vec2, vec, life);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000071A5 File Offset: 0x000053A5
		public static bool CheckGoldForAlternativeSolution(int requiredGold, ref TextObject explanation)
		{
			if (Hero.MainHero.Gold < requiredGold)
			{
				explanation = new TextObject("{=jkYQmtIF}You need to have at least {GOLD_AMOUNT}{GOLD_ICON} to pay for the expenses beforehand.", null);
				explanation.SetTextVariable("GOLD_AMOUNT", requiredGold);
				return false;
			}
			return true;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000071D2 File Offset: 0x000053D2
		public static List<SkillObject> GetAlternativeSolutionMeleeSkills()
		{
			return new List<SkillObject>
			{
				DefaultSkills.OneHanded,
				DefaultSkills.TwoHanded,
				DefaultSkills.Polearm
			};
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000071FC File Offset: 0x000053FC
		public static bool CheckRosterForAlternativeSolution(TroopRoster troopRoster, int requiredTroopCount, ref TextObject explanation, int minimumTier = 0, bool mountedRequired = false)
		{
			int num = 0;
			foreach (TroopRosterElement troopRosterElement in troopRoster.GetTroopRoster())
			{
				if (!troopRosterElement.Character.IsHero && !troopRosterElement.Character.IsNotTransferableInPartyScreen && (!mountedRequired || troopRosterElement.Character.IsMounted) && (minimumTier == 0 || troopRosterElement.Character.Tier >= minimumTier))
				{
					num += troopRosterElement.Number - troopRosterElement.WoundedNumber;
				}
			}
			if (num < requiredTroopCount)
			{
				if (minimumTier == 0)
				{
					explanation = new TextObject("{=AdkSktd2}You have to send {NUMBER} {?MOUNTED}cavalry {?}{\\?}troops to this quest.", null);
				}
				else
				{
					explanation = new TextObject("{=Cg3hH8gN}You have to send {NUMBER} {?MOUNTED}cavalry {?}{\\?}troops with at least tier {TIER} to this quest.", null);
					explanation.SetTextVariable("TIER", minimumTier);
				}
				explanation.SetTextVariable("MOUNTED", mountedRequired ? 1 : 0);
				explanation.SetTextVariable("NUMBER", requiredTroopCount);
				return false;
			}
			return true;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000072F0 File Offset: 0x000054F0
		public static List<SkillObject> GetAlternativeSolutionRangedSkills()
		{
			return new List<SkillObject>
			{
				DefaultSkills.Bow,
				DefaultSkills.Crossbow,
				DefaultSkills.Throwing
			};
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00007318 File Offset: 0x00005518
		public static bool CheckMinorMajorCoercion(QuestBase questToCheck, MapEvent mapEvent, PartyBase attackerParty)
		{
			return (mapEvent.IsForcingSupplies || mapEvent.IsForcingVolunteers) && attackerParty == PartyBase.MainParty && mapEvent.MapEventSettlement.IsVillage && (QuestManager.QuestExistInClan(questToCheck, mapEvent.MapEventSettlement.OwnerClan) || QuestManager.QuestExistInSettlementNotables(questToCheck, mapEvent.MapEventSettlement));
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00007370 File Offset: 0x00005570
		public static void ApplyGenericMinorMajorCoercionConsequences(QuestBase quest, MapEvent mapEvent)
		{
			TextObject textObject = new TextObject("{=tWZ4a8Ih}You are accused in {SETTLEMENT} of a crime and {QUEST_GIVER.LINK} no longer trusts you in this matter.", null);
			textObject.SetTextVariable("SETTLEMENT", mapEvent.MapEventSettlement.EncyclopediaLinkWithName);
			StringHelpers.SetCharacterProperties("QUEST_GIVER", quest.QuestGiver.CharacterObject, textObject, false);
			quest.CompleteQuestWithFail(textObject);
			ChangeRelationAction.ApplyPlayerRelation(quest.QuestGiver, -10, true, true);
			quest.QuestGiver.AddPower(-10f);
			TraitLevelingHelper.OnIssueSolvedThroughAlternativeSolution(Hero.MainHero, new Tuple<TraitObject, int>[]
			{
				new Tuple<TraitObject, int>(DefaultTraits.Honor, -50)
			});
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00007400 File Offset: 0x00005600
		public static int GetAveragePriceOfItemInTheWorld(ItemObject item)
		{
			int num = 0;
			int num2 = 0;
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsTown)
				{
					num2 += settlement.Town.GetItemPrice(item, null, false);
					num++;
				}
				else if (settlement.IsVillage)
				{
					num2 += settlement.Village.GetItemPrice(item, null, false);
					num++;
				}
			}
			return num2 / num;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00007490 File Offset: 0x00005690
		public static void CheckWarDeclarationAndFailOrCancelTheQuest(QuestBase questToCheck, IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail, TextObject failLog, TextObject cancelLog)
		{
			if (questToCheck.QuestGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				if (DiplomacyHelper.IsWarCausedByPlayer(faction1, faction2, detail))
				{
					questToCheck.CompleteQuestWithFail(failLog);
					return;
				}
				questToCheck.CompleteQuestWithCancel(cancelLog);
			}
		}
	}
}
