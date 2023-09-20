using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
	// Token: 0x02000073 RID: 115
	public struct PlatformDirectoryPath
	{
		// Token: 0x060003E2 RID: 994 RVA: 0x0000C3DA File Offset: 0x0000A5DA
		public PlatformDirectoryPath(PlatformFileType type, string path)
		{
			this.Type = type;
			this.Path = path;
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0000C3EA File Offset: 0x0000A5EA
		public static PlatformDirectoryPath operator +(PlatformDirectoryPath path, string str)
		{
			return new PlatformDirectoryPath(path.Type, path.Path + str);
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x0000C403 File Offset: 0x0000A603
		public override string ToString()
		{
			return this.Type + " " + this.Path;
		}

		// Token: 0x0400012E RID: 302
		public PlatformFileType Type;

		// Token: 0x0400012F RID: 303
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string Path;
	}
}
