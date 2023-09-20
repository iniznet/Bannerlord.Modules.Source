using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000081 RID: 129
	public struct SingleplayerBattleSceneData
	{
		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06001031 RID: 4145 RVA: 0x00048EB9 File Offset: 0x000470B9
		// (set) Token: 0x06001032 RID: 4146 RVA: 0x00048EC1 File Offset: 0x000470C1
		public string SceneID { get; private set; }

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06001033 RID: 4147 RVA: 0x00048ECA File Offset: 0x000470CA
		// (set) Token: 0x06001034 RID: 4148 RVA: 0x00048ED2 File Offset: 0x000470D2
		public TerrainType Terrain { get; private set; }

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001035 RID: 4149 RVA: 0x00048EDB File Offset: 0x000470DB
		// (set) Token: 0x06001036 RID: 4150 RVA: 0x00048EE3 File Offset: 0x000470E3
		public List<TerrainType> TerrainTypes { get; private set; }

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001037 RID: 4151 RVA: 0x00048EEC File Offset: 0x000470EC
		// (set) Token: 0x06001038 RID: 4152 RVA: 0x00048EF4 File Offset: 0x000470F4
		public ForestDensity ForestDensity { get; private set; }

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001039 RID: 4153 RVA: 0x00048EFD File Offset: 0x000470FD
		// (set) Token: 0x0600103A RID: 4154 RVA: 0x00048F05 File Offset: 0x00047105
		public List<int> MapIndices { get; private set; }

		// Token: 0x0600103B RID: 4155 RVA: 0x00048F0E File Offset: 0x0004710E
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
