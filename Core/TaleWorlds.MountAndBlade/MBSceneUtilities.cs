﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000241 RID: 577
	public static class MBSceneUtilities
	{
		// Token: 0x06001F6F RID: 8047 RVA: 0x0006F680 File Offset: 0x0006D880
		public static MBList<Path> GetAllSpawnPaths(Scene scene)
		{
			MBList<Path> mblist = new MBList<Path>();
			for (int i = 0; i < 32; i++)
			{
				string text = "spawn_path_" + i.ToString("D2");
				Path pathWithName = scene.GetPathWithName(text);
				if (pathWithName != null && pathWithName.NumberOfPoints > 1)
				{
					mblist.Add(pathWithName);
				}
			}
			return mblist;
		}

		// Token: 0x06001F70 RID: 8048 RVA: 0x0006F6DC File Offset: 0x0006D8DC
		public static List<Vec2> GetSceneBoundaryPoints(Scene scene, out string boundaryName)
		{
			List<Vec2> list = new List<Vec2>();
			int softBoundaryVertexCount = scene.GetSoftBoundaryVertexCount();
			if (softBoundaryVertexCount >= 3)
			{
				boundaryName = "walk_area";
				for (int i = 0; i < softBoundaryVertexCount; i++)
				{
					Vec2 softBoundaryVertex = scene.GetSoftBoundaryVertex(i);
					list.Add(softBoundaryVertex);
				}
			}
			else
			{
				boundaryName = "scene_boundary";
				Vec3 vec;
				Vec3 vec2;
				scene.GetBoundingBox(out vec, out vec2);
				float num = MathF.Min(2f, vec2.x - vec.x);
				float num2 = MathF.Min(2f, vec2.y - vec.y);
				List<Vec2> list2 = new List<Vec2>
				{
					new Vec2(vec.x + num, vec.y + num2),
					new Vec2(vec2.x - num, vec.y + num2),
					new Vec2(vec2.x - num, vec2.y - num2),
					new Vec2(vec.x + num, vec2.y - num2)
				};
				list.AddRange(list2);
			}
			return list;
		}

		// Token: 0x06001F71 RID: 8049 RVA: 0x0006F7F4 File Offset: 0x0006D9F4
		public static void ProjectPositionToDeploymentBoundaries(BattleSideEnum side, ref WorldPosition position)
		{
			Mission mission = Mission.Current;
			IMissionDeploymentPlan deploymentPlan = mission.DeploymentPlan;
			if (deploymentPlan.HasDeploymentBoundaries(side, DeploymentPlanType.Initial))
			{
				IMissionDeploymentPlan missionDeploymentPlan = deploymentPlan;
				Vec2 vec = position.AsVec2;
				if (!missionDeploymentPlan.IsPositionInsideDeploymentBoundaries(side, vec, DeploymentPlanType.Initial))
				{
					IMissionDeploymentPlan missionDeploymentPlan2 = deploymentPlan;
					vec = position.AsVec2;
					Vec2 closestDeploymentBoundaryPosition = missionDeploymentPlan2.GetClosestDeploymentBoundaryPosition(side, vec, DeploymentPlanType.Initial);
					position = new WorldPosition(mission.Scene, new Vec3(closestDeploymentBoundaryPosition, position.GetGroundZ(), -1f));
				}
			}
		}

		// Token: 0x06001F72 RID: 8050 RVA: 0x0006F864 File Offset: 0x0006DA64
		public static void RadialSortBoundaries(ref List<Vec2> boundaries)
		{
			MBSceneUtilities.<>c__DisplayClass8_0 CS$<>8__locals1 = new MBSceneUtilities.<>c__DisplayClass8_0();
			if (boundaries.Count == 0)
			{
				return;
			}
			CS$<>8__locals1.boundaryCenter = Vec2.Zero;
			foreach (Vec2 vec in boundaries)
			{
				CS$<>8__locals1.boundaryCenter += vec;
			}
			MBSceneUtilities.<>c__DisplayClass8_0 CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.boundaryCenter.x = CS$<>8__locals2.boundaryCenter.x / (float)boundaries.Count;
			MBSceneUtilities.<>c__DisplayClass8_0 CS$<>8__locals3 = CS$<>8__locals1;
			CS$<>8__locals3.boundaryCenter.y = CS$<>8__locals3.boundaryCenter.y / (float)boundaries.Count;
			boundaries = boundaries.OrderBy((Vec2 b) => (b - CS$<>8__locals1.boundaryCenter).RotationInRadians).ToList<Vec2>();
		}

		// Token: 0x06001F73 RID: 8051 RVA: 0x0006F924 File Offset: 0x0006DB24
		public static bool IsPointInsideBoundaries(in Vec2 point, List<Vec2> boundaries, float acceptanceThreshold = 0.01f)
		{
			acceptanceThreshold = MathF.Max(0f, acceptanceThreshold);
			if (boundaries.Count <= 2)
			{
				return false;
			}
			bool flag = true;
			for (int i = 0; i < boundaries.Count; i++)
			{
				Vec2 vec = boundaries[i];
				Vec2 vec2 = boundaries[(i + 1) % boundaries.Count] - vec;
				Vec2 vec3 = point - vec;
				if (vec2.x * vec3.y - vec2.y * vec3.x < -acceptanceThreshold)
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		// Token: 0x06001F74 RID: 8052 RVA: 0x0006F9B0 File Offset: 0x0006DBB0
		public static float FindClosestPointToBoundaries(in Vec2 position, List<Vec2> boundaries, out Vec2 closestPoint)
		{
			closestPoint = position;
			float num = float.MaxValue;
			for (int i = 0; i < boundaries.Count; i++)
			{
				Vec2 vec = boundaries[i];
				Vec2 vec2 = boundaries[(i + 1) % boundaries.Count];
				Vec2 vec3;
				float closestPointOnLineSegment = MBMath.GetClosestPointOnLineSegment(position, vec, vec2, out vec3);
				if (closestPointOnLineSegment <= num)
				{
					num = closestPointOnLineSegment;
					closestPoint = vec3;
				}
			}
			return num;
		}

		// Token: 0x04000B89 RID: 2953
		public const int MaxNumberOfSpawnPaths = 32;

		// Token: 0x04000B8A RID: 2954
		public const string SpawnPathPrefix = "spawn_path_";

		// Token: 0x04000B8B RID: 2955
		public const string SoftBorderVertexTag = "walk_area_vertex";

		// Token: 0x04000B8C RID: 2956
		public const string SoftBoundaryName = "walk_area";

		// Token: 0x04000B8D RID: 2957
		public const string SceneBoundaryName = "scene_boundary";
	}
}
