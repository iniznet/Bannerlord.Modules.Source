using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.SecondPhase.ConspiracyQuests
{
	internal class ConspiracyBaseOfOperationsDiscoveredConspiracyQuest : ConspiracyQuestBase
	{
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=3Pq58i2u}Conspiracy base of operations discovered", null);
			}
		}

		public override TextObject SideNotificationText
		{
			get
			{
				TextObject textObject = new TextObject("{=aY4zWYpg}You have have received an important message from {MENTOR.LINK}.", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		public override TextObject StartMessageLogFromMentor
		{
			get
			{
				TextObject textObject = new TextObject("{=XQrmVPKL}{PLAYER.LINK} I hope this letter finds you well. I have learned from a spy in {LOCATION_LINK} that our adversaries have set up a camp in its environs. She could not tell me what they plan to do, but if you raided the camp, stole some of their supplies, and brought it back to me, we could get some idea of their wicked intentions. Search around {LOCATION_LINK} to find the hideout.", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
				textObject.SetTextVariable("LOCATION_LINK", this._baseLocation.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		public override TextObject StartLog
		{
			get
			{
				TextObject textObject = new TextObject("{=rTYNL1LB}{MENTOR.LINK} told you about a group of conspirators operating in a hideout in the vicinity of {LOCATION_LINK}. You should go there and raid the hideout with a small group of fighters and take the bandits by surprise.", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("LOCATION_LINK", this._baseLocation.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		public override float ConspiracyStrengthDecreaseAmount
		{
			get
			{
				return this._conspiracyStrengthDecreaseAmount;
			}
		}

		private TextObject HideoutBossName
		{
			get
			{
				MobileParty mobileParty = this._hideout.Parties.FirstOrDefault((MobileParty p) => p.IsBanditBossParty);
				if (mobileParty != null && mobileParty.MemberRoster.TotalManCount > 0)
				{
					return mobileParty.MemberRoster.GetCharacterAtIndex(0).Name;
				}
				return new TextObject("{=izCbZEZg}Conspiracy Commander{%Commander is male.}", null);
			}
		}

		private TextObject HideoutSpottedLog
		{
			get
			{
				return new TextObject("{=nrdl5QaF}My spy spotted some conspirators at the camp, and some local bandits have joined them. My spy does not know if they are expecting an attack, so I implore you to be cautious and to be ready for anything. Needless to say, I'm sure you will send any documents you can find to me so I can study them. Go quickly and return safely.", null);
			}
		}

		private TextObject HideoutRemovedLog
		{
			get
			{
				return new TextObject("{=cLZWjrZP}They have moved to another hiding place.", null);
			}
		}

		private TextObject NotDueledWithHideoutBossAndDefeatLog
		{
			get
			{
				TextObject textObject = new TextObject("{=nOLFHL3x}You and your men have defeated {BOSS_NAME} and the rest of the conspirators as {MENTOR.LINK} asked you to do.", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("BOSS_NAME", this.HideoutBossName);
				return textObject;
			}
		}

		private TextObject NotDueledWithHideoutBossAndDefeatedLog
		{
			get
			{
				TextObject textObject = new TextObject("{=EV5ykPuT}You and your men were defeated by {BOSS_NAME} and his conspirators. Rest of your men finds your broken body among the bloodied pile of corpses. Yet you live to fight another day.", null);
				textObject.SetTextVariable("BOSS_NAME", this.HideoutBossName);
				return textObject;
			}
		}

		private TextObject DueledWithHideoutBossAndDefeatLog
		{
			get
			{
				TextObject textObject = new TextObject("{=LKiREaFZ}You have defeated {BOSS_NAME} in a fair duel his men the conspirators scatters and runs away in shame.", null);
				textObject.SetTextVariable("BOSS_NAME", this.HideoutBossName);
				return textObject;
			}
		}

		private TextObject DueledWithHideoutBossAndDefeatedLog
		{
			get
			{
				TextObject textObject = new TextObject("{=Uk7F483P}You were defeated by the {BOSS_NAME} in the duel. Your men takes your wounded body to the safety. As agreed, conspirators quickly leave and disappear without a trace.", null);
				textObject.SetTextVariable("BOSS_NAME", this.HideoutBossName);
				return textObject;
			}
		}

		public ConspiracyBaseOfOperationsDiscoveredConspiracyQuest(string questId, Hero questGiver)
			: base(questId, questGiver)
		{
			this._raiderParties = new List<MobileParty>();
			this._hideout = this.SelectHideout();
			if (this._hideout.Hideout.IsSpotted)
			{
				base.AddLog(this.HideoutSpottedLog, false);
				base.AddTrackedObject(this._hideout);
			}
			this._baseLocation = SettlementHelper.FindNearestSettlement((Settlement p) => p.IsFortification, this._hideout);
			this._conspiracyStrengthDecreaseAmount = 0f;
			this.InitializeHideout();
			this._isDone = false;
		}

		private Settlement SelectHideout()
		{
			Settlement settlement = SettlementHelper.FindRandomHideout(delegate(Settlement s)
			{
				if (!s.Hideout.IsInfested)
				{
					return false;
				}
				if (!StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine)
				{
					return StoryModeData.IsKingdomImperial(SettlementHelper.FindNearestFortification(null, s).OwnerClan.Kingdom);
				}
				return !StoryModeData.IsKingdomImperial(SettlementHelper.FindNearestFortification(null, s).OwnerClan.Kingdom);
			});
			if (settlement == null)
			{
				settlement = SettlementHelper.FindRandomHideout(delegate(Settlement s)
				{
					if (!StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine)
					{
						return StoryModeData.IsKingdomImperial(SettlementHelper.FindNearestFortification(null, s).OwnerClan.Kingdom);
					}
					return !StoryModeData.IsKingdomImperial(SettlementHelper.FindNearestFortification(null, s).OwnerClan.Kingdom);
				});
				if (settlement == null)
				{
					settlement = SettlementHelper.FindRandomHideout((Settlement s) => s.Hideout.IsInfested);
					if (settlement == null)
					{
						settlement = SettlementHelper.FindRandomHideout(null);
					}
				}
			}
			if (!settlement.Hideout.IsInfested)
			{
				for (int i = 0; i < 2; i++)
				{
					if (!settlement.Hideout.IsInfested)
					{
						this._raiderParties.Add(this.CreateRaiderParty(settlement, false, i));
					}
				}
			}
			return settlement;
		}

		private MobileParty CreateRaiderParty(Settlement hideout, bool isBanditBossParty, int partyIndex)
		{
			MobileParty mobileParty = BanditPartyComponent.CreateBanditParty("conspiracy_discovered_quest_raider_party_" + partyIndex, hideout.OwnerClan, hideout.Hideout, isBanditBossParty);
			TroopRoster troopRoster = new TroopRoster(mobileParty.Party);
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>(hideout.Culture.StringId + "_bandit");
			troopRoster.AddToCounts(@object, 6 - troopRoster.TotalManCount, false, 0, 0, true, -1);
			TroopRoster troopRoster2 = new TroopRoster(mobileParty.Party);
			mobileParty.InitializeMobilePartyAtPosition(troopRoster, troopRoster2, hideout.Position2D);
			mobileParty.SetCustomName(new TextObject("{=u1Pkt4HC}Raiders", null));
			mobileParty.ActualClan = hideout.OwnerClan;
			mobileParty.Position2D = hideout.Position2D;
			mobileParty.Party.Visuals.SetMapIconAsDirty();
			EnterSettlementAction.ApplyForParty(mobileParty, hideout);
			float totalStrength = mobileParty.Party.TotalStrength;
			int num = (int)(1f * MBRandom.RandomFloat * 20f * totalStrength + 50f);
			mobileParty.InitializePartyTrade(num);
			mobileParty.Ai.SetMoveGoToSettlement(hideout);
			EnterSettlementAction.ApplyForParty(mobileParty, hideout);
			mobileParty.SetPartyUsedByQuest(true);
			return mobileParty;
		}

		protected override void InitializeQuestOnGameLoad()
		{
			this._baseLocation = SettlementHelper.FindNearestFortification(null, this._hideout);
			this.SetDialogs();
		}

		private void InitializeHideout()
		{
			base.AddTrackedObject(this._baseLocation);
		}

		private void ChangeHideoutParties()
		{
			PartyTemplateObject partyTemplateObject = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("conspiracy_anti_imperial_special_raider_party_template") : Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("conspiracy_imperial_special_raider_party_template"));
			foreach (MobileParty mobileParty in this._hideout.Parties)
			{
				if (mobileParty.IsBandit)
				{
					mobileParty.SetCustomName(new TextObject("{=FRSas4xT}Conspiracy Troops", null));
					mobileParty.SetPartyUsedByQuest(true);
					if (mobileParty.IsBanditBossParty)
					{
						mobileParty.MemberRoster.GetTroopRoster().First((TroopRosterElement t) => t.Character == this._hideout.Culture.BanditBoss);
						int num = mobileParty.MemberRoster.TotalManCount - 1;
						mobileParty.MemberRoster.Clear();
						base.DistributeConspiracyRaiderTroopsByLevel(partyTemplateObject, mobileParty.Party, num);
						CharacterObject characterObject = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? Campaign.Current.ObjectManager.GetObject<CharacterObject>("anti_imperial_conspiracy_boss") : Campaign.Current.ObjectManager.GetObject<CharacterObject>("imperial_conspiracy_boss"));
						characterObject.SetTransferableInPartyScreen(false);
						mobileParty.MemberRoster.AddToCounts(characterObject, 1, true, 0, 0, true, -1);
					}
					else
					{
						int totalManCount = mobileParty.MemberRoster.TotalManCount;
						mobileParty.MemberRoster.Clear();
						base.DistributeConspiracyRaiderTroopsByLevel(partyTemplateObject, mobileParty.Party, totalManCount);
					}
				}
			}
		}

		protected override void RegisterEvents()
		{
			base.RegisterEvents();
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
			CampaignEvents.OnHideoutSpottedEvent.AddNonSerializedListener(this, new Action<PartyBase, PartyBase>(this.OnHideoutSpotted));
		}

		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement == this._hideout && !this._isDone)
			{
				MobileParty mobileParty = this._hideout.Parties.FirstOrDefault((MobileParty p) => p.IsBanditBossParty);
				if (mobileParty != null && mobileParty.IsActive && (mobileParty.MemberRoster.TotalManCount <= 0 || (mobileParty.MemberRoster.GetCharacterAtIndex(0) != null && mobileParty.MemberRoster.GetCharacterAtIndex(0).StringId != (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? "anti_imperial_conspiracy_boss" : "imperial_conspiracy_boss"))))
				{
					this.ChangeHideoutParties();
				}
			}
			if (this._isDone)
			{
				if (Hero.MainHero.IsPrisoner)
				{
					EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
				}
				foreach (MobileParty mobileParty2 in this._hideout.Parties.ToList<MobileParty>())
				{
					if (mobileParty2.IsBandit)
					{
						DestroyPartyAction.Apply(null, mobileParty2);
					}
				}
				if (this._isSuccess)
				{
					base.CompleteQuestWithSuccess();
					return;
				}
				base.AddLog(this.HideoutRemovedLog, false);
				base.CompleteQuestWithFail(null);
			}
		}

		private void OnMissionEnded(IMission mission)
		{
			if (Settlement.CurrentSettlement == this._hideout && PlayerEncounter.Current != null)
			{
				MapEvent playerMapEvent = MapEvent.PlayerMapEvent;
				if (playerMapEvent != null)
				{
					if (playerMapEvent.WinningSide == playerMapEvent.PlayerSide)
					{
						if (this._dueledWithHideoutBoss)
						{
							this.DueledWithHideoutBossAndDefeatedCaravan();
						}
						else
						{
							this.NotDueledWithHideoutBossAndDefeatedCaravan();
						}
						this._isSuccess = true;
					}
					else
					{
						if (playerMapEvent.WinningSide != -1)
						{
							if (this._dueledWithHideoutBoss)
							{
								this.DueledWithHideoutBossAndDefeatedByCaravan();
							}
							else
							{
								this.NotDueledWithHideoutBossAndDefeatedByCaravan();
							}
						}
						this._isSuccess = false;
					}
					this._isDone = true;
				}
			}
		}

		private void OnHideoutSpotted(PartyBase party, PartyBase hideoutParty)
		{
			if (party == PartyBase.MainParty && hideoutParty.Settlement == this._hideout)
			{
				base.AddLog(this.HideoutSpottedLog, false);
				base.AddTrackedObject(this._hideout);
			}
		}

		private void NotDueledWithHideoutBossAndDefeatedCaravan()
		{
			base.AddLog(this.NotDueledWithHideoutBossAndDefeatLog, false);
			this._conspiracyStrengthDecreaseAmount = 50f;
		}

		private void NotDueledWithHideoutBossAndDefeatedByCaravan()
		{
			base.AddLog(this.NotDueledWithHideoutBossAndDefeatedLog, false);
		}

		private void DueledWithHideoutBossAndDefeatedCaravan()
		{
			base.AddLog(this.DueledWithHideoutBossAndDefeatLog, false);
			this._conspiracyStrengthDecreaseAmount = 75f;
		}

		private void DueledWithHideoutBossAndDefeatedByCaravan()
		{
			base.AddLog(this.DueledWithHideoutBossAndDefeatedLog, false);
		}

		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=UdHL9YZC}Well well, isn't this the famous {PLAYER.LINK}! You have been a thorn at our side for a while now. It's good that you are here now. It spares us from searching for you.[if:idle_angry][ib:warrior]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.bandit_hideout_boss_fight_start_on_condition))
				.BeginPlayerOptions()
				.PlayerOption(new TextObject("{=bZI82WMt}Let's get this over with! Men Attack!", null), null)
				.NpcLine(new TextObject("{=H2FMIJmw}My wolves! Kill them!", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_continue_battle_on_consequence))
				.CloseDialog()
				.PlayerOption(new TextObject("{=5PGokzW1}Talk is cheap. If you really want me that bad, I challenge you to a duel.", null), null)
				.NpcLine(new TextObject("{=karjORwI}To hell with that! Why would I want to duel with you?", null), null, null)
				.PlayerLine(new TextObject("{=MU2O1SaZ}There is an army waiting for you outside,", null), null)
				.PlayerLine(new TextObject("{=tF6VeYaA}If you win, I promise my army won't crush you.", null), null)
				.PlayerLine(new TextObject("{=fUcwKbW8}If I win I will just kill you and let these poor excuses you call conspirators run away.", null), null)
				.NpcLine(new TextObject("{=C0xbbPqE}I will duel you for your insolence! Die dog!", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_start_duel_fight_on_consequence))
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
		}

		private bool bandit_hideout_boss_fight_start_on_condition()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
			return encounteredParty != null && !encounteredParty.IsMobile && encounteredParty.MapFaction.IsBanditFaction && CharacterObject.OneToOneConversationCharacter.StringId == (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? "anti_imperial_conspiracy_boss" : "imperial_conspiracy_boss") && encounteredParty.MapFaction.IsBanditFaction && encounteredParty.IsSettlement && encounteredParty.Settlement == this._hideout && Mission.Current != null && Mission.Current.GetMissionBehavior<HideoutMissionController>() != null && encounteredParty.Settlement.IsHideout;
		}

		private void bandit_hideout_start_duel_fight_on_consequence()
		{
			this._dueledWithHideoutBoss = true;
			Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightDuelMode;
		}

		private void bandit_hideout_continue_battle_on_consequence()
		{
			this._dueledWithHideoutBoss = false;
			Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightBattleMode;
		}

		protected override void OnStartQuest()
		{
			base.OnStartQuest();
			this.SetDialogs();
		}

		protected override void OnCompleteWithSuccess()
		{
			base.OnCompleteWithSuccess();
			base.AddLog(new TextObject("{=6Dd3Pa07}You managed to thwart the conspiracy.", null), false);
			foreach (MobileParty mobileParty in this._raiderParties)
			{
				if (mobileParty.IsActive)
				{
					DestroyPartyAction.Apply(null, mobileParty);
				}
			}
			this._raiderParties.Clear();
		}

		protected override void OnTimedOut()
		{
			base.OnTimedOut();
			base.AddLog(new TextObject("{=S5Dn2K3m}You couldn't stop the conspiracy.", null), false);
		}

		internal static void AutoGeneratedStaticCollectObjectsConspiracyBaseOfOperationsDiscoveredConspiracyQuest(object o, List<object> collectedObjects)
		{
			((ConspiracyBaseOfOperationsDiscoveredConspiracyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._hideout);
			collectedObjects.Add(this._raiderParties);
		}

		internal static object AutoGeneratedGetMemberValue_hideout(object o)
		{
			return ((ConspiracyBaseOfOperationsDiscoveredConspiracyQuest)o)._hideout;
		}

		internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
		{
			return ((ConspiracyBaseOfOperationsDiscoveredConspiracyQuest)o)._raiderParties;
		}

		private const string AntiImperialHideoutBossStringId = "anti_imperial_conspiracy_boss";

		private const string ImperialHideoutBossStringId = "imperial_conspiracy_boss";

		private const int RaiderPartySize = 6;

		private const int RaiderPartyCount = 2;

		[SaveableField(1)]
		private readonly Settlement _hideout;

		private Settlement _baseLocation;

		private bool _dueledWithHideoutBoss;

		private bool _isSuccess;

		private bool _isDone;

		private float _conspiracyStrengthDecreaseAmount;

		[SaveableField(2)]
		private readonly List<MobileParty> _raiderParties;
	}
}
