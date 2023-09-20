using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	public class PlatformFileHelperPC : IPlatformFileHelper
	{
		private string DocumentsPath
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			}
		}

		private string ProgramDataPath
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			}
		}

		public PlatformFileHelperPC(string applicationName)
		{
			this.ApplicationName = applicationName;
		}

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

		public Task<SaveResult> SaveFileAsync(PlatformFilePath path, byte[] data)
		{
			return Task.FromResult<SaveResult>(this.SaveFile(path, data));
		}

		public Task<SaveResult> SaveFileStringAsync(PlatformFilePath path, string data)
		{
			return Task.FromResult<SaveResult>(this.SaveFileString(path, data));
		}

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

		public string GetFileFullPath(PlatformFilePath filePath)
		{
			return Path.GetFullPath(Path.Combine(this.GetDirectoryFullPath(filePath.FolderPath), filePath.FileName));
		}

		public bool FileExists(PlatformFilePath path)
		{
			return File.Exists(this.GetFileFullPath(path));
		}

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

		public void CreateDirectory(PlatformDirectoryPath path)
		{
			Directory.CreateDirectory(this.GetDirectoryFullPath(path));
		}

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

		public void RenameFile(PlatformFilePath filePath, string newName)
		{
			string fileFullPath = this.GetFileFullPath(filePath);
			string fileFullPath2 = this.GetFileFullPath(new PlatformFilePath(filePath.FolderPath, newName));
			File.Move(fileFullPath, fileFullPath2);
		}

		public string GetError()
		{
			return PlatformFileHelperPC.Error;
		}

		private readonly string ApplicationName;

		private static string Error;
	}
}
