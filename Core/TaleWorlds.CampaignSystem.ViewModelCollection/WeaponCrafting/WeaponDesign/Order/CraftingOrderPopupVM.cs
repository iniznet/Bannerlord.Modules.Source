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
	public class CraftingOrderPopupVM : ViewModel
	{
		public bool HasOrders
		{
			get
			{
				return this.CraftingOrders.Count > 0;
			}
		}

		public CraftingOrderPopupVM(Action<CraftingOrderItemVM> onDoneAction, Func<CraftingAvailableHeroItemVM> getCurrentCraftingHero, Func<CraftingOrder, IEnumerable<CraftingStatData>> getOrderStatDatas)
		{
			this._onDoneAction = onDoneAction;
			this._getCurrentCraftingHero = getCurrentCraftingHero;
			this._getOrderStatDatas = getOrderStatDatas;
			this._craftingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			this.CraftingOrders = new MBBindingList<CraftingOrderItemVM>();
		}

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

		public void ExecuteOpenPopup()
		{
			this.IsVisible = true;
		}

		public void ExecuteCloseWithoutSelection()
		{
			this.IsVisible = false;
		}

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

		private Action<CraftingOrderItemVM> _onDoneAction;

		private Func<CraftingAvailableHeroItemVM> _getCurrentCraftingHero;

		private Func<CraftingOrder, IEnumerable<CraftingStatData>> _getOrderStatDatas;

		private readonly ICraftingCampaignBehavior _craftingBehavior;

		private string _orderCountText;

		private MBBindingList<CraftingOrderItemVM> _craftingOrders;

		private CraftingOrderItemVM _selectedCraftingOrder;

		private bool _isVisible;
	}
}
