﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Settlements
{
	internal class FakeMarketData : IMarketData
	{
		internal static void AutoGeneratedStaticCollectObjectsFakeMarketData(object o, List<object> collectedObjects)
		{
			((FakeMarketData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		public int GetPrice(ItemObject item, MobileParty tradingParty, bool isSelling, PartyBase merchantParty)
		{
			return item.Value;
		}

		public int GetPrice(EquipmentElement itemRosterElement, MobileParty tradingParty, bool isSelling, PartyBase merchantParty)
		{
			return itemRosterElement.ItemValue;
		}
	}
}
