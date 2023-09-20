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
	internal class DisruptSupplyLinesConspiracyQuest : ConspiracyQuestBase
	{
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=y150haHv}Disrupt Supply Lines", null);
			}
		}

		public override TextObject SideNotificationText
		{
			get
			{
				TextObject textObject = new TextObject("{=IPP6MKfy}{MENTOR.LINK} notified you about a weapons caravan that will supply conspirators with weapons and armour.", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

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

		public override float ConspiracyStrengthDecreaseAmount
		{
			get
			{
				return 75f;
			}
		}

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

		private TextObject MainHeroFailedToDisrupt
		{
			get
			{
				return new TextObject("{=9aRqqx3U}The caravan has delivered its supplies to the conspirators. A stronger adversary awaits us...", null);
			}
		}

		private TextObject MainHeroLostCombat
		{
			get
			{
				return new TextObject("{=bT9yspaQ}You have lost the battle against the conspiracy's caravan. A stronger adversary awaits us...", null);
			}
		}

		private Settlement QuestFromSettlement
		{
			get
			{
				return this._caravanTargetSettlements[0];
			}
		}

		private Settlement QuestToSettlement
		{
			get
			{
				return this._caravanTargetSettlements[this._caravanTargetSettlements.Length - 1];
			}
		}

		public MobileParty ConspiracyCaravan
		{
			get
			{
				return this._questCaravanMobileParty;
			}
		}

		public int CaravanPartySize
		{
			get
			{
				return 70 + 70 * (int)this.GetQuestDifficultyMultiplier();
			}
		}

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

		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		protected override void OnTimedOut()
		{
			MobileParty questCaravanMobileParty = this._questCaravanMobileParty;
			if (questCaravanMobileParty != null && questCaravanMobileParty.IsActive)
			{
				DestroyPartyAction.Apply(null, this._questCaravanMobileParty);
			}
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
		}

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

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (this._questCaravanMobileParty != null && this._questCaravanMobileParty == party)
			{
				this.AddLogForSettlementVisit(settlement);
			}
		}

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

		private void DailyTick()
		{
			if (this._questCaravanMobileParty == null && this._questStartTime.ElapsedDaysUntilNow >= 5f)
			{
				this.CreateQuestCaravanParty();
				this.SetDialogs();
			}
		}

		private void AddLogForSettlementVisit(Settlement settlement)
		{
			TextObject textObject = new TextObject("{=SVcr0EJM}Caravan is moving on to {TO_SETTLEMENT_LINK} from {FROM_SETTLEMENT_LINK}.", null);
			int num = Array.IndexOf<Settlement>(this._caravanTargetSettlements, settlement) + 1;
			textObject.SetTextVariable("FROM_SETTLEMENT_LINK", settlement.EncyclopediaLinkWithName);
			textObject.SetTextVariable("TO_SETTLEMENT_LINK", this._caravanTargetSettlements[num].EncyclopediaLinkWithName);
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ConspiracyQuestMapNotification(this, textObject));
			base.AddLog(textObject, false);
		}

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

		private float GetQuestDifficultyMultiplier()
		{
			return MBMath.ClampFloat((0f + (float)Clan.PlayerClan.Fiefs.Count * 0.1f + Clan.PlayerClan.TotalStrength * 0.0008f + Clan.PlayerClan.Renown * 1.5E-05f + (float)Clan.PlayerClan.Lords.Count * 0.002f + (float)Clan.PlayerClan.Companions.Count * 0.01f + (float)Clan.PlayerClan.SupporterNotables.Count * 0.001f + (float)Hero.MainHero.OwnedCaravans.Count * 0.01f + (float)PartyBase.MainParty.NumberOfAllMembers * 0.002f + (float)CharacterObject.PlayerCharacter.Level * 0.002f) * 0.975f + MBRandom.RandomFloat * 0.025f, 0.1f, 1f);
		}

		private void BattleWon()
		{
			base.AddLog(this.PlayerDefeatedCaravanLog, false);
			base.CompleteQuestWithSuccess();
		}

		private void BattleLost()
		{
			base.AddLog(this.MainHeroLostCombat, false);
			base.CompleteQuestWithFail(null);
		}

		private void FailedToDisrupt()
		{
			base.AddLog(this.MainHeroFailedToDisrupt, false);
			base.CompleteQuestWithFail(null);
		}

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

		private bool conversation_with_caravan_master_condition()
		{
			return this._questCaravanMobileParty != null && ConversationHelper.GetConversationCharacterPartyLeader(this._questCaravanMobileParty.Party) == CharacterObject.OneToOneConversationCharacter;
		}

		private void cancel_encounter_consequence()
		{
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		internal static void AutoGeneratedStaticCollectObjectsDisruptSupplyLinesConspiracyQuest(object o, List<object> collectedObjects)
		{
			((DisruptSupplyLinesConspiracyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._caravanTargetSettlements);
			collectedObjects.Add(this._questCaravanMobileParty);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this._questStartTime, collectedObjects);
		}

		internal static object AutoGeneratedGetMemberValue_caravanTargetSettlements(object o)
		{
			return ((DisruptSupplyLinesConspiracyQuest)o)._caravanTargetSettlements;
		}

		internal static object AutoGeneratedGetMemberValue_questCaravanMobileParty(object o)
		{
			return ((DisruptSupplyLinesConspiracyQuest)o)._questCaravanMobileParty;
		}

		internal static object AutoGeneratedGetMemberValue_questStartTime(object o)
		{
			return ((DisruptSupplyLinesConspiracyQuest)o)._questStartTime;
		}

		private const int NumberOfSettlementsToVisit = 6;

		private const int SpawnCaravanWaitDaysAfterQuestStarted = 5;

		[SaveableField(1)]
		private readonly Settlement[] _caravanTargetSettlements;

		[SaveableField(2)]
		private MobileParty _questCaravanMobileParty;

		[SaveableField(3)]
		private readonly CampaignTime _questStartTime;
	}
}
