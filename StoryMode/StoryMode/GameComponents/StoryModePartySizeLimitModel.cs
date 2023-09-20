using System;
using System.Linq;
using StoryMode.Quests.SecondPhase.ConspiracyQuests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace StoryMode.GameComponents
{
	// Token: 0x02000044 RID: 68
	public class StoryModePartySizeLimitModel : DefaultPartySizeLimitModel
	{
		// Token: 0x060003C1 RID: 961 RVA: 0x000174BC File Offset: 0x000156BC
		public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false)
		{
			if (party.IsMobile)
			{
				QuestBase questBase = Campaign.Current.QuestManager.Quests.FirstOrDefault((QuestBase q) => !q.IsFinalized && q.GetType() == typeof(DisruptSupplyLinesConspiracyQuest));
				if (questBase != null)
				{
					MobileParty conspiracyCaravan = ((DisruptSupplyLinesConspiracyQuest)questBase).ConspiracyCaravan;
					if (((conspiracyCaravan != null) ? conspiracyCaravan.Party : null) == party)
					{
						return new ExplainedNumber((float)((DisruptSupplyLinesConspiracyQuest)questBase).CaravanPartySize, false, null);
					}
				}
			}
			return base.GetPartyMemberSizeLimit(party, includeDescriptions);
		}
	}
}
