using System;
using System.Linq;
using Helpers;
using StoryMode.Quests.FirstPhase;
using StoryMode.Quests.PlayerClanQuests;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	public class FirstPhaseCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.BeforeMissionOpenedEvent.AddNonSerializedListener(this, new Action(this.OnBeforeMissionOpened));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			StoryModeEvents.OnBannerPieceCollectedEvent.AddNonSerializedListener(this, new Action(this.OnBannerPieceCollected));
			StoryModeEvents.OnStoryModeTutorialEndedEvent.AddNonSerializedListener(this, new Action(this.OnStoryModeTutorialEnded));
			StoryModeEvents.OnMainStoryLineSideChosenEvent.AddNonSerializedListener(this, new Action<MainStoryLineSide>(this.OnMainStoryLineSideChosen));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Location>("_imperialMentorHouse", ref this._imperialMentorHouse);
			dataStore.SyncData<Location>("_antiImperialMentorHouse", ref this._antiImperialMentorHouse);
			dataStore.SyncData<bool>("_popUpShowed", ref this._popUpShowed);
		}

		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this.SpawnMentorsIfNeeded();
		}

		private void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			Settlement settlement = Settlement.FindFirst((Settlement s) => s.IsTown && !s.IsUnderSiege && s.Culture.StringId == "empire");
			this._imperialMentorHouse = this.ReserveHouseForMentor(StoryModeHeroes.ImperialMentor, settlement);
			Settlement settlement2 = Settlement.FindFirst((Settlement s) => s.IsTown && !s.IsUnderSiege && s.Culture.StringId == "battania");
			this._antiImperialMentorHouse = this.ReserveHouseForMentor(StoryModeHeroes.AntiImperialMentor, settlement2);
			StoryModeManager.Current.MainStoryLine.SetMentorSettlements(settlement, settlement2);
		}

		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			if (detail == 1)
			{
				if (quest is BannerInvestigationQuest)
				{
					new MeetWithIstianaQuest(StoryModeManager.Current.MainStoryLine.ImperialMentorSettlement).StartQuest();
					new MeetWithArzagosQuest(StoryModeManager.Current.MainStoryLine.AntiImperialMentorSettlement).StartQuest();
					return;
				}
				if (quest is MeetWithIstianaQuest)
				{
					Hero imperialMentor = StoryModeHeroes.ImperialMentor;
					new IstianasBannerPieceQuest(imperialMentor, this.FindSuitableHideout(imperialMentor)).StartQuest();
					return;
				}
				if (quest is MeetWithArzagosQuest)
				{
					Hero antiImperialMentor = StoryModeHeroes.AntiImperialMentor;
					new ArzagosBannerPieceQuest(antiImperialMentor, this.FindSuitableHideout(antiImperialMentor)).StartQuest();
				}
			}
		}

		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			this.SpawnMentorsIfNeeded();
		}

		private void OnBeforeMissionOpened()
		{
			this.SpawnMentorsIfNeeded();
		}

		private void SpawnMentorsIfNeeded()
		{
			if (this._imperialMentorHouse != null && this._antiImperialMentorHouse != null && Settlement.CurrentSettlement != null && (StoryModeHeroes.ImperialMentor.CurrentSettlement == Settlement.CurrentSettlement || StoryModeHeroes.AntiImperialMentor.CurrentSettlement == Settlement.CurrentSettlement))
			{
				this.SpawnMentorInHouse(Settlement.CurrentSettlement);
			}
		}

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (settlement.StringId == "tutorial_training_field" && party == MobileParty.MainParty && TutorialPhase.Instance.TutorialQuestPhase == TutorialQuestPhase.Finalized && !this._popUpShowed && TutorialPhase.Instance.IsSkipped)
			{
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=EWD4Op6d}Notification", null).ToString(), new TextObject("{=lJiEsNiQ}A few hours after you leave the training ground, you come across a wounded man lying under a tree. You share your water with him and try to dress his wounds as best as you can. He tells you he is a traveling doctor. To thank you for your help, he hands you a small bronze artifact which he says was once given to him in payment by a warrior who said only that it was related to 'Neretzes' Folly.' He suspects it might be of great value. You resolve to find out more.", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, delegate
				{
					this._popUpShowed = true;
					CampaignEventDispatcher.Instance.RemoveListeners(Campaign.Current.GetCampaignBehavior<TutorialPhaseCampaignBehavior>());
					MBInformationManager.ShowSceneNotification(new FindingFirstBannerPieceSceneNotificationItem(Hero.MainHero, new Action(this.OnPieceFoundAction)));
				}, null, "", 0f, null, null, null), false, false);
			}
		}

		private void OnPieceFoundAction()
		{
			this.SelectClanName();
		}

		private void OnStoryModeTutorialEnded()
		{
			new RebuildPlayerClanQuest().StartQuest();
			new BannerInvestigationQuest().StartQuest();
		}

		private void OnBannerPieceCollected()
		{
			TextObject textObject = new TextObject("{=Pus87ZW2}You've found the {BANNER_PIECE_COUNT} banner piece!", null);
			if (FirstPhase.Instance == null || FirstPhase.Instance.CollectedBannerPieceCount == 1)
			{
				textObject.SetTextVariable("BANNER_PIECE_COUNT", new TextObject("{=oAoTaAWg}first", null));
			}
			else if (FirstPhase.Instance.CollectedBannerPieceCount == 2)
			{
				textObject.SetTextVariable("BANNER_PIECE_COUNT", new TextObject("{=9ZyXl25X}second", null));
			}
			else if (FirstPhase.Instance.CollectedBannerPieceCount == 3)
			{
				textObject.SetTextVariable("BANNER_PIECE_COUNT", new TextObject("{=4cw169Kb}third and the final", null));
			}
			MBInformationManager.AddQuickInformation(textObject, 0, null, "");
		}

		private void OnMainStoryLineSideChosen(MainStoryLineSide side)
		{
			this._imperialMentorHouse.RemoveReservation();
			this._imperialMentorHouse = null;
			this._antiImperialMentorHouse.RemoveReservation();
			this._antiImperialMentorHouse = null;
		}

		private void SelectClanName()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=JJiKk4ow}Select your family name: ", null).ToString(), string.Empty, true, false, GameTexts.FindText("str_done", null).ToString(), null, new Action<string>(this.OnChangeClanNameDone), null, false, new Func<string, Tuple<bool, string>>(FactionHelper.IsClanNameApplicable), "", Clan.PlayerClan.Name.ToString()), false, false);
		}

		private void OnChangeClanNameDone(string newClanName)
		{
			TextObject textObject = GameTexts.FindText("str_generic_clan_name", null);
			textObject.SetTextVariable("CLAN_NAME", new TextObject(newClanName, null));
			Clan.PlayerClan.InitializeClan(textObject, textObject, Clan.PlayerClan.Culture, Clan.PlayerClan.Banner, default(Vec2), false);
			this.OpenBannerSelectionScreen();
		}

		private void OpenBannerSelectionScreen()
		{
			Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<BannerEditorState>(), 0);
		}

		private Settlement FindSuitableHideout(Hero questGiver)
		{
			Settlement settlement = null;
			float num = float.MaxValue;
			foreach (Hideout hideout in Hideout.All)
			{
				if (!StoryModeManager.Current.MainStoryLine.BusyHideouts.Contains(hideout))
				{
					float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(hideout.Settlement, questGiver.CurrentSettlement);
					if (distance < num)
					{
						num = distance;
						settlement = hideout.Settlement;
					}
				}
			}
			return settlement;
		}

		private void SpawnMentorInHouse(Settlement settlement)
		{
			Hero hero = ((StoryModeHeroes.ImperialMentor.CurrentSettlement == settlement) ? StoryModeHeroes.ImperialMentor : StoryModeHeroes.AntiImperialMentor);
			Location location = ((StoryModeHeroes.ImperialMentor.CurrentSettlement == settlement) ? this._imperialMentorHouse : this._antiImperialMentorHouse);
			CharacterObject characterObject = hero.CharacterObject;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(characterObject.Race, "_settlement");
			LocationCharacter locationCharacter = new LocationCharacter(new AgentData(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common", true, 0, null, true, false, null, false, false, true);
			location.AddCharacter(locationCharacter);
		}

		private Location ReserveHouseForMentor(Hero mentor, Settlement settlement)
		{
			MBList<Location> mblist = new MBList<Location>();
			mblist.Add(settlement.LocationComplex.GetLocationWithId("house_1"));
			mblist.Add(settlement.LocationComplex.GetLocationWithId("house_2"));
			mblist.Add(settlement.LocationComplex.GetLocationWithId("house_3"));
			object obj = mblist.First((Location h) => !h.IsReserved) ?? Extensions.GetRandomElement<Location>(mblist);
			TextObject textObject = new TextObject("{=EZ19JOGj}{MENTOR.NAME}'s House", null);
			StringHelpers.SetCharacterProperties("MENTOR", mentor.CharacterObject, textObject, false);
			object obj2 = obj;
			obj2.ReserveLocation(textObject, textObject);
			return obj2;
		}

		private Location _imperialMentorHouse;

		private Location _antiImperialMentorHouse;

		private bool _popUpShowed;
	}
}
