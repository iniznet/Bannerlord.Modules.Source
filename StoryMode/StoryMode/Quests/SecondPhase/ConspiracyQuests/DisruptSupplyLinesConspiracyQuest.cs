using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.SecondPhase.ConspiracyQuests
{
	// Token: 0x0200002A RID: 42
	internal class DisruptSupplyLinesConspiracyQuest : ConspiracyQuestBase
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600021F RID: 543 RVA: 0x0000C1A4 File Offset: 0x0000A3A4
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=y150haHv}Disrupt Supply Lines", null);
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000C1B4 File Offset: 0x0000A3B4
		public override TextObject SideNotificationText
		{
			get
			{
				TextObject textObject = new TextObject("{=IPP6MKfy}{MENTOR.LINK} notified you about a weapons caravan that will supply conspirators with weapons and armour.", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000221 RID: 545 RVA: 0x0000C1E8 File Offset: 0x0000A3E8
		public override TextObject StartMessageLogFromMentor
		{
			get
			{
				TextObject textObject = new TextObject("{=01Y1DAqA}{MENTOR.LINK} has sent you a message: As you may know, I receive reports from my spies in marketplaces around here. There is a merchant who I have been following - I know he is connected with {OTHER_MENTOR.LINK}. Now, I hear he has bought up a large supply of weapons and armor in {QUEST_FROM_SETTLEMENT_NAME}, and plans to travel to {QUEST_TO_SETTLEMENT_NAME}. From there it will move onward. I expect that {OTHER_MENTOR.LINK} is arming {?OTHER_MENTOR.GENDER}her{?}his{\\?} allies in the gangs in that area. If the caravan delivers its load, then I expect we will soon find some of our friends stabbed to death in the streets by hired thugs, and the rest of our friends too frightened to acknowledge us. I need you to track it down and destroy it. Try to intercept it on the first leg of its journey, before it gets to {QUEST_TO_SETTLEMENT_NAME}. If you fail, find out the next town to which it is going. It may take some time to find it, and when you do, it will be well guarded. But I trust in your perseverance, your skill and your understanding of how important this is. Good hunting.", null);
				StringHelpers.SetCharacterProperties("OTHER_MENTOR", StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? StoryModeHeroes.AntiImperialMentor.CharacterObject : StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("QUEST_FROM_SETTLEMENT_NAME", this.QuestFromSettlement.EncyclopediaLinkWithName);
				textObject.SetTextVariable("QUEST_TO_SETTLEMENT_NAME", this.QuestToSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000222 RID: 546 RVA: 0x0000C27C File Offset: 0x0000A47C
		public override TextObject StartLog
		{
			get
			{
				TextObject textObject = new TextObject("{=ZKdBlAmp}An arms caravan to resupply the conspirators will be soon on its way.{newline}{MENTOR.LINK}'s message:{newline}\"Our spies have learned about an arms caravan that is attempting to bring the conspirators high quality weapons and armor. We know that it will set out on its route from {QUEST_FROM_SETTLEMENT_NAME} to {QUEST_TO_SETTLEMENT_NAME} after {SPAWN_DAYS} days. We will find out and notify you about the new routes that it takes as it progresses.\"", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("QUEST_FROM_SETTLEMENT_NAME", this.QuestFromSettlement.EncyclopediaLinkWithName);
				textObject.SetTextVariable("QUEST_TO_SETTLEMENT_NAME", this.QuestToSettlement.EncyclopediaLinkWithName);
				textObject.SetTextVariable("SPAWN_DAYS", 5);
				return textObject;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000223 RID: 547 RVA: 0x0000C2E9 File Offset: 0x0000A4E9
		public override float ConspiracyStrengthDecreaseAmount
		{
			get
			{
				return 75f;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0000C2F0 File Offset: 0x0000A4F0
		private TextObject PlayerDefeatedCaravanLog
		{
			get
			{
				TextObject textObject = new TextObject("{=Db63Pe03}You have defeated the caravan and acquired its supplies. {OTHER_MENTOR.LINK}'s allies will not have their weapons. This will give us time and resources to prepare.", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("OTHER_MENTOR", StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? StoryModeHeroes.AntiImperialMentor.CharacterObject : StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000225 RID: 549 RVA: 0x0000C356 File Offset: 0x0000A556
		private TextObject MainHeroFailedToDisrupt
		{
			get
			{
				return new TextObject("{=9aRqqx3U}The caravan has delivered its supplies to the conspirators. A stronger adversary awaits us...", null);
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000226 RID: 550 RVA: 0x0000C363 File Offset: 0x0000A563
		private TextObject MainHeroLostCombat
		{
			get
			{
				return new TextObject("{=bT9yspaQ}You have lost the battle against the conspiracy's caravan. A stronger adversary awaits us...", null);
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000227 RID: 551 RVA: 0x0000C370 File Offset: 0x0000A570
		private Settlement QuestFromSettlement
		{
			get
			{
				return this._caravanTargetSettlements[0];
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000228 RID: 552 RVA: 0x0000C37A File Offset: 0x0000A57A
		private Settlement QuestToSettlement
		{
			get
			{
				return this._caravanTargetSettlements[this._caravanTargetSettlements.Length - 1];
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000229 RID: 553 RVA: 0x0000C38D File Offset: 0x0000A58D
		public MobileParty ConspiracyCaravan
		{
			get
			{
				return this._questCaravanMobileParty;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600022A RID: 554 RVA: 0x0000C395 File Offset: 0x0000A595
		public int CaravanPartySize
		{
			get
			{
				return 70 + 70 * (int)this.GetQuestDifficultyMultiplier();
			}
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000C3A4 File Offset: 0x0000A5A4
		public DisruptSupplyLinesConspiracyQuest(string questId, Hero questGiver)
			: base(questId, questGiver)
		{
			this._questStartTime = CampaignTime.Now;
			this._caravanTargetSettlements = new Settlement[7];
			this._caravanTargetSettlements[0] = this.GetQuestFromSettlement();
			for (int i = 1; i <= 6; i++)
			{
				this._caravanTargetSettlements[i] = this.GetNextSettlement(this._caravanTargetSettlements[i - 1]);
			}
			base.AddTrackedObject(this.QuestFromSettlement);
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000C410 File Offset: 0x0000A610
		private Settlement GetQuestFromSettlement()
		{
			Settlement settlement = SettlementHelper.FindRandomSettlement(delegate(Settlement s)
			{
				if (!s.IsTown || s.MapFaction == Clan.PlayerClan.MapFaction)
				{
					return false;
				}
				if (!StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine)
				{
					return !StoryModeData.IsKingdomImperial(s.OwnerClan.Kingdom);
				}
				return StoryModeData.IsKingdomImperial(s.OwnerClan.Kingdom);
			});
			if (settlement == null)
			{
				settlement = SettlementHelper.FindRandomSettlement(delegate(Settlement s)
				{
					if (!s.IsTown)
					{
						return false;
					}
					if (!StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine)
					{
						return !StoryModeData.IsKingdomImperial(s.OwnerClan.Kingdom);
					}
					return StoryModeData.IsKingdomImperial(s.OwnerClan.Kingdom);
				});
			}
			if (settlement == null)
			{
				settlement = SettlementHelper.FindRandomSettlement((Settlement s) => s.IsTown);
			}
			return settlement;
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000C494 File Offset: 0x0000A694
		private Settlement GetNextSettlement(Settlement settlement)
		{
			Settlement settlement2 = SettlementHelper.FindNearestTown((Settlement s) => !this._caravanTargetSettlements.Contains(s) && s.MapFaction != Clan.PlayerClan.MapFaction && (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? StoryModeData.IsKingdomImperial(s.OwnerClan.Kingdom) : (!StoryModeData.IsKingdomImperial(s.OwnerClan.Kingdom))) && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, s) > 100f && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, s) < 500f, settlement);
			if (settlement2 == null)
			{
				settlement2 = SettlementHelper.FindRandomSettlement((Settlement s) => !this._caravanTargetSettlements.Contains(s) && s.IsTown && s.MapFaction != Clan.PlayerClan.MapFaction && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, s) > 100f && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, s) < 500f);
			}
			if (settlement2 == null)
			{
				settlement2 = SettlementHelper.FindRandomSettlement((Settlement s) => !this._caravanTargetSettlements.Contains(s) && s.IsTown && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, s) > 100f && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, s) < 500f);
			}
			return settlement2;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000C4F8 File Offset: 0x0000A6F8
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000C500 File Offset: 0x0000A700
		protected override void OnTimedOut()
		{
			MobileParty questCaravanMobileParty = this._questCaravanMobileParty;
			if (questCaravanMobileParty != null && questCaravanMobileParty.IsActive)
			{
				DestroyPartyAction.Apply(null, this._questCaravanMobileParty);
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000C524 File Offset: 0x0000A724
		protected override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000C590 File Offset: 0x0000A790
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (this._questCaravanMobileParty != null && this._questCaravanMobileParty == party)
			{
				if (settlement == this.QuestToSettlement)
				{
					DestroyPartyAction.Apply(null, this._questCaravanMobileParty);
					this.FailedToDisrupt();
					return;
				}
				int num = Array.IndexOf<Settlement>(this._caravanTargetSettlements, settlement) + 1;
				SetPartyAiAction.GetActionForVisitingSettlement(this._questCaravanMobileParty, this._caravanTargetSettlements[num]);
				if (base.IsTracked(settlement))
				{
					base.RemoveTrackedObject(settlement);
				}
				base.AddTrackedObject(this._caravanTargetSettlements[num]);
			}
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000C60A File Offset: 0x0000A80A
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (this._questCaravanMobileParty != null && this._questCaravanMobileParty == party)
			{
				this.AddLogForSettlementVisit(settlement);
			}
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000C624 File Offset: 0x0000A824
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent && this._questCaravanMobileParty != null && mapEvent.InvolvedParties.Contains(this._questCaravanMobileParty.Party))
			{
				if (mapEvent.WinningSide == mapEvent.PlayerSide)
				{
					if (this._questCaravanMobileParty.Party.NumberOfHealthyMembers > 0)
					{
						DestroyPartyAction.Apply(null, this._questCaravanMobileParty);
					}
					this.BattleWon();
					return;
				}
				if (mapEvent.WinningSide != -1)
				{
					DestroyPartyAction.Apply(null, this._questCaravanMobileParty);
					this.BattleLost();
				}
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000C6A8 File Offset: 0x0000A8A8
		private void DailyTick()
		{
			if (this._questCaravanMobileParty == null && this._questStartTime.ElapsedDaysUntilNow >= 5f)
			{
				this.CreateQuestCaravanParty();
				this.SetDialogs();
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000C6E0 File Offset: 0x0000A8E0
		private void AddLogForSettlementVisit(Settlement settlement)
		{
			TextObject textObject = new TextObject("{=SVcr0EJM}Caravan is moving on to {TO_SETTLEMENT_LINK} from {FROM_SETTLEMENT_LINK}.", null);
			int num = Array.IndexOf<Settlement>(this._caravanTargetSettlements, settlement) + 1;
			textObject.SetTextVariable("FROM_SETTLEMENT_LINK", settlement.EncyclopediaLinkWithName);
			textObject.SetTextVariable("TO_SETTLEMENT_LINK", this._caravanTargetSettlements[num].EncyclopediaLinkWithName);
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ConspiracyQuestMapNotification(this, textObject));
			base.AddLog(textObject, false);
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000C754 File Offset: 0x0000A954
		private void CreateQuestCaravanParty()
		{
			PartyTemplateObject partyTemplateObject = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("conspiracy_anti_imperial_special_raider_party_template") : Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("conspiracy_imperial_special_raider_party_template"));
			Hero hero = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? StoryModeHeroes.AntiImperialMentor : StoryModeHeroes.ImperialMentor);
			string text;
			string text2;
			this.GetAdditionalVisualsForParty(this.QuestFromSettlement.Culture, out text, out text2);
			string[] array = new string[] { "aserai", "battania", "khuzait", "sturgia", "vlandia" };
			Clan clan = CampaignData.NeutralFaction;
			foreach (Clan clan2 in Clan.All)
			{
				if (!clan2.IsEliminated && !clan2.IsNeutralClan && !clan2.IsBanditFaction && !clan2.IsMinorFaction && ((StoryModeManager.Current.MainStoryLine.IsOnAntiImperialQuestLine && clan2.Culture.StringId == "empire") || (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine && array.Contains(clan2.Culture.StringId))))
				{
					clan = clan2;
					break;
				}
			}
			this._questCaravanMobileParty = CustomPartyComponent.CreateQuestParty(this.QuestFromSettlement.GatePosition, 0f, this.QuestFromSettlement, new TextObject("{=eVzg5Mtl}Conspiracy Caravan", null), clan, partyTemplateObject, hero, 0, text, text2, 4f, true);
			this._questCaravanMobileParty.Aggressiveness = 0f;
			this._questCaravanMobileParty.MemberRoster.Clear();
			this._questCaravanMobileParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("fish"), 20);
			this._questCaravanMobileParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("grain"), 40);
			this._questCaravanMobileParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("butter"), 20);
			base.DistributeConspiracyRaiderTroopsByLevel(partyTemplateObject, this._questCaravanMobileParty.Party, this.CaravanPartySize);
			this._questCaravanMobileParty.IgnoreByOtherPartiesTill(base.QuestDueTime);
			this._questCaravanMobileParty.SetPartyUsedByQuest(true);
			SetPartyAiAction.GetActionForVisitingSettlement(this._questCaravanMobileParty, this._caravanTargetSettlements[1]);
			this._questCaravanMobileParty.Ai.SetDoNotMakeNewDecisions(true);
			base.AddTrackedObject(this._questCaravanMobileParty);
			this._questCaravanMobileParty.IgnoreByOtherPartiesTill(CampaignTime.WeeksFromNow(3f));
			this.AddLogForSettlementVisit(this.QuestFromSettlement);
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000CA0C File Offset: 0x0000AC0C
		private void GetAdditionalVisualsForParty(CultureObject culture, out string mountStringId, out string harnessStringId)
		{
			if (culture.StringId == "aserai" || culture.StringId == "khuzait")
			{
				mountStringId = "camel";
				harnessStringId = ((MBRandom.RandomFloat > 0.5f) ? "camel_saddle_a" : "camel_saddle_b");
				return;
			}
			mountStringId = "mule";
			harnessStringId = ((MBRandom.RandomFloat > 0.5f) ? "mule_load_a" : ((MBRandom.RandomFloat > 0.5f) ? "mule_load_b" : "mule_load_c"));
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000CA94 File Offset: 0x0000AC94
		private float GetQuestDifficultyMultiplier()
		{
			return MBMath.ClampFloat((0f + (float)Clan.PlayerClan.Fiefs.Count * 0.1f + Clan.PlayerClan.TotalStrength * 0.0008f + Clan.PlayerClan.Renown * 1.5E-05f + (float)Clan.PlayerClan.Lords.Count * 0.002f + (float)Clan.PlayerClan.Companions.Count * 0.01f + (float)Clan.PlayerClan.SupporterNotables.Count * 0.001f + (float)Hero.MainHero.OwnedCaravans.Count * 0.01f + (float)PartyBase.MainParty.NumberOfAllMembers * 0.002f + (float)CharacterObject.PlayerCharacter.Level * 0.002f) * 0.975f + MBRandom.RandomFloat * 0.025f, 0.1f, 1f);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000CB80 File Offset: 0x0000AD80
		private void BattleWon()
		{
			base.AddLog(this.PlayerDefeatedCaravanLog, false);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000CB96 File Offset: 0x0000AD96
		private void BattleLost()
		{
			base.AddLog(this.MainHeroLostCombat, false);
			base.CompleteQuestWithFail(null);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000CBAD File Offset: 0x0000ADAD
		private void FailedToDisrupt()
		{
			base.AddLog(this.MainHeroFailedToDisrupt, false);
			base.CompleteQuestWithFail(null);
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000CBC4 File Offset: 0x0000ADC4
		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=ch9f3A1e}Greetings, {?PLAYER.GENDER}madam{?}sir{\\?}. Why did you stop our caravan? I trust you are not robbing us.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.conversation_with_caravan_master_condition))
				.BeginPlayerOptions()
				.PlayerOption(new TextObject("{=Xx94UrYe}I might be. What are you carrying? Honest goods, or weapons? How about you let us have a look.", null), null)
				.NpcLine(new TextObject("{=LXGXxKqw}Ah... Well, I suppose we can drop the charade. I know who sent you, and I suppose you know who sent me. Certainly, you can see my wares, and then you can feel their sharp end in your belly.", null), null, null)
				.CloseDialog()
				.PlayerOption(new TextObject("{=cEaXehHy}I was just checking on something. You can move along.", null), null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.cancel_encounter_consequence))
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000CC71 File Offset: 0x0000AE71
		private bool conversation_with_caravan_master_condition()
		{
			return this._questCaravanMobileParty != null && ConversationHelper.GetConversationCharacterPartyLeader(this._questCaravanMobileParty.Party) == CharacterObject.OneToOneConversationCharacter;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000CC94 File Offset: 0x0000AE94
		private void cancel_encounter_consequence()
		{
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000CCA3 File Offset: 0x0000AEA3
		internal static void AutoGeneratedStaticCollectObjectsDisruptSupplyLinesConspiracyQuest(object o, List<object> collectedObjects)
		{
			((DisruptSupplyLinesConspiracyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000CCB1 File Offset: 0x0000AEB1
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._caravanTargetSettlements);
			collectedObjects.Add(this._questCaravanMobileParty);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this._questStartTime, collectedObjects);
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000CCE3 File Offset: 0x0000AEE3
		internal static object AutoGeneratedGetMemberValue_caravanTargetSettlements(object o)
		{
			return ((DisruptSupplyLinesConspiracyQuest)o)._caravanTargetSettlements;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000CCF0 File Offset: 0x0000AEF0
		internal static object AutoGeneratedGetMemberValue_questCaravanMobileParty(object o)
		{
			return ((DisruptSupplyLinesConspiracyQuest)o)._questCaravanMobileParty;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000CCFD File Offset: 0x0000AEFD
		internal static object AutoGeneratedGetMemberValue_questStartTime(object o)
		{
			return ((DisruptSupplyLinesConspiracyQuest)o)._questStartTime;
		}

		// Token: 0x040000AD RID: 173
		private const int NumberOfSettlementsToVisit = 6;

		// Token: 0x040000AE RID: 174
		private const int SpawnCaravanWaitDaysAfterQuestStarted = 5;

		// Token: 0x040000AF RID: 175
		[SaveableField(1)]
		private readonly Settlement[] _caravanTargetSettlements;

		// Token: 0x040000B0 RID: 176
		[SaveableField(2)]
		private MobileParty _questCaravanMobileParty;

		// Token: 0x040000B1 RID: 177
		[SaveableField(3)]
		private readonly CampaignTime _questStartTime;
	}
}
