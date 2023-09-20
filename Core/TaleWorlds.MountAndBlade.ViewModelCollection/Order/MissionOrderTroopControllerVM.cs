using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000018 RID: 24
	public class MissionOrderTroopControllerVM : ViewModel
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x00008320 File Offset: 0x00006520
		private Mission Mission
		{
			get
			{
				return Mission.Current;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x00008327 File Offset: 0x00006527
		private Team Team
		{
			get
			{
				return Mission.Current.PlayerTeam;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x00008333 File Offset: 0x00006533
		public OrderController OrderController
		{
			get
			{
				return this.Team.PlayerOrderController;
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00008340 File Offset: 0x00006540
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

		// Token: 0x060001F5 RID: 501 RVA: 0x00008434 File Offset: 0x00006634
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

		// Token: 0x060001F6 RID: 502 RVA: 0x0000849D File Offset: 0x0000669D
		public void ExecuteSelectAll()
		{
			this.SelectAllFormations(true);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x000084A8 File Offset: 0x000066A8
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

		// Token: 0x060001F8 RID: 504 RVA: 0x00008540 File Offset: 0x00006740
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

		// Token: 0x060001F9 RID: 505 RVA: 0x0000864D File Offset: 0x0000684D
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

		// Token: 0x060001FA RID: 506 RVA: 0x0000866C File Offset: 0x0000686C
		internal void SetTroopActiveOrders(OrderTroopItemVM item)
		{
			bool flag = BannerlordConfig.OrderLayoutType == 1;
			item.ActiveOrders.Clear();
			List<OrderSubType> list = new List<OrderSubType>();
			OrderType orderType = MissionOrderVM.GetOrderOverrideForUI(item.Formation, OrderSetType.Movement);
			if (orderType == OrderType.None)
			{
				orderType = OrderController.GetActiveMovementOrderOf(item.Formation);
			}
			switch (orderType)
			{
			case OrderType.Move:
				list.Add(OrderSubType.MoveToPosition);
				goto IL_D1;
			case OrderType.MoveToLineSegment:
			case OrderType.MoveToLineSegmentWithHorizontalLayout:
			case OrderType.ChargeWithTarget:
				break;
			case OrderType.Charge:
				list.Add(OrderSubType.Charge);
				goto IL_D1;
			case OrderType.StandYourGround:
				list.Add(OrderSubType.Stop);
				goto IL_D1;
			case OrderType.FollowMe:
				list.Add(OrderSubType.FollowMe);
				goto IL_D1;
			default:
				switch (orderType)
				{
				case OrderType.Retreat:
					list.Add(OrderSubType.Retreat);
					goto IL_D1;
				case OrderType.Advance:
					list.Add(OrderSubType.Advance);
					goto IL_D1;
				case OrderType.FallBack:
					list.Add(OrderSubType.Fallback);
					goto IL_D1;
				}
				break;
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 164);
			IL_D1:
			OrderType orderType2 = MissionOrderVM.GetOrderOverrideForUI(item.Formation, OrderSetType.Form);
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
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 209);
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
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 223);
				}
			}
			orderType3 = OrderController.GetActiveFiringOrderOf(item.Formation);
			if (orderType3 != OrderType.HoldFire)
			{
				if (orderType3 != OrderType.FireAtWill)
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 237);
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
						Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 253);
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
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "SetTroopActiveOrders", 269);
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

		// Token: 0x060001FB RID: 507 RVA: 0x0000895C File Offset: 0x00006B5C
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

		// Token: 0x060001FC RID: 508 RVA: 0x00008A6C File Offset: 0x00006C6C
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

		// Token: 0x060001FD RID: 509 RVA: 0x00008AB4 File Offset: 0x00006CB4
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

		// Token: 0x060001FE RID: 510 RVA: 0x00008B20 File Offset: 0x00006D20
		public void OnDeselectFormation(int index)
		{
			OrderTroopItemVM orderTroopItemVM = this.TroopList.FirstOrDefault((OrderTroopItemVM t) => t.Formation.Index == index);
			this.OnDeselectFormation(orderTroopItemVM);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00008B5C File Offset: 0x00006D5C
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

		// Token: 0x06000200 RID: 512 RVA: 0x00008C0C File Offset: 0x00006E0C
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

		// Token: 0x06000201 RID: 513 RVA: 0x00008D90 File Offset: 0x00006F90
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

		// Token: 0x06000202 RID: 514 RVA: 0x00008E18 File Offset: 0x00007018
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

		// Token: 0x06000203 RID: 515 RVA: 0x00008FFC File Offset: 0x000071FC
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

		// Token: 0x06000204 RID: 516 RVA: 0x000090EC File Offset: 0x000072EC
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

		// Token: 0x06000205 RID: 517 RVA: 0x00009180 File Offset: 0x00007380
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

		// Token: 0x06000206 RID: 518 RVA: 0x00009594 File Offset: 0x00007794
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

		// Token: 0x06000207 RID: 519 RVA: 0x00009604 File Offset: 0x00007804
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

		// Token: 0x06000208 RID: 520 RVA: 0x00009748 File Offset: 0x00007948
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

		// Token: 0x06000209 RID: 521 RVA: 0x0000979C File Offset: 0x0000799C
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

		// Token: 0x0600020A RID: 522 RVA: 0x00009860 File Offset: 0x00007A60
		private void GetMaxAndCurrentAmmoOfAgent(Agent agent, out int currentAmmo, out int maxAmmo)
		{
			currentAmmo = 0;
			maxAmmo = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (!agent.Equipment[equipmentIndex].IsEmpty && agent.Equipment[equipmentIndex].CurrentUsageItem.IsRangedWeapon)
				{
					currentAmmo = agent.Equipment.GetAmmoAmount(agent.Equipment[equipmentIndex].CurrentUsageItem.AmmoClass);
					maxAmmo = agent.Equipment.GetMaxAmmo(agent.Equipment[equipmentIndex].CurrentUsageItem.AmmoClass);
					return;
				}
			}
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00009904 File Offset: 0x00007B04
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

		// Token: 0x0600020C RID: 524 RVA: 0x000099BC File Offset: 0x00007BBC
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

		// Token: 0x0600020D RID: 525 RVA: 0x00009A15 File Offset: 0x00007C15
		private void SortFormations()
		{
			this.TroopList.Sort(this._formationIndexComparer);
			this.TransferTargetList.Sort(this._formationIndexComparer);
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00009A39 File Offset: 0x00007C39
		private int GetFormationMorale(Formation formation)
		{
			if (!this._isDeployment)
			{
				return (int)MissionGameModels.Current.BattleMoraleModel.GetAverageMorale(formation);
			}
			return 0;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00009A58 File Offset: 0x00007C58
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
				Debug.FailedAssert("Added troop item is null!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\MissionOrderTroopControllerVM.cs", "AddTroopItemIfNotExist", 861);
			}
			return orderTroopItemVM;
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000210 RID: 528 RVA: 0x00009AF7 File Offset: 0x00007CF7
		// (set) Token: 0x06000211 RID: 529 RVA: 0x00009B00 File Offset: 0x00007D00
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

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000212 RID: 530 RVA: 0x00009C40 File Offset: 0x00007E40
		// (set) Token: 0x06000213 RID: 531 RVA: 0x00009C48 File Offset: 0x00007E48
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

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000214 RID: 532 RVA: 0x00009C73 File Offset: 0x00007E73
		// (set) Token: 0x06000215 RID: 533 RVA: 0x00009C7B File Offset: 0x00007E7B
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

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000216 RID: 534 RVA: 0x00009C98 File Offset: 0x00007E98
		// (set) Token: 0x06000217 RID: 535 RVA: 0x00009CA0 File Offset: 0x00007EA0
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

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000218 RID: 536 RVA: 0x00009CBD File Offset: 0x00007EBD
		// (set) Token: 0x06000219 RID: 537 RVA: 0x00009CC5 File Offset: 0x00007EC5
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

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600021A RID: 538 RVA: 0x00009CE2 File Offset: 0x00007EE2
		// (set) Token: 0x0600021B RID: 539 RVA: 0x00009CEA File Offset: 0x00007EEA
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

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600021C RID: 540 RVA: 0x00009D0C File Offset: 0x00007F0C
		// (set) Token: 0x0600021D RID: 541 RVA: 0x00009D14 File Offset: 0x00007F14
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

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600021E RID: 542 RVA: 0x00009D36 File Offset: 0x00007F36
		// (set) Token: 0x0600021F RID: 543 RVA: 0x00009D3E File Offset: 0x00007F3E
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

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000220 RID: 544 RVA: 0x00009D60 File Offset: 0x00007F60
		// (set) Token: 0x06000221 RID: 545 RVA: 0x00009D68 File Offset: 0x00007F68
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

		// Token: 0x040000E7 RID: 231
		private readonly MissionOrderVM _missionOrder;

		// Token: 0x040000E8 RID: 232
		private readonly Action _onTransferFinised;

		// Token: 0x040000E9 RID: 233
		private readonly bool _isMultiplayer;

		// Token: 0x040000EA RID: 234
		private List<ValueTuple<int, List<int>>> _filterData;

		// Token: 0x040000EB RID: 235
		private bool _isDeployment;

		// Token: 0x040000EC RID: 236
		private MissionOrderTroopControllerVM.TroopItemFormationIndexComparer _formationIndexComparer;

		// Token: 0x040000ED RID: 237
		private MBBindingList<OrderTroopItemVM> _troopList;

		// Token: 0x040000EE RID: 238
		private bool _isTransferActive;

		// Token: 0x040000EF RID: 239
		private MBBindingList<OrderTroopItemVM> _transferTargetList;

		// Token: 0x040000F0 RID: 240
		private int _transferValue;

		// Token: 0x040000F1 RID: 241
		private int _transferMaxValue;

		// Token: 0x040000F2 RID: 242
		private string _transferTitleText;

		// Token: 0x040000F3 RID: 243
		private string _acceptText;

		// Token: 0x040000F4 RID: 244
		private string _cancelText;

		// Token: 0x040000F5 RID: 245
		private bool _isTransferValid;

		// Token: 0x0200012E RID: 302
		private class TroopItemFormationIndexComparer : IComparer<OrderTroopItemVM>
		{
			// Token: 0x0600183D RID: 6205 RVA: 0x0004FC5C File Offset: 0x0004DE5C
			public int Compare(OrderTroopItemVM x, OrderTroopItemVM y)
			{
				return x.Formation.Index.CompareTo(y.Formation.Index);
			}
		}
	}
}
