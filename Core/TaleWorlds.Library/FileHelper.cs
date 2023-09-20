using System;
using System.IO;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	public static class FileHelper
	{
		public static SaveResult SaveFile(PlatformFilePath path, byte[] data)
		{
			return Common.PlatformFileHelper.SaveFile(path, data);
		}

		public static SaveResult SaveFileString(PlatformFilePath path, string data)
		{
			return Common.PlatformFileHelper.SaveFileString(path, data);
		}

		public static string GetFileFullPath(PlatformFilePath path)
		{
			return Common.PlatformFileHelper.GetFileFullPath(path);
		}

		public static SaveResult AppendLineToFileString(PlatformFilePath path, string data)
		{
			return Common.PlatformFileHelper.AppendLineToFileString(path, data);
		}

		public static Task<SaveResult> SaveFileAsync(PlatformFilePath path, byte[] data)
		{
			return Common.PlatformFileHelper.SaveFileAsync(path, data);
		}

		public static Task<SaveResult> SaveFileStringAsync(PlatformFilePath path, string data)
		{
			return Common.PlatformFileHelper.SaveFileStringAsync(path, data);
		}

		public static string GetError()
		{
			return Common.PlatformFileHelper.GetError();
		}

		public static bool FileExists(PlatformFilePath path)
		{
			return Common.PlatformFileHelper.FileExists(path);
		}

		public static Task<string> GetFileContentStringAsync(PlatformFilePath path)
		{
			return Common.PlatformFileHelper.GetFileContentStringAsync(path);
		}

		public static string GetFileContentString(PlatformFilePath path)
		{
			return Common.PlatformFileHelper.GetFileContentString(path);
		}

		public static void DeleteFile(PlatformFilePath path)
		{
			Common.PlatformFileHelper.DeleteFile(path);
		}

		public static PlatformFilePath[] GetFiles(PlatformDirectoryPath path, string searchPattern)
		{
			return Common.PlatformFileHelper.GetFiles(path, searchPattern);
		}

		public static byte[] GetFileContent(PlatformFilePath filePath)
		{
			return Common.PlatformFileHelper.GetFileContent(filePath);
		}

		public static void CopyFile(PlatformFilePath source, PlatformFilePath target)
		{
			byte[] fileContent = FileHelper.GetFileContent(source);
			FileHelper.SaveFile(target, fileContent);
		}

		public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);
			if (!directoryInfo.Exists)
			{
				return;
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			Directory.CreateDirectory(destinationDir);
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				string text = Path.Combine(destinationDir, fileInfo.Name);
				fileInfo.CopyTo(text);
			}
			if (recursive)
			{
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					string text2 = Path.Combine(destinationDir, directoryInfo2.Name);
					FileHelper.CopyDirectory(directoryInfo2.FullName, text2, true);
				}
			}
		}
	}
}
