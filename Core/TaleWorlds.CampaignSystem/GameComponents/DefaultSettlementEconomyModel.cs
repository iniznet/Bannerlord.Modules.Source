using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultSettlementEconomyModel : SettlementEconomyModel
	{
		private DefaultSettlementEconomyModel.CategoryValues CategoryValuesCache
		{
			get
			{
				if (this._categoryValues == null)
				{
					this._categoryValues = new DefaultSettlementEconomyModel.CategoryValues();
				}
				return this._categoryValues;
			}
		}

		public override ValueTuple<float, float> GetSupplyDemandForCategory(Town town, ItemCategory category, float dailySupply, float dailyDemand, float oldSupply, float oldDemand)
		{
			float num = oldSupply * 0.85f + dailySupply * 0.15f;
			float num2 = oldDemand * 0.85f + dailyDemand * 0.15f;
			num = MathF.Max(0.1f, num);
			return new ValueTuple<float, float>(num, num2);
		}

		public override float GetDailyDemandForCategory(Town town, ItemCategory category, int extraProsperity)
		{
			float num = MathF.Max(0f, town.Prosperity + (float)extraProsperity);
			float num2 = MathF.Max(0f, town.Prosperity - 3000f);
			float num3 = category.BaseDemand * num;
			float num4 = category.LuxuryDemand * num2;
			float num5 = num3 + num4;
			if (category.BaseDemand < 1E-08f)
			{
				num5 = num * 0.01f;
			}
			return num5;
		}

		public override int GetTownGoldChange(Town town)
		{
			float num = 10000f + town.Prosperity * 12f - (float)town.Gold;
			return MathF.Round(0.25f * num);
		}

		public override float GetDemandChangeFromValue(float purchaseValue)
		{
			return purchaseValue * 0.15f;
		}

		public override float GetEstimatedDemandForCategory(Town town, ItemData itemData, ItemCategory category)
		{
			return this.GetDailyDemandForCategory(town, category, 1000);
		}

		private DefaultSettlementEconomyModel.CategoryValues _categoryValues;

		private const int ProsperityLuxuryTreshold = 3000;

		private const float dailyChangeFactor = 0.15f;

		private const float oneMinusDailyChangeFactor = 0.85f;

		private class CategoryValues
		{
			public CategoryValues()
			{
				this.PriceDict = new Dictionary<ItemCategory, int>();
				foreach (ItemObject itemObject in Items.All)
				{
					this.PriceDict[itemObject.GetItemCategory()] = itemObject.Value;
				}
			}

			public int GetValueOfCategory(ItemCategory category)
			{
				int num = 1;
				this.PriceDict.TryGetValue(category, out num);
				return num;
			}

			public Dictionary<ItemCategory, int> PriceDict;
		}
	}
}
