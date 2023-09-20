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
	public class CampaignPeriodicEventManager
	{
		private double DeltaHours
		{
			get
			{
				return CampaignTime.DeltaTime.ToHours;
			}
		}

		private double DeltaDays
		{
			get
			{
				return CampaignTime.DeltaTime.ToDays;
			}
		}

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

		internal void TickPeriodicEvents()
		{
			this.PeriodicHourlyTick();
			this.PeriodicDailyTick();
			this.PeriodicQuarterDailyTick();
		}

		private void PeriodicQuarterDailyTick()
		{
			double deltaDays = this.DeltaDays;
			this._quarterDailyPartyTicker.PeriodicTickSome(deltaDays * 4.0);
		}

		internal void MobilePartyHourlyTick()
		{
			this._mobilePartyHourlyTicker.PeriodicTickSome(this.DeltaHours);
		}

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

		private void PeriodicHourlyTick()
		{
			double deltaHours = this.DeltaHours;
			this._hourlyTickMobilePartyTicker.PeriodicTickSome(deltaHours);
			this._hourlyTickSettlementTicker.PeriodicTickSome(deltaHours);
			this._hourlyTickClanTicker.PeriodicTickSome(deltaHours);
		}

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

		public static MBCampaignEvent CreatePeriodicEvent(CampaignTime triggerPeriod, CampaignTime initialWait)
		{
			MBCampaignEvent mbcampaignEvent = new MBCampaignEvent(triggerPeriod, initialWait);
			Campaign.Current.CustomPeriodicCampaignEvents.Add(mbcampaignEvent);
			return mbcampaignEvent;
		}

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

		internal void OnTick(float dt)
		{
			this.SignalPeriodicEvents();
		}

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

		internal static void AutoGeneratedStaticCollectObjectsCampaignPeriodicEventManager(object o, List<object> collectedObjects)
		{
			((CampaignPeriodicEventManager)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

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

		internal static object AutoGeneratedGetMemberValue_mobilePartyHourlyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._mobilePartyHourlyTicker;
		}

		internal static object AutoGeneratedGetMemberValue_mobilePartyDailyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._mobilePartyDailyTicker;
		}

		internal static object AutoGeneratedGetMemberValue_dailyTickPartyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickPartyTicker;
		}

		internal static object AutoGeneratedGetMemberValue_hourlyTickMobilePartyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._hourlyTickMobilePartyTicker;
		}

		internal static object AutoGeneratedGetMemberValue_hourlyTickSettlementTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._hourlyTickSettlementTicker;
		}

		internal static object AutoGeneratedGetMemberValue_hourlyTickClanTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._hourlyTickClanTicker;
		}

		internal static object AutoGeneratedGetMemberValue_dailyTickTownTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickTownTicker;
		}

		internal static object AutoGeneratedGetMemberValue_dailyTickSettlementTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickSettlementTicker;
		}

		internal static object AutoGeneratedGetMemberValue_dailyTickHeroTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickHeroTicker;
		}

		internal static object AutoGeneratedGetMemberValue_dailyTickClanTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._dailyTickClanTicker;
		}

		internal static object AutoGeneratedGetMemberValue_quarterDailyPartyTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._quarterDailyPartyTicker;
		}

		internal static object AutoGeneratedGetMemberValue_caravanMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._caravanMobilePartyPartialHourlyAiEventTicker;
		}

		internal static object AutoGeneratedGetMemberValue_garrisonMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._garrisonMobilePartyPartialHourlyAiEventTicker;
		}

		internal static object AutoGeneratedGetMemberValue_militiaMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._militiaMobilePartyPartialHourlyAiEventTicker;
		}

		internal static object AutoGeneratedGetMemberValue_villagerMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._villagerMobilePartyPartialHourlyAiEventTicker;
		}

		internal static object AutoGeneratedGetMemberValue_customMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._customMobilePartyPartialHourlyAiEventTicker;
		}

		internal static object AutoGeneratedGetMemberValue_banditMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._banditMobilePartyPartialHourlyAiEventTicker;
		}

		internal static object AutoGeneratedGetMemberValue_lordMobilePartyPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._lordMobilePartyPartialHourlyAiEventTicker;
		}

		internal static object AutoGeneratedGetMemberValue_partiesWithoutPartyComponentsPartialHourlyAiEventTicker(object o)
		{
			return ((CampaignPeriodicEventManager)o)._partiesWithoutPartyComponentsPartialHourlyAiEventTicker;
		}

		[SaveableField(120)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _mobilePartyHourlyTicker;

		[SaveableField(130)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _mobilePartyDailyTicker;

		[SaveableField(140)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _dailyTickPartyTicker;

		[SaveableField(150)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _hourlyTickMobilePartyTicker;

		[SaveableField(160)]
		private CampaignPeriodicEventManager.PeriodicTicker<Settlement> _hourlyTickSettlementTicker;

		[SaveableField(170)]
		private CampaignPeriodicEventManager.PeriodicTicker<Clan> _hourlyTickClanTicker;

		[SaveableField(180)]
		private CampaignPeriodicEventManager.PeriodicTicker<Town> _dailyTickTownTicker;

		[SaveableField(190)]
		private CampaignPeriodicEventManager.PeriodicTicker<Settlement> _dailyTickSettlementTicker;

		[SaveableField(200)]
		private CampaignPeriodicEventManager.PeriodicTicker<Hero> _dailyTickHeroTicker;

		[SaveableField(210)]
		private CampaignPeriodicEventManager.PeriodicTicker<Clan> _dailyTickClanTicker;

		[SaveableField(320)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _quarterDailyPartyTicker;

		[SaveableField(230)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _caravanMobilePartyPartialHourlyAiEventTicker;

		[SaveableField(250)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _garrisonMobilePartyPartialHourlyAiEventTicker;

		[SaveableField(260)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _militiaMobilePartyPartialHourlyAiEventTicker;

		[SaveableField(270)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _villagerMobilePartyPartialHourlyAiEventTicker;

		[SaveableField(280)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _customMobilePartyPartialHourlyAiEventTicker;

		[SaveableField(290)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _banditMobilePartyPartialHourlyAiEventTicker;

		[SaveableField(300)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _lordMobilePartyPartialHourlyAiEventTicker;

		[SaveableField(310)]
		private CampaignPeriodicEventManager.PeriodicTicker<MobileParty> _partiesWithoutPartyComponentsPartialHourlyAiEventTicker;

		private static readonly CampaignTime MinimumPeriodicEventInterval = CampaignTime.Hours(0.05f);

		private CampaignTime _lastGameTime = CampaignTime.Zero;

		internal class PeriodicTicker<T>
		{
			[SaveableProperty(1)]
			private double TickDebt { get; set; }

			[SaveableProperty(2)]
			private int Index { get; set; }

			internal PeriodicTicker()
			{
				this.TickDebt = 0.0;
				this.Index = -1;
			}

			internal void Initialize(MBReadOnlyList<T> list, Action<T> action, bool doParallel)
			{
				this._list = list;
				this._action = action;
				this._doParallel = doParallel;
			}

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

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
			}

			private readonly List<T> _currentFrameToTickListFlattened = new List<T>();

			private bool _doParallel;

			private MBReadOnlyList<T> _list;

			private Action<T> _action;
		}
	}
}
