using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultInventoryCapacityModel : InventoryCapacityModel
	{
		public override int GetItemAverageWeight()
		{
			return 10;
		}

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

		private const int _itemAverageWeight = 10;

		private const float TroopsFactor = 2f;

		private const float SpareMountsFactor = 2f;

		private const float PackAnimalsFactor = 10f;

		private static readonly TextObject _textTroops = new TextObject("{=5k4dxUEJ}Troops", null);

		private static readonly TextObject _textHorses = new TextObject("{=1B8ZDOLs}Horses", null);

		private static readonly TextObject _textBase = new TextObject("{=basevalue}Base", null);

		private static readonly TextObject _textSpareMounts = new TextObject("{=rCiKbsyW}Spare Mounts", null);

		private static readonly TextObject _textPackAnimals = new TextObject("{=dI1AOyqh}Pack Animals", null);
	}
}
