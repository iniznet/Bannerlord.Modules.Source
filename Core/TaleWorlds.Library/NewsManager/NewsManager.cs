using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace TaleWorlds.Library.NewsManager
{
	// Token: 0x020000A0 RID: 160
	public class NewsManager
	{
		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x00012A9E File Offset: 0x00010C9E
		public MBReadOnlyList<NewsItem> NewsItems
		{
			get
			{
				return this._newsItems;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060005D7 RID: 1495 RVA: 0x00012AA6 File Offset: 0x00010CA6
		// (set) Token: 0x060005D8 RID: 1496 RVA: 0x00012AAE File Offset: 0x00010CAE
		public bool IsInPreviewMode { get; private set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x00012AB7 File Offset: 0x00010CB7
		// (set) Token: 0x060005DA RID: 1498 RVA: 0x00012ABF File Offset: 0x00010CBF
		public string LocalizationID { get; private set; }

		// Token: 0x060005DB RID: 1499 RVA: 0x00012AC8 File Offset: 0x00010CC8
		public NewsManager()
		{
			this._newsItems = new MBList<NewsItem>();
			this.UpdateConfigSettings();
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x00012AE8 File Offset: 0x00010CE8
		public async Task<MBReadOnlyList<NewsItem>> GetNewsItems(bool forceRefresh)
		{
			await this.UpdateNewsItems(forceRefresh);
			return this.NewsItems;
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x00012B35 File Offset: 0x00010D35
		public void SetNewsSourceURL(string url)
		{
			this._newsSourceURL = url;
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x00012B40 File Offset: 0x00010D40
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

		// Token: 0x060005DF RID: 1503 RVA: 0x00012B90 File Offset: 0x00010D90
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

		// Token: 0x060005E0 RID: 1504 RVA: 0x00012C00 File Offset: 0x00010E00
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

		// Token: 0x060005E1 RID: 1505 RVA: 0x00012C84 File Offset: 0x00010E84
		private bool GetIsInPreviewMode(XmlDocument configDocument)
		{
			return configDocument != null && configDocument.HasChildNodes && bool.Parse(configDocument.ChildNodes[0].SelectSingleNode("UsePreviewLink").Attributes["Value"].InnerText);
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x00012CC2 File Offset: 0x00010EC2
		private string GetLocalizationCode(XmlDocument configDocument)
		{
			if (configDocument != null && configDocument.HasChildNodes)
			{
				return configDocument.ChildNodes[0].SelectSingleNode("LocalizationID").Attributes["Value"].InnerText;
			}
			return "en";
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x00012D00 File Offset: 0x00010F00
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

		// Token: 0x060005E4 RID: 1508 RVA: 0x00012D60 File Offset: 0x00010F60
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

		// Token: 0x060005E5 RID: 1509 RVA: 0x00012E90 File Offset: 0x00011090
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

		// Token: 0x040001B0 RID: 432
		private string _newsSourceURL;

		// Token: 0x040001B1 RID: 433
		private MBList<NewsItem> _newsItems;

		// Token: 0x040001B2 RID: 434
		private bool _isNewsItemCacheDirty = true;

		// Token: 0x040001B5 RID: 437
		private PlatformFilePath _configPath;

		// Token: 0x040001B6 RID: 438
		private const string DataFolder = "Configs";

		// Token: 0x040001B7 RID: 439
		private const string FileName = "NewsFeedConfig.xml";
	}
}
