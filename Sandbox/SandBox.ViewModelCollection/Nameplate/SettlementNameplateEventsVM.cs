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
	public class SettlementNameplateEventsVM : ViewModel
	{
		public bool IsEventsRegistered { get; private set; }

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

		public void Tick()
		{
			if (this._areQuestsDirty)
			{
				this.RefreshQuestCounts();
				this._areQuestsDirty = false;
			}
		}

		private void PopulateEventList()
		{
			if (Campaign.Current.TournamentManager.GetTournamentGame(this._settlement.Town) != null)
			{
				this.EventsList.Add(new SettlementNameplateEventItemVM(SettlementNameplateEventItemVM.SettlementEventType.Tournament));
			}
		}

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

		private void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			this.RemoveTournament(town);
		}

		private void OnTournamentCancelled(Town town)
		{
			this.RemoveTournament(town);
		}

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

		private void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero hero)
		{
			if (issue.IssueSettlement == this._settlement && issue.IssueQuest == null)
			{
				this._areQuestsDirty = true;
			}
		}

		private void OnQuestStarted(QuestBase quest)
		{
			if (this.IsQuestRelated(quest))
			{
				this._areQuestsDirty = true;
			}
		}

		private void OnQuestLogAdded(QuestBase quest, bool hideInformation)
		{
			if (this.IsQuestRelated(quest))
			{
				this._areQuestsDirty = true;
			}
		}

		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails details)
		{
			if (this.IsQuestRelated(quest))
			{
				this._areQuestsDirty = true;
			}
		}

		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (settlement == this._settlement)
			{
				this._areQuestsDirty = true;
			}
		}

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (settlement == this._settlement)
			{
				this._areQuestsDirty = true;
			}
		}

		private void OnHeroTakenPrisoner(PartyBase capturer, Hero prisoner)
		{
			if (prisoner.CurrentSettlement == this._settlement)
			{
				this._areQuestsDirty = true;
			}
		}

		private void AddPrimaryProductionIcon()
		{
			string stringId = this._settlement.Village.VillageType.PrimaryProduction.StringId;
			string text = (stringId.Contains("camel") ? "camel" : ((stringId.Contains("horse") || stringId.Contains("mule")) ? "horse" : stringId));
			this.EventsList.Add(new SettlementNameplateEventItemVM(text));
		}

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

		private bool IsQuestRelated(QuestBase quest)
		{
			IssueBase issueOfQuest = IssueManager.GetIssueOfQuest(quest);
			return (issueOfQuest != null && issueOfQuest.IssueSettlement == this._settlement) || this._relatedQuests.Contains(quest) || SandBoxUIHelper.IsQuestRelatedToSettlement(quest, this._settlement);
		}

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

		private List<QuestBase> _relatedQuests;

		private Settlement _settlement;

		private bool _areQuestsDirty;

		private MBBindingList<QuestMarkerVM> _trackQuests;

		private MBBindingList<SettlementNameplateEventItemVM> _eventsList;
	}
}
