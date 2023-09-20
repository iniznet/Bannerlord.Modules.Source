using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public struct CustomBattleSceneData
	{
		public string SceneID { get; private set; }

		public TextObject Name { get; private set; }

		public TerrainType Terrain { get; private set; }

		public List<TerrainType> TerrainTypes { get; private set; }

		public ForestDensity ForestDensity { get; private set; }

		public bool IsSiegeMap { get; private set; }

		public bool IsVillageMap { get; private set; }

		public bool IsLordsHallMap { get; private set; }

		public CustomBattleSceneData(string sceneID, TextObject name, TerrainType terrain, List<TerrainType> terrainTypes, ForestDensity forestDensity, bool isSiegeMap, bool isVillageMap, bool isLordsHallMap)
		{
			this.SceneID = sceneID;
			this.Name = name;
			this.Terrain = terrain;
			this.TerrainTypes = terrainTypes;
			this.ForestDensity = forestDensity;
			this.IsSiegeMap = isSiegeMap;
			this.IsVillageMap = isVillageMap;
			this.IsLordsHallMap = isLordsHallMap;
		}
	}
}
