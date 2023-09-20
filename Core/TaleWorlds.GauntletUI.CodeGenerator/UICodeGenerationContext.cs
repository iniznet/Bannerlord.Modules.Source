using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.Library.CodeGeneration;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	public class UICodeGenerationContext
	{
		public ResourceDepot ResourceDepot { get; private set; }

		public WidgetFactory WidgetFactory { get; private set; }

		public FontFactory FontFactory { get; private set; }

		public BrushFactory BrushFactory { get; private set; }

		public SpriteData SpriteData { get; private set; }

		public UICodeGenerationContext(string nameSpace, string outputFolder)
		{
			this._nameSpace = nameSpace;
			this._outputFolder = outputFolder;
			this._widgetTemplateGenerateContexts = new List<WidgetTemplateGenerateContext>();
		}

		public void Prepare(IEnumerable<string> resourceLocations, IEnumerable<PrefabExtension> prefabExtensions)
		{
			this.ResourceDepot = new ResourceDepot();
			foreach (string text in resourceLocations)
			{
				this.ResourceDepot.AddLocation(BasePath.Name, text);
			}
			this.ResourceDepot.CollectResources();
			this.WidgetFactory = new WidgetFactory(this.ResourceDepot, "Prefabs");
			foreach (PrefabExtension prefabExtension in prefabExtensions)
			{
				this.WidgetFactory.PrefabExtensionContext.AddExtension(prefabExtension);
			}
			this.WidgetFactory.Initialize(null);
			this.SpriteData = new SpriteData("SpriteData");
			this.SpriteData.Load(this.ResourceDepot);
			this.FontFactory = new FontFactory(this.ResourceDepot);
			this.FontFactory.LoadAllFonts(this.SpriteData);
			this.BrushFactory = new BrushFactory(this.ResourceDepot, "Brushes", this.SpriteData, this.FontFactory);
			this.BrushFactory.Initialize();
		}

		public void AddPrefabVariant(string prefabName, string variantName, UICodeGenerationVariantExtension variantExtension, Dictionary<string, object> data)
		{
			WidgetTemplateGenerateContext widgetTemplateGenerateContext = WidgetTemplateGenerateContext.CreateAsRoot(this, prefabName, variantName, variantExtension, data);
			this._widgetTemplateGenerateContexts.Add(widgetTemplateGenerateContext);
		}

		private static void ClearFolder(string folderName)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(folderName);
			FileInfo[] files = directoryInfo.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Delete();
			}
			foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
			{
				UICodeGenerationContext.ClearFolder(directoryInfo2.FullName);
				directoryInfo2.Delete();
			}
		}

		public void Generate()
		{
			Dictionary<string, CodeGenerationContext> dictionary = new Dictionary<string, CodeGenerationContext>();
			foreach (WidgetTemplateGenerateContext widgetTemplateGenerateContext in this._widgetTemplateGenerateContexts)
			{
				string text = widgetTemplateGenerateContext.PrefabName + ".gen.cs";
				if (!dictionary.ContainsKey(text))
				{
					dictionary.Add(text, new CodeGenerationContext());
				}
				NamespaceCode namespaceCode = dictionary[text].FindOrCreateNamespace(this._nameSpace);
				widgetTemplateGenerateContext.GenerateInto(namespaceCode);
			}
			string fullPath = Path.GetFullPath(Directory.GetCurrentDirectory() + "\\..\\..\\..\\Source\\" + this._outputFolder);
			UICodeGenerationContext.ClearFolder(fullPath);
			List<string> list = new List<string> { "System.Numerics", "TaleWorlds.Library" };
			foreach (KeyValuePair<string, CodeGenerationContext> keyValuePair in dictionary)
			{
				string key = keyValuePair.Key;
				CodeGenerationContext codeGenerationContext = dictionary[key];
				CodeGenerationFile codeGenerationFile = new CodeGenerationFile(list);
				codeGenerationContext.GenerateInto(codeGenerationFile);
				string text2 = codeGenerationFile.GenerateText();
				File.WriteAllText(fullPath + "\\" + key, text2, Encoding.UTF8);
			}
			CodeGenerationContext codeGenerationContext2 = new CodeGenerationContext();
			NamespaceCode namespaceCode2 = codeGenerationContext2.FindOrCreateNamespace(this._nameSpace);
			ClassCode classCode = new ClassCode();
			classCode.Name = "GeneratedUIPrefabCreator";
			classCode.AccessModifier = ClassCodeAccessModifier.Public;
			classCode.InheritedInterfaces.Add("TaleWorlds.GauntletUI.PrefabSystem.IGeneratedUIPrefabCreator");
			MethodCode methodCode = new MethodCode();
			methodCode.Name = "CollectGeneratedPrefabDefinitions";
			methodCode.MethodSignature = "(TaleWorlds.GauntletUI.PrefabSystem.GeneratedPrefabContext generatedPrefabContext)";
			foreach (WidgetTemplateGenerateContext widgetTemplateGenerateContext2 in this._widgetTemplateGenerateContexts)
			{
				MethodCode methodCode2 = widgetTemplateGenerateContext2.GenerateCreatorMethod();
				classCode.AddMethod(methodCode2);
				methodCode.AddLine(string.Concat(new string[] { "generatedPrefabContext.AddGeneratedPrefab(\"", widgetTemplateGenerateContext2.PrefabName, "\", \"", widgetTemplateGenerateContext2.VariantName, "\", ", methodCode2.Name, ");" }));
			}
			classCode.AddMethod(methodCode);
			namespaceCode2.AddClass(classCode);
			CodeGenerationFile codeGenerationFile2 = new CodeGenerationFile(null);
			codeGenerationContext2.GenerateInto(codeGenerationFile2);
			string text3 = codeGenerationFile2.GenerateText();
			File.WriteAllText(fullPath + "\\PrefabCodes.gen.cs", text3, Encoding.UTF8);
		}

		private List<WidgetTemplateGenerateContext> _widgetTemplateGenerateContexts;

		private readonly string _nameSpace;

		private readonly string _outputFolder;
	}
}
