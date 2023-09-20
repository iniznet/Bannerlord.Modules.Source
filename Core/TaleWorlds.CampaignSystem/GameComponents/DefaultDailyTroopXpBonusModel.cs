using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultDailyTroopXpBonusModel : DailyTroopXpBonusModel
	{
		public override int CalculateDailyTroopXpBonus(Town town)
		{
			return this.CalculateTroopXpBonusInternal(town);
		}

		private int CalculateTroopXpBonusInternal(Town town)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, false, null);
			foreach (Building building in town.Buildings)
			{
				float buildingEffectAmount = building.GetBuildingEffectAmount(BuildingEffectEnum.Experience);
				if (buildingEffectAmount > 0f)
				{
					explainedNumber.Add(buildingEffectAmount, building.Name, null);
				}
			}
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Leadership.RaiseTheMeek, town, ref explainedNumber);
			PerkHelper.AddPerkBonusForTown(DefaultPerks.TwoHanded.ProjectileDeflection, town, ref explainedNumber);
			return (int)explainedNumber.ResultNumber;
		}

		public override float CalculateGarrisonXpBonusMultiplier(Town town)
		{
			return 1f;
		}
	}
}
