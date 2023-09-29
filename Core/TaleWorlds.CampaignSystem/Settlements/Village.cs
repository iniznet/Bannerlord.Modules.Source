﻿using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Settlements
{
	public class Village : SettlementComponent
	{
		internal static void AutoGeneratedStaticCollectObjectsVillage(object o, List<object> collectedObjects)
		{
			((Village)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._bound);
			collectedObjects.Add(this._marketData);
		}

		internal static object AutoGeneratedGetMemberValueLastDemandSatisfiedTime(object o)
		{
			return ((Village)o).LastDemandSatisfiedTime;
		}

		internal static object AutoGeneratedGetMemberValueHearth(object o)
		{
			return ((Village)o).Hearth;
		}

		internal static object AutoGeneratedGetMemberValueTradeTaxAccumulated(object o)
		{
			return ((Village)o).TradeTaxAccumulated;
		}

		internal static object AutoGeneratedGetMemberValue_villageState(object o)
		{
			return ((Village)o)._villageState;
		}

		internal static object AutoGeneratedGetMemberValue_bound(object o)
		{
			return ((Village)o)._bound;
		}

		internal static object AutoGeneratedGetMemberValue_marketData(object o)
		{
			return ((Village)o)._marketData;
		}

		public static MBReadOnlyList<Village> All
		{
			get
			{
				return Campaign.Current.AllVillages;
			}
		}

		public IEnumerable<PartyBase> GetDefenderParties(MapEvent.BattleTypes battleType)
		{
			yield return base.Settlement.Party;
			foreach (MobileParty mobileParty in base.Settlement.Parties)
			{
				if (mobileParty.MapFaction == base.Settlement.MapFaction && ((!mobileParty.IsMilitia && !mobileParty.IsVillager) || battleType == MapEvent.BattleTypes.Raid || battleType == MapEvent.BattleTypes.IsForcingSupplies || battleType == MapEvent.BattleTypes.IsForcingVolunteers) && !mobileParty.IsCaravan)
				{
					yield return mobileParty.Party;
				}
			}
			List<MobileParty>.Enumerator enumerator = default(List<MobileParty>.Enumerator);
			yield break;
			yield break;
		}

		public PartyBase GetNextDefenderParty(ref int partyIndex, MapEvent.BattleTypes battleType)
		{
			partyIndex++;
			if (partyIndex == 0)
			{
				return base.Settlement.Party;
			}
			for (int i = partyIndex - 1; i < base.Settlement.Parties.Count; i++)
			{
				MobileParty mobileParty = base.Settlement.Parties[i];
				if (mobileParty.MapFaction == base.Settlement.MapFaction && !mobileParty.IsCaravan)
				{
					partyIndex = i + 1;
					return mobileParty.Party;
				}
			}
			return null;
		}

		public Village.VillageStates VillageState
		{
			get
			{
				return this._villageState;
			}
			set
			{
				if (this._villageState != value)
				{
					this._villageState = value;
					switch (this._villageState)
					{
					case Village.VillageStates.Normal:
						CampaignEventDispatcher.Instance.OnVillageBecomeNormal(this);
						return;
					case Village.VillageStates.BeingRaided:
						CampaignEventDispatcher.Instance.OnVillageBeingRaided(this);
						return;
					case Village.VillageStates.ForcedForVolunteers:
					case Village.VillageStates.ForcedForSupplies:
						break;
					case Village.VillageStates.Looted:
						CampaignEventDispatcher.Instance.OnVillageLooted(this);
						break;
					default:
						return;
					}
				}
			}
		}

		public bool IsDeserted
		{
			get
			{
				return this._villageState == Village.VillageStates.Looted;
			}
		}

		[SaveableProperty(105)]
		public float LastDemandSatisfiedTime { get; private set; }

		public Settlement Bound
		{
			get
			{
				return this._bound;
			}
			private set
			{
				if (this._bound != value)
				{
					Settlement bound = this._bound;
					if (bound != null)
					{
						bound.RemoveBoundVillageInternal(this);
					}
					this._bound = value;
					Settlement bound2 = this._bound;
					if (bound2 == null)
					{
						return;
					}
					bound2.AddBoundVillageInternal(this);
				}
			}
		}

		public Settlement TradeBound
		{
			get
			{
				if (!this._bound.IsTown)
				{
					return this._tradeBound;
				}
				return this._bound;
			}
			internal set
			{
				if (this._tradeBound != value && !this._bound.IsTown)
				{
					Settlement tradeBound = this._tradeBound;
					if (tradeBound != null)
					{
						tradeBound.Town.RemoveTradeBoundVillageInternal(this);
					}
					this._tradeBound = value;
					Settlement tradeBound2 = this._tradeBound;
					if (tradeBound2 == null)
					{
						return;
					}
					tradeBound2.Town.SetTradeBoundVillageInternal(this);
				}
			}
		}

		public VillageMarketData MarketData
		{
			get
			{
				return this._marketData;
			}
		}

		[SaveableProperty(108)]
		public float Hearth { get; set; }

		[SaveableProperty(110)]
		public int TradeTaxAccumulated { get; set; }

		public Village()
		{
			this.LastDemandSatisfiedTime = -1f;
			this._marketData = new VillageMarketData(this);
		}

		public void DailyTick()
		{
			int hearthLevel = this.GetHearthLevel();
			this.Hearth += this.HearthChange;
			if (hearthLevel != this.GetHearthLevel())
			{
				base.Settlement.Party.SetLevelMaskIsDirty();
			}
			if (this.Hearth < 10f)
			{
				this.Hearth = 10f;
			}
			base.Owner.Settlement.Militia += this.MilitiaChange;
			if (base.Gold > 1000)
			{
				base.ChangeGold(1000 - base.Gold);
			}
		}

		public override void OnInit()
		{
			ChangeVillageStateAction.ApplyBySettingToNormal(base.Settlement);
			base.ChangeGold(1000);
		}

		public int GetWerehouseCapacity()
		{
			float num = Campaign.Current.Models.VillageProductionCalculatorModel.CalculateDailyFoodProductionAmount(this);
			foreach (ValueTuple<ItemObject, float> valueTuple in this.VillageType.Productions)
			{
				float num2 = Campaign.Current.Models.VillageProductionCalculatorModel.CalculateDailyProductionAmount(this, valueTuple.Item1);
				num += num2;
			}
			return MathF.Ceiling(MathF.Max(1f, num) * 5f);
		}

		public override int GetItemPrice(ItemObject item, MobileParty tradingParty = null, bool isSelling = false)
		{
			if (this.TradeBound == null)
			{
				return 1;
			}
			return this.TradeBound.Town.MarketData.GetPrice(item, tradingParty, isSelling, null);
		}

		public override int GetItemPrice(EquipmentElement itemRosterElement, MobileParty tradingParty = null, bool isSelling = false)
		{
			if (this.TradeBound == null)
			{
				return 1;
			}
			return this.TradeBound.Town.MarketData.GetPrice(itemRosterElement, tradingParty, isSelling, null);
		}

		public override string ToString()
		{
			return base.Name.ToString();
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			bool isInitialized = base.IsInitialized;
			base.Deserialize(objectManager, node);
			base.BackgroundCropPosition = float.Parse(node.Attributes["background_crop_position"].Value);
			base.BackgroundMeshName = node.Attributes["background_mesh"].Value;
			base.CastleBackgroundMeshName = node.Attributes["castle_background_mesh"].Value;
			base.WaitMeshName = node.Attributes["wait_mesh"].Value;
			if (!isInitialized)
			{
				this.Hearth = (float)int.Parse(node.Attributes["hearth"].Value);
			}
			this.VillageType = (VillageType)objectManager.ReadObjectReferenceFromXml("village_type", typeof(VillageType), node);
			if (!isInitialized)
			{
				this.Bound = (Settlement)objectManager.ReadObjectReferenceFromXml("bound", typeof(Settlement), node);
				if (this.Bound.IsTown)
				{
					this.Bound.Town.SetTradeBoundVillageInternal(this);
				}
			}
		}

		public bool IsProducing(ItemObject item)
		{
			using (List<ValueTuple<ItemObject, float>>.Enumerator enumerator = this.VillageType.Productions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Item1 == item)
					{
						return true;
					}
				}
			}
			return false;
		}

		public float HearthChange
		{
			get
			{
				return Campaign.Current.Models.SettlementProsperityModel.CalculateHearthChange(this, false).ResultNumber;
			}
		}

		public float Militia
		{
			get
			{
				return base.Owner.Settlement.Militia;
			}
		}

		public float MilitiaChange
		{
			get
			{
				return Campaign.Current.Models.SettlementMilitiaModel.CalculateMilitiaChange(base.Owner.Settlement, false).ResultNumber;
			}
		}

		public ExplainedNumber MilitiaChangeExplanation
		{
			get
			{
				return Campaign.Current.Models.SettlementMilitiaModel.CalculateMilitiaChange(base.Owner.Settlement, true);
			}
		}

		public ExplainedNumber HearthChangeExplanation
		{
			get
			{
				return Campaign.Current.Models.SettlementProsperityModel.CalculateHearthChange(this, true);
			}
		}

		public int GetHearthLevel()
		{
			if (this.Hearth >= 600f)
			{
				return 2;
			}
			if (this.Hearth >= 200f)
			{
				return 1;
			}
			return 0;
		}

		public override SettlementComponent.ProsperityLevel GetProsperityLevel()
		{
			if (this.GetHearthLevel() >= 2)
			{
				return SettlementComponent.ProsperityLevel.High;
			}
			if (this.GetHearthLevel() >= 1)
			{
				return SettlementComponent.ProsperityLevel.Mid;
			}
			return SettlementComponent.ProsperityLevel.Low;
		}

		protected override void OnInventoryUpdated(ItemRosterElement item, int count)
		{
		}

		public const int MidHearthThreshold = 600;

		public const int LowHearthThreshold = 200;

		private const int InitialVillageGold = 1000;

		public const int NumberOfDaysToFillVillageStocks = 5;

		[CachedData]
		public VillagerPartyComponent VillagerPartyComponent;

		[SaveableField(104)]
		private Village.VillageStates _villageState;

		[SaveableField(106)]
		private Settlement _bound;

		public VillageType VillageType;

		private Settlement _tradeBound;

		[SaveableField(107)]
		private VillageMarketData _marketData;

		public enum VillageStates
		{
			Normal,
			BeingRaided,
			ForcedForVolunteers,
			ForcedForSupplies,
			Looted
		}
	}
}
