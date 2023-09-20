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
	// Token: 0x0200001E RID: 30
	public class LocateAndRescueTravellerTutorialQuest : StoryModeQuestBase
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00007587 File Offset: 0x00005787
		private TextObject _startQuestLogText
		{
			get
			{
				return new TextObject("{=JJo0i8an}Look around the village to find the party that captured the traveller whom the headman told you about.", null);
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00007594 File Offset: 0x00005794
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=ACyYhA2s}Locate and Rescue Traveller", null);
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000075A4 File Offset: 0x000057A4
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

		// Token: 0x06000134 RID: 308 RVA: 0x00007640 File Offset: 0x00005840
		protected override void RegisterEvents()
		{
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000076C0 File Offset: 0x000058C0
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
			this.AddGameMenus();
		}

		// Token: 0x06000136 RID: 310 RVA: 0x000076D0 File Offset: 0x000058D0
		private MobileParty CreateRaiderParty()
		{
			Settlement settlement = SettlementHelper.FindNearestHideout((Settlement x) => x.IsActive, null);
			MobileParty mobileParty = BanditPartyComponent.CreateBanditParty("locate_and_rescue_traveller_quest_raider_party_" + this._raiderPartyCount, StoryModeHeroes.RadiersClan, settlement.Hideout, false);
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
			SetPartyAiAction.GetActionForPatrollingAroundSettlement(mobileParty, object3);
			mobileParty.Ai.SetDoNotMakeNewDecisions(true);
			mobileParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
			mobileParty.Party.Visuals.SetMapIconAsDirty();
			base.AddTrackedObject(mobileParty);
			mobileParty.IsActive = true;
			this._raiderPartyCount++;
			mobileParty.SetPartyUsedByQuest(true);
			return mobileParty;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000783C File Offset: 0x00005A3C
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

		// Token: 0x06000138 RID: 312 RVA: 0x000078B4 File Offset: 0x00005AB4
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

		// Token: 0x06000139 RID: 313 RVA: 0x000078F4 File Offset: 0x00005AF4
		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("{=BdYaRvhm}I don't know who you are, but I'm in your debt. These brigands would've marched us to our deaths.[ib:demure]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.meeting_tacitus_on_condition))
				.NpcLine(new TextObject("{=9VxUSDQ7}My name's Tacteos. I'm a doctor by trade. I was on, well, a bit of a quest, but now I'm thinking I'm not really made for this kind of thing.[ib:demure]", null), null, null)
				.NpcLine(new TextObject("{=5LJTeOBT}I was with a caravan and they just came out of the brush. We were surrounded and outnumbered, so we gave up. I figured they'd keep us alive, if just for the ransom. But then they started flogging us along at top speed, without any water, and I was just about ready to drop.[ib:demure]", null), null, null)
				.NpcLine(new TextObject("{=XdDQdSsW}I could feel the signs of heat-stroke creeping up and I told them but they just flogged me more... If your group hadn't come along... Maybe I have a way to thank you properly.[ib:demure]", null), null, null)
				.PlayerLine(new TextObject("{=bkZFbCRx}We're looking for two children captured by the raiders. Can you tell us anything?", null), null)
				.NpcLine(new TextObject("{=ehnbi5yD}I am afraid I haven't seen any children. But after our caravan was attacked, the chief of the raiders, the one they call Radagos, took and rode off with our more valuable belongings, including a chest that I had.[ib:demure]", null), null, null)
				.NpcLine(new TextObject("{=RF3NoR3d}He seemed to be controlling more than one band raiding around this area. If this lot has your kin, then I think he'd be the one to know.[ib:demure]", null), null, null)
				.NpcLine(new TextObject("{=K75sH3vW}And since I have nothing of value left to repay your help, I'll tell you this. If you do catch up with and defeat that ruffian, you may be able to recover my chest. It contains a valuable ornament which I was told could be of great value, if you knew where to sell it.[ib:demure]", null), null, null)
				.NpcLine(new TextObject("{=8GCW5IRO}I was trying to find out more about it, but, as I say, I've had all my urge for travelling flogged out of me. Right now I don't think I'd venture more than 20 paces from a well as long as I live.[ib:demure]", null), null, null)
				.PlayerLine(new TextObject("{=Zyn5FrTR}We'll keep that in mind.", null), null)
				.NpcLine(new TextObject("{=vJyTsFdU}It doesn't look like much and I suspect this lot would give it away for a few coins, but I got it from a mercenary whom I treated once, and swore it was related to 'Neretzes's Folly'. I don't know what that means, except that Neretzes was, of course, the emperor who died in battle some years back. Maybe you can find out its true value.[ib:demure]", null), null, null)
				.NpcLine(new TextObject("{=tsjQtWsO}Thanks for saving me again. I hope our paths will cross again![ib:demure]", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.meeting_tacitus_on_consequence))
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("{=!}Start encounter.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.meeting_with_raider_party_on_condition))
				.CloseDialog(), this);
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00007A64 File Offset: 0x00005C64
		private bool meeting_tacitus_on_condition()
		{
			return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == StoryModeHeroes.Tacitus && !Hero.OneToOneConversationHero.HasMet;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00007A88 File Offset: 0x00005C88
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

		// Token: 0x0600013C RID: 316 RVA: 0x00007AF4 File Offset: 0x00005CF4
		private bool meeting_with_raider_party_on_condition()
		{
			return this._raiderParties.Any((MobileParty p) => ConversationHelper.GetConversationCharacterPartyLeader(p.Party) == CharacterObject.OneToOneConversationCharacter);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00007B20 File Offset: 0x00005D20
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

		// Token: 0x0600013E RID: 318 RVA: 0x00007CD4 File Offset: 0x00005ED4
		private void AddGameMenus()
		{
			base.AddGameMenu("encounter_raiders_quest", new TextObject("{=mU1bC1mp}You encountered the raider party.", null), new OnInitDelegate(this.game_menu_encounter_on_init), 4, 0);
			base.AddGameMenuOption("encounter_raiders_quest", "encounter_raiders_quest_attack", new TextObject("{=1r0tDsrR}Attack!", null), new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_attack_on_consequence), false, -1, null);
			base.AddGameMenuOption("encounter_raiders_quest", "encounter_raiders_quest_send_troops", new TextObject("{=z3VamNrX}Send in your troops.", null), new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_send_troops_on_condition), null, false, -1, null);
			base.AddGameMenuOption("encounter_raiders_quest", "encounter_raiders_quest_leave", new TextObject("{=2YYRyrOO}Leave...", null), new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_leave_on_consequence), true, -1, null);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00007D9C File Offset: 0x00005F9C
		private void game_menu_encounter_on_init(MenuCallbackArgs args)
		{
			bool flag = false;
			if (PlayerEncounter.Battle == null)
			{
				PlayerEncounter.StartBattle();
				flag = true;
			}
			if (PlayerEncounter.BattleState == null && !flag)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
			PlayerEncounter.Update();
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00007DCF File Offset: 0x00005FCF
		private bool game_menu_encounter_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00007DDA File Offset: 0x00005FDA
		private void game_menu_encounter_leave_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterLeaveConsequence(args);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00007DE4 File Offset: 0x00005FE4
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

		// Token: 0x06000143 RID: 323 RVA: 0x00007E33 File Offset: 0x00006033
		internal void game_menu_encounter_attack_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterAttackConsequence(args);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00007E3B File Offset: 0x0000603B
		private bool game_menu_encounter_send_troops_on_condition(MenuCallbackArgs args)
		{
			args.IsEnabled = false;
			args.Tooltip = new TextObject("{=hnFkhPhp}This option is disabled during tutorial stage.", null);
			args.optionLeaveType = 10;
			return true;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00007E5E File Offset: 0x0000605E
		[GameMenuInitializationHandler("encounter_raiders_quest")]
		private static void game_menu_encounter_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_looter");
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00007E70 File Offset: 0x00006070
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

		// Token: 0x06000147 RID: 327 RVA: 0x00007EA0 File Offset: 0x000060A0
		private void OpenRecruitMoreTroopsPopUp()
		{
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=y3fn2vWY}Recruit Troops", null).ToString(), new TextObject("{=taOCFKtZ}You need to recruit more troops to deal with the raider party. Go back to village to recruit more troops.", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00007EFC File Offset: 0x000060FC
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

		// Token: 0x06000149 RID: 329 RVA: 0x000080F0 File Offset: 0x000062F0
		private void HourlyTick()
		{
			if (4 > MobileParty.MainParty.MemberRoster.TotalManCount)
			{
				this.DespawnRaiderParties();
				this.OpenRecruitMoreTroopsPopUp();
				Campaign.Current.TimeControlMode = 0;
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000811B File Offset: 0x0000631B
		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			if (this._raiderParties.Contains(mobileParty))
			{
				this._raiderParties.Remove(mobileParty);
			}
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00008138 File Offset: 0x00006338
		protected override void OnCompleteWithSuccess()
		{
			TutorialPhase.Instance.RemoveTutorialFocusSettlement();
			TutorialPhase.Instance.RemoveTutorialFocusMobileParty();
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000814E File Offset: 0x0000634E
		internal static void AutoGeneratedStaticCollectObjectsLocateAndRescueTravellerTutorialQuest(object o, List<object> collectedObjects)
		{
			((LocateAndRescueTravellerTutorialQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000815C File Offset: 0x0000635C
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._raiderParties);
			collectedObjects.Add(this._startQuestLog);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000817D File Offset: 0x0000637D
		internal static object AutoGeneratedGetMemberValue_raiderPartyCount(object o)
		{
			return ((LocateAndRescueTravellerTutorialQuest)o)._raiderPartyCount;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000818F File Offset: 0x0000638F
		internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
		{
			return ((LocateAndRescueTravellerTutorialQuest)o)._raiderParties;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000819C File Offset: 0x0000639C
		internal static object AutoGeneratedGetMemberValue_defeatedRaiderPartyCount(object o)
		{
			return ((LocateAndRescueTravellerTutorialQuest)o)._defeatedRaiderPartyCount;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x000081AE File Offset: 0x000063AE
		internal static object AutoGeneratedGetMemberValue_startQuestLog(object o)
		{
			return ((LocateAndRescueTravellerTutorialQuest)o)._startQuestLog;
		}

		// Token: 0x0400007C RID: 124
		private const int MainPartyHealHitPointLimit = 50;

		// Token: 0x0400007D RID: 125
		private const int PlayerPartySizeMinLimitToSpawnRaiders = 4;

		// Token: 0x0400007E RID: 126
		private const int RaiderPartySize = 6;

		// Token: 0x0400007F RID: 127
		private const int RaiderPartyCount = 3;

		// Token: 0x04000080 RID: 128
		private const string RaiderPartyStringId = "locate_and_rescue_traveller_quest_raider_party_";

		// Token: 0x04000081 RID: 129
		[SaveableField(1)]
		private int _raiderPartyCount;

		// Token: 0x04000082 RID: 130
		[SaveableField(2)]
		private readonly List<MobileParty> _raiderParties;

		// Token: 0x04000083 RID: 131
		[SaveableField(3)]
		private int _defeatedRaiderPartyCount;

		// Token: 0x04000084 RID: 132
		[SaveableField(4)]
		private readonly JournalLog _startQuestLog;
	}
}
