using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.PlayerClanQuests
{
	public class RebuildPlayerClanQuest : StoryModeQuestBase
	{
		private TextObject _startQuestLogText
		{
			get
			{
				return new TextObject("{=IITkXnnU}Calradia is a land full of peril - but also opportunities. To face the challenges that await, you will need to build up your clan.\nYour brother told you that there are many ways to go about this but that none forego coin. Trade would be one means to this end, fighting and selling off captured bandits in town another. Whatever path you choose to pursue, travelling alone would make you easy pickings for whomever came across your trail.\nYou know that you can recruit men to follow you from the notables of villages and towns, though they may ask you for a favor or two of their own before they allow you access to their more valued fighters.\nNaturally, you may also find more unique characters in the taverns of Calradia. However, these tend to favor more established clans.", null);
			}
		}

		private TextObject _goldGoalLogText
		{
			get
			{
				return new TextObject("{=bXYFXLgg}Increase your denars by 1000", null);
			}
		}

		private TextObject _partySizeGoalLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=b6hQWKHe}Grow your party to {PARTY_SIZE} men", null);
				textObject.SetTextVariable("PARTY_SIZE", 20);
				return textObject;
			}
		}

		private TextObject _clanTierGoalLogText
		{
			get
			{
				return new TextObject("{=RbXiEdXk}Reach Clan Tier 1", null);
			}
		}

		private TextObject _hireCompanionGoalLogText
		{
			get
			{
				return new TextObject("{=e8Tjf8Ph}Hire 1 Companion", null);
			}
		}

		private TextObject _successLogText
		{
			get
			{
				return new TextObject("{=eJX7rhch}You have successfully rebuilt your clan.", null);
			}
		}

		public override TextObject Title
		{
			get
			{
				return new TextObject("{=bESRdcRo}Establish Your Clan", null);
			}
		}

		public RebuildPlayerClanQuest()
			: base("rebuild_player_clan_storymode_quest", null, CampaignTime.Never)
		{
			this._finishQuest = false;
			this.SetDialogs();
		}

		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.HeroOrPartyTradedGold.AddNonSerializedListener(this, new Action<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ValueTuple<int, string>, bool>(this.HeroOrPartyTradedGold));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.OnTroopRecruitedEvent.AddNonSerializedListener(this, new Action<Hero, Settlement, Hero, CharacterObject, int>(this.OnTroopRecruited));
			CampaignEvents.RenownGained.AddNonSerializedListener(this, new Action<Hero, int, bool>(this.OnRenownGained));
			CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(this.OnNewCompanionAdded));
		}

		protected override void OnStartQuest()
		{
			base.AddLog(this._startQuestLogText, true);
			this._goldGoalLog = base.AddDiscreteLog(this._goldGoalLogText, new TextObject("{=hYgmzZJX}Denars", null), Hero.MainHero.Gold, 2000, null, true);
			this._partySizeGoalLog = base.AddDiscreteLog(this._partySizeGoalLogText, new TextObject("{=DO4PE3Oo}Current Party Size", null), 1, 20, null, true);
			this._clanTierGoalLog = base.AddDiscreteLog(this._clanTierGoalLogText, new TextObject("{=aZxHIra4}Renown", null), (int)Clan.PlayerClan.Renown, 50, null, true);
			this._hireCompanionGoalLog = base.AddDiscreteLog(this._hireCompanionGoalLogText, new TextObject("{=VLD5416o}Companion Hired", null), 0, 1, null, true);
		}

		protected override void OnCompleteWithSuccess()
		{
			GainRenownAction.Apply(Hero.MainHero, 25f, false);
			base.AddLog(this._successLogText, false);
		}

		protected override void SetDialogs()
		{
		}

		private void HeroOrPartyTradedGold(ValueTuple<Hero, PartyBase> giver, ValueTuple<Hero, PartyBase> recipient, ValueTuple<int, string> goldAmount, bool showNotification)
		{
			this.UpdateProgresses();
		}

		protected override void HourlyTick()
		{
			this.UpdateProgresses();
		}

		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			this.UpdateProgresses();
		}

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			this.UpdateProgresses();
		}

		private void OnMapEventEnded(MapEvent mapEvent)
		{
			this.UpdateProgresses();
		}

		private void OnTroopRecruited(Hero recruiterHero, Settlement recruitmentSettlement, Hero recruitmentSource, CharacterObject troop, int amount)
		{
			this.UpdateProgresses();
		}

		private void OnRenownGained(Hero hero, int gainedRenown, bool doNotNotify)
		{
			this.UpdateProgresses();
		}

		private void OnNewCompanionAdded(Hero newCompanion)
		{
			this.UpdateProgresses();
		}

		private void UpdateProgresses()
		{
			this._goldGoalLog.UpdateCurrentProgress((Hero.MainHero.Gold > 2000) ? 2000 : Hero.MainHero.Gold);
			this._partySizeGoalLog.UpdateCurrentProgress((PartyBase.MainParty.MemberRoster.TotalManCount > 20) ? 20 : PartyBase.MainParty.MemberRoster.TotalManCount);
			this._clanTierGoalLog.UpdateCurrentProgress((Clan.PlayerClan.Renown > 50f) ? 50 : ((int)Clan.PlayerClan.Renown));
			this._hireCompanionGoalLog.UpdateCurrentProgress((Clan.PlayerClan.Companions.Count > 1) ? 1 : Clan.PlayerClan.Companions.Count);
			if (this._goldGoalLog.CurrentProgress >= 2000 && this._partySizeGoalLog.CurrentProgress >= 20 && this._clanTierGoalLog.CurrentProgress >= 50 && this._hireCompanionGoalLog.CurrentProgress >= 1 && !this._finishQuest)
			{
				this._finishQuest = true;
				base.CompleteQuestWithSuccess();
			}
		}

		internal static void AutoGeneratedStaticCollectObjectsRebuildPlayerClanQuest(object o, List<object> collectedObjects)
		{
			((RebuildPlayerClanQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._goldGoalLog);
			collectedObjects.Add(this._partySizeGoalLog);
			collectedObjects.Add(this._clanTierGoalLog);
			collectedObjects.Add(this._hireCompanionGoalLog);
		}

		internal static object AutoGeneratedGetMemberValue_goldGoalLog(object o)
		{
			return ((RebuildPlayerClanQuest)o)._goldGoalLog;
		}

		internal static object AutoGeneratedGetMemberValue_partySizeGoalLog(object o)
		{
			return ((RebuildPlayerClanQuest)o)._partySizeGoalLog;
		}

		internal static object AutoGeneratedGetMemberValue_clanTierGoalLog(object o)
		{
			return ((RebuildPlayerClanQuest)o)._clanTierGoalLog;
		}

		internal static object AutoGeneratedGetMemberValue_hireCompanionGoalLog(object o)
		{
			return ((RebuildPlayerClanQuest)o)._hireCompanionGoalLog;
		}

		private const int GoldGoal = 2000;

		private const int PartySizeGoal = 20;

		private const int ClanTierRenownGoal = 50;

		private const int RenownReward = 25;

		private const int HiredCompanionGoal = 1;

		[SaveableField(1)]
		private JournalLog _goldGoalLog;

		[SaveableField(2)]
		private JournalLog _partySizeGoalLog;

		[SaveableField(3)]
		private JournalLog _clanTierGoalLog;

		[SaveableField(4)]
		private JournalLog _hireCompanionGoalLog;

		private bool _finishQuest;
	}
}
