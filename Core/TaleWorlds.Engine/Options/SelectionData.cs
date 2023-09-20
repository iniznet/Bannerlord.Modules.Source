using System;

namespace TaleWorlds.Engine.Options
{
	// Token: 0x0200009E RID: 158
	public struct SelectionData
	{
		// Token: 0x06000BB8 RID: 3000 RVA: 0x0000CFE3 File Offset: 0x0000B1E3
		public SelectionData(bool isLocalizationId, string data)
		{
			this.IsLocalizationId = isLocalizationId;
			this.Data = data;
		}

		// Token: 0x040001F5 RID: 501
		public bool IsLocalizationId;

		// Token: 0x040001F6 RID: 502
		public string Data;
	}
}
