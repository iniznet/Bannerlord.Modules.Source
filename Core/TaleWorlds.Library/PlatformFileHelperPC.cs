using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	// Token: 0x02000075 RID: 117
	public class PlatformFileHelperPC : IPlatformFileHelper
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x0000C420 File Offset: 0x0000A620
		private string DocumentsPath
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x0000C428 File Offset: 0x0000A628
		private string ProgramDataPath
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			}
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0000C431 File Offset: 0x0000A631
		public PlatformFileHelperPC(string applicationName)
		{
			this.ApplicationName = applicationName;
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x0000C440 File Offset: 0x0000A640
		public SaveResult SaveFile(PlatformFilePath path, byte[] data)
		{
			SaveResult saveResult = SaveResult.PlatformFileHelperFailure;
			PlatformFileHelperPC.Error = "";
			try
			{
				this.CreateDirectory(path.FolderPath);
				File.WriteAllBytes(this.GetFileFullPath(path), data);
				saveResult = SaveResult.Success;
			}
			catch (Exception ex)
			{
				PlatformFileHelperPC.Error = ex.Message;
				saveResult = SaveResult.PlatformFileHelperFailure;
			}
			return saveResult;
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0000C498 File Offset: 0x0000A698
		public SaveResult SaveFileString(PlatformFilePath path, string data)
		{
			SaveResult saveResult = SaveResult.PlatformFileHelperFailure;
			PlatformFileHelperPC.Error = "";
			try
			{
				this.CreateDirectory(path.FolderPath);
				File.WriteAllText(this.GetFileFullPath(path), data, Encoding.UTF8);
				saveResult = SaveResult.Success;
			}
			catch (Exception ex)
			{
				PlatformFileHelperPC.Error = ex.Message;
				saveResult = SaveResult.PlatformFileHelperFailure;
			}
			return saveResult;
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0000C4F4 File Offset: 0x0000A6F4
		public Task<SaveResult> SaveFileAsync(PlatformFilePath path, byte[] data)
		{
			return Task.FromResult<SaveResult>(this.SaveFile(path, data));
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x0000C503 File Offset: 0x0000A703
		public Task<SaveResult> SaveFileStringAsync(PlatformFilePath path, string data)
		{
			return Task.FromResult<SaveResult>(this.SaveFileString(path, data));
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0000C514 File Offset: 0x0000A714
		public SaveResult AppendLineToFileString(PlatformFilePath path, string data)
		{
			SaveResult saveResult = SaveResult.PlatformFileHelperFailure;
			PlatformFileHelperPC.Error = "";
			try
			{
				this.CreateDirectory(path.FolderPath);
				File.AppendAllText(this.GetFileFullPath(path), "\n" + data, Encoding.UTF8);
				saveResult = SaveResult.Success;
			}
			catch (Exception ex)
			{
				PlatformFileHelperPC.Error = ex.Message;
				saveResult = SaveResult.PlatformFileHelperFailure;
			}
			return saveResult;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x0000C578 File Offset: 0x0000A778
		private string GetDirectoryFullPath(PlatformDirectoryPath directoryPath)
		{
			string text = "";
			switch (directoryPath.Type)
			{
			case PlatformFileType.User:
				text = Path.Combine(this.DocumentsPath, this.ApplicationName);
				break;
			case PlatformFileType.Application:
				text = Path.Combine(this.ProgramDataPath, this.ApplicationName);
				break;
			case PlatformFileType.Temporary:
				text = Path.Combine(this.DocumentsPath, this.ApplicationName, "Temp");
				break;
			}
			return Path.Combine(text, directoryPath.Path);
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x0000C5F1 File Offset: 0x0000A7F1
		public string GetFileFullPath(PlatformFilePath filePath)
		{
			return Path.GetFullPath(Path.Combine(this.GetDirectoryFullPath(filePath.FolderPath), filePath.FileName));
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0000C60F File Offset: 0x0000A80F
		public bool FileExists(PlatformFilePath path)
		{
			return File.Exists(this.GetFileFullPath(path));
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0000C620 File Offset: 0x0000A820
		public async Task<string> GetFileContentStringAsync(PlatformFilePath path)
		{
			string text;
			if (!this.FileExists(path))
			{
				text = null;
			}
			else
			{
				string fileFullPath = this.GetFileFullPath(path);
				string text2 = string.Empty;
				using (FileStream sourceStream = File.Open(fileFullPath, FileMode.Open))
				{
					byte[] buffer = new byte[sourceStream.Length];
					await sourceStream.ReadAsync(buffer, 0, (int)sourceStream.Length);
					text2 = Encoding.UTF8.GetString(buffer);
					buffer = null;
				}
				FileStream sourceStream = null;
				text = text2;
			}
			return text;
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0000C670 File Offset: 0x0000A870
		public string GetFileContentString(PlatformFilePath path)
		{
			if (!this.FileExists(path))
			{
				return null;
			}
			string fileFullPath = this.GetFileFullPath(path);
			string text = null;
			PlatformFileHelperPC.Error = "";
			try
			{
				text = File.ReadAllText(fileFullPath, Encoding.UTF8);
			}
			catch (Exception ex)
			{
				PlatformFileHelperPC.Error = ex.Message;
				Debug.Print(PlatformFileHelperPC.Error, 0, Debug.DebugColor.White, 17592186044416UL);
			}
			return text;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0000C6E0 File Offset: 0x0000A8E0
		public byte[] GetFileContent(PlatformFilePath path)
		{
			if (!this.FileExists(path))
			{
				return null;
			}
			string fileFullPath = this.GetFileFullPath(path);
			byte[] array = null;
			PlatformFileHelperPC.Error = "";
			try
			{
				array = File.ReadAllBytes(fileFullPath);
			}
			catch (Exception ex)
			{
				PlatformFileHelperPC.Error = ex.Message;
				Debug.Print(PlatformFileHelperPC.Error, 0, Debug.DebugColor.White, 17592186044416UL);
			}
			return array;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x0000C748 File Offset: 0x0000A948
		public bool DeleteFile(PlatformFilePath path)
		{
			string fileFullPath = this.GetFileFullPath(path);
			if (!this.FileExists(path))
			{
				return false;
			}
			bool flag;
			try
			{
				File.Delete(fileFullPath);
				flag = true;
			}
			catch (Exception ex)
			{
				PlatformFileHelperPC.Error = ex.Message;
				Debug.Print(PlatformFileHelperPC.Error, 0, Debug.DebugColor.White, 17592186044416UL);
				flag = false;
			}
			return flag;
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0000C7A8 File Offset: 0x0000A9A8
		public void CreateDirectory(PlatformDirectoryPath path)
		{
			Directory.CreateDirectory(this.GetDirectoryFullPath(path));
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0000C7B8 File Offset: 0x0000A9B8
		public PlatformFilePath[] GetFiles(PlatformDirectoryPath path, string searchPattern)
		{
			string directoryFullPath = this.GetDirectoryFullPath(path);
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryFullPath);
			PlatformFilePath[] array = null;
			PlatformFileHelperPC.Error = "";
			if (directoryInfo.Exists)
			{
				try
				{
					FileInfo[] files = directoryInfo.GetFiles(searchPattern, SearchOption.AllDirectories);
					array = new PlatformFilePath[files.Length];
					for (int i = 0; i < files.Length; i++)
					{
						FileInfo fileInfo = files[i];
						fileInfo.FullName.Substring(directoryFullPath.Length);
						PlatformFilePath platformFilePath = new PlatformFilePath(path, fileInfo.Name);
						array[i] = platformFilePath;
					}
					return array;
				}
				catch (Exception ex)
				{
					PlatformFileHelperPC.Error = ex.Message;
					return array;
				}
			}
			array = new PlatformFilePath[0];
			return array;
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0000C864 File Offset: 0x0000AA64
		public void RenameFile(PlatformFilePath filePath, string newName)
		{
			string fileFullPath = this.GetFileFullPath(filePath);
			string fileFullPath2 = this.GetFileFullPath(new PlatformFilePath(filePath.FolderPath, newName));
			File.Move(fileFullPath, fileFullPath2);
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0000C891 File Offset: 0x0000AA91
		public string GetError()
		{
			return PlatformFileHelperPC.Error;
		}

		// Token: 0x04000134 RID: 308
		private readonly string ApplicationName;

		// Token: 0x04000135 RID: 309
		private static string Error;
	}
}
