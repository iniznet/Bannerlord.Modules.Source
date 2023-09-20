using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	public class LoadContext
	{
		public object RootObject { get; private set; }

		public DefinitionContext DefinitionContext { get; private set; }

		public ISaveDriver Driver { get; private set; }

		public LoadContext(DefinitionContext definitionContext, ISaveDriver driver)
		{
			this.DefinitionContext = definitionContext;
			this._objectHeaderLoadDatas = null;
			this._containerHeaderLoadDatas = null;
			this._strings = null;
			this.Driver = driver;
		}

		internal static ObjectLoadData CreateLoadData(LoadData loadData, int i, ObjectHeaderLoadData header)
		{
			ArchiveDeserializer archiveDeserializer = new ArchiveDeserializer();
			archiveDeserializer.LoadFrom(loadData.GameData.ObjectData[i]);
			SaveEntryFolder rootFolder = archiveDeserializer.RootFolder;
			ObjectLoadData objectLoadData = new ObjectLoadData(header);
			SaveEntryFolder childFolder = rootFolder.GetChildFolder(new FolderId(i, SaveFolderExtension.Object));
			objectLoadData.InitializeReaders(childFolder);
			objectLoadData.FillCreatedObject();
			objectLoadData.Read();
			objectLoadData.FillObject();
			return objectLoadData;
		}

		public bool Load(LoadData loadData, bool loadAsLateInitialize)
		{
			bool flag = false;
			try
			{
				using (new PerformanceTestBlock("LoadContext::Load Headers"))
				{
					using (new PerformanceTestBlock("LoadContext::Load And Create Header"))
					{
						ArchiveDeserializer archiveDeserializer = new ArchiveDeserializer();
						archiveDeserializer.LoadFrom(loadData.GameData.Header);
						SaveEntryFolder headerRootFolder = archiveDeserializer.RootFolder;
						BinaryReader binaryReader = headerRootFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Config)).GetBinaryReader();
						this._objectCount = binaryReader.ReadInt();
						this._stringCount = binaryReader.ReadInt();
						this._containerCount = binaryReader.ReadInt();
						this._objectHeaderLoadDatas = new ObjectHeaderLoadData[this._objectCount];
						this._containerHeaderLoadDatas = new ContainerHeaderLoadData[this._containerCount];
						this._strings = new string[this._stringCount];
						TWParallel.For(0, this._objectCount, delegate(int startInclusive, int endExclusive)
						{
							for (int l = startInclusive; l < endExclusive; l++)
							{
								ObjectHeaderLoadData objectHeaderLoadData3 = new ObjectHeaderLoadData(this, l);
								SaveEntryFolder childFolder = headerRootFolder.GetChildFolder(new FolderId(l, SaveFolderExtension.Object));
								objectHeaderLoadData3.InitialieReaders(childFolder);
								this._objectHeaderLoadDatas[l] = objectHeaderLoadData3;
							}
						}, 16);
						TWParallel.For(0, this._containerCount, delegate(int startInclusive, int endExclusive)
						{
							for (int m = startInclusive; m < endExclusive; m++)
							{
								ContainerHeaderLoadData containerHeaderLoadData2 = new ContainerHeaderLoadData(this, m);
								SaveEntryFolder childFolder2 = headerRootFolder.GetChildFolder(new FolderId(m, SaveFolderExtension.Container));
								containerHeaderLoadData2.InitialieReaders(childFolder2);
								this._containerHeaderLoadDatas[m] = containerHeaderLoadData2;
							}
						}, 16);
					}
					using (new PerformanceTestBlock("LoadContext::Create Objects"))
					{
						foreach (ObjectHeaderLoadData objectHeaderLoadData in this._objectHeaderLoadDatas)
						{
							objectHeaderLoadData.CreateObject();
							if (objectHeaderLoadData.Id == 0)
							{
								this.RootObject = objectHeaderLoadData.Target;
							}
						}
						foreach (ContainerHeaderLoadData containerHeaderLoadData in this._containerHeaderLoadDatas)
						{
							if (containerHeaderLoadData.GetObjectTypeDefinition())
							{
								containerHeaderLoadData.CreateObject();
							}
						}
					}
				}
				GC.Collect();
				using (new PerformanceTestBlock("LoadContext::Load Strings"))
				{
					ArchiveDeserializer archiveDeserializer2 = new ArchiveDeserializer();
					archiveDeserializer2.LoadFrom(loadData.GameData.Strings);
					for (int j = 0; j < this._stringCount; j++)
					{
						string text = LoadContext.LoadString(archiveDeserializer2, j);
						this._strings[j] = text;
					}
				}
				GC.Collect();
				using (new PerformanceTestBlock("LoadContext::Resolve Objects"))
				{
					for (int k = 0; k < this._objectHeaderLoadDatas.Length; k++)
					{
						ObjectHeaderLoadData objectHeaderLoadData2 = this._objectHeaderLoadDatas[k];
						TypeDefinition typeDefinition = objectHeaderLoadData2.TypeDefinition;
						if (typeDefinition != null)
						{
							object loadedObject = objectHeaderLoadData2.LoadedObject;
							if (typeDefinition.CheckIfRequiresAdvancedResolving(loadedObject))
							{
								ObjectLoadData objectLoadData = LoadContext.CreateLoadData(loadData, k, objectHeaderLoadData2);
								objectHeaderLoadData2.AdvancedResolveObject(loadData.MetaData, objectLoadData);
							}
							else
							{
								objectHeaderLoadData2.ResolveObject();
							}
						}
					}
				}
				GC.Collect();
				using (new PerformanceTestBlock("LoadContext::Load Object Datas"))
				{
					TWParallel.For(0, this._objectCount, delegate(int startInclusive, int endExclusive)
					{
						for (int n = startInclusive; n < endExclusive; n++)
						{
							ObjectHeaderLoadData objectHeaderLoadData4 = this._objectHeaderLoadDatas[n];
							if (objectHeaderLoadData4.Target == objectHeaderLoadData4.LoadedObject)
							{
								LoadContext.CreateLoadData(loadData, n, objectHeaderLoadData4);
							}
						}
					}, 16);
				}
				using (new PerformanceTestBlock("LoadContext::Load Container Datas"))
				{
					TWParallel.For(0, this._containerCount, delegate(int startInclusive, int endExclusive)
					{
						for (int num = startInclusive; num < endExclusive; num++)
						{
							byte[] array = loadData.GameData.ContainerData[num];
							ArchiveDeserializer archiveDeserializer3 = new ArchiveDeserializer();
							archiveDeserializer3.LoadFrom(array);
							SaveEntryFolder rootFolder = archiveDeserializer3.RootFolder;
							ContainerLoadData containerLoadData = new ContainerLoadData(this._containerHeaderLoadDatas[num]);
							SaveEntryFolder childFolder3 = rootFolder.GetChildFolder(new FolderId(num, SaveFolderExtension.Container));
							containerLoadData.InitializeReaders(childFolder3);
							containerLoadData.FillCreatedObject();
							containerLoadData.Read();
							containerLoadData.FillObject();
						}
					}, 16);
				}
				GC.Collect();
				if (!loadAsLateInitialize)
				{
					this.CreateLoadCallbackInitializator(loadData).InitializeObjects();
				}
				flag = true;
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				flag = false;
			}
			return flag;
		}

		internal LoadCallbackInitializator CreateLoadCallbackInitializator(LoadData loadData)
		{
			return new LoadCallbackInitializator(loadData, this._objectHeaderLoadDatas, this._objectCount);
		}

		private static string LoadString(ArchiveDeserializer saveArchive, int id)
		{
			return saveArchive.RootFolder.GetChildFolder(new FolderId(-1, SaveFolderExtension.Strings)).GetEntry(new EntryId(id, SaveEntryExtension.Txt)).GetBinaryReader()
				.ReadString();
		}

		public static bool TryConvertType(Type sourceType, Type targetType, ref object data)
		{
			if (LoadContext.<TryConvertType>g__isNum|23_2(sourceType) && LoadContext.<TryConvertType>g__isNum|23_2(targetType))
			{
				try
				{
					data = Convert.ChangeType(data, targetType);
					return true;
				}
				catch
				{
					return false;
				}
			}
			if (LoadContext.<TryConvertType>g__isNum|23_2(sourceType) && targetType == typeof(string))
			{
				if (LoadContext.<TryConvertType>g__isInt|23_0(sourceType))
				{
					data = Convert.ToInt64(data).ToString();
				}
				else if (LoadContext.<TryConvertType>g__isFloat|23_1(sourceType))
				{
					data = Convert.ToDouble(data).ToString(CultureInfo.InvariantCulture);
				}
				return true;
			}
			if (sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == typeof(List<>) && targetType.IsGenericType)
			{
				targetType.GetGenericTypeDefinition() == typeof(MBList<>);
			}
			return false;
		}

		public ObjectHeaderLoadData GetObjectWithId(int id)
		{
			ObjectHeaderLoadData objectHeaderLoadData = null;
			if (id != -1)
			{
				objectHeaderLoadData = this._objectHeaderLoadDatas[id];
			}
			return objectHeaderLoadData;
		}

		public ContainerHeaderLoadData GetContainerWithId(int id)
		{
			ContainerHeaderLoadData containerHeaderLoadData = null;
			if (id != -1)
			{
				containerHeaderLoadData = this._containerHeaderLoadDatas[id];
			}
			return containerHeaderLoadData;
		}

		public string GetStringWithId(int id)
		{
			string text = null;
			if (id != -1)
			{
				text = this._strings[id];
			}
			return text;
		}

		[CompilerGenerated]
		internal static bool <TryConvertType>g__isInt|23_0(Type type)
		{
			return type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(ulong) || type == typeof(uint) || type == typeof(ushort);
		}

		[CompilerGenerated]
		internal static bool <TryConvertType>g__isFloat|23_1(Type type)
		{
			return type == typeof(double) || type == typeof(float);
		}

		[CompilerGenerated]
		internal static bool <TryConvertType>g__isNum|23_2(Type type)
		{
			return LoadContext.<TryConvertType>g__isInt|23_0(type) || LoadContext.<TryConvertType>g__isFloat|23_1(type);
		}

		private int _objectCount;

		private int _stringCount;

		private int _containerCount;

		private ObjectHeaderLoadData[] _objectHeaderLoadDatas;

		private ContainerHeaderLoadData[] _containerHeaderLoadDatas;

		private string[] _strings;
	}
}
