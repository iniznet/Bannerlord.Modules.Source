using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000040 RID: 64
	public static class EngineFilePaths
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x00003313 File Offset: 0x00001513
		public static PlatformDirectoryPath ConfigsPath
		{
			get
			{
				return new PlatformDirectoryPath(PlatformFileType.User, "Configs");
			}
		}

		// Token: 0x04000049 RID: 73
		public const string ConfigsDirectoryName = "Configs";
	}
}
