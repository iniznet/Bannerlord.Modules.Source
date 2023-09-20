using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200001F RID: 31
	[Serializable]
	public class SteamAccessObject
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00002D06 File Offset: 0x00000F06
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00002D0E File Offset: 0x00000F0E
		public string UserName { get; private set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00002D17 File Offset: 0x00000F17
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00002D1F File Offset: 0x00000F1F
		public string ExternalAccessToken { get; private set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00002D28 File Offset: 0x00000F28
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00002D30 File Offset: 0x00000F30
		public int AppId { get; private set; }

		// Token: 0x06000094 RID: 148 RVA: 0x00002D39 File Offset: 0x00000F39
		public SteamAccessObject(string userName, string externalAccessToken, int appId)
		{
			this.UserName = userName;
			this.ExternalAccessToken = externalAccessToken;
			this.AppId = appId;
		}
	}
}
