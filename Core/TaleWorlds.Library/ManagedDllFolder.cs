using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200005B RID: 91
	public static class ManagedDllFolder
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000289 RID: 649 RVA: 0x000073AC File Offset: 0x000055AC
		public static string Name
		{
			get
			{
				if (!string.IsNullOrEmpty(ManagedDllFolder._overridenFolder))
				{
					return ManagedDllFolder._overridenFolder;
				}
				if (ApplicationPlatform.CurrentPlatform == Platform.Orbis)
				{
					return "/app0/";
				}
				if (ApplicationPlatform.CurrentPlatform == Platform.Durango)
				{
					return "/";
				}
				return "";
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x000073E1 File Offset: 0x000055E1
		public static void OverrideManagedDllFolder(string overridenFolder)
		{
			ManagedDllFolder._overridenFolder = overridenFolder;
		}

		// Token: 0x040000F3 RID: 243
		private static string _overridenFolder;
	}
}
