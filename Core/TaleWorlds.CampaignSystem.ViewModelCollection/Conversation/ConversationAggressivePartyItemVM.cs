using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Quests;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Conversation
{
	public class ConversationAggressivePartyItemVM : ViewModel
	{
		public ConversationAggressivePartyItemVM(MobileParty party, CharacterObject leader = null)
		{
			this.Party = party;
			if (leader != null)
			{
				this.LeaderVisual = new ImageIdentifierVM(CampaignUIHelper.GetCharacterCode(leader, false));
			}
			else if (party != null)
			{
				CharacterObject visualPartyLeader = CampaignUIHelper.GetVisualPartyLeader(party.Party);
				if (visualPartyLeader != null)
				{
					this.LeaderVisual = new ImageIdentifierVM(CampaignUIHelper.GetCharacterCode(visualPartyLeader, false));
				}
			}
			this.HealthyAmount = ((party != null) ? party.Party.NumberOfHealthyMembers : 0);
			this.RefreshQuests();
		}

		private void RefreshQuests()
		{
			this.Quests = new MBBindingList<QuestMarkerVM>();
			if (this.Party != null)
			{
				List<QuestBase> questsRelatedToParty = CampaignUIHelper.GetQuestsRelatedToParty(this.Party);
				CampaignUIHelper.IssueQuestFlags issueQuestFlags = CampaignUIHelper.IssueQuestFlags.None;
				for (int i = 0; i < questsRelatedToParty.Count; i++)
				{
					issueQuestFlags |= CampaignUIHelper.GetQuestType(questsRelatedToParty[i], this.Party.LeaderHero);
				}
				Hero leaderHero = this.Party.LeaderHero;
				if (((leaderHero != null) ? leaderHero.Issue : null) != null)
				{
					issueQuestFlags |= CampaignUIHelper.GetIssueType(this.Party.LeaderHero.Issue);
				}
				if ((issueQuestFlags & CampaignUIHelper.IssueQuestFlags.TrackedIssue) != CampaignUIHelper.IssueQuestFlags.None)
				{
					this.Quests.Add(new QuestMarkerVM(CampaignUIHelper.IssueQuestFlags.TrackedIssue, null, null));
				}
				else if ((issueQuestFlags & CampaignUIHelper.IssueQuestFlags.ActiveIssue) != CampaignUIHelper.IssueQuestFlags.None)
				{
					this.Quests.Add(new QuestMarkerVM(CampaignUIHelper.IssueQuestFlags.ActiveIssue, null, null));
				}
				else if ((issueQuestFlags & CampaignUIHelper.IssueQuestFlags.AvailableIssue) != CampaignUIHelper.IssueQuestFlags.None)
				{
					this.Quests.Add(new QuestMarkerVM(CampaignUIHelper.IssueQuestFlags.AvailableIssue, null, null));
				}
				if ((issueQuestFlags & CampaignUIHelper.IssueQuestFlags.TrackedStoryQuest) != CampaignUIHelper.IssueQuestFlags.None)
				{
					this.Quests.Add(new QuestMarkerVM(CampaignUIHelper.IssueQuestFlags.TrackedStoryQuest, null, null));
					return;
				}
				if ((issueQuestFlags & CampaignUIHelper.IssueQuestFlags.ActiveStoryQuest) != CampaignUIHelper.IssueQuestFlags.None)
				{
					this.Quests.Add(new QuestMarkerVM(CampaignUIHelper.IssueQuestFlags.ActiveStoryQuest, null, null));
				}
			}
		}

		public void ExecuteShowPartyTooltip()
		{
			if (this.Party != null)
			{
				InformationManager.ShowTooltip(typeof(MobileParty), new object[] { this.Party, false, true });
			}
		}

		public void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

		[DataSourceProperty]
		public ImageIdentifierVM LeaderVisual
		{
			get
			{
				return this._leaderVisual;
			}
			set
			{
				if (value != this._leaderVisual)
				{
					this._leaderVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "LeaderVisual");
				}
			}
		}

		[DataSourceProperty]
		public int HealthyAmount
		{
			get
			{
				return this._healthyAmount;
			}
			set
			{
				if (value != this._healthyAmount)
				{
					this._healthyAmount = value;
					base.OnPropertyChangedWithValue(value, "HealthyAmount");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<QuestMarkerVM> Quests
		{
			get
			{
				return this._quests;
			}
			set
			{
				if (value != this._quests)
				{
					this._quests = value;
					base.OnPropertyChangedWithValue<MBBindingList<QuestMarkerVM>>(value, "Quests");
				}
			}
		}

		public readonly MobileParty Party;

		private MBBindingList<QuestMarkerVM> _quests;

		private ImageIdentifierVM _leaderVisual;

		private int _healthyAmount;
	}
}
