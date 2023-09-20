﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.CampaignSystem
{
	public sealed class Kingdom : MBObjectBase, IFaction
	{
		internal static void AutoGeneratedStaticCollectObjectsKingdom(object o, List<object> collectedObjects)
		{
			((Kingdom)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._unresolvedDecisions);
			collectedObjects.Add(this._rulingClan);
			collectedObjects.Add(this._armies);
			collectedObjects.Add(this._activePolicies);
			collectedObjects.Add(this.Name);
			collectedObjects.Add(this.InformalName);
			collectedObjects.Add(this.EncyclopediaText);
			collectedObjects.Add(this.EncyclopediaTitle);
			collectedObjects.Add(this.EncyclopediaRulerTitle);
			collectedObjects.Add(this.Culture);
			collectedObjects.Add(this.InitialHomeLand);
			collectedObjects.Add(this.Banner);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.LastKingdomDecisionConclusionDate, collectedObjects);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.LastMercenaryOfferTime, collectedObjects);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.NotAttackableByPlayerUntilTime, collectedObjects);
		}

		internal static object AutoGeneratedGetMemberValueName(object o)
		{
			return ((Kingdom)o).Name;
		}

		internal static object AutoGeneratedGetMemberValueInformalName(object o)
		{
			return ((Kingdom)o).InformalName;
		}

		internal static object AutoGeneratedGetMemberValueEncyclopediaText(object o)
		{
			return ((Kingdom)o).EncyclopediaText;
		}

		internal static object AutoGeneratedGetMemberValueEncyclopediaTitle(object o)
		{
			return ((Kingdom)o).EncyclopediaTitle;
		}

		internal static object AutoGeneratedGetMemberValueEncyclopediaRulerTitle(object o)
		{
			return ((Kingdom)o).EncyclopediaRulerTitle;
		}

		internal static object AutoGeneratedGetMemberValueCulture(object o)
		{
			return ((Kingdom)o).Culture;
		}

		internal static object AutoGeneratedGetMemberValueInitialHomeLand(object o)
		{
			return ((Kingdom)o).InitialHomeLand;
		}

		internal static object AutoGeneratedGetMemberValueLabelColor(object o)
		{
			return ((Kingdom)o).LabelColor;
		}

		internal static object AutoGeneratedGetMemberValueColor(object o)
		{
			return ((Kingdom)o).Color;
		}

		internal static object AutoGeneratedGetMemberValueColor2(object o)
		{
			return ((Kingdom)o).Color2;
		}

		internal static object AutoGeneratedGetMemberValueAlternativeColor(object o)
		{
			return ((Kingdom)o).AlternativeColor;
		}

		internal static object AutoGeneratedGetMemberValueAlternativeColor2(object o)
		{
			return ((Kingdom)o).AlternativeColor2;
		}

		internal static object AutoGeneratedGetMemberValuePrimaryBannerColor(object o)
		{
			return ((Kingdom)o).PrimaryBannerColor;
		}

		internal static object AutoGeneratedGetMemberValueSecondaryBannerColor(object o)
		{
			return ((Kingdom)o).SecondaryBannerColor;
		}

		internal static object AutoGeneratedGetMemberValueMainHeroCrimeRating(object o)
		{
			return ((Kingdom)o).MainHeroCrimeRating;
		}

		internal static object AutoGeneratedGetMemberValueBanner(object o)
		{
			return ((Kingdom)o).Banner;
		}

		internal static object AutoGeneratedGetMemberValueLastArmyCreationDay(object o)
		{
			return ((Kingdom)o).LastArmyCreationDay;
		}

		internal static object AutoGeneratedGetMemberValueLastKingdomDecisionConclusionDate(object o)
		{
			return ((Kingdom)o).LastKingdomDecisionConclusionDate;
		}

		internal static object AutoGeneratedGetMemberValueLastMercenaryOfferTime(object o)
		{
			return ((Kingdom)o).LastMercenaryOfferTime;
		}

		internal static object AutoGeneratedGetMemberValueNotAttackableByPlayerUntilTime(object o)
		{
			return ((Kingdom)o).NotAttackableByPlayerUntilTime;
		}

		internal static object AutoGeneratedGetMemberValueMercenaryWallet(object o)
		{
			return ((Kingdom)o).MercenaryWallet;
		}

		internal static object AutoGeneratedGetMemberValuePoliticalStagnation(object o)
		{
			return ((Kingdom)o).PoliticalStagnation;
		}

		internal static object AutoGeneratedGetMemberValue_unresolvedDecisions(object o)
		{
			return ((Kingdom)o)._unresolvedDecisions;
		}

		internal static object AutoGeneratedGetMemberValue_rulingClan(object o)
		{
			return ((Kingdom)o)._rulingClan;
		}

		internal static object AutoGeneratedGetMemberValue_armies(object o)
		{
			return ((Kingdom)o)._armies;
		}

		internal static object AutoGeneratedGetMemberValue_activePolicies(object o)
		{
			return ((Kingdom)o)._activePolicies;
		}

		internal static object AutoGeneratedGetMemberValue_isEliminated(object o)
		{
			return ((Kingdom)o)._isEliminated;
		}

		internal static object AutoGeneratedGetMemberValue_aggressiveness(object o)
		{
			return ((Kingdom)o)._aggressiveness;
		}

		internal static object AutoGeneratedGetMemberValue_tributeWallet(object o)
		{
			return ((Kingdom)o)._tributeWallet;
		}

		internal static object AutoGeneratedGetMemberValue_kingdomBudgetWallet(object o)
		{
			return ((Kingdom)o)._kingdomBudgetWallet;
		}

		[SaveableProperty(1)]
		public TextObject Name { get; private set; }

		[SaveableProperty(2)]
		public TextObject InformalName { get; private set; }

		[SaveableProperty(3)]
		public TextObject EncyclopediaText { get; private set; }

		[SaveableProperty(4)]
		public TextObject EncyclopediaTitle { get; private set; }

		[SaveableProperty(5)]
		public TextObject EncyclopediaRulerTitle { get; private set; }

		public string EncyclopediaLink
		{
			get
			{
				return Campaign.Current.EncyclopediaManager.GetIdentifier(typeof(Kingdom)) + "-" + base.StringId;
			}
		}

		public TextObject EncyclopediaLinkWithName
		{
			get
			{
				return HyperlinkTexts.GetKingdomHyperlinkText(this.EncyclopediaLink, this.InformalName);
			}
		}

		public MBReadOnlyList<KingdomDecision> UnresolvedDecisions
		{
			get
			{
				return this._unresolvedDecisions;
			}
		}

		[SaveableProperty(6)]
		public CultureObject Culture { get; private set; }

		[SaveableProperty(17)]
		public Settlement InitialHomeLand { get; private set; }

		public Vec2 InitialPosition
		{
			get
			{
				return this.InitialHomeLand.GatePosition;
			}
		}

		public bool IsMapFaction
		{
			get
			{
				return true;
			}
		}

		[SaveableProperty(8)]
		public uint LabelColor { get; private set; }

		[SaveableProperty(9)]
		public uint Color { get; private set; }

		[SaveableProperty(10)]
		public uint Color2 { get; private set; }

		[SaveableProperty(11)]
		public uint AlternativeColor { get; private set; }

		[SaveableProperty(12)]
		public uint AlternativeColor2 { get; private set; }

		[SaveableProperty(13)]
		public uint PrimaryBannerColor { get; private set; }

		[SaveableProperty(14)]
		public uint SecondaryBannerColor { get; private set; }

		[SaveableProperty(15)]
		public float MainHeroCrimeRating { get; set; }

		public IEnumerable<StanceLink> Stances
		{
			get
			{
				return this._stances;
			}
		}

		public MBReadOnlyList<Town> Fiefs
		{
			get
			{
				return this._fiefsCache;
			}
		}

		public MBReadOnlyList<Village> Villages
		{
			get
			{
				return this._villagesCache;
			}
		}

		public MBReadOnlyList<Settlement> Settlements
		{
			get
			{
				return this._settlementsCache;
			}
		}

		public MBReadOnlyList<Hero> Heroes
		{
			get
			{
				return this._heroesCache;
			}
		}

		public MBReadOnlyList<Hero> Lords
		{
			get
			{
				return this._lordsCache;
			}
		}

		public MBReadOnlyList<WarPartyComponent> WarPartyComponents
		{
			get
			{
				return this._warPartyComponentsCache;
			}
		}

		public float DailyCrimeRatingChange
		{
			get
			{
				return Campaign.Current.Models.CrimeModel.GetDailyCrimeRatingChange(this, false).ResultNumber;
			}
		}

		public ExplainedNumber DailyCrimeRatingChangeExplained
		{
			get
			{
				return Campaign.Current.Models.CrimeModel.GetDailyCrimeRatingChange(this, true);
			}
		}

		public CharacterObject BasicTroop
		{
			get
			{
				return this.Culture.BasicTroop;
			}
		}

		public Hero Leader
		{
			get
			{
				Clan rulingClan = this._rulingClan;
				if (rulingClan == null)
				{
					return null;
				}
				return rulingClan.Leader;
			}
		}

		[SaveableProperty(16)]
		public Banner Banner { get; private set; }

		public bool IsBanditFaction
		{
			get
			{
				return false;
			}
		}

		public bool IsMinorFaction
		{
			get
			{
				return false;
			}
		}

		bool IFaction.IsKingdomFaction
		{
			get
			{
				return true;
			}
		}

		public bool IsRebelClan
		{
			get
			{
				return false;
			}
		}

		public bool IsClan
		{
			get
			{
				return false;
			}
		}

		public bool IsOutlaw
		{
			get
			{
				return false;
			}
		}

		public MBReadOnlyList<Clan> Clans
		{
			get
			{
				return this._clans;
			}
		}

		public Clan RulingClan
		{
			get
			{
				return this._rulingClan;
			}
			set
			{
				this._rulingClan = value;
			}
		}

		[SaveableProperty(19)]
		public int LastArmyCreationDay { get; private set; }

		public MBReadOnlyList<Army> Armies
		{
			get
			{
				return this._armies;
			}
		}

		public override string ToString()
		{
			return this.Name.ToString();
		}

		public float TotalStrength
		{
			get
			{
				float num = 0f;
				int count = this._clans.Count;
				for (int i = 0; i < count; i++)
				{
					num += this._clans[i].TotalStrength;
				}
				return num;
			}
		}

		[CachedData]
		internal bool _midPointCalculated { get; set; }

		public float DistanceToClosestNonAllyFortification
		{
			get
			{
				if (this._distanceToClosestNonAllyFortificationCacheDirty)
				{
					this._distanceToClosestNonAllyFortificationCache = FactionHelper.GetDistanceToClosestNonAllyFortificationOfFaction(this);
					this._distanceToClosestNonAllyFortificationCacheDirty = false;
				}
				return this._distanceToClosestNonAllyFortificationCache;
			}
		}

		public IList<PolicyObject> ActivePolicies
		{
			get
			{
				return this._activePolicies;
			}
		}

		public static MBReadOnlyList<Kingdom> All
		{
			get
			{
				return Campaign.Current.Kingdoms;
			}
		}

		[SaveableProperty(28)]
		public CampaignTime LastKingdomDecisionConclusionDate { get; private set; }

		public bool IsEliminated
		{
			get
			{
				return this._isEliminated;
			}
		}

		[SaveableProperty(41)]
		public CampaignTime LastMercenaryOfferTime { get; set; }

		public IFaction MapFaction
		{
			get
			{
				return this;
			}
		}

		[SaveableProperty(50)]
		public CampaignTime NotAttackableByPlayerUntilTime { get; set; }

		public float Aggressiveness
		{
			get
			{
				return this._aggressiveness;
			}
			internal set
			{
				this._aggressiveness = MathF.Clamp(value, 0f, 100f);
			}
		}

		public IEnumerable<MobileParty> AllParties
		{
			get
			{
				foreach (MobileParty mobileParty in Campaign.Current.MobileParties)
				{
					if (mobileParty.MapFaction == this)
					{
						yield return mobileParty;
					}
				}
				List<MobileParty>.Enumerator enumerator = default(List<MobileParty>.Enumerator);
				yield break;
				yield break;
			}
		}

		public Settlement FactionMidSettlement
		{
			get
			{
				if (!this._midPointCalculated)
				{
					this.UpdateFactionMidPoint();
				}
				return this._kingdomMidSettlement;
			}
		}

		[SaveableProperty(70)]
		public int MercenaryWallet { get; internal set; }

		public int TributeWallet
		{
			get
			{
				return this._tributeWallet;
			}
			set
			{
				this._tributeWallet = value;
			}
		}

		public int KingdomBudgetWallet
		{
			get
			{
				return this._kingdomBudgetWallet;
			}
			set
			{
				this._kingdomBudgetWallet = value;
			}
		}

		public Kingdom()
		{
			this._activePolicies = new List<PolicyObject>();
			this._armies = new MBList<Army>();
			this.InitializeCachedLists();
			this.EncyclopediaText = TextObject.Empty;
			this.EncyclopediaTitle = TextObject.Empty;
			this.EncyclopediaRulerTitle = TextObject.Empty;
			float randomFloat = MBRandom.RandomFloat;
			float randomFloat2 = MBRandom.RandomFloat;
			this.PoliticalStagnation = 10 + (int)(randomFloat * randomFloat2 * 100f);
			this._midPointCalculated = false;
			this._distanceToClosestNonAllyFortificationCacheDirty = true;
			this._isEliminated = false;
			this.NotAttackableByPlayerUntilTime = CampaignTime.Zero;
			this.LastArmyCreationDay = (int)CampaignTime.Now.ToDays;
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
		}

		public static Kingdom CreateKingdom(string stringID)
		{
			stringID = Campaign.Current.CampaignObjectManager.FindNextUniqueStringId<Kingdom>(stringID);
			Kingdom kingdom = new Kingdom();
			kingdom.StringId = stringID;
			Campaign.Current.CampaignObjectManager.AddKingdom(kingdom);
			return kingdom;
		}

		public void InitializeKingdom(TextObject name, TextObject informalName, CultureObject culture, Banner banner, uint kingdomColor1, uint kingdomColor2, Settlement initialHomeland, TextObject encyclopediaText, TextObject encyclopediaTitle, TextObject encyclopediaRulerTitle)
		{
			this.ChangeKingdomName(name, informalName);
			this.Culture = culture;
			this.Banner = banner;
			this.Color = kingdomColor1;
			this.Color2 = kingdomColor2;
			this.PrimaryBannerColor = this.Color;
			this.SecondaryBannerColor = this.Color2;
			this.InitialHomeLand = initialHomeland;
			this.PoliticalStagnation = 100;
			this.EncyclopediaText = encyclopediaText;
			this.EncyclopediaTitle = encyclopediaTitle;
			this.EncyclopediaRulerTitle = encyclopediaRulerTitle;
			foreach (PolicyObject policyObject in this.Culture.DefaultPolicyList)
			{
				this.AddPolicy(policyObject);
			}
		}

		private void InitializeCachedLists()
		{
			this._clans = new MBList<Clan>();
			this._stances = new List<StanceLink>();
			this._fiefsCache = new MBList<Town>();
			this._villagesCache = new MBList<Village>();
			this._settlementsCache = new MBList<Settlement>();
			this._heroesCache = new MBList<Hero>();
			this._lordsCache = new MBList<Hero>();
			this._warPartyComponentsCache = new MBList<WarPartyComponent>();
		}

		public void ChangeKingdomName(TextObject name, TextObject informalName)
		{
			this.Name = name;
			this.InformalName = informalName;
		}

		public void OnNewGameCreated(CampaignGameStarter starter)
		{
			this.InitialHomeLand = this.Leader.HomeSettlement;
		}

		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData, ObjectLoadData objectLoadData)
		{
			this.InitializeCachedLists();
		}

		protected override void AfterLoad()
		{
			for (int i = this._activePolicies.Count - 1; i >= 0; i--)
			{
				if (this._activePolicies[i] == null || !this._activePolicies[i].IsReady)
				{
					this._activePolicies.RemoveAt(i);
				}
			}
			for (int j = this.Clans.Count - 1; j >= 0; j--)
			{
				Clan clan = this.Clans[j];
				if (clan.GetStanceWith(this).IsAtConstantWar)
				{
					foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents.ToList<WarPartyComponent>())
					{
						if (warPartyComponent.MobileParty.MapEvent != null && (warPartyComponent.MobileParty.Army == null || warPartyComponent.MobileParty.Army.LeaderParty == warPartyComponent.MobileParty))
						{
							warPartyComponent.MobileParty.MapEvent.FinalizeEvent();
						}
					}
					ChangeKingdomAction.ApplyByLeaveWithRebellionAgainstKingdom(clan, false);
				}
			}
		}

		public bool IsAtWarWith(IFaction other)
		{
			return FactionManager.IsAtWarAgainstFaction(this, other);
		}

		public void ConsiderSiegesAndMapEvents(IFaction factionToConsiderAgainst)
		{
			foreach (Clan clan in this.Clans)
			{
				clan.ConsiderSiegesAndMapEvents(factionToConsiderAgainst);
			}
		}

		internal void AddStanceInternal(StanceLink stanceLink)
		{
			this._stances.Add(stanceLink);
		}

		internal void RemoveStanceInternal(StanceLink stanceLink)
		{
			this._stances.Remove(stanceLink);
		}

		public StanceLink GetStanceWith(IFaction other)
		{
			return FactionManager.Instance.GetStanceLinkInternal(this, other);
		}

		internal void AddArmyInternal(Army army)
		{
			this._armies.Add(army);
		}

		internal void RemoveArmyInternal(Army army)
		{
			this._armies.Remove(army);
		}

		public void CreateArmy(Hero armyLeader, Settlement targetSettlement, Army.ArmyTypes selectedArmyType)
		{
			if (!armyLeader.IsActive)
			{
				Debug.Print("Failed to create army, leader - " + ((armyLeader != null) ? armyLeader.Name : null) + " is inactive", 0, Debug.DebugColor.White, 17592186044416UL);
				return;
			}
			if (((armyLeader != null) ? armyLeader.PartyBelongedTo.LeaderHero : null) != null)
			{
				Army army = new Army(this, armyLeader.PartyBelongedTo, selectedArmyType)
				{
					AIBehavior = Army.AIBehaviorFlags.Gathering
				};
				army.Gather(targetSettlement);
				this.LastArmyCreationDay = (int)CampaignTime.Now.ToDays;
				CampaignEventDispatcher.Instance.OnArmyCreated(army);
			}
			if (armyLeader == Hero.MainHero)
			{
				MapState mapState = Game.Current.GameStateManager.GameStates.Single((GameState S) => S is MapState) as MapState;
				if (mapState == null)
				{
					return;
				}
				mapState.OnArmyCreated(MobileParty.MainParty);
			}
		}

		private void UpdateFactionMidPoint()
		{
			this._kingdomMidSettlement = FactionHelper.FactionMidSettlement(this);
			this._midPointCalculated = this._kingdomMidSettlement != null;
		}

		public void AddDecision(KingdomDecision kingdomDecision, bool ignoreInfluenceCost = false)
		{
			if (!ignoreInfluenceCost)
			{
				Clan proposerClan = kingdomDecision.ProposerClan;
				int influenceCost = kingdomDecision.GetInfluenceCost(proposerClan);
				ChangeClanInfluenceAction.Apply(proposerClan, (float)(-(float)influenceCost));
			}
			bool flag;
			if (!kingdomDecision.DetermineChooser().Leader.IsHumanPlayerCharacter)
			{
				flag = kingdomDecision.DetermineSupporters().Any((Supporter x) => x.IsPlayer);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			CampaignEventDispatcher.Instance.OnKingdomDecisionAdded(kingdomDecision, flag2);
			if (kingdomDecision.Kingdom != Clan.PlayerClan.Kingdom)
			{
				new KingdomElection(kingdomDecision).StartElection();
				return;
			}
			this._unresolvedDecisions.Add(kingdomDecision);
		}

		public void RemoveDecision(KingdomDecision kingdomDecision)
		{
			this._unresolvedDecisions.Remove(kingdomDecision);
		}

		public void OnKingdomDecisionConcluded()
		{
			this.LastKingdomDecisionConclusionDate = CampaignTime.Now;
		}

		public void AddPolicy(PolicyObject policy)
		{
			if (!this._activePolicies.Contains(policy))
			{
				this._activePolicies.Add(policy);
			}
		}

		public void RemovePolicy(PolicyObject policy)
		{
			if (this._activePolicies.Contains(policy))
			{
				this._activePolicies.Remove(policy);
			}
		}

		public bool HasPolicy(PolicyObject policy)
		{
			for (int i = 0; i < this._activePolicies.Count; i++)
			{
				if (this._activePolicies[i] == policy)
				{
					return true;
				}
			}
			return false;
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.EncyclopediaText = ((node.Attributes["text"] != null) ? new TextObject(node.Attributes["text"].Value, null) : TextObject.Empty);
			this.EncyclopediaTitle = ((node.Attributes["title"] != null) ? new TextObject(node.Attributes["title"].Value, null) : TextObject.Empty);
			this.EncyclopediaRulerTitle = ((node.Attributes["ruler_title"] != null) ? new TextObject(node.Attributes["ruler_title"].Value, null) : TextObject.Empty);
			this.InitializeKingdom(new TextObject(node.Attributes["name"].Value, null), (node.Attributes["short_name"] != null) ? new TextObject(node.Attributes["short_name"].Value, null) : new TextObject(node.Attributes["name"].Value, null), (CultureObject)objectManager.ReadObjectReferenceFromXml("culture", typeof(CultureObject), node), null, (node.Attributes["color"] == null) ? 0U : Convert.ToUInt32(node.Attributes["color"].Value, 16), (node.Attributes["color2"] == null) ? 0U : Convert.ToUInt32(node.Attributes["color2"].Value, 16), null, this.EncyclopediaText, this.EncyclopediaTitle, this.EncyclopediaRulerTitle);
			Hero hero = objectManager.ReadObjectReferenceFromXml("owner", typeof(Hero), node) as Hero;
			this.RulingClan = ((hero != null) ? hero.Clan : null);
			this.LabelColor = ((node.Attributes["label_color"] == null) ? 0U : Convert.ToUInt32(node.Attributes["label_color"].Value, 16));
			this.AlternativeColor = ((node.Attributes["alternative_color"] == null) ? 0U : Convert.ToUInt32(node.Attributes["alternative_color"].Value, 16));
			this.AlternativeColor2 = ((node.Attributes["alternative_color2"] == null) ? 0U : Convert.ToUInt32(node.Attributes["alternative_color2"].Value, 16));
			this.PrimaryBannerColor = ((node.Attributes["primary_banner_color"] == null) ? 0U : Convert.ToUInt32(node.Attributes["primary_banner_color"].Value, 16));
			this.SecondaryBannerColor = ((node.Attributes["secondary_banner_color"] == null) ? 0U : Convert.ToUInt32(node.Attributes["secondary_banner_color"].Value, 16));
			if (node.Attributes["banner_key"] != null)
			{
				this.Banner = new Banner();
				this.Banner.Deserialize(node.Attributes["banner_key"].Value);
			}
			else
			{
				this.Banner = Banner.CreateRandomClanBanner(base.StringId.GetDeterministicHashCode());
			}
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "relationships")
				{
					using (IEnumerator enumerator2 = xmlNode.ChildNodes.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							XmlNode xmlNode2 = (XmlNode)obj2;
							IFaction faction;
							if (xmlNode2.Attributes["clan"] != null)
							{
								faction = (Clan)objectManager.ReadObjectReferenceFromXml("clan", typeof(Clan), xmlNode2);
							}
							else
							{
								faction = (Kingdom)objectManager.ReadObjectReferenceFromXml("kingdom", typeof(Kingdom), xmlNode2);
							}
							int num = Convert.ToInt32(xmlNode2.Attributes["value"].InnerText);
							if (num > 0)
							{
								FactionManager.DeclareAlliance(this, faction);
							}
							else if (num < 0)
							{
								FactionManager.DeclareWar(this, faction, false);
							}
							else
							{
								FactionManager.SetNeutral(this, faction);
							}
							if (xmlNode2.Attributes["isAtWar"] != null && Convert.ToBoolean(xmlNode2.Attributes["isAtWar"].Value))
							{
								FactionManager.DeclareWar(this, faction, false);
							}
						}
						continue;
					}
				}
				if (xmlNode.Name == "policies")
				{
					foreach (object obj3 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode3 = (XmlNode)obj3;
						PolicyObject @object = Game.Current.ObjectManager.GetObject<PolicyObject>(xmlNode3.Attributes["id"].Value);
						if (@object != null)
						{
							this.AddPolicy(@object);
						}
					}
				}
			}
		}

		internal void AddClanInternal(Clan clan)
		{
			this._clans.Add(clan);
			this._midPointCalculated = false;
			this._distanceToClosestNonAllyFortificationCacheDirty = true;
		}

		internal void RemoveClanInternal(Clan clan)
		{
			this._clans.Remove(clan);
			this._midPointCalculated = false;
			this._distanceToClosestNonAllyFortificationCacheDirty = true;
		}

		public void OnFortificationAdded(Town fief)
		{
			this._fiefsCache.Add(fief);
			this._settlementsCache.Add(fief.Settlement);
			foreach (Village village in fief.Settlement.BoundVillages)
			{
				this.OnBoundVillageAdded(village);
			}
		}

		public void OnFiefRemoved(Town fief)
		{
			this._fiefsCache.Remove(fief);
			this._settlementsCache.Remove(fief.Settlement);
			foreach (Village village in fief.Settlement.BoundVillages)
			{
				this._villagesCache.Remove(village);
				this._settlementsCache.Remove(village.Settlement);
			}
		}

		internal void OnBoundVillageAdded(Village village)
		{
			this._villagesCache.Add(village);
			this._settlementsCache.Add(village.Settlement);
		}

		public void OnHeroAdded(Hero hero)
		{
			this._heroesCache.Add(hero);
			if (hero.Occupation == Occupation.Lord)
			{
				this._lordsCache.Add(hero);
			}
		}

		public void OnHeroRemoved(Hero hero)
		{
			this._heroesCache.Remove(hero);
			if (hero.Occupation == Occupation.Lord)
			{
				this._lordsCache.Remove(hero);
			}
		}

		public void OnWarPartyAdded(WarPartyComponent warPartyComponent)
		{
			this._warPartyComponentsCache.Add(warPartyComponent);
		}

		public void OnWarPartyRemoved(WarPartyComponent warPartyComponent)
		{
			this._warPartyComponentsCache.Remove(warPartyComponent);
		}

		public void ReactivateKingdom()
		{
			this._isEliminated = false;
		}

		internal void DeactivateKingdom()
		{
			this._isEliminated = true;
		}

		string IFaction.get_StringId()
		{
			return base.StringId;
		}

		MBGUID IFaction.get_Id()
		{
			return base.Id;
		}

		[SaveableField(10)]
		private MBList<KingdomDecision> _unresolvedDecisions = new MBList<KingdomDecision>();

		[CachedData]
		private List<StanceLink> _stances;

		[CachedData]
		private MBList<Town> _fiefsCache;

		[CachedData]
		private MBList<Village> _villagesCache;

		[CachedData]
		private MBList<Settlement> _settlementsCache;

		[CachedData]
		private MBList<Hero> _heroesCache;

		[CachedData]
		private MBList<Hero> _lordsCache;

		[CachedData]
		private MBList<WarPartyComponent> _warPartyComponentsCache;

		[CachedData]
		private MBList<Clan> _clans;

		[SaveableField(18)]
		private Clan _rulingClan;

		[SaveableField(20)]
		private MBList<Army> _armies;

		[CachedData]
		private float _distanceToClosestNonAllyFortificationCache;

		[CachedData]
		internal bool _distanceToClosestNonAllyFortificationCacheDirty = true;

		[SaveableField(23)]
		public int PoliticalStagnation;

		[SaveableField(26)]
		private List<PolicyObject> _activePolicies;

		[SaveableField(29)]
		private bool _isEliminated;

		[SaveableField(60)]
		private float _aggressiveness;

		[CachedData]
		private Settlement _kingdomMidSettlement;

		[SaveableField(80)]
		private int _tributeWallet;

		[SaveableField(81)]
		private int _kingdomBudgetWallet;
	}
}
