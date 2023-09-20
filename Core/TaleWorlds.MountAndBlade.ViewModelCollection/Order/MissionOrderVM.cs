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
	public class MissionOrderVM : ViewModel
	{
		private Team Team
		{
			get
			{
				return Mission.Current.PlayerTeam;
			}
		}

		public OrderItemVM LastSelectedOrderItem { get; private set; }

		public OrderController OrderController
		{
			get
			{
				return this.Team.PlayerOrderController;
			}
		}

		public bool IsMovementSubOrdersShown
		{
			get
			{
				return this._movementSet.ShowOrders;
			}
		}

		public bool IsFacingSubOrdersShown
		{
			get
			{
				OrderSetVM facingSet = this._facingSet;
				return facingSet != null && facingSet.ShowOrders;
			}
		}

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

		public OrderSetVM LastSelectedOrderSet
		{
			get
			{
				return this.OrderSets.FirstOrDefault((OrderSetVM o) => o.OrderSetType == this.LastSelectedOrderSetType);
			}
		}

		public bool PlayerHasAnyTroopUnderThem
		{
			get
			{
				return this.Team.FormationsIncludingEmpty.Any((Formation f) => f.PlayerOwner == Agent.Main && f.CountOfUnits > 0);
			}
		}

		private Mission Mission
		{
			get
			{
				return Mission.Current;
			}
		}

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
			OrderTroopItemVM.OnSelectionChange += this.OnTroopItemSelectionStateChanged;
			this.RefreshValues();
			this.Mission.OnMainAgentChanged += this.MissionOnMainAgentChanged;
			this.CanUseShortcuts = this._isMultiplayer;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ReturnText = new TextObject("{=EmVbbIUc}Return", null).ToString();
			foreach (OrderSetVM orderSetVM in this.OrderSetsWithOrdersByType.Values)
			{
				orderSetVM.RefreshValues();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			OrderTroopItemVM.OnSelectionChange -= this.OnTroopItemSelectionStateChanged;
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
					if (!dictionary.ContainsKey(formation.PhysicalClass))
					{
						TextObject localizedName = formation.PhysicalClass.GetLocalizedName();
						TextObject textObject = GameTexts.FindText("str_troop_group_name_definite", null);
						textObject.SetTextVariable("FORMATION_CLASS", localizedName);
						dictionary.Add(formation.PhysicalClass, textObject);
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

		private void OnTroopItemSelectionStateChanged(OrderTroopItemVM troopItem, bool isSelected)
		{
			for (int i = 0; i < this.TroopController.TroopList.Count; i++)
			{
				this.TroopController.TroopList[i].IsTargetRelevant = this.TroopController.TroopList[i].IsSelected;
			}
		}

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
				this.UpdateTitleOrdersKeyVisualVisibility();
				if (!this.IsDeployment)
				{
					this.InputRestrictions.ResetInputRestrictions();
					return true;
				}
			}
			return false;
		}

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

		private void OnOrder(OrderItemVM orderItem, OrderSetType orderSetType, bool fromSelection)
		{
			if (this.LastSelectedOrderItem != orderItem || !fromSelection)
			{
				this._onBeforeOrderDelegate();
				if (this.LastSelectedOrderItem != null)
				{
					this.LastSelectedOrderItem.IsSelected = false;
				}
				if (orderItem.IsTitle)
				{
					this.LastSelectedOrderSetType = orderSetType;
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
				if (this.LastSelectedOrderItem != null && this.LastSelectedOrderItem.OrderSubType != OrderSubType.None && !fromSelection)
				{
					OrderSetVM orderSetVM;
					if (this.LastSelectedOrderItem.OrderSubType == OrderSubType.Return && this.OrderSetsWithOrdersByType.TryGetValue(this.LastSelectedOrderSetType, out orderSetVM))
					{
						orderSetVM.ShowOrders = false;
						this.UpdateTitleOrdersKeyVisualVisibility();
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
					this.UpdateTitleOrdersKeyVisualVisibility();
				}
			}
		}

		private void UpdateTitleOrdersKeyVisualVisibility()
		{
			bool flag = this.OrderSets.Any((OrderSetVM o) => o.ShowOrders && o.Orders.Count > 0);
			bool? flag2 = null;
			if (flag)
			{
				flag2 = new bool?(false);
			}
			for (int i = 0; i < this.OrderSets.Count; i++)
			{
				this.OrderSets[i].TitleOrder.ShortcutKey.SetForcedVisibility(flag2);
			}
		}

		public void SetFocusedFormations(MBReadOnlyList<Formation> focusedFormationsCache)
		{
			this._focusedFormationsCache = focusedFormationsCache;
		}

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
					orderSetVM.ShowOrders = false;
					this.UpdateTitleOrdersKeyVisualVisibility();
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
						goto IL_1B4;
					}
				}
				foreach (OrderTroopItemVM orderTroopItemVM in this.TroopController.TroopList.Where((OrderTroopItemVM item) => item.IsSelected))
				{
					list.Add(GameTexts.FindText("str_formation_class_string", orderTroopItemVM.Formation.PhysicalClass.GetName()));
				}
			}
			IL_1B4:
			if (!list.IsEmpty<TextObject>())
			{
				TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
				textObject.SetTextVariable("STR1", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false));
				textObject.SetTextVariable("STR2", this.LastSelectedOrderItem.MainTitle);
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
						goto IL_830;
					}
					goto IL_830;
				}
				case OrderSubType.Charge:
				{
					MBReadOnlyList<Formation> focusedFormationsCache = this._focusedFormationsCache;
					if (focusedFormationsCache != null && focusedFormationsCache.Count > 0)
					{
						this.OrderController.SetOrderWithFormation(OrderType.Charge, this._focusedFormationsCache[0]);
						goto IL_830;
					}
					this.OrderController.SetOrder(OrderType.Charge);
					goto IL_830;
				}
				case OrderSubType.FollowMe:
					this.OrderController.SetOrderWithAgent(OrderType.FollowMe, Agent.Main);
					goto IL_830;
				case OrderSubType.Advance:
				{
					MBReadOnlyList<Formation> focusedFormationsCache2 = this._focusedFormationsCache;
					if (focusedFormationsCache2 != null && focusedFormationsCache2.Count > 0)
					{
						this.OrderController.SetOrderWithFormation(OrderType.Advance, this._focusedFormationsCache[0]);
						goto IL_830;
					}
					this.OrderController.SetOrder(OrderType.Advance);
					goto IL_830;
				}
				case OrderSubType.Fallback:
					this.OrderController.SetOrder(OrderType.FallBack);
					goto IL_830;
				case OrderSubType.Stop:
					this.OrderController.SetOrder(OrderType.StandYourGround);
					goto IL_830;
				case OrderSubType.Retreat:
					this.OrderController.SetOrder(OrderType.Retreat);
					goto IL_830;
				case OrderSubType.FormLine:
					this.OrderController.SetOrder(OrderType.ArrangementLine);
					goto IL_830;
				case OrderSubType.FormClose:
					this.OrderController.SetOrder(OrderType.ArrangementCloseOrder);
					goto IL_830;
				case OrderSubType.FormLoose:
					this.OrderController.SetOrder(OrderType.ArrangementLoose);
					goto IL_830;
				case OrderSubType.FormCircular:
					this.OrderController.SetOrder(OrderType.ArrangementCircular);
					goto IL_830;
				case OrderSubType.FormSchiltron:
					this.OrderController.SetOrder(OrderType.ArrangementSchiltron);
					goto IL_830;
				case OrderSubType.FormV:
					this.OrderController.SetOrder(OrderType.ArrangementVee);
					goto IL_830;
				case OrderSubType.FormColumn:
					this.OrderController.SetOrder(OrderType.ArrangementColumn);
					goto IL_830;
				case OrderSubType.FormScatter:
					this.OrderController.SetOrder(OrderType.ArrangementScatter);
					goto IL_830;
				case OrderSubType.ToggleStart:
				case OrderSubType.ToggleEnd:
					goto IL_830;
				case OrderSubType.ToggleFacing:
					if (OrderController.GetActiveFacingOrderOf(this.OrderController.SelectedFormations.FirstOrDefault<Formation>()) == OrderType.LookAtDirection)
					{
						this.OrderController.SetOrder(OrderType.LookAtEnemy);
						goto IL_830;
					}
					this.OrderController.SetOrderWithPosition(OrderType.LookAtDirection, new WorldPosition(this.Mission.Scene, UIntPtr.Zero, this._getOrderFlagPosition(), false));
					goto IL_830;
				case OrderSubType.ToggleFire:
					if (this.LastSelectedOrderItem.SelectionState == 3)
					{
						this.OrderController.SetOrder(OrderType.FireAtWill);
						goto IL_830;
					}
					this.OrderController.SetOrder(OrderType.HoldFire);
					goto IL_830;
				case OrderSubType.ToggleMount:
					if (this.LastSelectedOrderItem.SelectionState == 3)
					{
						this.OrderController.SetOrder(OrderType.Mount);
						goto IL_830;
					}
					this.OrderController.SetOrder(OrderType.Dismount);
					goto IL_830;
				case OrderSubType.ToggleAI:
				{
					if (this.LastSelectedOrderItem.SelectionState == 3)
					{
						this.OrderController.SetOrder(OrderType.AIControlOff);
						goto IL_830;
					}
					this.OrderController.SetOrder(OrderType.AIControlOn);
					using (List<Formation>.Enumerator enumerator4 = this.OrderController.SelectedFormations.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							Formation formation = enumerator4.Current;
							this.TeamOnFormationAIActiveBehaviorChanged(formation);
						}
						goto IL_830;
					}
					break;
				}
				case OrderSubType.ToggleTransfer:
					break;
				case OrderSubType.ActivationFaceDirection:
					this.OrderController.SetOrderWithPosition(OrderType.LookAtDirection, new WorldPosition(this.Mission.Scene, UIntPtr.Zero, this._getOrderFlagPosition(), false));
					goto IL_830;
				case OrderSubType.FaceEnemy:
					this.OrderController.SetOrder(OrderType.LookAtEnemy);
					goto IL_830;
				default:
					goto IL_830;
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
			IL_830:
			if (this.ActiveTargetState == 0)
			{
				IEnumerable<OrderTroopItemVM> enumerable = this.TroopController.TroopList.Where((OrderTroopItemVM item) => item.IsSelected);
				enumerable.Count<OrderTroopItemVM>();
				using (IEnumerator<OrderTroopItemVM> enumerator2 = enumerable.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						OrderTroopItemVM orderTroopItemVM4 = enumerator2.Current;
						this.TroopController.SetTroopActiveOrders(orderTroopItemVM4);
						orderTroopItemVM4.IsTargetRelevant = orderTroopItemVM4.IsSelected;
					}
					goto IL_912;
				}
			}
			foreach (OrderSiegeMachineVM orderSiegeMachineVM2 in this.DeploymentController.SiegeMachineList.Where((OrderSiegeMachineVM item) => item.IsSelected))
			{
				this.DeploymentController.SetSiegeMachineActiveOrders(orderSiegeMachineVM2);
			}
			IL_912:
			this.UpdateTitleOrdersKeyVisualVisibility();
			this.LastSelectedOrderItem = null;
			this.LastSelectedOrderSetType = OrderSetType.None;
		}

		public void AfterInitialize()
		{
			this.TroopController.UpdateTroops();
			if (!this.IsDeployment)
			{
				this.TroopController.SelectAllFormations(false);
			}
			this.DeploymentController.SetCurrentActiveOrders();
		}

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
				this.TroopController.RefreshTroopFormationTargetVisuals();
			}
			this.DeploymentController.Update();
			this.DisplayFormationAIFeedback();
		}

		public void OnEscape()
		{
			if (this.IsToggleOrderShown)
			{
				if (this._currentActivationType == MissionOrderVM.ActivationType.Hold)
				{
					if (this.LastSelectedOrderItem != null)
					{
						this.UpdateTitleOrdersKeyVisualVisibility();
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
						this.UpdateTitleOrdersKeyVisualVisibility();
						this.OrderSetsWithOrdersByType[this.LastSelectedOrderSetType].ShowOrders = false;
						this.LastSelectedOrderSetType = OrderSetType.None;
						return;
					}
					this.LastSelectedOrderSetType = OrderSetType.None;
					this.TryCloseToggleOrder(false);
				}
			}
		}

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

		private void MissionOnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.Mission.MainAgent == null)
			{
				this.TryCloseToggleOrder(false);
				this.Mission.IsOrderMenuOpen = false;
			}
		}

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

		private void OnTransferFinished()
		{
			this._onTransferTroopsFinishedDelegate.DynamicInvokeWithLog(Array.Empty<object>());
		}

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

		public void OnDeploymentFinished()
		{
			this.TroopController.OnDeploymentFinished();
			this.DeploymentController.FinalizeDeployment();
			this.IsDeployment = false;
		}

		public void OnFiltersSet(List<ValueTuple<int, List<int>>> filterData)
		{
			this._filterData = filterData;
			this.TroopController.OnFiltersSet(filterData);
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

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

		public InputRestrictions InputRestrictions;

		private MissionOrderVM.ActivationType _currentActivationType;

		private Timer _updateTroopsTimer;

		internal readonly Dictionary<OrderSetType, OrderSetVM> OrderSetsWithOrdersByType;

		private readonly Camera _deploymentCamera;

		private bool _isTroopPlacingActive;

		private OrderSetType _lastSelectedOrderSetType;

		private bool _isPressedViewOrders;

		private readonly OnToggleActivateOrderStateDelegate _onActivateToggleOrder;

		private readonly OnToggleActivateOrderStateDelegate _onDeactivateToggleOrder;

		private readonly GetOrderFlagPositionDelegate _getOrderFlagPosition;

		private readonly ToggleOrderPositionVisibilityDelegate _toggleOrderPositionVisibility;

		private readonly OnRefreshVisualsDelegate _onRefreshVisuals;

		private readonly OnToggleActivateOrderStateDelegate _onTransferTroopsFinishedDelegate;

		private readonly OnBeforeOrderDelegate _onBeforeOrderDelegate;

		private readonly Action<bool> _toggleMissionInputs;

		private readonly List<DeploymentPoint> _deploymentPoints;

		private readonly bool _isMultiplayer;

		private OrderSetVM _movementSet;

		private MBReadOnlyList<Formation> _focusedFormationsCache;

		private OrderSetVM _facingSet;

		private int _delayValueForAIFormationModifications;

		private readonly List<Formation> _modifiedAIFormations = new List<Formation>();

		private List<ValueTuple<int, List<int>>> _filterData;

		private InputKeyItemVM _cancelInputKey;

		private MBBindingList<OrderSetVM> _orderSets;

		private MissionOrderTroopControllerVM _troopController;

		private MissionOrderDeploymentControllerVM _deploymentController;

		private bool _isDeployment;

		private int _activeTargetState;

		private bool _isToggleOrderShown;

		private bool _isTroopListShown;

		private bool _canUseShortcuts;

		private bool _isHolding;

		private bool _isAnyOrderSetActive;

		private string _returnText;

		public enum CursorState
		{
			Move,
			Face,
			Form
		}

		public enum OrderTargets
		{
			Troops,
			SiegeMachines
		}

		public enum ActivationType
		{
			NotActive,
			Hold,
			Click
		}
	}
}
