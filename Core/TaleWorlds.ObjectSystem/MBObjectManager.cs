using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.ObjectSystem
{
	public sealed class MBObjectManager
	{
		public static MBObjectManager Instance { get; private set; }

		private MBObjectManager()
		{
		}

		public static MBObjectManager Init()
		{
			MBObjectManager instance = MBObjectManager.Instance;
			MBObjectManager.Instance = new MBObjectManager();
			return MBObjectManager.Instance;
		}

		public void Destroy()
		{
			this.ClearAllObjects();
			MBObjectManager.Instance = null;
		}

		public int NumRegisteredTypes
		{
			get
			{
				if (this.ObjectTypeRecords == null)
				{
					return 0;
				}
				return this.ObjectTypeRecords.Count;
			}
		}

		public int MaxRegisteredTypes
		{
			get
			{
				return 256;
			}
		}

		public void RegisterType<T>(string classPrefix, string classListPrefix, uint typeId, bool autoCreateInstance = true, bool isTemporary = false) where T : MBObjectBase
		{
			if (this.NumRegisteredTypes > this.MaxRegisteredTypes)
			{
				Debug.FailedAssert(new MBTooManyRegisteredTypesException().ToString(), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "RegisterType", 66);
			}
			this.ObjectTypeRecords.Add(new MBObjectManager.ObjectTypeRecord<T>(typeId, classPrefix, classListPrefix, autoCreateInstance, isTemporary));
		}

		public bool HasType(Type type)
		{
			if (type.IsSealed)
			{
				using (List<MBObjectManager.IObjectTypeRecord>.Enumerator enumerator = this.ObjectTypeRecords.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.ObjectClass == type)
						{
							return true;
						}
					}
					return false;
				}
			}
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (type.IsAssignableFrom(objectTypeRecord.ObjectClass))
				{
					return true;
				}
			}
			return false;
		}

		public string FindRegisteredClassPrefix(Type type)
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.ObjectClass == type)
				{
					return objectTypeRecord.ElementName;
				}
			}
			Debug.FailedAssert(type.Name + " could not be found in MBObjectManager objectTypeRecords!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "FindRegisteredClassPrefix", 108);
			return null;
		}

		public Type FindRegisteredType(string classPrefix)
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.ElementName == classPrefix)
				{
					return objectTypeRecord.ObjectClass;
				}
			}
			Debug.FailedAssert(classPrefix + " could not be found in MBObjectManager objectTypeRecords!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "FindRegisteredType", 122);
			return null;
		}

		public T RegisterObject<T>(T obj) where T : MBObjectBase
		{
			MBObjectBase mbobjectBase;
			this.RegisterObjectInternalWithoutTypeId<T>(obj, false, out mbobjectBase);
			return mbobjectBase as T;
		}

		public T RegisterPresumedObject<T>(T obj) where T : MBObjectBase
		{
			MBObjectBase mbobjectBase;
			this.RegisterObjectInternalWithoutTypeId<T>(obj, true, out mbobjectBase);
			return mbobjectBase as T;
		}

		internal void TryRegisterObjectWithoutInitialization(MBObjectBase obj)
		{
			Type type = obj.GetType();
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.ObjectClass == type)
				{
					objectTypeRecord.RegisterMBObjectWithoutInitialization(obj);
					return;
				}
			}
			Debug.FailedAssert(obj.GetType().Name + " could not be found in MBObjectManager objectTypeRecords!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "TryRegisterObjectWithoutInitialization", 153);
		}

		private void RegisterObjectInternalWithoutTypeId<T>(T obj, bool presumed, out MBObjectBase registeredObject) where T : MBObjectBase
		{
			Type type = obj.GetType();
			type = typeof(T);
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.ObjectClass == type)
				{
					objectTypeRecord.RegisterMBObject(obj, presumed, out registeredObject);
					return;
				}
			}
			registeredObject = null;
			Debug.FailedAssert(typeof(T).Name + " could not be found in MBObjectManager objectTypeRecords!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "RegisterObjectInternalWithoutTypeId", 170);
		}

		public void UnregisterObject(MBObjectBase obj)
		{
			if (obj == null)
			{
				return;
			}
			Type type = obj.GetType();
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (type == objectTypeRecord.ObjectClass)
				{
					objectTypeRecord.UnregisterMBObject(obj);
					this.AfterUnregisterObject(obj);
					return;
				}
			}
			Debug.FailedAssert("UnregisterObject call for an unregistered object! Type: " + obj.GetType(), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "UnregisterObject", 192);
		}

		private void AfterUnregisterObject(MBObjectBase obj)
		{
			if (this._handlers != null)
			{
				foreach (IObjectManagerHandler objectManagerHandler in this._handlers)
				{
					objectManagerHandler.AfterUnregisterObject(obj);
				}
			}
		}

		public T GetObject<T>(Func<T, bool> predicate) where T : MBObjectBase
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsSealed)
			{
				using (List<MBObjectManager.IObjectTypeRecord>.Enumerator enumerator = this.ObjectTypeRecords.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MBObjectManager.IObjectTypeRecord objectTypeRecord = enumerator.Current;
						if (objectTypeRecord.ObjectClass == typeFromHandle)
						{
							return ((MBObjectManager.ObjectTypeRecord<T>)objectTypeRecord).FirstOrDefault(predicate);
						}
					}
					goto IL_B1;
				}
			}
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord2 in this.ObjectTypeRecords)
			{
				if (typeFromHandle.IsAssignableFrom(objectTypeRecord2.ObjectClass))
				{
					return objectTypeRecord2.OfType<T>().FirstOrDefault(predicate);
				}
			}
			IL_B1:
			Debug.FailedAssert(typeof(T).Name + " could not be found in MBObjectManager objectTypeRecords!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "GetObject", 232);
			return default(T);
		}

		public T GetObject<T>(string objectName) where T : MBObjectBase
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsSealed)
			{
				using (List<MBObjectManager.IObjectTypeRecord>.Enumerator enumerator = this.ObjectTypeRecords.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MBObjectManager.IObjectTypeRecord objectTypeRecord = enumerator.Current;
						if (objectTypeRecord.ObjectClass == typeFromHandle)
						{
							return ((MBObjectManager.ObjectTypeRecord<T>)objectTypeRecord).GetObject(objectName);
						}
					}
					goto IL_C3;
				}
			}
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord2 in this.ObjectTypeRecords)
			{
				if (typeFromHandle.IsAssignableFrom(objectTypeRecord2.ObjectClass))
				{
					T t = objectTypeRecord2.GetMBObject(objectName) as T;
					if (t != null)
					{
						return t;
					}
				}
			}
			IL_C3:
			return default(T);
		}

		public T GetFirstObject<T>() where T : MBObjectBase
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsSealed)
			{
				using (List<MBObjectManager.IObjectTypeRecord>.Enumerator enumerator = this.ObjectTypeRecords.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MBObjectManager.IObjectTypeRecord objectTypeRecord = enumerator.Current;
						if (objectTypeRecord.ObjectClass == typeFromHandle)
						{
							return ((MBObjectManager.ObjectTypeRecord<T>)objectTypeRecord).GetFirstObject();
						}
					}
					goto IL_C0;
				}
			}
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord2 in this.ObjectTypeRecords)
			{
				T t;
				if (typeFromHandle.IsAssignableFrom(objectTypeRecord2.ObjectClass) && (t = objectTypeRecord2.GetFirstMBObject() as T) != null)
				{
					return t;
				}
			}
			IL_C0:
			return default(T);
		}

		public bool ContainsObject<T>(string objectName) where T : MBObjectBase
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsSealed)
			{
				using (List<MBObjectManager.IObjectTypeRecord>.Enumerator enumerator = this.ObjectTypeRecords.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MBObjectManager.IObjectTypeRecord objectTypeRecord = enumerator.Current;
						if (objectTypeRecord.ObjectClass == typeFromHandle)
						{
							return objectTypeRecord.ContainsObject(objectName);
						}
					}
					return false;
				}
			}
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord2 in this.ObjectTypeRecords)
			{
				if (typeFromHandle.IsAssignableFrom(objectTypeRecord2.ObjectClass))
				{
					bool flag = objectTypeRecord2.ContainsObject(objectName);
					if (flag)
					{
						return flag;
					}
				}
			}
			return false;
		}

		public void RemoveTemporaryTypes()
		{
			for (int i = this.ObjectTypeRecords.Count - 1; i >= 0; i--)
			{
				MBObjectManager.IObjectTypeRecord objectTypeRecord = this.ObjectTypeRecords[i];
				if (objectTypeRecord.IsTemporary)
				{
					foreach (object obj in objectTypeRecord)
					{
						MBObjectBase mbobjectBase = (MBObjectBase)obj;
						this.UnregisterObject(mbobjectBase);
					}
					this.ObjectTypeRecords.Remove(objectTypeRecord);
				}
			}
		}

		public void PreAfterLoad()
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				objectTypeRecord.PreAfterLoad();
			}
		}

		public void AfterLoad()
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				objectTypeRecord.AfterLoad();
			}
		}

		public MBObjectBase GetObject(MBGUID objectId)
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.TypeNo == objectId.GetTypeIndex())
				{
					return objectTypeRecord.GetMBObject(objectId);
				}
			}
			Debug.FailedAssert(objectId.GetTypeIndex() + " could not be found in MBObjectManager objectTypeRecords!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "GetObject", 391);
			return null;
		}

		public MBObjectBase GetObject(string typeName, string objectName)
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.ElementName == typeName)
				{
					return objectTypeRecord.GetMBObject(objectName);
				}
			}
			Debug.FailedAssert(typeName + " could not be found in MBObjectManager objectTypeRecords!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "GetObject", 406);
			return null;
		}

		private MBObjectBase GetPresumedObject(string typeName, string objectName, bool isInitialize = false)
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.ElementName == typeName)
				{
					MBObjectBase mbobjectBase = objectTypeRecord.GetMBObject(objectName);
					if (mbobjectBase != null)
					{
						return mbobjectBase;
					}
					if (objectTypeRecord.AutoCreate)
					{
						mbobjectBase = objectTypeRecord.CreatePresumedMBObject(objectName);
						MBObjectBase mbobjectBase2;
						objectTypeRecord.RegisterMBObject(mbobjectBase, true, out mbobjectBase2);
						return mbobjectBase2;
					}
					throw new MBCanNotCreatePresumedObjectException();
				}
			}
			Debug.FailedAssert(typeName + " could not be found in MBObjectManager objectTypeRecords!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "GetPresumedObject", 434);
			return null;
		}

		public MBReadOnlyList<T> GetObjectTypeList<T>() where T : MBObjectBase
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsSealed)
			{
				using (List<MBObjectManager.IObjectTypeRecord>.Enumerator enumerator = this.ObjectTypeRecords.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MBObjectManager.IObjectTypeRecord objectTypeRecord = enumerator.Current;
						if (objectTypeRecord.ObjectClass == typeFromHandle)
						{
							return ((MBObjectManager.ObjectTypeRecord<T>)objectTypeRecord).GetObjectsList();
						}
					}
					goto IL_F4;
				}
				goto IL_64;
				IL_F4:
				Debug.FailedAssert(typeof(T).Name + " could not be found in MBObjectManager objectTypeRecords!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "GetObjectTypeList", 471);
				return null;
			}
			IL_64:
			MBList<T> mblist = new MBList<T>();
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord2 in this.ObjectTypeRecords)
			{
				if (typeFromHandle.IsAssignableFrom(objectTypeRecord2.ObjectClass))
				{
					foreach (object obj in objectTypeRecord2.GetList())
					{
						mblist.Add((T)((object)obj));
					}
				}
			}
			return mblist;
		}

		public IList<MBObjectBase> CreateObjectTypeList(Type objectClassType)
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.ObjectClass == objectClassType)
				{
					List<MBObjectBase> list = new List<MBObjectBase>();
					foreach (object obj in objectTypeRecord)
					{
						MBObjectBase mbobjectBase = obj as MBObjectBase;
						list.Add(mbobjectBase);
					}
					return list;
				}
			}
			return null;
		}

		public void LoadXML(string id, bool isDevelopment, string gameType, bool skipXmlFilterForEditor = false)
		{
			bool flag = skipXmlFilterForEditor || isDevelopment;
			XmlDocument mergedXmlForManaged = MBObjectManager.GetMergedXmlForManaged(id, false, flag, gameType);
			try
			{
				this.LoadXml(mergedXmlForManaged, isDevelopment);
			}
			catch (Exception ex)
			{
				Debug.ShowError("Could not load merged xml file correctly: " + id + "Error: " + ex.Message);
			}
		}

		public static XmlDocument GetMergedXmlForManaged(string id, bool skipValidation, bool ignoreGameTypeInclusionCheck = true, string gameType = "")
		{
			List<Tuple<string, string>> list = new List<Tuple<string, string>>();
			List<string> list2 = new List<string>();
			foreach (MbObjectXmlInformation mbObjectXmlInformation in XmlResource.XmlInformationList)
			{
				if (mbObjectXmlInformation.Id == id && (ignoreGameTypeInclusionCheck || mbObjectXmlInformation.GameTypesIncluded.Count == 0 || mbObjectXmlInformation.GameTypesIncluded.Contains(gameType)))
				{
					string xsdPath = ModuleHelper.GetXsdPath(mbObjectXmlInformation.Id);
					string text = ModuleHelper.GetXmlPath(mbObjectXmlInformation.ModuleName, mbObjectXmlInformation.Name);
					if (File.Exists(text))
					{
						list.Add(Tuple.Create<string, string>(ModuleHelper.GetXmlPath(mbObjectXmlInformation.ModuleName, mbObjectXmlInformation.Name), xsdPath));
						MBObjectManager.HandleXsltList(ModuleHelper.GetXsltPath(mbObjectXmlInformation.ModuleName, mbObjectXmlInformation.Name), ref list2);
					}
					else
					{
						string text2 = text.Replace(".xml", "");
						if (Directory.Exists(text2))
						{
							foreach (FileInfo fileInfo in new DirectoryInfo(text2).GetFiles("*.xml"))
							{
								text = text2 + "/" + fileInfo.Name;
								list.Add(Tuple.Create<string, string>(text, xsdPath));
								MBObjectManager.HandleXsltList(text.Replace(".xml", ".xsl"), ref list2);
							}
						}
						else
						{
							list.Add(Tuple.Create<string, string>("", ""));
							if (!MBObjectManager.HandleXsltList(ModuleHelper.GetXsltPath(mbObjectXmlInformation.ModuleName, mbObjectXmlInformation.Name), ref list2))
							{
								Debug.ShowError(string.Concat(new string[]
								{
									"Unable to find xml or xslt file for the entry '",
									ModuleHelper.GetModuleFullPath(mbObjectXmlInformation.ModuleName),
									"ModuleData/",
									mbObjectXmlInformation.Name,
									"' in SubModule.xml."
								}));
							}
						}
					}
				}
			}
			return MBObjectManager.CreateMergedXmlFile(list, list2, skipValidation);
		}

		public static XmlDocument GetMergedXmlForNative(string id, out List<string> usedPaths)
		{
			usedPaths = new List<string>();
			List<Tuple<string, string>> list = new List<Tuple<string, string>>();
			List<string> list2 = new List<string>();
			foreach (MbObjectXmlInformation mbObjectXmlInformation in XmlResource.MbprojXmls)
			{
				if (mbObjectXmlInformation.Id == id)
				{
					if (File.Exists(ModuleHelper.GetXmlPathForNative(mbObjectXmlInformation.ModuleName, mbObjectXmlInformation.Name)))
					{
						usedPaths.Add(ModuleHelper.GetXmlPathForNativeWBase(mbObjectXmlInformation.ModuleName, mbObjectXmlInformation.Name));
						list.Add(Tuple.Create<string, string>(ModuleHelper.GetXmlPathForNative(mbObjectXmlInformation.ModuleName, mbObjectXmlInformation.Name), string.Empty));
					}
					else
					{
						list.Add(Tuple.Create<string, string>("", ""));
					}
					MBObjectManager.HandleXsltList(ModuleHelper.GetXsltPathForNative(mbObjectXmlInformation.ModuleName, mbObjectXmlInformation.Name), ref list2);
				}
			}
			return MBObjectManager.CreateMergedXmlFile(list, list2, true);
		}

		private static bool HandleXsltList(string xslPath, ref List<string> xsltList)
		{
			string text = xslPath + "t";
			if (File.Exists(xslPath))
			{
				xsltList.Add(xslPath);
				return true;
			}
			if (File.Exists(text))
			{
				xsltList.Add(text);
				return true;
			}
			xsltList.Add("");
			return false;
		}

		public static XmlDocument CreateMergedXmlFile(List<Tuple<string, string>> toBeMerged, List<string> xsltList, bool skipValidation)
		{
			XmlDocument xmlDocument = MBObjectManager.CreateDocumentFromXmlFile(toBeMerged[0].Item1, toBeMerged[0].Item2, skipValidation);
			for (int i = 1; i < toBeMerged.Count; i++)
			{
				if (xsltList[i] != "")
				{
					xmlDocument = MBObjectManager.ApplyXslt(xsltList[i], xmlDocument);
				}
				if (toBeMerged[i].Item1 != "")
				{
					XmlDocument xmlDocument2 = MBObjectManager.CreateDocumentFromXmlFile(toBeMerged[i].Item1, toBeMerged[i].Item2, skipValidation);
					xmlDocument = MBObjectManager.MergeTwoXmls(xmlDocument, xmlDocument2);
				}
			}
			return xmlDocument;
		}

		public static XmlDocument ApplyXslt(string xsltPath, XmlDocument baseDocument)
		{
			XmlReader xmlReader = new XmlNodeReader(baseDocument);
			XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
			xslCompiledTransform.Load(xsltPath);
			XmlDocument xmlDocument = new XmlDocument(baseDocument.CreateNavigator().NameTable);
			using (XmlWriter xmlWriter = xmlDocument.CreateNavigator().AppendChild())
			{
				xslCompiledTransform.Transform(xmlReader, xmlWriter);
				xmlWriter.Close();
			}
			return xmlDocument;
		}

		public static XmlDocument MergeTwoXmls(XmlDocument xmlDocument1, XmlDocument xmlDocument2)
		{
			XDocument xdocument = MBObjectManager.ToXDocument(xmlDocument1);
			XDocument xdocument2 = MBObjectManager.ToXDocument(xmlDocument2);
			xdocument.Root.Add(xdocument2.Root.Elements());
			return MBObjectManager.ToXmlDocument(xdocument);
		}

		public static XDocument ToXDocument(XmlDocument xmlDocument)
		{
			XDocument xdocument;
			using (XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument))
			{
				xmlNodeReader.MoveToContent();
				xdocument = XDocument.Load(xmlNodeReader);
			}
			return xdocument;
		}

		public static XmlDocument ToXmlDocument(XDocument xDocument)
		{
			XmlDocument xmlDocument = new XmlDocument();
			using (xDocument.CreateReader())
			{
				xmlDocument.Load(xDocument.CreateReader());
			}
			return xmlDocument;
		}

		public void LoadOneXmlFromFile(string xmlPath, string xsdPath, bool skipValidation = false)
		{
			try
			{
				XmlDocument xmlDocument = MBObjectManager.CreateDocumentFromXmlFile(xmlPath, xsdPath, skipValidation);
				this.LoadXml(xmlDocument, false);
			}
			catch (Exception ex)
			{
				Debug.ShowError("Could not load xml file correctly: " + xmlPath + "Error:" + ex.Message);
			}
		}

		public XmlDocument LoadXMLFromFileSkipValidation(string xmlPath, string xsdPath)
		{
			XmlDocument xmlDocument;
			try
			{
				xmlDocument = MBObjectManager.CreateDocumentFromXmlFile(xmlPath, xsdPath, true);
			}
			catch
			{
				Debug.ShowError("Could not load xml file correctly: " + xmlPath);
				xmlDocument = null;
			}
			return xmlDocument;
		}

		private static void LoadXmlWithValidation(string xmlPath, string xsdPath, XmlDocument xmlDocument)
		{
			Debug.Print("opening " + xsdPath, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(xsdPath);
				xmlSchemaSet.Add(null, xmlTextReader);
			}
			catch (FileNotFoundException)
			{
				Debug.Print("xsd file of " + xmlPath + " could not be found!", 0, Debug.DebugColor.Red, 17592186044416UL);
				Debug.ShowError("xsd file of " + xmlPath + " could not be found!");
			}
			catch (Exception)
			{
				Debug.Print("xsd file of " + xmlPath + " could not be read!", 0, Debug.DebugColor.Red, 17592186044416UL);
				Debug.ShowError("xsd file of " + xmlPath + " could not be read!");
			}
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ValidationType = ValidationType.None;
			xmlReaderSettings.Schemas.Add(xmlSchemaSet);
			xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
			xmlReaderSettings.ValidationEventHandler += MBObjectManager.ValidationEventHandler;
			xmlReaderSettings.CloseInput = true;
			try
			{
				XmlReader xmlReader = XmlReader.Create(xmlPath, xmlReaderSettings);
				xmlDocument.Load(xmlReader);
				xmlReader.Close();
				XmlReaderSettings xmlReaderSettings2 = new XmlReaderSettings();
				xmlReaderSettings2.ValidationType = ValidationType.Schema;
				xmlReaderSettings2.Schemas.Add(xmlSchemaSet);
				xmlReaderSettings2.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
				xmlReaderSettings2.ValidationEventHandler += MBObjectManager.ValidationEventHandler;
				xmlReaderSettings2.CloseInput = true;
				xmlReader = XmlReader.Create(xmlPath, xmlReaderSettings2);
				xmlDocument.Load(xmlReader);
				xmlReader.Close();
			}
			catch (Exception ex)
			{
				string localPath = new Uri(xmlDocument.BaseURI).LocalPath;
				Debug.ShowError("Xml:" + localPath + "\n" + ex.Message);
			}
			if (xmlTextReader != null)
			{
				xmlTextReader.Close();
			}
		}

		private static void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			XmlReader xmlReader = (XmlReader)sender;
			string text = string.Empty;
			XmlSeverityType severity = e.Severity;
			if (severity != XmlSeverityType.Error)
			{
				if (severity == XmlSeverityType.Warning)
				{
					text = text + "Warning: " + e.Message;
				}
			}
			else
			{
				text = text + "Error: " + e.Message;
			}
			text = string.Concat(new string[] { text, "\nNode: ", xmlReader.Name, "  Value: ", xmlReader.Value });
			text = text + "\nLine: " + e.Exception.LineNumber;
			text = text + "\nXML Path: " + xmlReader.BaseURI;
			Debug.Print(text, 0, Debug.DebugColor.Red, 17592186044416UL);
			Debug.ShowError(text);
		}

		private static XmlDocument CreateDocumentFromXmlFile(string xmlPath, string xsdPath, bool forceSkipValidation = false)
		{
			Debug.Print("opening " + xmlPath, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(xmlPath);
			string text = streamReader.ReadToEnd();
			if (!forceSkipValidation)
			{
				MBObjectManager.LoadXmlWithValidation(xmlPath, xsdPath, xmlDocument);
			}
			else
			{
				xmlDocument.LoadXml(text);
			}
			streamReader.Close();
			return xmlDocument;
		}

		public void LoadXml(XmlDocument doc, bool isDevelopment = false)
		{
			int i = 0;
			bool flag = false;
			string text = null;
			while (i < doc.ChildNodes.Count)
			{
				int num = i;
				foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
				{
					if (objectTypeRecord.ElementListName == doc.ChildNodes[num].Name)
					{
						text = objectTypeRecord.ElementName;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
				i++;
			}
			if (flag)
			{
				for (XmlNode xmlNode = doc.ChildNodes[i].ChildNodes[0]; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode.NodeType != XmlNodeType.Comment)
					{
						string value = xmlNode.Attributes["id"].Value;
						MBObjectBase presumedObject = this.GetPresumedObject(text, value, true);
						presumedObject.Deserialize(this, xmlNode);
						presumedObject.AfterInitialized();
					}
				}
			}
		}

		public MBObjectBase CreateObjectFromXmlNode(XmlNode node)
		{
			string name = node.Name;
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.ElementName == name)
				{
					string value = node.Attributes["id"].Value;
					MBObjectBase presumedObject = this.GetPresumedObject(objectTypeRecord.ElementName, value, false);
					presumedObject.Deserialize(this, node);
					presumedObject.AfterInitialized();
					return presumedObject;
				}
			}
			return null;
		}

		public MBObjectBase CreateObjectWithoutDeserialize(XmlNode node)
		{
			string name = node.Name;
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				if (objectTypeRecord.ElementName == name)
				{
					string value = node.Attributes["id"].Value;
					MBObjectBase presumedObject = this.GetPresumedObject(objectTypeRecord.ElementName, value, false);
					presumedObject.Initialize();
					presumedObject.AfterInitialized();
					return presumedObject;
				}
			}
			return null;
		}

		public void UnregisterNonReadyObjects()
		{
			foreach (IEnumerable enumerable in this.ObjectTypeRecords)
			{
				List<MBObjectBase> list = new List<MBObjectBase>();
				foreach (object obj in enumerable)
				{
					MBObjectBase mbobjectBase = (MBObjectBase)obj;
					if (!mbobjectBase.IsReady)
					{
						list.Add(mbobjectBase);
					}
				}
				foreach (MBObjectBase mbobjectBase2 in list)
				{
					this.UnregisterObject(mbobjectBase2);
				}
			}
		}

		public void ClearAllObjects()
		{
			for (int i = this.ObjectTypeRecords.Count - 1; i >= 0; i--)
			{
				List<MBObjectBase> list = new List<MBObjectBase>();
				foreach (object obj in this.ObjectTypeRecords[i])
				{
					MBObjectBase mbobjectBase = (MBObjectBase)obj;
					list.Add(mbobjectBase);
				}
				foreach (MBObjectBase mbobjectBase2 in list)
				{
					this.ObjectTypeRecords[i].UnregisterMBObject(mbobjectBase2);
					this.AfterUnregisterObject(mbobjectBase2);
				}
			}
		}

		public void ClearAllObjectsWithType(Type type)
		{
			for (int i = this.ObjectTypeRecords.Count - 1; i >= 0; i--)
			{
				if (this.ObjectTypeRecords[i].ObjectClass == type)
				{
					List<MBObjectBase> list = new List<MBObjectBase>();
					foreach (object obj in this.ObjectTypeRecords[i])
					{
						MBObjectBase mbobjectBase = (MBObjectBase)obj;
						list.Add(mbobjectBase);
					}
					foreach (MBObjectBase mbobjectBase2 in list)
					{
						this.UnregisterObject(mbobjectBase2);
					}
				}
			}
		}

		public T ReadObjectReferenceFromXml<T>(string attributeName, XmlNode node) where T : MBObjectBase
		{
			if (node.Attributes[attributeName] == null)
			{
				return default(T);
			}
			string value = node.Attributes[attributeName].Value;
			string text = value.Split(".".ToCharArray())[0];
			if (text == value)
			{
				throw new MBInvalidReferenceException(value);
			}
			string text2 = value.Split(".".ToCharArray())[1];
			if (text == string.Empty || text2 == string.Empty)
			{
				throw new MBInvalidReferenceException(value);
			}
			return this.GetPresumedObject(text, text2, false) as T;
		}

		public MBObjectBase ReadObjectReferenceFromXml(string attributeName, Type objectType, XmlNode node)
		{
			if (node.Attributes[attributeName] == null)
			{
				return null;
			}
			string value = node.Attributes[attributeName].Value;
			string text = value.Split(".".ToCharArray())[0];
			if (text == value)
			{
				throw new MBInvalidReferenceException(value);
			}
			string text2 = value.Split(".".ToCharArray())[1];
			if (text == string.Empty || text2 == string.Empty)
			{
				throw new MBInvalidReferenceException(value);
			}
			return this.GetPresumedObject(text, text2, false);
		}

		public T CreateObject<T>(string stringId) where T : MBObjectBase, new()
		{
			T t = new T();
			t.StringId = stringId;
			this.RegisterObject<T>(t);
			if (this._handlers != null)
			{
				foreach (IObjectManagerHandler objectManagerHandler in this._handlers)
				{
					objectManagerHandler.AfterCreateObject(t);
				}
			}
			return t;
		}

		public T CreateObject<T>() where T : MBObjectBase, new()
		{
			return this.CreateObject<T>(typeof(T).Name.ToString() + "_1");
		}

		public void DebugPrint(PrintOutputDelegate printOutput)
		{
			printOutput("-Printing MBObjectManager Debug-");
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				printOutput(objectTypeRecord.DebugBasicDump());
			}
		}

		public void AddHandler(IObjectManagerHandler handler)
		{
			if (this._handlers == null)
			{
				this._handlers = new List<IObjectManagerHandler>();
			}
			this._handlers.Add(handler);
		}

		public void RemoveHandler(IObjectManagerHandler handler)
		{
			this._handlers.Remove(handler);
		}

		public string DebugDump()
		{
			string text = "";
			text += "--------------------------------------\r\n";
			text += "----Printing MBObjectManager Debug----\r\n";
			text += "--------------------------------------\r\n";
			text += "\r\n";
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				text += objectTypeRecord.DebugDump();
			}
			File.WriteAllText("mbobjectmanagerdump.txt", text);
			return text;
		}

		public void ReInitialize()
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords.ToList<MBObjectManager.IObjectTypeRecord>())
			{
				objectTypeRecord.ReInitialize();
			}
		}

		public string GetObjectTypeIds()
		{
			string text = "";
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				text = string.Concat(new object[]
				{
					text,
					objectTypeRecord.TypeNo,
					" - ",
					objectTypeRecord.GetType().FullName,
					"\n"
				});
			}
			return text;
		}

		internal List<MBObjectManager.IObjectTypeRecord> ObjectTypeRecords = new List<MBObjectManager.IObjectTypeRecord>();

		private List<IObjectManagerHandler> _handlers;

		internal interface IObjectTypeRecord : IEnumerable
		{
			bool AutoCreate { get; }

			string ElementName { get; }

			string ElementListName { get; }

			Type ObjectClass { get; }

			uint TypeNo { get; }

			bool IsTemporary { get; }

			void ReInitialize();

			MBObjectBase CreatePresumedMBObject(string objectName);

			void RegisterMBObject(MBObjectBase obj, bool presumed, out MBObjectBase registeredObject);

			void RegisterMBObjectWithoutInitialization(MBObjectBase obj);

			void UnregisterMBObject(MBObjectBase obj);

			MBObjectBase GetFirstMBObject();

			MBObjectBase GetMBObject(string objId);

			MBObjectBase GetMBObject(MBGUID objId);

			bool ContainsObject(string objId);

			string DebugDump();

			string DebugBasicDump();

			IEnumerable GetList();

			void PreAfterLoad();

			void AfterLoad();
		}

		internal class ObjectTypeRecord<T> : MBObjectManager.IObjectTypeRecord, IEnumerable, IEnumerable<T> where T : MBObjectBase
		{
			bool MBObjectManager.IObjectTypeRecord.AutoCreate
			{
				get
				{
					return this._autoCreate;
				}
			}

			string MBObjectManager.IObjectTypeRecord.ElementName
			{
				get
				{
					return this._elementName;
				}
			}

			string MBObjectManager.IObjectTypeRecord.ElementListName
			{
				get
				{
					return this._elementListName;
				}
			}

			Type MBObjectManager.IObjectTypeRecord.ObjectClass
			{
				get
				{
					return typeof(T);
				}
			}

			uint MBObjectManager.IObjectTypeRecord.TypeNo
			{
				get
				{
					return this._typeNo;
				}
			}

			bool MBObjectManager.IObjectTypeRecord.IsTemporary
			{
				get
				{
					return this._isTemporary;
				}
			}

			internal MBList<T> RegisteredObjectsList { get; }

			internal ObjectTypeRecord(uint newTypeNo, string classPrefix, string classListPrefix, bool autoCreate, bool isTemporary)
			{
				this._typeNo = newTypeNo;
				this._elementName = classPrefix;
				this._elementListName = classListPrefix;
				this._autoCreate = autoCreate;
				this._isTemporary = isTemporary;
				this._registeredObjects = new Dictionary<string, T>();
				this._registeredObjectsWithGuid = new Dictionary<MBGUID, T>();
				this.RegisteredObjectsList = new MBList<T>();
				this._objCount = 0U;
			}

			void MBObjectManager.IObjectTypeRecord.ReInitialize()
			{
				uint num = 0U;
				foreach (T t in this.RegisteredObjectsList)
				{
					uint subId = t.Id.SubId;
					if (subId > num)
					{
						num = subId;
					}
				}
				this._objCount = num + 1U;
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return this.EnumerateElements();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.EnumerateElements();
			}

			internal MBGUID GetNewId()
			{
				uint typeNo = this._typeNo;
				uint num = this._objCount + 1U;
				this._objCount = num;
				return new MBGUID(typeNo, num);
			}

			MBObjectBase MBObjectManager.IObjectTypeRecord.CreatePresumedMBObject(string objectName)
			{
				return this.CreatePresumedObject(objectName);
			}

			private T CreatePresumedObject(string objectName)
			{
				T t = Activator.CreateInstance<T>();
				t.StringId = objectName;
				t.IsReady = false;
				t.IsInitialized = false;
				return t;
			}

			MBObjectBase MBObjectManager.IObjectTypeRecord.GetMBObject(string objId)
			{
				return this.GetObject(objId);
			}

			MBObjectBase MBObjectManager.IObjectTypeRecord.GetFirstMBObject()
			{
				return this.GetFirstObject();
			}

			internal T GetFirstObject()
			{
				if (this.RegisteredObjectsList.Count <= 0)
				{
					return default(T);
				}
				return this.RegisteredObjectsList[0];
			}

			internal T GetObject(string objId)
			{
				T t;
				this._registeredObjects.TryGetValue(objId, out t);
				return t;
			}

			bool MBObjectManager.IObjectTypeRecord.ContainsObject(string objId)
			{
				return this._registeredObjects.ContainsKey(objId);
			}

			public MBObjectBase GetMBObject(MBGUID objId)
			{
				T t = default(T);
				this._registeredObjectsWithGuid.TryGetValue(objId, out t);
				return t;
			}

			void MBObjectManager.IObjectTypeRecord.RegisterMBObjectWithoutInitialization(MBObjectBase mbObject)
			{
				T t = (T)((object)mbObject);
				if (!string.IsNullOrEmpty(t.StringId) && t.Id.InternalValue != 0U && !this._registeredObjects.ContainsKey(t.StringId))
				{
					this._registeredObjects.Add(t.StringId, t);
					this._registeredObjectsWithGuid.Add(t.Id, t);
					this.RegisteredObjectsList.Add(t);
				}
			}

			void MBObjectManager.IObjectTypeRecord.RegisterMBObject(MBObjectBase obj, bool presumed, out MBObjectBase registeredObject)
			{
				if (obj is T)
				{
					this.RegisterObject(obj as T, presumed, out registeredObject);
					return;
				}
				registeredObject = null;
			}

			internal void RegisterObject(T obj, bool presumed, out MBObjectBase registeredObject)
			{
				T t;
				if (this._registeredObjects.TryGetValue(obj.StringId, out t))
				{
					if (t == obj || presumed)
					{
						registeredObject = t;
						return;
					}
					ValueTuple<string, long> idParts = this.GetIdParts(obj.StringId);
					string item = idParts.Item1;
					long num = idParts.Item2;
					if (this._registeredObjects.ContainsKey(obj.StringId))
					{
						num = (long)((ulong)this._objCount);
						obj.StringId = item + num.ToString();
						while (this._registeredObjects.ContainsKey(obj.StringId))
						{
							num += 1L;
							obj.StringId = item + num.ToString();
						}
					}
				}
				this._registeredObjects.Add(obj.StringId, obj);
				obj.Id = this.GetNewId();
				this._registeredObjectsWithGuid.Add(obj.Id, obj);
				this.RegisteredObjectsList.Add(obj);
				obj.IsReady = !presumed;
				obj.OnRegistered();
				registeredObject = obj;
			}

			[return: TupleElementNames(new string[] { "str", "number" })]
			private ValueTuple<string, long> GetIdParts(string stringId)
			{
				int num = stringId.Length - 1;
				while (num > 0 && char.IsDigit(stringId[num]))
				{
					num--;
				}
				string text = stringId.Substring(0, num + 1);
				long num2 = 0L;
				if (num < stringId.Length - 1)
				{
					long.TryParse(stringId.Substring(num + 1, stringId.Length - num - 1), out num2);
				}
				return new ValueTuple<string, long>(text, num2);
			}

			void MBObjectManager.IObjectTypeRecord.UnregisterMBObject(MBObjectBase obj)
			{
				if (obj is T)
				{
					this.UnregisterObject((T)((object)obj));
					return;
				}
				throw new MBIllegalRegisterException();
			}

			private void UnregisterObject(T obj)
			{
				obj.OnUnregistered();
				if (this._registeredObjects.ContainsKey(obj.StringId) && this._registeredObjects[obj.StringId] == obj)
				{
					this._registeredObjects.Remove(obj.StringId);
				}
				if (this._registeredObjectsWithGuid.ContainsKey(obj.Id) && this._registeredObjectsWithGuid[obj.Id] == obj)
				{
					this._registeredObjectsWithGuid.Remove(obj.Id);
				}
				this.RegisteredObjectsList.Remove(obj);
			}

			internal MBReadOnlyList<T> GetObjectsList()
			{
				return this.RegisteredObjectsList;
			}

			IEnumerable MBObjectManager.IObjectTypeRecord.GetList()
			{
				return this.RegisteredObjectsList;
			}

			string MBObjectManager.IObjectTypeRecord.DebugDump()
			{
				string text = "";
				text += "**************************************\r\n";
				text = string.Concat(new object[] { text, this._elementName, " ", this._objCount, "\r\n" });
				text += "**************************************\r\n";
				text += "\r\n";
				foreach (KeyValuePair<MBGUID, T> keyValuePair in this._registeredObjectsWithGuid)
				{
					text = string.Concat(new string[]
					{
						text,
						keyValuePair.Key.ToString(),
						" ",
						keyValuePair.Value.ToString(),
						"\r\n"
					});
				}
				return text;
			}

			string MBObjectManager.IObjectTypeRecord.DebugBasicDump()
			{
				return this._elementName + " " + this._objCount;
			}

			private IEnumerator<T> EnumerateElements()
			{
				int num;
				for (int i = 0; i < this.RegisteredObjectsList.Count; i = num + 1)
				{
					yield return this.RegisteredObjectsList[i];
					num = i;
				}
				yield break;
			}

			void MBObjectManager.IObjectTypeRecord.PreAfterLoad()
			{
				for (int i = this.RegisteredObjectsList.Count - 1; i >= 0; i--)
				{
					this.RegisteredObjectsList[i].PreAfterLoadInternal();
				}
			}

			void MBObjectManager.IObjectTypeRecord.AfterLoad()
			{
				for (int i = this.RegisteredObjectsList.Count - 1; i >= 0; i--)
				{
					this.RegisteredObjectsList[i].AfterLoadInternal();
				}
			}

			private readonly bool _autoCreate;

			private readonly string _elementName;

			private readonly string _elementListName;

			private uint _objCount;

			private readonly uint _typeNo;

			private readonly bool _isTemporary;

			private readonly Dictionary<string, T> _registeredObjects;

			private readonly Dictionary<MBGUID, T> _registeredObjectsWithGuid;
		}
	}
}
