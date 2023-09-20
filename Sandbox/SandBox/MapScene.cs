using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public class MapScene : IMapScene
	{
		public Scene Scene
		{
			get
			{
				return this._scene;
			}
		}

		public MapScene()
		{
			this._sceneLevels = new Dictionary<string, uint>();
		}

		public Vec2 GetTerrainSize()
		{
			return this._terrainSize;
		}

		public uint GetSceneLevel(string name)
		{
			uint num;
			if (this._sceneLevels.TryGetValue(name, out num) && num != 2147483647U)
			{
				return num;
			}
			uint upgradeLevelMaskOfLevelName = this._scene.GetUpgradeLevelMaskOfLevelName(name);
			this._sceneLevels[name] = upgradeLevelMaskOfLevelName;
			return upgradeLevelMaskOfLevelName;
		}

		public void SetSceneLevels(List<string> levels)
		{
			foreach (string text in levels)
			{
				this._sceneLevels.Add(text, 2147483647U);
			}
		}

		public List<AtmosphereState> GetAtmosphereStates()
		{
			List<AtmosphereState> list = new List<AtmosphereState>();
			foreach (GameEntity gameEntity in this.Scene.FindEntitiesWithTag("atmosphere_probe"))
			{
				MapAtmosphereProbe firstScriptOfType = gameEntity.GetFirstScriptOfType<MapAtmosphereProbe>();
				Vec3 globalPosition = gameEntity.GlobalPosition;
				AtmosphereState atmosphereState = new AtmosphereState
				{
					Position = globalPosition,
					HumidityAverage = firstScriptOfType.rainDensity,
					HumidityVariance = 5f,
					TemperatureAverage = firstScriptOfType.temperature,
					TemperatureVariance = 5f,
					distanceForMaxWeight = firstScriptOfType.minRadius,
					distanceForMinWeight = firstScriptOfType.maxRadius,
					ColorGradeTexture = firstScriptOfType.colorGrade
				};
				list.Add(atmosphereState);
			}
			return list;
		}

		public void ValidateAgentVisualsReseted()
		{
			if (this._scene != null && this._agentRendererSceneController != null)
			{
				MBAgentRendererSceneController.ValidateAgentVisualsReseted(this._scene, this._agentRendererSceneController);
			}
		}

		public void SetAtmosphereColorgrade(TerrainType terrainType)
		{
		}

		public void AddNewEntityToMapScene(string entityId, Vec2 position)
		{
			GameEntity gameEntity = GameEntity.Instantiate(this._scene, entityId, true);
			if (gameEntity != null)
			{
				Vec3 vec;
				vec..ctor(position.x, position.y, 0f, -1f);
				vec.z = this._scene.GetGroundHeightAtPosition(position.ToVec3(0f), 6402441);
				gameEntity.SetLocalPosition(vec);
			}
		}

		public void GetFaceIndexForMultiplePositions(int movedPartyCount, float[] positionArray, PathFaceRecord[] resultArray)
		{
			MBMapScene.GetFaceIndexForMultiplePositions(this._scene, movedPartyCount, positionArray, resultArray, false, true);
		}

		public void GetMapBorders(out Vec2 minimumPosition, out Vec2 maximumPosition, out float maximumHeight)
		{
			GameEntity firstEntityWithName = this._scene.GetFirstEntityWithName("border_min");
			GameEntity firstEntityWithName2 = this._scene.GetFirstEntityWithName("border_max");
			Vec2 vec;
			if (!(firstEntityWithName != null))
			{
				vec = Vec2.Zero;
			}
			else
			{
				MatrixFrame matrixFrame = firstEntityWithName.GetGlobalFrame();
				vec = matrixFrame.origin.AsVec2;
			}
			minimumPosition = vec;
			Vec2 vec2;
			if (!(firstEntityWithName2 != null))
			{
				vec2 = new Vec2(900f, 900f);
			}
			else
			{
				MatrixFrame matrixFrame = firstEntityWithName2.GetGlobalFrame();
				vec2 = matrixFrame.origin.AsVec2;
			}
			maximumPosition = vec2;
			maximumHeight = ((firstEntityWithName2 != null) ? firstEntityWithName2.GetGlobalFrame().origin.z : 670f);
		}

		public void Load()
		{
			Debug.Print("Creating map scene", 0, 12, 17592186044416UL);
			this._scene = Scene.CreateNewScene(false, true, 1, "MapScene");
			this._scene.SetClothSimulationState(true);
			this._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._scene, 4096);
			this._agentRendererSceneController.SetDoTimerBasedForcedSkeletonUpdates(false);
			this._scene.SetOcclusionMode(true);
			SceneInitializationData sceneInitializationData;
			sceneInitializationData..ctor(true);
			sceneInitializationData.UsePhysicsMaterials = false;
			sceneInitializationData.EnableFloraPhysics = false;
			sceneInitializationData.UseTerrainMeshBlending = false;
			sceneInitializationData.CreateOros = false;
			Debug.Print("reading map scene", 0, 12, 17592186044416UL);
			this._scene.Read("Main_map", ref sceneInitializationData, "");
			Utilities.SetAllocationAlwaysValidScene(this._scene);
			this._scene.DisableStaticShadows(true);
			this._scene.InvalidateTerrainPhysicsMaterials();
			this.LoadAtmosphereData(this._scene);
			this.DisableUnwalkableNavigationMeshes();
			MBMapScene.ValidateTerrainSoundIds();
			this._scene.OptimizeScene(true, false);
			Vec2i vec2i;
			float num;
			int num2;
			int num3;
			this._scene.GetTerrainData(ref vec2i, ref num, ref num2, ref num3);
			this._terrainSize.x = (float)vec2i.X * num;
			this._terrainSize.y = (float)vec2i.Y * num;
			MBMapScene.GetBattleSceneIndexMap(this._scene, ref this._battleTerrainIndexMap, ref this._battleTerrainIndexMapWidth, ref this._battleTerrainIndexMapHeight);
			Debug.Print("Ticking map scene for first initialization", 0, 12, 17592186044416UL);
			this._scene.Tick(0.1f);
			AsyncTask asyncTask = AsyncTask.CreateWithDelegate(new ManagedDelegate
			{
				Instance = new ManagedDelegate.DelegateDefinition(Campaign.LateAITick)
			}, false);
			Campaign.Current.CampaignLateAITickTask = asyncTask;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("refresh_battle_scene_index_map", "campaign")]
		public static string RefreshBattleSceneIndexMap(List<string> strings)
		{
			MapScene mapScene = Campaign.Current.MapSceneWrapper as MapScene;
			MBMapScene.GetBattleSceneIndexMap(mapScene._scene, ref mapScene._battleTerrainIndexMap, ref mapScene._battleTerrainIndexMapWidth, ref mapScene._battleTerrainIndexMapHeight);
			return "Success!";
		}

		public void Destroy()
		{
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._scene, this._agentRendererSceneController, false);
			this._agentRendererSceneController = null;
		}

		public void DisableUnwalkableNavigationMeshes()
		{
			this.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(1), false);
			this.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(12), false);
			this.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(0), false);
			this.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(9), false);
			this.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(13), false);
			this.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(14), false);
		}

		public PathFaceRecord GetFaceIndex(Vec2 position)
		{
			PathFaceRecord pathFaceRecord;
			pathFaceRecord..ctor(-1, -1, -1);
			this._scene.GetNavMeshFaceIndex(ref pathFaceRecord, position, false, true);
			return pathFaceRecord;
		}

		public bool AreFacesOnSameIsland(PathFaceRecord startingFace, PathFaceRecord endFace, bool ignoreDisabled)
		{
			return this._scene.DoesPathExistBetweenFaces(startingFace.FaceIndex, endFace.FaceIndex, ignoreDisabled);
		}

		private void LoadAtmosphereData(Scene mapScene)
		{
			MBMapScene.LoadAtmosphereData(mapScene);
		}

		public TerrainType GetTerrainTypeAtPosition(Vec2 position)
		{
			PathFaceRecord faceIndex = this.GetFaceIndex(position);
			return this.GetFaceTerrainType(faceIndex);
		}

		void IMapScene.SetSoundParameters(float tod, int season, float cameraHeight)
		{
			MBMapScene.SetSoundParameters(this._scene, tod, season, cameraHeight);
		}

		public TerrainType GetFaceTerrainType(PathFaceRecord navMeshFace)
		{
			if (!navMeshFace.IsValid())
			{
				Debug.FailedAssert("Null nav mesh face tried to get terrain type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\MapScene.cs", "GetFaceTerrainType", 272);
				return 4;
			}
			switch (navMeshFace.FaceGroupIndex)
			{
			case 1:
				return 4;
			case 2:
				return 5;
			case 3:
				return 2;
			case 4:
				return 10;
			case 5:
				return 3;
			case 6:
				return 11;
			case 7:
				return 1;
			case 8:
				return 12;
			case 10:
				return 0;
			case 11:
				return 9;
			case 13:
				return 13;
			case 14:
				return 14;
			}
			return 4;
		}

		public static int GetNavigationMeshIndexOfTerrainType(TerrainType terrainType)
		{
			switch (terrainType)
			{
			case 0:
				return 10;
			case 1:
				return 7;
			case 2:
				return 3;
			case 3:
				return 5;
			case 4:
				return 1;
			case 5:
				return 2;
			case 9:
				return 11;
			case 10:
				return 4;
			case 11:
				return 6;
			case 12:
				return 8;
			case 13:
				return 13;
			case 14:
				return 14;
			}
			return -1;
		}

		public List<TerrainType> GetEnvironmentTerrainTypes(Vec2 position)
		{
			List<TerrainType> list = new List<TerrainType>();
			Vec2 vec;
			vec..ctor(1f, 0f);
			list.Add(this.GetTerrainTypeAtPosition(position));
			for (int i = 0; i < 8; i++)
			{
				vec.RotateCCW(0.7853982f * (float)i);
				for (int j = 1; j < 7; j++)
				{
					TerrainType terrainTypeAtPosition = this.GetTerrainTypeAtPosition(position + (float)j * vec);
					this.GetFaceIndex(position + (float)j * vec);
					if (!list.Contains(terrainTypeAtPosition))
					{
						list.Add(terrainTypeAtPosition);
					}
				}
			}
			return list;
		}

		public List<TerrainType> GetEnvironmentTerrainTypesCount(Vec2 position, out TerrainType currentPositionTerrainType)
		{
			List<TerrainType> list = new List<TerrainType>();
			Vec2 vec;
			vec..ctor(1f, 0f);
			currentPositionTerrainType = this.GetTerrainTypeAtPosition(position);
			list.Add(currentPositionTerrainType);
			for (int i = 0; i < 8; i++)
			{
				vec.RotateCCW(0.7853982f * (float)i);
				for (int j = 1; j < 7; j++)
				{
					PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(position + (float)j * vec);
					if (faceIndex.IsValid())
					{
						TerrainType faceTerrainType = this.GetFaceTerrainType(faceIndex);
						list.Add(faceTerrainType);
					}
				}
			}
			return list;
		}

		public MapPatchData GetMapPatchAtPosition(Vec2 position)
		{
			if (this._battleTerrainIndexMap != null)
			{
				int num = MathF.Floor(position.x / this._terrainSize.X * (float)this._battleTerrainIndexMapWidth);
				int num2 = MathF.Floor(position.y / this._terrainSize.Y * (float)this._battleTerrainIndexMapHeight);
				num = MBMath.ClampIndex(num, 0, this._battleTerrainIndexMapWidth);
				int num3 = (MBMath.ClampIndex(num2, 0, this._battleTerrainIndexMapHeight) * this._battleTerrainIndexMapWidth + num) * 2;
				byte b = this._battleTerrainIndexMap[num3];
				byte b2 = this._battleTerrainIndexMap[num3 + 1];
				Vec2 vec;
				vec..ctor((float)(b2 & 15) / 15f, (float)((b2 >> 4) & 15) / 15f);
				MapPatchData mapPatchData = default(MapPatchData);
				mapPatchData.sceneIndex = (int)b;
				mapPatchData.normalizedCoordinates = vec;
				return mapPatchData;
			}
			return default(MapPatchData);
		}

		public Vec2 GetAccessiblePointNearPosition(Vec2 position, float radius)
		{
			return MBMapScene.GetAccessiblePointNearPosition(this._scene, position, radius);
		}

		public bool GetPathBetweenAIFaces(PathFaceRecord startingFace, PathFaceRecord endingFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, NavigationPath path)
		{
			return this._scene.GetPathBetweenAIFaces(startingFace.FaceIndex, endingFace.FaceIndex, startingPosition, endingPosition, agentRadius, path);
		}

		public bool GetPathDistanceBetweenAIFaces(PathFaceRecord startingAiFace, PathFaceRecord endingAiFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, float distanceLimit, out float distance)
		{
			return this._scene.GetPathDistanceBetweenAIFaces(startingAiFace.FaceIndex, endingAiFace.FaceIndex, startingPosition, endingPosition, agentRadius, distanceLimit, ref distance);
		}

		public bool IsLineToPointClear(PathFaceRecord startingFace, Vec2 position, Vec2 destination, float agentRadius)
		{
			return this._scene.IsLineToPointClear(startingFace.FaceIndex, position, destination, agentRadius);
		}

		public Vec2 GetLastPointOnNavigationMeshFromPositionToDestination(PathFaceRecord startingFace, Vec2 position, Vec2 destination)
		{
			return this._scene.GetLastPointOnNavigationMeshFromPositionToDestination(startingFace.FaceIndex, position, destination);
		}

		public Vec2 GetNavigationMeshCenterPosition(PathFaceRecord face)
		{
			Vec3 zero = Vec3.Zero;
			this._scene.GetNavMeshCenterPosition(face.FaceIndex, ref zero);
			return zero.AsVec2;
		}

		public int GetNumberOfNavigationMeshFaces()
		{
			return this._scene.GetNavMeshFaceCount();
		}

		public bool GetHeightAtPoint(Vec2 point, ref float height)
		{
			return this._scene.GetHeightAtPoint(point, 6402441, ref height);
		}

		public float GetWinterTimeFactor()
		{
			return this._scene.GetWinterTimeFactor();
		}

		public float GetFaceVertexZ(PathFaceRecord navMeshFace)
		{
			return this._scene.GetNavMeshFaceFirstVertexZ(navMeshFace.FaceIndex);
		}

		public Vec3 GetGroundNormal(Vec2 position)
		{
			return this._scene.GetNormalAt(position);
		}

		public void GetTerrainHeightandNormal(Vec2 position, out float height, out Vec3 normal)
		{
			this._scene.GetTerrainHeightAndNormal(position, ref height, ref normal);
		}

		public string GetTerrainTypeName(TerrainType type)
		{
			string text = "Invalid";
			switch (type)
			{
			case 0:
				text = "Water";
				break;
			case 1:
				text = "Mountain";
				break;
			case 2:
				text = "Snow";
				break;
			case 3:
				text = "Steppe";
				break;
			case 4:
				text = "Plain";
				break;
			case 5:
				text = "Desert";
				break;
			case 6:
				text = "Swamp";
				break;
			case 7:
				text = "Dune";
				break;
			case 8:
				text = "Bridge";
				break;
			case 9:
				text = "River";
				break;
			case 10:
				text = "Forest";
				break;
			case 11:
				text = "ShallowRiver";
				break;
			case 12:
				text = "Lake";
				break;
			case 13:
				text = "Canyon";
				break;
			}
			return text;
		}

		private Scene _scene;

		private MBAgentRendererSceneController _agentRendererSceneController;

		private Dictionary<string, uint> _sceneLevels;

		private int _battleTerrainIndexMapWidth;

		private int _battleTerrainIndexMapHeight;

		private byte[] _battleTerrainIndexMap;

		private Vec2 _terrainSize;
	}
}
