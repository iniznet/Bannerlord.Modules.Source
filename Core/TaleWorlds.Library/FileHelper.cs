using System;
using System.IO;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	// Token: 0x0200002C RID: 44
	public static class FileHelper
	{
		// Token: 0x0600015C RID: 348 RVA: 0x00005D64 File Offset: 0x00003F64
		public static SaveResult SaveFile(PlatformFilePath path, byte[] data)
		{
			return Common.PlatformFileHelper.SaveFile(path, data);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00005D72 File Offset: 0x00003F72
		public static SaveResult SaveFileString(PlatformFilePath path, string data)
		{
			return Common.PlatformFileHelper.SaveFileString(path, data);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00005D80 File Offset: 0x00003F80
		public static string GetFileFullPath(PlatformFilePath path)
		{
			return Common.PlatformFileHelper.GetFileFullPath(path);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00005D8D File Offset: 0x00003F8D
		public static SaveResult AppendLineToFileString(PlatformFilePath path, string data)
		{
			return Common.PlatformFileHelper.AppendLineToFileString(path, data);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00005D9B File Offset: 0x00003F9B
		public static Task<SaveResult> SaveFileAsync(PlatformFilePath path, byte[] data)
		{
			return Common.PlatformFileHelper.SaveFileAsync(path, data);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00005DA9 File Offset: 0x00003FA9
		public static Task<SaveResult> SaveFileStringAsync(PlatformFilePath path, string data)
		{
			return Common.PlatformFileHelper.SaveFileStringAsync(path, data);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00005DB7 File Offset: 0x00003FB7
		public static string GetError()
		{
			return Common.PlatformFileHelper.GetError();
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00005DC3 File Offset: 0x00003FC3
		public static bool FileExists(PlatformFilePath path)
		{
			return Common.PlatformFileHelper.FileExists(path);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00005DD0 File Offset: 0x00003FD0
		public static Task<string> GetFileContentStringAsync(PlatformFilePath path)
		{
			return Common.PlatformFileHelper.GetFileContentStringAsync(path);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00005DDD File Offset: 0x00003FDD
		public static string GetFileContentString(PlatformFilePath path)
		{
			return Common.PlatformFileHelper.GetFileContentString(path);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00005DEA File Offset: 0x00003FEA
		public static void DeleteFile(PlatformFilePath path)
		{
			Common.PlatformFileHelper.DeleteFile(path);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00005DF8 File Offset: 0x00003FF8
		public static PlatformFilePath[] GetFiles(PlatformDirectoryPath path, string searchPattern)
		{
			return Common.PlatformFileHelper.GetFiles(path, searchPattern);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00005E06 File Offset: 0x00004006
		public static byte[] GetFileContent(PlatformFilePath filePath)
		{
			return Common.PlatformFileHelper.GetFileContent(filePath);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00005E14 File Offset: 0x00004014
		public static void CopyFile(PlatformFilePath source, PlatformFilePath target)
		{
			byte[] fileContent = FileHelper.GetFileContent(source);
			FileHelper.SaveFile(target, fileContent);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00005E30 File Offset: 0x00004030
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
