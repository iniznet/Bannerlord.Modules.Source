using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.CampaignSystem
{
	public class CampaignObjectManager
	{
		[SaveableProperty(80)]
		public MBReadOnlyList<Settlement> Settlements { get; private set; }

		public MBReadOnlyList<MobileParty> MobileParties
		{
			get
			{
				return this._mobileParties;
			}
		}

		public MBReadOnlyList<MobileParty> CaravanParties
		{
			get
			{
				return this._caravanParties;
			}
		}

		public MBReadOnlyList<MobileParty> MilitiaParties
		{
			get
			{
				return this._militiaParties;
			}
		}

		public MBReadOnlyList<MobileParty> GarrisonParties
		{
			get
			{
				return this._garrisonParties;
			}
		}

		public MBReadOnlyList<MobileParty> BanditParties
		{
			get
			{
				return this._banditParties;
			}
		}

		public MBReadOnlyList<MobileParty> VillagerParties
		{
			get
			{
				return this._villagerParties;
			}
		}

		public MBReadOnlyList<MobileParty> LordParties
		{
			get
			{
				return this._lordParties;
			}
		}

		public MBReadOnlyList<MobileParty> CustomParties
		{
			get
			{
				return this._customParties;
			}
		}

		public MBReadOnlyList<MobileParty> PartiesWithoutPartyComponent
		{
			get
			{
				return this._partiesWithoutPartyComponent;
			}
		}

		public MBReadOnlyList<Hero> AliveHeroes
		{
			get
			{
				return this._aliveHeroes;
			}
		}

		public MBReadOnlyList<Hero> DeadOrDisabledHeroes
		{
			get
			{
				return this._deadOrDisabledHeroes;
			}
		}

		public MBReadOnlyList<Clan> Clans
		{
			get
			{
				return this._clans;
			}
		}

		public MBReadOnlyList<Kingdom> Kingdoms
		{
			get
			{
				return this._kingdoms;
			}
		}

		public MBReadOnlyList<IFaction> Factions
		{
			get
			{
				return this._factions;
			}
		}

		public CampaignObjectManager()
		{
			this._objects = new CampaignObjectManager.ICampaignObjectType[5];
			this._mobileParties = new MBList<MobileParty>();
			this._caravanParties = new MBList<MobileParty>();
			this._militiaParties = new MBList<MobileParty>();
			this._garrisonParties = new MBList<MobileParty>();
			this._customParties = new MBList<MobileParty>();
			this._banditParties = new MBList<MobileParty>();
			this._villagerParties = new MBList<MobileParty>();
			this._lordParties = new MBList<MobileParty>();
			this._partiesWithoutPartyComponent = new MBList<MobileParty>();
			this._deadOrDisabledHeroes = new MBList<Hero>();
			this._aliveHeroes = new MBList<Hero>();
			this._clans = new MBList<Clan>();
			this._kingdoms = new MBList<Kingdom>();
			this._factions = new MBList<IFaction>();
		}

		private void InitializeManagerObjectLists()
		{
			this._objects[4] = new CampaignObjectManager.CampaignObjectType<MobileParty>(this._mobileParties);
			this._objects[0] = new CampaignObjectManager.CampaignObjectType<Hero>(this._deadOrDisabledHeroes);
			this._objects[1] = new CampaignObjectManager.CampaignObjectType<Hero>(this._aliveHeroes);
			this._objects[2] = new CampaignObjectManager.CampaignObjectType<Clan>(this._clans);
			this._objects[3] = new CampaignObjectManager.CampaignObjectType<Kingdom>(this._kingdoms);
			this._objectTypesAndNextIds = new Dictionary<Type, uint>();
			foreach (CampaignObjectManager.ICampaignObjectType campaignObjectType in this._objects)
			{
				uint maxObjectSubId = campaignObjectType.GetMaxObjectSubId();
				uint num;
				if (this._objectTypesAndNextIds.TryGetValue(campaignObjectType.ObjectClass, out num))
				{
					if (num <= maxObjectSubId)
					{
						this._objectTypesAndNextIds[campaignObjectType.ObjectClass] = maxObjectSubId + 1U;
					}
				}
				else
				{
					this._objectTypesAndNextIds.Add(campaignObjectType.ObjectClass, maxObjectSubId + 1U);
				}
			}
		}

		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData, ObjectLoadData objectLoadData)
		{
			this._objects = new CampaignObjectManager.ICampaignObjectType[5];
			this._factions = new MBList<IFaction>();
			this._caravanParties = new MBList<MobileParty>();
			this._militiaParties = new MBList<MobileParty>();
			this._garrisonParties = new MBList<MobileParty>();
			this._customParties = new MBList<MobileParty>();
			this._banditParties = new MBList<MobileParty>();
			this._villagerParties = new MBList<MobileParty>();
			this._lordParties = new MBList<MobileParty>();
			this._partiesWithoutPartyComponent = new MBList<MobileParty>();
		}

		internal void PreAfterLoad()
		{
			CampaignObjectManager.ICampaignObjectType[] objects = this._objects;
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].PreAfterLoad();
			}
		}

		internal void AfterLoad()
		{
			CampaignObjectManager.ICampaignObjectType[] objects = this._objects;
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].AfterLoad();
			}
		}

		internal void InitializeOnLoad()
		{
			this.Settlements = MBObjectManager.Instance.GetObjectTypeList<Settlement>();
			foreach (Clan clan in this._clans)
			{
				if (!this._factions.Contains(clan))
				{
					this._factions.Add(clan);
				}
			}
			foreach (Kingdom kingdom in this._kingdoms)
			{
				if (!this._factions.Contains(kingdom))
				{
					this._factions.Add(kingdom);
				}
			}
			foreach (MobileParty mobileParty in this._mobileParties)
			{
				mobileParty.UpdatePartyComponentFlags();
				this.AddPartyToAppropriateList(mobileParty);
			}
			this.InitializeManagerObjectLists();
		}

		internal void InitializeOnNewGame()
		{
			List<Hero> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<Hero>();
			MBReadOnlyList<MobileParty> objectTypeList2 = MBObjectManager.Instance.GetObjectTypeList<MobileParty>();
			MBReadOnlyList<Clan> objectTypeList3 = MBObjectManager.Instance.GetObjectTypeList<Clan>();
			MBReadOnlyList<Kingdom> objectTypeList4 = MBObjectManager.Instance.GetObjectTypeList<Kingdom>();
			this.Settlements = MBObjectManager.Instance.GetObjectTypeList<Settlement>();
			foreach (Hero hero in objectTypeList)
			{
				if (hero.HeroState == Hero.CharacterStates.Dead || hero.HeroState == Hero.CharacterStates.Disabled)
				{
					if (!this._deadOrDisabledHeroes.Contains(hero))
					{
						this._deadOrDisabledHeroes.Add(hero);
					}
				}
				else if (!this._aliveHeroes.Contains(hero))
				{
					this._aliveHeroes.Add(hero);
				}
			}
			foreach (Clan clan in objectTypeList3)
			{
				if (!this._clans.Contains(clan))
				{
					this._clans.Add(clan);
				}
				if (!this._factions.Contains(clan))
				{
					this._factions.Add(clan);
				}
			}
			foreach (Kingdom kingdom in objectTypeList4)
			{
				if (!this._kingdoms.Contains(kingdom))
				{
					this._kingdoms.Add(kingdom);
				}
				if (!this._factions.Contains(kingdom))
				{
					this._factions.Add(kingdom);
				}
			}
			foreach (MobileParty mobileParty in objectTypeList2)
			{
				this._mobileParties.Add(mobileParty);
				this.AddPartyToAppropriateList(mobileParty);
			}
			this.InitializeManagerObjectLists();
			this.InitializeCachedData();
		}

		private void InitializeCachedData()
		{
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsVillage)
				{
					settlement.OwnerClan.OnBoundVillageAdded(settlement.Village);
				}
			}
			foreach (Clan clan in Clan.All)
			{
				if (clan.Kingdom != null)
				{
					foreach (Hero hero in clan.Heroes)
					{
						clan.Kingdom.OnHeroAdded(hero);
					}
				}
			}
		}

		internal void AddMobileParty(MobileParty party)
		{
			party.Id = new MBGUID(14U, Campaign.Current.CampaignObjectManager.GetNextUniqueObjectIdOfType<MobileParty>());
			this._mobileParties.Add(party);
			this.OnItemAdded<MobileParty>(CampaignObjectManager.CampaignObjects.MobileParty, party);
			this.AddPartyToAppropriateList(party);
		}

		internal void RemoveMobileParty(MobileParty party)
		{
			this._mobileParties.Remove(party);
			this.OnItemRemoved<MobileParty>(CampaignObjectManager.CampaignObjects.MobileParty, party);
			this.RemovePartyFromAppropriateList(party);
		}

		internal void BeforePartyComponentChanged(MobileParty party)
		{
			this.RemovePartyFromAppropriateList(party);
		}

		internal void AfterPartyComponentChanged(MobileParty party)
		{
			this.AddPartyToAppropriateList(party);
		}

		internal void AddHero(Hero hero)
		{
			hero.Id = new MBGUID(32U, Campaign.Current.CampaignObjectManager.GetNextUniqueObjectIdOfType<Hero>());
			this.OnHeroAdded(hero);
		}

		internal void UnregisterDeadHero(Hero hero)
		{
			this._deadOrDisabledHeroes.Remove(hero);
			this.OnItemRemoved<Hero>(CampaignObjectManager.CampaignObjects.DeadOrDisabledHeroes, hero);
			CampaignEventDispatcher.Instance.OnHeroUnregistered(hero);
		}

		private void OnHeroAdded(Hero hero)
		{
			if (hero.HeroState == Hero.CharacterStates.Dead || hero.HeroState == Hero.CharacterStates.Disabled)
			{
				this._deadOrDisabledHeroes.Add(hero);
				this.OnItemAdded<Hero>(CampaignObjectManager.CampaignObjects.DeadOrDisabledHeroes, hero);
				return;
			}
			this._aliveHeroes.Add(hero);
			this.OnItemAdded<Hero>(CampaignObjectManager.CampaignObjects.AliveHeroes, hero);
		}

		internal void HeroStateChanged(Hero hero, Hero.CharacterStates oldState)
		{
			bool flag = oldState == Hero.CharacterStates.Dead || oldState == Hero.CharacterStates.Disabled;
			bool flag2 = hero.HeroState == Hero.CharacterStates.Dead || hero.HeroState == Hero.CharacterStates.Disabled;
			if (flag != flag2)
			{
				if (flag2)
				{
					if (this._aliveHeroes.Contains(hero))
					{
						this._aliveHeroes.Remove(hero);
					}
				}
				else if (this._deadOrDisabledHeroes.Contains(hero))
				{
					this._deadOrDisabledHeroes.Remove(hero);
				}
				this.OnHeroAdded(hero);
			}
		}

		internal void AddClan(Clan clan)
		{
			clan.Id = new MBGUID(18U, Campaign.Current.CampaignObjectManager.GetNextUniqueObjectIdOfType<Clan>());
			this._clans.Add(clan);
			this.OnItemAdded<Clan>(CampaignObjectManager.CampaignObjects.Clans, clan);
			this._factions.Add(clan);
		}

		internal void RemoveClan(Clan clan)
		{
			if (this._clans.Contains(clan))
			{
				this._clans.Remove(clan);
				this.OnItemRemoved<Clan>(CampaignObjectManager.CampaignObjects.Clans, clan);
			}
			if (this._factions.Contains(clan))
			{
				this._factions.Remove(clan);
			}
		}

		internal void AddKingdom(Kingdom kingdom)
		{
			kingdom.Id = new MBGUID(20U, Campaign.Current.CampaignObjectManager.GetNextUniqueObjectIdOfType<Kingdom>());
			this._kingdoms.Add(kingdom);
			this.OnItemAdded<Kingdom>(CampaignObjectManager.CampaignObjects.Kingdoms, kingdom);
			this._factions.Add(kingdom);
		}

		private void AddPartyToAppropriateList(MobileParty party)
		{
			if (party.IsBandit)
			{
				this._banditParties.Add(party);
				return;
			}
			if (party.IsCaravan)
			{
				this._caravanParties.Add(party);
				return;
			}
			if (party.IsLordParty)
			{
				this._lordParties.Add(party);
				return;
			}
			if (party.IsMilitia)
			{
				this._militiaParties.Add(party);
				return;
			}
			if (party.IsVillager)
			{
				this._villagerParties.Add(party);
				return;
			}
			if (party.IsCustomParty)
			{
				this._customParties.Add(party);
				return;
			}
			if (party.IsGarrison)
			{
				this._garrisonParties.Add(party);
				return;
			}
			this._partiesWithoutPartyComponent.Add(party);
		}

		private void RemovePartyFromAppropriateList(MobileParty party)
		{
			if (party.IsBandit)
			{
				this._banditParties.Remove(party);
				return;
			}
			if (party.IsCaravan)
			{
				this._caravanParties.Remove(party);
				return;
			}
			if (party.IsLordParty)
			{
				this._lordParties.Remove(party);
				return;
			}
			if (party.IsMilitia)
			{
				this._militiaParties.Remove(party);
				return;
			}
			if (party.IsVillager)
			{
				this._villagerParties.Remove(party);
				return;
			}
			if (party.IsCustomParty)
			{
				this._customParties.Remove(party);
				return;
			}
			if (party.IsGarrison)
			{
				this._garrisonParties.Remove(party);
				return;
			}
			this._partiesWithoutPartyComponent.Remove(party);
		}

		private void OnItemAdded<T>(CampaignObjectManager.CampaignObjects targetList, T obj) where T : MBObjectBase
		{
			CampaignObjectManager.CampaignObjectType<T> campaignObjectType = (CampaignObjectManager.CampaignObjectType<T>)this._objects[(int)targetList];
			if (campaignObjectType != null)
			{
				campaignObjectType.OnItemAdded(obj);
			}
		}

		private void OnItemRemoved<T>(CampaignObjectManager.CampaignObjects targetList, T obj) where T : MBObjectBase
		{
			CampaignObjectManager.CampaignObjectType<T> campaignObjectType = (CampaignObjectManager.CampaignObjectType<T>)this._objects[(int)targetList];
			if (campaignObjectType != null)
			{
				campaignObjectType.UnregisterItem(obj);
			}
		}

		public T Find<T>(Predicate<T> predicate) where T : MBObjectBase
		{
			foreach (CampaignObjectManager.ICampaignObjectType campaignObjectType in this._objects)
			{
				if (typeof(T) == campaignObjectType.ObjectClass)
				{
					T t = ((CampaignObjectManager.CampaignObjectType<T>)campaignObjectType).Find(predicate);
					if (t != null)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		private uint GetNextUniqueObjectIdOfType<T>() where T : MBObjectBase
		{
			uint num;
			if (this._objectTypesAndNextIds.TryGetValue(typeof(T), out num))
			{
				this._objectTypesAndNextIds[typeof(T)] = num + 1U;
			}
			return num;
		}

		public T Find<T>(string id) where T : MBObjectBase
		{
			foreach (CampaignObjectManager.ICampaignObjectType campaignObjectType in this._objects)
			{
				if (campaignObjectType != null && typeof(T) == campaignObjectType.ObjectClass)
				{
					T t = ((CampaignObjectManager.CampaignObjectType<T>)campaignObjectType).Find(id);
					if (t != null)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		public string FindNextUniqueStringId<T>(string id) where T : MBObjectBase
		{
			List<CampaignObjectManager.CampaignObjectType<T>> list = new List<CampaignObjectManager.CampaignObjectType<T>>();
			foreach (CampaignObjectManager.ICampaignObjectType campaignObjectType in this._objects)
			{
				if (campaignObjectType != null && typeof(T) == campaignObjectType.ObjectClass)
				{
					list.Add(campaignObjectType as CampaignObjectManager.CampaignObjectType<T>);
				}
			}
			return CampaignObjectManager.CampaignObjectType<T>.FindNextUniqueStringId(list, id);
		}

		internal static void AutoGeneratedStaticCollectObjectsCampaignObjectManager(object o, List<object> collectedObjects)
		{
			((CampaignObjectManager)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._deadOrDisabledHeroes);
			collectedObjects.Add(this._aliveHeroes);
			collectedObjects.Add(this._clans);
			collectedObjects.Add(this._kingdoms);
			collectedObjects.Add(this._mobileParties);
			collectedObjects.Add(this.Settlements);
		}

		internal static object AutoGeneratedGetMemberValueSettlements(object o)
		{
			return ((CampaignObjectManager)o).Settlements;
		}

		internal static object AutoGeneratedGetMemberValue_deadOrDisabledHeroes(object o)
		{
			return ((CampaignObjectManager)o)._deadOrDisabledHeroes;
		}

		internal static object AutoGeneratedGetMemberValue_aliveHeroes(object o)
		{
			return ((CampaignObjectManager)o)._aliveHeroes;
		}

		internal static object AutoGeneratedGetMemberValue_clans(object o)
		{
			return ((CampaignObjectManager)o)._clans;
		}

		internal static object AutoGeneratedGetMemberValue_kingdoms(object o)
		{
			return ((CampaignObjectManager)o)._kingdoms;
		}

		internal static object AutoGeneratedGetMemberValue_mobileParties(object o)
		{
			return ((CampaignObjectManager)o)._mobileParties;
		}

		internal const uint HeroObjectManagerTypeID = 32U;

		internal const uint MobilePartyObjectManagerTypeID = 14U;

		internal const uint ClanObjectManagerTypeID = 18U;

		internal const uint KingdomObjectManagerTypeID = 20U;

		private CampaignObjectManager.ICampaignObjectType[] _objects;

		private Dictionary<Type, uint> _objectTypesAndNextIds;

		[SaveableField(20)]
		private readonly MBList<Hero> _deadOrDisabledHeroes;

		[SaveableField(30)]
		private readonly MBList<Hero> _aliveHeroes;

		[SaveableField(40)]
		private readonly MBList<Clan> _clans;

		[SaveableField(50)]
		private readonly MBList<Kingdom> _kingdoms;

		private MBList<IFaction> _factions;

		[SaveableField(71)]
		private MBList<MobileParty> _mobileParties;

		private MBList<MobileParty> _caravanParties;

		private MBList<MobileParty> _militiaParties;

		private MBList<MobileParty> _garrisonParties;

		private MBList<MobileParty> _banditParties;

		private MBList<MobileParty> _villagerParties;

		private MBList<MobileParty> _customParties;

		private MBList<MobileParty> _lordParties;

		private MBList<MobileParty> _partiesWithoutPartyComponent;

		private interface ICampaignObjectType : IEnumerable
		{
			Type ObjectClass { get; }

			void PreAfterLoad();

			void AfterLoad();

			uint GetMaxObjectSubId();
		}

		private class CampaignObjectType<T> : CampaignObjectManager.ICampaignObjectType, IEnumerable, IEnumerable<T> where T : MBObjectBase
		{
			public uint MaxCreatedPostfixIndex { get; private set; }

			public CampaignObjectType(IEnumerable<T> registeredObjects)
			{
				this._registeredObjects = registeredObjects;
				foreach (T t in this._registeredObjects)
				{
					ValueTuple<string, uint> idParts = CampaignObjectManager.CampaignObjectType<T>.GetIdParts(t.StringId);
					if (idParts.Item2 > this.MaxCreatedPostfixIndex)
					{
						this.MaxCreatedPostfixIndex = idParts.Item2;
					}
				}
			}

			Type CampaignObjectManager.ICampaignObjectType.ObjectClass
			{
				get
				{
					return typeof(T);
				}
			}

			public void PreAfterLoad()
			{
				foreach (T t in this._registeredObjects.ToList<T>())
				{
					t.PreAfterLoadInternal();
				}
			}

			public void AfterLoad()
			{
				foreach (T t in this._registeredObjects.ToList<T>())
				{
					t.IsReady = true;
					t.AfterLoadInternal();
				}
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return this._registeredObjects.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._registeredObjects.GetEnumerator();
			}

			public uint GetMaxObjectSubId()
			{
				uint num = 0U;
				foreach (T t in this._registeredObjects)
				{
					if (t.Id.SubId > num)
					{
						num = t.Id.SubId;
					}
				}
				return num;
			}

			public void OnItemAdded(T item)
			{
				ValueTuple<string, uint> idParts = CampaignObjectManager.CampaignObjectType<T>.GetIdParts(item.StringId);
				if (idParts.Item2 > this.MaxCreatedPostfixIndex)
				{
					this.MaxCreatedPostfixIndex = idParts.Item2;
				}
				this.RegisterItem(item);
			}

			private void RegisterItem(T item)
			{
				item.IsReady = true;
			}

			public void UnregisterItem(T item)
			{
				item.IsReady = false;
			}

			public T Find(string id)
			{
				foreach (T t in this._registeredObjects)
				{
					if (t.StringId == id)
					{
						return t;
					}
				}
				return default(T);
			}

			public T Find(Predicate<T> predicate)
			{
				foreach (T t in this._registeredObjects)
				{
					if (predicate(t))
					{
						return t;
					}
				}
				return default(T);
			}

			public static string FindNextUniqueStringId(List<CampaignObjectManager.CampaignObjectType<T>> lists, string id)
			{
				if (!CampaignObjectManager.CampaignObjectType<T>.Exist(lists, id))
				{
					return id;
				}
				ValueTuple<string, uint> idParts = CampaignObjectManager.CampaignObjectType<T>.GetIdParts(id);
				string item = idParts.Item1;
				uint num = idParts.Item2;
				num = MathF.Max(num, lists.Max((CampaignObjectManager.CampaignObjectType<T> x) => x.MaxCreatedPostfixIndex));
				num += 1U;
				return item + num;
			}

			[return: TupleElementNames(new string[] { "str", "number" })]
			private static ValueTuple<string, uint> GetIdParts(string stringId)
			{
				int num = stringId.Length - 1;
				while (num > 0 && char.IsDigit(stringId[num]))
				{
					num--;
				}
				string text = stringId.Substring(0, num + 1);
				uint num2 = 0U;
				if (num < stringId.Length - 1)
				{
					uint.TryParse(stringId.Substring(num + 1, stringId.Length - num - 1), out num2);
				}
				return new ValueTuple<string, uint>(text, num2);
			}

			private static bool Exist(List<CampaignObjectManager.CampaignObjectType<T>> lists, string id)
			{
				using (List<CampaignObjectManager.CampaignObjectType<T>>.Enumerator enumerator = lists.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Find(id) != null)
						{
							return true;
						}
					}
				}
				return false;
			}

			private readonly IEnumerable<T> _registeredObjects;
		}

		private enum CampaignObjects
		{
			DeadOrDisabledHeroes,
			AliveHeroes,
			Clans,
			Kingdoms,
			MobileParty,
			ObjectCount
		}
	}
}
