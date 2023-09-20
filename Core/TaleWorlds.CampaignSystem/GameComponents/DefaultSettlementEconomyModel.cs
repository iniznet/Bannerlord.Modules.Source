using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000135 RID: 309
	public class DefaultSettlementEconomyModel : SettlementEconomyModel
	{
		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x06001701 RID: 5889 RVA: 0x000716C8 File Offset: 0x0006F8C8
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

		// Token: 0x06001702 RID: 5890 RVA: 0x000716E4 File Offset: 0x0006F8E4
		public override ValueTuple<float, float> GetSupplyDemandForCategory(Town town, ItemCategory category, float dailySupply, float dailyDemand, float oldSupply, float oldDemand)
		{
			float num = oldSupply * 0.85f + dailySupply * 0.15f;
			float num2 = oldDemand * 0.85f + dailyDemand * 0.15f;
			num = MathF.Max(0.1f, num);
			return new ValueTuple<float, float>(num, num2);
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x00071728 File Offset: 0x0006F928
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

		// Token: 0x06001704 RID: 5892 RVA: 0x0007178C File Offset: 0x0006F98C
		public override int GetTownGoldChange(Town town)
		{
			float num = 10000f + town.Prosperity * 12f - (float)town.Gold;
			return MathF.Round(0.25f * num);
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x000717C0 File Offset: 0x0006F9C0
		public override float GetDemandChangeFromValue(float purchaseValue)
		{
			return purchaseValue * 0.15f;
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x000717C9 File Offset: 0x0006F9C9
		public override float GetEstimatedDemandForCategory(Town town, ItemData itemData, ItemCategory category)
		{
			return this.GetDailyDemandForCategory(town, category, 1000);
		}

		// Token: 0x0400081B RID: 2075
		private DefaultSettlementEconomyModel.CategoryValues _categoryValues;

		// Token: 0x0400081C RID: 2076
		private const int ProsperityLuxuryTreshold = 3000;

		// Token: 0x0400081D RID: 2077
		private const float dailyChangeFactor = 0.15f;

		// Token: 0x0400081E RID: 2078
		private const float oneMinusDailyChangeFactor = 0.85f;

		// Token: 0x02000510 RID: 1296
		private class CategoryValues
		{
			// Token: 0x0600423D RID: 16957 RVA: 0x00134E24 File Offset: 0x00133024
			public CategoryValues()
			{
				this.PriceDict = new Dictionary<ItemCategory, int>();
				foreach (ItemObject itemObject in Items.All)
				{
					this.PriceDict[itemObject.GetItemCategory()] = itemObject.Value;
				}
			}

			// Token: 0x0600423E RID: 16958 RVA: 0x00134E98 File Offset: 0x00133098
			public int GetValueOfCategory(ItemCategory category)
			{
				int num = 1;
				this.PriceDict.TryGetValue(category, out num);
				return num;
			}

			// Token: 0x040015B8 RID: 5560
			public Dictionary<ItemCategory, int> PriceDict;
		}
	}
}
