using System;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000011 RID: 17
	public class LoadData
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002BB1 File Offset: 0x00000DB1
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002BB9 File Offset: 0x00000DB9
		public MetaData MetaData { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002BC2 File Offset: 0x00000DC2
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002BCA File Offset: 0x00000DCA
		public GameData GameData { get; private set; }

		// Token: 0x0600004F RID: 79 RVA: 0x00002BD3 File Offset: 0x00000DD3
		public LoadData(MetaData metaData, GameData gameData)
		{
			this.MetaData = metaData;
			this.GameData = gameData;
		}
	}
}
