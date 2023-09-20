using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000022 RID: 34
	public static class ConfigurationManager
	{
		// Token: 0x060000DD RID: 221 RVA: 0x00004FA3 File Offset: 0x000031A3
		public static void SetConfigurationManager(IConfigurationManager configurationManager)
		{
			ConfigurationManager._configurationManager = configurationManager;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00004FAB File Offset: 0x000031AB
		public static string GetAppSettings(string name)
		{
			if (ConfigurationManager._configurationManager != null)
			{
				return ConfigurationManager._configurationManager.GetAppSettings(name);
			}
			return null;
		}

		// Token: 0x0400006B RID: 107
		private static IConfigurationManager _configurationManager;
	}
}
