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
	// Token: 0x02000033 RID: 51
	public class IstianasBannerPieceQuest : StoryModeQuestBase
	{
		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060002EC RID: 748 RVA: 0x000101E1 File Offset: 0x0000E3E1
		private TextObject _startQuestLog
		{
			get
			{
				return new TextObject("{=GxlPj4GC}Find the hideout that Istiana told you about and get the next banner piece.", null);
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060002ED RID: 749 RVA: 0x000101EE File Offset: 0x0000E3EE
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=WTjAYUoD}Find another piece of the banner for Istiana", null);
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002EE RID: 750 RVA: 0x000101FB File Offset: 0x0000E3FB
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00010200 File Offset: 0x0000E400
		public IstianasBannerPieceQuest(Hero questGiver, Settlement hideout)
			: base("istiana_banner_piece_quest", questGiver, StoryModeManager.Current.MainStoryLine.FirstPhase.FirstPhaseEndTime)
		{
			this._hideout = hideout;
			StoryModeManager.Current.MainStoryLine.BusyHideouts.Add(this._hideout.Hideout);
			this._raiderParties = new List<MobileParty>();
			this.InitializeHideout();
			base.AddTrackedObject(this._hideout);
			this.SetDialogs();
			base.InitializeQuestOnCreation();
			base.AddLog(this._startQuestLog, false);
			this._hideoutBattleEndState = IstianasBannerPieceQuest.HideoutBattleEndState.None;
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00010291 File Offset: 0x0000E491
		protected override void OnFinalize()
		{
			base.OnFinalize();
			StoryModeManager.Current.MainStoryLine.BusyHideouts.Remove(this._hideout.Hideout);
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x000102B9 File Offset: 0x0000E4B9
		protected override void OnStartQuest()
		{
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x000102BB File Offset: 0x0000E4BB
		protected override void RegisterEvents()
		{
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x000102EC File Offset: 0x0000E4EC
		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("hero_main_options", 100).PlayerLine(new TextObject("{=dlBFVkDj}About the task you gave me...", null), null).Condition(new ConversationSentence.OnConditionDelegate(this.conversation_lord_task_given_on_condition))
				.NpcLine(new TextObject("{=F26iH45g}What happened? Have you assembled the banner?", null), null, null)
				.Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
				.PlayerLine(new TextObject("{=rY0fdQSb}No, I am still working on it...", null), null)
				.CloseDialog(), this);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00010370 File Offset: 0x0000E570
		private bool conversation_lord_task_given_on_condition()
		{
			return Hero.OneToOneConversationHero == base.QuestGiver && base.IsOngoing;
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00010387 File Offset: 0x0000E587
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
			StoryModeManager.Current.MainStoryLine.BusyHideouts.Add(this._hideout.Hideout);
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x000103B0 File Offset: 0x0000E5B0
		private void InitializeHideout()
		{
			this._hideout.Hideout.IsSpotted = true;
			this._hideout.IsVisible = true;
			this._hideoutBattleEndState = IstianasBannerPieceQuest.HideoutBattleEndState.None;
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

		// Token: 0x060002F7 RID: 759 RVA: 0x00010424 File Offset: 0x0000E624
		private MobileParty CreateRaiderParty(int number)
		{
			MobileParty mobileParty = BanditPartyComponent.CreateBanditParty("istiana_banner_piece_quest_raider_party_" + number, StoryModeHeroes.RadiersClan, this._hideout.Hideout, false);
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

		// Token: 0x060002F8 RID: 760 RVA: 0x00010558 File Offset: 0x0000E758
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (PlayerEncounter.Current != null && mapEvent.IsPlayerMapEvent && Settlement.CurrentSettlement == this._hideout)
			{
				if (mapEvent.WinningSide == mapEvent.PlayerSide)
				{
					this._hideoutBattleEndState = IstianasBannerPieceQuest.HideoutBattleEndState.Victory;
					return;
				}
				if (mapEvent.WinningSide == -1)
				{
					this._hideoutBattleEndState = IstianasBannerPieceQuest.HideoutBattleEndState.Retreated;
					if (Hero.MainHero.IsPrisoner && this._raiderParties.Contains(Hero.MainHero.PartyBelongedToAsPrisoner.MobileParty))
					{
						EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
						if (Hero.MainHero.HitPoints < 50)
						{
							Hero.MainHero.Heal(50 - Hero.MainHero.HitPoints, false);
						}
						InformationManager.ShowInquiry(new InquiryData(new TextObject("{=FPhWhjq7}Defeated", null).ToString(), new TextObject("{=rbjjDYXj}You were defeated by the raiders in the hideout but you managed to escape. You need to wait to be able to attack again.", null).ToString(), true, false, new TextObject("{=yQtzabbe}Close", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
					}
					if (this._hideout.Parties.Count == 0)
					{
						this.InitializeHideout();
					}
					this._hideoutBattleEndState = IstianasBannerPieceQuest.HideoutBattleEndState.None;
					return;
				}
				this._hideoutBattleEndState = IstianasBannerPieceQuest.HideoutBattleEndState.Defeated;
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00010684 File Offset: 0x0000E884
		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement == this._hideout && !this._hideout.Hideout.IsInfested)
			{
				this.InitializeHideout();
			}
			if (this._hideoutBattleEndState == IstianasBannerPieceQuest.HideoutBattleEndState.Victory)
			{
				FirstPhase.Instance.CollectBannerPiece();
				base.CompleteQuestWithSuccess();
				this._hideoutBattleEndState = IstianasBannerPieceQuest.HideoutBattleEndState.None;
			}
			else if (this._hideoutBattleEndState == IstianasBannerPieceQuest.HideoutBattleEndState.Retreated || this._hideoutBattleEndState == IstianasBannerPieceQuest.HideoutBattleEndState.Defeated)
			{
				if (Hero.MainHero.IsPrisoner)
				{
					EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
				}
				if (Hero.MainHero.HitPoints < 50)
				{
					Hero.MainHero.Heal(50 - Hero.MainHero.HitPoints, false);
				}
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=FPhWhjq7}Defeated", null).ToString(), new TextObject("{=vxDVCK3n}You were defeated by the raiders in the hideout but you managed to escape. You need to wait to be able to attack again.", null).ToString(), true, false, new TextObject("{=yQtzabbe}Close", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
			}
			if (this._hideout.Parties.Count == 0)
			{
				this.InitializeHideout();
			}
			this._hideoutBattleEndState = IstianasBannerPieceQuest.HideoutBattleEndState.None;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0001079A File Offset: 0x0000E99A
		internal static void AutoGeneratedStaticCollectObjectsIstianasBannerPieceQuest(object o, List<object> collectedObjects)
		{
			((IstianasBannerPieceQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060002FB RID: 763 RVA: 0x000107A8 File Offset: 0x0000E9A8
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._hideout);
			collectedObjects.Add(this._raiderParties);
		}

		// Token: 0x060002FC RID: 764 RVA: 0x000107C9 File Offset: 0x0000E9C9
		internal static object AutoGeneratedGetMemberValue_hideout(object o)
		{
			return ((IstianasBannerPieceQuest)o)._hideout;
		}

		// Token: 0x060002FD RID: 765 RVA: 0x000107D6 File Offset: 0x0000E9D6
		internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
		{
			return ((IstianasBannerPieceQuest)o)._raiderParties;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x000107E3 File Offset: 0x0000E9E3
		internal static object AutoGeneratedGetMemberValue_hideoutBattleEndState(object o)
		{
			return ((IstianasBannerPieceQuest)o)._hideoutBattleEndState;
		}

		// Token: 0x040000F2 RID: 242
		private const int MainPartyHealHitPointLimit = 50;

		// Token: 0x040000F3 RID: 243
		private const int RaiderPartySize = 10;

		// Token: 0x040000F4 RID: 244
		private const int RaiderPartyCount = 2;

		// Token: 0x040000F5 RID: 245
		private const string IstianaRaiderPartyStringId = "istiana_banner_piece_quest_raider_party_";

		// Token: 0x040000F6 RID: 246
		[SaveableField(1)]
		private readonly Settlement _hideout;

		// Token: 0x040000F7 RID: 247
		[SaveableField(2)]
		private readonly List<MobileParty> _raiderParties;

		// Token: 0x040000F8 RID: 248
		[SaveableField(3)]
		private IstianasBannerPieceQuest.HideoutBattleEndState _hideoutBattleEndState;

		// Token: 0x02000078 RID: 120
		public enum HideoutBattleEndState
		{
			// Token: 0x04000248 RID: 584
			None,
			// Token: 0x04000249 RID: 585
			Retreated,
			// Token: 0x0400024A RID: 586
			Defeated,
			// Token: 0x0400024B RID: 587
			Victory
		}
	}
}
