using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	internal class ContainerLoadData
	{
		public int Id
		{
			get
			{
				return this.ContainerHeaderLoadData.Id;
			}
		}

		public object Target
		{
			get
			{
				return this.ContainerHeaderLoadData.Target;
			}
		}

		public LoadContext Context
		{
			get
			{
				return this.ContainerHeaderLoadData.Context;
			}
		}

		public ContainerDefinition TypeDefinition
		{
			get
			{
				return this.ContainerHeaderLoadData.TypeDefinition;
			}
		}

		public ContainerHeaderLoadData ContainerHeaderLoadData { get; private set; }

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

		public void FillCreatedObject()
		{
			foreach (ObjectLoadData objectLoadData in this._childStructs.Values)
			{
				objectLoadData.CreateStruct();
			}
		}

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

		private static Assembly GetAssemblyByName(string name)
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault((Assembly assembly) => assembly.GetName().FullName == name);
		}

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
						ObjectLoadData objectLoadData2;
						object obj;
						if (this._childStructs.TryGetValue(num, out objectLoadData2))
						{
							obj = objectLoadData2.Target;
						}
						else
						{
							obj = ContainerLoadData.GetDefaultObject(this._saveId, this.Context, false);
						}
						elementLoadData.SetCustomStructData(obj);
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
						ObjectLoadData objectLoadData3;
						object obj2;
						if (this._childStructs.TryGetValue(num2, out objectLoadData3))
						{
							obj2 = objectLoadData3.Target;
						}
						else
						{
							obj2 = ContainerLoadData.GetDefaultObject(this._saveId, this.Context, false);
						}
						elementLoadData2.SetCustomStructData(obj2);
					}
					if (elementLoadData3.SavedMemberType == SavedMemberType.CustomStruct)
					{
						int num3 = (int)elementLoadData3.Data;
						ObjectLoadData objectLoadData4;
						object obj3;
						if (this._childStructs.TryGetValue(num3, out objectLoadData4))
						{
							obj3 = objectLoadData4.Target;
						}
						else
						{
							obj3 = ContainerLoadData.GetDefaultObject(this._saveId, this.Context, true);
						}
						elementLoadData3.SetCustomStructData(obj3);
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
						ObjectLoadData objectLoadData5;
						object obj4;
						if (this._childStructs.TryGetValue(num4, out objectLoadData5))
						{
							obj4 = objectLoadData5.Target;
						}
						else
						{
							obj4 = ContainerLoadData.GetDefaultObject(this._saveId, this.Context, false);
						}
						elementLoadData4.SetCustomStructData(obj4);
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
						ObjectLoadData objectLoadData6;
						object obj5;
						if (this._childStructs.TryGetValue(num5, out objectLoadData6))
						{
							obj5 = objectLoadData6.Target;
						}
						else
						{
							obj5 = ContainerLoadData.GetDefaultObject(this._saveId, this.Context, false);
						}
						elementLoadData5.SetCustomStructData(obj5);
					}
					object dataToUse5 = elementLoadData5.GetDataToUse();
					collection.GetType().GetMethod("Enqueue").Invoke(collection, new object[] { dataToUse5 });
				}
			}
		}

		private static object GetDefaultObject(SaveId saveId, LoadContext context, bool getValueId = false)
		{
			ContainerSaveId containerSaveId = (ContainerSaveId)saveId;
			TypeDefinitionBase typeDefinitionBase;
			if (getValueId)
			{
				typeDefinitionBase = context.DefinitionContext.TryGetTypeDefinition(containerSaveId.ValueId);
			}
			else
			{
				typeDefinitionBase = context.DefinitionContext.TryGetTypeDefinition(containerSaveId.KeyId);
			}
			return Activator.CreateInstance(((StructDefinition)typeDefinitionBase).Type);
		}

		private SaveId _saveId;

		private int _elementCount;

		private ContainerType _containerType;

		private ElementLoadData[] _keys;

		private ElementLoadData[] _values;

		private Dictionary<int, ObjectLoadData> _childStructs;
	}
}
