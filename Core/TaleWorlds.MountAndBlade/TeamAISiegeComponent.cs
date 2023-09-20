using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class TeamAISiegeComponent : TeamAIComponent
	{
		public static List<SiegeLane> SiegeLanes { get; private set; }

		public static SiegeQuerySystem QuerySystem { get; protected set; }

		public CastleGate OuterGate { get; }

		public List<IPrimarySiegeWeapon> PrimarySiegeWeapons { get; }

		public CastleGate InnerGate { get; }

		public MBReadOnlyList<SiegeLadder> Ladders
		{
			get
			{
				return this._ladders;
			}
		}

		public bool AreLaddersReady { get; private set; }

		public List<int> DifficultNavmeshIDs { get; private set; }

		protected TeamAISiegeComponent(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
			this.CastleGates = currentMission.ActiveMissionObjects.FindAllWithType<CastleGate>().ToList<CastleGate>();
			this.WallSegments = currentMission.ActiveMissionObjects.FindAllWithType<WallSegment>().ToList<WallSegment>();
			this.OuterGate = this.CastleGates.FirstOrDefault((CastleGate g) => g.GameEntity.HasTag("outer_gate"));
			this.InnerGate = this.CastleGates.FirstOrDefault((CastleGate g) => g.GameEntity.HasTag("inner_gate"));
			this.SceneSiegeWeapons = Mission.Current.MissionObjects.FindAllWithType<SiegeWeapon>().ToList<SiegeWeapon>();
			this._ladders = this.SceneSiegeWeapons.OfType<SiegeLadder>().ToMBList<SiegeLadder>();
			this.Ram = this.SceneSiegeWeapons.FirstOrDefault((SiegeWeapon ssw) => ssw is BatteringRam) as BatteringRam;
			this.SiegeTowers = this.SceneSiegeWeapons.OfType<SiegeTower>().ToList<SiegeTower>();
			this.PrimarySiegeWeapons = new List<IPrimarySiegeWeapon>();
			this.PrimarySiegeWeapons.AddRange(this._ladders);
			if (this.Ram != null)
			{
				this.PrimarySiegeWeapons.Add(this.Ram);
			}
			this.PrimarySiegeWeapons.AddRange(this.SiegeTowers);
			this.PrimarySiegeWeaponNavMeshFaceIDs = new HashSet<int>();
			using (List<IPrimarySiegeWeapon>.Enumerator enumerator = this.PrimarySiegeWeapons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IPrimarySiegeWeapon primarySiegeWeapon;
					List<int> list;
					if ((primarySiegeWeapon = enumerator.Current) != null && primarySiegeWeapon.GetNavmeshFaceIds(out list))
					{
						this.PrimarySiegeWeaponNavMeshFaceIDs.UnionWith(list);
					}
				}
			}
			this.CastleKeyPositions = new List<MissionObject>();
			this.CastleKeyPositions.AddRange(this.CastleGates);
			this.CastleKeyPositions.AddRange(this.WallSegments);
			TeamAISiegeComponent.SiegeLanes = new List<SiegeLane>();
			int i;
			int j;
			for (i = 0; i < 3; i = j + 1)
			{
				TeamAISiegeComponent.SiegeLanes.Add(new SiegeLane((FormationAI.BehaviorSide)i, TeamAISiegeComponent.QuerySystem));
				TeamAISiegeComponent.SiegeLanes[i].SetPrimarySiegeWeapons((from psw in this.PrimarySiegeWeapons
					where psw.WeaponSide == (FormationAI.BehaviorSide)i
					select psw into um
					select (um)).ToList<IPrimarySiegeWeapon>());
				TeamAISiegeComponent.SiegeLanes[i].SetDefensePoints((from ckp in this.CastleKeyPositions
					where ((ICastleKeyPosition)ckp).DefenseSide == (FormationAI.BehaviorSide)i
					select ckp into dp
					select (ICastleKeyPosition)dp).ToList<ICastleKeyPosition>());
				TeamAISiegeComponent.SiegeLanes[i].RefreshLane();
				j = i;
			}
			TeamAISiegeComponent.SiegeLanes.ForEach(delegate(SiegeLane sl)
			{
				sl.SetSiegeQuerySystem(TeamAISiegeComponent.QuerySystem);
			});
			this.DifficultNavmeshIDs = new List<int>();
		}

		protected internal override void Tick(float dt)
		{
			if (!this._noProperLaneRemains)
			{
				int num = 0;
				SiegeLane siegeLane = null;
				foreach (SiegeLane siegeLane2 in TeamAISiegeComponent.SiegeLanes)
				{
					siegeLane2.RefreshLane();
					siegeLane2.DetermineLaneState();
					if (siegeLane2.IsBreach)
					{
						num++;
					}
					else
					{
						siegeLane = siegeLane2;
					}
				}
				if (siegeLane != null && num >= 2 && !siegeLane.IsOpen && siegeLane.LaneState >= SiegeLane.LaneStateEnum.Used)
				{
					siegeLane.SetLaneState(SiegeLane.LaneStateEnum.Unused);
				}
				if (TeamAISiegeComponent.SiegeLanes.Count != 0)
				{
					goto IL_1D0;
				}
				this._noProperLaneRemains = true;
				using (IEnumerator<FormationAI.BehaviorSide> enumerator2 = (from ckp in this.CastleKeyPositions.Where(delegate(MissionObject ckp)
					{
						CastleGate castleGate;
						return (castleGate = ckp as CastleGate) != null && castleGate.DefenseSide != FormationAI.BehaviorSide.BehaviorSideNotSet;
					})
					select ((CastleGate)ckp).DefenseSide).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						FormationAI.BehaviorSide difficultLaneSide = enumerator2.Current;
						SiegeLane siegeLane3 = new SiegeLane(difficultLaneSide, TeamAISiegeComponent.QuerySystem);
						siegeLane3.SetPrimarySiegeWeapons(new List<IPrimarySiegeWeapon>());
						siegeLane3.SetDefensePoints((from ckp in this.CastleKeyPositions
							where ((ICastleKeyPosition)ckp).DefenseSide == difficultLaneSide && ckp is CastleGate
							select ckp into dp
							select dp as ICastleKeyPosition).ToList<ICastleKeyPosition>());
						siegeLane3.RefreshLane();
						siegeLane3.DetermineLaneState();
						TeamAISiegeComponent.SiegeLanes.Add(siegeLane3);
					}
					goto IL_1D0;
				}
			}
			foreach (SiegeLane siegeLane4 in TeamAISiegeComponent.SiegeLanes)
			{
				siegeLane4.RefreshLane();
				siegeLane4.DetermineLaneState();
			}
			IL_1D0:
			base.Tick(dt);
		}

		public static void OnMissionFinalize()
		{
			if (TeamAISiegeComponent.SiegeLanes != null)
			{
				TeamAISiegeComponent.SiegeLanes.Clear();
				TeamAISiegeComponent.SiegeLanes = null;
			}
			TeamAISiegeComponent.QuerySystem = null;
		}

		public bool CalculateIsChargePastWallsApplicable(FormationAI.BehaviorSide side)
		{
			if (Mission.Current.MissionTeamAIType == Mission.MissionTeamAITypeEnum.SallyOut)
			{
				return false;
			}
			if (side == FormationAI.BehaviorSide.BehaviorSideNotSet && this.InnerGate != null && !this.InnerGate.IsGateOpen)
			{
				return false;
			}
			foreach (SiegeLane siegeLane in TeamAISiegeComponent.SiegeLanes)
			{
				if (side == FormationAI.BehaviorSide.BehaviorSideNotSet)
				{
					if (!siegeLane.IsOpen)
					{
						return false;
					}
				}
				else if (side == siegeLane.LaneSide)
				{
					return siegeLane.IsOpen && (siegeLane.IsBreach || (siegeLane.HasGate && (this.InnerGate == null || this.InnerGate.IsGateOpen)));
				}
			}
			return true;
		}

		public void SetAreLaddersReady(bool areLaddersReady)
		{
			this.AreLaddersReady = areLaddersReady;
		}

		public bool CalculateIsAnyLaneOpenToGetInside()
		{
			return TeamAISiegeComponent.SiegeLanes.Any((SiegeLane sl) => sl.IsOpen);
		}

		public bool CalculateIsAnyLaneOpenToGoOutside()
		{
			return TeamAISiegeComponent.SiegeLanes.Any(delegate(SiegeLane sl)
			{
				if (!sl.IsOpen)
				{
					return false;
				}
				if (!sl.IsBreach && !sl.HasGate)
				{
					return sl.PrimarySiegeWeapons.Any((IPrimarySiegeWeapon psw) => psw is SiegeTower);
				}
				return true;
			});
		}

		public bool IsPrimarySiegeWeaponNavmeshFaceId(int id)
		{
			return this.PrimarySiegeWeaponNavMeshFaceIDs.Contains(id);
		}

		public static bool IsFormationGroupInsideCastle(MBList<Formation> formationGroup, bool includeOnlyPositionedUnits, float thresholdPercentage = 0.4f)
		{
			int num = 0;
			foreach (Formation formation in formationGroup)
			{
				num += (includeOnlyPositionedUnits ? formation.Arrangement.PositionedUnitCount : formation.CountOfUnits);
			}
			float num2 = (float)num * thresholdPercentage;
			foreach (Formation formation2 in formationGroup)
			{
				if (formation2.CountOfUnits > 0)
				{
					num2 -= (float)formation2.CountUnitsOnNavMeshIDMod10(1, includeOnlyPositionedUnits);
					if (num2 <= 0f)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsFormationInsideCastle(Formation formation, bool includeOnlyPositionedUnits, float thresholdPercentage = 0.4f)
		{
			int num = (includeOnlyPositionedUnits ? formation.Arrangement.PositionedUnitCount : formation.CountOfUnits);
			float num2 = (float)num * thresholdPercentage;
			if (num == 0)
			{
				return !(formation.Team.TeamAI is TeamAISiegeAttacker) && !(formation.Team.TeamAI is TeamAISallyOutDefender) && (formation.Team.TeamAI is TeamAISiegeDefender || formation.Team.TeamAI is TeamAISallyOutAttacker);
			}
			if (includeOnlyPositionedUnits)
			{
				return (float)formation.QuerySystem.InsideCastleUnitCountPositioned >= num2;
			}
			return (float)formation.QuerySystem.InsideCastleUnitCountIncludingUnpositioned >= num2;
		}

		public bool IsCastleBreached()
		{
			int num = 0;
			int num2 = 0;
			foreach (Formation formation in this.Mission.AttackerTeam.FormationsIncludingSpecialAndEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					num2++;
					if (TeamAISiegeComponent.IsFormationInsideCastle(formation, true, 0.4f))
					{
						num++;
					}
				}
			}
			if (this.Mission.AttackerAllyTeam != null)
			{
				foreach (Formation formation2 in this.Mission.AttackerAllyTeam.FormationsIncludingSpecialAndEmpty)
				{
					if (formation2.CountOfUnits > 0)
					{
						num2++;
						if (TeamAISiegeComponent.IsFormationInsideCastle(formation2, true, 0.4f))
						{
							num++;
						}
					}
				}
			}
			return (float)num >= (float)num2 * 0.7f;
		}

		public override void OnDeploymentFinished()
		{
			foreach (SiegeLadder siegeLadder in this._ladders.Where((SiegeLadder l) => !l.IsDisabled))
			{
				this.DifficultNavmeshIDs.Add(siegeLadder.OnWallNavMeshId);
			}
			foreach (SiegeTower siegeTower in this.SiegeTowers)
			{
				this.DifficultNavmeshIDs.AddRange(siegeTower.CollectGetDifficultNavmeshIDs());
			}
		}

		public const int InsideCastleNavMeshID = 1;

		public const int SiegeTokenForceSize = 15;

		private const float FormationInsideCastleThresholdPercentage = 0.4f;

		private const float CastleBreachThresholdPercentage = 0.7f;

		public readonly IEnumerable<WallSegment> WallSegments;

		public readonly List<SiegeWeapon> SceneSiegeWeapons;

		protected readonly IEnumerable<CastleGate> CastleGates;

		protected readonly List<SiegeTower> SiegeTowers;

		protected readonly HashSet<int> PrimarySiegeWeaponNavMeshFaceIDs;

		protected BatteringRam Ram;

		protected List<MissionObject> CastleKeyPositions;

		private readonly MBList<SiegeLadder> _ladders;

		private bool _noProperLaneRemains;
	}
}
