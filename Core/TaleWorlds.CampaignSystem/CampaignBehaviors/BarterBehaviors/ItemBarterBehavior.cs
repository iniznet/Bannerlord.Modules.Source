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
	public class ItemBarterBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.BarterablesRequested.AddNonSerializedListener(this, new Action<BarterData>(this.CheckForBarters));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

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

		private const int ItemValueThreshold = 100;

		private ItemBarterBehavior.SettlementDistanceCache _distanceCache = new ItemBarterBehavior.SettlementDistanceCache();

		private class SettlementDistanceCache
		{
			public SettlementDistanceCache()
			{
				this._latestHeroPosition = new Vec2(-1f, -1f);
				this._sortedSettlements = new List<ItemBarterBehavior.SettlementDistanceCache.SettlementDistancePair>(64);
				this._closestSettlements = new List<Settlement>(3);
			}

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

			private Vec2 _latestHeroPosition;

			private List<ItemBarterBehavior.SettlementDistanceCache.SettlementDistancePair> _sortedSettlements;

			private List<Settlement> _closestSettlements;

			private struct SettlementDistancePair : IComparable<ItemBarterBehavior.SettlementDistanceCache.SettlementDistancePair>
			{
				public SettlementDistancePair(float distance, Settlement settlement)
				{
					this._distance = distance;
					this.Settlement = settlement;
				}

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

				private float _distance;

				public Settlement Settlement;
			}
		}
	}
}
