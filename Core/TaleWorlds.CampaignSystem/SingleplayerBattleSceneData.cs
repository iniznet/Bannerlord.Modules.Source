﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	public struct SingleplayerBattleSceneData
	{
		public string SceneID { get; private set; }

		public TerrainType Terrain { get; private set; }

		public List<TerrainType> TerrainTypes { get; private set; }

		public ForestDensity ForestDensity { get; private set; }

		public List<int> MapIndices { get; private set; }

		public SingleplayerBattleSceneData(string sceneID, TerrainType terrain, List<TerrainType> terrainTypes, ForestDensity forestDensity, List<int> mapIndices)
		{
			this.SceneID = sceneID;
			this.Terrain = terrain;
			this.TerrainTypes = terrainTypes;
			this.ForestDensity = forestDensity;
			this.MapIndices = mapIndices;
		}
	}
}
