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
	public class CreateKingdomQuest : StoryModeQuestBase
	{
		private TextObject _onQuestStartedImperialLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=N5Qg5ick}You told {MENTOR.LINK} that you will create your own imperial faction. You can do that by speaking to one of your governors once you fulfill the requirements. {?MENTOR.GENDER}She{?}He{\\?} expects to talk to you once you succeed.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		private TextObject _onQuestStartedAntiImperialLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=AxKDQJ4G}You told {MENTOR.LINK} that you will create your own kingdom to defeat the Empire. You can do that by speaking to one of your governors once you fulfill the requirements. {?MENTOR.GENDER}She{?}He{\\?} expects to talk to you once you succeed.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		private TextObject _imperialKingdomCreatedLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=UnjgFmnE}Heeding the advice of {MENTOR.LINK}, you have created an imperial faction. You can tell {?MENTOR.GENDER}her{?}him{\\?} that you will support your own kingdom.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.ImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		private TextObject _antiImperialKingdomCreatedLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=BekWpXmR}Heeding the advice of {MENTOR.LINK}, you have created a kingdom to oppose the Empire. You can tell {?MENTOR.GENDER}her{?}him{\\?} that you will support your own kingdom.", null);
				StringHelpers.SetCharacterProperties("MENTOR", StoryModeHeroes.AntiImperialMentor.CharacterObject, textObject, false);
				return textObject;
			}
		}

		private TextObject _leftKingdomAfterCreatingLogText
		{
			get
			{
				return new TextObject("{=nNavD2NO}You left the kingdom you have created. You can only support kingdoms that you are a part of.", null);
			}
		}

		private TextObject _clanTierRequirementLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=QxeKZ3nE}Reach Clan Tier {CLAN_TIER}", null);
				textObject.SetTextVariable("CLAN_TIER", Campaign.Current.Models.KingdomCreationModel.MinimumClanTierToCreateKingdom);
				return textObject;
			}
		}

		private TextObject _partySizeRequirementLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=NzQq2qp1}Gather {PARTY_SIZE} Troops", null);
				textObject.SetTextVariable("PARTY_SIZE", 100);
				return textObject;
			}
		}

		private TextObject _settlementOwnershipRequirementLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=Bo66bfTh}Own {?IS_IMPERIAL}an Imperial Settlement{?}a Settlement{\\?} ", null);
				textObject.SetTextVariable("IS_IMPERIAL", this._isImperial ? 1 : 0);
				return textObject;
			}
		}

		private TextObject _clanIndependenceRequirementLogText
		{
			get
			{
				return new TextObject("{=a0ZKBj6P}Be an independent clan", null);
			}
		}

		private TextObject _questFailedLogText
		{
			get
			{
				return new TextObject("{=tVlZTOst}You have chosen a different path.", null);
			}
		}

		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=HhFHRs7N}Create {?IS_IMPERIAL}an imperial faction{?}a non-imperial kingdom{\\?}", null);
				textObject.SetTextVariable("IS_IMPERIAL", this._isImperial ? 1 : 0);
				return textObject;
			}
		}

		public override bool IsRemainingTimeHidden
		{
			get
			{
				return false;
			}
		}

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

		protected override void SetDialogs()
		{
			this.DiscussDialogFlow = this.GetMentorDialogueFlow();
		}

		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		protected override void HourlyTick()
		{
		}

		private DialogFlow GetMentorDialogueFlow()
		{
			return DialogFlow.CreateDialogFlow("quest_discuss", 300).NpcLine("{=kbyqtszZ}I'm listening..", null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
				.PlayerLine("{=wErSpkjy}I'm still working on it.", null)
				.CloseDialog();
		}

		private void OnClanTierIncreased(Clan clan, bool showNotification)
		{
			if (!this._hasPlayerCreatedKingdom && clan == Clan.PlayerClan)
			{
				base.UpdateQuestTaskStage(this._clanTierRequirementLog, (int)MathF.Clamp((float)Clan.PlayerClan.Tier, 0f, (float)Campaign.Current.Models.KingdomCreationModel.MinimumClanTierToCreateKingdom));
			}
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			if (clan == Clan.PlayerClan)
			{
				this.CheckPlayerClanDiplomaticState(newKingdom);
			}
		}

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

		private void OnPartySizeChanged(PartyBase party)
		{
			if (!this._hasPlayerCreatedKingdom && party == PartyBase.MainParty)
			{
				int num = (int)MathF.Clamp((float)(MobileParty.MainParty.MemberRoster.TotalManCount - MobileParty.MainParty.MemberRoster.TotalWounded), 0f, 100f);
				base.UpdateQuestTaskStage(this._partySizeRequirementLog, num);
			}
		}

		private void MainStoryLineChosen(MainStoryLineSide chosenSide)
		{
			if (this._hasPlayerCreatedKingdom && ((chosenSide == MainStoryLineSide.CreateImperialKingdom && this._isImperial) || (chosenSide == MainStoryLineSide.CreateAntiImperialKingdom && !this._isImperial)))
			{
				base.CompleteQuestWithSuccess();
				return;
			}
			base.CompleteQuestWithCancel(this._questFailedLogText);
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.ClanTierIncrease.AddNonSerializedListener(this, new Action<Clan, bool>(this.OnClanTierIncreased));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.OnPartySizeChangedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.OnPartySizeChanged));
			StoryModeEvents.OnMainStoryLineSideChosenEvent.AddNonSerializedListener(this, new Action<MainStoryLineSide>(this.MainStoryLineChosen));
		}

		internal static void AutoGeneratedStaticCollectObjectsCreateKingdomQuest(object o, List<object> collectedObjects)
		{
			((CreateKingdomQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

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

		internal static object AutoGeneratedGetMemberValue_isImperial(object o)
		{
			return ((CreateKingdomQuest)o)._isImperial;
		}

		internal static object AutoGeneratedGetMemberValue_hasPlayerCreatedKingdom(object o)
		{
			return ((CreateKingdomQuest)o)._hasPlayerCreatedKingdom;
		}

		internal static object AutoGeneratedGetMemberValue_leftKingdomLog(object o)
		{
			return ((CreateKingdomQuest)o)._leftKingdomLog;
		}

		internal static object AutoGeneratedGetMemberValue_playerCreatedKingdom(object o)
		{
			return ((CreateKingdomQuest)o)._playerCreatedKingdom;
		}

		internal static object AutoGeneratedGetMemberValue_clanTierRequirementLog(object o)
		{
			return ((CreateKingdomQuest)o)._clanTierRequirementLog;
		}

		internal static object AutoGeneratedGetMemberValue_partySizeRequirementLog(object o)
		{
			return ((CreateKingdomQuest)o)._partySizeRequirementLog;
		}

		internal static object AutoGeneratedGetMemberValue_settlementOwnershipRequirementLog(object o)
		{
			return ((CreateKingdomQuest)o)._settlementOwnershipRequirementLog;
		}

		internal static object AutoGeneratedGetMemberValue_clanIndependenceRequirementLog(object o)
		{
			return ((CreateKingdomQuest)o)._clanIndependenceRequirementLog;
		}

		[SaveableField(1)]
		private readonly bool _isImperial;

		private const int PartySizeRequirement = 100;

		private const int SettlementCountRequirement = 1;

		[SaveableField(2)]
		private bool _hasPlayerCreatedKingdom;

		[SaveableField(9)]
		private JournalLog _leftKingdomLog;

		[SaveableField(10)]
		private Kingdom _playerCreatedKingdom;

		[SaveableField(4)]
		private readonly JournalLog _clanTierRequirementLog;

		[SaveableField(5)]
		private readonly JournalLog _partySizeRequirementLog;

		[SaveableField(6)]
		private readonly JournalLog _settlementOwnershipRequirementLog;

		[SaveableField(7)]
		private readonly JournalLog _clanIndependenceRequirementLog;
	}
}
