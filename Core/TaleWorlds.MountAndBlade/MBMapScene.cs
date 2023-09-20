using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001B7 RID: 439
	public static class MBMapScene
	{
		// Token: 0x06001974 RID: 6516 RVA: 0x0005B740 File Offset: 0x00059940
		public static Vec2 GetAccessiblePointNearPosition(Scene mapScene, Vec2 position, float radius)
		{
			return MBAPI.IMBMapScene.GetAccessiblePointNearPosition(mapScene.Pointer, position, radius).AsVec2;
		}

		// Token: 0x06001975 RID: 6517 RVA: 0x0005B767 File Offset: 0x00059967
		public static void RemoveZeroCornerBodies(Scene mapScene)
		{
			MBAPI.IMBMapScene.RemoveZeroCornerBodies(mapScene.Pointer);
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x0005B779 File Offset: 0x00059979
		public static void LoadAtmosphereData(Scene mapScene)
		{
			MBAPI.IMBMapScene.LoadAtmosphereData(mapScene.Pointer);
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x0005B78B File Offset: 0x0005998B
		public static void GetFaceIndexForMultiplePositions(Scene mapScene, int movedPartyCount, float[] positionArray, PathFaceRecord[] resultArray, bool check_if_disabled, bool check_height)
		{
			MBAPI.IMBMapScene.GetFaceIndexForMultiplePositions(mapScene.Pointer, movedPartyCount, positionArray, resultArray, check_if_disabled, check_height);
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x0005B7A4 File Offset: 0x000599A4
		public static void SetSoundParameters(Scene mapScene, float tod, int season, float cameraHeight)
		{
			MBAPI.IMBMapScene.SetSoundParameters(mapScene.Pointer, tod, season, cameraHeight);
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x0005B7B9 File Offset: 0x000599B9
		public static void TickStepSound(Scene mapScene, MBAgentVisuals visuals, int terrainType, int soundType)
		{
			MBAPI.IMBMapScene.TickStepSound(mapScene.Pointer, visuals.Pointer, terrainType, soundType);
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x0005B7D3 File Offset: 0x000599D3
		public static void TickAmbientSounds(Scene mapScene, int terrainType)
		{
			MBAPI.IMBMapScene.TickAmbientSounds(mapScene.Pointer, terrainType);
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x0005B7E6 File Offset: 0x000599E6
		public static bool GetMouseVisible()
		{
			return MBAPI.IMBMapScene.GetMouseVisible();
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x0005B7F2 File Offset: 0x000599F2
		public static void SendMouseKeyEvent(int mouseKeyId, bool isDown)
		{
			MBAPI.IMBMapScene.SendMouseKeyEvent(mouseKeyId, isDown);
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x0005B800 File Offset: 0x00059A00
		public static void SetMousePos(int posX, int posY)
		{
			MBAPI.IMBMapScene.SetMousePos(posX, posY);
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x0005B810 File Offset: 0x00059A10
		public static void TickVisuals(Scene mapScene, float tod, Mesh[] tickedMapMeshes)
		{
			for (int i = 0; i < tickedMapMeshes.Length; i++)
			{
				MBMapScene._tickedMapMeshesCachedArray[i] = tickedMapMeshes[i].Pointer;
			}
			MBAPI.IMBMapScene.TickVisuals(mapScene.Pointer, tod, MBMapScene._tickedMapMeshesCachedArray, tickedMapMeshes.Length);
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x0005B853 File Offset: 0x00059A53
		public static void ValidateTerrainSoundIds()
		{
			MBAPI.IMBMapScene.ValidateTerrainSoundIds();
		}

		// Token: 0x06001980 RID: 6528 RVA: 0x0005B85F File Offset: 0x00059A5F
		public static void GetGlobalIlluminationOfString(Scene mapScene, string value)
		{
			MBAPI.IMBMapScene.SetPoliticalColor(mapScene.Pointer, value);
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x0005B872 File Offset: 0x00059A72
		public static void GetColorGradeGridData(Scene mapScene, byte[] gridData, string textureName)
		{
			MBAPI.IMBMapScene.GetColorGradeGridData(mapScene.Pointer, gridData, textureName);
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x0005B888 File Offset: 0x00059A88
		public static void GetBattleSceneIndexMap(Scene mapScene, ref byte[] indexData, ref int width, ref int height)
		{
			MBAPI.IMBMapScene.GetBattleSceneIndexMapResolution(mapScene.Pointer, ref width, ref height);
			int num = width * height * 2;
			if (indexData == null || indexData.Length != num)
			{
				indexData = new byte[num];
			}
			MBAPI.IMBMapScene.GetBattleSceneIndexMap(mapScene.Pointer, indexData);
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x0005B8D4 File Offset: 0x00059AD4
		public static void SetFrameForAtmosphere(Scene mapScene, float tod, float cameraElevation, bool forceLoadTextures)
		{
			MBAPI.IMBMapScene.SetFrameForAtmosphere(mapScene.Pointer, tod, cameraElevation, forceLoadTextures);
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x0005B8E9 File Offset: 0x00059AE9
		public static void SetTerrainDynamicParams(Scene mapScene, Vec3 dynamic_params)
		{
			MBAPI.IMBMapScene.SetTerrainDynamicParams(mapScene.Pointer, dynamic_params);
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x0005B8FC File Offset: 0x00059AFC
		public static void SetSeasonTimeFactor(Scene mapScene, float seasonTimeFactor)
		{
			MBAPI.IMBMapScene.SetSeasonTimeFactor(mapScene.Pointer, seasonTimeFactor);
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x0005B90F File Offset: 0x00059B0F
		public static float GetSeasonTimeFactor(Scene mapScene)
		{
			return MBAPI.IMBMapScene.GetSeasonTimeFactor(mapScene.Pointer);
		}

		// Token: 0x040007C6 RID: 1990
		private static UIntPtr[] _tickedMapMeshesCachedArray = new UIntPtr[1024];
	}
}
