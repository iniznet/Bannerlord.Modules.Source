using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	// Token: 0x02000011 RID: 17
	public struct CustomBattleSceneData
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x0000817A File Offset: 0x0000637A
		// (set) Token: 0x060000EA RID: 234 RVA: 0x00008182 File Offset: 0x00006382
		public string SceneID { get; private set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000EB RID: 235 RVA: 0x0000818B File Offset: 0x0000638B
		// (set) Token: 0x060000EC RID: 236 RVA: 0x00008193 File Offset: 0x00006393
		public TextObject Name { get; private set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000ED RID: 237 RVA: 0x0000819C File Offset: 0x0000639C
		// (set) Token: 0x060000EE RID: 238 RVA: 0x000081A4 File Offset: 0x000063A4
		public TerrainType Terrain { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000EF RID: 239 RVA: 0x000081AD File Offset: 0x000063AD
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x000081B5 File Offset: 0x000063B5
		public List<TerrainType> TerrainTypes { get; private set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x000081BE File Offset: 0x000063BE
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x000081C6 File Offset: 0x000063C6
		public ForestDensity ForestDensity { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x000081CF File Offset: 0x000063CF
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x000081D7 File Offset: 0x000063D7
		public bool IsSiegeMap { get; private set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x000081E0 File Offset: 0x000063E0
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x000081E8 File Offset: 0x000063E8
		public bool IsVillageMap { get; private set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x000081F1 File Offset: 0x000063F1
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x000081F9 File Offset: 0x000063F9
		public bool IsLordsHallMap { get; private set; }

		// Token: 0x060000F9 RID: 249 RVA: 0x00008202 File Offset: 0x00006402
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
