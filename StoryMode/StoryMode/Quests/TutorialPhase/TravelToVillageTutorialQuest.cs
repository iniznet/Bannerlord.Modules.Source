using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.TutorialPhase
{
	// Token: 0x02000022 RID: 34
	public class TravelToVillageTutorialQuest : StoryModeQuestBase
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00008A1B File Offset: 0x00006C1B
		private TextObject _startQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=bNqLQKQS}You are out of food. There is a village called {VILLAGE_NAME} north of here where you can buy provisions and find some help.", null);
				textObject.SetTextVariable("VILLAGE_NAME", this._questVillage.Name);
				return textObject;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00008A3F File Offset: 0x00006C3F
		private TextObject _endQuestLog
		{
			get
			{
				return new TextObject("{=7VFLb3Qj}You have arrived at the village.", null);
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600017E RID: 382 RVA: 0x00008A4C File Offset: 0x00006C4C
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=oa4XFhve}Travel To Village {VILLAGE_NAME}", null);
				textObject.SetTextVariable("VILLAGE_NAME", this._questVillage.Name);
				return textObject;
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00008A70 File Offset: 0x00006C70
		public TravelToVillageTutorialQuest()
			: base("travel_to_village_tutorial_quest", null, CampaignTime.Never)
		{
			this._questVillage = Settlement.Find("village_ES3_2");
			base.AddTrackedObject(this._questVillage);
			this._refugeeParties = new MobileParty[4];
			TextObject textObject = new TextObject("{=3YHL3wpM}{BROTHER.NAME}:", null);
			TextObjectExtensions.SetCharacterProperties(textObject, "BROTHER", StoryModeHeroes.ElderBrother.CharacterObject, false);
			InformationManager.ShowInquiry(new InquiryData(textObject.ToString(), new TextObject("{=dE2ufxte}Before we do anything else... We're low on food. There's a village north of here where we can buy provisions and find some help. You're a better rider than I am so I'll let you lead the way...", null).ToString(), true, false, new TextObject("{=JOJ09cLW}Let's go.", null).ToString(), null, delegate
			{
				StoryModeEvents.Instance.OnTravelToVillageTutorialQuestStarted();
			}, null, "", 0f, null, null, null), false, false);
			this.SetDialogs();
			base.InitializeQuestOnCreation();
			base.AddLog(this._startQuestLog, false);
			TutorialPhase.Instance.SetTutorialFocusSettlement(this._questVillage);
			this.CreateRefugeeParties();
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00008B68 File Offset: 0x00006D68
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00008B70 File Offset: 0x00006D70
		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("{=MDtTC5j5}Don't hurt us![ib:weary][if:convo_confused_annoyed]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.news_about_raiders_condition))
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.news_about_raiders_consequence))
				.PlayerLine(new TextObject("{=pX5cx3b4}I mean you no harm. We're hunting a group of raiders who took our brother and sister.", null), null)
				.NpcLine(new TextObject("{=ajBBFq1D}Aii... Those devils. They raided our village. Took whoever they could catch. Slavers, I'll bet.[if:convo_grave][ib:nervous]", null), null, null)
				.NpcLine(new TextObject("{=AhthUkMu}People say they're still about. We're sleeping in the woods, not going back until they're gone. You hunt them down and kill every one, you hear! Heaven protect you! Heaven guide your swords![if:convo_predatory][ib:normal]", null), null, null)
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000020).NpcLine(new TextObject("{=pa9LrHln}We're here, I guess. So... We need food, and after that, maybe some men to come with us.", null), null, null).Condition(() => Settlement.CurrentSettlement != null && Settlement.CurrentSettlement == this._questVillage && Hero.OneToOneConversationHero == StoryModeHeroes.ElderBrother)
				.NpcLine(new TextObject("{=p0fmZY5r}The headman here can probably help us. Let's try to find him...", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.talk_with_brother_consequence))
				.CloseDialog(), this);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00008C74 File Offset: 0x00006E74
		private bool news_about_raiders_condition()
		{
			return Settlement.CurrentSettlement == null && MobileParty.ConversationParty != null && this._refugeeParties.Contains(MobileParty.ConversationParty);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00008C96 File Offset: 0x00006E96
		private void news_about_raiders_consequence()
		{
			PlayerEncounter.LeaveEncounter = true;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00008C9E File Offset: 0x00006E9E
		private void talk_with_brother_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += base.CompleteQuestWithSuccess;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00008CBC File Offset: 0x00006EBC
		protected override void RegisterEvents()
		{
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.BeforeMissionOpenedEvent.AddNonSerializedListener(this, new Action(this.OnBeforeMissionOpened));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			StoryModeEvents.OnTravelToVillageTutorialQuestStartedEvent.AddNonSerializedListener(this, new Action(this.OnTravelToVillageTutorialQuestStarted));
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00008D28 File Offset: 0x00006F28
		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (!TutorialPhase.Instance.IsCompleted && Settlement.CurrentSettlement == null && PlayerEncounter.EncounteredParty != null && args.MenuContext.GameMenu.StringId != "encounter_meeting" && args.MenuContext.GameMenu.StringId != "encounter")
			{
				if (this._refugeeParties.Contains(PlayerEncounter.EncounteredMobileParty))
				{
					GameMenu.SwitchToMenu("encounter_meeting");
					return;
				}
				PlayerEncounter.Finish(true);
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=EWD4Op6d}Notification", null).ToString(), new TextObject("{=pVKkclVk}Interactions are limited during tutorial phase. This interaction is disabled.", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00008E04 File Offset: 0x00007004
		private void OnBeforeMissionOpened()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement == Settlement.Find("village_ES3_2"))
			{
				int hitPoints = StoryModeHeroes.ElderBrother.HitPoints;
				int num = 50;
				if (hitPoints < num)
				{
					int num2 = num - hitPoints;
					StoryModeHeroes.ElderBrother.Heal(num2, false);
				}
				LocationCharacter locationCharacterOfHero = LocationComplex.Current.GetLocationCharacterOfHero(StoryModeHeroes.ElderBrother);
				PlayerEncounter.LocationEncounter.AddAccompanyingCharacter(locationCharacterOfHero, true);
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00008E68 File Offset: 0x00007068
		private void DailyTick()
		{
			for (int i = 0; i < this._refugeeParties.Length; i++)
			{
				if (this._refugeeParties[i].Party.IsStarving)
				{
					this._refugeeParties[i].Party.ItemRoster.AddToCounts(DefaultItems.Grain, 2);
				}
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00008EBC File Offset: 0x000070BC
		private void OnTravelToVillageTutorialQuestStarted()
		{
			MapState mapState;
			if ((mapState = GameStateManager.Current.ActiveState as MapState) != null)
			{
				mapState.Handler.StartCameraAnimation(this._questVillage.GatePosition, 1f);
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00008EF8 File Offset: 0x000070F8
		private void CreateRefugeeParties()
		{
			int i;
			int j;
			for (i = 0; i < 4; i = j + 1)
			{
				MobileParty.CreateParty("travel_to_village_quest_refuge_party_" + i, null, delegate(MobileParty party)
				{
					this.OnRefugeePartyCreated(party, i);
				});
				j = i;
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00008F5C File Offset: 0x0000715C
		private void OnRefugeePartyCreated(MobileParty refugeeParty, int index)
		{
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("storymode_quest_refugee_female");
			CharacterObject object2 = MBObjectManager.Instance.GetObject<CharacterObject>("storymode_quest_refugee_male");
			TroopRoster troopRoster = new TroopRoster(refugeeParty.Party);
			int num = MBRandom.RandomInt(6, 12);
			for (int i = 0; i < num; i++)
			{
				troopRoster.AddToCounts((MBRandom.RandomFloat < 0.5f) ? @object : object2, 1, false, 0, 0, true, -1);
			}
			refugeeParty.InitializeMobilePartyAroundPosition(troopRoster, new TroopRoster(refugeeParty.Party), this._questVillage.Position2D, MobileParty.MainParty.SeeingRange, 0f);
			refugeeParty.SetCustomName(new TextObject("{=7FWF01bW}Refugees", null));
			refugeeParty.InitializePartyTrade(200);
			refugeeParty.Party.SetCustomOwner(this._questVillage.OwnerClan.Leader);
			refugeeParty.SetCustomHomeSettlement(this._questVillage);
			SetPartyAiAction.GetActionForPatrollingAroundSettlement(refugeeParty, this._questVillage);
			refugeeParty.Ai.SetDoNotMakeNewDecisions(true);
			refugeeParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
			refugeeParty.SetPartyUsedByQuest(true);
			refugeeParty.Party.ItemRoster.AddToCounts(DefaultItems.Grain, 2);
			this._refugeeParties[index] = refugeeParty;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00009084 File Offset: 0x00007284
		protected override void OnCompleteWithSuccess()
		{
			foreach (MobileParty mobileParty in this._refugeeParties.ToList<MobileParty>())
			{
				DestroyPartyAction.Apply(null, mobileParty);
			}
			base.AddLog(this._endQuestLog, false);
			TutorialPhase.Instance.RemoveTutorialFocusSettlement();
		}

		// Token: 0x0600018D RID: 397 RVA: 0x000090F4 File Offset: 0x000072F4
		internal static void AutoGeneratedStaticCollectObjectsTravelToVillageTutorialQuest(object o, List<object> collectedObjects)
		{
			((TravelToVillageTutorialQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00009102 File Offset: 0x00007302
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._questVillage);
			collectedObjects.Add(this._refugeeParties);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00009123 File Offset: 0x00007323
		internal static object AutoGeneratedGetMemberValue_questVillage(object o)
		{
			return ((TravelToVillageTutorialQuest)o)._questVillage;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00009130 File Offset: 0x00007330
		internal static object AutoGeneratedGetMemberValue_refugeeParties(object o)
		{
			return ((TravelToVillageTutorialQuest)o)._refugeeParties;
		}

		// Token: 0x0400008C RID: 140
		private const int RefugePartyCount = 4;

		// Token: 0x0400008D RID: 141
		[SaveableField(1)]
		private Settlement _questVillage;

		// Token: 0x0400008E RID: 142
		[SaveableField(2)]
		private readonly MobileParty[] _refugeeParties;
	}
}
