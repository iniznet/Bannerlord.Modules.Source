using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000120 RID: 288
	public class DefaultPartyDesertionModel : PartyDesertionModel
	{
		// Token: 0x0600164F RID: 5711 RVA: 0x0006AD67 File Offset: 0x00068F67
		public override int GetMoraleThresholdForTroopDesertion(MobileParty party)
		{
			return 10;
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x0006AD6C File Offset: 0x00068F6C
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
