using System;

namespace SandBox.View.Missions.SandBox
{
	// Token: 0x02000025 RID: 37
	public class SpawnPointUnits
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000C821 File Offset: 0x0000AA21
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x0000C829 File Offset: 0x0000AA29
		public string SpName { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x0000C832 File Offset: 0x0000AA32
		// (set) Token: 0x060000FA RID: 250 RVA: 0x0000C83A File Offset: 0x0000AA3A
		public SpawnPointUnits.SceneType Place { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000FB RID: 251 RVA: 0x0000C843 File Offset: 0x0000AA43
		// (set) Token: 0x060000FC RID: 252 RVA: 0x0000C84B File Offset: 0x0000AA4B
		public int MinCount { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000FD RID: 253 RVA: 0x0000C854 File Offset: 0x0000AA54
		// (set) Token: 0x060000FE RID: 254 RVA: 0x0000C85C File Offset: 0x0000AA5C
		public int MaxCount { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000FF RID: 255 RVA: 0x0000C865 File Offset: 0x0000AA65
		// (set) Token: 0x06000100 RID: 256 RVA: 0x0000C86D File Offset: 0x0000AA6D
		public string Type { get; private set; }

		// Token: 0x06000101 RID: 257 RVA: 0x0000C876 File Offset: 0x0000AA76
		public SpawnPointUnits(string sp_name, SpawnPointUnits.SceneType place, int minCount, int maxCount)
		{
			this.SpName = sp_name;
			this.Place = place;
			this.MinCount = minCount;
			this.MaxCount = maxCount;
			this.CurrentCount = 0;
			this.SpawnedAgentCount = 0;
			this.Type = "other";
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000C8B4 File Offset: 0x0000AAB4
		public SpawnPointUnits(string sp_name, SpawnPointUnits.SceneType place, string type, int minCount, int maxCount)
		{
			this.SpName = sp_name;
			this.Place = place;
			this.Type = type;
			this.MinCount = minCount;
			this.MaxCount = maxCount;
			this.CurrentCount = 0;
			this.SpawnedAgentCount = 0;
		}

		// Token: 0x04000084 RID: 132
		public int CurrentCount;

		// Token: 0x04000086 RID: 134
		public int SpawnedAgentCount;

		// Token: 0x0200006C RID: 108
		public enum SceneType
		{
			// Token: 0x0400026B RID: 619
			Center,
			// Token: 0x0400026C RID: 620
			Tavern,
			// Token: 0x0400026D RID: 621
			VillageCenter,
			// Token: 0x0400026E RID: 622
			Arena,
			// Token: 0x0400026F RID: 623
			LordsHall,
			// Token: 0x04000270 RID: 624
			Castle,
			// Token: 0x04000271 RID: 625
			Dungeon,
			// Token: 0x04000272 RID: 626
			EmptyShop,
			// Token: 0x04000273 RID: 627
			All,
			// Token: 0x04000274 RID: 628
			NotDetermined
		}
	}
}
