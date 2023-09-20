using System;
using System.IO;
using System.Reflection;

namespace TaleWorlds.Library
{
	// Token: 0x0200002E RID: 46
	public static class BasePath
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00005F08 File Offset: 0x00004108
		public static string Name
		{
			get
			{
				if (ApplicationPlatform.CurrentEngine == EngineType.UnrealEngine)
				{
					return Path.GetFullPath(Path.GetDirectoryName(typeof(BasePath).Assembly.Location) + "/../../");
				}
				if (ApplicationPlatform.CurrentPlatform == Platform.Orbis)
				{
					return "/app0/";
				}
				if (ApplicationPlatform.CurrentPlatform == Platform.Durango)
				{
					return "/";
				}
				if (ApplicationPlatform.CurrentPlatform == Platform.Web)
				{
					return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/../../";
				}
				return "../../";
			}
		}
	}
}
