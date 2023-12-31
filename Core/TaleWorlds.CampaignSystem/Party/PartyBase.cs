﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.CampaignSystem.Party
{
	public sealed class PartyBase : IBattleCombatant, IRandomOwner
	{
		internal static void AutoGeneratedStaticCollectObjectsPartyBase(object o, List<object> collectedObjects)
		{
			((PartyBase)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this._lastEatingTime, collectedObjects);
			collectedObjects.Add(this._customOwner);
			collectedObjects.Add(this._mapEventSide);
			collectedObjects.Add(this.Settlement);
			collectedObjects.Add(this.MobileParty);
			collectedObjects.Add(this.MemberRoster);
			collectedObjects.Add(this.PrisonRoster);
			collectedObjects.Add(this.ItemRoster);
		}

		internal static object AutoGeneratedGetMemberValueSettlement(object o)
		{
			return ((PartyBase)o).Settlement;
		}

		internal static object AutoGeneratedGetMemberValueMobileParty(object o)
		{
			return ((PartyBase)o).MobileParty;
		}

		internal static object AutoGeneratedGetMemberValueMemberRoster(object o)
		{
			return ((PartyBase)o).MemberRoster;
		}

		internal static object AutoGeneratedGetMemberValuePrisonRoster(object o)
		{
			return ((PartyBase)o).PrisonRoster;
		}

		internal static object AutoGeneratedGetMemberValueItemRoster(object o)
		{
			return ((PartyBase)o).ItemRoster;
		}

		internal static object AutoGeneratedGetMemberValueRandomValue(object o)
		{
			return ((PartyBase)o).RandomValue;
		}

		internal static object AutoGeneratedGetMemberValueAverageBearingRotation(object o)
		{
			return ((PartyBase)o).AverageBearingRotation;
		}

		internal static object AutoGeneratedGetMemberValue_remainingFoodPercentage(object o)
		{
			return ((PartyBase)o)._remainingFoodPercentage;
		}

		internal static object AutoGeneratedGetMemberValue_lastEatingTime(object o)
		{
			return ((PartyBase)o)._lastEatingTime;
		}

		internal static object AutoGeneratedGetMemberValue_customOwner(object o)
		{
			return ((PartyBase)o)._customOwner;
		}

		internal static object AutoGeneratedGetMemberValue_index(object o)
		{
			return ((PartyBase)o)._index;
		}

		internal static object AutoGeneratedGetMemberValue_mapEventSide(object o)
		{
			return ((PartyBase)o)._mapEventSide;
		}

		internal static object AutoGeneratedGetMemberValue_numberOfMenWithHorse(object o)
		{
			return ((PartyBase)o)._numberOfMenWithHorse;
		}

		public Vec2 Position2D
		{
			get
			{
				if (!this.IsMobile)
				{
					return this.Settlement.Position2D;
				}
				return this.MobileParty.Position2D;
			}
		}

		public bool IsVisible
		{
			get
			{
				if (!this.IsMobile)
				{
					return this.Settlement.IsVisible;
				}
				return this.MobileParty.IsVisible;
			}
		}

		public bool IsActive
		{
			get
			{
				if (!this.IsMobile)
				{
					return this.Settlement.IsActive;
				}
				return this.MobileParty.IsActive;
			}
		}

		public SiegeEvent SiegeEvent
		{
			get
			{
				if (!this.IsMobile)
				{
					return this.Settlement.SiegeEvent;
				}
				return this.MobileParty.SiegeEvent;
			}
		}

		public void OnVisibilityChanged(bool value)
		{
			MapEvent mapEvent = this.MapEvent;
			if (mapEvent != null)
			{
				mapEvent.PartyVisibilityChanged(this, value);
			}
			CampaignEventDispatcher.Instance.OnPartyVisibilityChanged(this);
			this.SetVisualAsDirty();
		}

		[SaveableProperty(1)]
		public Settlement Settlement { get; private set; }

		[SaveableProperty(2)]
		public MobileParty MobileParty { get; private set; }

		public bool IsSettlement
		{
			get
			{
				return this.Settlement != null;
			}
		}

		public bool IsMobile
		{
			get
			{
				return this.MobileParty != null;
			}
		}

		[SaveableProperty(3)]
		public TroopRoster MemberRoster { get; private set; }

		[SaveableProperty(4)]
		public TroopRoster PrisonRoster { get; private set; }

		[SaveableProperty(5)]
		public ItemRoster ItemRoster { get; private set; }

		public TextObject Name
		{
			get
			{
				if (this.IsSettlement)
				{
					return this.Settlement.Name;
				}
				if (!this.IsMobile)
				{
					return TextObject.Empty;
				}
				return this.MobileParty.Name;
			}
		}

		public float DaysStarving
		{
			get
			{
				if (!this.IsStarving)
				{
					return 0f;
				}
				return this._lastEatingTime.ElapsedDaysUntilNow;
			}
		}

		public void OnConsumedFood()
		{
			this._lastEatingTime = CampaignTime.Now;
		}

		public int RemainingFoodPercentage
		{
			get
			{
				return this._remainingFoodPercentage;
			}
			set
			{
				this._remainingFoodPercentage = value;
			}
		}

		public bool IsStarving
		{
			get
			{
				return this._remainingFoodPercentage < 0;
			}
		}

		public string Id
		{
			get
			{
				MobileParty mobileParty = this.MobileParty;
				return ((mobileParty != null) ? mobileParty.StringId : null) ?? this.Settlement.StringId;
			}
		}

		public Hero Owner
		{
			get
			{
				Hero hero;
				if ((hero = this._customOwner) == null)
				{
					if (!this.IsMobile)
					{
						return this.Settlement.Owner;
					}
					hero = this.MobileParty.Owner;
				}
				return hero;
			}
		}

		public void SetCustomOwner(Hero customOwner)
		{
			this._customOwner = customOwner;
		}

		public Hero LeaderHero
		{
			get
			{
				MobileParty mobileParty = this.MobileParty;
				if (mobileParty == null)
				{
					return null;
				}
				return mobileParty.LeaderHero;
			}
		}

		public static PartyBase MainParty
		{
			get
			{
				if (Campaign.Current == null)
				{
					return null;
				}
				return Campaign.Current.MainParty.Party;
			}
		}

		public bool LevelMaskIsDirty { get; private set; }

		public void SetLevelMaskIsDirty()
		{
			this.LevelMaskIsDirty = true;
		}

		public void OnLevelMaskUpdated()
		{
			this.LevelMaskIsDirty = false;
		}

		public int Index
		{
			get
			{
				return this._index;
			}
			private set
			{
				this._index = value;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.Index >= 0;
			}
		}

		public IMapEntity MapEntity
		{
			get
			{
				if (this.IsMobile)
				{
					return this.MobileParty;
				}
				return this.Settlement;
			}
		}

		public IFaction MapFaction
		{
			get
			{
				if (this.MobileParty != null)
				{
					return this.MobileParty.MapFaction;
				}
				if (this.Settlement != null)
				{
					return this.Settlement.MapFaction;
				}
				return null;
			}
		}

		[SaveableProperty(210)]
		public int RandomValue { get; private set; } = MBRandom.RandomInt(1, int.MaxValue);

		public CultureObject Culture
		{
			get
			{
				return this.MapFaction.Culture;
			}
		}

		public Tuple<uint, uint> PrimaryColorPair
		{
			get
			{
				if (this.MapFaction == null)
				{
					return new Tuple<uint, uint>(4291609515U, 4291609515U);
				}
				return new Tuple<uint, uint>(this.MapFaction.Color, this.MapFaction.Color2);
			}
		}

		public Tuple<uint, uint> AlternativeColorPair
		{
			get
			{
				if (this.MapFaction == null)
				{
					return new Tuple<uint, uint>(4291609515U, 4291609515U);
				}
				return new Tuple<uint, uint>(this.MapFaction.AlternativeColor, this.MapFaction.AlternativeColor2);
			}
		}

		public Banner Banner
		{
			get
			{
				if (this.LeaderHero != null)
				{
					return this.LeaderHero.ClanBanner;
				}
				IFaction mapFaction = this.MapFaction;
				if (mapFaction == null)
				{
					return null;
				}
				return mapFaction.Banner;
			}
		}

		int IBattleCombatant.GetTacticsSkillAmount()
		{
			if (this.LeaderHero != null)
			{
				return this.LeaderHero.GetSkillValue(DefaultSkills.Tactics);
			}
			return 0;
		}

		public MapEvent MapEvent
		{
			get
			{
				MapEventSide mapEventSide = this._mapEventSide;
				if (mapEventSide == null)
				{
					return null;
				}
				return mapEventSide.MapEvent;
			}
		}

		public MapEventSide MapEventSide
		{
			get
			{
				return this._mapEventSide;
			}
			set
			{
				if (this._mapEventSide != value)
				{
					if (value != null && this.IsMobile && this.MapEvent != null && this.MapEvent.DefenderSide.LeaderParty == this)
					{
						Debug.FailedAssert(string.Format("Double MapEvent For {0}", this.Name), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Party\\PartyBase.cs", "MapEventSide", 246);
					}
					if (this._mapEventSide != null)
					{
						this._mapEventSide.RemovePartyInternal(this);
					}
					this._mapEventSide = value;
					if (this._mapEventSide != null)
					{
						this._mapEventSide.AddPartyInternal(this);
					}
					if (this.MobileParty != null)
					{
						foreach (MobileParty mobileParty in this.MobileParty.AttachedParties)
						{
							mobileParty.Party.MapEventSide = this._mapEventSide;
						}
					}
				}
			}
		}

		public BattleSideEnum Side
		{
			get
			{
				MapEventSide mapEventSide = this.MapEventSide;
				if (mapEventSide == null)
				{
					return BattleSideEnum.None;
				}
				return mapEventSide.MissionSide;
			}
		}

		public BattleSideEnum OpponentSide
		{
			get
			{
				if (this.Side == BattleSideEnum.Attacker)
				{
					return BattleSideEnum.Defender;
				}
				return BattleSideEnum.Attacker;
			}
		}

		internal void AfterLoad()
		{
			if (this.RandomValue == 0)
			{
				this.RandomValue = MBRandom.RandomInt(1, int.MaxValue);
			}
			TroopRoster prisonRoster = this.PrisonRoster;
			if (prisonRoster != null && prisonRoster.Contains(CharacterObject.PlayerCharacter) && (this != Hero.MainHero.PartyBelongedToAsPrisoner || (Hero.MainHero.PartyBelongedTo != null && Hero.MainHero.PartyBelongedToAsPrisoner != null)))
			{
				MobileParty partyBelongedTo = Hero.MainHero.PartyBelongedTo;
				PartyBase mainParty = PartyBase.MainParty;
				if (partyBelongedTo == ((mainParty != null) ? mainParty.MobileParty : null))
				{
					this.PrisonRoster.AddToCounts(CharacterObject.PlayerCharacter, -1, false, 0, 0, true, -1);
				}
				else
				{
					PlayerCaptivity.CaptorParty = this;
				}
			}
			if (this.IsMobile && this.MobileParty.IsCaravan && !this.MobileParty.IsCurrentlyUsedByAQuest && this._customOwner != null && this.MobileParty.Owner != this.Owner)
			{
				this.SetCustomOwner(null);
			}
			foreach (TroopRosterElement troopRosterElement in this.PrisonRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.HeroObject != null && troopRosterElement.Character.HeroObject.PartyBelongedToAsPrisoner == null)
				{
					this.PrisonRoster.RemoveTroop(troopRosterElement.Character, 1, default(UniqueTroopDescriptor), 0);
				}
			}
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.0", 27066))
			{
				this.MemberRoster.RemoveZeroCounts();
			}
		}

		internal void InitCache()
		{
			this._partyMemberSizeLastCheckVersion = -1;
			this._prisonerSizeLastCheckVersion = -1;
			this._lastNumberOfMenWithHorseVersionNo = -1;
			this._lastNumberOfMenPerTierVersionNo = -1;
			this._lastMemberRosterVersionNo = -1;
		}

		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData, ObjectLoadData objectLoadData)
		{
			this.InitCache();
		}

		public int PartySizeLimit
		{
			get
			{
				int versionNo = this.MemberRoster.VersionNo;
				if (this._partyMemberSizeLastCheckVersion != versionNo || this._cachedPartyMemberSizeLimit == 0)
				{
					this._partyMemberSizeLastCheckVersion = versionNo;
					this._cachedPartyMemberSizeLimit = (int)Campaign.Current.Models.PartySizeLimitModel.GetPartyMemberSizeLimit(this, false).ResultNumber;
				}
				return this._cachedPartyMemberSizeLimit;
			}
		}

		public int PrisonerSizeLimit
		{
			get
			{
				int versionNo = this.MemberRoster.VersionNo;
				if (this._prisonerSizeLastCheckVersion != versionNo || this._cachedPrisonerSizeLimit == 0)
				{
					this._prisonerSizeLastCheckVersion = versionNo;
					this._cachedPrisonerSizeLimit = (int)Campaign.Current.Models.PartySizeLimitModel.GetPartyPrisonerSizeLimit(this, false).ResultNumber;
				}
				return this._cachedPrisonerSizeLimit;
			}
		}

		public ExplainedNumber PartySizeLimitExplainer
		{
			get
			{
				return Campaign.Current.Models.PartySizeLimitModel.GetPartyMemberSizeLimit(this, true);
			}
		}

		public ExplainedNumber PrisonerSizeLimitExplainer
		{
			get
			{
				return Campaign.Current.Models.PartySizeLimitModel.GetPartyPrisonerSizeLimit(this, true);
			}
		}

		public int NumberOfHealthyMembers
		{
			get
			{
				return this.MemberRoster.TotalManCount - this.MemberRoster.TotalWounded;
			}
		}

		public int NumberOfRegularMembers
		{
			get
			{
				return this.MemberRoster.TotalRegulars;
			}
		}

		public int NumberOfWoundedTotalMembers
		{
			get
			{
				return this.MemberRoster.TotalWounded;
			}
		}

		public int NumberOfAllMembers
		{
			get
			{
				return this.MemberRoster.TotalManCount;
			}
		}

		public int NumberOfPrisoners
		{
			get
			{
				return this.PrisonRoster.TotalManCount;
			}
		}

		public int NumberOfMounts
		{
			get
			{
				return this.ItemRoster.NumberOfMounts;
			}
		}

		public int NumberOfPackAnimals
		{
			get
			{
				return this.ItemRoster.NumberOfPackAnimals;
			}
		}

		public IEnumerable<CharacterObject> PrisonerHeroes
		{
			get
			{
				int num;
				for (int i = 0; i < this.PrisonRoster.Count; i = num + 1)
				{
					if (this.PrisonRoster.GetElementNumber(i) > 0)
					{
						TroopRosterElement elementCopyAtIndex = this.PrisonRoster.GetElementCopyAtIndex(i);
						if (elementCopyAtIndex.Character.IsHero)
						{
							yield return elementCopyAtIndex.Character;
						}
					}
					num = i;
				}
				yield break;
			}
		}

		public int NumberOfMenWithHorse
		{
			get
			{
				if (this._lastNumberOfMenWithHorseVersionNo != this.MemberRoster.VersionNo)
				{
					this.RecalculateNumberOfMenWithHorses();
					this._lastNumberOfMenWithHorseVersionNo = this.MemberRoster.VersionNo;
				}
				return this._numberOfMenWithHorse;
			}
		}

		public int NumberOfMenWithoutHorse
		{
			get
			{
				return this.NumberOfAllMembers - this.NumberOfMenWithHorse;
			}
		}

		public int GetNumberOfHealthyMenOfTier(int tier)
		{
			if (tier < 0)
			{
				Debug.FailedAssert("Requested men count for negative tier.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Party\\PartyBase.cs", "GetNumberOfHealthyMenOfTier", 461);
				return 0;
			}
			bool flag = false;
			if (this._numberOfHealthyMenPerTier == null || tier >= this._numberOfHealthyMenPerTier.Length)
			{
				int num = MathF.Max(tier, 6);
				this._numberOfHealthyMenPerTier = new int[num + 1];
				flag = true;
			}
			else if (this._lastNumberOfMenPerTierVersionNo != this.MemberRoster.VersionNo)
			{
				flag = true;
			}
			if (flag)
			{
				for (int i = 0; i < this._numberOfHealthyMenPerTier.Length; i++)
				{
					this._numberOfHealthyMenPerTier[i] = 0;
				}
				for (int j = 0; j < this.MemberRoster.Count; j++)
				{
					CharacterObject characterAtIndex = this.MemberRoster.GetCharacterAtIndex(j);
					if (characterAtIndex != null && !characterAtIndex.IsHero)
					{
						int tier2 = characterAtIndex.Tier;
						if (tier2 >= 0 && tier2 < this._numberOfHealthyMenPerTier.Length)
						{
							int num2 = this.MemberRoster.GetElementNumber(j) - this.MemberRoster.GetElementWoundedNumber(j);
							this._numberOfHealthyMenPerTier[tier2] += num2;
						}
					}
				}
				this._lastNumberOfMenPerTierVersionNo = this.MemberRoster.VersionNo;
			}
			return this._numberOfHealthyMenPerTier[tier];
		}

		public int InventoryCapacity
		{
			get
			{
				if (this.MobileParty == null)
				{
					return 100;
				}
				return (int)Campaign.Current.Models.InventoryCapacityModel.CalculateInventoryCapacity(this.MobileParty, false, 0, 0, 0, false).ResultNumber;
			}
		}

		public float TotalStrength
		{
			get
			{
				if (this._lastMemberRosterVersionNo == this.MemberRoster.VersionNo)
				{
					return this._cachedTotalStrength;
				}
				this._cachedTotalStrength = this.CalculateStrength();
				this._lastMemberRosterVersionNo = this.MemberRoster.VersionNo;
				return this._cachedTotalStrength;
			}
		}

		public PartyBase(MobileParty mobileParty)
			: this(mobileParty, null)
		{
		}

		public PartyBase(Settlement settlement)
			: this(null, settlement)
		{
		}

		private PartyBase(MobileParty mobileParty, Settlement settlement)
		{
			this.Index = Campaign.Current.GeneratePartyId(this);
			this.MobileParty = mobileParty;
			this.Settlement = settlement;
			this.ItemRoster = new ItemRoster();
			this.MemberRoster = new TroopRoster(this);
			this.PrisonRoster = new TroopRoster(this);
			this.MemberRoster.NumberChangedCallback = new NumberChangedCallback(this.MemberRosterNumberChanged);
			this.PrisonRoster.IsPrisonRoster = true;
		}

		private void RecalculateNumberOfMenWithHorses()
		{
			this._numberOfMenWithHorse = 0;
			for (int i = 0; i < this.MemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = this.MemberRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character != null && elementCopyAtIndex.Character.IsMounted)
				{
					this._numberOfMenWithHorse += elementCopyAtIndex.Number;
				}
			}
		}

		public int GetNumberOfMenWith(TraitObject trait)
		{
			int num = 0;
			foreach (TroopRosterElement troopRosterElement in this.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.GetTraitLevel(trait) > 0)
				{
					num += troopRosterElement.Number;
				}
			}
			return num;
		}

		public int AddPrisoner(CharacterObject element, int numberToAdd)
		{
			return this.PrisonRoster.AddToCounts(element, numberToAdd, false, 0, 0, true, -1);
		}

		public int AddMember(CharacterObject element, int numberToAdd, int numberToAddWounded = 0)
		{
			return this.MemberRoster.AddToCounts(element, numberToAdd, false, numberToAddWounded, 0, true, -1);
		}

		public void AddPrisoners(TroopRoster roster)
		{
			foreach (TroopRosterElement troopRosterElement in roster.GetTroopRoster())
			{
				this.AddPrisoner(troopRosterElement.Character, troopRosterElement.Number);
			}
		}

		public void AddMembers(TroopRoster roster)
		{
			this.MemberRoster.Add(roster);
		}

		public override string ToString()
		{
			if (!this.IsSettlement)
			{
				return this.MobileParty.Name.ToString();
			}
			return this.Settlement.Name.ToString();
		}

		public void PlaceRandomPositionAroundPosition(Vec2 centerPosition, float radius)
		{
			Vec2 vec = new Vec2(0f, 0f);
			bool flag;
			do
			{
				vec.x = centerPosition.x + MBRandom.RandomFloat * radius * 2f - radius;
				vec.y = centerPosition.y + MBRandom.RandomFloat * radius * 2f - radius;
				PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(vec);
				PathFaceRecord faceIndex2 = Campaign.Current.MapSceneWrapper.GetFaceIndex(centerPosition);
				flag = Campaign.Current.MapSceneWrapper.AreFacesOnSameIsland(faceIndex, faceIndex2, false);
			}
			while (!flag);
			if (this.IsMobile)
			{
				this.MobileParty.Position2D = vec;
				this.MobileParty.Ai.SetMoveModeHold();
			}
		}

		public int AddElementToMemberRoster(CharacterObject element, int numberToAdd, bool insertAtFront = false)
		{
			return this.MemberRoster.AddToCounts(element, numberToAdd, insertAtFront, 0, 0, true, -1);
		}

		public void AddToMemberRosterElementAtIndex(int index, int numberToAdd, int woundedCount = 0)
		{
			this.MemberRoster.AddToCountsAtIndex(index, numberToAdd, woundedCount, 0, true);
		}

		public void WoundMemberRosterElements(CharacterObject elementObj, int numberToWound)
		{
			this.MemberRoster.AddToCounts(elementObj, 0, false, numberToWound, 0, true, -1);
		}

		public void WoundMemberRosterElementsWithIndex(int elementIndex, int numberToWound)
		{
			this.MemberRoster.AddToCountsAtIndex(elementIndex, 0, numberToWound, 0, true);
		}

		private float CalculateStrength()
		{
			float num = 0f;
			float num2 = 0f;
			MapEvent.PowerCalculationContext powerCalculationContext = MapEvent.PowerCalculationContext.Default;
			BattleSideEnum battleSideEnum = BattleSideEnum.Defender;
			if (this.MapEvent != null)
			{
				num2 = Campaign.Current.Models.MilitaryPowerModel.GetLeaderModifierInMapEvent(this.MapEvent, this.Side);
				powerCalculationContext = this.MapEvent.SimulationContext;
			}
			for (int i = 0; i < this.MemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = this.MemberRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character != null)
				{
					float troopPower = Campaign.Current.Models.MilitaryPowerModel.GetTroopPower(elementCopyAtIndex.Character, battleSideEnum, powerCalculationContext, num2);
					num += (float)(elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber) * troopPower;
				}
			}
			return num;
		}

		internal bool GetCharacterFromPartyRank(int partyRank, out CharacterObject character, out PartyBase party, out int stackIndex, bool includeWoundeds = false)
		{
			for (int i = 0; i < this.MemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = this.MemberRoster.GetElementCopyAtIndex(i);
				int num = elementCopyAtIndex.Number - (includeWoundeds ? 0 : elementCopyAtIndex.WoundedNumber);
				partyRank -= num;
				if (!elementCopyAtIndex.Character.IsHero && partyRank < 0)
				{
					character = elementCopyAtIndex.Character;
					party = this;
					stackIndex = i;
					return true;
				}
			}
			character = null;
			party = null;
			stackIndex = 0;
			return false;
		}

		public static bool IsPositionOkForTraveling(Vec2 position)
		{
			IMapScene mapSceneWrapper = Campaign.Current.MapSceneWrapper;
			PathFaceRecord faceIndex = mapSceneWrapper.GetFaceIndex(position);
			if (!faceIndex.IsValid())
			{
				return false;
			}
			TerrainType faceTerrainType = mapSceneWrapper.GetFaceTerrainType(faceIndex);
			return PartyBase.ValidTerrainTypes.Contains(faceTerrainType);
		}

		private void MemberRosterNumberChanged(bool numberChanged, bool woundedNumberChanged, bool heroNumberChanged)
		{
			if (numberChanged || heroNumberChanged)
			{
				CampaignEventDispatcher.Instance.OnPartySizeChanged(this);
			}
		}

		public void UpdateVisibilityAndInspected(float mainPartySeeingRange = 0f)
		{
			bool flag = false;
			bool flag2 = false;
			if (this.IsSettlement)
			{
				flag = true;
				ISpottable spottable;
				if ((spottable = this.Settlement.SettlementComponent as ISpottable) != null && !spottable.IsSpotted)
				{
					flag = false;
				}
				if (flag)
				{
					flag2 = PartyBase.CalculateSettlementInspected(this.Settlement, mainPartySeeingRange);
				}
			}
			else if (this.MobileParty.IsActive)
			{
				if (Campaign.Current.TrueSight)
				{
					flag = true;
				}
				else
				{
					if (this.MobileParty.CurrentSettlement != null)
					{
						Hero leaderHero = this.MobileParty.LeaderHero;
						if (((leaderHero != null) ? leaderHero.ClanBanner : null) == null && (this.MobileParty.MapEvent == null || !this.MobileParty.MapEvent.IsSiegeAssault || this.MobileParty.Party.Side != BattleSideEnum.Attacker))
						{
							goto IL_C9;
						}
					}
					PartyBase.CalculateVisibilityAndInspected(this.MobileParty, out flag, out flag2, mainPartySeeingRange);
				}
			}
			IL_C9:
			if (this.IsSettlement)
			{
				this.Settlement.IsVisible = flag;
				this.Settlement.IsInspected = flag2;
				return;
			}
			this.MobileParty.IsVisible = flag;
			this.MobileParty.IsInspected = flag2;
		}

		private static void CalculateVisibilityAndInspected(IMapPoint mapPoint, out bool isVisible, out bool isInspected, float mainPartySeeingRange = 0f)
		{
			isInspected = false;
			MobileParty mobileParty = mapPoint as MobileParty;
			if (((mobileParty != null) ? mobileParty.Army : null) != null && mobileParty.Army.LeaderParty.AttachedParties.IndexOf(mobileParty) >= 0)
			{
				isVisible = mobileParty.Army.LeaderParty.IsVisible;
				return;
			}
			if (mobileParty != null && MobileParty.MainParty.CurrentSettlement != null && MobileParty.MainParty.CurrentSettlement.SiegeEvent != null && MobileParty.MainParty.CurrentSettlement.SiegeEvent.BesiegerCamp.IsBesiegerSideParty(mobileParty))
			{
				isVisible = true;
				return;
			}
			float num = PartyBase.CalculateVisibilityRangeOfMapPoint(mapPoint, mainPartySeeingRange);
			isVisible = num > 1f && mapPoint.IsActive;
			if (isVisible)
			{
				if (mapPoint.IsInspected)
				{
					isInspected = true;
					return;
				}
				isInspected = 1f / num < Campaign.Current.Models.MapVisibilityModel.GetPartyRelativeInspectionRange(mapPoint);
			}
		}

		private static bool CalculateSettlementInspected(IMapPoint mapPoint, float mainPartySeeingRange = 0f)
		{
			return 1f / PartyBase.CalculateVisibilityRangeOfMapPoint(mapPoint, mainPartySeeingRange) < Campaign.Current.Models.MapVisibilityModel.GetPartyRelativeInspectionRange(mapPoint);
		}

		private static float CalculateVisibilityRangeOfMapPoint(IMapPoint mapPoint, float mainPartySeeingRange)
		{
			MobileParty mainParty = MobileParty.MainParty;
			float lengthSquared = (mainParty.Position2D - mapPoint.Position2D).LengthSquared;
			float num = mainPartySeeingRange;
			if (mainPartySeeingRange == 0f)
			{
				num = mainParty.SeeingRange;
			}
			float num2 = num * num / lengthSquared;
			float num3 = 0.25f;
			MobileParty mobileParty;
			if ((mobileParty = mapPoint as MobileParty) != null)
			{
				num3 = Campaign.Current.Models.MapVisibilityModel.GetPartySpottingDifficulty(mainParty, mobileParty);
			}
			return num2 / num3;
		}

		[SaveableProperty(12)]
		public float AverageBearingRotation { get; set; }

		public BasicCultureObject BasicCulture
		{
			get
			{
				return this.Culture;
			}
		}

		public BasicCharacterObject General
		{
			get
			{
				MobileParty mobileParty = this.MobileParty;
				if (((mobileParty != null) ? mobileParty.Army : null) != null)
				{
					MobileParty leaderParty = this.MobileParty.Army.LeaderParty;
					if (leaderParty == null)
					{
						return null;
					}
					Hero leaderHero = leaderParty.LeaderHero;
					if (leaderHero == null)
					{
						return null;
					}
					return leaderHero.CharacterObject;
				}
				else
				{
					Hero leaderHero2 = this.LeaderHero;
					if (leaderHero2 == null)
					{
						return null;
					}
					return leaderHero2.CharacterObject;
				}
			}
		}

		public void SetAsCameraFollowParty()
		{
			Campaign.Current.CameraFollowParty = this;
		}

		internal void OnFinishLoadState()
		{
			this.SetVisualAsDirty();
			MobileParty mobileParty = this.MobileParty;
			if (mobileParty != null)
			{
				mobileParty.OnFinishLoadState();
			}
			this.MemberRoster.NumberChangedCallback = new NumberChangedCallback(this.MemberRosterNumberChanged);
		}

		[CachedData]
		public bool IsVisualDirty { get; private set; }

		public void SetVisualAsDirty()
		{
			this.IsVisualDirty = true;
		}

		public void OnVisualsUpdated()
		{
			this.IsVisualDirty = false;
		}

		internal void OnHeroAdded(Hero heroObject)
		{
			MobileParty mobileParty = this.MobileParty;
			if (mobileParty == null)
			{
				return;
			}
			mobileParty.OnHeroAdded(heroObject);
		}

		internal void OnHeroRemoved(Hero heroObject)
		{
			MobileParty mobileParty = this.MobileParty;
			if (mobileParty == null)
			{
				return;
			}
			mobileParty.OnHeroRemoved(heroObject);
		}

		internal void OnHeroAddedAsPrisoner(Hero heroObject)
		{
			heroObject.OnAddedToPartyAsPrisoner(this);
		}

		internal void OnHeroRemovedAsPrisoner(Hero heroObject)
		{
			heroObject.OnRemovedFromPartyAsPrisoner(this);
		}

		public void ResetTempXp()
		{
			this.MemberRoster.ClearTempXp();
		}

		public void OnGameInitialized()
		{
			if (this.IsMobile)
			{
				this.MobileParty.OnGameInitialized();
				return;
			}
			if (this.IsSettlement)
			{
				this.Settlement.OnGameInitialized();
			}
		}

		private static readonly HashSet<TerrainType> ValidTerrainTypes = new HashSet<TerrainType>
		{
			TerrainType.Snow,
			TerrainType.Steppe,
			TerrainType.Plain,
			TerrainType.Desert,
			TerrainType.Swamp,
			TerrainType.Dune,
			TerrainType.Bridge,
			TerrainType.Forest,
			TerrainType.Fording
		};

		[SaveableField(15)]
		private int _remainingFoodPercentage;

		[SaveableField(182)]
		private CampaignTime _lastEatingTime = CampaignTime.Now;

		[SaveableField(8)]
		private Hero _customOwner;

		[SaveableField(9)]
		private int _index;

		[SaveableField(200)]
		private MapEventSide _mapEventSide;

		[CachedData]
		private int _lastMemberRosterVersionNo;

		[CachedData]
		private int _partyMemberSizeLastCheckVersion;

		[CachedData]
		private int _cachedPartyMemberSizeLimit;

		[CachedData]
		private int _prisonerSizeLastCheckVersion;

		[CachedData]
		private int _cachedPrisonerSizeLimit;

		[CachedData]
		private int _lastNumberOfMenWithHorseVersionNo;

		[CachedData]
		private int _lastNumberOfMenPerTierVersionNo;

		[SaveableField(17)]
		private int _numberOfMenWithHorse;

		private int[] _numberOfHealthyMenPerTier;

		[CachedData]
		private float _cachedTotalStrength;
	}
}
