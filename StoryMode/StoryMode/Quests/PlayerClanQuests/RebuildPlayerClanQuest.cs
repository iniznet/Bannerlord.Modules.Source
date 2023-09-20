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
	// Token: 0x0200002D RID: 45
	public class RebuildPlayerClanQuest : StoryModeQuestBase
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000254 RID: 596 RVA: 0x0000CFAB File Offset: 0x0000B1AB
		private TextObject _startQuestLogText
		{
			get
			{
				return new TextObject("{=IITkXnnU}Calradia is a land full of peril - but also opportunities. To face the challenges that await, you will need to build up your clan.\nYour brother told you that there are many ways to go about this but that none forego coin. Trade would be one means to this end, fighting and selling off captured bandits in town another. Whatever path you choose to pursue, travelling alone would make you easy pickings for whomever came across your trail.\nYou know that you can recruit men to follow you from the notables of villages and towns, though they may ask you for a favor or two of their own before they allow you access to their more valued fighters.\nNaturally, you may also find more unique characters in the taverns of Calradia. However, these tend to favor more established clans.", null);
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000255 RID: 597 RVA: 0x0000CFB8 File Offset: 0x0000B1B8
		private TextObject _goldGoalLogText
		{
			get
			{
				return new TextObject("{=bXYFXLgg}Increase your denars by 1000", null);
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000CFC5 File Offset: 0x0000B1C5
		private TextObject _partySizeGoalLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=b6hQWKHe}Grow your party to {PARTY_SIZE} men", null);
				textObject.SetTextVariable("PARTY_SIZE", 20);
				return textObject;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000257 RID: 599 RVA: 0x0000CFE0 File Offset: 0x0000B1E0
		private TextObject _clanTierGoalLogText
		{
			get
			{
				return new TextObject("{=RbXiEdXk}Reach Clan Tier 1", null);
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000CFED File Offset: 0x0000B1ED
		private TextObject _hireCompanionGoalLogText
		{
			get
			{
				return new TextObject("{=e8Tjf8Ph}Hire 1 Companion", null);
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000259 RID: 601 RVA: 0x0000CFFA File Offset: 0x0000B1FA
		private TextObject _successLogText
		{
			get
			{
				return new TextObject("{=eJX7rhch}You have successfully rebuilt your clan.", null);
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0000D007 File Offset: 0x0000B207
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=bESRdcRo}Establish Your Clan", null);
			}
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000D014 File Offset: 0x0000B214
		public RebuildPlayerClanQuest()
			: base("rebuild_player_clan_storymode_quest", null, CampaignTime.Never)
		{
			this._finishQuest = false;
			this.SetDialogs();
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000D034 File Offset: 0x0000B234
		protected override void InitializeQuestOnGameLoad()
		{
			this.SetDialogs();
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000D03C File Offset: 0x0000B23C
		protected override void RegisterEvents()
		{
			CampaignEvents.HeroOrPartyTradedGold.AddNonSerializedListener(this, new Action<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ValueTuple<int, string>, bool>(this.HeroOrPartyTradedGold));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.OnTroopRecruitedEvent.AddNonSerializedListener(this, new Action<Hero, Settlement, Hero, CharacterObject, int>(this.OnTroopRecruited));
			CampaignEvents.RenownGained.AddNonSerializedListener(this, new Action<Hero, int, bool>(this.OnRenownGained));
			CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(this.OnNewCompanionAdded));
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000D104 File Offset: 0x0000B304
		protected override void OnStartQuest()
		{
			base.AddLog(this._startQuestLogText, true);
			this._goldGoalLog = base.AddDiscreteLog(this._goldGoalLogText, new TextObject("{=hYgmzZJX}Denars", null), Hero.MainHero.Gold, 2000, null, true);
			this._partySizeGoalLog = base.AddDiscreteLog(this._partySizeGoalLogText, new TextObject("{=DO4PE3Oo}Current Party Size", null), 1, 20, null, true);
			this._clanTierGoalLog = base.AddDiscreteLog(this._clanTierGoalLogText, new TextObject("{=aZxHIra4}Renown", null), (int)Clan.PlayerClan.Renown, 50, null, true);
			this._hireCompanionGoalLog = base.AddDiscreteLog(this._hireCompanionGoalLogText, new TextObject("{=VLD5416o}Companion Hired", null), 0, 1, null, true);
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000D1BC File Offset: 0x0000B3BC
		protected override void OnCompleteWithSuccess()
		{
			GainRenownAction.Apply(Hero.MainHero, 25f, false);
			base.AddLog(this._successLogText, false);
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000D1DC File Offset: 0x0000B3DC
		protected override void SetDialogs()
		{
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000D1DE File Offset: 0x0000B3DE
		private void HeroOrPartyTradedGold(ValueTuple<Hero, PartyBase> giver, ValueTuple<Hero, PartyBase> recipient, ValueTuple<int, string> goldAmount, bool showNotification)
		{
			this.UpdateProgresses();
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000D1E6 File Offset: 0x0000B3E6
		private void HourlyTick()
		{
			this.UpdateProgresses();
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000D1EE File Offset: 0x0000B3EE
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			this.UpdateProgresses();
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000D1F6 File Offset: 0x0000B3F6
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			this.UpdateProgresses();
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000D1FE File Offset: 0x0000B3FE
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			this.UpdateProgresses();
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000D206 File Offset: 0x0000B406
		private void OnTroopRecruited(Hero recruiterHero, Settlement recruitmentSettlement, Hero recruitmentSource, CharacterObject troop, int amount)
		{
			this.UpdateProgresses();
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000D20E File Offset: 0x0000B40E
		private void OnRenownGained(Hero hero, int gainedRenown, bool doNotNotify)
		{
			this.UpdateProgresses();
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000D216 File Offset: 0x0000B416
		private void OnNewCompanionAdded(Hero newCompanion)
		{
			this.UpdateProgresses();
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000D220 File Offset: 0x0000B420
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

		// Token: 0x0600026A RID: 618 RVA: 0x0000D338 File Offset: 0x0000B538
		internal static void AutoGeneratedStaticCollectObjectsRebuildPlayerClanQuest(object o, List<object> collectedObjects)
		{
			((RebuildPlayerClanQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000D346 File Offset: 0x0000B546
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._goldGoalLog);
			collectedObjects.Add(this._partySizeGoalLog);
			collectedObjects.Add(this._clanTierGoalLog);
			collectedObjects.Add(this._hireCompanionGoalLog);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000D37F File Offset: 0x0000B57F
		internal static object AutoGeneratedGetMemberValue_goldGoalLog(object o)
		{
			return ((RebuildPlayerClanQuest)o)._goldGoalLog;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000D38C File Offset: 0x0000B58C
		internal static object AutoGeneratedGetMemberValue_partySizeGoalLog(object o)
		{
			return ((RebuildPlayerClanQuest)o)._partySizeGoalLog;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000D399 File Offset: 0x0000B599
		internal static object AutoGeneratedGetMemberValue_clanTierGoalLog(object o)
		{
			return ((RebuildPlayerClanQuest)o)._clanTierGoalLog;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000D3A6 File Offset: 0x0000B5A6
		internal static object AutoGeneratedGetMemberValue_hireCompanionGoalLog(object o)
		{
			return ((RebuildPlayerClanQuest)o)._hireCompanionGoalLog;
		}

		// Token: 0x040000BB RID: 187
		private const int GoldGoal = 2000;

		// Token: 0x040000BC RID: 188
		private const int PartySizeGoal = 20;

		// Token: 0x040000BD RID: 189
		private const int ClanTierRenownGoal = 50;

		// Token: 0x040000BE RID: 190
		private const int RenownReward = 25;

		// Token: 0x040000BF RID: 191
		private const int HiredCompanionGoal = 1;

		// Token: 0x040000C0 RID: 192
		[SaveableField(1)]
		private JournalLog _goldGoalLog;

		// Token: 0x040000C1 RID: 193
		[SaveableField(2)]
		private JournalLog _partySizeGoalLog;

		// Token: 0x040000C2 RID: 194
		[SaveableField(3)]
		private JournalLog _clanTierGoalLog;

		// Token: 0x040000C3 RID: 195
		[SaveableField(4)]
		private JournalLog _hireCompanionGoalLog;

		// Token: 0x040000C4 RID: 196
		private bool _finishQuest;
	}
}
