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
	internal class DestroyRaidersConspiracyQuest : ConspiracyQuestBase
	{
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=DfiACGay}Destroy Raiders", null);
			}
		}

		public override float ConspiracyStrengthDecreaseAmount
		{
			get
			{
				return 50f;
			}
		}

		private int RegularRaiderPartyTroopCount
		{
			get
			{
				return 17 + MathF.Ceiling(23f * Campaign.Current.Models.IssueModel.GetIssueDifficultyMultiplier());
			}
		}

		private int SpecialRaiderPartyTroopCount
		{
			get
			{
				return 33 + MathF.Ceiling(37f * Campaign.Current.Models.IssueModel.GetIssueDifficultyMultiplier());
			}
		}

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

		public override TextObject SideNotificationText
		{
			get
			{
				TextObject textObject = new TextObject("{=T7OTmJUp}{MENTOR.LINK} has a message for you", null);
				StringHelpers.SetCharacterProperties("MENTOR", base.QuestGiver.CharacterObject, textObject, false);
				return textObject;
			}
		}

		private TextObject _destroyRaidersQuestSucceededLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=qg05CSZb}You have defeated all the raiders near {TARGET_SETTLEMENT}. Many people now hope you can bring peace and prosperity back to the region.", null);
				textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		private TextObject _destroyRaidersQuestFailedOnTimedOutLogText
		{
			get
			{
				return new TextObject("{=DaBN0O7N}You have failed to defeat all raider parties in time. Many of the locals feel that you've brought misfortune upon them, and want nothing to do with you.", null);
			}
		}

		private TextObject _destroyRaidersQuestFailedOnPlayerDefeatedByRaidersLogText
		{
			get
			{
				return new TextObject("{=mN60B07k}You have lost the battle against raiders and failed to defeat conspiracy forces. Many of the locals feel that you've brought misfortune upon them, and want nothing to do with you.", null);
			}
		}

		private TextObject _destroyRaidersRegularPartiesProgress
		{
			get
			{
				TextObject textObject = new TextObject("{=dbLb3krw}Hunt the gangs of {RAIDER_NAME}", null);
				textObject.SetTextVariable("RAIDER_NAME", this._banditFaction.Name);
				return textObject;
			}
		}

		private TextObject _destroyRaidersSpecialPartyProgress
		{
			get
			{
				return new TextObject("{=QVkuaezc}Hunt the conspiracy war party", null);
			}
		}

		private TextObject _destroyRaidersRegularProgressNotification
		{
			get
			{
				TextObject textObject = new TextObject("{=US0VAHiE}You have eliminated a {RAIDER_NAME} party.", null);
				textObject.SetTextVariable("RAIDER_NAME", this._banditFaction.Name);
				return textObject;
			}
		}

		private TextObject _destroyRaidersRegularProgressCompletedNotification
		{
			get
			{
				TextObject textObject = new TextObject("{=LfH7VXDH}You have eliminated all {RAIDER_NAME} gangs in the vicinity.", null);
				textObject.SetTextVariable("RAIDER_NAME", this._banditFaction.Name);
				return textObject;
			}
		}

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

		private TextObject _destroyRaidersSpecialPartySpawnNotification
		{
			get
			{
				TextObject textObject = new TextObject("{=QOVLkdTp}A conspiracy war party is now patrolling around {SETTLEMENT}.", null);
				textObject.SetTextVariable("SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		public DestroyRaidersConspiracyQuest(string questId, Hero questGiver)
			: base(questId, questGiver)
		{
			this._regularRaiderParties = new List<MobileParty>(3);
			this._directedRaidersToEngagePlayer = new List<MobileParty>(3);
			this._targetSettlement = this.DetermineTargetSettlement();
			this._banditFaction = this.GetBanditTypeForSettlement(this._targetSettlement);
		}

		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(this.GetConspiracyCaptainDialogue(), this);
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroTakenPrisoner));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.MobilePartyDestroyed));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
		}

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
				Debug.FailedAssert("Destroy raiders conspiracy quest settlement is null", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\Quests\\SecondPhase\\ConspiracyQuests\\DestroyRaidersConspiracyQuest.cs", "DetermineTargetSettlement", 304);
				settlement = Extensions.GetRandomElementWithPredicate<Settlement>(Settlement.All, (Settlement t) => t.IsTown || t.IsCastle);
			}
			return settlement;
		}

		private void InitializeRaiders()
		{
			List<Settlement> list = this.DetermineClosestHideouts();
			for (int i = 0; i < 3; i++)
			{
				this.SpawnRaiderPartyAtHideout(list.ElementAt(i), false);
			}
		}

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

		private void SetDefaultRaiderAi(MobileParty raiderParty)
		{
			SetPartyAiAction.GetActionForPatrollingAroundSettlement(raiderParty, this._targetSettlement);
			raiderParty.Ai.CheckPartyNeedsUpdate();
			raiderParty.Ai.SetDoNotMakeNewDecisions(true);
			raiderParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
		}

		private Clan GetBanditTypeForSettlement(Settlement settlement)
		{
			Settlement closestHideout = SettlementHelper.FindNearestHideout((Settlement x) => x.IsActive, settlement);
			return Clan.BanditFactions.FirstOrDefault((Clan t) => t.Culture == closestHideout.Culture);
		}

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

		private void OnHeroTakenPrisoner(PartyBase capturer, Hero prisoner)
		{
			if (prisoner.Clan != Clan.PlayerClan && capturer.IsMobile && (this._regularRaiderParties.Contains(capturer.MobileParty) || this._specialRaiderParty == capturer.MobileParty))
			{
				Debug.FailedAssert("Hero has been taken prisoner by conspiracy raider party", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\Quests\\SecondPhase\\ConspiracyQuests\\DestroyRaidersConspiracyQuest.cs", "OnHeroTakenPrisoner", 524);
				EndCaptivityAction.ApplyByEscape(prisoner, null);
			}
		}

		protected override void HourlyTick()
		{
			foreach (MobileParty mobileParty in this._regularRaiderParties)
			{
				this.CheckRaiderPartyPlayerEncounter(mobileParty);
			}
		}

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

		private DialogFlow GetConspiracyCaptainDialogue()
		{
			return DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=bzmcPtZ6}We know you. We were told to look out for you. We know what you're planning with {MENTOR.NAME}. You will fail, and you will die.[ib:closed][if:convo_predatory]", null, null).Condition(delegate
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
				.NpcLine("{=9aY0ifwi}We shall meet again...[if:convo_insulted]", null, null)
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog();
		}

		private void OnConspiracyCaptainDialogueEnd()
		{
			PlayerEncounter.RestartPlayerEncounter(this._specialRaiderParty.Party, PartyBase.MainParty, true);
			PlayerEncounter.StartBattle();
		}

		private void OnQuestSucceeded()
		{
			if (this._targetSettlement.OwnerClan != Clan.PlayerClan && !this._targetSettlement.OwnerClan.MapFaction.IsAtWarWith(Clan.PlayerClan.MapFaction))
			{
				ChangeRelationAction.ApplyPlayerRelation(this._targetSettlement.OwnerClan.Leader, 5, true, true);
			}
			Clan.PlayerClan.AddRenown(5f, true);
			this._targetSettlement.Town.Security += 5f;
			this._targetSettlement.Town.Prosperity += 5f;
			base.AddLog(this._destroyRaidersQuestSucceededLogText, false);
			base.CompleteQuestWithSuccess();
		}

		private void OnQuestFailedByDefeat()
		{
			this.OnQuestFailed();
			base.AddLog(this._destroyRaidersQuestFailedOnPlayerDefeatedByRaidersLogText, false);
			base.CompleteQuestWithFail(null);
		}

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

		protected override void OnTimedOut()
		{
			this.OnQuestFailed();
			base.AddLog(this._destroyRaidersQuestFailedOnTimedOutLogText, false);
		}

		internal static void AutoGeneratedStaticCollectObjectsDestroyRaidersConspiracyQuest(object o, List<object> collectedObjects)
		{
			((DestroyRaidersConspiracyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

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

		internal static object AutoGeneratedGetMemberValue_targetSettlement(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._targetSettlement;
		}

		internal static object AutoGeneratedGetMemberValue_regularRaiderParties(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._regularRaiderParties;
		}

		internal static object AutoGeneratedGetMemberValue_specialRaiderParty(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._specialRaiderParty;
		}

		internal static object AutoGeneratedGetMemberValue_regularPartiesProgressTracker(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._regularPartiesProgressTracker;
		}

		internal static object AutoGeneratedGetMemberValue_specialPartyProgressTracker(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._specialPartyProgressTracker;
		}

		internal static object AutoGeneratedGetMemberValue_banditFaction(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._banditFaction;
		}

		internal static object AutoGeneratedGetMemberValue_conspiracyCaptainCharacter(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._conspiracyCaptainCharacter;
		}

		internal static object AutoGeneratedGetMemberValue_closestHideout(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._closestHideout;
		}

		internal static object AutoGeneratedGetMemberValue_directedRaidersToEngagePlayer(object o)
		{
			return ((DestroyRaidersConspiracyQuest)o)._directedRaidersToEngagePlayer;
		}

		private const int QuestSuccededRelationBonus = 5;

		private const int QuestSucceededSecurityBonus = 5;

		private const int QuestSuceededProsperityBonus = 5;

		private const int QuestSuceededRenownBonus = 5;

		private const int QuestFailedRelationPenalty = -5;

		private const int NumberOfRegularRaidersToSpawn = 3;

		private const float RaiderPartyPlayerEncounterRadius = 9f;

		[SaveableField(1)]
		private readonly Settlement _targetSettlement;

		[SaveableField(2)]
		private readonly List<MobileParty> _regularRaiderParties;

		[SaveableField(3)]
		private MobileParty _specialRaiderParty;

		[SaveableField(4)]
		private JournalLog _regularPartiesProgressTracker;

		[SaveableField(5)]
		private JournalLog _specialPartyProgressTracker;

		[SaveableField(6)]
		private Clan _banditFaction;

		[SaveableField(7)]
		private CharacterObject _conspiracyCaptainCharacter;

		[SaveableField(8)]
		private Settlement _closestHideout;

		[SaveableField(9)]
		private List<MobileParty> _directedRaidersToEngagePlayer;
	}
}
