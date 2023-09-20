using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate
{
	// Token: 0x02000016 RID: 22
	public class SettlementNameplateEventsVM : ViewModel
	{
		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0000AF29 File Offset: 0x00009129
		// (set) Token: 0x06000225 RID: 549 RVA: 0x0000AF31 File Offset: 0x00009131
		public bool IsEventsRegistered { get; private set; }

		// Token: 0x06000226 RID: 550 RVA: 0x0000AF3A File Offset: 0x0000913A
		public SettlementNameplateEventsVM(Settlement settlement)
		{
			this._settlement = settlement;
			this.EventsList = new MBBindingList<SettlementNameplateEventItemVM>();
			this.TrackQuests = new MBBindingList<QuestMarkerVM>();
			this._relatedQuests = new List<QuestBase>();
			if (settlement.IsVillage)
			{
				this.AddPrimaryProductionIcon();
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000AF78 File Offset: 0x00009178
		public void Tick()
		{
			if (this._areQuestsDirty)
			{
				this.RefreshQuestCounts();
				this._areQuestsDirty = false;
			}
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000AF8F File Offset: 0x0000918F
		private void PopulateEventList()
		{
			if (Campaign.Current.TournamentManager.GetTournamentGame(this._settlement.Town) != null)
			{
				this.EventsList.Add(new SettlementNameplateEventItemVM(SettlementNameplateEventItemVM.SettlementEventType.Tournament));
			}
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000AFC0 File Offset: 0x000091C0
		public void RegisterEvents()
		{
			if (!this.IsEventsRegistered)
			{
				this.PopulateEventList();
				CampaignEvents.TournamentStarted.AddNonSerializedListener(this, new Action<Town>(this.OnTournamentStarted));
				CampaignEvents.TournamentFinished.AddNonSerializedListener(this, new Action<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>(this.OnTournamentFinished));
				CampaignEvents.TournamentCancelled.AddNonSerializedListener(this, new Action<Town>(this.OnTournamentCancelled));
				CampaignEvents.OnNewIssueCreatedEvent.AddNonSerializedListener(this, new Action<IssueBase>(this.OnNewIssueCreated));
				CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, new Action<IssueBase, IssueBase.IssueUpdateDetails, Hero>(this.OnIssueUpdated));
				CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, new Action<QuestBase>(this.OnQuestStarted));
				CampaignEvents.QuestLogAddedEvent.AddNonSerializedListener(this, new Action<QuestBase, bool>(this.OnQuestLogAdded));
				CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroTakenPrisoner));
				this.IsEventsRegistered = true;
				this.RefreshQuestCounts();
			}
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000B0E8 File Offset: 0x000092E8
		public void UnloadEvents()
		{
			if (this.IsEventsRegistered)
			{
				CampaignEvents.TournamentStarted.ClearListeners(this);
				CampaignEvents.TournamentFinished.ClearListeners(this);
				CampaignEvents.TournamentCancelled.ClearListeners(this);
				CampaignEvents.OnNewIssueCreatedEvent.ClearListeners(this);
				CampaignEvents.OnIssueUpdatedEvent.ClearListeners(this);
				CampaignEvents.OnQuestStartedEvent.ClearListeners(this);
				CampaignEvents.QuestLogAddedEvent.ClearListeners(this);
				CampaignEvents.OnQuestCompletedEvent.ClearListeners(this);
				CampaignEvents.SettlementEntered.ClearListeners(this);
				CampaignEvents.OnSettlementLeftEvent.ClearListeners(this);
				CampaignEvents.HeroPrisonerTaken.ClearListeners(this);
				int num = this.EventsList.Count;
				for (int i = 0; i < num; i++)
				{
					if (this.EventsList[i].EventType != SettlementNameplateEventItemVM.SettlementEventType.Production)
					{
						this.EventsList.RemoveAt(i);
						num--;
						i--;
					}
				}
				this.IsEventsRegistered = false;
			}
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000B1C0 File Offset: 0x000093C0
		private void OnTournamentStarted(Town town)
		{
			if (this._settlement.Town != null && town == this._settlement.Town)
			{
				bool flag = false;
				for (int i = 0; i < this.EventsList.Count; i++)
				{
					if (this.EventsList[i].EventType == SettlementNameplateEventItemVM.SettlementEventType.Tournament)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.EventsList.Add(new SettlementNameplateEventItemVM(SettlementNameplateEventItemVM.SettlementEventType.Tournament));
				}
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000B22B File Offset: 0x0000942B
		private void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			this.RemoveTournament(town);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000B234 File Offset: 0x00009434
		private void OnTournamentCancelled(Town town)
		{
			this.RemoveTournament(town);
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000B240 File Offset: 0x00009440
		private void RemoveTournament(Town town)
		{
			if (this._settlement.Town != null && town == this._settlement.Town)
			{
				if (this.EventsList.Count((SettlementNameplateEventItemVM e) => e.EventType == SettlementNameplateEventItemVM.SettlementEventType.Tournament) > 0)
				{
					int num = -1;
					for (int i = 0; i < this.EventsList.Count; i++)
					{
						if (this.EventsList[i].EventType == SettlementNameplateEventItemVM.SettlementEventType.Tournament)
						{
							num = i;
							break;
						}
					}
					if (num != -1)
					{
						this.EventsList.RemoveAt(num);
						return;
					}
					Debug.FailedAssert("There should be a tournament item to remove", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Nameplate\\SettlementNameplateEventsVM.cs", "RemoveTournament", 162);
				}
			}
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000B2F4 File Offset: 0x000094F4
		private void RefreshQuestCounts()
		{
			this._relatedQuests.Clear();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = Campaign.Current.IssueManager.GetNumOfActiveIssuesInSettlement(this._settlement, false);
			int numOfAvailableIssuesInSettlement = Campaign.Current.IssueManager.GetNumOfAvailableIssuesInSettlement(this._settlement);
			this.TrackQuests.Clear();
			List<QuestBase> list;
			if (Campaign.Current.QuestManager.TrackedObjects.TryGetValue(this._settlement, ref list))
			{
				foreach (QuestBase questBase in list)
				{
					if (questBase.IsSpecialQuest)
					{
						if (!this.TrackQuests.Any((QuestMarkerVM x) => x.IssueQuestFlag == SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest))
						{
							this.TrackQuests.Add(new QuestMarkerVM(SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest));
							this._relatedQuests.Add(questBase);
							continue;
						}
					}
					if (!this.TrackQuests.Any((QuestMarkerVM x) => x.IssueQuestFlag == SandBoxUIHelper.IssueQuestFlags.TrackedIssue))
					{
						this.TrackQuests.Add(new QuestMarkerVM(SandBoxUIHelper.IssueQuestFlags.TrackedIssue));
						this._relatedQuests.Add(questBase);
					}
				}
			}
			List<ValueTuple<bool, QuestBase>> questsRelatedToSettlement = SandBoxUIHelper.GetQuestsRelatedToSettlement(this._settlement);
			for (int i = 0; i < questsRelatedToSettlement.Count; i++)
			{
				if (questsRelatedToSettlement[i].Item1)
				{
					if (questsRelatedToSettlement[i].Item2.IsSpecialQuest)
					{
						num++;
					}
					else
					{
						num4++;
					}
				}
				else if (questsRelatedToSettlement[i].Item2.IsSpecialQuest)
				{
					num3++;
				}
				else
				{
					num2++;
				}
				this._relatedQuests.Add(questsRelatedToSettlement[i].Item2);
			}
			this.HandleIssueCount(numOfAvailableIssuesInSettlement, SettlementNameplateEventItemVM.SettlementEventType.AvailableIssue);
			this.HandleIssueCount(num4, SettlementNameplateEventItemVM.SettlementEventType.ActiveQuest);
			this.HandleIssueCount(num, SettlementNameplateEventItemVM.SettlementEventType.ActiveStoryQuest);
			this.HandleIssueCount(num2, SettlementNameplateEventItemVM.SettlementEventType.TrackedIssue);
			this.HandleIssueCount(num3, SettlementNameplateEventItemVM.SettlementEventType.TrackedStoryQuest);
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000B504 File Offset: 0x00009704
		private void OnNewIssueCreated(IssueBase issue)
		{
			if (issue.IssueSettlement != this._settlement)
			{
				Hero issueOwner = issue.IssueOwner;
				if (((issueOwner != null) ? issueOwner.CurrentSettlement : null) != this._settlement)
				{
					return;
				}
			}
			this._areQuestsDirty = true;
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000B535 File Offset: 0x00009735
		private void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero hero)
		{
			if (issue.IssueSettlement == this._settlement && issue.IssueQuest == null)
			{
				this._areQuestsDirty = true;
			}
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000B554 File Offset: 0x00009754
		private void OnQuestStarted(QuestBase quest)
		{
			if (this.IsQuestRelated(quest))
			{
				this._areQuestsDirty = true;
			}
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000B566 File Offset: 0x00009766
		private void OnQuestLogAdded(QuestBase quest, bool hideInformation)
		{
			if (this.IsQuestRelated(quest))
			{
				this._areQuestsDirty = true;
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000B578 File Offset: 0x00009778
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails details)
		{
			if (this.IsQuestRelated(quest))
			{
				this._areQuestsDirty = true;
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000B58A File Offset: 0x0000978A
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (settlement == this._settlement)
			{
				this._areQuestsDirty = true;
			}
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000B59C File Offset: 0x0000979C
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (settlement == this._settlement)
			{
				this._areQuestsDirty = true;
			}
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000B5AE File Offset: 0x000097AE
		private void OnHeroTakenPrisoner(PartyBase capturer, Hero prisoner)
		{
			if (prisoner.CurrentSettlement == this._settlement)
			{
				this._areQuestsDirty = true;
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000B5C8 File Offset: 0x000097C8
		private void AddPrimaryProductionIcon()
		{
			string stringId = this._settlement.Village.VillageType.PrimaryProduction.StringId;
			string text = (stringId.Contains("camel") ? "camel" : ((stringId.Contains("horse") || stringId.Contains("mule")) ? "horse" : stringId));
			this.EventsList.Add(new SettlementNameplateEventItemVM(text));
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000B638 File Offset: 0x00009838
		private void HandleIssueCount(int count, SettlementNameplateEventItemVM.SettlementEventType eventType)
		{
			SettlementNameplateEventItemVM settlementNameplateEventItemVM = this.EventsList.FirstOrDefault((SettlementNameplateEventItemVM e) => e.EventType == eventType);
			if (count > 0 && settlementNameplateEventItemVM == null)
			{
				this.EventsList.Add(new SettlementNameplateEventItemVM(eventType));
				return;
			}
			if (count == 0 && settlementNameplateEventItemVM != null)
			{
				this.EventsList.Remove(settlementNameplateEventItemVM);
			}
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000B69C File Offset: 0x0000989C
		private bool IsQuestRelated(QuestBase quest)
		{
			IssueBase issueOfQuest = IssueManager.GetIssueOfQuest(quest);
			return (issueOfQuest != null && issueOfQuest.IssueSettlement == this._settlement) || this._relatedQuests.Contains(quest) || SandBoxUIHelper.IsQuestRelatedToSettlement(quest, this._settlement);
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600023B RID: 571 RVA: 0x0000B6DD File Offset: 0x000098DD
		// (set) Token: 0x0600023C RID: 572 RVA: 0x0000B6E5 File Offset: 0x000098E5
		[DataSourceProperty]
		public MBBindingList<QuestMarkerVM> TrackQuests
		{
			get
			{
				return this._trackQuests;
			}
			set
			{
				if (value != this._trackQuests)
				{
					this._trackQuests = value;
					base.OnPropertyChangedWithValue<MBBindingList<QuestMarkerVM>>(value, "TrackQuests");
				}
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600023D RID: 573 RVA: 0x0000B703 File Offset: 0x00009903
		// (set) Token: 0x0600023E RID: 574 RVA: 0x0000B70B File Offset: 0x0000990B
		public MBBindingList<SettlementNameplateEventItemVM> EventsList
		{
			get
			{
				return this._eventsList;
			}
			set
			{
				if (value != this._eventsList)
				{
					this._eventsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<SettlementNameplateEventItemVM>>(value, "EventsList");
				}
			}
		}

		// Token: 0x0400010D RID: 269
		private List<QuestBase> _relatedQuests;

		// Token: 0x0400010E RID: 270
		private Settlement _settlement;

		// Token: 0x0400010F RID: 271
		private bool _areQuestsDirty;

		// Token: 0x04000110 RID: 272
		private MBBindingList<QuestMarkerVM> _trackQuests;

		// Token: 0x04000111 RID: 273
		private MBBindingList<SettlementNameplateEventItemVM> _eventsList;
	}
}
