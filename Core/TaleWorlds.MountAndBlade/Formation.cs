using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ComponentInterfaces;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000208 RID: 520
	public sealed class Formation : IFormation
	{
		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06001CAB RID: 7339 RVA: 0x00065FC0 File Offset: 0x000641C0
		// (remove) Token: 0x06001CAC RID: 7340 RVA: 0x00065FF8 File Offset: 0x000641F8
		public event Action<Formation, Agent> OnUnitAdded;

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x06001CAD RID: 7341 RVA: 0x00066030 File Offset: 0x00064230
		// (remove) Token: 0x06001CAE RID: 7342 RVA: 0x00066068 File Offset: 0x00064268
		public event Action<Formation, Agent> OnUnitRemoved;

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06001CAF RID: 7343 RVA: 0x000660A0 File Offset: 0x000642A0
		// (remove) Token: 0x06001CB0 RID: 7344 RVA: 0x000660D8 File Offset: 0x000642D8
		public event Action<Formation> OnUnitCountChanged;

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06001CB1 RID: 7345 RVA: 0x00066110 File Offset: 0x00064310
		// (remove) Token: 0x06001CB2 RID: 7346 RVA: 0x00066148 File Offset: 0x00064348
		public event Action<Formation> OnUnitSpacingChanged;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x06001CB3 RID: 7347 RVA: 0x00066180 File Offset: 0x00064380
		// (remove) Token: 0x06001CB4 RID: 7348 RVA: 0x000661B8 File Offset: 0x000643B8
		public event Action<Formation> OnTick;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x06001CB5 RID: 7349 RVA: 0x000661F0 File Offset: 0x000643F0
		// (remove) Token: 0x06001CB6 RID: 7350 RVA: 0x00066228 File Offset: 0x00064428
		public event Action<Formation> OnWidthChanged;

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x06001CB7 RID: 7351 RVA: 0x00066260 File Offset: 0x00064460
		// (remove) Token: 0x06001CB8 RID: 7352 RVA: 0x00066298 File Offset: 0x00064498
		public event Action<Formation, MovementOrder.MovementOrderEnum> OnBeforeMovementOrderApplied;

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06001CB9 RID: 7353 RVA: 0x000662D0 File Offset: 0x000644D0
		// (remove) Token: 0x06001CBA RID: 7354 RVA: 0x00066308 File Offset: 0x00064508
		public event Action<Formation, ArrangementOrder.ArrangementOrderEnum> OnAfterArrangementOrderApplied;

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06001CBB RID: 7355 RVA: 0x0006633D File Offset: 0x0006453D
		public FormationClass PrimaryClass
		{
			get
			{
				return this.QuerySystem.MainClass;
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06001CBC RID: 7356 RVA: 0x0006634A File Offset: 0x0006454A
		public int CountOfUnits
		{
			get
			{
				return this.Arrangement.UnitCount + this._detachedUnits.Count;
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x06001CBD RID: 7357 RVA: 0x00066363 File Offset: 0x00064563
		public int CountOfDetachedUnits
		{
			get
			{
				return this._detachedUnits.Count;
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x06001CBE RID: 7358 RVA: 0x00066370 File Offset: 0x00064570
		public int CountOfUndetachableNonPlayerUnits
		{
			get
			{
				return this._undetachableNonPlayerUnitCount;
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06001CBF RID: 7359 RVA: 0x00066378 File Offset: 0x00064578
		public int CountOfUnitsWithoutDetachedOnes
		{
			get
			{
				return this.Arrangement.UnitCount + this._looseDetachedUnits.Count;
			}
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x06001CC0 RID: 7360 RVA: 0x00066391 File Offset: 0x00064591
		public MBList<IFormationUnit> UnitsWithoutLooseDetachedOnes
		{
			get
			{
				return this.Arrangement.GetAllUnits();
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x06001CC1 RID: 7361 RVA: 0x0006639E File Offset: 0x0006459E
		public int CountOfUnitsWithoutLooseDetachedOnes
		{
			get
			{
				return this.Arrangement.UnitCount;
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x06001CC2 RID: 7362 RVA: 0x000663AB File Offset: 0x000645AB
		public int CountOfDetachableNonplayerUnits
		{
			get
			{
				return this.Arrangement.UnitCount - ((this.IsPlayerTroopInFormation || this.HasPlayerControlledTroop) ? 1 : 0) - this.CountOfUndetachableNonPlayerUnits;
			}
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x06001CC3 RID: 7363 RVA: 0x000663D4 File Offset: 0x000645D4
		public Vec2 OrderPosition
		{
			get
			{
				return this._orderPosition.AsVec2;
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06001CC4 RID: 7364 RVA: 0x000663E1 File Offset: 0x000645E1
		public Vec3 OrderGroundPosition
		{
			get
			{
				return this._orderPosition.GetGroundVec3();
			}
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x06001CC5 RID: 7365 RVA: 0x000663EE File Offset: 0x000645EE
		public bool OrderPositionIsValid
		{
			get
			{
				return this._orderPosition.IsValid;
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x06001CC6 RID: 7366 RVA: 0x000663FB File Offset: 0x000645FB
		public float Depth
		{
			get
			{
				return this.Arrangement.Depth;
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x06001CC7 RID: 7367 RVA: 0x00066408 File Offset: 0x00064608
		public float MinimumWidth
		{
			get
			{
				return this.Arrangement.MinimumWidth;
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x06001CC8 RID: 7368 RVA: 0x00066415 File Offset: 0x00064615
		public float MaximumWidth
		{
			get
			{
				return this.Arrangement.MaximumWidth;
			}
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06001CC9 RID: 7369 RVA: 0x00066422 File Offset: 0x00064622
		public float UnitDiameter
		{
			get
			{
				return Formation.GetDefaultUnitDiameter(this.CalculateHasSignificantNumberOfMounted && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x06001CCA RID: 7370 RVA: 0x00066447 File Offset: 0x00064647
		public Vec2 Direction
		{
			get
			{
				return this._direction;
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x06001CCB RID: 7371 RVA: 0x00066450 File Offset: 0x00064650
		public Vec2 CurrentDirection
		{
			get
			{
				return (this.QuerySystem.EstimatedDirection * 0.8f + this.Direction * 0.2f).Normalized();
			}
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x06001CCC RID: 7372 RVA: 0x0006648F File Offset: 0x0006468F
		public Vec2 SmoothedAverageUnitPosition
		{
			get
			{
				return this._smoothedAverageUnitPosition;
			}
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x06001CCD RID: 7373 RVA: 0x00066497 File Offset: 0x00064697
		public int UnitSpacing
		{
			get
			{
				return this._unitSpacing;
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06001CCE RID: 7374 RVA: 0x0006649F File Offset: 0x0006469F
		public MBReadOnlyList<Agent> LooseDetachedUnits
		{
			get
			{
				return this._looseDetachedUnits;
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x06001CCF RID: 7375 RVA: 0x000664A7 File Offset: 0x000646A7
		public MBReadOnlyList<Agent> DetachedUnits
		{
			get
			{
				return this._detachedUnits;
			}
		}

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06001CD0 RID: 7376 RVA: 0x000664AF File Offset: 0x000646AF
		// (set) Token: 0x06001CD1 RID: 7377 RVA: 0x000664B7 File Offset: 0x000646B7
		public AttackEntityOrderDetachment AttackEntityOrderDetachment { get; private set; }

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x06001CD2 RID: 7378 RVA: 0x000664C0 File Offset: 0x000646C0
		// (set) Token: 0x06001CD3 RID: 7379 RVA: 0x000664C8 File Offset: 0x000646C8
		public FormationAI AI { get; private set; }

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06001CD4 RID: 7380 RVA: 0x000664D1 File Offset: 0x000646D1
		// (set) Token: 0x06001CD5 RID: 7381 RVA: 0x000664D9 File Offset: 0x000646D9
		public Formation TargetFormation { get; set; }

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06001CD6 RID: 7382 RVA: 0x000664E2 File Offset: 0x000646E2
		// (set) Token: 0x06001CD7 RID: 7383 RVA: 0x000664EA File Offset: 0x000646EA
		public FormationQuerySystem QuerySystem { get; private set; }

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x06001CD8 RID: 7384 RVA: 0x000664F3 File Offset: 0x000646F3
		public MBReadOnlyList<IDetachment> Detachments
		{
			get
			{
				return this._detachments;
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x06001CD9 RID: 7385 RVA: 0x000664FB File Offset: 0x000646FB
		// (set) Token: 0x06001CDA RID: 7386 RVA: 0x00066503 File Offset: 0x00064703
		public int? OverridenUnitCount { get; private set; }

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x06001CDB RID: 7387 RVA: 0x0006650C File Offset: 0x0006470C
		// (set) Token: 0x06001CDC RID: 7388 RVA: 0x00066514 File Offset: 0x00064714
		public bool IsSpawning { get; private set; }

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06001CDD RID: 7389 RVA: 0x0006651D File Offset: 0x0006471D
		// (set) Token: 0x06001CDE RID: 7390 RVA: 0x00066525 File Offset: 0x00064725
		public bool IsAITickedAfterSplit { get; set; }

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x06001CDF RID: 7391 RVA: 0x0006652E File Offset: 0x0006472E
		// (set) Token: 0x06001CE0 RID: 7392 RVA: 0x00066536 File Offset: 0x00064736
		public bool HasPlayerControlledTroop { get; private set; }

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x06001CE1 RID: 7393 RVA: 0x0006653F File Offset: 0x0006473F
		// (set) Token: 0x06001CE2 RID: 7394 RVA: 0x00066547 File Offset: 0x00064747
		public bool IsPlayerTroopInFormation { get; private set; }

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x06001CE3 RID: 7395 RVA: 0x00066550 File Offset: 0x00064750
		// (set) Token: 0x06001CE4 RID: 7396 RVA: 0x00066558 File Offset: 0x00064758
		public bool ContainsAgentVisuals { get; set; }

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x06001CE5 RID: 7397 RVA: 0x00066561 File Offset: 0x00064761
		// (set) Token: 0x06001CE6 RID: 7398 RVA: 0x00066569 File Offset: 0x00064769
		public FiringOrder FiringOrder { get; set; }

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x06001CE7 RID: 7399 RVA: 0x00066572 File Offset: 0x00064772
		// (set) Token: 0x06001CE8 RID: 7400 RVA: 0x0006657A File Offset: 0x0006477A
		public Agent PlayerOwner
		{
			get
			{
				return this._playerOwner;
			}
			set
			{
				this._playerOwner = value;
				this._isAIControlled = value == null;
			}
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x06001CEA RID: 7402 RVA: 0x000665C0 File Offset: 0x000647C0
		// (set) Token: 0x06001CE9 RID: 7401 RVA: 0x0006658D File Offset: 0x0006478D
		public string BannerCode
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				this._bannerCode = value;
				if (GameNetwork.IsServer)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new InitializeFormation(this, this.Team, this._bannerCode));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
			}
		}

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x06001CEB RID: 7403 RVA: 0x000665C8 File Offset: 0x000647C8
		public bool IsSplittableByAI
		{
			get
			{
				return this.IsAIOwned && this.IsConvenientForTransfer;
			}
		}

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06001CEC RID: 7404 RVA: 0x000665DC File Offset: 0x000647DC
		public bool IsAIOwned
		{
			get
			{
				return !this._enforceNotSplittableByAI && (this.IsAIControlled || (!this.Team.IsPlayerGeneral && (!this.Team.IsPlayerSergeant || this.PlayerOwner != Agent.Main)));
			}
		}

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x06001CED RID: 7405 RVA: 0x0006662B File Offset: 0x0006482B
		public bool IsConvenientForTransfer
		{
			get
			{
				return Mission.Current.MissionTeamAIType != Mission.MissionTeamAITypeEnum.Siege || this.Team.Side != BattleSideEnum.Attacker || this.QuerySystem.InsideCastleUnitCountIncludingUnpositioned == 0;
			}
		}

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06001CEE RID: 7406 RVA: 0x00066658 File Offset: 0x00064858
		public bool EnforceNotSplittableByAI
		{
			get
			{
				return this._enforceNotSplittableByAI;
			}
		}

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06001CEF RID: 7407 RVA: 0x00066660 File Offset: 0x00064860
		public bool IsAIControlled
		{
			get
			{
				return this._isAIControlled;
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06001CF0 RID: 7408 RVA: 0x00066668 File Offset: 0x00064868
		public Vec2 OrderLocalAveragePosition
		{
			get
			{
				if (this._orderLocalAveragePositionIsDirty)
				{
					this._orderLocalAveragePositionIsDirty = false;
					this._orderLocalAveragePosition = default(Vec2);
					if (this.UnitsWithoutLooseDetachedOnes.Count > 0)
					{
						int num = 0;
						foreach (IFormationUnit formationUnit in this.UnitsWithoutLooseDetachedOnes)
						{
							Vec2? localPositionOfUnitOrDefault = this.Arrangement.GetLocalPositionOfUnitOrDefault(formationUnit);
							if (localPositionOfUnitOrDefault != null)
							{
								this._orderLocalAveragePosition += localPositionOfUnitOrDefault.Value;
								num++;
							}
						}
						if (num > 0)
						{
							this._orderLocalAveragePosition *= 1f / (float)num;
						}
					}
				}
				return this._orderLocalAveragePosition;
			}
		}

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x06001CF1 RID: 7409 RVA: 0x0006673C File Offset: 0x0006493C
		// (set) Token: 0x06001CF2 RID: 7410 RVA: 0x00066744 File Offset: 0x00064944
		public FacingOrder FacingOrder
		{
			get
			{
				return this._facingOrder;
			}
			set
			{
				this._facingOrder = value;
			}
		}

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06001CF3 RID: 7411 RVA: 0x0006674D File Offset: 0x0006494D
		// (set) Token: 0x06001CF4 RID: 7412 RVA: 0x00066758 File Offset: 0x00064958
		public ArrangementOrder ArrangementOrder
		{
			get
			{
				return this._arrangementOrder;
			}
			set
			{
				if (value.OrderType == this._arrangementOrder.OrderType)
				{
					this._arrangementOrder.SoftUpdate(this);
					return;
				}
				this._arrangementOrder.OnCancel(this);
				int arrangementOrderDefensivenessChange = ArrangementOrder.GetArrangementOrderDefensivenessChange(this._arrangementOrder.OrderEnum, value.OrderEnum);
				if (arrangementOrderDefensivenessChange != 0 && MovementOrder.GetMovementOrderDefensiveness(this._movementOrder.OrderEnum) != 0)
				{
					this._formationOrderDefensivenessFactor += arrangementOrderDefensivenessChange;
					this.UpdateAgentDrivenPropertiesBasedOnOrderDefensiveness();
				}
				if (this.FormOrder.OrderEnum == FormOrder.FormOrderEnum.Custom)
				{
					this.FormOrder = FormOrder.FormOrderCustom(Formation.TransformCustomWidthBetweenArrangementOrientations(this._arrangementOrder.OrderEnum, value.OrderEnum, this.FormOrder.CustomFlankWidth));
				}
				this._arrangementOrder = value;
				this._arrangementOrder.OnApply(this);
				Action<Formation, ArrangementOrder.ArrangementOrderEnum> onAfterArrangementOrderApplied = this.OnAfterArrangementOrderApplied;
				if (onAfterArrangementOrderApplied == null)
				{
					return;
				}
				onAfterArrangementOrderApplied(this, this._arrangementOrder.OrderEnum);
			}
		}

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06001CF5 RID: 7413 RVA: 0x00066841 File Offset: 0x00064A41
		// (set) Token: 0x06001CF6 RID: 7414 RVA: 0x00066849 File Offset: 0x00064A49
		public FormOrder FormOrder
		{
			get
			{
				return this._formOrder;
			}
			set
			{
				this._formOrder = value;
				this._formOrder.OnApply(this);
			}
		}

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06001CF7 RID: 7415 RVA: 0x0006685E File Offset: 0x00064A5E
		// (set) Token: 0x06001CF8 RID: 7416 RVA: 0x00066868 File Offset: 0x00064A68
		public RidingOrder RidingOrder
		{
			get
			{
				return this._ridingOrder;
			}
			set
			{
				if (this._ridingOrder != value)
				{
					this._ridingOrder = value;
					this.ApplyActionOnEachUnit(delegate(Agent agent)
					{
						agent.SetRidingOrder((int)value.OrderEnum);
					}, null);
					this.Arrangement_OnShapeChanged();
				}
			}
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x06001CF9 RID: 7417 RVA: 0x000668BA File Offset: 0x00064ABA
		// (set) Token: 0x06001CFA RID: 7418 RVA: 0x000668C2 File Offset: 0x00064AC2
		public WeaponUsageOrder WeaponUsageOrder
		{
			get
			{
				return this._weaponUsageOrder;
			}
			set
			{
				this._weaponUsageOrder = value;
			}
		}

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06001CFB RID: 7419 RVA: 0x000668CB File Offset: 0x00064ACB
		private bool IsSimulationFormation
		{
			get
			{
				return this.Team == null;
			}
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06001CFC RID: 7420 RVA: 0x000668D8 File Offset: 0x00064AD8
		public bool HasAnyMountedUnit
		{
			get
			{
				if (this._overridenHasAnyMountedUnit != null)
				{
					return this._overridenHasAnyMountedUnit.Value;
				}
				int num = (int)(this.QuerySystem.GetRangedCavalryUnitRatioWithoutExpiration * (float)this.CountOfUnits + 1E-05f);
				int num2 = (int)(this.QuerySystem.GetCavalryUnitRatioWithoutExpiration * (float)this.CountOfUnits + 1E-05f);
				return num + num2 > 0;
			}
		}

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06001CFD RID: 7421 RVA: 0x00066938 File Offset: 0x00064B38
		public IEnumerable<FormationClass> SecondaryClasses
		{
			get
			{
				FormationClass primaryClass = this.PrimaryClass;
				if (primaryClass != FormationClass.Infantry && this.QuerySystem.InfantryUnitRatio > 0f)
				{
					yield return FormationClass.Infantry;
				}
				if (primaryClass != FormationClass.Ranged && this.QuerySystem.RangedUnitRatio > 0f)
				{
					yield return FormationClass.Ranged;
				}
				if (primaryClass != FormationClass.Cavalry && this.QuerySystem.CavalryUnitRatio > 0f)
				{
					yield return FormationClass.Cavalry;
				}
				if (primaryClass != FormationClass.HorseArcher && this.QuerySystem.RangedCavalryUnitRatio > 0f)
				{
					yield return FormationClass.HorseArcher;
				}
				yield break;
			}
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06001CFE RID: 7422 RVA: 0x00066948 File Offset: 0x00064B48
		// (set) Token: 0x06001CFF RID: 7423 RVA: 0x00066955 File Offset: 0x00064B55
		public float Width
		{
			get
			{
				return this.Arrangement.Width;
			}
			private set
			{
				this.Arrangement.Width = value;
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06001D00 RID: 7424 RVA: 0x00066963 File Offset: 0x00064B63
		public bool IsDeployment
		{
			get
			{
				return Mission.Current.GetMissionBehavior<BattleDeploymentHandler>() != null;
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06001D01 RID: 7425 RVA: 0x00066972 File Offset: 0x00064B72
		public FormationClass InitialClass
		{
			get
			{
				if (this._initialClass == FormationClass.NumberOfAllFormations)
				{
					return this.FormationIndex;
				}
				return this._initialClass;
			}
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06001D02 RID: 7426 RVA: 0x0006698B File Offset: 0x00064B8B
		// (set) Token: 0x06001D03 RID: 7427 RVA: 0x00066994 File Offset: 0x00064B94
		public IFormationArrangement Arrangement
		{
			get
			{
				return this._arrangement;
			}
			set
			{
				if (this._arrangement != null)
				{
					this._arrangement.OnWidthChanged -= this.Arrangement_OnWidthChanged;
					this._arrangement.OnShapeChanged -= this.Arrangement_OnShapeChanged;
				}
				this._arrangement = value;
				if (this._arrangement != null)
				{
					this._arrangement.OnWidthChanged += this.Arrangement_OnWidthChanged;
					this._arrangement.OnShapeChanged += this.Arrangement_OnShapeChanged;
				}
				this.Arrangement_OnWidthChanged();
				this.Arrangement_OnShapeChanged();
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06001D04 RID: 7428 RVA: 0x00066A20 File Offset: 0x00064C20
		public float Interval
		{
			get
			{
				if (this.CalculateHasSignificantNumberOfMounted && !(this.RidingOrder == RidingOrder.RidingOrderDismount))
				{
					return Formation.CavalryInterval(this.UnitSpacing);
				}
				return Formation.InfantryInterval(this.UnitSpacing);
			}
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x06001D05 RID: 7429 RVA: 0x00066A53 File Offset: 0x00064C53
		public bool CalculateHasSignificantNumberOfMounted
		{
			get
			{
				if (this._overridenHasAnyMountedUnit != null)
				{
					return this._overridenHasAnyMountedUnit.Value;
				}
				return this.QuerySystem.CavalryUnitRatio + this.QuerySystem.RangedCavalryUnitRatio >= 0.1f;
			}
		}

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x06001D06 RID: 7430 RVA: 0x00066A8F File Offset: 0x00064C8F
		public float Distance
		{
			get
			{
				if (this.CalculateHasSignificantNumberOfMounted && !(this.RidingOrder == RidingOrder.RidingOrderDismount))
				{
					return Formation.CavalryDistance(this.UnitSpacing);
				}
				return Formation.InfantryDistance(this.UnitSpacing);
			}
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x06001D07 RID: 7431 RVA: 0x00066AC4 File Offset: 0x00064CC4
		public Vec2 CurrentPosition
		{
			get
			{
				return this.QuerySystem.GetAveragePositionWithMaxAge(0.1f) + this.CurrentDirection.TransformToParentUnitF(-this.OrderLocalAveragePosition);
			}
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06001D08 RID: 7432 RVA: 0x00066AFF File Offset: 0x00064CFF
		// (set) Token: 0x06001D09 RID: 7433 RVA: 0x00066B07 File Offset: 0x00064D07
		public Agent Captain
		{
			get
			{
				return this._captain;
			}
			set
			{
				if (this._captain != value)
				{
					this._captain = value;
					this.OnCaptainChanged();
				}
			}
		}

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06001D0A RID: 7434 RVA: 0x00066B1F File Offset: 0x00064D1F
		public float MinimumDistance
		{
			get
			{
				return Formation.GetDefaultMinimumDistance(this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06001D0B RID: 7435 RVA: 0x00066B44 File Offset: 0x00064D44
		public bool IsLoose
		{
			get
			{
				return ArrangementOrder.GetUnitLooseness(this.ArrangementOrder.OrderEnum);
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06001D0C RID: 7436 RVA: 0x00066B56 File Offset: 0x00064D56
		public float MinimumInterval
		{
			get
			{
				return Formation.GetDefaultMinimumInterval(this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06001D0D RID: 7437 RVA: 0x00066B7B File Offset: 0x00064D7B
		public float MaximumInterval
		{
			get
			{
				return Formation.GetDefaultMaximumInterval(this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x06001D0E RID: 7438 RVA: 0x00066BA0 File Offset: 0x00064DA0
		public float MaximumDistance
		{
			get
			{
				return Formation.GetDefaultMaximumDistance(this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x06001D0F RID: 7439 RVA: 0x00066BC5 File Offset: 0x00064DC5
		// (set) Token: 0x06001D10 RID: 7440 RVA: 0x00066BCD File Offset: 0x00064DCD
		internal bool PostponeCostlyOperations { get; private set; }

		// Token: 0x06001D11 RID: 7441 RVA: 0x00066BD8 File Offset: 0x00064DD8
		public Formation(Team team, int index)
		{
			this.Team = team;
			this.Index = index;
			this.FormationIndex = (FormationClass)index;
			this.IsSpawning = false;
			this.Reset();
		}

		// Token: 0x06001D12 RID: 7442 RVA: 0x00066C3C File Offset: 0x00064E3C
		~Formation()
		{
			if (!this.IsSimulationFormation)
			{
				Formation._simulationFormationTemp = null;
			}
		}

		// Token: 0x06001D13 RID: 7443 RVA: 0x00066C70 File Offset: 0x00064E70
		bool IFormation.GetIsLocalPositionAvailable(Vec2 localPosition, Vec2? nearestAvailableUnitPositionLocal)
		{
			Vec2 vec = this.Direction.TransformToParentUnitF(localPosition);
			WorldPosition worldPosition = this.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.NavMeshVec3);
			worldPosition.SetVec2(this.OrderPosition + vec);
			WorldPosition worldPosition2 = WorldPosition.Invalid;
			if (nearestAvailableUnitPositionLocal != null)
			{
				vec = this.Direction.TransformToParentUnitF(nearestAvailableUnitPositionLocal.Value);
				worldPosition2 = this.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.NavMeshVec3);
				worldPosition2.SetVec2(this.OrderPosition + vec);
			}
			float num = MathF.Abs(localPosition.x) + MathF.Abs(localPosition.y) + (this.Interval + this.Distance) * 2f;
			return Mission.Current.IsFormationUnitPositionAvailable(ref this._orderPosition, ref worldPosition, ref worldPosition2, num, this.Team);
		}

		// Token: 0x06001D14 RID: 7444 RVA: 0x00066D34 File Offset: 0x00064F34
		IFormationUnit IFormation.GetClosestUnitTo(Vec2 localPosition, MBList<IFormationUnit> unitsWithSpaces, float? maxDistance)
		{
			Vec2 vec = this.Direction.TransformToParentUnitF(localPosition);
			Vec2 vec2 = this.OrderPosition + vec;
			return this.GetClosestUnitToAux(vec2, unitsWithSpaces, maxDistance);
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x00066D68 File Offset: 0x00064F68
		IFormationUnit IFormation.GetClosestUnitTo(IFormationUnit targetUnit, MBList<IFormationUnit> unitsWithSpaces, float? maxDistance)
		{
			return this.GetClosestUnitToAux(((Agent)targetUnit).Position.AsVec2, unitsWithSpaces, maxDistance);
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x00066D90 File Offset: 0x00064F90
		void IFormation.SetUnitToFollow(IFormationUnit unit, IFormationUnit toFollow, Vec2 vector)
		{
			Agent agent = unit as Agent;
			Agent agent2 = toFollow as Agent;
			agent.SetColumnwiseFollowAgent(agent2, ref vector);
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x00066DB4 File Offset: 0x00064FB4
		bool IFormation.BatchUnitPositions(MBArrayList<Vec2i> orderedPositionIndices, MBArrayList<Vec2> orderedLocalPositions, MBList2D<int> availabilityTable, MBList2D<WorldPosition> globalPositionTable, int fileCount, int rankCount)
		{
			if (this._orderPosition.IsValid && this._orderPosition.GetNavMesh() != UIntPtr.Zero)
			{
				Mission.Current.BatchFormationUnitPositions(orderedPositionIndices, orderedLocalPositions, availabilityTable, globalPositionTable, this._orderPosition, this.Direction, fileCount, rankCount);
				return true;
			}
			return false;
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x00066E07 File Offset: 0x00065007
		public WorldPosition CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache)
		{
			if (worldPositionEnforcedCache != WorldPosition.WorldPositionEnforcedCache.NavMeshVec3)
			{
				if (worldPositionEnforcedCache == WorldPosition.WorldPositionEnforcedCache.GroundVec3)
				{
					this._orderPosition.GetGroundVec3();
				}
			}
			else
			{
				this._orderPosition.GetNavMeshVec3();
			}
			return this._orderPosition;
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x00066E34 File Offset: 0x00065034
		public void SetMovementOrder(MovementOrder input)
		{
			Action<Formation, MovementOrder.MovementOrderEnum> onBeforeMovementOrderApplied = this.OnBeforeMovementOrderApplied;
			if (onBeforeMovementOrderApplied != null)
			{
				onBeforeMovementOrderApplied(this, input.OrderEnum);
			}
			if (input.OrderEnum == MovementOrder.MovementOrderEnum.Invalid)
			{
				input = MovementOrder.MovementOrderStop;
			}
			bool flag = !this._movementOrder.AreOrdersPracticallySame(this._movementOrder, input, this.IsAIControlled);
			if (flag)
			{
				this._movementOrder.OnCancel(this);
			}
			if (flag)
			{
				if (MovementOrder.GetMovementOrderDefensivenessChange(this._movementOrder.OrderEnum, input.OrderEnum) != 0)
				{
					if (MovementOrder.GetMovementOrderDefensiveness(input.OrderEnum) == 0)
					{
						this._formationOrderDefensivenessFactor = 0;
					}
					else
					{
						this._formationOrderDefensivenessFactor = MovementOrder.GetMovementOrderDefensiveness(input.OrderEnum) + ArrangementOrder.GetArrangementOrderDefensiveness(this._arrangementOrder.OrderEnum);
					}
					this.UpdateAgentDrivenPropertiesBasedOnOrderDefensiveness();
				}
				this._movementOrder = input;
				this._movementOrder.OnApply(this);
			}
		}

		// Token: 0x06001D1A RID: 7450 RVA: 0x00066F00 File Offset: 0x00065100
		public void SetControlledByAI(bool isControlledByAI, bool enforceNotSplittableByAI = false)
		{
			if (this._isAIControlled != isControlledByAI)
			{
				this._isAIControlled = isControlledByAI;
				if (this._isAIControlled)
				{
					if (this.AI.ActiveBehavior != null && this.CountOfUnits > 0)
					{
						bool forceTickOccasionally = Mission.Current.ForceTickOccasionally;
						Mission.Current.ForceTickOccasionally = true;
						BehaviorComponent activeBehavior = this.AI.ActiveBehavior;
						this.AI.Tick();
						Mission.Current.ForceTickOccasionally = forceTickOccasionally;
						if (activeBehavior == this.AI.ActiveBehavior)
						{
							this.AI.ActiveBehavior.OnBehaviorActivated();
						}
						this.SetMovementOrder(this.AI.ActiveBehavior.CurrentOrder);
					}
					this._enforceNotSplittableByAI = enforceNotSplittableByAI;
					return;
				}
				this._enforceNotSplittableByAI = false;
			}
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x00066FBA File Offset: 0x000651BA
		public void ResetArrangementOrderTickTimer()
		{
			this._arrangementOrderTickOccasionallyTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x00066FD8 File Offset: 0x000651D8
		public void SetPositioning(WorldPosition? position = null, Vec2? direction = null, int? unitSpacing = null)
		{
			Vec2 orderPosition = this.OrderPosition;
			Vec2 direction2 = this.Direction;
			WorldPosition? worldPosition = null;
			bool flag = false;
			bool flag2 = false;
			if (position != null && position.Value.IsValid)
			{
				if (!this.HasBeenPositioned && !this.IsSimulationFormation)
				{
					this.HasBeenPositioned = true;
				}
				if (position.Value.AsVec2 != this.OrderPosition)
				{
					if (!Mission.Current.IsPositionInsideBoundaries(position.Value.AsVec2))
					{
						Vec2 closestBoundaryPosition = Mission.Current.GetClosestBoundaryPosition(position.Value.AsVec2);
						if (this.OrderPosition != closestBoundaryPosition)
						{
							WorldPosition value = position.Value;
							value.SetVec2(closestBoundaryPosition);
							worldPosition = new WorldPosition?(value);
						}
					}
					else
					{
						worldPosition = position;
					}
				}
			}
			if (direction != null && this.Direction != direction.Value)
			{
				flag = true;
			}
			if (unitSpacing != null && this.UnitSpacing != unitSpacing.Value)
			{
				flag2 = true;
			}
			if (worldPosition != null || flag || flag2)
			{
				this.Arrangement.BeforeFormationFrameChange();
				if (worldPosition != null)
				{
					this._orderPosition = worldPosition.Value;
				}
				if (flag)
				{
					this._direction = direction.Value;
				}
				if (flag2)
				{
					this._unitSpacing = unitSpacing.Value;
					Action<Formation> onUnitSpacingChanged = this.OnUnitSpacingChanged;
					if (onUnitSpacingChanged != null)
					{
						onUnitSpacingChanged(this);
					}
					this.Arrangement_OnShapeChanged();
					this.Arrangement.AreLocalPositionsDirty = true;
				}
				if (!this.IsSimulationFormation && this.Arrangement.IsTurnBackwardsNecessary(orderPosition, worldPosition, direction2, flag, direction))
				{
					this.Arrangement.TurnBackwards();
				}
				this.Arrangement.OnFormationFrameChanged();
				if (worldPosition != null)
				{
					this.ArrangementOrder.OnOrderPositionChanged(this, orderPosition);
				}
			}
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x000671B8 File Offset: 0x000653B8
		public int GetCountOfUnitsWithCondition(Func<Agent, bool> function)
		{
			int num = 0;
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				if (function((Agent)formationUnit))
				{
					num++;
				}
			}
			foreach (Agent agent in this._detachedUnits)
			{
				if (function(agent))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x00067268 File Offset: 0x00065468
		public ref readonly MovementOrder GetReadonlyMovementOrderReference()
		{
			return ref this._movementOrder;
		}

		// Token: 0x06001D1F RID: 7455 RVA: 0x00067270 File Offset: 0x00065470
		public Agent GetFirstUnit()
		{
			return this.GetUnitWithIndex(0);
		}

		// Token: 0x06001D20 RID: 7456 RVA: 0x0006727C File Offset: 0x0006547C
		public int GetCountOfUnitsInClass(FormationClass formationClass, bool excludeBannerBearer)
		{
			int num = 0;
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				bool flag = false;
				switch (formationClass)
				{
				case FormationClass.Infantry:
					flag = (excludeBannerBearer ? QueryLibrary.IsInfantryWithoutBanner((Agent)formationUnit) : QueryLibrary.IsInfantry((Agent)formationUnit));
					break;
				case FormationClass.Ranged:
					flag = (excludeBannerBearer ? QueryLibrary.IsRangedWithoutBanner((Agent)formationUnit) : QueryLibrary.IsRanged((Agent)formationUnit));
					break;
				case FormationClass.Cavalry:
					flag = (excludeBannerBearer ? QueryLibrary.IsCavalryWithoutBanner((Agent)formationUnit) : QueryLibrary.IsCavalry((Agent)formationUnit));
					break;
				case FormationClass.HorseArcher:
					flag = (excludeBannerBearer ? QueryLibrary.IsRangedCavalryWithoutBanner((Agent)formationUnit) : QueryLibrary.IsRangedCavalry((Agent)formationUnit));
					break;
				}
				if (flag)
				{
					num++;
				}
			}
			foreach (Agent agent in this._detachedUnits)
			{
				bool flag2 = false;
				switch (formationClass)
				{
				case FormationClass.Infantry:
					flag2 = (excludeBannerBearer ? QueryLibrary.IsInfantryWithoutBanner(agent) : QueryLibrary.IsInfantry(agent));
					break;
				case FormationClass.Ranged:
					flag2 = (excludeBannerBearer ? QueryLibrary.IsRangedWithoutBanner(agent) : QueryLibrary.IsRanged(agent));
					break;
				case FormationClass.Cavalry:
					flag2 = (excludeBannerBearer ? QueryLibrary.IsCavalryWithoutBanner(agent) : QueryLibrary.IsCavalry(agent));
					break;
				case FormationClass.HorseArcher:
					flag2 = (excludeBannerBearer ? QueryLibrary.IsRangedCavalryWithoutBanner(agent) : QueryLibrary.IsRangedCavalry(agent));
					break;
				}
				if (flag2)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06001D21 RID: 7457 RVA: 0x00067430 File Offset: 0x00065630
		public void SetSpawnIndex(int value = 0)
		{
			this._currentSpawnIndex = value;
		}

		// Token: 0x06001D22 RID: 7458 RVA: 0x00067439 File Offset: 0x00065639
		public int GetNextSpawnIndex()
		{
			int currentSpawnIndex = this._currentSpawnIndex;
			this._currentSpawnIndex++;
			return currentSpawnIndex;
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x00067450 File Offset: 0x00065650
		public Agent GetUnitWithIndex(int unitIndex)
		{
			if (this.Arrangement.GetAllUnits().Count > unitIndex)
			{
				return (Agent)this.Arrangement.GetAllUnits()[unitIndex];
			}
			unitIndex -= this.Arrangement.GetAllUnits().Count;
			if (this._detachedUnits.Count > unitIndex)
			{
				return this._detachedUnits[unitIndex];
			}
			return null;
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x000674B8 File Offset: 0x000656B8
		public Vec2 GetAveragePositionOfUnits(bool excludeDetachedUnits, bool excludePlayer)
		{
			int num = (excludeDetachedUnits ? this.CountOfUnitsWithoutDetachedOnes : this.CountOfUnits);
			if (num > 0)
			{
				Vec2 vec = Vec2.Zero;
				foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
				{
					Agent agent = (Agent)formationUnit;
					if (!excludePlayer || !agent.IsMainAgent)
					{
						vec += agent.Position.AsVec2;
					}
					else
					{
						num--;
					}
				}
				if (excludeDetachedUnits)
				{
					for (int i = 0; i < this._looseDetachedUnits.Count; i++)
					{
						vec += this._looseDetachedUnits[i].Position.AsVec2;
					}
				}
				else
				{
					for (int j = 0; j < this._detachedUnits.Count; j++)
					{
						vec += this._detachedUnits[j].Position.AsVec2;
					}
				}
				if (num > 0)
				{
					return vec * (1f / (float)num);
				}
			}
			return Vec2.Invalid;
		}

		// Token: 0x06001D25 RID: 7461 RVA: 0x000675EC File Offset: 0x000657EC
		public Agent GetMedianAgent(bool excludeDetachedUnits, bool excludePlayer, Vec2 averagePosition)
		{
			excludeDetachedUnits = excludeDetachedUnits && this.CountOfUnitsWithoutDetachedOnes > 0;
			excludePlayer = excludePlayer && (this.CountOfUndetachableNonPlayerUnits > 0 || this.CountOfDetachableNonplayerUnits > 0);
			float num = float.MaxValue;
			Agent agent = null;
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				Agent agent2 = (Agent)formationUnit;
				if (!excludePlayer || !agent2.IsMainAgent)
				{
					float num2 = agent2.Position.AsVec2.DistanceSquared(averagePosition);
					if (num2 <= num)
					{
						agent = agent2;
						num = num2;
					}
				}
			}
			if (excludeDetachedUnits)
			{
				for (int i = 0; i < this._looseDetachedUnits.Count; i++)
				{
					float num3 = this._looseDetachedUnits[i].Position.AsVec2.DistanceSquared(averagePosition);
					if (num3 <= num)
					{
						agent = this._looseDetachedUnits[i];
						num = num3;
					}
				}
			}
			else
			{
				for (int j = 0; j < this._detachedUnits.Count; j++)
				{
					float num4 = this._detachedUnits[j].Position.AsVec2.DistanceSquared(averagePosition);
					if (num4 <= num)
					{
						agent = this._detachedUnits[j];
						num = num4;
					}
				}
			}
			return agent;
		}

		// Token: 0x06001D26 RID: 7462 RVA: 0x0006775C File Offset: 0x0006595C
		public Agent.UnderAttackType GetUnderAttackTypeOfUnits(float timeLimit = 3f)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			timeLimit += MBCommon.GetTotalMissionTime();
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				num = MathF.Max(num, ((Agent)formationUnit).LastMeleeHitTime);
				num2 = MathF.Max(num2, ((Agent)formationUnit).LastRangedHitTime);
				if (num2 >= 0f && num2 < timeLimit)
				{
					return Agent.UnderAttackType.UnderRangedAttack;
				}
				if (num >= 0f && num < timeLimit)
				{
					return Agent.UnderAttackType.UnderMeleeAttack;
				}
			}
			for (int i = 0; i < this._detachedUnits.Count; i++)
			{
				num = MathF.Max(num, this._detachedUnits[i].LastMeleeHitTime);
				num2 = MathF.Max(num2, this._detachedUnits[i].LastRangedHitTime);
				if (num2 >= 0f && num2 < timeLimit)
				{
					return Agent.UnderAttackType.UnderRangedAttack;
				}
				if (num >= 0f && num < timeLimit)
				{
					return Agent.UnderAttackType.UnderMeleeAttack;
				}
			}
			return Agent.UnderAttackType.NotUnderAttack;
		}

		// Token: 0x06001D27 RID: 7463 RVA: 0x0006787C File Offset: 0x00065A7C
		public Agent.MovementBehaviorType GetMovementTypeOfUnits()
		{
			float curMissionTime = MBCommon.GetTotalMissionTime();
			int retreatingCount = 0;
			int attackingCount = 0;
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				if (agent.IsAIControlled && (agent.IsRetreating() || (agent.Formation != null && agent.Formation._movementOrder.OrderType == OrderType.Retreat)))
				{
					int num = retreatingCount;
					retreatingCount = num + 1;
				}
				if (curMissionTime - agent.LastMeleeAttackTime < 3f)
				{
					int num = attackingCount;
					attackingCount = num + 1;
				}
			}, null);
			if (this.CountOfUnits > 0 && (float)retreatingCount / (float)this.CountOfUnits > 0.3f)
			{
				return Agent.MovementBehaviorType.Flee;
			}
			if (attackingCount > 0)
			{
				return Agent.MovementBehaviorType.Engaged;
			}
			return Agent.MovementBehaviorType.Idle;
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x000678E8 File Offset: 0x00065AE8
		public IEnumerable<Agent> GetUnitsWithoutDetachedOnes()
		{
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				yield return formationUnit as Agent;
			}
			List<IFormationUnit>.Enumerator enumerator = default(List<IFormationUnit>.Enumerator);
			int num;
			for (int i = 0; i < this._looseDetachedUnits.Count; i = num + 1)
			{
				yield return this._looseDetachedUnits[i];
				num = i;
			}
			yield break;
			yield break;
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x000678F8 File Offset: 0x00065AF8
		public Vec2 GetWallDirectionOfRelativeFormationLocation(Agent unit)
		{
			if (unit.IsDetachedFromFormation)
			{
				return Vec2.Invalid;
			}
			Vec2? localWallDirectionOfRelativeFormationLocation = this.Arrangement.GetLocalWallDirectionOfRelativeFormationLocation(unit);
			if (localWallDirectionOfRelativeFormationLocation != null)
			{
				return this.Direction.TransformToParentUnitF(localWallDirectionOfRelativeFormationLocation.Value);
			}
			return Vec2.Invalid;
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x00067944 File Offset: 0x00065B44
		public Vec2 GetDirectionOfUnit(Agent unit)
		{
			if (unit.IsDetachedFromFormation)
			{
				return unit.GetMovementDirection();
			}
			Vec2? localDirectionOfUnitOrDefault = this.Arrangement.GetLocalDirectionOfUnitOrDefault(unit);
			if (localDirectionOfUnitOrDefault != null)
			{
				return this.Direction.TransformToParentUnitF(localDirectionOfUnitOrDefault.Value);
			}
			return unit.GetMovementDirection();
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x00067994 File Offset: 0x00065B94
		private WorldPosition GetOrderPositionOfUnitAux(Agent unit)
		{
			WorldPosition? worldPositionOfUnitOrDefault = this.Arrangement.GetWorldPositionOfUnitOrDefault(unit);
			if (worldPositionOfUnitOrDefault != null)
			{
				return worldPositionOfUnitOrDefault.Value;
			}
			WorldPosition worldPosition = this._movementOrder.CreateNewOrderWorldPosition(this, WorldPosition.WorldPositionEnforcedCache.NavMeshVec3);
			if (worldPosition.GetNavMesh() == UIntPtr.Zero || !Mission.Current.IsPositionInsideBoundaries(worldPosition.AsVec2))
			{
				return unit.GetWorldPosition();
			}
			return worldPosition;
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x000679FC File Offset: 0x00065BFC
		public WorldPosition GetOrderPositionOfUnit(Agent unit)
		{
			if (unit.IsDetachedFromFormation && (this._movementOrder.MovementState != MovementOrder.MovementStateEnum.Charge || !unit.Detachment.IsLoose))
			{
				WorldFrame? detachmentFrame = this.GetDetachmentFrame(unit);
				if (detachmentFrame != null)
				{
					return detachmentFrame.Value.Origin;
				}
				return WorldPosition.Invalid;
			}
			else
			{
				switch (this._movementOrder.MovementState)
				{
				case MovementOrder.MovementStateEnum.Charge:
					if (unit.Mission.Mode == MissionMode.Deployment)
					{
						return this.GetOrderPositionOfUnitAux(unit);
					}
					return this._movementOrder.CreateNewOrderWorldPosition(this, WorldPosition.WorldPositionEnforcedCache.None);
				case MovementOrder.MovementStateEnum.Hold:
					return this.GetOrderPositionOfUnitAux(unit);
				case MovementOrder.MovementStateEnum.Retreat:
					return WorldPosition.Invalid;
				case MovementOrder.MovementStateEnum.StandGround:
					return unit.GetWorldPosition();
				default:
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Formation.cs", "GetOrderPositionOfUnit", 1408);
					return WorldPosition.Invalid;
				}
			}
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x00067ACC File Offset: 0x00065CCC
		public Vec2 GetCurrentGlobalPositionOfUnit(Agent unit, bool blendWithOrderDirection)
		{
			if (unit.IsDetachedFromFormation)
			{
				return unit.Position.AsVec2;
			}
			Vec2? localPositionOfUnitOrDefaultWithAdjustment = this.Arrangement.GetLocalPositionOfUnitOrDefaultWithAdjustment(unit, blendWithOrderDirection ? ((this.QuerySystem.EstimatedInterval - this.Interval) * 0.9f) : 0f);
			if (localPositionOfUnitOrDefaultWithAdjustment != null)
			{
				return (blendWithOrderDirection ? this.CurrentDirection : this.QuerySystem.EstimatedDirection).TransformToParentUnitF(localPositionOfUnitOrDefaultWithAdjustment.Value) + this.CurrentPosition;
			}
			return unit.Position.AsVec2;
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x00067B68 File Offset: 0x00065D68
		public float GetAverageMaximumMovementSpeedOfUnits()
		{
			if (this.CountOfUnitsWithoutDetachedOnes == 0)
			{
				return 0.1f;
			}
			float num = 0f;
			foreach (Agent agent in this.GetUnitsWithoutDetachedOnes())
			{
				num += agent.RunSpeedCached;
			}
			return num / (float)this.CountOfUnitsWithoutDetachedOnes;
		}

		// Token: 0x06001D2F RID: 7471 RVA: 0x00067BD4 File Offset: 0x00065DD4
		public float GetMovementSpeedOfUnits()
		{
			float? num;
			float? num2;
			this.ArrangementOrder.GetMovementSpeedRestriction(out num, out num2);
			if (num == null && num2 == null)
			{
				num = new float?(1f);
			}
			if (num2 != null)
			{
				if (this.CountOfUnits == 0)
				{
					return 0.1f;
				}
				IEnumerable<Agent> enumerable;
				if (this.CountOfUnitsWithoutDetachedOnes != 0)
				{
					enumerable = this.GetUnitsWithoutDetachedOnes();
				}
				else
				{
					IEnumerable<Agent> enumerable2 = this._detachedUnits;
					enumerable = enumerable2;
				}
				return enumerable.Min((Agent u) => u.WalkSpeedCached) * num2.Value;
			}
			else
			{
				if (this.CountOfUnits == 0)
				{
					return 0.1f;
				}
				IEnumerable<Agent> enumerable3;
				if (this.CountOfUnitsWithoutDetachedOnes != 0)
				{
					enumerable3 = this.GetUnitsWithoutDetachedOnes();
				}
				else
				{
					IEnumerable<Agent> enumerable2 = this._detachedUnits;
					enumerable3 = enumerable2;
				}
				return enumerable3.Average((Agent u) => u.RunSpeedCached) * num.Value;
			}
		}

		// Token: 0x06001D30 RID: 7472 RVA: 0x00067CC0 File Offset: 0x00065EC0
		public float GetFormationPower()
		{
			float sum = 0f;
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				sum += agent.CharacterPowerCached;
			}, null);
			return sum;
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x00067CF8 File Offset: 0x00065EF8
		public float GetFormationMeleeFightingPower()
		{
			float sum = 0f;
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				sum += agent.CharacterPowerCached * ((this.FormationIndex == FormationClass.Ranged || this.FormationIndex == FormationClass.HorseArcher) ? 0.4f : 1f);
			}, null);
			return sum;
		}

		// Token: 0x06001D32 RID: 7474 RVA: 0x00067D38 File Offset: 0x00065F38
		internal IDetachment GetDetachmentForDebug(Agent agent)
		{
			return this.Detachments.FirstOrDefault((IDetachment d) => d.IsAgentUsingOrInterested(agent));
		}

		// Token: 0x06001D33 RID: 7475 RVA: 0x00067D69 File Offset: 0x00065F69
		public IDetachment GetDetachmentOrDefault(Agent agent)
		{
			return agent.Detachment;
		}

		// Token: 0x06001D34 RID: 7476 RVA: 0x00067D71 File Offset: 0x00065F71
		public WorldFrame? GetDetachmentFrame(Agent agent)
		{
			return agent.Detachment.GetAgentFrame(agent);
		}

		// Token: 0x06001D35 RID: 7477 RVA: 0x00067D80 File Offset: 0x00065F80
		public Vec2 GetMiddleFrontUnitPositionOffset()
		{
			Vec2 localPositionOfReservedUnitPosition = this.Arrangement.GetLocalPositionOfReservedUnitPosition();
			return this.Direction.TransformToParentUnitF(localPositionOfReservedUnitPosition);
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x00067DA8 File Offset: 0x00065FA8
		public List<IFormationUnit> GetUnitsToPopWithReferencePosition(int count, Vec3 targetPosition)
		{
			int num = MathF.Min(count, this.Arrangement.UnitCount);
			List<IFormationUnit> list = ((num == 0) ? new List<IFormationUnit>() : this.Arrangement.GetUnitsToPop(num, targetPosition));
			int num2 = count - list.Count;
			if (num2 > 0)
			{
				List<Agent> list2 = this._looseDetachedUnits.Take(num2).ToList<Agent>();
				num2 -= list2.Count;
				list.AddRange(list2);
			}
			if (num2 > 0)
			{
				IEnumerable<Agent> enumerable = this._detachedUnits.Take(num2);
				num2 -= enumerable.Count<Agent>();
				list.AddRange(enumerable);
			}
			return list;
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x00067E34 File Offset: 0x00066034
		public List<IFormationUnit> GetUnitsToPop(int count)
		{
			int num = MathF.Min(count, this.Arrangement.UnitCount);
			List<IFormationUnit> list = ((num == 0) ? new List<IFormationUnit>() : this.Arrangement.GetUnitsToPop(num));
			int num2 = count - list.Count;
			if (num2 > 0)
			{
				List<Agent> list2 = this._looseDetachedUnits.Take(num2).ToList<Agent>();
				num2 -= list2.Count;
				list.AddRange(list2);
			}
			if (num2 > 0)
			{
				IEnumerable<Agent> enumerable = this._detachedUnits.Take(num2);
				num2 -= enumerable.Count<Agent>();
				list.AddRange(enumerable);
			}
			return list;
		}

		// Token: 0x06001D38 RID: 7480 RVA: 0x00067EBE File Offset: 0x000660BE
		public IEnumerable<ValueTuple<WorldPosition, Vec2>> GetUnavailableUnitPositionsAccordingToNewOrder(Formation simulationFormation, in WorldPosition position, in Vec2 direction, float width, int unitSpacing)
		{
			return Formation.GetUnavailableUnitPositionsAccordingToNewOrder(this, simulationFormation, position, direction, this.Arrangement, width, unitSpacing);
		}

		// Token: 0x06001D39 RID: 7481 RVA: 0x00067EE0 File Offset: 0x000660E0
		public void GetUnitSpawnFrameWithIndex(int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, float width, int unitCount, int unitSpacing, bool isMountedFormation, out WorldPosition? unitSpawnPosition, out Vec2? unitSpawnDirection)
		{
			float num;
			Formation.GetUnitPositionWithIndexAccordingToNewOrder(null, unitIndex, formationPosition, formationDirection, this.Arrangement, width, unitSpacing, unitCount, isMountedFormation, this.Index, out unitSpawnPosition, out unitSpawnDirection, out num);
		}

		// Token: 0x06001D3A RID: 7482 RVA: 0x00067F10 File Offset: 0x00066110
		public void GetUnitPositionWithIndexAccordingToNewOrder(Formation simulationFormation, int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, float width, int unitSpacing, out WorldPosition? unitSpawnPosition, out Vec2? unitSpawnDirection)
		{
			float num;
			Formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, formationPosition, formationDirection, this.Arrangement, width, unitSpacing, this.Arrangement.UnitCount, this.HasAnyMountedUnit, this.Index, out unitSpawnPosition, out unitSpawnDirection, out num);
		}

		// Token: 0x06001D3B RID: 7483 RVA: 0x00067F50 File Offset: 0x00066150
		public void GetUnitPositionWithIndexAccordingToNewOrder(Formation simulationFormation, int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, float width, int unitSpacing, int overridenUnitCount, out WorldPosition? unitPosition, out Vec2? unitDirection)
		{
			float num;
			Formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, formationPosition, formationDirection, this.Arrangement, width, unitSpacing, overridenUnitCount, this.HasAnyMountedUnit, this.Index, out unitPosition, out unitDirection, out num);
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x00067F88 File Offset: 0x00066188
		public void GetUnitPositionWithIndexAccordingToNewOrder(Formation simulationFormation, int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, float width, int unitSpacing, out WorldPosition? unitSpawnPosition, out Vec2? unitSpawnDirection, out float actualWidth)
		{
			Formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, formationPosition, formationDirection, this.Arrangement, width, unitSpacing, this.Arrangement.UnitCount, this.HasAnyMountedUnit, this.Index, out unitSpawnPosition, out unitSpawnDirection, out actualWidth);
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x00067FC8 File Offset: 0x000661C8
		public bool HasUnitsWithCondition(Func<Agent, bool> function)
		{
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				if (function((Agent)formationUnit))
				{
					return true;
				}
			}
			for (int i = 0; i < this._detachedUnits.Count; i++)
			{
				if (function(this._detachedUnits[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x0006805C File Offset: 0x0006625C
		public bool HasUnitsWithCondition(Func<Agent, bool> function, out Agent result)
		{
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				if (function((Agent)formationUnit))
				{
					result = (Agent)formationUnit;
					return true;
				}
			}
			for (int i = 0; i < this._detachedUnits.Count; i++)
			{
				if (function(this._detachedUnits[i]))
				{
					result = this._detachedUnits[i];
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x00068108 File Offset: 0x00066308
		public bool HasAnyEnemyFormationsThatIsNotEmpty()
		{
			foreach (Team team in Mission.Current.Teams)
			{
				if (team.IsEnemyOf(this.Team))
				{
					using (List<Formation>.Enumerator enumerator2 = team.FormationsIncludingSpecialAndEmpty.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.CountOfUnits > 0)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x000681B0 File Offset: 0x000663B0
		public bool HasUnitWithConditionLimitedRandom(Func<Agent, bool> function, int startingIndex, int willBeCheckedUnitCount, out Agent resultAgent)
		{
			int unitCount = this.Arrangement.UnitCount;
			int count = this._detachedUnits.Count;
			if (unitCount + count <= willBeCheckedUnitCount)
			{
				return this.HasUnitsWithCondition(function, out resultAgent);
			}
			for (int i = 0; i < willBeCheckedUnitCount; i++)
			{
				if (startingIndex < unitCount)
				{
					int num = MBRandom.RandomInt(unitCount);
					if (function((Agent)this.Arrangement.GetAllUnits()[num]))
					{
						resultAgent = (Agent)this.Arrangement.GetAllUnits()[num];
						return true;
					}
				}
				else if (count > 0)
				{
					int num = MBRandom.RandomInt(count);
					if (function(this._detachedUnits[num]))
					{
						resultAgent = this._detachedUnits[num];
						return true;
					}
				}
			}
			resultAgent = null;
			return false;
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x0006826C File Offset: 0x0006646C
		public int[] CollectUnitIndices()
		{
			if (this._agentIndicesCache == null || this._agentIndicesCache.Length != this.CountOfUnits)
			{
				this._agentIndicesCache = new int[this.CountOfUnits];
			}
			int num = 0;
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				this._agentIndicesCache[num] = ((Agent)formationUnit).Index;
				num++;
			}
			for (int i = 0; i < this._detachedUnits.Count; i++)
			{
				this._agentIndicesCache[num] = this._detachedUnits[i].Index;
				num++;
			}
			return this._agentIndicesCache;
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x00068338 File Offset: 0x00066538
		public void ApplyActionOnEachUnit(Action<Agent> action, Agent ignoreAgent = null)
		{
			if (ignoreAgent == null)
			{
				foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
				{
					Agent agent = (Agent)formationUnit;
					action(agent);
				}
				for (int i = 0; i < this._detachedUnits.Count; i++)
				{
					action(this._detachedUnits[i]);
				}
				return;
			}
			foreach (IFormationUnit formationUnit2 in this.Arrangement.GetAllUnits())
			{
				Agent agent2 = (Agent)formationUnit2;
				if (agent2 != ignoreAgent)
				{
					action(agent2);
				}
			}
			for (int j = 0; j < this._detachedUnits.Count; j++)
			{
				Agent agent3 = this._detachedUnits[j];
				if (agent3 != ignoreAgent)
				{
					action(agent3);
				}
			}
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x00068448 File Offset: 0x00066648
		public void ApplyActionOnEachAttachedUnit(Action<Agent> action)
		{
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				Agent agent = (Agent)formationUnit;
				action(agent);
			}
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x000684A8 File Offset: 0x000666A8
		public void ApplyActionOnEachDetachedUnit(Action<Agent> action)
		{
			for (int i = 0; i < this._detachedUnits.Count; i++)
			{
				action(this._detachedUnits[i]);
			}
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x000684E0 File Offset: 0x000666E0
		public void ApplyActionOnEachUnitViaBackupList(Action<Agent> action)
		{
			if (this.Arrangement.GetAllUnits().Count > 0)
			{
				foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits().ToArray())
				{
					action((Agent)formationUnit);
				}
			}
			if (this._detachedUnits.Count > 0)
			{
				foreach (Agent agent in this._detachedUnits.ToArray())
				{
					action(agent);
				}
			}
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x00068564 File Offset: 0x00066764
		public void ApplyActionOnEachUnit(Action<Agent, List<WorldPosition>> action, List<WorldPosition> list)
		{
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				action((Agent)formationUnit, list);
			}
			for (int i = 0; i < this._detachedUnits.Count; i++)
			{
				action(this._detachedUnits[i], list);
			}
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x000685EC File Offset: 0x000667EC
		public int CountUnitsOnNavMeshIDMod10(int navMeshID, bool includeOnlyPositionedUnits)
		{
			int num = 0;
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				if (((Agent)formationUnit).GetCurrentNavigationFaceId() % 10 == navMeshID && (!includeOnlyPositionedUnits || this.Arrangement.GetUnpositionedUnits() == null || this.Arrangement.GetUnpositionedUnits().IndexOf(formationUnit) < 0))
				{
					num++;
				}
			}
			if (!includeOnlyPositionedUnits)
			{
				using (List<Agent>.Enumerator enumerator2 = this._detachedUnits.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.GetCurrentNavigationFaceId() % 10 == navMeshID)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x000686C8 File Offset: 0x000668C8
		public void OnAgentControllerChanged(Agent agent, Agent.ControllerType oldController)
		{
			Agent.ControllerType controller = agent.Controller;
			if (oldController != Agent.ControllerType.Player && controller == Agent.ControllerType.Player)
			{
				this.HasPlayerControlledTroop = true;
				if (!GameNetwork.IsMultiplayer)
				{
					this.TryRelocatePlayerUnit();
				}
				if (!agent.IsDetachableFromFormation)
				{
					this.OnUndetachableNonPlayerUnitRemoved(agent);
					return;
				}
			}
			else if (oldController == Agent.ControllerType.Player && controller != Agent.ControllerType.Player)
			{
				this.HasPlayerControlledTroop = false;
				if (!agent.IsDetachableFromFormation)
				{
					this.OnUndetachableNonPlayerUnitAdded(agent);
				}
			}
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x00068726 File Offset: 0x00066926
		public void OnMassUnitTransferStart()
		{
			this.PostponeCostlyOperations = true;
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x00068730 File Offset: 0x00066930
		public void OnMassUnitTransferEnd()
		{
			this.FormOrder = this.FormOrder;
			this.QuerySystem.Expire();
			this.Team.QuerySystem.ExpireAfterUnitAddRemove();
			this.PostponeCostlyOperations = false;
			if (this._formationClassNeedsUpdate)
			{
				this.CalculateFormationClass();
			}
			if (Mission.Current.IsTeleportingAgents)
			{
				this.SetPositioning(new WorldPosition?(this._orderPosition), null, null);
				this.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					agent.UpdateCachedAndFormationValues(true, false);
				}, null);
			}
		}

		// Token: 0x06001D4B RID: 7499 RVA: 0x000687CE File Offset: 0x000669CE
		public void OnBatchUnitRemovalStart()
		{
			this.PostponeCostlyOperations = true;
			this.Arrangement.OnBatchRemoveStart();
		}

		// Token: 0x06001D4C RID: 7500 RVA: 0x000687E2 File Offset: 0x000669E2
		public void OnBatchUnitRemovalEnd()
		{
			this.Arrangement.OnBatchRemoveEnd();
			this.FormOrder = this.FormOrder;
			this.QuerySystem.ExpireAfterUnitAddRemove();
			this.Team.QuerySystem.ExpireAfterUnitAddRemove();
			this.PostponeCostlyOperations = false;
		}

		// Token: 0x06001D4D RID: 7501 RVA: 0x00068820 File Offset: 0x00066A20
		public void OnUnitAddedOrRemoved()
		{
			if (!this.PostponeCostlyOperations)
			{
				this.FormOrder = this.FormOrder;
				this.QuerySystem.ExpireAfterUnitAddRemove();
				Team team = this.Team;
				if (team != null)
				{
					team.QuerySystem.ExpireAfterUnitAddRemove();
				}
			}
			Action<Formation> onUnitCountChanged = this.OnUnitCountChanged;
			if (onUnitCountChanged == null)
			{
				return;
			}
			onUnitCountChanged(this);
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x00068873 File Offset: 0x00066A73
		public void OnAgentLostMount(Agent agent)
		{
			if (!agent.IsDetachedFromFormation)
			{
				this._arrangement.OnUnitLostMount(agent);
			}
		}

		// Token: 0x06001D4F RID: 7503 RVA: 0x00068889 File Offset: 0x00066A89
		public void OnFormationDispersed()
		{
			this.Arrangement.OnFormationDispersed();
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				agent.UpdateCachedAndFormationValues(true, false);
			}, null);
		}

		// Token: 0x06001D50 RID: 7504 RVA: 0x000688BC File Offset: 0x00066ABC
		public void OnUnitDetachmentChanged(Agent unit, bool isOldDetachmentLoose, bool isNewDetachmentLoose)
		{
			if (isOldDetachmentLoose && !isNewDetachmentLoose)
			{
				this._looseDetachedUnits.Remove(unit);
				return;
			}
			if (!isOldDetachmentLoose && isNewDetachmentLoose)
			{
				this._looseDetachedUnits.Add(unit);
			}
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x000688E6 File Offset: 0x00066AE6
		public void OnUndetachableNonPlayerUnitAdded(Agent unit)
		{
			if (unit.Formation == this && !unit.IsPlayerControlled)
			{
				this._undetachableNonPlayerUnitCount++;
			}
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x0006890D File Offset: 0x00066B0D
		public void OnUndetachableNonPlayerUnitRemoved(Agent unit)
		{
			if (unit.Formation == this && !unit.IsPlayerControlled)
			{
				this._undetachableNonPlayerUnitCount--;
			}
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x00068934 File Offset: 0x00066B34
		public void ReleaseFormationFromAI()
		{
			this._isAIControlled = false;
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x0006893D File Offset: 0x00066B3D
		public void ResetMovementOrderPositionCache()
		{
			this._movementOrder.ResetPositionCache();
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x0006894C File Offset: 0x00066B4C
		public void Reset()
		{
			this.Arrangement = new LineFormation(this, true);
			this._arrangementOrderTickOccasionallyTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
			this.ResetAux();
			this.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			this._enforceNotSplittableByAI = false;
			this.ContainsAgentVisuals = false;
			this.PlayerOwner = null;
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x000689A8 File Offset: 0x00066BA8
		public IEnumerable<Formation> Split(int count = 2)
		{
			foreach (Formation formation in this.Team.FormationsIncludingEmpty)
			{
				formation.PostponeCostlyOperations = true;
			}
			IEnumerable<Formation> enumerable = this.Team.MasterOrderController.SplitFormation(this, count);
			if (enumerable.Count<Formation>() > 1 && this.Team != null)
			{
				foreach (Formation formation2 in enumerable)
				{
					formation2.QuerySystem.Expire();
				}
			}
			foreach (Formation formation3 in this.Team.FormationsIncludingEmpty)
			{
				formation3.PostponeCostlyOperations = false;
			}
			return enumerable;
		}

		// Token: 0x06001D57 RID: 7511 RVA: 0x00068AA4 File Offset: 0x00066CA4
		public void TransferUnits(Formation target, int unitCount)
		{
			this.PostponeCostlyOperations = true;
			target.PostponeCostlyOperations = true;
			this.Team.MasterOrderController.TransferUnits(this, target, unitCount);
			this.PostponeCostlyOperations = false;
			target.PostponeCostlyOperations = false;
			this.QuerySystem.Expire();
			target.QuerySystem.Expire();
			this.Team.QuerySystem.ExpireAfterUnitAddRemove();
			target.Team.QuerySystem.ExpireAfterUnitAddRemove();
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x00068B18 File Offset: 0x00066D18
		public void TransferUnitsAux(Formation target, int unitCount, bool isPlayerOrder, bool useSelectivePop)
		{
			if (!isPlayerOrder && !this.IsSplittableByAI)
			{
				return;
			}
			MBDebug.Print(string.Concat(new object[]
			{
				this.FormationIndex.GetName(),
				" has ",
				this.CountOfUnits,
				" units, ",
				target.FormationIndex.GetName(),
				" has ",
				target.CountOfUnits,
				" units"
			}), 0, Debug.DebugColor.White, 17592186044416UL);
			MBDebug.Print(string.Concat(new object[]
			{
				this.Team.Side,
				" ",
				this.FormationIndex.GetName(),
				" transfers ",
				unitCount,
				" units to ",
				target.FormationIndex.GetName()
			}), 0, Debug.DebugColor.White, 17592186044416UL);
			if (unitCount == 0)
			{
				return;
			}
			if (target.CountOfUnits == 0)
			{
				target.CopyOrdersFrom(this);
				target.SetPositioning(new WorldPosition?(this._orderPosition), new Vec2?(this._direction), new int?(this._unitSpacing));
			}
			BattleBannerBearersModel battleBannerBearersModel = MissionGameModels.Current.BattleBannerBearersModel;
			List<IFormationUnit> list;
			if (battleBannerBearersModel.GetFormationBanner(this) == null)
			{
				list = (useSelectivePop ? this.GetUnitsToPopWithReferencePosition(unitCount, target.OrderPositionIsValid ? target.OrderPosition.ToVec3(0f) : target.QuerySystem.MedianPosition.GetGroundVec3()) : this.GetUnitsToPop(unitCount).ToList<IFormationUnit>());
			}
			else
			{
				List<Agent> formationBannerBearers = battleBannerBearersModel.GetFormationBannerBearers(this);
				int num = Math.Min(this.CountOfUnits, unitCount + formationBannerBearers.Count);
				list = (useSelectivePop ? this.GetUnitsToPopWithReferencePosition(num, target.OrderPositionIsValid ? target.OrderPosition.ToVec3(0f) : target.QuerySystem.MedianPosition.GetGroundVec3()) : this.GetUnitsToPop(num).ToList<IFormationUnit>());
				foreach (Agent agent in formationBannerBearers)
				{
					if (list.Count <= unitCount)
					{
						break;
					}
					list.Remove(agent);
				}
				if (list.Count > unitCount)
				{
					int num2 = list.Count - unitCount;
					list.RemoveRange(list.Count - num2, num2);
				}
			}
			if (battleBannerBearersModel.GetFormationBanner(target) != null)
			{
				foreach (Agent agent2 in battleBannerBearersModel.GetFormationBannerBearers(target))
				{
					if (agent2.Formation == this && !list.Contains(agent2))
					{
						int num3 = list.FindIndex(delegate(IFormationUnit unit)
						{
							Agent agent3;
							return (agent3 = unit as Agent) != null && agent3.Banner == null;
						});
						if (num3 < 0)
						{
							break;
						}
						list[num3] = agent2;
					}
				}
			}
			foreach (IFormationUnit formationUnit in list)
			{
				((Agent)formationUnit).Formation = target;
			}
			this.Team.TriggerOnFormationsChanged(this);
			this.Team.TriggerOnFormationsChanged(target);
			MBDebug.Print(string.Concat(new object[]
			{
				this.FormationIndex.GetName(),
				" has ",
				this.CountOfUnits,
				" units, ",
				target.FormationIndex.GetName(),
				" has ",
				target.CountOfUnits,
				" units"
			}), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x00068EEC File Offset: 0x000670EC
		[Conditional("DEBUG")]
		public void DebugArrangements()
		{
			foreach (Team team in Mission.Current.Teams)
			{
				foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						formation.ApplyActionOnEachUnit(delegate(Agent agent)
						{
							agent.AgentVisuals.SetContourColor(null, true);
						}, null);
					}
				}
			}
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				agent.AgentVisuals.SetContourColor(new uint?(4294901760U), true);
			}, null);
			Vec3 vec = this.Direction.ToVec3(0f);
			vec.RotateAboutZ(1.5707964f);
			bool isSimulationFormation = this.IsSimulationFormation;
			vec * this.Width * 0.5f;
			this.Direction.ToVec3(0f) * this.Depth * 0.5f;
			bool orderPositionIsValid = this.OrderPositionIsValid;
			this.QuerySystem.MedianPosition.SetVec2(this.CurrentPosition);
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				WorldPosition orderPositionOfUnit = this.GetOrderPositionOfUnit(agent);
				if (orderPositionOfUnit.IsValid)
				{
					Vec2 vec2 = this.GetDirectionOfUnit(agent);
					vec2.Normalize();
					vec2 *= 0.1f;
					orderPositionOfUnit.GetGroundVec3() + vec2.ToVec3(0f);
					orderPositionOfUnit.GetGroundVec3() - vec2.LeftVec().ToVec3(0f);
					orderPositionOfUnit.GetGroundVec3() + vec2.LeftVec().ToVec3(0f);
					string.Concat(new object[]
					{
						"(",
						((IFormationUnit)agent).FormationFileIndex,
						",",
						((IFormationUnit)agent).FormationRankIndex,
						")"
					});
				}
			}, null);
			bool orderPositionIsValid2 = this.OrderPositionIsValid;
			foreach (IDetachment detachment in this.Detachments)
			{
				UsableMachine usableMachine = detachment as UsableMachine;
				RangedSiegeWeapon rangedSiegeWeapon = detachment as RangedSiegeWeapon;
			}
			if (this.Arrangement is ColumnFormation)
			{
				this.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					agent.GetFollowedUnit();
					string.Concat(new object[]
					{
						"(",
						((IFormationUnit)agent).FormationFileIndex,
						",",
						((IFormationUnit)agent).FormationRankIndex,
						")"
					});
				}, null);
			}
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x000690EC File Offset: 0x000672EC
		public void AddUnit(Agent unit)
		{
			bool countOfUnits = this.CountOfUnits != 0;
			if (this.Arrangement.AddUnit(unit) && Mission.Current.HasMissionBehavior<AmmoSupplyLogic>() && Mission.Current.GetMissionBehavior<AmmoSupplyLogic>().IsAgentEligibleForAmmoSupply(unit))
			{
				unit.SetScriptedCombatFlags(unit.GetScriptedCombatFlags() | Agent.AISpecialCombatModeFlags.IgnoreAmmoLimitForRangeCalculation);
				unit.ResetAiWaitBeforeShootFactor();
				unit.UpdateAgentStats();
			}
			if (unit.IsPlayerControlled)
			{
				this.HasPlayerControlledTroop = true;
			}
			if (unit.IsPlayerTroop)
			{
				this.IsPlayerTroopInFormation = true;
			}
			if (!unit.IsDetachableFromFormation && !unit.IsPlayerControlled)
			{
				this.OnUndetachableNonPlayerUnitAdded(unit);
			}
			if (unit.Character != null)
			{
				if (this._initialClass == FormationClass.NumberOfAllFormations)
				{
					this._initialClass = (FormationClass)unit.Character.DefaultFormationGroup;
				}
				else if (this._initialClass != (FormationClass)unit.Character.DefaultFormationGroup)
				{
					if (this.PostponeCostlyOperations)
					{
						this._formationClassNeedsUpdate = true;
					}
					else
					{
						this.CalculateFormationClass();
						this._formationClassNeedsUpdate = false;
					}
				}
			}
			this._movementOrder.OnUnitJoinOrLeave(this, unit, true);
			this.OnUnitAddedOrRemoved();
			Action<Formation, Agent> onUnitAdded = this.OnUnitAdded;
			if (onUnitAdded != null)
			{
				onUnitAdded(this, unit);
			}
			if (!countOfUnits && this.CountOfUnits > 0)
			{
				if (Mission.Current.Mode == MissionMode.Battle && !this.IsAIControlled)
				{
					this.SetControlledByAI(true, false);
				}
				TeamAIComponent teamAI = this.Team.TeamAI;
				if (teamAI == null)
				{
					return;
				}
				teamAI.OnUnitAddedToFormationForTheFirstTime(this);
			}
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x0006923C File Offset: 0x0006743C
		public void RemoveUnit(Agent unit)
		{
			if (unit.IsDetachedFromFormation)
			{
				unit.Detachment.RemoveAgent(unit);
				this._detachedUnits.Remove(unit);
				this._looseDetachedUnits.Remove(unit);
				unit.Detachment = null;
				unit.DetachmentWeight = -1f;
			}
			else
			{
				this.Arrangement.RemoveUnit(unit);
			}
			if (unit.IsPlayerTroop)
			{
				this.IsPlayerTroopInFormation = false;
			}
			if (unit.IsPlayerControlled)
			{
				this.HasPlayerControlledTroop = false;
			}
			if (unit == this.Captain && !unit.CanLeadFormationsRemotely)
			{
				this.Captain = null;
			}
			if (!unit.IsDetachableFromFormation && !unit.IsPlayerControlled)
			{
				this.OnUndetachableNonPlayerUnitRemoved(unit);
			}
			this._movementOrder.OnUnitJoinOrLeave(this, unit, false);
			this.OnUnitAddedOrRemoved();
			Action<Formation, Agent> onUnitRemoved = this.OnUnitRemoved;
			if (onUnitRemoved == null)
			{
				return;
			}
			onUnitRemoved(this, unit);
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x0006930A File Offset: 0x0006750A
		public void DetachUnit(Agent unit, bool isLoose)
		{
			this.Arrangement.RemoveUnit(unit);
			this._detachedUnits.Add(unit);
			if (isLoose)
			{
				this._looseDetachedUnits.Add(unit);
			}
			this.OnUnitAttachedOrDetached();
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x0006933C File Offset: 0x0006753C
		public void AttachUnit(Agent unit)
		{
			this._detachedUnits.Remove(unit);
			this._looseDetachedUnits.Remove(unit);
			this.Arrangement.AddUnit(unit);
			unit.Detachment = null;
			unit.DetachmentWeight = -1f;
			this.OnUnitAttachedOrDetached();
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x00069388 File Offset: 0x00067588
		public void SwitchUnitLocations(Agent firstUnit, Agent secondUnit)
		{
			if (!firstUnit.IsDetachedFromFormation && !secondUnit.IsDetachedFromFormation && (((IFormationUnit)firstUnit).FormationFileIndex != -1 || ((IFormationUnit)secondUnit).FormationFileIndex != -1))
			{
				if (((IFormationUnit)firstUnit).FormationFileIndex == -1)
				{
					this.Arrangement.SwitchUnitLocationsWithUnpositionedUnit(secondUnit, firstUnit);
					return;
				}
				if (((IFormationUnit)secondUnit).FormationFileIndex == -1)
				{
					this.Arrangement.SwitchUnitLocationsWithUnpositionedUnit(firstUnit, secondUnit);
					return;
				}
				this.Arrangement.SwitchUnitLocations(firstUnit, secondUnit);
			}
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x000693F4 File Offset: 0x000675F4
		public void Tick(float dt)
		{
			if (this.Team.HasTeamAi && (this.IsAIControlled || this.Team.IsPlayerSergeant) && this.CountOfUnitsWithoutDetachedOnes > 0)
			{
				this.AI.Tick();
			}
			else
			{
				this.IsAITickedAfterSplit = true;
			}
			int num = 0;
			while (!this._movementOrder.IsApplicable(this) && num++ < 10)
			{
				this.SetMovementOrder(this._movementOrder.GetSubstituteOrder(this));
			}
			if (this._arrangementOrderTickOccasionallyTimer.Check(Mission.Current.CurrentTime))
			{
				this._arrangementOrder.TickOccasionally(this);
			}
			this._movementOrder.Tick(this);
			WorldPosition worldPosition = this._movementOrder.CreateNewOrderWorldPosition(this, WorldPosition.WorldPositionEnforcedCache.None);
			Vec2 direction = this._facingOrder.GetDirection(this, this._movementOrder._targetAgent);
			if (worldPosition.IsValid || direction.IsValid)
			{
				this.SetPositioning(new WorldPosition?(worldPosition), new Vec2?(direction), null);
			}
			this.TickDetachments(dt);
			Action<Formation> onTick = this.OnTick;
			if (onTick != null)
			{
				onTick(this);
			}
			this.SmoothAverageUnitPosition(dt);
			if (this._isArrangementShapeChanged)
			{
				this._isArrangementShapeChanged = false;
			}
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x00069528 File Offset: 0x00067728
		public void JoinDetachment(IDetachment detachment)
		{
			if (!this.Team.DetachmentManager.ContainsDetachment(detachment))
			{
				this.Team.DetachmentManager.MakeDetachment(detachment);
			}
			this._detachments.Add(detachment);
			this.Team.DetachmentManager.OnFormationJoinDetachment(this, detachment);
		}

		// Token: 0x06001D61 RID: 7521 RVA: 0x00069577 File Offset: 0x00067777
		public void FormAttackEntityDetachment(GameEntity targetEntity)
		{
			this.AttackEntityOrderDetachment = new AttackEntityOrderDetachment(targetEntity);
			this.JoinDetachment(this.AttackEntityOrderDetachment);
		}

		// Token: 0x06001D62 RID: 7522 RVA: 0x00069591 File Offset: 0x00067791
		public void LeaveDetachment(IDetachment detachment)
		{
			detachment.OnFormationLeave(this);
			this._detachments.Remove(detachment);
			this.Team.DetachmentManager.OnFormationLeaveDetachment(this, detachment);
		}

		// Token: 0x06001D63 RID: 7523 RVA: 0x000695B9 File Offset: 0x000677B9
		public void DisbandAttackEntityDetachment()
		{
			if (this.AttackEntityOrderDetachment != null)
			{
				this.Team.DetachmentManager.DestroyDetachment(this.AttackEntityOrderDetachment);
				this.AttackEntityOrderDetachment = null;
			}
		}

		// Token: 0x06001D64 RID: 7524 RVA: 0x000695E0 File Offset: 0x000677E0
		public void Rearrange(IFormationArrangement arrangement)
		{
			if (this.Arrangement.GetType() == arrangement.GetType())
			{
				return;
			}
			IFormationArrangement arrangement2 = this.Arrangement;
			this.Arrangement = arrangement;
			arrangement2.RearrangeTo(arrangement);
			arrangement.RearrangeFrom(arrangement2);
			arrangement2.RearrangeTransferUnits(arrangement);
			this.FormOrder = this.FormOrder;
			this._movementOrder.OnArrangementChanged(this);
		}

		// Token: 0x06001D65 RID: 7525 RVA: 0x00069644 File Offset: 0x00067844
		public void TickForColumnArrangementInitialPositioning(Formation formation)
		{
			if ((this.ReferencePosition.Value - this.OrderPosition).LengthSquared >= 1f && !this.IsDeployment)
			{
				this.ArrangementOrder.RearrangeAux(this, true);
			}
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x00069690 File Offset: 0x00067890
		public float CalculateFormationDirectionEnforcingFactorForRank(int rankIndex)
		{
			if (rankIndex == -1)
			{
				return 0f;
			}
			return this.ArrangementOrder.CalculateFormationDirectionEnforcingFactorForRank(rankIndex, this.Arrangement.RankCount);
		}

		// Token: 0x06001D67 RID: 7527 RVA: 0x000696C1 File Offset: 0x000678C1
		public void BeginSpawn(int unitCount, bool isMounted)
		{
			this.IsSpawning = true;
			this.OverridenUnitCount = new int?(unitCount);
			this._overridenHasAnyMountedUnit = new bool?(isMounted);
		}

		// Token: 0x06001D68 RID: 7528 RVA: 0x000696E4 File Offset: 0x000678E4
		public void EndSpawn()
		{
			this.IsSpawning = false;
			this.OverridenUnitCount = null;
			this._overridenHasAnyMountedUnit = null;
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x00069713 File Offset: 0x00067913
		internal bool IsUnitDetachedForDebug(Agent unit)
		{
			return this._detachedUnits.Contains(unit);
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x00069724 File Offset: 0x00067924
		internal IEnumerable<IFormationUnit> GetUnitsToPopWithPriorityFunction(int count, Func<Agent, int> priorityFunction, List<Agent> excludedHeroes, bool excludeBannerman)
		{
			Formation.<>c__DisplayClass317_0 CS$<>8__locals1 = new Formation.<>c__DisplayClass317_0();
			CS$<>8__locals1.excludedHeroes = excludedHeroes;
			CS$<>8__locals1.excludeBannerman = excludeBannerman;
			CS$<>8__locals1.priorityFunction = priorityFunction;
			List<IFormationUnit> list = new List<IFormationUnit>();
			if (count <= 0)
			{
				return list;
			}
			CS$<>8__locals1.selectCondition = (Agent agent) => !CS$<>8__locals1.excludedHeroes.Contains(agent3) && (!CS$<>8__locals1.excludeBannerman || agent3.Banner == null);
			List<Agent> list2 = (from unit in this._arrangement.GetAllUnits().Concat(this._detachedUnits).Where(delegate(IFormationUnit unit)
				{
					Agent agent3;
					return (agent3 = unit as Agent) != null && CS$<>8__locals1.selectCondition(agent3);
				})
				select unit as Agent).ToList<Agent>();
			if (list2.IsEmpty<Agent>())
			{
				return list;
			}
			int num = count;
			CS$<>8__locals1.bestFit = int.MaxValue;
			while (num > 0 && CS$<>8__locals1.bestFit > 0 && list2.Count > 0)
			{
				Formation.<>c__DisplayClass317_1 CS$<>8__locals2 = new Formation.<>c__DisplayClass317_1();
				Formation.<>c__DisplayClass317_0 CS$<>8__locals3 = CS$<>8__locals1;
				IEnumerable<Agent> enumerable = list2;
				Func<Agent, int> func;
				if ((func = CS$<>8__locals1.<>9__3) == null)
				{
					func = (CS$<>8__locals1.<>9__3 = (Agent unit) => CS$<>8__locals1.priorityFunction(unit));
				}
				CS$<>8__locals3.bestFit = enumerable.Max(func);
				Formation.<>c__DisplayClass317_1 CS$<>8__locals4 = CS$<>8__locals2;
				Func<IFormationUnit, bool> func2;
				if ((func2 = CS$<>8__locals1.<>9__4) == null)
				{
					func2 = (CS$<>8__locals1.<>9__4 = delegate(IFormationUnit unit)
					{
						Agent agent2;
						return (agent2 = unit as Agent) != null && CS$<>8__locals1.selectCondition(agent2) && CS$<>8__locals1.priorityFunction(agent2) == CS$<>8__locals1.bestFit;
					});
				}
				CS$<>8__locals4.bestFitCondition = func2;
				int num2 = Math.Min(num, this._arrangement.GetAllUnits().Count((IFormationUnit unit) => CS$<>8__locals2.bestFitCondition(unit)));
				if (num2 > 0)
				{
					IEnumerable<IFormationUnit> toPop2 = this._arrangement.GetUnitsToPopWithCondition(num2, CS$<>8__locals2.bestFitCondition);
					if (!toPop2.IsEmpty<IFormationUnit>())
					{
						list.AddRange(toPop2);
						num -= toPop2.Count<IFormationUnit>();
						list2.RemoveAll((Agent unit) => toPop2.Contains(unit));
					}
				}
				if (num > 0)
				{
					IEnumerable<Agent> toPop3 = this._looseDetachedUnits.Where((Agent agent) => CS$<>8__locals2.bestFitCondition(agent)).Take(num);
					if (!toPop3.IsEmpty<Agent>())
					{
						list.AddRange(toPop3);
						num -= toPop3.Count<Agent>();
						list2.RemoveAll((Agent unit) => toPop3.Contains(unit));
					}
				}
				if (num > 0)
				{
					IEnumerable<Agent> toPop = this._detachedUnits.Where((Agent agent) => CS$<>8__locals2.bestFitCondition(agent)).Take(num);
					if (!toPop.IsEmpty<Agent>())
					{
						list.AddRange(toPop);
						num -= toPop.Count<Agent>();
						list2.RemoveAll((Agent unit) => toPop.Contains(unit));
					}
				}
			}
			return list;
		}

		// Token: 0x06001D6B RID: 7531 RVA: 0x000699B4 File Offset: 0x00067BB4
		internal void TransferUnitsWithPriorityFunction(Formation target, int unitCount, Func<Agent, int> priorityFunction, bool excludeBannerman, List<Agent> excludedAgents)
		{
			MBDebug.Print(string.Concat(new object[]
			{
				this.FormationIndex.GetName(),
				" has ",
				this.CountOfUnits,
				" units, ",
				target.FormationIndex.GetName(),
				" has ",
				target.CountOfUnits,
				" units"
			}), 0, Debug.DebugColor.White, 17592186044416UL);
			MBDebug.Print(string.Concat(new object[]
			{
				this.Team.Side.ToString(),
				" ",
				this.FormationIndex.GetName(),
				" transfers ",
				unitCount,
				" units to ",
				target.FormationIndex.GetName()
			}), 0, Debug.DebugColor.White, 17592186044416UL);
			if (unitCount == 0)
			{
				return;
			}
			if (target.CountOfUnits == 0)
			{
				target.CopyOrdersFrom(this);
				target.SetPositioning(new WorldPosition?(this._orderPosition), new Vec2?(this._direction), new int?(this._unitSpacing));
			}
			foreach (IFormationUnit formationUnit in new List<IFormationUnit>(this.GetUnitsToPopWithPriorityFunction(unitCount, priorityFunction, excludedAgents, excludeBannerman)))
			{
				((Agent)formationUnit).Formation = target;
			}
			this.Team.TriggerOnFormationsChanged(this);
			this.Team.TriggerOnFormationsChanged(target);
			MBDebug.Print(string.Concat(new object[]
			{
				this.FormationIndex.GetName(),
				" has ",
				this.CountOfUnits,
				" units, ",
				target.FormationIndex.GetName(),
				" has ",
				target.CountOfUnits,
				" units"
			}), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06001D6C RID: 7532 RVA: 0x00069BC4 File Offset: 0x00067DC4
		private IFormationUnit GetClosestUnitToAux(Vec2 position, MBList<IFormationUnit> unitsWithSpaces, float? maxDistance)
		{
			if (unitsWithSpaces == null)
			{
				unitsWithSpaces = this.Arrangement.GetAllUnits();
			}
			IFormationUnit formationUnit = null;
			float num = ((maxDistance != null) ? (maxDistance.Value * maxDistance.Value) : float.MaxValue);
			for (int i = 0; i < unitsWithSpaces.Count; i++)
			{
				IFormationUnit formationUnit2 = unitsWithSpaces[i];
				if (formationUnit2 != null)
				{
					float num2 = ((Agent)formationUnit2).Position.AsVec2.DistanceSquared(position);
					if (num > num2)
					{
						num = num2;
						formationUnit = formationUnit2;
					}
				}
			}
			return formationUnit;
		}

		// Token: 0x06001D6D RID: 7533 RVA: 0x00069C4C File Offset: 0x00067E4C
		private void CopyOrdersFrom(Formation target)
		{
			this.SetMovementOrder(target._movementOrder);
			this.FormOrder = target.FormOrder;
			this.SetPositioning(null, null, new int?(target.UnitSpacing));
			this.RidingOrder = target.RidingOrder;
			this.WeaponUsageOrder = target.WeaponUsageOrder;
			this.FiringOrder = target.FiringOrder;
			this._isAIControlled = target.IsAIControlled || !target.Team.IsPlayerGeneral;
			if (target.AI.Side != FormationAI.BehaviorSide.BehaviorSideNotSet)
			{
				this.AI.Side = target.AI.Side;
			}
			this.SetMovementOrder(target._movementOrder);
			this.FacingOrder = target.FacingOrder;
			this.ArrangementOrder = target.ArrangementOrder;
		}

		// Token: 0x06001D6E RID: 7534 RVA: 0x00069D20 File Offset: 0x00067F20
		private void TickDetachments(float dt)
		{
			if (!this.IsDeployment)
			{
				for (int i = this._detachments.Count - 1; i >= 0; i--)
				{
					IDetachment detachment = this._detachments[i];
					UsableMachine usableMachine = detachment as UsableMachine;
					if (((usableMachine != null) ? usableMachine.Ai : null) != null)
					{
						usableMachine.Ai.Tick(null, this, this.Team, dt);
						if (usableMachine.Ai.HasActionCompleted || (usableMachine.IsDisabledForBattleSideAI(this.Team.Side) && usableMachine.ShouldAutoLeaveDetachmentWhenDisabled(this.Team.Side)))
						{
							this.LeaveDetachment(detachment);
						}
					}
				}
			}
		}

		// Token: 0x06001D6F RID: 7535 RVA: 0x00069DC0 File Offset: 0x00067FC0
		[Conditional("DEBUG")]
		private void TickOrderDebug()
		{
			WorldPosition medianPosition = this.QuerySystem.MedianPosition;
			WorldPosition worldPosition = this.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3);
			medianPosition.SetVec2(this.QuerySystem.AveragePosition);
			if (worldPosition.IsValid)
			{
				if (!this._movementOrder.GetPosition(this).IsValid)
				{
					if (this.AI != null)
					{
						BehaviorComponent activeBehavior = this.AI.ActiveBehavior;
						return;
					}
				}
				else if (this.AI != null)
				{
					BehaviorComponent activeBehavior2 = this.AI.ActiveBehavior;
					return;
				}
			}
			else if (this.AI != null)
			{
				BehaviorComponent activeBehavior3 = this.AI.ActiveBehavior;
			}
		}

		// Token: 0x06001D70 RID: 7536 RVA: 0x00069E50 File Offset: 0x00068050
		[Conditional("DEBUG")]
		private void TickDebug(float dt)
		{
			if (!MBDebug.IsDisplayingHighLevelAI)
			{
				return;
			}
			if (!this.IsSimulationFormation && this._movementOrder.OrderEnum == MovementOrder.MovementOrderEnum.FollowEntity)
			{
				string name = this._movementOrder.TargetEntity.Name;
			}
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x00069E81 File Offset: 0x00068081
		private void OnUnitAttachedOrDetached()
		{
			this.FormOrder = this.FormOrder;
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x00069E8F File Offset: 0x0006808F
		[Conditional("DEBUG")]
		private void AssertDetachments()
		{
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x00069E91 File Offset: 0x00068091
		private void SetOrderPosition(WorldPosition pos)
		{
			this._orderPosition = pos;
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x00069E9A File Offset: 0x0006809A
		private int GetHeroPointForCaptainSelection(Agent agent)
		{
			return agent.Character.Level + 100 * agent.Character.GetSkillValue(DefaultSkills.Charm);
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x00069EBB File Offset: 0x000680BB
		private void OnCaptainChanged()
		{
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				agent.UpdateAgentProperties();
			}, null);
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x00069EE3 File Offset: 0x000680E3
		private void UpdateAgentDrivenPropertiesBasedOnOrderDefensiveness()
		{
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				agent.Defensiveness = (float)this._formationOrderDefensivenessFactor;
			}, null);
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x00069EF8 File Offset: 0x000680F8
		private void ResetAux()
		{
			if (this._detachments != null)
			{
				for (int i = this._detachments.Count - 1; i >= 0; i--)
				{
					this.LeaveDetachment(this._detachments[i]);
				}
			}
			else
			{
				this._detachments = new MBList<IDetachment>();
			}
			this._detachedUnits = new MBList<Agent>();
			this._looseDetachedUnits = new MBList<Agent>();
			this.AttackEntityOrderDetachment = null;
			this.AI = new FormationAI(this);
			this.QuerySystem = new FormationQuerySystem(this);
			this.SetPositioning(null, new Vec2?(Vec2.Forward), new int?(1));
			this.SetMovementOrder(MovementOrder.MovementOrderStop);
			if (this._overridenHasAnyMountedUnit != null)
			{
				bool? overridenHasAnyMountedUnit = this._overridenHasAnyMountedUnit;
				bool flag = true;
				if ((overridenHasAnyMountedUnit.GetValueOrDefault() == flag) & (overridenHasAnyMountedUnit != null))
				{
					this.ArrangementOrder = ArrangementOrder.ArrangementOrderSkein;
					goto IL_EB;
				}
			}
			this.FormOrder = FormOrder.FormOrderWide;
			this.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			IL_EB:
			this.RidingOrder = RidingOrder.RidingOrderFree;
			this.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
			this.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			this.Width = 0f * (this.Interval + this.UnitDiameter) + this.UnitDiameter;
			this.HasBeenPositioned = false;
			this._currentSpawnIndex = 0;
			this.IsPlayerTroopInFormation = false;
			this.HasPlayerControlledTroop = false;
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x0006A04D File Offset: 0x0006824D
		private void ResetForSimulation()
		{
			this.Arrangement.Reset();
			this.ResetAux();
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x0006A060 File Offset: 0x00068260
		private void TryRelocatePlayerUnit()
		{
			if (this.HasPlayerControlledTroop || this.IsPlayerTroopInFormation)
			{
				IFormationUnit playerUnit = this.Arrangement.GetPlayerUnit();
				if (playerUnit != null && playerUnit.FormationFileIndex >= 0 && playerUnit.FormationRankIndex >= 0)
				{
					this.Arrangement.SwitchUnitLocationsWithBackMostUnit(playerUnit);
				}
			}
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x0006A0AC File Offset: 0x000682AC
		private void CalculateFormationClass()
		{
			int[] array = new int[4];
			int num = 0;
			int num2 = 0;
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				Agent agent = formationUnit as Agent;
				if (agent != null)
				{
					int[] array2 = array;
					int defaultFormationGroup = agent.Character.DefaultFormationGroup;
					int num3 = array2[defaultFormationGroup] + 1;
					array2[defaultFormationGroup] = num3;
					if (num3 > num)
					{
						num = array[agent.Character.DefaultFormationGroup];
						num2 = agent.Character.DefaultFormationGroup;
					}
				}
			}
			foreach (Agent agent2 in this._detachedUnits)
			{
				int[] array3 = array;
				int defaultFormationGroup2 = agent2.Character.DefaultFormationGroup;
				int num3 = array3[defaultFormationGroup2] + 1;
				array3[defaultFormationGroup2] = num3;
				if (num3 > num)
				{
					num = array[agent2.Character.DefaultFormationGroup];
					num2 = agent2.Character.DefaultFormationGroup;
				}
			}
			this._initialClass = (FormationClass)num2;
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x0006A1D0 File Offset: 0x000683D0
		private void SmoothAverageUnitPosition(float dt)
		{
			this._smoothedAverageUnitPosition = ((!this._smoothedAverageUnitPosition.IsValid) ? this.QuerySystem.AveragePosition : Vec2.Lerp(this._smoothedAverageUnitPosition, this.QuerySystem.AveragePosition, dt * 3f));
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x0006A20F File Offset: 0x0006840F
		private void Arrangement_OnWidthChanged()
		{
			Action<Formation> onWidthChanged = this.OnWidthChanged;
			if (onWidthChanged == null)
			{
				return;
			}
			onWidthChanged(this);
		}

		// Token: 0x06001D7D RID: 7549 RVA: 0x0006A222 File Offset: 0x00068422
		private void Arrangement_OnShapeChanged()
		{
			this._orderLocalAveragePositionIsDirty = true;
			this._isArrangementShapeChanged = true;
			if (!GameNetwork.IsMultiplayer)
			{
				this.TryRelocatePlayerUnit();
			}
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x0006A240 File Offset: 0x00068440
		public static float GetLastSimulatedFormationsOccupationWidthIfLesserThanActualWidth(Formation simulationFormation)
		{
			float occupationWidth = simulationFormation.Arrangement.GetOccupationWidth(simulationFormation.OverridenUnitCount.GetValueOrDefault());
			if (simulationFormation.Width > occupationWidth)
			{
				return occupationWidth;
			}
			return -1f;
		}

		// Token: 0x06001D7F RID: 7551 RVA: 0x0006A278 File Offset: 0x00068478
		public static List<WorldFrame> GetFormationFramesForBeforeFormationCreation(float width, int manCount, bool areMounted, WorldPosition spawnOrigin, Mat3 spawnRotation)
		{
			List<Formation.AgentArrangementData> list = new List<Formation.AgentArrangementData>();
			Formation formation = new Formation(null, -1);
			formation.SetOrderPosition(spawnOrigin);
			formation._direction = spawnRotation.f.AsVec2;
			LineFormation lineFormation = new LineFormation(formation, true);
			lineFormation.Width = width;
			for (int i = 0; i < manCount; i++)
			{
				list.Add(new Formation.AgentArrangementData(i, lineFormation));
			}
			lineFormation.OnFormationFrameChanged();
			foreach (Formation.AgentArrangementData agentArrangementData in list)
			{
				lineFormation.AddUnit(agentArrangementData);
			}
			List<WorldFrame> list2 = new List<WorldFrame>();
			int cachedOrderedAndAvailableUnitPositionIndicesCount = lineFormation.GetCachedOrderedAndAvailableUnitPositionIndicesCount();
			for (int j = 0; j < cachedOrderedAndAvailableUnitPositionIndicesCount; j++)
			{
				Vec2i cachedOrderedAndAvailableUnitPositionIndexAt = lineFormation.GetCachedOrderedAndAvailableUnitPositionIndexAt(j);
				WorldPosition globalPositionAtIndex = lineFormation.GetGlobalPositionAtIndex(cachedOrderedAndAvailableUnitPositionIndexAt.X, cachedOrderedAndAvailableUnitPositionIndexAt.Y);
				list2.Add(new WorldFrame(spawnRotation, globalPositionAtIndex));
			}
			return list2;
		}

		// Token: 0x06001D80 RID: 7552 RVA: 0x0006A370 File Offset: 0x00068570
		public static float GetDefaultUnitDiameter(bool isMounted)
		{
			if (isMounted)
			{
				return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.QuadrupedalRadius) * 2f;
			}
			return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius) * 2f;
		}

		// Token: 0x06001D81 RID: 7553 RVA: 0x0006A398 File Offset: 0x00068598
		public static float GetDefaultMinimumInterval(bool isMounted)
		{
			if (!isMounted)
			{
				return Formation.InfantryInterval(0);
			}
			return Formation.CavalryInterval(0);
		}

		// Token: 0x06001D82 RID: 7554 RVA: 0x0006A3AA File Offset: 0x000685AA
		public static float GetDefaultMaximumInterval(bool isMounted)
		{
			if (!isMounted)
			{
				return Formation.InfantryInterval(2);
			}
			return Formation.CavalryInterval(2);
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x0006A3BC File Offset: 0x000685BC
		public static float GetDefaultMinimumDistance(bool isMounted)
		{
			if (!isMounted)
			{
				return Formation.InfantryDistance(0);
			}
			return Formation.CavalryDistance(0);
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x0006A3CE File Offset: 0x000685CE
		public static float GetDefaultMaximumDistance(bool isMounted)
		{
			if (!isMounted)
			{
				return Formation.InfantryDistance(2);
			}
			return Formation.CavalryDistance(2);
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x0006A3E0 File Offset: 0x000685E0
		public static float InfantryInterval(int unitSpacing)
		{
			return 0.38f * (float)unitSpacing;
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x0006A3EA File Offset: 0x000685EA
		public static float CavalryInterval(int unitSpacing)
		{
			return 0.18f + 0.32f * (float)unitSpacing;
		}

		// Token: 0x06001D87 RID: 7559 RVA: 0x0006A3FA File Offset: 0x000685FA
		public static float InfantryDistance(int unitSpacing)
		{
			return 0.4f * (float)unitSpacing;
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x0006A404 File Offset: 0x00068604
		public static float CavalryDistance(int unitSpacing)
		{
			return 1.7f + 0.3f * (float)unitSpacing;
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x0006A414 File Offset: 0x00068614
		public static bool IsDefenseRelatedAIDrivenComponent(DrivenProperty drivenProperty)
		{
			return drivenProperty == DrivenProperty.AIDecideOnAttackChance || drivenProperty == DrivenProperty.AIAttackOnDecideChance || drivenProperty == DrivenProperty.AIAttackOnParryChance || drivenProperty == DrivenProperty.AiUseShieldAgainstEnemyMissileProbability || drivenProperty == DrivenProperty.AiDefendWithShieldDecisionChanceValue;
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x0006A430 File Offset: 0x00068630
		private static void GetUnitPositionWithIndexAccordingToNewOrder(Formation simulationFormation, int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, IFormationArrangement arrangement, float width, int unitSpacing, int unitCount, bool isMounted, int index, out WorldPosition? unitPosition, out Vec2? unitDirection, out float actualWidth)
		{
			unitPosition = null;
			unitDirection = null;
			if (simulationFormation == null)
			{
				if (Formation._simulationFormationTemp == null || Formation._simulationFormationUniqueIdentifier != index)
				{
					Formation._simulationFormationTemp = new Formation(null, -1);
				}
				simulationFormation = Formation._simulationFormationTemp;
			}
			if (simulationFormation.UnitSpacing == unitSpacing && MathF.Abs(simulationFormation.Width - width + 1E-05f) < simulationFormation.Interval + simulationFormation.UnitDiameter - 1E-05f && simulationFormation.OrderPositionIsValid)
			{
				Vec3 orderGroundPosition = simulationFormation.OrderGroundPosition;
				WorldPosition worldPosition = formationPosition;
				if (orderGroundPosition.NearlyEquals(worldPosition.GetGroundVec3(), 0.1f) && simulationFormation.Direction.NearlyEquals(formationDirection, 0.1f) && !(simulationFormation.Arrangement.GetType() != arrangement.GetType()))
				{
					goto IL_15E;
				}
			}
			simulationFormation._overridenHasAnyMountedUnit = new bool?(isMounted);
			simulationFormation.ResetForSimulation();
			simulationFormation.SetPositioning(null, null, new int?(unitSpacing));
			simulationFormation.OverridenUnitCount = new int?(unitCount);
			simulationFormation.SetPositioning(new WorldPosition?(formationPosition), new Vec2?(formationDirection), null);
			simulationFormation.Rearrange(arrangement.Clone(simulationFormation));
			simulationFormation.Arrangement.DeepCopyFrom(arrangement);
			simulationFormation.Width = width;
			Formation._simulationFormationUniqueIdentifier = index;
			IL_15E:
			actualWidth = simulationFormation.Width;
			if (width >= actualWidth)
			{
				Vec2? vec = simulationFormation.Arrangement.GetLocalPositionOfUnitOrDefault(unitIndex);
				if (vec == null)
				{
					vec = simulationFormation.Arrangement.CreateNewPosition(unitIndex);
				}
				if (vec != null)
				{
					Vec2 vec2 = simulationFormation.Direction.TransformToParentUnitF(vec.Value);
					WorldPosition worldPosition2 = simulationFormation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
					worldPosition2.SetVec2(worldPosition2.AsVec2 + vec2);
					unitPosition = new WorldPosition?(worldPosition2);
					unitDirection = new Vec2?(formationDirection);
				}
			}
		}

		// Token: 0x06001D8B RID: 7563 RVA: 0x0006A631 File Offset: 0x00068831
		private static IEnumerable<ValueTuple<WorldPosition, Vec2>> GetUnavailableUnitPositionsAccordingToNewOrder(Formation formation, Formation simulationFormation, WorldPosition position, Vec2 direction, IFormationArrangement arrangement, float width, int unitSpacing)
		{
			if (simulationFormation == null)
			{
				if (Formation._simulationFormationTemp == null || Formation._simulationFormationUniqueIdentifier != formation.Index)
				{
					Formation._simulationFormationTemp = new Formation(null, -1);
				}
				simulationFormation = Formation._simulationFormationTemp;
			}
			if (simulationFormation.UnitSpacing != unitSpacing || MathF.Abs(simulationFormation.Width - width) >= simulationFormation.Interval + simulationFormation.UnitDiameter || !simulationFormation.OrderPositionIsValid || !simulationFormation.OrderGroundPosition.NearlyEquals(position.GetGroundVec3(), 0.1f) || !simulationFormation.Direction.NearlyEquals(direction, 0.1f) || simulationFormation.Arrangement.GetType() != arrangement.GetType())
			{
				simulationFormation._overridenHasAnyMountedUnit = new bool?(formation.HasAnyMountedUnit);
				simulationFormation.ResetForSimulation();
				simulationFormation.SetPositioning(null, null, new int?(unitSpacing));
				simulationFormation.OverridenUnitCount = new int?(formation.CountOfUnitsWithoutDetachedOnes);
				simulationFormation.SetPositioning(new WorldPosition?(position), new Vec2?(direction), null);
				simulationFormation.Rearrange(arrangement.Clone(simulationFormation));
				simulationFormation.Arrangement.DeepCopyFrom(arrangement);
				simulationFormation.Width = width;
				Formation._simulationFormationUniqueIdentifier = formation.Index;
			}
			IEnumerable<Vec2> unavailableUnitPositions = simulationFormation.Arrangement.GetUnavailableUnitPositions();
			foreach (Vec2 vec in unavailableUnitPositions)
			{
				Vec2 vec2 = simulationFormation.Direction.TransformToParentUnitF(vec);
				WorldPosition worldPosition = simulationFormation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
				worldPosition.SetVec2(worldPosition.AsVec2 + vec2);
				yield return new ValueTuple<WorldPosition, Vec2>(worldPosition, direction);
			}
			IEnumerator<Vec2> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001D8C RID: 7564 RVA: 0x0006A66E File Offset: 0x0006886E
		private static float TransformCustomWidthBetweenArrangementOrientations(ArrangementOrder.ArrangementOrderEnum orderTypeOld, ArrangementOrder.ArrangementOrderEnum orderTypeNew, float currentCustomWidth)
		{
			if (orderTypeOld != ArrangementOrder.ArrangementOrderEnum.Column && orderTypeNew == ArrangementOrder.ArrangementOrderEnum.Column)
			{
				return currentCustomWidth * 0.1f;
			}
			if (orderTypeOld == ArrangementOrder.ArrangementOrderEnum.Column && orderTypeNew != ArrangementOrder.ArrangementOrderEnum.Column)
			{
				return currentCustomWidth / 0.1f;
			}
			return currentCustomWidth;
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x0006A691 File Offset: 0x00068891
		public override int GetHashCode()
		{
			return (int)(this.Team.TeamIndex * 10 + this.FormationIndex);
		}

		// Token: 0x04000979 RID: 2425
		public const float AveragePositionCalculatePeriod = 0.05f;

		// Token: 0x0400097A RID: 2426
		public const int MinimumUnitSpacing = 0;

		// Token: 0x0400097B RID: 2427
		public const int MaximumUnitSpacing = 2;

		// Token: 0x0400097C RID: 2428
		private static Formation _simulationFormationTemp;

		// Token: 0x0400097D RID: 2429
		private static int _simulationFormationUniqueIdentifier;

		// Token: 0x04000986 RID: 2438
		public readonly Team Team;

		// Token: 0x04000987 RID: 2439
		public readonly int Index;

		// Token: 0x04000988 RID: 2440
		public readonly FormationClass FormationIndex;

		// Token: 0x04000989 RID: 2441
		public Banner Banner;

		// Token: 0x0400098A RID: 2442
		public bool HasBeenPositioned;

		// Token: 0x0400098B RID: 2443
		public Vec2? ReferencePosition;

		// Token: 0x0400098C RID: 2444
		private FormationClass _initialClass = FormationClass.NumberOfAllFormations;

		// Token: 0x0400098D RID: 2445
		private bool _formationClassNeedsUpdate;

		// Token: 0x0400098E RID: 2446
		private Agent _playerOwner;

		// Token: 0x0400098F RID: 2447
		private string _bannerCode;

		// Token: 0x04000990 RID: 2448
		private bool _isAIControlled = true;

		// Token: 0x04000991 RID: 2449
		private bool _enforceNotSplittableByAI = true;

		// Token: 0x04000992 RID: 2450
		private WorldPosition _orderPosition;

		// Token: 0x04000993 RID: 2451
		private Vec2 _direction;

		// Token: 0x04000994 RID: 2452
		private int _unitSpacing;

		// Token: 0x04000995 RID: 2453
		private Vec2 _orderLocalAveragePosition;

		// Token: 0x04000996 RID: 2454
		private bool _orderLocalAveragePositionIsDirty = true;

		// Token: 0x04000997 RID: 2455
		private int _formationOrderDefensivenessFactor = 2;

		// Token: 0x04000998 RID: 2456
		private MovementOrder _movementOrder;

		// Token: 0x04000999 RID: 2457
		private FacingOrder _facingOrder;

		// Token: 0x0400099A RID: 2458
		private ArrangementOrder _arrangementOrder;

		// Token: 0x0400099B RID: 2459
		private Timer _arrangementOrderTickOccasionallyTimer;

		// Token: 0x0400099C RID: 2460
		private FormOrder _formOrder;

		// Token: 0x0400099D RID: 2461
		private RidingOrder _ridingOrder;

		// Token: 0x0400099E RID: 2462
		private WeaponUsageOrder _weaponUsageOrder;

		// Token: 0x0400099F RID: 2463
		private Agent _captain;

		// Token: 0x040009A0 RID: 2464
		private Vec2 _smoothedAverageUnitPosition = Vec2.Invalid;

		// Token: 0x040009A1 RID: 2465
		private MBList<IDetachment> _detachments;

		// Token: 0x040009A2 RID: 2466
		private IFormationArrangement _arrangement;

		// Token: 0x040009A3 RID: 2467
		private int[] _agentIndicesCache;

		// Token: 0x040009A4 RID: 2468
		private MBList<Agent> _detachedUnits;

		// Token: 0x040009A5 RID: 2469
		private int _undetachableNonPlayerUnitCount;

		// Token: 0x040009A6 RID: 2470
		private MBList<Agent> _looseDetachedUnits;

		// Token: 0x040009A7 RID: 2471
		private bool? _overridenHasAnyMountedUnit;

		// Token: 0x040009A8 RID: 2472
		private bool _isArrangementShapeChanged;

		// Token: 0x040009A9 RID: 2473
		private int _currentSpawnIndex;

		// Token: 0x0200052B RID: 1323
		private class AgentArrangementData : IFormationUnit
		{
			// Token: 0x17000971 RID: 2417
			// (get) Token: 0x0600399A RID: 14746 RVA: 0x000E8D2F File Offset: 0x000E6F2F
			// (set) Token: 0x0600399B RID: 14747 RVA: 0x000E8D37 File Offset: 0x000E6F37
			public IFormationArrangement Formation { get; private set; }

			// Token: 0x17000972 RID: 2418
			// (get) Token: 0x0600399C RID: 14748 RVA: 0x000E8D40 File Offset: 0x000E6F40
			// (set) Token: 0x0600399D RID: 14749 RVA: 0x000E8D48 File Offset: 0x000E6F48
			public int FormationFileIndex { get; set; } = -1;

			// Token: 0x17000973 RID: 2419
			// (get) Token: 0x0600399E RID: 14750 RVA: 0x000E8D51 File Offset: 0x000E6F51
			// (set) Token: 0x0600399F RID: 14751 RVA: 0x000E8D59 File Offset: 0x000E6F59
			public int FormationRankIndex { get; set; } = -1;

			// Token: 0x17000974 RID: 2420
			// (get) Token: 0x060039A0 RID: 14752 RVA: 0x000E8D62 File Offset: 0x000E6F62
			public IFormationUnit FollowedUnit { get; }

			// Token: 0x17000975 RID: 2421
			// (get) Token: 0x060039A1 RID: 14753 RVA: 0x000E8D6A File Offset: 0x000E6F6A
			public bool IsShieldUsageEncouraged
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000976 RID: 2422
			// (get) Token: 0x060039A2 RID: 14754 RVA: 0x000E8D6D File Offset: 0x000E6F6D
			public bool IsPlayerUnit
			{
				get
				{
					return false;
				}
			}

			// Token: 0x060039A3 RID: 14755 RVA: 0x000E8D70 File Offset: 0x000E6F70
			public AgentArrangementData(int index, IFormationArrangement arrangement)
			{
				this.Formation = arrangement;
			}
		}
	}
}
