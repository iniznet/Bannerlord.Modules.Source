using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200016A RID: 362
	public class SiegeQuerySystem
	{
		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x0600126A RID: 4714 RVA: 0x000474B1 File Offset: 0x000456B1
		public int LeftRegionMemberCount
		{
			get
			{
				return this._leftRegionMemberCount.Value;
			}
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x0600126B RID: 4715 RVA: 0x000474BE File Offset: 0x000456BE
		public int LeftCloseAttackerCount
		{
			get
			{
				return this._leftCloseAttackerCount.Value;
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x0600126C RID: 4716 RVA: 0x000474CB File Offset: 0x000456CB
		public int MiddleRegionMemberCount
		{
			get
			{
				return this._middleRegionMemberCount.Value;
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x0600126D RID: 4717 RVA: 0x000474D8 File Offset: 0x000456D8
		public int MiddleCloseAttackerCount
		{
			get
			{
				return this._middleCloseAttackerCount.Value;
			}
		}

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x0600126E RID: 4718 RVA: 0x000474E5 File Offset: 0x000456E5
		public int RightRegionMemberCount
		{
			get
			{
				return this._rightRegionMemberCount.Value;
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x0600126F RID: 4719 RVA: 0x000474F2 File Offset: 0x000456F2
		public int RightCloseAttackerCount
		{
			get
			{
				return this._rightCloseAttackerCount.Value;
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06001270 RID: 4720 RVA: 0x000474FF File Offset: 0x000456FF
		public int InsideAttackerCount
		{
			get
			{
				return this._insideAttackerCount.Value;
			}
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06001271 RID: 4721 RVA: 0x0004750C File Offset: 0x0004570C
		public int LeftDefenderCount
		{
			get
			{
				return this._leftDefenderCount.Value;
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06001272 RID: 4722 RVA: 0x00047519 File Offset: 0x00045719
		public int MiddleDefenderCount
		{
			get
			{
				return this._middleDefenderCount.Value;
			}
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06001273 RID: 4723 RVA: 0x00047526 File Offset: 0x00045726
		public int RightDefenderCount
		{
			get
			{
				return this._rightDefenderCount.Value;
			}
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x00047534 File Offset: 0x00045734
		public SiegeQuerySystem(Team team, IEnumerable<SiegeLane> lanes)
		{
			Mission mission = Mission.Current;
			this._attackerTeam = mission.AttackerTeam;
			Team defenderTeam = mission.DefenderTeam;
			SiegeLane siegeLane = lanes.FirstOrDefault((SiegeLane l) => l.LaneSide == FormationAI.BehaviorSide.Left);
			SiegeLane siegeLane2 = lanes.FirstOrDefault((SiegeLane l) => l.LaneSide == FormationAI.BehaviorSide.Middle);
			SiegeLane siegeLane3 = lanes.FirstOrDefault((SiegeLane l) => l.LaneSide == FormationAI.BehaviorSide.Right);
			Mission mission2 = Mission.Current;
			GameEntity gameEntity = mission2.Scene.FindEntityWithTag("left_defender_origin");
			this.LeftDefenderOrigin = ((gameEntity != null) ? gameEntity.GlobalPosition : (siegeLane.DefenderOrigin.AsVec2.IsNonZero() ? siegeLane.DefenderOrigin.GetGroundVec3() : new Vec3(0f, 0f, 0f, -1f)));
			GameEntity gameEntity2 = mission2.Scene.FindEntityWithTag("left_attacker_origin");
			this.LeftAttackerOrigin = ((gameEntity2 != null) ? gameEntity2.GlobalPosition : (siegeLane.AttackerOrigin.AsVec2.IsNonZero() ? siegeLane.AttackerOrigin.GetGroundVec3() : new Vec3(0f, 0f, 0f, -1f)));
			GameEntity gameEntity3 = mission2.Scene.FindEntityWithTag("middle_defender_origin");
			this.MidDefenderOrigin = ((gameEntity3 != null) ? gameEntity3.GlobalPosition : (siegeLane2.DefenderOrigin.AsVec2.IsNonZero() ? siegeLane2.DefenderOrigin.GetGroundVec3() : new Vec3(0f, 0f, 0f, -1f)));
			GameEntity gameEntity4 = mission2.Scene.FindEntityWithTag("middle_attacker_origin");
			this.MiddleAttackerOrigin = ((gameEntity4 != null) ? gameEntity4.GlobalPosition : (siegeLane2.AttackerOrigin.AsVec2.IsNonZero() ? siegeLane2.AttackerOrigin.GetGroundVec3() : new Vec3(0f, 0f, 0f, -1f)));
			GameEntity gameEntity5 = mission2.Scene.FindEntityWithTag("right_defender_origin");
			this.RightDefenderOrigin = ((gameEntity5 != null) ? gameEntity5.GlobalPosition : (siegeLane3.DefenderOrigin.AsVec2.IsNonZero() ? siegeLane3.DefenderOrigin.GetGroundVec3() : new Vec3(0f, 0f, 0f, -1f)));
			GameEntity gameEntity6 = mission2.Scene.FindEntityWithTag("right_attacker_origin");
			this.RightAttackerOrigin = ((gameEntity6 != null) ? gameEntity6.GlobalPosition : (siegeLane3.AttackerOrigin.AsVec2.IsNonZero() ? siegeLane3.AttackerOrigin.GetGroundVec3() : new Vec3(0f, 0f, 0f, -1f)));
			this.LeftToMidDir = (this.MiddleAttackerOrigin.AsVec2 - this.LeftDefenderOrigin.AsVec2).Normalized();
			this.MidToLeftDir = (this.LeftAttackerOrigin.AsVec2 - this.MidDefenderOrigin.AsVec2).Normalized();
			this.MidToRightDir = (this.RightAttackerOrigin.AsVec2 - this.MidDefenderOrigin.AsVec2).Normalized();
			this.RightToMidDir = (this.MiddleAttackerOrigin.AsVec2 - this.RightDefenderOrigin.AsVec2).Normalized();
			this._leftRegionMemberCount = new QueryData<int>(() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.Left), 5f);
			this._leftCloseAttackerCount = new QueryData<int>(() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.LeftClose), 5f);
			this._middleRegionMemberCount = new QueryData<int>(() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.Middle), 5f);
			this._middleCloseAttackerCount = new QueryData<int>(() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.MiddleClose), 5f);
			this._rightRegionMemberCount = new QueryData<int>(() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.Right), 5f);
			this._rightCloseAttackerCount = new QueryData<int>(() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.RightClose), 5f);
			this._insideAttackerCount = new QueryData<int>(() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.Inside), 5f);
			this._leftDefenderCount = new QueryData<int>(() => mission.GetNearbyAllyAgentsCount(this.LeftDefenderOrigin.AsVec2, 10f, defenderTeam), 5f);
			this._middleDefenderCount = new QueryData<int>(() => mission.GetNearbyAllyAgentsCount(this.MidDefenderOrigin.AsVec2, 10f, defenderTeam), 5f);
			this._rightDefenderCount = new QueryData<int>(() => mission.GetNearbyAllyAgentsCount(this.RightDefenderOrigin.AsVec2, 10f, defenderTeam), 5f);
			this.DefenderLeftToDefenderMidDir = (this.MidDefenderOrigin.AsVec2 - this.LeftDefenderOrigin.AsVec2).Normalized();
			this.DefenderMidToDefenderRightDir = (this.RightDefenderOrigin.AsVec2 - this.MidDefenderOrigin.AsVec2).Normalized();
			this.InitializeTelemetryScopeNames();
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x00047AA8 File Offset: 0x00045CA8
		private int LocateAttackers(SiegeQuerySystem.RegionEnum region)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			foreach (Agent agent in this._attackerTeam.ActiveAgents)
			{
				Vec2 vec = agent.Position.AsVec2 - this.LeftDefenderOrigin.AsVec2;
				Vec2 vec2 = agent.Position.AsVec2 - this.MidDefenderOrigin.AsVec2;
				Vec2 vec3 = agent.Position.AsVec2 - this.RightDefenderOrigin.AsVec2;
				if (vec.Normalize() < 15f && Math.Abs(agent.Position.z - this.LeftDefenderOrigin.z) <= 3f)
				{
					num2++;
					num++;
				}
				else
				{
					if (vec.DotProduct(this.LeftToMidDir) >= 0f && vec.DotProduct(this.LeftToMidDir.RightVec()) >= 0f)
					{
						num++;
					}
					else if (vec2.DotProduct(this.MidToLeftDir) >= 0f && vec2.DotProduct(this.MidToLeftDir.RightVec()) >= 0f)
					{
						num++;
					}
					if (vec3.Normalize() < 15f && Math.Abs(agent.Position.z - this.RightDefenderOrigin.z) <= 3f)
					{
						num6++;
						num5++;
					}
					else
					{
						if (vec3.DotProduct(this.RightToMidDir) >= 0f && vec3.DotProduct(this.RightToMidDir.LeftVec()) >= 0f)
						{
							num5++;
						}
						else if (vec2.DotProduct(this.MidToRightDir) >= 0f && vec2.DotProduct(this.MidToRightDir.LeftVec()) >= 0f)
						{
							num5++;
						}
						if (vec2.Normalize() < 15f && Math.Abs(agent.Position.z - this.MidDefenderOrigin.z) <= 3f)
						{
							num4++;
							num3++;
						}
						else
						{
							if ((vec2.DotProduct(this.MidToLeftDir) < 0f || vec2.DotProduct(this.MidToLeftDir.RightVec()) < 0f || vec.DotProduct(this.LeftToMidDir) < 0f || vec.DotProduct(this.LeftToMidDir.RightVec()) < 0f) && (vec2.DotProduct(this.MidToRightDir) < 0f || vec2.DotProduct(this.MidToRightDir.LeftVec()) < 0f || vec3.DotProduct(this.RightToMidDir) < 0f || vec3.DotProduct(this.RightToMidDir.LeftVec()) < 0f))
							{
								num3++;
							}
							if (agent.GetCurrentNavigationFaceId() % 10 == 1)
							{
								num7++;
							}
						}
					}
				}
			}
			float currentTime = Mission.Current.CurrentTime;
			this._leftRegionMemberCount.SetValue(num, currentTime);
			this._leftCloseAttackerCount.SetValue(num2, currentTime);
			this._middleRegionMemberCount.SetValue(num3, currentTime);
			this._middleCloseAttackerCount.SetValue(num4, currentTime);
			this._rightRegionMemberCount.SetValue(num5, currentTime);
			this._rightCloseAttackerCount.SetValue(num6, currentTime);
			this._insideAttackerCount.SetValue(num7, currentTime);
			switch (region)
			{
			case SiegeQuerySystem.RegionEnum.Left:
				return num;
			case SiegeQuerySystem.RegionEnum.LeftClose:
				return num2;
			case SiegeQuerySystem.RegionEnum.Middle:
				return num3;
			case SiegeQuerySystem.RegionEnum.MiddleClose:
				return num4;
			case SiegeQuerySystem.RegionEnum.Right:
				return num5;
			case SiegeQuerySystem.RegionEnum.RightClose:
				return num6;
			case SiegeQuerySystem.RegionEnum.Inside:
				return num7;
			default:
				return 0;
			}
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x00047EB0 File Offset: 0x000460B0
		public void Expire()
		{
			this._leftRegionMemberCount.Expire();
			this._leftCloseAttackerCount.Expire();
			this._middleRegionMemberCount.Expire();
			this._middleCloseAttackerCount.Expire();
			this._rightRegionMemberCount.Expire();
			this._rightCloseAttackerCount.Expire();
			this._insideAttackerCount.Expire();
			this._leftDefenderCount.Expire();
			this._middleDefenderCount.Expire();
			this._rightDefenderCount.Expire();
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x00047F2B File Offset: 0x0004612B
		private void InitializeTelemetryScopeNames()
		{
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x00047F30 File Offset: 0x00046130
		public int DeterminePositionAssociatedSide(Vec3 position)
		{
			float num = position.AsVec2.DistanceSquared(this.LeftDefenderOrigin.AsVec2);
			float num2 = position.AsVec2.DistanceSquared(this.MidDefenderOrigin.AsVec2);
			float num3 = position.AsVec2.DistanceSquared(this.RightDefenderOrigin.AsVec2);
			FormationAI.BehaviorSide behaviorSide;
			if (num < num2 && num < num3)
			{
				behaviorSide = FormationAI.BehaviorSide.Left;
			}
			else if (num3 < num2)
			{
				behaviorSide = FormationAI.BehaviorSide.Right;
			}
			else
			{
				behaviorSide = FormationAI.BehaviorSide.Middle;
			}
			FormationAI.BehaviorSide behaviorSide2 = FormationAI.BehaviorSide.BehaviorSideNotSet;
			switch (behaviorSide)
			{
			case FormationAI.BehaviorSide.Left:
				if ((position.AsVec2 - this.LeftDefenderOrigin.AsVec2).Normalized().DotProduct(this.DefenderLeftToDefenderMidDir) > 0f)
				{
					behaviorSide2 = FormationAI.BehaviorSide.Middle;
				}
				break;
			case FormationAI.BehaviorSide.Middle:
				if ((position.AsVec2 - this.MidDefenderOrigin.AsVec2).Normalized().DotProduct(this.DefenderMidToDefenderRightDir) > 0f)
				{
					behaviorSide2 = FormationAI.BehaviorSide.Right;
				}
				else
				{
					behaviorSide2 = FormationAI.BehaviorSide.Left;
				}
				break;
			case FormationAI.BehaviorSide.Right:
				if ((position.AsVec2 - this.RightDefenderOrigin.AsVec2).Normalized().DotProduct(this.DefenderMidToDefenderRightDir) < 0f)
				{
					behaviorSide2 = FormationAI.BehaviorSide.Middle;
				}
				break;
			}
			int num4 = 1 << (int)behaviorSide;
			if (behaviorSide2 != FormationAI.BehaviorSide.BehaviorSideNotSet)
			{
				num4 |= 1 << (int)behaviorSide2;
			}
			return num4;
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x000480AE File Offset: 0x000462AE
		public static bool AreSidesRelated(FormationAI.BehaviorSide side, int connectedSides)
		{
			return ((1 << (int)side) & connectedSides) != 0;
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x000480BC File Offset: 0x000462BC
		public static int SideDistance(int connectedSides, int side)
		{
			while (connectedSides != 0 && side != 0)
			{
				connectedSides >>= 1;
				side >>= 1;
			}
			int i = ((connectedSides != 0) ? connectedSides : side);
			int num = 0;
			while (i > 0)
			{
				num++;
				if ((i & 1) == 1)
				{
					break;
				}
				i >>= 1;
			}
			return num;
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x0600127B RID: 4731 RVA: 0x000480FA File Offset: 0x000462FA
		public Vec3 LeftDefenderOrigin { get; }

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x0600127C RID: 4732 RVA: 0x00048102 File Offset: 0x00046302
		public Vec3 MidDefenderOrigin { get; }

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x0600127D RID: 4733 RVA: 0x0004810A File Offset: 0x0004630A
		public Vec3 RightDefenderOrigin { get; }

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x0600127E RID: 4734 RVA: 0x00048112 File Offset: 0x00046312
		public Vec3 LeftAttackerOrigin { get; }

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x0600127F RID: 4735 RVA: 0x0004811A File Offset: 0x0004631A
		public Vec3 MiddleAttackerOrigin { get; }

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06001280 RID: 4736 RVA: 0x00048122 File Offset: 0x00046322
		public Vec3 RightAttackerOrigin { get; }

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06001281 RID: 4737 RVA: 0x0004812A File Offset: 0x0004632A
		public Vec2 LeftToMidDir { get; }

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06001282 RID: 4738 RVA: 0x00048132 File Offset: 0x00046332
		public Vec2 MidToLeftDir { get; }

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06001283 RID: 4739 RVA: 0x0004813A File Offset: 0x0004633A
		public Vec2 MidToRightDir { get; }

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06001284 RID: 4740 RVA: 0x00048142 File Offset: 0x00046342
		public Vec2 RightToMidDir { get; }

		// Token: 0x04000502 RID: 1282
		private const float LaneProximityDistance = 15f;

		// Token: 0x04000503 RID: 1283
		private readonly Team _attackerTeam;

		// Token: 0x04000504 RID: 1284
		public Vec2 DefenderLeftToDefenderMidDir;

		// Token: 0x04000505 RID: 1285
		public Vec2 DefenderMidToDefenderRightDir;

		// Token: 0x04000506 RID: 1286
		private readonly QueryData<int> _leftRegionMemberCount;

		// Token: 0x04000507 RID: 1287
		private readonly QueryData<int> _leftCloseAttackerCount;

		// Token: 0x04000508 RID: 1288
		private readonly QueryData<int> _middleRegionMemberCount;

		// Token: 0x04000509 RID: 1289
		private readonly QueryData<int> _middleCloseAttackerCount;

		// Token: 0x0400050A RID: 1290
		private readonly QueryData<int> _rightRegionMemberCount;

		// Token: 0x0400050B RID: 1291
		private readonly QueryData<int> _rightCloseAttackerCount;

		// Token: 0x0400050C RID: 1292
		private readonly QueryData<int> _insideAttackerCount;

		// Token: 0x0400050D RID: 1293
		private readonly QueryData<int> _leftDefenderCount;

		// Token: 0x0400050E RID: 1294
		private readonly QueryData<int> _middleDefenderCount;

		// Token: 0x0400050F RID: 1295
		private readonly QueryData<int> _rightDefenderCount;

		// Token: 0x020004DC RID: 1244
		private enum RegionEnum
		{
			// Token: 0x04001AF6 RID: 6902
			Left,
			// Token: 0x04001AF7 RID: 6903
			LeftClose,
			// Token: 0x04001AF8 RID: 6904
			Middle,
			// Token: 0x04001AF9 RID: 6905
			MiddleClose,
			// Token: 0x04001AFA RID: 6906
			Right,
			// Token: 0x04001AFB RID: 6907
			RightClose,
			// Token: 0x04001AFC RID: 6908
			Inside
		}
	}
}
