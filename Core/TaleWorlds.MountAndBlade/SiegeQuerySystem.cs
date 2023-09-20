using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class SiegeQuerySystem
	{
		public int LeftRegionMemberCount
		{
			get
			{
				return this._leftRegionMemberCount.Value;
			}
		}

		public int LeftCloseAttackerCount
		{
			get
			{
				return this._leftCloseAttackerCount.Value;
			}
		}

		public int MiddleRegionMemberCount
		{
			get
			{
				return this._middleRegionMemberCount.Value;
			}
		}

		public int MiddleCloseAttackerCount
		{
			get
			{
				return this._middleCloseAttackerCount.Value;
			}
		}

		public int RightRegionMemberCount
		{
			get
			{
				return this._rightRegionMemberCount.Value;
			}
		}

		public int RightCloseAttackerCount
		{
			get
			{
				return this._rightCloseAttackerCount.Value;
			}
		}

		public int InsideAttackerCount
		{
			get
			{
				return this._insideAttackerCount.Value;
			}
		}

		public int LeftDefenderCount
		{
			get
			{
				return this._leftDefenderCount.Value;
			}
		}

		public int MiddleDefenderCount
		{
			get
			{
				return this._middleDefenderCount.Value;
			}
		}

		public int RightDefenderCount
		{
			get
			{
				return this._rightDefenderCount.Value;
			}
		}

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

		private void InitializeTelemetryScopeNames()
		{
		}

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

		public static bool AreSidesRelated(FormationAI.BehaviorSide side, int connectedSides)
		{
			return ((1 << (int)side) & connectedSides) != 0;
		}

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

		public Vec3 LeftDefenderOrigin { get; }

		public Vec3 MidDefenderOrigin { get; }

		public Vec3 RightDefenderOrigin { get; }

		public Vec3 LeftAttackerOrigin { get; }

		public Vec3 MiddleAttackerOrigin { get; }

		public Vec3 RightAttackerOrigin { get; }

		public Vec2 LeftToMidDir { get; }

		public Vec2 MidToLeftDir { get; }

		public Vec2 MidToRightDir { get; }

		public Vec2 RightToMidDir { get; }

		private const float LaneProximityDistance = 15f;

		private readonly Team _attackerTeam;

		public Vec2 DefenderLeftToDefenderMidDir;

		public Vec2 DefenderMidToDefenderRightDir;

		private readonly QueryData<int> _leftRegionMemberCount;

		private readonly QueryData<int> _leftCloseAttackerCount;

		private readonly QueryData<int> _middleRegionMemberCount;

		private readonly QueryData<int> _middleCloseAttackerCount;

		private readonly QueryData<int> _rightRegionMemberCount;

		private readonly QueryData<int> _rightCloseAttackerCount;

		private readonly QueryData<int> _insideAttackerCount;

		private readonly QueryData<int> _leftDefenderCount;

		private readonly QueryData<int> _middleDefenderCount;

		private readonly QueryData<int> _rightDefenderCount;

		private enum RegionEnum
		{
			Left,
			LeftClose,
			Middle,
			MiddleClose,
			Right,
			RightClose,
			Inside
		}
	}
}
