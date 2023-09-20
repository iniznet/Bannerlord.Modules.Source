using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000037 RID: 55
	public class CampaignPeriodicEventManager
	{
		// Token: 0x1700018F RID: 399
		// (get) Token: 0x060006F5 RID: 1781 RVA: 0x0001F1C4 File Offset: 0x0001D3C4
		private double DeltaHours
		{
			get
			{
				return CampaignTime.DeltaTime.ToHours;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x060006F6 RID: 1782 RVA: 0x0001F1E0 File Offset: 0x0001D3E0
		private double DeltaDays
		{
			get
			{
				return CampaignTime.DeltaTime.ToDays;
			}
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0001F1FC File Offset: 0x0001D3FC
		internal CampaignPeriodicEventManager()
		{
			this._mobilePartyHourlyTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._mobilePartyDailyTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._hourlyTickMobilePartyTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._hourlyTickSettlementTicker = new CampaignPeriodicEventManager.PeriodicTicker<Settlement>();
			this._dailyTickSettlementTicker = new CampaignPeriodicEventManager.PeriodicTicker<Settlement>();
			this._hourlyTickClanTicker = new CampaignPeriodicEventManager.PeriodicTicker<Clan>();
			this._dailyTickPartyTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._dailyTickTownTicker = new CampaignPeriodicEventManager.PeriodicTicker<Town>();
			this._dailyTickHeroTicker = new CampaignPeriodicEventManager.PeriodicTicker<Hero>();
			this._dailyTickClanTicker = new CampaignPeriodicEventManager.PeriodicTicker<Clan>();
			this._quarterDailyPartyTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._caravanMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._garrisonMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._militiaMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._villagerMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._customMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._banditMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._lordMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			this._partiesWithoutPartyComponentsPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0001F2EC File Offset: 0x0001D4EC
		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData)
		{
			if (this._caravanMobilePartyPartialHourlyAiEventTicker == null)
			{
				this._caravanMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
				this._garrisonMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
				this._militiaMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
				this._villagerMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
				this._customMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
				this._banditMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
				this._lordMobilePartyPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
				this._partiesWithoutPartyComponentsPartialHourlyAiEventTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
				this._quarterDailyPartyTicker = new CampaignPeriodicEventManager.PeriodicTicker<MobileParty>();
			}
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0001F364 File Offset: 0x0001D564
		internal void InitializeTickers()
		{
			MBList<Settlement> mblist = this.ShuffleSettlements();
			this._mobilePartyHourlyTicker.Initialize(MobileParty.All, delegate(MobileParty x)
			{
				x.HourlyTick();
			}, false);
			this._mobilePartyDailyTicker.Initialize(MobileParty.All, delegate(MobileParty x)
			{
				x.DailyTick();
			}, false);
			this._hourlyTickMobilePartyTicker.Initialize(MobileParty.All, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.HourlyTickParty(x);
			}, false);
			this._hourlyTickSettlementTicker.Initialize(mblist, delegate(Settlement x)
			{
				CampaignEventDispatcher.Instance.HourlyTickSettlement(x);
			}, false);
			this._dailyTickSettlementTicker.Initialize(mblist, delegate(Settlement x)
			{
				CampaignEventDispatcher.Instance.DailyTickSettlement(x);
			}, false);
			this._hourlyTickClanTicker.Initialize(Clan.All, delegate(Clan x)
			{
				CampaignEventDispatcher.Instance.HourlyTickClan(x);
			}, false);
			this._dailyTickPartyTicker.Initialize(MobileParty.All, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.DailyTickParty(x);
			}, false);
			this._dailyTickTownTicker.Initialize(Town.AllTowns, delegate(Town x)
			{
				CampaignEventDispatcher.Instance.DailyTickTown(x);
			}, false);
			this._dailyTickHeroTicker.Initialize(Hero.AllAliveHeroes, delegate(Hero x)
			{
				CampaignEventDispatcher.Instance.DailyTickHero(x);
			}, false);
			this._dailyTickClanTicker.Initialize(Clan.All, delegate(Clan x)
			{
				CampaignEventDispatcher.Instance.DailyTickClan(x);
			}, false);
			bool flag = false;
			this._caravanMobilePartyPartialHourlyAiEventTicker.Initialize(MobileParty.AllCaravanParties, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.TickPartialHourlyAi(x);
			}, flag);
			this._garrisonMobilePartyPartialHourlyAiEventTicker.Initialize(MobileParty.AllGarrisonParties, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.TickPartialHourlyAi(x);
			}, flag);
			this._militiaMobilePartyPartialHourlyAiEventTicker.Initialize(MobileParty.AllMilitiaParties, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.TickPartialHourlyAi(x);
			}, flag);
			this._villagerMobilePartyPartialHourlyAiEventTicker.Initialize(MobileParty.AllVillagerParties, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.TickPartialHourlyAi(x);
			}, flag);
			this._customMobilePartyPartialHourlyAiEventTicker.Initialize(MobileParty.AllCustomParties, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.TickPartialHourlyAi(x);
			}, flag);
			this._banditMobilePartyPartialHourlyAiEventTicker.Initialize(MobileParty.AllBanditParties, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.TickPartialHourlyAi(x);
			}, flag);
			this._lordMobilePartyPartialHourlyAiEventTicker.Initialize(MobileParty.AllLordParties, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.TickPartialHourlyAi(x);
			}, flag);
			this._partiesWithoutPartyComponentsPartialHourlyAiEventTicker.Initialize(MobileParty.AllPartiesWithoutPartyComponent, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.TickPartialHourlyAi(x);
			}, flag);
			this._quarterDailyPartyTicker.Initialize(MobileParty.All, delegate(MobileParty x)
			{
				CampaignEventDispatcher.Instance.QuarterDailyPartyTick(x);
			}, false);
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x0001F704 File Offset: 0x0001D904
		private MBList<Settlement> ShuffleSettlements()
		{
			Stack<Settlement> stack = new Stack<Settlement>();
			Stack<Settlement> stack2 = new Stack<Settlement>();
			Stack<Settlement> stack3 = new Stack<Settlement>();
			Stack<Settlement> stack4 = new Stack<Settlement>();
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsVillage)
				{
					stack.Push(settlement);
				}
				else if (settlement.IsCastle)
				{
					stack2.Push(settlement);
				}
				else if (settlement.IsTown)
				{
					stack3.Push(settlement);
				}
				else
				{
					stack4.Push(settlement);
				}
			}
			float num = (float)Settlement.All.Count;
			float num2 = (float)stack.Count / num;
			float num3 = (float)stack2.Count / num;
			float num4 = (float)stack3.Count / num;
			float num5 = (float)stack4.Count / num;
			float num6 = num2;
			float num7 = num3;
			float num8 = num4;
			float num9 = num5;
			MBList<Settlement> mblist = new MBList<Settlement>();
			while (mblist.Count != Settlement.All.Count)
			{
				num6 += num2;
				if (num6 >= 1f && !stack.IsEmpty<Settlement>())
				{
					mblist.Add(stack.Pop());
					num6 -= 1f;
				}
				num7 += num3;
				if (num7 >= 1f && !stack2.IsEmpty<Settlement>())
				{
					mblist.Add(stack2.Pop());
					num7 -= 1f;
				}
				num8 += num4;
				if (num8 >= 1f && !stack3.IsEmpty<Settlement>())
				{
					mblist.Add(stack3.Pop());
					num8 -= 1f;
				}
				num9 += num5;
				if (num9 >= 1f && !stack4.IsEmpty<Settlement>())
				{
					mblist.Add(stack4.Pop());
					num9 -= 1f;
				}
			}
			return mblist;
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0001F8D8 File Offset: 0x0001DAD8
		internal void TickPeriodicEvents()
		{
			this.PeriodicHourlyTick();
			this.PeriodicDailyTick();
			this.PeriodicQuarterDailyTick();
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0001F8EC File Offset: 0x0001DAEC
		private void PeriodicQuarterDailyTick()
		{
			double deltaDays = this.DeltaDays;
			this._quarterDailyPartyTicker.PeriodicTickSome(deltaDays * 4.0);
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0001F916 File Offset: 0x0001DB16
		internal void MobilePartyHourlyTick()
		{
			this._mobilePartyHourlyTicker.PeriodicTickSome(this.DeltaHours);
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0001F92C File Offset: 0x0001DB2C
		internal void TickPartialHourlyAi()
		{
			this._caravanMobilePartyPartialHourlyAiEventTicker.PeriodicTickSome(this.DeltaHours * 0.99);
			this._garrisonMobilePartyPartialHourlyAiEventTicker.PeriodicTickSome(this.DeltaHours * 0.99);
			this._militiaMobilePartyPartialHourlyAiEventTicker.PeriodicTickSome(this.DeltaHours * 0.99);
			this._villagerMobilePartyPartialHourlyAiEventTicker.PeriodicTickSome(this.DeltaHours * 0.99);
			this._customMobilePartyPartialHourlyAiEventTicker.PeriodicTickSome(this.DeltaHours * 0.99);
			this._banditMobilePartyPartialHourlyAiEventTicker.PeriodicTickSome(this.DeltaHours * 0.99);
			this._lordMobilePartyPartialHourlyAiEventTicker.PeriodicTickSome(this.DeltaHours * 0.99);
			this._partiesWithoutPartyComponentsPartialHourlyAiEventTicker.PeriodicTickSome(this.DeltaHours * 0.99);
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x0001FA14 File Offset: 0x0001DC14
		private void PeriodicHourlyTick()
		{
			double deltaHours = this.DeltaHours;
			this._hourlyTickMobilePartyTicker.PeriodicTickSome(deltaHours);
			this._hourlyTickSettlementTicker.PeriodicTickSome(deltaHours);
			this._hourlyTickClanTicker.PeriodicTickSome(deltaHours);
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x0001FA4C File Offset: 0x0001DC4C
		private void PeriodicDailyTick()
		{
			double deltaDays = this.DeltaDays;
			this._dailyTickPartyTicker.PeriodicTickSome(deltaDays);
			this._mobilePartyDailyTicker.PeriodicTickSome(deltaDays);
			this._dailyTickTownTicker.PeriodicTickSome(deltaDays);
			this._dailyTickSettlementTicker.PeriodicTickSome(deltaDays);
			this._dailyTickHeroTicker.PeriodicTickSome(deltaDays);
			this._dailyTickClanTicker.PeriodicTickSome(deltaDays);
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0001FAA8 File Offset: 0x0001DCA8
		public static MBCampaignEvent CreatePeriodicEvent(CampaignTime triggerPeriod, CampaignTime initialWait)
		{
			MBCampaignEvent mbcampaignEvent = new MBCampaignEvent(triggerPeriod, initialWait);
			Campaign.Current.CustomPeriodicCampaignEvents.Add(mbcampaignEvent);
			return mbcampaignEvent;
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x0001FAD0 File Offset: 0x0001DCD0
		private void DeleteMarkedPeriodicEvents()
		{
			List<MBCampaignEvent> customPeriodicCampaignEvents = Campaign.Current.CustomPeriodicCampaignEvents;
			for (int i = customPeriodicCampaignEvents.Count - 1; i >= 0; i--)
			{
				if (customPeriodicCampaignEvents[i].isEventDeleted)
				{
					customPeriodicCampaignEvents.RemoveAt(i);
				}
			}
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x0001FB10 File Offset: 0x0001DD10
		internal void OnTick(float dt)
		{
			this.SignalPeriodicEvents();
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x0001FB18 File Offset: 0x0001DD18
		private void SignalPeriodicEvents()
		{
			if ((this._lastGameTime + CampaignPeriodicEventManager.MinimumPeriodicEventInterval).IsPast)
			{
				this._lastGameTime = CampaignTime.Now;
				List<MBCampaignEvent> customPeriodicCampaignEvents = Campaign.Current.CustomPeriodicCampaignEvents;
				for (int i = customPeriodicCampaignEvents.Count - 1; i >= 0; i--)
				{
					customPeriodicCampaignEvents[i].CheckUpdate();
				}
				this.DeleteMarkedPeriodicEvents();
				MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
				if (mapState == null)
				{
					return;
				}
				mapState.OnSignalPeriodicEvents();
			}
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0001FB98 File Offset: 0x0001DD98
		internal static void AutoGeneratedStaticCollectObjectsCampaignPeriodicEventManager(object o, List<object> collectedObjects)
		{
			((CampaignPeriodicEventManager)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x0001FBA8 File Offset: 0x0001DDA8
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._mobilePartyHourlyTicker);
			collectedObjects.Add(this._mobilePartyDailyTicker);
			collectedObjects.Add(this._dailyTickPartyTicker);
			collectedObjects.Add(this._hourlyTickMobilePartyTicker);
			collectedObjects.Add(this._hourlyTickSettlementTicker);
			collectedObjects.Add(this._hourlyTickClanTicker);
			collectedObjects.Add(this._dailyTickTownTicker);
			collectedObjects.Add(this._dailyTickSettlementTicker);
			collectedObjects.Add(this._dailyTickHeroTicker);
			collectedObjects.Add(this._dailyTickClanTicker);
			collectedObjects.Add(this._quarterDailyPartyTicker);
			collectedObjects.Add(this._caravanMobilePartyPartialHourlyAiEventTicker);
			collectedObjects.Add(this._garrisonMobilePartyPartialHourlyAiEventTicker);
			collectedObjects.Add(this._militiaMobilePartyPartialHourlyAiEventTicker);
			collectedObjects.Add(this._villagerMobilePartyPartialHourlyAiEventTicker);
			collectedObjects.Add(this._customMobilePartyPartialHourlyAiEventTicker);
			collectedObjects.Add(this._banditMobilePartyPartialHourlyAiEventTicker);
			collectedObjects.Add(this._lordMobilePartyPartialHourlyAiEventTicker);
			collectedObjects.Add(this._partiesWithoutPartyComponentsPartialHourlyAiEventTicker);
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x0001FC99 File Offset: 0x0001DE99
		internal static object AutoGeneratedGetMemberValue_mobilePartyHourlyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._mobilePartyHourlyTicker;
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x0001FCA6 File Offset: 0x0001DEA6
		internal static object AutoGeneratedGetMemberValue_mobilePartyDailyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._mobilePartyDailyTicker;
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x0001FCB3 File Offset: 0x0001DEB3
		internal static object AutoGeneratedGetMemberValue_dailyTickPartyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickPartyTicker;
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x0001FCC0 File Offset: 0x0001DEC0
		internal static object AutoGeneratedGetMemberValue_hourlyTickMobilePartyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._hourlyTickMobilePartyTicker;
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x0001FCCD File Offset: 0x0001DECD
		internal static object AutoGeneratedGetMemberValue_hourlyTickSettlementTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._hourlyTickSettlementTicker;
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0001FCDA File Offset: 0x0001DEDA
		internal static object AutoGeneratedGetMemberValue_hourlyTickClanTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._hourlyTickClanTicker;
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x0001FCE7 File Offset: 0x0001DEE7
		internal static object AutoGeneratedGetMemberValue_dailyTickTownTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickTownTicker;
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0001FCF4 File Offset: 0x0001DEF4
		internal static object AutoGeneratedGetMemberValue_dailyTickSettlementTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickSettlementTicker;
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0001FD01 File Offset: 0x0001DF01
		internal static object AutoGeneratedGetMemberValue_dailyTickHeroTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickHeroTicker;
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x0001FD0E File Offset: 0x0001DF0E
		internal static object AutoGeneratedGetMemberValue_dailyTickClanTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickClanTicker;
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x0001FD1B File Offset: 0x0001DF1B
		internal static object AutoGeneratedGetMemberValue_quarterDailyPartyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._quarterDailyPartyTicker;
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x0001FD28 File Offset: 0x0001DF28
		internal static object AutoGeneratedGetMemberValue_caravanMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._caravanMobilePartyPartialHourlyAiEventTicker;
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x0001FD35 File Offset: 0x0001DF35
		internal static object AutoGeneratedGetMemberValue_garrisonMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._garrisonMobilePartyPartialHourlyAiEventTicker;
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x0001FD42 File Offset: 0x0001DF42
		internal static object AutoGeneratedGetMemberValue_militiaMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._militiaMobilePartyPartialHourlyAiEventTicker;
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0001FD4F File Offset: 0x0001DF4F
		internal static object AutoGeneratedGetMemberValue_villagerMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._villagerMobilePartyPartialHourlyAiEventTicker;
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x0001FD5C File Offset: 0x0001DF5C
		internal static object AutoGeneratedGetMemberValue_customMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._customMobilePartyPartialHourlyAiEventTicker;
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x0001FD69 File Offset: 0x0001DF69
		internal static object AutoGeneratedGetMemberValue_banditMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._banditMobilePartyPartialHourlyAiEventTicker;
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0001FD76 File Offset: 0x0001DF76
		internal static object AutoGeneratedGetMemberValue_lordMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._lordMobilePartyPartialHourlyAiEventTicker;
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x0001FD83 File Offset: 0x0001DF83
		internal static object AutoGeneratedGetMemberValue_partiesWithoutPartyComponentsPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._partiesWithoutPartyComponentsPartialHourlyAiEventTicker;
		}

		// Token: 0x0400026E RID: 622
		[SaveableField(120)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _mobilePartyHourlyTicker;

		// Token: 0x0400026F RID: 623
		[SaveableField(130)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _mobilePartyDailyTicker;

		// Token: 0x04000270 RID: 624
		[SaveableField(140)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _dailyTickPartyTicker;

		// Token: 0x04000271 RID: 625
		[SaveableField(150)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _hourlyTickMobilePartyTicker;

		// Token: 0x04000272 RID: 626
		[SaveableField(160)]
		private CampaignPeriodicEventManager.PeriodicTicker<Settlement> _hourlyTickSettlementTicker;

		// Token: 0x04000273 RID: 627
		[SaveableField(170)]
		private CampaignPeriodicEventManager.PeriodicTicker<Clan> _hourlyTickClanTicker;

		// Token: 0x04000274 RID: 628
		[SaveableField(180)]
		private CampaignPeriodicEventManager.PeriodicTicker<Town> _dailyTickTownTicker;

		// Token: 0x04000275 RID: 629
		[SaveableField(190)]
		private CampaignPeriodicEventManager.PeriodicTicker<Settlement> _dailyTickSettlementTicker;

		// Token: 0x04000276 RID: 630
		[SaveableField(200)]
		private CampaignPeriodicEventManager.PeriodicTicker<Hero> _dailyTickHeroTicker;

		// Token: 0x04000277 RID: 631
		[SaveableField(210)]
		private CampaignPeriodicEventManager.PeriodicTicker<Clan> _dailyTickClanTicker;

		// Token: 0x04000278 RID: 632
		[SaveableField(320)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _quarterDailyPartyTicker;

		// Token: 0x04000279 RID: 633
		[SaveableField(230)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _caravanMobilePartyPartialHourlyAiEventTicker;

		// Token: 0x0400027A RID: 634
		[SaveableField(250)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _garrisonMobilePartyPartialHourlyAiEventTicker;

		// Token: 0x0400027B RID: 635
		[SaveableField(260)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _militiaMobilePartyPartialHourlyAiEventTicker;

		// Token: 0x0400027C RID: 636
		[SaveableField(270)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _villagerMobilePartyPartialHourlyAiEventTicker;

		// Token: 0x0400027D RID: 637
		[SaveableField(280)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _customMobilePartyPartialHourlyAiEventTicker;

		// Token: 0x0400027E RID: 638
		[SaveableField(290)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _banditMobilePartyPartialHourlyAiEventTicker;

		// Token: 0x0400027F RID: 639
		[SaveableField(300)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _lordMobilePartyPartialHourlyAiEventTicker;

		// Token: 0x04000280 RID: 640
		[SaveableField(310)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _partiesWithoutPartyComponentsPartialHourlyAiEventTicker;

		// Token: 0x04000281 RID: 641
		private static readonly CampaignTime MinimumPeriodicEventInterval = CampaignTime.Hours(0.05f);

		// Token: 0x04000282 RID: 642
		private CampaignTime _lastGameTime = CampaignTime.Zero;

		// Token: 0x02000489 RID: 1161
		internal class PeriodicTicker<T>
		{
			// Token: 0x17000D57 RID: 3415
			// (get) Token: 0x0600400D RID: 16397 RVA: 0x00130E90 File Offset: 0x0012F090
			// (set) Token: 0x0600400E RID: 16398 RVA: 0x00130E98 File Offset: 0x0012F098
			[SaveableProperty(1)]
			private double TickDebt { get; set; }

			// Token: 0x17000D58 RID: 3416
			// (get) Token: 0x0600400F RID: 16399 RVA: 0x00130EA1 File Offset: 0x0012F0A1
			// (set) Token: 0x06004010 RID: 16400 RVA: 0x00130EA9 File Offset: 0x0012F0A9
			[SaveableProperty(2)]
			private int Index { get; set; }

			// Token: 0x06004011 RID: 16401 RVA: 0x00130EB2 File Offset: 0x0012F0B2
			internal PeriodicTicker()
			{
				this.TickDebt = 0.0;
				this.Index = -1;
			}

			// Token: 0x06004012 RID: 16402 RVA: 0x00130EDB File Offset: 0x0012F0DB
			internal void Initialize(MBReadOnlyList<T> list, Action<T> action, bool doParallel)
			{
				this._list = list;
				this._action = action;
				this._doParallel = doParallel;
			}

			// Token: 0x06004013 RID: 16403 RVA: 0x00130EF4 File Offset: 0x0012F0F4
			internal void PeriodicTickSome(double timeUnitsElapsed)
			{
				if (this._list.Count == 0)
				{
					this.TickDebt = 0.0;
					return;
				}
				this.TickDebt += timeUnitsElapsed * (double)this._list.Count;
				while (this.TickDebt > 1.0)
				{
					this.Index++;
					if (this.Index >= this._list.Count)
					{
						this.Index = 0;
					}
					if (this._doParallel)
					{
						this._currentFrameToTickListFlattened.Add(this._list[this.Index]);
					}
					else
					{
						this._action(this._list[this.Index]);
					}
					this.TickDebt -= 1.0;
				}
				if (this._doParallel && this._currentFrameToTickListFlattened.Count > 0)
				{
					TWParallel.For(0, this._currentFrameToTickListFlattened.Count, delegate(int startInclusive, int endExclusive)
					{
						for (int i = startInclusive; i < endExclusive; i++)
						{
							this._action(this._currentFrameToTickListFlattened[i]);
						}
					}, 1);
					this._currentFrameToTickListFlattened.Clear();
				}
			}

			// Token: 0x06004014 RID: 16404 RVA: 0x00131014 File Offset: 0x0012F214
			public override string ToString()
			{
				object[] array = new object[7];
				array[0] = "PeriodicTicker  @";
				int num = 1;
				object obj;
				if (this.Index != -1)
				{
					T t = this._list[this.Index];
					obj = t.ToString();
				}
				else
				{
					obj = "null";
				}
				array[num] = obj;
				array[2] = "\t\t(";
				array[3] = this.Index;
				array[4] = " / ";
				array[5] = this._list.Count;
				array[6] = ")";
				return string.Concat(array);
			}

			// Token: 0x06004015 RID: 16405 RVA: 0x0013109F File Offset: 0x0012F29F
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
			}

			// Token: 0x040013A6 RID: 5030
			private readonly List<T> _currentFrameToTickListFlattened = new List<T>();

			// Token: 0x040013A9 RID: 5033
			private bool _doParallel;

			// Token: 0x040013AA RID: 5034
			private MBReadOnlyList<T> _list;

			// Token: 0x040013AB RID: 5035
			private Action<T> _action;
		}
	}
}
