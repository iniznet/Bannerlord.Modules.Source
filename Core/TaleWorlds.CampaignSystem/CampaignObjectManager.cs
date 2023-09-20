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
	// Token: 0x02000052 RID: 82
	public class CampaignObjectManager
	{
		// Token: 0x17000199 RID: 409
		// (get) Token: 0x060007AF RID: 1967 RVA: 0x00020C46 File Offset: 0x0001EE46
		// (set) Token: 0x060007B0 RID: 1968 RVA: 0x00020C4E File Offset: 0x0001EE4E
		[SaveableProperty(80)]
		public MBReadOnlyList<Settlement> Settlements { get; private set; }

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x060007B1 RID: 1969 RVA: 0x00020C57 File Offset: 0x0001EE57
		public MBReadOnlyList<MobileParty> MobileParties
		{
			get
			{
				return this._mobileParties;
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x060007B2 RID: 1970 RVA: 0x00020C5F File Offset: 0x0001EE5F
		public MBReadOnlyList<MobileParty> CaravanParties
		{
			get
			{
				return this._caravanParties;
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x00020C67 File Offset: 0x0001EE67
		public MBReadOnlyList<MobileParty> MilitiaParties
		{
			get
			{
				return this._militiaParties;
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x060007B4 RID: 1972 RVA: 0x00020C6F File Offset: 0x0001EE6F
		public MBReadOnlyList<MobileParty> GarrisonParties
		{
			get
			{
				return this._garrisonParties;
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x060007B5 RID: 1973 RVA: 0x00020C77 File Offset: 0x0001EE77
		public MBReadOnlyList<MobileParty> BanditParties
		{
			get
			{
				return this._banditParties;
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x060007B6 RID: 1974 RVA: 0x00020C7F File Offset: 0x0001EE7F
		public MBReadOnlyList<MobileParty> VillagerParties
		{
			get
			{
				return this._villagerParties;
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x060007B7 RID: 1975 RVA: 0x00020C87 File Offset: 0x0001EE87
		public MBReadOnlyList<MobileParty> LordParties
		{
			get
			{
				return this._lordParties;
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060007B8 RID: 1976 RVA: 0x00020C8F File Offset: 0x0001EE8F
		public MBReadOnlyList<MobileParty> CustomParties
		{
			get
			{
				return this._customParties;
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060007B9 RID: 1977 RVA: 0x00020C97 File Offset: 0x0001EE97
		public MBReadOnlyList<MobileParty> PartiesWithoutPartyComponent
		{
			get
			{
				return this._partiesWithoutPartyComponent;
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060007BA RID: 1978 RVA: 0x00020C9F File Offset: 0x0001EE9F
		public MBReadOnlyList<Hero> AliveHeroes
		{
			get
			{
				return this._aliveHeroes;
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060007BB RID: 1979 RVA: 0x00020CA7 File Offset: 0x0001EEA7
		public MBReadOnlyList<Hero> DeadOrDisabledHeroes
		{
			get
			{
				return this._deadOrDisabledHeroes;
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060007BC RID: 1980 RVA: 0x00020CAF File Offset: 0x0001EEAF
		public MBReadOnlyList<Clan> Clans
		{
			get
			{
				return this._clans;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060007BD RID: 1981 RVA: 0x00020CB7 File Offset: 0x0001EEB7
		public MBReadOnlyList<Kingdom> Kingdoms
		{
			get
			{
				return this._kingdoms;
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060007BE RID: 1982 RVA: 0x00020CBF File Offset: 0x0001EEBF
		public MBReadOnlyList<IFaction> Factions
		{
			get
			{
				return this._factions;
			}
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x00020CC8 File Offset: 0x0001EEC8
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

		// Token: 0x060007C0 RID: 1984 RVA: 0x00020D84 File Offset: 0x0001EF84
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

		// Token: 0x060007C1 RID: 1985 RVA: 0x00020E60 File Offset: 0x0001F060
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

		// Token: 0x060007C2 RID: 1986 RVA: 0x00020EDC File Offset: 0x0001F0DC
		internal void PreAfterLoad()
		{
			CampaignObjectManager.ICampaignObjectType[] objects = this._objects;
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].PreAfterLoad();
			}
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x00020F08 File Offset: 0x0001F108
		internal void AfterLoad()
		{
			CampaignObjectManager.ICampaignObjectType[] objects = this._objects;
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].AfterLoad();
			}
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x00020F34 File Offset: 0x0001F134
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

		// Token: 0x060007C5 RID: 1989 RVA: 0x00021054 File Offset: 0x0001F254
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

		// Token: 0x060007C6 RID: 1990 RVA: 0x00021260 File Offset: 0x0001F460
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

		// Token: 0x060007C7 RID: 1991 RVA: 0x00021350 File Offset: 0x0001F550
		internal void AddMobileParty(MobileParty party)
		{
			party.Id = new MBGUID(14U, Campaign.Current.CampaignObjectManager.GetNextUniqueObjectIdOfType<MobileParty>());
			this._mobileParties.Add(party);
			this.OnItemAdded<MobileParty>(CampaignObjectManager.CampaignObjects.MobileParty, party);
			this.AddPartyToAppropriateList(party);
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x00021389 File Offset: 0x0001F589
		internal void RemoveMobileParty(MobileParty party)
		{
			this._mobileParties.Remove(party);
			this.OnItemRemoved<MobileParty>(CampaignObjectManager.CampaignObjects.MobileParty, party);
			this.RemovePartyFromAppropriateList(party);
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x000213A7 File Offset: 0x0001F5A7
		internal void BeforePartyComponentChanged(MobileParty party)
		{
			this.RemovePartyFromAppropriateList(party);
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x000213B0 File Offset: 0x0001F5B0
		internal void AfterPartyComponentChanged(MobileParty party)
		{
			this.AddPartyToAppropriateList(party);
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x000213B9 File Offset: 0x0001F5B9
		internal void AddHero(Hero hero)
		{
			hero.Id = new MBGUID(32U, Campaign.Current.CampaignObjectManager.GetNextUniqueObjectIdOfType<Hero>());
			this.OnHeroAdded(hero);
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x000213DE File Offset: 0x0001F5DE
		internal void UnregisterDeadHero(Hero hero)
		{
			this._deadOrDisabledHeroes.Remove(hero);
			this.OnItemRemoved<Hero>(CampaignObjectManager.CampaignObjects.DeadOrDisabledHeroes, hero);
			CampaignEventDispatcher.Instance.OnHeroUnregistered(hero);
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x00021400 File Offset: 0x0001F600
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

		// Token: 0x060007CE RID: 1998 RVA: 0x00021440 File Offset: 0x0001F640
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

		// Token: 0x060007CF RID: 1999 RVA: 0x000214B3 File Offset: 0x0001F6B3
		internal void AddClan(Clan clan)
		{
			clan.Id = new MBGUID(18U, Campaign.Current.CampaignObjectManager.GetNextUniqueObjectIdOfType<Clan>());
			this._clans.Add(clan);
			this.OnItemAdded<Clan>(CampaignObjectManager.CampaignObjects.Clans, clan);
			this._factions.Add(clan);
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x000214F1 File Offset: 0x0001F6F1
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

		// Token: 0x060007D1 RID: 2001 RVA: 0x00021531 File Offset: 0x0001F731
		internal void AddKingdom(Kingdom kingdom)
		{
			kingdom.Id = new MBGUID(20U, Campaign.Current.CampaignObjectManager.GetNextUniqueObjectIdOfType<Kingdom>());
			this._kingdoms.Add(kingdom);
			this.OnItemAdded<Kingdom>(CampaignObjectManager.CampaignObjects.Kingdoms, kingdom);
			this._factions.Add(kingdom);
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x00021570 File Offset: 0x0001F770
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

		// Token: 0x060007D3 RID: 2003 RVA: 0x0002161C File Offset: 0x0001F81C
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

		// Token: 0x060007D4 RID: 2004 RVA: 0x000216D0 File Offset: 0x0001F8D0
		private void OnItemAdded<T>(CampaignObjectManager.CampaignObjects targetList, T obj) where T : MBObjectBase
		{
			CampaignObjectManager.CampaignObjectType<T> campaignObjectType = (CampaignObjectManager.CampaignObjectType<T>)this._objects[(int)targetList];
			if (campaignObjectType != null)
			{
				campaignObjectType.OnItemAdded(obj);
			}
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x000216F8 File Offset: 0x0001F8F8
		private void OnItemRemoved<T>(CampaignObjectManager.CampaignObjects targetList, T obj) where T : MBObjectBase
		{
			CampaignObjectManager.CampaignObjectType<T> campaignObjectType = (CampaignObjectManager.CampaignObjectType<T>)this._objects[(int)targetList];
			if (campaignObjectType != null)
			{
				campaignObjectType.UnregisterItem(obj);
			}
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x00021720 File Offset: 0x0001F920
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

		// Token: 0x060007D7 RID: 2007 RVA: 0x00021780 File Offset: 0x0001F980
		private uint GetNextUniqueObjectIdOfType<T>() where T : MBObjectBase
		{
			uint num;
			if (this._objectTypesAndNextIds.TryGetValue(typeof(T), out num))
			{
				this._objectTypesAndNextIds[typeof(T)] = num + 1U;
			}
			return num;
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x000217C0 File Offset: 0x0001F9C0
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

		// Token: 0x060007D9 RID: 2009 RVA: 0x00021824 File Offset: 0x0001FA24
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

		// Token: 0x060007DA RID: 2010 RVA: 0x0002187D File Offset: 0x0001FA7D
		internal static void AutoGeneratedStaticCollectObjectsCampaignObjectManager(object o, List<object> collectedObjects)
		{
			((CampaignObjectManager)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x0002188C File Offset: 0x0001FA8C
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._deadOrDisabledHeroes);
			collectedObjects.Add(this._aliveHeroes);
			collectedObjects.Add(this._clans);
			collectedObjects.Add(this._kingdoms);
			collectedObjects.Add(this._mobileParties);
			collectedObjects.Add(this.Settlements);
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x000218E1 File Offset: 0x0001FAE1
		internal static object AutoGeneratedGetMemberValueSettlements(object o)
		{
			return ((CampaignObjectManager)o).Settlements;
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x000218EE File Offset: 0x0001FAEE
		internal static object AutoGeneratedGetMemberValue_deadOrDisabledHeroes(object o)
		{
			return ((CampaignObjectManager)o)._deadOrDisabledHeroes;
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x000218FB File Offset: 0x0001FAFB
		internal static object AutoGeneratedGetMemberValue_aliveHeroes(object o)
		{
			return ((CampaignObjectManager)o)._aliveHeroes;
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x00021908 File Offset: 0x0001FB08
		internal static object AutoGeneratedGetMemberValue_clans(object o)
		{
			return ((CampaignObjectManager)o)._clans;
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x00021915 File Offset: 0x0001FB15
		internal static object AutoGeneratedGetMemberValue_kingdoms(object o)
		{
			return ((CampaignObjectManager)o)._kingdoms;
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x00021922 File Offset: 0x0001FB22
		internal static object AutoGeneratedGetMemberValue_mobileParties(object o)
		{
			return ((CampaignObjectManager)o)._mobileParties;
		}

		// Token: 0x04000294 RID: 660
		internal const uint HeroObjectManagerTypeID = 32U;

		// Token: 0x04000295 RID: 661
		internal const uint MobilePartyObjectManagerTypeID = 14U;

		// Token: 0x04000296 RID: 662
		internal const uint ClanObjectManagerTypeID = 18U;

		// Token: 0x04000297 RID: 663
		internal const uint KingdomObjectManagerTypeID = 20U;

		// Token: 0x04000298 RID: 664
		private CampaignObjectManager.ICampaignObjectType[] _objects;

		// Token: 0x04000299 RID: 665
		private Dictionary<Type, uint> _objectTypesAndNextIds;

		// Token: 0x0400029A RID: 666
		[SaveableField(20)]
		private readonly MBList<Hero> _deadOrDisabledHeroes;

		// Token: 0x0400029B RID: 667
		[SaveableField(30)]
		private readonly MBList<Hero> _aliveHeroes;

		// Token: 0x0400029C RID: 668
		[SaveableField(40)]
		private readonly MBList<Clan> _clans;

		// Token: 0x0400029D RID: 669
		[SaveableField(50)]
		private readonly MBList<Kingdom> _kingdoms;

		// Token: 0x0400029E RID: 670
		private MBList<IFaction> _factions;

		// Token: 0x0400029F RID: 671
		[SaveableField(71)]
		private MBList<MobileParty> _mobileParties;

		// Token: 0x040002A0 RID: 672
		private MBList<MobileParty> _caravanParties;

		// Token: 0x040002A1 RID: 673
		private MBList<MobileParty> _militiaParties;

		// Token: 0x040002A2 RID: 674
		private MBList<MobileParty> _garrisonParties;

		// Token: 0x040002A3 RID: 675
		private MBList<MobileParty> _banditParties;

		// Token: 0x040002A4 RID: 676
		private MBList<MobileParty> _villagerParties;

		// Token: 0x040002A5 RID: 677
		private MBList<MobileParty> _customParties;

		// Token: 0x040002A6 RID: 678
		private MBList<MobileParty> _lordParties;

		// Token: 0x040002A7 RID: 679
		private MBList<MobileParty> _partiesWithoutPartyComponent;

		// Token: 0x02000498 RID: 1176
		private interface ICampaignObjectType : IEnumerable
		{
			// Token: 0x17000D6B RID: 3435
			// (get) Token: 0x06004074 RID: 16500
			Type ObjectClass { get; }

			// Token: 0x06004075 RID: 16501
			void PreAfterLoad();

			// Token: 0x06004076 RID: 16502
			void AfterLoad();

			// Token: 0x06004077 RID: 16503
			uint GetMaxObjectSubId();
		}

		// Token: 0x02000499 RID: 1177
		private class CampaignObjectType<T> : CampaignObjectManager.ICampaignObjectType, IEnumerable, IEnumerable<T> where T : MBObjectBase
		{
			// Token: 0x17000D6C RID: 3436
			// (get) Token: 0x06004078 RID: 16504 RVA: 0x00131411 File Offset: 0x0012F611
			// (set) Token: 0x06004079 RID: 16505 RVA: 0x00131419 File Offset: 0x0012F619
			public uint MaxCreatedPostfixIndex { get; private set; }

			// Token: 0x0600407A RID: 16506 RVA: 0x00131424 File Offset: 0x0012F624
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

			// Token: 0x17000D6D RID: 3437
			// (get) Token: 0x0600407B RID: 16507 RVA: 0x001314A0 File Offset: 0x0012F6A0
			Type CampaignObjectManager.ICampaignObjectType.ObjectClass
			{
				get
				{
					return typeof(T);
				}
			}

			// Token: 0x0600407C RID: 16508 RVA: 0x001314AC File Offset: 0x0012F6AC
			public void PreAfterLoad()
			{
				foreach (T t in this._registeredObjects.ToList<T>())
				{
					t.PreAfterLoadInternal();
				}
			}

			// Token: 0x0600407D RID: 16509 RVA: 0x00131508 File Offset: 0x0012F708
			public void AfterLoad()
			{
				foreach (T t in this._registeredObjects.ToList<T>())
				{
					t.IsReady = true;
					t.AfterLoadInternal();
				}
			}

			// Token: 0x0600407E RID: 16510 RVA: 0x00131570 File Offset: 0x0012F770
			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return this._registeredObjects.GetEnumerator();
			}

			// Token: 0x0600407F RID: 16511 RVA: 0x0013157D File Offset: 0x0012F77D
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._registeredObjects.GetEnumerator();
			}

			// Token: 0x06004080 RID: 16512 RVA: 0x0013158C File Offset: 0x0012F78C
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

			// Token: 0x06004081 RID: 16513 RVA: 0x00131600 File Offset: 0x0012F800
			public void OnItemAdded(T item)
			{
				ValueTuple<string, uint> idParts = CampaignObjectManager.CampaignObjectType<T>.GetIdParts(item.StringId);
				if (idParts.Item2 > this.MaxCreatedPostfixIndex)
				{
					this.MaxCreatedPostfixIndex = idParts.Item2;
				}
				this.RegisterItem(item);
			}

			// Token: 0x06004082 RID: 16514 RVA: 0x0013163F File Offset: 0x0012F83F
			private void RegisterItem(T item)
			{
				item.IsReady = true;
			}

			// Token: 0x06004083 RID: 16515 RVA: 0x0013164D File Offset: 0x0012F84D
			public void UnregisterItem(T item)
			{
				item.IsReady = false;
			}

			// Token: 0x06004084 RID: 16516 RVA: 0x0013165C File Offset: 0x0012F85C
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

			// Token: 0x06004085 RID: 16517 RVA: 0x001316C4 File Offset: 0x0012F8C4
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

			// Token: 0x06004086 RID: 16518 RVA: 0x00131724 File Offset: 0x0012F924
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

			// Token: 0x06004087 RID: 16519 RVA: 0x0013178C File Offset: 0x0012F98C
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

			// Token: 0x06004088 RID: 16520 RVA: 0x001317F4 File Offset: 0x0012F9F4
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

			// Token: 0x040013E6 RID: 5094
			private readonly IEnumerable<T> _registeredObjects;
		}

		// Token: 0x0200049A RID: 1178
		private enum CampaignObjects
		{
			// Token: 0x040013E9 RID: 5097
			DeadOrDisabledHeroes,
			// Token: 0x040013EA RID: 5098
			AliveHeroes,
			// Token: 0x040013EB RID: 5099
			Clans,
			// Token: 0x040013EC RID: 5100
			Kingdoms,
			// Token: 0x040013ED RID: 5101
			MobileParty,
			// Token: 0x040013EE RID: 5102
			ObjectCount
		}
	}
}
