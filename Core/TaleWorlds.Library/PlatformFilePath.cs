using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
	// Token: 0x02000076 RID: 118
	public struct PlatformFilePath
	{
		// Token: 0x060003F8 RID: 1016 RVA: 0x0000C898 File Offset: 0x0000AA98
		public PlatformFilePath(PlatformDirectoryPath folderPath, string fileName)
		{
			this.FolderPath = folderPath;
			this.FileName = fileName;
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0000C8A8 File Offset: 0x0000AAA8
		public static PlatformFilePath operator +(PlatformFilePath path, string str)
		{
			return new PlatformFilePath(path.FolderPath, path.FileName + str);
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x0000C8C1 File Offset: 0x0000AAC1
		public string FileFullPath
		{
			get
			{
				return Common.PlatformFileHelper.GetFileFullPath(this);
			}
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0000C8D4 File Offset: 0x0000AAD4
		public string GetFileNameWithoutExtension()
		{
			int num = this.FileName.LastIndexOf('.');
			if (num == -1)
			{
				return this.FileName;
			}
			return this.FileName.Substring(0, num);
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x0000C907 File Offset: 0x0000AB07
		public override string ToString()
		{
			return this.FolderPath.ToString() + " - " + this.FileName;
		}

		// Token: 0x04000136 RID: 310
		public PlatformDirectoryPath FolderPath;

		// Token: 0x04000137 RID: 311
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string FileName;
	}
}
