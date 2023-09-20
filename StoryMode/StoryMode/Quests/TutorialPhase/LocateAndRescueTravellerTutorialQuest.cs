using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
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
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.TutorialPhase
{
	public class LocateAndRescueTravellerTutorialQuest : StoryModeQuestBase
	{
		private TextObject _startQuestLogText
		{
			get
			{
				return new TextObject("{=JJo0i8an}Look around the village to find the party that captured the traveller whom the headman told you about.", null);
			}
		}

		public override TextObject Title
		{
			get
			{
				return new TextObject("{=ACyYhA2s}Locate and Rescue Traveller", null);
			}
		}

		public LocateAndRescueTravellerTutorialQuest(Hero questGiver)
			: base("locate_and_rescue_traveler_tutorial_quest", questGiver, CampaignTime.Never)
		{
			this._raiderParties = new List<MobileParty>();
			this._defeatedRaiderPartyCount = 0;
			this.SetDialogs();
			this.AddGameMenus();
			base.InitializeQuestOnCreation();
			this._raiderPartyCount = 0;
			this._startQuestLog = base.AddDiscreteLog(this._startQuestLogText, new TextObject("{=UkNUuyr1}Defeated Parties", null), this._defeatedRaiderPartyCount, 3, null, false);
			if (MobileParty.MainParty.MemberRoster.TotalManCount >= 4)
			{
				this.SpawnRaiderParties();
			}
			TutorialPhase.Instance.SetTutorialFocusSettlement(Settlement.Find("village_ES3_2"));
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
		}

		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
			this.AddGameMenus();
		}

		private MobileParty CreateRaiderParty()
		{
			Settlement settlement = SettlementHelper.FindNearestHideout((Settlement x) => x.IsActive, null);
			MobileParty mobileParty = BanditPartyComponent.CreateBanditParty("locate_and_rescue_traveller_quest_raider_party_" + this._raiderPartyCount, settlement.OwnerClan, settlement.Hideout, false);
			TroopRoster troopRoster = new TroopRoster(mobileParty.Party);
			TroopRoster troopRoster2 = new TroopRoster(mobileParty.Party);
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("storymode_quest_raider");
			troopRoster.AddToCounts(@object, 6, false, 0, 0, true, -1);
			CharacterObject object2 = MBObjectManager.Instance.GetObject<CharacterObject>("tutorial_placeholder_volunteer");
			troopRoster2.AddToCounts(object2, (MBRandom.RandomFloat >= 0.5f) ? 1 : 2, false, 0, 0, true, -1);
			Settlement object3 = MBObjectManager.Instance.GetObject<Settlement>("village_ES3_2");
			mobileParty.InitializeMobilePartyAroundPosition(troopRoster, troopRoster2, object3.GatePosition, MobileParty.MainParty.SeeingRange * 0.75f, 0f);
			mobileParty.SetCustomName(new TextObject("{=u1Pkt4HC}Raiders", null));
			mobileParty.InitializePartyTrade(200);
			mobileParty.ActualClan = settlement.OwnerClan;
			SetPartyAiAction.GetActionForPatrollingAroundSettlement(mobileParty, object3);
			mobileParty.Ai.SetDoNotMakeNewDecisions(true);
			mobileParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
			mobileParty.Party.SetVisualAsDirty();
			base.AddTrackedObject(mobileParty);
			mobileParty.IsActive = true;
			this._raiderPartyCount++;
			mobileParty.SetPartyUsedByQuest(true);
			return mobileParty;
		}

		private void DespawnRaiderParties()
		{
			if (Extensions.IsEmpty<MobileParty>(this._raiderParties))
			{
				return;
			}
			foreach (MobileParty mobileParty in this._raiderParties.ToList<MobileParty>())
			{
				base.RemoveTrackedObject(mobileParty);
				DestroyPartyAction.Apply(null, mobileParty);
			}
			this._raiderParties.Clear();
		}

		private void SpawnRaiderParties()
		{
			if (!Extensions.IsEmpty<MobileParty>(this._raiderParties))
			{
				return;
			}
			for (int i = this._defeatedRaiderPartyCount; i < 3; i++)
			{
				this._raiderParties.Add(this.CreateRaiderParty());
			}
		}

		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("{=BdYaRvhm}I don't know who you are, but I'm in your debt. These brigands would've marched us to our deaths.[ib:nervous2][if:convo_uncomfortable_voice]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.meeting_tacitus_on_condition))
				.NpcLine(new TextObject("{=9VxUSDQ7}My name's Tacteos. I'm a doctor by trade. I was on, well, a bit of a quest, but now I'm thinking I'm not really made for this kind of thing.[ib:nervous][if:convo_pondering]", null), null, null)
				.NpcLine(new TextObject("{=5LJTeOBT}I was with a caravan and they just came out of the brush. We were surrounded and outnumbered, so we gave up. I figured they'd keep us alive, if just for the ransom. But then they started flogging us along at top speed, without any water, and I was just about ready to drop.[ib:nervous2]", null), null, null)
				.NpcLine(new TextObject("{=XdDQdSsW}I could feel the signs of heat-stroke creeping up and I told them but they just flogged me more... If your group hadn't come along... Maybe I have a way to thank you properly.[ib:normal][if:convo_thinking]", null), null, null)
				.PlayerLine(new TextObject("{=bkZFbCRx}We're looking for two children captured by the raiders. Can you tell us anything?", null), null)
				.NpcLine(new TextObject("{=ehnbi5yD}I am afraid I haven't seen any children. But after our caravan was attacked, the chief of the raiders, the one they call Radagos, took and rode off with our more valuable belongings, including a chest that I had.[ib:closed][if:convo_empathic_voice]", null), null, null)
				.NpcLine(new TextObject("{=RF3NoR3d}He seemed to be controlling more than one band raiding around this area. If this lot has your kin, then I think he'd be the one to know.[if:convo_pondering]", null), null, null)
				.NpcLine(new TextObject("{=K75sH3vW}And since I have nothing of value left to repay your help, I'll tell you this. If you do catch up with and defeat that ruffian, you may be able to recover my chest. It contains a valuable ornament which I was told could be of great value, if you knew where to sell it.[if:convo_pondering]", null), null, null)
				.NpcLine(new TextObject("{=8GCW5IRO}I was trying to find out more about it, but, as I say, I've had all my urge for travelling flogged out of me. Right now I don't think I'd venture more than 20 paces from a well as long as I live.[ib:closed2][if:convo_shocked]", null), null, null)
				.PlayerLine(new TextObject("{=Zyn5FrTR}We'll keep that in mind.", null), null)
				.NpcLine(new TextObject("{=vJyTsFdU}It doesn't look like much and I suspect this lot would give it away for a few coins, but I got it from a mercenary whom I treated once, and swore it was related to 'Neretzes's Folly'. I don't know what that means, except that Neretzes was, of course, the emperor who died in battle some years back. Maybe you can find out its true value.[if:convo_calm_friendly]", null), null, null)
				.NpcLine(new TextObject("{=tsjQtWsO}Thanks for saving me again. I hope our paths will cross again![ib:normal2][if:convo_calm_friendly]", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.meeting_tacitus_on_consequence))
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("{=!}Start encounter.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.meeting_with_raider_party_on_condition))
				.CloseDialog(), this);
		}

		private bool meeting_tacitus_on_condition()
		{
			return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == StoryModeHeroes.Tacitus && !Hero.OneToOneConversationHero.HasMet;
		}

		private void meeting_tacitus_on_consequence()
		{
			foreach (MobileParty mobileParty in this._raiderParties)
			{
				if (mobileParty.IsActive)
				{
					DestroyPartyAction.Apply(null, mobileParty);
				}
			}
			DisableHeroAction.Apply(StoryModeHeroes.Tacitus);
			base.CompleteQuestWithSuccess();
		}

		private bool meeting_with_raider_party_on_condition()
		{
			return this._raiderParties.Any((MobileParty p) => ConversationHelper.GetConversationCharacterPartyLeader(p.Party) == CharacterObject.OneToOneConversationCharacter);
		}

		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement == null && PlayerEncounter.EncounteredMobileParty != null)
			{
				if (this._raiderParties.Any((MobileParty p) => p == PlayerEncounter.EncounteredMobileParty) && args.MenuContext.GameMenu.StringId != "encounter_meeting" && args.MenuContext.GameMenu.StringId != "encounter" && args.MenuContext.GameMenu.StringId != "encounter_raiders_quest")
				{
					GameMenu.SwitchToMenu("encounter_raiders_quest");
				}
			}
			if (Hero.MainHero.HitPoints < 50)
			{
				Hero.MainHero.Heal(50 - Hero.MainHero.HitPoints, false);
			}
			Hero elderBrother = StoryModeHeroes.ElderBrother;
			if (elderBrother.HitPoints < 50)
			{
				elderBrother.Heal(50 - elderBrother.HitPoints, false);
			}
			if (Hero.MainHero.IsPrisoner)
			{
				EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
				if (elderBrother.IsPrisoner)
				{
					EndCaptivityAction.ApplyByPeace(elderBrother, null);
				}
				if (elderBrother.PartyBelongedTo != MobileParty.MainParty)
				{
					MobileParty.MainParty.AddElementToMemberRoster(elderBrother.CharacterObject, 1, false);
				}
				DisableHeroAction.Apply(StoryModeHeroes.Tacitus);
				TextObject textObject = new TextObject("{=ORnjaMlM}You were defeated by the raiders, but your brother saved you. It doesn't look like they're going anywhere, though, so you should attack again once you're ready.\nYou must have at least {NUMBER} members in your party. If you don't, go back to the village and recruit some more troops.", null);
				textObject.SetTextVariable("NUMBER", 4);
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=FPhWhjq7}Defeated", null).ToString(), textObject.ToString(), true, false, new TextObject("{=lmG7uRK2}Okay", null).ToString(), null, delegate
				{
					PartyBase mainParty = PartyBase.MainParty;
					if (mainParty != null && mainParty.MemberRoster.TotalManCount >= 4)
					{
						this.SpawnRaiderParties();
						return;
					}
					Campaign campaign = Campaign.Current;
					if (campaign == null)
					{
						return;
					}
					campaign.VisualTrackerManager.RegisterObject(MBObjectManager.Instance.GetObject<Settlement>("village_ES3_2"));
				}, null, "", 0f, null, null, null), false, false);
				this.DespawnRaiderParties();
			}
		}

		private void AddGameMenus()
		{
			base.AddGameMenu("encounter_raiders_quest", new TextObject("{=mU1bC1mp}You encountered the raider party.", null), new OnInitDelegate(this.game_menu_encounter_on_init), 4, 0);
			base.AddGameMenuOption("encounter_raiders_quest", "encounter_raiders_quest_attack", new TextObject("{=1r0tDsrR}Attack!", null), new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_attack_on_consequence), false, -1);
			base.AddGameMenuOption("encounter_raiders_quest", "encounter_raiders_quest_send_troops", new TextObject("{=z3VamNrX}Send in your troops.", null), new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_send_troops_on_condition), null, false, -1);
			base.AddGameMenuOption("encounter_raiders_quest", "encounter_raiders_quest_leave", new TextObject("{=2YYRyrOO}Leave...", null), new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_leave_on_consequence), true, -1);
		}

		private void game_menu_encounter_on_init(MenuCallbackArgs args)
		{
			if (PlayerEncounter.Battle == null)
			{
				PlayerEncounter.StartBattle();
			}
			PlayerEncounter.Update();
		}

		private bool game_menu_encounter_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		private void game_menu_encounter_leave_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterLeaveConsequence();
		}

		private bool game_menu_encounter_attack_on_condition(MenuCallbackArgs args)
		{
			if (PartyBase.MainParty.MemberRoster.TotalManCount < 4)
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=DyE3luNM}You need to have at least {NUMBER} member in your party to deal with the raider party. Go back to village to recruit more troops.", null);
				args.Tooltip.SetTextVariable("NUMBER", 4);
			}
			return MenuHelper.EncounterAttackCondition(args);
		}

		internal void game_menu_encounter_attack_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterAttackConsequence(args);
		}

		private bool game_menu_encounter_send_troops_on_condition(MenuCallbackArgs args)
		{
			args.IsEnabled = false;
			args.Tooltip = new TextObject("{=hnFkhPhp}This option is disabled during tutorial stage.", null);
			args.optionLeaveType = 10;
			return true;
		}

		[GameMenuInitializationHandler("encounter_raiders_quest")]
		private static void game_menu_encounter_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_looter");
		}

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party == MobileParty.MainParty)
			{
				if (4 > MobileParty.MainParty.MemberRoster.TotalManCount)
				{
					this.DespawnRaiderParties();
					this.OpenRecruitMoreTroopsPopUp();
					return;
				}
				this.SpawnRaiderParties();
			}
		}

		private void OpenRecruitMoreTroopsPopUp()
		{
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=y3fn2vWY}Recruit Troops", null).ToString(), new TextObject("{=taOCFKtZ}You need to recruit more troops to deal with the raider party. Go back to village to recruit more troops.", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
		}

		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent)
			{
				if (mapEvent.PlayerSide == mapEvent.WinningSide)
				{
					using (List<MobileParty>.Enumerator enumerator = this._raiderParties.ToList<MobileParty>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MobileParty party = enumerator.Current;
							if (mapEvent.InvolvedParties.Any((PartyBase p) => p == party.Party))
							{
								this._defeatedRaiderPartyCount++;
								this._startQuestLog.UpdateCurrentProgress(this._defeatedRaiderPartyCount);
								party.MemberRoster.Clear();
								if (this._raiderParties.Count > 1)
								{
									this._raiderParties.Remove(party);
								}
							}
							if (party.MemberRoster.TotalManCount == 0 && this._raiderParties.Count > 1)
							{
								this._raiderParties.Remove(party);
							}
						}
					}
					if (this._defeatedRaiderPartyCount >= 3)
					{
						MobileParty mobileParty = this._raiderParties[0];
						Hero tacitus = StoryModeHeroes.Tacitus;
						TakePrisonerAction.Apply(mobileParty.Party, tacitus);
						mobileParty.PrisonRoster.AddToCounts(Campaign.Current.ObjectManager.GetObject<CharacterObject>("villager_empire"), 2, false, 0, 0, true, -1);
						InformationManager.ShowInquiry(new InquiryData(new TextObject("{=EWD4Op6d}Notification", null).ToString(), new TextObject("{=OMrnTIe0}You rescue several prisoners that the raiders had been dragging along. They look parched and exhausted. You give them a bit of water and bread, and after a short while one staggers to his feet and comes over to you.", null).ToString(), true, false, new TextObject("{=lmG7uRK2}Okay", null).ToString(), null, delegate
						{
							CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(StoryModeHeroes.Tacitus.CharacterObject, null, true, true, false, false, false, false));
						}, null, "", 0f, null, null, null), false, false);
					}
				}
				if (4 > MobileParty.MainParty.MemberRoster.TotalManCount)
				{
					this.DespawnRaiderParties();
					this.OpenRecruitMoreTroopsPopUp();
				}
			}
		}

		protected override void HourlyTick()
		{
			if (4 > MobileParty.MainParty.MemberRoster.TotalManCount)
			{
				this.DespawnRaiderParties();
				this.OpenRecruitMoreTroopsPopUp();
				Campaign.Current.TimeControlMode = 0;
			}
		}

		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			if (this._raiderParties.Contains(mobileParty))
			{
				this._raiderParties.Remove(mobileParty);
			}
		}

		protected override void OnCompleteWithSuccess()
		{
			TutorialPhase.Instance.RemoveTutorialFocusSettlement();
			TutorialPhase.Instance.RemoveTutorialFocusMobileParty();
		}

		internal static void AutoGeneratedStaticCollectObjectsLocateAndRescueTravellerTutorialQuest(object o, List<object> collectedObjects)
		{
			((LocateAndRescueTravellerTutorialQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._raiderParties);
			collectedObjects.Add(this._startQuestLog);
		}

		internal static object AutoGeneratedGetMemberValue_raiderPartyCount(object o)
		{
			return ((LocateAndRescueTravellerTutorialQuest)o)._raiderPartyCount;
		}

		internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
		{
			return ((LocateAndRescueTravellerTutorialQuest)o)._raiderParties;
		}

		internal static object AutoGeneratedGetMemberValue_defeatedRaiderPartyCount(object o)
		{
			return ((LocateAndRescueTravellerTutorialQuest)o)._defeatedRaiderPartyCount;
		}

		internal static object AutoGeneratedGetMemberValue_startQuestLog(object o)
		{
			return ((LocateAndRescueTravellerTutorialQuest)o)._startQuestLog;
		}

		private const int MainPartyHealHitPointLimit = 50;

		private const int PlayerPartySizeMinLimitToSpawnRaiders = 4;

		private const int RaiderPartySize = 6;

		private const int RaiderPartyCount = 3;

		private const string RaiderPartyStringId = "locate_and_rescue_traveller_quest_raider_party_";

		[SaveableField(1)]
		private int _raiderPartyCount;

		[SaveableField(2)]
		private readonly List<MobileParty> _raiderParties;

		[SaveableField(3)]
		private int _defeatedRaiderPartyCount;

		[SaveableField(4)]
		private readonly JournalLog _startQuestLog;
	}
}
