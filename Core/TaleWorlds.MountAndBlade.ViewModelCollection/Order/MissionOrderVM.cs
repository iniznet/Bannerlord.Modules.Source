using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000020 RID: 32
	public class MissionOrderVM : ViewModel
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600023F RID: 575 RVA: 0x00009D9D File Offset: 0x00007F9D
		private Team Team
		{
			get
			{
				return Mission.Current.PlayerTeam;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000240 RID: 576 RVA: 0x00009DA9 File Offset: 0x00007FA9
		// (set) Token: 0x06000241 RID: 577 RVA: 0x00009DB1 File Offset: 0x00007FB1
		public OrderItemVM LastSelectedOrderItem { get; private set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000242 RID: 578 RVA: 0x00009DBA File Offset: 0x00007FBA
		public OrderController OrderController
		{
			get
			{
				return this.Team.PlayerOrderController;
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000243 RID: 579 RVA: 0x00009DC7 File Offset: 0x00007FC7
		public bool IsMovementSubOrdersShown
		{
			get
			{
				return this._movementSet.ShowOrders;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000244 RID: 580 RVA: 0x00009DD4 File Offset: 0x00007FD4
		public bool IsFacingSubOrdersShown
		{
			get
			{
				OrderSetVM facingSet = this._facingSet;
				return facingSet != null && facingSet.ShowOrders;
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000245 RID: 581 RVA: 0x00009DE7 File Offset: 0x00007FE7
		// (set) Token: 0x06000246 RID: 582 RVA: 0x00009DEF File Offset: 0x00007FEF
		public bool IsTroopPlacingActive
		{
			get
			{
				return this._isTroopPlacingActive;
			}
			set
			{
				this._isTroopPlacingActive = value;
				this._toggleOrderPositionVisibility(!value);
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000247 RID: 583 RVA: 0x00009E07 File Offset: 0x00008007
		// (set) Token: 0x06000248 RID: 584 RVA: 0x00009E0F File Offset: 0x0000800F
		public OrderSetType LastSelectedOrderSetType
		{
			get
			{
				return this._lastSelectedOrderSetType;
			}
			set
			{
				if (value != this._lastSelectedOrderSetType)
				{
					this._lastSelectedOrderSetType = value;
					this.IsAnyOrderSetActive = this._lastSelectedOrderSetType != OrderSetType.None;
				}
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000249 RID: 585 RVA: 0x00009E33 File Offset: 0x00008033
		public bool PlayerHasAnyTroopUnderThem
		{
			get
			{
				return this.Team.FormationsIncludingEmpty.Any((Formation f) => f.PlayerOwner == Agent.Main && f.CountOfUnits > 0);
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x0600024A RID: 586 RVA: 0x00009E64 File Offset: 0x00008064
		private Mission Mission
		{
			get
			{
				return Mission.Current;
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00009E6C File Offset: 0x0000806C
		public MissionOrderVM(Camera deploymentCamera, List<DeploymentPoint> deploymentPoints, Action<bool> toggleMissionInputs, bool isDeployment, GetOrderFlagPositionDelegate getOrderFlagPosition, OnRefreshVisualsDelegate refreshVisuals, ToggleOrderPositionVisibilityDelegate setSuspendTroopPlacer, OnToggleActivateOrderStateDelegate onActivateToggleOrder, OnToggleActivateOrderStateDelegate onDeactivateToggleOrder, OnToggleActivateOrderStateDelegate onTransferTroopsFinishedDelegate, OnBeforeOrderDelegate onBeforeOrderDelegate, bool isMultiplayer)
		{
			this._deploymentCamera = deploymentCamera;
			this._toggleMissionInputs = toggleMissionInputs;
			this._deploymentPoints = deploymentPoints;
			this._getOrderFlagPosition = getOrderFlagPosition;
			this._onRefreshVisuals = refreshVisuals;
			this._toggleOrderPositionVisibility = setSuspendTroopPlacer;
			this._onActivateToggleOrder = onActivateToggleOrder;
			this._onDeactivateToggleOrder = onDeactivateToggleOrder;
			this._onTransferTroopsFinishedDelegate = onTransferTroopsFinishedDelegate;
			this._onBeforeOrderDelegate = onBeforeOrderDelegate;
			this._isMultiplayer = isMultiplayer;
			this.IsDeployment = isDeployment;
			this.OrderSetsWithOrdersByType = new Dictionary<OrderSetType, OrderSetVM>();
			this.OrderSets = new MBBindingList<OrderSetVM>();
			this.DeploymentController = new MissionOrderDeploymentControllerVM(this._deploymentPoints, this, this._deploymentCamera, this._toggleMissionInputs, this._onRefreshVisuals);
			this.TroopController = new MissionOrderTroopControllerVM(this, this._isMultiplayer, this.IsDeployment, new Action(this.OnTransferFinished));
			this.PopulateOrderSets();
			this.Team.OnFormationAIActiveBehaviorChanged += this.TeamOnFormationAIActiveBehaviorChanged;
			this.RefreshValues();
			this.Mission.OnMainAgentChanged += this.MissionOnMainAgentChanged;
			this.CanUseShortcuts = this._isMultiplayer;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00009F8C File Offset: 0x0000818C
		private void PopulateOrderSets()
		{
			this._movementSet = new OrderSetVM(OrderSetType.Movement, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
			OrderSetVM orderSetVM = new OrderSetVM(OrderSetType.Form, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
			bool flag = BannerlordConfig.OrderLayoutType == 1;
			this.OrderSets.Add(this._movementSet);
			this.OrderSetsWithOrdersByType.Add(OrderSetType.Movement, this._movementSet);
			if (flag)
			{
				this._facingSet = new OrderSetVM(OrderSetType.Facing, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
				this.OrderSets.Add(this._facingSet);
				this.OrderSets.Add(orderSetVM);
				this.OrderSetsWithOrdersByType.Add(OrderSetType.Facing, this._facingSet);
				this.OrderSetsWithOrdersByType.Add(OrderSetType.Form, orderSetVM);
			}
			else
			{
				OrderSetVM orderSetVM2 = new OrderSetVM(OrderSetType.Toggle, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
				this.OrderSets.Add(orderSetVM);
				this.OrderSets.Add(orderSetVM2);
				this.OrderSetsWithOrdersByType.Add(OrderSetType.Form, orderSetVM);
				this.OrderSetsWithOrdersByType.Add(OrderSetType.Toggle, orderSetVM2);
			}
			OrderSetVM orderSetVM3 = new OrderSetVM(OrderSubType.ToggleFire, this.OrderSets.Count, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
			this.OrderSets.Add(orderSetVM3);
			OrderSetVM orderSetVM4 = new OrderSetVM(OrderSubType.ToggleMount, this.OrderSets.Count, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
			this.OrderSets.Add(orderSetVM4);
			OrderSetVM orderSetVM5 = new OrderSetVM(OrderSubType.ToggleAI, this.OrderSets.Count, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
			this.OrderSets.Add(orderSetVM5);
			if (flag)
			{
				if (!this._isMultiplayer)
				{
					OrderSetVM orderSetVM6 = new OrderSetVM(OrderSubType.ToggleTransfer, this.OrderSets.Count, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
					this.OrderSets.Add(orderSetVM6);
					return;
				}
			}
			else
			{
				OrderSetVM orderSetVM7 = new OrderSetVM(OrderSubType.ActivationFaceDirection, this.OrderSets.Count, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
				this.OrderSets.Add(orderSetVM7);
				OrderSetVM orderSetVM8 = new OrderSetVM(OrderSubType.FormClose, this.OrderSets.Count, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
				this.OrderSets.Add(orderSetVM8);
				OrderSetVM orderSetVM9 = new OrderSetVM(OrderSubType.FormLine, this.OrderSets.Count, new Action<OrderItemVM, OrderSetType, bool>(this.OnOrder), this._isMultiplayer);
				this.OrderSets.Add(orderSetVM9);
			}
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000A213 File Offset: 0x00008413
		private void TeamOnFormationAIActiveBehaviorChanged(Formation formation)
		{
			if (formation.IsAIControlled)
			{
				if (this._modifiedAIFormations.IndexOf(formation) < 0)
				{
					this._modifiedAIFormations.Add(formation);
				}
				this._delayValueForAIFormationModifications = 3;
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000A240 File Offset: 0x00008440
		private void DisplayFormationAIFeedback()
		{
			this._delayValueForAIFormationModifications = Math.Max(0, this._delayValueForAIFormationModifications - 1);
			if (this._delayValueForAIFormationModifications == 0 && this._modifiedAIFormations.Count > 0)
			{
				for (int i = 0; i < this._modifiedAIFormations.Count; i++)
				{
					Formation formation = this._modifiedAIFormations[i];
					if (((formation != null) ? formation.AI.ActiveBehavior : null) != null && formation.FormationIndex < FormationClass.NumberOfRegularFormations)
					{
						MissionOrderVM.DisplayFormationAIFeedbackAux(this._modifiedAIFormations);
					}
					else
					{
						this._modifiedAIFormations[i] = null;
					}
				}
				this._modifiedAIFormations.Clear();
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000A2DC File Offset: 0x000084DC
		private static void DisplayFormationAIFeedbackAux(List<Formation> formations)
		{
			Dictionary<FormationClass, TextObject> dictionary = new Dictionary<FormationClass, TextObject>();
			Type type = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < formations.Count; i++)
			{
				Formation formation = formations[i];
				if (((formation != null) ? formation.AI.ActiveBehavior : null) != null && (type == null || type == formation.AI.ActiveBehavior.GetType()))
				{
					type = formation.AI.ActiveBehavior.GetType();
					switch (formation.AI.Side)
					{
					case FormationAI.BehaviorSide.Left:
						flag = true;
						break;
					case FormationAI.BehaviorSide.Middle:
						flag3 = true;
						break;
					case FormationAI.BehaviorSide.Right:
						flag2 = true;
						break;
					}
					if (!dictionary.ContainsKey(formation.PrimaryClass))
					{
						TextObject localizedName = formation.PrimaryClass.GetLocalizedName();
						TextObject textObject = GameTexts.FindText("str_troop_group_name_definite", null);
						textObject.SetTextVariable("FORMATION_CLASS", localizedName);
						dictionary.Add(formation.PrimaryClass, textObject);
					}
					formations[i] = null;
				}
			}
			if (dictionary.Count == 1)
			{
				MBTextManager.SetTextVariable("IS_PLURAL", 0);
				MBTextManager.SetTextVariable("TROOP_NAMES_BEGIN", TextObject.Empty, false);
				MBTextManager.SetTextVariable("TROOP_NAMES_END", dictionary.First<KeyValuePair<FormationClass, TextObject>>().Value, false);
			}
			else
			{
				MBTextManager.SetTextVariable("IS_PLURAL", 1);
				TextObject value = dictionary.Last<KeyValuePair<FormationClass, TextObject>>().Value;
				TextObject textObject2;
				if (dictionary.Count == 2)
				{
					textObject2 = dictionary.First<KeyValuePair<FormationClass, TextObject>>().Value;
				}
				else
				{
					textObject2 = GameTexts.FindText("str_LEFT_comma_RIGHT", null);
					textObject2.SetTextVariable("LEFT", dictionary.First<KeyValuePair<FormationClass, TextObject>>().Value);
					textObject2.SetTextVariable("RIGHT", dictionary.Last<KeyValuePair<FormationClass, TextObject>>().Value);
					for (int j = 2; j < dictionary.Count - 1; j++)
					{
						TextObject textObject3 = GameTexts.FindText("str_LEFT_comma_RIGHT", null);
						textObject3.SetTextVariable("LEFT", textObject2);
						textObject3.SetTextVariable("RIGHT", dictionary.Values.ElementAt(j));
						textObject2 = textObject3;
					}
				}
				MBTextManager.SetTextVariable("TROOP_NAMES_BEGIN", textObject2, false);
				MBTextManager.SetTextVariable("TROOP_NAMES_END", value, false);
			}
			bool flag4 = (flag ? 1 : 0) + (flag3 ? 1 : 0) + (flag2 ? 1 : 0) > 1;
			MBTextManager.SetTextVariable("IS_LEFT", flag4 ? 2 : (flag ? 1 : 0));
			MBTextManager.SetTextVariable("IS_MIDDLE", (!flag4 && flag3) ? 1 : 0);
			MBTextManager.SetTextVariable("IS_RIGHT", (!flag4 && flag2) ? 1 : 0);
			string name = type.Name;
			InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_formation_ai_behavior_text", name).ToString()));
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000A59C File Offset: 0x0000879C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ReturnText = new TextObject("{=EmVbbIUc}Return", null).ToString();
			foreach (OrderSetVM orderSetVM in this.OrderSetsWithOrdersByType.Values)
			{
				orderSetVM.RefreshValues();
			}
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000A610 File Offset: 0x00008810
		public void OnOrderLayoutTypeChanged()
		{
			this.TroopController = new MissionOrderTroopControllerVM(this, this._isMultiplayer, this.IsDeployment, new Action(this.OnTransferFinished));
			this.OrderSets.Clear();
			this.OrderSetsWithOrdersByType.Clear();
			this.PopulateOrderSets();
			this.TroopController.UpdateTroops();
			this.TroopController.TroopList.ApplyActionOnAllItems(delegate(OrderTroopItemVM x)
			{
				this.TroopController.SetTroopActiveOrders(x);
			});
			this.TroopController.OnFiltersSet(this._filterData);
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000A698 File Offset: 0x00008898
		public void OpenToggleOrder(bool fromHold, bool displayMessage = true)
		{
			if (this.IsToggleOrderShown)
			{
				return;
			}
			if (this.CheckCanBeOpened(displayMessage))
			{
				Mission.Current.IsOrderMenuOpen = true;
				this._currentActivationType = (fromHold ? MissionOrderVM.ActivationType.Hold : MissionOrderVM.ActivationType.Click);
				this.IsToggleOrderShown = true;
				this.TroopController.IsTransferActive = false;
				this.DeploymentController.ProcessSiegeMachines();
				if (this.OrderController.SelectedFormations.IsEmpty<Formation>())
				{
					this.TroopController.SelectAllFormations(true);
				}
				this.SetActiveOrders();
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000A714 File Offset: 0x00008914
		private bool CheckCanBeOpened(bool displayMessage = false)
		{
			if (Agent.Main == null)
			{
				if (displayMessage)
				{
					InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=GMhOZGnb}Cannot issue order while dead.", null).ToString()));
				}
				return false;
			}
			if (Mission.Current.Mode != MissionMode.Deployment && !Agent.Main.IsPlayerControlled)
			{
				if (displayMessage)
				{
					InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=b1DHZsaH}Cannot issue order right now.", null).ToString()));
				}
				return false;
			}
			if (!this.Team.HasBots || !this.PlayerHasAnyTroopUnderThem || (!this.Team.IsPlayerGeneral && !this.Team.IsPlayerSergeant))
			{
				if (displayMessage)
				{
					InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=DQvGNQ0g}There isn't any unit under command.", null).ToString()));
				}
				return false;
			}
			return !Mission.Current.IsMissionEnding || Mission.Current.CheckIfBattleInRetreat();
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000A7E8 File Offset: 0x000089E8
		public bool TryCloseToggleOrder(bool dontApplySelected = false)
		{
			if (this.IsToggleOrderShown)
			{
				Mission.Current.IsOrderMenuOpen = false;
				if (this.LastSelectedOrderItem != null && !dontApplySelected)
				{
					this.ApplySelectedOrder();
				}
				this.LastSelectedOrderSetType = OrderSetType.None;
				this.LastSelectedOrderItem = null;
				this.OrderSets.ApplyActionOnAllItems(delegate(OrderSetVM s)
				{
					s.Orders.ApplyActionOnAllItems(delegate(OrderItemVM o)
					{
						o.IsSelected = false;
					});
				});
				bool isDeployment = this._isDeployment;
				this.IsToggleOrderShown = false;
				this.UpdateTitleOrdersKeyVisualVisibility(false);
				if (!this.IsDeployment)
				{
					this.InputRestrictions.ResetInputRestrictions();
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000A87C File Offset: 0x00008A7C
		internal void SetActiveOrders()
		{
			bool flag = this.ActiveTargetState == 1;
			if (flag)
			{
				this.DeploymentController.SetCurrentActiveOrders();
			}
			else
			{
				this.TroopController.SetCurrentActiveOrders();
			}
			List<OrderSubjectVM> list = (flag ? this.DeploymentController.SiegeMachineList.Cast<OrderSubjectVM>().ToList<OrderSubjectVM>() : this.TroopController.TroopList.Cast<OrderSubjectVM>().ToList<OrderSubjectVM>()).Where((OrderSubjectVM item) => item.IsSelected && item.IsSelectable).ToList<OrderSubjectVM>();
			this.OrderSetsWithOrdersByType[OrderSetType.Movement].ResetActiveStatus(false);
			this.OrderSetsWithOrdersByType[OrderSetType.Form].ResetActiveStatus(flag && list.IsEmpty<OrderSubjectVM>());
			if (this.OrderSetsWithOrdersByType.ContainsKey(OrderSetType.Toggle))
			{
				this.OrderSetsWithOrdersByType[OrderSetType.Toggle].ResetActiveStatus(flag);
			}
			if (this.OrderSetsWithOrdersByType.ContainsKey(OrderSetType.Facing))
			{
				this.OrderSetsWithOrdersByType[OrderSetType.Facing].ResetActiveStatus(flag);
			}
			foreach (OrderSetVM orderSetVM in this.OrderSets.Where((OrderSetVM s) => !s.ContainsOrders))
			{
				orderSetVM.ResetActiveStatus(false);
			}
			if (list.Count > 0)
			{
				List<OrderItemVM> list2 = new List<OrderItemVM>();
				foreach (OrderSubjectVM orderSubjectVM in list)
				{
					foreach (OrderItemVM orderItemVM in orderSubjectVM.ActiveOrders)
					{
						if (!list2.Contains(orderItemVM))
						{
							list2.Add(orderItemVM);
						}
					}
				}
				foreach (OrderItemVM orderItemVM2 in list2)
				{
					orderItemVM2.SelectionState = 2;
					if (orderItemVM2.IsTitle)
					{
						orderItemVM2.SetActiveState(true);
					}
					if (orderItemVM2.OrderSetType != OrderSetType.None)
					{
						this.OrderSetsWithOrdersByType[orderItemVM2.OrderSetType].SetActiveOrder(this.OrderSetsWithOrdersByType[orderItemVM2.OrderSetType].TitleOrder);
					}
				}
				list2 = list[0].ActiveOrders;
				foreach (OrderSubjectVM orderSubjectVM2 in list)
				{
					list2 = list2.Intersect(orderSubjectVM2.ActiveOrders).ToList<OrderItemVM>();
					if (list2.IsEmpty<OrderItemVM>())
					{
						break;
					}
				}
				foreach (OrderItemVM orderItemVM3 in list2)
				{
					orderItemVM3.SelectionState = 3;
					if (orderItemVM3.OrderSetType != OrderSetType.None)
					{
						this.OrderSetsWithOrdersByType[orderItemVM3.OrderSetType].SetActiveOrder(orderItemVM3);
					}
				}
			}
			this.OrderSetsWithOrdersByType[OrderSetType.Movement].FinalizeActiveStatus(false);
			this.OrderSetsWithOrdersByType[OrderSetType.Form].FinalizeActiveStatus(flag && list.IsEmpty<OrderSubjectVM>());
			if (this.OrderSetsWithOrdersByType.ContainsKey(OrderSetType.Toggle))
			{
				this.OrderSetsWithOrdersByType[OrderSetType.Toggle].FinalizeActiveStatus(flag);
			}
			if (this.OrderSetsWithOrdersByType.ContainsKey(OrderSetType.Facing))
			{
				this.OrderSetsWithOrdersByType[OrderSetType.Facing].FinalizeActiveStatus(flag);
			}
			foreach (OrderSetVM orderSetVM2 in this.OrderSets.Where((OrderSetVM s) => !s.ContainsOrders))
			{
				orderSetVM2.FinalizeActiveStatus(false);
			}
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000AC8C File Offset: 0x00008E8C
		private void OnOrder(OrderItemVM orderItem, OrderSetType orderSetType, bool fromSelection)
		{
			if (this.LastSelectedOrderItem != orderItem || !fromSelection)
			{
				bool flag = false;
				this._onBeforeOrderDelegate();
				if (this.LastSelectedOrderItem != null)
				{
					this.LastSelectedOrderItem.IsSelected = false;
				}
				if (orderItem.IsTitle)
				{
					this.LastSelectedOrderSetType = orderSetType;
					flag = true;
				}
				this.LastSelectedOrderItem = orderItem;
				if (this.LastSelectedOrderItem != null)
				{
					this.LastSelectedOrderItem.IsSelected = true;
					if (this.LastSelectedOrderItem.OrderSubType == OrderSubType.None)
					{
						this.OrderSetsWithOrdersByType[this.LastSelectedOrderSetType].ShowOrders = false;
						this.LastSelectedOrderSetType = orderSetType;
						this.OrderSetsWithOrdersByType[this.LastSelectedOrderSetType].ShowOrders = true;
					}
				}
				if (orderSetType == this.LastSelectedOrderSetType)
				{
					this.LastSelectedOrderSetType = orderItem.OrderSetType;
				}
				if (this.LastSelectedOrderItem != null && this.LastSelectedOrderItem.OrderSubType != OrderSubType.None && !fromSelection)
				{
					OrderSetVM orderSetVM;
					if (this.LastSelectedOrderItem.OrderSubType == OrderSubType.Return && this.OrderSetsWithOrdersByType.TryGetValue(this.LastSelectedOrderSetType, out orderSetVM))
					{
						this.UpdateTitleOrdersKeyVisualVisibility(false);
						orderSetVM.ShowOrders = false;
						this.LastSelectedOrderSetType = OrderSetType.None;
					}
					else if (this._currentActivationType == MissionOrderVM.ActivationType.Hold && this.LastSelectedOrderSetType != OrderSetType.None)
					{
						this.ApplySelectedOrder();
						if (this.LastSelectedOrderItem != null && this.LastSelectedOrderSetType != OrderSetType.None)
						{
							this.OrderSetsWithOrdersByType[this.LastSelectedOrderSetType].ShowOrders = false;
							this.LastSelectedOrderSetType = OrderSetType.None;
							this.LastSelectedOrderItem = null;
						}
						this.OrderSets.ApplyActionOnAllItems(delegate(OrderSetVM s)
						{
							s.Orders.ApplyActionOnAllItems(delegate(OrderItemVM o)
							{
								o.IsSelected = false;
							});
						});
					}
					else if (this.IsDeployment)
					{
						this.ApplySelectedOrder();
					}
					else
					{
						this.TryCloseToggleOrder(false);
					}
				}
				if (!fromSelection)
				{
					this.UpdateTitleOrdersKeyVisualVisibility(flag);
				}
			}
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000AE4C File Offset: 0x0000904C
		private void UpdateTitleOrdersKeyVisualVisibility(bool isTitleOrderSelected)
		{
			for (int i = 0; i < this.OrderSets.Count; i++)
			{
				this.OrderSets[i].TitleOrder.ShortcutKey.SetForcedVisibility(isTitleOrderSelected ? new bool?(false) : null);
			}
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000AEA0 File Offset: 0x000090A0
		public void ApplySelectedOrder()
		{
			bool flag = this._isPressedViewOrders || (this.LastSelectedOrderItem.OrderSetType == OrderSetType.None && this.LastSelectedOrderItem.IsTitle) || !this.LastSelectedOrderItem.IsTitle;
			if (this.LastSelectedOrderItem == null)
			{
				return;
			}
			if (this.LastSelectedOrderItem.OrderSubType == OrderSubType.Return)
			{
				OrderSetVM orderSetVM;
				if (this.OrderSetsWithOrdersByType.TryGetValue(this.LastSelectedOrderSetType, out orderSetVM))
				{
					this.UpdateTitleOrdersKeyVisualVisibility(false);
					orderSetVM.ShowOrders = false;
					this.LastSelectedOrderSetType = OrderSetType.None;
				}
				else
				{
					this.TryCloseToggleOrder(true);
				}
				this.LastSelectedOrderItem = null;
				return;
			}
			List<TextObject> list = new List<TextObject>();
			if (this.OrderController.SelectedFormations.Count == 0 && this.ActiveTargetState == 0)
			{
				this.TroopController.UpdateTroops();
				this.TroopController.SelectAllFormations(true);
			}
			if (this.LastSelectedOrderItem.OrderSubType != OrderSubType.ToggleTransfer && flag)
			{
				if (this.ActiveTargetState == 1)
				{
					using (IEnumerator<OrderSiegeMachineVM> enumerator = this.DeploymentController.SiegeMachineList.Where((OrderSiegeMachineVM item) => item.IsSelected && ((this.LastSelectedOrderItem.OrderSetType != OrderSetType.Toggle && this.LastSelectedOrderItem.OrderSubType != OrderSubType.ToggleFacing) || !item.IsPrimarySiegeMachine)).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							OrderSiegeMachineVM orderSiegeMachineVM = enumerator.Current;
							list.Add(GameTexts.FindText("str_siege_engine", orderSiegeMachineVM.MachineClass));
						}
						goto IL_1B5;
					}
				}
				foreach (OrderTroopItemVM orderTroopItemVM in this.TroopController.TroopList.Where((OrderTroopItemVM item) => item.IsSelected))
				{
					list.Add(GameTexts.FindText("str_formation_class_string", orderTroopItemVM.Formation.PrimaryClass.GetName()));
				}
			}
			IL_1B5:
			if (!list.IsEmpty<TextObject>())
			{
				TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
				textObject.SetTextVariable("STR1", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false));
				textObject.SetTextVariable("STR2", this.LastSelectedOrderItem.TooltipText);
				InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
			}
			if (this.LastSelectedOrderSetType != OrderSetType.None)
			{
				this.OrderSetsWithOrdersByType[this.LastSelectedOrderSetType].ShowOrders = false;
				foreach (OrderSetVM orderSetVM2 in this.OrderSetsWithOrdersByType.Values)
				{
					orderSetVM2.TitleOrder.IsActive = orderSetVM2.TitleOrder.SelectionState != 0;
				}
			}
			if (this.ActiveTargetState == 0)
			{
				switch (this.LastSelectedOrderItem.OrderSubType)
				{
				case OrderSubType.None:
				case OrderSubType.Return:
					return;
				case OrderSubType.MoveToPosition:
				{
					Vec3 vec = this._getOrderFlagPosition();
					WorldPosition worldPosition = new WorldPosition(this.Mission.Scene, UIntPtr.Zero, vec, false);
					if (this.Mission.IsFormationUnitPositionAvailable(ref worldPosition, this.Team))
					{
						this.OrderController.SetOrderWithTwoPositions(OrderType.MoveToLineSegment, worldPosition, worldPosition);
						goto IL_7C8;
					}
					goto IL_7C8;
				}
				case OrderSubType.Charge:
					this.OrderController.SetOrder(OrderType.Charge);
					goto IL_7C8;
				case OrderSubType.FollowMe:
					this.OrderController.SetOrderWithAgent(OrderType.FollowMe, Agent.Main);
					goto IL_7C8;
				case OrderSubType.Advance:
					this.OrderController.SetOrder(OrderType.Advance);
					goto IL_7C8;
				case OrderSubType.Fallback:
					this.OrderController.SetOrder(OrderType.FallBack);
					goto IL_7C8;
				case OrderSubType.Stop:
					this.OrderController.SetOrder(OrderType.StandYourGround);
					goto IL_7C8;
				case OrderSubType.Retreat:
					this.OrderController.SetOrder(OrderType.Retreat);
					goto IL_7C8;
				case OrderSubType.FormLine:
					this.OrderController.SetOrder(OrderType.ArrangementLine);
					goto IL_7C8;
				case OrderSubType.FormClose:
					this.OrderController.SetOrder(OrderType.ArrangementCloseOrder);
					goto IL_7C8;
				case OrderSubType.FormLoose:
					this.OrderController.SetOrder(OrderType.ArrangementLoose);
					goto IL_7C8;
				case OrderSubType.FormCircular:
					this.OrderController.SetOrder(OrderType.ArrangementCircular);
					goto IL_7C8;
				case OrderSubType.FormSchiltron:
					this.OrderController.SetOrder(OrderType.ArrangementSchiltron);
					goto IL_7C8;
				case OrderSubType.FormV:
					this.OrderController.SetOrder(OrderType.ArrangementVee);
					goto IL_7C8;
				case OrderSubType.FormColumn:
					this.OrderController.SetOrder(OrderType.ArrangementColumn);
					goto IL_7C8;
				case OrderSubType.FormScatter:
					this.OrderController.SetOrder(OrderType.ArrangementScatter);
					goto IL_7C8;
				case OrderSubType.ToggleStart:
				case OrderSubType.ToggleEnd:
					goto IL_7C8;
				case OrderSubType.ToggleFacing:
					if (OrderController.GetActiveFacingOrderOf(this.OrderController.SelectedFormations.FirstOrDefault<Formation>()) == OrderType.LookAtDirection)
					{
						this.OrderController.SetOrder(OrderType.LookAtEnemy);
						goto IL_7C8;
					}
					this.OrderController.SetOrderWithPosition(OrderType.LookAtDirection, new WorldPosition(this.Mission.Scene, UIntPtr.Zero, this._getOrderFlagPosition(), false));
					goto IL_7C8;
				case OrderSubType.ToggleFire:
					if (this.LastSelectedOrderItem.SelectionState == 3)
					{
						this.OrderController.SetOrder(OrderType.FireAtWill);
						goto IL_7C8;
					}
					this.OrderController.SetOrder(OrderType.HoldFire);
					goto IL_7C8;
				case OrderSubType.ToggleMount:
					if (this.LastSelectedOrderItem.SelectionState == 3)
					{
						this.OrderController.SetOrder(OrderType.Mount);
						goto IL_7C8;
					}
					this.OrderController.SetOrder(OrderType.Dismount);
					goto IL_7C8;
				case OrderSubType.ToggleAI:
				{
					if (this.LastSelectedOrderItem.SelectionState == 3)
					{
						this.OrderController.SetOrder(OrderType.AIControlOff);
						goto IL_7C8;
					}
					this.OrderController.SetOrder(OrderType.AIControlOn);
					using (List<Formation>.Enumerator enumerator4 = this.OrderController.SelectedFormations.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							Formation formation = enumerator4.Current;
							this.TeamOnFormationAIActiveBehaviorChanged(formation);
						}
						goto IL_7C8;
					}
					break;
				}
				case OrderSubType.ToggleTransfer:
					break;
				case OrderSubType.ActivationFaceDirection:
					this.OrderController.SetOrderWithPosition(OrderType.LookAtDirection, new WorldPosition(this.Mission.Scene, UIntPtr.Zero, this._getOrderFlagPosition(), false));
					goto IL_7C8;
				case OrderSubType.FaceEnemy:
					this.OrderController.SetOrder(OrderType.LookAtEnemy);
					goto IL_7C8;
				default:
					goto IL_7C8;
				}
				if (!this.IsDeployment)
				{
					foreach (OrderTroopItemVM orderTroopItemVM2 in this.TroopController.TransferTargetList)
					{
						orderTroopItemVM2.IsSelected = false;
						orderTroopItemVM2.IsSelectable = !this.OrderController.IsFormationListening(orderTroopItemVM2.Formation);
					}
					OrderTroopItemVM orderTroopItemVM3 = this.TroopController.TransferTargetList.FirstOrDefault((OrderTroopItemVM t) => t.IsSelectable);
					if (orderTroopItemVM3 != null)
					{
						orderTroopItemVM3.IsSelected = true;
						this.TroopController.IsTransferValid = true;
						GameTexts.SetVariable("{FORMATION_INDEX}", Common.ToRoman(orderTroopItemVM3.Formation.Index + 1));
						this.TroopController.TransferTitleText = new TextObject("{=DvnRkWQg}Transfer Troops To {FORMATION_INDEX}", null).ToString();
						this.TroopController.IsTransferActive = true;
						this.TroopController.IsTransferValid = false;
						this.TroopController.TransferMaxValue = this.TroopController.TroopList.Where((OrderTroopItemVM t) => t.IsSelected).Sum((OrderTroopItemVM t) => t.CurrentMemberCount);
						this.TroopController.TransferValue = this.TroopController.TransferMaxValue;
						this.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
					}
					else
					{
						MBInformationManager.AddQuickInformation(new TextObject("{=SLY8z9fP}All formations are selected!", null), 0, null, "");
					}
				}
			}
			else
			{
				OrderSubType orderSubType = this.LastSelectedOrderItem.OrderSubType;
				if (orderSubType <= OrderSubType.Stop)
				{
					if (orderSubType != OrderSubType.MoveToPosition)
					{
						if (orderSubType == OrderSubType.Stop)
						{
							this.OrderController.SiegeWeaponController.SetOrder(SiegeWeaponOrderType.Stop);
						}
					}
					else
					{
						this.OrderController.SiegeWeaponController.SetOrder(SiegeWeaponOrderType.Attack);
					}
				}
				else if (orderSubType != OrderSubType.ToggleFacing)
				{
					if (orderSubType == OrderSubType.Return)
					{
						return;
					}
				}
				else
				{
					this.OrderController.SiegeWeaponController.SetOrder(SiegeWeaponOrderType.FireAtWalls);
				}
			}
			IL_7C8:
			if (this.ActiveTargetState == 0)
			{
				using (IEnumerator<OrderTroopItemVM> enumerator2 = this.TroopController.TroopList.Where((OrderTroopItemVM item) => item.IsSelected).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						OrderTroopItemVM orderTroopItemVM4 = enumerator2.Current;
						this.TroopController.SetTroopActiveOrders(orderTroopItemVM4);
					}
					goto IL_895;
				}
			}
			foreach (OrderSiegeMachineVM orderSiegeMachineVM2 in this.DeploymentController.SiegeMachineList.Where((OrderSiegeMachineVM item) => item.IsSelected))
			{
				this.DeploymentController.SetSiegeMachineActiveOrders(orderSiegeMachineVM2);
			}
			IL_895:
			this.UpdateTitleOrdersKeyVisualVisibility(false);
			this.LastSelectedOrderItem = null;
			this.LastSelectedOrderSetType = OrderSetType.None;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000B7B0 File Offset: 0x000099B0
		public void AfterInitialize()
		{
			this.TroopController.UpdateTroops();
			if (!this.IsDeployment)
			{
				this.TroopController.SelectAllFormations(false);
			}
			this.DeploymentController.SetCurrentActiveOrders();
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000B7DC File Offset: 0x000099DC
		public void Update()
		{
			if (this.IsToggleOrderShown)
			{
				if (!this.CheckCanBeOpened(false))
				{
					if (this.IsToggleOrderShown)
					{
						this.TryCloseToggleOrder(false);
					}
				}
				else if (this._updateTroopsTimer.Check(MBCommon.GetApplicationTime()))
				{
					this.TroopController.IntervalUpdate();
				}
			}
			this.DeploymentController.Update();
			this.DisplayFormationAIFeedback();
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000B83C File Offset: 0x00009A3C
		public void OnEscape()
		{
			if (this.IsToggleOrderShown)
			{
				if (this._currentActivationType == MissionOrderVM.ActivationType.Hold)
				{
					if (this.LastSelectedOrderItem != null)
					{
						this.UpdateTitleOrdersKeyVisualVisibility(false);
						this.OrderSetsWithOrdersByType[this.LastSelectedOrderSetType].ShowOrders = false;
						this.LastSelectedOrderItem = null;
						return;
					}
				}
				else if (this._currentActivationType == MissionOrderVM.ActivationType.Click)
				{
					this.LastSelectedOrderItem = null;
					if (this.LastSelectedOrderSetType != OrderSetType.None)
					{
						this.UpdateTitleOrdersKeyVisualVisibility(false);
						this.OrderSetsWithOrdersByType[this.LastSelectedOrderSetType].ShowOrders = false;
						this.LastSelectedOrderSetType = OrderSetType.None;
						return;
					}
					this.LastSelectedOrderSetType = OrderSetType.None;
					this.TryCloseToggleOrder(false);
				}
			}
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000B8D9 File Offset: 0x00009AD9
		public void ViewOrders()
		{
			this._isPressedViewOrders = true;
			if (!this.IsToggleOrderShown)
			{
				this.TroopController.UpdateTroops();
				this.OpenToggleOrder(false, true);
			}
			else
			{
				this.TryCloseToggleOrder(false);
			}
			this._isPressedViewOrders = false;
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000B90E File Offset: 0x00009B0E
		public void OnSelect(int formationTroopIndex)
		{
			if (!this.CheckCanBeOpened(true))
			{
				return;
			}
			if (this.ActiveTargetState == 0)
			{
				this.TroopController.OnSelectFormationWithIndex(formationTroopIndex);
			}
			else if (this.ActiveTargetState == 1)
			{
				this.DeploymentController.OnSelectFormationWithIndex(formationTroopIndex);
			}
			this.OpenToggleOrder(false, true);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000B950 File Offset: 0x00009B50
		public void OnGiveOrder(int pressedIndex)
		{
			if (!this.CheckCanBeOpened(true))
			{
				return;
			}
			OrderSetVM orderSetVM = this.OrderSetsWithOrdersByType.Values.FirstOrDefault((OrderSetVM o) => o.ShowOrders);
			if (orderSetVM != null)
			{
				if (orderSetVM.Orders.Count > pressedIndex)
				{
					orderSetVM.Orders[pressedIndex].ExecuteAction();
					return;
				}
				if (pressedIndex == 8)
				{
					orderSetVM.Orders[orderSetVM.Orders.Count - 1].ExecuteAction();
					return;
				}
			}
			else
			{
				int num = (int)MathF.Clamp((float)pressedIndex, 0f, (float)(this.OrderSets.Count - 1));
				if (num >= 0 && this.OrderSets.Count > num && this.OrderSets[num].TitleOrder.SelectionState != 0)
				{
					this.OpenToggleOrder(false, true);
					this.OrderSets[num].TitleOrder.ExecuteAction();
				}
			}
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000BA43 File Offset: 0x00009C43
		private void MissionOnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.Mission.MainAgent == null)
			{
				this.TryCloseToggleOrder(false);
				this.Mission.IsOrderMenuOpen = false;
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000BA68 File Offset: 0x00009C68
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Mission.OnMainAgentChanged -= this.MissionOnMainAgentChanged;
			this.DeploymentController.OnFinalize();
			this.TroopController.OnFinalize();
			this._deploymentPoints.Clear();
			foreach (OrderSetVM orderSetVM in this._orderSets)
			{
				orderSetVM.OnFinalize();
			}
			this.InputRestrictions = null;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000BAF8 File Offset: 0x00009CF8
		internal void OnDeployAll()
		{
			this.TroopController.UpdateTroops();
			foreach (OrderTroopItemVM orderTroopItemVM in this.TroopController.TroopList)
			{
				this.TroopController.SetTroopActiveOrders(orderTroopItemVM);
			}
			if (!this.IsDeployment)
			{
				this.TroopController.SelectAllFormations(false);
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000BB70 File Offset: 0x00009D70
		private void OnOrderShownToggle()
		{
			this.IsTroopListShown = this.IsToggleOrderShown && !this.IsDeployment;
			if (!this._isDeployment)
			{
				if (this.IsToggleOrderShown)
				{
					this._onActivateToggleOrder();
				}
				else
				{
					this._onDeactivateToggleOrder();
				}
			}
			this._updateTroopsTimer = (this.IsToggleOrderShown ? new Timer(MBCommon.GetApplicationTime() - 2f, 2f, true) : null);
			this.IsTroopPlacingActive = this.IsToggleOrderShown && this.ActiveTargetState == 0;
			foreach (OrderSetVM orderSetVM in this.OrderSetsWithOrdersByType.Values)
			{
				orderSetVM.ShowOrders = false;
				orderSetVM.TitleOrder.IsActive = orderSetVM.TitleOrder.SelectionState != 0;
			}
			if (!this.IsDeployment && this.TroopController.TroopList.FirstOrDefault<OrderTroopItemVM>() != null)
			{
				this.TroopController.TroopList.ApplyActionOnAllItems(delegate(OrderTroopItemVM t)
				{
					t.IsSelectionActive = false;
				});
				this.TroopController.TroopList[0].IsSelectionActive = true;
			}
			this._onRefreshVisuals();
			if (!this.IsToggleOrderShown)
			{
				this._currentActivationType = MissionOrderVM.ActivationType.NotActive;
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000BCE0 File Offset: 0x00009EE0
		public void SelectNextTroop(int direction)
		{
			if (!this.CheckCanBeOpened(true))
			{
				return;
			}
			if (this.TroopController.TroopList.Count > 0)
			{
				OrderTroopItemVM orderTroopItemVM = this.TroopController.TroopList.FirstOrDefault((OrderTroopItemVM t) => t.IsSelectionActive);
				if (orderTroopItemVM != null)
				{
					int num = ((direction > 0) ? 1 : (-1));
					orderTroopItemVM.IsSelectionActive = false;
					int num2 = this.TroopController.TroopList.IndexOf(orderTroopItemVM) + num;
					for (int i = 0; i < this.TroopController.TroopList.Count; i++)
					{
						int num3 = (num2 + i * num) % this.TroopController.TroopList.Count;
						if (num3 < 0)
						{
							num3 += this.TroopController.TroopList.Count;
						}
						if (this.TroopController.TroopList[num3].IsSelectable)
						{
							this.TroopController.TroopList[num3].IsSelectionActive = true;
							return;
						}
					}
					return;
				}
				this.TroopController.TroopList.FirstOrDefault<OrderTroopItemVM>().IsSelectionActive = true;
			}
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000BE00 File Offset: 0x0000A000
		public void ToggleSelectionForCurrentTroop()
		{
			if (!this.CheckCanBeOpened(true))
			{
				return;
			}
			OrderTroopItemVM orderTroopItemVM = this.TroopController.TroopList.FirstOrDefault((OrderTroopItemVM t) => t.IsSelectionActive);
			if (orderTroopItemVM != null)
			{
				if (orderTroopItemVM.IsSelected)
				{
					this.TroopController.OnDeselectFormation(orderTroopItemVM);
					return;
				}
				this.TroopController.OnSelectFormation(orderTroopItemVM);
			}
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000BE6B File Offset: 0x0000A06B
		private void OnTransferFinished()
		{
			this._onTransferTroopsFinishedDelegate.DynamicInvokeWithLog(Array.Empty<object>());
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000BE7E File Offset: 0x0000A07E
		internal OrderSetType GetOrderSetWithShortcutIndex(int index)
		{
			switch (index)
			{
			case 0:
				return OrderSetType.Movement;
			case 1:
				return OrderSetType.Form;
			case 2:
				return OrderSetType.Toggle;
			case 3:
				return OrderSetType.Facing;
			default:
				return (OrderSetType)index;
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000BEA4 File Offset: 0x0000A0A4
		internal IEnumerable<OrderItemVM> GetAllOrderItemsForSubType(OrderSubType orderSubType)
		{
			Func<OrderItemVM, bool> <>9__4;
			IEnumerable<OrderItemVM> enumerable = this.OrderSets.Select((OrderSetVM s) => s.Orders).SelectMany(delegate(MBBindingList<OrderItemVM> o)
			{
				Func<OrderItemVM, bool> func;
				if ((func = <>9__4) == null)
				{
					func = (<>9__4 = (OrderItemVM l) => l.OrderSubType == orderSubType);
				}
				return o.Where(func);
			});
			IEnumerable<OrderItemVM> enumerable2 = from s in this.OrderSets
				where s.TitleOrder.OrderSubType == orderSubType
				select s into t
				select t.TitleOrder;
			return enumerable.Union(enumerable2);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000BF3C File Offset: 0x0000A13C
		[Conditional("DEBUG")]
		private void DebugTick()
		{
			if (this.IsToggleOrderShown)
			{
				string text = "SelectedFormations (" + this.OrderController.SelectedFormations.Count + ") :";
				foreach (Formation formation in this.OrderController.SelectedFormations)
				{
					text = text + " " + formation.FormationIndex.GetName();
				}
			}
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000BFD4 File Offset: 0x0000A1D4
		internal static OrderType GetOrderOverrideForUI(Formation formation, OrderSetType setType)
		{
			OrderType overridenOrderType = formation.Team.PlayerOrderController.GetOverridenOrderType(formation);
			switch (overridenOrderType)
			{
			case OrderType.Move:
			case OrderType.Charge:
			case OrderType.StandYourGround:
			case OrderType.FollowMe:
			case OrderType.GuardMe:
			case OrderType.Retreat:
			case OrderType.Advance:
			case OrderType.FallBack:
				if (setType == OrderSetType.Movement)
				{
					return overridenOrderType;
				}
				break;
			case OrderType.LookAtEnemy:
			case OrderType.LookAtDirection:
			case OrderType.HoldFire:
			case OrderType.FireAtWill:
			case OrderType.Mount:
			case OrderType.Dismount:
			case OrderType.AIControlOn:
			case OrderType.AIControlOff:
				if (setType == OrderSetType.Toggle)
				{
					return overridenOrderType;
				}
				break;
			case OrderType.ArrangementLine:
			case OrderType.ArrangementCloseOrder:
			case OrderType.ArrangementLoose:
			case OrderType.ArrangementCircular:
			case OrderType.ArrangementSchiltron:
			case OrderType.ArrangementVee:
			case OrderType.ArrangementColumn:
			case OrderType.ArrangementScatter:
				if (setType == OrderSetType.Form)
				{
					return overridenOrderType;
				}
				break;
			}
			return OrderType.None;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000C0AF File Offset: 0x0000A2AF
		public void OnDeploymentFinished()
		{
			this.TroopController.OnDeploymentFinished();
			this.DeploymentController.FinalizeDeployment();
			this.IsDeployment = false;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000C0CE File Offset: 0x0000A2CE
		public void OnFiltersSet(List<ValueTuple<int, List<int>>> filterData)
		{
			this._filterData = filterData;
			this.TroopController.OnFiltersSet(filterData);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000C0E3 File Offset: 0x0000A2E3
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x0600026D RID: 621 RVA: 0x0000C0F2 File Offset: 0x0000A2F2
		// (set) Token: 0x0600026E RID: 622 RVA: 0x0000C0FA File Offset: 0x0000A2FA
		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600026F RID: 623 RVA: 0x0000C118 File Offset: 0x0000A318
		// (set) Token: 0x06000270 RID: 624 RVA: 0x0000C120 File Offset: 0x0000A320
		[DataSourceProperty]
		public MBBindingList<OrderSetVM> OrderSets
		{
			get
			{
				return this._orderSets;
			}
			set
			{
				if (value == this._orderSets)
				{
					return;
				}
				this._orderSets = value;
				base.OnPropertyChangedWithValue<MBBindingList<OrderSetVM>>(value, "OrderSets");
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000271 RID: 625 RVA: 0x0000C13F File Offset: 0x0000A33F
		// (set) Token: 0x06000272 RID: 626 RVA: 0x0000C147 File Offset: 0x0000A347
		[DataSourceProperty]
		public MissionOrderTroopControllerVM TroopController
		{
			get
			{
				return this._troopController;
			}
			set
			{
				if (value == this._troopController)
				{
					return;
				}
				this._troopController = value;
				base.OnPropertyChangedWithValue<MissionOrderTroopControllerVM>(value, "TroopController");
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000273 RID: 627 RVA: 0x0000C166 File Offset: 0x0000A366
		// (set) Token: 0x06000274 RID: 628 RVA: 0x0000C16E File Offset: 0x0000A36E
		[DataSourceProperty]
		public MissionOrderDeploymentControllerVM DeploymentController
		{
			get
			{
				return this._deploymentController;
			}
			set
			{
				if (value == this._deploymentController)
				{
					return;
				}
				this._deploymentController = value;
				base.OnPropertyChangedWithValue<MissionOrderDeploymentControllerVM>(value, "DeploymentController");
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000275 RID: 629 RVA: 0x0000C18D File Offset: 0x0000A38D
		// (set) Token: 0x06000276 RID: 630 RVA: 0x0000C198 File Offset: 0x0000A398
		[DataSourceProperty]
		public int ActiveTargetState
		{
			get
			{
				return this._activeTargetState;
			}
			set
			{
				if (value == this._activeTargetState)
				{
					return;
				}
				this._activeTargetState = value;
				base.OnPropertyChangedWithValue(value, "ActiveTargetState");
				this.IsTroopPlacingActive = value == 0;
				foreach (OrderSetVM orderSetVM in this.OrderSetsWithOrdersByType.Values)
				{
					orderSetVM.ShowOrders = false;
					orderSetVM.TitleOrder.IsActive = orderSetVM.TitleOrder.SelectionState != 0;
				}
				this._onRefreshVisuals();
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000277 RID: 631 RVA: 0x0000C23C File Offset: 0x0000A43C
		// (set) Token: 0x06000278 RID: 632 RVA: 0x0000C244 File Offset: 0x0000A444
		[DataSourceProperty]
		public bool IsDeployment
		{
			get
			{
				return this._isDeployment;
			}
			set
			{
				this._isDeployment = value;
				base.OnPropertyChangedWithValue(value, "IsDeployment");
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000279 RID: 633 RVA: 0x0000C259 File Offset: 0x0000A459
		// (set) Token: 0x0600027A RID: 634 RVA: 0x0000C261 File Offset: 0x0000A461
		[DataSourceProperty]
		public bool IsToggleOrderShown
		{
			get
			{
				return this._isToggleOrderShown;
			}
			set
			{
				if (value == this._isToggleOrderShown)
				{
					return;
				}
				this._isToggleOrderShown = value;
				this.OnOrderShownToggle();
				base.OnPropertyChangedWithValue(value, "IsToggleOrderShown");
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600027B RID: 635 RVA: 0x0000C286 File Offset: 0x0000A486
		// (set) Token: 0x0600027C RID: 636 RVA: 0x0000C28E File Offset: 0x0000A48E
		[DataSourceProperty]
		public bool IsTroopListShown
		{
			get
			{
				return this._isTroopListShown;
			}
			set
			{
				if (value == this._isTroopListShown)
				{
					return;
				}
				this._isTroopListShown = value;
				base.OnPropertyChangedWithValue(value, "IsTroopListShown");
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600027D RID: 637 RVA: 0x0000C2AD File Offset: 0x0000A4AD
		// (set) Token: 0x0600027E RID: 638 RVA: 0x0000C2B8 File Offset: 0x0000A4B8
		[DataSourceProperty]
		public bool CanUseShortcuts
		{
			get
			{
				return this._canUseShortcuts;
			}
			set
			{
				if (value != this._canUseShortcuts)
				{
					this._canUseShortcuts = value;
					base.OnPropertyChangedWithValue(value, "CanUseShortcuts");
					for (int i = 0; i < this.OrderSets.Count; i++)
					{
						this.OrderSets[i].CanUseShortcuts = value;
					}
				}
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600027F RID: 639 RVA: 0x0000C309 File Offset: 0x0000A509
		// (set) Token: 0x06000280 RID: 640 RVA: 0x0000C311 File Offset: 0x0000A511
		[DataSourceProperty]
		public bool IsHolding
		{
			get
			{
				return this._isHolding;
			}
			set
			{
				if (value != this._isHolding)
				{
					this._isHolding = value;
					base.OnPropertyChangedWithValue(value, "IsHolding");
				}
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000281 RID: 641 RVA: 0x0000C32F File Offset: 0x0000A52F
		// (set) Token: 0x06000282 RID: 642 RVA: 0x0000C337 File Offset: 0x0000A537
		[DataSourceProperty]
		public bool IsAnyOrderSetActive
		{
			get
			{
				return this._isAnyOrderSetActive;
			}
			set
			{
				if (value != this._isAnyOrderSetActive)
				{
					this._isAnyOrderSetActive = value;
					base.OnPropertyChangedWithValue(value, "IsAnyOrderSetActive");
				}
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000283 RID: 643 RVA: 0x0000C355 File Offset: 0x0000A555
		// (set) Token: 0x06000284 RID: 644 RVA: 0x0000C35D File Offset: 0x0000A55D
		[DataSourceProperty]
		public string ReturnText
		{
			get
			{
				return this._returnText;
			}
			set
			{
				if (value != this._returnText)
				{
					this._returnText = value;
					base.OnPropertyChangedWithValue<string>(value, "ReturnText");
				}
			}
		}

		// Token: 0x040000F7 RID: 247
		public InputRestrictions InputRestrictions;

		// Token: 0x040000F8 RID: 248
		private MissionOrderVM.ActivationType _currentActivationType;

		// Token: 0x040000F9 RID: 249
		private Timer _updateTroopsTimer;

		// Token: 0x040000FA RID: 250
		internal readonly Dictionary<OrderSetType, OrderSetVM> OrderSetsWithOrdersByType;

		// Token: 0x040000FB RID: 251
		private readonly Camera _deploymentCamera;

		// Token: 0x040000FC RID: 252
		private bool _isTroopPlacingActive;

		// Token: 0x040000FD RID: 253
		private OrderSetType _lastSelectedOrderSetType;

		// Token: 0x040000FE RID: 254
		private bool _isPressedViewOrders;

		// Token: 0x040000FF RID: 255
		private readonly OnToggleActivateOrderStateDelegate _onActivateToggleOrder;

		// Token: 0x04000100 RID: 256
		private readonly OnToggleActivateOrderStateDelegate _onDeactivateToggleOrder;

		// Token: 0x04000101 RID: 257
		private readonly GetOrderFlagPositionDelegate _getOrderFlagPosition;

		// Token: 0x04000102 RID: 258
		private readonly ToggleOrderPositionVisibilityDelegate _toggleOrderPositionVisibility;

		// Token: 0x04000103 RID: 259
		private readonly OnRefreshVisualsDelegate _onRefreshVisuals;

		// Token: 0x04000104 RID: 260
		private readonly OnToggleActivateOrderStateDelegate _onTransferTroopsFinishedDelegate;

		// Token: 0x04000105 RID: 261
		private readonly OnBeforeOrderDelegate _onBeforeOrderDelegate;

		// Token: 0x04000106 RID: 262
		private readonly Action<bool> _toggleMissionInputs;

		// Token: 0x04000107 RID: 263
		private readonly List<DeploymentPoint> _deploymentPoints;

		// Token: 0x04000108 RID: 264
		private readonly bool _isMultiplayer;

		// Token: 0x04000109 RID: 265
		private OrderSetVM _movementSet;

		// Token: 0x0400010A RID: 266
		private OrderSetVM _facingSet;

		// Token: 0x0400010B RID: 267
		private int _delayValueForAIFormationModifications;

		// Token: 0x0400010C RID: 268
		private readonly List<Formation> _modifiedAIFormations = new List<Formation>();

		// Token: 0x0400010D RID: 269
		private List<ValueTuple<int, List<int>>> _filterData;

		// Token: 0x0400010E RID: 270
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x0400010F RID: 271
		private MBBindingList<OrderSetVM> _orderSets;

		// Token: 0x04000110 RID: 272
		private MissionOrderTroopControllerVM _troopController;

		// Token: 0x04000111 RID: 273
		private MissionOrderDeploymentControllerVM _deploymentController;

		// Token: 0x04000112 RID: 274
		private bool _isDeployment;

		// Token: 0x04000113 RID: 275
		private int _activeTargetState;

		// Token: 0x04000114 RID: 276
		private bool _isToggleOrderShown;

		// Token: 0x04000115 RID: 277
		private bool _isTroopListShown;

		// Token: 0x04000116 RID: 278
		private bool _canUseShortcuts;

		// Token: 0x04000117 RID: 279
		private bool _isHolding;

		// Token: 0x04000118 RID: 280
		private bool _isAnyOrderSetActive;

		// Token: 0x04000119 RID: 281
		private string _returnText;

		// Token: 0x0200013B RID: 315
		public enum CursorState
		{
			// Token: 0x04000BCA RID: 3018
			Move,
			// Token: 0x04000BCB RID: 3019
			Face,
			// Token: 0x04000BCC RID: 3020
			Form
		}

		// Token: 0x0200013C RID: 316
		public enum OrderTargets
		{
			// Token: 0x04000BCE RID: 3022
			Troops,
			// Token: 0x04000BCF RID: 3023
			SiegeMachines
		}

		// Token: 0x0200013D RID: 317
		public enum ActivationType
		{
			// Token: 0x04000BD1 RID: 3025
			NotActive,
			// Token: 0x04000BD2 RID: 3026
			Hold,
			// Token: 0x04000BD3 RID: 3027
			Click
		}
	}
}
