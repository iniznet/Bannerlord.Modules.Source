using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Map
{
	// Token: 0x020000CA RID: 202
	public interface IMapScene
	{
		// Token: 0x0600127A RID: 4730
		void Load();

		// Token: 0x0600127B RID: 4731
		void Destroy();

		// Token: 0x0600127C RID: 4732
		PathFaceRecord GetFaceIndex(Vec2 position);

		// Token: 0x0600127D RID: 4733
		bool AreFacesOnSameIsland(PathFaceRecord startingFace, PathFaceRecord endFace, bool ignoreDisabled);

		// Token: 0x0600127E RID: 4734
		TerrainType GetTerrainTypeAtPosition(Vec2 position);

		// Token: 0x0600127F RID: 4735
		List<TerrainType> GetEnvironmentTerrainTypes(Vec2 position);

		// Token: 0x06001280 RID: 4736
		List<TerrainType> GetEnvironmentTerrainTypesCount(Vec2 position, out TerrainType currentPositionTerrainType);

		// Token: 0x06001281 RID: 4737
		MapPatchData GetMapPatchAtPosition(Vec2 position);

		// Token: 0x06001282 RID: 4738
		TerrainType GetFaceTerrainType(PathFaceRecord faceIndex);

		// Token: 0x06001283 RID: 4739
		Vec2 GetAccessiblePointNearPosition(Vec2 position, float radius);

		// Token: 0x06001284 RID: 4740
		bool GetPathBetweenAIFaces(PathFaceRecord startingFace, PathFaceRecord endingFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, NavigationPath path);

		// Token: 0x06001285 RID: 4741
		bool GetPathDistanceBetweenAIFaces(PathFaceRecord startingAiFace, PathFaceRecord endingAiFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, float distanceLimit, out float distance);

		// Token: 0x06001286 RID: 4742
		bool IsLineToPointClear(PathFaceRecord startingFace, Vec2 position, Vec2 destination, float agentRadius);

		// Token: 0x06001287 RID: 4743
		Vec2 GetLastPointOnNavigationMeshFromPositionToDestination(PathFaceRecord startingFace, Vec2 position, Vec2 destination);

		// Token: 0x06001288 RID: 4744
		Vec2 GetNavigationMeshCenterPosition(PathFaceRecord face);

		// Token: 0x06001289 RID: 4745
		int GetNumberOfNavigationMeshFaces();

		// Token: 0x0600128A RID: 4746
		bool GetHeightAtPoint(Vec2 point, ref float height);

		// Token: 0x0600128B RID: 4747
		float GetWinterTimeFactor();

		// Token: 0x0600128C RID: 4748
		void GetTerrainHeightandNormal(Vec2 position, out float height, out Vec3 normal);

		// Token: 0x0600128D RID: 4749
		float GetFaceVertexZ(PathFaceRecord navMeshFace);

		// Token: 0x0600128E RID: 4750
		Vec3 GetGroundNormal(Vec2 position);

		// Token: 0x0600128F RID: 4751
		string GetTerrainTypeName(TerrainType type);

		// Token: 0x06001290 RID: 4752
		Vec2 GetTerrainSize();

		// Token: 0x06001291 RID: 4753
		uint GetSceneLevel(string name);

		// Token: 0x06001292 RID: 4754
		void SetSoundParameters(float tod, int season, float cameraHeight);

		// Token: 0x06001293 RID: 4755
		void SetSceneLevels(List<string> levels);

		// Token: 0x06001294 RID: 4756
		List<AtmosphereState> GetAtmosphereStates();

		// Token: 0x06001295 RID: 4757
		void SetAtmosphereColorgrade(TerrainType terrainType);

		// Token: 0x06001296 RID: 4758
		void AddNewEntityToMapScene(string entityId, Vec2 position);

		// Token: 0x06001297 RID: 4759
		void GetFaceIndexForMultiplePositions(int movedPartyCount, float[] positionArray, PathFaceRecord[] resultArray);

		// Token: 0x06001298 RID: 4760
		void GetMapBorders(out Vec2 minimumPosition, out Vec2 maximumPosition, out float maximumHeight);
	}
}
