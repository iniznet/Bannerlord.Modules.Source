using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200007B RID: 123
	[EngineClass("rglScene")]
	public sealed class Scene : NativeObject
	{
		// Token: 0x060008C7 RID: 2247 RVA: 0x00008F1D File Offset: 0x0000711D
		private Scene()
		{
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x00008F25 File Offset: 0x00007125
		internal Scene(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x00008F34 File Offset: 0x00007134
		public bool IsMultiplayerScene()
		{
			return EngineApplicationInterface.IScene.IsMultiplayerScene(this);
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00008F41 File Offset: 0x00007141
		public string TakePhotoModePicture(bool saveAmbientOcclusionPass, bool savingObjectIdPass, bool saveShadowPass)
		{
			return EngineApplicationInterface.IScene.TakePhotoModePicture(this, saveAmbientOcclusionPass, savingObjectIdPass, saveShadowPass);
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x00008F51 File Offset: 0x00007151
		public string GetAllColorGradeNames()
		{
			return EngineApplicationInterface.IScene.GetAllColorGradeNames(this);
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x00008F5E File Offset: 0x0000715E
		public string GetAllFilterNames()
		{
			return EngineApplicationInterface.IScene.GetAllFilterNames(this);
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x00008F6B File Offset: 0x0000716B
		public float GetPhotoModeRoll()
		{
			return EngineApplicationInterface.IScene.GetPhotoModeRoll(this);
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x00008F78 File Offset: 0x00007178
		public bool GetPhotoModeOrbit()
		{
			return EngineApplicationInterface.IScene.GetPhotoModeOrbit(this);
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x00008F85 File Offset: 0x00007185
		public bool GetPhotoModeOn()
		{
			return EngineApplicationInterface.IScene.GetPhotoModeOn(this);
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x00008F92 File Offset: 0x00007192
		public void GetPhotoModeFocus(ref float focus, ref float focusStart, ref float focusEnd, ref float exposure, ref bool vignetteOn)
		{
			EngineApplicationInterface.IScene.GetPhotoModeFocus(this, ref focus, ref focusStart, ref focusEnd, ref exposure, ref vignetteOn);
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x00008FA6 File Offset: 0x000071A6
		public int GetSceneColorGradeIndex()
		{
			return EngineApplicationInterface.IScene.GetSceneColorGradeIndex(this);
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x00008FB3 File Offset: 0x000071B3
		public int GetSceneFilterIndex()
		{
			return EngineApplicationInterface.IScene.GetSceneFilterIndex(this);
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x00008FC0 File Offset: 0x000071C0
		public string GetLoadingStateName()
		{
			return EngineApplicationInterface.IScene.GetLoadingStateName(this);
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x00008FCD File Offset: 0x000071CD
		public void SetPhotoModeRoll(float roll)
		{
			EngineApplicationInterface.IScene.SetPhotoModeRoll(this, roll);
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x00008FDB File Offset: 0x000071DB
		public void SetPhotoModeOrbit(bool orbit)
		{
			EngineApplicationInterface.IScene.SetPhotoModeOrbit(this, orbit);
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x00008FE9 File Offset: 0x000071E9
		public void SetPhotoModeOn(bool on)
		{
			EngineApplicationInterface.IScene.SetPhotoModeOn(this, on);
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x00008FF7 File Offset: 0x000071F7
		public void SetPhotoModeFocus(float focusStart, float focusEnd, float focus, float exposure)
		{
			EngineApplicationInterface.IScene.SetPhotoModeFocus(this, focusStart, focusEnd, focus, exposure);
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x00009009 File Offset: 0x00007209
		public void SetPhotoModeFov(float verticalFov)
		{
			EngineApplicationInterface.IScene.SetPhotoModeFov(this, verticalFov);
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x00009017 File Offset: 0x00007217
		public float GetPhotoModeFov()
		{
			return EngineApplicationInterface.IScene.GetPhotoModeFov(this);
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x00009024 File Offset: 0x00007224
		public void SetPhotoModeVignette(bool vignetteOn)
		{
			EngineApplicationInterface.IScene.SetPhotoModeVignette(this, vignetteOn);
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x00009032 File Offset: 0x00007232
		public void SetSceneColorGradeIndex(int index)
		{
			EngineApplicationInterface.IScene.SetSceneColorGradeIndex(this, index);
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x00009040 File Offset: 0x00007240
		public int SetSceneFilterIndex(int index)
		{
			return EngineApplicationInterface.IScene.SetSceneFilterIndex(this, index);
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x0000904E File Offset: 0x0000724E
		public void SetSceneColorGrade(string textureName)
		{
			EngineApplicationInterface.IScene.SetSceneColorGrade(this, textureName);
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x0000905C File Offset: 0x0000725C
		public void SetUpgradeLevel(int level)
		{
			EngineApplicationInterface.IScene.SetUpgradeLevel(base.Pointer, level);
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x0000906F File Offset: 0x0000726F
		public void CreateBurstParticle(int particleId, MatrixFrame frame)
		{
			EngineApplicationInterface.IScene.CreateBurstParticle(this, particleId, ref frame);
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x00009080 File Offset: 0x00007280
		public float[] GetTerrainHeightData(int nodeXIndex, int nodeYIndex)
		{
			float[] array = new float[EngineApplicationInterface.IScene.GetNodeDataCount(this, nodeXIndex, nodeYIndex)];
			EngineApplicationInterface.IScene.FillTerrainHeightData(this, nodeXIndex, nodeYIndex, array);
			return array;
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x000090B0 File Offset: 0x000072B0
		public short[] GetTerrainPhysicsMaterialIndexData(int nodeXIndex, int nodeYIndex)
		{
			short[] array = new short[EngineApplicationInterface.IScene.GetNodeDataCount(this, nodeXIndex, nodeYIndex)];
			EngineApplicationInterface.IScene.FillTerrainPhysicsMaterialIndexData(this, nodeXIndex, nodeYIndex, array);
			return array;
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x000090DF File Offset: 0x000072DF
		public void GetTerrainData(out Vec2i nodeDimension, out float nodeSize, out int layerCount, out int layerVersion)
		{
			EngineApplicationInterface.IScene.GetTerrainData(this, out nodeDimension, out nodeSize, out layerCount, out layerVersion);
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x000090F1 File Offset: 0x000072F1
		public void GetTerrainNodeData(int xIndex, int yIndex, out int vertexCountAlongAxis, out float quadLength, out float minHeight, out float maxHeight)
		{
			EngineApplicationInterface.IScene.GetTerrainNodeData(this, xIndex, yIndex, out vertexCountAlongAxis, out quadLength, out minHeight, out maxHeight);
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x00009108 File Offset: 0x00007308
		public PhysicsMaterial GetTerrainPhysicsMaterialAtLayer(int layerIndex)
		{
			int terrainPhysicsMaterialIndexAtLayer = EngineApplicationInterface.IScene.GetTerrainPhysicsMaterialIndexAtLayer(this, layerIndex);
			return new PhysicsMaterial(terrainPhysicsMaterialIndexAtLayer);
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x00009128 File Offset: 0x00007328
		public void SetSceneColorGrade(Scene scene, string textureName)
		{
			EngineApplicationInterface.IScene.SetSceneColorGrade(scene, textureName);
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x00009136 File Offset: 0x00007336
		public float GetWaterLevel()
		{
			return EngineApplicationInterface.IScene.GetWaterLevel(this);
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x00009143 File Offset: 0x00007343
		public float GetWaterLevelAtPosition(Vec2 position, bool checkWaterBodyEntities)
		{
			return EngineApplicationInterface.IScene.GetWaterLevelAtPosition(this, position, checkWaterBodyEntities);
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x00009154 File Offset: 0x00007354
		public bool GetPathBetweenAIFaces(UIntPtr startingFace, UIntPtr endingFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, NavigationPath path)
		{
			int num = path.PathPoints.Length;
			if (EngineApplicationInterface.IScene.GetPathBetweenAIFacePointers(base.Pointer, startingFace, endingFace, startingPosition, endingPosition, agentRadius, path.PathPoints, ref num))
			{
				path.Size = num;
				return true;
			}
			path.Size = 0;
			return false;
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x000091A0 File Offset: 0x000073A0
		public bool GetPathBetweenAIFaces(int startingFace, int endingFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, NavigationPath path)
		{
			int num = path.PathPoints.Length;
			if (EngineApplicationInterface.IScene.GetPathBetweenAIFaceIndices(base.Pointer, startingFace, endingFace, startingPosition, endingPosition, agentRadius, path.PathPoints, ref num))
			{
				path.Size = num;
				return true;
			}
			path.Size = 0;
			return false;
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x000091EC File Offset: 0x000073EC
		public bool GetPathDistanceBetweenAIFaces(int startingAiFace, int endingAiFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, float distanceLimit, out float distance)
		{
			return EngineApplicationInterface.IScene.GetPathDistanceBetweenAIFaces(base.Pointer, startingAiFace, endingAiFace, startingPosition, endingPosition, agentRadius, distanceLimit, out distance);
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x00009214 File Offset: 0x00007414
		public void GetNavMeshFaceIndex(ref PathFaceRecord record, Vec2 position, bool checkIfDisabled, bool ignoreHeight = false)
		{
			EngineApplicationInterface.IScene.GetNavMeshFaceIndex(base.Pointer, ref record, position, checkIfDisabled, ignoreHeight);
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x0000922B File Offset: 0x0000742B
		public void GetNavMeshFaceIndex(ref PathFaceRecord record, Vec3 position, bool checkIfDisabled)
		{
			EngineApplicationInterface.IScene.GetNavMeshFaceIndex3(base.Pointer, ref record, position, checkIfDisabled);
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x00009240 File Offset: 0x00007440
		public static Scene CreateNewScene(bool initialize_physics = true, bool enable_decals = true, DecalAtlasGroup atlasGroup = DecalAtlasGroup.All, string sceneName = "mono_renderscene")
		{
			return EngineApplicationInterface.IScene.CreateNewScene(initialize_physics, enable_decals, (int)atlasGroup, sceneName);
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x00009250 File Offset: 0x00007450
		public MetaMesh CreatePathMesh(string baseEntityName, bool isWaterPath)
		{
			return EngineApplicationInterface.IScene.CreatePathMesh(base.Pointer, baseEntityName, isWaterPath);
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x00009264 File Offset: 0x00007464
		public void SetActiveVisibilityLevels(List<string> levelsToActivate)
		{
			string text = "";
			for (int i = 0; i < levelsToActivate.Count; i++)
			{
				if (!levelsToActivate[i].Contains("$"))
				{
					if (i != 0)
					{
						text += "$";
					}
					text += levelsToActivate[i];
				}
			}
			EngineApplicationInterface.IScene.SetActiveVisibilityLevels(base.Pointer, text);
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x000092C9 File Offset: 0x000074C9
		public void SetDoNotWaitForLoadingStatesToRender(bool value)
		{
			EngineApplicationInterface.IScene.SetDoNotWaitForLoadingStatesToRender(base.Pointer, value);
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x000092DC File Offset: 0x000074DC
		public MetaMesh CreatePathMesh(IList<GameEntity> pathNodes, bool isWaterPath = false)
		{
			return EngineApplicationInterface.IScene.CreatePathMesh2(base.Pointer, pathNodes.Select((GameEntity e) => e.Pointer).ToArray<UIntPtr>(), pathNodes.Count, isWaterPath);
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x0000932A File Offset: 0x0000752A
		public GameEntity GetEntityWithGuid(string guid)
		{
			return EngineApplicationInterface.IScene.GetEntityWithGuid(base.Pointer, guid);
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x0000933D File Offset: 0x0000753D
		public bool IsEntityFrameChanged(string containsName)
		{
			return EngineApplicationInterface.IScene.CheckPathEntitiesFrameChanged(base.Pointer, containsName);
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x00009350 File Offset: 0x00007550
		public void GetTerrainHeightAndNormal(Vec2 position, out float height, out Vec3 normal)
		{
			EngineApplicationInterface.IScene.GetTerrainHeightAndNormal(base.Pointer, position, out height, out normal);
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x00009365 File Offset: 0x00007565
		public int GetFloraInstanceCount()
		{
			return EngineApplicationInterface.IScene.GetFloraInstanceCount(base.Pointer);
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x00009377 File Offset: 0x00007577
		public int GetFloraRendererTextureUsage()
		{
			return EngineApplicationInterface.IScene.GetFloraRendererTextureUsage(base.Pointer);
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x00009389 File Offset: 0x00007589
		public int GetTerrainMemoryUsage()
		{
			return EngineApplicationInterface.IScene.GetTerrainMemoryUsage(base.Pointer);
		}

		// Token: 0x060008F8 RID: 2296 RVA: 0x0000939B File Offset: 0x0000759B
		public void StallLoadingRenderingsUntilFurtherNotice()
		{
			EngineApplicationInterface.IScene.StallLoadingRenderingsUntilFurtherNotice(base.Pointer);
		}

		// Token: 0x060008F9 RID: 2297 RVA: 0x000093AD File Offset: 0x000075AD
		public int GetNavMeshFaceCount()
		{
			return EngineApplicationInterface.IScene.GetNavMeshFaceCount(base.Pointer);
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x000093BF File Offset: 0x000075BF
		public void ResumeLoadingRenderings()
		{
			EngineApplicationInterface.IScene.ResumeLoadingRenderings(base.Pointer);
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x000093D1 File Offset: 0x000075D1
		public uint GetUpgradeLevelMask()
		{
			return EngineApplicationInterface.IScene.GetUpgradeLevelMask(base.Pointer);
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x000093E3 File Offset: 0x000075E3
		public void SetUpgradeLevelVisibility(uint mask)
		{
			EngineApplicationInterface.IScene.SetUpgradeLevelVisibilityWithMask(base.Pointer, mask);
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x000093F8 File Offset: 0x000075F8
		public void SetUpgradeLevelVisibility(List<string> levels)
		{
			string text = "";
			for (int i = 0; i < levels.Count - 1; i++)
			{
				text = text + levels[i] + "|";
			}
			text += levels[levels.Count - 1];
			EngineApplicationInterface.IScene.SetUpgradeLevelVisibility(base.Pointer, text);
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x00009457 File Offset: 0x00007657
		public int GetIdOfNavMeshFace(int faceIndex)
		{
			return EngineApplicationInterface.IScene.GetIdOfNavMeshFace(base.Pointer, faceIndex);
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x0000946A File Offset: 0x0000766A
		public void SetClothSimulationState(bool state)
		{
			EngineApplicationInterface.IScene.SetClothSimulationState(base.Pointer, state);
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x0000947D File Offset: 0x0000767D
		public void GetNavMeshCenterPosition(int faceIndex, ref Vec3 centerPosition)
		{
			EngineApplicationInterface.IScene.GetNavMeshFaceCenterPosition(base.Pointer, faceIndex, ref centerPosition);
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x00009491 File Offset: 0x00007691
		public GameEntity GetFirstEntityWithName(string name)
		{
			return EngineApplicationInterface.IScene.GetFirstEntityWithName(base.Pointer, name);
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x000094A4 File Offset: 0x000076A4
		public GameEntity GetCampaignEntityWithName(string name)
		{
			return EngineApplicationInterface.IScene.GetCampaignEntityWithName(base.Pointer, name);
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x000094B8 File Offset: 0x000076B8
		public void GetAllEntitiesWithScriptComponent<T>(ref List<GameEntity> entities) where T : ScriptComponentBehavior
		{
			NativeObjectArray nativeObjectArray = NativeObjectArray.Create();
			string name = typeof(T).Name;
			EngineApplicationInterface.IScene.GetAllEntitiesWithScriptComponent(base.Pointer, name, nativeObjectArray.Pointer);
			for (int i = 0; i < nativeObjectArray.Count; i++)
			{
				entities.Add(nativeObjectArray.GetElementAt(i) as GameEntity);
			}
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x00009518 File Offset: 0x00007718
		public GameEntity GetFirstEntityWithScriptComponent<T>() where T : ScriptComponentBehavior
		{
			string name = typeof(T).Name;
			return EngineApplicationInterface.IScene.GetFirstEntityWithScriptComponent(base.Pointer, name);
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x00009546 File Offset: 0x00007746
		public uint GetUpgradeLevelMaskOfLevelName(string levelName)
		{
			return EngineApplicationInterface.IScene.GetUpgradeLevelMaskOfLevelName(base.Pointer, levelName);
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x00009559 File Offset: 0x00007759
		public string GetUpgradeLevelNameOfIndex(int index)
		{
			return EngineApplicationInterface.IScene.GetUpgradeLevelNameOfIndex(base.Pointer, index);
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0000956C File Offset: 0x0000776C
		public int GetUpgradeLevelCount()
		{
			return EngineApplicationInterface.IScene.GetUpgradeLevelCount(base.Pointer);
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x0000957E File Offset: 0x0000777E
		public float GetWinterTimeFactor()
		{
			return EngineApplicationInterface.IScene.GetWinterTimeFactor(base.Pointer);
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x00009590 File Offset: 0x00007790
		public float GetNavMeshFaceFirstVertexZ(int faceIndex)
		{
			return EngineApplicationInterface.IScene.GetNavMeshFaceFirstVertexZ(base.Pointer, faceIndex);
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x000095A3 File Offset: 0x000077A3
		public void SetWinterTimeFactor(float winterTimeFactor)
		{
			EngineApplicationInterface.IScene.SetWinterTimeFactor(base.Pointer, winterTimeFactor);
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x000095B6 File Offset: 0x000077B6
		public void SetDrynessFactor(float drynessFactor)
		{
			EngineApplicationInterface.IScene.SetDrynessFactor(base.Pointer, drynessFactor);
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x000095C9 File Offset: 0x000077C9
		public float GetFog()
		{
			return EngineApplicationInterface.IScene.GetFog(base.Pointer);
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x000095DB File Offset: 0x000077DB
		public void SetFog(float fogDensity, ref Vec3 fogColor, float fogFalloff)
		{
			EngineApplicationInterface.IScene.SetFog(base.Pointer, fogDensity, ref fogColor, fogFalloff);
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x000095F0 File Offset: 0x000077F0
		public void SetFogAdvanced(float fogFalloffOffset, float fogFalloffMinFog, float fogFalloffStartDist)
		{
			EngineApplicationInterface.IScene.SetFogAdvanced(base.Pointer, fogFalloffOffset, fogFalloffMinFog, fogFalloffStartDist);
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x00009605 File Offset: 0x00007805
		public void SetFogAmbientColor(ref Vec3 fogAmbientColor)
		{
			EngineApplicationInterface.IScene.SetFogAmbientColor(base.Pointer, ref fogAmbientColor);
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x00009618 File Offset: 0x00007818
		public void SetTemperature(float temperature)
		{
			EngineApplicationInterface.IScene.SetTemperature(base.Pointer, temperature);
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x0000962B File Offset: 0x0000782B
		public void SetHumidity(float humidity)
		{
			EngineApplicationInterface.IScene.SetHumidity(base.Pointer, humidity);
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x0000963E File Offset: 0x0000783E
		public void SetDynamicShadowmapCascadesRadiusMultiplier(float multiplier)
		{
			EngineApplicationInterface.IScene.SetDynamicShadowmapCascadesRadiusMultiplier(base.Pointer, multiplier);
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x00009651 File Offset: 0x00007851
		public void SetEnvironmentMultiplier(bool useMultiplier, float multiplier)
		{
			EngineApplicationInterface.IScene.SetEnvironmentMultiplier(base.Pointer, useMultiplier, multiplier);
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x00009665 File Offset: 0x00007865
		public void SetSkyRotation(float rotation)
		{
			EngineApplicationInterface.IScene.SetSkyRotation(base.Pointer, rotation);
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x00009678 File Offset: 0x00007878
		public void SetSkyBrightness(float brightness)
		{
			EngineApplicationInterface.IScene.SetSkyBrightness(base.Pointer, brightness);
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x0000968B File Offset: 0x0000788B
		public void SetForcedSnow(bool value)
		{
			EngineApplicationInterface.IScene.SetForcedSnow(base.Pointer, value);
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x0000969E File Offset: 0x0000789E
		public void SetSunLight(ref Vec3 color, ref Vec3 direction)
		{
			EngineApplicationInterface.IScene.SetSunLight(base.Pointer, color, direction);
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x000096BC File Offset: 0x000078BC
		public void SetSunDirection(ref Vec3 direction)
		{
			EngineApplicationInterface.IScene.SetSunDirection(base.Pointer, direction);
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x000096D4 File Offset: 0x000078D4
		public void SetSun(ref Vec3 color, float altitude, float angle, float intensity)
		{
			EngineApplicationInterface.IScene.SetSun(base.Pointer, color, altitude, angle, intensity);
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x000096F0 File Offset: 0x000078F0
		public void SetSunAngleAltitude(float angle, float altitude)
		{
			EngineApplicationInterface.IScene.SetSunAngleAltitude(base.Pointer, angle, altitude);
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x00009704 File Offset: 0x00007904
		public void SetSunSize(float size)
		{
			EngineApplicationInterface.IScene.SetSunSize(base.Pointer, size);
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x00009717 File Offset: 0x00007917
		public void SetSunShaftStrength(float strength)
		{
			EngineApplicationInterface.IScene.SetSunShaftStrength(base.Pointer, strength);
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x0000972A File Offset: 0x0000792A
		public float GetRainDensity()
		{
			return EngineApplicationInterface.IScene.GetRainDensity(base.Pointer);
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x0000973C File Offset: 0x0000793C
		public void SetRainDensity(float density)
		{
			EngineApplicationInterface.IScene.SetRainDensity(base.Pointer, density);
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x0000974F File Offset: 0x0000794F
		public float GetSnowDensity()
		{
			return EngineApplicationInterface.IScene.GetSnowDensity(base.Pointer);
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x00009761 File Offset: 0x00007961
		public void SetSnowDensity(float density)
		{
			EngineApplicationInterface.IScene.SetSnowDensity(base.Pointer, density);
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x00009774 File Offset: 0x00007974
		public void AddDecalInstance(Decal decal, string decalSetID, bool deletable)
		{
			EngineApplicationInterface.IScene.AddDecalInstance(base.Pointer, decal.Pointer, decalSetID, deletable);
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x0000978E File Offset: 0x0000798E
		public void SetShadow(bool shadowEnabled)
		{
			EngineApplicationInterface.IScene.SetShadow(base.Pointer, shadowEnabled);
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x000097A1 File Offset: 0x000079A1
		public int AddPointLight(ref Vec3 position, float radius)
		{
			return EngineApplicationInterface.IScene.AddPointLight(base.Pointer, position, radius);
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x000097BA File Offset: 0x000079BA
		public int AddDirectionalLight(ref Vec3 position, ref Vec3 direction, float radius)
		{
			return EngineApplicationInterface.IScene.AddDirectionalLight(base.Pointer, position, direction, radius);
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x000097D9 File Offset: 0x000079D9
		public void SetLightPosition(int lightIndex, ref Vec3 position)
		{
			EngineApplicationInterface.IScene.SetLightPosition(base.Pointer, lightIndex, position);
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x000097F2 File Offset: 0x000079F2
		public void SetLightDiffuseColor(int lightIndex, ref Vec3 diffuseColor)
		{
			EngineApplicationInterface.IScene.SetLightDiffuseColor(base.Pointer, lightIndex, diffuseColor);
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x0000980B File Offset: 0x00007A0B
		public void SetLightDirection(int lightIndex, ref Vec3 direction)
		{
			EngineApplicationInterface.IScene.SetLightDirection(base.Pointer, lightIndex, direction);
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x00009824 File Offset: 0x00007A24
		public void SetMieScatterFocus(float strength)
		{
			EngineApplicationInterface.IScene.SetMieScatterFocus(base.Pointer, strength);
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x00009837 File Offset: 0x00007A37
		public void SetMieScatterStrength(float strength)
		{
			EngineApplicationInterface.IScene.SetMieScatterStrength(base.Pointer, strength);
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x0000984A File Offset: 0x00007A4A
		public void SetBrightpassThreshold(float threshold)
		{
			EngineApplicationInterface.IScene.SetBrightpassTreshold(base.Pointer, threshold);
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x0000985D File Offset: 0x00007A5D
		public void SetLensDistortion(float amount)
		{
			EngineApplicationInterface.IScene.SetLensDistortion(base.Pointer, amount);
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x00009870 File Offset: 0x00007A70
		public void SetHexagonVignetteAlpha(float amount)
		{
			EngineApplicationInterface.IScene.SetHexagonVignetteAlpha(base.Pointer, amount);
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x00009883 File Offset: 0x00007A83
		public void SetMinExposure(float minExposure)
		{
			EngineApplicationInterface.IScene.SetMinExposure(base.Pointer, minExposure);
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x00009896 File Offset: 0x00007A96
		public void SetMaxExposure(float maxExposure)
		{
			EngineApplicationInterface.IScene.SetMaxExposure(base.Pointer, maxExposure);
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x000098A9 File Offset: 0x00007AA9
		public void SetTargetExposure(float targetExposure)
		{
			EngineApplicationInterface.IScene.SetTargetExposure(base.Pointer, targetExposure);
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x000098BC File Offset: 0x00007ABC
		public void SetMiddleGray(float middleGray)
		{
			EngineApplicationInterface.IScene.SetMiddleGray(base.Pointer, middleGray);
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x000098CF File Offset: 0x00007ACF
		public void SetBloomStrength(float bloomStrength)
		{
			EngineApplicationInterface.IScene.SetBloomStrength(base.Pointer, bloomStrength);
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x000098E2 File Offset: 0x00007AE2
		public void SetBloomAmount(float bloomAmount)
		{
			EngineApplicationInterface.IScene.SetBloomAmount(base.Pointer, bloomAmount);
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x000098F5 File Offset: 0x00007AF5
		public void SetGrainAmount(float grainAmount)
		{
			EngineApplicationInterface.IScene.SetGrainAmount(base.Pointer, grainAmount);
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x00009908 File Offset: 0x00007B08
		public GameEntity AddItemEntity(ref MatrixFrame placementFrame, MetaMesh metaMesh)
		{
			return EngineApplicationInterface.IScene.AddItemEntity(base.Pointer, ref placementFrame, metaMesh.Pointer);
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x00009921 File Offset: 0x00007B21
		public void RemoveEntity(GameEntity entity, int removeReason)
		{
			EngineApplicationInterface.IScene.RemoveEntity(base.Pointer, entity.Pointer, removeReason);
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x0000993A File Offset: 0x00007B3A
		public bool AttachEntity(GameEntity entity, bool showWarnings = false)
		{
			return EngineApplicationInterface.IScene.AttachEntity(base.Pointer, entity.Pointer, showWarnings);
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x00009953 File Offset: 0x00007B53
		public void AddEntityWithMesh(Mesh mesh, ref MatrixFrame frame)
		{
			EngineApplicationInterface.IScene.AddEntityWithMesh(base.Pointer, mesh.Pointer, ref frame);
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x0000996C File Offset: 0x00007B6C
		public void AddEntityWithMultiMesh(MetaMesh mesh, ref MatrixFrame frame)
		{
			EngineApplicationInterface.IScene.AddEntityWithMultiMesh(base.Pointer, mesh.Pointer, ref frame);
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x00009985 File Offset: 0x00007B85
		public void Tick(float dt)
		{
			EngineApplicationInterface.IScene.Tick(base.Pointer, dt);
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x00009998 File Offset: 0x00007B98
		public void ClearAll()
		{
			EngineApplicationInterface.IScene.ClearAll(base.Pointer);
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x000099AC File Offset: 0x00007BAC
		public void SetDefaultLighting()
		{
			Vec3 vec = new Vec3(1.15f, 1.2f, 1.25f, -1f);
			Vec3 vec2 = new Vec3(1f, -1f, -1f, -1f);
			vec2.Normalize();
			this.SetSunLight(ref vec, ref vec2);
			this.SetShadow(false);
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x00009A08 File Offset: 0x00007C08
		public bool CalculateEffectiveLighting()
		{
			return EngineApplicationInterface.IScene.CalculateEffectiveLighting(base.Pointer);
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x00009A1A File Offset: 0x00007C1A
		public bool GetPathDistanceBetweenPositions(ref WorldPosition point0, ref WorldPosition point1, float agentRadius, out float pathDistance)
		{
			pathDistance = 0f;
			return EngineApplicationInterface.IScene.GetPathDistanceBetweenPositions(base.Pointer, ref point0, ref point1, agentRadius, ref pathDistance);
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x00009A39 File Offset: 0x00007C39
		public bool IsLineToPointClear(ref WorldPosition position, ref WorldPosition destination, float agentRadius)
		{
			return EngineApplicationInterface.IScene.IsLineToPointClear2(base.Pointer, position.GetNavMesh(), position.AsVec2, destination.AsVec2, agentRadius);
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x00009A5E File Offset: 0x00007C5E
		public bool IsLineToPointClear(int startingFace, Vec2 position, Vec2 destination, float agentRadius)
		{
			return EngineApplicationInterface.IScene.IsLineToPointClear(base.Pointer, startingFace, position, destination, agentRadius);
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x00009A75 File Offset: 0x00007C75
		public Vec2 GetLastPointOnNavigationMeshFromPositionToDestination(int startingFace, Vec2 position, Vec2 destination)
		{
			return EngineApplicationInterface.IScene.GetLastPointOnNavigationMeshFromPositionToDestination(base.Pointer, startingFace, position, destination);
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x00009A8A File Offset: 0x00007C8A
		public Vec3 GetLastPointOnNavigationMeshFromWorldPositionToDestination(ref WorldPosition position, Vec2 destination)
		{
			return EngineApplicationInterface.IScene.GetLastPointOnNavigationMeshFromWorldPositionToDestination(base.Pointer, ref position, destination);
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x00009A9E File Offset: 0x00007C9E
		public bool DoesPathExistBetweenFaces(int firstNavMeshFace, int secondNavMeshFace, bool ignoreDisabled)
		{
			return EngineApplicationInterface.IScene.DoesPathExistBetweenFaces(base.Pointer, firstNavMeshFace, secondNavMeshFace, ignoreDisabled);
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x00009AB3 File Offset: 0x00007CB3
		public bool GetHeightAtPoint(Vec2 point, BodyFlags excludeBodyFlags, ref float height)
		{
			return EngineApplicationInterface.IScene.GetHeightAtPoint(base.Pointer, point, excludeBodyFlags, ref height);
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x00009AC8 File Offset: 0x00007CC8
		public Vec3 GetNormalAt(Vec2 position)
		{
			return EngineApplicationInterface.IScene.GetNormalAt(base.Pointer, position);
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x00009ADC File Offset: 0x00007CDC
		public void GetEntities(ref List<GameEntity> entities)
		{
			NativeObjectArray nativeObjectArray = NativeObjectArray.Create();
			EngineApplicationInterface.IScene.GetEntities(base.Pointer, nativeObjectArray.Pointer);
			for (int i = 0; i < nativeObjectArray.Count; i++)
			{
				entities.Add(nativeObjectArray.GetElementAt(i) as GameEntity);
			}
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x00009B29 File Offset: 0x00007D29
		public void GetRootEntities(NativeObjectArray entities)
		{
			EngineApplicationInterface.IScene.GetRootEntities(this, entities);
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000947 RID: 2375 RVA: 0x00009B37 File Offset: 0x00007D37
		public int RootEntityCount
		{
			get
			{
				return EngineApplicationInterface.IScene.GetRootEntityCount(base.Pointer);
			}
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x00009B4C File Offset: 0x00007D4C
		public int SelectEntitiesInBoxWithScriptComponent<T>(ref Vec3 boundingBoxMin, ref Vec3 boundingBoxMax, GameEntity[] entitiesOutput, UIntPtr[] entityIds) where T : ScriptComponentBehavior
		{
			string name = typeof(T).Name;
			int num = EngineApplicationInterface.IScene.SelectEntitiesInBoxWithScriptComponent(base.Pointer, ref boundingBoxMin, ref boundingBoxMax, entityIds, entitiesOutput.Length, name);
			for (int i = 0; i < num; i++)
			{
				entitiesOutput[i] = new GameEntity(entityIds[i]);
			}
			return num;
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x00009B9B File Offset: 0x00007D9B
		public int SelectEntitiesCollidedWith(ref Ray ray, Intersection[] intersectionsOutput, UIntPtr[] entityIds)
		{
			return EngineApplicationInterface.IScene.SelectEntitiesCollidedWith(base.Pointer, ref ray, entityIds, intersectionsOutput);
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x00009BB0 File Offset: 0x00007DB0
		public int GenerateContactsWithCapsule(ref CapsuleData capsule, BodyFlags exclude_flags, Intersection[] intersectionsOutput)
		{
			return EngineApplicationInterface.IScene.GenerateContactsWithCapsule(base.Pointer, ref capsule, exclude_flags, intersectionsOutput);
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x00009BC5 File Offset: 0x00007DC5
		public void InvalidateTerrainPhysicsMaterials()
		{
			EngineApplicationInterface.IScene.InvalidateTerrainPhysicsMaterials(base.Pointer);
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x00009BD8 File Offset: 0x00007DD8
		public void Read(string sceneName)
		{
			SceneInitializationData sceneInitializationData = new SceneInitializationData(true);
			EngineApplicationInterface.IScene.Read(base.Pointer, sceneName, ref sceneInitializationData, "");
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x00009C05 File Offset: 0x00007E05
		public void Read(string sceneName, ref SceneInitializationData initData, string forcedAtmoName = "")
		{
			EngineApplicationInterface.IScene.Read(base.Pointer, sceneName, ref initData, forcedAtmoName);
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x00009C1C File Offset: 0x00007E1C
		public MatrixFrame ReadAndCalculateInitialCamera()
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.IScene.ReadAndCalculateInitialCamera(base.Pointer, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x00009C44 File Offset: 0x00007E44
		public void OptimizeScene(bool optimizeFlora = true, bool optimizeOro = false)
		{
			EngineApplicationInterface.IScene.OptimizeScene(base.Pointer, optimizeFlora, optimizeOro);
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x00009C58 File Offset: 0x00007E58
		public float GetTerrainHeight(Vec2 position, bool checkHoles = true)
		{
			return EngineApplicationInterface.IScene.GetTerrainHeight(base.Pointer, position, checkHoles);
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x00009C6C File Offset: 0x00007E6C
		public void CheckResources()
		{
			EngineApplicationInterface.IScene.CheckResources(base.Pointer);
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x00009C7E File Offset: 0x00007E7E
		public void ForceLoadResources()
		{
			EngineApplicationInterface.IScene.ForceLoadResources(base.Pointer);
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x00009C90 File Offset: 0x00007E90
		public void SetDepthOfFieldParameters(float depthOfFieldFocusStart, float depthOfFieldFocusEnd, bool isVignetteOn)
		{
			EngineApplicationInterface.IScene.SetDofParams(base.Pointer, depthOfFieldFocusStart, depthOfFieldFocusEnd, isVignetteOn);
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00009CA5 File Offset: 0x00007EA5
		public void SetDepthOfFieldFocus(float depthOfFieldFocus)
		{
			EngineApplicationInterface.IScene.SetDofFocus(base.Pointer, depthOfFieldFocus);
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x00009CB8 File Offset: 0x00007EB8
		public void ResetDepthOfFieldParams()
		{
			EngineApplicationInterface.IScene.SetDofFocus(base.Pointer, 0f);
			EngineApplicationInterface.IScene.SetDofParams(base.Pointer, 0f, 0f, true);
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000956 RID: 2390 RVA: 0x00009CEA File Offset: 0x00007EEA
		public bool HasTerrainHeightmap
		{
			get
			{
				return EngineApplicationInterface.IScene.HasTerrainHeightmap(base.Pointer);
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000957 RID: 2391 RVA: 0x00009CFC File Offset: 0x00007EFC
		public bool ContainsTerrain
		{
			get
			{
				return EngineApplicationInterface.IScene.ContainsTerrain(base.Pointer);
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000959 RID: 2393 RVA: 0x00009D21 File Offset: 0x00007F21
		// (set) Token: 0x06000958 RID: 2392 RVA: 0x00009D0E File Offset: 0x00007F0E
		public float TimeOfDay
		{
			get
			{
				return EngineApplicationInterface.IScene.GetTimeOfDay(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IScene.SetTimeOfDay(base.Pointer, value);
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600095A RID: 2394 RVA: 0x00009D33 File Offset: 0x00007F33
		public bool IsAtmosphereIndoor
		{
			get
			{
				return EngineApplicationInterface.IScene.IsAtmosphereIndoor(base.Pointer);
			}
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x00009D45 File Offset: 0x00007F45
		public void PreloadForRendering()
		{
			EngineApplicationInterface.IScene.PreloadForRendering(base.Pointer);
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x00009D57 File Offset: 0x00007F57
		public Vec3 LastFinalRenderCameraPosition
		{
			get
			{
				return EngineApplicationInterface.IScene.GetLastFinalRenderCameraPosition(base.Pointer);
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x00009D6C File Offset: 0x00007F6C
		public MatrixFrame LastFinalRenderCameraFrame
		{
			get
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				EngineApplicationInterface.IScene.GetLastFinalRenderCameraFrame(base.Pointer, ref matrixFrame);
				return matrixFrame;
			}
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x00009D94 File Offset: 0x00007F94
		public void SetColorGradeBlend(string texture1, string texture2, float alpha)
		{
			EngineApplicationInterface.IScene.SetColorGradeBlend(base.Pointer, texture1, texture2, alpha);
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x00009DA9 File Offset: 0x00007FA9
		public float GetGroundHeightAtPosition(Vec3 position, BodyFlags excludeFlags = BodyFlags.CommonCollisionExcludeFlags)
		{
			return EngineApplicationInterface.IScene.GetGroundHeightAtPosition(base.Pointer, position, (uint)excludeFlags);
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x00009DBD File Offset: 0x00007FBD
		public float GetGroundHeightAtPositionMT(Vec3 position, BodyFlags excludeFlags = BodyFlags.CommonCollisionExcludeFlags)
		{
			return EngineApplicationInterface.IScene.GetGroundHeightAtPosition(base.Pointer, position, (uint)excludeFlags);
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x00009DD1 File Offset: 0x00007FD1
		public float GetGroundHeightAtPosition(Vec3 position, out Vec3 normal, BodyFlags excludeFlags = BodyFlags.CommonCollisionExcludeFlags)
		{
			normal = Vec3.Invalid;
			return EngineApplicationInterface.IScene.GetGroundHeightAtPosition(base.Pointer, position, (uint)excludeFlags);
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x00009DF0 File Offset: 0x00007FF0
		public float GetGroundHeightAtPositionMT(Vec3 position, out Vec3 normal, BodyFlags excludeFlags = BodyFlags.CommonCollisionExcludeFlags)
		{
			normal = Vec3.Invalid;
			return EngineApplicationInterface.IScene.GetGroundHeightAndNormalAtPosition(base.Pointer, position, ref normal, (uint)excludeFlags);
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x00009E10 File Offset: 0x00008010
		public void PauseSceneSounds()
		{
			EngineApplicationInterface.IScene.PauseSceneSounds(base.Pointer);
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x00009E22 File Offset: 0x00008022
		public void ResumeSceneSounds()
		{
			EngineApplicationInterface.IScene.ResumeSceneSounds(base.Pointer);
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x00009E34 File Offset: 0x00008034
		public void FinishSceneSounds()
		{
			EngineApplicationInterface.IScene.FinishSceneSounds(base.Pointer);
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x00009E48 File Offset: 0x00008048
		public bool BoxCastOnlyForCamera(Vec3[] boxPoints, Vec3 centerPoint, bool castSupportRay, Vec3 supportRaycastPoint, Vec3 dir, float distance, out float collisionDistance, out Vec3 closestPoint, out GameEntity collidedEntity, bool preFilter = true, bool postFilter = true, BodyFlags excludedBodyFlags = BodyFlags.Disabled | BodyFlags.Dynamic | BodyFlags.Ladder | BodyFlags.OnlyCollideWithRaycast | BodyFlags.AILimiter | BodyFlags.Barrier | BodyFlags.Barrier3D | BodyFlags.Ragdoll | BodyFlags.RagdollLimiter | BodyFlags.DroppedItem | BodyFlags.DoNotCollideWithRaycast | BodyFlags.DontCollideWithCamera | BodyFlags.AgentOnly | BodyFlags.MissileOnly)
		{
			collisionDistance = float.NaN;
			closestPoint = Vec3.Invalid;
			UIntPtr zero = UIntPtr.Zero;
			bool flag = castSupportRay && EngineApplicationInterface.IScene.RayCastForClosestEntityOrTerrain(base.Pointer, ref supportRaycastPoint, ref centerPoint, 0f, ref collisionDistance, ref closestPoint, ref zero, excludedBodyFlags);
			if (!flag)
			{
				flag = EngineApplicationInterface.IScene.BoxCastOnlyForCamera(base.Pointer, boxPoints, ref centerPoint, ref dir, distance, ref collisionDistance, ref closestPoint, ref zero, excludedBodyFlags, preFilter, postFilter);
			}
			if (flag && zero != UIntPtr.Zero)
			{
				collidedEntity = new GameEntity(zero);
			}
			else
			{
				collidedEntity = null;
			}
			return flag;
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x00009EE0 File Offset: 0x000080E0
		public bool BoxCast(Vec3 boxMin, Vec3 boxMax, bool castSupportRay, Vec3 supportRaycastPoint, Vec3 dir, float distance, out float collisionDistance, out Vec3 closestPoint, out GameEntity collidedEntity, BodyFlags excludedBodyFlags = BodyFlags.CameraCollisionRayCastExludeFlags)
		{
			collisionDistance = float.NaN;
			closestPoint = Vec3.Invalid;
			UIntPtr zero = UIntPtr.Zero;
			Vec3 vec = (boxMin + boxMax) * 0.5f;
			bool flag = castSupportRay && EngineApplicationInterface.IScene.RayCastForClosestEntityOrTerrain(base.Pointer, ref supportRaycastPoint, ref vec, 0f, ref collisionDistance, ref closestPoint, ref zero, excludedBodyFlags);
			if (!flag)
			{
				flag = EngineApplicationInterface.IScene.BoxCast(base.Pointer, ref boxMin, ref boxMax, ref dir, distance, ref collisionDistance, ref closestPoint, ref zero, excludedBodyFlags);
			}
			if (flag && zero != UIntPtr.Zero)
			{
				collidedEntity = new GameEntity(zero);
			}
			else
			{
				collidedEntity = null;
			}
			return flag;
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x00009F88 File Offset: 0x00008188
		public bool RayCastForClosestEntityOrTerrain(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, out Vec3 closestPoint, out GameEntity collidedEntity, float rayThickness = 0.01f, BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
		{
			collisionDistance = float.NaN;
			closestPoint = Vec3.Invalid;
			UIntPtr zero = UIntPtr.Zero;
			bool flag = EngineApplicationInterface.IScene.RayCastForClosestEntityOrTerrain(base.Pointer, ref sourcePoint, ref targetPoint, rayThickness, ref collisionDistance, ref closestPoint, ref zero, excludeBodyFlags);
			if (flag && zero != UIntPtr.Zero)
			{
				collidedEntity = new GameEntity(zero);
				return flag;
			}
			collidedEntity = null;
			return flag;
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x00009FEC File Offset: 0x000081EC
		public bool RayCastForClosestEntityOrTerrainMT(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, out Vec3 closestPoint, out GameEntity collidedEntity, float rayThickness = 0.01f, BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
		{
			collisionDistance = float.NaN;
			closestPoint = Vec3.Invalid;
			UIntPtr zero = UIntPtr.Zero;
			bool flag = EngineApplicationInterface.IScene.RayCastForClosestEntityOrTerrain(base.Pointer, ref sourcePoint, ref targetPoint, rayThickness, ref collisionDistance, ref closestPoint, ref zero, excludeBodyFlags);
			if (flag && zero != UIntPtr.Zero)
			{
				collidedEntity = new GameEntity(zero);
				return flag;
			}
			collidedEntity = null;
			return flag;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0000A050 File Offset: 0x00008250
		public bool RayCastForClosestEntityOrTerrain(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, out GameEntity collidedEntity, float rayThickness = 0.01f, BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
		{
			Vec3 vec;
			return this.RayCastForClosestEntityOrTerrain(sourcePoint, targetPoint, out collisionDistance, out vec, out collidedEntity, rayThickness, excludeBodyFlags);
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0000A070 File Offset: 0x00008270
		public bool RayCastForClosestEntityOrTerrainMT(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, out GameEntity collidedEntity, float rayThickness = 0.01f, BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
		{
			Vec3 vec;
			return this.RayCastForClosestEntityOrTerrainMT(sourcePoint, targetPoint, out collisionDistance, out vec, out collidedEntity, rayThickness, excludeBodyFlags);
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0000A090 File Offset: 0x00008290
		public bool RayCastForClosestEntityOrTerrain(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, out Vec3 closestPoint, float rayThickness = 0.01f, BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
		{
			collisionDistance = float.NaN;
			closestPoint = Vec3.Invalid;
			UIntPtr zero = UIntPtr.Zero;
			return EngineApplicationInterface.IScene.RayCastForClosestEntityOrTerrain(base.Pointer, ref sourcePoint, ref targetPoint, rayThickness, ref collisionDistance, ref closestPoint, ref zero, excludeBodyFlags);
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0000A0D4 File Offset: 0x000082D4
		public bool RayCastForClosestEntityOrTerrainMT(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, out Vec3 closestPoint, float rayThickness = 0.01f, BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
		{
			collisionDistance = float.NaN;
			closestPoint = Vec3.Invalid;
			UIntPtr zero = UIntPtr.Zero;
			return EngineApplicationInterface.IScene.RayCastForClosestEntityOrTerrain(base.Pointer, ref sourcePoint, ref targetPoint, rayThickness, ref collisionDistance, ref closestPoint, ref zero, excludeBodyFlags);
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x0000A118 File Offset: 0x00008318
		public bool RayCastForClosestEntityOrTerrain(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, float rayThickness = 0.01f, BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
		{
			Vec3 vec;
			return this.RayCastForClosestEntityOrTerrain(sourcePoint, targetPoint, out collisionDistance, out vec, rayThickness, excludeBodyFlags);
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x0000A134 File Offset: 0x00008334
		public bool RayCastForClosestEntityOrTerrainMT(Vec3 sourcePoint, Vec3 targetPoint, out float collisionDistance, float rayThickness = 0.01f, BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
		{
			Vec3 vec;
			return this.RayCastForClosestEntityOrTerrainMT(sourcePoint, targetPoint, out collisionDistance, out vec, rayThickness, excludeBodyFlags);
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0000A150 File Offset: 0x00008350
		public void ImportNavigationMeshPrefab(string navMeshPrefabName, int navMeshGroupShift)
		{
			EngineApplicationInterface.IScene.LoadNavMeshPrefab(base.Pointer, navMeshPrefabName, navMeshGroupShift);
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x0000A164 File Offset: 0x00008364
		public void MarkFacesWithIdAsLadder(int faceGroupId, bool isLadder)
		{
			EngineApplicationInterface.IScene.MarkFacesWithIdAsLadder(base.Pointer, faceGroupId, isLadder);
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x0000A178 File Offset: 0x00008378
		public void SetAbilityOfFacesWithId(int faceGroupId, bool isEnabled)
		{
			EngineApplicationInterface.IScene.SetAbilityOfFacesWithId(base.Pointer, faceGroupId, isEnabled);
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x0000A18C File Offset: 0x0000838C
		public void SwapFaceConnectionsWithID(int hubFaceGroupID, int toBeSeparatedFaceGroupId, int toBeMergedFaceGroupId)
		{
			EngineApplicationInterface.IScene.SwapFaceConnectionsWithId(base.Pointer, hubFaceGroupID, toBeSeparatedFaceGroupId, toBeMergedFaceGroupId);
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x0000A1A1 File Offset: 0x000083A1
		public void MergeFacesWithId(int faceGroupId0, int faceGroupId1, int newFaceGroupId)
		{
			EngineApplicationInterface.IScene.MergeFacesWithId(base.Pointer, faceGroupId0, faceGroupId1, newFaceGroupId);
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x0000A1B6 File Offset: 0x000083B6
		public void SeparateFacesWithId(int faceGroupId0, int faceGroupId1)
		{
			EngineApplicationInterface.IScene.SeparateFacesWithId(base.Pointer, faceGroupId0, faceGroupId1);
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x0000A1CA File Offset: 0x000083CA
		public bool IsAnyFaceWithId(int faceGroupId)
		{
			return EngineApplicationInterface.IScene.IsAnyFaceWithId(base.Pointer, faceGroupId);
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0000A1E0 File Offset: 0x000083E0
		public bool GetNavigationMeshForPosition(ref Vec3 position)
		{
			int num;
			return this.GetNavigationMeshForPosition(ref position, out num, 1.5f);
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x0000A1FB File Offset: 0x000083FB
		public bool GetNavigationMeshForPosition(ref Vec3 position, out int faceGroupId, float heightDifferenceLimit = 1.5f)
		{
			faceGroupId = int.MinValue;
			return EngineApplicationInterface.IScene.GetNavigationMeshFaceForPosition(base.Pointer, ref position, ref faceGroupId, heightDifferenceLimit);
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0000A217 File Offset: 0x00008417
		public bool DoesPathExistBetweenPositions(WorldPosition position, WorldPosition destination)
		{
			return EngineApplicationInterface.IScene.DoesPathExistBetweenPositions(base.Pointer, position, destination);
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0000A22B File Offset: 0x0000842B
		public void EnsurePostfxSystem()
		{
			EngineApplicationInterface.IScene.EnsurePostfxSystem(base.Pointer);
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0000A23D File Offset: 0x0000843D
		public void SetBloom(bool mode)
		{
			EngineApplicationInterface.IScene.SetBloom(base.Pointer, mode);
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x0000A250 File Offset: 0x00008450
		public void SetDofMode(bool mode)
		{
			EngineApplicationInterface.IScene.SetDofMode(base.Pointer, mode);
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0000A263 File Offset: 0x00008463
		public void SetOcclusionMode(bool mode)
		{
			EngineApplicationInterface.IScene.SetOcclusionMode(base.Pointer, mode);
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x0000A276 File Offset: 0x00008476
		public void SetExternalInjectionTexture(Texture texture)
		{
			EngineApplicationInterface.IScene.SetExternalInjectionTexture(base.Pointer, texture.Pointer);
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x0000A28E File Offset: 0x0000848E
		public void SetSunshaftMode(bool mode)
		{
			EngineApplicationInterface.IScene.SetSunshaftMode(base.Pointer, mode);
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x0000A2A1 File Offset: 0x000084A1
		public Vec3 GetSunDirection()
		{
			return EngineApplicationInterface.IScene.GetSunDirection(base.Pointer);
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0000A2B3 File Offset: 0x000084B3
		public float GetNorthAngle()
		{
			return EngineApplicationInterface.IScene.GetNorthAngle(base.Pointer);
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x0000A2C8 File Offset: 0x000084C8
		public float GetNorthRotation()
		{
			float northAngle = this.GetNorthAngle();
			return 0.017453292f * -northAngle;
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0000A2E4 File Offset: 0x000084E4
		public bool GetTerrainMinMaxHeight(out float minHeight, out float maxHeight)
		{
			minHeight = 0f;
			maxHeight = 0f;
			return EngineApplicationInterface.IScene.GetTerrainMinMaxHeight(this, ref minHeight, ref maxHeight);
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0000A301 File Offset: 0x00008501
		public void GetPhysicsMinMax(ref Vec3 min_max)
		{
			EngineApplicationInterface.IScene.GetPhysicsMinMax(base.Pointer, ref min_max);
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x0000A314 File Offset: 0x00008514
		public bool IsEditorScene()
		{
			return EngineApplicationInterface.IScene.IsEditorScene(base.Pointer);
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0000A326 File Offset: 0x00008526
		public void SetMotionBlurMode(bool mode)
		{
			EngineApplicationInterface.IScene.SetMotionBlurMode(base.Pointer, mode);
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x0000A339 File Offset: 0x00008539
		public void SetAntialiasingMode(bool mode)
		{
			EngineApplicationInterface.IScene.SetAntialiasingMode(base.Pointer, mode);
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x0000A34C File Offset: 0x0000854C
		public void SetDLSSMode(bool mode)
		{
			EngineApplicationInterface.IScene.SetDLSSMode(base.Pointer, mode);
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0000A35F File Offset: 0x0000855F
		public IEnumerable<GameEntity> FindEntitiesWithTag(string tag)
		{
			return GameEntity.GetEntitiesWithTag(this, tag);
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x0000A368 File Offset: 0x00008568
		public GameEntity FindEntityWithTag(string tag)
		{
			return GameEntity.GetFirstEntityWithTag(this, tag);
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x0000A371 File Offset: 0x00008571
		public GameEntity FindEntityWithName(string name)
		{
			return GameEntity.GetFirstEntityWithName(this, name);
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x0000A37A File Offset: 0x0000857A
		public IEnumerable<GameEntity> FindEntitiesWithTagExpression(string expression)
		{
			return GameEntity.GetEntitiesWithTagExpression(this, expression);
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0000A383 File Offset: 0x00008583
		public int GetSoftBoundaryVertexCount()
		{
			return EngineApplicationInterface.IScene.GetSoftBoundaryVertexCount(base.Pointer);
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x0000A395 File Offset: 0x00008595
		public int GetHardBoundaryVertexCount()
		{
			return EngineApplicationInterface.IScene.GetHardBoundaryVertexCount(base.Pointer);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x0000A3A7 File Offset: 0x000085A7
		public Vec2 GetSoftBoundaryVertex(int index)
		{
			return EngineApplicationInterface.IScene.GetSoftBoundaryVertex(base.Pointer, index);
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x0000A3BA File Offset: 0x000085BA
		public Vec2 GetHardBoundaryVertex(int index)
		{
			return EngineApplicationInterface.IScene.GetHardBoundaryVertex(base.Pointer, index);
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x0000A3CD File Offset: 0x000085CD
		public Path GetPathWithName(string name)
		{
			return EngineApplicationInterface.IScene.GetPathWithName(base.Pointer, name);
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x0000A3E0 File Offset: 0x000085E0
		public void DeletePathWithName(string name)
		{
			EngineApplicationInterface.IScene.DeletePathWithName(base.Pointer, name);
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x0000A3F3 File Offset: 0x000085F3
		public void AddPath(string name)
		{
			EngineApplicationInterface.IScene.AddPath(base.Pointer, name);
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x0000A406 File Offset: 0x00008606
		public void AddPathPoint(string name, MatrixFrame frame)
		{
			EngineApplicationInterface.IScene.AddPathPoint(base.Pointer, name, ref frame);
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x0000A41B File Offset: 0x0000861B
		public void GetBoundingBox(out Vec3 min, out Vec3 max)
		{
			min = Vec3.Invalid;
			max = Vec3.Invalid;
			EngineApplicationInterface.IScene.GetBoundingBox(base.Pointer, ref min, ref max);
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x0000A445 File Offset: 0x00008645
		public void SetName(string name)
		{
			EngineApplicationInterface.IScene.SetName(base.Pointer, name);
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x0000A458 File Offset: 0x00008658
		public string GetName()
		{
			return EngineApplicationInterface.IScene.GetName(base.Pointer);
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x0000A46A File Offset: 0x0000866A
		public string GetModulePath()
		{
			return EngineApplicationInterface.IScene.GetModulePath(base.Pointer);
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000999 RID: 2457 RVA: 0x0000A47C File Offset: 0x0000867C
		// (set) Token: 0x0600099A RID: 2458 RVA: 0x0000A48E File Offset: 0x0000868E
		public float TimeSpeed
		{
			get
			{
				return EngineApplicationInterface.IScene.GetTimeSpeed(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IScene.SetTimeSpeed(base.Pointer, value);
			}
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x0000A4A1 File Offset: 0x000086A1
		public void SetOwnerThread()
		{
			EngineApplicationInterface.IScene.SetOwnerThread(base.Pointer);
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x0000A4B4 File Offset: 0x000086B4
		public Path[] GetPathsWithNamePrefix(string prefix)
		{
			int numberOfPathsWithNamePrefix = EngineApplicationInterface.IScene.GetNumberOfPathsWithNamePrefix(base.Pointer, prefix);
			UIntPtr[] array = new UIntPtr[numberOfPathsWithNamePrefix];
			EngineApplicationInterface.IScene.GetPathsWithNamePrefix(base.Pointer, array, prefix);
			Path[] array2 = new Path[numberOfPathsWithNamePrefix];
			for (int i = 0; i < numberOfPathsWithNamePrefix; i++)
			{
				UIntPtr uintPtr = array[i];
				array2[i] = new Path(uintPtr);
			}
			return array2;
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x0000A50F File Offset: 0x0000870F
		public void SetUseConstantTime(bool value)
		{
			EngineApplicationInterface.IScene.SetUseConstantTime(base.Pointer, value);
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x0000A522 File Offset: 0x00008722
		public bool CheckPointCanSeePoint(Vec3 source, Vec3 target, float? distanceToCheck = null)
		{
			if (distanceToCheck == null)
			{
				distanceToCheck = new float?(source.Distance(target));
			}
			return EngineApplicationInterface.IScene.CheckPointCanSeePoint(base.Pointer, source, target, distanceToCheck.Value);
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x0000A555 File Offset: 0x00008755
		public void SetPlaySoundEventsAfterReadyToRender(bool value)
		{
			EngineApplicationInterface.IScene.SetPlaySoundEventsAfterReadyToRender(base.Pointer, value);
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x0000A568 File Offset: 0x00008768
		public void DisableStaticShadows(bool value)
		{
			EngineApplicationInterface.IScene.DisableStaticShadows(base.Pointer, value);
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x0000A57B File Offset: 0x0000877B
		public Mesh GetSkyboxMesh()
		{
			return EngineApplicationInterface.IScene.GetSkyboxMesh(base.Pointer);
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x0000A58D File Offset: 0x0000878D
		public void SetAtmosphereWithName(string name)
		{
			EngineApplicationInterface.IScene.SetAtmosphereWithName(base.Pointer, name);
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x0000A5A0 File Offset: 0x000087A0
		public void FillEntityWithHardBorderPhysicsBarrier(GameEntity entity)
		{
			EngineApplicationInterface.IScene.FillEntityWithHardBorderPhysicsBarrier(base.Pointer, entity.Pointer);
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x0000A5B8 File Offset: 0x000087B8
		public void ClearDecals()
		{
			EngineApplicationInterface.IScene.ClearDecals(base.Pointer);
		}

		// Token: 0x0400018D RID: 397
		public const float AutoClimbHeight = 1.5f;

		// Token: 0x0400018E RID: 398
		public const float NavMeshHeightLimit = 1.5f;

		// Token: 0x0400018F RID: 399
		public static readonly TWSharedMutex PhysicsAndRayCastLock = new TWSharedMutex();
	}
}
