using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	public class MissionOrderTroopControllerVM : ViewModel
	{
		private Mission Mission
		{
			get
			{
				return Mission.Current;
			}
		}

		private Team Team
		{
			get
			{
				return Mission.Current.PlayerTeam;
			}
		}

		public OrderController OrderController
		{
			get
			{
				return this.Team.PlayerOrderController;
			}
		}

		public MissionOrderTroopControllerVM(MissionOrderVM missionOrder, bool isMultiplayer, bool isDeployment, Action onTransferFinised)
		{
			this._missionOrder = missionOrder;
			this._onTransferFinised = onTransferFinised;
			this._isMultiplayer = isMultiplayer;
			this._isDeployment = isDeployment;
			this.TroopList = new MBBindingList<OrderTroopItemVM>();
			this.TransferTargetList = new MBBindingList<OrderTroopItemVM>();
			this.TroopList.Clear();
			this.TransferTargetList.Clear();
			for (int i = 0; i < 8; i++)
			{
				OrderTroopItemVM orderTroopItemVM = new OrderTroopItemVM(this.Team.GetFormation((FormationClass)i), new Action<OrderTroopItemVM>(this.ExecuteSelectTransferTroop), new Func<Formation, int>(this.GetFormationMorale));
				this.TransferTargetList.Add(orderTroopItemVM);
				orderTroopItemVM.IsSelected = false;
			}
			this.Team.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnTroopOrderIssued);
			if (this.TroopList.Count > 0)
			{
				this.OnSelectFormation(this.TroopList[0]);
			}
			this._formationIndexComparer = new MissionOrderTroopControllerVM.TroopItemFormationIndexComparer();
			this.SortFormations();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this._troopList.ApplyActionOnAllItems(delegate(OrderTroopItemVM x)
			{
				x.RefreshValues();
			});
			this.AcceptText = GameTexts.FindText("str_selection_widget_accept", null).ToString();
			this.CancelText = GameTexts.FindText("str_selection_widget_cancel", null).ToString();
		}

		public void ExecuteSelectAll()
		{
			this.SelectAllFormations(true);
		}

		public void ExecuteSelectTransferTroop(OrderTroopItemVM targetTroop)
		{
			foreach (OrderTroopItemVM orderTroopItemVM in this.TransferTargetList)
			{
				orderTroopItemVM.IsSelected = false;
			}
			targetTroop.IsSelected = targetTroop.IsSelectable;
			this.IsTransferValid = targetTroop.IsSelectable;
			GameTexts.SetVariable("FORMATION_INDEX", Common.ToRoman(targetTroop.Formation.Index + 1));
			this.TransferTitleText = new TextObject("{=DvnRkWQg}Transfer Troops To {FORMATION_INDEX}", null).ToString();
		}

		public void ExecuteConfirmTransfer()
		{
			this.IsTransferActive = false;
			OrderTroopItemVM orderTroopItemVM = this.TransferTargetList.Single((OrderTroopItemVM t) => t.IsSelected);
			int num = this.TransferValue;
			int num2 = this.TroopList.Where((OrderTroopItemVM t) => t.IsSelected).Sum((OrderTroopItemVM t) => t.CurrentMemberCount);
			num = MathF.Min(num, num2);
			this.OrderController.SetOrderWithFormationAndNumber(OrderType.Transfer, orderTroopItemVM.Formation, num);
			for (int i = 0; i < this.TroopList.Count; i++)
			{
				OrderTroopItemVM orderTroopItemVM2 = this.TroopList[i];
				if (!orderTroopItemVM2.ContainsDeadTroop && orderTroopItemVM2.CurrentMemberCount == 0)
				{
					this.TroopList.RemoveAt(i);
					i--;
				}
			}
			Action onTransferFinised = this._onTransferFinised;
			if (onTransferFinised == null)
			{
				return;
			}
			onTransferFinised.DynamicInvokeWithLog(Array.Empty<object>());
		}

		public void ExecuteCancelTransfer()
		{
			this.IsTransferActive = false;
			Action onTransferFinised = this._onTransferFinised;
			if (onTransferFinised == null)
			{
				return;
			}
			onTransferFinised.DynamicInvokeWithLog(Array.Empty<object>());
		}

		public void ExecuteReset()
		{
			this.RefreshValues();
		}

		internal void SetTroopActiveOrders(OrderTroopItemVM item)
		{
			bool flag = BannerlordConfig.OrderLayoutType == 1;
			item.ActiveOrders.Clear();
			List<OrderSubType> list = new List<OrderSubType>();
			OrderType orderType = OrderUIHelper.GetOrderOverrideForUI(item.Formation, OrderSetType.Movement);
			if (orderType == OrderType.None)
			{
				orderType = OrderController.GetActiveMovementOrderOf(item.Formation);
			}
			switch (orderType)
			{
			case OrderType.Move:
				list.Add(OrderSubType.MoveToPosition);
				goto IL_D9;
			case OrderType.Charge:
				list.Add(OrderSubType.Charge);
				goto IL_D9;
			case OrderType.ChargeWithTarget:
				list.Add(OrderSubType.Charge);
				goto IL_D9;
			case OrderType.StandYourGround:
				list.Add(OrderSubType.Stop);
				goto IL_D9;
			case OrderType.FollowMe:
				list.Add(OrderSubType.FollowMe);
				goto IL_D9;
			case OrderType.Retreat:
				list.Add(OrderSubType.Retreat);
				goto IL_D9;
			case OrderType.Advance:
				list.Add(OrderSubType.Advance);
				goto IL_D9;
			case OrderType.FallBack:
				list.Add(OrderSubType.Fallback);
				goto IL_D9;
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 175);
			IL_D9:
			OrderType orderType2 = OrderUIHelper.GetOrderOverrideForUI(item.Formation, OrderSetType.Form);
			if (orderType2 == OrderType.None)
			{
				orderType2 = OrderController.GetActiveArrangementOrderOf(item.Formation);
			}
			switch (orderType2)
			{
			case OrderType.ArrangementLine:
				list.Add(OrderSubType.FormLine);
				break;
			case OrderType.ArrangementCloseOrder:
				list.Add(OrderSubType.FormClose);
				break;
			case OrderType.ArrangementLoose:
				list.Add(OrderSubType.FormLoose);
				break;
			case OrderType.ArrangementCircular:
				list.Add(OrderSubType.FormCircular);
				break;
			case OrderType.ArrangementSchiltron:
				list.Add(OrderSubType.FormSchiltron);
				break;
			case OrderType.ArrangementVee:
				list.Add(OrderSubType.FormV);
				break;
			case OrderType.ArrangementColumn:
				list.Add(OrderSubType.FormColumn);
				break;
			case OrderType.ArrangementScatter:
				list.Add(OrderSubType.FormScatter);
				break;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 220);
				break;
			}
			OrderType orderType3 = OrderController.GetActiveRidingOrderOf(item.Formation);
			if (orderType3 != OrderType.Mount)
			{
				if (orderType3 == OrderType.Dismount)
				{
					list.Add(OrderSubType.ToggleMount);
				}
				else
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 234);
				}
			}
			orderType3 = OrderController.GetActiveFiringOrderOf(item.Formation);
			if (orderType3 != OrderType.HoldFire)
			{
				if (orderType3 != OrderType.FireAtWill)
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 248);
				}
			}
			else
			{
				list.Add(OrderSubType.ToggleFire);
			}
			if (!this._isMultiplayer)
			{
				orderType3 = OrderController.GetActiveAIControlOrderOf(item.Formation);
				if (orderType3 != OrderType.AIControlOn)
				{
					if (orderType3 != OrderType.AIControlOff)
					{
						Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 264);
					}
				}
				else
				{
					list.Add(OrderSubType.ToggleAI);
				}
			}
			orderType3 = OrderController.GetActiveFacingOrderOf(item.Formation);
			if (orderType3 != OrderType.LookAtEnemy)
			{
				if (orderType3 == OrderType.LookAtDirection)
				{
					list.Add(OrderSubType.ActivationFaceDirection);
				}
				else
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 280);
				}
			}
			else
			{
				list.Add(flag ? OrderSubType.FaceEnemy : OrderSubType.ToggleFacing);
			}
			foreach (OrderSubType orderSubType in list)
			{
				item.ActiveOrders.AddRange(this._missionOrder.GetAllOrderItemsForSubType(orderSubType));
			}
		}

		internal void SelectAllFormations(bool uiFeedback = true)
		{
			foreach (OrderSetVM orderSetVM in this._missionOrder.OrderSetsWithOrdersByType.Values)
			{
				orderSetVM.ShowOrders = false;
			}
			if (this.TroopList.Any((OrderTroopItemVM t) => t.IsSelectable))
			{
				this.OrderController.SelectAllFormations(uiFeedback);
				if (uiFeedback && this.OrderController.SelectedFormations.Count > 0)
				{
					InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=xTv4tCbZ}Everybody!! Listen to me", null).ToString()));
				}
			}
			foreach (OrderTroopItemVM orderTroopItemVM in this.TroopList)
			{
				orderTroopItemVM.IsSelected = orderTroopItemVM.IsSelectable;
			}
			this._missionOrder.SetActiveOrders();
		}

		internal void AddSelectedFormation(OrderTroopItemVM item)
		{
			if (!item.IsSelectable)
			{
				return;
			}
			Formation formation = this.Team.GetFormation(item.InitialFormationClass);
			this.OrderController.SelectFormation(formation);
			item.IsSelected = true;
			this._missionOrder.SetActiveOrders();
		}

		internal void SetSelectedFormation(OrderTroopItemVM item)
		{
			this.UpdateTroops();
			if (!item.IsSelectable)
			{
				return;
			}
			this.OrderController.ClearSelectedFormations();
			foreach (OrderTroopItemVM orderTroopItemVM in this.TroopList)
			{
				orderTroopItemVM.IsSelected = false;
			}
			this.AddSelectedFormation(item);
		}

		public void OnDeselectFormation(int index)
		{
			OrderTroopItemVM orderTroopItemVM = this.TroopList.FirstOrDefault((OrderTroopItemVM t) => t.Formation.Index == index);
			this.OnDeselectFormation(orderTroopItemVM);
		}

		internal void OnDeselectFormation(OrderTroopItemVM item)
		{
			if (item != null)
			{
				Formation formation = this.Team.GetFormation(item.InitialFormationClass);
				if (this.OrderController.SelectedFormations.Contains(formation))
				{
					this.OrderController.DeselectFormation(formation);
				}
				item.IsSelected = false;
				if (this._isDeployment)
				{
					if (this.TroopList.Count((OrderTroopItemVM t) => t.IsSelected) != 0)
					{
						this._missionOrder.SetActiveOrders();
						return;
					}
					this._missionOrder.TryCloseToggleOrder(true);
					this._missionOrder.IsTroopPlacingActive = false;
					return;
				}
				else
				{
					this._missionOrder.SetActiveOrders();
				}
			}
		}

		internal void OnSelectFormation(OrderTroopItemVM item)
		{
			foreach (OrderSetVM orderSetVM in this._missionOrder.OrderSetsWithOrdersByType.Values)
			{
				orderSetVM.ShowOrders = false;
			}
			this.UpdateTroops();
			this._missionOrder.IsTroopPlacingActive = true;
			if (Input.IsKeyDown(InputKey.LeftControl))
			{
				if (item.IsSelected)
				{
					this.OnDeselectFormation(item);
				}
				else
				{
					this.AddSelectedFormation(item);
				}
			}
			else
			{
				this.SetSelectedFormation(item);
			}
			if (this.IsTransferActive)
			{
				foreach (OrderTroopItemVM orderTroopItemVM in this.TransferTargetList)
				{
					orderTroopItemVM.IsSelectable = !this.OrderController.IsFormationListening(orderTroopItemVM.Formation);
				}
				this.IsTransferValid = this.TransferTargetList.Any((OrderTroopItemVM t) => t.IsSelected && t.IsSelectable);
				this.TransferMaxValue = this.TroopList.Where((OrderTroopItemVM t) => t.IsSelected).Sum((OrderTroopItemVM t) => t.CurrentMemberCount);
				this.TransferValue = this.TransferMaxValue;
			}
		}

		internal void CheckSelectableFormations()
		{
			foreach (OrderTroopItemVM orderTroopItemVM in this.TroopList)
			{
				Formation formation = this.Team.GetFormation(orderTroopItemVM.InitialFormationClass);
				if (formation != null)
				{
					bool flag = this.OrderController.IsFormationSelectable(formation);
					orderTroopItemVM.IsSelectable = flag;
					if (!orderTroopItemVM.IsSelectable && orderTroopItemVM.IsSelected)
					{
						this.OnDeselectFormation(orderTroopItemVM);
					}
				}
			}
		}

		internal void UpdateTroops()
		{
			List<Formation> list;
			if (this.Mission.MainAgent != null && this.Mission.MainAgent.Controller != Agent.ControllerType.Player)
			{
				list = this.Team.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0).ToList<Formation>();
			}
			else
			{
				list = this.Team.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && (!f.IsPlayerTroopInFormation || f.CountOfUnits > 1)).ToList<Formation>();
			}
			foreach (OrderTroopItemVM orderTroopItemVM in this.TroopList)
			{
				this.SetTroopActiveOrders(orderTroopItemVM);
				orderTroopItemVM.IsSelectable = this.OrderController.IsFormationSelectable(orderTroopItemVM.Formation);
				if (orderTroopItemVM.IsSelectable && this.OrderController.IsFormationListening(orderTroopItemVM.Formation))
				{
					orderTroopItemVM.IsSelected = true;
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				Formation formation = list[i];
				if (formation != null && this.TroopList.All((OrderTroopItemVM item) => item.Formation != formation))
				{
					OrderTroopItemVM orderTroopItemVM2 = new OrderTroopItemVM(formation, new Action<OrderTroopItemVM>(this.OnSelectFormation), new Func<Formation, int>(this.GetFormationMorale));
					orderTroopItemVM2 = this.AddTroopItemIfNotExist(orderTroopItemVM2, -1);
					this.SetTroopActiveOrders(orderTroopItemVM2);
					orderTroopItemVM2.IsSelectable = this.OrderController.IsFormationSelectable(formation);
					if (orderTroopItemVM2.IsSelectable && this.OrderController.IsFormationListening(formation))
					{
						orderTroopItemVM2.IsSelected = true;
					}
					this.SortFormations();
				}
			}
		}

		public void AddTroops(Agent agent)
		{
			if (agent.Team != this.Team || agent.Formation == null || agent.IsPlayerControlled)
			{
				return;
			}
			Formation formation = agent.Formation;
			OrderTroopItemVM orderTroopItemVM = this.TroopList.FirstOrDefault((OrderTroopItemVM item) => item.Formation.FormationIndex == formation.FormationIndex);
			if (orderTroopItemVM == null)
			{
				OrderTroopItemVM orderTroopItemVM2 = new OrderTroopItemVM(formation, new Action<OrderTroopItemVM>(this.OnSelectFormation), new Func<Formation, int>(this.GetFormationMorale));
				orderTroopItemVM2 = this.AddTroopItemIfNotExist(orderTroopItemVM2, -1);
				this.SetTroopActiveOrders(orderTroopItemVM2);
				orderTroopItemVM2.IsSelectable = this.OrderController.IsFormationSelectable(formation);
				if (orderTroopItemVM2.IsSelectable && this.OrderController.IsFormationListening(formation))
				{
					orderTroopItemVM2.IsSelected = true;
					return;
				}
			}
			else
			{
				orderTroopItemVM.SetFormationClassFromFormation(formation);
				bool flag = this.OrderController.IsFormationSelectable(formation);
				orderTroopItemVM.IsSelectable = flag;
			}
		}

		public void RemoveTroops(Agent agent)
		{
			if (agent.Team != this.Team || agent.Formation == null)
			{
				return;
			}
			Formation formation = agent.Formation;
			OrderTroopItemVM orderTroopItemVM = this.TroopList.FirstOrDefault((OrderTroopItemVM item) => item.Formation.FormationIndex == formation.FormationIndex);
			if (orderTroopItemVM != null)
			{
				orderTroopItemVM.OnFormationAgentRemoved(agent);
				orderTroopItemVM.SetFormationClassFromFormation(formation);
				orderTroopItemVM.IsSelectable = this.OrderController.IsFormationSelectable(formation);
				if (!orderTroopItemVM.IsSelectable && orderTroopItemVM.IsSelected)
				{
					this.OnDeselectFormation(orderTroopItemVM);
				}
			}
		}

		private void OrderController_OnTroopOrderIssued(OrderType orderType, IEnumerable<Formation> appliedFormations, params object[] delegateParams)
		{
			foreach (OrderSetVM orderSetVM in this._missionOrder.OrderSetsWithOrdersByType.Values)
			{
				orderSetVM.TitleOrder.IsActive = orderSetVM.TitleOrder.SelectionState != 0;
			}
			this._missionOrder.OrderSetsWithOrdersByType[OrderSetType.Movement].ShowOrders = false;
			if (orderType == OrderType.Transfer)
			{
				if (!(delegateParams[1] is object[]))
				{
					int num = (int)delegateParams[1];
				}
				Formation formation = delegateParams[0] as Formation;
				OrderTroopItemVM orderTroopItemVM = this.TroopList.FirstOrDefault((OrderTroopItemVM item) => item.Formation == formation);
				if (orderTroopItemVM == null)
				{
					int num2 = -1;
					for (int i = 0; i < this.TroopList.Count; i++)
					{
						if (this.TroopList[i].Formation.Index > formation.Index)
						{
							num2 = i;
							break;
						}
					}
					OrderTroopItemVM orderTroopItemVM2 = new OrderTroopItemVM(formation, new Action<OrderTroopItemVM>(this.OnSelectFormation), new Func<Formation, int>(this.GetFormationMorale));
					orderTroopItemVM2 = this.AddTroopItemIfNotExist(orderTroopItemVM2, num2);
					this.SetTroopActiveOrders(orderTroopItemVM2);
					orderTroopItemVM2.IsSelectable = this.OrderController.IsFormationSelectable(formation);
					if (orderTroopItemVM2.IsSelectable && this.OrderController.IsFormationListening(formation))
					{
						orderTroopItemVM2.IsSelected = true;
					}
					this.OnFiltersSet(this._filterData);
				}
				else
				{
					orderTroopItemVM.SetFormationClassFromFormation(formation);
				}
				using (IEnumerator<Formation> enumerator2 = appliedFormations.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Formation sourceFormation2 = enumerator2.Current;
						OrderTroopItemVM orderTroopItemVM3 = this.TroopList.FirstOrDefault((OrderTroopItemVM item) => item.Formation == sourceFormation2);
						if (orderTroopItemVM3 == null)
						{
							int num3 = -1;
							for (int j = 0; j < this.TroopList.Count; j++)
							{
								if (this.TroopList[j].Formation.Index > sourceFormation2.Index)
								{
									num3 = j;
									break;
								}
							}
							OrderTroopItemVM orderTroopItemVM4 = new OrderTroopItemVM(sourceFormation2, new Action<OrderTroopItemVM>(this.OnSelectFormation), new Func<Formation, int>(this.GetFormationMorale));
							orderTroopItemVM4 = this.AddTroopItemIfNotExist(orderTroopItemVM4, num3);
							this.SetTroopActiveOrders(orderTroopItemVM4);
							orderTroopItemVM4.IsSelectable = this.OrderController.IsFormationSelectable(sourceFormation2);
							if (orderTroopItemVM4.IsSelectable && this.OrderController.IsFormationListening(sourceFormation2))
							{
								orderTroopItemVM4.IsSelected = true;
							}
							this.OnFiltersSet(this._filterData);
						}
						else
						{
							orderTroopItemVM3.SetFormationClassFromFormation(sourceFormation2);
						}
					}
				}
				int num4 = 1;
				using (IEnumerator<Formation> enumerator2 = appliedFormations.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Formation sourceFormation = enumerator2.Current;
						this.TroopList.FirstOrDefault((OrderTroopItemVM item) => item.Formation.Index == sourceFormation.Index).SetFormationClassFromFormation(sourceFormation);
						num4++;
					}
				}
			}
			this.UpdateTroops();
			this.SortFormations();
			foreach (OrderTroopItemVM orderTroopItemVM5 in this.TroopList.Where((OrderTroopItemVM item) => item.IsSelected))
			{
				this.SetTroopActiveOrders(orderTroopItemVM5);
			}
			this._missionOrder.SetActiveOrders();
			this.CheckSelectableFormations();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Team.OnOrderIssued -= new OnOrderIssuedDelegate(this.OrderController_OnTroopOrderIssued);
			foreach (OrderTroopItemVM orderTroopItemVM in this.TroopList)
			{
				orderTroopItemVM.OnFinalize();
			}
			this._transferTargetList = null;
		}

		internal void IntervalUpdate()
		{
			for (int i = this.TroopList.Count - 1; i >= 0; i--)
			{
				OrderTroopItemVM orderTroopItemVM = this.TroopList[i];
				Formation formation = this.Team.GetFormation(orderTroopItemVM.InitialFormationClass);
				if (formation != null && formation.CountOfUnits > 0)
				{
					orderTroopItemVM.UnderAttackOfType = (int)formation.GetUnderAttackTypeOfUnits(3f);
					orderTroopItemVM.BehaviorType = (int)formation.GetMovementTypeOfUnits();
					if (!this._isDeployment)
					{
						orderTroopItemVM.Morale = (int)MissionGameModels.Current.BattleMoraleModel.GetAverageMorale(formation);
						if (orderTroopItemVM.SetFormationClassFromFormation(formation))
						{
							this.UpdateTroops();
						}
						orderTroopItemVM.IsAmmoAvailable = formation.QuerySystem.RangedUnitRatio > 0f || formation.QuerySystem.RangedCavalryUnitRatio > 0f;
						if (orderTroopItemVM.IsAmmoAvailable)
						{
							int totalCurrentAmmo = 0;
							int totalMaxAmmo = 0;
							orderTroopItemVM.Formation.ApplyActionOnEachUnit(delegate(Agent agent)
							{
								if (!agent.IsMainAgent)
								{
									int num;
									int num2;
									this.GetMaxAndCurrentAmmoOfAgent(agent, out num, out num2);
									totalCurrentAmmo += num;
									totalMaxAmmo += num2;
								}
							}, null);
							orderTroopItemVM.AmmoPercentage = (float)totalCurrentAmmo / (float)totalMaxAmmo;
						}
					}
				}
				else if (formation != null && formation.CountOfUnits == 0)
				{
					orderTroopItemVM.Morale = 0;
					orderTroopItemVM.SetFormationClassFromFormation(formation);
				}
			}
		}

		internal void RefreshTroopFormationTargetVisuals()
		{
			for (int i = 0; i < this.TroopList.Count; i++)
			{
				this.TroopList[i].RefreshTargetedOrderVisual();
			}
		}

		internal void OnSelectFormationWithIndex(int formationTroopIndex)
		{
			this.UpdateTroops();
			OrderTroopItemVM orderTroopItemVM = this.TroopList.SingleOrDefault((OrderTroopItemVM t) => t.Formation.Index == formationTroopIndex);
			if (orderTroopItemVM != null)
			{
				if (orderTroopItemVM.IsSelectable)
				{
					this.OnSelectFormation(orderTroopItemVM);
					return;
				}
			}
			else
			{
				this.SelectAllFormations(true);
			}
		}

		internal void SetCurrentActiveOrders()
		{
			List<OrderSubjectVM> list = (from item in this.TroopList.Cast<OrderSubjectVM>().ToList<OrderSubjectVM>()
				where item.IsSelected && item.IsSelectable
				select item).ToList<OrderSubjectVM>();
			if (list.IsEmpty<OrderSubjectVM>())
			{
				this.OrderController.SelectAllFormations(false);
				foreach (OrderTroopItemVM orderTroopItemVM in this.TroopList.Where((OrderTroopItemVM s) => this.OrderController.SelectedFormations.Contains(s.Formation)))
				{
					orderTroopItemVM.IsSelected = true;
					orderTroopItemVM.IsSelectable = true;
					this.SetTroopActiveOrders(orderTroopItemVM);
					list.Add(orderTroopItemVM);
				}
			}
		}

		private void GetMaxAndCurrentAmmoOfAgent(Agent agent, out int currentAmmo, out int maxAmmo)
		{
			currentAmmo = 0;
			maxAmmo = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (!agent.Equipment[equipmentIndex].IsEmpty && agent.Equipment[equipmentIndex].CurrentUsageItem.IsRangedWeapon)
				{
					currentAmmo = agent.Equipment.GetAmmoAmount(equipmentIndex);
					maxAmmo = agent.Equipment.GetMaxAmmo(equipmentIndex);
					return;
				}
			}
		}

		public void OnFiltersSet(List<ValueTuple<int, List<int>>> filterData)
		{
			if (filterData == null)
			{
				return;
			}
			this._filterData = filterData;
			using (List<ValueTuple<int, List<int>>>.Enumerator enumerator = filterData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ValueTuple<int, List<int>> filter = enumerator.Current;
					OrderTroopItemVM orderTroopItemVM = this.TroopList.FirstOrDefault((OrderTroopItemVM f) => f.Formation.Index == filter.Item1);
					if (orderTroopItemVM != null)
					{
						orderTroopItemVM.UpdateFilterData(filter.Item2);
					}
					OrderTroopItemVM orderTroopItemVM2 = this.TransferTargetList.FirstOrDefault((OrderTroopItemVM f) => f.Formation.Index == filter.Item1);
					if (orderTroopItemVM2 != null)
					{
						orderTroopItemVM2.UpdateFilterData(filter.Item2);
					}
				}
			}
		}

		public void OnDeploymentFinished()
		{
			this._isDeployment = false;
			this.SortFormations();
			for (int i = this.TroopList.Count - 1; i >= 0; i--)
			{
				if (this.TroopList[i].CurrentMemberCount <= 0)
				{
					this.TroopList.RemoveAt(i);
				}
			}
			this.SelectAllFormations(false);
		}

		private void SortFormations()
		{
			this.TroopList.Sort(this._formationIndexComparer);
			this.TransferTargetList.Sort(this._formationIndexComparer);
		}

		private int GetFormationMorale(Formation formation)
		{
			if (!this._isDeployment)
			{
				return (int)MissionGameModels.Current.BattleMoraleModel.GetAverageMorale(formation);
			}
			return 0;
		}

		private OrderTroopItemVM AddTroopItemIfNotExist(OrderTroopItemVM troopItem, int index = -1)
		{
			OrderTroopItemVM orderTroopItemVM = null;
			if (troopItem != null)
			{
				bool flag = true;
				orderTroopItemVM = this.TroopList.FirstOrDefault((OrderTroopItemVM t) => t.Formation.Index == troopItem.Formation.Index);
				if (orderTroopItemVM == null)
				{
					flag = false;
					orderTroopItemVM = troopItem;
				}
				if (flag)
				{
					this.TroopList.Remove(orderTroopItemVM);
				}
				if (index == -1)
				{
					this.TroopList.Add(troopItem);
				}
				else
				{
					this.TroopList.Insert(index, troopItem);
				}
			}
			else
			{
				Debug.FailedAssert("Added troop item is null!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "AddTroopItemIfNotExist", 882);
			}
			return orderTroopItemVM;
		}

		[DataSourceProperty]
		public bool IsTransferActive
		{
			get
			{
				return this._isTransferActive;
			}
			set
			{
				if (value != this._isTransferActive)
				{
					this._isTransferActive = value;
					base.OnPropertyChanged("IsTransferActive");
					this._missionOrder.IsTroopPlacingActive = !value;
					if (this._missionOrder.OrderSetsWithOrdersByType.ContainsKey(OrderSetType.Toggle))
					{
						this._missionOrder.OrderSetsWithOrdersByType[OrderSetType.Toggle].GetOrder(OrderSubType.ToggleTransfer).SelectionState = (value ? 3 : 1);
						this._missionOrder.OrderSetsWithOrdersByType[OrderSetType.Toggle].FinalizeActiveStatus(false);
					}
					if (this._isTransferActive)
					{
						foreach (OrderTroopItemVM orderTroopItemVM in this.TransferTargetList)
						{
							orderTroopItemVM.SetFormationClassFromFormation(orderTroopItemVM.Formation);
							orderTroopItemVM.Morale = (int)MissionGameModels.Current.BattleMoraleModel.GetAverageMorale(orderTroopItemVM.Formation);
							orderTroopItemVM.IsAmmoAvailable = orderTroopItemVM.Formation.QuerySystem.RangedUnitRatio > 0f || orderTroopItemVM.Formation.QuerySystem.RangedCavalryUnitRatio > 0f;
						}
					}
					if (this.Mission != null)
					{
						this.Mission.IsTransferMenuOpen = value;
					}
				}
			}
		}

		[DataSourceProperty]
		public bool IsTransferValid
		{
			get
			{
				return this._isTransferValid;
			}
			set
			{
				if (value != this._isTransferValid)
				{
					this._isTransferValid = value;
					base.OnPropertyChanged("IsTransferValid");
					if (!value)
					{
						this.TransferTitleText = "";
					}
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OrderTroopItemVM> TransferTargetList
		{
			get
			{
				return this._transferTargetList;
			}
			set
			{
				if (value != this._transferTargetList)
				{
					this._transferTargetList = value;
					base.OnPropertyChanged("TransferTargetList");
				}
			}
		}

		[DataSourceProperty]
		public int TransferMaxValue
		{
			get
			{
				return this._transferMaxValue;
			}
			set
			{
				if (value != this._transferMaxValue)
				{
					this._transferMaxValue = value;
					base.OnPropertyChanged("TransferMaxValue");
				}
			}
		}

		[DataSourceProperty]
		public int TransferValue
		{
			get
			{
				return this._transferValue;
			}
			set
			{
				if (value != this._transferValue)
				{
					this._transferValue = value;
					base.OnPropertyChanged("TransferValue");
				}
			}
		}

		[DataSourceProperty]
		public string TransferTitleText
		{
			get
			{
				return this._transferTitleText;
			}
			set
			{
				if (value != this._transferTitleText)
				{
					this._transferTitleText = value;
					base.OnPropertyChanged("TransferTitleText");
				}
			}
		}

		[DataSourceProperty]
		public string AcceptText
		{
			get
			{
				return this._acceptText;
			}
			set
			{
				if (value != this._acceptText)
				{
					this._acceptText = value;
					base.OnPropertyChanged("AcceptText");
				}
			}
		}

		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChanged("CancelText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OrderTroopItemVM> TroopList
		{
			get
			{
				return this._troopList;
			}
			set
			{
				if (value != this._troopList)
				{
					this._troopList = value;
					base.OnPropertyChanged("TroopList");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
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
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetResetInputKey(HotKey hotKey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		private readonly MissionOrderVM _missionOrder;

		private readonly Action _onTransferFinised;

		private readonly bool _isMultiplayer;

		private List<ValueTuple<int, List<int>>> _filterData;

		private bool _isDeployment;

		private MissionOrderTroopControllerVM.TroopItemFormationIndexComparer _formationIndexComparer;

		private MBBindingList<OrderTroopItemVM> _troopList;

		private bool _isTransferActive;

		private MBBindingList<OrderTroopItemVM> _transferTargetList;

		private int _transferValue;

		private int _transferMaxValue;

		private string _transferTitleText;

		private string _acceptText;

		private string _cancelText;

		private bool _isTransferValid;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _resetInputKey;

		private class TroopItemFormationIndexComparer : IComparer<OrderTroopItemVM>
		{
			public int Compare(OrderTroopItemVM x, OrderTroopItemVM y)
			{
				return x.Formation.Index.CompareTo(y.Formation.Index);
			}
		}
	}
}
