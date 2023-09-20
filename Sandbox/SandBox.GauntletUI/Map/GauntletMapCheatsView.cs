using System;
using System.IO;
using System.Xml;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000025 RID: 37
	[OverrideView(typeof(MapCheatsView))]
	internal class GauntletMapCheatsView : MapView
	{
		// Token: 0x06000158 RID: 344 RVA: 0x0000A918 File Offset: 0x00008B18
		protected override void CreateLayout()
		{
			base.CreateLayout();
			bool flag = false;
			this._dataSource = new MapCheatsVM();
			try
			{
				string text = ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/sandbox_cheats.xml";
				XmlDocument xmlDocument = new XmlDocument();
				File.Exists(text);
				xmlDocument.Load(text);
				XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("Cheats").SelectNodes("Cheat");
				for (int i = 0; i < xmlNodeList.Count; i++)
				{
					XmlNode xmlNode = xmlNodeList[i];
					string value = xmlNode.Attributes["name"].Value;
					string value2 = xmlNode.Attributes["code"].Value;
					XmlAttribute xmlAttribute = xmlNode.Attributes["closeOnExecute"];
					bool flag2 = bool.Parse(((xmlAttribute != null) ? xmlAttribute.Value : null) ?? "false");
					this._dataSource.AddCheat(value, value2, flag2);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message, 0, 12, 17592186044416UL);
				Debug.Print(ex.StackTrace, 0, 12, 17592186044416UL);
			}
			if (flag)
			{
				base.Layer = new GauntletLayer(4500, "GauntletLayer", false);
				(base.Layer as GauntletLayer).LoadMovie("MapCheats", this._dataSource);
				base.Layer.InputRestrictions.SetInputRestrictions(false, 3);
				base.MapScreen.AddLayer(base.Layer);
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000AA94 File Offset: 0x00008C94
		protected override void OnFinalize()
		{
			base.OnFinalize();
			MapCheatsVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			if (base.Layer != null)
			{
				base.MapScreen.RemoveLayer(base.Layer);
			}
			base.Layer = null;
			this._dataSource = null;
		}

		// Token: 0x040000B1 RID: 177
		private MapCheatsVM _dataSource;
	}
}
