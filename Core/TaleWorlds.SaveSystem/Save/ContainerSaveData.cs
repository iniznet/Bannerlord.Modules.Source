using System;
using System.Collections;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	internal class ContainerSaveData
	{
		public int ObjectId { get; private set; }

		public ISaveContext Context { get; private set; }

		public object Target { get; private set; }

		public Type Type { get; private set; }

		internal int ElementPropertyCount
		{
			get
			{
				if (this._childStructs.Count <= 0)
				{
					return 0;
				}
				return this._childStructs[0].PropertyCount;
			}
		}

		internal int ElementFieldCount
		{
			get
			{
				if (this._childStructs.Count <= 0)
				{
					return 0;
				}
				return this._childStructs[0].FieldCount;
			}
		}

		public ContainerSaveData(ISaveContext context, int objectId, object target, ContainerType containerType)
		{
			this.ObjectId = objectId;
			this.Context = context;
			this.Target = target;
			this._containerType = containerType;
			this.Type = target.GetType();
			this._elementCount = this.GetElementCount();
			this._childStructs = new List<ObjectSaveData>();
			this._typeDefinition = context.DefinitionContext.GetContainerDefinition(this.Type);
			if (this._typeDefinition == null)
			{
				throw new Exception("Could not find type definition of container type: " + this.Type);
			}
		}

		public void CollectChildren()
		{
			this._keys = new ElementSaveData[this._elementCount];
			this._values = new ElementSaveData[this._elementCount];
			if (this._containerType == ContainerType.Dictionary)
			{
				IDictionary dictionary = (IDictionary)this.Target;
				int num = 0;
				using (IDictionaryEnumerator enumerator = dictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						object key = dictionaryEntry.Key;
						object value = dictionaryEntry.Value;
						ElementSaveData elementSaveData = new ElementSaveData(this, key, num);
						ElementSaveData elementSaveData2 = new ElementSaveData(this, value, this._elementCount + num);
						this._keys[num] = elementSaveData;
						this._values[num] = elementSaveData2;
						num++;
					}
					return;
				}
			}
			if (this._containerType == ContainerType.List || this._containerType == ContainerType.CustomList || this._containerType == ContainerType.CustomReadOnlyList)
			{
				IList list = (IList)this.Target;
				for (int i = 0; i < this._elementCount; i++)
				{
					object obj2 = list[i];
					ElementSaveData elementSaveData3 = new ElementSaveData(this, obj2, i);
					this._values[i] = elementSaveData3;
				}
				return;
			}
			if (this._containerType == ContainerType.Queue)
			{
				IEnumerable enumerable = (ICollection)this.Target;
				int num2 = 0;
				using (IEnumerator enumerator2 = enumerable.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						object obj3 = enumerator2.Current;
						ElementSaveData elementSaveData4 = new ElementSaveData(this, obj3, num2);
						this._values[num2] = elementSaveData4;
						num2++;
					}
					return;
				}
			}
			if (this._containerType == ContainerType.Array)
			{
				Array array = (Array)this.Target;
				for (int j = 0; j < this._elementCount; j++)
				{
					object value2 = array.GetValue(j);
					ElementSaveData elementSaveData5 = new ElementSaveData(this, value2, j);
					this._values[j] = elementSaveData5;
				}
			}
		}

		public void SaveHeaderTo(SaveEntryFolder parentFolder, IArchiveContext archiveContext)
		{
			SaveEntryFolder saveEntryFolder = archiveContext.CreateFolder(parentFolder, new FolderId(this.ObjectId, SaveFolderExtension.Container), 1);
			BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
			this._typeDefinition.SaveId.WriteTo(binaryWriter);
			binaryWriter.WriteByte((byte)this._containerType);
			binaryWriter.WriteInt(this.GetElementCount());
			saveEntryFolder.CreateEntry(new EntryId(-1, SaveEntryExtension.Object)).FillFrom(binaryWriter);
			BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
		}

		public void SaveTo(SaveEntryFolder parentFolder, IArchiveContext archiveContext)
		{
			int num = ((this._containerType == ContainerType.Dictionary) ? (this._elementCount * 2) : this._elementCount);
			SaveEntryFolder saveEntryFolder = archiveContext.CreateFolder(parentFolder, new FolderId(this.ObjectId, SaveFolderExtension.Container), num);
			for (int i = 0; i < this._elementCount; i++)
			{
				VariableSaveData variableSaveData = this._values[i];
				BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
				variableSaveData.SaveTo(binaryWriter);
				saveEntryFolder.CreateEntry(new EntryId(i, SaveEntryExtension.Value)).FillFrom(binaryWriter);
				BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
				if (this._containerType == ContainerType.Dictionary)
				{
					VariableSaveData variableSaveData2 = this._keys[i];
					BinaryWriter binaryWriter2 = BinaryWriterFactory.GetBinaryWriter();
					variableSaveData2.SaveTo(binaryWriter2);
					saveEntryFolder.CreateEntry(new EntryId(i, SaveEntryExtension.Key)).FillFrom(binaryWriter2);
					BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter2);
				}
			}
			foreach (ObjectSaveData objectSaveData in this._childStructs)
			{
				TypeDefinition structDefinition = this.Context.DefinitionContext.GetStructDefinition(objectSaveData.Type);
				ISavedStruct savedStruct;
				if (structDefinition == null || !(structDefinition is StructDefinition) || (savedStruct = objectSaveData.Target as ISavedStruct) == null || !savedStruct.IsDefault())
				{
					objectSaveData.SaveTo(saveEntryFolder, archiveContext);
				}
			}
		}

		internal int GetElementCount()
		{
			if (this._containerType == ContainerType.List || this._containerType == ContainerType.CustomList || this._containerType == ContainerType.CustomReadOnlyList)
			{
				return ((IList)this.Target).Count;
			}
			if (this._containerType == ContainerType.Queue)
			{
				return ((ICollection)this.Target).Count;
			}
			if (this._containerType == ContainerType.Dictionary)
			{
				return ((IDictionary)this.Target).Count;
			}
			if (this._containerType == ContainerType.Array)
			{
				return ((Array)this.Target).GetLength(0);
			}
			return 0;
		}

		public void CollectStrings()
		{
			foreach (object obj in this.GetChildString())
			{
				string text = (string)obj;
				this.Context.AddOrGetStringId(text);
			}
			foreach (ObjectSaveData objectSaveData in this._childStructs)
			{
				objectSaveData.CollectStrings();
			}
		}

		public void CollectStringsInto(List<string> collection)
		{
			foreach (object obj in this.GetChildString())
			{
				string text = (string)obj;
				collection.Add(text);
			}
		}

		public void CollectStructs()
		{
			foreach (ElementSaveData elementSaveData in this.GetChildElementSaveDatas())
			{
				if (elementSaveData.MemberType == SavedMemberType.CustomStruct)
				{
					object elementValue = elementSaveData.ElementValue;
					ObjectSaveData objectSaveData = new ObjectSaveData(this.Context, elementSaveData.ElementIndex, elementValue, false);
					this._childStructs.Add(objectSaveData);
				}
			}
			foreach (ObjectSaveData objectSaveData2 in this._childStructs)
			{
				objectSaveData2.CollectStructs();
			}
		}

		public void CollectMembers()
		{
			foreach (ObjectSaveData objectSaveData in this._childStructs)
			{
				objectSaveData.CollectMembers();
			}
		}

		public IEnumerable<ElementSaveData> GetChildElementSaveDatas()
		{
			int num;
			for (int i = 0; i < this._elementCount; i = num + 1)
			{
				ElementSaveData elementSaveData = this._keys[i];
				ElementSaveData value = this._values[i];
				if (elementSaveData != null)
				{
					yield return elementSaveData;
				}
				if (value != null)
				{
					yield return value;
				}
				value = null;
				num = i;
			}
			yield break;
		}

		public IEnumerable<object> GetChildElements()
		{
			return ContainerSaveData.GetChildElements(this._containerType, this.Target);
		}

		public static IEnumerable<object> GetChildElements(ContainerType containerType, object target)
		{
			if (containerType == ContainerType.List || containerType == ContainerType.CustomList || containerType == ContainerType.CustomReadOnlyList)
			{
				IList list = (IList)target;
				int num;
				for (int i = 0; i < list.Count; i = num + 1)
				{
					object obj = list[i];
					if (obj != null)
					{
						yield return obj;
					}
					num = i;
				}
				list = null;
			}
			else if (containerType == ContainerType.Queue)
			{
				ICollection collection = (ICollection)target;
				foreach (object obj2 in collection)
				{
					if (obj2 != null)
					{
						yield return obj2;
					}
				}
				IEnumerator enumerator = null;
			}
			else if (containerType == ContainerType.Dictionary)
			{
				IDictionary dictionary = (IDictionary)target;
				foreach (object obj3 in dictionary)
				{
					DictionaryEntry entry = (DictionaryEntry)obj3;
					yield return entry.Key;
					object value = entry.Value;
					if (value != null)
					{
						yield return value;
					}
					entry = default(DictionaryEntry);
				}
				IDictionaryEnumerator dictionaryEnumerator = null;
			}
			else if (containerType == ContainerType.Array)
			{
				Array array = (Array)target;
				int num;
				for (int i = 0; i < array.Length; i = num + 1)
				{
					object value2 = array.GetValue(i);
					if (value2 != null)
					{
						yield return value2;
					}
					num = i;
				}
				array = null;
			}
			yield break;
			yield break;
		}

		public IEnumerable<object> GetChildObjects(ISaveContext context)
		{
			List<object> list = new List<object>();
			ContainerSaveData.GetChildObjects(context, this._typeDefinition, this._containerType, this.Target, list);
			return list;
		}

		public static void GetChildObjects(ISaveContext context, ContainerDefinition containerDefinition, ContainerType containerType, object target, List<object> collectedObjects)
		{
			if (containerDefinition.CollectObjectsMethod != null)
			{
				if (!containerDefinition.HasNoChildObject)
				{
					containerDefinition.CollectObjectsMethod(target, collectedObjects);
					return;
				}
			}
			else
			{
				if (containerType == ContainerType.List || containerType == ContainerType.CustomList || containerType == ContainerType.CustomReadOnlyList)
				{
					IList list = (IList)target;
					for (int i = 0; i < list.Count; i++)
					{
						object obj = list[i];
						if (obj != null)
						{
							ContainerSaveData.ProcessChildObjectElement(obj, context, collectedObjects);
						}
					}
					return;
				}
				if (containerType == ContainerType.Queue)
				{
					using (IEnumerator enumerator = ((ICollection)target).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj2 = enumerator.Current;
							if (obj2 != null)
							{
								ContainerSaveData.ProcessChildObjectElement(obj2, context, collectedObjects);
							}
						}
						return;
					}
				}
				if (containerType == ContainerType.Dictionary)
				{
					using (IDictionaryEnumerator enumerator2 = ((IDictionary)target).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj3 = enumerator2.Current;
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj3;
							ContainerSaveData.ProcessChildObjectElement(dictionaryEntry.Key, context, collectedObjects);
							object value = dictionaryEntry.Value;
							if (value != null)
							{
								ContainerSaveData.ProcessChildObjectElement(value, context, collectedObjects);
							}
						}
						return;
					}
				}
				if (containerType == ContainerType.Array)
				{
					Array array = (Array)target;
					for (int j = 0; j < array.Length; j++)
					{
						object value2 = array.GetValue(j);
						if (value2 != null)
						{
							ContainerSaveData.ProcessChildObjectElement(value2, context, collectedObjects);
						}
					}
					return;
				}
				foreach (object obj4 in ContainerSaveData.GetChildElements(containerType, target))
				{
					ContainerSaveData.ProcessChildObjectElement(obj4, context, collectedObjects);
				}
			}
		}

		private static void ProcessChildObjectElement(object childElement, ISaveContext context, List<object> collectedObjects)
		{
			Type type = childElement.GetType();
			bool isClass = type.IsClass;
			if (isClass && type != typeof(string))
			{
				collectedObjects.Add(childElement);
				return;
			}
			if (!isClass)
			{
				TypeDefinition structDefinition = context.DefinitionContext.GetStructDefinition(type);
				if (structDefinition != null)
				{
					if (structDefinition.CollectObjectsMethod != null)
					{
						structDefinition.CollectObjectsMethod(childElement, collectedObjects);
						return;
					}
					for (int i = 0; i < structDefinition.MemberDefinitions.Count; i++)
					{
						MemberDefinition memberDefinition = structDefinition.MemberDefinitions[i];
						ObjectSaveData.GetChildObjectFrom(context, childElement, memberDefinition, collectedObjects);
					}
				}
			}
		}

		private IEnumerable<object> GetChildString()
		{
			foreach (object obj in this.GetChildElements())
			{
				if (obj.GetType() == typeof(string))
				{
					yield return obj;
				}
			}
			IEnumerator<object> enumerator = null;
			yield break;
			yield break;
		}

		private ContainerType _containerType;

		private ElementSaveData[] _keys;

		private ElementSaveData[] _values;

		private ContainerDefinition _typeDefinition;

		private int _elementCount;

		private List<ObjectSaveData> _childStructs;
	}
}
