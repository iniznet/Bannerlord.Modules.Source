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
	// Token: 0x02000008 RID: 8
	public sealed class MBObjectManager
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002398 File Offset: 0x00000598
		// (set) Token: 0x06000038 RID: 56 RVA: 0x0000239F File Offset: 0x0000059F
		public static MBObjectManager Instance { get; private set; }

		// Token: 0x06000039 RID: 57 RVA: 0x000023A7 File Offset: 0x000005A7
		private MBObjectManager()
		{
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000023BA File Offset: 0x000005BA
		public static MBObjectManager Init()
		{
			MBObjectManager instance = MBObjectManager.Instance;
			MBObjectManager.Instance = new MBObjectManager();
			return MBObjectManager.Instance;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000023D1 File Offset: 0x000005D1
		public void Destroy()
		{
			this.ClearAllObjects();
			MBObjectManager.Instance = null;
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600003C RID: 60 RVA: 0x000023DF File Offset: 0x000005DF
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600003D RID: 61 RVA: 0x000023F6 File Offset: 0x000005F6
		public int MaxRegisteredTypes
		{
			get
			{
				return 256;
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002400 File Offset: 0x00000600
		public void RegisterType<T>(string classPrefix, string classListPrefix, uint typeId, bool autoCreateInstance = true, bool isTemporary = false) where T : MBObjectBase
		{
			if (this.NumRegisteredTypes > this.MaxRegisteredTypes)
			{
				Debug.FailedAssert(new MBTooManyRegisteredTypesException().ToString(), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.ObjectSystem\\MBObjectManager.cs", "RegisterType", 66);
			}
			this.ObjectTypeRecords.Add(new MBObjectManager.ObjectTypeRecord<T>(typeId, classPrefix, classListPrefix, autoCreateInstance, isTemporary));
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002450 File Offset: 0x00000650
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

		// Token: 0x06000040 RID: 64 RVA: 0x00002504 File Offset: 0x00000704
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

		// Token: 0x06000041 RID: 65 RVA: 0x0000258C File Offset: 0x0000078C
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

		// Token: 0x06000042 RID: 66 RVA: 0x00002610 File Offset: 0x00000810
		public T RegisterObject<T>(T obj) where T : MBObjectBase
		{
			MBObjectBase mbobjectBase;
			this.RegisterObjectInternalWithoutTypeId<T>(obj, false, out mbobjectBase);
			return mbobjectBase as T;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002634 File Offset: 0x00000834
		public T RegisterPresumedObject<T>(T obj) where T : MBObjectBase
		{
			MBObjectBase mbobjectBase;
			this.RegisterObjectInternalWithoutTypeId<T>(obj, true, out mbobjectBase);
			return mbobjectBase as T;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002658 File Offset: 0x00000858
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

		// Token: 0x06000045 RID: 69 RVA: 0x000026EC File Offset: 0x000008EC
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

		// Token: 0x06000046 RID: 70 RVA: 0x000027A0 File Offset: 0x000009A0
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

		// Token: 0x06000047 RID: 71 RVA: 0x0000283C File Offset: 0x00000A3C
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

		// Token: 0x06000048 RID: 72 RVA: 0x00002898 File Offset: 0x00000A98
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

		// Token: 0x06000049 RID: 73 RVA: 0x000029AC File Offset: 0x00000BAC
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

		// Token: 0x0600004A RID: 74 RVA: 0x00002AA4 File Offset: 0x00000CA4
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

		// Token: 0x0600004B RID: 75 RVA: 0x00002B9C File Offset: 0x00000D9C
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

		// Token: 0x0600004C RID: 76 RVA: 0x00002C74 File Offset: 0x00000E74
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

		// Token: 0x0600004D RID: 77 RVA: 0x00002D08 File Offset: 0x00000F08
		public void PreAfterLoad()
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				objectTypeRecord.PreAfterLoad();
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002D58 File Offset: 0x00000F58
		public void AfterLoad()
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				objectTypeRecord.AfterLoad();
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002DA8 File Offset: 0x00000FA8
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

		// Token: 0x06000050 RID: 80 RVA: 0x00002E3C File Offset: 0x0000103C
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

		// Token: 0x06000051 RID: 81 RVA: 0x00002EC4 File Offset: 0x000010C4
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

		// Token: 0x06000052 RID: 82 RVA: 0x00002F74 File Offset: 0x00001174
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

		// Token: 0x06000053 RID: 83 RVA: 0x000030D0 File Offset: 0x000012D0
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

		// Token: 0x06000054 RID: 84 RVA: 0x00003184 File Offset: 0x00001384
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

		// Token: 0x06000055 RID: 85 RVA: 0x000031D8 File Offset: 0x000013D8
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

		// Token: 0x06000056 RID: 86 RVA: 0x000033D8 File Offset: 0x000015D8
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

		// Token: 0x06000057 RID: 87 RVA: 0x000034D8 File Offset: 0x000016D8
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

		// Token: 0x06000058 RID: 88 RVA: 0x00003524 File Offset: 0x00001724
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

		// Token: 0x06000059 RID: 89 RVA: 0x000035C4 File Offset: 0x000017C4
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

		// Token: 0x0600005A RID: 90 RVA: 0x00003630 File Offset: 0x00001830
		public static XmlDocument MergeTwoXmls(XmlDocument xmlDocument1, XmlDocument xmlDocument2)
		{
			XDocument xdocument = MBObjectManager.ToXDocument(xmlDocument1);
			XDocument xdocument2 = MBObjectManager.ToXDocument(xmlDocument2);
			xdocument.Root.Add(xdocument2.Root.Elements());
			return MBObjectManager.ToXmlDocument(xdocument);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003668 File Offset: 0x00001868
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

		// Token: 0x0600005C RID: 92 RVA: 0x000036A8 File Offset: 0x000018A8
		public static XmlDocument ToXmlDocument(XDocument xDocument)
		{
			XmlDocument xmlDocument = new XmlDocument();
			using (xDocument.CreateReader())
			{
				xmlDocument.Load(xDocument.CreateReader());
			}
			return xmlDocument;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000036EC File Offset: 0x000018EC
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

		// Token: 0x0600005E RID: 94 RVA: 0x0000373C File Offset: 0x0000193C
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

		// Token: 0x0600005F RID: 95 RVA: 0x0000377C File Offset: 0x0000197C
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

		// Token: 0x06000060 RID: 96 RVA: 0x00003948 File Offset: 0x00001B48
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

		// Token: 0x06000061 RID: 97 RVA: 0x00003A10 File Offset: 0x00001C10
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

		// Token: 0x06000062 RID: 98 RVA: 0x00003A68 File Offset: 0x00001C68
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

		// Token: 0x06000063 RID: 99 RVA: 0x00003B68 File Offset: 0x00001D68
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

		// Token: 0x06000064 RID: 100 RVA: 0x00003C04 File Offset: 0x00001E04
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

		// Token: 0x06000065 RID: 101 RVA: 0x00003CA0 File Offset: 0x00001EA0
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

		// Token: 0x06000066 RID: 102 RVA: 0x00003D84 File Offset: 0x00001F84
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

		// Token: 0x06000067 RID: 103 RVA: 0x00003E60 File Offset: 0x00002060
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

		// Token: 0x06000068 RID: 104 RVA: 0x00003F40 File Offset: 0x00002140
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

		// Token: 0x06000069 RID: 105 RVA: 0x00003FE4 File Offset: 0x000021E4
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

		// Token: 0x0600006A RID: 106 RVA: 0x00004074 File Offset: 0x00002274
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

		// Token: 0x0600006B RID: 107 RVA: 0x000040F0 File Offset: 0x000022F0
		public T CreateObject<T>() where T : MBObjectBase, new()
		{
			return this.CreateObject<T>(typeof(T).Name.ToString() + "_1");
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004118 File Offset: 0x00002318
		public void DebugPrint(PrintOutputDelegate printOutput)
		{
			printOutput("-Printing MBObjectManager Debug-");
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
			{
				printOutput(objectTypeRecord.DebugBasicDump());
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000417C File Offset: 0x0000237C
		public void AddHandler(IObjectManagerHandler handler)
		{
			if (this._handlers == null)
			{
				this._handlers = new List<IObjectManagerHandler>();
			}
			this._handlers.Add(handler);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000419D File Offset: 0x0000239D
		public void RemoveHandler(IObjectManagerHandler handler)
		{
			this._handlers.Remove(handler);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000041AC File Offset: 0x000023AC
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

		// Token: 0x06000070 RID: 112 RVA: 0x00004248 File Offset: 0x00002448
		public void ReInitialize()
		{
			foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords.ToList<MBObjectManager.IObjectTypeRecord>())
			{
				objectTypeRecord.ReInitialize();
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000042A0 File Offset: 0x000024A0
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

		// Token: 0x0400000A RID: 10
		internal List<MBObjectManager.IObjectTypeRecord> ObjectTypeRecords = new List<MBObjectManager.IObjectTypeRecord>();

		// Token: 0x0400000B RID: 11
		private List<IObjectManagerHandler> _handlers;

		// Token: 0x02000014 RID: 20
		internal interface IObjectTypeRecord : IEnumerable
		{
			// Token: 0x1700000B RID: 11
			// (get) Token: 0x06000091 RID: 145
			bool AutoCreate { get; }

			// Token: 0x1700000C RID: 12
			// (get) Token: 0x06000092 RID: 146
			string ElementName { get; }

			// Token: 0x1700000D RID: 13
			// (get) Token: 0x06000093 RID: 147
			string ElementListName { get; }

			// Token: 0x1700000E RID: 14
			// (get) Token: 0x06000094 RID: 148
			Type ObjectClass { get; }

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x06000095 RID: 149
			uint TypeNo { get; }

			// Token: 0x17000010 RID: 16
			// (get) Token: 0x06000096 RID: 150
			bool IsTemporary { get; }

			// Token: 0x06000097 RID: 151
			void ReInitialize();

			// Token: 0x06000098 RID: 152
			MBObjectBase CreatePresumedMBObject(string objectName);

			// Token: 0x06000099 RID: 153
			void RegisterMBObject(MBObjectBase obj, bool presumed, out MBObjectBase registeredObject);

			// Token: 0x0600009A RID: 154
			void RegisterMBObjectWithoutInitialization(MBObjectBase obj);

			// Token: 0x0600009B RID: 155
			void UnregisterMBObject(MBObjectBase obj);

			// Token: 0x0600009C RID: 156
			MBObjectBase GetFirstMBObject();

			// Token: 0x0600009D RID: 157
			MBObjectBase GetMBObject(string objId);

			// Token: 0x0600009E RID: 158
			MBObjectBase GetMBObject(MBGUID objId);

			// Token: 0x0600009F RID: 159
			bool ContainsObject(string objId);

			// Token: 0x060000A0 RID: 160
			string DebugDump();

			// Token: 0x060000A1 RID: 161
			string DebugBasicDump();

			// Token: 0x060000A2 RID: 162
			IEnumerable GetList();

			// Token: 0x060000A3 RID: 163
			void PreAfterLoad();

			// Token: 0x060000A4 RID: 164
			void AfterLoad();
		}

		// Token: 0x02000015 RID: 21
		internal class ObjectTypeRecord<T> : MBObjectManager.IObjectTypeRecord, IEnumerable, IEnumerable<T> where T : MBObjectBase
		{
			// Token: 0x17000011 RID: 17
			// (get) Token: 0x060000A5 RID: 165 RVA: 0x000047BA File Offset: 0x000029BA
			bool MBObjectManager.IObjectTypeRecord.AutoCreate
			{
				get
				{
					return this._autoCreate;
				}
			}

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x060000A6 RID: 166 RVA: 0x000047C2 File Offset: 0x000029C2
			string MBObjectManager.IObjectTypeRecord.ElementName
			{
				get
				{
					return this._elementName;
				}
			}

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x060000A7 RID: 167 RVA: 0x000047CA File Offset: 0x000029CA
			string MBObjectManager.IObjectTypeRecord.ElementListName
			{
				get
				{
					return this._elementListName;
				}
			}

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x060000A8 RID: 168 RVA: 0x000047D2 File Offset: 0x000029D2
			Type MBObjectManager.IObjectTypeRecord.ObjectClass
			{
				get
				{
					return typeof(T);
				}
			}

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x060000A9 RID: 169 RVA: 0x000047DE File Offset: 0x000029DE
			uint MBObjectManager.IObjectTypeRecord.TypeNo
			{
				get
				{
					return this._typeNo;
				}
			}

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x060000AA RID: 170 RVA: 0x000047E6 File Offset: 0x000029E6
			bool MBObjectManager.IObjectTypeRecord.IsTemporary
			{
				get
				{
					return this._isTemporary;
				}
			}

			// Token: 0x17000017 RID: 23
			// (get) Token: 0x060000AB RID: 171 RVA: 0x000047EE File Offset: 0x000029EE
			internal MBList<T> RegisteredObjectsList { get; }

			// Token: 0x060000AC RID: 172 RVA: 0x000047F8 File Offset: 0x000029F8
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

			// Token: 0x060000AD RID: 173 RVA: 0x00004858 File Offset: 0x00002A58
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

			// Token: 0x060000AE RID: 174 RVA: 0x000048C8 File Offset: 0x00002AC8
			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return this.EnumerateElements();
			}

			// Token: 0x060000AF RID: 175 RVA: 0x000048D0 File Offset: 0x00002AD0
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.EnumerateElements();
			}

			// Token: 0x060000B0 RID: 176 RVA: 0x000048D8 File Offset: 0x00002AD8
			internal MBGUID GetNewId()
			{
				uint typeNo = this._typeNo;
				uint num = this._objCount + 1U;
				this._objCount = num;
				return new MBGUID(typeNo, num);
			}

			// Token: 0x060000B1 RID: 177 RVA: 0x00004901 File Offset: 0x00002B01
			MBObjectBase MBObjectManager.IObjectTypeRecord.CreatePresumedMBObject(string objectName)
			{
				return this.CreatePresumedObject(objectName);
			}

			// Token: 0x060000B2 RID: 178 RVA: 0x0000490F File Offset: 0x00002B0F
			private T CreatePresumedObject(string objectName)
			{
				T t = Activator.CreateInstance<T>();
				t.StringId = objectName;
				t.IsReady = false;
				t.IsInitialized = false;
				return t;
			}

			// Token: 0x060000B3 RID: 179 RVA: 0x0000493A File Offset: 0x00002B3A
			MBObjectBase MBObjectManager.IObjectTypeRecord.GetMBObject(string objId)
			{
				return this.GetObject(objId);
			}

			// Token: 0x060000B4 RID: 180 RVA: 0x00004948 File Offset: 0x00002B48
			MBObjectBase MBObjectManager.IObjectTypeRecord.GetFirstMBObject()
			{
				return this.GetFirstObject();
			}

			// Token: 0x060000B5 RID: 181 RVA: 0x00004958 File Offset: 0x00002B58
			internal T GetFirstObject()
			{
				if (this.RegisteredObjectsList.Count <= 0)
				{
					return default(T);
				}
				return this.RegisteredObjectsList[0];
			}

			// Token: 0x060000B6 RID: 182 RVA: 0x0000498C File Offset: 0x00002B8C
			internal T GetObject(string objId)
			{
				T t;
				this._registeredObjects.TryGetValue(objId, out t);
				return t;
			}

			// Token: 0x060000B7 RID: 183 RVA: 0x000049A9 File Offset: 0x00002BA9
			bool MBObjectManager.IObjectTypeRecord.ContainsObject(string objId)
			{
				return this._registeredObjects.ContainsKey(objId);
			}

			// Token: 0x060000B8 RID: 184 RVA: 0x000049B8 File Offset: 0x00002BB8
			public MBObjectBase GetMBObject(MBGUID objId)
			{
				T t = default(T);
				this._registeredObjectsWithGuid.TryGetValue(objId, out t);
				return t;
			}

			// Token: 0x060000B9 RID: 185 RVA: 0x000049E4 File Offset: 0x00002BE4
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

			// Token: 0x060000BA RID: 186 RVA: 0x00004A71 File Offset: 0x00002C71
			void MBObjectManager.IObjectTypeRecord.RegisterMBObject(MBObjectBase obj, bool presumed, out MBObjectBase registeredObject)
			{
				if (obj is T)
				{
					this.RegisterObject(obj as T, presumed, out registeredObject);
					return;
				}
				registeredObject = null;
			}

			// Token: 0x060000BB RID: 187 RVA: 0x00004A94 File Offset: 0x00002C94
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

			// Token: 0x060000BC RID: 188 RVA: 0x00004BD4 File Offset: 0x00002DD4
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

			// Token: 0x060000BD RID: 189 RVA: 0x00004C3B File Offset: 0x00002E3B
			void MBObjectManager.IObjectTypeRecord.UnregisterMBObject(MBObjectBase obj)
			{
				if (obj is T)
				{
					this.UnregisterObject((T)((object)obj));
					return;
				}
				throw new MBIllegalRegisterException();
			}

			// Token: 0x060000BE RID: 190 RVA: 0x00004C58 File Offset: 0x00002E58
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

			// Token: 0x060000BF RID: 191 RVA: 0x00004D21 File Offset: 0x00002F21
			internal MBReadOnlyList<T> GetObjectsList()
			{
				return this.RegisteredObjectsList;
			}

			// Token: 0x060000C0 RID: 192 RVA: 0x00004D29 File Offset: 0x00002F29
			IEnumerable MBObjectManager.IObjectTypeRecord.GetList()
			{
				return this.RegisteredObjectsList;
			}

			// Token: 0x060000C1 RID: 193 RVA: 0x00004D34 File Offset: 0x00002F34
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

			// Token: 0x060000C2 RID: 194 RVA: 0x00004E30 File Offset: 0x00003030
			string MBObjectManager.IObjectTypeRecord.DebugBasicDump()
			{
				return this._elementName + " " + this._objCount;
			}

			// Token: 0x060000C3 RID: 195 RVA: 0x00004E4D File Offset: 0x0000304D
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

			// Token: 0x060000C4 RID: 196 RVA: 0x00004E5C File Offset: 0x0000305C
			void MBObjectManager.IObjectTypeRecord.PreAfterLoad()
			{
				for (int i = this.RegisteredObjectsList.Count - 1; i >= 0; i--)
				{
					this.RegisteredObjectsList[i].PreAfterLoadInternal();
				}
			}

			// Token: 0x060000C5 RID: 197 RVA: 0x00004E98 File Offset: 0x00003098
			void MBObjectManager.IObjectTypeRecord.AfterLoad()
			{
				for (int i = this.RegisteredObjectsList.Count - 1; i >= 0; i--)
				{
					this.RegisteredObjectsList[i].AfterLoadInternal();
				}
			}

			// Token: 0x04000012 RID: 18
			private readonly bool _autoCreate;

			// Token: 0x04000013 RID: 19
			private readonly string _elementName;

			// Token: 0x04000014 RID: 20
			private readonly string _elementListName;

			// Token: 0x04000015 RID: 21
			private uint _objCount;

			// Token: 0x04000016 RID: 22
			private readonly uint _typeNo;

			// Token: 0x04000017 RID: 23
			private readonly bool _isTemporary;

			// Token: 0x04000018 RID: 24
			private readonly Dictionary<string, T> _registeredObjects;

			// Token: 0x04000019 RID: 25
			private readonly Dictionary<MBGUID, T> _registeredObjectsWithGuid;
		}
	}
}
