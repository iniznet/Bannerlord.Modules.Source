using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
	public struct PlatformDirectoryPath
	{
		public PlatformDirectoryPath(PlatformFileType type, string path)
		{
			this.Type = type;
			this.Path = path;
		}

		public static PlatformDirectoryPath operator +(PlatformDirectoryPath path, string str)
		{
			return new PlatformDirectoryPath(path.Type, path.Path + str);
		}

		public override string ToString()
		{
			return this.Type + " " + this.Path;
		}

		public PlatformFileType Type;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string Path;
	}
}
