using System;

namespace TaleWorlds.Diamond.ChatSystem.Library
{
	// Token: 0x02000008 RID: 8
	[Serializable]
	public class UserInfo
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000023CD File Offset: 0x000005CD
		// (set) Token: 0x0600001D RID: 29 RVA: 0x000023D5 File Offset: 0x000005D5
		public string UserName { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000023DE File Offset: 0x000005DE
		// (set) Token: 0x0600001F RID: 31 RVA: 0x000023E6 File Offset: 0x000005E6
		public string DisplayName { get; set; }

		// Token: 0x06000020 RID: 32 RVA: 0x000023EF File Offset: 0x000005EF
		public UserInfo(string userName, string displayName)
		{
			this.UserName = userName;
			this.DisplayName = displayName;
		}
	}
}
