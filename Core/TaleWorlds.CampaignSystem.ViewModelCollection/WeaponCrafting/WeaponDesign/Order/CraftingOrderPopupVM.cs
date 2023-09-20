using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign.Order
{
	// Token: 0x020000EB RID: 235
	public class CraftingOrderPopupVM : ViewModel
	{
		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x06001621 RID: 5665 RVA: 0x00052BEA File Offset: 0x00050DEA
		public bool HasOrders
		{
			get
			{
				return this.CraftingOrders.Count > 0;
			}
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x00052BFA File Offset: 0x00050DFA
		public CraftingOrderPopupVM(Action<CraftingOrderItemVM> onDoneAction, Func<CraftingAvailableHeroItemVM> getCurrentCraftingHero, Func<CraftingOrder, IEnumerable<CraftingStatData>> getOrderStatDatas)
		{
			this._onDoneAction = onDoneAction;
			this._getCurrentCraftingHero = getCurrentCraftingHero;
			this._getOrderStatDatas = getOrderStatDatas;
			this._craftingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			this.CraftingOrders = new MBBindingList<CraftingOrderItemVM>();
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x00052C34 File Offset: 0x00050E34
		public void RefreshOrders()
		{
			this.CraftingOrders.Clear();
			if (Campaign.Current.GameMode == CampaignGameMode.Tutorial)
			{
				return;
			}
			IReadOnlyDictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots> craftingOrders = this._craftingBehavior.CraftingOrders;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			CraftingCampaignBehavior.CraftingOrderSlots craftingOrderSlots = craftingOrders[(currentSettlement != null) ? currentSettlement.Town : null];
			if (craftingOrderSlots == null)
			{
				return;
			}
			List<CraftingOrder> list = craftingOrderSlots.Slots.Where((CraftingOrder x) => x != null).ToList<CraftingOrder>();
			foreach (KeyValuePair<string, List<CraftingOrder>> keyValuePair in craftingOrderSlots.QuestOrders.Where((KeyValuePair<string, List<CraftingOrder>> x) => x.Key != null && x.Value != null))
			{
				list.AddRange(keyValuePair.Value);
			}
			OrderDifficultyComparer orderDifficultyComparer = new OrderDifficultyComparer();
			list.Sort(orderDifficultyComparer);
			foreach (CraftingOrder craftingOrder in list)
			{
				List<CraftingStatData> list2 = this._getOrderStatDatas(craftingOrder).ToList<CraftingStatData>();
				this.CraftingOrders.Add(new CraftingOrderItemVM(craftingOrder, new Action<CraftingOrderItemVM>(this.SelectOrder), this._getCurrentCraftingHero, list2));
			}
			TextObject textObject = new TextObject("{=MkVTRqAw}Orders ({ORDER_COUNT})", null);
			textObject.SetTextVariable("ORDER_COUNT", this.CraftingOrders.Count);
			this.OrderCountText = textObject.ToString();
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x00052DD0 File Offset: 0x00050FD0
		public void SelectOrder(CraftingOrderItemVM order)
		{
			if (this.SelectedCraftingOrder != null)
			{
				this.SelectedCraftingOrder.IsSelected = false;
			}
			this.SelectedCraftingOrder = order;
			this.SelectedCraftingOrder.IsSelected = true;
			this._onDoneAction(order);
			this.IsVisible = false;
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x00052E0C File Offset: 0x0005100C
		public void ExecuteOpenPopup()
		{
			this.IsVisible = true;
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x00052E15 File Offset: 0x00051015
		public void ExecuteCloseWithoutSelection()
		{
			this.IsVisible = false;
		}

		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x06001627 RID: 5671 RVA: 0x00052E1E File Offset: 0x0005101E
		// (set) Token: 0x06001628 RID: 5672 RVA: 0x00052E26 File Offset: 0x00051026
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
					Game game = Game.Current;
					if (game == null)
					{
						return;
					}
					game.EventManager.TriggerEvent<CraftingOrderSelectionOpenedEvent>(new CraftingOrderSelectionOpenedEvent(this._isVisible));
				}
			}
		}

		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x06001629 RID: 5673 RVA: 0x00052E63 File Offset: 0x00051063
		// (set) Token: 0x0600162A RID: 5674 RVA: 0x00052E6B File Offset: 0x0005106B
		[DataSourceProperty]
		public CraftingOrderItemVM SelectedCraftingOrder
		{
			get
			{
				return this._selectedCraftingOrder;
			}
			set
			{
				if (value != this._selectedCraftingOrder)
				{
					this._selectedCraftingOrder = value;
					base.OnPropertyChangedWithValue<CraftingOrderItemVM>(value, "SelectedCraftingOrder");
				}
			}
		}

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x0600162B RID: 5675 RVA: 0x00052E89 File Offset: 0x00051089
		// (set) Token: 0x0600162C RID: 5676 RVA: 0x00052E91 File Offset: 0x00051091
		[DataSourceProperty]
		public string OrderCountText
		{
			get
			{
				return this._orderCountText;
			}
			set
			{
				if (value != this._orderCountText)
				{
					this._orderCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "OrderCountText");
				}
			}
		}

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x0600162D RID: 5677 RVA: 0x00052EB4 File Offset: 0x000510B4
		// (set) Token: 0x0600162E RID: 5678 RVA: 0x00052EBC File Offset: 0x000510BC
		[DataSourceProperty]
		public MBBindingList<CraftingOrderItemVM> CraftingOrders
		{
			get
			{
				return this._craftingOrders;
			}
			set
			{
				if (value != this._craftingOrders)
				{
					this._craftingOrders = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingOrderItemVM>>(value, "CraftingOrders");
				}
			}
		}

		// Token: 0x04000A5D RID: 2653
		private Action<CraftingOrderItemVM> _onDoneAction;

		// Token: 0x04000A5E RID: 2654
		private Func<CraftingAvailableHeroItemVM> _getCurrentCraftingHero;

		// Token: 0x04000A5F RID: 2655
		private Func<CraftingOrder, IEnumerable<CraftingStatData>> _getOrderStatDatas;

		// Token: 0x04000A60 RID: 2656
		private readonly ICraftingCampaignBehavior _craftingBehavior;

		// Token: 0x04000A61 RID: 2657
		private string _orderCountText;

		// Token: 0x04000A62 RID: 2658
		private MBBindingList<CraftingOrderItemVM> _craftingOrders;

		// Token: 0x04000A63 RID: 2659
		private CraftingOrderItemVM _selectedCraftingOrder;

		// Token: 0x04000A64 RID: 2660
		private bool _isVisible;
	}
}
