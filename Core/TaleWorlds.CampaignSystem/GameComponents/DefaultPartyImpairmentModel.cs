using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000123 RID: 291
	public class DefaultPartyImpairmentModel : PartyImpairmentModel
	{
		// Token: 0x06001661 RID: 5729 RVA: 0x0006B898 File Offset: 0x00069A98
		public override float GetSiegeExpectedVulnerabilityTime()
		{
			float num = (2f + MBRandom.RandomFloatNormal + 24f - CampaignTime.Now.CurrentHourInDay) % 24f;
			float num2 = MathF.Pow(MBRandom.RandomFloat, 6f);
			return (((MBRandom.RandomFloatNormal > 0f) ? num2 : (1f - num2)) * 24f + num) % 24f;
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x0006B900 File Offset: 0x00069B00
		public override float GetDisorganizedStateDuration(MobileParty party)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(6f, false, null);
			if (party.MapEvent != null && (party.MapEvent.IsRaid || party.MapEvent.IsSiegeAssault) && party.HasPerk(DefaultPerks.Tactics.SwiftRegroup, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Tactics.SwiftRegroup.PrimaryBonus, DefaultPerks.Tactics.SwiftRegroup.Description);
			}
			if (party.HasPerk(DefaultPerks.Scouting.Foragers, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Scouting.Foragers.SecondaryBonus, DefaultPerks.Scouting.Foragers.Description);
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x0006B99C File Offset: 0x00069B9C
		public override bool CanGetDisorganized(PartyBase party)
		{
			return party.IsActive && party.IsMobile && party.MobileParty.MemberRoster.TotalManCount >= 10;
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x0006B9C7 File Offset: 0x00069BC7
		public override float GetVulnerabilityStateDuration(PartyBase party)
		{
			return MBRandom.RandomFloatNormal + 4f;
		}

		// Token: 0x040007D0 RID: 2000
		private const float BaseDisorganizedStateDuration = 6f;

		// Token: 0x040007D1 RID: 2001
		private static readonly TextObject _settlementInvolvedMapEvent = new TextObject("{=KVlPhPSD}Settlement involved map event", null);
	}
}
