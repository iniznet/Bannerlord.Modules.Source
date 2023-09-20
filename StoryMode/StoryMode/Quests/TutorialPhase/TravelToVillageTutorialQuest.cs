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
	public class TravelToVillageTutorialQuest : StoryModeQuestBase
	{
		private TextObject _startQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=bNqLQKQS}You are out of food. There is a village called {VILLAGE_NAME} north of here where you can buy provisions and find some help.", null);
				textObject.SetTextVariable("VILLAGE_NAME", this._questVillage.Name);
				return textObject;
			}
		}

		private TextObject _endQuestLog
		{
			get
			{
				return new TextObject("{=7VFLb3Qj}You have arrived at the village.", null);
			}
		}

		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=oa4XFhve}Travel To Village {VILLAGE_NAME}", null);
				textObject.SetTextVariable("VILLAGE_NAME", this._questVillage.Name);
				return textObject;
			}
		}

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

		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		protected override void HourlyTick()
		{
		}

		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("{=MDtTC5j5}Don't hurt us![ib:nervous][if:convo_nervous]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.news_about_raiders_condition))
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.news_about_raiders_consequence))
				.PlayerLine(new TextObject("{=pX5cx3b4}I mean you no harm. We're hunting a group of raiders who took our brother and sister.", null), null)
				.NpcLine(new TextObject("{=ajBBFq1D}Aii... Those devils. They raided our village. Took whoever they could catch. Slavers, I'll bet.[if:convo_nervous][ib:nervous2]", null), null, null)
				.NpcLine(new TextObject("{=AhthUkMu}People say they're still about. We're sleeping in the woods, not going back until they're gone. You hunt them down and kill every one, you hear! Heaven protect you! Heaven guide your swords![if:convo_nervous2][ib:nervous]", null), null, null)
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000020).NpcLine(new TextObject("{=pa9LrHln}We're here, I guess. So... We need food, and after that, maybe some men to come with us.[if:convo_thinking]", null), null, null).Condition(() => Settlement.CurrentSettlement != null && Settlement.CurrentSettlement == this._questVillage && Hero.OneToOneConversationHero == StoryModeHeroes.ElderBrother)
				.NpcLine(new TextObject("{=p0fmZY5r}The headman here can probably help us. Let's try to find him...[if:convo_pondering]", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.talk_with_brother_consequence))
				.CloseDialog(), this);
		}

		private bool news_about_raiders_condition()
		{
			return Settlement.CurrentSettlement == null && MobileParty.ConversationParty != null && this._refugeeParties.Contains(MobileParty.ConversationParty);
		}

		private void news_about_raiders_consequence()
		{
			PlayerEncounter.LeaveEncounter = true;
		}

		private void talk_with_brother_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += base.CompleteQuestWithSuccess;
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.BeforeMissionOpenedEvent.AddNonSerializedListener(this, new Action(this.OnBeforeMissionOpened));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			StoryModeEvents.OnTravelToVillageTutorialQuestStartedEvent.AddNonSerializedListener(this, new Action(this.OnTravelToVillageTutorialQuestStarted));
		}

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

		private void OnTravelToVillageTutorialQuestStarted()
		{
			MapState mapState;
			if ((mapState = GameStateManager.Current.ActiveState as MapState) != null)
			{
				mapState.Handler.StartCameraAnimation(this._questVillage.GatePosition, 1f);
			}
		}

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

		protected override void OnCompleteWithSuccess()
		{
			foreach (MobileParty mobileParty in this._refugeeParties.ToList<MobileParty>())
			{
				DestroyPartyAction.Apply(null, mobileParty);
			}
			base.AddLog(this._endQuestLog, false);
			TutorialPhase.Instance.RemoveTutorialFocusSettlement();
		}

		internal static void AutoGeneratedStaticCollectObjectsTravelToVillageTutorialQuest(object o, List<object> collectedObjects)
		{
			((TravelToVillageTutorialQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._questVillage);
			collectedObjects.Add(this._refugeeParties);
		}

		internal static object AutoGeneratedGetMemberValue_questVillage(object o)
		{
			return ((TravelToVillageTutorialQuest)o)._questVillage;
		}

		internal static object AutoGeneratedGetMemberValue_refugeeParties(object o)
		{
			return ((TravelToVillageTutorialQuest)o)._refugeeParties;
		}

		private const int RefugePartyCount = 4;

		[SaveableField(1)]
		private Settlement _questVillage;

		[SaveableField(2)]
		private readonly MobileParty[] _refugeeParties;
	}
}
