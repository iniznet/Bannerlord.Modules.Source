using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x02000033 RID: 51
	internal class ContainerLoadData
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x00008781 File Offset: 0x00006981
		public int Id
		{
			get
			{
				return this.ContainerHeaderLoadData.Id;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x0000878E File Offset: 0x0000698E
		public object Target
		{
			get
			{
				return this.ContainerHeaderLoadData.Target;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000879B File Offset: 0x0000699B
		public LoadContext Context
		{
			get
			{
				return this.ContainerHeaderLoadData.Context;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x000087A8 File Offset: 0x000069A8
		public ContainerDefinition TypeDefinition
		{
			get
			{
				return this.ContainerHeaderLoadData.TypeDefinition;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001DA RID: 474 RVA: 0x000087B5 File Offset: 0x000069B5
		// (set) Token: 0x060001DB RID: 475 RVA: 0x000087BD File Offset: 0x000069BD
		public ContainerHeaderLoadData ContainerHeaderLoadData { get; private set; }

		// Token: 0x060001DC RID: 476 RVA: 0x000087C8 File Offset: 0x000069C8
		public ContainerLoadData(ContainerHeaderLoadData headerLoadData)
		{
			this.ContainerHeaderLoadData = headerLoadData;
			this._childStructs = new Dictionary<int, ObjectLoadData>();
			this._saveId = headerLoadData.SaveId;
			this._containerType = headerLoadData.ContainerType;
			this._elementCount = headerLoadData.ElementCount;
			this._keys = new ElementLoadData[this._elementCount];
			this._values = new ElementLoadData[this._elementCount];
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00008834 File Offset: 0x00006A34
		private FolderId[] GetChildStructNames(SaveEntryFolder saveEntryFolder)
		{
			List<FolderId> list = new List<FolderId>();
			foreach (SaveEntryFolder saveEntryFolder2 in saveEntryFolder.ChildFolders)
			{
				if (saveEntryFolder2.FolderId.Extension == SaveFolderExtension.Struct && !list.Contains(saveEntryFolder2.FolderId))
				{
					list.Add(saveEntryFolder2.FolderId);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060001DE RID: 478 RVA: 0x000088B8 File Offset: 0x00006AB8
		public void InitializeReaders(SaveEntryFolder saveEntryFolder)
		{
			foreach (FolderId folderId in this.GetChildStructNames(saveEntryFolder))
			{
				int localId = folderId.LocalId;
				ObjectLoadData objectLoadData = new ObjectLoadData(this.Context, localId);
				this._childStructs.Add(localId, objectLoadData);
			}
			for (int j = 0; j < this._elementCount; j++)
			{
				BinaryReader binaryReader = saveEntryFolder.GetEntry(new EntryId(j, SaveEntryExtension.Value)).GetBinaryReader();
				ElementLoadData elementLoadData = new ElementLoadData(this, binaryReader);
				this._values[j] = elementLoadData;
				if (this._containerType == ContainerType.Dictionary)
				{
					BinaryReader binaryReader2 = saveEntryFolder.GetEntry(new EntryId(j, SaveEntryExtension.Key)).GetBinaryReader();
					ElementLoadData elementLoadData2 = new ElementLoadData(this, binaryReader2);
					this._keys[j] = elementLoadData2;
				}
			}
			foreach (KeyValuePair<int, ObjectLoadData> keyValuePair in this._childStructs)
			{
				int key = keyValuePair.Key;
				ObjectLoadData value = keyValuePair.Value;
				SaveEntryFolder childFolder = saveEntryFolder.GetChildFolder(new FolderId(key, SaveFolderExtension.Struct));
				value.InitializeReaders(childFolder);
			}
		}

		// Token: 0x060001DF RID: 479 RVA: 0x000089E4 File Offset: 0x00006BE4
		public void FillCreatedObject()
		{
			foreach (ObjectLoadData objectLoadData in this._childStructs.Values)
			{
				objectLoadData.CreateStruct();
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00008A3C File Offset: 0x00006C3C
		public void Read()
		{
			for (int i = 0; i < this._elementCount; i++)
			{
				this._values[i].Read();
				if (this._containerType == ContainerType.Dictionary)
				{
					this._keys[i].Read();
				}
			}
			foreach (ObjectLoadData objectLoadData in this._childStructs.Values)
			{
				objectLoadData.Read();
			}
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00008AC8 File Offset: 0x00006CC8
		private static Assembly GetAssemblyByName(string name)
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault((Assembly assembly) => assembly.GetName().FullName == name);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00008B00 File Offset: 0x00006D00
		public void FillObject()
		{
			foreach (ObjectLoadData objectLoadData in this._childStructs.Values)
			{
				objectLoadData.FillObject();
			}
			for (int i = 0; i < this._elementCount; i++)
			{
				if (this._containerType == ContainerType.List || this._containerType == ContainerType.CustomList || this._containerType == ContainerType.CustomReadOnlyList)
				{
					IList list = (IList)this.Target;
					ElementLoadData elementLoadData = this._values[i];
					if (elementLoadData.SavedMemberType == SavedMemberType.CustomStruct)
					{
						int num = (int)elementLoadData.Data;
						ObjectLoadData objectLoadData2 = this._childStructs[num];
						elementLoadData.SetCustomStructData(objectLoadData2.Target);
					}
					object dataToUse = elementLoadData.GetDataToUse();
					if (list != null)
					{
						list.Add(dataToUse);
					}
				}
				else if (this._containerType == ContainerType.Dictionary)
				{
					IDictionary dictionary = (IDictionary)this.Target;
					ElementLoadData elementLoadData2 = this._keys[i];
					ElementLoadData elementLoadData3 = this._values[i];
					if (elementLoadData2.SavedMemberType == SavedMemberType.CustomStruct)
					{
						int num2 = (int)elementLoadData2.Data;
						ObjectLoadData objectLoadData3 = this._childStructs[num2];
						elementLoadData2.SetCustomStructData(objectLoadData3.Target);
					}
					if (elementLoadData3.SavedMemberType == SavedMemberType.CustomStruct)
					{
						int num3 = (int)elementLoadData3.Data;
						ObjectLoadData objectLoadData4 = this._childStructs[num3];
						elementLoadData3.SetCustomStructData(objectLoadData4.Target);
					}
					object dataToUse2 = elementLoadData2.GetDataToUse();
					object dataToUse3 = elementLoadData3.GetDataToUse();
					if (dictionary != null && dataToUse2 != null)
					{
						dictionary.Add(dataToUse2, dataToUse3);
					}
				}
				else if (this._containerType == ContainerType.Array)
				{
					Array array = (Array)this.Target;
					ElementLoadData elementLoadData4 = this._values[i];
					if (elementLoadData4.SavedMemberType == SavedMemberType.CustomStruct)
					{
						int num4 = (int)elementLoadData4.Data;
						ObjectLoadData objectLoadData5 = this._childStructs[num4];
						elementLoadData4.SetCustomStructData(objectLoadData5.Target);
					}
					object dataToUse4 = elementLoadData4.GetDataToUse();
					array.SetValue(dataToUse4, i);
				}
				else if (this._containerType == ContainerType.Queue)
				{
					ICollection collection = (ICollection)this.Target;
					ElementLoadData elementLoadData5 = this._values[i];
					if (elementLoadData5.SavedMemberType == SavedMemberType.CustomStruct)
					{
						int num5 = (int)elementLoadData5.Data;
						ObjectLoadData objectLoadData6 = this._childStructs[num5];
						elementLoadData5.SetCustomStructData(objectLoadData6.Target);
					}
					object dataToUse5 = elementLoadData5.GetDataToUse();
					collection.GetType().GetMethod("Enqueue").Invoke(collection, new object[] { dataToUse5 });
				}
			}
		}

		// Token: 0x0400008C RID: 140
		private SaveId _saveId;

		// Token: 0x0400008D RID: 141
		private int _elementCount;

		// Token: 0x0400008E RID: 142
		private ContainerType _containerType;

		// Token: 0x0400008F RID: 143
		private ElementLoadData[] _keys;

		// Token: 0x04000090 RID: 144
		private ElementLoadData[] _values;

		// Token: 0x04000091 RID: 145
		private Dictionary<int, ObjectLoadData> _childStructs;
	}
}
