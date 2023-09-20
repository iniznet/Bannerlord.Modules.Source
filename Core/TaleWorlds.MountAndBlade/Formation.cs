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
	public sealed class Formation : IFormation
	{
		public event Action<Formation, Agent> OnUnitAdded;

		public event Action<Formation, Agent> OnUnitRemoved;

		public event Action<Formation> OnUnitCountChanged;

		public event Action<Formation> OnUnitSpacingChanged;

		public event Action<Formation> OnTick;

		public event Action<Formation> OnWidthChanged;

		public event Action<Formation, MovementOrder.MovementOrderEnum> OnBeforeMovementOrderApplied;

		public event Action<Formation, ArrangementOrder.ArrangementOrderEnum> OnAfterArrangementOrderApplied;

		public FormationClass PrimaryClass
		{
			get
			{
				return this.QuerySystem.MainClass;
			}
		}

		public int CountOfUnits
		{
			get
			{
				return this.Arrangement.UnitCount + this._detachedUnits.Count;
			}
		}

		public int CountOfDetachedUnits
		{
			get
			{
				return this._detachedUnits.Count;
			}
		}

		public int CountOfUndetachableNonPlayerUnits
		{
			get
			{
				return this._undetachableNonPlayerUnitCount;
			}
		}

		public int CountOfUnitsWithoutDetachedOnes
		{
			get
			{
				return this.Arrangement.UnitCount + this._looseDetachedUnits.Count;
			}
		}

		public MBList<IFormationUnit> UnitsWithoutLooseDetachedOnes
		{
			get
			{
				return this.Arrangement.GetAllUnits();
			}
		}

		public int CountOfUnitsWithoutLooseDetachedOnes
		{
			get
			{
				return this.Arrangement.UnitCount;
			}
		}

		public int CountOfDetachableNonplayerUnits
		{
			get
			{
				return this.Arrangement.UnitCount - ((this.IsPlayerTroopInFormation || this.HasPlayerControlledTroop) ? 1 : 0) - this.CountOfUndetachableNonPlayerUnits;
			}
		}

		public Vec2 OrderPosition
		{
			get
			{
				return this._orderPosition.AsVec2;
			}
		}

		public Vec3 OrderGroundPosition
		{
			get
			{
				return this._orderPosition.GetGroundVec3();
			}
		}

		public bool OrderPositionIsValid
		{
			get
			{
				return this._orderPosition.IsValid;
			}
		}

		public float Depth
		{
			get
			{
				return this.Arrangement.Depth;
			}
		}

		public float MinimumWidth
		{
			get
			{
				return this.Arrangement.MinimumWidth;
			}
		}

		public float MaximumWidth
		{
			get
			{
				return this.Arrangement.MaximumWidth;
			}
		}

		public float UnitDiameter
		{
			get
			{
				return Formation.GetDefaultUnitDiameter(this.CalculateHasSignificantNumberOfMounted && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		public Vec2 Direction
		{
			get
			{
				return this._direction;
			}
		}

		public Vec2 CurrentDirection
		{
			get
			{
				return (this.QuerySystem.EstimatedDirection * 0.8f + this.Direction * 0.2f).Normalized();
			}
		}

		public Vec2 SmoothedAverageUnitPosition
		{
			get
			{
				return this._smoothedAverageUnitPosition;
			}
		}

		public int UnitSpacing
		{
			get
			{
				return this._unitSpacing;
			}
		}

		public MBReadOnlyList<Agent> LooseDetachedUnits
		{
			get
			{
				return this._looseDetachedUnits;
			}
		}

		public MBReadOnlyList<Agent> DetachedUnits
		{
			get
			{
				return this._detachedUnits;
			}
		}

		public AttackEntityOrderDetachment AttackEntityOrderDetachment { get; private set; }

		public FormationAI AI { get; private set; }

		public Formation TargetFormation { get; set; }

		public FormationQuerySystem QuerySystem { get; private set; }

		public MBReadOnlyList<IDetachment> Detachments
		{
			get
			{
				return this._detachments;
			}
		}

		public int? OverridenUnitCount { get; private set; }

		public bool IsSpawning { get; private set; }

		public bool IsAITickedAfterSplit { get; set; }

		public bool HasPlayerControlledTroop { get; private set; }

		public bool IsPlayerTroopInFormation { get; private set; }

		public bool ContainsAgentVisuals { get; set; }

		public FiringOrder FiringOrder { get; set; }

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

		public bool IsSplittableByAI
		{
			get
			{
				return this.IsAIOwned && this.IsConvenientForTransfer;
			}
		}

		public bool IsAIOwned
		{
			get
			{
				return !this._enforceNotSplittableByAI && (this.IsAIControlled || (!this.Team.IsPlayerGeneral && (!this.Team.IsPlayerSergeant || this.PlayerOwner != Agent.Main)));
			}
		}

		public bool IsConvenientForTransfer
		{
			get
			{
				return Mission.Current.MissionTeamAIType != Mission.MissionTeamAITypeEnum.Siege || this.Team.Side != BattleSideEnum.Attacker || this.QuerySystem.InsideCastleUnitCountIncludingUnpositioned == 0;
			}
		}

		public bool EnforceNotSplittableByAI
		{
			get
			{
				return this._enforceNotSplittableByAI;
			}
		}

		public bool IsAIControlled
		{
			get
			{
				return this._isAIControlled;
			}
		}

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

		private bool IsSimulationFormation
		{
			get
			{
				return this.Team == null;
			}
		}

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

		public bool IsDeployment
		{
			get
			{
				return Mission.Current.GetMissionBehavior<BattleDeploymentHandler>() != null;
			}
		}

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

		public Vec2 CurrentPosition
		{
			get
			{
				return this.QuerySystem.GetAveragePositionWithMaxAge(0.1f) + this.CurrentDirection.TransformToParentUnitF(-this.OrderLocalAveragePosition);
			}
		}

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

		public float MinimumDistance
		{
			get
			{
				return Formation.GetDefaultMinimumDistance(this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		public bool IsLoose
		{
			get
			{
				return ArrangementOrder.GetUnitLooseness(this.ArrangementOrder.OrderEnum);
			}
		}

		public float MinimumInterval
		{
			get
			{
				return Formation.GetDefaultMinimumInterval(this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		public float MaximumInterval
		{
			get
			{
				return Formation.GetDefaultMaximumInterval(this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		public float MaximumDistance
		{
			get
			{
				return Formation.GetDefaultMaximumDistance(this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount));
			}
		}

		internal bool PostponeCostlyOperations { get; private set; }

		public Formation(Team team, int index)
		{
			this.Team = team;
			this.Index = index;
			this.FormationIndex = (FormationClass)index;
			this.IsSpawning = false;
			this.Reset();
		}

		~Formation()
		{
			if (!this.IsSimulationFormation)
			{
				Formation._simulationFormationTemp = null;
			}
		}

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

		IFormationUnit IFormation.GetClosestUnitTo(Vec2 localPosition, MBList<IFormationUnit> unitsWithSpaces, float? maxDistance)
		{
			Vec2 vec = this.Direction.TransformToParentUnitF(localPosition);
			Vec2 vec2 = this.OrderPosition + vec;
			return this.GetClosestUnitToAux(vec2, unitsWithSpaces, maxDistance);
		}

		IFormationUnit IFormation.GetClosestUnitTo(IFormationUnit targetUnit, MBList<IFormationUnit> unitsWithSpaces, float? maxDistance)
		{
			return this.GetClosestUnitToAux(((Agent)targetUnit).Position.AsVec2, unitsWithSpaces, maxDistance);
		}

		void IFormation.SetUnitToFollow(IFormationUnit unit, IFormationUnit toFollow, Vec2 vector)
		{
			Agent agent = unit as Agent;
			Agent agent2 = toFollow as Agent;
			agent.SetColumnwiseFollowAgent(agent2, ref vector);
		}

		bool IFormation.BatchUnitPositions(MBArrayList<Vec2i> orderedPositionIndices, MBArrayList<Vec2> orderedLocalPositions, MBList2D<int> availabilityTable, MBList2D<WorldPosition> globalPositionTable, int fileCount, int rankCount)
		{
			if (this._orderPosition.IsValid && this._orderPosition.GetNavMesh() != UIntPtr.Zero)
			{
				Mission.Current.BatchFormationUnitPositions(orderedPositionIndices, orderedLocalPositions, availabilityTable, globalPositionTable, this._orderPosition, this.Direction, fileCount, rankCount);
				return true;
			}
			return false;
		}

		public WorldPosition CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache)
		{
			if (!this.OrderPositionIsValid)
			{
				Debug.Print(string.Concat(new object[]
				{
					"Formation order position is not valid. Team: ",
					this.Team.TeamIndex,
					", Formation: ",
					(int)this.FormationIndex
				}), 0, Debug.DebugColor.Yellow, 17592186044416UL);
			}
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

		public void ResetArrangementOrderTickTimer()
		{
			this._arrangementOrderTickOccasionallyTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
		}

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

		public ref readonly MovementOrder GetReadonlyMovementOrderReference()
		{
			return ref this._movementOrder;
		}

		public Agent GetFirstUnit()
		{
			return this.GetUnitWithIndex(0);
		}

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

		public void SetSpawnIndex(int value = 0)
		{
			this._currentSpawnIndex = value;
		}

		public int GetNextSpawnIndex()
		{
			int currentSpawnIndex = this._currentSpawnIndex;
			this._currentSpawnIndex++;
			return currentSpawnIndex;
		}

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

		private WorldPosition GetOrderPositionOfUnitAux(Agent unit)
		{
			WorldPosition? worldPositionOfUnitOrDefault = this.Arrangement.GetWorldPositionOfUnitOrDefault(unit);
			if (worldPositionOfUnitOrDefault != null)
			{
				return worldPositionOfUnitOrDefault.Value;
			}
			if (!this.OrderPositionIsValid)
			{
				WorldPosition worldPosition = unit.GetWorldPosition();
				Debug.Print(string.Concat(new object[]
				{
					"Formation order position is not valid. Team: ",
					this.Team.TeamIndex,
					", Formation: ",
					(int)this.FormationIndex,
					"Unit Pos: ",
					worldPosition.GetGroundVec3(),
					"Mission Mode: ",
					Mission.Current.Mode.ToString()
				}), 0, Debug.DebugColor.Yellow, 17592186044416UL);
			}
			WorldPosition worldPosition2 = this._movementOrder.CreateNewOrderWorldPosition(this, WorldPosition.WorldPositionEnforcedCache.NavMeshVec3);
			if (worldPosition2.GetNavMesh() == UIntPtr.Zero || !Mission.Current.IsPositionInsideBoundaries(worldPosition2.AsVec2))
			{
				return unit.GetWorldPosition();
			}
			return worldPosition2;
		}

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
					if (!this.OrderPositionIsValid)
					{
						WorldPosition worldPosition = unit.GetWorldPosition();
						Debug.Print(string.Concat(new object[]
						{
							"Formation order position is not valid. Team: ",
							this.Team.TeamIndex,
							", Formation: ",
							(int)this.FormationIndex,
							"Unit Pos: ",
							worldPosition.GetGroundVec3()
						}), 0, Debug.DebugColor.Yellow, 17592186044416UL);
					}
					return this._movementOrder.CreateNewOrderWorldPosition(this, WorldPosition.WorldPositionEnforcedCache.None);
				case MovementOrder.MovementStateEnum.Hold:
					return this.GetOrderPositionOfUnitAux(unit);
				case MovementOrder.MovementStateEnum.Retreat:
					return WorldPosition.Invalid;
				case MovementOrder.MovementStateEnum.StandGround:
					return unit.GetWorldPosition();
				default:
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Formation.cs", "GetOrderPositionOfUnit", 1438);
					return WorldPosition.Invalid;
				}
			}
		}

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

		public float GetFormationPower()
		{
			float sum = 0f;
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				sum += agent.CharacterPowerCached;
			}, null);
			return sum;
		}

		public float GetFormationMeleeFightingPower()
		{
			float sum = 0f;
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				sum += agent.CharacterPowerCached * ((this.FormationIndex == FormationClass.Ranged || this.FormationIndex == FormationClass.HorseArcher) ? 0.4f : 1f);
			}, null);
			return sum;
		}

		internal IDetachment GetDetachmentForDebug(Agent agent)
		{
			return this.Detachments.FirstOrDefault((IDetachment d) => d.IsAgentUsingOrInterested(agent));
		}

		public IDetachment GetDetachmentOrDefault(Agent agent)
		{
			return agent.Detachment;
		}

		public WorldFrame? GetDetachmentFrame(Agent agent)
		{
			return agent.Detachment.GetAgentFrame(agent);
		}

		public Vec2 GetMiddleFrontUnitPositionOffset()
		{
			Vec2 localPositionOfReservedUnitPosition = this.Arrangement.GetLocalPositionOfReservedUnitPosition();
			return this.Direction.TransformToParentUnitF(localPositionOfReservedUnitPosition);
		}

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

		public IEnumerable<ValueTuple<WorldPosition, Vec2>> GetUnavailableUnitPositionsAccordingToNewOrder(Formation simulationFormation, in WorldPosition position, in Vec2 direction, float width, int unitSpacing)
		{
			return Formation.GetUnavailableUnitPositionsAccordingToNewOrder(this, simulationFormation, position, direction, this.Arrangement, width, unitSpacing);
		}

		public void GetUnitSpawnFrameWithIndex(int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, float width, int unitCount, int unitSpacing, bool isMountedFormation, out WorldPosition? unitSpawnPosition, out Vec2? unitSpawnDirection)
		{
			float num;
			Formation.GetUnitPositionWithIndexAccordingToNewOrder(null, unitIndex, formationPosition, formationDirection, this.Arrangement, width, unitSpacing, unitCount, isMountedFormation, this.Index, out unitSpawnPosition, out unitSpawnDirection, out num);
		}

		public void GetUnitPositionWithIndexAccordingToNewOrder(Formation simulationFormation, int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, float width, int unitSpacing, out WorldPosition? unitSpawnPosition, out Vec2? unitSpawnDirection)
		{
			float num;
			Formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, formationPosition, formationDirection, this.Arrangement, width, unitSpacing, this.Arrangement.UnitCount, this.HasAnyMountedUnit, this.Index, out unitSpawnPosition, out unitSpawnDirection, out num);
		}

		public void GetUnitPositionWithIndexAccordingToNewOrder(Formation simulationFormation, int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, float width, int unitSpacing, int overridenUnitCount, out WorldPosition? unitPosition, out Vec2? unitDirection)
		{
			float num;
			Formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, formationPosition, formationDirection, this.Arrangement, width, unitSpacing, overridenUnitCount, this.HasAnyMountedUnit, this.Index, out unitPosition, out unitDirection, out num);
		}

		public void GetUnitPositionWithIndexAccordingToNewOrder(Formation simulationFormation, int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, float width, int unitSpacing, out WorldPosition? unitSpawnPosition, out Vec2? unitSpawnDirection, out float actualWidth)
		{
			Formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, formationPosition, formationDirection, this.Arrangement, width, unitSpacing, this.Arrangement.UnitCount, this.HasAnyMountedUnit, this.Index, out unitSpawnPosition, out unitSpawnDirection, out actualWidth);
		}

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

		public void ApplyActionOnEachAttachedUnit(Action<Agent> action)
		{
			foreach (IFormationUnit formationUnit in this.Arrangement.GetAllUnits())
			{
				Agent agent = (Agent)formationUnit;
				action(agent);
			}
		}

		public void ApplyActionOnEachDetachedUnit(Action<Agent> action)
		{
			for (int i = 0; i < this._detachedUnits.Count; i++)
			{
				action(this._detachedUnits[i]);
			}
		}

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

		public void OnMassUnitTransferStart()
		{
			this.PostponeCostlyOperations = true;
		}

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

		public void OnBatchUnitRemovalStart()
		{
			this.PostponeCostlyOperations = true;
			this.Arrangement.OnBatchRemoveStart();
		}

		public void OnBatchUnitRemovalEnd()
		{
			this.Arrangement.OnBatchRemoveEnd();
			this.FormOrder = this.FormOrder;
			this.QuerySystem.ExpireAfterUnitAddRemove();
			this.Team.QuerySystem.ExpireAfterUnitAddRemove();
			this.PostponeCostlyOperations = false;
		}

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

		public void OnAgentLostMount(Agent agent)
		{
			if (!agent.IsDetachedFromFormation)
			{
				this._arrangement.OnUnitLostMount(agent);
			}
		}

		public void OnFormationDispersed()
		{
			this.Arrangement.OnFormationDispersed();
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				agent.UpdateCachedAndFormationValues(true, false);
			}, null);
		}

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

		public void OnUndetachableNonPlayerUnitAdded(Agent unit)
		{
			if (unit.Formation == this && !unit.IsPlayerControlled)
			{
				this._undetachableNonPlayerUnitCount++;
			}
		}

		public void OnUndetachableNonPlayerUnitRemoved(Agent unit)
		{
			if (unit.Formation == this && !unit.IsPlayerControlled)
			{
				this._undetachableNonPlayerUnitCount--;
			}
		}

		public void ReleaseFormationFromAI()
		{
			this._isAIControlled = false;
		}

		public void ResetMovementOrderPositionCache()
		{
			this._movementOrder.ResetPositionCache();
		}

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

		public void AttachUnit(Agent unit)
		{
			this._detachedUnits.Remove(unit);
			this._looseDetachedUnits.Remove(unit);
			this.Arrangement.AddUnit(unit);
			unit.Detachment = null;
			unit.DetachmentWeight = -1f;
			this.OnUnitAttachedOrDetached();
		}

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

		public void JoinDetachment(IDetachment detachment)
		{
			if (!this.Team.DetachmentManager.ContainsDetachment(detachment))
			{
				this.Team.DetachmentManager.MakeDetachment(detachment);
			}
			this._detachments.Add(detachment);
			this.Team.DetachmentManager.OnFormationJoinDetachment(this, detachment);
		}

		public void FormAttackEntityDetachment(GameEntity targetEntity)
		{
			this.AttackEntityOrderDetachment = new AttackEntityOrderDetachment(targetEntity);
			this.JoinDetachment(this.AttackEntityOrderDetachment);
		}

		public void LeaveDetachment(IDetachment detachment)
		{
			detachment.OnFormationLeave(this);
			this._detachments.Remove(detachment);
			this.Team.DetachmentManager.OnFormationLeaveDetachment(this, detachment);
		}

		public void DisbandAttackEntityDetachment()
		{
			if (this.AttackEntityOrderDetachment != null)
			{
				this.Team.DetachmentManager.DestroyDetachment(this.AttackEntityOrderDetachment);
				this.AttackEntityOrderDetachment = null;
			}
		}

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

		public void TickForColumnArrangementInitialPositioning(Formation formation)
		{
			if ((this.ReferencePosition.Value - this.OrderPosition).LengthSquared >= 1f && !this.IsDeployment)
			{
				this.ArrangementOrder.RearrangeAux(this, true);
			}
		}

		public float CalculateFormationDirectionEnforcingFactorForRank(int rankIndex)
		{
			if (rankIndex == -1)
			{
				return 0f;
			}
			return this.ArrangementOrder.CalculateFormationDirectionEnforcingFactorForRank(rankIndex, this.Arrangement.RankCount);
		}

		public void BeginSpawn(int unitCount, bool isMounted)
		{
			this.IsSpawning = true;
			this.OverridenUnitCount = new int?(unitCount);
			this._overridenHasAnyMountedUnit = new bool?(isMounted);
		}

		public void EndSpawn()
		{
			this.IsSpawning = false;
			this.OverridenUnitCount = null;
			this._overridenHasAnyMountedUnit = null;
		}

		internal bool IsUnitDetachedForDebug(Agent unit)
		{
			return this._detachedUnits.Contains(unit);
		}

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

		private void OnUnitAttachedOrDetached()
		{
			this.FormOrder = this.FormOrder;
		}

		[Conditional("DEBUG")]
		private void AssertDetachments()
		{
		}

		private void SetOrderPosition(WorldPosition pos)
		{
			this._orderPosition = pos;
		}

		private int GetHeroPointForCaptainSelection(Agent agent)
		{
			return agent.Character.Level + 100 * agent.Character.GetSkillValue(DefaultSkills.Charm);
		}

		private void OnCaptainChanged()
		{
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				agent.UpdateAgentProperties();
			}, null);
		}

		private void UpdateAgentDrivenPropertiesBasedOnOrderDefensiveness()
		{
			this.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				agent.Defensiveness = (float)this._formationOrderDefensivenessFactor;
			}, null);
		}

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

		private void ResetForSimulation()
		{
			this.Arrangement.Reset();
			this.ResetAux();
		}

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

		private void SmoothAverageUnitPosition(float dt)
		{
			this._smoothedAverageUnitPosition = ((!this._smoothedAverageUnitPosition.IsValid) ? this.QuerySystem.AveragePosition : Vec2.Lerp(this._smoothedAverageUnitPosition, this.QuerySystem.AveragePosition, dt * 3f));
		}

		private void Arrangement_OnWidthChanged()
		{
			Action<Formation> onWidthChanged = this.OnWidthChanged;
			if (onWidthChanged == null)
			{
				return;
			}
			onWidthChanged(this);
		}

		private void Arrangement_OnShapeChanged()
		{
			this._orderLocalAveragePositionIsDirty = true;
			this._isArrangementShapeChanged = true;
			if (!GameNetwork.IsMultiplayer)
			{
				this.TryRelocatePlayerUnit();
			}
		}

		public static float GetLastSimulatedFormationsOccupationWidthIfLesserThanActualWidth(Formation simulationFormation)
		{
			float occupationWidth = simulationFormation.Arrangement.GetOccupationWidth(simulationFormation.OverridenUnitCount.GetValueOrDefault());
			if (simulationFormation.Width > occupationWidth)
			{
				return occupationWidth;
			}
			return -1f;
		}

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

		public static float GetDefaultUnitDiameter(bool isMounted)
		{
			if (isMounted)
			{
				return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.QuadrupedalRadius) * 2f;
			}
			return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius) * 2f;
		}

		public static float GetDefaultMinimumInterval(bool isMounted)
		{
			if (!isMounted)
			{
				return Formation.InfantryInterval(0);
			}
			return Formation.CavalryInterval(0);
		}

		public static float GetDefaultMaximumInterval(bool isMounted)
		{
			if (!isMounted)
			{
				return Formation.InfantryInterval(2);
			}
			return Formation.CavalryInterval(2);
		}

		public static float GetDefaultMinimumDistance(bool isMounted)
		{
			if (!isMounted)
			{
				return Formation.InfantryDistance(0);
			}
			return Formation.CavalryDistance(0);
		}

		public static float GetDefaultMaximumDistance(bool isMounted)
		{
			if (!isMounted)
			{
				return Formation.InfantryDistance(2);
			}
			return Formation.CavalryDistance(2);
		}

		public static float InfantryInterval(int unitSpacing)
		{
			return 0.38f * (float)unitSpacing;
		}

		public static float CavalryInterval(int unitSpacing)
		{
			return 0.18f + 0.32f * (float)unitSpacing;
		}

		public static float InfantryDistance(int unitSpacing)
		{
			return 0.4f * (float)unitSpacing;
		}

		public static float CavalryDistance(int unitSpacing)
		{
			return 1.7f + 0.3f * (float)unitSpacing;
		}

		public static bool IsDefenseRelatedAIDrivenComponent(DrivenProperty drivenProperty)
		{
			return drivenProperty == DrivenProperty.AIDecideOnAttackChance || drivenProperty == DrivenProperty.AIAttackOnDecideChance || drivenProperty == DrivenProperty.AIAttackOnParryChance || drivenProperty == DrivenProperty.AiUseShieldAgainstEnemyMissileProbability || drivenProperty == DrivenProperty.AiDefendWithShieldDecisionChanceValue;
		}

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

		public override int GetHashCode()
		{
			return (int)(this.Team.TeamIndex * 10 + this.FormationIndex);
		}

		public const float AveragePositionCalculatePeriod = 0.05f;

		public const int MinimumUnitSpacing = 0;

		public const int MaximumUnitSpacing = 2;

		private static Formation _simulationFormationTemp;

		private static int _simulationFormationUniqueIdentifier;

		public readonly Team Team;

		public readonly int Index;

		public readonly FormationClass FormationIndex;

		public Banner Banner;

		public bool HasBeenPositioned;

		public Vec2? ReferencePosition;

		private FormationClass _initialClass = FormationClass.NumberOfAllFormations;

		private bool _formationClassNeedsUpdate;

		private Agent _playerOwner;

		private string _bannerCode;

		private bool _isAIControlled = true;

		private bool _enforceNotSplittableByAI = true;

		private WorldPosition _orderPosition;

		private Vec2 _direction;

		private int _unitSpacing;

		private Vec2 _orderLocalAveragePosition;

		private bool _orderLocalAveragePositionIsDirty = true;

		private int _formationOrderDefensivenessFactor = 2;

		private MovementOrder _movementOrder;

		private FacingOrder _facingOrder;

		private ArrangementOrder _arrangementOrder;

		private Timer _arrangementOrderTickOccasionallyTimer;

		private FormOrder _formOrder;

		private RidingOrder _ridingOrder;

		private WeaponUsageOrder _weaponUsageOrder;

		private Agent _captain;

		private Vec2 _smoothedAverageUnitPosition = Vec2.Invalid;

		private MBList<IDetachment> _detachments;

		private IFormationArrangement _arrangement;

		private int[] _agentIndicesCache;

		private MBList<Agent> _detachedUnits;

		private int _undetachableNonPlayerUnitCount;

		private MBList<Agent> _looseDetachedUnits;

		private bool? _overridenHasAnyMountedUnit;

		private bool _isArrangementShapeChanged;

		private int _currentSpawnIndex;

		private class AgentArrangementData : IFormationUnit
		{
			public IFormationArrangement Formation { get; private set; }

			public int FormationFileIndex { get; set; } = -1;

			public int FormationRankIndex { get; set; } = -1;

			public IFormationUnit FollowedUnit { get; }

			public bool IsShieldUsageEncouraged
			{
				get
				{
					return true;
				}
			}

			public bool IsPlayerUnit
			{
				get
				{
					return false;
				}
			}

			public AgentArrangementData(int index, IFormationArrangement arrangement)
			{
				this.Formation = arrangement;
			}
		}
	}
}
