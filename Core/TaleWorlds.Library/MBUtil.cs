using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TaleWorlds.Library
{
	public static class MBUtil
	{
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

		public static T[] ArrayAdd<T>(T[] tArray, T t)
		{
			List<T> list = tArray.ToList<T>();
			list.Add(t);
			return list.ToArray();
		}

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
