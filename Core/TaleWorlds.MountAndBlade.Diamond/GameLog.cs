using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000110 RID: 272
	[Serializable]
	public class GameLog
	{
		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000502 RID: 1282 RVA: 0x00007A0C File Offset: 0x00005C0C
		// (set) Token: 0x06000503 RID: 1283 RVA: 0x00007A14 File Offset: 0x00005C14
		public int Id { get; set; }

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000504 RID: 1284 RVA: 0x00007A1D File Offset: 0x00005C1D
		// (set) Token: 0x06000505 RID: 1285 RVA: 0x00007A25 File Offset: 0x00005C25
		public GameLogType Type { get; set; }

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000506 RID: 1286 RVA: 0x00007A2E File Offset: 0x00005C2E
		// (set) Token: 0x06000507 RID: 1287 RVA: 0x00007A36 File Offset: 0x00005C36
		public PlayerId Player { get; set; }

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x00007A3F File Offset: 0x00005C3F
		// (set) Token: 0x06000509 RID: 1289 RVA: 0x00007A47 File Offset: 0x00005C47
		public float GameTime { get; set; }

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x0600050A RID: 1290 RVA: 0x00007A50 File Offset: 0x00005C50
		// (set) Token: 0x0600050B RID: 1291 RVA: 0x00007A58 File Offset: 0x00005C58
		public Dictionary<string, string> Data { get; set; }

		// Token: 0x0600050C RID: 1292 RVA: 0x00007A61 File Offset: 0x00005C61
		public GameLog()
		{
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x00007A69 File Offset: 0x00005C69
		public GameLog(GameLogType type, PlayerId player, float gameTime)
		{
			this.Type = type;
			this.Player = player;
			this.GameTime = gameTime;
			this.Data = new Dictionary<string, string>();
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x00007A94 File Offset: 0x00005C94
		public string GetDataAsString()
		{
			string text = "{}";
			try
			{
				text = JsonConvert.SerializeObject(this.Data, Formatting.None);
			}
			catch (Exception)
			{
			}
			return text;
		}
	}
}
