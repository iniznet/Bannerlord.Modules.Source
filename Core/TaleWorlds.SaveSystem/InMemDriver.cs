using System;
using System.IO;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x0200000F RID: 15
	public class InMemDriver : ISaveDriver
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00002AB8 File Offset: 0x00000CB8
		public Task<SaveResultWithMessage> Save(string saveName, int version, MetaData metaData, GameData gameData)
		{
			byte[] data = gameData.GetData();
			MemoryStream memoryStream = new MemoryStream();
			metaData.Add("version", version.ToString());
			metaData.Serialize(memoryStream);
			memoryStream.Write(data, 0, data.Length);
			this._data = memoryStream.GetBuffer();
			return Task.FromResult<SaveResultWithMessage>(SaveResultWithMessage.Default);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002B10 File Offset: 0x00000D10
		public MetaData LoadMetaData(string saveName)
		{
			MemoryStream memoryStream = new MemoryStream(this._data);
			MetaData metaData = MetaData.Deserialize(memoryStream);
			memoryStream.Close();
			return metaData;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002B38 File Offset: 0x00000D38
		public LoadData Load(string saveName)
		{
			MemoryStream memoryStream = new MemoryStream(this._data);
			MetaData metaData = MetaData.Deserialize(memoryStream);
			byte[] array = new byte[memoryStream.Length - memoryStream.Position];
			memoryStream.Read(array, 0, array.Length);
			GameData gameData = GameData.CreateFrom(array);
			return new LoadData(metaData, gameData);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002B84 File Offset: 0x00000D84
		public SaveGameFileInfo[] GetSaveGameFileInfos()
		{
			return new SaveGameFileInfo[0];
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002B8C File Offset: 0x00000D8C
		public string[] GetSaveGameFileNames()
		{
			return new string[0];
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002B94 File Offset: 0x00000D94
		public bool Delete(string saveName)
		{
			this._data = new byte[0];
			return true;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002BA3 File Offset: 0x00000DA3
		public bool IsSaveGameFileExists(string saveName)
		{
			return false;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002BA6 File Offset: 0x00000DA6
		public bool IsWorkingAsync()
		{
			return false;
		}

		// Token: 0x04000016 RID: 22
		private byte[] _data;
	}
}
