using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPartyImpairmentModel : PartyImpairmentModel
	{
		public override float GetSiegeExpectedVulnerabilityTime()
		{
			float num = (2f + MBRandom.RandomFloatNormal + 24f - CampaignTime.Now.CurrentHourInDay) % 24f;
			float num2 = MathF.Pow(MBRandom.RandomFloat, 6f);
			return (((MBRandom.RandomFloatNormal > 0f) ? num2 : (1f - num2)) * 24f + num) % 24f;
		}

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

		public override bool CanGetDisorganized(PartyBase party)
		{
			return party.IsActive && party.IsMobile && party.MobileParty.MemberRoster.TotalManCount >= 10;
		}

		public override float GetVulnerabilityStateDuration(PartyBase party)
		{
			return MBRandom.RandomFloatNormal + 4f;
		}

		private const float BaseDisorganizedStateDuration = 6f;

		private static readonly TextObject _settlementInvolvedMapEvent = new TextObject("{=KVlPhPSD}Settlement involved map event", null);
	}
}
