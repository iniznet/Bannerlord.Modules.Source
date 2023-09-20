using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x02000059 RID: 89
	public static class FilePaths
	{
		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000657 RID: 1623 RVA: 0x00017093 File Offset: 0x00015293
		public static PlatformDirectoryPath SavePath
		{
			get
			{
				return new PlatformDirectoryPath(PlatformFileType.User, "Game Saves");
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x000170A0 File Offset: 0x000152A0
		public static PlatformDirectoryPath RecordingsPath
		{
			get
			{
				return new PlatformDirectoryPath(PlatformFileType.User, "Recordings");
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000659 RID: 1625 RVA: 0x000170AD File Offset: 0x000152AD
		public static PlatformDirectoryPath StatisticsPath
		{
			get
			{
				return new PlatformDirectoryPath(PlatformFileType.User, "Statistics");
			}
		}

		// Token: 0x04000339 RID: 825
		public const string SaveDirectoryName = "Game Saves";

		// Token: 0x0400033A RID: 826
		public const string RecordingsDirectoryName = "Recordings";

		// Token: 0x0400033B RID: 827
		public const string StatisticsDirectoryName = "Statistics";
	}
}
