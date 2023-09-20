using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003DB RID: 987
	public class TradeRumorsCampaignBehavior : CampaignBehaviorBase, ITradeRumorCampaignBehavior, ICampaignBehavior
	{
		// Token: 0x17000CED RID: 3309
		// (get) Token: 0x06003BB0 RID: 15280 RVA: 0x0011A560 File Offset: 0x00118760
		public IEnumerable<TradeRumor> TradeRumors
		{
			get
			{
				foreach (TradeRumor tradeRumor in this._tradeRumors)
				{
					if (!tradeRumor.IsExpired())
					{
						yield return tradeRumor;
					}
				}
				List<TradeRumor>.Enumerator enumerator = default(List<TradeRumor>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x06003BB1 RID: 15281 RVA: 0x0011A570 File Offset: 0x00118770
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Settlement, CampaignTime>>("_enteredSettlements", ref this._enteredSettlements);
			dataStore.SyncData<List<TradeRumor>>("_tradeRumors", ref this._tradeRumors);
		}

		// Token: 0x06003BB2 RID: 15282 RVA: 0x0011A598 File Offset: 0x00118798
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnTradeRumorIsTakenEvent.AddNonSerializedListener(this, new Action<List<TradeRumor>, Settlement>(this.OnTradeRumorIsTaken));
		}

		// Token: 0x06003BB3 RID: 15283 RVA: 0x0011A601 File Offset: 0x00118801
		public void OnTradeRumorIsTaken(List<TradeRumor> newRumors, Settlement sourceSettlement = null)
		{
			this.AddTradeRumors(newRumors, sourceSettlement);
		}

		// Token: 0x06003BB4 RID: 15284 RVA: 0x0011A60C File Offset: 0x0011880C
		public void AddTradeRumors(List<TradeRumor> newRumors, Settlement sourceSettlement = null)
		{
			bool flag = true;
			foreach (TradeRumor tradeRumor in newRumors)
			{
				foreach (TradeRumor tradeRumor2 in this.TradeRumors)
				{
					if (tradeRumor2.Settlement == tradeRumor.Settlement && tradeRumor2.ItemCategory == tradeRumor.ItemCategory)
					{
						flag = false;
					}
				}
				if (flag)
				{
					this._tradeRumors.Add(tradeRumor);
				}
			}
		}

		// Token: 0x06003BB5 RID: 15285 RVA: 0x0011A6BC File Offset: 0x001188BC
		private void OnNewGameCreated(CampaignGameStarter starter)
		{
		}

		// Token: 0x06003BB6 RID: 15286 RVA: 0x0011A6BE File Offset: 0x001188BE
		public void DailyTick()
		{
			this.AddDailyTradeRumors(1);
			this.DeleteExpiredRumors();
			this.DeleteExpiredEnteredSettlements();
		}

		// Token: 0x06003BB7 RID: 15287 RVA: 0x0011A6D4 File Offset: 0x001188D4
		private void DeleteExpiredEnteredSettlements()
		{
			List<Settlement> list = new List<Settlement>();
			foreach (KeyValuePair<Settlement, CampaignTime> keyValuePair in this._enteredSettlements)
			{
				if (CampaignTime.Now - keyValuePair.Value >= CampaignTime.Days(1f))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (Settlement settlement in list)
			{
				this._enteredSettlements.Remove(settlement);
			}
		}

		// Token: 0x06003BB8 RID: 15288 RVA: 0x0011A79C File Offset: 0x0011899C
		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty == null || (!mobileParty.IsMainParty && (!mobileParty.IsCaravan || mobileParty.Party.Owner == null || mobileParty.Party.Owner.Clan != Clan.PlayerClan || !Hero.MainHero.GetPerkValue(DefaultPerks.Trade.TravelingRumors))) || !settlement.IsTown)
			{
				return;
			}
			Town town = settlement.Town;
			if (((town != null) ? town.MarketData : null) == null)
			{
				return;
			}
			if (!this._enteredSettlements.ContainsKey(settlement) || (this._enteredSettlements.ContainsKey(settlement) && CampaignTime.Now - this._enteredSettlements[settlement] >= CampaignTime.Days(1f)))
			{
				List<TradeRumor> list = new List<TradeRumor>();
				IEnumerable<TradeRumor> tradeRumors = this._tradeRumors;
				Func<TradeRumor, bool> <>9__0;
				Func<TradeRumor, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (TradeRumor x) => x.Settlement == settlement);
				}
				foreach (TradeRumor tradeRumor in tradeRumors.Where(func))
				{
					list.Add(tradeRumor);
				}
				foreach (TradeRumor tradeRumor2 in list)
				{
					this._tradeRumors.Remove(tradeRumor2);
				}
				List<TradeRumor> list2 = new List<TradeRumor>();
				foreach (ItemObject itemObject in Items.AllTradeGoods)
				{
					list2.Add(new TradeRumor(settlement, itemObject, settlement.Town.GetItemPrice(itemObject, null, false), settlement.Town.GetItemPrice(itemObject, null, true), 10));
				}
				this.AddTradeRumors(list2, settlement);
				if (!this._enteredSettlements.ContainsKey(settlement))
				{
					this._enteredSettlements.Add(settlement, CampaignTime.Now);
					return;
				}
				this._enteredSettlements[settlement] = CampaignTime.Now;
			}
		}

		// Token: 0x06003BB9 RID: 15289 RVA: 0x0011AA08 File Offset: 0x00118C08
		public void DeleteExpiredRumors()
		{
			List<TradeRumor> list = new List<TradeRumor>();
			foreach (TradeRumor tradeRumor in this._tradeRumors.Where((TradeRumor x) => x.IsExpired()))
			{
				list.Add(tradeRumor);
			}
			foreach (TradeRumor tradeRumor2 in list)
			{
				this._tradeRumors.Remove(tradeRumor2);
			}
		}

		// Token: 0x06003BBA RID: 15290 RVA: 0x0011AAC4 File Offset: 0x00118CC4
		public void AddDailyTradeRumors(int numberOfTradeRumors)
		{
			int num = 0;
			foreach (ItemObject itemObject in Items.All)
			{
				if (itemObject.Type == ItemObject.ItemTypeEnum.Goods || itemObject.Type == ItemObject.ItemTypeEnum.Horse)
				{
					num++;
				}
			}
			int count = Campaign.Current.AllTowns.Count;
			List<TradeRumor> list = new List<TradeRumor>();
			for (int i = 0; i < numberOfTradeRumors; i++)
			{
				int num2 = MBRandom.RandomInt(count);
				int num3 = MBRandom.RandomInt(num);
				foreach (Town town in Campaign.Current.AllTowns)
				{
					num2--;
					if (num2 < 0)
					{
						using (List<ItemObject>.Enumerator enumerator = Items.All.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ItemObject itemObject2 = enumerator.Current;
								if (itemObject2.Type == ItemObject.ItemTypeEnum.Goods || itemObject2.Type == ItemObject.ItemTypeEnum.Horse)
								{
									num3--;
									if (num3 < 0)
									{
										list.Add(new TradeRumor(town.Settlement, itemObject2, town.GetItemPrice(itemObject2, null, false), town.GetItemPrice(itemObject2, null, true), 10 + MBRandom.RandomInt(10)));
										break;
									}
								}
							}
							break;
						}
					}
				}
				if (Hero.MainHero.GetPerkValue(DefaultPerks.Trade.Tollgates))
				{
					foreach (Workshop workshop in Hero.MainHero.OwnedWorkshops)
					{
						foreach (ItemObject itemObject3 in Items.AllTradeGoods)
						{
							list.Add(new TradeRumor(workshop.Settlement, itemObject3, workshop.Settlement.Town.GetItemPrice(itemObject3, null, false), workshop.Settlement.Town.GetItemPrice(itemObject3, null, true), 10));
						}
					}
				}
			}
			this.AddTradeRumors(list, null);
		}

		// Token: 0x0400122F RID: 4655
		private List<TradeRumor> _tradeRumors = new List<TradeRumor>();

		// Token: 0x04001230 RID: 4656
		private Dictionary<Settlement, CampaignTime> _enteredSettlements = new Dictionary<Settlement, CampaignTime>();
	}
}
