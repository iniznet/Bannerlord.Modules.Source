using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	public class UserDataManager
	{
		public UserData UserData { get; private set; }

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

		public bool HasUserData()
		{
			return File.Exists(this._filePath);
		}

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

		private const string DataFolder = "\\Mount and Blade II Bannerlord\\Configs\\";

		private const string FileName = "LauncherData.xml";

		private readonly string _filePath;
	}
}
