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
	public class DefaultRaidModel : RaidModel
	{
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

		public override MBReadOnlyList<ValueTuple<ItemObject, float>> GetCommonLootItemScores()
		{
			return this.CommonLootItemSpawnChances;
		}

		public override int GoldRewardForEachLostHearth
		{
			get
			{
				return 4;
			}
		}

		private MBReadOnlyList<ValueTuple<ItemObject, float>> _commonLootItems;
	}
}
