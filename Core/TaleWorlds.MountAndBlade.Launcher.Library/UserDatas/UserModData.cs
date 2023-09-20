using System;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	// Token: 0x0200001B RID: 27
	public class UserModData
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00005773 File Offset: 0x00003973
		// (set) Token: 0x06000114 RID: 276 RVA: 0x0000577B File Offset: 0x0000397B
		public string Id { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00005784 File Offset: 0x00003984
		// (set) Token: 0x06000116 RID: 278 RVA: 0x0000578C File Offset: 0x0000398C
		public string LastKnownVersion { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00005795 File Offset: 0x00003995
		// (set) Token: 0x06000118 RID: 280 RVA: 0x0000579D File Offset: 0x0000399D
		public bool IsSelected { get; set; }

		// Token: 0x06000119 RID: 281 RVA: 0x000057A6 File Offset: 0x000039A6
		public UserModData()
		{
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000057AE File Offset: 0x000039AE
		public UserModData(string id, string lastKnownVersion, bool isSelected)
		{
			this.Id = id;
			this.LastKnownVersion = lastKnownVersion;
			this.IsSelected = isSelected;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000057CC File Offset: 0x000039CC
		public bool IsUpdatedToBeDefault(ModuleInfo module)
		{
			return this.LastKnownVersion != module.Version.ToString() && module.IsDefault;
		}
	}
}
