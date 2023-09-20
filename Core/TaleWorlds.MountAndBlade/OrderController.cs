using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class OrderController
	{
		public SiegeWeaponController SiegeWeaponController { get; private set; }

		public MBReadOnlyList<Formation> SelectedFormations
		{
			get
			{
				return this._selectedFormations;
			}
		}

		public event OnOrderIssuedDelegate OnOrderIssued;

		public event Action OnSelectedFormationsChanged;

		public Dictionary<Formation, Formation> simulationFormations { get; private set; }

		public OrderController(Mission mission, Team team, Agent owner)
		{
			this._mission = mission;
			this._team = team;
			this.Owner = owner;
			this._gesturesEnabled = true;
			this._selectedFormations = new MBList<Formation>();
			this.SiegeWeaponController = new SiegeWeaponController(mission, this._team);
			this.simulationFormations = new Dictionary<Formation, Formation>();
			this.actualWidths = new Dictionary<Formation, float>();
			this.actualUnitSpacings = new Dictionary<Formation, int>();
			foreach (Formation formation in this._team.FormationsIncludingEmpty)
			{
				formation.OnWidthChanged += this.Formation_OnWidthChanged;
				formation.OnUnitSpacingChanged += this.Formation_OnUnitSpacingChanged;
			}
			if (this._team.IsPlayerGeneral)
			{
				foreach (Formation formation2 in this._team.FormationsIncludingEmpty)
				{
					formation2.PlayerOwner = owner;
				}
			}
			this.CreateDefaultOrderOverrides();
		}

		private void Formation_OnUnitSpacingChanged(Formation formation)
		{
			this.actualUnitSpacings.Remove(formation);
		}

		private void Formation_OnWidthChanged(Formation formation)
		{
			this.actualWidths.Remove(formation);
		}

		private void OnSelectedFormationsCollectionChanged()
		{
			Action onSelectedFormationsChanged = this.OnSelectedFormationsChanged;
			if (onSelectedFormationsChanged != null)
			{
				onSelectedFormationsChanged();
			}
			foreach (Formation formation in this.SelectedFormations.Except(this.simulationFormations.Keys))
			{
				this.simulationFormations[formation] = new Formation(null, -1);
			}
		}

		private void SelectFormation(Formation formation, Agent selectorAgent)
		{
			if (!this._selectedFormations.Contains(formation) && this.IsFormationSelectable(formation, selectorAgent))
			{
				if (GameNetwork.IsClient)
				{
					GameNetwork.BeginModuleEventAsClient();
					GameNetwork.WriteMessage(new SelectFormation(formation.Index));
					GameNetwork.EndModuleEventAsClient();
				}
				if (selectorAgent != null && this.AreGesturesEnabled())
				{
					OrderController.PlayFormationSelectedGesture(formation, selectorAgent);
				}
				MBDebug.Print(((formation != null) ? new FormationClass?(formation.InitialClass) : null) + " added to selected formations.", 0, Debug.DebugColor.White, 17592186044416UL);
				this._selectedFormations.Add(formation);
				this.OnSelectedFormationsCollectionChanged();
				return;
			}
			Debug.FailedAssert("Formation already selected or is not selectable", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "SelectFormation", 208);
		}

		public void SelectFormation(Formation formation)
		{
			this.SelectFormation(formation, this.Owner);
		}

		public void DeselectFormation(Formation formation)
		{
			if (this._selectedFormations.Contains(formation))
			{
				if (GameNetwork.IsClient)
				{
					GameNetwork.BeginModuleEventAsClient();
					GameNetwork.WriteMessage(new UnselectFormation(formation.Index));
					GameNetwork.EndModuleEventAsClient();
				}
				MBDebug.Print(((formation != null) ? new FormationClass?(formation.InitialClass) : null) + " is removed from selected formations.", 0, Debug.DebugColor.White, 17592186044416UL);
				this._selectedFormations.Remove(formation);
				this.OnSelectedFormationsCollectionChanged();
				return;
			}
			Debug.FailedAssert("Trying to deselect an unselected formation", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "DeselectFormation", 234);
		}

		public bool IsFormationListening(Formation formation)
		{
			return this.SelectedFormations.Contains(formation);
		}

		public bool IsFormationSelectable(Formation formation)
		{
			return this.IsFormationSelectable(formation, this.Owner);
		}

		public bool BackupAndDisableGesturesEnabled()
		{
			bool gesturesEnabled = this._gesturesEnabled;
			this._gesturesEnabled = false;
			return gesturesEnabled;
		}

		public void RestoreGesturesEnabled(bool oldValue)
		{
			this._gesturesEnabled = oldValue;
		}

		private bool IsFormationSelectable(Formation formation, Agent selectorAgent)
		{
			return (selectorAgent == null || formation.PlayerOwner == selectorAgent) && formation.CountOfUnits > 0;
		}

		private bool AreGesturesEnabled()
		{
			return this._gesturesEnabled && this._mission.IsOrderGesturesEnabled() && !GameNetwork.IsClientOrReplay;
		}

		private void SelectAllFormations(Agent selectorAgent, bool uiFeedback)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new SelectAllFormations());
				GameNetwork.EndModuleEventAsClient();
			}
			if (uiFeedback && selectorAgent != null && this.AreGesturesEnabled())
			{
				selectorAgent.MakeVoice(SkinVoiceManager.VoiceType.Everyone, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
			}
			MBDebug.Print("Selected formations being cleared. Select all formations:", 0, Debug.DebugColor.White, 17592186044416UL);
			this._selectedFormations.Clear();
			IEnumerable<Formation> formationsIncludingEmpty = this._team.FormationsIncludingEmpty;
			Func<Formation, bool> <>9__0;
			Func<Formation, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (Formation f) => f.CountOfUnits > 0 && this.IsFormationSelectable(f, selectorAgent));
			}
			foreach (Formation formation in formationsIncludingEmpty.Where(func))
			{
				MBDebug.Print(formation.InitialClass + " added to selected formations.", 0, Debug.DebugColor.White, 17592186044416UL);
				this._selectedFormations.Add(formation);
			}
			this.OnSelectedFormationsCollectionChanged();
		}

		public void SelectAllFormations(bool uiFeedback = false)
		{
			this.SelectAllFormations(this.Owner, uiFeedback);
		}

		public void ClearSelectedFormations()
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ClearSelectedFormations());
				GameNetwork.EndModuleEventAsClient();
			}
			MBDebug.Print("Selected formations being cleared.", 0, Debug.DebugColor.White, 17592186044416UL);
			this._selectedFormations.Clear();
			this.OnSelectedFormationsCollectionChanged();
		}

		public void ReleaseFormationsFromAI()
		{
			foreach (Formation formation in this.SelectedFormations)
			{
				formation.ReleaseFormationFromAI();
			}
		}

		public unsafe void SetOrder(OrderType orderType)
		{
			MBDebug.Print("SetOrder " + orderType + "on team", 0, Debug.DebugColor.White, 17592186044416UL);
			this.BeforeSetOrder(orderType);
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplyOrder(orderType));
				GameNetwork.EndModuleEventAsClient();
			}
			switch (orderType)
			{
			case OrderType.Charge:
			{
				using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation = enumerator.Current;
						formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
					}
					goto IL_80E;
				}
				break;
			}
			case OrderType.ChargeWithTarget:
			case OrderType.FollowMe:
			case OrderType.FollowEntity:
			case OrderType.GuardMe:
			case OrderType.LookAtDirection:
			case OrderType.FormCustom:
			case OrderType.CohesionHigh:
			case OrderType.CohesionMedium:
			case OrderType.CohesionLow:
			case OrderType.RideFree:
				goto IL_7F5;
			case OrderType.StandYourGround:
				break;
			case OrderType.Retreat:
				goto IL_2A4;
			case OrderType.AdvanceTenPaces:
				goto IL_160;
			case OrderType.FallBackTenPaces:
				goto IL_1C6;
			case OrderType.Advance:
				goto IL_22E;
			case OrderType.FallBack:
				goto IL_269;
			case OrderType.LookAtEnemy:
				goto IL_2DF;
			case OrderType.ArrangementLine:
				goto IL_530;
			case OrderType.ArrangementCloseOrder:
				goto IL_571;
			case OrderType.ArrangementLoose:
				goto IL_5B2;
			case OrderType.ArrangementCircular:
				goto IL_5F3;
			case OrderType.ArrangementSchiltron:
				goto IL_634;
			case OrderType.ArrangementVee:
				goto IL_675;
			case OrderType.ArrangementColumn:
				goto IL_6B6;
			case OrderType.ArrangementScatter:
				goto IL_6F7;
			case OrderType.FormDeep:
				goto IL_738;
			case OrderType.FormWide:
				goto IL_779;
			case OrderType.FormWider:
				goto IL_7B7;
			case OrderType.HoldFire:
				goto IL_324;
			case OrderType.FireAtWill:
				goto IL_35F;
			case OrderType.Mount:
				goto IL_3F2;
			case OrderType.Dismount:
				goto IL_39A;
			case OrderType.UseAnyWeapon:
				goto IL_44A;
			case OrderType.UseBluntWeaponsOnly:
				goto IL_485;
			case OrderType.AIControlOn:
				goto IL_4C0;
			case OrderType.AIControlOff:
				goto IL_4F8;
			default:
				goto IL_7F5;
			}
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation2 = enumerator.Current;
					formation2.SetMovementOrder(MovementOrder.MovementOrderStop);
				}
				goto IL_80E;
			}
			IL_160:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation3 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation3);
					if (formation3.GetReadonlyMovementOrderReference().OrderEnum == MovementOrder.MovementOrderEnum.Move)
					{
						MovementOrder movementOrder = *formation3.GetReadonlyMovementOrderReference();
						movementOrder.Advance(formation3, 7f);
						formation3.SetMovementOrder(movementOrder);
					}
				}
				goto IL_80E;
			}
			IL_1C6:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation4 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation4);
					if (formation4.GetReadonlyMovementOrderReference().OrderEnum == MovementOrder.MovementOrderEnum.Move)
					{
						MovementOrder movementOrder2 = *formation4.GetReadonlyMovementOrderReference();
						movementOrder2.FallBack(formation4, 7f);
						formation4.SetMovementOrder(movementOrder2);
					}
				}
				goto IL_80E;
			}
			IL_22E:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation5 = enumerator.Current;
					formation5.SetMovementOrder(MovementOrder.MovementOrderAdvance);
				}
				goto IL_80E;
			}
			IL_269:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation6 = enumerator.Current;
					formation6.SetMovementOrder(MovementOrder.MovementOrderFallBack);
				}
				goto IL_80E;
			}
			IL_2A4:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation7 = enumerator.Current;
					formation7.SetMovementOrder(MovementOrder.MovementOrderRetreat);
				}
				goto IL_80E;
			}
			IL_2DF:
			FacingOrder facingOrderLookAtEnemy = FacingOrder.FacingOrderLookAtEnemy;
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation8 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation8);
					formation8.FacingOrder = facingOrderLookAtEnemy;
				}
				goto IL_80E;
			}
			IL_324:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation9 = enumerator.Current;
					formation9.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
				}
				goto IL_80E;
			}
			IL_35F:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation10 = enumerator.Current;
					formation10.FiringOrder = FiringOrder.FiringOrderFireAtWill;
				}
				goto IL_80E;
			}
			IL_39A:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation11 = enumerator.Current;
					if (formation11.IsMounted() || formation11.HasAnyMountedUnit)
					{
						OrderController.TryCancelStopOrder(formation11);
					}
					formation11.RidingOrder = RidingOrder.RidingOrderDismount;
				}
				goto IL_80E;
			}
			IL_3F2:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation12 = enumerator.Current;
					if (formation12.IsMounted() || formation12.HasAnyMountedUnit)
					{
						OrderController.TryCancelStopOrder(formation12);
					}
					formation12.RidingOrder = RidingOrder.RidingOrderMount;
				}
				goto IL_80E;
			}
			IL_44A:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation13 = enumerator.Current;
					formation13.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
				}
				goto IL_80E;
			}
			IL_485:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation14 = enumerator.Current;
					formation14.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseOnlyBlunt;
				}
				goto IL_80E;
			}
			IL_4C0:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation15 = enumerator.Current;
					formation15.SetControlledByAI(true, false);
				}
				goto IL_80E;
			}
			IL_4F8:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation16 = enumerator.Current;
					formation16.SetControlledByAI(false, false);
				}
				goto IL_80E;
			}
			IL_530:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation17 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation17);
					formation17.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				}
				goto IL_80E;
			}
			IL_571:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation18 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation18);
					formation18.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
				}
				goto IL_80E;
			}
			IL_5B2:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation19 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation19);
					formation19.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
				}
				goto IL_80E;
			}
			IL_5F3:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation20 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation20);
					formation20.ArrangementOrder = ArrangementOrder.ArrangementOrderCircle;
				}
				goto IL_80E;
			}
			IL_634:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation21 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation21);
					formation21.ArrangementOrder = ArrangementOrder.ArrangementOrderSquare;
				}
				goto IL_80E;
			}
			IL_675:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation22 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation22);
					formation22.ArrangementOrder = ArrangementOrder.ArrangementOrderSkein;
				}
				goto IL_80E;
			}
			IL_6B6:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation23 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation23);
					formation23.ArrangementOrder = ArrangementOrder.ArrangementOrderColumn;
				}
				goto IL_80E;
			}
			IL_6F7:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation24 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation24);
					formation24.ArrangementOrder = ArrangementOrder.ArrangementOrderScatter;
				}
				goto IL_80E;
			}
			IL_738:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation25 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation25);
					formation25.FormOrder = FormOrder.FormOrderDeep;
				}
				goto IL_80E;
			}
			IL_779:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation26 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation26);
					formation26.FormOrder = FormOrder.FormOrderWide;
				}
				goto IL_80E;
			}
			IL_7B7:
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation27 = enumerator.Current;
					OrderController.TryCancelStopOrder(formation27);
					formation27.FormOrder = FormOrder.FormOrderWider;
				}
				goto IL_80E;
			}
			IL_7F5:
			Debug.FailedAssert("[DEBUG]Invalid order type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "SetOrder", 686);
			IL_80E:
			this.AfterSetOrder(orderType);
			if (this.OnOrderIssued != null)
			{
				this.OnOrderIssued(orderType, this.SelectedFormations, Array.Empty<object>());
			}
		}

		private static void PlayOrderGestures(OrderType orderType, Agent agent, MBList<Formation> selectedFormations)
		{
			switch (orderType)
			{
			case OrderType.Move:
			case OrderType.MoveToLineSegment:
			case OrderType.MoveToLineSegmentWithHorizontalLayout:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Move, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.Charge:
			case OrderType.ChargeWithTarget:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Charge, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.StandYourGround:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Stop, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.FollowMe:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Follow, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.Retreat:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Retreat, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.AdvanceTenPaces:
			case OrderType.Advance:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Advance, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.FallBackTenPaces:
			case OrderType.FallBack:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FallBack, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.LookAtEnemy:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FaceEnemy, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.LookAtDirection:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FaceDirection, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.ArrangementLine:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FormLine, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.ArrangementCloseOrder:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FormShieldWall, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.ArrangementLoose:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FormLoose, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.ArrangementCircular:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FormCircle, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.ArrangementSchiltron:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FormSquare, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.ArrangementVee:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FormSkein, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.ArrangementColumn:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FormColumn, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.ArrangementScatter:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FormScatter, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.HoldFire:
				agent.MakeVoice(SkinVoiceManager.VoiceType.HoldFire, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.FireAtWill:
				agent.MakeVoice(SkinVoiceManager.VoiceType.FireAtWill, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.Mount:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Mount, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.Dismount:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Dismount, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.AIControlOn:
				agent.MakeVoice(SkinVoiceManager.VoiceType.CommandDelegate, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			case OrderType.AIControlOff:
				agent.MakeVoice(SkinVoiceManager.VoiceType.CommandUndelegate, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				break;
			}
			if (selectedFormations.Count > 0 && agent != null && agent.Controller != Agent.ControllerType.AI)
			{
				MissionWeapon wieldedWeapon = agent.WieldedWeapon;
				switch (wieldedWeapon.IsEmpty ? WeaponClass.Undefined : wieldedWeapon.Item.PrimaryWeapon.WeaponClass)
				{
				case WeaponClass.Undefined:
				case WeaponClass.Stone:
					if (agent.MountAgent == null)
					{
						agent.SetActionChannel(1, (orderType == OrderType.FollowMe) ? (agent.GetIsLeftStance() ? OrderController.act_command_follow_unarmed_leftstance : OrderController.act_command_follow_unarmed) : (agent.GetIsLeftStance() ? OrderController.act_command_unarmed_leftstance : OrderController.act_command_unarmed), false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						return;
					}
					agent.SetActionChannel(1, (orderType == OrderType.FollowMe) ? OrderController.act_horse_command_follow_unarmed : OrderController.act_horse_command_unarmed, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					return;
				case WeaponClass.Dagger:
				case WeaponClass.OneHandedSword:
				case WeaponClass.OneHandedAxe:
				case WeaponClass.Mace:
				case WeaponClass.Pick:
				case WeaponClass.OneHandedPolearm:
				case WeaponClass.ThrowingAxe:
				case WeaponClass.ThrowingKnife:
					if (agent.MountAgent == null)
					{
						agent.SetActionChannel(1, (orderType == OrderType.FollowMe) ? (agent.GetIsLeftStance() ? OrderController.act_command_follow_leftstance : OrderController.act_command_follow) : (agent.GetIsLeftStance() ? OrderController.act_command_leftstance : OrderController.act_command), false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						return;
					}
					agent.SetActionChannel(1, (orderType == OrderType.FollowMe) ? OrderController.act_horse_command_follow : OrderController.act_horse_command, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					return;
				case WeaponClass.TwoHandedSword:
				case WeaponClass.TwoHandedAxe:
				case WeaponClass.TwoHandedMace:
				case WeaponClass.TwoHandedPolearm:
				case WeaponClass.LowGripPolearm:
				case WeaponClass.Crossbow:
				case WeaponClass.Javelin:
				case WeaponClass.Pistol:
				case WeaponClass.Musket:
					if (agent.MountAgent == null)
					{
						agent.SetActionChannel(1, (orderType == OrderType.FollowMe) ? (agent.GetIsLeftStance() ? OrderController.act_command_follow_2h_leftstance : OrderController.act_command_follow_2h) : (agent.GetIsLeftStance() ? OrderController.act_command_2h_leftstance : OrderController.act_command_2h), false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						return;
					}
					agent.SetActionChannel(1, (orderType == OrderType.FollowMe) ? OrderController.act_horse_command_follow_2h : OrderController.act_horse_command_2h, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					return;
				case WeaponClass.Bow:
					if (agent.MountAgent == null)
					{
						agent.SetActionChannel(1, (orderType == OrderType.FollowMe) ? OrderController.act_command_follow_bow : OrderController.act_command_bow, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						return;
					}
					agent.SetActionChannel(1, (orderType == OrderType.FollowMe) ? OrderController.act_horse_command_follow_bow : OrderController.act_horse_command_bow, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					return;
				case WeaponClass.Boulder:
					return;
				}
				Debug.FailedAssert("Unexpected weapon class.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "PlayOrderGestures", 863);
			}
		}

		private static void PlayFormationSelectedGesture(Formation formation, Agent agent)
		{
			if (formation.SecondaryClasses.Any<FormationClass>())
			{
				agent.MakeVoice(SkinVoiceManager.VoiceType.MixedFormation, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				return;
			}
			switch (formation.PrimaryClass)
			{
			case FormationClass.Infantry:
			case FormationClass.HeavyInfantry:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Infantry, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				return;
			case FormationClass.Ranged:
			case FormationClass.NumberOfDefaultFormations:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Archers, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				return;
			case FormationClass.Cavalry:
			case FormationClass.LightCavalry:
			case FormationClass.HeavyCavalry:
				agent.MakeVoice(SkinVoiceManager.VoiceType.Cavalry, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				return;
			case FormationClass.HorseArcher:
				agent.MakeVoice(SkinVoiceManager.VoiceType.HorseArchers, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				return;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "PlayFormationSelectedGesture", 902);
				return;
			}
		}

		private unsafe void AfterSetOrder(OrderType orderType)
		{
			MBDebug.Print("After set order called, number of selected formations: " + this.SelectedFormations.Count, 0, Debug.DebugColor.White, 17592186044416UL);
			foreach (Formation formation in this.SelectedFormations)
			{
				MBDebug.Print(((formation != null) ? new FormationClass?(formation.FormationIndex) : null) + " formation being processed.", 0, Debug.DebugColor.White, 17592186044416UL);
				bool flag = false;
				if (formation.IsPlayerTroopInFormation)
				{
					flag = formation.GetReadonlyMovementOrderReference()->OrderEnum == MovementOrder.MovementOrderEnum.Follow;
				}
				formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					agent.UpdateCachedAndFormationValues(true, false);
				}, flag ? Mission.Current.MainAgent : null);
				MBDebug.Print("Update cached and formation values on each agent complete, number of selected formations: " + this.SelectedFormations.Count, 0, Debug.DebugColor.White, 17592186044416UL);
				this._mission.SetRandomDecideTimeOfAgentsWithIndices(formation.CollectUnitIndices(), null, null);
				MBDebug.Print("Set random decide time of agents with indices complete, number of selected formations: " + this.SelectedFormations.Count, 0, Debug.DebugColor.White, 17592186044416UL);
			}
			MBDebug.Print("After set order loop complete, number of selected formations: " + this.SelectedFormations.Count, 0, Debug.DebugColor.White, 17592186044416UL);
			if (this.Owner != null && this.AreGesturesEnabled())
			{
				OrderController.PlayOrderGestures(orderType, this.Owner, this._selectedFormations);
			}
		}

		private void BeforeSetOrder(OrderType orderType)
		{
			foreach (Formation formation in this.SelectedFormations.Where((Formation f) => !this.IsFormationSelectable(f, this.Owner)).ToList<Formation>())
			{
				this.DeselectFormation(formation);
			}
			if (!GameNetwork.IsClientOrReplay && orderType != OrderType.AIControlOff && orderType != OrderType.AIControlOn)
			{
				this.ReleaseFormationsFromAI();
			}
		}

		public void SetOrderWithAgent(OrderType orderType, Agent agent)
		{
			MBDebug.Print(string.Concat(new object[] { "SetOrderWithAgent ", orderType, " ", agent.Name, "on team" }), 0, Debug.DebugColor.White, 17592186044416UL);
			this.BeforeSetOrder(orderType);
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplyOrderWithAgent(orderType, agent));
				GameNetwork.EndModuleEventAsClient();
			}
			if (orderType != OrderType.FollowMe)
			{
				if (orderType != OrderType.GuardMe)
				{
					goto IL_E7;
				}
			}
			else
			{
				using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation = enumerator.Current;
						formation.SetMovementOrder(MovementOrder.MovementOrderFollow(agent));
					}
					goto IL_100;
				}
			}
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation2 = enumerator.Current;
					formation2.SetMovementOrder(MovementOrder.MovementOrderGuard(agent));
				}
				goto IL_100;
			}
			IL_E7:
			Debug.FailedAssert("[DEBUG]Invalid order type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "SetOrderWithAgent", 988);
			IL_100:
			this.AfterSetOrder(orderType);
			OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
			if (onOrderIssued == null)
			{
				return;
			}
			onOrderIssued(orderType, this.SelectedFormations, new object[] { agent });
		}

		public void SetOrderWithPosition(OrderType orderType, WorldPosition orderPosition)
		{
			MBDebug.Print(string.Concat(new object[] { "SetOrderWithPosition ", orderType, " ", orderPosition, "on team" }), 0, Debug.DebugColor.White, 17592186044416UL);
			this.BeforeSetOrder(orderType);
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplyOrderWithPosition(orderType, orderPosition.GetGroundVec3()));
				GameNetwork.EndModuleEventAsClient();
			}
			if (orderType != OrderType.Move)
			{
				if (orderType != OrderType.LookAtDirection)
				{
					if (orderType != OrderType.FormCustom)
					{
						goto IL_15A;
					}
					goto IL_10E;
				}
			}
			else
			{
				using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation = enumerator.Current;
						formation.SetMovementOrder(MovementOrder.MovementOrderMove(orderPosition));
					}
					goto IL_173;
				}
			}
			FacingOrder facingOrder = FacingOrder.FacingOrderLookAtDirection(OrderController.GetOrderLookAtDirection(this.SelectedFormations, orderPosition.AsVec2));
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation2 = enumerator.Current;
					formation2.FacingOrder = facingOrder;
				}
				goto IL_173;
			}
			IL_10E:
			float orderFormCustomWidth = OrderController.GetOrderFormCustomWidth(this.SelectedFormations, orderPosition.GetGroundVec3());
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation3 = enumerator.Current;
					formation3.FormOrder = FormOrder.FormOrderCustom(orderFormCustomWidth);
				}
				goto IL_173;
			}
			IL_15A:
			Debug.FailedAssert("[DEBUG]Invalid order type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "SetOrderWithPosition", 1038);
			IL_173:
			this.AfterSetOrder(orderType);
			if (this.OnOrderIssued != null)
			{
				this.OnOrderIssued(orderType, this.SelectedFormations, new object[] { orderPosition });
			}
		}

		public void SetOrderWithFormation(OrderType orderType, Formation orderFormation)
		{
			MBDebug.Print(string.Concat(new object[] { "SetOrderWithFormation ", orderType, " ", orderFormation, "on team" }), 0, Debug.DebugColor.White, 17592186044416UL);
			this.BeforeSetOrder(orderType);
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplyOrderWithFormation(orderType, orderFormation.Index));
				GameNetwork.EndModuleEventAsClient();
			}
			if (orderType == OrderType.ChargeWithTarget)
			{
				using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation = enumerator.Current;
						formation.SetMovementOrder(MovementOrder.MovementOrderChargeToTarget(orderFormation));
					}
					goto IL_C0;
				}
			}
			Debug.FailedAssert("Invalid order type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "SetOrderWithFormation", 1073);
			IL_C0:
			this.AfterSetOrder(orderType);
			if (this.OnOrderIssued != null)
			{
				this.OnOrderIssued(orderType, this.SelectedFormations, new object[] { orderFormation });
			}
		}

		public void SetOrderWithFormationAndPercentage(OrderType orderType, Formation orderFormation, float percentage)
		{
			int num = (int)(percentage * 100f);
			num = MBMath.ClampInt(num, 0, 100);
			this.BeforeSetOrder(orderType);
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplyOrderWithFormationAndPercentage(orderType, orderFormation.Index, num));
				GameNetwork.EndModuleEventAsClient();
			}
			Debug.FailedAssert("Invalid order type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "SetOrderWithFormationAndPercentage", 1114);
			this.AfterSetOrder(orderType);
			if (this.OnOrderIssued != null)
			{
				this.OnOrderIssued(orderType, this.SelectedFormations, new object[] { orderFormation, percentage });
			}
		}

		public void TransferUnitWithPriorityFunction(Formation orderFormation, int number, bool hasShield, bool hasSpear, bool hasThrown, bool isHeavy, bool isRanged, bool isMounted, bool excludeBannerman, List<Agent> excludedAgents)
		{
			OrderController.<>c__DisplayClass74_0 CS$<>8__locals1 = new OrderController.<>c__DisplayClass74_0();
			CS$<>8__locals1.hasShield = hasShield;
			CS$<>8__locals1.hasSpear = hasSpear;
			CS$<>8__locals1.hasThrown = hasThrown;
			CS$<>8__locals1.isHeavy = isHeavy;
			CS$<>8__locals1.isRanged = isRanged;
			CS$<>8__locals1.isMounted = isMounted;
			this.BeforeSetOrder(OrderType.Transfer);
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplyOrderWithFormationAndNumber(OrderType.Transfer, orderFormation.Index, number));
				GameNetwork.EndModuleEventAsClient();
			}
			List<int> list = null;
			int num = this.SelectedFormations.Sum((Formation f) => f.CountOfUnits);
			int num2 = number;
			int num3 = 0;
			if (this.SelectedFormations.Count > 1)
			{
				list = new List<int>();
			}
			foreach (Formation formation in this.SelectedFormations)
			{
				int countOfUnits = formation.CountOfUnits;
				int num4 = num2 * countOfUnits / num;
				if (!GameNetwork.IsClientOrReplay)
				{
					formation.OnMassUnitTransferStart();
					orderFormation.OnMassUnitTransferStart();
					formation.TransferUnitsWithPriorityFunction(orderFormation, num4, new Func<Agent, int>(CS$<>8__locals1.<TransferUnitWithPriorityFunction>g__priorityFunction|0), excludeBannerman, excludedAgents);
					formation.OnMassUnitTransferEnd();
					orderFormation.OnMassUnitTransferEnd();
				}
				if (list != null)
				{
					list.Add(num4);
				}
				num2 -= num4;
				num -= countOfUnits;
				num3 += num4;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				orderFormation.QuerySystem.Expire();
			}
			this.AfterSetOrder(OrderType.Transfer);
			if (this.OnOrderIssued != null)
			{
				if (list != null)
				{
					object[] array = new object[list.Count + 1];
					array[0] = number;
					for (int i = 0; i < list.Count; i++)
					{
						array[i + 1] = list[i];
					}
					this.OnOrderIssued(OrderType.Transfer, this.SelectedFormations, new object[] { orderFormation, array });
					return;
				}
				this.OnOrderIssued(OrderType.Transfer, this.SelectedFormations, new object[] { orderFormation, number });
			}
		}

		public void RearrangeFormationsAccordingToFilters(Team team, List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> MassTransferData)
		{
			team.RearrangeFormationsAccordingToFilters(MassTransferData);
		}

		public void SetOrderWithFormationAndNumber(OrderType orderType, Formation orderFormation, int number)
		{
			this.BeforeSetOrder(orderType);
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplyOrderWithFormationAndNumber(orderType, orderFormation.Index, number));
				GameNetwork.EndModuleEventAsClient();
			}
			List<int> list = null;
			if (orderType == OrderType.Transfer)
			{
				int num = this.SelectedFormations.Sum((Formation f) => f.CountOfUnits);
				int num2 = number;
				int num3 = 0;
				if (this.SelectedFormations.Count > 1)
				{
					list = new List<int>();
				}
				foreach (Formation formation in this.SelectedFormations)
				{
					int countOfUnits = formation.CountOfUnits;
					int num4 = num2 * countOfUnits / num;
					if (!GameNetwork.IsClientOrReplay)
					{
						formation.OnMassUnitTransferStart();
						orderFormation.OnMassUnitTransferStart();
						formation.TransferUnitsAux(orderFormation, num4, true, num4 < countOfUnits && orderFormation.CountOfUnits > 0 && orderFormation.OrderPositionIsValid);
						formation.OnMassUnitTransferEnd();
						orderFormation.OnMassUnitTransferEnd();
					}
					if (list != null)
					{
						list.Add(num4);
					}
					num2 -= num4;
					num -= countOfUnits;
					num3 += num4;
				}
				if (!GameNetwork.IsClientOrReplay)
				{
					orderFormation.QuerySystem.Expire();
				}
			}
			else
			{
				Debug.FailedAssert("[DEBUG]Invalid order type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "SetOrderWithFormationAndNumber", 1363);
			}
			this.AfterSetOrder(orderType);
			if (this.OnOrderIssued != null)
			{
				if (list != null)
				{
					object[] array = new object[list.Count + 1];
					array[0] = number;
					for (int i = 0; i < list.Count; i++)
					{
						array[i + 1] = list[i];
					}
					this.OnOrderIssued(orderType, this.SelectedFormations, new object[] { orderFormation, array });
					return;
				}
				this.OnOrderIssued(orderType, this.SelectedFormations, new object[] { orderFormation, number });
			}
		}

		public void SetOrderWithTwoPositions(OrderType orderType, WorldPosition position1, WorldPosition position2)
		{
			MBDebug.Print(string.Concat(new object[] { "SetOrderWithTwoPositions ", orderType, " ", position1, " ", position2, "on team" }), 0, Debug.DebugColor.White, 17592186044416UL);
			this.BeforeSetOrder(orderType);
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplyOrderWithTwoPositions(orderType, position1.GetGroundVec3(), position2.GetGroundVec3()));
				GameNetwork.EndModuleEventAsClient();
			}
			if (orderType - OrderType.MoveToLineSegment <= 1)
			{
				bool flag = orderType == OrderType.MoveToLineSegment;
				IEnumerable<Formation> enumerable = this.SelectedFormations.Where((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0);
				if (enumerable.Any<Formation>())
				{
					OrderController.MoveToLineSegment(enumerable, this.simulationFormations, position1, position2, this.OnOrderIssued, this.actualWidths, this.actualUnitSpacings, flag);
				}
			}
			else
			{
				Debug.FailedAssert("Invalid order type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "SetOrderWithTwoPositions", 1419);
			}
			this.AfterSetOrder(orderType);
			if (this.OnOrderIssued != null)
			{
				this.OnOrderIssued(orderType, this.SelectedFormations, new object[] { position1, position2 });
			}
		}

		public void SetOrderWithOrderableObject(IOrderable target)
		{
			BattleSideEnum side = this.SelectedFormations[0].Team.Side;
			OrderType order = target.GetOrder(side);
			this.BeforeSetOrder(order);
			MissionObject missionObject = target as MissionObject;
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ApplyOrderWithMissionObject(missionObject));
				GameNetwork.EndModuleEventAsClient();
			}
			switch (order)
			{
			case OrderType.Move:
				break;
			case OrderType.MoveToLineSegment:
				goto IL_19A;
			case OrderType.MoveToLineSegmentWithHorizontalLayout:
			{
				IPointDefendable pointDefendable = target as IPointDefendable;
				Vec3 globalPosition = pointDefendable.DefencePoints.Last<DefencePoint>().GameEntity.GlobalPosition;
				Vec3 globalPosition2 = pointDefendable.DefencePoints.First<DefencePoint>().GameEntity.GlobalPosition;
				IEnumerable<Formation> enumerable = this.SelectedFormations.Where((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0);
				if (enumerable.Any<Formation>())
				{
					WorldPosition worldPosition = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition, false);
					WorldPosition worldPosition2 = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition2, false);
					OrderController.MoveToLineSegment(enumerable, this.simulationFormations, worldPosition, worldPosition2, this.OnOrderIssued, this.actualWidths, this.actualUnitSpacings, false);
					goto IL_36F;
				}
				goto IL_36F;
			}
			default:
			{
				if (order == OrderType.FollowEntity)
				{
					GameEntity waitEntity = (target as UsableMachine).WaitEntity;
					foreach (Formation formation in this.SelectedFormations)
					{
						formation.SetMovementOrder(MovementOrder.MovementOrderFollowEntity(waitEntity));
					}
					goto IL_36F;
				}
				switch (order)
				{
				case OrderType.Use:
				{
					UsableMachine usableMachine = target as UsableMachine;
					this.ToggleSideOrderUse(this.SelectedFormations, usableMachine);
					goto IL_36F;
				}
				case OrderType.AttackEntity:
				{
					GameEntity gameEntity = missionObject.GameEntity;
					using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Formation formation2 = enumerator.Current;
							formation2.SetMovementOrder(MovementOrder.MovementOrderAttackEntity(gameEntity, !(missionObject is CastleGate)));
						}
						goto IL_36F;
					}
					break;
				}
				case OrderType.PointDefence:
					break;
				default:
					goto IL_36F;
				}
				IPointDefendable pointDefendable2 = target as IPointDefendable;
				using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation3 = enumerator.Current;
						formation3.SetMovementOrder(MovementOrder.MovementOrderMove(pointDefendable2.MiddleFrame.Origin));
					}
					goto IL_36F;
				}
				break;
			}
			}
			WorldPosition worldPosition3 = new WorldPosition(this._mission.Scene, UIntPtr.Zero, missionObject.GameEntity.GlobalPosition, false);
			using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation4 = enumerator.Current;
					formation4.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition3));
				}
				goto IL_36F;
			}
			IL_19A:
			IPointDefendable pointDefendable3 = target as IPointDefendable;
			Vec3 globalPosition3 = pointDefendable3.DefencePoints.Last<DefencePoint>().GameEntity.GlobalPosition;
			Vec3 globalPosition4 = pointDefendable3.DefencePoints.First<DefencePoint>().GameEntity.GlobalPosition;
			IEnumerable<Formation> enumerable2 = this.SelectedFormations.Where((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0);
			if (enumerable2.Any<Formation>())
			{
				WorldPosition worldPosition4 = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition3, false);
				WorldPosition worldPosition5 = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition4, false);
				OrderController.MoveToLineSegment(enumerable2, this.simulationFormations, worldPosition4, worldPosition5, this.OnOrderIssued, this.actualWidths, this.actualUnitSpacings, true);
			}
			IL_36F:
			this.AfterSetOrder(order);
			OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
			if (onOrderIssued == null)
			{
				return;
			}
			onOrderIssued(order, this.SelectedFormations, new object[] { target });
		}

		public unsafe static OrderType GetActiveMovementOrderOf(Formation formation)
		{
			MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
			switch (movementOrder.MovementState)
			{
			case MovementOrder.MovementStateEnum.Charge:
				movementOrder = *formation.GetReadonlyMovementOrderReference();
				if (movementOrder.OrderType == OrderType.GuardMe)
				{
					return OrderType.GuardMe;
				}
				return OrderType.Charge;
			case MovementOrder.MovementStateEnum.Hold:
			{
				movementOrder = *formation.GetReadonlyMovementOrderReference();
				OrderType orderType = movementOrder.OrderType;
				if (orderType <= OrderType.FollowMe)
				{
					if (orderType == OrderType.ChargeWithTarget)
					{
						return OrderType.Charge;
					}
					if (orderType == OrderType.FollowMe)
					{
						return OrderType.FollowMe;
					}
				}
				else
				{
					if (orderType == OrderType.Advance)
					{
						return OrderType.Advance;
					}
					if (orderType == OrderType.FallBack)
					{
						return OrderType.FallBack;
					}
				}
				return OrderType.Move;
			}
			case MovementOrder.MovementStateEnum.Retreat:
				return OrderType.Retreat;
			case MovementOrder.MovementStateEnum.StandGround:
				return OrderType.StandYourGround;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "GetActiveMovementOrderOf", 1580);
				return OrderType.Move;
			}
		}

		public static OrderType GetActiveFacingOrderOf(Formation formation)
		{
			if (formation.FacingOrder.OrderType == OrderType.LookAtDirection)
			{
				return OrderType.LookAtDirection;
			}
			return OrderType.LookAtEnemy;
		}

		public static OrderType GetActiveRidingOrderOf(Formation formation)
		{
			OrderType orderType = formation.RidingOrder.OrderType;
			if (orderType == OrderType.RideFree)
			{
				return OrderType.Mount;
			}
			return orderType;
		}

		public static OrderType GetActiveArrangementOrderOf(Formation formation)
		{
			return formation.ArrangementOrder.OrderType;
		}

		public static OrderType GetActiveFormOrderOf(Formation formation)
		{
			return formation.FormOrder.OrderType;
		}

		public static OrderType GetActiveWeaponUsageOrderOf(Formation formation)
		{
			return formation.WeaponUsageOrder.OrderType;
		}

		public static OrderType GetActiveFiringOrderOf(Formation formation)
		{
			return formation.FiringOrder.OrderType;
		}

		public static OrderType GetActiveAIControlOrderOf(Formation formation)
		{
			if (formation.IsAIControlled)
			{
				return OrderType.AIControlOn;
			}
			return OrderType.AIControlOff;
		}

		public void SimulateNewOrderWithPositionAndDirection(WorldPosition formationLineBegin, WorldPosition formationLineEnd, out List<WorldPosition> simulationAgentFrames, bool isFormationLayoutVertical)
		{
			IEnumerable<Formation> enumerable = this.SelectedFormations.Where((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0);
			if (enumerable.Any<Formation>())
			{
				OrderController.SimulateNewOrderWithPositionAndDirection(enumerable, this.simulationFormations, formationLineBegin, formationLineEnd, out simulationAgentFrames, isFormationLayoutVertical);
				return;
			}
			simulationAgentFrames = new List<WorldPosition>();
		}

		public void SimulateNewFacingOrder(Vec2 direction, out List<WorldPosition> simulationAgentFrames)
		{
			IEnumerable<Formation> enumerable = this.SelectedFormations.Where((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0);
			if (enumerable.Any<Formation>())
			{
				OrderController.SimulateNewFacingOrder(enumerable, this.simulationFormations, direction, out simulationAgentFrames);
				return;
			}
			simulationAgentFrames = new List<WorldPosition>();
		}

		public void SimulateNewCustomWidthOrder(float width, out List<WorldPosition> simulationAgentFrames)
		{
			IEnumerable<Formation> enumerable = this.SelectedFormations.Where((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0);
			if (enumerable.Any<Formation>())
			{
				OrderController.SimulateNewCustomWidthOrder(enumerable, this.simulationFormations, width, out simulationAgentFrames);
				return;
			}
			simulationAgentFrames = new List<WorldPosition>();
		}

		private static void SimulateNewOrderWithPositionAndDirectionAux(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition formationLineBegin, WorldPosition formationLineEnd, bool isSimulatingAgentFrames, out List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, out List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, out bool isLineShort, bool isFormationLayoutVertical = true)
		{
			float length = (formationLineEnd.AsVec2 - formationLineBegin.AsVec2).Length;
			isLineShort = false;
			if (length < ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius))
			{
				isLineShort = true;
			}
			else
			{
				float num;
				if (isFormationLayoutVertical)
				{
					num = formations.Sum((Formation f) => f.MinimumWidth) + (float)(formations.Count<Formation>() - 1) * 1.5f;
				}
				else
				{
					num = formations.Max((Formation f) => f.Width);
				}
				if (length < num)
				{
					isLineShort = true;
				}
			}
			if (isLineShort)
			{
				float num2;
				if (isFormationLayoutVertical)
				{
					num2 = formations.Sum((Formation f) => f.Width);
					num2 += (float)(formations.Count<Formation>() - 1) * 1.5f;
				}
				else
				{
					num2 = formations.Max((Formation f) => f.Width);
				}
				Vec2 direction = formations.MaxBy((Formation f) => f.CountOfUnitsWithoutDetachedOnes).Direction;
				direction.RotateCCW(-1.5707964f);
				direction.Normalize();
				formationLineEnd = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 + num2 / 2f * direction, formationLineBegin, 1f, true);
				formationLineBegin = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 - num2 / 2f * direction, formationLineBegin, 1f, true);
			}
			else
			{
				formationLineEnd = Mission.Current.GetStraightPathToTarget(formationLineEnd.AsVec2, formationLineBegin, 1f, true);
			}
			if (isFormationLayoutVertical)
			{
				OrderController.SimulateNewOrderWithVerticalLayout(formations, simulationFormations, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, out simulationAgentFrames, isSimulatingFormationChanges, out simulationFormationChanges);
				return;
			}
			OrderController.SimulateNewOrderWithHorizontalLayout(formations, simulationFormations, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, out simulationAgentFrames, isSimulatingFormationChanges, out simulationFormationChanges);
		}

		private static Formation GetSimulationFormation(Formation formation, Dictionary<Formation, Formation> simulationFormations)
		{
			if (simulationFormations == null)
			{
				return null;
			}
			return simulationFormations[formation];
		}

		private static void SimulateNewFacingOrder(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, Vec2 direction, out List<WorldPosition> simulationAgentFrames)
		{
			simulationAgentFrames = new List<WorldPosition>();
			foreach (Formation formation in formations)
			{
				float width = formation.Width;
				WorldPosition worldPosition = formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
				int num = 0;
				OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), worldPosition, direction, ref width, ref num);
				float num2;
				OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, null, worldPosition, direction, width, num, false, out num2);
			}
		}

		private static void SimulateNewCustomWidthOrder(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, float width, out List<WorldPosition> simulationAgentFrames)
		{
			simulationAgentFrames = new List<WorldPosition>();
			foreach (Formation formation in formations)
			{
				float num = width;
				num = MathF.Min(num, formation.MaximumWidth);
				Mat3 identity = Mat3.Identity;
				Vec2 vec = formation.Direction;
				identity.f = vec.ToVec3(0f);
				identity.Orthonormalize();
				WorldPosition worldPosition = formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
				int num2 = 0;
				Formation formation2 = formation;
				Formation simulationFormation = OrderController.GetSimulationFormation(formation, simulationFormations);
				vec = formation.Direction;
				OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation2, simulationFormation, worldPosition, vec, ref num, ref num2);
				int count = simulationAgentFrames.Count;
				Formation formation3 = formation;
				Formation simulationFormation2 = OrderController.GetSimulationFormation(formation, simulationFormations);
				List<WorldPosition> list = simulationAgentFrames;
				List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list2 = null;
				vec = formation.Direction;
				float num3;
				OrderController.SimulateNewOrderWithFrameAndWidth(formation3, simulationFormation2, list, list2, worldPosition, vec, num, num2, false, out num3);
				float lastSimulatedFormationsOccupationWidthIfLesserThanActualWidth = Formation.GetLastSimulatedFormationsOccupationWidthIfLesserThanActualWidth(OrderController.GetSimulationFormation(formation, simulationFormations));
				if (lastSimulatedFormationsOccupationWidthIfLesserThanActualWidth > 0f)
				{
					simulationAgentFrames.RemoveRange(count, simulationAgentFrames.Count - count);
					Formation formation4 = formation;
					Formation simulationFormation3 = OrderController.GetSimulationFormation(formation, simulationFormations);
					List<WorldPosition> list3 = simulationAgentFrames;
					List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list4 = null;
					vec = formation.Direction;
					OrderController.SimulateNewOrderWithFrameAndWidth(formation4, simulationFormation3, list3, list4, worldPosition, vec, lastSimulatedFormationsOccupationWidthIfLesserThanActualWidth, num2, false, out num3);
				}
			}
		}

		public static void SimulateNewOrderWithPositionAndDirection(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition formationLineBegin, WorldPosition formationLineEnd, out List<WorldPosition> simulationAgentFrames, bool isFormationLayoutVertical = true)
		{
			List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list;
			bool flag;
			OrderController.SimulateNewOrderWithPositionAndDirectionAux(formations, simulationFormations, formationLineBegin, formationLineEnd, true, out simulationAgentFrames, false, out list, out flag, isFormationLayoutVertical);
		}

		public static void SimulateNewOrderWithPositionAndDirection(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition formationLineBegin, WorldPosition formationLineEnd, out List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> formationChanges, out bool isLineShort, bool isFormationLayoutVertical = true)
		{
			List<WorldPosition> list;
			OrderController.SimulateNewOrderWithPositionAndDirectionAux(formations, simulationFormations, formationLineBegin, formationLineEnd, false, out list, true, out formationChanges, out isLineShort, isFormationLayoutVertical);
		}

		private static void SimulateNewOrderWithVerticalLayout(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition formationLineBegin, WorldPosition formationLineEnd, bool isSimulatingAgentFrames, out List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, out List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : new List<WorldPosition>());
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : new List<ValueTuple<Formation, int, float, WorldPosition, Vec2>>());
			Vec2 vec = formationLineEnd.AsVec2 - formationLineBegin.AsVec2;
			float length = vec.Length;
			vec.Normalize();
			float num = MathF.Max(0f, length - (float)(formations.Count<Formation>() - 1) * 1.5f);
			float num2 = formations.Sum((Formation f) => f.Width);
			bool flag = num.ApproximatelyEqualsTo(num2, 0.1f);
			float num3 = formations.Sum((Formation f) => f.MinimumWidth);
			float num4 = num3 + (float)(formations.Count<Formation>() - 1) * 1.5f;
			if (length < num4)
			{
				num = num3;
			}
			Vec2 vec2 = new Vec2(-vec.y, vec.x).Normalized();
			float num5 = 0f;
			foreach (Formation formation in formations)
			{
				float minimumWidth = formation.MinimumWidth;
				float num6 = (flag ? formation.Width : MathF.Min((num < num2) ? formation.Width : float.MaxValue, num * (minimumWidth / num3)));
				num6 = MathF.Min(num6, formation.MaximumWidth);
				WorldPosition worldPosition = formationLineBegin;
				worldPosition.SetVec2(worldPosition.AsVec2 + vec * (num6 * 0.5f + num5));
				int num7 = 0;
				OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), worldPosition, vec2, ref num6, ref num7);
				float num8;
				OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, worldPosition, vec2, num6, num7, false, out num8);
				num5 += num6 + 1.5f;
			}
		}

		private static void DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(Formation formation, Formation simulationFormation, in WorldPosition formationPosition, in Vec2 formationDirection, ref float formationWidth, ref int unitSpacingReduction)
		{
			if (simulationFormation.UnitSpacing != formation.UnitSpacing)
			{
				simulationFormation = new Formation(null, -1);
			}
			int num = formation.CountOfUnitsWithoutDetachedOnes - 1;
			float num2 = formationWidth;
			do
			{
				WorldPosition? worldPosition;
				Vec2? vec;
				formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, num, formationPosition, formationDirection, formationWidth, formation.UnitSpacing - unitSpacingReduction, out worldPosition, out vec, out num2);
				if (worldPosition != null)
				{
					break;
				}
				unitSpacingReduction++;
			}
			while (formation.UnitSpacing - unitSpacingReduction >= 0);
			unitSpacingReduction = MathF.Min(unitSpacingReduction, formation.UnitSpacing);
			if (unitSpacingReduction > 0)
			{
				formationWidth = num2;
			}
		}

		private static float GetGapBetweenLinesOfFormation(Formation f, float unitSpacing)
		{
			float num = 0f;
			float num2 = 0.2f;
			if (f.HasAnyMountedUnit && !(f.RidingOrder == RidingOrder.RidingOrderDismount))
			{
				num = 2f;
				num2 = 0.6f;
			}
			return num + unitSpacing * num2;
		}

		private static void SimulateNewOrderWithHorizontalLayout(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition formationLineBegin, WorldPosition formationLineEnd, bool isSimulatingAgentFrames, out List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, out List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : new List<WorldPosition>());
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : new List<ValueTuple<Formation, int, float, WorldPosition, Vec2>>());
			Vec2 vec = formationLineEnd.AsVec2 - formationLineBegin.AsVec2;
			float num = vec.Normalize();
			float num2 = formations.Max((Formation f) => f.MinimumWidth);
			if (num < num2)
			{
				num = num2;
			}
			Vec2 vec2 = new Vec2(-vec.y, vec.x).Normalized();
			float num3 = 0f;
			foreach (Formation formation in formations)
			{
				float num4 = num;
				num4 = MathF.Min(num4, formation.MaximumWidth);
				WorldPosition worldPosition = formationLineBegin;
				worldPosition.SetVec2((formationLineEnd.AsVec2 + formationLineBegin.AsVec2) * 0.5f - vec2 * num3);
				int num5 = 0;
				OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), worldPosition, vec2, ref num4, ref num5);
				float num6;
				OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, worldPosition, vec2, num4, num5, true, out num6);
				num3 += num6 + OrderController.GetGapBetweenLinesOfFormation(formation, (float)(formation.UnitSpacing - num5));
			}
		}

		private static void SimulateNewOrderWithFrameAndWidth(Formation formation, Formation simulationFormation, List<WorldPosition> simulationAgentFrames, List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, in WorldPosition formationPosition, in Vec2 formationDirection, float formationWidth, int unitSpacingReduction, bool simulateFormationDepth, out float simulatedFormationDepth)
		{
			int num = 0;
			float num2 = (simulateFormationDepth ? 0f : float.NaN);
			bool flag = Mission.Current.Mode != MissionMode.Deployment || Mission.Current.IsOrderPositionAvailable(formationPosition, formation.Team);
			foreach (Agent agent in from u in formation.GetUnitsWithoutDetachedOnes()
				orderby MBCommon.Hash(u.Index, u)
				select u)
			{
				WorldPosition? worldPosition = null;
				Vec2? vec = null;
				if (flag)
				{
					formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, num, formationPosition, formationDirection, formationWidth, formation.UnitSpacing - unitSpacingReduction, out worldPosition, out vec);
				}
				else
				{
					worldPosition = new WorldPosition?(agent.GetWorldPosition());
					vec = new Vec2?(agent.GetMovementDirection());
				}
				if (worldPosition != null)
				{
					if (simulationAgentFrames != null)
					{
						simulationAgentFrames.Add(worldPosition.Value);
					}
					if (simulateFormationDepth)
					{
						WorldPosition worldPosition2 = formationPosition;
						Vec2 asVec = worldPosition2.AsVec2;
						worldPosition2 = formationPosition;
						Vec2 asVec2 = worldPosition2.AsVec2;
						Vec2 vec2 = formationDirection;
						float num3 = Vec2.DistanceToLine(asVec, asVec2 + vec2.RightVec(), worldPosition.Value.AsVec2);
						if (num3 > num2)
						{
							num2 = num3;
						}
					}
				}
				num++;
			}
			if (flag)
			{
				if (simulationFormationChanges != null)
				{
					simulationFormationChanges.Add(ValueTuple.Create<Formation, int, float, WorldPosition, Vec2>(formation, unitSpacingReduction, formationWidth, formationPosition, formationDirection));
				}
			}
			else
			{
				WorldPosition worldPosition3 = formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
				if (simulationFormationChanges != null)
				{
					simulationFormationChanges.Add(ValueTuple.Create<Formation, int, float, WorldPosition, Vec2>(formation, unitSpacingReduction, formationWidth, worldPosition3, formation.Direction));
				}
			}
			simulatedFormationDepth = num2 + formation.UnitDiameter;
		}

		public void SimulateDestinationFrames(out List<WorldPosition> simulationAgentFrames, float minDistance = 3f)
		{
			List<Formation> selectedFormations = this.SelectedFormations;
			simulationAgentFrames = new List<WorldPosition>(100);
			float minDistanceSq = minDistance * minDistance;
			Action<Agent, List<WorldPosition>> <>9__0;
			foreach (Formation formation in selectedFormations)
			{
				Action<Agent, List<WorldPosition>> action;
				if ((action = <>9__0) == null)
				{
					action = (<>9__0 = delegate(Agent agent, List<WorldPosition> localSimulationAgentFrames)
					{
						WorldPosition worldPosition;
						if (this._mission.IsTeleportingAgents && !agent.CanTeleport())
						{
							worldPosition = agent.GetWorldPosition();
						}
						else
						{
							worldPosition = agent.Formation.GetOrderPositionOfUnit(agent);
						}
						if (!GameNetwork.IsMultiplayer && this._mission.Mode == MissionMode.Deployment)
						{
							MBSceneUtilities.ProjectPositionToDeploymentBoundaries(agent.Formation.Team.Side, ref worldPosition);
						}
						if (worldPosition.IsValid && agent.Position.AsVec2.DistanceSquared(worldPosition.AsVec2) >= minDistanceSq)
						{
							localSimulationAgentFrames.Add(worldPosition);
						}
					});
				}
				formation.ApplyActionOnEachUnit(action, simulationAgentFrames);
			}
		}

		private void ToggleSideOrderUse(IEnumerable<Formation> formations, UsableMachine usable)
		{
			IEnumerable<Formation> enumerable = formations.Where(new Func<Formation, bool>(usable.IsUsedByFormation));
			if (enumerable.IsEmpty<Formation>())
			{
				foreach (Formation formation in formations)
				{
					formation.StartUsingMachine(usable, true);
				}
				if (!usable.HasWaitFrame)
				{
					return;
				}
				using (IEnumerator<Formation> enumerator = formations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation2 = enumerator.Current;
						formation2.SetMovementOrder(MovementOrder.MovementOrderFollowEntity(usable.WaitEntity));
					}
					return;
				}
			}
			foreach (Formation formation3 in enumerable)
			{
				formation3.StopUsingMachine(usable, true);
			}
		}

		private static int GetLineOrderByClass(FormationClass formationClass)
		{
			return Array.IndexOf<FormationClass>(new FormationClass[]
			{
				FormationClass.HeavyInfantry,
				FormationClass.Infantry,
				FormationClass.HeavyCavalry,
				FormationClass.Cavalry,
				FormationClass.LightCavalry,
				FormationClass.NumberOfDefaultFormations,
				FormationClass.Ranged,
				FormationClass.HorseArcher
			}, formationClass);
		}

		public static IEnumerable<Formation> SortFormationsForHorizontalLayout(IEnumerable<Formation> formations)
		{
			return formations.OrderBy((Formation f) => OrderController.GetLineOrderByClass(f.FormationIndex));
		}

		private static IEnumerable<Formation> GetSortedFormations(IEnumerable<Formation> formations, bool isFormationLayoutVertical)
		{
			if (isFormationLayoutVertical)
			{
				return formations;
			}
			return OrderController.SortFormationsForHorizontalLayout(formations);
		}

		private static void MoveToLineSegment(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition TargetLineSegmentBegin, WorldPosition TargetLineSegmentEnd, OnOrderIssuedDelegate OnOrderIssued, Dictionary<Formation, float> actualWidths, Dictionary<Formation, int> actualUnitSpacings, bool isFormationLayoutVertical = true)
		{
			foreach (Formation formation in formations)
			{
				int num;
				if (actualUnitSpacings.TryGetValue(formation, out num))
				{
					formation.SetPositioning(null, null, new int?(num));
				}
				float num2;
				if (actualWidths.TryGetValue(formation, out num2))
				{
					formation.FormOrder = FormOrder.FormOrderCustom(num2);
				}
			}
			formations = OrderController.GetSortedFormations(formations, isFormationLayoutVertical);
			List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list;
			bool flag;
			OrderController.SimulateNewOrderWithPositionAndDirection(formations, simulationFormations, TargetLineSegmentBegin, TargetLineSegmentEnd, out list, out flag, isFormationLayoutVertical);
			if (!formations.Any<Formation>())
			{
				return;
			}
			foreach (ValueTuple<Formation, int, float, WorldPosition, Vec2> valueTuple in list)
			{
				Formation item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				float item3 = valueTuple.Item3;
				WorldPosition item4 = valueTuple.Item4;
				Vec2 item5 = valueTuple.Item5;
				int unitSpacing = item.UnitSpacing;
				float width = item.Width;
				if (item2 > 0)
				{
					int num3 = MathF.Max(item.UnitSpacing - item2, 0);
					item.SetPositioning(null, null, new int?(num3));
					if (item.UnitSpacing != unitSpacing)
					{
						actualUnitSpacings[item] = unitSpacing;
					}
				}
				if (item.Width != item3 && item.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Column)
				{
					item.FormOrder = FormOrder.FormOrderCustom(item3);
					if (flag)
					{
						actualWidths[item] = width;
					}
				}
				if (!flag)
				{
					item.SetMovementOrder(MovementOrder.MovementOrderMove(item4));
					item.FacingOrder = FacingOrder.FacingOrderLookAtDirection(item5);
					item.FormOrder = FormOrder.FormOrderCustom(item3);
					if (OnOrderIssued != null)
					{
						MBList<Formation> mblist = new MBList<Formation> { item };
						OnOrderIssued(OrderType.Move, mblist, new object[] { item4 });
						OnOrderIssued(OrderType.LookAtDirection, mblist, new object[] { item5 });
						OnOrderIssued(OrderType.FormCustom, mblist, new object[] { item3 });
					}
				}
				else
				{
					Formation formation2 = formations.MaxBy((Formation f) => f.CountOfUnitsWithoutDetachedOnes);
					OrderType activeFacingOrderOf = OrderController.GetActiveFacingOrderOf(formation2);
					if (activeFacingOrderOf == OrderType.LookAtEnemy)
					{
						item.SetMovementOrder(MovementOrder.MovementOrderMove(item4));
						if (OnOrderIssued != null)
						{
							MBList<Formation> mblist2 = new MBList<Formation> { item };
							OnOrderIssued(OrderType.Move, mblist2, new object[] { item4 });
							OnOrderIssued(OrderType.LookAtEnemy, mblist2, Array.Empty<object>());
						}
					}
					else if (activeFacingOrderOf == OrderType.LookAtDirection)
					{
						item.SetMovementOrder(MovementOrder.MovementOrderMove(item4));
						item.FacingOrder = FacingOrder.FacingOrderLookAtDirection(formation2.Direction);
						if (OnOrderIssued != null)
						{
							MBList<Formation> mblist3 = new MBList<Formation> { item };
							OnOrderIssued(OrderType.Move, mblist3, new object[] { item4 });
							OnOrderIssued(OrderType.LookAtDirection, mblist3, new object[] { formation2.Direction });
						}
					}
					else
					{
						Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "MoveToLineSegment", 2414);
					}
				}
			}
		}

		public static Vec2 GetOrderLookAtDirection(IEnumerable<Formation> formations, Vec2 target)
		{
			if (!formations.Any<Formation>())
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", "GetOrderLookAtDirection", 2434);
				return Vec2.One;
			}
			Formation formation = formations.MaxBy((Formation f) => f.CountOfUnitsWithoutDetachedOnes);
			return (target - formation.OrderPosition).Normalized();
		}

		public static float GetOrderFormCustomWidth(IEnumerable<Formation> formations, Vec3 orderPosition)
		{
			return (Agent.Main.Position - orderPosition).Length;
		}

		public void TransferUnits(Formation source, Formation target, int count)
		{
			source.TransferUnitsAux(target, count, false, count < source.CountOfUnits && target.CountOfUnits > 0);
			OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
			if (onOrderIssued == null)
			{
				return;
			}
			onOrderIssued(OrderType.Transfer, new MBList<Formation> { source }, new object[] { target, count });
		}

		public IEnumerable<Formation> SplitFormation(Formation formation, int count = 2)
		{
			if (!formation.IsSplittableByAI || formation.CountOfUnitsWithoutDetachedOnes < count)
			{
				return new List<Formation> { formation };
			}
			MBDebug.Print(string.Concat(new object[]
			{
				(formation.Team.Side == BattleSideEnum.Attacker) ? "Attacker team" : "Defender team",
				" formation ",
				(int)formation.FormationIndex,
				" split"
			}), 0, Debug.DebugColor.White, 17592186044416UL);
			List<Formation> list = new List<Formation> { formation };
			while (count > 1)
			{
				int num = formation.CountOfUnits / count;
				int i = 0;
				while (i < 8)
				{
					Formation formation2 = formation.Team.GetFormation((FormationClass)i);
					if (formation2.CountOfUnits == 0)
					{
						formation.TransferUnitsAux(formation2, num, false, false);
						list.Add(formation2);
						OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
						if (onOrderIssued == null)
						{
							break;
						}
						onOrderIssued(OrderType.Transfer, new MBList<Formation> { formation }, new object[] { formation2, num });
						break;
					}
					else
					{
						i++;
					}
				}
				count--;
			}
			return list;
		}

		[Conditional("DEBUG")]
		public void TickDebug()
		{
		}

		public void AddOrderOverride(Func<Formation, MovementOrder, MovementOrder> orderOverride)
		{
			if (this.orderOverrides == null)
			{
				this.orderOverrides = new List<Func<Formation, MovementOrder, MovementOrder>>();
				this.overridenOrders = new List<ValueTuple<Formation, OrderType>>();
			}
			this.orderOverrides.Add(orderOverride);
		}

		public OrderType GetOverridenOrderType(Formation formation)
		{
			if (this.overridenOrders == null)
			{
				return OrderType.None;
			}
			ValueTuple<Formation, OrderType> valueTuple = this.overridenOrders.FirstOrDefault((ValueTuple<Formation, OrderType> oo) => oo.Item1 == formation);
			if (valueTuple.Item1 != null)
			{
				return valueTuple.Item2;
			}
			return OrderType.None;
		}

		private void CreateDefaultOrderOverrides()
		{
			this.AddOrderOverride(delegate(Formation formation, MovementOrder order)
			{
				if (formation.ArrangementOrder.OrderType == OrderType.ArrangementCloseOrder && order.OrderType == OrderType.StandYourGround)
				{
					Vec2 averagePosition = formation.QuerySystem.AveragePosition;
					float movementSpeed = formation.QuerySystem.MovementSpeed;
					WorldPosition medianPosition = formation.QuerySystem.MedianPosition;
					medianPosition.SetVec2(averagePosition + formation.Direction * formation.Depth * (0.5f + movementSpeed));
					return MovementOrder.MovementOrderMove(medianPosition);
				}
				return MovementOrder.MovementOrderStop;
			});
		}

		private static void TryCancelStopOrder(Formation formation)
		{
			if (!GameNetwork.IsClientOrReplay && formation.GetReadonlyMovementOrderReference().OrderEnum == MovementOrder.MovementOrderEnum.Stop)
			{
				WorldPosition worldPosition = formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
				if (worldPosition.IsValid)
				{
					formation.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition));
				}
			}
		}

		private static readonly ActionIndexCache act_command = ActionIndexCache.Create("act_command");

		private static readonly ActionIndexCache act_command_leftstance = ActionIndexCache.Create("act_command_leftstance");

		private static readonly ActionIndexCache act_command_unarmed = ActionIndexCache.Create("act_command_unarmed");

		private static readonly ActionIndexCache act_command_unarmed_leftstance = ActionIndexCache.Create("act_command_unarmed_leftstance");

		private static readonly ActionIndexCache act_command_2h = ActionIndexCache.Create("act_command_2h");

		private static readonly ActionIndexCache act_command_2h_leftstance = ActionIndexCache.Create("act_command_2h_leftstance");

		private static readonly ActionIndexCache act_command_bow = ActionIndexCache.Create("act_command_bow");

		private static readonly ActionIndexCache act_command_follow = ActionIndexCache.Create("act_command_follow");

		private static readonly ActionIndexCache act_command_follow_leftstance = ActionIndexCache.Create("act_command_follow_leftstance");

		private static readonly ActionIndexCache act_command_follow_unarmed = ActionIndexCache.Create("act_command_follow_unarmed");

		private static readonly ActionIndexCache act_command_follow_unarmed_leftstance = ActionIndexCache.Create("act_command_follow_unarmed_leftstance");

		private static readonly ActionIndexCache act_command_follow_2h = ActionIndexCache.Create("act_command_follow_2h");

		private static readonly ActionIndexCache act_command_follow_2h_leftstance = ActionIndexCache.Create("act_command_follow_2h_leftstance");

		private static readonly ActionIndexCache act_command_follow_bow = ActionIndexCache.Create("act_command_follow_bow");

		private static readonly ActionIndexCache act_horse_command = ActionIndexCache.Create("act_horse_command");

		private static readonly ActionIndexCache act_horse_command_unarmed = ActionIndexCache.Create("act_horse_command_unarmed");

		private static readonly ActionIndexCache act_horse_command_2h = ActionIndexCache.Create("act_horse_command_2h");

		private static readonly ActionIndexCache act_horse_command_bow = ActionIndexCache.Create("act_horse_command_bow");

		private static readonly ActionIndexCache act_horse_command_follow = ActionIndexCache.Create("act_horse_command_follow");

		private static readonly ActionIndexCache act_horse_command_follow_unarmed = ActionIndexCache.Create("act_horse_command_follow_unarmed");

		private static readonly ActionIndexCache act_horse_command_follow_2h = ActionIndexCache.Create("act_horse_command_follow_2h");

		private static readonly ActionIndexCache act_horse_command_follow_bow = ActionIndexCache.Create("act_horse_command_follow_bow");

		public const float FormationGapInLine = 1.5f;

		private readonly Mission _mission;

		private readonly Team _team;

		public Agent Owner;

		private readonly MBList<Formation> _selectedFormations;

		private Dictionary<Formation, float> actualWidths;

		private Dictionary<Formation, int> actualUnitSpacings;

		private List<Func<Formation, MovementOrder, MovementOrder>> orderOverrides;

		private List<ValueTuple<Formation, OrderType>> overridenOrders;

		private bool _gesturesEnabled;
	}
}
