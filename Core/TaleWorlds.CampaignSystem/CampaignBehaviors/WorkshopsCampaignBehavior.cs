using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class WorkshopsCampaignBehavior : CampaignBehaviorBase, IWorkshopWarehouseCampaignBehavior
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, new Action<Town>(this.DailyTickTown));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.WorkshopOwnerChangedEvent.AddNonSerializedListener(this, new Action<Workshop, Hero>(this.OnWorkshopOwnerChanged));
			CampaignEvents.WorkshopTypeChangedEvent.AddNonSerializedListener(this, new Action<Workshop>(this.OnWorkshopTypeChanged));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<KeyValuePair<Settlement, ItemRoster>[]>("_warehouseRosterPerSettlement", ref this._warehouseRosterPerSettlement);
			dataStore.SyncData<WorkshopsCampaignBehavior.WorkshopData[]>("_workshopData", ref this._workshopData);
		}

		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int i)
		{
			if (i >= 10)
			{
				if (i == 10)
				{
					this.InitializeBehaviorData();
					this.FillItemsInAllCategories();
					this.InitializeWorkshops();
					this.BuildWorkshopsAtGameStart();
				}
				if (i % 20 == 0)
				{
					this.RunTownShopsAtGameStart();
				}
			}
		}

		private void InitializeBehaviorData()
		{
			if (this._workshopData == null)
			{
				this._workshopData = new WorkshopsCampaignBehavior.WorkshopData[Campaign.Current.Models.WorkshopModel.MaximumWorkshopsPlayerCanHave];
			}
			if (this._warehouseRosterPerSettlement == null)
			{
				this._warehouseRosterPerSettlement = new KeyValuePair<Settlement, ItemRoster>[Campaign.Current.Models.WorkshopModel.MaximumWorkshopsPlayerCanHave];
			}
		}

		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this.InitializeBehaviorData();
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.0", 27066))
			{
				foreach (Workshop workshop in Hero.MainHero.OwnedWorkshops)
				{
					this.AddNewWorkshopData(workshop);
					this.AddNewWarehouseDataIfNeeded(workshop.Settlement);
				}
			}
			this.EnsureBehaviorDataSize();
			this.FillItemsInAllCategories();
		}

		private void EnsureBehaviorDataSize()
		{
			if (this._workshopData.Length < Campaign.Current.Models.WorkshopModel.MaximumWorkshopsPlayerCanHave)
			{
				WorkshopsCampaignBehavior.WorkshopData[] array = new WorkshopsCampaignBehavior.WorkshopData[Campaign.Current.Models.WorkshopModel.MaximumWorkshopsPlayerCanHave];
				for (int i = 0; i < Campaign.Current.Models.WorkshopModel.MaximumWorkshopsPlayerCanHave; i++)
				{
					if (i < this._workshopData.Length)
					{
						array[i] = this._workshopData[i];
					}
					else
					{
						array[i] = null;
					}
				}
				this._workshopData = array;
			}
			if (this._warehouseRosterPerSettlement.Length < Campaign.Current.Models.WorkshopModel.MaximumWorkshopsPlayerCanHave)
			{
				KeyValuePair<Settlement, ItemRoster>[] array2 = new KeyValuePair<Settlement, ItemRoster>[Campaign.Current.Models.WorkshopModel.MaximumWorkshopsPlayerCanHave];
				for (int j = 0; j < Campaign.Current.Models.WorkshopModel.MaximumWorkshopsPlayerCanHave; j++)
				{
					if (j < this._warehouseRosterPerSettlement.Length && this._warehouseRosterPerSettlement[j].Key != null && this._warehouseRosterPerSettlement[j].Value != null)
					{
						array2[j] = this._warehouseRosterPerSettlement[j];
					}
				}
				this._warehouseRosterPerSettlement = array2;
			}
		}

		private void OnWorkshopTypeChanged(Workshop workshop)
		{
			if (workshop.Owner == Hero.MainHero)
			{
				this.RemoveWorkshopData(workshop);
				this.AddNewWorkshopData(workshop);
			}
		}

		private void OnWorkshopOwnerChanged(Workshop workshop, Hero oldOwner)
		{
			Hero owner = workshop.Owner;
			if (owner == Hero.MainHero)
			{
				this.AddNewWarehouseDataIfNeeded(workshop.Settlement);
				this.AddNewWorkshopData(workshop);
				return;
			}
			if (oldOwner == Hero.MainHero && Clan.PlayerClan.Leader != owner)
			{
				if (Hero.MainHero.OwnedWorkshops.All((Workshop x) => x.Settlement != workshop.Settlement))
				{
					if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement == workshop.Settlement)
					{
						this.TransferWarehouseToPlayerParty(Settlement.CurrentSettlement);
					}
					this.RemoveWarehouseData(workshop.Settlement);
				}
				this.RemoveWorkshopData(workshop);
			}
		}

		private void DailyTickTown(Town town)
		{
			if (!town.InRebelliousState)
			{
				foreach (Workshop workshop in town.Workshops)
				{
					this.RunTownWorkshop(town, workshop);
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
					Hero notableOwnerForWorkshop = Campaign.Current.Models.WorkshopModel.GetNotableOwnerForWorkshop(workshop.Settlement);
					ChangeOwnerOfWorkshopAction.ApplyByDeath(workshop, notableOwnerForWorkshop);
				}
			}
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			this.TransferPlayerWorkshopsIfNeeded();
		}

		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
		{
			this.TransferPlayerWorkshopsIfNeeded();
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
				this.TransferPlayerWorkshopsIfNeeded();
			}
		}

		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.InitializeGameMenus(campaignGameStarter);
		}

		protected void InitializeGameMenus(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddGameMenuOption("town", "manage_warehouse", "{=LK4kNZkb}Enter the warehouse", new GameMenuOption.OnConditionDelegate(this.warehouse_manage_on_condition), new GameMenuOption.OnConsequenceDelegate(this.warehouse_manage_on_consequence), false, 8, false, null);
			campaignGameStarter.AddPlayerLine("workshop_worker_manage_warehouse", "player_options", "warehouse", "{=mBnoWa8R}I would like to access the Warehouse.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_worker_manage_warehouse_answer", "warehouse", "player_options", "{=Y4LhmAdi}Sure, boss. Go ahead.", null, new ConversationSentence.OnConsequenceDelegate(this.warehouse_manage_on_consequence), 100, null);
		}

		private void warehouse_manage_on_consequence()
		{
			InventoryLogic.CapacityData capacityData = new InventoryLogic.CapacityData(new Func<int>(WorkshopsCampaignBehavior.<>c.<>9.<warehouse_manage_on_consequence>g__CapacityDelegate|20_0), new Func<TextObject>(WorkshopsCampaignBehavior.<>c.<>9.<warehouse_manage_on_consequence>g__CapacityExceededWarningDelegate|20_1), new Func<TextObject>(WorkshopsCampaignBehavior.<>c.<>9.<warehouse_manage_on_consequence>g__CapacityExceededHintDelegate|20_2));
			InventoryManager.OpenScreenAsWarehouse(this.GetWarehouseRoster(Settlement.CurrentSettlement), capacityData);
			Campaign.Current.ConversationManager.ContinueConversation();
		}

		private void warehouse_manage_on_consequence(MenuCallbackArgs args)
		{
			InventoryLogic.CapacityData capacityData = new InventoryLogic.CapacityData(new Func<int>(WorkshopsCampaignBehavior.<>c.<>9.<warehouse_manage_on_consequence>g__CapacityDelegate|21_0), new Func<TextObject>(WorkshopsCampaignBehavior.<>c.<>9.<warehouse_manage_on_consequence>g__CapacityExceededWarningDelegate|21_1), new Func<TextObject>(WorkshopsCampaignBehavior.<>c.<>9.<warehouse_manage_on_consequence>g__CapacityExceededHintDelegate|21_2));
			InventoryManager.OpenScreenAsWarehouse(this.GetWarehouseRoster(Settlement.CurrentSettlement), capacityData);
		}

		private bool warehouse_manage_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Warehouse;
			return this.GetWarehouseRoster(Settlement.CurrentSettlement) != null;
		}

		bool IWorkshopWarehouseCampaignBehavior.IsGettingInputsFromWarehouse(Workshop workshop)
		{
			WorkshopsCampaignBehavior.WorkshopData dataOfWorkshop = this.GetDataOfWorkshop(workshop);
			return dataOfWorkshop != null && dataOfWorkshop.IsGettingInputsFromWarehouse;
		}

		void IWorkshopWarehouseCampaignBehavior.SetIsGettingInputsFromWarehouse(Workshop workshop, bool isActive)
		{
			WorkshopsCampaignBehavior.WorkshopData dataOfWorkshop = this.GetDataOfWorkshop(workshop);
			if (dataOfWorkshop != null)
			{
				dataOfWorkshop.IsGettingInputsFromWarehouse = isActive;
			}
		}

		float IWorkshopWarehouseCampaignBehavior.GetStockProductionInWarehouseRatio(Workshop workshop)
		{
			WorkshopsCampaignBehavior.WorkshopData dataOfWorkshop = this.GetDataOfWorkshop(workshop);
			if (dataOfWorkshop != null)
			{
				return dataOfWorkshop.StockProductionInWarehouseRatio;
			}
			return 0f;
		}

		void IWorkshopWarehouseCampaignBehavior.SetStockProductionInWarehouseRatio(Workshop workshop, float ratio)
		{
			WorkshopsCampaignBehavior.WorkshopData dataOfWorkshop = this.GetDataOfWorkshop(workshop);
			if (dataOfWorkshop != null)
			{
				dataOfWorkshop.StockProductionInWarehouseRatio = ratio;
			}
		}

		int IWorkshopWarehouseCampaignBehavior.GetInputCount(Workshop workshop)
		{
			ItemRoster warehouseRoster = this.GetWarehouseRoster(workshop.Settlement);
			int num = 0;
			List<ItemCategory> list = new List<ItemCategory>();
			foreach (WorkshopType.Production production in workshop.WorkshopType.Productions)
			{
				foreach (ValueTuple<ItemCategory, int> valueTuple in production.Inputs)
				{
					ItemCategory item = valueTuple.Item1;
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			foreach (ItemCategory itemCategory in list)
			{
				foreach (ItemRosterElement itemRosterElement in warehouseRoster)
				{
					if (itemRosterElement.EquipmentElement.Item.ItemCategory == itemCategory)
					{
						num += itemRosterElement.Amount;
						break;
					}
				}
			}
			return num;
		}

		ExplainedNumber IWorkshopWarehouseCampaignBehavior.GetInputDailyChange(Workshop workshop)
		{
			ItemRoster warehouseRoster = this.GetWarehouseRoster(workshop.Settlement);
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, true, null);
			Dictionary<ItemCategory, float> dictionary = new Dictionary<ItemCategory, float>();
			foreach (WorkshopType.Production production in workshop.WorkshopType.Productions)
			{
				foreach (ValueTuple<ItemCategory, int> valueTuple in production.Inputs)
				{
					float num = Campaign.Current.Models.WorkshopModel.GetEffectiveConversionSpeedOfProduction(workshop, production.ConversionSpeed, false).ResultNumber * (float)valueTuple.Item2;
					ItemCategory item = valueTuple.Item1;
					float num2;
					if (!dictionary.TryGetValue(item, out num2))
					{
						dictionary.Add(item, num);
					}
					else
					{
						dictionary[item] = num2 + num;
					}
				}
			}
			foreach (KeyValuePair<ItemCategory, float> keyValuePair in dictionary)
			{
				int num3 = 0;
				foreach (ItemRosterElement itemRosterElement in warehouseRoster)
				{
					if (itemRosterElement.EquipmentElement.Item.ItemCategory == keyValuePair.Key)
					{
						num3 += itemRosterElement.Amount;
					}
				}
				if (num3 > 0)
				{
					TextObject textObject = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null);
					textObject.SetTextVariable("RANK", keyValuePair.Key.GetName());
					textObject.SetTextVariable("NUMBER", num3);
					explainedNumber.Add(-keyValuePair.Value, textObject, null);
				}
			}
			return explainedNumber;
		}

		int IWorkshopWarehouseCampaignBehavior.GetOutputCount(Workshop workshop)
		{
			ItemRoster warehouseRoster = this.GetWarehouseRoster(workshop.Settlement);
			int num = 0;
			List<ItemCategory> list = new List<ItemCategory>();
			foreach (WorkshopType.Production production in workshop.WorkshopType.Productions)
			{
				foreach (ValueTuple<ItemCategory, int> valueTuple in production.Outputs)
				{
					ItemCategory item = valueTuple.Item1;
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			foreach (ItemCategory itemCategory in list)
			{
				foreach (ItemRosterElement itemRosterElement in warehouseRoster)
				{
					if (itemRosterElement.EquipmentElement.Item.ItemCategory == itemCategory)
					{
						num += itemRosterElement.Amount;
						break;
					}
				}
			}
			return num;
		}

		ExplainedNumber IWorkshopWarehouseCampaignBehavior.GetOutputDailyChange(Workshop workshop)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, true, null);
			ItemRoster warehouseRoster = this.GetWarehouseRoster(workshop.Settlement);
			Dictionary<ItemCategory, float> dictionary = new Dictionary<ItemCategory, float>();
			foreach (WorkshopType.Production production in workshop.WorkshopType.Productions)
			{
				foreach (ValueTuple<ItemCategory, int> valueTuple in production.Outputs)
				{
					ItemCategory item = valueTuple.Item1;
					if (item.IsTradeGood)
					{
						int item2 = valueTuple.Item2;
						float num = Campaign.Current.Models.WorkshopModel.GetEffectiveConversionSpeedOfProduction(workshop, production.ConversionSpeed, false).ResultNumber * (float)item2 * this.GetDataOfWorkshop(workshop).StockProductionInWarehouseRatio;
						float num2;
						if (!dictionary.TryGetValue(item, out num2))
						{
							dictionary.Add(item, num);
						}
						else
						{
							dictionary[item] = num2 + num;
						}
					}
				}
			}
			foreach (KeyValuePair<ItemCategory, float> keyValuePair in dictionary)
			{
				int num3 = 0;
				foreach (ItemRosterElement itemRosterElement in warehouseRoster)
				{
					if (itemRosterElement.EquipmentElement.Item.ItemCategory == keyValuePair.Key)
					{
						num3 += itemRosterElement.Amount;
					}
				}
				TextObject textObject = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null);
				textObject.SetTextVariable("RANK", keyValuePair.Key.GetName());
				textObject.SetTextVariable("NUMBER", num3);
				explainedNumber.Add(keyValuePair.Value, textObject, null);
			}
			return explainedNumber;
		}

		bool IWorkshopWarehouseCampaignBehavior.IsRawMaterialsSufficientInTownMarket(Workshop workshop)
		{
			for (int i = 0; i < workshop.WorkshopType.Productions.Count; i++)
			{
				WorkshopType.Production production = workshop.WorkshopType.Productions[i];
				int num;
				if (this.DetermineItemRosterHasSufficientInputs(production, workshop.Settlement.Town.Owner.ItemRoster, workshop.Settlement.Town, out num))
				{
					return true;
				}
			}
			return false;
		}

		public float GetWarehouseItemRosterWeight(Settlement settlement)
		{
			ItemRoster warehouseRoster = this.GetWarehouseRoster(settlement);
			float num = 0f;
			foreach (ItemRosterElement itemRosterElement in warehouseRoster)
			{
				num += itemRosterElement.GetRosterElementWeight();
			}
			return num;
		}

		private bool TickOneProductionCycleForPlayerWorkshop(WorkshopType.Production production, Workshop workshop)
		{
			bool flag = false;
			int num = 0;
			Town town = workshop.Settlement.Town;
			WorkshopsCampaignBehavior.WorkshopData dataOfWorkshop = this.GetDataOfWorkshop(workshop);
			bool flag2 = dataOfWorkshop.IsGettingInputsFromWarehouse;
			if (flag2)
			{
				flag = this.DetermineItemRosterHasSufficientInputs(production, this.GetWarehouseRoster(workshop.Settlement), town, out num);
				if (flag)
				{
					num = 0;
				}
				else
				{
					flag2 = false;
				}
			}
			if (!flag)
			{
				flag = this.DetermineItemRosterHasSufficientInputs(production, town.Owner.ItemRoster, town, out num);
			}
			if (flag)
			{
				int num2;
				List<EquipmentElement> itemsToProduce = this.GetItemsToProduce(production, workshop, out num2);
				bool flag3;
				if (!production.Inputs.Any((ValueTuple<ItemCategory, int> x) => !x.Item1.IsTradeGood))
				{
					flag3 = !production.Outputs.Any((ValueTuple<ItemCategory, int> x) => !x.Item1.IsTradeGood);
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				float num3 = dataOfWorkshop.StockProductionInWarehouseRatio;
				bool flag5 = num3.ApproximatelyEqualsTo(1f, 1E-05f);
				if (this.CanPlayerWorkshopProduceThisCycle(production, workshop, num, num2, flag4, flag5))
				{
					Dictionary<ItemObject, int> dictionary = new Dictionary<ItemObject, int>();
					foreach (ValueTuple<ItemCategory, int> valueTuple in production.Inputs)
					{
						if (flag2)
						{
							this.ConsumeInputFromWarehouse(valueTuple.Item1, valueTuple.Item2, workshop);
						}
						else
						{
							this.ConsumeInputFromTownMarket(valueTuple.Item1, valueTuple.Item2, town, workshop, flag4);
						}
					}
					foreach (EquipmentElement equipmentElement in itemsToProduce)
					{
						WorkshopsCampaignBehavior.WorkshopData dataOfWorkshop2 = this.GetDataOfWorkshop(workshop);
						if (equipmentElement.Item.IsTradeGood && this.CanItemFitInWarehouse(workshop.Settlement, equipmentElement))
						{
							this.AddOutputProgressForWarehouse(workshop, num3);
							if (dataOfWorkshop2.ProductionProgressForWarehouse >= 1f)
							{
								this.ProduceAnOutputToWarehouse(equipmentElement, workshop);
								SkillLevelingManager.OnProductionProducedToWarehouse(equipmentElement);
								this.AddOutputProgressForWarehouse(workshop, -1f);
								if (dictionary.ContainsKey(equipmentElement.Item))
								{
									Dictionary<ItemObject, int> dictionary2 = dictionary;
									ItemObject itemObject = equipmentElement.Item;
									int num4 = dictionary2[itemObject];
									dictionary2[itemObject] = num4 + 1;
								}
								else
								{
									dictionary.Add(equipmentElement.Item, 1);
								}
							}
						}
						else
						{
							num3 = 0f;
						}
						this.AddOutputProgressForTown(workshop, 1f - num3);
						if (dataOfWorkshop2.ProductionProgressForTown >= 1f)
						{
							this.ProduceAnOutputToTown(equipmentElement, workshop, flag4);
							this.AddOutputProgressForTown(workshop, -1f);
							if (dictionary.ContainsKey(equipmentElement.Item))
							{
								Dictionary<ItemObject, int> dictionary3 = dictionary;
								ItemObject itemObject = equipmentElement.Item;
								int num4 = dictionary3[itemObject];
								dictionary3[itemObject] = num4 + 1;
							}
							else
							{
								dictionary.Add(equipmentElement.Item, 1);
							}
						}
					}
					foreach (KeyValuePair<ItemObject, int> keyValuePair in dictionary)
					{
						ItemObject key = keyValuePair.Key;
						int value = keyValuePair.Value;
						CampaignEventDispatcher.Instance.OnItemProduced(key, workshop.Settlement, value);
					}
					return true;
				}
			}
			return false;
		}

		private void ProduceAnOutputToWarehouse(EquipmentElement outputItem, Workshop workshop)
		{
			this.GetWarehouseRoster(workshop.Settlement).AddToCounts(outputItem, 1);
		}

		private void ConsumeInputFromWarehouse(ItemCategory productionInput, int inputCount, Workshop workshop)
		{
			ItemRoster warehouseRoster = this.GetWarehouseRoster(workshop.Settlement);
			int num = inputCount;
			for (int i = 0; i < warehouseRoster.Count; i++)
			{
				if (num == 0)
				{
					return;
				}
				ItemObject itemAtIndex = warehouseRoster.GetItemAtIndex(i);
				if (itemAtIndex.ItemCategory == productionInput)
				{
					int elementNumber = warehouseRoster.GetElementNumber(i);
					int num2 = MathF.Min(num, elementNumber);
					num -= num2;
					warehouseRoster.AddToCounts(itemAtIndex, -inputCount);
					CampaignEventDispatcher.Instance.OnItemConsumed(itemAtIndex, workshop.Settlement, inputCount);
				}
			}
		}

		private bool CanPlayerWorkshopProduceThisCycle(WorkshopType.Production production, Workshop workshop, int inputMaterialCost, int outputIncome, bool effectCapital, bool allOutputsWillBeSentToWarehouse)
		{
			float num = (workshop.WorkshopType.IsHidden ? ((float)inputMaterialCost) : ((float)inputMaterialCost + 200f / production.ConversionSpeed));
			if (Campaign.Current.GameStarted && (float)outputIncome <= num)
			{
				return false;
			}
			if (workshop.Capital < inputMaterialCost)
			{
				return false;
			}
			if (effectCapital)
			{
				bool flag = workshop.Settlement.Town.Gold >= outputIncome;
				bool flag2 = !this.IsWarehouseAtLimit(workshop.Settlement);
				if (!flag && (!allOutputsWillBeSentToWarehouse || flag2))
				{
					return false;
				}
			}
			return true;
		}

		private void HandlePlayerWorkshopExpense(Workshop shop)
		{
			int expense = shop.Expense;
			if (shop.Capital > Campaign.Current.Models.WorkshopModel.CapitalLowLimit)
			{
				shop.ChangeGold(-expense);
				return;
			}
			if (shop.Owner.Gold >= expense)
			{
				shop.Owner.Gold -= expense;
				return;
			}
			if (shop.Capital >= expense)
			{
				shop.ChangeGold(-expense);
				return;
			}
			this.ChangeWorkshopOwnerByBankruptcy(shop);
		}

		private bool TickOneProductionCycleForNotableWorkshop(WorkshopType.Production production, Workshop workshop)
		{
			Town town = workshop.Settlement.Town;
			int num = 0;
			if (!this.DetermineItemRosterHasSufficientInputs(production, town.Owner.ItemRoster, town, out num))
			{
				return false;
			}
			int num2;
			List<EquipmentElement> itemsToProduce = this.GetItemsToProduce(production, workshop, out num2);
			bool flag;
			if (!production.Inputs.Any((ValueTuple<ItemCategory, int> x) => !x.Item1.IsTradeGood))
			{
				flag = !production.Outputs.Any((ValueTuple<ItemCategory, int> x) => !x.Item1.IsTradeGood);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (this.CanNotableWorkshopProduceThisCycle(production, workshop, num, num2, flag2))
			{
				foreach (ValueTuple<ItemCategory, int> valueTuple in production.Inputs)
				{
					this.ConsumeInputFromTownMarket(valueTuple.Item1, valueTuple.Item2, town, workshop, flag2);
				}
				foreach (EquipmentElement equipmentElement in itemsToProduce)
				{
					this.ProduceAnOutputToTown(equipmentElement, workshop, flag2);
					CampaignEventDispatcher.Instance.OnItemProduced(equipmentElement.Item, workshop.Settlement, 1);
				}
				return true;
			}
			return false;
		}

		private bool CanNotableWorkshopProduceThisCycle(WorkshopType.Production production, Workshop workshop, int inputMaterialCost, int outputIncome, bool effectCapital)
		{
			float num = (workshop.WorkshopType.IsHidden ? ((float)inputMaterialCost) : ((float)inputMaterialCost + 200f / production.ConversionSpeed));
			return (!Campaign.Current.GameStarted || (float)outputIncome > num) && (workshop.Settlement.Town.Gold >= outputIncome || !effectCapital) && workshop.Capital >= inputMaterialCost;
		}

		private void HandleNotableWorkshopExpense(Workshop shop)
		{
			int expense = shop.Expense;
			if (shop.Capital >= expense)
			{
				shop.ChangeGold(-expense);
				return;
			}
			this.ChangeWorkshopOwnerByBankruptcy(shop);
		}

		private WorkshopsCampaignBehavior.WorkshopData GetDataOfWorkshop(Workshop workshop)
		{
			for (int i = 0; i < this._workshopData.Length; i++)
			{
				WorkshopsCampaignBehavior.WorkshopData workshopData = this._workshopData[i];
				if (workshopData != null && workshopData.Workshop == workshop)
				{
					return workshopData;
				}
			}
			return null;
		}

		private List<EquipmentElement> GetItemsToProduce(WorkshopType.Production production, Workshop workshop, out int income)
		{
			List<EquipmentElement> list = new List<EquipmentElement>();
			income = 0;
			for (int i = 0; i < production.Outputs.Count; i++)
			{
				int item = production.Outputs[i].Item2;
				for (int j = 0; j < item; j++)
				{
					EquipmentElement randomItem = this.GetRandomItem(production.Outputs[i].Item1, workshop.Settlement.Town);
					list.Add(randomItem);
					income += workshop.Settlement.Town.GetItemPrice(randomItem, null, true);
				}
			}
			return list;
		}

		private void ProduceAnOutputToTown(EquipmentElement outputItem, Workshop workshop, bool effectCapital)
		{
			Town town = workshop.Settlement.Town;
			int itemPrice = town.GetItemPrice(outputItem, null, false);
			town.Owner.ItemRoster.AddToCounts(outputItem, 1);
			if (Campaign.Current.GameStarted && effectCapital)
			{
				int num = MathF.Min(1000, itemPrice);
				workshop.ChangeGold(num);
				town.ChangeGold(-num);
			}
		}

		private void ConsumeInputFromTownMarket(ItemCategory productionInput, int productionInputCount, Town town, Workshop workshop, bool effectCapital)
		{
			ItemRoster itemRoster = town.Owner.ItemRoster;
			int num = itemRoster.FindIndex((ItemObject x) => x.ItemCategory == productionInput);
			if (num >= 0)
			{
				ItemObject itemAtIndex = itemRoster.GetItemAtIndex(num);
				if (Campaign.Current.GameStarted && effectCapital)
				{
					int itemPrice = town.GetItemPrice(itemAtIndex, null, false);
					workshop.ChangeGold(-itemPrice);
					town.ChangeGold(itemPrice);
				}
				itemRoster.AddToCounts(itemAtIndex, -productionInputCount);
				CampaignEventDispatcher.Instance.OnItemConsumed(itemAtIndex, town.Owner.Settlement, productionInputCount);
			}
		}

		private bool IsItemPreferredForTown(ItemObject item, Town townComponent)
		{
			return item.Culture == null || item.Culture.StringId == "neutral_culture" || item.Culture == townComponent.Culture;
		}

		private bool DetermineItemRosterHasSufficientInputs(WorkshopType.Production production, ItemRoster itemRoster, Town town, out int inputMaterialCost)
		{
			List<ValueTuple<ItemCategory, int>> inputs = production.Inputs;
			inputMaterialCost = 0;
			foreach (ValueTuple<ItemCategory, int> valueTuple in inputs)
			{
				ItemCategory item = valueTuple.Item1;
				int num = valueTuple.Item2;
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

		private void AddOutputProgressForWarehouse(Workshop workshop, float progressToAdd)
		{
			this.GetDataOfWorkshop(workshop).ProductionProgressForWarehouse += progressToAdd;
		}

		private void AddOutputProgressForTown(Workshop workshop, float progressToAdd)
		{
			this.GetDataOfWorkshop(workshop).ProductionProgressForTown += progressToAdd;
		}

		private bool CanItemFitInWarehouse(Settlement settlement, EquipmentElement equipmentElement)
		{
			return this.GetWarehouseItemRosterWeight(settlement) + equipmentElement.Weight <= (float)Campaign.Current.Models.WorkshopModel.WarehouseCapacity;
		}

		private bool IsWarehouseAtLimit(Settlement settlement)
		{
			return this.GetWarehouseItemRosterWeight(settlement) >= (float)Campaign.Current.Models.WorkshopModel.WarehouseCapacity;
		}

		private void AddNewWorkshopData(Workshop workshop)
		{
			for (int i = 0; i < this._workshopData.Length; i++)
			{
				if (this._workshopData[i] == null)
				{
					this._workshopData[i] = new WorkshopsCampaignBehavior.WorkshopData(workshop);
					return;
				}
			}
		}

		private void RemoveWorkshopData(Workshop workshop)
		{
			for (int i = 0; i < this._workshopData.Length; i++)
			{
				WorkshopsCampaignBehavior.WorkshopData workshopData = this._workshopData[i];
				if (workshopData != null && workshopData.Workshop == workshop)
				{
					this._workshopData[i] = null;
					return;
				}
			}
		}

		private ItemRoster GetWarehouseRoster(Settlement settlement)
		{
			foreach (KeyValuePair<Settlement, ItemRoster> keyValuePair in this._warehouseRosterPerSettlement)
			{
				if (keyValuePair.Key == settlement)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		private void FillItemsInAllCategories()
		{
			foreach (ItemObject itemObject in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
			{
				if (this.IsProducable(itemObject))
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

		private bool IsProducable(ItemObject item)
		{
			return !item.MultiplayerItem && !item.NotMerchandise && !item.IsCraftedByPlayer;
		}

		private void RemoveWarehouseData(Settlement settlement)
		{
			for (int i = 0; i < this._warehouseRosterPerSettlement.Length; i++)
			{
				if (this._warehouseRosterPerSettlement[i].Key == settlement)
				{
					this._warehouseRosterPerSettlement[i] = new KeyValuePair<Settlement, ItemRoster>(null, null);
					return;
				}
			}
		}

		private void AddNewWarehouseDataIfNeeded(Settlement settlement)
		{
			bool flag = false;
			foreach (KeyValuePair<Settlement, ItemRoster> keyValuePair in this._warehouseRosterPerSettlement)
			{
				if (keyValuePair.Key == settlement)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int j = 0; j < this._warehouseRosterPerSettlement.Length; j++)
				{
					KeyValuePair<Settlement, ItemRoster> keyValuePair2 = this._warehouseRosterPerSettlement[j];
					if (keyValuePair2.Value == null)
					{
						this._warehouseRosterPerSettlement[j] = new KeyValuePair<Settlement, ItemRoster>(settlement, new ItemRoster());
						return;
					}
				}
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
			ItemObject itemObject = null;
			ItemModifier itemModifier = null;
			List<ValueTuple<ItemObject, float>> list = new List<ValueTuple<ItemObject, float>>();
			List<ItemObject> list2;
			if (this._itemsInCategory.TryGetValue(itemGroupBase, out list2))
			{
				foreach (ItemObject itemObject2 in list2)
				{
					if ((townComponent == null || this.IsItemPreferredForTown(itemObject2, townComponent)) && itemObject2.ItemCategory == itemGroupBase)
					{
						float num = 1f / ((float)MathF.Max(100, itemObject2.Value) + 100f);
						list.Add(new ValueTuple<ItemObject, float>(itemObject2, num));
					}
				}
				itemObject = MBRandom.ChooseWeighted<ItemObject>(list);
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

		private void TransferPlayerWorkshopsIfNeeded()
		{
			int count = Hero.MainHero.OwnedWorkshops.Count;
			List<Workshop> list = Hero.MainHero.OwnedWorkshops.ToList<Workshop>();
			for (int i = 0; i < count; i++)
			{
				Workshop workshop = list[i];
				if (workshop.Settlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					Hero notableOwnerForWorkshop = Campaign.Current.Models.WorkshopModel.GetNotableOwnerForWorkshop(workshop.Settlement);
					if (notableOwnerForWorkshop != null)
					{
						WorkshopType workshopType = this.DecideBestWorkshopType(workshop.Settlement, false, workshop.WorkshopType);
						ChangeOwnerOfWorkshopAction.ApplyByWar(workshop, notableOwnerForWorkshop, workshopType);
					}
				}
			}
		}

		private void ChangeWorkshopOwnerByBankruptcy(Workshop workshop)
		{
			int costForNotable = Campaign.Current.Models.WorkshopModel.GetCostForNotable(workshop);
			Hero notableOwnerForWorkshop = Campaign.Current.Models.WorkshopModel.GetNotableOwnerForWorkshop(workshop.Settlement);
			WorkshopType workshopType = this.DecideBestWorkshopType(workshop.Settlement, false, workshop.WorkshopType);
			ChangeOwnerOfWorkshopAction.ApplyByBankruptcy(workshop, notableOwnerForWorkshop, workshopType, costForNotable);
		}

		private void HandleDailyExpense(Workshop shop)
		{
			if (!shop.WorkshopType.IsHidden)
			{
				if (shop.Owner != Hero.MainHero)
				{
					this.HandleNotableWorkshopExpense(shop);
					return;
				}
				this.HandlePlayerWorkshopExpense(shop);
			}
		}

		private float FindTotalInputDensityScore(Settlement settlement, WorkshopType workshop, IDictionary<ItemCategory, float> productionDict, bool atGameStart)
		{
			int num = 0;
			for (int i = 0; i < settlement.Town.Workshops.Length; i++)
			{
				if (settlement.Town.Workshops[i].WorkshopType == workshop)
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
							float priceFactor = settlement.Town.MarketData.GetPriceFactor(item);
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
					InitializeWorkshopAction.ApplyByNewGame(bornSettlement.Town.Workshops[num], ownerHero, workshopType);
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
				town.InitializeWorkshops(Campaign.Current.Models.WorkshopModel.DefaultWorkshopCountInSettlement);
			}
		}

		private void BuildWorkshopsAtGameStart()
		{
			foreach (Town town in Town.AllTowns)
			{
				this.BuildArtisanWorkshop(town);
				for (int i = 1; i < town.Workshops.Length; i++)
				{
					Hero notableOwnerForWorkshop = Campaign.Current.Models.WorkshopModel.GetNotableOwnerForWorkshop(town.Settlement);
					this.BuildWorkshopForHeroAtGameStart(notableOwnerForWorkshop);
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
				town.Workshops[0].InitializeWorkshop(hero, workshopType);
			}
		}

		private void RunTownShopsAtGameStart()
		{
			foreach (Town town in Town.AllTowns)
			{
				foreach (Workshop workshop in town.Workshops)
				{
					this.RunTownWorkshop(town, workshop);
				}
			}
		}

		private void RunTownWorkshop(Town townComponent, Workshop workshop)
		{
			WorkshopType workshopType = workshop.WorkshopType;
			bool flag = false;
			for (int i = 0; i < workshopType.Productions.Count; i++)
			{
				float num = workshop.GetProductionProgress(i);
				if (num > 1f)
				{
					num = 1f;
				}
				num += Campaign.Current.Models.WorkshopModel.GetEffectiveConversionSpeedOfProduction(workshop, workshopType.Productions[i].ConversionSpeed, false).ResultNumber;
				if (num >= 1f)
				{
					bool flag2 = true;
					while (flag2 && num >= 1f)
					{
						flag2 = ((workshop.Owner == Hero.MainHero) ? this.TickOneProductionCycleForPlayerWorkshop(workshopType.Productions[i], workshop) : this.TickOneProductionCycleForNotableWorkshop(workshopType.Productions[i], workshop));
						if (flag2)
						{
							flag = true;
						}
						num -= 1f;
					}
				}
				workshop.SetProgress(i, num);
			}
			if (flag)
			{
				workshop.UpdateLastRunTime();
			}
		}

		public void TransferWarehouseToPlayerParty(Settlement settlement)
		{
			foreach (ItemRosterElement itemRosterElement in this.GetWarehouseRoster(settlement))
			{
				MobileParty.MainParty.ItemRoster.AddToCounts(itemRosterElement.EquipmentElement, itemRosterElement.Amount);
			}
			this.RemoveWarehouseData(settlement);
		}

		private KeyValuePair<Settlement, ItemRoster>[] _warehouseRosterPerSettlement;

		private WorkshopsCampaignBehavior.WorkshopData[] _workshopData;

		private readonly Dictionary<ItemCategory, List<ItemObject>> _itemsInCategory = new Dictionary<ItemCategory, List<ItemObject>>();

		public class WorkshopsCampaignBehaviorTypeDefiner : SaveableTypeDefiner
		{
			public WorkshopsCampaignBehaviorTypeDefiner()
				: base(155828)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(WorkshopsCampaignBehavior.WorkshopData), 10, null);
			}

			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<Workshop, WorkshopsCampaignBehavior.WorkshopData>));
				base.ConstructContainerDefinition(typeof(WorkshopsCampaignBehavior.WorkshopData[]));
			}
		}

		internal class WorkshopData
		{
			public WorkshopData(Workshop workshop)
			{
				this.Workshop = workshop;
			}

			public override string ToString()
			{
				return this.Workshop.WorkshopType.ToString() + " in " + this.Workshop.Settlement.GetName();
			}

			internal static void AutoGeneratedStaticCollectObjectsWorkshopData(object o, List<object> collectedObjects)
			{
				((WorkshopsCampaignBehavior.WorkshopData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.Workshop);
			}

			internal static object AutoGeneratedGetMemberValueWorkshop(object o)
			{
				return ((WorkshopsCampaignBehavior.WorkshopData)o).Workshop;
			}

			internal static object AutoGeneratedGetMemberValueIsGettingInputsFromWarehouse(object o)
			{
				return ((WorkshopsCampaignBehavior.WorkshopData)o).IsGettingInputsFromWarehouse;
			}

			internal static object AutoGeneratedGetMemberValueProductionProgressForWarehouse(object o)
			{
				return ((WorkshopsCampaignBehavior.WorkshopData)o).ProductionProgressForWarehouse;
			}

			internal static object AutoGeneratedGetMemberValueProductionProgressForTown(object o)
			{
				return ((WorkshopsCampaignBehavior.WorkshopData)o).ProductionProgressForTown;
			}

			internal static object AutoGeneratedGetMemberValueStockProductionInWarehouseRatio(object o)
			{
				return ((WorkshopsCampaignBehavior.WorkshopData)o).StockProductionInWarehouseRatio;
			}

			[SaveableField(1)]
			public Workshop Workshop;

			[SaveableField(2)]
			public bool IsGettingInputsFromWarehouse;

			[SaveableField(3)]
			public float ProductionProgressForWarehouse;

			[SaveableField(4)]
			public float ProductionProgressForTown;

			[SaveableField(5)]
			public float StockProductionInWarehouseRatio;
		}
	}
}
