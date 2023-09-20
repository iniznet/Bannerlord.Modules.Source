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
	public class MissionOrderDeploymentControllerVM : ViewModel
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

		public void SetIsOrderPreconfigured(bool isPreconfigured)
		{
			this._isOrderPreconfigured = isPreconfigured;
		}

		internal void Update()
		{
			for (int i = 0; i < this.DeploymentTargets.Count; i++)
			{
				this.DeploymentTargets[i].Update();
			}
		}

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

		internal void SetSiegeMachineActiveOrders(OrderSiegeMachineVM siegeItemVM)
		{
			siegeItemVM.ActiveOrders.Clear();
		}

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

		internal void OnSelectedSiegeWeaponsChanged()
		{
		}

		public void OnRefreshSelectedDeploymentPoint(DeploymentSiegeMachineVM item)
		{
			this.RefreshSelectedDeploymentPoint(item.DeploymentPoint);
		}

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

		public void ExecuteCancelSelectedDeploymentPoint()
		{
			this.OnSelectDeploymentSiegeMachine(null);
		}

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

		public const uint ENTITYHIGHLIGHTCOLOR = 4289622555U;

		public const uint ENTITYSELECTEDCOLOR = 4293481743U;

		private GameEntity _currentSelectedEntity;

		private GameEntity _currentHoveredEntity;

		private InquiryData _siegeDeployQueryData;

		private SiegeDeploymentHandler _siegeDeploymentHandler;

		private BattleDeploymentHandler _battleDeploymentHandler;

		private readonly List<DeploymentPoint> _deploymentPoints;

		internal DeploymentSiegeMachineVM _selectedDeploymentPointVM;

		private readonly MissionOrderVM _missionOrder;

		private readonly Camera _deploymentCamera;

		private readonly Action<bool> _toggleMissionInputs;

		private readonly OnRefreshVisualsDelegate _onRefreshVisuals;

		private bool _isOrderPreconfigured;

		private MBBindingList<OrderSiegeMachineVM> _siegeMachineList;

		private MBBindingList<DeploymentSiegeMachineVM> _siegeDeploymentList;

		private MBBindingList<DeploymentSiegeMachineVM> _deploymentTargets;

		private bool _isSiegeDeploymentListActive;
	}
}
