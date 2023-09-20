using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public static class FilePaths
	{
		public static PlatformDirectoryPath SavePath
		{
			get
			{
				return new PlatformDirectoryPath(PlatformFileType.User, "Game Saves");
			}
		}

		public static PlatformDirectoryPath RecordingsPath
		{
			get
			{
				return new PlatformDirectoryPath(PlatformFileType.User, "Recordings");
			}
		}

		public static PlatformDirectoryPath StatisticsPath
		{
			get
			{
				return new PlatformDirectoryPath(PlatformFileType.User, "Statistics");
			}
		}

		public const string SaveDirectoryName = "Game Saves";

		public const string RecordingsDirectoryName = "Recordings";

		public const string StatisticsDirectoryName = "Statistics";
	}
}
