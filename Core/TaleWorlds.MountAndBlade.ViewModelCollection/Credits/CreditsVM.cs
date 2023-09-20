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
	public class CreditsVM : ViewModel
	{
		public CreditsVM()
		{
			this.ExitKey = InputKeyItemVM.CreateFromHotKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"), false);
			this.ExitText = new TextObject("{=3CsACce8}Exit", null).ToString();
		}

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

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ExitKey.OnFinalize();
		}

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

		public CreditsItemVM _rootItem;

		private InputKeyItemVM _exitKey;

		private string _exitText;
	}
}
