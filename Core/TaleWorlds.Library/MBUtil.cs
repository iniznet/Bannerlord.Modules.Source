using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TaleWorlds.Library
{
	// Token: 0x02000068 RID: 104
	public static class MBUtil
	{
		// Token: 0x06000395 RID: 917 RVA: 0x0000B434 File Offset: 0x00009634
		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
			if (!directoryInfo.Exists)
			{
				return;
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				string text = Path.Combine(destDirName, fileInfo.Name);
				fileInfo.CopyTo(text, false);
			}
			if (copySubDirs)
			{
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					string text2 = Path.Combine(destDirName, directoryInfo2.Name);
					MBUtil.DirectoryCopy(directoryInfo2.FullName, text2, copySubDirs);
				}
			}
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0000B4D4 File Offset: 0x000096D4
		public static T[] ArrayAdd<T>(T[] tArray, T t)
		{
			List<T> list = tArray.ToList<T>();
			list.Add(t);
			return list.ToArray();
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0000B4E8 File Offset: 0x000096E8
		public static T[] ArrayRemove<T>(T[] tArray, T t)
		{
			List<T> list = tArray.ToList<T>();
			if (!list.Remove(t))
			{
				return tArray;
			}
			return list.ToArray();
		}
	}
}
