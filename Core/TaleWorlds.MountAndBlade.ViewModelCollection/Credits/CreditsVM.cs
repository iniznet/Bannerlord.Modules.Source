using System;
using System.IO;
using System.Xml;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Credits
{
	// Token: 0x0200010C RID: 268
	public class CreditsVM : ViewModel
	{
		// Token: 0x060017EF RID: 6127 RVA: 0x0004F205 File Offset: 0x0004D405
		public CreditsVM()
		{
			this.ExitKey = InputKeyItemVM.CreateFromHotKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"), false);
			this.ExitText = new TextObject("{=3CsACce8}Exit", null).ToString();
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x0004F244 File Offset: 0x0004D444
		private static CreditsItemVM CreateFromFile(string path)
		{
			CreditsItemVM creditsItemVM = null;
			try
			{
				if (File.Exists(path))
				{
					XmlDocument xmlDocument = new XmlDocument();
					using (XmlReader xmlReader = XmlReader.Create(path, new XmlReaderSettings
					{
						IgnoreComments = true
					}))
					{
						xmlDocument.Load(xmlReader);
					}
					XmlNode xmlNode = null;
					for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
					{
						XmlNode xmlNode2 = xmlDocument.ChildNodes.Item(i);
						if (xmlNode2.NodeType == XmlNodeType.Element && xmlNode2.Name == "Credits")
						{
							xmlNode = xmlNode2;
							break;
						}
					}
					if (xmlNode != null)
					{
						creditsItemVM = CreditsVM.CreateItem(xmlNode);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Print("Could not load Credits xml from " + path + ". Exception: " + ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				creditsItemVM = null;
			}
			return creditsItemVM;
		}

		// Token: 0x060017F1 RID: 6129 RVA: 0x0004F334 File Offset: 0x0004D534
		public void FillFromFile(string path)
		{
			try
			{
				if (File.Exists(path))
				{
					XmlDocument xmlDocument = new XmlDocument();
					using (XmlReader xmlReader = XmlReader.Create(path, new XmlReaderSettings
					{
						IgnoreComments = true
					}))
					{
						xmlDocument.Load(xmlReader);
					}
					XmlNode xmlNode = null;
					for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
					{
						XmlNode xmlNode2 = xmlDocument.ChildNodes.Item(i);
						if (xmlNode2.NodeType == XmlNodeType.Element && xmlNode2.Name == "Credits")
						{
							xmlNode = xmlNode2;
							break;
						}
					}
					if (xmlNode != null)
					{
						CreditsItemVM creditsItemVM = CreditsVM.CreateItem(xmlNode);
						this._rootItem = creditsItemVM;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Print("Could not load Credits xml. Exception: " + ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		// Token: 0x060017F2 RID: 6130 RVA: 0x0004F420 File Offset: 0x0004D620
		private static CreditsItemVM CreateItem(XmlNode node)
		{
			CreditsItemVM creditsItemVM = null;
			if (node.Name.ToLower() == "LoadFromFile".ToLower())
			{
				string value = node.Attributes["Name"].Value;
				string text = "";
				if (node.Attributes["PlatformSpecific"] != null && node.Attributes["PlatformSpecific"].Value.ToLower() == "true")
				{
					if (ApplicationPlatform.IsPlatformConsole())
					{
						text = "Console";
					}
					else
					{
						text = "PC";
					}
				}
				if (node.Attributes["ConsoleSpecific"] != null && node.Attributes["ConsoleSpecific"].Value.ToLower() == "true")
				{
					if (ApplicationPlatform.CurrentPlatform == Platform.Durango)
					{
						text = "XBox";
					}
					else if (ApplicationPlatform.CurrentPlatform == Platform.Orbis)
					{
						text = "PlayStation";
					}
					else
					{
						text = "PC";
					}
				}
				creditsItemVM = CreditsVM.CreateFromFile(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/" + value + text + ".xml");
			}
			else
			{
				creditsItemVM = new CreditsItemVM();
				creditsItemVM.Type = node.Name;
				if (node.Attributes["Text"] != null)
				{
					creditsItemVM.Text = new TextObject(node.Attributes["Text"].Value, null).ToString();
				}
				else
				{
					creditsItemVM.Text = "";
				}
				foreach (object obj in node.ChildNodes)
				{
					CreditsItemVM creditsItemVM2 = CreditsVM.CreateItem((XmlNode)obj);
					creditsItemVM.Items.Add(creditsItemVM2);
				}
			}
			return creditsItemVM;
		}

		// Token: 0x060017F3 RID: 6131 RVA: 0x0004F5F4 File Offset: 0x0004D7F4
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ExitKey.OnFinalize();
		}

		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x060017F4 RID: 6132 RVA: 0x0004F607 File Offset: 0x0004D807
		// (set) Token: 0x060017F5 RID: 6133 RVA: 0x0004F60F File Offset: 0x0004D80F
		[DataSourceProperty]
		public CreditsItemVM RootItem
		{
			get
			{
				return this._rootItem;
			}
			set
			{
				if (value != this._rootItem)
				{
					this._rootItem = value;
					base.OnPropertyChangedWithValue<CreditsItemVM>(value, "RootItem");
				}
			}
		}

		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x060017F6 RID: 6134 RVA: 0x0004F62D File Offset: 0x0004D82D
		// (set) Token: 0x060017F7 RID: 6135 RVA: 0x0004F635 File Offset: 0x0004D835
		[DataSourceProperty]
		public InputKeyItemVM ExitKey
		{
			get
			{
				return this._exitKey;
			}
			set
			{
				if (value != this._exitKey)
				{
					this._exitKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ExitKey");
				}
			}
		}

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x060017F8 RID: 6136 RVA: 0x0004F653 File Offset: 0x0004D853
		// (set) Token: 0x060017F9 RID: 6137 RVA: 0x0004F65B File Offset: 0x0004D85B
		[DataSourceProperty]
		public string ExitText
		{
			get
			{
				return this._exitText;
			}
			set
			{
				if (value != this._exitText)
				{
					this._exitText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExitText");
				}
			}
		}

		// Token: 0x04000B75 RID: 2933
		public CreditsItemVM _rootItem;

		// Token: 0x04000B76 RID: 2934
		private InputKeyItemVM _exitKey;

		// Token: 0x04000B77 RID: 2935
		private string _exitText;
	}
}
