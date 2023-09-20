using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign.Order;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	// Token: 0x020000DA RID: 218
	public class CraftingVM : ViewModel
	{
		// Token: 0x06001412 RID: 5138 RVA: 0x0004C270 File Offset: 0x0004A470
		public CraftingVM(Crafting crafting, Action onClose, Action resetCamera, Action onWeaponCrafted, Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> getItemUsageSetFlags)
		{
			this._crafting = crafting;
			this._onClose = onClose;
			this._resetCamera = resetCamera;
			this._onWeaponCrafted = onWeaponCrafted;
			this._craftingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			this._getItemUsageSetFlags = getItemUsageSetFlags;
			this.AvailableCharactersForSmithing = new MBBindingList<CraftingAvailableHeroItemVM>();
			this.MainActionHint = new BasicTooltipViewModel();
			this.TutorialNotification = new ElementNotificationVM();
			if (Campaign.Current.GameMode == CampaignGameMode.Campaign)
			{
				foreach (Hero hero in CraftingHelper.GetAvailableHeroesForCrafting())
				{
					this.AvailableCharactersForSmithing.Add(new CraftingAvailableHeroItemVM(hero, new Action<CraftingAvailableHeroItemVM>(this.UpdateCraftingHero)));
				}
				this.CurrentCraftingHero = this.AvailableCharactersForSmithing.FirstOrDefault<CraftingAvailableHeroItemVM>();
			}
			else
			{
				this.CurrentCraftingHero = new CraftingAvailableHeroItemVM(Hero.MainHero, new Action<CraftingAvailableHeroItemVM>(this.UpdateCraftingHero));
			}
			this.UpdateCurrentMaterialsAvailable();
			this.Smelting = new SmeltingVM(new Action(this.OnSmeltItemSelection), new Action(this.UpdateAll));
			this.Refinement = new RefinementVM(new Action(this.OnRefinementSelectionChange), new Func<CraftingAvailableHeroItemVM>(this.GetCurrentCraftingHero));
			this.WeaponDesign = new WeaponDesignVM(this._crafting, this._craftingBehavior, new Action(this.OnRequireUpdateFromWeaponDesign), this._onWeaponCrafted, new Func<CraftingAvailableHeroItemVM>(this.GetCurrentCraftingHero), new Action<CraftingOrder>(this.RefreshHeroAvailabilities), this._getItemUsageSetFlags);
			this.ExecuteSwitchToCrafting();
			this.RefreshValues();
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x0004C42C File Offset: 0x0004A62C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			this.CancelLbl = GameTexts.FindText("str_exit", null).ToString();
			this.ResetCameraHint = new HintViewModel(GameTexts.FindText("str_reset_camera", null), null);
			this.CraftingHint = new HintViewModel(GameTexts.FindText("str_crafting", null), null);
			this.RefiningHint = new HintViewModel(GameTexts.FindText("str_refining", null), null);
			this.SmeltingHint = new HintViewModel(GameTexts.FindText("str_smelting", null), null);
			this.RefinementText = GameTexts.FindText("str_crafting_category_refinement", null).ToString();
			this.CraftingText = GameTexts.FindText("str_crafting_category_crafting", null).ToString();
			this.SmeltingText = GameTexts.FindText("str_crafting_category_smelting", null).ToString();
			this.SelectItemToSmeltText = new TextObject("{=rUeWBOOi}Select an item to smelt", null).ToString();
			this.SelectItemToRefineText = new TextObject("{=BqLsZhhr}Select an item to refine", null).ToString();
			this.TutorialNotification.RefreshValues();
			this._availableCharactersForSmithing.ApplyActionOnAllItems(delegate(CraftingAvailableHeroItemVM x)
			{
				x.RefreshValues();
			});
			this._playerCurrentMaterials.ApplyActionOnAllItems(delegate(CraftingResourceItemVM x)
			{
				x.RefreshValues();
			});
			CraftingAvailableHeroItemVM currentCraftingHero = this._currentCraftingHero;
			if (currentCraftingHero == null)
			{
				return;
			}
			currentCraftingHero.RefreshValues();
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x0004C5A4 File Offset: 0x0004A7A4
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.WeaponDesign.OnFinalize();
			InputKeyItemVM confirmInputKey = this.ConfirmInputKey;
			if (confirmInputKey != null)
			{
				confirmInputKey.OnFinalize();
			}
			InputKeyItemVM exitInputKey = this.ExitInputKey;
			if (exitInputKey != null)
			{
				exitInputKey.OnFinalize();
			}
			InputKeyItemVM previousTabInputKey = this.PreviousTabInputKey;
			if (previousTabInputKey != null)
			{
				previousTabInputKey.OnFinalize();
			}
			InputKeyItemVM nextTabInputKey = this.NextTabInputKey;
			if (nextTabInputKey != null)
			{
				nextTabInputKey.OnFinalize();
			}
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			EventManager eventManager = game.EventManager;
			if (eventManager == null)
			{
				return;
			}
			eventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x0004C62B File Offset: 0x0004A82B
		private void OnRequireUpdateFromWeaponDesign()
		{
			CraftingVM.OnItemRefreshedDelegate onItemRefreshed = this.OnItemRefreshed;
			if (onItemRefreshed != null)
			{
				onItemRefreshed(true);
			}
			this.UpdateAll();
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x0004C645 File Offset: 0x0004A845
		public void OnCraftingLogicRefreshed(Crafting newCraftingLogic)
		{
			this._crafting = newCraftingLogic;
			this.WeaponDesign.OnCraftingLogicRefreshed(newCraftingLogic);
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x0004C65C File Offset: 0x0004A85C
		private void UpdateCurrentMaterialCosts()
		{
			for (int i = 0; i < 9; i++)
			{
				this.PlayerCurrentMaterials[i].ResourceAmount = MobileParty.MainParty.ItemRoster.GetItemNumber(this.PlayerCurrentMaterials[i].ResourceItem);
				this.PlayerCurrentMaterials[i].ResourceChangeAmount = 0;
			}
			if (this.IsInSmeltingMode)
			{
				if (this.Smelting.CurrentSelectedItem != null)
				{
					int[] smeltingOutputForItem = Campaign.Current.Models.SmithingModel.GetSmeltingOutputForItem(this.Smelting.CurrentSelectedItem.EquipmentElement.Item);
					for (int j = 0; j < 9; j++)
					{
						this.PlayerCurrentMaterials[j].ResourceChangeAmount = smeltingOutputForItem[j];
					}
					return;
				}
			}
			else
			{
				if (this.IsInRefinementMode)
				{
					RefinementActionItemVM currentSelectedAction = this.Refinement.CurrentSelectedAction;
					if (currentSelectedAction == null)
					{
						return;
					}
					Crafting.RefiningFormula refineFormula = currentSelectedAction.RefineFormula;
					SmithingModel smithingModel = Campaign.Current.Models.SmithingModel;
					for (int k = 0; k < 9; k++)
					{
						this.PlayerCurrentMaterials[k].ResourceChangeAmount = 0;
						if (smithingModel.GetCraftingMaterialItem(refineFormula.Input1) == this.PlayerCurrentMaterials[k].ResourceItem)
						{
							this.PlayerCurrentMaterials[k].ResourceChangeAmount -= refineFormula.Input1Count;
						}
						else if (smithingModel.GetCraftingMaterialItem(refineFormula.Input2) == this.PlayerCurrentMaterials[k].ResourceItem)
						{
							this.PlayerCurrentMaterials[k].ResourceChangeAmount -= refineFormula.Input2Count;
						}
						else if (smithingModel.GetCraftingMaterialItem(refineFormula.Output) == this.PlayerCurrentMaterials[k].ResourceItem)
						{
							this.PlayerCurrentMaterials[k].ResourceChangeAmount += refineFormula.OutputCount;
						}
						else if (smithingModel.GetCraftingMaterialItem(refineFormula.Output2) == this.PlayerCurrentMaterials[k].ResourceItem)
						{
							this.PlayerCurrentMaterials[k].ResourceChangeAmount += refineFormula.Output2Count;
						}
					}
					int[] array = new int[9];
					foreach (CraftingResourceItemVM craftingResourceItemVM in currentSelectedAction.InputMaterials)
					{
						array[(int)craftingResourceItemVM.ResourceMaterial] -= craftingResourceItemVM.ResourceAmount;
					}
					using (IEnumerator<CraftingResourceItemVM> enumerator = currentSelectedAction.OutputMaterials.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CraftingResourceItemVM craftingResourceItemVM2 = enumerator.Current;
							array[(int)craftingResourceItemVM2.ResourceMaterial] += craftingResourceItemVM2.ResourceAmount;
						}
						return;
					}
				}
				int[] smithingCostsForWeaponDesign = Campaign.Current.Models.SmithingModel.GetSmithingCostsForWeaponDesign(this._crafting.CurrentWeaponDesign);
				for (int l = 0; l < 9; l++)
				{
					this.PlayerCurrentMaterials[l].ResourceChangeAmount = smithingCostsForWeaponDesign[l];
				}
			}
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x0004C998 File Offset: 0x0004AB98
		private void UpdateCurrentMaterialsAvailable()
		{
			if (this.PlayerCurrentMaterials == null)
			{
				this.PlayerCurrentMaterials = new MBBindingList<CraftingResourceItemVM>();
				for (int i = 0; i < 9; i++)
				{
					this.PlayerCurrentMaterials.Add(new CraftingResourceItemVM((CraftingMaterials)i, 0, 0));
				}
			}
			for (int j = 0; j < 9; j++)
			{
				ItemObject craftingMaterialItem = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem((CraftingMaterials)j);
				this.PlayerCurrentMaterials[j].ResourceAmount = MobileParty.MainParty.ItemRoster.GetItemNumber(craftingMaterialItem);
			}
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x0004CA1C File Offset: 0x0004AC1C
		private void UpdateAll()
		{
			this.UpdateCurrentMaterialCosts();
			this.UpdateCurrentMaterialsAvailable();
			this.RefreshEnableMainAction();
			this.UpdateCraftingStamina();
			this.UpdateCraftingSkills();
			CraftingOrder craftingOrder;
			if (!this.IsInCraftingMode)
			{
				craftingOrder = null;
			}
			else
			{
				CraftingOrderItemVM activeCraftingOrder = this.WeaponDesign.ActiveCraftingOrder;
				craftingOrder = ((activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder : null);
			}
			this.RefreshHeroAvailabilities(craftingOrder);
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x0004CA70 File Offset: 0x0004AC70
		private void UpdateCraftingSkills()
		{
			foreach (CraftingAvailableHeroItemVM craftingAvailableHeroItemVM in this.AvailableCharactersForSmithing)
			{
				craftingAvailableHeroItemVM.RefreshSkills();
			}
		}

		// Token: 0x0600141B RID: 5147 RVA: 0x0004CABC File Offset: 0x0004ACBC
		private void UpdateCraftingStamina()
		{
			foreach (CraftingAvailableHeroItemVM craftingAvailableHeroItemVM in this.AvailableCharactersForSmithing)
			{
				craftingAvailableHeroItemVM.RefreshStamina();
			}
		}

		// Token: 0x0600141C RID: 5148 RVA: 0x0004CB08 File Offset: 0x0004AD08
		private void RefreshHeroAvailabilities(CraftingOrder order)
		{
			foreach (CraftingAvailableHeroItemVM craftingAvailableHeroItemVM in this.AvailableCharactersForSmithing)
			{
				craftingAvailableHeroItemVM.RefreshOrderAvailability(order);
			}
		}

		// Token: 0x0600141D RID: 5149 RVA: 0x0004CB54 File Offset: 0x0004AD54
		private void RefreshEnableMainAction()
		{
			if (Campaign.Current.GameMode == CampaignGameMode.Tutorial)
			{
				this.IsMainActionEnabled = true;
				return;
			}
			this.IsMainActionEnabled = true;
			if (!this.HaveEnergy())
			{
				this.IsMainActionEnabled = false;
				if (this.MainActionHint != null)
				{
					this.MainActionHint = new BasicTooltipViewModel(() => new TextObject("{=PRE5RKpp}You must rest and spend time before you can do this action.", null).ToString());
				}
			}
			else if (!this.HaveMaterialsNeeded())
			{
				this.IsMainActionEnabled = false;
				if (this.MainActionHint != null)
				{
					this.MainActionHint = new BasicTooltipViewModel(() => new TextObject("{=gduqxfck}You don't have all required materials!", null).ToString());
				}
			}
			if (this.IsInSmeltingMode)
			{
				this.IsMainActionEnabled = this.IsMainActionEnabled && this.Smelting.IsAnyItemSelected;
				this.IsSmeltingItemSelected = this.Smelting.IsAnyItemSelected;
				if (!this.IsSmeltingItemSelected && this.MainActionHint != null)
				{
					this.MainActionHint = new BasicTooltipViewModel(() => new TextObject("{=SzuCFlNq}No item selected.", null).ToString());
					return;
				}
			}
			else if (this.IsInRefinementMode)
			{
				this.IsMainActionEnabled = this.IsMainActionEnabled && this.Refinement.IsValidRefinementActionSelected;
				this.IsRefinementItemSelected = this.Refinement.IsValidRefinementActionSelected;
				if (!this.IsRefinementItemSelected && this.MainActionHint != null)
				{
					this.MainActionHint = new BasicTooltipViewModel(() => new TextObject("{=SzuCFlNq}No item selected.", null).ToString());
					return;
				}
			}
			else
			{
				if (this.WeaponDesign != null)
				{
					if (!this.WeaponDesign.HaveUnlockedAllSelectedPieces())
					{
						this.IsMainActionEnabled = false;
						if (this.MainActionHint != null)
						{
							this.MainActionHint = new BasicTooltipViewModel(() => new TextObject("{=Wir2xZIg}You haven't unlocked some of the selected pieces.", null).ToString());
						}
					}
					else if (!this.WeaponDesign.CanCompleteOrder())
					{
						this.IsMainActionEnabled = false;
						if (this.MainActionHint != null)
						{
							CraftingVM.<>c__DisplayClass20_0 CS$<>8__locals1 = new CraftingVM.<>c__DisplayClass20_0();
							CraftingVM.<>c__DisplayClass20_0 CS$<>8__locals2 = CS$<>8__locals1;
							CraftingOrderItemVM activeCraftingOrder = this.WeaponDesign.ActiveCraftingOrder;
							CS$<>8__locals2.order = ((activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder : null);
							CS$<>8__locals1.item = this._crafting.GetCurrentCraftedItemObject(false, null);
							this.MainActionHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetOrderCannotBeCompletedReasonTooltip(CS$<>8__locals1.order, CS$<>8__locals1.item));
						}
					}
				}
				if (this.IsMainActionEnabled && this.MainActionHint != null)
				{
					this.MainActionHint = new BasicTooltipViewModel();
				}
			}
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x0004CDC7 File Offset: 0x0004AFC7
		private bool HaveEnergy()
		{
			CraftingAvailableHeroItemVM currentCraftingHero = this.CurrentCraftingHero;
			return ((currentCraftingHero != null) ? currentCraftingHero.Hero : null) == null || this._craftingBehavior.GetHeroCraftingStamina(this.CurrentCraftingHero.Hero) > 10;
		}

		// Token: 0x0600141F RID: 5151 RVA: 0x0004CDF9 File Offset: 0x0004AFF9
		private bool HaveMaterialsNeeded()
		{
			return !this.PlayerCurrentMaterials.Any((CraftingResourceItemVM m) => m.ResourceChangeAmount + m.ResourceAmount < 0);
		}

		// Token: 0x06001420 RID: 5152 RVA: 0x0004CE28 File Offset: 0x0004B028
		public void UpdateCraftingHero(CraftingAvailableHeroItemVM newHero)
		{
			this.CurrentCraftingHero = newHero;
			this.WeaponDesign.OnCraftingHeroChanged(newHero);
			this.Refinement.OnCraftingHeroChanged(newHero);
			this.Smelting.OnCraftingHeroChanged(newHero);
			this.RefreshEnableMainAction();
			this.UpdateCraftingSkills();
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x0004CE64 File Offset: 0x0004B064
		public void ExecuteConfirm()
		{
			if (this.WeaponDesign.IsInFinalCraftingStage && this.WeaponDesign.CraftingResultPopup.CanConfirm)
			{
				this.WeaponDesign.CraftingResultPopup.ExecuteFinalizeCrafting();
				return;
			}
			if (this.WeaponDesign.CraftingHistory.IsVisible)
			{
				CraftingHistoryVM craftingHistory = this.WeaponDesign.CraftingHistory;
				if (((craftingHistory != null) ? craftingHistory.SelectedDesign : null) != null)
				{
					this.WeaponDesign.CraftingHistory.ExecuteDone();
					return;
				}
			}
			if (!this.WeaponDesign.CraftingOrderPopup.IsVisible && !this.WeaponDesign.CraftingHistory.IsVisible && !this.WeaponDesign.WeaponClassSelectionPopup.IsVisible && this.IsMainActionEnabled)
			{
				this.ExecuteMainAction();
			}
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x0004CF24 File Offset: 0x0004B124
		public void ExecuteCancel()
		{
			if (this.WeaponDesign.IsInFinalCraftingStage)
			{
				this.WeaponDesign.CraftingResultPopup.ExecuteFinalizeCrafting();
				return;
			}
			if (this.IsCharacterSelectionActive)
			{
				this.IsCharacterSelectionActive = false;
				return;
			}
			if (this.WeaponDesign.CraftingOrderPopup.IsVisible)
			{
				this.WeaponDesign.CraftingOrderPopup.ExecuteCloseWithoutSelection();
				return;
			}
			if (this.WeaponDesign.WeaponClassSelectionPopup.IsVisible)
			{
				this.WeaponDesign.WeaponClassSelectionPopup.ExecuteClosePopup();
				return;
			}
			if (this.WeaponDesign.CraftingHistory.IsVisible)
			{
				this.WeaponDesign.CraftingHistory.ExecuteCancel();
				return;
			}
			this.Smelting.SaveItemLockStates();
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x06001423 RID: 5155 RVA: 0x0004CFE4 File Offset: 0x0004B1E4
		public void ExecuteMainAction()
		{
			if (this.IsInSmeltingMode)
			{
				this.Smelting.TrySmeltingSelectedItems(this.CurrentCraftingHero.Hero);
			}
			else if (this.IsInRefinementMode)
			{
				this.Refinement.ExecuteSelectedRefinement(this.CurrentCraftingHero.Hero);
			}
			else if (Campaign.Current.GameMode == CampaignGameMode.Tutorial)
			{
				CraftingState craftingState;
				if ((craftingState = GameStateManager.Current.ActiveState as CraftingState) != null)
				{
					ItemObject currentCraftedItemObject = craftingState.CraftingLogic.GetCurrentCraftedItemObject(true, null);
					ItemObject itemObject = MBObjectManager.Instance.GetObject<ItemObject>(currentCraftedItemObject.WeaponDesign.HashedCode) ?? MBObjectManager.Instance.RegisterObject<ItemObject>(currentCraftedItemObject);
					PartyBase.MainParty.ItemRoster.AddToCounts(itemObject, 1);
					this.WeaponDesign.IsInFinalCraftingStage = false;
				}
			}
			else
			{
				if (!this.HaveMaterialsNeeded() || !this.HaveEnergy())
				{
					return;
				}
				this.WeaponDesign.ModifierTier = Campaign.Current.Models.SmithingModel.GetModifierTierForSmithedWeapon(this._crafting.CurrentWeaponDesign, this.CurrentCraftingHero.Hero);
				this.WeaponDesign.OverridenData = Campaign.Current.Models.SmithingModel.GetModifierChanges(this.WeaponDesign.ModifierTier, this.CurrentCraftingHero.Hero, this._crafting.GetCurrentCraftedItemObject(false, null).GetWeaponWithUsageIndex(0));
				this.WeaponDesign.IsInFinalCraftingStage = true;
				this.WeaponDesign.RegisterTempItemObject();
				if (this.WeaponDesign.IsInOrderMode)
				{
					WeaponDesignVM weaponDesign = this.WeaponDesign;
					ICraftingCampaignBehavior craftingBehavior = this._craftingBehavior;
					CraftingAvailableHeroItemVM currentCraftingHero = this.GetCurrentCraftingHero();
					Hero hero = ((currentCraftingHero != null) ? currentCraftingHero.Hero : null);
					CraftingOrderItemVM activeCraftingOrder = this.WeaponDesign.ActiveCraftingOrder;
					weaponDesign.CraftedItemObject = craftingBehavior.CreateCraftedWeaponInCraftingOrderMode(hero, (activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder : null, this._crafting.CurrentWeaponDesign, this.WeaponDesign.ModifierTier, this.WeaponDesign.OverridenData);
				}
				this.WeaponDesign.CreateCraftingResultPopup();
				Action onWeaponCrafted = this._onWeaponCrafted;
				if (onWeaponCrafted != null)
				{
					onWeaponCrafted();
				}
			}
			if (!this.IsInSmeltingMode)
			{
				this.UpdateAll();
			}
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x0004D1F4 File Offset: 0x0004B3F4
		public void ExecuteResetCamera()
		{
			this._resetCamera();
		}

		// Token: 0x06001425 RID: 5157 RVA: 0x0004D201 File Offset: 0x0004B401
		private CraftingAvailableHeroItemVM GetCurrentCraftingHero()
		{
			return this.CurrentCraftingHero;
		}

		// Token: 0x06001426 RID: 5158 RVA: 0x0004D209 File Offset: 0x0004B409
		public void SetConfirmInputKey(HotKey hotKey)
		{
			this.ConfirmInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x0004D218 File Offset: 0x0004B418
		public void SetExitInputKey(HotKey hotKey)
		{
			this.ExitInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0004D227 File Offset: 0x0004B427
		public void SetPreviousTabInputKey(HotKey hotKey)
		{
			this.PreviousTabInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x0004D236 File Offset: 0x0004B436
		public void SetNextTabInputKey(HotKey hotKey)
		{
			this.NextTabInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x0600142A RID: 5162 RVA: 0x0004D245 File Offset: 0x0004B445
		// (set) Token: 0x0600142B RID: 5163 RVA: 0x0004D24D File Offset: 0x0004B44D
		public InputKeyItemVM ConfirmInputKey
		{
			get
			{
				return this._confirmInputKey;
			}
			set
			{
				if (value != this._confirmInputKey)
				{
					this._confirmInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ConfirmInputKey");
				}
			}
		}

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x0600142C RID: 5164 RVA: 0x0004D26B File Offset: 0x0004B46B
		// (set) Token: 0x0600142D RID: 5165 RVA: 0x0004D273 File Offset: 0x0004B473
		public InputKeyItemVM ExitInputKey
		{
			get
			{
				return this._exitInputKey;
			}
			set
			{
				if (value != this._exitInputKey)
				{
					this._exitInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ExitInputKey");
				}
			}
		}

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x0600142E RID: 5166 RVA: 0x0004D291 File Offset: 0x0004B491
		// (set) Token: 0x0600142F RID: 5167 RVA: 0x0004D299 File Offset: 0x0004B499
		public InputKeyItemVM PreviousTabInputKey
		{
			get
			{
				return this._previousTabInputKey;
			}
			set
			{
				if (value != this._previousTabInputKey)
				{
					this._previousTabInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PreviousTabInputKey");
				}
			}
		}

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06001430 RID: 5168 RVA: 0x0004D2B7 File Offset: 0x0004B4B7
		// (set) Token: 0x06001431 RID: 5169 RVA: 0x0004D2BF File Offset: 0x0004B4BF
		public InputKeyItemVM NextTabInputKey
		{
			get
			{
				return this._nextTabInputKey;
			}
			set
			{
				if (value != this._nextTabInputKey)
				{
					this._nextTabInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "NextTabInputKey");
				}
			}
		}

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x06001432 RID: 5170 RVA: 0x0004D2DD File Offset: 0x0004B4DD
		// (set) Token: 0x06001433 RID: 5171 RVA: 0x0004D2E5 File Offset: 0x0004B4E5
		public bool CanSwitchTabs
		{
			get
			{
				return this._canSwitchTabs;
			}
			set
			{
				if (value != this._canSwitchTabs)
				{
					this._canSwitchTabs = value;
					base.OnPropertyChangedWithValue(value, "CanSwitchTabs");
				}
			}
		}

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x06001434 RID: 5172 RVA: 0x0004D303 File Offset: 0x0004B503
		// (set) Token: 0x06001435 RID: 5173 RVA: 0x0004D30B File Offset: 0x0004B50B
		public bool IsCharacterSelectionActive
		{
			get
			{
				return this._isCharacterSelectionActive;
			}
			set
			{
				if (value != this._isCharacterSelectionActive)
				{
					this._isCharacterSelectionActive = value;
					base.OnPropertyChangedWithValue(value, "IsCharacterSelectionActive");
				}
			}
		}

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x06001436 RID: 5174 RVA: 0x0004D329 File Offset: 0x0004B529
		// (set) Token: 0x06001437 RID: 5175 RVA: 0x0004D331 File Offset: 0x0004B531
		[DataSourceProperty]
		public MBBindingList<CraftingResourceItemVM> PlayerCurrentMaterials
		{
			get
			{
				return this._playerCurrentMaterials;
			}
			set
			{
				if (value != this._playerCurrentMaterials)
				{
					this._playerCurrentMaterials = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingResourceItemVM>>(value, "PlayerCurrentMaterials");
				}
			}
		}

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06001438 RID: 5176 RVA: 0x0004D34F File Offset: 0x0004B54F
		// (set) Token: 0x06001439 RID: 5177 RVA: 0x0004D357 File Offset: 0x0004B557
		[DataSourceProperty]
		public MBBindingList<CraftingAvailableHeroItemVM> AvailableCharactersForSmithing
		{
			get
			{
				return this._availableCharactersForSmithing;
			}
			set
			{
				if (value != this._availableCharactersForSmithing)
				{
					this._availableCharactersForSmithing = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingAvailableHeroItemVM>>(value, "AvailableCharactersForSmithing");
				}
			}
		}

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x0600143A RID: 5178 RVA: 0x0004D375 File Offset: 0x0004B575
		// (set) Token: 0x0600143B RID: 5179 RVA: 0x0004D37D File Offset: 0x0004B57D
		[DataSourceProperty]
		public CraftingAvailableHeroItemVM CurrentCraftingHero
		{
			get
			{
				return this._currentCraftingHero;
			}
			set
			{
				if (value != this._currentCraftingHero)
				{
					this._currentCraftingHero = value;
					base.OnPropertyChangedWithValue<CraftingAvailableHeroItemVM>(value, "CurrentCraftingHero");
				}
			}
		}

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x0600143C RID: 5180 RVA: 0x0004D39B File Offset: 0x0004B59B
		// (set) Token: 0x0600143D RID: 5181 RVA: 0x0004D3A3 File Offset: 0x0004B5A3
		[DataSourceProperty]
		public string CurrentCategoryText
		{
			get
			{
				return this._currentCategoryText;
			}
			set
			{
				if (value != this._currentCategoryText)
				{
					this._currentCategoryText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCategoryText");
				}
			}
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x0600143E RID: 5182 RVA: 0x0004D3C6 File Offset: 0x0004B5C6
		// (set) Token: 0x0600143F RID: 5183 RVA: 0x0004D3CE File Offset: 0x0004B5CE
		[DataSourceProperty]
		public string CraftingText
		{
			get
			{
				return this._craftingText;
			}
			set
			{
				if (value != this._craftingText)
				{
					this._craftingText = value;
					base.OnPropertyChangedWithValue<string>(value, "CraftingText");
				}
			}
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x06001440 RID: 5184 RVA: 0x0004D3F1 File Offset: 0x0004B5F1
		// (set) Token: 0x06001441 RID: 5185 RVA: 0x0004D3F9 File Offset: 0x0004B5F9
		[DataSourceProperty]
		public string SmeltingText
		{
			get
			{
				return this._smeltingText;
			}
			set
			{
				if (value != this._smeltingText)
				{
					this._smeltingText = value;
					base.OnPropertyChangedWithValue<string>(value, "SmeltingText");
				}
			}
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x06001442 RID: 5186 RVA: 0x0004D41C File Offset: 0x0004B61C
		// (set) Token: 0x06001443 RID: 5187 RVA: 0x0004D424 File Offset: 0x0004B624
		[DataSourceProperty]
		public string RefinementText
		{
			get
			{
				return this._refinementText;
			}
			set
			{
				if (value != this._refinementText)
				{
					this._refinementText = value;
					base.OnPropertyChangedWithValue<string>(value, "RefinementText");
				}
			}
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x06001444 RID: 5188 RVA: 0x0004D447 File Offset: 0x0004B647
		// (set) Token: 0x06001445 RID: 5189 RVA: 0x0004D44F File Offset: 0x0004B64F
		[DataSourceProperty]
		public string MainActionText
		{
			get
			{
				return this._mainActionText;
			}
			set
			{
				if (value != this._mainActionText)
				{
					this._mainActionText = value;
					base.OnPropertyChangedWithValue<string>(value, "MainActionText");
				}
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x06001446 RID: 5190 RVA: 0x0004D472 File Offset: 0x0004B672
		// (set) Token: 0x06001447 RID: 5191 RVA: 0x0004D47A File Offset: 0x0004B67A
		[DataSourceProperty]
		public bool IsMainActionEnabled
		{
			get
			{
				return this._isMainActionEnabled;
			}
			set
			{
				if (value != this._isMainActionEnabled)
				{
					this._isMainActionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMainActionEnabled");
				}
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x06001448 RID: 5192 RVA: 0x0004D498 File Offset: 0x0004B698
		// (set) Token: 0x06001449 RID: 5193 RVA: 0x0004D4A0 File Offset: 0x0004B6A0
		[DataSourceProperty]
		public int ItemValue
		{
			get
			{
				return this._itemValue;
			}
			set
			{
				if (value != this._itemValue)
				{
					this._itemValue = value;
					base.OnPropertyChangedWithValue(value, "ItemValue");
				}
			}
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x0600144A RID: 5194 RVA: 0x0004D4BE File Offset: 0x0004B6BE
		// (set) Token: 0x0600144B RID: 5195 RVA: 0x0004D4C6 File Offset: 0x0004B6C6
		[DataSourceProperty]
		public HintViewModel CraftingHint
		{
			get
			{
				return this._craftingHint;
			}
			set
			{
				if (value != this._craftingHint)
				{
					this._craftingHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CraftingHint");
				}
			}
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x0600144C RID: 5196 RVA: 0x0004D4E4 File Offset: 0x0004B6E4
		// (set) Token: 0x0600144D RID: 5197 RVA: 0x0004D4EC File Offset: 0x0004B6EC
		[DataSourceProperty]
		public HintViewModel RefiningHint
		{
			get
			{
				return this._refiningHint;
			}
			set
			{
				if (value != this._refiningHint)
				{
					this._refiningHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RefiningHint");
				}
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x0600144E RID: 5198 RVA: 0x0004D50A File Offset: 0x0004B70A
		// (set) Token: 0x0600144F RID: 5199 RVA: 0x0004D512 File Offset: 0x0004B712
		[DataSourceProperty]
		public HintViewModel SmeltingHint
		{
			get
			{
				return this._smeltingHint;
			}
			set
			{
				if (value != this._smeltingHint)
				{
					this._smeltingHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SmeltingHint");
				}
			}
		}

		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x06001450 RID: 5200 RVA: 0x0004D530 File Offset: 0x0004B730
		// (set) Token: 0x06001451 RID: 5201 RVA: 0x0004D538 File Offset: 0x0004B738
		[DataSourceProperty]
		public HintViewModel ResetCameraHint
		{
			get
			{
				return this._resetCameraHint;
			}
			set
			{
				if (value != this._resetCameraHint)
				{
					this._resetCameraHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResetCameraHint");
				}
			}
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x06001452 RID: 5202 RVA: 0x0004D556 File Offset: 0x0004B756
		// (set) Token: 0x06001453 RID: 5203 RVA: 0x0004D55E File Offset: 0x0004B75E
		[DataSourceProperty]
		public BasicTooltipViewModel MainActionHint
		{
			get
			{
				return this._mainActionHint;
			}
			set
			{
				if (value != this._mainActionHint)
				{
					this._mainActionHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "MainActionHint");
				}
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x06001454 RID: 5204 RVA: 0x0004D57C File Offset: 0x0004B77C
		// (set) Token: 0x06001455 RID: 5205 RVA: 0x0004D584 File Offset: 0x0004B784
		[DataSourceProperty]
		public string DoneLbl
		{
			get
			{
				return this._doneLbl;
			}
			set
			{
				if (value != this._doneLbl)
				{
					this._doneLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneLbl");
				}
			}
		}

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x06001456 RID: 5206 RVA: 0x0004D5A7 File Offset: 0x0004B7A7
		// (set) Token: 0x06001457 RID: 5207 RVA: 0x0004D5AF File Offset: 0x0004B7AF
		[DataSourceProperty]
		public string CancelLbl
		{
			get
			{
				return this._cancelLbl;
			}
			set
			{
				if (value != this._cancelLbl)
				{
					this._cancelLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelLbl");
				}
			}
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x0004D5D4 File Offset: 0x0004B7D4
		public void ExecuteSwitchToCrafting()
		{
			this.IsInSmeltingMode = false;
			this.IsInCraftingMode = true;
			this.IsInRefinementMode = false;
			this.CurrentCategoryText = new TextObject("{=POjDNVW3}Forging", null).ToString();
			this.MainActionText = GameTexts.FindText("str_crafting_category_crafting", null).ToString();
			CraftingVM.OnItemRefreshedDelegate onItemRefreshed = this.OnItemRefreshed;
			if (onItemRefreshed != null)
			{
				onItemRefreshed(true);
			}
			this.UpdateCurrentMaterialCosts();
			this.UpdateAll();
			WeaponDesignVM weaponDesign = this.WeaponDesign;
			if (weaponDesign == null)
			{
				return;
			}
			weaponDesign.ChangeModeIfHeroIsUnavailable();
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x0004D650 File Offset: 0x0004B850
		public void ExecuteSwitchToSmelting()
		{
			this.IsInSmeltingMode = true;
			this.IsInCraftingMode = false;
			this.IsInRefinementMode = false;
			this.CurrentCategoryText = new TextObject("{=4cU98rkg}Smelting", null).ToString();
			this.MainActionText = GameTexts.FindText("str_crafting_category_smelting", null).ToString();
			CraftingVM.OnItemRefreshedDelegate onItemRefreshed = this.OnItemRefreshed;
			if (onItemRefreshed != null)
			{
				onItemRefreshed(false);
			}
			this.UpdateCurrentMaterialCosts();
			this.Smelting.RefreshList();
			this.UpdateAll();
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x0004D6C8 File Offset: 0x0004B8C8
		public void ExecuteSwitchToRefinement()
		{
			this.IsInSmeltingMode = false;
			this.IsInCraftingMode = false;
			this.IsInRefinementMode = true;
			this.CurrentCategoryText = new TextObject("{=p7raHA9x}Refinement", null).ToString();
			this.MainActionText = GameTexts.FindText("str_crafting_category_refinement", null).ToString();
			CraftingVM.OnItemRefreshedDelegate onItemRefreshed = this.OnItemRefreshed;
			if (onItemRefreshed != null)
			{
				onItemRefreshed(false);
			}
			this.UpdateCurrentMaterialCosts();
			this.Refinement.RefreshRefinementActionsList(this.CurrentCraftingHero.Hero);
			this.UpdateAll();
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x0004D74A File Offset: 0x0004B94A
		private void OnRefinementSelectionChange()
		{
			this.UpdateCurrentMaterialCosts();
			this.RefreshEnableMainAction();
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x0004D758 File Offset: 0x0004B958
		private void OnSmeltItemSelection()
		{
			this.UpdateCurrentMaterialCosts();
			this.RefreshEnableMainAction();
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x0004D766 File Offset: 0x0004B966
		public void SetCurrentDesignManually(CraftingTemplate craftingTemplate, ValueTuple<CraftingPiece, int>[] pieces)
		{
			if (!this.IsInCraftingMode)
			{
				this.ExecuteSwitchToCrafting();
			}
			this.WeaponDesign.SetDesignManually(craftingTemplate, pieces, false);
		}

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x0600145E RID: 5214 RVA: 0x0004D784 File Offset: 0x0004B984
		// (set) Token: 0x0600145F RID: 5215 RVA: 0x0004D78C File Offset: 0x0004B98C
		[DataSourceProperty]
		public SmeltingVM Smelting
		{
			get
			{
				return this._smelting;
			}
			set
			{
				if (value != this._smelting)
				{
					this._smelting = value;
					base.OnPropertyChangedWithValue<SmeltingVM>(value, "Smelting");
				}
			}
		}

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x06001460 RID: 5216 RVA: 0x0004D7AA File Offset: 0x0004B9AA
		// (set) Token: 0x06001461 RID: 5217 RVA: 0x0004D7B2 File Offset: 0x0004B9B2
		[DataSourceProperty]
		public WeaponDesignVM WeaponDesign
		{
			get
			{
				return this._weaponDesign;
			}
			set
			{
				if (value != this._weaponDesign)
				{
					this._weaponDesign = value;
					base.OnPropertyChangedWithValue<WeaponDesignVM>(value, "WeaponDesign");
				}
			}
		}

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x06001462 RID: 5218 RVA: 0x0004D7D0 File Offset: 0x0004B9D0
		// (set) Token: 0x06001463 RID: 5219 RVA: 0x0004D7D8 File Offset: 0x0004B9D8
		[DataSourceProperty]
		public RefinementVM Refinement
		{
			get
			{
				return this._refinement;
			}
			set
			{
				if (value != this._refinement)
				{
					this._refinement = value;
					base.OnPropertyChangedWithValue<RefinementVM>(value, "Refinement");
				}
			}
		}

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x0004D7F6 File Offset: 0x0004B9F6
		// (set) Token: 0x06001465 RID: 5221 RVA: 0x0004D7FE File Offset: 0x0004B9FE
		[DataSourceProperty]
		public bool IsInCraftingMode
		{
			get
			{
				return this._isInCraftingMode;
			}
			set
			{
				if (value != this._isInCraftingMode)
				{
					this._isInCraftingMode = value;
					base.OnPropertyChangedWithValue(value, "IsInCraftingMode");
				}
			}
		}

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x06001466 RID: 5222 RVA: 0x0004D81C File Offset: 0x0004BA1C
		// (set) Token: 0x06001467 RID: 5223 RVA: 0x0004D824 File Offset: 0x0004BA24
		[DataSourceProperty]
		public bool IsInSmeltingMode
		{
			get
			{
				return this._isInSmeltingMode;
			}
			set
			{
				if (value != this._isInSmeltingMode)
				{
					this._isInSmeltingMode = value;
					base.OnPropertyChangedWithValue(value, "IsInSmeltingMode");
				}
			}
		}

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x06001468 RID: 5224 RVA: 0x0004D842 File Offset: 0x0004BA42
		// (set) Token: 0x06001469 RID: 5225 RVA: 0x0004D84A File Offset: 0x0004BA4A
		[DataSourceProperty]
		public bool IsInRefinementMode
		{
			get
			{
				return this._isInRefinementMode;
			}
			set
			{
				if (value != this._isInRefinementMode)
				{
					this._isInRefinementMode = value;
					base.OnPropertyChangedWithValue(value, "IsInRefinementMode");
				}
			}
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x0600146A RID: 5226 RVA: 0x0004D868 File Offset: 0x0004BA68
		// (set) Token: 0x0600146B RID: 5227 RVA: 0x0004D870 File Offset: 0x0004BA70
		[DataSourceProperty]
		public bool IsSmeltingItemSelected
		{
			get
			{
				return this._isSmeltingItemSelected;
			}
			set
			{
				if (value != this._isSmeltingItemSelected)
				{
					this._isSmeltingItemSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSmeltingItemSelected");
				}
			}
		}

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x0600146C RID: 5228 RVA: 0x0004D88E File Offset: 0x0004BA8E
		// (set) Token: 0x0600146D RID: 5229 RVA: 0x0004D896 File Offset: 0x0004BA96
		[DataSourceProperty]
		public bool IsRefinementItemSelected
		{
			get
			{
				return this._isRefinementItemSelected;
			}
			set
			{
				if (value != this._isRefinementItemSelected)
				{
					this._isRefinementItemSelected = value;
					base.OnPropertyChangedWithValue(value, "IsRefinementItemSelected");
				}
			}
		}

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x0600146E RID: 5230 RVA: 0x0004D8B4 File Offset: 0x0004BAB4
		// (set) Token: 0x0600146F RID: 5231 RVA: 0x0004D8BC File Offset: 0x0004BABC
		[DataSourceProperty]
		public string SelectItemToSmeltText
		{
			get
			{
				return this._selectItemToSmeltText;
			}
			set
			{
				if (value != this._selectItemToSmeltText)
				{
					this._selectItemToSmeltText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectItemToSmeltText");
				}
			}
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x06001470 RID: 5232 RVA: 0x0004D8DF File Offset: 0x0004BADF
		// (set) Token: 0x06001471 RID: 5233 RVA: 0x0004D8E7 File Offset: 0x0004BAE7
		[DataSourceProperty]
		public string SelectItemToRefineText
		{
			get
			{
				return this._selectItemToRefineText;
			}
			set
			{
				if (value != this._selectItemToRefineText)
				{
					this._selectItemToRefineText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectItemToRefineText");
				}
			}
		}

		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x06001472 RID: 5234 RVA: 0x0004D90A File Offset: 0x0004BB0A
		// (set) Token: 0x06001473 RID: 5235 RVA: 0x0004D912 File Offset: 0x0004BB12
		[DataSourceProperty]
		public ElementNotificationVM TutorialNotification
		{
			get
			{
				return this._tutorialNotification;
			}
			set
			{
				if (value != this._tutorialNotification)
				{
					this._tutorialNotification = value;
					base.OnPropertyChangedWithValue<ElementNotificationVM>(value, "TutorialNotification");
				}
			}
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x0004D930 File Offset: 0x0004BB30
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = string.Empty;
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = this._latestTutorialElementID;
				}
			}
		}

		// Token: 0x0400095C RID: 2396
		private const int _minimumRequiredStamina = 10;

		// Token: 0x0400095D RID: 2397
		public CraftingVM.OnItemRefreshedDelegate OnItemRefreshed;

		// Token: 0x0400095E RID: 2398
		private readonly Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> _getItemUsageSetFlags;

		// Token: 0x0400095F RID: 2399
		private readonly ICraftingCampaignBehavior _craftingBehavior;

		// Token: 0x04000960 RID: 2400
		private readonly Action _onClose;

		// Token: 0x04000961 RID: 2401
		private readonly Action _resetCamera;

		// Token: 0x04000962 RID: 2402
		private readonly Action _onWeaponCrafted;

		// Token: 0x04000963 RID: 2403
		private Crafting _crafting;

		// Token: 0x04000964 RID: 2404
		private InputKeyItemVM _confirmInputKey;

		// Token: 0x04000965 RID: 2405
		private InputKeyItemVM _exitInputKey;

		// Token: 0x04000966 RID: 2406
		private InputKeyItemVM _previousTabInputKey;

		// Token: 0x04000967 RID: 2407
		private InputKeyItemVM _nextTabInputKey;

		// Token: 0x04000968 RID: 2408
		private bool _canSwitchTabs;

		// Token: 0x04000969 RID: 2409
		private bool _isCharacterSelectionActive;

		// Token: 0x0400096A RID: 2410
		private string _doneLbl;

		// Token: 0x0400096B RID: 2411
		private string _cancelLbl;

		// Token: 0x0400096C RID: 2412
		private HintViewModel _resetCameraHint;

		// Token: 0x0400096D RID: 2413
		private HintViewModel _smeltingHint;

		// Token: 0x0400096E RID: 2414
		private HintViewModel _craftingHint;

		// Token: 0x0400096F RID: 2415
		private HintViewModel _refiningHint;

		// Token: 0x04000970 RID: 2416
		private BasicTooltipViewModel _mainActionHint;

		// Token: 0x04000971 RID: 2417
		private int _itemValue = -1;

		// Token: 0x04000972 RID: 2418
		private string _currentCategoryText;

		// Token: 0x04000973 RID: 2419
		private string _mainActionText;

		// Token: 0x04000974 RID: 2420
		private string _craftingText;

		// Token: 0x04000975 RID: 2421
		private string _smeltingText;

		// Token: 0x04000976 RID: 2422
		private string _refinementText;

		// Token: 0x04000977 RID: 2423
		private bool _isMainActionEnabled;

		// Token: 0x04000978 RID: 2424
		private MBBindingList<CraftingAvailableHeroItemVM> _availableCharactersForSmithing;

		// Token: 0x04000979 RID: 2425
		private CraftingAvailableHeroItemVM _currentCraftingHero;

		// Token: 0x0400097A RID: 2426
		private MBBindingList<CraftingResourceItemVM> _playerCurrentMaterials;

		// Token: 0x0400097B RID: 2427
		private bool _isInSmeltingMode;

		// Token: 0x0400097C RID: 2428
		private bool _isInCraftingMode;

		// Token: 0x0400097D RID: 2429
		private bool _isInRefinementMode;

		// Token: 0x0400097E RID: 2430
		private SmeltingVM _smelting;

		// Token: 0x0400097F RID: 2431
		private RefinementVM _refinement;

		// Token: 0x04000980 RID: 2432
		private WeaponDesignVM _weaponDesign;

		// Token: 0x04000981 RID: 2433
		private bool _isSmeltingItemSelected;

		// Token: 0x04000982 RID: 2434
		private bool _isRefinementItemSelected;

		// Token: 0x04000983 RID: 2435
		private string _selectItemToSmeltText;

		// Token: 0x04000984 RID: 2436
		private string _selectItemToRefineText;

		// Token: 0x04000985 RID: 2437
		public ElementNotificationVM _tutorialNotification;

		// Token: 0x04000986 RID: 2438
		private string _latestTutorialElementID;

		// Token: 0x02000208 RID: 520
		// (Invoke) Token: 0x060020D4 RID: 8404
		public delegate void OnItemRefreshedDelegate(bool isItemVisible);
	}
}
