using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000017 RID: 23
	public class MissionOrderDeploymentControllerVM : ViewModel
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001C7 RID: 455 RVA: 0x00007195 File Offset: 0x00005395
		private Mission Mission
		{
			get
			{
				return Mission.Current;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000719C File Offset: 0x0000539C
		private Team Team
		{
			get
			{
				return Mission.Current.PlayerTeam;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x000071A8 File Offset: 0x000053A8
		public OrderController OrderController
		{
			get
			{
				return this.Team.PlayerOrderController;
			}
		}

		// Token: 0x060001CA RID: 458 RVA: 0x000071B8 File Offset: 0x000053B8
		public MissionOrderDeploymentControllerVM(List<DeploymentPoint> deploymentPoints, MissionOrderVM missionOrder, Camera deploymentCamera, Action<bool> toggleMissionInputs, OnRefreshVisualsDelegate onRefreshVisuals)
		{
			this._deploymentPoints = deploymentPoints;
			this._missionOrder = missionOrder;
			this._deploymentCamera = deploymentCamera;
			this._toggleMissionInputs = toggleMissionInputs;
			this._onRefreshVisuals = onRefreshVisuals;
			this.SiegeMachineList = new MBBindingList<OrderSiegeMachineVM>();
			this.SiegeDeploymentList = new MBBindingList<DeploymentSiegeMachineVM>();
			this.DeploymentTargets = new MBBindingList<DeploymentSiegeMachineVM>();
			MBTextManager.SetTextVariable("UNDEPLOYED_SIEGE_MACHINE_COUNT", this.SiegeMachineList.Count((OrderSiegeMachineVM s) => !s.SiegeWeapon.IsUsed).ToString(), false);
			this._siegeDeployQueryData = new InquiryData(new TextObject("{=TxphX8Uk}Deployment", null).ToString(), new TextObject("{=LlrlE199}You can still deploy siege engines.\nBegin anyway?", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), delegate
			{
				this._siegeDeploymentHandler.FinishDeployment();
				this._missionOrder.TryCloseToggleOrder(false);
			}, null, "", 0f, null, null, null);
			this.SiegeMachineList.Clear();
			this.OrderController.SiegeWeaponController.OnSelectedSiegeWeaponsChanged += this.OnSelectedSiegeWeaponsChanged;
			this._siegeDeploymentHandler = this.Mission.GetMissionBehavior<SiegeDeploymentHandler>();
			if (this._siegeDeploymentHandler != null)
			{
				this._siegeDeploymentHandler.OnDeploymentReady += this.ExecuteDeployAll;
			}
			else
			{
				this._battleDeploymentHandler = this.Mission.GetMissionBehavior<BattleDeploymentHandler>();
				if (this._battleDeploymentHandler != null)
				{
					this._battleDeploymentHandler.OnDeploymentReady += this.ExecuteDeployAll;
				}
			}
			this.SiegeDeploymentList.Clear();
			if (this._siegeDeploymentHandler != null)
			{
				int num = 1;
				foreach (DeploymentPoint deploymentPoint in this._deploymentPoints)
				{
					OrderSiegeMachineVM orderSiegeMachineVM = new OrderSiegeMachineVM(deploymentPoint, new Action<OrderSiegeMachineVM>(this.OnSelectOrderSiegeMachine), num++);
					this.SiegeMachineList.Add(orderSiegeMachineVM);
					if (deploymentPoint.DeployableWeapons.Any((SynchedMissionObject x) => this._siegeDeploymentHandler.GetMaxDeployableWeaponCountOfPlayer(x.GetType()) > 0))
					{
						DeploymentSiegeMachineVM deploymentSiegeMachineVM = new DeploymentSiegeMachineVM(deploymentPoint, null, this._deploymentCamera, new Action<DeploymentSiegeMachineVM>(this.OnRefreshSelectedDeploymentPoint), new Action<DeploymentPoint>(this.OnEntityHover), deploymentPoint.IsDeployed);
						this.DeploymentTargets.Add(deploymentSiegeMachineVM);
					}
				}
			}
			this.RefreshValues();
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000741C File Offset: 0x0000561C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._siegeMachineList.ApplyActionOnAllItems(delegate(OrderSiegeMachineVM x)
			{
				x.RefreshValues();
			});
			this._siegeDeploymentList.ApplyActionOnAllItems(delegate(DeploymentSiegeMachineVM x)
			{
				x.RefreshValues();
			});
			this._deploymentTargets.ApplyActionOnAllItems(delegate(DeploymentSiegeMachineVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060001CC RID: 460 RVA: 0x000074AD File Offset: 0x000056AD
		public void SetIsOrderPreconfigured(bool isPreconfigured)
		{
			this._isOrderPreconfigured = isPreconfigured;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x000074B8 File Offset: 0x000056B8
		internal void Update()
		{
			for (int i = 0; i < this.DeploymentTargets.Count; i++)
			{
				this.DeploymentTargets[i].Update();
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x000074EC File Offset: 0x000056EC
		internal void DeployFormationsOfPlayer()
		{
			if (!this.Mission.PlayerTeam.IsPlayerGeneral)
			{
				this.Mission.AutoDeployTeamUsingTeamAI(this.Mission.PlayerTeam, false);
			}
			else if (this.Mission.IsSiegeBattle)
			{
				this.Mission.AutoDeployTeamUsingTeamAI(this.Mission.PlayerTeam, false);
			}
			AssignPlayerRoleInTeamMissionController missionBehavior = Mission.Current.GetMissionBehavior<AssignPlayerRoleInTeamMissionController>();
			if (missionBehavior == null)
			{
				return;
			}
			missionBehavior.OnPlayerTeamDeployed();
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000755C File Offset: 0x0000575C
		internal void SetSiegeMachineActiveOrders(OrderSiegeMachineVM siegeItemVM)
		{
			siegeItemVM.ActiveOrders.Clear();
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000756C File Offset: 0x0000576C
		internal void ProcessSiegeMachines()
		{
			foreach (OrderSiegeMachineVM orderSiegeMachineVM in this.SiegeMachineList)
			{
				orderSiegeMachineVM.RefreshSiegeWeapon();
				if (!orderSiegeMachineVM.IsSelectable && orderSiegeMachineVM.IsSelected)
				{
					this.OnDeselectSiegeMachine(orderSiegeMachineVM);
				}
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x000075D0 File Offset: 0x000057D0
		internal void SelectAllSiegeMachines()
		{
			this.OrderController.SiegeWeaponController.SelectAll();
			foreach (OrderSiegeMachineVM orderSiegeMachineVM in this.SiegeMachineList)
			{
				orderSiegeMachineVM.IsSelected = orderSiegeMachineVM.IsSelectable;
				if (orderSiegeMachineVM.IsSelectable)
				{
					this.SetSiegeMachineActiveOrders(orderSiegeMachineVM);
				}
			}
			this._missionOrder.SetActiveOrders();
			this._onRefreshVisuals();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00007658 File Offset: 0x00005858
		internal void AddSelectedSiegeMachine(OrderSiegeMachineVM item)
		{
			if (!item.IsSelectable)
			{
				return;
			}
			this.OrderController.SiegeWeaponController.Select(item.SiegeWeapon);
			item.IsSelected = true;
			this._missionOrder.SetActiveOrders();
			this._onRefreshVisuals();
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00007698 File Offset: 0x00005898
		internal void SetSelectedSiegeMachine(OrderSiegeMachineVM item)
		{
			this.ProcessSiegeMachines();
			if (!item.IsSelectable)
			{
				return;
			}
			this.SetSiegeMachineActiveOrders(item);
			this.OrderController.SiegeWeaponController.ClearSelectedWeapons();
			foreach (OrderSiegeMachineVM orderSiegeMachineVM in this.SiegeMachineList)
			{
				orderSiegeMachineVM.IsSelected = false;
			}
			this.AddSelectedSiegeMachine(item);
			this._onRefreshVisuals();
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000771C File Offset: 0x0000591C
		internal void OnDeselectSiegeMachine(OrderSiegeMachineVM item)
		{
			if (this.OrderController.SiegeWeaponController.SelectedWeapons.Contains(item.SiegeWeapon))
			{
				this.OrderController.SiegeWeaponController.Deselect(item.SiegeWeapon);
			}
			item.IsSelected = false;
			this._missionOrder.SetActiveOrders();
			this._onRefreshVisuals();
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000777C File Offset: 0x0000597C
		internal void OnSelectOrderSiegeMachine(OrderSiegeMachineVM item)
		{
			this.ProcessSiegeMachines();
			this._missionOrder.IsTroopPlacingActive = false;
			if (item.IsSelectable)
			{
				if (Input.DebugInput.IsControlDown())
				{
					if (item.IsSelected)
					{
						this.OnDeselectSiegeMachine(item);
					}
					else
					{
						this.AddSelectedSiegeMachine(item);
					}
				}
				else
				{
					this.SetSelectedSiegeMachine(item);
				}
				this._onRefreshVisuals();
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x000077DC File Offset: 0x000059DC
		internal void OnSelectDeploymentSiegeMachine(DeploymentSiegeMachineVM item)
		{
			this.IsSiegeDeploymentListActive = false;
			GameEntity currentSelectedEntity = this._currentSelectedEntity;
			if (currentSelectedEntity != null)
			{
				currentSelectedEntity.SetContourColor(null, true);
			}
			this._currentSelectedEntity = null;
			this._selectedDeploymentPointVM = null;
			this.SiegeDeploymentList.Clear();
			bool flag = false;
			if (item != null && (!(item.MachineType != null) || this._siegeDeploymentHandler.GetDeployableWeaponCountOfPlayer(item.MachineType) != 0) && (item.DeploymentPoint.DeployedWeapon == null || !(item.DeploymentPoint.DeployedWeapon.GetType() == item.MachineType)))
			{
				bool flag2 = !item.DeploymentPoint.IsDeployed || item.DeploymentPoint.DeployedWeapon != item.SiegeWeapon;
				if (item.DeploymentPoint.IsDeployed)
				{
					if (item.SiegeWeapon == null)
					{
						SoundEvent.PlaySound2D("event:/ui/dropdown");
					}
					item.DeploymentPoint.Disband();
				}
				flag = !this.SiegeMachineList.Any((OrderSiegeMachineVM s) => s.DeploymentPoint.IsDeployed);
				if (flag2 && item.SiegeWeapon != null)
				{
					SiegeEngineType machine = item.Machine;
					if (machine == DefaultSiegeEngineTypes.Catapult || machine == DefaultSiegeEngineTypes.FireCatapult || machine == DefaultSiegeEngineTypes.Onager || machine == DefaultSiegeEngineTypes.FireOnager)
					{
						SoundEvent.PlaySound2D("event:/ui/mission/catapult");
					}
					else if (machine == DefaultSiegeEngineTypes.Ram)
					{
						SoundEvent.PlaySound2D("event:/ui/mission/batteringram");
					}
					else if (machine == DefaultSiegeEngineTypes.SiegeTower)
					{
						SoundEvent.PlaySound2D("event:/ui/mission/siegetower");
					}
					else if (machine == DefaultSiegeEngineTypes.Trebuchet || machine == DefaultSiegeEngineTypes.Bricole)
					{
						SoundEvent.PlaySound2D("event:/ui/mission/catapult");
					}
					else if (machine == DefaultSiegeEngineTypes.Ballista || machine == DefaultSiegeEngineTypes.FireBallista)
					{
						SoundEvent.PlaySound2D("event:/ui/mission/ballista");
					}
					item.DeploymentPoint.Deploy(item.SiegeWeapon);
				}
			}
			this.ProcessSiegeMachines();
			if (flag && this._missionOrder.IsToggleOrderShown)
			{
				this._missionOrder.SetActiveOrders();
			}
			this._onRefreshVisuals();
			foreach (DeploymentSiegeMachineVM deploymentSiegeMachineVM in this.DeploymentTargets)
			{
				deploymentSiegeMachineVM.RefreshWithDeployedWeapon();
			}
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x00007A20 File Offset: 0x00005C20
		internal void OnSelectedSiegeWeaponsChanged()
		{
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00007A22 File Offset: 0x00005C22
		public void OnRefreshSelectedDeploymentPoint(DeploymentSiegeMachineVM item)
		{
			this.RefreshSelectedDeploymentPoint(item.DeploymentPoint);
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00007A30 File Offset: 0x00005C30
		public void OnEntityHover(GameEntity hoveredEntity)
		{
			if (this._currentHoveredEntity == hoveredEntity)
			{
				return;
			}
			DeploymentPoint deploymentPoint = null;
			if (hoveredEntity != null)
			{
				if (hoveredEntity.HasScriptOfType<DeploymentPoint>())
				{
					deploymentPoint = hoveredEntity.GetFirstScriptOfType<DeploymentPoint>();
				}
				else if (this._siegeDeploymentHandler != null)
				{
					deploymentPoint = this._siegeDeploymentHandler.PlayerDeploymentPoints.SingleOrDefault((DeploymentPoint dp) => dp.IsDeployed && hoveredEntity.GetScriptComponents().Any((ScriptComponentBehavior sc) => dp.DeployedWeapon == sc));
				}
			}
			this.OnEntityHover(deploymentPoint);
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00007AB8 File Offset: 0x00005CB8
		public void OnEntityHover(DeploymentPoint deploymentPoint)
		{
			if (this._currentSelectedEntity != this._currentHoveredEntity)
			{
				GameEntity currentHoveredEntity = this._currentHoveredEntity;
				if (currentHoveredEntity != null)
				{
					currentHoveredEntity.SetContourColor(null, true);
				}
			}
			if (deploymentPoint != null)
			{
				this._currentHoveredEntity = (deploymentPoint.IsDeployed ? deploymentPoint.DeployedWeapon.GameEntity : deploymentPoint.GameEntity);
			}
			else
			{
				this._currentHoveredEntity = null;
			}
			if (this._currentSelectedEntity != this._currentHoveredEntity)
			{
				GameEntity currentHoveredEntity2 = this._currentHoveredEntity;
				if (currentHoveredEntity2 == null)
				{
					return;
				}
				currentHoveredEntity2.SetContourColor(new uint?(4289622555U), true);
			}
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00007B50 File Offset: 0x00005D50
		public void OnEntitySelect(GameEntity selectedEntity)
		{
			if (this._currentSelectedEntity == selectedEntity)
			{
				return;
			}
			DeploymentPoint deploymentPoint = null;
			if (selectedEntity != null && this._siegeDeploymentHandler != null)
			{
				if (selectedEntity.HasScriptOfType<DeploymentPoint>())
				{
					deploymentPoint = selectedEntity.GetFirstScriptOfType<DeploymentPoint>();
				}
				else if (this._siegeDeploymentHandler != null)
				{
					deploymentPoint = this._siegeDeploymentHandler.PlayerDeploymentPoints.SingleOrDefault((DeploymentPoint dp) => dp.IsDeployed && selectedEntity.GetScriptComponents().Any((ScriptComponentBehavior sc) => dp.DeployedWeapon == sc));
				}
			}
			if (deploymentPoint != null)
			{
				this._missionOrder.IsTroopPlacingActive = false;
				this.RefreshSelectedDeploymentPoint(deploymentPoint);
				return;
			}
			this.ExecuteCancelSelectedDeploymentPoint();
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00007BF4 File Offset: 0x00005DF4
		public void RefreshSelectedDeploymentPoint(DeploymentPoint selectedDeploymentPoint)
		{
			this.IsSiegeDeploymentListActive = false;
			foreach (DeploymentSiegeMachineVM deploymentSiegeMachineVM in this.DeploymentTargets)
			{
				if (deploymentSiegeMachineVM.DeploymentPoint == selectedDeploymentPoint)
				{
					this._selectedDeploymentPointVM = deploymentSiegeMachineVM;
				}
			}
			if (!this._selectedDeploymentPointVM.IsSelected)
			{
				this._selectedDeploymentPointVM.IsSelected = true;
			}
			this.SiegeDeploymentList.Clear();
			DeploymentSiegeMachineVM deploymentSiegeMachineVM2;
			foreach (SynchedMissionObject synchedMissionObject in selectedDeploymentPoint.DeployableWeapons)
			{
				Type type = synchedMissionObject.GetType();
				if (this._siegeDeploymentHandler.GetMaxDeployableWeaponCountOfPlayer(type) > 0)
				{
					deploymentSiegeMachineVM2 = new DeploymentSiegeMachineVM(selectedDeploymentPoint, synchedMissionObject as SiegeWeapon, this._deploymentCamera, new Action<DeploymentSiegeMachineVM>(this.OnSelectDeploymentSiegeMachine), null, selectedDeploymentPoint.IsDeployed && selectedDeploymentPoint.DeployedWeapon == synchedMissionObject);
					this.SiegeDeploymentList.Add(deploymentSiegeMachineVM2);
					deploymentSiegeMachineVM2.RemainingCount = this._siegeDeploymentHandler.GetDeployableWeaponCountOfPlayer(type);
				}
			}
			deploymentSiegeMachineVM2 = new DeploymentSiegeMachineVM(selectedDeploymentPoint, null, this._deploymentCamera, new Action<DeploymentSiegeMachineVM>(this.OnSelectDeploymentSiegeMachine), null, !selectedDeploymentPoint.IsDeployed);
			this.SiegeDeploymentList.Add(deploymentSiegeMachineVM2);
			selectedDeploymentPoint.GameEntity.SetContourColor(new uint?(4293481743U), true);
			this.IsSiegeDeploymentListActive = true;
			GameEntity currentSelectedEntity = this._currentSelectedEntity;
			if (currentSelectedEntity != null)
			{
				currentSelectedEntity.SetContourColor(null, true);
			}
			this._currentSelectedEntity = selectedDeploymentPoint.GameEntity;
			GameEntity currentSelectedEntity2 = this._currentSelectedEntity;
			if (currentSelectedEntity2 == null)
			{
				return;
			}
			currentSelectedEntity2.SetContourColor(new uint?(4293481743U), true);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00007DAC File Offset: 0x00005FAC
		public void ExecuteCancelSelectedDeploymentPoint()
		{
			this.OnSelectDeploymentSiegeMachine(null);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00007DB8 File Offset: 0x00005FB8
		public void ExecuteBeginSiege()
		{
			this.IsSiegeDeploymentListActive = false;
			if (this._siegeDeploymentHandler != null && this._siegeDeploymentHandler.PlayerDeploymentPoints.Any((DeploymentPoint d) => !d.IsDeployed && d.DeployableWeaponTypes.Any((Type type) => this._siegeDeploymentHandler.GetDeployableWeaponCountOfPlayer(type) > 0)))
			{
				InformationManager.ShowInquiry(this._siegeDeployQueryData, false, false);
				return;
			}
			this._missionOrder.TryCloseToggleOrder(false);
			if (this._siegeDeploymentHandler != null)
			{
				this._siegeDeploymentHandler.FinishDeployment();
				return;
			}
			this._battleDeploymentHandler.FinishDeployment();
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00007E30 File Offset: 0x00006030
		public void ExecuteAutoDeploy()
		{
			this.Mission.TryRemakeInitialDeploymentPlanForBattleSide(this.Mission.PlayerTeam.Side);
			if (this.Mission.IsSiegeBattle)
			{
				this.Mission.AutoDeployTeamUsingTeamAI(this.Mission.PlayerTeam, false);
				return;
			}
			this.Mission.AutoDeployTeamUsingDeploymentPlan(this.Mission.PlayerTeam);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00007E94 File Offset: 0x00006094
		public void ExecuteDeployAll()
		{
			if (this._siegeDeploymentHandler != null)
			{
				this._siegeDeploymentHandler.DeployAllSiegeWeaponsOfPlayer();
				this.Mission.ForceTickOccasionally = true;
				bool isTeleportingAgents = Mission.Current.IsTeleportingAgents;
				this.Mission.IsTeleportingAgents = true;
				this.DeployFormationsOfPlayer();
				this._siegeDeploymentHandler.ForceUpdateAllUnits();
				this._missionOrder.OnDeployAll();
				foreach (OrderSiegeMachineVM orderSiegeMachineVM in this.SiegeMachineList)
				{
					orderSiegeMachineVM.RefreshSiegeWeapon();
				}
				foreach (DeploymentSiegeMachineVM deploymentSiegeMachineVM in this.DeploymentTargets)
				{
					deploymentSiegeMachineVM.RefreshWithDeployedWeapon();
				}
				this.Mission.IsTeleportingAgents = isTeleportingAgents;
				this.Mission.ForceTickOccasionally = false;
				this.SelectAllSiegeMachines();
				return;
			}
			if (this._battleDeploymentHandler != null)
			{
				this.DeployFormationsOfPlayer();
				this._battleDeploymentHandler.ForceUpdateAllUnits();
				this._missionOrder.OnDeployAll();
			}
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00007FB0 File Offset: 0x000061B0
		public void FinalizeDeployment()
		{
			this._missionOrder.IsDeployment = false;
			foreach (OrderSiegeMachineVM orderSiegeMachineVM in this.SiegeMachineList.ToList<OrderSiegeMachineVM>())
			{
				if (orderSiegeMachineVM.DeploymentPoint.IsDeployed)
				{
					this.SetSiegeMachineActiveOrders(orderSiegeMachineVM);
				}
				else
				{
					this.SiegeMachineList.Remove(orderSiegeMachineVM);
				}
			}
			this.DeploymentTargets.Clear();
			this.SiegeDeploymentList.Clear();
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00008048 File Offset: 0x00006248
		internal void OnSelectFormationWithIndex(int formationTroopIndex)
		{
			OrderSiegeMachineVM orderSiegeMachineVM = this.SiegeMachineList.ElementAtOrDefault(formationTroopIndex);
			if (orderSiegeMachineVM != null)
			{
				this.OnSelectOrderSiegeMachine(orderSiegeMachineVM);
				return;
			}
			this.SelectAllSiegeMachines();
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00008074 File Offset: 0x00006274
		internal void SetCurrentActiveOrders()
		{
			List<OrderSubjectVM> list = (from item in this.SiegeMachineList.Cast<OrderSubjectVM>().ToList<OrderSubjectVM>()
				where item.IsSelected && item.IsSelectable
				select item).ToList<OrderSubjectVM>();
			if (list.IsEmpty<OrderSubjectVM>())
			{
				this.OrderController.SiegeWeaponController.SelectAll();
				foreach (OrderSiegeMachineVM orderSiegeMachineVM in this.SiegeMachineList.Where((OrderSiegeMachineVM s) => s.IsSelectable && s.DeploymentPoint.IsDeployed))
				{
					orderSiegeMachineVM.IsSelected = true;
					this.SetSiegeMachineActiveOrders(orderSiegeMachineVM);
					list.Add(orderSiegeMachineVM);
				}
			}
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00008148 File Offset: 0x00006348
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.OrderController.SiegeWeaponController.OnSelectedSiegeWeaponsChanged -= this.OnSelectedSiegeWeaponsChanged;
			this.SiegeDeploymentList.Clear();
			foreach (OrderSiegeMachineVM orderSiegeMachineVM in this.SiegeMachineList.ToList<OrderSiegeMachineVM>())
			{
				if (!orderSiegeMachineVM.DeploymentPoint.IsDeployed)
				{
					this.SiegeMachineList.Remove(orderSiegeMachineVM);
				}
			}
			this._siegeDeploymentHandler = null;
			this._siegeDeployQueryData = null;
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001E5 RID: 485 RVA: 0x000081F0 File Offset: 0x000063F0
		// (set) Token: 0x060001E6 RID: 486 RVA: 0x000081F8 File Offset: 0x000063F8
		[DataSourceProperty]
		public MBBindingList<OrderSiegeMachineVM> SiegeMachineList
		{
			get
			{
				return this._siegeMachineList;
			}
			set
			{
				if (value != this._siegeMachineList)
				{
					this._siegeMachineList = value;
					base.OnPropertyChanged("SiegeMachineList");
				}
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001E7 RID: 487 RVA: 0x00008215 File Offset: 0x00006415
		// (set) Token: 0x060001E8 RID: 488 RVA: 0x0000821D File Offset: 0x0000641D
		[DataSourceProperty]
		public MBBindingList<DeploymentSiegeMachineVM> DeploymentTargets
		{
			get
			{
				return this._deploymentTargets;
			}
			set
			{
				if (value != this._deploymentTargets)
				{
					this._deploymentTargets = value;
					base.OnPropertyChanged("DeploymentTargets");
				}
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001E9 RID: 489 RVA: 0x0000823A File Offset: 0x0000643A
		// (set) Token: 0x060001EA RID: 490 RVA: 0x00008244 File Offset: 0x00006444
		[DataSourceProperty]
		public bool IsSiegeDeploymentListActive
		{
			get
			{
				return this._isSiegeDeploymentListActive;
			}
			set
			{
				if (value != this._isSiegeDeploymentListActive)
				{
					this._isSiegeDeploymentListActive = value;
					base.OnPropertyChanged("IsSiegeDeploymentListActive");
					this._toggleMissionInputs(value);
					this._onRefreshVisuals();
					if (this._selectedDeploymentPointVM != null)
					{
						this._selectedDeploymentPointVM.IsSelected = value;
					}
				}
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001EB RID: 491 RVA: 0x00008297 File Offset: 0x00006497
		// (set) Token: 0x060001EC RID: 492 RVA: 0x0000829F File Offset: 0x0000649F
		[DataSourceProperty]
		public MBBindingList<DeploymentSiegeMachineVM> SiegeDeploymentList
		{
			get
			{
				return this._siegeDeploymentList;
			}
			set
			{
				if (value != this._siegeDeploymentList)
				{
					this._siegeDeploymentList = value;
					base.OnPropertyChanged("SiegeDeploymentList");
				}
			}
		}

		// Token: 0x040000D5 RID: 213
		public const uint ENTITYHIGHLIGHTCOLOR = 4289622555U;

		// Token: 0x040000D6 RID: 214
		public const uint ENTITYSELECTEDCOLOR = 4293481743U;

		// Token: 0x040000D7 RID: 215
		private GameEntity _currentSelectedEntity;

		// Token: 0x040000D8 RID: 216
		private GameEntity _currentHoveredEntity;

		// Token: 0x040000D9 RID: 217
		private InquiryData _siegeDeployQueryData;

		// Token: 0x040000DA RID: 218
		private SiegeDeploymentHandler _siegeDeploymentHandler;

		// Token: 0x040000DB RID: 219
		private BattleDeploymentHandler _battleDeploymentHandler;

		// Token: 0x040000DC RID: 220
		private readonly List<DeploymentPoint> _deploymentPoints;

		// Token: 0x040000DD RID: 221
		internal DeploymentSiegeMachineVM _selectedDeploymentPointVM;

		// Token: 0x040000DE RID: 222
		private readonly MissionOrderVM _missionOrder;

		// Token: 0x040000DF RID: 223
		private readonly Camera _deploymentCamera;

		// Token: 0x040000E0 RID: 224
		private readonly Action<bool> _toggleMissionInputs;

		// Token: 0x040000E1 RID: 225
		private readonly OnRefreshVisualsDelegate _onRefreshVisuals;

		// Token: 0x040000E2 RID: 226
		private bool _isOrderPreconfigured;

		// Token: 0x040000E3 RID: 227
		private MBBindingList<OrderSiegeMachineVM> _siegeMachineList;

		// Token: 0x040000E4 RID: 228
		private MBBindingList<DeploymentSiegeMachineVM> _siegeDeploymentList;

		// Token: 0x040000E5 RID: 229
		private MBBindingList<DeploymentSiegeMachineVM> _deploymentTargets;

		// Token: 0x040000E6 RID: 230
		private bool _isSiegeDeploymentListActive;
	}
}
