using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class WorkshopsCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, new Action<Town>(this.DailyTickTown));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.OnWorkshopChangedEvent.AddNonSerializedListener(this, new Action<Workshop, Hero, WorkshopType>(this.OnWorkshopChanged));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		private void OnNewGameCreated(CampaignGameStarter obj)
		{
			this.FillItemsInAllCategories();
		}

		private void OnSessionLaunched(CampaignGameStarter obj)
		{
			this.FillItemsInAllCategories();
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Workshop, int>>("_playerOwnedWorkshopsDaysInBankruptcy", ref this._playerOwnedWorkshopsDaysInBankruptcy);
		}

		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int i)
		{
			if (i >= 10)
			{
				if (i == 10)
				{
					this.InitializeWorkshops();
					this.BuildWorkshopsAtGameStart();
				}
				if (i % 20 == 0)
				{
					this.RunTownShopsAtGameStart();
				}
			}
		}

		private void FillItemsInAllCategories()
		{
			foreach (ItemObject itemObject in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
			{
				if (WorkshopsCampaignBehavior.IsProducable(itemObject))
				{
					ItemCategory itemCategory = itemObject.ItemCategory;
					if (itemCategory != null)
					{
						List<ItemObject> list;
						if (!this._itemsInCategory.TryGetValue(itemCategory, out list))
						{
							list = new List<ItemObject>();
							this._itemsInCategory[itemCategory] = list;
						}
						list.Add(itemObject);
					}
				}
			}
		}

		private void OnWorkshopChanged(Workshop workshop, Hero oldOwner, WorkshopType oldType)
		{
			if (oldOwner != null && oldOwner.IsHumanPlayerCharacter)
			{
				this.RemoveBankruptcyIfExist(workshop);
			}
		}

		private void RemoveBankruptcyIfExist(Workshop workshop)
		{
			if (this._playerOwnedWorkshopsDaysInBankruptcy.ContainsKey(workshop))
			{
				this._playerOwnedWorkshopsDaysInBankruptcy.Remove(workshop);
			}
		}

		private static bool IsProducable(ItemObject item)
		{
			return !item.MultiplayerItem && !item.NotMerchandise && !item.IsCraftedByPlayer;
		}

		private void DailyTickTown(Town town)
		{
			if (!town.InRebelliousState)
			{
				foreach (Workshop workshop in town.Workshops)
				{
					this.RunTownWorkshop(town, workshop, true);
					this.HandleWorkshopConstruction(workshop);
					this.HandleDailyExpense(workshop);
				}
			}
		}

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			if (!victim.IsHumanPlayerCharacter)
			{
				foreach (Workshop workshop in victim.OwnedWorkshops.ToList<Workshop>())
				{
					Hero hero = Campaign.Current.Models.WorkshopModel.SelectNextOwnerForWorkshop(workshop.Settlement.Town, workshop, workshop.Owner, 0);
					if (hero != null)
					{
						ChangeOwnerOfWorkshopAction.ApplyByDeath(workshop, hero, null);
					}
				}
			}
		}

		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
		{
			IFaction faction3 = ((faction1 == Hero.MainHero.MapFaction) ? faction1 : ((faction2 == Hero.MainHero.MapFaction) ? faction2 : null));
			if (faction3 != null)
			{
				IFaction faction4 = ((faction3 != faction1) ? faction1 : faction2);
				int count = Hero.MainHero.OwnedWorkshops.Count;
				List<Workshop> list = Hero.MainHero.OwnedWorkshops.ToList<Workshop>();
				for (int i = 0; i < count; i++)
				{
					Workshop workshop = list[i];
					if (workshop != null && workshop.Settlement.MapFaction == faction4)
					{
						Hero hero = Campaign.Current.Models.WorkshopModel.SelectNextOwnerForWorkshop(workshop.Settlement.Town, workshop, workshop.Owner, 0);
						if (hero != null)
						{
							WorkshopType workshopType = this.DecideBestWorkshopType(workshop.Settlement, false, workshop.WorkshopType);
							ChangeOwnerOfWorkshopAction.ApplyByWarDeclaration(workshop, hero, workshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), true, null);
						}
					}
				}
			}
		}

		private void ChangeWorkshopOwnerByBankruptcy(Workshop workshop)
		{
			int sellingCost = Campaign.Current.Models.WorkshopModel.GetSellingCost(workshop);
			Hero hero = Campaign.Current.Models.WorkshopModel.SelectNextOwnerForWorkshop(workshop.Settlement.Town, workshop, workshop.Owner, sellingCost);
			if (hero != null)
			{
				WorkshopType workshopType = this.DecideBestWorkshopType(workshop.Settlement, false, workshop.WorkshopType);
				ChangeOwnerOfWorkshopAction.ApplyByBankruptcy(workshop, hero, workshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), true, sellingCost, null);
			}
		}

		private void HandleDailyExpense(Workshop shop)
		{
			if (shop.IsRunning)
			{
				int expense = shop.Expense;
				if (shop.Capital >= expense)
				{
					shop.ChangeGold(-expense);
					this.RemoveBankruptcyIfExist(shop);
					return;
				}
				if (shop.CanBeDowngraded)
				{
					shop.Downgrade();
					return;
				}
				this.DeclareBankruptcy(shop);
			}
		}

		private void DeclareBankruptcy(Workshop workshop)
		{
			if (workshop.Owner.IsHumanPlayerCharacter)
			{
				if (!this._playerOwnedWorkshopsDaysInBankruptcy.ContainsKey(workshop))
				{
					this._playerOwnedWorkshopsDaysInBankruptcy.Add(workshop, 1);
					return;
				}
				Dictionary<Workshop, int> playerOwnedWorkshopsDaysInBankruptcy = this._playerOwnedWorkshopsDaysInBankruptcy;
				int num = playerOwnedWorkshopsDaysInBankruptcy[workshop];
				playerOwnedWorkshopsDaysInBankruptcy[workshop] = num + 1;
				if (this._playerOwnedWorkshopsDaysInBankruptcy[workshop] >= Campaign.Current.Models.WorkshopModel.DaysForPlayerSaveWorkshopFromBankruptcy)
				{
					this.ChangeWorkshopOwnerByBankruptcy(workshop);
					this._playerOwnedWorkshopsDaysInBankruptcy.Remove(workshop);
					return;
				}
			}
			else
			{
				this.ChangeWorkshopOwnerByBankruptcy(workshop);
			}
		}

		private void HandleWorkshopConstruction(Workshop workshop)
		{
			if (workshop.ConstructionTimeRemained > 0)
			{
				workshop.ApplyDailyConstruction();
			}
		}

		private EquipmentElement GetRandomItem(ItemCategory itemGroupBase, Town townComponent)
		{
			EquipmentElement randomItemAux = this.GetRandomItemAux(itemGroupBase, townComponent);
			if (randomItemAux.Item != null)
			{
				return randomItemAux;
			}
			return this.GetRandomItemAux(itemGroupBase, null);
		}

		private EquipmentElement GetRandomItemAux(ItemCategory itemGroupBase, Town townComponent = null)
		{
			float num = 0f;
			ItemObject itemObject = null;
			ItemModifier itemModifier = null;
			List<ItemObject> list;
			if (this._itemsInCategory.TryGetValue(itemGroupBase, out list))
			{
				foreach (ItemObject itemObject2 in list)
				{
					if ((townComponent == null || this.IsItemPreferredForTown(itemObject2, townComponent)) && itemObject2.ItemCategory == itemGroupBase)
					{
						float num2 = 1f / ((float)MathF.Max(100, itemObject2.Value) + 100f);
						if (MBRandom.RandomFloat * (num + num2) >= num)
						{
							itemObject = itemObject2;
						}
						num += num2;
					}
				}
				ItemModifierGroup itemModifierGroup;
				if (itemObject == null)
				{
					itemModifierGroup = null;
				}
				else
				{
					ItemComponent itemComponent = itemObject.ItemComponent;
					itemModifierGroup = ((itemComponent != null) ? itemComponent.ItemModifierGroup : null);
				}
				ItemModifierGroup itemModifierGroup2 = itemModifierGroup;
				if (itemModifierGroup2 != null)
				{
					itemModifier = itemModifierGroup2.GetRandomItemModifierProductionScoreBased();
				}
			}
			return new EquipmentElement(itemObject, itemModifier, null, false);
		}

		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newSettlementOwner, Hero oldSettlementOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (settlement.IsTown)
			{
				foreach (Workshop workshop in settlement.Town.Workshops)
				{
					if (workshop.Owner != null && workshop.Owner.MapFaction.IsAtWarWith(newSettlementOwner.MapFaction) && workshop.Owner.GetPerkValue(DefaultPerks.Trade.RapidDevelopment))
					{
						GiveGoldAction.ApplyBetweenCharacters(null, workshop.Owner, MathF.Round(DefaultPerks.Trade.RapidDevelopment.PrimaryBonus), false);
					}
				}
			}
		}

		private float FindTotalInputDensityScore(Settlement bornSettlement, WorkshopType workshop, IDictionary<ItemCategory, float> productionDict, bool atGameStart)
		{
			int num = 0;
			for (int i = 0; i < bornSettlement.Town.Workshops.Length; i++)
			{
				if (bornSettlement.Town.Workshops[i].WorkshopType == workshop)
				{
					num++;
				}
			}
			float num2 = 0.01f;
			float num3 = 0f;
			foreach (WorkshopType.Production production in workshop.Productions)
			{
				bool flag = false;
				using (List<ValueTuple<ItemCategory, int>>.Enumerator enumerator2 = production.Outputs.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Item1.IsTradeGood)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					foreach (ValueTuple<ItemCategory, int> valueTuple in production.Inputs)
					{
						ItemCategory item = valueTuple.Item1;
						int item2 = valueTuple.Item2;
						float num4;
						if (productionDict.TryGetValue(item, out num4))
						{
							num2 += num4 / (production.ConversionSpeed * (float)item2);
						}
						if (!atGameStart)
						{
							float priceFactor = bornSettlement.Town.MarketData.GetPriceFactor(item);
							num3 += Math.Max(0f, 1f - priceFactor);
						}
					}
				}
			}
			float num5 = 1f + (float)(num * 6);
			num2 *= (float)workshop.Frequency * (1f / (float)Math.Pow((double)num5, 3.0));
			num2 += num3;
			num2 = MathF.Pow(num2, 0.6f);
			return num2;
		}

		private void BuildWorkshopForHeroAtGameStart(Hero ownerHero)
		{
			Settlement bornSettlement = ownerHero.BornSettlement;
			WorkshopType workshopType = this.DecideBestWorkshopType(bornSettlement, true, null);
			if (workshopType != null)
			{
				int num = -1;
				for (int i = 0; i < bornSettlement.Town.Workshops.Length; i++)
				{
					if (bornSettlement.Town.Workshops[i].WorkshopType == null)
					{
						num = i;
						break;
					}
				}
				if (num >= 0)
				{
					bornSettlement.Town.Workshops[num].SetWorkshop(ownerHero, workshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), true, 0, 1, null);
					TextObject textObject;
					TextObject textObject2;
					NameGenerator.Current.GenerateHeroNameAndHeroFullName(ownerHero, out textObject, out textObject2, true);
					ownerHero.SetName(textObject2, textObject);
				}
			}
		}

		private WorkshopType DecideBestWorkshopType(Settlement currentSettlement, bool atGameStart, WorkshopType workshopToExclude = null)
		{
			IDictionary<ItemCategory, float> dictionary = new Dictionary<ItemCategory, float>();
			foreach (Village village in Village.All.Where((Village x) => x.TradeBound == currentSettlement))
			{
				foreach (ValueTuple<ItemObject, float> valueTuple in village.VillageType.Productions)
				{
					ItemCategory itemCategory = valueTuple.Item1.ItemCategory;
					if (itemCategory != DefaultItemCategories.Grain || village.VillageType.PrimaryProduction == DefaultItems.Grain)
					{
						float item = valueTuple.Item2;
						if (itemCategory == DefaultItemCategories.Cow || itemCategory == DefaultItemCategories.Hog)
						{
							itemCategory = DefaultItemCategories.Hides;
						}
						if (itemCategory == DefaultItemCategories.Sheep)
						{
							itemCategory = DefaultItemCategories.Wool;
						}
						float num;
						if (dictionary.TryGetValue(itemCategory, out num))
						{
							dictionary[itemCategory] = num + item;
						}
						else
						{
							dictionary.Add(itemCategory, item);
						}
					}
				}
			}
			Dictionary<WorkshopType, float> dictionary2 = new Dictionary<WorkshopType, float>();
			float num2 = 0f;
			foreach (WorkshopType workshopType in WorkshopType.All)
			{
				if (!workshopType.IsHidden && (workshopToExclude == null || workshopToExclude != workshopType))
				{
					float num3 = this.FindTotalInputDensityScore(currentSettlement, workshopType, dictionary, atGameStart);
					dictionary2.Add(workshopType, num3);
					num2 += num3;
				}
			}
			float num4 = num2 * MBRandom.RandomFloat;
			WorkshopType workshopType2 = null;
			foreach (WorkshopType workshopType3 in WorkshopType.All)
			{
				if (!workshopType3.IsHidden && (workshopToExclude == null || workshopToExclude != workshopType3))
				{
					num4 -= dictionary2[workshopType3];
					if (num4 < 0f)
					{
						workshopType2 = workshopType3;
						break;
					}
				}
			}
			if (workshopType2 == null)
			{
				workshopType2 = WorkshopType.All[MBRandom.RandomInt(1, WorkshopType.All.Count)];
			}
			return workshopType2;
		}

		private void InitializeWorkshops()
		{
			foreach (Town town in Town.AllTowns)
			{
				town.InitializeWorkshops(4);
			}
		}

		private void BuildWorkshopsAtGameStart()
		{
			foreach (Town town in Town.AllTowns)
			{
				this.BuildArtisanWorkshop(town);
				for (int i = 1; i < town.Workshops.Length; i++)
				{
					Hero hero = this.SelectRandomOwner(town);
					this.BuildWorkshopForHeroAtGameStart(hero);
				}
			}
		}

		private void BuildArtisanWorkshop(Town town)
		{
			Hero hero = town.Settlement.Notables.FirstOrDefault((Hero x) => x.IsArtisan);
			if (hero == null)
			{
				hero = town.Settlement.Notables.FirstOrDefault<Hero>();
			}
			if (hero != null)
			{
				WorkshopType workshopType = WorkshopType.Find("artisans");
				town.Workshops[0].SetWorkshop(hero, workshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), false, 0, 1, null);
			}
		}

		private Hero SelectRandomOwner(Town town)
		{
			Hero hero = null;
			Settlement settlement = town.Settlement;
			float num = 0f;
			foreach (Hero hero2 in settlement.Notables)
			{
				int count = hero2.OwnedWorkshops.Count;
				float num2 = hero2.Power / MathF.Pow(10f, (float)count);
				num += num2;
			}
			num *= MBRandom.RandomFloat;
			foreach (Hero hero3 in settlement.Notables)
			{
				int count2 = hero3.OwnedWorkshops.Count;
				float num3 = hero3.Power / MathF.Pow(10f, (float)count2);
				num -= num3;
				if (num < 0f)
				{
					hero = hero3;
					break;
				}
			}
			return hero;
		}

		private void RunTownShopsAtGameStart()
		{
			foreach (Town town in Town.AllTowns)
			{
				foreach (Workshop workshop in town.Workshops)
				{
					this.RunTownWorkshop(town, workshop, true);
				}
			}
		}

		private void RunTownWorkshop(Town townComponent, Workshop workshop, bool willBeSold = true)
		{
			if (workshop.IsRunning && !this._playerOwnedWorkshopsDaysInBankruptcy.ContainsKey(workshop))
			{
				WorkshopType workshopType = workshop.WorkshopType;
				bool flag = false;
				bool flag2 = false;
				for (int i = 0; i < workshopType.Productions.Count; i++)
				{
					float num = workshop.GetProductionProgress(i);
					if (num > 1f)
					{
						num = 1f;
					}
					float policyEffectToProduction = Campaign.Current.Models.WorkshopModel.GetPolicyEffectToProduction(townComponent);
					ExplainedNumber explainedNumber = new ExplainedNumber(workshopType.Productions[i].ConversionSpeed * policyEffectToProduction, false, null);
					if (townComponent.Governor != null && townComponent.Governor.GetPerkValue(DefaultPerks.Trade.MercenaryConnections))
					{
						PerkHelper.AddPerkBonusForTown(DefaultPerks.Trade.MercenaryConnections, townComponent, ref explainedNumber);
					}
					if (workshop.Owner != null)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Steward.Sweatshops, workshop.Owner.CharacterObject, true, ref explainedNumber);
					}
					num += explainedNumber.ResultNumber;
					if (num >= 1f)
					{
						bool flag3 = true;
						bool flag4 = true;
						while (flag4 && num >= 1f)
						{
							flag4 = this.DoProduction(workshopType.Productions[i], workshop, townComponent);
							if (!flag4 && flag3)
							{
								flag = true;
							}
							else if (flag4)
							{
								flag2 = true;
							}
							num -= 1f;
						}
					}
					workshop.SetProgress(i, num);
				}
				if (flag2)
				{
					workshop.ResetNotRunnedDays();
					return;
				}
				if (flag)
				{
					workshop.IncreaseNotRunnedDays();
				}
			}
		}

		private static bool DetermineTownHasSufficientInputs(WorkshopType.Production production, Town town, out int inputMaterialCost)
		{
			List<ValueTuple<ItemCategory, int>> inputs = production.Inputs;
			inputMaterialCost = 0;
			foreach (ValueTuple<ItemCategory, int> valueTuple in inputs)
			{
				ItemCategory item = valueTuple.Item1;
				int num = valueTuple.Item2;
				ItemRoster itemRoster = town.Owner.ItemRoster;
				for (int i = 0; i < itemRoster.Count; i++)
				{
					ItemObject itemAtIndex = itemRoster.GetItemAtIndex(i);
					if (itemAtIndex.ItemCategory == item)
					{
						int elementNumber = itemRoster.GetElementNumber(i);
						int num2 = MathF.Min(num, elementNumber);
						num -= num2;
						inputMaterialCost += town.GetItemPrice(itemAtIndex, null, false) * num2;
					}
				}
				if (num > 0)
				{
					return false;
				}
			}
			return true;
		}

		private bool DoProduction(WorkshopType.Production production, Workshop workshop, Town town)
		{
			List<ValueTuple<EquipmentElement, int>> list = new List<ValueTuple<EquipmentElement, int>>();
			MBReadOnlyList<ValueTuple<ItemCategory, int>> inputs = production.Inputs;
			int num;
			if (!WorkshopsCampaignBehavior.DetermineTownHasSufficientInputs(production, town, out num))
			{
				return false;
			}
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < production.Outputs.Count; i++)
			{
				int item = production.Outputs[i].Item2;
				num2 += item;
				for (int j = 0; j < item; j++)
				{
					EquipmentElement randomItem = this.GetRandomItem(production.Outputs[i].Item1, town);
					list.Add(new ValueTuple<EquipmentElement, int>(randomItem, 1));
					num3 += town.GetItemPrice(randomItem, null, true);
				}
			}
			bool flag = false;
			if (workshop.WorkshopType.Productions.Count > 1)
			{
				foreach (ValueTuple<ItemCategory, int> valueTuple in production.Inputs)
				{
					if (valueTuple.Item1 != null && !valueTuple.Item1.IsTradeGood)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					foreach (ValueTuple<ItemCategory, int> valueTuple2 in production.Outputs)
					{
						if (valueTuple2.Item1 != null && !valueTuple2.Item1.IsTradeGood)
						{
							flag = true;
							break;
						}
					}
				}
			}
			float num4 = (workshop.WorkshopType.IsHidden ? ((float)num) : ((float)num + 200f / production.ConversionSpeed));
			if ((Campaign.Current.GameStarted && (float)num3 <= num4) || (num3 > town.Gold && !flag) || num > workshop.Capital)
			{
				return false;
			}
			if (list.Sum((ValueTuple<EquipmentElement, int> t) => t.Item2) == num2)
			{
				foreach (ValueTuple<EquipmentElement, int> valueTuple3 in list)
				{
					WorkshopsCampaignBehavior.ProduceOutput(valueTuple3.Item1, town, workshop, valueTuple3.Item2, flag);
				}
				foreach (ValueTuple<ItemCategory, int> valueTuple4 in inputs)
				{
					WorkshopsCampaignBehavior.ConsumeInput(valueTuple4.Item1, town, workshop, flag);
				}
				return true;
			}
			return false;
		}

		private static void ProduceOutput(EquipmentElement outputItem, Town town, Workshop workshop, int count, bool doNotEffectCapital)
		{
			int itemPrice = town.GetItemPrice(outputItem, null, false);
			town.Owner.ItemRoster.AddToCounts(outputItem, count);
			if (Campaign.Current.GameStarted && !doNotEffectCapital)
			{
				int num = MathF.Min(1000, itemPrice) * count;
				workshop.ChangeGold(num);
				town.ChangeGold(-num);
			}
			CampaignEventDispatcher.Instance.OnItemProduced(outputItem.Item, town.Owner.Settlement, count);
		}

		private static void ConsumeInput(ItemCategory productionInput, Town town, Workshop workshop, bool doNotEffectCapital)
		{
			ItemRoster itemRoster = town.Owner.ItemRoster;
			int num = itemRoster.FindIndex((ItemObject x) => x.ItemCategory == productionInput);
			if (num >= 0)
			{
				ItemObject itemAtIndex = itemRoster.GetItemAtIndex(num);
				itemRoster.AddToCounts(itemAtIndex, -1);
				if (Campaign.Current.GameStarted && !doNotEffectCapital)
				{
					int itemPrice = town.GetItemPrice(itemAtIndex, null, false);
					workshop.ChangeGold(-itemPrice);
					town.ChangeGold(itemPrice);
				}
				CampaignEventDispatcher.Instance.OnItemConsumed(itemAtIndex, town.Owner.Settlement, 1);
			}
		}

		private bool IsItemPreferredForTown(ItemObject item, Town townComponent)
		{
			return item.Culture == null || item.Culture.StringId == "neutral_culture" || item.Culture == townComponent.Culture;
		}

		private const string TransactionStringID = "str_workshop_profits";

		private const int WorkShopCount = 4;

		private readonly Dictionary<ItemCategory, List<ItemObject>> _itemsInCategory = new Dictionary<ItemCategory, List<ItemObject>>();

		private Dictionary<Workshop, int> _playerOwnedWorkshopsDaysInBankruptcy = new Dictionary<Workshop, int>();
	}
}
