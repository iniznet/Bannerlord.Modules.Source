using System;

namespace TaleWorlds.Library
{
	public static class ManagedDllFolder
	{
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

		public static void OverrideManagedDllFolder(string overridenFolder)
		{
			ManagedDllFolder._overridenFolder = overridenFolder;
		}

		private static string _overridenFolder;
	}
}
