using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000082 RID: 130
	public struct ConversationSceneData
	{
		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x0600103C RID: 4156 RVA: 0x00048F35 File Offset: 0x00047135
		// (set) Token: 0x0600103D RID: 4157 RVA: 0x00048F3D File Offset: 0x0004713D
		public string SceneID { get; private set; }

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x0600103E RID: 4158 RVA: 0x00048F46 File Offset: 0x00047146
		// (set) Token: 0x0600103F RID: 4159 RVA: 0x00048F4E File Offset: 0x0004714E
		public TerrainType Terrain { get; private set; }

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001040 RID: 4160 RVA: 0x00048F57 File Offset: 0x00047157
		// (set) Token: 0x06001041 RID: 4161 RVA: 0x00048F5F File Offset: 0x0004715F
		public List<TerrainType> TerrainTypes { get; private set; }

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06001042 RID: 4162 RVA: 0x00048F68 File Offset: 0x00047168
		// (set) Token: 0x06001043 RID: 4163 RVA: 0x00048F70 File Offset: 0x00047170
		public ForestDensity ForestDensity { get; private set; }

		// Token: 0x06001044 RID: 4164 RVA: 0x00048F79 File Offset: 0x00047179
		public ConversationSceneData(string sceneID, TerrainType terrain, List<TerrainType> terrainTypes, ForestDensity forestDensity)
		{
			this.SceneID = sceneID;
			this.Terrain = terrain;
			this.TerrainTypes = terrainTypes;
			this.ForestDensity = forestDensity;
		}
	}
}
