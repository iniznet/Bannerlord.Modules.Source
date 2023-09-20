using System;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors
{
	public class AiArmyMemberBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, new Action<MobileParty, PartyThinkParams>(this.AiHourlyTick));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		public void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
		{
			if (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty)
			{
				return;
			}
			AIBehaviorTuple aibehaviorTuple = new AIBehaviorTuple(mobileParty.Army.LeaderParty, AiBehavior.EscortParty, false);
			ValueTuple<AIBehaviorTuple, float> valueTuple = new ValueTuple<AIBehaviorTuple, float>(aibehaviorTuple, 0.25f);
			p.AddBehaviorScore(valueTuple);
		}

		private const float FollowingArmyLeaderDefaultScore = 0.25f;
	}
}
