using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace TaleWorlds.Library.NewsManager
{
	public class NewsManager
	{
		public MBReadOnlyList<NewsItem> NewsItems
		{
			get
			{
				return this._newsItems;
			}
		}

		public bool IsInPreviewMode { get; private set; }

		public string LocalizationID { get; private set; }

		public NewsManager()
		{
			this._newsItems = new MBList<NewsItem>();
			this.UpdateConfigSettings();
		}

		public async Task<MBReadOnlyList<NewsItem>> GetNewsItems(bool forceRefresh)
		{
			await this.UpdateNewsItems(forceRefresh);
			return this.NewsItems;
		}

		public void SetNewsSourceURL(string url)
		{
			this._newsSourceURL = url;
		}

		public async Task UpdateNewsItems(bool forceRefresh)
		{
			if (ApplicationPlatform.CurrentPlatform != Platform.Durango && ApplicationPlatform.CurrentPlatform != Platform.GDKDesktop)
			{
				if (this._isNewsItemCacheDirty || forceRefresh)
				{
					try
					{
						if (Uri.IsWellFormedUriString(this._newsSourceURL, UriKind.Absolute))
						{
							this._newsItems = await NewsManager.DeserializeObjectAsync<MBList<NewsItem>>(await HttpHelper.DownloadStringTaskAsync(this._newsSourceURL));
						}
						else
						{
							Debug.FailedAssert("News file doesn't exist", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\NewsSystem\\NewsManager.cs", "UpdateNewsItems", 74);
						}
					}
					catch (Exception)
					{
					}
					this._isNewsItemCacheDirty = false;
				}
			}
		}

		public static Task<T> DeserializeObjectAsync<T>(string json)
		{
			Task<T> task;
			try
			{
				using (new StringReader(json))
				{
					task = Task.FromResult<T>(JsonConvert.DeserializeObject<T>(json));
				}
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				task = Task.FromResult<T>(default(T));
			}
			return task;
		}

		private void UpdateConfigSettings()
		{
			this._configPath = this.GetConfigXMLPath();
			this.IsInPreviewMode = false;
			this.LocalizationID = "en";
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(this._configPath);
				this.IsInPreviewMode = this.GetIsInPreviewMode(xmlDocument);
				this.LocalizationID = this.GetLocalizationCode(xmlDocument);
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		private bool GetIsInPreviewMode(XmlDocument configDocument)
		{
			return configDocument != null && configDocument.HasChildNodes && bool.Parse(configDocument.ChildNodes[0].SelectSingleNode("UsePreviewLink").Attributes["Value"].InnerText);
		}

		private string GetLocalizationCode(XmlDocument configDocument)
		{
			if (configDocument != null && configDocument.HasChildNodes)
			{
				return configDocument.ChildNodes[0].SelectSingleNode("LocalizationID").Attributes["Value"].InnerText;
			}
			return "en";
		}

		public void UpdateLocalizationID(string localizationID)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(this._configPath);
			if (xmlDocument.HasChildNodes)
			{
				xmlDocument.ChildNodes[0].SelectSingleNode("LocalizationID").Attributes["Value"].Value = localizationID;
			}
			xmlDocument.Save(this._configPath);
		}

		private PlatformFilePath GetConfigXMLPath()
		{
			PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.User, "Configs");
			PlatformFilePath platformFilePath = new PlatformFilePath(platformDirectoryPath, "NewsFeedConfig.xml");
			bool flag = FileHelper.FileExists(platformFilePath);
			bool flag2 = true;
			if (flag)
			{
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.Load(platformFilePath);
					flag2 = xmlDocument.HasChildNodes && xmlDocument.FirstChild.HasChildNodes;
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
					flag2 = false;
				}
			}
			if (!flag || !flag2)
			{
				try
				{
					XmlDocument xmlDocument2 = new XmlDocument();
					XmlNode xmlNode = xmlDocument2.CreateElement("Root");
					xmlDocument2.AppendChild(xmlNode);
					((XmlElement)xmlNode.AppendChild(xmlDocument2.CreateElement("LocalizationID"))).SetAttribute("Value", "en");
					((XmlElement)xmlNode.AppendChild(xmlDocument2.CreateElement("UsePreviewLink"))).SetAttribute("Value", "False");
					xmlDocument2.Save(platformFilePath);
				}
				catch (Exception ex2)
				{
					Debug.Print(ex2.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				}
			}
			return platformFilePath;
		}

		public void OnFinalize()
		{
			MBList<NewsItem> newsItems = this._newsItems;
			if (newsItems != null)
			{
				newsItems.Clear();
			}
			this._newsItems = null;
			this.LocalizationID = null;
		}

		private string _newsSourceURL;

		private MBList<NewsItem> _newsItems;

		private bool _isNewsItemCacheDirty = true;

		private PlatformFilePath _configPath;

		private const string DataFolder = "Configs";

		private const string FileName = "NewsFeedConfig.xml";
	}
}
