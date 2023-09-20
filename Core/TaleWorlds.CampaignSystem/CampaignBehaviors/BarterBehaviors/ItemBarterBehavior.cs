using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors
{
	// Token: 0x020003F9 RID: 1017
	public class ItemBarterBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003D0A RID: 15626 RVA: 0x001226A1 File Offset: 0x001208A1
		public override void RegisterEvents()
		{
			CampaignEvents.BarterablesRequested.AddNonSerializedListener(this, new Action<BarterData>(this.CheckForBarters));
		}

		// Token: 0x06003D0B RID: 15627 RVA: 0x001226BA File Offset: 0x001208BA
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003D0C RID: 15628 RVA: 0x001226BC File Offset: 0x001208BC
		public void CheckForBarters(BarterData args)
		{
			Vec2 vec;
			if (args.OffererHero != null)
			{
				vec = args.OffererHero.GetPosition().AsVec2;
			}
			else if (args.OffererParty != null)
			{
				vec = args.OffererParty.MobileParty.GetPosition().AsVec2;
			}
			else
			{
				vec = args.OtherHero.GetPosition().AsVec2;
			}
			List<Settlement> closestSettlements = this._distanceCache.GetClosestSettlements(vec);
			if (args.OffererParty != null && args.OtherParty != null)
			{
				for (int i = 0; i < args.OffererParty.ItemRoster.Count; i++)
				{
					ItemRosterElement elementCopyAtIndex = args.OffererParty.ItemRoster.GetElementCopyAtIndex(i);
					if (elementCopyAtIndex.Amount > 0 && elementCopyAtIndex.EquipmentElement.GetBaseValue() > 100)
					{
						int num = this.CalculateAverageItemValueInNearbySettlements(elementCopyAtIndex.EquipmentElement, args.OffererParty, closestSettlements);
						Barterable barterable = new ItemBarterable(args.OffererHero, args.OtherHero, args.OffererParty, args.OtherParty, elementCopyAtIndex, num);
						args.AddBarterable<ItemBarterGroup>(barterable, false);
					}
				}
				for (int j = 0; j < args.OtherParty.ItemRoster.Count; j++)
				{
					ItemRosterElement elementCopyAtIndex2 = args.OtherParty.ItemRoster.GetElementCopyAtIndex(j);
					if (elementCopyAtIndex2.Amount > 0 && elementCopyAtIndex2.EquipmentElement.GetBaseValue() > 100)
					{
						int num2 = this.CalculateAverageItemValueInNearbySettlements(elementCopyAtIndex2.EquipmentElement, args.OtherParty, closestSettlements);
						Barterable barterable2 = new ItemBarterable(args.OtherHero, args.OffererHero, args.OtherParty, args.OffererParty, elementCopyAtIndex2, num2);
						args.AddBarterable<ItemBarterGroup>(barterable2, false);
					}
				}
			}
		}

		// Token: 0x06003D0D RID: 15629 RVA: 0x00122868 File Offset: 0x00120A68
		private int CalculateAverageItemValueInNearbySettlements(EquipmentElement itemRosterElement, PartyBase involvedParty, List<Settlement> nearbySettlements)
		{
			int num = 0;
			if (!nearbySettlements.IsEmpty<Settlement>())
			{
				foreach (Settlement settlement in nearbySettlements)
				{
					num += settlement.Town.GetItemPrice(itemRosterElement, involvedParty.MobileParty, true);
				}
				num /= nearbySettlements.Count;
			}
			return num;
		}

		// Token: 0x0400125D RID: 4701
		private const int ItemValueThreshold = 100;

		// Token: 0x0400125E RID: 4702
		private ItemBarterBehavior.SettlementDistanceCache _distanceCache = new ItemBarterBehavior.SettlementDistanceCache();

		// Token: 0x02000748 RID: 1864
		private class SettlementDistanceCache
		{
			// Token: 0x06005680 RID: 22144 RVA: 0x0016E6A8 File Offset: 0x0016C8A8
			public SettlementDistanceCache()
			{
				this._latestHeroPosition = new Vec2(-1f, -1f);
				this._sortedSettlements = new List<ItemBarterBehavior.SettlementDistanceCache.SettlementDistancePair>(64);
				this._closestSettlements = new List<Settlement>(3);
			}

			// Token: 0x06005681 RID: 22145 RVA: 0x0016E6E0 File Offset: 0x0016C8E0
			public List<Settlement> GetClosestSettlements(Vec2 position)
			{
				if (!position.NearlyEquals(this._latestHeroPosition, 1E-05f))
				{
					this._latestHeroPosition = position;
					MBReadOnlyList<Settlement> all = Settlement.All;
					int count = all.Count;
					for (int i = 0; i < count; i++)
					{
						Settlement settlement = all[i];
						if (settlement.IsTown)
						{
							this._sortedSettlements.Add(new ItemBarterBehavior.SettlementDistanceCache.SettlementDistancePair(position.DistanceSquared(settlement.GatePosition), settlement));
						}
					}
					this._sortedSettlements.Sort();
					this._closestSettlements.Clear();
					this._closestSettlements.Add(this._sortedSettlements[0].Settlement);
					this._closestSettlements.Add(this._sortedSettlements[1].Settlement);
					this._closestSettlements.Add(this._sortedSettlements[2].Settlement);
					this._sortedSettlements.Clear();
				}
				return this._closestSettlements;
			}

			// Token: 0x04001E01 RID: 7681
			private Vec2 _latestHeroPosition;

			// Token: 0x04001E02 RID: 7682
			private List<ItemBarterBehavior.SettlementDistanceCache.SettlementDistancePair> _sortedSettlements;

			// Token: 0x04001E03 RID: 7683
			private List<Settlement> _closestSettlements;

			// Token: 0x020007BC RID: 1980
			private struct SettlementDistancePair : IComparable<ItemBarterBehavior.SettlementDistanceCache.SettlementDistancePair>
			{
				// Token: 0x060057C5 RID: 22469 RVA: 0x0017010C File Offset: 0x0016E30C
				public SettlementDistancePair(float distance, Settlement settlement)
				{
					this._distance = distance;
					this.Settlement = settlement;
				}

				// Token: 0x060057C6 RID: 22470 RVA: 0x0017011C File Offset: 0x0016E31C
				public int CompareTo(ItemBarterBehavior.SettlementDistanceCache.SettlementDistancePair other)
				{
					if (this._distance == other._distance)
					{
						return 0;
					}
					if (this._distance > other._distance)
					{
						return 1;
					}
					return -1;
				}

				// Token: 0x04001F4D RID: 8013
				private float _distance;

				// Token: 0x04001F4E RID: 8014
				public Settlement Settlement;
			}
		}
	}
}
