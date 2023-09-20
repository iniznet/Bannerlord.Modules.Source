using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	// Token: 0x0200001E RID: 30
	public class UserDataManager
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000129 RID: 297 RVA: 0x0000589A File Offset: 0x00003A9A
		// (set) Token: 0x0600012A RID: 298 RVA: 0x000058A2 File Offset: 0x00003AA2
		public UserData UserData { get; private set; }

		// Token: 0x0600012B RID: 299 RVA: 0x000058AC File Offset: 0x00003AAC
		public UserDataManager()
		{
			this.UserData = new UserData();
			string text = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			text += "\\Mount and Blade II Bannerlord\\Configs\\";
			if (!Directory.Exists(text))
			{
				try
				{
					Directory.CreateDirectory(text);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
			this._filePath = text + "LauncherData.xml";
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00005918 File Offset: 0x00003B18
		public bool HasUserData()
		{
			return File.Exists(this._filePath);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00005928 File Offset: 0x00003B28
		public void LoadUserData()
		{
			if (!File.Exists(this._filePath))
			{
				return;
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserData));
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(this._filePath))
				{
					this.UserData = (UserData)xmlSerializer.Deserialize(xmlReader);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		// Token: 0x0600012E RID: 302 RVA: 0x000059A4 File Offset: 0x00003BA4
		public void SaveUserData()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserData));
			try
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(this._filePath, new XmlWriterSettings
				{
					Indent = true
				}))
				{
					xmlSerializer.Serialize(xmlWriter, this.UserData);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		// Token: 0x04000092 RID: 146
		private const string DataFolder = "\\Mount and Blade II Bannerlord\\Configs\\";

		// Token: 0x04000093 RID: 147
		private const string FileName = "LauncherData.xml";

		// Token: 0x04000094 RID: 148
		private readonly string _filePath;
	}
}
