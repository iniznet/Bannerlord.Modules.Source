using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000178 RID: 376
	public abstract class TeamAISiegeComponent : TeamAIComponent
	{
		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06001350 RID: 4944 RVA: 0x0004B6D4 File Offset: 0x000498D4
		// (set) Token: 0x06001351 RID: 4945 RVA: 0x0004B6DB File Offset: 0x000498DB
		public static List<SiegeLane> SiegeLanes { get; private set; }

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06001352 RID: 4946 RVA: 0x0004B6E3 File Offset: 0x000498E3
		// (set) Token: 0x06001353 RID: 4947 RVA: 0x0004B6EA File Offset: 0x000498EA
		public static SiegeQuerySystem QuerySystem { get; protected set; }

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06001354 RID: 4948 RVA: 0x0004B6F2 File Offset: 0x000498F2
		public CastleGate OuterGate { get; }

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06001355 RID: 4949 RVA: 0x0004B6FA File Offset: 0x000498FA
		public List<IPrimarySiegeWeapon> PrimarySiegeWeapons { get; }

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06001356 RID: 4950 RVA: 0x0004B702 File Offset: 0x00049902
		public CastleGate InnerGate { get; }

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06001357 RID: 4951 RVA: 0x0004B70A File Offset: 0x0004990A
		public MBReadOnlyList<SiegeLadder> Ladders
		{
			get
			{
				return this._ladders;
			}
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06001358 RID: 4952 RVA: 0x0004B712 File Offset: 0x00049912
		// (set) Token: 0x06001359 RID: 4953 RVA: 0x0004B71A File Offset: 0x0004991A
		public bool AreLaddersReady { get; private set; }

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x0600135A RID: 4954 RVA: 0x0004B723 File Offset: 0x00049923
		// (set) Token: 0x0600135B RID: 4955 RVA: 0x0004B72B File Offset: 0x0004992B
		public List<int> DifficultNavmeshIDs { get; private set; }

		// Token: 0x0600135C RID: 4956 RVA: 0x0004B734 File Offset: 0x00049934
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

		// Token: 0x0600135D RID: 4957 RVA: 0x0004BA70 File Offset: 0x00049C70
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

		// Token: 0x0600135E RID: 4958 RVA: 0x0004BC7C File Offset: 0x00049E7C
		public static void OnMissionFinalize()
		{
			if (TeamAISiegeComponent.SiegeLanes != null)
			{
				TeamAISiegeComponent.SiegeLanes.Clear();
				TeamAISiegeComponent.SiegeLanes = null;
			}
			TeamAISiegeComponent.QuerySystem = null;
		}

		// Token: 0x0600135F RID: 4959 RVA: 0x0004BC9C File Offset: 0x00049E9C
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

		// Token: 0x06001360 RID: 4960 RVA: 0x0004BD68 File Offset: 0x00049F68
		public void SetAreLaddersReady(bool areLaddersReady)
		{
			this.AreLaddersReady = areLaddersReady;
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x0004BD71 File Offset: 0x00049F71
		public bool CalculateIsAnyLaneOpenToGetInside()
		{
			return TeamAISiegeComponent.SiegeLanes.Any((SiegeLane sl) => sl.IsOpen);
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x0004BD9C File Offset: 0x00049F9C
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

		// Token: 0x06001363 RID: 4963 RVA: 0x0004BDC7 File Offset: 0x00049FC7
		public bool IsPrimarySiegeWeaponNavmeshFaceId(int id)
		{
			return this.PrimarySiegeWeaponNavMeshFaceIDs.Contains(id);
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x0004BDD8 File Offset: 0x00049FD8
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

		// Token: 0x06001365 RID: 4965 RVA: 0x0004BEA0 File Offset: 0x0004A0A0
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

		// Token: 0x06001366 RID: 4966 RVA: 0x0004BF40 File Offset: 0x0004A140
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

		// Token: 0x06001367 RID: 4967 RVA: 0x0004C03C File Offset: 0x0004A23C
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

		// Token: 0x0400057D RID: 1405
		public const int InsideCastleNavMeshID = 1;

		// Token: 0x0400057E RID: 1406
		public const int SiegeTokenForceSize = 15;

		// Token: 0x0400057F RID: 1407
		private const float FormationInsideCastleThresholdPercentage = 0.4f;

		// Token: 0x04000580 RID: 1408
		private const float CastleBreachThresholdPercentage = 0.7f;

		// Token: 0x04000583 RID: 1411
		public readonly IEnumerable<WallSegment> WallSegments;

		// Token: 0x04000584 RID: 1412
		public readonly List<SiegeWeapon> SceneSiegeWeapons;

		// Token: 0x04000585 RID: 1413
		protected readonly IEnumerable<CastleGate> CastleGates;

		// Token: 0x04000586 RID: 1414
		protected readonly List<SiegeTower> SiegeTowers;

		// Token: 0x04000587 RID: 1415
		protected readonly HashSet<int> PrimarySiegeWeaponNavMeshFaceIDs;

		// Token: 0x04000588 RID: 1416
		protected BatteringRam Ram;

		// Token: 0x04000589 RID: 1417
		protected List<MissionObject> CastleKeyPositions;

		// Token: 0x0400058A RID: 1418
		private readonly MBList<SiegeLadder> _ladders;

		// Token: 0x0400058B RID: 1419
		private bool _noProperLaneRemains;
	}
}
