using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public static class EngineFilePaths
	{
		public static PlatformDirectoryPath ConfigsPath
		{
			get
			{
				return new PlatformDirectoryPath(PlatformFileType.User, "Configs");
			}
		}

		public const string ConfigsDirectoryName = "Configs";
	}
}
