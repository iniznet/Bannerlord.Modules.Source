using System;

namespace TaleWorlds.Library
{
	public static class ConfigurationManager
	{
		public static void SetConfigurationManager(IConfigurationManager configurationManager)
		{
			ConfigurationManager._configurationManager = configurationManager;
		}

		public static string GetAppSettings(string name)
		{
			if (ConfigurationManager._configurationManager != null)
			{
				return ConfigurationManager._configurationManager.GetAppSettings(name);
			}
			return null;
		}

		private static IConfigurationManager _configurationManager;
	}
}
