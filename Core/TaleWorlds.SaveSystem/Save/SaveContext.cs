using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	public class SaveContext : ISaveContext
	{
		public object RootObject { get; private set; }

		public GameData SaveData { get; private set; }

		public DefinitionContext DefinitionContext { get; private set; }

		public static SaveContext.SaveStatistics GetStatistics()
		{
			return new SaveContext.SaveStatistics(SaveContext._typeStatistics, SaveContext._containerStatistics);
		}

		public static bool EnableSaveStatistics
		{
			get
			{
				return false;
			}
		}

		public SaveContext(DefinitionContext definitionContext)
		{
			this.DefinitionContext = definitionContext;
			this._childObjects = new List<object>(131072);
			this._idsOfChildObjects = new Dictionary<object, int>(131072);
			this._strings = new List<string>(131072);
			this._idsOfStrings = new Dictionary<string, int>(131072);
			this._childContainers = new List<object>(131072);
			this._idsOfChildContainers = new Dictionary<object, int>(131072);
			this._temporaryCollectedObjects = new List<object>(4096);
			this._locker = new object();
		}

		private void CollectObjects()
		{
			using (new PerformanceTestBlock("SaveContext::CollectObjects"))
			{
				this._objectsToIterate = new Queue<object>(1024);
				this._objectsToIterate.Enqueue(this.RootObject);
				while (this._objectsToIterate.Count > 0)
				{
					object obj = this._objectsToIterate.Dequeue();
					ContainerType containerType;
					if (obj.GetType().IsContainer(out containerType))
					{
						this.CollectContainerObjects(containerType, obj);
					}
					else
					{
						this.CollectObjects(obj);
					}
				}
			}
		}

		private void CollectContainerObjects(ContainerType containerType, object parent)
		{
			if (!this._idsOfChildContainers.ContainsKey(parent))
			{
				int count = this._childContainers.Count;
				this._childContainers.Add(parent);
				this._idsOfChildContainers.Add(parent, count);
				Type type = parent.GetType();
				ContainerDefinition containerDefinition = this.DefinitionContext.GetContainerDefinition(type);
				if (containerDefinition == null)
				{
					string text = "Cant find definition for " + type.FullName;
					Debug.Print(text, 0, Debug.DebugColor.Red, 17592186044416UL);
					Debug.FailedAssert(text, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\Save\\SaveContext.cs", "CollectContainerObjects", 154);
				}
				ContainerSaveData.GetChildObjects(this, containerDefinition, containerType, parent, this._temporaryCollectedObjects);
				for (int i = 0; i < this._temporaryCollectedObjects.Count; i++)
				{
					object obj = this._temporaryCollectedObjects[i];
					if (obj != null)
					{
						this._objectsToIterate.Enqueue(obj);
					}
				}
				this._temporaryCollectedObjects.Clear();
			}
		}

		private void CollectObjects(object parent)
		{
			if (!this._idsOfChildObjects.ContainsKey(parent))
			{
				int count = this._childObjects.Count;
				this._childObjects.Add(parent);
				this._idsOfChildObjects.Add(parent, count);
				Type type = parent.GetType();
				TypeDefinition classDefinition = this.DefinitionContext.GetClassDefinition(type);
				if (classDefinition == null)
				{
					throw new Exception("Could not find type definition of type: " + type);
				}
				ObjectSaveData.GetChildObjects(this, classDefinition, parent, this._temporaryCollectedObjects);
				for (int i = 0; i < this._temporaryCollectedObjects.Count; i++)
				{
					object obj = this._temporaryCollectedObjects[i];
					if (obj != null)
					{
						this._objectsToIterate.Enqueue(obj);
					}
				}
				this._temporaryCollectedObjects.Clear();
			}
		}

		public void AddStrings(List<string> texts)
		{
			object locker = this._locker;
			lock (locker)
			{
				for (int i = 0; i < texts.Count; i++)
				{
					string text = texts[i];
					if (text != null && !this._idsOfStrings.ContainsKey(text))
					{
						int count = this._strings.Count;
						this._idsOfStrings.Add(text, count);
						this._strings.Add(text);
					}
				}
			}
		}

		public int AddOrGetStringId(string text)
		{
			int num = -1;
			if (text == null)
			{
				num = -1;
			}
			else
			{
				object locker = this._locker;
				lock (locker)
				{
					int num2;
					if (this._idsOfStrings.TryGetValue(text, out num2))
					{
						num = num2;
					}
					else
					{
						num = this._strings.Count;
						this._idsOfStrings.Add(text, num);
						this._strings.Add(text);
					}
				}
			}
			return num;
		}

		public int GetObjectId(object target)
		{
			if (!this._idsOfChildObjects.ContainsKey(target))
			{
				Debug.Print(string.Format("SAVE ERROR. Cant find {0} with type {1}", target, target.GetType()), 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.FailedAssert("SAVE ERROR. Cant find target object on save", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\Save\\SaveContext.cs", "GetObjectId", 261);
			}
			return this._idsOfChildObjects[target];
		}

		public int GetContainerId(object target)
		{
			return this._idsOfChildContainers[target];
		}

		public int GetStringId(string target)
		{
			if (target == null)
			{
				return -1;
			}
			int num = -1;
			object locker = this._locker;
			lock (locker)
			{
				num = this._idsOfStrings[target];
			}
			return num;
		}

		private static void SaveStringTo(SaveEntryFolder stringsFolder, int id, string value)
		{
			BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
			binaryWriter.WriteString(value);
			stringsFolder.CreateEntry(new EntryId(id, SaveEntryExtension.Txt)).FillFrom(binaryWriter);
			BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
		}

		public bool Save(object target, MetaData metaData, out string errorMessage)
		{
			errorMessage = "";
			bool flag = false;
			if (SaveContext.EnableSaveStatistics)
			{
				SaveContext._typeStatistics = new Dictionary<string, ValueTuple<int, int, int, long>>();
				SaveContext._containerStatistics = new Dictionary<string, ValueTuple<int, int, int, int, long>>();
			}
			try
			{
				this.RootObject = target;
				using (new PerformanceTestBlock("SaveContext::Save"))
				{
					BinaryWriterFactory.Initialize();
					this.CollectObjects();
					ArchiveConcurrentSerializer headerSerializer = new ArchiveConcurrentSerializer();
					byte[][] objectData = new byte[this._childObjects.Count][];
					using (new PerformanceTestBlock("SaveContext::Saving Objects"))
					{
						if (!SaveContext.EnableSaveStatistics)
						{
							TWParallel.For(0, this._childObjects.Count, delegate(int startInclusive, int endExclusive)
							{
								for (int l = startInclusive; l < endExclusive; l++)
								{
									this.SaveSingleObject(headerSerializer, objectData, l);
								}
							}, 16);
						}
						else
						{
							for (int i = 0; i < this._childObjects.Count; i++)
							{
								this.SaveSingleObject(headerSerializer, objectData, i);
							}
						}
					}
					byte[][] containerData = new byte[this._childContainers.Count][];
					using (new PerformanceTestBlock("SaveContext::Saving Containers"))
					{
						if (!SaveContext.EnableSaveStatistics)
						{
							TWParallel.For(0, this._childContainers.Count, delegate(int startInclusive, int endExclusive)
							{
								for (int m = startInclusive; m < endExclusive; m++)
								{
									this.SaveSingleContainer(headerSerializer, containerData, m);
								}
							}, 16);
						}
						else
						{
							for (int j = 0; j < this._childContainers.Count; j++)
							{
								this.SaveSingleContainer(headerSerializer, containerData, j);
							}
						}
					}
					SaveEntryFolder saveEntryFolder = SaveEntryFolder.CreateRootFolder();
					BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
					binaryWriter.WriteInt(this._idsOfChildObjects.Count);
					binaryWriter.WriteInt(this._strings.Count);
					binaryWriter.WriteInt(this._idsOfChildContainers.Count);
					saveEntryFolder.CreateEntry(new EntryId(-1, SaveEntryExtension.Config)).FillFrom(binaryWriter);
					headerSerializer.SerializeFolderConcurrent(saveEntryFolder);
					BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
					ArchiveSerializer archiveSerializer = new ArchiveSerializer();
					SaveEntryFolder saveEntryFolder2 = SaveEntryFolder.CreateRootFolder();
					SaveEntryFolder saveEntryFolder3 = archiveSerializer.CreateFolder(saveEntryFolder2, new FolderId(-1, SaveFolderExtension.Strings), this._strings.Count);
					for (int k = 0; k < this._strings.Count; k++)
					{
						string text = this._strings[k];
						SaveContext.SaveStringTo(saveEntryFolder3, k, text);
					}
					archiveSerializer.SerializeFolder(saveEntryFolder2);
					byte[] array = headerSerializer.FinalizeAndGetBinaryDataConcurrent();
					byte[] array2 = archiveSerializer.FinalizeAndGetBinaryData();
					this.SaveData = new GameData(array, array2, objectData, containerData);
					BinaryWriterFactory.Release();
				}
				flag = true;
			}
			catch (Exception ex)
			{
				errorMessage = "SaveContext Error\n";
				errorMessage += ex.Message;
				flag = false;
			}
			return flag;
		}

		private void SaveSingleObject(ArchiveConcurrentSerializer headerSerializer, byte[][] objectData, int id)
		{
			object obj = this._childObjects[id];
			ArchiveSerializer archiveSerializer = new ArchiveSerializer();
			SaveEntryFolder saveEntryFolder = SaveEntryFolder.CreateRootFolder();
			SaveEntryFolder saveEntryFolder2 = SaveEntryFolder.CreateRootFolder();
			ObjectSaveData objectSaveData = new ObjectSaveData(this, id, obj, true);
			objectSaveData.CollectStructs();
			objectSaveData.CollectMembers();
			objectSaveData.CollectStrings();
			objectSaveData.SaveHeaderTo(saveEntryFolder2, headerSerializer);
			objectSaveData.SaveTo(saveEntryFolder, archiveSerializer);
			headerSerializer.SerializeFolderConcurrent(saveEntryFolder2);
			archiveSerializer.SerializeFolder(saveEntryFolder);
			byte[] array = archiveSerializer.FinalizeAndGetBinaryData();
			objectData[id] = array;
			if (SaveContext.EnableSaveStatistics)
			{
				string name = objectSaveData.Type.Name;
				int num = array.Length;
				ValueTuple<int, int, int, long> valueTuple;
				if (SaveContext._typeStatistics.TryGetValue(name, out valueTuple))
				{
					SaveContext._typeStatistics[name] = new ValueTuple<int, int, int, long>(valueTuple.Item1 + 1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4 + (long)num);
					return;
				}
				SaveContext._typeStatistics[name] = new ValueTuple<int, int, int, long>(1, objectSaveData.FieldCount, objectSaveData.PropertyCount, (long)num);
			}
		}

		private void SaveSingleContainer(ArchiveConcurrentSerializer headerSerializer, byte[][] containerData, int id)
		{
			object obj = this._childContainers[id];
			ArchiveSerializer archiveSerializer = new ArchiveSerializer();
			SaveEntryFolder saveEntryFolder = SaveEntryFolder.CreateRootFolder();
			SaveEntryFolder saveEntryFolder2 = SaveEntryFolder.CreateRootFolder();
			ContainerType containerType;
			obj.GetType().IsContainer(out containerType);
			ContainerSaveData containerSaveData = new ContainerSaveData(this, id, obj, containerType);
			containerSaveData.CollectChildren();
			containerSaveData.CollectStructs();
			containerSaveData.CollectMembers();
			containerSaveData.CollectStrings();
			containerSaveData.SaveHeaderTo(saveEntryFolder2, headerSerializer);
			containerSaveData.SaveTo(saveEntryFolder, archiveSerializer);
			headerSerializer.SerializeFolderConcurrent(saveEntryFolder2);
			archiveSerializer.SerializeFolder(saveEntryFolder);
			byte[] array = archiveSerializer.FinalizeAndGetBinaryData();
			containerData[id] = array;
			if (SaveContext.EnableSaveStatistics)
			{
				string containerName = this.GetContainerName(containerSaveData.Type);
				long num = (long)array.Length;
				ValueTuple<int, int, int, int, long> valueTuple;
				if (SaveContext._containerStatistics.TryGetValue(containerName, out valueTuple))
				{
					SaveContext._containerStatistics[containerName] = new ValueTuple<int, int, int, int, long>(valueTuple.Item1 + 1, valueTuple.Item2 + containerSaveData.GetElementCount(), valueTuple.Item3, valueTuple.Item4, SaveContext._containerStatistics[containerName].Item5 + num);
					return;
				}
				SaveContext._containerStatistics[containerName] = new ValueTuple<int, int, int, int, long>(1, containerSaveData.GetElementCount(), containerSaveData.ElementFieldCount, containerSaveData.ElementPropertyCount, num);
			}
		}

		private string GetContainerName(Type t)
		{
			string text = t.Name;
			foreach (Type type in t.GetGenericArguments())
			{
				if (t.IsContainer())
				{
					text += this.GetContainerName(type);
				}
				else
				{
					text = text + type.Name + ".";
				}
			}
			return text.TrimEnd(new char[] { '.' });
		}

		private List<object> _childObjects;

		private Dictionary<object, int> _idsOfChildObjects;

		private List<object> _childContainers;

		private Dictionary<object, int> _idsOfChildContainers;

		private List<string> _strings;

		private Dictionary<string, int> _idsOfStrings;

		private List<object> _temporaryCollectedObjects;

		private object _locker;

		private static Dictionary<string, ValueTuple<int, int, int, long>> _typeStatistics;

		private static Dictionary<string, ValueTuple<int, int, int, int, long>> _containerStatistics;

		private Queue<object> _objectsToIterate;

		public struct SaveStatistics
		{
			public SaveStatistics(Dictionary<string, ValueTuple<int, int, int, long>> typeStatistics, Dictionary<string, ValueTuple<int, int, int, int, long>> containerStatistics)
			{
				this._typeStatistics = typeStatistics;
				this._containerStatistics = containerStatistics;
			}

			public ValueTuple<int, int, int, long> GetObjectCounts(string key)
			{
				if (this._typeStatistics.ContainsKey(key))
				{
					return this._typeStatistics[key];
				}
				return default(ValueTuple<int, int, int, long>);
			}

			public ValueTuple<int, int, int, int, long> GetContainerCounts(string key)
			{
				return this._containerStatistics[key];
			}

			public long GetContainerSize(string key)
			{
				return this._containerStatistics[key].Item5;
			}

			public List<string> GetTypeKeys()
			{
				return this._typeStatistics.Keys.ToList<string>();
			}

			public List<string> GetContainerKeys()
			{
				return this._containerStatistics.Keys.ToList<string>();
			}

			private Dictionary<string, ValueTuple<int, int, int, long>> _typeStatistics;

			private Dictionary<string, ValueTuple<int, int, int, int, long>> _containerStatistics;
		}
	}
}
