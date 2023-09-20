using System;
using System.Collections.Generic;
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
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.FirstPhase
{
	public class ArzagosBannerPieceQuest : StoryModeQuestBase
	{
		private TextObject _startQuestLog
		{
			get
			{
				return new TextObject("{=wvHvnEog}Find the hideout that Arzagos told you about and get the next banner piece.", null);
			}
		}

		public override TextObject Title
		{
			get
			{
				return new TextObject("{=ay1gPPsP}Find another piece of the banner for Arzagos", null);
			}
		}

		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		public ArzagosBannerPieceQuest(Hero questGiver, Settlement hideout)
			: base("arzagos_banner_piece_quest", questGiver, StoryModeManager.Current.MainStoryLine.FirstPhase.FirstPhaseEndTime)
		{
			this._hideout = hideout;
			StoryModeManager.Current.MainStoryLine.BusyHideouts.Add(this._hideout.Hideout);
			this._raiderParties = new List<MobileParty>();
			this.InitializeHideout();
			base.AddTrackedObject(this._hideout);
			this.SetDialogs();
			base.InitializeQuestOnCreation();
			base.AddLog(this._startQuestLog, false);
			this._hideoutBattleEndState = ArzagosBannerPieceQuest.HideoutBattleEndState.None;
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			StoryModeManager.Current.MainStoryLine.BusyHideouts.Remove(this._hideout.Hideout);
		}

		protected override void OnStartQuest()
		{
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
		}

		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("hero_main_options", 100).PlayerLine(new TextObject("{=dlBFVkDj}About the task you gave me...", null), null).Condition(new ConversationSentence.OnConditionDelegate(this.conversation_lord_task_given_on_condition))
				.NpcLine(new TextObject("{=a0JxUMgo}What happened? Did you find the piece of the banner?", null), null, null)
				.Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
				.PlayerLine(new TextObject("{=rY0fdQSb}No, I am still working on it...", null), null)
				.CloseDialog(), this);
		}

		private bool conversation_lord_task_given_on_condition()
		{
			return Hero.OneToOneConversationHero == base.QuestGiver && base.IsOngoing;
		}

		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
			StoryModeManager.Current.MainStoryLine.BusyHideouts.Add(this._hideout.Hideout);
		}

		private void InitializeHideout()
		{
			this._hideout.Hideout.IsSpotted = true;
			this._hideout.IsVisible = true;
			this._hideoutBattleEndState = ArzagosBannerPieceQuest.HideoutBattleEndState.None;
			if (!this._hideout.Hideout.IsInfested)
			{
				for (int i = 0; i < 2; i++)
				{
					if (!this._hideout.Hideout.IsInfested)
					{
						this._raiderParties.Add(this.CreateRaiderParty(i));
					}
				}
			}
		}

		private MobileParty CreateRaiderParty(int number)
		{
			MobileParty mobileParty = BanditPartyComponent.CreateBanditParty("arzagos_banner_piece_quest_raider_party_" + number, StoryModeHeroes.RadiersClan, this._hideout.Hideout, false);
			TroopRoster troopRoster = new TroopRoster(mobileParty.Party);
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>(this._hideout.Culture.StringId + "_bandit");
			troopRoster.AddToCounts(@object, 5, false, 0, 0, true, -1);
			TroopRoster troopRoster2 = new TroopRoster(mobileParty.Party);
			mobileParty.InitializeMobilePartyAtPosition(troopRoster, troopRoster2, this._hideout.Position2D);
			mobileParty.SetCustomName(new TextObject("{=u1Pkt4HC}Raiders", null));
			mobileParty.ActualClan = StoryModeHeroes.RadiersClan;
			mobileParty.Position2D = this._hideout.Position2D;
			mobileParty.Party.Visuals.SetMapIconAsDirty();
			float totalStrength = mobileParty.Party.TotalStrength;
			int num = (int)(1f * MBRandom.RandomFloat * 20f * totalStrength + 50f);
			mobileParty.InitializePartyTrade(num);
			mobileParty.Ai.SetMoveGoToSettlement(this._hideout);
			mobileParty.Ai.SetDoNotMakeNewDecisions(true);
			mobileParty.SetPartyUsedByQuest(true);
			EnterSettlementAction.ApplyForParty(mobileParty, this._hideout);
			return mobileParty;
		}

		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (PlayerEncounter.Current != null && mapEvent.IsPlayerMapEvent && Settlement.CurrentSettlement == this._hideout)
			{
				if (mapEvent.WinningSide == mapEvent.PlayerSide)
				{
					this._hideoutBattleEndState = ArzagosBannerPieceQuest.HideoutBattleEndState.Victory;
					return;
				}
				if (mapEvent.WinningSide == -1)
				{
					this._hideoutBattleEndState = ArzagosBannerPieceQuest.HideoutBattleEndState.Retreated;
					if (Hero.MainHero.IsPrisoner && this._raiderParties.Contains(Hero.MainHero.PartyBelongedToAsPrisoner.MobileParty))
					{
						EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
						if (Hero.MainHero.HitPoints < 50)
						{
							Hero.MainHero.Heal(50 - Hero.MainHero.HitPoints, false);
						}
						InformationManager.ShowInquiry(new InquiryData(new TextObject("{=FPhWhjq7}Defeated", null).ToString(), new TextObject("{=6iytd81P}You are defeated by the bandits in the hideout but you managed to escape. You need to wait a while before attacking again.", null).ToString(), true, false, new TextObject("{=yQtzabbe}Close", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
					}
					if (this._hideout.Parties.Count == 0)
					{
						this.InitializeHideout();
					}
					this._hideoutBattleEndState = ArzagosBannerPieceQuest.HideoutBattleEndState.None;
					return;
				}
				this._hideoutBattleEndState = ArzagosBannerPieceQuest.HideoutBattleEndState.Defeated;
			}
		}

		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement == this._hideout && !this._hideout.Hideout.IsInfested)
			{
				this.InitializeHideout();
			}
			if (this._hideoutBattleEndState == ArzagosBannerPieceQuest.HideoutBattleEndState.Victory)
			{
				FirstPhase.Instance.CollectBannerPiece();
				base.CompleteQuestWithSuccess();
				this._hideoutBattleEndState = ArzagosBannerPieceQuest.HideoutBattleEndState.None;
				return;
			}
			if (this._hideoutBattleEndState == ArzagosBannerPieceQuest.HideoutBattleEndState.Retreated || this._hideoutBattleEndState == ArzagosBannerPieceQuest.HideoutBattleEndState.Defeated)
			{
				if (Hero.MainHero.IsPrisoner)
				{
					EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
					if (Hero.MainHero.HitPoints < 50)
					{
						Hero.MainHero.Heal(50 - Hero.MainHero.HitPoints, false);
					}
					InformationManager.ShowInquiry(new InquiryData(new TextObject("{=FPhWhjq7}Defeated", null).ToString(), new TextObject("{=btAV7mmq}You are defeated by the raiders in the hideout but you managed to escape. You need to wait a while before attacking again.", null).ToString(), true, false, new TextObject("{=yQtzabbe}Close", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
				}
				if (this._hideout.Parties.Count == 0)
				{
					this.InitializeHideout();
				}
				this._hideoutBattleEndState = ArzagosBannerPieceQuest.HideoutBattleEndState.None;
			}
		}

		internal static void AutoGeneratedStaticCollectObjectsArzagosBannerPieceQuest(object o, List<object> collectedObjects)
		{
			((ArzagosBannerPieceQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._hideout);
			collectedObjects.Add(this._raiderParties);
		}

		internal static object AutoGeneratedGetMemberValue_hideout(object o)
		{
			return ((ArzagosBannerPieceQuest)o)._hideout;
		}

		internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
		{
			return ((ArzagosBannerPieceQuest)o)._raiderParties;
		}

		internal static object AutoGeneratedGetMemberValue_hideoutBattleEndState(object o)
		{
			return ((ArzagosBannerPieceQuest)o)._hideoutBattleEndState;
		}

		private const int MainPartyHealHitPointLimit = 50;

		private const int RaiderPartySize = 10;

		private const int RaiderPartyCount = 2;

		private const string ArzagosRaiderPartyStringId = "arzagos_banner_piece_quest_raider_party_";

		[SaveableField(1)]
		private readonly Settlement _hideout;

		[SaveableField(2)]
		private readonly List<MobileParty> _raiderParties;

		[SaveableField(3)]
		private ArzagosBannerPieceQuest.HideoutBattleEndState _hideoutBattleEndState;

		public enum HideoutBattleEndState
		{
			None,
			Retreated,
			Defeated,
			Victory
		}
	}
}
