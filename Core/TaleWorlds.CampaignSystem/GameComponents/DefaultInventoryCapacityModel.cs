using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000111 RID: 273
	public class DefaultInventoryCapacityModel : InventoryCapacityModel
	{
		// Token: 0x060015BD RID: 5565 RVA: 0x00066E05 File Offset: 0x00065005
		public override int GetItemAverageWeight()
		{
			return 10;
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x00066E0C File Offset: 0x0006500C
		public override ExplainedNumber CalculateInventoryCapacity(MobileParty mobileParty, bool includeDescriptions = false, int additionalTroops = 0, int additionalSpareMounts = 0, int additionalPackAnimals = 0, bool includeFollowers = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			PartyBase party = mobileParty.Party;
			int num = party.NumberOfMounts;
			int num2 = party.NumberOfHealthyMembers;
			int num3 = party.NumberOfPackAnimals;
			if (includeFollowers)
			{
				foreach (MobileParty mobileParty2 in mobileParty.AttachedParties)
				{
					num += party.NumberOfMounts;
					num2 += party.NumberOfHealthyMembers;
					num3 += party.NumberOfPackAnimals;
				}
			}
			if (mobileParty.HasPerk(DefaultPerks.Steward.ArenicosHorses, false))
			{
				num2 += MathF.Round((float)num2 * DefaultPerks.Steward.ArenicosHorses.PrimaryBonus);
			}
			if (mobileParty.HasPerk(DefaultPerks.Steward.ForcedLabor, false))
			{
				num2 += party.PrisonRoster.TotalHealthyCount;
			}
			explainedNumber.Add(10f, DefaultInventoryCapacityModel._textBase, null);
			explainedNumber.Add((float)num2 * 2f * 10f, DefaultInventoryCapacityModel._textTroops, null);
			explainedNumber.Add((float)num * 2f * 10f, DefaultInventoryCapacityModel._textSpareMounts, null);
			float num4 = (float)num3 * 10f * 10f;
			float num5 = 0f;
			if (mobileParty.HasPerk(DefaultPerks.Scouting.BeastWhisperer, true))
			{
				num5 += DefaultPerks.Scouting.BeastWhisperer.SecondaryBonus;
			}
			if (mobileParty.HasPerk(DefaultPerks.Riding.DeeperSacks, false))
			{
				num5 += DefaultPerks.Riding.DeeperSacks.PrimaryBonus;
			}
			if (mobileParty.HasPerk(DefaultPerks.Steward.ArenicosMules, false))
			{
				num5 += DefaultPerks.Steward.ArenicosMules.PrimaryBonus;
			}
			num4 *= num5 + 1f;
			explainedNumber.Add(num4, DefaultInventoryCapacityModel._textPackAnimals, null);
			if (mobileParty.HasPerk(DefaultPerks.Trade.CaravanMaster, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Trade.CaravanMaster.PrimaryBonus, DefaultPerks.Trade.CaravanMaster.Name);
			}
			explainedNumber.LimitMin(10f);
			return explainedNumber;
		}

		// Token: 0x0400079B RID: 1947
		private const int _itemAverageWeight = 10;

		// Token: 0x0400079C RID: 1948
		private const float TroopsFactor = 2f;

		// Token: 0x0400079D RID: 1949
		private const float SpareMountsFactor = 2f;

		// Token: 0x0400079E RID: 1950
		private const float PackAnimalsFactor = 10f;

		// Token: 0x0400079F RID: 1951
		private static readonly TextObject _textTroops = new TextObject("{=5k4dxUEJ}Troops", null);

		// Token: 0x040007A0 RID: 1952
		private static readonly TextObject _textHorses = new TextObject("{=1B8ZDOLs}Horses", null);

		// Token: 0x040007A1 RID: 1953
		private static readonly TextObject _textBase = new TextObject("{=basevalue}Base", null);

		// Token: 0x040007A2 RID: 1954
		private static readonly TextObject _textSpareMounts = new TextObject("{=rCiKbsyW}Spare Mounts", null);

		// Token: 0x040007A3 RID: 1955
		private static readonly TextObject _textPackAnimals = new TextObject("{=dI1AOyqh}Pack Animals", null);
	}
}
