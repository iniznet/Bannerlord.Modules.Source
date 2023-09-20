using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000131 RID: 305
	public class DefaultRaidModel : RaidModel
	{
		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x060016E2 RID: 5858 RVA: 0x00070614 File Offset: 0x0006E814
		private MBReadOnlyList<ValueTuple<ItemObject, float>> CommonLootItemSpawnChances
		{
			get
			{
				if (this._commonLootItems == null)
				{
					List<ValueTuple<ItemObject, float>> list = new List<ValueTuple<ItemObject, float>>
					{
						new ValueTuple<ItemObject, float>(DefaultItems.Hides, 1f),
						new ValueTuple<ItemObject, float>(DefaultItems.HardWood, 1f),
						new ValueTuple<ItemObject, float>(DefaultItems.Tools, 1f),
						new ValueTuple<ItemObject, float>(DefaultItems.Grain, 1f),
						new ValueTuple<ItemObject, float>(Campaign.Current.ObjectManager.GetObject<ItemObject>("linen"), 1f),
						new ValueTuple<ItemObject, float>(Campaign.Current.ObjectManager.GetObject<ItemObject>("sheep"), 1f),
						new ValueTuple<ItemObject, float>(Campaign.Current.ObjectManager.GetObject<ItemObject>("mule"), 1f),
						new ValueTuple<ItemObject, float>(Campaign.Current.ObjectManager.GetObject<ItemObject>("pottery"), 1f)
					};
					for (int i = list.Count - 1; i >= 0; i--)
					{
						ItemObject item = list[i].Item1;
						float num = 100f / ((float)item.Value + 1f);
						list[i] = new ValueTuple<ItemObject, float>(item, num);
					}
					this._commonLootItems = new MBReadOnlyList<ValueTuple<ItemObject, float>>(list);
				}
				return this._commonLootItems;
			}
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x0007076C File Offset: 0x0006E96C
		public override float CalculateHitDamage(MapEventSide attackerSide, float settlementHitPoints)
		{
			float num = (MathF.Sqrt((float)attackerSide.TroopCount) + 5f) / 900f * (float)CampaignTime.DeltaTime.ToHours;
			float num2 = 0f;
			foreach (MapEventParty mapEventParty in attackerSide.Parties)
			{
				MobileParty mobileParty = mapEventParty.Party.MobileParty;
				if (((mobileParty != null) ? mobileParty.LeaderHero : null) != null && mapEventParty.Party.MobileParty.LeaderHero.GetPerkValue(DefaultPerks.Roguery.NoRestForTheWicked))
				{
					num2 += DefaultPerks.Roguery.NoRestForTheWicked.SecondaryBonus;
				}
			}
			return num + num * num2;
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x00070830 File Offset: 0x0006EA30
		public override MBReadOnlyList<ValueTuple<ItemObject, float>> GetCommonLootItemScores()
		{
			return this.CommonLootItemSpawnChances;
		}

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x060016E5 RID: 5861 RVA: 0x00070838 File Offset: 0x0006EA38
		public override int GoldRewardForEachLostHearth
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0400081A RID: 2074
		private MBReadOnlyList<ValueTuple<ItemObject, float>> _commonLootItems;
	}
}
