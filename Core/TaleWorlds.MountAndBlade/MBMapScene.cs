﻿using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class MBMapScene
	{
		public static Vec2 GetAccessiblePointNearPosition(Scene mapScene, Vec2 position, float radius)
		{
			return MBAPI.IMBMapScene.GetAccessiblePointNearPosition(mapScene.Pointer, position, radius).AsVec2;
		}

		public static void RemoveZeroCornerBodies(Scene mapScene)
		{
			MBAPI.IMBMapScene.RemoveZeroCornerBodies(mapScene.Pointer);
		}

		public static void LoadAtmosphereData(Scene mapScene)
		{
			MBAPI.IMBMapScene.LoadAtmosphereData(mapScene.Pointer);
		}

		public static void GetFaceIndexForMultiplePositions(Scene mapScene, int movedPartyCount, float[] positionArray, PathFaceRecord[] resultArray, bool check_if_disabled, bool check_height)
		{
			MBAPI.IMBMapScene.GetFaceIndexForMultiplePositions(mapScene.Pointer, movedPartyCount, positionArray, resultArray, check_if_disabled, check_height);
		}

		public static void SetSoundParameters(Scene mapScene, float tod, int season, float cameraHeight)
		{
			MBAPI.IMBMapScene.SetSoundParameters(mapScene.Pointer, tod, season, cameraHeight);
		}

		public static void TickStepSound(Scene mapScene, MBAgentVisuals visuals, int terrainType, int soundType)
		{
			MBAPI.IMBMapScene.TickStepSound(mapScene.Pointer, visuals.Pointer, terrainType, soundType);
		}

		public static void TickAmbientSounds(Scene mapScene, int terrainType)
		{
			MBAPI.IMBMapScene.TickAmbientSounds(mapScene.Pointer, terrainType);
		}

		public static bool GetMouseVisible()
		{
			return MBAPI.IMBMapScene.GetMouseVisible();
		}

		public static void SendMouseKeyEvent(int mouseKeyId, bool isDown)
		{
			MBAPI.IMBMapScene.SendMouseKeyEvent(mouseKeyId, isDown);
		}

		public static void SetMousePos(int posX, int posY)
		{
			MBAPI.IMBMapScene.SetMousePos(posX, posY);
		}

		public static void TickVisuals(Scene mapScene, float tod, Mesh[] tickedMapMeshes)
		{
			for (int i = 0; i < tickedMapMeshes.Length; i++)
			{
				MBMapScene._tickedMapMeshesCachedArray[i] = tickedMapMeshes[i].Pointer;
			}
			MBAPI.IMBMapScene.TickVisuals(mapScene.Pointer, tod, MBMapScene._tickedMapMeshesCachedArray, tickedMapMeshes.Length);
		}

		public static void ValidateTerrainSoundIds()
		{
			MBAPI.IMBMapScene.ValidateTerrainSoundIds();
		}

		public static void GetGlobalIlluminationOfString(Scene mapScene, string value)
		{
			MBAPI.IMBMapScene.SetPoliticalColor(mapScene.Pointer, value);
		}

		public static void GetColorGradeGridData(Scene mapScene, byte[] gridData, string textureName)
		{
			MBAPI.IMBMapScene.GetColorGradeGridData(mapScene.Pointer, gridData, textureName);
		}

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

		public static void SetFrameForAtmosphere(Scene mapScene, float tod, float cameraElevation, bool forceLoadTextures)
		{
			MBAPI.IMBMapScene.SetFrameForAtmosphere(mapScene.Pointer, tod, cameraElevation, forceLoadTextures);
		}

		public static void SetTerrainDynamicParams(Scene mapScene, Vec3 dynamic_params)
		{
			MBAPI.IMBMapScene.SetTerrainDynamicParams(mapScene.Pointer, dynamic_params);
		}

		public static void SetSeasonTimeFactor(Scene mapScene, float seasonTimeFactor)
		{
			MBAPI.IMBMapScene.SetSeasonTimeFactor(mapScene.Pointer, seasonTimeFactor);
		}

		public static float GetSeasonTimeFactor(Scene mapScene)
		{
			return MBAPI.IMBMapScene.GetSeasonTimeFactor(mapScene.Pointer);
		}

		private static UIntPtr[] _tickedMapMeshesCachedArray = new UIntPtr[1024];
	}
}
