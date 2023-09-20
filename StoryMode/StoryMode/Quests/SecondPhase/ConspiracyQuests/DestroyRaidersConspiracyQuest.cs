using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
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
	// Token: 0x02000029 RID: 41
	internal class DestroyRaidersConspiracyQuest : ConspiracyQuestBase
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000B112 File Offset: 0x00009312
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=DfiACGay}Destroy Raiders", null);
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060001E9 RID: 489 RVA: 0x0000B11F File Offset: 0x0000931F
		public override float ConspiracyStrengthDecreaseAmount
		{
			get
			{
				return 50f;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060001EA RID: 490 RVA: 0x0000B126 File Offset: 0x00009326
		private int RegularRaiderPartyTroopCount
		{
			get
			{
				return 17 + MathF.Ceiling(23f * Campaign.Current.Models.IssueModel.GetIssueDifficultyMultiplier());
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060001EB RID: 491 RVA: 0x0000B14A File Offset: 0x0000934A
		private int SpecialRaiderPartyTroopCount
		{
			get
			{
				return 33 + MathF.Ceiling(37f * Campaign.Current.Models.IssueModel.GetIssueDifficultyMultiplier());
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060001EC RID: 492 RVA: 0x0000B170 File Offset: 0x00009370
		public override TextObject StartLog
		{
			get
			{
				TextObject textObject = new TextObject("{=Dr63pCHt}{MENTOR.LINK} has sent you a message about bandit attacks near {TARGET_SETTLEMENT}, and advises you to go there and eliminate them all before their actions turn the locals against your movement. ", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.QuestGiver.CharacterObject, textObject, false);
				textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060001ED RID: 493 RVA: 0x0000B1BC File Offset: 0x000093BC
		public override TextObject StartMessageLogFromMentor
		{
			get
			{
				TextObject textObject = new TextObject("{=V5K8RpAa}{MENTOR.LINK}'s message: “Greetings, {PLAYER.NAME}. We have a new problem. I've had reports from my agents of unusual bandit activity near {TARGET_SETTLEMENT}. They appear to be raiding and killing travellers {?IS_EMPIRE}under the protection of the Empire{?}who aren't under the protection of the Empire{\\?}, and leaving the others alone. This seems very much like the work of {NEMESIS_MENTOR.LINK}, to terrorize local merchants so that no one will stand up for our cause. I advise you to wipe these bandits out as quickly as possible. That would send a good message, both to our allies and our enemies.”", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.QuestGiver.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, textObject, false);
				bool isOnImperialQuestLine = StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine;
				StringHelpers.SetCharacterProperties("NEMESIS_MENTOR", isOnImperialQuestLine ? StoryModeHeroes.AntiImperialMentor.CharacterObject : StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				textObject.SetTextVariable("IS_IMPERIAL", isOnImperialQuestLine ? 1 : 0);
				textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060001EE RID: 494 RVA: 0x0000B268 File Offset: 0x00009468
		public override TextObject SideNotificationText
		{
			get
			{
				TextObject textObject = new TextObject("{=T7OTmJUp}{MENTOR.LINK} has a message for you", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.QuestGiver.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000B29A File Offset: 0x0000949A
		private TextObject _destroyRaidersQuestSucceededLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=qg05CSZb}You have defeated all the raiders near {TARGET_SETTLEMENT}. Many people now hope you can bring peace and prosperity back to the region.", null);
				textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x0000B2BE File Offset: 0x000094BE
		private TextObject _destroyRaidersQuestFailedOnTimedOutLogText
		{
			get
			{
				return new TextObject("{=DaBN0O7N}You have failed to defeat all raider parties in time. Many of the locals feel that you've brought misfortune upon them, and want nothing to do with you.", null);
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000B2CB File Offset: 0x000094CB
		private TextObject _destroyRaidersQuestFailedOnPlayerDefeatedByRaidersLogText
		{
			get
			{
				return new TextObject("{=mN60B07k}You have lost the battle against raiders and failed to defeat conspiracy forces. Many of the locals feel that you've brought misfortune upon them, and want nothing to do with you.", null);
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x0000B2D8 File Offset: 0x000094D8
		private TextObject _destroyRaidersRegularPartiesProgress
		{
			get
			{
				TextObject textObject = new TextObject("{=dbLb3krw}Hunt the gangs of {RAIDER_NAME}", null);
				textObject.SetTextVariable("RAIDER_NAME", this._banditFaction.Name);
				return textObject;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x0000B2FC File Offset: 0x000094FC
		private TextObject _destroyRaidersSpecialPartyProgress
		{
			get
			{
				return new TextObject("{=QVkuaezc}Hunt the conspiracy war party", null);
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x0000B309 File Offset: 0x00009509
		private TextObject _destroyRaidersRegularProgressNotification
		{
			get
			{
				TextObject textObject = new TextObject("{=US0VAHiE}You have eliminated a {RAIDER_NAME} party.", null);
				textObject.SetTextVariable("RAIDER_NAME", this._banditFaction.Name);
				return textObject;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x0000B32D File Offset: 0x0000952D
		private TextObject _destroyRaidersRegularProgressCompletedNotification
		{
			get
			{
				TextObject textObject = new TextObject("{=LfH7VXDH}You have eliminated all {RAIDER_NAME} gangs in the vicinity.", null);
				textObject.SetTextVariable("RAIDER_NAME", this._banditFaction.Name);
				return textObject;
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x0000B351 File Offset: 0x00009551
		private TextObject _destroyRaidersSpecialPartyInformationQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=agrsO3qQ}Due to your successful skirmishes against {RAIDER_NAME}, a conspiracy war party is now patrolling around {SETTLEMENT}.", null);
				textObject.SetTextVariable("RAIDER_NAME", this._banditFaction.Name);
				textObject.SetTextVariable("SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x0000B38C File Offset: 0x0000958C
		private TextObject _destroyRaidersSpecialPartySpawnNotification
		{
			get
			{
				TextObject textObject = new TextObject("{=QOVLkdTp}A conspiracy war party is now patrolling around {SETTLEMENT}.", null);
				textObject.SetTextVariable("SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000B3B0 File Offset: 0x000095B0
		public DestroyRaidersConspiracyQuest(string questId, Hero questGiver)
			: base(questId, questGiver)
		{
			this._regularRaiderParties = new List<MobileParty>(3);
			this._directedRaidersToEngagePlayer = new List<MobileParty>(3);
			this._targetSettlement = this.DetermineTargetSettlement();
			this._banditFaction = this.GetBanditTypeForSettlement(this._targetSettlement);
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000B3F0 File Offset: 0x000095F0
		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(this.GetConspiracyCaptainDialogue(), this);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000B408 File Offset: 0x00009608
		protected override void RegisterEvents()
		{
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroTakenPrisoner));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.MobilePartyDestroyed));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000B488 File Offset: 0x00009688
		private void OnGameMenuOpened(MenuCallbackArgs menuCallbackArgs)
		{
			if (menuCallbackArgs.MenuContext.GameMenu.StringId == "prisoner_wait")
			{
				PartyBase captorParty = PlayerCaptivity.CaptorParty;
				if (captorParty != null && captorParty.IsMobile && (this._regularRaiderParties.Contains(PlayerCaptivity.CaptorParty.MobileParty) || this._specialRaiderParty == PlayerCaptivity.CaptorParty.MobileParty))
				{
					this.OnQuestFailedByDefeat();
				}
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000B4F4 File Offset: 0x000096F4
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
			this.DetermineClosestHideouts();
			if (this._directedRaidersToEngagePlayer == null)
			{
				this._directedRaidersToEngagePlayer = new List<MobileParty>(3);
				return;
			}
			if (this._directedRaidersToEngagePlayer.Count > this._regularRaiderParties.Count)
			{
				this._directedRaidersToEngagePlayer = new List<MobileParty>(3);
				using (List<MobileParty>.Enumerator enumerator = this._regularRaiderParties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MobileParty mobileParty = enumerator.Current;
						this.SetDefaultRaiderAi(mobileParty);
					}
					return;
				}
			}
			foreach (MobileParty mobileParty2 in this._regularRaiderParties)
			{
				this.CheckRaiderPartyPlayerEncounter(mobileParty2);
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000B5D0 File Offset: 0x000097D0
		protected override void OnStartQuest()
		{
			base.OnStartQuest();
			string text = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? "conspiracy_commander_antiempire" : "conspiracy_commander_empire");
			this._conspiracyCaptainCharacter = Game.Current.ObjectManager.GetObject<CharacterObject>(text);
			this.InitializeRaiders();
			this._regularPartiesProgressTracker = base.AddDiscreteLog(this._destroyRaidersRegularPartiesProgress, TextObject.Empty, 0, 3, null, false);
			this.SetDialogs();
			base.InitializeQuestOnCreation();
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000B644 File Offset: 0x00009844
		private Settlement DetermineTargetSettlement()
		{
			Settlement settlement = null;
			if (!Extensions.IsEmpty<Settlement>(Clan.PlayerClan.Settlements))
			{
				settlement = Extensions.GetRandomElementWithPredicate<Settlement>(Clan.PlayerClan.Settlements, (Settlement t) => t.IsTown || t.IsCastle);
			}
			else
			{
				MBList<Settlement> mblist = Extensions.ToMBList<Settlement>(StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom.Settlements.Where((Settlement t) => t.IsTown || t.IsCastle));
				if (!Extensions.IsEmpty<Settlement>(mblist))
				{
					settlement = Extensions.GetRandomElement<Settlement>(mblist);
				}
			}
			if (settlement == null)
			{
				Debug.FailedAssert("Destroy raiders conspiracy quest settlement is null", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\Quests\\SecondPhase\\ConspiracyQuests\\DestroyRaidersConspiracyQuest.cs", "DetermineTargetSettlement", 305);
				settlement = Extensions.GetRandomElementWithPredicate<Settlement>(Settlement.All, (Settlement t) => t.IsTown || t.IsCastle);
			}
			return settlement;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000B72C File Offset: 0x0000992C
		private void InitializeRaiders()
		{
			List<Settlement> list = this.DetermineClosestHideouts();
			for (int i = 0; i < 3; i++)
			{
				this.SpawnRaiderPartyAtHideout(list.ElementAt(i), false);
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000B75C File Offset: 0x0000995C
		private List<Settlement> DetermineClosestHideouts()
		{
			MapDistanceModel model = Campaign.Current.Models.MapDistanceModel;
			List<Settlement> list = (from x in Hideout.All
				select x.Settlement into t
				orderby model.GetDistance(this._targetSettlement, t)
				select t).Take(3).ToList<Settlement>();
			this._closestHideout = list[0];
			return list;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000B7E0 File Offset: 0x000099E0
		private void SpawnRaiderPartyAtHideout(Settlement hideout, bool isSpecialParty = false)
		{
			MobileParty mobileParty = BanditPartyComponent.CreateBanditParty(string.Concat(new object[]
			{
				"destroy_raiders_conspiracy_quest_",
				this._banditFaction.Name,
				"_",
				CampaignTime.Now.ElapsedSecondsUntilNow
			}), this._banditFaction, hideout.Hideout, false);
			PartyTemplateObject partyTemplateObject;
			int num;
			TextObject textObject;
			if (isSpecialParty)
			{
				this._specialRaiderParty = mobileParty;
				partyTemplateObject = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("conspiracy_anti_imperial_special_raider_party_template") : Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("conspiracy_imperial_special_raider_party_template"));
				num = this.SpecialRaiderPartyTroopCount;
				textObject = new TextObject("{=GW7Zg3IP}Conspiracy War Party", null);
			}
			else
			{
				this._regularRaiderParties.Add(mobileParty);
				partyTemplateObject = this._banditFaction.DefaultPartyTemplate;
				num = this.RegularRaiderPartyTroopCount;
				textObject = this._banditFaction.Name;
			}
			mobileParty.InitializeMobilePartyAroundPosition(partyTemplateObject, hideout.GatePosition, 0.2f, 0.1f, num);
			mobileParty.SetCustomName(textObject);
			mobileParty.MemberRoster.Clear();
			mobileParty.SetPartyUsedByQuest(true);
			this.SetDefaultRaiderAi(mobileParty);
			if (isSpecialParty)
			{
				mobileParty.MemberRoster.AddToCounts(this._conspiracyCaptainCharacter, 1, true, 0, 0, true, -1);
				mobileParty.ItemRoster.Clear();
				mobileParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("vlandia_horse"), num / 2);
				MBInformationManager.AddQuickInformation(this._destroyRaidersSpecialPartySpawnNotification, 0, null, "");
			}
			base.DistributeConspiracyRaiderTroopsByLevel(partyTemplateObject, mobileParty.Party, num);
			base.AddTrackedObject(mobileParty);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000B96C File Offset: 0x00009B6C
		private void SetDefaultRaiderAi(MobileParty raiderParty)
		{
			SetPartyAiAction.GetActionForPatrollingAroundSettlement(raiderParty, this._targetSettlement);
			raiderParty.Ai.CheckPartyNeedsUpdate();
			raiderParty.Ai.SetDoNotMakeNewDecisions(true);
			raiderParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000B99C File Offset: 0x00009B9C
		private Clan GetBanditTypeForSettlement(Settlement settlement)
		{
			Settlement closestHideout = SettlementHelper.FindNearestHideout((Settlement x) => x.IsActive, settlement);
			return Clan.BanditFactions.FirstOrDefault((Clan t) => t.Culture == closestHideout.Culture);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000B9F0 File Offset: 0x00009BF0
		private void MobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			if (destroyerParty != null && destroyerParty.MobileParty == MobileParty.MainParty)
			{
				if (this._regularRaiderParties.Contains(mobileParty))
				{
					this.OnBanditPartyClearedByPlayer(mobileParty);
					return;
				}
				if (this._specialRaiderParty == mobileParty)
				{
					this.OnSpecialBanditPartyClearedByPlayer();
				}
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000BA28 File Offset: 0x00009C28
		private void MapEventEnded(MapEvent mapEvent)
		{
			if (mapEvent.WinningSide != -1 && mapEvent.DefeatedSide != -1 && mapEvent.IsPlayerMapEvent && mapEvent.InvolvedParties.Any((PartyBase t) => t.IsMobile && (this._regularRaiderParties.Contains(t.MobileParty) || t.MobileParty == this._specialRaiderParty)))
			{
				if (PlayerEncounter.Battle.WinningSide == PlayerEncounter.Battle.PlayerSide)
				{
					using (List<MapEventParty>.Enumerator enumerator = mapEvent.GetMapEventSide(mapEvent.DefeatedSide).Parties.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MapEventParty mapEventParty = enumerator.Current;
							MobileParty mobileParty = mapEventParty.Party.MobileParty;
							if (mobileParty != null && mobileParty.Party.NumberOfHealthyMembers > 0 && (this._regularRaiderParties.Contains(mobileParty) || this._specialRaiderParty == mobileParty))
							{
								DestroyPartyAction.Apply(PartyBase.MainParty, mobileParty);
							}
						}
						return;
					}
				}
				PartyBase captorParty = PlayerCaptivity.CaptorParty;
				if (captorParty == null || !captorParty.IsMobile || (!this._regularRaiderParties.Contains(PlayerCaptivity.CaptorParty.MobileParty) && this._specialRaiderParty != PlayerCaptivity.CaptorParty.MobileParty))
				{
					this.OnQuestFailedByDefeat();
				}
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000BB60 File Offset: 0x00009D60
		private void OnSpecialBanditPartyClearedByPlayer()
		{
			if (base.IsTracked(this._specialRaiderParty))
			{
				base.RemoveTrackedObject(this._specialRaiderParty);
			}
			this._specialPartyProgressTracker.UpdateCurrentProgress(1);
			this._specialRaiderParty = null;
			this.OnQuestSucceeded();
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000BB98 File Offset: 0x00009D98
		private void OnBanditPartyClearedByPlayer(MobileParty defeatedParty)
		{
			this._regularRaiderParties.Remove(defeatedParty);
			this._regularPartiesProgressTracker.UpdateCurrentProgress(3 - this._regularRaiderParties.Count);
			if (this._regularPartiesProgressTracker.HasBeenCompleted())
			{
				MBInformationManager.AddQuickInformation(this._destroyRaidersRegularProgressCompletedNotification, 0, null, "");
				base.AddLog(this._destroyRaidersSpecialPartyInformationQuestLog, false);
				this._specialPartyProgressTracker = base.AddDiscreteLog(this._destroyRaidersSpecialPartyProgress, TextObject.Empty, 0, 1, null, false);
				this.SpawnRaiderPartyAtHideout(this._closestHideout, true);
				return;
			}
			if (base.IsTracked(defeatedParty))
			{
				base.RemoveTrackedObject(defeatedParty);
			}
			MBInformationManager.AddQuickInformation(this._destroyRaidersRegularProgressNotification, 0, null, "");
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000BC44 File Offset: 0x00009E44
		private void OnHeroTakenPrisoner(PartyBase capturer, Hero prisoner)
		{
			if (prisoner.Clan != Clan.PlayerClan && capturer.IsMobile && (this._regularRaiderParties.Contains(capturer.MobileParty) || this._specialRaiderParty == capturer.MobileParty))
			{
				Debug.FailedAssert("Hero has been taken prisoner by conspiracy raider party", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\Quests\\SecondPhase\\ConspiracyQuests\\DestroyRaidersConspiracyQuest.cs", "OnHeroTakenPrisoner", 525);
				EndCaptivityAction.ApplyByEscape(prisoner, null);
			}
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000BCA8 File Offset: 0x00009EA8
		private void HourlyTick()
		{
			foreach (MobileParty mobileParty in this._regularRaiderParties)
			{
				this.CheckRaiderPartyPlayerEncounter(mobileParty);
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000BCFC File Offset: 0x00009EFC
		private void CheckRaiderPartyPlayerEncounter(MobileParty raiderParty)
		{
			if (raiderParty.Position2D.DistanceSquared(MobileParty.MainParty.Position2D) <= 9f && raiderParty.Ai.DoNotAttackMainPartyUntil.IsPast && raiderParty.Party.TotalStrength > PartyBase.MainParty.TotalStrength * 1.2f && MobileParty.MainParty.CurrentSettlement == null)
			{
				if (!this._directedRaidersToEngagePlayer.Contains(raiderParty))
				{
					SetPartyAiAction.GetActionForEngagingParty(raiderParty, MobileParty.MainParty);
					raiderParty.Ai.CheckPartyNeedsUpdate();
					this._directedRaidersToEngagePlayer.Add(raiderParty);
					return;
				}
			}
			else if (this._directedRaidersToEngagePlayer.Contains(raiderParty))
			{
				this._directedRaidersToEngagePlayer.Remove(raiderParty);
				this.SetDefaultRaiderAi(raiderParty);
			}
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000BDBC File Offset: 0x00009FBC
		private DialogFlow GetConspiracyCaptainDialogue()
		{
			return DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=bzmcPtZ6}We know you. We were told to look out for you. We know what you're planning with {MENTOR.NAME}. You will fail, and you will die.", null, null).Condition(delegate
			{
				StringHelpers.SetCharacterProperties("MENTOR", base.QuestGiver.CharacterObject, null, false);
				return CharacterObject.OneToOneConversationCharacter == this._conspiracyCaptainCharacter && this._specialRaiderParty != null && !this._specialPartyProgressTracker.HasBeenCompleted();
			})
				.BeginPlayerOptions()
				.PlayerOption("{=BrHU0NuE}Maybe. But if we do, you won't live to see it.", null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnConspiracyCaptainDialogueEnd;
				})
				.NpcLine("{=EoLcoaHM}We'll see...", null, null)
				.CloseDialog()
				.PlayerOption("{=TLaxmQDF}You'll without a doubt perish by my sword, but today is not the day.", null)
				.Consequence(delegate
				{
					PlayerEncounter.LeaveEncounter = true;
				})
				.NpcLine("{=9aY0ifwi}We shall meet again...", null, null)
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog();
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000BE6E File Offset: 0x0000A06E
		private void OnConspiracyCaptainDialogueEnd()
		{
			PlayerEncounter.RestartPlayerEncounter(this._specialRaiderParty.Party, PartyBase.MainParty, true);
			PlayerEncounter.StartBattle();
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000BE8C File Offset: 0x0000A08C
		private void OnQuestSucceeded()
		{
			if (this._targetSettlement.OwnerClan != Clan.PlayerClan && !this._targetSettlement.OwnerClan.MapFaction.IsAtWarWith(Clan.PlayerClan.MapFaction))
			{
				ChangeRelationAction.ApplyPlayerRelation(this._targetSettlement.OwnerClan.Leader, 5, true, true);
			}
			Clan.PlayerClan.AddRenown(5f, true);
			this._targetSettlement.Town.Security += 5f;
			this._targetSettlement.Prosperity += 5f;
			base.AddLog(this._destroyRaidersQuestSucceededLogText, false);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000BF3B File Offset: 0x0000A13B
		private void OnQuestFailedByDefeat()
		{
			this.OnQuestFailed();
			base.AddLog(this._destroyRaidersQuestFailedOnPlayerDefeatedByRaidersLogText, false);
			base.CompleteQuestWithFail(null);
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000BF58 File Offset: 0x0000A158
		private void OnQuestFailed()
		{
			foreach (MobileParty mobileParty in this._regularRaiderParties)
			{
				if (mobileParty.IsActive)
				{
					DestroyPartyAction.Apply(null, mobileParty);
				}
			}
			if (this._specialRaiderParty != null && this._specialRaiderParty.IsActive)
			{
				DestroyPartyAction.Apply(null, this._specialRaiderParty);
			}
			if (this._targetSettlement.OwnerClan != Clan.PlayerClan)
			{
				ChangeRelationAction.ApplyPlayerRelation(this._targetSettlement.OwnerClan.Leader, -5, true, true);
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000C000 File Offset: 0x0000A200
		protected override void OnTimedOut()
		{
			this.OnQuestFailed();
			base.AddLog(this._destroyRaidersQuestFailedOnTimedOutLogText, false);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000C016 File Offset: 0x0000A216
		internal static void AutoGeneratedStaticCollectObjectsDestroyRaidersConspiracyQuest(object o, List<object> collectedObjects)
		{
			((DestroyRaidersConspiracyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000C024 File Offset: 0x0000A224
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._targetSettlement);
			collectedObjects.Add(this._regularRaiderParties);
			collectedObjects.Add(this._specialRaiderParty);
			collectedObjects.Add(this._regularPartiesProgressTracker);
			collectedObjects.Add(this._specialPartyProgressTracker);
			collectedObjects.Add(this._banditFaction);
			collectedObjects.Add(this._conspiracyCaptainCharacter);
			collectedObjects.Add(this._closestHideout);
			collectedObjects.Add(this._directedRaidersToEngagePlayer);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000C0A4 File Offset: 0x0000A2A4
		internal static object AutoGeneratedGetMemberValue_targetSettlement(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._targetSettlement;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000C0B1 File Offset: 0x0000A2B1
		internal static object AutoGeneratedGetMemberValue_regularRaiderParties(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._regularRaiderParties;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000C0BE File Offset: 0x0000A2BE
		internal static object AutoGeneratedGetMemberValue_specialRaiderParty(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._specialRaiderParty;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000C0CB File Offset: 0x0000A2CB
		internal static object AutoGeneratedGetMemberValue_regularPartiesProgressTracker(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._regularPartiesProgressTracker;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000C0D8 File Offset: 0x0000A2D8
		internal static object AutoGeneratedGetMemberValue_specialPartyProgressTracker(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._specialPartyProgressTracker;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000C0E5 File Offset: 0x0000A2E5
		internal static object AutoGeneratedGetMemberValue_banditFaction(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._banditFaction;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000C0F2 File Offset: 0x0000A2F2
		internal static object AutoGeneratedGetMemberValue_conspiracyCaptainCharacter(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._conspiracyCaptainCharacter;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000C0FF File Offset: 0x0000A2FF
		internal static object AutoGeneratedGetMemberValue_closestHideout(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._closestHideout;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000C10C File Offset: 0x0000A30C
		internal static object AutoGeneratedGetMemberValue_directedRaidersToEngagePlayer(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._directedRaidersToEngagePlayer;
		}

		// Token: 0x0400009D RID: 157
		private const int QuestSuccededRelationBonus = 5;

		// Token: 0x0400009E RID: 158
		private const int QuestSucceededSecurityBonus = 5;

		// Token: 0x0400009F RID: 159
		private const int QuestSuceededProsperityBonus = 5;

		// Token: 0x040000A0 RID: 160
		private const int QuestSuceededRenownBonus = 5;

		// Token: 0x040000A1 RID: 161
		private const int QuestFailedRelationPenalty = -5;

		// Token: 0x040000A2 RID: 162
		private const int NumberOfRegularRaidersToSpawn = 3;

		// Token: 0x040000A3 RID: 163
		private const float RaiderPartyPlayerEncounterRadius = 9f;

		// Token: 0x040000A4 RID: 164
		[SaveableField(1)]
		private readonly Settlement _targetSettlement;

		// Token: 0x040000A5 RID: 165
		[SaveableField(2)]
		private readonly List<MobileParty> _regularRaiderParties;

		// Token: 0x040000A6 RID: 166
		[SaveableField(3)]
		private MobileParty _specialRaiderParty;

		// Token: 0x040000A7 RID: 167
		[SaveableField(4)]
		private JournalLog _regularPartiesProgressTracker;

		// Token: 0x040000A8 RID: 168
		[SaveableField(5)]
		private JournalLog _specialPartyProgressTracker;

		// Token: 0x040000A9 RID: 169
		[SaveableField(6)]
		private Clan _banditFaction;

		// Token: 0x040000AA RID: 170
		[SaveableField(7)]
		private CharacterObject _conspiracyCaptainCharacter;

		// Token: 0x040000AB RID: 171
		[SaveableField(8)]
		private Settlement _closestHideout;

		// Token: 0x040000AC RID: 172
		[SaveableField(9)]
		private List<MobileParty> _directedRaidersToEngagePlayer;
	}
}
