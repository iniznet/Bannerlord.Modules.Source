using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade.DividableTasks;

namespace TaleWorlds.MountAndBlade
{
	public abstract class RangedSiegeWeaponAi : UsableMachineAIBase
	{
		public RangedSiegeWeaponAi(RangedSiegeWeapon rangedSiegeWeapon)
			: base(rangedSiegeWeapon)
		{
			this._threatSeeker = new RangedSiegeWeaponAi.ThreatSeeker(rangedSiegeWeapon);
			((RangedSiegeWeapon)this.UsableMachine).OnReloadDone += this.FindNextTarget;
			this._delayTimer = this._delayDuration;
			this._targetEvaluationTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
		}

		protected override void OnTick(Agent agentToCompareTo, Formation formationToCompareTo, Team potentialUsersTeam, float dt)
		{
			base.OnTick(agentToCompareTo, formationToCompareTo, potentialUsersTeam, dt);
			if (this.UsableMachine.PilotAgent != null && this.UsableMachine.PilotAgent.IsAIControlled)
			{
				RangedSiegeWeapon rangedSiegeWeapon = this.UsableMachine as RangedSiegeWeapon;
				if (rangedSiegeWeapon.State == RangedSiegeWeapon.WeaponState.WaitingAfterShooting && rangedSiegeWeapon.PilotAgent != null && rangedSiegeWeapon.PilotAgent.IsAIControlled)
				{
					rangedSiegeWeapon.AiRequestsManualReload();
				}
				if (this._threatSeeker.UpdateThreatSeekerTask() && dt > 0f && this._target == null && rangedSiegeWeapon.State == RangedSiegeWeapon.WeaponState.Idle)
				{
					if (this._delayTimer <= 0f)
					{
						this.FindNextTarget();
					}
					this._delayTimer -= dt;
				}
				if (this._target != null)
				{
					if (this._target.Agent != null && !this._target.Agent.IsActive())
					{
						this._target = null;
						return;
					}
					if (rangedSiegeWeapon.State == RangedSiegeWeapon.WeaponState.Idle && rangedSiegeWeapon.UserCountNotInStruckAction > 0)
					{
						if (DebugSiegeBehavior.ToggleTargetDebug)
						{
							Agent pilotAgent = this.UsableMachine.PilotAgent;
						}
						if (this._targetEvaluationTimer.Check(Mission.Current.CurrentTime) && !((RangedSiegeWeapon)this.UsableMachine).CanShootAtBox(this._target.BoundingBoxMin, this._target.BoundingBoxMax, 5U))
						{
							this._cannotShootCounter++;
						}
						if (this._cannotShootCounter < 4)
						{
							if (rangedSiegeWeapon.AimAtThreat(this._target) && rangedSiegeWeapon.PilotAgent != null)
							{
								this._delayTimer -= dt;
								if (this._delayTimer <= 0f)
								{
									rangedSiegeWeapon.AiRequestsShoot();
									this._target = null;
									this.SetTargetingTimer();
									this._cannotShootCounter = 0;
									this._targetEvaluationTimer.Reset(Mission.Current.CurrentTime);
								}
							}
						}
						else
						{
							this._target = null;
							this.SetTargetingTimer();
							this._cannotShootCounter = 0;
						}
					}
					else
					{
						this._targetEvaluationTimer.Reset(Mission.Current.CurrentTime);
					}
				}
			}
			this.AfterTick(agentToCompareTo, formationToCompareTo, potentialUsersTeam, dt);
		}

		private void SetTargetFromThreatSeeker()
		{
			this._target = this._threatSeeker.PrepareTargetFromTask();
		}

		public void FindNextTarget()
		{
			if (this.UsableMachine.PilotAgent != null && this.UsableMachine.PilotAgent.IsAIControlled)
			{
				this._threatSeeker.PrepareThreatSeekerTask(new Action(this.SetTargetFromThreatSeeker));
				this.SetTargetingTimer();
			}
		}

		private void AfterTick(Agent agentToCompareTo, Formation formationToCompareTo, Team potentialUsersTeam, float dt)
		{
			if ((dt <= 0f || (agentToCompareTo != null && this.UsableMachine.PilotAgent != agentToCompareTo) || (formationToCompareTo != null && (this.UsableMachine.PilotAgent == null || !this.UsableMachine.PilotAgent.IsAIControlled || this.UsableMachine.PilotAgent.Formation != formationToCompareTo))) && this.UsableMachine.PilotAgent == null)
			{
				this._threatSeeker.Release();
				this._target = null;
			}
		}

		private void SetTargetingTimer()
		{
			this._delayTimer = this._delayDuration + MBRandom.RandomFloat * 0.5f;
		}

		private const float TargetEvaluationDelay = 0.5f;

		private const int MaxTargetEvaluationCount = 4;

		private readonly RangedSiegeWeaponAi.ThreatSeeker _threatSeeker;

		private Threat _target;

		private float _delayTimer;

		private float _delayDuration = 1f;

		private int _cannotShootCounter;

		private readonly Timer _targetEvaluationTimer;

		public class ThreatSeeker
		{
			public ThreatSeeker(RangedSiegeWeapon weapon)
			{
				this.Weapon = weapon;
				this.WeaponPositions = new List<Vec3> { this.Weapon.GameEntity.GlobalPosition };
				this._targetAgent = null;
				IEnumerable<UsableMachine> enumerable = Mission.Current.ActiveMissionObjects.FindAllWithType<UsableMachine>();
				this._potentialTargetUsableMachines = (from um in enumerable.WhereQ(delegate(UsableMachine um)
					{
						ITargetable targetable;
						return um.GameEntity.HasScriptOfType<DestructableComponent>() && (targetable = um as ITargetable) != null && targetable.GetSide() != this.Weapon.Side && targetable.GetTargetEntity() != null;
					})
					select um as ITargetable).ToList<ITargetable>();
				this._referencePositions = enumerable.OfType<ICastleKeyPosition>().ToList<ICastleKeyPosition>();
				this._getMostDangerousThreat = new FindMostDangerousThreat(null);
			}

			public Threat PrepareTargetFromTask()
			{
				Agent agent2;
				this._currentThreat = this._getMostDangerousThreat.GetResult(out agent2);
				if (this._currentThreat != null && this._currentThreat.WeaponEntity == null)
				{
					this._currentThreat.Agent = this._targetAgent;
					if (this._targetAgent == null || !this._targetAgent.IsActive() || this._targetAgent.Formation != this._currentThreat.Formation || !this.Weapon.CanShootAtAgent(this._targetAgent))
					{
						this._targetAgent = agent2;
						float selectedAgentScore = float.MaxValue;
						Agent selectedAgent = this._targetAgent;
						Action<Agent> action = delegate(Agent agent)
						{
							float num = agent.Position.DistanceSquared(this.Weapon.GameEntity.GlobalPosition) * (MBRandom.RandomFloat * 0.2f + 0.8f);
							if (agent == this._targetAgent)
							{
								num *= 0.5f;
							}
							if (selectedAgentScore > num && this.Weapon.CanShootAtAgent(agent))
							{
								selectedAgent = agent;
								selectedAgentScore = num;
							}
						};
						if (agent2.Detachment == null)
						{
							this._currentThreat.Formation.ApplyActionOnEachAttachedUnit(action);
						}
						else
						{
							this._currentThreat.Formation.ApplyActionOnEachDetachedUnit(action);
						}
						this._targetAgent = selectedAgent ?? this._currentThreat.Formation.GetUnitWithIndex(MBRandom.RandomInt(this._currentThreat.Formation.CountOfUnits));
						this._currentThreat.Agent = this._targetAgent;
					}
				}
				if (this._currentThreat != null && this._currentThreat.WeaponEntity == null && this._currentThreat.Agent == null)
				{
					this._currentThreat = null;
				}
				return this._currentThreat;
			}

			public bool UpdateThreatSeekerTask()
			{
				Agent targetAgent = this._targetAgent;
				if (targetAgent != null && !targetAgent.IsActive())
				{
					this._targetAgent = null;
				}
				return this._getMostDangerousThreat.Update();
			}

			public void PrepareThreatSeekerTask(Action lastAction)
			{
				this._getMostDangerousThreat.Prepare(this.GetAllThreats(), this.Weapon);
				this._getMostDangerousThreat.SetLastAction(lastAction);
			}

			public void Release()
			{
				this._targetAgent = null;
				this._currentThreat = null;
			}

			public List<Threat> GetAllThreats()
			{
				List<Threat> list = new List<Threat>();
				for (int i = this._potentialTargetUsableMachines.Count - 1; i >= 0; i--)
				{
					ITargetable targetable = this._potentialTargetUsableMachines[i];
					UsableMachine usableMachine;
					if ((usableMachine = targetable as UsableMachine) != null && (usableMachine.IsDestroyed || usableMachine.IsDeactivated || usableMachine.GameEntity == null))
					{
						this._potentialTargetUsableMachines.RemoveAt(i);
					}
					else
					{
						Threat threat = new Threat
						{
							WeaponEntity = targetable,
							ThreatValue = this.Weapon.ProcessTargetValue(targetable.GetTargetValue(this.WeaponPositions), targetable.GetTargetFlags())
						};
						list.Add(threat);
					}
				}
				foreach (Team team in Mission.Current.Teams)
				{
					if (team.Side.GetOppositeSide() == this.Weapon.Side)
					{
						foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
						{
							if (formation.CountOfUnits > 0)
							{
								float targetValueOfFormation = RangedSiegeWeaponAi.ThreatSeeker.GetTargetValueOfFormation(formation, this._referencePositions);
								if (targetValueOfFormation != -1f)
								{
									list.Add(new Threat
									{
										Formation = formation,
										ThreatValue = this.Weapon.ProcessTargetValue(targetValueOfFormation, RangedSiegeWeaponAi.ThreatSeeker.GetTargetFlagsOfFormation())
									});
								}
							}
						}
					}
				}
				return list;
			}

			private static float GetTargetValueOfFormation(Formation formation, IEnumerable<ICastleKeyPosition> referencePositions)
			{
				if (formation.QuerySystem.LocalEnemyPower / formation.QuerySystem.LocalAllyPower > 0.5f)
				{
					return -1f;
				}
				float num = (float)formation.CountOfUnits * 3f;
				if (TeamAISiegeComponent.IsFormationInsideCastle(formation, true, 0.4f))
				{
					num *= 3f;
				}
				num *= RangedSiegeWeaponAi.ThreatSeeker.GetPositionMultiplierOfFormation(formation, referencePositions);
				float num2 = MBMath.ClampFloat(formation.QuerySystem.LocalAllyPower / (formation.QuerySystem.LocalEnemyPower + 0.01f), 0f, 5f) / 5f;
				return num * num2;
			}

			public static TargetFlags GetTargetFlagsOfFormation()
			{
				return TargetFlags.None | TargetFlags.IsMoving | TargetFlags.IsFlammable | TargetFlags.IsAttacker;
			}

			private static float GetPositionMultiplierOfFormation(Formation formation, IEnumerable<ICastleKeyPosition> referencePositions)
			{
				ICastleKeyPosition castleKeyPosition;
				float minimumDistanceBetweenPositions = RangedSiegeWeaponAi.ThreatSeeker.GetMinimumDistanceBetweenPositions(formation.GetMedianAgent(false, false, formation.GetAveragePositionOfUnits(false, false)).Position, referencePositions, out castleKeyPosition);
				bool flag = castleKeyPosition.AttackerSiegeWeapon != null && castleKeyPosition.AttackerSiegeWeapon.HasCompletedAction();
				float num;
				if (formation.PhysicalClass.IsRanged())
				{
					if (minimumDistanceBetweenPositions < 20f)
					{
						num = 1f;
					}
					else if (minimumDistanceBetweenPositions < 35f)
					{
						num = 0.8f;
					}
					else
					{
						num = 0.6f;
					}
					return num + (flag ? 0.2f : 0f);
				}
				if (minimumDistanceBetweenPositions < 15f)
				{
					num = 0.2f;
				}
				else if (minimumDistanceBetweenPositions < 40f)
				{
					num = 0.15f;
				}
				else
				{
					num = 0.12f;
				}
				return num * (flag ? 7.5f : 1f);
			}

			private static float GetMinimumDistanceBetweenPositions(Vec3 position, IEnumerable<ICastleKeyPosition> referencePositions, out ICastleKeyPosition closestCastlePosition)
			{
				if (referencePositions != null && referencePositions.Count<ICastleKeyPosition>() != 0)
				{
					closestCastlePosition = referencePositions.MinBy((ICastleKeyPosition rp) => rp.GetPosition().DistanceSquared(position));
					return MathF.Sqrt(closestCastlePosition.GetPosition().DistanceSquared(position));
				}
				closestCastlePosition = null;
				return -1f;
			}

			public static Threat GetMaxThreat(List<ICastleKeyPosition> castleKeyPositions)
			{
				List<ITargetable> list = new List<ITargetable>();
				List<Threat> list2 = new List<Threat>();
				using (IEnumerator<GameEntity> enumerator = Mission.Current.ActiveMissionObjects.Select((MissionObject amo) => amo.GameEntity).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ITargetable targetable;
						if ((targetable = enumerator.Current.GetFirstScriptOfType<UsableMachine>() as ITargetable) != null)
						{
							list.Add(targetable);
						}
					}
				}
				list.RemoveAll((ITargetable um) => um.GetSide() == BattleSideEnum.Defender);
				list2.AddRange(list.Select(delegate(ITargetable um)
				{
					Threat threat = new Threat();
					threat.WeaponEntity = um;
					threat.ThreatValue = um.GetTargetValue(castleKeyPositions.Select((ICastleKeyPosition c) => c.GetPosition()).ToList<Vec3>());
					return threat;
				}));
				return list2.MaxBy((Threat t) => t.ThreatValue);
			}

			private FindMostDangerousThreat _getMostDangerousThreat;

			private const float SingleUnitThreatValue = 3f;

			private const float InsideWallsThreatMultiplier = 3f;

			private Threat _currentThreat;

			private Agent _targetAgent;

			public RangedSiegeWeapon Weapon;

			public List<Vec3> WeaponPositions;

			private readonly List<ITargetable> _potentialTargetUsableMachines;

			private readonly List<ICastleKeyPosition> _referencePositions;
		}
	}
}
