﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Settlements
{
	public class VillageMarketData : IMarketData
	{
		internal static void AutoGeneratedStaticCollectObjectsVillageMarketData(object o, List<object> collectedObjects)
		{
			((VillageMarketData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._village);
		}

		internal static object AutoGeneratedGetMemberValue_village(object o)
		{
			return ((VillageMarketData)o)._village;
		}

		private Settlement ClosestTown
		{
			get
			{
				if (this._closestTown == null)
				{
					this._closestTown = SettlementHelper.FindNearestTown(null, this._village.Settlement);
				}
				return this._closestTown;
			}
		}

		public VillageMarketData(Village village)
		{
			this._village = village;
		}

		public int GetPrice(ItemObject item, MobileParty tradingParty, bool isSelling, PartyBase merchantParty)
		{
			return this.GetPrice(new EquipmentElement(item, null, null, false), tradingParty, isSelling, merchantParty);
		}

		public int GetPrice(EquipmentElement itemRosterElement, MobileParty tradingParty, bool isSelling, PartyBase merchantParty)
		{
			ItemData categoryData = (this._village.TradeBound ?? this.ClosestTown).Town.MarketData.GetCategoryData(itemRosterElement.Item.GetItemCategory());
			return Campaign.Current.Models.TradeItemPriceFactorModel.GetPrice(itemRosterElement, tradingParty, merchantParty, isSelling, (float)categoryData.InStoreValue, categoryData.Supply, categoryData.Demand);
		}

		[SaveableField(1)]
		private Village _village;

		private Settlement _closestTown;
	}
}