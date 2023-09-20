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
	// Token: 0x0200002F RID: 47
	public class ArzagosBannerPieceQuest : StoryModeQuestBase
	{
		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000277 RID: 631 RVA: 0x0000D628 File Offset: 0x0000B828
		private TextObject _startQuestLog
		{
			get
			{
				return new TextObject("{=wvHvnEog}Find the hideout that Arzagos told you about and get the next banner piece.", null);
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000278 RID: 632 RVA: 0x0000D635 File Offset: 0x0000B835
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=ay1gPPsP}Find another piece of the banner for Arzagos", null);
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000279 RID: 633 RVA: 0x0000D642 File Offset: 0x0000B842
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000D648 File Offset: 0x0000B848
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

		// Token: 0x0600027B RID: 635 RVA: 0x0000D6D9 File Offset: 0x0000B8D9
		protected override void OnFinalize()
		{
			base.OnFinalize();
			StoryModeManager.Current.MainStoryLine.BusyHideouts.Remove(this._hideout.Hideout);
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000D701 File Offset: 0x0000B901
		protected override void OnStartQuest()
		{
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000D703 File Offset: 0x0000B903
		protected override void RegisterEvents()
		{
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000D734 File Offset: 0x0000B934
		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("hero_main_options", 100).PlayerLine(new TextObject("{=dlBFVkDj}About the task you gave me...", null), null).Condition(new ConversationSentence.OnConditionDelegate(this.conversation_lord_task_given_on_condition))
				.NpcLine(new TextObject("{=a0JxUMgo}What happened? Did you find the piece of the banner?", null), null, null)
				.Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
				.PlayerLine(new TextObject("{=rY0fdQSb}No, I am still working on it...", null), null)
				.CloseDialog(), this);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000D7B8 File Offset: 0x0000B9B8
		private bool conversation_lord_task_given_on_condition()
		{
			return Hero.OneToOneConversationHero == base.QuestGiver && base.IsOngoing;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000D7CF File Offset: 0x0000B9CF
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
			StoryModeManager.Current.MainStoryLine.BusyHideouts.Add(this._hideout.Hideout);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000D7F8 File Offset: 0x0000B9F8
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

		// Token: 0x06000282 RID: 642 RVA: 0x0000D86C File Offset: 0x0000BA6C
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

		// Token: 0x06000283 RID: 643 RVA: 0x0000D9A0 File Offset: 0x0000BBA0
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

		// Token: 0x06000284 RID: 644 RVA: 0x0000DACC File Offset: 0x0000BCCC
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

		// Token: 0x06000285 RID: 645 RVA: 0x0000DBDE File Offset: 0x0000BDDE
		internal static void AutoGeneratedStaticCollectObjectsArzagosBannerPieceQuest(object o, List<object> collectedObjects)
		{
			((ArzagosBannerPieceQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000DBEC File Offset: 0x0000BDEC
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._hideout);
			collectedObjects.Add(this._raiderParties);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000DC0D File Offset: 0x0000BE0D
		internal static object AutoGeneratedGetMemberValue_hideout(object o)
		{
			return ((ArzagosBannerPieceQuest)o)._hideout;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000DC1A File Offset: 0x0000BE1A
		internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
		{
			return ((ArzagosBannerPieceQuest)o)._raiderParties;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000DC27 File Offset: 0x0000BE27
		internal static object AutoGeneratedGetMemberValue_hideoutBattleEndState(object o)
		{
			return ((ArzagosBannerPieceQuest)o)._hideoutBattleEndState;
		}

		// Token: 0x040000C6 RID: 198
		private const int MainPartyHealHitPointLimit = 50;

		// Token: 0x040000C7 RID: 199
		private const int RaiderPartySize = 10;

		// Token: 0x040000C8 RID: 200
		private const int RaiderPartyCount = 2;

		// Token: 0x040000C9 RID: 201
		private const string ArzagosRaiderPartyStringId = "arzagos_banner_piece_quest_raider_party_";

		// Token: 0x040000CA RID: 202
		[SaveableField(1)]
		private readonly Settlement _hideout;

		// Token: 0x040000CB RID: 203
		[SaveableField(2)]
		private readonly List<MobileParty> _raiderParties;

		// Token: 0x040000CC RID: 204
		[SaveableField(3)]
		private ArzagosBannerPieceQuest.HideoutBattleEndState _hideoutBattleEndState;

		// Token: 0x02000073 RID: 115
		public enum HideoutBattleEndState
		{
			// Token: 0x04000232 RID: 562
			None,
			// Token: 0x04000233 RID: 563
			Retreated,
			// Token: 0x04000234 RID: 564
			Defeated,
			// Token: 0x04000235 RID: 565
			Victory
		}
	}
}
