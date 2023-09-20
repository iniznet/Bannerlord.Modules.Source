using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPartyDesertionModel : PartyDesertionModel
	{
		public override int GetMoraleThresholdForTroopDesertion(MobileParty party)
		{
			return 10;
		}

		public override int GetNumberOfDeserters(MobileParty mobileParty)
		{
			bool flag = mobileParty.IsWageLimitExceeded();
			bool flag2 = mobileParty.Party.NumberOfAllMembers > mobileParty.LimitedPartySize;
			int num = 0;
			if (flag)
			{
				int num2 = mobileParty.TotalWage - mobileParty.PaymentLimit;
				num = MathF.Min(20, MathF.Max(1, (int)((float)num2 / Campaign.Current.AverageWage * 0.25f)));
			}
			else if (flag2)
			{
				if (mobileParty.IsGarrison)
				{
					num = MathF.Ceiling((float)(mobileParty.Party.NumberOfAllMembers - mobileParty.LimitedPartySize) * 0.25f);
				}
				else
				{
					num = ((mobileParty.Party.NumberOfAllMembers > mobileParty.LimitedPartySize) ? MathF.Max(1, (int)((float)(mobileParty.Party.NumberOfAllMembers - mobileParty.LimitedPartySize) * 0.25f)) : 0);
				}
			}
			return num;
		}
	}
}
