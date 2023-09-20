using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.FirstPhase
{
	// Token: 0x02000032 RID: 50
	public class CreateKingdomQuest : StoryModeQuestBase
	{
		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0000F9BC File Offset: 0x0000DBBC
		private TextObject _onQuestStartedImperialLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=N5Qg5ick}You told {MENTOR.LINK} that you will create your own imperial faction. You can do that by speaking to one of your governors once you fulfill the requirements. {?MENTOR.GENDER}She{?}He{\\?} expects to talk to you once you succeed.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002CB RID: 715 RVA: 0x0000F9F0 File Offset: 0x0000DBF0
		private TextObject _onQuestStartedAntiImperialLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=AxKDQJ4G}You told {MENTOR.LINK} that you will create your own kingdom to defeat the Empire. You can do that by speaking to one of your governors once you fulfill the requirements. {?MENTOR.GENDER}She{?}He{\\?} expects to talk to you once you succeed.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0000FA24 File Offset: 0x0000DC24
		private TextObject _imperialKingdomCreatedLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=UnjgFmnE}Heeding the advice of {MENTOR.LINK}, you have created an imperial faction. You can tell {?MENTOR.GENDER}her{?}him{\\?} that you will support your own kingdom.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060002CD RID: 717 RVA: 0x0000FA58 File Offset: 0x0000DC58
		private TextObject _antiImperialKingdomCreatedLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=BekWpXmR}Heeding the advice of {MENTOR.LINK}, you have created a kingdom to oppose the Empire. You can tell {?MENTOR.GENDER}her{?}him{\\?} that you will support your own kingdom.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0000FA89 File Offset: 0x0000DC89
		private TextObject _leftKingdomAfterCreatingLogText
		{
			get
			{
				return new TextObject("{=nNavD2NO}You left the kingdom you have created. You can only support kingdoms that you are a part of.", null);
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060002CF RID: 719 RVA: 0x0000FA96 File Offset: 0x0000DC96
		private TextObject _clanTierRequirementLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=QxeKZ3nE}Reach Clan Tier {CLAN_TIER}", null);
				textObject.SetTextVariable("CLAN_TIER", Campaign.Current.Models.KingdomCreationModel.MinimumClanTierToCreateKingdom);
				return textObject;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x0000FAC3 File Offset: 0x0000DCC3
		private TextObject _partySizeRequirementLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=NzQq2qp1}Gather {PARTY_SIZE} Troops", null);
				textObject.SetTextVariable("PARTY_SIZE", 100);
				return textObject;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x0000FADE File Offset: 0x0000DCDE
		private TextObject _settlementOwnershipRequirementLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=Bo66bfTh}Own {?IS_IMPERIAL}an Imperial Settlement{?}a Settlement{\\?} ", null);
				textObject.SetTextVariable("IS_IMPERIAL", this._isImperial ? 1 : 0);
				return textObject;
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x0000FB03 File Offset: 0x0000DD03
		private TextObject _clanIndependenceRequirementLogText
		{
			get
			{
				return new TextObject("{=a0ZKBj6P}Be an independent clan", null);
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x0000FB10 File Offset: 0x0000DD10
		private TextObject _questFailedLogText
		{
			get
			{
				return new TextObject("{=tVlZTOst}You have chosen a different path.", null);
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000FB1D File Offset: 0x0000DD1D
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=HhFHRs7N}Create {?IS_IMPERIAL}an imperial faction{?}a non-imperial kingdom{\\?}", null);
				textObject.SetTextVariable("IS_IMPERIAL", this._isImperial ? 1 : 0);
				return textObject;
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000FB48 File Offset: 0x0000DD48
		public CreateKingdomQuest(Hero questGiver)
			: base("main_storyline_create_kingdom_quest_" + ((StoryModeHeroes.ImperialMentor == questGiver) ? "1" : "0"), questGiver, StoryModeManager.Current.MainStoryLine.FirstPhase.FirstPhaseEndTime)
		{
			this._isImperial = StoryModeHeroes.ImperialMentor == questGiver;
			this.SetDialogs();
			if (this._isImperial)
			{
				base.AddLog(this._onQuestStartedImperialLogText, false);
			}
			else
			{
				base.AddLog(this._onQuestStartedAntiImperialLogText, false);
			}
			int minimumClanTierToCreateKingdom = Campaign.Current.Models.KingdomCreationModel.MinimumClanTierToCreateKingdom;
			this._clanTierRequirementLog = base.AddDiscreteLog(this._clanTierRequirementLogText, new TextObject("{=tTLvo8sM}Clan Tier", null), (int)MathF.Clamp((float)Clan.PlayerClan.Tier, 0f, (float)minimumClanTierToCreateKingdom), minimumClanTierToCreateKingdom, null, false);
			this._partySizeRequirementLog = base.AddDiscreteLog(this._partySizeRequirementLogText, new TextObject("{=aClquusd}Troop Count", null), (int)MathF.Clamp((float)(MobileParty.MainParty.MemberRoster.TotalManCount - MobileParty.MainParty.MemberRoster.TotalWounded), 0f, 100f), 100, null, false);
			this._clanIndependenceRequirementLog = base.AddDiscreteLog(this._clanIndependenceRequirementLogText, new TextObject("{=qa0o7xaj}Clan Independence", null), (Clan.PlayerClan.Kingdom == null) ? 1 : 0, 1, null, false);
			int num;
			if (!this._isImperial)
			{
				num = Clan.PlayerClan.Settlements.Count((Settlement t) => t.IsFortification);
			}
			else
			{
				num = Clan.PlayerClan.Settlements.Count((Settlement t) => t.IsFortification && t.Culture == StoryModeData.ImperialCulture);
			}
			int num2 = num;
			num2 = (int)MathF.Clamp((float)num2, 0f, 1f);
			this._settlementOwnershipRequirementLog = base.AddDiscreteLog(this._settlementOwnershipRequirementLogText, new TextObject("{=gL3WCqM5}Settlement Count", null), num2, 1, null, false);
			base.InitializeQuestOnCreation();
			this.CheckPlayerClanDiplomaticState(Clan.PlayerClan.Kingdom);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000FD47 File Offset: 0x0000DF47
		protected override void SetDialogs()
		{
			this.DiscussDialogFlow = this.GetMentorDialogueFlow();
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000FD55 File Offset: 0x0000DF55
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000FD5D File Offset: 0x0000DF5D
		private DialogFlow GetMentorDialogueFlow()
		{
			return DialogFlow.CreateDialogFlow("quest_discuss", 300).NpcLine("{=kbyqtszZ}I'm listening..", null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
				.PlayerLine("{=wErSpkjy}I'm still working on it.", null)
				.CloseDialog();
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000FD9C File Offset: 0x0000DF9C
		private void OnClanTierIncreased(Clan clan, bool showNotification)
		{
			if (!this._hasPlayerCreatedKingdom && clan == Clan.PlayerClan)
			{
				base.UpdateQuestTaskStage(this._clanTierRequirementLog, (int)MathF.Clamp((float)Clan.PlayerClan.Tier, 0f, (float)Campaign.Current.Models.KingdomCreationModel.MinimumClanTierToCreateKingdom));
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000FDF0 File Offset: 0x0000DFF0
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			if (clan == Clan.PlayerClan)
			{
				this.CheckPlayerClanDiplomaticState(newKingdom);
			}
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000FE04 File Offset: 0x0000E004
		private void CheckPlayerClanDiplomaticState(Kingdom newKingdom)
		{
			if (newKingdom == null)
			{
				if (this._hasPlayerCreatedKingdom)
				{
					this._leftKingdomLog = base.AddLog(this._leftKingdomAfterCreatingLogText, false);
					this._hasPlayerCreatedKingdom = false;
				}
				base.UpdateQuestTaskStage(this._clanIndependenceRequirementLog, 1);
				return;
			}
			if (newKingdom.RulingClan != Clan.PlayerClan)
			{
				if (this._playerCreatedKingdom == newKingdom && this._isImperial == StoryModeData.IsKingdomImperial(newKingdom))
				{
					base.RemoveLog(this._leftKingdomLog);
				}
				return;
			}
			this._playerCreatedKingdom = newKingdom;
			if (StoryModeData.IsKingdomImperial(newKingdom))
			{
				if (!this._isImperial)
				{
					base.UpdateQuestTaskStage(this._clanIndependenceRequirementLog, 0);
					return;
				}
				this._hasPlayerCreatedKingdom = true;
				if (this._leftKingdomLog != null)
				{
					base.RemoveLog(this._leftKingdomLog);
					return;
				}
				base.AddLog(this._imperialKingdomCreatedLogText, false);
				return;
			}
			else
			{
				if (this._isImperial)
				{
					base.UpdateQuestTaskStage(this._clanIndependenceRequirementLog, 0);
					return;
				}
				this._hasPlayerCreatedKingdom = true;
				if (this._leftKingdomLog != null)
				{
					base.RemoveLog(this._leftKingdomLog);
					return;
				}
				base.AddLog(this._antiImperialKingdomCreatedLogText, false);
				return;
			}
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000FF08 File Offset: 0x0000E108
		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (!this._hasPlayerCreatedKingdom && (newOwner == Hero.MainHero || oldOwner == Hero.MainHero))
			{
				int num = -1;
				if (this._isImperial && settlement.Culture == StoryModeData.ImperialCulture)
				{
					num = Clan.PlayerClan.Settlements.Count((Settlement t) => t.IsFortification && t.Culture == StoryModeData.ImperialCulture);
				}
				else if (!this._isImperial)
				{
					num = Clan.PlayerClan.Settlements.Count((Settlement t) => t.IsFortification);
				}
				if (num != -1)
				{
					base.UpdateQuestTaskStage(this._settlementOwnershipRequirementLog, (int)MathF.Clamp((float)num, 0f, 1f));
				}
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0000FFD8 File Offset: 0x0000E1D8
		private void OnPartySizeChanged(PartyBase party)
		{
			if (!this._hasPlayerCreatedKingdom && party == PartyBase.MainParty)
			{
				int num = (int)MathF.Clamp((float)(MobileParty.MainParty.MemberRoster.TotalManCount - MobileParty.MainParty.MemberRoster.TotalWounded), 0f, 100f);
				base.UpdateQuestTaskStage(this._partySizeRequirementLog, num);
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00010033 File Offset: 0x0000E233
		private void MainStoryLineChosen(MainStoryLineSide chosenSide)
		{
			if (this._hasPlayerCreatedKingdom && ((chosenSide == MainStoryLineSide.CreateImperialKingdom && this._isImperial) || (chosenSide == MainStoryLineSide.CreateAntiImperialKingdom && !this._isImperial)))
			{
				base.CompleteQuestWithSuccess();
				return;
			}
			base.AddLog(this._questFailedLogText, false);
			base.CompleteQuestWithFail(null);
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00010074 File Offset: 0x0000E274
		protected override void RegisterEvents()
		{
			CampaignEvents.ClanTierIncrease.AddNonSerializedListener(this, new Action<Clan, bool>(this.OnClanTierIncreased));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.OnPartySizeChangedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.OnPartySizeChanged));
			StoryModeEvents.OnMainStoryLineSideChosenEvent.AddNonSerializedListener(this, new Action<MainStoryLineSide>(this.MainStoryLineChosen));
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x000100F4 File Offset: 0x0000E2F4
		internal static void AutoGeneratedStaticCollectObjectsCreateKingdomQuest(object o, List<object> collectedObjects)
		{
			((CreateKingdomQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00010104 File Offset: 0x0000E304
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._leftKingdomLog);
			collectedObjects.Add(this._playerCreatedKingdom);
			collectedObjects.Add(this._clanTierRequirementLog);
			collectedObjects.Add(this._partySizeRequirementLog);
			collectedObjects.Add(this._settlementOwnershipRequirementLog);
			collectedObjects.Add(this._clanIndependenceRequirementLog);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00010160 File Offset: 0x0000E360
		internal static object AutoGeneratedGetMemberValue_isImperial(object o)
		{
			return ((CreateKingdomQuest)o)._isImperial;
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00010172 File Offset: 0x0000E372
		internal static object AutoGeneratedGetMemberValue_hasPlayerCreatedKingdom(object o)
		{
			return ((CreateKingdomQuest)o)._hasPlayerCreatedKingdom;
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00010184 File Offset: 0x0000E384
		internal static object AutoGeneratedGetMemberValue_leftKingdomLog(object o)
		{
			return ((CreateKingdomQuest)o)._leftKingdomLog;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00010191 File Offset: 0x0000E391
		internal static object AutoGeneratedGetMemberValue_playerCreatedKingdom(object o)
		{
			return ((CreateKingdomQuest)o)._playerCreatedKingdom;
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0001019E File Offset: 0x0000E39E
		internal static object AutoGeneratedGetMemberValue_clanTierRequirementLog(object o)
		{
			return ((CreateKingdomQuest)o)._clanTierRequirementLog;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x000101AB File Offset: 0x0000E3AB
		internal static object AutoGeneratedGetMemberValue_partySizeRequirementLog(object o)
		{
			return ((CreateKingdomQuest)o)._partySizeRequirementLog;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x000101B8 File Offset: 0x0000E3B8
		internal static object AutoGeneratedGetMemberValue_settlementOwnershipRequirementLog(object o)
		{
			return ((CreateKingdomQuest)o)._settlementOwnershipRequirementLog;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x000101C5 File Offset: 0x0000E3C5
		internal static object AutoGeneratedGetMemberValue_clanIndependenceRequirementLog(object o)
		{
			return ((CreateKingdomQuest)o)._clanIndependenceRequirementLog;
		}

		// Token: 0x040000E8 RID: 232
		[SaveableField(1)]
		private readonly bool _isImperial;

		// Token: 0x040000E9 RID: 233
		private const int PartySizeRequirement = 100;

		// Token: 0x040000EA RID: 234
		private const int SettlementCountRequirement = 1;

		// Token: 0x040000EB RID: 235
		[SaveableField(2)]
		private bool _hasPlayerCreatedKingdom;

		// Token: 0x040000EC RID: 236
		[SaveableField(9)]
		private JournalLog _leftKingdomLog;

		// Token: 0x040000ED RID: 237
		[SaveableField(10)]
		private Kingdom _playerCreatedKingdom;

		// Token: 0x040000EE RID: 238
		[SaveableField(4)]
		private readonly JournalLog _clanTierRequirementLog;

		// Token: 0x040000EF RID: 239
		[SaveableField(5)]
		private readonly JournalLog _partySizeRequirementLog;

		// Token: 0x040000F0 RID: 240
		[SaveableField(6)]
		private readonly JournalLog _settlementOwnershipRequirementLog;

		// Token: 0x040000F1 RID: 241
		[SaveableField(7)]
		private readonly JournalLog _clanIndependenceRequirementLog;
	}
}
