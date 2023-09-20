using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
	public struct PlatformFilePath
	{
		public PlatformFilePath(PlatformDirectoryPath folderPath, string fileName)
		{
			this.FolderPath = folderPath;
			this.FileName = fileName;
		}

		public static PlatformFilePath operator +(PlatformFilePath path, string str)
		{
			return new PlatformFilePath(path.FolderPath, path.FileName + str);
		}

		public string FileFullPath
		{
			get
			{
				return Common.PlatformFileHelper.GetFileFullPath(this);
			}
		}

		public string GetFileNameWithoutExtension()
		{
			int num = this.FileName.LastIndexOf('.');
			if (num == -1)
			{
				return this.FileName;
			}
			return this.FileName.Substring(0, num);
		}

		public override string ToString()
		{
			return this.FolderPath.ToString() + " - " + this.FileName;
		}

		public PlatformDirectoryPath FolderPath;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string FileName;
	}
}
