using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	public class FileDriver : ISaveDriver
	{
		private static PlatformDirectoryPath SavePath
		{
			get
			{
				string text = "Game Saves\\";
				return new PlatformDirectoryPath(PlatformFileType.User, text);
			}
		}

		private PlatformFilePath GetSaveFilePath(string fileName)
		{
			return new PlatformFilePath(FileDriver.SavePath, fileName);
		}

		public Task<SaveResultWithMessage> Save(string saveName, int version, MetaData metaData, GameData gameData)
		{
			SaveResult saveResult = SaveResult.FileDriverFailure;
			PlatformFilePath saveFilePath = this.GetSaveFilePath(saveName + ".sav");
			MemoryStream memoryStream = new MemoryStream();
			metaData.Add("Version", version.ToString());
			metaData.Serialize(memoryStream);
			using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionLevel.Fastest, true))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(deflateStream))
				{
					GameData.Write(binaryWriter, gameData);
				}
			}
			ArraySegment<byte> arraySegment;
			if (memoryStream.TryGetBuffer(out arraySegment))
			{
				byte[] array = arraySegment.Array;
				Array.Resize<byte>(ref array, arraySegment.Count);
				saveResult = FileHelper.SaveFile(saveFilePath, array);
			}
			memoryStream.Close();
			string error = Common.PlatformFileHelper.GetError();
			return Task.FromResult<SaveResultWithMessage>(new SaveResultWithMessage(saveResult, error));
		}

		public MetaData LoadMetaData(string saveName)
		{
			byte[] fileContent = FileHelper.GetFileContent(this.GetSaveFilePath(saveName + ".sav"));
			if (fileContent != null)
			{
				return MetaData.Deserialize(new MemoryStream(fileContent));
			}
			Debug.Print("[Load meta data error]: " + saveName, 0, Debug.DebugColor.White, 17592186044416UL);
			return null;
		}

		public LoadData Load(string saveName)
		{
			byte[] fileContent = FileHelper.GetFileContent(this.GetSaveFilePath(saveName + ".sav"));
			if (fileContent != null)
			{
				MemoryStream memoryStream = new MemoryStream(fileContent);
				MetaData metaData = MetaData.Deserialize(memoryStream);
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
				{
					try
					{
						GameData gameData;
						if (this.GetApplicationVersionOfMetaData(metaData) < ApplicationVersion.FromString("v1.1.0", 21456))
						{
							gameData = (GameData)new BinaryFormatter().Deserialize(deflateStream);
							return new LoadData(metaData, gameData);
						}
						using (BinaryReader binaryReader = new BinaryReader(deflateStream))
						{
							gameData = GameData.Read(binaryReader);
						}
						return new LoadData(metaData, gameData);
					}
					catch (Exception ex)
					{
						Debug.Print(ex.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
						return null;
					}
				}
			}
			Debug.Print("[Load error]: " + saveName, 0, Debug.DebugColor.White, 17592186044416UL);
			return null;
		}

		public SaveGameFileInfo[] GetSaveGameFileInfos()
		{
			PlatformFilePath[] files = FileHelper.GetFiles(FileDriver.SavePath, "*.sav");
			List<SaveGameFileInfo> list = new List<SaveGameFileInfo>((files != null) ? files.Length : 0);
			if (files != null)
			{
				foreach (PlatformFilePath platformFilePath in files)
				{
					string fileNameWithoutExtension = platformFilePath.GetFileNameWithoutExtension();
					MetaData metaData = SaveManager.LoadMetaData(fileNameWithoutExtension, this);
					list.Add(new SaveGameFileInfo
					{
						Name = fileNameWithoutExtension,
						MetaData = metaData,
						IsCorrupted = (metaData == null || this.GetApplicationVersionOfMetaData(metaData) == ApplicationVersion.Empty)
					});
				}
			}
			return list.ToArray();
		}

		private ApplicationVersion GetApplicationVersionOfMetaData(MetaData metaData)
		{
			string text = ((metaData != null) ? metaData["ApplicationVersion"] : null);
			if (text == null)
			{
				return ApplicationVersion.Empty;
			}
			return ApplicationVersion.FromString(text, 21456);
		}

		public string[] GetSaveGameFileNames()
		{
			List<string> list = new List<string>();
			PlatformFilePath[] files = FileHelper.GetFiles(FileDriver.SavePath, "*.sav");
			if (files != null)
			{
				foreach (PlatformFilePath platformFilePath in files)
				{
					string fileNameWithoutExtension = platformFilePath.GetFileNameWithoutExtension();
					list.Add(fileNameWithoutExtension);
				}
			}
			return list.ToArray();
		}

		public bool Delete(string saveName)
		{
			PlatformFilePath saveFilePath = this.GetSaveFilePath(saveName + ".sav");
			if (FileHelper.FileExists(saveFilePath))
			{
				FileHelper.DeleteFile(saveFilePath);
				return true;
			}
			return false;
		}

		public bool IsSaveGameFileExists(string saveName)
		{
			return FileHelper.FileExists(this.GetSaveFilePath(saveName + ".sav"));
		}

		public bool IsWorkingAsync()
		{
			return false;
		}

		private const string SaveDirectoryName = "Game Saves";
	}
}
